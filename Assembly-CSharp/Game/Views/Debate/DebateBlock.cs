using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu.Debate;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000A99 RID: 2713
	public class DebateBlock : MonoBehaviour, IDebateSelectable
	{
		// Token: 0x17000E8A RID: 3722
		// (get) Token: 0x060084AF RID: 33967 RVA: 0x003DB48E File Offset: 0x003D968E
		// (set) Token: 0x060084B0 RID: 33968 RVA: 0x003DB496 File Offset: 0x003D9696
		public Vector2Int Position { get; private set; }

		// Token: 0x17000E8B RID: 3723
		// (get) Token: 0x060084B1 RID: 33969 RVA: 0x003DB49F File Offset: 0x003D969F
		// (set) Token: 0x060084B2 RID: 33970 RVA: 0x003DB4A7 File Offset: 0x003D96A7
		public bool IsSelf { get; private set; }

		// Token: 0x17000E8C RID: 3724
		// (get) Token: 0x060084B3 RID: 33971 RVA: 0x003DB4B0 File Offset: 0x003D96B0
		public RectTransform RectTrans
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x17000E8D RID: 3725
		// (get) Token: 0x060084B4 RID: 33972 RVA: 0x003DB4BD File Offset: 0x003D96BD
		// (set) Token: 0x060084B5 RID: 33973 RVA: 0x003DB4C5 File Offset: 0x003D96C5
		public DebateNode DebateNode { get; private set; }

		// Token: 0x17000E8E RID: 3726
		// (get) Token: 0x060084B6 RID: 33974 RVA: 0x003DB4CE File Offset: 0x003D96CE
		// (set) Token: 0x060084B7 RID: 33975 RVA: 0x003DB4D6 File Offset: 0x003D96D6
		public DebateNodeEffectState DebateNodeEffectState { get; private set; } = DebateNodeEffectState.Invalid;

		// Token: 0x17000E8F RID: 3727
		// (get) Token: 0x060084B8 RID: 33976 RVA: 0x003DB4DF File Offset: 0x003D96DF
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x060084B9 RID: 33977 RVA: 0x003DB4E8 File Offset: 0x003D96E8
		public void Init(Vector2Int position, bool isSelf, RectTransform operationLayout)
		{
			this.Position = position;
			this.IsSelf = isSelf;
			this.button.interactable = false;
			this.button.ClearAndAddListener(new Action(this.OnClick));
			this.strategyTargetMark.SetActive(false);
			this.SetSelected(false);
			this.RefreshBack();
			this.hover.transform.SetParent(operationLayout);
			this.selectedMark.transform.SetParent(operationLayout);
		}

		// Token: 0x060084BA RID: 33978 RVA: 0x003DB56C File Offset: 0x003D976C
		public void Refresh(DebateNode debateNode)
		{
			this.DebateNode = debateNode;
			this.button.interactable = (this.IsSelf && this.DebateNode.TaiwuCanMakeMove && (this.Model.DebateGame.GetPlayerCanMakeMove(true) || this.Model.DebateGame.GetNodeIsContainingEffect(this.DebateNode.Coordinate, 41)) && this.Model.IsTaiwuRound);
			this.RefreshBack();
			this.RefreshTip();
		}

		// Token: 0x060084BB RID: 33979 RVA: 0x003DB5F4 File Offset: 0x003D97F4
		private void RefreshBack()
		{
			bool flag = this.DebateNode == null;
			if (!flag)
			{
				Sprite sprite = this.IsSelf ? (this.button.interactable ? this.spriteSelfNormal : this.spriteSelfDisable) : this.spriteEnemyNormal;
				this.imageBack.sprite = sprite;
				string scoreSpriteName = (this.Position.x == 0 || this.Position.x == DebateConstants.DebateLineNodeCount - 1) ? DebatePlayer.GetScoreSpriteName(true, this.IsSelf, this.Model.LifeSkillType) : string.Empty;
				this.imageScore.SetSprite(scoreSpriteName, false, null);
			}
		}

		// Token: 0x060084BC RID: 33980 RVA: 0x003DB6AC File Offset: 0x003D98AC
		public void OnClick()
		{
			bool flag = this._onStrategySelectTarget != null;
			if (flag)
			{
				bool isSelected = !this.selectedMark.activeSelf;
				this.strategyTargetMark.SetActive(!isSelected);
				this.SetSelected(isSelected);
				this._onStrategySelectTarget(isSelected);
			}
			else
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Block", this);
				GEvent.OnEvent(UiEvents.CombatLifeSkillClickBlock, args);
			}
		}

		// Token: 0x060084BD RID: 33981 RVA: 0x003DB724 File Offset: 0x003D9924
		public void ShowStrategyTargetMark(bool show, Action<bool> onClick)
		{
			this.button.interactable = show;
			this.strategyTargetMark.SetActive(show);
			this._onStrategySelectTarget = onClick;
			bool flag = !show;
			if (flag)
			{
				this.SetSelected(false);
			}
		}

		// Token: 0x060084BE RID: 33982 RVA: 0x003DB763 File Offset: 0x003D9963
		public void SetSelected(bool isSelected)
		{
			this.selectedMark.SetActive(isSelected);
		}

		// Token: 0x060084BF RID: 33983 RVA: 0x003DB773 File Offset: 0x003D9973
		public Transform GetTransform()
		{
			return this.RectTrans;
		}

		// Token: 0x060084C0 RID: 33984 RVA: 0x003DB77C File Offset: 0x003D997C
		public void PlayEffectAddedAnim(DebateNodeEffectState nodeEffectState)
		{
			this.DebateNodeEffectState = new DebateNodeEffectState(nodeEffectState);
			AudioManager.Instance.PlaySound(ViewDebate.SoundAddNodeEffect, false, true);
			int index = nodeEffectState.TemplateId + 1;
			string createAnimName = string.Format("eff_lifeskillcombat_5dalei_tongyong{0}", index);
			this.effectPlayer.PlayEffectAt(this.RectTrans, createAnimName, 0.2f, false);
			DOVirtual.DelayedCall(0.2f, delegate
			{
				bool flag = !this.gameObject || !this.gameObject.activeSelf;
				if (!flag)
				{
					string loopAnimName = string.Format("eff_lifeskillcombat_5dalei_{0}xh", index);
					this._effectObj = this.effectPlayer.PlayEffectAt(this.RectTrans, loopAnimName, -1f, false);
					DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[nodeEffectState.TemplateId];
					AudioManager.Instance.PlaySoundNoRepeat(nodeEffectConfig.LoopSound, 100, true, true);
				}
			}, true).SetTarget(base.gameObject);
		}

		// Token: 0x060084C1 RID: 33985 RVA: 0x003DB828 File Offset: 0x003D9A28
		public static bool HasExtraTriggeredAnim(short nodeEffectTemplateId)
		{
			int index = (int)(nodeEffectTemplateId + 1);
			return index == 2 || index == 5;
		}

		// Token: 0x060084C2 RID: 33986 RVA: 0x003DB84C File Offset: 0x003D9A4C
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
				CanvasGroup canvasGroup = this.effectPlayer.gameObject.GetOrAddComponent<CanvasGroup>();
				canvasGroup.enabled = true;
				canvasGroup.DOKill(true);
				canvasGroup.DOFade(0f, 1f).OnComplete(delegate
				{
					this.effectPlayer.ReturnEffectObject(this._effectObj);
					canvasGroup.enabled = false;
					canvasGroup.alpha = 1f;
					this._effectObj = null;
					this.DebateNodeEffectState = DebateNodeEffectState.Invalid;
				}).SetTarget(base.gameObject);
			}
		}

		// Token: 0x060084C3 RID: 33987 RVA: 0x003DB918 File Offset: 0x003D9B18
		public void RemoveEffect()
		{
			base.transform.DOKill(false);
			bool flag = this._effectObj == null;
			if (!flag)
			{
				DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[this.DebateNodeEffectState.TemplateId];
				bool flag2 = nodeEffectConfig == null;
				if (!flag2)
				{
					AudioManager.Instance.StopSound(nodeEffectConfig.LoopSound);
					this.effectPlayer.ReturnEffectObject(this._effectObj);
					this._effectObj = null;
					this.DebateNodeEffectState = DebateNodeEffectState.Invalid;
				}
			}
		}

		// Token: 0x060084C4 RID: 33988 RVA: 0x003DB998 File Offset: 0x003D9B98
		private void RefreshTip()
		{
			this.tip.enabled = (this.DebateNode.EffectState.TemplateId >= 0);
			bool flag = !this.tip.enabled;
			if (!flag)
			{
				this.tip.Type = TipType.LifeSkillCombatBlock;
				TooltipInvoker tooltipInvoker = this.tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.tip.RuntimeParam.Set<DebateNodeEffectState>("EffectState", this.DebateNode.EffectState);
			}
		}

		// Token: 0x040065BE RID: 26046
		[SerializeField]
		private CButton button;

		// Token: 0x040065BF RID: 26047
		[SerializeField]
		private GameObject strategyTargetMark;

		// Token: 0x040065C0 RID: 26048
		[SerializeField]
		private GameObject selectedMark;

		// Token: 0x040065C1 RID: 26049
		[SerializeField]
		private GameObject hover;

		// Token: 0x040065C2 RID: 26050
		[SerializeField]
		private CImage imageBack;

		// Token: 0x040065C3 RID: 26051
		[SerializeField]
		private CImage imageScore;

		// Token: 0x040065C4 RID: 26052
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x040065C5 RID: 26053
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x040065C6 RID: 26054
		[SerializeField]
		private Sprite spriteSelfNormal;

		// Token: 0x040065C7 RID: 26055
		[SerializeField]
		private Sprite spriteSelfDisable;

		// Token: 0x040065C8 RID: 26056
		[SerializeField]
		private Sprite spriteEnemyNormal;

		// Token: 0x040065C9 RID: 26057
		[SerializeField]
		private Sprite spriteEnemyDisable;

		// Token: 0x040065CC RID: 26060
		private GameObject _effectObj;

		// Token: 0x040065CD RID: 26061
		private Action<bool> _onStrategySelectTarget;

		// Token: 0x040065D0 RID: 26064
		public const float EffectAddedAnimDuration = 0.2f;

		// Token: 0x040065D1 RID: 26065
		public const float EffectTriggeredAnimDuration = 1f;

		// Token: 0x040065D2 RID: 26066
		public const float EffectTriggeredExtraAnimDuration = 1f;
	}
}
