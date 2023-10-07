using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    internal static class EncodingEx
    {
        public static Encoding Default => Encoding.UTF8;

        public static Encoding GetRequiredEncoding(Encoding? encoding)
            => encoding ?? Default;

    }
}
