using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D7 RID: 215
[ExecuteInEditMode]
[RequireComponent(typeof(CRawImage))]
public class UIGrabGraphic : MonoBehaviour
{
	// Token: 0x060007AA RID: 1962 RVA: 0x00035DBC File Offset: 0x00033FBC
	private void Awake()
	{
		this._grabPropertyId = Shader.PropertyToID(this.shaderPropertyName);
		bool flag = this.flipVertical;
		if (flag)
		{
			Shader shader = Shader.Find("UI/FlipVertical");
			bool flag2 = shader != null;
			if (flag2)
			{
				this._flipMaterial = new Material(shader);
			}
			bool flag3 = this._flipMaterial == null;
			if (flag3)
			{
				Debug.LogError("UIGrabGraphic: Could not find 'UI/FlipVertical' shader.");
			}
		}
		this._lastScreenSize = new Vector2((float)Mathf.Min(Screen.width, 4096), (float)Mathf.Min(Screen.height, 4096));
		bool flag4 = this.enableDynamicResolution;
		if (flag4)
		{
			this.UpdateRenderTextureSize();
		}
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x00035E68 File Offset: 0x00034068
	private void LateUpdate()
	{
		bool flag = this.enableDynamicResolution;
		if (flag)
		{
			Vector2 currentScreenSize = new Vector2((float)Mathf.Min(Screen.width, 4096), (float)Mathf.Min(Screen.height, 4096));
			bool flag2 = currentScreenSize != this._lastScreenSize;
			if (flag2)
			{
				this.UpdateRenderTextureSize();
			}
		}
		Texture grabTexture = Shader.GetGlobalTexture(this._grabPropertyId);
		bool flag3 = grabTexture == null;
		if (!flag3)
		{
			RenderTexture targetRT = this.GetOrCreateTargetRenderTexture();
			bool flag4 = targetRT == null;
			if (!flag4)
			{
				bool flag5 = this.flipVertical;
				if (flag5)
				{
					Graphics.Blit(grabTexture, targetRT, this._flipMaterial, 0);
				}
				else
				{
					Graphics.Blit(grabTexture, targetRT);
				}
			}
		}
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00035F20 File Offset: 0x00034120
	private void OnDestroy()
	{
		this.CleanupRenderTexture();
		bool flag = this._flipMaterial != null;
		if (flag)
		{
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				Object.Destroy(this._flipMaterial);
			}
			else
			{
				Object.DestroyImmediate(this._flipMaterial);
			}
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00035F6C File Offset: 0x0003416C
	private void UpdateRenderTextureSize()
	{
		int screenWidth = Mathf.Min(Screen.width, 4096);
		int screenHeight = Mathf.Min(Screen.height, 4096);
		this._lastScreenSize = new Vector2((float)screenWidth, (float)screenHeight);
		bool flag = this._renderTexture != null && (this._renderTexture.width != screenWidth || this._renderTexture.height != screenHeight);
		if (flag)
		{
			this.CleanupRenderTexture();
		}
		bool flag2 = this._renderTexture == null;
		if (flag2)
		{
			this._renderTexture = new RenderTexture(screenWidth, screenHeight, 0, RenderTextureFormat.ARGB32)
			{
				name = "UIGrabGraphic_RT",
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
			bool flag3 = this.targetRawImage != null;
			if (flag3)
			{
				this.targetRawImage.texture = this._renderTexture;
				this.ShareRenderTexture();
			}
		}
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00036054 File Offset: 0x00034254
	private RenderTexture GetOrCreateTargetRenderTexture()
	{
		bool flag = this.enableDynamicResolution;
		RenderTexture result;
		if (flag)
		{
			result = this._renderTexture;
		}
		else
		{
			RawImage rawImage = this.targetRawImage;
			result = (((rawImage != null) ? rawImage.texture : null) as RenderTexture);
		}
		return result;
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x00036094 File Offset: 0x00034294
	private void CleanupRenderTexture()
	{
		bool flag = this._renderTexture != null;
		if (flag)
		{
			this._renderTexture.Release();
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				Object.Destroy(this._renderTexture);
			}
			else
			{
				Object.DestroyImmediate(this._renderTexture);
			}
			this._renderTexture = null;
		}
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x000360EC File Offset: 0x000342EC
	private void ShareRenderTexture()
	{
		foreach (UIGrabGraphic.IRtReceiver receiver in this._rtReceivers)
		{
			if (receiver != null)
			{
				receiver.SetRenderTexture(this._renderTexture);
			}
		}
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x00036150 File Offset: 0x00034350
	public void RegisterRtReceiver(UIGrabGraphic.IRtReceiver receiver)
	{
		this._rtReceivers.Add(receiver);
		if (receiver != null)
		{
			receiver.SetRenderTexture(this._renderTexture);
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00036174 File Offset: 0x00034374
	public bool UnregisterRtReceiver(UIGrabGraphic.IRtReceiver receiver)
	{
		return this._rtReceivers.Remove(receiver);
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00036194 File Offset: 0x00034394
	public void ForceUpdateRenderTextureSize()
	{
		bool flag = this.enableDynamicResolution;
		if (flag)
		{
			this.UpdateRenderTextureSize();
		}
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x000361B8 File Offset: 0x000343B8
	public void SetDynamicResolutionEnabled(bool enabled)
	{
		bool flag = this.enableDynamicResolution == enabled;
		if (!flag)
		{
			this.enableDynamicResolution = enabled;
			if (enabled)
			{
				this.UpdateRenderTextureSize();
			}
			else
			{
				this.CleanupRenderTexture();
			}
		}
	}

	// Token: 0x040007C5 RID: 1989
	[SerializeField]
	private RawImage targetRawImage;

	// Token: 0x040007C6 RID: 1990
	[SerializeField]
	private string shaderPropertyName;

	// Token: 0x040007C7 RID: 1991
	[SerializeField]
	private bool flipVertical = true;

	// Token: 0x040007C8 RID: 1992
	[Header("Dynamic Resolution Settings")]
	[SerializeField]
	private bool enableDynamicResolution = true;

	// Token: 0x040007C9 RID: 1993
	private const RenderTextureFormat RtFormat = RenderTextureFormat.ARGB32;

	// Token: 0x040007CA RID: 1994
	private const int MaxTextureSize = 4096;

	// Token: 0x040007CB RID: 1995
	private int _grabPropertyId = -1;

	// Token: 0x040007CC RID: 1996
	private Material _flipMaterial;

	// Token: 0x040007CD RID: 1997
	private RenderTexture _renderTexture;

	// Token: 0x040007CE RID: 1998
	private Vector2 _lastScreenSize;

	// Token: 0x040007CF RID: 1999
	private readonly HashSet<UIGrabGraphic.IRtReceiver> _rtReceivers = new HashSet<UIGrabGraphic.IRtReceiver>();

	// Token: 0x02001135 RID: 4405
	public interface IRtReceiver
	{
		// Token: 0x0600C1A5 RID: 49573
		void SetRenderTexture(RenderTexture rt);
	}
}
