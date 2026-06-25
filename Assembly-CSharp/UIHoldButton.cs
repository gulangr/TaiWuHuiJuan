using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000A7 RID: 167
[RequireComponent(typeof(CButtonObsolete))]
public class UIHoldButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x060005C4 RID: 1476 RVA: 0x00026617 File Offset: 0x00024817
	private void Awake()
	{
		this.Button = base.GetComponent<CButtonObsolete>();
		this._isDown = false;
		this.RealFrequency = this.Frequency;
		this.RealAcceleration = (float)this.Acceleration * 0.001f;
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x0002664C File Offset: 0x0002484C
	public void OnPointerDown(PointerEventData eventData)
	{
		bool flag = eventData.button > PointerEventData.InputButton.Left;
		if (!flag)
		{
			this._isDown = true;
			this.LastActiveTime = 0f;
			this.ActiveCount = 0;
			this._downTime = Time.realtimeSinceStartup;
			this.RealFrequency = this.Frequency;
		}
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00026699 File Offset: 0x00024899
	public void OnPointerUp(PointerEventData eventData)
	{
		this._isDown = false;
		this.HoldState = false;
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000266AA File Offset: 0x000248AA
	private void OnDisable()
	{
		this.OnPointerUp(null);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000266B8 File Offset: 0x000248B8
	private void Update()
	{
		bool flag = !this._isDown;
		if (!flag)
		{
			bool flag2 = !this.Button.interactable;
			if (flag2)
			{
				this.OnPointerUp(null);
			}
			else
			{
				bool flag3 = !this.HoldState && Time.realtimeSinceStartup - this._downTime > this.HoldWait;
				if (flag3)
				{
					this.HoldState = true;
				}
				bool holdState = this.HoldState;
				if (holdState)
				{
					bool flag4 = Time.realtimeSinceStartup - this.LastActiveTime > this.RealFrequency;
					if (flag4)
					{
						this.ActivateOnce();
					}
				}
			}
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x0002674C File Offset: 0x0002494C
	protected virtual void ActivateOnce()
	{
		this.LastActiveTime = Time.realtimeSinceStartup;
		bool flag = this.Button != null;
		if (flag)
		{
			this.Button.onClick.Invoke();
		}
		this.ActiveCount++;
		this.RealFrequency = Mathf.Max(Time.deltaTime, this.Frequency - this.RealAcceleration * (float)this.ActiveCount % 10f);
	}

	// Token: 0x040004B5 RID: 1205
	[Tooltip("长按生效的等待时长")]
	public float HoldWait = 0.5f;

	// Token: 0x040004B6 RID: 1206
	[Tooltip("长按下对点击的响应频率")]
	public float Frequency = 0.1f;

	// Token: 0x040004B7 RID: 1207
	[ReadOnly]
	[Tooltip("长按状态")]
	public bool HoldState;

	// Token: 0x040004B8 RID: 1208
	[Tooltip("响应加速度（毫秒）")]
	public short Acceleration;

	// Token: 0x040004B9 RID: 1209
	protected float RealFrequency;

	// Token: 0x040004BA RID: 1210
	protected int ActiveCount;

	// Token: 0x040004BB RID: 1211
	protected float RealAcceleration;

	// Token: 0x040004BC RID: 1212
	private float _downTime;

	// Token: 0x040004BD RID: 1213
	protected float LastActiveTime;

	// Token: 0x040004BE RID: 1214
	private bool _isDown;

	// Token: 0x040004BF RID: 1215
	protected CButtonObsolete Button;
}
