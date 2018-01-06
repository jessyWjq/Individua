using System;

namespace SZXFZ.Web.Models
{
    public class MsgSocket
    {
        private string _tag = "web";
        private string _type = "hss";

        public string Tag
        {
            get
            {
                return _tag;
            }

            set
            {
                _tag = String.IsNullOrEmpty(value) ? "web" : value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = String.IsNullOrEmpty(value) ? "hss" : value;
            }
        }

        public dynamic Ulist { get; set; }
    }
}