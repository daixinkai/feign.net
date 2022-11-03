using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    partial class ObjectQueryMap
    {

        //private static readonly Func<T, NamingPolicy, IDictionary<string, string>> Invoker;

        //static ObjectQueryMap()
        //{
        //    Invoker = CreateGetObjectStringParametersDelegate();
        //}

        //public IDictionary<string, string> GetStringParameters() => Invoker(Value);

        //private static Func<T, NamingPolicy, IDictionary<string, string>> CreateGetObjectStringParametersDelegate()
        //{
        //    //Func<T,IDictionary<string,string>>
        //    var param = Expression.Parameter(typeof(T), "param");
        //    var namingPolicy = Expression.Parameter(typeof(NamingPolicy), "namingPolicy");
        //    var result = Expression.New(typeof(Dictionary<string, string>));
        //    Dictionary<string, Expression> propertyExpressions = new Dictionary<string, Expression>();
        //    foreach (var property in typeof(T).GetProperties())
        //    {
        //        if (property.GetMethod == null)
        //        {
        //            continue;
        //        }
        //        if (property.PropertyType.IsValueType)
        //        {
        //            //tostring

        //            continue;
        //        }

        //        object propertyValue = property.GetValue(value);
        //        if (propertyValue == null)
        //        {
        //            continue;
        //        }
        //        if (propertyValue is string)
        //        {
        //            yield return new KeyValuePair<string, string>(GetName(property, namingPolicy), propertyValue.ToString());
        //            continue;
        //        }
        //        if (propertyValue is IEnumerable)
        //        {
        //            foreach (var item in propertyValue as IEnumerable)
        //            {
        //                if (item == null)
        //                {
        //                    continue;
        //                }
        //                yield return new KeyValuePair<string, string>(GetName(property, namingPolicy), converters.ConvertValue<string>(item, true));
        //            }
        //            continue;
        //        }
        //        yield return new KeyValuePair<string, string>(GetName(property, namingPolicy), converters.ConvertValue<string>(propertyValue, true));
        //    }

        //}


    }
}
