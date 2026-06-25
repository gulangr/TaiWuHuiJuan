using System;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class UI_FollowingInjuryHelper : Refers
{
	// Token: 0x06002116 RID: 8470 RVA: 0x000F0BE8 File Offset: 0x000EEDE8
	public void Init(UIBase parent)
	{
		this._parent = parent;
		bool flag = !this._inited;
		if (flag)
		{
			this.InitRefers();
			this.InitInjuryPage();
			this.InitHealthInfo();
		}
		this._inited = true;
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000F0C28 File Offset: 0x000EEE28
	public void Clear()
	{
		this._injuryGroupController.CharacterId = -1;
		this._poisonGroupController.CharacterId = -1;
		this._disorderOfQiController.CharacterId = -1;
		this._disorderOfQiController.OnFillDisorderOfQi = null;
		this._disorderOfQiController.OnFillChangeOfDisorderOfQi = null;
		bool flag = this._healthUIElement != null;
		if (flag)
		{
			this._healthUIElement.CharacterId = -1;
			this._healthUIElement.OnFillHealthChange = null;
		}
		this._inited = false;
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000F0CA4 File Offset: 0x000EEEA4
	public void Refresh(int characterId)
	{
		this._characterId = characterId;
		this._injuryGroupController.CharacterId = this._characterId;
		this._poisonGroupController.CharacterId = this._characterId;
		this._disorderOfQiController.CharacterId = this._characterId;
		this._healthUIElement.CharacterId = this._characterId;
		this._healthUIElement.GearMateMode = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x000F0D20 File Offset: 0x000EEF20
	private void InitInjuryPage()
	{
		Refers injuryRefers = this._injuryRoot;
		this._injuryGroupController = new CharacterInjuryGroup(new Refers[]
		{
			injuryRefers.CGet<Refers>("Chest"),
			injuryRefers.CGet<Refers>("Belly"),
			injuryRefers.CGet<Refers>("Head"),
			injuryRefers.CGet<Refers>("LeftHand"),
			injuryRefers.CGet<Refers>("RightHand"),
			injuryRefers.CGet<Refers>("LeftLeg"),
			injuryRefers.CGet<Refers>("RightLeg"),
			this._total
		}, this._mixPoisonBorder);
		this._injuryGroupController.CustomFillElement = new Action<Refers, sbyte, sbyte, sbyte>(this.FillInjuryElement);
		this._injuryGroupController.MixPoisonFillElement = new Action<GameObject>(this.FillMixPoisonElement);
		Refers poisonRefers = this._poisonRoot;
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
		TextMeshProUGUI label = this._total.CGet<TextMeshProUGUI>("PoisonValue");
		this._poisonGroupController.InitTotalLabel(label);
		Refers disorderOfQiRefers = this._areaPeriodEffect;
		CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
		TextMeshProUGUI stateLabel = disorderOfQiRefers.CGet<TextMeshProUGUI>("QiStateText");
		CImage stateIcon = disorderOfQiRefers.CGet<CImage>("QiStateIcon");
		QiContainer qiContainer = disorderOfQiRefers.CGet<QiContainer>("QiContainer");
		TooltipInvoker mouseTip = disorderOfQiRefers.CGet<TooltipInvoker>("QiMouseTip");
		this._disorderOfQiController = new CharacterDisorderOfQi(slider, stateLabel, stateIcon, qiContainer, mouseTip);
		this._disorderOfQiController.OnFillDisorderOfQi = new Action<float>(this.OnFillDisorderOfQi);
		this._disorderOfQiController.OnFillChangeOfDisorderOfQi = new Action<short>(this.OnFillChangeDisorderOfQiSlider);
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000F0F7D File Offset: 0x000EF17D
	private void InitHealthInfo()
	{
		this._healthUIElement = new CharacterHealth(this._characterHealthInfo);
		this._healthUIElement.SetGetHealthStringFunc(new Func<short[], int, string>(this.GetHealthString));
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000F0FAC File Offset: 0x000EF1AC
	private string GetHealthString(short[] paramsHealth, int characterId)
	{
		bool flag = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterId);
		if (flag)
		{
			paramsHealth[0] = paramsHealth[1];
		}
		return CommonUtils.GetCharacterHealthInfo(paramsHealth[0], paramsHealth[1], characterId).Item1;
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000F0FEC File Offset: 0x000EF1EC
	private void OnFillDisorderOfQi(float value)
	{
		CImage cImage = this._areaPeriodEffect.CGet<CImage>("Image_Real_Progress");
		bool flag = cImage != null;
		if (flag)
		{
			cImage.fillAmount = value;
		}
		CharacterAttributeDataView.UpdateDisorderOfQiSliderHandle(this._areaPeriodEffect, value);
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000F102C File Offset: 0x000EF22C
	private void OnFillChangeDisorderOfQiSlider(short changeValue)
	{
		Refers disorderOfQiRefers = this._areaPeriodEffect;
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

	// Token: 0x0600211E RID: 8478 RVA: 0x000F10FC File Offset: 0x000EF2FC
	private void FillInjuryElement(Refers refers, sbyte bodyPartType, sbyte outerValue, sbyte innerValue)
	{
		refers.CGet<GameObject>("NewInner").SetActive(false);
		refers.CGet<GameObject>("NewOuter").SetActive(false);
		refers.CGet<TextMeshProUGUI>("InnerValue").text = innerValue.ToString().SetColor((innerValue <= 0) ? "grey" : "innerinjury");
		refers.CGet<TextMeshProUGUI>("OuterValue").text = outerValue.ToString().SetColor((outerValue <= 0) ? "grey" : "outterinjury");
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000F118C File Offset: 0x000EF38C
	private void FillMixPoisonElement(GameObject heartBorder)
	{
		bool flag = this._characterId == -1;
		if (!flag)
		{
			CommonUtils.SetMixPoisonBorder(heartBorder, this._characterId);
		}
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000F11B8 File Offset: 0x000EF3B8
	private void InitRefers()
	{
		this._injuryRoot = base.CGet<Refers>("InjuryRoot");
		this._total = base.CGet<Refers>("Total");
		this._mixPoisonBorder = base.CGet<GameObject>("MixPoisonBorder");
		this._poisonRoot = base.CGet<Refers>("PoisonRoot");
		this._areaPeriodEffect = base.CGet<Refers>("AreaPeriodEffect");
		this._characterHealthInfo = base.CGet<CharacterHealthBar>("CharacterHealthInfo");
	}

	// Token: 0x04001993 RID: 6547
	private CharacterInjuryGroup _injuryGroupController;

	// Token: 0x04001994 RID: 6548
	private CharacterPoisonGroup _poisonGroupController;

	// Token: 0x04001995 RID: 6549
	private CharacterDisorderOfQi _disorderOfQiController;

	// Token: 0x04001996 RID: 6550
	private CharacterHealth _healthUIElement;

	// Token: 0x04001997 RID: 6551
	private bool _inited = false;

	// Token: 0x04001998 RID: 6552
	private UIBase _parent;

	// Token: 0x04001999 RID: 6553
	private int _characterId;

	// Token: 0x0400199A RID: 6554
	private Refers _injuryRoot;

	// Token: 0x0400199B RID: 6555
	private Refers _total;

	// Token: 0x0400199C RID: 6556
	private GameObject _mixPoisonBorder;

	// Token: 0x0400199D RID: 6557
	private Refers _poisonRoot;

	// Token: 0x0400199E RID: 6558
	private Refers _areaPeriodEffect;

	// Token: 0x0400199F RID: 6559
	private CharacterHealthBar _characterHealthInfo;
}
