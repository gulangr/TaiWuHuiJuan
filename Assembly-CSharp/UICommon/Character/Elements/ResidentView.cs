using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.Building;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F7 RID: 1527
	public class ResidentView : Refers
	{
		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x06004803 RID: 18435 RVA: 0x0021C50D File Offset: 0x0021A70D
		public int CharId
		{
			get
			{
				return this._charId;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x06004804 RID: 18436 RVA: 0x0021C515 File Offset: 0x0021A715
		private BuildingModel _buildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06004805 RID: 18437 RVA: 0x0021C51C File Offset: 0x0021A71C
		public void RenderShopCharInfo(int charId)
		{
			this._charId = charId;
			this.GetRefers(false);
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charId);
			});
			this.SetLockToggleState(charId);
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				CharacterAvatar characterAvatar = new CharacterAvatar(base.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				CharacterHappiness happinessController = new CharacterHappiness(base.CGet<Refers>("Happiness"), false);
				elementList.Add(happinessController);
				CharacterFavorability favorController = new CharacterFavorability(base.CGet<Refers>("Favor"), false);
				elementList.Add(favorController);
				CharacterGender characterGender = new CharacterGender(base.CGet<Refers>("Gender"));
				elementList.Add(characterGender);
				CharacterHealth healthController = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
				elementList.Add(healthController);
				base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(false);
				this.UserObject = elementList;
			}
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId != -1);
			this._functionHolder.SetActive(charId != -1);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			this.InitMouseTipDisplayer();
		}

		// Token: 0x06004806 RID: 18438 RVA: 0x0021C6B8 File Offset: 0x0021A8B8
		public void RenderResidentCharInfo(int charId)
		{
			this._charId = charId;
			this.GetRefers(true);
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charId);
			});
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				CharacterAvatar characterAvatar = new CharacterAvatar(base.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				CharacterHappiness happinessController = new CharacterHappiness(base.CGet<Refers>("Happiness"), false);
				elementList.Add(happinessController);
				CharacterFavorability favorController = new CharacterFavorability(base.CGet<Refers>("Favor"), false);
				elementList.Add(favorController);
				CharacterGender characterGender = new CharacterGender(base.CGet<Refers>("Gender"));
				elementList.Add(characterGender);
				CharacterHealth healthController = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
				elementList.Add(healthController);
				base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(false);
				this.UserObject = elementList;
			}
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId != -1);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			this.InitMouseTipDisplayer();
		}

		// Token: 0x06004807 RID: 18439 RVA: 0x0021C830 File Offset: 0x0021AA30
		private void InitMouseTipDisplayer()
		{
			TooltipInvoker mouseTipDisplayer = base.GetComponent<TooltipInvoker>();
			bool flag = mouseTipDisplayer == null;
			if (!flag)
			{
				mouseTipDisplayer.enabled = (this._charId >= 0);
				bool flag2 = this._charId < 0;
				if (!flag2)
				{
					mouseTipDisplayer.Type = TipType.CharacterOnMapBlock;
					TooltipInvoker tooltipInvoker = mouseTipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTipDisplayer.RuntimeParam.Set("CharId", this._charId);
				}
			}
		}

		// Token: 0x06004808 RID: 18440 RVA: 0x0021C8AC File Offset: 0x0021AAAC
		private void GetRefers(bool isResident)
		{
			bool flag = !this._isGetRefers;
			if (flag)
			{
				this._selectCharBack = base.CGet<GameObject>("SelectCharBack");
				this._charInfoHolder = base.CGet<GameObject>("CharInfoHolder");
				this._showMainCharacterMenu = base.CGet<CButtonObsolete>("ShowMainCharacterMenu");
				bool flag2 = !isResident;
				if (flag2)
				{
					this._functionHolder = base.CGet<GameObject>("FunctionHolder");
					this._lockToggle = base.CGet<CToggleObsolete>("LockToggle");
				}
				this._isGetRefers = true;
			}
		}

		// Token: 0x06004809 RID: 18441 RVA: 0x0021C930 File Offset: 0x0021AB30
		private void SetLockToggleState(int charId)
		{
			bool flag = charId == -1;
			if (!flag)
			{
				this._lockToggle.onValueChanged.RemoveAllListeners();
				this._lockToggle.isOn = (!ResidentView._unlockedWorkingList.Contains(charId) || !this._buildingModel.VillagerWork.ContainsKey(charId));
				bool flag2 = !this._buildingModel.VillagerWork.ContainsKey(charId);
				if (flag2)
				{
					BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, false);
				}
				this._lockToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					this.OnLockToggleChange(isOn, charId);
				});
			}
		}

		// Token: 0x0600480A RID: 18442 RVA: 0x0021C9F8 File Offset: 0x0021ABF8
		private void OnLockToggleChange(bool isOn, int charId)
		{
			BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, !isOn);
		}

		// Token: 0x0600480B RID: 18443 RVA: 0x0021CA08 File Offset: 0x0021AC08
		private void ShowCharacterMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0600480C RID: 18444 RVA: 0x0021CA46 File Offset: 0x0021AC46
		public static void SetUnlockedWorkingList(List<int> list)
		{
			ResidentView._unlockedWorkingList = list;
		}

		// Token: 0x0600480D RID: 18445 RVA: 0x0021CA50 File Offset: 0x0021AC50
		public void RenderManagerLeader(int charId, BuildingBlockItem buildingConfig, BuildingBlockKey blockKey, Action<int> actionWhenQuickAssign)
		{
			this._charId = charId;
			this.GetManagerLeaderRefers();
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this._charId);
			});
			this.SetLockToggleState(charId);
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				CharacterAvatar characterAvatar = new CharacterAvatar(base.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				CharacterHappiness happinessController = new CharacterHappiness(base.CGet<Refers>("Happiness"), false);
				elementList.Add(happinessController);
				CharacterHealth healthController = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
				elementList.Add(healthController);
				Refers ageRefers = base.CGet<Refers>("CharacterAgeInfo");
				TextMeshProUGUI ageLabel = ageRefers.CGet<TextMeshProUGUI>("Value");
				CharacterAge characterAge = new CharacterAge(ageLabel, null, null, null, true, true, null, null);
				elementList.Add(characterAge);
				TextMeshProUGUI identityLabel = base.CGet<TextMeshProUGUI>("IdentityLabel");
				CharacterIdentity identityItem = new CharacterIdentity(identityLabel, null);
				elementList.Add(identityItem);
				this.UserObject = elementList;
			}
			this.AsyncRefreshEfficiency(charId, blockKey);
			base.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(charId >= 0);
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId >= 0);
			base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(charId >= 0);
			this._lockToggle.gameObject.SetActive(charId >= 0);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			this.InitMouseTipDisplayer();
			bool flag2 = this._charId >= 0;
			if (flag2)
			{
				this.RefreshManagerLeaderInfo(buildingConfig, actionWhenQuickAssign);
			}
			this._canTeachFlag.gameObject.SetActive(true);
			this.InitLeaderTips(buildingConfig);
			base.CGet<GameObject>("NoLeaderWarning").SetActive(charId < 0 && buildingConfig.NeedLeader);
			this.RefreshInfoGridBg();
		}

		// Token: 0x0600480E RID: 18446 RVA: 0x0021CCA8 File Offset: 0x0021AEA8
		private void AsyncRefreshEfficiency(int charId, BuildingBlockKey blockKey)
		{
			Refers efficiencyRefers;
			bool flag = this.CTryGet<Refers>("Efficiency", out efficiencyRefers);
			if (flag)
			{
				TextMeshProUGUI efficiencyLabel = efficiencyRefers.CGet<TextMeshProUGUI>("Value");
				BuildingDomainMethod.AsyncCall.CalcTaiwuVillagerEfficiencyInBuilding(null, blockKey, charId, delegate(int offset, RawDataPool pool)
				{
					int result = 0;
					Serializer.Deserialize(pool, offset, ref result);
					bool flag2 = result < 0;
					if (flag2)
					{
						efficiencyLabel.text = "0%";
					}
					else
					{
						efficiencyLabel.text = string.Format("{0}%", result);
					}
				});
			}
		}

		// Token: 0x0600480F RID: 18447 RVA: 0x0021CCF8 File Offset: 0x0021AEF8
		private void RefreshMemberBookCount(ShopBuildingTeachBookData teachBookData)
		{
			Refers bookCountRefers;
			bool flag = this.CTryGet<Refers>("BookCount", out bookCountRefers);
			if (flag)
			{
				TextMeshProUGUI value = bookCountRefers.CGet<TextMeshProUGUI>("Value");
				value.text = string.Format("{0}/{1}", teachBookData.MemberLearnedBookCount, teachBookData.LeaderCanTeachBookCount);
			}
		}

		// Token: 0x06004810 RID: 18448 RVA: 0x0021CD4C File Offset: 0x0021AF4C
		private void InitLeaderTips(BuildingBlockItem buildingConfig)
		{
			short[] roleIds = buildingConfig.VillagerRoleTemplateIds;
			this._leaderTips.Type = TipType.GeneralLines;
			TooltipInvoker leaderTips = this._leaderTips;
			if (leaderTips.RuntimeParam == null)
			{
				leaderTips.RuntimeParam = new ArgumentBox();
			}
			ResidentView.<>c__DisplayClass32_0 CS$<>8__locals1;
			CS$<>8__locals1.param = this._leaderTips.RuntimeParam;
			CS$<>8__locals1.param.Set("Title", LocalStringManager.Get(LanguageKey.LK_Building_Leader_Tips_Title));
			CS$<>8__locals1.lineCount = 0;
			ResidentView.<InitLeaderTips>g__AddNode|32_0(new GeneralLineData
			{
				Type = 3,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_Leader_Tips_Content)
				}
			}, ref CS$<>8__locals1);
			ResidentView.<InitLeaderTips>g__AddNode|32_0(new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_Leader_Tips_SubTitle_1)
				}
			}, ref CS$<>8__locals1);
			foreach (short roleId in roleIds)
			{
				string roleName = VillagerRoleUtils.GetRoleOriginalNameWithGrade(roleId);
				ResidentView.<InitLeaderTips>g__AddNode|32_0(new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						roleName
					},
					ExtraArgs = new List<object>
					{
						20
					}
				}, ref CS$<>8__locals1);
			}
			CS$<>8__locals1.param.Set("LineCount", CS$<>8__locals1.lineCount);
		}

		// Token: 0x06004811 RID: 18449 RVA: 0x0021CEA4 File Offset: 0x0021B0A4
		private void RefreshManagerLeaderInfo(BuildingBlockItem buildingConfig, Action<int> actionWhenQuickAssign)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._charId, delegate(int offset, RawDataPool dataPool)
			{
				VillagerRoleCharacterDisplayData villagerDisplayData = null;
				Serializer.Deserialize(dataPool, offset, ref villagerDisplayData);
				bool flag = buildingConfig.RequireLifeSkillType >= 0;
				if (flag)
				{
					sbyte type = buildingConfig.RequireLifeSkillType;
					LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
					this._qualifications.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					this._qualifications.CGet<TextMeshProUGUI>("SkillName").SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Qualification), true);
					this._qualifications.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillQualifications[(int)type].ToString(), true);
					this._attainment.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					this._attainment.CGet<TextMeshProUGUI>("SkillName").SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
					this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillAttainments[(int)type].ToString(), true);
				}
				else
				{
					bool flag2 = buildingConfig.RequireCombatSkillType >= 0;
					if (flag2)
					{
						sbyte type2 = buildingConfig.RequireCombatSkillType;
						CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
						this._qualifications.CGet<CImage>("SkillIcon").SetSprite(config2.DisplayIcon, false, null);
						this._qualifications.CGet<TextMeshProUGUI>("SkillName").SetText(config2.Name + LocalStringManager.Get(LanguageKey.LK_Qualification), true);
						this._qualifications.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.CombatSkillQualifications[(int)type2].ToString(), true);
						this._attainment.CGet<CImage>("SkillIcon").SetSprite(config2.DisplayIcon, false, null);
						this._attainment.CGet<TextMeshProUGUI>("SkillName").SetText(config2.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
						this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.CombatSkillAttainments[(int)type2].ToString(), true);
					}
				}
				this._readBook.CGet<TextMeshProUGUI>("Grade").SetText(LocalStringManager.Get(string.Format("LK_Grade_{0}", villagerDisplayData.ReadBookMaxGrade)).SetColor(Colors.Instance.GradeColors[(int)villagerDisplayData.ReadBookMaxGrade]), true);
				this.SetMatchRoleButton(villagerDisplayData, buildingConfig, actionWhenQuickAssign);
				GameObject matchedButton = this._matchVillagerRole.CGet<GameObject>("MatchedButton");
				GameObject notMatchedButton = this._matchVillagerRole.CGet<GameObject>("NotMatchedButton");
				matchedButton.SetActive(villagerDisplayData.MatchVillagerRole);
				notMatchedButton.SetActive(!villagerDisplayData.MatchVillagerRole);
				ResidentView.SetMatchBg(this._matchVillagerRole.CGet<CImage>("MatchBg"), villagerDisplayData.MatchVillagerRole);
				ResidentView.SetMatchLabel(this._matchVillagerRole.CGet<TextMeshProUGUI>("MatchName"), villagerDisplayData.MatchVillagerRole);
				ResidentView.SetMatchBigBg(this._leaderCanTeachBg, villagerDisplayData.MatchVillagerRole);
				ResidentView.SetActiveByMatch(this._leaderCanTeachFrame, villagerDisplayData.MatchVillagerRole);
				ResidentView.SetActiveByMatch(this._canTeachFlag, villagerDisplayData.MatchVillagerRole);
			});
		}

		// Token: 0x06004812 RID: 18450 RVA: 0x0021CEE8 File Offset: 0x0021B0E8
		private void SetMatchRoleButton(VillagerRoleCharacterDisplayData villagerDisplayData, BuildingBlockItem buildingConfig, Action<int> actionWhenQuickAssign)
		{
			bool hasTaiwuShrine = ViewBuildingArea.HasBuilding(45, true);
			CButtonObsolete matchRoleBtn = this._matchVillagerRole.CGet<CButtonObsolete>("BtnMatchVillagerRole");
			PointerTrigger pointerTrigger = matchRoleBtn.GetComponent<PointerTrigger>();
			matchRoleBtn.interactable = hasTaiwuShrine;
			matchRoleBtn.onClick.RemoveAllListeners();
			pointerTrigger.enabled = hasTaiwuShrine;
			bool flag = hasTaiwuShrine;
			if (flag)
			{
				matchRoleBtn.ClearAndAddListener(delegate
				{
					bool flag2 = !villagerDisplayData.MatchVillagerRole;
					if (flag2)
					{
						Action<int> actionWhenQuickAssign2 = actionWhenQuickAssign;
						if (actionWhenQuickAssign2 != null)
						{
							actionWhenQuickAssign2(villagerDisplayData.Id);
						}
					}
					else
					{
						VillagerRoleUtils.ConfirmAndAssignRole(villagerDisplayData.Id, -1, null, null, null);
					}
				});
			}
		}

		// Token: 0x06004813 RID: 18451 RVA: 0x0021CF64 File Offset: 0x0021B164
		private void GetManagerLeaderRefers()
		{
			bool flag = !this._isGetRefers;
			if (flag)
			{
				this._selectCharBack = base.CGet<GameObject>("SelectCharBack");
				this._charInfoHolder = base.CGet<GameObject>("CharInfoHolder");
				this._showMainCharacterMenu = base.CGet<CButtonObsolete>("ShowMainCharacterMenu");
				this._qualifications = base.CGet<Refers>("Qualifications");
				this._attainment = base.CGet<Refers>("Attainment");
				this._readBook = base.CGet<Refers>("ReadBook");
				this._matchVillagerRole = base.CGet<Refers>("MatchVillagerRole");
				this._leaderTips = base.CGet<TooltipInvoker>("LeaderTips");
				this._leaderCanTeachBg = base.CGet<CImage>("CanTeachBg");
				this._canTeachFlag = base.CGet<CImage>("CanTeachFlag");
				this._leaderCanTeachFrame = base.CGet<CImage>("CanTeachBgFrame");
				this._lockToggle = base.CGet<CToggleObsolete>("LockToggle");
				this._isGetRefers = true;
			}
		}

		// Token: 0x06004814 RID: 18452 RVA: 0x0021D058 File Offset: 0x0021B258
		private void GetManagerMemberRefers()
		{
			bool flag = !this._isGetRefers;
			if (flag)
			{
				this._selectCharBack = base.CGet<GameObject>("SelectCharBack");
				this._charInfoHolder = base.CGet<GameObject>("CharInfoHolder");
				this._showMainCharacterMenu = base.CGet<CButtonObsolete>("ShowMainCharacterMenu");
				this._attainment = base.CGet<Refers>("Attainment");
				this._leftPotential = base.CGet<Refers>("LeftPotential");
				this._functionHolder = base.CGet<GameObject>("FunctionHolder");
				this._lockToggle = base.CGet<CToggleObsolete>("LockToggle");
				this._isGetRefers = true;
			}
		}

		// Token: 0x06004815 RID: 18453 RVA: 0x0021D0F8 File Offset: 0x0021B2F8
		public void RenderManagerMemberInfo(int charId, BuildingBlockItem buildingConfig, BuildingBlockKey blockKey)
		{
			this._charId = charId;
			this.GetManagerMemberRefers();
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charId);
			});
			this.SetLockToggleState(charId);
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				Game.Components.Avatar.Avatar avatarRefer = base.CGet<Game.Components.Avatar.Avatar>("Avatar");
				CharacterAvatar characterAvatar = new CharacterAvatar(avatarRefer, true);
				CanvasGroup avatarGroup = avatarRefer.GetComponent<CanvasGroup>();
				bool flag2 = avatarGroup != null;
				if (flag2)
				{
					avatarGroup.alpha = 0f;
				}
				characterAvatar.OnFillAvatar = delegate()
				{
					bool flag4 = avatarGroup != null;
					if (flag4)
					{
						avatarGroup.alpha = 1f;
					}
				};
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				CharacterHappiness happinessController = new CharacterHappiness(base.CGet<Refers>("Happiness"), false);
				elementList.Add(happinessController);
				CharacterHealth healthController = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
				elementList.Add(healthController);
				base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(this._charId >= 0);
				this.UserObject = elementList;
			}
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId != -1);
			this._functionHolder.SetActive(charId != -1);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			this.InitMouseTipDisplayer();
			bool flag3 = this._charId >= 0;
			if (flag3)
			{
				this.RefreshManagerMemberInfo(buildingConfig, blockKey);
			}
			else
			{
				this.SetChildBg(false);
			}
		}

		// Token: 0x06004816 RID: 18454 RVA: 0x0021D2E0 File Offset: 0x0021B4E0
		private void RefreshManagerMemberInfo(BuildingBlockItem buildingConfig, BuildingBlockKey blockKey)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._charId, delegate(int offset, RawDataPool dataPool)
			{
				VillagerRoleCharacterDisplayData villagerDisplayData = null;
				Serializer.Deserialize(dataPool, offset, ref villagerDisplayData);
				bool flag = buildingConfig.RequireLifeSkillType >= 0;
				if (flag)
				{
					sbyte type = buildingConfig.RequireLifeSkillType;
					LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
					this._attainment.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					TextMeshProUGUI skillNameLabel;
					bool flag2 = this._attainment.CTryGet<TextMeshProUGUI>("SkillName", out skillNameLabel);
					if (flag2)
					{
						skillNameLabel.SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
					}
					this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillAttainments[(int)type].ToString(), true);
				}
				else
				{
					bool flag3 = buildingConfig.RequireCombatSkillType >= 0;
					if (flag3)
					{
						sbyte type2 = buildingConfig.RequireCombatSkillType;
						CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
						this._attainment.CGet<CImage>("SkillIcon").SetSprite(config2.DisplayIcon, false, null);
						TextMeshProUGUI skillNameLabel2;
						bool flag4 = this._attainment.CTryGet<TextMeshProUGUI>("SkillName", out skillNameLabel2);
						if (flag4)
						{
							skillNameLabel2.SetText(config2.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
						}
						this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.CombatSkillAttainments[(int)type2].ToString(), true);
					}
				}
				this._leftPotential.CGet<TextMeshProUGUI>("LeftCount").SetText(villagerDisplayData.LeftPotentialCount.ToString(), true);
			});
			BuildingDomainMethod.AsyncCall.GetShopBuildingTeachBookData(null, blockKey, this._charId, delegate(int offset, RawDataPool dataPool)
			{
				ShopBuildingTeachBookData teachBookData = null;
				Serializer.Deserialize(dataPool, offset, ref teachBookData);
				TooltipInvoker mouseTip = this.CGet<TooltipInvoker>("ReadBookMouseTip1");
				mouseTip.Type = TipType.BuildingTeachBook;
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.RuntimeParam.Set<ShopBuildingTeachBookData>("TeachBookData", teachBookData);
				TooltipInvoker mouseTip2 = this.CGet<TooltipInvoker>("ReadBookMouseTip2");
				mouseTip2.Type = TipType.BuildingTeachBook;
				tooltipInvoker = mouseTip2;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip2.RuntimeParam.Set<ShopBuildingTeachBookData>("TeachBookData", teachBookData);
				this.SetFinishReadingTip(teachBookData);
				this.RefreshMemberBookCount(teachBookData);
			});
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this._charId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData characterDisplayData = null;
				Serializer.Deserialize(pool, offset, ref characterDisplayData);
				bool isChild = AgeGroup.GetAgeGroup(characterDisplayData.PhysiologicalAge) == 1;
				this.SetChildBg(isChild);
			});
			this.AsyncRefreshEfficiency(this._charId, blockKey);
			this.RefreshInfoGridBg();
		}

		// Token: 0x06004817 RID: 18455 RVA: 0x0021D364 File Offset: 0x0021B564
		private void SetChildBg(bool isChild)
		{
			GameObject childBg;
			bool flag = this.CTryGet<GameObject>("TeenagerBg", out childBg);
			if (flag)
			{
				childBg.gameObject.SetActive(isChild);
			}
			TooltipInvoker childBg2;
			bool flag2 = this.CTryGet<TooltipInvoker>("TeenagerBg2", out childBg2);
			if (flag2)
			{
				childBg2.gameObject.SetActive(isChild);
			}
		}

		// Token: 0x06004818 RID: 18456 RVA: 0x0021D3B4 File Offset: 0x0021B5B4
		private void SetFinishReadingTip(ShopBuildingTeachBookData teachBookData)
		{
			TooltipInvoker finishReadingTips;
			bool flag = this.CTryGet<TooltipInvoker>("IconFinishReading", out finishReadingTips);
			if (flag)
			{
				bool displayReadingTip = teachBookData.TeachBookResult == 3 || teachBookData.TeachBookResult == 4;
				finishReadingTips.gameObject.SetActive(displayReadingTip);
				bool flag2 = !displayReadingTip;
				if (!flag2)
				{
					bool flag3 = finishReadingTips.PresetParam == null || finishReadingTips.PresetParam.Length < 1;
					if (flag3)
					{
						finishReadingTips.PresetParam = new string[1];
					}
					bool flag4 = teachBookData.TeachBookResult == 4;
					if (flag4)
					{
						finishReadingTips.PresetParam[0] = "LK_Building_UnableToLearn";
					}
					else
					{
						finishReadingTips.PresetParam[0] = "LK_Building_FinishLearning";
					}
				}
			}
		}

		// Token: 0x06004819 RID: 18457 RVA: 0x0021D460 File Offset: 0x0021B660
		private void RefreshInfoGridBg()
		{
			CommonParameterGrid grid;
			bool flag = this.CTryGet<CommonParameterGrid>("InfoGrid", out grid);
			if (flag)
			{
				grid.RefreshBack();
			}
		}

		// Token: 0x0600481A RID: 18458 RVA: 0x0021D488 File Offset: 0x0021B688
		public void RenderCraftsman(int charId, sbyte lifeSkillType)
		{
			this._charId = charId;
			this.GetManagerLeaderRefers();
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this._charId);
			});
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				CharacterAvatar characterAvatar = new CharacterAvatar(base.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				Refers ageRefers = base.CGet<Refers>("CharacterAgeInfo");
				TextMeshProUGUI ageLabel = ageRefers.CGet<TextMeshProUGUI>("Value");
				CImage fiveElemsIcon = ageRefers.CGet<CImage>("Icon");
				fiveElemsIcon.gameObject.SetActive(false);
				CharacterAge characterAge = new CharacterAge(ageLabel, null, null, null, false, true, null, null);
				elementList.Add(characterAge);
				this.UserObject = elementList;
			}
			base.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(charId >= 0);
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId >= 0);
			base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(false);
			this._lockToggle.gameObject.SetActive(false);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			this.InitMouseTipDisplayer();
			bool flag2 = this._charId > 0;
			if (flag2)
			{
				this.RefreshManagerLeaderInfo(lifeSkillType);
			}
		}

		// Token: 0x0600481B RID: 18459 RVA: 0x0021D630 File Offset: 0x0021B830
		private void RefreshManagerLeaderInfo(sbyte lifeSkillType)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._charId, delegate(int offset, RawDataPool dataPool)
			{
				VillagerRoleCharacterDisplayData villagerDisplayData = null;
				Serializer.Deserialize(dataPool, offset, ref villagerDisplayData);
				bool flag = lifeSkillType >= 0;
				if (flag)
				{
					sbyte type = lifeSkillType;
					LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
					this._qualifications.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					this._qualifications.CGet<TextMeshProUGUI>("SkillName").SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Qualification), true);
					this._qualifications.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillQualifications[(int)type].ToString(), true);
					this._attainment.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					this._attainment.CGet<TextMeshProUGUI>("SkillName").SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
					this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillAttainments[(int)type].ToString(), true);
				}
				this._readBook.CGet<TextMeshProUGUI>("Grade").SetText(LocalStringManager.Get(string.Format("LK_Grade_{0}", villagerDisplayData.ReadBookMaxGrade)).SetColor(Colors.Instance.GradeColors[(int)villagerDisplayData.ReadBookMaxGrade]), true);
			});
		}

		// Token: 0x0600481C RID: 18460 RVA: 0x0021D66C File Offset: 0x0021B86C
		public void RenderManagerLeaderCraftsmanPanel(int charId, BuildingBlockItem buildingConfig, Action<bool> matchVillagerRoleAction, Action<int> actionWhenQuickAssign)
		{
			this._charId = charId;
			this.GetManagerLeaderRefers();
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this._charId);
			});
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				Game.Components.Avatar.Avatar avatarRefer = base.CGet<Game.Components.Avatar.Avatar>("Avatar");
				CharacterAvatar characterAvatar = new CharacterAvatar(avatarRefer, true);
				CanvasGroup avatarGroup = avatarRefer.GetComponent<CanvasGroup>();
				bool flag2 = avatarGroup != null;
				if (flag2)
				{
					avatarGroup.alpha = 0f;
				}
				characterAvatar.OnFillAvatar = delegate()
				{
					bool flag4 = avatarGroup != null;
					if (flag4)
					{
						avatarGroup.alpha = 1f;
					}
				};
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				Refers ageRefers = base.CGet<Refers>("CharacterAgeInfo");
				TextMeshProUGUI ageLabel = ageRefers.CGet<TextMeshProUGUI>("Value");
				CImage fiveElemsIcon = ageRefers.CGet<CImage>("Icon");
				fiveElemsIcon.gameObject.SetActive(false);
				CharacterAge characterAge = new CharacterAge(ageLabel, null, null, null, false, true, null, null);
				elementList.Add(characterAge);
				this.UserObject = elementList;
			}
			base.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(charId >= 0);
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId >= 0);
			base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(charId >= 0);
			this._lockToggle.gameObject.SetActive(charId >= 0);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			this.InitMouseTipDisplayer();
			bool flag3 = this._charId >= 0;
			if (flag3)
			{
				this.RefreshManagerLeaderInfo_CraftsmanPanel(buildingConfig, matchVillagerRoleAction, actionWhenQuickAssign);
			}
			this.InitLeaderTips(buildingConfig);
			base.CGet<GameObject>("NoLeaderWarning").SetActive(charId < 0 && buildingConfig.NeedLeader);
		}

		// Token: 0x0600481D RID: 18461 RVA: 0x0021D8A8 File Offset: 0x0021BAA8
		private void RefreshManagerLeaderInfo_CraftsmanPanel(BuildingBlockItem buildingConfig, Action<bool> matchVillagerRoleAction, Action<int> actionWhenQuickAssign)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._charId, delegate(int offset, RawDataPool dataPool)
			{
				VillagerRoleCharacterDisplayData villagerDisplayData = null;
				Serializer.Deserialize(dataPool, offset, ref villagerDisplayData);
				bool flag = buildingConfig.RequireLifeSkillType >= 0;
				if (flag)
				{
					sbyte type = buildingConfig.RequireLifeSkillType;
					LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
					this._qualifications.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					this._qualifications.CGet<TextMeshProUGUI>("SkillName").SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Qualification), true);
					this._qualifications.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillQualifications[(int)type].ToString(), true);
					this._attainment.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
					this._attainment.CGet<TextMeshProUGUI>("SkillName").SetText(config.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
					this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.LifeSkillAttainments[(int)type].ToString(), true);
				}
				else
				{
					bool flag2 = buildingConfig.RequireCombatSkillType >= 0;
					if (flag2)
					{
						sbyte type2 = buildingConfig.RequireCombatSkillType;
						CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
						this._qualifications.CGet<CImage>("SkillIcon").SetSprite(config2.DisplayIcon, false, null);
						this._qualifications.CGet<TextMeshProUGUI>("SkillName").SetText(config2.Name + LocalStringManager.Get(LanguageKey.LK_Qualification), true);
						this._qualifications.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.CombatSkillQualifications[(int)type2].ToString(), true);
						this._attainment.CGet<CImage>("SkillIcon").SetSprite(config2.DisplayIcon, false, null);
						this._attainment.CGet<TextMeshProUGUI>("SkillName").SetText(config2.Name + LocalStringManager.Get(LanguageKey.LK_Attainment), true);
						this._attainment.CGet<TextMeshProUGUI>("SkillCount").SetText(villagerDisplayData.CombatSkillAttainments[(int)type2].ToString(), true);
					}
				}
				Action<bool> matchVillagerRoleAction2 = matchVillagerRoleAction;
				if (matchVillagerRoleAction2 != null)
				{
					matchVillagerRoleAction2(villagerDisplayData.MatchVillagerRole);
				}
				this._readBook.CGet<TextMeshProUGUI>("Grade").SetText(LocalStringManager.Get(string.Format("LK_Grade_{0}", villagerDisplayData.ReadBookMaxGrade)).SetColor(Colors.Instance.GradeColors[(int)villagerDisplayData.ReadBookMaxGrade]), true);
				this.SetMatchRoleButton(villagerDisplayData, buildingConfig, actionWhenQuickAssign);
				GameObject matchedButton = this._matchVillagerRole.CGet<GameObject>("MatchedButton");
				GameObject notMatchedButton = this._matchVillagerRole.CGet<GameObject>("NotMatchedButton");
				matchedButton.SetActive(villagerDisplayData.MatchVillagerRole);
				notMatchedButton.SetActive(!villagerDisplayData.MatchVillagerRole);
				ResidentView.SetMatchBg(this._matchVillagerRole.CGet<CImage>("MatchBg"), villagerDisplayData.MatchVillagerRole);
				ResidentView.SetMatchLabel(this._matchVillagerRole.CGet<TextMeshProUGUI>("MatchName"), villagerDisplayData.MatchVillagerRole);
				ResidentView.SetMatchBigBg(this._leaderCanTeachBg, villagerDisplayData.MatchVillagerRole);
				ResidentView.SetActiveByMatch(this._leaderCanTeachFrame, villagerDisplayData.MatchVillagerRole);
				ResidentView.SetActiveByMatch(this._canTeachFlag, villagerDisplayData.MatchVillagerRole);
			});
		}

		// Token: 0x0600481E RID: 18462 RVA: 0x0021D8F1 File Offset: 0x0021BAF1
		private static void SetMatchBg(CImage bg, bool match)
		{
			bg.SetSprite(match ? "ui_buildingpopup_single_base_1_0" : "ui_buildingpopup_single_base_1_1", false, null);
		}

		// Token: 0x0600481F RID: 18463 RVA: 0x0021D90C File Offset: 0x0021BB0C
		private static void SetMatchLabel(TextMeshProUGUI label, bool match)
		{
			string str = LocalStringManager.Get(match ? LanguageKey.LK_Building_Arrangement_TeachMatch : LanguageKey.LK_Building_Arrangement_TeachNotMatch);
			label.SetText(str, true);
		}

		// Token: 0x06004820 RID: 18464 RVA: 0x0021D938 File Offset: 0x0021BB38
		private static void SetMatchBigBg(CImage bigBg, bool match)
		{
			bigBg.SetSprite(match ? "ui_buildingpopup_single_base_character_0_1" : "ui_buildingpopup_single_base_character_1_0", false, null);
		}

		// Token: 0x06004821 RID: 18465 RVA: 0x0021D953 File Offset: 0x0021BB53
		private static void SetActiveByMatch(CImage bigFrame, bool match)
		{
			bigFrame.gameObject.SetActive(match);
		}

		// Token: 0x06004822 RID: 18466 RVA: 0x0021D964 File Offset: 0x0021BB64
		[Obsolete]
		public void RenderCharInfo(int charId, bool isInfected = false)
		{
			this.GetRefers(false);
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charId);
			});
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				CharacterAvatar characterAvatar = new CharacterAvatar(base.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				CharacterBehavior behaviorController = new CharacterBehavior(base.CGet<Refers>("BehaviorType"), false);
				elementList.Add(behaviorController);
				CharacterHappiness happinessController = new CharacterHappiness(base.CGet<Refers>("Happiness"), false);
				elementList.Add(happinessController);
				CharacterFavorability favorController = new CharacterFavorability(base.CGet<Refers>("Favor"), false);
				elementList.Add(favorController);
				CharacterFame fameController = new CharacterFame(base.CGet<Refers>("Fame"), false);
				elementList.Add(fameController);
				CharacterHealth healthController = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
				elementList.Add(healthController);
				this.UserObject = elementList;
			}
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			base.CGet<GameObject>("InfectedIcon").SetActive(isInfected);
		}

		// Token: 0x06004823 RID: 18467 RVA: 0x0021DAB4 File Offset: 0x0021BCB4
		[Obsolete]
		public void RenderExpandCharInfo(int charId)
		{
			this.GetRefers(false);
			this._showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charId);
			});
			this.SetLockToggleState(charId);
			List<CharacterUIElement> elementList = this.UserObject as List<CharacterUIElement>;
			bool flag = elementList == null;
			if (flag)
			{
				elementList = new List<CharacterUIElement>();
				CharacterAvatar characterAvatar = new CharacterAvatar(base.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
				elementList.Add(characterAvatar);
				CharacterName nameController = new CharacterName(base.CGet<TextMeshProUGUI>("NameText"), null, null);
				elementList.Add(nameController);
				CharacterBehavior behaviorController = new CharacterBehavior(base.CGet<Refers>("BehaviorType"), true);
				elementList.Add(behaviorController);
				CharacterFavorability favorController = new CharacterFavorability(base.CGet<Refers>("Favor"), true);
				elementList.Add(favorController);
				CharacterFame fameController = new CharacterFame(base.CGet<Refers>("Fame"), true);
				elementList.Add(fameController);
				CharacterHealth healthController = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
				elementList.Add(healthController);
				base.CGet<CButtonObsolete>("ChangeButton").gameObject.SetActive(false);
				this.UserObject = elementList;
			}
			this._selectCharBack.SetActive(charId == -1);
			this._charInfoHolder.SetActive(charId != -1);
			this._functionHolder.SetActive(charId != -1);
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
			base.CGet<CImage>("ExpandManpower").SetSprite((charId == -1) ? "building_kuang_3" : "building_kuang_2", false, null);
		}

		// Token: 0x06004826 RID: 18470 RVA: 0x0021DC84 File Offset: 0x0021BE84
		[CompilerGenerated]
		internal static void <InitLeaderTips>g__AddNode|32_0(GeneralLineData lineData, ref ResidentView.<>c__DisplayClass32_0 A_1)
		{
			ArgumentBox param = A_1.param;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			param.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x040031C2 RID: 12738
		private GameObject _selectCharBack;

		// Token: 0x040031C3 RID: 12739
		private GameObject _charInfoHolder;

		// Token: 0x040031C4 RID: 12740
		private GameObject _functionHolder;

		// Token: 0x040031C5 RID: 12741
		private CButtonObsolete _showMainCharacterMenu;

		// Token: 0x040031C6 RID: 12742
		private CToggleObsolete _lockToggle;

		// Token: 0x040031C7 RID: 12743
		private bool _isGetRefers;

		// Token: 0x040031C8 RID: 12744
		private static List<int> _unlockedWorkingList = new List<int>();

		// Token: 0x040031C9 RID: 12745
		private int _charId = -1;

		// Token: 0x040031CA RID: 12746
		private Refers _qualifications;

		// Token: 0x040031CB RID: 12747
		private Refers _attainment;

		// Token: 0x040031CC RID: 12748
		private Refers _readBook;

		// Token: 0x040031CD RID: 12749
		private Refers _matchVillagerRole;

		// Token: 0x040031CE RID: 12750
		private Refers _leftPotential;

		// Token: 0x040031CF RID: 12751
		private TooltipInvoker _leaderTips;

		// Token: 0x040031D0 RID: 12752
		private CImage _leaderCanTeachBg;

		// Token: 0x040031D1 RID: 12753
		private CImage _canTeachFlag;

		// Token: 0x040031D2 RID: 12754
		private CImage _leaderCanTeachFrame;
	}
}
