using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BFB RID: 3067
	public class BuildingManageSubPageInfoBase : BuildingManageSubPage
	{
		// Token: 0x06009BF4 RID: 39924 RVA: 0x00491162 File Offset: 0x0048F362
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			this.buildingInfoItemTemplate.gameObject.SetActive(false);
		}

		// Token: 0x06009BF5 RID: 39925 RVA: 0x00491180 File Offset: 0x0048F380
		private void SetTxtInfo(TextMeshProUGUI txtMesh, string desc)
		{
			bool flag = string.IsNullOrEmpty(desc);
			if (flag)
			{
				txtMesh.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				txtMesh.text = desc.ColorReplace();
				txtMesh.transform.parent.gameObject.SetActive(true);
			}
		}

		// Token: 0x06009BF6 RID: 39926 RVA: 0x004911DC File Offset: 0x0048F3DC
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this.SetTxtInfo(this.textDesc, this.ParentView.ConfigData.Desc);
			this.SetTxtInfo(this.textFunction, this.ParentView.ConfigData.FuncDesc);
			this.RefreshLockBuildingList();
			bool isShow = !SingletonObject.getInstance<WorldMapModel>().IsAtSecretVillage() && this.ParentView.IsTaiwuVillageBuilding && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			bool flag;
			if (isShow)
			{
				List<short> expandBuildings = this.ParentView.ConfigData.ExpandBuildings;
				flag = (expandBuildings != null && expandBuildings.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool showExpand = flag;
			this.rootExpand.gameObject.SetActive(showExpand);
			bool flag2 = showExpand;
			if (flag2)
			{
				this.RefreshBuildingInfoList(this.layoutExpand, this.ParentView.ConfigData.ExpandBuildings);
			}
			bool flag3;
			if (isShow)
			{
				List<short> dependBuildings = this.ParentView.ConfigData.DependBuildings;
				flag3 = (dependBuildings != null && dependBuildings.Count > 0);
			}
			else
			{
				flag3 = false;
			}
			bool showDepend = flag3;
			this.rootDepend.gameObject.SetActive(showDepend);
			bool flag4 = showDepend;
			if (flag4)
			{
				this.RefreshBuildingInfoList(this.layoutDepend, this.ParentView.ConfigData.DependBuildings);
			}
		}

		// Token: 0x06009BF7 RID: 39927 RVA: 0x00491314 File Offset: 0x0048F514
		private void RefreshBuildingInfoList(RectTransform layout, List<short> buildingList)
		{
			for (int i = 0; i < buildingList.Count; i++)
			{
				short templateId = buildingList[i];
				BuildingInfoItem item = (i < layout.childCount) ? layout.GetChild(i).GetComponent<BuildingInfoItem>() : Object.Instantiate<BuildingInfoItem>(this.buildingInfoItemTemplate, layout);
				item.gameObject.SetActive(true);
				ValueTuple<EBuildingNotAvailableType, string> valueTuple = this.IsBuildingTemplateIdEnabled(templateId);
				EBuildingNotAvailableType notAvailableType = valueTuple.Item1;
				string notAvailableTip = valueTuple.Item2;
				item.Refresh(templateId, notAvailableType, notAvailableTip, new Action<short>(this.OnClickBuildingInfoView));
			}
			for (int j = buildingList.Count; j < layout.childCount; j++)
			{
				layout.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x06009BF8 RID: 39928 RVA: 0x004913D8 File Offset: 0x0048F5D8
		private ValueTuple<EBuildingNotAvailableType, string> IsBuildingTemplateIdEnabled(short templateId)
		{
			BuildingBlockItem config = BuildingBlock.Instance[templateId];
			EBuildingNotAvailableType buildingBotAvailableType = EBuildingNotAvailableType.None;
			LanguageKey resultLanguageKey = LanguageKey.LK_BuildingInfoView_Tip;
			ViewBuildingArea buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
			bool flag = config.IsUnique && buildingArea.ContainsBuilding(config.TemplateId, false);
			if (flag)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
			}
			Dictionary<short, bool> dependBuildingDict;
			bool flag2 = !buildingArea.CanBuildAnywhere(config, out dependBuildingDict);
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
			bool flag7 = ViewBuildingOverview.SpaceMeet(config, this.DisplayData.BuildingSpaceLimit - this.DisplayData.BuildingSpaceCurr);
			if (flag7)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
			}
			bool flag8 = config.BuildingCoreItem != -1;
			if (flag8)
			{
				bool hasBuildingCore = GameData.Domains.Building.SharedMethods.HasBuildingCore(config, (this.DisplayData.CannotUseInventoryBuildingCore != null) ? this.DisplayData.CanUseBuildingCore.Concat(this.DisplayData.CannotUseInventoryBuildingCore).ToList<ItemDisplayData>() : this.DisplayData.CanUseBuildingCore).Item1;
				bool flag9 = !hasBuildingCore;
				if (flag9)
				{
					buildingBotAvailableType = EBuildingNotAvailableType.BuildConditionNotMet;
				}
			}
			bool isUnlock = this.CanUnlockBuildingByLifeSkill(config) && CommonUtils.CanUnlockBuildingByMainProgress(config);
			bool flag10 = !isUnlock;
			if (flag10)
			{
				buildingBotAvailableType = EBuildingNotAvailableType.Locked;
			}
			return new ValueTuple<EBuildingNotAvailableType, string>(buildingBotAvailableType, LocalStringManager.Get(resultLanguageKey));
		}

		// Token: 0x06009BF9 RID: 39929 RVA: 0x004915C8 File Offset: 0x0048F7C8
		private bool CanUnlockBuildingByLifeSkill(BuildingBlockItem blockItem)
		{
			return !this._lockBuildingList.Contains(blockItem.TemplateId);
		}

		// Token: 0x06009BFA RID: 39930 RVA: 0x004915F0 File Offset: 0x0048F7F0
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
			bool flag = this.DisplayData.LearnedLifeSkillItems != null;
			if (flag)
			{
				foreach (GameData.Domains.Character.LifeSkillItem learnedLifeSkillItems in this.DisplayData.LearnedLifeSkillItems)
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

		// Token: 0x06009BFB RID: 39931 RVA: 0x0049174C File Offset: 0x0048F94C
		private void OnClickBuildingInfoView(short buildingTemplateId)
		{
			this.QuickHide();
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			argumentBox.Set("AutoSelectBuildingTemplateId", buildingTemplateId);
			UIElement.BuildingOverview.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
		}

		// Token: 0x040078CC RID: 30924
		[SerializeField]
		private TextMeshProUGUI textDesc;

		// Token: 0x040078CD RID: 30925
		[SerializeField]
		private TextMeshProUGUI textFunction;

		// Token: 0x040078CE RID: 30926
		[SerializeField]
		private BuildingInfoItem buildingInfoItemTemplate;

		// Token: 0x040078CF RID: 30927
		[SerializeField]
		private GameObject rootExpand;

		// Token: 0x040078D0 RID: 30928
		[SerializeField]
		private RectTransform layoutExpand;

		// Token: 0x040078D1 RID: 30929
		[SerializeField]
		private GameObject rootDepend;

		// Token: 0x040078D2 RID: 30930
		[SerializeField]
		private RectTransform layoutDepend;

		// Token: 0x040078D3 RID: 30931
		private readonly List<short> _lockBuildingList = new List<short>();
	}
}
