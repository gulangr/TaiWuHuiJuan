using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000A8 RID: 168
[RequireComponent(typeof(CButtonObsolete))]
public class UIHoldButton2 : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x060005CB RID: 1483 RVA: 0x000267DE File Offset: 0x000249DE
	private void Awake()
	{
		this.Button = base.GetComponent<CButtonObsolete>();
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x000267F0 File Offset: 0x000249F0
	private void Update()
	{
		bool flag = !this._isDown;
		if (!flag)
		{
			this._holdTimer += Time.deltaTime;
			bool flag2 = !this.Button.interactable;
			if (flag2)
			{
				this.OnPointerUp(null);
			}
			else
			{
				bool flag3 = this._holdTimer < this.holdWait;
				if (!flag3)
				{
					this._v = Mathf.Min(this.v0 + this.acceleration * (this._holdTimer - this.holdWait), this.vmax);
					float triggerInterval = 1f / this._v;
					this._triggerTimer += Time.deltaTime;
					while (this._triggerTimer >= triggerInterval)
					{
						this.TriggerButton();
						this._triggerTimer -= triggerInterval;
					}
				}
			}
		}
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x000268CC File Offset: 0x00024ACC
	private void TriggerButton()
	{
		Button.ButtonClickedEvent onClick = this.Button.onClick;
		if (onClick != null)
		{
			onClick.Invoke();
		}
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x000268E6 File Offset: 0x00024AE6
	public void OnPointerDown(PointerEventData eventData)
	{
		this._isDown = true;
		this._holdTimer = 0f;
		this._triggerTimer = 0f;
		this._v = this.v0;
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00026912 File Offset: 0x00024B12
	public void OnPointerUp(PointerEventData eventData)
	{
		this._isDown = false;
		this._holdTimer = 0f;
		this._triggerTimer = 0f;
		this._v = 0f;
	}

	// Token: 0x040004C0 RID: 1216
	[Tooltip("长按此时间后，开始生效，单位是秒")]
	public float holdWait = 0.5f;

	// Token: 0x040004C1 RID: 1217
	[Tooltip("初速度，单位是触发次数/秒")]
	public float v0 = 50f;

	// Token: 0x040004C2 RID: 1218
	[Tooltip("最大速度")]
	public float vmax = 1000f;

	// Token: 0x040004C3 RID: 1219
	[Tooltip("加速度，单位是触发次数/秒^2")]
	public float acceleration = 0.1f;

	// Token: 0x040004C4 RID: 1220
	private float _v = 0f;

	// Token: 0x040004C5 RID: 1221
	private bool _isDown = false;

	// Token: 0x040004C6 RID: 1222
	private float _holdTimer = 0f;

	// Token: 0x040004C7 RID: 1223
	private float _triggerTimer = 0f;

	// Token: 0x040004C8 RID: 1224
	protected CButtonObsolete Button;
}
