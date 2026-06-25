using System;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Combat;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F02 RID: 3842
	public class CombatBeginTeammateEnemy : BaseCombatBeginTeammate
	{
		// Token: 0x0600B12B RID: 45355 RVA: 0x0050C3B0 File Offset: 0x0050A5B0
		public override void PlayCommandBubbleAnim(float showDelay = 0f, Action onShow = null, float disappearDelay = 5f, Action onEnd = null)
		{
			base.ClearSequenceAnim();
			this.commandBubble.gameObject.SetActive(false);
			this.commandBubbleSeq = DOTween.Sequence();
			bool flag = showDelay > 0f;
			if (flag)
			{
				this.commandBubbleSeq.AppendInterval(showDelay);
			}
			this.commandBubbleSeq.AppendCallback(delegate
			{
				Action onShow2 = onShow;
				if (onShow2 != null)
				{
					onShow2();
				}
			});
			bool flag2 = disappearDelay > 0f;
			if (flag2)
			{
				this.commandBubbleSeq.AppendInterval(disappearDelay);
			}
			this.commandBubbleSeq.AppendCallback(delegate
			{
				Action onEnd2 = onEnd;
				if (onEnd2 != null)
				{
					onEnd2();
				}
			});
			this.commandBubbleSeq.SetUpdate(UpdateType.Late, true);
			this.commandBubbleSeq.Restart(true, -1f);
		}

		// Token: 0x0600B12C RID: 45356 RVA: 0x0050C47C File Offset: 0x0050A67C
		public override ParticleSystem GetEffectYuanshanThreeDemon(bool isDemon, bool isEnemy)
		{
			return (isDemon ^ isEnemy) ? this.eff_yuanshanthreedemon_qiehuanhong : this.eff_yuanshanthreedemon_qiehuanlan;
		}

		// Token: 0x0400893C RID: 35132
		[SerializeField]
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400893D RID: 35133
		[SerializeField]
		public TextMeshProUGUI characterName;

		// Token: 0x0400893E RID: 35134
		[SerializeField]
		public CButton button;

		// Token: 0x0400893F RID: 35135
		[SerializeField]
		public ParticleSystem lightEffect;

		// Token: 0x04008940 RID: 35136
		[SerializeField]
		public Bubble betrayBubble;

		// Token: 0x04008941 RID: 35137
		[SerializeField]
		public Bubble commandBubble;

		// Token: 0x04008942 RID: 35138
		[SerializeField]
		public RectTransform commandHolder;

		// Token: 0x04008943 RID: 35139
		[SerializeField]
		public CombatDefeatMarkTotalCount defeatMarksBack;

		// Token: 0x04008944 RID: 35140
		[SerializeField]
		public ParticleSystem eff_yuanshanthreedemon_qiehuanlan;

		// Token: 0x04008945 RID: 35141
		[SerializeField]
		public ParticleSystem eff_yuanshanthreedemon_qiehuanhong;

		// Token: 0x04008946 RID: 35142
		[SerializeField]
		public TooltipInvoker mouseTipDisplayer;

		// Token: 0x04008947 RID: 35143
		[SerializeField]
		public SkeletonGraphic betrayedTips0;

		// Token: 0x04008948 RID: 35144
		[SerializeField]
		public SkeletonGraphic betrayedTips1;

		// Token: 0x04008949 RID: 35145
		[SerializeField]
		public SkeletonGraphic betrayAni0;

		// Token: 0x0400894A RID: 35146
		[SerializeField]
		public SkeletonGraphic betrayAni1;

		// Token: 0x0400894B RID: 35147
		[SerializeField]
		public CImage buttonFrame;
	}
}
