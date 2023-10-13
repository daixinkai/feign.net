using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    /// <summary>
    /// An interface representing request mapping
    /// </summary>
    internal interface IRequestMapping
    {
        string GetMethod();
    }
}
