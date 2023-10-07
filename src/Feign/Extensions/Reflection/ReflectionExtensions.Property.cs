using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    partial class ReflectionExtensions
    {
        public static PropertyInfo[] GetPropertiesIncludingBaseInterfaces(this Type type)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());
            GetPropertiesFromBaseInterfaces(type, properties);
            return properties.ToArray();
        }

        private static void GetPropertiesFromBaseInterfaces(this Type type, List<PropertyInfo> properties)
        {
            foreach (var item in type.GetInterfaces())
            {
                foreach (var property in item.GetProperties())
                {
                    if (!properties.Contains(property))
                    {
                        properties.Add(property);
                    }
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                GetPropertiesFromBaseInterfaces(item, properties);
            }
        }

        public static void DefineAutoProperty(this TypeBuilder typeBuilder, PropertyInfo property)
        {

            MethodAttributes methodAttributes =
                MethodAttributes.Public
                | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final;

            string fieldName = "<" + property.Name + ">k__BackingField";
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, property.PropertyType, FieldAttributes.Private);
            fieldBuilder.SetCustomAttribute(() => new CompilerGeneratedAttribute());
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, Type.EmptyTypes);
            if (property.CanRead)
            {
                MethodBuilder propertyGet = typeBuilder.DefineMethod("get_" + property.Name, methodAttributes, property.PropertyType, Type.EmptyTypes);
                propertyGet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                ILGenerator iLGenerator = propertyGet.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                iLGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(propertyGet);
            }

            if (property.CanWrite)
            {
                MethodBuilder propertySet = typeBuilder.DefineMethod("set_" + property.Name, methodAttributes, typeof(void), new Type[] { property.PropertyType });
                propertySet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                propertySet.DefineParameter(1, ParameterAttributes.None, "value");
                ILGenerator iLGenerator = propertySet.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                iLGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetSetMethod(propertySet);
            }

            propertyBuilder.CopyCustomAttributes(property);
        }

        public static void DefineExplicitAutoProperty(this TypeBuilder typeBuilder, PropertyInfo property)
        {
            if (property.DeclaringType == null || !property.DeclaringType.IsInterface)
            {
                throw new ArgumentException(nameof(property));
            }

            MethodAttributes methodAttributes =
                MethodAttributes.Private
                | MethodAttributes.SpecialName
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final;

            //string prefix = property.DeclaringType.FullName + ".";
            string prefix = property.DeclaringType.GetFullName() + ".";

            string fieldName = "<" + prefix + property.Name + ">k__BackingField";
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, property.PropertyType, FieldAttributes.Private);
            fieldBuilder.SetCustomAttribute(() => new CompilerGeneratedAttribute());
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(prefix + property.Name, PropertyAttributes.None, property.PropertyType, Type.EmptyTypes);
            if (property.CanRead)
            {
                MethodBuilder propertyGet = typeBuilder.DefineMethod(prefix + "get_" + property.Name, methodAttributes, property.PropertyType, Type.EmptyTypes);
                propertyGet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                ILGenerator iLGenerator = propertyGet.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                iLGenerator.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(propertyGet, property.GetMethod!);
                propertyBuilder.SetGetMethod(propertyGet);
            }

            if (property.CanWrite)
            {
                MethodBuilder propertySet = typeBuilder.DefineMethod(prefix + "set_" + property.Name, methodAttributes, typeof(void), new Type[] { property.PropertyType });
                propertySet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                propertySet.DefineParameter(1, ParameterAttributes.None, "value");
                ILGenerator iLGenerator = propertySet.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
                iLGenerator.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(propertySet, property.SetMethod!);
                propertyBuilder.SetSetMethod(propertySet);
            }

            propertyBuilder.CopyCustomAttributes(property);
        }

        public static void OverrideProperty(this TypeBuilder typeBuilder, PropertyInfo property, Action<ILGenerator>? getterInvoker, Action<ILGenerator>? setterInvoker)
        {
            MethodAttributes methodAttributes = MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, Type.EmptyTypes);
            if (property.CanRead)
            {
                MethodAttributes scope = property.GetMethod!.Attributes.HasFlag(MethodAttributes.Public) ? MethodAttributes.Public : MethodAttributes.Family;
                MethodBuilder propertyGet = typeBuilder.DefineMethod("get_" + property.Name, scope | methodAttributes, property.PropertyType, Type.EmptyTypes);
                //propertyGet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                ILGenerator iLGenerator = propertyGet.GetILGenerator();
                getterInvoker?.Invoke(iLGenerator);
                if (!property.GetMethod.IsAbstract)
                {
                    typeBuilder.DefineMethodOverride(propertyGet, property.GetMethod);
                }
                propertyBuilder.SetGetMethod(propertyGet);
            }

            if (property.CanWrite)
            {
                MethodAttributes scope = property.SetMethod!.Attributes.HasFlag(MethodAttributes.Public) ? MethodAttributes.Public : MethodAttributes.Family;
                MethodBuilder propertySet = typeBuilder.DefineMethod("set_" + property.Name, scope | methodAttributes, typeof(void), new Type[] { property.PropertyType });
                //propertySet.SetCustomAttribute(() => new CompilerGeneratedAttribute());
                propertySet.DefineParameter(1, ParameterAttributes.None, "value");
                ILGenerator iLGenerator = propertySet.GetILGenerator();
                setterInvoker?.Invoke(iLGenerator);
                iLGenerator.Emit(OpCodes.Ret);
                if (!property.GetMethod!.IsAbstract)
                {
                    typeBuilder.DefineMethodOverride(propertySet, property.SetMethod);
                }
                propertyBuilder.SetSetMethod(propertySet);
            }

            propertyBuilder.CopyCustomAttributes(property);

        }

    }
}
