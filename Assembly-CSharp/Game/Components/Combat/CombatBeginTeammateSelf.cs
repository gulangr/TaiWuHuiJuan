using System;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Combat
{
	// Token: 0x02000F03 RID: 3843
	public class CombatBeginTeammateSelf : BaseCombatBeginTeammate
	{
		// Token: 0x0600B12E RID: 45358 RVA: 0x0050C4AC File Offset: 0x0050A6AC
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

		// Token: 0x0600B12F RID: 45359 RVA: 0x0050C578 File Offset: 0x0050A778
		public override ParticleSystem GetEffectYuanshanThreeDemon(bool isDemon, bool isEnemy)
		{
			return (isDemon ^ isEnemy) ? this.eff_yuanshanthreedemon_qiehuanhong : this.eff_yuanshanthreedemon_qiehuanlan;
		}

		// Token: 0x0400894C RID: 35148
		[SerializeField]
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400894D RID: 35149
		[SerializeField]
		public TextMeshProUGUI characterName;

		// Token: 0x0400894E RID: 35150
		[SerializeField]
		public CButton button;

		// Token: 0x0400894F RID: 35151
		[SerializeField]
		public CButton yuanshanThreeVitalsBtn;

		// Token: 0x04008950 RID: 35152
		[SerializeField]
		public ParticleSystem lightEffect;

		// Token: 0x04008951 RID: 35153
		[SerializeField]
		public Bubble betrayBubble;

		// Token: 0x04008952 RID: 35154
		[SerializeField]
		public Bubble commandBubble;

		// Token: 0x04008953 RID: 35155
		[SerializeField]
		public RectTransform commandHolder;

		// Token: 0x04008954 RID: 35156
		[SerializeField]
		public CombatDefeatMarkTotalCount defeatMarksBack;

		// Token: 0x04008955 RID: 35157
		[SerializeField]
		public GameObject invalidAvatar;

		// Token: 0x04008956 RID: 35158
		[SerializeField]
		public ParticleSystem eff_yuanshanthreedemon_qiehuanlan;

		// Token: 0x04008957 RID: 35159
		[SerializeField]
		public ParticleSystem eff_yuanshanthreedemon_qiehuanhong;

		// Token: 0x04008958 RID: 35160
		[SerializeField]
		public ParticleSystem effectIronPlateChar;

		// Token: 0x04008959 RID: 35161
		[SerializeField]
		public TooltipInvoker mouseTipDisplayer;

		// Token: 0x0400895A RID: 35162
		[SerializeField]
		public CImage buttonFrame;

		// Token: 0x0400895B RID: 35163
		[Header("NormalState")]
		[SerializeField]
		public Sprite normalSprite;

		// Token: 0x0400895C RID: 35164
		public SpriteState normalState;

		// Token: 0x0400895D RID: 35165
		[Header("VitalState")]
		[SerializeField]
		public Sprite vitalSprite;

		// Token: 0x0400895E RID: 35166
		public SpriteState vitalState;
	}
}
