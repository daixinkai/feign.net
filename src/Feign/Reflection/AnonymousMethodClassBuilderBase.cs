using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    abstract class AnonymousMethodClassBuilderBase
    {
        public class Comparer
        {
            public MethodInfo Method { get; set; }
            public Type TargetType { get; set; }
            public ParameterInfo[] Parameters { get; set; }

            public override bool Equals(object obj)
            {
                Comparer comparer = obj as Comparer;
                if (comparer == null)
                {
                    return false;
                }
                if (TargetType != comparer.TargetType)
                {
                    return false;
                }

                if (Method.Name != comparer.Method.Name)
                {
                    return false;
                }

                ParameterInfo[] parameters = Parameters ?? Method.GetParameters();
                ParameterInfo[] comparerParameters = comparer.Parameters ?? comparer.Method.GetParameters();

                if (parameters.Length != comparerParameters.Length)
                {
                    return false;
                }
                for (int i = 0; i < Method.GetParameters().Length; i++)
                {
                    if (parameters[i].ParameterType != comparerParameters[i].ParameterType)
                    {
                        return false;
                    }
                }

                if (Method.ReturnType != comparer.Method.ReturnType)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int hashCode = 0;
                if (TargetType != null)
                {
                    hashCode += TargetType.GetHashCode();
                }
                hashCode += Method.Name.GetHashCode();
                ParameterInfo[] parameters = Parameters ?? Method.GetParameters();
                foreach (var item in parameters)
                {
                    hashCode += item.ParameterType.GetHashCode();
                }
                if (Method.ReturnType != null)
                {
                    hashCode += Method.ReturnType.GetHashCode();
                }
                return hashCode;
            }
        }

        protected static FieldBuilder CreateField(TypeBuilder typeBuilder, string fieldName, Type fieldType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Private);
            return fieldBuilder;
        }

        protected static ConstructorBuilder CreateConstructor(TypeBuilder typeBuilder, List<FieldBuilder> fieldBuilders)
        {
            List<Type> types = fieldBuilders.Select(s => s.FieldType).ToList();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               fieldBuilders.Select(s => s.FieldType).ToArray());

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            if (fieldBuilders.Count > 0)
            {
                for (int i = 0; i < fieldBuilders.Count; i++)
                {
                    constructorIlGenerator.Emit(OpCodes.Ldarg_0);
                    constructorIlGenerator.Emit(OpCodes.Ldarg_S, (i + 1));
                    constructorIlGenerator.Emit(OpCodes.Stfld, fieldBuilders[i]);
                }
            }
            constructorIlGenerator.Emit(OpCodes.Ret);
            return constructorBuilder;
        }

        protected static MethodBuilder CreateMethod(TypeBuilder typeBuilder, MethodInfo method, List<FieldBuilder> fieldBuilders)
        {

            MethodAttributes methodAttributes = MethodAttributes.Public;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, Type.EmptyTypes);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            if (fieldBuilders.Count > 0)
            {
                for (int i = 0; i < fieldBuilders.Count; i++)
                {
                    iLGenerator.Emit(OpCodes.Ldarg_0); // this
                    iLGenerator.Emit(OpCodes.Ldfld, fieldBuilders[i]);
                }
            }

            if (method.IsStatic)
            {
                iLGenerator.Emit(OpCodes.Call, method);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Callvirt, method);
            }

            iLGenerator.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        protected static Tuple<Type, ConstructorInfo, MethodInfo> FillType(TypeBuilder typeBuilder, Comparer comparer)
        {
            // field
            ParameterInfo[] parameters = comparer.Parameters ?? comparer.Method.GetParameters();
            List<FieldBuilder> fieldBuilders = new List<FieldBuilder>();
            if (!comparer.Method.IsStatic)
            {
                if (comparer.TargetType == null)
                {
                    throw new ArgumentException("targetType");
                }
                fieldBuilders.Add(CreateField(typeBuilder, "_this_" + comparer.TargetType.Name, comparer.TargetType));
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                fieldBuilders.Add(CreateField(typeBuilder, "_" + parameters[i].Name, parameters[i].ParameterType));
            }

            //constructor
            ConstructorBuilder constructorBuilder = CreateConstructor(typeBuilder, fieldBuilders);

            MethodBuilder methodBuilder = CreateMethod(typeBuilder, comparer.Method, fieldBuilders);

            return Tuple.Create(typeBuilder.CreateTypeInfo().AsType(), (ConstructorInfo)constructorBuilder, (MethodInfo)methodBuilder);
        }

        protected static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, string typeName, Type parentType)
        {
            return moduleBuilder.DefineType(typeName,
                          //TypeAttributes.Public |
                          TypeAttributes.NotPublic |
                          TypeAttributes.Class |
                          TypeAttributes.AutoClass |
                          TypeAttributes.AnsiClass |
                          TypeAttributes.BeforeFieldInit |
                          TypeAttributes.AutoLayout,
                          parentType);
        }

    }
}
