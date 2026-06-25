using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views
{
	// Token: 0x020006F2 RID: 1778
	public class NewFunctionUnlockEffectItem : MonoBehaviour
	{
		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x0600545B RID: 21595 RVA: 0x00271FB7 File Offset: 0x002701B7
		public int UnlockType
		{
			get
			{
				return this.unlockType;
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x0600545C RID: 21596 RVA: 0x00271FBF File Offset: 0x002701BF
		private GameObject Root
		{
			get
			{
				return (this.effectRoot != null) ? this.effectRoot : base.gameObject;
			}
		}

		// Token: 0x0600545D RID: 21597 RVA: 0x00271FDD File Offset: 0x002701DD
		private void Awake()
		{
			this.ApplyParticleSortingOrder();
		}

		// Token: 0x0600545E RID: 21598 RVA: 0x00271FE7 File Offset: 0x002701E7
		public void Play()
		{
			this.Root.SetActive(true);
		}

		// Token: 0x0600545F RID: 21599 RVA: 0x00271FF7 File Offset: 0x002701F7
		public void Stop()
		{
			this.Root.SetActive(false);
		}

		// Token: 0x06005460 RID: 21600 RVA: 0x00272008 File Offset: 0x00270208
		private void ApplyParticleSortingOrder()
		{
			ParticleSystemRenderer[] renderers = this.Root.GetComponentsInChildren<ParticleSystemRenderer>(true);
			for (int i = 0; i < renderers.Length; i++)
			{
				int order = renderers[i].sortingOrder;
				renderers[i].sortingOrder = order % 10 + 800;
			}
		}

		// Token: 0x04003921 RID: 14625
		private const int SortingOrderBase = 800;

		// Token: 0x04003922 RID: 14626
		[SerializeField]
		[FormerlySerializedAs("artAssetId")]
		[FormerlySerializedAs("unlockType")]
		[Tooltip("对应 NewFunctionUnlock 配置 Type 字段")]
		private int unlockType;

		// Token: 0x04003923 RID: 14627
		[SerializeField]
		[Tooltip("留空则使用本节点")]
		private GameObject effectRoot;
	}
}
