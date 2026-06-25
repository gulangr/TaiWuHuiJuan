using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F30 RID: 3888
	public class InjuryHealthArea : MonoBehaviour
	{
		// Token: 0x1700143A RID: 5178
		// (get) Token: 0x0600B2E5 RID: 45797 RVA: 0x00517313 File Offset: 0x00515513
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x1700143B RID: 5179
		// (get) Token: 0x0600B2E6 RID: 45798 RVA: 0x0051731B File Offset: 0x0051551B
		public CButton SliderButton
		{
			get
			{
				return this.sliderButton;
			}
		}

		// Token: 0x1700143C RID: 5180
		// (get) Token: 0x0600B2E7 RID: 45799 RVA: 0x00517323 File Offset: 0x00515523
		public GameObject Selected
		{
			get
			{
				return this.selected;
			}
		}

		// Token: 0x0600B2E8 RID: 45800 RVA: 0x0051732C File Offset: 0x0051552C
		public void Set(CharacterInjuryDisplayData displayData)
		{
			InjuryHealthArea.<>c__DisplayClass20_0 CS$<>8__locals1 = new InjuryHealthArea.<>c__DisplayClass20_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.displayData = displayData;
			CS$<>8__locals1.totalWidth = this.progressIncrease.rectTransform.rect.width;
			bool flag = float.IsNaN(CS$<>8__locals1.totalWidth) || float.IsInfinity(CS$<>8__locals1.totalWidth) || CS$<>8__locals1.totalWidth <= 0f;
			if (flag)
			{
				CS$<>8__locals1.totalWidth = 0f;
			}
			bool isSameChar = this._charId == CS$<>8__locals1.displayData.CharacterId;
			bool isValueChanged = this._health != (int)CS$<>8__locals1.displayData.Health;
			bool needPlayAnim = isSameChar && isValueChanged;
			CS$<>8__locals1.leftMaxHealthIsZero = (CS$<>8__locals1.displayData.LeftMaxHealth <= 0);
			bool flag2 = needPlayAnim;
			if (flag2)
			{
				bool isAddValue = this._health < (int)CS$<>8__locals1.displayData.Health;
				float curRate = CS$<>8__locals1.leftMaxHealthIsZero ? 0f : ((float)this._health / (float)CS$<>8__locals1.displayData.LeftMaxHealth);
				float targetRate = CS$<>8__locals1.leftMaxHealthIsZero ? 0f : ((float)CS$<>8__locals1.displayData.Health / (float)CS$<>8__locals1.displayData.LeftMaxHealth);
				bool flag3 = isAddValue;
				if (flag3)
				{
					this.progressIncrease.fillAmount = curRate;
					DOTween.Sequence().Append(this.progressIncrease.DOFillAmount(targetRate, 0.2f).From(curRate, true, false)).AppendInterval(0.2f).Append(this.progress.DOFillAmount(targetRate, 0.2f).From(curRate, true, false)).AppendInterval(0.2f).AppendCallback(new TweenCallback(CS$<>8__locals1.<Set>g__Refresh|2));
					this.progressIncrease.gameObject.SetActive(true);
				}
				else
				{
					this.progressReduce.rectTransform.anchoredPosition = Vector2.zero.SetX(CS$<>8__locals1.totalWidth * (1f - curRate));
					this.progressReduce.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
					int changeValue = (int)CS$<>8__locals1.displayData.Health - this._health;
					float previewWith = CS$<>8__locals1.totalWidth * (float)Math.Abs(changeValue);
					Vector2 previewSize = new Vector2(previewWith, this.progressReduce.rectTransform.rect.height);
					DOTween.Sequence().Append(this.progressReduce.rectTransform.DOSizeDelta(previewSize, 0.2f, false)).AppendInterval(0.2f).AppendCallback(delegate
					{
						CS$<>8__locals1.<>4__this.progress.fillAmount = targetRate;
						CS$<>8__locals1.<>4__this.progressReduce.rectTransform.SetPivot(CS$<>8__locals1.<>4__this.progressReduce.rectTransform.pivot.SetX(0f));
					}).Append(this.progressReduce.rectTransform.DOSizeDelta(previewSize.SetX(0f), 0.2f, false)).AppendInterval(0.2f).AppendCallback(new TweenCallback(CS$<>8__locals1.<Set>g__Refresh|2));
					this.progressReduce.gameObject.SetActive(true);
				}
			}
			else
			{
				CS$<>8__locals1.<Set>g__Refresh|2();
			}
			this.RefreshTip(CS$<>8__locals1.displayData);
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

		// Token: 0x0600B2E9 RID: 45801 RVA: 0x00517710 File Offset: 0x00515910
		private void SetProgress(short health, short leftHealth, int characterId = -1)
		{
			ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(health, leftHealth, characterId);
			this.textState.text = info.Item1;
			this.progress.fillAmount = info.Item2;
		}

		// Token: 0x0600B2EA RID: 45802 RVA: 0x0051774C File Offset: 0x0051594C
		private void RefreshTip(CharacterInjuryDisplayData displayData)
		{
			InjuryHealthArea.<>c__DisplayClass22_0 CS$<>8__locals1;
			CS$<>8__locals1.displayData = displayData;
			InjuryHealthArea.<RefreshTip>g__Refresh|22_0(this.tip, ref CS$<>8__locals1);
			InjuryHealthArea.<RefreshTip>g__Refresh|22_0(this.sliderTip, ref CS$<>8__locals1);
		}

		// Token: 0x0600B2EB RID: 45803 RVA: 0x00517780 File Offset: 0x00515980
		public void ShowInfectNotice(short previewHealth)
		{
			previewHealth += this._characterInjuryDisplayData.HealthRecovery;
			this.SetProgress(previewHealth, this._characterInjuryDisplayData.LeftMaxHealth, this._characterInjuryDisplayData.CharacterId);
			this.Selected.SetActive(previewHealth != this._characterInjuryDisplayData.Health);
			float curRate = (float)this._characterInjuryDisplayData.Health / (float)this._characterInjuryDisplayData.LeftMaxHealth;
			float targetRate = (float)previewHealth / (float)this._characterInjuryDisplayData.LeftMaxHealth;
			bool isAddValue = previewHealth > this._characterInjuryDisplayData.Health;
			bool isReduceValue = previewHealth < this._characterInjuryDisplayData.Health;
			this.progress.fillAmount = curRate;
			this.progressIncrease.gameObject.SetActive(isAddValue);
			this.progressReduce.gameObject.SetActive(isReduceValue);
			bool flag = isAddValue;
			if (flag)
			{
				this.progressIncrease.fillAmount = targetRate;
			}
			else
			{
				bool flag2 = isReduceValue;
				if (flag2)
				{
					float totalWidth = this.progressIncrease.rectTransform.rect.width;
					bool flag3 = float.IsNaN(totalWidth) || float.IsInfinity(totalWidth) || totalWidth <= 0f;
					if (!flag3)
					{
						this.progressReduce.rectTransform.anchoredPosition = Vector2.zero.SetX(totalWidth * (1f - curRate));
						int changeValue = (int)previewHealth - this._health;
						float previewWith = totalWidth * (float)Math.Abs(changeValue);
						this.progressReduce.rectTransform.SetWidth(previewWith);
					}
				}
			}
		}

		// Token: 0x0600B2ED RID: 45805 RVA: 0x00517918 File Offset: 0x00515B18
		[CompilerGenerated]
		internal static void <RefreshTip>g__Refresh|22_0(TooltipInvoker mouseTip, ref InjuryHealthArea.<>c__DisplayClass22_0 A_1)
		{
			bool flag = mouseTip == null;
			if (!flag)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.HealthInfo;
				mouseTip.IsLanguageKey = false;
				if (mouseTip.RuntimeParam == null)
				{
					mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				mouseTip.RuntimeParam.SetObject("CharacterInjuryDisplayData", A_1.displayData);
			}
		}

		// Token: 0x04008ACE RID: 35534
		[SerializeField]
		private TextMeshProUGUI textState;

		// Token: 0x04008ACF RID: 35535
		[SerializeField]
		private CImage progress;

		// Token: 0x04008AD0 RID: 35536
		[SerializeField]
		private CImage progressIncrease;

		// Token: 0x04008AD1 RID: 35537
		[SerializeField]
		private CImage progressReduce;

		// Token: 0x04008AD2 RID: 35538
		[SerializeField]
		private CButton button;

		// Token: 0x04008AD3 RID: 35539
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04008AD4 RID: 35540
		[SerializeField]
		private GameObject selected;

		// Token: 0x04008AD5 RID: 35541
		[SerializeField]
		private GameObject hover;

		// Token: 0x04008AD6 RID: 35542
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04008AD7 RID: 35543
		[SerializeField]
		private CButton sliderButton;

		// Token: 0x04008AD8 RID: 35544
		[SerializeField]
		private TooltipInvoker sliderTip;

		// Token: 0x04008AD9 RID: 35545
		private CharacterInjuryDisplayData _characterInjuryDisplayData;

		// Token: 0x04008ADA RID: 35546
		private int _charId;

		// Token: 0x04008ADB RID: 35547
		private int _health;
	}
}
