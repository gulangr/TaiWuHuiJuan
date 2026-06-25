using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.ModSystem;
using MoonSharp.Interpreter;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class EncyclopediaStructure : ScriptableObject
{
	// Token: 0x04000DA1 RID: 3489
	public List<EncyclopediaStructure.EncyclopediaVolData> VolDataList;

	// Token: 0x02001177 RID: 4471
	[Serializable]
	public class EncyclopediaPageData
	{
		// Token: 0x0600C290 RID: 49808 RVA: 0x00573790 File Offset: 0x00571990
		public void Init()
		{
			bool flag = this.ConfigTable == null;
			if (flag)
			{
				this.ConfigTable = (LuaGame.Instance.DoString(this.ConfigDataAsset.text) as Table);
				this.SimpleTipsList = new List<ValueTuple<string, string>>();
				Table simpleConfigTable = this.ConfigTable.Get("Simple");
				for (int i = 1; i <= simpleConfigTable.Length; i++)
				{
					Table elemTable = simpleConfigTable.Get(i);
					string title = elemTable.Get(1);
					string content = elemTable.Get(2);
					this.SimpleTipsList.Add(new ValueTuple<string, string>(title, content));
				}
				this.SingleDescTipsList = this.LuaTableToStringList(this.ConfigTable.Get("SingleDesc"));
				this.LearningAssistantTextList = this.LuaTableToStringList(this.ConfigTable.Get("LearningAssistance"));
				this.KeywordsSearchList.AddRange(this.LuaTableToStringList(this.ConfigTable.Get("Redirect")));
			}
		}

		// Token: 0x0600C291 RID: 49809 RVA: 0x00573894 File Offset: 0x00571A94
		private List<string> LuaTableToStringList(Table table)
		{
			List<string> retList = new List<string>();
			bool flag = table == null;
			List<string> result;
			if (flag)
			{
				result = retList;
			}
			else
			{
				for (int i = 1; i <= table.Length; i++)
				{
					retList.Add(table.Get(i));
				}
				result = retList;
			}
			return result;
		}

		// Token: 0x04009717 RID: 38679
		public string DisplayName;

		// Token: 0x04009718 RID: 38680
		public string Path;

		// Token: 0x04009719 RID: 38681
		public int VolIndex;

		// Token: 0x0400971A RID: 38682
		public TextAsset PageDataAsset;

		// Token: 0x0400971B RID: 38683
		public TextAsset ConfigDataAsset;

		// Token: 0x0400971C RID: 38684
		[NonSerialized]
		public Table ConfigTable;

		// Token: 0x0400971D RID: 38685
		[NonSerialized]
		public List<string> SingleDescTipsList;

		// Token: 0x0400971E RID: 38686
		[TupleElementNames(new string[]
		{
			"title",
			"content"
		})]
		[NonSerialized]
		public List<ValueTuple<string, string>> SimpleTipsList;

		// Token: 0x0400971F RID: 38687
		[NonSerialized]
		public List<string> LearningAssistantTextList;

		// Token: 0x04009720 RID: 38688
		public List<string> KeywordsSearchList;

		// Token: 0x04009721 RID: 38689
		public string ContentSearchString;
	}

	// Token: 0x02001178 RID: 4472
	[Serializable]
	public class EncyclopediaVolData
	{
		// Token: 0x04009722 RID: 38690
		public string Name;

		// Token: 0x04009723 RID: 38691
		public string DisplayName;

		// Token: 0x04009724 RID: 38692
		public int Index;

		// Token: 0x04009725 RID: 38693
		public sbyte VolType;

		// Token: 0x04009726 RID: 38694
		[NonSerialized]
		public bool FoldState;

		// Token: 0x04009727 RID: 38695
		public List<EncyclopediaStructure.EncyclopediaPageData> PageDataList;
	}
}
