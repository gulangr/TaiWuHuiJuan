using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Character
{
	// Token: 0x02000F35 RID: 3893
	public class InjuryQiArea : MonoBehaviour
	{
		// Token: 0x1700144D RID: 5197
		// (get) Token: 0x0600B312 RID: 45842 RVA: 0x00518283 File Offset: 0x00516483
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x1700144E RID: 5198
		// (get) Token: 0x0600B313 RID: 45843 RVA: 0x0051828B File Offset: 0x0051648B
		public CButton SliderButton
		{
			get
			{
				return this.sliderButton;
			}
		}

		// Token: 0x1700144F RID: 5199
		// (get) Token: 0x0600B314 RID: 45844 RVA: 0x00518293 File Offset: 0x00516493
		public GameObject Selected
		{
			get
			{
				return this.selected;
			}
		}

		// Token: 0x0600B315 RID: 45845 RVA: 0x0051829C File Offset: 0x0051649C
		public void Set(CharacterInjuryDisplayData displayData)
		{
			InjuryQiArea.<>c__DisplayClass30_0 CS$<>8__locals1 = new InjuryQiArea.<>c__DisplayClass30_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.displayData = displayData;
			this.slider.wholeNumbers = true;
			this.slider.maxValue = (float)DisorderLevelOfQi.MaxValue;
			CS$<>8__locals1.totalWidth = this.imageRealProgress.rectTransform.rect.width;
			bool isSameChar = this._charId == CS$<>8__locals1.displayData.CharacterId;
			bool isValueChanged = this._disorderOfQi != (int)CS$<>8__locals1.displayData.DisorderOfQi;
			bool needPlayAnim = isSameChar && isValueChanged;
			bool flag = needPlayAnim;
			if (flag)
			{
				this.imageRecoverBetter.gameObject.SetActive(false);
				this.imageRecoverWorse.gameObject.SetActive(false);
				bool isAddValue = this._disorderOfQi < (int)CS$<>8__locals1.displayData.DisorderOfQi;
				float curRate = (float)this._disorderOfQi / (float)DisorderLevelOfQi.MaxValue;
				float targetRate = (float)CS$<>8__locals1.displayData.DisorderOfQi / (float)DisorderLevelOfQi.MaxValue;
				bool flag2 = isAddValue;
				if (flag2)
				{
					this.changeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
					this.imageAddMax.fillAmount = curRate;
					DOTween.Sequence().Append(this.imageAddMax.DOFillAmount(targetRate, 0.2f).From(curRate, true, false)).AppendInterval(0.2f).Append(this.imageRealProgress.DOFillAmount(targetRate, 0.2f).From(curRate, true, false)).AppendInterval(0.2f).AppendCallback(new TweenCallback(CS$<>8__locals1.<Set>g__Refresh|2));
					this.imageAddMax.gameObject.SetActive(true);
				}
				else
				{
					float previewRate = (float)(DisorderLevelOfQi.MaxValue - CS$<>8__locals1.displayData.DisorderOfQi) / (float)DisorderLevelOfQi.MaxValue;
					this.changeMask.SetPivot(this.changeMask.pivot.SetX(1f));
					this.changeMask.anchoredPosition = this.changeMask.anchoredPosition.SetX(0f);
					int changeValue = (int)CS$<>8__locals1.displayData.DisorderOfQi - this._disorderOfQi;
					this.changeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)Mathf.Abs(changeValue) / this.slider.maxValue * CS$<>8__locals1.totalWidth);
					this.imageReduceMin.fillAmount = 0f;
					DOTween.Sequence().Append(this.imageReduceMin.DOFillAmount(previewRate, 0.2f).From(0f, true, false)).AppendInterval(0.2f).AppendCallback(delegate
					{
						CS$<>8__locals1.<>4__this.imageRealProgress.fillAmount = targetRate;
						CS$<>8__locals1.<>4__this.imageReduceMin.fillOrigin = Image.OriginHorizontal.Left.ToInt();
					}).Append(this.imageReduceMin.DOFillAmount(0f, 0.2f).From(previewRate, true, false)).AppendInterval(0.2f).AppendCallback(new TweenCallback(CS$<>8__locals1.<Set>g__Refresh|2));
					this.imageReduceMin.gameObject.SetActive(true);
				}
			}
			else
			{
				CS$<>8__locals1.<Set>g__Refresh|2();
			}
			this.tip.Type = TipType.DisorderOfQi;
			TooltipInvoker tooltipInvoker = this.tip;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = new ArgumentBox());
			}
			ArgumentBox btnTipArgs = argumentBox;
			btnTipArgs.Clear();
			btnTipArgs.Set<CharacterInjuryDisplayData>("CharacterInjuryDisplayData", CS$<>8__locals1.displayData);
			this.sliderTip.Type = TipType.DisorderOfQi;
			tooltipInvoker = this.sliderTip;
			ArgumentBox argumentBox2;
			if ((argumentBox2 = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox2 = (tooltipInvoker.RuntimeParam = new ArgumentBox());
			}
			ArgumentBox sliderTipArgs = argumentBox2;
			sliderTipArgs.Clear();
			sliderTipArgs.Set<CharacterInjuryDisplayData>("CharacterInjuryDisplayData", CS$<>8__locals1.displayData);
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				CS$<>8__locals1.<>4__this.hover.SetActive(true);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				CS$<>8__locals1.<>4__this.hover.SetActive(false);
			});
			this.hover.SetActive(false);
			this.selected.SetActive(false);
		}

		// Token: 0x0600B316 RID: 45846 RVA: 0x005186EE File Offset: 0x005168EE
		public static string GetDisorderOfQiLevelIcon(sbyte level)
		{
			return InjuryQiArea.DisorderOfQiLevelIcon[(level < 2) ? 0 : 1];
		}

		// Token: 0x0600B317 RID: 45847 RVA: 0x00518700 File Offset: 0x00516900
		public void ShowInfectNotice(SimulateEatingEffectResult simulateEatingEffectResult)
		{
			short curDisorderOfQi = this._characterInjuryDisplayData.DisorderOfQi;
			bool isAdd = simulateEatingEffectResult.MinDisorderOfQi > (int)curDisorderOfQi;
			bool isReduce = simulateEatingEffectResult.MaxDisorderOfQi < (int)curDisorderOfQi;
			short changeValue = -DisorderLevelOfQi.MaxValue;
			float totalWidth = this.imageRealProgress.rectTransform.rect.width;
			this.changeMask.SetPivot(this.changeMask.pivot.SetX((float)((changeValue > 0) ? 0 : 1)));
			this.changeMask.anchoredPosition = this.changeMask.anchoredPosition.SetX(0f);
			float maskWidth = isReduce ? ((float)Mathf.Abs((int)changeValue) / this.slider.maxValue * totalWidth) : 0f;
			this.changeMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskWidth);
			this.imageRecoverBetter.gameObject.SetActive(false);
			this.imageRecoverWorse.gameObject.SetActive(false);
			this.imageAddMin.gameObject.SetActive(isAdd);
			this.imageAddMax.gameObject.SetActive(isAdd);
			this.imageAddMin.fillAmount = (float)simulateEatingEffectResult.MinDisorderOfQi / (float)DisorderLevelOfQi.MaxValue;
			this.imageAddMax.fillAmount = (float)simulateEatingEffectResult.MaxDisorderOfQi / (float)DisorderLevelOfQi.MaxValue;
			this.imageReduceMin.gameObject.SetActive(isReduce);
			this.imageReduceMax.gameObject.SetActive(isReduce);
			this.imageReduceMin.fillAmount = (float)((int)DisorderLevelOfQi.MaxValue - simulateEatingEffectResult.MinDisorderOfQi) / (float)DisorderLevelOfQi.MaxValue;
			this.imageReduceMax.fillAmount = (float)((int)DisorderLevelOfQi.MaxValue - simulateEatingEffectResult.MaxDisorderOfQi) / (float)DisorderLevelOfQi.MaxValue;
			this.selected.SetActive(true);
		}

		// Token: 0x0600B319 RID: 45849 RVA: 0x005188C0 File Offset: 0x00516AC0
		// Note: this type is marked as 'beforefieldinit'.
		static InjuryQiArea()
		{
			LanguageKey[] array = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.E23492D894508AE955CA047489440C5B44AB511667C0ACFD8B9553E5949F84DA).FieldHandle);
			InjuryQiArea.DisorderOfQiLevelLangKeys = array;
			InjuryQiArea.DisorderOfQiLevelIcon = new string[]
			{
				"ui9_icon_qi_disorder_0",
				"ui9_icon_qi_disorder_1"
			};
		}

		// Token: 0x04008B04 RID: 35588
		public static readonly string[] DisorderOfQiLevelColors = new string[]
		{
			"BehaviorType_Just",
			"BehaviorType_Even",
			"BehaviorType_Even",
			"BehaviorType_Even",
			"brightred"
		};

		// Token: 0x04008B05 RID: 35589
		public static readonly LanguageKey[] DisorderOfQiLevelLangKeys;

		// Token: 0x04008B06 RID: 35590
		public static readonly string[] DisorderOfQiLevelIcon;

		// Token: 0x04008B07 RID: 35591
		[SerializeField]
		private CSlider slider;

		// Token: 0x04008B08 RID: 35592
		[SerializeField]
		private CImage imageRealProgress;

		// Token: 0x04008B09 RID: 35593
		[SerializeField]
		private CImage imageRecoverBetter;

		// Token: 0x04008B0A RID: 35594
		[SerializeField]
		private CImage imageRecoverWorse;

		// Token: 0x04008B0B RID: 35595
		[SerializeField]
		private CImage imageAddMin;

		// Token: 0x04008B0C RID: 35596
		[SerializeField]
		private CImage imageAddMax;

		// Token: 0x04008B0D RID: 35597
		[SerializeField]
		private CImage imageReduceMin;

		// Token: 0x04008B0E RID: 35598
		[SerializeField]
		private CImage imageReduceMax;

		// Token: 0x04008B0F RID: 35599
		[SerializeField]
		private RectTransform changeMask;

		// Token: 0x04008B10 RID: 35600
		[SerializeField]
		private CImage stateIcon;

		// Token: 0x04008B11 RID: 35601
		[SerializeField]
		private TextMeshProUGUI stateName;

		// Token: 0x04008B12 RID: 35602
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04008B13 RID: 35603
		[SerializeField]
		private CButton button;

		// Token: 0x04008B14 RID: 35604
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04008B15 RID: 35605
		[SerializeField]
		private GameObject selected;

		// Token: 0x04008B16 RID: 35606
		[SerializeField]
		private GameObject hover;

		// Token: 0x04008B17 RID: 35607
		[SerializeField]
		private CButton sliderButton;

		// Token: 0x04008B18 RID: 35608
		[SerializeField]
		private TooltipInvoker sliderTip;

		// Token: 0x04008B19 RID: 35609
		private CharacterInjuryDisplayData _characterInjuryDisplayData;

		// Token: 0x04008B1A RID: 35610
		private int _charId;

		// Token: 0x04008B1B RID: 35611
		private int _disorderOfQi;
	}
}
