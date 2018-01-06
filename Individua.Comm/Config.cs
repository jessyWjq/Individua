using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Comm
{
    public static class Config
    {
        /// <summary>
        /// 文件上传地址
        /// </summary>
        public static string FileUploadPath { get; private set; }
        /// <summary>
        /// 文件服务访问地址
        /// </summary>
        public static string FileServiceUrl { get; private set; }
        static Config()
        {
            string[] FileServiceUrls = ConfigurationManager.AppSettings["FileServiceUrl"].Split(',');

            string[] FileUploadPaths = ConfigurationManager.AppSettings["FileUploadPath"].Split(',');

#if DEBUG
            FileServiceUrl = FileServiceUrls[1];
            FileUploadPath = FileUploadPaths[1];
#else
            FileServiceUrl = FileServiceUrls[0];
            FileUploadPath = FileUploadPaths[0];
#endif

            //FileUploadPath = ConfigurationManager.AppSettings["FileUploadPath"];
        }
    }
}
