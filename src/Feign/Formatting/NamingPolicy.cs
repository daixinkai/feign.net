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
