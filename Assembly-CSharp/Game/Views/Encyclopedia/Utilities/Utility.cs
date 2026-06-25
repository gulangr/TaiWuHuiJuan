using System;
using System.Text.RegularExpressions;

namespace Game.Views.Encyclopedia.Utilities
{
	// Token: 0x02000A76 RID: 2678
	internal static class Utility
	{
		// Token: 0x060083C9 RID: 33737 RVA: 0x003D4F70 File Offset: 0x003D3170
		internal static bool IsValidNodeId(int nodeId)
		{
			return nodeId > Utility.InValidNodeId;
		}

		// Token: 0x060083CA RID: 33738 RVA: 0x003D4F8A File Offset: 0x003D318A
		internal static string GetTextWithDisplayLevel(string src, EEncyclopediaContentLevel level)
		{
			return src.ColorReplace();
		}

		// Token: 0x060083CB RID: 33739 RVA: 0x003D4F94 File Offset: 0x003D3194
		internal static string GetHighlightText(string src, NodeData nodeData, OptimizedHtmlPatternMatcher matcher, int index, bool selecting = false)
		{
			return Utility.GetTextWithDisplayLevel(((matcher != null) ? matcher.Pattern : null).IsNullOrEmpty() ? src.ParseText() : src.ParseHighlightText().ApplyHighlightColor(matcher, selecting, index), (nodeData.Layer == EEncyclopediaContentLayer.Content) ? nodeData.Level : EEncyclopediaContentLevel.None);
		}

		// Token: 0x060083CC RID: 33740 RVA: 0x003D4FE4 File Offset: 0x003D31E4
		internal static string GetHighlightText(string src, EEncyclopediaContentLevel level, OptimizedHtmlPatternMatcher matcher, int index, bool selecting = false)
		{
			return string.IsNullOrEmpty(src) ? "" : Utility.GetTextWithDisplayLevel(((matcher != null) ? matcher.Pattern : null).IsNullOrEmpty() ? src.ParseText() : src.ParseHighlightText().ApplyHighlightColor(matcher, selecting, index), level);
		}

		// Token: 0x060083CD RID: 33741 RVA: 0x003D5030 File Offset: 0x003D3230
		internal static string GetValidInputString(string src)
		{
			return (src.Contains('{') || src.Contains('}')) ? "{-}" : src;
		}

		// Token: 0x060083CE RID: 33742 RVA: 0x003D504E File Offset: 0x003D324E
		internal static string NormalText(this string src)
		{
			return Utility._htmlTagRemover.Replace(src, "").ParseText();
		}

		// Token: 0x060083CF RID: 33743 RVA: 0x003D5068 File Offset: 0x003D3268
		internal static bool NormalTextContains(this string src, string value, out int resultCount)
		{
			bool flag = value.Contains('\\');
			bool result;
			if (flag)
			{
				resultCount = 0;
				result = false;
			}
			else
			{
				MatchCollection matches = Regex.Matches(src.NormalText(), value, RegexOptions.IgnoreCase);
				resultCount = matches.Count;
				result = (resultCount > 0);
			}
			return result;
		}

		// Token: 0x040064E4 RID: 25828
		internal static int InValidNodeId = -1;

		// Token: 0x040064E5 RID: 25829
		internal static int maxFavoriteBarSaveNum = 8;

		// Token: 0x040064E6 RID: 25830
		private static Regex _hypers = new Regex("<color=([^>]+)><u><link=\"([^\"]+)\">", RegexOptions.Compiled);

		// Token: 0x040064E7 RID: 25831
		private static readonly Regex _htmlTagRemover = new Regex("<[^>]*>");
	}
}
