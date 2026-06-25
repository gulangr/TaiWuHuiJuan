using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000331 RID: 817
public class CommonSwitch : CToggleObsolete
{
	// Token: 0x06002F54 RID: 12116 RVA: 0x001736DF File Offset: 0x001718DF
	protected override void Start()
	{
		base.Start();
		this.RefreshSwitchDisplay();
		this.UpdateButtonPosition();
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x001736F7 File Offset: 0x001718F7
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.RefreshSwitchDisplay();
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x00173709 File Offset: 0x00171909
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.RefreshSwitchDisplay();
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x0017371C File Offset: 0x0017191C
	public override void OnPointerClick(PointerEventData eventData)
	{
		bool flag = !this.interactable;
		if (!flag)
		{
			base.OnPointerClick(eventData);
			this.OnPointerEnter(eventData);
			this.AnimateSwitch();
		}
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x00173750 File Offset: 0x00171950
	private void RefreshSwitchDisplay()
	{
		this.UpdateBackgroundSprite();
		this.UpdateButtonSprite();
		this.UpdateButtonIcon();
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x00173768 File Offset: 0x00171968
	private void UpdateBackgroundSprite()
	{
		bool flag = this.backgroundImage == null;
		if (!flag)
		{
			bool flag2 = !this.interactable;
			if (flag2)
			{
				this.backgroundImage.sprite = this.backgroundDisabledSprite;
			}
			else
			{
				bool isOn = this.isOn;
				if (isOn)
				{
					this.backgroundImage.sprite = (base.Hovering ? this.backgroundOnHoverSprite : this.backgroundOnNormalSprite);
				}
				else
				{
					this.backgroundImage.sprite = (base.Hovering ? this.backgroundOffHoverSprite : this.backgroundOffNormalSprite);
				}
			}
		}
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x00173800 File Offset: 0x00171A00
	private void UpdateButtonSprite()
	{
		bool flag = this.buttonImage == null;
		if (!flag)
		{
			bool flag2 = !this.interactable;
			if (flag2)
			{
				this.buttonImage.sprite = this.buttonDisabledSprite;
			}
			else
			{
				this.buttonImage.sprite = (base.Hovering ? this.buttonHoverSprite : this.buttonNormalSprite);
			}
		}
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x00173868 File Offset: 0x00171A68
	private void UpdateButtonIcon()
	{
		bool flag = this.buttonIcon == null;
		if (!flag)
		{
			bool flag2 = !this.interactable;
			if (flag2)
			{
				this.buttonIcon.sprite = (this.isOn ? this.buttonIconOnDisabledSprite : this.buttonIconOffDisabledSprite);
			}
			else
			{
				bool isOn = this.isOn;
				if (isOn)
				{
					this.buttonIcon.sprite = (base.Hovering ? this.buttonIconOnHoverSprite : this.buttonIconOnNormalSprite);
				}
				else
				{
					this.buttonIcon.sprite = (base.Hovering ? this.buttonIconOffHoverSprite : this.buttonIconOffNormalSprite);
				}
			}
		}
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x00173914 File Offset: 0x00171B14
	private void UpdateButtonPosition()
	{
		bool flag = this.buttonImage == null;
		if (!flag)
		{
			Vector2 targetPosition = this.isOn ? this.buttonImageOnPosition : this.buttonImageOffPosition;
			this.buttonImage.rectTransform.anchoredPosition = targetPosition;
		}
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x00173960 File Offset: 0x00171B60
	private void AnimateSwitch()
	{
		bool flag = this.buttonImage == null;
		if (!flag)
		{
			this.buttonImage.rectTransform.DOKill(true);
			bool flag2 = this.animationDuration > 0f;
			if (flag2)
			{
				this.buttonImage.rectTransform.DOAnchorPos(this.isOn ? this.buttonImageOnPosition : this.buttonImageOffPosition, this.animationDuration, false).SetEase(Ease.OutExpo).OnUpdate(new TweenCallback(this.RefreshSwitchDisplay));
			}
			else
			{
				this.buttonImage.rectTransform.anchoredPosition = (this.isOn ? this.buttonImageOnPosition : this.buttonImageOffPosition);
				this.RefreshSwitchDisplay();
			}
		}
	}

	// Token: 0x17000534 RID: 1332
	// (get) Token: 0x06002F5E RID: 12126 RVA: 0x00173A20 File Offset: 0x00171C20
	// (set) Token: 0x06002F5F RID: 12127 RVA: 0x00173A28 File Offset: 0x00171C28
	public override bool interactable
	{
		get
		{
			return base.interactable;
		}
		set
		{
			base.interactable = value;
			this.RefreshSwitchDisplay();
		}
	}

	// Token: 0x17000535 RID: 1333
	// (get) Token: 0x06002F60 RID: 12128 RVA: 0x00173A3A File Offset: 0x00171C3A
	// (set) Token: 0x06002F61 RID: 12129 RVA: 0x00173A44 File Offset: 0x00171C44
	public override bool isOn
	{
		get
		{
			return base.isOn;
		}
		set
		{
			bool flag = base.isOn == value;
			if (!flag)
			{
				base.isOn = value;
				this.RefreshSwitchDisplay();
				this.AnimateSwitch();
			}
		}
	}

	// Token: 0x04002263 RID: 8803
	[SerializeField]
	private CImage backgroundImage;

	// Token: 0x04002264 RID: 8804
	[SerializeField]
	private CImage buttonImage;

	// Token: 0x04002265 RID: 8805
	[SerializeField]
	private CImage buttonIcon;

	// Token: 0x04002266 RID: 8806
	[SerializeField]
	private float animationDuration = 0.1f;

	// Token: 0x04002267 RID: 8807
	[SerializeField]
	private Sprite backgroundOffNormalSprite;

	// Token: 0x04002268 RID: 8808
	[SerializeField]
	private Sprite backgroundOffHoverSprite;

	// Token: 0x04002269 RID: 8809
	[SerializeField]
	private Sprite backgroundDisabledSprite;

	// Token: 0x0400226A RID: 8810
	[SerializeField]
	private Sprite backgroundOnNormalSprite;

	// Token: 0x0400226B RID: 8811
	[SerializeField]
	private Sprite backgroundOnHoverSprite;

	// Token: 0x0400226C RID: 8812
	[SerializeField]
	private Sprite buttonNormalSprite;

	// Token: 0x0400226D RID: 8813
	[SerializeField]
	private Sprite buttonHoverSprite;

	// Token: 0x0400226E RID: 8814
	[SerializeField]
	private Sprite buttonDisabledSprite;

	// Token: 0x0400226F RID: 8815
	[SerializeField]
	private Sprite buttonIconOffNormalSprite;

	// Token: 0x04002270 RID: 8816
	[SerializeField]
	private Sprite buttonIconOffHoverSprite;

	// Token: 0x04002271 RID: 8817
	[SerializeField]
	private Sprite buttonIconOffDisabledSprite;

	// Token: 0x04002272 RID: 8818
	[SerializeField]
	private Sprite buttonIconOnNormalSprite;

	// Token: 0x04002273 RID: 8819
	[SerializeField]
	private Sprite buttonIconOnHoverSprite;

	// Token: 0x04002274 RID: 8820
	[SerializeField]
	private Sprite buttonIconOnDisabledSprite;

	// Token: 0x04002275 RID: 8821
	[SerializeField]
	private Vector2 buttonImageOnPosition;

	// Token: 0x04002276 RID: 8822
	[SerializeField]
	private Vector2 buttonImageOffPosition;
}
