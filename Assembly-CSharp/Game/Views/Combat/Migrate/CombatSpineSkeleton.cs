using System;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B58 RID: 2904
	public class CombatSpineSkeleton : MonoBehaviour
	{
		// Token: 0x04006DED RID: 28141
		public int UserInt;

		// Token: 0x04006DEE RID: 28142
		public CImage commandPrepareBar;

		// Token: 0x04006DEF RID: 28143
		public RectTransform commandPrepare;

		// Token: 0x04006DF0 RID: 28144
		public TextMeshProUGUI commandName;

		// Token: 0x04006DF1 RID: 28145
		public SkeletonAnimation pet;

		// Token: 0x04006DF2 RID: 28146
		public CImage currPosMark;

		// Token: 0x04006DF3 RID: 28147
		public ParticleSystem changeTrickParticle;
	}
}
