using System;
using GameData.Adventure;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C6D RID: 3181
	[ExecuteAlways]
	public class AdventurePointLight : MonoBehaviour
	{
		// Token: 0x170010F2 RID: 4338
		// (get) Token: 0x0600A1E8 RID: 41448 RVA: 0x004BB1D0 File Offset: 0x004B93D0
		// (set) Token: 0x0600A1E9 RID: 41449 RVA: 0x004BB1D8 File Offset: 0x004B93D8
		public AdventureBlockIndex BlockIndex
		{
			get
			{
				return this._blockIndex;
			}
			set
			{
				this._blockIndex = value;
			}
		}

		// Token: 0x170010F3 RID: 4339
		// (get) Token: 0x0600A1EA RID: 41450 RVA: 0x004BB1E4 File Offset: 0x004B93E4
		public float CurrentIntensity
		{
			get
			{
				bool flag = this.BreathingPeriod <= 0.001f;
				float result;
				if (flag)
				{
					result = this.Intensity;
				}
				else
				{
					float t = Time.time * (6.2831855f / this.BreathingPeriod);
					float factor = (Mathf.Sin(t) + 1f) * 0.5f;
					result = Mathf.Lerp(this.MinIntensity, this.Intensity, factor);
				}
				return result;
			}
		}

		// Token: 0x0600A1EB RID: 41451 RVA: 0x004BB24C File Offset: 0x004B944C
		private void OnEnable()
		{
			bool flag = AdventureLightingManager.Instance != null;
			if (flag)
			{
				AdventureLightingManager.Instance.RegisterLight(this);
			}
			else
			{
				AdventureLightingManager.AddPendingLight(this);
			}
		}

		// Token: 0x0600A1EC RID: 41452 RVA: 0x004BB280 File Offset: 0x004B9480
		private void OnDisable()
		{
			bool flag = AdventureLightingManager.Instance != null;
			if (flag)
			{
				AdventureLightingManager.Instance.UnregisterLight(this);
			}
			AdventureLightingManager.RemovePendingLight(this);
		}

		// Token: 0x04007DD4 RID: 32212
		public AdventurePointLight.ShapeType Shape = AdventurePointLight.ShapeType.OneByOne;

		// Token: 0x04007DD5 RID: 32213
		public AdventurePointLight.LightMode Mode = AdventurePointLight.LightMode.Uniform;

		// Token: 0x04007DD6 RID: 32214
		private AdventureBlockIndex _blockIndex = AdventureBlockIndex.Center;

		// Token: 0x04007DD7 RID: 32215
		public float VirtualZ = 1f;

		// Token: 0x04007DD8 RID: 32216
		[Range(0f, 90f)]
		public float Angle = 45f;

		// Token: 0x04007DD9 RID: 32217
		public float Range = 1f;

		// Token: 0x04007DDA RID: 32218
		public bool NoRangeClamp;

		// Token: 0x04007DDB RID: 32219
		public Color LightColor = Color.white;

		// Token: 0x04007DDC RID: 32220
		public float Intensity = 1f;

		// Token: 0x04007DDD RID: 32221
		public int FullIntensityRange = 1;

		// Token: 0x04007DDE RID: 32222
		public float BreathingPeriod = 0f;

		// Token: 0x04007DDF RID: 32223
		public float MinIntensity = 0f;

		// Token: 0x04007DE0 RID: 32224
		[Range(0f, 16f)]
		public int Priority = 7;

		// Token: 0x020023A6 RID: 9126
		public enum LightMode
		{
			// Token: 0x0400DF8A RID: 57226
			Uniform,
			// Token: 0x0400DF8B RID: 57227
			Smooth
		}

		// Token: 0x020023A7 RID: 9127
		public enum ShapeType
		{
			// Token: 0x0400DF8D RID: 57229
			OneByOne = 1,
			// Token: 0x0400DF8E RID: 57230
			ThreeByThree = 3
		}
	}
}
