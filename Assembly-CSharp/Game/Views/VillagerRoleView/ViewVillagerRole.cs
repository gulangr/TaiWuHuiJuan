using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Coffee.UIExtensions;
using Config;
using DisplayConfig;
using FrameWork;
using FrameWork.UISystem.Components.EffectPlayer;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Building.BuildingManage;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.VillagerRole;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000737 RID: 1847
	public class ViewVillagerRole : UIBase
	{
		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x060058B8 RID: 22712 RVA: 0x00292935 File Offset: 0x00290B35
		private YieldHelper _yieldHelper
		{
			get
			{
				return SingletonObject.getInstance<YieldHelper>();
			}
		}

		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x060058B9 RID: 22713 RVA: 0x0029293C File Offset: 0x00290B3C
		private short RoleKey
		{
			get
			{
				return (short)this.roleToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x060058BA RID: 22714 RVA: 0x0029294A File Offset: 0x00290B4A
		public bool IsFulongSpecialInteractOpen
		{
			get
			{
				return this._activeChicken;
			}
		}

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x060058BB RID: 22715 RVA: 0x00292952 File Offset: 0x00290B52
		private ViewVillagerRole.EVillagerRolePage _CurrentPage
		{
			get
			{
				return this.slideBook.CurrentAvtiveIndex + ViewVillagerRole.EVillagerRolePage.RoleDescription;
			}
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x00292961 File Offset: 0x00290B61
		private void Awake()
		{
			this.PageAwake();
		}

		// Token: 0x060058BD RID: 22717 RVA: 0x0029296C File Offset: 0x00290B6C
		private void PageAwake()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.InitPage();
				this.LineageAwake();
				this.RoleManageAwake();
				this.ChickenPageAwake();
				this._inited = true;
				this.chickenPageIndexer.SetButtonAction(delegate
				{
					this.SetPageActive(ViewVillagerRole.EVillagerRolePage.ChickenAssign, true);
				});
				this.roleAssignPageIndexer.SetButtonAction(delegate
				{
					this.SetPageActive(ViewVillagerRole.EVillagerRolePage.RoleAssign, true);
				});
				this.descPageIndexer.SetButtonAction(delegate
				{
					this.SetPageActive(ViewVillagerRole.EVillagerRolePage.RoleDescription, true);
				});
			}
		}

		// Token: 0x060058BE RID: 22718 RVA: 0x002929F4 File Offset: 0x00290BF4
		public override void OnInit(ArgumentBox argsBox)
		{
			this.PageAwake();
			Enum targetRolePage;
			argsBox.Get("EnterPage", out targetRolePage);
			this._targetRolePage = (ViewVillagerRole.EVillagerRolePage)targetRolePage;
			bool flag = this._targetRolePage == ViewVillagerRole.EVillagerRolePage.None;
			if (flag)
			{
				this._targetRolePage = ViewVillagerRole.EVillagerRolePage.RoleDescription;
			}
			this.LineagePageInit(argsBox);
			this.RoleManagePageInit(argsBox);
			this.ChickenPageInit(argsBox);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x060058BF RID: 22719 RVA: 0x00292A78 File Offset: 0x00290C78
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(126);
			this.SetPageActive(this._targetRolePage, false);
			this.SetMainTitle(this._CurrentPage);
			this.SetEnableIndexerInteractAll(true);
			this.RoleManagePageEnable();
			this.LineageOnEnable();
			this.ChickenPageEnable();
		}

		// Token: 0x060058C0 RID: 22720 RVA: 0x00292AC6 File Offset: 0x00290CC6
		private void OnDisable()
		{
			this.LineageOnDisable();
			this.RoleManagePageDisable();
			this.ChickenPageDisable();
		}

		// Token: 0x060058C1 RID: 22721 RVA: 0x00292ADE File Offset: 0x00290CDE
		private void InitPage()
		{
			this.BindUIElement();
			this.InitRoleToggleGroup();
		}

		// Token: 0x060058C2 RID: 22722 RVA: 0x00292AEF File Offset: 0x00290CEF
		public override void InitMonitorFieldIds()
		{
			this.LineageInitMonitorFieldIds();
			this.RoleManagePageInitMonitorFieldIds();
			this.ChickenPageMonitorFieldIds();
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x00292B07 File Offset: 0x00290D07
		private void BindUIElement()
		{
			this.slideBook.Init();
			this.slideBook.OnPageSwitchComplete = new Action<int, int>(this.OnPageSwitch);
		}

		// Token: 0x060058C4 RID: 22724 RVA: 0x00292B30 File Offset: 0x00290D30
		private void OnPageSwitch(int previousPage, int currentPage)
		{
			bool flag = previousPage == 2;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.OnAssignChickenToVillagerRole, null);
			}
			bool flag2 = this._CurrentPage == ViewVillagerRole.EVillagerRolePage.RoleAssign;
			if (flag2)
			{
				bool openSelectVillager = this._openSelectVillager;
				if (openSelectVillager)
				{
					this._openSelectVillager = false;
					this.ClickVillagerBtn();
				}
			}
			bool flag3 = this._CurrentPage == ViewVillagerRole.EVillagerRolePage.RoleDescription;
			if (flag3)
			{
				bool openSendVillager = this._openSendVillager;
				if (openSendVillager)
				{
					this._openSendVillager = false;
					this.OpenDispatchPanel();
				}
			}
			bool flag4 = this._CurrentPage == ViewVillagerRole.EVillagerRolePage.ChickenAssign;
			if (flag4)
			{
				this.ChickenAssignEnable();
			}
			this.SetMainTitle(this._CurrentPage);
			this.SetEnableIndexerInteractAll(true);
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x00292BDC File Offset: 0x00290DDC
		private void InitRoleToggleGroup()
		{
			this.roleToggleGroup.Init(-1);
			this.roleToggleGroup.OnActiveIndexChange += this.OnToggleChange;
			for (int i = 0; i < VillagerRole.Instance.Count; i++)
			{
				ToggleStyle togStyle = this.roleToggleGroup.Get(i).GetComponent<ToggleStyle>();
				VillagerRoleUtils.AsyncSetRoleName(togStyle.Label, (short)i, this, true, null);
			}
			GameObject taiwuToggle = this.roleToggleGroup.transform.GetChild(this.roleToggleGroup.transform.childCount - 1).gameObject;
			this.roleToggleGroup.SetInteractable(false, this.roleToggleGroup.Count() - 1);
			this.taiwuLabel.SetText(OrganizationMember.Instance[10].GradeName, true);
			this.buttonClosePopup.ClearAndAddListener(new Action(this.CleanUp));
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x00292CC5 File Offset: 0x00290EC5
		private void CleanUp()
		{
			this.LinageCleanUp();
			this.ChickenCleanUp();
			this.QuickHide();
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x00292CDD File Offset: 0x00290EDD
		private void OnToggleChange(int newTog, int oldTog)
		{
			this.OnToggleChangeLineage(newTog, oldTog);
			this.ToggleChangeRoleManage(newTog);
			this.OnChangeRoleToggleChicken(newTog, oldTog);
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x00292CFC File Offset: 0x00290EFC
		protected override void OnClick(Transform btn)
		{
			bool isAnimating = this.slideBook.IsAnimating;
			if (!isAnimating)
			{
				bool flag = btn.name == "AssignRole";
				if (flag)
				{
					this.ClickVillagerBtn();
					bool flag2 = this._CurrentPage != ViewVillagerRole.EVillagerRolePage.RoleAssign;
					if (flag2)
					{
						this.SetPageActive(ViewVillagerRole.EVillagerRolePage.RoleAssign, true);
					}
				}
				else
				{
					bool flag3 = btn.name == "SendVillager";
					if (flag3)
					{
						bool flag4 = !CommonUtils.CheckRoleDispatch(this.RoleKey);
						if (flag4)
						{
							this.roleToggleGroup.Set(0, false);
						}
						this.OpenDispatchPanel();
						bool flag5 = this._CurrentPage != ViewVillagerRole.EVillagerRolePage.RoleDescription;
						if (flag5)
						{
							this.SetPageActive(ViewVillagerRole.EVillagerRolePage.RoleDescription, true);
						}
					}
					else
					{
						bool flag6 = btn.name == "DispatchButton";
						if (flag6)
						{
							this.OpenDispatchPanel();
							bool flag7 = this._CurrentPage != ViewVillagerRole.EVillagerRolePage.RoleAssign;
							if (flag7)
							{
								this.SetPageActive(ViewVillagerRole.EVillagerRolePage.RoleAssign, true);
							}
						}
					}
				}
				CButton btnComponent = btn.GetComponent<CButton>();
				this.LineageOnClick(btnComponent);
				this.RoleManageClick(btnComponent);
				this.ChickenPageOnClick(btnComponent);
			}
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x00292E19 File Offset: 0x00291019
		private void OnListenerIdReady()
		{
			this.roleToggleGroup.Set(0, false);
			TaiwuDomainMethod.Call.GetVillagerRoleCharacterDisplayDataOnPanel(this.Element.GameDataListenerId);
			OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 14, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool dataPool)
			{
				bool isOpen = false;
				Serializer.Deserialize(dataPool, offset, ref isOpen);
				this._activeChicken = isOpen;
				this.chickenPageIndexer.SetInteractable(isOpen);
				this.slideBook.SetToggleInteractable(2, isOpen);
			});
			this.ChickenPageListenerIdReady();
		}

		// Token: 0x060058CA RID: 22730 RVA: 0x00292E58 File Offset: 0x00291058
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			this.LineageOnNotifyGameData(notifications);
			this.RoleManagePageNotifyGameData(notifications);
			this.ChickenPageNotifyGameData(notifications);
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x00292E74 File Offset: 0x00291074
		private void SetPageActive(ViewVillagerRole.EVillagerRolePage pageType, bool isAniamted)
		{
			bool flag = pageType == ViewVillagerRole.EVillagerRolePage.RoleAssign;
			if (!flag)
			{
				bool flag2 = pageType == ViewVillagerRole.EVillagerRolePage.RoleDescription;
				if (flag2)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(100);
				}
			}
			this.SetEnableIndexerInteractAll(!isAniamted);
			this.slideBook.SetDisplayPage(pageType - ViewVillagerRole.EVillagerRolePage.RoleDescription, isAniamted);
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x00292EBC File Offset: 0x002910BC
		private void SetEnableIndexerInteractAll(bool enableHover)
		{
			this.chickenPageIndexer.SetEnableIndexerInteract(enableHover && this._CurrentPage != ViewVillagerRole.EVillagerRolePage.ChickenAssign);
			this.roleAssignPageIndexer.SetEnableIndexerInteract(enableHover && this._CurrentPage != ViewVillagerRole.EVillagerRolePage.RoleAssign);
			this.descPageIndexer.SetEnableIndexerInteract(enableHover && this._CurrentPage != ViewVillagerRole.EVillagerRolePage.RoleDescription);
		}

		// Token: 0x060058CD RID: 22733 RVA: 0x00292F24 File Offset: 0x00291124
		private void SetEnableIndexerInteract(Refers refers, bool enableInteract)
		{
			refers.CGet<PointerTrigger>("PageIndexDescTrigger").enabled = enableInteract;
			refers.CGet<CButtonObsolete>("PageIndexBtn").interactable = enableInteract;
			bool flag = !enableInteract;
			if (flag)
			{
				refers.CGet<GameObject>("HoverObj").SetActive(false);
			}
		}

		// Token: 0x060058CE RID: 22734 RVA: 0x00292F74 File Offset: 0x00291174
		private void SetMainTitle(ViewVillagerRole.EVillagerRolePage pageType)
		{
			bool flag = pageType == ViewVillagerRole.EVillagerRolePage.ChickenAssign;
			LanguageKey titleId;
			if (flag)
			{
				titleId = LanguageKey.LK_AssignChicken_Title;
			}
			else
			{
				bool flag2 = pageType == ViewVillagerRole.EVillagerRolePage.RoleAssign;
				if (flag2)
				{
					titleId = LanguageKey.LK_Building_RoleAssign;
				}
				else
				{
					titleId = LanguageKey.LK_Building_RoleDescription;
				}
			}
			this.titleTxt.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerRole_MainTitle, LocalStringManager.Get(LanguageKey.LK_Building_TaiwuVillageLineage_Name), LocalStringManager.Get(titleId));
		}

		// Token: 0x060058CF RID: 22735 RVA: 0x00292FD4 File Offset: 0x002911D4
		private bool IsChickenInVillage(int chickenId)
		{
			return this._villageChickenIdList.Contains(chickenId);
		}

		// Token: 0x060058D0 RID: 22736 RVA: 0x00292FE2 File Offset: 0x002911E2
		private void ChickenPageAwake()
		{
			this.ChickenPageInitRefers();
			this.btnQuickAssign.ClearAndAddListener(new Action(this.OnQuickAssign));
		}

		// Token: 0x060058D1 RID: 22737 RVA: 0x00293004 File Offset: 0x00291204
		private void OnQuickAssign()
		{
			BuildingDomainMethod.Call.QuickAssignChicken(this.SelectedOrgMemberId);
			this.GetChickenList();
		}

		// Token: 0x060058D2 RID: 22738 RVA: 0x0029301A File Offset: 0x0029121A
		public void ChickenPageInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			this._selectedRoleIndex = 0;
			AudioManager.Instance.PlaySound("SFX_assignchicken_open", false, false);
		}

		// Token: 0x060058D3 RID: 22739 RVA: 0x0029303D File Offset: 0x0029123D
		private void ChickenPageListenerIdReady()
		{
			this.GetChickenList();
		}

		// Token: 0x060058D4 RID: 22740 RVA: 0x00293047 File Offset: 0x00291247
		private void ChickenPageEnable()
		{
		}

		// Token: 0x060058D5 RID: 22741 RVA: 0x0029304C File Offset: 0x0029124C
		private void ChickenAssignEnable()
		{
			this.unlockStateParticle.gameObject.SetActive(this._isExtraEffectUnlocked);
			bool isExtraEffectUnlocked = this._isExtraEffectUnlocked;
			if (isExtraEffectUnlocked)
			{
				this.unlockStateParticle.Play();
			}
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x00293089 File Offset: 0x00291289
		private void ChickenPageDisable()
		{
		}

		// Token: 0x060058D7 RID: 22743 RVA: 0x0029308C File Offset: 0x0029128C
		private void OnChangeRoleToggleChicken(int newTog, int oldTog)
		{
			this._selectedRoleIndex = newTog;
			this._roleEffectValues.Clear();
			bool flag = this._sectFulongOrgMemberChickens != null;
			if (flag)
			{
				this.RefreshContentWithSelectedOrgMemberId(true);
			}
		}

		// Token: 0x060058D8 RID: 22744 RVA: 0x002930C4 File Offset: 0x002912C4
		private void RefreshContentWithSelectedOrgMemberId(bool rebuildRoleEffectValues = false)
		{
			bool flag = this._chickenList == null;
			if (!flag)
			{
				VillagerRoleItem config = VillagerRole.Instance[this._selectedRoleIndex];
				this.RefreshContentWithRole(config);
				NeedPersonality[] needPersonalityList = this.GetSelectedRoleNeedPersonalityList();
				this.RefreshSelectedRoleEffectArea(needPersonalityList, config, rebuildRoleEffectValues);
				this.roleImage.SetSprite(string.Format("VillagerRole_badge_0_{0}", this._selectedRoleIndex), false, null);
			}
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x00293130 File Offset: 0x00291330
		private void OnPreviewSelected(List<int> previewSelectedIds)
		{
			this._isInEffectPreviewMode = true;
			this._previewSelectedChickenIds = previewSelectedIds;
			VillagerRoleItem config = VillagerRole.Instance[this._selectedRoleIndex];
			NeedPersonality[] needPersonalityList = this.GetSelectedRoleNeedPersonalityList();
			this.RefreshSelectedRoleEffectArea(needPersonalityList, config, false);
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x00293170 File Offset: 0x00291370
		private void RefreshSelectedRoleEffectArea(NeedPersonality[] needPersonalityList, VillagerRoleItem config, bool rebuildRoleEffectValues = false)
		{
			List<ViewVillagerRole.ChickenPersonality> chickenHasPersonalityList = this.CalculateCurrentChickenHasPersonalities(needPersonalityList, this._isInEffectPreviewMode);
			this.RefreshSelectedRolePersonality(needPersonalityList, chickenHasPersonalityList);
			this.RefreshSelectedRolePersonalityCost(needPersonalityList, chickenHasPersonalityList);
			this.RefreshSelectedRoleEffects(config, chickenHasPersonalityList, rebuildRoleEffectValues);
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x002931A8 File Offset: 0x002913A8
		private void RefreshContentWithRole(VillagerRoleItem config)
		{
			this.RefreshChickenSlots();
		}

		// Token: 0x060058DC RID: 22748 RVA: 0x002931B4 File Offset: 0x002913B4
		private void RefreshChickenSlots()
		{
			for (int i = 0; i < this.chickenSlotList.Count; i++)
			{
				this.RefreshChickenSlot(i);
			}
		}

		// Token: 0x060058DD RID: 22749 RVA: 0x002931E8 File Offset: 0x002913E8
		private void RefreshChickenSlot(int index)
		{
			List<int> chickenIdList = this.SelectedChickenIdList;
			bool flag = chickenIdList == null || index >= chickenIdList.Count;
			if (flag)
			{
				this.chickenSlotList[index].RefreshEmpty(true);
				this.chickenSlotList[index].RefreshButton(new Action<int>(this.OnChickenClick));
			}
			else
			{
				int chickenId = chickenIdList[index];
				bool isInVillage = this._chickenList.FindIndex((GameData.Domains.Building.Chicken c) => c.Id == chickenId) != -1;
				GameData.Domains.Building.Chicken chicken = this._chickenDict[chickenId];
				string nickName = this._nickNameDict[chickenId];
				SelectableChickenItem chickenSlot = this.chickenSlotList[index];
				chickenSlot.Refresh(index, chicken, nickName, false, !this.IsChickenInVillage(chickenId), new Action<int>(this.OnChickenClick), new Action<int>(this.OnFeedChickenClick), new Action<int>(this.OnClearSingleChicken), true);
			}
		}

		// Token: 0x060058DE RID: 22750 RVA: 0x002932F4 File Offset: 0x002914F4
		private void RefreshSelectedRolePersonality(NeedPersonality[] needPersonalityTypeList, List<ViewVillagerRole.ChickenPersonality> chickenPersonalities)
		{
			ViewVillagerRole.<>c__DisplayClass76_0 CS$<>8__locals1 = new ViewVillagerRole.<>c__DisplayClass76_0();
			CS$<>8__locals1.needPersonalityTypeList = needPersonalityTypeList;
			int i;
			int j;
			for (i = 0; i < CS$<>8__locals1.needPersonalityTypeList.Length; i = j + 1)
			{
				PersonalityItem personalityConfig = Personality.Instance[(int)CS$<>8__locals1.needPersonalityTypeList[i].PersonalityType];
				bool flag = i >= this.personalityIconList.Count;
				if (flag)
				{
					break;
				}
				this.personalityIconList[i].SetSprite(personalityConfig.Icon, false, null);
				this.personalityNameLabelList[i].text = LocalStringManager.Get(personalityConfig.Name).SetColor(CommonUtils.GetPersonalityColor((int)CS$<>8__locals1.needPersonalityTypeList[i].PersonalityType));
				int total = (from e in chickenPersonalities
				where e.Type == CS$<>8__locals1.needPersonalityTypeList[i].PersonalityType
				select e).Sum((ViewVillagerRole.ChickenPersonality e) => e.Value);
				this.personalityLabelList[i].text = string.Format("+{0}", total);
				j = i;
			}
		}

		// Token: 0x060058DF RID: 22751 RVA: 0x00293468 File Offset: 0x00291668
		private void RefreshSelectedRolePersonalityCost(NeedPersonality[] needPersonalityList, List<ViewVillagerRole.ChickenPersonality> chickenPersonalities)
		{
			int uiIndex = 0;
			for (int j = 0; j < needPersonalityList.Length; j++)
			{
				NeedPersonality needPersonality = needPersonalityList[j];
				int chickenCount = chickenPersonalities.Count((ViewVillagerRole.ChickenPersonality e) => e.Type == needPersonality.PersonalityType);
				for (int i = 0; i < needPersonality.NeedCount; i++)
				{
					bool flag = uiIndex >= this.needPersonalityIconList.Count;
					if (flag)
					{
						break;
					}
					PersonalityItem personalityConfig = Personality.Instance[(int)needPersonality.PersonalityType];
					this.needPersonalityIconList[uiIndex].SetSprite(personalityConfig.Icon, false, null);
					bool isActive = i < chickenCount;
					this.needPersonalityIconList[uiIndex].color = (isActive ? Color.white : Color.gray);
					this.needPersonalityIconBackList[uiIndex].SetSprite(isActive ? "popup_assignchicken_circular_0" : "popup_assignchicken_circular_1", false, null);
					uiIndex++;
				}
			}
		}

		// Token: 0x060058E0 RID: 22752 RVA: 0x00293584 File Offset: 0x00291784
		private void RefreshSelectedRoleEffects(VillagerRoleItem config, List<ViewVillagerRole.ChickenPersonality> chickenHasPersonalityList, bool rebuildRoleEffectValues = false)
		{
			this.PrepareEnoughEffectItems(config.EffectTextList.Length);
			bool isExtraEffectUnlocked = ViewVillagerRole.IsRoleExtraEffectUnlocked(config.NeedPersonalityList, chickenHasPersonalityList);
			this.RefreshSelectedRoleEffectLockDesc(isExtraEffectUnlocked);
			this.HandleUnlockAndLoopEffect(isExtraEffectUnlocked);
			int[] extraIndices = config.ExtraEffectIndices;
			List<int> indices = EasyPool.Get<List<int>>();
			indices.Clear();
			bool flag = isExtraEffectUnlocked;
			if (flag)
			{
				for (int i = 0; i < config.EffectTextList.Length; i++)
				{
					indices.Add(i);
				}
			}
			else
			{
				for (int j = 0; j < config.EffectTextList.Length; j++)
				{
					bool flag2 = extraIndices.Contains(j);
					if (!flag2)
					{
						indices.Add(j);
					}
				}
				indices.AddRange(extraIndices);
			}
			while (this._roleEffectValues.Count < config.EffectTextList.Length)
			{
				this._roleEffectValues.Add(-1);
			}
			bool hasNumberUpdateParticle = false;
			for (int k = 0; k < config.EffectTextList.Length; k++)
			{
				int index = indices[k];
				bool isExtra = extraIndices.Contains(index);
				float[] factors = config.EffectDisplayValueList[index];
				float value = this.CalculateEffectValue(config.NeedPersonalityList, factors, chickenHasPersonalityList);
				Refers item = this.villagerRoleEffectItemTemplate.transform.parent.GetChild(k).GetComponent<Refers>();
				item.CGet<CImage>("Bg").enabled = (k % 2 == 1);
				TextMeshProUGUI effectDesc = item.CGet<TextMeshProUGUI>("EffectDesc");
				TextMeshProUGUI effectValue = item.CGet<TextMeshProUGUI>("EffectValue");
				UIParticle roleEffectUnlockParticle = item.CGet<UIParticle>("RoleEffectUnlockParticle");
				UIParticle roleEffectNumberParticle = item.CGet<UIParticle>("RoleEffectNumberParticle");
				for (int l = 0; l < config.NeedPersonalityList.Length; l++)
				{
					CImage icon = item.CGet<CImage>(string.Format("Icon_{0}", l + 1));
					bool hasValue = (double)Math.Abs(config.EffectDisplayValueList[index][l] - 0f) > 0.001;
					icon.gameObject.SetActive(hasValue);
					bool flag3 = hasValue;
					if (flag3)
					{
						PersonalityItem personalityConfig = Personality.Instance[(int)config.NeedPersonalityList[l].PersonalityType];
						icon.SetSprite(personalityConfig.Icon, false, null);
					}
				}
				bool isEffectItemNearVisible = this.IsEffectItemNearVisible(item.GetComponent<RectTransform>());
				bool isLockedExtra = isExtra && !isExtraEffectUnlocked;
				bool flag4 = !isLockedExtra && isEffectItemNearVisible;
				if (flag4)
				{
					this.HandleRoleItemEffect(rebuildRoleEffectValues, value, index, roleEffectNumberParticle, ref hasNumberUpdateParticle);
				}
				bool needPlayEffect = this._needPlayEffect;
				if (needPlayEffect)
				{
					bool flag5 = isExtra;
					if (flag5)
					{
						bool flag6 = isExtraEffectUnlocked && !this._isExtraEffectUnlocked && isEffectItemNearVisible;
						if (flag6)
						{
							this._particlePlayHelper.PlayOnceParticle(roleEffectUnlockParticle, 1.5f, null);
						}
					}
					else
					{
						bool flag7 = chickenHasPersonalityList.Count == 1 && isEffectItemNearVisible && value != 0f;
						if (flag7)
						{
							this._particlePlayHelper.PlayOnceParticle(roleEffectUnlockParticle, 1.5f, null);
						}
					}
				}
				effectDesc.text = config.EffectTextList[index];
				effectValue.gameObject.SetActive(!isLockedExtra && value != 0f);
				effectValue.text = string.Format(config.EffectValueTextList[index], Math.Floor((double)value)).ColorReplace();
			}
			bool flag8 = hasNumberUpdateParticle;
			if (flag8)
			{
				AudioManager.Instance.PlaySound("SFX_assignchicken_shengji", false, false);
			}
			bool flag9 = !this._isInEffectPreviewMode;
			if (flag9)
			{
				this._isExtraEffectUnlocked = isExtraEffectUnlocked;
			}
			bool needPlayEffect2 = this._needPlayEffect;
			if (needPlayEffect2)
			{
				this._needPlayEffect = false;
			}
		}

		// Token: 0x060058E1 RID: 22753 RVA: 0x0029394C File Offset: 0x00291B4C
		private bool IsEffectItemNearVisible(RectTransform item)
		{
			Vector3[] itemCorners = new Vector3[4];
			item.GetComponent<RectTransform>().GetWorldCorners(itemCorners);
			Vector3[] viewportCorners = new Vector3[4];
			RectTransform viewport = item.transform.parent.parent.GetComponent<RectTransform>();
			viewport.GetWorldCorners(viewportCorners);
			float itemHeight = itemCorners[1].y - itemCorners[0].y;
			float visibleHeight = itemHeight;
			bool flag = itemCorners[1].y > viewportCorners[1].y;
			if (flag)
			{
				visibleHeight -= itemCorners[1].y - viewportCorners[1].y;
			}
			bool flag2 = itemCorners[0].y < viewportCorners[0].y;
			if (flag2)
			{
				visibleHeight -= viewportCorners[0].y - itemCorners[0].y;
			}
			visibleHeight = Mathf.Max(0f, visibleHeight);
			return (double)visibleHeight >= (double)itemHeight * 0.6;
		}

		// Token: 0x060058E2 RID: 22754 RVA: 0x00293A58 File Offset: 0x00291C58
		private void HandleRoleItemEffect(bool rebuildRoleEffectValues, float effectValue, int index, UIParticle roleEffectNumberParticle, ref bool hasNumberUpdateParticle)
		{
			int newValue = (int)Math.Abs(Math.Floor((double)effectValue));
			bool needPlayEffect = this._needPlayEffect;
			if (needPlayEffect)
			{
				int oldValue = this._roleEffectValues[index];
				bool flag = newValue > oldValue;
				if (flag)
				{
					this._particlePlayHelper.PlayOnceParticle(roleEffectNumberParticle, 1.5f, null);
					hasNumberUpdateParticle = true;
				}
			}
			bool flag2 = this._needPlayEffect || rebuildRoleEffectValues;
			if (flag2)
			{
				this._roleEffectValues[index] = newValue;
			}
		}

		// Token: 0x060058E3 RID: 22755 RVA: 0x00293AD0 File Offset: 0x00291CD0
		private void HandleUnlockAndLoopEffect(bool isExtraEffectUnlocked)
		{
			bool needPlayUnlockOnceEffect = this._needPlayEffect && isExtraEffectUnlocked && !this._isExtraEffectUnlocked;
			bool flag = !needPlayUnlockOnceEffect;
			if (flag)
			{
				this._particlePlayHelper.SetActiveParticle(this.unlockStateParticle, isExtraEffectUnlocked && !this._isInEffectPreviewMode, true);
			}
			bool flag2 = needPlayUnlockOnceEffect;
			if (flag2)
			{
				this.disableClick.SetActive(true);
				AudioManager.Instance.PlaySound("SFX_assignchicken_daming", false, false);
				this._particlePlayHelper.PlayOnceParticle(this.unlockParticle, 1.5f, delegate
				{
					this.disableClick.SetActive(false);
				});
				Coroutine newCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.9f, delegate
				{
					this._particlePlayHelper.SetActiveParticle(this.unlockStateParticle, true, true);
				});
			}
		}

		// Token: 0x060058E4 RID: 22756 RVA: 0x00293B84 File Offset: 0x00291D84
		private void RefreshSelectedRoleEffectLockDesc(bool isExtraEffectUnlocked)
		{
			OrganizationMemberItem selectedOrgMemberConfig = ViewVillagerRole.GetOrgMemberConfig(this.SelectedRoleConfig.TemplateId);
			NeedPersonality need0 = this.SelectedRoleConfig.NeedPersonalityList[0];
			sbyte p0 = need0.PersonalityType;
			PersonalityItem personalityConfig0 = Personality.Instance[(int)p0];
			OrganizationMemberItem orgConfig = OrganizationMember.Instance[this.SelectedRoleConfig.OrganizationMember];
			VillagerRoleUtils.AsyncGetRoleName(this.SelectedRoleConfig.TemplateId, this, delegate(string roleName)
			{
				this.effectLockDesc.text = (isExtraEffectUnlocked ? LocalStringManager.GetFormat(LanguageKey.LK_AssignChicken_EffectDesc, roleName.SetGradeColor((int)orgConfig.Grade)).SetColor("brightblue") : LocalStringManager.GetFormat(LanguageKey.LK_AssignChicken_EffectLockedDesc, personalityConfig0.Name.SetColor(CommonUtils.GetPersonalityColor((int)p0)), roleName.SetGradeColor((int)orgConfig.Grade)).SetColor("brightyellow"));
				this.effectLockDesc.GetComponent<TMPTextSpriteHelper>().Parse();
			}, true);
		}

		// Token: 0x060058E5 RID: 22757 RVA: 0x00293C28 File Offset: 0x00291E28
		private void PrepareEnoughEffectItems(int count)
		{
			Refers template = this.villagerRoleEffectItemTemplate;
			Transform parent = template.transform.parent;
			for (int i = count; i < parent.childCount; i++)
			{
				parent.GetChild(i).gameObject.SetActive(false);
			}
			for (int j = 0; j < count; j++)
			{
				bool flag = j < parent.childCount;
				if (flag)
				{
					parent.GetChild(j).gameObject.SetActive(true);
				}
				else
				{
					Refers refers = Object.Instantiate<Refers>(template, template.transform.parent);
					refers.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x060058E6 RID: 22758 RVA: 0x00293CD3 File Offset: 0x00291ED3
		private VillagerRoleItem SelectedRoleConfig
		{
			get
			{
				return VillagerRole.Instance[this._selectedRoleIndex];
			}
		}

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x060058E7 RID: 22759 RVA: 0x00293CE5 File Offset: 0x00291EE5
		private short SelectedOrgMemberId
		{
			get
			{
				return this.SelectedRoleConfig.OrganizationMember;
			}
		}

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x060058E8 RID: 22760 RVA: 0x00293CF2 File Offset: 0x00291EF2
		private List<int> SelectedChickenIdList
		{
			get
			{
				return this._sectFulongOrgMemberChickens.ContainsKey(this.SelectedOrgMemberId) ? this._sectFulongOrgMemberChickens[this.SelectedOrgMemberId].Items : null;
			}
		}

		// Token: 0x060058E9 RID: 22761 RVA: 0x00293D20 File Offset: 0x00291F20
		private List<ViewVillagerRole.ChickenPersonality> CalculateCurrentChickenHasPersonalities(NeedPersonality[] needPersonalityList, bool isPreview = false)
		{
			this._cachedChickenPersonalityList.Clear();
			IEnumerable<int> chickenIdList = isPreview ? this._previewSelectedChickenIds : this.SelectedChickenIdList;
			bool flag = chickenIdList == null;
			List<ViewVillagerRole.ChickenPersonality> cachedChickenPersonalityList;
			if (flag)
			{
				cachedChickenPersonalityList = this._cachedChickenPersonalityList;
			}
			else
			{
				foreach (NeedPersonality needPersonality in needPersonalityList)
				{
					foreach (int chickenId in chickenIdList)
					{
						GameData.Domains.Building.Chicken chicken = this._chickenDict[chickenId];
						bool flag2 = chicken.TemplateId < 0;
						if (!flag2)
						{
							bool flag3 = !this.IsChickenInVillage(chickenId);
							if (!flag3)
							{
								Config.ChickenItem chickenConfig = Config.Chicken.Instance[chicken.TemplateId];
								bool flag4 = chickenConfig.PersonalityType != needPersonality.PersonalityType;
								if (!flag4)
								{
									this._cachedChickenPersonalityList.Add(new ViewVillagerRole.ChickenPersonality
									{
										ChickenId = chickenId,
										Type = chickenConfig.PersonalityType,
										Value = chickenConfig.PersonalityValue
									});
								}
							}
						}
					}
				}
				cachedChickenPersonalityList = this._cachedChickenPersonalityList;
			}
			return cachedChickenPersonalityList;
		}

		// Token: 0x060058EA RID: 22762 RVA: 0x00293E74 File Offset: 0x00292074
		private NeedPersonality[] GetSelectedRoleNeedPersonalityList()
		{
			VillagerRoleItem config = VillagerRole.Instance[this._selectedRoleIndex];
			return config.NeedPersonalityList;
		}

		// Token: 0x060058EB RID: 22763 RVA: 0x00293EA0 File Offset: 0x002920A0
		private bool IsPersonalityFitSelectedOrgMemberId(sbyte personalityType)
		{
			NeedPersonality[] needPersonalityList = this.GetSelectedRoleNeedPersonalityList();
			return needPersonalityList.Any((NeedPersonality e) => e.PersonalityType == personalityType);
		}

		// Token: 0x060058EC RID: 22764 RVA: 0x00293ED8 File Offset: 0x002920D8
		private static OrganizationMemberItem GetOrgMemberConfig(short roleId)
		{
			VillagerRoleItem config = VillagerRole.Instance[roleId];
			return OrganizationMember.Instance[config.OrganizationMember];
		}

		// Token: 0x060058ED RID: 22765 RVA: 0x00293F08 File Offset: 0x00292108
		private static Comparison<GameData.Domains.Building.Chicken> MakeChickenComparison(sbyte selectedPersonalityType, HashSet<int> selectedChichenIds)
		{
			return delegate(GameData.Domains.Building.Chicken chicken1, GameData.Domains.Building.Chicken chicken2)
			{
				Config.ChickenItem config = Config.Chicken.Instance[chicken1.TemplateId];
				Config.ChickenItem config2 = Config.Chicken.Instance[chicken2.TemplateId];
				bool personalityMatch = config.PersonalityType == selectedPersonalityType;
				bool personalityMatch2 = config2.PersonalityType == selectedPersonalityType;
				bool selected = selectedChichenIds.Contains(chicken1.Id);
				bool selected2 = selectedChichenIds.Contains(chicken2.Id);
				bool flag = selected && !selected2;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					bool flag2 = !selected && selected2;
					if (flag2)
					{
						result = 1;
					}
					else
					{
						bool flag3 = personalityMatch && !personalityMatch2;
						if (flag3)
						{
							result = -1;
						}
						else
						{
							bool flag4 = !personalityMatch && personalityMatch2;
							if (flag4)
							{
								result = 1;
							}
							else
							{
								int valueCompare = config2.PersonalityValue.CompareTo(config.PersonalityValue);
								bool flag5 = valueCompare != 0;
								if (flag5)
								{
									result = valueCompare;
								}
								else
								{
									result = chicken1.Id.CompareTo(chicken2.Id);
								}
							}
						}
					}
				}
				return result;
			};
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x00293F3C File Offset: 0x0029213C
		private float CalculateEffectValue(NeedPersonality[] personalities, float[] factors, List<ViewVillagerRole.ChickenPersonality> chickenHasPersonalityList)
		{
			ViewVillagerRole.<>c__DisplayClass96_0 CS$<>8__locals1 = new ViewVillagerRole.<>c__DisplayClass96_0();
			CS$<>8__locals1.personalities = personalities;
			float effectValue = 0f;
			int j;
			int i;
			for (j = 0; j < CS$<>8__locals1.personalities.Length; j = i + 1)
			{
				int chickenTotalPersonalityValue = (from e in chickenHasPersonalityList
				where e.Type == CS$<>8__locals1.personalities[j].PersonalityType
				select e).Sum((ViewVillagerRole.ChickenPersonality e) => e.Value);
				effectValue += (float)chickenTotalPersonalityValue * factors[j];
				i = j;
			}
			return effectValue;
		}

		// Token: 0x060058EF RID: 22767 RVA: 0x00293FF0 File Offset: 0x002921F0
		private static bool IsRoleExtraEffectUnlocked(NeedPersonality[] personalities, List<ViewVillagerRole.ChickenPersonality> chickenHasPersonalityList)
		{
			for (int i = 0; i < personalities.Length; i++)
			{
				int needChickenCount = personalities[i].NeedCount;
				int count = 0;
				foreach (ViewVillagerRole.ChickenPersonality chickenPersonality in chickenHasPersonalityList)
				{
					bool flag = chickenPersonality.Type == personalities[i].PersonalityType;
					if (flag)
					{
						count++;
					}
				}
				bool flag2 = count < needChickenCount;
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060058F0 RID: 22768 RVA: 0x0029409C File Offset: 0x0029229C
		private void GetChickenList()
		{
			bool flag = this._sectFulongOrgMemberChickens == null;
			if (!flag)
			{
				Location location = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
				BuildingDomainMethod.Call.GetSettlementChickenIdList(this.Element.GameDataListenerId, location);
			}
		}

		// Token: 0x060058F1 RID: 22769 RVA: 0x002940D6 File Offset: 0x002922D6
		public void ChickenPageMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 132, ulong.MaxValue, null));
		}

		// Token: 0x060058F2 RID: 22770 RVA: 0x002940F4 File Offset: 0x002922F4
		public void ChickenPageNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						this.ChickenPageHandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
					}
				}
				else
				{
					this.ChickenPageHandleDataModification(notification.Uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}

		// Token: 0x060058F3 RID: 22771 RVA: 0x0029419C File Offset: 0x0029239C
		private void ChickenPageHandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
		{
			if (domainId == 9)
			{
				switch (methodId)
				{
				case 135:
				{
					bool success = false;
					Serializer.Deserialize(dataPool, offset, ref success);
					break;
				}
				case 136:
				{
					bool success2 = false;
					Serializer.Deserialize(dataPool, offset, ref success2);
					break;
				}
				case 137:
				{
					Serializer.Deserialize(dataPool, offset, ref this._chickenList);
					bool flag = this._chickenList == null;
					if (!flag)
					{
						for (int i = 0; i < this._chickenList.Count; i++)
						{
							this._chickenDict[this._chickenList[i].Id] = this._chickenList[i];
						}
						this.RemoveInvalidChickenData();
					}
					break;
				}
				case 138:
				{
					List<string> nickNames = new List<string>();
					Serializer.Deserialize(dataPool, offset, ref nickNames);
					for (int j = 0; j < nickNames.Count; j++)
					{
						this._nickNameDict[this._chickenList[j].Id] = nickNames[j];
					}
					this.RefreshContentWithSelectedOrgMemberId(false);
					this.Element.ShowAfterRefresh();
					break;
				}
				case 139:
				{
					Serializer.Deserialize(dataPool, offset, ref this._villageChickenIdList);
					HashSet<int> allChickenIdSet = new HashSet<int>(this._villageChickenIdList);
					foreach (KeyValuePair<short, IntList> kv in this._sectFulongOrgMemberChickens)
					{
						bool flag2 = kv.Value.Items != null;
						if (flag2)
						{
							foreach (int chickenId2 in kv.Value.Items)
							{
								allChickenIdSet.Add(chickenId2);
							}
						}
					}
					List<int> idList = allChickenIdSet.ToList<int>();
					BuildingDomainMethod.Call.GetChickenDataList(this.Element.GameDataListenerId, idList);
					BuildingDomainMethod.Call.GetChickenNicknameList(this.Element.GameDataListenerId, idList);
					break;
				}
				default:
					if (methodId == 238)
					{
						Serializer.Deserialize(dataPool, offset, ref this._returnFavor);
						int chickenId = this.SelectedChickenIdList[this._feedChickenIndex];
						bool isInVillage = this._chickenList.FindIndex((GameData.Domains.Building.Chicken c) => c.Id == chickenId) != -1;
						GameData.Domains.Building.Chicken chicken = this._chickenDict[chickenId];
						chicken.Happiness += this._returnFavor;
						this._chickenDict[chickenId] = chicken;
						this.RefreshChickenSlot(this._feedChickenIndex);
					}
					break;
				}
			}
		}

		// Token: 0x060058F4 RID: 22772 RVA: 0x002944A0 File Offset: 0x002926A0
		private void RemoveInvalidChickenData()
		{
			foreach (KeyValuePair<short, IntList> kv in this._sectFulongOrgMemberChickens)
			{
				List<int> items = kv.Value.Items;
				if (items != null)
				{
					items.RemoveAll((int e) => !this._chickenDict.ContainsKey(e));
				}
			}
		}

		// Token: 0x060058F5 RID: 22773 RVA: 0x00294518 File Offset: 0x00292718
		private void ChickenPageHandleDataModification(DataUid uid, int offset, RawDataPool dataPool)
		{
			ushort domainId = uid.DomainId;
			ushort num = domainId;
			if (num == 19)
			{
				ushort dataId = uid.DataId;
				ushort num2 = dataId;
				if (num2 == 132)
				{
					bool flag = this._sectFulongOrgMemberChickens == null;
					if (flag)
					{
						this._sectFulongOrgMemberChickens = new Dictionary<short, IntList>();
					}
					Serializer.DeserializeModifications<short>(dataPool, offset, this._sectFulongOrgMemberChickens);
					this.villagerRoleInfoDispatch.RefreshByChickUnlock();
					this.GetChickenList();
				}
			}
		}

		// Token: 0x060058F6 RID: 22774 RVA: 0x0029458C File Offset: 0x0029278C
		private void RefreshDebug()
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			foreach (KeyValuePair<short, IntList> kv in this._sectFulongOrgMemberChickens)
			{
				sb.Append(kv.Key);
				sb.AppendLine(":");
				bool flag = kv.Value.Items == null;
				if (!flag)
				{
					foreach (int chickenId in kv.Value.Items)
					{
						sb.AppendFormat("    {0}", chickenId);
						sb.AppendLine();
					}
				}
			}
		}

		// Token: 0x060058F7 RID: 22775 RVA: 0x0029467C File Offset: 0x0029287C
		protected void ChickenPageOnClick(CButton btn)
		{
		}

		// Token: 0x060058F8 RID: 22776 RVA: 0x00294680 File Offset: 0x00292880
		private void OnChickenClick(int index)
		{
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("Callback", new Action<List<int>>(this.OnSelectedChicken));
			argsBox.SetObject("CancelCallback", new Action(delegate
			{
				this.darkMask.SetActive(false);
				this._isInEffectPreviewMode = false;
				this.RefreshContentWithSelectedOrgMemberId(false);
			}));
			argsBox.SetObject("PreviewCallback", new Action<List<int>>(this.OnPreviewSelected));
			argsBox.SetObject("PersonalitySortTypes", (from e in this.SelectedRoleConfig.NeedPersonalityList
			select e.PersonalityType).ToList<sbyte>());
			List<GameData.Domains.Building.Chicken> chickenDataList = (this._chickenList != null) ? this._chickenList.Where(delegate(GameData.Domains.Building.Chicken e)
			{
				Config.ChickenItem chickenConfig = Config.Chicken.Instance[e.TemplateId];
				bool flag3 = chickenConfig.TemplateId == 63;
				return !flag3 && this.IsPersonalityFitSelectedOrgMemberId(chickenConfig.PersonalityType);
			}).ToList<GameData.Domains.Building.Chicken>() : new List<GameData.Domains.Building.Chicken>();
			argsBox.SetObject("ChickenList", chickenDataList);
			argsBox.Set("MaxSelectCount", this.chickenSlotList.Count);
			List<int> selectedChickenIdList = this.SelectedChickenIdList;
			bool flag = selectedChickenIdList != null && selectedChickenIdList.Count > 0;
			if (flag)
			{
				argsBox.SetObject("SelectedChickenIdList", this.SelectedChickenIdList);
			}
			argsBox.SetObject("PersonalitySortTypes", (from e in this.SelectedRoleConfig.NeedPersonalityList
			select e.PersonalityType).ToList<sbyte>());
			argsBox.SetObject("ComparisonMaker", new UI_SelectChicken.ComparisonMaker(ViewVillagerRole.MakeChickenComparison));
			Dictionary<int, short> chickenIdToRoleDict = new Dictionary<int, short>();
			foreach (KeyValuePair<short, IntList> pair in this._sectFulongOrgMemberChickens)
			{
				bool flag2 = pair.Value.Items == null;
				if (!flag2)
				{
					foreach (int chickenId in pair.Value.Items)
					{
						chickenIdToRoleDict[chickenId] = VillagerRoleUtils.GetRoleIdFromOrgMemberId(pair.Key);
					}
				}
			}
			argsBox.SetObject("ChickenIdToRoleDict", chickenIdToRoleDict);
			UIElement.SelectChicken.SetOnInitArgs(argsBox);
			UIManager.Instance.ShowUI(UIElement.SelectChicken, true);
			this._particlePlayHelper.SetActiveParticle(this.unlockStateParticle, false, false);
			this.darkMask.SetActive(true);
		}

		// Token: 0x060058F9 RID: 22777 RVA: 0x00294900 File Offset: 0x00292B00
		private void OnSelectedChicken(List<int> chickenIds)
		{
			this.darkMask.SetActive(false);
			this._isInEffectPreviewMode = false;
			this._needPlayEffect = true;
			short orgId = this.SelectedOrgMemberId;
			Dictionary<short, IntList> dict = this._sectFulongOrgMemberChickens;
			bool called = false;
			foreach (KeyValuePair<short, IntList> kv in dict)
			{
				bool flag = kv.Key == orgId;
				if (flag)
				{
					bool flag2 = kv.Value.Items == null;
					if (!flag2)
					{
						foreach (int chickenId in kv.Value.Items)
						{
							bool flag3 = !chickenIds.Contains(chickenId);
							if (flag3)
							{
								called = true;
								BuildingDomainMethod.Call.UnsetFulongChicken(this.Element.GameDataListenerId, kv.Key, chickenId);
							}
						}
					}
				}
				else
				{
					foreach (int chickenId2 in chickenIds)
					{
						bool flag4 = kv.Value.Items != null && kv.Value.Items.Contains(chickenId2);
						if (flag4)
						{
							called = true;
							BuildingDomainMethod.Call.UnsetFulongChicken(this.Element.GameDataListenerId, kv.Key, chickenId2);
						}
					}
				}
			}
			List<int> currentChickenIdList = this.SelectedChickenIdList;
			foreach (int chickenId3 in chickenIds)
			{
				bool flag5 = currentChickenIdList == null || !currentChickenIdList.Contains(chickenId3);
				if (flag5)
				{
					called = true;
					BuildingDomainMethod.Call.SetFulongChicken(this.Element.GameDataListenerId, orgId, chickenId3);
				}
			}
			bool flag6 = !called;
			if (flag6)
			{
				this.RefreshContentWithSelectedOrgMemberId(false);
			}
		}

		// Token: 0x060058FA RID: 22778 RVA: 0x00294B60 File Offset: 0x00292D60
		private void OnClearSingleChicken(int index)
		{
			int chicken = this.SelectedChickenIdList[index];
			BuildingDomainMethod.Call.UnsetFulongChicken(this.Element.GameDataListenerId, this.SelectedOrgMemberId, chicken);
		}

		// Token: 0x060058FB RID: 22779 RVA: 0x00294B94 File Offset: 0x00292D94
		private void OnFeedChickenClick(int index)
		{
			this._feedChickenIndex = index;
			bool flag = this.SelectedChickenIdList != null && this.SelectedChickenIdList.Count > index;
			if (flag)
			{
				int chickenId = this.SelectedChickenIdList[index];
				short templateId = this._chickenDict[chickenId].TemplateId;
				Game.Views.Building.BuildingManage.ChickenCommonHelper.SelectItemAndFeedChicken(this.Element.GameDataListenerId, this, (int)templateId, null);
			}
		}

		// Token: 0x060058FC RID: 22780 RVA: 0x00294BFB File Offset: 0x00292DFB
		private void ChickenCleanUp()
		{
			this.disableClick.SetActive(false);
			this.darkMask.SetActive(false);
		}

		// Token: 0x060058FD RID: 22781 RVA: 0x00294C18 File Offset: 0x00292E18
		private void ChickenPageInitRefers()
		{
			foreach (SelectableChickenItem item in this.chickenSlotList)
			{
				item.Init();
			}
		}

		// Token: 0x060058FE RID: 22782 RVA: 0x00294C70 File Offset: 0x00292E70
		private void LineageAwake()
		{
			this.InitRefers();
			this.currentVillagerScroll.OnSelectDataChange = new Action(this.RefreshRemoveAllButton);
			this.currentVillagerScroll.OnRemoveVillagerRole = new Action<int>(this.OnClickRemoveSingle);
			this.btnQuickRemoveAll.ClearAndAddListener(delegate
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_VillagerRole_ConfirmRemove),
					Content = LocalStringManager.Get(LanguageKey.LK_VillagerRole_ConfirmRemoveDescription),
					Yes = delegate()
					{
						foreach (int item in this.currentVillagerScroll.SelectedCharId)
						{
							TaiwuDomainMethod.Call.SetVillagerRole(-1, item, -1);
						}
						this.GetDisplayData();
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			});
		}

		// Token: 0x060058FF RID: 22783 RVA: 0x00294CCC File Offset: 0x00292ECC
		private void OnClickRemoveSingle(int charId)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
				Content = LocalStringManager.Get(LanguageKey.LK_Building_RemoveVillagerRoleConfirm),
				Yes = delegate()
				{
					TaiwuDomainMethod.Call.SetVillagerRole(this.Element.GameDataListenerId, charId, -1);
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06005900 RID: 22784 RVA: 0x00294D51 File Offset: 0x00292F51
		private void LineageOnDisable()
		{
			this._searchInputText = "";
		}

		// Token: 0x06005901 RID: 22785 RVA: 0x00294D5F File Offset: 0x00292F5F
		private void LineageOnEnable()
		{
			this.villageListToggleGroup.Set(0, false);
		}

		// Token: 0x06005902 RID: 22786 RVA: 0x00294D70 File Offset: 0x00292F70
		private void LineagePageInit(ArgumentBox argsBox)
		{
			this._enterType = ViewVillagerRole.EnterType.Normal;
			bool flag = argsBox != null;
			if (flag)
			{
				Enum enterType;
				bool flag2 = argsBox.Get("EnterType", out enterType);
				if (flag2)
				{
					this._enterType = (ViewVillagerRole.EnterType)enterType;
				}
				bool flag3 = argsBox.Get("TargetRoleTemplateId", out this._targetRoleTemplateId);
				if (flag3)
				{
					this._needAutoJumpPage = true;
				}
			}
			this.NeedDataListenerId = true;
			this.GetDisplayData();
		}

		// Token: 0x06005903 RID: 22787 RVA: 0x00294DD6 File Offset: 0x00292FD6
		private void GetDisplayData()
		{
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayDataRolePage(null, delegate(int offset, RawDataPool dataPool)
			{
				List<VillagerCharDisplayData> VillagerCharDisplayDataList = new List<VillagerCharDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref VillagerCharDisplayDataList);
				this.HandleVillagerDisplayData(VillagerCharDisplayDataList);
				this.RefreshPage();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06005904 RID: 22788 RVA: 0x00294DEC File Offset: 0x00292FEC
		public void LineageInitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
			{
				34U
			}));
		}

		// Token: 0x06005905 RID: 22789 RVA: 0x00294E18 File Offset: 0x00293018
		public unsafe void LineageOnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						bool flag = notification.DomainId == 5;
						if (flag)
						{
							ushort methodId = notification.MethodId;
							bool flag2 = methodId == 121 || methodId == 111;
							if (flag2)
							{
								this.GetDisplayData();
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag3 = uid.DomainId == 4 && uid.DataId == 0;
					if (flag3)
					{
						bool flag4 = uid.SubId1 == 34U;
						if (flag4)
						{
							ResourceInts resources;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref resources);
							this._taiwuAuthorityCount = *(ref resources.Items.FixedElementField + (IntPtr)7 * 4);
							this.RefreshAuthorityCostAndQuickRemoveAll();
						}
					}
				}
			}
		}

		// Token: 0x06005906 RID: 22790 RVA: 0x00294F38 File Offset: 0x00293138
		private void LineageOnClick(CButton btn)
		{
		}

		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06005907 RID: 22791 RVA: 0x00294F3C File Offset: 0x0029313C
		private List<VillagerCharDisplayData> _activeSearchVillagerList
		{
			get
			{
				int activeIndex = this.villageListToggleGroup.GetActiveIndex();
				if (!true)
				{
				}
				List<VillagerCharDisplayData> result;
				switch (activeIndex)
				{
				case 0:
					result = this._searchCurrentVillagerList;
					break;
				case 1:
					result = this._searchHistoryVillagerList;
					break;
				case 2:
					result = this._searchLostVillagerList;
					break;
				default:
					result = this._searchCurrentVillagerList;
					break;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x06005908 RID: 22792 RVA: 0x00294F98 File Offset: 0x00293198
		private void HandleVillagerDisplayData(List<VillagerCharDisplayData> VillagerCharDisplayDataList)
		{
			this.ResetVillagerList();
			for (int i = 0; i < VillagerCharDisplayDataList.Count; i++)
			{
				VillagerCharDisplayData data = VillagerCharDisplayDataList[i];
				bool flag = data.RoleTemplateId < 0;
				if (!flag)
				{
					bool flag2 = (data.Flags & 1) > 0;
					if (flag2)
					{
						this._currentVillagerList[(int)data.RoleTemplateId].Add(data);
					}
					else
					{
						bool flag3 = (data.Flags & 2) > 0;
						if (flag3)
						{
							this._lostVillagerList[(int)data.RoleTemplateId].Add(data);
						}
						else
						{
							this._historyVillagerList[(int)data.RoleTemplateId].Add(data);
						}
					}
				}
			}
		}

		// Token: 0x06005909 RID: 22793 RVA: 0x00295049 File Offset: 0x00293249
		private void ResetVillagerList()
		{
			ViewVillagerRole.ResetVillagerList(this._historyVillagerList);
			ViewVillagerRole.ResetVillagerList(this._currentVillagerList);
			ViewVillagerRole.ResetVillagerList(this._lostVillagerList);
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x00295070 File Offset: 0x00293270
		private static void ResetVillagerList(List<VillagerCharDisplayData>[] villagerList)
		{
			for (int i = 0; i < villagerList.Length; i++)
			{
				List<VillagerCharDisplayData> t = villagerList[i];
				bool flag = t == null;
				if (flag)
				{
					t = new List<VillagerCharDisplayData>();
					villagerList[i] = t;
				}
				else
				{
					t.Clear();
				}
			}
		}

		// Token: 0x0600590B RID: 22795 RVA: 0x002950B8 File Offset: 0x002932B8
		private void RefreshPage()
		{
			this.roleToggleGroup.Set((this._needAutoJumpPage && this._targetRoleTemplateId != -1) ? this._targetRoleTemplateId : ((int)this.RoleKey), true);
			this._needAutoJumpPage = false;
			this.FilterSortAndRefresh(false, false);
			this.RefreshAuthorityCostAndQuickRemoveAll();
		}

		// Token: 0x0600590C RID: 22796 RVA: 0x0029510C File Offset: 0x0029330C
		private void RefreshAuthorityCostAndQuickRemoveAll()
		{
			int currentVillagerCount = (this._currentVillagerList[(int)this.RoleKey] == null) ? 0 : this._currentVillagerList[(int)this.RoleKey].Count;
			int lostVillagerCount = (this._lostVillagerList[(int)this.RoleKey] == null) ? 0 : this._lostVillagerList[(int)this.RoleKey].Count;
			int villagerCount = currentVillagerCount + lostVillagerCount;
			int cost = GameData.Domains.Taiwu.VillagerRole.SharedMethods.CalcSetVillagerRoleAuthorityCost(villagerCount, this.RoleKey);
			bool isEnoughAuthority = this._taiwuAuthorityCount >= cost;
			this.authorityCost.SetText(CommonUtils.GetDisplayStringForNum(this._taiwuAuthorityCount, 100000).SetColor(isEnoughAuthority ? "brightblue" : "brightred") + "/" + CommonUtils.GetDisplayStringForNum(cost, 100000).SetColor("pinkyellow"), true);
			this.setVillagerBtn.interactable = isEnoughAuthority;
			this.setVillagerBtnInner.interactable = isEnoughAuthority;
			this.RefreshRemoveAllButton();
		}

		// Token: 0x0600590D RID: 22797 RVA: 0x002951F7 File Offset: 0x002933F7
		private void RefreshRemoveAllButton()
		{
			this.btnQuickRemoveAll.interactable = (this.currentVillagerScroll.SelectedCharId.Count > 0);
		}

		// Token: 0x0600590E RID: 22798 RVA: 0x00295219 File Offset: 0x00293419
		private void ClickVillagerBtn()
		{
			TaiwuDomainMethod.AsyncCall.GetVillagersAvailableForVillagerRole(this, false, delegate(int offset, RawDataPool dataPool)
			{
				List<int> charIdList = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref charIdList);
				TaiwuDomainMethod.AsyncCall.GetVillagersForWorkDisplayData(this, charIdList, delegate(int offset, RawDataPool pool)
				{
					List<VillagerSelectCharacterDisplayData> displayData = new List<VillagerSelectCharacterDisplayData>();
					Serializer.Deserialize(pool, offset, ref displayData);
					List<ISelectCharacterData> selectList = (from item in displayData
					select new VillagerSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
					CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.AssignRolePeasant + (int)this.RoleKey);
					config.InteractionMode = ESelectCharacterInteractionMode.Instant;
					config.SelectionMode = ESelectCharacterSelectionMode.Multiple;
					config.TargetCount = selectList.Count;
					config.CustomTextGenerator = new Func<IReadOnlyList<int>, string>(this.GenerateCustomCostText);
					config.Title = LanguageKey.LK_Building_TaiwuVillageLineage_GrantBtn.Tr();
					config.ConfirmInteractableChecker = new Func<IReadOnlyList<int>, bool>(this.CheckConfirmInteractable);
					config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
					{
						ESelectCharacterFilterMenuId.Gender,
						ESelectCharacterFilterMenuId.BehaviorType,
						ESelectCharacterFilterMenuId.Relation,
						ESelectCharacterFilterMenuId.AdoreRelation,
						ESelectCharacterFilterMenuId.EnemyRelation,
						ESelectCharacterFilterMenuId.WorkStatus,
						ESelectCharacterFilterMenuId.RoleArrangementWork,
						ESelectCharacterFilterMenuId.Identity
					};
					UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(this.OnSelectedVillager)));
					UIManager.Instance.MaskUI(UIElement.SelectChar);
				});
			});
		}

		// Token: 0x0600590F RID: 22799 RVA: 0x00295230 File Offset: 0x00293430
		private bool CheckConfirmInteractable(IReadOnlyList<int> list)
		{
			int costResult = this.GetAssignCost(list);
			return this._taiwuAuthorityCount >= costResult && list != null && list.Count > 0;
		}

		// Token: 0x06005910 RID: 22800 RVA: 0x00295264 File Offset: 0x00293464
		private string GenerateCustomCostText(IReadOnlyList<int> list)
		{
			int costResult = this.GetAssignCost(list);
			bool isEnoughAuthority = this._taiwuAuthorityCount >= costResult;
			this._sb.Clear();
			this._sb.Append(LanguageKey.LK_EventWindow_FameActionAuthorityCost.Tr());
			this._sb.Append(LanguageKey.LK_Colon_Symbol.Tr());
			this._sb.Append(CommonUtils.GetDisplayStringForNum(this._taiwuAuthorityCount, 100000).SetColor(isEnoughAuthority ? "brightblue" : "brightred"));
			this._sb.Append("/");
			this._sb.Append(CommonUtils.GetDisplayStringForNum(costResult, 100000).SetColor("pinkyellow"));
			return this._sb.ToString();
		}

		// Token: 0x06005911 RID: 22801 RVA: 0x00295330 File Offset: 0x00293530
		private int GetAssignCost(IReadOnlyList<int> list)
		{
			int currentVillagerCount = (this._currentVillagerList[(int)this.RoleKey] == null) ? 0 : this._currentVillagerList[(int)this.RoleKey].Count;
			int lostVillagerCount = (this._lostVillagerList[(int)this.RoleKey] == null) ? 0 : this._lostVillagerList[(int)this.RoleKey].Count;
			int villagerCount = currentVillagerCount + lostVillagerCount;
			int costResult = 0;
			for (int i = 0; i < list.Count; i++)
			{
				int cost = GameData.Domains.Taiwu.VillagerRole.SharedMethods.CalcSetVillagerRoleAuthorityCost(villagerCount + i, this.RoleKey);
				costResult += cost;
			}
			return costResult;
		}

		// Token: 0x06005912 RID: 22802 RVA: 0x002953C8 File Offset: 0x002935C8
		private int GetCurrValue(HashSet<int> selectedCharIdList)
		{
			int totalCost = 0;
			int currentVillagerCount = (this._currentVillagerList[(int)this.RoleKey] == null) ? 0 : this._currentVillagerList[(int)this.RoleKey].Count;
			int lostVillagerCount = (this._lostVillagerList[(int)this.RoleKey] == null) ? 0 : this._lostVillagerList[(int)this.RoleKey].Count;
			int baseCount = currentVillagerCount + lostVillagerCount;
			for (int i = 0; i < selectedCharIdList.Count; i++)
			{
				totalCost += GameData.Domains.Taiwu.VillagerRole.SharedMethods.CalcSetVillagerRoleAuthorityCost(baseCount + i, this.RoleKey);
			}
			return totalCost;
		}

		// Token: 0x06005913 RID: 22803 RVA: 0x0029545C File Offset: 0x0029365C
		private bool FilterSelectedVillager(VillagerCharDisplayData displayData)
		{
			bool flag = this._currentVillagerList == null;
			return flag || this._currentVillagerList[(int)this.RoleKey].All((VillagerCharDisplayData v) => v.CharacterId != displayData.CharacterId);
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x002954AA File Offset: 0x002936AA
		private void RefreshDataCount(bool clearSelected = false, bool rebuildListStructure = false)
		{
			this.currentVillagerScroll.SetData(this._activeSearchVillagerList, this.RoleKey, this.villageListToggleGroup.GetActiveIndex() == 0, clearSelected, rebuildListStructure);
			this.RefreshCount();
		}

		// Token: 0x06005915 RID: 22805 RVA: 0x002954DC File Offset: 0x002936DC
		private void OnSelectedVillager(int selectedVillagerId)
		{
			bool needConfirm = false;
			short roleIndex = 0;
			Func<VillagerCharDisplayData, bool> <>9__0;
			while ((int)roleIndex < this._currentVillagerList.Length)
			{
				bool flag = roleIndex == this.RoleKey;
				if (!flag)
				{
					IEnumerable<VillagerCharDisplayData> source = this._currentVillagerList[(int)roleIndex];
					Func<VillagerCharDisplayData, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((VillagerCharDisplayData t) => t.CharacterId == selectedVillagerId));
					}
					bool flag2 = source.Any(predicate);
					if (flag2)
					{
						needConfirm = true;
						break;
					}
				}
				roleIndex += 1;
			}
			bool flag3 = needConfirm;
			if (flag3)
			{
				VillagerRoleItem config = VillagerRole.Instance[roleIndex];
				OrganizationMemberItem orgConfig = OrganizationMember.Instance[config.OrganizationMember];
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
					Content = LocalStringManager.GetFormat(LanguageKey.LK_Building_ChangeVillagerRoleConfirm, orgConfig.GradeName),
					Yes = delegate()
					{
						TaiwuDomainMethod.Call.SetVillagerRole(this.Element.GameDataListenerId, selectedVillagerId, this.RoleKey);
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				TaiwuDomainMethod.Call.SetVillagerRole(this.Element.GameDataListenerId, selectedVillagerId, this.RoleKey);
			}
		}

		// Token: 0x06005916 RID: 22806 RVA: 0x0029561B File Offset: 0x0029381B
		private void OnSelectedVillager(List<int> selectedVillagerIds)
		{
			this.BatchAddVillagerRole(selectedVillagerIds);
			this.GetDisplayData();
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x00295630 File Offset: 0x00293830
		private void BatchAddVillagerRole(List<int> selectedVillagerIds)
		{
			foreach (int selectedVillagerId in selectedVillagerIds)
			{
				TaiwuDomainMethod.Call.SetVillagerRole(this.Element.GameDataListenerId, selectedVillagerId, this.RoleKey);
			}
		}

		// Token: 0x06005918 RID: 22808 RVA: 0x00295694 File Offset: 0x00293894
		private void RefreshCount()
		{
			this.historyVillagerCount.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, this._searchHistoryVillagerList.Count.ToString()), true);
			this.currentVillagerCount.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, this._searchCurrentVillagerList.Count.ToString()), true);
			this.lostVillagerCount.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, this._searchLostVillagerList.Count.ToString()), true);
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x00295720 File Offset: 0x00293920
		private void OnToggleChangeLineage(int newToggle, int oldToggle)
		{
			this.FilterSortAndRefresh(true, false);
			this.RefreshCount();
			this.RefreshAuthorityCostAndQuickRemoveAll();
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x0029573C File Offset: 0x0029393C
		private void OnHistoryVillagerRenderer(int index, Refers refers)
		{
			VillagerCharDisplayData data = this._searchHistoryVillagerList[index];
			this.OnVillagerRenderer(refers, data, false);
		}

		// Token: 0x0600591B RID: 22811 RVA: 0x00295764 File Offset: 0x00293964
		private void OnCurrentVillagerRenderer(int index, Refers refers)
		{
			VillagerCharDisplayData data = this._searchCurrentVillagerList[index];
			this.OnVillagerRenderer(refers, data, true);
		}

		// Token: 0x0600591C RID: 22812 RVA: 0x0029578C File Offset: 0x0029398C
		private void OnLostVillagerRenderer(int index, Refers refers)
		{
			VillagerCharDisplayData data = this._searchLostVillagerList[index];
			this.OnVillagerRenderer(refers, data, false);
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x002957B4 File Offset: 0x002939B4
		private void OnVillagerRenderer(Refers refers, VillagerCharDisplayData data, bool isCurrent)
		{
			Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			avatar.gameObject.SetActive(true);
			avatar.Refresh(data.AvatarRelatedData, data.CharacterTemplateId);
			CButtonObsolete merchantTypeButton = refers.CGet<CButtonObsolete>("MerchantTypeButton");
			merchantTypeButton.gameObject.SetActive(isCurrent && this.RoleKey == 3);
			merchantTypeButton.ClearAndAddListener(delegate
			{
				RectTransform rect = merchantTypeButton.GetComponent<RectTransform>();
				ArgumentBox argumentBox2 = EasyPool.Get<ArgumentBox>().SetObject("AnchorItem", rect).SetObject("Pos", rect.TransformPoint(default(Vector3).SetY(-130f))).Set("CharId", data.CharacterId);
				UIElement.VillagerSelectMerchantType.SetOnInitArgs(argumentBox2);
				UIManager.Instance.ShowUI(UIElement.VillagerSelectMerchantType, true);
			});
			TooltipInvoker mouse = refers.CGet<TooltipInvoker>("MouseTipDisplayer");
			mouse.Type = TipType.CharacterOnMapBlock;
			TooltipInvoker tooltipInvoker = mouse;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox.Set("CharId", data.CharacterId).Set("IsLineage", true);
			TextMeshProUGUI nameText = refers.CGet<TextMeshProUGUI>("NameText");
			string fullName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, SingletonObject.getInstance<BasicGameData>().TaiwuCharId == data.CharacterId, false);
			nameText.SetText(fullName, true);
			CImage bg = refers.CGet<CImage>("Bg");
			bg.SetSprite(isCurrent ? "building_lineage_characterbase_0" : "building_lineage_characterbase_1", false, null);
			CImage attainmentLine = refers.CGet<CImage>("AttainmentLine");
			attainmentLine.SetSprite(isCurrent ? "building_lineage_strip_0" : "building_lineage_strip_1", false, null);
			CImage personalityLine = refers.CGet<CImage>("PersonalityLine");
			personalityLine.SetSprite(isCurrent ? "building_lineage_strip_0" : "building_lineage_strip_1", false, null);
			CButtonObsolete characterDetailBtn = refers.CGet<CButtonObsolete>("CharacterDetailBtn");
			characterDetailBtn.interactable = (data.Health <= 0);
			characterDetailBtn.ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", data.CharacterId);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			});
			PointerTrigger pTrigger = refers.gameObject.GetComponent<PointerTrigger>();
			pTrigger.EnterEvent.RemoveAllListeners();
			pTrigger.ExitEvent.RemoveAllListeners();
			pTrigger.EnterEvent.AddListener(delegate()
			{
				CButtonObsolete removeBtn2 = refers.CGet<CButtonObsolete>("RemoveBtn");
				removeBtn2.gameObject.SetActive(this.villageListToggleGroup.GetActiveIndex() == 0);
			});
			CButtonObsolete removeBtn = refers.CGet<CButtonObsolete>("RemoveBtn");
			Action <>9__4;
			removeBtn.ClearAndAddListener(delegate
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Building_RemoveVillagerRoleConfirm);
				Action yes;
				if ((yes = <>9__4) == null)
				{
					yes = (<>9__4 = delegate()
					{
						TaiwuDomainMethod.Call.SetVillagerRole(this.Element.GameDataListenerId, data.CharacterId, -1);
					});
				}
				dialogCmd.Yes = yes;
				DialogCmd cmd = dialogCmd;
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			});
			CImage attainmentIcon = refers.CGet<CImage>("AttainmentIcon");
			TextMeshProUGUI attainmentValue = refers.CGet<TextMeshProUGUI>("AttainmentValue");
			ValueTuple<sbyte, short, bool> villagerRoleAttainmentTypeAndValue = ViewVillagerRole.GetVillagerRoleAttainmentTypeAndValue(this.RoleKey, data);
			sbyte skillType = villagerRoleAttainmentTypeAndValue.Item1;
			short skillValue = villagerRoleAttainmentTypeAndValue.Item2;
			bool isLifeSkill = villagerRoleAttainmentTypeAndValue.Item3;
			attainmentIcon.SetSprite(isLifeSkill ? ("mousetip_jiyi_" + skillType.ToString()) : ("mousetip_gongfa_" + skillType.ToString()), false, null);
			attainmentValue.SetText(skillValue.ToString(), true);
			CImage personalityIcon = refers.CGet<CImage>("PersonalityIcon");
			TextMeshProUGUI personalityValue = refers.CGet<TextMeshProUGUI>("PersonalityValue");
			ValueTuple<sbyte, short> villagerRolePersonalityTypeAndValue = ViewVillagerRole.GetVillagerRolePersonalityTypeAndValue(this.RoleKey, data);
			sbyte personalityType = villagerRolePersonalityTypeAndValue.Item1;
			short value = villagerRolePersonalityTypeAndValue.Item2;
			personalityIcon.SetSprite("mousetip_qiyuan_" + personalityType.ToString(), false, null);
			personalityValue.SetText(value.ToString(), true);
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x00295B30 File Offset: 0x00293D30
		[return: TupleElementNames(new string[]
		{
			"skillType",
			"value",
			"isLifeSkill"
		})]
		public unsafe static ValueTuple<sbyte, short, bool> GetVillagerRoleAttainmentTypeAndValue(short roleTemplateId, VillagerCharDisplayData data)
		{
			bool flag = roleTemplateId == 0;
			ValueTuple<sbyte, short, bool> result;
			if (flag)
			{
				result = new ValueTuple<sbyte, short, bool>(14, *data.LifeSkillAttainments[14], true);
			}
			else
			{
				bool flag2 = roleTemplateId == 1;
				if (flag2)
				{
					sbyte lifeSkillType = 0;
					short value = 0;
					for (sbyte i = 6; i <= 7; i += 1)
					{
						bool flag3 = *data.LifeSkillAttainments[(int)i] > value;
						if (flag3)
						{
							lifeSkillType = i;
							value = *data.LifeSkillAttainments[(int)i];
						}
					}
					for (sbyte j = 10; j <= 11; j += 1)
					{
						bool flag4 = *data.LifeSkillAttainments[(int)j] > value;
						if (flag4)
						{
							lifeSkillType = j;
							value = *data.LifeSkillAttainments[(int)j];
						}
					}
					result = new ValueTuple<sbyte, short, bool>(lifeSkillType, value, true);
				}
				else
				{
					bool flag5 = roleTemplateId == 2;
					if (flag5)
					{
						sbyte lifeSkillType2 = 0;
						short value2 = 0;
						for (sbyte k = 8; k <= 9; k += 1)
						{
							bool flag6 = *data.LifeSkillAttainments[(int)k] > value2;
							if (flag6)
							{
								lifeSkillType2 = k;
								value2 = *data.LifeSkillAttainments[(int)k];
							}
						}
						result = new ValueTuple<sbyte, short, bool>(lifeSkillType2, value2, true);
					}
					else
					{
						bool flag7 = roleTemplateId == 3;
						if (flag7)
						{
							result = new ValueTuple<sbyte, short, bool>(15, *data.LifeSkillAttainments[15], true);
						}
						else
						{
							bool flag8 = roleTemplateId == 4;
							if (flag8)
							{
								sbyte lifeSkillType3 = 0;
								short value3 = 0;
								for (sbyte l = 0; l <= 3; l += 1)
								{
									bool flag9 = *data.LifeSkillAttainments[(int)l] > value3;
									if (flag9)
									{
										lifeSkillType3 = l;
										value3 = *data.LifeSkillAttainments[(int)l];
									}
								}
								result = new ValueTuple<sbyte, short, bool>(lifeSkillType3, value3, true);
							}
							else
							{
								bool flag10 = roleTemplateId == 5;
								if (flag10)
								{
									sbyte combatSkillType = 0;
									short value4 = 0;
									for (sbyte m = 0; m < 14; m += 1)
									{
										bool flag11 = *data.CombatSkillAttainments[(int)m] > value4;
										if (flag11)
										{
											combatSkillType = m;
											value4 = *data.CombatSkillAttainments[(int)m];
										}
									}
									result = new ValueTuple<sbyte, short, bool>(combatSkillType, value4, false);
								}
								else
								{
									result = new ValueTuple<sbyte, short, bool>(4, *data.LifeSkillAttainments[4], true);
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600591F RID: 22815 RVA: 0x00295D90 File Offset: 0x00293F90
		[return: TupleElementNames(new string[]
		{
			"personalityType",
			"value"
		})]
		public unsafe static ValueTuple<sbyte, short> GetVillagerRolePersonalityTypeAndValue(short roleTemplateId, VillagerCharDisplayData data)
		{
			VillagerRoleItem config = VillagerRole.Instance[roleTemplateId];
			sbyte type = config.NeedPersonalityList[0].PersonalityType;
			return new ValueTuple<sbyte, short>(type, (short)(*data.Personalities[(int)type]));
		}

		// Token: 0x06005920 RID: 22816 RVA: 0x00295DD4 File Offset: 0x00293FD4
		private void FilterAndSort()
		{
			this._searchCurrentVillagerList = this._currentVillagerList[(int)this.RoleKey].Where(new Func<VillagerCharDisplayData, bool>(this.FilterVillager)).ToList<VillagerCharDisplayData>();
			this._searchHistoryVillagerList = this._historyVillagerList[(int)this.RoleKey].Where(new Func<VillagerCharDisplayData, bool>(this.FilterVillager)).ToList<VillagerCharDisplayData>();
			this._searchLostVillagerList = this._lostVillagerList[(int)this.RoleKey].Where(new Func<VillagerCharDisplayData, bool>(this.FilterVillager)).ToList<VillagerCharDisplayData>();
		}

		// Token: 0x06005921 RID: 22817 RVA: 0x00295E5D File Offset: 0x0029405D
		private void FilterSortAndRefresh(bool clearSelected = false, bool rebuildListStructure = false)
		{
			this.FilterAndSort();
			this.RefreshDataCount(clearSelected, rebuildListStructure);
		}

		// Token: 0x06005922 RID: 22818 RVA: 0x00295E70 File Offset: 0x00294070
		private bool FilterVillager(VillagerCharDisplayData displayData)
		{
			string inputValue = this._searchInputText;
			bool flag = !inputValue.IsNullOrEmpty();
			if (flag)
			{
				string nameText = NameCenter.GetMonasticTitleOrDisplayName(ref displayData.NameData, false, false);
				bool flag2 = !nameText.Contains(inputValue);
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005923 RID: 22819 RVA: 0x00295EBC File Offset: 0x002940BC
		private void InitRefers()
		{
			this.setVillagerBtnInner.ClearAndAddListener(new Action(this.ClickVillagerBtn));
			this.setVillagerBtn.ClearAndAddListener(delegate
			{
				bool flag = this._CurrentPage != ViewVillagerRole.EVillagerRolePage.RoleAssign;
				if (flag)
				{
					this.SetPageActive(ViewVillagerRole.EVillagerRolePage.RoleAssign, true);
				}
				this.ClickVillagerBtn();
			});
			this.villageListToggleGroup.Init(0);
			this.villageListToggleGroup.OnActiveIndexChange += this.OnVillagerListToggleChange;
			this.InitSearch();
			this.InitDataList();
		}

		// Token: 0x06005924 RID: 22820 RVA: 0x00295F30 File Offset: 0x00294130
		private void OnVillagerListToggleChange(int newTog, int oldTog)
		{
			this.btnQuickRemoveAll.gameObject.SetActive(newTog == 0);
			bool flag = newTog == 0;
			if (flag)
			{
				this.currentVillagerScroll.SetData(this._searchCurrentVillagerList, this.RoleKey, this.villageListToggleGroup.GetActiveIndex() == 0, true, false);
			}
			else
			{
				bool flag2 = newTog == 1;
				if (flag2)
				{
					this.currentVillagerScroll.SetData(this._searchHistoryVillagerList, this.RoleKey, this.villageListToggleGroup.GetActiveIndex() == 0, true, false);
				}
				else
				{
					bool flag3 = newTog == 2;
					if (flag3)
					{
						this.currentVillagerScroll.SetData(this._searchLostVillagerList, this.RoleKey, this.villageListToggleGroup.GetActiveIndex() == 0, true, false);
					}
				}
			}
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x00295FF0 File Offset: 0x002941F0
		private void InitDataList()
		{
			this._historyVillagerList = new List<VillagerCharDisplayData>[VillagerRole.Instance.Count];
			this._currentVillagerList = new List<VillagerCharDisplayData>[VillagerRole.Instance.Count];
			this._lostVillagerList = new List<VillagerCharDisplayData>[VillagerRole.Instance.Count];
			for (int i = 0; i < VillagerRole.Instance.Count; i++)
			{
				this._historyVillagerList[i] = new List<VillagerCharDisplayData>();
				this._currentVillagerList[i] = new List<VillagerCharDisplayData>();
				this._lostVillagerList[i] = new List<VillagerCharDisplayData>();
			}
		}

		// Token: 0x06005926 RID: 22822 RVA: 0x0029607F File Offset: 0x0029427F
		private void InitSearch()
		{
		}

		// Token: 0x06005927 RID: 22823 RVA: 0x00296084 File Offset: 0x00294284
		private void LinageCleanUp()
		{
			bool isAnimating = this.slideBook.IsAnimating;
			if (isAnimating)
			{
				this.slideBook.StopAnimation(false);
			}
		}

		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06005928 RID: 22824 RVA: 0x002960B0 File Offset: 0x002942B0
		public List<VillagerRoleManageDisplayData> RoleManageDisplayList
		{
			get
			{
				return this._roleManageDisplayList;
			}
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x002960B8 File Offset: 0x002942B8
		public bool IsExtraEffectUnlocked(int roleId)
		{
			return this._roleExtraEffectUnlockList.GetOrDefault(roleId);
		}

		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x0600592A RID: 22826 RVA: 0x002960C6 File Offset: 0x002942C6
		public IReadOnlyList<bool> RoleExtraEffectUnlockList
		{
			get
			{
				return this._roleExtraEffectUnlockList;
			}
		}

		// Token: 0x0600592B RID: 22827 RVA: 0x002960CE File Offset: 0x002942CE
		private void RoleManageAwake()
		{
			this.RoleManageInitRefers();
			this.InitRoleEffectDetails();
			this.InitRoleItem();
		}

		// Token: 0x0600592C RID: 22828 RVA: 0x002960E6 File Offset: 0x002942E6
		public void RoleManagePageInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.ReqAllData();
				this.Element.ShowAfterRefresh();
			}));
		}

		// Token: 0x0600592D RID: 22829 RVA: 0x00296118 File Offset: 0x00294318
		private void RoleManagePageEnable()
		{
			GEvent.Add(UiEvents.OnVillagerDispatched, new GEvent.Callback(this.OnVillagerDispatched));
			GEvent.Add(UiEvents.OnAssignChickenToVillagerRole, new GEvent.Callback(this.OnAssignChickenToVillagerRole));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x0600592E RID: 22830 RVA: 0x00296178 File Offset: 0x00294378
		private void RoleManagePageDisable()
		{
			GEvent.Remove(UiEvents.OnVillagerDispatched, new GEvent.Callback(this.OnVillagerDispatched));
			GEvent.Remove(UiEvents.OnAssignChickenToVillagerRole, new GEvent.Callback(this.OnAssignChickenToVillagerRole));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			this._roleManageDisplayList.Clear();
			this._villagerCharacterDisplayDataDict.Clear();
		}

		// Token: 0x0600592F RID: 22831 RVA: 0x002961F0 File Offset: 0x002943F0
		public void RoleManagePageInitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 58, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 132, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 176, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 183, ulong.MaxValue, null));
		}

		// Token: 0x06005930 RID: 22832 RVA: 0x00296268 File Offset: 0x00294468
		public void RoleManagePageNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						this.HandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
					}
				}
				else
				{
					this.HandleDataModification(notification.Uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x00296310 File Offset: 0x00294510
		private void HandleDataModification(DataUid uid, int offset, RawDataPool pool)
		{
			ushort dataId = uid.DataId;
			ushort num = dataId;
			if (num != 132)
			{
				if (num != 176)
				{
					if (num == 183)
					{
						Serializer.Deserialize(pool, offset, ref this.FarmerAutoCollectStorageType);
						this.RefreshSelectedRoleAutoActions();
					}
				}
				else
				{
					Serializer.DeserializeModifications<short>(pool, offset, this._villagerRoleAutoActionStates);
					this.RefreshSelectedRoleAutoActions();
				}
			}
			else
			{
				BuildingDomainMethod.Call.GetVillagerRoleExtraEffectUnlockState(this.Element.GameDataListenerId);
			}
		}

		// Token: 0x06005932 RID: 22834 RVA: 0x0029638C File Offset: 0x0029458C
		private void HandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool pool)
		{
			bool flag = domainId == 9 && methodId == 142;
			if (flag)
			{
				Serializer.Deserialize(pool, offset, ref this._roleExtraEffectUnlockList);
				this.RefreshRoleChickRelatedInfo();
			}
			bool flag2 = domainId == 5 && methodId == 117;
			if (flag2)
			{
				Serializer.Deserialize(pool, offset, ref this._roleManageDisplayList);
				this.RefreshSelectedRole();
				this.RefreshRoleItems();
			}
			bool flag3 = domainId == 5 && methodId == 109;
			if (flag3)
			{
				List<VillagerRoleCharacterDisplayData> list = new List<VillagerRoleCharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref list);
				foreach (VillagerRoleCharacterDisplayData displayData in list)
				{
					this._villagerCharacterDisplayDataDict[displayData.Id] = displayData;
				}
				bool flag4 = this._roleManageDisplayList != null;
				if (flag4)
				{
					List<int> removeList = new List<int>();
					using (Dictionary<int, VillagerRoleCharacterDisplayData>.Enumerator enumerator2 = this._villagerCharacterDisplayDataDict.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<int, VillagerRoleCharacterDisplayData> kvp = enumerator2.Current;
							bool flag5 = !this._roleManageDisplayList.Any((VillagerRoleManageDisplayData n) => n.CharacterIds != null && n.CharacterIds.Contains(kvp.Key));
							if (flag5)
							{
								removeList.Add(kvp.Key);
							}
						}
					}
					foreach (int key in removeList)
					{
						this._villagerCharacterDisplayDataDict.Remove(key);
					}
				}
				this.OnGetCharacterDisplayDataList();
			}
			bool flag6 = domainId == 5 && methodId == 111;
			if (flag6)
			{
				this.CheckSuccessAndRefreshAll(offset, pool, "BatchSetVillagerRole failed");
				GEvent.OnEvent(UiEvents.OnSetVillagerRole, null);
			}
			bool flag7 = domainId == 5 && methodId == 113;
			if (flag7)
			{
				this.CheckSuccessAndRefreshAll(offset, pool, "RecallVillager failed");
			}
			bool flag8 = domainId == 5 && methodId == 114;
			if (flag8)
			{
				this.CheckSuccessAndRefreshAll(offset, pool, "AssignTargetItem failed");
			}
			bool flag9 = domainId == 5 && methodId == 116;
			if (flag9)
			{
				this.CheckSuccessAndRefreshAll(offset, pool, "AssignArrangementIncreaseOrDecrease failed");
			}
		}

		// Token: 0x06005933 RID: 22835 RVA: 0x00296600 File Offset: 0x00294800
		private void CheckSuccessAndRefreshAll(int offset, RawDataPool pool, string failedMessage)
		{
			bool success = false;
			Serializer.Deserialize(pool, offset, ref success);
			bool flag = !success;
			if (flag)
			{
				Debug.LogError(failedMessage);
			}
			else
			{
				this.ReqAllData();
			}
		}

		// Token: 0x06005934 RID: 22836 RVA: 0x00296633 File Offset: 0x00294833
		protected void RoleManageClick(CButton btn)
		{
		}

		// Token: 0x06005935 RID: 22837 RVA: 0x00296638 File Offset: 0x00294838
		private void RoleManageInitRefers()
		{
			this.roleItemList = (from t in this.roleToggleGroup.GetAll()
			select t.GetComponent<ToggleStyle>()).ToList<ToggleStyle>();
			this.villagerRoleInfoAutoAction.OnInit();
			this.villagerRoleInfoDispatch.OnInit();
		}

		// Token: 0x06005936 RID: 22838 RVA: 0x00296698 File Offset: 0x00294898
		private void OpenDispatchPanel()
		{
			this.villagerRoleInfoDispatch.OpenDispatchPanel();
		}

		// Token: 0x06005937 RID: 22839 RVA: 0x002966A7 File Offset: 0x002948A7
		private void InitRoleItem()
		{
			this.SetupVillagerLabelNameWindow(this.villagerRoleLabelRefers);
			this.SetupVillagerLabelNameWindow(this.villagerRoleLabelRefersLineage);
		}

		// Token: 0x06005938 RID: 22840 RVA: 0x002966C4 File Offset: 0x002948C4
		private void SetupVillagerLabelNameWindow(Refers villagerRoleLabelRefers)
		{
			string labelName = "";
			CButton editNameButton = villagerRoleLabelRefers.CGet<CButton>("EditNameButton");
			TextMeshProUGUI roleNameLabel = villagerRoleLabelRefers.CGet<TextMeshProUGUI>("RoleNameLabel");
			VillagerRoleUtils.AsyncSetRoleName(roleNameLabel, this.RoleKey, this, true, delegate(string name)
			{
				labelName = name;
			});
			editNameButton.ClearAndAddListener(delegate
			{
				this.OpenChangeNameWindow(labelName, this.RoleKey, (int)this.RoleKey);
			});
		}

		// Token: 0x06005939 RID: 22841 RVA: 0x00296730 File Offset: 0x00294930
		private void OpenChangeNameWindow(string roleName, short roleTemplateId, int index)
		{
			new RenameCfg
			{
				Title = LanguageKey.LK_Building_QuickAction_Rename_Title.Tr(),
				Description = LanguageKey.LK_UI_Following_Rename_Desc.TrFormat(roleName),
				EmptyDesc = LanguageKey.Lk_NewGameSubPageAvatar_ChangeName_Tips.Tr(),
				CharCount = ViewBuildingManage.GetBuildingNameCharCount(),
				Submit = delegate(string x)
				{
					this.OnRenameSubmit(x, roleTemplateId, index);
				}
			}.Show();
		}

		// Token: 0x0600593A RID: 22842 RVA: 0x002967B4 File Offset: 0x002949B4
		private void OnRenameSubmit(string newName, short roleTemplateId, int index)
		{
			TaiwuDomainMethod.AsyncCall.SetVillagerRoleNickName(this, roleTemplateId, newName, delegate(int offset, RawDataPool pool)
			{
				int nickNameKey = -1;
				Serializer.Deserialize(pool, offset, ref nickNameKey);
				this.RefreshRoleItem(index);
				this.RefreshRoleDetails();
				GEvent.OnEvent(UiEvents.VillagerRoleNickNameChanged, null);
			});
		}

		// Token: 0x0600593B RID: 22843 RVA: 0x002967EB File Offset: 0x002949EB
		private void InitRoleEffectDetails()
		{
		}

		// Token: 0x0600593C RID: 22844 RVA: 0x002967EE File Offset: 0x002949EE
		private void ToggleChangeRoleManage(int index)
		{
			this.OnSelectedRole();
			this.SetupVillagerLabelNameWindow(this.villagerRoleLabelRefers);
			this.SetupVillagerLabelNameWindow(this.villagerRoleLabelRefersLineage);
		}

		// Token: 0x0600593D RID: 22845 RVA: 0x00296814 File Offset: 0x00294A14
		private void OnSelectedRole()
		{
			bool flag = this._roleManageDisplayList == null || this._roleManageDisplayList.Count == 0;
			if (!flag)
			{
				VillagerRoleItem config = ViewVillagerRole.GetRoleConfigByIndex((int)this.RoleKey);
				this.villagerRoleInfoNeed.Refresh(config);
				this.villagerRoleInfoBook.Refresh(config);
				this.villagerRoleInfoBuilding.Refresh(config, new Action(this.QuickHide));
				this.villagerRoleInfoDispatch.Refresh(config, this);
				this.villagerRoleInfoAutoAction.Refresh(config, this);
			}
		}

		// Token: 0x0600593E RID: 22846 RVA: 0x002968A0 File Offset: 0x00294AA0
		private void OnGetCharacterDisplayDataList()
		{
			this.RefreshRoleDetails();
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("VillagerCharacterDisplayDataDict", this._villagerCharacterDisplayDataDict);
			argsBox.SetObject("RoleManageDisplayList", this._roleManageDisplayList);
			GEvent.OnEvent(UiEvents.RefreshVillagerRoleDispatch, argsBox);
			this.RefreshRoleItems();
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x0600593F RID: 22847 RVA: 0x00296904 File Offset: 0x00294B04
		private void RefreshRoleItems()
		{
			for (int i = 0; i < this.roleItemList.Count; i++)
			{
				this.RefreshRoleItem(i);
			}
		}

		// Token: 0x06005940 RID: 22848 RVA: 0x00296936 File Offset: 0x00294B36
		private void RefreshSelectedRole()
		{
			this.OnSelectedRole();
		}

		// Token: 0x06005941 RID: 22849 RVA: 0x00296940 File Offset: 0x00294B40
		private void RefreshSelectedRoleAutoActions()
		{
			VillagerRoleItem config = ViewVillagerRole.GetRoleConfigByIndex((int)this.RoleKey);
			this.villagerRoleInfoAutoAction.Refresh(config, this);
		}

		// Token: 0x06005942 RID: 22850 RVA: 0x00296968 File Offset: 0x00294B68
		private void RefreshRoleItem(int index)
		{
			bool flag = index == 7;
			if (!flag)
			{
				ToggleStyle toggleStyle = this.roleToggleGroup.Get(index).GetComponent<ToggleStyle>();
				TextMeshProUGUI labelComp = toggleStyle.Label;
				VillagerRoleUtils.AsyncSetRoleName(labelComp, (short)index, this, true, null);
			}
		}

		// Token: 0x06005943 RID: 22851 RVA: 0x002969A6 File Offset: 0x00294BA6
		private void RefreshRoleDetails()
		{
			this.SetupVillagerLabelNameWindow(this.villagerRoleLabelRefers);
			this.SetupVillagerLabelNameWindow(this.villagerRoleLabelRefersLineage);
		}

		// Token: 0x06005944 RID: 22852 RVA: 0x002969C3 File Offset: 0x00294BC3
		private void RefreshRoleChickRelatedInfo()
		{
			this.villagerRoleInfoDispatch.RefreshByChickUnlock();
			this.villagerRoleInfoAutoAction.RefreshByChickUnlock();
		}

		// Token: 0x06005945 RID: 22853 RVA: 0x002969E0 File Offset: 0x00294BE0
		private void OnVillagerDispatched(ArgumentBox argbox)
		{
			int charId;
			argbox.Get("CharacterId", out charId);
			List<int> list = EasyPool.Get<List<int>>();
			list.Clear();
			list.Add(charId);
			TaiwuDomainMethod.Call.GetVillagerRoleCharacterDisplayDataList(this.Element.GameDataListenerId, list);
			EasyPool.Free<List<int>>(list);
		}

		// Token: 0x06005946 RID: 22854 RVA: 0x00296A2A File Offset: 0x00294C2A
		private void OnAssignChickenToVillagerRole(ArgumentBox argbox)
		{
			this.ReqAllData();
		}

		// Token: 0x06005947 RID: 22855 RVA: 0x00296A34 File Offset: 0x00294C34
		private void OnTopUiChanged(ArgumentBox argumentBox)
		{
			bool flag = UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				this.ReqAllData();
			}
		}

		// Token: 0x06005948 RID: 22856 RVA: 0x00296A5F File Offset: 0x00294C5F
		private void ReqAllData()
		{
			TaiwuDomainMethod.Call.GetAllVillagerRoleDisplayData(this.Element.GameDataListenerId);
		}

		// Token: 0x06005949 RID: 22857 RVA: 0x00296A74 File Offset: 0x00294C74
		public bool IsAutoActionEnabled(short roleTemplateId, short actionTemplateId)
		{
			bool flag = this._villagerRoleAutoActionStates == null;
			ulong state;
			return !flag && this._villagerRoleAutoActionStates.TryGetValue(roleTemplateId, out state) && BitOperation.GetBit(state, (int)actionTemplateId);
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x00296AB4 File Offset: 0x00294CB4
		public void SetAutoActionEnabled(short roleTemplateId, short actionTemplateId, bool e)
		{
			bool flag = this._villagerRoleAutoActionStates == null;
			if (!flag)
			{
				ulong state;
				bool flag2 = !this._villagerRoleAutoActionStates.TryGetValue(roleTemplateId, out state);
				if (!flag2)
				{
					state = BitOperation.SetBit(state, (int)actionTemplateId, e);
					ExtraDomainMethod.Call.SetVillagerRoleAutoActionState(roleTemplateId, state);
				}
			}
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x00296AFC File Offset: 0x00294CFC
		private static VillagerRoleItem GetRoleConfigByIndex(int index)
		{
			return VillagerRole.Instance[index];
		}

		// Token: 0x04003D26 RID: 15654
		[Header("通用部分")]
		[SerializeField]
		private TextMeshProUGUI titleTxt;

		// Token: 0x04003D27 RID: 15655
		[SerializeField]
		private CButton buttonClosePopup;

		// Token: 0x04003D28 RID: 15656
		[SerializeField]
		private SlideBookComponent slideBook;

		// Token: 0x04003D29 RID: 15657
		[SerializeField]
		private TextMeshProUGUI taiwuLabel;

		// Token: 0x04003D2A RID: 15658
		[SerializeField]
		private SlideBookIndexerComponent chickenPageIndexer;

		// Token: 0x04003D2B RID: 15659
		[SerializeField]
		private SlideBookIndexerComponent roleAssignPageIndexer;

		// Token: 0x04003D2C RID: 15660
		[SerializeField]
		private SlideBookIndexerComponent descPageIndexer;

		// Token: 0x04003D2D RID: 15661
		[SerializeField]
		private CToggleGroup roleToggleGroup;

		// Token: 0x04003D2E RID: 15662
		private ViewVillagerRole.EnterType _enterType = ViewVillagerRole.EnterType.Normal;

		// Token: 0x04003D2F RID: 15663
		private int _targetRoleTemplateId = -1;

		// Token: 0x04003D30 RID: 15664
		private bool _needAutoJumpPage = false;

		// Token: 0x04003D31 RID: 15665
		private StringBuilder _sb = new StringBuilder();

		// Token: 0x04003D32 RID: 15666
		private bool _openSelectVillager = false;

		// Token: 0x04003D33 RID: 15667
		private bool _openSendVillager = false;

		// Token: 0x04003D34 RID: 15668
		private bool _activeChicken = false;

		// Token: 0x04003D35 RID: 15669
		private bool _inited;

		// Token: 0x04003D36 RID: 15670
		private ViewVillagerRole.EVillagerRolePage _targetRolePage;

		// Token: 0x04003D37 RID: 15671
		private Dictionary<short, IntList> _sectFulongOrgMemberChickens;

		// Token: 0x04003D38 RID: 15672
		private readonly Dictionary<int, string> _nickNameDict = new Dictionary<int, string>();

		// Token: 0x04003D39 RID: 15673
		private readonly Dictionary<int, GameData.Domains.Building.Chicken> _chickenDict = new Dictionary<int, GameData.Domains.Building.Chicken>();

		// Token: 0x04003D3A RID: 15674
		private List<int> _villageChickenIdList = new List<int>();

		// Token: 0x04003D3B RID: 15675
		private List<GameData.Domains.Building.Chicken> _chickenList;

		// Token: 0x04003D3C RID: 15676
		private sbyte _returnFavor;

		// Token: 0x04003D3D RID: 15677
		private int _selectedRoleIndex = -1;

		// Token: 0x04003D3E RID: 15678
		private int _feedChickenIndex = -1;

		// Token: 0x04003D3F RID: 15679
		private bool _isInEffectPreviewMode = false;

		// Token: 0x04003D40 RID: 15680
		private bool _needPlayEffect = false;

		// Token: 0x04003D41 RID: 15681
		private List<int> _previewSelectedChickenIds;

		// Token: 0x04003D42 RID: 15682
		private UIParticlePlayHelper _particlePlayHelper = new UIParticlePlayHelper();

		// Token: 0x04003D43 RID: 15683
		private bool _isExtraEffectUnlocked = false;

		// Token: 0x04003D44 RID: 15684
		private readonly List<int> _roleEffectValues = new List<int>();

		// Token: 0x04003D45 RID: 15685
		private const int RoleCount = 7;

		// Token: 0x04003D46 RID: 15686
		private readonly List<ViewVillagerRole.ChickenPersonality> _cachedChickenPersonalityList = new List<ViewVillagerRole.ChickenPersonality>();

		// Token: 0x04003D47 RID: 15687
		[Header("元鸡")]
		[SerializeField]
		private Refers villagerRoleEffectItemTemplate;

		// Token: 0x04003D48 RID: 15688
		[SerializeField]
		private List<SelectableChickenItem> chickenSlotList;

		// Token: 0x04003D49 RID: 15689
		[SerializeField]
		private List<CImage> personalityIconList;

		// Token: 0x04003D4A RID: 15690
		[SerializeField]
		private List<TextMeshProUGUI> personalityLabelList;

		// Token: 0x04003D4B RID: 15691
		[SerializeField]
		private List<TextMeshProUGUI> personalityNameLabelList;

		// Token: 0x04003D4C RID: 15692
		[SerializeField]
		private List<CImage> needPersonalityIconList;

		// Token: 0x04003D4D RID: 15693
		[SerializeField]
		private List<CImage> needPersonalityIconBackList;

		// Token: 0x04003D4E RID: 15694
		[SerializeField]
		private TextMeshProUGUI effectLockDesc;

		// Token: 0x04003D4F RID: 15695
		[SerializeField]
		private GameObject darkMask;

		// Token: 0x04003D50 RID: 15696
		[SerializeField]
		private UIParticle unlockParticle;

		// Token: 0x04003D51 RID: 15697
		[SerializeField]
		private UIParticle unlockStateParticle;

		// Token: 0x04003D52 RID: 15698
		[SerializeField]
		private GameObject disableClick;

		// Token: 0x04003D53 RID: 15699
		[SerializeField]
		private CImage roleImage;

		// Token: 0x04003D54 RID: 15700
		[SerializeField]
		private CButton btnQuickAssign;

		// Token: 0x04003D55 RID: 15701
		private string _searchInputText;

		// Token: 0x04003D56 RID: 15702
		private int _taiwuAuthorityCount;

		// Token: 0x04003D57 RID: 15703
		private List<VillagerCharDisplayData>[] _historyVillagerList;

		// Token: 0x04003D58 RID: 15704
		private List<VillagerCharDisplayData>[] _currentVillagerList;

		// Token: 0x04003D59 RID: 15705
		private List<VillagerCharDisplayData>[] _lostVillagerList;

		// Token: 0x04003D5A RID: 15706
		private List<VillagerCharDisplayData> _searchHistoryVillagerList = new List<VillagerCharDisplayData>();

		// Token: 0x04003D5B RID: 15707
		private List<VillagerCharDisplayData> _searchCurrentVillagerList = new List<VillagerCharDisplayData>();

		// Token: 0x04003D5C RID: 15708
		private List<VillagerCharDisplayData> _searchLostVillagerList = new List<VillagerCharDisplayData>();

		// Token: 0x04003D5D RID: 15709
		public const string SelectVillagerSorterSaveKey = "TaiwuVillageLineage_Sort0";

		// Token: 0x04003D5E RID: 15710
		[Header("身份名册")]
		[SerializeField]
		private TextMeshProUGUI historyVillagerCount;

		// Token: 0x04003D5F RID: 15711
		[SerializeField]
		private TextMeshProUGUI currentVillagerCount;

		// Token: 0x04003D60 RID: 15712
		[SerializeField]
		private TextMeshProUGUI lostVillagerCount;

		// Token: 0x04003D61 RID: 15713
		[SerializeField]
		private TextMeshProUGUI authorityCost;

		// Token: 0x04003D62 RID: 15714
		[SerializeField]
		private CButton setVillagerBtn;

		// Token: 0x04003D63 RID: 15715
		[SerializeField]
		private CButton setVillagerBtnInner;

		// Token: 0x04003D64 RID: 15716
		[SerializeField]
		private CToggleGroup villageListToggleGroup;

		// Token: 0x04003D65 RID: 15717
		[SerializeField]
		private CButton btnQuickRemoveAll;

		// Token: 0x04003D66 RID: 15718
		[SerializeField]
		private CButton btnSelectAll;

		// Token: 0x04003D67 RID: 15719
		[SerializeField]
		private GameObject checkmarkSelectAll;

		// Token: 0x04003D68 RID: 15720
		[SerializeField]
		private CharacterScrollMultipleMode currentVillagerScroll;

		// Token: 0x04003D69 RID: 15721
		[SerializeField]
		private Refers villagerRoleLabelRefersLineage;

		// Token: 0x04003D6A RID: 15722
		private List<bool> _roleExtraEffectUnlockList = new List<bool>();

		// Token: 0x04003D6B RID: 15723
		private readonly Dictionary<short, ulong> _villagerRoleAutoActionStates = new Dictionary<short, ulong>();

		// Token: 0x04003D6C RID: 15724
		private readonly Dictionary<int, VillagerRoleCharacterDisplayData> _villagerCharacterDisplayDataDict = new Dictionary<int, VillagerRoleCharacterDisplayData>();

		// Token: 0x04003D6D RID: 15725
		private List<VillagerRoleManageDisplayData> _roleManageDisplayList = new List<VillagerRoleManageDisplayData>();

		// Token: 0x04003D6E RID: 15726
		private bool _isSelectedRoleExtraEffectUnlocked;

		// Token: 0x04003D6F RID: 15727
		public sbyte FarmerAutoCollectStorageType;

		// Token: 0x04003D70 RID: 15728
		[Header("身份说明")]
		[SerializeField]
		private VillagerRoleInfoNeed villagerRoleInfoNeed;

		// Token: 0x04003D71 RID: 15729
		[SerializeField]
		private VillagerRoleInfoBook villagerRoleInfoBook;

		// Token: 0x04003D72 RID: 15730
		[SerializeField]
		private VillagerRoleInfoBuilding villagerRoleInfoBuilding;

		// Token: 0x04003D73 RID: 15731
		[SerializeField]
		private VillagerRoleInfoDispatch villagerRoleInfoDispatch;

		// Token: 0x04003D74 RID: 15732
		[SerializeField]
		private VillagerRoleInfoAutoAction villagerRoleInfoAutoAction;

		// Token: 0x04003D75 RID: 15733
		[SerializeField]
		private Refers villagerRoleLabelRefers;

		// Token: 0x04003D76 RID: 15734
		private List<ToggleStyle> roleItemList;

		// Token: 0x02001BF2 RID: 7154
		public enum EVillagerRolePage
		{
			// Token: 0x0400BF24 RID: 48932
			None,
			// Token: 0x0400BF25 RID: 48933
			RoleDescription,
			// Token: 0x0400BF26 RID: 48934
			RoleAssign,
			// Token: 0x0400BF27 RID: 48935
			ChickenAssign
		}

		// Token: 0x02001BF3 RID: 7155
		private struct ChickenPersonality
		{
			// Token: 0x0400BF28 RID: 48936
			public int ChickenId;

			// Token: 0x0400BF29 RID: 48937
			public sbyte Type;

			// Token: 0x0400BF2A RID: 48938
			public int Value;
		}

		// Token: 0x02001BF4 RID: 7156
		public enum EnterType
		{
			// Token: 0x0400BF2C RID: 48940
			Normal,
			// Token: 0x0400BF2D RID: 48941
			FromRoleManage
		}
	}
}
