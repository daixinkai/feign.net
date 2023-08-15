//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace Feign
//{
//    internal static class ExpressionExtensions
//    {
//        /// <summary>
//        /// Nullable.HasValue表达式
//        /// </summary>
//        /// <param name="expression"></param>
//        /// <returns></returns>
//        internal static UnaryExpression NullableHasValueExpression(this Expression expression)
//        {
//            return Expression.IsTrue(expression.GetPropertyExpression("HasValue"));
//        }
//        /// <summary>
//        /// 属性表达式
//        /// </summary>
//        /// <param name="expression"></param>
//        /// <param name="propertyName"></param>
//        /// <returns></returns>
//        internal static MemberExpression GetPropertyExpression(this Expression expression, string propertyName)
//        {
//            return Expression.Property(expression, expression.Type.GetProperty(propertyName));
//        }

//        /// <summary>
//        /// Nullable.Value表达式
//        /// </summary>
//        /// <param name="expression"></param>
//        internal static Expression NullableGetValueExpression(this Expression expression)
//        {
//            return expression.GetPropertyExpression("Value");
//        }

//        internal static Tuple<bool, Expression> GetNullableValueExpression(this Expression expression, Type targetType)
//        {
//            Type sourceType = expression.Type;

//            Expression resultExpression = expression;

//            bool result = false;

//            //处理Nullable
//            if (sourceType.IsNullableType())
//            {
//                result = true;
//                //如果源类型是Nullable
//                var targetConvertType = targetType;
//                if (targetType.IsNullableType())
//                {
//                    //目标类型也是Nullable
//                    targetConvertType = targetType.GenericTypeArguments[0];
//                }

//                ParameterExpression parameter = Expression.Variable(targetConvertType, "value");

//                //获取一个设置 TargetProperty 值的表达式
//                resultExpression = Expression.Block(
//                  new ParameterExpression[] { parameter },
//                  Expression.IfThenElse(
//                    expression.NullableHasValueExpression(),
//                      //有值 转换
//                      Expression.Assign(parameter, expression.NullableGetValueExpression().ConvertValueExpression(targetConvertType)),
//                      //无值,默认
//                      Expression.Assign(parameter, Expression.Constant(GetDefaultValue(targetConvertType), targetConvertType))),
//                      Expression.Convert(parameter, targetConvertType));

//            }
//            else if (targetType.IsNullableType())
//            {

//                var targetConvertType = targetType.GenericTypeArguments[0];

//                if (!sourceType.Equals(targetConvertType))
//                {
//                    resultExpression = Expression.Convert(expression, targetConvertType);
//                }

//            }
//            return new Tuple<bool, Expression>(result, resultExpression);
//        }

//        internal static Expression ConvertValueExpression(this Expression expression, Type targetType)
//        {
//            if (ReferenceEquals(expression.Type, targetType))
//            {
//                return expression;
//            }
//            return Expression.Convert(expression, targetType);
//        }

//        internal static object GetDefaultValue(Type type)
//        {
//            if (type.IsValueType)
//            {
//                return Activator.CreateInstance(type);
//            }
//            return null;
//        }

//        internal static Expression IsNotNull(this Expression expression)
//        {
//            return Expression.NotEqual(expression, Expression.Constant(null));
//        }

//        internal static Expression IsNullOrWhiteSpace(this Expression stringExpression)
//        {
//            var method = typeof(string).GetMethod("IsNullOrWhiteSpace", new Type[] { typeof(string) });
//            return Expression.Call(null, method, stringExpression);
//        }

//        internal static Expression InstanceToStringExpression(this Expression instance)
//        {
//            return Expression.Call(instance, instance.Type.GetMethod("ToString", Type.EmptyTypes));
//        }

//    }
//}
