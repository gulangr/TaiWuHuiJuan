using System;
using UnityEngine;

// Token: 0x020003DB RID: 987
[RequireComponent(typeof(CRawImage))]
public class UIVignetteEffect : MonoBehaviour
{
	// Token: 0x06003B47 RID: 15175 RVA: 0x001E0174 File Offset: 0x001DE374
	private void Awake()
	{
		this._rawImage = base.GetComponent<CRawImage>();
		bool flag = this._rawImage != null && this._rawImage.material != null;
		if (flag)
		{
			this._runtimeMat = new Material(this._rawImage.material);
			this._rawImage.material = this._runtimeMat;
		}
	}

	// Token: 0x06003B48 RID: 15176 RVA: 0x001E01DE File Offset: 0x001DE3DE
	private void OnEnable()
	{
		this.ApplyParameters();
	}

	// Token: 0x06003B49 RID: 15177 RVA: 0x001E01E8 File Offset: 0x001DE3E8
	private void OnDestroy()
	{
		bool flag = this._runtimeMat != null;
		if (flag)
		{
			Object.Destroy(this._runtimeMat);
			this._runtimeMat = null;
		}
	}

	// Token: 0x06003B4A RID: 15178 RVA: 0x001E021C File Offset: 0x001DE41C
	public void SetDarkness(float value)
	{
		this.darkness = Mathf.Clamp01(value);
		bool flag = this._runtimeMat != null;
		if (flag)
		{
			this._runtimeMat.SetFloat(UIVignetteEffect.DarknessId, this.darkness);
		}
	}

	// Token: 0x06003B4B RID: 15179 RVA: 0x001E0260 File Offset: 0x001DE460
	public void SetBlurStrength(float value)
	{
		this.blurStrength = Mathf.Clamp(value, 0f, 0.02f);
		bool flag = this._runtimeMat != null;
		if (flag)
		{
			this._runtimeMat.SetFloat(UIVignetteEffect.BlurStrengthId, this.blurStrength);
		}
	}

	// Token: 0x06003B4C RID: 15180 RVA: 0x001E02AC File Offset: 0x001DE4AC
	private void ApplyParameters()
	{
		bool flag = this._runtimeMat == null;
		if (!flag)
		{
			this._runtimeMat.SetFloat(UIVignetteEffect.VignetteRadiusId, this.vignetteRadius);
			this._runtimeMat.SetFloat(UIVignetteEffect.VignetteSoftnessId, this.vignetteSoftness);
			this._runtimeMat.SetFloat(UIVignetteEffect.EllipseRatioId, this.ellipseRatio);
			this._runtimeMat.SetFloat(UIVignetteEffect.BlurStrengthId, this.blurStrength);
			this._runtimeMat.SetInt(UIVignetteEffect.BlurSampleCountId, this.blurSampleCount);
			this._runtimeMat.SetFloat(UIVignetteEffect.DarknessId, this.darkness);
		}
	}

	// Token: 0x04002AA1 RID: 10913
	private static readonly int VignetteRadiusId = Shader.PropertyToID("_VignetteRadius");

	// Token: 0x04002AA2 RID: 10914
	private static readonly int VignetteSoftnessId = Shader.PropertyToID("_VignetteSoftness");

	// Token: 0x04002AA3 RID: 10915
	private static readonly int EllipseRatioId = Shader.PropertyToID("_EllipseRatio");

	// Token: 0x04002AA4 RID: 10916
	private static readonly int BlurStrengthId = Shader.PropertyToID("_BlurStrength");

	// Token: 0x04002AA5 RID: 10917
	private static readonly int BlurSampleCountId = Shader.PropertyToID("_BlurSampleCount");

	// Token: 0x04002AA6 RID: 10918
	private static readonly int DarknessId = Shader.PropertyToID("_Darkness");

	// Token: 0x04002AA7 RID: 10919
	[Header("Vignette")]
	[SerializeField]
	[Range(0f, 2f)]
	private float vignetteRadius = 0.8f;

	// Token: 0x04002AA8 RID: 10920
	[SerializeField]
	[Range(0f, 1f)]
	private float vignetteSoftness = 0.4f;

	// Token: 0x04002AA9 RID: 10921
	[SerializeField]
	[Range(0.3f, 1f)]
	private float ellipseRatio = 0.6f;

	// Token: 0x04002AAA RID: 10922
	[SerializeField]
	[Range(0f, 1f)]
	private float darkness = 0.7f;

	// Token: 0x04002AAB RID: 10923
	[Header("Blur")]
	[SerializeField]
	[Range(0f, 0.02f)]
	private float blurStrength = 0.006f;

	// Token: 0x04002AAC RID: 10924
	[SerializeField]
	[Range(4f, 32f)]
	private int blurSampleCount = 16;

	// Token: 0x04002AAD RID: 10925
	private CRawImage _rawImage;

	// Token: 0x04002AAE RID: 10926
	private Material _runtimeMat;
}
