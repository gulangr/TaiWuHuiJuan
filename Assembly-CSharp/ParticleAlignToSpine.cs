using System;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class ParticleAlignToSpine : MonoBehaviour
{
	// Token: 0x060004DC RID: 1244 RVA: 0x00021F08 File Offset: 0x00020108
	private void Awake()
	{
		bool flag = null != this.Particle;
		if (flag)
		{
			bool flag2 = null != this.SpineGraphic;
			if (flag2)
			{
				this.SpineGraphic.AnimationState.Start += delegate(TrackEntry entry)
				{
					this.Particle.Play(true);
				};
			}
			bool flag3 = null != this.SpineAnimation;
			if (flag3)
			{
				this.SpineAnimation.AnimationState.Start += delegate(TrackEntry entry)
				{
					this.Particle.Play(true);
				};
			}
		}
	}

	// Token: 0x040003E4 RID: 996
	public ParticleSystem Particle;

	// Token: 0x040003E5 RID: 997
	public SkeletonGraphic SpineGraphic;

	// Token: 0x040003E6 RID: 998
	public SkeletonAnimation SpineAnimation;
}
