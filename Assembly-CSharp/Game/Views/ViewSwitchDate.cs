using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000711 RID: 1809
	public class ViewSwitchDate : UIBase
	{
		// Token: 0x060055CE RID: 21966 RVA: 0x0027C115 File Offset: 0x0027A315
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x060055CF RID: 21967 RVA: 0x0027C120 File Offset: 0x0027A320
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				bool flag = this.scrollNumberTemplate == null && this.digitColumnsRoot != null;
				if (flag)
				{
					this.scrollNumberTemplate = this.digitColumnsRoot.GetComponentInChildren<SwitchDateDigitColumn>(true);
				}
			}
		}

		// Token: 0x060055D0 RID: 21968 RVA: 0x0027C174 File Offset: 0x0027A374
		public override void OnInit(ArgumentBox argsBox)
		{
			this.Init();
			this._isUnknown = false;
			bool flag = argsBox != null;
			if (flag)
			{
				argsBox.Get("IsUnknown", out this._isUnknown);
			}
		}

		// Token: 0x060055D1 RID: 21969 RVA: 0x0027C1AA File Offset: 0x0027A3AA
		private void OnEnable()
		{
			this.PlayEffect();
		}

		// Token: 0x060055D2 RID: 21970 RVA: 0x0027C1B4 File Offset: 0x0027A3B4
		private void OnDisable()
		{
			Sequence playSequence = this._playSequence;
			if (playSequence != null)
			{
				playSequence.Kill(false);
			}
			this._playSequence = null;
			this.ClearDigitColumns();
			this.Reset();
		}

		// Token: 0x060055D3 RID: 21971 RVA: 0x0027C1E0 File Offset: 0x0027A3E0
		private void Reset()
		{
			this.bg.alpha = 0f;
			bool flag = this.textGroup != null;
			if (flag)
			{
				this.textGroup.alpha = 0f;
			}
			bool flag2 = this.leftText != null;
			if (flag2)
			{
				this.leftText.alpha = 0f;
			}
			bool flag3 = this.rightText != null;
			if (flag3)
			{
				this.rightText.alpha = 0f;
			}
		}

		// Token: 0x060055D4 RID: 21972 RVA: 0x0027C264 File Offset: 0x0027A464
		private void PlayEffect()
		{
			Sequence playSequence = this._playSequence;
			if (playSequence != null)
			{
				playSequence.Kill(false);
			}
			this.ClearDigitColumns();
			this.Reset();
			bool isUnknown = this._isUnknown;
			if (isUnknown)
			{
				this.PlayUnknownDateEffect();
			}
			else
			{
				this.PlayKnownDateEffect();
			}
		}

		// Token: 0x060055D5 RID: 21973 RVA: 0x0027C2AD File Offset: 0x0027A4AD
		private void PlayUnknownDateEffect()
		{
			this.PlayDateEffectWithSlotScroll(LanguageKey.LK_ThreeQuestionMark.Tr());
		}

		// Token: 0x060055D6 RID: 21974 RVA: 0x0027C2C4 File Offset: 0x0027A4C4
		private void PlayKnownDateEffect()
		{
			this.PlayDateEffectWithSlotScroll(CommonUtils.GetYearByDate(SingletonObject.getInstance<BasicGameData>().CurrDate).ToString());
		}

		// Token: 0x060055D7 RID: 21975 RVA: 0x0027C2F0 File Offset: 0x0027A4F0
		private void PlayDateEffectWithSlotScroll(string targetText)
		{
			bool flag = !this.BuildDigitColumns(targetText);
			if (flag)
			{
				this.Finish();
			}
			else
			{
				this.textBg.SetActive(false);
				SwitchDateScrollSettings s = this.scrollSettings;
				float scrollPhaseDuration = this.GetScrollPhaseDuration(targetText.Length);
				this._playSequence = DOTween.Sequence().SetUpdate(true);
				this._playSequence.Append(this.bg.DOFade(1f, s.bgFadeInDuration).OnStart(new TweenCallback(this.PlayEffFadeIn)));
				this._playSequence.AppendCallback(delegate
				{
					this.textBg.SetActive(true);
				});
				Sequence textIntro = DOTween.Sequence();
				textIntro.AppendCallback(delegate
				{
					bool flag4 = this.textGroup != null;
					if (flag4)
					{
						this.textGroup.alpha = 1f;
					}
				});
				bool flag2 = this.leftText != null;
				if (flag2)
				{
					textIntro.Insert(0f, this.leftText.DOFade(1f, s.sideTextFadeDuration));
				}
				else
				{
					textIntro.Insert(0f, DOTween.Sequence().AppendInterval(s.sideTextFadeDuration));
				}
				bool flag3 = this.rightText != null;
				if (flag3)
				{
					textIntro.Insert(s.rightTextStartDelay, this.rightText.DOFade(1f, s.sideTextFadeDuration));
				}
				float scrollStartAfterBg = s.rightTextStartDelay + s.sideTextFadeDuration + s.delayBeforeScroll;
				textIntro.AppendInterval(scrollStartAfterBg);
				this._playSequence.Append(textIntro);
				this._playSequence.AppendCallback(delegate
				{
					this.StartDigitScroll(targetText);
				});
				this._playSequence.AppendInterval(scrollPhaseDuration);
				this._playSequence.AppendCallback(delegate
				{
					this.TriggerTimeSwitchEvent(this._isUnknown);
				});
				Sequence exit = DOTween.Sequence();
				exit.Insert(s.textGroupFadeOutDelay, this.textGroup.DOFade(0f, s.textGroupFadeOutDuration));
				exit.InsertCallback(s.textGroupFadeOutDelay + s.effFadeOutDelay, new TweenCallback(this.PlayEffFadeOut));
				float bgFadeStart = s.delayBeforeBgFadeOut + s.effFadeOutDelay + s.textGroupFadeOutDelay;
				exit.Insert(bgFadeStart, this.bg.DOFade(0f, s.bgFadeOutDuration));
				exit.AppendInterval(s.bgFadeOutDuration);
				this._playSequence.Append(exit);
				this._playSequence.OnComplete(new TweenCallback(this.Finish));
			}
		}

		// Token: 0x060055D8 RID: 21976 RVA: 0x0027C578 File Offset: 0x0027A778
		private void PlayEffFadeIn()
		{
			bool flag = this.effFadeIn == null;
			if (!flag)
			{
				this.effFadeIn.gameObject.SetActive(true);
				this.effFadeIn.Play();
			}
		}

		// Token: 0x060055D9 RID: 21977 RVA: 0x0027C5B6 File Offset: 0x0027A7B6
		private void PlayEffFadeOut()
		{
			this.textBg.SetActive(false);
			this.effFadeOut.gameObject.SetActive(true);
			this.effFadeOut.Play();
		}

		// Token: 0x060055DA RID: 21978 RVA: 0x0027C5E4 File Offset: 0x0027A7E4
		private float GetScrollPhaseDuration(int columnCount)
		{
			bool flag = columnCount <= 0;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = (float)(columnCount - 1) * this.scrollSettings.columnStartInterval + this.scrollSettings.columnScrollDuration;
			}
			return result;
		}

		// Token: 0x060055DB RID: 21979 RVA: 0x0027C628 File Offset: 0x0027A828
		private bool BuildDigitColumns(string targetText)
		{
			this._digitColumns.Clear();
			bool flag = this.digitColumnsRoot == null || this.scrollNumberTemplate == null || string.IsNullOrEmpty(targetText);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CommonUtils.PrepareEnoughChildren(this.digitColumnsRoot, this.scrollNumberTemplate.gameObject, targetText.Length, null);
				for (int i = 0; i < targetText.Length; i++)
				{
					SwitchDateDigitColumn column = this.digitColumnsRoot.GetChild(i).GetComponent<SwitchDateDigitColumn>();
					bool flag2 = column == null;
					if (!flag2)
					{
						column.ApplyVisualSettings(this.scrollSettings.digitHeight, this.scrollSettings.fontSize, this.scrollSettings.digitColor);
						column.PrepareStrip(targetText[i], this.scrollSettings.minScrollCycles);
						this._digitColumns.Add(column);
					}
				}
				result = (this._digitColumns.Count > 0);
			}
			return result;
		}

		// Token: 0x060055DC RID: 21980 RVA: 0x0027C73C File Offset: 0x0027A93C
		private void StartDigitScroll(string targetText)
		{
			SwitchDateScrollSettings s = this.scrollSettings;
			for (int i = 0; i < this._digitColumns.Count; i++)
			{
				bool flag = i >= targetText.Length;
				if (flag)
				{
					break;
				}
				SwitchDateDigitColumn column = this._digitColumns[i];
				char targetChar = targetText[i];
				float delay = (float)i * s.columnStartInterval;
				column.PlayScroll(targetChar, s.columnScrollDuration, s.scrollEase, s.minScrollCycles, s.scrollOvershootLineRatio, s.scrollSettleDurationRatio, s.digitFadeInDuration).SetDelay(delay).SetUpdate(true);
			}
		}

		// Token: 0x060055DD RID: 21981 RVA: 0x0027C7E0 File Offset: 0x0027A9E0
		private void ClearDigitColumns()
		{
			bool flag = this.digitColumnsRoot != null && this.scrollNumberTemplate != null;
			if (flag)
			{
				CommonUtils.PrepareEnoughChildren(this.digitColumnsRoot, this.scrollNumberTemplate.gameObject, 0, null);
			}
			this._digitColumns.Clear();
		}

		// Token: 0x060055DE RID: 21982 RVA: 0x0027C83C File Offset: 0x0027AA3C
		private void TriggerTimeSwitchEvent(bool isUnknown)
		{
			GEvent.OnEvent(UiEvents.OnForceUpdateTime, EasyPool.Get<ArgumentBox>().Set("normal", !isUnknown));
		}

		// Token: 0x060055DF RID: 21983 RVA: 0x0027C862 File Offset: 0x0027AA62
		private void Finish()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("AfterSwitchDateDisplayFinish", true);
			this.QuickHide();
		}

		// Token: 0x04003AAC RID: 15020
		[SerializeField]
		private CanvasGroup bg;

		// Token: 0x04003AAD RID: 15021
		[SerializeField]
		private CanvasGroup textGroup;

		// Token: 0x04003AAE RID: 15022
		[SerializeField]
		private UIParticle effFadeIn;

		// Token: 0x04003AAF RID: 15023
		[SerializeField]
		private UIParticle effFadeOut;

		// Token: 0x04003AB0 RID: 15024
		[SerializeField]
		private GameObject textBg;

		// Token: 0x04003AB1 RID: 15025
		[SerializeField]
		private CanvasGroup leftText;

		// Token: 0x04003AB2 RID: 15026
		[SerializeField]
		private CanvasGroup rightText;

		// Token: 0x04003AB3 RID: 15027
		[Header("年份滚轮")]
		[Tooltip("带 HorizontalLayoutGroup 的容器；首个 inactive 子节点为 scrollNumberTemplate")]
		[SerializeField]
		private RectTransform digitColumnsRoot;

		// Token: 0x04003AB4 RID: 15028
		[Tooltip("单列滚轮模板，置于 digitColumnsRoot 下并保持 inactive")]
		[SerializeField]
		private SwitchDateDigitColumn scrollNumberTemplate;

		// Token: 0x04003AB5 RID: 15029
		[SerializeField]
		private SwitchDateScrollSettings scrollSettings = new SwitchDateScrollSettings();

		// Token: 0x04003AB6 RID: 15030
		private readonly List<SwitchDateDigitColumn> _digitColumns = new List<SwitchDateDigitColumn>();

		// Token: 0x04003AB7 RID: 15031
		private bool _isUnknown;

		// Token: 0x04003AB8 RID: 15032
		private Sequence _playSequence;

		// Token: 0x04003AB9 RID: 15033
		private bool _inited;
	}
}
