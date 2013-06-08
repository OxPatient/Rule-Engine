using System;

namespace Yea.Localization
{
	public class LocalizationException : Exception
	{
		public LocalizationException(string message = "")
			: base(message)
		{
		}
	}

	public class DefaultLocaleNotFoundException : LocalizationException
	{
	}

	public class IncompleteLocaleException : LocalizationException
	{
		public IncompleteLocaleException(string message = "")
			: base(message)
		{
		}
	}

	public class MalformedStringException : LocalizationException
	{
		public MalformedStringException(string message = "")
			: base(message)
		{
		}
	}

	public class DuplicateKeyException : LocalizationException
	{
		public DuplicateKeyException(string message = "")
			: base(message)
		{
		}
	}

	public class DefaultCollectionKeyNotSet : LocalizationException
	{
		public DefaultCollectionKeyNotSet(string message = "")
			: base(message)
		{
		}
	}

	public class StringNotFoundException : LocalizationException
	{
		public StringNotFoundException(string message)
			: base(message)
		{
		}
	}
}
