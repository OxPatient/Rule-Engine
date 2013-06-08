using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Yea.Localization
{
	public class Locale : IComparable<Locale>
	{
		public readonly Dictionary<string, StringCollection> StringCollections = new Dictionary<string, StringCollection>();

		public string Key { get; private set; }
		public string Name { get; set; }
		public bool RightToLeft { get; set; }
		public string ParentLocale { get; set; }

        public string XmlPath { private set; get; }
		private bool _fullyLoaded;

		public int CompareTo(Locale other)
		{
			return String.Compare(Key, other.Key, StringComparison.Ordinal);
		}

		public Locale(string localeKey)
		{
			Key = localeKey;
			RightToLeft = false;
		}

		public StringTranslation GetString(string collectionKey, string key)
		{
			if (!_fullyLoaded)
			{
				Load(XmlPath);
			}

			return StringCollections[collectionKey].StringsTable[key];
		}

		public override string ToString()
		{
			return string.IsNullOrEmpty(Name) ? Key : Name;
		}

		public static explicit operator string(Locale l)
		{
			return l.ToString();
		}

		private void LoadPropertiesXml(string xmlPath)
		{
			XmlPath = xmlPath;
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(xmlPath);

			var node = xmlDocument.SelectSingleNode(@"/localization/locale");
			if (node == null)
				throw new IncompleteLocaleException("The required locale element 'locale' was not found.");

			node = xmlDocument.SelectSingleNode(@"/localization/locale/name");
			if (node == null)
				throw new IncompleteLocaleException("The required locale element 'name' was not found.");
			Name = node.InnerText;

			node = xmlDocument.SelectSingleNode(@"/localization/locale/rtl");
			RightToLeft = node != null && node.InnerText == "true";

			node = xmlDocument.SelectSingleNode(@"/localization/locale/parentLocale");
			ParentLocale = node != null ? node.InnerText : null;
		}

		private void SavePropertiesXml(string xmlPath)
		{
			var xmlDocument = new XmlDocument();

			var xmlDeclaration = xmlDocument.CreateXmlDeclaration(@"1.0", @"utf-8", null);

			xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

			var xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement(@"localization"));
			xmlNode = xmlNode.AppendChild(xmlDocument.CreateElement(@"locale"));

			xmlNode.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, @"name", @"")).InnerText = Name;
			xmlNode.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, @"rtl", @"")).InnerText = RightToLeft.ToString().ToLower();
			xmlNode.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, @"parentLocale", @"")).InnerText = ParentLocale;

			xmlDocument.Save(xmlPath);
		}

		//Loads only properties.xml for DisplayName enumeration
		public bool LoadProperties(string xmlPath)
		{
			if (!File.Exists(xmlPath))
				return false;

			var folder = Path.GetDirectoryName(xmlPath);
			if (string.IsNullOrEmpty(folder))
				return false;

			StringCollections.Clear();
			LoadPropertiesXml(xmlPath);

			return true;
		}

		public bool Load(string xmlPath)
		{
			if (!File.Exists(xmlPath))
				return false;

			var folder = Path.GetDirectoryName(xmlPath);
			if (string.IsNullOrEmpty(folder))
				return false;

			StringCollections.Clear();
			LoadPropertiesXml(xmlPath);

			foreach (var stringFile in Directory.GetFiles(folder, @"*.xml"))
			{
				if (string.Compare(stringFile, xmlPath, true) == 0)
					continue;

				var stringKey = Path.GetFileNameWithoutExtension(stringFile);
				if (string.IsNullOrEmpty(stringKey))
					continue;

				var stringCollection = new StringCollection(stringKey);
				stringCollection.Load(stringFile);

				StringCollections[stringKey] = stringCollection;
			}

			_fullyLoaded = true;

			return true;
		}

		public bool Save(string xmlPath, StringCollection collection = null, bool exportStrings = true)
		{
			xmlPath = Path.GetFullPath(xmlPath);
			SavePropertiesXml(xmlPath);

			if (exportStrings)
			{
				var folder = Path.GetDirectoryName(xmlPath);
				if (string.IsNullOrEmpty(folder))
					return false;

				if (collection == null)
				{
					foreach (var sCollection in StringCollections.Values)
					{
						sCollection.Save(Path.Combine(folder, sCollection.Key + @".xml"));
					}
				}
				else
				{
					collection.Save(Path.Combine(folder, collection.Key + @".xml"));
				}
			}

			return true;
		}

		public bool Save(StringCollection collection = null)
		{
			if(string.IsNullOrEmpty(XmlPath))
				throw new Exception("Calling save without save as!");

			return Save(XmlPath, collection);
		}

		public void Cleanup()
		{
			if(string.IsNullOrEmpty(ParentLocale))
				return;

			var manager = new LocaleManager();

			foreach(var collection in StringCollections.Values)
			{
				var deleteList = new List<string>();
				foreach(var key in collection.StringsTable.Keys)
				{
					try
					{
						var translation = manager.Locales[ParentLocale].GetString(collection.Key, key);
						if (translation.AliasedKey)
						{
							deleteList.Add(key);
						}
					}
                    catch (KeyNotFoundException)
                    {
                        deleteList.Add(key);
                    }
				}

				foreach(var key in deleteList)
				{
					collection.StringsTable.Remove(key);
				}
			}
		}
	}
}
