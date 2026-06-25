using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Adventure;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C68 RID: 3176
	[ExecuteAlways]
	public class AdventureLightingManager : MonoBehaviour
	{
		// Token: 0x170010ED RID: 4333
		// (get) Token: 0x0600A1A0 RID: 41376 RVA: 0x004B88E4 File Offset: 0x004B6AE4
		// (set) Token: 0x0600A1A1 RID: 41377 RVA: 0x004B88EB File Offset: 0x004B6AEB
		public static AdventureLightingManager Instance { get; private set; }

		// Token: 0x170010EE RID: 4334
		// (get) Token: 0x0600A1A2 RID: 41378 RVA: 0x004B88F4 File Offset: 0x004B6AF4
		public bool IsCustomLightingEnabled
		{
			get
			{
				return this.EnableCustomLighting && AdventureLightingManager.IsSystemLightingEnabled();
			}
		}

		// Token: 0x0600A1A3 RID: 41379 RVA: 0x004B8918 File Offset: 0x004B6B18
		public static bool IsFeatureEnabled()
		{
			bool flag = !Application.isPlaying;
			bool result;
			if (flag)
			{
				result = (AdventureLightingManager.Instance == null || AdventureLightingManager.Instance.EnableCustomLighting);
			}
			else
			{
				result = (AdventureLightingManager.IsSystemLightingEnabled() && (AdventureLightingManager.Instance == null || AdventureLightingManager.Instance.EnableCustomLighting));
			}
			return result;
		}

		// Token: 0x0600A1A4 RID: 41380 RVA: 0x004B8978 File Offset: 0x004B6B78
		public static bool HasActiveLighting()
		{
			return AdventureLightingManager.Instance != null && AdventureLightingManager.Instance.IsCustomLightingEnabled;
		}

		// Token: 0x0600A1A5 RID: 41381 RVA: 0x004B89A4 File Offset: 0x004B6BA4
		private static bool IsSystemLightingEnabled()
		{
			bool flag = !Application.isPlaying;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
				result = (globalSettings == null || globalSettings.AdventureLighting);
			}
			return result;
		}

		// Token: 0x0600A1A6 RID: 41382 RVA: 0x004B89D8 File Offset: 0x004B6BD8
		public void RotateAzimuth()
		{
			bool flag = !this.IsCustomLightingEnabled;
			if (!flag)
			{
				bool flag2 = this._rotationTweener != null && this._rotationTweener.IsActive();
				if (flag2)
				{
					this._rotationTweener.Kill(false);
				}
				float targetAngle = this.GlobalAzimuthAngle + this.RotationAnglePerStep;
				this._rotationTweener = DOTween.To(() => this.GlobalAzimuthAngle, delegate(float x)
				{
					this.GlobalAzimuthAngle = x;
				}, targetAngle, this.RotationDuration).SetEase(Ease.OutQuad);
			}
		}

		// Token: 0x0600A1A7 RID: 41383 RVA: 0x004B8A5C File Offset: 0x004B6C5C
		public static void AddPendingLight(AdventurePointLight light)
		{
			bool flag = !AdventureLightingManager._pendingLights.Contains(light);
			if (flag)
			{
				AdventureLightingManager._pendingLights.Add(light);
			}
		}

		// Token: 0x0600A1A8 RID: 41384 RVA: 0x004B8A88 File Offset: 0x004B6C88
		public static void RemovePendingLight(AdventurePointLight light)
		{
			AdventureLightingManager._pendingLights.Remove(light);
		}

		// Token: 0x170010EF RID: 4335
		// (get) Token: 0x0600A1A9 RID: 41385 RVA: 0x004B8A97 File Offset: 0x004B6C97
		// (set) Token: 0x0600A1AA RID: 41386 RVA: 0x004B8A9F File Offset: 0x004B6C9F
		public bool IsMonthPassEffectReady { get; set; }

		// Token: 0x0600A1AB RID: 41387 RVA: 0x004B8AA8 File Offset: 0x004B6CA8
		public void PlayMonthPassEffect()
		{
			bool flag = !this.IsMonthPassEffectReady;
			if (!flag)
			{
				this.IsMonthPassEffectReady = false;
				bool flag2 = this._effectTweener != null && this._effectTweener.IsActive();
				if (flag2)
				{
					this._effectTweener.Kill(false);
				}
				this._effectIntensityMultiplier = this.MonthPassStartPercentage;
				this._effectTweener = DOTween.To(() => this._effectIntensityMultiplier, delegate(float x)
				{
					this._effectIntensityMultiplier = x;
				}, 1f, this.MonthPassEffectDuration).SetEase(Ease.OutQuad);
			}
		}

		// Token: 0x0600A1AC RID: 41388 RVA: 0x004B8B34 File Offset: 0x004B6D34
		private void OnEnable()
		{
			AdventureLightingManager.Instance = this;
			this._activePointLights.Clear();
			this._activeHighlights.Clear();
			this._runtimeLights.Clear();
			this._ghostLights.Clear();
			this._effectIntensityMultiplier = 1f;
			GEvent.Add(UiEvents.OnAdvanceMonthAnimationComplete, new GEvent.Callback(this.OnAdvanveMonthAnimationComplete));
			GEvent.Add(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyProcessComplete));
			GEvent.Add(UiEvents.AdventureLightingSettingChanged, new GEvent.Callback(this.AdventureLightingSettingChanged));
			this.RegisterExistingLights();
			this.UpdateShaderData();
		}

		// Token: 0x0600A1AD RID: 41389 RVA: 0x004B8BE0 File Offset: 0x004B6DE0
		private void OnDisable()
		{
			bool flag = AdventureLightingManager.Instance == this;
			if (flag)
			{
				AdventureLightingManager.Instance = null;
			}
			this._activePointLights.Clear();
			this._activeHighlights.Clear();
			this._runtimeLights.Clear();
			this._ghostLights.Clear();
			bool flag2 = this._effectTweener != null && this._effectTweener.IsActive();
			if (flag2)
			{
				this._effectTweener.Kill(false);
			}
			bool flag3 = this._rotationTweener != null && this._rotationTweener.IsActive();
			if (flag3)
			{
				this._rotationTweener.Kill(false);
			}
			GEvent.Remove(UiEvents.OnAdvanceMonthAnimationComplete, new GEvent.Callback(this.OnAdvanveMonthAnimationComplete));
			GEvent.Remove(UiEvents.MonthNotifyProcessComplete, new GEvent.Callback(this.OnMonthNotifyProcessComplete));
			GEvent.Remove(UiEvents.AdventureLightingSettingChanged, new GEvent.Callback(this.AdventureLightingSettingChanged));
			this.ApplyDisabledShaderState();
		}

		// Token: 0x0600A1AE RID: 41390 RVA: 0x004B8CD7 File Offset: 0x004B6ED7
		private void OnAdvanveMonthAnimationComplete(ArgumentBox argBox)
		{
			this.IsMonthPassEffectReady = true;
		}

		// Token: 0x0600A1AF RID: 41391 RVA: 0x004B8CE2 File Offset: 0x004B6EE2
		private void OnMonthNotifyProcessComplete(ArgumentBox argBox)
		{
			this.PlayMonthPassEffect();
		}

		// Token: 0x0600A1B0 RID: 41392 RVA: 0x004B8CEC File Offset: 0x004B6EEC
		private void Update()
		{
			this.UpdateShaderData();
		}

		// Token: 0x0600A1B1 RID: 41393 RVA: 0x004B8CF8 File Offset: 0x004B6EF8
		private void AdventureLightingSettingChanged(ArgumentBox argBox)
		{
			bool flag = !this.IsCustomLightingEnabled;
			if (flag)
			{
				bool flag2 = this._rotationTweener != null && this._rotationTweener.IsActive();
				if (flag2)
				{
					this._rotationTweener.Kill(false);
				}
				bool flag3 = this._effectTweener != null && this._effectTweener.IsActive();
				if (flag3)
				{
					this._effectTweener.Kill(false);
				}
			}
			this.UpdateShaderData();
		}

		// Token: 0x0600A1B2 RID: 41394 RVA: 0x004B8D6C File Offset: 0x004B6F6C
		private void RegisterExistingLights()
		{
			foreach (AdventurePointLight light in AdventureLightingManager._pendingLights)
			{
				bool flag = !light.isActiveAndEnabled;
				if (!flag)
				{
					this.RegisterLight(light);
				}
			}
			AdventureLightingManager._pendingLights.Clear();
		}

		// Token: 0x0600A1B3 RID: 41395 RVA: 0x004B8DE0 File Offset: 0x004B6FE0
		private void ApplyDisabledShaderState()
		{
			Shader.SetGlobalColor(AdventureLightingManager.IdGlobalLightColor, new Color(0f, 0f, 0f, -1f));
			Shader.SetGlobalInt(AdventureLightingManager.IdPointLightCount, 0);
			Shader.SetGlobalInt(AdventureLightingManager.IdHighlightCount, 0);
		}

		// Token: 0x0600A1B4 RID: 41396 RVA: 0x004B8E20 File Offset: 0x004B7020
		public void RegisterLight(AdventurePointLight light)
		{
			bool flag = !this._activePointLights.Contains(light);
			if (flag)
			{
				this._activePointLights.Add(light);
				AdventureLightingManager.LightRuntimeData data = new AdventureLightingManager.LightRuntimeData
				{
					SourceLight = light,
					Index = light.BlockIndex,
					CurrentFade = 0f,
					IsGhost = false,
					FadeDuration = this.LightFadeDuration
				};
				this._runtimeLights.Add(data);
			}
		}

		// Token: 0x0600A1B5 RID: 41397 RVA: 0x004B8E94 File Offset: 0x004B7094
		public void UnregisterLight(AdventurePointLight light)
		{
			bool flag = this._activePointLights.Contains(light);
			if (flag)
			{
				this._activePointLights.Remove(light);
				AdventureLightingManager.LightRuntimeData data = this._runtimeLights.Find((AdventureLightingManager.LightRuntimeData x) => x.SourceLight == light);
				bool flag2 = data != null;
				if (flag2)
				{
					this._runtimeLights.Remove(data);
					this.CreateGhost(data);
				}
			}
		}

		// Token: 0x0600A1B6 RID: 41398 RVA: 0x004B8F10 File Offset: 0x004B7110
		private void CreateGhost(AdventureLightingManager.LightRuntimeData sourceData)
		{
			bool flag = sourceData.CurrentFade <= 0.01f;
			if (!flag)
			{
				AdventureLightingManager.LightRuntimeData ghost = new AdventureLightingManager.LightRuntimeData
				{
					SourceLight = null,
					Index = sourceData.Index,
					CurrentFade = sourceData.CurrentFade,
					IsGhost = true,
					TargetIntensity = ((sourceData.SourceLight != null) ? sourceData.SourceLight.CurrentIntensity : 1f),
					FadeDuration = this.LightFadeDuration,
					SnapshotColor = ((sourceData.SourceLight != null) ? sourceData.SourceLight.LightColor : Color.white),
					SnapshotShape = ((sourceData.SourceLight != null) ? sourceData.SourceLight.Shape : AdventurePointLight.ShapeType.OneByOne),
					SnapshotMode = ((sourceData.SourceLight != null) ? sourceData.SourceLight.Mode : AdventurePointLight.LightMode.Uniform),
					SnapshotRange = ((sourceData.SourceLight != null) ? sourceData.SourceLight.Range : 1f),
					SnapshotFullRange = ((sourceData.SourceLight != null) ? sourceData.SourceLight.FullIntensityRange : 1),
					SnapshotAngle = ((sourceData.SourceLight != null) ? sourceData.SourceLight.Angle : 45f),
					SnapshotVirtualZ = ((sourceData.SourceLight != null) ? sourceData.SourceLight.VirtualZ : 1f),
					SnapshotWorldPos = ((sourceData.SourceLight != null) ? sourceData.SourceLight.transform.position : AdventureLightingManager.GetGridVector(sourceData.Index)),
					SnapshotWorldScale = ((sourceData.SourceLight != null) ? sourceData.SourceLight.transform.lossyScale.x : 1f),
					SnapshotPriority = ((sourceData.SourceLight != null) ? sourceData.SourceLight.Priority : 0),
					SnapshotNoRangeClamp = (sourceData.SourceLight != null && sourceData.SourceLight.NoRangeClamp)
				};
				this._ghostLights.Add(ghost);
			}
		}

		// Token: 0x0600A1B7 RID: 41399 RVA: 0x004B914A File Offset: 0x004B734A
		public void ClearHighlights()
		{
			this._activeHighlights.Clear();
		}

		// Token: 0x0600A1B8 RID: 41400 RVA: 0x004B9159 File Offset: 0x004B7359
		public void AddHighlight(AdventureBlockIndex index, float intensity)
		{
			this.AddHighlight(index, Color.white, intensity);
		}

		// Token: 0x0600A1B9 RID: 41401 RVA: 0x004B916C File Offset: 0x004B736C
		public void AddHighlight(AdventureBlockIndex index, Color color, float intensity)
		{
			this._activeHighlights.Add(new AdventureLightingManager.HighlightData
			{
				Index = index,
				Color = color,
				Intensity = intensity
			});
		}

		// Token: 0x0600A1BA RID: 41402 RVA: 0x004B91A7 File Offset: 0x004B73A7
		private void UpdateShaderData()
		{
			this.UpdateGlobalLightParams();
			this.UpdatePointLightParams();
			this.UpdateHighlightParams();
		}

		// Token: 0x0600A1BB RID: 41403 RVA: 0x004B91C0 File Offset: 0x004B73C0
		private void UpdateGlobalLightParams()
		{
			bool flag = !this.IsCustomLightingEnabled;
			if (flag)
			{
				this.ApplyDisabledShaderState();
			}
			else
			{
				float radPhi = this.GlobalIncidenceAngle * 0.017453292f;
				float radTheta = this.GlobalAzimuthAngle * 0.017453292f;
				float z = Mathf.Sin(radPhi);
				float xyMag = Mathf.Cos(radPhi);
				float x = Mathf.Sin(radTheta) * xyMag;
				float y = Mathf.Cos(radTheta) * xyMag;
				Vector3 dir = new Vector3(-x, -y, z);
				Shader.SetGlobalVector(AdventureLightingManager.IdGlobalLightDir, new Vector4(dir.x, dir.y, dir.z, 0f));
				Color globalCol = this.GlobalColor;
				globalCol.a = this.GlobalIntensity * this._effectIntensityMultiplier;
				Shader.SetGlobalColor(AdventureLightingManager.IdGlobalLightColor, globalCol);
			}
		}

		// Token: 0x0600A1BC RID: 41404 RVA: 0x004B9288 File Offset: 0x004B7488
		private void UpdatePointLightParams()
		{
			bool flag = !this.IsCustomLightingEnabled;
			if (!flag)
			{
				float dt = Time.deltaTime;
				for (int i = this._runtimeLights.Count - 1; i >= 0; i--)
				{
					AdventureLightingManager.LightRuntimeData data = this._runtimeLights[i];
					bool flag2 = data.SourceLight == null;
					if (flag2)
					{
						this._runtimeLights.RemoveAt(i);
					}
					else
					{
						AdventureBlockIndex currentIndex = data.SourceLight.BlockIndex;
						bool flag3 = currentIndex != data.Index;
						if (flag3)
						{
							this.CreateGhost(data);
							data.Index = currentIndex;
							data.CurrentFade = 0f;
						}
						bool flag4 = data.CurrentFade < 1f;
						if (flag4)
						{
							data.CurrentFade += dt / Mathf.Max(0.01f, data.FadeDuration);
							bool flag5 = data.CurrentFade > 1f;
							if (flag5)
							{
								data.CurrentFade = 1f;
							}
						}
					}
				}
				for (int j = this._ghostLights.Count - 1; j >= 0; j--)
				{
					AdventureLightingManager.LightRuntimeData ghost = this._ghostLights[j];
					ghost.CurrentFade -= dt / Mathf.Max(0.01f, ghost.FadeDuration);
					bool flag6 = ghost.CurrentFade <= 0f;
					if (flag6)
					{
						this._ghostLights.RemoveAt(j);
					}
				}
				int shaderLightCount = 0;
				foreach (AdventureLightingManager.LightRuntimeData data2 in this._runtimeLights)
				{
					bool flag7 = shaderLightCount >= 64;
					if (flag7)
					{
						break;
					}
					this.FillShaderLightData(shaderLightCount, data2);
					shaderLightCount++;
				}
				foreach (AdventureLightingManager.LightRuntimeData ghost2 in this._ghostLights)
				{
					bool flag8 = shaderLightCount >= 64;
					if (flag8)
					{
						break;
					}
					this.FillShaderLightData(shaderLightCount, ghost2);
					shaderLightCount++;
				}
				this._pointLightCount = shaderLightCount;
				Shader.SetGlobalVectorArray(AdventureLightingManager.IdPointLightPos, this._pointLightPos);
				Shader.SetGlobalVectorArray(AdventureLightingManager.IdPointLightWorldPos, this._pointLightWorldPos);
				Shader.SetGlobalVectorArray(AdventureLightingManager.IdPointLightColor, this._pointLightColor);
				Shader.SetGlobalVectorArray(AdventureLightingManager.IdPointLightParam, this._pointLightParam);
				Shader.SetGlobalInt(AdventureLightingManager.IdPointLightCount, this._pointLightCount);
			}
		}

		// Token: 0x0600A1BD RID: 41405 RVA: 0x004B9544 File Offset: 0x004B7744
		private void FillShaderLightData(int index, AdventureLightingManager.LightRuntimeData data)
		{
			Color color = Color.white;
			bool isGhost = data.IsGhost;
			float intensity;
			float shape;
			float mode;
			float range;
			float fullRange;
			float posZ;
			bool noRangeClamp;
			if (isGhost)
			{
				intensity = data.TargetIntensity * data.CurrentFade;
				float angle = data.SnapshotAngle;
				color = data.SnapshotColor;
				shape = (float)data.SnapshotShape;
				mode = (float)data.SnapshotMode;
				range = data.SnapshotRange;
				fullRange = (float)data.SnapshotFullRange;
				posZ = ((mode > 0.5f) ? data.SnapshotVirtualZ : (angle * 0.017453292f));
				noRangeClamp = data.SnapshotNoRangeClamp;
			}
			else
			{
				intensity = data.SourceLight.CurrentIntensity * data.CurrentFade;
				float angle = data.SourceLight.Angle;
				color = data.SourceLight.LightColor;
				shape = (float)data.SourceLight.Shape;
				mode = (float)data.SourceLight.Mode;
				range = data.SourceLight.Range;
				fullRange = (float)data.SourceLight.FullIntensityRange;
				posZ = ((mode > 0.5f) ? data.SourceLight.VirtualZ : (angle * 0.017453292f));
				noRangeClamp = data.SourceLight.NoRangeClamp;
			}
			intensity *= this._effectIntensityMultiplier;
			this._pointLightPos[index] = new Vector4((float)data.Index.Gx, (float)data.Index.Gy, posZ, mode);
			Vector2 lightWorldPos2D = data.IsGhost ? data.SnapshotWorldPos : ((data.SourceLight != null) ? data.SourceLight.transform.position : AdventureLightingManager.GetGridVector(data.Index));
			float worldScale = data.IsGhost ? data.SnapshotWorldScale : ((data.SourceLight != null) ? data.SourceLight.transform.lossyScale.x : 1f);
			this._pointLightWorldPos[index] = new Vector4(lightWorldPos2D.x, lightWorldPos2D.y, worldScale, noRangeClamp ? 1f : 0f);
			color.a = intensity;
			this._pointLightColor[index] = color;
			float priority = (float)(data.IsGhost ? data.SnapshotPriority : ((data.SourceLight != null) ? data.SourceLight.Priority : 0));
			this._pointLightParam[index] = new Vector4(shape, range, fullRange, priority);
		}

		// Token: 0x0600A1BE RID: 41406 RVA: 0x004B97D4 File Offset: 0x004B79D4
		private void UpdateHighlightParams()
		{
			bool flag = !this.IsCustomLightingEnabled;
			if (!flag)
			{
				this._highlightCount = Mathf.Min(this._activeHighlights.Count, 64);
				for (int i = 0; i < this._highlightCount; i++)
				{
					AdventureLightingManager.HighlightData h = this._activeHighlights[i];
					float intensity = h.Intensity * this._effectIntensityMultiplier;
					this._highlightGrids[i] = new Vector4((float)h.Index.Gx, (float)h.Index.Gy, intensity, 0f);
					this._highlightColors[i] = h.Color;
				}
				Shader.SetGlobalVectorArray(AdventureLightingManager.IdHighlightGrids, this._highlightGrids);
				Shader.SetGlobalVectorArray(AdventureLightingManager.IdHighlightColors, this._highlightColors);
				Shader.SetGlobalInt(AdventureLightingManager.IdHighlightCount, this._highlightCount);
			}
		}

		// Token: 0x0600A1BF RID: 41407 RVA: 0x004B98BC File Offset: 0x004B7ABC
		private static Vector2 GetGridVector(AdventureBlockIndex index)
		{
			return new Vector2((float)index.Gx, (float)index.Gy);
		}

		// Token: 0x04007D80 RID: 32128
		private const int MAX_LIGHT_COUNT = 64;

		// Token: 0x04007D81 RID: 32129
		private const int MAX_HIGHLIGHT_COUNT = 64;

		// Token: 0x04007D82 RID: 32130
		[Header("System Settings")]
		public bool EnableCustomLighting = true;

		// Token: 0x04007D83 RID: 32131
		[Header("Global Light Settings")]
		[Tooltip("入射角 (0-90)。90度垂直于屏幕（正面），0度为平行（掠射）。")]
		[Range(0f, 90f)]
		public float GlobalIncidenceAngle = 60f;

		// Token: 0x04007D84 RID: 32132
		[Tooltip("方位角 (0-360)。0度为上方，90度为右侧，180度为下方（顺时针）。")]
		[Range(0f, 360f)]
		public float GlobalAzimuthAngle = 0f;

		// Token: 0x04007D85 RID: 32133
		public Color GlobalColor = Color.white;

		// Token: 0x04007D86 RID: 32134
		[Range(0f, 5f)]
		public float GlobalIntensity = 1f;

		// Token: 0x04007D87 RID: 32135
		[Header("Interaction Settings")]
		[Tooltip("每次行动力变化时，方位角增加的角度")]
		public float RotationAnglePerStep = 5f;

		// Token: 0x04007D88 RID: 32136
		[Tooltip("方位角平滑过渡的时间")]
		public float RotationDuration = 0.3f;

		// Token: 0x04007D89 RID: 32137
		private Tweener _rotationTweener;

		// Token: 0x04007D8A RID: 32138
		private Vector4[] _pointLightPos = new Vector4[64];

		// Token: 0x04007D8B RID: 32139
		private Vector4[] _pointLightWorldPos = new Vector4[64];

		// Token: 0x04007D8C RID: 32140
		private Vector4[] _pointLightColor = new Vector4[64];

		// Token: 0x04007D8D RID: 32141
		private Vector4[] _pointLightParam = new Vector4[64];

		// Token: 0x04007D8E RID: 32142
		private int _pointLightCount = 0;

		// Token: 0x04007D8F RID: 32143
		private Vector4[] _highlightGrids = new Vector4[64];

		// Token: 0x04007D90 RID: 32144
		private Vector4[] _highlightColors = new Vector4[64];

		// Token: 0x04007D91 RID: 32145
		private int _highlightCount = 0;

		// Token: 0x04007D92 RID: 32146
		private List<AdventurePointLight> _activePointLights = new List<AdventurePointLight>();

		// Token: 0x04007D93 RID: 32147
		private static List<AdventurePointLight> _pendingLights = new List<AdventurePointLight>();

		// Token: 0x04007D94 RID: 32148
		private List<AdventureLightingManager.HighlightData> _activeHighlights = new List<AdventureLightingManager.HighlightData>();

		// Token: 0x04007D95 RID: 32149
		private static readonly int IdGlobalLightDir = Shader.PropertyToID("_GlobalLightDir");

		// Token: 0x04007D96 RID: 32150
		private static readonly int IdGlobalLightColor = Shader.PropertyToID("_GlobalLightColor");

		// Token: 0x04007D97 RID: 32151
		private static readonly int IdPointLightPos = Shader.PropertyToID("_AdvPointLightPos");

		// Token: 0x04007D98 RID: 32152
		private static readonly int IdPointLightWorldPos = Shader.PropertyToID("_AdvPointLightWorldPos");

		// Token: 0x04007D99 RID: 32153
		private static readonly int IdPointLightColor = Shader.PropertyToID("_AdvPointLightColor");

		// Token: 0x04007D9A RID: 32154
		private static readonly int IdPointLightParam = Shader.PropertyToID("_AdvPointLightParam");

		// Token: 0x04007D9B RID: 32155
		private static readonly int IdPointLightCount = Shader.PropertyToID("_AdvPointLightCount");

		// Token: 0x04007D9C RID: 32156
		private static readonly int IdHighlightGrids = Shader.PropertyToID("_AdvHighlightGrids");

		// Token: 0x04007D9D RID: 32157
		private static readonly int IdHighlightColors = Shader.PropertyToID("_AdvHighlightColors");

		// Token: 0x04007D9E RID: 32158
		private static readonly int IdHighlightCount = Shader.PropertyToID("_AdvHighlightCount");

		// Token: 0x04007D9F RID: 32159
		private List<AdventureLightingManager.LightRuntimeData> _runtimeLights = new List<AdventureLightingManager.LightRuntimeData>();

		// Token: 0x04007DA0 RID: 32160
		private List<AdventureLightingManager.LightRuntimeData> _ghostLights = new List<AdventureLightingManager.LightRuntimeData>();

		// Token: 0x04007DA1 RID: 32161
		[Header("Transition Settings")]
		[Tooltip("灯光移动或开关时的淡入淡出时间 (秒)")]
		public float LightFadeDuration = 0.3f;

		// Token: 0x04007DA2 RID: 32162
		[Header("Month Pass Effect Settings")]
		[Tooltip("过月效果持续时间")]
		public float MonthPassEffectDuration = 2f;

		// Token: 0x04007DA3 RID: 32163
		[Tooltip("过月效果起始强度百分比 (0-1)")]
		[Range(0f, 1f)]
		public float MonthPassStartPercentage = 0f;

		// Token: 0x04007DA5 RID: 32165
		private float _effectIntensityMultiplier = 1f;

		// Token: 0x04007DA6 RID: 32166
		private Tweener _effectTweener;

		// Token: 0x0200239F RID: 9119
		private struct HighlightData
		{
			// Token: 0x0400DF6A RID: 57194
			public AdventureBlockIndex Index;

			// Token: 0x0400DF6B RID: 57195
			public Color Color;

			// Token: 0x0400DF6C RID: 57196
			public float Intensity;
		}

		// Token: 0x020023A0 RID: 9120
		private class LightRuntimeData
		{
			// Token: 0x0400DF6D RID: 57197
			public AdventurePointLight SourceLight;

			// Token: 0x0400DF6E RID: 57198
			public AdventureBlockIndex Index;

			// Token: 0x0400DF6F RID: 57199
			public float CurrentFade;

			// Token: 0x0400DF70 RID: 57200
			public bool IsGhost;

			// Token: 0x0400DF71 RID: 57201
			public float TargetIntensity;

			// Token: 0x0400DF72 RID: 57202
			public float FadeDuration;

			// Token: 0x0400DF73 RID: 57203
			public Color SnapshotColor;

			// Token: 0x0400DF74 RID: 57204
			public AdventurePointLight.ShapeType SnapshotShape;

			// Token: 0x0400DF75 RID: 57205
			public AdventurePointLight.LightMode SnapshotMode;

			// Token: 0x0400DF76 RID: 57206
			public float SnapshotRange;

			// Token: 0x0400DF77 RID: 57207
			public int SnapshotFullRange;

			// Token: 0x0400DF78 RID: 57208
			public float SnapshotAngle;

			// Token: 0x0400DF79 RID: 57209
			public float SnapshotVirtualZ;

			// Token: 0x0400DF7A RID: 57210
			public Vector2 SnapshotWorldPos;

			// Token: 0x0400DF7B RID: 57211
			public float SnapshotWorldScale;

			// Token: 0x0400DF7C RID: 57212
			public int SnapshotPriority;

			// Token: 0x0400DF7D RID: 57213
			public bool SnapshotNoRangeClamp;
		}
	}
}
