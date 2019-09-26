using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    static class ReflectionHelper
    {

        static readonly MethodInfo GetMethodFromHandleMethodInfo = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

        static readonly MethodInfo GetMethodFromHandleMethodInfoAndTypeHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        static readonly MethodInfo GetTypeFromHandleMethodInfo = typeof(Type).GetMethod("GetTypeFromHandle");

        public static LocalBuilder DefineEmitMethodInfo(ILGenerator iLGenerator, MethodInfo method)
        {
            LocalBuilder methodLocalBuilder = iLGenerator.DeclareLocal(typeof(MethodInfo));
            EmitMethodInfo(iLGenerator, method);
            iLGenerator.Emit(OpCodes.Stloc, methodLocalBuilder);
            return methodLocalBuilder;
        }

        public static void EmitMethodInfo(ILGenerator iLGenerator, MethodInfo method)
        {
            iLGenerator.Emit(OpCodes.Ldtoken, method);
            //iLGenerator.Emit(OpCodes.Call, GetMethodFromHandleMethodInfo);
            iLGenerator.Emit(OpCodes.Ldtoken, method.DeclaringType);
            iLGenerator.Emit(OpCodes.Call, GetMethodFromHandleMethodInfoAndTypeHandle);
            iLGenerator.Emit(OpCodes.Castclass, typeof(MethodInfo));
        }

        public static LocalBuilder DefineEmitType(ILGenerator iLGenerator, Type type)
        {
            LocalBuilder typeLocalBuilder = iLGenerator.DeclareLocal(typeof(Type));
            iLGenerator.Emit(OpCodes.Ldtoken, type);
            iLGenerator.Emit(OpCodes.Stloc, typeLocalBuilder);
            iLGenerator.Emit(OpCodes.Call, GetTypeFromHandleMethodInfo);
            return typeLocalBuilder;
        }

        public static void EmitType(ILGenerator iLGenerator, Type type)
        {
            iLGenerator.Emit(OpCodes.Ldtoken, type);
            iLGenerator.Emit(OpCodes.Call, GetTypeFromHandleMethodInfo);
        }

    }
}
