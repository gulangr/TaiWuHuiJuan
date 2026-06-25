using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class CricketView : Refers
{
	// Token: 0x06002087 RID: 8327 RVA: 0x000ECC10 File Offset: 0x000EAE10
	private static bool CheckIsMatch(string idStr, string attach)
	{
		bool flag = attach.EndsWith(idStr) && CricketView.CheckIsMatchPrefix(attach, attach.Length - idStr.Length);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			int index = attach.IndexOf(idStr, StringComparison.Ordinal);
			bool flag2 = index + idStr.Length >= attach.Length;
			result = (!flag2 && CricketView.CheckIsMatchPrefix(attach, index) && attach[index + idStr.Length] == '_');
		}
		return result;
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000ECC8C File Offset: 0x000EAE8C
	private static bool CheckIsMatchPrefix(string attach, int index)
	{
		bool flag = index < 2;
		return !flag && attach[index - 1] == '_' && !char.IsDigit(attach[index - 2]);
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06002089 RID: 8329 RVA: 0x000ECCCB File Offset: 0x000EAECB
	// (set) Token: 0x0600208A RID: 8330 RVA: 0x000ECCD3 File Offset: 0x000EAED3
	public bool IsSinging { get; private set; }

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x0600208B RID: 8331 RVA: 0x000ECCDC File Offset: 0x000EAEDC
	// (set) Token: 0x0600208C RID: 8332 RVA: 0x000ECCE4 File Offset: 0x000EAEE4
	public ITradeableContent ItemData { get; private set; }

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x0600208D RID: 8333 RVA: 0x000ECCED File Offset: 0x000EAEED
	public string Name
	{
		get
		{
			return new ValueTuple<short, short>(this.ColorId, this.PartId).CalcCricketName();
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x0600208E RID: 8334 RVA: 0x000ECD05 File Offset: 0x000EAF05
	public int Level
	{
		get
		{
			return this._isCombineCricket ? Mathf.Max((int)this._colorConfig.Level, (int)this._partConfig.Level) : ((int)this._colorConfig.Level);
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x0600208F RID: 8335 RVA: 0x000ECD38 File Offset: 0x000EAF38
	public int Value
	{
		get
		{
			ITradeableContent itemData = this.ItemData;
			long? num = (itemData != null) ? new long?(itemData.Value) : null;
			return ((num != null) ? new int?((int)num.GetValueOrDefault()) : null) ?? (this._isCombineCricket ? Mathf.Max(this._colorConfig.Value, this._partConfig.Value) : this._colorConfig.Value);
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06002090 RID: 8336 RVA: 0x000ECDC9 File Offset: 0x000EAFC9
	public int SingSize
	{
		get
		{
			return (int)(this._isCombineCricket ? (this._colorConfig.SingSize + this._partConfig.SingSize) : this._colorConfig.SingSize);
		}
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06002091 RID: 8337 RVA: 0x000ECDF7 File Offset: 0x000EAFF7
	public int SingPitch
	{
		get
		{
			return (int)(this._isCombineCricket ? (this._colorConfig.SingPitch + this._partConfig.SingPitch) : this._colorConfig.SingPitch);
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06002092 RID: 8338 RVA: 0x000ECE25 File Offset: 0x000EB025
	public SkeletonGraphic SkeletonGraphic
	{
		get
		{
			return this._skeletonGraphic;
		}
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000ECE30 File Offset: 0x000EB030
	public void SetSingImagePositionToZero()
	{
		bool flag = this._singImage != null;
		if (flag)
		{
			this._singImage.rectTransform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x000ECE64 File Offset: 0x000EB064
	private void OnDisable()
	{
		AudioManager.Instance.RemoveAudioSource(this._audioSource);
		bool flag = this._loopAniTween != null && this._loopAniTween.IsActive();
		if (flag)
		{
			this._loopAniTween.Kill(false);
		}
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x000ECEAC File Offset: 0x000EB0AC
	private void OnDestroy()
	{
		bool flag = this._loopAniTween != null && this._loopAniTween.IsActive();
		if (flag)
		{
			this._loopAniTween.Kill(false);
		}
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x000ECEE4 File Offset: 0x000EB0E4
	public void SetCricketData(short colorId, short partId, bool showMouseTips = false, ITradeableContent itemData = null, bool isTripleTail = false)
	{
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		this.ColorId = colorId;
		this.PartId = partId;
		this.ShowMouseTips = showMouseTips;
		this.ItemData = itemData;
		this.IsTripleTail = isTripleTail;
		bool flag = !this.Inited;
		if (flag)
		{
			this._skeletonGraphic = base.CGet<SkeletonGraphic>("SkeletonGraphic");
			this._singImage = base.CGet<CImage>("SingImage");
			this._audioSource = base.GetComponent<AudioSource>();
			this._skeletonGraphic.material = new UnityEngine.Material(this._skeletonGraphic.material);
			this._normalShader = this._skeletonGraphic.material.shader;
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
		this._skeletonGraphic.Initialize(false);
		short partTemplateId = (this._isCombineCricket ? this._partConfig : this._colorConfig).TemplateId;
		Skeleton skeleton = this._skeletonGraphic.Skeleton;
		ICollection<Skin.SkinEntry> attachmentDict = skeleton.Data.DefaultSkin.Attachments;
		List<ECricketSlot> showDefaultSlotList = EasyPool.Get<List<ECricketSlot>>();
		this._skeletonGraphic.color = this._skeletonGraphic.color.SetAlpha(1f);
		this._skeletonGraphic.material.SetColor(CricketView.TargetColor, this._isCombineCricket ? this._colorConfig.Color.HexStringToColor() : Color.white);
		showDefaultSlotList.Clear();
		string idString = partTemplateId.ToString();
		for (ECricketSlot slotType = ECricketSlot.Head; slotType < ECricketSlot.Count; slotType++)
		{
			string slotName = slotType.ToString();
			bool attachmentExist = false;
			bool defaultAttachmentExist = false;
			string attachmentName = null;
			string defaultAttachment = null;
			int slotIndex = skeleton.Data.FindSlot(slotName).Index;
			foreach (Skin.SkinEntry entry in attachmentDict)
			{
				bool flag2 = entry.SlotIndex == slotIndex;
				if (flag2)
				{
					string attachName = entry.Name;
					bool flag3 = CricketView.CheckIsMatch(idString, attachName);
					if (flag3)
					{
						attachmentExist = true;
						attachmentName = attachName;
						break;
					}
					bool flag4 = attachName.Contains("_c");
					if (flag4)
					{
						defaultAttachmentExist = true;
						defaultAttachment = attachName;
					}
				}
			}
			bool showSlot = Array.IndexOf<ECricketSlot>(CricketView.OptionalSlots, slotType) < 0 || attachmentExist;
			bool flag5 = !attachmentExist && defaultAttachmentExist;
			if (flag5)
			{
				showDefaultSlotList.Add(slotType);
			}
			bool flag6 = CricketView.ShowOnlyDefaultSlot.ContainsKey(slotType) && !showDefaultSlotList.Contains(CricketView.ShowOnlyDefaultSlot[slotType]);
			if (flag6)
			{
				showSlot = false;
			}
			skeleton.FindSlot(slotName).A = (float)(showSlot ? 1 : 0);
			bool flag7 = showSlot;
			if (flag7)
			{
				bool flag8 = attachmentExist;
				if (flag8)
				{
					skeleton.SetAttachment(slotName, attachmentName);
				}
				else
				{
					bool flag9 = defaultAttachmentExist;
					if (flag9)
					{
						skeleton.SetAttachment(slotName, defaultAttachment);
					}
				}
				bool flag10 = CricketView.HideOtherSlot.ContainsKey(slotType);
				if (flag10)
				{
					skeleton.FindSlot(CricketView.HideOtherSlot[slotType].ToString()).A = 0f;
					showDefaultSlotList.Remove(CricketView.HideOtherSlot[slotType]);
				}
			}
		}
		EasyPool.Free<List<ECricketSlot>>(showDefaultSlotList);
		skeleton.FindSlot(ECricketSlot.Cercus_m.ToString()).A = (float)(isTripleTail ? 1 : 0);
		TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
		tipDisplayer.enabled = showMouseTips;
		base.GetComponent<CEmptyGraphic>().enabled = showMouseTips;
		if (showMouseTips)
		{
			tipDisplayer.Type = (isTripleTail ? TipType.SingleDesc : TipType.Cricket);
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			if (isTripleTail)
			{
				tipDisplayer.RuntimeParam.SetObject("arg0", Misc.Instance[34].Name);
			}
			else
			{
				tipDisplayer.RuntimeParam.SetObject("ItemData", itemData);
			}
			tipDisplayer.Refresh(false, -1);
		}
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x000ED3CC File Offset: 0x000EB5CC
	public void SetCricketData(CricketView other)
	{
		this.SetCricketData(other.ColorId, other.PartId, other.ShowMouseTips, other.ItemData, other.IsTripleTail);
	}

	// Token: 0x06002098 RID: 8344 RVA: 0x000ED3F4 File Offset: 0x000EB5F4
	public void PlayAnimation(ECricketAnim ani, bool loop = false, bool restart = false)
	{
		bool flag = base.gameObject == null || !base.gameObject.activeInHierarchy;
		if (!flag)
		{
			string aniName = ani.ToString().ToLower();
			TrackEntry currEntry = this._skeletonGraphic.AnimationState.GetCurrent(0);
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
				this._skeletonGraphic.AnimationState.SetAnimation(0, aniName, loop);
			}
		}
	}

	// Token: 0x06002099 RID: 8345 RVA: 0x000ED4E1 File Offset: 0x000EB6E1
	public void StopAnimation()
	{
		Tween loopAniTween = this._loopAniTween;
		if (loopAniTween != null)
		{
			loopAniTween.Kill(false);
		}
		this._skeletonGraphic.AnimationState.SetEmptyAnimation(0, 0f);
		this._skeletonGraphic.Skeleton.SetBonesToSetupPose();
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x000ED520 File Offset: 0x000EB720
	public float GetAnimationDuration(ECricketAnim animType)
	{
		Spine.Animation animation = this._skeletonGraphic.Skeleton.Data.FindAnimation(animType.ToString().ToLower());
		return (animation != null) ? animation.Duration : 0f;
	}

	// Token: 0x0600209B RID: 8347 RVA: 0x000ED56C File Offset: 0x000EB76C
	public float GetEventTime(ECricketAnim ani, string evtName)
	{
		float time = -1f;
		ExposedList<Timeline> timeLineList = this._skeletonGraphic.Skeleton.Data.FindAnimation(ani.ToString().ToLower()).Timelines;
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

	// Token: 0x0600209C RID: 8348 RVA: 0x000ED65C File Offset: 0x000EB85C
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

	// Token: 0x0600209D RID: 8349 RVA: 0x000ED96F File Offset: 0x000EBB6F
	public void StopLoopSing()
	{
		this._loopSing = false;
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x000ED97C File Offset: 0x000EBB7C
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

	// Token: 0x0600209F RID: 8351 RVA: 0x000ED9E8 File Offset: 0x000EBBE8
	public void PauseSing()
	{
		bool isSinging = this.IsSinging;
		if (isSinging)
		{
			this._audioSource.Pause();
		}
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x000EDA0C File Offset: 0x000EBC0C
	public void ResumeSing()
	{
		bool isSinging = this.IsSinging;
		if (isSinging)
		{
			this._audioSource.Play();
		}
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000EDA30 File Offset: 0x000EBC30
	public void Twinkle()
	{
		this._skeletonGraphic.material.shader = base.CGet<Shader>("TwinkleShader");
		this._skeletonGraphic.material.SetColor(CricketView.TwinkleColor, Color.white.SetAlpha(0.5f));
		DOVirtual.Float(0f, 2f, 0.1f, delegate(float stepValue)
		{
			float twinkleRate = (stepValue < 1f) ? Mathf.Lerp(0.5f, 1.2f, stepValue) : Mathf.Lerp(1.2f, 0.5f, stepValue - 1f);
			this._skeletonGraphic.material.SetFloat(CricketView.TwinkleRate, twinkleRate);
		}).SetAutoKill(true);
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
		{
			this._skeletonGraphic.material.shader = this._normalShader;
		});
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x000EDAC2 File Offset: 0x000EBCC2
	public void Fade(bool fadeIn)
	{
		this._skeletonGraphic.DOKill(false);
		this._skeletonGraphic.DOFade((float)(fadeIn ? 1 : 0), 1f).SetEase(Ease.InCirc);
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x000EDAF2 File Offset: 0x000EBCF2
	public void SetSoundData(bool isStartCombat, bool isInCombat, bool isOnLeft)
	{
		this._isStartCombat = isStartCombat;
		this._isInCombat = isInCombat;
		this._isOnLeft = isOnLeft;
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000EDB14 File Offset: 0x000EBD14
	// Note: this type is marked as 'beforefieldinit'.
	static CricketView()
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
		CricketView.ShowOnlyDefaultSlot = dictionary;
		Dictionary<ECricketSlot, ECricketSlot> dictionary2 = new Dictionary<ECricketSlot, ECricketSlot>();
		dictionary2[ECricketSlot.Trophi_m] = ECricketSlot.Trophi_l;
		CricketView.HideOtherSlot = dictionary2;
		CricketView.TargetColor = Shader.PropertyToID("_TargetColor");
		CricketView.TwinkleColor = Shader.PropertyToID("_TwinkleColor");
		CricketView.TwinkleRate = Shader.PropertyToID("_TwinkleRate");
	}

	// Token: 0x04001890 RID: 6288
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

	// Token: 0x04001891 RID: 6289
	private static readonly Dictionary<ECricketSlot, ECricketSlot> ShowOnlyDefaultSlot;

	// Token: 0x04001892 RID: 6290
	private static readonly Dictionary<ECricketSlot, ECricketSlot> HideOtherSlot;

	// Token: 0x04001893 RID: 6291
	[HideInInspector]
	public bool Inited;

	// Token: 0x04001894 RID: 6292
	private CricketPartsItem _colorConfig;

	// Token: 0x04001895 RID: 6293
	private CricketPartsItem _partConfig;

	// Token: 0x04001896 RID: 6294
	private bool _isCombineCricket;

	// Token: 0x04001897 RID: 6295
	private bool _loopSing;

	// Token: 0x0400189A RID: 6298
	[HideInInspector]
	public short ColorId;

	// Token: 0x0400189B RID: 6299
	[HideInInspector]
	public short PartId;

	// Token: 0x0400189C RID: 6300
	[HideInInspector]
	public bool ShowMouseTips;

	// Token: 0x0400189D RID: 6301
	[HideInInspector]
	public bool IsTripleTail;

	// Token: 0x0400189E RID: 6302
	private SkeletonGraphic _skeletonGraphic;

	// Token: 0x0400189F RID: 6303
	private CImage _singImage;

	// Token: 0x040018A0 RID: 6304
	private AudioSource _audioSource;

	// Token: 0x040018A1 RID: 6305
	private Shader _normalShader;

	// Token: 0x040018A2 RID: 6306
	private Tween _loopAniTween;

	// Token: 0x040018A3 RID: 6307
	private Action<float> _onSingStart;

	// Token: 0x040018A4 RID: 6308
	private TweenCallback _onSingStartCallBack;

	// Token: 0x040018A5 RID: 6309
	private static readonly int TargetColor;

	// Token: 0x040018A6 RID: 6310
	private static readonly int TwinkleColor;

	// Token: 0x040018A7 RID: 6311
	private static readonly int TwinkleRate;

	// Token: 0x040018A8 RID: 6312
	private bool _isStartCombat;

	// Token: 0x040018A9 RID: 6313
	private bool _isInCombat;

	// Token: 0x040018AA RID: 6314
	private bool _isOnLeft;
}
