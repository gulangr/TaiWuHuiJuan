using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200038C RID: 908
public class UI_Logo : UIBase
{
	// Token: 0x060035DF RID: 13791 RVA: 0x001B1B18 File Offset: 0x001AFD18
	private void OnEnable()
	{
		base.CGet<CanvasGroup>("LogoShow").gameObject.SetActive(true);
		base.CGet<TextMeshProUGUI>("HealthGame").gameObject.SetActive(false);
		ConchShipCursor.Instance.SetCursorVisible(true);
		this.SetupHintTween();
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x001B1B68 File Offset: 0x001AFD68
	private void SetupHintTween()
	{
		this.hintGroup.DOKill(false);
		this.hintGroup.alpha = 0f;
		this.hintGroup.DOFade(1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x001B1BB8 File Offset: 0x001AFDB8
	private void Update()
	{
		bool flag = this._canEnterGame && Input.anyKeyDown;
		if (flag)
		{
			AudioManager.Instance.PlaySound("ui_title_click", false, false);
			this._canEnterGame = false;
			base.CGet<CanvasGroup>("ClickToStart").gameObject.SetActive(false);
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("CGName", "LOGO");
			box.Set("RenderMode", 0);
			box.Set("JumpOpen", false);
			bool musicOn = SingletonObject.getInstance<GlobalSettings>().BgmOn;
			UIElement cgPlayer = UIElement.CgPlayer;
			cgPlayer.OnDeActive = (Action)Delegate.Combine(cgPlayer.OnDeActive, new Action(delegate()
			{
				SingletonObject.getInstance<GlobalSettings>().BgmOn = musicOn;
			}));
			box.SetObject("OnVideoPlayStart", new Action(this.<Update>g__OnVideoPlayStart|7_0));
			UIElement.CgPlayer.SetOnInitArgs(box);
			UIElement.CgPlayer.Show();
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(2f, delegate
			{
				base.CGet<CanvasGroup>("GameTitle").DOFade(0f, 1f);
			});
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(6f, delegate
			{
				base.CGet<CImage>("BlackMask").DOFade(1f, 2f);
			});
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(8f, delegate
			{
				GameApp.Instance.ChangeGameState(EGameState.Login, null);
			});
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(9f, delegate
			{
				base.CGet<CImage>("BlackMask").DOFade(0f, 1f).OnComplete(delegate
				{
					this.Element.Destroy();
				});
			});
		}
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x001B1D30 File Offset: 0x001AFF30
	public override void OnInit(ArgumentBox argsBox)
	{
		SingletonObject.getInstance<GlobalSettings>().HaveShowLogo = true;
		base.CGet<CImage>("BlackMask").SetAlpha(1f);
		base.CGet<CanvasGroup>("ClickToStart").gameObject.SetActive(false);
		base.CGet<CanvasGroup>("ClickToStart").alpha = 0f;
		base.CGet<CanvasGroup>("GameTitle").alpha = 0f;
		CanvasGroup logoCanvasGroup = base.CGet<CanvasGroup>("LogoShow");
		logoCanvasGroup.alpha = 0f;
		bool flag = this.AnimSequenceIn == null;
		if (flag)
		{
			this.SetAnimSequenceIn();
		}
		GEvent.AddOneShot(EEvents.OnGameResourceReady, new GEvent.Callback(this.OnGameResReady));
	}

	// Token: 0x060035E3 RID: 13795 RVA: 0x001B1DE8 File Offset: 0x001AFFE8
	public override void QuickHide()
	{
		UIElement mainMenu = UIElement.MainMenu;
		mainMenu.OnShowed = (Action)Delegate.Combine(mainMenu.OnShowed, new Action(delegate()
		{
			this.Element.Hide(true);
		}));
		GameApp.Instance.ChangeGameState(EGameState.Login, null);
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x001B1E20 File Offset: 0x001B0020
	private void SetAnimSequenceIn()
	{
		this.AnimSequenceIn = DOTween.Sequence();
		this.AnimSequenceIn.Append(base.CGet<CanvasGroup>("LogoShow").DOFade(1f, this.TmLogoFadeIn * this._speed));
		this.AnimSequenceIn.AppendInterval(this.TmLogoStay * this._speed);
		this.AnimSequenceIn.Pause<Sequence>();
	}

	// Token: 0x060035E5 RID: 13797 RVA: 0x001B1E8C File Offset: 0x001B008C
	private void OnGameResReady(ArgumentBox argBox)
	{
		base.CGet<CanvasGroup>("GameTitle").alpha = 1f;
		CanvasGroup logoCanvasGroup = base.CGet<CanvasGroup>("LogoShow");
		logoCanvasGroup.gameObject.SetActive(false);
		base.Invoke("ShowClickToStart", 2f);
	}

	// Token: 0x060035E6 RID: 13798 RVA: 0x001B1EDA File Offset: 0x001B00DA
	private void ShowClickToStart()
	{
		base.CGet<CanvasGroup>("ClickToStart").gameObject.SetActive(true);
		this._canEnterGame = true;
	}

	// Token: 0x060035E8 RID: 13800 RVA: 0x001B1F0F File Offset: 0x001B010F
	[CompilerGenerated]
	private void <Update>g__OnVideoPlayStart|7_0()
	{
		SingletonObject.getInstance<GlobalSettings>().BgmOn = false;
		base.CGet<CImage>("BlackMask").DOFade(0f, 1f);
	}

	// Token: 0x04002717 RID: 10007
	[SerializeField]
	private CanvasGroup hintGroup;

	// Token: 0x04002718 RID: 10008
	public float TmLogoFadeIn;

	// Token: 0x04002719 RID: 10009
	public float TmLogoStay;

	// Token: 0x0400271A RID: 10010
	private bool _canEnterGame;

	// Token: 0x0400271B RID: 10011
	private readonly float _speed = 1f;
}
