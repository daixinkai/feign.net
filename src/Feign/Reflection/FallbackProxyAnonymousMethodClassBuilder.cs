using Feign.Fallback;
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
    class FallbackProxyAnonymousMethodClassBuilder : AnonymousMethodClassBuilderBase
    {

        static readonly System.Collections.Concurrent.ConcurrentDictionary<Comparer, Tuple<Type, ConstructorInfo, MethodInfo>> _map = new System.Collections.Concurrent.ConcurrentDictionary<Comparer, Tuple<Type, ConstructorInfo, MethodInfo>>();

        public static Tuple<Type, ConstructorInfo, MethodInfo> BuildType(ModuleBuilder moduleBuilder, Type targetType, MethodInfo method)
        {
            return BuildType(moduleBuilder, targetType, method, null);
        }

        public static Tuple<Type, ConstructorInfo, MethodInfo> BuildType(ModuleBuilder moduleBuilder, Type targetType, MethodInfo method, ParameterInfo[] parameters)
        {
            Comparer comparer = new Comparer
            {
                TargetType = targetType,
                Method = method,
                Parameters = parameters
            };
            return _map.GetOrAdd(comparer, key =>
            {
                string typeName = comparer.TargetType.Name;
                typeName += "_" + Guid.NewGuid().ToString("N").ToUpper();
                return BuildNewType(moduleBuilder, typeName, comparer.TargetType.Namespace + ".Anonymous", comparer);
            });
        }

        static Tuple<Type, ConstructorInfo, MethodInfo> BuildNewType(ModuleBuilder moduleBuilder, string typeName, string nameSpace, Comparer comparer)
        {
            string fullName = nameSpace + "." + typeName;
            if (fullName.StartsWith("."))
            {
                fullName = fullName.TrimStart('.');
            }
            TypeBuilder typeBuilder = CreateTypeBuilder(moduleBuilder, fullName, null);
            // field
            ParameterInfo[] parameters = comparer.Parameters ?? comparer.Method.GetParameters();
            List<FieldBuilder> fieldBuilders = new List<FieldBuilder>();
            List<FieldBuilder> parameterFields = new List<FieldBuilder>();
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
                var fieldBuilder = CreateField(typeBuilder, "_" + parameters[i].Name, parameters[i].ParameterType);
                fieldBuilders.Add(fieldBuilder);
                parameterFields.Add(fieldBuilder);
            }
            ////add methodInfo
            //fieldBuilders.Add(CreateField(typeBuilder, "_" + Guid.NewGuid().ToString("N"), typeof(MethodInfo)));

            //constructor
            ConstructorBuilder constructorBuilder = CreateConstructor(typeBuilder, fieldBuilders);
            MethodBuilder methodBuilder = CreateMethod(typeBuilder, comparer.Method, fieldBuilders);
            typeBuilder.AddInterfaceImplementation(typeof(IFallbackProxy));
            //IFallbackTarget

            AddFallbackTarget_GetParameters(typeBuilder, parameters, parameterFields);
            AddFallbackTarget_GetParameterTypes(typeBuilder, parameters);
            AddFallbackTarget_MethodName(typeBuilder, comparer.Method.Name);
            AddFallbackTarget_ReturnType(typeBuilder, comparer.Method.ReturnType);
            return Tuple.Create(typeBuilder.CreateTypeInfo().AsType(), (ConstructorInfo)constructorBuilder, (MethodInfo)methodBuilder);
        }

        static void AddFallbackTarget_GetParameters(TypeBuilder typeBuilder, ParameterInfo[] parameters, List<FieldBuilder> parameterFields)
        {
            MethodInfo getParametersMethod = typeof(IFallbackProxy).GetMethod("GetParameters");
            MethodAttributes methodAttributes =
                    MethodAttributes.Public
                    | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot
                    | MethodAttributes.Virtual
                    | MethodAttributes.Final;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(getParametersMethod.Name, methodAttributes, CallingConventions.Standard, getParametersMethod.ReturnType, Type.EmptyTypes);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            LocalBuilder map = iLGenerator.DeclareLocal(typeof(IDictionary<string, object>));
            iLGenerator.Emit(OpCodes.Newobj, typeof(Dictionary<string, object>).GetConstructor(Type.EmptyTypes));
            iLGenerator.Emit(OpCodes.Stloc, map);
            //iLGenerator.Emit(OpCodes.Pop);
            MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod("Add", new Type[] { typeof(string), typeof(object) });

            for (int i = 0; i < parameterFields.Count; i++)
            {
                iLGenerator.Emit(OpCodes.Ldloc, map);
                iLGenerator.Emit(OpCodes.Ldstr, parameters[i].Name);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, parameterFields[i]);
                if (parameterFields[i].FieldType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, parameterFields[i].FieldType);
                }
                iLGenerator.Emit(OpCodes.Call, addMethod);
            }

            iLGenerator.Emit(OpCodes.Ldloc, map);
            iLGenerator.Emit(OpCodes.Ret);
        }

        static void AddFallbackTarget_GetParameterTypes(TypeBuilder typeBuilder, ParameterInfo[] parameters)
        {
            MethodInfo getParametersMethod = typeof(IFallbackProxy).GetMethod("GetParameterTypes");
            MethodAttributes methodAttributes =
                    MethodAttributes.Public
                    | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot
                    | MethodAttributes.Virtual
                    | MethodAttributes.Final;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(getParametersMethod.Name, methodAttributes, CallingConventions.Standard, getParametersMethod.ReturnType, Type.EmptyTypes);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();

            LocalBuilder types = iLGenerator.DeclareLocal(typeof(Type[]));
            iLGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
            iLGenerator.Emit(OpCodes.Newarr, typeof(Type));
            iLGenerator.Emit(OpCodes.Stloc, types);


            for (int i = 0; i < parameters.Length; i++)
            {
                iLGenerator.Emit(OpCodes.Ldloc, types);
                iLGenerator.Emit(OpCodes.Ldc_I4, i);
                ReflectionHelper.EmitType(iLGenerator, parameters[i].ParameterType);
                iLGenerator.Emit(OpCodes.Stelem_Ref);
            }


            iLGenerator.Emit(OpCodes.Ldloc, types);
            iLGenerator.Emit(OpCodes.Ret);
        }

        static void AddFallbackTarget_MethodName(TypeBuilder typeBuilder, string methodName)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("MethodName", PropertyAttributes.None, typeof(string), Type.EmptyTypes);
            MethodBuilder propertyGet = typeBuilder.DefineMethod("get_MethodName", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(string), Type.EmptyTypes);
            ILGenerator iLGenerator = propertyGet.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldstr, methodName);
            iLGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGet);
        }
        static void AddFallbackTarget_ReturnType(TypeBuilder typeBuilder, Type returnType)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("ReturnType", PropertyAttributes.None, typeof(Type), Type.EmptyTypes);
            MethodBuilder propertyGet = typeBuilder.DefineMethod("get_ReturnType", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(Type), Type.EmptyTypes);
            ILGenerator iLGenerator = propertyGet.GetILGenerator();
            if (returnType == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
                //iLGenerator.Emit(OpCodes.Stloc_0);
                //iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ret);
            }
            else
            {
                var getTypeMethod = typeof(Type).GetMethod("GetTypeFromHandle");
                iLGenerator.Emit(OpCodes.Ldtoken, returnType);
                iLGenerator.Emit(OpCodes.Call, getTypeMethod);
                //iLGenerator.Emit(OpCodes.Stloc_0);
                //iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ret);
            }
            propertyBuilder.SetGetMethod(propertyGet);
        }

        static void AddFallbackTarget_Method(TypeBuilder typeBuilder, MethodInfo method)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Method", PropertyAttributes.None, typeof(MethodInfo), Type.EmptyTypes);
            MethodBuilder propertyGet = typeBuilder.DefineMethod("get_Method", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, typeof(MethodInfo), Type.EmptyTypes);
            ILGenerator iLGenerator = propertyGet.GetILGenerator();
            ReflectionHelper.EmitMethodInfo(iLGenerator, method);
            iLGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGet);
        }

    }
}
