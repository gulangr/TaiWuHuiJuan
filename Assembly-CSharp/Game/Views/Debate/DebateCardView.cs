using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Global;
using GameData.Domains.Taiwu.Debate;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Debate
{
	// Token: 0x02000A9D RID: 2717
	public class DebateCardView : MonoBehaviour, IDebateSelectable
	{
		// Token: 0x17000E9B RID: 3739
		// (get) Token: 0x060084F2 RID: 34034 RVA: 0x003DC5EA File Offset: 0x003DA7EA
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x17000E9C RID: 3740
		// (get) Token: 0x060084F3 RID: 34035 RVA: 0x003DC5F1 File Offset: 0x003DA7F1
		public DebateStrategyItem CardConfig
		{
			get
			{
				return this._cardConfig;
			}
		}

		// Token: 0x17000E9D RID: 3741
		// (get) Token: 0x060084F4 RID: 34036 RVA: 0x003DC5F9 File Offset: 0x003DA7F9
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x17000E9E RID: 3742
		// (get) Token: 0x060084F5 RID: 34037 RVA: 0x003DC606 File Offset: 0x003DA806
		public int EffectCardId
		{
			get
			{
				LifeSkillCombatCard effectCard = this._effectCard;
				return (int)((effectCard != null) ? effectCard.EffectCardId : -1);
			}
		}

		// Token: 0x17000E9F RID: 3743
		// (get) Token: 0x060084F6 RID: 34038 RVA: 0x003DC61A File Offset: 0x003DA81A
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x17000EA0 RID: 3744
		// (get) Token: 0x060084F7 RID: 34039 RVA: 0x003DC622 File Offset: 0x003DA822
		private GameObject SelectedMark
		{
			get
			{
				return this.selected;
			}
		}

		// Token: 0x17000EA1 RID: 3745
		// (get) Token: 0x060084F8 RID: 34040 RVA: 0x003DC62A File Offset: 0x003DA82A
		// (set) Token: 0x060084F9 RID: 34041 RVA: 0x003DC632 File Offset: 0x003DA832
		public int Index { get; private set; }

		// Token: 0x060084FA RID: 34042 RVA: 0x003DC63C File Offset: 0x003DA83C
		public void SetData(short strategyTemplateId, int index)
		{
			this.Index = index;
			this._cardConfig = DebateStrategy.Instance.GetItem(strategyTemplateId);
			bool isInLifeSkillCombat = UIManager.Instance.IsFocusElement(UIElement.Debate);
			this._isCostMeet = (!isInLifeSkillCombat || this.Model.CheckCost(this._cardConfig));
			List<StrategyTarget> list;
			this._isTargetMeet = (!isInLifeSkillCombat || this.Model.DebateGame.TryGetStrategyTarget(this._cardConfig.TemplateId, true, out list));
			this.textTitle.text = this._cardConfig.Name;
			LanguageRuleTips component = this.textTitle.GetComponent<LanguageRuleTips>();
			if (component != null)
			{
				component.Refresh();
			}
			this.RefreshTip();
			this.picture.SetTexture("ui9_tex_debate_card_picture_{0}".GetFormat(this._cardConfig.LifeSkillType));
			this.imageFrame.SetSprite("ui9_back_debate_card_frame_{0}".GetFormat((int)(this._cardConfig.Level - 1)), false, null);
			this.imageCover.SetSprite("ui9_back_debate_card_cover_{0}".GetFormat((int)(this._cardConfig.Level - 1)), false, null);
			this.imageEffectMark.SetSprite("ui9_icon_debate_card_effect_{0}".GetFormat(this._cardConfig.MarkType.ToInt()), true, null);
			this.imageTypeIcon.SetSprite(this.GetTypeIcon(), false, null);
			this.SetEnabled(false, false);
			this.RefreshCostNumber();
			this.SetSelected(false);
			this.strategyTargetMark.SetActive(false);
			bool flag = this.canvas;
			if (flag)
			{
				this.canvas.overrideSorting = false;
			}
			UnityEvent enterEvent = this.pointerTrigger.EnterEvent;
			bool flag2 = enterEvent == null;
			if (flag2)
			{
				enterEvent = new UnityEvent();
			}
			enterEvent.RemoveAllListeners();
			enterEvent.AddListener(new UnityAction(this.OnPointerTriggerEnter));
			UnityEvent exitEvent = this.pointerTrigger.ExitEvent;
			bool flag3 = exitEvent == null;
			if (flag3)
			{
				exitEvent = new UnityEvent();
			}
			exitEvent.RemoveAllListeners();
			exitEvent.AddListener(new UnityAction(this.OnPointerTriggerExit));
			this.ShowCover(false);
		}

		// Token: 0x060084FB RID: 34043 RVA: 0x003DC868 File Offset: 0x003DAA68
		public void SetOnClick(Action action)
		{
			this.Button.ClearAndAddListener(delegate
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(258);
				bool flag = this._onStrategySelectTarget != null;
				if (flag)
				{
					bool isSelected = !this.SelectedMark.activeSelf;
					this.strategyTargetMark.SetActive(!isSelected);
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
				}
			});
		}

		// Token: 0x060084FC RID: 34044 RVA: 0x003DC8A4 File Offset: 0x003DAAA4
		public void SetEnabled(bool canUseCard, bool selfIsTarget = false)
		{
			bool realCanUseCard = selfIsTarget ? canUseCard : (canUseCard && this._isCostMeet && this._isTargetMeet);
			this.SetStyleRoot(realCanUseCard);
			this.SetPointerTrigger(realCanUseCard);
			this.SetInteractable(realCanUseCard);
		}

		// Token: 0x060084FD RID: 34045 RVA: 0x003DC8E5 File Offset: 0x003DAAE5
		public void SetInteractable(bool interactable)
		{
			this.Button.interactable = interactable;
		}

		// Token: 0x060084FE RID: 34046 RVA: 0x003DC8F4 File Offset: 0x003DAAF4
		public void SetStyleRoot(bool interactable)
		{
			this.disableRoot.SetInteractable(interactable);
		}

		// Token: 0x060084FF RID: 34047 RVA: 0x003DC903 File Offset: 0x003DAB03
		public void SetPointerTrigger(bool enable)
		{
			this.pointerTrigger.enabled = enable;
		}

		// Token: 0x06008500 RID: 34048 RVA: 0x003DC914 File Offset: 0x003DAB14
		public void SetSelected(bool isSelected, bool hasScale)
		{
			this.selected.SetActive(isSelected);
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
		}

		// Token: 0x06008501 RID: 34049 RVA: 0x003DC958 File Offset: 0x003DAB58
		public void ShowStrategyTargetMark(bool show, Action<bool> onClick)
		{
			this.Button.interactable = show;
			this.strategyTargetMark.SetActive(show);
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

		// Token: 0x06008502 RID: 34050 RVA: 0x003DC9AF File Offset: 0x003DABAF
		public void SetSelected(bool isSelected)
		{
			this.SetSelected(isSelected, false);
		}

		// Token: 0x06008503 RID: 34051 RVA: 0x003DC9BC File Offset: 0x003DABBC
		public void OnPointerTriggerEnter()
		{
			bool flag = this.canvas;
			if (flag)
			{
				this.canvas.overrideSorting = true;
			}
			this.animRoot.DOKill(false);
			this.animRoot.DOAnchorPosY(this.hoverLocalMoveY, this.hoverDuration, false);
			this.animRoot.DOScale(this.hoverScaleY, this.hoverDuration);
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

		// Token: 0x06008504 RID: 34052 RVA: 0x003DCA88 File Offset: 0x003DAC88
		public void OnPointerTriggerExit()
		{
			bool flag = this.canvas;
			if (flag)
			{
				this.canvas.overrideSorting = false;
			}
			this.animRoot.DOKill(false);
			this.animRoot.DOAnchorPosY(0f, this.hoverDuration, false);
			this.animRoot.DOScale(1f, this.hoverDuration);
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

		// Token: 0x06008505 RID: 34053 RVA: 0x003DCB40 File Offset: 0x003DAD40
		private void RefreshTip()
		{
			this.tip.Type = TipType.LifeSkillCombatStrategy;
			TooltipInvoker tooltipInvoker = this.tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.tip.RuntimeParam.Set("TemplateId", this._cardConfig.TemplateId);
			this.tip.RuntimeParam.Set("IsPointMeet", this._isCostMeet);
			this.tip.RuntimeParam.Set("IsTargetMeet", this._isTargetMeet);
		}

		// Token: 0x06008506 RID: 34054 RVA: 0x003DCBD4 File Offset: 0x003DADD4
		public void SetNewEvent(Action action)
		{
			this._onShowNew = action;
		}

		// Token: 0x06008507 RID: 34055 RVA: 0x003DCBE0 File Offset: 0x003DADE0
		private string GetTypeIcon()
		{
			sbyte type = this._cardConfig.LifeSkillType;
			LifeSkillTypeItem config = LifeSkillType.Instance[type];
			return config.Icon;
		}

		// Token: 0x06008508 RID: 34056 RVA: 0x003DCC14 File Offset: 0x003DAE14
		private void RefreshCostNumber()
		{
			string color = this._isCostMeet ? "brightblue" : "brightred";
			this.textCost.text = this._cardConfig.UsedCost.ToString().SetColor(color);
		}

		// Token: 0x06008509 RID: 34057 RVA: 0x003DCC5C File Offset: 0x003DAE5C
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

		// Token: 0x0600850A RID: 34058 RVA: 0x003DCCB0 File Offset: 0x003DAEB0
		public void PlayUseEffect()
		{
			string effectName = this.GetUseEffectName();
			float duration = 0.7f;
			this.effectPlayer.PlayEffectAt(this.animRoot, effectName, duration, false);
		}

		// Token: 0x0600850B RID: 34059 RVA: 0x003DCCE0 File Offset: 0x003DAEE0
		public Transform GetTransform()
		{
			return base.transform;
		}

		// Token: 0x0600850C RID: 34060 RVA: 0x003DCCE8 File Offset: 0x003DAEE8
		public void ShowCover(bool show)
		{
			this.imageCover.gameObject.SetActive(show);
		}

		// Token: 0x040065E3 RID: 26083
		[SerializeField]
		private float hoverLocalMoveY = 100f;

		// Token: 0x040065E4 RID: 26084
		[SerializeField]
		private float selectedLocalMoveY = 20f;

		// Token: 0x040065E5 RID: 26085
		[SerializeField]
		private float hoverScaleY = 1.1f;

		// Token: 0x040065E6 RID: 26086
		[SerializeField]
		private float hoverDuration = 0.2f;

		// Token: 0x040065E7 RID: 26087
		[SerializeField]
		private CButton button;

		// Token: 0x040065E8 RID: 26088
		[SerializeField]
		private CRawImage picture;

		// Token: 0x040065E9 RID: 26089
		[SerializeField]
		private CImage imageEffectMark;

		// Token: 0x040065EA RID: 26090
		[SerializeField]
		private CImage imageFrame;

		// Token: 0x040065EB RID: 26091
		[SerializeField]
		private CImage imageCover;

		// Token: 0x040065EC RID: 26092
		[SerializeField]
		private CImage imageTypeIcon;

		// Token: 0x040065ED RID: 26093
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040065EE RID: 26094
		[SerializeField]
		private TextMeshProUGUI textCost;

		// Token: 0x040065EF RID: 26095
		[SerializeField]
		private GameObject strategyTargetMark;

		// Token: 0x040065F0 RID: 26096
		[SerializeField]
		private GameObject selected;

		// Token: 0x040065F1 RID: 26097
		[SerializeField]
		private HSVStyleRoot disableRoot;

		// Token: 0x040065F2 RID: 26098
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x040065F3 RID: 26099
		[SerializeField]
		private Canvas canvas;

		// Token: 0x040065F4 RID: 26100
		[SerializeField]
		private RectTransform animRoot;

		// Token: 0x040065F5 RID: 26101
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x040065F6 RID: 26102
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x040065F7 RID: 26103
		private LifeSkillCombatCard _effectCard;

		// Token: 0x040065F8 RID: 26104
		private DebateStrategyItem _cardConfig;

		// Token: 0x040065F9 RID: 26105
		private Action _onShowTip;

		// Token: 0x040065FA RID: 26106
		private Action _onHideTip;

		// Token: 0x040065FB RID: 26107
		private Action _onShowNew;

		// Token: 0x040065FC RID: 26108
		private Action<bool> _onStrategySelectTarget;

		// Token: 0x040065FE RID: 26110
		private bool _isCostMeet;

		// Token: 0x040065FF RID: 26111
		private bool _isTargetMeet;

		// Token: 0x04006600 RID: 26112
		public const float EnemyCardAnimDuration = 0.4f;

		// Token: 0x04006601 RID: 26113
		public const float UseCardAnimDuration = 0.7f;

		// Token: 0x04006602 RID: 26114
		public const float UseCardFailedAnimDuration = 1f;

		// Token: 0x04006603 RID: 26115
		public const float UseCardTargetPawnAnimDuration = 0.5f;
	}
}
