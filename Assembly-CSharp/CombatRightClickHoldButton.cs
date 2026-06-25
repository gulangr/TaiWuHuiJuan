using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200015A RID: 346
[RequireComponent(typeof(CButton))]
public class CombatRightClickHoldButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler
{
	// Token: 0x1700021C RID: 540
	// (get) Token: 0x060012FE RID: 4862 RVA: 0x00073CDF File Offset: 0x00071EDF
	// (set) Token: 0x060012FF RID: 4863 RVA: 0x00073CE7 File Offset: 0x00071EE7
	public float HoldThreshold
	{
		get
		{
			return this.holdThreshold;
		}
		set
		{
			this.holdThreshold = Mathf.Max(0f, value);
		}
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x00073CFC File Offset: 0x00071EFC
	public void OnPointerDown(PointerEventData eventData)
	{
		bool flag = eventData.button != PointerEventData.InputButton.Right;
		if (!flag)
		{
			this._isRightPointerDown = true;
			this._rightHoldTriggered = false;
			this._rightDownTime = Time.unscaledTime;
		}
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x00073D38 File Offset: 0x00071F38
	public void OnPointerUp(PointerEventData eventData)
	{
		bool flag = eventData.button != PointerEventData.InputButton.Right;
		if (!flag)
		{
			bool flag2 = this._isRightPointerDown && !this._rightHoldTriggered;
			if (flag2)
			{
				Action<Vector2> onRightClick = this.OnRightClick;
				if (onRightClick != null)
				{
					onRightClick(eventData.position);
				}
			}
			this.ResetRightPointerState();
		}
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x00073D90 File Offset: 0x00071F90
	public void OnPointerExit(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Right;
		if (flag)
		{
			this.ResetRightPointerState();
		}
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00073DB4 File Offset: 0x00071FB4
	private void Update()
	{
		bool flag = !this._isRightPointerDown || this._rightHoldTriggered;
		if (!flag)
		{
			bool flag2 = Time.unscaledTime - this._rightDownTime < this.holdThreshold;
			if (!flag2)
			{
				this._rightHoldTriggered = true;
				Action<Vector2> onRightHold = this.OnRightHold;
				if (onRightHold != null)
				{
					onRightHold(Input.mousePosition);
				}
			}
		}
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00073E16 File Offset: 0x00072016
	private void OnDisable()
	{
		this.ResetRightPointerState();
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00073E20 File Offset: 0x00072020
	private void ResetRightPointerState()
	{
		this._isRightPointerDown = false;
		this._rightHoldTriggered = false;
		this._rightDownTime = 0f;
	}

	// Token: 0x04001005 RID: 4101
	[SerializeField]
	private float holdThreshold = 0.3f;

	// Token: 0x04001006 RID: 4102
	public Action<Vector2> OnRightClick;

	// Token: 0x04001007 RID: 4103
	public Action<Vector2> OnRightHold;

	// Token: 0x04001008 RID: 4104
	private bool _isRightPointerDown;

	// Token: 0x04001009 RID: 4105
	private bool _rightHoldTriggered;

	// Token: 0x0400100A RID: 4106
	private float _rightDownTime;
}
