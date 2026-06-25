using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C47 RID: 3143
	public class TimeBallEffect : MonoBehaviour
	{
		// Token: 0x06009FC4 RID: 40900 RVA: 0x004A9FA8 File Offset: 0x004A81A8
		public void InitEffect()
		{
			this.fillPatricleMatList.Clear();
			for (int i = 0; i < this.fillParticleList.Count; i++)
			{
				ParticleSystemRenderer r = this.fillParticleList[i].Particle.GetComponent<ParticleSystemRenderer>();
				this.fillPatricleMatList.Add(new TimeBallEffect.FillParticle
				{
					Renderer = r,
					ColorPriority = new ValueTuple<float, Color>[]
					{
						new ValueTuple<float, Color>(1f, this.fillParticleList[i].GoldColor),
						new ValueTuple<float, Color>(0.25f, this.fillParticleList[i].BlueColor),
						new ValueTuple<float, Color>(float.MinValue, this.fillParticleList[i].GreyColor)
					}
				});
			}
		}

		// Token: 0x06009FC5 RID: 40901 RVA: 0x004AA088 File Offset: 0x004A8288
		private float Remap01ToXy(float min, float max, float amount)
		{
			float rate = Mathf.InverseLerp(0f, 1f, amount);
			return Mathf.Lerp(min, max, rate);
		}

		// Token: 0x06009FC6 RID: 40902 RVA: 0x004AA0B4 File Offset: 0x004A82B4
		public void SetParticleFillAmount(float amount)
		{
			this.fullParticle.gameObject.SetActive(amount >= 1f);
			for (int i = 0; i < this.fillPatricleMatList.Count; i++)
			{
				TimeBallEffect.FillParticle fp = this.fillPatricleMatList[i];
				MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
				propertyBlock.SetFloat(TimeBallEffect.MatProgress, this.Remap01ToXy(1E-05f, 1.15f, amount));
				foreach (ValueTuple<float, Color> valueTuple in fp.ColorPriority)
				{
					float threshold = valueTuple.Item1;
					Color color = valueTuple.Item2;
					bool flag = amount >= threshold;
					if (flag)
					{
						propertyBlock.SetColor(TimeBallEffect.MatColor, color);
						break;
					}
				}
				fp.Renderer.SetPropertyBlock(propertyBlock);
			}
			this.uiParticle.SetAllDirty();
		}

		// Token: 0x04007BA2 RID: 31650
		[SerializeField]
		internal List<TimeBallEffect.FillParticleData> fillParticleList;

		// Token: 0x04007BA3 RID: 31651
		[SerializeField]
		internal GameObject fullParticle;

		// Token: 0x04007BA4 RID: 31652
		[SerializeField]
		internal UIParticle uiParticle;

		// Token: 0x04007BA5 RID: 31653
		private static readonly int MatProgress = Shader.PropertyToID("_Progress");

		// Token: 0x04007BA6 RID: 31654
		private static readonly int MatColor = Shader.PropertyToID("_Color");

		// Token: 0x04007BA7 RID: 31655
		private List<TimeBallEffect.FillParticle> fillPatricleMatList = new List<TimeBallEffect.FillParticle>();

		// Token: 0x02002366 RID: 9062
		[Serializable]
		public class FillParticleData
		{
			// Token: 0x0400DEB6 RID: 57014
			public ParticleSystem Particle;

			// Token: 0x0400DEB7 RID: 57015
			public Color GreyColor;

			// Token: 0x0400DEB8 RID: 57016
			public Color BlueColor;

			// Token: 0x0400DEB9 RID: 57017
			public Color GoldColor;
		}

		// Token: 0x02002367 RID: 9063
		private class FillParticle
		{
			// Token: 0x0400DEBA RID: 57018
			public Renderer Renderer;

			// Token: 0x0400DEBB RID: 57019
			public ValueTuple<float, Color>[] ColorPriority;
		}
	}
}
