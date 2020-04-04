using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    partial class ReflectionExtensions
    {
        static readonly MethodInfo GetMethodFromHandleMethodInfo = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

        static readonly MethodInfo GetMethodFromHandleMethodInfoAndTypeHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        static readonly MethodInfo GetTypeFromHandleMethodInfo = typeof(Type).GetMethod("GetTypeFromHandle");

        public static LocalBuilder DefineEmitMethodInfo(this ILGenerator iLGenerator, MethodInfo method)
        {
            LocalBuilder methodLocalBuilder = iLGenerator.DeclareLocal(typeof(MethodInfo));
            iLGenerator.EmitMethodInfo(method);
            iLGenerator.Emit(OpCodes.Stloc, methodLocalBuilder);
            return methodLocalBuilder;
        }
        /// <summary>
        ///  like  methodof(ReflectionExtensions.EmitMethodInfo(ILGenerator,MethodInfo))
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="method"></param>
        public static void EmitMethodInfo(this ILGenerator iLGenerator, MethodInfo method)
        {
            iLGenerator.Emit(OpCodes.Ldtoken, method);
            //iLGenerator.Emit(OpCodes.Call, GetMethodFromHandleMethodInfo);
            iLGenerator.Emit(OpCodes.Ldtoken, method.DeclaringType);
            iLGenerator.Emit(OpCodes.Call, GetMethodFromHandleMethodInfoAndTypeHandle);
            iLGenerator.Emit(OpCodes.Castclass, typeof(MethodInfo));
        }

        public static LocalBuilder DefineEmitType(this ILGenerator iLGenerator, Type type)
        {
            LocalBuilder typeLocalBuilder = iLGenerator.DeclareLocal(typeof(Type));
            iLGenerator.Emit(OpCodes.Ldtoken, type);
            iLGenerator.Emit(OpCodes.Stloc, typeLocalBuilder);
            iLGenerator.Emit(OpCodes.Call, GetTypeFromHandleMethodInfo);
            return typeLocalBuilder;
        }

        /// <summary>
        ///  like  typeof(ReflectionExtensions)
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="type"></param>
        public static void EmitType(this ILGenerator iLGenerator, Type type)
        {
            iLGenerator.Emit(OpCodes.Ldtoken, type);
            iLGenerator.Emit(OpCodes.Call, GetTypeFromHandleMethodInfo);
        }

        /// <summary>
        ///  like  typeof(ReflectionExtensions)
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="value"></param>
        public static void EmitEnumValue(this ILGenerator iLGenerator, Enum value)
        {
            Type enumType = value.GetType();
            iLGenerator.Emit(OpCodes.Ldc_I4, value.GetHashCode());
        }

        /// <summary>
        /// like new string[]{"1","2","3","4"}
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="list"></param>
        public static void EmitStringArray(this ILGenerator iLGenerator, IEnumerable<string> list)
        {
            iLGenerator.Emit(OpCodes.Ldc_I4, list.Count());
            iLGenerator.Emit(OpCodes.Newarr, typeof(string));
            int index = 0;
            foreach (var item in list)
            {
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.Emit(OpCodes.Ldc_I4, index);
                iLGenerator.Emit(OpCodes.Ldstr, item);
                iLGenerator.Emit(OpCodes.Stelem_Ref);
                index++;
            }
        }


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


    }
}
