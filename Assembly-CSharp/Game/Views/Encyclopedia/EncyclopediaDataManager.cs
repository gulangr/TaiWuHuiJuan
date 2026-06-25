using System;
using System.Collections.Generic;
using System.Linq;
using Game.Views.Encyclopedia.Save;
using Game.Views.Encyclopedia.SyntaxTree;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A54 RID: 2644
	public class EncyclopediaDataManager
	{
		// Token: 0x17000E54 RID: 3668
		// (get) Token: 0x0600828C RID: 33420 RVA: 0x003CD2F5 File Offset: 0x003CB4F5
		public OrderedHashSet<int> PinnedNodeList
		{
			get
			{
				return this._pinnedNodeList;
			}
		}

		// Token: 0x17000E55 RID: 3669
		// (get) Token: 0x0600828D RID: 33421 RVA: 0x003CD300 File Offset: 0x003CB500
		public static EncyclopediaDataManager Instance
		{
			get
			{
				bool flag = EncyclopediaDataManager.instance == null;
				if (flag)
				{
					EncyclopediaDataManager.instance = new EncyclopediaDataManager();
				}
				return EncyclopediaDataManager.instance;
			}
		}

		// Token: 0x0600828E RID: 33422 RVA: 0x003CD32F File Offset: 0x003CB52F
		public void ReInitialize()
		{
			this.Init();
		}

		// Token: 0x0600828F RID: 33423 RVA: 0x003CD33C File Offset: 0x003CB53C
		private void Init()
		{
			EncyclopediaDataProcessor.Init();
			this.encyclopediaData = new EncyclopediaData();
			int id = 1;
			int index = 0;
			while (index < EncyclopediaContent.Instance.Count)
			{
				EncyclopediaContentItem config = EncyclopediaContent.Instance[index];
				NodeData levelOneNode = null;
				bool flag = !config.Title1.IsNullOrEmpty();
				if (!flag)
				{
					goto IL_DE;
				}
				levelOneNode = this.encyclopediaData.NodeKeyDict.GetValueOrDefault(config.ParentKey1);
				bool flag2 = levelOneNode == null;
				if (!flag2)
				{
					goto IL_DE;
				}
				levelOneNode = new NodeData
				{
					Id = id,
					NodeLayerType = NodeLayerType.One,
					NodeContentType = NodeContentType.Title,
					Title = config.Title1,
					TemplateId = config.TemplateId
				};
				this.encyclopediaData.NodeIdDict[id++] = levelOneNode;
				this.encyclopediaData.NodeKeyDict[config.Key] = levelOneNode;
				this.encyclopediaData.NodeList.Add(levelOneNode);
				IL_46E:
				index++;
				continue;
				IL_DE:
				NodeData levelTwoNode = null;
				bool flag3 = levelOneNode != null && !config.Title2.IsNullOrEmpty();
				if (flag3)
				{
					levelTwoNode = this.encyclopediaData.NodeKeyDict.GetValueOrDefault(config.ParentKey2);
					bool flag4 = levelTwoNode == null;
					if (flag4)
					{
						levelTwoNode = new NodeData
						{
							Id = id,
							NodeLayerType = NodeLayerType.Two,
							NodeContentType = NodeContentType.Title,
							Title = config.Title2,
							TemplateId = config.TemplateId
						};
						this.encyclopediaData.NodeIdDict[id++] = levelTwoNode;
						this.encyclopediaData.NodeKeyDict[config.Key] = levelTwoNode;
						levelOneNode.AddChild(levelTwoNode);
						this.encyclopediaData.NodeList.Add(levelTwoNode);
						goto IL_46E;
					}
				}
				NodeData levelThreeNode = null;
				bool flag5 = levelTwoNode != null && !config.Title3.IsNullOrEmpty();
				if (flag5)
				{
					levelThreeNode = this.encyclopediaData.NodeKeyDict.GetValueOrDefault(config.ParentKey3);
					bool flag6 = levelThreeNode == null;
					if (flag6)
					{
						levelThreeNode = new NodeData
						{
							Id = id,
							NodeLayerType = NodeLayerType.Three,
							NodeContentType = NodeContentType.Title,
							Title = config.Title3,
							TemplateId = config.TemplateId
						};
						this.encyclopediaData.NodeIdDict[id++] = levelThreeNode;
						this.encyclopediaData.NodeKeyDict[config.Key] = levelThreeNode;
						levelTwoNode.AddChild(levelThreeNode);
						this.encyclopediaData.NodeList.Add(levelThreeNode);
						goto IL_46E;
					}
				}
				NodeData levelFourNode = null;
				bool flag7 = levelThreeNode != null && !config.Title4.IsNullOrEmpty();
				if (flag7)
				{
					levelFourNode = this.encyclopediaData.NodeKeyDict.GetValueOrDefault(config.ParentKey4);
					bool flag8 = levelFourNode == null;
					if (flag8)
					{
						levelFourNode = new NodeData
						{
							Id = id,
							NodeLayerType = NodeLayerType.Four,
							NodeContentType = NodeContentType.Text,
							Title = config.Title4,
							TemplateId = config.TemplateId
						};
						this.encyclopediaData.NodeIdDict[id++] = levelFourNode;
						this.encyclopediaData.NodeKeyDict[config.Key] = levelFourNode;
						levelThreeNode.AddChild(levelFourNode);
						this.encyclopediaData.NodeList.Add(levelFourNode);
						goto IL_46E;
					}
				}
				bool flag9 = levelThreeNode != null || levelFourNode != null;
				if (flag9)
				{
					NodeContentType nodeContentType = NodeContentType.SingleText;
					bool flag10 = config.Inserts.Length != 0;
					if (flag10)
					{
						int refTemplateId = config.Inserts.First<int>();
						EncyclopediaReferenceItem refConfig = EncyclopediaReference.Instance[refTemplateId];
						EEncyclopediaReferenceInsertType insertType = refConfig.InsertType;
						if (!true)
						{
						}
						NodeContentType nodeContentType2;
						if (insertType != EEncyclopediaReferenceInsertType.ConfigTable)
						{
							if (insertType != EEncyclopediaReferenceInsertType.Figure)
							{
								if (insertType != EEncyclopediaReferenceInsertType.TableCollection)
								{
									nodeContentType2 = NodeContentType.SingleText;
								}
								else
								{
									nodeContentType2 = NodeContentType.TableCollection;
								}
							}
							else
							{
								nodeContentType2 = NodeContentType.Image;
							}
						}
						else
						{
							nodeContentType2 = NodeContentType.Table;
						}
						if (!true)
						{
						}
						nodeContentType = nodeContentType2;
					}
					NodeData levelLeaf = new NodeData
					{
						Id = id,
						NodeLayerType = NodeLayerType.Content,
						NodeContentType = nodeContentType,
						TemplateId = config.TemplateId
					};
					this.encyclopediaData.NodeIdDict[id++] = levelLeaf;
					this.encyclopediaData.NodeKeyDict[config.Key] = levelLeaf;
					bool flag11 = levelFourNode != null;
					if (flag11)
					{
						levelFourNode.AddChild(levelLeaf);
					}
					else
					{
						bool flag12 = levelThreeNode != null;
						if (flag12)
						{
							levelThreeNode.AddChild(levelLeaf);
						}
					}
					this.encyclopediaData.NodeList.Add(levelLeaf);
				}
				goto IL_46E;
			}
		}

		// Token: 0x06008290 RID: 33424 RVA: 0x003CD7D4 File Offset: 0x003CB9D4
		public List<NodeData> GetAllLevelOneData()
		{
			return (from n in this.encyclopediaData.NodeIdDict.Values
			where n.NodeLayerType == NodeLayerType.One
			orderby n.Id
			select n).ToList<NodeData>();
		}

		// Token: 0x06008291 RID: 33425 RVA: 0x003CD848 File Offset: 0x003CBA48
		public NodeData GetNodeData(int id)
		{
			NodeData value;
			this.encyclopediaData.NodeIdDict.TryGetValue(id, out value);
			return value;
		}

		// Token: 0x06008292 RID: 33426 RVA: 0x003CD870 File Offset: 0x003CBA70
		public NodeData GetNodeData(string dataKey)
		{
			NodeData value;
			this.encyclopediaData.NodeKeyDict.TryGetValue(dataKey, out value);
			return value;
		}

		// Token: 0x06008293 RID: 33427 RVA: 0x003CD898 File Offset: 0x003CBA98
		public List<NodeData> GetHistoryDatas()
		{
			List<NodeData> result = new List<NodeData>();
			foreach (KeyValuePair<int, NodeData> item in this._history)
			{
				result.Add(item.Value);
			}
			return result;
		}

		// Token: 0x06008294 RID: 33428 RVA: 0x003CD8FC File Offset: 0x003CBAFC
		public void AddHistory(NodeData data)
		{
			this._history.Add(data.Id, data);
		}

		// Token: 0x06008295 RID: 33429 RVA: 0x003CD914 File Offset: 0x003CBB14
		public void AddTabHistory(int dataId)
		{
			NodeData targetData = this.GetNodeData(dataId);
			bool flag = targetData == null;
			if (!flag)
			{
				this._tabHistory.Add(targetData.Id, targetData);
			}
		}

		// Token: 0x06008296 RID: 33430 RVA: 0x003CD948 File Offset: 0x003CBB48
		public int GetNextTabHistory(int dataId)
		{
			bool flag2 = !this._tabHistory.ContainsKey(dataId);
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				bool flag = false;
				foreach (KeyValuePair<int, NodeData> item in this._tabHistory)
				{
					bool flag3 = flag;
					if (flag3)
					{
						return item.Key;
					}
					bool flag4 = item.Key == dataId;
					if (flag4)
					{
						flag = true;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x06008297 RID: 33431 RVA: 0x003CD9D8 File Offset: 0x003CBBD8
		public int GetPreviousTabHistory(int dataId)
		{
			bool flag = !this._tabHistory.ContainsKey(dataId);
			int result2;
			if (flag)
			{
				result2 = -1;
			}
			else
			{
				int result = -1;
				foreach (KeyValuePair<int, NodeData> item in this._tabHistory)
				{
					bool flag2 = item.Key == dataId;
					if (flag2)
					{
						return result;
					}
					result = item.Key;
				}
				result2 = -1;
			}
			return result2;
		}

		// Token: 0x06008298 RID: 33432 RVA: 0x003CDA60 File Offset: 0x003CBC60
		public int TabHistoryAmount()
		{
			return this._tabHistory.Count;
		}

		// Token: 0x06008299 RID: 33433 RVA: 0x003CDA6D File Offset: 0x003CBC6D
		public IReadOnlyList<NodeData> GetAllNodeDataList()
		{
			return this.encyclopediaData.NodeList;
		}

		// Token: 0x0600829A RID: 33434 RVA: 0x003CDA7A File Offset: 0x003CBC7A
		public IReadOnlyList<NodeData> GetAllFavorDataList()
		{
			return (from t in this.encyclopediaData.NodeList
			where Save.SaveData.FavoritesInfos.Contains(t.Key)
			select t).ToList<NodeData>();
		}

		// Token: 0x0600829B RID: 33435 RVA: 0x003CDAB0 File Offset: 0x003CBCB0
		public void ResetTempShowLevel()
		{
			EncyclopediaData encyclopediaData = this.encyclopediaData;
			bool flag = ((encyclopediaData != null) ? encyclopediaData.NodeList : null) == null;
			if (!flag)
			{
				foreach (NodeData item in this.encyclopediaData.NodeList)
				{
					item.TempShowLevel = EEncyclopediaContentLevel.None;
				}
			}
		}

		// Token: 0x040063E4 RID: 25572
		private static EncyclopediaDataManager instance;

		// Token: 0x040063E5 RID: 25573
		private EncyclopediaData encyclopediaData;

		// Token: 0x040063E6 RID: 25574
		private LRUCache<int, NodeData> _history = new LRUCache<int, NodeData>(999);

		// Token: 0x040063E7 RID: 25575
		private LRUCache<int, NodeData> _tabHistory = new LRUCache<int, NodeData>(999);

		// Token: 0x040063E8 RID: 25576
		private OrderedHashSet<int> _pinnedNodeList = new OrderedHashSet<int>();

		// Token: 0x040063E9 RID: 25577
		public readonly Dictionary<int, Vector2> DetailPageScrollPosDict = new Dictionary<int, Vector2>();

		// Token: 0x040063EA RID: 25578
		public int CurrentPinnedDataId;

		// Token: 0x040063EB RID: 25579
		public bool IsHighlightPinned;
	}
}
