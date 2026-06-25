using System;
using System.IO;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.Migrate;
using TMPro;
using UnityEngine;

// Token: 0x02000369 RID: 873
public class UI_Advance : UIBase
{
	// Token: 0x060032AF RID: 12975 RVA: 0x0018FEA4 File Offset: 0x0018E0A4
	public override void OnInit(ArgumentBox argsBox)
	{
		this.HideMask();
		this._baseGameData = SingletonObject.getInstance<BasicGameData>();
		this._monthCalculateText = this.advanceMonth.monthAreaCalculate;
		this.advanceMonth.gameObject.SetActive(false);
		this.advanceMonth.fromSolarTerm.SetSprite(string.Empty, false, null);
		this.advanceMonth.toSolarTerm.SetSprite(string.Empty, false, null);
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x0018FF18 File Offset: 0x0018E118
	private void OnEnable()
	{
		this.savingInfo.SetActive(false);
		GEvent.Add(EEvents.OnAdvancingMonthStateChange, new GEvent.Callback(this.OnAdvanceMonthState));
		GEvent.Add(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.OnSavingWorldStateChange));
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x0018FF64 File Offset: 0x0018E164
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnAdvancingMonthStateChange, new GEvent.Callback(this.OnAdvanceMonthState));
		GEvent.Remove(EEvents.OnSavingWorldStateChange, new GEvent.Callback(this.OnSavingWorldStateChange));
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x0018FF98 File Offset: 0x0018E198
	private void OnDestroy()
	{
		UIElement eventWindow = UIElement.EventWindow;
		eventWindow.OnActive = (Action)Delegate.Remove(eventWindow.OnActive, new Action(this.HideMask));
		UIElement monthNotify = UIElement.MonthNotify;
		monthNotify.OnDeActive = (Action)Delegate.Remove(monthNotify.OnDeActive, new Action(this.HideMask));
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x0018FFF4 File Offset: 0x0018E1F4
	private void ShowAdvanceMonth()
	{
		this.mask.SetActive(true);
		CommandKitBase.SetDisable(true);
		this._showingAdvanceInfo = true;
		TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
		MonthItem curMonthConfig = Month.Instance[timeManager.GetMonthInCurrYear()];
		MonthItem nextMonthConfig = Month.Instance[timeManager.GetMonthInYear(this._baseGameData.CurrDate + 1)];
		TextMeshProUGUI fromSolarTermName = this.advanceMonth.solarTermName;
		TextMeshProUGUI toSolarTermName = this.advanceMonth.solarTermNextName;
		CImage fromImg = this.advanceMonth.fromSolarTerm;
		CImage toImg = this.advanceMonth.toSolarTerm;
		fromSolarTermName.text = curMonthConfig.Name;
		toSolarTermName.text = nextMonthConfig.Name;
		Color color = fromSolarTermName.color;
		color.a = 1f;
		fromSolarTermName.color = color;
		color.a = 0f;
		toSolarTermName.color = color;
		fromImg.enabled = false;
		fromImg.color = Color.white;
		toImg.enabled = false;
		toImg.color = UI_Advance.TransparentWhite;
		this._monthCalculateText.text = AdvancingMonthState.Instance[0].HintText;
		ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/MonthTexture/", curMonthConfig.Texture), delegate(Sprite sprite)
		{
			fromImg.sprite = sprite;
			fromImg.enabled = true;
			bool enabled = toImg.enabled;
			if (enabled)
			{
				base.<ShowAdvanceMonth>g__ShowAnimation|0();
			}
		}, null, false);
		ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/MonthTexture/", nextMonthConfig.Texture), delegate(Sprite sprite)
		{
			toImg.sprite = sprite;
			toImg.enabled = true;
			bool enabled = fromImg.enabled;
			if (enabled)
			{
				base.<ShowAdvanceMonth>g__ShowAnimation|0();
			}
		}, null, false);
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x001901A9 File Offset: 0x0018E3A9
	private void WaitSomeTime()
	{
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x001901AC File Offset: 0x0018E3AC
	private void HideAdvanceMonth()
	{
		bool flag = !this._showingAdvanceInfo;
		if (!flag)
		{
			this._showingAdvanceInfo = false;
			this.advanceMonth.gameObject.SetActive(false);
			this.advanceMonth.fromSolarTerm.SetSprite(string.Empty, false, null);
			this.advanceMonth.toSolarTerm.SetSprite(string.Empty, false, null);
			GEvent.OnEvent(UiEvents.OnAdvanceMonthAnimationComplete, null);
			bool flag2 = !UIElement.FullScreenMask.Exist;
			if (flag2)
			{
				CommandKitBase.SetDisable(false);
			}
		}
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x00190238 File Offset: 0x0018E438
	private void HideMask()
	{
		bool flag = null != this.mask;
		if (flag)
		{
			this.mask.SetActive(false);
		}
	}

	// Token: 0x060032B7 RID: 12983 RVA: 0x00190264 File Offset: 0x0018E464
	private void OnSavingWorldStateChange(ArgumentBox argBox)
	{
		bool savingWorld = SingletonObject.getInstance<BasicGameData>().SavingWorld;
		RectTransform savingInfoRect = this.savingInfo.GetComponent<RectTransform>();
		bool flag = savingWorld;
		if (flag)
		{
			UIManager.Instance.MaskComponent(savingInfoRect);
		}
		else
		{
			UIManager.Instance.UnMaskComponent(savingInfoRect);
		}
		bool flag2 = savingWorld;
		if (flag2)
		{
			this.savingInfoText.alpha = 0f;
			this.savingInfoText.DOFade(1f, 0.33f);
			UnityEngine.Material mat = this.savingInfoTextBg.material;
			mat.SetTextureOffset("_MaskTex", new Vector2(1f, 1f));
			DOTween.To(() => mat.GetTextureOffset("_MaskTex"), delegate(Vector2 x)
			{
				mat.SetTextureOffset("_MaskTex", x);
			}, new Vector2(0f, 1f), 0.33f).SetEase(Ease.OutQuad);
		}
		Action<bool> onSavingWorldStateChanged = UI_Advance.OnSavingWorldStateChanged;
		if (onSavingWorldStateChanged != null)
		{
			onSavingWorldStateChanged(savingWorld);
		}
	}

	// Token: 0x060032B8 RID: 12984 RVA: 0x00190360 File Offset: 0x0018E560
	private void OnAdvanceMonthState(ArgumentBox argBox)
	{
		bool flag = this._baseGameData.AdvancingMonthState > 0 && this._baseGameData.AdvancingMonthState < 14 && !this._showingAdvanceInfo;
		if (flag)
		{
			this.ShowAdvanceMonth();
		}
		bool flag2 = this._baseGameData.AdvancingMonthState == 14 && this._showingAdvanceInfo;
		if (flag2)
		{
			this.HideAdvanceMonth();
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				this.HideMask();
			}
			UIElement eventWindow = UIElement.EventWindow;
			eventWindow.OnActive = (Action)Delegate.Remove(eventWindow.OnActive, new Action(this.HideMask));
			UIElement eventWindow2 = UIElement.EventWindow;
			eventWindow2.OnActive = (Action)Delegate.Combine(eventWindow2.OnActive, new Action(this.HideMask));
			UIElement monthNotify = UIElement.MonthNotify;
			monthNotify.OnDeActive = (Action)Delegate.Remove(monthNotify.OnDeActive, new Action(this.HideMask));
			UIElement monthNotify2 = UIElement.MonthNotify;
			monthNotify2.OnDeActive = (Action)Delegate.Combine(monthNotify2.OnDeActive, new Action(this.HideMask));
		}
		else
		{
			bool flag3 = this._baseGameData.AdvancingMonthState == 0;
			if (flag3)
			{
				GameApp.AdvancingMonth = false;
			}
			AdvancingMonthStateItem advancingMonthState = AdvancingMonthState.Instance[(int)this._baseGameData.AdvancingMonthState];
			bool flag4 = advancingMonthState != null;
			if (flag4)
			{
				this._monthCalculateText.text = advancingMonthState.HintText;
			}
		}
	}

	// Token: 0x04002519 RID: 9497
	private const string SolarTermTexturePath = "RemakeResources/Textures/MonthTexture/";

	// Token: 0x0400251A RID: 9498
	private const string AdvanceAnimationPath = "RemakeResources/SpineAnimations/Advance/";

	// Token: 0x0400251B RID: 9499
	private static readonly Color TransparentWhite = new Color(1f, 1f, 1f, 0f);

	// Token: 0x0400251C RID: 9500
	private BasicGameData _baseGameData;

	// Token: 0x0400251D RID: 9501
	private bool _showingAdvanceInfo;

	// Token: 0x0400251E RID: 9502
	private TextMeshProUGUI _monthCalculateText;

	// Token: 0x0400251F RID: 9503
	[NonSerialized]
	public static Action<bool> OnSavingWorldStateChanged;

	// Token: 0x04002520 RID: 9504
	[SerializeField]
	private AdvanceMonth advanceMonth;

	// Token: 0x04002521 RID: 9505
	[SerializeField]
	private GameObject savingInfo;

	// Token: 0x04002522 RID: 9506
	[SerializeField]
	private CImage savingInfoTextBg;

	// Token: 0x04002523 RID: 9507
	[SerializeField]
	private CanvasGroup savingInfoText;

	// Token: 0x04002524 RID: 9508
	[SerializeField]
	private GameObject mask;
}
