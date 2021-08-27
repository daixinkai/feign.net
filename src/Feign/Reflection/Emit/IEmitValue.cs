using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    internal interface IEmitValue<T>
    {
        void Emit(ILGenerator iLGenerator);
    }
}
