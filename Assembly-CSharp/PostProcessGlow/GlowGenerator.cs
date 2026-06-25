using System;
using System.Collections;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x020005FE RID: 1534
	public class GlowGenerator : MonoBehaviour
	{
		// Token: 0x0600485B RID: 18523 RVA: 0x0021E6C4 File Offset: 0x0021C8C4
		private void Awake()
		{
			this.SetupCamera();
		}

		// Token: 0x0600485C RID: 18524 RVA: 0x0021E6D0 File Offset: 0x0021C8D0
		private void OnEnable()
		{
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				base.StartCoroutine(this.Generate());
			}
		}

		// Token: 0x0600485D RID: 18525 RVA: 0x0021E6FC File Offset: 0x0021C8FC
		private void SetupCamera()
		{
			bool flag = this.glowCamera.targetTexture == null;
			if (flag)
			{
				RenderTexture renderTexture = new RenderTexture(AspectRatioController.ViewSize.x, AspectRatioController.ViewSize.y, (int)this.glowCamera.depth);
				this.glowCamera.targetTexture = renderTexture;
			}
			bool flag2 = this.outputImage.texture == null;
			if (flag2)
			{
				RenderTexture renderTexture2 = new RenderTexture(AspectRatioController.ViewSize.x, AspectRatioController.ViewSize.y, (int)this.glowCamera.depth);
				this.outputImage.texture = renderTexture2;
			}
		}

		// Token: 0x0600485E RID: 18526 RVA: 0x0021E7AC File Offset: 0x0021C9AC
		private IEnumerator Generate()
		{
			for (;;)
			{
				bool flag = !GlowTargetController.Instance || !GlowTargetController.Instance.HasGlowTargets;
				if (flag)
				{
					this.outputImage.gameObject.SetActive(false);
					yield return null;
				}
				else
				{
					RenderTexture source = this.glowCamera.targetTexture;
					GlowTargetController.Instance.SetAllAsOriginalColor();
					yield return null;
					RenderTexture originalBackup = RenderTexture.GetTemporary(source.width, source.height);
					Graphics.Blit(source, originalBackup);
					GlowTargetController.Instance.SetAllAsGlowColor();
					yield return null;
					RenderTexture coloredSource = RenderTexture.GetTemporary(source.width, source.height);
					Graphics.Blit(source, coloredSource);
					bool flag2 = this.downSampleMode == GlowGenerator.DownSampleMode.Half;
					RenderTexture rt;
					RenderTexture rt2;
					if (flag2)
					{
						rt = RenderTexture.GetTemporary(coloredSource.width / 2, coloredSource.height / 2);
						rt2 = RenderTexture.GetTemporary(coloredSource.width / 2, coloredSource.height / 2);
						Graphics.Blit(coloredSource, rt);
					}
					else
					{
						bool flag3 = this.downSampleMode == GlowGenerator.DownSampleMode.Quarter;
						if (flag3)
						{
							rt = RenderTexture.GetTemporary(coloredSource.width / 4, coloredSource.height / 4);
							rt2 = RenderTexture.GetTemporary(coloredSource.width / 4, coloredSource.height / 4);
							Graphics.Blit(coloredSource, rt, this.glowMaterial, 0);
						}
						else
						{
							rt = RenderTexture.GetTemporary(coloredSource.width, coloredSource.height);
							rt2 = RenderTexture.GetTemporary(coloredSource.width, coloredSource.height);
							Graphics.Blit(coloredSource, rt);
						}
					}
					RenderTexture.ReleaseTemporary(coloredSource);
					this.glowMaterial.SetFloat(GlowGenerator.DilateStrength, this.dilateStrength);
					this.glowMaterial.SetFloat(GlowGenerator.GlowAlpha, this.glowAlpha);
					int num;
					for (int i = 0; i < this.dilateIterations; i = num + 1)
					{
						int dilatePass = this.GetDilatePass();
						Graphics.Blit(rt, rt2, this.glowMaterial, dilatePass);
						Graphics.Blit(rt2, rt);
						num = i;
					}
					for (int j = 0; j < this.iteration; j = num + 1)
					{
						Graphics.Blit(rt, rt2, this.glowMaterial, 1);
						Graphics.Blit(rt2, rt, this.glowMaterial, 2);
						num = j;
					}
					RenderTexture.ReleaseTemporary(rt2);
					RenderTexture finalOutput = this.outputImage.texture as RenderTexture;
					this.glowMaterial.SetTexture(GlowGenerator.OriginalTex, originalBackup);
					Graphics.Blit(rt, finalOutput, this.glowMaterial, 7);
					RenderTexture.ReleaseTemporary(rt);
					RenderTexture.ReleaseTemporary(originalBackup);
					this.outputImage.gameObject.SetActive(true);
					rt = null;
					rt2 = null;
					source = null;
					originalBackup = null;
					coloredSource = null;
					finalOutput = null;
				}
			}
			yield break;
		}

		// Token: 0x0600485F RID: 18527 RVA: 0x0021E7BC File Offset: 0x0021C9BC
		private int GetDilatePass()
		{
			switch (this.dilateMode)
			{
			case GlowGenerator.DilateMode.Simple:
				return 4;
			case GlowGenerator.DilateMode.ColorPreserving:
				return 5;
			}
			return 3;
		}

		// Token: 0x040031F4 RID: 12788
		private static readonly int OriginalTex = Shader.PropertyToID("_OriginalTex");

		// Token: 0x040031F5 RID: 12789
		private static readonly int DilateStrength = Shader.PropertyToID("_DilateStrength");

		// Token: 0x040031F6 RID: 12790
		private static readonly int GlowAlpha = Shader.PropertyToID("_GlowAlpha");

		// Token: 0x040031F7 RID: 12791
		[SerializeField]
		private Camera glowCamera;

		// Token: 0x040031F8 RID: 12792
		[SerializeField]
		private Material glowMaterial;

		// Token: 0x040031F9 RID: 12793
		[SerializeField]
		private CRawImage outputImage;

		// Token: 0x040031FA RID: 12794
		[SerializeField]
		private GlowGenerator.DownSampleMode downSampleMode = GlowGenerator.DownSampleMode.Quarter;

		// Token: 0x040031FB RID: 12795
		[SerializeField]
		private GlowGenerator.DilateMode dilateMode = GlowGenerator.DilateMode.ChannelMax;

		// Token: 0x040031FC RID: 12796
		[SerializeField]
		[Range(0f, 8f)]
		private int iteration = 3;

		// Token: 0x040031FD RID: 12797
		[SerializeField]
		[Range(0f, 5f)]
		private int dilateIterations = 1;

		// Token: 0x040031FE RID: 12798
		[SerializeField]
		[Range(0.5f, 3f)]
		private float dilateStrength = 1f;

		// Token: 0x040031FF RID: 12799
		[SerializeField]
		[Range(0f, 1f)]
		private float glowAlpha = 1f;

		// Token: 0x020019BB RID: 6587
		private enum DownSampleMode
		{
			// Token: 0x0400B328 RID: 45864
			Off,
			// Token: 0x0400B329 RID: 45865
			Half,
			// Token: 0x0400B32A RID: 45866
			Quarter
		}

		// Token: 0x020019BC RID: 6588
		private enum DilateMode
		{
			// Token: 0x0400B32C RID: 45868
			Simple,
			// Token: 0x0400B32D RID: 45869
			ColorPreserving,
			// Token: 0x0400B32E RID: 45870
			ChannelMax
		}
	}
}
