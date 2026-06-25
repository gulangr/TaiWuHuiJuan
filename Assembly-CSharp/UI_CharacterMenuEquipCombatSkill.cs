using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Views.CharacterMenu;
using Game.Views.Combat;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using PostProcessGlow;
using Spine;
using Spine.Unity;
using TMPro;
using UILogic.CharacterMenu.CharacterMenuEquipCombatSkill;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001D1 RID: 465
public class UI_CharacterMenuEquipCombatSkill : UI_CharacterMenuSubPageBase
{
	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001CCF RID: 7375 RVA: 0x000C9DD7 File Offset: 0x000C7FD7
	public override LanguageKey TitleKey
	{
		get
		{
			return LanguageKey.LK_CharacterMenu_Title_EquipCombatSkill;
		}
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x000C9DE0 File Offset: 0x000C7FE0
	public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
	{
		return curSubTogglePage == ECharacterSubToggleBase.EquipCombatSkillBase;
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001CD1 RID: 7377 RVA: 0x000C9DF6 File Offset: 0x000C7FF6
	public override bool ShowBaseAttribute
	{
		get
		{
			return this.CurTabIndex == 0;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001CD2 RID: 7378 RVA: 0x000C9E01 File Offset: 0x000C8001
	private CombatSkillModel CombatSkillModel
	{
		get
		{
			return SingletonObject.getInstance<CombatSkillModel>();
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001CD3 RID: 7379 RVA: 0x000C9E08 File Offset: 0x000C8008
	private BuildingModel BuildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001CD4 RID: 7380 RVA: 0x000C9E0F File Offset: 0x000C800F
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001CD5 RID: 7381 RVA: 0x000C9E1B File Offset: 0x000C801B
	private bool CurrCharIsTaiwu
	{
		get
		{
			return base.CharacterMenu.CurrentCharacterIsTaiwu;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001CD6 RID: 7382 RVA: 0x000C9E28 File Offset: 0x000C8028
	private bool IsTaiwuTeammateButNotBeast
	{
		get
		{
			return base.CharacterMenu.CurrentCharacterIsTaiwuTeammate && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001CD7 RID: 7383 RVA: 0x000C9E53 File Offset: 0x000C8053
	private bool IsTaiwuTeamButNotBeast
	{
		get
		{
			return base.CharacterMenu.IsTaiwuTeam && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001CD8 RID: 7384 RVA: 0x000C9E7E File Offset: 0x000C807E
	private bool IsInEditingMode
	{
		get
		{
			return this._layoutManager.IsEditingMode;
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001CD9 RID: 7385 RVA: 0x000C9E8B File Offset: 0x000C808B
	private bool IsTweeningToEditingMode
	{
		get
		{
			return this._layoutManager.IsTweenToEditingMode;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001CDA RID: 7386 RVA: 0x000C9E98 File Offset: 0x000C8098
	// (set) Token: 0x06001CDB RID: 7387 RVA: 0x000C9E9F File Offset: 0x000C809F
	public static bool IsChangingMasterState { get; set; }

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001CDC RID: 7388 RVA: 0x000C9EA7 File Offset: 0x000C80A7
	private bool IsInSkillOrderEditMode
	{
		get
		{
			RectTransform setSkillOrderPanel = this._setSkillOrderPanel;
			return setSkillOrderPanel != null && setSkillOrderPanel.gameObject.activeSelf;
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001CDD RID: 7389 RVA: 0x000C9EC0 File Offset: 0x000C80C0
	private short LoopingNeigong
	{
		get
		{
			return SingletonObject.getInstance<TaiwuCharacterModel>().LoopingNeigong;
		}
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x000C9ECC File Offset: 0x000C80CC
	private void ClearLocalDisplayDataCache()
	{
		bool flag = this._cachedCharId != base.CharacterMenu.CurCharacterId;
		if (flag)
		{
			this._localDisplayDataCache.Clear();
			this._cachedCharId = base.CharacterMenu.CurCharacterId;
		}
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x000C9F14 File Offset: 0x000C8114
	private bool TryGetDisplayData(int charId, short skillId, out CombatSkillDisplayData data)
	{
		data = this.GetDisplayData(charId, skillId);
		return data != null;
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x000C9F38 File Offset: 0x000C8138
	private CombatSkillDisplayData GetDisplayData(int charId, short skillId)
	{
		CombatSkillDisplayData cached;
		bool flag = this._localDisplayDataCache.TryGetValue(new ValueTuple<int, short>(charId, skillId), out cached);
		CombatSkillDisplayData result;
		if (flag)
		{
			result = cached;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x000C9F68 File Offset: 0x000C8168
	private bool IsDisplayDataReady(int charId, List<short> skillIds)
	{
		foreach (short skillId in skillIds)
		{
			bool flag = this.GetDisplayData(charId, skillId) == null;
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x000C9FCC File Offset: 0x000C81CC
	private bool IsReadyToRefreshEquippedSkills()
	{
		bool flag = this._dataMonitor == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			this.RefreshEquippedCombatSkills();
			bool skillsReady = this.IsDisplayDataReady(base.CharacterMenu.CurCharacterId, this._equippedCombatSkillList);
			bool gridsReady = this._genericGridAllocation[0] != byte.MaxValue;
			result = (skillsReady && gridsReady);
		}
		return result;
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x000CA024 File Offset: 0x000C8224
	private void TryRefreshEquippedSkills()
	{
		bool ready = this.IsReadyToRefreshEquippedSkills();
		bool flag = !base.gameObject.activeInHierarchy;
		if (!flag)
		{
			bool flag2 = !ready;
			if (!flag2)
			{
				this.UpdateEquippedSkills(true);
				bool flag3 = !this._isLearnedSkillsRequestSent;
				if (flag3)
				{
					this._isLearnedSkillsRequestSent = true;
					this.RequestLearnedSkillDisplayData();
				}
			}
		}
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x000CA07C File Offset: 0x000C827C
	private void RequestDisplayDataDirect(int charId, List<short> skillIds)
	{
		bool flag = skillIds == null;
		if (!flag)
		{
			bool flag2 = skillIds.Count == 0;
			if (flag2)
			{
				this.TryRefreshEquippedSkills();
				bool flag3 = this._dataMonitor != null && charId == this._dataMonitor.Character.CharacterId;
				if (flag3)
				{
					bool flag4 = this.IsDisplayDataReady(charId, this._dataMonitor.LearnedCombatSkills);
					if (flag4)
					{
						this.RefreshCombatSkillScroll(false, false);
					}
				}
			}
			else
			{
				List<short> toRequest = new List<short>();
				foreach (short skillId in skillIds)
				{
					bool flag5 = !this._localDisplayDataCache.ContainsKey(new ValueTuple<int, short>(charId, skillId));
					if (flag5)
					{
						toRequest.Add(skillId);
					}
				}
				bool flag6 = toRequest.Count > 0;
				if (flag6)
				{
					CombatSkillDomainMethod.Call.GetCombatSkillDisplayData(this.Element.GameDataListenerId, charId, toRequest);
				}
				else
				{
					this.TryRefreshEquippedSkills();
					bool flag7 = this._dataMonitor != null && charId == this._dataMonitor.Character.CharacterId;
					if (flag7)
					{
						bool flag8 = this.IsDisplayDataReady(charId, this._dataMonitor.LearnedCombatSkills);
						if (flag8)
						{
							this.RefreshCombatSkillScroll(false, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x000CA1E0 File Offset: 0x000C83E0
	private void OnReceiveDisplayDataDirect(List<CombatSkillDisplayData> dataList)
	{
		bool flag = dataList == null;
		if (!flag)
		{
			int charId = base.CharacterMenu.CurCharacterId;
			foreach (CombatSkillDisplayData data in dataList)
			{
				this._localDisplayDataCache[new ValueTuple<int, short>(charId, data.TemplateId)] = data;
			}
			this.TryRefreshEquippedSkills();
			bool flag2 = this.IsDisplayDataReady(charId, this._dataMonitor.LearnedCombatSkills);
			if (flag2)
			{
				this.RefreshCombatSkillScroll(false, false);
			}
		}
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x000CA288 File Offset: 0x000C8488
	public override void OnInit(ArgumentBox argsBox)
	{
		for (int i = 0; i < this._specificGridCount.Length; i++)
		{
			this._specificGridCount[i] = -1;
		}
		this.ClearAllWaitingFlag();
		this.NeiliPageOnInit();
		int tabIndex;
		bool flag = argsBox.Get("TabIndex", out tabIndex);
		if (flag)
		{
			this.CurTabIndex = tabIndex;
		}
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.UpdateCharacter));
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x000CA305 File Offset: 0x000C8505
	private void ClearAllWaitingFlag()
	{
		this._waitingAddSkillId = -1;
		this._waitingRemoveSkillId = -1;
		this._isWaitingGetGenericGridAllocation = false;
		this._pendingGenericGridChangeType = -1;
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x000CA324 File Offset: 0x000C8524
	private void SetAiAllocationLockToggleAndSave(bool value)
	{
		this._doNotSaveAiAllocationLockToggle = false;
		this._aiAllocationLockToggle.isOn = value;
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x000CA33B File Offset: 0x000C853B
	private void SetAiAllcationLockToggleWithoutSave(bool value)
	{
		this._doNotSaveAiAllocationLockToggle = true;
		this._aiAllocationLockToggle.isOn = value;
		this._doNotSaveAiAllocationLockToggle = false;
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x000CA35C File Offset: 0x000C855C
	private void OnAiAllocationLockToggle(bool isOn)
	{
		bool doNotSaveAiAllocationLockToggle = this._doNotSaveAiAllocationLockToggle;
		if (!doNotSaveAiAllocationLockToggle)
		{
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				CharacterDomainMethod.Call.SetNeiliAllocationLock(base.CharacterMenu.CurCharacterId, isOn);
				CharacterDomainMethod.Call.IsNeiliAllocationLocked(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			}
		}
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x000CA3B8 File Offset: 0x000C85B8
	private void OnAiEquipLockToggleChanged(bool isOn)
	{
		bool isTaiwuTeammateButNotBeast = this.IsTaiwuTeammateButNotBeast;
		if (isTaiwuTeammateButNotBeast)
		{
			CharacterDomainMethod.Call.SetCombatSkillPlanLock(base.CharacterMenu.CurCharacterId, isOn);
			CharacterDomainMethod.Call.IsCombatSkillEquipmentLocked(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		}
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x000CA400 File Offset: 0x000C8600
	public override void InitMonitorFieldIds()
	{
		this._updatedEquipPlan = -1;
		bool isTaiwuTeamButNotBeast = this.IsTaiwuTeamButNotBeast;
		if (isTaiwuTeamButNotBeast)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 5, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 113, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this.TaiwuCharId, new uint[]
			{
				17U,
				111U
			}));
		}
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x000CA47A File Offset: 0x000C867A
	private void Awake()
	{
		this.InitNeiliPageOnAwake();
		this.InitEquipSkill();
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x000CA48C File Offset: 0x000C868C
	private void InitEquipSkill()
	{
		this._equipSkillRefers = base.CGet<Refers>("EquipSkill");
		this._layoutManager = new LayoutManager(this._equipSkillRefers.GetComponent<UIHorizontalLayoutSwitcher>());
		this._selectSkillArgBox.Set("ShowLifeSkill", false);
		this._selectSkillArgBox.Set("ShowCombatSkill", true);
		this._aiEquipLockToggle = this._equipSkillRefers.CGet<CommonSwitch>("AiEquipLockToggle");
		this._aiEquipLockToggle.onValueChanged.RemoveAllListeners();
		this._aiEquipLockToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnAiEquipLockToggleChanged));
		this._aiEquipLockToggleObj = this._equipSkillRefers.CGet<GameObject>("AiEquipLockToggleObject");
		this._autoLoad = this._equipSkillRefers.CGet<CButtonObsolete>("AutoLoad");
		this._scrollLeftParticleList.Clear();
		this._scrollRightParticleList.Clear();
		this._slotTypeHolder = this._equipSkillRefers.CGet<RectTransform>("SlotTypeHolder");
		this._slotTypeHolderTop = this._equipSkillRefers.CGet<RectTransform>("SlotTypeHolderTop");
		for (sbyte i = 0; i < 5; i += 1)
		{
			sbyte type = i;
			Refers slotTypeRefers = this._slotTypeHolder.GetChild((int)i).GetComponent<Refers>();
			RectTransform slotHolder = slotTypeRefers.CGet<RectTransform>("SlotHolder");
			CButtonObsolete typeBackPointerTrigger = slotTypeRefers.CGet<CButtonObsolete>("TypeBackPointerTrigger");
			this._gridHoverHelper.RegisterCheck(typeBackPointerTrigger.GetComponent<RectTransform>(), type, false);
			this._gridHoverHelper.RegisterViewport(i, slotHolder.transform.parent.GetComponent<RectTransform>());
			typeBackPointerTrigger.ClearAndAddListener(delegate
			{
				this._currentSelectedEquipType = type;
				this.RefreshEquippedSkillHovers();
			});
			GameObject scrollLeftParticle = slotTypeRefers.CGet<GameObject>("ScrollLeftParticle");
			scrollLeftParticle.gameObject.SetActive(false);
			this._scrollLeftParticleList.Add(scrollLeftParticle);
			GameObject scrollRightParticle = slotTypeRefers.CGet<GameObject>("ScrollRightParticle");
			scrollRightParticle.gameObject.SetActive(false);
			this._scrollRightParticleList.Add(scrollRightParticle);
			TextMeshProUGUI typeName = slotTypeRefers.CGet<TextMeshProUGUI>("TypeName");
			CImage logo = slotTypeRefers.CGet<CImage>("Logo");
			typeName.text = LocalStringManager.Get("LK_CombatSkill_EquipType_" + i.ToString());
			logo.SetSprite("ui_sp_icon_qi_" + this._equipTypeLogos[(int)i].ToString(), false, null);
			EquipCombatSkillSlot slot = slotHolder.GetChild(0).GetComponent<EquipCombatSkillSlot>();
			bool flag = slot != null;
			if (flag)
			{
				this.InitEquipSkillSlot(slot, type);
				this.SetupEquipSkillSlot(slot, type, 0);
			}
			slotTypeRefers.CGet<CButtonObsolete>("AddGenericGrid").ClearAndAddListener(delegate
			{
				bool isWaitingGetGenericGridAllocation = this._isWaitingGetGenericGridAllocation;
				if (!isWaitingGetGenericGridAllocation)
				{
					CharacterDomainMethod.Call.AllocateGenericGrid(this.CharacterMenu.CurCharacterId, type);
					this._isWaitingGetGenericGridAllocation = true;
					this._pendingGenericGridChangeType = type;
					CharacterDomainMethod.Call.GetGenericGridAllocation(this.Element.GameDataListenerId, this.CharacterMenu.CurCharacterId);
				}
			});
			slotTypeRefers.CGet<CButtonObsolete>("ReduceGenericGrid").ClearAndAddListener(delegate
			{
				bool isWaitingGetGenericGridAllocation = this._isWaitingGetGenericGridAllocation;
				if (!isWaitingGetGenericGridAllocation)
				{
					bool flag2 = type < 1 || this._genericGridAllocation[(int)(type - 1)] <= 0;
					if (!flag2)
					{
						CharacterDomainMethod.Call.DeallocateGenericGrid(this.CharacterMenu.CurCharacterId, type);
						this._isWaitingGetGenericGridAllocation = true;
						this._pendingGenericGridChangeType = type;
						CharacterDomainMethod.Call.GetGenericGridAllocation(this.Element.GameDataListenerId, this.CharacterMenu.CurCharacterId);
					}
				}
			});
		}
		CToggleGroupObsolete planTogGroup = this._equipSkillRefers.CGet<CToggleGroupObsolete>("PlanHolder");
		planTogGroup.InitPreOnToggle(-1);
		planTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnPlanTogChange);
		foreach (CToggleObsolete toggle in planTogGroup.GetAll())
		{
			Refers refers = toggle.GetComponent<Refers>();
			List<TextMeshProUGUI> labelList = refers.CGetList<TextMeshProUGUI>("Label");
			foreach (TextMeshProUGUI lable in labelList)
			{
				lable.text = LocalStringManager.Get(string.Format("LK_TraditionalChineseNumber_{0}", toggle.Key + 1));
			}
		}
		this._setSkillOrderPanel = this._equipSkillRefers.CGet<RectTransform>("SetOrderPanel");
		this._skillOrderSlotHolder = this._equipSkillRefers.CGet<RectTransform>("SkillOrderHolder");
		this._combatSkillScrollView2 = this._equipSkillRefers.CGet<GroupedCombatSkillScrollView2>("GroupedCombatSkillScrollView");
		this._combatSkillScrollView2.Init("CombatSkill");
		this._combatSkillScrollView2.EnableDataChangeCheck = true;
		this._equipViewRightToggleGroup = this._equipSkillRefers.CGet<CToggleGroupObsolete>("RightToggleGroup");
		this._equipViewRightToggleGroup.InitPreOnToggle(-1);
		this._equipViewRightToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnEquipRightToggleGroupChange);
		this._equipViewAttributeView = this._equipSkillRefers.CGet<CharacterAttributeDataView>("CharacterAttributeView");
		this._equipViewAttributeView.InitElements();
		this._equipViewScrollArea = this._equipSkillRefers.CGet<GameObject>("ScrollArea");
		this.InitCombatSkillScrollViewMaterial();
		this._maskToFocusCombatSkillScroll = this._equipSkillRefers.CGet<GameObject>("MaskToFocusCombatSkillScroll");
		this._skillOrderFocusDict.Clear();
		RectTransform setOrderButton = this._equipSkillRefers.CGet<RectTransform>("SetCombatSkillOrder");
		this._skillOrderFocusDict[setOrderButton] = new ValueTuple<int, Transform>(setOrderButton.GetSiblingIndex(), setOrderButton.parent);
		for (int j = 0; j < this._skillOrderSlotHolder.childCount; j++)
		{
			int index = j;
			Refers slotRefers = this._skillOrderSlotHolder.GetChild(j).GetComponent<Refers>();
			slotRefers.UserInt = index;
			slotRefers.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
			{
				this.OnClickSkillOrderSlot(index);
			});
			CombatSkillView combatSkillView = slotRefers.CGet<CombatSkillView>("CombatSkillView");
			CImage combatSkillViewImage = combatSkillView.CGet<CImage>("CombatSkillView_CImage");
			combatSkillViewImage.raycastTarget = false;
		}
		this.RefreshGenericGridTips();
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x000CAA30 File Offset: 0x000C8C30
	private void OnEquipRightToggleGroupChange(CToggleObsolete newToggle, CToggleObsolete oldToggle)
	{
		int key = newToggle.Key;
		CommonUtils.SetActiveByMove(this._equipViewScrollArea.GetComponent<RectTransform>(), key == 0);
		this._equipViewAttributeView.gameObject.SetActive(key == 1);
		bool activeSelf = this._equipViewAttributeView.gameObject.activeSelf;
		if (activeSelf)
		{
			this._equipViewAttributeView.SetCurrentCharacterId(base.CharacterMenu.CurCharacterId);
		}
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x000CAA9C File Offset: 0x000C8C9C
	private void InitEquipSkillSlot(EquipCombatSkillSlot slot, sbyte type)
	{
		this._gridHoverHelper.RegisterCheck(slot.GetComponent<RectTransform>(), type, true);
	}

	// Token: 0x06001CF1 RID: 7409 RVA: 0x000CAAB4 File Offset: 0x000C8CB4
	private void SetupEquipSkillSlot(EquipCombatSkillSlot slot, sbyte type, int index)
	{
		UI_CharacterMenuEquipCombatSkill.<>c__DisplayClass111_0 CS$<>8__locals1 = new UI_CharacterMenuEquipCombatSkill.<>c__DisplayClass111_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.type = type;
		CS$<>8__locals1.index = index;
		UI_CharacterMenuEquipCombatSkill.<>c__DisplayClass111_0 CS$<>8__locals2 = CS$<>8__locals1;
		EquipCombatSkillMonitor dataMonitor = this._dataMonitor;
		CS$<>8__locals2.skillId = this.GetSkillIdByTypeAndIndex((dataMonitor != null) ? dataMonitor.EquippedCombatSkills : null, CS$<>8__locals1.type, CS$<>8__locals1.index);
		bool equipped = CS$<>8__locals1.skillId >= 0;
		CS$<>8__locals1.root = this._slotTypeHolderTop.GetChild((int)CS$<>8__locals1.type).GetComponent<Refers>().CGet<Refers>("ParticleArea");
		CombatSkillDisplayData skillData = null;
		bool flag = equipped && CS$<>8__locals1.skillId >= 0;
		if (flag)
		{
			this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, CS$<>8__locals1.skillId, out skillData);
		}
		slot.Setup(CS$<>8__locals1.type, CS$<>8__locals1.index, this.IsInEditingMode, this._realDataEquippedSkillSlotConfig, this._isDisablingPreviewParticleByRemoving, equipped, delegate
		{
			CS$<>8__locals1.<>4__this.OnClickSkillSlot(CS$<>8__locals1.type, CS$<>8__locals1.index);
		}, delegate
		{
			CS$<>8__locals1.<>4__this.OnClickSlotDeleteButton(CS$<>8__locals1.type, CS$<>8__locals1.index);
		}, delegate
		{
			UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? currentConfig = CS$<>8__locals1.<>4__this._realDataEquippedSkillSlotConfig;
			bool flag2 = currentConfig == null;
			if (!flag2)
			{
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line = currentConfig.Value.Lines[(int)CS$<>8__locals1.type];
				bool flag3 = CS$<>8__locals1.index < 0 || CS$<>8__locals1.index >= line.SlotConfigs.Count;
				if (!flag3)
				{
					int startX = line.SlotConfigs[CS$<>8__locals1.index].Index;
					UI_CharacterMenuEquipCombatSkill.GridParticleHelper.RefreshPreviewParticle(CS$<>8__locals1.root, startX, 1, UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddSkill);
				}
			}
		}, new Action(this.RefreshByExitPreview), delegate
		{
			CS$<>8__locals1.<>4__this.GeneratePreviewAndShowForRemoveSkill(CS$<>8__locals1.skillId);
		}, skillData, new Action<CombatSkillDisplayData>(this.OnSkillMasteryChangedInSlot), () => CS$<>8__locals1.<>4__this.IsInEditingMode);
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x000CABEC File Offset: 0x000C8DEC
	private void OnSkillMasteryChangedInSlot(CombatSkillDisplayData skillData)
	{
		bool flag = skillData == null;
		if (!flag)
		{
			this._localDisplayDataCache[new ValueTuple<int, short>(base.CharacterMenu.CurCharacterId, skillData.TemplateId)] = skillData;
			ArgumentBox args = new ArgumentBox();
			args.Set("CharId", base.CharacterMenu.CurCharacterId);
			args.Set("SkillId", skillData.TemplateId);
			GEvent.OnEvent(UiEvents.OnCombatSkillMasteryChanged, args);
			sbyte equipType = CombatSkill.Instance[skillData.TemplateId].EquipType;
			bool flag2 = equipType == 0;
			if (flag2)
			{
				this.RequireGridCounts();
				this.ForceRefreshEquippedSkillDisplayData(0, 4);
			}
			else
			{
				this.ForceRefreshEquippedSkillDisplayData(equipType);
			}
		}
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x000CACA6 File Offset: 0x000C8EA6
	private void InitCombatSkillScrollViewMaterial()
	{
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x000CACAC File Offset: 0x000C8EAC
	private void OnEnable()
	{
		bool flag = this._dataMonitor == null;
		if (flag)
		{
			this.SetupDataMonitor();
		}
		GEvent.Add(UiEvents.OnTaiwuLoopingNeigongMayChange, new GEvent.Callback(this.OnTaiwuLoopingNeigongMayChange));
		GEvent.Add(UiEvents.OnCombatSkillMasteryChanged, new GEvent.Callback(this.OnCombatSkillMasteryChanged));
		bool flag2 = ViewCharacterMenu.CurSubToggleIndex == ECharacterSubToggleBase.AttainmentBase;
		if (flag2)
		{
			this.HideCombatSkillScroll();
		}
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x000CAD1C File Offset: 0x000C8F1C
	private new void OnDisable()
	{
		GEvent.Remove(UiEvents.OnTaiwuLoopingNeigongMayChange, new GEvent.Callback(this.OnTaiwuLoopingNeigongMayChange));
		GEvent.Remove(UiEvents.OnCombatSkillMasteryChanged, new GEvent.Callback(this.OnCombatSkillMasteryChanged));
		this.ClearDataMonitor();
		this.ClearAllWaitingFlag();
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x000CAD70 File Offset: 0x000C8F70
	private void OnCombatSkillMasteryChanged(ArgumentBox args)
	{
		int charId;
		short skillId;
		bool flag = args.Get("CharId", out charId) && charId == base.CharacterMenu.CurCharacterId && args.Get("SkillId", out skillId);
		if (flag)
		{
			sbyte equipType = CombatSkill.Instance[skillId].EquipType;
			bool flag2 = equipType == 0;
			if (flag2)
			{
				this.RequireGridCounts();
				this.ForceRefreshEquippedSkillDisplayData(0, 4);
			}
			else
			{
				this.ForceRefreshEquippedSkillDisplayData(equipType);
			}
		}
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x000CADEA File Offset: 0x000C8FEA
	private void OnTaiwuLoopingNeigongMayChange(ArgumentBox argumentBox)
	{
		this.UpdateNeiliPageCombatSkill();
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x000CADF4 File Offset: 0x000C8FF4
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "SetCombatSkillOrder";
		if (flag)
		{
			bool flag2 = !this.IsInSkillOrderEditMode;
			if (flag2)
			{
				this.ShowSkillOrderPanel();
			}
			else
			{
				this.HideSkillOrderPanel(false);
			}
		}
		else
		{
			bool flag3 = btnName == "ResetOrder";
			if (flag3)
			{
				this._dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Equip_Skill_Order_Change_Reset_Title);
				this._dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Equip_Skill_Order_Change_Reset_Tips);
				this._dialogCmd.Yes = delegate()
				{
					this.ResetSkillOrder();
					this.InitSkillOrderList(false);
				};
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._dialogCmd);
				UIElement.Dialog.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				bool flag4 = btnName == "ConfirmOrder";
				if (flag4)
				{
					this.HideSkillOrderPanel(true);
				}
				else
				{
					bool flag5 = btnName == "CancelOrder";
					if (flag5)
					{
						this.HideSkillOrderPanel(false);
					}
					else
					{
						bool flag6 = btnName == "NeiliPanel";
						if (flag6)
						{
							ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
							argBox2.Set("NeiliType", this._dataMonitor.NeiliType);
							UIElement.FiveElementsPanel.SetOnInitArgs(argBox2);
							UIManager.Instance.ShowUI(UIElement.FiveElementsPanel, true);
						}
						else
						{
							bool flag7 = btnName == "SelectSkillButton";
							if (flag7)
							{
								this.OnCombatSkillClick();
							}
							else
							{
								bool flag8 = btnName == "OpenLoopUIButton";
								if (flag8)
								{
									this.OnCombatSkillClick();
								}
								else
								{
									bool flag9 = btnName == "AutoLoad";
									if (flag9)
									{
										this.OnClickAutoLoad();
									}
									else
									{
										bool flag10 = btnName == "PlanAppend";
										if (flag10)
										{
											this.PlanAppend();
										}
										else
										{
											bool flag11 = btnName == "PlanCopy";
											if (flag11)
											{
												this.PlanCopy();
											}
											else
											{
												bool flag12 = btnName == "PlanClear";
												if (flag12)
												{
													this.DoConfirmOnEquippedCombatSkill(new Action(this.PlanClear), LanguageKey.LK_CombatSkill_Equip_PlanClear_Confirm, LanguageKey.LK_Common_Attention);
												}
												else
												{
													bool flag13 = btnName == "PlanDelete";
													if (flag13)
													{
														this.DoConfirmOnEquippedCombatSkill(new Action(this.PlanDelete), LanguageKey.LK_CombatSkill_Equip_PlanDelete_Confirm, LanguageKey.LK_Common_Attention);
													}
													else
													{
														bool flag14 = btnName == "AutoLoadAllocationButton";
														if (flag14)
														{
															CharacterDomainMethod.Call.AutoAllocateNeili(base.CharacterMenu.CurCharacterId);
															bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
															if (currentCharacterIsTaiwuTeammate)
															{
																this.SetAiAllocationLockToggleAndSave(false);
															}
														}
														else
														{
															bool flag15 = btnName == "EditButton";
															if (flag15)
															{
																bool flag16 = !this.IsInEditingMode;
																if (flag16)
																{
																	this.OpenCombatSkillScroll(false, -1);
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
						}
					}
				}
			}
		}
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x000CB0BF File Offset: 0x000C92BF
	private void OnCombatSkillClick()
	{
		UIManager.Instance.ShowUI(UIElement.Looping, true);
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x000CB0D3 File Offset: 0x000C92D3
	private IEnumerator ShowSelfCoroutine()
	{
		bool currentLanguageGlowGenerated = this._generatedGlowImages.ContainsKey(this.CurLanguageType);
		yield return new WaitUntil(delegate()
		{
			EquipCombatSkillMonitor dataMonitor = this._dataMonitor;
			return (dataMonitor != null && dataMonitor.Init) & currentLanguageGlowGenerated;
		});
		NeiliTypeItem typeConfig = NeiliType.Instance[this._dataMonitor.NeiliType];
		bool showLine = typeConfig.LinePos != null;
		this._neiliRefers.CGet<Transform>("BlueLine").gameObject.SetActive(showLine && typeConfig.ColorType == 1);
		this._neiliRefers.CGet<Transform>("RedLine").gameObject.SetActive(showLine && typeConfig.ColorType == 2);
		base.CharacterMenu.SetCurPageSubpage(this.CurTabIndex);
		this.Element.ShowAfterRefresh();
		yield break;
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x000CB0E2 File Offset: 0x000C92E2
	private void RefreshPageObjectActive()
	{
		this.RefreshObjectActiveInTab1(false);
		this.RefreshObjectActiveInTab0();
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x000CB0F4 File Offset: 0x000C92F4
	public override void OnSwitchToSubpage(int subPageIndex)
	{
		this.CurTabIndex = subPageIndex;
		bool flag = subPageIndex == 0;
		if (flag)
		{
			this.OnConsummateLevelChange();
		}
		this.RefreshObjectActiveInTab1(false);
		this.RefreshObjectActiveInTab0();
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x000CB12C File Offset: 0x000C932C
	public override void OnSubpageVisible()
	{
		this.NeiliPageOnSubpageVisible();
		this._equipSkillRefers.CGet<CToggleGroupObsolete>("PlanHolder").transform.parent.gameObject.SetActive(base.CharacterMenu.CanOperate);
		this._equipViewRightToggleGroup.Set(1, true, false);
		base.StartCoroutine(this.ShowSelfCoroutine());
		this.ClearAllWaitingFlag();
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x000CB194 File Offset: 0x000C9394
	protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type3 = notification.Type;
			byte b = type3;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 173;
						if (flag2)
						{
							sbyte[] result = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
							for (sbyte type = 0; type < 5; type += 1)
							{
								this._extraSpecificGridCount[(int)type] = result[(int)type];
							}
						}
						bool flag3 = notification.MethodId == 85;
						if (flag3)
						{
							sbyte[] result2 = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result2);
							for (sbyte type2 = 0; type2 < 5; type2 += 1)
							{
								this._specificGridCount[(int)type2] = result2[(int)type2];
							}
							this._totalGenericGrid = result2[5];
						}
						bool flag4 = notification.MethodId == 157;
						if (flag4)
						{
							ValueTuple<int, int> tuple = new ValueTuple<int, int>(-1, -1);
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref tuple);
							ValueTuple<int, int> valueTuple = tuple;
							int id = valueTuple.Item1;
							int count = valueTuple.Item2;
							this._currEquipPlan = id;
							this._unlockedCombatSkillPlanCount = count;
							CToggleGroupObsolete planTogGroup = this._equipSkillRefers.CGet<CToggleGroupObsolete>("PlanHolder");
							bool flag5 = planTogGroup.GetActive().Key != this._currEquipPlan;
							if (flag5)
							{
								this._autoUpdatingPlanTog = true;
								planTogGroup.SetWithoutNotify(this._currEquipPlan, true);
								this._autoUpdatingPlanTog = false;
							}
							this.UpdateCurrentPlanDisplayData();
							bool isInSkillOrderEditMode = this.IsInSkillOrderEditMode;
							if (isInSkillOrderEditMode)
							{
								this.InitSkillOrderList(true);
							}
							this.RefreshPlanToggles();
						}
						bool flag6 = notification.MethodId == 163;
						if (flag6)
						{
							bool locked = false;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref locked);
							this._aiEquipLockToggle.isOn = locked;
						}
						bool flag7 = notification.MethodId == 160;
						if (flag7)
						{
							bool locked2 = false;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref locked2);
							this.SetAiAllcationLockToggleWithoutSave(locked2);
						}
						bool flag8 = notification.MethodId == 169;
						if (flag8)
						{
							this._isWaitingGetGenericGridAllocation = false;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._genericGridAllocation);
							bool flag9 = this._pendingGenericGridChangeType >= 0;
							if (flag9)
							{
								this.ForceRefreshEquippedSkillDisplayData(this._pendingGenericGridChangeType);
								this._pendingGenericGridChangeType = -1;
							}
							else
							{
								this.TryRefreshEquippedSkills();
							}
							bool isInEditingMode = this.IsInEditingMode;
							if (isInEditingMode)
							{
								this.RefreshCombatSkillScroll(false, true);
							}
							CharacterDomainMethod.Call.GetCurrentPlanIdAndPlanCount(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
						}
					}
					else
					{
						bool flag10 = notification.DomainId == 5;
						if (!flag10)
						{
							bool flag11 = notification.DomainId == 7 && notification.MethodId == 0;
							if (flag11)
							{
								List<CombatSkillDisplayData> dataList = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
								this.OnReceiveDisplayDataDirect(dataList);
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag12 = uid.DomainId == 5 && uid.DataId == 16;
				if (!flag12)
				{
					bool flag13 = uid.DomainId == 19 && uid.DataId == 5;
					if (flag13)
					{
						Serializer.DeserializeModifications<int>(wrapper.DataPool, notification.ValueOffset, this._skillOrderPlans);
						bool isInSkillOrderEditMode2 = this.IsInSkillOrderEditMode;
						if (isInSkillOrderEditMode2)
						{
							this.InitSkillOrderList(true);
						}
					}
					else
					{
						bool flag14 = uid.DomainId == 19 && uid.DataId == 113;
						if (!flag14)
						{
							bool flag15 = uid.DomainId == 8 && uid.DataId == 10 && uid.SubId1 == 3U;
							if (flag15)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatNeiliAllocation);
								this.OnNeiliAllocationChange();
							}
							else
							{
								bool flag16 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == base.CharacterMenu.CurCharacterId && uid.SubId1 == 17U;
								if (flag16)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._featureIds);
									this.UpdateNeiliAllocation();
								}
								else
								{
									bool flag17 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == base.CharacterMenu.CurCharacterId && uid.SubId1 == 111U;
									if (flag17)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._neiliType);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x000CB69C File Offset: 0x000C989C
	private void RefreshPlanToggles()
	{
		Transform planHolder = this._equipSkillRefers.CGet<CToggleGroupObsolete>("PlanHolder").transform;
		for (int i = 0; i < 9; i++)
		{
			planHolder.GetChild(i).gameObject.SetActive(i < this._unlockedCombatSkillPlanCount);
		}
		this._equipSkillRefers.CGet<UISwitcher>("PlanAppend").Switch(this._unlockedCombatSkillPlanCount < 9);
		this._equipSkillRefers.CGet<UISwitcher>("PlanCopy").Switch(this._unlockedCombatSkillPlanCount < 9);
		UISwitcher deleteSwitch = this._equipSkillRefers.CGet<UISwitcher>("PlanDelete");
		bool canDelete = this._unlockedCombatSkillPlanCount > 1;
		deleteSwitch.Switch(canDelete);
		TooltipInvoker deleteSwitchTip = deleteSwitch.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = deleteSwitchTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		deleteSwitchTip.RuntimeParam.Set("arg0", (canDelete ? LanguageKey.LK_CombatSkill_Equip_PlanDelete : LanguageKey.LK_CombatSkill_Equip_PlanDelete_Last).Tr());
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x000CB7A0 File Offset: 0x000C99A0
	private void OpenCombatSkillScroll(bool init = false, sbyte initialEquipType = -1)
	{
		bool isTweenToNoneEditMode = this._layoutManager.IsTweenToNoneEditMode;
		if (!isTweenToNoneEditMode)
		{
			this._currentSelectedEquipType = initialEquipType;
			this.RefreshEquippedSkillHovers();
			this._layoutManager.SwitchToEditingMode(true, delegate
			{
				this.UpdateGenericGrid();
			}, null);
			base.CharacterMenu.OnTryClosePage = delegate()
			{
				this.HideCombatSkillScroll();
				base.CharacterMenu.OnTryClosePage = null;
			};
			this._gridHoverHelper.HideAllGridHoverArea(true);
			this._equipViewRightToggleGroup.Set(0, true, false);
			this.RefreshEditButton(true);
			this._maskToFocusCombatSkillScroll.gameObject.SetActive(true);
			this._maskToFocusCombatSkillScroll.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).SetEase(Ease.OutQuart);
			GameObject focusObject2 = this._equipSkillRefers.CGet<GameObject>("FocusObjects2");
			focusObject2.SetActive(true);
			base.CharacterMenu.SetCharacterListAlpha(0f);
		}
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x000CB884 File Offset: 0x000C9A84
	private void RefreshEditButton(bool isInEditingMode)
	{
		CButtonObsolete cButton = this._equipSkillRefers.CGet<CButtonObsolete>("EditButton");
		cButton.gameObject.SetActive(true);
		cButton.interactable = (base.CharacterMenu.CanOperate && !isInEditingMode);
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x000CB8CC File Offset: 0x000C9ACC
	private void HideCombatSkillScroll()
	{
		bool isTweenToEditingMode = this._layoutManager.IsTweenToEditingMode;
		if (isTweenToEditingMode)
		{
			base.DelayFrameCall(delegate
			{
				base.CharacterMenu.OnTryClosePage = new Action(this.HideCombatSkillScroll);
			}, 1U);
		}
		else
		{
			this._layoutManager.SwitchToNoneEditMode(true, delegate
			{
				this._maskToFocusCombatSkillScroll.gameObject.SetActive(false);
			}, delegate(float aToBValue)
			{
				this._maskToFocusCombatSkillScroll.GetComponent<CanvasGroup>().alpha = aToBValue;
			});
			this.RefreshEditButton(false);
			this.UnselectTargetSlot();
			this._equipSkillRefers.CGet<GameObject>("FocusObjects2").SetActive(false);
			this.UpdateEquippedSkills(true);
			base.CharacterMenu.SetCharacterListAlpha(1f);
			this._equipViewRightToggleGroup.Set(1, true, false);
		}
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x000CB974 File Offset: 0x000C9B74
	private int CompareCombatSkill(CombatSkillDisplayData l, CombatSkillDisplayData r)
	{
		bool lCanPut = this.CanPutSkill(l.TemplateId);
		bool rCanPut = this.CanPutSkill(r.TemplateId);
		bool flag = lCanPut != rCanPut;
		int result;
		if (flag)
		{
			result = (lCanPut ? -1 : 1);
		}
		else
		{
			result = l.TemplateId.CompareTo(r.TemplateId);
		}
		return result;
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x000CB9C8 File Offset: 0x000C9BC8
	private int GetUiSlotIndex(CombatSkillEquipment equipment, sbyte type, short skillId)
	{
		List<short> skills = this.GetReuseSkillList("GetUiSlotIndex");
		equipment.GetValidSkills(type, skills);
		int index = 0;
		foreach (short slotSkillId in skills)
		{
			CombatSkillDisplayData skillData;
			bool flag = this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, slotSkillId, out skillData);
			if (flag)
			{
				sbyte gridCount = skillData.GridCount;
				bool flag2 = slotSkillId == skillId;
				if (flag2)
				{
					return index;
				}
				index += (int)gridCount;
			}
		}
		return -1;
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x000CBA6C File Offset: 0x000C9C6C
	private List<short> GetReuseSkillList(string funcName)
	{
		List<short> list;
		bool flag = !UI_CharacterMenuEquipCombatSkill.ReuseSkillLists.TryGetValue(funcName, out list);
		if (flag)
		{
			list = new List<short>();
			UI_CharacterMenuEquipCombatSkill.ReuseSkillLists.Add(funcName, list);
		}
		list.Clear();
		return list;
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x000CBAB0 File Offset: 0x000C9CB0
	private void GeneratePreviewAndShowForAddSkill(short skillId)
	{
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? currentConfig = this._realDataEquippedSkillSlotConfig;
		bool flag = currentConfig == null;
		if (flag)
		{
			this._realDataEquippedSkillSlotConfig = this.GenerateCurrentEquippedSkillConfig();
			currentConfig = this._realDataEquippedSkillSlotConfig;
			bool flag2 = currentConfig == null;
			if (flag2)
			{
				return;
			}
		}
		sbyte equipType = CombatSkill.Instance[skillId].EquipType;
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? modified = this.ModifyConfigByAddSkill(currentConfig.Value, skillId, equipType);
		bool flag3 = modified == null;
		if (!flag3)
		{
			this.UpdateEquippedSkills(modified.Value);
			this.RefreshAvailableGenericGrid(modified.Value.GenericGridCountConfig);
			this.RefreshGridCount(modified.Value, false);
		}
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x000CBB60 File Offset: 0x000C9D60
	private void GeneratePreviewAndShowForRemoveSkill(short skillId)
	{
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? currentConfig = this._realDataEquippedSkillSlotConfig;
		bool flag = currentConfig == null;
		if (flag)
		{
			this._realDataEquippedSkillSlotConfig = this.GenerateCurrentEquippedSkillConfig();
			currentConfig = this._realDataEquippedSkillSlotConfig;
			bool flag2 = currentConfig == null;
			if (flag2)
			{
				return;
			}
		}
		sbyte equipType = CombatSkill.Instance[skillId].EquipType;
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? modified = this.ModifyConfigByRemoveSkill(currentConfig.Value, skillId, equipType);
		bool flag3 = modified == null;
		if (!flag3)
		{
			this.UpdateEquippedSkills(modified.Value);
			this.RefreshAvailableGenericGrid(modified.Value.GenericGridCountConfig);
			this.RefreshGridCount(modified.Value, false);
		}
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x000CBC10 File Offset: 0x000C9E10
	private void GeneratePreviewAndShowForChangeGenericGridCount(sbyte equipType, bool isAdd)
	{
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? currentConfig = this._realDataEquippedSkillSlotConfig;
		bool flag = currentConfig == null;
		if (flag)
		{
			this._realDataEquippedSkillSlotConfig = this.GenerateCurrentEquippedSkillConfig();
			currentConfig = this._realDataEquippedSkillSlotConfig;
			bool flag2 = currentConfig == null;
			if (flag2)
			{
				return;
			}
		}
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? modified = this.ModifyConfigByChangeGenericGridCount(currentConfig.Value, equipType, isAdd);
		bool flag3 = modified == null;
		if (!flag3)
		{
			this.UpdateEquippedSkills(modified.Value);
			this.RefreshAvailableGenericGrid(modified.Value.GenericGridCountConfig);
			this.RefreshGridCount(modified.Value, true);
		}
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x000CBCAC File Offset: 0x000C9EAC
	private void HideTargetSlotSelectedEffectByEquipType(sbyte equipType)
	{
		Refers root = this._slotTypeHolderTop.GetChild((int)equipType).GetComponent<Refers>().CGet<Refers>("ParticleArea");
		UI_CharacterMenuEquipCombatSkill.GridParticleHelper.HidePreviewParticle(root);
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x000CBCE0 File Offset: 0x000C9EE0
	private int CountUsedGrids(sbyte equipType)
	{
		List<short> skills = this.GetReuseSkillList("CountUsedGrids");
		this._dataMonitor.EquippedCombatSkills.GetValidSkills(equipType, skills);
		return skills.Sum(delegate(short skillId)
		{
			CombatSkillDisplayData displayData = this.GetDisplayData(base.CharacterMenu.CurCharacterId, skillId);
			return (int)((displayData != null) ? displayData.GridCount : 0);
		});
	}

	// Token: 0x06001D0B RID: 7435 RVA: 0x000CBD23 File Offset: 0x000C9F23
	private void UnselectTargetSlot()
	{
	}

	// Token: 0x06001D0C RID: 7436 RVA: 0x000CBD28 File Offset: 0x000C9F28
	private void RefreshCombatSkillScroll(bool init = false, bool forceRefresh = false)
	{
		this._typeFilteredCombatSkillList.Clear();
		this._combatSkillIdList = this._dataMonitor.LearnedCombatSkills;
		foreach (short id in this._combatSkillIdList)
		{
			CombatSkillDisplayData data;
			bool flag = this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, id, out data);
			if (flag)
			{
				this._typeFilteredCombatSkillList.Add(data);
			}
		}
		this.RefreshDisabledCombatSkills();
		this._typeFilteredCombatSkillList.Sort(new Comparison<CombatSkillDisplayData>(this.CompareCombatSkill));
		bool shouldReset = init || !this._combatSkillScrollInitialized;
		this._combatSkillScrollView2.SetSkillList(this._typeFilteredCombatSkillList, new GroupedCombatSkillScrollView2.CombatSkillGroupGetter(this.GroupSkillsByEquipStatus), new Action<CombatSkillDisplayData, Refers>(this.OnRenderCombatSkill), new Action<GroupedCombatSkillScrollView2.CombatSkillGroup, Refers>(this.OnRenderCombatSkillGroupTitle), shouldReset, forceRefresh);
		bool flag2 = shouldReset;
		if (flag2)
		{
			this._combatSkillScrollInitialized = true;
		}
		bool flag3 = this._waitingRemoveSkillId >= 0 && !this.IsSkillEquipped(this._waitingRemoveSkillId);
		if (flag3)
		{
			this._waitingRemoveSkillId = -1;
			this._isDisablingPreviewParticleByRemoving = true;
			base.DelayFrameCall(delegate
			{
				this._isDisablingPreviewParticleByRemoving = false;
			}, 2U);
		}
		bool flag4 = this._waitingAddSkillId >= 0 && this.IsSkillEquipped(this._waitingAddSkillId);
		if (flag4)
		{
			this._waitingAddSkillId = -1;
		}
	}

	// Token: 0x06001D0D RID: 7437 RVA: 0x000CBEA0 File Offset: 0x000CA0A0
	private List<GroupedCombatSkillScrollView2.CombatSkillGroup> GroupSkillsByEquipStatus(List<CombatSkillDisplayData> skillList)
	{
		List<GroupedCombatSkillScrollView2.CombatSkillGroup> groups = new List<GroupedCombatSkillScrollView2.CombatSkillGroup>();
		GroupedCombatSkillScrollView2.CombatSkillGroup equippedGroup = new GroupedCombatSkillScrollView2.CombatSkillGroup(0, LocalStringManager.Get(LanguageKey.LK_EquipCombatSkill_ScrollGroupTitle_0));
		GroupedCombatSkillScrollView2.CombatSkillGroup unequippedGroup = new GroupedCombatSkillScrollView2.CombatSkillGroup(1, LocalStringManager.Get(LanguageKey.LK_EquipCombatSkill_ScrollGroupTitle_1));
		foreach (CombatSkillDisplayData skill in skillList)
		{
			bool flag = this.IsSkillEquipped(skill.TemplateId);
			if (flag)
			{
				equippedGroup.SkillList.Add(skill);
			}
			else
			{
				unequippedGroup.SkillList.Add(skill);
			}
		}
		bool flag2 = equippedGroup.SkillList.Count > 0;
		if (flag2)
		{
			groups.Add(equippedGroup);
		}
		bool flag3 = unequippedGroup.SkillList.Count > 0;
		if (flag3)
		{
			groups.Add(unequippedGroup);
		}
		return groups;
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x000CBF8C File Offset: 0x000CA18C
	private void OnRenderCombatSkillGroupTitle(GroupedCombatSkillScrollView2.CombatSkillGroup group, Refers refers)
	{
		CImage icon = refers.CGet<CImage>("Icon");
		icon.SetSprite(string.Format("ui_charactermenu_23_icon_method_{0}", group.GroupId), false, null);
		CImage split = refers.CGet<CImage>("Split");
		split.SetSprite(string.Format("ui_sp_strip_{0}", (group.GroupId == 0) ? 3 : 1), false, null);
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x000CBFF4 File Offset: 0x000CA1F4
	private bool IsSkillEquipped(short skillId)
	{
		List<short> list = this.GetReuseSkillList("IsSkillEquipped");
		for (sbyte type = 0; type < 5; type += 1)
		{
			this._dataMonitor.EquippedCombatSkills.GetValidSkills(type, list);
			bool flag = list.Contains(skillId);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x000CC04C File Offset: 0x000CA24C
	private void RefreshEquippedCombatSkills()
	{
		this._equippedCombatSkillList.Clear();
		List<short> list = this.GetReuseSkillList("RefreshequippedCombatSkills");
		for (sbyte type = 0; type < 5; type += 1)
		{
			this._dataMonitor.EquippedCombatSkills.GetValidSkills(type, list);
			this._equippedCombatSkillList.AddRange(list);
		}
	}

	// Token: 0x06001D11 RID: 7441 RVA: 0x000CC0A8 File Offset: 0x000CA2A8
	private void RefreshDisabledCombatSkills()
	{
		this._disabledCombatSkillList.Clear();
		this._disabledCombatSkillList.AddRange(from d in this._typeFilteredCombatSkillList
		where !this.CanPutSkill(d.TemplateId)
		select d.TemplateId);
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x000CC10C File Offset: 0x000CA30C
	private void OnRenderCombatSkill(CombatSkillDisplayData skillData, Refers refers)
	{
		bool flag = skillData == null;
		if (!flag)
		{
			CommonCombatSkill commonCombatSkill = refers.CGet<CommonCombatSkill>("CommonCombatSkill");
			CButtonObsolete interact = refers.CGet<CButtonObsolete>("Interact");
			short skillTemplateId = skillData.TemplateId;
			bool equipped = this._equippedCombatSkillList.Contains(skillTemplateId);
			TooltipInvoker tip = interact.GetComponent<TooltipInvoker>();
			tip.RuntimeParam = commonCombatSkill.mouseTip.RuntimeParam;
			commonCombatSkill.toggle.isOn = false;
			bool canPut = this.CanPutSkill(skillTemplateId);
			bool interactable = equipped || canPut;
			commonCombatSkill.toggle.interactable = interactable;
			CButtonObsolete button = interact;
			bool flag2 = button != null;
			if (flag2)
			{
				button.interactable = interactable;
				button.ClearAndAddListener(delegate
				{
					bool flag5 = skillData != null;
					if (flag5)
					{
						CombatSkillItem skillConfig = CombatSkill.Instance[skillData.TemplateId];
						this._currentSelectedEquipType = skillConfig.EquipType;
						this.RefreshEquippedSkillHovers();
					}
					bool equipped2 = equipped;
					if (equipped2)
					{
						this.RemoveSkill(skillTemplateId);
					}
					else
					{
						this.AddSkill(skillTemplateId);
					}
				});
			}
			PointerTrigger pointerTrigger = interact.GetComponent<PointerTrigger>();
			bool flag3 = pointerTrigger != null;
			if (flag3)
			{
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.RemoveAllListeners();
				bool flag4 = interactable;
				if (flag4)
				{
					bool equipped3 = equipped;
					if (equipped3)
					{
						pointerTrigger.EnterEvent.AddListener(delegate()
						{
							commonCombatSkill.toggle.Hovering = true;
							this.GeneratePreviewAndShowForRemoveSkill(skillData.TemplateId);
						});
						pointerTrigger.ExitEvent.AddListener(new UnityAction(this.RefreshByExitPreview));
					}
					else
					{
						pointerTrigger.EnterEvent.AddListener(delegate()
						{
							commonCombatSkill.toggle.Hovering = false;
							this.GeneratePreviewAndShowForAddSkill(skillData.TemplateId);
						});
						pointerTrigger.ExitEvent.AddListener(new UnityAction(this.RefreshByExitPreview));
					}
				}
			}
		}
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x000CC2C8 File Offset: 0x000CA4C8
	public override void OnCurrentCharacterChange(int prevCharacterId)
	{
		this._isLearnedSkillsRequestSent = false;
		this.ClearLocalDisplayDataCache();
		this.UpdateCharacter();
		bool flag = this.CurTabIndex == 1;
		if (flag)
		{
			CharacterDomainMethod.Call.GetCurrentPlanIdAndPlanCount(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			bool isTaiwuTeammateButNotBeast = this.IsTaiwuTeammateButNotBeast;
			if (isTaiwuTeammateButNotBeast)
			{
				CharacterDomainMethod.Call.IsCombatSkillEquipmentLocked(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			}
			this.RefreshObjectActiveInTab1(true);
		}
		bool flag2 = this.CurTabIndex == 0;
		if (flag2)
		{
			bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (currentCharacterIsTaiwuTeammate)
			{
				CharacterDomainMethod.Call.IsNeiliAllocationLocked(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			}
			this.RefreshObjectActiveInTab0();
		}
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x000CC388 File Offset: 0x000CA588
	private void RefreshObjectActiveInTab1(bool isCharacterChange)
	{
		GameObject equipObject = this._equipSkillRefers.gameObject;
		GameObject invisiblePage = base.CGet<GameObject>("InvisiblePage");
		bool flag = this.CurTabIndex != 1;
		if (flag)
		{
			bool canShow = !this.IsThreeVitals();
			UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(equipObject, false);
			UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(invisiblePage, !canShow);
		}
		else
		{
			bool canShow2 = this.IsTaiwuTeamButNotBeast && !this.IsThreeVitals();
			bool flag2 = canShow2;
			if (flag2)
			{
				if (isCharacterChange)
				{
					this._waitEquipmentRefreshToHideInvisible = true;
				}
				else
				{
					UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(invisiblePage, false);
					UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(equipObject, true);
				}
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(invisiblePage, true);
				UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(equipObject, false);
			}
		}
		this._aiEquipLockToggleObj.SetActive(this.CurTabIndex == 1 && !this.CurrCharIsTaiwu);
		this._equipSkillRefers.CGet<RectTransform>("SetCombatSkillOrder").gameObject.SetActive(this.CurTabIndex == 1 && this.CurrCharIsTaiwu);
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x000CC488 File Offset: 0x000CA688
	private void ResolveWaitEquipmentRefreshToHideInvisible()
	{
		bool flag = !base.gameObject.activeInHierarchy || !this._waitEquipmentRefreshToHideInvisible || this._resolvingInvisiblePageVisibility;
		if (!flag)
		{
			this._resolvingInvisiblePageVisibility = true;
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				this._resolvingInvisiblePageVisibility = false;
				bool flag2 = this == null || !base.gameObject.activeInHierarchy || !this._waitEquipmentRefreshToHideInvisible;
				if (!flag2)
				{
					this._waitEquipmentRefreshToHideInvisible = false;
					this.ApplyEquipVisibilityAfterRefresh();
				}
			});
		}
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x000CC4DC File Offset: 0x000CA6DC
	private void ApplyEquipVisibilityAfterRefresh()
	{
		bool canShow = this.IsTaiwuTeamButNotBeast && !this.IsThreeVitals();
		Refers equipSkillRefers = this._equipSkillRefers;
		GameObject equipObject = (equipSkillRefers != null) ? equipSkillRefers.gameObject : null;
		GameObject invisiblePage = base.CGet<GameObject>("InvisiblePage");
		UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(equipObject, this.CurTabIndex == 1 && canShow);
		UI_CharacterMenuEquipCombatSkill.SetActiveIfNeeded(invisiblePage, this.CurTabIndex == 1 && !canShow);
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x000CC548 File Offset: 0x000CA748
	private static void SetActiveIfNeeded(GameObject target, bool shouldActive)
	{
		bool flag = target != null && target.activeSelf != shouldActive;
		if (flag)
		{
			target.SetActive(shouldActive);
		}
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x000CC57C File Offset: 0x000CA77C
	private bool IsThreeVitals()
	{
		List<int> yuanshanThreeVitalsIdList = this.BuildingModel.GetYuanshanThreeVitalsIdList();
		return yuanshanThreeVitalsIdList.Contains(base.CharacterMenu.CurCharacterId);
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x000CC5AC File Offset: 0x000CA7AC
	private void RefreshObjectActiveInTab0()
	{
		this._neiliAllocationTeammate.SetActive(this.CurTabIndex == 0 && base.CharacterMenu.CurrentCharacterIsTaiwuTeammate);
		this._neiliRefers.gameObject.SetActive(this.CurTabIndex == 0 && !this.IsThreeVitals());
		GameObject invisiblePage = base.CGet<GameObject>("InvisiblePage");
		bool flag = this.CurTabIndex == 0;
		if (flag)
		{
			invisiblePage.SetActive(this.IsThreeVitals());
		}
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x000CC62C File Offset: 0x000CA82C
	private void UpdateCharacter()
	{
		this._currNeili = -1;
		this._neiliAllocation.Items.FixedElementField = -1;
		this._baseNeiliAllocation.Items.FixedElementField = -1;
		this._genericGridAllocation[0] = byte.MaxValue;
		this._selectSkillArgBox.Set("CharId", base.CharacterMenu.CurCharacterId);
		this._neiliPageCombatSkill.gameObject.SetActive(this.CurrCharIsTaiwu);
		CharacterAttributeDataView equipViewAttributeView = this._equipViewAttributeView;
		if (equipViewAttributeView != null)
		{
			equipViewAttributeView.SetCurrentCharacterId(base.CharacterMenu.CurCharacterId);
		}
		this.UpdateNeiliAllocationHolderBeforeChangeCharacter();
		this.ClearDataMonitor();
		bool flag = base.CharacterMenu.CurCharacterId < 0;
		if (!flag)
		{
			this.SetupDataMonitor();
			this._combatNeiliAllocation.Initialize();
			bool flag2 = UIElement.Combat.Exist && UIElement.Combat.UiBaseAs<ViewCombat>().IsCharInCombat(base.CharacterMenu.CurCharacterId);
			if (flag2)
			{
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(8, 10, (ulong)base.CharacterMenu.CurCharacterId, new uint[]
				{
					3U
				}));
			}
		}
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x000CC74C File Offset: 0x000CA94C
	private void SetupDataMonitor()
	{
		this._dataMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(base.CharacterMenu.CurCharacterId, false);
		this._dataMonitor.AddConsummateLevelListener(new Action(this.OnConsummateLevelChange));
		this._dataMonitor.AddMaxNeiliListener(new Action(this.OnMaxNeiliChange));
		this._dataMonitor.AddCurrNeiliListener(new Action(this.OnCurrNeiliChange));
		this._dataMonitor.AddNeiliTypeListener(new Action(this.OnNeiliTypeChange));
		this._dataMonitor.AddNeiliTypeListener(new Action(this.UpdateNeiliPageCombatSkill));
		this._dataMonitor.AddNeiliProportionOfFiveElementsListener(new Action(this.UpdateNeiliOfFiveElements));
		this._dataMonitor.AddNeiliAllocationListener(new Action(this.OnNeiliAllocationChange));
		this._dataMonitor.AddAllocatedNeiliEffectsListener(new Action(this.OnAllocatedNeiliEffectsChange));
		this._dataMonitor.AddBaseNeiliAllocationListener(new Action(this.OnBaseNeiliAllocationChange));
		this._dataMonitor.AddLearnedCombatSkillsListener(new Action(this.OnLearnedCombatSkillChange));
		this._dataMonitor.AddEquippedCombatSkillsListener(new Action(this.OnEquippedCombatSkillChange));
		this._dataMonitor.AddBirthMonthListener(new Action(this.OnBirthMonthChange));
		bool init = this._dataMonitor.Init;
		if (init)
		{
			this._dataMonitor.OnDataInit();
		}
	}

	// Token: 0x06001D1C RID: 7452 RVA: 0x000CC8B4 File Offset: 0x000CAAB4
	private void ClearDataMonitor()
	{
		EquipCombatSkillMonitor dataMonitor = this._dataMonitor;
		if (dataMonitor != null)
		{
			dataMonitor.RemoveConsummateLevelListener(new Action(this.OnConsummateLevelChange));
		}
		EquipCombatSkillMonitor dataMonitor2 = this._dataMonitor;
		if (dataMonitor2 != null)
		{
			dataMonitor2.RemoveMaxNeiliListener(new Action(this.OnMaxNeiliChange));
		}
		EquipCombatSkillMonitor dataMonitor3 = this._dataMonitor;
		if (dataMonitor3 != null)
		{
			dataMonitor3.RemoveCurrNeiliListener(new Action(this.OnCurrNeiliChange));
		}
		EquipCombatSkillMonitor dataMonitor4 = this._dataMonitor;
		if (dataMonitor4 != null)
		{
			dataMonitor4.RemoveNeiliTypeListener(new Action(this.OnNeiliTypeChange));
		}
		EquipCombatSkillMonitor dataMonitor5 = this._dataMonitor;
		if (dataMonitor5 != null)
		{
			dataMonitor5.RemoveNeiliTypeListener(new Action(this.UpdateNeiliPageCombatSkill));
		}
		EquipCombatSkillMonitor dataMonitor6 = this._dataMonitor;
		if (dataMonitor6 != null)
		{
			dataMonitor6.RemoveNeiliProportionOfFiveElementsListener(new Action(this.UpdateNeiliOfFiveElements));
		}
		EquipCombatSkillMonitor dataMonitor7 = this._dataMonitor;
		if (dataMonitor7 != null)
		{
			dataMonitor7.RemoveNeiliAllocationListener(new Action(this.OnNeiliAllocationChange));
		}
		EquipCombatSkillMonitor dataMonitor8 = this._dataMonitor;
		if (dataMonitor8 != null)
		{
			dataMonitor8.RemoveAllocatedNeiliEffectsListener(new Action(this.OnAllocatedNeiliEffectsChange));
		}
		EquipCombatSkillMonitor dataMonitor9 = this._dataMonitor;
		if (dataMonitor9 != null)
		{
			dataMonitor9.RemoveBaseNeiliAllocationListener(new Action(this.OnBaseNeiliAllocationChange));
		}
		EquipCombatSkillMonitor dataMonitor10 = this._dataMonitor;
		if (dataMonitor10 != null)
		{
			dataMonitor10.RemoveLearnedCombatSkillsListener(new Action(this.OnLearnedCombatSkillChange));
		}
		EquipCombatSkillMonitor dataMonitor11 = this._dataMonitor;
		if (dataMonitor11 != null)
		{
			dataMonitor11.RemoveEquippedCombatSkillsListener(new Action(this.OnEquippedCombatSkillChange));
		}
		EquipCombatSkillMonitor dataMonitor12 = this._dataMonitor;
		if (dataMonitor12 != null)
		{
			dataMonitor12.RemoveBirthMonthListener(new Action(this.OnBirthMonthChange));
		}
		this._dataMonitor = null;
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x000CCA31 File Offset: 0x000CAC31
	private void OnConsummateLevelChange()
	{
		this.UpdateNeiliPageConsummate();
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x000CCA3C File Offset: 0x000CAC3C
	private void OnMaxNeiliChange()
	{
		bool flag = this._dataMonitor.ConsummateLevel >= 0 && this._currNeili >= 0 && this._neiliAllocation.Items.FixedElementField >= 0 && this._baseNeiliAllocation.Items.FixedElementField >= 0;
		if (flag)
		{
			this.UpdateNeiliAllocation();
		}
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x000CCA9C File Offset: 0x000CAC9C
	private void OnCurrNeiliChange()
	{
		this._currNeili = this._dataMonitor.CurrNeili;
		bool flag = this._dataMonitor.ConsummateLevel >= 0 && this._dataMonitor.MaxNeili >= 0 && this._neiliAllocation.Items.FixedElementField >= 0 && this._baseNeiliAllocation.Items.FixedElementField >= 0;
		if (flag)
		{
			this.UpdateNeiliAllocation();
		}
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x000CCB10 File Offset: 0x000CAD10
	private void OnNeiliAllocationChange()
	{
		this._neiliAllocation = this._dataMonitor.NeiliAllocation;
		bool flag = this._dataMonitor.ConsummateLevel >= 0 && this._dataMonitor.MaxNeili >= 0 && this._currNeili >= 0 && this._baseNeiliAllocation.Items.FixedElementField >= 0;
		if (flag)
		{
			this.UpdateNeiliAllocation();
		}
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x000CCB7C File Offset: 0x000CAD7C
	private void OnBaseNeiliAllocationChange()
	{
		this._baseNeiliAllocation = this._dataMonitor.BaseNeiliAllocation;
		bool flag = this._dataMonitor.ConsummateLevel >= 0 && this._dataMonitor.MaxNeili >= 0 && this._currNeili >= 0 && this._neiliAllocation.Items.FixedElementField >= 0;
		if (flag)
		{
			this.UpdateNeiliAllocation();
		}
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x000CCBE5 File Offset: 0x000CADE5
	private void OnAllocatedNeiliEffectsChange()
	{
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x000CCBE8 File Offset: 0x000CADE8
	private void RequestLearnedSkillDisplayData()
	{
		bool flag = this._dataMonitor == null || base.CharacterMenu.CurCharacterId != this._dataMonitor.Character.CharacterId;
		if (!flag)
		{
			this.RequestDisplayDataDirect(base.CharacterMenu.CurCharacterId, this._dataMonitor.LearnedCombatSkills);
		}
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x000CCC48 File Offset: 0x000CAE48
	private void RequestEquippedSkillDisplayData()
	{
		bool flag = this._dataMonitor == null || base.CharacterMenu.CurCharacterId != this._dataMonitor.Character.CharacterId;
		if (!flag)
		{
			List<short> equippedSkillIds = new List<short>();
			List<short> skillsPerType = this.GetReuseSkillList("RequestEquippedSkillDisplayData_PerType");
			for (sbyte type = 0; type < 5; type += 1)
			{
				this._dataMonitor.EquippedCombatSkills.GetValidSkills(type, skillsPerType);
				equippedSkillIds.AddRange(skillsPerType);
			}
			this.RequestDisplayDataDirect(base.CharacterMenu.CurCharacterId, equippedSkillIds);
		}
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x000CCCDD File Offset: 0x000CAEDD
	private void ForceRefreshEquippedSkillDisplayData(sbyte equipType)
	{
		this.ForceRefreshEquippedSkillDisplayData(equipType, equipType);
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x000CCCEC File Offset: 0x000CAEEC
	private void ForceRefreshEquippedSkillDisplayData(sbyte fromType, sbyte toType)
	{
		bool flag = this._dataMonitor == null || base.CharacterMenu.CurCharacterId != this._dataMonitor.Character.CharacterId;
		if (!flag)
		{
			int charId = base.CharacterMenu.CurCharacterId;
			List<short> skillsToRefresh = new List<short>();
			List<short> skillsPerType = this.GetReuseSkillList("ForceRefreshEquippedSkillDisplayData");
			for (sbyte type = fromType; type <= toType; type += 1)
			{
				this._dataMonitor.EquippedCombatSkills.GetValidSkills(type, skillsPerType);
				skillsToRefresh.AddRange(skillsPerType);
			}
			foreach (short skillId in skillsToRefresh)
			{
				this._localDisplayDataCache.Remove(new ValueTuple<int, short>(charId, skillId));
			}
			bool flag2 = skillsToRefresh.Count > 0;
			if (flag2)
			{
				CombatSkillDomainMethod.Call.GetCombatSkillDisplayData(this.Element.GameDataListenerId, charId, skillsToRefresh);
			}
		}
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x000CCDFC File Offset: 0x000CAFFC
	private void UpdateCurrentPlanDisplayData()
	{
		bool flag = this._updatedEquipPlan < 0;
		if (flag)
		{
			this._updatedEquipPlan = this._currEquipPlan;
		}
		else
		{
			this._updatedEquipPlan = this._currEquipPlan;
			this.RequestEquippedSkillDisplayData();
		}
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x000CCE3C File Offset: 0x000CB03C
	private void OnLearnedCombatSkillChange()
	{
		bool flag = this._dataMonitor == null;
		if (!flag)
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, new Action(this.RequireGridCounts));
		}
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x000CCE74 File Offset: 0x000CB074
	private void OnEquippedCombatSkillChange()
	{
		this.RefreshEquippedCombatSkills();
		this.ForceRefreshEquippedSkillDisplayData(0, 4);
		this.RequireGridCounts();
		bool needUpdateGenericGrid = this._needUpdateGenericGrid;
		if (needUpdateGenericGrid)
		{
			this._genericGridAllocation[0] = byte.MaxValue;
			this.RequireGridCounts();
			this._needUpdateGenericGrid = false;
		}
		bool isInEditingMode = this.IsInEditingMode;
		if (isInEditingMode)
		{
			this.RefreshCombatSkillScroll(false, false);
		}
		else
		{
			this._waitingRemoveSkillId = -1;
			this._waitingAddSkillId = -1;
		}
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x000CCEE8 File Offset: 0x000CB0E8
	private void RequireGridCounts()
	{
		CharacterDomainMethod.Call.GetCombatSkillExtraSlotCounts(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		CharacterDomainMethod.Call.GetCombatSkillSlotCounts(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
		CharacterDomainMethod.Call.GetGenericGridAllocation(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x000CCF4C File Offset: 0x000CB14C
	private void OnBirthMonthChange()
	{
		MonthItem monthConfig = Month.Instance[this._dataMonitor.BirthMonth];
		this.UpdateNeiliOnBirthMonthChange(monthConfig);
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x000CCF78 File Offset: 0x000CB178
	private void CacheBonusHolder(RectTransform bonusHolder, ref Dictionary<string, GameObject> cacheMap)
	{
		cacheMap.Clear();
		foreach (object obj in bonusHolder)
		{
			RectTransform child = (RectTransform)obj;
			foreach (object obj2 in child)
			{
				RectTransform elementObj = (RectTransform)obj2;
				bool activeSelf = elementObj.gameObject.activeSelf;
				if (activeSelf)
				{
					cacheMap.Add(elementObj.name, elementObj.gameObject);
				}
			}
		}
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x000CD040 File Offset: 0x000CB240
	private void OnClickSlotDeleteButton(sbyte type, int index)
	{
		bool flag = !base.CharacterMenu.CanOperate;
		if (!flag)
		{
			short skillId = this.GetSkillIdByTypeAndIndex(this._dataMonitor.EquippedCombatSkills, type, index);
			bool flag2 = skillId >= 0;
			if (flag2)
			{
				this.RemoveSkill(skillId);
			}
		}
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x000CD08C File Offset: 0x000CB28C
	private void OnClickSkillSlot(sbyte type, int index)
	{
		bool flag = !base.CharacterMenu.CanOperate;
		if (!flag)
		{
			bool isInEditingMode = this.IsInEditingMode;
			if (isInEditingMode)
			{
				this._currentSelectedEquipType = type;
				this.RefreshEquippedSkillHovers();
				short skillId = this.GetSkillIdByTypeAndIndex(this._dataMonitor.EquippedCombatSkills, type, index);
				bool flag2 = skillId >= 0;
				if (flag2)
				{
					this.RemoveSkill(skillId);
				}
			}
			else
			{
				this._combatSkillIdList = this._dataMonitor.LearnedCombatSkills;
				this.OpenCombatSkillScroll(!this.IsInEditingMode, type);
			}
		}
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x000CD117 File Offset: 0x000CB317
	private void SwitchCombatScrollViewFilterByEquipType(sbyte equipType, short skillId = -1)
	{
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x000CD11C File Offset: 0x000CB31C
	private short GetSkillIdByTypeAndIndex(CombatSkillEquipment equipment, sbyte type, int skillIndex)
	{
		bool flag = equipment == null;
		short result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			List<short> skills = this.GetReuseSkillList("GetSkillIdByTypeAndIndex");
			equipment.GetValidSkills(type, skills);
			bool flag2 = skillIndex < skills.Count;
			if (flag2)
			{
				result = skills[skillIndex];
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x000CD167 File Offset: 0x000CB367
	private void RefreshByExitPreview()
	{
		this.UpdateEquippedSkills(false);
		this.UpdateGenericGrid();
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x000CD17C File Offset: 0x000CB37C
	private void UpdateEquippedSkills(bool saveShowConfig)
	{
		if (saveShowConfig)
		{
			this._realDataEquippedSkillSlotConfig = this.GenerateCurrentEquippedSkillConfig();
			this.UpdateGenericGrid();
		}
		bool flag = this._realDataEquippedSkillSlotConfig != null;
		if (flag)
		{
			this.UpdateEquippedSkills(this._realDataEquippedSkillSlotConfig.Value);
			this.RefreshEquippedSkillHovers();
		}
		this.UpdateAutoLoadAllState();
		this.RefreshEditButton(this.IsInEditingMode);
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x000CD1E4 File Offset: 0x000CB3E4
	private void UpdateEquippedSkills(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig config)
	{
		bool isUpdatingEquippedSkills = this._isUpdatingEquippedSkills;
		if (!isUpdatingEquippedSkills)
		{
			this._isUpdatingEquippedSkills = true;
			try
			{
				this.UpdateEquippedSkillsInternal(config);
			}
			finally
			{
				this._isUpdatingEquippedSkills = false;
			}
		}
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x000CD22C File Offset: 0x000CB42C
	private void UpdateEquippedSkillsInternal(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig config)
	{
		for (sbyte type = 0; type < 5; type += 1)
		{
			bool flag = !this.IsInEditingMode;
			if (flag)
			{
				this.HideTargetSlotSelectedEffectByEquipType(type);
			}
			UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig lineConfig = config.Lines[(int)type];
			Refers slotTypeRefers = this._slotTypeHolder.GetChild((int)type).GetComponent<Refers>();
			RectTransform slotHolder = slotTypeRefers.CGet<RectTransform>("SlotHolder");
			EquipCombatSkillSlot template = slotTypeRefers.CGet<EquipCombatSkillSlot>("SlotTemplate");
			this.PrepareSlotToNumber(slotHolder, lineConfig.SlotConfigs.Count, template.gameObject, type);
			for (int index = 0; index < lineConfig.SlotConfigs.Count; index++)
			{
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slotConfig = lineConfig.SlotConfigs[index];
				this.UpdateSkillSlot(slotHolder, type, index, slotConfig.SkillData, slotConfig.CombatSkillId);
			}
			Refers root = this._slotTypeHolderTop.GetChild((int)type).GetComponent<Refers>().CGet<Refers>("ParticleArea");
			bool flag2 = lineConfig.ParticleConfig != null;
			if (flag2)
			{
				UI_CharacterMenuEquipCombatSkill.GridParticleHelper.RefreshPreviewParticle(root, lineConfig.ParticleConfig.Value.StartGridX, lineConfig.ParticleConfig.Value.GridWidth, lineConfig.ParticleConfig.Value.ParticleType);
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.GridParticleHelper.HidePreviewParticle(root);
			}
		}
		this.ResolveWaitEquipmentRefreshToHideInvisible();
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x000CD38C File Offset: 0x000CB58C
	private void RefreshEquippedSkillHovers()
	{
		for (sbyte type = 0; type < 5; type += 1)
		{
			bool showGrid = (this.IsInEditingMode || this.IsTweeningToEditingMode) && type == this._currentSelectedEquipType;
			Refers slotTypeRefers = this._slotTypeHolder.GetChild((int)type).GetComponent<Refers>();
			bool flag = showGrid;
			if (flag)
			{
				UI_CharacterMenuEquipCombatSkill.GridParticleHelper.RefreshGrid(slotTypeRefers, this.GetAvailableGridCount(type), 1f);
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.GridParticleHelper.HideGrid(slotTypeRefers);
			}
		}
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x000CD404 File Offset: 0x000CB604
	private bool CanPutSkill(short skillId)
	{
		bool flag = skillId < 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			sbyte type = CombatSkill.Instance[skillId].EquipType;
			CombatSkillDisplayData mySkillData;
			bool flag2 = !this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, skillId, out mySkillData);
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool revoked = mySkillData.Revoked;
				if (revoked)
				{
					result = false;
				}
				else
				{
					int usedCount = 0;
					List<short> skills = this.GetReuseSkillList("CanPutSkill");
					this._dataMonitor.EquippedCombatSkills.GetValidSkills(type, skills);
					foreach (short slotSkillId in skills)
					{
						CombatSkillDisplayData skillData;
						bool flag3 = this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, slotSkillId, out skillData);
						if (flag3)
						{
							usedCount += (int)((slotSkillId == skillId) ? 0 : skillData.GridCount);
						}
					}
					int availableSlotCount = this.GetAvailableGridCount(type);
					result = (availableSlotCount - usedCount >= (int)mySkillData.GridCount);
				}
			}
		}
		return result;
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x000CD520 File Offset: 0x000CB720
	private void UpdateSkillSlot(RectTransform slotHolder, sbyte equipType, int childIndex, CombatSkillDisplayData skillData, short skillTemplateId)
	{
		Transform slotTransform = slotHolder.GetChild(childIndex);
		EquipCombatSkillSlot slot = slotTransform.GetComponent<EquipCombatSkillSlot>();
		slot.UpdateSlot(skillData, skillTemplateId, base.CharacterMenu.CanOperate, this.IsInEditingMode, this.IsTaiwuTeamButNotBeast, this._neiliType, delegate
		{
			CommonUtils.ShowCombatSkillQuickEdit(this.CharacterMenu.CurCharacterId, skillData, slotTransform.GetComponent<RectTransform>());
		});
		bool flag = skillData != null;
		if (flag)
		{
			this.UpdateDisableInfo(slot, skillData, equipType, childIndex);
		}
	}

	// Token: 0x06001D38 RID: 7480 RVA: 0x000CD5B4 File Offset: 0x000CB7B4
	private void UpdateDisableInfo(EquipCombatSkillSlot slot, CombatSkillDisplayData skillData, sbyte type, int index)
	{
		slot.UpdateDisableInfo(skillData, type, base.CharacterMenu.CanOperate, this.IsInEditingMode, delegate
		{
			this.OnDisableButtonCloseClick((int)type, index);
		}, delegate
		{
			this.OnDisableJumpButtonCloseClick(skillData.TemplateId);
		}, delegate
		{
			this.GeneratePreviewAndShowForRemoveSkill(skillData.TemplateId);
		}, new Action(this.RefreshByExitPreview));
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x000CD63C File Offset: 0x000CB83C
	private void UpdateDisableInfoView(Refers disableRefers, CombatSkillDisplayData skillData, sbyte type)
	{
		bool flag = disableRefers == null;
		if (!flag)
		{
			bool disable = !skillData.CanAffect;
			disableRefers.gameObject.SetActive(disable);
			bool flag2 = type != -1;
			if (flag2)
			{
				CButtonObsolete button = disableRefers.GetComponent<CButtonObsolete>();
				bool flag3 = disable;
				if (flag3)
				{
					button.ClearAndAddListener(delegate
					{
						this.RemoveSkill(skillData.TemplateId);
						this._currentSelectedEquipType = type;
						this.RefreshEquippedSkillHovers();
					});
				}
				else
				{
					button.onClick.RemoveAllListeners();
				}
			}
			disableRefers.CGet<CButtonObsolete>("ButtonClose_Mini").gameObject.SetActive(false);
			disableRefers.CGet<CButtonObsolete>("JumpButton").gameObject.SetActive(false);
			TooltipInvoker tip = disableRefers.CGet<TooltipInvoker>("Tip");
			ArgumentBox argumentBox = tip.RuntimeParam ?? EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			bool conflicting = skillData.Conflicting;
			if (conflicting)
			{
				argumentBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation));
				argumentBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation_Conflicting));
			}
			else
			{
				argumentBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation));
				argumentBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation_Slot_NotEnough));
			}
			tip.RuntimeParam = argumentBox;
		}
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x000CD7A8 File Offset: 0x000CB9A8
	private void OnDisableButtonCloseClick(int type, int index)
	{
		short skillId = this.GetSkillIdByTypeAndIndex(this._dataMonitor.EquippedCombatSkills, (sbyte)type, index);
		bool flag = skillId >= 0;
		if (flag)
		{
			this.RemoveSkill(skillId);
		}
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x000CD7E0 File Offset: 0x000CB9E0
	private void OnDisableJumpButtonCloseClick(short templateId)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.Clear();
		args.Set("TemplateId", templateId);
		GEvent.OnEvent(UiEvents.EnterCharacterMenuPractice, args);
		ArgumentBox args2 = new ArgumentBox();
		args2.SetObject("TargetPageIndex", ECharacterSubToggleBase.EquipCombatSkillBase);
		GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args2);
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x000CD844 File Offset: 0x000CBA44
	private void UpdateGenericGrid()
	{
		bool flag = !this.IsTaiwuTeamButNotBeast || this._genericGridAllocation[0] == byte.MaxValue || this._dataMonitor.CharacterId != base.CharacterMenu.CurCharacterId;
		if (!flag)
		{
			this._realDataEquippedSkillSlotConfig = this.GenerateCurrentEquippedSkillConfig();
			bool flag2 = this._realDataEquippedSkillSlotConfig != null;
			if (flag2)
			{
				this.RefreshAvailableGenericGrid(this._realDataEquippedSkillSlotConfig.Value.GenericGridCountConfig);
				this.RefreshGridCount(this._realDataEquippedSkillSlotConfig.Value, false);
			}
		}
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x000CD8D4 File Offset: 0x000CBAD4
	private int CalcAvailableGenericGrid()
	{
		int availableGenericGrid = (int)this._totalGenericGrid;
		for (sbyte type = 1; type < 5; type += 1)
		{
			availableGenericGrid -= GameData.Domains.Character.CombatSkillHelper.GetGenericAllocationTotalCost(type, (int)this._genericGridAllocation[(int)(type - 1)]);
		}
		return availableGenericGrid;
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x000CD914 File Offset: 0x000CBB14
	private void RefreshAvailableGenericGrid(UI_CharacterMenuEquipCombatSkill.ShowGenericGridCountConfig config)
	{
		TextMeshProUGUI genericGridText = this._equipSkillRefers.CGet<TextMeshProUGUI>("GenericGrid");
		TextMeshProUGUI textMeshProUGUI = genericGridText;
		string str = config.LeftGenericGridCount.Value.ToString();
		UI_CharacterMenuEquipCombatSkill.NumberPreviewState state = config.LeftGenericGridCount.State;
		if (!true)
		{
		}
		string color;
		switch (state)
		{
		case UI_CharacterMenuEquipCombatSkill.NumberPreviewState.NotPreview:
			color = "pinkyellow";
			break;
		case UI_CharacterMenuEquipCombatSkill.NumberPreviewState.More:
			color = "brightblue";
			break;
		case UI_CharacterMenuEquipCombatSkill.NumberPreviewState.Less:
			color = "brightred";
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		textMeshProUGUI.text = str.SetColor(color);
		GameObject genericGridAddPreview = this._equipSkillRefers.CGet<GameObject>("GenericGridAddPreview");
		GameObject genericGridReducePreview = this._equipSkillRefers.CGet<GameObject>("GenericGridReducePreview");
		genericGridAddPreview.SetActive(config.LeftGenericGridCount.State == UI_CharacterMenuEquipCombatSkill.NumberPreviewState.More);
		genericGridReducePreview.SetActive(config.LeftGenericGridCount.State == UI_CharacterMenuEquipCombatSkill.NumberPreviewState.Less);
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x000CD9F0 File Offset: 0x000CBBF0
	private void RefreshGridCount(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig config, bool isPreview = false)
	{
		for (sbyte type = 0; type < 5; type += 1)
		{
			Refers slotTypeRefers = this._slotTypeHolder.GetChild((int)type).GetComponent<Refers>();
			TextMeshProUGUI label = slotTypeRefers.CGet<TextMeshProUGUI>("AvailableGridLabel");
			UI_CharacterMenuEquipCombatSkill.PreviewNumber availableSlotCount = config.Lines[(int)type].AvailableSlotCount;
			int totalGrid = availableSlotCount.Value;
			int usedGrid = config.Lines[(int)type].UsedSlotCount;
			string color = (usedGrid > totalGrid) ? "brightred" : "brightblue";
			UI_CharacterMenuEquipCombatSkill.NumberPreviewState state = availableSlotCount.State;
			if (!true)
			{
			}
			string text;
			switch (state)
			{
			case UI_CharacterMenuEquipCombatSkill.NumberPreviewState.NotPreview:
				text = "pinkyellow";
				break;
			case UI_CharacterMenuEquipCombatSkill.NumberPreviewState.More:
				text = "brightblue";
				break;
			case UI_CharacterMenuEquipCombatSkill.NumberPreviewState.Less:
				text = "brightred";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			string color2 = text;
			label.text = usedGrid.ToString().SetColor(color) + "/" + totalGrid.ToString().SetColor(color2);
			TextMeshProUGUI minusGridCount = slotTypeRefers.CGet<TextMeshProUGUI>("MinusGrid");
			minusGridCount.transform.parent.gameObject.SetActive(totalGrid < 0);
			bool flag = totalGrid < 0;
			if (flag)
			{
				minusGridCount.text = availableSlotCount.ToString();
			}
			bool flag2 = !isPreview;
			if (flag2)
			{
				this.RefreshAddReduceGridButton(slotTypeRefers, type, config.GenericGridCountConfig.LeftGenericGridCount.Value);
			}
		}
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x000CDB5B File Offset: 0x000CBD5B
	private void RefreshAddReduceGridButton(Refers slotTypeRefers, sbyte type, int availableGenericGrid)
	{
		this.RefreshAddGridButton(slotTypeRefers, type, availableGenericGrid);
		this.RefreshReduceGridButton(slotTypeRefers, type);
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x000CDB74 File Offset: 0x000CBD74
	private void RefreshAddGridButton(Refers slotTypeRefers, sbyte equipType, int availableGenericGrid)
	{
		CButtonObsolete addGrid = slotTypeRefers.CGet<CButtonObsolete>("AddGenericGrid");
		TooltipInvoker addTip = addGrid.GetComponent<TooltipInvoker>();
		addTip.enabled = true;
		bool flag = equipType == 0;
		if (flag)
		{
			addGrid.gameObject.SetActive(false);
		}
		else
		{
			addGrid.gameObject.SetActive(true);
			int genericAllocationNextCost = GameData.Domains.Character.CombatSkillHelper.GetGenericAllocationNextCost(equipType, (int)this._genericGridAllocation[(int)(equipType - 1)]);
			bool canAllocateNext = equipType > 0 && availableGenericGrid >= genericAllocationNextCost;
			int availableCountForAddGenericGrid = this.GetAvailableGridCountForAddGenericGrid(equipType);
			int availableSlotCount = this.GetAvailableGridCount(equipType);
			bool reachTempLimit = availableCountForAddGenericGrid >= (int)GameData.Domains.Character.CombatSkillHelper.MaxSlotCounts[(int)equipType] || availableSlotCount >= 99;
			bool canUseAdd = this.IsTaiwuTeamButNotBeast && base.CharacterMenu.CanOperate && !reachTempLimit && canAllocateNext;
			addGrid.interactable = canUseAdd;
			PointerTrigger addGridPointerTrigger = addGrid.GetComponent<PointerTrigger>();
			addGridPointerTrigger.EnterEvent.RemoveAllListeners();
			addGridPointerTrigger.ExitEvent.RemoveAllListeners();
			addGridPointerTrigger.enabled = canUseAdd;
			bool isInEditingMode = this.IsInEditingMode;
			if (isInEditingMode)
			{
				sbyte type = equipType;
				addGridPointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.GeneratePreviewAndShowForChangeGenericGridCount(type, true);
				});
				addGridPointerTrigger.ExitEvent.AddListener(new UnityAction(this.RefreshByExitPreview));
			}
			TooltipInvoker tooltipInvoker = addTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string addTipNumberStr = genericAllocationNextCost.ToString().SetColor(canUseAdd ? "pinkyellow" : "brightred");
			string addTipContent = ("<SpName=sp_23_logo_1><color=#brightblue>-" + addTipNumberStr + "</color>").ColorReplace();
			addTip.RuntimeParam.Set("arg0", addTipContent);
			addTip.Refresh(false, -1);
		}
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x000CDD30 File Offset: 0x000CBF30
	private void RefreshReduceGridButton(Refers slotTypeRefers, sbyte equipType)
	{
		CButtonObsolete reduceGrid = slotTypeRefers.CGet<CButtonObsolete>("ReduceGenericGrid");
		TooltipInvoker reduceTip = reduceGrid.GetComponent<TooltipInvoker>();
		reduceTip.enabled = true;
		bool flag = equipType == 0;
		if (flag)
		{
			reduceGrid.gameObject.SetActive(false);
		}
		else
		{
			reduceGrid.gameObject.SetActive(true);
			bool canUseReduce = this.IsTaiwuTeamButNotBeast && base.CharacterMenu.CanOperate && equipType > 0 && this._genericGridAllocation[(int)(equipType - 1)] > 0;
			reduceGrid.interactable = canUseReduce;
			PointerTrigger addGridPointerTrigger = reduceGrid.GetComponent<PointerTrigger>();
			addGridPointerTrigger.EnterEvent.RemoveAllListeners();
			addGridPointerTrigger.ExitEvent.RemoveAllListeners();
			addGridPointerTrigger.enabled = canUseReduce;
			bool isInEditingMode = this.IsInEditingMode;
			if (isInEditingMode)
			{
				sbyte type = equipType;
				addGridPointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.GeneratePreviewAndShowForChangeGenericGridCount(type, false);
				});
				addGridPointerTrigger.ExitEvent.AddListener(new UnityAction(this.RefreshByExitPreview));
			}
			TooltipInvoker tooltipInvoker = reduceTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			int reduceTipNumber = canUseReduce ? GameData.Domains.Character.CombatSkillHelper.GetGenericAllocationNextCost(equipType, (int)(this._genericGridAllocation[(int)(equipType - 1)] - 1)) : 0;
			string reduceTipContent = string.Format("<SpName=sp_23_logo_1><color=#brightblue>+{0}</color>", reduceTipNumber).ColorReplace();
			reduceTip.RuntimeParam.Set("arg0", reduceTipContent);
			reduceTip.Refresh(false, -1);
		}
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x000CDE9C File Offset: 0x000CC09C
	private void PrepareSlotToNumber(Transform slotHolder, int count, GameObject template, sbyte equipType)
	{
		int childSlotCount;
		for (childSlotCount = slotHolder.childCount - 2; childSlotCount < count; childSlotCount++)
		{
			GameObject slotObj = Object.Instantiate<GameObject>(template, slotHolder);
			slotObj.gameObject.SetActive(true);
			slotObj.transform.SetSiblingIndex(childSlotCount);
			this.InitEquipSkillSlot(slotObj.GetComponent<EquipCombatSkillSlot>(), equipType);
		}
		for (int i = 0; i < childSlotCount; i++)
		{
			slotHolder.GetChild(i).gameObject.SetActive(i < count);
		}
		for (int j = 0; j < slotHolder.childCount - 2; j++)
		{
			Transform slot = slotHolder.GetChild(j);
			this.SetupEquipSkillSlot(slot.GetComponent<EquipCombatSkillSlot>(), equipType, j);
		}
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x000CDF5C File Offset: 0x000CC15C
	private void OnEditEquipSkill(int skillTemplateId)
	{
		bool isTaiwuTeammateButNotBeast = this.IsTaiwuTeammateButNotBeast;
		if (isTaiwuTeammateButNotBeast)
		{
			this._aiEquipLockToggle.isOn = true;
		}
		this.UnselectTargetSlot();
		sbyte type = CombatSkill.Instance[skillTemplateId].EquipType;
		bool flag = type == 0;
		if (flag)
		{
			this.RequireGridCounts();
		}
		this.RequestEquippedSkillDisplayData();
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x000CDFB4 File Offset: 0x000CC1B4
	private void AddSkill(short skillTemplateId)
	{
		bool flag = this._waitingRemoveSkillId >= 0 || this._waitingAddSkillId >= 0;
		if (!flag)
		{
			this._waitingAddSkillId = skillTemplateId;
			CharacterDomainMethod.Call.AddEquippedCombatSkill(base.CharacterMenu.CurCharacterId, skillTemplateId);
			this.OnEditEquipSkill((int)skillTemplateId);
		}
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x000CE004 File Offset: 0x000CC204
	private void RemoveSkill(short skillTemplateId)
	{
		bool flag = this._waitingRemoveSkillId >= 0 || this._waitingAddSkillId >= 0;
		if (!flag)
		{
			this._waitingRemoveSkillId = skillTemplateId;
			CharacterDomainMethod.Call.RemoveEquippedCombatSkill(base.CharacterMenu.CurCharacterId, skillTemplateId);
			this.OnEditEquipSkill((int)skillTemplateId);
		}
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x000CE054 File Offset: 0x000CC254
	private void OnPlanTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool autoUpdatingPlanTog = this._autoUpdatingPlanTog;
		if (!autoUpdatingPlanTog)
		{
			bool flag = !this.IsInSkillOrderEditMode || !this.SkillOrderChanged();
			if (flag)
			{
				CharacterDomainMethod.Call.UpdateCombatSkillPlan(base.CharacterMenu.CurCharacterId, togNew.Key);
				this._needUpdateGenericGrid = true;
			}
			else
			{
				GameObject checkMarkNew = togNew.graphic.gameObject;
				checkMarkNew.SetActive(false);
				this._dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Equip_Skill_Order_Change_Confirm_Title).ColorReplace();
				this._dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Equip_Skill_Order_Change_Confirm_Tips).ColorReplace();
				this._dialogCmd.Yes = delegate()
				{
					ExtraDomainMethod.Call.SetCombatSkillOrderPlan(this._skillOrderPlans[this._currEquipPlan]);
					CharacterDomainMethod.Call.UpdateCombatSkillPlan(this.CharacterMenu.CurCharacterId, togNew.Key);
					this._needUpdateGenericGrid = true;
					checkMarkNew.SetActive(true);
				};
				this._dialogCmd.No = delegate()
				{
					this._autoUpdatingPlanTog = true;
					this._equipSkillRefers.CGet<CToggleGroupObsolete>("PlanHolder").Set(this._currEquipPlan, true, false);
					this._autoUpdatingPlanTog = false;
					checkMarkNew.SetActive(true);
				};
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._dialogCmd);
				UIElement.Dialog.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			this.RequestEquippedSkillDisplayData();
		}
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x000CE19C File Offset: 0x000CC39C
	private void ShowSkillOrderPanel()
	{
		this.InitSkillOrderList(true);
		this._setSkillOrderPanel.gameObject.SetActive(true);
		foreach (KeyValuePair<Transform, ValueTuple<int, Transform>> kv in this._skillOrderFocusDict)
		{
			kv.Key.SetParent(this._setSkillOrderPanel, true);
		}
		base.CharacterMenu.OnTryClosePage = delegate()
		{
			this.HideSkillOrderPanel(false);
		};
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x000CE234 File Offset: 0x000CC434
	private void HideSkillOrderPanel(bool save)
	{
		bool flag = this.SkillOrderChanged();
		if (flag)
		{
			this._dialogCmd.Title = LocalStringManager.Get(save ? LanguageKey.LK_Equip_Skill_Order_Change_Confirm_Title : LanguageKey.LK_Equip_Skill_Order_Change_Cancel_Title).ColorReplace();
			this._dialogCmd.Content = LocalStringManager.Get(save ? LanguageKey.LK_Equip_Skill_Order_Change_Confirm_Tips : LanguageKey.LK_Equip_Skill_Order_Change_Cancel_Tips).ColorReplace();
			this._dialogCmd.Yes = delegate()
			{
				bool save2 = save;
				if (save2)
				{
					ExtraDomainMethod.Call.SetCombatSkillOrderPlan(this._skillOrderPlans[this._currEquipPlan]);
				}
				else
				{
					List<short> skillOrderList = this._skillOrderPlans[this._currEquipPlan].Items;
					skillOrderList.Clear();
					skillOrderList.AddRange(this._skillOrderOriginPlan.Items);
				}
				this.HideSkillOrderPanel();
			};
			Action <>9__2;
			this._dialogCmd.No = delegate()
			{
				ViewCharacterMenu characterMenu = this.CharacterMenu;
				Action onTryClosePage;
				if ((onTryClosePage = <>9__2) == null)
				{
					onTryClosePage = (<>9__2 = delegate()
					{
						this.HideSkillOrderPanel(false);
					});
				}
				characterMenu.OnTryClosePage = onTryClosePage;
			};
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._dialogCmd);
			UIElement.Dialog.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			this.HideSkillOrderPanel();
		}
	}

	// Token: 0x06001D4A RID: 7498 RVA: 0x000CE324 File Offset: 0x000CC524
	private void HideSkillOrderPanel()
	{
		this._equipSkillRefers.CGet<CScrollRectLegacy>("SetOrderVerticalScrollView").ScrollTo(Vector2.zero, 0.3f);
		this._setSkillOrderPanel.gameObject.SetActive(false);
		foreach (KeyValuePair<Transform, ValueTuple<int, Transform>> kv in this._skillOrderFocusDict)
		{
			kv.Key.SetParent(kv.Value.Item2, true);
			kv.Key.SetSiblingIndex(kv.Value.Item1);
		}
		bool isInEditingMode = this.IsInEditingMode;
		if (isInEditingMode)
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				base.CharacterMenu.OnTryClosePage = delegate()
				{
					this.HideCombatSkillScroll();
					base.CharacterMenu.OnTryClosePage = null;
				};
			});
		}
		else
		{
			base.CharacterMenu.OnTryClosePage = null;
		}
	}

	// Token: 0x06001D4B RID: 7499 RVA: 0x000CE40C File Offset: 0x000CC60C
	private unsafe void InitSkillOrderList(bool syncOriginPlan = true)
	{
		bool flag = !this._skillOrderPlans.ContainsKey(this._currEquipPlan);
		if (flag)
		{
			GameData.Utilities.ShortList plan = GameData.Utilities.ShortList.Create();
			List<short> equippedSkillList = EasyPool.Get<List<short>>();
			equippedSkillList.Clear();
			for (sbyte type = 1; type < 4; type += 1)
			{
				ArraySegmentList<short> segment = *this._dataMonitor.EquippedCombatSkills[type];
				foreach (short ptr in segment)
				{
					short skillId = ptr;
					bool flag2 = skillId >= 0;
					if (flag2)
					{
						equippedSkillList.Add(skillId);
					}
				}
			}
			plan.Items.AddRange(equippedSkillList);
			while (plan.Items.Count < 54)
			{
				plan.Items.Add(-1);
			}
			EasyPool.Free<List<short>>(equippedSkillList);
			this._skillOrderPlans.Add(this._currEquipPlan, plan);
		}
		if (syncOriginPlan)
		{
			this._skillOrderOriginPlan.Items.Clear();
			this._skillOrderOriginPlan.Items.AddRange(this._skillOrderPlans[this._currEquipPlan].Items);
		}
		for (int i = 0; i < 54; i++)
		{
			this.UpdateSkillOrderSlot(i);
		}
		bool flag3 = this._selectedSkillOrderSlot != null;
		if (flag3)
		{
			this._selectedSkillOrderSlot.CGet<GameObject>("Highlight").SetActive(false);
			this._selectedSkillOrderSlot = null;
			this._equipSkillRefers.CGet<GameObject>("OpenCharMenuTips").SetActive(false);
		}
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x000CE5A4 File Offset: 0x000CC7A4
	private void UpdateSkillOrderSlot(int index)
	{
		short skillId = this._skillOrderPlans[this._currEquipPlan].Items[index];
		Refers refers = this._skillOrderSlotHolder.GetChild(index).GetComponent<Refers>();
		CombatSkillView skillView = refers.CGet<CombatSkillView>("CombatSkillView");
		skillView.gameObject.SetActive(skillId >= 0);
		refers.CGet<Refers>("Disable").gameObject.SetActive(false);
		bool flag = skillId >= 0;
		if (flag)
		{
			CombatSkillDisplayData skillData = this.GetDisplayData(base.CharacterMenu.CurCharacterId, skillId);
			bool flag2 = skillData != null;
			if (flag2)
			{
				skillView.SetData(skillData, false, false, true, false);
				this.UpdateDisableInfoView(refers.CGet<Refers>("Disable"), skillData, -1);
			}
		}
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x000CE668 File Offset: 0x000CC868
	private void OnClickSkillOrderSlot(int index)
	{
		Refers slotRefers = this._skillOrderSlotHolder.GetChild(index).GetComponent<Refers>();
		bool flag = this._selectedSkillOrderSlot == null;
		if (flag)
		{
			this._selectedSkillOrderSlot = slotRefers;
			this._selectedSkillOrderSlot.CGet<GameObject>("Highlight").SetActive(true);
			this._equipSkillRefers.CGet<GameObject>("OpenCharMenuTips").SetActive(true);
		}
		else
		{
			bool flag2 = this._selectedSkillOrderSlot != slotRefers;
			if (flag2)
			{
				List<short> skillOrderList = this._skillOrderPlans[this._currEquipPlan].Items;
				List<short> list = skillOrderList;
				int userInt = this._selectedSkillOrderSlot.UserInt;
				List<short> list2 = skillOrderList;
				int userInt2 = slotRefers.UserInt;
				short value = skillOrderList[slotRefers.UserInt];
				short value2 = skillOrderList[this._selectedSkillOrderSlot.UserInt];
				list[userInt] = value;
				list2[userInt2] = value2;
				this.UpdateSkillOrderSlot(this._selectedSkillOrderSlot.UserInt);
				this.UpdateSkillOrderSlot(slotRefers.UserInt);
			}
			this._selectedSkillOrderSlot.CGet<GameObject>("Highlight").SetActive(false);
			this._selectedSkillOrderSlot = null;
			this._equipSkillRefers.CGet<GameObject>("OpenCharMenuTips").SetActive(false);
		}
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x000CE7AC File Offset: 0x000CC9AC
	private unsafe void ResetSkillOrder()
	{
		List<short> skillOrderList = this._skillOrderPlans[this._currEquipPlan].Items;
		skillOrderList.Clear();
		for (sbyte type = 1; type < 4; type += 1)
		{
			ArraySegmentList<short> segment = *this._dataMonitor.EquippedCombatSkills[type];
			foreach (short ptr in segment)
			{
				short skillId = ptr;
				bool flag = skillId >= 0;
				if (flag)
				{
					skillOrderList.Add(skillId);
				}
			}
		}
		while (skillOrderList.Count < 54)
		{
			skillOrderList.Add(-1);
		}
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x000CE858 File Offset: 0x000CCA58
	private bool SkillOrderChanged()
	{
		List<short> skillOrderList = this._skillOrderPlans[this._currEquipPlan].Items;
		for (int i = 0; i < 54; i++)
		{
			bool flag = skillOrderList[i] != this._skillOrderOriginPlan.Items[i];
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x000CE8BC File Offset: 0x000CCABC
	private void UpdateAutoLoadAllState()
	{
		this._autoLoad.interactable = base.CharacterMenu.CanOperate;
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x000CE8D8 File Offset: 0x000CCAD8
	private bool IsEquippedCombatSkill()
	{
		for (sbyte i = 0; i < 5; i += 1)
		{
			List<short> skills = this.GetReuseSkillList("IsEquippedCombatSkill");
			this._dataMonitor.EquippedCombatSkills.GetValidSkills(i, skills);
			bool flag = skills.Any((short skillId) => skillId >= 0);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x000CE94D File Offset: 0x000CCB4D
	private void PlanAppend()
	{
		CharacterDomainMethod.Call.AppendCombatSkillPlan(base.CharacterMenu.CurCharacterId);
		this.RequireGridCounts();
	}

	// Token: 0x06001D53 RID: 7507 RVA: 0x000CE968 File Offset: 0x000CCB68
	private void PlanCopy()
	{
		CharacterDomainMethod.Call.DuplicateCurrentCombatSkillPlan(base.CharacterMenu.CurCharacterId);
		this.RequireGridCounts();
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x000CE983 File Offset: 0x000CCB83
	private void PlanClear()
	{
		this._aiEquipLockToggle.isOn = true;
		CharacterDomainMethod.Call.UnequipAllCombatSkills(base.CharacterMenu.CurCharacterId);
		this.RequireGridCounts();
	}

	// Token: 0x06001D55 RID: 7509 RVA: 0x000CE9AB File Offset: 0x000CCBAB
	private void PlanDelete()
	{
		CharacterDomainMethod.Call.DeleteCombatSkillPlan(base.CharacterMenu.CurCharacterId, this._currEquipPlan);
		this.RequireGridCounts();
		this.RequestEquippedSkillDisplayData();
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x000CE9D4 File Offset: 0x000CCBD4
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

	// Token: 0x06001D57 RID: 7511 RVA: 0x000CEA34 File Offset: 0x000CCC34
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

	// Token: 0x06001D58 RID: 7512 RVA: 0x000CEA64 File Offset: 0x000CCC64
	private void OnClickAutoLoad()
	{
		this.DoConfirmOnEquippedCombatSkill(new Action(this.AutoLoadCombatSkills), LanguageKey.LK_Equip_Skill_AutoLoad_Confirm, LanguageKey.LK_Equip_Skill_Autoload_Title);
	}

	// Token: 0x06001D59 RID: 7513 RVA: 0x000CEA84 File Offset: 0x000CCC84
	private void AutoLoadCombatSkills()
	{
		this._aiEquipLockToggle.isOn = false;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			CharacterDomainMethod.Call.AutoEquipCombatSkills(base.CharacterMenu.CurCharacterId);
			this.RequireGridCounts();
		});
	}

	// Token: 0x06001D5A RID: 7514 RVA: 0x000CEAAC File Offset: 0x000CCCAC
	private int GetAvailableGridCount(sbyte equipType)
	{
		return (int)(this._specificGridCount[(int)equipType] + (sbyte)((equipType > 0) ? this._genericGridAllocation[(int)(equipType - 1)] : 0));
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x000CEAD8 File Offset: 0x000CCCD8
	private int GetAvailableGridCountForAddGenericGrid(sbyte equipType)
	{
		return (int)(this._specificGridCount[(int)equipType] - this._extraSpecificGridCount[(int)equipType] + (sbyte)((equipType > 0) ? this._genericGridAllocation[(int)(equipType - 1)] : 0));
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x000CEB10 File Offset: 0x000CCD10
	private void OnGUI()
	{
		for (sbyte i = 0; i < 5; i += 1)
		{
			Refers slotTypeRefers = this._slotTypeHolder.GetChild((int)i).GetComponent<Refers>();
			UI_CharacterMenuEquipCombatSkill.GridParticleHelper.UpdatePosition(slotTypeRefers);
			RectTransform slotHolder = slotTypeRefers.CGet<RectTransform>("SlotHolder");
			RectTransform hoverArea = slotTypeRefers.CGet<RectTransform>("SlotHoverArea");
			hoverArea.anchoredPosition = new Vector2(slotHolder.anchoredPosition.x + 5f, slotHolder.anchoredPosition.y);
			RectTransform previewParticleArea = this._slotTypeHolderTop.GetChild((int)i).GetComponent<Refers>().CGet<Refers>("ParticleArea").GetComponent<RectTransform>();
			previewParticleArea.anchoredPosition = new Vector2(slotHolder.anchoredPosition.x + 5f, slotHolder.anchoredPosition.y);
		}
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x000CEBE0 File Offset: 0x000CCDE0
	private void Update()
	{
		bool flag = !this._equipSkillRefers.gameObject.activeInHierarchy;
		if (!flag)
		{
			bool flag2 = Time.realtimeSinceStartup - this._lastTickTime < 0.08f;
			if (!flag2)
			{
				this._lastTickTime = Time.realtimeSinceStartup;
				bool isFocus = UIManager.Instance.IsFocusElement(UIElement.CharacterMenu);
				this.TickGridHover(isFocus);
				for (sbyte i = 0; i < 5; i += 1)
				{
					Refers slotTypeRefers = this._slotTypeHolder.GetChild((int)i).GetComponent<Refers>();
					RectTransform slotHolder = slotTypeRefers.CGet<RectTransform>("SlotHolder");
					RectTransform viewport = slotHolder.parent.GetComponent<RectTransform>();
					Vector3[] slotCorners = new Vector3[4];
					slotHolder.GetWorldCorners(slotCorners);
					Vector3[] viewportCorners = new Vector3[4];
					viewport.GetWorldCorners(viewportCorners);
					bool showLeft = slotCorners[0].x < viewportCorners[0].x - 0.001f;
					this._scrollLeftParticleList[(int)i].SetActive(showLeft);
					bool showRight = slotCorners[2].x > viewportCorners[2].x + 0.001f;
					this._scrollRightParticleList[(int)i].SetActive(showRight);
				}
			}
		}
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x000CED28 File Offset: 0x000CCF28
	private void TickGridHover(bool isFocusCurrentUi)
	{
		bool canOperate = base.CharacterMenu.CanOperate && isFocusCurrentUi;
		UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem? mouseHoveringCheckItem = canOperate ? this._gridHoverHelper.Tick(this.IsInEditingMode, this.IsInSkillOrderEditMode) : null;
		bool flag = !canOperate;
		if (flag)
		{
			this._gridHoverHelper.HideAllGridHoverArea(this.IsInEditingMode);
		}
		bool flag2 = mouseHoveringCheckItem != null;
		if (flag2)
		{
			RectTransform targetRect = mouseHoveringCheckItem.Value.Rect;
			for (sbyte type = 0; type < 5; type += 1)
			{
				bool flag3 = type != mouseHoveringCheckItem.Value.EquipType;
				if (!flag3)
				{
					bool flag4 = !mouseHoveringCheckItem.Value.IsSkillSlot;
					if (!flag4)
					{
						EquipCombatSkillSlot slot = targetRect.GetComponent<EquipCombatSkillSlot>();
						if (slot != null)
						{
							slot.SetHoverAreaActive(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x000CEE0C File Offset: 0x000CD00C
	private void RefreshGenericGridTips()
	{
		TooltipInvoker genericGridBack = this._equipSkillRefers.CGet<TooltipInvoker>("GenericGridBack");
		UI_CharacterMenuEquipCombatSkill.GridTipsHelper.RefreshTipsSimple(genericGridBack);
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x000CEE34 File Offset: 0x000CD034
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		CRawImage glowImage = null;
		CRawImage generated;
		bool flag = this._generatedGlowImages.TryGetValue(languageType, out generated);
		if (flag)
		{
			glowImage = generated;
		}
		foreach (CRawImage generatedImage in this._generatedGlowImages.Values)
		{
			generatedImage.gameObject.SetActive(false);
		}
		bool flag2 = glowImage != null;
		if (flag2)
		{
			glowImage.gameObject.SetActive(true);
		}
		else
		{
			this.TryInitGlowLabels(languageType);
		}
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x000CEED8 File Offset: 0x000CD0D8
	private void OnNeiliTypeChange()
	{
		this.UpdateNeiliPageOnNeiliTypeChanged();
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x000CEEE4 File Offset: 0x000CD0E4
	private static UI_CharacterMenuEquipCombatSkill.NumberPreviewState CalcNumberPreview<T>(T preview, T old) where T : IComparable<T>
	{
		int compare = preview.CompareTo(old);
		bool flag = compare > 0;
		UI_CharacterMenuEquipCombatSkill.NumberPreviewState result;
		if (flag)
		{
			result = UI_CharacterMenuEquipCombatSkill.NumberPreviewState.More;
		}
		else
		{
			bool flag2 = compare < 0;
			if (flag2)
			{
				result = UI_CharacterMenuEquipCombatSkill.NumberPreviewState.Less;
			}
			else
			{
				result = UI_CharacterMenuEquipCombatSkill.NumberPreviewState.NotPreview;
			}
		}
		return result;
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x000CEF20 File Offset: 0x000CD120
	private UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? GenerateCurrentEquippedSkillConfig()
	{
		EquipCombatSkillMonitor dataMonitor = this._dataMonitor;
		bool flag = ((dataMonitor != null) ? dataMonitor.EquippedCombatSkills : null) == null;
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = this._specificGridCount[0] < 0;
			if (flag2)
			{
				result = null;
			}
			else
			{
				bool flag3 = this._totalGenericGrid < 0;
				if (flag3)
				{
					result = null;
				}
				else
				{
					bool flag4 = this._genericGridAllocation[0] == byte.MaxValue;
					if (flag4)
					{
						result = null;
					}
					else
					{
						List<short> skills = this.GetReuseSkillList("GenerateCurrentEquippedSkillConfig");
						List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig> lines = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig>();
						for (sbyte type = 0; type < 5; type += 1)
						{
							this._dataMonitor.EquippedCombatSkills.GetValidSkills(type, skills);
							int availableSlotCount = this.GetAvailableGridCount(type);
							List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> slots = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig>();
							int index = 0;
							foreach (short skillId in skills)
							{
								CombatSkillDisplayData skillData = this.GetDisplayData(base.CharacterMenu.CurCharacterId, skillId);
								bool flag5 = skillData == null;
								if (flag5)
								{
									return null;
								}
								int slotIndex = this.GetUiSlotIndex(this._dataMonitor.EquippedCombatSkills, type, skillId);
								bool flag6 = slotIndex == -1;
								if (flag6)
								{
									return null;
								}
								UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slotConfig = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
								{
									CombatSkillId = skillId,
									SkillData = skillData,
									Index = slotIndex
								};
								slots.Add(slotConfig);
								index += (int)slotConfig.SkillData.GridCount;
							}
							while (index < availableSlotCount)
							{
								slots.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
								{
									CombatSkillId = -1,
									SkillData = null,
									Index = index
								});
								index++;
							}
							UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig
							{
								SlotConfigs = slots,
								EquipType = type,
								AvailableSlotCount = new UI_CharacterMenuEquipCombatSkill.PreviewNumber(availableSlotCount, UI_CharacterMenuEquipCombatSkill.NumberPreviewState.NotPreview),
								UsedSlotCount = this.CountUsedGrids(type),
								ParticleConfig = null
							};
							lines.Add(line);
						}
						result = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig?(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig
						{
							Lines = lines,
							GenericGridCountConfig = new UI_CharacterMenuEquipCombatSkill.ShowGenericGridCountConfig
							{
								LeftGenericGridCount = new UI_CharacterMenuEquipCombatSkill.PreviewNumber(this.CalcAvailableGenericGrid(), UI_CharacterMenuEquipCombatSkill.NumberPreviewState.NotPreview)
							}
						});
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x000CF1D4 File Offset: 0x000CD3D4
	private UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? ModifyConfigByAddSkill(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, short skillId, sbyte equipType)
	{
		CombatSkillDisplayData newSkillData;
		bool flag = !this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, skillId, out newSkillData);
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = originConfig.Lines[(int)equipType].SlotConfigs.Any((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slot) => slot.CombatSkillId == skillId);
			if (flag2)
			{
				result = null;
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine = originConfig.Lines[(int)equipType];
				UI_CharacterMenuEquipCombatSkill.PreviewNumber availableCount = originConfigLine.AvailableSlotCount;
				int originUsedCount = originConfigLine.SlotConfigs.Sum((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slot) => (int)((slot.CombatSkillId >= 0) ? slot.SkillData.GridCount : 0));
				bool flag3 = originUsedCount >= availableCount.Value;
				if (flag3)
				{
					result = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig?(originConfig);
				}
				else
				{
					sbyte gridCount = newSkillData.GridCount;
					bool canAffect = originUsedCount + (int)gridCount <= availableCount.Value;
					CombatSkillDisplayData newSkillDataClone = new CombatSkillDisplayData(newSkillData);
					newSkillDataClone.CanAffect = canAffect;
					List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> newSlots = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig>();
					bool insertedNewSkillSlot = false;
					int usedSlot = 0;
					UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig? particleConfig = null;
					foreach (UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig originSlot in originConfigLine.SlotConfigs)
					{
						bool flag4 = originSlot.CombatSkillId >= 0;
						if (flag4)
						{
							newSlots.Add(originSlot);
							usedSlot += (int)originSlot.SkillData.GridCount;
						}
						else
						{
							bool flag5 = insertedNewSkillSlot;
							if (flag5)
							{
								newSlots.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
								{
									CombatSkillId = -1,
									SkillData = null,
									Index = usedSlot
								});
								usedSlot++;
							}
							else
							{
								newSlots.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
								{
									CombatSkillId = skillId,
									SkillData = newSkillDataClone,
									Index = usedSlot
								});
								insertedNewSkillSlot = true;
								particleConfig = new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig?(new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig
								{
									StartGridX = usedSlot,
									GridWidth = (int)newSkillDataClone.GridCount,
									ParticleType = UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddSkill
								});
								usedSlot += (int)newSkillDataClone.GridCount;
							}
						}
						bool flag6 = usedSlot >= availableCount.Value;
						if (flag6)
						{
							break;
						}
					}
					UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig
					{
						Lines = originConfig.Lines.Select((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line, int i) => (i == (int)equipType) ? new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig
						{
							EquipType = line.EquipType,
							AvailableSlotCount = line.AvailableSlotCount,
							UsedSlotCount = line.UsedSlotCount + (int)gridCount,
							SlotConfigs = newSlots,
							ParticleConfig = particleConfig
						} : line).ToList<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig>(),
						GenericGridCountConfig = originConfig.GenericGridCountConfig
					};
					bool flag7 = equipType == 0;
					if (flag7)
					{
						sbyte[] specificGridsChange = newSkillData.SpecificGrids;
						sbyte genericGridCountChange = newSkillData.GenericGrid;
						this.ModifyConfigWithNeigongAddOrUnMaster(ref newConfig, originConfigLine, originConfig, specificGridsChange, (int)genericGridCountChange);
					}
					result = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig?(newConfig);
				}
			}
		}
		return result;
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x000CF4F8 File Offset: 0x000CD6F8
	private UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? ModifyConfigByRemoveSkill(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, short skillId, sbyte equipType)
	{
		CombatSkillDisplayData newSkillData;
		bool flag = !this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, skillId, out newSkillData);
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = originConfig.Lines[(int)equipType].SlotConfigs.All((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slot) => slot.CombatSkillId != skillId);
			if (flag2)
			{
				result = null;
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine = originConfig.Lines[(int)equipType];
				int availableCount = originConfigLine.AvailableSlotCount.Value;
				int originUsedCount = originConfigLine.SlotConfigs.Sum((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slot) => (int)((slot.CombatSkillId >= 0) ? slot.SkillData.GridCount : 0));
				List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> newSlots = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig>();
				bool removedSkillSlot = false;
				int removedSlotGridCount = 0;
				int usedSlots = 0;
				UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig? particleConfig = null;
				foreach (UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig originSlot in originConfigLine.SlotConfigs)
				{
					bool flag3 = originSlot.CombatSkillId == skillId;
					if (flag3)
					{
						removedSkillSlot = true;
						newSlots.Add(originSlot);
						removedSlotGridCount = (int)originSlot.SkillData.GridCount;
						particleConfig = new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig?(new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig
						{
							StartGridX = originSlot.Index,
							GridWidth = (int)originSlot.SkillData.GridCount,
							ParticleType = UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.RemoveSkill
						});
						usedSlots += (int)originSlot.SkillData.GridCount;
					}
					else
					{
						bool flag4 = !removedSkillSlot;
						if (flag4)
						{
							newSlots.Add(originSlot);
							usedSlots += (int)originSlot.SkillData.GridCount;
						}
						else
						{
							bool flag5 = originSlot.SkillData != null;
							if (flag5)
							{
								bool newCanAffect = (int)originSlot.SkillData.GridCount + usedSlots - removedSlotGridCount <= availableCount;
								bool originCanAffect = (int)originSlot.SkillData.GridCount + usedSlots <= availableCount;
								bool flag6 = !originCanAffect && newCanAffect;
								if (flag6)
								{
									CombatSkillDisplayData newSkillDataClone = new CombatSkillDisplayData(originSlot.SkillData);
									newSkillDataClone.CanAffect = true;
									newSlots.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
									{
										CombatSkillId = originSlot.CombatSkillId,
										SkillData = newSkillDataClone,
										Index = originSlot.Index
									});
									usedSlots += (int)originSlot.SkillData.GridCount;
								}
								else
								{
									newSlots.Add(originSlot);
									usedSlots += (int)originSlot.SkillData.GridCount;
								}
							}
							else
							{
								newSlots.Add(originSlot);
								usedSlots++;
							}
						}
					}
				}
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig
				{
					Lines = originConfig.Lines.Select((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line, int i) => (i == (int)equipType) ? new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig
					{
						EquipType = line.EquipType,
						AvailableSlotCount = line.AvailableSlotCount,
						UsedSlotCount = line.UsedSlotCount - removedSlotGridCount,
						SlotConfigs = newSlots,
						ParticleConfig = particleConfig
					} : line).ToList<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig>(),
					GenericGridCountConfig = originConfig.GenericGridCountConfig
				};
				bool flag7 = equipType == 0;
				if (flag7)
				{
					sbyte[] specificGridsChange = new sbyte[this._specificGridCount.Length - 1];
					for (int j = 0; j < specificGridsChange.Length; j++)
					{
						specificGridsChange[j] = -newSkillData.SpecificGrids[j];
					}
					int genericGridCountChange = (int)(-(int)newSkillData.GenericGrid);
					this.ModifyConfigWithNeigongRemoveOrMaster(ref newConfig, originConfigLine, originConfig, newSkillData.TemplateId, specificGridsChange, genericGridCountChange);
				}
				result = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig?(newConfig);
			}
		}
		return result;
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x000CF8BC File Offset: 0x000CDABC
	private UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? ModifyConfigByActiveMasterSkill(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, short skillId, sbyte equipType, CombatSkillDisplayData previewDisplayData)
	{
		CombatSkillDisplayData originalSkillData;
		bool flag = !this.TryGetDisplayData(base.CharacterMenu.CurCharacterId, skillId, out originalSkillData);
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = originConfig.Lines[(int)equipType].SlotConfigs.All((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slot) => slot.CombatSkillId != skillId);
			if (flag2)
			{
				result = null;
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine = originConfig.Lines[(int)equipType];
				int availableCount = originConfigLine.AvailableSlotCount.Value;
				int originUsedCount = originConfigLine.SlotConfigs.Sum((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slot) => (int)((slot.CombatSkillId >= 0) ? slot.SkillData.GridCount : 0));
				List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> newSlots = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig>();
				bool masterSkillSlot = false;
				int usedSlots = 0;
				foreach (UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig originSlot in originConfigLine.SlotConfigs)
				{
					bool flag3 = originSlot.CombatSkillId == skillId;
					if (flag3)
					{
						bool newCanAffect = (int)previewDisplayData.GridCount + usedSlots <= availableCount;
						previewDisplayData.CanAffect = newCanAffect;
						UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig newSlot = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
						{
							CombatSkillId = originSlot.CombatSkillId,
							Index = usedSlots,
							SkillData = previewDisplayData
						};
						newSlots.Add(newSlot);
						usedSlots += (int)previewDisplayData.GridCount;
						masterSkillSlot = true;
					}
					else
					{
						bool flag4 = !masterSkillSlot;
						if (flag4)
						{
							newSlots.Add(originSlot);
							usedSlots += (int)originSlot.SkillData.GridCount;
						}
						else
						{
							bool flag5 = originSlot.SkillData != null;
							if (flag5)
							{
								bool newCanAffect2 = (int)originSlot.SkillData.GridCount + usedSlots <= availableCount;
								CombatSkillDisplayData newSkillDataClone = new CombatSkillDisplayData(originSlot.SkillData);
								newSkillDataClone.CanAffect = newCanAffect2;
								newSlots.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
								{
									CombatSkillId = originSlot.CombatSkillId,
									SkillData = newSkillDataClone,
									Index = usedSlots
								});
								usedSlots += (int)originSlot.SkillData.GridCount;
							}
							else
							{
								newSlots.Add(originSlot);
								usedSlots++;
							}
						}
					}
				}
				bool mastered = previewDisplayData.Mastered;
				if (mastered)
				{
					newSlots.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
					{
						CombatSkillId = -1,
						Index = usedSlots,
						SkillData = null
					});
				}
				else
				{
					List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> newSlots2 = newSlots;
					bool flag6 = newSlots2[newSlots2.Count - 1].SkillData == null;
					if (flag6)
					{
						newSlots.RemoveAt(newSlots.Count - 1);
					}
				}
				UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig
				{
					Lines = originConfig.Lines.Select((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line, int i) => (i == (int)equipType) ? new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig
					{
						EquipType = line.EquipType,
						AvailableSlotCount = line.AvailableSlotCount,
						UsedSlotCount = line.UsedSlotCount,
						SlotConfigs = newSlots
					} : line).ToList<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig>(),
					GenericGridCountConfig = originConfig.GenericGridCountConfig
				};
				bool flag7 = equipType == 0;
				if (flag7)
				{
					sbyte[] specificGrids = new sbyte[this._specificGridCount.Length - 1];
					for (int j = 0; j < specificGrids.Length; j++)
					{
						specificGrids[j] = previewDisplayData.SpecificGrids[j] - originalSkillData.SpecificGrids[j];
					}
					int genericGridCount = (int)(previewDisplayData.GenericGrid - originalSkillData.GenericGrid);
					bool mastered2 = previewDisplayData.Mastered;
					if (mastered2)
					{
						this.ModifyConfigWithNeigongRemoveOrMaster(ref newConfig, originConfigLine, originConfig, previewDisplayData.TemplateId, specificGrids, genericGridCount);
					}
					else
					{
						this.ModifyConfigWithNeigongAddOrUnMaster(ref newConfig, originConfigLine, originConfig, specificGrids, genericGridCount);
					}
				}
				result = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig?(newConfig);
			}
		}
		return result;
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x000CFCB8 File Offset: 0x000CDEB8
	private void ModifyConfigWithNeigongAddOrUnMaster(ref UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, sbyte[] specificGridsChange, int genericGridCountChange)
	{
		sbyte[] newSpecificGridCount = new sbyte[this._specificGridCount.Length];
		for (sbyte t = 0; t < 5; t += 1)
		{
			bool flag = t == 0;
			if (flag)
			{
				newSpecificGridCount[(int)t] = this._specificGridCount[(int)t];
			}
			else
			{
				int originSpecificGridCountWithoutExtra = (int)(this._specificGridCount[(int)t] - this._extraSpecificGridCount[(int)t]);
				int newSpecificGridCountWithoutExtra = originSpecificGridCountWithoutExtra + (int)specificGridsChange[(int)(t - 1)];
				bool flag2 = newSpecificGridCountWithoutExtra > (int)GameData.Domains.Character.CombatSkillHelper.MaxSlotCounts[(int)t];
				if (flag2)
				{
					newSpecificGridCountWithoutExtra = (int)GameData.Domains.Character.CombatSkillHelper.MaxSlotCounts[(int)t];
				}
				int specificGrid = newSpecificGridCountWithoutExtra + (int)this._extraSpecificGridCount[(int)t];
				newSpecificGridCount[(int)t] = (sbyte)Math.Max(specificGrid, 0);
			}
		}
		byte[] newGenericAllocation = UI_CharacterMenuEquipCombatSkill.GenerateNewGenericAllocationAndTotalGenericGridCount(this._genericGridAllocation, (int)this._totalGenericGrid + genericGridCountChange, newSpecificGridCount, this._extraSpecificGridCount);
		this.ModifyConfigWithNeigong(ref newConfig, originConfigLine, originConfig, newSpecificGridCount, genericGridCountChange);
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x000CFD7C File Offset: 0x000CDF7C
	private void ModifyConfigWithNeigongRemoveOrMaster(ref UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, short templateId, sbyte[] specificGridsChange, int genericGridCountChange)
	{
		sbyte[] newSpecificGridCount = new sbyte[this._specificGridCount.Length];
		for (sbyte t = 0; t < 5; t += 1)
		{
			bool flag = t == 0;
			if (flag)
			{
				newSpecificGridCount[(int)t] = this._specificGridCount[(int)t];
			}
			else
			{
				int otherNeigongProvidedSpecificGrid = 1;
				foreach (UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig slotConfig in originConfigLine.SlotConfigs)
				{
					bool flag2 = slotConfig.SkillData == null;
					if (!flag2)
					{
						bool flag3 = slotConfig.SkillData.TemplateId != templateId;
						if (flag3)
						{
							otherNeigongProvidedSpecificGrid += (int)slotConfig.SkillData.SpecificGrids[(int)(t - 1)];
						}
					}
				}
				sbyte max = GameData.Domains.Character.CombatSkillHelper.MaxSlotCounts[(int)t];
				int newValue = Math.Min(Math.Max((int)(this._specificGridCount[(int)t] + specificGridsChange[(int)(t - 1)]), otherNeigongProvidedSpecificGrid), (int)max);
				newSpecificGridCount[(int)t] = (sbyte)newValue;
			}
		}
		this.ModifyConfigWithNeigong(ref newConfig, originConfigLine, originConfig, newSpecificGridCount, genericGridCountChange);
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x000CFE8C File Offset: 0x000CE08C
	private void ModifyConfigWithNeigong(ref UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, sbyte[] newSpecificGridCount, int genericGridCountChange)
	{
		byte[] newGenericAllocation = UI_CharacterMenuEquipCombatSkill.GenerateNewGenericAllocationAndTotalGenericGridCount(this._genericGridAllocation, (int)this._totalGenericGrid + genericGridCountChange, newSpecificGridCount, this._extraSpecificGridCount);
		int[] newAvailableGridCount = new int[this._genericGridAllocation.Length + 1];
		int left = (int)this._totalGenericGrid + genericGridCountChange;
		newAvailableGridCount[0] = originConfigLine.AvailableSlotCount.Value;
		for (sbyte otherType = 1; otherType < 5; otherType += 1)
		{
			int value = (int)(newSpecificGridCount[(int)otherType] + (sbyte)newGenericAllocation[(int)(otherType - 1)]);
			newAvailableGridCount[(int)otherType] = value;
			left -= (int)newGenericAllocation[(int)(otherType - 1)];
		}
		newConfig.GenericGridCountConfig = new UI_CharacterMenuEquipCombatSkill.ShowGenericGridCountConfig
		{
			LeftGenericGridCount = new UI_CharacterMenuEquipCombatSkill.PreviewNumber(left, UI_CharacterMenuEquipCombatSkill.CalcNumberPreview<int>(left, originConfig.GenericGridCountConfig.LeftGenericGridCount.Value))
		};
		for (sbyte otherType2 = 1; otherType2 < 5; otherType2 += 1)
		{
			UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line = originConfig.Lines[(int)otherType2];
			int deltaGrid = newAvailableGridCount[(int)otherType2] - line.AvailableSlotCount.Value;
			bool flag = deltaGrid > 0;
			if (flag)
			{
				newConfig.Lines[(int)otherType2] = UI_CharacterMenuEquipCombatSkill.ModifyOtherLineByAddGrid(originConfig, otherType2, newAvailableGridCount[(int)otherType2]);
			}
			else
			{
				bool flag2 = deltaGrid < 0;
				if (flag2)
				{
					newConfig.Lines[(int)otherType2] = UI_CharacterMenuEquipCombatSkill.ModifyOtherLineByReduceGrid(originConfig, otherType2, newAvailableGridCount[(int)otherType2]);
				}
				else
				{
					newConfig.Lines[(int)otherType2] = originConfig.Lines[(int)otherType2];
				}
			}
		}
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x000CFFF0 File Offset: 0x000CE1F0
	private UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? ModifyConfigByChangeGenericGridCount(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, sbyte equipType, bool isAdd)
	{
		Tester.Assert(equipType != 0, "Wrong equipType");
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig originConfigLine = originConfig.Lines[(int)equipType];
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig newLineConfig;
		if (isAdd)
		{
			newLineConfig = UI_CharacterMenuEquipCombatSkill.ModifyOtherLineByAddGrid(originConfig, equipType, originConfigLine.AvailableSlotCount.Value + 1);
		}
		else
		{
			newLineConfig = UI_CharacterMenuEquipCombatSkill.ModifyOtherLineByReduceGrid(originConfig, equipType, originConfigLine.AvailableSlotCount.Value - 1);
		}
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig newConfig = new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig
		{
			Lines = originConfig.Lines.Select((UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line, int i) => (i == (int)equipType) ? newLineConfig : line).ToList<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig>(),
			GenericGridCountConfig = new UI_CharacterMenuEquipCombatSkill.ShowGenericGridCountConfig
			{
				LeftGenericGridCount = new UI_CharacterMenuEquipCombatSkill.PreviewNumber
				{
					Value = originConfig.GenericGridCountConfig.LeftGenericGridCount.Value + (isAdd ? -1 : 1),
					State = (isAdd ? UI_CharacterMenuEquipCombatSkill.NumberPreviewState.Less : UI_CharacterMenuEquipCombatSkill.NumberPreviewState.More)
				}
			}
		};
		return new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig?(newConfig);
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x000D0100 File Offset: 0x000CE300
	private static byte[] GenerateNewGenericAllocationAndTotalGenericGridCount(byte[] oldGenericAllocation, int oldTotal, sbyte[] specificGridCount, sbyte[] extraSpecificGridCount)
	{
		int total = oldTotal;
		byte[] newAllocation = new byte[oldGenericAllocation.Length];
		oldGenericAllocation.CopyTo(newAllocation, 0);
		for (int i = 0; i < oldGenericAllocation.Length; i++)
		{
			sbyte eType = (sbyte)(i + 1);
			byte allocatedGridCount = newAllocation[i];
			sbyte maxSlotCount = GameData.Domains.Character.CombatSkillHelper.MaxSlotCounts[(int)eType];
			byte allocatableGenericGridCount = (byte)Math.Max(0, (int)(maxSlotCount - specificGridCount[(int)eType]));
			bool flag = allocatedGridCount > allocatableGenericGridCount;
			if (flag)
			{
				allocatedGridCount = allocatableGenericGridCount;
				newAllocation[i] = allocatedGridCount;
			}
			int allocationCost = GameData.Domains.Character.CombatSkillHelper.GetGenericAllocationTotalCost(eType, (int)allocatedGridCount);
			bool flag2 = total >= allocationCost;
			if (flag2)
			{
				total -= allocationCost;
			}
			else
			{
				do
				{
					allocatedGridCount -= 1;
					allocationCost = GameData.Domains.Character.CombatSkillHelper.GetGenericAllocationTotalCost(eType, (int)allocatedGridCount);
				}
				while (total < allocationCost && allocatedGridCount > 0);
				newAllocation[i] = allocatedGridCount;
				total -= allocationCost;
			}
		}
		return newAllocation;
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x000D01D8 File Offset: 0x000CE3D8
	private static byte[] PreviewGenericAllocation(byte[] genericAllocation, int totalGenericGrid)
	{
		byte[] result = new byte[genericAllocation.Length];
		genericAllocation.CopyTo(result, 0);
		return result;
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x000D0200 File Offset: 0x000CE400
	private static UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig ModifyOtherLineByAddGrid(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, sbyte otherType, int newAvailableCount)
	{
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line = originConfig.Lines[(int)otherType];
		List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> newOtherLine = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig>();
		UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig otherLineParticleConfig = new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig
		{
			StartGridX = line.AvailableSlotCount.Value,
			GridWidth = newAvailableCount - line.AvailableSlotCount.Value,
			ParticleType = UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddGrid
		};
		int otherUsedSlot = 0;
		foreach (UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig originSlot in line.SlotConfigs)
		{
			bool flag = originSlot.CombatSkillId >= 0;
			if (flag)
			{
				bool newCanAffect = (int)originSlot.SkillData.GridCount + otherUsedSlot <= newAvailableCount;
				bool originCanAffect = (int)originSlot.SkillData.GridCount + otherUsedSlot <= line.AvailableSlotCount.Value;
				bool flag2 = !originCanAffect && newCanAffect;
				if (flag2)
				{
					CombatSkillDisplayData newSkillDataClone2 = new CombatSkillDisplayData(originSlot.SkillData);
					newSkillDataClone2.CanAffect = true;
					newOtherLine.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
					{
						CombatSkillId = originSlot.CombatSkillId,
						SkillData = newSkillDataClone2,
						Index = otherUsedSlot
					});
					otherUsedSlot += (int)originSlot.SkillData.GridCount;
				}
				else
				{
					newOtherLine.Add(originSlot);
					otherUsedSlot += (int)originSlot.SkillData.GridCount;
				}
			}
			else
			{
				bool flag3 = otherUsedSlot < newAvailableCount;
				if (flag3)
				{
					newOtherLine.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
					{
						CombatSkillId = -1,
						SkillData = null,
						Index = otherUsedSlot
					});
					otherUsedSlot++;
				}
			}
		}
		return new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig
		{
			AvailableSlotCount = new UI_CharacterMenuEquipCombatSkill.PreviewNumber(newAvailableCount, UI_CharacterMenuEquipCombatSkill.CalcNumberPreview<int>(newAvailableCount, line.AvailableSlotCount.Value)),
			EquipType = otherType,
			SlotConfigs = newOtherLine,
			ParticleConfig = new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig?(otherLineParticleConfig),
			UsedSlotCount = line.UsedSlotCount
		};
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x000D0418 File Offset: 0x000CE618
	private static UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig ModifyOtherLineByReduceGrid(UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig originConfig, sbyte otherType, int newAvailableCount)
	{
		UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line = originConfig.Lines[(int)otherType];
		List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> newOtherLine = new List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig>();
		UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig particleConfig = new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig
		{
			StartGridX = newAvailableCount,
			GridWidth = line.AvailableSlotCount.Value - newAvailableCount,
			ParticleType = UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.RemoveGrid
		};
		int otherUsedSlot = 0;
		foreach (UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig originSlot in line.SlotConfigs)
		{
			bool flag = originSlot.CombatSkillId >= 0;
			if (flag)
			{
				bool newCanAffect = (int)originSlot.SkillData.GridCount + otherUsedSlot <= newAvailableCount;
				bool originCanAffect = (int)originSlot.SkillData.GridCount + otherUsedSlot <= line.AvailableSlotCount.Value;
				bool flag2 = originCanAffect && !newCanAffect;
				if (flag2)
				{
					CombatSkillDisplayData newSkillDataClone2 = new CombatSkillDisplayData(originSlot.SkillData);
					newSkillDataClone2.CanAffect = false;
					newOtherLine.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
					{
						CombatSkillId = originSlot.CombatSkillId,
						SkillData = newSkillDataClone2,
						Index = otherUsedSlot
					});
					otherUsedSlot += (int)originSlot.SkillData.GridCount;
				}
				else
				{
					newOtherLine.Add(originSlot);
					otherUsedSlot += (int)originSlot.SkillData.GridCount;
				}
			}
			else
			{
				bool flag3 = otherUsedSlot < newAvailableCount;
				if (flag3)
				{
					newOtherLine.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
					{
						CombatSkillId = -1,
						SkillData = null,
						Index = otherUsedSlot
					});
					otherUsedSlot++;
				}
				else
				{
					newOtherLine.Add(new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig
					{
						CombatSkillId = -1,
						SkillData = null,
						Index = otherUsedSlot
					});
					otherUsedSlot++;
				}
			}
		}
		return new UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig
		{
			AvailableSlotCount = new UI_CharacterMenuEquipCombatSkill.PreviewNumber(newAvailableCount, UI_CharacterMenuEquipCombatSkill.CalcNumberPreview<int>(newAvailableCount, line.AvailableSlotCount.Value)),
			EquipType = otherType,
			SlotConfigs = newOtherLine,
			ParticleConfig = new UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig?(particleConfig),
			UsedSlotCount = line.UsedSlotCount
		};
	}

	// Token: 0x06001D6F RID: 7535 RVA: 0x000D065C File Offset: 0x000CE85C
	private void NeiliPageOnInit()
	{
		this._neiliRefers = base.CGet<Refers>("Neili");
		this._neiliRefers.CGet<RectTransform>("FiveElementsTypeHolder").gameObject.SetActive(false);
		this._neiliRefers.CGet<GameObject>("Mix").SetActive(false);
		this._neiliRefers.CGet<Transform>("BlueLine").gameObject.SetActive(false);
		this._neiliRefers.CGet<Transform>("RedLine").gameObject.SetActive(false);
		this._aiAllocationLockToggle = this._neiliRefers.CGet<CToggleObsolete>("AiAllocationLockToggle");
		this._aiAllocationLockToggle.onValueChanged.RemoveAllListeners();
		this._aiAllocationLockToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnAiAllocationLockToggle));
		this._neiliAllocationTeammate = this._neiliRefers.CGet<GameObject>("NeiliAllocationTeammate");
		this._neiliPageCombatSkill = this._neiliRefers.CGet<Refers>("CombatSkill");
	}

	// Token: 0x06001D70 RID: 7536 RVA: 0x000D0758 File Offset: 0x000CE958
	private void NeiliPageOnSubpageVisible()
	{
		this._neiliRefers.CGet<RectTransform>("FiveElementsTypeHolder").gameObject.SetActive(true);
		this._neiliRefers.CGet<GameObject>("Mix").SetActive(true);
		this._neiliRefers.CGet<Transform>("BlueLine").gameObject.SetActive(false);
		this._neiliRefers.CGet<Transform>("RedLine").gameObject.SetActive(false);
	}

	// Token: 0x06001D71 RID: 7537 RVA: 0x000D07D4 File Offset: 0x000CE9D4
	private unsafe void InitNeiliPageOnAwake()
	{
		this._neiliAllocationHolder = this._neiliRefers.CGet<RectTransform>("NeiliAllocation");
		this._fiveElementGlowLabelList = this._neiliRefers.CGetList<TextMeshProUGUI>("FiveElementGlowLabel_");
		this._glowLabelParent = this._neiliRefers.CGet<GameObject>("GlowLabelGroup");
		RectTransform consummateLevelHolder = this._neiliRefers.CGet<RectTransform>("ConsummateLevelHolder");
		RectTransform oddLevelHolder = this._neiliRefers.CGet<RectTransform>("OddLevelHolder");
		float consummateHeight = consummateLevelHolder.rect.height;
		float consummateInterval = consummateHeight / (float)consummateLevelHolder.childCount;
		bool flag = consummateLevelHolder.childCount > 1;
		if (flag)
		{
			for (int i = 0; i < consummateLevelHolder.childCount; i++)
			{
				RectTransform child = consummateLevelHolder.GetChild(i) as RectTransform;
				bool flag2 = child == null;
				if (!flag2)
				{
					float yPos = (float)(i + 1) * consummateInterval;
					child.anchoredPosition = new Vector2(child.anchoredPosition.x, yPos);
					Refers consummateRefers = child.GetComponent<Refers>();
					string labelText = LocalStringManager.Get(string.Format("LK_Consummate_Level_{0}", i));
					consummateRefers.CGet<TextMeshProUGUI>("NameLight").text = labelText;
					consummateRefers.CGet<TextMeshProUGUI>("NameGrey").text = labelText;
				}
			}
		}
		float oddHeight = oddLevelHolder.rect.height;
		float oddLevelInterval = oddHeight / (float)(oddLevelHolder.childCount - 1);
		bool flag3 = oddLevelHolder.childCount > 1;
		if (flag3)
		{
			for (int j = 0; j < oddLevelHolder.childCount; j++)
			{
				RectTransform child2 = oddLevelHolder.GetChild(j) as RectTransform;
				bool flag4 = child2 == null;
				if (!flag4)
				{
					float yPos2 = (float)j * oddLevelInterval + oddLevelInterval / 2f;
					child2.anchoredPosition = new Vector2(child2.anchoredPosition.x, yPos2);
				}
			}
		}
		for (int k = 0; k < consummateLevelHolder.childCount; k++)
		{
			Refers consummateRefers2 = consummateLevelHolder.GetChild(k).GetComponent<Refers>();
			TooltipInvoker mouseTip = consummateRefers2.CGet<TooltipInvoker>("Tips");
			int consummateLevel = (k + 1) * 2;
			ConsummateLevelItem config = ConsummateLevel.Instance[consummateLevel];
			mouseTip.PresetParam[0] = config.Name.ColorReplace();
			mouseTip.PresetParam[1] = string.Concat(new string[]
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Tips, consummateLevel).ColorReplace(),
				"\n",
				LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Neili_Tips, 20 * consummateLevel).ColorReplace(),
				"\n\n",
				config.Desc.ColorReplace()
			});
		}
		for (int l = 0; l < oddLevelHolder.childCount; l++)
		{
			int consummateLevel2 = l * 2 + 1;
			oddLevelHolder.GetChild(l).GetComponent<TooltipInvoker>().PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Tips, consummateLevel2).ColorReplace() + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Neili_Tips, 20 * consummateLevel2).ColorReplace();
		}
		for (byte m = 0; m < 4; m += 1)
		{
			byte type = m;
			Refers allocationRefers = this._neiliAllocationHolder.GetChild((int)m).GetComponent<Refers>();
			CButtonObsolete reduceBtn = allocationRefers.CGet<CButtonObsolete>("Reduce");
			CButtonObsolete addBtn = allocationRefers.CGet<CButtonObsolete>("Add");
			reduceBtn.ClearAndAddListener(delegate
			{
				ref short ptr = ref this._neiliAllocation[(int)type];
				ptr -= 1;
				ref short ptr2 = ref this._baseNeiliAllocation[(int)type];
				ptr2 -= 1;
				this._currNeili += CombatHelper.CalcNeiliCost(*this._baseNeiliAllocation[(int)type]);
				CharacterDomainMethod.Call.DeallocateNeili(this.CharacterMenu.CurCharacterId, type);
				bool currentCharacterIsTaiwuTeammate = this.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
				if (currentCharacterIsTaiwuTeammate)
				{
					this.SetAiAllocationLockToggleAndSave(true);
				}
				reduceBtn.gameObject.SetActive(*this._baseNeiliAllocation[(int)type] > 0);
				addBtn.gameObject.SetActive(true);
			});
			addBtn.ClearAndAddListener(delegate
			{
				this._currNeili -= CombatHelper.CalcNeiliCost(*this._baseNeiliAllocation[(int)type]);
				ref short ptr = ref this._neiliAllocation[(int)type];
				ptr += 1;
				ref short ptr2 = ref this._baseNeiliAllocation[(int)type];
				ptr2 += 1;
				CharacterDomainMethod.Call.AllocateNeili(this.CharacterMenu.CurCharacterId, type);
				bool currentCharacterIsTaiwuTeammate = this.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
				if (currentCharacterIsTaiwuTeammate)
				{
					this.SetAiAllocationLockToggleAndSave(true);
				}
				reduceBtn.gameObject.SetActive(true);
				addBtn.gameObject.SetActive(CombatHelper.CanAllocateNeili(type, this._baseNeiliAllocation, this._currNeili, this._dataMonitor.ConsummateLevel));
			});
			reduceBtn.GetComponent<TooltipInvoker>().enabled = false;
			addBtn.GetComponent<TooltipInvoker>().enabled = false;
		}
		this.TryInitGlowLabels(this.CurLanguageType);
	}

	// Token: 0x06001D72 RID: 7538 RVA: 0x000D0BD8 File Offset: 0x000CEDD8
	private void UpdateNeiliAllocationHolderBeforeChangeCharacter()
	{
		bool flag = this._neiliAllocationHolder != null;
		if (flag)
		{
			byte i = 0;
			while ((int)i < this._neiliAllocationHolder.childCount)
			{
				Refers allocationRefers = this._neiliAllocationHolder.GetChild((int)i).GetComponent<Refers>();
				bool locked = SingletonObject.getInstance<DisplayTriggerModel>().IsNeiliAllocationTypeRestricted(i) || !base.CharacterMenu.CanOperate;
				CButtonObsolete addButton = allocationRefers.CGet<CButtonObsolete>("Add");
				CButtonObsolete reduceButton = allocationRefers.CGet<CButtonObsolete>("Reduce");
				bool lockVisible = this.CurrCharIsTaiwu && locked;
				addButton.interactable = !lockVisible;
				reduceButton.interactable = !lockVisible;
				allocationRefers.CGet<GameObject>("Lock").SetActive(lockVisible);
				i += 1;
			}
		}
	}

	// Token: 0x06001D73 RID: 7539 RVA: 0x000D0CA4 File Offset: 0x000CEEA4
	private unsafe void UpdateNeiliAllocation()
	{
		this._neiliRefers.CGet<TextMeshProUGUI>("TotalNeili_1").text = this._currNeili.ToString();
		this._neiliRefers.CGet<TextMeshProUGUI>("TotalNeili_2").text = string.Format("/{0}", this._dataMonitor.MaxNeili);
		this._neiliRefers.CGet<TextMeshProUGUI>("TotalNeiliAllocation_1").text = this._baseNeiliAllocation.GetTotal().ToString();
		for (byte type = 0; type < 4; type += 1)
		{
			short allocateValue = (this._combatNeiliAllocation.GetTotal() > 0) ? (*(ref this._combatNeiliAllocation.Items.FixedElementField + (IntPtr)type * 2)) : (*(ref this._neiliAllocation.Items.FixedElementField + (IntPtr)type * 2));
			short baseValue = *(ref this._baseNeiliAllocation.Items.FixedElementField + (IntPtr)type * 2);
			Refers allocationRefers = this._neiliAllocationHolder.GetChild((int)type).GetComponent<Refers>();
			TextMeshProUGUI valueText = allocationRefers.CGet<TextMeshProUGUI>("Value");
			TextMeshProUGUI typeNameLabel = allocationRefers.CGet<TextMeshProUGUI>("TypeName");
			typeNameLabel.text = LocalStringManager.Get(string.Format("LK_Neili_Allocation_Type_{0}", type)).SetColor(CombatNeiliAllocation.NeiliAllocationFontColor[(int)type]);
			bool locked = SingletonObject.getInstance<DisplayTriggerModel>().IsNeiliAllocationTypeRestricted(type);
			int displayDelta = (int)(*(ref this._neiliAllocation.Items.FixedElementField + (IntPtr)type * 2) - *(ref this._baseNeiliAllocation.Items.FixedElementField + (IntPtr)type * 2));
			valueText.text = ((int)allocateValue - displayDelta).ToString().SetColor(CombatNeiliAllocation.NeiliAllocationFontColor[(int)type]);
			allocationRefers.CGet<TextMeshProUGUI>("ExtraValue").text = ((displayDelta > 0) ? string.Format("+{0}", displayDelta).SetColor("brightblue") : ((displayDelta < 0) ? displayDelta.ToString().SetColor("brightred") : ""));
			bool isTaiwuTeam = base.CharacterMenu.IsTaiwuTeam;
			allocationRefers.CGet<CButtonObsolete>("Reduce").gameObject.SetActive(isTaiwuTeam && base.CharacterMenu.CanOperate && !locked && baseValue > 0);
			allocationRefers.CGet<CButtonObsolete>("Add").gameObject.SetActive(isTaiwuTeam && base.CharacterMenu.CanOperate && !locked && CombatHelper.CanAllocateNeili(type, this._baseNeiliAllocation, this._currNeili, this._dataMonitor.ConsummateLevel));
		}
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x000D0F30 File Offset: 0x000CF130
	private void UpdateNeiliPageConsummate()
	{
		RectTransform consummateLevelHolder = this._neiliRefers.CGet<RectTransform>("ConsummateLevelHolder");
		RectTransform effectHolder = this._neiliRefers.CGet<RectTransform>("ConsummateEffectHolder1");
		int displayConsummate = Mathf.Min((int)this._dataMonitor.ConsummateLevel, 18);
		int consummateGroup = (displayConsummate > 1) ? ((displayConsummate - 1) / 2) : -1;
		bool particleShowOnLowLayer = consummateGroup == 4 || consummateGroup == 5;
		ResLoader.Load<Texture2D>(string.Format("{0}{1}", "RemakeResources/Textures/CharacterMenu_2/ui_charactermenu_16_bg_", Mathf.Max(consummateGroup, 0)), delegate(Texture2D texture)
		{
			this._neiliRefers.CGet<CRawImage>("ConsummateBack").texture = texture;
		}, null, false);
		this._neiliRefers.CGet<CImage>("ConsummateBar").fillAmount = (float)displayConsummate / 18f;
		this._neiliRefers.CGet<GameObject>("EffectMaskLow").SetActive(particleShowOnLowLayer);
		this._neiliRefers.CGet<GameObject>("EffectMaskHigh").SetActive(!particleShowOnLowLayer);
		for (int i = 0; i < 9; i++)
		{
			bool active = i <= consummateGroup;
			bool showParticle = i == Mathf.Max(consummateGroup, 0);
			Refers consummateRefers = consummateLevelHolder.GetChild(i).GetComponent<Refers>();
			ParticleSystem consummateParticle = effectHolder.GetChild(i).GetComponent<ParticleSystem>();
			consummateRefers.CGet<TextMeshProUGUI>("NameGrey").gameObject.SetActive(!active);
			consummateRefers.CGet<TextMeshProUGUI>("NameLight").gameObject.SetActive(active);
			consummateParticle.gameObject.SetActive(showParticle);
			this.ShowConsummateEffect(particleShowOnLowLayer, showParticle, i);
			bool flag = showParticle;
			if (flag)
			{
				SkeletonGraphic[] aniList = consummateParticle.GetComponentsInChildren<SkeletonGraphic>();
				foreach (SkeletonGraphic graphic in aniList)
				{
					TrackEntry trackEntry = graphic.AnimationState.SetAnimation(0, graphic.Skeleton.Data.Animations.Items[0].Name, true);
					trackEntry.MixDuration = 0f;
				}
			}
		}
		bool flag2 = this._dataMonitor.MaxNeili >= 0 && this._currNeili >= 0 && this._neiliAllocation.Items.FixedElementField >= 0 && this._baseNeiliAllocation.Items.FixedElementField >= 0;
		if (flag2)
		{
			this.UpdateNeiliAllocation();
		}
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x000D1174 File Offset: 0x000CF374
	private void ShowConsummateEffect(bool low, bool show, int index)
	{
		GameObject mask = this._neiliRefers.CGet<GameObject>(low ? "EffectMaskLow" : "EffectMaskHigh");
		UIParticle particle2 = mask.transform.GetChild(index).GetComponent<UIParticle>();
		particle2.gameObject.SetActive(show);
		if (show)
		{
			particle2.RefreshParticles();
			particle2.Play();
		}
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x000D11D4 File Offset: 0x000CF3D4
	private void UpdateNeiliPageOnNeiliTypeChanged()
	{
		Refers typeRefers = this._neiliRefers.CGet<Refers>("NeiliType");
		bool isPure = this._dataMonitor.NeiliType < 6;
		NeiliTypeItem typeConfig = NeiliType.Instance[this._dataMonitor.NeiliType];
		this.UpdateNeiliPureType(typeConfig);
		bool flag = isPure;
		if (flag)
		{
			typeRefers.gameObject.SetActive(false);
			this.HideLines();
		}
		else
		{
			typeRefers.gameObject.SetActive(true);
			bool isBuff = typeConfig.ColorType == 1;
			this.UpdateLine(typeConfig, isBuff);
			UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeIcon(typeRefers, typeConfig, isBuff);
			UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeTags(typeRefers, typeConfig, isBuff);
		}
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x000D1270 File Offset: 0x000CF470
	private void UpdateNeiliPureType(NeiliTypeItem typeConfig)
	{
		RectTransform fiveElementsTypeHolder = this._neiliRefers.CGet<RectTransform>("FiveElementsTypeHolder");
		for (sbyte type = 0; type < 5; type += 1)
		{
			bool isCurrentType = type == typeConfig.TemplateId;
			bool isBuff = typeConfig.ColorType == 1;
			Refers refers = UI_CharacterMenuEquipCombatSkill.GetFiveElementRefers(fiveElementsTypeHolder, type);
			CImage neiliTypePure = refers.CGet<CImage>("NeiliTypePure");
			Refers leftTypeTag = refers.CGet<Refers>("LeftTypeTag");
			Refers rightTypeTag = refers.CGet<Refers>("RightTypeTag");
			neiliTypePure.gameObject.SetActive(isCurrentType);
			bool flag = !isCurrentType;
			if (!flag)
			{
				neiliTypePure.SetSprite(isBuff ? "ui_charactermenu_16_light_yuanpan_1" : "ui_charactermenu_16_light_yuanpan_0", false, null);
				bool isLeft = neiliTypePure.GetComponent<RectTransform>().anchoredPosition.x < 0f;
				leftTypeTag.gameObject.SetActive(isLeft);
				rightTypeTag.gameObject.SetActive(!isLeft);
				Refers tagRefers = isLeft ? leftTypeTag : rightTypeTag;
				UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeTag(typeConfig, isBuff, tagRefers);
				UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeIconTips(typeConfig, neiliTypePure);
			}
		}
		bool flag2 = typeConfig.TemplateId == 5;
		if (flag2)
		{
			Refers refers2 = this._neiliRefers.CGet<Refers>("MixNeiliTypePure");
			refers2.gameObject.SetActive(true);
			Refers rightTypeTag2 = refers2.CGet<Refers>("RightTypeTag");
			bool isBuff2 = typeConfig.ColorType == 1;
			UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeTag(typeConfig, isBuff2, rightTypeTag2);
			UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeIconTips(typeConfig, refers2.GetComponent<CImage>());
		}
		else
		{
			this._neiliRefers.CGet<Refers>("MixNeiliTypePure").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x000D1404 File Offset: 0x000CF604
	private void UpdateNeiliPageCombatSkill()
	{
		bool flag = !this.CurrCharIsTaiwu;
		if (!flag)
		{
			CommonCombatSkill combatSkillView = this._neiliPageCombatSkill.CGet<CommonCombatSkill>("CommonCombatSkill");
			CombatSkillDisplayData skillData;
			bool flag2 = this.LoopingNeigong != -1 && this.CombatSkillModel.TryGet(base.CharacterMenu.CurCharacterId, this.LoopingNeigong, out skillData);
			if (flag2)
			{
				combatSkillView.Refresh(skillData);
				CombatSkillItem combatSkillConfig = CombatSkill.Instance[this.LoopingNeigong];
				bool showTranslate = true;
				bool flag3 = skillData.FiveElementDestTypeWhileLooping != -1;
				string elementIcon;
				string elementName;
				string elementIcon2;
				string elementName2;
				if (flag3)
				{
					sbyte destType = skillData.FiveElementDestTypeWhileLooping;
					sbyte transferType = skillData.FiveElementTransferTypeWhileLooping;
					if (!true)
					{
					}
					sbyte b;
					switch (transferType)
					{
					case 0:
						b = FiveElementsType.Countered[(int)destType];
						break;
					case 1:
						b = FiveElementsType.Countering[(int)destType];
						break;
					case 2:
						b = FiveElementsType.Produced[(int)destType];
						break;
					default:
						b = FiveElementsType.Producing[(int)destType];
						break;
					}
					if (!true)
					{
					}
					sbyte srcType = b;
					elementIcon = CommonUtils.GetFiveElementsIconByType(destType);
					elementName = CommonUtils.GetFiveElementsNameByType(destType);
					elementIcon2 = CommonUtils.GetFiveElementsIconByType(srcType);
					elementName2 = CommonUtils.GetFiveElementsNameByType(srcType);
				}
				else
				{
					sbyte type = combatSkillConfig.FiveElements;
					bool flag4 = type == 5;
					if (flag4)
					{
						int fiveElement = CommonUtils.GetFiveElementByNeiliType((int)this._dataMonitor.NeiliType);
						type = (sbyte)fiveElement;
						showTranslate = false;
					}
					elementIcon2 = CommonUtils.GetFiveElementsIconByType(type);
					elementName2 = CommonUtils.GetFiveElementsNameByType(type);
					elementIcon = elementIcon2;
					elementName = elementName2;
				}
				this._neiliPageCombatSkill.CGet<CImage>("ElementIcon_1").SetSprite(elementIcon2, false, null);
				this._neiliPageCombatSkill.CGet<TextMeshProUGUI>("ElementLabel_1").text = elementName2;
				this._neiliPageCombatSkill.CGet<CImage>("ElementIcon_2").SetSprite(elementIcon, false, null);
				this._neiliPageCombatSkill.CGet<TextMeshProUGUI>("ElementLabel_2").text = elementName;
				this._neiliPageCombatSkill.CGet<GameObject>("Empty").SetActive(false);
				this._neiliPageCombatSkill.CGet<GameObject>("Translate").SetActive(showTranslate);
				combatSkillView.gameObject.SetActive(true);
			}
			else
			{
				this._neiliPageCombatSkill.CGet<GameObject>("Empty").SetActive(true);
				this._neiliPageCombatSkill.CGet<GameObject>("Translate").SetActive(false);
				combatSkillView.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x000D1650 File Offset: 0x000CF850
	private void TryInitGlowLabels(LocalStringManager.LanguageType languageType)
	{
		bool generatingGlow = this._generatingGlow;
		if (!generatingGlow)
		{
			bool generated = this._generatedGlowImages.ContainsKey(languageType);
			bool flag = generated;
			if (!flag)
			{
				base.StartCoroutine(this.GenerateGlowCo(languageType));
			}
		}
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x000D168C File Offset: 0x000CF88C
	private IEnumerator GenerateGlowCo(LocalStringManager.LanguageType languageType)
	{
		this._glowLabelParent.SetActive(true);
		int num;
		for (int index = 0; index < this._fiveElementGlowLabelList.Count; index = num + 1)
		{
			TextMeshProUGUI label = this._fiveElementGlowLabelList[index];
			label.text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", index));
			label = null;
			num = index;
		}
		yield return null;
		GlowImageGenerator.GlowParameters glowParameters = new GlowImageGenerator.GlowParameters
		{
			iteration = 1,
			dilateIterations = 2,
			dilateStrength = 1f,
			glowAlpha = 0.8f
		};
		GlowImageGenerator.Instance.GenerateGameObjectGlow(this._glowLabelParent, new Color(0.99f, 0.77f, 0.44f), glowParameters, delegate(CRawImage rawImage)
		{
			this._generatingGlow = false;
			this._generatedGlowImages[languageType] = rawImage;
			this._glowLabelParent.SetActive(false);
		}, null);
		yield break;
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x000D16A4 File Offset: 0x000CF8A4
	private static void UpdateNeiliTypeTags(Refers typeRefers, NeiliTypeItem typeConfig, bool isBuff)
	{
		RectTransform typeTransform = typeRefers.GetComponent<RectTransform>();
		typeTransform.anchoredPosition = new Vector2((float)typeConfig.TypeIconPos[0], (float)typeConfig.TypeIconPos[1]);
		bool isLeft = typeTransform.anchoredPosition.x < 0f;
		GameObject leftTypeTag = typeRefers.CGet<GameObject>("LeftTypeTag");
		GameObject rightTypeTag = typeRefers.CGet<GameObject>("RightTypeTag");
		leftTypeTag.SetActive(isLeft);
		rightTypeTag.SetActive(!isLeft);
		Refers tagRefers = (isLeft ? leftTypeTag : rightTypeTag).GetComponent<Refers>();
		UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeTag(typeConfig, isBuff, tagRefers);
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x000D1730 File Offset: 0x000CF930
	private static void UpdateNeiliTypeTag(NeiliTypeItem typeConfig, bool isBuff, Refers tagRefers)
	{
		TextMeshProUGUI typeNameBuff = tagRefers.CGet<TextMeshProUGUI>("TypeNameBuff");
		TextMeshProUGUI typeNameDebuff = tagRefers.CGet<TextMeshProUGUI>("TypeNameDebuff");
		typeNameBuff.gameObject.SetActive(isBuff);
		typeNameDebuff.gameObject.SetActive(!isBuff);
		TMP_Text tmp_Text = isBuff ? typeNameBuff : typeNameDebuff;
		string name = typeConfig.Name;
		tmp_Text.text = name.Substring(3, name.Length - 3);
		CImage bg = tagRefers.CGet<CImage>("Bg");
		bg.SetSprite(isBuff ? "ui_charactermenu_16_gradientarrow_base_1" : "ui_charactermenu_16_gradientarrow_base_0", false, null);
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x000D17BC File Offset: 0x000CF9BC
	private static void UpdateNeiliTypeIcon(Refers typeRefers, NeiliTypeItem typeConfig, bool isBuff)
	{
		CImage icon = typeRefers.CGet<CImage>("Icon");
		icon.SetSprite(isBuff ? "ui_charactermenu_16_celestial_icon_1" : "ui_charactermenu_16_celestial_icon_0", false, null);
		UI_CharacterMenuEquipCombatSkill.UpdateNeiliTypeIconTips(typeConfig, icon);
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x000D17F8 File Offset: 0x000CF9F8
	private static void UpdateNeiliTypeIconTips(NeiliTypeItem typeConfig, CImage icon)
	{
		TooltipInvoker tipDisplayer = icon.GetComponent<TooltipInvoker>();
		tipDisplayer.PresetParam[0] = typeConfig.Name;
		tipDisplayer.PresetParam[1] = typeConfig.Desc.ColorReplace();
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x000D1830 File Offset: 0x000CFA30
	private void UpdateLine(NeiliTypeItem typeConfig, bool isBuff)
	{
		Transform blueLine = this._neiliRefers.CGet<Transform>("BlueLine");
		Transform redLine = this._neiliRefers.CGet<Transform>("RedLine");
		bool showLine = typeConfig.LinePos != null;
		bool ready = this.Element.Ready;
		if (ready)
		{
			blueLine.gameObject.SetActive(showLine && isBuff);
			redLine.gameObject.SetActive(showLine && !isBuff);
		}
		bool flag = showLine;
		if (flag)
		{
			Transform neiliLine = isBuff ? blueLine : redLine;
			neiliLine.localPosition = new Vector2((float)typeConfig.LinePos[0], (float)typeConfig.LinePos[1]);
			neiliLine.localRotation = Quaternion.Euler(0f, 0f, (float)typeConfig.LineAngle);
		}
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x000D18F4 File Offset: 0x000CFAF4
	private void HideLines()
	{
		this._neiliRefers.CGet<Transform>("BlueLine").gameObject.SetActive(false);
		this._neiliRefers.CGet<Transform>("RedLine").gameObject.SetActive(false);
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x000D1930 File Offset: 0x000CFB30
	private unsafe void UpdateNeiliOfFiveElements()
	{
		int total = 0;
		NeiliProportionOfFiveElements neiliFiveElements = this._dataMonitor.NeiliProportionOfFiveElements;
		RectTransform fiveElementsTypeHolder = this._neiliRefers.CGet<RectTransform>("FiveElementsTypeHolder");
		for (byte type = 0; type < 5; type += 1)
		{
			total += (int)(*neiliFiveElements[(int)type]);
		}
		for (sbyte type2 = 0; type2 < 5; type2 += 1)
		{
			int neiliValue = (int)(*neiliFiveElements[(int)type2]);
			int percent = (total > 0) ? (neiliValue * 100 / total) : 0;
			Refers refers = UI_CharacterMenuEquipCombatSkill.GetFiveElementRefers(fiveElementsTypeHolder, type2);
			refers.CGet<TextMeshProUGUI>("Value").text = string.Format("{0}%", percent);
		}
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x000D19E0 File Offset: 0x000CFBE0
	private void UpdateNeiliOnBirthMonthChange(MonthItem monthConfig)
	{
		RectTransform fiveElementsTypeHolder = this._neiliRefers.CGet<RectTransform>("FiveElementsTypeHolder");
		for (sbyte type = 0; type < 5; type += 1)
		{
			bool isInnate = type == monthConfig.FiveElementsType;
			Refers refers = UI_CharacterMenuEquipCombatSkill.GetFiveElementRefers(fiveElementsTypeHolder, type);
			GameObject innateIcon = refers.CGet<GameObject>("Innate");
			TooltipInvoker innateTips = innateIcon.GetComponent<TooltipInvoker>();
			innateIcon.SetActive(isInnate);
			innateTips.gameObject.SetActive(isInnate);
			TooltipInvoker tooltipInvoker = innateTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			innateTips.RuntimeParam.Set("BirthMonth", (int)this._dataMonitor.BirthMonth);
		}
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x000D1A94 File Offset: 0x000CFC94
	private static Refers GetFiveElementRefers(Transform fiveElementsTypeHolder, sbyte type)
	{
		return fiveElementsTypeHolder.GetChild((int)type).GetComponent<Refers>();
	}

	// Token: 0x0400166A RID: 5738
	private const int ExtraChildCountInSlotHolder = 2;

	// Token: 0x0400166B RID: 5739
	private const string ConsummateBackPath = "RemakeResources/Textures/CharacterMenu_2/ui_charactermenu_16_bg_";

	// Token: 0x0400166C RID: 5740
	private readonly int[] _equipTypeLogos = new int[]
	{
		0,
		2,
		3,
		4,
		5
	};

	// Token: 0x0400166D RID: 5741
	private static readonly Vector2 SlotSize = new Vector2(186f, 200f);

	// Token: 0x0400166F RID: 5743
	private const string EquipSkillLogTag = "EquipCombatSkill";

	// Token: 0x04001670 RID: 5744
	private EquipCombatSkillMonitor _dataMonitor;

	// Token: 0x04001671 RID: 5745
	private int _currEquipPlan;

	// Token: 0x04001672 RID: 5746
	private int _updatedEquipPlan;

	// Token: 0x04001673 RID: 5747
	private int _currNeili;

	// Token: 0x04001674 RID: 5748
	private NeiliAllocation _neiliAllocation;

	// Token: 0x04001675 RID: 5749
	private NeiliAllocation _baseNeiliAllocation;

	// Token: 0x04001676 RID: 5750
	private NeiliAllocation _combatNeiliAllocation;

	// Token: 0x04001677 RID: 5751
	private sbyte _totalGenericGrid = -1;

	// Token: 0x04001678 RID: 5752
	private readonly sbyte[] _specificGridCount = new sbyte[5];

	// Token: 0x04001679 RID: 5753
	private readonly sbyte[] _extraSpecificGridCount = new sbyte[5];

	// Token: 0x0400167A RID: 5754
	private byte[] _genericGridAllocation = new byte[]
	{
		byte.MaxValue,
		byte.MaxValue,
		byte.MaxValue,
		byte.MaxValue
	};

	// Token: 0x0400167B RID: 5755
	private bool _autoUpdatingPlanTog;

	// Token: 0x0400167C RID: 5756
	private bool _needUpdateGenericGrid;

	// Token: 0x0400167D RID: 5757
	private readonly ArgumentBox _selectSkillArgBox = new ArgumentBox();

	// Token: 0x0400167E RID: 5758
	private GameObject _neiliAllocationTeammate;

	// Token: 0x0400167F RID: 5759
	private Refers _equipSkillRefers;

	// Token: 0x04001680 RID: 5760
	private RectTransform _slotTypeHolder;

	// Token: 0x04001681 RID: 5761
	private RectTransform _slotTypeHolderTop;

	// Token: 0x04001682 RID: 5762
	private CToggleObsolete _aiEquipLockToggle;

	// Token: 0x04001683 RID: 5763
	private GameObject _aiEquipLockToggleObj;

	// Token: 0x04001684 RID: 5764
	private readonly GameData.Utilities.ShortList _skillOrderOriginPlan = GameData.Utilities.ShortList.Create();

	// Token: 0x04001685 RID: 5765
	private readonly Dictionary<int, GameData.Utilities.ShortList> _skillOrderPlans = new Dictionary<int, GameData.Utilities.ShortList>();

	// Token: 0x04001686 RID: 5766
	private int _unlockedCombatSkillPlanCount;

	// Token: 0x04001687 RID: 5767
	private RectTransform _setSkillOrderPanel;

	// Token: 0x04001688 RID: 5768
	private RectTransform _skillOrderSlotHolder;

	// Token: 0x04001689 RID: 5769
	[TupleElementNames(new string[]
	{
		"sibling",
		"parent"
	})]
	private readonly Dictionary<Transform, ValueTuple<int, Transform>> _skillOrderFocusDict = new Dictionary<Transform, ValueTuple<int, Transform>>();

	// Token: 0x0400168A RID: 5770
	private Refers _selectedSkillOrderSlot;

	// Token: 0x0400168B RID: 5771
	private readonly DialogCmd _dialogCmd = new DialogCmd
	{
		Type = 1
	};

	// Token: 0x0400168C RID: 5772
	private Refers _neiliPageCombatSkill;

	// Token: 0x0400168D RID: 5773
	private CButtonObsolete _autoLoad;

	// Token: 0x0400168E RID: 5774
	private LayoutManager _layoutManager;

	// Token: 0x0400168F RID: 5775
	private CToggleGroupObsolete _equipViewRightToggleGroup;

	// Token: 0x04001690 RID: 5776
	private GameObject _equipViewScrollArea;

	// Token: 0x04001691 RID: 5777
	private CharacterAttributeDataView _equipViewAttributeView;

	// Token: 0x04001692 RID: 5778
	private List<short> _combatSkillIdList = new List<short>();

	// Token: 0x04001693 RID: 5779
	private readonly List<CombatSkillDisplayData> _typeFilteredCombatSkillList = new List<CombatSkillDisplayData>();

	// Token: 0x04001694 RID: 5780
	private GroupedCombatSkillScrollView2 _combatSkillScrollView2;

	// Token: 0x04001695 RID: 5781
	private bool _combatSkillScrollInitialized;

	// Token: 0x04001696 RID: 5782
	private GameObject _maskToFocusCombatSkillScroll;

	// Token: 0x04001697 RID: 5783
	private UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? _realDataEquippedSkillSlotConfig;

	// Token: 0x04001698 RID: 5784
	private sbyte _currentSelectedEquipType = -1;

	// Token: 0x04001699 RID: 5785
	private readonly List<short> _equippedCombatSkillList = new List<short>();

	// Token: 0x0400169A RID: 5786
	private readonly List<short> _disabledCombatSkillList = new List<short>();

	// Token: 0x0400169B RID: 5787
	private bool _isLearnedSkillsRequestSent;

	// Token: 0x0400169C RID: 5788
	private bool _isUpdatingEquippedSkills;

	// Token: 0x0400169D RID: 5789
	[TupleElementNames(new string[]
	{
		"charId",
		"skillId"
	})]
	private readonly Dictionary<ValueTuple<int, short>, CombatSkillDisplayData> _localDisplayDataCache = new Dictionary<ValueTuple<int, short>, CombatSkillDisplayData>();

	// Token: 0x0400169E RID: 5790
	private int _cachedCharId = -1;

	// Token: 0x0400169F RID: 5791
	private List<short> _featureIds = new List<short>();

	// Token: 0x040016A0 RID: 5792
	private sbyte _neiliType = -1;

	// Token: 0x040016A1 RID: 5793
	private readonly UI_CharacterMenuEquipCombatSkill.GridHoverHelper _gridHoverHelper = new UI_CharacterMenuEquipCombatSkill.GridHoverHelper();

	// Token: 0x040016A2 RID: 5794
	private readonly List<GameObject> _scrollLeftParticleList = new List<GameObject>();

	// Token: 0x040016A3 RID: 5795
	private readonly List<GameObject> _scrollRightParticleList = new List<GameObject>();

	// Token: 0x040016A4 RID: 5796
	private bool _isWaitingGetGenericGridAllocation;

	// Token: 0x040016A5 RID: 5797
	private sbyte _pendingGenericGridChangeType = -1;

	// Token: 0x040016A6 RID: 5798
	private short _waitingAddSkillId = -1;

	// Token: 0x040016A7 RID: 5799
	private short _waitingRemoveSkillId = -1;

	// Token: 0x040016A8 RID: 5800
	private bool _isDisablingPreviewParticleByRemoving;

	// Token: 0x040016A9 RID: 5801
	private static readonly Dictionary<string, List<short>> ReuseSkillLists = new Dictionary<string, List<short>>();

	// Token: 0x040016AA RID: 5802
	private bool _waitEquipmentRefreshToHideInvisible;

	// Token: 0x040016AB RID: 5803
	private bool _resolvingInvisiblePageVisibility;

	// Token: 0x040016AC RID: 5804
	private float _lastTickTime;

	// Token: 0x040016AD RID: 5805
	private Refers _neiliRefers;

	// Token: 0x040016AE RID: 5806
	private CToggleObsolete _aiAllocationLockToggle;

	// Token: 0x040016AF RID: 5807
	private bool _doNotSaveAiAllocationLockToggle;

	// Token: 0x040016B0 RID: 5808
	private RectTransform _neiliAllocationHolder;

	// Token: 0x040016B1 RID: 5809
	private List<TextMeshProUGUI> _fiveElementGlowLabelList;

	// Token: 0x040016B2 RID: 5810
	private GameObject _glowLabelParent;

	// Token: 0x040016B3 RID: 5811
	private readonly Dictionary<LocalStringManager.LanguageType, CRawImage> _generatedGlowImages = new Dictionary<LocalStringManager.LanguageType, CRawImage>();

	// Token: 0x040016B4 RID: 5812
	private bool _generatingGlow = false;

	// Token: 0x020013DF RID: 5087
	private enum EEquipRightAreaToggle
	{
		// Token: 0x04009F38 RID: 40760
		CombatSkillScroll,
		// Token: 0x04009F39 RID: 40761
		AttributeView
	}

	// Token: 0x020013E0 RID: 5088
	private static class GridTipsHelper
	{
		// Token: 0x0600CA38 RID: 51768 RVA: 0x005909A8 File Offset: 0x0058EBA8
		public static void RefreshTipsSimple(TooltipInvoker tipDisplayer)
		{
			if (tipDisplayer.RuntimeParam == null)
			{
				tipDisplayer.RuntimeParam = new ArgumentBox();
			}
			ArgumentBox tipParam = tipDisplayer.RuntimeParam;
			tipDisplayer.Type = TipType.Simple;
			tipParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_EquipCombat_GenericGrid_Tips_Title));
			tipParam.Set("arg1", LocalStringManager.Get(LanguageKey.LK_EquipCombat_GenericGrid_Tips_Content_Simple));
		}
	}

	// Token: 0x020013E1 RID: 5089
	public static class GridParticleHelper
	{
		// Token: 0x0600CA39 RID: 51769 RVA: 0x00590A08 File Offset: 0x0058EC08
		private static int GetLineWidth(int gridCount)
		{
			return 186 * gridCount;
		}

		// Token: 0x0600CA3A RID: 51770 RVA: 0x00590A24 File Offset: 0x0058EC24
		public static void UpdatePosition(Refers slotTypeRefers)
		{
			RectTransform slotHolder = slotTypeRefers.CGet<RectTransform>("SlotHolder");
			Refers refers = slotTypeRefers.CGet<Refers>("GridArea");
			refers.GetComponent<RectTransform>().anchoredPosition = new Vector2(slotHolder.anchoredPosition.x + 5f, slotHolder.anchoredPosition.y);
		}

		// Token: 0x0600CA3B RID: 51771 RVA: 0x00590A78 File Offset: 0x0058EC78
		public static void RefreshGrid(Refers slotTypeRefers, int gridCount, float alpha = 1f)
		{
			Refers refers = slotTypeRefers.CGet<Refers>("GridArea");
			refers.GetComponent<CanvasGroup>().alpha = alpha;
			RectTransform downArrowRoot = refers.CGet<RectTransform>("DownArrowRoot");
			RectTransform downArrowTemplate = refers.CGet<RectTransform>("DownArrowTemplate");
			RectTransform downLine = refers.CGet<RectTransform>("DownLine");
			RectTransform upArrowRoot = refers.CGet<RectTransform>("UpArrowRoot");
			RectTransform upArrowTemplate = refers.CGet<RectTransform>("UpArrowTemplate");
			RectTransform upLine = refers.CGet<RectTransform>("UpLine");
			downLine.gameObject.SetActive(true);
			upLine.gameObject.SetActive(true);
			downLine.sizeDelta = new Vector2((float)UI_CharacterMenuEquipCombatSkill.GridParticleHelper.GetLineWidth(gridCount), downLine.rect.height);
			upLine.sizeDelta = new Vector2((float)UI_CharacterMenuEquipCombatSkill.GridParticleHelper.GetLineWidth(gridCount), upLine.rect.height);
			CommonUtils.PrepareEnoughChildren(downArrowRoot.transform, downArrowTemplate.gameObject, gridCount + 1, null);
			for (int i = 0; i < gridCount + 1; i++)
			{
				RectTransform arrow = downArrowRoot.GetChild(i).GetComponent<RectTransform>();
				arrow.gameObject.SetActive(true);
				arrow.GetComponent<CImage>().SetSprite((i == 0 || i == gridCount) ? "ui_charactermenu_23_img_scalebar_0" : "ui_charactermenu_23_img_scalebar_2", false, null);
				int scaleX = (i == gridCount) ? -1 : 1;
				arrow.localScale = new Vector3((float)scaleX, 1f, 1f);
				arrow.anchoredPosition = new Vector3((float)(186 * i), -6f, 0f);
			}
			CommonUtils.PrepareEnoughChildren(upArrowRoot.transform, upArrowTemplate.gameObject, gridCount + 1, null);
			for (int j = 0; j < gridCount + 1; j++)
			{
				RectTransform arrow2 = upArrowRoot.GetChild(j).GetComponent<RectTransform>();
				arrow2.gameObject.SetActive(true);
				arrow2.anchoredPosition = new Vector3((float)(186 * j), 9f, 0f);
			}
		}

		// Token: 0x0600CA3C RID: 51772 RVA: 0x00590C94 File Offset: 0x0058EE94
		public static void HideGrid(Refers slotTypeRefers)
		{
			Refers refers = slotTypeRefers.CGet<Refers>("GridArea");
			RectTransform downArrowRoot = refers.CGet<RectTransform>("DownArrowRoot");
			RectTransform upArrowRoot = refers.CGet<RectTransform>("UpArrowRoot");
			RectTransform downLine = refers.CGet<RectTransform>("DownLine");
			RectTransform upLine = refers.CGet<RectTransform>("UpLine");
			downLine.gameObject.SetActive(false);
			upLine.gameObject.SetActive(false);
			for (int i = 0; i < downArrowRoot.childCount; i++)
			{
				downArrowRoot.GetChild(i).gameObject.SetActive(false);
			}
			for (int j = 0; j < upArrowRoot.childCount; j++)
			{
				upArrowRoot.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x0600CA3D RID: 51773 RVA: 0x00590D5C File Offset: 0x0058EF5C
		public static void RefreshPreviewParticle(Refers refers, int gridIndex, int width, UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType particleType)
		{
			UI_CharacterMenuEquipCombatSkill.GridParticleHelper.<>c__DisplayClass5_0 CS$<>8__locals1;
			CS$<>8__locals1.particleType = particleType;
			GameObject targetParticle = refers.CGet<GameObject>(UI_CharacterMenuEquipCombatSkill.GridParticleHelper.<RefreshPreviewParticle>g__GetParticleListName|5_0(ref CS$<>8__locals1));
			targetParticle.SetActive(width > 0);
			Transform transform = targetParticle.transform;
			CImage cornerImage = transform.Find("CornerImage").GetComponent<CImage>();
			cornerImage.SetSprite(UI_CharacterMenuEquipCombatSkill.GridParticleHelper.<RefreshPreviewParticle>g__GetCornerImage|5_1(ref CS$<>8__locals1), false, null);
			RectTransform rect = targetParticle.GetComponent<RectTransform>();
			rect.anchoredPosition = new Vector2((float)(186 * gridIndex), 0f);
			rect.sizeDelta = new Vector2((float)(width * 186), rect.sizeDelta.y);
		}

		// Token: 0x0600CA3E RID: 51774 RVA: 0x00590DF8 File Offset: 0x0058EFF8
		public static void HidePreviewParticle(Refers refers)
		{
			GameObject blueParticle = refers.CGet<GameObject>("BlueParticle");
			blueParticle.SetActive(false);
			GameObject redParticle = refers.CGet<GameObject>("RedParticle");
			redParticle.SetActive(false);
		}

		// Token: 0x0600CA3F RID: 51775 RVA: 0x00590E30 File Offset: 0x0058F030
		[CompilerGenerated]
		internal static string <RefreshPreviewParticle>g__GetParticleListName|5_0(ref UI_CharacterMenuEquipCombatSkill.GridParticleHelper.<>c__DisplayClass5_0 A_0)
		{
			if (!true)
			{
			}
			string result;
			switch (A_0.particleType)
			{
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddSkill:
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddGrid:
				result = "BlueParticle";
				break;
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.RemoveSkill:
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.RemoveGrid:
				result = "RedParticle";
				break;
			default:
				throw new ArgumentOutOfRangeException("particleType", A_0.particleType, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600CA40 RID: 51776 RVA: 0x00590E94 File Offset: 0x0058F094
		[CompilerGenerated]
		internal static string <RefreshPreviewParticle>g__GetCornerImage|5_1(ref UI_CharacterMenuEquipCombatSkill.GridParticleHelper.<>c__DisplayClass5_0 A_0)
		{
			if (!true)
			{
			}
			string result;
			switch (A_0.particleType)
			{
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddSkill:
				result = "ui_charactermenu_23_icon_method_5";
				break;
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.RemoveSkill:
				result = "ui_charactermenu_23_icon_method_4";
				break;
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.AddGrid:
				result = "ui_charactermenu_23_icon_method_5";
				break;
			case UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType.RemoveGrid:
				result = "ui_charactermenu_23_icon_method_4";
				break;
			default:
				throw new ArgumentOutOfRangeException("particleType", A_0.particleType, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x020026B4 RID: 9908
		public enum ParticleType
		{
			// Token: 0x0400EB21 RID: 60193
			AddSkill,
			// Token: 0x0400EB22 RID: 60194
			RemoveSkill,
			// Token: 0x0400EB23 RID: 60195
			AddGrid,
			// Token: 0x0400EB24 RID: 60196
			RemoveGrid
		}
	}

	// Token: 0x020013E2 RID: 5090
	private class GridHoverHelper
	{
		// Token: 0x0600CA41 RID: 51777 RVA: 0x00590F08 File Offset: 0x0058F108
		public void RegisterCheck(RectTransform rect, sbyte equipType, bool isSkillSlot = true)
		{
			this._checkList.Add(new UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem
			{
				Rect = rect,
				EquipType = equipType,
				IsSkillSlot = isSkillSlot
			});
		}

		// Token: 0x0600CA42 RID: 51778 RVA: 0x00590F43 File Offset: 0x0058F143
		public void RegisterViewport(sbyte equipType, RectTransform viewport)
		{
			this._viewportDict[equipType] = viewport;
		}

		// Token: 0x0600CA43 RID: 51779 RVA: 0x00590F54 File Offset: 0x0058F154
		public UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem? Tick(bool isInEditingMode, bool isInSkillOrderEditMode)
		{
			UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem? result;
			if (isInEditingMode)
			{
				UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem? firstInItem = null;
				foreach (UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem checkItem in this._checkList)
				{
					bool flag = !checkItem.Rect.gameObject.activeSelf;
					if (!flag)
					{
						bool isIn = RectTransformUtility.RectangleContainsScreenPoint(checkItem.Rect, Input.mousePosition, UIManager.Instance.UiCamera);
						bool isSkillSlot = checkItem.IsSkillSlot;
						if (isSkillSlot)
						{
							this.OnMouseInOut(checkItem, isIn);
						}
						bool flag2 = isIn && firstInItem == null;
						if (flag2)
						{
							firstInItem = new UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem?(checkItem);
						}
					}
				}
				bool flag3 = firstInItem != null;
				if (flag3)
				{
					result = new UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem?(firstInItem.Value);
				}
				else
				{
					result = null;
				}
			}
			else if (isInSkillOrderEditMode)
			{
				result = null;
			}
			else
			{
				UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem? inCheckItem = null;
				foreach (UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem checkItem2 in this._checkList)
				{
					bool flag4 = !checkItem2.Rect.gameObject.gameObject.activeSelf;
					if (!flag4)
					{
						bool isIn2 = RectTransformUtility.RectangleContainsScreenPoint(checkItem2.Rect, Input.mousePosition, UIManager.Instance.UiCamera);
						bool flag5 = isIn2;
						if (flag5)
						{
							inCheckItem = new UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem?(checkItem2);
						}
						bool isSkillSlot2 = checkItem2.IsSkillSlot;
						if (isSkillSlot2)
						{
							this.OnMouseInOut(checkItem2, isIn2);
						}
					}
				}
				result = inCheckItem;
			}
			return result;
		}

		// Token: 0x0600CA44 RID: 51780 RVA: 0x00591138 File Offset: 0x0058F338
		private void OnMouseInOut(UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem checkItem, bool isIn)
		{
			RectTransform rect = checkItem.Rect;
			EquipCombatSkillSlot slot = rect.GetComponent<EquipCombatSkillSlot>();
			bool flag = slot == null;
			if (!flag)
			{
				slot.OnMouseInOut(isIn);
			}
		}

		// Token: 0x0600CA45 RID: 51781 RVA: 0x0059116C File Offset: 0x0058F36C
		public void HideAllGridHoverArea(bool isInEditingMode)
		{
			foreach (UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem checkItem in this._checkList)
			{
				bool flag = !checkItem.IsSkillSlot;
				if (!flag)
				{
					EquipCombatSkillSlot slot = checkItem.Rect.GetComponent<EquipCombatSkillSlot>();
					bool flag2 = slot == null;
					if (!flag2)
					{
						slot.OnHideAllGridHoverArea(isInEditingMode);
					}
				}
			}
		}

		// Token: 0x04009F3A RID: 40762
		private readonly List<UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem> _checkList = new List<UI_CharacterMenuEquipCombatSkill.GridHoverHelper.CheckItem>();

		// Token: 0x04009F3B RID: 40763
		private readonly Dictionary<sbyte, RectTransform> _viewportDict = new Dictionary<sbyte, RectTransform>();

		// Token: 0x020026B6 RID: 9910
		public struct CheckItem
		{
			// Token: 0x0400EB26 RID: 60198
			public RectTransform Rect;

			// Token: 0x0400EB27 RID: 60199
			public sbyte EquipType;

			// Token: 0x0400EB28 RID: 60200
			public bool IsSkillSlot;
		}
	}

	// Token: 0x020013E3 RID: 5091
	public enum NumberPreviewState
	{
		// Token: 0x04009F3D RID: 40765
		NotPreview,
		// Token: 0x04009F3E RID: 40766
		More,
		// Token: 0x04009F3F RID: 40767
		Less
	}

	// Token: 0x020013E4 RID: 5092
	public struct PreviewNumber
	{
		// Token: 0x0600CA47 RID: 51783 RVA: 0x0059120F File Offset: 0x0058F40F
		public PreviewNumber(int value, UI_CharacterMenuEquipCombatSkill.NumberPreviewState state = UI_CharacterMenuEquipCombatSkill.NumberPreviewState.NotPreview)
		{
			this.Value = value;
			this.State = state;
		}

		// Token: 0x04009F40 RID: 40768
		public int Value;

		// Token: 0x04009F41 RID: 40769
		public UI_CharacterMenuEquipCombatSkill.NumberPreviewState State;
	}

	// Token: 0x020013E5 RID: 5093
	public struct ShowEquippedSkillConfig
	{
		// Token: 0x04009F42 RID: 40770
		public List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig> Lines;

		// Token: 0x04009F43 RID: 40771
		public UI_CharacterMenuEquipCombatSkill.ShowGenericGridCountConfig GenericGridCountConfig;
	}

	// Token: 0x020013E6 RID: 5094
	public struct ShowGenericGridCountConfig
	{
		// Token: 0x04009F44 RID: 40772
		public UI_CharacterMenuEquipCombatSkill.PreviewNumber LeftGenericGridCount;
	}

	// Token: 0x020013E7 RID: 5095
	public struct PreviewParticleConfig
	{
		// Token: 0x04009F45 RID: 40773
		public int StartGridX;

		// Token: 0x04009F46 RID: 40774
		public int GridWidth;

		// Token: 0x04009F47 RID: 40775
		public UI_CharacterMenuEquipCombatSkill.GridParticleHelper.ParticleType ParticleType;
	}

	// Token: 0x020013E8 RID: 5096
	public struct ShowEquippedSkillLineConfig
	{
		// Token: 0x04009F48 RID: 40776
		public sbyte EquipType;

		// Token: 0x04009F49 RID: 40777
		public UI_CharacterMenuEquipCombatSkill.PreviewNumber AvailableSlotCount;

		// Token: 0x04009F4A RID: 40778
		public int UsedSlotCount;

		// Token: 0x04009F4B RID: 40779
		public List<UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillSlotConfig> SlotConfigs;

		// Token: 0x04009F4C RID: 40780
		public UI_CharacterMenuEquipCombatSkill.PreviewParticleConfig? ParticleConfig;
	}

	// Token: 0x020013E9 RID: 5097
	public struct ShowEquippedSkillSlotConfig
	{
		// Token: 0x04009F4D RID: 40781
		public short CombatSkillId;

		// Token: 0x04009F4E RID: 40782
		public int Index;

		// Token: 0x04009F4F RID: 40783
		public CombatSkillDisplayData SkillData;
	}
}
