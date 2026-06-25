using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.Camera;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B0B RID: 2827
	public class CombatParticle : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F53 RID: 3923
		// (get) Token: 0x06008AF7 RID: 35575 RVA: 0x00404C0C File Offset: 0x00402E0C
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008AF8 RID: 35576 RVA: 0x00404C14 File Offset: 0x00402E14
		public void Setup()
		{
			Debug.Log("prepare " + base.GetType().Name + " particleRenderTexture");
			RenderTexture particleRenderTexture = new RenderTexture(2560, 1440, 0);
			RenderTexture particleRenderTexture2 = new RenderTexture(2560, 1440, 0);
			Debug.Log("completed " + base.GetType().Name + " particleRenderTexture");
			this.particleCamera1.targetTexture = particleRenderTexture;
			this.particleCamera2.targetTexture = particleRenderTexture2;
			this.particleImage1.texture = particleRenderTexture;
			this.particleImage2.texture = particleRenderTexture2;
			this._particleRenderRect = this.particleImage1.rectTransform;
			this.particleCamera1.enabled = true;
			this.particleCamera2.enabled = true;
			this.particleImage1.gameObject.SetActive(true);
			this.particleImage2.gameObject.SetActive(true);
			CombatModel model = this.Model;
			model.OnParticleToPlayChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model.OnParticleToPlayChanged, new OnCharacterDataChangedEvent(this.OnParticleToPlayChanged));
			CombatModel model2 = this.Model;
			model2.OnParticleToLoopChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model2.OnParticleToLoopChanged, new OnCharacterDataChangedEvent(this.OnParticleToLoopChanged));
			CombatModel model3 = this.Model;
			model3.OnParticleToLoopByCombatSkillChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model3.OnParticleToLoopByCombatSkillChanged, new OnCharacterDataChangedEvent(this.OnParticleToLoopByCombatSkillChanged));
			GEvent.Add(UiEvents.PlaySkeletonParticle, new GEvent.Callback(this.OnPlaySkeletonParticle));
			this.grab1.gameObject.SetActive(true);
			this.grab2.gameObject.SetActive(true);
			this.grab1.RegisterRtReceiver(this.particleCamera1.GetComponent<CameraQuadRenderer>());
			this.grab2.RegisterRtReceiver(this.particleCamera2.GetComponent<CameraQuadRenderer>());
		}

		// Token: 0x06008AF9 RID: 35577 RVA: 0x00404DEC File Offset: 0x00402FEC
		public void Close()
		{
			this.particleCamera1.DOKill(false);
			this.particleCamera2.DOKill(false);
			this.CleanupAllParticles();
			this.grab1.UnregisterRtReceiver(this.particleCamera1.GetComponent<CameraQuadRenderer>());
			this.grab2.UnregisterRtReceiver(this.particleCamera2.GetComponent<CameraQuadRenderer>());
			this.particleCamera1.enabled = false;
			this.particleCamera2.enabled = false;
			bool flag = this.particleCamera1.targetTexture != null;
			if (flag)
			{
				this.particleCamera1.targetTexture.Release();
				this.particleCamera1.targetTexture = null;
			}
			bool flag2 = this.particleCamera2.targetTexture != null;
			if (flag2)
			{
				this.particleCamera2.targetTexture.Release();
				this.particleCamera2.targetTexture = null;
			}
			CombatModel model = this.Model;
			model.OnParticleToPlayChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model.OnParticleToPlayChanged, new OnCharacterDataChangedEvent(this.OnParticleToPlayChanged));
			CombatModel model2 = this.Model;
			model2.OnParticleToLoopChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model2.OnParticleToLoopChanged, new OnCharacterDataChangedEvent(this.OnParticleToLoopChanged));
			CombatModel model3 = this.Model;
			model3.OnParticleToLoopByCombatSkillChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model3.OnParticleToLoopByCombatSkillChanged, new OnCharacterDataChangedEvent(this.OnParticleToLoopByCombatSkillChanged));
			GEvent.Remove(UiEvents.PlaySkeletonParticle, new GEvent.Callback(this.OnPlaySkeletonParticle));
		}

		// Token: 0x06008AFA RID: 35578 RVA: 0x00404F60 File Offset: 0x00403160
		private void OnPlaySkeletonParticle(ArgumentBox argumentBox)
		{
			bool isAlly;
			argumentBox.Get("IsAlly", out isAlly);
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter combatSubProcessorCharacter;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out combatSubProcessorCharacter);
			if (!flag)
			{
				SkeletonAnimation skeleton = this.GetSkeleton(charId);
				string vfxName;
				argumentBox.Get("VfxName", out vfxName);
				this.PlayVfx(isAlly, skeleton, vfxName);
			}
		}

		// Token: 0x06008AFB RID: 35579 RVA: 0x00404FD8 File Offset: 0x004031D8
		private void OnParticleToLoopByCombatSkillChanged(int charId)
		{
			CombatSubProcessorCharacterDisplay displayProcessor;
			bool flag = this.Model.TryGetCharacterDisplayProcessor(charId, out displayProcessor);
			if (flag)
			{
				string particleToLoop = displayProcessor.ParticleToLoopByCombatSkill;
				bool isAlly = this.Model.CharIsAlly(charId);
				this.SetLoopVfxByCombatSkill(charId, isAlly, particleToLoop);
			}
		}

		// Token: 0x06008AFC RID: 35580 RVA: 0x00405020 File Offset: 0x00403220
		private void OnParticleToLoopChanged(int charId)
		{
			CombatSubProcessorCharacterDisplay displayProcessor;
			bool flag = this.Model.TryGetCharacterDisplayProcessor(charId, out displayProcessor);
			if (flag)
			{
				string particleToLoop = displayProcessor.ParticleToLoop;
				bool isAlly = this.Model.CharIsAlly(charId);
				this.SetLoopVfx(charId, isAlly, particleToLoop);
			}
		}

		// Token: 0x06008AFD RID: 35581 RVA: 0x00405068 File Offset: 0x00403268
		private void OnParticleToPlayChanged(int charId)
		{
			CombatSubProcessorCharacterDisplay displayProcessor;
			bool flag = this.Model.TryGetCharacterDisplayProcessor(charId, out displayProcessor);
			if (flag)
			{
				string particleToPlay = displayProcessor.ParticleToPlay;
				bool flag2 = string.IsNullOrEmpty(particleToPlay);
				if (!flag2)
				{
					bool flag3 = this.ParticleLocalizeCallback != null;
					if (flag3)
					{
						particleToPlay = this.ParticleLocalizeCallback(particleToPlay);
					}
					bool isAlly = this.Model.CharIsAlly(charId);
					this.PlayVfx(isAlly, this.GetSkeleton(charId), particleToPlay);
				}
			}
		}

		// Token: 0x06008AFE RID: 35582 RVA: 0x004050E0 File Offset: 0x004032E0
		public void PlayVfx(bool isAlly, SkeletonAnimation skeleton, string vfxName)
		{
			GameObject vfxGo = CombatPoolAdaptor.GetObject(vfxName, true);
			bool flag = vfxGo == null;
			if (flag)
			{
				throw new Exception("Vfx " + vfxName + " not found");
			}
			Transform vfxTransform = vfxGo.transform;
			ParticleSystem particle = vfxGo.GetComponent<ParticleSystem>();
			bool flag2 = vfxGo.GetComponent<Refers>() == null;
			if (flag2)
			{
				vfxGo.AddComponent<Refers>();
			}
			vfxGo.GetComponent<Refers>().UserObject = skeleton;
			vfxGo.name = vfxName;
			vfxGo.SetActive(true);
			vfxTransform.SetParent(this.particleHolder, false);
			vfxTransform.localEulerAngles = Vector3.zero.SetY((float)(isAlly ? 0 : 180));
			this.UpdateParticlePos(vfxTransform);
			particle.Play(true);
			SkeletonAnimation[] aniList = vfxGo.GetComponentsInChildren<SkeletonAnimation>();
			foreach (SkeletonAnimation ani in aniList)
			{
				ani.AnimationState.SetAnimation(0, ani.Skeleton.Data.Animations.Items[0].Name, false).MixDuration = 0f;
			}
			base.StartCoroutine(this.DestroyParticle(vfxName, particle));
		}

		// Token: 0x06008AFF RID: 35583 RVA: 0x00405204 File Offset: 0x00403404
		public void SetLoopVfx(int charId, bool isAlly, string vfxName, IDictionary<int, ParticleSystem> particleDict)
		{
			bool flag = particleDict.ContainsKey(charId);
			if (flag)
			{
				Object.Destroy(particleDict[charId].gameObject);
				particleDict.Remove(charId);
			}
			bool flag2 = vfxName.IsNullOrEmpty();
			if (!flag2)
			{
				SkeletonAnimation skeleton = this.GetSkeleton(charId);
				GameObject vfxGo = CombatPoolAdaptor.GetObject(vfxName, true);
				Transform vfxTransform = vfxGo.transform;
				ParticleSystem particle = vfxGo.GetComponent<ParticleSystem>();
				bool flag3 = vfxGo.GetComponent<Refers>() == null;
				if (flag3)
				{
					vfxGo.AddComponent<Refers>();
				}
				vfxGo.GetComponent<Refers>().UserObject = skeleton;
				vfxGo.name = vfxName;
				vfxGo.SetActive(true);
				vfxTransform.SetParent(this.particleHolder, false);
				vfxTransform.localEulerAngles = Vector3.zero.SetY((float)(isAlly ? 0 : 180));
				this.UpdateParticlePos(vfxTransform);
				particle.Play(true);
				particleDict.Add(charId, particle);
			}
		}

		// Token: 0x06008B00 RID: 35584 RVA: 0x004052E7 File Offset: 0x004034E7
		public void SetLoopVfx(int charId, bool isAlly, string vfxName)
		{
			this.SetLoopVfx(charId, isAlly, vfxName, this._loopParticleDict);
		}

		// Token: 0x06008B01 RID: 35585 RVA: 0x004052FA File Offset: 0x004034FA
		public void SetLoopVfxByCombatSkill(int charId, bool isAlly, string vfxName)
		{
			this.SetLoopVfx(charId, isAlly, vfxName, this._loopParticleByCombatSkillDict);
		}

		// Token: 0x06008B02 RID: 35586 RVA: 0x00405310 File Offset: 0x00403510
		public void UpdateAllParticlePos()
		{
			for (int i = 0; i < this.particleHolder.childCount; i++)
			{
				Transform vfxTransform = this.particleHolder.GetChild(i);
				bool flag = !vfxTransform.gameObject.activeSelf;
				if (!flag)
				{
					this.UpdateParticlePos(vfxTransform);
				}
			}
		}

		// Token: 0x06008B03 RID: 35587 RVA: 0x00405364 File Offset: 0x00403564
		public void UpdateParticlePos(Transform vfxTransform)
		{
			bool flag = vfxTransform == null || !vfxTransform.gameObject.activeInHierarchy;
			if (!flag)
			{
				bool flag2 = this._particleRenderRect == null || this.particleHolder == null || this.particleCamera1 == null;
				if (!flag2)
				{
					bool flag3 = !this.particleCamera1.enabled;
					if (!flag3)
					{
						Refers refers = vfxTransform.GetComponent<Refers>();
						bool flag4 = refers == null;
						if (!flag4)
						{
							SkeletonAnimation skeleton = refers.UserObject as SkeletonAnimation;
							bool flag5 = skeleton == null;
							if (!flag5)
							{
								UIManager instance = UIManager.Instance;
								Camera uiCamera = (instance != null) ? instance.UiCamera : null;
								bool flag6 = uiCamera == null;
								if (!flag6)
								{
									Vector3 skeletonWorldPos = skeleton.transform.position;
									Vector3 screenPos = uiCamera.WorldToScreenPoint(skeletonWorldPos);
									bool flag7 = float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y) || float.IsNaN(screenPos.z);
									if (!flag7)
									{
										bool flag8 = screenPos.z <= 0f;
										if (!flag8)
										{
											Vector2 localPos;
											bool flag9 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this._particleRenderRect, screenPos, uiCamera, out localPos);
											if (!flag9)
											{
												Rect rect = this._particleRenderRect.rect;
												bool flag10 = rect.width <= 0.001f || rect.height <= 0.001f;
												if (!flag10)
												{
													float coordX = Mathf.Clamp(localPos.x - rect.x, 0f, rect.width);
													float coordY = Mathf.Clamp(localPos.y - rect.y, 0f, rect.height);
													float pixelX = Mathf.Clamp(coordX / rect.width * (float)this.particleCamera1.pixelWidth, 0f, (float)this.particleCamera1.pixelWidth);
													float pixelY = Mathf.Clamp(coordY / rect.height * (float)this.particleCamera1.pixelHeight, 0f, (float)this.particleCamera1.pixelHeight);
													Ray ray = this.particleCamera1.ScreenPointToRay(new Vector2(pixelX, pixelY));
													Plane plane = new Plane(Vector3.forward, Vector3.zero.SetZ(this.particleHolder.position.z));
													float distance;
													bool flag11 = !plane.Raycast(ray, out distance);
													if (!flag11)
													{
														bool flag12 = float.IsNaN(distance) || float.IsInfinity(distance);
														if (!flag12)
														{
															vfxTransform.localPosition = this.particleHolder.InverseTransformPoint(ray.GetPoint(distance));
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008B04 RID: 35588 RVA: 0x00405634 File Offset: 0x00403834
		public void SetParticleTimePause(bool pause)
		{
			for (int i = 0; i < this.particleHolder.childCount; i++)
			{
				ParticleSystem particle = this.particleHolder.GetChild(i).GetComponent<ParticleSystem>();
				bool flag = !particle.gameObject.activeSelf;
				if (!flag)
				{
					if (pause)
					{
						particle.Pause(true);
					}
					else
					{
						particle.Play(true);
					}
				}
			}
		}

		// Token: 0x06008B05 RID: 35589 RVA: 0x004056A0 File Offset: 0x004038A0
		public void SetCameraEnabled(bool isEnabled)
		{
			this.particleCamera1.enabled = isEnabled;
			this.particleCamera2.enabled = isEnabled;
			CameraQuadRenderer quadRenderer = this.particleCamera1.GetComponent<CameraQuadRenderer>();
			bool flag = quadRenderer != null;
			if (flag)
			{
				quadRenderer.enabled = isEnabled;
			}
			CameraQuadRenderer quadRenderer2 = this.particleCamera2.GetComponent<CameraQuadRenderer>();
			bool flag2 = quadRenderer2 != null;
			if (flag2)
			{
				quadRenderer2.enabled = isEnabled;
			}
		}

		// Token: 0x06008B06 RID: 35590 RVA: 0x00405708 File Offset: 0x00403908
		public void DoCameraOrthoSize(float orthoSize, float duration)
		{
			this.particleCamera1.DOKill(false);
			this.particleCamera2.DOKill(false);
			this.particleCamera1.DOOrthoSize(orthoSize, duration).OnUpdate(new TweenCallback(this.UpdateAllParticlePos));
			this.particleCamera2.DOOrthoSize(orthoSize, duration);
		}

		// Token: 0x06008B07 RID: 35591 RVA: 0x00405760 File Offset: 0x00403960
		public void CleanupAllParticles()
		{
			for (int i = 0; i < this.particleHolder.childCount; i++)
			{
				GameObject particleGo = this.particleHolder.GetChild(i).GetComponent<ParticleSystem>().gameObject;
				bool flag = !particleGo.activeSelf;
				if (!flag)
				{
					CombatPoolAdaptor.Destroy(particleGo.name, particleGo, true);
				}
			}
			this._loopParticleDict.Clear();
			this._loopParticleByCombatSkillDict.Clear();
		}

		// Token: 0x06008B08 RID: 35592 RVA: 0x004057D6 File Offset: 0x004039D6
		private IEnumerator DestroyParticle(string vfxName, ParticleSystem particle)
		{
			float timeAccumulator = 0f;
			while (timeAccumulator < particle.main.duration)
			{
				bool flag = !particle.isPaused;
				if (flag)
				{
					timeAccumulator += Time.deltaTime;
				}
				yield return null;
			}
			CombatPoolAdaptor.Destroy(vfxName, particle.gameObject, true);
			yield break;
		}

		// Token: 0x06008B09 RID: 35593 RVA: 0x004057F4 File Offset: 0x004039F4
		private SkeletonAnimation GetSkeleton(int charId)
		{
			bool flag = this.GetSkeletonCallback != null;
			SkeletonAnimation result;
			if (flag)
			{
				result = this.GetSkeletonCallback(charId);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06008B0A RID: 35594 RVA: 0x00405824 File Offset: 0x00403A24
		public ParticleSystem PlayDefendVfx(int charId, bool isAlly, string vfxName)
		{
			SkeletonAnimation skeleton = this.GetSkeleton(charId);
			GameObject vfxGo = CombatPoolAdaptor.GetObject(vfxName, true);
			bool flag = vfxGo == null;
			ParticleSystem result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Transform vfxTransform = vfxGo.transform;
				ParticleSystem particle = vfxGo.GetComponent<ParticleSystem>();
				bool flag2 = vfxGo.GetComponent<Refers>() == null;
				if (flag2)
				{
					vfxGo.AddComponent<Refers>();
				}
				vfxGo.GetComponent<Refers>().UserObject = skeleton;
				vfxGo.name = vfxName;
				vfxGo.SetActive(true);
				vfxTransform.SetParent(this.particleHolder, false);
				vfxTransform.localEulerAngles = Vector3.zero.SetY((float)(isAlly ? 0 : 180));
				this.UpdateParticlePos(vfxTransform);
				particle.Play(true);
				result = particle;
			}
			return result;
		}

		// Token: 0x17000F54 RID: 3924
		// (get) Token: 0x06008B0B RID: 35595 RVA: 0x004058DC File Offset: 0x00403ADC
		// (set) Token: 0x06008B0C RID: 35596 RVA: 0x004058E4 File Offset: 0x00403AE4
		public Func<int, SkeletonAnimation> GetSkeletonCallback { get; set; }

		// Token: 0x17000F55 RID: 3925
		// (get) Token: 0x06008B0D RID: 35597 RVA: 0x004058ED File Offset: 0x00403AED
		// (set) Token: 0x06008B0E RID: 35598 RVA: 0x004058F5 File Offset: 0x00403AF5
		public Func<string, string> ParticleLocalizeCallback { get; set; }

		// Token: 0x04006AA5 RID: 27301
		private RectTransform _particleRenderRect;

		// Token: 0x04006AA6 RID: 27302
		private readonly Dictionary<int, ParticleSystem> _loopParticleDict = new Dictionary<int, ParticleSystem>();

		// Token: 0x04006AA7 RID: 27303
		private readonly Dictionary<int, ParticleSystem> _loopParticleByCombatSkillDict = new Dictionary<int, ParticleSystem>();

		// Token: 0x04006AAA RID: 27306
		[SerializeField]
		private Camera particleCamera1;

		// Token: 0x04006AAB RID: 27307
		[SerializeField]
		private Camera particleCamera2;

		// Token: 0x04006AAC RID: 27308
		[SerializeField]
		private Transform particleHolder;

		// Token: 0x04006AAD RID: 27309
		[SerializeField]
		private CRawImage particleImage1;

		// Token: 0x04006AAE RID: 27310
		[SerializeField]
		private CRawImage particleImage2;

		// Token: 0x04006AAF RID: 27311
		[SerializeField]
		private UIGrabGraphic grab1;

		// Token: 0x04006AB0 RID: 27312
		[SerializeField]
		private UIGrabGraphic grab2;
	}
}
