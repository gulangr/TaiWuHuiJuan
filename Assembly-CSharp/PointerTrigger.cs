using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000088 RID: 136
public class PointerTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00022453 File Offset: 0x00020653
	// (set) Token: 0x060004F5 RID: 1269 RVA: 0x0002245B File Offset: 0x0002065B
	public bool AtEnter { get; set; }

	// Token: 0x060004F6 RID: 1270 RVA: 0x00022464 File Offset: 0x00020664
	public void SetBindElement(UIElement element)
	{
		this._bindElement = element;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00022470 File Offset: 0x00020670
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		bool onlyResponseWhenToggleEnabled = this.OnlyResponseWhenToggleEnabled;
		if (onlyResponseWhenToggleEnabled)
		{
			bool flag = this.Selectable == null || !this.Selectable.interactable;
			if (flag)
			{
				return;
			}
		}
		bool flag2 = this._bindElement != null && !this._bindElement.Ready;
		if (!flag2)
		{
			Vector2 screenMousePos = UIManager.Instance.UiCamera.ScreenToViewportPoint(Input.mousePosition);
			bool flag3 = screenMousePos.x < 0f || screenMousePos.x > 1f || screenMousePos.y < 0f || screenMousePos.y > 1f;
			if (!flag3)
			{
				bool flag4 = this.IsBlockByUIElement();
				if (!flag4)
				{
					this.AtEnter = true;
					UnityEvent enterEvent = this.EnterEvent;
					if (enterEvent != null)
					{
						enterEvent.Invoke();
					}
					PointerTriggerEvent enterEventWithParam = this.EnterEventWithParam;
					if (enterEventWithParam != null)
					{
						enterEventWithParam.Invoke(base.gameObject);
					}
				}
			}
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0002256C File Offset: 0x0002076C
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		bool onlyResponseWhenToggleEnabled = this.OnlyResponseWhenToggleEnabled;
		if (onlyResponseWhenToggleEnabled)
		{
			bool flag = this.Selectable == null || !this.Selectable.interactable;
			if (flag)
			{
				return;
			}
		}
		this.AtEnter = false;
		UnityEvent exitEvent = this.ExitEvent;
		if (exitEvent != null)
		{
			exitEvent.Invoke();
		}
		PointerTriggerEvent exitEventWithParam = this.ExitEventWithParam;
		if (exitEventWithParam != null)
		{
			exitEventWithParam.Invoke(base.gameObject);
		}
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x000225DC File Offset: 0x000207DC
	protected virtual void OnDisable()
	{
		bool ignoreOnDisableTrigger = this.IgnoreOnDisableTrigger;
		if (!ignoreOnDisableTrigger)
		{
			UnityEvent exitEvent = this.ExitEvent;
			if (exitEvent != null)
			{
				exitEvent.Invoke();
			}
			PointerTriggerEvent exitEventWithParam = this.ExitEventWithParam;
			if (exitEventWithParam != null)
			{
				exitEventWithParam.Invoke(base.gameObject);
			}
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00022620 File Offset: 0x00020820
	private bool IsBlockByUIElement()
	{
		bool flag = null != this.Toggle;
		bool result;
		if (flag)
		{
			result = (this.Toggle.isOn || !this.Toggle.interactable);
		}
		else
		{
			bool flag2 = null != this.Selectable;
			result = (flag2 && !this.Selectable.interactable);
		}
		return result;
	}

	// Token: 0x040003FE RID: 1022
	public Toggle Toggle;

	// Token: 0x040003FF RID: 1023
	public Selectable Selectable;

	// Token: 0x04000400 RID: 1024
	public bool OnlyResponseWhenToggleEnabled;

	// Token: 0x04000402 RID: 1026
	private UIElement _bindElement;

	// Token: 0x04000403 RID: 1027
	public UnityEvent EnterEvent;

	// Token: 0x04000404 RID: 1028
	public UnityEvent ExitEvent;

	// Token: 0x04000405 RID: 1029
	public PointerTriggerEvent EnterEventWithParam;

	// Token: 0x04000406 RID: 1030
	public PointerTriggerEvent ExitEventWithParam;

	// Token: 0x04000407 RID: 1031
	public bool IgnoreOnDisableTrigger = false;
}
