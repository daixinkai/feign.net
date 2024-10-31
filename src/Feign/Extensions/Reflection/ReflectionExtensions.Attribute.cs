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
    partial class ReflectionExtensions
    {
        public static T? GetCustomAttributeIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T? attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
            {
                return attribute;
            }
            return GetCustomAttributeFromBaseInterfaces<T>(type);
        }

        private static T? GetCustomAttributeFromBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T? attribute;
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
                try
                {
                    //Unable to cast object of type 'System.Collections.ObjectModel.ReadOnlyCollection`1[System.Reflection.CustomAttributeTypedArgument]' to type 'System.Array'.
                    CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(data.Constructor, data.ConstructorArguments.Select(static s => s.Value).ToArray());
                    parameterBuilder.SetCustomAttribute(customAttributeBuilder);
                }
                catch
                {
                }
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


        private static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeData data)
        {
            List<CustomAttributeNamedArgument> propertyArguments = data.NamedArguments.Where(static s => !s.IsField).ToList();
            List<CustomAttributeNamedArgument> fieldArguments = data.NamedArguments.Where(static s => s.IsField).ToList();
#pragma warning disable CS8620
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(
                data.Constructor,
                data.ConstructorArguments.Select(GetArgumentValue).ToArray(),
                propertyArguments.Select(static s => (PropertyInfo)s.MemberInfo).ToArray(),
                propertyArguments.Select(static s => s.TypedValue.Value).ToArray(),
                fieldArguments.Select(static s => (FieldInfo)s.MemberInfo).ToArray(),
                fieldArguments.Select(static s => s.TypedValue.Value).ToArray());
#pragma warning restore CS8620
            return customAttributeBuilder;
        }

        private static object? GetArgumentValue(CustomAttributeTypedArgument argument)
        {
            if (argument.ArgumentType.IsArray)
            {
                var values = argument.Value as System.Collections.ObjectModel.ReadOnlyCollection<CustomAttributeTypedArgument>;
                return values!.Select(GetArgumentValue).ToArray();
            }
            return argument.Value;
        }

        private static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(Expression<Func<TAttribute>> expression) where TAttribute : Attribute
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException(nameof(expression));
            }
            if (expression.Body.NodeType == ExpressionType.New)
            {
                var newExpression = expression.Body as NewExpression;
                return new CustomAttributeBuilder(
                    newExpression!.Constructor!,
                    newExpression.Arguments.OfType<ConstantExpression>().Select(static s => s.Value).ToArray()
                );
            }
            else if (expression.Body.NodeType == ExpressionType.MemberInit)
            {
                var memberInitExpression = expression.Body as MemberInitExpression;
                var fields = memberInitExpression!.Bindings.OfType<MemberAssignment>().Where(static s => s.Member.MemberType == MemberTypes.Field).ToList();
                var properties = memberInitExpression.Bindings.OfType<MemberAssignment>().Where(static s => s.Member.MemberType == MemberTypes.Property).ToList();
                return new CustomAttributeBuilder(
                    memberInitExpression.NewExpression!.Constructor!,
                    memberInitExpression.NewExpression.Arguments.OfType<ConstantExpression>().Select(static s => s.Value).ToArray(),
                    properties.Select(static s => s.Member).OfType<PropertyInfo>().ToArray(),
                    properties.Select(static s => s.Expression).OfType<ConstantExpression>().Select(static s => s.Value).ToArray(),
                    fields.Select(static s => s.Member).OfType<FieldInfo>().ToArray(),
                    fields.Select(static s => s.Expression).OfType<ConstantExpression>().Select(static s => s.Value).ToArray()
                 );
            }
            throw new ArgumentException(nameof(expression));
        }


    }
}
