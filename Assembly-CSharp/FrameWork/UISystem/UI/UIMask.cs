using System;
using System.Collections;
using UnityEngine;

namespace FrameWork.UISystem.UI
{
	// Token: 0x02000FF6 RID: 4086
	public class UIMask : MonoBehaviour
	{
		// Token: 0x0600BA5D RID: 47709 RVA: 0x0054E65C File Offset: 0x0054C85C
		private void Awake()
		{
			bool flag = this.processMaterial != null;
			if (flag)
			{
				this.processMaterial = new Material(this.processMaterial);
			}
		}

		// Token: 0x0600BA5E RID: 47710 RVA: 0x0054E68B File Offset: 0x0054C88B
		private void OnEnable()
		{
			this.maskRawImage.enabled = false;
			this.UpdateMaskSizeAndTexture();
			this._isInitialized = true;
			this.DoProcess();
		}

		// Token: 0x0600BA5F RID: 47711 RVA: 0x0054E6B0 File Offset: 0x0054C8B0
		private void OnDisable()
		{
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				bool flag2 = this._enableDelayCo != null;
				if (flag2)
				{
					base.StopCoroutine(this._enableDelayCo);
					this._enableDelayCo = null;
				}
				bool isLegacyRegistered = this._isLegacyRegistered;
				if (isLegacyRegistered)
				{
					UIMaskManager instance = SingletonObject.getInstance<UIMaskManager>();
					if (instance != null)
					{
						instance.LegacyUnregister(this);
					}
					this._isLegacyRegistered = false;
				}
			}
		}

		// Token: 0x0600BA60 RID: 47712 RVA: 0x0054E714 File Offset: 0x0054C914
		private void OnDestroy()
		{
			this.CleanupRenderTextures();
		}

		// Token: 0x0600BA61 RID: 47713 RVA: 0x0054E720 File Offset: 0x0054C920
		private void LateUpdate()
		{
			Vector2 currentScreenSize = new Vector2((float)Screen.width, (float)Screen.height);
			bool flag = currentScreenSize != this._lastScreenSize;
			if (flag)
			{
				this.UpdateMaskSizeAndTexture();
			}
		}

		// Token: 0x0600BA62 RID: 47714 RVA: 0x0054E75C File Offset: 0x0054C95C
		public void ForceInitialize()
		{
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				bool isSharedManaged = this._isSharedManaged;
				if (!isSharedManaged)
				{
					this.LegacyRegisterToManager();
				}
			}
		}

		// Token: 0x0600BA63 RID: 47715 RVA: 0x0054E78B File Offset: 0x0054C98B
		public void MarkAsSharedManaged()
		{
			this._isSharedManaged = true;
		}

		// Token: 0x170014FE RID: 5374
		// (get) Token: 0x0600BA64 RID: 47716 RVA: 0x0054E795 File Offset: 0x0054C995
		public bool IsSharedManaged
		{
			get
			{
				return this._isSharedManaged;
			}
		}

		// Token: 0x0600BA65 RID: 47717 RVA: 0x0054E7A0 File Offset: 0x0054C9A0
		private void LegacyRegisterToManager()
		{
			bool isLegacyRegistered = this._isLegacyRegistered;
			if (!isLegacyRegistered)
			{
				this._isLegacyRegistered = true;
				SingletonObject.getInstance<UIMaskManager>().LegacyRegister(this);
			}
		}

		// Token: 0x0600BA66 RID: 47718 RVA: 0x0054E7CD File Offset: 0x0054C9CD
		public void AssignGrab(Material mat, string grabName)
		{
			this._grabName = grabName;
			this._isGrabReady = false;
			this.grabRawImage.material = mat;
			this.maskRawImage.enabled = false;
		}

		// Token: 0x0600BA67 RID: 47719 RVA: 0x0054E7F8 File Offset: 0x0054C9F8
		public void SetGrabVisible(bool visible)
		{
			bool flag = !visible && this._enableDelayCo != null;
			if (flag)
			{
				base.StopCoroutine(this._enableDelayCo);
				this._enableDelayCo = null;
			}
			this.grabRawImage.enabled = visible;
		}

		// Token: 0x0600BA68 RID: 47720 RVA: 0x0054E83C File Offset: 0x0054CA3C
		public void SetOutputVisible(bool visible)
		{
			bool flag = !visible && this._enableDelayCo != null;
			if (flag)
			{
				base.StopCoroutine(this._enableDelayCo);
				this._enableDelayCo = null;
			}
			this.maskRawImage.enabled = visible;
		}

		// Token: 0x0600BA69 RID: 47721 RVA: 0x0054E880 File Offset: 0x0054CA80
		public void SetOnePassBlurVisible(bool visible)
		{
			bool flag = this.onePassBlurRawImage != null;
			if (flag)
			{
				this.onePassBlurRawImage.enabled = visible;
			}
		}

		// Token: 0x170014FF RID: 5375
		// (get) Token: 0x0600BA6A RID: 47722 RVA: 0x0054E8AB File Offset: 0x0054CAAB
		public bool IsOnePassBlurActive
		{
			get
			{
				return this.onePassBlurRawImage != null && this.onePassBlurRawImage.enabled;
			}
		}

		// Token: 0x0600BA6B RID: 47723 RVA: 0x0054E8CC File Offset: 0x0054CACC
		public void InvalidateDisplay()
		{
			bool flag = this._enableDelayCo != null;
			if (flag)
			{
				base.StopCoroutine(this._enableDelayCo);
				this._enableDelayCo = null;
			}
			this.maskRawImage.enabled = false;
			this.SetOnePassBlurVisible(false);
		}

		// Token: 0x17001500 RID: 5376
		// (get) Token: 0x0600BA6C RID: 47724 RVA: 0x0054E912 File Offset: 0x0054CB12
		public RenderTexture CurrentBlurTexture
		{
			get
			{
				return this._finalRT;
			}
		}

		// Token: 0x17001501 RID: 5377
		// (get) Token: 0x0600BA6D RID: 47725 RVA: 0x0054E91A File Offset: 0x0054CB1A
		public bool IsDisplayingBlur
		{
			get
			{
				return this.maskRawImage.enabled && this._finalRT != null;
			}
		}

		// Token: 0x17001502 RID: 5378
		// (get) Token: 0x0600BA6E RID: 47726 RVA: 0x0054E938 File Offset: 0x0054CB38
		public bool IsGrabActive
		{
			get
			{
				return this.grabRawImage.enabled;
			}
		}

		// Token: 0x17001503 RID: 5379
		// (get) Token: 0x0600BA6F RID: 47727 RVA: 0x0054E945 File Offset: 0x0054CB45
		public RectTransform MaskRectTransform
		{
			get
			{
				return this.maskRawImage.rectTransform;
			}
		}

		// Token: 0x0600BA70 RID: 47728 RVA: 0x0054E954 File Offset: 0x0054CB54
		public void DoProcess()
		{
			bool flag = !this.enableProcessing || !this._isInitialized;
			if (!flag)
			{
				bool processSuccess = this.ProcessTexture();
				bool flag2 = processSuccess;
				if (flag2)
				{
					this.ApplyProcessedTexture();
				}
			}
		}

		// Token: 0x0600BA71 RID: 47729 RVA: 0x0054E990 File Offset: 0x0054CB90
		public void SetValueFactor(float value)
		{
			this.valueFactor = value;
		}

		// Token: 0x0600BA72 RID: 47730 RVA: 0x0054E99C File Offset: 0x0054CB9C
		private void UpdateMaskSizeAndTexture()
		{
			float screenWidth = (float)Screen.width;
			float screenHeight = (float)Screen.height;
			this._lastScreenSize = new Vector2(screenWidth, screenHeight);
			float screenAspect = screenWidth / screenHeight;
			float referenceAspect = this.referenceResolution.x / this.referenceResolution.y;
			bool flag = screenAspect > referenceAspect;
			Vector2 newSize;
			if (flag)
			{
				newSize = new Vector2(this.referenceResolution.y * screenAspect, this.referenceResolution.y);
			}
			else
			{
				newSize = new Vector2(this.referenceResolution.x, this.referenceResolution.x / screenAspect);
			}
			this.maskRawImage.rectTransform.sizeDelta = newSize;
			bool flag2 = this._finalRT != null && ((float)this._finalRT.width != screenWidth || (float)this._finalRT.height != screenHeight);
			if (flag2)
			{
				this.CleanupRenderTextures();
			}
			bool flag3 = this._finalRT == null;
			if (flag3)
			{
				this._finalRT = new RenderTexture((int)screenWidth, (int)screenHeight, 0, RenderTextureFormat.ARGB32)
				{
					name = "UIMask_Final_Instance",
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp
				};
				this.maskRawImage.texture = this._finalRT;
				this.maskRawImage.enabled = false;
			}
		}

		// Token: 0x0600BA73 RID: 47731 RVA: 0x0054EAE8 File Offset: 0x0054CCE8
		private void CleanupRenderTextures()
		{
			bool flag = this._finalRT != null;
			if (flag)
			{
				this._finalRT.Release();
				Object.DestroyImmediate(this._finalRT);
				this._finalRT = null;
			}
		}

		// Token: 0x0600BA74 RID: 47732 RVA: 0x0054EB28 File Offset: 0x0054CD28
		private bool ProcessTexture()
		{
			bool flag = !this._isInitialized;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.UpdateMaterialProperties();
				Texture grabTexture = Shader.GetGlobalTexture(this._grabName);
				bool flag2 = !grabTexture;
				if (flag2)
				{
					bool flag3 = !this._isGrabReady;
					if (flag3)
					{
						result = false;
					}
					else
					{
						Debug.LogWarning("UIMask: Lost grab texture '" + this._grabName + "'. This may indicate a material assignment issue.");
						this._isGrabReady = false;
						result = false;
					}
				}
				else
				{
					bool flag4 = !this._isGrabReady;
					if (flag4)
					{
						this._isGrabReady = true;
					}
					int blurWidth = Mathf.RoundToInt((float)grabTexture.width * this.blurResolutionScale);
					int blurHeight = Mathf.RoundToInt((float)grabTexture.height * this.blurResolutionScale);
					RenderTexture colorAdjustRt = RenderTexture.GetTemporary(grabTexture.width, grabTexture.height, 0, RenderTextureFormat.ARGB32);
					RenderTexture rt = RenderTexture.GetTemporary(blurWidth, blurHeight, 0, RenderTextureFormat.ARGB32);
					RenderTexture rt2 = RenderTexture.GetTemporary(blurWidth, blurHeight, 0, RenderTextureFormat.ARGB32);
					colorAdjustRt.filterMode = FilterMode.Bilinear;
					rt.filterMode = FilterMode.Bilinear;
					rt2.filterMode = FilterMode.Bilinear;
					Graphics.Blit(grabTexture, colorAdjustRt, this.processMaterial, 2);
					Graphics.Blit(colorAdjustRt, rt);
					for (int i = 0; i < this.blurIterations; i++)
					{
						Graphics.Blit(rt, rt2, this.processMaterial, 1);
						Graphics.Blit(rt2, rt, this.processMaterial, 0);
					}
					Graphics.Blit(rt, this._finalRT);
					RenderTexture.ReleaseTemporary(colorAdjustRt);
					RenderTexture.ReleaseTemporary(rt);
					RenderTexture.ReleaseTemporary(rt2);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600BA75 RID: 47733 RVA: 0x0054ECB8 File Offset: 0x0054CEB8
		private void UpdateMaterialProperties()
		{
			bool flag = this.processMaterial == null;
			if (!flag)
			{
				this.processMaterial.SetFloat(UIMask.HueShift, this.hueShift);
				this.processMaterial.SetFloat(UIMask.SaturationFactor, this.saturationFactor);
				this.processMaterial.SetFloat(UIMask.ValueFactor, this.valueFactor);
				this.processMaterial.SetFloat(UIMask.Contrast, this.contrast);
				this.processMaterial.SetFloat(UIMask.BlurStrength, this.blurStrength);
				this.processMaterial.SetFloat(UIMask.ScaleFactor, this.scaleFactor);
				float screenWidth = (float)Screen.width;
				float screenHeight = (float)Screen.height;
				float screenAspect = screenWidth / screenHeight;
				float referenceAspect = this.referenceResolution.x / this.referenceResolution.y;
				Vector4 cropUvRect = new Vector4(0f, 0f, 1f, 1f);
				bool flag2 = screenAspect > referenceAspect;
				if (flag2)
				{
					float actualContentWidth = screenHeight * referenceAspect;
					float margin = (screenWidth - actualContentWidth) / 2f;
					float marginU = margin / screenWidth;
					cropUvRect = new Vector4(marginU, 0f, 1f - 2f * marginU, 1f);
				}
				else
				{
					bool flag3 = screenAspect < referenceAspect;
					if (flag3)
					{
						float actualContentHeight = screenWidth / referenceAspect;
						float margin2 = (screenHeight - actualContentHeight) / 2f;
						float marginV = margin2 / screenHeight;
						cropUvRect = new Vector4(0f, marginV, 1f, 1f - 2f * marginV);
					}
				}
				this.processMaterial.SetVector(UIMask.CropRect, cropUvRect);
			}
		}

		// Token: 0x0600BA76 RID: 47734 RVA: 0x0054EE4C File Offset: 0x0054D04C
		private void ApplyProcessedTexture()
		{
			bool flag = !this.maskRawImage.enabled && this._enableDelayCo == null && base.isActiveAndEnabled;
			if (flag)
			{
				this._enableDelayCo = base.StartCoroutine(this.DelayEnableResult());
			}
		}

		// Token: 0x0600BA77 RID: 47735 RVA: 0x0054EE90 File Offset: 0x0054D090
		private IEnumerator DelayEnableResult()
		{
			yield return null;
			yield return null;
			this.SetOnePassBlurVisible(false);
			this.maskRawImage.texture = this._finalRT;
			this.maskRawImage.enabled = true;
			this._enableDelayCo = null;
			yield break;
		}

		// Token: 0x0400900A RID: 36874
		private static readonly int HueShift = Shader.PropertyToID("_HueShift");

		// Token: 0x0400900B RID: 36875
		private static readonly int SaturationFactor = Shader.PropertyToID("_SaturationFactor");

		// Token: 0x0400900C RID: 36876
		private static readonly int ValueFactor = Shader.PropertyToID("_ValueFactor");

		// Token: 0x0400900D RID: 36877
		private static readonly int Contrast = Shader.PropertyToID("_Contrast");

		// Token: 0x0400900E RID: 36878
		private static readonly int BlurStrength = Shader.PropertyToID("_BlurStrength");

		// Token: 0x0400900F RID: 36879
		private static readonly int ScaleFactor = Shader.PropertyToID("_ScaleFactor");

		// Token: 0x04009010 RID: 36880
		private static readonly int CropRect = Shader.PropertyToID("_CropRect");

		// Token: 0x04009011 RID: 36881
		private const RenderTextureFormat RtFormat = RenderTextureFormat.ARGB32;

		// Token: 0x04009012 RID: 36882
		[SerializeField]
		private CRawImage grabRawImage;

		// Token: 0x04009013 RID: 36883
		[SerializeField]
		private CRawImage maskRawImage;

		// Token: 0x04009014 RID: 36884
		[SerializeField]
		private CRawImage onePassBlurRawImage;

		// Token: 0x04009015 RID: 36885
		[Header("处理设置")]
		[SerializeField]
		private Material processMaterial;

		// Token: 0x04009016 RID: 36886
		[SerializeField]
		private bool enableProcessing = true;

		// Token: 0x04009017 RID: 36887
		[SerializeField]
		private Vector2 referenceResolution = new Vector2(2560f, 1440f);

		// Token: 0x04009018 RID: 36888
		[Header("性能优化")]
		[Range(0.25f, 1f)]
		[SerializeField]
		private float blurResolutionScale = 0.5f;

		// Token: 0x04009019 RID: 36889
		[Header("HSV效果参数")]
		[Range(-1f, 1f)]
		[SerializeField]
		private float hueShift;

		// Token: 0x0400901A RID: 36890
		[Range(0f, 2f)]
		[SerializeField]
		private float saturationFactor = 1f;

		// Token: 0x0400901B RID: 36891
		[Range(0f, 1f)]
		[SerializeField]
		private float valueFactor = 1f;

		// Token: 0x0400901C RID: 36892
		[Header("其他效果参数")]
		[Range(0f, 3f)]
		[SerializeField]
		private float contrast = 1f;

		// Token: 0x0400901D RID: 36893
		[Header("模糊参数")]
		[Range(0f, 5f)]
		[SerializeField]
		private float blurStrength = 2f;

		// Token: 0x0400901E RID: 36894
		[Range(1f, 8f)]
		[SerializeField]
		private int blurIterations = 3;

		// Token: 0x0400901F RID: 36895
		[Header("缩放参数")]
		[Range(0.1f, 2f)]
		[SerializeField]
		private float scaleFactor = 1f;

		// Token: 0x04009020 RID: 36896
		private string _grabName;

		// Token: 0x04009021 RID: 36897
		private bool _isGrabReady;

		// Token: 0x04009022 RID: 36898
		private bool _isLegacyRegistered;

		// Token: 0x04009023 RID: 36899
		private bool _isSharedManaged;

		// Token: 0x04009024 RID: 36900
		private RenderTexture _finalRT;

		// Token: 0x04009025 RID: 36901
		private bool _isInitialized;

		// Token: 0x04009026 RID: 36902
		private Vector2 _lastScreenSize;

		// Token: 0x04009027 RID: 36903
		private Coroutine _enableDelayCo;
	}
}
