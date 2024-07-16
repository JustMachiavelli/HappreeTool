using System.Xml.Linq;
using System.Xml.XPath;

namespace HappreeTool.Documents
{
    public static class XmlUtils
    {
        /// <summary>
        /// 确认xml文件中存在等于预期内容的指定层次路径的node
        /// </summary>
        /// <param name="xmlPath">xml文件路径</param>
        /// <param name="nodePath">node层次路径</param>
        /// <param name="expected">预期的内容</param>
        /// <returns></returns>
        public static bool ExistExpectedTextInSpecificNode(string xmlPath, string nodePath, string expected)
        {
            try
            {
                XDocument doc = XDocument.Load(xmlPath);
                IEnumerable<XElement> subNodes = doc.XPathSelectElements(nodePath);
                return subNodes.Any(subNode => subNode.Value == expected);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
