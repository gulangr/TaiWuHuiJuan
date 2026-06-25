using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000D4 RID: 212
[Obsolete]
public class CToggleObsolete : Toggle
{
	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000791 RID: 1937 RVA: 0x00035509 File Offset: 0x00033709
	// (set) Token: 0x06000792 RID: 1938 RVA: 0x00035511 File Offset: 0x00033711
	public bool Hovering
	{
		get
		{
			return this._hovering;
		}
		set
		{
			this._hovering = value;
			UnityEvent<bool> onHoveringChange = this.OnHoveringChange;
			if (onHoveringChange != null)
			{
				onHoveringChange.Invoke(value);
			}
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000793 RID: 1939 RVA: 0x0003552E File Offset: 0x0003372E
	// (set) Token: 0x06000794 RID: 1940 RVA: 0x00035538 File Offset: 0x00033738
	public new virtual bool interactable
	{
		get
		{
			return base.interactable;
		}
		set
		{
			bool flag = null == base.gameObject;
			if (!flag)
			{
				base.interactable = value;
				Toggle.ToggleEvent onInteractableChange = this.OnInteractableChange;
				if (onInteractableChange != null)
				{
					onInteractableChange.Invoke(value);
				}
				this.RefreshLabelDisplay();
				Toggle.ToggleEvent onInteractableChangeReverse = this.OnInteractableChangeReverse;
				if (onInteractableChangeReverse != null)
				{
					onInteractableChangeReverse.Invoke(!value);
				}
			}
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000795 RID: 1941 RVA: 0x00035590 File Offset: 0x00033790
	// (set) Token: 0x06000796 RID: 1942 RVA: 0x00035598 File Offset: 0x00033798
	public new virtual bool isOn
	{
		get
		{
			return base.isOn;
		}
		set
		{
			bool flag = null == base.gameObject;
			if (!flag)
			{
				base.isOn = value;
				this.RefreshLabelDisplay();
			}
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000797 RID: 1943 RVA: 0x000355C7 File Offset: 0x000337C7
	// (set) Token: 0x06000798 RID: 1944 RVA: 0x000355D0 File Offset: 0x000337D0
	public new virtual bool enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			bool flag = null == base.gameObject;
			if (!flag)
			{
				base.enabled = value;
				UnityEvent<bool> onEnabledChange = this.OnEnabledChange;
				if (onEnabledChange != null)
				{
					onEnabledChange.Invoke(value);
				}
			}
		}
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0003560B File Offset: 0x0003380B
	protected override void OnEnable()
	{
		this.RefreshLabelDisplay();
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x00035615 File Offset: 0x00033815
	protected override void OnDisable()
	{
		this.Hovering = false;
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x00035620 File Offset: 0x00033820
	public void Register(CToggleGroupObsolete tg)
	{
		this._toggleGroup = tg;
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0003562A File Offset: 0x0003382A
	public void UnRegister()
	{
		this._toggleGroup = null;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00035634 File Offset: 0x00033834
	private void CheckPointerPerformExit()
	{
		bool flag = this.isOn && !this.ignoreHoveringCheckWhenClicking;
		if (flag)
		{
			this.OnPointerExit(null);
		}
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00035664 File Offset: 0x00033864
	public override void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.button > PointerEventData.InputButton.Left;
		if (!flag)
		{
			bool flag2 = !this.interactable;
			if (flag2)
			{
				this.PlayButtonAudio();
			}
			else
			{
				bool flag3 = !UGUIUtils.IsScreenAreaInteract();
				if (!flag3)
				{
					ConchShipCursor.Instance.SetDefaultCursor();
					bool flag4 = this._toggleGroup != null;
					if (flag4)
					{
						bool flag5 = !this._toggleGroup.ValidateStateChange(this, this.isOn);
						if (!flag5)
						{
							base.OnPointerClick(eventData);
							this.CheckPointerPerformExit();
							this._toggleGroup.NotifyToggle(this, this.isOn, true);
							this.PlayButtonAudio();
						}
					}
					else
					{
						this.isOn = !this.isOn;
						this.CheckPointerPerformExit();
						this.PlayButtonAudio();
					}
				}
			}
		}
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x00035734 File Offset: 0x00033934
	private void RefreshLabelDisplay()
	{
		List<TextMeshProUGUI> labelList = this.LabelList;
		bool flag = labelList == null || labelList.Count <= 0;
		if (!flag)
		{
			bool bypassCheckInteractable = this.LabelList.Count < 3 || this.LabelList[2] == null;
			bool flag2 = this.LabelList.Count < 2 || this.LabelList[1] == null || this.LabelList[0] == null;
			if (!flag2)
			{
				this.LabelList[0].gameObject.SetActive((bypassCheckInteractable || this.interactable) && !this.isOn && !this.Hovering);
				this.LabelList[1].gameObject.SetActive((bypassCheckInteractable || this.interactable) && (this.isOn || this.Hovering));
				bool flag3 = this.LabelList.Count < 3 || this.LabelList[2] == null;
				if (!flag3)
				{
					this.LabelList[2].gameObject.SetActive(!this.interactable);
				}
			}
		}
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00035880 File Offset: 0x00033A80
	private void PlayButtonAudio()
	{
		bool interactable = this.interactable;
		if (interactable)
		{
			bool flag = !string.IsNullOrEmpty(this.ClickAudioKey);
			if (flag)
			{
				AudioManager.Instance.PlaySound(this.ClickAudioKey, false, false);
			}
			else
			{
				AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			}
		}
		else
		{
			bool flag2 = !string.IsNullOrEmpty(this.DisableClickAudioKey);
			if (flag2)
			{
				AudioManager.Instance.PlaySound(this.DisableClickAudioKey, false, false);
			}
			else
			{
				AudioManager.Instance.PlaySound("ui_default_click_fail", false, false);
			}
		}
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x00035910 File Offset: 0x00033B10
	public override void OnPointerEnter(PointerEventData eventData)
	{
		this.Hovering = true;
		this.RefreshLabelDisplay();
		bool flag = !UGUIUtils.IsScreenAreaInteract() || !this.interactable;
		if (!flag)
		{
			base.OnPointerEnter(eventData);
		}
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x00035950 File Offset: 0x00033B50
	public override void OnPointerExit(PointerEventData eventData)
	{
		this.Hovering = false;
		this.RefreshLabelDisplay();
		bool flag = !UGUIUtils.IsScreenAreaInteract() || !this.interactable;
		if (!flag)
		{
			base.OnPointerExit(eventData);
		}
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x00035990 File Offset: 0x00033B90
	public void SetLabelContent(string content)
	{
		bool flag = this.LabelList == null || this.LabelList.Count == 0;
		if (!flag)
		{
			foreach (TextMeshProUGUI label in this.LabelList)
			{
				label.text = content;
			}
		}
	}

	// Token: 0x040007B8 RID: 1976
	public int Key;

	// Token: 0x040007B9 RID: 1977
	public string ClickAudioKey;

	// Token: 0x040007BA RID: 1978
	public string DisableClickAudioKey = "ui_default_click_fail";

	// Token: 0x040007BB RID: 1979
	public string LabelLanguageKey;

	// Token: 0x040007BC RID: 1980
	[SerializeField]
	public List<TextMeshProUGUI> LabelList;

	// Token: 0x040007BD RID: 1981
	public Toggle.ToggleEvent OnInteractableChange;

	// Token: 0x040007BE RID: 1982
	public Toggle.ToggleEvent OnInteractableChangeReverse;

	// Token: 0x040007BF RID: 1983
	public UnityEvent<bool> OnEnabledChange;

	// Token: 0x040007C0 RID: 1984
	public UnityEvent<bool> OnHoveringChange;

	// Token: 0x040007C1 RID: 1985
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x040007C2 RID: 1986
	public bool ignoreHoveringCheckWhenClicking = false;

	// Token: 0x040007C3 RID: 1987
	private bool _hovering;
}
