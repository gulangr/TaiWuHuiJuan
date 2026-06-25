using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.InteractCheckResult
{
	// Token: 0x020009F6 RID: 2550
	public class ViewInteractCheckResult : UIBase
	{
		// Token: 0x06007D9D RID: 32157 RVA: 0x003A54CE File Offset: 0x003A36CE
		private void Awake()
		{
			this.backButton.ClearAndAddListener(new Action(this.OnBackButtonClicked));
		}

		// Token: 0x06007D9E RID: 32158 RVA: 0x003A54E9 File Offset: 0x003A36E9
		public override void OnInit(ArgumentBox argsBox)
		{
			this.backButton.ClearAndAddListener(new Action(this.OnBackButtonClicked));
			this.ReadArgs(argsBox);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06007D9F RID: 32159 RVA: 0x003A5518 File Offset: 0x003A3718
		private void RefreshAll()
		{
			InteractCheckItem config = InteractCheck.Instance[this._interactData.InteractCheckTemplateId];
			short[] phaseList = this._interactData.IsEscape ? config.EscapePhaseList : config.ActionPhaseList;
			this.SetIsInAnimation(true);
			this._refreshConfig = this.GenerateRefreshConfig(phaseList);
			this.RefreshSpine(this._refreshConfig, false);
			this.RefreshParticle(this._refreshConfig, false);
			this.RefreshPhases(this._refreshConfig, false);
			AudioManager.Instance.PlaySound("Yao_Effect", false, false);
		}

		// Token: 0x06007DA0 RID: 32160 RVA: 0x003A55AC File Offset: 0x003A37AC
		private void RefreshParticle(ViewInteractCheckResult.RefreshConfig refreshConfig, bool asFinished = false)
		{
			ViewInteractCheckResult.<>c__DisplayClass9_0 CS$<>8__locals1 = new ViewInteractCheckResult.<>c__DisplayClass9_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.isSuccess = refreshConfig.IsAllSuccess;
			this.successIcon.gameObject.SetActive(false);
			this.failIcon.gameObject.SetActive(false);
			this.blueLoopParticle.gameObject.SetActive(false);
			this.redLoopParticle.gameObject.SetActive(false);
			this.blueOnceParticle2.gameObject.SetActive(false);
			this.redOnceParticle2.gameObject.SetActive(false);
			this.commonOnceParticle.gameObject.SetActive(false);
			bool flag = !asFinished;
			if (flag)
			{
				ParticleSystem particle = CS$<>8__locals1.isSuccess ? this.blueOnceParticle2 : this.redOnceParticle2;
				particle.Stop();
				particle.gameObject.SetActive(true);
				particle.Play();
				this.commonOnceParticle.Stop();
				this.commonOnceParticle.gameObject.SetActive(true);
				this.commonOnceParticle.Play();
				this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(CS$<>8__locals1.<RefreshParticle>g__Action1|0)));
				this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, delegate
				{
					particle.Stop();
					particle.gameObject.SetActive(false);
					CS$<>8__locals1.<>4__this.commonOnceParticle.Stop();
					CS$<>8__locals1.<>4__this.commonOnceParticle.gameObject.SetActive(false);
				}));
			}
			else
			{
				this.blueOnceParticle2.gameObject.SetActive(false);
				this.redOnceParticle2.gameObject.SetActive(false);
				this.commonOnceParticle.gameObject.SetActive(false);
				this.blueOnceParticle2.Stop();
				this.redOnceParticle2.Stop();
				this.commonOnceParticle.Stop();
				CS$<>8__locals1.<RefreshParticle>g__Action1|0();
			}
		}

		// Token: 0x06007DA1 RID: 32161 RVA: 0x003A579A File Offset: 0x003A399A
		private void RefreshResultIcon(bool success)
		{
			this.successIcon.gameObject.SetActive(success);
			this.failIcon.gameObject.SetActive(!success);
		}

		// Token: 0x06007DA2 RID: 32162 RVA: 0x003A57C4 File Offset: 0x003A39C4
		private void RefreshSpine(ViewInteractCheckResult.RefreshConfig refreshConfig, bool asFinished = false)
		{
			bool flag = !asFinished;
			if (flag)
			{
				int totalPhase = refreshConfig.PhaseList.Count;
				int successPhase = refreshConfig.PhaseList.Count((ViewInteractCheckResult.RefreshPhaseConfig t) => t.State == ViewInteractCheckResult.PhaseState.Success);
				bool flag2 = successPhase == totalPhase;
				int groupIndex;
				if (flag2)
				{
					groupIndex = Random.Range(1, 3);
				}
				else
				{
					bool flag3 = successPhase * 2 >= totalPhase;
					if (flag3)
					{
						groupIndex = 3;
					}
					else
					{
						groupIndex = 4;
					}
				}
				int random = Random.Range(1, 5);
				this._spineEntry = this.spine.AnimationState.SetAnimation(0, string.Format("cups_{0}_{1}", random, groupIndex), false);
			}
			else
			{
				this._spineEntry.TrackTime = this._spineEntry.AnimationEnd;
				this._spineEntry.TimeScale = 0f;
				this.spine.AnimationState.Apply(this.spine.Skeleton);
			}
			this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.15f, delegate
			{
				this.spine.GetComponent<CanvasGroup>().alpha = 1f;
			}));
		}

		// Token: 0x06007DA3 RID: 32163 RVA: 0x003A58E8 File Offset: 0x003A3AE8
		private ViewInteractCheckResult.RefreshConfig GenerateRefreshConfig(short[] phaseList)
		{
			ViewInteractCheckResult.RefreshConfig refreshConfig = new ViewInteractCheckResult.RefreshConfig
			{
				PhaseList = new List<ViewInteractCheckResult.RefreshPhaseConfig>()
			};
			for (int index = 0; index < phaseList.Length; index++)
			{
				short phase = phaseList[index];
				ViewInteractCheckResult.RefreshPhaseConfig phaseRefreshConfig = new ViewInteractCheckResult.RefreshPhaseConfig
				{
					PhaseId = (int)phase,
					State = ViewInteractCheckResult.CalcPhaseState(this._interactData, index)
				};
				refreshConfig.PhaseList.Add(phaseRefreshConfig);
			}
			return refreshConfig;
		}

		// Token: 0x06007DA4 RID: 32164 RVA: 0x003A5964 File Offset: 0x003A3B64
		public static ViewInteractCheckResult.PhaseState CalcPhaseState(EventInteractCheckData interactData, int phaseIndex)
		{
			InteractCheckItem config = InteractCheck.Instance[interactData.InteractCheckTemplateId];
			bool checkAllPhase = config.CheckAllPhase;
			bool isEscape = interactData.IsEscape;
			ViewInteractCheckResult.PhaseState result;
			if (isEscape)
			{
				result = ((interactData.FailPhase == 5) ? ViewInteractCheckResult.PhaseState.Success : ViewInteractCheckResult.PhaseState.Fail);
			}
			else
			{
				bool flag = interactData.FailPhaseIndex < 0 || (int)interactData.FailPhaseIndex > phaseIndex;
				if (flag)
				{
					result = ViewInteractCheckResult.PhaseState.Success;
				}
				else
				{
					bool flag2 = (int)interactData.FailPhaseIndex == phaseIndex;
					if (flag2)
					{
						result = ViewInteractCheckResult.PhaseState.Fail;
					}
					else
					{
						bool flag3 = !checkAllPhase;
						if (flag3)
						{
							result = ViewInteractCheckResult.PhaseState.NoNeedCheck;
						}
						else
						{
							result = ViewInteractCheckResult.PhaseState.Fail;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06007DA5 RID: 32165 RVA: 0x003A59F0 File Offset: 0x003A3BF0
		private void RefreshPhases(ViewInteractCheckResult.RefreshConfig refreshConfig, bool asFinished = false)
		{
			CommonUtils.PrepareEnoughChildren(this.layout, this.phaseTemplate, refreshConfig.PhaseList.Count, null);
			this._phaseCoroutines.Clear();
			float layoutWidth = this.layout.rect.width;
			List<float> positionPercentList = new List<float>();
			bool flag = refreshConfig.PhaseList.Count == 1;
			if (flag)
			{
				positionPercentList.Add(0.5f);
			}
			else
			{
				bool flag2 = refreshConfig.PhaseList.Count == 2;
				if (flag2)
				{
					positionPercentList.Add(0.33333334f);
					positionPercentList.Add(0.6666667f);
				}
				else
				{
					for (int i = 0; i < refreshConfig.PhaseList.Count; i++)
					{
						positionPercentList.Add((float)i / (float)(refreshConfig.PhaseList.Count - 1));
					}
				}
			}
			int lastHasParticle = 0;
			for (int j = 0; j < refreshConfig.PhaseList.Count; j++)
			{
				Transform phaseItem = this.layout.GetChild(j);
				phaseItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(layoutWidth * positionPercentList[j], 0f);
				Refers refers2 = phaseItem.GetComponent<Refers>();
				ViewInteractCheckResult.RefreshPhaseConfig phaseRefreshConfig = refreshConfig.PhaseList[j];
				this.RefreshPhaseItem(refers2, phaseRefreshConfig, j, true);
				this.RefreshPhaseItemIcon(refers2, phaseRefreshConfig, j, true);
				this.RefreshPhaseItemBeforeParticle(refers2, j);
				bool flag3 = phaseRefreshConfig.State != ViewInteractCheckResult.PhaseState.NoNeedCheck;
				if (flag3)
				{
					lastHasParticle = j;
				}
			}
			float velocity = positionPercentList[lastHasParticle] / 0.75f;
			bool vIsZero = lastHasParticle == 0 && positionPercentList.Count > 2;
			this.progressbarValue.fillAmount = 0f;
			for (int k = 0; k <= lastHasParticle; k++)
			{
				Transform phaseItem2 = this.layout.GetChild(k);
				Refers refers = phaseItem2.GetComponent<Refers>();
				int ii = k;
				bool flag4 = !asFinished;
				if (flag4)
				{
					bool isLast = k == lastHasParticle;
					float myDelay = vIsZero ? 0.75f : (positionPercentList[k] / velocity);
					this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(Mathf.Max(0f, myDelay), delegate
					{
						this.RefreshPhaseItemIcon(refers, refreshConfig.PhaseList[ii], ii, false);
						this.PlayPhaseItemParticle(refers, refreshConfig.PhaseList[ii], isLast, false);
					}));
					this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(Mathf.Max(0f, myDelay) + 0.2f, delegate
					{
						this.RefreshPhaseItem(refers, refreshConfig.PhaseList[ii], ii, false);
					}));
				}
				else
				{
					this.RefreshPhaseItemIcon(refers, refreshConfig.PhaseList[ii], ii, false);
					this.RefreshPhaseItem(refers, refreshConfig.PhaseList[ii], ii, false);
					this.PlayPhaseItemParticle(refers, refreshConfig.PhaseList[ii], false, asFinished);
				}
			}
			float targetFillAmount = positionPercentList[lastHasParticle];
			bool needExtendProgress = refreshConfig.IsAllSuccess && targetFillAmount < 1f;
			bool flag5 = needExtendProgress;
			if (flag5)
			{
				targetFillAmount = 1f;
			}
			if (asFinished)
			{
				this.progressbarValue.fillAmount = targetFillAmount;
			}
			else
			{
				float progressDuration = vIsZero ? 0.75f : (targetFillAmount / velocity);
				this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.05f, delegate
				{
					this.progressbarValue.DOFillAmount(targetFillAmount, progressDuration);
				}));
			}
		}

		// Token: 0x06007DA6 RID: 32166 RVA: 0x003A5E48 File Offset: 0x003A4048
		private void RefreshPhaseItemBeforeParticle(Refers refers, int index)
		{
			ParticleSystem blueOnceParticle = refers.CGet<ParticleSystem>("BlueOnceParticle");
			ParticleSystem redOnceParticle = refers.CGet<ParticleSystem>("RedOnceParticle");
			UIParticle redLoopParticle = refers.CGet<UIParticle>("RedLoopParticle");
			UIParticle blueLoopParticle = refers.CGet<UIParticle>("BlueLoopParticle");
			blueOnceParticle.gameObject.SetActive(false);
			redOnceParticle.gameObject.SetActive(false);
			redLoopParticle.gameObject.SetActive(false);
			blueLoopParticle.gameObject.SetActive(false);
			TextMeshProUGUI indexLabel = refers.CGet<TextMeshProUGUI>("IndexLabel");
			indexLabel.text = LocalStringManager.Get("LK_TraditionalChineseNumber_" + (index + 1).ToString());
		}

		// Token: 0x06007DA7 RID: 32167 RVA: 0x003A5EEC File Offset: 0x003A40EC
		private void PlayPhaseItemParticle(Refers refers, ViewInteractCheckResult.RefreshPhaseConfig refreshConfigPhase, bool isLast, bool asFinished = false)
		{
			ViewInteractCheckResult.<>c__DisplayClass17_0 CS$<>8__locals1 = new ViewInteractCheckResult.<>c__DisplayClass17_0();
			CS$<>8__locals1.isLast = isLast;
			CS$<>8__locals1.<>4__this = this;
			ParticleSystem blueOnceParticle = refers.CGet<ParticleSystem>("BlueOnceParticle");
			ParticleSystem redOnceParticle = refers.CGet<ParticleSystem>("RedOnceParticle");
			UIParticle redLoopParticle = refers.CGet<UIParticle>("RedLoopParticle");
			UIParticle blueLoopParticle = refers.CGet<UIParticle>("BlueLoopParticle");
			ParticleSystem onceParticle = (refreshConfigPhase.State == ViewInteractCheckResult.PhaseState.Success) ? blueOnceParticle : redOnceParticle;
			CS$<>8__locals1.sound = ((refreshConfigPhase.State == ViewInteractCheckResult.PhaseState.Success) ? "Yao_Factor_Suc" : "Yao_Factor_Fail");
			bool flag = !asFinished;
			if (flag)
			{
				onceParticle.Stop();
				onceParticle.Play();
				this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.02f, delegate
				{
					AudioManager.Instance.PlaySound(CS$<>8__locals1.sound, false, false);
				}));
				onceParticle.gameObject.SetActive(true);
			}
			else
			{
				onceParticle.gameObject.SetActive(false);
				onceParticle.Stop();
			}
			CS$<>8__locals1.loopParticle = ((refreshConfigPhase.State == ViewInteractCheckResult.PhaseState.Success) ? blueLoopParticle : redLoopParticle);
			bool flag2 = !asFinished;
			if (flag2)
			{
				this._phaseCoroutines.Add(SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.5f, new Action(CS$<>8__locals1.<PlayPhaseItemParticle>g__Action1|1)));
			}
			else
			{
				CS$<>8__locals1.<PlayPhaseItemParticle>g__Action1|1();
			}
		}

		// Token: 0x06007DA8 RID: 32168 RVA: 0x003A6024 File Offset: 0x003A4224
		public override void QuickHide()
		{
			bool flag = !this._isInAnimation;
			if (flag)
			{
				base.QuickHide();
			}
		}

		// Token: 0x06007DA9 RID: 32169 RVA: 0x003A6048 File Offset: 0x003A4248
		private void Update()
		{
			bool flag = !this._isInAnimation && (CommonCommandKit.Space.Check(this.Element, false, false, true, true, false) || CommonCommandKit.Esc.Check(this.Element, false, false, true, true, false));
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007DAA RID: 32170 RVA: 0x003A609D File Offset: 0x003A429D
		private void SetIsInAnimation(bool b)
		{
			this._isInAnimation = b;
			this.hotkeyDisplay.Refresh(b ? EHotKeyDisplayType.InteractCheckUnlockedSkip : EHotKeyDisplayType.InteractCheckUnlockedClose);
		}

		// Token: 0x06007DAB RID: 32171 RVA: 0x003A60BC File Offset: 0x003A42BC
		private void RefreshPhaseItem(Refers refers, ViewInteractCheckResult.RefreshPhaseConfig refreshConfigPhase, int index, bool asDefault = false)
		{
			CImage bg = refers.CGet<CImage>("Bg");
			CImage light = refers.CGet<CImage>("Light");
			CImage point = refers.CGet<CImage>("Point");
			TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
			TooltipInvoker tipDisplayer = refers.CGet<TooltipInvoker>("TipDisplayer");
			ViewInteractCheckResult.PhaseState phaseState = refreshConfigPhase.State;
			if (asDefault)
			{
				phaseState = ViewInteractCheckResult.PhaseState.NoNeedCheck;
			}
			CImage cimage = bg;
			if (!true)
			{
			}
			string spriteName;
			switch (phaseState)
			{
			case ViewInteractCheckResult.PhaseState.Success:
				spriteName = "ui9_back_interactcheck_box_1";
				break;
			case ViewInteractCheckResult.PhaseState.Fail:
				spriteName = "ui9_back_interactcheck_box_2";
				break;
			case ViewInteractCheckResult.PhaseState.NoNeedCheck:
				spriteName = "ui9_back_interactcheck_box_3";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			cimage.SetSprite(spriteName, false, null);
			CImage cimage2 = point;
			if (!true)
			{
			}
			switch (phaseState)
			{
			case ViewInteractCheckResult.PhaseState.Success:
				spriteName = "ui_taiwuevent_judgment_point_1";
				break;
			case ViewInteractCheckResult.PhaseState.Fail:
				spriteName = "ui_taiwuevent_judgment_point_2";
				break;
			case ViewInteractCheckResult.PhaseState.NoNeedCheck:
				spriteName = "ui_taiwuevent_judgment_point_0";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			cimage2.SetSprite(spriteName, false, null);
			light.gameObject.SetActive(phaseState != ViewInteractCheckResult.PhaseState.NoNeedCheck);
			bool activeSelf = light.gameObject.activeSelf;
			if (activeSelf)
			{
				CImage cimage3 = light;
				if (!true)
				{
				}
				if (phaseState != ViewInteractCheckResult.PhaseState.Success)
				{
					if (phaseState != ViewInteractCheckResult.PhaseState.Fail)
					{
						throw new ArgumentOutOfRangeException();
					}
					spriteName = "ui9_back_interactcheck_light_1";
				}
				else
				{
					spriteName = "ui9_back_interactcheck_light_0";
				}
				if (!true)
				{
				}
				cimage3.SetSprite(spriteName, false, null);
			}
			InteractCheckTipItem phaseConfig = InteractCheckTip.Instance[refreshConfigPhase.PhaseId];
			label.text = phaseConfig.PhaseName;
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tipDisplayer.RuntimeParam.Set<EventInteractCheckData>("InteractData", this._interactData);
			tipDisplayer.RuntimeParam.Set("PhaseIndex", index);
		}

		// Token: 0x06007DAC RID: 32172 RVA: 0x003A6280 File Offset: 0x003A4480
		private void RefreshPhaseItemIcon(Refers refers, ViewInteractCheckResult.RefreshPhaseConfig refreshConfigPhase, int index, bool asDefault = false)
		{
			InteractCheckTipItem phaseConfig = InteractCheckTip.Instance[refreshConfigPhase.PhaseId];
			CImage icon = refers.CGet<CImage>("Icon");
			icon.SetSprite(phaseConfig.PhaseIcon, false, null);
			bool flag = !asDefault;
			if (flag)
			{
				RectTransform iconTrans = icon.GetComponent<RectTransform>();
				iconTrans.localScale = Vector3.zero;
				iconTrans.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutExpo);
			}
			icon.gameObject.SetActive(!asDefault);
		}

		// Token: 0x06007DAD RID: 32173 RVA: 0x003A6304 File Offset: 0x003A4504
		private void OnBackButtonClicked()
		{
			bool isInAnimation = this._isInAnimation;
			if (isInAnimation)
			{
				this._phaseCoroutines.ForEach(new Action<Coroutine>(SingletonObject.getInstance<YieldHelper>().StopYield));
				this.RefreshAllAsAnimationFinished();
				this.SetIsInAnimation(false);
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007DAE RID: 32174 RVA: 0x003A6354 File Offset: 0x003A4554
		private void RefreshAllAsAnimationFinished()
		{
			this.RefreshParticle(this._refreshConfig, true);
			this.RefreshPhases(this._refreshConfig, true);
			this.RefreshSpine(this._refreshConfig, true);
		}

		// Token: 0x06007DAF RID: 32175 RVA: 0x003A6381 File Offset: 0x003A4581
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get<EventInteractCheckData>("InteractData", out this._interactData);
		}

		// Token: 0x06007DB0 RID: 32176 RVA: 0x003A6396 File Offset: 0x003A4596
		private void OnEnable()
		{
			AudioManager.Instance.EnableMusicVolumeRate(0.2f);
			AudioManager.Instance.SetMusicVolumeWithFade(0.2f, 0f);
			this.RefreshAll();
		}

		// Token: 0x06007DB1 RID: 32177 RVA: 0x003A63C8 File Offset: 0x003A45C8
		private void OnDisable()
		{
			TaiwuEventDomainMethod.Call.EventSelectContinue();
			this.StopAllAudio();
			AudioManager.Instance.DisableMusicVolumeRate();
			AudioManager.Instance.SetMusicVolumeWithFade(0.2f, 0f);
			this.spine.GetComponent<CanvasGroup>().alpha = 0f;
		}

		// Token: 0x06007DB2 RID: 32178 RVA: 0x003A6419 File Offset: 0x003A4619
		private void StopAllAudio()
		{
			AudioManager.Instance.StopSound("Yao_AmbLoop");
		}

		// Token: 0x04005FA3 RID: 24483
		private EventInteractCheckData _interactData;

		// Token: 0x04005FA4 RID: 24484
		private readonly List<Coroutine> _phaseCoroutines = new List<Coroutine>();

		// Token: 0x04005FA5 RID: 24485
		private bool _isInAnimation;

		// Token: 0x04005FA6 RID: 24486
		private TrackEntry _spineEntry;

		// Token: 0x04005FA7 RID: 24487
		[Header("")]
		[SerializeField]
		private CButton backButton;

		// Token: 0x04005FA8 RID: 24488
		[SerializeField]
		private CImage failIcon;

		// Token: 0x04005FA9 RID: 24489
		[SerializeField]
		private CImage progressbarValue;

		// Token: 0x04005FAA RID: 24490
		[SerializeField]
		private CImage successIcon;

		// Token: 0x04005FAB RID: 24491
		[SerializeField]
		private GameObject phaseTemplate;

		// Token: 0x04005FAC RID: 24492
		[SerializeField]
		private HotkeyDisplay hotkeyDisplay;

		// Token: 0x04005FAD RID: 24493
		[SerializeField]
		private ParticleSystem blueOnceParticle2;

		// Token: 0x04005FAE RID: 24494
		[SerializeField]
		private ParticleSystem commonOnceParticle;

		// Token: 0x04005FAF RID: 24495
		[SerializeField]
		private ParticleSystem redOnceParticle2;

		// Token: 0x04005FB0 RID: 24496
		[SerializeField]
		private RectTransform layout;

		// Token: 0x04005FB1 RID: 24497
		[SerializeField]
		private SkeletonGraphic spine;

		// Token: 0x04005FB2 RID: 24498
		[SerializeField]
		private ParticleSystem redLoopParticle;

		// Token: 0x04005FB3 RID: 24499
		[SerializeField]
		private ParticleSystem blueLoopParticle;

		// Token: 0x04005FB4 RID: 24500
		private ViewInteractCheckResult.RefreshConfig _refreshConfig;

		// Token: 0x02001F87 RID: 8071
		private struct RefreshConfig
		{
			// Token: 0x17001944 RID: 6468
			// (get) Token: 0x0600F44B RID: 62539 RVA: 0x0061FC20 File Offset: 0x0061DE20
			public bool IsAllSuccess
			{
				get
				{
					return this.PhaseList.All((ViewInteractCheckResult.RefreshPhaseConfig t) => t.State == ViewInteractCheckResult.PhaseState.Success);
				}
			}

			// Token: 0x0400CDD8 RID: 52696
			public List<ViewInteractCheckResult.RefreshPhaseConfig> PhaseList;
		}

		// Token: 0x02001F88 RID: 8072
		public enum PhaseState
		{
			// Token: 0x0400CDDA RID: 52698
			Success,
			// Token: 0x0400CDDB RID: 52699
			Fail,
			// Token: 0x0400CDDC RID: 52700
			NoNeedCheck
		}

		// Token: 0x02001F89 RID: 8073
		private struct RefreshPhaseConfig
		{
			// Token: 0x0400CDDD RID: 52701
			public int PhaseId;

			// Token: 0x0400CDDE RID: 52702
			public ViewInteractCheckResult.PhaseState State;
		}
	}
}
