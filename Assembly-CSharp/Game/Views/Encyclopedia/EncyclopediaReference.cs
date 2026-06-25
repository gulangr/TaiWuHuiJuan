using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A62 RID: 2658
	[Serializable]
	public class EncyclopediaReference
	{
		// Token: 0x060082B2 RID: 33458 RVA: 0x003CE544 File Offset: 0x003CC744
		public static void Init()
		{
			EncyclopediaReference.DataArray.Clear();
			EncyclopediaReference.DataDict.Clear();
			foreach (string line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, EncyclopediaDataProcessor.Language, "EncyclopediaAssets", "EncyclopediaReference.tsv")))
			{
				try
				{
					string[] data = line.Split('\t', StringSplitOptions.None);
					int id = EncyclopediaReference.DataArray.Count;
					string linkId = data.ParseStr(0);
					EEncyclopediaReferenceInsertType insertType = data.Parse(1, EEncyclopediaReferenceInsertType.Invalid, EnumMap.InsertType);
					string param = data.ParseStr(2);
					string[] stringParams = data.ParseStrArray(3);
					string[] desc = data.ParseStrArray(4);
					string title = data.ParseStr(5);
					EncyclopediaReference.DataArray.Add(new EncyclopediaReferenceItem(id, linkId, insertType, param, stringParams, desc, title));
					EncyclopediaReference.DataDict[linkId] = id;
				}
				catch (Exception e)
				{
					EncyclopediaDataProcessor.ErrorReport(string.Format("处理{0}时失败: {1}", line, e));
				}
			}
		}

		// Token: 0x17000E5C RID: 3676
		public EncyclopediaReferenceItem this[int index]
		{
			get
			{
				return (index < EncyclopediaReference.DataArray.Count) ? EncyclopediaReference.DataArray[index] : null;
			}
		}

		// Token: 0x17000E5D RID: 3677
		public EncyclopediaReferenceItem this[string id]
		{
			get
			{
				int index;
				bool flag = EncyclopediaReference.DataDict.TryGetValue(id, out index);
				EncyclopediaReferenceItem result;
				if (flag)
				{
					result = this[index];
				}
				else
				{
					Debug.LogWarning("EncyclopediaReferenceItem id [" + id + "] is not valid.");
					result = null;
				}
				return result;
			}
		}

		// Token: 0x04006449 RID: 25673
		private static EncyclopediaReference _instance;

		// Token: 0x0400644A RID: 25674
		public static readonly List<EncyclopediaReferenceItem> DataArray = new List<EncyclopediaReferenceItem>();

		// Token: 0x0400644B RID: 25675
		public static readonly Dictionary<string, int> DataDict = new Dictionary<string, int>();

		// Token: 0x0400644C RID: 25676
		public static EncyclopediaReference Instance = new EncyclopediaReference();
	}
}
