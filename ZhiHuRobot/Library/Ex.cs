using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhiHuRobot.Library
{
    public static class Ex
    {
        public static string DumpToString(this byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data);
        }
        public static List<HtmlNode> GetElements(this HtmlNode doc, string tagName, string property, Func<string, bool> filter)
        {
            var query = from node in doc.SelectNodes(String.Format("//{0}[@{1}]", tagName, property))
                        let propValue = node.GetAttributeValue(property, "")
                        where filter(propValue)
                        select node;

            return query.ToList();
        }
        public static List<HtmlNode> GetElements(this HtmlNode doc, string tagName, string property, string value)
        {
            return doc.GetElements(tagName, property, propVal => propVal == value);
        }
        public static List<HtmlNode> GetElementsByClassName(this HtmlNode doc, string tagName, string className)
        {
            return doc.GetElements(tagName, "class", propValue =>
            {
                propValue = (propValue ?? "").Trim();
                if (String.IsNullOrWhiteSpace(propValue)) return false;
                return propValue.Split(' ').Select(c => c.Trim()).Any(c => c == className);
            });
        }
        public static HtmlNode GetElementByClassName(this HtmlNode doc, string tagName, string className)
        {
            return doc.GetElementsByClassName(tagName, className).FirstOrDefault();
        }


        public static List<HtmlNode> GetElements(this HtmlDocument doc, string tagName, string property, Func<string, bool> filter)
        {

            return doc.DocumentNode.GetElements(tagName, property, filter);
        }
        public static List<HtmlNode> GetElements(this HtmlDocument doc, string tagName, string property, string value)
        {
            return doc.DocumentNode.GetElements(tagName, property, value);
        }
        public static List<HtmlNode> GetElementsByClassName(this HtmlDocument doc, string tagName, string className)
        {
            return doc.DocumentNode.GetElementsByClassName(tagName, className);
        }
        public static HtmlNode GetElementByClassName(this HtmlDocument doc, string tagName, string className)
        {
            return doc.DocumentNode.GetElementByClassName(tagName, className);
        }


    }
}
