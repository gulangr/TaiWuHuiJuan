using System;
using UnityEngine;

namespace FrameWork.Camera
{
	// Token: 0x02001075 RID: 4213
	[RequireComponent(typeof(Camera))]
	public class CameraQuadRenderer : MonoBehaviour, UIGrabGraphic.IRtReceiver
	{
		// Token: 0x0600BF36 RID: 48950 RVA: 0x0056748C File Offset: 0x0056568C
		private static bool IsFinite(float value)
		{
			return !float.IsNaN(value) && !float.IsInfinity(value);
		}

		// Token: 0x1700157F RID: 5503
		// (get) Token: 0x0600BF37 RID: 48951 RVA: 0x005674B2 File Offset: 0x005656B2
		// (set) Token: 0x0600BF38 RID: 48952 RVA: 0x005674BA File Offset: 0x005656BA
		public RenderTexture RenderTexture
		{
			get
			{
				return this.renderTexture;
			}
			set
			{
				this.renderTexture = value;
				this.UpdateQuadTexture();
			}
		}

		// Token: 0x17001580 RID: 5504
		// (get) Token: 0x0600BF39 RID: 48953 RVA: 0x005674CB File Offset: 0x005656CB
		// (set) Token: 0x0600BF3A RID: 48954 RVA: 0x005674D4 File Offset: 0x005656D4
		public int QuadLayer
		{
			get
			{
				return this.quadLayer;
			}
			set
			{
				this.quadLayer = value;
				bool flag = this._quad != null;
				if (flag)
				{
					this._quad.layer = this.quadLayer;
				}
			}
		}

		// Token: 0x17001581 RID: 5505
		// (get) Token: 0x0600BF3B RID: 48955 RVA: 0x0056750D File Offset: 0x0056570D
		// (set) Token: 0x0600BF3C RID: 48956 RVA: 0x00567518 File Offset: 0x00565718
		public bool EnableAspectRatioCompensation
		{
			get
			{
				return this.enableAspectRatioCompensation;
			}
			set
			{
				this.enableAspectRatioCompensation = value;
				bool flag = this._quad != null;
				if (flag)
				{
					this.UpdateQuadTransform();
				}
			}
		}

		// Token: 0x17001582 RID: 5506
		// (get) Token: 0x0600BF3D RID: 48957 RVA: 0x00567546 File Offset: 0x00565746
		// (set) Token: 0x0600BF3E RID: 48958 RVA: 0x00567550 File Offset: 0x00565750
		public float ReferenceAspectRatio
		{
			get
			{
				return this.referenceAspectRatio;
			}
			set
			{
				this.referenceAspectRatio = value;
				bool flag = this._quad != null;
				if (flag)
				{
					this.UpdateQuadTransform();
				}
			}
		}

		// Token: 0x17001583 RID: 5507
		// (get) Token: 0x0600BF3F RID: 48959 RVA: 0x0056757E File Offset: 0x0056577E
		// (set) Token: 0x0600BF40 RID: 48960 RVA: 0x00567588 File Offset: 0x00565788
		public bool EnableTextureTransform
		{
			get
			{
				return this.enableTextureTransform;
			}
			set
			{
				this.enableTextureTransform = value;
				bool flag = this._quad != null;
				if (flag)
				{
					this.UpdateQuadTexture();
				}
			}
		}

		// Token: 0x0600BF41 RID: 48961 RVA: 0x005675B8 File Offset: 0x005657B8
		private void Awake()
		{
			this._camera = base.GetComponent<Camera>();
			bool flag = !this._camera.orthographic;
			if (flag)
			{
				Debug.LogWarning("Camera " + base.name + " is not orthographic. This script is designed for orthographic cameras only.");
			}
			this.UpdateScreenResolutionTracking();
			bool flag2 = this.createQuadOnAwake;
			if (flag2)
			{
				this.CreateQuad();
			}
		}

		// Token: 0x0600BF42 RID: 48962 RVA: 0x0056761B File Offset: 0x0056581B
		private void Start()
		{
			this.UpdateQuadTexture();
		}

		// Token: 0x0600BF43 RID: 48963 RVA: 0x00567628 File Offset: 0x00565828
		public void CreateQuad()
		{
			bool flag = this._quad != null;
			if (flag)
			{
				this.DestroyQuad();
			}
			this._quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			this._quad.name = base.gameObject.name + "_RenderQuad";
			this._quad.transform.SetParent(base.transform);
			Collider quadCollider = this._quad.GetComponent<Collider>();
			bool flag2 = quadCollider != null;
			if (flag2)
			{
				Object.DestroyImmediate(quadCollider);
			}
			this._quadRenderer = this._quad.GetComponent<MeshRenderer>();
			this._quad.layer = this.quadLayer;
			this.SetupMaterial();
			this.UpdateQuadTransform();
		}

		// Token: 0x0600BF44 RID: 48964 RVA: 0x005676E4 File Offset: 0x005658E4
		private void SetupMaterial()
		{
			bool flag = this.quadMaterial == null;
			if (flag)
			{
				Shader shader = Shader.Find("Unlit/Texture");
				bool flag2 = shader == null;
				if (flag2)
				{
					Debug.LogError("Shader 'Unlit/Texture' not found after build. It may have been stripped. Please check Graphics Settings -> Always Included Shaders.");
					return;
				}
				this.quadMaterial = new Material(shader)
				{
					name = "QuadRenderMaterial"
				};
			}
			this._quadRenderer.material = this.quadMaterial;
		}

		// Token: 0x0600BF45 RID: 48965 RVA: 0x00567752 File Offset: 0x00565952
		private void UpdateScreenResolutionTracking()
		{
			this._lastScreenWidth = Screen.width;
			this._lastScreenHeight = Screen.height;
			this._lastScreenAspectRatio = (float)Screen.width / (float)Screen.height;
		}

		// Token: 0x0600BF46 RID: 48966 RVA: 0x00567780 File Offset: 0x00565980
		private bool HasScreenResolutionChanged()
		{
			return this._lastScreenWidth != Screen.width || this._lastScreenHeight != Screen.height;
		}

		// Token: 0x0600BF47 RID: 48967 RVA: 0x005677B4 File Offset: 0x005659B4
		private Vector2 GetAspectRatioCompensationScale()
		{
			bool flag = !this.enableAspectRatioCompensation;
			Vector2 result;
			if (flag)
			{
				result = Vector2.one;
			}
			else
			{
				float currentAspectRatio = (float)Screen.width / (float)Screen.height;
				float aspectRatioDifference = currentAspectRatio / this.referenceAspectRatio;
				Vector2 compensation = Vector2.one;
				bool flag2 = currentAspectRatio > this.referenceAspectRatio;
				if (flag2)
				{
					compensation.x = aspectRatioDifference;
					compensation.y = 1f;
				}
				else
				{
					bool flag3 = currentAspectRatio < this.referenceAspectRatio;
					if (flag3)
					{
						compensation.x = 1f;
						compensation.y = 1f / aspectRatioDifference;
					}
					else
					{
						compensation = Vector2.one;
					}
				}
				result = compensation;
			}
			return result;
		}

		// Token: 0x0600BF48 RID: 48968 RVA: 0x0056785C File Offset: 0x00565A5C
		private void UpdateQuadTransform()
		{
			bool flag = this._quad == null || this._camera == null;
			if (!flag)
			{
				bool flag2 = !this._camera.enabled;
				if (!flag2)
				{
					bool flag3 = this._camera.pixelWidth <= 0 || this._camera.pixelHeight <= 0;
					if (!flag3)
					{
						float nearPlane = this._camera.nearClipPlane;
						float farPlane = this._camera.farClipPlane;
						float planeDiff = farPlane - nearPlane;
						bool flag4 = !CameraQuadRenderer.IsFinite(nearPlane) || !CameraQuadRenderer.IsFinite(farPlane) || planeDiff <= 0f;
						if (!flag4)
						{
							bool flag5 = planeDiff < 0.01f;
							float farPlaneDistance;
							if (flag5)
							{
								farPlaneDistance = nearPlane + planeDiff * 0.5f;
							}
							else
							{
								farPlaneDistance = Mathf.Clamp(farPlane - 0.01f, nearPlane + 1E-05f, farPlane);
							}
							bool flag6 = !CameraQuadRenderer.IsFinite(farPlaneDistance);
							if (!flag6)
							{
								this._quad.transform.position = this._camera.transform.position + this._camera.transform.forward * farPlaneDistance;
								float zForScreenToWorld = farPlaneDistance;
								float zMax = farPlane - 1E-05f;
								bool flag7 = zMax > nearPlane + 1E-05f;
								if (flag7)
								{
									zForScreenToWorld = Mathf.Clamp(zForScreenToWorld, nearPlane + 1E-05f, zMax);
								}
								Vector3 bottomLeft = this._camera.ScreenToWorldPoint(new Vector3(0f, 0f, zForScreenToWorld));
								Vector3 topRight = this._camera.ScreenToWorldPoint(new Vector3((float)this._camera.pixelWidth, (float)this._camera.pixelHeight, zForScreenToWorld));
								bottomLeft = base.transform.InverseTransformPoint(bottomLeft);
								topRight = base.transform.InverseTransformPoint(topRight);
								float baseQuadWidth = topRight.x - bottomLeft.x;
								float baseQuadHeight = topRight.y - bottomLeft.y;
								float quadWidth = baseQuadWidth;
								float quadHeight = baseQuadHeight;
								bool flag8 = this.enableAspectRatioCompensation;
								if (flag8)
								{
									Vector2 aspectCompensation = this.GetAspectRatioCompensationScale();
									float currentAspectRatio = (float)Screen.width / (float)Screen.height;
									bool flag9 = currentAspectRatio > this.referenceAspectRatio;
									if (flag9)
									{
										quadWidth *= aspectCompensation.x;
									}
									else
									{
										bool flag10 = currentAspectRatio < this.referenceAspectRatio;
										if (flag10)
										{
											quadHeight *= aspectCompensation.y;
										}
									}
								}
								this._quad.transform.localScale = new Vector3(quadWidth, quadHeight, 1f);
								this._quad.transform.localRotation = Quaternion.identity;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600BF49 RID: 48969 RVA: 0x00567AFC File Offset: 0x00565CFC
		private void UpdateQuadTexture()
		{
			bool flag = this._quadRenderer != null && this.quadMaterial != null && this.renderTexture != null;
			if (flag)
			{
				this.quadMaterial.mainTexture = this.renderTexture;
				bool flag2 = this.enableAspectRatioCompensation;
				if (flag2)
				{
					this.UpdateTextureTransform();
				}
				else
				{
					this.quadMaterial.mainTextureScale = Vector2.one;
					this.quadMaterial.mainTextureOffset = Vector2.zero;
				}
			}
		}

		// Token: 0x0600BF4A RID: 48970 RVA: 0x00567B88 File Offset: 0x00565D88
		private void UpdateTextureTransform()
		{
			bool flag = this.quadMaterial == null;
			if (!flag)
			{
				bool flag2 = !this.enableTextureTransform;
				if (flag2)
				{
					this.quadMaterial.mainTextureScale = Vector2.one;
					this.quadMaterial.mainTextureOffset = Vector2.zero;
				}
				else
				{
					this.quadMaterial.mainTextureScale = Vector2.one;
					this.quadMaterial.mainTextureOffset = Vector2.zero;
				}
			}
		}

		// Token: 0x0600BF4B RID: 48971 RVA: 0x00567BFC File Offset: 0x00565DFC
		public void SetReferenceResolution(int width, int height)
		{
			this.referenceAspectRatio = (float)width / (float)height;
			bool flag = this._quad != null;
			if (flag)
			{
				this.UpdateQuadTransform();
			}
		}

		// Token: 0x0600BF4C RID: 48972 RVA: 0x00567C30 File Offset: 0x00565E30
		public void DestroyQuad()
		{
			bool flag = this._quad != null;
			if (flag)
			{
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					Object.Destroy(this._quad);
				}
				else
				{
					Object.DestroyImmediate(this._quad);
				}
				this._quad = null;
				this._quadRenderer = null;
			}
		}

		// Token: 0x0600BF4D RID: 48973 RVA: 0x00567C88 File Offset: 0x00565E88
		private void Update()
		{
			bool flag = this._camera == null;
			if (flag)
			{
				this._camera = base.GetComponent<Camera>();
			}
			bool flag2 = Application.isPlaying && this._quad != null;
			if (flag2)
			{
				bool flag3 = this._camera == null || !this._camera.enabled;
				if (!flag3)
				{
					bool flag4 = this.HasScreenResolutionChanged();
					if (flag4)
					{
						this.UpdateScreenResolutionTracking();
					}
					this.UpdateQuadTransform();
					this.UpdateQuadTexture();
				}
			}
		}

		// Token: 0x0600BF4E RID: 48974 RVA: 0x00567D14 File Offset: 0x00565F14
		private void OnDestroy()
		{
			this.DestroyQuad();
			bool flag = this.quadMaterial != null && this.quadMaterial.name == "QuadRenderMaterial";
			if (flag)
			{
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					Object.Destroy(this.quadMaterial);
				}
				else
				{
					Object.DestroyImmediate(this.quadMaterial);
				}
			}
		}

		// Token: 0x0600BF4F RID: 48975 RVA: 0x00567D7C File Offset: 0x00565F7C
		public void SetRenderTexture(RenderTexture rt)
		{
			this.renderTexture = rt;
		}

		// Token: 0x04009285 RID: 37509
		[Header("Render Settings")]
		[SerializeField]
		private RenderTexture renderTexture;

		// Token: 0x04009286 RID: 37510
		[Header("Quad Settings")]
		[SerializeField]
		private Material quadMaterial;

		// Token: 0x04009287 RID: 37511
		[SerializeField]
		private bool createQuadOnAwake = true;

		// Token: 0x04009288 RID: 37512
		[SerializeField]
		private int quadLayer;

		// Token: 0x04009289 RID: 37513
		[Header("Aspect Ratio Compensation")]
		[SerializeField]
		private bool enableAspectRatioCompensation = true;

		// Token: 0x0400928A RID: 37514
		[SerializeField]
		private float referenceAspectRatio = 1.7777778f;

		// Token: 0x0400928B RID: 37515
		[SerializeField]
		private bool enableTextureTransform = true;

		// Token: 0x0400928C RID: 37516
		private Camera _camera;

		// Token: 0x0400928D RID: 37517
		private GameObject _quad;

		// Token: 0x0400928E RID: 37518
		private MeshRenderer _quadRenderer;

		// Token: 0x0400928F RID: 37519
		private float _lastScreenAspectRatio;

		// Token: 0x04009290 RID: 37520
		private int _lastScreenWidth;

		// Token: 0x04009291 RID: 37521
		private int _lastScreenHeight;
	}
}
