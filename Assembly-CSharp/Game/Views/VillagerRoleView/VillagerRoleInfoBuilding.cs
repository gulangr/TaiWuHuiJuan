using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using Game.Views.Building;
using Game.Views.Building.BuildingManage;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x0200073A RID: 1850
	public class VillagerRoleInfoBuilding : MonoBehaviour
	{
		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x0600596D RID: 22893 RVA: 0x00297853 File Offset: 0x00295A53
		private BuildingAreaData _areaData
		{
			get
			{
				return this._displayData.AreaData;
			}
		}

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x0600596E RID: 22894 RVA: 0x00297860 File Offset: 0x00295A60
		private List<BuildingBlockData> BlockList
		{
			get
			{
				return this._displayData.BlockList;
			}
		}

		// Token: 0x0600596F RID: 22895 RVA: 0x00297870 File Offset: 0x00295A70
		public void Refresh(VillagerRoleItem roleConfig, Action onBeforeOpenBuilding)
		{
			this._onBeforeOpenBuilding = onBeforeOpenBuilding;
			this._config = roleConfig;
			Location taiwuVillageBlockKey = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
			BuildingDomainMethod.AsyncCall.GetTaiwuVillageBuildingDataForVillagerRole(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this.RefreshLockBuildingList();
				this.RefreshBuildings();
			});
		}

		// Token: 0x06005970 RID: 22896 RVA: 0x002978AA File Offset: 0x00295AAA
		private void OnDestroy()
		{
			VillagerRoleInfoBuilding.RoleToBuildingsDict.Clear();
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x002978B8 File Offset: 0x00295AB8
		private void RefreshBuildings()
		{
			VillagerRoleInfoBuilding.BuildingLists buildingLists = this.GetOrMakeBuildingIds();
			CommonUtils.PrepareEnoughChildren(this.root1, this.buildingTemplate.gameObject, buildingLists.CanMakeBuildings.Count, null);
			for (int i = 0; i < buildingLists.CanMakeBuildings.Count; i++)
			{
				short buildingId = buildingLists.CanMakeBuildings[i];
				BuildingBlockItem buildingConfig = BuildingBlock.Instance[buildingId];
				BuildingInfoItem item = this.root1.GetChild(i).GetComponent<BuildingInfoItem>();
				this.RefreshBuildingItem(item, buildingConfig);
			}
			this.buildingArea1.SetActive(buildingLists.CanMakeBuildings.Count > 0);
			CommonUtils.PrepareEnoughChildren(this.root2, this.buildingTemplate.gameObject, buildingLists.CannotMakeBuildings.Count, null);
			for (int j = 0; j < buildingLists.CannotMakeBuildings.Count; j++)
			{
				short buildingId2 = buildingLists.CannotMakeBuildings[j];
				BuildingBlockItem buildingConfig2 = BuildingBlock.Instance[buildingId2];
				BuildingInfoItem item2 = this.root2.GetChild(j).GetComponent<BuildingInfoItem>();
				this.RefreshBuildingItem(item2, buildingConfig2);
			}
			this.buildingArea2.SetActive(buildingLists.CannotMakeBuildings.Count > 0);
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x00297A0C File Offset: 0x00295C0C
		private void RefreshBuildingItem(BuildingInfoItem refers, BuildingBlockItem buildingConfig)
		{
			ValueTuple<EBuildingNotAvailableType, string> valueTuple = this.IsBuildingTemplateIdEnabled(buildingConfig.TemplateId);
			EBuildingNotAvailableType notAvailableType = valueTuple.Item1;
			string notAvailableTip = valueTuple.Item2;
			refers.Refresh(buildingConfig.TemplateId, notAvailableType, notAvailableTip, new Action<short>(this.OnClickBuildingInfoView));
		}

		// Token: 0x06005973 RID: 22899 RVA: 0x00297A50 File Offset: 0x00295C50
		private void OnClickBuildingInfoView(short buildingTemplateId)
		{
			Action onBeforeOpenBuilding = this._onBeforeOpenBuilding;
			if (onBeforeOpenBuilding != null)
			{
				onBeforeOpenBuilding();
			}
			bool isBuildingAreaActive = UIManager.Instance.IsElementActive(UIElement.BuildingArea);
			bool flag = isBuildingAreaActive;
			if (flag)
			{
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.Clear();
				argumentBox.Set("AutoSelectBuildingTemplateId", buildingTemplateId);
				argumentBox.Set("NeedClearBuildingOverviewFilter", true);
				UIElement.BuildingOverview.SetOnInitArgs(argumentBox);
				UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
			}
			else
			{
				this.OpenBuildingView(buildingTemplateId);
			}
		}

		// Token: 0x06005974 RID: 22900 RVA: 0x00297AD8 File Offset: 0x00295CD8
		public bool ContainsBuilding(short blockTemplateId, bool requireCanUse = false)
		{
			for (short blockIndex = 0; blockIndex < (short)(this._areaData.Width * this._areaData.Width); blockIndex += 1)
			{
				bool flag = this.BlockList[(int)blockIndex].TemplateId != blockTemplateId;
				if (!flag)
				{
					bool flag2 = !requireCanUse || this.BlockList[(int)blockIndex].CanUse();
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005975 RID: 22901 RVA: 0x00297B54 File Offset: 0x00295D54
		public bool CanBuildAnywhere(BuildingBlockItem configData, out Dictionary<short, bool> dependBuildingDict)
		{
			dependBuildingDict = null;
			bool flag = configData.IsUnique && this.ContainsBuilding(configData.TemplateId, false) && configData.TemplateId != 45;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = configData.DependBuildings.Count == 0;
				if (flag2)
				{
					foreach (BuildingBlockData block in this.BlockList)
					{
						bool flag3 = this.BlockCanBuild(block.BlockIndex, (int)this._areaData.Width, (int)configData.Width, configData, out dependBuildingDict);
						if (flag3)
						{
							return true;
						}
					}
					result = false;
				}
				else
				{
					List<BuildingBlockData> neighborBlockList = EasyPool.Get<List<BuildingBlockData>>();
					foreach (BuildingBlockData building in this.BlockList)
					{
						bool flag4 = configData.DependBuildings.Contains(building.TemplateId);
						if (flag4)
						{
							this.GetNeighborBlocks(building.BlockIndex, ref neighborBlockList, BuildingBlock.Instance[building.TemplateId].Width, 2, null);
							foreach (BuildingBlockData neighborBlock in neighborBlockList)
							{
								bool flag5 = neighborBlock.TemplateId != 0 || neighborBlock.RootBlockIndex >= 0;
								if (!flag5)
								{
									bool flag6 = this.BlockCanBuild(neighborBlock.BlockIndex, (int)this._areaData.Width, (int)configData.Width, configData, out dependBuildingDict);
									if (flag6)
									{
										EasyPool.Free<List<BuildingBlockData>>(neighborBlockList);
										return true;
									}
								}
							}
						}
					}
					EasyPool.Free<List<BuildingBlockData>>(neighborBlockList);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06005976 RID: 22902 RVA: 0x00297D50 File Offset: 0x00295F50
		private bool BlockCanBuild(short rootIndex, int areaWidth, int buildingWidth, BuildingBlockItem configData, out Dictionary<short, bool> dependBuildingDict)
		{
			dependBuildingDict = null;
			bool flag = this.BlockList[(int)rootIndex].TemplateId != 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int blockX = (int)rootIndex % areaWidth;
				int blockY = (int)rootIndex / areaWidth;
				int edge = areaWidth - buildingWidth + 1;
				bool flag2 = blockX == edge || blockY == edge;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool canBuild = this.NearDependBuildings(rootIndex, configData, areaWidth, out dependBuildingDict);
					bool flag3 = !canBuild;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = blockX; i < Math.Min(blockX + buildingWidth, areaWidth); i++)
						{
							for (int j = blockY; j < Math.Min(blockY + buildingWidth, areaWidth); j++)
							{
								int index = j * areaWidth + i;
								bool flag4 = index < 0 || index >= this.BlockList.Count;
								if (flag4)
								{
									return false;
								}
								bool flag5 = this.BlockList[index].TemplateId > 0;
								if (flag5)
								{
									return false;
								}
								bool flag6 = this.BlockList[index].RootBlockIndex >= 0;
								if (flag6)
								{
									return false;
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06005977 RID: 22903 RVA: 0x00297E90 File Offset: 0x00296090
		private bool NearDependBuildings(short rootIndex, BuildingBlockItem configData, int areaWidth, out Dictionary<short, bool> dependBuildingDict)
		{
			dependBuildingDict = null;
			List<short> dependBuildings = configData.DependBuildings;
			bool flag = dependBuildings.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				List<BuildingBlockData> neighborList = new List<BuildingBlockData>();
				List<int> neighborDistanceList = EasyPool.Get<List<int>>();
				dependBuildingDict = new Dictionary<short, bool>();
				foreach (short id in dependBuildings)
				{
					dependBuildingDict[id] = false;
				}
				this.GetNeighborBlocks(rootIndex, ref neighborList, configData.Width, 2, neighborDistanceList);
				this._blockDependencyDistanceDict[(int)rootIndex] = int.MaxValue;
				for (int i = 0; i < neighborList.Count; i++)
				{
					BuildingBlockData neighborBlock = neighborList[i];
					bool flag2 = neighborBlock.RootBlockIndex >= 0;
					if (flag2)
					{
						neighborBlock = this.BlockList[(int)neighborBlock.RootBlockIndex];
					}
					bool flag3 = neighborBlock.TemplateId != 0 && neighborBlock.CanUse();
					if (flag3)
					{
						int dependIndex = dependBuildings.IndexOf(neighborBlock.TemplateId);
						bool flag4 = dependIndex >= 0;
						if (flag4)
						{
							dependBuildingDict[neighborBlock.TemplateId] = true;
							BuildingBlockItem neighborConfigData = BuildingBlock.Instance[neighborBlock.TemplateId];
							int distance = this.GetDistanceInTwoBlock(rootIndex, (int)neighborBlock.BlockIndex, (int)configData.Width, areaWidth, (int)neighborConfigData.Width);
							int currentDistance = int.MaxValue;
							this._blockDependencyDistanceDict.TryGetValue((int)rootIndex, out currentDistance);
							this._blockDependencyDistanceDict[(int)rootIndex] = Mathf.Min(distance, currentDistance);
						}
					}
				}
				EasyPool.Free<List<int>>(neighborDistanceList);
				result = dependBuildingDict.Values.All((bool v) => v);
			}
			return result;
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x00298078 File Offset: 0x00296278
		private int GetDistanceInTwoBlock(short rootIndex, int neighborIndex, int blockWidth, int areaWidth, int neighborWidth)
		{
			return CommonUtils.GetDistanceInTwoBlock(rootIndex, neighborIndex, blockWidth, areaWidth, neighborWidth);
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x00298096 File Offset: 0x00296296
		public void GetNeighborBlocks(short blockIndex, ref List<BuildingBlockData> neighborBlockList, sbyte blockWidth = 1, int range = 2, List<int> neighborDistanceList = null)
		{
			CommonUtils.GetNeighborBlocks(this._areaData, this.BlockList, blockIndex, ref neighborBlockList, blockWidth, range, neighborDistanceList);
		}

		// Token: 0x0600597A RID: 22906 RVA: 0x002980B4 File Offset: 0x002962B4
		private void OpenBuildingView(short buildingTemplateId)
		{
			bool flag = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure || SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventTaiwu.InAdventure;
			if (!flag)
			{
				GEvent.OnEvent(UiEvents.HideMapBlockCharList, null);
				Location blockKey = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
				ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
				argsBox.Set("AreaId", blockKey.AreaId);
				argsBox.Set("BlockId", blockKey.BlockId);
				argsBox.Set("OpenBuildingOverview", true);
				argsBox.Set("AutoSelectBuildingTemplateId", buildingTemplateId);
				argsBox.Set("NeedClearBuildingOverviewFilter", true);
				UIElement.BuildingArea.SetOnInitArgs(argsBox);
				CommandManager.AddCommand<CommandStackUI, UIElement>(EPriority.StackUINormal, UIElement.StateBuilding);
			}
		}

		// Token: 0x0600597B RID: 22907 RVA: 0x00298174 File Offset: 0x00296374
		private ValueTuple<EBuildingNotAvailableType, string> IsBuildingTemplateIdEnabled(short templateId)
		{
			BuildingBlockItem config = BuildingBlock.Instance[templateId];
			EBuildingNotAvailableType buildingBotAvailableType = EBuildingNotAvailableType.None;
			LanguageKey resultLanguageKey = LanguageKey.LK_BuildingInfoView_Tip;
			bool flag = config.IsUnique && this.ContainsBuilding(config.TemplateId, false);
			if (flag)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
			}
			Dictionary<short, bool> dependBuildingDict;
			bool flag2 = !this.CanBuildAnywhere(config, out dependBuildingDict);
			if (flag2)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
				foreach (short id in config.DependBuildings)
				{
					bool hasBuilding;
					bool flag3 = dependBuildingDict != null && dependBuildingDict.TryGetValue(id, out hasBuilding) && hasBuilding;
					if (!flag3)
					{
						BuildingBlockItem buildingConfig = BuildingBlock.Instance[id];
						bool flag4 = buildingConfig.Type == EBuildingBlockType.Building;
						if (!flag4)
						{
							EBuildingBlockType type = buildingConfig.Type;
							bool flag5 = type == EBuildingBlockType.NormalResource || type == EBuildingBlockType.SpecialResource;
							if (flag5)
							{
							}
						}
					}
				}
			}
			bool flag6 = !CommonUtils.IsBuildingCostResourcesEnough(config);
			if (flag6)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
			}
			bool flag7 = ViewBuildingOverview.SpaceMeet(config, this._displayData.BuildingSpaceLimit - this._displayData.BuildingSpaceCurr);
			if (flag7)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
			}
			bool flag8 = config.BuildingCoreItem != -1;
			if (flag8)
			{
				List<ItemDisplayData> coreItemData = new List<ItemDisplayData>();
				bool flag9 = this._displayData.CannotUseInventoryBuildingCore != null;
				if (flag9)
				{
					coreItemData.AddRange(this._displayData.CannotUseInventoryBuildingCore);
				}
				bool flag10 = this._displayData.CanUseBuildingCore != null;
				if (flag10)
				{
					coreItemData.AddRange(this._displayData.CanUseBuildingCore);
				}
				bool hasBuildingCore = GameData.Domains.Building.SharedMethods.HasBuildingCore(config, coreItemData).Item1;
				bool flag11 = !hasBuildingCore;
				if (flag11)
				{
					buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
				}
			}
			bool isUnlock = this.CanUnlockBuildingByLifeSkill(config) && CommonUtils.CanUnlockBuildingByMainProgress(config);
			bool flag12 = !isUnlock;
			if (flag12)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.Locked;
			}
			return new ValueTuple<EBuildingNotAvailableType, string>(buildingBotAvailableType, LocalStringManager.Get(resultLanguageKey));
		}

		// Token: 0x0600597C RID: 22908 RVA: 0x00298378 File Offset: 0x00296578
		private bool CanUnlockBuildingByLifeSkill(BuildingBlockItem blockItem)
		{
			return !this._lockBuildingList.Contains(blockItem.TemplateId);
		}

		// Token: 0x0600597D RID: 22909 RVA: 0x002983A0 File Offset: 0x002965A0
		private void RefreshLockBuildingList()
		{
			this._lockBuildingList.Clear();
			List<short> lockBuildingList = new List<short>();
			for (int i = 0; i < LifeSkill.Instance.Count; i++)
			{
				Config.LifeSkillItem lifeSkillItem = LifeSkill.Instance.GetItem((short)i);
				CommonUtils.GetUnlockBuildingListFromConfig(lifeSkillItem, lockBuildingList);
				this._lockBuildingList.AddRange(lockBuildingList);
			}
			bool flag = this._displayData.LearnedLifeSkillItems != null;
			if (flag)
			{
				foreach (GameData.Domains.Character.LifeSkillItem learnedLifeSkillItems in this._displayData.LearnedLifeSkillItems)
				{
					Config.LifeSkillItem unlockItemConfig = LifeSkill.Instance[learnedLifeSkillItems.SkillTemplateId];
					List<short> idList = new List<short>();
					CommonUtils.GetUnlockBuildingListFromConfig(unlockItemConfig, idList);
					bool flag2 = learnedLifeSkillItems.IsPageRead(0);
					if (flag2)
					{
						foreach (short id in idList)
						{
							bool flag3 = this._lockBuildingList.Contains(id);
							if (flag3)
							{
								this._lockBuildingList.Remove(id);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600597E RID: 22910 RVA: 0x002984FC File Offset: 0x002966FC
		private VillagerRoleInfoBuilding.BuildingLists GetOrMakeBuildingIds()
		{
			VillagerRoleInfoBuilding.BuildingLists lists;
			bool flag = VillagerRoleInfoBuilding.RoleToBuildingsDict.TryGetValue(this._config.TemplateId, out lists);
			VillagerRoleInfoBuilding.BuildingLists result;
			if (flag)
			{
				result = lists;
			}
			else
			{
				List<short> canMakeBuildingIds = new List<short>();
				List<short> cannotMakeBuildingIds = new List<short>();
				VillagerRoleInfoBuilding.BuildingLists buildingLists = new VillagerRoleInfoBuilding.BuildingLists
				{
					CanMakeBuildings = canMakeBuildingIds,
					CannotMakeBuildings = cannotMakeBuildingIds
				};
				VillagerRoleInfoBuilding.RoleToBuildingsDict.Add(this._config.TemplateId, buildingLists);
				foreach (BuildingBlockItem buildingConfig in ((IEnumerable<BuildingBlockItem>)BuildingBlock.Instance))
				{
					short[] villagerRoleTemplateIds = buildingConfig.VillagerRoleTemplateIds;
					bool flag2 = villagerRoleTemplateIds == null || !villagerRoleTemplateIds.Contains(this._config.TemplateId);
					if (!flag2)
					{
						bool canMakeItem = buildingConfig.CanMakeItem;
						if (canMakeItem)
						{
							canMakeBuildingIds.Add(buildingConfig.TemplateId);
						}
						else
						{
							cannotMakeBuildingIds.Add(buildingConfig.TemplateId);
						}
					}
				}
				result = buildingLists;
			}
			return result;
		}

		// Token: 0x04003D84 RID: 15748
		private VillagerRoleItem _config;

		// Token: 0x04003D85 RID: 15749
		private readonly List<short> _lockBuildingList = new List<short>();

		// Token: 0x04003D86 RID: 15750
		private TaiwuVillageBuildingDataForVillagerRole _displayData = null;

		// Token: 0x04003D87 RID: 15751
		private Action _onBeforeOpenBuilding;

		// Token: 0x04003D88 RID: 15752
		private static readonly Dictionary<short, VillagerRoleInfoBuilding.BuildingLists> RoleToBuildingsDict = new Dictionary<short, VillagerRoleInfoBuilding.BuildingLists>();

		// Token: 0x04003D89 RID: 15753
		private readonly Dictionary<int, int> _blockDependencyDistanceDict = new Dictionary<int, int>();

		// Token: 0x04003D8A RID: 15754
		[SerializeField]
		private RectTransform root1;

		// Token: 0x04003D8B RID: 15755
		[SerializeField]
		private RectTransform root2;

		// Token: 0x04003D8C RID: 15756
		[SerializeField]
		private VillagerRoleBuildingInfoItem buildingTemplate;

		// Token: 0x04003D8D RID: 15757
		[SerializeField]
		private GameObject buildingArea1;

		// Token: 0x04003D8E RID: 15758
		[SerializeField]
		private GameObject buildingArea2;

		// Token: 0x02001C0C RID: 7180
		private struct BuildingLists
		{
			// Token: 0x0400BF65 RID: 48997
			public List<short> CanMakeBuildings;

			// Token: 0x0400BF66 RID: 48998
			public List<short> CannotMakeBuildings;
		}
	}
}
