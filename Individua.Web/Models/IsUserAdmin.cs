using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace Individua.Web.Models
{
    /// <summary>
    /// 验证是否是管理员权限
    /// </summary>
    public static class IsUserAdmin
    {
        /// <summary>
        /// 验证是否是管理员权限
        /// </summary>
        /// <returns></returns>
        public static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
                LogHelper.WriteLog(typeof(IsUserAdmin), ex.Message);
            }
            catch (Exception ex)
            {
                isAdmin = false;
                LogHelper.WriteLog(typeof(IsUserAdmin), ex.Message);
            }
            return isAdmin;
        }
    }
}