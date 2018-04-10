using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public static class HtmlTextHelper
    {

        #region 匹配一个同时具有开始标签和闭合标签（即开始和闭合标签对称）、且具有某个指定的属性（attributeName）和属性值（attributeValue，例如 id="abc" 或 value="true" 等）的 html 元素的正则表达式。
        #region 原始表达式（转义前的版本）。
        //下面的表达式为原始的未进行转义的表达式原型，匹配含有名称为 attributeName 、值为 attributeValue 的属性的开闭标签匹配的 html 文档字符串：
        //注意！下面的表达式中的长空白没有匹配作用，主要是为了阅读时的清晰明了，使用时必须删除掉所有的空白或者设置 RegexOptions 属性的值为 IgnorePatternWhitespace。
        //(<([\w-]+)    (\s+([\w-]+)\s*=\s*(["']?)([^"<>]*?)\5)*    \s+    ((attributeName)\s*=\s*(["']?)\s*(attributeValue)\s*\9)    (\s+([\w-]+)\s*=\s*(["']?)([^"<>]*?)\13)*    \s*>)
        //    (?>                                                                                               //1个左括号，但经测试发现，该括号不会被分组功能识别为一个组，即正则引擎在分组时会跳过这个括号，不会把这个括号中的内容当成一个分组。
        //        [^<>]+                                                                                        //匹配不含任何标签符号的部分的内容。
        //    |
        //        <[\w-]+    (\s+([\w-]+)\s*=\s*(["']?)([^"<>]*?)\17)*    \s*?/>                                //匹配单独一个的 “开闭标签”（即类似于 “<img *** />”）。
        //    |
        //        <!--[\s\S]+?-->                                                                               //匹配注释标签。
        //    |
        //        <[Bb][Rr]\s*>                                                                                 //匹配不规则的换行符标签。
        //    |
        //        <[Hh][Rr]\s*>                                                                                 //匹配不规则的水平分割线标签。
        //    |
        //        (<([\w-]+)    (\s+([\w-]+)\s*=\s*(["']?)([^"<>]*?)\23)*    \s*?>)    (?<DEPTH>)               //匹配正常的开始标签（形如：“<a href="">”、“<input >” 等）。
        //    |
        //        (</[\w-]+>)    (?<-DEPTH>)                                                                    //匹配正常的结束标签（形如：“</a>”、“</input>” 等）。
        //    )*
        //    (?(DEPTH)(?!))                                                                                    //3个左括号
        //</\\2>
        #endregion
        #region 转义后的表达式
        //(<([\\w-]+)    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\5)*    \\s+    ((attributeName)\\s*=\\s*([\"']?)\\s*(attributeValue)\\s*\\9)    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\13)*    \\s*>)
        //    (?>                                                                                               //1个左括号，但经测试发现，该括号不会被分组功能识别为一个组，即正则引擎在分组时会跳过这个括号，不会把这个括号中的内容当成一个分组。
        //        [^<>]+                                                                                        //匹配不含任何标签符号的部分的内容。
        //    |
        //        <[\\w-]+    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\17)*    \\s*?/>                       //匹配单独一个的 “开闭标签”（即类似于 “<img *** />”）。
        //    |
        //        <!--[\\s\\S]+?-->                                                                             //匹配注释标签。
        //    |
        //        <[Bb][Rr]\\s*>                                                                                //匹配不规则的换行符标签。
        //    |
        //        <[Hh][Rr]\\s*>                                                                                //匹配不规则的水平分割线标签。
        //    |
        //        (<([\\w-]+)    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\23)*    \\s*?>)    (?<DEPTH>)      //匹配正常的开始标签（形如：“<a href="">”、“<input >” 等）。
        //    |
        //        (</[\\w-]+>)    (?<-DEPTH>)                                                                   //匹配正常的结束标签（形如：“</a>”、“</input>” 等）。
        //    )*
        //    (?(DEPTH)(?!))                                                                                    //3个左括号
        //</\\2>
        #endregion
        /// <summary>
        /// 组别说明：
        /// --------------------------------------------------第 1 行--------------------------------------------------
        /// 组01：最外层元素的开始标签，不包含结束标签和任何的子元素。
        /// 组02：最外层元素的标签名（TagName）。
        /// 组03：最外层元素中的（在用户指定的 Attribute 之前的）普通 Attribute 部分（包括 AttributeName 和 AttributeValue）。
        /// 组04：组 3 中的 AttributeName 部分。
        /// 组05：组 3 中的 AttributeValue 的前引号（" 或 '）。
        /// 组06：组 3 中的 AttributeValue 部分。
        /// 组07：用户传入的整个 Attribute 部分。
        /// 组08：组 07 中的 AttributeName 部分。
        /// 组09：组 07 中的 AttributeValue 部分的前引号（" 或 '）。
        /// 组10：组 07 中的 AttributeValue 部分。
        /// 组11：最外层元素中的（在用户指定的 Attribute 之后的）普通 Attribute 部分（包括 AttributeName 和 AttributeValue）。
        /// 组12：组 11 中的 AttributeName 部分。
        /// 组13：组 11 中的 AttributeValue 的前引号（" 或 '）。
        /// 组14：组 11 中的 AttributeValue 部分。
        /// --------------------------------------------------第 5 行--------------------------------------------------
        /// 组15：最外层元素的子级中的自关闭的 Attribute 部分（包括 AttributeName 和 AttributeValue）。
        /// 组16：组 15 中的 AttributeName 部分。
        /// 组17：组 15 中的 AttributeValue 的前引号（" 或 '）。
        /// 组18：组 15 中的 AttributeValue 部分。
        /// --------------------------------------------------第 13 行--------------------------------------------------
        /// 组19：最外层元素的某个子元素的开始标签，不包含结束标签和任何当前元素的子元素。
        /// 组20：组 16 中的元素的标签名（TagName）。
        /// 组21：组 16 中的元素的普通 Attribute 部分（包括 AttributeName 和 AttributeValue）。
        /// 组22：组 16 中的 AttributeName 部分。
        /// 组23：组 16 中的 AttributeValue 的前引号（" 或 '）。
        /// 组24：组 16 中的 AttributeValue 部分。
        /// --------------------------------------------------第 15 行--------------------------------------------------
        /// 组25：最外层元素的某个子元素的结束标签。
        /// 组26：暂时未知。
        /// </summary>
        public const string MATCHING_SPECIFIED_ATTRIBUTE_ELEMENT_PATTERN =
            "(<([\\w-]+)    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\5)*    \\s+    ((attributeName)\\s*=\\s*([\"']?)\\s*(attributeValue)\\s*\\9)    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\13)*    \\s*>)" +    //14个左括号
            "    (?>" +                                                                                                     //1个左括号，但经测试发现，该括号不会被分组功能识别为一个组，即正则引擎在分组时会跳过这个括号，不会把这个括号中的内容当成一个分组。
            "        [^<>]+" +                                                                                              //匹配不含任何标签符号的部分的内容。
            "    |" +
            "        <[\\w-]+    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\17)*    \\s*?/>" +                             //匹配单独一个的 “开闭标签”（即类似于 “<img *** />”）。
            "    |" +
            "        <!--[\\s\\S]+?-->" +                                                                                   //匹配注释标签。
            "    |" +
            "        <[Bb][Rr]\\s*>" +                                                                                      //匹配不规则的换行符标签。
            "    |" +
            "        <[Hh][Rr]\\s*>" +                                                                                      //匹配不规则的水平分割线标签。
            "    |" +
            "        (<([\\w-]+)    (\\s+([\\w-]+)\\s*=\\s*([\"']?)([^\"<>]*?)\\23)*    \\s*?>)    (?<DEPTH>)" +            //匹配正常的开始标签（形如：“<a href="">”、“<input >” 等）。
            "    |" +
            "        (</[\\w-]+>)    (?<-DEPTH>)" +                                                                         //匹配正常的结束标签（形如：“</a>”、“</input>” 等）。
            "    )*" +
            "    (?(DEPTH)(?!))" +                                                                                          //3个左括号
            "</\\2>";
        #endregion

        public static List<string> GetElementTextsById(this string htmlContent, string idValue)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                throw new ArgumentNullException("htmlContent");
            }
            if (string.IsNullOrEmpty(idValue))
            {
                throw new ArgumentNullException("idValue");
            }
            else if (false == Regex.IsMatch(idValue, "^[^\"'<>]+$"))
            {
                throw new ArgumentException();
            }

            return GetElementTextsByAttribute(htmlContent, "id", idValue);
        }
        public static string GetElementTextById(this string htmlContent, string idValue)
        {
            return GetElementTextsById(htmlContent, idValue).Single();
        }

        public static List<string> GetElementTextsByAttribute(this string htmlContent, string attributeName, string attributeValue = "")
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                throw new ArgumentNullException("htmlContent");
            }
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName");
            }
            if (attributeValue == null)     // attributeValue，必须不为 null，但可以是 string.Empty（即 ""）。
            {
                throw new ArgumentNullException("attributeValue");
            }
            if (false == Regex.IsMatch(attributeName, "^[\\w-]+$"))
            {
                throw new ArgumentException();
            }
            if (false == Regex.IsMatch(attributeValue, "^[^\"'<>]*$"))
            {
                string errMsg = "参数 attributeValue 中含有非法字符（\"、'、< 和 > 其中之一）！";
                throw new ArgumentException(errMsg);
            }

            if (attributeValue == "")
            {
                attributeValue = "\\S*";
            }
            string matchPattern = MATCHING_SPECIFIED_ATTRIBUTE_ELEMENT_PATTERN.Replace("attributeName", attributeName).Replace("attributeValue", attributeValue);
            List<string> result = new List<string>();

            MatchCollection matches = Regex.Matches(htmlContent, matchPattern.ToString(), RegexOptions.IgnorePatternWhitespace);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    result.Add(match.Value);
                }
            }
            return result.Count == 0 ? null : result;
        }

        public static string GetAttribute(this string htmlElementText, string attributeName)
        {
            if (string.IsNullOrEmpty(htmlElementText))
            {
                throw new ArgumentNullException("htmlElementText");
            }
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException("attributeName");
            }
            if (false == Regex.IsMatch(attributeName, "^[\\w-]+$"))
            {
                throw new ArgumentException();
            }

            string attributeValuePattern = "[^\"'<>]*";

            string matchPattern = MATCHING_SPECIFIED_ATTRIBUTE_ELEMENT_PATTERN.Replace("attributeName", attributeName).Replace("attributeValue", attributeValuePattern);

            Match match = Regex.Match(htmlElementText, matchPattern, RegexOptions.IgnorePatternWhitespace);
            if (false == match.Success)
            {
                string errMsg = "所传入的 Html 元素字符串 htmlElementText 格式非法！";
                throw new ArgumentException(errMsg);
            }
            return match.Groups[10].Value;
        }
    }
}
