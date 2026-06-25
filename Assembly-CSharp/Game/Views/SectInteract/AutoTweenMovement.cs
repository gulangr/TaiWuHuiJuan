using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x0200099E RID: 2462
	public class AutoTweenMovement : MonoBehaviour
	{
		// Token: 0x06007676 RID: 30326 RVA: 0x0037370C File Offset: 0x0037190C
		private void Awake()
		{
			this.rt.DOAnchorPosY(this.rt.anchoredPosition.y + this.distance, this.duration, false).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}

		// Token: 0x0400595D RID: 22877
		public RectTransform rt;

		// Token: 0x0400595E RID: 22878
		[SerializeField]
		private float distance = 20f;

		// Token: 0x0400595F RID: 22879
		[SerializeField]
		private float duration = 2f;
	}
}
