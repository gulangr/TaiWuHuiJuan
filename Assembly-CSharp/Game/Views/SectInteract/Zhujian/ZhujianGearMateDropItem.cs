using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C8 RID: 2504
	public class ZhujianGearMateDropItem : MonoBehaviour
	{
		// Token: 0x1400007C RID: 124
		// (add) Token: 0x06007974 RID: 31092 RVA: 0x00387198 File Offset: 0x00385398
		// (remove) Token: 0x06007975 RID: 31093 RVA: 0x003871D0 File Offset: 0x003853D0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnTrigger;

		// Token: 0x06007976 RID: 31094 RVA: 0x00387208 File Offset: 0x00385408
		private void OnTriggerEnter2D(Collider2D other)
		{
			bool flag = other.GetComponent<ZhujianGearMateDropItem>() != null;
			if (!flag)
			{
				bool flag2 = this.ExplosionPrefab != null;
				if (flag2)
				{
					GameObject destroyParticle = Object.Instantiate<GameObject>(this.ExplosionPrefab, base.transform.parent);
					destroyParticle.SetActive(true);
					destroyParticle.transform.position = base.transform.position;
					ParticleSystem ps;
					bool flag3 = destroyParticle.TryGetComponent<ParticleSystem>(out ps);
					if (flag3)
					{
						ps.Play();
					}
					Object.Destroy(destroyParticle, 1.5f);
				}
				bool flag4 = !ZhujianGearMateDropItem._audioLock;
				if (flag4)
				{
					AudioManager.Instance.PlaySound(string.Format("SFX_GearMate_machine_falling_0{0}", Random.Range(1, 4)), false, false);
					ZhujianGearMateDropItem._audioLock = true;
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
					{
						ZhujianGearMateDropItem._audioLock = false;
					});
				}
				Action onTrigger = this.OnTrigger;
				if (onTrigger != null)
				{
					onTrigger();
				}
				this.OnTrigger = null;
				bool flag5 = !this.destroyWhenLetGo;
				if (flag5)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06007977 RID: 31095 RVA: 0x00387330 File Offset: 0x00385530
		private void OnTriggerExit(Collider other)
		{
			bool flag = other.GetComponent<ZhujianGearMateDropItem>() != null;
			if (!flag)
			{
				bool flag2 = this.destroyWhenLetGo;
				if (flag2)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04005C1D RID: 23581
		[NonSerialized]
		public GameObject ExplosionPrefab;

		// Token: 0x04005C1E RID: 23582
		public bool destroyWhenLetGo;

		// Token: 0x04005C20 RID: 23584
		private static bool _audioLock;
	}
}
