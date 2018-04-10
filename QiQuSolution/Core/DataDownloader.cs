using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Core
{
    public static class DataDownloader
    {
        public static List<SourceData> DownloadData()
        {
            Uri url = new Uri("http://77tj.org/api/tencent/onlineim");

            return DownloadData(url);
        }

        public static List<SourceData> DownloadData(this Uri url)
        {
            string result;
            try
            {
                result = url.GetTextContentByHttpWebRequest();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("下载数据失败！", ex);
            }
            if(string.IsNullOrEmpty(result))
            {
                throw new ApplicationException("下载数据失败！获取不到指定的 Json 数据！");
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            List<DTOSourceData> tmpData;
            List<SourceData> data;
            try
            {
                tmpData = (List<DTOSourceData>)js.Deserialize(result, typeof(List<DTOSourceData>));
                data = tmpData.ConvertAll<SourceData>(i => i.ToSourceData());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("把 Json 类型数据转换成 SourceData 类型的对象失败！", ex);
            }
            if (data == null || data.Count <= 0)
            {
                throw new ApplicationException("获取指定地址的数据失败！");
            }
            return data;
        }
    }
}
