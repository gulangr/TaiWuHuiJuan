using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A51 RID: 2641
	[Serializable]
	public class EncyclopediaContent
	{
		// Token: 0x17000E48 RID: 3656
		public EncyclopediaContentItem this[int index]
		{
			get
			{
				return (index < EncyclopediaContent.DataArray.Count) ? EncyclopediaContent.DataArray[index] : null;
			}
		}

		// Token: 0x17000E49 RID: 3657
		public EncyclopediaContentItem this[string id]
		{
			get
			{
				int index;
				return EncyclopediaContent.DataDict.TryGetValue(id, out index) ? this[index] : null;
			}
		}

		// Token: 0x06008279 RID: 33401 RVA: 0x003CCE40 File Offset: 0x003CB040
		public static void Init()
		{
			EncyclopediaContent.DataArray.Clear();
			EncyclopediaContent.DataDict.Clear();
			foreach (string line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, EncyclopediaDataProcessor.Language, "EncyclopediaAssets", "EncyclopediaContent.tsv")))
			{
				try
				{
					string[] data = line.Split('\t', StringSplitOptions.None);
					int id = EncyclopediaContent.DataArray.Count;
					string title = data.ParseStr(0);
					string title2 = data.ParseStr(1);
					string title3 = data.ParseStr(2);
					string title4 = data.ParseStr(3);
					string title5 = data.ParseStr(4);
					EEncyclopediaContentLayer layer = data.Parse(5, EEncyclopediaContentLayer.Content, EnumMap.Layer);
					string content = data.ParseStr(6);
					EEncyclopediaContentLevel level = data.Parse(7, EEncyclopediaContentLevel.Mid, EnumMap.Level);
					EEncyclopediaContentFonts fonts = data.Parse(8, EEncyclopediaContentFonts.Default, EnumMap.Fonts);
					EEncyclopediaContentLayout[] layout = data.ParseArray(9, EEncyclopediaContentLayout.None, EnumMap.Layout);
					string[] enabledHyperLinks = data.ParseStrArray(10);
					int[] inserts = (from x in data.ParseStrArray(11)
					select EncyclopediaReference.DataDict.GetValueOrDefault(x, -1) into x
					where x >= 0
					select x).ToArray<int>();
					string key = data.ParseStr(12);
					EncyclopediaContent.DataArray.Add(new EncyclopediaContentItem(id, title, title2, title3, title4, title5, layer, content, level, fonts, layout, enabledHyperLinks, inserts, key));
					EncyclopediaContent.DataDict[key] = id;
				}
				catch (Exception e)
				{
					EncyclopediaDataProcessor.ErrorReport(string.Format("处理{0}时失败: {1}", line, e));
				}
			}
		}

		// Token: 0x17000E4A RID: 3658
		// (get) Token: 0x0600827A RID: 33402 RVA: 0x003CD02C File Offset: 0x003CB22C
		public int Count
		{
			get
			{
				return EncyclopediaContent.DataArray.Count;
			}
		}

		// Token: 0x040063D3 RID: 25555
		public static readonly List<EncyclopediaContentItem> DataArray = new List<EncyclopediaContentItem>();

		// Token: 0x040063D4 RID: 25556
		public static readonly Dictionary<string, int> DataDict = new Dictionary<string, int>();

		// Token: 0x040063D5 RID: 25557
		public static readonly EncyclopediaContent Instance = new EncyclopediaContent();
	}
}
