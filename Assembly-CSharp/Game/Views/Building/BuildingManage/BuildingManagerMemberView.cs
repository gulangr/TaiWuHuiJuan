using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF3 RID: 3059
	public class BuildingManagerMemberView : MonoBehaviour
	{
		// Token: 0x06009B73 RID: 39795 RVA: 0x0048D3EC File Offset: 0x0048B5EC
		public unsafe void Refresh(int index, int charId, BuildingBlockData blockData, VillagerRoleCharacterDisplayData villagerRoleData, CharacterDisplayData charData, ShopBuildingTeachBookData teachBookData, int upgradeQualification, int efficiency, bool isUnlocked, Action<int> onSelectChar, Action<int> onCancelChar, Action<int, bool> onSetUnlockChar)
		{
			this._charId = charId;
			bool exist = charId >= 0;
			this.rootExist.SetActive(exist);
			this.buttonSelect.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonChange.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonCancel.ClearAndAddListener(delegate
			{
				onCancelChar(index);
			});
			bool flag = !exist;
			if (!flag)
			{
				this.buttonAvatar.ClearAndAddListener(delegate
				{
					this.ShowCharacterMenu(charId);
				});
				this.InitMouseTipDisplayer();
				BuildingBlockItem buildingConfig = BuildingBlock.Instance[blockData.TemplateId];
				this.avatar.Refresh(charData, true);
				this.textName.text = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
				string skillName = string.Empty;
				int skillAttainment = 0;
				bool flag2 = buildingConfig.RequireLifeSkillType >= 0;
				if (flag2)
				{
					sbyte type = buildingConfig.RequireLifeSkillType;
					LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
					skillName = config.Name;
					skillAttainment = (int)(*villagerRoleData.LifeSkillAttainments[(int)type]);
				}
				else
				{
					bool flag3 = buildingConfig.RequireCombatSkillType >= 0;
					if (flag3)
					{
						sbyte type2 = buildingConfig.RequireCombatSkillType;
						CombatSkillTypeItem config2 = CombatSkillType.Instance[type2];
						skillName = config2.Name;
						skillAttainment = (int)(*villagerRoleData.CombatSkillAttainments[(int)type2]);
					}
				}
				string title = skillName + LanguageKey.LK_Attainment.Tr();
				this.propertyAttainment.Set(title, skillAttainment.ToString(), null);
				this.SetTeachBookTip(this.propertyAttainment.Tip, teachBookData);
				string title2 = LanguageKey.LK_Building_Arrangement_LeftPotential.Tr();
				string value = villagerRoleData.LeftPotentialCount.ToString();
				this.propertyPotential.Set(title2, value, null);
				this.SetTeachBookTip(this.propertyPotential.Tip, teachBookData);
				string title3 = LanguageKey.LK_Building_Shop_LearnedBookCount.Tr();
				string value2 = string.Format("{0}/{1}", teachBookData.MemberLearnedBookCount, teachBookData.LeaderCanTeachBookCount);
				this.propertyBookCount.Set(title3, value2, null);
				this.SetTeachBookTip(this.propertyBookCount.Tip, teachBookData);
				string title4 = LanguageKey.LK_Building_Shop_CharacterInfo_Efficency.Tr();
				efficiency = Math.Max(0, efficiency);
				string value3 = string.Format("{0}%", efficiency);
				this.propertyEfficiency.Set(title4, value3, null);
				this.buttonLock.gameObject.SetActive(!isUnlocked);
				this.buttonUnlock.gameObject.SetActive(isUnlocked);
				this.buttonLock.ClearAndAddListener(delegate
				{
					onSetUnlockChar(charId, true);
				});
				this.buttonUnlock.ClearAndAddListener(delegate
				{
					onSetUnlockChar(charId, false);
				});
				this.RefreshMarkUpgrade(upgradeQualification);
				this.RefreshMarkStudyBook(teachBookData);
				this.RefreshMarkChild(charData.PhysiologicalAge);
			}
		}

		// Token: 0x06009B74 RID: 39796 RVA: 0x0048D740 File Offset: 0x0048B940
		private void SetTeachBookTip(TooltipInvoker mouseTip, ShopBuildingTeachBookData teachBookData)
		{
			mouseTip.Type = TipType.BuildingTeachBook;
			if (mouseTip.RuntimeParam == null)
			{
				mouseTip.RuntimeParam = new ArgumentBox();
			}
			mouseTip.RuntimeParam.Set<ShopBuildingTeachBookData>("TeachBookData", teachBookData);
		}

		// Token: 0x06009B75 RID: 39797 RVA: 0x0048D783 File Offset: 0x0048B983
		private void RefreshMarkUpgrade(int upgradeQualification)
		{
			this.markUpgradeSmall.SetActive(upgradeQualification == 2);
			this.markUpgradeLarge.SetActive(upgradeQualification == 3);
		}

		// Token: 0x06009B76 RID: 39798 RVA: 0x0048D7A8 File Offset: 0x0048B9A8
		private void RefreshMarkStudyBook(ShopBuildingTeachBookData teachBookData)
		{
			sbyte teachBookResult = teachBookData.TeachBookResult;
			bool showMarkStudyBook = teachBookResult == 3 || teachBookResult == 4;
			this.markStudyBook.SetActive(showMarkStudyBook);
			bool flag = !showMarkStudyBook;
			if (!flag)
			{
				sbyte teachBookResult2 = teachBookData.TeachBookResult;
				if (!true)
				{
				}
				string text;
				if (teachBookResult2 != 3)
				{
					if (teachBookResult2 != 4)
					{
						if (!true)
						{
						}
						<PrivateImplementationDetails>.ThrowSwitchExpressionException(teachBookResult2);
					}
					else
					{
						text = LanguageKey.LK_Building_UnableToLearn.Tr();
					}
				}
				else
				{
					text = LanguageKey.LK_Building_FinishLearning.Tr();
				}
				if (!true)
				{
				}
				string content = text;
				this.tipStudyBook.Type = TipType.SingleDesc;
				this.tipStudyBook.PresetParam = new string[]
				{
					content
				};
			}
		}

		// Token: 0x06009B77 RID: 39799 RVA: 0x0048D858 File Offset: 0x0048BA58
		private void RefreshMarkChild(short age)
		{
			bool isChild = AgeGroup.GetAgeGroup(age) == 1;
			this.markChild.gameObject.SetActive(isChild);
		}

		// Token: 0x06009B78 RID: 39800 RVA: 0x0048D884 File Offset: 0x0048BA84
		private void ShowCharacterMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			argBox.Set("PreviousView", 12);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06009B79 RID: 39801 RVA: 0x0048D8D0 File Offset: 0x0048BAD0
		private void InitMouseTipDisplayer()
		{
			TooltipInvoker mouseTipDisplayer = this.tipChar;
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

		// Token: 0x06009B7A RID: 39802 RVA: 0x0048D94C File Offset: 0x0048BB4C
		public void SetForOperator(CharacterDisplayData charData, int index, Action<int> onSelectChar, Action<int> onCancelChar, IAsyncMethodRequestHandler handler, bool isUnlocked, Action<int, bool> onSetUnlockChar)
		{
			this._charId = ((charData != null) ? charData.CharacterId : -1);
			bool exist = this._charId >= 0;
			this.rootExist.SetActive(exist);
			this.buttonSelect.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonChange.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonCancel.ClearAndAddListener(delegate
			{
				onCancelChar(index);
			});
			this.propertyAttainment.gameObject.SetActive(false);
			this.propertyPotential.gameObject.SetActive(false);
			this.propertyBookCount.gameObject.SetActive(false);
			this.propertyEfficiency.gameObject.SetActive(false);
			this.identity.gameObject.SetActive(true);
			this.propertyWorkStatus.gameObject.SetActive(true);
			this.propertyPersonality.gameObject.SetActive(true);
			this.markUpgradeSmall.SetActive(false);
			this.markUpgradeLarge.SetActive(false);
			this.markStudyBook.SetActive(false);
			this.markChild.SetActive(false);
			this.buttonLock.gameObject.SetActive(false);
			this.buttonUnlock.gameObject.SetActive(false);
			bool flag = !exist;
			if (!flag)
			{
				this.buttonLock.gameObject.SetActive(!isUnlocked);
				this.buttonUnlock.gameObject.SetActive(isUnlocked);
				this.buttonLock.ClearAndAddListener(delegate
				{
					this.buttonLock.gameObject.SetActive(false);
					this.buttonUnlock.gameObject.SetActive(true);
					Action<int, bool> onSetUnlockChar2 = onSetUnlockChar;
					if (onSetUnlockChar2 != null)
					{
						onSetUnlockChar2(this._charId, true);
					}
				});
				this.buttonUnlock.ClearAndAddListener(delegate
				{
					this.buttonLock.gameObject.SetActive(true);
					this.buttonUnlock.gameObject.SetActive(false);
					Action<int, bool> onSetUnlockChar2 = onSetUnlockChar;
					if (onSetUnlockChar2 != null)
					{
						onSetUnlockChar2(this._charId, false);
					}
				});
				this.InitMouseTipDisplayer();
				this.buttonAvatar.ClearAndAddListener(delegate
				{
					this.ShowCharacterMenu(this._charId);
				});
				this.avatar.Refresh(charData, true);
				this.textName.text = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
				this.identity.Set(charData, false, true, false);
				this.RefreshWorkStatus(charData, handler);
				int personalitySum = charData.Personalities.GetSum();
				string personalityIcon = "ui9_icon_personality_big_0";
				this.propertyPersonality.Set(personalityIcon, LanguageKey.LK_Team_Tog_Personality.Tr(), personalitySum.ToString(), null, false);
			}
		}

		// Token: 0x06009B7B RID: 39803 RVA: 0x0048DBCC File Offset: 0x0048BDCC
		private void RefreshWorkStatus(CharacterDisplayData charData, IAsyncMethodRequestHandler handler)
		{
			BuildingManagerMemberView.<>c__DisplayClass32_0 CS$<>8__locals1 = new BuildingManagerMemberView.<>c__DisplayClass32_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.charId = charData.CharacterId;
			BuildingManagerMemberView.<>c__DisplayClass32_0 CS$<>8__locals2 = CS$<>8__locals1;
			int num = this._workStatusRequestVersion + 1;
			this._workStatusRequestVersion = num;
			CS$<>8__locals2.requestVersion = num;
			string title = LanguageKey.LK_WorkState.Tr();
			Dictionary<int, VillagerWorkData> villagerWork = SingletonObject.getInstance<BuildingModel>().VillagerWork;
			bool flag = !villagerWork.TryGetValue(CS$<>8__locals1.charId, out CS$<>8__locals1.workData);
			if (flag)
			{
				this.propertyWorkStatus.Set(title, LanguageKey.UI_VillagerWork_Idle.Tr(), null);
			}
			else
			{
				bool flag2 = CS$<>8__locals1.workData.WorkType == 0 || CS$<>8__locals1.workData.WorkType == 1;
				if (flag2)
				{
					BuildingManagerMemberView.<>c__DisplayClass32_1 CS$<>8__locals3 = new BuildingManagerMemberView.<>c__DisplayClass32_1();
					CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals1;
					BuildingManagerMemberView.<>c__DisplayClass32_1 CS$<>8__locals4 = CS$<>8__locals3;
					bool isRemove;
					if (CS$<>8__locals3.CS$<>8__locals1.workData.WorkType == 0 && CS$<>8__locals3.CS$<>8__locals1.workData.BuildingBlockIndex >= 0)
					{
						BuildingBlockData taiwuBuildingData = SingletonObject.getInstance<BuildingModel>().GetTaiwuBuildingData(new BuildingBlockKey(CS$<>8__locals3.CS$<>8__locals1.workData.AreaId, CS$<>8__locals3.CS$<>8__locals1.workData.BlockId, CS$<>8__locals3.CS$<>8__locals1.workData.BuildingBlockIndex));
						isRemove = (taiwuBuildingData != null && taiwuBuildingData.OperationType == 1);
					}
					else
					{
						isRemove = false;
					}
					CS$<>8__locals4.isRemove = isRemove;
					string workStatusText = CS$<>8__locals3.isRemove ? LanguageKey.LK_Building_RemoveTip2.Tr() : LanguageKey.UI_VillagerWork_Build.Tr();
					this.propertyWorkStatus.Set(title, workStatusText, null);
					Location workLocation = new Location(CS$<>8__locals3.CS$<>8__locals1.workData.AreaId, CS$<>8__locals3.CS$<>8__locals1.workData.BlockId);
					MapDomainMethod.AsyncCall.IsLocationInBuildingEffectRange(handler, charData.Location, workLocation, delegate(int offset, RawDataPool pool)
					{
						bool flag5 = CS$<>8__locals3.CS$<>8__locals1.requestVersion != CS$<>8__locals3.CS$<>8__locals1.<>4__this._workStatusRequestVersion || CS$<>8__locals3.CS$<>8__locals1.<>4__this._charId != CS$<>8__locals3.CS$<>8__locals1.charId;
						if (!flag5)
						{
							bool inRange = false;
							Serializer.Deserialize(pool, offset, ref inRange);
							bool flag6 = !inRange;
							if (flag6)
							{
								CS$<>8__locals3.CS$<>8__locals1.<>4__this.propertyWorkStatus.SetValue(LanguageKey.UI_VillagerWork_Move.Tr());
							}
							else
							{
								bool flag7 = CS$<>8__locals3.CS$<>8__locals1.workData.WorkType == 0;
								if (flag7)
								{
									CS$<>8__locals3.CS$<>8__locals1.<>4__this.propertyWorkStatus.SetValue(CS$<>8__locals3.isRemove ? LanguageKey.LK_Building_RemoveTip2.Tr() : LanguageKey.UI_VillagerWork_Build.Tr());
								}
								else
								{
									CS$<>8__locals3.CS$<>8__locals1.<>4__this.propertyWorkStatus.SetValue(LocalStringManager.Get(string.Format("LK_WorkType_{0}", CS$<>8__locals3.CS$<>8__locals1.workData.WorkType)));
								}
							}
						}
					});
				}
				else
				{
					bool flag3 = CS$<>8__locals1.workData.WorkType >= 10;
					if (flag3)
					{
						bool flag4 = CS$<>8__locals1.workData.AreaId == charData.Location.AreaId && CS$<>8__locals1.workData.BlockId == charData.Location.BlockId;
						if (flag4)
						{
							this.propertyWorkStatus.Set(title, LocalStringManager.Get(string.Format("LK_WorkType_{0}", CS$<>8__locals1.workData.WorkType)), null);
						}
						else
						{
							this.propertyWorkStatus.Set(title, LanguageKey.UI_VillagerWork_Move.Tr(), null);
						}
					}
					else
					{
						this.propertyWorkStatus.Set(title, LanguageKey.UI_VillagerWork_Idle.Tr(), null);
					}
				}
			}
		}

		// Token: 0x06009B7C RID: 39804 RVA: 0x0048DE78 File Offset: 0x0048C078
		public void SetForResident(CharacterDisplayData charData, int index, bool isUnlocked, Action<int> onSelectChar, Action<int> onCancelChar, Action<int, bool> onSetUnlockChar)
		{
			this.identity.gameObject.SetActive(false);
			this.propertyWorkStatus.gameObject.SetActive(false);
			this.propertyPersonality.gameObject.SetActive(false);
			this._charId = ((charData != null) ? charData.CharacterId : -1);
			bool exist = this._charId >= 0;
			this.rootExist.SetActive(exist);
			this.buttonSelect.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonChange.ClearAndAddListener(delegate
			{
				onSelectChar(index);
			});
			this.buttonCancel.ClearAndAddListener(delegate
			{
				onCancelChar(index);
			});
			bool flag = !exist;
			if (!flag)
			{
				this.InitMouseTipDisplayer();
				this.buttonAvatar.ClearAndAddListener(delegate
				{
					this.ShowCharacterMenu(this._charId);
				});
				this.avatar.Refresh(charData, true);
				this.textName.text = NameCenter.GetNameByDisplayData(charData, false, false);
				this.propertyAttainment.Set(CommonUtils.GetHappinessIconName(HappinessType.GetHappinessType(charData.Happiness)), LanguageKey.LK_VillagerInfo_Happiness.Tr(), CommonUtils.GetHappinessString(HappinessType.GetHappinessType(charData.Happiness)), null, false);
				this.propertyPotential.Set(CommonUtils.GetFavorabilityIconName(charData.FavorabilityToTaiwu, true), LanguageKey.LK_Favorability.Tr(), CommonUtils.GetFavorStringByInteracted(charData.FavorabilityToTaiwu, true), null, false);
				this.propertyBookCount.SetValue(CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)charData.TemplateId, charData.OrgInfo, charData.Gender, charData.PhysiologicalAge, false));
				this.buttonLock.gameObject.SetActive(isUnlocked);
				this.buttonUnlock.gameObject.SetActive(!isUnlocked);
				this.buttonLock.ClearAndAddListener(delegate
				{
					onSetUnlockChar(this._charId, false);
				});
				this.buttonUnlock.ClearAndAddListener(delegate
				{
					onSetUnlockChar(this._charId, true);
				});
			}
		}

		// Token: 0x06009B7D RID: 39805 RVA: 0x0048E0A4 File Offset: 0x0048C2A4
		public void Refresh(CharacterDisplayData charData)
		{
			this._charId = charData.CharacterId;
			this.buttonAvatar.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charData.CharacterId);
			});
			this.avatar.Refresh(charData, true);
			bool isTaiwu = charData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.textName.text = NameCenter.GetMonasticTitleOrDisplayName(charData, isTaiwu);
			this.InitMouseTipDisplayer();
			this.buttonLock.gameObject.SetActive(false);
			this.buttonUnlock.gameObject.SetActive(false);
			this.propertyAttainment.gameObject.SetActive(false);
			this.propertyEfficiency.gameObject.SetActive(false);
			this.propertyPotential.gameObject.SetActive(false);
			this.propertyBookCount.gameObject.SetActive(false);
			this.buttonSelect.gameObject.SetActive(false);
			this.buttonChange.gameObject.SetActive(false);
			this.buttonCancel.gameObject.SetActive(false);
			this.markChild.gameObject.SetActive(false);
			this.markStudyBook.gameObject.SetActive(false);
			this.markUpgradeLarge.gameObject.SetActive(false);
			this.markUpgradeSmall.gameObject.SetActive(false);
		}

		// Token: 0x0400786D RID: 30829
		[SerializeField]
		private TooltipInvoker tipChar;

		// Token: 0x0400786E RID: 30830
		[SerializeField]
		private CButton buttonUnlock;

		// Token: 0x0400786F RID: 30831
		[SerializeField]
		private CButton buttonLock;

		// Token: 0x04007870 RID: 30832
		[SerializeField]
		private CButton buttonSelect;

		// Token: 0x04007871 RID: 30833
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04007872 RID: 30834
		[SerializeField]
		private CButton buttonChange;

		// Token: 0x04007873 RID: 30835
		[SerializeField]
		private CButton buttonAvatar;

		// Token: 0x04007874 RID: 30836
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007875 RID: 30837
		[SerializeField]
		private GameObject rootExist;

		// Token: 0x04007876 RID: 30838
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04007877 RID: 30839
		[SerializeField]
		private PropertyItem propertyAttainment;

		// Token: 0x04007878 RID: 30840
		[SerializeField]
		private PropertyItem propertyPotential;

		// Token: 0x04007879 RID: 30841
		[SerializeField]
		private PropertyItem propertyBookCount;

		// Token: 0x0400787A RID: 30842
		[SerializeField]
		private PropertyItem propertyEfficiency;

		// Token: 0x0400787B RID: 30843
		[SerializeField]
		private GameObject markUpgradeSmall;

		// Token: 0x0400787C RID: 30844
		[SerializeField]
		private GameObject markUpgradeLarge;

		// Token: 0x0400787D RID: 30845
		[SerializeField]
		private GameObject markStudyBook;

		// Token: 0x0400787E RID: 30846
		[SerializeField]
		private TooltipInvoker tipStudyBook;

		// Token: 0x0400787F RID: 30847
		[SerializeField]
		private GameObject markChild;

		// Token: 0x04007880 RID: 30848
		[Header("操作人手专用（SetForOperator）")]
		[SerializeField]
		private Identity identity;

		// Token: 0x04007881 RID: 30849
		[SerializeField]
		private PropertyItem propertyWorkStatus;

		// Token: 0x04007882 RID: 30850
		[SerializeField]
		private PropertyItem propertyPersonality;

		// Token: 0x04007883 RID: 30851
		private int _charId;

		// Token: 0x04007884 RID: 30852
		private int _workStatusRequestVersion;
	}
}
