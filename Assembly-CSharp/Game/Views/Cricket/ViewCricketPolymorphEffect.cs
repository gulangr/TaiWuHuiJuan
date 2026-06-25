using System;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Cricket
{
	// Token: 0x02000ABE RID: 2750
	public class ViewCricketPolymorphEffect : UIBase
	{
		// Token: 0x0600876D RID: 34669 RVA: 0x003EF9A0 File Offset: 0x003EDBA0
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("CricketItemId", out this._cricketItemId);
			int colorId = 0;
			argsBox.Get("ColorId", out colorId);
			this._colorId = (short)colorId;
			int partId = 0;
			argsBox.Get("PartId", out partId);
			this._partId = (short)partId;
			argsBox.Get("CharId", out this._charId);
			this.SetupRenderTarget();
			this.cricket.SetCricketData(this._colorId, this._partId, false, null, false);
			this.FetchCharacterData();
		}

		// Token: 0x0600876E RID: 34670 RVA: 0x003EFA2C File Offset: 0x003EDC2C
		private void SetupRenderTarget()
		{
			this._avatarRestored = false;
			this._avatarOriginalParent = null;
			this._rt = new RenderTexture(970, 1062, 24, RenderTextureFormat.ARGB32);
			this._rt.Create();
			this._processedRt = new RenderTexture(970, 1062, 0, RenderTextureFormat.ARGB32);
			this._processedRt.Create();
			int uiLayer = 5;
			this._cameraObj = new GameObject("CricketPolymorphEffectCamera");
			this._cameraObj.transform.position = new Vector3(0f, 0f, -10f);
			this._renderCamera = this._cameraObj.AddComponent<Camera>();
			this._renderCamera.orthographic = true;
			this._renderCamera.orthographicSize = 526f;
			this._renderCamera.clearFlags = CameraClearFlags.Color;
			this._renderCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			this._renderCamera.nearClipPlane = 0.3f;
			this._renderCamera.farClipPlane = 500f;
			this._renderCamera.targetTexture = this._rt;
			this._renderCamera.cullingMask = 1 << uiLayer;
			this._renderCamera.depth = 10f;
			this._cameraObj.layer = uiLayer;
			GameObject canvasObj = new GameObject("CricketPolymorphEffectCanvas", new Type[]
			{
				typeof(Canvas)
			});
			canvasObj.layer = uiLayer;
			canvasObj.transform.SetParent(this._cameraObj.transform, false);
			this._renderCanvas = canvasObj.GetComponent<Canvas>();
			this._renderCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			this._renderCanvas.worldCamera = this._renderCamera;
			this._renderCanvas.planeDistance = 250f;
			this._renderCanvas.sortingOrder = 100;
			CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(960f, 1052f);
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
			this._avatarOriginalParent = this.avatarNode.parent;
			RectTransform avatarRt = this.avatarNode;
			bool flag = avatarRt != null;
			if (flag)
			{
				this._originalAnchorMin = avatarRt.anchorMin;
				this._originalAnchorMax = avatarRt.anchorMax;
				this._originalScale = avatarRt.localScale;
				avatarRt.anchorMin = new Vector2(0.5f, 0.5f);
				avatarRt.anchorMax = new Vector2(0.5f, 0.5f);
				avatarRt.anchoredPosition = Vector2.zero;
				avatarRt.localScale = Vector3.one;
			}
			this.avatarNode.SetParent(this._renderCanvas.transform, false);
			this.avatarNode.gameObject.SetActive(true);
		}

		// Token: 0x0600876F RID: 34671 RVA: 0x003EFCF6 File Offset: 0x003EDEF6
		private void FetchCharacterData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, this._charId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData data = new CharacterDisplayData();
				Serializer.Deserialize(pool, offset, ref data);
				this._charDisplayData = data;
				this.avatar.Refresh(this._charDisplayData, false);
				bool isTaiwu = this._charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				this.nameText.text = NameCenter.GetMonasticTitleOrDisplayName(this._charDisplayData, isTaiwu);
				this.StartPerformance();
			});
		}

		// Token: 0x06008770 RID: 34672 RVA: 0x003EFD14 File Offset: 0x003EDF14
		private void StartPerformance()
		{
			this._sequence = DOTween.Sequence();
			this._sequence.AppendInterval(0.1f);
			this._sequence.AppendCallback(delegate
			{
				AudioManager.Instance.PlaySound("CCricket_Human", false, false);
			});
			this._sequence.AppendInterval(0.9f);
			this._sequence.AppendCallback(delegate
			{
				this.AssignRTToParticles();
				bool flag = this.renaddParticle != null;
				if (flag)
				{
					this.renaddParticle.gameObject.SetActive(true);
				}
				bool flag2 = this.renParticle != null;
				if (flag2)
				{
					this.renParticle.gameObject.SetActive(true);
				}
				this.particle.gameObject.SetActive(true);
				this.particle.Play();
				this._particlesActive = true;
			});
			this._sequence.Insert(2.2f, this.cricket.skeletonGraphic.DOFade(0f, 0.3f));
			this._sequence.AppendCallback(delegate
			{
				this.nameArea.gameObject.SetActive(true);
			});
			float restoreTime = Mathf.Min(this.avatarRestoreTime, 1.8f);
			this._sequence.AppendInterval(restoreTime);
			this._sequence.AppendCallback(delegate
			{
				bool flag = this.renaddParticle != null;
				if (flag)
				{
					this.renaddParticle.Stop();
					this.renaddParticle.gameObject.SetActive(false);
				}
				bool flag2 = this.renParticle != null;
				if (flag2)
				{
					this.renParticle.Stop();
					this.renParticle.gameObject.SetActive(false);
				}
				this._particlesActive = false;
				this.RestoreAvatarToUI();
			});
			this._sequence.AppendInterval(1.8f - restoreTime);
			this._sequence.AppendCallback(delegate
			{
				this.particle.Stop();
				this.particle.gameObject.SetActive(false);
			});
			this._sequence.AppendInterval(2f);
			this._sequence.OnComplete(delegate
			{
				this._sequence = null;
				this.nameArea.gameObject.SetActive(false);
				bool flag = this.avatarNode != null;
				if (flag)
				{
					this.avatarNode.gameObject.SetActive(false);
				}
				TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("CricketPolymorphEffectOver", "Completed", true);
				TaiwuEventDomainMethod.Call.TriggerListener("CricketPolymorphEffectOver", true);
				this.QuickHide();
			});
		}

		// Token: 0x06008771 RID: 34673 RVA: 0x003EFE60 File Offset: 0x003EE060
		private void RestoreAvatarToUI()
		{
			bool flag = this._avatarRestored || this._avatarOriginalParent == null || this.avatarNode == null;
			if (!flag)
			{
				this.avatarNode.SetParent(this._avatarOriginalParent, false);
				bool flag2 = this.particle != null;
				if (flag2)
				{
					this.avatarNode.SetSiblingIndex(Mathf.Max(0, this.particle.transform.GetSiblingIndex() - 1));
				}
				RectTransform avatarRt = this.avatarNode;
				bool flag3 = avatarRt != null;
				if (flag3)
				{
					avatarRt.anchorMin = this._originalAnchorMin;
					avatarRt.anchorMax = this._originalAnchorMax;
					avatarRt.anchoredPosition = this.avatarOriginPosition + this.avatarRestoreOffset;
					avatarRt.localScale = this._originalScale;
				}
				this.avatarNode.gameObject.SetActive(true);
				this._avatarOriginalParent = null;
				this._avatarRestored = true;
			}
		}

		// Token: 0x06008772 RID: 34674 RVA: 0x003EFF54 File Offset: 0x003EE154
		private void AssignRTToParticles()
		{
			bool flag = this._rt == null;
			if (!flag)
			{
				bool flag2 = this._binaryAlphaMat == null;
				if (flag2)
				{
					Shader shader = Shader.Find("Hidden/BinaryAlphaRT");
					bool flag3 = shader != null;
					if (flag3)
					{
						this._binaryAlphaMat = new Material(shader);
					}
				}
				bool flag4 = this._binaryAlphaMat != null && this._processedRt != null;
				if (flag4)
				{
					Graphics.Blit(this._rt, this._processedRt, this._binaryAlphaMat);
				}
				RenderTexture targetRt = (this._binaryAlphaMat != null) ? this._processedRt : this._rt;
				bool flag5 = this.renaddParticle != null;
				if (flag5)
				{
					ParticleSystemRenderer renderer = this.renaddParticle.GetComponent<ParticleSystemRenderer>();
					bool flag6 = renderer != null;
					if (flag6)
					{
						renderer.material.mainTexture = targetRt;
					}
				}
				bool flag7 = this.renParticle != null;
				if (flag7)
				{
					ParticleSystemRenderer renderer2 = this.renParticle.GetComponent<ParticleSystemRenderer>();
					bool flag8 = renderer2 != null;
					if (flag8)
					{
						renderer2.material.mainTexture = targetRt;
					}
				}
			}
		}

		// Token: 0x06008773 RID: 34675 RVA: 0x003F0084 File Offset: 0x003EE284
		private void Update()
		{
			bool flag = !this._particlesActive;
			if (!flag)
			{
				bool flag2 = this._binaryAlphaMat != null && this._rt != null && this._processedRt != null;
				if (flag2)
				{
					Graphics.Blit(this._rt, this._processedRt, this._binaryAlphaMat);
				}
			}
		}

		// Token: 0x06008774 RID: 34676 RVA: 0x003F00E8 File Offset: 0x003EE2E8
		private void Teardown()
		{
			this._particlesActive = false;
			bool flag = this._sequence != null;
			if (flag)
			{
				this._sequence.Kill(false);
				this._sequence = null;
			}
			bool flag2 = this.particle != null;
			if (flag2)
			{
				this.particle.Stop();
				this.particle.gameObject.SetActive(false);
			}
			bool flag3 = this.renaddParticle != null;
			if (flag3)
			{
				this.renaddParticle.Stop();
				this.renaddParticle.gameObject.SetActive(false);
			}
			bool flag4 = this.renParticle != null;
			if (flag4)
			{
				this.renParticle.Stop();
				this.renParticle.gameObject.SetActive(false);
			}
			this.RestoreAvatarToUI();
			bool flag5 = this._cameraObj != null;
			if (flag5)
			{
				this._renderCamera.targetTexture = null;
				Object.Destroy(this._cameraObj);
				this._cameraObj = null;
				this._renderCamera = null;
				this._renderCanvas = null;
			}
			bool flag6 = this._processedRt != null;
			if (flag6)
			{
				this._processedRt.Release();
				Object.Destroy(this._processedRt);
				this._processedRt = null;
			}
			bool flag7 = this._binaryAlphaMat != null;
			if (flag7)
			{
				Object.Destroy(this._binaryAlphaMat);
				this._binaryAlphaMat = null;
			}
			bool flag8 = this._rt != null;
			if (flag8)
			{
				this._rt.Release();
				Object.Destroy(this._rt);
				this._rt = null;
			}
		}

		// Token: 0x06008775 RID: 34677 RVA: 0x003F0288 File Offset: 0x003EE488
		public override void QuickHide()
		{
			bool flag = this._sequence != null;
			if (!flag)
			{
				base.QuickHide();
			}
		}

		// Token: 0x06008776 RID: 34678 RVA: 0x003F02AC File Offset: 0x003EE4AC
		private void OnDisable()
		{
			this.Teardown();
		}

		// Token: 0x040067F2 RID: 26610
		[SerializeField]
		private CricketViewNew cricket;

		// Token: 0x040067F3 RID: 26611
		[SerializeField]
		private RectTransform avatarNode;

		// Token: 0x040067F4 RID: 26612
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040067F5 RID: 26613
		[SerializeField]
		private UIParticle particle;

		// Token: 0x040067F6 RID: 26614
		[SerializeField]
		private RectTransform nameArea;

		// Token: 0x040067F7 RID: 26615
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040067F8 RID: 26616
		[SerializeField]
		private ParticleSystem renaddParticle;

		// Token: 0x040067F9 RID: 26617
		[SerializeField]
		private ParticleSystem renParticle;

		// Token: 0x040067FA RID: 26618
		[SerializeField]
		private Vector2 avatarOriginPosition = Vector2.zero;

		// Token: 0x040067FB RID: 26619
		[SerializeField]
		private Vector2 avatarRestoreOffset = Vector2.zero;

		// Token: 0x040067FC RID: 26620
		[SerializeField]
		private float avatarRestoreTime = 1.5f;

		// Token: 0x040067FD RID: 26621
		private GameObject _cameraObj;

		// Token: 0x040067FE RID: 26622
		private Camera _renderCamera;

		// Token: 0x040067FF RID: 26623
		private Canvas _renderCanvas;

		// Token: 0x04006800 RID: 26624
		private RenderTexture _rt;

		// Token: 0x04006801 RID: 26625
		private RenderTexture _processedRt;

		// Token: 0x04006802 RID: 26626
		private Material _binaryAlphaMat;

		// Token: 0x04006803 RID: 26627
		private Sequence _sequence;

		// Token: 0x04006804 RID: 26628
		private int _cricketItemId;

		// Token: 0x04006805 RID: 26629
		private int _charId;

		// Token: 0x04006806 RID: 26630
		private short _colorId;

		// Token: 0x04006807 RID: 26631
		private short _partId;

		// Token: 0x04006808 RID: 26632
		private CharacterDisplayData _charDisplayData;

		// Token: 0x04006809 RID: 26633
		private Transform _avatarOriginalParent;

		// Token: 0x0400680A RID: 26634
		private Vector2 _originalAnchorMin;

		// Token: 0x0400680B RID: 26635
		private Vector2 _originalAnchorMax;

		// Token: 0x0400680C RID: 26636
		private Vector3 _originalScale;

		// Token: 0x0400680D RID: 26637
		private bool _avatarRestored;

		// Token: 0x0400680E RID: 26638
		private bool _particlesActive;
	}
}
