using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000344 RID: 836
public class CommonSortMenuBar : MonoBehaviour
{
	// Token: 0x1700055E RID: 1374
	// (get) Token: 0x060030EB RID: 12523 RVA: 0x0017FFC7 File Offset: 0x0017E1C7
	private TextStyle TextStyle
	{
		get
		{
			return this.label.GetComponent<TextStyle>();
		}
	}

	// Token: 0x1700055F RID: 1375
	// (get) Token: 0x060030EC RID: 12524 RVA: 0x0017FFD4 File Offset: 0x0017E1D4
	private HSVStyleRoot HsvStyleRoot
	{
		get
		{
			return base.GetComponent<HSVStyleRoot>();
		}
	}

	// Token: 0x17000560 RID: 1376
	// (get) Token: 0x060030ED RID: 12525 RVA: 0x0017FFDC File Offset: 0x0017E1DC
	private bool IsButtonInteractable
	{
		get
		{
			return this.button.interactable;
		}
	}

	// Token: 0x060030EE RID: 12526 RVA: 0x0017FFE9 File Offset: 0x0017E1E9
	public void SetInteractable(bool interactable)
	{
		this.button.interactable = interactable;
		this.RefreshStyle();
	}

	// Token: 0x060030EF RID: 12527 RVA: 0x00180000 File Offset: 0x0017E200
	public void SetSelected(bool selected)
	{
		bool flag = this._isSelected == selected;
		if (!flag)
		{
			this._isSelected = selected;
			this.RefreshStyle();
		}
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x0018002B File Offset: 0x0017E22B
	public void UpdateLabel(string text)
	{
		this.label.text = text;
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x0018003B File Offset: 0x0017E23B
	public void UpdateStatusIcon(bool isDown)
	{
		CommonSortMenuBar.UpdateIconDirection(this.statusIcon, isDown);
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x0018004C File Offset: 0x0017E24C
	public void UpdateSortDirectionIcon(bool isActive, bool isDown)
	{
		this.directionIcon.gameObject.SetActive(isActive);
		bool flag = !isActive;
		if (!flag)
		{
			CommonSortMenuBar.UpdateIconDirection(this.directionIcon, isDown);
		}
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x00180083 File Offset: 0x0017E283
	private static void UpdateIconDirection(CImage icon, bool isDown)
	{
		icon.rectTransform.localRotation = (isDown ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 0f, 180f));
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x001800BF File Offset: 0x0017E2BF
	public void AddEnterEvent(UnityAction action)
	{
		this.pointerTrigger.EnterEvent.AddListener(action);
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x001800D4 File Offset: 0x0017E2D4
	public void AddExitEvent(UnityAction action)
	{
		this.pointerTrigger.ExitEvent.AddListener(action);
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x001800E9 File Offset: 0x0017E2E9
	public void RemoveEnterEvent(UnityAction action)
	{
		this.pointerTrigger.EnterEvent.RemoveListener(action);
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x001800FE File Offset: 0x0017E2FE
	public void RemoveExitEvent(UnityAction action)
	{
		this.pointerTrigger.ExitEvent.RemoveListener(action);
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x00180114 File Offset: 0x0017E314
	private void Awake()
	{
		this.button.OnInteractableChange.AddListener(new UnityAction<bool>(this.OnButtonInteractableChange));
		this.pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerTriggerEnter));
		this.pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerTriggerExit));
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x0018017C File Offset: 0x0017E37C
	private void OnDestroy()
	{
		this.button.OnInteractableChange.RemoveListener(new UnityAction<bool>(this.OnButtonInteractableChange));
		this.pointerTrigger.EnterEvent.RemoveListener(new UnityAction(this.OnPointerTriggerEnter));
		this.pointerTrigger.ExitEvent.RemoveListener(new UnityAction(this.OnPointerTriggerExit));
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x001801E1 File Offset: 0x0017E3E1
	private void OnButtonInteractableChange(bool interactable)
	{
		this.RefreshStyle();
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x001801EB File Offset: 0x0017E3EB
	private void OnPointerTriggerEnter()
	{
		this._isPointerEnter = true;
		this.RefreshStyle();
	}

	// Token: 0x060030FC RID: 12540 RVA: 0x001801FC File Offset: 0x0017E3FC
	private void OnPointerTriggerExit()
	{
		this._isPointerEnter = false;
		this.RefreshStyle();
	}

	// Token: 0x060030FD RID: 12541 RVA: 0x00180210 File Offset: 0x0017E410
	private void RefreshStyle()
	{
		bool flag = !this.IsButtonInteractable;
		if (flag)
		{
			this.UpdateLabelStyle(this.labelNormalStyle);
			this.UpdateBg(this.disabledBg);
			this.HsvStyleRoot.SetDefaultGrayAndBlack();
		}
		else
		{
			this.HsvStyleRoot.SetDefault();
			bool isSelected = this._isSelected;
			if (isSelected)
			{
				this.UpdateLabelStyle(this.labelSelectedStyle);
				this.UpdateBg(this.selectedBg);
			}
			else
			{
				bool isPointerEnter = this._isPointerEnter;
				if (isPointerEnter)
				{
					this.UpdateLabelStyle(this.labelHoverStyle);
					this.UpdateBg(this.hoverBg);
				}
				else
				{
					this.UpdateLabelStyle(this.labelNormalStyle);
					this.UpdateBg(this.normalBg);
				}
			}
		}
	}

	// Token: 0x060030FE RID: 12542 RVA: 0x001802C8 File Offset: 0x0017E4C8
	private void UpdateBg(Sprite sprite)
	{
		this.bg.sprite = sprite;
	}

	// Token: 0x060030FF RID: 12543 RVA: 0x001802D8 File Offset: 0x0017E4D8
	private void UpdateLabelStyle(TextStyleData styleData)
	{
		bool flag = styleData != null;
		if (flag)
		{
			styleData.ApplyTo(this.label);
		}
	}

	// Token: 0x040023CD RID: 9165
	[SerializeField]
	private CButtonObsolete button;

	// Token: 0x040023CE RID: 9166
	[SerializeField]
	private PointerTrigger pointerTrigger;

	// Token: 0x040023CF RID: 9167
	[SerializeField]
	private TextMeshProUGUI label;

	// Token: 0x040023D0 RID: 9168
	[SerializeField]
	private CImage bg;

	// Token: 0x040023D1 RID: 9169
	[SerializeField]
	private CImage statusIcon;

	// Token: 0x040023D2 RID: 9170
	[SerializeField]
	private CImage directionIcon;

	// Token: 0x040023D3 RID: 9171
	[SerializeField]
	private TextStyleData labelNormalStyle;

	// Token: 0x040023D4 RID: 9172
	[SerializeField]
	private TextStyleData labelHoverStyle;

	// Token: 0x040023D5 RID: 9173
	[SerializeField]
	private TextStyleData labelSelectedStyle;

	// Token: 0x040023D6 RID: 9174
	[SerializeField]
	private Sprite normalBg;

	// Token: 0x040023D7 RID: 9175
	[SerializeField]
	private Sprite hoverBg;

	// Token: 0x040023D8 RID: 9176
	[SerializeField]
	private Sprite selectedBg;

	// Token: 0x040023D9 RID: 9177
	[SerializeField]
	private Sprite disabledBg;

	// Token: 0x040023DA RID: 9178
	private bool _isPointerEnter;

	// Token: 0x040023DB RID: 9179
	private bool _isSelected;
}
