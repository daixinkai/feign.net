using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Formatting
{
    /// <summary>
    /// Determines the naming policy used to convert a string-based name to another format, such as a camel-casing format.
    /// </summary>
    public abstract class NamingPolicy
    {
        /// <summary>
        /// Returns the naming policy for default.
        /// </summary>
        public static NamingPolicy Default { get; } = new DefaultNamingPolicy();
        /// <summary>
        /// Returns the naming policy for camel-casing.
        /// </summary>
        public static NamingPolicy CamelCase { get; } = new CamelCaseNamingPolicy();

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Returns the naming policy for lower snake-casing.
        /// </summary>
        public static NamingPolicy SnakeCaseLower { get; } = new SeparatorNamingPolicy(true, '_');

        /// <summary>
        /// Returns the naming policy for upper snake-casing.
        /// </summary>
        public static NamingPolicy SnakeCaseUpper { get; } = new SeparatorNamingPolicy(false, '_');

        /// <summary>
        /// Returns the naming policy for lower kebab-casing.
        /// </summary>
        public static NamingPolicy KebabCaseLower { get; } = new SeparatorNamingPolicy(true, '-');

        /// <summary>
        /// Returns the naming policy for upper kebab-casing.
        /// </summary>
        public static NamingPolicy KebabCaseUpper { get; } = new SeparatorNamingPolicy(false, '-');
#endif

        /// <summary>
        /// When overridden in a derived class, converts the specified name according to the policy.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted name.</returns>
        public abstract string ConvertName(string name);


        internal class DefaultNamingPolicy : NamingPolicy
        {
            public override string ConvertName(string name)
            {
                return name;
            }
        }

    }
}
