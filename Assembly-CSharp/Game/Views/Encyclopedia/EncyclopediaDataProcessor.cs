using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A55 RID: 2645
	public static class EncyclopediaDataProcessor
	{
		// Token: 0x17000E56 RID: 3670
		// (get) Token: 0x0600829D RID: 33437 RVA: 0x003CDB67 File Offset: 0x003CBD67
		public static string Language
		{
			get
			{
				return EncyclopediaDataProcessor._language;
			}
		}

		// Token: 0x0600829E RID: 33438 RVA: 0x003CDB70 File Offset: 0x003CBD70
		public static void Init()
		{
			string languageDir = "Language_" + LocalStringManager.CurLanguageType.ToString();
			bool flag = EncyclopediaDataProcessor._init && EncyclopediaDataProcessor._language != languageDir;
			if (!flag)
			{
				EncyclopediaDataProcessor._language = languageDir;
				EncyclopediaReference.Init();
				EncyclopediaContent.Init();
				EncyclopediaDataProcessor.Tables.Clear();
			}
		}

		// Token: 0x0600829F RID: 33439 RVA: 0x003CDBD4 File Offset: 0x003CBDD4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ErrorReport(string str)
		{
			Debug.LogWarning(str);
		}

		// Token: 0x060082A0 RID: 33440 RVA: 0x003CDBE0 File Offset: 0x003CBDE0
		public static string[] ParseStrArray(string str)
		{
			bool flag = str == string.Empty;
			string[] result;
			if (flag)
			{
				result = Array.Empty<string>();
			}
			else
			{
				bool flag2 = str.StartsWith("{") && str.EndsWith("}");
				if (flag2)
				{
					result = (from x in str.Substring(1, str.Length - 1 - 1).Split(',', StringSplitOptions.None)
					select x.Replace("\\n", "\n").Replace("\\u002c", ",").TrimStart()).ToArray<string>();
				}
				else
				{
					EncyclopediaDataProcessor.ErrorReport("[" + str + "]格式有误");
					result = (from x in str.ColorReplace().Split(',', StringSplitOptions.None)
					select x.Replace("\\n", "\n").Replace("\\u002c", ",").TrimStart()).ToArray<string>();
				}
			}
			return result;
		}

		// Token: 0x060082A1 RID: 33441 RVA: 0x003CDCBF File Offset: 0x003CBEBF
		public static string[] ParseStrArray(this string[] data, int index)
		{
			return (data.Length > index) ? EncyclopediaDataProcessor.ParseStrArray(data[index]) : Array.Empty<string>();
		}

		// Token: 0x060082A2 RID: 33442 RVA: 0x003CDCD6 File Offset: 0x003CBED6
		public static string ParseStr(this string[] data, int index)
		{
			return (data.Length > index) ? data[index].Replace("\\n", "\n").Replace("\\u002c", ",").ColorReplace() : string.Empty;
		}

		// Token: 0x060082A3 RID: 33443 RVA: 0x003CDD0C File Offset: 0x003CBF0C
		public static T Parse<T>(string data, T defaultValue, Dictionary<string, string> mapping = null) where T : struct
		{
			bool flag = string.IsNullOrEmpty(data);
			T result2;
			if (flag)
			{
				result2 = defaultValue;
			}
			else
			{
				string result;
				bool flag2 = mapping != null && mapping.TryGetValue(data, out result);
				if (flag2)
				{
					data = result;
				}
				T output;
				bool flag3 = Enum.TryParse<T>(data, out output);
				if (flag3)
				{
					result2 = output;
				}
				else
				{
					EncyclopediaDataProcessor.ErrorReport(string.Format("{0} cannot be convert to {1}", data, typeof(T)));
					result2 = defaultValue;
				}
			}
			return result2;
		}

		// Token: 0x060082A4 RID: 33444 RVA: 0x003CDD72 File Offset: 0x003CBF72
		public static T Parse<T>(this string[] data, int index, T defaultValue, Dictionary<string, string> mapping = null) where T : struct
		{
			return (data.Length <= index) ? defaultValue : EncyclopediaDataProcessor.Parse<T>(data[index], defaultValue, mapping);
		}

		// Token: 0x060082A5 RID: 33445 RVA: 0x003CDD88 File Offset: 0x003CBF88
		public static T[] ParseArray<T>(this string[] data, int index, T defaultValue, Dictionary<string, string> mapping = null) where T : struct
		{
			return (from x in data.ParseStrArray(index)
			select EncyclopediaDataProcessor.Parse<T>(x, defaultValue, mapping)).ToArray<T>();
		}

		// Token: 0x060082A6 RID: 33446 RVA: 0x003CDDC8 File Offset: 0x003CBFC8
		public static string[][] GetTable(string tableName)
		{
			string[][] table;
			bool flag = EncyclopediaDataProcessor.Tables.TryGetValue(tableName, out table);
			string[][] result;
			if (flag)
			{
				result = table;
			}
			else
			{
				table = (from l in File.ReadLines(Path.Combine(Application.streamingAssetsPath, EncyclopediaDataProcessor.Language, "EncyclopediaAssets", tableName + ".tsv"))
				where !string.IsNullOrEmpty(l)
				select l.Replace("\\n", "\n").Replace("\\u002c", ",").ColorReplace().Split('\t', StringSplitOptions.None).ToArray<string>()).ToArray<string[]>();
				EncyclopediaDataProcessor.Tables.Add(tableName, table);
				result = table;
			}
			return result;
		}

		// Token: 0x060082A7 RID: 33447 RVA: 0x003CDE74 File Offset: 0x003CC074
		public static int GetTableExtraHeaderCount(string refName)
		{
			EncyclopediaReferenceItem encyclopediaReferenceItem = EncyclopediaReference.Instance[refName];
			string[] data = (encyclopediaReferenceItem != null) ? encyclopediaReferenceItem.Params : null;
			int count;
			return (data != null && data.Length > 0 && int.TryParse(data[0], out count)) ? count : 0;
		}

		// Token: 0x040063EC RID: 25580
		internal static bool _init = false;

		// Token: 0x040063ED RID: 25581
		private static string _language = string.Empty;

		// Token: 0x040063EE RID: 25582
		private static readonly Dictionary<string, string[][]> Tables = new Dictionary<string, string[][]>();
	}
}
