using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.Cricket.Combat;
using Game.Views.MouseTips;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB9 RID: 2745
	public class CricketViewNew : MonoBehaviour
	{
		// Token: 0x060086AA RID: 34474 RVA: 0x003E9FAC File Offset: 0x003E81AC
		private static bool CheckIsMatch(string idStr, string attach)
		{
			bool flag = attach.EndsWith(idStr) && CricketViewNew.CheckIsMatchPrefix(attach, attach.Length - idStr.Length);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int index = attach.IndexOf(idStr, StringComparison.Ordinal);
				bool flag2 = index + idStr.Length >= attach.Length;
				result = (!flag2 && CricketViewNew.CheckIsMatchPrefix(attach, index) && attach[index + idStr.Length] == '_');
			}
			return result;
		}

		// Token: 0x060086AB RID: 34475 RVA: 0x003EA028 File Offset: 0x003E8228
		private static bool CheckIsMatchPrefix(string attach, int index)
		{
			bool flag = index < 2;
			return !flag && attach[index - 1] == '_' && !char.IsDigit(attach[index - 2]);
		}

		// Token: 0x17000ED1 RID: 3793
		// (get) Token: 0x060086AC RID: 34476 RVA: 0x003EA067 File Offset: 0x003E8267
		// (set) Token: 0x060086AD RID: 34477 RVA: 0x003EA06F File Offset: 0x003E826F
		public bool IsSinging { get; private set; }

		// Token: 0x17000ED2 RID: 3794
		// (get) Token: 0x060086AE RID: 34478 RVA: 0x003EA078 File Offset: 0x003E8278
		// (set) Token: 0x060086AF RID: 34479 RVA: 0x003EA080 File Offset: 0x003E8280
		public ITradeableContent ItemData { get; private set; }

		// Token: 0x17000ED3 RID: 3795
		// (get) Token: 0x060086B0 RID: 34480 RVA: 0x003EA089 File Offset: 0x003E8289
		public string Name
		{
			get
			{
				return new ValueTuple<short, short>(this.ColorId, this.PartId).CalcCricketName();
			}
		}

		// Token: 0x17000ED4 RID: 3796
		// (get) Token: 0x060086B1 RID: 34481 RVA: 0x003EA0A1 File Offset: 0x003E82A1
		public int Level
		{
			get
			{
				return this._isCombineCricket ? Mathf.Max((int)this._colorConfig.Level, (int)this._partConfig.Level) : ((int)this._colorConfig.Level);
			}
		}

		// Token: 0x17000ED5 RID: 3797
		// (get) Token: 0x060086B2 RID: 34482 RVA: 0x003EA0D4 File Offset: 0x003E82D4
		public int Value
		{
			get
			{
				ITradeableContent itemData = this.ItemData;
				long? num = (itemData != null) ? new long?(itemData.Value) : null;
				return ((num != null) ? new int?((int)num.GetValueOrDefault()) : null) ?? (this._isCombineCricket ? Mathf.Max(this._colorConfig.Value, this._partConfig.Value) : this._colorConfig.Value);
			}
		}

		// Token: 0x17000ED6 RID: 3798
		// (get) Token: 0x060086B3 RID: 34483 RVA: 0x003EA165 File Offset: 0x003E8365
		public int SingSize
		{
			get
			{
				return (int)(this._isCombineCricket ? (this._colorConfig.SingSize + this._partConfig.SingSize) : this._colorConfig.SingSize);
			}
		}

		// Token: 0x17000ED7 RID: 3799
		// (get) Token: 0x060086B4 RID: 34484 RVA: 0x003EA193 File Offset: 0x003E8393
		public int SingPitch
		{
			get
			{
				return (int)(this._isCombineCricket ? (this._colorConfig.SingPitch + this._partConfig.SingPitch) : this._colorConfig.SingPitch);
			}
		}

		// Token: 0x060086B5 RID: 34485 RVA: 0x003EA1C4 File Offset: 0x003E83C4
		private void OnDisable()
		{
			AudioManager.Instance.RemoveAudioSource(this._audioSource);
			bool flag = this._loopAniTween != null && this._loopAniTween.IsActive();
			if (flag)
			{
				this._loopAniTween.Kill(false);
			}
		}

		// Token: 0x060086B6 RID: 34486 RVA: 0x003EA20C File Offset: 0x003E840C
		private void OnDestroy()
		{
			bool flag = this._loopAniTween != null && this._loopAniTween.IsActive();
			if (flag)
			{
				this._loopAniTween.Kill(false);
			}
		}

		// Token: 0x060086B7 RID: 34487 RVA: 0x003EA244 File Offset: 0x003E8444
		public void SetSingImagePositionToZero()
		{
			bool flag = this._singImage != null;
			if (flag)
			{
				this._singImage.rectTransform.localPosition = Vector3.zero;
			}
		}

		// Token: 0x060086B8 RID: 34488 RVA: 0x003EA278 File Offset: 0x003E8478
		public void SetCricketData(short colorId, short partId, bool showMouseTips = false, ITradeableContent itemData = null, bool isTripleTail = false)
		{
			Profiler.BeginSample("CricketViewNew.SetCricketData");
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			this.ColorId = colorId;
			this.PartId = partId;
			this.ShowMouseTips = showMouseTips;
			this.ItemData = itemData;
			this.IsTripleTail = isTripleTail;
			bool flag = !this.Inited;
			if (flag)
			{
				this._singImage = this.singImage;
				this._audioSource = base.GetComponent<AudioSource>();
				this.skeletonGraphic.material = new UnityEngine.Material(this.skeletonGraphic.material);
				this._normalShader = this.skeletonGraphic.material.shader;
				this.Inited = true;
			}
			this._colorConfig = CricketParts.Instance[colorId];
			this._partConfig = CricketParts.Instance[partId];
			this._isCombineCricket = (partId > 0);
			this._loopSing = false;
			this._singImage.DOKill(false);
			this._singImage.SetAlpha(0f);
			this._audioSource.DOKill(false);
			this._audioSource.pitch = 1.25f - 0.65f * (float)this.SingPitch / 100f;
			this._audioSource.volume = ((settings.SeOn && !AudioManager.Instance.GetShouldMuteIfFocus()) ? ((float)settings.SeVolume / 100f) : 0f);
			this._audioSource.Stop();
			AudioManager.Instance.AddAudioSource(this._audioSource, this._audioSource.volume);
			this.skeletonGraphic.Initialize(false);
			short partTemplateId = (this._isCombineCricket ? this._partConfig : this._colorConfig).TemplateId;
			Skeleton skeleton = this.skeletonGraphic.Skeleton;
			this.skeletonGraphic.color = this.skeletonGraphic.color.SetAlpha(1f);
			this.skeletonGraphic.material.SetColor(CricketViewNew.TargetColor, this._isCombineCricket ? this._colorConfig.Color.HexStringToColor() : Color.white);
			CricketViewSpineHelper.SetSpineDisplay(partTemplateId, skeleton, skeleton.Data.DefaultSkin, isTripleTail);
			Profiler.EndSample();
			TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
			tipDisplayer.enabled = showMouseTips;
			base.GetComponent<CEmptyGraphic>().enabled = showMouseTips;
			if (showMouseTips)
			{
				this.RestoreDefaultMouseTip(false);
			}
		}

		// Token: 0x060086B9 RID: 34489 RVA: 0x003EA4CF File Offset: 0x003E86CF
		public void SetCricketData(CricketView other)
		{
			this.SetCricketData(other.ColorId, other.PartId, other.ShowMouseTips, other.ItemData, other.IsTripleTail);
		}

		// Token: 0x060086BA RID: 34490 RVA: 0x003EA4F8 File Offset: 0x003E86F8
		public void RestoreDefaultMouseTip(bool forceUseHideAndShow = false)
		{
			bool flag = !this.ShowMouseTips;
			if (!flag)
			{
				TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
				tipDisplayer.Type = (this.IsTripleTail ? TipType.SingleDesc : TipType.Cricket);
				TooltipInvoker tooltipInvoker = tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tipDisplayer.RuntimeParam.Clear();
				bool isTripleTail = this.IsTripleTail;
				if (isTripleTail)
				{
					tipDisplayer.RuntimeParam.SetObject("arg0", Misc.Instance[34].Name);
				}
				else
				{
					tipDisplayer.RuntimeParam.SetObject("ItemData", this.ItemData);
				}
				tipDisplayer.Refresh(forceUseHideAndShow, -1);
			}
		}

		// Token: 0x060086BB RID: 34491 RVA: 0x003EA5A8 File Offset: 0x003E87A8
		public void ShowSkillReplaceTip(CricketCombatDisplayData displayData, bool forceUseHideAndShow = false)
		{
			bool flag = displayData == null;
			if (flag)
			{
				this.RestoreDefaultMouseTip(forceUseHideAndShow);
			}
			else
			{
				int replacedSkillId = (int)displayData.Data.Skill;
				CricketSkillItem replacedSkill = CricketSkill.Instance[replacedSkillId];
				bool flag2 = replacedSkill == null;
				if (flag2)
				{
					this.RestoreDefaultMouseTip(forceUseHideAndShow);
				}
				else
				{
					int originalSkillId = (int)displayData.OriginalSkill;
					CricketSkillItem originalSkill = (originalSkillId >= 0) ? CricketSkill.Instance[originalSkillId] : null;
					bool flag3 = originalSkill != null && originalSkillId == replacedSkillId;
					if (flag3)
					{
						this.RestoreDefaultMouseTip(forceUseHideAndShow);
					}
					else
					{
						TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
						CricketViewNew.CricketSkillReplaceRuntime runtime = new CricketViewNew.CricketSkillReplaceRuntime(CommonTip.DefValue.CricketSkillReplace, originalSkill != null);
						bool flag4 = originalSkill != null;
						if (flag4)
						{
							runtime.Set("skillName1", originalSkill.Name).Set("skillDesc1", originalSkill.Desc);
						}
						runtime.Set("skillName2", replacedSkill.Name).Set("skillDesc2", replacedSkill.Desc);
						tipDisplayer.Type = TipType.CommonTip;
						TooltipInvoker tooltipInvoker = tipDisplayer;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						tipDisplayer.RuntimeParam.Clear();
						tipDisplayer.RuntimeParam.SetObject("Runtime", runtime);
						tipDisplayer.Refresh(forceUseHideAndShow, -1);
					}
				}
			}
		}

		// Token: 0x060086BC RID: 34492 RVA: 0x003EA6F4 File Offset: 0x003E88F4
		public void PlayAnimation(ECricketAnim ani, bool loop = false, bool restart = false)
		{
			bool flag = base.gameObject == null || !base.gameObject.activeInHierarchy;
			if (!flag)
			{
				string aniName = ani.ToString().ToLower();
				TrackEntry currEntry = this.skeletonGraphic.AnimationState.GetCurrent(0);
				bool flag2 = !restart && currEntry != null && currEntry.Animation.Name == aniName;
				if (!flag2)
				{
					bool flag3 = ani == ECricketAnim.Die;
					if (flag3)
					{
						bool loopSing = this._loopSing;
						if (loopSing)
						{
							this.StopLoopSing();
						}
						bool flag4 = this._loopAniTween != null && this._loopAniTween.IsActive() && this._loopAniTween.IsPlaying();
						if (flag4)
						{
							this._loopAniTween.Kill(false);
							this._loopAniTween = null;
						}
					}
					this.skeletonGraphic.AnimationState.SetAnimation(0, aniName, loop);
				}
			}
		}

		// Token: 0x060086BD RID: 34493 RVA: 0x003EA7E1 File Offset: 0x003E89E1
		public void StopAnimation()
		{
			Tween loopAniTween = this._loopAniTween;
			if (loopAniTween != null)
			{
				loopAniTween.Kill(false);
			}
			this.skeletonGraphic.AnimationState.SetEmptyAnimation(0, 0f);
			this.skeletonGraphic.Skeleton.SetBonesToSetupPose();
		}

		// Token: 0x060086BE RID: 34494 RVA: 0x003EA820 File Offset: 0x003E8A20
		public float GetAnimationDuration(ECricketAnim animType)
		{
			Spine.Animation animation = this.skeletonGraphic.Skeleton.Data.FindAnimation(animType.ToString().ToLower());
			return (animation != null) ? animation.Duration : 0f;
		}

		// Token: 0x060086BF RID: 34495 RVA: 0x003EA86C File Offset: 0x003E8A6C
		public float GetEventTime(ECricketAnim ani, string evtName)
		{
			float time = -1f;
			ExposedList<Timeline> timeLineList = this.skeletonGraphic.Skeleton.Data.FindAnimation(ani.ToString().ToLower()).Timelines;
			Func<Spine.Event, bool> <>9__0;
			foreach (Timeline timeLine in timeLineList)
			{
				EventTimeline eventTimeLine = timeLine as EventTimeline;
				bool flag = eventTimeLine != null;
				if (flag)
				{
					IEnumerable<Spine.Event> events = eventTimeLine.Events;
					Func<Spine.Event, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((Spine.Event evt) => evt.Data.Name == evtName));
					}
					Spine.Event aniEvent = events.FirstOrDefault(predicate);
					bool flag2 = aniEvent != null;
					if (flag2)
					{
						time = aniEvent.Time;
						break;
					}
				}
			}
			return time;
		}

		// Token: 0x060086C0 RID: 34496 RVA: 0x003EA95C File Offset: 0x003E8B5C
		public void Sing(bool loop = false, bool playSound = true, bool playAni = true, float singSize = -1f, Action<float> onSingStart = null, float fadeInDuration = 0f)
		{
			bool flag = this == null || !base.gameObject.activeInHierarchy || this._singImage == null;
			if (flag)
			{
				this.IsSinging = false;
			}
			else
			{
				bool flag2 = onSingStart != null && this._onSingStartCallBack == null;
				if (flag2)
				{
					this._onSingStartCallBack = delegate()
					{
						Action<float> onSingStart2 = this._onSingStart;
						if (onSingStart2 != null)
						{
							onSingStart2((singSize >= 0f) ? singSize : ((float)this.SingSize / 100f));
						}
					};
				}
				bool flag3 = onSingStart == null && this._onSingStartCallBack != null;
				if (flag3)
				{
					this._onSingStartCallBack = null;
				}
				this._onSingStart = onSingStart;
				this.IsSinging = true;
				this._loopSing = loop;
				this._singImage.DOKill(false);
				this._singImage.rectTransform.DOKill(false);
				this._singImage.rectTransform.localScale = Vector3.zero;
				this._singImage.rectTransform.DOScale((0.3f + 0.7f * singSize >= 0f) ? singSize : ((float)this.SingSize / 100f), 1.2f).OnStart(this._onSingStartCallBack);
				this._singImage.SetAlpha(1f);
				Action <>9__3;
				this._singImage.DOFade(0f, 0.6f).SetDelay(0.6f).OnComplete(delegate
				{
					YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
					float sec = Random.Range(3f, (float)(4 + this.Level));
					Action job;
					if ((job = <>9__3) == null)
					{
						job = (<>9__3 = delegate()
						{
							bool loopSing = this._loopSing;
							if (loopSing)
							{
								this.Sing(true, playSound, playAni, singSize, onSingStart, 0f);
							}
							else
							{
								this.IsSinging = false;
							}
						});
					}
					instance.DelaySecondsDo(sec, job);
				});
				bool playSound2 = playSound;
				if (playSound2)
				{
					GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
					this._audioSource.DOKill(false);
					float rate = (!this._isStartCombat || this._isInCombat) ? 1f : 0.5f;
					float volume = (settings.SeOn && !AudioManager.Instance.GetShouldMuteIfFocus()) ? ((float)settings.SeVolume * rate / 100f - (float)(9 - this.Level) * 0.02f) : 0f;
					bool isOnCricketCombat = UIManager.Instance.IsElementActive(UIElement.CricketCombat);
					this._audioSource.panStereo = ((!isOnCricketCombat || this._isInCombat) ? 0f : (this._isOnLeft ? -0.25f : 0.25f));
					this._audioSource.volume = 0f;
					this._audioSource.DOFade(volume, fadeInDuration);
					this._audioSource.Play();
					this._audioSource.DOFade(0f, 0.6f).SetDelay(0.6f).OnComplete(new TweenCallback(this._audioSource.Stop)).SetUpdate(true);
					AudioManager.Instance.AddAudioSource(this._audioSource, volume);
				}
				bool playAni2 = playAni;
				if (playAni2)
				{
					this.PlayAnimation(ECricketAnim.Idle2, false, true);
					this._loopAniTween = DOVirtual.DelayedCall(0.33f, delegate
					{
						this.PlayAnimation(ECricketAnim.Idle, true, false);
					}, true);
				}
			}
		}

		// Token: 0x060086C1 RID: 34497 RVA: 0x003EAC6F File Offset: 0x003E8E6F
		public void StopLoopSing()
		{
			this._loopSing = false;
		}

		// Token: 0x060086C2 RID: 34498 RVA: 0x003EAC7C File Offset: 0x003E8E7C
		public void StopSing(float fadeDuration)
		{
			this.StopAnimation();
			this._loopSing = false;
			this._singImage.DOKill(false);
			this._singImage.DOFade(0f, fadeDuration);
			this._audioSource.DOKill(false);
			this._audioSource.DOFade(0f, fadeDuration);
			AudioManager.Instance.RemoveAudioSource(this._audioSource);
		}

		// Token: 0x060086C3 RID: 34499 RVA: 0x003EACE8 File Offset: 0x003E8EE8
		public void PauseSing()
		{
			bool isSinging = this.IsSinging;
			if (isSinging)
			{
				this._audioSource.Pause();
			}
		}

		// Token: 0x060086C4 RID: 34500 RVA: 0x003EAD0C File Offset: 0x003E8F0C
		public void ResumeSing()
		{
			bool isSinging = this.IsSinging;
			if (isSinging)
			{
				this._audioSource.Play();
			}
		}

		// Token: 0x060086C5 RID: 34501 RVA: 0x003EAD30 File Offset: 0x003E8F30
		public void Twinkle()
		{
			this.skeletonGraphic.material.shader = this.twinkleShader;
			this.skeletonGraphic.material.SetColor(CricketViewNew.TwinkleColor, Color.white.SetAlpha(0.5f));
			DOVirtual.Float(0f, 2f, 0.1f, delegate(float stepValue)
			{
				float twinkleRate = (stepValue < 1f) ? Mathf.Lerp(0.5f, 1.2f, stepValue) : Mathf.Lerp(1.2f, 0.5f, stepValue - 1f);
				this.skeletonGraphic.material.SetFloat(CricketViewNew.TwinkleRate, twinkleRate);
			}).SetAutoKill(true);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
			{
				this.skeletonGraphic.material.shader = this._normalShader;
			});
		}

		// Token: 0x060086C6 RID: 34502 RVA: 0x003EADBD File Offset: 0x003E8FBD
		public void Fade(bool fadeIn)
		{
			this.skeletonGraphic.DOKill(false);
			this.skeletonGraphic.DOFade((float)(fadeIn ? 1 : 0), 1f).SetEase(Ease.InCirc);
		}

		// Token: 0x060086C7 RID: 34503 RVA: 0x003EADED File Offset: 0x003E8FED
		public void SetSoundData(bool isStartCombat, bool isInCombat, bool isOnLeft)
		{
			this._isStartCombat = isStartCombat;
			this._isInCombat = isInCombat;
			this._isOnLeft = isOnLeft;
		}

		// Token: 0x060086C8 RID: 34504 RVA: 0x003EAE08 File Offset: 0x003E9008
		public CImage GetSingImage()
		{
			return this.singImage;
		}

		// Token: 0x060086CA RID: 34506 RVA: 0x003EAE2C File Offset: 0x003E902C
		// Note: this type is marked as 'beforefieldinit'.
		static CricketViewNew()
		{
			Dictionary<ECricketSlot, ECricketSlot> dictionary = new Dictionary<ECricketSlot, ECricketSlot>();
			dictionary[ECricketSlot.Trophi_lmask] = ECricketSlot.Trophi_l;
			dictionary[ECricketSlot.Trophi_rmask] = ECricketSlot.Trophi_r;
			dictionary[ECricketSlot.FrontLeg_lmask] = ECricketSlot.FrontLeg_l;
			dictionary[ECricketSlot.FrontLeg_rmask] = ECricketSlot.FrontLeg_r;
			dictionary[ECricketSlot.BackLeg_lmask] = ECricketSlot.BackLeg_l;
			dictionary[ECricketSlot.BackLeg_rmask] = ECricketSlot.BackLeg_r;
			dictionary[ECricketSlot.BackPaw_lmask] = ECricketSlot.BackPaw_l;
			dictionary[ECricketSlot.BackPaw_rmask] = ECricketSlot.BackPaw_r;
			dictionary[ECricketSlot.Leg_lmask] = ECricketSlot.Leg_l;
			dictionary[ECricketSlot.Leg_rmask] = ECricketSlot.Leg_r;
			CricketViewNew.ShowOnlyDefaultSlot = dictionary;
			Dictionary<ECricketSlot, ECricketSlot> dictionary2 = new Dictionary<ECricketSlot, ECricketSlot>();
			dictionary2[ECricketSlot.Trophi_m] = ECricketSlot.Trophi_l;
			CricketViewNew.HideOtherSlot = dictionary2;
			CricketViewNew.TargetColor = Shader.PropertyToID("_TargetColor");
			CricketViewNew.TwinkleColor = Shader.PropertyToID("_TwinkleColor");
			CricketViewNew.TwinkleRate = Shader.PropertyToID("_TwinkleRate");
		}

		// Token: 0x04006773 RID: 26483
		public SkeletonGraphic skeletonGraphic;

		// Token: 0x04006774 RID: 26484
		[SerializeField]
		private CImage singImage;

		// Token: 0x04006775 RID: 26485
		[SerializeField]
		private Shader twinkleShader;

		// Token: 0x04006776 RID: 26486
		private static readonly ECricketSlot[] OptionalSlots = new ECricketSlot[]
		{
			ECricketSlot.Cirrus_l2,
			ECricketSlot.Cirrus_r2,
			ECricketSlot.Trophi_l2,
			ECricketSlot.Trophi_r2,
			ECricketSlot.Trophi_m,
			ECricketSlot.WingLmask,
			ECricketSlot.WingRmask,
			ECricketSlot.Wing_l_1,
			ECricketSlot.Wing_r_1,
			ECricketSlot.WingLmask_1,
			ECricketSlot.WingRmask_1,
			ECricketSlot.Wing_l_2,
			ECricketSlot.Wing_r_2,
			ECricketSlot.WingLmask_2,
			ECricketSlot.WingRmask_2,
			ECricketSlot.MidLeg_l,
			ECricketSlot.MidLeg_r,
			ECricketSlot.MidPaw_l,
			ECricketSlot.MidPaw_r,
			ECricketSlot.MidLeg_lmask,
			ECricketSlot.MidLeg_rmask,
			ECricketSlot.MidPaw_lmask,
			ECricketSlot.MidPaw_rmask
		};

		// Token: 0x04006777 RID: 26487
		private static readonly Dictionary<ECricketSlot, ECricketSlot> ShowOnlyDefaultSlot;

		// Token: 0x04006778 RID: 26488
		private static readonly Dictionary<ECricketSlot, ECricketSlot> HideOtherSlot;

		// Token: 0x04006779 RID: 26489
		[HideInInspector]
		public bool Inited;

		// Token: 0x0400677A RID: 26490
		private CricketPartsItem _colorConfig;

		// Token: 0x0400677B RID: 26491
		private CricketPartsItem _partConfig;

		// Token: 0x0400677C RID: 26492
		private bool _isCombineCricket;

		// Token: 0x0400677D RID: 26493
		private bool _loopSing;

		// Token: 0x04006780 RID: 26496
		[HideInInspector]
		public short ColorId;

		// Token: 0x04006781 RID: 26497
		[HideInInspector]
		public short PartId;

		// Token: 0x04006782 RID: 26498
		[HideInInspector]
		public bool ShowMouseTips;

		// Token: 0x04006783 RID: 26499
		[HideInInspector]
		public bool IsTripleTail;

		// Token: 0x04006784 RID: 26500
		private CImage _singImage;

		// Token: 0x04006785 RID: 26501
		private AudioSource _audioSource;

		// Token: 0x04006786 RID: 26502
		private Shader _normalShader;

		// Token: 0x04006787 RID: 26503
		private Tween _loopAniTween;

		// Token: 0x04006788 RID: 26504
		private Action<float> _onSingStart;

		// Token: 0x04006789 RID: 26505
		private TweenCallback _onSingStartCallBack;

		// Token: 0x0400678A RID: 26506
		private static readonly int TargetColor;

		// Token: 0x0400678B RID: 26507
		private static readonly int TwinkleColor;

		// Token: 0x0400678C RID: 26508
		private static readonly int TwinkleRate;

		// Token: 0x0400678D RID: 26509
		private bool _isStartCombat;

		// Token: 0x0400678E RID: 26510
		private bool _isInCombat;

		// Token: 0x0400678F RID: 26511
		private bool _isOnLeft;

		// Token: 0x02002077 RID: 8311
		private class CricketSkillReplaceRuntime : CommonTipBaseRuntime
		{
			// Token: 0x0600F74C RID: 63308 RVA: 0x00628E7C File Offset: 0x0062707C
			public CricketSkillReplaceRuntime(CommonTipItem configLine, bool hasOriginalSkill) : base(configLine)
			{
				this._hasOriginalSkill = hasOriginalSkill;
			}

			// Token: 0x0600F74D RID: 63309 RVA: 0x00628E9C File Offset: 0x0062709C
			public override string GetArgument(string key)
			{
				string value;
				return this._arguments.TryGetValue(key, out value) ? value : null;
			}

			// Token: 0x0600F74E RID: 63310 RVA: 0x00628EC4 File Offset: 0x006270C4
			public override bool ShouldShowAtom(string paragraphName, string name)
			{
				bool flag = name == "originalSkillAtom" && !this._hasOriginalSkill;
				return !flag && base.ShouldShowAtom(paragraphName, name);
			}

			// Token: 0x0600F74F RID: 63311 RVA: 0x00628F00 File Offset: 0x00627100
			public CricketViewNew.CricketSkillReplaceRuntime Set(string key, string value)
			{
				bool flag = string.IsNullOrEmpty(key);
				CricketViewNew.CricketSkillReplaceRuntime result;
				if (flag)
				{
					result = this;
				}
				else
				{
					bool flag2 = value == null;
					if (flag2)
					{
						this._arguments.Remove(key);
					}
					else
					{
						this._arguments[key] = value;
					}
					result = this;
				}
				return result;
			}

			// Token: 0x0400D129 RID: 53545
			private readonly bool _hasOriginalSkill;

			// Token: 0x0400D12A RID: 53546
			private readonly Dictionary<string, string> _arguments = new Dictionary<string, string>();
		}
	}
}
