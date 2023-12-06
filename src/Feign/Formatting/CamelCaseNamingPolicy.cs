using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    public class CamelCaseNamingPolicy : NamingPolicy
    {
#if USE_SYSTEM_TEXT_JSON
        public override string ConvertName(string name) => JsonNamingPolicy.CamelCase.ConvertName(name);
#else
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
            {
                return name;
            }

#if NET45 || NETSTANDARD2_0
            char[] chars = name.ToCharArray();
            FixCasing(chars);
            return new string(chars);
#else
            return string.Create(name.Length, name, (chars, name) =>
            {
                name.AsSpan().CopyTo(chars);
                FixCasing(chars);
            });
#endif
        }

        private static void FixCasing(
#if NET45 || NETSTANDARD2_0
char[] chars
#else
 Span<char> chars

#endif
            )
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);

                // Stop when next char is already lowercase.
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // If the next char is a space, lowercase current char before exiting.
                    if (chars[i + 1] == ' ')
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }
        }
#endif

    }
}
