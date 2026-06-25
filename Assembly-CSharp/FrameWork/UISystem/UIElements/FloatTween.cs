using System;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001002 RID: 4098
	internal struct FloatTween : ITweenValue
	{
		// Token: 0x17001507 RID: 5383
		// (get) Token: 0x0600BADA RID: 47834 RVA: 0x00551390 File Offset: 0x0054F590
		// (set) Token: 0x0600BADB RID: 47835 RVA: 0x005513A8 File Offset: 0x0054F5A8
		public float startValue
		{
			get
			{
				return this.m_StartValue;
			}
			set
			{
				this.m_StartValue = value;
			}
		}

		// Token: 0x17001508 RID: 5384
		// (get) Token: 0x0600BADC RID: 47836 RVA: 0x005513B4 File Offset: 0x0054F5B4
		// (set) Token: 0x0600BADD RID: 47837 RVA: 0x005513CC File Offset: 0x0054F5CC
		public float targetValue
		{
			get
			{
				return this.m_TargetValue;
			}
			set
			{
				this.m_TargetValue = value;
			}
		}

		// Token: 0x17001509 RID: 5385
		// (get) Token: 0x0600BADE RID: 47838 RVA: 0x005513D8 File Offset: 0x0054F5D8
		// (set) Token: 0x0600BADF RID: 47839 RVA: 0x005513F0 File Offset: 0x0054F5F0
		public float duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = value;
			}
		}

		// Token: 0x1700150A RID: 5386
		// (get) Token: 0x0600BAE0 RID: 47840 RVA: 0x005513FC File Offset: 0x0054F5FC
		// (set) Token: 0x0600BAE1 RID: 47841 RVA: 0x00551414 File Offset: 0x0054F614
		public bool ignoreTimeScale
		{
			get
			{
				return this.m_IgnoreTimeScale;
			}
			set
			{
				this.m_IgnoreTimeScale = value;
			}
		}

		// Token: 0x0600BAE2 RID: 47842 RVA: 0x00551420 File Offset: 0x0054F620
		public void TweenValue(float floatPercentage)
		{
			bool flag = !this.ValidTarget();
			if (!flag)
			{
				float newValue = Mathf.Lerp(this.m_StartValue, this.m_TargetValue, floatPercentage);
				this.m_Target.Invoke(newValue);
			}
		}

		// Token: 0x0600BAE3 RID: 47843 RVA: 0x00551460 File Offset: 0x0054F660
		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			bool flag = this.m_Target == null;
			if (flag)
			{
				this.m_Target = new FloatTween.FloatTweenCallback();
			}
			this.m_Target.AddListener(callback);
		}

		// Token: 0x0600BAE4 RID: 47844 RVA: 0x00551494 File Offset: 0x0054F694
		public bool GetIgnoreTimescale()
		{
			return this.m_IgnoreTimeScale;
		}

		// Token: 0x0600BAE5 RID: 47845 RVA: 0x005514AC File Offset: 0x0054F6AC
		public float GetDuration()
		{
			return this.m_Duration;
		}

		// Token: 0x0600BAE6 RID: 47846 RVA: 0x005514C4 File Offset: 0x0054F6C4
		public bool ValidTarget()
		{
			return this.m_Target != null;
		}

		// Token: 0x04009052 RID: 36946
		private FloatTween.FloatTweenCallback m_Target;

		// Token: 0x04009053 RID: 36947
		private float m_StartValue;

		// Token: 0x04009054 RID: 36948
		private float m_TargetValue;

		// Token: 0x04009055 RID: 36949
		private float m_Duration;

		// Token: 0x04009056 RID: 36950
		private bool m_IgnoreTimeScale;

		// Token: 0x0200263C RID: 9788
		public class FloatTweenCallback : UnityEvent<float>
		{
		}
	}
}
