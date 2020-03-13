using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class ReflectionExtensions
    {

        //public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        //{
        //    return type.GetCustomAttributes<T>().
        //      Union(type.GetInterfaces().
        //      SelectMany(interfaceType => interfaceType.GetCustomAttributes<T>())).
        //      Distinct();
        //}

        //public static T GetCustomAttributeIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        //{
        //    return type.GetCustomAttributesIncludingBaseInterfaces<T>().FirstOrDefault();
        //}


        public static T GetCustomAttributeIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
            {
                return attribute;
            }
            return GetCustomAttributeFromBaseInterfaces<T>(type);
        }

        static T GetCustomAttributeFromBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T attribute = null;
            foreach (var item in type.GetInterfaces())
            {
                attribute = item.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                attribute = GetCustomAttributeFromBaseInterfaces<T>(item);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }


        public static bool IsDefinedIncludingBaseInterfaces<T>(this Type type)
        {
            return type.IsDefined(typeof(T)) || type.GetInterfaces().Any(s => IsDefinedIncludingBaseInterfaces<T>(s));
        }

        public static MethodInfo[] GetMethodsIncludingBaseInterfaces(this Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods().Where(s => !s.IsSpecialName));
            GetMethodsFromBaseInterfaces(type, methods);
            return methods.ToArray();
        }

        static void GetMethodsFromBaseInterfaces(this Type type, List<MethodInfo> methods)
        {
            foreach (var item in type.GetInterfaces())
            {
                foreach (var method in item.GetMethods().Where(s => !s.IsSpecialName))
                {
                    if (!methods.Contains(method))
                    {
                        methods.Add(method);
                    }
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                GetMethodsFromBaseInterfaces(item, methods);
            }
        }

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

        public static bool IsVoidMethod(this MethodInfo method)
        {
            return method.ReturnType == null || method.ReturnType == typeof(void);
        }

        public static bool IsTaskMethod(this MethodInfo method)
        {
            return method.ReturnType == typeof(Task) || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        //public static Type GetReturnType(this MethodInfo method)
        //{
        //    if (!IsTaskMethod(method))
        //    {
        //        return method.ReturnType;
        //    }

        //    if (method.ReturnType.IsGenericType)
        //    {
        //        return method.ReturnType.GetGenericArguments()[0];
        //    }
        //    return null;
        //    //return method.ReturnType;
        //}

        public static void CallBaseTypeDefaultConstructor(this ILGenerator constructorIlGenerator, Type baseType)
        {
            var defaultConstructor = baseType.GetConstructors().Where(s => s.GetParameters().Length == 0).FirstOrDefault();
            if (defaultConstructor == null)
            {
                throw new ArgumentException("The default constructor not found . Type : " + baseType.FullName);
            }
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            constructorIlGenerator.Emit(OpCodes.Call, defaultConstructor);
        }

        public static void CallBaseTypeConstructor(this ILGenerator constructorIlGenerator, ConstructorInfo baseTypeConstructor)
        {
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            for (int i = 1; i <= baseTypeConstructor.GetParameters().Length; i++)
            {
                constructorIlGenerator.Emit(OpCodes.Ldarg_S, i);
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseTypeConstructor);
        }

        public static void BuildAndCallBaseTypeDefaultConstructor(this TypeBuilder typeBuilder)
        {
            Type baseType = (typeBuilder.BaseType ?? typeof(object));
            ConstructorInfo baseConstructorInfo = baseType.GetConstructors().Where(s => s.GetParameters().Length == 0).FirstOrDefault();
            if (baseConstructorInfo == null)
            {
                throw new ArgumentException("The default constructor not found . Type : " + baseType.FullName);
            }
            var parameterTypes = baseConstructorInfo.GetParameters().Select(s => s.ParameterType).ToArray();

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }

        public static void BuildAndCallBaseTypeConstructor(this TypeBuilder typeBuilder, Type[] baseConstructorParameterTypes)
        {
            ConstructorInfo baseConstructorInfo = (typeBuilder.BaseType ?? typeof(object)).GetConstructors().Where(s => Equals(s.GetParameters(), baseConstructorParameterTypes)).FirstOrDefault();
            if (baseConstructorInfo == null)
            {
                throw new ArgumentException("The constructor not found . Type[] : " + string.Join(",", baseConstructorParameterTypes.Select(s => s.FullName)));
            }
            typeBuilder.BuildCallBaseTypeConstructor(baseConstructorInfo);
        }

        public static void BuildCallBaseTypeConstructor(this TypeBuilder typeBuilder, ConstructorInfo baseConstructorInfo)
        {
            var parameters = baseConstructorInfo.GetParameters();
            var parameterTypes = parameters.Select(s => s.ParameterType).ToArray();
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);
            for (int i = 0; i < parameters.Length; i++)
            {
                constructorBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
            }

            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.CallBaseTypeConstructor(baseConstructorInfo);
            constructorIlGenerator.Emit(OpCodes.Ret);

        }

        static bool Equals(Type[] types1, Type[] types2)
        {
            if (types1.Length != types2.Length)
            {
                return false;
            }

            for (int i = 0; i < types1.Length; i++)
            {
                var type1 = types1[i];
                var type2 = types2[i];
                //if (type1.IsArray != type2.IsArray)
                //{
                //    return false;
                //}
                if (types1[i] != types2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static void BuildConstructor(TypeBuilder typeBuilder, ConstructorInfo baseConstructor)
        {
            var parameterTypes = baseConstructor.GetParameters().Select(s => s.ParameterType).ToArray();

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public,
               CallingConventions.Standard,
               parameterTypes);
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                constructorBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameterTypes[i].Name);
            }
            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            for (int i = 1; i <= baseConstructor.GetParameters().Length; i++)
            {
                constructorIlGenerator.Emit(OpCodes.Ldarg_S, i);
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseConstructor);
            constructorIlGenerator.Emit(OpCodes.Ret);
        }
        public static void CopyCustomAttributes(this MethodBuilder methodBuilder, MethodInfo method)
        {
            var datas = CustomAttributeData.GetCustomAttributes(method);
            foreach (var data in datas)
            {
                CustomAttributeBuilder customAttributeBuilder = GetCustomAttributeBuilder(data);
                methodBuilder.SetCustomAttribute(customAttributeBuilder);
            }
        }

        public static void SetCustomAttribute<TAttribute>(this MethodBuilder methodBuilder, Expression<Func<TAttribute>> expression) where TAttribute : Attribute
        {
            methodBuilder.SetCustomAttribute(GetCustomAttributeBuilder(expression));
        }

        public static void SetCustomAttribute<TAttribute>(this PropertyBuilder propertyBuilder, Expression<Func<TAttribute>> expression) where TAttribute : Attribute
        {
            propertyBuilder.SetCustomAttribute(GetCustomAttributeBuilder(expression));
        }

        public static void SetCustomAttribute<TAttribute>(this FieldBuilder fieldBuilder, Expression<Func<TAttribute>> expression) where TAttribute : Attribute
        {
            fieldBuilder.SetCustomAttribute(GetCustomAttributeBuilder(expression));
        }

        public static void CopyCustomAttributes(this ParameterBuilder parameterBuilder, ParameterInfo parameter)
        {
            var datas = CustomAttributeData.GetCustomAttributes(parameter);
            foreach (var data in datas)
            {
                CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(data.Constructor, data.ConstructorArguments.Select(s => s.Value).ToArray());
                parameterBuilder.SetCustomAttribute(customAttributeBuilder);
            }
        }

        public static void CopyCustomAttributes(this PropertyBuilder propertyBuilder, PropertyInfo property)
        {
            var datas = CustomAttributeData.GetCustomAttributes(property);
            foreach (var data in datas)
            {
                CustomAttributeBuilder customAttributeBuilder = GetCustomAttributeBuilder(data);
                propertyBuilder.SetCustomAttribute(customAttributeBuilder);
            }
        }


        static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeData data)
        {
            List<CustomAttributeNamedArgument> propertyArguments = data.NamedArguments.Where(s => !s.IsField).ToList();
            List<CustomAttributeNamedArgument> fieldArguments = data.NamedArguments.Where(s => s.IsField).ToList();
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(
                data.Constructor,
                data.ConstructorArguments.Select(GetArgumentValue).ToArray(),
                propertyArguments.Select(s => (PropertyInfo)s.MemberInfo).ToArray(),
                propertyArguments.Select(s => s.TypedValue.Value).ToArray(),
                fieldArguments.Select(s => (FieldInfo)s.MemberInfo).ToArray(),
                fieldArguments.Select(s => s.TypedValue.Value).ToArray());
            return customAttributeBuilder;
        }

        static object GetArgumentValue(CustomAttributeTypedArgument argument)
        {
            if (argument.ArgumentType.IsArray)
            {
                var values = argument.Value as System.Collections.ObjectModel.ReadOnlyCollection<CustomAttributeTypedArgument>;
                return values.Select(GetArgumentValue).ToArray();
            }
            return argument.Value;
        }

        static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(Expression<Func<TAttribute>> expression) where TAttribute : Attribute
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException(nameof(expression));
            }
            if (expression.Body.NodeType == ExpressionType.New)
            {
                NewExpression newExpression = expression.Body as NewExpression;
                return new CustomAttributeBuilder(
                    newExpression.Constructor,
                    newExpression.Arguments.OfType<ConstantExpression>().Select(s => s.Value).ToArray()
                );
            }
            else if (expression.Body.NodeType == ExpressionType.MemberInit)
            {
                MemberInitExpression memberInitExpression = expression.Body as MemberInitExpression;
                var fields = memberInitExpression.Bindings.OfType<MemberAssignment>().Where(s => s.Member.MemberType == MemberTypes.Field).ToList();
                var properties = memberInitExpression.Bindings.OfType<MemberAssignment>().Where(s => s.Member.MemberType == MemberTypes.Property).ToList();
                return new CustomAttributeBuilder(
                    memberInitExpression.NewExpression.Constructor,
                    memberInitExpression.NewExpression.Arguments.OfType<ConstantExpression>().Select(s => s.Value).ToArray(),
                    properties.Select(s => s.Member).OfType<PropertyInfo>().ToArray(),
                    properties.Select(s => s.Expression).OfType<ConstantExpression>().Select(s => s.Value).ToArray(),
                    fields.Select(s => s.Member).OfType<FieldInfo>().ToArray(),
                    fields.Select(s => s.Expression).OfType<ConstantExpression>().Select(s => s.Value).ToArray()
                 );
            }
            throw new ArgumentException(nameof(expression));
        }

    }
}
