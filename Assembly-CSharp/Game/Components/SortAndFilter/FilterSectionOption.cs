using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C9F RID: 3231
	[RequireComponent(typeof(CToggle))]
	public class FilterSectionOption : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17001129 RID: 4393
		// (get) Token: 0x0600A467 RID: 42087 RVA: 0x004CC498 File Offset: 0x004CA698
		public CToggle Toggle
		{
			get
			{
				bool flag = this._toggle == null;
				if (flag)
				{
					this._toggle = base.GetComponent<CToggle>();
				}
				return this._toggle;
			}
		}

		// Token: 0x0600A468 RID: 42088 RVA: 0x004CC4CC File Offset: 0x004CA6CC
		private void Awake()
		{
			this._toggle = base.GetComponent<CToggle>();
			this._toggle.transition = Selectable.Transition.None;
			this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			bool flag = this.backgroundImage == null;
			if (flag)
			{
				this.backgroundImage = (this._toggle.targetGraphic as CImage);
			}
			bool flag2 = this.checkmarkImage == null;
			if (flag2)
			{
				this.checkmarkImage = (this._toggle.graphic as CImage);
			}
			this.RefreshVisual();
		}

		// Token: 0x0600A469 RID: 42089 RVA: 0x004CC563 File Offset: 0x004CA763
		private void OnEnable()
		{
			this.RefreshVisual();
		}

		// Token: 0x0600A46A RID: 42090 RVA: 0x004CC570 File Offset: 0x004CA770
		private void OnDestroy()
		{
			bool flag = this._toggle != null;
			if (flag)
			{
				this._toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
		}

		// Token: 0x0600A46B RID: 42091 RVA: 0x004CC5AB File Offset: 0x004CA7AB
		public void Refresh(string text, bool showTip = true)
		{
			this._optionText = text;
			this._showTip = showTip;
			this._cleanLabel = text;
			this.label.SetText(text, true);
			this.RefreshTipText();
		}

		// Token: 0x0600A46C RID: 42092 RVA: 0x004CC5D8 File Offset: 0x004CA7D8
		public void SetInteractable(bool interactable, string disabledTooltip = "")
		{
			this._disabledTooltip = (disabledTooltip ?? string.Empty);
			this.Toggle.interactable = interactable;
			this.RefreshTipText();
			this.RefreshVisual();
		}

		// Token: 0x0600A46D RID: 42093 RVA: 0x004CC606 File Offset: 0x004CA806
		public void SetIsOnWithoutNotify(bool isOn)
		{
			this.Toggle.SetIsOnWithoutNotify(isOn);
			this.RefreshVisual();
		}

		// Token: 0x0600A46E RID: 42094 RVA: 0x004CC61D File Offset: 0x004CA81D
		public void SetIsOn(bool isOn)
		{
			this.Toggle.isOn = isOn;
			this.RefreshVisual();
		}

		// Token: 0x0600A46F RID: 42095 RVA: 0x004CC634 File Offset: 0x004CA834
		public void SetCount(int count)
		{
			this.label.SetText(this._cleanLabel + ((count >= 100) ? "(...)" : string.Format("({0})", count)), true);
		}

		// Token: 0x1700112A RID: 4394
		// (get) Token: 0x0600A470 RID: 42096 RVA: 0x004CC66B File Offset: 0x004CA86B
		public bool IsOn
		{
			get
			{
				return this.Toggle.isOn;
			}
		}

		// Token: 0x0600A471 RID: 42097 RVA: 0x004CC678 File Offset: 0x004CA878
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._isHovered = true;
			this.RefreshVisual();
		}

		// Token: 0x0600A472 RID: 42098 RVA: 0x004CC689 File Offset: 0x004CA889
		public void OnPointerExit(PointerEventData eventData)
		{
			this._isHovered = false;
			this.RefreshVisual();
		}

		// Token: 0x0600A473 RID: 42099 RVA: 0x004CC69A File Offset: 0x004CA89A
		private void OnToggleValueChanged(bool _)
		{
			this.RefreshVisual();
		}

		// Token: 0x0600A474 RID: 42100 RVA: 0x004CC6A4 File Offset: 0x004CA8A4
		private void RefreshVisual()
		{
			bool flag = this.backgroundImage != null;
			if (flag)
			{
				this.backgroundImage.sprite = (this.Toggle.interactable ? this.unselectedNormalSprite : this.disabledSprite);
			}
			bool flag2 = this.checkmarkImage != null;
			if (flag2)
			{
				this.checkmarkImage.sprite = this.selectedNormalSprite;
			}
			bool flag3 = this.hoverImage == null;
			if (!flag3)
			{
				bool showHover = this._isHovered && this.Toggle.interactable;
				this.hoverImage.gameObject.SetActive(showHover);
				bool flag4 = !showHover;
				if (!flag4)
				{
					this.hoverImage.sprite = (this.Toggle.isOn ? this.selectedHoverSprite : this.unselectedHoverSprite);
				}
			}
		}

		// Token: 0x0600A475 RID: 42101 RVA: 0x004CC77C File Offset: 0x004CA97C
		private void RefreshTipText()
		{
			bool flag = this.tip == null || !this._showTip;
			if (!flag)
			{
				this.tip.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = this.tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				string tipText = (!this.Toggle.interactable && !string.IsNullOrEmpty(this._disabledTooltip)) ? this._disabledTooltip : this._optionText;
				this.tip.RuntimeParam.Set("arg0", tipText);
			}
		}

		// Token: 0x04008223 RID: 33315
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008224 RID: 33316
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04008225 RID: 33317
		[SerializeField]
		private CImage backgroundImage;

		// Token: 0x04008226 RID: 33318
		[SerializeField]
		private CImage checkmarkImage;

		// Token: 0x04008227 RID: 33319
		[SerializeField]
		private CImage hoverImage;

		// Token: 0x04008228 RID: 33320
		[SerializeField]
		private Sprite disabledSprite;

		// Token: 0x04008229 RID: 33321
		[SerializeField]
		private Sprite unselectedNormalSprite;

		// Token: 0x0400822A RID: 33322
		[SerializeField]
		private Sprite unselectedHoverSprite;

		// Token: 0x0400822B RID: 33323
		[SerializeField]
		private Sprite selectedNormalSprite;

		// Token: 0x0400822C RID: 33324
		[SerializeField]
		private Sprite selectedHoverSprite;

		// Token: 0x0400822D RID: 33325
		private CToggle _toggle;

		// Token: 0x0400822E RID: 33326
		private bool _isHovered;

		// Token: 0x0400822F RID: 33327
		private bool _showTip;

		// Token: 0x04008230 RID: 33328
		private string _optionText = string.Empty;

		// Token: 0x04008231 RID: 33329
		private string _disabledTooltip = string.Empty;

		// Token: 0x04008232 RID: 33330
		private string _cleanLabel = string.Empty;
	}
}
