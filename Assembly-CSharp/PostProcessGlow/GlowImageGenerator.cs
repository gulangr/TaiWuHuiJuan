using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace PostProcessGlow
{
	// Token: 0x020005FF RID: 1535
	public class GlowImageGenerator : MonoBehaviour
	{
		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06004862 RID: 18530 RVA: 0x0021E860 File Offset: 0x0021CA60
		public static GlowImageGenerator Instance
		{
			get
			{
				bool flag = GlowImageGenerator._instance == null;
				if (flag)
				{
					GameObject go = new GameObject("GlowImageGenerator");
					GlowImageGenerator._instance = go.AddComponent<GlowImageGenerator>();
					Object.DontDestroyOnLoad(go);
				}
				return GlowImageGenerator._instance;
			}
		}

		// Token: 0x06004863 RID: 18531 RVA: 0x0021E8A5 File Offset: 0x0021CAA5
		public void SetDefaultParameters(GlowImageGenerator.GlowParameters parameters)
		{
			this._defaultParameters = parameters;
		}

		// Token: 0x06004864 RID: 18532 RVA: 0x0021E8B0 File Offset: 0x0021CAB0
		[Preserve]
		private void Awake()
		{
			bool flag = GlowImageGenerator._instance == null;
			if (flag)
			{
				GlowImageGenerator._instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
				this.InitializeComponents();
			}
			else
			{
				bool flag2 = GlowImageGenerator._instance != this;
				if (flag2)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06004865 RID: 18533 RVA: 0x0021E908 File Offset: 0x0021CB08
		private void InitializeComponents()
		{
			bool flag = this.renderCamera == null;
			if (flag)
			{
				GameObject cameraGo = new GameObject("GlowRenderCamera");
				cameraGo.transform.SetParent(base.transform);
				this.renderCamera = cameraGo.AddComponent<Camera>();
				this.renderCamera.clearFlags = CameraClearFlags.Color;
				this.renderCamera.backgroundColor = Color.clear;
				this.renderCamera.orthographic = true;
				this.renderCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
				this.renderCamera.depth = -100f;
				this.renderCamera.enabled = false;
			}
			bool flag2 = this.renderCanvas == null;
			if (flag2)
			{
				GameObject canvasGo = new GameObject("GlowRenderCanvas");
				canvasGo.transform.SetParent(base.transform);
				this.renderCanvas = canvasGo.AddComponent<Canvas>();
				this.renderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				this.renderCanvas.worldCamera = this.renderCamera;
				this.renderCanvas.sortingOrder = 0;
				CanvasScaler canvasScaler = canvasGo.AddComponent<CanvasScaler>();
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				canvasScaler.matchWidthOrHeight = 0.5f;
				canvasGo.AddComponent<GraphicRaycaster>();
				canvasGo.layer = LayerMask.NameToLayer("UI");
			}
		}

		// Token: 0x06004866 RID: 18534 RVA: 0x0021EA58 File Offset: 0x0021CC58
		public void GenerateTextGlow(TextMeshProUGUI sourceText, Color glowColor, GlowImageGenerator.GlowParameters parameters = null, Action<CRawImage> onComplete = null, Action<string> onError = null, Vector2Int? renderSize = null)
		{
			RectTransform sourceRect = sourceText.rectTransform;
			Rect rect = sourceRect.rect;
			Vector2Int realRenderSize = renderSize ?? new Vector2Int((int)(rect.width * 1.5f), (int)(rect.height * 1.5f));
			GlowImageGenerator.GlowRequest request = new GlowImageGenerator.GlowRequest
			{
				Type = GlowImageGenerator.GlowRequest.RequestType.Text,
				SourceText = sourceText,
				GlowColor = glowColor,
				RenderSize = realRenderSize,
				OnComplete = onComplete,
				OnError = onError,
				AnchorMin = sourceRect.anchorMin,
				AnchorMax = sourceRect.anchorMax,
				Pivot = sourceRect.pivot,
				AnchoredPosition = sourceRect.anchoredPosition,
				SizeDelta = sourceRect.sizeDelta,
				LocalPosition = sourceRect.localPosition,
				LocalRotation = sourceRect.localRotation,
				LocalScale = sourceRect.localScale,
				Parameters = (parameters ?? this._defaultParameters)
			};
			this._requestQueue.Enqueue(request);
			bool flag = !this._isProcessing;
			if (flag)
			{
				base.StartCoroutine(this.ProcessRequestQueue());
			}
		}

		// Token: 0x06004867 RID: 18535 RVA: 0x0021EB7C File Offset: 0x0021CD7C
		public void GenerateImageGlow(Image sourceImage, Color glowColor, GlowImageGenerator.GlowParameters parameters = null, Action<CRawImage> onComplete = null, Action<string> onError = null)
		{
			RectTransform sourceRect = sourceImage.rectTransform;
			Rect rect = sourceRect.rect;
			Vector2Int renderSize = new Vector2Int((int)(rect.width * 1.5f), (int)(rect.height * 1.5f));
			GlowImageGenerator.GlowRequest request = new GlowImageGenerator.GlowRequest
			{
				Type = GlowImageGenerator.GlowRequest.RequestType.Image,
				SourceImage = sourceImage,
				GlowColor = glowColor,
				RenderSize = renderSize,
				OnComplete = onComplete,
				OnError = onError,
				AnchorMin = sourceRect.anchorMin,
				AnchorMax = sourceRect.anchorMax,
				Pivot = sourceRect.pivot,
				AnchoredPosition = sourceRect.anchoredPosition,
				SizeDelta = sourceRect.sizeDelta,
				LocalPosition = sourceRect.localPosition,
				LocalRotation = sourceRect.localRotation,
				LocalScale = sourceRect.localScale,
				Parameters = (parameters ?? this._defaultParameters)
			};
			this._requestQueue.Enqueue(request);
			bool flag = !this._isProcessing;
			if (flag)
			{
				base.StartCoroutine(this.ProcessRequestQueue());
			}
		}

		// Token: 0x06004868 RID: 18536 RVA: 0x0021EC8C File Offset: 0x0021CE8C
		public void GenerateGameObjectGlow(GameObject sourceGameObject, Color glowColor, GlowImageGenerator.GlowParameters parameters = null, Action<CRawImage> onComplete = null, Action<string> onError = null)
		{
			bool flag = sourceGameObject == null;
			if (flag)
			{
				if (onError != null)
				{
					onError("SourceGameObject is null");
				}
			}
			else
			{
				RectTransform rectTransform = sourceGameObject.GetComponent<RectTransform>();
				bool flag2 = rectTransform == null;
				if (flag2)
				{
					if (onError != null)
					{
						onError("SourceGameObject does not have a RectTransform component");
					}
				}
				else
				{
					Rect rect = rectTransform.rect;
					Vector2Int renderSize = new Vector2Int((int)(rect.width * 1.5f), (int)(rect.height * 1.5f));
					GlowImageGenerator.GlowRequest request = new GlowImageGenerator.GlowRequest
					{
						Type = GlowImageGenerator.GlowRequest.RequestType.GameObject,
						SourceGameObject = sourceGameObject,
						GlowColor = glowColor,
						RenderSize = renderSize,
						OnComplete = onComplete,
						OnError = onError,
						AnchorMin = rectTransform.anchorMin,
						AnchorMax = rectTransform.anchorMax,
						Pivot = rectTransform.pivot,
						AnchoredPosition = rectTransform.anchoredPosition,
						SizeDelta = rectTransform.sizeDelta,
						LocalPosition = rectTransform.localPosition,
						LocalRotation = rectTransform.localRotation,
						LocalScale = rectTransform.localScale,
						Parameters = (parameters ?? this._defaultParameters)
					};
					this._requestQueue.Enqueue(request);
					bool flag3 = !this._isProcessing;
					if (flag3)
					{
						base.StartCoroutine(this.ProcessRequestQueue());
					}
				}
			}
		}

		// Token: 0x06004869 RID: 18537 RVA: 0x0021EDE5 File Offset: 0x0021CFE5
		private IEnumerator ProcessRequestQueue()
		{
			this._isProcessing = true;
			while (this._requestQueue.Count > 0)
			{
				GlowImageGenerator.GlowRequest request = this._requestQueue.Dequeue();
				yield return base.StartCoroutine(this.ProcessSingleRequest(request));
				request = null;
			}
			this._isProcessing = false;
			yield break;
		}

		// Token: 0x0600486A RID: 18538 RVA: 0x0021EDF4 File Offset: 0x0021CFF4
		private IEnumerator ProcessSingleRequest(GlowImageGenerator.GlowRequest request)
		{
			bool flag = request.Type == GlowImageGenerator.GlowRequest.RequestType.GameObject;
			if (flag)
			{
				yield return base.StartCoroutine(this.ProcessGameObjectRequest(request));
			}
			else
			{
				yield return base.StartCoroutine(this.ProcessSingleItemRequest(request));
			}
			yield break;
		}

		// Token: 0x0600486B RID: 18539 RVA: 0x0021EE0A File Offset: 0x0021D00A
		private IEnumerator ProcessSingleItemRequest(GlowImageGenerator.GlowRequest request)
		{
			RenderTexture result = null;
			string error = null;
			this.SetupRenderingSize(request.RenderSize);
			RectTransform sourceRect = (request.Type == GlowImageGenerator.GlowRequest.RequestType.Text) ? request.SourceText.rectTransform : request.SourceImage.rectTransform;
			Transform originalParent = sourceRect.parent;
			int originalSiblingIndex = sourceRect.GetSiblingIndex();
			bool flag = request.Type == GlowImageGenerator.GlowRequest.RequestType.Text;
			if (flag)
			{
				this._textComponent = request.SourceText;
				this._textObject = this._textComponent.gameObject;
				this._textObject.transform.SetParent(this.renderCanvas.transform, false);
				RectTransform rectTransform = this._textObject.GetComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = Vector2.zero;
				this._textObject.SetActive(true);
				rectTransform = null;
			}
			else
			{
				this._imageComponent = request.SourceImage;
				this._imageObject = this._imageComponent.gameObject;
				this._imageObject.transform.SetParent(this.renderCanvas.transform, false);
				RectTransform rectTransform2 = this._imageObject.GetComponent<RectTransform>();
				rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform2.pivot = new Vector2(0.5f, 0.5f);
				rectTransform2.anchoredPosition = Vector2.zero;
				this._imageObject.SetActive(true);
				rectTransform2 = null;
			}
			yield return null;
			yield return base.StartCoroutine(this.GenerateGlowEffectCoroutine(request, delegate(RenderTexture generatedResult, string generatedError)
			{
				result = generatedResult;
				error = generatedError;
			}));
			bool flag2 = request.Type == GlowImageGenerator.GlowRequest.RequestType.Text;
			if (flag2)
			{
				this._textObject.SetActive(false);
				this._textObject.transform.SetParent(originalParent, false);
				this._textObject.transform.SetSiblingIndex(originalSiblingIndex);
			}
			else
			{
				this._imageObject.SetActive(false);
				this._imageObject.transform.SetParent(originalParent, false);
				this._imageObject.transform.SetSiblingIndex(originalSiblingIndex);
			}
			bool flag3 = error != null;
			if (flag3)
			{
				Debug.LogError("GlowImageGenerator: Error processing request: " + error);
				Action<string> onError = request.OnError;
				if (onError != null)
				{
					onError(error);
				}
			}
			else
			{
				CRawImage resultImage = this.CreateResultImage(request, result, originalParent, originalSiblingIndex);
				Action<CRawImage> onComplete = request.OnComplete;
				if (onComplete != null)
				{
					onComplete(resultImage);
				}
				resultImage = null;
			}
			yield break;
		}

		// Token: 0x0600486C RID: 18540 RVA: 0x0021EE20 File Offset: 0x0021D020
		private IEnumerator ProcessGameObjectRequest(GlowImageGenerator.GlowRequest request)
		{
			RenderTexture result = null;
			string error = null;
			List<TextMeshProUGUI> childTexts = new List<TextMeshProUGUI>();
			List<Image> childImages = new List<Image>();
			List<GlowImageGenerator.OriginalChildData> originalChildData = new List<GlowImageGenerator.OriginalChildData>();
			GameObject sourceGameObject = request.SourceGameObject;
			sourceGameObject.SetActive(true);
			RectTransform sourceRect = sourceGameObject.GetComponent<RectTransform>();
			this.CollectChildComponents(sourceGameObject.transform, childTexts, childImages, originalChildData);
			this.SetupRenderingSize(request.RenderSize);
			Transform originalParent = sourceRect.parent;
			int originalSiblingIndex = sourceRect.GetSiblingIndex();
			Vector2 originalPosition = sourceRect.anchoredPosition;
			Vector2 originalAnchorMin = sourceRect.anchorMin;
			Vector2 originalAnchorMax = sourceRect.anchorMax;
			Vector2 originalPivot = sourceRect.pivot;
			Vector2 originalSizeDelta = sourceRect.sizeDelta;
			Vector3 originalLocalPosition = sourceRect.localPosition;
			Quaternion originalLocalRotation = sourceRect.localRotation;
			Vector3 originalLocalScale = sourceRect.localScale;
			bool originalActive = sourceGameObject.activeSelf;
			sourceGameObject.transform.SetParent(this.renderCanvas.transform, false);
			sourceRect.anchorMin = new Vector2(0.5f, 0.5f);
			sourceRect.anchorMax = new Vector2(0.5f, 0.5f);
			sourceRect.pivot = new Vector2(0.5f, 0.5f);
			sourceRect.anchoredPosition = Vector2.zero;
			sourceGameObject.SetActive(true);
			yield return null;
			yield return base.StartCoroutine(this.GenerateGlowEffectForGameObject(request, childTexts, childImages, originalChildData, delegate(RenderTexture generatedResult, string generatedError)
			{
				result = generatedResult;
				error = generatedError;
			}));
			sourceGameObject.SetActive(originalActive);
			sourceGameObject.transform.SetParent(originalParent, false);
			sourceGameObject.transform.SetSiblingIndex(originalSiblingIndex);
			sourceRect.anchorMin = originalAnchorMin;
			sourceRect.anchorMax = originalAnchorMax;
			sourceRect.pivot = originalPivot;
			sourceRect.anchoredPosition = originalPosition;
			sourceRect.sizeDelta = originalSizeDelta;
			sourceRect.localPosition = originalLocalPosition;
			sourceRect.localRotation = originalLocalRotation;
			sourceRect.localScale = originalLocalScale;
			int num;
			for (int i = 0; i < originalChildData.Count; i = num + 1)
			{
				GlowImageGenerator.OriginalChildData data = originalChildData[i];
				RectTransform childRectTransform = data.Transform.GetComponent<RectTransform>();
				childRectTransform.anchorMin = data.AnchorMin;
				childRectTransform.anchorMax = data.AnchorMax;
				childRectTransform.pivot = data.Pivot;
				childRectTransform.anchoredPosition = data.AnchoredPosition;
				childRectTransform.sizeDelta = data.SizeDelta;
				childRectTransform.localPosition = data.LocalPosition;
				childRectTransform.localRotation = data.LocalRotation;
				childRectTransform.localScale = data.LocalScale;
				data.Transform.gameObject.SetActive(data.IsActive);
				TextMeshProUGUI textComponent = data.Transform.GetComponent<TextMeshProUGUI>();
				bool flag = textComponent != null;
				if (flag)
				{
					textComponent.color = data.OriginalTextColor;
				}
				Image imageComponent = data.Transform.GetComponent<Image>();
				bool flag2 = imageComponent != null;
				if (flag2)
				{
					imageComponent.color = data.OriginalImageColor;
				}
				data = default(GlowImageGenerator.OriginalChildData);
				childRectTransform = null;
				textComponent = null;
				imageComponent = null;
				num = i;
			}
			bool flag3 = error != null;
			if (flag3)
			{
				Debug.LogError("GlowImageGenerator: Error processing GameObject request: " + error);
				Action<string> onError = request.OnError;
				if (onError != null)
				{
					onError(error);
				}
			}
			else
			{
				CRawImage resultImage = this.CreateResultImage(request, result, originalParent, originalSiblingIndex);
				Action<CRawImage> onComplete = request.OnComplete;
				if (onComplete != null)
				{
					onComplete(resultImage);
				}
				resultImage = null;
			}
			yield break;
		}

		// Token: 0x0600486D RID: 18541 RVA: 0x0021EE38 File Offset: 0x0021D038
		private void CollectChildComponents(Transform parent, List<TextMeshProUGUI> childTexts, List<Image> childImages, List<GlowImageGenerator.OriginalChildData> originalChildData)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				RectTransform childRectTransform = child.GetComponent<RectTransform>();
				bool flag = childRectTransform == null;
				if (!flag)
				{
					GlowImageGenerator.OriginalChildData originalData = new GlowImageGenerator.OriginalChildData
					{
						Transform = child,
						AnchorMin = childRectTransform.anchorMin,
						AnchorMax = childRectTransform.anchorMax,
						Pivot = childRectTransform.pivot,
						AnchoredPosition = childRectTransform.anchoredPosition,
						SizeDelta = childRectTransform.sizeDelta,
						LocalPosition = childRectTransform.localPosition,
						LocalRotation = childRectTransform.localRotation,
						LocalScale = childRectTransform.localScale,
						IsActive = child.gameObject.activeSelf
					};
					TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
					bool flag2 = textComponent != null;
					if (flag2)
					{
						childTexts.Add(textComponent);
						originalData.OriginalTextColor = textComponent.color;
					}
					Image imageComponent = child.GetComponent<Image>();
					bool flag3 = imageComponent != null;
					if (flag3)
					{
						childImages.Add(imageComponent);
						originalData.OriginalImageColor = imageComponent.color;
					}
					originalChildData.Add(originalData);
					this.CollectChildComponents(child, childTexts, childImages, originalChildData);
				}
			}
		}

		// Token: 0x0600486E RID: 18542 RVA: 0x0021EF86 File Offset: 0x0021D186
		private IEnumerator GenerateGlowEffectForGameObject(GlowImageGenerator.GlowRequest request, List<TextMeshProUGUI> childTexts, List<Image> childImages, List<GlowImageGenerator.OriginalChildData> originalChildData, Action<RenderTexture, string> onComplete)
		{
			RenderTexture source = this.renderCamera.targetTexture;
			bool flag = source == null;
			if (flag)
			{
				if (onComplete != null)
				{
					onComplete(null, "RenderCamera.targetTexture is null");
				}
				yield break;
			}
			bool needDelayBeforeFirstRender = false;
			foreach (TextMeshProUGUI childText in childTexts)
			{
				bool flag2 = childText.GetComponent<AdvancedCurvedText>() || childText.GetComponent<CurvedText>();
				if (flag2)
				{
					needDelayBeforeFirstRender = true;
					break;
				}
				childText = null;
			}
			List<TextMeshProUGUI>.Enumerator enumerator = default(List<TextMeshProUGUI>.Enumerator);
			bool flag3 = needDelayBeforeFirstRender;
			if (flag3)
			{
				yield return null;
			}
			this.renderCamera.Render();
			yield return null;
			int reqName = request.GetHashCode();
			RenderTexture originalBackup = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, originalBackup);
			int num;
			for (int i = 0; i < childTexts.Count; i = num + 1)
			{
				childTexts[i].color = request.GlowColor;
				num = i;
			}
			for (int j = 0; j < childImages.Count; j = num + 1)
			{
				childImages[j].color = request.GlowColor;
				num = j;
			}
			yield return null;
			this.renderCamera.Render();
			RenderTexture coloredSource = RenderTexture.GetTemporary(source.width, source.height);
			this.glowMaterial.SetColor(GlowImageGenerator.GlowColor, request.GlowColor);
			Graphics.Blit(source, coloredSource, this.glowMaterial, 8);
			bool flag4 = request.Parameters.downSampleMode == GlowImageGenerator.DownSampleMode.Half;
			RenderTexture rt;
			RenderTexture rt2;
			if (flag4)
			{
				rt = RenderTexture.GetTemporary(coloredSource.width / 2, coloredSource.height / 2);
				rt2 = RenderTexture.GetTemporary(coloredSource.width / 2, coloredSource.height / 2);
				Graphics.Blit(coloredSource, rt);
			}
			else
			{
				bool flag5 = request.Parameters.downSampleMode == GlowImageGenerator.DownSampleMode.Quarter;
				if (flag5)
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
			this.glowMaterial.SetFloat(GlowImageGenerator.DilateStrengthId, request.Parameters.dilateStrength);
			this.glowMaterial.SetFloat(GlowImageGenerator.GlowAlphaId, request.Parameters.glowAlpha);
			for (int k = 0; k < request.Parameters.dilateIterations; k = num + 1)
			{
				int dilatePass = this.GetDilatePass(request.Parameters.dilateMode);
				Graphics.Blit(rt, rt2, this.glowMaterial, dilatePass);
				Graphics.Blit(rt2, rt);
				num = k;
			}
			for (int l = 0; l < request.Parameters.iteration; l = num + 1)
			{
				Graphics.Blit(rt, rt2, this.glowMaterial, 1);
				Graphics.Blit(rt2, rt, this.glowMaterial, 2);
				num = l;
			}
			RenderTexture.ReleaseTemporary(rt2);
			RenderTexture finalOutput = new RenderTexture(source.width, source.height, 24);
			this.glowMaterial.SetTexture(GlowImageGenerator.OriginalTexId, originalBackup);
			Graphics.Blit(rt, finalOutput, this.glowMaterial, 7);
			for (int m = 0; m < originalChildData.Count; m = num + 1)
			{
				GlowImageGenerator.OriginalChildData data = originalChildData[m];
				TextMeshProUGUI textComponent = data.Transform.GetComponent<TextMeshProUGUI>();
				bool flag6 = textComponent != null;
				if (flag6)
				{
					textComponent.color = data.OriginalTextColor;
				}
				Image imageComponent = data.Transform.GetComponent<Image>();
				bool flag7 = imageComponent != null;
				if (flag7)
				{
					imageComponent.color = data.OriginalImageColor;
				}
				data = default(GlowImageGenerator.OriginalChildData);
				textComponent = null;
				imageComponent = null;
				num = m;
			}
			RenderTexture.ReleaseTemporary(rt);
			RenderTexture.ReleaseTemporary(originalBackup);
			if (onComplete != null)
			{
				onComplete(finalOutput, null);
			}
			yield break;
		}

		// Token: 0x0600486F RID: 18543 RVA: 0x0021EFBC File Offset: 0x0021D1BC
		private void SetupRenderingSize(Vector2Int size)
		{
			this.renderCamera.orthographicSize = (float)size.y * 0.5f;
			this.renderCamera.aspect = (float)size.x / (float)size.y;
			CanvasScaler canvasScaler = this.renderCanvas.GetComponent<CanvasScaler>();
			canvasScaler.referenceResolution = new Vector2((float)size.x, (float)size.y);
			bool flag = this.renderCamera.targetTexture != null;
			if (flag)
			{
				this.renderCamera.targetTexture.Release();
			}
			this.renderCamera.targetTexture = new RenderTexture(size.x, size.y, 24);
		}

		// Token: 0x06004870 RID: 18544 RVA: 0x0021F074 File Offset: 0x0021D274
		private CRawImage CreateResultImage(GlowImageGenerator.GlowRequest request, RenderTexture texture, Transform parent, int siblingIndex)
		{
			GameObject go = new GameObject("GlowResultImage");
			go.transform.SetParent(parent, false);
			go.transform.SetSiblingIndex(siblingIndex);
			CRawImage rawImage = go.AddComponent<CRawImage>();
			rawImage.texture = texture;
			rawImage.raycastTarget = false;
			RectTransform resultRect = rawImage.rectTransform;
			resultRect.anchorMin = request.AnchorMin;
			resultRect.anchorMax = request.AnchorMax;
			resultRect.pivot = request.Pivot;
			resultRect.anchoredPosition = request.AnchoredPosition;
			resultRect.sizeDelta = request.RenderSize;
			resultRect.localPosition = request.LocalPosition;
			resultRect.localRotation = request.LocalRotation;
			resultRect.localScale = request.LocalScale;
			return rawImage;
		}

		// Token: 0x06004871 RID: 18545 RVA: 0x0021F139 File Offset: 0x0021D339
		private IEnumerator GenerateGlowEffectCoroutine(GlowImageGenerator.GlowRequest request, Action<RenderTexture, string> onComplete)
		{
			RenderTexture source = this.renderCamera.targetTexture;
			bool flag = source == null;
			if (flag)
			{
				if (onComplete != null)
				{
					onComplete(null, "RenderCamera.targetTexture is null");
				}
				yield break;
			}
			bool needDelayBeforeFirstRender = this._textComponent.GetComponent<AdvancedCurvedText>() || this._textComponent.GetComponent<CurvedText>();
			bool flag2 = needDelayBeforeFirstRender;
			if (flag2)
			{
				yield return null;
			}
			this.renderCamera.Render();
			yield return null;
			RenderTexture originalBackup = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, originalBackup);
			bool flag3 = request.Type == GlowImageGenerator.GlowRequest.RequestType.Text;
			Color[] originalColors;
			if (flag3)
			{
				originalColors = new Color[]
				{
					this._textComponent.color
				};
				this._textComponent.color = request.GlowColor;
			}
			else
			{
				originalColors = new Color[]
				{
					this._imageComponent.color
				};
				this._imageComponent.color = request.GlowColor;
			}
			yield return null;
			this.renderCamera.Render();
			RenderTexture coloredSource = RenderTexture.GetTemporary(source.width, source.height);
			this.glowMaterial.SetColor(GlowImageGenerator.GlowColor, request.GlowColor);
			Graphics.Blit(source, coloredSource, this.glowMaterial, 8);
			bool flag4 = request.Parameters.downSampleMode == GlowImageGenerator.DownSampleMode.Half;
			RenderTexture rt;
			RenderTexture rt2;
			if (flag4)
			{
				rt = RenderTexture.GetTemporary(coloredSource.width / 2, coloredSource.height / 2);
				rt2 = RenderTexture.GetTemporary(coloredSource.width / 2, coloredSource.height / 2);
				Graphics.Blit(coloredSource, rt);
			}
			else
			{
				bool flag5 = request.Parameters.downSampleMode == GlowImageGenerator.DownSampleMode.Quarter;
				if (flag5)
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
			this.glowMaterial.SetFloat(GlowImageGenerator.DilateStrengthId, request.Parameters.dilateStrength);
			this.glowMaterial.SetFloat(GlowImageGenerator.GlowAlphaId, request.Parameters.glowAlpha);
			int num;
			for (int i = 0; i < request.Parameters.dilateIterations; i = num + 1)
			{
				int dilatePass = this.GetDilatePass(request.Parameters.dilateMode);
				Graphics.Blit(rt, rt2, this.glowMaterial, dilatePass);
				Graphics.Blit(rt2, rt);
				num = i;
			}
			for (int j = 0; j < request.Parameters.iteration; j = num + 1)
			{
				Graphics.Blit(rt, rt2, this.glowMaterial, 1);
				Graphics.Blit(rt2, rt, this.glowMaterial, 2);
				num = j;
			}
			RenderTexture.ReleaseTemporary(rt2);
			RenderTexture finalOutput = new RenderTexture(source.width, source.height, 24);
			this.glowMaterial.SetTexture(GlowImageGenerator.OriginalTexId, originalBackup);
			Graphics.Blit(rt, finalOutput, this.glowMaterial, 7);
			bool flag6 = request.Type == GlowImageGenerator.GlowRequest.RequestType.Text;
			if (flag6)
			{
				this._textComponent.color = originalColors[0];
			}
			else
			{
				this._imageComponent.color = originalColors[0];
			}
			RenderTexture.ReleaseTemporary(rt);
			RenderTexture.ReleaseTemporary(originalBackup);
			if (onComplete != null)
			{
				onComplete(finalOutput, null);
			}
			yield break;
		}

		// Token: 0x06004872 RID: 18546 RVA: 0x0021F158 File Offset: 0x0021D358
		private int GetDilatePass(GlowImageGenerator.DilateMode dilateMode)
		{
			switch (dilateMode)
			{
			case GlowImageGenerator.DilateMode.Simple:
				return 4;
			case GlowImageGenerator.DilateMode.ColorPreserving:
				return 5;
			}
			return 3;
		}

		// Token: 0x04003200 RID: 12800
		private static readonly int OriginalTexId = Shader.PropertyToID("_OriginalTex");

		// Token: 0x04003201 RID: 12801
		private static readonly int DilateStrengthId = Shader.PropertyToID("_DilateStrength");

		// Token: 0x04003202 RID: 12802
		private static readonly int GlowAlphaId = Shader.PropertyToID("_GlowAlpha");

		// Token: 0x04003203 RID: 12803
		private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

		// Token: 0x04003204 RID: 12804
		[Header("组件引用")]
		[SerializeField]
		private Camera renderCamera;

		// Token: 0x04003205 RID: 12805
		[SerializeField]
		private Canvas renderCanvas;

		// Token: 0x04003206 RID: 12806
		[SerializeField]
		private Material glowMaterial;

		// Token: 0x04003207 RID: 12807
		private GlowImageGenerator.GlowParameters _defaultParameters = new GlowImageGenerator.GlowParameters();

		// Token: 0x04003208 RID: 12808
		private Queue<GlowImageGenerator.GlowRequest> _requestQueue = new Queue<GlowImageGenerator.GlowRequest>();

		// Token: 0x04003209 RID: 12809
		private bool _isProcessing;

		// Token: 0x0400320A RID: 12810
		private GameObject _textObject;

		// Token: 0x0400320B RID: 12811
		private GameObject _imageObject;

		// Token: 0x0400320C RID: 12812
		private TextMeshProUGUI _textComponent;

		// Token: 0x0400320D RID: 12813
		private Image _imageComponent;

		// Token: 0x0400320E RID: 12814
		private static GlowImageGenerator _instance;

		// Token: 0x020019BE RID: 6590
		public enum DownSampleMode
		{
			// Token: 0x0400B33C RID: 45884
			Off,
			// Token: 0x0400B33D RID: 45885
			Half,
			// Token: 0x0400B33E RID: 45886
			Quarter
		}

		// Token: 0x020019BF RID: 6591
		public enum DilateMode
		{
			// Token: 0x0400B340 RID: 45888
			Simple,
			// Token: 0x0400B341 RID: 45889
			ColorPreserving,
			// Token: 0x0400B342 RID: 45890
			ChannelMax
		}

		// Token: 0x020019C0 RID: 6592
		[Serializable]
		public class GlowParameters
		{
			// Token: 0x0400B343 RID: 45891
			public GlowImageGenerator.DownSampleMode downSampleMode = GlowImageGenerator.DownSampleMode.Quarter;

			// Token: 0x0400B344 RID: 45892
			public GlowImageGenerator.DilateMode dilateMode = GlowImageGenerator.DilateMode.ChannelMax;

			// Token: 0x0400B345 RID: 45893
			[Range(0f, 8f)]
			public int iteration = 3;

			// Token: 0x0400B346 RID: 45894
			[Range(0f, 5f)]
			public int dilateIterations = 1;

			// Token: 0x0400B347 RID: 45895
			[Range(0.5f, 3f)]
			public float dilateStrength = 1f;

			// Token: 0x0400B348 RID: 45896
			[Range(0f, 1f)]
			public float glowAlpha = 1f;
		}

		// Token: 0x020019C1 RID: 6593
		public class GlowRequest
		{
			// Token: 0x0400B349 RID: 45897
			public GlowImageGenerator.GlowRequest.RequestType Type;

			// Token: 0x0400B34A RID: 45898
			public Vector2Int RenderSize;

			// Token: 0x0400B34B RID: 45899
			public Color GlowColor;

			// Token: 0x0400B34C RID: 45900
			public Action<CRawImage> OnComplete;

			// Token: 0x0400B34D RID: 45901
			public Action<string> OnError;

			// Token: 0x0400B34E RID: 45902
			public TextMeshProUGUI SourceText;

			// Token: 0x0400B34F RID: 45903
			public Image SourceImage;

			// Token: 0x0400B350 RID: 45904
			public GameObject SourceGameObject;

			// Token: 0x0400B351 RID: 45905
			public Vector2 AnchorMin;

			// Token: 0x0400B352 RID: 45906
			public Vector2 AnchorMax;

			// Token: 0x0400B353 RID: 45907
			public Vector2 Pivot;

			// Token: 0x0400B354 RID: 45908
			public Vector2 AnchoredPosition;

			// Token: 0x0400B355 RID: 45909
			public Vector2 SizeDelta;

			// Token: 0x0400B356 RID: 45910
			public Vector3 LocalPosition;

			// Token: 0x0400B357 RID: 45911
			public Quaternion LocalRotation;

			// Token: 0x0400B358 RID: 45912
			public Vector3 LocalScale;

			// Token: 0x0400B359 RID: 45913
			public GlowImageGenerator.GlowParameters Parameters;

			// Token: 0x020026C4 RID: 9924
			public enum RequestType
			{
				// Token: 0x0400EB75 RID: 60277
				Text,
				// Token: 0x0400EB76 RID: 60278
				Image,
				// Token: 0x0400EB77 RID: 60279
				GameObject
			}
		}

		// Token: 0x020019C2 RID: 6594
		private struct OriginalChildData
		{
			// Token: 0x0400B35A RID: 45914
			public Transform Transform;

			// Token: 0x0400B35B RID: 45915
			public Vector2 AnchorMin;

			// Token: 0x0400B35C RID: 45916
			public Vector2 AnchorMax;

			// Token: 0x0400B35D RID: 45917
			public Vector2 Pivot;

			// Token: 0x0400B35E RID: 45918
			public Vector2 AnchoredPosition;

			// Token: 0x0400B35F RID: 45919
			public Vector2 SizeDelta;

			// Token: 0x0400B360 RID: 45920
			public Vector3 LocalPosition;

			// Token: 0x0400B361 RID: 45921
			public Quaternion LocalRotation;

			// Token: 0x0400B362 RID: 45922
			public Vector3 LocalScale;

			// Token: 0x0400B363 RID: 45923
			public bool IsActive;

			// Token: 0x0400B364 RID: 45924
			public Color OriginalTextColor;

			// Token: 0x0400B365 RID: 45925
			public Color OriginalImageColor;
		}
	}
}
