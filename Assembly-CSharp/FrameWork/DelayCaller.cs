using System;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork
{
	// Token: 0x02000FE8 RID: 4072
	public class DelayCaller : MonoBehaviour
	{
		// Token: 0x170014F4 RID: 5364
		// (get) Token: 0x0600B9E6 RID: 47590 RVA: 0x0054AEE6 File Offset: 0x005490E6
		public bool IsInDelay
		{
			get
			{
				return this._inDelay;
			}
		}

		// Token: 0x0600B9E7 RID: 47591 RVA: 0x0054AEF0 File Offset: 0x005490F0
		private void OnEnable()
		{
			bool flag = this.autoStartOnEnable;
			if (flag)
			{
				this.StartDelay();
			}
		}

		// Token: 0x0600B9E8 RID: 47592 RVA: 0x0054AF10 File Offset: 0x00549110
		private void OnDisable()
		{
			bool flag = this.autoStopOnDisable;
			if (flag)
			{
				this.Stop();
			}
		}

		// Token: 0x0600B9E9 RID: 47593 RVA: 0x0054AF2F File Offset: 0x0054912F
		public void StartDelay()
		{
			this._startTime = Time.realtimeSinceStartup;
			this._inDelay = true;
		}

		// Token: 0x0600B9EA RID: 47594 RVA: 0x0054AF44 File Offset: 0x00549144
		public void Stop()
		{
			this._inDelay = false;
		}

		// Token: 0x0600B9EB RID: 47595 RVA: 0x0054AF50 File Offset: 0x00549150
		private void Update()
		{
			bool flag = !this._inDelay || Time.realtimeSinceStartup - this._startTime <= this.delay;
			if (!flag)
			{
				UnityEvent unityEvent = this.callback;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this._inDelay = false;
			}
		}

		// Token: 0x04008FC0 RID: 36800
		public float delay = 0.1f;

		// Token: 0x04008FC1 RID: 36801
		public UnityEvent callback;

		// Token: 0x04008FC2 RID: 36802
		private float _startTime;

		// Token: 0x04008FC3 RID: 36803
		private bool _inDelay;

		// Token: 0x04008FC4 RID: 36804
		[Tooltip("是否在OnEnable时自动开始延迟，否则需要手动调用StartDelay方法")]
		public bool autoStartOnEnable = false;

		// Token: 0x04008FC5 RID: 36805
		[Tooltip("是否在OnDisable时自动停止延迟并重置状态，否则需要手动调用Stop方法")]
		public bool autoStopOnDisable = false;
	}
}
