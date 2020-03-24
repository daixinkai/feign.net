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

        static void GetPropertiesFromBaseInterfaces(this Type type, List<PropertyInfo> properties)
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

        public static void DefineAutoProperty(this TypeBuilder typeBuilder, Type type, PropertyInfo property)
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

        public static void DefineExplicitAutoProperty(this TypeBuilder typeBuilder, Type type, PropertyInfo property)
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
                typeBuilder.DefineMethodOverride(propertyGet, property.GetMethod);
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
                typeBuilder.DefineMethodOverride(propertySet, property.SetMethod);
                propertyBuilder.SetSetMethod(propertySet);
            }

            propertyBuilder.CopyCustomAttributes(property);
        }

        public static void DefineReadOnlyProperty(this TypeBuilder typeBuilder, Type interfaceType, string propertyName, string propertyValue)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, typeof(string), Type.EmptyTypes);
            MethodBuilder propertyGet = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(string), Type.EmptyTypes);
            ILGenerator iLGenerator = propertyGet.GetILGenerator();
            if (propertyValue == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, propertyValue);
            }
            iLGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGet);
        }



    }
}
