using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Individua.Web.Models
{
    public class AndroidResult
    {
        
        
        private string _isError= "false";
        private string _errorType="2";
        private string _errorMessage="";
        


        public string IsError
        {
            get
            {
                return _isError;
            }

            set
            {
                _isError = String.IsNullOrEmpty(value) ? "false" : value;                              
            }
        }

        public string ErrorType
        {
            get
            {
                return _errorType;
            }

            set
            {
                _errorType = String.IsNullOrEmpty(value) ? "2" : value;
                
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = String.IsNullOrEmpty(value) ? "" : value;

            }
        }
        public dynamic Result { get; set; }

    }

    /// <summary>
    ///  安卓 res 返回值枚举
    /// </summary>
    public enum AndroidResStr {
        Success = 1,
        Error = 0
    }

    public enum ErrorType
    {
        Success = 1,
        Error = 2
    }
}