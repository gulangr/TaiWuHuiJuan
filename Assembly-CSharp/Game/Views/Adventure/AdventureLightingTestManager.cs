using System;
using GameData.Adventure;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C69 RID: 3177
	public class AdventureLightingTestManager : MonoBehaviour
	{
		// Token: 0x0600A1C6 RID: 41414 RVA: 0x004B9AC8 File Offset: 0x004B7CC8
		private void LateUpdate()
		{
			bool keyDown = Input.GetKeyDown(this.ToggleGUIKey);
			if (keyDown)
			{
				this.ShowGUI = !this.ShowGUI;
			}
			this.UpdateShaderData();
			this.UpdatePointLightIndicator();
		}

		// Token: 0x0600A1C7 RID: 41415 RVA: 0x004B9B02 File Offset: 0x004B7D02
		private void OnDisable()
		{
			Shader.SetGlobalColor(AdventureLightingTestManager.IdGlobalLightColor, new Color(0f, 0f, 0f, -1f));
			Shader.SetGlobalInt(AdventureLightingTestManager.IdPointLightCount, 0);
			Shader.SetGlobalInt(AdventureLightingTestManager.IdHighlightCount, 0);
		}

		// Token: 0x0600A1C8 RID: 41416 RVA: 0x004B9B41 File Offset: 0x004B7D41
		private void UpdateShaderData()
		{
			this.UpdateGlobalLightParams();
			this.UpdatePointLightParams();
			Shader.SetGlobalInt(AdventureLightingTestManager.IdHighlightCount, 0);
		}

		// Token: 0x0600A1C9 RID: 41417 RVA: 0x004B9B60 File Offset: 0x004B7D60
		private void UpdateGlobalLightParams()
		{
			AdventureLightingTestManager.TestMode mode = this.Mode;
			AdventureLightingTestManager.TestMode testMode = mode;
			if (testMode != AdventureLightingTestManager.TestMode.NormalLighting)
			{
				if (testMode == AdventureLightingTestManager.TestMode.DirectLightControl)
				{
					Vector3 dir = AdventureLightingTestManager.ComputeDirFromAngles(this.DirectIncidenceAngle, this.DirectAzimuthAngle);
					Shader.SetGlobalVector(AdventureLightingTestManager.IdGlobalLightDir, new Vector4(dir.x, dir.y, dir.z, 0f));
					Color col = this.DirectLightColor;
					col.a = this.DirectLightIntensity;
					Shader.SetGlobalColor(AdventureLightingTestManager.IdGlobalLightColor, col);
				}
			}
			else
			{
				float radPhi = this.GlobalIncidenceAngle * 0.017453292f;
				float radTheta = this.GlobalAzimuthAngle * 0.017453292f;
				float z = Mathf.Sin(radPhi);
				float xyMag = Mathf.Cos(radPhi);
				float x = Mathf.Sin(radTheta) * xyMag;
				float y = Mathf.Cos(radTheta) * xyMag;
				Vector3 dir2 = new Vector3(-x, -y, z);
				Shader.SetGlobalVector(AdventureLightingTestManager.IdGlobalLightDir, new Vector4(dir2.x, dir2.y, dir2.z, 0f));
				Color col2 = this.GlobalColor;
				col2.a = this.GlobalIntensity;
				Shader.SetGlobalColor(AdventureLightingTestManager.IdGlobalLightColor, col2);
			}
		}

		// Token: 0x0600A1CA RID: 41418 RVA: 0x004B9C88 File Offset: 0x004B7E88
		private void UpdatePointLightParams()
		{
			bool flag = this.Mode == AdventureLightingTestManager.TestMode.DirectLightControl;
			if (flag)
			{
				Shader.SetGlobalInt(AdventureLightingTestManager.IdPointLightCount, 0);
			}
			else
			{
				int count = 0;
				bool flag2 = this.PointLights != null;
				if (flag2)
				{
					foreach (AdventurePointLight light in this.PointLights)
					{
						bool flag3 = light == null || !light.isActiveAndEnabled;
						if (!flag3)
						{
							bool flag4 = count >= 64;
							if (flag4)
							{
								break;
							}
							this.FillLightData(count, light);
							count++;
						}
					}
				}
				Shader.SetGlobalVectorArray(AdventureLightingTestManager.IdPointLightPos, this._pointLightPos);
				Shader.SetGlobalVectorArray(AdventureLightingTestManager.IdPointLightWorldPos, this._pointLightWorldPos);
				Shader.SetGlobalVectorArray(AdventureLightingTestManager.IdPointLightColor, this._pointLightColor);
				Shader.SetGlobalVectorArray(AdventureLightingTestManager.IdPointLightParam, this._pointLightParam);
				Shader.SetGlobalInt(AdventureLightingTestManager.IdPointLightCount, count);
			}
		}

		// Token: 0x0600A1CB RID: 41419 RVA: 0x004B9D78 File Offset: 0x004B7F78
		private void FillLightData(int index, AdventurePointLight light)
		{
			float posZ = (light.Mode == AdventurePointLight.LightMode.Smooth) ? light.VirtualZ : (light.Angle * 0.017453292f);
			float mode = (float)light.Mode;
			this._pointLightPos[index] = new Vector4((float)light.BlockIndex.Gx, (float)light.BlockIndex.Gy, posZ, mode);
			Vector2 worldPos = light.transform.position;
			float worldScale = light.transform.lossyScale.x;
			this._pointLightWorldPos[index] = new Vector4(worldPos.x, worldPos.y, worldScale, 0f);
			Color color = light.LightColor;
			color.a = light.CurrentIntensity;
			this._pointLightColor[index] = color;
			this._pointLightParam[index] = new Vector4((float)light.Shape, light.Range, (float)light.FullIntensityRange, (float)light.Priority);
		}

		// Token: 0x0600A1CC RID: 41420 RVA: 0x004B9E78 File Offset: 0x004B8078
		private void UpdatePointLightIndicator()
		{
			bool flag = this.PointLightIndicator == null || this.PointLights == null || this.PointLights.Length == 0;
			if (!flag)
			{
				AdventurePointLight light = this.PointLights[0];
				bool flag2 = light == null;
				if (!flag2)
				{
					int gx = light.BlockIndex.Gx;
					int gy = light.BlockIndex.Gy;
					float x = (float)(gx + gy) * this.CellSize.x / 2f;
					float y = (float)(gy - gx) * this.CellSize.y / 2f;
					this.PointLightIndicator.anchoredPosition = new Vector2(x, y);
				}
			}
		}

		// Token: 0x0600A1CD RID: 41421 RVA: 0x004B9F2C File Offset: 0x004B812C
		private void OnGUI()
		{
			bool flag = !this.ShowGUI;
			if (!flag)
			{
				float panelWidth = 340f;
				float panelHeight = Mathf.Min((float)Screen.height - 20f, 900f);
				GUILayout.BeginArea(new Rect(10f, 10f, panelWidth, panelHeight));
				this._scrollPos = GUILayout.BeginScrollView(this._scrollPos, GUI.skin.box, new GUILayoutOption[]
				{
					GUILayout.Width(panelWidth),
					GUILayout.Height(panelHeight)
				});
				GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 14,
					fontStyle = FontStyle.Bold
				};
				GUILayout.Label("Adventure Lighting Test", titleStyle, Array.Empty<GUILayoutOption>());
				GUILayout.Label(string.Format("[{0}] toggle panel", this.ToggleGUIKey), new GUIStyle(GUI.skin.label)
				{
					fontSize = 10
				}, Array.Empty<GUILayoutOption>());
				GUILayout.Space(6f);
				GUILayout.Label("Test Mode:", GUI.skin.box, Array.Empty<GUILayoutOption>());
				this.Mode = (AdventureLightingTestManager.TestMode)GUILayout.SelectionGrid((int)this.Mode, new string[]
				{
					"Normal Lighting",
					"Direct Light Ctrl"
				}, 1, Array.Empty<GUILayoutOption>());
				GUILayout.Space(6f);
				AdventureLightingTestManager.TestMode mode = this.Mode;
				AdventureLightingTestManager.TestMode testMode = mode;
				if (testMode != AdventureLightingTestManager.TestMode.NormalLighting)
				{
					if (testMode == AdventureLightingTestManager.TestMode.DirectLightControl)
					{
						this.DrawDirectLightControlGUI();
					}
				}
				else
				{
					this.DrawGlobalLightGUI();
					GUILayout.Space(6f);
					this.DrawPointLightsGUI();
				}
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
		}

		// Token: 0x0600A1CE RID: 41422 RVA: 0x004BA0C4 File Offset: 0x004B82C4
		private void DrawGlobalLightGUI()
		{
			GUILayout.Label("=== Global Light ===", GUI.skin.box, Array.Empty<GUILayoutOption>());
			this.GlobalIncidenceAngle = this.LabeledSlider("Incidence Angle", this.GlobalIncidenceAngle, 0f, 90f, "{0:F1}°");
			this.GlobalAzimuthAngle = this.LabeledSlider("Azimuth Angle", this.GlobalAzimuthAngle, 0f, 360f, "{0:F1}°");
			this.GlobalIntensity = this.LabeledSlider("Intensity", this.GlobalIntensity, 0f, 5f, "{0:F2}");
			GUILayout.Space(4f);
			this.GlobalColor = this.ColorSliders("Light Color", this.GlobalColor);
			GUILayout.Space(4f);
			Vector3 computedDir = AdventureLightingTestManager.ComputeDirFromAngles(this.GlobalIncidenceAngle, this.GlobalAzimuthAngle);
			GUILayout.Label(string.Format("Dir: ({0:F2}, {1:F2}, {2:F2})", computedDir.x, computedDir.y, computedDir.z), new GUIStyle(GUI.skin.label)
			{
				fontSize = 10
			}, Array.Empty<GUILayoutOption>());
		}

		// Token: 0x0600A1CF RID: 41423 RVA: 0x004BA1EC File Offset: 0x004B83EC
		private void DrawDirectLightControlGUI()
		{
			GUILayout.Label("=== Direct Light Control ===", GUI.skin.box, Array.Empty<GUILayoutOption>());
			GUILayout.Label("Presets:", Array.Empty<GUILayoutOption>());
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag = this.PresetBtn("NW 315");
			if (flag)
			{
				this.DirectAzimuthAngle = 315f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			bool flag2 = this.PresetBtn("N   0");
			if (flag2)
			{
				this.DirectAzimuthAngle = 0f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			bool flag3 = this.PresetBtn("NE 45");
			if (flag3)
			{
				this.DirectAzimuthAngle = 45f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag4 = this.PresetBtn("W  270");
			if (flag4)
			{
				this.DirectAzimuthAngle = 270f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			bool flag5 = this.PresetBtn("TOP 90");
			if (flag5)
			{
				this.DirectIncidenceAngle = 90f;
			}
			bool flag6 = this.PresetBtn("E   90");
			if (flag6)
			{
				this.DirectAzimuthAngle = 90f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag7 = this.PresetBtn("SW 225");
			if (flag7)
			{
				this.DirectAzimuthAngle = 225f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			bool flag8 = this.PresetBtn("S  180");
			if (flag8)
			{
				this.DirectAzimuthAngle = 180f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			bool flag9 = this.PresetBtn("SE 135");
			if (flag9)
			{
				this.DirectAzimuthAngle = 135f;
				this.DirectIncidenceAngle = this.PresetIncidenceAngle;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.Space(4f);
			this.PresetIncidenceAngle = this.LabeledSlider("Preset Incidence", this.PresetIncidenceAngle, 0f, 90f, "{0:F0}°");
			GUILayout.Space(6f);
			this.DirectIncidenceAngle = this.LabeledSlider("Incidence Angle", this.DirectIncidenceAngle, 0f, 90f, "{0:F1}°");
			this.DirectAzimuthAngle = this.LabeledSlider("Azimuth Angle", this.DirectAzimuthAngle, 0f, 360f, "{0:F1}°");
			GUILayout.Space(4f);
			this.DirectLightIntensity = this.LabeledSlider("Intensity", this.DirectLightIntensity, 0f, 5f, "{0:F2}");
			this.DirectLightColor = this.ColorSliders("Color", this.DirectLightColor);
			GUILayout.Space(4f);
			Vector3 dir = AdventureLightingTestManager.ComputeDirFromAngles(this.DirectIncidenceAngle, this.DirectAzimuthAngle);
			GUILayout.Label(string.Format("Dir: ({0:F3}, {1:F3}, {2:F3})", dir.x, dir.y, dir.z), new GUIStyle(GUI.skin.label)
			{
				fontSize = 10
			}, Array.Empty<GUILayoutOption>());
		}

		// Token: 0x0600A1D0 RID: 41424 RVA: 0x004BA50C File Offset: 0x004B870C
		private bool PresetBtn(string label)
		{
			return GUILayout.Button(label, new GUILayoutOption[]
			{
				GUILayout.Width(100f),
				GUILayout.Height(25f)
			});
		}

		// Token: 0x0600A1D1 RID: 41425 RVA: 0x004BA544 File Offset: 0x004B8744
		private void DrawPointLightsGUI()
		{
			GUILayout.Label("=== Point Lights ===", GUI.skin.box, Array.Empty<GUILayoutOption>());
			bool flag = this.PointLights == null || this.PointLights.Length == 0;
			if (flag)
			{
				GUILayout.Label("No point lights assigned in inspector.", Array.Empty<GUILayoutOption>());
			}
			else
			{
				string[] names = new string[this.PointLights.Length];
				for (int i = 0; i < this.PointLights.Length; i++)
				{
					names[i] = ((this.PointLights[i] != null) ? this.PointLights[i].name : "(null)");
				}
				this._selectedPointLightIndex = GUILayout.SelectionGrid(Mathf.Clamp(this._selectedPointLightIndex, 0, this.PointLights.Length - 1), names, 2, Array.Empty<GUILayoutOption>());
				GUILayout.Space(4f);
				AdventurePointLight light = this.PointLights[this._selectedPointLightIndex];
				bool flag2 = light == null;
				if (flag2)
				{
					GUILayout.Label("Selected light is null.", Array.Empty<GUILayoutOption>());
				}
				else
				{
					GUILayout.Label("Editing: " + light.name, new GUIStyle(GUI.skin.label)
					{
						fontStyle = FontStyle.Bold
					}, Array.Empty<GUILayoutOption>());
					GUILayout.Space(4f);
					GUILayout.Label("Grid Position:", Array.Empty<GUILayoutOption>());
					int gx = this.IntIncrementField("X", light.BlockIndex.Gx);
					int gy = this.IntIncrementField("Y", light.BlockIndex.Gy);
					light.BlockIndex = new AdventureBlockIndex(gx, gy);
					GUILayout.Space(4f);
					light.Angle = this.LabeledSlider("Angle", light.Angle, 0f, 90f, "{0:F1}°");
					light.VirtualZ = this.LabeledSlider("Virtual Z", light.VirtualZ, 0f, 5f, "{0:F2}");
					light.Range = this.LabeledSlider("Range", light.Range, 0f, 10f, "{0:F1}");
					light.FullIntensityRange = Mathf.RoundToInt(this.LabeledSlider("Full Range", (float)light.FullIntensityRange, 0f, 10f, "{0:F0}"));
					light.Intensity = this.LabeledSlider("Intensity", light.Intensity, 0f, 5f, "{0:F2}");
					light.Priority = Mathf.RoundToInt(this.LabeledSlider("Priority", (float)light.Priority, 0f, 16f, "{0:F0}"));
					GUILayout.Space(4f);
					GUILayout.Label("Mode:", Array.Empty<GUILayoutOption>());
					light.Mode = (AdventurePointLight.LightMode)GUILayout.SelectionGrid((int)light.Mode, new string[]
					{
						"Uniform",
						"Smooth"
					}, 2, Array.Empty<GUILayoutOption>());
					GUILayout.Label("Shape:", Array.Empty<GUILayoutOption>());
					light.Shape = GUILayout.SelectionGrid(light.Shape - AdventurePointLight.ShapeType.OneByOne, new string[]
					{
						"1x1",
						"3x3"
					}, 2, Array.Empty<GUILayoutOption>()) + AdventurePointLight.ShapeType.OneByOne;
					GUILayout.Space(4f);
					light.LightColor = this.ColorSliders("Color", light.LightColor);
					GUILayout.Space(4f);
					GUILayout.Label("Breathing:", Array.Empty<GUILayoutOption>());
					light.BreathingPeriod = this.LabeledSlider("Period", light.BreathingPeriod, 0f, 10f, "{0:F1}s");
					light.MinIntensity = this.LabeledSlider("Min Intensity", light.MinIntensity, 0f, 2f, "{0:F2}");
				}
			}
		}

		// Token: 0x0600A1D2 RID: 41426 RVA: 0x004BA8F4 File Offset: 0x004B8AF4
		private int IntIncrementField(string label, int value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(20f)
			});
			bool flag = GUILayout.Button("-1", new GUILayoutOption[]
			{
				GUILayout.Width(40f)
			});
			if (flag)
			{
				value--;
			}
			string input = GUILayout.TextField(value.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			});
			int parsed;
			bool flag2 = int.TryParse(input, out parsed);
			if (flag2)
			{
				value = parsed;
			}
			bool flag3 = GUILayout.Button("+1", new GUILayoutOption[]
			{
				GUILayout.Width(40f)
			});
			if (flag3)
			{
				value++;
			}
			GUILayout.EndHorizontal();
			return value;
		}

		// Token: 0x0600A1D3 RID: 41427 RVA: 0x004BA9B4 File Offset: 0x004B8BB4
		private float LabeledSlider(string label, float value, float min, float max, string format)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			value = GUILayout.HorizontalSlider(value, min, max, Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format(format, value), new GUILayoutOption[]
			{
				GUILayout.Width(55f)
			});
			GUILayout.EndHorizontal();
			return value;
		}

		// Token: 0x0600A1D4 RID: 41428 RVA: 0x004BAA28 File Offset: 0x004B8C28
		private Color ColorSliders(string label, Color color)
		{
			GUILayout.Label(label, Array.Empty<GUILayoutOption>());
			color.r = this.LabeledSlider("  R", color.r, 0f, 1f, "{0:F2}");
			color.g = this.LabeledSlider("  G", color.g, 0f, 1f, "{0:F2}");
			color.b = this.LabeledSlider("  B", color.b, 0f, 1f, "{0:F2}");
			return color;
		}

		// Token: 0x0600A1D5 RID: 41429 RVA: 0x004BAABC File Offset: 0x004B8CBC
		private static Vector3 ComputeDirFromAngles(float incidence, float azimuth)
		{
			float radPhi = incidence * 0.017453292f;
			float radTheta = azimuth * 0.017453292f;
			float z = Mathf.Sin(radPhi);
			float xyMag = Mathf.Cos(radPhi);
			float x = Mathf.Sin(radTheta) * xyMag;
			float y = Mathf.Cos(radTheta) * xyMag;
			return new Vector3(-x, -y, z);
		}

		// Token: 0x04007DA7 RID: 32167
		[Header("Test Mode")]
		public AdventureLightingTestManager.TestMode Mode = AdventureLightingTestManager.TestMode.NormalLighting;

		// Token: 0x04007DA8 RID: 32168
		[Header("Global Light")]
		[Range(0f, 90f)]
		[Tooltip("入射角 (0-90)。90度垂直于屏幕，0度平行。")]
		public float GlobalIncidenceAngle = 60f;

		// Token: 0x04007DA9 RID: 32169
		[Range(0f, 360f)]
		[Tooltip("方位角 (0-360)。0度上方，顺时针。")]
		public float GlobalAzimuthAngle = 0f;

		// Token: 0x04007DAA RID: 32170
		[Range(0f, 5f)]
		public float GlobalIntensity = 1f;

		// Token: 0x04007DAB RID: 32171
		public Color GlobalColor = Color.white;

		// Token: 0x04007DAC RID: 32172
		[Header("Direct Light Control (DirectLightControl mode)")]
		[Range(0f, 90f)]
		[Tooltip("入射角。90度垂直于屏幕，0度平行。")]
		public float DirectIncidenceAngle = 60f;

		// Token: 0x04007DAD RID: 32173
		[Range(0f, 360f)]
		[Tooltip("方位角。0度上方，顺时针。")]
		public float DirectAzimuthAngle = 0f;

		// Token: 0x04007DAE RID: 32174
		[Range(0f, 5f)]
		public float DirectLightIntensity = 1f;

		// Token: 0x04007DAF RID: 32175
		public Color DirectLightColor = Color.white;

		// Token: 0x04007DB0 RID: 32176
		[Tooltip("预设按钮使用的入射角")]
		public float PresetIncidenceAngle = 45f;

		// Token: 0x04007DB1 RID: 32177
		[Header("Point Lights")]
		[Tooltip("直接引用场景中的点光源组件")]
		public AdventurePointLight[] PointLights;

		// Token: 0x04007DB2 RID: 32178
		[Header("Point Light Indicator")]
		[Tooltip("点光源位置指示器节点，放在Block下作为子节点")]
		public RectTransform PointLightIndicator;

		// Token: 0x04007DB3 RID: 32179
		[Tooltip("一个格子对应的UI尺寸（宽=水平间距，高=垂直间距），用于等距坐标变换")]
		public Vector2 CellSize = new Vector2(640f, 332f);

		// Token: 0x04007DB4 RID: 32180
		[Header("GUI")]
		public bool ShowGUI = true;

		// Token: 0x04007DB5 RID: 32181
		public KeyCode ToggleGUIKey = KeyCode.F1;

		// Token: 0x04007DB6 RID: 32182
		private const int MAX_LIGHT_COUNT = 64;

		// Token: 0x04007DB7 RID: 32183
		private static readonly int IdGlobalLightDir = Shader.PropertyToID("_GlobalLightDir");

		// Token: 0x04007DB8 RID: 32184
		private static readonly int IdGlobalLightColor = Shader.PropertyToID("_GlobalLightColor");

		// Token: 0x04007DB9 RID: 32185
		private static readonly int IdPointLightPos = Shader.PropertyToID("_AdvPointLightPos");

		// Token: 0x04007DBA RID: 32186
		private static readonly int IdPointLightWorldPos = Shader.PropertyToID("_AdvPointLightWorldPos");

		// Token: 0x04007DBB RID: 32187
		private static readonly int IdPointLightColor = Shader.PropertyToID("_AdvPointLightColor");

		// Token: 0x04007DBC RID: 32188
		private static readonly int IdPointLightParam = Shader.PropertyToID("_AdvPointLightParam");

		// Token: 0x04007DBD RID: 32189
		private static readonly int IdPointLightCount = Shader.PropertyToID("_AdvPointLightCount");

		// Token: 0x04007DBE RID: 32190
		private static readonly int IdHighlightCount = Shader.PropertyToID("_AdvHighlightCount");

		// Token: 0x04007DBF RID: 32191
		private readonly Vector4[] _pointLightPos = new Vector4[64];

		// Token: 0x04007DC0 RID: 32192
		private readonly Vector4[] _pointLightWorldPos = new Vector4[64];

		// Token: 0x04007DC1 RID: 32193
		private readonly Vector4[] _pointLightColor = new Vector4[64];

		// Token: 0x04007DC2 RID: 32194
		private readonly Vector4[] _pointLightParam = new Vector4[64];

		// Token: 0x04007DC3 RID: 32195
		private Vector2 _scrollPos;

		// Token: 0x04007DC4 RID: 32196
		private int _selectedPointLightIndex;

		// Token: 0x020023A2 RID: 9122
		public enum TestMode
		{
			// Token: 0x0400DF80 RID: 57216
			NormalLighting,
			// Token: 0x0400DF81 RID: 57217
			DirectLightControl
		}
	}
}
