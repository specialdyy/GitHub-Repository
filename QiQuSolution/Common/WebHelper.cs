using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class WebHelper
    {
        /// <summary>
        /// 如果需要更多的连接时可以通过该方法设置系统的最大连接数限制。
        /// </summary>
        /// <param name="maxConnectionCount"></param>
        public static void SetMaxHttpWebRequestConnectionCount(this int maxConnectionCount)
        {
            if(maxConnectionCount > 1024 || maxConnectionCount <= 2)
            {
                throw new ArgumentOutOfRangeException("maxConnectionCount");
            }
            ServicePointManager.DefaultConnectionLimit = maxConnectionCount;
        }

        public static HttpWebRequest SetHttpWebRequestHeaders(this HttpWebRequest request, string accept, string userAgent, string host, string referer = null, bool? keepAlive = true, Dictionary<string, string> otherHeaders = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            #region 这些信息必须以这种属性赋值的方式来设置，否则如果用 request.Headers 来设置的话会报类似这样的错误：“必须使用适当的属性或方法修改 “Connection” 标头。”
            if (string.IsNullOrEmpty(accept))
            {
                request.Accept = accept;
            }
            if (string.IsNullOrEmpty(referer))
            {
                request.Referer = referer;
            }
            if (string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (string.IsNullOrEmpty(host))
            {
                request.Host = host;
            }
            if (keepAlive.HasValue)
            {
                request.KeepAlive = keepAlive.Value;
            }
            #endregion

            if (otherHeaders != null && otherHeaders.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in otherHeaders)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }
            return request;
        }

        public static HttpWebRequest SetHttpWebRequestCookies(this HttpWebRequest request, CookieCollection cookies)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            if (cookies.Count < 1)
            {
                return request;
            }

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
            return request;

            #region 设置报文头中的 Cookie。
            #region Cookie Content
            ////Cookie：
            //JXM800295=1;
            //CNZZDATA3260529=cnzz_eid%3D1166721843-1447876047-%26ntime%3D1448366485;
            //bdshare_firstime=1447876047715;
            //clicktopay=1447876480618;
            //ct_760_ad_index=731;
            //ct_300_ad_index=714;
            //ct_160_ad_index=1287;
            //ct_cpv_ad_index=733;
            //ct_960_ad_index=661;
            //lzstat_uv=2229606763683479947|3600220
            #endregion

            //string defaultPath = "/";
            //string defaultDomain = "www.400gb.com";
            //Dictionary<string, string> cookiesDict = new Dictionary<string, string>();
            //cookiesDict.Add("lzstat_uv", "22282395372947674394|3260529");
            //cookiesDict.Add("ctdisk_auth", "WTkCNwE3BDsFOQVkA2pSWQE2AGYIO1NnCjBSYF0wUjtTPgM3Ag4JZ1VgVjFWO1U0ADBfMwNlBGNab1ViDzcBYFk5AmMBZQRvBWYFYgNtUmEBZgBrCGlTNQo%2FUmVdYFI7UzADZAIz");
            //cookiesDict.Add("pubcookie", "BGRQZQo8VGsOMlY3XDUCCVdjCV8Gcgd0AWhTMlxuBzdWbgBmVysCcwQIAW5SbAYyXW0LNVRmXz8NNVZhCWABaARiUDAKN1RsDjhWZ1xlAmRXaAk0BmMHMwE1U2RcNgc0VmQAM1dhAmwEZwFfUnwGcV01CydUO181DW9WMQlmAWEEY1BnCk9ULw5iVm9cYAIuVzIJOQZsByoBblM%2FXA4HZ1Y2ADZXZgIzBDgBZ1I7BjxdawtdVDJfOg04VmAJZwFoBGJQZgppVDkOOVYwXGMCNVczCTQGNAdiAWhTaFw3B2dWMQBmVzECOwQxAWJSawZgXWkLMQ%3D%3D");
            //cookiesDict.Add("JXM731410", "1");
            //#region 由于 Cookie 的 Value 中含有 “,” 导致运行时出现异常，上网搜索后得知可通过用 “%2C” 来替代 “,” 的方式来解决，在当前代码中测试通过。
            //cookiesDict.Add("Hm_lvt_74590c71164d9fba556697bee04ad65c", "1445021488,1445029426,1445029907,1445030380".Replace(",", "%2C"));
            //#endregion
            //cookiesDict.Add("bdshare_firstime", "1444959022118");
            //cookiesDict.Add("JXM800295", "1");

            //CookieCollection cookies = new CookieCollection();
            //cookiesDict.ToList().ForEach(cookie => cookies.Add(new Cookie(cookie.Key, cookie.Value, defaultPath, defaultDomain)));


            //request.CookieContainer = new CookieContainer();
            //request.CookieContainer.Add(cookies);
            #endregion
        }

        public static HttpWebRequest SetHttpWebRequestCookies(this HttpWebRequest request, IEnumerable<Cookie> cookies)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            if (cookies.Count() < 1)
            {
                return request;
            }
            CookieCollection cookieCollection = new CookieCollection();
            cookies.ToList().ForEach(cookie => cookieCollection.Add(cookie));
            return SetHttpWebRequestCookies(request, cookieCollection);
        }

        public static HttpWebRequest SetHttpWebRequestCookies(this HttpWebRequest request, params Cookie[] cookies)
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            return SetHttpWebRequestCookies(request, cookies.AsEnumerable());
        }

        public static string GetTextContentByHttpWebRequest(this Uri url, bool convertUnicodeNumberToHanZiIfExist = true, Dictionary<HttpRequestHeader, string> requestHeaders = null, Dictionary<string, string> extendRequestHeaders = null, CookieCollection cookies = null, bool isPostMethod = false, string postData = null, Encoding postDataEncoding = null)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Proxy = null;

            if (requestHeaders != null && requestHeaders.Count > 0)
            {
                if (requestHeaders.ContainsKey(HttpRequestHeader.ContentLength) && string.IsNullOrEmpty(postData))
                {
                    throw new Exception("不能为不写入数据的操作设置 Content-Length 或 Chunked 编码。");
                }
                HttpRequestHeader key;
                string val;
                string keepAliveStringPattern = "(keep[\\-_ ]?alive)|(true)";
                foreach (var header in requestHeaders)
                {
                    key = header.Key;
                    val = header.Value;
                    #region （未完整！待完善！）注意这些属性的设置方法。
                    ////这些属性不能通过像下面这样的方式来实现：
                    //request.Headers.Add(HttpRequestHeader.Accept, "text/html, application/xhtml+xml, */*");
                    //request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
                    //request.Headers.Add(HttpRequestHeader.Host, "xn--fjqw09e1ga392m.ctfile.com");
                    //request.Headers.Add(HttpRequestHeader.Connection, "Keep-Alive");

                    ////而要通过像下面这样的方式来实现：
                    //request.Accept = "text/html, application/xhtml+xml, */*";
                    //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    //request.Host = "xn--fjqw09e1ga392m.ctfile.com";
                    //request.KeepAlive = true;
                    #endregion
                    if (key == HttpRequestHeader.Accept)
                    {
                        request.Accept = val;
                    }
                    else if (key == HttpRequestHeader.UserAgent)
                    {
                        request.UserAgent = val;
                    }
                    else if (key == HttpRequestHeader.Host)
                    {
                        request.Host = val;
                    }
                    else if (key == HttpRequestHeader.ContentType)
                    {
                        request.ContentType = val;
                    }
                    else if (key == HttpRequestHeader.ContentLength)
                    {
                        request.ContentLength = Convert.ToInt32(val);
                    }
                    else if (key == HttpRequestHeader.Referer)
                    {
                        request.Referer = val;
                    }
                    else if (key == HttpRequestHeader.Connection || key == HttpRequestHeader.KeepAlive)
                    {
                        if (Regex.IsMatch(val, keepAliveStringPattern, RegexOptions.IgnoreCase))
                        {
                            request.KeepAlive = true;
                        }
                        else
                        {
                            request.KeepAlive = false;
                        }
                    }
                    else
                    {
                        request.Headers.Add(key, val);
                    }
                }
            }
            if(extendRequestHeaders != null && extendRequestHeaders.Count > 0)
            {
                foreach (var header in extendRequestHeaders)
                {
                    if (string.IsNullOrEmpty(header.Key) || string.IsNullOrEmpty(header.Value))
                    {
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            if (cookies != null && cookies.Count > 0)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            if(isPostMethod)
            {
                request.Method = "POST";
                if (false == string.IsNullOrEmpty(postData))
                {
                    if (postDataEncoding == null)
                    {
                        postDataEncoding = Encoding.Default;
                    }
                    byte[] data = postDataEncoding.GetBytes(postData);
                    if (request.ContentLength == 0)
                    {
                        request.ContentLength = data.Length;
                    }
                    //request.Method = "POST";
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(data, 0, data.Length);
                    }
                }
            }

            //request.ProtocolVersion = new Version("1.0");
            string content;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                string encoding = response.Headers[HttpResponseHeader.ContentEncoding];
                bool isGzipFormatData = (encoding != null && encoding.ToLower().Trim() == "gzip");
                using (Stream stream = response.GetResponseStream())
                {
                    if (isGzipFormatData)
                    {
                        content = stream.DecodeGzipStream();
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                }

                if (convertUnicodeNumberToHanZiIfExist)
                {
                    if (content.IsContainUnicodeNumber())
                    {
                        content = content.ConvertToHanZiString();
                    }
                }
            }
            return content;
        }


        public static string RepeatDownloadContentIgnore503Error(this Uri url, bool convertUnicodeNumberToHanZiIfExist = true, Dictionary<HttpRequestHeader, string> requestHeaders = null, Dictionary<string, string> extendRequestHeaders = null, CookieCollection cookies = null, bool isPostMethod = false, string postData = null, Encoding postDataEncoding = null)
        {
            int delayMilliseconds = 250;
            string content;
            while (true)
            {
                try
                {
                    content = url.GetTextContentByHttpWebRequest(convertUnicodeNumberToHanZiIfExist, requestHeaders, extendRequestHeaders, cookies, isPostMethod, postData, postDataEncoding);
                    break;
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("503") || ex.Message.Contains("服务器不可用"))
                    {
                        Thread.Sleep(delayMilliseconds);
                        if (delayMilliseconds < 1500)
                        {
                            delayMilliseconds += 250;
                        }
                        continue;
                    }
                    throw ex;
                }
            }
            return content;
        }

        public static HttpWebResponseInfo<Image> GetPictureByHttpWebRequest(this Uri url, Dictionary<HttpRequestHeader, string> requestHeaderDictionary = null, CookieCollection cookies = null)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Proxy = null;

            if (requestHeaderDictionary != null && requestHeaderDictionary.Count > 0)
            {
                HttpRequestHeader key;
                string val;
                string keepAliveStringPattern = "(keep[\\-_ ]?alive)|(true)";
                foreach (var header in requestHeaderDictionary)
                {
                    key = header.Key;
                    val = header.Value;
                    #region （未完整！待完善！）注意这些属性的设置方法。
                    ////这些属性不能通过像下面这样的方式来实现：
                    //request.Headers.Add(HttpRequestHeader.Accept, "text/html, application/xhtml+xml, */*");
                    //request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
                    //request.Headers.Add(HttpRequestHeader.Host, "xn--fjqw09e1ga392m.ctfile.com");
                    //request.Headers.Add(HttpRequestHeader.Connection, "Keep-Alive");

                    ////而要通过像下面这样的方式来实现：
                    //request.Accept = "text/html, application/xhtml+xml, */*";
                    //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    //request.Host = "xn--fjqw09e1ga392m.ctfile.com";
                    //request.KeepAlive = true;
                    #endregion
                    if (key == HttpRequestHeader.Accept)
                    {
                        request.Accept = val;
                    }
                    else if (key == HttpRequestHeader.UserAgent)
                    {
                        request.UserAgent = val;
                    }
                    else if (key == HttpRequestHeader.Host)
                    {
                        request.Host = val;
                    }
                    else if (key == HttpRequestHeader.ContentType)
                    {
                        request.ContentType = val;
                    }
                    else if (key == HttpRequestHeader.ContentLength)
                    {
                        request.ContentLength = Convert.ToInt32(val);
                    }
                    else if (key == HttpRequestHeader.Referer)
                    {
                        request.Referer = val;
                    }
                    else if (key == HttpRequestHeader.Connection || key == HttpRequestHeader.KeepAlive)
                    {
                        if (Regex.IsMatch(val, keepAliveStringPattern, RegexOptions.IgnoreCase))
                        {
                            request.KeepAlive = true;
                        }
                        else
                        {
                            request.KeepAlive = false;
                        }
                    }
                    else
                    {
                        request.Headers.Add(key, val);
                    }
                }
            }
            if (cookies != null && cookies.Count > 0)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            HttpWebResponseInfo<Image> result = new HttpWebResponseInfo<Image>();
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                if (response.Headers.AllKeys.Count()>0)
                {
                    foreach (string key in response.Headers.AllKeys)
                    {
                        result.ResponseHeaders.Add(key, response.Headers.Get(key));
                    }
                }
                #region 有效性待确认！初步测试得出服务端返回的 Cookie 貌似是以 Header 的形式来返回，因此下面的方法可能会没有意义。
                if (response.Cookies.Count > 0)
                {
                    foreach (Cookie cookie in response.Cookies)
                    {
                        result.ResponseCookies.Add(cookie);
                    }
                }
                #endregion


                Stream picStream = response.GetResponseStream();
                Image img = Bitmap.FromStream(picStream);
                result.ResponseData = img;
                return result;
            }
        }
    }
}
