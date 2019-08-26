using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone
{

    class Singletons
    {
        static readonly IDictionary<Type, object> _map = new Dictionary<Type, object>();
        public object Value { get; set; }
        public static object GetInstance(Type type)
        {
            object value;
            _map.TryGetValue(type, out value);
            return value;
        }
        public static void SetInstance(Type type, object value)
        {
            if (_map.ContainsKey(type))
            {
                _map[type] = value;
            }
            else
            {
                _map.Add(type, value);
            }
        }
    }

}
