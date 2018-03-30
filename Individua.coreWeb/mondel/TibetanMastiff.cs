using Individua.coreWeb.Infs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Individua.coreWeb.mondel
{
    /// <summary>
    /// 藏獒
    /// </summary>
    public class TibetanMastiff : IDog
    {
        /// <summary>
        /// 品种
        /// </summary>
        public string Breed
        {
            get
            {
                return "Mastiff Class（獒犬类）";
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return "小黑";
            }
        }
    }
}
