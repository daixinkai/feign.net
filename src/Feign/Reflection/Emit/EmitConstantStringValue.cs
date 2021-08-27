using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    internal class EmitConstantStringValue : IEmitValue<string>
    {
        public EmitConstantStringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public void Emit(ILGenerator iLGenerator)
        {
            iLGenerator.Emit(OpCodes.Ldstr, Value);
        }
    }
}
