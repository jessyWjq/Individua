using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Individua.coreWeb.Infs
{
    interface IDog
    {

        /// <summary>
        /// 品种
        /// </summary>
        string Breed { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
    }
}
