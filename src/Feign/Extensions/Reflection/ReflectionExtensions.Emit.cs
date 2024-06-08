using Feign.Reflection;
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

        private static readonly MethodInfo GetMethodFromHandleMethodInfoAndTypeHandle = typeof(MethodBase).GetRequiredMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        private static readonly MethodInfo GetTypeFromHandleMethodInfo = typeof(Type).GetRequiredMethod("GetTypeFromHandle");

        public static MethodBuilder DefineMethodBuilder(this TypeBuilder typeBuilder, MethodInfo method, MethodAttributes methodAttributes, bool copyCustomAttributes)
        {

            var arguments = method.GetParameters().Select(a => a.ParameterType).ToArray();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, arguments);
            int position = 1;
            foreach (var parameter in method.GetParameters())
            {
                var parameterBuilder = methodBuilder.DefineParameter(position, ParameterAttributes.None, parameter.Name);
                parameterBuilder.CopyCustomAttributes(parameter);
                position++;
            }
            return methodBuilder;
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
            iLGenerator.Emit(OpCodes.Ldtoken, method.DeclaringType!);
            iLGenerator.Emit(OpCodes.Call, GetMethodFromHandleMethodInfoAndTypeHandle);
            iLGenerator.Emit(OpCodes.Castclass, typeof(MethodInfo));
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
        ///  like  = string value
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="value"></param>
        public static void EmitStringValue(this ILGenerator iLGenerator, string? value)
        {
            if (value == null)
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldstr, value);
            }
        }

        /// <summary>
        ///  like  = Enum value
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="value"></param>
        public static void EmitEnumValue(this ILGenerator iLGenerator, Enum value)
        {
            //Type enumType = value.GetType();
            //iLGenerator.Emit(OpCodes.Ldc_I4, value.GetHashCode());
            iLGenerator.EmitInt32Value(value.GetHashCode());
        }

        public static void EmitInt32Value(this ILGenerator iLGenerator, int value)
        {
            switch (value)
            {
                case -1:
                    iLGenerator.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    iLGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    iLGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    iLGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    iLGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    iLGenerator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    iLGenerator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    iLGenerator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    iLGenerator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    iLGenerator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value > -128 && value <= 128)
                    {
                        iLGenerator.Emit(OpCodes.Ldc_I4_S, value);
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        public static void EmitLdargEx(this ILGenerator iLGenerator, int from, int count)
        {
            for (int i = from; i <= count; i++)
            {
                iLGenerator.EmitLdarg(i);
            }
        }

        public static void EmitLdarg(this ILGenerator iLGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    iLGenerator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    iLGenerator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    iLGenerator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    iLGenerator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    iLGenerator.EmitLdargS(index);
                    break;
            }
        }

        public static void EmitLdargS(this ILGenerator iLGenerator, int index)
        {
            if (index > byte.MaxValue)
            {
                iLGenerator.Emit(OpCodes.Ldarg_S, index);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldarg_S, (byte)index);
            }
        }

        //        /// <summary>
        //        /// like new string[]{"1","2","3","4"}
        //        /// </summary>
        //        /// <param name="iLGenerator"></param>
        //        /// <param name="list"></param>
        //        public static void EmitStringArray(this ILGenerator iLGenerator, IEnumerable<IEmitValue<string>> list)
        //        {
        //            int count = list.Count();
        //#if !NET45
        //            if (count == 0)
        //            {
        //                iLGenerator.Emit(OpCodes.Call, typeof(Array).GetRequiredMethod("Empty").MakeGenericMethod(typeof(string)));
        //                return;
        //            }
        //#endif

        //            iLGenerator.EmitInt32Value(count);
        //            iLGenerator.Emit(OpCodes.Newarr, typeof(string));
        //            int index = 0;
        //            foreach (var item in list)
        //            {
        //                iLGenerator.Emit(OpCodes.Dup);
        //                iLGenerator.EmitInt32Value(index);
        //                item.Emit(iLGenerator);
        //                iLGenerator.Emit(OpCodes.Stelem_Ref);
        //                index++;
        //            }

        //        }

        /// <summary>
        /// like new T[]{"1","2","3","4"}
        /// </summary>
        /// <param name="iLGenerator"></param>
        /// <param name="list"></param>
        public static void EmitNewArray<T>(this ILGenerator iLGenerator, IEnumerable<IEmitValue<T>> list)
        {
            int count = list.Count();
#if !NET45
            if (count == 0)
            {
                iLGenerator.Emit(OpCodes.Call, typeof(Array).GetRequiredMethod("Empty").MakeGenericMethod(typeof(T)));
                return;
            }
#endif

            iLGenerator.EmitInt32Value(count);
            iLGenerator.Emit(OpCodes.Newarr, typeof(T));
            int index = 0;
            foreach (var item in list)
            {
                iLGenerator.Emit(OpCodes.Dup);
                iLGenerator.EmitInt32Value(index);
                item.Emit(iLGenerator);
                iLGenerator.Emit(OpCodes.Stelem_Ref);
                index++;
            }

        }

        public static void CallBaseTypeDefaultConstructor(this ILGenerator constructorIlGenerator, Type baseType)
        {
            var defaultConstructor = baseType.GetDefaultConstructor();
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
                constructorIlGenerator.EmitLdarg(i);
            }
            constructorIlGenerator.Emit(OpCodes.Call, baseTypeConstructor);
        }

        public static void EmitGetProperty(this ILGenerator iLGenerator, PropertyInfo property)
        {
            iLGenerator.Emit(OpCodes.Call, property.GetMethod!);
        }

        public static void EmitSetProperty(this ILGenerator iLGenerator, PropertyInfo property)
        {
            iLGenerator.Emit(OpCodes.Call, property.SetMethod!);
        }

        public static void EmitNop(this ILGenerator _)
        {
            //#if DEBUG
            //            _.Emit(OpCodes.Nop);
            //#endif
        }
    }
}
