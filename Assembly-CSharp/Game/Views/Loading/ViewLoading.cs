using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Loading
{
	// Token: 0x020009F5 RID: 2549
	public class ViewLoading : UIBase
	{
		// Token: 0x06007D88 RID: 32136 RVA: 0x003A4DBA File Offset: 0x003A2FBA
		private void Awake()
		{
			this.debugTool.SetActive(false);
		}

		// Token: 0x06007D89 RID: 32137 RVA: 0x003A4DCC File Offset: 0x003A2FCC
		private void OnPrevious()
		{
			this._tipIndex--;
			bool flag = this._tipIndex < 0;
			if (flag)
			{
				this._tipIndex = LoadingTips.Instance.Count - 1;
			}
			this.SetTipContent(this._tipIndex);
		}

		// Token: 0x06007D8A RID: 32138 RVA: 0x003A4E14 File Offset: 0x003A3014
		private void OnNext()
		{
			this._tipIndex++;
			bool flag = this._tipIndex >= LoadingTips.Instance.Count;
			if (flag)
			{
				this._tipIndex = 0;
			}
			this.SetTipContent(this._tipIndex);
		}

		// Token: 0x06007D8B RID: 32139 RVA: 0x003A4E60 File Offset: 0x003A3060
		public override void OnInit(ArgumentBox argsBox)
		{
			this._progressTopLimit = 0;
			this._nowProgress = 0;
			this._autoAdd = true;
			bool flag = argsBox != null;
			if (flag)
			{
				argsBox.Get<Action>("OnLoadingStart", out this._onLoadingStart);
				argsBox.Get<Action>("OnLoadingFinish", out this._onLoadingFinish);
				bool flag2 = this._onLoadingStart != null;
				if (flag2)
				{
					UIElement element = this.Element;
					element.OnShowed = (Action)Delegate.Combine(element.OnShowed, this._onLoadingStart);
				}
			}
			this.InitImageAndText();
			this.UpdateProgress();
			this._tipIndex = Random.Range(0, LoadingTips.Instance.Count);
			this.SetTipContent(this._tipIndex);
		}

		// Token: 0x06007D8C RID: 32140 RVA: 0x003A4F10 File Offset: 0x003A3110
		private void SetTipContent(int tipIndex)
		{
			LoadingTipsItem tipConfig = LoadingTips.Instance[tipIndex];
			this.tipTitle.text = tipConfig.Title;
			this.tipContent.text = tipConfig.Content;
		}

		// Token: 0x06007D8D RID: 32141 RVA: 0x003A4F4E File Offset: 0x003A314E
		public override void NotifyUIShow()
		{
		}

		// Token: 0x06007D8E RID: 32142 RVA: 0x003A4F51 File Offset: 0x003A3151
		public override void NotifyUIHide()
		{
			GEvent.OnEvent(UiEvents.OnLoadingElementHide, EasyPool.Get<ArgumentBox>().SetObject("Element", this.Element));
			GLog.LogWithTag("UI_Loading hide", Array.Empty<object>());
		}

		// Token: 0x06007D8F RID: 32143 RVA: 0x003A4F88 File Offset: 0x003A3188
		private void OnEnable()
		{
			this.PlayAnimation();
			GEvent.Add(EEvents.LoadingProgress, new GEvent.Callback(this.OnLoadingProgress));
			CommandKitBase.SetDisable(true);
			foreach (UnityEngine.Material material2 in this.effMaterials)
			{
				material2.color = this.effMatHideColor;
				material2.DOFade(1f, 0.3f);
			}
			UnityEngine.Material[] array2 = this.effMaterials2;
			for (int j = 0; j < array2.Length; j++)
			{
				UnityEngine.Material material = array2[j];
				material.SetColor("_TintColor", this.effMatHideColor2);
				DOTween.To(() => material.GetColor("_TintColor"), delegate(Color x)
				{
					material.SetColor("_TintColor", x);
				}, this.effMatShowColor2, 0.3f).SetEase(Ease.OutQuad);
			}
			this.LoadRandomBg();
		}

		// Token: 0x06007D90 RID: 32144 RVA: 0x003A5074 File Offset: 0x003A3274
		private void LoadRandomBg()
		{
			ResLoader.Load<Texture2D>("RemakeResources/Textures/UITexturesRemake/ui9_tex_loading_swordtomb_{0}".GetFormat(Random.Range(0, this.RandomBackgroundAmount - 1)), delegate(Texture2D tex)
			{
				bool flag = tex != null;
				if (flag)
				{
					this.mainBg.texture = tex;
				}
			}, null, false);
		}

		// Token: 0x06007D91 RID: 32145 RVA: 0x003A50A8 File Offset: 0x003A32A8
		private void PlayAnimation()
		{
			this.effFlowAnimation.Play();
		}

		// Token: 0x06007D92 RID: 32146 RVA: 0x003A50B7 File Offset: 0x003A32B7
		private void OnDisable()
		{
			this._onLoadingStart = null;
			GEvent.Remove(EEvents.LoadingProgress, new GEvent.Callback(this.OnLoadingProgress));
			CommandKitBase.SetDisable(false);
			DisplayEventHandler.HandlePendingNormalDialogActions();
		}

		// Token: 0x06007D93 RID: 32147 RVA: 0x003A50E6 File Offset: 0x003A32E6
		private void InitImageAndText()
		{
		}

		// Token: 0x06007D94 RID: 32148 RVA: 0x003A50E9 File Offset: 0x003A32E9
		private void UpdateProgress()
		{
			this.matSwordProgress.SetFloat("_rongjie", 1f - (float)this._nowProgress / 100f);
		}

		// Token: 0x06007D95 RID: 32149 RVA: 0x003A5110 File Offset: 0x003A3310
		private void OnLoadingProgress(ArgumentBox argsBox)
		{
			int progress;
			argsBox.Get("Progress", out progress);
			float time;
			argsBox.Get("AniTime", out time);
			this._progressTopLimit = progress;
			this._autoAdd = (time <= 0f);
			bool flag = !this._autoAdd;
			if (flag)
			{
				this._loadingBarTweener = DOVirtual.Float((float)this._nowProgress, (float)this._progressTopLimit, time, delegate(float value)
				{
					int newProgress = (int)Mathf.Min(value, (float)this._progressTopLimit);
					bool flag3 = this._nowProgress != newProgress;
					if (flag3)
					{
						this._nowProgress = newProgress;
						this.UpdateProgress();
					}
				}).SetEase(Ease.Linear).SetAutoKill(true).OnComplete(delegate
				{
					this._autoAdd = true;
				});
			}
			else
			{
				bool flag2 = this._loadingBarTweener != null;
				if (flag2)
				{
					this._loadingBarTweener.Kill(false);
					this._loadingBarTweener = null;
				}
			}
			AudioManager.Instance.OnApplicationFocus(Application.isFocused);
		}

		// Token: 0x06007D96 RID: 32150 RVA: 0x003A51DC File Offset: 0x003A33DC
		private void LateUpdate()
		{
			bool flag = this._autoAdd && this._nowProgress < this._progressTopLimit;
			if (flag)
			{
				bool flag2 = Time.frameCount % 2 == 0;
				if (flag2)
				{
					this._nowProgress = Mathf.Clamp(this._nowProgress + 3, 0, this._progressTopLimit);
					this.UpdateProgress();
					bool flag3 = this._progressTopLimit >= 100 && this._nowProgress >= this._progressTopLimit;
					if (flag3)
					{
						this.FinishLoading();
					}
				}
			}
		}

		// Token: 0x06007D97 RID: 32151 RVA: 0x003A5268 File Offset: 0x003A3468
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, true, true, true, false) || CommonCommandKit.LeftMouse.Check(this.Element, false, true, true, true, false);
			if (flag)
			{
				this.OnPrevious();
			}
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, true, true, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, true, true, true, false);
			if (flag2)
			{
				this.OnNext();
			}
		}

		// Token: 0x06007D98 RID: 32152 RVA: 0x003A52F0 File Offset: 0x003A34F0
		private void FinishLoading()
		{
			Action onLoadingFinish = this._onLoadingFinish;
			if (onLoadingFinish != null)
			{
				onLoadingFinish();
			}
			this._onLoadingFinish = null;
			this._progressTopLimit = 0;
			this.Element.Hide(false);
			foreach (UnityEngine.Material material2 in this.effMaterials)
			{
				material2.color = this.effMatShowColor;
				material2.DOFade(0f, 1f);
			}
			UnityEngine.Material[] array2 = this.effMaterials2;
			for (int j = 0; j < array2.Length; j++)
			{
				UnityEngine.Material material = array2[j];
				material.SetColor("_TintColor", this.effMatShowColor2);
				DOTween.To(() => material.GetColor("_TintColor"), delegate(Color x)
				{
					material.SetColor("_TintColor", x);
				}, this.effMatHideColor2, 0.3f).SetEase(Ease.OutQuad);
			}
		}

		// Token: 0x04005F88 RID: 24456
		public int RandomBackgroundAmount = 9;

		// Token: 0x04005F89 RID: 24457
		[SerializeField]
		private CRawImage mainBg;

		// Token: 0x04005F8A RID: 24458
		[SerializeField]
		private TextMeshProUGUI tipTitle;

		// Token: 0x04005F8B RID: 24459
		[SerializeField]
		private TextMeshProUGUI tipContent;

		// Token: 0x04005F8C RID: 24460
		[SerializeField]
		private GameObject debugTool;

		// Token: 0x04005F8D RID: 24461
		[SerializeField]
		private CButton btnStayLoading;

		// Token: 0x04005F8E RID: 24462
		[SerializeField]
		private CButton btnPrevious;

		// Token: 0x04005F8F RID: 24463
		[SerializeField]
		private CButton btnNext;

		// Token: 0x04005F90 RID: 24464
		[SerializeField]
		private CButton btnRandom;

		// Token: 0x04005F91 RID: 24465
		[SerializeField]
		private CButton btnRandomBg;

		// Token: 0x04005F92 RID: 24466
		[SerializeField]
		private Animation effFlowAnimation;

		// Token: 0x04005F93 RID: 24467
		[SerializeField]
		private UnityEngine.Material matSwordProgress;

		// Token: 0x04005F94 RID: 24468
		[SerializeField]
		private GameObject effObj;

		// Token: 0x04005F95 RID: 24469
		[SerializeField]
		private UnityEngine.Material[] effMaterials;

		// Token: 0x04005F96 RID: 24470
		[SerializeField]
		private UnityEngine.Material[] effMaterials2;

		// Token: 0x04005F97 RID: 24471
		private Color effMatHideColor = new Color(1f, 1f, 1f, 0f);

		// Token: 0x04005F98 RID: 24472
		private Color effMatShowColor = Color.white;

		// Token: 0x04005F99 RID: 24473
		private Color effMatHideColor2 = new Color(0.5f, 0.5f, 0.5f, 0f);

		// Token: 0x04005F9A RID: 24474
		private Color effMatShowColor2 = new Color(0.5f, 0.5f, 0.5f, 1f);

		// Token: 0x04005F9B RID: 24475
		private const int AutoAddFrame = 2;

		// Token: 0x04005F9C RID: 24476
		private int _progressTopLimit;

		// Token: 0x04005F9D RID: 24477
		private int _nowProgress;

		// Token: 0x04005F9E RID: 24478
		private bool _autoAdd;

		// Token: 0x04005F9F RID: 24479
		private Tweener _loadingBarTweener;

		// Token: 0x04005FA0 RID: 24480
		private Action _onLoadingFinish;

		// Token: 0x04005FA1 RID: 24481
		private Action _onLoadingStart;

		// Token: 0x04005FA2 RID: 24482
		private int _tipIndex;
	}
}
