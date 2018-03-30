using Individua.coreCLI.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Individua.coreCLI.Comm
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
