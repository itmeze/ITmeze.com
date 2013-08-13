using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITmeze.Core.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrWhitespace(this string text)
		{
			return string.IsNullOrWhiteSpace(text);
		}

		public static string FormatWith(this string text, params object[] args)
		{
			return string.Format(text, args);
		}

		public static string ToSlug(this string value)
		{

			value = value.ToLowerInvariant();

			var byset = Encoding.GetEncoding("Cyrillic").GetBytes(value);
			value = Encoding.ASCII.GetString(byset);

			value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

			value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

			value = value.Trim('-', '_');

			value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

			return value;
		}
	}
}
