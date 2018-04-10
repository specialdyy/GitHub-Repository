using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HttpWebResponseInfo<T>
    {
        public HttpWebResponseInfo()
        {
            this.responseCookies = new CookieCollection();
            this.responseHeaders = new Dictionary<string, string>();
        }

        private T responseData;

        public T ResponseData
        {
            get
            {
                return responseData;
            }
            set
            {
                responseData = value;
            }
        }

        private Dictionary<string, string> responseHeaders;

        public Dictionary<string, string> ResponseHeaders
        {
            get
            {
                return responseHeaders;
            }
            set
            {
                responseHeaders = value;
            }
        }

        private CookieCollection responseCookies;

        public CookieCollection ResponseCookies
        {
            get
            {
                return responseCookies;
            }
            set
            {
                responseCookies = value;
            }
        }
    }
}
