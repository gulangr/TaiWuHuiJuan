using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

namespace FrameWork.UISystem.Components.EffectPlayer
{
	// Token: 0x0200102D RID: 4141
	public class UIParticlePlayHelper
	{
		// Token: 0x0600BD50 RID: 48464 RVA: 0x00560332 File Offset: 0x0055E532
		public static void SetParticleActive(UIParticle particle, bool isOn)
		{
			particle.gameObject.SetActive(isOn);
		}

		// Token: 0x0600BD51 RID: 48465 RVA: 0x00560344 File Offset: 0x0055E544
		public void PlayOnceParticle(UIParticle particle, float time, Action onFinished = null)
		{
			particle.gameObject.SetActive(true);
			particle.Play();
			Coroutine coroutine;
			bool flag = this._playOneParticleCoroutines.TryGetValue(particle, out coroutine);
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(coroutine);
			}
			Coroutine newCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(time, delegate
			{
				particle.gameObject.SetActive(false);
				particle.Stop();
				bool flag2 = onFinished != null;
				if (flag2)
				{
					onFinished();
				}
			});
			this._playOneParticleCoroutines[particle] = newCoroutine;
		}

		// Token: 0x0600BD52 RID: 48466 RVA: 0x005603D4 File Offset: 0x0055E5D4
		public void SetActiveParticle(UIParticle particle, bool isActive, bool needPlay = false)
		{
			bool needPlayInner = needPlay && (!particle.gameObject.activeSelf && isActive);
			particle.gameObject.SetActive(isActive);
			bool flag = needPlayInner;
			if (flag)
			{
				particle.Play();
			}
		}

		// Token: 0x0600BD53 RID: 48467 RVA: 0x00560414 File Offset: 0x0055E614
		public void RegisterParticle(string key, UIParticle sourceParticle)
		{
			this._registeredKeys.Add(key);
			PoolManager.SetSrcObject(key, sourceParticle.gameObject);
		}

		// Token: 0x0600BD54 RID: 48468 RVA: 0x00560434 File Offset: 0x0055E634
		public void ClearAll()
		{
			foreach (string key in this._registeredKeys)
			{
				this.UnregisterParticle(key);
			}
			this._registeredKeys.Clear();
		}

		// Token: 0x0600BD55 RID: 48469 RVA: 0x0056049C File Offset: 0x0055E69C
		public void UnregisterParticle(string key)
		{
			PoolManager.RemoveData(key);
		}

		// Token: 0x0600BD56 RID: 48470 RVA: 0x005604A8 File Offset: 0x0055E6A8
		public void PlayOneParticlePooled(string key, Transform parent, float delay)
		{
			GameObject p = PoolManager.GetObject(key);
			p.transform.SetParent(parent, false);
			Action <>9__1;
			this.PlayOnceParticle(p.GetComponent<UIParticle>(), delay, delegate
			{
				p.SetActive(false);
				YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
				float sec = delay + this.PoolDestroyDelay;
				Action job;
				if ((job = <>9__1) == null)
				{
					job = (<>9__1 = delegate()
					{
						PoolManager.Destroy(key, p);
					});
				}
				instance.DelaySecondsDo(sec, job);
			});
		}

		// Token: 0x040091BC RID: 37308
		public float PoolDestroyDelay = 1.5f;

		// Token: 0x040091BD RID: 37309
		private readonly Dictionary<UIParticle, Coroutine> _playOneParticleCoroutines = new Dictionary<UIParticle, Coroutine>();

		// Token: 0x040091BE RID: 37310
		protected HashSet<string> _registeredKeys = new HashSet<string>();
	}
}
