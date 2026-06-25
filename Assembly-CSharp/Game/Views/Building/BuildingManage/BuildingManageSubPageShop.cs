using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Building.RecordBase;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using Game.Views.VillagerRoleView;
using GameData.Combat.Math;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C02 RID: 3074
	public class BuildingManageSubPageShop : BuildingManageSubPage
	{
		// Token: 0x17001080 RID: 4224
		// (get) Token: 0x06009C33 RID: 39987 RVA: 0x00492831 File Offset: 0x00490A31
		private List<int> ShopManagerList
		{
			get
			{
				return this.DisplayData.ShopManagerList;
			}
		}

		// Token: 0x17001081 RID: 4225
		// (get) Token: 0x06009C34 RID: 39988 RVA: 0x0049283E File Offset: 0x00490A3E
		private BuildingManageYieldTipsData TipsData
		{
			get
			{
				return this.DisplayData.TipsData;
			}
		}

		// Token: 0x17001082 RID: 4226
		// (get) Token: 0x06009C35 RID: 39989 RVA: 0x0049284B File Offset: 0x00490A4B
		private ItemDisplayData FixingBookItemData
		{
			get
			{
				return this.DisplayData.FixingBookItemData;
			}
		}

		// Token: 0x17001083 RID: 4227
		// (get) Token: 0x06009C36 RID: 39990 RVA: 0x00492858 File Offset: 0x00490A58
		private SkillBookPageDisplayData SkillBookPageDisplayData
		{
			get
			{
				return this.DisplayData.SkillBookPageDisplayData;
			}
		}

		// Token: 0x17001084 RID: 4228
		// (get) Token: 0x06009C37 RID: 39991 RVA: 0x00492865 File Offset: 0x00490A65
		private CValuePercentBonus ShopProgressBonus
		{
			get
			{
				return this.DisplayData.TaiwuVillageResourceBlockEffect;
			}
		}

		// Token: 0x17001085 RID: 4229
		// (get) Token: 0x06009C38 RID: 39992 RVA: 0x00492877 File Offset: 0x00490A77
		public BuildingModel BuildingModel
		{
			get
			{
				return this._buildingModel;
			}
		}

		// Token: 0x17001086 RID: 4230
		// (get) Token: 0x06009C39 RID: 39993 RVA: 0x0049287F File Offset: 0x00490A7F
		public BuildingBlockKey BlockKey
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x17001087 RID: 4231
		// (get) Token: 0x06009C3A RID: 39994 RVA: 0x0049288C File Offset: 0x00490A8C
		public BuildingBlockData BlockData
		{
			get
			{
				return this.ParentView.BlockData;
			}
		}

		// Token: 0x17001088 RID: 4232
		// (get) Token: 0x06009C3B RID: 39995 RVA: 0x00492899 File Offset: 0x00490A99
		private BuildingBlockItem ConfigData
		{
			get
			{
				return this.ParentView.ConfigData;
			}
		}

		// Token: 0x17001089 RID: 4233
		// (get) Token: 0x06009C3C RID: 39996 RVA: 0x004928A6 File Offset: 0x00490AA6
		private ShopEventItem ShopEventData
		{
			get
			{
				return ViewBuildingManage.GetShopEventConfig(this.ConfigData.TemplateId);
			}
		}

		// Token: 0x1700108A RID: 4234
		// (get) Token: 0x06009C3D RID: 39997 RVA: 0x004928B8 File Offset: 0x00490AB8
		private int ShopManageProgressDelta
		{
			get
			{
				return (this.ShopManagerList[0] > -1 || !this.ConfigData.NeedLeader) ? GameData.Domains.Building.SharedMethods.GetShopManageProgressDelta(this.BlockData.TemplateId, this.DisplayData.BuildingAttainment) : 0;
			}
		}

		// Token: 0x06009C3E RID: 39998 RVA: 0x004928F4 File Offset: 0x00490AF4
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			this._buildingModel = SingletonObject.getInstance<BuildingModel>();
			this.buttonShopQuickSelect.ClearAndAddListener(new Action(this.OnClickButtonShopQuickSelect));
			this.buttonShopQuickCancel.ClearAndAddListener(new Action(this.OnClickButtonShopQuickCancel));
			this.buttonVillagerManager.ClearAndAddListener(new Action(this.OnClickButtonVillagerManager));
			this.buttonArrangementSetting.ClearAndAddListener(new Action(this.arrangementSettingPanel.Show));
		}

		// Token: 0x06009C3F RID: 39999 RVA: 0x0049297A File Offset: 0x00490B7A
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this.UpdateShopManageTitle();
			this.UpdateShopManagers();
			this.UpdateShopManageInfo();
			this.arrangementSettingPanel.Refresh();
			this.UpdateAutoWork();
			this.UpdateRecord();
		}

		// Token: 0x06009C40 RID: 40000 RVA: 0x004929B4 File Offset: 0x00490BB4
		public override bool QuickHide()
		{
			bool activeSelf = this.arrangementSettingPanel.gameObject.activeSelf;
			bool result;
			if (activeSelf)
			{
				this.arrangementSettingPanel.Hide();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06009C41 RID: 40001 RVA: 0x004929EC File Offset: 0x00490BEC
		private void UpdateShopManageTitle()
		{
			ValueTuple<string, string> buildingRequireIconAndName = BuildingManageSubPageShop.GetBuildingRequireIconAndName(this.ParentView.BlockData.TemplateId);
			string skillIcon = buildingRequireIconAndName.Item1;
			string skillName = buildingRequireIconAndName.Item2;
			this.imageSkillIcon.SetSprite(skillIcon, false, null);
			this.textSkillName.SetText(skillName, true);
			this._stringBuilder.Clear();
			this._stringBuilder.Append(this.ParentView.ConfigData.LeaderName).Append("(").Append(this.GetLeaderCount()).Append("/").Append(1).Append(")");
			this.textManagerLeaderTitle.text = this._stringBuilder.ToString();
			this._stringBuilder.Clear();
			this._stringBuilder.Append(this.ParentView.ConfigData.MemberName).Append("(").Append(this.GetMemberCount()).Append("/").Append(6).Append(")");
			this.textManagerMemberTitle.text = this._stringBuilder.ToString();
		}

		// Token: 0x06009C42 RID: 40002 RVA: 0x00492B18 File Offset: 0x00490D18
		private void UpdateShopManagers()
		{
			for (int i = 0; i < 7; i++)
			{
				int charId = this.ShopManagerList[i];
				bool flag = i == 0;
				if (flag)
				{
					bool hasTaiwuShrine = this.ParentView.HasBuilding(45, true);
					List<VillagerRoleCharacterDisplayData> villagerRoleDataList = this.DisplayData.VillagerRoleDataList;
					VillagerRoleCharacterDisplayData villagerRoleData = (villagerRoleDataList != null) ? villagerRoleDataList.GetOrDefault(i) : null;
					List<CharacterDisplayData> characterDataList = this.DisplayData.CharacterDataList;
					CharacterDisplayData charData = (characterDataList != null) ? characterDataList.GetOrDefault(i) : null;
					List<int> villagerEfficiencyList = this.DisplayData.VillagerEfficiencyList;
					int efficiency = (villagerEfficiencyList != null) ? villagerEfficiencyList.GetOrDefault(i) : 0;
					List<int> unlockedWorkingVillagerList = this.DisplayData.UnlockedWorkingVillagerList;
					bool isUnlocked = unlockedWorkingVillagerList != null && unlockedWorkingVillagerList.Contains(charId);
					this.managerLeaderView.Refresh(i, charId, this.BlockData, villagerRoleData, charData, efficiency, hasTaiwuShrine, isUnlocked, new Action<int>(this.OpenSelectChar), new Action<int>(this.CancelShopManager), new Action<int, bool>(this.SetUnlockedWorkingVillager), new Action<bool>(this.OnAssignRole));
				}
				else
				{
					int memberIndex = i - 1;
					List<VillagerRoleCharacterDisplayData> villagerRoleDataList2 = this.DisplayData.VillagerRoleDataList;
					VillagerRoleCharacterDisplayData villagerRoleData2 = (villagerRoleDataList2 != null) ? villagerRoleDataList2.GetOrDefault(i) : null;
					List<CharacterDisplayData> characterDataList2 = this.DisplayData.CharacterDataList;
					CharacterDisplayData charData2 = (characterDataList2 != null) ? characterDataList2.GetOrDefault(i) : null;
					List<ShopBuildingTeachBookData> teachBookDataList = this.DisplayData.TeachBookDataList;
					ShopBuildingTeachBookData teachData = (teachBookDataList != null) ? teachBookDataList.GetOrDefault(i) : null;
					List<int> villagerEfficiencyList2 = this.DisplayData.VillagerEfficiencyList;
					int efficiency2 = (villagerEfficiencyList2 != null) ? villagerEfficiencyList2.GetOrDefault(i) : 0;
					List<int> unlockedWorkingVillagerList2 = this.DisplayData.UnlockedWorkingVillagerList;
					bool isUnlocked2 = unlockedWorkingVillagerList2 != null && unlockedWorkingVillagerList2.Contains(charId);
					Dictionary<int, int> shopManagerUpgradeQualificationDict = this.DisplayData.ShopManagerUpgradeQualificationDict;
					int upgradeQualification = (shopManagerUpgradeQualificationDict != null) ? shopManagerUpgradeQualificationDict.GetOrDefault(charId) : 0;
					this.managerMemberViewArray[memberIndex].Refresh(i, charId, this.BlockData, villagerRoleData2, charData2, teachData, upgradeQualification, efficiency2, isUnlocked2, new Action<int>(this.OpenSelectChar), new Action<int>(this.CancelShopManager), new Action<int, bool>(this.SetUnlockedWorkingVillager));
				}
			}
		}

		// Token: 0x06009C43 RID: 40003 RVA: 0x00492D10 File Offset: 0x00490F10
		private void SetProduceProgressInfo(TooltipInvoker tips)
		{
			tips.Type = TipType.GeneralLines;
			GeneralLineData desc = new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_ManageProduceValue_Tips_Text)
				}
			};
			GeneralLineData title = new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Building_ManageProduceValue_Tips_Title))
				}
			};
			GeneralLineData contentProgress = new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Building_ManageProduceValue_Tips_ContentNormal, this.ShopManageProgressDelta)
				},
				ExtraArgs = new List<object>
				{
					20
				}
			};
			GeneralLineData contentEffect = new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Building_ManageProduceValue_Tips_ContentExp, this.ShopManageProgressEffectDelta())
				},
				ExtraArgs = new List<object>
				{
					20
				}
			};
			int lineCount = 3;
			if (tips.RuntimeParam == null)
			{
				tips.RuntimeParam = new ArgumentBox();
			}
			tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Building_ManageProduceValue_Tips_Title)).SetObject("LineData1", desc).SetObject("LineData2", title).SetObject("LineData3", contentProgress);
			bool flag = this.ShopProgressBonus > 0;
			if (flag)
			{
				lineCount++;
				tips.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), contentEffect);
			}
			ShopEventItem shopEvent = null;
			bool flag2 = this.ConfigData.SuccesEvent.CheckIndex(0);
			if (flag2)
			{
				shopEvent = ShopEvent.Instance.GetItem(this.ConfigData.SuccesEvent[0]);
			}
			bool flag3 = shopEvent != null && (GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this.ConfigData, shopEvent) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(this.ConfigData, shopEvent) || shopEvent.RecruitPeopleProb.Count != 0);
			if (flag3)
			{
				int rate2 = this.DisplayData.SuccessRates[1];
				bool giveRate2 = false;
				LanguageKey languageKey = LanguageKey.LK_Building_ManageProduceValue_Tips_Text_10;
				short templateId = this.ConfigData.TemplateId;
				short num = templateId;
				if (num != 215)
				{
					if (num == 216)
					{
						giveRate2 = true;
						languageKey = LanguageKey.LK_Building_ManageProduceValue_Tips_Text_11;
					}
				}
				else
				{
					giveRate2 = true;
					languageKey = LanguageKey.LK_Building_ManageProduceValue_Tips_Text_10;
				}
				bool flag4 = rate2 >= 0 && giveRate2;
				if (flag4)
				{
					GeneralLineData gamblingHouseOrBrothel = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(languageKey)
						}
					};
					GeneralLineData gamblingHouseOrBrothelValue = new GeneralLineData
					{
						Type = 5,
						Args = new List<string>
						{
							LocalStringManager.GetFormat(LanguageKey.LK_Building_ManageProduceValue_Tips_Text_9, rate2, (rate2 < 100) ? "pinkyellow" : "lightblue")
						}
					};
					lineCount++;
					tips.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), gamblingHouseOrBrothel);
					lineCount++;
					tips.RuntimeParam.SetObject(string.Format("LineData{0}", lineCount), gamblingHouseOrBrothelValue);
				}
				tips.RuntimeParam.Set("LineCount", lineCount);
			}
			else
			{
				tips.RuntimeParam.Set("LineCount", lineCount);
			}
		}

		// Token: 0x06009C44 RID: 40004 RVA: 0x00493074 File Offset: 0x00491274
		private int ShopManageProgressEffectDelta()
		{
			int baseDelta = this.ShopManageProgressDelta;
			return baseDelta * this.ShopProgressBonus - baseDelta;
		}

		// Token: 0x06009C45 RID: 40005 RVA: 0x004930A0 File Offset: 0x004912A0
		private void UpdateShopManageInfo()
		{
			bool show = this.IsFixBookManage() || this.IsNormalManage();
			this.rootProgressArea.SetActive(show);
			bool flag = show;
			if (flag)
			{
				this.UpdateNormalManage();
				this.UpdateFixBookManage();
			}
		}

		// Token: 0x06009C46 RID: 40006 RVA: 0x004930E4 File Offset: 0x004912E4
		private void UpdateNormalManage()
		{
			bool isNormalManage = this.IsNormalManage();
			this.rootNormal.gameObject.SetActive(isNormalManage);
			bool flag = !isNormalManage;
			if (!flag)
			{
				bool showProgress = this.ConfigData.MaxProduceValue > 0;
				this.rootNormalProgress.gameObject.SetActive(showProgress);
				bool flag2 = showProgress;
				if (flag2)
				{
					this.imageNormalProgress.fillAmount = this.BlockData.ShopProgressFill;
					int cur;
					int delta;
					int max;
					this.textNormalProgress.text = this.GetPredictProgressText(out cur, out delta, out max);
					this.imageNormalProgressIncrease.fillAmount = (float)(cur + delta) / (float)max;
					this.SetProduceProgressInfo(this.tipNormalProgress);
				}
				CImage requiredIcon = this.imageRequire;
				bool showRequire = GameData.Domains.Building.SharedMethods.BuildingRequireSafetyOrCulture(this.ConfigData);
				requiredIcon.gameObject.SetActive(showRequire);
				bool showProduct = this.GetShopManagerCount() > 0 && (this.ConfigData.IsCollectResourceBuilding || GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this.ConfigData, this.ShopEventData) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(this.ConfigData, this.ShopEventData));
				this.textProduct.gameObject.SetActive(showProduct);
				this.rootIcon.SetActive(showRequire || showProduct);
				TooltipInvoker requireMouseTip = this.tipRequire;
				bool mouseTipDisable = this.GetShopManagerCount() == 0;
				this.tipProduct.enabled = !mouseTipDisable;
				requireMouseTip.enabled = !mouseTipDisable;
				bool flag3 = mouseTipDisable;
				if (!flag3)
				{
					bool flag4 = showProduct;
					if (flag4)
					{
						bool flag5 = GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this.ConfigData, this.ShopEventData) || GameData.Domains.Building.SharedMethods.IsBuildingSoldItem(this.ConfigData, this.ShopEventData);
						if (flag5)
						{
							this.tipProduct.Type = TipType.BuildingProduce;
							this.tipProduct.RuntimeParam = new ArgumentBox().SetObject("ProduceData", this.TipsData);
							this.textProduct.text = MouseTipBuildingProduce.CalcProduct(this.TipsData, GameData.Domains.Building.SharedMethods.IsBuildingProduceMoneyAuthority(this.ConfigData, this.ShopEventData));
						}
						else
						{
							bool isCollectResourceBuilding = this.ConfigData.IsCollectResourceBuilding;
							if (isCollectResourceBuilding)
							{
								this.tipProduct.Type = TipType.BuildingProduceCollectResource;
								this.tipProduct.RuntimeParam = new ArgumentBox().SetObject("ProduceData", this.TipsData);
								this.textProduct.text = this.TipsData.ResourceOutputValuation.ToString();
							}
						}
					}
					bool activeSelf = requiredIcon.gameObject.activeSelf;
					if (activeSelf)
					{
						this.UpdateRequiredTip(requiredIcon, this.TipsData);
					}
				}
			}
		}

		// Token: 0x06009C47 RID: 40007 RVA: 0x00493378 File Offset: 0x00491578
		private void UpdateRequiredTip(CImage requiredIcon, BuildingManageYieldTipsData tipsData)
		{
			TooltipInvoker requiredTips = requiredIcon.GetComponent<TooltipInvoker>();
			string postfix;
			bool flag;
			bool flag2;
			MouseTipBuildingRequireCultureSafety.CalcIconName(this.ConfigData, out postfix, out flag, out flag2);
			requiredIcon.SetSprite("building_effect_" + postfix, false, null);
			bool flag3 = tipsData.SafetyOrCultureFactorSettlementsAndPickValue == null || tipsData.SafetyOrCultureFactorSettlementsAndPickValue.Count == 0;
			if (flag3)
			{
				requiredTips.Type = TipType.SingleDesc;
				requiredTips.RuntimeParam = new ArgumentBox();
				requiredTips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Building_Effect_Tips_Text_5));
			}
			else
			{
				requiredTips.Type = TipType.BuildingRequireCultureSafety;
				requiredTips.RuntimeParam = new ArgumentBox();
				requiredTips.RuntimeParam.Set("TemplateId", this.ConfigData.TemplateId);
				requiredTips.RuntimeParam.Set<BuildingManageYieldTipsData>("ProduceData", tipsData);
			}
		}

		// Token: 0x06009C48 RID: 40008 RVA: 0x00493448 File Offset: 0x00491648
		private void UpdateFixBookManage()
		{
			bool isFixBookManage = this.IsFixBookManage();
			this.rootFixBook.SetActive(isFixBookManage);
			bool flag = !isFixBookManage;
			if (!flag)
			{
				bool flag2 = this.FixingBookItemData != null;
				if (flag2)
				{
					bool canFix = this.SkillBookPageDisplayData.CanFix();
					int needProgress = (int)this.SkillBookPageDisplayData.GetFixProgress().Item2;
					SingletonObject.getInstance<BasicGameData>().ChallengeModeData.ApplyChallengeModeBuildingWorkHard(ref needProgress);
					int curProgress = Math.Min(needProgress, (int)this.BlockData.ShopProgress);
					this.rootBookProgress.gameObject.SetActive(canFix);
					this.imageBookProgress.fillAmount = (float)curProgress / (float)needProgress;
					this.imageBookProgressIncrease.fillAmount = (float)(curProgress + this.ShopManageProgressDelta) / (float)needProgress;
					this.rootBookInfo.gameObject.SetActive(canFix);
					this.rootProgressState.SetActive(!canFix);
					bool flag3 = canFix;
					if (flag3)
					{
						this.curBookPageInfoCell.SetData(new BookPageInfoData(this.FixingBookItemData));
						sbyte[] bookPageStates = new sbyte[this.FixingBookItemData.BookPageStates.Length];
						for (int i = 0; i < bookPageStates.Length; i++)
						{
							bookPageStates[i] = this.FixingBookItemData.BookPageStates[i];
						}
						for (int j = 0; j < bookPageStates.Length; j++)
						{
							bool flag4 = bookPageStates[j] != 0;
							if (flag4)
							{
								bookPageStates[j] = 0;
								break;
							}
						}
						this.newBookPageInfoCell.SetData(bookPageStates);
						this.textBookProgress.text = ViewBuildingManage.GetPredictProgressText(curProgress, this.ShopManageProgressDelta, needProgress);
						this.buttonFixBook.SetFixBookFunc(this.FixingBookItemData, ItemResourceButton.ItemResourceButtonState.Change, null, delegate
						{
							DialogCmd cmd = new DialogCmd
							{
								Title = LocalStringManager.Get(LanguageKey.LK_Building_ChangeBook),
								Content = LocalStringManager.Get(LanguageKey.LK_Building_ChangeBookTip),
								Yes = new Action(this.OpenMultiSelectItemWindow)
							};
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
						}, null);
						this.textItemName.text = this.FixingBookItemData.GetName(true);
					}
					else
					{
						this.textProgressState.SetText(LocalStringManager.Get(LanguageKey.LK_Building_AccomplishFix), true);
						this.buttonFixBook.SetFixBookFunc(this.FixingBookItemData, ItemResourceButton.ItemResourceButtonState.Reveive, null, null, delegate
						{
							bool isTaiwuOnSettlement = SingletonObject.getInstance<WorldMapModel>().IsTaiwuOnSettlement;
							BuildingDomainMethod.Call.ReceiveFixBook(this.BlockKey, isTaiwuOnSettlement);
							this.ParentView.RequestData();
						});
					}
				}
				else
				{
					this.rootBookProgress.gameObject.SetActive(false);
					this.rootBookInfo.gameObject.SetActive(false);
					this.rootProgressState.SetActive(true);
					this.textProgressState.SetText(LocalStringManager.Get(LanguageKey.LK_Building_FixBook_None), true);
					this.buttonFixBook.SetFixBookFunc(null, ItemResourceButton.ItemResourceButtonState.Add, new Action(this.OpenMultiSelectItemWindow), null, null);
				}
			}
		}

		// Token: 0x06009C49 RID: 40009 RVA: 0x004936CA File Offset: 0x004918CA
		private void UpdateAutoWork()
		{
			BuildingDomainMethod.AsyncCall.GetBuildingIsAutoWork(null, this.BlockKey.BuildingBlockIndex, delegate(int offset, RawDataPool dataPool)
			{
				bool isAutoArrange = false;
				Serializer.Deserialize(dataPool, offset, ref isAutoArrange);
				this.toggleAutoArrange.onValueChanged.RemoveAllListeners();
				this.toggleAutoArrange.isOn = isAutoArrange;
				this.toggleAutoArrange.onValueChanged.AddListener(delegate(bool isOn)
				{
					BuildingDomainMethod.Call.SetBuildingAutoWork(this.BlockKey.BuildingBlockIndex, isOn);
				});
			});
		}

		// Token: 0x06009C4A RID: 40010 RVA: 0x004936EB File Offset: 0x004918EB
		private void OnClickButtonShopQuickSelect()
		{
			BuildingDomainMethod.AsyncCall.QuickArrangeShopManager(null, this.BlockKey, null);
			this.ParentView.RequestData();
		}

		// Token: 0x06009C4B RID: 40011 RVA: 0x00493708 File Offset: 0x00491908
		private void OnClickButtonShopQuickCancel()
		{
			for (sbyte i = 0; i < 7; i += 1)
			{
				BuildingDomainMethod.Call.SetShopManager(this.BlockKey, i, -1);
			}
			this.ParentView.RequestData();
		}

		// Token: 0x06009C4C RID: 40012 RVA: 0x00493744 File Offset: 0x00491944
		private void OnClickButtonVillagerManager()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
			argBox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleDescription);
			argBox.Set("RoleTemplateId", this.ConfigData.VillagerRoleTemplateIds[0]);
			UIElement.VillagerRole.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
		}

		// Token: 0x06009C4D RID: 40013 RVA: 0x004937B4 File Offset: 0x004919B4
		private void OpenSelectChar(int index)
		{
			this._selectingShopManagerIndex = index;
			List<int> charIdListTemp = new List<int>();
			List<int> availableWorker2 = this.DisplayData.AvailableWorker;
			List<int> availableWorker = (availableWorker2 != null) ? (from id in availableWorker2
			where !this._shopManagerListCached.Contains(id)
			select id).ToList<int>() : null;
			bool flag = availableWorker != null && availableWorker.Count > 0;
			if (flag)
			{
				charIdListTemp.AddRange(availableWorker);
			}
			bool flag2 = this._selectingShopManagerIndex != 0;
			if (flag2)
			{
				List<int> availableChildren2 = this.DisplayData.AvailableChildren;
				List<int> availableChildren = (availableChildren2 != null) ? (from id in availableChildren2
				where !this._shopManagerListCached.Contains(id)
				select id).ToList<int>() : null;
				bool flag3 = availableChildren != null && availableChildren.Count > 0;
				if (flag3)
				{
					charIdListTemp.AddRange(availableChildren);
				}
			}
			bool isTutorial = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == 1;
			bool flag4 = isTutorial;
			if (flag4)
			{
				charIdListTemp.Clear();
				charIdListTemp.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			}
			this.ShowSelectCharWithFilter(charIdListTemp, new SelectCharacterCallback(this.SelectShopManager));
		}

		// Token: 0x06009C4E RID: 40014 RVA: 0x004938C0 File Offset: 0x00491AC0
		private void ShowSelectCharWithFilter(List<int> charIdList, SelectCharacterCallback callback)
		{
			int curId = this.DisplayData.ShopManagerList[this._selectingShopManagerIndex];
			TaiwuDomainMethod.AsyncCall.GetVillagersForWorkDisplayData(this.ParentView, charIdList, delegate(int offset, RawDataPool pool)
			{
				List<VillagerSelectCharacterDisplayData> displayData = new List<VillagerSelectCharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new VillagerSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.Villager);
				config.InteractionMode = ESelectCharacterInteractionMode.Slot;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				CommonSelectCharacterConfig commonSelectCharacterConfig = config;
				object initialSelectedCharacterIds;
				if (curId < 0)
				{
					initialSelectedCharacterIds = null;
				}
				else
				{
					(initialSelectedCharacterIds = new List<int>()).Add(curId);
				}
				commonSelectCharacterConfig.InitialSelectedCharacterIds = initialSelectedCharacterIds;
				config.BannedCharacterIds = (from id in this.DisplayData.ShopManagerList
				where id >= 0
				select id).ToHashSet<int>();
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.WorkStatus,
					ESelectCharacterFilterMenuId.RoleArrangementWork,
					ESelectCharacterFilterMenuId.Identity
				};
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06009C4F RID: 40015 RVA: 0x00493918 File Offset: 0x00491B18
		private void SelectShopManager(List<int> list)
		{
			int oldId = this.DisplayData.ShopManagerList[this._selectingShopManagerIndex];
			int newId = (list.Count == 0) ? -1 : list.Single<int>();
			bool flag = oldId == newId;
			if (!flag)
			{
				BuildingDomainMethod.Call.SetShopManager(this.BlockKey, (sbyte)this._selectingShopManagerIndex, newId);
				this.ParentView.RequestData();
			}
		}

		// Token: 0x06009C50 RID: 40016 RVA: 0x00493979 File Offset: 0x00491B79
		private void CancelShopManager(int index)
		{
			BuildingDomainMethod.Call.SetShopManager(this.BlockKey, (sbyte)index, -1);
			this.ParentView.RequestData();
		}

		// Token: 0x06009C51 RID: 40017 RVA: 0x00493997 File Offset: 0x00491B97
		private void SetUnlockedWorkingVillager(int charId, bool isUnlock)
		{
			BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, isUnlock);
			this.ParentView.RequestData();
		}

		// Token: 0x06009C52 RID: 40018 RVA: 0x004939AE File Offset: 0x00491BAE
		private void OnAssignRole(bool _)
		{
			this.ParentView.RequestData();
		}

		// Token: 0x06009C53 RID: 40019 RVA: 0x004939C0 File Offset: 0x00491BC0
		private void OpenMultiSelectItemWindow()
		{
			SelectItemConfig config = SelectItemConfig.CreateMultipleSelectConfig(new SelectItemRules(), new SelectItemsCallback(this.OnConfirmSelectItem), "", 0, -1, new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount | ESelectItemColumnType.Type | ESelectItemColumnType.Value | ESelectItemColumnType.Weight));
			config.MaxSelectCount = 1;
			config.InitialSelectedItems = null;
			config.AllowEmpty = true;
			config.ShowSelectedArea = true;
			config.OperationMode = ESelectItemOperationMode.Slot;
			config.CustomTextGenerator = null;
			config.ExternalItems = this.DisplayData.InventoryCanSoldItemList;
			config.ExternalWarehouseItems = this.DisplayData.WarehouseCanSoldItemList;
			config.ExternalTreasuryItems = this.DisplayData.TreasuryCanSoldItemList;
			UIElement.SelectItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectItemConfig", config));
			UIManager.Instance.MaskUI(UIElement.SelectItem);
		}

		// Token: 0x06009C54 RID: 40020 RVA: 0x00493A84 File Offset: 0x00491C84
		private void OnConfirmSelectItem(List<SelectedItemData> selectedItemDataList)
		{
			ItemSourceType removeToItemSourceType = this.DisplayData.CanTransferItemToWarehouse ? ItemSourceType.Inventory : ItemSourceType.Warehouse;
			ItemDisplayData oldItem = this.DisplayData.FixingBookItemData;
			bool hasOld = oldItem != null;
			SelectedItemData selectedItemData = selectedItemDataList.FirstOrDefault<SelectedItemData>();
			ItemDisplayData newItem = ((selectedItemData != null) ? selectedItemData.ItemData : null) as ItemDisplayData;
			bool hasNew = newItem != null;
			bool flag = !hasOld && !hasNew;
			if (!flag)
			{
				bool flag2 = !hasOld && hasNew;
				if (flag2)
				{
					BuildingDomainMethod.Call.AddFixBook(this.BlockKey, newItem.RealKey, newItem.ItemSourceTypeEnum);
				}
				bool flag3 = hasOld && !hasNew;
				if (flag3)
				{
					BuildingDomainMethod.Call.ChangeFixBook(this.ParentView.Element.GameDataListenerId, this.BlockKey, ItemKey.Invalid, removeToItemSourceType);
				}
				bool flag4 = hasOld && hasNew;
				if (flag4)
				{
					BuildingDomainMethod.Call.ChangeFixBook(this.ParentView.Element.GameDataListenerId, this.BlockKey, newItem.RealKey, newItem.ItemSourceTypeEnum);
				}
				this.ParentView.RequestData();
				bool flag5 = this.BlockData.TemplateId == 105;
				if (flag5)
				{
					BuildingDomainMethod.AsyncCall.GetBuildingBlockData(this.ParentView, this.BlockKey, delegate(int offset2, RawDataPool pool2)
					{
						BuildingBlockData blockData = null;
						Serializer.Deserialize(pool2, offset2, ref blockData);
						UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>().UpdateBuildingData(this.BlockKey, blockData, true);
					});
				}
			}
		}

		// Token: 0x06009C55 RID: 40021 RVA: 0x00493BB4 File Offset: 0x00491DB4
		[return: TupleElementNames(new string[]
		{
			"iconName",
			"requireName"
		})]
		public static ValueTuple<string, string> GetBuildingRequireIconAndName(short templateId)
		{
			BuildingBlockItem buildingConfig = BuildingBlock.Instance[templateId];
			bool flag = buildingConfig.RequireLifeSkillType >= 0;
			ValueTuple<string, string> result;
			if (flag)
			{
				sbyte type = buildingConfig.RequireLifeSkillType;
				LifeSkillTypeItem config = LifeSkillType.Instance[type];
				result = new ValueTuple<string, string>(config.DisplayIcon, config.Name);
			}
			else
			{
				bool flag2 = buildingConfig.RequireCombatSkillType >= 0;
				if (flag2)
				{
					sbyte type2 = buildingConfig.RequireCombatSkillType;
					CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
					result = new ValueTuple<string, string>(config2.DisplayIcon, config2.Name);
				}
				else
				{
					result = new ValueTuple<string, string>(string.Empty, string.Empty);
				}
			}
			return result;
		}

		// Token: 0x06009C56 RID: 40022 RVA: 0x00493C5C File Offset: 0x00491E5C
		private sbyte GetShopManagerCount()
		{
			List<int> managerList = this.ShopManagerList;
			sbyte count = 0;
			for (int i = 0; i < managerList.Count; i++)
			{
				bool flag = managerList[i] != -1;
				if (flag)
				{
					count += 1;
				}
			}
			return count;
		}

		// Token: 0x06009C57 RID: 40023 RVA: 0x00493CAC File Offset: 0x00491EAC
		private int GetLeaderCount()
		{
			List<int> managerList = this.ShopManagerList;
			return (managerList[0] >= 0) ? 1 : 0;
		}

		// Token: 0x06009C58 RID: 40024 RVA: 0x00493CD4 File Offset: 0x00491ED4
		private int GetMemberCount()
		{
			List<int> managerList = this.ShopManagerList;
			sbyte count = 0;
			for (int i = 0; i < managerList.Count; i++)
			{
				bool flag = managerList[i] != -1;
				if (flag)
				{
					count += 1;
				}
			}
			return (int)count - this.GetLeaderCount();
		}

		// Token: 0x06009C59 RID: 40025 RVA: 0x00493D28 File Offset: 0x00491F28
		private sbyte GetOperationNeedSkillType()
		{
			bool isCollectResourceBuilding = this.ConfigData.IsCollectResourceBuilding;
			sbyte lifeSkillType;
			if (isCollectResourceBuilding)
			{
				sbyte resourceType = this._buildingModel.GetCollectBuildingResourceTypeWithToxicology(this.BlockKey, this.BlockData);
				lifeSkillType = this.ConfigData.RequireLifeSkillType;
			}
			else
			{
				lifeSkillType = this.ConfigData.RequireLifeSkillType;
			}
			return lifeSkillType;
		}

		// Token: 0x06009C5A RID: 40026 RVA: 0x00493D84 File Offset: 0x00491F84
		private bool IsFixBookManage()
		{
			return this.BlockData.TemplateId == 105;
		}

		// Token: 0x06009C5B RID: 40027 RVA: 0x00493DA8 File Offset: 0x00491FA8
		private bool IsNormalManage()
		{
			return GameData.Domains.Building.SharedMethods.BuildingIsShopWithEvent(this.ConfigData);
		}

		// Token: 0x06009C5C RID: 40028 RVA: 0x00493DC8 File Offset: 0x00491FC8
		private string GetPredictProgressText(out int cur, out int delta, out int max)
		{
			cur = (int)this.BlockData.ShopProgress;
			delta = this.ShopManageProgressDelta * this.ShopProgressBonus;
			int needProgress = (int)this.ConfigData.MaxProduceValue;
			SingletonObject.getInstance<BasicGameData>().ChallengeModeData.ApplyChallengeModeBuildingWorkHard(ref needProgress);
			max = needProgress;
			return ViewBuildingManage.GetPredictProgressText(cur, delta, max);
		}

		// Token: 0x06009C5D RID: 40029 RVA: 0x00493E28 File Offset: 0x00492028
		private void UpdateRecord()
		{
			this.textManagerLearnRecordTitle.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_ShopLearnRecordTitle, this.ConfigData.Name, LanguageKey.LK_Building_ShopLearnEventBook.Tr());
			this.generalRecord.DataRender = new Func<TransferableRecord, TransferableRecordDataBase, ValueTuple<string, string>>(BuildingManageSubPageShop.ShopEventRecordDataRender);
			this.generalRecord.Set(this.DisplayData.ShopEventRecordData, new Func<TransferableRecord, bool>(this.CanShowRecord));
		}

		// Token: 0x06009C5E RID: 40030 RVA: 0x00493E9B File Offset: 0x0049209B
		private bool CanShowRecord(TransferableRecord record)
		{
			return record.RecordType < 0 || ShopEvent.Instance[record.RecordType].ShopEventType == 1;
		}

		// Token: 0x06009C5F RID: 40031 RVA: 0x00493EC4 File Offset: 0x004920C4
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		private static ValueTuple<string, string> ShopEventRecordDataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			ShopEventItem config = ShopEvent.Instance[record.RecordType];
			bool flag = config != null;
			ValueTuple<string, string> result;
			if (flag)
			{
				result = new ValueTuple<string, string>(string.Format(config.Desc, (from x in record.Arguments
				select GameMessageUtils.ReadArguments(x.Item1, x.Item2, data)).ToArray<object>()).ColorReplace(), "");
			}
			else
			{
				Debug.LogWarning(string.Format("Invalid record type: {0}", record.RecordType));
				result = new ValueTuple<string, string>("", "");
			}
			return result;
		}

		// Token: 0x040078F0 RID: 30960
		[Header("顶部")]
		[SerializeField]
		private CImage imageSkillIcon;

		// Token: 0x040078F1 RID: 30961
		[SerializeField]
		private TextMeshProUGUI textSkillName;

		// Token: 0x040078F2 RID: 30962
		[SerializeField]
		private CButton buttonShopQuickSelect;

		// Token: 0x040078F3 RID: 30963
		[SerializeField]
		private CButton buttonShopQuickCancel;

		// Token: 0x040078F4 RID: 30964
		[SerializeField]
		private CButton buttonArrangementSetting;

		// Token: 0x040078F5 RID: 30965
		[SerializeField]
		private CToggle toggleAutoArrange;

		// Token: 0x040078F6 RID: 30966
		[SerializeField]
		private CButton buttonVillagerManager;

		// Token: 0x040078F7 RID: 30967
		[SerializeField]
		private GameObject rootProgressArea;

		// Token: 0x040078F8 RID: 30968
		[Header("普通进度")]
		[SerializeField]
		private GameObject rootNormal;

		// Token: 0x040078F9 RID: 30969
		[SerializeField]
		private GameObject rootIcon;

		// Token: 0x040078FA RID: 30970
		[SerializeField]
		private TextMeshProUGUI textProduct;

		// Token: 0x040078FB RID: 30971
		[SerializeField]
		private TooltipInvoker tipProduct;

		// Token: 0x040078FC RID: 30972
		[SerializeField]
		private CImage imageRequire;

		// Token: 0x040078FD RID: 30973
		[SerializeField]
		private TooltipInvoker tipRequire;

		// Token: 0x040078FE RID: 30974
		[SerializeField]
		private GameObject rootNormalProgress;

		// Token: 0x040078FF RID: 30975
		[SerializeField]
		private CImage imageNormalProgress;

		// Token: 0x04007900 RID: 30976
		[SerializeField]
		private CImage imageNormalProgressIncrease;

		// Token: 0x04007901 RID: 30977
		[SerializeField]
		private TextMeshProUGUI textNormalProgress;

		// Token: 0x04007902 RID: 30978
		[SerializeField]
		private TooltipInvoker tipNormalProgress;

		// Token: 0x04007903 RID: 30979
		[Header("修理藏书进度")]
		[SerializeField]
		private GameObject rootFixBook;

		// Token: 0x04007904 RID: 30980
		[SerializeField]
		private TextMeshProUGUI textItemName;

		// Token: 0x04007905 RID: 30981
		[SerializeField]
		private ItemResourceButton buttonFixBook;

		// Token: 0x04007906 RID: 30982
		[SerializeField]
		private BookPageInfoCell curBookPageInfoCell;

		// Token: 0x04007907 RID: 30983
		[SerializeField]
		private BookPageInfoCell newBookPageInfoCell;

		// Token: 0x04007908 RID: 30984
		[SerializeField]
		private GameObject rootBookInfo;

		// Token: 0x04007909 RID: 30985
		[SerializeField]
		private GameObject rootProgressState;

		// Token: 0x0400790A RID: 30986
		[SerializeField]
		private TextMeshProUGUI textProgressState;

		// Token: 0x0400790B RID: 30987
		[SerializeField]
		private GameObject rootBookProgress;

		// Token: 0x0400790C RID: 30988
		[SerializeField]
		private CImage imageBookProgress;

		// Token: 0x0400790D RID: 30989
		[SerializeField]
		private CImage imageBookProgressIncrease;

		// Token: 0x0400790E RID: 30990
		[SerializeField]
		private TextMeshProUGUI textBookProgress;

		// Token: 0x0400790F RID: 30991
		[Header("主事")]
		[SerializeField]
		private TextMeshProUGUI textManagerLeaderTitle;

		// Token: 0x04007910 RID: 30992
		[SerializeField]
		private BuildingManagerLeaderView managerLeaderView;

		// Token: 0x04007911 RID: 30993
		[Header("成员")]
		[SerializeField]
		private TextMeshProUGUI textManagerMemberTitle;

		// Token: 0x04007912 RID: 30994
		[SerializeField]
		private BuildingManagerMemberView[] managerMemberViewArray;

		// Token: 0x04007913 RID: 30995
		[Header("研习簿")]
		[SerializeField]
		private TextMeshProUGUI textManagerLearnRecordTitle;

		// Token: 0x04007914 RID: 30996
		[SerializeField]
		private GeneralRecord generalRecord;

		// Token: 0x04007915 RID: 30997
		[Header("派遣方案界面")]
		[SerializeField]
		private ArrangementSettingPanel arrangementSettingPanel;

		// Token: 0x04007916 RID: 30998
		private readonly StringBuilder _stringBuilder = new StringBuilder();

		// Token: 0x04007917 RID: 30999
		private BuildingModel _buildingModel;

		// Token: 0x04007918 RID: 31000
		private readonly int[] _shopManagerListCached = new int[7];

		// Token: 0x04007919 RID: 31001
		private int _selectingShopManagerIndex;
	}
}
