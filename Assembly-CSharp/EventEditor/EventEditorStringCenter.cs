using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameData.Domains.Item;

namespace EventEditor
{
	// Token: 0x02000635 RID: 1589
	public class EventEditorStringCenter
	{
		// Token: 0x06004B28 RID: 19240 RVA: 0x00235360 File Offset: 0x00233560
		public static string DecodeTag(string targetString)
		{
			return EventEditorStringCenter.TagRegex.Replace(targetString, delegate(Match tagMatch)
			{
				string tagString = tagMatch.Value;
				MatchCollection collections = EventEditorStringCenter.PairRegex.Matches(tagString);
				foreach (object obj in collections)
				{
					Match i = (Match)obj;
					bool flag = i.Groups.Count < 3;
					if (!flag)
					{
						EventEditorStringCenter._pairInfos.Add(i.Groups["Name"].Value, i.Groups["Value"].Value);
					}
				}
				Match match = EventEditorStringCenter.TagNameRegex.Match(tagString);
				string tagName = match.Groups["TagName"].Value;
				string text = tagName;
				string a = text;
				string resultString;
				if (!(a == "Character"))
				{
					if (!(a == "Item"))
					{
						resultString = tagString;
					}
					else
					{
						resultString = EventEditorStringCenter.DecodeItem();
					}
				}
				else
				{
					resultString = EventEditorStringCenter.DecodeCharacter();
				}
				EventEditorStringCenter._pairInfos.Clear();
				bool flag2 = string.IsNullOrEmpty(resultString);
				if (flag2)
				{
					resultString = "{Error}";
				}
				return resultString;
			});
		}

		// Token: 0x06004B29 RID: 19241 RVA: 0x0023539C File Offset: 0x0023359C
		private static string DecodeCharacter()
		{
			string characterKey = EventEditorStringCenter._pairInfos["key"];
			EventEditorRole eventEditorRole = EventEditorSimulateEnvironment.Instance.GetRole(characterKey);
			bool flag = eventEditorRole == null;
			string result;
			if (flag)
			{
				result = characterKey;
			}
			else
			{
				string attrKey = EventEditorStringCenter._pairInfos["str"];
				string text = attrKey;
				string a = text;
				if (!(a == "Name"))
				{
					result = string.Empty;
				}
				else
				{
					result = eventEditorRole.GetName();
				}
			}
			return result;
		}

		// Token: 0x06004B2A RID: 19242 RVA: 0x00235410 File Offset: 0x00233610
		private static string DecodeItem()
		{
			string itemKey;
			bool flag = EventEditorStringCenter._pairInfos.TryGetValue("key", out itemKey);
			sbyte itemType;
			short itemTemplateId;
			string iconName;
			if (flag)
			{
				EventEditorItem item = EventEditorSimulateEnvironment.Instance.GetItem(itemKey);
				bool flag2 = item == null;
				if (flag2)
				{
					return itemKey;
				}
				itemType = item.Type;
				itemTemplateId = item.TemplateId;
				iconName = item.Icon;
			}
			else
			{
				string itemTypeString;
				string itemIdString;
				bool flag3 = !EventEditorStringCenter._pairInfos.TryGetValue("type", out itemTypeString) || !ItemType.TypeName2TypeId.TryGetValue(itemTypeString, out itemType) || !EventEditorStringCenter._pairInfos.TryGetValue("id", out itemIdString) || !short.TryParse(itemIdString, out itemTemplateId);
				if (flag3)
				{
					return string.Empty;
				}
				iconName = ItemTemplateHelper.GetIcon(itemType, itemTemplateId);
			}
			string result = string.Empty;
			string strKey;
			bool flag4 = EventEditorStringCenter._pairInfos.TryGetValue("str", out strKey);
			if (flag4)
			{
				string text = strKey;
				string a = text;
				if (!(a == "Name"))
				{
					if (a == "ColorName")
					{
						result = ItemUtils.GetItemColorName(itemType, itemTemplateId);
					}
				}
				else
				{
					result = ItemTemplateHelper.GetName(itemType, itemTemplateId);
				}
			}
			string itemSprite;
			bool flag5 = EventEditorStringCenter._pairInfos.TryGetValue("sp", out itemSprite);
			if (flag5)
			{
				string text2 = itemSprite;
				string a2 = text2;
				if (a2 == "Icon")
				{
					result = result + "<sprite name=\"" + iconName + "\"";
				}
			}
			return result;
		}

		// Token: 0x04003431 RID: 13361
		private static readonly Regex TagRegex = new Regex("<(?!(/)?color|(/)?size|(/)?link).*?/>");

		// Token: 0x04003432 RID: 13362
		private static readonly Regex TagNameRegex = new Regex("<(?<TagName>(?!(/)?color|(/)?size|(/)?link)[a-z|A-Z]+)( +)?");

		// Token: 0x04003433 RID: 13363
		private static readonly Regex PairRegex = new Regex("(?<Name>(?!(/)?color|(/)?size|(/)?link)[a-z|A-Z]+)( +)?=( +)?(?<Value>[\\w]+)");

		// Token: 0x04003434 RID: 13364
		private static Dictionary<string, string> _pairInfos = new Dictionary<string, string>();
	}
}
