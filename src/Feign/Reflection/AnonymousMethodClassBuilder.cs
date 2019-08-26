using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    class AnonymousMethodClassBuilder : AnonymousMethodClassBuilderBase
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
            return FillType(typeBuilder, comparer);
        }
    }
}
