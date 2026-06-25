using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001027 RID: 4135
	public class UIHoldButtonCanChangeMaxSpeed : UIHoldButton
	{
		// Token: 0x0600BD2C RID: 48428 RVA: 0x0055F424 File Offset: 0x0055D624
		protected override void ActivateOnce()
		{
			int invokeTime = 1;
			this.RealFrequency = this.Frequency - this.RealAcceleration * (float)(this.ActiveCount / this.accelerationSpeed);
			bool flag = this.RealFrequency <= Time.deltaTime;
			if (flag)
			{
				invokeTime = this.maxCountPreFrame;
				this.RealFrequency = Time.deltaTime;
			}
			while (invokeTime-- > 0)
			{
				bool flag2 = this.Button != null;
				if (flag2)
				{
					this.Button.onClick.Invoke();
				}
				this.ActiveCount++;
			}
			this.LastActiveTime = Time.realtimeSinceStartup;
		}

		// Token: 0x04009196 RID: 37270
		[Tooltip("最大每帧响应次数")]
		public int maxCountPreFrame = 10;

		// Token: 0x04009197 RID: 37271
		[Tooltip("响应几次加速一次")]
		public int accelerationSpeed = 2;
	}
}
