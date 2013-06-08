namespace Yea.Localization
{
	public class StringTranslation
	{
		public string Key { get; set; }

		public string Value { get; set; }

		public bool DeriveFromParent { get; set; }

		public bool BumpVersion { get; set; }

		public uint Version { get; set; }

		public string CloneOf { get; internal set; }

		public bool AliasedKey
		{
			get { return !string.IsNullOrEmpty(CloneOf); }
		}

		public StringTranslation(string key, string value = @"")
		{
			Key = key;
			Value = value;
			BumpVersion = false;
		}

		public override string ToString()
		{
			return Value;
		}

		public static implicit operator string(StringTranslation st)
		{
			return st.ToString();
		}
	}
}
