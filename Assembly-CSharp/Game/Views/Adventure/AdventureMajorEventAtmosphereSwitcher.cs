using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C7D RID: 3197
	public class AdventureMajorEventAtmosphereSwitcher : MonoBehaviour
	{
		// Token: 0x0600A34E RID: 41806 RVA: 0x004C6A94 File Offset: 0x004C4C94
		private void InitRuntimeCameraAndCanvas()
		{
			Camera createdFrontCamera;
			Transform createdFrontContainer;
			this._runtimeFrontRoot = this.CreateRuntimeCameraAndCanvas("AtmosphereFrontRuntime", this.cameraDepthFront, 10000f, out createdFrontCamera, out createdFrontContainer);
			Camera createdBackCamera;
			Transform createdBackContainer;
			this._runtimeBackRoot = this.CreateRuntimeCameraAndCanvas("AtmosphereBackRuntime", this.cameraDepthBack, 20000f, out createdBackCamera, out createdBackContainer);
			this.frontCamera = createdFrontCamera;
			this.backCamera = createdBackCamera;
			this.frontEffectContainer = createdFrontContainer;
			this.backEffectContainer = createdBackContainer;
		}

		// Token: 0x0600A34F RID: 41807 RVA: 0x004C6B00 File Offset: 0x004C4D00
		private GameObject CreateRuntimeCameraAndCanvas(string rootName, float cameraDepth, float offsetX, out Camera cameraComponent, out Transform canvasContainer)
		{
			GameObject root = new GameObject(rootName);
			root.transform.position = new Vector3(offsetX, 0f, 0f);
			GameObject cameraGo = new GameObject(rootName + "_Camera");
			cameraGo.transform.SetParent(root.transform, false);
			cameraComponent = cameraGo.AddComponent<Camera>();
			cameraComponent.orthographic = true;
			cameraComponent.clearFlags = CameraClearFlags.Color;
			cameraComponent.backgroundColor = new Color(0f, 0f, 0f, 0f);
			cameraComponent.cullingMask = 1 << this.atmosphereLayer;
			cameraComponent.depth = cameraDepth;
			cameraComponent.nearClipPlane = 0.01f;
			cameraComponent.farClipPlane = 100f;
			GameObject canvasGo = new GameObject(rootName + "_Canvas");
			canvasGo.transform.SetParent(cameraGo.transform, false);
			Canvas canvas = canvasGo.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = cameraComponent;
			canvas.planeDistance = 1f;
			canvas.sortingOrder = 0;
			canvasGo.AddComponent<CanvasScaler>();
			canvasGo.AddComponent<GraphicRaycaster>();
			AdventureMajorEventAtmosphereSwitcher.SetLayerRecursively(canvasGo.transform, this.atmosphereLayer);
			canvasContainer = canvasGo.transform;
			return root;
		}

		// Token: 0x0600A350 RID: 41808 RVA: 0x004C6C50 File Offset: 0x004C4E50
		private static void SetLayerRecursively(Transform root, int layer)
		{
			bool flag = root == null;
			if (!flag)
			{
				root.gameObject.layer = layer;
				for (int i = 0; i < root.childCount; i++)
				{
					AdventureMajorEventAtmosphereSwitcher.SetLayerRecursively(root.GetChild(i), layer);
				}
			}
		}

		// Token: 0x1700110F RID: 4367
		// (get) Token: 0x0600A351 RID: 41809 RVA: 0x004C6C9D File Offset: 0x004C4E9D
		public int CurrentAtmosphereType
		{
			get
			{
				return this._currentAtmosphereType;
			}
		}

		// Token: 0x0600A352 RID: 41810 RVA: 0x004C6CA8 File Offset: 0x004C4EA8
		private void Awake()
		{
			this.InitNodeMap();
			this.InitRuntimeCameraAndCanvas();
			this.InitRenderTextures();
			this.InitAnimationCurves();
			this.ExtractColorsFromReferenceMaterials();
			bool flag = this.backCamera != null;
			if (flag)
			{
				this.backCamera.gameObject.SetActive(false);
			}
			bool flag2 = this.backImage != null;
			if (flag2)
			{
				this.backImage.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A353 RID: 41811 RVA: 0x004C6D20 File Offset: 0x004C4F20
		private void ExtractColorsFromReferenceMaterials()
		{
			bool flag = this.colorReferenceMaterials == null || this.colorReferenceMaterials.Length == 0;
			if (!flag)
			{
				bool flag2 = this.atmosphereHighlightColors == null || this.atmosphereHighlightColors.Length == 0;
				if (flag2)
				{
					this.atmosphereHighlightColors = new AdventureMajorEventAtmosphereSwitcher.HighlightColorPair[3];
				}
				int i = 0;
				while (i < this.colorReferenceMaterials.Length && i < 3)
				{
					Material material = this.colorReferenceMaterials[i];
					bool flag3 = material == null;
					if (!flag3)
					{
						bool flag4 = material.HasProperty(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightColor1);
						if (flag4)
						{
							this.atmosphereHighlightColors[i].color1 = material.GetColor(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightColor1);
						}
						bool flag5 = material.HasProperty(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightColor2);
						if (flag5)
						{
							this.atmosphereHighlightColors[i].color2 = material.GetColor(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightColor2);
						}
						Debug.Log(string.Format("[AdventureMajorEventAtmosphereSwitcher] 从材质提取颜色 - 氛围类型{0}: Color1={1}, Color2={2}", i, this.atmosphereHighlightColors[i].color1, this.atmosphereHighlightColors[i].color2));
					}
					i++;
				}
			}
		}

		// Token: 0x0600A354 RID: 41812 RVA: 0x004C6E58 File Offset: 0x004C5058
		private void InitAnimationCurves()
		{
			bool flag = !this.useAnimationCurves || this.switchAnimationClip == null;
			if (flag)
			{
				Debug.LogWarning("[AdventureMajorEventAtmosphereSwitcher] 动画曲线未启用或未设置，将使用 DOVirtual");
			}
			else
			{
				this._maskProgressCurve = this.GetCurveByName("material._MaskProgress");
				this._highlightWidth1Curve = this.GetCurveByName("material._HighlightWidth1");
				this._highlightWidth2Curve = this.GetCurveByName("material._HighlightWidth2");
				this._highlightOffset1Curve = this.GetCurveByName("material._HighlightOffset1");
				this._highlightOffset2Curve = this.GetCurveByName("material._HighlightOffset2");
				this._highlightIntensity1Curve = this.GetCurveByName("material._HighlightIntensity1");
				this._highlightIntensity2Curve = this.GetCurveByName("material._HighlightIntensity2");
				this._maskShrinkCurve = this.GetCurveByName("material._MaskShrink");
				this._edgeWidthCurve = this.GetCurveByName("material._EdgeWidth");
				bool flag2 = this._maskProgressCurve == null || this._maskProgressCurve.keys.Length == 0;
				if (flag2)
				{
					this._maskProgressCurve = AnimationCurve.Linear(0f, 0f, this.switchAnimationClip.length, 1f);
				}
			}
		}

		// Token: 0x0600A355 RID: 41813 RVA: 0x004C6F70 File Offset: 0x004C5170
		private AnimationCurve GetCurveByName(string name)
		{
			AdventureMajorEventAtmosphereSwitcher.AnimationCurveData curveData = this.animationCurves.Find((AdventureMajorEventAtmosphereSwitcher.AnimationCurveData c) => c.propertyName == name);
			return ((curveData != null) ? curveData.curve : null) ?? new AnimationCurve();
		}

		// Token: 0x0600A356 RID: 41814 RVA: 0x004C6FBC File Offset: 0x004C51BC
		private void InitNodeMap()
		{
			this._atmosphereNodeMap = new Dictionary<int, Transform>();
			bool flag = this.atmosphereNodes != null;
			if (flag)
			{
				foreach (AdventureMajorEventAtmosphereSwitcher.AtmosphereNodeConfig config in this.atmosphereNodes)
				{
					this._atmosphereNodeMap[config.atmosphereType] = config.nodeTransform;
				}
			}
		}

		// Token: 0x0600A357 RID: 41815 RVA: 0x004C701C File Offset: 0x004C521C
		private void InitRenderTextures()
		{
			this._frontRT = new RenderTexture(this.rtWidth, this.rtHeight, this.rtDepth)
			{
				name = "AtmosphereFrontRT"
			};
			this._backRT = new RenderTexture(this.rtWidth, this.rtHeight, this.rtDepth)
			{
				name = "AtmosphereBackRT"
			};
			bool flag = this.frontCamera != null;
			if (flag)
			{
				this.frontCamera.targetTexture = this._frontRT;
			}
			bool flag2 = this.backCamera != null;
			if (flag2)
			{
				this.backCamera.targetTexture = this._backRT;
			}
			bool flag3 = this.frontImage != null;
			if (flag3)
			{
				this.frontImage.texture = this._frontRT;
			}
			bool flag4 = this.backImage != null;
			if (flag4)
			{
				this.backImage.texture = this._backRT;
			}
			bool flag5 = this.dissolveMaterialAsset != null;
			if (flag5)
			{
				this._dissolveMaterialInstance = Object.Instantiate<Material>(this.dissolveMaterialAsset);
			}
		}

		// Token: 0x0600A358 RID: 41816 RVA: 0x004C7128 File Offset: 0x004C5328
		public void SetAtmosphereImmediate(int atmosphereType)
		{
			bool flag = this._switchCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._switchCoroutine);
				this._switchCoroutine = null;
			}
			this.KillSwitchTween();
			this._currentAtmosphereType = atmosphereType;
			this.MoveNodeToFrontContainer(atmosphereType);
			this.MoveOtherNodesToInactive(atmosphereType);
			bool flag2 = this.backCamera != null;
			if (flag2)
			{
				this.backCamera.gameObject.SetActive(false);
			}
			bool flag3 = this.backImage != null;
			if (flag3)
			{
				this.backImage.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A359 RID: 41817 RVA: 0x004C71BC File Offset: 0x004C53BC
		public void SwitchAtmosphere(int targetAtmosphereType)
		{
			bool flag = targetAtmosphereType == this._currentAtmosphereType;
			if (!flag)
			{
				bool flag2 = this._switchCoroutine != null;
				if (flag2)
				{
					base.StopCoroutine(this._switchCoroutine);
				}
				this.KillSwitchTween();
				this._switchCoroutine = base.StartCoroutine(this.DoSwitch(targetAtmosphereType));
			}
		}

		// Token: 0x0600A35A RID: 41818 RVA: 0x004C720D File Offset: 0x004C540D
		private IEnumerator DoSwitch(int targetType)
		{
			this.MoveNodeToBackContainer(targetType);
			bool flag = this.backCamera != null;
			if (flag)
			{
				this.backCamera.gameObject.SetActive(true);
			}
			bool flag2 = this.backImage != null;
			if (flag2)
			{
				this.backImage.gameObject.SetActive(true);
			}
			bool flag3 = this.backImage != null;
			if (flag3)
			{
				this.backImage.material = null;
			}
			bool flag4 = this._dissolveMaterialInstance != null && this.frontImage != null;
			if (flag4)
			{
				int currentType = (this._currentAtmosphereType >= 0) ? this._currentAtmosphereType : 0;
				this._dissolveMaterialInstance.SetColor(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightColor1, this.GetHighlightColor1(currentType));
				this._dissolveMaterialInstance.SetColor(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightColor2, this.GetHighlightColor2(currentType));
				this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderProgress, 0f);
				this.frontImage.material = this._dissolveMaterialInstance;
				bool flag5 = this.backImage != null;
				if (flag5)
				{
					this.backImage.material = null;
				}
				bool flag6 = this.useAnimationCurves && this._maskProgressCurve != null;
				if (flag6)
				{
					float animationLength = (this.switchAnimationClip != null) ? this.switchAnimationClip.length : this.switchDuration;
					float startTime = Time.realtimeSinceStartup;
					for (;;)
					{
						float elapsed = Time.realtimeSinceStartup - startTime;
						float normalizedTime = elapsed / animationLength;
						bool flag7 = normalizedTime >= 1f;
						if (flag7)
						{
							break;
						}
						bool flag8 = this._maskProgressCurve != null;
						if (flag8)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderProgress, this._maskProgressCurve.Evaluate(elapsed));
						}
						bool flag9 = this._highlightWidth1Curve != null;
						if (flag9)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightWidth1, this._highlightWidth1Curve.Evaluate(elapsed));
						}
						bool flag10 = this._highlightWidth2Curve != null;
						if (flag10)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightWidth2, this._highlightWidth2Curve.Evaluate(elapsed));
						}
						bool flag11 = this._highlightOffset1Curve != null;
						if (flag11)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightOffset1, this._highlightOffset1Curve.Evaluate(elapsed));
						}
						bool flag12 = this._highlightOffset2Curve != null;
						if (flag12)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightOffset2, this._highlightOffset2Curve.Evaluate(elapsed));
						}
						bool flag13 = this._highlightIntensity1Curve != null;
						if (flag13)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightIntensity1, this._highlightIntensity1Curve.Evaluate(elapsed));
						}
						bool flag14 = this._highlightIntensity2Curve != null;
						if (flag14)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderHighlightIntensity2, this._highlightIntensity2Curve.Evaluate(elapsed));
						}
						bool flag15 = this._maskShrinkCurve != null;
						if (flag15)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderMaskShrink, this._maskShrinkCurve.Evaluate(elapsed));
						}
						bool flag16 = this._edgeWidthCurve != null;
						if (flag16)
						{
							this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderEdgeWidth, this._edgeWidthCurve.Evaluate(elapsed));
						}
						yield return null;
					}
					bool flag17 = this._maskProgressCurve != null;
					if (flag17)
					{
						this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderProgress, this._maskProgressCurve.Evaluate(animationLength));
					}
				}
				else
				{
					this._switchTween = DOVirtual.Float(0f, 1f, this.switchDuration, delegate(float progress)
					{
						this._dissolveMaterialInstance.SetFloat(AdventureMajorEventAtmosphereSwitcher.ShaderProgress, progress);
					}).SetEase(Ease.Linear);
					yield return this._switchTween.WaitForCompletion();
					this._switchTween = null;
				}
			}
			else
			{
				yield return null;
			}
			this.SwapFrontAndBack(targetType);
			this._currentAtmosphereType = targetType;
			bool flag18 = this.backCamera != null;
			if (flag18)
			{
				this.backCamera.gameObject.SetActive(false);
			}
			bool flag19 = this.backImage != null;
			if (flag19)
			{
				this.backImage.gameObject.SetActive(false);
			}
			this.RestoreFrontImageMaterial();
			this.MoveOtherNodesToInactive(targetType);
			this._switchCoroutine = null;
			yield break;
		}

		// Token: 0x0600A35B RID: 41819 RVA: 0x004C7224 File Offset: 0x004C5424
		private void MoveNodeToFrontContainer(int atmosphereType)
		{
			Transform node;
			bool flag = this._atmosphereNodeMap.TryGetValue(atmosphereType, out node) && node != null;
			if (flag)
			{
				node.SetParent(this.frontEffectContainer);
				node.localPosition = Vector3.zero;
				node.localRotation = Quaternion.identity;
				node.localScale = Vector3.one;
			}
		}

		// Token: 0x0600A35C RID: 41820 RVA: 0x004C7284 File Offset: 0x004C5484
		private void MoveNodeToBackContainer(int atmosphereType)
		{
			Transform node;
			bool flag = this._atmosphereNodeMap.TryGetValue(atmosphereType, out node) && node != null;
			if (flag)
			{
				node.SetParent(this.backEffectContainer);
				node.localPosition = Vector3.zero;
				node.localRotation = Quaternion.identity;
				node.localScale = Vector3.one;
			}
		}

		// Token: 0x0600A35D RID: 41821 RVA: 0x004C72E4 File Offset: 0x004C54E4
		private void MoveOtherNodesToInactive(int activeAtmosphereType)
		{
			foreach (KeyValuePair<int, Transform> kvp in this._atmosphereNodeMap)
			{
				bool flag = kvp.Key != activeAtmosphereType && kvp.Value != null;
				if (flag)
				{
					kvp.Value.SetParent(this.inactiveContainer);
					kvp.Value.localPosition = Vector3.zero;
					kvp.Value.localRotation = Quaternion.identity;
					kvp.Value.localScale = Vector3.one;
				}
			}
		}

		// Token: 0x0600A35E RID: 41822 RVA: 0x004C73A0 File Offset: 0x004C55A0
		private void RestoreFrontImageMaterial()
		{
			bool flag = this.frontImage == null;
			if (!flag)
			{
				this.frontImage.material = null;
				this.frontImage.texture = this._frontRT;
			}
		}

		// Token: 0x0600A35F RID: 41823 RVA: 0x004C73E0 File Offset: 0x004C55E0
		private void SwapFrontAndBack(int newFrontType)
		{
			Transform transform = this.backEffectContainer;
			Transform transform2 = this.frontEffectContainer;
			this.frontEffectContainer = transform;
			this.backEffectContainer = transform2;
			Camera camera = this.backCamera;
			Camera camera2 = this.frontCamera;
			this.frontCamera = camera;
			this.backCamera = camera2;
			RenderTexture backRT = this._backRT;
			RenderTexture frontRT = this._frontRT;
			this._frontRT = backRT;
			this._backRT = frontRT;
			bool flag = this.frontCamera != null;
			if (flag)
			{
				this.frontCamera.targetTexture = this._frontRT;
			}
			bool flag2 = this.backCamera != null;
			if (flag2)
			{
				this.backCamera.targetTexture = this._backRT;
			}
			bool flag3 = this.frontImage != null;
			if (flag3)
			{
				this.frontImage.texture = this._frontRT;
			}
			bool flag4 = this.backImage != null;
			if (flag4)
			{
				this.backImage.texture = this._backRT;
			}
		}

		// Token: 0x0600A360 RID: 41824 RVA: 0x004C74D8 File Offset: 0x004C56D8
		private Color GetHighlightColor1(int atmosphereType)
		{
			bool flag = this.atmosphereHighlightColors != null && atmosphereType >= 0 && atmosphereType < this.atmosphereHighlightColors.Length;
			Color result;
			if (flag)
			{
				result = this.atmosphereHighlightColors[atmosphereType].color1;
			}
			else
			{
				if (!true)
				{
				}
				Color color;
				if (atmosphereType != 1)
				{
					if (atmosphereType != 2)
					{
						color = new Color(0.6f, 0.6f, 0.6f, 1f);
					}
					else
					{
						color = new Color(0.9f, 0.2f, 0.3f, 1f);
					}
				}
				else
				{
					color = new Color(1f, 0.8f, 0.4f, 1f);
				}
				if (!true)
				{
				}
				result = color;
			}
			return result;
		}

		// Token: 0x0600A361 RID: 41825 RVA: 0x004C7588 File Offset: 0x004C5788
		private Color GetHighlightColor2(int atmosphereType)
		{
			bool flag = this.atmosphereHighlightColors != null && atmosphereType >= 0 && atmosphereType < this.atmosphereHighlightColors.Length;
			Color result;
			if (flag)
			{
				result = this.atmosphereHighlightColors[atmosphereType].color2;
			}
			else
			{
				if (!true)
				{
				}
				Color color;
				if (atmosphereType != 1)
				{
					if (atmosphereType != 2)
					{
						color = new Color(0.5f, 0.5f, 0.5f, 1f);
					}
					else
					{
						color = new Color(0.7f, 0.1f, 0.2f, 1f);
					}
				}
				else
				{
					color = new Color(0.9f, 0.7f, 0.3f, 1f);
				}
				if (!true)
				{
				}
				result = color;
			}
			return result;
		}

		// Token: 0x0600A362 RID: 41826 RVA: 0x004C7638 File Offset: 0x004C5838
		private void KillSwitchTween()
		{
			bool flag = this._switchTween == null;
			if (!flag)
			{
				this._switchTween.Kill(false);
				this._switchTween = null;
			}
		}

		// Token: 0x0600A363 RID: 41827 RVA: 0x004C766C File Offset: 0x004C586C
		private void OnDestroy()
		{
			bool flag = this._switchCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._switchCoroutine);
				this._switchCoroutine = null;
			}
			this.KillSwitchTween();
			this.RestoreAllNodesToInactive();
			bool flag2 = this._runtimeFrontRoot != null;
			if (flag2)
			{
				Object.Destroy(this._runtimeFrontRoot);
			}
			bool flag3 = this._runtimeBackRoot != null;
			if (flag3)
			{
				Object.Destroy(this._runtimeBackRoot);
			}
			bool flag4 = this._frontRT != null;
			if (flag4)
			{
				this._frontRT.Release();
				Object.Destroy(this._frontRT);
			}
			bool flag5 = this._backRT != null;
			if (flag5)
			{
				this._backRT.Release();
				Object.Destroy(this._backRT);
			}
			bool flag6 = this._dissolveMaterialInstance != null;
			if (flag6)
			{
				Object.Destroy(this._dissolveMaterialInstance);
			}
		}

		// Token: 0x0600A364 RID: 41828 RVA: 0x004C7758 File Offset: 0x004C5958
		private void RestoreAllNodesToInactive()
		{
			bool flag = this.inactiveContainer == null || this._atmosphereNodeMap == null;
			if (!flag)
			{
				foreach (KeyValuePair<int, Transform> kvp in this._atmosphereNodeMap)
				{
					Transform node = kvp.Value;
					bool flag2 = node == null;
					if (!flag2)
					{
						node.SetParent(this.inactiveContainer);
						node.localPosition = Vector3.zero;
						node.localRotation = Quaternion.identity;
						node.localScale = Vector3.one;
					}
				}
			}
		}

		// Token: 0x04007EEB RID: 32491
		[Header("相机层 (前后两层，动态创建)")]
		[Tooltip("前景相机 - 显示当前气氛")]
		[SerializeField]
		private Camera frontCamera;

		// Token: 0x04007EEC RID: 32492
		[Tooltip("后景相机 - 显示目标气氛，仅切换时激活")]
		[SerializeField]
		private Camera backCamera;

		// Token: 0x04007EED RID: 32493
		[Header("CRawImage 显示节点")]
		[Tooltip("前景 CRawImage，正常显示当前气氛 RT；切换时换成 dissolve 材质")]
		[SerializeField]
		private CRawImage frontImage;

		// Token: 0x04007EEE RID: 32494
		[Tooltip("后景 CRawImage，切换时激活，显示目标气氛 RT（位于前景下方）")]
		[SerializeField]
		private CRawImage backImage;

		// Token: 0x04007EEF RID: 32495
		[Header("节点容器（前后容器运行时动态创建）")]
		[Tooltip("前景特效容器 - 当前活跃气氛的节点（运行时创建）")]
		private Transform frontEffectContainer;

		// Token: 0x04007EF0 RID: 32496
		[Tooltip("后景特效容器 - 目标气氛的节点（运行时创建）")]
		private Transform backEffectContainer;

		// Token: 0x04007EF1 RID: 32497
		[Tooltip("默认容器 - 非活跃的气氛节点存放处")]
		[SerializeField]
		private Transform inactiveContainer;

		// Token: 0x04007EF2 RID: 32498
		[Header("动态相机/Canvas 参数")]
		[SerializeField]
		private int atmosphereLayer = 5;

		// Token: 0x04007EF3 RID: 32499
		[SerializeField]
		private float cameraDepthFront = 201f;

		// Token: 0x04007EF4 RID: 32500
		[SerializeField]
		private float cameraDepthBack = 200f;

		// Token: 0x04007EF5 RID: 32501
		private GameObject _runtimeFrontRoot;

		// Token: 0x04007EF6 RID: 32502
		private GameObject _runtimeBackRoot;

		// Token: 0x04007EF7 RID: 32503
		[Header("气氛节点配置")]
		[Tooltip("3个气氛类型对应的节点 Transform（预置在界面中，不需要实例化）")]
		[SerializeField]
		private AdventureMajorEventAtmosphereSwitcher.AtmosphereNodeConfig[] atmosphereNodes;

		// Token: 0x04007EF8 RID: 32504
		[Header("RT 参数")]
		[SerializeField]
		private int rtWidth = 1920;

		// Token: 0x04007EF9 RID: 32505
		[SerializeField]
		private int rtHeight = 1080;

		// Token: 0x04007EFA RID: 32506
		[SerializeField]
		private int rtDepth = 16;

		// Token: 0x04007EFB RID: 32507
		[Header("过渡效果")]
		[Tooltip("AtmosphereDissolve 材质 Asset（会在运行时 Instantiate 为实例以避免污染 Asset）")]
		[SerializeField]
		private Material dissolveMaterialAsset;

		// Token: 0x04007EFC RID: 32508
		[SerializeField]
		private AnimationClip switchAnimationClip;

		// Token: 0x04007EFD RID: 32509
		[SerializeField]
		private float switchDuration = 0.8f;

		// Token: 0x04007EFE RID: 32510
		[Header("动画参数")]
		[SerializeField]
		private bool useAnimationCurves = true;

		// Token: 0x04007EFF RID: 32511
		[SerializeField]
		public List<AdventureMajorEventAtmosphereSwitcher.AnimationCurveData> animationCurves = new List<AdventureMajorEventAtmosphereSwitcher.AnimationCurveData>();

		// Token: 0x04007F00 RID: 32512
		[Header("双层高亮颜色配置（数组形式，下标对应氛围类型值）")]
		[Tooltip("每个氛围类型对应的高亮颜色数组，[0]=默认, [1]=正面, [2]=负面\n每个元素包含2个颜色：Color1对应_ShaderHighlightColor1，Color2对应_ShaderHighlightColor2")]
		[SerializeField]
		private AdventureMajorEventAtmosphereSwitcher.HighlightColorPair[] atmosphereHighlightColors = new AdventureMajorEventAtmosphereSwitcher.HighlightColorPair[3];

		// Token: 0x04007F01 RID: 32513
		[Header("颜色参考材质（可选）")]
		[Tooltip("拖入材质文件，程序会自动提取其中的_ShaderHighlightColor1和_ShaderHighlightColor2作为各氛围类型的颜色参考")]
		[SerializeField]
		private Material[] colorReferenceMaterials = new Material[3];

		// Token: 0x04007F02 RID: 32514
		private const int AtmosphereDefault = 0;

		// Token: 0x04007F03 RID: 32515
		private const int AtmospherePositive = 1;

		// Token: 0x04007F04 RID: 32516
		private const int AtmosphereNegative = 2;

		// Token: 0x04007F05 RID: 32517
		private static readonly int ShaderProgress = Shader.PropertyToID("_MaskProgress");

		// Token: 0x04007F06 RID: 32518
		private static readonly int ShaderHighlightColor1 = Shader.PropertyToID("_HighlightColor1");

		// Token: 0x04007F07 RID: 32519
		private static readonly int ShaderHighlightColor2 = Shader.PropertyToID("_HighlightColor2");

		// Token: 0x04007F08 RID: 32520
		private static readonly int ShaderHighlightIntensity1 = Shader.PropertyToID("_HighlightIntensity1");

		// Token: 0x04007F09 RID: 32521
		private static readonly int ShaderHighlightIntensity2 = Shader.PropertyToID("_HighlightIntensity2");

		// Token: 0x04007F0A RID: 32522
		private static readonly int ShaderHighlightWidth1 = Shader.PropertyToID("_HighlightWidth1");

		// Token: 0x04007F0B RID: 32523
		private static readonly int ShaderHighlightWidth2 = Shader.PropertyToID("_HighlightWidth2");

		// Token: 0x04007F0C RID: 32524
		private static readonly int ShaderHighlightOffset1 = Shader.PropertyToID("_HighlightOffset1");

		// Token: 0x04007F0D RID: 32525
		private static readonly int ShaderHighlightOffset2 = Shader.PropertyToID("_HighlightOffset2");

		// Token: 0x04007F0E RID: 32526
		private static readonly int ShaderMaskShrink = Shader.PropertyToID("_MaskShrink");

		// Token: 0x04007F0F RID: 32527
		private static readonly int ShaderEdgeWidth = Shader.PropertyToID("_EdgeWidth");

		// Token: 0x04007F10 RID: 32528
		private int _currentAtmosphereType = -1;

		// Token: 0x04007F11 RID: 32529
		private Coroutine _switchCoroutine;

		// Token: 0x04007F12 RID: 32530
		private Tween _switchTween;

		// Token: 0x04007F13 RID: 32531
		private AnimationCurve _maskProgressCurve;

		// Token: 0x04007F14 RID: 32532
		private AnimationCurve _highlightWidth1Curve;

		// Token: 0x04007F15 RID: 32533
		private AnimationCurve _highlightWidth2Curve;

		// Token: 0x04007F16 RID: 32534
		private AnimationCurve _highlightOffset1Curve;

		// Token: 0x04007F17 RID: 32535
		private AnimationCurve _highlightOffset2Curve;

		// Token: 0x04007F18 RID: 32536
		private AnimationCurve _highlightIntensity1Curve;

		// Token: 0x04007F19 RID: 32537
		private AnimationCurve _highlightIntensity2Curve;

		// Token: 0x04007F1A RID: 32538
		private AnimationCurve _maskShrinkCurve;

		// Token: 0x04007F1B RID: 32539
		private AnimationCurve _edgeWidthCurve;

		// Token: 0x04007F1C RID: 32540
		private RenderTexture _frontRT;

		// Token: 0x04007F1D RID: 32541
		private RenderTexture _backRT;

		// Token: 0x04007F1E RID: 32542
		private Material _dissolveMaterialInstance;

		// Token: 0x04007F1F RID: 32543
		private Dictionary<int, Transform> _atmosphereNodeMap;

		// Token: 0x020023E7 RID: 9191
		[Serializable]
		public struct AtmosphereNodeConfig
		{
			// Token: 0x0400E0E2 RID: 57570
			[Tooltip("气氛类型值（0=默认 1=正面 2=负面）")]
			public int atmosphereType;

			// Token: 0x0400E0E3 RID: 57571
			[Tooltip("该气氛类型的节点 Transform")]
			public Transform nodeTransform;
		}

		// Token: 0x020023E8 RID: 9192
		[Serializable]
		public class AnimationCurveData
		{
			// Token: 0x0400E0E4 RID: 57572
			public string propertyName;

			// Token: 0x0400E0E5 RID: 57573
			public AnimationCurve curve;
		}

		// Token: 0x020023E9 RID: 9193
		[Serializable]
		public struct HighlightColorPair
		{
			// Token: 0x0400E0E6 RID: 57574
			public Color color1;

			// Token: 0x0400E0E7 RID: 57575
			public Color color2;
		}
	}
}
