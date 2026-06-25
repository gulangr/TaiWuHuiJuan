using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views.Combat
{
	// Token: 0x02000AFA RID: 2810
	public class CombatSkillAsset : ScriptableObject
	{
		// Token: 0x04006A18 RID: 27160
		public RawAnimationAsset[] animations;

		// Token: 0x04006A19 RID: 27161
		[FormerlySerializedAs("Particles")]
		public GameObject[] particles;

		// Token: 0x04006A1A RID: 27162
		[FormerlySerializedAs("Audios")]
		public AudioClip[] audios;
	}
}
