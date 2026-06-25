using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000346 RID: 838
public class CommonToggleState : Refers
{
	// Token: 0x17000561 RID: 1377
	// (get) Token: 0x06003106 RID: 12550 RVA: 0x001804B0 File Offset: 0x0017E6B0
	public CommonToggleState.ToggleStates CurrState
	{
		get
		{
			bool flag = !this.toggle.enabled;
			CommonToggleState.ToggleStates result;
			if (flag)
			{
				result = CommonToggleState.ToggleStates.Disable;
			}
			else
			{
				bool flag2 = !this.toggle.interactable;
				if (flag2)
				{
					result = CommonToggleState.ToggleStates.NonInteract;
				}
				else
				{
					bool isOn = this.toggle.isOn;
					if (isOn)
					{
						result = CommonToggleState.ToggleStates.Highlight;
					}
					else
					{
						result = (this.toggle.Hovering ? CommonToggleState.ToggleStates.Hover : CommonToggleState.ToggleStates.Normal);
					}
				}
			}
			return result;
		}
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x00180514 File Offset: 0x0017E714
	public void Awake()
	{
		this.toggle.OnInteractableChange.AddListener(new UnityAction<bool>(this.OnStateChanged));
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnStateChanged));
		this.toggle.OnEnabledChange.AddListener(new UnityAction<bool>(this.OnStateChanged));
		this.toggle.OnHoveringChange.AddListener(new UnityAction<bool>(this.OnStateChanged));
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x00180598 File Offset: 0x0017E798
	public void OnEnable()
	{
		this._hasHover = (this.hover != null);
		this._hasHighlight = (this.highLight != null);
		this._hasDisable = (this.disableStyle != null);
		this._hasNonInteract = (this.nonInteract != null);
		this._hasMouseTip = (this.mouseTip != null);
		this.OnStateChanged();
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x00180608 File Offset: 0x0017E808
	public virtual void OnStateChanged()
	{
		this.InitializeStateDisplay();
		switch (this.CurrState)
		{
		case CommonToggleState.ToggleStates.Hover:
		{
			bool hasHover = this._hasHover;
			if (hasHover)
			{
				this.hover.SetActive(true);
			}
			break;
		}
		case CommonToggleState.ToggleStates.Highlight:
		{
			bool hasHighlight = this._hasHighlight;
			if (hasHighlight)
			{
				this.highLight.SetActive(true);
			}
			break;
		}
		case CommonToggleState.ToggleStates.Disable:
		{
			bool hasDisable = this._hasDisable;
			if (hasDisable)
			{
				this.disableStyle.enabled = true;
			}
			bool hasMouseTip = this._hasMouseTip;
			if (hasMouseTip)
			{
				this.mouseTip.enabled = false;
			}
			break;
		}
		case CommonToggleState.ToggleStates.NonInteract:
		{
			bool hasNonInteract = this._hasNonInteract;
			if (hasNonInteract)
			{
				this.nonInteract.SetActive(true);
			}
			break;
		}
		}
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x001806C8 File Offset: 0x0017E8C8
	public virtual void InitializeStateDisplay()
	{
		bool hasHover = this._hasHover;
		if (hasHover)
		{
			this.hover.SetActive(false);
		}
		bool hasNonInteract = this._hasNonInteract;
		if (hasNonInteract)
		{
			this.nonInteract.SetActive(false);
		}
		bool hasHighlight = this._hasHighlight;
		if (hasHighlight)
		{
			this.highLight.SetActive(false);
		}
		bool hasDisable = this._hasDisable;
		if (hasDisable)
		{
			this.disableStyle.enabled = false;
		}
		bool hasMouseTip = this._hasMouseTip;
		if (hasMouseTip)
		{
			this.mouseTip.enabled = true;
		}
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x0018074B File Offset: 0x0017E94B
	private void OnStateChanged(bool value)
	{
		this.OnStateChanged();
	}

	// Token: 0x040023E1 RID: 9185
	public GameObject hover;

	// Token: 0x040023E2 RID: 9186
	public GameObject nonInteract;

	// Token: 0x040023E3 RID: 9187
	public GameObject highLight;

	// Token: 0x040023E4 RID: 9188
	public TooltipInvoker mouseTip;

	// Token: 0x040023E5 RID: 9189
	public HSVStyleRoot disableStyle;

	// Token: 0x040023E6 RID: 9190
	public CToggleObsolete toggle;

	// Token: 0x040023E7 RID: 9191
	private bool _hasHover;

	// Token: 0x040023E8 RID: 9192
	private bool _hasHighlight;

	// Token: 0x040023E9 RID: 9193
	private bool _hasDisable;

	// Token: 0x040023EA RID: 9194
	private bool _hasNonInteract;

	// Token: 0x040023EB RID: 9195
	private bool _hasMouseTip;

	// Token: 0x020016D5 RID: 5845
	public enum ToggleStates
	{
		// Token: 0x0400A937 RID: 43319
		Normal,
		// Token: 0x0400A938 RID: 43320
		Hover,
		// Token: 0x0400A939 RID: 43321
		Highlight,
		// Token: 0x0400A93A RID: 43322
		Disable,
		// Token: 0x0400A93B RID: 43323
		NonInteract
	}
}
