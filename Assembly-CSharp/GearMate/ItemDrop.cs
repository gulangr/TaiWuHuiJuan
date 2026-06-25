using System;
using System.Diagnostics;
using UnityEngine;

namespace GearMate
{
	// Token: 0x0200061F RID: 1567
	public class ItemDrop : MonoBehaviour
	{
		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06004A2B RID: 18987 RVA: 0x0022BEE0 File Offset: 0x0022A0E0
		// (remove) Token: 0x06004A2C RID: 18988 RVA: 0x0022BF18 File Offset: 0x0022A118
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnTrigger;

		// Token: 0x06004A2D RID: 18989 RVA: 0x0022BF50 File Offset: 0x0022A150
		private void OnTriggerEnter2D(Collider2D other)
		{
			bool flag = this.eff_gearmate_zhujian_tubiaozha == null;
			if (!flag)
			{
				bool flag2 = other.GetComponent<ItemDrop>() != null;
				if (!flag2)
				{
					GameObject destroyParticle = Object.Instantiate<GameObject>(this.eff_gearmate_zhujian_tubiaozha, base.transform.parent);
					destroyParticle.SetActive(true);
					destroyParticle.transform.position = base.transform.position;
					destroyParticle.GetComponent<ParticleSystem>().Play();
					bool flag3 = !ItemDrop.audioLock;
					if (flag3)
					{
						AudioManager.Instance.PlaySound(string.Format("SFX_GearMate_machine_falling_0{0}", Random.Range(1, 4)), false, false);
						ItemDrop.audioLock = true;
						SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
						{
							ItemDrop.audioLock = false;
						});
					}
					Action onTrigger = this.OnTrigger;
					if (onTrigger != null)
					{
						onTrigger();
					}
					this.OnTrigger = null;
					Object.Destroy(destroyParticle, 1.5f);
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04003360 RID: 13152
		[NonSerialized]
		public GameObject eff_gearmate_zhujian_tubiaozha;

		// Token: 0x04003362 RID: 13154
		private static bool audioLock;
	}
}
