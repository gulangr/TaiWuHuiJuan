using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000D9 RID: 217
[DisallowMultipleComponent]
public class UIInteractionBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170000BC RID: 188
	// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0003621E File Offset: 0x0003441E
	// (set) Token: 0x060007B7 RID: 1975 RVA: 0x00036226 File Offset: 0x00034426
	public bool Active { get; private set; }

	// Token: 0x060007B8 RID: 1976 RVA: 0x00036230 File Offset: 0x00034430
	public void OnPointerEnter(PointerEventData eventData)
	{
		bool active = this.Active;
		if (!active)
		{
			this.Active = true;
			UIInteractionBehaviour.ActiveTriggerSet.Add(this);
			UIInteractionBehaviour.RefreshCursor();
			bool flag = this.hoverAudioDisabled;
			if (!flag)
			{
				Selectable selectable = base.GetComponent<Selectable>();
				bool flag2 = selectable != null;
				if (flag2)
				{
					bool flag3 = selectable != null && !selectable.interactable;
					if (flag3)
					{
						return;
					}
					bool flag4 = selectable.transition == Selectable.Transition.None;
					if (flag4)
					{
						return;
					}
					bool flag5 = selectable.transition == Selectable.Transition.SpriteSwap && !selectable.spriteState.highlightedSprite;
					if (flag5)
					{
						return;
					}
				}
				this.TryPlayHoverAudio();
			}
		}
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x000362E0 File Offset: 0x000344E0
	public void OnPointerExit(PointerEventData eventData)
	{
		bool flag = !this.Active;
		if (!flag)
		{
			this.Active = false;
			UIInteractionBehaviour.ActiveTriggerSet.Remove(this);
			UIInteractionBehaviour.RefreshCursor();
		}
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x00036317 File Offset: 0x00034517
	private void OnDisable()
	{
		UIInteractionBehaviour.ActiveTriggerSet.Remove(this);
		this.Active = false;
		UIInteractionBehaviour.RefreshCursor();
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x00036334 File Offset: 0x00034534
	private void OnDestroy()
	{
		UIInteractionBehaviour.ActiveTriggerSet.Remove(this);
		UIInteractionBehaviour.RefreshCursor();
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0003634C File Offset: 0x0003454C
	private static void RefreshCursor()
	{
		bool flag = UIInteractionBehaviour.ActiveTriggerSet.Count > 0;
		if (flag)
		{
			ConchShipCursor.Instance.TrySetClickableCursor();
		}
		else
		{
			ConchShipCursor.Instance.TrySetDefaultCursor();
		}
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x00036387 File Offset: 0x00034587
	public void Play(bool canInteract)
	{
		this.Play(PointerEventData.InputButton.Left, canInteract);
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x00036394 File Offset: 0x00034594
	public void Play(PointerEventData.InputButton button, bool canInteract)
	{
		string key = canInteract ? this.ResolveClickAudioKey(button) : this.ResolveDisableClickAudioKey();
		bool flag = !string.IsNullOrEmpty(key);
		if (flag)
		{
			AudioManager.Instance.PlaySound(key, false, false);
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x000363D0 File Offset: 0x000345D0
	private void TryPlayHoverAudio()
	{
		string key = this.ResolveHoverAudioKey();
		bool flag = !string.IsNullOrEmpty(key);
		if (flag)
		{
			AudioManager.Instance.PlaySound(key, false, false);
		}
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00036400 File Offset: 0x00034600
	private string ResolveClickAudioKey(PointerEventData.InputButton button)
	{
		bool flag = !string.IsNullOrEmpty(this.ClickAudioKey);
		string result;
		if (flag)
		{
			result = this.ClickAudioKey;
		}
		else
		{
			bool flag2 = button == PointerEventData.InputButton.Right;
			if (flag2)
			{
				result = UIInteractionBehaviour.ResolveAudioMeaning(this.RightClickAudioMeaning);
			}
			else
			{
				result = UIInteractionBehaviour.ResolveAudioMeaning(this.ClickAudioMeaning);
			}
		}
		return result;
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00036450 File Offset: 0x00034650
	private string ResolveDisableClickAudioKey()
	{
		bool flag = !string.IsNullOrEmpty(this.DisableClickAudioKey);
		string result;
		if (flag)
		{
			result = this.DisableClickAudioKey;
		}
		else
		{
			result = "ui_default_click_fail";
		}
		return result;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00036484 File Offset: 0x00034684
	private string ResolveHoverAudioKey()
	{
		bool flag = !string.IsNullOrEmpty(this.HoverAudioKey);
		string result;
		if (flag)
		{
			result = this.HoverAudioKey;
		}
		else
		{
			result = "ui_default_hover";
		}
		return result;
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x000364B8 File Offset: 0x000346B8
	private static string ResolveAudioMeaning(UIInteractAudioMeaning meaning)
	{
		if (!true)
		{
		}
		string result;
		switch (meaning)
		{
		case UIInteractAudioMeaning.None:
			result = string.Empty;
			break;
		case UIInteractAudioMeaning.DefaultClickLeft:
			result = "ui_default_click_left";
			break;
		case UIInteractAudioMeaning.DefaultClickRight:
			result = "ui_default_click_right";
			break;
		case UIInteractAudioMeaning.DefaultSecondClickLeft:
			result = "ui_default_second_click_left";
			break;
		case UIInteractAudioMeaning.DefaultCancel:
			result = "ui_default_cancel";
			break;
		case UIInteractAudioMeaning.DefaultSelect:
			result = "ui_default_select";
			break;
		case UIInteractAudioMeaning.DefaultSecondSelect:
			result = "ui_default_second_select";
			break;
		case UIInteractAudioMeaning.DefaultRandom:
			result = "ui_default_random";
			break;
		case UIInteractAudioMeaning.DefaultConfirm:
			result = "ui_default_confirm";
			break;
		case UIInteractAudioMeaning.DefaultAdd:
			result = "ui_default_add";
			break;
		case UIInteractAudioMeaning.DefaultAddAll:
			result = "ui_default_add_all";
			break;
		case UIInteractAudioMeaning.DefaultReduce:
			result = "ui_default_reduce";
			break;
		case UIInteractAudioMeaning.DefaultReduceAll:
			result = "ui_default_reduce_all";
			break;
		case UIInteractAudioMeaning.DefaultPut:
			result = "ui_default_put";
			break;
		case UIInteractAudioMeaning.DefaultTake:
			result = "ui_default_take";
			break;
		case UIInteractAudioMeaning.DefaultHover:
			result = "ui_default_hover";
			break;
		default:
			if (meaning != UIInteractAudioMeaning.Custom)
			{
				result = string.Empty;
			}
			else
			{
				result = string.Empty;
			}
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x040007E2 RID: 2018
	private static readonly HashSet<UIInteractionBehaviour> ActiveTriggerSet = new HashSet<UIInteractionBehaviour>();

	// Token: 0x040007E3 RID: 2019
	[Tooltip("点击音效含义")]
	public UIInteractAudioMeaning ClickAudioMeaning = UIInteractAudioMeaning.DefaultClickLeft;

	// Token: 0x040007E4 RID: 2020
	[Tooltip("点击右键音效含义")]
	public UIInteractAudioMeaning RightClickAudioMeaning = UIInteractAudioMeaning.None;

	// Token: 0x040007E5 RID: 2021
	[Tooltip("自定义点击音效（旧字段，优先级高于含义配置）")]
	public string ClickAudioKey;

	// Token: 0x040007E6 RID: 2022
	[Tooltip("自定义禁用点击音效（旧字段，优先级高于含义配置）")]
	public string DisableClickAudioKey;

	// Token: 0x040007E7 RID: 2023
	[Tooltip("自定义悬停音效")]
	public string HoverAudioKey;

	// Token: 0x040007E8 RID: 2024
	[SerializeField]
	private bool hoverAudioDisabled;
}
