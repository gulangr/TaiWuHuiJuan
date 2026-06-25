using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B7D RID: 2941
	public class PracticeSlice : MonoBehaviour
	{
		// Token: 0x17000FC1 RID: 4033
		// (get) Token: 0x06009154 RID: 37204 RVA: 0x0043C50F File Offset: 0x0043A70F
		private bool Interactable
		{
			get
			{
				return this.toggle.interactable;
			}
		}

		// Token: 0x06009155 RID: 37205 RVA: 0x0043C51C File Offset: 0x0043A71C
		public void SetPointerTriggerEnterEvent(UnityAction onPointerTriggerEnter)
		{
			this._onPointerTriggerEnter = onPointerTriggerEnter;
		}

		// Token: 0x06009156 RID: 37206 RVA: 0x0043C526 File Offset: 0x0043A726
		public void SetPointerTriggerExitEvent(UnityAction onPointerTriggerExit)
		{
			this._onPointerTriggerExit = onPointerTriggerExit;
		}

		// Token: 0x06009157 RID: 37207 RVA: 0x0043C530 File Offset: 0x0043A730
		public void SetSelected(bool selected)
		{
			this._isSelected = selected;
			this.RefreshStyle();
		}

		// Token: 0x06009158 RID: 37208 RVA: 0x0043C541 File Offset: 0x0043A741
		public void SetNameByActive(bool active)
		{
			this.nameLabel.SetSprite(active ? this._textIcon : "ui9_text_none_cn", false, null);
		}

		// Token: 0x06009159 RID: 37209 RVA: 0x0043C562 File Offset: 0x0043A762
		public void SetNameBgActive(bool active)
		{
			this.nameBg.transform.GetChild(0).gameObject.SetActive(active);
		}

		// Token: 0x0600915A RID: 37210 RVA: 0x0043C582 File Offset: 0x0043A782
		public void SetInteractable(bool interactable)
		{
			this.toggle.interactable = interactable;
			this.RefreshStyle();
		}

		// Token: 0x0600915B RID: 37211 RVA: 0x0043C599 File Offset: 0x0043A799
		public void SetPageShow(bool showDirectPage)
		{
			this.pageObj.SetActive(showDirectPage);
			this.noPageObj.SetActive(!showDirectPage);
			this.RefreshStyle();
		}

		// Token: 0x0600915C RID: 37212 RVA: 0x0043C5C0 File Offset: 0x0043A7C0
		public TooltipInvoker GetPageTip()
		{
			return this.pageObj.GetComponent<TooltipInvoker>();
		}

		// Token: 0x0600915D RID: 37213 RVA: 0x0043C5E0 File Offset: 0x0043A7E0
		public TooltipInvoker GetNoPageTip()
		{
			return this.noPageObj.GetComponent<TooltipInvoker>();
		}

		// Token: 0x0600915E RID: 37214 RVA: 0x0043C600 File Offset: 0x0043A800
		public void AutoRotateNameBg()
		{
			Quaternion rotation = base.GetComponent<RectTransform>().localRotation;
			this.nameBg.GetComponent<RectTransform>().localRotation = Quaternion.Inverse(rotation);
			this.num.GetComponent<RectTransform>().localRotation = Quaternion.Inverse(rotation);
		}

		// Token: 0x0600915F RID: 37215 RVA: 0x0043C648 File Offset: 0x0043A848
		public void SetNameLabel(string icon)
		{
			this._textIcon = icon;
			bool flag = null == this.nameLabel;
			if (!flag)
			{
				this.nameLabel.SetSprite(this._textIcon, false, null);
			}
		}

		// Token: 0x06009160 RID: 37216 RVA: 0x0043C683 File Offset: 0x0043A883
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerTriggerEnter));
			this.pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerTriggerExit));
		}

		// Token: 0x06009161 RID: 37217 RVA: 0x0043C6C0 File Offset: 0x0043A8C0
		private void OnDestroy()
		{
			this.pointerTrigger.EnterEvent.RemoveListener(new UnityAction(this.OnPointerTriggerEnter));
			this.pointerTrigger.ExitEvent.RemoveListener(new UnityAction(this.OnPointerTriggerExit));
		}

		// Token: 0x06009162 RID: 37218 RVA: 0x0043C6FD File Offset: 0x0043A8FD
		private void OnPointerTriggerEnter()
		{
			this._isPointerEnter = true;
			UnityAction onPointerTriggerEnter = this._onPointerTriggerEnter;
			if (onPointerTriggerEnter != null)
			{
				onPointerTriggerEnter();
			}
			this.RefreshStyle();
		}

		// Token: 0x06009163 RID: 37219 RVA: 0x0043C720 File Offset: 0x0043A920
		private void OnPointerTriggerExit()
		{
			this._isPointerEnter = false;
			UnityAction onPointerTriggerExit = this._onPointerTriggerExit;
			if (onPointerTriggerExit != null)
			{
				onPointerTriggerExit();
			}
			this.RefreshStyle();
		}

		// Token: 0x06009164 RID: 37220 RVA: 0x0043C744 File Offset: 0x0043A944
		public void RefreshStyle()
		{
			this.bg.sprite = (this._isSelected ? this.discNormal : this.discDisabled);
			bool flag = !this.Interactable && this.noPageObj.activeSelf;
			if (flag)
			{
				this.hover.SetActive(false);
				this.UpdateLabelStyle(this.labelDisabledStyle, this.labelDisabledColor);
				this.UpdateNameBg(this.disabledNameBg);
			}
			else
			{
				bool isSelected = this._isSelected;
				if (isSelected)
				{
					this.hover.SetActive(this.Interactable && this._isPointerEnter);
					this.UpdateLabelStyle(this.labelSelectedStyle, this.labelSelectedColor);
					this.UpdateNameBg(this.selectedNameBg);
				}
				else
				{
					bool flag2 = !this.Interactable;
					if (flag2)
					{
						this.hover.SetActive(false);
						this.UpdateLabelStyle(this.labelNormalStyle, this.labelNormalColor);
						this.UpdateNameBg(this.normalNameBg);
					}
					else
					{
						this.hover.SetActive(this._isPointerEnter);
						bool isPointerEnter = this._isPointerEnter;
						if (isPointerEnter)
						{
							this.UpdateLabelStyle(this.labelHoverStyle, this.labelHoverColor);
							this.UpdateNameBg(this.hoverNameBg);
						}
						else
						{
							this.UpdateLabelStyle(this.labelHoverStyle, this.labelHoverColor);
							this.UpdateNameBg(this.normalNameBg);
						}
					}
				}
			}
		}

		// Token: 0x06009165 RID: 37221 RVA: 0x0043C8AA File Offset: 0x0043AAAA
		public void SetNum(string index)
		{
			this.num.text = index;
		}

		// Token: 0x06009166 RID: 37222 RVA: 0x0043C8BA File Offset: 0x0043AABA
		public void SetReadingProgress(int val)
		{
			this.readingProgressMax.SetActive(val == 100);
			this.readingProgress.fillAmount = (float)val / 100f;
		}

		// Token: 0x06009167 RID: 37223 RVA: 0x0043C8E2 File Offset: 0x0043AAE2
		public void ShowReadingProgress()
		{
			this.readingProgress.transform.parent.gameObject.SetActive(true);
		}

		// Token: 0x06009168 RID: 37224 RVA: 0x0043C901 File Offset: 0x0043AB01
		public void HideReadingProgress()
		{
			this.readingProgress.transform.parent.gameObject.SetActive(false);
		}

		// Token: 0x06009169 RID: 37225 RVA: 0x0043C920 File Offset: 0x0043AB20
		private void UpdateLabelStyle(TextStyleData styleData, Color color)
		{
			bool flag = styleData != null;
			if (flag)
			{
				this.nameLabel.SetColor(styleData.fontColor);
			}
			bool flag2 = this.useLabelColor;
			if (flag2)
			{
				this.nameLabel.color = color;
			}
		}

		// Token: 0x0600916A RID: 37226 RVA: 0x0043C964 File Offset: 0x0043AB64
		private void UpdateNameBg(Sprite sprite)
		{
			this.nameBg.sprite = sprite;
		}

		// Token: 0x04006FF9 RID: 28665
		[SerializeField]
		private CImage bg;

		// Token: 0x04006FFA RID: 28666
		[SerializeField]
		private Sprite discNormal;

		// Token: 0x04006FFB RID: 28667
		[SerializeField]
		private Sprite discDisabled;

		// Token: 0x04006FFC RID: 28668
		[SerializeField]
		private CImage nameBg;

		// Token: 0x04006FFD RID: 28669
		[SerializeField]
		private CImage nameLabel;

		// Token: 0x04006FFE RID: 28670
		[SerializeField]
		private TextMeshProUGUI num;

		// Token: 0x04006FFF RID: 28671
		[SerializeField]
		private GameObject noPageObj;

		// Token: 0x04007000 RID: 28672
		[SerializeField]
		private GameObject pageObj;

		// Token: 0x04007001 RID: 28673
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04007002 RID: 28674
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04007003 RID: 28675
		[SerializeField]
		private GameObject hover;

		// Token: 0x04007004 RID: 28676
		[SerializeField]
		private CImage readingProgress;

		// Token: 0x04007005 RID: 28677
		[SerializeField]
		private GameObject readingProgressMax;

		// Token: 0x04007006 RID: 28678
		[SerializeField]
		private TextStyleData labelNormalStyle;

		// Token: 0x04007007 RID: 28679
		[SerializeField]
		private TextStyleData labelHoverStyle;

		// Token: 0x04007008 RID: 28680
		[SerializeField]
		private TextStyleData labelSelectedStyle;

		// Token: 0x04007009 RID: 28681
		[SerializeField]
		private TextStyleData labelDisabledStyle;

		// Token: 0x0400700A RID: 28682
		[Header("如果使用LabelColor，则在应用Preset后马上覆盖颜色")]
		[SerializeField]
		private bool useLabelColor;

		// Token: 0x0400700B RID: 28683
		[SerializeField]
		private Color labelNormalColor;

		// Token: 0x0400700C RID: 28684
		[SerializeField]
		private Color labelHoverColor;

		// Token: 0x0400700D RID: 28685
		[SerializeField]
		private Color labelSelectedColor;

		// Token: 0x0400700E RID: 28686
		[SerializeField]
		private Color labelDisabledColor;

		// Token: 0x0400700F RID: 28687
		[SerializeField]
		private Sprite normalNameBg;

		// Token: 0x04007010 RID: 28688
		[SerializeField]
		private Sprite hoverNameBg;

		// Token: 0x04007011 RID: 28689
		[SerializeField]
		private Sprite selectedNameBg;

		// Token: 0x04007012 RID: 28690
		[SerializeField]
		private Sprite disabledNameBg;

		// Token: 0x04007013 RID: 28691
		private UnityAction _onPointerTriggerEnter;

		// Token: 0x04007014 RID: 28692
		private UnityAction _onPointerTriggerExit;

		// Token: 0x04007015 RID: 28693
		private bool _isPointerEnter;

		// Token: 0x04007016 RID: 28694
		private bool _isSelected;

		// Token: 0x04007017 RID: 28695
		private string _textIcon;
	}
}
