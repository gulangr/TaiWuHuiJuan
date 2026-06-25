using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using EasyButtons;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CombatSkill;
using Game.Views.CharacterMenu.EquipCombatSkill;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA3 RID: 2979
	public class ViewCharacterMenuEquipCombatSkill : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17000FF4 RID: 4084
		// (get) Token: 0x060093FD RID: 37885 RVA: 0x0044FA43 File Offset: 0x0044DC43
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_EquipCombatSkill;
			}
		}

		// Token: 0x17000FF5 RID: 4085
		// (get) Token: 0x060093FE RID: 37886 RVA: 0x0044FA4A File Offset: 0x0044DC4A
		public override bool ShowBaseAttribute
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000FF6 RID: 4086
		// (get) Token: 0x060093FF RID: 37887 RVA: 0x0044FA4D File Offset: 0x0044DC4D
		public PoolItem SkillItemPool
		{
			get
			{
				return this._skillItemPool;
			}
		}

		// Token: 0x17000FF7 RID: 4087
		// (get) Token: 0x06009400 RID: 37888 RVA: 0x0044FA55 File Offset: 0x0044DC55
		public PoolItem EmptyGridPool
		{
			get
			{
				return this._emptyGridPool;
			}
		}

		// Token: 0x17000FF8 RID: 4088
		// (get) Token: 0x06009401 RID: 37889 RVA: 0x0044FA5D File Offset: 0x0044DC5D
		// (set) Token: 0x06009402 RID: 37890 RVA: 0x0044FA65 File Offset: 0x0044DC65
		public CombatSkillSortAndFilterController SortAndFilter { get; private set; }

		// Token: 0x17000FF9 RID: 4089
		// (get) Token: 0x06009403 RID: 37891 RVA: 0x0044FA6E File Offset: 0x0044DC6E
		public byte[] GenericGridAllocation
		{
			get
			{
				EquipCombatSkillDisplayData equipCombatSkillDisplayData = this._equipCombatSkillDisplayData;
				return ((equipCombatSkillDisplayData != null) ? equipCombatSkillDisplayData.GenericGridAllocation : null) ?? new byte[5];
			}
		}

		// Token: 0x17000FFA RID: 4090
		// (get) Token: 0x06009404 RID: 37892 RVA: 0x0044FA8C File Offset: 0x0044DC8C
		public List<CombatSkillDisplayDataCharacterMenuListItem> SkillDatas
		{
			get
			{
				return this._equipCombatSkillDisplayData.CombatSkillDisplayDatas;
			}
		}

		// Token: 0x17000FFB RID: 4091
		// (get) Token: 0x06009405 RID: 37893 RVA: 0x0044FA99 File Offset: 0x0044DC99
		public sbyte TotalGenericGridCount
		{
			get
			{
				return this.CombatSkillSlotCounts[5];
			}
		}

		// Token: 0x17000FFC RID: 4092
		// (get) Token: 0x06009406 RID: 37894 RVA: 0x0044FAA4 File Offset: 0x0044DCA4
		public bool CanOperate
		{
			get
			{
				CharacterMenuFunctionControlItem config;
				return base.CharacterMenu.CanOperate && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.SkillEquip));
			}
		}

		// Token: 0x17000FFD RID: 4093
		// (get) Token: 0x06009407 RID: 37895 RVA: 0x0044FAE7 File Offset: 0x0044DCE7
		private bool IsTaiwuTeammateButNotBeast
		{
			get
			{
				return base.CharacterMenu.CurrentCharacterIsTaiwuTeammate && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x17000FFE RID: 4094
		// (get) Token: 0x06009408 RID: 37896 RVA: 0x0044FB12 File Offset: 0x0044DD12
		private bool IsShowUIMask
		{
			get
			{
				return this.IsInEditingMode || this.IsInSortCombatSkillMode || this.IsInPreview || this.IsInFavoriteMode;
			}
		}

		// Token: 0x17000FFF RID: 4095
		// (get) Token: 0x06009409 RID: 37897 RVA: 0x0044FB35 File Offset: 0x0044DD35
		// (set) Token: 0x0600940A RID: 37898 RVA: 0x0044FB3D File Offset: 0x0044DD3D
		public bool IsInEditingMode
		{
			get
			{
				return this._isInEditingMode;
			}
			set
			{
				this._isInEditingMode = value;
				this.OnEditModeChanged();
			}
		}

		// Token: 0x17001000 RID: 4096
		// (get) Token: 0x0600940B RID: 37899 RVA: 0x0044FB4E File Offset: 0x0044DD4E
		// (set) Token: 0x0600940C RID: 37900 RVA: 0x0044FB56 File Offset: 0x0044DD56
		public bool IsInSortCombatSkillMode
		{
			get
			{
				return this._isInSortCombatSkillMode;
			}
			set
			{
				this._isInSortCombatSkillMode = value;
				this.OnInSortCombatSkillChanged();
			}
		}

		// Token: 0x17001001 RID: 4097
		// (get) Token: 0x0600940D RID: 37901 RVA: 0x0044FB67 File Offset: 0x0044DD67
		// (set) Token: 0x0600940E RID: 37902 RVA: 0x0044FB70 File Offset: 0x0044DD70
		public bool IsInPreview
		{
			get
			{
				return this._isInPreview;
			}
			set
			{
				bool flag = this._isInPreview && !value;
				if (flag)
				{
					this._isInPreview = false;
					this.UpdateDisplayData();
					this.Refresh(true);
				}
				else
				{
					this._isInPreview = value;
				}
			}
		}

		// Token: 0x0600940F RID: 37903 RVA: 0x0044FBB4 File Offset: 0x0044DDB4
		private void ResetAllModes()
		{
			this._isInEditingMode = false;
			this._isInSortCombatSkillMode = false;
			this._isInPreview = false;
			this._isInFavoriteMode = false;
			this._isAwaitingEquipResponse = false;
			this.ResetDotween();
			this.OnEditModeChanged();
		}

		// Token: 0x06009410 RID: 37904 RVA: 0x0044FBE8 File Offset: 0x0044DDE8
		private void ResetDotween()
		{
			this.viewBack.DOKill(false);
			int target = this.IsInEditingMode ? 1907 : 2230;
			this.viewBack.sizeDelta = this.viewBack.sizeDelta.SetX((float)target);
			this.leftArea.DOKill(false);
			target = (this.IsInEditingMode ? 30 : -415);
			this.leftArea.anchoredPosition = this.leftArea.anchoredPosition.SetX((float)target);
			this.middleArea.DOKill(false);
			target = (this.IsInEditingMode ? 1160 : 2070);
			this.middleArea.sizeDelta = this.middleArea.sizeDelta.SetX((float)target);
			this.rightArea.DOKill(false);
			target = (this.IsInEditingMode ? 0 : 800);
			this.rightArea.anchoredPosition = this.rightArea.anchoredPosition.SetX((float)target);
			this.ReleaseContentTweener();
		}

		// Token: 0x06009411 RID: 37905 RVA: 0x0044FCF6 File Offset: 0x0044DEF6
		public override void OnSubpageVisible()
		{
			base.OnSubpageVisible();
			this.RefreshSkillLineRoleDisplay();
		}

		// Token: 0x06009412 RID: 37906 RVA: 0x0044FD07 File Offset: 0x0044DF07
		public override void OnSubpageInVisible()
		{
			base.OnSubpageInVisible();
			this.ApplyScrollContentPosYOnPageOpenClose();
			this.HideAllSkillLineRoles();
			this.ResetAllModes();
		}

		// Token: 0x17001002 RID: 4098
		// (get) Token: 0x06009413 RID: 37907 RVA: 0x0044FD26 File Offset: 0x0044DF26
		// (set) Token: 0x06009414 RID: 37908 RVA: 0x0044FD2E File Offset: 0x0044DF2E
		public bool IsInFavoriteMode
		{
			get
			{
				return this._isInFavoriteMode;
			}
			set
			{
				this._isInFavoriteMode = value;
				this.OnFavoriteModeChanged();
			}
		}

		// Token: 0x06009415 RID: 37909 RVA: 0x0044FD40 File Offset: 0x0044DF40
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedWaitData = true;
			this.NeedDataListenerId = true;
			this._isUnavailable = false;
			this.IsInEditingMode = false;
			this.localLoadingAnim.SetLoadingEvent(delegate
			{
				this.invisiblePage.SetActive(false);
				this.viewBack.gameObject.SetActive(false);
			}, delegate
			{
				this.invisiblePage.SetActive(this._isUnavailable);
				this.viewBack.gameObject.SetActive(!this._isUnavailable);
				this.OnLocalLoadingComplete();
			});
		}

		// Token: 0x06009416 RID: 37910 RVA: 0x0044FD90 File Offset: 0x0044DF90
		private void OnLocalLoadingComplete()
		{
			this._scrollLayoutDeferUntilLoadingComplete = false;
			bool flag = !this._layoutDataReady;
			if (!flag)
			{
				this._scrollContentLayout.MarkDirty();
				this._scrollContentLayout.ScheduleApplyIfDirty();
			}
		}

		// Token: 0x06009417 RID: 37911 RVA: 0x0044FDCC File Offset: 0x0044DFCC
		private void ScheduleScrollLayoutIfReady()
		{
			this._scrollContentLayout.MarkDirty();
			bool flag = this._layoutDataReady && !this._scrollLayoutDeferUntilLoadingComplete;
			if (flag)
			{
				this._scrollContentLayout.ScheduleApplyIfDirty();
			}
		}

		// Token: 0x06009418 RID: 37912 RVA: 0x0044FE0C File Offset: 0x0044E00C
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.EquipCombatSkillBase;
		}

		// Token: 0x06009419 RID: 37913 RVA: 0x0044FE24 File Offset: 0x0044E024
		private void Awake()
		{
			this.invisiblePage.SetActive(false);
			this.viewBack.gameObject.SetActive(false);
			this._skillItemPool = new PoolItem("ViewCharacterMenuEquipCombatSkill_SkillItem", this.equipCombatSkillItem.gameObject);
			this._emptyGridPool = new PoolItem("ViewCharacterMenuEquipCombatSkill_EmptyGrid", this.emptyGrid);
			this.SortAndFilter = new CombatSkillSortAndFilterController(this.commonSortAndFilter, false, EFilterType.Common);
			this.SortAndFilter.Init(new Action(this.HandleSkillListChanged), "CharacterMenuEquipCombatSkillSortAndFilter");
			this.equipPlanToggleGroup.Init(-1);
			this.equipPlanToggleGroup.OnActiveIndexChange += this.HandleEquipPlanToggleChange;
			this.addPlanButton.ClearAndAddListener(delegate
			{
				CharacterDomainMethod.Call.AppendCombatSkillPlan(base.CharacterMenu.CurCharacterId);
				this.RequestData();
			});
			this.copyPlanButton.ClearAndAddListener(delegate
			{
				CharacterDomainMethod.Call.DuplicateCurrentCombatSkillPlan(base.CharacterMenu.CurCharacterId);
				this.RequestData();
			});
			this.clearPlanButton.ClearAndAddListener(delegate
			{
				this.DoConfirmOnEquippedCombatSkill(new Action(this.<Awake>g__PlanClear|101_6), LanguageKey.LK_CombatSkill_Equip_PlanClear_Confirm, LanguageKey.LK_Common_Attention);
			});
			this.deletePlanButton.ClearAndAddListener(delegate
			{
				this.DoConfirmOnEquippedCombatSkill(new Action(this.<Awake>g__PlanDelete|101_7), LanguageKey.LK_CombatSkill_Equip_PlanDelete_Confirm, LanguageKey.LK_Common_Attention);
			});
			this.favoriteToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				this.IsInFavoriteMode = isOn;
			});
			this.lockToggle.onValueChanged.AddListener(delegate(bool isON)
			{
				this.OnAiEquipLockToggleChanged(isON);
			});
			this.originContentWidth = this.equipCombatSkillScrollRect.content.rect.width;
			this._scrollContentLayout.Initialize(this, this.equipCombatSkillScrollRect, this.equipCombatSkillLines, this.originContentWidth);
		}

		// Token: 0x0600941A RID: 37914 RVA: 0x0044FFAF File Offset: 0x0044E1AF
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GlobalDomainMethod.Call.InvokeGuidingTrigger(88);
			this.ApplyScrollContentPosYOnPageOpenClose();
		}

		// Token: 0x0600941B RID: 37915 RVA: 0x0044FFDC File Offset: 0x0044E1DC
		private new void OnDisable()
		{
			this._scrollContentLayout.Dispose();
			this.ApplyScrollContentPosYOnPageOpenClose();
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			this.ResetAllModes();
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x0600941C RID: 37916 RVA: 0x00450028 File Offset: 0x0044E228
		private void ApplyScrollContentPosYOnPageOpenClose()
		{
			ScrollRect scrollRect = this.equipCombatSkillScrollRect;
			bool flag = ((scrollRect != null) ? scrollRect.content : null) == null;
			if (!flag)
			{
				this.SetContentPosY(0.5f);
			}
		}

		// Token: 0x0600941D RID: 37917 RVA: 0x00450060 File Offset: 0x0044E260
		private void OnTopUiChanged(ArgumentBox box)
		{
		}

		// Token: 0x0600941E RID: 37918 RVA: 0x00450063 File Offset: 0x0044E263
		private void HandleEquipPlanToggleChange(int newIndex, int oldIndex)
		{
			CharacterDomainMethod.Call.UpdateCombatSkillPlan(base.CharacterMenu.CurCharacterId, newIndex);
			this.RequestData();
		}

		// Token: 0x0600941F RID: 37919 RVA: 0x00450080 File Offset: 0x0044E280
		private void OnAiEquipLockToggleChanged(bool isOn)
		{
			bool isTaiwuTeammateButNotBeast = this.IsTaiwuTeammateButNotBeast;
			if (isTaiwuTeammateButNotBeast)
			{
				CharacterDomainMethod.Call.SetCombatSkillPlanLock(base.CharacterMenu.CurCharacterId, isOn);
			}
		}

		// Token: 0x06009420 RID: 37920 RVA: 0x004500AC File Offset: 0x0044E2AC
		private void LockAiEquip()
		{
			this.lockToggle.isOn = true;
		}

		// Token: 0x06009421 RID: 37921 RVA: 0x004500BC File Offset: 0x0044E2BC
		[Button("武学筛选按钮")]
		public void Set(int value)
		{
			this.SortAndFilter.SetDropdownOption(EFilterLine.First.ToInt(), 0, value);
		}

		// Token: 0x06009422 RID: 37922 RVA: 0x004500D8 File Offset: 0x0044E2D8
		public void SetCombatSkillFilterType(sbyte equipType)
		{
			this.SortAndFilter.ClearAllFilter();
			bool flag = equipType >= 0 && equipType < 5;
			if (flag)
			{
				this.SortAndFilter.SetDropdownOption(EFilterLine.First.ToInt(), 0, (int)equipType);
			}
			this.HandleSkillListChanged();
		}

		// Token: 0x06009423 RID: 37923 RVA: 0x00450124 File Offset: 0x0044E324
		public void ExitCurrentMode()
		{
			bool isInFavoriteMode = this.IsInFavoriteMode;
			if (isInFavoriteMode)
			{
				this.IsInFavoriteMode = false;
			}
			else
			{
				bool isInSortCombatSkillMode = this.IsInSortCombatSkillMode;
				if (isInSortCombatSkillMode)
				{
					this.IsInSortCombatSkillMode = false;
				}
				else
				{
					bool isInEditingMode = this.IsInEditingMode;
					if (isInEditingMode)
					{
						this.IsInEditingMode = false;
					}
				}
			}
		}

		// Token: 0x06009424 RID: 37924 RVA: 0x00450174 File Offset: 0x0044E374
		private void HandleSkillListChanged()
		{
			ViewCharacterMenuEquipCombatSkill.<>c__DisplayClass113_0 CS$<>8__locals1 = new ViewCharacterMenuEquipCombatSkill.<>c__DisplayClass113_0();
			this._equippedSkillDatas.Clear();
			this._unEquippedSkillDatas.Clear();
			List<CombatSkillDisplayDataCharacterMenuListItem> sourceList = this.SkillDatas ?? new List<CombatSkillDisplayDataCharacterMenuListItem>();
			ViewCharacterMenuEquipCombatSkill.<>c__DisplayClass113_0 CS$<>8__locals2 = CS$<>8__locals1;
			CombatSkillSortAndFilterController sortAndFilter = this.SortAndFilter;
			Func<IFilterableCombatSkill, bool> filter;
			if ((filter = ((sortAndFilter != null) ? sortAndFilter.GenerateFilter() : null)) == null && (filter = ViewCharacterMenuEquipCombatSkill.<>c.<>9__113_0) == null)
			{
				filter = (ViewCharacterMenuEquipCombatSkill.<>c.<>9__113_0 = ((IFilterableCombatSkill _) => true));
			}
			CS$<>8__locals2.filter = filter;
			CombatSkillSortAndFilterController sortAndFilter2 = this.SortAndFilter;
			Comparison<IFilterableCombatSkill> comparer = (sortAndFilter2 != null) ? sortAndFilter2.GenerateComparer(sourceList) : null;
			List<CombatSkillDisplayDataCharacterMenuListItem> outputList = sourceList.FindAll((CombatSkillDisplayDataCharacterMenuListItem d) => CS$<>8__locals1.filter(d));
			bool flag = comparer != null;
			if (flag)
			{
				outputList.Sort(comparer);
			}
			foreach (CombatSkillDisplayDataCharacterMenuListItem skillData in outputList)
			{
				bool flag2 = this.EquippedSkillDataDict.ContainsKey(skillData.TemplateId);
				if (flag2)
				{
					this._equippedSkillDatas.Add(skillData);
				}
				else
				{
					this._unEquippedSkillDatas.Add(skillData);
				}
			}
			bool flag3 = this.groupedSkillScroll != null;
			if (flag3)
			{
				this.groupedSkillScroll.Set(this, this._equippedSkillDatas, this._unEquippedSkillDatas, false);
			}
			CombatSkillSortAndFilterController sortAndFilter3 = this.SortAndFilter;
			if (sortAndFilter3 != null)
			{
				sortAndFilter3.AfterFilter(this.SkillDatas);
			}
		}

		// Token: 0x06009425 RID: 37925 RVA: 0x004502E0 File Offset: 0x0044E4E0
		protected override void OnClick(Transform btn)
		{
			string buttonName = btn.name;
			string text = buttonName;
			string a = text;
			if (!(a == "ChangeEditModeButton"))
			{
				if (!(a == "ChangeCombatSkillSortButton"))
				{
					if (!(a == "CloseSortCombatSkillButton"))
					{
						if (!(a == "AutoEquipButton"))
						{
							if (a == "PlanButtonReset")
							{
								UIManager.Instance.SetEscHandler(null);
								DialogCmd cmd = new DialogCmd
								{
									Title = LocalStringManager.Get(LanguageKey.LK_Equip_Skill_Order_Change_Reset_Title),
									Content = LocalStringManager.Get(LanguageKey.LK_Equip_Skill_Order_Change_Reset_Tips),
									Yes = delegate()
									{
										this.ResetSkillOrder();
									}
								};
								ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd);
								UIElement.Dialog.SetOnInitArgs(argBox);
								UIManager.Instance.MaskUI(UIElement.Dialog);
								UIElement dialog = UIElement.Dialog;
								dialog.OnHide = (Action)Delegate.Combine(dialog.OnHide, new Action(delegate()
								{
									UIManager.Instance.SetEscHandler(new Action(this.ExitCurrentMode));
								}));
							}
						}
						else
						{
							this.DoConfirmOnEquippedCombatSkill(new Action(this.<OnClick>g__AutoLoadCombatSkills|114_0), LanguageKey.LK_Equip_Skill_AutoLoad_Confirm, LanguageKey.LK_Equip_Skill_Autoload_Title);
						}
					}
					else
					{
						this.IsInSortCombatSkillMode = false;
					}
				}
				else
				{
					this.IsInSortCombatSkillMode = !this.IsInSortCombatSkillMode;
				}
			}
			else
			{
				this.IsInEditingMode = !this.IsInEditingMode;
			}
		}

		// Token: 0x06009426 RID: 37926 RVA: 0x00450434 File Offset: 0x0044E634
		private void OnDestroy()
		{
			this._isAwaitingEquipResponse = false;
			PoolItem skillItemPool = this._skillItemPool;
			if (skillItemPool != null)
			{
				skillItemPool.Destroy();
			}
			this._skillItemPool = null;
			PoolItem emptyGridPool = this._emptyGridPool;
			if (emptyGridPool != null)
			{
				emptyGridPool.Destroy();
			}
			this._emptyGridPool = null;
		}

		// Token: 0x06009427 RID: 37927 RVA: 0x00450470 File Offset: 0x0044E670
		public bool IsCombatSkillEquipped(short templateId)
		{
			return this.EquippedSkillDataDict.ContainsKey(templateId);
		}

		// Token: 0x06009428 RID: 37928 RVA: 0x00450490 File Offset: 0x0044E690
		public List<short> GetEquippedSkills(sbyte combatSkillEquipType)
		{
			return this.EquippedSkills[(int)combatSkillEquipType];
		}

		// Token: 0x06009429 RID: 37929 RVA: 0x004504AC File Offset: 0x0044E6AC
		private void ResetEquippedSkills()
		{
			for (sbyte type = 0; type < 5; type += 1)
			{
				List<short>[] equippedSkills = this.EquippedSkills;
				int num = (int)type;
				if (equippedSkills[num] == null)
				{
					equippedSkills[num] = new List<short>();
				}
				this._equipCombatSkillDisplayData.CurrentEquipPlan.GetValidSkills(type, this.EquippedSkills[(int)type]);
			}
		}

		// Token: 0x0600942A RID: 37930 RVA: 0x00450500 File Offset: 0x0044E700
		public void RequestData()
		{
			this._layoutDataReady = false;
			CharacterDomainMethod.Call.GetCombatSkillSlotCounts(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			CharacterDomainMethod.Call.GetCombatSkillExtraSlotCounts(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			CombatSkillDomainMethod.Call.GetEquipCombatSkillDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		}

		// Token: 0x0600942B RID: 37931 RVA: 0x0045056C File Offset: 0x0044E76C
		protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					this.HandleMethodReturn(notification, wrapper);
				}
			}
		}

		// Token: 0x0600942C RID: 37932 RVA: 0x004505DC File Offset: 0x0044E7DC
		private void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort num = domainId;
			if (num != 4)
			{
				if (num == 7)
				{
					if (notification.MethodId == 14)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._equipCombatSkillDisplayData);
						this._isUnavailable = (this._equipCombatSkillDisplayData == null || base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId));
						bool flag = this._equipCombatSkillDisplayData == null;
						if (flag)
						{
							this.localLoadingAnim.SetLoadingState(false);
						}
						else
						{
							EquipCombatSkillDisplayData equipCombatSkillDisplayData = this._equipCombatSkillDisplayData;
							if (equipCombatSkillDisplayData.CombatSkillDisplayDatas == null)
							{
								equipCombatSkillDisplayData.CombatSkillDisplayDatas = new List<CombatSkillDisplayDataCharacterMenuListItem>();
							}
							this.UpdateDisplayData();
							this._layoutDataReady = true;
							this.Refresh(true);
							this._isAwaitingEquipResponse = false;
							this.localLoadingAnim.SetLoadingState(false);
							bool flag2 = !this.Element.Ready;
							if (flag2)
							{
								this.Element.ShowAfterRefresh();
							}
						}
					}
				}
			}
			else if (notification.MethodId != 85)
			{
				if (notification.MethodId == 173)
				{
					sbyte[] result = null;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
					for (sbyte type = 0; type < 5; type += 1)
					{
						this.ExtraSpecificGridCount[(int)type] = result[(int)type];
					}
					this.ScheduleScrollLayoutIfReady();
				}
			}
			else
			{
				sbyte[] result = null;
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
				for (sbyte type2 = 0; type2 <= 5; type2 += 1)
				{
					this.CombatSkillSlotCounts[(int)type2] = result[(int)type2];
				}
				this.ScheduleScrollLayoutIfReady();
			}
		}

		// Token: 0x0600942D RID: 37933 RVA: 0x00450788 File Offset: 0x0044E988
		private void UpdateDisplayData()
		{
			bool flag = this._equipCombatSkillDisplayData == null;
			if (flag)
			{
				Debug.LogWarning("_equipCombatSkillDisplayData is null");
			}
			else
			{
				this.availableGenericGridCount = (sbyte)((int)this.TotalGenericGridCount - this.GenericGridAllocation.Sum());
				this.ResetEquippedSkills();
				HashSet<short> equippedSkillIdSet = EasyPool.Get<HashSet<short>>();
				for (sbyte i = 0; i < 5; i += 1)
				{
					foreach (short item in this.GetEquippedSkills(i))
					{
						equippedSkillIdSet.Add(item);
					}
				}
				this.EquippedSkillDataDict.Clear();
				foreach (CombatSkillDisplayDataCharacterMenuListItem skillData in this.SkillDatas)
				{
					bool flag2 = equippedSkillIdSet.Contains(skillData.TemplateId);
					if (flag2)
					{
						this.EquippedSkillDataDict[skillData.TemplateId] = skillData;
					}
				}
				EasyPool.Free<HashSet<short>>(equippedSkillIdSet);
				for (sbyte j = 0; j < 5; j += 1)
				{
					this.CombatSkillSlotAllocation[(int)j] = (sbyte)this.GetEquippedSkills(j).Sum((short x) => (int)((x != -1) ? this.EquippedSkillDataDict[x].GridCount : 0));
				}
				bool flag3 = this._equipCombatSkillDisplayData.CombatSkillOrderPlan.Items != null;
				if (flag3)
				{
					this._combatSkillOrderPlan.Clear();
					int skillCount = 0;
					foreach (short t in this._equipCombatSkillDisplayData.CombatSkillOrderPlan.Items)
					{
						this._combatSkillOrderPlan.Add(t);
						skillCount++;
					}
					while (skillCount < 54)
					{
						this._combatSkillOrderPlan.Add(-1);
						skillCount++;
					}
				}
				else
				{
					Debug.LogWarning("_equipCombatSkillDisplayData.CombatSkillOrderPlan.Items == null");
				}
			}
		}

		// Token: 0x0600942E RID: 37934 RVA: 0x004509B8 File Offset: 0x0044EBB8
		private void Refresh(bool needRefreshSkillScroll = true)
		{
			bool flag = this._equipCombatSkillDisplayData == null;
			if (flag)
			{
				Debug.LogWarning("_equipCombatSkillDisplayData is null");
			}
			else
			{
				this.availableGenericGridCountText.text = Math.Max(this.availableGenericGridCount, 0).ToString();
				this.lockToggle.isOn = this._equipCombatSkillDisplayData.IsCombatSkillLocked;
				for (int i = 0; i < 9; i++)
				{
					this.equipPlanToggleGroup.transform.GetChild(i).gameObject.SetActive(i < this._equipCombatSkillDisplayData.PlanCount);
				}
				this.equipPlanToggleGroup.SetWithoutNotify(this._equipCombatSkillDisplayData.CurrentPlanId);
				this.equipPlanToggleGroup.SetInteractable(this.CanOperate);
				this.copyPlanButton.interactable = (this.CanOperate && this._equipCombatSkillDisplayData.PlanCount < 9);
				this.copyPlanButton.GetComponent<TooltipInvoker>().PresetParam[0] = ((this._equipCombatSkillDisplayData.PlanCount < 9) ? LanguageKey.LK_CombatSkill_Equip_PlanCopy.Tr() : LanguageKey.LK_CombatSkill_Equip_PlanLimitMax.Tr());
				this.clearPlanButton.interactable = this.CanOperate;
				this.deletePlanButton.interactable = (this.CanOperate && this._equipCombatSkillDisplayData.PlanCount > 1);
				this.deletePlanButton.GetComponent<TooltipInvoker>().PresetParam[0] = ((this._equipCombatSkillDisplayData.PlanCount > 1) ? LanguageKey.LK_CombatSkill_Equip_PlanDelete.Tr() : LanguageKey.LK_CombatSkill_Equip_PlanDelete_Last.Tr());
				this.deletePlanButton.GetComponent<TooltipInvoker>().Refresh(false, -1);
				this.addPlanButton.interactable = (this.CanOperate && this._equipCombatSkillDisplayData.PlanCount < 9);
				this.addPlanButton.GetComponent<TooltipInvoker>().PresetParam[0] = ((this._equipCombatSkillDisplayData.PlanCount < 9) ? LanguageKey.LK_CombatSkill_Equip_PlanAppend.Tr() : LanguageKey.LK_CombatSkill_Equip_PlanLimitMax.Tr());
				this.addPlanButton.GetComponent<TooltipInvoker>().Refresh(false, -1);
				this.changeCombatSkillSortButton.interactable = this.CanOperate;
				this.changeEditModeButton.interactable = (this.CanOperate && !this.IsInEditingMode);
				this.autoEquipButton.interactable = this.CanOperate;
				this.favoriteToggle.interactable = this.CanOperate;
				foreach (EquipCombatSkillLine t in this.equipCombatSkillLines)
				{
					t.Set(this);
				}
				this.RebuildCombatSkillLineWithoutScrollByEquipType();
				this.ScheduleScrollLayoutIfReady();
				bool flag2 = !this.IsInPreview && needRefreshSkillScroll;
				if (flag2)
				{
					CombatSkillSortAndFilterController sortAndFilter = this.SortAndFilter;
					if (sortAndFilter != null)
					{
						sortAndFilter.NotifyDataChanged(this.SkillDatas);
					}
					this.HandleSkillListChanged();
					this.UpdateSortCombatSkillPanel();
					AttributeAndInjuryDynamic attributeAndInjuryDynamic = this.attributeAndInjury;
					if (attributeAndInjuryDynamic != null)
					{
						attributeAndInjuryDynamic.Refresh();
					}
				}
			}
		}

		// Token: 0x0600942F RID: 37935 RVA: 0x00450CCC File Offset: 0x0044EECC
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this._isAwaitingEquipResponse = false;
				this._scrollLayoutDeferUntilLoadingComplete = true;
				this.localLoadingAnim.SetLoadingState(true);
				this._layoutDataReady = false;
				this._scrollContentLayout.Invalidate();
				this.lockToggle.transform.parent.gameObject.SetActive(!base.CharacterMenu.CurrentCharacterIsTaiwu);
				this.changeCombatSkillSortButton.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwu);
				this.favoriteToggle.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwu);
				bool flag2 = this.attributeAndInjury != null;
				if (flag2)
				{
					this.attributeAndInjury.CharacterId = base.CharacterMenu.CurCharacterId;
				}
				this.RefreshSkillLineRoleDisplay();
				this.RequestData();
			}
		}

		// Token: 0x06009430 RID: 37936 RVA: 0x00450DB5 File Offset: 0x0044EFB5
		public void AllocateGenericGrid(sbyte type)
		{
			CharacterDomainMethod.Call.AllocateGenericGrid(base.CharacterMenu.CurCharacterId, type);
			this.LockAiEquip();
			this.RequestData();
		}

		// Token: 0x06009431 RID: 37937 RVA: 0x00450DD8 File Offset: 0x0044EFD8
		public void DeallocateGenericGrid(sbyte type)
		{
			CharacterDomainMethod.Call.DeallocateGenericGrid(base.CharacterMenu.CurCharacterId, type);
			this.LockAiEquip();
			this.RequestData();
		}

		// Token: 0x06009432 RID: 37938 RVA: 0x00450DFC File Offset: 0x0044EFFC
		public void AddEquippedCombatSkill(CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData)
		{
			bool revoked = combatSkillDisplayData.Revoked;
			if (!revoked)
			{
				CombatSkillItem config = this.GetCombatSkillConfig(combatSkillDisplayData);
				bool isAwaitingEquipResponse = this._isAwaitingEquipResponse;
				if (!isAwaitingEquipResponse)
				{
					bool flag = !this.IsInPreview || !this.HasEnoughBackendSlots(config.EquipType, (short)combatSkillDisplayData.GridCount);
					if (!flag)
					{
						this._isAwaitingEquipResponse = true;
						this._isInPreview = false;
						CharacterDomainMethod.Call.AddEquippedCombatSkill(base.CharacterMenu.CurCharacterId, combatSkillDisplayData.TemplateId);
						this.LockAiEquip();
						this.RequestData();
						bool flag2 = this.GetCombatSkillConfig(combatSkillDisplayData).EquipType == 4;
						if (flag2)
						{
							GlobalDomainMethod.Call.InvokeGuidingTrigger(227);
						}
					}
				}
			}
		}

		// Token: 0x06009433 RID: 37939 RVA: 0x00450EA8 File Offset: 0x0044F0A8
		private bool HasEnoughBackendSlots(sbyte equipType, short gridCount)
		{
			sbyte total = this.CombatSkillSlotCounts[(int)equipType];
			int generic = (int)((equipType > 0) ? this.GenericGridAllocation[(int)(equipType - 1)] : 0);
			EquipCombatSkillDisplayData equipCombatSkillDisplayData = this._equipCombatSkillDisplayData;
			bool flag = ((equipCombatSkillDisplayData != null) ? equipCombatSkillDisplayData.CurrentEquipPlan : null) == null;
			bool result;
			if (flag)
			{
				result = ((int)total + generic >= (int)gridCount);
			}
			else
			{
				List<short> skillIds = EasyPool.Get<List<short>>();
				this._equipCombatSkillDisplayData.CurrentEquipPlan.GetValidSkills(equipType, skillIds);
				int used = 0;
				foreach (short id in skillIds)
				{
					CombatSkillDisplayDataCharacterMenuListItem data;
					bool flag2 = this.EquippedSkillDataDict.TryGetValue(id, out data);
					if (flag2)
					{
						used += (int)data.GridCount;
					}
				}
				EasyPool.Free<List<short>>(skillIds);
				result = ((int)total + generic - used >= (int)gridCount);
			}
			return result;
		}

		// Token: 0x06009434 RID: 37940 RVA: 0x00450F90 File Offset: 0x0044F190
		public void RemoveEquippedCombatSkill(short templateId)
		{
			CharacterDomainMethod.Call.RemoveEquippedCombatSkill(base.CharacterMenu.CurCharacterId, templateId);
			this.LockAiEquip();
			this.RequestData();
		}

		// Token: 0x06009435 RID: 37941 RVA: 0x00450FB4 File Offset: 0x0044F1B4
		public void MasteredEquippedCombatSkill(short templateId, bool isMastered)
		{
			this._isInPreview = false;
			if (isMastered)
			{
				ExtraDomainMethod.Call.RemoveCharacterMasteredCombatSkill(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId, templateId);
			}
			else
			{
				ExtraDomainMethod.Call.AddCharacterMasteredCombatSkill(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId, templateId);
			}
			this.LockAiEquip();
			this.RequestData();
		}

		// Token: 0x06009436 RID: 37942 RVA: 0x0045101C File Offset: 0x0044F21C
		public void GetMasteredPreview(short templateId, bool isMastered, bool isPreview)
		{
			this._isInPreview = isPreview;
			bool flag = !isPreview;
			if (flag)
			{
				this.RequestData();
			}
			else
			{
				if (isMastered)
				{
					ExtraDomainMethod.Call.RemoveCharacterMasteredCombatSkill(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId, templateId);
				}
				else
				{
					ExtraDomainMethod.Call.AddCharacterMasteredCombatSkill(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId, templateId);
				}
				this.RequestData();
				bool flag2 = !isMastered;
				if (flag2)
				{
					ExtraDomainMethod.Call.RemoveCharacterMasteredCombatSkill(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId, templateId);
				}
				else
				{
					ExtraDomainMethod.Call.AddCharacterMasteredCombatSkill(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId, templateId);
				}
			}
		}

		// Token: 0x06009437 RID: 37943 RVA: 0x004510DC File Offset: 0x0044F2DC
		private void OnEditModeChanged()
		{
			bool flag = !this.IsInEditingMode;
			if (flag)
			{
				this.IsInPreview = false;
			}
			this.removeEquippedSkillHint.SetActive(this.IsInEditingMode);
			bool isInEditingMode = this.IsInEditingMode;
			if (isInEditingMode)
			{
				base.CharacterMenu.MoveCharacterScrollLeft(0.5f, Ease.OutExpo);
			}
			else
			{
				base.CharacterMenu.MoveCharacterScrollBack(0.5f, Ease.OutExpo);
			}
			this.changeEditModeButton.interactable = (this.CanOperate && !this.IsInEditingMode);
			this.SetTween();
			bool isInEditingMode2 = this.IsInEditingMode;
			if (isInEditingMode2)
			{
				UIManager.Instance.SetEscHandler(new Action(this.ExitCurrentMode));
			}
			else
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06009438 RID: 37944 RVA: 0x004511A0 File Offset: 0x0044F3A0
		private void SetTween()
		{
			int target = this.IsInEditingMode ? 1907 : 2230;
			this.viewBack.DOSizeDelta(new Vector2((float)target, this.viewBack.sizeDelta.y), 1f, false).SetEase(Ease.OutExpo);
			target = (this.IsInEditingMode ? 30 : -415);
			this.leftArea.DOAnchorPosX((float)target, 1f, false).SetEase(Ease.OutExpo);
			target = (this.IsInEditingMode ? 1160 : 2070);
			this.middleArea.DOSizeDelta(new Vector2((float)target, this.middleArea.sizeDelta.y), 1f, false).SetEase(Ease.OutExpo);
			target = (this.IsInEditingMode ? 0 : 800);
			this.rightArea.DOAnchorPosX((float)target, 1f, false).SetEase(Ease.OutExpo);
		}

		// Token: 0x06009439 RID: 37945 RVA: 0x00451294 File Offset: 0x0044F494
		private void OnInSortCombatSkillChanged()
		{
			RectTransform rect = this.removeEquippedSkillHint.GetComponent<RectTransform>();
			bool isInSortCombatSkillMode = this.IsInSortCombatSkillMode;
			if (isInSortCombatSkillMode)
			{
				UIManager.Instance.MaskComponent(this.sortCombatSkillPanel);
				rect.anchoredPosition = new Vector2(1074f, 50f);
			}
			else
			{
				UIManager.Instance.UnMaskComponent(this.sortCombatSkillPanel);
				rect.anchoredPosition = new Vector2(20f, 106.8f);
			}
			this.equipPlanToggleGroup.transform.SetParent(this.IsInSortCombatSkillMode ? this.toggleGroupRoot : this.equipPlanEditArea);
			((RectTransform)this.equipPlanToggleGroup.transform).anchoredPosition = new Vector2(0f, 0f);
			bool flag = this.IsInSortCombatSkillMode || this.IsInEditingMode;
			if (flag)
			{
				UIManager.Instance.SetEscHandler(new Action(this.ExitCurrentMode));
			}
			else
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x0600943A RID: 37946 RVA: 0x00451394 File Offset: 0x0044F594
		public void CombatSkillItemPreview(CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData, EEquipCombatSkillItemType eEquipCombatSkillItemType)
		{
			if (eEquipCombatSkillItemType == EEquipCombatSkillItemType.ScrollUnEquipped)
			{
				this.PreviewItemScrollUnEquipped(combatSkillDisplayData);
			}
		}

		// Token: 0x0600943B RID: 37947 RVA: 0x004513B8 File Offset: 0x0044F5B8
		private void PreviewItemScrollUnEquipped(CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData)
		{
			bool revoked = combatSkillDisplayData.Revoked;
			if (!revoked)
			{
				bool flag = this.EquippedSkillDataDict.ContainsKey(combatSkillDisplayData.TemplateId);
				if (!flag)
				{
					CombatSkillItem config = this.GetCombatSkillConfig(combatSkillDisplayData);
					EquipCombatSkillLine line = this.equipCombatSkillLines[(int)config.EquipType];
					bool flag2 = line.EmptyGridCount <= 0;
					if (flag2)
					{
						this._isInPreview = false;
					}
					else
					{
						sbyte[] combatSkillSlotAllocation = this.CombatSkillSlotAllocation;
						sbyte equipType = config.EquipType;
						combatSkillSlotAllocation[(int)equipType] = combatSkillSlotAllocation[(int)equipType] + combatSkillDisplayData.GridCount;
						this.EquippedSkills[(int)config.EquipType].Add(combatSkillDisplayData.TemplateId);
						this.EquippedSkillDataDict[combatSkillDisplayData.TemplateId] = combatSkillDisplayData;
						this.Refresh(false);
					}
				}
			}
		}

		// Token: 0x0600943C RID: 37948 RVA: 0x00451478 File Offset: 0x0044F678
		private void UpdateSortCombatSkillPanel()
		{
			bool flag = !base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (!flag)
			{
				this.ClearSkillPanel(this.sortSkillRootMain);
				this.ClearSkillPanel(this.sortSkillRootExtra);
				this.CreateSkillSlots(0, 18, this.sortSkillRootMain);
				this.CreateSkillSlots(18, 54, this.sortSkillRootExtra);
			}
		}

		// Token: 0x0600943D RID: 37949 RVA: 0x004514D8 File Offset: 0x0044F6D8
		public int GetIndexInSkillOrderPlan(int templateId)
		{
			return this._combatSkillOrderPlan.FindIndex((short x) => (int)x == templateId);
		}

		// Token: 0x0600943E RID: 37950 RVA: 0x00451510 File Offset: 0x0044F710
		private void ClearSkillPanel(Transform root)
		{
			int childCount = root.childCount;
			for (int i = childCount - 1; i >= 0; i--)
			{
				Transform obj = root.GetChild(i);
				EquipCombatSkillItem equipCombatSkillItem;
				bool flag = obj.TryGetComponent<EquipCombatSkillItem>(out equipCombatSkillItem);
				if (flag)
				{
					this._skillItemPool.DestroyObject(obj.gameObject);
				}
				else
				{
					this._emptyGridPool.DestroyObject(obj.gameObject);
				}
			}
		}

		// Token: 0x0600943F RID: 37951 RVA: 0x0045157C File Offset: 0x0044F77C
		private void CreateSkillSlots(int startIndex, int endIndex, Transform parent)
		{
			for (int i = startIndex; i < endIndex; i++)
			{
				int num = i;
				List<short> combatSkillOrderPlan = this._combatSkillOrderPlan;
				int? num2 = (combatSkillOrderPlan != null) ? new int?(combatSkillOrderPlan.Count) : null;
				bool flag = (num < num2.GetValueOrDefault() & num2 != null) && this._combatSkillOrderPlan[i] >= 0;
				if (flag)
				{
					this.CreateSkillItem(i, parent);
				}
				else
				{
					this.CreateEmptySlot(i, parent);
				}
			}
		}

		// Token: 0x06009440 RID: 37952 RVA: 0x00451604 File Offset: 0x0044F804
		private void CreateSkillItem(int index, Transform parent)
		{
			GameObject obj = this._skillItemPool.GetObject();
			obj.transform.SetParent(parent);
			obj.transform.SetAsLastSibling();
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			CombatSkillDisplayDataCharacterMenuListItem skillData = this.EquippedSkillDataDict[this._combatSkillOrderPlan[index]];
			EquipCombatSkillItem skillItem = obj.GetComponent<EquipCombatSkillItem>();
			skillItem.combatSkillItem.SetByCharacterMenuList(skillData);
			skillItem.Set(this, index, skillData, EEquipCombatSkillItemType.SortPanel, true);
		}

		// Token: 0x06009441 RID: 37953 RVA: 0x00451690 File Offset: 0x0044F890
		private void CreateEmptySlot(int index, Transform parent)
		{
			GameObject obj = this._emptyGridPool.GetObject();
			obj.transform.SetParent(parent);
			obj.transform.SetAsLastSibling();
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.transform.GetChild(1).gameObject.SetActive(false);
			int capturedIndex = index;
			obj.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.SetTargetSkillIndexTuple(capturedIndex);
			});
			PointerTrigger pointerTrigger = obj.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				GameObject hover = obj.transform.GetChild(2).gameObject;
				hover.SetActive(true);
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				GameObject hover = obj.transform.GetChild(2).gameObject;
				hover.SetActive(this.TargetSkillIndexTuple.Item1 == capturedIndex);
			});
		}

		// Token: 0x06009442 RID: 37954 RVA: 0x0045179C File Offset: 0x0044F99C
		public void SetTargetSkillIndexTuple(int index)
		{
			bool flag = this.TargetSkillIndexTuple.Item1 != -1;
			if (flag)
			{
				bool flag2 = index == this.TargetSkillIndexTuple.Item1;
				if (flag2)
				{
					this.TargetSkillIndexTuple.Item1 = -1;
				}
				else
				{
					this.TargetSkillIndexTuple.Item2 = index;
					this.SetCombatSkillOrderPlan();
					this.TargetSkillIndexTuple = new ValueTuple<int, int>(-1, -1);
				}
			}
			else
			{
				this.TargetSkillIndexTuple.Item1 = index;
			}
		}

		// Token: 0x06009443 RID: 37955 RVA: 0x00451818 File Offset: 0x0044FA18
		private void SetCombatSkillOrderPlan()
		{
			List<short> combatSkillOrderPlan = this._combatSkillOrderPlan;
			int item = this.TargetSkillIndexTuple.Item1;
			List<short> combatSkillOrderPlan2 = this._combatSkillOrderPlan;
			int item2 = this.TargetSkillIndexTuple.Item2;
			short value = this._combatSkillOrderPlan[this.TargetSkillIndexTuple.Item2];
			short value2 = this._combatSkillOrderPlan[this.TargetSkillIndexTuple.Item1];
			combatSkillOrderPlan[item] = value;
			combatSkillOrderPlan2[item2] = value2;
			ExtraDomainMethod.Call.SetCombatSkillOrderPlan(new GameData.Utilities.ShortList
			{
				Items = this._combatSkillOrderPlan
			});
			this.RequestData();
		}

		// Token: 0x06009444 RID: 37956 RVA: 0x004518B8 File Offset: 0x0044FAB8
		private void ResetSkillOrder()
		{
			this._combatSkillOrderPlan.Clear();
			for (sbyte type = 1; type < 4; type += 1)
			{
				List<short> segment = this.EquippedSkills[(int)type];
				foreach (short skillId in segment)
				{
					bool flag = skillId >= 0;
					if (flag)
					{
						this._combatSkillOrderPlan.Add(skillId);
					}
				}
			}
			while (this._combatSkillOrderPlan.Count < 54)
			{
				this._combatSkillOrderPlan.Add(-1);
			}
			ExtraDomainMethod.Call.SetCombatSkillOrderPlan(new GameData.Utilities.ShortList
			{
				Items = this._combatSkillOrderPlan
			});
			this.RequestData();
		}

		// Token: 0x06009445 RID: 37957 RVA: 0x00451990 File Offset: 0x0044FB90
		private void OnFavoriteModeChanged()
		{
			bool isInFavoriteMode = this.IsInFavoriteMode;
			if (isInFavoriteMode)
			{
				UIManager.Instance.MaskComponent(this.totalSkillsPageMaskNode);
			}
			else
			{
				UIManager.Instance.UnMaskComponent(this.totalSkillsPageMaskNode);
			}
			this.favoriteToggle.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwu);
			this.favoriteToggle.SetIsOnWithoutNotify(this.IsInFavoriteMode);
			this.Refresh(true);
			bool flag = this.IsInFavoriteMode || this.IsInEditingMode;
			if (flag)
			{
				UIManager.Instance.SetEscHandler(new Action(this.ExitCurrentMode));
			}
			else
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06009446 RID: 37958 RVA: 0x00451A3C File Offset: 0x0044FC3C
		public void FavoriteCombatSkill(short skillId, bool isFavorite)
		{
			if (isFavorite)
			{
				TaiwuDomainMethod.Call.AddFavoriteCombatSkill(skillId);
			}
			else
			{
				TaiwuDomainMethod.Call.RemoveFavoriteCombatSkill(skillId);
			}
			this.RequestData();
		}

		// Token: 0x06009447 RID: 37959 RVA: 0x00451A6C File Offset: 0x0044FC6C
		private void SetContentPosY(float normalizedVertical)
		{
			ScrollRect scrollRect = this.equipCombatSkillScrollRect;
			bool flag = ((scrollRect != null) ? scrollRect.content : null) == null;
			if (!flag)
			{
				ScrollRect scroll = this.equipCombatSkillScrollRect;
				RectTransform content = scroll.content;
				scroll.velocity = Vector2.zero;
				MouseWheelScale wheel = content.GetComponent<MouseWheelScale>();
				bool flag2 = wheel != null;
				if (flag2)
				{
					wheel.SetScaleProcess(0.3f);
				}
				Canvas.ForceUpdateCanvases();
				this.ContentDotween(Mathf.Clamp01(normalizedVertical));
			}
		}

		// Token: 0x06009448 RID: 37960 RVA: 0x00451AE8 File Offset: 0x0044FCE8
		private void ReleaseContentTweener()
		{
			bool flag = this.contentTweener != null;
			if (flag)
			{
				this.contentTweener.Kill(false);
				this.contentTweener = null;
			}
		}

		// Token: 0x06009449 RID: 37961 RVA: 0x00451B19 File Offset: 0x0044FD19
		private void ContentDotween(float aim)
		{
			this.ReleaseContentTweener();
			this.contentTweener = DOTween.To(() => this.equipCombatSkillScrollRect.verticalNormalizedPosition, delegate(float x)
			{
				this.equipCombatSkillScrollRect.verticalNormalizedPosition = x;
			}, aim, 0.5f).SetEase(Ease.OutQuad);
		}

		// Token: 0x0600944A RID: 37962 RVA: 0x00451B54 File Offset: 0x0044FD54
		private void RebuildCombatSkillLineWithoutScrollByEquipType()
		{
			if (this._combatSkillLineWithoutScrollByEquipType == null)
			{
				this._combatSkillLineWithoutScrollByEquipType = new Dictionary<sbyte, EquipCombatSkillLineWithoutScroll>();
			}
			this._combatSkillLineWithoutScrollByEquipType.Clear();
			bool flag = this.equipCombatSkillLines == null;
			if (!flag)
			{
				foreach (EquipCombatSkillLine line in this.equipCombatSkillLines)
				{
					EquipCombatSkillLineWithoutScroll ws = line as EquipCombatSkillLineWithoutScroll;
					bool flag2 = ws != null;
					if (flag2)
					{
						this._combatSkillLineWithoutScrollByEquipType[ws.CombatSkillEquipType] = ws;
					}
				}
			}
		}

		// Token: 0x0600944B RID: 37963 RVA: 0x00451BF8 File Offset: 0x0044FDF8
		public void CombatSkillLineItemSelected(EquipCombatSkillLineWithoutScroll item)
		{
			bool flag = this._combatSkillLineWithoutScrollByEquipType == null || this._combatSkillLineWithoutScrollByEquipType.Count == 0;
			if (flag)
			{
				this.RebuildCombatSkillLineWithoutScrollByEquipType();
			}
			foreach (KeyValuePair<sbyte, EquipCombatSkillLineWithoutScroll> kv in this._combatSkillLineWithoutScrollByEquipType)
			{
				bool flag2 = kv.Value != item;
				if (flag2)
				{
					kv.Value.OnSelectedCancel();
				}
			}
			item.OnSelected();
			this.SetContentPosY(item.PivotCenterY);
			this.IsInEditingMode = true;
			this.SetCombatSkillFilterType(item.CombatSkillEquipType);
		}

		// Token: 0x0600944C RID: 37964 RVA: 0x00451CB4 File Offset: 0x0044FEB4
		private void RefreshSkillLineRoleDisplay()
		{
			bool flag = this._isUnavailable || this.skillLine2Roles == null || this.skillLine2Roles.Length == 0;
			if (!flag)
			{
				this.HideAllSkillLineRoles();
				int charId = base.CharacterMenu.CurCharacterId;
				bool flag2 = charId < 0;
				if (!flag2)
				{
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForNeiliPage(this, charId, delegate(int offset, RawDataPool dataPool)
					{
						bool flag3 = charId != this.CharacterMenu.CurCharacterId;
						if (!flag3)
						{
							CharacterDisplayDataForNeiliPage data = null;
							Serializer.Deserialize(dataPool, offset, ref data);
							bool flag4 = data == null;
							if (!flag4)
							{
								this.HideAllSkillLineRoles();
								this.ApplySkillLineRoleDisplay(data.NeiliPercent);
							}
						}
					});
				}
			}
		}

		// Token: 0x0600944D RID: 37965 RVA: 0x00451D34 File Offset: 0x0044FF34
		private void HideAllSkillLineRoles()
		{
			bool flag = this.skillLine2Roles == null;
			if (!flag)
			{
				for (int i = 0; i < this.skillLine2Roles.Length; i++)
				{
					bool flag2 = this.skillLine2Roles[i] != null;
					if (flag2)
					{
						this.skillLine2Roles[i].SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600944E RID: 37966 RVA: 0x00451D8C File Offset: 0x0044FF8C
		private void ApplySkillLineRoleDisplay(NeiliProportionOfFiveElements proportion)
		{
			sbyte roleIndex = ViewCharacterMenuEquipCombatSkill.GetSkillLineRoleIndexFromFiveElements(proportion);
			bool flag = roleIndex >= 0 && (int)roleIndex < this.skillLine2Roles.Length && this.skillLine2Roles[(int)roleIndex] != null;
			if (flag)
			{
				this.skillLine2Roles[(int)roleIndex].SetActive(true);
			}
		}

		// Token: 0x0600944F RID: 37967 RVA: 0x00451DD4 File Offset: 0x0044FFD4
		private unsafe static sbyte GetSkillLineRoleIndexFromFiveElements(NeiliProportionOfFiveElements proportion)
		{
			sbyte dominantType = 0;
			sbyte dominantValue = *proportion[0];
			bool allEqual = true;
			for (sbyte i = 1; i < 5; i += 1)
			{
				sbyte value = *proportion[(int)i];
				bool flag = value != *proportion[0];
				if (flag)
				{
					allEqual = false;
				}
				bool flag2 = value > dominantValue;
				if (flag2)
				{
					dominantValue = value;
					dominantType = i;
				}
			}
			return allEqual ? 5 : dominantType;
		}

		// Token: 0x06009450 RID: 37968 RVA: 0x00451E48 File Offset: 0x00450048
		private CombatSkillItem GetCombatSkillConfig(CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData)
		{
			return CombatSkill.Instance[combatSkillDisplayData.TemplateId];
		}

		// Token: 0x06009451 RID: 37969 RVA: 0x00451E6C File Offset: 0x0045006C
		private bool IsEquippedCombatSkill()
		{
			return this.EquippedSkillDataDict.Count != 0;
		}

		// Token: 0x06009452 RID: 37970 RVA: 0x00451E8C File Offset: 0x0045008C
		private void DoConfirmOnEquippedCombatSkill(Action onConfirm, LanguageKey contentID, LanguageKey titleID = LanguageKey.LK_Common_Attention)
		{
			bool flag = this.IsEquippedCombatSkill();
			if (flag)
			{
				this.DoConfirm(onConfirm, contentID, titleID);
			}
			else if (onConfirm != null)
			{
				onConfirm();
			}
		}

		// Token: 0x06009453 RID: 37971 RVA: 0x00451EBC File Offset: 0x004500BC
		private void DoConfirm(Action onConfirm, LanguageKey contentID, LanguageKey titleID = LanguageKey.LK_Common_Attention)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(titleID),
				Content = LocalStringManager.Get(contentID),
				Yes = onConfirm
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x0600945A RID: 37978 RVA: 0x00452067 File Offset: 0x00450267
		[CompilerGenerated]
		private void <Awake>g__PlanClear|101_6()
		{
			CharacterDomainMethod.Call.UnequipAllCombatSkills(base.CharacterMenu.CurCharacterId);
			this.RequestData();
		}

		// Token: 0x0600945C RID: 37980 RVA: 0x004520A3 File Offset: 0x004502A3
		[CompilerGenerated]
		private void <Awake>g__PlanDelete|101_7()
		{
			CharacterDomainMethod.Call.DeleteCombatSkillPlan(base.CharacterMenu.CurCharacterId, this._equipCombatSkillDisplayData.CurrentPlanId);
			this.RequestData();
		}

		// Token: 0x0600945F RID: 37983 RVA: 0x004520DE File Offset: 0x004502DE
		[CompilerGenerated]
		private void <OnClick>g__AutoLoadCombatSkills|114_0()
		{
			CharacterDomainMethod.Call.AutoEquipCombatSkills(base.CharacterMenu.CurCharacterId);
			this.RequestData();
		}

		// Token: 0x040071F8 RID: 29176
		[SerializeField]
		private EquipCombatSkillItem equipCombatSkillItem;

		// Token: 0x040071F9 RID: 29177
		[SerializeField]
		private GameObject emptyGrid;

		// Token: 0x040071FA RID: 29178
		[SerializeField]
		private EquipCombatSkillGroupedSkillScroll groupedSkillScroll;

		// Token: 0x040071FB RID: 29179
		[SerializeField]
		private List<EquipCombatSkillLine> equipCombatSkillLines;

		// Token: 0x040071FC RID: 29180
		[SerializeField]
		private SortAndFilter commonSortAndFilter;

		// Token: 0x040071FD RID: 29181
		[SerializeField]
		private TextMeshProUGUI availableGenericGridCountText;

		// Token: 0x040071FE RID: 29182
		[SerializeField]
		private CToggleGroup equipPlanToggleGroup;

		// Token: 0x040071FF RID: 29183
		[SerializeField]
		private CButton addPlanButton;

		// Token: 0x04007200 RID: 29184
		[SerializeField]
		private CButton copyPlanButton;

		// Token: 0x04007201 RID: 29185
		[SerializeField]
		private CButton clearPlanButton;

		// Token: 0x04007202 RID: 29186
		[SerializeField]
		private CButton deletePlanButton;

		// Token: 0x04007203 RID: 29187
		[SerializeField]
		private CToggle lockToggle;

		// Token: 0x04007204 RID: 29188
		[SerializeField]
		private CButton changeCombatSkillSortButton;

		// Token: 0x04007205 RID: 29189
		[SerializeField]
		private CButton changeEditModeButton;

		// Token: 0x04007206 RID: 29190
		[SerializeField]
		private CButton autoEquipButton;

		// Token: 0x04007207 RID: 29191
		[SerializeField]
		private RectTransform equipPlanEditArea;

		// Token: 0x04007208 RID: 29192
		[SerializeField]
		private RectTransform sortSkillRootMain;

		// Token: 0x04007209 RID: 29193
		[SerializeField]
		private RectTransform sortSkillRootExtra;

		// Token: 0x0400720A RID: 29194
		[SerializeField]
		private RectTransform toggleGroupRoot;

		// Token: 0x0400720B RID: 29195
		[SerializeField]
		private CButton closeSortCombatSkillButton;

		// Token: 0x0400720C RID: 29196
		[SerializeField]
		private RectTransform viewBack;

		// Token: 0x0400720D RID: 29197
		[SerializeField]
		private RectTransform leftArea;

		// Token: 0x0400720E RID: 29198
		[SerializeField]
		private RectTransform middleArea;

		// Token: 0x0400720F RID: 29199
		[SerializeField]
		private RectTransform rightArea;

		// Token: 0x04007210 RID: 29200
		[SerializeField]
		private RectTransform sortCombatSkillPanel;

		// Token: 0x04007211 RID: 29201
		[SerializeField]
		private GameObject invisiblePage;

		// Token: 0x04007212 RID: 29202
		[SerializeField]
		private CToggle favoriteToggle;

		// Token: 0x04007213 RID: 29203
		[SerializeField]
		private GameObject removeEquippedSkillHint;

		// Token: 0x04007214 RID: 29204
		[SerializeField]
		private RectTransform editorModeMaskNode;

		// Token: 0x04007215 RID: 29205
		[SerializeField]
		private RectTransform totalSkillsPageMaskNode;

		// Token: 0x04007216 RID: 29206
		[SerializeField]
		private ScrollRect equipCombatSkillScrollRect;

		// Token: 0x04007217 RID: 29207
		[SerializeField]
		private GameObject[] skillLine2Roles;

		// Token: 0x04007218 RID: 29208
		[SerializeField]
		private AttributeAndInjuryDynamic attributeAndInjury;

		// Token: 0x04007219 RID: 29209
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x0400721A RID: 29210
		private PoolItem _skillItemPool;

		// Token: 0x0400721B RID: 29211
		private PoolItem _emptyGridPool;

		// Token: 0x0400721D RID: 29213
		private readonly List<CombatSkillDisplayDataCharacterMenuListItem> _equippedSkillDatas = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x0400721E RID: 29214
		private readonly List<CombatSkillDisplayDataCharacterMenuListItem> _unEquippedSkillDatas = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x0400721F RID: 29215
		private bool _isAwaitingEquipResponse;

		// Token: 0x04007220 RID: 29216
		private EquipCombatSkillDisplayData _equipCombatSkillDisplayData = new EquipCombatSkillDisplayData();

		// Token: 0x04007221 RID: 29217
		public readonly sbyte[] CombatSkillSlotCounts = new sbyte[6];

		// Token: 0x04007222 RID: 29218
		public readonly sbyte[] ExtraSpecificGridCount = new sbyte[5];

		// Token: 0x04007223 RID: 29219
		public readonly sbyte[] CombatSkillSlotAllocation = new sbyte[5];

		// Token: 0x04007224 RID: 29220
		public readonly List<short>[] EquippedSkills = new List<short>[5];

		// Token: 0x04007225 RID: 29221
		public sbyte availableGenericGridCount;

		// Token: 0x04007226 RID: 29222
		public readonly Dictionary<short, CombatSkillDisplayDataCharacterMenuListItem> EquippedSkillDataDict = new Dictionary<short, CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x04007227 RID: 29223
		public EquipCombatSkillItem TriggeredEquipCombatSkillItem;

		// Token: 0x04007228 RID: 29224
		private readonly List<short> _combatSkillOrderPlan = new List<short>(54);

		// Token: 0x04007229 RID: 29225
		private float originContentWidth;

		// Token: 0x0400722A RID: 29226
		private readonly EquipCombatSkillScrollContentLayout _scrollContentLayout = new EquipCombatSkillScrollContentLayout();

		// Token: 0x0400722B RID: 29227
		private bool _layoutDataReady;

		// Token: 0x0400722C RID: 29228
		private bool _scrollLayoutDeferUntilLoadingComplete;

		// Token: 0x0400722D RID: 29229
		private bool _isInEditingMode;

		// Token: 0x0400722E RID: 29230
		private bool _isInSortCombatSkillMode;

		// Token: 0x0400722F RID: 29231
		private bool _isInPreview;

		// Token: 0x04007230 RID: 29232
		private bool _isInFavoriteMode;

		// Token: 0x04007231 RID: 29233
		private bool _isUnavailable;

		// Token: 0x04007232 RID: 29234
		private const float ScrollContentPosYOnPageOpenClose = 0.5f;

		// Token: 0x04007233 RID: 29235
		private const int EditModeViewWidth = 1907;

		// Token: 0x04007234 RID: 29236
		private const int EditModeLeftArePosX = 30;

		// Token: 0x04007235 RID: 29237
		private const int EditModeMiddleAreWidth = 1160;

		// Token: 0x04007236 RID: 29238
		private const int EditModeRightPosX = 0;

		// Token: 0x04007237 RID: 29239
		private const int NormalModeViewWidth = 2230;

		// Token: 0x04007238 RID: 29240
		private const int NormalModeLeftArePosX = -415;

		// Token: 0x04007239 RID: 29241
		private const int NormalModeMiddleAreWidth = 2070;

		// Token: 0x0400723A RID: 29242
		private const int NormalModeRightPosX = 800;

		// Token: 0x0400723B RID: 29243
		private const float EditModeTransitionDuration = 1f;

		// Token: 0x0400723C RID: 29244
		public const int MainCombatSkillCount = 18;

		// Token: 0x0400723D RID: 29245
		public const int MaxCombatSkillCount = 54;

		// Token: 0x0400723E RID: 29246
		public ValueTuple<int, int> TargetSkillIndexTuple = new ValueTuple<int, int>(-1, -1);

		// Token: 0x0400723F RID: 29247
		private Tweener contentTweener = null;

		// Token: 0x04007240 RID: 29248
		private Dictionary<sbyte, EquipCombatSkillLineWithoutScroll> _combatSkillLineWithoutScrollByEquipType;
	}
}
