#region Usings

using System.Collections.Generic;
using System.IO;
using System.Xml;

#endregion

namespace Yea.Localization
{
    public class StringCollection
    {
        private readonly Dictionary<string, StringTranslation> _strings = new Dictionary<string, StringTranslation>();

        public StringCollection(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }

        public string this[string key]
        {
            get
            {
                try
                {
                    var temp = StringsTable[key];
                    if (temp.AliasedKey)
                    {
                        return this[temp.CloneOf];
                    }
                    if (temp.DeriveFromParent)
                    {
                        return null;
                    }

                    return temp.Value;
                }
                catch (KeyNotFoundException)
                {
                    throw new StringNotFoundException(key);
                }
            }
            set
            {
                if (StringsTable.ContainsKey(key))
                {
                    StringsTable[key] = new StringTranslation(key, value);
                }
                else
                {
                    StringsTable[key].Value = value;
                }
            }
        }

        public Dictionary<string, StringTranslation> StringsTable
        {
            get { return _strings; }
        }

        public void Save(string xmlPath)
        {
            var xmlDocument = new XmlDocument();

            var xmlDeclaration = xmlDocument.CreateXmlDeclaration(@"1.0", @"utf-8", null);

            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            var xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement(@"localization"));
            xmlNode = xmlNode.AppendChild(xmlDocument.CreateElement(@"strings"));

            foreach (var entry in _strings.Values)
            {
                var stringNode = xmlDocument.CreateElement(@"string");

                stringNode.SetAttribute(@"key", entry.Key);

                if (entry.AliasedKey)
                {
                    stringNode.SetAttribute(@"clone", entry.CloneOf);
                }
                else if (entry.DeriveFromParent)
                {
                    stringNode.SetAttribute(@"derive", @"true");
                }
                else if (!string.IsNullOrEmpty(entry.Value))
                {
                    stringNode.SetAttribute(@"value", Utils.NormalizeLineBreaks(entry.Value));

                    if (entry.BumpVersion)
                        ++entry.Version;

                    if (entry.Version != 0)
                        stringNode.SetAttribute(@"version", entry.Version.ToString());
                }
                else
                {
                    continue;
                }

                xmlNode.AppendChild(stringNode);
            }

            xmlDocument.Save(xmlPath);
        }

        public void Load(string xmlPath)
        {
            Key = Path.GetFileNameWithoutExtension(xmlPath);
            string localeKey = Path.GetFileName(Path.GetDirectoryName(xmlPath));

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);

            var node = xmlDocument.SelectSingleNode(@"/localization/strings");
            if (node == null)
                throw new IncompleteLocaleException("The required locale element 'strings' was not found.");

            var strings = xmlDocument.SelectNodes(@"/localization/strings/string");
            if (strings != null)
            {
                foreach (XmlNode text in strings)
                {
                    if (text.Attributes == null)
                        throw new MalformedStringException(
                            "Invalid translation string found. Attribute 'key' is required.");

                    var temp = text.Attributes["key"];

                    if (temp == null || string.IsNullOrEmpty(temp.InnerText))
                        throw new MalformedStringException(
                            "Invalid translation string found. Attribute 'key' is required.");

                    var key = temp.InnerText;

                    var value = text.Attributes["value"] != null ? text.Attributes["value"].InnerText : null;
                    var clone = text.Attributes["clone"] != null ? text.Attributes["clone"].InnerText : null;
                    var derive = text.Attributes["derive"] != null && text.Attributes["derive"].InnerText == @"true";

                    uint version = 0;
                    var versionElement = text.Attributes["version"];
                    if (versionElement != null)
                    {
                        version = uint.Parse(versionElement.InnerText);
                    }

                    if (_strings.ContainsKey(key))
                        throw new DuplicateKeyException(string.Format(
                            "Key {0} in {1}\\{2} was defined more than once.", key, localeKey, Key));

                    var newString = new StringTranslation(key);

                    if (value != null)
                    {
                        newString.Value = value;
                    }
                    else if (!string.IsNullOrEmpty(clone))
                    {
                        newString.CloneOf = clone;
                    }
                    else if (derive)
                    {
                        newString.DeriveFromParent = true;
                    }
                    else
                    {
                        //throw new MalformedStringException(string.Format("Invalid translation string {0} found. One of attributes 'value', 'clone', or 'derive' is required.", key));
                        continue;
                    }

                    _strings[newString.Key] = newString;
                    newString.Version = version;
                }
            }
        }

        public void Merge(StringCollection newStrings, bool overwriteExisting = true)
        {
            Merge(newStrings.StringsTable.Values, overwriteExisting);

            return;
        }

        public void Merge(IEnumerable<StringTranslation> newStrings, bool overwriteExisting = true)
        {
            foreach (var entry in newStrings)
            {
                if (StringsTable.ContainsKey(entry.Key) && !overwriteExisting)
                    continue;

                StringsTable[entry.Key] = entry;
            }
        }
    }
}