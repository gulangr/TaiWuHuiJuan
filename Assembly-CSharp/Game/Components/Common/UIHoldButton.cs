using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.Common
{
	// Token: 0x02000F8E RID: 3982
	[RequireComponent(typeof(CButton))]
	public class UIHoldButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x0600B726 RID: 46886 RVA: 0x0053791F File Offset: 0x00535B1F
		private void Awake()
		{
			this.Button = base.GetComponent<CButton>();
		}

		// Token: 0x0600B727 RID: 46887 RVA: 0x00537930 File Offset: 0x00535B30
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

		// Token: 0x0600B728 RID: 46888 RVA: 0x00537A0C File Offset: 0x00535C0C
		private void TriggerButton()
		{
			Button.ButtonClickedEvent onClick = this.Button.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
		}

		// Token: 0x0600B729 RID: 46889 RVA: 0x00537A26 File Offset: 0x00535C26
		public void OnPointerDown(PointerEventData eventData)
		{
			this._isDown = true;
			this._holdTimer = 0f;
			this._triggerTimer = 0f;
			this._v = this.v0;
		}

		// Token: 0x0600B72A RID: 46890 RVA: 0x00537A52 File Offset: 0x00535C52
		public void OnPointerUp(PointerEventData eventData)
		{
			this._isDown = false;
			this._holdTimer = 0f;
			this._triggerTimer = 0f;
			this._v = 0f;
		}

		// Token: 0x04008E3F RID: 36415
		[Tooltip("长按此时间后，开始生效，单位是秒")]
		public float holdWait = 0.5f;

		// Token: 0x04008E40 RID: 36416
		[Tooltip("初速度，单位是触发次数/秒")]
		public float v0 = 50f;

		// Token: 0x04008E41 RID: 36417
		[Tooltip("最大速度")]
		public float vmax = 1000f;

		// Token: 0x04008E42 RID: 36418
		[Tooltip("加速度，单位是触发次数/秒^2")]
		public float acceleration = 0.1f;

		// Token: 0x04008E43 RID: 36419
		private float _v = 0f;

		// Token: 0x04008E44 RID: 36420
		private bool _isDown = false;

		// Token: 0x04008E45 RID: 36421
		private float _holdTimer = 0f;

		// Token: 0x04008E46 RID: 36422
		private float _triggerTimer = 0f;

		// Token: 0x04008E47 RID: 36423
		protected CButton Button;
	}
}
