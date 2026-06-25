using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x0200095A RID: 2394
	public class MakeSubTypeWordEffectEmitter : MonoBehaviour
	{
		// Token: 0x0600726A RID: 29290 RVA: 0x00352B4C File Offset: 0x00350D4C
		private void Awake()
		{
			bool flag = !PoolManager.HasData("MakeSubTypeWordEffectEmitter_WordArrowEffect");
			if (flag)
			{
				PoolManager.SetSrcObjectWithTurnOff("MakeSubTypeWordEffectEmitter_WordArrowEffect", this.wordArrowEffectPrefab);
			}
		}

		// Token: 0x0600726B RID: 29291 RVA: 0x00352B7E File Offset: 0x00350D7E
		private void OnDestroy()
		{
			PoolManager.RemoveData("MakeSubTypeWordEffectEmitter_WordArrowEffect");
		}

		// Token: 0x0600726C RID: 29292 RVA: 0x00352B8C File Offset: 0x00350D8C
		public void EmitArrowEffect(Transform from)
		{
			GameObject effect = PoolManager.GetObject("MakeSubTypeWordEffectEmitter_WordArrowEffect");
			effect.transform.SetParent(base.transform);
			effect.SetActive(false);
			effect.transform.position = from.position;
			effect.SetActive(true);
			this._activeArrowEffectLookup.Add(effect);
			effect.transform.DOMove(this.targetSlot.transform.position, this.animDurArrowEffect, false).SetAutoKill(true).OnComplete(delegate
			{
				this._activeArrowEffectLookup.Remove(effect);
				PoolManager.Destroy("MakeSubTypeWordEffectEmitter_WordArrowEffect", effect);
				bool flag = this.targetSlot != null && this.targetSlot.gameObject.activeInHierarchy;
				if (flag)
				{
					this.targetSlot.PlayEffectWordsAffected();
				}
			});
		}

		// Token: 0x0600726D RID: 29293 RVA: 0x00352C50 File Offset: 0x00350E50
		private void OnDisable()
		{
			foreach (GameObject effect in this._activeArrowEffectLookup)
			{
				bool flag = effect != null;
				if (flag)
				{
					effect.transform.DOKill(false);
					PoolManager.Destroy("MakeSubTypeWordEffectEmitter_WordArrowEffect", effect);
				}
			}
			this._activeArrowEffectLookup.Clear();
		}

		// Token: 0x040054C3 RID: 21699
		[SerializeField]
		private GameObject wordArrowEffectPrefab;

		// Token: 0x040054C4 RID: 21700
		[SerializeField]
		private MakeSlotEffectHandler targetSlot;

		// Token: 0x040054C5 RID: 21701
		public const string PoolKey_ArrowEffect = "MakeSubTypeWordEffectEmitter_WordArrowEffect";

		// Token: 0x040054C6 RID: 21702
		public float animDurArrowEffect = 0.36f;

		// Token: 0x040054C7 RID: 21703
		private HashSet<GameObject> _activeArrowEffectLookup = new HashSet<GameObject>();
	}
}
