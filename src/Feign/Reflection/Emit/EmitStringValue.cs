using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    internal class EmitStringValue : IEmitValue<string>
    {
        public EmitStringValue(string? value)
        {
            Value = value;
        }

        public EmitStringValue(string? value, bool ignoreNullValue)
        {
            Value = value;
            IgnoreNullValue = ignoreNullValue;
        }

        public string? Value { get; }

        public bool IgnoreNullValue { get; }

        public bool Emit(ILGenerator iLGenerator)
        {
            if (Value == null && IgnoreNullValue)
            {
                return false;
            }
            iLGenerator.EmitStringValue(Value);
            return true;
        }
    }
}
