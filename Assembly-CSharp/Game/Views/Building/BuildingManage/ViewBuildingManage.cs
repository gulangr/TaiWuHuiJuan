using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.Common;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Building;
using Game.Components.Common;
using Game.Views.Building.BuildingAreaQuickActionMenu;
using Game.Views.VillagerRoleView;
using GameData.Combat.Math;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C11 RID: 3089
	public class ViewBuildingManage : UIBase
	{
		// Token: 0x17001090 RID: 4240
		// (get) Token: 0x06009CC5 RID: 40133 RVA: 0x00496BA4 File Offset: 0x00494DA4
		private bool IsBuildingManagementUnlocked
		{
			get
			{
				return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			}
		}

		// Token: 0x17001091 RID: 4241
		// (get) Token: 0x06009CC6 RID: 40134 RVA: 0x00496BB2 File Offset: 0x00494DB2
		public BuildingBlockKey BlockKey
		{
			get
			{
				return this._blockKey;
			}
		}

		// Token: 0x17001092 RID: 4242
		// (get) Token: 0x06009CC7 RID: 40135 RVA: 0x00496BBA File Offset: 0x00494DBA
		public BuildingBlockData BlockData
		{
			get
			{
				return this._blockData;
			}
		}

		// Token: 0x17001093 RID: 4243
		// (get) Token: 0x06009CC8 RID: 40136 RVA: 0x00496BC2 File Offset: 0x00494DC2
		public BuildingBlockItem ConfigData
		{
			get
			{
				return this._configData;
			}
		}

		// Token: 0x17001094 RID: 4244
		// (get) Token: 0x06009CC9 RID: 40137 RVA: 0x00496BCA File Offset: 0x00494DCA
		public int TaiwuCharId
		{
			get
			{
				return this._taiwuCharId;
			}
		}

		// Token: 0x17001095 RID: 4245
		// (get) Token: 0x06009CCA RID: 40138 RVA: 0x00496BD2 File Offset: 0x00494DD2
		public MapBlockData MapBlockData
		{
			get
			{
				return this._mapBlockData;
			}
		}

		// Token: 0x17001096 RID: 4246
		// (get) Token: 0x06009CCB RID: 40139 RVA: 0x00496BDA File Offset: 0x00494DDA
		public BuildingModel BuildingModel
		{
			get
			{
				return this._buildingModel;
			}
		}

		// Token: 0x17001097 RID: 4247
		// (get) Token: 0x06009CCC RID: 40140 RVA: 0x00496BE2 File Offset: 0x00494DE2
		public bool IsTaiwuVillageBuilding
		{
			get
			{
				return this._isTaiwuVillageBuilding;
			}
		}

		// Token: 0x17001098 RID: 4248
		// (get) Token: 0x06009CCD RID: 40141 RVA: 0x00496BEA File Offset: 0x00494DEA
		private int ResourceBlockRanking
		{
			get
			{
				return this._displayData.ResourceBlockRanking;
			}
		}

		// Token: 0x17001099 RID: 4249
		// (get) Token: 0x06009CCE RID: 40142 RVA: 0x00496BF7 File Offset: 0x00494DF7
		private BuildingFormulaContextBridge FormulaContext
		{
			get
			{
				return this._displayData.BuildingFormulaContextBridge;
			}
		}

		// Token: 0x1700109A RID: 4250
		// (get) Token: 0x06009CCF RID: 40143 RVA: 0x00496C04 File Offset: 0x00494E04
		private bool CanTransfer
		{
			get
			{
				return this._displayData.CanTransferItemToWarehouse;
			}
		}

		// Token: 0x06009CD0 RID: 40144 RVA: 0x00496C14 File Offset: 0x00494E14
		public override void OnInit(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.buildingListPanel.Init(this._mapBlockData, this._areaData, this._isTaiwuVillageBuilding, this);
			for (BuildingManageTogKey togKey = BuildingManageTogKey.Info; togKey < BuildingManageTogKey.Count; togKey++)
			{
				BuildingManageSubPage subPage = this.subPages[togKey.ToInt()];
				subPage.Init(this);
				subPage.gameObject.SetActive(false);
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x06009CD1 RID: 40145 RVA: 0x00496CB4 File Offset: 0x00494EB4
		private void Init(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				MapBlockData mapBlockData;
				bool flag2 = argsBox.Get<MapBlockData>("MapBlockData", out mapBlockData);
				if (flag2)
				{
					this._mapBlockData = mapBlockData;
				}
				BuildingBlockData blockData;
				bool flag3 = argsBox.Get<BuildingBlockData>("BuildingBlockData", out blockData);
				if (flag3)
				{
					bool isSameTemplate = this._buildingTemplateId == blockData.TemplateId;
					this._blockData = blockData;
					this._buildingTemplateId = this._blockData.TemplateId;
				}
				BuildingAreaData areaData;
				bool flag4 = argsBox.Get<BuildingAreaData>("BuildingAreaData", out areaData);
				if (flag4)
				{
					this._areaData = areaData;
				}
				Enum enumValue;
				BuildingManageTogKey tabKey = argsBox.Get("tabKey", out enumValue) ? ((BuildingManageTogKey)enumValue) : BuildingManageTogKey.Invalid;
				bool flag5 = tabKey >= BuildingManageTogKey.Info;
				if (flag5)
				{
					this.SetInitialTab(tabKey);
				}
				this._displayInited = false;
				this._buildingModel = SingletonObject.getInstance<BuildingModel>();
				this._blockKey = new BuildingBlockKey(this._mapBlockData.AreaId, this._mapBlockData.BlockId, this._blockData.BlockIndex);
				this._configData = BuildingBlock.Instance[this._blockData.TemplateId];
				this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				Location villageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
				this._isTaiwuVillageBuilding = (this._mapBlockData.GetLocation() == villageLocation);
				this.textTitle.text = this._configData.Name;
				this.btnTemplate.gameObject.SetActive(false);
				this.ResetData();
				this.HideCustomNameInput();
				this.ResetSensitiveWordTip();
				this.PlayOpenSound(this._blockData.TemplateId);
			}
		}

		// Token: 0x06009CD2 RID: 40146 RVA: 0x00496E52 File Offset: 0x00495052
		private void Awake()
		{
			this.mainToggleGroup.OnActiveIndexChange += this.OnToggleChange;
		}

		// Token: 0x06009CD3 RID: 40147 RVA: 0x00496E6D File Offset: 0x0049506D
		private void OnDestroy()
		{
			this.mainToggleGroup.OnActiveIndexChange -= this.OnToggleChange;
		}

		// Token: 0x06009CD4 RID: 40148 RVA: 0x00496E88 File Offset: 0x00495088
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingCustomNameChange, new GEvent.Callback(this.OnBuildingCustomNameChange));
			GEvent.Add(UiEvents.SwitchBuildingManage, new GEvent.Callback(this.OnSwitchBuildingManage));
			GEvent.Add(UiEvents.SamsaraPlatformRecordDataChange, new GEvent.Callback(this.RequestData));
			GEvent.Add(UiEvents.StartPlacingBuilding, new GEvent.Callback(this.StartPlacingBuilding));
			GEvent.Add(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.RequestData));
		}

		// Token: 0x06009CD5 RID: 40149 RVA: 0x00496F18 File Offset: 0x00495118
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingCustomNameChange, new GEvent.Callback(this.OnBuildingCustomNameChange));
			GEvent.Remove(UiEvents.SwitchBuildingManage, new GEvent.Callback(this.OnSwitchBuildingManage));
			GEvent.Remove(UiEvents.SamsaraPlatformRecordDataChange, new GEvent.Callback(this.RequestData));
			GEvent.Remove(UiEvents.StartPlacingBuilding, new GEvent.Callback(this.StartPlacingBuilding));
			GEvent.Remove(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.RequestData));
			GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
			GEvent.OnEvent(UiEvents.UpdateRoad, null);
			GEvent.OnEvent(UiEvents.RefreshExceptionInfo, null);
			GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
		}

		// Token: 0x06009CD6 RID: 40150 RVA: 0x00496FE4 File Offset: 0x004951E4
		private void StartPlacingBuilding(ArgumentBox argBox)
		{
			this.QuickHide();
		}

		// Token: 0x06009CD7 RID: 40151 RVA: 0x00496FF0 File Offset: 0x004951F0
		public override void QuickHide()
		{
			foreach (BuildingManageSubPage subPage in this.subPages)
			{
				bool flag = subPage.gameObject.activeSelf && subPage.QuickHide();
				if (flag)
				{
					return;
				}
			}
			base.QuickHide();
		}

		// Token: 0x06009CD8 RID: 40152 RVA: 0x0049703D File Offset: 0x0049523D
		public void RequestData(ArgumentBox _)
		{
			this.RequestData();
		}

		// Token: 0x06009CD9 RID: 40153 RVA: 0x00497048 File Offset: 0x00495248
		public void RequestData()
		{
			bool flag = !this._displayInited;
			if (flag)
			{
				this.CreateMainToggles();
				this.UpdateMainToggles();
			}
			List<CToggle> allToggleList = this.mainToggleGroup.GetAll();
			for (int i = 0; i < allToggleList.Count; i++)
			{
				CToggle toggle = allToggleList[i];
				bool activeSelf = toggle.gameObject.activeSelf;
				if (activeSelf)
				{
					this.subPages[i].RequestData();
				}
			}
			BuildingDomainMethod.AsyncCall.GetBuildingManageDisplayData(this, this._blockKey, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this._blockData = this._displayData.BlockData;
				BuildingDomainMethod.AsyncCall.GetBuildingExceptionData(this, delegate(int buildingExceptionOffset, RawDataPool buildingExceptionPool)
				{
					Serializer.Deserialize(buildingExceptionPool, buildingExceptionOffset, ref this._buildingExceptionData);
					this.InitDisplay();
					this.Refresh();
					this.Element.ShowAfterRefresh();
				});
			});
		}

		// Token: 0x06009CDA RID: 40154 RVA: 0x004970D8 File Offset: 0x004952D8
		private void PlayOpenSound(short templateId)
		{
			string soundName;
			ViewBuildingManage.SoundDict.TryGetValue(templateId, out soundName);
			foreach (KeyValuePair<short, string> pair in ViewBuildingManage.SoundDict)
			{
				AudioManager.Instance.StopAllSound(pair.Value);
			}
			AudioManager.Instance.PlaySound(soundName, false, false);
		}

		// Token: 0x06009CDB RID: 40155 RVA: 0x00497154 File Offset: 0x00495354
		private void InitDisplay()
		{
			bool flag = !this._displayInited;
			if (flag)
			{
				this.InitMainToggleGroup();
			}
			this._displayInited = true;
		}

		// Token: 0x06009CDC RID: 40156 RVA: 0x00497180 File Offset: 0x00495380
		private void Refresh()
		{
			this.ShowFixedBuildingInfo();
			int index = this.mainToggleGroup.GetActiveIndex();
			BuildingManageSubPage subPage = this.subPages.GetOrDefault(index);
			bool flag = subPage;
			if (flag)
			{
				subPage.gameObject.SetActive(true);
				subPage.Refresh(this._displayData);
			}
			this.buildingListPanel.Refresh(this._displayData.BlockList, this._blockData, this._buildingExceptionData);
			short templateId = this.ConfigData.TemplateId;
			if (!true)
			{
			}
			short num;
			if (templateId >= 303)
			{
				if (templateId <= 317)
				{
					num = 121;
					goto IL_130;
				}
			}
			else
			{
				switch (templateId)
				{
				case 45:
					num = 95;
					goto IL_130;
				case 46:
					num = 116;
					goto IL_130;
				case 47:
					num = 90;
					goto IL_130;
				case 49:
					num = 89;
					goto IL_130;
				}
			}
			short num2;
			if (!this.ConfigData.CanMakeItem)
			{
				num2 = (this.ConfigData.SuccesEvent.Any(delegate(short e)
				{
					ShopEventItem shopEventItem = ShopEvent.Instance[e];
					sbyte? b = (shopEventItem != null) ? new sbyte?(shopEventItem.ExchangeResourceGoods) : null;
					int? num3 = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
					int num4 = 0;
					return num3.GetValueOrDefault() >= num4 & num3 != null;
				}) ? 84 : (BuildingBlockData.IsResource(this.ConfigData.Type) ? 82 : -1));
			}
			else
			{
				num2 = 83;
			}
			num = num2;
			IL_130:
			if (!true)
			{
			}
			short triggerKey = num;
			bool flag2 = triggerKey >= 0;
			if (flag2)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(triggerKey);
			}
			bool canShowExpand = ViewBuildingManage.CanShowExpand(this._blockData);
			bool canShowUpgrade = ViewBuildingManage.CanShowUpgrade(this._blockData);
			bool flag3 = canShowExpand || canShowUpgrade;
			if (flag3)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(85);
			}
		}

		// Token: 0x06009CDD RID: 40157 RVA: 0x00497304 File Offset: 0x00495504
		private void ResetData()
		{
			foreach (CButton btn in this._btnGroup)
			{
				btn.gameObject.SetActive(false);
			}
			foreach (GameObject go in this._animationGoList)
			{
				Object.Destroy(go);
			}
			this._animationGoList.Clear();
		}

		// Token: 0x06009CDE RID: 40158 RVA: 0x004973B4 File Offset: 0x004955B4
		private void ShowFixedBuildingInfo()
		{
			bool isOne = this._blockData.ConfigData.Width == 1;
			this.buildingAreaBlockOne.gameObject.SetActive(isOne);
			this.buildingAreaBlockTwo.gameObject.SetActive(!isOne);
			CImage buildingIcon = isOne ? this.buildingAreaBlockOne.buildingIcon : this.buildingAreaBlockTwo.buildingIcon;
			ViewBuildingManage.SetBuildingIcon(buildingIcon, this._configData, false, null);
			buildingIcon.gameObject.SetActive(!ViewBuildingManage.IsShowAnimation(this._blockData.TemplateId));
			bool flag = ViewBuildingManage.IsShowAnimation(this._blockData.TemplateId);
			if (flag)
			{
				ResLoader.Load<GameObject>(ViewBuildingManage.GetBuildingAnimationPrefabPath(this._blockData.TemplateId), delegate(GameObject obj)
				{
					GameObject go = Object.Instantiate<GameObject>(obj, this.animationHolder.transform, false);
					this._animationGoList.Add(go);
				}, null, false);
			}
			this.textBuildingName.text = ViewBuildingManage.GetBuildingName(this._blockKey, this._blockData.TemplateId, this._mapBlockData.TemplateId, true, false);
			BuildingBlockItem configData = BuildingBlock.Instance[this._blockData.TemplateId];
			bool flag2 = this._blockData.TemplateId == 44;
			if (flag2)
			{
				this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.TaiwuVillage;
				this.quickEncyclopedia.gameObject.SetActive(true);
			}
			else
			{
				bool flag3 = this._blockData.TemplateId == 49;
				if (flag3)
				{
					this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.ChickenCoop;
					this.quickEncyclopedia.gameObject.SetActive(true);
				}
				else
				{
					bool flag4 = this._blockData.TemplateId == 47;
					if (flag4)
					{
						this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.BanquetHall;
						this.quickEncyclopedia.gameObject.SetActive(true);
					}
					else
					{
						bool isShop = configData.IsShop;
						if (isShop)
						{
							this.quickEncyclopedia.encyclopediaLink = EEncyclopediaTipLinkType.IndustryBuildings;
							this.quickEncyclopedia.gameObject.SetActive(true);
						}
						else
						{
							this.quickEncyclopedia.gameObject.SetActive(false);
						}
					}
				}
			}
			bool canRename = SingletonObject.getInstance<WorldMapModel>().IsAtTaiwuVillage(this._mapBlockData.AreaId, this._mapBlockData.BlockId) && BuildingBlockData.IsBuilding(this._configData.Type);
			this.buttonRename.interactable = canRename;
			TooltipInvoker component = this.buttonRename.GetComponent<TooltipInvoker>();
			ArgumentBox argumentBox;
			if ((argumentBox = component.RuntimeParam) == null)
			{
				argumentBox = (component.RuntimeParam = new ArgumentBox());
			}
			ArgumentBox runtimeParam = argumentBox;
			runtimeParam.Set("arg0", LocalStringManager.Get(canRename ? LanguageKey.LK_Building_Rename : LanguageKey.LK_Building_CannotRename));
			this.buildingWidth.SetSprite(CommonUtils.GetBuildingWidthIcon(this._configData.Width), false, null);
			this.UpdateLevelInfo();
			this.UpdateDamageInfo();
			this.UpdateMaintainInfo();
			this.UpdateRepairButton();
			this.UpdateEffectInfo();
			this.ShowEntranceButtons();
		}

		// Token: 0x06009CDF RID: 40159 RVA: 0x00497690 File Offset: 0x00495890
		private void UpdateLevelInfo()
		{
			sbyte buildingLevel = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
			int displayLevel = (this._blockData.OperationType == 0) ? 0 : Mathf.Max((int)buildingLevel, 0);
			this.textBuildingLevel.text = displayLevel.ToString();
			bool showLevel = this._configData.MaxLevel > 1;
			this.propertyLevel.gameObject.SetActive(showLevel);
			bool flag = !showLevel;
			if (!flag)
			{
				string value = string.Format("{0}/{1}", displayLevel, this._configData.MaxLevel);
				string title = LanguageKey.LK_Building_Level.Tr();
				this.propertyLevel.Set(title, value, null);
				TooltipInvoker levelTips = this.propertyLevel.Tip;
				levelTips.Type = TipType.BuildingLevel;
				levelTips.RuntimeParam = EasyPool.Get<ArgumentBox>();
				string line1Text = LocalStringManager.GetFormat(LanguageKey.LK_Building_LevelTip_Content1, displayLevel, this._configData.MaxLevel);
				levelTips.RuntimeParam.Set("Line1Text", line1Text.ColorReplace()).Set("BuildingBlockTemplateId", this._configData.TemplateId).Set("BuildingLevel", buildingLevel).Set("IsTaiwuVillageBuilding", this._isTaiwuVillageBuilding).Set("ResourceBlockRanking", this.ResourceBlockRanking);
				levelTips.Refresh(false, -1);
			}
		}

		// Token: 0x06009CE0 RID: 40160 RVA: 0x00497800 File Offset: 0x00495A00
		private void UpdateDamageInfo()
		{
			int percent = (int)((this._blockData.OperationType != 0) ? ((this._configData.MaxDurability - this._blockData.Durability) * 100 / this._configData.MaxDurability) : 0);
			string color = (this._blockData.Durability < this._configData.MaxDurability) ? "brightred" : "pinkyellow";
			string value = string.Format("{0}%", percent).SetColor(color);
			string title = LanguageKey.LK_Building_Damage.Tr();
			this.propertyDamage.Set(title, value, null);
			this.propertyDamage.Tip.Type = TipType.SingleDesc;
			this.propertyDamage.Tip.PresetParam = new string[]
			{
				LanguageKey.LK_Building_DamageTip.Tr()
			};
		}

		// Token: 0x06009CE1 RID: 40161 RVA: 0x004978D8 File Offset: 0x00495AD8
		private void UpdateMaintainInfo()
		{
			bool showMaintain = this._configData.BaseMaintenanceCost.Count > 0 && this._blockData.OperationType != 0 && this._isTaiwuVillageBuilding;
			this.propertyMaintain.gameObject.SetActive(showMaintain);
			bool flag = showMaintain;
			if (flag)
			{
				string icon = ResourceType.Instance[this._configData.BaseMaintenanceCost[0].ResourceType].Icon;
				string title = LanguageKey.LK_Building_Maintain_Title.Tr();
				int[] costArray = GameData.Domains.Building.SharedMethods.GetFinalMaintenanceCost(this._configData);
				int cost = costArray[(int)this._configData.BaseMaintenanceCost[0].ResourceType];
				string text = string.Format("{0}/{1}", cost, LocalStringManager.Get(LanguageKey.LK_Month));
				this.propertyMaintain.Set(icon, title, text, null, false);
				this.propertyMaintain.Tip.Type = TipType.SingleDesc;
				this.propertyMaintain.Tip.PresetParam = new string[]
				{
					LanguageKey.LK_Building_MaintainTip2.Tr()
				};
			}
		}

		// Token: 0x06009CE2 RID: 40162 RVA: 0x004979F4 File Offset: 0x00495BF4
		private void UpdateEffectInfo()
		{
			bool flag;
			if (this._isTaiwuVillageBuilding && this._blockData.OperationType != 0)
			{
				List<short> expandInfos = this._configData.ExpandInfos;
				flag = (expandInfos != null && expandInfos.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool showEffect = flag;
			this.effectAreaRoot.gameObject.SetActive(showEffect);
			bool flag2 = !showEffect;
			if (!flag2)
			{
				List<int> managers = this._buildingModel.GetBuildingShopManager(this._blockKey);
				bool hasLeader = (managers.CheckIndex(0) && managers[0] >= 0) || !this._configData.NeedLeader;
				for (int i = 0; i < this.effectPropertyArray.Length; i++)
				{
					bool hasEffect = this._configData.ExpandInfos.CheckIndex(i);
					PropertyItem property = this.effectPropertyArray[i];
					property.gameObject.SetActive(hasEffect);
					bool flag3 = !hasEffect;
					if (!flag3)
					{
						BuildingScaleItem scale = BuildingScale.Instance[this._configData.ExpandInfos[i]];
						sbyte level = this._buildingModel.GetBuildingLevel(this._blockKey, this._blockData);
						bool flag4 = !hasLeader;
						int value;
						if (flag4)
						{
							value = 0;
						}
						else
						{
							bool flag5 = scale.Formula >= 0;
							if (flag5)
							{
								bool flag6 = this._configData.Class == EBuildingBlockClass.BornResource;
								if (flag6)
								{
									bool flag7 = this.ResourceBlockRanking < 5;
									if (flag7)
									{
										int percentage = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(this.ResourceBlockRanking);
										int baseValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue((int)level, percentage);
										value = BuildingFormula.Instance[scale.Formula].Calculate(baseValue);
									}
									else
									{
										value = 0;
									}
								}
								else
								{
									bool flag8 = this.FormulaContext != null && this.FormulaContext.BlockKey.Equals(this._blockKey);
									if (flag8)
									{
										value = BuildingFormula.Instance[scale.Formula].Calculate(this.FormulaContext);
									}
									else
									{
										value = 0;
									}
								}
							}
							else
							{
								int levelEffectIndex = (int)(level - 1);
								value = ((scale.LevelEffect != null && scale.LevelEffect.CheckIndex(levelEffectIndex)) ? scale.LevelEffect[levelEffectIndex] : 0);
							}
						}
						bool flag9 = !hasLeader && this._configData.IsShop;
						string text;
						if (flag9)
						{
							text = "-";
						}
						else
						{
							text = ViewBuildingManage.GetBuildingScaleFormatString(scale.Type, value);
							bool flag10 = scale.Type == EBuildingScaleType.Maintaince;
							if (flag10)
							{
								text = string.Format("{0}", value).SetColor("brightred");
							}
						}
						string title = scale.Name;
						property.Set(title, text, null);
						bool flag11 = scale.TemplateId == 109;
						if (flag11)
						{
							property.gameObject.SetActive(false);
							ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
							{
								bool isOpen = false;
								Serializer.Deserialize(dataPool, offset, ref isOpen);
								property.gameObject.SetActive(isOpen);
							});
						}
					}
				}
			}
		}

		// Token: 0x06009CE3 RID: 40163 RVA: 0x00497D08 File Offset: 0x00495F08
		public static string GetBuildingScaleFormatString(EBuildingScaleType type, int value)
		{
			string displayStr = value.ToString();
			bool flag = type == EBuildingScaleType.Percentage;
			if (flag)
			{
				displayStr = value.ToString() + "%";
			}
			else
			{
				bool flag2 = type == EBuildingScaleType.BonusPercentage;
				if (flag2)
				{
					displayStr = ((value < 0) ? string.Format("{0}%", value) : string.Format("+{0}%", value));
				}
				else
				{
					bool flag3 = type == EBuildingScaleType.MovePoint;
					if (flag3)
					{
						displayStr = ((float)value / 10f).ToString("0.0");
					}
					else
					{
						bool flag4 = type == EBuildingScaleType.ReducePercentage;
						if (flag4)
						{
							displayStr = "-" + value.ToString() + "%";
						}
					}
				}
			}
			return displayStr;
		}

		// Token: 0x06009CE4 RID: 40164 RVA: 0x00497DB4 File Offset: 0x00495FB4
		private void UpdateRepairButton()
		{
			CButton btn = this.buttonRepair;
			bool needRepair = (this._configData.Type == EBuildingBlockType.Building || this._configData.Type == EBuildingBlockType.MainBuilding) && this._blockData.NeedMaintenanceCost() && this._isTaiwuVillageBuilding && this._configData.MaxDurability > this._blockData.Durability;
			btn.gameObject.SetActive(needRepair);
			bool flag = !btn.gameObject.activeSelf;
			if (!flag)
			{
				TooltipInvoker tipDisplayer = this.tipButtonRepair;
				tipDisplayer.Type = TipType.Simple;
				bool flag2 = tipDisplayer.PresetParam == null || tipDisplayer.PresetParam.Length != 2;
				if (flag2)
				{
					tipDisplayer.PresetParam = new string[2];
				}
				bool flag3 = !this.IsBuildingManagementUnlocked;
				if (flag3)
				{
					btn.interactable = false;
					tipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_Unmaintainable_Title);
					tipDisplayer.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_Functionality_Locked);
					tipDisplayer.Refresh(false, -1);
				}
				else
				{
					bool flag4 = this._blockData.Durability >= this._configData.MaxDurability;
					if (flag4)
					{
						btn.interactable = false;
						tipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Building_No_Need_For_Maintenance_Title);
						tipDisplayer.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_Building_No_Need_For_Maintenance_Desc);
						tipDisplayer.Refresh(false, -1);
					}
					else
					{
						bool enough = true;
						StringBuilder builder = EasyPool.Get<StringBuilder>();
						builder.Clear();
						int[] resourceCosts = new int[8];
						resourceCosts[6] = GameData.Domains.Building.SharedMethods.CalcRepairBuildingCost(this._blockData, this._configData);
						sbyte resType = 0;
						while ((int)resType < resourceCosts.Length)
						{
							bool flag5 = resourceCosts[(int)resType] <= 0;
							if (!flag5)
							{
								int resourceOwned = this._buildingModel.GetResourceCount(resType);
								int resourceCost = resourceCosts[(int)resType];
								enough = (resourceOwned >= resourceCost);
								string color = enough ? "brightblue" : "brightred";
								string resourceName = ResourceType.Instance[resType].Name;
								string ownedStr = CommonUtils.GetDisplayStringForNum(resourceOwned, 100000).SetColor(color);
								string costStr = CommonUtils.GetDisplayStringForNum(resourceCost, 100000);
								builder.AppendFormat("{0}:{1}/{2}\n", resourceName, ownedStr, costStr);
							}
							resType += 1;
						}
						tipDisplayer.PresetParam[0] = LocalStringManager.Get(enough ? LanguageKey.LK_Building_Click_To_Maintain : LanguageKey.LK_Building_Unmaintainable_Title);
						tipDisplayer.PresetParam[1] = builder.ToString();
						tipDisplayer.Refresh(false, -1);
						EasyPool.Free<StringBuilder>(builder);
						btn.interactable = enough;
					}
				}
			}
		}

		// Token: 0x06009CE5 RID: 40165 RVA: 0x0049803C File Offset: 0x0049623C
		private void ShowEntranceButtons()
		{
			this._btnIndex = 0;
			bool flag = this._blockData.TemplateId == 44;
			if (flag)
			{
				this.CreateButton(TopButtonType.Cricket, delegate
				{
					BuildingActionUtils.ShowCricketCollection();
				});
				bool canOperateStoneRoom = SingletonObject.getInstance<BuildingModel>().CanOperateStoneRoom;
				if (canOperateStoneRoom)
				{
					this.CreateButton(TopButtonType.StoneRoom, delegate
					{
						BuildingActionUtils.ShowStoneHouse(this._blockKey);
					});
				}
				ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
				{
					bool isOpen = false;
					Serializer.Deserialize(dataPool, offset, ref isOpen);
					bool flag18 = isOpen;
					if (flag18)
					{
						this.CreateButton(TopButtonType.JiaoPool, delegate
						{
							BuildingActionUtils.ShowJiaoPool(this._areaData);
						});
					}
				});
			}
			else
			{
				bool flag2 = this._blockData.TemplateId == 52;
				if (flag2)
				{
					bool isTaiwuVillageBuilding = this._isTaiwuVillageBuilding;
					if (isTaiwuVillageBuilding)
					{
						CButton button = this.CreateButton(TopButtonType.KungfuPracticeRoom, delegate
						{
							UIManager.Instance.ShowUI(UIElement.KungfuPracticeRoomPuppet, true);
						});
						ViewBuildingManage.RefreshKungfuRoomButton(button, this._displayData);
					}
					this.CreateButton(TopButtonType.PracticeCombatSkill, delegate
					{
						ViewBuildingManage.OpenKungfuRoomButtonPracticingCombatSkill(this._blockKey, this._displayData);
					});
				}
				else
				{
					bool flag3 = this._blockData.TemplateId == 50;
					if (flag3)
					{
						this.CreateButton(TopButtonType.SamsaraPlatform, delegate
						{
							SingletonObject.getInstance<CharacterMonitorModel>().RefreshAllMonitorCharacterAliveState();
							ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("BuildingData", this._blockData).SetObject("BuildingKey", this._blockKey);
							UIElement.SamsaraPlatform.SetOnInitArgs(args);
							UIManager.Instance.ShowUI(UIElement.SamsaraPlatform, true);
						});
						OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 11, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
						{
							bool isOpen = false;
							Serializer.Deserialize(dataPool, offset, ref isOpen);
							bool flag18 = isOpen;
							if (flag18)
							{
								this.CreateButton(TopButtonType.SwapSoul, delegate
								{
									UIManager.Instance.ShowUI(UIElement.SwapSoul, true);
								});
							}
						});
						StoryDomainMethod.AsyncCall.JingangMonkSoulBtnShow(this, delegate(int offset, RawDataPool dataPool)
						{
							bool isOpen = false;
							Serializer.Deserialize(dataPool, offset, ref isOpen);
							bool flag18 = isOpen;
							if (flag18)
							{
								this.CreateButton(TopButtonType.MonkSoul, delegate
								{
									BuildingDomainMethod.Call.SectMainStoryJingangClickMonkSoulBtn();
								});
							}
						});
					}
					else
					{
						bool flag4 = this._blockData.TemplateId == 49;
						if (flag4)
						{
							this.CreateButton(TopButtonType.Trough, delegate
							{
								BuildingActionUtils.ShowWarehouse(ItemSourceType.Trough);
							});
							OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 14, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
							{
								bool isOpen = false;
								Serializer.Deserialize(dataPool, offset, ref isOpen);
								bool flag18 = isOpen;
								if (flag18)
								{
									this.CreateButton(TopButtonType.AssignChicken, delegate
									{
										ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
										argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
										argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.ChickenAssign);
										UIElement.VillagerRole.SetOnInitArgs(argbox);
										UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
									});
								}
							});
						}
						else
						{
							bool flag5 = this._blockData.TemplateId == 45;
							if (flag5)
							{
								this.CreateButton(TopButtonType.TaiwuVillageLineage, delegate
								{
									ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
									argBox.Set("AreaId", this._mapBlockData.AreaId);
									argBox.Set("BlockId", this._mapBlockData.BlockId);
									argBox.Set("BuildingBlockIndex", this._blockData.BlockIndex);
									argBox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
									argBox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleDescription);
									UIElement.VillagerRole.SetOnInitArgs(argBox);
									UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
								});
								this.CreateButton(TopButtonType.TaiwuLifeSummary, delegate
								{
									ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
									UIElement.TaiwuLifeSummary.SetOnInitArgs(argBox);
									UIManager.Instance.ShowUI(UIElement.TaiwuLifeSummary, true);
								});
							}
							else
							{
								bool flag6 = this._blockData.TemplateId == 51;
								if (flag6)
								{
									this.CreateButton(TopButtonType.TeaHouseCaravan, delegate
									{
										UIElement.TeaHorseCaravan.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BuildingBlockData", this._blockData).SetObject("BuildingBlockKey", this._blockKey));
										UIManager.Instance.ShowUI(UIElement.TeaHorseCaravan, true);
									});
								}
								else
								{
									bool flag7 = this._blockData.TemplateId == 48;
									if (flag7)
									{
										CButton button2 = this.CreateButton(TopButtonType.Warehouse, delegate
										{
											BuildingActionUtils.ShowWarehouse(ItemSourceType.Warehouse);
										});
										button2.interactable = (this._blockData.OperationType == -1);
										Behaviour component = button2.GetComponent<TooltipInvoker>();
										sbyte operationType = this._blockData.OperationType;
										component.enabled = (operationType == 0 || operationType == 1);
										bool showMoreTog = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
										bool flag8 = showMoreTog;
										if (flag8)
										{
											this.CreateButton(TopButtonType.Treasury, delegate
											{
												BuildingActionUtils.ShowWarehouse(ItemSourceType.Treasury);
											});
											this.CreateButton(TopButtonType.Stock, delegate
											{
												BuildingActionUtils.ShowWarehouse(ItemSourceType.Stock);
											});
										}
									}
									else
									{
										short templateId = this._blockData.TemplateId;
										bool flag9 = templateId >= 276 && templateId <= 282;
										if (flag9)
										{
											this.CreateButton(TopButtonType.MerchantBuilding, delegate
											{
												BuildingActionUtils.ShowMerchant(this._configData, this._mapBlockData.AreaId);
											});
										}
										else
										{
											bool flag10 = this._blockData.TemplateId == 283;
											if (flag10)
											{
												this.CreateButton(TopButtonType.WuHuZhenBao, delegate
												{
													BuildingActionUtils.ShowSpecialShop(this._configData);
												});
											}
											else
											{
												templateId = this._blockData.TemplateId;
												bool flag11 = templateId >= 284 && templateId <= 302;
												if (flag11)
												{
													this.CreateButton(TopButtonType.SettlementTreasury, delegate
													{
														BuildingActionUtils.ShowTreasuryShop(this._blockData);
													});
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool canMakeItem = this._configData.CanMakeItem;
			if (canMakeItem)
			{
				this.CreateButton(TopButtonType.Make, delegate
				{
					BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Make);
				});
			}
			bool flag12 = this._configData.AddReadingLifeSkillBookEfficiency == 6 || this._configData.AddReadingLifeSkillBookEfficiency == 7 || this._configData.AddReadingLifeSkillBookEfficiency == 10 || this._configData.AddReadingLifeSkillBookEfficiency == 11 || this._configData.AddReadingLifeSkillBookEfficiency == 8 || this._configData.AddReadingLifeSkillBookEfficiency == 9 || this._blockData.TemplateId == 257 || this._blockData.TemplateId == 258;
			if (flag12)
			{
				this.CreateButton(TopButtonType.Repair, delegate
				{
					BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Repair);
				});
			}
			bool flag13 = this._configData.AddReadingLifeSkillBookEfficiency == 9;
			if (flag13)
			{
				this.CreateButton(TopButtonType.Poison, delegate
				{
					BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Poison);
				});
				this.CreateButton(TopButtonType.RemovePoison, delegate
				{
					BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.RemovePoison);
				});
			}
			bool flag14 = this._configData.AddReadingLifeSkillBookEfficiency == 6 || this._configData.AddReadingLifeSkillBookEfficiency == 7 || this._configData.AddReadingLifeSkillBookEfficiency == 10 || this._configData.AddReadingLifeSkillBookEfficiency == 11 || this._blockData.TemplateId == 257 || this._blockData.TemplateId == 258;
			if (flag14)
			{
				this.CreateButton(TopButtonType.Refine, delegate
				{
					BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Refine);
				});
			}
			bool flag15 = this._configData.AddReadingLifeSkillBookEfficiency == 10;
			if (flag15)
			{
				this.CreateButton(TopButtonType.Weave, delegate
				{
					BuildingActionUtils.ShowMake(this._blockData, this._blockKey, UI_Make.UIMakeTab.Weave);
				});
			}
			bool flag16 = this._configData.TemplateId >= 303 && this._configData.TemplateId <= 317;
			if (flag16)
			{
				this.CreateButton(TopButtonType.Prison, delegate
				{
					BuildingActionUtils.ShowPrison();
				});
				this.CreateButton(TopButtonType.Bounty, delegate
				{
					BuildingActionUtils.ShowBounty(this._blockKey);
				});
				this.CreateButton(TopButtonType.Law, delegate
				{
					BuildingActionUtils.ShowLaw(this._blockKey);
				});
			}
			bool flag17 = this._configData.ArtisanOrderAvailable && this._isTaiwuVillageBuilding;
			if (flag17)
			{
				this.CreateButton(TopButtonType.Craftsman, delegate
				{
					BuildingActionUtils.ShowCraftMan(this._blockData, this._blockKey);
				});
			}
			bool hasActiveButton = this._btnGroup.Any((CButton b) => b.gameObject.activeSelf);
			this.buttonLayoutGroup.gameObject.SetActive(hasActiveButton);
		}

		// Token: 0x06009CE6 RID: 40166 RVA: 0x004986C0 File Offset: 0x004968C0
		private CButton CreateButton(TopButtonType buttonType, Action action)
		{
			bool flag = this._btnGroup.Count > this._btnIndex;
			CButton button;
			if (flag)
			{
				button = this._btnGroup[this._btnIndex];
			}
			else
			{
				button = Object.Instantiate<CButton>(this.btnTemplate, this.buttonLayoutGroup.transform);
				this._btnGroup.Add(button);
			}
			string buttonName = ViewBuildingQuickActionMenu.GetTabName(buttonType, this._blockData.TemplateId);
			this._btnIndex++;
			button.gameObject.SetActive(true);
			string[] spriteNames = this.GetTabSpriteNames(buttonType);
			ViewBuildingQuickActionMenu.SetButtonSprites(button, spriteNames, false);
			button.GetComponentInChildren<TextMeshProUGUI>().SetText(LocalStringManager.Get(buttonName), true);
			button.ClearAndAddListener(delegate
			{
				Action action2 = action;
				if (action2 != null)
				{
					action2();
				}
			});
			button.interactable = (this._blockData.OperationType != 0);
			TooltipInvoker mouseTip = button.GetComponentInChildren<TooltipInvoker>();
			mouseTip.enabled = (this._blockData.OperationType == 0);
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.AppendLine(buttonName);
			bool flag2 = !button.interactable;
			if (flag2)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Building_Constructing.Tr());
			}
			mouseTip.PresetParam = new string[]
			{
				stringBuilder.ToString()
			};
			EasyPool.Free<StringBuilder>(stringBuilder);
			return button;
		}

		// Token: 0x06009CE7 RID: 40167 RVA: 0x00498820 File Offset: 0x00496A20
		private string[] GetTabSpriteNames(TopButtonType type)
		{
			TopButtonType realType = ViewBuildingQuickActionMenu.GetTabSpriteType(type);
			string prefix = "ui9_btn_building_manage_" + Enum.GetName(typeof(TopButtonType), realType).ToLower();
			return new string[]
			{
				prefix + "_0",
				prefix + "_1",
				prefix + "_2",
				prefix + "_3"
			};
		}

		// Token: 0x06009CE8 RID: 40168 RVA: 0x0049889C File Offset: 0x00496A9C
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "ButtonCloseView"))
			{
				if (!(a == "ButtonRename"))
				{
					if (a == "ButtonRepair")
					{
						this.OnClickRepair();
					}
				}
				else
				{
					this.OnClickButtonRename();
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06009CE9 RID: 40169 RVA: 0x004988FC File Offset: 0x00496AFC
		private void OnClickRepair()
		{
			string buildingName = ViewBuildingManage.GetBuildingName(this._blockKey, this._blockData.TemplateId, this._mapBlockData.TemplateId, false, false);
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.Set<BuildingBlockKey>("blockKey", this._blockKey);
			args.Set<BuildingBlockData>("blockData", this._blockData);
			args.Set("buildingName", buildingName);
			args.Set("ownMoney", this._buildingModel.GetResourceCount(6));
			args.Set("costMoney", GameData.Domains.Building.SharedMethods.CalcRepairBuildingCost(this._blockData, this._configData));
			args.SetObject("onConfirm", new Action(this.RequestData));
			UIElement.BuildingRepairDialog.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.BuildingRepairDialog);
		}

		// Token: 0x06009CEA RID: 40170 RVA: 0x004989CE File Offset: 0x00496BCE
		private void OnSwitchBuildingManage(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.RequestData();
		}

		// Token: 0x06009CEB RID: 40171 RVA: 0x004989E0 File Offset: 0x00496BE0
		public void SetInitialTab(BuildingManageTogKey tabKey)
		{
			this._initialTabKey = tabKey.ToInt();
		}

		// Token: 0x06009CEC RID: 40172 RVA: 0x004989F4 File Offset: 0x00496BF4
		private void CreateMainToggles()
		{
			Transform parent = this.mainToggleGroup.transform;
			Transform template = parent.GetChild(0);
			this.mainToggleGroup.Clear();
			for (BuildingManageTogKey togKey = BuildingManageTogKey.Info; togKey < BuildingManageTogKey.Count; togKey++)
			{
				int index = togKey.ToInt();
				Transform child = (index < parent.childCount) ? parent.GetChild(index) : Object.Instantiate<Transform>(template, parent);
				CToggle tog = child.GetComponent<CToggle>();
				string togName = this.GetMainToggleName(togKey);
				tog.name = togName;
				tog.GetComponentInChildren<TextMeshProUGUI>().SetText(togName, true);
				this.mainToggleGroup.Add(tog);
			}
			this.mainToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.mainToggleGroup, 2, null);
			for (int i = BuildingManageTogKey.Count.ToInt(); i < parent.childCount; i++)
			{
				parent.GetChild(i).gameObject.SetActive(false);
			}
		}

		// Token: 0x06009CED RID: 40173 RVA: 0x00498AF8 File Offset: 0x00496CF8
		private string GetMainToggleName(BuildingManageTogKey togKey)
		{
			if (!true)
			{
			}
			string result;
			switch (togKey)
			{
			case BuildingManageTogKey.Info:
				result = LanguageKey.LK_Building_Message.Tr();
				break;
			case BuildingManageTogKey.LiveInfo:
			{
				short templateId = this._configData.TemplateId;
				if (!true)
				{
				}
				string text;
				if (templateId != 46)
				{
					if (templateId != 47)
					{
						text = LocalStringManager.Get(LanguageKey.LK_Building_Message);
					}
					else
					{
						text = LanguageKey.LK_Building_ComfortableInfo1.Tr();
					}
				}
				else
				{
					text = LanguageKey.LK_Building_ResidentInfo.Tr();
				}
				if (!true)
				{
				}
				result = text;
				break;
			}
			case BuildingManageTogKey.Shop:
				result = LanguageKey.LK_Building_Shop.Tr();
				break;
			case BuildingManageTogKey.Production:
				result = LanguageKey.LK_Building_Production.Tr();
				break;
			case BuildingManageTogKey.New:
				result = LanguageKey.LK_Building_New.Tr();
				break;
			case BuildingManageTogKey.Expand:
				result = LanguageKey.LK_Building_Upgrade.Tr();
				break;
			case BuildingManageTogKey.Upgrade:
				result = LanguageKey.LK_Building_Upgrade.Tr();
				break;
			case BuildingManageTogKey.SamsaraPlatformRecord:
				result = LanguageKey.LK_Building_SamsaraPlatformLog.Tr();
				break;
			case BuildingManageTogKey.TeaHouseCaravanAwareness:
				result = LanguageKey.LK_Building_TeaHorse_Awareness_Title.Tr();
				break;
			case BuildingManageTogKey.Entertain:
				result = LanguageKey.LK_Building_Tab_Entertain.Tr();
				break;
			case BuildingManageTogKey.Reward:
				result = LanguageKey.LK_Building_Tab_Reward.Tr();
				break;
			case BuildingManageTogKey.Chicken:
				result = LanguageKey.LK_Building_ChickenCoop.Tr();
				break;
			case BuildingManageTogKey.PluckFeather:
				result = LanguageKey.LK_ChickenFeather.Tr();
				break;
			case BuildingManageTogKey.Remove:
				result = LanguageKey.LK_Building_Demolish.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("togKey", togKey, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06009CEE RID: 40174 RVA: 0x00498C70 File Offset: 0x00496E70
		private void InitMainToggleGroup()
		{
			bool flag = this._initialTabKey >= 0;
			if (flag)
			{
				this.mainToggleGroup.Set(this._initialTabKey, true);
				this._initialTabKey = -1;
			}
			else
			{
				List<CToggle> toggles = (from t in this.mainToggleGroup.GetAll()
				where t.gameObject.activeSelf && t.interactable
				select t).ToList<CToggle>();
				CToggle toggle = (toggles.Count > 1) ? toggles[1] : toggles[0];
				int index = this.mainToggleGroup.GetAll().IndexOf(toggle);
				this.mainToggleGroup.Set(index, true);
			}
		}

		// Token: 0x06009CEF RID: 40175 RVA: 0x00498D1C File Offset: 0x00496F1C
		private void OnToggleChange(int newTog, int oldTog)
		{
			for (BuildingManageTogKey togKey = BuildingManageTogKey.Info; togKey < BuildingManageTogKey.Count; togKey++)
			{
				BuildingManageSubPage subPage = this.subPages[togKey.ToInt()];
				bool isShow = newTog == togKey.ToInt();
				subPage.gameObject.SetActive(isShow);
				bool flag = isShow;
				if (flag)
				{
					subPage.Refresh(this._displayData);
				}
				bool flag2 = togKey == BuildingManageTogKey.Expand && isShow;
				if (flag2)
				{
					subPage.gameObject.SetActive(BuildingManageSubPageExpand.CheckCanOpenExpandPage(this._buildingTemplateId));
				}
			}
		}

		// Token: 0x06009CF0 RID: 40176 RVA: 0x00498DA8 File Offset: 0x00496FA8
		private void UpdateMainToggles()
		{
			CToggle infoTog = this.mainToggleGroup.Get(BuildingManageTogKey.Info.ToInt());
			bool canShowInfo = ViewBuildingManage.CanShowInfo();
			infoTog.gameObject.SetActive(canShowInfo);
			CToggle liveInfoTog = this.mainToggleGroup.Get(BuildingManageTogKey.LiveInfo.ToInt());
			bool canShowLiveInfo = ViewBuildingManage.CanShowLiveInfo(this._blockData);
			liveInfoTog.gameObject.SetActive(canShowLiveInfo);
			CToggle shopTog = this.mainToggleGroup.Get(BuildingManageTogKey.Shop.ToInt());
			bool canShowShop = ViewBuildingManage.CanShowShop(this._blockData, this._isTaiwuVillageBuilding);
			shopTog.gameObject.SetActive(canShowShop);
			shopTog.interactable = this.IsBuildingManagementUnlocked;
			CToggle productionTog = this.mainToggleGroup.Get(BuildingManageTogKey.Production.ToInt());
			bool canShowProduction = ViewBuildingManage.CanShowProduction(this._blockData, this._isTaiwuVillageBuilding);
			productionTog.gameObject.SetActive(canShowProduction);
			productionTog.interactable = this.IsBuildingManagementUnlocked;
			CToggle newTog = this.mainToggleGroup.Get(BuildingManageTogKey.New.ToInt());
			bool canShowNew = ViewBuildingManage.CanShowNew(this._blockData);
			newTog.gameObject.SetActive(canShowNew);
			CToggle expandTog = this.mainToggleGroup.Get(BuildingManageTogKey.Expand.ToInt());
			bool canShowExpand = ViewBuildingManage.CanShowExpand(this._blockData);
			expandTog.gameObject.SetActive(canShowExpand);
			string expandToggleText = ViewBuildingManage.GetExpandToggleText(this._configData);
			ViewBuildingManage.SetMainToggleText(expandTog, expandToggleText);
			CToggle upgradeTog = this.mainToggleGroup.Get(BuildingManageTogKey.Upgrade.ToInt());
			bool canShowUpgrade = ViewBuildingManage.CanShowUpgrade(this._blockData);
			upgradeTog.gameObject.SetActive(canShowUpgrade);
			CToggle removeTog = this.mainToggleGroup.Get(BuildingManageTogKey.Remove.ToInt());
			bool canShowRemove = ViewBuildingManage.CanShowRemove(this._blockData, this._isTaiwuVillageBuilding);
			removeTog.gameObject.SetActive(canShowRemove);
			bool flag = canShowRemove;
			if (flag)
			{
				sbyte operationType = this._blockData.OperationType;
				bool removeTogInteractable = operationType == -1 || operationType == 1;
				removeTog.interactable = (this.IsBuildingManagementUnlocked && removeTogInteractable);
				TooltipInvoker removeTogMouseTip = removeTog.GetComponentInChildren<TooltipInvoker>();
				removeTogMouseTip.enabled = !removeTogInteractable;
				bool flag2 = !removeTogInteractable;
				if (flag2)
				{
					TooltipInvoker tooltipInvoker = removeTogMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					bool flag3 = this._blockData.OperationType == 0;
					if (flag3)
					{
						removeTogMouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Building_RemoveToggle_Tip1));
					}
				}
			}
			bool isEntertain = ViewBuildingManage.CanShowEntertainAndReword(this._blockData);
			CToggle entertainTog = this.mainToggleGroup.Get(BuildingManageTogKey.Entertain.ToInt());
			entertainTog.gameObject.SetActive(isEntertain);
			entertainTog.interactable = (this.IsBuildingManagementUnlocked && this._isTaiwuVillageBuilding);
			CToggle rewardTog = this.mainToggleGroup.Get(BuildingManageTogKey.Reward.ToInt());
			rewardTog.gameObject.SetActive(isEntertain);
			rewardTog.interactable = (this.IsBuildingManagementUnlocked && this._isTaiwuVillageBuilding);
			CToggle samsaraPlatformRecordTog = this.mainToggleGroup.Get(BuildingManageTogKey.SamsaraPlatformRecord.ToInt());
			samsaraPlatformRecordTog.gameObject.SetActive(ViewBuildingManage.CanShowSamsaraPlatformRecord(this._blockData));
			CToggle teaHouseCaravanAwarenessTog = this.mainToggleGroup.Get(BuildingManageTogKey.TeaHouseCaravanAwareness.ToInt());
			teaHouseCaravanAwarenessTog.gameObject.SetActive(ViewBuildingManage.CanShowTeaHorseCaravanAware(this._blockData));
			CToggle chickenTog = this.mainToggleGroup.Get(BuildingManageTogKey.Chicken.ToInt());
			chickenTog.gameObject.SetActive(ViewBuildingManage.CanShowChickenCoop(this._blockData));
			BuildingDomainMethod.AsyncCall.IsFeatherSystemUnlocked(this, delegate(int offset, RawDataPool pool)
			{
				bool unlock = false;
				Serializer.Deserialize(pool, offset, ref unlock);
				CToggle pluckFeatherTog = this.mainToggleGroup.Get(BuildingManageTogKey.PluckFeather.ToInt());
				GameObject gameObject = pluckFeatherTog.gameObject;
				bool flag4;
				if (!canShowNew)
				{
					BuildingBlockItem configData = this._configData;
					flag4 = (((configData != null) ? new short?(configData.TemplateId) : null) == 49);
				}
				else
				{
					flag4 = false;
				}
				gameObject.SetActive(flag4 && unlock);
			});
		}

		// Token: 0x06009CF1 RID: 40177 RVA: 0x00499188 File Offset: 0x00497388
		public static bool CanShowInfo()
		{
			return true;
		}

		// Token: 0x06009CF2 RID: 40178 RVA: 0x0049918C File Offset: 0x0049738C
		public static bool CanShowLiveInfo(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short templateId = blockData.TemplateId;
				result = (templateId == 47 || templateId == 46);
			}
			return result;
		}

		// Token: 0x06009CF3 RID: 40179 RVA: 0x004991C8 File Offset: 0x004973C8
		public static bool CanShowNew(BuildingBlockData blockData)
		{
			return blockData.OperationType == 0;
		}

		// Token: 0x06009CF4 RID: 40180 RVA: 0x004991D4 File Offset: 0x004973D4
		public static bool CanShowShop(BuildingBlockData blockData, bool isTaiwuVillageBuilding)
		{
			bool flag = ViewBuildingManage.CanShowNew(blockData) || !isTaiwuVillageBuilding;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
				result = configData.IsShop;
			}
			return result;
		}

		// Token: 0x06009CF5 RID: 40181 RVA: 0x00499218 File Offset: 0x00497418
		public static bool CanShowProduction(BuildingBlockData blockData, bool isTaiwuVillageBuilding)
		{
			bool flag = ViewBuildingManage.CanShowNew(blockData) || !isTaiwuVillageBuilding;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
				ShopEventItem shopEventData = ViewBuildingManage.GetShopEventConfig(configData.TemplateId);
				bool hasShopManage = ViewBuildingManage.CanShowShop(blockData, true);
				bool flag2 = !hasShopManage;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool hasResourceIncome = shopEventData != null && shopEventData.ResourceList.Count > 0;
					bool hasItemIncome = shopEventData != null && (shopEventData.ResourceGoods != -1 || shopEventData.ItemList.Count > 0 || shopEventData.ItemGradeProbList.Count > 0);
					bool hasSoldItem = shopEventData != null && shopEventData.ExchangeResourceGoods != -1;
					bool hasRecruit = shopEventData != null && shopEventData.RecruitPeopleProb.Count > 0;
					result = (hasResourceIncome || hasItemIncome || hasSoldItem || hasRecruit);
				}
			}
			return result;
		}

		// Token: 0x06009CF6 RID: 40182 RVA: 0x004992FC File Offset: 0x004974FC
		public static bool CanShowRemove(BuildingBlockData blockData, bool isTaiwuVillageBuilding)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
				result = (configData.Class != EBuildingBlockClass.Static && configData.OperationTotalProgress[1] >= 0 && isTaiwuVillageBuilding);
			}
			return result;
		}

		// Token: 0x06009CF7 RID: 40183 RVA: 0x0049934C File Offset: 0x0049754C
		public static bool CanShowExpand(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockData.TemplateId];
				bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
				bool isUselessResource = isResource && blockConfig.Type == EBuildingBlockType.UselessResource;
				result = (blockConfig.MaxLevel > 1 && !isUselessResource && blockConfig.TemplateId != 50 && blockConfig.TemplateId != 51 && blockConfig.TemplateId != 48 && blockConfig.TemplateId != 46 && blockConfig.TemplateId != 47);
			}
			return result;
		}

		// Token: 0x06009CF8 RID: 40184 RVA: 0x004993E4 File Offset: 0x004975E4
		public static bool CanShowUpgrade(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockData.TemplateId];
				short templateId = blockConfig.TemplateId;
				result = (templateId == 48 || templateId == 46 || templateId == 47);
			}
			return result;
		}

		// Token: 0x06009CF9 RID: 40185 RVA: 0x0049943C File Offset: 0x0049763C
		public static bool CanShowEntertainAndReword(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockData.TemplateId];
				result = (blockConfig.TemplateId == 47);
			}
			return result;
		}

		// Token: 0x06009CFA RID: 40186 RVA: 0x0049947C File Offset: 0x0049767C
		public static bool CanShowSamsaraPlatformRecord(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockData.TemplateId];
				result = (blockConfig.TemplateId == 50);
			}
			return result;
		}

		// Token: 0x06009CFB RID: 40187 RVA: 0x004994BC File Offset: 0x004976BC
		public static bool CanShowTeaHorseCaravanAware(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockData.TemplateId];
				result = (blockConfig.TemplateId == 51);
			}
			return result;
		}

		// Token: 0x06009CFC RID: 40188 RVA: 0x004994FC File Offset: 0x004976FC
		public static bool CanShowChickenCoop(BuildingBlockData blockData)
		{
			bool isNew = ViewBuildingManage.CanShowNew(blockData);
			bool flag = isNew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockData.TemplateId];
				result = (blockConfig.TemplateId == 49);
			}
			return result;
		}

		// Token: 0x06009CFD RID: 40189 RVA: 0x0049953C File Offset: 0x0049773C
		public static string GetExpandToggleText(BuildingBlockItem blockConfig)
		{
			return (blockConfig.Class == EBuildingBlockClass.BornResource) ? LanguageKey.LK_Building_Expand_UpgradeResourceBuilding_Tab.Tr() : LanguageKey.LK_Building_Upgrade.Tr();
		}

		// Token: 0x06009CFE RID: 40190 RVA: 0x00499574 File Offset: 0x00497774
		private static void SetMainToggleText(CToggle mainToggle, string str)
		{
			TextMeshProUGUI text = mainToggle.GetComponentInChildren<TextMeshProUGUI>();
			text.SetText(str, true);
		}

		// Token: 0x06009CFF RID: 40191 RVA: 0x00499594 File Offset: 0x00497794
		private int OperationTypeToTogKey(sbyte operationType)
		{
			if (!true)
			{
			}
			BuildingManageTogKey buildingManageTogKey;
			if (operationType != 0)
			{
				if (operationType != 1)
				{
					buildingManageTogKey = BuildingManageTogKey.Info;
				}
				else
				{
					buildingManageTogKey = BuildingManageTogKey.Remove;
				}
			}
			else
			{
				buildingManageTogKey = BuildingManageTogKey.New;
			}
			if (!true)
			{
			}
			BuildingManageTogKey togKey = buildingManageTogKey;
			return togKey.ToInt();
		}

		// Token: 0x06009D00 RID: 40192 RVA: 0x004995D4 File Offset: 0x004977D4
		public bool HasBuilding(short templateId, bool needFinishBuild = false)
		{
			bool flag = this._displayData.BlockList == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (BuildingBlockData blockData in this._displayData.BlockList)
				{
					bool flag2 = blockData.TemplateId == templateId;
					if (flag2)
					{
						bool flag3 = !needFinishBuild;
						if (flag3)
						{
							return true;
						}
						bool flag4 = blockData.OperationType == -1;
						if (flag4)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06009D01 RID: 40193 RVA: 0x00499678 File Offset: 0x00497878
		public static int GetPracticeCombatSkillCostActionPoint(int buildingLevel)
		{
			int cost = GlobalConfig.Instance.CombatSkillPracticeActionPointCost[0] - (buildingLevel - 1) * GlobalConfig.Instance.CombatSkillPracticeActionPointCost[1];
			return cost / 10 * 10;
		}

		// Token: 0x06009D02 RID: 40194 RVA: 0x004996B0 File Offset: 0x004978B0
		public static int GetBuildingNameCharCount()
		{
			return (LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? ViewBuildingManage.BuildingNameCharCountCn : ViewBuildingManage.BuildingNameCharCountEn;
		}

		// Token: 0x06009D03 RID: 40195 RVA: 0x004996D8 File Offset: 0x004978D8
		private void OnClickButtonRename()
		{
			int value;
			string oldName = this._buildingModel.CustomBuildingName.TryGetValue(this._blockKey, out value) ? SingletonObject.getInstance<BasicGameData>().CustomTexts[value] : "";
			new RenameCfg
			{
				Title = LanguageKey.LK_Building_QuickAction_Rename_Title.Tr(),
				Description = LanguageKey.LK_Building_QuickAction_Rename_Desc.TrFormat(oldName),
				EmptyDesc = LanguageKey.LK_Building_QuickAction_Rename_Empty.Tr(),
				Default = oldName,
				Submit = new Action<string>(this.Rename),
				CharCount = ViewBuildingManage.GetBuildingNameCharCount()
			}.Show();
		}

		// Token: 0x06009D04 RID: 40196 RVA: 0x00499778 File Offset: 0x00497978
		public void HideCustomNameInput()
		{
			bool activeSelf = this.customNameInput.gameObject.activeSelf;
			if (activeSelf)
			{
				this.customNameInput.gameObject.SetActive(false);
				this.buttonRename.gameObject.SetActive(true);
				this.textBuildingName.gameObject.SetActive(true);
			}
		}

		// Token: 0x06009D05 RID: 40197 RVA: 0x004997D4 File Offset: 0x004979D4
		private void ResetSensitiveWordTip()
		{
			bool flag = this._sensitiveWordTipCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._sensitiveWordTipCoroutine);
			}
			this.commonWarningCanvasGroup.alpha = 0f;
		}

		// Token: 0x06009D06 RID: 40198 RVA: 0x0049980D File Offset: 0x00497A0D
		public void Rename()
		{
			this.Rename(this.customNameInput.text);
		}

		// Token: 0x06009D07 RID: 40199 RVA: 0x00499821 File Offset: 0x00497A21
		public void Rename(string newName)
		{
			BuildingDomainMethod.Call.SetBuildingCustomName(this._blockKey, newName);
			this.HideCustomNameInput();
		}

		// Token: 0x06009D08 RID: 40200 RVA: 0x00499838 File Offset: 0x00497A38
		private void OnBuildingCustomNameChange(ArgumentBox argBox)
		{
			BuildingBlockKey blockKey;
			argBox.Get<BuildingBlockKey>("BuildingBlockKey", out blockKey);
			bool flag = this._blockKey.Equals(blockKey);
			if (flag)
			{
				this.textBuildingName.text = ViewBuildingManage.GetBuildingName(this._blockKey, this._blockData.TemplateId, this._mapBlockData.TemplateId, true, false);
			}
		}

		// Token: 0x06009D09 RID: 40201 RVA: 0x00499894 File Offset: 0x00497A94
		public static void SetBuildingIcon(CImage image, BuildingBlockItem configData, bool setNativeSize = false, Action onSet = null)
		{
			string path = "Building/" + configData.Icon;
			ResLoader.LoadModOrGameResource<Sprite>(path, delegate(Sprite sprite)
			{
				image.sprite = sprite;
				image.enabled = true;
				bool setNativeSize2 = setNativeSize;
				if (setNativeSize2)
				{
					image.SetNativeSize();
				}
				Action onSet2 = onSet;
				if (onSet2 != null)
				{
					onSet2();
				}
			}, null);
		}

		// Token: 0x06009D0A RID: 40202 RVA: 0x004998E4 File Offset: 0x00497AE4
		public static void SetTexturePicture(CImage image, string path, bool setNativeSize = false)
		{
			ResLoader.Load<Sprite>(path, delegate(Sprite sprite)
			{
				image.sprite = sprite;
				image.enabled = true;
				bool setNativeSize2 = setNativeSize;
				if (setNativeSize2)
				{
					image.SetNativeSize();
				}
			}, null, false);
		}

		// Token: 0x06009D0B RID: 40203 RVA: 0x0049991C File Offset: 0x00497B1C
		public static string GetBuildingName(BuildingBlockKey blockKey, short buildingTemplateId, short mapBlockTemplateId, bool showOriginName = false, bool onlyOriginName = false)
		{
			BuildingBlockItem configData = BuildingBlock.Instance[buildingTemplateId];
			bool flag = configData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				bool isTaiwuVillage = mapModel.IsAtTaiwuVillage(blockKey.AreaId, blockKey.BlockId);
				string name = (configData.Type == EBuildingBlockType.MainBuilding) ? mapModel.GetBlockName(blockKey.AreaId, blockKey.BlockId, mapBlockTemplateId, -1) : configData.Name;
				bool flag2 = !onlyOriginName && isTaiwuVillage && buildingModel.CustomBuildingName.ContainsKey(blockKey);
				if (flag2)
				{
					Dictionary<int, string> customTexts = SingletonObject.getInstance<BasicGameData>().CustomTexts;
					bool flag3 = customTexts.ContainsKey(buildingModel.CustomBuildingName[blockKey]);
					if (flag3)
					{
						string customName = SingletonObject.getInstance<BasicGameData>().CustomTexts[buildingModel.CustomBuildingName[blockKey]];
						name = (showOriginName ? LocalStringManager.GetFormat(LanguageKey.LK_Custom_Building_Name_Format, customName, name) : customName);
					}
				}
				bool flag4 = configData.Type == EBuildingBlockType.MainBuilding;
				string nameColor;
				if (flag4)
				{
					nameColor = "C16927";
				}
				else
				{
					bool flag5 = configData.Type == EBuildingBlockType.NormalResource;
					if (flag5)
					{
						nameColor = "B5DEDE";
					}
					else
					{
						bool flag6 = configData.Type == EBuildingBlockType.SpecialResource;
						if (flag6)
						{
							nameColor = "ffe78f";
						}
						else
						{
							bool flag7 = configData.Type == EBuildingBlockType.UselessResource;
							if (flag7)
							{
								nameColor = "939393";
							}
							else
							{
								bool flag8 = ViewBuildingManage.SectSpecialBuildingIdList.Contains(configData.TemplateId) || configData.TemplateId == 45 || configData.TemplateId == 51 || configData.TemplateId == 50 || configData.TemplateId == 49 || (configData.TemplateId >= 276 && configData.TemplateId <= 282) || configData.TemplateId == 283 || (configData.TemplateId >= 284 && configData.TemplateId <= 302) || (configData.TemplateId >= 303 && configData.TemplateId <= 317);
								if (flag8)
								{
									nameColor = "B975FF";
								}
								else
								{
									bool flag9 = configData.Type == EBuildingBlockType.Building;
									if (flag9)
									{
										nameColor = "F8E0CA";
									}
									else
									{
										nameColor = "lightbrown";
									}
								}
							}
						}
					}
				}
				result = name.SetColor(nameColor);
			}
			return result;
		}

		// Token: 0x06009D0C RID: 40204 RVA: 0x00499B5C File Offset: 0x00497D5C
		public static void ShowDialog(string title, string content, Action yesAction, Action noAction)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = title,
				Content = content,
				Yes = delegate()
				{
					Action yesAction2 = yesAction;
					if (yesAction2 != null)
					{
						yesAction2();
					}
				},
				No = delegate()
				{
					Action noAction2 = noAction;
					if (noAction2 != null)
					{
						noAction2();
					}
				},
				GroupYesText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Confirm),
				GroupNoText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Cancel)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.ShowUI(UIElement.Dialog, true);
		}

		// Token: 0x06009D0D RID: 40205 RVA: 0x00499C04 File Offset: 0x00497E04
		public static string GetBuildingAnimationPrefabPath(short templateId)
		{
			string iconPath = BuildingBlock.Instance[templateId].Icon;
			string iconName = iconPath.Contains('/') ? iconPath.Substring(iconPath.LastIndexOf('/') + 1) : iconPath;
			return "RemakeResources/SpineAnimations/Building/AnimationPrefab/" + iconName + "_AnimationPrefab";
		}

		// Token: 0x06009D0E RID: 40206 RVA: 0x00499C58 File Offset: 0x00497E58
		public static bool IsShowAnimation(short templateId)
		{
			return ViewBuildingManage.SectBuildingIdList.Contains(templateId) || ViewBuildingManage.CityBuildingIdList.Contains(templateId) || ViewBuildingManage.TaiwuBuildingIdList.Contains(templateId);
		}

		// Token: 0x06009D0F RID: 40207 RVA: 0x00499C94 File Offset: 0x00497E94
		public static string GetBuildingExceptionString(BuildingExceptionType exceptionType)
		{
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (exceptionType)
			{
			case BuildingExceptionType.ManageStoppedForDependency:
				languageKey = LanguageKey.LK_Building_ExceptionTip_ManageStoppedForDependency;
				goto IL_6B;
			case BuildingExceptionType.ManageStoppedForNoLeader:
				languageKey = LanguageKey.LK_Building_ExceptionTip_ManageStoppedForNoLeader;
				goto IL_6B;
			case BuildingExceptionType.ComfortableHouseEntertainNoFood:
				languageKey = LanguageKey.LK_Building_ExceptionTip_ComfortableHouseEntertainNoFood_Long;
				goto IL_6B;
			case BuildingExceptionType.BuildStoppedForWorkerShortage:
				languageKey = LanguageKey.LK_Building_ExceptionTip_BuildStoppedForWorkerShortage;
				goto IL_6B;
			case BuildingExceptionType.DemolishStoppedForWorkerShortage:
				languageKey = LanguageKey.LK_Building_ExceptionTip_DemolishStoppedForWorkerShortage;
				goto IL_6B;
			case BuildingExceptionType.EffectStoppedForDependency:
				languageKey = LanguageKey.LK_Building_ExceptionTip_EffectStoppedForDependency;
				goto IL_6B;
			case BuildingExceptionType.Damaged:
				languageKey = LanguageKey.LK_Building_ExceptionTip_Damaged;
				goto IL_6B;
			}
			throw new ArgumentOutOfRangeException();
			IL_6B:
			if (!true)
			{
			}
			LanguageKey strKey = languageKey;
			return strKey.Tr();
		}

		// Token: 0x06009D10 RID: 40208 RVA: 0x00499D1C File Offset: 0x00497F1C
		public static string GetPredictProgressText(int progressDelta, CValuePercentBonus bonus, BuildingBlockItem configData, BuildingBlockData blockData)
		{
			int delta = progressDelta * bonus;
			int needProgress = (int)configData.MaxProduceValue;
			SingletonObject.getInstance<BasicGameData>().ChallengeModeData.ApplyChallengeModeBuildingWorkHard(ref needProgress);
			return ViewBuildingManage.GetPredictProgressText((int)blockData.ShopProgress, delta, needProgress);
		}

		// Token: 0x06009D11 RID: 40209 RVA: 0x00499D5C File Offset: 0x00497F5C
		public static string GetPredictProgressText(int cur, int delta, int max)
		{
			return string.Format("{0}+{1}{2}{3}", new object[]
			{
				cur.ToString().SetColor("pinkyellow"),
				Math.Min(delta, max),
				"/".SetColor("pinkyellow"),
				max.ToString().SetColor("pinkyellow")
			});
		}

		// Token: 0x06009D12 RID: 40210 RVA: 0x00499DC4 File Offset: 0x00497FC4
		public static ShopEventItem GetShopEventConfig(short templateId)
		{
			BuildingBlockItem configData = BuildingBlock.Instance[templateId];
			bool flag = configData.SuccesEvent.Count > 0;
			ShopEventItem result;
			if (flag)
			{
				result = ShopEvent.Instance[configData.SuccesEvent[0]];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06009D13 RID: 40211 RVA: 0x00499E0E File Offset: 0x0049800E
		public static void RefreshKungfuRoomButton(CButton button, BuildingManageDisplayData displayData)
		{
			button.interactable = (displayData.BlockData.OperationType != 0);
		}

		// Token: 0x06009D14 RID: 40212 RVA: 0x00499E28 File Offset: 0x00498028
		public static void OpenKungfuRoomButtonPracticingCombatSkill(BuildingBlockKey blockKey, BuildingManageDisplayData displayData)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			sbyte buildingLevel = SingletonObject.getInstance<BuildingModel>().GetBuildingLevel(blockKey, displayData.BlockData);
			int point = ViewBuildingManage.GetPracticeCombatSkillCostActionPoint((int)buildingLevel);
			argBox.Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			argBox.Set("ShowCombatSkill", true);
			argBox.Set("CurrLocationOrganizationTemplateId", displayData.CurrLocationOrganizationTemplateId);
			argBox.Set("CheckEquipRequirePracticeLevel", false);
			argBox.Set("ShowNone", false);
			argBox.Set("AtSettlement", displayData.CanTransferItemToWarehouse);
			argBox.Set("IsTaiwuVillageBuilding", displayData.IsTaiwuVillageBuilding);
			argBox.Set("ShowSelectCount", true);
			argBox.Set("PracticeCombatSkillCostActionPoint", point);
			argBox.SetObject("UnselectableCombatSkillList", new List<short>());
			argBox.SetObject("CombatSkillIdList", displayData.CanPracticeSkills ?? new List<short>());
			argBox.SetObject("Callback2", new Action<sbyte, short, int>(delegate(sbyte type, short skillId, int count)
			{
				bool flag = type < 0 || skillId < 0;
				if (!flag)
				{
					BuildingDomainMethod.AsyncCall.PracticingCombatSkillInPracticeRoom(null, blockKey, skillId, count, point * count, delegate(int offset2, RawDataPool dataPool2)
					{
						int proficiency = 0;
						Serializer.Deserialize(dataPool2, offset2, ref proficiency);
						ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
						argumentBox.Set("Proficiency", proficiency);
						argumentBox.SetObject("CollectInfo", new List<CollectResourceResult>
						{
							new CollectResourceResult
							{
								ResourceType = -1,
								ResourceCount = proficiency,
								ItemDisplayData = null
							}
						});
						argumentBox.Set("CollectType", 3);
						UIElement.CollectResource.SetOnInitArgs(argumentBox);
						UIManager.Instance.ShowUI(UIElement.CollectResource, true);
						EasyPool.Free<ArgumentBox>(argumentBox);
					});
				}
			}));
			argBox.Set("IsShowNeiLiFinish", false);
			argBox.Set("IsNeedDefaultSelectCombatSkill", true);
			UIElement.SelectSkill.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.SelectSkill);
		}

		// Token: 0x040079AB RID: 31147
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040079AC RID: 31148
		[Header("建筑表现")]
		[SerializeField]
		private BuildingAreaBlock buildingAreaBlockOne;

		// Token: 0x040079AD RID: 31149
		[SerializeField]
		private BuildingAreaBlock buildingAreaBlockTwo;

		// Token: 0x040079AE RID: 31150
		[SerializeField]
		private GameObject animationHolder;

		// Token: 0x040079AF RID: 31151
		[SerializeField]
		private CButton buttonRepair;

		// Token: 0x040079B0 RID: 31152
		[SerializeField]
		private TooltipInvoker tipButtonRepair;

		// Token: 0x040079B1 RID: 31153
		[SerializeField]
		private CImage buildingWidth;

		// Token: 0x040079B2 RID: 31154
		[SerializeField]
		private TextMeshProUGUI textBuildingName;

		// Token: 0x040079B3 RID: 31155
		[SerializeField]
		private TextMeshProUGUI textBuildingLevel;

		// Token: 0x040079B4 RID: 31156
		[Header("重命名")]
		[SerializeField]
		private CButton buttonRename;

		// Token: 0x040079B5 RID: 31157
		[SerializeField]
		private TMP_InputField customNameInput;

		// Token: 0x040079B6 RID: 31158
		[SerializeField]
		private CanvasGroup commonWarningCanvasGroup;

		// Token: 0x040079B7 RID: 31159
		[Header("子界面")]
		[SerializeField]
		private CToggleGroup mainToggleGroup;

		// Token: 0x040079B8 RID: 31160
		[SerializeField]
		private BuildingManageSubPage[] subPages;

		// Token: 0x040079B9 RID: 31161
		[Header("功能按钮")]
		[SerializeField]
		private CButton btnTemplate;

		// Token: 0x040079BA RID: 31162
		[SerializeField]
		private GameObject buttonLayoutGroup;

		// Token: 0x040079BB RID: 31163
		[Header("基础信息")]
		[SerializeField]
		private PropertyItem propertyLevel;

		// Token: 0x040079BC RID: 31164
		[SerializeField]
		private PropertyItem propertyDamage;

		// Token: 0x040079BD RID: 31165
		[SerializeField]
		private PropertyItem propertyMaintain;

		// Token: 0x040079BE RID: 31166
		[Header("建筑效果")]
		[SerializeField]
		private GameObject effectAreaRoot;

		// Token: 0x040079BF RID: 31167
		[SerializeField]
		private PropertyItem[] effectPropertyArray;

		// Token: 0x040079C0 RID: 31168
		[Header("建筑列表")]
		[SerializeField]
		private BuildingListPanel buildingListPanel;

		// Token: 0x040079C1 RID: 31169
		[Header("百晓册入口控件")]
		[SerializeField]
		private QuickEncyclopedia quickEncyclopedia;

		// Token: 0x040079C2 RID: 31170
		private static readonly Dictionary<short, string> SoundDict = new Dictionary<short, string>
		{
			{
				44,
				"ui_building_taiwucun"
			},
			{
				129,
				"ui_building_huolianshi"
			},
			{
				139,
				"ui_building_mugongfang"
			},
			{
				179,
				"ui_building_qiaojiangwu"
			},
			{
				203,
				"ui_building_shijiao"
			},
			{
				169,
				"ui_building_xiulou"
			},
			{
				149,
				"ui_building_yaofang"
			},
			{
				159,
				"ui_building_youshi"
			},
			{
				45,
				"ui_building_citang"
			}
		};

		// Token: 0x040079C3 RID: 31171
		private short _buildingTemplateId = -1;

		// Token: 0x040079C4 RID: 31172
		private short _settlementId;

		// Token: 0x040079C5 RID: 31173
		private BuildingModel _buildingModel;

		// Token: 0x040079C6 RID: 31174
		private BuildingBlockData _blockData;

		// Token: 0x040079C7 RID: 31175
		private BuildingBlockItem _configData;

		// Token: 0x040079C8 RID: 31176
		private BuildingAreaData _areaData;

		// Token: 0x040079C9 RID: 31177
		private MapBlockData _mapBlockData;

		// Token: 0x040079CA RID: 31178
		private int _taiwuCharId;

		// Token: 0x040079CB RID: 31179
		private readonly List<CButton> _btnGroup = new List<CButton>();

		// Token: 0x040079CC RID: 31180
		private int _btnIndex = 0;

		// Token: 0x040079CD RID: 31181
		private BuildingBlockKey _blockKey;

		// Token: 0x040079CE RID: 31182
		private readonly List<GameObject> _animationGoList = new List<GameObject>();

		// Token: 0x040079CF RID: 31183
		private bool _displayInited;

		// Token: 0x040079D0 RID: 31184
		private BuildingExceptionData _buildingExceptionData = new BuildingExceptionData();

		// Token: 0x040079D1 RID: 31185
		private BuildingManageDisplayData _displayData;

		// Token: 0x040079D2 RID: 31186
		private bool _isTaiwuVillageBuilding;

		// Token: 0x040079D3 RID: 31187
		private int _initialTabKey = BuildingManageTogKey.Invalid.ToInt();

		// Token: 0x040079D4 RID: 31188
		public const string BuildingAnimationPrefabPath = "RemakeResources/SpineAnimations/Building/AnimationPrefab/";

		// Token: 0x040079D5 RID: 31189
		private Coroutine _sensitiveWordTipCoroutine;

		// Token: 0x040079D6 RID: 31190
		private Tween _sensitiveWordTipTween;

		// Token: 0x040079D7 RID: 31191
		private static readonly int BuildingNameCharCountCn = 6;

		// Token: 0x040079D8 RID: 31192
		private static readonly int BuildingNameCharCountEn = 20;

		// Token: 0x040079D9 RID: 31193
		private static readonly List<short> SectSpecialBuildingIdList = new List<short>
		{
			239,
			244,
			240,
			241,
			242,
			243,
			245,
			246,
			247,
			248,
			249,
			250,
			251,
			252,
			253,
			259,
			260,
			261,
			262,
			263,
			264,
			265,
			266,
			267,
			268,
			269,
			270,
			271,
			272,
			273
		};

		// Token: 0x040079DA RID: 31194
		private static readonly List<short> CityBuildingIdList = new List<short>
		{
			224,
			225,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			235,
			236,
			237,
			238,
			254,
			255,
			256
		};

		// Token: 0x040079DB RID: 31195
		private static readonly List<short> TaiwuBuildingIdList = new List<short>
		{
			44,
			257,
			258
		};

		// Token: 0x040079DC RID: 31196
		private static readonly List<short> SectBuildingIdList = new List<short>
		{
			239,
			240,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250,
			251,
			252,
			253
		};
	}
}
