using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003B2 RID: 946
public class UI_ThreeVitals : UIBase
{
	// Token: 0x170005CC RID: 1484
	// (get) Token: 0x060038BB RID: 14523 RVA: 0x001C9F89 File Offset: 0x001C8189
	private GameObject Mask
	{
		get
		{
			return base.CGet<GameObject>("Mask");
		}
	}

	// Token: 0x060038BC RID: 14524 RVA: 0x001C9F98 File Offset: 0x001C8198
	public override void OnInit(ArgumentBox argsBox)
	{
		this._selectVitalIndex = 0;
		if (this._threeVitalsRefersList == null)
		{
			this._threeVitalsRefersList = base.CGetList<Refers>("ThreeVitalsChar");
		}
		this.NeedDataListenerId = true;
		foreach (Refers refers in this._threeVitalsRefersList)
		{
			refers.CGet<Bubble>("Bubble").Clear();
		}
		ExtraDomainMethod.AsyncCall.AreVitalsDemon(this, delegate(int offset, RawDataPool pool)
		{
			bool areVitalsDemon = false;
			Serializer.Deserialize(pool, offset, ref areVitalsDemon);
			this._isGoodEnd = !areVitalsDemon;
			string targetAmbience = this._isGoodEnd ? "sancai_ambience" : "sanmo_ambience";
			string currentAmbience = AudioManager.Instance.GetPlayingAmbience();
			bool flag = targetAmbience == currentAmbience;
			if (flag)
			{
				AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0f, 100);
			}
			AudioManager.Instance.PlayAmbience(targetAmbience, 1f, 100);
		});
		ExtraDomainMethod.AsyncCall.GetThreeVitalsCharDataList(this, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._threeVitalsCharDataList);
		});
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.SendRefresh));
	}

	// Token: 0x060038BD RID: 14525 RVA: 0x001CA070 File Offset: 0x001C8270
	private void OnEnable()
	{
		GEvent.Add(UiEvents.PlayVitalAnim, new GEvent.Callback(this.PlayVitalAnim));
	}

	// Token: 0x060038BE RID: 14526 RVA: 0x001CA08F File Offset: 0x001C828F
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.PlayVitalAnim, new GEvent.Callback(this.PlayVitalAnim));
		AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0.2f, 100);
	}

	// Token: 0x060038BF RID: 14527 RVA: 0x001CA0C8 File Offset: 0x001C82C8
	private void SendRefresh()
	{
		base.RemoveMonitorFieldId(19, 189);
		UIBase.MonitorDataField monitorDataField = new UIBase.MonitorDataField(19, 189, ulong.MaxValue, null);
		base.AppendMonitorFieldId(monitorDataField);
	}

	// Token: 0x060038C0 RID: 14528 RVA: 0x001CA100 File Offset: 0x001C8300
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				bool flag = notification.Uid.DomainId == 19 && notification.Uid.DataId == 189;
				if (flag)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._threeVitalsDataList);
					ExtraDomainMethod.AsyncCall.GetThreeVitalsTargetCharDataList(this, delegate(int offset, RawDataPool pool)
					{
						Serializer.Deserialize(pool, offset, ref this._targetCharDataList);
						this.Refresh();
						this.Element.ShowAfterRefresh();
					});
				}
			}
		}
	}

	// Token: 0x060038C1 RID: 14529 RVA: 0x001CA1C0 File Offset: 0x001C83C0
	private void Refresh()
	{
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Title_Good : LanguageKey.LK_ThreeVitals_Title_Bad);
		int helpCharCount = this._threeVitalsDataList.Count((SectStoryThreeVitalsCharacter d) => UI_ThreeVitals.IsVitalHelping(this._isGoodEnd, d.Infection));
		if (!true)
		{
		}
		string text;
		if (helpCharCount != 0)
		{
			if (helpCharCount != 1)
			{
				text = (this._isGoodEnd ? "popup_SelectRemainOfTeacher_base_3" : "popup_SelectRemainOfTeacher_base_6");
			}
			else
			{
				text = (this._isGoodEnd ? "popup_SelectRemainOfTeacher_base_2" : "popup_SelectRemainOfTeacher_base_5");
			}
		}
		else
		{
			text = (this._isGoodEnd ? "popup_SelectRemainOfTeacher_base_1" : "popup_SelectRemainOfTeacher_base_4");
		}
		if (!true)
		{
		}
		string backImage = text;
		ResLoader.LoadByName<Texture2D>(backImage, delegate(Texture2D t)
		{
			base.CGet<CRawImage>("Back").texture = t;
		}, null);
		for (int i = 0; i < this._threeVitalsRefersList.Count; i++)
		{
			this.RefreshChar(i, this._threeVitalsRefersList[i], this._threeVitalsDataList[i]);
		}
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x001CA2BC File Offset: 0x001C84BC
	private void RefreshChar(int index, Refers refers, SectStoryThreeVitalsCharacter vitalData)
	{
		CharacterDisplayData charData = this._threeVitalsCharDataList[index];
		bool isActive = !vitalData.IsInPrison && vitalData.HasPlayedComeAnim;
		SkeletonGraphic skeletonGraphic = refers.CGet<SkeletonGraphic>(this._isGoodEnd ? "GoodThreeVitalsState" : "BadThreeVitalsState");
		skeletonGraphic.gameObject.SetActive(isActive);
		refers.CGet<SkeletonGraphic>((!this._isGoodEnd) ? "GoodThreeVitalsState" : "BadThreeVitalsState").gameObject.SetActive(false);
		if (!true)
		{
		}
		int index2 = index;
		string text;
		if (index2 != 0)
		{
			if (index2 != 1)
			{
				text = (this._isGoodEnd ? "rencai" : "renmo");
			}
			else
			{
				text = (this._isGoodEnd ? "dicai" : "dimo");
			}
		}
		else
		{
			text = (this._isGoodEnd ? "tiancai" : "tianmo");
		}
		if (!true)
		{
		}
		string skeletonDataName = text;
		string path = "RemakeResources/Particle/UIEffectPrefabs/YuanshanThreeDemon/" + skeletonDataName + "_SkeletonData";
		bool flag = !this._isInAnim[index];
		if (flag)
		{
			skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
		}
		refers.CGet<TextMeshProUGUI>("NameText").text = (vitalData.IsInPrison ? LocalStringManager.Get(LanguageKey.LK_ThreeVitals_NoVital).SetColor("grey") : NameCenter.GetMonasticTitleOrDisplayName(charData, false));
		string progressLightImage = this._isGoodEnd ? "popup_SelectRemainOfTeacher_numberluminous_blue" : "popup_SelectRemainOfTeacher_numberluminous_red";
		refers.CGet<CImage>("ProgressLight").SetSprite(progressLightImage, false, null);
		string baseImage = this._isGoodEnd ? ((!isActive) ? "popup_SelectRemainOfTeacher_progressbar_base_3" : "popup_SelectRemainOfTeacher_progressbar_base_2") : ((!isActive) ? "popup_SelectRemainOfTeacher_progressbar_base_1" : "popup_SelectRemainOfTeacher_progressbar_base_0");
		refers.CGet<CImage>("Base").SetSprite(baseImage, false, null);
		bool flag2 = !this._isInAnim[index];
		if (flag2)
		{
			refers.CGet<GameObject>("EffectLightGood").transform.parent.gameObject.SetActive(isActive);
			refers.CGet<GameObject>("EffectLightGood").SetActive(this._isGoodEnd && isActive);
			refers.CGet<GameObject>("EffectLightBad").SetActive(!this._isGoodEnd && isActive);
		}
		refers.CGet<GameObject>("BadLink").SetActive(!this._isGoodEnd);
		SkeletonGraphic badVitalState = refers.CGet<SkeletonGraphic>("BadVitalState");
		badVitalState.gameObject.SetActive(!this._isGoodEnd);
		string badCharIdleAnimName = (!isActive) ? "idle_2" : "idle_1";
		badVitalState.AnimationState.SetAnimation(0, badCharIdleAnimName, true);
		SkeletonGraphic goodVitalState = refers.CGet<SkeletonGraphic>("GoodVitalState");
		goodVitalState.gameObject.SetActive(this._isGoodEnd);
		string goodCharIdleAnimName = (!isActive) ? "idle_1" : "idle_2";
		goodVitalState.AnimationState.SetAnimation(0, goodCharIdleAnimName, true);
		this.RefreshCharProgress(refers, vitalData);
		this.RefreshCharButton(index, refers, vitalData, true);
		this.RefreshCharButton(index, refers, vitalData, false);
		CButtonObsolete buttonDeport = refers.CGet<CButtonObsolete>("ButtonDeport");
		buttonDeport.interactable = isActive;
		buttonDeport.ClearAndAddListener(delegate
		{
			TaiwuEventDomainMethod.Call.OnClickDeportButton(index, this._isGoodEnd);
		});
		TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
		tip.enabled = isActive;
		bool enabled = tip.enabled;
		if (enabled)
		{
			tip.Type = TipType.ThreeVitals;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("isGoodEnd", this._isGoodEnd).SetObject("vitalData", vitalData);
		}
		bool flag3 = !vitalData.HasPlayedComeAnim && !this._isInAnim[index];
		if (flag3)
		{
			this.PlayVitalComeAnim(index);
		}
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x001CA698 File Offset: 0x001C8898
	private void PlayInfectAnim(int index, int lastInfection, Action onEnd)
	{
		this.Mask.SetActive(true);
		SectStoryThreeVitalsCharacter vitalData = this._threeVitalsDataList[index];
		Refers refers = this._threeVitalsRefersList[index];
		int maxInfection = GlobalConfig.Instance.ThreeVitalsMaxInfection;
		float infectionRate = (float)vitalData.Infection / (float)maxInfection;
		float fillAmount = vitalData.IsInPrison ? 0f : Mathf.Lerp(this.progressMinFillAmountBottom, this.progressMaxFillAmountBottom, infectionRate);
		float lastInfectionRate = (float)lastInfection / (float)maxInfection;
		YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
		SkeletonGraphic skeletonGraphic = refers.CGet<SkeletonGraphic>(this._isGoodEnd ? "GoodThreeVitalsState" : "BadThreeVitalsState");
		Array.Fill<bool>(this._isInAnim, true);
		skeletonGraphic.AnimationState.SetAnimation(0, "intake", false).Complete += delegate(TrackEntry entry)
		{
			skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
			Array.Fill<bool>(this._isInAnim, false);
		};
		bool isGoodEnd = this._isGoodEnd;
		if (isGoodEnd)
		{
			SkeletonGraphic goodVitalState = refers.CGet<SkeletonGraphic>("GoodVitalState");
			ParticleSystem itemParticle = refers.CGet<ParticleSystem>("EffectGoodTreeCreate");
			itemParticle.gameObject.SetActive(true);
			yieldHelper.DelaySecondsDo(5f, delegate
			{
				bool flag2 = itemParticle != null;
				if (flag2)
				{
					itemParticle.gameObject.SetActive(false);
				}
			});
		}
		else
		{
			SkeletonGraphic badVitalState = refers.CGet<SkeletonGraphic>("BadVitalState");
			badVitalState.AnimationState.SetAnimation(0, "intake", true);
		}
		GameObject effectButton = refers.CGet<GameObject>("EffectButton");
		GameObject effectBadLink = refers.CGet<GameObject>("EffectBadLink");
		bool flag = !this._isGoodEnd;
		if (flag)
		{
			DG.Tweening.Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				effectButton.SetActive(true);
			});
			sequence.AppendInterval(0.2f);
			sequence.AppendCallback(delegate
			{
				effectBadLink.SetActive(true);
			});
			sequence.AppendInterval(0.5f);
			TweenCallback<float> <>9__6;
			sequence.AppendCallback(delegate
			{
				this.RefreshHalfFill(refers, fillAmount, 1f);
				float lastInfectionRate = lastInfectionRate;
				float infectionRate = infectionRate;
				float duration = 0.2f;
				TweenCallback<float> onVirtualUpdate;
				if ((onVirtualUpdate = <>9__6) == null)
				{
					onVirtualUpdate = (<>9__6 = delegate(float rate)
					{
						this.RefreshProgressText(refers, rate);
					});
				}
				DOVirtual.Float(lastInfectionRate, infectionRate, duration, onVirtualUpdate);
				effectButton.SetActive(false);
				effectBadLink.SetActive(false);
				float pitch = this._isGoodEnd ? 0.98f : 1.17f;
				AudioManager.Instance.Play(new AudioCommand
				{
					AudioName = (this._isGoodEnd ? "sancai_progress" : "sanmo_progress"),
					Loop = false,
					CanSetPitchByGlobal = true,
					AudioType = SEType.Sound,
					Pitch = pitch
				});
			});
			sequence.AppendInterval(0.2f);
			sequence.AppendCallback(delegate
			{
				Action onEnd2 = onEnd;
				if (onEnd2 != null)
				{
					onEnd2();
				}
				this.Mask.SetActive(false);
			});
			sequence.Play<DG.Tweening.Sequence>();
		}
		else
		{
			DG.Tweening.Sequence sequence2 = DOTween.Sequence();
			TweenCallback<float> <>9__10;
			sequence2.AppendCallback(delegate
			{
				this.RefreshHalfFill(refers, fillAmount, 1f);
				float lastInfectionRate = lastInfectionRate;
				float infectionRate = infectionRate;
				float duration = 0.2f;
				TweenCallback<float> onVirtualUpdate;
				if ((onVirtualUpdate = <>9__10) == null)
				{
					onVirtualUpdate = (<>9__10 = delegate(float rate)
					{
						this.RefreshProgressText(refers, rate);
					});
				}
				DOVirtual.Float(lastInfectionRate, infectionRate, duration, onVirtualUpdate);
				effectButton.SetActive(false);
				effectBadLink.SetActive(false);
			});
			sequence2.AppendInterval(0.2f);
			sequence2.AppendInterval(0.5f);
			sequence2.AppendCallback(delegate
			{
				effectButton.SetActive(true);
			});
			sequence2.AppendInterval(0.2f);
			sequence2.AppendCallback(delegate
			{
				Action onEnd2 = onEnd;
				if (onEnd2 != null)
				{
					onEnd2();
				}
				this.Mask.SetActive(false);
			});
			sequence2.Play<DG.Tweening.Sequence>();
		}
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x001CA96C File Offset: 0x001C8B6C
	private void PlayVitalLeaveAnim(int index)
	{
		for (int i = 0; i < this._threeVitalsDataList.Count; i++)
		{
			bool flag = i != index && !this._threeVitalsDataList[i].IsInPrison;
			if (flag)
			{
				this.Speak(this._threeVitalsRefersList[i], this.GetBanishOtherSpeakContent(this._threeVitalsDataList[i].VitalType, this._isGoodEnd));
				this._threeVitalsRefersList[i].CGet<SkeletonGraphic>(this._isGoodEnd ? "GoodThreeVitalsState" : "BadThreeVitalsState").AnimationState.SetAnimation(0, "talk", false);
			}
		}
		AudioManager.Instance.PlaySound(this._isGoodEnd ? "sancai_disappear" : "sanmo_disappear", false, false);
		this._isInAnim[index] = true;
		this._selectVitalIndex = index;
		SectStoryThreeVitalsCharacter vitalData = this._threeVitalsDataList[index];
		Refers refers = this._threeVitalsRefersList[index];
		SkeletonGraphic skeletonGraphic = refers.CGet<SkeletonGraphic>(this._isGoodEnd ? "GoodThreeVitalsState" : "BadThreeVitalsState");
		string vitalEffectName = string.Format("{0}EffectVitalLeave{1}", this._isGoodEnd ? "Good" : "Bad", index);
		ParticleSystem vitalParticle = refers.CGet<ParticleSystem>(vitalEffectName);
		skeletonGraphic.AnimationState.SetAnimation(0, "out", false);
		vitalParticle.gameObject.SetActive(true);
		string itemEffectName = this._isGoodEnd ? "EffectGoodTreeDestroy" : "EffectBadLinkDestroy";
		ParticleSystem itemParticle = refers.CGet<ParticleSystem>(itemEffectName);
		itemParticle.gameObject.SetActive(true);
		this.Mask.SetActive(true);
		bool isGoodEnd = this._isGoodEnd;
		if (isGoodEnd)
		{
			SkeletonGraphic goodVitalState = refers.CGet<SkeletonGraphic>("GoodVitalState");
			goodVitalState.AnimationState.SetAnimation(0, "idle_1", true);
		}
		else
		{
			SkeletonGraphic badVitalState = refers.CGet<SkeletonGraphic>("BadVitalState");
			badVitalState.AnimationState.SetAnimation(0, "leave", false);
		}
		this.RefreshHalfFade(refers, vitalParticle.main.duration, true);
		base.StartCoroutine(this.SetTragetObjFade(refers.CGet<GameObject>(this._isGoodEnd ? "EffectLightGood" : "EffectLightBad"), false, vitalParticle.main.duration - 0.8f, 0.8f));
		DOVirtual.DelayedCall(skeletonGraphic.Skeleton.Data.FindAnimation("talk").Duration, delegate
		{
			ExtraDomainMethod.Call.SetVitalInPrison(vitalData.VitalType, true);
			this.Mask.SetActive(false);
			itemParticle.gameObject.SetActive(false);
			vitalParticle.gameObject.SetActive(false);
			skeletonGraphic.gameObject.SetActive(false);
			this._isInAnim[index] = false;
		}, true);
	}

	// Token: 0x060038C5 RID: 14533 RVA: 0x001CAC5C File Offset: 0x001C8E5C
	private string GetBanishOtherSpeakContent(SectStoryThreeVitalsCharacterType vitalType, bool isGoodEnd)
	{
		bool flag = vitalType == SectStoryThreeVitalsCharacterType.Heaven;
		LanguageKey languageKey;
		if (flag)
		{
			languageKey = (isGoodEnd ? LanguageKey.LK_ThreeVitals_Speak_BanishOther_EssenceSky : LanguageKey.LK_ThreeVitals_Speak_BanishOther_DemonSky);
		}
		else
		{
			bool flag2 = vitalType == SectStoryThreeVitalsCharacterType.Earth;
			if (flag2)
			{
				languageKey = (isGoodEnd ? LanguageKey.LK_ThreeVitals_Speak_BanishOther_EssenceEarth : LanguageKey.LK_ThreeVitals_Speak_BanishOther_DemonEarth);
			}
			else
			{
				languageKey = (isGoodEnd ? LanguageKey.LK_ThreeVitals_Speak_BanishOther_EssenceHuman : LanguageKey.LK_ThreeVitals_Speak_BanishOther_DemonHuman);
			}
		}
		return global::StringKey.CreateKey(languageKey).GetString();
	}

	// Token: 0x060038C6 RID: 14534 RVA: 0x001CACC8 File Offset: 0x001C8EC8
	private string GetTransferInfectionSpeakContent(SectStoryThreeVitalsCharacterType vitalType, bool isGoodEnd)
	{
		bool flag = vitalType == SectStoryThreeVitalsCharacterType.Heaven;
		LanguageKey languageKey;
		if (flag)
		{
			languageKey = (isGoodEnd ? LanguageKey.LK_ThreeVitals_Speak_Giveaway_Sky : LanguageKey.LK_ThreeVitals_Speak_Drain_Sky);
		}
		else
		{
			bool flag2 = vitalType == SectStoryThreeVitalsCharacterType.Earth;
			if (flag2)
			{
				languageKey = (isGoodEnd ? LanguageKey.LK_ThreeVitals_Speak_Giveaway_Earth : LanguageKey.LK_ThreeVitals_Speak_Drain_Earth);
			}
			else
			{
				languageKey = (isGoodEnd ? LanguageKey.LK_ThreeVitals_Speak_Giveaway_Human : LanguageKey.LK_ThreeVitals_Speak_Drain_Human);
			}
		}
		return global::StringKey.CreateKey(languageKey).GetString();
	}

	// Token: 0x060038C7 RID: 14535 RVA: 0x001CAD34 File Offset: 0x001C8F34
	private void PlayVitalAnim(ArgumentBox box)
	{
		int type;
		box.Get("Type", out type);
		bool isInPrison;
		box.Get("IsInPrison", out isInPrison);
		bool flag = isInPrison;
		if (flag)
		{
			this.PlayVitalLeaveAnim(type);
		}
		else
		{
			this.PlayVitalComeAnim(type);
		}
	}

	// Token: 0x060038C8 RID: 14536 RVA: 0x001CAD78 File Offset: 0x001C8F78
	private void PlayVitalComeAnim(int index)
	{
		this._isInAnim[index] = true;
		SectStoryThreeVitalsCharacter vitalData = this._threeVitalsDataList[index];
		Refers refers = this._threeVitalsRefersList[index];
		string itemEffectName = this._isGoodEnd ? "EffectGoodTreeCreate" : "EffectBadLinkCreate";
		ParticleSystem itemParticle = refers.CGet<ParticleSystem>(itemEffectName);
		bool flag = !this._isGoodEnd;
		if (flag)
		{
			itemParticle.gameObject.SetActive(true);
		}
		string vitalEffectName = string.Format("{0}EffectVitalCome{1}", this._isGoodEnd ? "Good" : "Bad", index);
		ParticleSystem vitalParticle = refers.CGet<ParticleSystem>(vitalEffectName);
		vitalParticle.gameObject.SetActive(true);
		base.StartCoroutine(this.SetTragetObjFade(refers.CGet<GameObject>(this._isGoodEnd ? "EffectLightGood" : "EffectLightBad"), true, vitalParticle.main.duration - 0.8f, 0.8f));
		this.Mask.SetActive(true);
		bool isGoodEnd = this._isGoodEnd;
		if (isGoodEnd)
		{
			SkeletonGraphic goodVitalState = refers.CGet<SkeletonGraphic>("GoodVitalState");
			goodVitalState.AnimationState.SetAnimation(0, "grow", false);
			AudioManager.Instance.PlaySound("sancai_appear", false, false);
		}
		else
		{
			SkeletonGraphic badVitalState = refers.CGet<SkeletonGraphic>("BadVitalState");
			badVitalState.AnimationState.SetAnimation(0, "remove", false);
			AudioManager.Instance.PlaySound("sanmo_appear", false, false);
		}
		SkeletonGraphic skeletonGraphic = refers.CGet<SkeletonGraphic>(this._isGoodEnd ? "GoodThreeVitalsState" : "BadThreeVitalsState");
		skeletonGraphic.gameObject.SetActive(true);
		skeletonGraphic.AnimationState.SetAnimation(0, "in", false);
		this.RefreshHalfFade(refers, vitalParticle.main.duration, false);
		int maxInfection = GlobalConfig.Instance.ThreeVitalsMaxInfection;
		float infectionRate = (float)vitalData.Infection / (float)maxInfection;
		float fillAmount = Mathf.Lerp(this.progressMinFillAmountBottom, this.progressMaxFillAmountBottom, infectionRate);
		this.RefreshHalfFill(refers, fillAmount, vitalParticle.main.duration);
		DOVirtual.DelayedCall(vitalParticle.main.duration, delegate
		{
			ExtraDomainMethod.Call.SetVitalHasPlayedComeAnim(vitalData.VitalType, true);
			this.Mask.SetActive(false);
			itemParticle.gameObject.SetActive(false);
			vitalParticle.gameObject.SetActive(false);
			this._isInAnim[index] = false;
		}, true);
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x001CAFFC File Offset: 0x001C91FC
	private void RefreshCharProgress(Refers refers, SectStoryThreeVitalsCharacter vitalData)
	{
		bool isActive = !vitalData.IsInPrison && vitalData.HasPlayedComeAnim;
		refers.CGet<GameObject>("ProgressBack").SetActive(isActive);
		int maxInfection = GlobalConfig.Instance.ThreeVitalsMaxInfection;
		float infectionRate = (float)vitalData.Infection / (float)maxInfection;
		this.RefreshProgressText(refers, infectionRate);
		float fillAmount = (!isActive) ? 0f : Mathf.Lerp(this.progressMinFillAmountBottom, this.progressMaxFillAmountBottom, infectionRate);
		this.RefreshHalfFill(refers, fillAmount, 0f);
		this.RefreshHalfFade(refers, 0f, false);
		int helpPercent = this._isGoodEnd ? GlobalConfig.Instance.ThreeVitalsThresholdHigh : GlobalConfig.Instance.ThreeVitalsThresholdLow;
		float helpRate = (float)helpPercent / 100f;
		float markRotation = Mathf.Lerp(this.progressMarkMinRotation, this.progressMarkMaxRotation, helpRate);
		RectTransform markRootLeft = refers.CGet<RectTransform>("MarkRootLeft");
		markRootLeft.gameObject.SetActive(isActive);
		markRootLeft.localRotation = Quaternion.Euler(Vector3.zero.SetZ(-markRotation));
		RectTransform markRootRight = refers.CGet<RectTransform>("MarkRootRight");
		markRootRight.gameObject.SetActive(isActive);
		markRootRight.localRotation = Quaternion.Euler(Vector3.zero.SetZ(markRotation));
		refers.CGet<GameObject>("FrameNormal").SetActive(!vitalData.IsInPrison);
		CImage frameDangerLeft = refers.CGet<CImage>("FrameDangerLeft");
		frameDangerLeft.gameObject.SetActive(!vitalData.IsInPrison);
		CImage frameDangerRight = refers.CGet<CImage>("FrameDangerRight");
		frameDangerRight.gameObject.SetActive(!vitalData.IsInPrison);
		float dangerRate = (float)GlobalConfig.Instance.ThreeVitalsThresholdLow / (float)GlobalConfig.Instance.ThreeVitalsMaxInfection;
		float minFillAmount = this._isGoodEnd ? this.progressMinFillAmountTop : this.progressMinFillAmountBottom;
		float maxFillAmount = this._isGoodEnd ? this.progressMaxFillAmountTop : this.progressMaxFillAmountBottom;
		float frameDangerFillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, dangerRate);
		frameDangerRight.fillAmount = (frameDangerLeft.fillAmount = frameDangerFillAmount);
		Image.Origin360 fillOrigin = this._isGoodEnd ? Image.Origin360.Top : Image.Origin360.Bottom;
		frameDangerRight.fillOrigin = (frameDangerLeft.fillOrigin = fillOrigin.ToInt());
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x001CB22C File Offset: 0x001C942C
	private void RefreshProgressText(Refers refers, float rate)
	{
		int maxInfection = GlobalConfig.Instance.ThreeVitalsMaxInfection;
		int value = Convert.ToInt32(rate * (float)maxInfection);
		string textColor = UI_ThreeVitals.IsVitalHelping(this._isGoodEnd, value) ? "brightblue" : "brightred";
		refers.CGet<TextMeshProUGUI>("ProgressText").text = string.Format("{0}%", value).SetColor(textColor);
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x001CB293 File Offset: 0x001C9493
	private void RefreshHalfFill(Refers refers, float fillAmount, float duration)
	{
		this.RefreshHalfFill(refers.CGet<GameObject>("FillLeft"), fillAmount, duration);
		this.RefreshHalfFill(refers.CGet<GameObject>("FillRight"), fillAmount, duration);
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x001CB2C0 File Offset: 0x001C94C0
	private void RefreshHalfFill(GameObject fill, float fillAmount, float duration)
	{
		Transform goodFillEffect = fill.transform.GetChild(0);
		goodFillEffect.gameObject.SetActive(this._isGoodEnd);
		Image goodFill = goodFillEffect.GetComponentInChildren<Image>();
		bool flag = duration == 0f;
		if (flag)
		{
			goodFill.fillAmount = fillAmount;
		}
		else
		{
			goodFill.DOFillAmount(fillAmount, duration);
		}
		Transform badFillEffect = fill.transform.GetChild(1);
		badFillEffect.gameObject.SetActive(!this._isGoodEnd);
		Image badFill = badFillEffect.GetComponentInChildren<Image>();
		bool flag2 = duration == 0f;
		if (flag2)
		{
			badFill.fillAmount = fillAmount;
		}
		else
		{
			badFill.DOFillAmount(fillAmount, duration);
		}
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x001CB35F File Offset: 0x001C955F
	private void RefreshHalfFade(Refers refers, float duration, bool fadeOut)
	{
		this.RefreshHalfFade(refers.CGet<GameObject>("FillLeft"), duration, fadeOut);
		this.RefreshHalfFade(refers.CGet<GameObject>("FillRight"), duration, fadeOut);
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x001CB38C File Offset: 0x001C958C
	private void RefreshHalfFade(GameObject fill, float duration, bool fadeOut)
	{
		Transform goodFillEffect = fill.transform.GetChild(0);
		Image goodFill = goodFillEffect.GetComponentInChildren<Image>();
		goodFill.DOFade((float)(fadeOut ? 0 : 1), duration);
		Transform badFillEffect = fill.transform.GetChild(1);
		Image badFill = badFillEffect.GetComponentInChildren<Image>();
		badFill.DOFade((float)(fadeOut ? 0 : 1), duration);
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x001CB3E4 File Offset: 0x001C95E4
	private void RefreshCharButton(int index, Refers refers, SectStoryThreeVitalsCharacter vitalData, bool isGood)
	{
		bool hasTarget = this.HasTarget();
		bool hasVital = !vitalData.IsInPrison;
		int maxInfection = GlobalConfig.Instance.ThreeVitalsMaxInfection;
		bool needTransfer = isGood ? (vitalData.Infection > 0) : (vitalData.Infection < maxInfection);
		string buttonName = isGood ? "ButtonGood" : "ButtonBad";
		CButtonObsolete selectButton = refers.CGet<CButtonObsolete>(buttonName);
		selectButton.gameObject.SetActive(isGood == this._isGoodEnd);
		selectButton.interactable = (hasVital && hasTarget && needTransfer);
		selectButton.ClearAndAddListener(delegate
		{
			this.OnClickSelectButton(index);
		});
		TooltipInvoker selectButtonTip = selectButton.GetComponent<TooltipInvoker>();
		string[] presetParam = selectButtonTip.PresetParam;
		bool flag = presetParam == null || presetParam.Length != 2;
		if (flag)
		{
			selectButtonTip.PresetParam = new string[2];
		}
		bool flag2 = hasTarget && needTransfer;
		if (flag2)
		{
			selectButtonTip.Type = TipType.Simple;
			LanguageKey titleKey = isGood ? LanguageKey.LK_ThreeVitals_SelectTarget_Good : LanguageKey.LK_ThreeVitals_SelectTarget_Bad;
			LanguageKey contentKey = isGood ? LanguageKey.LK_ThreeVitals_SelectTarget_Good_Tip : LanguageKey.LK_ThreeVitals_SelectTarget_Bad_Tip;
			selectButtonTip.PresetParam[0] = LocalStringManager.Get(titleKey);
			selectButtonTip.PresetParam[1] = LocalStringManager.Get(contentKey);
		}
		else
		{
			selectButtonTip.Type = TipType.SingleDesc;
			LanguageKey buttonTipKey = (!hasTarget) ? (this._isGoodEnd ? LanguageKey.LK_ThreeVitals_NoTarget_Good : LanguageKey.LK_ThreeVitals_NoTarget_Bad) : (this._isGoodEnd ? LanguageKey.LK_ThreeVitals_NoNeed_Good : LanguageKey.LK_ThreeVitals_NoNeed_Bad);
			selectButtonTip.PresetParam[0] = LocalStringManager.Get(buttonTipKey);
		}
		CButtonObsolete avatarButton = refers.CGet<CButtonObsolete>("AvatarButton");
		avatarButton.interactable = hasVital;
		avatarButton.ClearAndAddListener(delegate
		{
			this.OnClickAvatarButton(index);
		});
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x001CB594 File Offset: 0x001C9794
	private void OnClickAvatarButton(int index)
	{
		CharacterDisplayData vitalsCharData = this._threeVitalsCharDataList[index];
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", vitalsCharData.CharacterId);
		argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
	}

	// Token: 0x060038D1 RID: 14545 RVA: 0x001CB5FC File Offset: 0x001C97FC
	public static bool IsVitalHelping(bool isGoodEnd, int infection)
	{
		int maxInfection = GlobalConfig.Instance.ThreeVitalsMaxInfection;
		return isGoodEnd ? (infection * 100 / maxInfection < GlobalConfig.Instance.ThreeVitalsThresholdHigh) : (infection * 100 / maxInfection > GlobalConfig.Instance.ThreeVitalsThresholdLow);
	}

	// Token: 0x060038D2 RID: 14546 RVA: 0x001CB644 File Offset: 0x001C9844
	private bool HasTarget()
	{
		foreach (CharacterDisplayDataForInfect data in this._targetCharDataList)
		{
			bool flag = UI_ThreeVitals.IsTargetInteractable(this._isGoodEnd, (int)data.Infection);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060038D3 RID: 14547 RVA: 0x001CB6B4 File Offset: 0x001C98B4
	public static bool IsTargetInteractable(bool isGoodEnd, int infection)
	{
		if (!isGoodEnd)
		{
			if (infection <= 0)
			{
				goto IL_1E;
			}
		}
		else if (infection >= 200)
		{
			goto IL_1E;
		}
		return true;
		IL_1E:
		return false;
	}

	// Token: 0x060038D4 RID: 14548 RVA: 0x001CB6E4 File Offset: 0x001C98E4
	private void OnClickSelectButton(int index)
	{
		this._selectVitalIndex = index;
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("isGoodEnd", this._isGoodEnd).SetObject("targetCharDataList", this._targetCharDataList).SetObject("vitalData", this._threeVitalsDataList[index]).SetObject("vitalCharData", this._threeVitalsCharDataList[index]).SetObject("onConfirm", new Action<Dictionary<int, int>>(this.OnConfirmTransfer));
		UIElement.SelectThreeVitalsTarget.SetOnInitArgs(args);
		UIManager.Instance.ShowUI(UIElement.SelectThreeVitalsTarget, true);
	}

	// Token: 0x060038D5 RID: 14549 RVA: 0x001CB780 File Offset: 0x001C9980
	private void OnConfirmTransfer(Dictionary<int, int> targetTransferInfectionDict)
	{
		SectStoryThreeVitalsCharacter vitalData = this._threeVitalsDataList[this._selectVitalIndex];
		AudioManager.Instance.PlaySound(this._isGoodEnd ? "sancai_absorb" : "sanmo_absorb", false, false);
		int lastInfection = vitalData.Infection;
		foreach (KeyValuePair<int, int> keyValuePair in targetTransferInfectionDict)
		{
			int num;
			int num2;
			keyValuePair.Deconstruct(out num, out num2);
			int value = num2;
			bool isGoodEnd = this._isGoodEnd;
			if (isGoodEnd)
			{
				vitalData.Infection -= value;
			}
			else
			{
				vitalData.Infection += value;
			}
		}
		this.PlayInfectAnim(this._selectVitalIndex, lastInfection, delegate
		{
			foreach (KeyValuePair<int, int> keyValuePair2 in targetTransferInfectionDict)
			{
				int num3;
				int num4;
				keyValuePair2.Deconstruct(out num3, out num4);
				int charId = num3;
				int value2 = num4;
				bool flag = value2 == 0;
				if (!flag)
				{
					ExtraDomainMethod.Call.TransferInfectionBetweenVitalAndCharacter(charId, vitalData.VitalType, value2);
				}
			}
			this.SendRefresh();
		});
		this.Speak(this._threeVitalsRefersList[this._selectVitalIndex], this.GetTransferInfectionSpeakContent(vitalData.VitalType, this._isGoodEnd));
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x001CB8C0 File Offset: 0x001C9AC0
	private void Speak(Refers refers, string content)
	{
		Bubble bubble = refers.CGet<Bubble>("Bubble");
		bubble.SetText(content, true);
		DOVirtual.DelayedCall(this.bubbleDuration, delegate
		{
			bubble.Hide();
		}, false).SetTarget(bubble);
	}

	// Token: 0x060038D7 RID: 14551 RVA: 0x001CB918 File Offset: 0x001C9B18
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (a == "Close")
		{
			this.QuickHide();
		}
	}

	// Token: 0x060038D8 RID: 14552 RVA: 0x001CB947 File Offset: 0x001C9B47
	protected IEnumerator SetTragetObjFade(GameObject targetObj, bool fadeIn, float delayTime, float time = 0.8f)
	{
		yield return new WaitForSeconds(delayTime);
		float curTime = 0f;
		List<Gradient> colors = new List<Gradient>();
		List<float> baseAlphas = new List<float>();
		int num;
		for (int i = 0; i < targetObj.transform.childCount; i = num + 1)
		{
			ParticleSystem particleSystem = targetObj.transform.GetChild(i).GetComponent<ParticleSystem>();
			ParticleSystem.ColorOverLifetimeModule col = particleSystem.colorOverLifetime;
			colors.Add(new Gradient());
			colors[i].SetKeys(col.color.gradient.colorKeys, col.color.gradient.alphaKeys);
			baseAlphas.Add(col.color.gradient.alphaKeys.FirstOrDefault<GradientAlphaKey>().alpha);
			particleSystem = null;
			col = default(ParticleSystem.ColorOverLifetimeModule);
			num = i;
		}
		if (fadeIn)
		{
			targetObj.transform.parent.gameObject.SetActive(true);
			targetObj.SetActive(true);
			for (int j = 0; j < targetObj.transform.childCount; j = num + 1)
			{
				targetObj.transform.GetChild(j).GetComponent<ParticleSystem>().Play();
				num = j;
			}
		}
		while (curTime < time)
		{
			float progress = curTime / time;
			float clampedProgress = Mathf.Clamp(progress, 0f, 1f);
			float alphaMultiplier = fadeIn ? clampedProgress : (1f - clampedProgress);
			for (int k = 0; k < targetObj.transform.childCount; k = num + 1)
			{
				ParticleSystem particleSystem2 = targetObj.transform.GetChild(k).GetComponent<ParticleSystem>();
				ParticleSystem.ColorOverLifetimeModule col2 = particleSystem2.colorOverLifetime;
				GradientColorKey[] originalColorKeys = colors[k].colorKeys;
				GradientAlphaKey[] originalAlphaKeys = colors[k].alphaKeys;
				GradientAlphaKey[] newAlphaKeys = new GradientAlphaKey[originalAlphaKeys.Length];
				for (int l = 0; l < originalAlphaKeys.Length; l = num + 1)
				{
					float newAlpha = Mathf.Min(originalAlphaKeys[l].alpha * alphaMultiplier, originalAlphaKeys[l].alpha);
					newAlphaKeys[l] = new GradientAlphaKey(newAlpha, originalAlphaKeys[l].time);
					num = l;
				}
				Gradient grad = new Gradient();
				grad.SetKeys(originalColorKeys, newAlphaKeys);
				col2.color = grad;
				particleSystem2 = null;
				col2 = default(ParticleSystem.ColorOverLifetimeModule);
				originalColorKeys = null;
				originalAlphaKeys = null;
				newAlphaKeys = null;
				grad = null;
				num = k;
			}
			curTime += Time.deltaTime;
			yield return null;
		}
		for (int m = 0; m < targetObj.transform.childCount; m = num + 1)
		{
			ParticleSystem particleSystem3 = targetObj.transform.GetChild(m).GetComponent<ParticleSystem>();
			ParticleSystem.ColorOverLifetimeModule col3 = particleSystem3.colorOverLifetime;
			col3.color = colors[m];
			particleSystem3 = null;
			col3 = default(ParticleSystem.ColorOverLifetimeModule);
			num = m;
		}
		bool flag = !fadeIn;
		if (flag)
		{
			targetObj.SetActive(false);
			targetObj.transform.parent.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x0400291D RID: 10525
	[SerializeField]
	private float progressMarkMinRotation;

	// Token: 0x0400291E RID: 10526
	[SerializeField]
	private float progressMarkMaxRotation;

	// Token: 0x0400291F RID: 10527
	[SerializeField]
	private float progressMinFillAmountBottom;

	// Token: 0x04002920 RID: 10528
	[SerializeField]
	private float progressMaxFillAmountBottom;

	// Token: 0x04002921 RID: 10529
	[SerializeField]
	private float progressMinFillAmountTop;

	// Token: 0x04002922 RID: 10530
	[SerializeField]
	private float progressMaxFillAmountTop;

	// Token: 0x04002923 RID: 10531
	[SerializeField]
	private float bubbleDuration = 2f;

	// Token: 0x04002924 RID: 10532
	private List<Refers> _threeVitalsRefersList;

	// Token: 0x04002925 RID: 10533
	private List<CharacterDisplayData> _threeVitalsCharDataList = new List<CharacterDisplayData>();

	// Token: 0x04002926 RID: 10534
	private List<SectStoryThreeVitalsCharacter> _threeVitalsDataList = new List<SectStoryThreeVitalsCharacter>();

	// Token: 0x04002927 RID: 10535
	private List<CharacterDisplayDataForInfect> _targetCharDataList = new List<CharacterDisplayDataForInfect>();

	// Token: 0x04002928 RID: 10536
	private bool _isGoodEnd;

	// Token: 0x04002929 RID: 10537
	private int _dangerThreshold;

	// Token: 0x0400292A RID: 10538
	private int _selectVitalIndex;

	// Token: 0x0400292B RID: 10539
	private readonly bool[] _isInAnim = new bool[3];

	// Token: 0x0400292C RID: 10540
	private const float _progressBarAudioLength_Mo = 1.17f;

	// Token: 0x0400292D RID: 10541
	private const float _progressBarAudioLength_Cai = 0.98f;

	// Token: 0x0400292E RID: 10542
	private const float _refreshHalfFillDuration = 1f;

	// Token: 0x0400292F RID: 10543
	private Dictionary<Bubble, Coroutine> _closeBubbleCoroutineDic = new Dictionary<Bubble, Coroutine>();
}
