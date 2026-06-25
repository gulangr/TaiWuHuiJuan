using System;
using System.Collections;
using UnityEngine;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001003 RID: 4099
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		// Token: 0x0600BAE7 RID: 47847 RVA: 0x005514DF File Offset: 0x0054F6DF
		private static IEnumerator Start(T tweenInfo)
		{
			bool flag = !tweenInfo.ValidTarget();
			if (flag)
			{
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += (tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
				float percentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
				tweenInfo.TweenValue(percentage);
				yield return null;
			}
			tweenInfo.TweenValue(1f);
			yield break;
		}

		// Token: 0x0600BAE8 RID: 47848 RVA: 0x005514EE File Offset: 0x0054F6EE
		public void Init(MonoBehaviour coroutineContainer)
		{
			this.m_CoroutineContainer = coroutineContainer;
		}

		// Token: 0x0600BAE9 RID: 47849 RVA: 0x005514F8 File Offset: 0x0054F6F8
		public void StartTween(T info)
		{
			bool flag = this.m_CoroutineContainer == null;
			if (flag)
			{
				Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
			}
			else
			{
				this.StopTween();
				bool flag2 = !this.m_CoroutineContainer.gameObject.activeInHierarchy;
				if (flag2)
				{
					info.TweenValue(1f);
				}
				else
				{
					this.m_Tween = TweenRunner<T>.Start(info);
					this.m_CoroutineContainer.StartCoroutine(this.m_Tween);
				}
			}
		}

		// Token: 0x0600BAEA RID: 47850 RVA: 0x00551578 File Offset: 0x0054F778
		public void StopTween()
		{
			bool flag = this.m_Tween != null;
			if (flag)
			{
				this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
				this.m_Tween = null;
			}
		}

		// Token: 0x04009057 RID: 36951
		protected MonoBehaviour m_CoroutineContainer;

		// Token: 0x04009058 RID: 36952
		protected IEnumerator m_Tween;
	}
}
