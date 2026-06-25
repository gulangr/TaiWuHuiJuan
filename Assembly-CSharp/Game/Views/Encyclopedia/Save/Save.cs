using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Views.Encyclopedia.Save
{
	// Token: 0x02000A7A RID: 2682
	public static class Save
	{
		// Token: 0x17000E73 RID: 3699
		// (get) Token: 0x060083D3 RID: 33747 RVA: 0x003D516A File Offset: 0x003D336A
		public static Save.EncyclopediaSaveData SaveData
		{
			get
			{
				return Save._encyclopediaSaveData;
			}
		}

		// Token: 0x060083D4 RID: 33748 RVA: 0x003D5174 File Offset: 0x003D3374
		public static bool InitData(Save.EncyclopediaSaveData data, bool init)
		{
			bool needSave = false;
			bool flag = data.LabelStatus == null;
			if (flag)
			{
				bool flag2 = !init;
				if (flag2)
				{
					Debug.LogError("EncyclopediaSaveData.LabelStatus is null, reset LabelStatus.");
				}
				data.LabelStatus = new Dictionary<string, byte>
				{
					{
						"",
						9
					}
				};
				needSave = true;
			}
			bool flag3 = data.FavoritesInfos == null;
			if (flag3)
			{
				bool flag4 = !init;
				if (flag4)
				{
					Debug.LogError("EncyclopediaSaveData.favoritesInfos is null, reset favoritesInfos.");
				}
				data.FavoritesInfos = new HashSet<string>();
				needSave = true;
			}
			bool flag5 = data.FoldedLevelTwoNodeSet == null || data.DroppedLevelThreeNodeSet == null;
			if (flag5)
			{
				bool flag6 = data.FoldedLevelTwoNodeSet == null;
				if (flag6)
				{
					bool flag7 = !init;
					if (flag7)
					{
						Debug.LogError("EncyclopediaSaveData.FoldedLevelTwoNodeSet is null, reset FoldedLevelTwoNodeSet.");
					}
					data.FoldedLevelTwoNodeSet = new HashSet<string>();
					needSave = true;
				}
				bool flag8 = data.DroppedLevelThreeNodeSet == null;
				if (flag8)
				{
					bool flag9 = !init;
					if (flag9)
					{
						Debug.LogError("EncyclopediaSaveData.DroppedLevelThreeNodeSet is null, reset DroppedLevelThreeNodeSet.");
					}
					data.DroppedLevelThreeNodeSet = new HashSet<string>();
					needSave = true;
				}
			}
			return needSave;
		}

		// Token: 0x060083D5 RID: 33749 RVA: 0x003D527C File Offset: 0x003D347C
		public static bool AddFavoritesInfos(string favoriteInfo)
		{
			bool result = Save._encyclopediaSaveData.FavoritesInfos.Add(favoriteInfo);
			Save.SaveInfo();
			return result;
		}

		// Token: 0x060083D6 RID: 33750 RVA: 0x003D52A8 File Offset: 0x003D34A8
		public static bool RemoveFavoritesInfos(string favoriteInfo)
		{
			bool result = Save._encyclopediaSaveData.FavoritesInfos.Remove(favoriteInfo);
			Save.SaveInfo();
			return result;
		}

		// Token: 0x060083D7 RID: 33751 RVA: 0x003D52D2 File Offset: 0x003D34D2
		public static EEncyclopediaContentLevel GetShowLevel(NodeData nodeData, bool includeTemporary = true)
		{
			return (nodeData == null) ? Save.GetGlobalShowLevel() : Save.GetShowLevel(nodeData.Key, includeTemporary);
		}

		// Token: 0x060083D8 RID: 33752 RVA: 0x003D52EC File Offset: 0x003D34EC
		internal static EEncyclopediaContentLevel GetShowLevel(string nodeKey, bool includeTemporary = true)
		{
			bool flag = Save._encyclopediaSaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			bool flag2 = Save._encyclopediaSaveData == null;
			EEncyclopediaContentLevel result;
			if (flag2)
			{
				result = (EEncyclopediaContentLevel.Low | EEncyclopediaContentLevel.Inherit);
			}
			else
			{
				NodeData node = EncyclopediaDataManager.Instance.GetNodeData(nodeKey);
				while (((node != null) ? new EEncyclopediaContentLayer?(node.ConfigItem.Layer) : null) == EEncyclopediaContentLayer.Content)
				{
					node = node.Parent;
				}
				byte level;
				EEncyclopediaContentLevel retLv = Save._encyclopediaSaveData.LabelStatus.TryGetValue(((node != null) ? node.ConfigItem.Key : null) ?? "", out level) ? ((EEncyclopediaContentLevel)level) : Save.GetGlobalShowLevel();
				if (includeTemporary)
				{
					retLv |= ((node != null) ? node.TempShowLevel : EEncyclopediaContentLevel.None);
				}
				result = retLv;
			}
			return result;
		}

		// Token: 0x060083D9 RID: 33753 RVA: 0x003D53C8 File Offset: 0x003D35C8
		public static bool IsChildOf(this NodeData nodeData, NodeData ancestor)
		{
			while (nodeData.ConfigItem.TemplateId != ancestor.ConfigItem.TemplateId && (nodeData = nodeData.Parent) != null)
			{
			}
			return nodeData != null;
		}

		// Token: 0x060083DA RID: 33754 RVA: 0x003D540C File Offset: 0x003D360C
		public static void SetShowLevel(NodeData nodeData, EEncyclopediaContentLevel encyclopediaContentLevel)
		{
			bool flag = nodeData == null;
			if (flag)
			{
				Save.SetGlobalShowLevel(encyclopediaContentLevel);
			}
			else
			{
				bool flag2 = (EEncyclopediaContentLevel.Inherit & encyclopediaContentLevel) > EEncyclopediaContentLevel.None;
				if (flag2)
				{
					Save._encyclopediaSaveData.LabelStatus.Remove(nodeData.ConfigItem.Key);
				}
				else
				{
					Save._encyclopediaSaveData.LabelStatus[nodeData.ConfigItem.Key] = (byte)encyclopediaContentLevel;
				}
				Save.SaveInfo();
			}
		}

		// Token: 0x060083DB RID: 33755 RVA: 0x003D5478 File Offset: 0x003D3678
		public static EEncyclopediaContentLevel GetGlobalShowLevel()
		{
			bool flag = Save._encyclopediaSaveData == null;
			if (flag)
			{
				Save.LoadInfo();
			}
			return (EEncyclopediaContentLevel)Save._encyclopediaSaveData.GlobalLabelStatus;
		}

		// Token: 0x060083DC RID: 33756 RVA: 0x003D54A6 File Offset: 0x003D36A6
		public static void SetGlobalShowLevel(EEncyclopediaContentLevel encyclopediaContentLevel)
		{
			Save._encyclopediaSaveData.GlobalLabelStatus = (8 | (byte)encyclopediaContentLevel);
		}

		// Token: 0x060083DD RID: 33757 RVA: 0x003D54B8 File Offset: 0x003D36B8
		public static void ResetShowLevel()
		{
			EncyclopediaDataManager.Instance.ResetTempShowLevel();
			Save._encyclopediaSaveData.LabelStatus.Clear();
			Save.SaveInfo();
		}

		// Token: 0x060083DE RID: 33758 RVA: 0x003D54DC File Offset: 0x003D36DC
		public static void LoadInfo()
		{
			string jsonOutput = SingletonObject.getInstance<GlobalSettings>().FavoritEncyclopediaData;
			bool flag = string.IsNullOrEmpty(jsonOutput);
			if (flag)
			{
				Save._encyclopediaSaveData = new Save.EncyclopediaSaveData();
				Save.SaveInfo();
			}
			else
			{
				Save._encyclopediaSaveData = JsonConvert.DeserializeObject<Save.EncyclopediaSaveData>(jsonOutput);
				bool flag2 = Save._encyclopediaSaveData != null;
				if (flag2)
				{
					bool flag3 = Save.InitData(Save._encyclopediaSaveData, false);
					if (flag3)
					{
						Save.SaveInfo();
					}
				}
				else
				{
					Debug.LogError("EncyclopediaSaveData is null, reset EncyclopediaSaveData.");
					Save._encyclopediaSaveData = new Save.EncyclopediaSaveData();
					Save.SaveInfo();
				}
			}
		}

		// Token: 0x060083DF RID: 33759 RVA: 0x003D5560 File Offset: 0x003D3760
		public static void SaveInfo()
		{
			string jsonOutput = JsonConvert.SerializeObject(Save._encyclopediaSaveData);
			SingletonObject.getInstance<GlobalSettings>().FavoritEncyclopediaData = jsonOutput;
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
		}

		// Token: 0x04006500 RID: 25856
		private const byte GlobalDefaultShowLevel = 9;

		// Token: 0x04006501 RID: 25857
		private static Save.EncyclopediaSaveData _encyclopediaSaveData;

		// Token: 0x04006502 RID: 25858
		private static Encoding _encode;

		// Token: 0x0200201B RID: 8219
		public class EncyclopediaSaveData
		{
			// Token: 0x17001961 RID: 6497
			// (get) Token: 0x0600F621 RID: 63009 RVA: 0x0062578F File Offset: 0x0062398F
			// (set) Token: 0x0600F622 RID: 63010 RVA: 0x006257A4 File Offset: 0x006239A4
			public byte GlobalLabelStatus
			{
				get
				{
					return this.LabelStatus.GetValueOrDefault("", 9);
				}
				set
				{
					byte val;
					bool flag = this.LabelStatus.TryGetValue(string.Empty, out val) && val == value;
					if (!flag)
					{
						this.LabelStatus[""] = value;
						Save.SaveInfo();
					}
				}
			}

			// Token: 0x0600F623 RID: 63011 RVA: 0x006257EB File Offset: 0x006239EB
			public EncyclopediaSaveData()
			{
				Save.InitData(this, true);
			}

			// Token: 0x0600F624 RID: 63012 RVA: 0x00625824 File Offset: 0x00623A24
			[JsonConstructor]
			public EncyclopediaSaveData(byte globalLabelStatus)
			{
				this.LabelStatus[""] = globalLabelStatus;
			}

			// Token: 0x0400D008 RID: 53256
			public HashSet<string> FavoritesInfos;

			// Token: 0x0400D009 RID: 53257
			public bool Inited = false;

			// Token: 0x0400D00A RID: 53258
			[JsonIgnore]
			public Dictionary<string, byte> LabelStatus = new Dictionary<string, byte>();

			// Token: 0x0400D00B RID: 53259
			public HashSet<string> FoldedLevelTwoNodeSet = new HashSet<string>();

			// Token: 0x0400D00C RID: 53260
			public HashSet<string> DroppedLevelThreeNodeSet = new HashSet<string>();
		}
	}
}
