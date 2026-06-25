using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.Main.Reading;
using GameData.Common;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class UI_LifeSkillCombatResult : UIBase
{
	// Token: 0x170003DF RID: 991
	// (get) Token: 0x0600258A RID: 9610 RVA: 0x00113CFD File Offset: 0x00111EFD
	private bool IsWin
	{
		get
		{
			return this._combatResult == 1;
		}
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x00113D08 File Offset: 0x00111F08
	public override void OnInit(ArgumentBox argsBox)
	{
		bool exist = UIElement.SystemOption.Exist;
		if (exist)
		{
			UIManager.Instance.HideUI(UIElement.SystemOption);
		}
		argsBox.Get("CombatResult", out this._combatResult);
		argsBox.Get<DebateResult>("DisplayData", out this._displayData);
		argsBox.Get("LifeSkillType", out this._type);
		string imagePath = string.Format("RemakeResources/Textures/LifeSkillCombat/lifeskillcombat_settlement_chahua_{0}", this._type);
		ResLoader.Load<Texture2D>(imagePath, delegate(Texture2D texture)
		{
			this.CGet<CRawImage>("MainImage").texture = texture;
		}, null, false);
		bool win = this.IsWin;
		float aniTime = win ? 0.5f : 0.5f;
		SkeletonGraphic resultAni = base.CGet<SkeletonGraphic>("ResultAni");
		CanvasGroup mainWindow = base.CGet<CanvasGroup>("MainWindow");
		GameObject pointerMask = base.CGet<GameObject>("PointerMask");
		CButtonObsolete closeBtn = base.CGet<CButtonObsolete>("Close");
		resultAni.AnimationState.SetAnimation(0, win ? "combat_win" : "combat_lose", false);
		pointerMask.SetActive(true);
		closeBtn.interactable = false;
		mainWindow.alpha = 0f;
		mainWindow.DOFade(1f, 0.5f).SetDelay(aniTime / 2f).OnComplete(delegate
		{
			pointerMask.SetActive(false);
			closeBtn.interactable = true;
			this.CGet<GameObject>("ClickToStartTips").SetActive(true);
			this.CGet<ParticleSystem>(win ? "Win" : "Lose").gameObject.SetActive(true);
			AudioManager.Instance.PlaySound(win ? "ui_art_vic" : "ui_art_fail", false, false);
		});
		Refers readInCombatTipsRefers = base.CGet<Refers>("ReadInCombatTips");
		Refers loopInCombatTipsRefers = base.CGet<Refers>("LoopInCombatTips");
		Refers readingEventTipsRefers = base.CGet<Refers>("ReadingEventTips");
		Refers loopingEventTipsRefers = base.CGet<Refers>("LoopingEventTips");
		bool showReadingEvent = this._displayData.ShowReadingEvent;
		readingEventTipsRefers.gameObject.SetActive(showReadingEvent);
		bool showLoopInCombat = this._displayData.Evaluations != null && this._displayData.Evaluations.Contains(43);
		bool flag = showLoopInCombat;
		if (flag)
		{
			CombatEvaluationItem loopInCombatEvaluation = CombatEvaluation.Instance[43];
			loopInCombatTipsRefers.CGet<TextMeshProUGUI>("Label").text = loopInCombatEvaluation.Name;
			TooltipInvoker tips = loopInCombatTipsRefers.CGet<TooltipInvoker>("Image");
			tips.PresetParam[0] = loopInCombatEvaluation.Name;
			tips.PresetParam[1] = loopInCombatEvaluation.Desc;
		}
		bool showLoopingEvent = this._displayData.ShowLoopingEvent;
		loopingEventTipsRefers.gameObject.SetActive(showLoopingEvent);
		base.CGet<GameObject>("EvaluationHolder").gameObject.SetActive(showReadingEvent || showLoopingEvent || showLoopInCombat);
		GameObject effectMask = base.CGet<GameObject>("EffectMask");
		bool flag2 = showReadingEvent;
		if (flag2)
		{
			this.PlayAnimation(effectMask, readingEventTipsRefers.CGet<CanvasGroup>("canvasGroup"), readingEventTipsRefers.CGet<UIParticle>("effect"));
		}
		bool flag3 = showLoopingEvent;
		if (flag3)
		{
			this.PlayAnimation(effectMask, loopingEventTipsRefers.CGet<CanvasGroup>("canvasGroup"), loopingEventTipsRefers.CGet<UIParticle>("effect"));
		}
		bool flag4 = showLoopInCombat;
		if (flag4)
		{
			this.PlayAnimation(effectMask, loopInCombatTipsRefers.CGet<CanvasGroup>("canvasGroup"), loopInCombatTipsRefers.CGet<UIParticle>("effect"));
		}
		Refers expLayoutRefers = base.CGet<Refers>("ExpLayout");
		expLayoutRefers.CGet<TextMeshProUGUI>("AddValue").text = (CommonUtils.GetDisplayStringForNum(this._displayData.Exp.Second, 100000) ?? "");
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x00114058 File Offset: 0x00112258
	private void PlayAnimation(GameObject mask, CanvasGroup canvasGroup, UIParticle effect)
	{
		Sequence readingEventAniSeq = DOTween.Sequence();
		canvasGroup.alpha = 0f;
		readingEventAniSeq.AppendInterval(0.8f);
		readingEventAniSeq.AppendCallback(delegate
		{
			mask.SetActive(true);
			effect.Play();
		});
		readingEventAniSeq.AppendInterval(0.8f);
		readingEventAniSeq.AppendCallback(delegate
		{
			mask.SetActive(false);
			canvasGroup.DOFade(1f, 0.2f);
			effect.gameObject.SetActive(true);
			effect.Play();
		});
		readingEventAniSeq.AppendInterval(0.5f);
		readingEventAniSeq.AppendCallback(delegate
		{
			effect.gameObject.SetActive(false);
		});
		readingEventAniSeq.Play<Sequence>();
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x001140FC File Offset: 0x001122FC
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x0600258E RID: 9614 RVA: 0x0011412C File Offset: 0x0011232C
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x0600258F RID: 9615 RVA: 0x0011415C File Offset: 0x0011235C
	public override void QuickHide()
	{
		base.CGet<ParticleSystem>(this.IsWin ? "Win" : "Lose").gameObject.SetActive(false);
		bool flag = !base.CGet<CButtonObsolete>("Close").interactable || (UIElement.ReadingEvent.UiBaseAs<ViewReadingEvent>() != null && UIElement.ReadingEvent.UiBaseAs<ViewReadingEvent>().gameObject.activeSelf);
		if (!flag)
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("LifeSkillBattleComplete", "WinState", this.IsWin);
			TaiwuEventDomainMethod.Call.TriggerListener("LifeSkillBattleComplete", true);
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
			UIManager.Instance.HideUI(UIElement.LifeSkillCombatOld);
		}
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x00114240 File Offset: 0x00112440
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)SingletonObject.getInstance<BasicGameData>().TaiwuCharId, new uint[]
		{
			34U,
			66U
		}));
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x00114274 File Offset: 0x00112474
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId1 == 66U;
				if (flag)
				{
					int exp = 0;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref exp);
					Refers expLayoutRefers = base.CGet<Refers>("ExpLayout");
					expLayoutRefers.CGet<TextMeshProUGUI>("PrevValue").text = (CommonUtils.GetDisplayStringForNum(exp - this._displayData.Exp.Second, 100000) ?? "");
					this.Element.ShowAfterRefresh();
				}
			}
		}
	}

	// Token: 0x04001BE5 RID: 7141
	private const float WinAniTime = 0.5f;

	// Token: 0x04001BE6 RID: 7142
	private const float LoseAniTime = 0.5f;

	// Token: 0x04001BE7 RID: 7143
	private const float FadeTime = 0.5f;

	// Token: 0x04001BE8 RID: 7144
	private sbyte _combatResult;

	// Token: 0x04001BE9 RID: 7145
	private sbyte _type;

	// Token: 0x04001BEA RID: 7146
	private DebateResult _displayData;
}
