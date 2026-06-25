using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Views;
using Game.Views.Building;
using Game.Views.Building.BuildingAreaQuickActionMenu;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Components.Building
{
	// Token: 0x02000F61 RID: 3937
	public class BuildingAreaInfo : MonoBehaviour
	{
		// Token: 0x0600B41D RID: 46109 RVA: 0x0051EDA0 File Offset: 0x0051CFA0
		private void Awake()
		{
			this.jieqingBuildingRefer.gameObject.SetActive(false);
			this.planBuilding.onClick.ResetListener(delegate()
			{
				this._buildingArea.StartPlanBuilding();
			});
			this.multiplyRemoveBuilding.onClick.ResetListener(delegate()
			{
				this._buildingArea.StartMultiplyRemoveBuilding();
			});
			this.overviewBuilding.onClick.ResetListener(delegate()
			{
				UIElement.BuildingOverview.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("isHaveChickenKing", this._haveChickenKing));
				UIManager.Instance.ShowUI(UIElement.BuildingOverview, true);
			});
			this.roleManageButton.onClick.ResetListener(new Action(BuildingActionUtils.ShowVillageRole));
			this.wareHouse.onClick.ResetListener(delegate()
			{
				UIManager.Instance.ShowUI(UIElement.Warehouse, true);
			});
			this.quickRepairBuilding.onClick.ResetListener(new Action(this.QuickRepairAllBuilding));
			this.specialFunction.onClick.ResetListener(delegate()
			{
				this.specialFunctionContainer.gameObject.SetActive(!this.specialFunctionContainer.gameObject.activeSelf);
				bool activeSelf = this.specialFunctionContainer.gameObject.activeSelf;
				if (activeSelf)
				{
					this.normalFunctionContainer.gameObject.SetActive(false);
				}
			});
			this.normalFunction.onClick.ResetListener(delegate()
			{
				this.normalFunctionContainer.gameObject.SetActive(!this.normalFunctionContainer.gameObject.activeSelf);
				bool activeSelf = this.normalFunctionContainer.gameObject.activeSelf;
				if (activeSelf)
				{
					this.specialFunctionContainer.gameObject.SetActive(false);
				}
			});
			this.settlementInformationBtn.onClick.ResetListener(delegate()
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SettlementId", this._settlementId);
				CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.SettlementInformation, args);
			});
			this.combatSkillTreeBtn.onClick.ResetListener(delegate()
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("SectTemplateId", (sbyte)this.orgTemplateId);
				CommandManager.AddCommand<CommandMaskUIWithArgs, UIElement, ArgumentBox>(EPriority.ShowUINormal, UIElement.CombatSkillTree, args);
			});
			this.collectResource.onClick.ResetListener(delegate()
			{
				BuildingDomainMethod.Call.QuickCollectShopSoldItem();
				this.UpdateQuickBtnState();
			});
			this.collectPeople.onClick.ResetListener(new Action(UI_RecruitPeopleOverview.EntryFromBuildingArea));
			this.collectPawnshop.onClick.ResetListener(delegate()
			{
				BuildingDomainMethod.AsyncCall.GetAllPawnShopItem(null, false, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
					StringBuilder itemNameStr = EasyPool.Get<StringBuilder>();
					int totalCost = 0;
					for (int i = 0; i < itemDisplayDatas.Count; i++)
					{
						ItemDisplayData item = itemDisplayDatas[i];
						int value = ItemTemplateHelper.GetBaseValue(item.Key.ItemType, item.Key.TemplateId);
						totalCost += value;
						string itemName = item.GetName(false).SetGradeColor((int)item.Grade);
						itemNameStr.Append(itemName);
						bool flag = i != itemDisplayDatas.Count - 1;
						if (flag)
						{
							itemNameStr.Append("、");
						}
					}
					UIElement.ConfirmDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new ConfirmDialogCmd
					{
						Title = LanguageKey.Lk_Building_PawnShop_GetItem_Title.Tr(),
						ContentUpper = LanguageKey.Lk_Building_PawnShop_GetItem_Content1.TrFormat(itemNameStr),
						ContentLower = LanguageKey.Lk_Building_PawnShop_GetItem_Content2.Tr(),
						ConfirmDialogCost = new List<ConfirmDialogCost>
						{
							new ConfirmDialogCost
							{
								Type = EConfirmDialogCostType.Money,
								ValueCost = totalCost,
								ValueHave = SingletonObject.getInstance<BuildingModel>().GetResourceCount(6)
							}
						},
						Yes = delegate()
						{
							BuildingDomainMethod.AsyncCall.GetAllPawnShopItem(null, true, delegate(int offset, RawDataPool dataPool)
							{
								List<ItemDisplayData> itemDisplayDatas2 = new List<ItemDisplayData>();
								Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas2);
								ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
								argBox.SetObject("ItemList", itemDisplayDatas2);
								argBox.Set("InWareHouse", true);
								UIElement.GetItem.SetOnInitArgs(argBox);
								UIManager.Instance.MaskUI(UIElement.GetItem);
								this.UpdateQuickBtnState();
							});
						}
					}));
					UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
				});
			});
			this.collectItem.onClick.ResetListener(delegate()
			{
				BuildingDomainMethod.AsyncCall.QuickCollectShopItem(null, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("ItemList", itemDisplayDatas);
					argBox.Set("InWareHouse", true);
					UIElement.GetItem.SetOnInitArgs(argBox);
					UIManager.Instance.MaskUI(UIElement.GetItem);
					this.UpdateQuickBtnState();
				});
			});
			this.collectFeather.onClick.ResetListener(delegate()
			{
				BuildingDomainMethod.AsyncCall.PluckAllChickenFeathers(null, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("ItemList", itemDisplayDatas);
					UIElement.GetItem.SetOnInitArgs(argBox);
					UIManager.Instance.MaskUI(UIElement.GetItem);
					this.UpdateQuickBtnState();
				});
			});
			this.collectAll.onClick.ResetListener(delegate()
			{
				bool activeSelf = this.collectResource.gameObject.activeSelf;
				if (activeSelf)
				{
					BuildingDomainMethod.Call.QuickCollectShopSoldItem();
					this.UpdateQuickBtnState();
				}
				List<ItemDisplayData> allItems = new List<ItemDisplayData>();
				int pendingRequests = 0;
				bool featherActive = this.collectFeather.gameObject.activeSelf && this.collectFeather.interactable;
				bool itemActive = this.collectItem.gameObject.activeSelf;
				bool peopleActive = this.collectPeople.gameObject.activeSelf;
				bool flag = featherActive;
				if (flag)
				{
					int pendingRequests2 = pendingRequests;
					pendingRequests = pendingRequests2 + 1;
				}
				bool itemActive3 = itemActive;
				if (itemActive3)
				{
					int pendingRequests2 = pendingRequests;
					pendingRequests = pendingRequests2 + 1;
				}
				bool flag2 = pendingRequests == 0;
				if (flag2)
				{
					bool peopleActive2 = peopleActive;
					if (peopleActive2)
					{
						UI_RecruitPeopleOverview.EntryFromBuildingArea();
					}
				}
				else
				{
					bool flag3 = featherActive;
					if (flag3)
					{
						BuildingDomainMethod.AsyncCall.PluckAllChickenFeathers(null, delegate(int offset, RawDataPool dataPool)
						{
							List<ItemDisplayData> featherItems = new List<ItemDisplayData>();
							Serializer.Deserialize(dataPool, offset, ref featherItems);
							allItems.AddRange(featherItems);
							int num = pendingRequests - 1;
							pendingRequests = num;
							bool flag4 = num == 0;
							if (flag4)
							{
								base.<Awake>g__ShowGetItem|20();
							}
						});
					}
					bool itemActive2 = itemActive;
					if (itemActive2)
					{
						BuildingDomainMethod.AsyncCall.QuickCollectShopItem(null, delegate(int offset, RawDataPool dataPool)
						{
							List<ItemDisplayData> shopItems = new List<ItemDisplayData>();
							Serializer.Deserialize(dataPool, offset, ref shopItems);
							allItems.AddRange(shopItems);
							int num = pendingRequests - 1;
							pendingRequests = num;
							bool flag4 = num == 0;
							if (flag4)
							{
								base.<Awake>g__ShowGetItem|20();
							}
						});
					}
				}
			});
		}

		// Token: 0x0600B41E RID: 46110 RVA: 0x0051EFA4 File Offset: 0x0051D1A4
		private void OnEnable()
		{
			this.specialFunctionContainer.gameObject.SetActive(false);
			this.normalFunctionContainer.gameObject.SetActive(false);
			GEvent.Add(UiEvents.OnUpdateQuickBtnState, new GEvent.Callback(this.OnUpdateQuickBtnState));
		}

		// Token: 0x0600B41F RID: 46111 RVA: 0x0051EFF2 File Offset: 0x0051D1F2
		private void OnDisable()
		{
			this.jieqingBuildingRefer.gameObject.SetActive(false);
			GEvent.Remove(UiEvents.OnUpdateQuickBtnState, new GEvent.Callback(this.OnUpdateQuickBtnState));
		}

		// Token: 0x0600B420 RID: 46112 RVA: 0x0051F023 File Offset: 0x0051D223
		private void OnUpdateQuickBtnState(ArgumentBox argumentBox)
		{
			this.UpdateQuickBtnState();
		}

		// Token: 0x0600B421 RID: 46113 RVA: 0x0051F02D File Offset: 0x0051D22D
		public void SetBuildingArea(ViewBuildingArea buildingArea)
		{
			this._buildingArea = buildingArea;
		}

		// Token: 0x0600B422 RID: 46114 RVA: 0x0051F038 File Offset: 0x0051D238
		public void RefreshBuildAreaInfo(bool isTaiwuVillage, bool isSectLocation, bool isCityLocation, bool isBambooHouse, MapBlockData mapBlockData, BuildingAreaData areaData, List<BuildingBlockData> blockList, short settlementId, bool haveChickenKing)
		{
			this._haveChickenKing = haveChickenKing;
			this._settlementId = settlementId;
			this.SetBtnState(isTaiwuVillage, isSectLocation, isBambooHouse);
			this.SetBuildingSpaceMouseTip();
			this.RefreshRepairBuilding();
			this.UpdateQuickBtnState();
			SettlementInfo settlementInfo = SingletonObject.getInstance<WorldMapModel>().GetLocationOrganizationInfo(mapBlockData.GetLocation());
			this.orgTemplateId = (short)settlementInfo.OrgTemplateId;
			sbyte templateId = Organization.Instance[settlementInfo.OrgTemplateId].TemplateId;
			this.UpdateQiwenxingtaiBtnState(isSectLocation, templateId);
			this.RefreshRoleManageButton(isTaiwuVillage);
			this.wareHouse.interactable = (isTaiwuVillage || isBambooHouse);
			this.RefreshSpecialFunction(mapBlockData, areaData, blockList, isTaiwuVillage);
			this.RefreshNormalFunction(mapBlockData, areaData, blockList, isTaiwuVillage);
			this.UpdateSettlementInfo(settlementInfo, isSectLocation, isTaiwuVillage);
		}

		// Token: 0x0600B423 RID: 46115 RVA: 0x0051F0F4 File Offset: 0x0051D2F4
		private void SetBtnState(bool isTaiwuVillage, bool isSectLocation, bool isBambooHouse)
		{
			TutorialChapterModel tutorial = SingletonObject.getInstance<TutorialChapterModel>();
			bool notChapter4 = tutorial.InGuiding && tutorial.TutorialChapterIndex != 3;
			bool isVillageUnlocked = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			this.middleHolder.gameObject.SetActive(isTaiwuVillage);
			this.rightHodler.gameObject.SetActive(isTaiwuVillage || notChapter4);
			this.leftHolder.gameObject.SetActive(!tutorial.InGuiding && !worldMapModel.AtSecretVillageAreaId && !isBambooHouse);
			this.topHolder.gameObject.SetActive(!tutorial.InGuiding);
			this.quickRepairBuilding.gameObject.SetActive(!tutorial.InGuiding);
			this.multiplyRemoveBuilding.gameObject.SetActive(!tutorial.InGuiding);
			this.planBuilding.gameObject.SetActive(!tutorial.InGuiding);
			this.overviewBuilding.gameObject.SetActive(isTaiwuVillage || notChapter4);
			this.overviewBuilding.interactable = (isVillageUnlocked || tutorial.InGuiding);
			this.overviewBuilding.GetComponent<TooltipInvoker>().enabled = !tutorial.InGuiding;
			this.specialFunction.gameObject.SetActive(isTaiwuVillage);
			this.roleManageButton.gameObject.SetActive(isTaiwuVillage);
			this.settlementInformationBtn.gameObject.SetActive(!isTaiwuVillage && !isBambooHouse && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(13));
			this.combatSkillTreeBtn.gameObject.SetActive(isSectLocation);
		}

		// Token: 0x0600B424 RID: 46116 RVA: 0x0051F28C File Offset: 0x0051D48C
		private void UpdateSettlementInfo(SettlementInfo settlementInfo, bool isSectLocation, bool isTaiwuVillage)
		{
			OrganizationDomainMethod.AsyncCall.GetDisplayData(null, settlementInfo.SettlementId, delegate(int offset, RawDataPool pool)
			{
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				SettlementDisplayData displayData = default(SettlementDisplayData);
				Serializer.Deserialize(pool, offset, ref displayData);
				this.populationHodler.gameObject.SetActive(!isTaiwuVillage);
				bool flag = !isTaiwuVillage;
				TooltipInvoker tooltipInvoker;
				if (flag)
				{
					TextMeshProUGUI[] populationTextArray = this.populationHodler.GetComponentsInChildren<TextMeshProUGUI>();
					TooltipInvoker populationMouseTip = this.populationHodler.GetComponent<TooltipInvoker>();
					sb.Clear();
					sb.Append(LanguageKey.LK_Population.Tr()).Append(":").Append(Math.Max(displayData.Population, 0)).Append("/").Append(Math.Max(displayData.MaxPopulation, 0));
					tooltipInvoker = populationMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					populationMouseTip.RuntimeParam.Set("arg0", sb.ToString());
					populationTextArray[0].SetText(math.max(displayData.Population, 0).ToString(), true);
					populationTextArray[1].SetText(math.max(displayData.MaxPopulation, 0).ToString(), true);
				}
				TextMeshProUGUI[] cultureTextArray = this.cultureHodler.GetComponentsInChildren<TextMeshProUGUI>();
				TooltipInvoker cultureMouseTip = this.cultureHodler.GetComponent<TooltipInvoker>();
				sb.Clear();
				sb.Append(LanguageKey.LK_Culture.Tr()).Append(":").Append(displayData.Culture).Append("/").Append(displayData.MaxCulture);
				tooltipInvoker = cultureMouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				cultureMouseTip.RuntimeParam.Set("arg0", sb.ToString());
				cultureTextArray[0].SetText(displayData.Culture.ToString(), true);
				cultureTextArray[1].SetText(displayData.MaxCulture.ToString(), true);
				TextMeshProUGUI[] safetyTextArray = this.safetyHodler.GetComponentsInChildren<TextMeshProUGUI>();
				TooltipInvoker safetyMouseTip = this.safetyHodler.GetComponent<TooltipInvoker>();
				sb.Clear();
				sb.Append(LanguageKey.LK_Safety.Tr()).Append(":").Append(displayData.Safety).Append("/").Append(displayData.MaxSafety);
				tooltipInvoker = safetyMouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				safetyMouseTip.RuntimeParam.Set("arg0", sb.ToString());
				safetyTextArray[0].SetText(displayData.Safety.ToString(), true);
				safetyTextArray[1].SetText(displayData.MaxSafety.ToString(), true);
				this.approvingRateHodler.gameObject.SetActive(isSectLocation);
				this.approvingRateDecorate.gameObject.SetActive(isSectLocation);
				bool isSectLocation2 = isSectLocation;
				if (isSectLocation2)
				{
					TextMeshProUGUI[] approvingTextArray = this.approvingRateHodler.GetComponentsInChildren<TextMeshProUGUI>();
					TooltipInvoker approvingMouseTip = this.approvingRateHodler.GetComponent<TooltipInvoker>();
					sb.Clear();
					sb.Append(LanguageKey.LK_Building_SectSupport_Title.Tr()).Append(":").Append(BuildingAreaInfo.GetApprovingRateFormatStr((int)displayData.ApprovingRate)).Append("/").Append(BuildingAreaInfo.GetApprovingRateFormatStr(displayData.ApprovingRateUpperLimit));
					tooltipInvoker = approvingMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					approvingMouseTip.RuntimeParam.Set("arg0", sb.ToString());
					approvingTextArray[0].SetText(BuildingAreaInfo.GetApprovingRateFormatStr((int)displayData.ApprovingRate), true);
					approvingTextArray[1].SetText(BuildingAreaInfo.GetApprovingRateFormatStr(displayData.ApprovingRateUpperLimit), true);
				}
				EasyPool.Free<StringBuilder>(sb);
			});
		}

		// Token: 0x0600B425 RID: 46117 RVA: 0x0051F2D0 File Offset: 0x0051D4D0
		public static string GetApprovingRateFormatStr(int approvingRate)
		{
			return Math.Round((double)((float)approvingRate / 10f), 1).ToString() + "%";
		}

		// Token: 0x0600B426 RID: 46118 RVA: 0x0051F304 File Offset: 0x0051D504
		private void SetBuildingSpaceMouseTip()
		{
			bool flag = !this.overviewBuilding.gameObject.activeSelf;
			if (!flag)
			{
				TaiwuDomainMethod.AsyncCall.GetTaiwuVillageSpaceLimitInfo(null, delegate(int offset, RawDataPool dataPool)
				{
					ValueTuple<int, int, int, int> info = default(ValueTuple<int, int, int, int>);
					Serializer.Deserialize(dataPool, offset, ref info);
					TooltipInvoker mouseTip = this.spaceMouseTipDisplayer;
					mouseTip.Type = TipType.GeneralLines;
					GeneralLineData desc = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(LanguageKey.LK_Building_SpaceTip)
						}
					};
					GeneralLineData title = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Taiwu_BuildingSpace_Title))
						}
					};
					GeneralLineData contentVillage = new GeneralLineData
					{
						Type = 5,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_VillageProvide, info.Item1)
						},
						ExtraArgs = new List<object>
						{
							20
						}
					};
					GeneralLineData contentSpaceExtraAdd = new GeneralLineData
					{
						Type = 5,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_EmeiProvide, info.Item2)
						},
						ExtraArgs = new List<object>
						{
							20
						}
					};
					GeneralLineData contentProsperousConstruction = new GeneralLineData
					{
						Type = 5,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_ProsperousConstructionProvide, info.Item3)
						},
						ExtraArgs = new List<object>
						{
							20
						}
					};
					GeneralLineData contentResourceBlockEffect = new GeneralLineData
					{
						Type = 5,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Building_Space_BuildingProvide, info.Item4)
						},
						ExtraArgs = new List<object>
						{
							20
						}
					};
					int lineCount = 3;
					TooltipInvoker tooltipInvoker = mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Taiwu_BuildingSpace_Title)).SetObject("LineData1", desc).SetObject("LineData2", title).SetObject("LineData3", contentVillage);
					bool flag2 = info.Item2 > 0;
					if (flag2)
					{
						lineCount++;
						mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentSpaceExtraAdd);
					}
					bool flag3 = info.Item3 > 0;
					if (flag3)
					{
						lineCount++;
						mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentProsperousConstruction);
					}
					bool flag4 = info.Item4 > 0;
					if (flag4)
					{
						lineCount++;
						mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentResourceBlockEffect);
					}
					mouseTip.RuntimeParam.Set("LineCount", lineCount);
				});
			}
		}

		// Token: 0x0600B427 RID: 46119 RVA: 0x0051F340 File Offset: 0x0051D540
		public void UpdateBuildingSpaceInfo(int buildingSpaceCurr, int buildingSpaceLimit)
		{
			string value = (buildingSpaceCurr > buildingSpaceLimit) ? buildingSpaceCurr.ToString().SetColor("red") : buildingSpaceCurr.ToString().SetColor("pinkyellow");
			this.spaceLimitText.SetText(string.Format("{0}/{1}", value, buildingSpaceLimit), true);
		}

		// Token: 0x0600B428 RID: 46120 RVA: 0x0051F39C File Offset: 0x0051D59C
		private void RefreshSpecialFunction(MapBlockData mapBlockData, BuildingAreaData areaData, List<BuildingBlockData> blockList, bool isTaiwuVillage)
		{
			BuildingAreaInfo.<>c__DisplayClass46_0 CS$<>8__locals1 = new BuildingAreaInfo.<>c__DisplayClass46_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.blockList = blockList;
			CS$<>8__locals1.mapBlockData = mapBlockData;
			CS$<>8__locals1.areaData = areaData;
			CS$<>8__locals1.isTaiwuVillage = isTaiwuVillage;
			EMapBlockType type;
			bool flag;
			if (!CS$<>8__locals1.isTaiwuVillage)
			{
				WorldMapModel instance = SingletonObject.getInstance<WorldMapModel>();
				MapBlockItem mapBlockItem;
				if (instance == null)
				{
					mapBlockItem = null;
				}
				else
				{
					MapBlockData currentBlockData = instance.CurrentBlockData;
					mapBlockItem = ((currentBlockData != null) ? currentBlockData.GetConfig() : null);
				}
				MapBlockItem mapBlockItem2 = mapBlockItem;
				if (mapBlockItem2 != null)
				{
					type = mapBlockItem2.Type;
					if (type != EMapBlockType.City && type != EMapBlockType.Sect)
					{
						flag = (type != EMapBlockType.Town);
						goto IL_6C;
					}
				}
				flag = false;
				IL_6C:;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.specialFunction.interactable = false;
			}
			else
			{
				BuildingDomainMethod.AsyncCall.GetBuildingFunctionData(null, delegate(int offset, RawDataPool dataPool)
				{
					BuildingAreaInfo.<>c__DisplayClass46_1 CS$<>8__locals2 = new BuildingAreaInfo.<>c__DisplayClass46_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.buildingFunctionData = null;
					Serializer.Deserialize(dataPool, offset, ref CS$<>8__locals2.buildingFunctionData);
					List<short> btnBuildingTemplateIdList = new List<short>();
					CS$<>8__locals2.buildingModel = SingletonObject.getInstance<BuildingModel>();
					CS$<>8__locals2.btnCount = 0;
					CS$<>8__locals1.<>4__this.specialFunctionContainer.ResetState();
					using (List<BuildingBlockData>.Enumerator enumerator = CS$<>8__locals1.blockList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BuildingAreaInfo.<>c__DisplayClass46_2 CS$<>8__locals3 = new BuildingAreaInfo.<>c__DisplayClass46_2();
							CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
							CS$<>8__locals3.blockData = enumerator.Current;
							bool flag3 = CS$<>8__locals3.blockData.TemplateId <= 0 || CS$<>8__locals3.blockData.OperationType == 0;
							if (!flag3)
							{
								BuildingBlockKey blockKey = new BuildingBlockKey(CS$<>8__locals1.mapBlockData.AreaId, CS$<>8__locals1.mapBlockData.BlockId, CS$<>8__locals3.blockData.BlockIndex);
								bool flag4 = CS$<>8__locals3.blockData.TemplateId == 44;
								if (flag4)
								{
									CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.Cricket, LanguageKey.LK_Building_Btn_Cricket, "ui9_btn_building_bottom_function_cricket_", new Action(BuildingActionUtils.ShowCricketCollection));
									int btnCount = CS$<>8__locals3.CS$<>8__locals2.btnCount;
									CS$<>8__locals3.CS$<>8__locals2.btnCount = btnCount + 1;
									bool canOperateStoneRoom = SingletonObject.getInstance<BuildingModel>().CanOperateStoneRoom;
									if (canOperateStoneRoom)
									{
										CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.StoneRoom, LanguageKey.LK_Building_StoneRoom, "ui9_btn_building_bottom_function_stone_house_", delegate()
										{
											BuildingActionUtils.ShowStoneHouse(blockKey);
										});
										btnCount = CS$<>8__locals3.CS$<>8__locals2.btnCount;
										CS$<>8__locals3.CS$<>8__locals2.btnCount = btnCount + 1;
									}
									bool jiaoPoolOpen = CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.JiaoPoolOpen;
									if (jiaoPoolOpen)
									{
										btnCount = CS$<>8__locals3.CS$<>8__locals2.btnCount;
										CS$<>8__locals3.CS$<>8__locals2.btnCount = btnCount + 1;
										TemplatedContainer templatedContainer = CS$<>8__locals1.<>4__this.specialFunctionContainer;
										TemplatedContainer.ButtonOrderType buttonType = TemplatedContainer.ButtonOrderType.JiaoPool;
										LanguageKey nameKey = LanguageKey.LK_Building_Jiaochi;
										string iconName = "ui9_btn_building_bottom_function_jiao_pool_";
										Action click;
										if ((click = CS$<>8__locals1.<>9__3) == null)
										{
											click = (CS$<>8__locals1.<>9__3 = delegate()
											{
												BuildingActionUtils.ShowJiaoPool(CS$<>8__locals1.areaData);
											});
										}
										templatedContainer.CreateBtn(buttonType, nameKey, iconName, click);
									}
								}
								else
								{
									bool flag5 = CS$<>8__locals3.blockData.TemplateId == 52 && !btnBuildingTemplateIdList.Contains(52);
									if (flag5)
									{
										btnBuildingTemplateIdList.Add(52);
										bool isTaiwuVillage2 = CS$<>8__locals1.isTaiwuVillage;
										if (isTaiwuVillage2)
										{
											CButton btn = CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.KungfuPracticeRoom, LanguageKey.LK_Building_KungfuRoomIcon, "ui9_btn_building_bottom_function_practice_", new Action(BuildingActionUtils.ShowKungfuPracticeRoom));
											btn.interactable = (CS$<>8__locals3.blockData.OperationType != 0);
										}
										Action<sbyte, short, int> <>9__8;
										CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.PracticeCombatSkill, LanguageKey.LK_PracticeCombatSkill_Name, "ui9_btn_building_bottom_function_practice_skill_", delegate()
										{
											ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
											argBox.Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
											argBox.Set("ShowCombatSkill", true);
											argBox.Set("CurrLocationOrganizationTemplateId", CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.OrganizationTemplateIdOfTaiwuLocation);
											argBox.Set("CheckEquipRequirePracticeLevel", false);
											argBox.Set("ShowNone", false);
											argBox.Set("AtSettlement", CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.CanTransfer);
											argBox.Set("IsTaiwuVillageBuilding", CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.isTaiwuVillage);
											argBox.Set("ShowSelectCount", true);
											argBox.Set("PracticeCombatSkillCostActionPoint", UI_BuildingManage.GetPracticeCombatSkillCostActionPoint((int)CS$<>8__locals3.CS$<>8__locals2.buildingModel.GetBuildingLevel(blockKey, CS$<>8__locals3.blockData)));
											argBox.SetObject("UnselectableCombatSkillList", new List<short>());
											argBox.SetObject("CombatSkillIdList", CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.CanPracticeSkills);
											ArgumentBox argumentBox = argBox;
											string key = "Callback2";
											Action<sbyte, short, int> arg;
											if ((arg = <>9__8) == null)
											{
												arg = (<>9__8 = delegate(sbyte type, short skillId, int count)
												{
													bool flag9 = type < 0 || skillId < 0;
													if (!flag9)
													{
														BuildingDomainMethod.AsyncCall.PracticingCombatSkillInPracticeRoom(null, blockKey, skillId, count, UI_BuildingManage.GetPracticeCombatSkillCostActionPoint((int)CS$<>8__locals3.CS$<>8__locals2.buildingModel.GetBuildingLevel(blockKey, CS$<>8__locals3.blockData)) * count, delegate(int offset2, RawDataPool dataPool2)
														{
															int proficiency = 0;
															Serializer.Deserialize(dataPool2, offset2, ref proficiency);
															ArgumentBox argumentBox2 = EasyPool.Get<ArgumentBox>();
															argumentBox2.Set("Proficiency", proficiency);
															argumentBox2.SetObject("CollectInfo", new List<CollectResourceResult>
															{
																new CollectResourceResult
																{
																	ResourceType = -1,
																	ResourceCount = proficiency,
																	ItemDisplayData = null
																}
															});
															argumentBox2.Set("CollectType", 3);
															UIElement.CollectResource.SetOnInitArgs(argumentBox2);
															UIManager.Instance.ShowUI(UIElement.CollectResource, true);
															EasyPool.Free<ArgumentBox>(argumentBox2);
														});
													}
												});
											}
											argumentBox.SetObject(key, arg);
											argBox.Set("IsShowNeiLiFinish", false);
											argBox.Set("IsNeedDefaultSelectCombatSkill", true);
											UIElement.SelectSkill.SetOnInitArgs(argBox);
											UIManager.Instance.MaskUI(UIElement.SelectSkill);
										});
									}
									else
									{
										bool flag6 = CS$<>8__locals3.blockData.TemplateId == 50;
										if (flag6)
										{
											CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.SamsaraPlatform, LanguageKey.LK_Building_Samsara_Platform, "ui9_btn_building_bottom_function_samsara_", delegate()
											{
												SingletonObject.getInstance<CharacterMonitorModel>().RefreshAllMonitorCharacterAliveState();
												ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("BuildingData", CS$<>8__locals3.blockData).SetObject("BuildingKey", blockKey);
												UIElement.SamsaraPlatform.SetOnInitArgs(args);
												UIManager.Instance.ShowUI(UIElement.SamsaraPlatform, true);
											});
											bool jingangFunctionOpen = CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.JingangFunctionOpen;
											if (jingangFunctionOpen)
											{
												CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.SwapSoul, LanguageKey.LK_Building_Btn_SoulSwapCeremony, "ui9_btn_building_bottom_function_swap_soul_", new Action(BuildingActionUtils.ShowSwapSoul));
											}
											bool jingangMonkSoul = CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.JingangMonkSoul;
											if (jingangMonkSoul)
											{
												CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.MonkSoul, LanguageKey.UI_SectMainStory_Jingang_MonkSoul, "ui9_btn_building_bottom_function_swap_soul_", new Action(BuildingDomainMethod.Call.SectMainStoryJingangClickMonkSoulBtn));
											}
										}
										else
										{
											bool flag7 = CS$<>8__locals3.blockData.TemplateId == 49 && !btnBuildingTemplateIdList.Contains(49);
											if (flag7)
											{
												btnBuildingTemplateIdList.Add(52);
												CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.ChickenCoop, LanguageKey.LK_Building_ChickenCoop, "ui9_btn_building_bottom_function_chicken_coop_", delegate()
												{
													BuildingActionUtils.ShowChickenCoop(blockKey);
												});
												bool fulongFunctionOpen = CS$<>8__locals3.CS$<>8__locals2.buildingFunctionData.FulongFunctionOpen;
												if (fulongFunctionOpen)
												{
													CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.AssignChicken, LanguageKey.LK_AssignChicken_Title, "ui9_btn_building_bottom_function_village_role_", new Action(BuildingActionUtils.ShowVillageRoleChickenAssign));
												}
											}
											else
											{
												bool flag8 = CS$<>8__locals3.blockData.TemplateId == 51;
												if (flag8)
												{
													CS$<>8__locals1.<>4__this.specialFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.TeaHouseCaravan, LanguageKey.LK_Building_TeaHouseCaravan, "ui9_btn_building_bottom_function_tea_house_", delegate()
													{
														BuildingActionUtils.ShowTeaHorseCaravan(CS$<>8__locals3.blockData, blockKey);
													});
												}
											}
										}
									}
								}
							}
						}
					}
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.specialFunction.interactable = (CS$<>8__locals2.btnCount > 0);
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.specialFunctionContainer.SortBtn();
					});
				});
			}
		}

		// Token: 0x0600B429 RID: 46121 RVA: 0x0051F440 File Offset: 0x0051D640
		private void RefreshNormalFunction(MapBlockData mapBlockData, BuildingAreaData areaData, List<BuildingBlockData> blockList, bool isTaiwuVillage)
		{
			BuildingAreaInfo.<>c__DisplayClass47_0 CS$<>8__locals1 = new BuildingAreaInfo.<>c__DisplayClass47_0();
			CS$<>8__locals1.mapBlockData = mapBlockData;
			CS$<>8__locals1.areaData = areaData;
			bool flag;
			if (!isTaiwuVillage)
			{
				WorldMapModel instance = SingletonObject.getInstance<WorldMapModel>();
				MapBlockItem mapBlockItem;
				if (instance == null)
				{
					mapBlockItem = null;
				}
				else
				{
					MapBlockData currentBlockData = instance.CurrentBlockData;
					mapBlockItem = ((currentBlockData != null) ? currentBlockData.GetConfig() : null);
				}
				MapBlockItem mapBlockItem2 = mapBlockItem;
				if (mapBlockItem2 != null)
				{
					EMapBlockType type = mapBlockItem2.Type;
					if (type != EMapBlockType.City && type != EMapBlockType.Sect)
					{
						flag = (type != EMapBlockType.Town);
						goto IL_59;
					}
				}
				flag = false;
				IL_59:;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.normalFunction.interactable = false;
			}
			else
			{
				int btnCount = 0;
				this.normalFunctionContainer.ResetState();
				List<short> btnBuildingTemplateIdList = new List<short>();
				using (List<BuildingBlockData>.Enumerator enumerator = blockList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BuildingAreaInfo.<>c__DisplayClass47_1 CS$<>8__locals2 = new BuildingAreaInfo.<>c__DisplayClass47_1();
						CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
						CS$<>8__locals2.blockData = enumerator.Current;
						bool flag3 = CS$<>8__locals2.blockData.TemplateId <= 0 || CS$<>8__locals2.blockData.OperationType == 0;
						if (!flag3)
						{
							BuildingBlockKey blockKey = new BuildingBlockKey(CS$<>8__locals2.CS$<>8__locals1.mapBlockData.AreaId, CS$<>8__locals2.CS$<>8__locals1.mapBlockData.BlockId, CS$<>8__locals2.blockData.BlockIndex);
							BuildingBlockItem configData = BuildingBlock.Instance[CS$<>8__locals2.blockData.TemplateId];
							short templateId = configData.TemplateId;
							short num = templateId;
							if (num >= 276)
							{
								if (num >= 303)
								{
									if (num <= 317)
									{
										this.normalFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.Prison, LanguageKey.LK_SettlementPrison, "ui9_btn_building_bottom_function_prison_", delegate()
										{
											ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
											argsBox.SetObject("MapBlockData", CS$<>8__locals2.CS$<>8__locals1.mapBlockData);
											argsBox.SetObject("BuildingBlockData", CS$<>8__locals2.blockData);
											argsBox.Set<BuildingAreaData>("BuildingAreaData", CS$<>8__locals2.CS$<>8__locals1.areaData);
											UIElement.BuildingManage.SetOnInitArgs(argsBox);
											UIManager.Instance.ShowUI(UIElement.BuildingManage, true);
										});
										btnCount++;
									}
								}
								else if (num > 282)
								{
									if (num != 283)
									{
										this.normalFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.SettlementTreasury, LanguageKey.LK_Building_Treasury, "ui9_btn_building_bottom_function_treasury_shop_", delegate()
										{
											BuildingActionUtils.ShowTreasuryShop(CS$<>8__locals2.blockData);
										});
										btnCount++;
									}
									else
									{
										this.normalFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.WuHuZhenBao, LanguageKey.LK_Merchant, "ui9_btn_building_bottom_function_merchant_", delegate()
										{
											BuildingActionUtils.ShowSpecialShop(configData);
										});
										btnCount++;
									}
								}
								else
								{
									this.normalFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.MerchantBuilding, LanguageKey.LK_Merchant, "ui9_btn_building_bottom_function_merchant_", delegate()
									{
										BuildingActionUtils.ShowMerchant(configData, CS$<>8__locals2.CS$<>8__locals1.mapBlockData.AreaId);
									});
									btnCount++;
								}
							}
							else if (num >= 239)
							{
								if (num <= 253)
								{
									this.normalFunctionContainer.CreateBtn(TemplatedContainer.ButtonOrderType.Sect, ViewBuildingArea.GetBuildingName(configData, blockKey, CS$<>8__locals2.CS$<>8__locals1.mapBlockData.TemplateId, false), "ui9_btn_building_bottom_function_sect_", delegate()
									{
										TaiwuEventDomainMethod.Call.OnSectSpecialBuildingClicked(configData.TemplateId);
									});
									btnCount++;
								}
							}
							bool flag4 = configData.CanMakeItem && !btnBuildingTemplateIdList.Contains(configData.TemplateId);
							if (flag4)
							{
								btnBuildingTemplateIdList.Add(configData.TemplateId);
								TemplatedContainer.ButtonOrderType orderType = TemplatedContainer.GetButtonOrderType(configData.RequireLifeSkillType);
								TemplatedContainer templatedContainer = this.normalFunctionContainer;
								TemplatedContainer.ButtonOrderType buttonType = orderType;
								short templateId2 = CS$<>8__locals2.blockData.TemplateId;
								templatedContainer.CreateBtn(buttonType, (templateId2 == 257 || templateId2 == 258) ? LanguageKey.LK_Building_FixWood.Tr() : configData.Name, BuildingAreaInfo.GetMakeBuildingIcon(configData.TemplateId), delegate()
								{
									BuildingActionUtils.ShowMake(CS$<>8__locals2.blockData, blockKey, UI_Make.UIMakeTab.Make);
								});
								btnCount++;
							}
						}
					}
				}
				this.normalFunctionContainer.SortBtn();
				this.normalFunction.interactable = (btnCount > 0);
			}
		}

		// Token: 0x0600B42A RID: 46122 RVA: 0x0051F840 File Offset: 0x0051DA40
		private static string GetMakeBuildingIcon(short templateId)
		{
			if (!true)
			{
			}
			string result;
			if (templateId <= 159)
			{
				if (templateId <= 139)
				{
					if (templateId == 129)
					{
						result = "ui9_btn_building_bottom_function_forging_";
						goto IL_AF;
					}
					if (templateId != 139)
					{
						goto IL_A7;
					}
				}
				else
				{
					if (templateId == 149)
					{
						result = "ui9_btn_building_bottom_function_medicine_";
						goto IL_AF;
					}
					if (templateId != 159)
					{
						goto IL_A7;
					}
					result = "ui9_btn_building_bottom_function_toxicology_";
					goto IL_AF;
				}
			}
			else if (templateId <= 179)
			{
				if (templateId == 169)
				{
					result = "ui9_btn_building_bottom_function_weaving_";
					goto IL_AF;
				}
				if (templateId != 179)
				{
					goto IL_A7;
				}
				result = "ui9_btn_building_bottom_function_jade_";
				goto IL_AF;
			}
			else
			{
				if (templateId == 203)
				{
					result = "ui9_btn_building_bottom_function_kitchen_";
					goto IL_AF;
				}
				if (templateId - 257 > 1)
				{
					goto IL_A7;
				}
			}
			result = "ui9_btn_building_bottom_function_wood_working_";
			goto IL_AF;
			IL_A7:
			result = string.Empty;
			IL_AF:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B42B RID: 46123 RVA: 0x0051F908 File Offset: 0x0051DB08
		public void UpdateQuickBtnState()
		{
			BuildingDomainMethod.AsyncCall.QuickCollectShopItemCount(null, delegate(int offset, RawDataPool dataPool)
			{
				int count = 0;
				Serializer.Deserialize(dataPool, offset, ref count);
				this.collectItem.gameObject.SetActive(count > 0);
			});
			BuildingDomainMethod.AsyncCall.QuickCollectShopSoldItemCount(null, delegate(int offset, RawDataPool dataPool)
			{
				int count = 0;
				Serializer.Deserialize(dataPool, offset, ref count);
				this.collectResource.gameObject.SetActive(count > 0);
			});
			this.RefreshQuickRecruitPeopleCount();
			BuildingDomainMethod.AsyncCall.GetAllPawnShopItem(null, false, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
				this.collectPawnshop.gameObject.SetActive(itemDisplayDatas.Count > 0);
			});
			BuildingDomainMethod.AsyncCall.QuickCollectBuildingEarnCount(null, delegate(int offset, RawDataPool dataPool)
			{
				int count = 0;
				Serializer.Deserialize(dataPool, offset, ref count);
				this.collectAll.interactable = (count > 0);
			});
			BuildingDomainMethod.AsyncCall.IsAnyChickensCanPluck(null, delegate(int offset, RawDataPool dataPool)
			{
				bool res = false;
				Serializer.Deserialize(dataPool, offset, ref res);
				this.collectFeather.gameObject.SetActive(res);
				this.collectFeather.interactable = SingletonObject.getInstance<WorldMapModel>().IsAtTaiwuVillage(-1, -1);
				this.collectFeather.GetComponent<TooltipInvoker>().PresetParam[0] = (this.collectFeather.interactable ? LanguageKey.LK_ChickenPluckAllFeathers_Tip.Tr() : LanguageKey.LK_ChickenPluckAllFeathers_Tip_NotInTaiwuVillager.Tr());
			});
			GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
		}

		// Token: 0x0600B42C RID: 46124 RVA: 0x0051F98B File Offset: 0x0051DB8B
		private void RefreshQuickRecruitPeopleCount()
		{
			BuildingDomainMethod.AsyncCall.QuickRecruitPeopleCount(null, delegate(int offset, RawDataPool dataPool)
			{
				int count = 0;
				Serializer.Deserialize(dataPool, offset, ref count);
				this.collectPeople.gameObject.SetActive(count > 0);
			});
		}

		// Token: 0x0600B42D RID: 46125 RVA: 0x0051F9A1 File Offset: 0x0051DBA1
		public void RefreshRepairBuilding()
		{
			BuildingDomainMethod.AsyncCall.CalcQuickRepairAllBuildingCostMoney(null, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._quickRepairAllCostMoney);
				this.quickRepairBuilding.interactable = (this._quickRepairAllCostMoney > 0);
			});
		}

		// Token: 0x0600B42E RID: 46126 RVA: 0x0051F9B8 File Offset: 0x0051DBB8
		public void QuickRepairAllBuilding()
		{
			UIElement confirmDialog = UIElement.ConfirmDialog;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			string key = "Cmd";
			ConfirmDialogCmd confirmDialogCmd = new ConfirmDialogCmd();
			confirmDialogCmd.Title = LanguageKey.LK_Building_QuickFix.Tr();
			confirmDialogCmd.ContentUpper = "";
			confirmDialogCmd.ContentLower = LanguageKey.LK_Building_ConfirmOperate.TrFormat(LanguageKey.LK_Building_QuickFix.Tr());
			confirmDialogCmd.ConfirmDialogCost = new List<ConfirmDialogCost>
			{
				new ConfirmDialogCost
				{
					Type = EConfirmDialogCostType.Money,
					ValueCost = this._quickRepairAllCostMoney,
					ValueHave = SingletonObject.getInstance<BuildingModel>().GetResourceCount(6)
				}
			};
			confirmDialogCmd.Yes = delegate()
			{
				BuildingDomainMethod.Call.QuickRepairAllBuilding();
				GEvent.OnEvent(UiEvents.RepairAllBuilding, null);
			};
			confirmDialog.SetOnInitArgs(argumentBox.SetObject(key, confirmDialogCmd));
			UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
		}

		// Token: 0x0600B42F RID: 46127 RVA: 0x0051FA94 File Offset: 0x0051DC94
		private void RefreshRoleManageButton(bool isTaiwuVillage)
		{
			bool hasTaiwuShrine = ViewBuildingArea.HasBuilding(45, true);
			this.roleManageButton.interactable = (isTaiwuVillage && hasTaiwuShrine);
		}

		// Token: 0x0600B430 RID: 46128 RVA: 0x0051FABC File Offset: 0x0051DCBC
		private void UpdateQiwenxingtaiBtnState(bool isSectLocation, sbyte templateId)
		{
			bool flag = !isSectLocation;
			if (!flag)
			{
				bool orgAvailable = BuildingAreaInfo.JieQingData.OrgAvailable((short)templateId);
				bool flag2 = !orgAvailable;
				if (!flag2)
				{
					OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
					{
						bool unlock = false;
						Serializer.Deserialize(pool, offset, ref unlock);
						this.jieqingBuildingRefer.gameObject.SetActive(unlock);
						bool flag3 = !unlock;
						if (!flag3)
						{
							CButtonObsolete button = this.jieqingBuildingRefer.CGet<CButtonObsolete>("JieQingSpecialBuild");
							button.interactable = false;
							button.onClick.ResetListener(delegate()
							{
								GEvent.OnEvent(UiEvents.BuildQiwenxingtai, null);
							});
							this.RefreshQiWenXingtaiTips(templateId);
						}
					});
				}
			}
		}

		// Token: 0x0600B431 RID: 46129 RVA: 0x0051FB14 File Offset: 0x0051DD14
		public void RefreshQiWenXingtaiTips(sbyte templateId)
		{
			this.jieqingBuildingRefer.CGet<TextMeshProUGUI>("BuildingSpaceLimitText").text = string.Format("{0}/1", BuildingAreaInfo.JieQingData.HasQiwenXingtai ? 1 : 0);
			this._jieQingData.ApprovingRateInit = false;
			OrganizationDomainMethod.AsyncCall.GetOrganizationCombatSkillsDisplayData(null, templateId, delegate(int offset, RawDataPool dataPool)
			{
				OrganizationCombatSkillsDisplayData data = new OrganizationCombatSkillsDisplayData();
				Serializer.Deserialize(dataPool, offset, ref data);
				this._jieQingData.ApprovingRate = data.ApprovingRate;
				this._jieQingData.ApprovingRateInit = true;
				this._jieQingData.TryRefresh((short)templateId, this.jieqingBuildingRefer);
			});
			this._jieQingData.CountInit = false;
			ExtraDomainMethod.AsyncCall.GetSectExtraLegacyBuildingCounts(null, delegate(int offset, RawDataPool dataPool)
			{
				ValueTuple<int, int> counts = new ValueTuple<int, int>(0, 0);
				Serializer.Deserialize(dataPool, offset, ref counts);
				this._jieQingData.CurrCount = counts.Item1;
				this._jieQingData.MaxCount = counts.Item2;
				this._jieQingData.CountInit = true;
				this._jieQingData.TryRefresh((short)templateId, this.jieqingBuildingRefer);
			});
		}

		// Token: 0x04008C28 RID: 35880
		[SerializeField]
		private CButton overviewBuilding;

		// Token: 0x04008C29 RID: 35881
		[SerializeField]
		private CButton planBuilding;

		// Token: 0x04008C2A RID: 35882
		[SerializeField]
		private CButton multiplyRemoveBuilding;

		// Token: 0x04008C2B RID: 35883
		[SerializeField]
		private CButton quickRepairBuilding;

		// Token: 0x04008C2C RID: 35884
		[SerializeField]
		private CButton collectAll;

		// Token: 0x04008C2D RID: 35885
		[SerializeField]
		private CButton collectResource;

		// Token: 0x04008C2E RID: 35886
		[SerializeField]
		private CButton collectPeople;

		// Token: 0x04008C2F RID: 35887
		[SerializeField]
		private CButton collectItem;

		// Token: 0x04008C30 RID: 35888
		[SerializeField]
		private CButton collectPawnshop;

		// Token: 0x04008C31 RID: 35889
		[SerializeField]
		private CButton collectFeather;

		// Token: 0x04008C32 RID: 35890
		[SerializeField]
		private CButton wareHouse;

		// Token: 0x04008C33 RID: 35891
		[SerializeField]
		private CButton normalFunction;

		// Token: 0x04008C34 RID: 35892
		[SerializeField]
		private CButton specialFunction;

		// Token: 0x04008C35 RID: 35893
		[SerializeField]
		private CButton roleManageButton;

		// Token: 0x04008C36 RID: 35894
		[SerializeField]
		private CButton settlementInformationBtn;

		// Token: 0x04008C37 RID: 35895
		[SerializeField]
		private CButton combatSkillTreeBtn;

		// Token: 0x04008C38 RID: 35896
		[SerializeField]
		private TextMeshProUGUI spaceLimitText;

		// Token: 0x04008C39 RID: 35897
		[SerializeField]
		private TextMeshProUGUI sectSupportText;

		// Token: 0x04008C3A RID: 35898
		[SerializeField]
		private TooltipInvoker spaceMouseTipDisplayer;

		// Token: 0x04008C3B RID: 35899
		[SerializeField]
		private TemplatedContainer specialFunctionContainer;

		// Token: 0x04008C3C RID: 35900
		[SerializeField]
		private TemplatedContainer normalFunctionContainer;

		// Token: 0x04008C3D RID: 35901
		[SerializeField]
		private RectTransform middleHolder;

		// Token: 0x04008C3E RID: 35902
		[SerializeField]
		private RectTransform rightHodler;

		// Token: 0x04008C3F RID: 35903
		[SerializeField]
		private RectTransform leftHolder;

		// Token: 0x04008C40 RID: 35904
		[SerializeField]
		private RectTransform topHolder;

		// Token: 0x04008C41 RID: 35905
		[SerializeField]
		private RectTransform populationHodler;

		// Token: 0x04008C42 RID: 35906
		[SerializeField]
		private RectTransform cultureHodler;

		// Token: 0x04008C43 RID: 35907
		[SerializeField]
		private RectTransform safetyHodler;

		// Token: 0x04008C44 RID: 35908
		[SerializeField]
		private RectTransform approvingRateHodler;

		// Token: 0x04008C45 RID: 35909
		[SerializeField]
		private RectTransform approvingRateDecorate;

		// Token: 0x04008C46 RID: 35910
		[SerializeField]
		private Refers jieqingBuildingRefer;

		// Token: 0x04008C47 RID: 35911
		private ViewBuildingArea _buildingArea;

		// Token: 0x04008C48 RID: 35912
		private bool _haveChickenKing;

		// Token: 0x04008C49 RID: 35913
		private short _settlementId;

		// Token: 0x04008C4A RID: 35914
		private short orgTemplateId;

		// Token: 0x04008C4B RID: 35915
		private int _quickRepairAllCostMoney;

		// Token: 0x04008C4C RID: 35916
		private readonly BuildingAreaInfo.JieQingData _jieQingData = new BuildingAreaInfo.JieQingData();

		// Token: 0x0200258D RID: 9613
		private class JieQingData
		{
			// Token: 0x17001B92 RID: 7058
			// (get) Token: 0x06010C0A RID: 68618 RVA: 0x0066E1EE File Offset: 0x0066C3EE
			public static BuildingBlockItem Config
			{
				get
				{
					return BuildingBlock.Instance[275];
				}
			}

			// Token: 0x17001B93 RID: 7059
			// (get) Token: 0x06010C0B RID: 68619 RVA: 0x0066E1FF File Offset: 0x0066C3FF
			public static bool HasQiwenXingtai
			{
				get
				{
					return ViewBuildingArea.HasBuilding(275, false);
				}
			}

			// Token: 0x17001B94 RID: 7060
			// (get) Token: 0x06010C0C RID: 68620 RVA: 0x0066E20C File Offset: 0x0066C40C
			public static bool ResourceEngough
			{
				get
				{
					return CommonUtils.IsBuildingCostResourcesEnough(BuildingAreaInfo.JieQingData.Config);
				}
			}

			// Token: 0x06010C0D RID: 68621 RVA: 0x0066E218 File Offset: 0x0066C418
			public static bool OrgAvailable(short templateId)
			{
				return BuildingAreaInfo.JieQingData.Config.AvailableOrganization.Contains(templateId);
			}

			// Token: 0x06010C0E RID: 68622 RVA: 0x0066E22C File Offset: 0x0066C42C
			public void TryRefresh(short templateId, Refers jieqingBuildingRefer)
			{
				bool flag = !this.ApprovingRateInit || !this.CountInit;
				if (!flag)
				{
					bool approvingRateReach = BuildingAreaInfo.JieQingData.Config.ApprovingRate <= this.ApprovingRate;
					List<ESpecialBuildErrorType> errorTypes = new List<ESpecialBuildErrorType>();
					bool flag2 = !approvingRateReach;
					if (flag2)
					{
						errorTypes.Add(ESpecialBuildErrorType.Approve);
					}
					bool flag3 = !BuildingAreaInfo.JieQingData.ResourceEngough;
					if (flag3)
					{
						errorTypes.Add(ESpecialBuildErrorType.Resrouce);
					}
					bool hasQiwenXingtai = BuildingAreaInfo.JieQingData.HasQiwenXingtai;
					if (hasQiwenXingtai)
					{
						errorTypes.Add(ESpecialBuildErrorType.AlreadyBuilt);
					}
					bool flag4 = !BuildingAreaInfo.JieQingData.OrgAvailable(templateId);
					if (flag4)
					{
						errorTypes.Add(ESpecialBuildErrorType.NotAvailable);
					}
					bool flag5 = this.CurrCount >= this.MaxCount;
					if (flag5)
					{
						errorTypes.Add(ESpecialBuildErrorType.ReachMaxCount);
					}
					bool canInteract = errorTypes.Count == 0;
					DisableStyleRoot disableStyleRoot = jieqingBuildingRefer.CGet<DisableStyleRoot>("DisableStyleRoot");
					disableStyleRoot.SetStyleEffect(!canInteract, false);
					jieqingBuildingRefer.CGet<CButtonObsolete>("JieQingSpecialBuild").interactable = canInteract;
					TooltipInvoker tips = jieqingBuildingRefer.CGet<TooltipInvoker>("MouseTipDisplayer");
					tips.Type = TipType.SpecialBuild;
					TooltipInvoker tooltipInvoker = tips;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tips.RuntimeParam.SetObject("BuildingBlockItem", BuildingAreaInfo.JieQingData.Config);
					tips.RuntimeParam.SetObject("ErrorTypes", errorTypes);
					tips.RuntimeParam.Set("ApproveRate", this.ApprovingRate);
					tips.RuntimeParam.Set("ApproveNeeded", BuildingAreaInfo.JieQingData.Config.ApprovingRate);
					tips.RuntimeParam.Set("CurCount", this.CurrCount);
					tips.RuntimeParam.Set("MaxCount", this.MaxCount);
				}
			}

			// Token: 0x0400E846 RID: 59462
			public short ApprovingRate = 0;

			// Token: 0x0400E847 RID: 59463
			public bool ApprovingRateInit = false;

			// Token: 0x0400E848 RID: 59464
			public int CurrCount = 0;

			// Token: 0x0400E849 RID: 59465
			public int MaxCount = 0;

			// Token: 0x0400E84A RID: 59466
			public bool CountInit = false;
		}
	}
}
