using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace GILES
{
	/**
	 * Helper methods for working with strings.
	 */
	public static class pb_StringUtil
	{
		/**
		 * Convert a collection of items to a string.
		 */
		public static string ToStringF(this IEnumerable val)
		{
			return val.ToStringF('\n');
		}

		/**
		 * Convert a collection of items to a string with delimiter.
		 */
		public static string ToStringF(this IEnumerable val, char delimiter)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach(var obj in val)
			{
				sb.Append(obj.ToString());
				sb.Append(delimiter);
			}

			return sb.ToString();
		}

		/**
		 * Convert a string from camel case to spaced and caps'd.
		 * Ex: myVariableName becomes My Variable Name
		 */
		public static string SplitCamelCase(this string str)
		{
			string split = Regex.Replace(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})", " ", RegexOptions.Compiled).Trim();

			char[] split_chars = split.ToCharArray();
			split_chars[0] = char.ToUpper(split_chars[0]);
			return new string(split_chars);

			// return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
		}

		/**
		 * Trunate string to length.
		 */
		public static string Truncate(this string value, int length)
		{
			if(value.Length > length)
				return value.Substring(0, length);
			else
				return value;
		}
	}
}