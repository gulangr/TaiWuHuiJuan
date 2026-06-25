using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AFB RID: 2811
	public class SpecialCombatCharAsset : ScriptableObject, ISerializationCallbackReceiver
	{
		// Token: 0x06008A56 RID: 35414 RVA: 0x00400E7C File Offset: 0x003FF07C
		public void OnBeforeSerialize()
		{
			this.ParticleKeys.Clear();
			this.ParticleValues.Clear();
			this.AudioKeys.Clear();
			this.AudioValues.Clear();
			foreach (KeyValuePair<string, GameObject> kvp in this.ParticleDict)
			{
				this.ParticleKeys.Add(kvp.Key);
				this.ParticleValues.Add(kvp.Value);
			}
			foreach (KeyValuePair<string, AudioClip> kvp2 in this.AudioDict)
			{
				this.AudioKeys.Add(kvp2.Key);
				this.AudioValues.Add(kvp2.Value);
			}
		}

		// Token: 0x06008A57 RID: 35415 RVA: 0x00400F88 File Offset: 0x003FF188
		public void OnAfterDeserialize()
		{
			this.ParticleDict = new Dictionary<string, GameObject>();
			this.AudioDict = new Dictionary<string, AudioClip>();
			for (int i = 0; i < Mathf.Min(this.ParticleKeys.Count, this.ParticleValues.Count); i++)
			{
				this.ParticleDict.Add(this.ParticleKeys[i], this.ParticleValues[i]);
			}
			for (int j = 0; j < Mathf.Min(this.AudioKeys.Count, this.AudioValues.Count); j++)
			{
				this.AudioDict.Add(this.AudioKeys[j], this.AudioValues[j]);
			}
		}

		// Token: 0x04006A1B RID: 27163
		public SkeletonDataAsset SkeletonData;

		// Token: 0x04006A1C RID: 27164
		public SkeletonDataAsset PetSkeletonData;

		// Token: 0x04006A1D RID: 27165
		public Dictionary<string, GameObject> ParticleDict = new Dictionary<string, GameObject>();

		// Token: 0x04006A1E RID: 27166
		public Dictionary<string, AudioClip> AudioDict = new Dictionary<string, AudioClip>();

		// Token: 0x04006A1F RID: 27167
		public List<string> ParticleKeys = new List<string>();

		// Token: 0x04006A20 RID: 27168
		public List<GameObject> ParticleValues = new List<GameObject>();

		// Token: 0x04006A21 RID: 27169
		public List<string> AudioKeys = new List<string>();

		// Token: 0x04006A22 RID: 27170
		public List<AudioClip> AudioValues = new List<AudioClip>();
	}
}
