using Individua.coreWeb.Infs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Individua.coreWeb.mondel
{
    /// <summary>
    /// 萨摩耶
    /// </summary>
    public class Samoyed : IDog
    {
        /// <summary>
        /// 品种
        /// </summary>
        public string Breed
        {
            get
            {
                return "Samoyed（萨摩耶）";
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return "小黄";
            }
        }
    }
}
