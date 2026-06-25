using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F00 RID: 3840
	public abstract class BaseCombatBeginTeammate : MonoBehaviour
	{
		// Token: 0x0600B125 RID: 45349
		public abstract void PlayCommandBubbleAnim(float showDelay = 0f, Action onShow = null, float disappearDelay = 5f, Action onEnd = null);

		// Token: 0x0600B126 RID: 45350
		public abstract ParticleSystem GetEffectYuanshanThreeDemon(bool isDemon, bool isEnemy);

		// Token: 0x0600B127 RID: 45351 RVA: 0x0050C2D8 File Offset: 0x0050A4D8
		public void ClearSequenceAnim()
		{
			bool flag = this.commandBubbleSeq == null;
			if (!flag)
			{
				this.commandBubbleSeq.Pause<Sequence>();
				this.commandBubbleSeq.Kill(false);
				this.commandBubbleSeq = null;
			}
		}

		// Token: 0x0400892A RID: 35114
		protected Sequence commandBubbleSeq;
	}
}
