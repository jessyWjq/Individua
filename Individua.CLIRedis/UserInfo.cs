using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.CLIRedis
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

        public static implicit operator Func<object>(UserInfo v)
        {
            throw new NotImplementedException();
        }
    }
}
