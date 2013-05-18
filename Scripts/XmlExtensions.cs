using UnityEngine;
using System.Xml;

namespace XmlExtensions {
	/// <summary>
	/// Helper methods for parsing XML atlas data file
	/// </summary>
    public static class XmlNodeExtensions {
        public static Rect GetRect(this XmlNode node, string name) {
            XmlNode valueNode = node.GetValueForKey(name);
            return (valueNode != null) ? StringToRect(valueNode.InnerText) : new Rect(0, 0, 0, 0);
        }

        public static Vector2 GetVector2(this XmlNode node, string name) {
            XmlNode valueNode = node.GetValueForKey(name);
            return (valueNode != null) ? StringToVector2(valueNode.InnerText) : Vector2.zero;
        }

        public static bool GetBool(this XmlNode node, string name) {
            XmlNode valueNode = node.GetValueForKey(name);
            return (valueNode != null) ? (valueNode.Name.ToLower () == "true") : false;
        }

        public static XmlNode GetValueForKey(this XmlNode node, string name) {
            XmlNode keyNode = node.SelectSingleNode("key[.='" + name + "']");
            return (keyNode != null) ? keyNode.NextSibling : null;
        }

        /* *** Private Methods *** */

        private static Vector2 StringToVector2(string s) {
            string _s = s.Substring(1, s.Length - 2);
            string[] sa = _s.Split(',');
            return new Vector2(System.Convert.ToSingle(sa[0]), System.Convert.ToSingle(sa[1]));
        }

        private static Rect StringToRect(string s) {
            string _s = s.Substring(1, s.Length - 2);
            string[] sa = _s.Split(new string[] { "},{" }, System.StringSplitOptions.None);
            Vector2 v1 = StringToVector2(sa[0] + "}");
            Vector2 v2 = StringToVector2("{" + sa[1]);
            return new Rect(v1.x, v1.y, v2.x, v2.y);
        }
    }
}
