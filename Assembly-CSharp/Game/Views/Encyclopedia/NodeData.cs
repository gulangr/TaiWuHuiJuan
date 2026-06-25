using System;
using System.Collections.Generic;
using System.Linq;
using Game.Views.Encyclopedia.Elements;
using Game.Views.Encyclopedia.Save;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A53 RID: 2643
	[Serializable]
	public class NodeData
	{
		// Token: 0x17000E4B RID: 3659
		// (get) Token: 0x0600827E RID: 33406 RVA: 0x003CD08B File Offset: 0x003CB28B
		public EEncyclopediaContentLevel RecursiveTempShowLevel
		{
			get
			{
				return (this.Id != -1) ? (Save.GetShowLevel(this.Key, true) | this.RecursiveTempShowLevelQuery) : Save.GetGlobalShowLevel();
			}
		}

		// Token: 0x17000E4C RID: 3660
		// (get) Token: 0x0600827F RID: 33407 RVA: 0x003CD0B0 File Offset: 0x003CB2B0
		internal EEncyclopediaContentLevel RecursiveTempShowLevelQuery
		{
			get
			{
				return (this.Layer == EEncyclopediaContentLayer.Content) ? this.Parent.RecursiveTempShowLevelQuery : (this.TempShowLevel | this.TempTipLevel);
			}
		}

		// Token: 0x17000E4D RID: 3661
		// (get) Token: 0x06008280 RID: 33408 RVA: 0x003CD0D4 File Offset: 0x003CB2D4
		public EncyclopediaContentItem ConfigItem
		{
			get
			{
				return EncyclopediaContent.Instance[this.TemplateId];
			}
		}

		// Token: 0x17000E4E RID: 3662
		// (get) Token: 0x06008281 RID: 33409 RVA: 0x003CD0E6 File Offset: 0x003CB2E6
		public string Content
		{
			get
			{
				return this.ConfigItem.Content;
			}
		}

		// Token: 0x17000E4F RID: 3663
		// (get) Token: 0x06008282 RID: 33410 RVA: 0x003CD0F3 File Offset: 0x003CB2F3
		public string Key
		{
			get
			{
				return this.ConfigItem.Key;
			}
		}

		// Token: 0x17000E50 RID: 3664
		// (get) Token: 0x06008283 RID: 33411 RVA: 0x003CD100 File Offset: 0x003CB300
		public EEncyclopediaContentLevel Level
		{
			get
			{
				return this.ConfigItem.Level;
			}
		}

		// Token: 0x17000E51 RID: 3665
		// (get) Token: 0x06008284 RID: 33412 RVA: 0x003CD10D File Offset: 0x003CB30D
		public EEncyclopediaContentLayer Layer
		{
			get
			{
				return this.ConfigItem.Layer;
			}
		}

		// Token: 0x17000E52 RID: 3666
		// (get) Token: 0x06008285 RID: 33413 RVA: 0x003CD11A File Offset: 0x003CB31A
		public NodeData Parent
		{
			get
			{
				return EncyclopediaDataManager.Instance.GetNodeData(this.ParentId);
			}
		}

		// Token: 0x17000E53 RID: 3667
		// (get) Token: 0x06008286 RID: 33414 RVA: 0x003CD12C File Offset: 0x003CB32C
		public NodeData LevelOneRoot
		{
			get
			{
				NodeLayerType nodeLayerType = this.NodeLayerType;
				if (!true)
				{
				}
				NodeData result;
				switch (nodeLayerType)
				{
				case NodeLayerType.One:
					result = this;
					break;
				case NodeLayerType.Two:
					result = this.Parent;
					break;
				case NodeLayerType.Three:
					result = this.Parent.Parent;
					break;
				case NodeLayerType.Four:
					result = this.Parent.Parent.Parent;
					break;
				case NodeLayerType.Content:
					result = this.Parent.LevelOneRoot;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x06008287 RID: 33415 RVA: 0x003CD1A7 File Offset: 0x003CB3A7
		public void AddChild(NodeData child)
		{
			if (this.Children == null)
			{
				this.Children = new List<int>();
			}
			this.Children.Add(child.Id);
			child.ParentId = this.Id;
		}

		// Token: 0x06008288 RID: 33416 RVA: 0x003CD1DC File Offset: 0x003CB3DC
		public bool IsSearchOverLevel(bool includeChildren = false, bool includeContent = false)
		{
			bool result;
			if (this.ConfigItem.Level <= (Save.GetGlobalShowLevel() & EEncyclopediaContentLevel.LowMidHigh))
			{
				if (includeContent | includeChildren)
				{
					List<int> children = this.Children;
					if (children != null && children.Count > 0)
					{
						result = (from childId in this.Children
						select EncyclopediaDataManager.Instance.GetNodeData(childId) into childNode
						where !includeContent || childNode.NodeLayerType == NodeLayerType.Content
						select childNode).Any((NodeData childNode) => childNode.IsSearchOverLevel(includeChildren, includeContent));
						goto IL_99;
					}
				}
				result = false;
				IL_99:;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06008289 RID: 33417 RVA: 0x003CD285 File Offset: 0x003CB485
		public IEnumerable<SearchIndex> Search(string value, bool onlyTitle, bool includeChildren = false, bool includeContent = false)
		{
			bool flag;
			if (includeContent | includeChildren)
			{
				List<int> children = this.Children;
				flag = (children != null && children.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				IEnumerable<NodeData> source = from childId in this.Children
				select EncyclopediaDataManager.Instance.GetNodeData(childId);
				Func<NodeData, bool> <>9__1;
				Func<NodeData, bool> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((NodeData childNode) => !includeContent || childNode.NodeLayerType == NodeLayerType.Content));
				}
				IEnumerable<NodeData> source2 = source.Where(predicate);
				Func<NodeData, IEnumerable<SearchIndex>> <>9__2;
				Func<NodeData, IEnumerable<SearchIndex>> selector;
				if ((selector = <>9__2) == null)
				{
					selector = (<>9__2 = ((NodeData childNode) => childNode.Search(value, onlyTitle, includeChildren, includeContent)));
				}
				foreach (SearchIndex index in source2.SelectMany(selector))
				{
					yield return index;
					index = null;
				}
				IEnumerator<SearchIndex> enumerator = null;
				yield break;
			}
			bool flag3 = this.NodeLayerType == NodeLayerType.Two;
			if (flag3)
			{
				yield break;
			}
			int resultCount = 0;
			bool flag4 = this.NodeLayerType != NodeLayerType.Content;
			if (flag4)
			{
				bool flag5 = !this.Title.IsNullOrEmpty() && this.Title.NormalTextContains(value, out resultCount);
				if (flag5)
				{
					int num;
					for (int i = 0; i < resultCount; i = num + 1)
					{
						yield return new SearchIndex(i);
						num = i;
					}
				}
			}
			bool flag6 = onlyTitle || this.NodeLayerType == NodeLayerType.One;
			if (flag6)
			{
				yield break;
			}
			bool flag7 = !this.Content.IsNullOrEmpty() && this.Content.NormalTextContains(value, out resultCount);
			if (flag7)
			{
				int num;
				for (int j = 0; j < resultCount; j = num + 1)
				{
					yield return new SearchIndex(j);
					num = j;
				}
			}
			bool flag8 = this.ConfigItem.Inserts.Length == 0;
			if (flag8)
			{
				yield break;
			}
			foreach (int refTemplateId in this.ConfigItem.Inserts)
			{
				EncyclopediaReferenceItem refConfig = EncyclopediaReference.Instance[refTemplateId];
				bool flag9 = refConfig.InsertType == EEncyclopediaReferenceInsertType.ConfigTable;
				if (flag9)
				{
					foreach (SearchIndex result in this.SearchTable(refConfig, value, -1))
					{
						yield return result;
						result = null;
					}
					IEnumerator<SearchIndex> enumerator2 = null;
				}
				else
				{
					bool flag10 = refConfig.InsertType == EEncyclopediaReferenceInsertType.TableCollection;
					if (flag10)
					{
						bool flag11 = refConfig.Title.NormalTextContains(value, out resultCount);
						int num;
						if (flag11)
						{
							for (int k = 0; k < resultCount; k = num + 1)
							{
								yield return new SearchIndex(k, -1, -1);
								num = k;
							}
						}
						for (int tableIndex = 0; tableIndex < refConfig.Desc.Length; tableIndex = num + 1)
						{
							bool flag12 = refConfig.Desc[tableIndex].NormalTextContains(value, out resultCount);
							if (flag12)
							{
								for (int l = 0; l < resultCount; l = num + 1)
								{
									yield return new SearchIndex(l, tableIndex, -1);
									num = l;
								}
							}
							foreach (SearchIndex result2 in this.SearchTable(EncyclopediaReference.Instance[refConfig.Params[tableIndex]], value, tableIndex))
							{
								yield return result2;
								result2 = null;
							}
							IEnumerator<SearchIndex> enumerator3 = null;
							num = tableIndex;
						}
					}
				}
				refConfig = null;
			}
			int[] array = null;
			yield break;
			yield break;
		}

		// Token: 0x0600828A RID: 33418 RVA: 0x003CD2B2 File Offset: 0x003CB4B2
		public IEnumerable<SearchIndex> SearchTable(EncyclopediaReferenceItem refConfig, string value, int index = -1)
		{
			string[] titleAndNote = refConfig.Title.Split('\n', 2, StringSplitOptions.None);
			int resultCount;
			bool flag = index == -1 && titleAndNote[0].NormalTextContains(value, out resultCount);
			if (flag)
			{
				int num;
				for (int i = 0; i < resultCount; i = num + 1)
				{
					yield return new SearchIndex(i, -1, index);
					num = i;
				}
			}
			int cellIndex = 0;
			foreach (string title in refConfig.Desc)
			{
				bool flag2 = title.NormalTextContains(value, out resultCount);
				int num;
				if (flag2)
				{
					for (int j = 0; j < resultCount; j = num + 1)
					{
						yield return new SearchIndex(j, cellIndex, index);
						num = j;
					}
				}
				num = cellIndex;
				cellIndex = num + 1;
				title = null;
			}
			string[] array = null;
			string tableName = refConfig.Param;
			string[][] table = EncyclopediaDataProcessor.GetTable(tableName);
			int colCount = TableElement.CalculateTableColCount(refConfig, table);
			string[] array2;
			int row = TableElement.ParsingCfgHeader(refConfig, out array2) ? 1 : 0;
			foreach (string[] tableRow in table)
			{
				int num;
				for (int col = 0; col < Math.Min(colCount, tableRow.Length); col = num + 1)
				{
					bool flag3 = tableRow[col].NormalTextContains(value, out resultCount);
					if (flag3)
					{
						cellIndex = col + row * colCount;
						for (int k = 0; k < resultCount; k = num + 1)
						{
							yield return new SearchIndex(k, cellIndex, index);
							num = k;
						}
					}
					num = col;
				}
				num = row;
				row = num + 1;
				tableRow = null;
			}
			string[][] array3 = null;
			bool flag4 = titleAndNote.CheckIndex(1) && titleAndNote[1].NormalTextContains(value, out resultCount);
			if (flag4)
			{
				int num;
				for (int l = 0; l < resultCount; l = num + 1)
				{
					yield return new SearchIndex(l, -2, index);
					num = l;
				}
			}
			yield break;
		}

		// Token: 0x040063D9 RID: 25561
		public int Id;

		// Token: 0x040063DA RID: 25562
		public int ParentId;

		// Token: 0x040063DB RID: 25563
		public NodeContentType NodeContentType = NodeContentType.Text;

		// Token: 0x040063DC RID: 25564
		public NodeLayerType NodeLayerType;

		// Token: 0x040063DD RID: 25565
		public List<int> Children;

		// Token: 0x040063DE RID: 25566
		public int TemplateId;

		// Token: 0x040063DF RID: 25567
		public string Title;

		// Token: 0x040063E0 RID: 25568
		public bool IsCollapse = false;

		// Token: 0x040063E1 RID: 25569
		public EEncyclopediaContentLevel TempTipLevel;

		// Token: 0x040063E2 RID: 25570
		public EEncyclopediaContentLevel TempShowLevel;

		// Token: 0x040063E3 RID: 25571
		public bool DefaultDisplayChild = false;
	}
}
