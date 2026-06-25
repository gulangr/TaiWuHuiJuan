using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Taiwu.Debate;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class LifeSkillCombatBlock : ILifeSkillCombatSelectable
{
	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x060025AE RID: 9646 RVA: 0x001152B1 File Offset: 0x001134B1
	public Vector2Int Position { get; }

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x060025AF RID: 9647 RVA: 0x001152B9 File Offset: 0x001134B9
	public bool IsSelf { get; }

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x060025B0 RID: 9648 RVA: 0x001152C1 File Offset: 0x001134C1
	public RectTransform RectTrans
	{
		get
		{
			return this._refers.GetComponent<RectTransform>();
		}
	}

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x060025B1 RID: 9649 RVA: 0x001152CE File Offset: 0x001134CE
	private CButtonObsolete Button
	{
		get
		{
			return this._refers.CGet<CButtonObsolete>("Button");
		}
	}

	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x060025B2 RID: 9650 RVA: 0x001152E0 File Offset: 0x001134E0
	private GameObject StrategyTargetMark
	{
		get
		{
			return this._refers.CGet<GameObject>("StrategyTargetMark");
		}
	}

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x060025B3 RID: 9651 RVA: 0x001152F2 File Offset: 0x001134F2
	private GameObject SelectedMark
	{
		get
		{
			return this._refers.CGet<GameObject>("SelectedMark");
		}
	}

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x060025B4 RID: 9652 RVA: 0x00115304 File Offset: 0x00113504
	private GameObject Hover
	{
		get
		{
			return this._refers.CGet<GameObject>("Hover");
		}
	}

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x060025B5 RID: 9653 RVA: 0x00115316 File Offset: 0x00113516
	private CImage InteractableMark
	{
		get
		{
			return this._refers.CGet<CImage>("InteractableMark");
		}
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x060025B6 RID: 9654 RVA: 0x00115328 File Offset: 0x00113528
	private EffectPlayer EffectPlayer
	{
		get
		{
			return this._refers.CGet<EffectPlayer>("EffectPlayer");
		}
	}

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x060025B7 RID: 9655 RVA: 0x0011533A File Offset: 0x0011353A
	// (set) Token: 0x060025B8 RID: 9656 RVA: 0x00115342 File Offset: 0x00113542
	public DebateNode DebateNode { get; private set; }

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x060025B9 RID: 9657 RVA: 0x0011534B File Offset: 0x0011354B
	// (set) Token: 0x060025BA RID: 9658 RVA: 0x00115353 File Offset: 0x00113553
	public DebateNodeEffectState DebateNodeEffectState { get; private set; } = DebateNodeEffectState.Invalid;

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x060025BB RID: 9659 RVA: 0x0011535C File Offset: 0x0011355C
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x00115364 File Offset: 0x00113564
	public LifeSkillCombatBlock(Refers refers, Vector2Int position, bool isSelf, RectTransform operationLayout)
	{
		this._refers = refers;
		this.Position = position;
		this.IsSelf = isSelf;
		this.Button.interactable = false;
		this.Button.ClearAndAddListener(new Action(this.OnClick));
		this.StrategyTargetMark.SetActive(false);
		this.SetSelected(false);
		this.RefreshInteractableMark();
		this.Hover.transform.SetParent(operationLayout);
		this.SelectedMark.transform.SetParent(operationLayout);
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x00115400 File Offset: 0x00113600
	public void Refresh(DebateNode debateNode)
	{
		this.DebateNode = debateNode;
		this.Button.interactable = (this.IsSelf && this.DebateNode.TaiwuCanMakeMove && (this.Model.DebateGame.GetPlayerCanMakeMove(true) || this.Model.DebateGame.GetNodeIsContainingEffect(this.DebateNode.Coordinate, 41)) && this.Model.IsTaiwuRound);
		this.RefreshInteractableMark();
		this.RefreshTip();
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x00115488 File Offset: 0x00113688
	private void RefreshInteractableMark()
	{
		bool flag = this.DebateNode == null;
		if (!flag)
		{
			string sp = this.IsSelf ? (this.Button.interactable ? "lifeskillcombat_sign_0_0" : "lifeskillcombat_sign_1_0") : "lifeskillcombat_sign_0_2";
			this.InteractableMark.SetSprite(sp, false, null);
		}
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x001154E0 File Offset: 0x001136E0
	public void OnClick()
	{
		bool flag = this._onStrategySelectTarget != null;
		if (flag)
		{
			bool isSelected = !this.SelectedMark.activeSelf;
			this.StrategyTargetMark.SetActive(!isSelected);
			this.SetSelected(isSelected);
			this._onStrategySelectTarget(isSelected);
		}
		else
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Block", this);
			GEvent.OnEvent(UiEvents.CombatLifeSkillClickBlock, args);
		}
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x00115558 File Offset: 0x00113758
	public void ShowStrategyTargetMark(bool show, Action<bool> onClick)
	{
		this.Button.interactable = show;
		this.StrategyTargetMark.SetActive(show);
		this._onStrategySelectTarget = onClick;
		bool flag = !show;
		if (flag)
		{
			this.SetSelected(false);
		}
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x00115597 File Offset: 0x00113797
	public void SetSelected(bool isSelected)
	{
		this.SelectedMark.SetActive(isSelected);
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x001155A7 File Offset: 0x001137A7
	public Transform GetTransform()
	{
		return this.RectTrans;
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x001155B0 File Offset: 0x001137B0
	public void PlayEffectAddedAnim(DebateNodeEffectState nodeEffectState)
	{
		this.DebateNodeEffectState = new DebateNodeEffectState(nodeEffectState);
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundAddNodeEffect, false, true);
		int index = nodeEffectState.TemplateId + 1;
		string createAnimName = string.Format("eff_lifeskillcombat_5dalei_tongyong{0}", index);
		this.EffectPlayer.PlayEffectAt(this.RectTrans, createAnimName, 0.2f, false);
		DOVirtual.DelayedCall(0.2f, delegate
		{
			bool flag = !this._refers || !this._refers.gameObject.activeSelf;
			if (!flag)
			{
				string loopAnimName = string.Format("eff_lifeskillcombat_5dalei_{0}xh", index);
				this._effectObj = this.EffectPlayer.PlayEffectAt(this.RectTrans, loopAnimName, -1f, false);
				DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[nodeEffectState.TemplateId];
				AudioManager.Instance.PlaySoundNoRepeat(nodeEffectConfig.LoopSound, 100, true, true);
			}
		}, true).SetTarget(this._refers);
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x0011565C File Offset: 0x0011385C
	public static bool HasExtraTriggeredAnim(short nodeEffectTemplateId)
	{
		int index = (int)(nodeEffectTemplateId + 1);
		return index == 2 || index == 5;
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x00115680 File Offset: 0x00113880
	public void PlayEffectRemovedAnim(bool isLast)
	{
		bool flag = this._effectObj == null;
		if (!flag)
		{
			if (isLast)
			{
				DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[this.DebateNodeEffectState.TemplateId];
				bool flag2 = nodeEffectConfig == null;
				if (flag2)
				{
					return;
				}
				AudioManager.Instance.StopSound(nodeEffectConfig.LoopSound);
			}
			CanvasGroup canvasGroup = this.EffectPlayer.gameObject.GetOrAddComponent<CanvasGroup>();
			canvasGroup.enabled = true;
			canvasGroup.DOKill(true);
			canvasGroup.DOFade(0f, 1f).OnComplete(delegate
			{
				this.EffectPlayer.ReturnEffectObject(this._effectObj);
				canvasGroup.enabled = false;
				canvasGroup.alpha = 1f;
				this._effectObj = null;
				this.DebateNodeEffectState = DebateNodeEffectState.Invalid;
			}).SetTarget(this._refers);
		}
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x0011574C File Offset: 0x0011394C
	public void RemoveEffect()
	{
		this._refers.DOKill(false);
		bool flag = this._effectObj == null;
		if (!flag)
		{
			DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[this.DebateNodeEffectState.TemplateId];
			bool flag2 = nodeEffectConfig == null;
			if (!flag2)
			{
				AudioManager.Instance.StopSound(nodeEffectConfig.LoopSound);
				this.EffectPlayer.ReturnEffectObject(this._effectObj);
				this._effectObj = null;
				this.DebateNodeEffectState = DebateNodeEffectState.Invalid;
			}
		}
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x001157CC File Offset: 0x001139CC
	private void RefreshTip()
	{
		TooltipInvoker tip = this._refers.CGet<TooltipInvoker>("Tip");
		tip.enabled = (this.DebateNode.EffectState.TemplateId >= 0);
		bool flag = !tip.enabled;
		if (!flag)
		{
			tip.Type = TipType.LifeSkillCombatBlock;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tip.RuntimeParam.Set<DebateNodeEffectState>("EffectState", this.DebateNode.EffectState);
		}
	}

	// Token: 0x04001BF4 RID: 7156
	private Refers _refers;

	// Token: 0x04001BF7 RID: 7159
	private GameObject _effectObj;

	// Token: 0x04001BF8 RID: 7160
	private Action<bool> _onStrategySelectTarget;

	// Token: 0x04001BFB RID: 7163
	public const float EffectAddedAnimDuration = 0.2f;

	// Token: 0x04001BFC RID: 7164
	public const float EffectTriggeredAnimDuration = 1f;

	// Token: 0x04001BFD RID: 7165
	public const float EffectTriggeredExtraAnimDuration = 1f;
}
