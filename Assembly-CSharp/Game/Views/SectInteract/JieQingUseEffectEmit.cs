using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A4 RID: 2468
	public class JieQingUseEffectEmit : MonoBehaviour
	{
		// Token: 0x060076F1 RID: 30449 RVA: 0x003760F5 File Offset: 0x003742F5
		private void Awake()
		{
			this.effPool = new PoolItem("ViewJieQingInteract_EmitEffect", this.emitEffectPrefab);
		}

		// Token: 0x060076F2 RID: 30450 RVA: 0x0037610E File Offset: 0x0037430E
		private void OnDestroy()
		{
			this.effPool.Destroy();
		}

		// Token: 0x060076F3 RID: 30451 RVA: 0x00376120 File Offset: 0x00374320
		public void EmitArrowEffect(Transform from)
		{
			GameObject effect = this.effPool.GetObject();
			effect.transform.SetParent(base.transform);
			effect.SetActive(false);
			effect.transform.position = from.position;
			effect.transform.localScale = Vector3.one;
			effect.SetActive(true);
			this._activeArrowEffectLookup.Add(effect);
			effect.transform.DOMove(this.targetPos.transform.position, this.animDurArrowEffect, false).SetAutoKill(true).SetEase(Ease.InOutSine).OnComplete(delegate
			{
				this._activeArrowEffectLookup.Remove(effect);
				this.effPool.DestroyObject(effect);
				bool flag = this.targetPosEffect != null;
				if (flag)
				{
					this.targetPosEffect.Play();
				}
			});
		}

		// Token: 0x060076F4 RID: 30452 RVA: 0x00376204 File Offset: 0x00374404
		private void OnDisable()
		{
			foreach (GameObject effect in this._activeArrowEffectLookup)
			{
				bool flag = effect != null;
				if (flag)
				{
					effect.transform.DOKill(false);
					this.effPool.DestroyObject(effect);
				}
			}
			this._activeArrowEffectLookup.Clear();
		}

		// Token: 0x040059C2 RID: 22978
		[SerializeField]
		private GameObject emitEffectPrefab;

		// Token: 0x040059C3 RID: 22979
		[SerializeField]
		private Transform targetPos;

		// Token: 0x040059C4 RID: 22980
		[SerializeField]
		private ParticleSystem targetPosEffect;

		// Token: 0x040059C5 RID: 22981
		private PoolItem effPool;

		// Token: 0x040059C6 RID: 22982
		public float animDurArrowEffect = 0.36f;

		// Token: 0x040059C7 RID: 22983
		private HashSet<GameObject> _activeArrowEffectLookup = new HashSet<GameObject>();
	}
}
