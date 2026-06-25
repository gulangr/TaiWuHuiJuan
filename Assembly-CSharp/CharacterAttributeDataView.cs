using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using Game.Views.Combat;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UICommon;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class CharacterAttributeDataView : Refers
{
	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001C30 RID: 7216 RVA: 0x000C29F2 File Offset: 0x000C0BF2
	private bool IsTaiwu
	{
		get
		{
			return this._characterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001C31 RID: 7217 RVA: 0x000C2A06 File Offset: 0x000C0C06
	private bool IsTaiwuGearMate
	{
		get
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001C32 RID: 7218 RVA: 0x000C2A18 File Offset: 0x000C0C18
	private ViewCharacterMenu CharacterMenu
	{
		get
		{
			return UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
		}
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001C33 RID: 7219 RVA: 0x000C2A24 File Offset: 0x000C0C24
	private bool CharacterMenuCanNotOperate
	{
		get
		{
			return this.CharacterMenu.Element.IsShowing && !this.CharacterMenu.CanOperate;
		}
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x000C2A49 File Offset: 0x000C0C49
	private void Awake()
	{
		this.InitElements();
		this._originalPos = new Vector2?(base.GetComponent<RectTransform>().anchoredPosition);
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x000C2A6C File Offset: 0x000C0C6C
	private void OnDestroy()
	{
		bool flag = this._disorderOfQiController != null;
		if (flag)
		{
			this._disorderOfQiController.OnFillDisorderOfQi = null;
			this._disorderOfQiController.OnFillChangeOfDisorderOfQi = null;
		}
		bool flag2 = this._healthUIElement != null;
		if (flag2)
		{
			this._healthUIElement.OnFillHealthChange = null;
		}
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x000C2ABD File Offset: 0x000C0CBD
	public void InitElements()
	{
		this.Init();
		this.InitAttributePage();
		this.InitInjuryPage();
		this.InitHealthInfo();
		this.InitSamsaraPage();
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x000C2AE3 File Offset: 0x000C0CE3
	private void Start()
	{
		this.InitTabToggleGroup();
		base.CGet<CToggleGroupObsolete>("TabToggleGroup").Set((int)CharacterAttributeDataView.CurTabIndex, true, true);
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x000C2B05 File Offset: 0x000C0D05
	private void InitTabToggleGroup()
	{
		base.CGet<CToggleGroupObsolete>("TabToggleGroup").InitPreOnToggle(-1);
		base.CGet<CToggleGroupObsolete>("TabToggleGroup").OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnTabToggleGroupChange);
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x000C2B38 File Offset: 0x000C0D38
	private void OnEnable()
	{
		bool inBreakUI = this._inBreakUI;
		if (!inBreakUI)
		{
			this.SetEnable();
		}
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x000C2B5C File Offset: 0x000C0D5C
	public void SetEnable()
	{
		DOTweenAnimation[] doTweenAnimations = base.GetComponentsInChildren<DOTweenAnimation>(true);
		Array.ForEach<DOTweenAnimation>(doTweenAnimations, delegate(DOTweenAnimation e)
		{
			bool flag = e.tween == null;
			if (flag)
			{
				bool flag2 = e != this._bodyAnimation;
				if (flag2)
				{
					e.CreateTween(false, true);
				}
			}
		});
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		this.InitCombatDifficultyTips();
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x000C2BA4 File Offset: 0x000C0DA4
	private void OnDisable()
	{
		bool inBreakUI = this._inBreakUI;
		if (!inBreakUI)
		{
			this.SetDisable();
		}
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x000C2BC8 File Offset: 0x000C0DC8
	public void SetDisable()
	{
		this.LastPoisonValueArray = null;
		bool flag = this._poisonGroupController != null;
		if (flag)
		{
			this._poisonGroupController.OnPoisonChanged = null;
		}
		this.ReleaseCharacter();
		this.HideInfectNotice(true, true);
		this._storedTabIndex = -1;
		bool flag2 = this._listenerId != 0;
		if (flag2)
		{
			GameDataBridge.UnregisterListener(this._listenerId);
			this._listenerId = 0;
		}
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x000C2C34 File Offset: 0x000C0E34
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.Type == 1;
			if (flag)
			{
				bool flag2 = notification.DomainId == 8 && notification.MethodId == 2;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatCharDisplayData);
					bool flag3 = this._combatCharDisplayData == null;
					if (flag3)
					{
						this._poisonGroupController.IsCombatCharacter = false;
					}
					bool flag4 = this._samsaraInfoMonitor != null;
					if (flag4)
					{
						this._samsaraInfoMonitor.Character.CombatCharacter = this._combatCharDisplayData;
					}
					this.ForceRefreshPageData();
				}
				else
				{
					bool flag5 = notification.DomainId == 4 && notification.MethodId == 174;
					if (flag5)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._allBodyPartExists);
					}
					else
					{
						bool flag6 = notification.DomainId == 8 && notification.MethodId == 63;
						if (flag6)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._completeDamageStepDisplayData);
							this.ForceRefreshPageData();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x000C2DA4 File Offset: 0x000C0FA4
	public void SetCurrentCharacterId(int charId)
	{
		bool flag = -1 != charId;
		if (flag)
		{
			bool flag2 = UIElement.Combat.Exist && UIElement.Combat.UiBaseAs<ViewCombat>().IsCharInCombat(charId);
			if (flag2)
			{
				this._poisonGroupController.IsCombatCharacter = true;
				CombatDomainMethod.Call.GetCombatCharDisplayData(this._listenerId, charId);
			}
			else
			{
				this._poisonGroupController.IsCombatCharacter = false;
			}
			this._characterId = charId;
			this._majorAttributeController.CharacterId = charId;
			this._secondaryAttributeController.CharacterId = charId;
			EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
			if (eatingItemMonitor != null)
			{
				eatingItemMonitor.RemoveEatingItemListener(new Action(this.FillEatingItem));
			}
			this._eatingItemMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(charId, false);
			this._eatingItemMonitor.AddEatingItemListener(new Action(this.FillEatingItem));
			bool init = this._eatingItemMonitor.Init;
			if (init)
			{
				this.FillEatingItem();
			}
			this._preLifeTableController.ForEach(delegate(CommonCharacterToggle e)
			{
				e.CharacterId = -1;
			});
			bool flag3 = this._samsaraInfoMonitor != null;
			if (flag3)
			{
				this._samsaraInfoMonitor.Character.CombatCharacter = null;
				this._samsaraInfoMonitor.RemoveSamsaraListener(new Action(this.OnGetCharacterSamsara));
			}
			this._samsaraInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<SamsaraMonitor>(charId, false);
			this._samsaraInfoMonitor.AddSamsaraListener(new Action(this.OnGetCharacterSamsara));
			bool init2 = this._samsaraInfoMonitor.Init;
			if (init2)
			{
				this.OnGetCharacterSamsara();
			}
			bool flag4 = this.LastPoisonValueArray != null;
			if (flag4)
			{
				this._poisonGroupController.SetInitValue(this.LastPoisonValueArray);
			}
			this._poisonGroupController.CharacterId = charId;
			this._injuryGroupController.CharacterId = charId;
			this._disorderOfQiController.CharacterId = charId;
			this._healthUIElement.CharacterId = charId;
			this._healthUIElement.GearMateMode = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
			bool flag5 = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			if (flag5)
			{
				this.ForceRefreshPageData();
			}
			this.RefreshAllHealBtn(charId, false);
			Refers totalRefersRoot = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total");
			bool flag6 = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(charId);
			if (flag6)
			{
				bool healingOuterLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingOuterRestriction;
				totalRefersRoot.CGet<GameObject>("OuterLocked").SetActive(healingOuterLocked);
				totalRefersRoot.CGet<DisableStyleRoot>("TotalOuter").SetStyleEffect(healingOuterLocked, false);
				bool healingInnerLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingInnerRestriction;
				totalRefersRoot.CGet<GameObject>("InnerLocked").SetActive(healingInnerLocked);
				totalRefersRoot.CGet<DisableStyleRoot>("TotalInner").SetStyleEffect(healingInnerLocked, false);
			}
			else
			{
				totalRefersRoot.CGet<GameObject>("OuterLocked").SetActive(false);
				totalRefersRoot.CGet<GameObject>("InnerLocked").SetActive(false);
				totalRefersRoot.CGet<DisableStyleRoot>("TotalOuter").SetStyleEffect(false, false);
				totalRefersRoot.CGet<DisableStyleRoot>("TotalInner").SetStyleEffect(false, false);
			}
			TooltipInvoker totalOuterMouseTipDisplayer = totalRefersRoot.CGet<TooltipInvoker>("TotalOuterMouseTipDisplayer");
			TooltipInvoker tooltipInvoker = totalOuterMouseTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			totalOuterMouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Out_BodyInjury));
			totalOuterMouseTipDisplayer.RuntimeParam.Set("arg1", this.IsTaiwuGearMate ? (LocalStringManager.Get(LanguageKey.LK_BodyInjury_Tip_Content) + "\n\n" + LocalStringManager.Get(LanguageKey.LK_MouseTip_GearMateNoMonthlyHeal)) : LocalStringManager.Get(LanguageKey.LK_BodyInjury_Tip_Content));
			TooltipInvoker totalInnerMouseTipDisplayer = totalRefersRoot.CGet<TooltipInvoker>("TotalInnerMouseTipDisplayer");
			tooltipInvoker = totalInnerMouseTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			totalInnerMouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Inner_BodyInjury));
			totalInnerMouseTipDisplayer.RuntimeParam.Set("arg1", this.IsTaiwuGearMate ? (LocalStringManager.Get(LanguageKey.LK_BodyInjury_Tip_Content) + "\n\n" + LocalStringManager.Get(LanguageKey.LK_MouseTip_GearMateNoMonthlyHeal)) : LocalStringManager.Get(LanguageKey.LK_BodyInjury_Tip_Content));
			this.InitCombatDifficultyTips();
			CharacterDomainMethod.Call.GetCharacterAllBodyPartExists(this._listenerId, charId);
			CombatDomainMethod.Call.GetCompleteDamageStepDisplayData(this._listenerId, charId);
		}
	}

	// Token: 0x06001C3F RID: 7231 RVA: 0x000C31E8 File Offset: 0x000C13E8
	public bool HasAttributeToTopical(ItemKey itemKey)
	{
		bool flag = itemKey.ItemType != 8;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			AttributeMonitor majorAttributeMonitor = this._majorAttributeController.GetMonitor<AttributeMonitor>();
			MedicineItem medicineItem = Medicine.Instance.GetItem(itemKey.TemplateId);
			result = (medicineItem.RequiredMainAttributeType < 0 || majorAttributeMonitor.CurMainAttribute[(int)medicineItem.RequiredMainAttributeType] >= (short)medicineItem.RequiredMainAttributeValue);
		}
		return result;
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x000C3250 File Offset: 0x000C1450
	public void SetTabState(sbyte state)
	{
		bool lockTab = this.LockTab;
		if (!lockTab)
		{
			bool flag = -1 == state;
			if (flag)
			{
				this.SetVisible(false);
			}
			else
			{
				bool flag2 = state < 3;
				if (flag2)
				{
					this.SetVisible(true);
					this.InitTabToggleGroup();
					base.CGet<CToggleGroupObsolete>("TabToggleGroup").Set((int)state, true, false);
				}
			}
		}
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x000C32AC File Offset: 0x000C14AC
	public void SetVisible(bool active)
	{
		bool flag = this._originalPos == null;
		if (!flag)
		{
			if (active)
			{
				((RectTransform)base.transform).anchoredPosition = this._originalPos.Value;
			}
			else
			{
				((RectTransform)base.transform).anchoredPosition = this._originalPos.Value + Vector2.right * 1000f;
			}
		}
	}

	// Token: 0x06001C42 RID: 7234 RVA: 0x000C3328 File Offset: 0x000C1528
	public void ReleaseCharacter()
	{
		this._majorAttributeController.CharacterId = -1;
		this._secondaryAttributeController.CharacterId = -1;
		EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
		if (eatingItemMonitor != null)
		{
			eatingItemMonitor.RemoveEatingItemListener(new Action(this.FillEatingItem));
		}
		this._eatingItemMonitor = null;
		this._poisonGroupController.CharacterId = -1;
		this._injuryGroupController.CharacterId = -1;
		this._disorderOfQiController.CharacterId = -1;
		this._healthUIElement.CharacterId = -1;
		bool flag = this._samsaraInfoMonitor != null;
		if (flag)
		{
			this._samsaraInfoMonitor.RemoveSamsaraListener(new Action(this.OnGetCharacterSamsara));
			this._samsaraInfoMonitor.Character.CombatCharacter = null;
		}
		this._samsaraInfoMonitor = null;
		this._samsaraElementControllerList.ForEach(delegate(CharacterUIElement e)
		{
			e.CharacterId = -1;
		});
		this._characterId = -1;
	}

	// Token: 0x06001C43 RID: 7235 RVA: 0x000C341C File Offset: 0x000C161C
	private void TempSetTableState(sbyte state)
	{
		bool flag = CharacterAttributeDataView.CurTabIndex == state;
		if (!flag)
		{
			this._storedTabIndex = CharacterAttributeDataView.CurTabIndex;
			this.SetTabState(state);
		}
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x000C344C File Offset: 0x000C164C
	private void BackToPrevState()
	{
		bool flag = CharacterAttributeDataView.CurTabIndex == this._storedTabIndex;
		if (flag)
		{
			this._storedTabIndex = -1;
		}
		bool flag2 = this._storedTabIndex < 0;
		if (!flag2)
		{
			this.SetTabState(this._storedTabIndex);
			this._storedTabIndex = -1;
		}
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x000C3498 File Offset: 0x000C1698
	private unsafe void OnTabToggleGroupChange(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		GameObject gameObject;
		bool flag = this.CTryGet<GameObject>("TabCharacterDisplay", out gameObject);
		if (flag)
		{
			this.OnTabToggleGroupChangeForCharacterMenuEquip(newTog, preTog);
		}
		else
		{
			CharacterAttributeDataView.CurTabIndex = (sbyte)newTog.Key;
			base.CGet<Refers>("TabAttribute").gameObject.SetActive(CharacterAttributeDataView.CurTabIndex == 0);
			base.CGet<Refers>("TabInjury").gameObject.SetActive(CharacterAttributeDataView.CurTabIndex == 1);
			base.CGet<Refers>("TabSamsara").gameObject.SetActive(CharacterAttributeDataView.CurTabIndex == 2);
			bool flag2 = CharacterAttributeDataView.CurTabIndex == 2 && this._samsaraInfoMonitor != null;
			if (flag2)
			{
				PreexistenceCharIds preexistenceCharIds = this._samsaraInfoMonitor.PreexistenceCharIds;
				for (int i = 0; i < 9; i++)
				{
					bool flag3 = this._samsaraInfoMonitor.PreLifeCharacters[i] != null;
					if (flag3)
					{
						int index = preexistenceCharIds.GetIndexByPos(i);
						this._preLifeTableController[i].CharacterId = *(ref preexistenceCharIds.CharIds.FixedElementField + (IntPtr)index * 4);
					}
				}
			}
		}
	}

	// Token: 0x06001C46 RID: 7238 RVA: 0x000C35B8 File Offset: 0x000C17B8
	private void OnTabToggleGroupChangeForCharacterMenuEquip(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		CharacterAttributeDataView.CurTabIndex = (sbyte)newTog.Key;
		GameObject tabCharacterDisplay;
		bool flag = this.CTryGet<GameObject>("TabCharacterDisplay", out tabCharacterDisplay);
		if (flag)
		{
			tabCharacterDisplay.SetActive(CharacterAttributeDataView.CurTabIndex == 0);
		}
		base.CGet<Refers>("TabAttribute").gameObject.SetActive(CharacterAttributeDataView.CurTabIndex == 1);
		base.CGet<Refers>("TabInjury").gameObject.SetActive(CharacterAttributeDataView.CurTabIndex == 2);
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x000C3630 File Offset: 0x000C1830
	public void ShowTabCharacterDisplay()
	{
		base.CGet<CToggleGroupObsolete>("TabToggleGroup").Set(0, true, false);
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x000C3648 File Offset: 0x000C1848
	public void ForceRefreshPageData()
	{
		this._majorAttributeController.Refresh();
		this._secondaryAttributeController.Refresh();
		EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
		if (eatingItemMonitor != null)
		{
			eatingItemMonitor.Refresh();
		}
		this._poisonGroupController.Refresh();
		this._injuryGroupController.Refresh();
		this._disorderOfQiController.Refresh();
		SamsaraMonitor samsaraInfoMonitor = this._samsaraInfoMonitor;
		if (samsaraInfoMonitor != null)
		{
			samsaraInfoMonitor.Refresh();
		}
		this._healthUIElement.Refresh();
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x000C36C4 File Offset: 0x000C18C4
	public unsafe void SetEquipCompareResult(CharacterAttributeDisplayData data, CharacterAttributeDisplayData deltaData = null)
	{
		bool flag = data == null;
		if (flag)
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				this._majorAttributeController.SetMainAttributeBonus(i, 0, -1);
			}
			for (sbyte j = 0; j < 4; j += 1)
			{
				this._majorAttributeController.SetAtkHitAttributeBonus(j, 0, -1);
				this._majorAttributeController.SetDefHitAttributeBonus(j, 0, -1);
			}
			this._majorAttributeController.SetAtkPenetrabilityBonus(true, 0, -1);
			this._majorAttributeController.SetAtkPenetrabilityBonus(false, 0, -1);
			this._majorAttributeController.SetDefPenetrabilityBonus(true, 0, -1);
			this._majorAttributeController.SetDefPenetrabilityBonus(false, 0, -1);
			this._secondaryAttributeController.FillElement();
		}
		else
		{
			bool flag2 = deltaData == null;
			if (flag2)
			{
				for (sbyte k = 0; k < 6; k += 1)
				{
					this._majorAttributeController.SetMainAttributeBonus(k, (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)k * 2)), -1);
				}
				for (sbyte l = 0; l < 4; l += 1)
				{
					this._majorAttributeController.SetAtkHitAttributeBonus(l, *(ref data.AtkHitAttribute.Items.FixedElementField + (IntPtr)l * 4), -1);
					this._majorAttributeController.SetDefHitAttributeBonus(l, *(ref data.DefHitAttribute.Items.FixedElementField + (IntPtr)l * 4), -1);
				}
				this._majorAttributeController.SetAtkPenetrabilityBonus(true, data.AtkPenetrability.Outer, -1);
				this._majorAttributeController.SetAtkPenetrabilityBonus(false, data.AtkPenetrability.Inner, -1);
				this._majorAttributeController.SetDefPenetrabilityBonus(true, data.DefPenetrability.Outer, -1);
				this._majorAttributeController.SetDefPenetrabilityBonus(false, data.DefPenetrability.Inner, -1);
				this._secondaryAttributeController.SetCompareValue(0, (float)data.RecoveryOfStanceAndBreath.Outer, -1f);
				this._secondaryAttributeController.SetCompareValue(1, (float)data.RecoveryOfStanceAndBreath.Inner, -1f);
				this._secondaryAttributeController.SetCompareValue(2, (float)data.MoveSpeed, -1f);
				this._secondaryAttributeController.SetCompareValue(3, (float)data.RecoveryOfFlaw, -1f);
				this._secondaryAttributeController.SetCompareValue(4, (float)data.CastSpeed, -1f);
				this._secondaryAttributeController.SetCompareValue(5, (float)data.RecoveryOfBlockedAcupoint, -1f);
				this._secondaryAttributeController.SetCompareValue(6, (float)data.WeaponSwitchSpeed, -1f);
				this._secondaryAttributeController.SetCompareValue(7, (float)data.AttackSpeed, -1f);
				this._secondaryAttributeController.SetCompareValue(8, (float)data.InnerRatio, -1f);
				this._secondaryAttributeController.SetCompareValue(9, (float)data.RecoveryOfQiDisorder, -1f);
			}
			else
			{
				for (sbyte m = 0; m < 6; m += 1)
				{
					this._majorAttributeController.SetMainAttributeBonus(m, (int)(*(ref deltaData.MaxMainAttributes.Items.FixedElementField + (IntPtr)m * 2)), (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)m * 2)));
				}
				for (sbyte n = 0; n < 4; n += 1)
				{
					this._majorAttributeController.SetAtkHitAttributeBonus(n, *(ref deltaData.AtkHitAttribute.Items.FixedElementField + (IntPtr)n * 4), *(ref data.AtkHitAttribute.Items.FixedElementField + (IntPtr)n * 4));
					this._majorAttributeController.SetDefHitAttributeBonus(n, *(ref deltaData.DefHitAttribute.Items.FixedElementField + (IntPtr)n * 4), *(ref data.DefHitAttribute.Items.FixedElementField + (IntPtr)n * 4));
				}
				this._majorAttributeController.SetAtkPenetrabilityBonus(true, deltaData.AtkPenetrability.Outer, data.AtkPenetrability.Outer);
				this._majorAttributeController.SetAtkPenetrabilityBonus(false, deltaData.AtkPenetrability.Inner, data.AtkPenetrability.Inner);
				this._majorAttributeController.SetDefPenetrabilityBonus(true, deltaData.DefPenetrability.Outer, data.DefPenetrability.Outer);
				this._majorAttributeController.SetDefPenetrabilityBonus(false, deltaData.DefPenetrability.Inner, data.DefPenetrability.Inner);
				this._secondaryAttributeController.SetCompareValue(0, (float)deltaData.RecoveryOfStanceAndBreath.Outer, (float)data.RecoveryOfStanceAndBreath.Outer);
				this._secondaryAttributeController.SetCompareValue(1, (float)deltaData.RecoveryOfStanceAndBreath.Inner, (float)data.RecoveryOfStanceAndBreath.Inner);
				this._secondaryAttributeController.SetCompareValue(2, (float)deltaData.MoveSpeed, (float)data.MoveSpeed);
				this._secondaryAttributeController.SetCompareValue(3, (float)deltaData.RecoveryOfFlaw, (float)data.RecoveryOfFlaw);
				this._secondaryAttributeController.SetCompareValue(4, (float)deltaData.CastSpeed, (float)data.CastSpeed);
				this._secondaryAttributeController.SetCompareValue(5, (float)deltaData.RecoveryOfBlockedAcupoint, (float)data.RecoveryOfBlockedAcupoint);
				this._secondaryAttributeController.SetCompareValue(6, (float)deltaData.WeaponSwitchSpeed, (float)data.WeaponSwitchSpeed);
				this._secondaryAttributeController.SetCompareValue(7, (float)deltaData.AttackSpeed, (float)data.AttackSpeed);
				this._secondaryAttributeController.SetCompareValue(8, (float)deltaData.InnerRatio, (float)data.InnerRatio);
				this._secondaryAttributeController.SetCompareValue(9, (float)deltaData.RecoveryOfQiDisorder, (float)data.RecoveryOfQiDisorder);
			}
		}
	}

	// Token: 0x06001C4A RID: 7242 RVA: 0x000C3C40 File Offset: 0x000C1E40
	private string OtherAttributeValueToShowString(float value)
	{
		return string.Format("{0}%", (int)value);
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x000C3C64 File Offset: 0x000C1E64
	private void InitAttributePage()
	{
		Refers attributeRefers = base.CGet<Refers>("TabAttribute");
		RectTransform mainAttributeHolder = attributeRefers.CGet<RectTransform>("MainAttributeHolder");
		RectTransform attackDefendHolder = attributeRefers.CGet<RectTransform>("AttackDefendAttributeHolder");
		RectTransform hitAvoidHolder = attributeRefers.CGet<RectTransform>("HitAvoidAttributeHolder");
		Dictionary<sbyte, Refers> mainAttributeRefersMap = new Dictionary<sbyte, Refers>();
		Dictionary<sbyte, Refers> recoveryRefersMap = new Dictionary<sbyte, Refers>();
		for (sbyte type = 0; type < 6; type += 1)
		{
			Transform child = mainAttributeHolder.GetChild((int)type);
			mainAttributeRefersMap.Add(type, child.GetComponent<Refers>());
			recoveryRefersMap.Add(type, child.GetComponent<Refers>().CGet<Refers>("Extra"));
		}
		Refers[] atkPenetrabilityRefers = new Refers[]
		{
			attackDefendHolder.GetChild(0).GetComponent<Refers>(),
			attackDefendHolder.GetChild(2).GetComponent<Refers>()
		};
		Refers[] defPenetrabilityRefers = new Refers[]
		{
			attackDefendHolder.GetChild(1).GetComponent<Refers>(),
			attackDefendHolder.GetChild(3).GetComponent<Refers>()
		};
		Refers[] atkHitValueRefers = new Refers[4];
		Refers[] defHitValueRefers = new Refers[4];
		for (sbyte i = 0; i < 4; i += 1)
		{
			atkHitValueRefers[(int)i] = hitAvoidHolder.GetChild((int)(i * 2)).GetComponent<Refers>();
			defHitValueRefers[(int)i] = hitAvoidHolder.GetChild((int)(i * 2 + 1)).GetComponent<Refers>();
		}
		this._majorAttributeController = new CharacterMajorAttribute(mainAttributeRefersMap, recoveryRefersMap, atkHitValueRefers, defHitValueRefers, atkPenetrabilityRefers, defPenetrabilityRefers);
		this._secondaryAttributeController = new CharacterSecondaryAttribute(new Dictionary<sbyte, Refers>
		{
			{
				0,
				attributeRefers.CGet<Refers>("AttributeSlider_JiashiSudu")
			},
			{
				1,
				attributeRefers.CGet<Refers>("AttributeSlider_TiqiSudu")
			},
			{
				2,
				attributeRefers.CGet<Refers>("AttributeSlider_YidongSudu")
			},
			{
				3,
				attributeRefers.CGet<Refers>("AttributeSlider_BufaWenjian")
			},
			{
				4,
				attributeRefers.CGet<Refers>("AttributeSlider_ShizhanSudu")
			},
			{
				5,
				attributeRefers.CGet<Refers>("AttributeSlider_YinqiChongGuan")
			},
			{
				6,
				attributeRefers.CGet<Refers>("AttributeSlider_BingqiQiehuan")
			},
			{
				7,
				attributeRefers.CGet<Refers>("AttributeSlider_GongjiSudu")
			},
			{
				8,
				attributeRefers.CGet<Refers>("AttributeSlider_NeigongFahui")
			},
			{
				9,
				attributeRefers.CGet<Refers>("AttributeSlider_TiaoxiTuna")
			}
		});
		this._secondaryAttributeController.SetValueToStringFunc(new Func<float, string>(this.OtherAttributeValueToShowString));
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x000C3EAC File Offset: 0x000C20AC
	private void InitCombatDifficultyTips()
	{
		Refers attributeRefers = base.CGet<Refers>("TabAttribute");
		RectTransform penetrationsRoot = attributeRefers.CGet<RectTransform>("DifficultyLayout_Penetrations");
		RectTransform penetrationResistsRoot = attributeRefers.CGet<RectTransform>("DifficultyLayout_PenetrationResists");
		RectTransform hitValuesRoot = attributeRefers.CGet<RectTransform>("DifficultyLayout_HitValues");
		RectTransform avoidValuesRoot = attributeRefers.CGet<RectTransform>("DifficultyLayout_AvoidValues");
		CharacterAttributeDataView.<>c__DisplayClass75_0 CS$<>8__locals1;
		CS$<>8__locals1.isTaiwu = this.IsTaiwu;
		CS$<>8__locals1.isTaiwuGroupChar = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamWithSpecialMember(this._characterId);
		CS$<>8__locals1.isValidCharacter = (this._characterId != -1);
		byte difficultyTemplateId = SingletonObject.getInstance<BasicGameData>().CombatDifficulty;
		CombatDifficultyItem config = CombatDifficulty.Instance.GetItem(difficultyTemplateId);
		CharacterAttributeDataView.<InitCombatDifficultyTips>g__SetDifficultyRoot|75_0(penetrationsRoot, (int)(config.Penetrations - 100), LanguageKey.LK_Combat_Attack_Value, ref CS$<>8__locals1);
		CharacterAttributeDataView.<InitCombatDifficultyTips>g__SetDifficultyRoot|75_0(penetrationResistsRoot, (int)(config.PenetrationResists - 100), LanguageKey.LK_Combat_Defend_Value, ref CS$<>8__locals1);
		CharacterAttributeDataView.<InitCombatDifficultyTips>g__SetDifficultyRoot|75_0(hitValuesRoot, (int)(config.HitValues - 100), LanguageKey.LK_WeaponHitRate, ref CS$<>8__locals1);
		CharacterAttributeDataView.<InitCombatDifficultyTips>g__SetDifficultyRoot|75_0(avoidValuesRoot, (int)(config.AvoidValues - 100), LanguageKey.LK_CombatAttribute_Avoid, ref CS$<>8__locals1);
		RectTransform secondAttributeRoot = attributeRefers.CGet<RectTransform>("DifficultyLayout_SecondAttribute");
		int valueOfRecoveryOfStance = (int)(config.RecoveryOfStanceAndBreath.Outer - 100);
		int valueOfRecoveryOfBreath = (int)(config.RecoveryOfStanceAndBreath.Inner - 100);
		int valueOfMoveSpeed = (int)(config.MoveSpeed - 100);
		int valueOfRecoveryOfFlaw = (int)(config.RecoveryOfFlaw - 100);
		int valueOfCastSpeed = (int)(config.CastSpeed - 100);
		int valueOfRecoveryOfBlockedAcupoint = (int)(config.RecoveryOfBlockedAcupoint - 100);
		int valueOfWeaponSwitchSpeed = (int)(config.WeaponSwitchSpeed - 100);
		int valueOfAttackSpeed = (int)(config.AttackSpeed - 100);
		int valueOfInnerRatio = (int)(config.InnerRatio - 100);
		int valueOfRecoveryOfQiDisorder = (int)(config.RecoveryOfQiDisorder - 100);
		bool hasUpValue = valueOfRecoveryOfStance > 0 || valueOfRecoveryOfBreath > 0 || valueOfMoveSpeed > 0 || valueOfRecoveryOfFlaw > 0 || valueOfCastSpeed > 0 || valueOfRecoveryOfBlockedAcupoint > 0 || valueOfWeaponSwitchSpeed > 0 || valueOfAttackSpeed > 0 || valueOfInnerRatio > 0 || valueOfRecoveryOfQiDisorder > 0;
		bool hasDownValue = valueOfRecoveryOfStance < 0 || valueOfRecoveryOfBreath < 0 || valueOfMoveSpeed < 0 || valueOfRecoveryOfFlaw < 0 || valueOfCastSpeed < 0 || valueOfRecoveryOfBlockedAcupoint < 0 || valueOfWeaponSwitchSpeed < 0 || valueOfAttackSpeed < 0 || valueOfInnerRatio < 0 || valueOfRecoveryOfQiDisorder < 0;
		TooltipInvoker secondAttributeUpTips = secondAttributeRoot.Find("Up").GetComponent<TooltipInvoker>();
		TooltipInvoker secondAttributeDownTips = secondAttributeRoot.Find("Down").GetComponent<TooltipInvoker>();
		secondAttributeUpTips.gameObject.SetActive((hasUpValue & CS$<>8__locals1.isValidCharacter) && !CS$<>8__locals1.isTaiwu && !CS$<>8__locals1.isTaiwuGroupChar);
		secondAttributeDownTips.gameObject.SetActive((hasDownValue & CS$<>8__locals1.isValidCharacter) && !CS$<>8__locals1.isTaiwu && !CS$<>8__locals1.isTaiwuGroupChar);
		bool flag = CS$<>8__locals1.isTaiwu | CS$<>8__locals1.isTaiwuGroupChar;
		if (!flag)
		{
			string[] secondAttributeTipsData = new string[2];
			secondAttributeTipsData[0] = LocalStringManager.Get(LanguageKey.LK_CombatDifficultyInfluence);
			CS$<>8__locals1.cacheList = EasyPool.Get<List<string>>();
			CS$<>8__locals1.cacheList.Add(LocalStringManager.Get(LanguageKey.LK_CombatDifficultyInfluenceEffect));
			CS$<>8__locals1.cacheList.Add(string.Empty);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(18, valueOfRecoveryOfStance, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(19, valueOfRecoveryOfBreath, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(20, valueOfMoveSpeed, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(21, valueOfRecoveryOfFlaw, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(22, valueOfCastSpeed, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(23, valueOfRecoveryOfBlockedAcupoint, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(24, valueOfWeaponSwitchSpeed, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(25, valueOfAttackSpeed, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(26, valueOfInnerRatio, ref CS$<>8__locals1);
			CharacterAttributeDataView.<InitCombatDifficultyTips>g__AddChangeInfo|75_1(27, valueOfRecoveryOfQiDisorder, ref CS$<>8__locals1);
			secondAttributeTipsData[1] = string.Join("\n", CS$<>8__locals1.cacheList).ColorReplace();
			bool flag2 = hasUpValue;
			if (flag2)
			{
				secondAttributeUpTips.PresetParam = secondAttributeTipsData;
			}
			bool flag3 = hasDownValue;
			if (flag3)
			{
				secondAttributeDownTips.PresetParam = secondAttributeTipsData;
			}
			EasyPool.Free<List<string>>(CS$<>8__locals1.cacheList);
		}
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x000C4248 File Offset: 0x000C2448
	private void Init()
	{
		Refers injuryRefers = base.CGet<Refers>("TabInjury");
		Refers injuryRefersRoot = injuryRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("InjuryRoot");
		injuryRefersRoot.CGet<Refers>("Chest").UserInt = 0;
		injuryRefersRoot.CGet<Refers>("Head").UserInt = 2;
		injuryRefersRoot.CGet<Refers>("Belly").UserInt = 1;
		injuryRefersRoot.CGet<Refers>("LeftHand").UserInt = 3;
		injuryRefersRoot.CGet<Refers>("RightHand").UserInt = 4;
		injuryRefersRoot.CGet<Refers>("RightLeg").UserInt = 6;
		this._injuryParts = new Refers[7];
		this._injuryParts[0] = injuryRefersRoot.CGet<Refers>("Chest");
		this._injuryParts[2] = injuryRefersRoot.CGet<Refers>("Head");
		this._injuryParts[1] = injuryRefersRoot.CGet<Refers>("Belly");
		this._injuryParts[3] = injuryRefersRoot.CGet<Refers>("LeftHand");
		this._injuryParts[4] = injuryRefersRoot.CGet<Refers>("RightHand");
		this._injuryParts[5] = injuryRefersRoot.CGet<Refers>("LeftLeg");
		this._injuryParts[6] = injuryRefersRoot.CGet<Refers>("RightLeg");
		this._outerTotalInjuryNotice = injuryRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total").CGet<GameObject>("OuterHighLightNotice");
		this._innerTotalInjuryNotice = injuryRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total").CGet<GameObject>("InnerHighLightNotice");
		Refers poisonRefersRoot = injuryRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("PoisonRoot");
		this._poisonTypes = new Refers[6];
		this._poisonTypes[0] = poisonRefersRoot.CGet<Refers>("Resist_Lie");
		this._poisonTypes[1] = poisonRefersRoot.CGet<Refers>("Resist_Yu");
		this._poisonTypes[2] = poisonRefersRoot.CGet<Refers>("Resist_Han");
		this._poisonTypes[3] = poisonRefersRoot.CGet<Refers>("Resist_Chi");
		this._poisonTypes[4] = poisonRefersRoot.CGet<Refers>("Resist_Fu");
		this._poisonTypes[5] = poisonRefersRoot.CGet<Refers>("Resist_Huan");
		this._recoverChangeAddMin = injuryRefers.CGet<Refers>("AreaPeriodEffect").CGet<CImage>("RecoverChangeAddMin");
		this._recoverChangeAddMax = injuryRefers.CGet<Refers>("AreaPeriodEffect").CGet<CImage>("RecoverChangeAddMax");
		this._recoverChangeReduceMin = injuryRefers.CGet<Refers>("AreaPeriodEffect").CGet<CImage>("RecoverChangeReduceMin");
		this._recoverChangeReduceMax = injuryRefers.CGet<Refers>("AreaPeriodEffect").CGet<CImage>("RecoverChangeReduceMax");
		this._qiProgress = injuryRefers.CGet<Refers>("AreaPeriodEffect").CGet<CImage>("Image_Real_Progress");
		this.HideInfectNotice(true, true);
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x000C44E0 File Offset: 0x000C26E0
	public void ShowEatDropNotice(ItemDisplayData itemDisplayData, int amount = 1)
	{
		bool flag = itemDisplayData.Key.ItemType == 12 && itemDisplayData.Key.TemplateId != 265;
		if (!flag)
		{
			bool flag2;
			if (itemDisplayData.Key.ItemType == 8)
			{
				MedicineItem medicineItem = Medicine.Instance[itemDisplayData.Key.TemplateId];
				short? num = (medicineItem != null) ? new short?(medicineItem.Duration) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				int num3 = 0;
				flag2 = (num2.GetValueOrDefault() == num3 & num2 != null);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (!flag3)
			{
				List<Refers> eatingItemRefers = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaEatingItems").CGetList<Refers>("Item_");
				bool isWug = EatingItems.IsWug(itemDisplayData.Key);
				bool isWugKing = isWug && EatingItems.IsWugKing(itemDisplayData.Key);
				int begin = isWug ? (eatingItemRefers.Count - 1) : 0;
				int end = isWug ? 0 : (eatingItemRefers.Count - 1);
				int step = isWug ? -1 : 1;
				int preferIndex = -1;
				int normalIndex = -1;
				for (int i = begin; i != end; i += step)
				{
					ItemKey existItemKey = this._eatingItemMonitor.EatingItemList[i].Item1;
					bool flag4 = !EatingItems.IsValid(existItemKey) && normalIndex < 0;
					if (flag4)
					{
						normalIndex = i;
					}
					bool flag5 = EatingItems.IsWugKing(existItemKey) && isWugKing && preferIndex < 0;
					if (flag5)
					{
						preferIndex = i;
					}
				}
				int finalIndex = (preferIndex >= 0) ? preferIndex : normalIndex;
				for (int j = 0; j < (int)this._eatingItemMonitor.CanEatingMaxCount; j++)
				{
					ItemKey existItemKey2 = this._eatingItemMonitor.EatingItemList[j].Item1;
					bool hasItem = EatingItems.IsValid(existItemKey2) || EatingItems.IsWugKing(existItemKey2);
					bool show = j >= finalIndex && amount > 0 && !hasItem;
					bool flag6 = show;
					if (flag6)
					{
						amount -= ItemTemplateHelper.GetItemCountUnit(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
					}
					eatingItemRefers[j].CGet<GameObject>("Highlight").SetActive(show);
				}
				this.TempSetTableState(1);
			}
		}
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x000C4740 File Offset: 0x000C2940
	public void HideEatDropNotice(bool backToPrevState = true)
	{
		List<Refers> eatingItemRefers = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaEatingItems").CGetList<Refers>("Item_");
		for (int i = 0; i < eatingItemRefers.Count; i++)
		{
			eatingItemRefers[i].CGet<GameObject>("Highlight").SetActive(false);
		}
		if (backToPrevState)
		{
			this.BackToPrevState();
		}
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x000C47A8 File Offset: 0x000C29A8
	public bool IsRecoverInnerOuterMedicineCanUse(short templateId, out string tipContent)
	{
		tipContent = string.Empty;
		AttributeMonitor majorAttributeMonitor = this._majorAttributeController.GetMonitor<AttributeMonitor>();
		bool flag = majorAttributeMonitor == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MedicineItem medicineItem = Medicine.Instance.GetItem(templateId);
			bool flag2 = medicineItem == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = medicineItem.EffectType != EMedicineEffectType.RecoverInnerInjury && medicineItem.EffectType > EMedicineEffectType.RecoverOuterInjury;
				if (flag3)
				{
					tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
					result = false;
				}
				else
				{
					bool flag4 = medicineItem.Duration == 0;
					if (flag4)
					{
						bool isCombat = UIManager.Instance.IsElementActive(UIElement.Combat);
						Injuries injuries = this.GetInjuryPoisonMonitor().Injuries;
						ValueTuple<sbyte, sbyte> sum = injuries.GetBothSum();
						sbyte value = (medicineItem.EffectType == EMedicineEffectType.RecoverOuterInjury) ? sum.Item1 : sum.Item2;
						bool hasInjury = value > 0;
						bool flag5 = !isCombat && !hasInjury;
						if (flag5)
						{
							tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
							return false;
						}
						bool flag6 = hasInjury;
						if (flag6)
						{
							bool isInner = medicineItem.EffectType == EMedicineEffectType.RecoverInnerInjury;
							bool isMeetEffectThreshold = false;
							for (sbyte i = 0; i < 7; i += 1)
							{
								sbyte injuryValue = injuries.Get(i, isInner);
								bool flag7 = injuryValue > 0 && (short)injuryValue <= medicineItem.EffectThresholdValue;
								if (flag7)
								{
									isMeetEffectThreshold = true;
									break;
								}
							}
							bool flag8 = !isMeetEffectThreshold;
							if (flag8)
							{
								tipContent = LanguageKey.LK_UsingMedicine_Tip_Value_Not_Enough.Tr();
								return false;
							}
						}
					}
					bool flag9 = medicineItem.RequiredMainAttributeType >= 0 && majorAttributeMonitor.CurMainAttribute[(int)medicineItem.RequiredMainAttributeType] < (short)medicineItem.RequiredMainAttributeValue;
					if (flag9)
					{
						ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Strength + (int)medicineItem.RequiredMainAttributeType;
						CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[type.ToInt()];
						tipContent = LocalStringManager.GetFormat(LanguageKey.LK_UsingMedicine_Tip_Attribute_Not_Enough, characterPropertyDisplayItem.Name);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x000C4988 File Offset: 0x000C2B88
	public void ShowInfectNotice(ItemDisplayData itemDisplayData, int amount = 1)
	{
		this._showingInfectNotice = true;
		InjuryPoisonMonitor injuryPoisonMonitor = this._poisonGroupController.GetMonitor<InjuryPoisonMonitor>();
		SimulateEatingEffectResult simulateEatingEffectResult = new SimulateEatingEffectResult();
		CharacterDomainMethod.AsyncCall.SimulateEatingEffect(null, this._characterId, itemDisplayData.Key, amount, delegate(int offset, RawDataPool dataPool)
		{
			bool flag = !this._showingInfectNotice;
			if (!flag)
			{
				Serializer.Deserialize(dataPool, offset, ref simulateEatingEffectResult);
				switch (itemDisplayData.Key.ItemType)
				{
				case 7:
				{
					FoodItem foodItem = Food.Instance.GetItem(itemDisplayData.Key.TemplateId);
					break;
				}
				case 8:
				{
					MedicineItem medicineItem = Medicine.Instance.GetItem(itemDisplayData.Key.TemplateId);
					bool flag2 = medicineItem != null;
					if (flag2)
					{
						switch (medicineItem.EffectType)
						{
						case EMedicineEffectType.RecoverOuterInjury:
						case EMedicineEffectType.RecoverInnerInjury:
						{
							bool flag3 = medicineItem.Duration == 0;
							if (flag3)
							{
								base.<ShowInfectNotice>g__ShowInjury|5(medicineItem);
							}
							break;
						}
						case EMedicineEffectType.RecoverHealth:
							base.<ShowInfectNotice>g__ShowHealth|2();
							break;
						case EMedicineEffectType.ChangeDisorderOfQi:
							base.<ShowInfectNotice>g__ShowDisorderOfQi|3();
							break;
						case EMedicineEffectType.DetoxPoison:
						case EMedicineEffectType.ApplyPoison:
							base.<ShowInfectNotice>g__ShowPoison|4(medicineItem);
							break;
						}
					}
					break;
				}
				case 9:
				{
					TeaWineItem teaWineItem = TeaWine.Instance.GetItem(itemDisplayData.Key.TemplateId);
					base.<ShowInfectNotice>g__ShowDisorderOfQi|3();
					break;
				}
				}
			}
		});
		this.TempSetTableState(1);
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x000C4A00 File Offset: 0x000C2C00
	public void HideInfectNotice(bool backToPrevState = true, bool refreshData = true)
	{
		this._showingInfectNotice = false;
		bool flag = this._poisonGroupController != null && refreshData;
		if (flag)
		{
			InjuryPoisonMonitor injuryPoisonMonitor = this._poisonGroupController.GetMonitor<InjuryPoisonMonitor>();
			if (injuryPoisonMonitor != null)
			{
				injuryPoisonMonitor.OnDataInit();
			}
		}
		if (backToPrevState)
		{
			this.BackToPrevState();
		}
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x000C4A50 File Offset: 0x000C2C50
	public void OnBodyPartPointEnter(GameObject obj)
	{
		Refers refers = obj.GetComponentInParent<Refers>();
		this.SetMouseTipBodyPartParams(refers);
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x000C4A70 File Offset: 0x000C2C70
	private void InitInjuryPage()
	{
		Refers rootRefers = base.CGet<Refers>("TabInjury");
		Refers injuryRefers = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("InjuryRoot");
		this._injuryGroupRefers = new Refers[8];
		this._injuryGroupRefers[0] = injuryRefers.CGet<Refers>("Chest");
		this._injuryGroupRefers[1] = injuryRefers.CGet<Refers>("Belly");
		this._injuryGroupRefers[2] = injuryRefers.CGet<Refers>("Head");
		this._injuryGroupRefers[3] = injuryRefers.CGet<Refers>("LeftHand");
		this._injuryGroupRefers[4] = injuryRefers.CGet<Refers>("RightHand");
		this._injuryGroupRefers[5] = injuryRefers.CGet<Refers>("LeftLeg");
		this._injuryGroupRefers[6] = injuryRefers.CGet<Refers>("RightLeg");
		this._injuryGroupRefers[7] = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total");
		Refers refers = base.CGet<Refers>("TabInjury");
		GameObject mixPoisonBorder = refers.CGet<GameObject>("MixPoisonBorder");
		this._injuryGroupController = new CharacterInjuryGroup(this._injuryGroupRefers, mixPoisonBorder);
		this._injuryGroupController.CustomFillElement = new Action<Refers, sbyte, sbyte, sbyte>(this.FillInjuryElement);
		this._injuryGroupController.MixPoisonFillElement = new Action<GameObject>(this.FillMixPoisonElement);
		Refers personRefers = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Person");
		this._bodyAnimation = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<DOTweenAnimation>("PartsRoot");
		this._newlyInnerInjuryParts = new GameObject[7];
		this._newlyInnerInjuryParts[0] = personRefers.CGet<GameObject>("ChestInnerNew");
		this._newlyInnerInjuryParts[1] = personRefers.CGet<GameObject>("BellyInnerNew");
		this._newlyInnerInjuryParts[2] = personRefers.CGet<GameObject>("HeadInnerNew");
		this._newlyInnerInjuryParts[3] = personRefers.CGet<GameObject>("LeftHandInnerNew");
		this._newlyInnerInjuryParts[4] = personRefers.CGet<GameObject>("RightHandInnerNew");
		this._newlyInnerInjuryParts[5] = personRefers.CGet<GameObject>("LeftLegInnerNew");
		this._newlyInnerInjuryParts[6] = personRefers.CGet<GameObject>("RightLegInnerNew");
		this._newlyOuterInjuryParts = new GameObject[7];
		this._newlyOuterInjuryParts[0] = personRefers.CGet<GameObject>("ChestOuterNew");
		this._newlyOuterInjuryParts[1] = personRefers.CGet<GameObject>("BellyOuterNew");
		this._newlyOuterInjuryParts[2] = personRefers.CGet<GameObject>("HeadOuterNew");
		this._newlyOuterInjuryParts[3] = personRefers.CGet<GameObject>("LeftHandOuterNew");
		this._newlyOuterInjuryParts[4] = personRefers.CGet<GameObject>("RightHandOuterNew");
		this._newlyOuterInjuryParts[5] = personRefers.CGet<GameObject>("LeftLegOuterNew");
		this._newlyOuterInjuryParts[6] = personRefers.CGet<GameObject>("RightLegOuterNew");
		Refers poisonRefers = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("PoisonRoot");
		Refers[] allPoisonRefersArray = new Refers[]
		{
			poisonRefers.CGet<Refers>("Resist_Lie"),
			poisonRefers.CGet<Refers>("Resist_Yu"),
			poisonRefers.CGet<Refers>("Resist_Han"),
			poisonRefers.CGet<Refers>("Resist_Chi"),
			poisonRefers.CGet<Refers>("Resist_Fu"),
			poisonRefers.CGet<Refers>("Resist_Huan")
		};
		GameObject[] allPoisonMarksArray = new GameObject[6];
		Refers poisonMarkRefers = poisonRefers.CGet<Refers>("Poisoned");
		allPoisonMarksArray[0] = poisonMarkRefers.CGet<GameObject>("Lie");
		allPoisonMarksArray[1] = poisonMarkRefers.CGet<GameObject>("Yu");
		allPoisonMarksArray[2] = poisonMarkRefers.CGet<GameObject>("Han");
		allPoisonMarksArray[3] = poisonMarkRefers.CGet<GameObject>("Chi");
		allPoisonMarksArray[4] = poisonMarkRefers.CGet<GameObject>("Fu");
		allPoisonMarksArray[5] = poisonMarkRefers.CGet<GameObject>("Huan");
		this._poisonGroupController = new CharacterPoisonGroup(allPoisonRefersArray, allPoisonMarksArray);
		TextMeshProUGUI label = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total").CGet<TextMeshProUGUI>("PoisonValue");
		this._poisonGroupController.InitTotalLabel(label);
		this._poisonGroupController.GetOldPoisonHandler = new Func<ValueTuple<int[], int[]>>(this.GetOldPoisonData);
		Refers disorderOfQiRefers = rootRefers.CGet<Refers>("AreaPeriodEffect");
		CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
		TextMeshProUGUI stateLabel = disorderOfQiRefers.CGet<TextMeshProUGUI>("QiStateText");
		CImage stateIcon = disorderOfQiRefers.CGet<CImage>("QiStateIcon");
		QiContainer qiContainer = disorderOfQiRefers.CGet<QiContainer>("QiContainer");
		TooltipInvoker mouseTip = disorderOfQiRefers.CGet<TooltipInvoker>("QiMouseTip");
		this._disorderOfQiController = new CharacterDisorderOfQi(slider, stateLabel, stateIcon, qiContainer, mouseTip);
		this._disorderOfQiController.OnFillDisorderOfQi = new Action<float>(this.OnFillDisorderOfQi);
		this._disorderOfQiController.OnFillChangeOfDisorderOfQi = new Action<short>(this.OnFillChangeDisorderOfQiSlider);
		CButtonObsolete usingMedicineButton = refers.CGet<CButtonObsolete>("UsingMedicineInteractButton");
		usingMedicineButton.ClearAndAddListener(delegate
		{
			this.OnClickShowMedicineItem(UsingMedicineItemType.Invalid);
		});
		CButtonObsolete healButton = base.CGet<Refers>("TabInjury").CGet<CButtonObsolete>("HealInteractButton");
		healButton.ClearAndAddListener(delegate
		{
			this.OnClickShowHealUI();
		});
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x000C4F38 File Offset: 0x000C3138
	private void InitHealthInfo()
	{
		Refers refers = base.CGet<Refers>("TabInjury");
		this._healthUIElement = new CharacterHealth(refers.CGet<CharacterHealthBar>("CharacterHealthInfo"));
		this._healthUIElement.SetGetHealthStringFunc(new Func<short[], int, string>(this.GetHealthString));
		this._healthUIElement.OnFillHealthChange = new Action(this.OnFillHealthChange);
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x000C4F98 File Offset: 0x000C3198
	private string GetHealthString(short[] paramsHealth, int characterId)
	{
		bool flag = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
		if (flag)
		{
			paramsHealth[0] = paramsHealth[1];
		}
		return CommonUtils.GetCharacterHealthInfo(paramsHealth[0], paramsHealth[1], characterId).Item1;
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x000C4FD8 File Offset: 0x000C31D8
	private void OnFillHealthChange()
	{
		Refers healthBarRefers = this._healthUIElement.CharacterHealthBar.GetComponent<Refers>();
		AgeHealthMonitor ageHealthMonitor = this._healthUIElement.GetMonitor<AgeHealthMonitor>();
		bool flag = ageHealthMonitor != null;
		if (flag)
		{
			float increaseRate = (float)(ageHealthMonitor.Health + ageHealthMonitor.HealthRecovery) / (float)ageHealthMonitor.LeftMaxHealth;
			float reduceRate = (float)Math.Abs(ageHealthMonitor.HealthRecovery) / (float)ageHealthMonitor.LeftMaxHealth;
		}
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x000C5040 File Offset: 0x000C3240
	private unsafe ValueTuple<int[], int[]> GetOldPoisonData()
	{
		bool flag = this._samsaraInfoMonitor != null && this._samsaraInfoMonitor.Character.CombatCharacter != null;
		ValueTuple<int[], int[]> result;
		if (flag)
		{
			int[] oldPoison = new int[6];
			int[] oldPoisonResists = new int[6];
			for (sbyte i = 0; i < 6; i += 1)
			{
				oldPoison[(int)i] = *(ref this._samsaraInfoMonitor.Character.CombatCharacter.OldPoisons.Items.FixedElementField + (IntPtr)i * 4);
			}
			result = new ValueTuple<int[], int[]>(oldPoison, null);
		}
		else
		{
			result = new ValueTuple<int[], int[]>(null, null);
		}
		return result;
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x000C50D4 File Offset: 0x000C32D4
	private void OnFillDisorderOfQi(float value)
	{
		CImage cImage = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect").CGet<CImage>("Image_Real_Progress");
		bool flag = cImage != null;
		if (flag)
		{
			cImage.fillAmount = value;
		}
		CharacterAttributeDataView.UpdateDisorderOfQiSliderHandle(base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect"), value);
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x000C5134 File Offset: 0x000C3334
	public static void UpdateDisorderOfQiSliderHandle(Refers disorderOfQiRefers, float percentValue)
	{
		RectTransform qiStateBack = disorderOfQiRefers.CGet<RectTransform>("QiStateBack");
		RectTransform qiStateBackA = disorderOfQiRefers.CGet<RectTransform>("QiStateBackA");
		RectTransform qiStateBackB = disorderOfQiRefers.CGet<RectTransform>("QiStateBackB");
		RectTransform qiChangeMask = disorderOfQiRefers.CGet<RectTransform>("QiChangeMask");
		float pivotX = Mathf.Lerp(0.2f, 0.8f, percentValue);
		qiStateBack.pivot = qiStateBack.pivot.SetX(pivotX);
		qiStateBack.anchoredPosition = Vector2.zero;
		qiStateBackA.anchorMax = qiStateBackA.anchorMax.SetX(pivotX);
		qiStateBackA.anchoredPosition = Vector2.zero;
		qiStateBackB.anchorMin = qiStateBackA.anchorMin.SetX(pivotX);
		qiStateBackB.anchoredPosition = Vector2.zero;
		qiChangeMask.anchorMax = (qiChangeMask.anchorMin = qiChangeMask.anchorMin.SetX(pivotX));
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x000C5204 File Offset: 0x000C3404
	private void OnFillChangeDisorderOfQiSlider(short changeValue)
	{
		Refers rootRefers = base.CGet<Refers>("TabInjury");
		Refers disorderOfQiRefers = rootRefers.CGet<Refers>("AreaPeriodEffect");
		CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
		GameObject betterImg = disorderOfQiRefers.CGet<GameObject>("Image_RecoverBetter");
		GameObject worseImg = disorderOfQiRefers.CGet<GameObject>("Image_RecoverWorse");
		RectTransform qiChangeMask = disorderOfQiRefers.CGet<RectTransform>("QiChangeMask");
		float totalWidth = disorderOfQiRefers.CGet<CImage>("Image_Real_Progress").rectTransform.rect.width;
		SamsaraMonitor samsaraInfoMonitor = this._samsaraInfoMonitor;
		bool flag = ((samsaraInfoMonitor != null) ? samsaraInfoMonitor.Character.CombatCharacter : null) != null;
		if (flag)
		{
			short oldDisorderOfQi = this._samsaraInfoMonitor.Character.CombatCharacter.OldDisorderOfQi;
			short curDisorderOfQi = this._disorderOfQiController.GetMonitor<DisorderOfQiMonitor>().DisorderOfQi;
			betterImg.SetActive(curDisorderOfQi <= oldDisorderOfQi);
			worseImg.SetActive(curDisorderOfQi > oldDisorderOfQi);
			qiChangeMask.SetPivot(qiChangeMask.pivot.SetX((float)((curDisorderOfQi <= oldDisorderOfQi) ? 0 : 1)));
			qiChangeMask.anchoredPosition = qiChangeMask.anchoredPosition.SetX(0f);
			qiChangeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)Mathf.Abs((int)(curDisorderOfQi - oldDisorderOfQi)) / slider.maxValue * totalWidth);
		}
		else
		{
			betterImg.SetActive(changeValue <= 0);
			worseImg.SetActive(changeValue > 0);
			qiChangeMask.SetPivot(qiChangeMask.pivot.SetX((float)((changeValue > 0) ? 0 : 1)));
			qiChangeMask.anchoredPosition = qiChangeMask.anchoredPosition.SetX(0f);
			qiChangeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)Mathf.Abs((int)changeValue) / slider.maxValue * totalWidth);
		}
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x000C53AC File Offset: 0x000C35AC
	private void ShowPreviewReduceDisorderOfQi(bool showChange, bool showMask, short changeValue)
	{
		Refers rootRefers = base.CGet<Refers>("TabInjury");
		Refers disorderOfQiRefers = rootRefers.CGet<Refers>("AreaPeriodEffect");
		CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
		GameObject betterImg = disorderOfQiRefers.CGet<GameObject>("Image_RecoverBetter");
		GameObject worseImg = disorderOfQiRefers.CGet<GameObject>("Image_RecoverWorse");
		RectTransform qiChangeMask = disorderOfQiRefers.CGet<RectTransform>("QiChangeMask");
		float totalWidth = disorderOfQiRefers.CGet<CImage>("Image_Real_Progress").rectTransform.rect.width;
		betterImg.SetActive(showChange && changeValue < 0);
		worseImg.SetActive(showChange && changeValue > 0);
		qiChangeMask.SetPivot(qiChangeMask.pivot.SetX((float)((changeValue > 0) ? 0 : 1)));
		qiChangeMask.anchoredPosition = qiChangeMask.anchoredPosition.SetX(0f);
		float maskWidth = showMask ? ((float)Mathf.Abs((int)changeValue) / slider.maxValue * totalWidth) : 0f;
		qiChangeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskWidth);
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x000C54A8 File Offset: 0x000C36A8
	private void FillInjuryElement(Refers refers, sbyte bodyPartType, sbyte outerValue, sbyte innerValue)
	{
		SamsaraMonitor samsaraInfoMonitor = this._samsaraInfoMonitor;
		bool flag = ((samsaraInfoMonitor != null) ? samsaraInfoMonitor.Character.CombatCharacter : null) == null;
		if (flag)
		{
			refers.CGet<GameObject>("NewInner").SetActive(false);
			refers.CGet<GameObject>("NewOuter").SetActive(false);
			bool showInner = innerValue > 0;
			bool showOuter = outerValue > 0;
			this._newlyInnerInjuryParts[(int)bodyPartType].SetActive(showInner);
			this._newlyOuterInjuryParts[(int)bodyPartType].SetActive(showOuter);
			refers.CGet<TextMeshProUGUI>("InnerValue").text = innerValue.ToString().SetColor((innerValue <= 0) ? "grey" : "innerinjury");
			refers.CGet<TextMeshProUGUI>("OuterValue").text = outerValue.ToString().SetColor((outerValue <= 0) ? "grey" : "outterinjury");
			DOTweenAnimation bodyAnimation = this._bodyAnimation;
			object obj;
			if (bodyAnimation == null)
			{
				obj = null;
			}
			else
			{
				DOTweenAnimation component = bodyAnimation.GetComponent<DOTweenAnimation>();
				obj = ((component != null) ? component.tween : null);
			}
			bool flag2 = obj != null;
			if (flag2)
			{
				this._bodyAnimation.DORestart();
				this._bodyAnimation.DOPause();
			}
		}
		else
		{
			ValueTuple<sbyte, sbyte> valueTuple = this._samsaraInfoMonitor.Character.CombatCharacter.OldInjuries.Get(bodyPartType);
			sbyte oldOuter = valueTuple.Item1;
			sbyte oldInner = valueTuple.Item2;
			int newAddInnerValue = (int)(innerValue - oldInner);
			int newAddOuterValue = (int)(outerValue - oldOuter);
			this._newlyInnerInjuryParts[(int)bodyPartType].SetActive(newAddInnerValue > 0);
			this._newlyOuterInjuryParts[(int)bodyPartType].SetActive(newAddOuterValue > 0);
			refers.CGet<GameObject>("NewInner").SetActive(newAddInnerValue > 0);
			refers.CGet<GameObject>("NewOuter").SetActive(newAddOuterValue > 0);
			refers.CGet<TextMeshProUGUI>("InnerValue").text = innerValue.ToString().SetColor((innerValue <= 0) ? "grey" : "innerinjury");
			refers.CGet<TextMeshProUGUI>("OuterValue").text = outerValue.ToString().SetColor((outerValue <= 0) ? "grey" : "outterinjury");
			refers.CGet<TextMeshProUGUI>("InnerValueNew").text = string.Format("+{0}", newAddInnerValue);
			refers.CGet<TextMeshProUGUI>("OuterValueNew").text = string.Format("+{0}", newAddOuterValue);
			bool flag3 = newAddInnerValue > 0;
			if (flag3)
			{
				refers.CGet<TextMeshProUGUI>("InnerValueNew").GetComponent<DOTweenAnimation>().DORestart();
				this._bodyAnimation.DORestart();
			}
			bool flag4 = newAddOuterValue > 0;
			if (flag4)
			{
				refers.CGet<TextMeshProUGUI>("OuterValueNew").GetComponent<DOTweenAnimation>().DORestart();
				this._bodyAnimation.DORestart();
			}
		}
		refers.CGet<GameObject>("HighLightNotice").SetActive(false);
		this.SetMouseTipBodyPartParams(refers);
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x000C5768 File Offset: 0x000C3968
	private void FillMixPoisonElement(GameObject heartBorder)
	{
		bool flag = this._characterId == -1;
		if (!flag)
		{
			CommonUtils.SetMixPoisonBorder(heartBorder, this._characterId);
		}
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x000C5794 File Offset: 0x000C3994
	private void FillEatingItem()
	{
		List<Refers> eatingItemRefers = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaEatingItems").CGetList<Refers>("Item_");
		Refers wugAnimations = base.CGet<Refers>("TabInjury").CGet<Refers>("WugAnimationReferences");
		int canEatingMaxCount = (int)this._eatingItemMonitor.CanEatingMaxCount;
		for (int i = 0; i < eatingItemRefers.Count; i++)
		{
			Refers refers = eatingItemRefers[i];
			CImage normalImage = refers.CGet<CImage>("Normal");
			CImage icon = refers.CGet<CImage>("Icon");
			TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("Mousetip");
			mouseTip.IsLanguageKey = false;
			refers.GetComponent<PointerTrigger>().EnterEvent.RemoveAllListeners();
			string iconName = string.Empty;
			ValueTuple<ItemKey, short> tuple = new ValueTuple<ItemKey, short>(ItemKey.Invalid, -1);
			bool flag = this._eatingItemMonitor.EatingItemList.CheckIndex(i);
			if (flag)
			{
				tuple = this._eatingItemMonitor.EatingItemList[i];
			}
			bool flag2 = EatingItems.IsValid(tuple.Item1);
			if (flag2)
			{
				normalImage.SetSprite("ui_charactermenu_02_foodbar_0", false, null);
				refers.GetComponent<PointerTrigger>().EnterEvent.AddListener(delegate()
				{
					refers.CGet<GameObject>("Hover").SetActive(true);
				});
				bool flag3 = !EatingItems.IsWug(tuple.Item1);
				if (flag3)
				{
					iconName = ItemTemplateHelper.GetIcon(tuple.Item1.ItemType, tuple.Item1.TemplateId);
				}
				bool flag4 = EatingItems.IsWug(tuple.Item1) && !EatingItems.IsWugKing(tuple.Item1);
				if (flag4)
				{
					mouseTip.Type = TipType.EatingWug;
					mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("TemplateId", tuple.Item1.TemplateId).Set("CharId", this._characterId).Set("EatingTime", tuple.Item2);
					refers.GetComponent<CButtonObsolete>().interactable = true;
				}
				else
				{
					ItemDomainMethod.AsyncCall.GetItemDisplayData(null, tuple.Item1, delegate(int offset, RawDataPool dataPool)
					{
						ItemDisplayData itemData = null;
						Serializer.Deserialize(dataPool, offset, ref itemData);
						mouseTip.Type = TooltipManager.ItemTypeToTipType[tuple.Item1.ItemType];
						bool flag6 = mouseTip.RuntimeParam == null;
						if (flag6)
						{
							mouseTip.RuntimeParam = new ArgumentBox();
						}
						mouseTip.RuntimeParam.SetObject("ItemData", itemData);
						mouseTip.RuntimeParam.Set("EatingTime", tuple.Item2);
						mouseTip.RuntimeParam.Set("CharId", this._characterId);
					});
				}
			}
			else
			{
				normalImage.SetSprite("ui_charactermenu_02_foodbar_2", false, null);
				refers.GetComponent<CButtonObsolete>().interactable = (i < canEatingMaxCount);
				string tipsContent = LocalStringManager.Get((i < canEatingMaxCount) ? LanguageKey.LK_CharacterMenu_Injury_EatingNull : LanguageKey.LK_CharacterMenu_Injury_EatingLimit);
				mouseTip.Type = TipType.SingleDesc;
				mouseTip.RuntimeParam = null;
				mouseTip.PresetParam = new string[]
				{
					tipsContent
				};
			}
			icon.SetSprite(iconName, false, null);
			bool isWug = EatingItems.IsWug(tuple.Item1);
			SkeletonGraphic wugSkeleton = refers.CGet<SkeletonGraphic>("Wug");
			wugSkeleton.gameObject.SetActive(isWug);
			bool flag5 = isWug;
			if (flag5)
			{
				MedicineItem wugConfig = Medicine.Instance[tuple.Item1.TemplateId];
				string wugName = CharacterAttributeDataView.WugSkeletonNames[(int)wugConfig.WugGrowthType];
				SkeletonDataAsset dataAsset = wugAnimations.CGet<SkeletonDataAsset>(wugName);
				CommonUtils.SetSkeletonDataAsset(wugSkeleton, dataAsset, "default", "animation", true);
				string slotOrAttachmentName = CharacterAttributeDataView.WugSkeletonSlotOrAttachmentNames[(int)wugConfig.WugGrowthType];
				string slotName = string.Format("images/{0}", slotOrAttachmentName);
				string attachmentName = string.Format("images/{0}_{1}", slotOrAttachmentName, (int)(wugConfig.WugType + 1));
				wugSkeleton.Skeleton.FindSlot(slotName).Attachment = wugSkeleton.Skeleton.GetAttachment(slotName, attachmentName);
			}
			bool showing = mouseTip.Showing;
			if (showing)
			{
				mouseTip.Refresh(false, -1);
			}
		}
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x000C5BAF File Offset: 0x000C3DAF
	public void DelayRefreshOnEatItemSend()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
		{
			this.SetCurrentCharacterId(this._characterId);
		});
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001C61 RID: 7265 RVA: 0x000C5BCA File Offset: 0x000C3DCA
	private MedicineItem SelectedInjuryPartUsingMedicineConfig
	{
		get
		{
			return (this._selectedInjuryPartUsingMedicine == null) ? null : Medicine.Instance[this._selectedInjuryPartUsingMedicine.RealKey.TemplateId];
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001C62 RID: 7266 RVA: 0x000C5BF1 File Offset: 0x000C3DF1
	public bool IsSelectInjuryPart
	{
		get
		{
			return this._selectedInjuryPartUsingMedicine != null;
		}
	}

	// Token: 0x06001C63 RID: 7267 RVA: 0x000C5C00 File Offset: 0x000C3E00
	public void EnterSelectInjuryPart(ItemDisplayData itemData, Action<List<sbyte>> onConfirm, Action onCancel)
	{
		this._onCancelSelectInjuryPart = onCancel;
		this._selectedInjuryPartUsingMedicine = itemData;
		this._selectedInjuryPartList.Clear();
		Refers rootRefers = base.CGet<Refers>("TabInjury");
		Refers injuryRefers = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("InjuryRoot");
		this.HighLight(injuryRefers.gameObject);
		this.RefreshInjuryPartForSelect(true, false);
		Action <>9__2;
		base.CGet<CButtonObsolete>("ButtonConfirm").ClearAndAddListener(delegate
		{
			bool flag = this._curCanSelectInjuryPartCount > 0 && this._selectedInjuryPartList.Count < (int)this.SelectedInjuryPartUsingMedicineConfig.InjuryRecoveryTimes;
			if (flag)
			{
				this.ShowSelectInjuryPartCursorTip(false);
				string title = LanguageKey.LK_Use_Medicine_InjuryRecoveryTimes_Dialog_Title.Tr();
				string content = LanguageKey.LK_Use_Medicine_InjuryRecoveryTimes_Dialog_Content.Tr();
				string title2 = title;
				string text = content;
				Action onConfirm2 = new Action(base.<EnterSelectInjuryPart>g__Confirm|1);
				Action onCancel2;
				if ((onCancel2 = <>9__2) == null)
				{
					onCancel2 = (<>9__2 = delegate()
					{
						this.ShowSelectInjuryPartCursorTip(true);
					});
				}
				CommonUtils.ShowConfirmDialog(title2, text, onConfirm2, onCancel2, EDialogType.None);
			}
			else
			{
				base.<EnterSelectInjuryPart>g__Confirm|1();
			}
		});
		base.CGet<CButtonObsolete>("ButtonCancel").ClearAndAddListener(new Action(this.ExitSelectInjuryPart));
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x000C5CB0 File Offset: 0x000C3EB0
	private void RefreshInjuryPartForSelect(bool isEnter, bool isClick)
	{
		CButtonObsolete buttonConfirm = base.CGet<CButtonObsolete>("ButtonConfirm");
		buttonConfirm.interactable = (this._selectedInjuryPartList.Count > 0);
		buttonConfirm.GetComponent<HSVStyleRoot>().SetInteractable(buttonConfirm.interactable);
		MedicineItem config = Medicine.Instance[this._selectedInjuryPartUsingMedicine.RealKey.TemplateId];
		InjuryPoisonMonitor injuryPoisonMonitor = this._poisonGroupController.GetMonitor<InjuryPoisonMonitor>();
		bool isInner = config.EffectType == EMedicineEffectType.RecoverInnerInjury;
		this._curCanSelectInjuryPartCount = 0;
		if (isEnter)
		{
			this._totalCanSelectInjuryPartCount = 0;
		}
		sbyte index = 0;
		while ((int)index < this._injuryParts.Length)
		{
			sbyte partType = index;
			Refers injuryPart = this._injuryParts[(int)index];
			sbyte injuryValue = injuryPoisonMonitor.Injuries.Get(index, isInner);
			bool isSelectedUsingMedicineItemType = !isEnter && !isClick && (short)partType == this._usingMedicineItemType;
			bool isSelected = this._selectedInjuryPartList.Contains(partType);
			bool hasInjury = injuryValue > 0 || UIManager.Instance.IsElementActive(UIElement.Combat);
			bool isMeetThreshold = (short)injuryValue <= config.EffectThresholdValue;
			bool canSelect = hasInjury && isMeetThreshold && this._selectedInjuryPartList.Count < (int)config.InjuryRecoveryTimes;
			bool flag = isEnter && canSelect;
			if (flag)
			{
				this._totalCanSelectInjuryPartCount++;
			}
			bool flag2 = canSelect && !isSelected;
			if (flag2)
			{
				this._curCanSelectInjuryPartCount++;
			}
			GameObject highLightNotice = injuryPart.CGet<GameObject>("HighLightNotice");
			highLightNotice.SetActive(isSelected);
			injuryPart.CGet<GameObject>("SelectedIcon").SetActive(isSelected || isSelectedUsingMedicineItemType);
			GameObject hoverIcon = injuryPart.CGet<GameObject>("HoverIcon");
			hoverIcon.SetActive(false);
			TextMeshProUGUI valueLabel = injuryPart.CGet<TextMeshProUGUI>(isInner ? "InnerValue" : "OuterValue");
			string originValueColor = (injuryValue > 0) ? (isInner ? "innerinjury" : "outterinjury") : "grey";
			bool flag3 = !isSelected;
			if (flag3)
			{
				valueLabel.text = injuryValue.ToString().SetColor(originValueColor);
			}
			PointerTrigger pointerTrigger = injuryPart.CGet<PointerTrigger>("PointerTrigger");
			pointerTrigger.IgnoreOnDisableTrigger = isSelected;
			pointerTrigger.enabled = (canSelect && !isSelected);
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.RemoveAllListeners();
			bool enabled = pointerTrigger.enabled;
			if (enabled)
			{
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					highLightNotice.SetActive(true);
					hoverIcon.SetActive(true);
					int curValue = Mathf.Max(0, (int)((short)injuryValue - config.EffectValue));
					valueLabel.text = curValue.ToString().SetColor("brightblue");
				});
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					highLightNotice.SetActive(false);
					hoverIcon.SetActive(false);
					valueLabel.text = injuryValue.ToString().SetColor(originValueColor);
				});
			}
			CButtonObsolete button = pointerTrigger.GetComponent<CButtonObsolete>();
			button.interactable = (canSelect | isSelected);
			button.onClick.RemoveAllListeners();
			bool interactable = button.interactable;
			if (interactable)
			{
				button.onClick.AddListener(delegate()
				{
					bool isSelected = isSelected;
					if (isSelected)
					{
						this._selectedInjuryPartList.Remove(partType);
					}
					else
					{
						this._selectedInjuryPartList.Add(partType);
					}
					this.RefreshInjuryPartForSelect(false, true);
				});
			}
			button.GetComponent<HSVStyleRoot>().SetInteractable(button.interactable);
			TooltipInvoker partTip = injuryPart.CGet<GameObject>("Back").GetComponent<TooltipInvoker>();
			bool flag4 = partTip;
			if (flag4)
			{
				partTip.enabled = false;
			}
			TooltipInvoker selectTip = button.GetComponent<TooltipInvoker>();
			selectTip.enabled = (!hasInjury || !isMeetThreshold);
			string[] presetParam = selectTip.PresetParam;
			bool flag5 = presetParam == null || presetParam.Length != 1;
			if (flag5)
			{
				selectTip.PresetParam = new string[1];
			}
			bool flag6 = !hasInjury;
			if (flag6)
			{
				selectTip.PresetParam[0] = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr().ColorReplace() + LanguageKey.LK_Ignore.Tr().SetColor("brightred");
			}
			else
			{
				bool flag7 = !isMeetThreshold;
				if (flag7)
				{
					selectTip.PresetParam[0] = LanguageKey.LK_UsingMedicine_Tip_Value_Not_Enough.Tr().ColorReplace() + LanguageKey.LK_Ignore.Tr().SetColor("brightred");
				}
			}
			index += 1;
		}
		this.ShowSelectInjuryPartCursorTip(true);
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x000C614C File Offset: 0x000C434C
	public void ExitSelectInjuryPart()
	{
		this._onCancelSelectInjuryPart();
		this._onCancelSelectInjuryPart = null;
		this._selectedInjuryPartList.Clear();
		this.RefreshInjuryPartForSelect(false, false);
		this._selectedInjuryPartUsingMedicine = null;
		this.CancelHighLight();
		this.RefreshInjuryAndPoison(this._canUseMedicineItem);
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x000C619D File Offset: 0x000C439D
	private void ShowSelectInjuryPartCursorTip(bool show)
	{
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x000C61A0 File Offset: 0x000C43A0
	private void InitSamsaraPage()
	{
		Refers samsaraPageRefers = base.CGet<Refers>("TabSamsara");
		CToggleGroupObsolete samsaraDataGroup = samsaraPageRefers.CGet<CToggleGroupObsolete>("SamsaraDataGroup");
		samsaraDataGroup.InitPreOnToggle(-1);
		samsaraDataGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSamsaraDataToggleGroupChange);
		this._preLifeTableController = new List<CommonCharacterToggle>();
		List<CommonCharacterToggle> characterTableRefersList = samsaraPageRefers.CGetList<CommonCharacterToggle>("Character_");
		for (int i = 0; i < characterTableRefersList.Count; i++)
		{
			CommonCharacterToggle controller = characterTableRefersList[i];
			controller.Refresh();
			controller.IsShowGrave = false;
			this._preLifeTableController.Add(controller);
		}
		CToggleGroupObsolete tableGroup = samsaraPageRefers.CGet<CToggleGroupObsolete>("CharacterTable");
		tableGroup.InitPreOnToggle(-1);
		tableGroup.SetInteractable(false, null);
		tableGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.UpdateCurrentPreLife);
		Refers attrPageRefers = samsaraPageRefers.CGet<Refers>("AttributeTabPage");
		this._samsaraElementControllerList = new List<CharacterUIElement>();
		this._samsaraElementControllerList.Add(new CharacterName(attrPageRefers.CGet<Refers>("CharacterName")));
		this._samsaraElementControllerList.Add(new CharacterAvatar(attrPageRefers.CGet<Game.Components.Avatar.Avatar>("Avatar"), false));
		this._samsaraElementControllerList.Add(new CharacterDetailInfo(attrPageRefers));
		TextMeshProUGUI deathDateLabel = attrPageRefers.CGet<TextMeshProUGUI>("DeadDate");
		CharacterAge characterAge = new CharacterAge(deathDateLabel, null, null, null, false, false, null, null);
		characterAge.GetAgeText = delegate(sbyte birthMonth, short displayAge)
		{
			bool flag = birthMonth < 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = LocalStringManager.GetFormat(LanguageKey.LK_Died_AtAge, displayAge);
			}
			return result;
		};
		this._samsaraElementControllerList.Add(characterAge);
		this._samsaraFeatureScroll = new CharacterFeatureScroll(samsaraPageRefers.CGet<InfinityScrollLegacy>("FeatureScroll"), null, false, null);
		this._samsaraElementControllerList.Add(this._samsaraFeatureScroll);
		this._samsaraElementControllerList.Add(new CharacterLifeSkillScroll(samsaraPageRefers.CGet<InfinityScrollLegacy>("LifeSkillScroll"), LifeOrCombatSkillScrollItemType.Both, ESkillIconType.Display));
		this._samsaraElementControllerList.Add(new CharacterCombatSKillScroll(samsaraPageRefers.CGet<InfinityScrollLegacy>("CombatSkillScroll"), LifeOrCombatSkillScrollItemType.Both, ESkillIconType.Display));
		this._samsaraElementControllerList.ForEach(delegate(CharacterUIElement e)
		{
			e.SetIsDead(true);
		});
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x000C63C0 File Offset: 0x000C45C0
	private void UpdateCurrentPreLife(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		int charId = -1;
		CommonCharacterToggle commonCharacterToggle;
		bool flag;
		if (null != newTog)
		{
			commonCharacterToggle = (newTog as CommonCharacterToggle);
			flag = (commonCharacterToggle != null);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			charId = commonCharacterToggle.CharacterId;
		}
		bool isRanchenzi = this._samsaraInfoMonitor.IsRanchenzi;
		if (isRanchenzi)
		{
			this.RefreshAsRanchenziSamsaraInfo(charId);
			CToggleObsolete toggle = base.CGet<Refers>("TabSamsara").CGet<CToggleGroupObsolete>("SamsaraDataGroup").GetActive();
			bool flag3 = null == toggle;
			if (flag3)
			{
				base.CGet<Refers>("TabSamsara").CGet<CToggleGroupObsolete>("SamsaraDataGroup").Set(0, true, false);
			}
			else
			{
				this.OnSamsaraDataToggleGroupChange(toggle, null);
			}
		}
		else
		{
			this._samsaraElementControllerList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = charId;
			});
		}
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x000C6490 File Offset: 0x000C4690
	private unsafe void OnGetCharacterSamsara()
	{
		Refers samsaraPageRefers = base.CGet<Refers>("TabSamsara");
		bool isRanchenzi = this._samsaraInfoMonitor.IsRanchenzi;
		if (isRanchenzi)
		{
			this.RefreshAsRanchenziSamsaraTable();
			samsaraPageRefers.CGet<CToggleGroupObsolete>("SamsaraDataGroup").Get(0).isOn = false;
			samsaraPageRefers.CGet<CToggleGroupObsolete>("SamsaraDataGroup").Set(0, true, false);
		}
		else
		{
			CToggleGroupObsolete tableGroup = samsaraPageRefers.CGet<CToggleGroupObsolete>("CharacterTable");
			CToggleGroupObsolete samsaraDataGroup = samsaraPageRefers.CGet<CToggleGroupObsolete>("SamsaraDataGroup");
			tableGroup.GetAllActive().Clear();
			int needAutoOnIndex = -1;
			PreexistenceCharIds preexistenceCharIds = this._samsaraInfoMonitor.PreexistenceCharIds;
			for (int i = 0; i < this._preLifeTableController.Count; i++)
			{
				CommonCharacterToggle controller = this._preLifeTableController[i];
				controller.enabled = true;
				DeadCharacter preLifeCharacter = this._samsaraInfoMonitor.PreLifeCharacters[i];
				bool hasPreLife = preLifeCharacter != null;
				tableGroup.SetInteractable(hasPreLife, i);
				tableGroup.Set(i, false, false);
				bool flag = !hasPreLife;
				if (flag)
				{
					controller.CharacterId = -1;
				}
				else
				{
					int index = preexistenceCharIds.GetIndexByPos(i);
					int charId = *(ref preexistenceCharIds.CharIds.FixedElementField + (IntPtr)index * 4);
					SingletonObject.getInstance<CharacterMonitorModel>().AddDeadCharacterCache(charId, preLifeCharacter);
					tableGroup.SetInteractable(true, i);
					controller.CharacterId = charId;
					bool flag2 = index == 0;
					if (flag2)
					{
						needAutoOnIndex = i;
					}
				}
			}
			bool flag3 = needAutoOnIndex >= 0;
			if (flag3)
			{
				CToggleObsolete toggle = tableGroup.Get(needAutoOnIndex);
				bool isOn = toggle.isOn;
				if (isOn)
				{
					tableGroup.OnActiveToggleChange(toggle, null);
				}
				else
				{
					tableGroup.Set(needAutoOnIndex, true, false);
				}
			}
			tableGroup.gameObject.SetActive(preexistenceCharIds.Count > 0);
			samsaraPageRefers.CGet<GameObject>("NoContent_Table").SetActive(preexistenceCharIds.Count <= 0);
			samsaraPageRefers.CGet<GameObject>("NoContent_Info").SetActive(preexistenceCharIds.Count <= 0);
			samsaraPageRefers.CGet<GameObject>("InvisiblePage").SetActive(false);
			bool flag4 = preexistenceCharIds.Count <= 0;
			if (flag4)
			{
				this.UpdateCurrentPreLife(null, null);
				samsaraDataGroup.SetInteractable(false, null);
				this.UpdateSamsaraTogLabel(samsaraDataGroup);
				List<CToggleObsolete> list = samsaraDataGroup.GetAll();
				foreach (CToggleObsolete cToggle in list)
				{
					cToggle.isOn = false;
				}
				samsaraDataGroup.GetAllActive().Clear();
				samsaraPageRefers.CGet<Refers>("AttributeTabPage").gameObject.SetActive(false);
				samsaraPageRefers.CGet<InfinityScrollLegacy>("FeatureScroll").gameObject.SetActive(false);
				samsaraPageRefers.CGet<InfinityScrollLegacy>("LifeSkillScroll").gameObject.SetActive(false);
				samsaraPageRefers.CGet<InfinityScrollLegacy>("CombatSkillScroll").gameObject.SetActive(false);
			}
			else
			{
				samsaraDataGroup.SetInteractable(true, null);
				samsaraDataGroup.Get(0).isOn = false;
				samsaraDataGroup.Set(0, true, false);
				this.UpdateSamsaraTogLabel(samsaraDataGroup);
			}
		}
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x000C67BC File Offset: 0x000C49BC
	private void OnSamsaraDataToggleGroupChange(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		Refers samsaraPageRefers = base.CGet<Refers>("TabSamsara");
		bool isRanchenzi = this._samsaraInfoMonitor.IsRanchenzi;
		if (isRanchenzi)
		{
			int index = samsaraPageRefers.CGet<CToggleGroupObsolete>("CharacterTable").GetActive().GetComponent<Refers>().UserInt;
			bool flag = !this.IsRanchenziSamsaraVisible(index);
			if (flag)
			{
				samsaraPageRefers.CGet<GameObject>("InvisiblePage").SetActive(true);
				samsaraPageRefers.CGet<Refers>("AttributeTabPage").gameObject.SetActive(false);
				samsaraPageRefers.CGet<InfinityScrollLegacy>("FeatureScroll").gameObject.SetActive(false);
				samsaraPageRefers.CGet<InfinityScrollLegacy>("LifeSkillScroll").gameObject.SetActive(false);
				samsaraPageRefers.CGet<InfinityScrollLegacy>("CombatSkillScroll").gameObject.SetActive(false);
				return;
			}
		}
		samsaraPageRefers.CGet<GameObject>("InvisiblePage").SetActive(false);
		samsaraPageRefers.CGet<Refers>("AttributeTabPage").gameObject.SetActive(newTog.Key == 0);
		samsaraPageRefers.CGet<InfinityScrollLegacy>("FeatureScroll").gameObject.SetActive(newTog.Key == 1);
		samsaraPageRefers.CGet<InfinityScrollLegacy>("LifeSkillScroll").gameObject.SetActive(newTog.Key == 2);
		samsaraPageRefers.CGet<InfinityScrollLegacy>("CombatSkillScroll").gameObject.SetActive(newTog.Key == 3);
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x000C6915 File Offset: 0x000C4B15
	private void UpdateSamsaraTogLabel(CToggleGroupObsolete togGroup)
	{
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x000C6918 File Offset: 0x000C4B18
	private void RefreshAsRanchenziSamsaraTable()
	{
		Refers samsaraPageRefers = base.CGet<Refers>("TabSamsara");
		CToggleGroupObsolete tableGroup = samsaraPageRefers.CGet<CToggleGroupObsolete>("CharacterTable");
		CToggleGroupObsolete samsaraDataGroup = samsaraPageRefers.CGet<CToggleGroupObsolete>("SamsaraDataGroup");
		tableGroup.gameObject.SetActive(true);
		samsaraPageRefers.CGet<GameObject>("NoContent_Table").SetActive(false);
		samsaraPageRefers.CGet<GameObject>("NoContent_Info").SetActive(false);
		List<CToggleObsolete> toggleList = tableGroup.GetAll();
		int setToIndex = -1;
		for (int i = 0; i < toggleList.Count; i++)
		{
			Refers refers = toggleList[i].GetComponent<Refers>();
			refers.UserInt = i;
			this._preLifeTableController[i].enabled = false;
			refers.CGet<GameObject>("CharacterInfo").SetActive(true);
			refers.CGet<TextMeshProUGUI>("Order").text = LocalStringManager.Get(CommonUtils.DigitLanguageKeys[i + 1]);
			refers.GetComponent<CImage>().SetSprite("sp_03_mh_touxiang_2", false, null);
			tableGroup.SetInteractable(true, i);
			CharacterItem config = Character.Instance.GetItem(this._ranChenziSamsaraCharacterTemplateIdList[i]);
			Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			bool flag = this.IsRanchenziSamsaraVisible(i);
			if (flag)
			{
				avatar.Refresh(null, config.TemplateId);
				refers.CGet<TextMeshProUGUI>("Name").text = config.Surname + config.GivenName;
				bool flag2 = setToIndex < 0;
				if (flag2)
				{
					setToIndex = i;
				}
				avatar.GetComponent<RectTransform>().anchoredPosition = Vector2.down * 30f;
			}
			else
			{
				AtlasInfo.Instance.GetSpriteWithoutPackerName("sp_01_shengmiren", delegate(Sprite sprite)
				{
					avatar.Refresh(sprite);
				});
				refers.CGet<TextMeshProUGUI>("Name").text = "? ? ?";
				avatar.GetComponent<RectTransform>().anchoredPosition = Vector2.down * 10f;
			}
		}
		samsaraDataGroup.SetInteractable(true, null);
		this.UpdateSamsaraTogLabel(samsaraDataGroup);
		setToIndex = Mathf.Max(0, setToIndex);
		tableGroup.Get(setToIndex).isOn = false;
		tableGroup.Set(setToIndex, true, false);
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x000C6B64 File Offset: 0x000C4D64
	private void RefreshAsRanchenziSamsaraInfo(int index)
	{
		Refers samsaraPageRefers = base.CGet<Refers>("TabSamsara");
		Refers attributeTabRefers = samsaraPageRefers.CGet<Refers>("AttributeTabPage");
		Game.Components.Avatar.Avatar avatar = attributeTabRefers.CGet<Game.Components.Avatar.Avatar>("Avatar");
		Refers charmRefers = attributeTabRefers.CGet<Refers>("CharacterCharm");
		Refers happinessRefers = attributeTabRefers.CGet<Refers>("CharacterHappiness");
		Refers behaviorRefers = attributeTabRefers.CGet<Refers>("CharacterBehavior");
		Refers samsaraRefers = attributeTabRefers.CGet<Refers>("CharacterSamsara");
		Refers organizationRefers = attributeTabRefers.CGet<Refers>("CharacterOrganization");
		Refers identityRefers = attributeTabRefers.CGet<Refers>("CharacterIdentity");
		Refers nameRefers = attributeTabRefers.CGet<Refers>("CharacterName");
		TextMeshProUGUI deadDateLabel = attributeTabRefers.CGet<TextMeshProUGUI>("DeadDate");
		charmRefers.CGet<TextMeshProUGUI>("InfoName").text = CharacterPropertyDisplay.Instance[101].Name;
		happinessRefers.CGet<TextMeshProUGUI>("InfoName").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness);
		behaviorRefers.CGet<TextMeshProUGUI>("InfoName").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior);
		samsaraRefers.CGet<TextMeshProUGUI>("InfoName").text = LocalStringManager.Get(LanguageKey.LK_Samsara);
		organizationRefers.CGet<TextMeshProUGUI>("InfoName").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization);
		identityRefers.CGet<TextMeshProUGUI>("InfoName").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Identity);
		InfinityScrollLegacy featureScroll = samsaraPageRefers.CGet<InfinityScrollLegacy>("FeatureScroll");
		InfinityScrollLegacy lifeSkillScroll = samsaraPageRefers.CGet<InfinityScrollLegacy>("LifeSkillScroll");
		InfinityScrollLegacy combatSkillScroll = samsaraPageRefers.CGet<InfinityScrollLegacy>("CombatSkillScroll");
		bool flag = !this.IsRanchenziSamsaraVisible(index);
		if (flag)
		{
			string resPath = "RemakeResources/Textures/NpcFace/NormalFace/NpcFace_ranchenzi";
			ResLoader.Load<Sprite>(resPath, new Action<Sprite>(avatar.Refresh), null, false);
			string unableToBeKnown = LocalStringManager.Get(LanguageKey.LK_UnableToBeKnown);
			nameRefers.CGet<TextMeshProUGUI>("Name").text = string.Empty;
			charmRefers.CGet<TextMeshProUGUI>("InfoValue").text = string.Empty;
			happinessRefers.CGet<TextMeshProUGUI>("InfoValue").text = string.Empty;
			behaviorRefers.CGet<TextMeshProUGUI>("InfoValue").text = string.Empty;
			samsaraRefers.CGet<TextMeshProUGUI>("InfoValue").text = string.Empty;
			organizationRefers.CGet<TextMeshProUGUI>("InfoValue").text = string.Empty;
			identityRefers.CGet<TextMeshProUGUI>("InfoValue").text = string.Empty;
			deadDateLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Died_AtAge, "? ? ?");
			featureScroll.UpdateData(0);
			lifeSkillScroll.UpdateData(0);
			combatSkillScroll.UpdateData(0);
		}
		else
		{
			CharacterItem config = Character.Instance[this._ranChenziSamsaraCharacterTemplateIdList[index]];
			avatar.Refresh(null, config.TemplateId);
			nameRefers.CGet<TextMeshProUGUI>("Name").text = config.Surname + config.GivenName;
			charmRefers.CGet<TextMeshProUGUI>("InfoValue").text = CommonUtils.GetCharmLevelText(config.BaseAttraction, config.Gender, config.ActualAge, 0, CreatingType.IsFixedPresetType(config.CreatingType), true);
			happinessRefers.CGet<TextMeshProUGUI>("InfoValue").text = CommonUtils.GetHappinessString(HappinessType.GetHappinessType(config.Happiness));
			behaviorRefers.CGet<TextMeshProUGUI>("InfoValue").text = Config.BehaviorType.Instance[(short)GameData.Domains.Character.BehaviorType.GetBehaviorType(config.BaseMorality)].Name;
			samsaraRefers.CGet<TextMeshProUGUI>("InfoValue").text = LocalStringManager.GetFormat(LanguageKey.LK_SamsaraCount, index + 1);
			organizationRefers.CGet<TextMeshProUGUI>("InfoValue").text = Organization.Instance[config.OrganizationInfo.OrgTemplateId].Name;
			identityRefers.CGet<TextMeshProUGUI>("InfoValue").text = OrganizationMember.Instance[Organization.Instance[config.OrganizationInfo.OrgTemplateId].Members[(int)config.OrganizationInfo.Grade]].GradeName;
			bool hideAge = config.HideAge;
			if (hideAge)
			{
				deadDateLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Died_AtAge, "? ? ?");
			}
			else
			{
				deadDateLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Died_AtAge, config.ActualAge);
			}
			this._samsaraFeatureScroll.SetShowFeatureListFromOutside(config.FeatureIds);
			lifeSkillScroll.OnItemRender = new Action<int, Refers>(this.OnRanchenziSamsaraLifeSkillItemRender);
			lifeSkillScroll.UpdateData(16);
			combatSkillScroll.OnItemRender = new Action<int, Refers>(this.OnRanchenziSamsaraCombatSkillItemRender);
			combatSkillScroll.UpdateData(14);
		}
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x000C6FE8 File Offset: 0x000C51E8
	private void OnRanchenziSamsaraLifeSkillItemRender(int index, Refers refers)
	{
		int characterIndex = base.CGet<Refers>("TabSamsara").CGet<CToggleGroupObsolete>("CharacterTable").GetActive().GetComponent<Refers>().UserInt;
		CharacterItem characterConfig = Character.Instance[this._ranChenziSamsaraCharacterTemplateIdList[characterIndex]];
		LifeSkillTypeItem config = Config.LifeSkillType.Instance[index];
		refers.CGet<TextMeshProUGUI>("SkillName").text = config.Name;
		refers.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
		refers.CGet<TextMeshProUGUI>("Qualification").text = (ref characterConfig.BaseLifeSkillQualifications.Items.FixedElementField + (IntPtr)index * 2).ToString();
		refers.CGet<TextMeshProUGUI>("Attainment").text = "???";
		TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
		bool flag = null != mouseTip;
		if (flag)
		{
			mouseTip.Type = TipType.Simple;
			mouseTip.enabled = true;
			mouseTip.IsLanguageKey = false;
			mouseTip.PresetParam = new string[2];
			mouseTip.PresetParam[0] = config.Name;
			mouseTip.PresetParam[1] = config.Desc;
		}
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x000C7108 File Offset: 0x000C5308
	private void OnRanchenziSamsaraCombatSkillItemRender(int index, Refers refers)
	{
		int characterIndex = base.CGet<Refers>("TabSamsara").CGet<CToggleGroupObsolete>("CharacterTable").GetActive().GetComponent<Refers>().UserInt;
		CharacterItem characterConfig = Character.Instance[this._ranChenziSamsaraCharacterTemplateIdList[characterIndex]];
		CombatSkillTypeItem config = CombatSkillType.Instance[index];
		refers.CGet<TextMeshProUGUI>("SkillName").text = config.Name;
		refers.CGet<CImage>("SkillIcon").SetSprite(config.DisplayIcon, false, null);
		refers.CGet<TextMeshProUGUI>("Qualification").text = (ref characterConfig.BaseLifeSkillQualifications.Items.FixedElementField + (IntPtr)index * 2).ToString();
		refers.CGet<TextMeshProUGUI>("Attainment").text = "???";
		TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
		bool flag = null != mouseTip;
		if (flag)
		{
			mouseTip.Type = TipType.Simple;
			mouseTip.enabled = true;
			mouseTip.IsLanguageKey = false;
			mouseTip.PresetParam = new string[2];
			mouseTip.PresetParam[0] = config.Name;
			mouseTip.PresetParam[1] = config.Desc;
		}
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x000C7228 File Offset: 0x000C5428
	private bool IsRanchenziSamsaraVisible(int index)
	{
		return SingletonObject.getInstance<BasicGameData>().XiangshuAvatarTaskStatusArray[index].JuniorXiangshuTaskStatus != 0;
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x000C7254 File Offset: 0x000C5454
	private void SetMouseTipBodyPartParams(Refers refers)
	{
		bool flag = this._injuryGroupController.GetMonitor<InjuryPoisonMonitor>() == null;
		if (!flag)
		{
			TooltipInvoker mouseTipDisPlayer = refers.CGet<GameObject>("Back").GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = mouseTipDisPlayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			mouseTipDisPlayer.RuntimeParam.Clear();
			string text = refers.CGet<TextMeshProUGUI>("Name").text;
			mouseTipDisPlayer.RuntimeParam.Set("Title", text);
			mouseTipDisPlayer.RuntimeParam.Set("Type", refers.transform.name);
			mouseTipDisPlayer.RuntimeParam.SetObject("Injury", this._injuryGroupController.GetMonitor<InjuryPoisonMonitor>().Injuries);
			mouseTipDisPlayer.RuntimeParam.SetObject("CompleteDamageStepDisplayData", this._completeDamageStepDisplayData);
			mouseTipDisPlayer.RuntimeParam.SetObject("AllBodyPartExists", this._allBodyPartExists);
			mouseTipDisPlayer.RuntimeParam.Set("CharacterId", this._characterId);
		}
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x000C735C File Offset: 0x000C555C
	public void RefreshAllHealBtn(int charId, bool disableAll = false)
	{
		AsyncMethodCallbackDelegate <>9__1;
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool pool)
		{
			CharacterDisplayData characterDisplayData = null;
			Serializer.Deserialize(pool, offset, ref characterDisplayData);
			CharacterMonitorModel characterMonitorModel = SingletonObject.getInstance<CharacterMonitorModel>();
			bool isCanHealTeammate = characterMonitorModel.IsTaiwuTeamCharacter(charId) || characterMonitorModel.IsTaiwuSpecialTeammate(charId);
			bool disable = UIManager.Instance.IsElementActive(UIElement.Combat) || !isCanHealTeammate || this.CharacterMenuCanNotOperate || characterDisplayData.TemplateId == 880;
			bool disableAll2 = disableAll;
			if (disableAll2)
			{
				this.EnableHealButton(false);
			}
			else
			{
				this.EnableHealButton(!disable);
			}
			bool isOnUsingMedicineItem = UIManager.Instance.IsElementActive(UIElement.UsingMedicineItem);
			bool flag = disable || isOnUsingMedicineItem || characterMonitorModel.IsTaiwuGearMate(charId);
			if (flag)
			{
				this._canUseMedicineItem = false;
				this.EnableUsingMedicineButton(this._canUseMedicineItem);
			}
			else
			{
				IAsyncMethodRequestHandler requestHandler = null;
				int charId2 = charId;
				sbyte itemType = 8;
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(int offset, RawDataPool dataPool)
					{
						List<ItemDisplayData> medicineItemDisplayDatas = null;
						Serializer.Deserialize(dataPool, offset, ref medicineItemDisplayDatas);
						this._canUseMedicineItem = (!disableAll && medicineItemDisplayDatas != null && medicineItemDisplayDatas.Count > 0);
						this.EnableUsingMedicineButton(this._canUseMedicineItem);
					});
				}
				CharacterDomainMethod.AsyncCall.GetInventoryItemsByItemType(requestHandler, charId2, itemType, callback);
			}
		});
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x000C73A0 File Offset: 0x000C55A0
	private void EnableHealButton(bool isEnabled)
	{
		CButtonObsolete healButton = base.CGet<Refers>("TabInjury").CGet<CButtonObsolete>("HealInteractButton");
		healButton.interactable = isEnabled;
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x000C73CC File Offset: 0x000C55CC
	private void EnableUsingMedicineButton(bool isEnabled)
	{
		base.CGet<Refers>("TabInjury").CGet<CButtonObsolete>("UsingMedicineInteractButton").interactable = isEnabled;
		this.RefreshInjuryAndPoison(isEnabled);
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x000C73F3 File Offset: 0x000C55F3
	private void OnPoisonChanged()
	{
		this.RefreshInjuryAndPoison(this._canUseMedicineItem);
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x000C7404 File Offset: 0x000C5604
	private void RefreshInjuryAndPoison(bool isEnabled)
	{
		CharacterAttributeDataView.<>c__DisplayClass126_0 CS$<>8__locals1 = new CharacterAttributeDataView.<>c__DisplayClass126_0();
		CS$<>8__locals1.isEnabled = isEnabled;
		CS$<>8__locals1.<>4__this = this;
		bool isSelectInjuryPart = this.IsSelectInjuryPart;
		if (!isSelectInjuryPart)
		{
			CharacterInjuryGroup injuryGroupController = this._injuryGroupController;
			InjuryPoisonMonitor injuryMonitor = (injuryGroupController != null) ? injuryGroupController.GetMonitor<InjuryPoisonMonitor>() : null;
			CharacterPoisonGroup poisonGroupController = this._poisonGroupController;
			InjuryPoisonMonitor poisonMonitor = ((poisonGroupController != null) ? poisonGroupController.GetMonitorItem(this._characterId) : null) as InjuryPoisonMonitor;
			CharacterHealth healthUIElement = this._healthUIElement;
			AgeHealthMonitor ageHealthMonitor = (healthUIElement != null) ? healthUIElement.GetMonitor<AgeHealthMonitor>() : null;
			CharacterDisorderOfQi disorderOfQiController = this._disorderOfQiController;
			DisorderOfQiMonitor qiMonitor = (disorderOfQiController != null) ? disorderOfQiController.GetMonitor<DisorderOfQiMonitor>() : null;
			bool flag = injuryMonitor == null || poisonMonitor == null || ageHealthMonitor == null || qiMonitor == null;
			if (!flag)
			{
				bool flag2 = this._injuryParts == null || this._poisonTypes == null;
				if (!flag2)
				{
					CS$<>8__locals1.isUsingMedicineItemPage = UIElement.UsingMedicineItem.IsShowing;
					bool isUsingMedicineItemPage = CS$<>8__locals1.isUsingMedicineItemPage;
					if (isUsingMedicineItemPage)
					{
						this._poisonGroupController.OnPoisonChanged = new Action(this.OnPoisonChanged);
					}
					sbyte i = 0;
					while ((int)i < this._injuryParts.Length)
					{
						Refers part = this._injuryParts[(int)i];
						ValueTuple<sbyte, sbyte> injury = injuryMonitor.Injuries.Get(i);
						CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(part, (short)i + UsingMedicineItemType.BodyPartTypeChest, injury.Item1 > 0 || injury.Item2 > 0);
						i += 1;
					}
					sbyte j = 0;
					while ((int)j < this._poisonTypes.Length)
					{
						Refers part2 = this._poisonTypes[(int)j];
						CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(part2, (short)j + UsingMedicineItemType.PoisonTypeHot, poisonMonitor.Poisons[(int)j] > 0);
						j += 1;
					}
					Refers healthRefers = base.CGet<Refers>("TabInjury").CGet<CharacterHealthBar>("CharacterHealthInfo").GetComponent<Refers>();
					bool healthNotGood = ageHealthMonitor.Health < ageHealthMonitor.LeftMaxHealth;
					CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(healthRefers, UsingMedicineItemType.Health, healthNotGood);
					Refers qiRefers = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect");
					CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(qiRefers, UsingMedicineItemType.DisorderOfQi, qiMonitor.DisorderOfQi > 0);
				}
			}
		}
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x000C7616 File Offset: 0x000C5816
	public void EnterBreakThoughUI()
	{
		this._inBreakUI = true;
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x000C7620 File Offset: 0x000C5820
	public void LeaveBreakThoughUI()
	{
		this._inBreakUI = false;
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000C762C File Offset: 0x000C582C
	public void OnClickShowHealUI()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
		List<int> teamCharList = monitor.GetTaiwuTeamCharIds();
		teamCharList.AddRange(monitor.GetTaiwuSpecialGroup());
		argBox.SetObject("DoctorList", teamCharList);
		List<int> patientList = new List<int>();
		patientList.AddRange(teamCharList);
		argBox.SetObject("PatientList", patientList);
		argBox.Set("NeedPay", false);
		argBox.Set("CurrentCharacterId", this._characterId);
		CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			KidnappedCharacterList kidnappedCharacterList = null;
			Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
			bool flag = kidnappedCharacterList != null;
			if (flag)
			{
				for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
				{
					patientList.Add(kidnappedCharacterList.Get(i).CharId);
				}
			}
			UIElement.Heal.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.Heal, true);
		});
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x000C76EC File Offset: 0x000C58EC
	public void OnClickShowMedicineItem(short type)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CurrentCharacterId", this._characterId).Set("UsingMedicineItemType", type).SetObject("LastPoisonValue", this.GetInjuryPoisonMonitor().Poisons);
		UIElement.UsingMedicineItem.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.UsingMedicineItem, true);
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x000C774F File Offset: 0x000C594F
	public void UsingMedicineItemSwitch(short type)
	{
		GEvent.OnEvent(UiEvents.UsingMedicineItemSwitch, EasyPool.Get<ArgumentBox>().Set("UsingMedicineItemType", type));
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x000C7770 File Offset: 0x000C5970
	public InjuryPoisonMonitor GetInjuryPoisonMonitor()
	{
		return this._injuryGroupController.GetMonitor<InjuryPoisonMonitor>();
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x000C7790 File Offset: 0x000C5990
	public DisorderOfQiMonitor GetDisorderOfQiMonitor()
	{
		CharacterDisorderOfQi disorderOfQiController = this._disorderOfQiController;
		return (disorderOfQiController != null) ? disorderOfQiController.GetMonitor<DisorderOfQiMonitor>() : null;
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x000C77B4 File Offset: 0x000C59B4
	public AgeHealthMonitor GetAgeHealthMonitor()
	{
		CharacterHealth healthUIElement = this._healthUIElement;
		return (healthUIElement != null) ? healthUIElement.GetMonitor<AgeHealthMonitor>() : null;
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x000C77D8 File Offset: 0x000C59D8
	public void UnselectPartByUsingMedicineItemType(short type)
	{
		Refers refers = this.GetPartRefersByUsingMedicineItemType(type);
		if (refers != null)
		{
			List<GameObject> list = refers.CGetList<GameObject>("SelectedIcon");
			if (list != null)
			{
				list.ForEach(delegate(GameObject r)
				{
					r.SetActive(false);
				});
			}
		}
		this._usingMedicineItemType = UsingMedicineItemType.Invalid;
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x000C7834 File Offset: 0x000C5A34
	public void SelectPartByUsingMedicineItemType(short type)
	{
		bool flag = this._usingMedicineItemType > UsingMedicineItemType.Invalid;
		if (flag)
		{
			this.UnselectPartByUsingMedicineItemType(this._usingMedicineItemType);
		}
		Refers refers = this.GetPartRefersByUsingMedicineItemType(type);
		if (refers != null)
		{
			List<GameObject> list = refers.CGetList<GameObject>("SelectedIcon");
			if (list != null)
			{
				list.ForEach(delegate(GameObject r)
				{
					r.SetActive(true);
				});
			}
		}
		this._usingMedicineItemType = type;
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x000C78AC File Offset: 0x000C5AAC
	private Refers GetPartRefersByUsingMedicineItemType(short type)
	{
		bool flag = type == UsingMedicineItemType.Invalid;
		Refers result;
		if (flag)
		{
			result = null;
		}
		else
		{
			sbyte i = 0;
			while ((int)i < this._injuryParts.Length)
			{
				bool flag2 = type - UsingMedicineItemType.BodyPartTypeChest == (short)i;
				if (flag2)
				{
					return this._injuryParts[(int)i];
				}
				i += 1;
			}
			sbyte j = 0;
			while ((int)j < this._poisonTypes.Length)
			{
				bool flag3 = type - UsingMedicineItemType.PoisonTypeHot == (short)j;
				if (flag3)
				{
					return this._poisonTypes[(int)j];
				}
				j += 1;
			}
			bool flag4 = type == UsingMedicineItemType.Health;
			if (flag4)
			{
				result = base.CGet<Refers>("TabInjury").CGet<CharacterHealthBar>("CharacterHealthInfo").GetComponent<Refers>();
			}
			else
			{
				bool flag5 = type == UsingMedicineItemType.DisorderOfQi;
				if (flag5)
				{
					result = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect");
				}
				else
				{
					result = null;
				}
			}
		}
		return result;
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x000C799C File Offset: 0x000C5B9C
	private void HighLight(GameObject target)
	{
		bool flag = null == target;
		if (!flag)
		{
			this._focusingTuple.Item1 = target;
			this._focusingTuple.Item2 = target.transform.parent;
			this._focusingTuple.Item3 = target.transform.GetSiblingIndex();
			GameObject focusMask = base.CGet<GameObject>("FocusMask");
			focusMask.gameObject.SetActive(true);
			target.transform.SetParent(focusMask.transform, true);
			target.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x000C7A2C File Offset: 0x000C5C2C
	private void CancelHighLight()
	{
		bool flag = null == this._focusingTuple.Item1;
		if (!flag)
		{
			this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
			this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
			this._focusingTuple.Item1 = null;
			base.CGet<GameObject>("FocusMask").SetActive(false);
		}
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x000C7C1C File Offset: 0x000C5E1C
	[CompilerGenerated]
	internal static void <InitCombatDifficultyTips>g__SetDifficultyRoot|75_0(RectTransform root, int changeValue, LanguageKey typeLangId, ref CharacterAttributeDataView.<>c__DisplayClass75_0 A_3)
	{
		string typeName = LocalStringManager.Get(typeLangId);
		TooltipInvoker upTips = root.Find("Up").GetComponent<TooltipInvoker>();
		TooltipInvoker downTips = root.Find("Down").GetComponent<TooltipInvoker>();
		upTips.gameObject.SetActive((changeValue > 0 & A_3.isValidCharacter) && !A_3.isTaiwu && !A_3.isTaiwuGroupChar);
		downTips.gameObject.SetActive((changeValue < 0 & A_3.isValidCharacter) && !A_3.isTaiwu && !A_3.isTaiwuGroupChar);
		bool flag = (Mathf.Abs(changeValue) != 0 & A_3.isValidCharacter) && !A_3.isTaiwu;
		if (flag)
		{
			string[] tipsData = new string[2];
			tipsData[0] = LocalStringManager.Get(LanguageKey.LK_CombatDifficultyInfluence);
			bool flag2 = changeValue > 0;
			if (flag2)
			{
				tipsData[1] = LocalStringManager.GetFormat(LanguageKey.LK_CombatDifficultyInfluence_MergedEffect, typeName, "lightblue", "+" + changeValue.ToString());
			}
			else
			{
				tipsData[1] = LocalStringManager.GetFormat(LanguageKey.LK_CombatDifficultyInfluence_MergedEffect, typeName, "brightred", changeValue);
			}
			upTips.PresetParam = tipsData;
			downTips.PresetParam = tipsData;
		}
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x000C7D44 File Offset: 0x000C5F44
	[CompilerGenerated]
	internal static void <InitCombatDifficultyTips>g__AddChangeInfo|75_1(short templateId, int changeValue, ref CharacterAttributeDataView.<>c__DisplayClass75_0 A_2)
	{
		bool flag = changeValue == 0;
		if (!flag)
		{
			CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance.GetItem(templateId);
			bool flag2 = propertyConfig == null;
			if (!flag2)
			{
				bool flag3 = changeValue > 0;
				if (flag3)
				{
					A_2.cacheList.Add(string.Format("·<SpName={0}>{1}: <color=#{2}>+{3}%</color>", new object[]
					{
						propertyConfig.TipsIcon,
						propertyConfig.Name,
						"brightblue",
						changeValue
					}).SetColor("pinkyellow"));
				}
				else
				{
					A_2.cacheList.Add(string.Format("·<SpName={0}>{1}: <color=#{2}>{3}%</color>", new object[]
					{
						propertyConfig.TipsIcon,
						propertyConfig.Name,
						"brightred",
						changeValue
					}).SetColor("pinkyellow"));
				}
			}
		}
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x000C7E14 File Offset: 0x000C6014
	[CompilerGenerated]
	internal static void <ShowInfectNotice>g__SetNoticeShow|80_1(GameObject obj, int value)
	{
		obj.SetActive(value != 0);
	}

	// Token: 0x040015FB RID: 5627
	private int _listenerId;

	// Token: 0x040015FC RID: 5628
	private int _characterId;

	// Token: 0x040015FD RID: 5629
	public bool IsTaiwuTeam;

	// Token: 0x040015FE RID: 5630
	public static readonly string[] WugSkeletonNames = new string[]
	{
		"Wug1",
		"Wug2",
		"Wug1",
		"Wug2",
		"WugGrown",
		"WugKing"
	};

	// Token: 0x040015FF RID: 5631
	public static readonly string[] WugSkeletonSlotOrAttachmentNames = new string[]
	{
		"tiny",
		"mid",
		"tiny",
		"mid",
		"big",
		"king"
	};

	// Token: 0x04001600 RID: 5632
	public const string WugSkeletonSlotTemplate = "images/{0}";

	// Token: 0x04001601 RID: 5633
	public const string WugSkeletonAttachmentTemplate = "images/{0}_{1}";

	// Token: 0x04001602 RID: 5634
	private Vector2? _originalPos;

	// Token: 0x04001603 RID: 5635
	public bool LockTab;

	// Token: 0x04001604 RID: 5636
	public static sbyte CurTabIndex;

	// Token: 0x04001605 RID: 5637
	private sbyte _storedTabIndex = -1;

	// Token: 0x04001606 RID: 5638
	private CharacterMajorAttribute _majorAttributeController;

	// Token: 0x04001607 RID: 5639
	private CharacterSecondaryAttribute _secondaryAttributeController;

	// Token: 0x04001608 RID: 5640
	private EatingItemMonitor _eatingItemMonitor;

	// Token: 0x04001609 RID: 5641
	private CharacterInjuryGroup _injuryGroupController;

	// Token: 0x0400160A RID: 5642
	private CharacterPoisonGroup _poisonGroupController;

	// Token: 0x0400160B RID: 5643
	private CharacterDisorderOfQi _disorderOfQiController;

	// Token: 0x0400160C RID: 5644
	private CombatCharacterDisplayData _combatCharDisplayData;

	// Token: 0x0400160D RID: 5645
	private CharacterHealth _healthUIElement;

	// Token: 0x0400160E RID: 5646
	private SamsaraMonitor _samsaraInfoMonitor;

	// Token: 0x0400160F RID: 5647
	private List<CommonCharacterToggle> _preLifeTableController;

	// Token: 0x04001610 RID: 5648
	private List<CharacterUIElement> _samsaraElementControllerList;

	// Token: 0x04001611 RID: 5649
	private List<short> _ranChenziSamsaraCharacterTemplateIdList = new List<short>
	{
		47,
		56,
		65,
		74,
		83,
		92,
		101,
		110,
		119
	};

	// Token: 0x04001612 RID: 5650
	private Refers[] _injuryParts;

	// Token: 0x04001613 RID: 5651
	private GameObject[] _newlyInnerInjuryParts;

	// Token: 0x04001614 RID: 5652
	private GameObject[] _newlyOuterInjuryParts;

	// Token: 0x04001615 RID: 5653
	private GameObject _outerTotalInjuryNotice;

	// Token: 0x04001616 RID: 5654
	private GameObject _innerTotalInjuryNotice;

	// Token: 0x04001617 RID: 5655
	private Refers[] _poisonTypes;

	// Token: 0x04001618 RID: 5656
	private CImage _recoverChangeAddMin;

	// Token: 0x04001619 RID: 5657
	private CImage _recoverChangeAddMax;

	// Token: 0x0400161A RID: 5658
	private CImage _recoverChangeReduceMin;

	// Token: 0x0400161B RID: 5659
	private CImage _recoverChangeReduceMax;

	// Token: 0x0400161C RID: 5660
	private CImage _qiProgress;

	// Token: 0x0400161D RID: 5661
	private bool _canUseMedicineItem;

	// Token: 0x0400161E RID: 5662
	private CompleteDamageStepDisplayData _completeDamageStepDisplayData = new CompleteDamageStepDisplayData();

	// Token: 0x0400161F RID: 5663
	private List<bool> _allBodyPartExists = new List<bool>();

	// Token: 0x04001620 RID: 5664
	private DOTweenAnimation _bodyAnimation;

	// Token: 0x04001621 RID: 5665
	private bool _showingInfectNotice;

	// Token: 0x04001622 RID: 5666
	private short _usingMedicineItemType = UsingMedicineItemType.Invalid;

	// Token: 0x04001623 RID: 5667
	[NonSerialized]
	public int[] LastPoisonValueArray;

	// Token: 0x04001624 RID: 5668
	[TupleElementNames(new string[]
	{
		"target",
		"parent",
		"sibling"
	})]
	private ValueTuple<GameObject, Transform, int> _focusingTuple;

	// Token: 0x04001625 RID: 5669
	private bool _inBreakUI = false;

	// Token: 0x04001626 RID: 5670
	private int _curCanSelectInjuryPartCount;

	// Token: 0x04001627 RID: 5671
	private int _totalCanSelectInjuryPartCount;

	// Token: 0x04001628 RID: 5672
	private readonly List<sbyte> _selectedInjuryPartList = new List<sbyte>();

	// Token: 0x04001629 RID: 5673
	private ItemDisplayData _selectedInjuryPartUsingMedicine;

	// Token: 0x0400162A RID: 5674
	private Action _onCancelSelectInjuryPart;

	// Token: 0x0400162B RID: 5675
	private CharacterFeatureScroll _samsaraFeatureScroll;

	// Token: 0x0400162C RID: 5676
	private Refers[] _injuryGroupRefers;
}
