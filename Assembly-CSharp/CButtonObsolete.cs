using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020000C7 RID: 199
[Obsolete]
public class CButtonObsolete : Button
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060006D7 RID: 1751 RVA: 0x00030193 File Offset: 0x0002E393
	// (set) Token: 0x060006D8 RID: 1752 RVA: 0x0003019C File Offset: 0x0002E39C
	public new bool interactable
	{
		get
		{
			return base.interactable;
		}
		set
		{
			bool flag = CanvasUpdateRegistry.IsRebuildingLayout() || CanvasUpdateRegistry.IsRebuildingGraphics();
			if (flag)
			{
				Debug.LogWarning("CButton.interactable 在 Canvas 重建期间被设置，可能导致异常。GameObject: " + base.gameObject.name);
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					this._recordedInteractable = value;
					base.StartCoroutine(this.DelaySetInteractable());
				}
			}
			else
			{
				base.interactable = value;
				CButtonObsolete.ButtonEvent onInteractableChange = this.OnInteractableChange;
				if (onInteractableChange != null)
				{
					onInteractableChange.Invoke(value);
				}
				this.OnInteractableChangeInternal(value);
				CButtonObsolete.ButtonEvent onInteractableChangeReverse = this.OnInteractableChangeReverse;
				if (onInteractableChangeReverse != null)
				{
					onInteractableChangeReverse.Invoke(!value);
				}
			}
		}
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00030239 File Offset: 0x0002E439
	private IEnumerator DelaySetInteractable()
	{
		yield return null;
		base.interactable = this._recordedInteractable;
		CButtonObsolete.ButtonEvent onInteractableChange = this.OnInteractableChange;
		if (onInteractableChange != null)
		{
			onInteractableChange.Invoke(this._recordedInteractable);
		}
		this.OnInteractableChangeInternal(this._recordedInteractable);
		CButtonObsolete.ButtonEvent onInteractableChangeReverse = this.OnInteractableChangeReverse;
		if (onInteractableChangeReverse != null)
		{
			onInteractableChangeReverse.Invoke(!this._recordedInteractable);
		}
		yield break;
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00030248 File Offset: 0x0002E448
	protected virtual void OnInteractableChangeInternal(bool value)
	{
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060006DB RID: 1755 RVA: 0x0003024B File Offset: 0x0002E44B
	// (set) Token: 0x060006DC RID: 1756 RVA: 0x00030253 File Offset: 0x0002E453
	public CButtonObsolete.State SelectState { get; private set; }

	// Token: 0x060006DD RID: 1757 RVA: 0x0003025C File Offset: 0x0002E45C
	protected override void Awake()
	{
		base.Awake();
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			this._startScale = base.transform.localScale;
			this._pointerTrigger = base.GetComponent<PointerTrigger>();
		}
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x0003029C File Offset: 0x0002E49C
	protected override void OnDisable()
	{
		base.OnDisable();
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			Tweener scaleTweener = this._scaleTweener;
			if (scaleTweener != null)
			{
				scaleTweener.Complete();
			}
			bool usePressScale = this.UsePressScale;
			if (usePressScale)
			{
				base.transform.localScale = this._startScale;
			}
			this._scaleTweener = null;
		}
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x000302F4 File Offset: 0x0002E4F4
	public void ClearAndAddListener(Action action)
	{
		base.onClick.RemoveAllListeners();
		bool flag = action == null;
		if (!flag)
		{
			base.onClick.AddListener(new UnityAction(action.Invoke));
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00030330 File Offset: 0x0002E530
	public override void OnPointerClick(PointerEventData eventData)
	{
		bool flag = !UGUIUtils.IsScreenAreaInteract();
		if (!flag)
		{
			bool flag2 = eventData.button == PointerEventData.InputButton.Left && this.EnableClickAudio;
			if (flag2)
			{
				bool interactable = this.interactable;
				if (interactable)
				{
					bool flag3 = !string.IsNullOrEmpty(this.ClickAudioKey);
					if (flag3)
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
					bool flag4 = !string.IsNullOrEmpty(this.DisableClickAudioKey);
					if (flag4)
					{
						AudioManager.Instance.PlaySound(this.DisableClickAudioKey, false, false);
					}
					else
					{
						AudioManager.Instance.PlaySound("ui_default_click_fail", false, false);
					}
				}
			}
			base.OnPointerClick(eventData);
			bool flag5 = eventData.button > PointerEventData.InputButton.Left;
			if (!flag5)
			{
				bool flag6 = this._pointerTrigger && this._pointerTrigger.enabled;
				if (flag6)
				{
					bool needExecutePointerExitEventOnClick = this.NeedExecutePointerExitEventOnClick;
					if (needExecutePointerExitEventOnClick)
					{
						this._pointerTrigger.OnPointerExit(eventData);
					}
					bool needExecutePointerEnterEventOnClick = this.NeedExecutePointerEnterEventOnClick;
					if (needExecutePointerEnterEventOnClick)
					{
						this._pointerTrigger.OnPointerEnter(eventData);
					}
				}
			}
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x0003045C File Offset: 0x0002E65C
	public override void OnDeselect(BaseEventData eventData)
	{
		bool flag = !UGUIUtils.IsScreenAreaInteract();
		if (!flag)
		{
			base.OnDeselect(eventData);
			this.SelectState = CButtonObsolete.State.Deselect;
			Action<CButtonObsolete.State> onSelectStateChange = this.OnSelectStateChange;
			if (onSelectStateChange != null)
			{
				onSelectStateChange(this.SelectState);
			}
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x000304A4 File Offset: 0x0002E6A4
	public override void OnSelect(BaseEventData eventData)
	{
		bool flag = !UGUIUtils.IsScreenAreaInteract();
		if (!flag)
		{
			base.OnSelect(eventData);
			this.SelectState = CButtonObsolete.State.Select;
			Action<CButtonObsolete.State> onSelectStateChange = this.OnSelectStateChange;
			if (onSelectStateChange != null)
			{
				onSelectStateChange(this.SelectState);
			}
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x000304EC File Offset: 0x0002E6EC
	public override void OnPointerDown(PointerEventData eventData)
	{
		bool flag = !UGUIUtils.IsScreenAreaInteract();
		if (!flag)
		{
			bool flag2 = eventData.button > PointerEventData.InputButton.Left;
			if (!flag2)
			{
				base.OnPointerDown(eventData);
				this.SelectState = CButtonObsolete.State.Select;
				Action<CButtonObsolete.State> onSelectStateChange = this.OnSelectStateChange;
				if (onSelectStateChange != null)
				{
					onSelectStateChange(this.SelectState);
				}
				bool flag3 = this._scaleTweener != null;
				if (!flag3)
				{
					bool usePressScale = this.UsePressScale;
					if (usePressScale)
					{
						this._scaleTweener = DOVirtual.Float(0f, 1f, this.ScaleInterval, delegate(float stepValue)
						{
							base.transform.localScale = Vector3.Lerp(this._startScale, this.ScaleToValue, stepValue);
						}).SetAutoKill(true).OnComplete(delegate
						{
							this._scaleTweener = null;
						});
					}
				}
			}
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000305A0 File Offset: 0x0002E7A0
	public override void OnPointerUp(PointerEventData eventData)
	{
		bool flag = !UGUIUtils.IsScreenAreaInteract();
		if (!flag)
		{
			bool flag2 = eventData.button > PointerEventData.InputButton.Left;
			if (!flag2)
			{
				base.OnPointerUp(eventData);
				this.SelectState = CButtonObsolete.State.Deselect;
				Action<CButtonObsolete.State> onSelectStateChange = this.OnSelectStateChange;
				if (onSelectStateChange != null)
				{
					onSelectStateChange(this.SelectState);
				}
				Tweener scaleTweener = this._scaleTweener;
				if (scaleTweener != null)
				{
					scaleTweener.Complete(true);
				}
				bool usePressScale = this.UsePressScale;
				if (usePressScale)
				{
					this._scaleTweener = DOVirtual.Float(1f, 0f, this.ScaleInterval, delegate(float stepValue)
					{
						base.transform.localScale = Vector3.Lerp(this._startScale, this.ScaleToValue, stepValue);
					}).SetAutoKill(true).OnComplete(delegate
					{
						this._scaleTweener = null;
					});
				}
			}
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00030658 File Offset: 0x0002E858
	public override void OnPointerEnter(PointerEventData eventData)
	{
		bool flag = !UGUIUtils.IsScreenAreaInteract();
		if (!flag)
		{
			base.OnPointerEnter(eventData);
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00030680 File Offset: 0x0002E880
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

	// Token: 0x0400075C RID: 1884
	public bool AutoListen;

	// Token: 0x0400075D RID: 1885
	public string ClickAudioKey;

	// Token: 0x0400075E RID: 1886
	public string DisableClickAudioKey = "ui_default_click_fail";

	// Token: 0x0400075F RID: 1887
	public Vector2 ScaleToValue = Vector2.one * -1f;

	// Token: 0x04000760 RID: 1888
	public float ScaleInterval = 0.3f;

	// Token: 0x04000761 RID: 1889
	[SerializeField]
	public List<TextMeshProUGUI> LabelList;

	// Token: 0x04000762 RID: 1890
	[FormerlySerializedAs("_needScale")]
	public bool UsePressScale;

	// Token: 0x04000763 RID: 1891
	public bool NeedExecutePointerExitEventOnClick;

	// Token: 0x04000764 RID: 1892
	public bool NeedExecutePointerEnterEventOnClick;

	// Token: 0x04000765 RID: 1893
	private Tweener _scaleTweener;

	// Token: 0x04000766 RID: 1894
	private Vector3 _startScale;

	// Token: 0x04000767 RID: 1895
	public CButtonObsolete.ButtonEvent OnInteractableChange;

	// Token: 0x04000768 RID: 1896
	public CButtonObsolete.ButtonEvent OnInteractableChangeReverse;

	// Token: 0x04000769 RID: 1897
	private PointerTrigger _pointerTrigger;

	// Token: 0x0400076A RID: 1898
	[NonSerialized]
	public bool EnableClickAudio = true;

	// Token: 0x0400076B RID: 1899
	private bool _recordedInteractable = false;

	// Token: 0x0400076C RID: 1900
	public Action<CButtonObsolete.State> OnSelectStateChange;

	// Token: 0x0200112A RID: 4394
	[Serializable]
	public class ButtonEvent : UnityEvent<bool>
	{
	}

	// Token: 0x0200112B RID: 4395
	public enum State
	{
		// Token: 0x040095D4 RID: 38356
		Select,
		// Token: 0x040095D5 RID: 38357
		Deselect
	}
}
