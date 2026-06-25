using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200023C RID: 572
public class LifeSkillCombatCardView : Refers, ILifeSkillCombatSelectable
{
	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x0600251F RID: 9503 RVA: 0x00111120 File Offset: 0x0010F320
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002520 RID: 9504 RVA: 0x00111127 File Offset: 0x0010F327
	public DebateStrategyItem CardConfig
	{
		get
		{
			return this._cardConfig;
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x06002521 RID: 9505 RVA: 0x0011112F File Offset: 0x0010F32F
	public new RectTransform RectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x06002522 RID: 9506 RVA: 0x0011113C File Offset: 0x0010F33C
	public int EffectCardId
	{
		get
		{
			LifeSkillCombatCard effectCard = this._effectCard;
			return (int)((effectCard != null) ? effectCard.EffectCardId : -1);
		}
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06002523 RID: 9507 RVA: 0x00111150 File Offset: 0x0010F350
	public CButtonObsolete Button
	{
		get
		{
			return base.CGet<CButtonObsolete>("Button");
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x06002524 RID: 9508 RVA: 0x0011115D File Offset: 0x0010F35D
	private CImage Frame
	{
		get
		{
			return base.CGet<CImage>("Frame");
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x06002525 RID: 9509 RVA: 0x0011116A File Offset: 0x0010F36A
	private CImage Cover
	{
		get
		{
			return base.CGet<CImage>("Cover");
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x06002526 RID: 9510 RVA: 0x00111177 File Offset: 0x0010F377
	private CImage Content
	{
		get
		{
			return base.CGet<CImage>("Content");
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x06002527 RID: 9511 RVA: 0x00111184 File Offset: 0x0010F384
	private CImage Mark
	{
		get
		{
			return base.CGet<CImage>("Mark");
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06002528 RID: 9512 RVA: 0x00111191 File Offset: 0x0010F391
	private CImage TypeIcon
	{
		get
		{
			return base.CGet<CImage>("TypeIcon");
		}
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x06002529 RID: 9513 RVA: 0x0011119E File Offset: 0x0010F39E
	private TextMeshProUGUI Title
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Title");
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x0600252A RID: 9514 RVA: 0x001111AB File Offset: 0x0010F3AB
	private GameObject StrategyTargetMark
	{
		get
		{
			return base.CGet<GameObject>("StrategyTargetMark");
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x0600252B RID: 9515 RVA: 0x001111B8 File Offset: 0x0010F3B8
	private RectTransform CostLayout
	{
		get
		{
			return base.CGet<RectTransform>("CostLayout");
		}
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x0600252C RID: 9516 RVA: 0x001111C5 File Offset: 0x0010F3C5
	private GameObject Selected
	{
		get
		{
			return base.CGet<GameObject>("Selected");
		}
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x0600252D RID: 9517 RVA: 0x001111D2 File Offset: 0x0010F3D2
	private GameObject Hover
	{
		get
		{
			return base.CGet<GameObject>("Hover");
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x0600252E RID: 9518 RVA: 0x001111DF File Offset: 0x0010F3DF
	private DisableStyleRoot DisableRoot
	{
		get
		{
			return base.CGet<DisableStyleRoot>("DisableRoot");
		}
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x0600252F RID: 9519 RVA: 0x001111EC File Offset: 0x0010F3EC
	private PointerTrigger PointerTrigger
	{
		get
		{
			return base.CGet<PointerTrigger>("PointerTrigger");
		}
	}

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06002530 RID: 9520 RVA: 0x001111F9 File Offset: 0x0010F3F9
	private Canvas Canvas
	{
		get
		{
			return base.CGet<Canvas>("Canvas");
		}
	}

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06002531 RID: 9521 RVA: 0x00111206 File Offset: 0x0010F406
	private RectTransform AnimRoot
	{
		get
		{
			return base.CGet<RectTransform>("AnimRoot");
		}
	}

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06002532 RID: 9522 RVA: 0x00111213 File Offset: 0x0010F413
	private EffectPlayer EffectPlayer
	{
		get
		{
			return base.CGet<EffectPlayer>("EffectPlayer");
		}
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06002533 RID: 9523 RVA: 0x00111220 File Offset: 0x0010F420
	private GameObject CostNotMeetBg
	{
		get
		{
			return base.CGet<GameObject>("CostNotMeetBg");
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x06002534 RID: 9524 RVA: 0x0011122D File Offset: 0x0010F42D
	private TooltipInvoker Tip
	{
		get
		{
			return base.CGet<TooltipInvoker>("Tip");
		}
	}

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x06002535 RID: 9525 RVA: 0x0011123A File Offset: 0x0010F43A
	private GameObject SelectedMark
	{
		get
		{
			return this.Selected;
		}
	}

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06002536 RID: 9526 RVA: 0x00111242 File Offset: 0x0010F442
	// (set) Token: 0x06002537 RID: 9527 RVA: 0x0011124A File Offset: 0x0010F44A
	public int Index { get; private set; }

	// Token: 0x06002538 RID: 9528 RVA: 0x00111254 File Offset: 0x0010F454
	public void SetData(short strategyTemplateId, int index)
	{
		this.Index = index;
		this._cardConfig = DebateStrategy.Instance.GetItem(strategyTemplateId);
		bool isInLifeSkillCombat = UIManager.Instance.IsElementActive(UIElement.LifeSkillCombatOld);
		this._isCostMeet = (!isInLifeSkillCombat || this.Model.CheckCost(this._cardConfig));
		List<StrategyTarget> list;
		this._isTargetMeet = (!isInLifeSkillCombat || this.Model.DebateGame.TryGetStrategyTarget(this._cardConfig.TemplateId, true, out list));
		this.Title.text = this._cardConfig.Name;
		this.RefreshTip();
		this.Frame.SetSprite(this.GetBackImage(), false, null);
		this.Content.SetSprite(this._cardConfig.Image, false, null);
		EDebateStrategyMarkType markType = this._cardConfig.MarkType;
		if (!true)
		{
		}
		string text;
		switch (markType)
		{
		case EDebateStrategyMarkType.Attach:
			text = "LifeSkillCombatCard_Effect_0";
			break;
		case EDebateStrategyMarkType.Affect:
			text = "LifeSkillCombatCard_Effect_1";
			break;
		case EDebateStrategyMarkType.Other:
			text = "LifeSkillCombatCard_Effect_2";
			break;
		default:
			text = string.Empty;
			break;
		}
		if (!true)
		{
		}
		string markImageName = text;
		this.Mark.SetSprite(markImageName, false, null);
		this.TypeIcon.SetSprite(this.GetTypeIcon(), false, null);
		this.SetEnabled(false, false);
		this.RefreshCostNumber();
		this.SetSelected(false);
		this.Hover.SetActive(false);
		this.StrategyTargetMark.SetActive(false);
		bool flag = this.Canvas;
		if (flag)
		{
			this.Canvas.overrideSorting = false;
		}
		UnityEvent enterEvent = this.PointerTrigger.EnterEvent;
		bool flag2 = enterEvent == null;
		if (flag2)
		{
			enterEvent = new UnityEvent();
		}
		enterEvent.RemoveAllListeners();
		enterEvent.AddListener(new UnityAction(this.OnPointerTriggerEnter));
		UnityEvent exitEvent = this.PointerTrigger.ExitEvent;
		bool flag3 = exitEvent == null;
		if (flag3)
		{
			exitEvent = new UnityEvent();
		}
		exitEvent.RemoveAllListeners();
		exitEvent.AddListener(new UnityAction(this.OnPointerTriggerExit));
		this.ShowCover(false);
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x00111458 File Offset: 0x0010F658
	public void SetOnClick(Action action)
	{
		this.Button.ClearAndAddListener(delegate
		{
			bool flag = this._onStrategySelectTarget != null;
			if (flag)
			{
				bool isSelected = !this.SelectedMark.activeSelf;
				this.StrategyTargetMark.SetActive(!isSelected);
				this.SelectedMark.SetActive(isSelected);
				this._onStrategySelectTarget(isSelected);
			}
			else
			{
				Action action2 = action;
				if (action2 != null)
				{
					action2();
				}
				Action onClickShowCountMaxTip = this._onClickShowCountMaxTip;
				if (onClickShowCountMaxTip != null)
				{
					onClickShowCountMaxTip();
				}
			}
		});
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x00111494 File Offset: 0x0010F694
	public void SetEnabled(bool canUseCard, bool selfIsTarget = false)
	{
		bool realCanUseCard = selfIsTarget ? canUseCard : (canUseCard && this._isCostMeet && this._isTargetMeet);
		this.DisableRoot.SetStyleEffect(!realCanUseCard, false);
		this.SetPointerTrigger(realCanUseCard);
		this.SetInteractable(realCanUseCard);
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x001114DE File Offset: 0x0010F6DE
	public void SetInteractable(bool interactable)
	{
		this.Button.interactable = interactable;
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x001114ED File Offset: 0x0010F6ED
	public void SetPointerTrigger(bool enable)
	{
		this.PointerTrigger.enabled = enable;
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x001114FC File Offset: 0x0010F6FC
	public void SetSelected(bool isSelected, bool hasScale)
	{
		this.Selected.SetActive(isSelected);
		if (isSelected)
		{
			this.SetPointerTrigger(false);
			this.OnPointerTriggerEnter();
		}
		else
		{
			this.SetPointerTrigger(true);
			this.OnPointerTriggerExit();
		}
		LifeSkillCombatCardView tipCardView = this._tipCardView;
		if (tipCardView != null)
		{
			tipCardView.SetSelected(isSelected);
		}
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x00111554 File Offset: 0x0010F754
	public void ShowStrategyTargetMark(bool show, Action<bool> onClick)
	{
		this.Button.interactable = show;
		this.StrategyTargetMark.SetActive(show);
		this._onStrategySelectTarget = onClick;
		if (show)
		{
			this.SetEnabled(true, true);
		}
		else
		{
			this.SetData(this.CardConfig.TemplateId, this.Index);
		}
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x001115AB File Offset: 0x0010F7AB
	public void SetSelected(bool isSelected)
	{
		this.SetSelected(isSelected, false);
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x001115B8 File Offset: 0x0010F7B8
	public void OnPointerTriggerEnter()
	{
		this.Hover.SetActive(true);
		bool flag = this.Canvas;
		if (flag)
		{
			this.Canvas.overrideSorting = true;
		}
		this.AnimRoot.DOKill(false);
		this.AnimRoot.DOAnchorPosY(this.HoverLocalMoveY, this.HoverDuration, false);
		this.AnimRoot.DOScale(this.HoverScaleY, this.HoverDuration);
		ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Card", this).Set("IsEnter", true);
		bool flag2 = this.Model.FocusingCardItem == null;
		if (flag2)
		{
			GEvent.OnEvent(UiEvents.CombatLifeSkillHoverStrategy, args);
		}
		Action onShowTip = this._onShowTip;
		if (onShowTip != null)
		{
			onShowTip();
		}
		Action onShowNew = this._onShowNew;
		if (onShowNew != null)
		{
			onShowNew();
		}
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x00111690 File Offset: 0x0010F890
	public void OnPointerTriggerExit()
	{
		this.Hover.SetActive(false);
		bool flag = this.Canvas;
		if (flag)
		{
			this.Canvas.overrideSorting = false;
		}
		this.AnimRoot.DOKill(false);
		this.AnimRoot.DOAnchorPosY(0f, this.HoverDuration, false);
		this.AnimRoot.DOScale(1f, this.HoverDuration);
		ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Card", this).Set("IsEnter", false);
		bool flag2 = this.Model.FocusingCardItem == null;
		if (flag2)
		{
			GEvent.OnEvent(UiEvents.CombatLifeSkillHoverStrategy, args);
		}
		Action onHideTip = this._onHideTip;
		if (onHideTip != null)
		{
			onHideTip();
		}
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x00111754 File Offset: 0x0010F954
	private void RefreshTip()
	{
		this.Tip.Type = TipType.LifeSkillCombatStrategy;
		TooltipInvoker tip = this.Tip;
		if (tip.RuntimeParam == null)
		{
			tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		this.Tip.RuntimeParam.Set("TemplateId", this._cardConfig.TemplateId);
		this.Tip.RuntimeParam.Set("IsPointMeet", this._isCostMeet);
		this.Tip.RuntimeParam.Set("IsTargetMeet", this._isTargetMeet);
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x001117E8 File Offset: 0x0010F9E8
	public void SetNewEvent(Action action)
	{
		this._onShowNew = action;
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x001117F4 File Offset: 0x0010F9F4
	public void InitTipEvent(LifeSkillCombatCardView tipCardView, Transform followTrans = null, LifeSkillCombatCardView.GetAnchoredPosOffset getAnchoredPosOffset = null, LifeSkillCombatCardView.GetShowCountMaxTip getShowCountMaxTip = null)
	{
		this._tipCardView = tipCardView;
		this._getAnchoredPosOffset = getAnchoredPosOffset;
		this._onShowTip = delegate()
		{
		};
		this._onHideTip = delegate()
		{
			this._tipCardView.gameObject.SetActive(false);
		};
	}

	// Token: 0x06002545 RID: 9541 RVA: 0x00111848 File Offset: 0x0010FA48
	public void RefreshTipCardPositionFollowerOffset(bool forceRefresh = false)
	{
		PositionFollower follower = this._tipCardView.gameObject.GetOrAddComponent<PositionFollower>();
		bool flag = follower.Target != base.transform && !forceRefresh;
		if (!flag)
		{
			LifeSkillCombatCardView.GetAnchoredPosOffset getAnchoredPosOffset = this._getAnchoredPosOffset;
			Vector2 offset = (getAnchoredPosOffset != null) ? getAnchoredPosOffset() : Vector3.zero;
			follower.Offset = offset;
		}
	}

	// Token: 0x06002546 RID: 9542 RVA: 0x001118B0 File Offset: 0x0010FAB0
	public void SetTipCardPositionFollowerEnabled(bool isEnabled)
	{
		bool flag = !this._tipCardView;
		if (!flag)
		{
			PositionFollower follower = this._tipCardView.gameObject.GetComponent<PositionFollower>();
			bool flag2 = !follower;
			if (!flag2)
			{
				follower.enabled = isEnabled;
			}
		}
	}

	// Token: 0x06002547 RID: 9543 RVA: 0x001118FC File Offset: 0x0010FAFC
	private string GetBackImage()
	{
		int index = 1;
		return string.Format("lifeskillcombat_card_{0}_{1}", index, (int)(this._cardConfig.Level - 1));
	}

	// Token: 0x06002548 RID: 9544 RVA: 0x00111934 File Offset: 0x0010FB34
	private string GetTypeIcon()
	{
		int index = 0;
		sbyte type = this._cardConfig.LifeSkillType;
		return string.Format("lifeskillcombat_cardtype_{0}_{1}", index, type);
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x0011196C File Offset: 0x0010FB6C
	private void RefreshCostNumber()
	{
		this.CostNotMeetBg.SetActive(!this._isCostMeet);
		int firstNumber = (int)(this._cardConfig.UsedCost / 10);
		int secondNumber = (int)(this._cardConfig.UsedCost % 10);
		CImage firstImage = this.CostLayout.GetChild(0).GetComponent<CImage>();
		firstImage.gameObject.SetActive(firstNumber > 0);
		CImage secondImage = this.CostLayout.GetChild(1).GetComponent<CImage>();
		int index = this._isCostMeet ? 0 : 6;
		firstImage.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", index, firstNumber), false, null);
		secondImage.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", index, secondNumber), false, null);
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x00111A30 File Offset: 0x0010FC30
	private string GetUseEffectName()
	{
		string result;
		switch (this._cardConfig.Level)
		{
		case 1:
			result = "eff_lifeskillcombat_ui_kpzddijika";
			break;
		case 2:
			result = "eff_lifeskillcombat_ui_kpzdzhongjika";
			break;
		case 3:
			result = "eff_lifeskillcombat_ui_kpzdgaojika";
			break;
		default:
			result = string.Empty;
			break;
		}
		return result;
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x00111A84 File Offset: 0x0010FC84
	public void PlayUseEffect()
	{
		string effectName = this.GetUseEffectName();
		float duration = 0.7f;
		this.EffectPlayer.PlayEffectAt(this.AnimRoot, effectName, duration, false);
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x00111AB4 File Offset: 0x0010FCB4
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x00111ABC File Offset: 0x0010FCBC
	public void ShowCover(bool show)
	{
		if (show)
		{
			sbyte level = this._cardConfig.Level;
			if (!true)
			{
			}
			string text;
			switch (level)
			{
			case 1:
				text = "lifeskillcombat_card_0_0";
				break;
			case 2:
				text = "lifeskillcombat_card_0_1";
				break;
			case 3:
				text = "lifeskillcombat_card_0_2";
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string spName = text;
			this.Cover.SetSprite(spName, false, null);
		}
		this.Cover.gameObject.SetActive(show);
	}

	// Token: 0x04001BAF RID: 7087
	[SerializeField]
	private float HoverLocalMoveY = 100f;

	// Token: 0x04001BB0 RID: 7088
	[SerializeField]
	private float SelectedLocalMoveY = 20f;

	// Token: 0x04001BB1 RID: 7089
	[SerializeField]
	private float HoverScaleY = 1.1f;

	// Token: 0x04001BB2 RID: 7090
	[SerializeField]
	private float HoverDuration = 0.2f;

	// Token: 0x04001BB3 RID: 7091
	private LifeSkillCombatCard _effectCard;

	// Token: 0x04001BB4 RID: 7092
	private DebateStrategyItem _cardConfig;

	// Token: 0x04001BB5 RID: 7093
	private Action _onShowTip;

	// Token: 0x04001BB6 RID: 7094
	private Action _onHideTip;

	// Token: 0x04001BB7 RID: 7095
	private Action _onShowNew;

	// Token: 0x04001BB8 RID: 7096
	private Action _onClickShowCountMaxTip;

	// Token: 0x04001BB9 RID: 7097
	private LifeSkillCombatCardView _tipCardView;

	// Token: 0x04001BBA RID: 7098
	private LifeSkillCombatCardView.GetAnchoredPosOffset _getAnchoredPosOffset;

	// Token: 0x04001BBB RID: 7099
	private Action<bool> _onStrategySelectTarget;

	// Token: 0x04001BBD RID: 7101
	private bool _isCostMeet;

	// Token: 0x04001BBE RID: 7102
	private bool _isTargetMeet;

	// Token: 0x04001BBF RID: 7103
	public const float EnemyCardAnimDuration = 0.4f;

	// Token: 0x04001BC0 RID: 7104
	public const float UseCardAnimDuration = 0.7f;

	// Token: 0x04001BC1 RID: 7105
	public const float UseCardFailedAnimDuration = 1f;

	// Token: 0x04001BC2 RID: 7106
	public const float UseCardTargetPawnAnimDuration = 0.5f;

	// Token: 0x02001542 RID: 5442
	// (Invoke) Token: 0x0600CE38 RID: 52792
	public delegate bool GetShowCountMaxTip();

	// Token: 0x02001543 RID: 5443
	// (Invoke) Token: 0x0600CE3C RID: 52796
	public delegate Vector2 GetAnchoredPosOffset();
}
