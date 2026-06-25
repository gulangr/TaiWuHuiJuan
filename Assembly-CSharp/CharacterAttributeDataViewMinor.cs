using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using FrameWork;
using Game.Views.Combat;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class CharacterAttributeDataViewMinor : Refers
{
	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001C92 RID: 7314 RVA: 0x000C7EF9 File Offset: 0x000C60F9
	private Refers _tabInjury
	{
		get
		{
			return base.CGet<Refers>("TabInjury");
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001C93 RID: 7315 RVA: 0x000C7F06 File Offset: 0x000C6106
	private Refers _totalHolder
	{
		get
		{
			return base.CGet<Refers>("Total");
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001C94 RID: 7316 RVA: 0x000C7F13 File Offset: 0x000C6113
	private bool IsTaiwuGearMate
	{
		get
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
		}
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x000C7F25 File Offset: 0x000C6125
	private void Awake()
	{
		this.InitElements();
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x000C7F30 File Offset: 0x000C6130
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

	// Token: 0x06001C97 RID: 7319 RVA: 0x000C7F84 File Offset: 0x000C6184
	public void InitElements()
	{
		bool flag = this.inited;
		if (!flag)
		{
			this.InitInjuryPage();
			this.InitHealthInfo();
			this.inited = true;
		}
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x000C7FB4 File Offset: 0x000C61B4
	public void SetEnable()
	{
		this._injuryPoisonMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<InjuryPoisonMonitor>(this._characterId, false);
		this._injuryPoisonMonitor.AddInjuriesListener(new Action(this.RefreshInjuryAndPoison));
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x000C8008 File Offset: 0x000C6208
	public void SetDisable()
	{
		this._injuryPoisonMonitor.RemoveInjuriesListener(new Action(this.RefreshInjuryAndPoison));
		this.ReleaseCharacter();
		this.HideEatDropNotice(true);
		this._storedTabIndex = -1;
		GameDataBridge.UnregisterListener(this._listenerId);
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x000C8048 File Offset: 0x000C6248
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
					this.ForceRefreshPageData();
				}
				else
				{
					bool flag3 = notification.DomainId == 4 && notification.MethodId == 174;
					if (flag3)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._allBodyPartExists);
					}
					else
					{
						bool flag4 = notification.DomainId == 8 && notification.MethodId == 63;
						if (flag4)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._completeDamageStepDisplayData);
							this.ForceRefreshPageData();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x000C816C File Offset: 0x000C636C
	public void SetCurrentCharacterId(int charId)
	{
		bool flag = -1 != charId;
		if (flag)
		{
			bool flag2 = UIElement.Combat.Exist && UIElement.Combat.UiBaseAs<ViewCombat>().IsCharInCombat(charId);
			if (flag2)
			{
				CombatDomainMethod.Call.GetCombatCharDisplayData(this._listenerId, charId);
			}
			this._characterId = charId;
			this._disorderOfQiController.CharacterId = charId;
			this._healthUIElement.CharacterId = charId;
			this._healthUIElement.GearMateMode = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
			bool flag3 = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			if (flag3)
			{
				this.ForceRefreshPageData();
			}
			Refers totalRefersRoot = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total");
			bool flag4 = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(charId);
			if (flag4)
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
			CharacterDomainMethod.Call.GetCharacterAllBodyPartExists(this._listenerId, charId);
			CombatDomainMethod.Call.GetCompleteDamageStepDisplayData(this._listenerId, charId);
		}
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x000C8424 File Offset: 0x000C6624
	public void ReleaseCharacter()
	{
		this._disorderOfQiController.CharacterId = -1;
		this._healthUIElement.CharacterId = -1;
		this._characterId = -1;
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x000C8448 File Offset: 0x000C6648
	private void BackToPrevState()
	{
		bool flag = CharacterAttributeDataViewMinor.CurTabIndex == this._storedTabIndex;
		if (flag)
		{
			this._storedTabIndex = -1;
		}
		bool flag2 = this._storedTabIndex < 0;
		if (!flag2)
		{
			this._storedTabIndex = -1;
		}
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x000C8486 File Offset: 0x000C6686
	public void ForceRefreshPageData()
	{
		this._disorderOfQiController.Refresh();
		this._healthUIElement.Refresh();
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x000C84A4 File Offset: 0x000C66A4
	public void HideEatDropNotice(bool backToPrevState = true)
	{
		if (backToPrevState)
		{
			this.BackToPrevState();
		}
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x000C84C0 File Offset: 0x000C66C0
	private void InitInjuryPage()
	{
		Refers rootRefers = base.CGet<Refers>("TabInjury");
		Refers refers = base.CGet<Refers>("TabInjury");
		TextMeshProUGUI label = rootRefers.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total").CGet<TextMeshProUGUI>("PoisonValue");
		Refers disorderOfQiRefers = rootRefers.CGet<Refers>("AreaPeriodEffect");
		CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
		TextMeshProUGUI stateLabel = disorderOfQiRefers.CGet<TextMeshProUGUI>("QiStateText");
		CImage stateIcon = disorderOfQiRefers.CGet<CImage>("QiStateIcon");
		QiContainer qiContainer = disorderOfQiRefers.CGet<QiContainer>("QiContainer");
		TooltipInvoker mouseTip = disorderOfQiRefers.CGet<TooltipInvoker>("QiMouseTip");
		this._disorderOfQiController = new CharacterDisorderOfQi(slider, stateLabel, stateIcon, qiContainer, mouseTip);
		this._disorderOfQiController.OnFillDisorderOfQi = new Action<float>(this.OnFillDisorderOfQi);
		this._disorderOfQiController.OnFillChangeOfDisorderOfQi = new Action<short>(this.OnFillChangeDisorderOfQiSlider);
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x000C8598 File Offset: 0x000C6798
	private void InitHealthInfo()
	{
		Refers refers = base.CGet<Refers>("TabInjury");
		this._healthUIElement = new CharacterHealth(refers.CGet<CharacterHealthBar>("CharacterHealthInfo"));
		this._healthUIElement.SetGetHealthStringFunc(new Func<short[], int, string>(this.GetHealthString));
		this._healthUIElement.OnFillHealthChange = new Action(this.OnFillHealthChange);
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x000C85F8 File Offset: 0x000C67F8
	private string GetHealthString(short[] paramsHealth, int characterId)
	{
		bool flag = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
		if (flag)
		{
			paramsHealth[0] = paramsHealth[1];
		}
		return CommonUtils.GetCharacterHealthInfo(paramsHealth[0], paramsHealth[1], characterId).Item1;
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x000C8638 File Offset: 0x000C6838
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

	// Token: 0x06001CA4 RID: 7332 RVA: 0x000C869C File Offset: 0x000C689C
	private void OnFillDisorderOfQi(float value)
	{
		CImage cImage = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect").CGet<CImage>("Image_Real_Progress");
		bool flag = cImage != null;
		if (flag)
		{
			cImage.fillAmount = value;
		}
		CharacterAttributeDataViewMinor.UpdateDisorderOfQiSliderHandle(base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect"), value);
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x000C86FC File Offset: 0x000C68FC
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

	// Token: 0x06001CA6 RID: 7334 RVA: 0x000C87CC File Offset: 0x000C69CC
	private void OnFillChangeDisorderOfQiSlider(short changeValue)
	{
		Refers rootRefers = base.CGet<Refers>("TabInjury");
		Refers disorderOfQiRefers = rootRefers.CGet<Refers>("AreaPeriodEffect");
		CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
		GameObject betterImg = disorderOfQiRefers.CGet<GameObject>("Image_RecoverBetter");
		GameObject worseImg = disorderOfQiRefers.CGet<GameObject>("Image_RecoverWorse");
		RectTransform qiChangeMask = disorderOfQiRefers.CGet<RectTransform>("QiChangeMask");
		float totalWidth = disorderOfQiRefers.CGet<CImage>("Image_Real_Progress").rectTransform.rect.width;
		betterImg.SetActive(changeValue <= 0);
		worseImg.SetActive(changeValue > 0);
		qiChangeMask.SetPivot(qiChangeMask.pivot.SetX((float)((changeValue > 0) ? 0 : 1)));
		qiChangeMask.anchoredPosition = qiChangeMask.anchoredPosition.SetX(0f);
		qiChangeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)Mathf.Abs((int)changeValue) / slider.maxValue * totalWidth);
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x000C88B0 File Offset: 0x000C6AB0
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

	// Token: 0x06001CA8 RID: 7336 RVA: 0x000C89AC File Offset: 0x000C6BAC
	private void RefreshInjuryAndPoison()
	{
		CharacterHealth healthUIElement = this._healthUIElement;
		AgeHealthMonitor ageHealthMonitor = (healthUIElement != null) ? healthUIElement.GetMonitor<AgeHealthMonitor>() : null;
		CharacterDisorderOfQi disorderOfQiController = this._disorderOfQiController;
		DisorderOfQiMonitor qiMonitor = (disorderOfQiController != null) ? disorderOfQiController.GetMonitor<DisorderOfQiMonitor>() : null;
		bool flag = this._injuryPoisonMonitor == null || ageHealthMonitor == null || qiMonitor == null;
		if (!flag)
		{
			ValueTuple<sbyte, sbyte> bothSum = this._injuryPoisonMonitor.Injuries.GetBothSum();
			sbyte totalOuter = bothSum.Item1;
			sbyte totalInner = bothSum.Item2;
			this.SetOuterInjury((int)totalOuter);
			this.SetInnerInjury((int)totalInner);
			int totalPosition = this.GetAllPoisonSum();
			this.SetTotalPosion(totalPosition);
			Refers healthRefers = base.CGet<Refers>("TabInjury").CGet<CharacterHealthBar>("CharacterHealthInfo").GetComponent<Refers>();
			bool healthNotGood = ageHealthMonitor.Health < ageHealthMonitor.LeftMaxHealth;
			Refers qiRefers = base.CGet<Refers>("TabInjury").CGet<Refers>("AreaPeriodEffect");
		}
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x000C8A80 File Offset: 0x000C6C80
	private int GetAllPoisonSum()
	{
		this._injuryPoisonMonitor.Poisons.Sum();
		int sum = 0;
		for (sbyte i = 0; i < 6; i += 1)
		{
			sum += (int)PoisonsAndLevels.CalcPoisonedLevel(this._injuryPoisonMonitor.Poisons[(int)i]);
		}
		return sum;
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x000C8ACE File Offset: 0x000C6CCE
	private void SetTotalPosion(int totalPosition)
	{
		this._totalHolder.CGet<TextMeshProUGUI>("PoisonValue").text = totalPosition.ToString().SetColor("poisoned");
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x000C8AF8 File Offset: 0x000C6CF8
	private void SetInnerInjury(int totalInner)
	{
		this._totalHolder.CGet<TextMeshProUGUI>("InnerValue").text = totalInner.ToString().SetColor((totalInner <= 0) ? "grey" : "innerinjury");
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x000C8B2D File Offset: 0x000C6D2D
	private void SetOuterInjury(int totalOuter)
	{
		this._totalHolder.CGet<TextMeshProUGUI>("OuterValue").text = totalOuter.ToString().SetColor((totalOuter <= 0) ? "grey" : "outterinjury");
	}

	// Token: 0x0400163D RID: 5693
	private InjuryPoisonMonitor _injuryPoisonMonitor;

	// Token: 0x0400163E RID: 5694
	private int _listenerId;

	// Token: 0x0400163F RID: 5695
	private int _characterId;

	// Token: 0x04001640 RID: 5696
	public static sbyte CurTabIndex;

	// Token: 0x04001641 RID: 5697
	private sbyte _storedTabIndex = -1;

	// Token: 0x04001642 RID: 5698
	private CharacterDisorderOfQi _disorderOfQiController;

	// Token: 0x04001643 RID: 5699
	private CombatCharacterDisplayData _combatCharDisplayData;

	// Token: 0x04001644 RID: 5700
	private CharacterHealth _healthUIElement;

	// Token: 0x04001645 RID: 5701
	private GameObject _outerTotalInjuryNotice;

	// Token: 0x04001646 RID: 5702
	private GameObject _innerTotalInjuryNotice;

	// Token: 0x04001647 RID: 5703
	private CImage _qiProgress;

	// Token: 0x04001648 RID: 5704
	private bool _canUseMedicineItem;

	// Token: 0x04001649 RID: 5705
	private CompleteDamageStepDisplayData _completeDamageStepDisplayData = new CompleteDamageStepDisplayData();

	// Token: 0x0400164A RID: 5706
	private List<bool> _allBodyPartExists = new List<bool>();

	// Token: 0x0400164B RID: 5707
	private bool _showingInfectNotice;

	// Token: 0x0400164C RID: 5708
	private bool inited = false;
}
