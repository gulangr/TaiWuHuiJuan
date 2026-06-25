using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001020 RID: 4128
	[ExecuteInEditMode]
	public class RadialBlurEffect : MonoBehaviour
	{
		// Token: 0x0600BD0B RID: 48395 RVA: 0x0055EA78 File Offset: 0x0055CC78
		private void Refresh()
		{
			Graphic grp = base.GetComponent<Graphic>();
			Material material;
			bool flag;
			if (grp != null)
			{
				material = grp.material;
				flag = (material != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				material.SetVector(RadialBlurEffect.BlurCenter, new Vector4(this.centerX, this.centerY, 0f, 0f));
				material.SetFloat(RadialBlurEffect.BlurFactor, this.blurFactor);
				material.SetFloat(RadialBlurEffect.SampleCount, (float)this.sampleCount);
				material.SetFloat(RadialBlurEffect.LerpFactor, this.lerpFactor);
			}
		}

		// Token: 0x04009169 RID: 37225
		private static readonly int BlurCenter = Shader.PropertyToID("_BlurCenter");

		// Token: 0x0400916A RID: 37226
		private static readonly int BlurFactor = Shader.PropertyToID("_BlurFactor");

		// Token: 0x0400916B RID: 37227
		private static readonly int SampleCount = Shader.PropertyToID("_SampleCount");

		// Token: 0x0400916C RID: 37228
		private static readonly int LerpFactor = Shader.PropertyToID("_LerpFactor");

		// Token: 0x0400916D RID: 37229
		[Range(0f, 0.1f)]
		public float blurFactor = 0.02f;

		// Token: 0x0400916E RID: 37230
		[Range(3f, 30f)]
		public int sampleCount = 10;

		// Token: 0x0400916F RID: 37231
		[Range(0f, 1f)]
		public float lerpFactor = 1f;

		// Token: 0x04009170 RID: 37232
		[Range(0f, 1f)]
		public float centerX = 0.5f;

		// Token: 0x04009171 RID: 37233
		[Range(0f, 1f)]
		public float centerY = 0.5f;
	}
}
