using System;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x02000161 RID: 353
[RequireComponent(typeof(IHasSkeletonDataAsset), typeof(IAnimationStateComponent))]
public class RawAnimationPlayer : MonoBehaviour
{
	// Token: 0x06001369 RID: 4969 RVA: 0x000772E4 File Offset: 0x000754E4
	private void Awake()
	{
		Tester.Assert(this.rawAnimation != null, "");
		IHasSkeletonDataAsset dataAsset = base.GetComponent<IHasSkeletonDataAsset>();
		Spine.Animation spineAnimation = this.rawAnimation.GetAnimation(dataAsset.SkeletonDataAsset);
		IAnimationStateComponent animationState = base.GetComponent<IAnimationStateComponent>();
		animationState.AnimationState.SetAnimation(0, spineAnimation, true);
	}

	// Token: 0x04001042 RID: 4162
	public RawAnimationAsset rawAnimation;
}
