using System;
using System.Collections.Generic;
using System.Text;

namespace Individua.coreCLI.interfaces
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
