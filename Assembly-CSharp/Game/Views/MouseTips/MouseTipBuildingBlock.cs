using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Config.Common;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Avatar;
using Game.Components.Item;
using Game.Components.MouseTip;
using Game.Views.Building;
using GameData.Combat.Math;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000839 RID: 2105
	public class MouseTipBuildingBlock : MouseTipBase
	{
		// Token: 0x17000C59 RID: 3161
		// (get) Token: 0x060066AC RID: 26284 RVA: 0x002ECE87 File Offset: 0x002EB087
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C5A RID: 3162
		// (get) Token: 0x060066AD RID: 26285 RVA: 0x002ECE8A File Offset: 0x002EB08A
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x060066AE RID: 26286 RVA: 0x002ECE94 File Offset: 0x002EB094
		protected override void Init(ArgumentBox argsBox)
		{
			bool isTaiwuVillage;
			argsBox.Get("IsTaiwuVillage", out isTaiwuVillage);
			sbyte buildingLevel;
			argsBox.Get("BuildingLevel", out buildingLevel);
			BuildingBlockKey blockKey;
			argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out blockKey);
			BuildingBlockData blockData;
			argsBox.Get<BuildingBlockData>("BuildingBlockData", out blockData);
			this._simpleMode = (!isTaiwuVillage || blockData.OperationType == 0);
			BuildingBlockItem buildingBlockItem = BuildingBlock.Instance[blockData.TemplateId];
			ViewBuildingArea.SetBuildingIcon(this.buildingIcon, buildingBlockItem, true, null);
			RectTransform buildingIconRect = this.buildingIcon.GetComponent<RectTransform>();
			float iconScale = (buildingBlockItem.Width == 1) ? 0.5f : 0.25f;
			buildingIconRect.localScale = new Vector3(iconScale, iconScale, iconScale);
			this.buildingIcon.GetComponent<ImageAspectKeeper>().Refresh();
			this.buildingName.SetText(buildingBlockItem.Name.ColorReplace(), true);
			this.desc.SetText(buildingBlockItem.Desc.ColorReplace(), true);
			this._hasFunctionDesc = !string.IsNullOrEmpty(buildingBlockItem.FuncDesc);
			bool flag = blockData.TemplateId == 275;
			if (flag)
			{
				this.funcDescHolder.gameObject.SetActive(false);
				OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
				{
					bool jieqingSpecialUnlocked = false;
					Serializer.Deserialize(dataPool, offset, ref jieqingSpecialUnlocked);
					this.funcDescHolder.gameObject.SetActive(this._simpleMode && this._hasFunctionDesc && jieqingSpecialUnlocked);
					this.funcDesc.SetText(buildingBlockItem.FuncDesc.ColorReplace(), true);
				});
			}
			else
			{
				this.funcDesc.SetText(buildingBlockItem.FuncDesc.ColorReplace(), true);
				this.funcDescHolder.gameObject.SetActive(this._simpleMode && this._hasFunctionDesc);
			}
			this._hasManagerList = buildingBlockItem.IsShop;
			this.baseInfoHolder.gameObject.SetActive(!this._simpleMode);
			this.moreInfo.gameObject.SetActive(!this._simpleMode);
			this.extraHolder.gameObject.SetActive(false);
			this.scaleEffectHolder.gameObject.SetActive(false);
			this.totalScaleEffectHolder.gameObject.SetActive(false);
			this.shopProgressHolder.gameObject.SetActive(false);
			this.getItemHolder.transform.parent.gameObject.SetActive(false);
			this.getPeopleHolder.transform.parent.gameObject.SetActive(false);
			this.getResourceHolder.transform.parent.gameObject.SetActive(false);
			this.soldItemHodler.transform.parent.gameObject.SetActive(false);
			this.managerListHodler.transform.parent.gameObject.SetActive(false);
			bool flag2 = !this._simpleMode;
			if (flag2)
			{
				TextMeshProUGUI[] levelTextArray = this.levelHolder.GetComponentsInChildren<TextMeshProUGUI>();
				levelTextArray[0].SetText(LanguageKey.LK_Building_Level.Tr(), true);
				levelTextArray[1].SetText(buildingLevel.ToString() + "/" + buildingBlockItem.MaxLevel.ToString(), true);
				this.maintenanceHolder.gameObject.SetActive(buildingBlockItem.BaseMaintenanceCost.Count > 0);
				string damageText = string.Format("{0}%", (int)((buildingBlockItem.MaxDurability - blockData.Durability) * 100 / buildingBlockItem.MaxDurability));
				damageText = damageText.SetColor((blockData.Durability < buildingBlockItem.MaxDurability) ? "brightred" : "brightblue").ColorReplace();
				TextMeshProUGUI[] damageHolderTextArray = this.damageHolder.GetComponentsInChildren<TextMeshProUGUI>();
				damageHolderTextArray[0].SetText(LanguageKey.LK_Building_Damage.Tr(), true);
				damageHolderTextArray[1].SetText(damageText, true);
				bool flag3 = buildingBlockItem.BaseMaintenanceCost.Count > 0;
				if (flag3)
				{
					StringBuilder sb = EasyPool.Get<StringBuilder>();
					sb.Append("<SpName=" + CommonUtils.GetResOrExpIcon(buildingBlockItem.BaseMaintenanceCost[0].ResourceType, false) + ">");
					int[] costArray = GameData.Domains.Building.SharedMethods.GetFinalMaintenanceCost(buildingBlockItem);
					int cost = costArray[(int)buildingBlockItem.BaseMaintenanceCost[0].ResourceType];
					string text = string.Format("{0}/{1}", cost, LocalStringManager.Get(LanguageKey.LK_Month));
					sb.Append(text);
					TextMeshProUGUI[] maintenanceTextArray = this.maintenanceHolder.GetComponentsInChildren<TextMeshProUGUI>();
					maintenanceTextArray[0].SetText(LanguageKey.LK_Building_Maintain_Title.Tr(), true);
					TextMeshProUGUI value = maintenanceTextArray[1];
					value.SetText(sb.ToString(), true);
					value.GetComponent<TMPTextSpriteHelper>().Parse();
					EasyPool.Free<StringBuilder>(sb);
				}
				List<short> expandInfos = buildingBlockItem.ExpandInfos;
				bool showScaleEffect = expandInfos != null && expandInfos.Count > 0;
				this.scaleEffectHolder.gameObject.SetActive(showScaleEffect);
				bool flag4 = showScaleEffect;
				if (flag4)
				{
					BuildingDomainMethod.AsyncCall.GetTaiwuVillageBlockEffectInfo(null, blockKey, delegate(int offset, RawDataPool dataPool)
					{
						TaiwuVillageBlockEffectInfo effectInfo = new TaiwuVillageBlockEffectInfo();
						Serializer.Deserialize(dataPool, offset, ref effectInfo);
						this.scaleEffectHolder.Rebuild<RectTransform>(buildingBlockItem.ExpandInfos.Count, delegate(RectTransform rect, int index)
						{
							TextMeshProUGUI[] scaleEffectArray = rect.GetComponentsInChildren<TextMeshProUGUI>();
							TextMeshProUGUI labelName = scaleEffectArray[0];
							TextMeshProUGUI labelValue = scaleEffectArray[1];
							List<int> managers = this.BuildingModel.GetBuildingShopManager(blockKey);
							bool hasLeader = (managers.CheckIndex(0) && managers[0] >= 0) || !buildingBlockItem.NeedLeader;
							BuildingScaleItem buildingScaleItem = BuildingScale.Instance[buildingBlockItem.ExpandInfos[index]];
							labelName.SetText(buildingScaleItem.Name, true);
							this.SetSingleValue(labelValue, hasLeader, (int)buildingLevel, blockKey, buildingScaleItem, buildingBlockItem, effectInfo);
							bool flag7 = buildingScaleItem.TemplateId == 109;
							if (flag7)
							{
								rect.gameObject.SetActive(false);
								ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
								{
									bool isOpen = false;
									Serializer.Deserialize(dataPool, offset, ref isOpen);
									rect.gameObject.SetActive(isOpen);
								});
							}
						});
						bool showTotalEffect = buildingBlockItem.Class == EBuildingBlockClass.BornResource;
						this.totalScaleEffectHolder.gameObject.SetActive(showTotalEffect);
						bool flag6 = showTotalEffect;
						if (flag6)
						{
							List<short> scaleTemplateIdList = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectScaleTemplateIdList(blockData.TemplateId);
							this.totalScaleEffectHolder.Rebuild<RectTransform>(scaleTemplateIdList.Count, delegate(RectTransform rect, int index)
							{
								TextMeshProUGUI[] scaleEffectArray = rect.GetComponentsInChildren<TextMeshProUGUI>();
								TextMeshProUGUI labelName = scaleEffectArray[0];
								TextMeshProUGUI labelValue = scaleEffectArray[1];
								BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleTemplateIdList[index]];
								int sum = this.GetEffectSum(scaleConfig.Formula, effectInfo.LevelList);
								labelName.SetText(scaleConfig.Name, true);
								bool flag7 = scaleConfig.Class == EBuildingScaleClass.MemberResourceIncome;
								if (flag7)
								{
									byte worldResourceType = (scaleConfig.ResourceType < 6) ? 4 : 5;
									CValuePercent percent = (int)GameData.Domains.World.SharedMethods.GetGainResourcePercent(worldResourceType);
									sum *= percent;
								}
								string valueStr = UI_BuildingManage.GetBuildingScaleFormatString(scaleConfig.Type, sum);
								labelValue.SetText(valueStr, true);
							});
						}
					});
				}
				this.shopProgressHolder.gameObject.SetActive(GameData.Domains.Building.SharedMethods.BuildingShowManageProgress(buildingBlockItem));
				bool activeSelf = this.shopProgressHolder.gameObject.activeSelf;
				if (activeSelf)
				{
					BuildingDomainMethod.AsyncCall.GetTaiwuVillageShopData(null, blockKey, delegate(int offset, RawDataPool dataPool)
					{
						BuildingShopData shopData = new BuildingShopData();
						Serializer.Deserialize(dataPool, offset, ref shopData);
						List<int> shopManagerList = this.BuildingModel.GetBuildingShopManager(blockKey);
						int progressDelta = UI_BuildingManage.GetShopManageProgressDelta(shopManagerList, buildingBlockItem, blockData, shopData.Attainment);
						this.shopProgressFill.fillAmount = blockData.ShopProgressFill;
						this.shopProgressText.SetText(UI_BuildingManage.GetPredictProgressText(progressDelta, shopData.ResourceBlockEffect, buildingBlockItem, blockData), true);
					});
				}
				bool flag5 = GameData.Domains.Building.SharedMethods.BuildingGetDisplayData(buildingBlockItem);
				if (flag5)
				{
					BuildingDomainMethod.AsyncCall.GetBuildingEarningDisplayData(this, blockKey, delegate(int offset, RawDataPool dataPool)
					{
						BuildingEarningDisplayData earningData = new BuildingEarningDisplayData();
						Serializer.Deserialize(dataPool, offset, ref earningData);
						StringBuilder sb2 = EasyPool.Get<StringBuilder>();
						bool showResourceList = GameData.Domains.Building.SharedMethods.BuildingShopEventHaveResourceList(buildingBlockItem);
						this.getResourceHolder.transform.parent.gameObject.SetActive(showResourceList);
						bool flag6 = showResourceList;
						if (flag6)
						{
							BuildingEarningDisplayData earningData8 = earningData;
							int? num;
							if (earningData8 == null)
							{
								num = null;
							}
							else
							{
								List<IntPair> collectionResourceList = earningData8.CollectionResourceList;
								num = ((collectionResourceList != null) ? new int?(collectionResourceList.Count) : null);
							}
							int? num2 = num;
							int currCount = num2.GetValueOrDefault();
							sb2.Append(LanguageKey.LK_Building_Production.Tr()).Append("(").Append(currCount).Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(buildingBlockItem.TemplateId)).Append(")");
							this.getResourceTitle.SetText(sb2.ToString(), true);
							this.getResourceHolder.Rebuild<RectTransform>(currCount, delegate(RectTransform rect, int index)
							{
								IntPair data = earningData.CollectionResourceList[index];
								CImage icon = rect.GetComponentInChildren<CImage>();
								TextMeshProUGUI count2 = rect.GetComponentInChildren<TextMeshProUGUI>();
								icon.SetSprite(CommonUtils.GetResOrExpIcon((sbyte)data.First, true), false, null);
								count2.SetText(data.Second.ToString(), true);
							});
						}
						bool showSoldItem = GameData.Domains.Building.SharedMethods.BuildingShopEventHaveSoldItemList(buildingBlockItem);
						this.soldItemHodler.transform.parent.gameObject.SetActive(showSoldItem);
						bool flag7 = showSoldItem;
						if (flag7)
						{
							int currCount2 = this.GetSoldItemSuccessCount(earningData);
							sbyte slotCount = GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(buildingBlockItem.TemplateId);
							sb2.Append(LanguageKey.LK_Building_Production.Tr()).Append("(").Append(currCount2).Append("/").Append(slotCount).Append(")");
							this.soldItemTitle.SetText(sb2.ToString(), true);
							this.autoSoldValue.transform.parent.gameObject.SetActive(true);
							this.autoSoldValue.SetText((earningData != null && earningData.AutoSoldItem) ? LanguageKey.LK_Option_On.Tr() : LanguageKey.LK_Option_Off.Tr(), true);
							this.soldItemHodler.Rebuild<RectTransform>((int)slotCount, delegate(RectTransform rect, int index)
							{
								Transform resource = rect.GetChild(0);
								bool flag13 = earningData != null && earningData.ShopSoldItemEarnList != null;
								if (flag13)
								{
									IntPair earnData = earningData.ShopSoldItemEarnList[index];
									bool haveResource = earnData.First >= 0;
									resource.gameObject.SetActive(haveResource);
									bool flag14 = haveResource;
									if (flag14)
									{
										CImage resourceIcon = resource.GetComponentInChildren<CImage>();
										TextMeshProUGUI resourceCount = resource.GetComponentInChildren<TextMeshProUGUI>();
										resourceIcon.SetSprite(CommonUtils.GetResOrExpIcon((sbyte)earnData.First, true), false, null);
										resourceCount.SetText(earnData.Second.ToString(), true);
									}
								}
								else
								{
									resource.gameObject.SetActive(false);
								}
								Transform item = rect.GetChild(1);
								bool flag15 = earningData != null && earningData.ShopSoldItemDisplayList != null;
								if (flag15)
								{
									ItemDisplayData itemData = earningData.ShopSoldItemDisplayList[index];
									bool haveItem = itemData != null;
									item.gameObject.SetActive(haveItem);
									bool flag16 = haveItem;
									if (flag16)
									{
										ItemBack itemBack = item.GetComponent<ItemBack>();
										itemBack.Set(itemData, false);
									}
								}
								else
								{
									item.gameObject.SetActive(false);
								}
							});
						}
						bool showItemList = GameData.Domains.Building.SharedMethods.BuildingShopEventHaveItemList(buildingBlockItem);
						this.getItemHolder.transform.parent.gameObject.SetActive(showItemList);
						bool flag8 = showItemList;
						if (flag8)
						{
							BuildingEarningDisplayData earningData2 = earningData;
							int? num3;
							if (earningData2 == null)
							{
								num3 = null;
							}
							else
							{
								List<RecruitCharacterData> recruitCharacterDataList = earningData2.RecruitCharacterDataList;
								num3 = ((recruitCharacterDataList != null) ? new int?(recruitCharacterDataList.Count) : null);
							}
							int? num2 = num3;
							int currCount3 = num2.GetValueOrDefault();
							sb2.Append(LanguageKey.LK_Building_Production.Tr()).Append("(").Append(currCount3).Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(buildingBlockItem.TemplateId)).Append(")");
							this.getItemTitle.SetText(sb2.ToString(), true);
							this.getItemHolder.Rebuild<ItemBack>(currCount3, delegate(ItemBack itemBack, int index)
							{
								BuildingEarningDisplayData earningData7 = earningData;
								itemBack.Set((earningData7 != null) ? earningData7.CollectionItemDisplayList[index] : null, false);
							});
						}
						bool showGetPeople = GameData.Domains.Building.SharedMethods.BuildingShopEventRecruitPeople(buildingBlockItem);
						this.getPeopleHolder.transform.parent.gameObject.SetActive(showGetPeople);
						bool flag9 = showGetPeople;
						if (flag9)
						{
							BuildingEarningDisplayData earningData3 = earningData;
							int? num4;
							if (earningData3 == null)
							{
								num4 = null;
							}
							else
							{
								List<RecruitCharacterData> recruitCharacterDataList2 = earningData3.RecruitCharacterDataList;
								num4 = ((recruitCharacterDataList2 != null) ? new int?(recruitCharacterDataList2.Count) : null);
							}
							int? num2 = num4;
							int currCount4 = num2.GetValueOrDefault();
							sb2.Clear();
							sb2.Append(LanguageKey.LK_Building_ShopRecruitPeopleBtn.Tr()).Append("(").Append(currCount4).Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(buildingBlockItem.TemplateId)).Append(")");
							this.getPeopleTitle.SetText(sb2.ToString(), true);
							this.getPeopleHolder.Rebuild<RectTransform>(currCount4, delegate(RectTransform rect, int index)
							{
								BuildingEarningDisplayData earningData7 = earningData;
								RecruitCharacterData recruitCharacterData = (earningData7 != null) ? earningData7.RecruitCharacterDataList[index] : null;
								Game.Components.Avatar.Avatar avatar = rect.GetComponentInChildren<Game.Components.Avatar.Avatar>();
								TextMeshProUGUI characterName = rect.GetComponentInChildren<TextMeshProUGUI>();
								ValueTuple<string, string> name = recruitCharacterData.FullName.GetName(recruitCharacterData.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
								string surname = name.Item1;
								string givenName = name.Item2;
								avatar.Refresh(recruitCharacterData.GenerateAvatarRelatedData(), recruitCharacterData.TemplateId);
								characterName.text = surname + givenName;
							});
						}
						bool hasManager = buildingBlockItem.IsShop;
						this.managerListHodler.transform.parent.gameObject.SetActive(hasManager);
						bool flag10 = hasManager;
						if (flag10)
						{
							this.autoArrange.SetText((earningData != null && earningData.AutoArrange) ? LanguageKey.LK_Option_On.Tr() : LanguageKey.LK_Option_Off.Tr(), true);
							BuildingEarningDisplayData earningData4 = earningData;
							int? num5;
							if (earningData4 == null)
							{
								num5 = null;
							}
							else
							{
								List<BuildingManagerDisplayData> managerDisplayDataList = earningData4.ManagerDisplayDataList;
								num5 = ((managerDisplayDataList != null) ? new int?(managerDisplayDataList.Count) : null);
							}
							int? num2 = num5;
							int count = num2.GetValueOrDefault();
							this.managerListHodler.Rebuild<BuildingManagerTipsInfo>(count, delegate(BuildingManagerTipsInfo info, int index)
							{
								BuildingManagerDisplayData managerDisplayData = earningData.ManagerDisplayDataList[index];
								info.SetManagerInfo(managerDisplayData, index);
							});
						}
						bool flag11 = buildingBlockItem.TemplateId == 47;
						if (flag11)
						{
							this.getPeopleHolder.transform.parent.gameObject.SetActive(true);
							this.soldItemHodler.transform.parent.gameObject.SetActive(true);
							BuildingEarningDisplayData earningData5 = earningData;
							int? num6;
							if (earningData5 == null)
							{
								num6 = null;
							}
							else
							{
								List<CharacterDisplayData> comfortableHouses = earningData5.ComfortableHouses;
								num6 = ((comfortableHouses != null) ? new int?(comfortableHouses.Count) : null);
							}
							int? num2 = num6;
							int currCount5 = num2.GetValueOrDefault();
							sb2.Clear();
							sb2.Append(LanguageKey.Lk_Building_Feast_Character_Title.Tr()).Append("(").Append(currCount5).Append("/").Append(GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(buildingBlockItem.TemplateId)).Append(")");
							this.getPeopleTitle.SetText(sb2.ToString(), true);
							this.getPeopleHolder.Rebuild<RectTransform>(currCount5, delegate(RectTransform rect, int index)
							{
								BuildingEarningDisplayData earningData7 = earningData;
								CharacterDisplayData characterDisplay = (earningData7 != null) ? earningData7.ComfortableHouses[index] : null;
								Game.Components.Avatar.Avatar avatar = rect.GetComponentInChildren<Game.Components.Avatar.Avatar>();
								TextMeshProUGUI characterName = rect.GetComponentInChildren<TextMeshProUGUI>();
								ValueTuple<string, string> name = characterDisplay.FullName.GetName(characterDisplay.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
								string surname = name.Item1;
								string givenName = name.Item2;
								avatar.Refresh(characterDisplay, true);
								characterName.text = surname + givenName;
							});
							ValueTuple<int, int> feastGiftCount = this.GetFeastGiftCount(earningData);
							int resourceCount = feastGiftCount.Item1;
							int itemCount = feastGiftCount.Item2;
							int totalCount = resourceCount + itemCount;
							sb2.Clear();
							sb2.Append(LanguageKey.LK_Building_Tab_Reward.Tr()).Append("(").Append(totalCount).Append("/").Append(GlobalConfig.Instance.FeastGiftCount).Append(")");
							this.soldItemTitle.SetText(sb2.ToString(), true);
							this.autoSoldValue.transform.parent.gameObject.SetActive(false);
							this.soldItemHodler.Rebuild<RectTransform>(totalCount, delegate(RectTransform rect, int index)
							{
								TextMeshProUGUI resourceCount;
								bool flag13 = index < resourceCount;
								if (flag13)
								{
									Transform resource = rect.GetChild(0);
									Transform item = rect.GetChild(1);
									resource.gameObject.SetActive(true);
									item.gameObject.SetActive(false);
									IntPair resourceData = earningData.CollectionResourceList[index];
									CImage resourceIcon = resource.GetComponentInChildren<CImage>();
									resourceCount = resource.GetComponentInChildren<TextMeshProUGUI>();
									resourceIcon.SetSprite(CommonUtils.GetResOrExpIcon((sbyte)resourceData.First, true), false, null);
									resourceCount.SetText(resourceData.Second.ToString(), true);
								}
								bool flag14 = index >= resourceCount && index < totalCount;
								if (flag14)
								{
									Transform resource2 = rect.GetChild(0);
									Transform item2 = rect.GetChild(1);
									resource2.gameObject.SetActive(false);
									item2.gameObject.SetActive(true);
									ItemDisplayData itemData = earningData.CollectionItemDisplayList[index - resourceCount];
									ItemBack itemBack = item2.GetComponent<ItemBack>();
									itemBack.Set(itemData, false);
								}
							});
						}
						bool flag12 = buildingBlockItem.TemplateId == 46;
						if (flag12)
						{
							this.getPeopleHolder.transform.parent.gameObject.SetActive(true);
							BuildingEarningDisplayData earningData6 = earningData;
							int? num7;
							if (earningData6 == null)
							{
								num7 = null;
							}
							else
							{
								List<CharacterDisplayData> residences = earningData6.Residences;
								num7 = ((residences != null) ? new int?(residences.Count) : null);
							}
							int? num2 = num7;
							int currCount6 = num2.GetValueOrDefault();
							sb2.Clear();
							sb2.Append(LanguageKey.LK_Building_ResidentCount.Tr()).Append("(").Append(currCount6).Append("/").Append(BuildingScale.DefValue.ResidenceCapacity.GetLevelEffect((int)buildingLevel)).Append(")");
							this.getPeopleTitle.SetText(sb2.ToString(), true);
							this.getPeopleHolder.Rebuild<RectTransform>(0, null);
						}
						EasyPool.Free<StringBuilder>(sb2);
					});
				}
			}
		}

		// Token: 0x060066AF RID: 26287 RVA: 0x002ED45C File Offset: 0x002EB65C
		private int GetSoldItemSuccessCount(BuildingEarningDisplayData displayData)
		{
			bool flag = ((displayData != null) ? displayData.ShopSoldItemEarnList : null) == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				for (int i = 0; i < displayData.ShopSoldItemEarnList.Count; i++)
				{
					bool flag2 = displayData.ShopSoldItemEarnList[i].First >= 0;
					if (flag2)
					{
						count++;
					}
				}
				result = count;
			}
			return result;
		}

		// Token: 0x060066B0 RID: 26288 RVA: 0x002ED4C8 File Offset: 0x002EB6C8
		[return: TupleElementNames(new string[]
		{
			"resourceCount",
			"itemCount"
		})]
		private ValueTuple<int, int> GetFeastGiftCount(BuildingEarningDisplayData displayData)
		{
			List<IntPair> collectionResourceList = displayData.CollectionResourceList;
			int item = (collectionResourceList != null) ? collectionResourceList.Count : 0;
			List<ItemDisplayData> collectionItemDisplayList = displayData.CollectionItemDisplayList;
			return new ValueTuple<int, int>(item, (collectionItemDisplayList != null) ? collectionItemDisplayList.Count : 0);
		}

		// Token: 0x060066B1 RID: 26289 RVA: 0x002ED504 File Offset: 0x002EB704
		private int GetEffectSum(int formulaTemplateId, List<int> levelList)
		{
			int[] baseValue = new int[Math.Min(levelList.Count, 5)];
			for (int index = 0; index < baseValue.Length; index++)
			{
				int level = levelList[index];
				int percentage = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(index);
				int effectValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue(level, percentage);
				baseValue[index] = effectValue;
			}
			return GameData.Domains.Building.SharedMethods.CalcResourceBlockTotalEffectValue(formulaTemplateId, baseValue);
		}

		// Token: 0x060066B2 RID: 26290 RVA: 0x002ED570 File Offset: 0x002EB770
		private void SetSingleValue(TextMeshProUGUI labelValue, bool hasLeader, int buildingLevel, BuildingBlockKey blockKey, BuildingScaleItem buildingScaleItem, BuildingBlockItem buildingBlockItem, TaiwuVillageBlockEffectInfo effectInfo)
		{
			bool flag = !hasLeader;
			int value;
			if (flag)
			{
				value = 0;
			}
			else
			{
				bool flag2 = buildingScaleItem.Formula >= 0;
				if (flag2)
				{
					bool flag3 = buildingBlockItem.Class == EBuildingBlockClass.BornResource;
					if (flag3)
					{
						bool flag4 = effectInfo.BlockRanking < 5;
						if (flag4)
						{
							int percentage = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(effectInfo.BlockRanking);
							int baseValue = GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentageValue(buildingLevel, percentage);
							value = BuildingFormula.Instance[buildingScaleItem.Formula].Calculate(baseValue);
						}
						else
						{
							value = 0;
						}
					}
					else
					{
						bool flag5 = effectInfo.FormulaContextBridge != null && effectInfo.FormulaContextBridge.BlockKey.Equals(blockKey);
						if (flag5)
						{
							value = BuildingFormula.Instance[buildingScaleItem.Formula].Calculate(effectInfo.FormulaContextBridge);
						}
						else
						{
							value = 0;
						}
					}
				}
				else
				{
					int levelEffectIndex = buildingLevel - 1;
					value = ((buildingScaleItem.LevelEffect != null && buildingScaleItem.LevelEffect.CheckIndex(levelEffectIndex)) ? buildingScaleItem.LevelEffect[levelEffectIndex] : 0);
				}
			}
			bool flag6 = !hasLeader && buildingBlockItem.IsShop;
			if (flag6)
			{
				labelValue.text = "-";
			}
			else
			{
				labelValue.text = UI_BuildingManage.GetBuildingScaleFormatString(buildingScaleItem.Type, value);
				bool flag7 = buildingScaleItem.Type == EBuildingScaleType.Maintaince;
				if (flag7)
				{
					labelValue.text = string.Format("{0}", value).SetColor("brightred");
				}
			}
		}

		// Token: 0x060066B3 RID: 26291 RVA: 0x002ED6E8 File Offset: 0x002EB8E8
		private void Update()
		{
			bool simpleMode = this._simpleMode;
			if (!simpleMode)
			{
				bool hasStick = this.HasStick;
				if (!hasStick)
				{
					bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
					this.funcDescHolder.gameObject.SetActive(altDown && this._hasFunctionDesc);
					this.extraHolder.gameObject.SetActive(altDown && this._hasManagerList);
					bool flag = altDown;
					if (flag)
					{
						this.moreInfo.RefreshCancelDetail();
					}
					else
					{
						this.moreInfo.RefreshPressToDetail();
					}
				}
			}
		}

		// Token: 0x060066B4 RID: 26292 RVA: 0x002ED781 File Offset: 0x002EB981
		public override void Refresh(ArgumentBox argBox)
		{
			this.Init(argBox);
		}

		// Token: 0x040047EF RID: 18415
		[SerializeField]
		private CImage buildingIcon;

		// Token: 0x040047F0 RID: 18416
		[SerializeField]
		private CImage shopProgressFill;

		// Token: 0x040047F1 RID: 18417
		[SerializeField]
		private TextMeshProUGUI buildingName;

		// Token: 0x040047F2 RID: 18418
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x040047F3 RID: 18419
		[SerializeField]
		private TextMeshProUGUI funcDesc;

		// Token: 0x040047F4 RID: 18420
		[SerializeField]
		private TextMeshProUGUI shopProgressText;

		// Token: 0x040047F5 RID: 18421
		[SerializeField]
		private TextMeshProUGUI getItemTitle;

		// Token: 0x040047F6 RID: 18422
		[SerializeField]
		private TextMeshProUGUI getPeopleTitle;

		// Token: 0x040047F7 RID: 18423
		[SerializeField]
		private TextMeshProUGUI getResourceTitle;

		// Token: 0x040047F8 RID: 18424
		[SerializeField]
		private TextMeshProUGUI soldItemTitle;

		// Token: 0x040047F9 RID: 18425
		[SerializeField]
		private TextMeshProUGUI autoSoldValue;

		// Token: 0x040047FA RID: 18426
		[SerializeField]
		private TextMeshProUGUI autoArrange;

		// Token: 0x040047FB RID: 18427
		[SerializeField]
		private GameObject funcDescHolder;

		// Token: 0x040047FC RID: 18428
		[SerializeField]
		private GameObject baseInfoHolder;

		// Token: 0x040047FD RID: 18429
		[SerializeField]
		private GameObject levelHolder;

		// Token: 0x040047FE RID: 18430
		[SerializeField]
		private GameObject maintenanceHolder;

		// Token: 0x040047FF RID: 18431
		[SerializeField]
		private GameObject damageHolder;

		// Token: 0x04004800 RID: 18432
		[SerializeField]
		private GameObject shopProgressHolder;

		// Token: 0x04004801 RID: 18433
		[SerializeField]
		private GameObject extraHolder;

		// Token: 0x04004802 RID: 18434
		[SerializeField]
		private TemplatedContainerAssemblyNew scaleEffectHolder;

		// Token: 0x04004803 RID: 18435
		[SerializeField]
		private TemplatedContainerAssemblyNew totalScaleEffectHolder;

		// Token: 0x04004804 RID: 18436
		[SerializeField]
		private TemplatedContainerAssemblyNew getItemHolder;

		// Token: 0x04004805 RID: 18437
		[SerializeField]
		private TemplatedContainerAssemblyNew getPeopleHolder;

		// Token: 0x04004806 RID: 18438
		[SerializeField]
		private TemplatedContainerAssemblyNew getResourceHolder;

		// Token: 0x04004807 RID: 18439
		[SerializeField]
		private TemplatedContainerAssemblyNew soldItemHodler;

		// Token: 0x04004808 RID: 18440
		[SerializeField]
		private TemplatedContainerAssemblyNew managerListHodler;

		// Token: 0x04004809 RID: 18441
		[SerializeField]
		private MoreInfo2 moreInfo;

		// Token: 0x0400480A RID: 18442
		private bool _hasFunctionDesc;

		// Token: 0x0400480B RID: 18443
		private bool _hasManagerList;

		// Token: 0x0400480C RID: 18444
		private bool _simpleMode;
	}
}
