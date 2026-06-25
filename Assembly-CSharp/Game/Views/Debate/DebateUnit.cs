using System;
using System.Linq;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu.Debate;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA1 RID: 2721
	public class DebateUnit : MonoBehaviour, IDebateSelectable
	{
		// Token: 0x17000EAC RID: 3756
		// (get) Token: 0x0600855E RID: 34142 RVA: 0x003DF95E File Offset: 0x003DDB5E
		public RectTransform RectTrans
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x17000EAD RID: 3757
		// (get) Token: 0x0600855F RID: 34143 RVA: 0x003DF96B File Offset: 0x003DDB6B
		public RectTransform ConflictEffectRoot
		{
			get
			{
				return this.conflictEffectRoot;
			}
		}

		// Token: 0x17000EAE RID: 3758
		// (get) Token: 0x06008560 RID: 34144 RVA: 0x003DF973 File Offset: 0x003DDB73
		public RectTransform ConflictDrawEffectRoot
		{
			get
			{
				return this.conflictDrawEffectRoot;
			}
		}

		// Token: 0x17000EAF RID: 3759
		// (get) Token: 0x06008561 RID: 34145 RVA: 0x003DF97B File Offset: 0x003DDB7B
		public RectTransform DestroyEffectRoot
		{
			get
			{
				return this.destroyEffectRoot;
			}
		}

		// Token: 0x17000EB0 RID: 3760
		// (get) Token: 0x06008562 RID: 34146 RVA: 0x003DF983 File Offset: 0x003DDB83
		public Vector2Int Position
		{
			get
			{
				return new Vector2Int(this.Pawn.Coordinate.First, this.Pawn.Coordinate.Second);
			}
		}

		// Token: 0x17000EB1 RID: 3761
		// (get) Token: 0x06008563 RID: 34147 RVA: 0x003DF9AA File Offset: 0x003DDBAA
		// (set) Token: 0x06008564 RID: 34148 RVA: 0x003DF9B2 File Offset: 0x003DDBB2
		public Pawn Pawn { get; private set; }

		// Token: 0x17000EB2 RID: 3762
		// (get) Token: 0x06008565 RID: 34149 RVA: 0x003DF9BB File Offset: 0x003DDBBB
		// (set) Token: 0x06008566 RID: 34150 RVA: 0x003DF9C3 File Offset: 0x003DDBC3
		public bool IsRevealed { get; private set; }

		// Token: 0x17000EB3 RID: 3763
		// (get) Token: 0x06008567 RID: 34151 RVA: 0x003DF9CC File Offset: 0x003DDBCC
		// (set) Token: 0x06008568 RID: 34152 RVA: 0x003DF9D4 File Offset: 0x003DDBD4
		public int Power { get; private set; }

		// Token: 0x17000EB4 RID: 3764
		// (get) Token: 0x06008569 RID: 34153 RVA: 0x003DF9DD File Offset: 0x003DDBDD
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x0600856A RID: 34154 RVA: 0x003DF9E4 File Offset: 0x003DDBE4
		public void Init(Pawn pawn, int bases)
		{
			this.SetData(pawn, true, true);
			this.RefreshPower(bases, true);
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.hover.SetActive(true);
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Unit", this).Set("IsEnter", true);
				GEvent.OnEvent(UiEvents.CombatLifeSkillHoverUnit, args);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.hover.SetActive(false);
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Unit", this).Set("IsEnter", false);
				GEvent.OnEvent(UiEvents.CombatLifeSkillHoverUnit, args);
			});
			this.selfRevealParticle.gameObject.SetActive(false);
			this.enemyRevealParticle.gameObject.SetActive(false);
		}

		// Token: 0x0600856B RID: 34155 RVA: 0x003DFA88 File Offset: 0x003DDC88
		public void SetData(Pawn pawn, bool isCreate = false, bool refreshStrategy = true)
		{
			this.RectTrans.localScale = Vector3.one;
			this.Pawn = pawn;
			this.RefreshGrade();
			this.RefreshDebugInfo();
			this.RefreshTip();
			if (refreshStrategy)
			{
				this.RefreshPower(-1, isCreate);
				this.RefreshStrategy(0, isCreate, false);
				this.PlayRevealSound();
			}
			this.button.interactable = false;
			this.button.ClearAndAddListener(new Action(this.OnClick));
			this.strategyTargetMark.SetActive(false);
			this.SetSelected(false);
			string skinName = this.Pawn.IsOwnedByTaiwu ? string.Format("lifeskillcombat_chesspieces_{0}", this.Model.LifeSkillType) : string.Format("lifeskillcombat_redchesspieces_{0}", this.Model.LifeSkillType);
			Skin skin = this.skeletonGraphic.Skeleton.Data.FindSkin(skinName);
			this.skeletonGraphic.Skeleton.SetSkin(skin);
		}

		// Token: 0x0600856C RID: 34156 RVA: 0x003DFB8C File Offset: 0x003DDD8C
		private void OnClick()
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
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Unit", this);
				GEvent.OnEvent(UiEvents.CombatLifeSkillClickUnit, args);
			}
		}

		// Token: 0x17000EB5 RID: 3765
		// (get) Token: 0x0600856D RID: 34157 RVA: 0x003DFC04 File Offset: 0x003DDE04
		public int CurStrategyCount
		{
			get
			{
				Pawn pawn = this.Pawn;
				int? num;
				if (pawn == null)
				{
					num = null;
				}
				else
				{
					int[] strategies = pawn.Strategies;
					if (strategies == null)
					{
						num = null;
					}
					else
					{
						num = new int?(strategies.Count((int s) => s >= 0));
					}
				}
				int? num2 = num;
				return num2.GetValueOrDefault();
			}
		}

		// Token: 0x17000EB6 RID: 3766
		// (get) Token: 0x0600856E RID: 34158 RVA: 0x003DFC6B File Offset: 0x003DDE6B
		public int MaxStrategyCount
		{
			get
			{
				return DebateConstants.PawnStrategyLimit;
			}
		}

		// Token: 0x0600856F RID: 34159 RVA: 0x003DFC74 File Offset: 0x003DDE74
		public void PlayRemoveStrategyAnim(int index, bool isCastedByTaiwu)
		{
			bool flag = this.strategyLayout == null;
			if (!flag)
			{
				DebateUnitStrategy strategy = this.strategyLayout.GetChild(index).GetComponent<DebateUnitStrategy>();
				strategy.PlayRemoveStrategyAnim(isCastedByTaiwu);
			}
		}

		// Token: 0x06008570 RID: 34160 RVA: 0x003DFCAC File Offset: 0x003DDEAC
		public void PlayAddStrategyAnim(int index, bool revealed, bool isCastedByTaiwu)
		{
			bool flag = this.strategyLayout == null;
			if (!flag)
			{
				DebateUnitStrategy strategy = this.strategyLayout.GetChild(index).GetComponent<DebateUnitStrategy>();
				strategy.PlayAddStrategyAnim(revealed, isCastedByTaiwu);
			}
		}

		// Token: 0x06008571 RID: 34161 RVA: 0x003DFCE4 File Offset: 0x003DDEE4
		public void RefreshStrategy(int selectedSlotCount = 0, bool isCreate = false, bool isPreview = false)
		{
			bool flag = this.strategyLayout == null;
			if (!flag)
			{
				int vlidCount = 0;
				for (int i = 0; i < this.strategyLayout.transform.childCount; i++)
				{
					int id = this.Pawn.Strategies[i];
					ActivatedStrategy strategy;
					bool valid = this.Model.DebateGame.ActivatedStrategies.TryGetValue(id, out strategy) && strategy != null && !isCreate;
					bool flag2 = valid;
					if (flag2)
					{
						vlidCount++;
					}
				}
				for (int j = 0; j < this.strategyLayout.transform.childCount; j++)
				{
					int id2 = this.Pawn.Strategies[j];
					ActivatedStrategy strategy2;
					bool valid2 = this.Model.DebateGame.ActivatedStrategies.TryGetValue(id2, out strategy2) && strategy2 != null && !isCreate;
					bool isSlotSelected = !valid2 && j == vlidCount && selectedSlotCount > 0;
					bool isSlotPreviewed = !valid2 && !isSlotSelected && isPreview;
					DebateUnitStrategy strategyRefers = this.strategyLayout.GetChild(j).GetComponent<DebateUnitStrategy>();
					this._needPlayRevealSound = strategyRefers.Refresh(strategy2, valid2, isSlotSelected, isSlotPreviewed);
				}
			}
		}

		// Token: 0x06008572 RID: 34162 RVA: 0x003DFE1A File Offset: 0x003DE01A
		private void RefreshGrade()
		{
			this.PlayIdleAnim();
		}

		// Token: 0x06008573 RID: 34163 RVA: 0x003DFE24 File Offset: 0x003DE024
		public void PlayIdleAnim()
		{
			string animName = "idle_chess";
			this.skeletonGraphic.AnimationState.SetAnimation(0, animName, true);
		}

		// Token: 0x06008574 RID: 34164 RVA: 0x003DFE4C File Offset: 0x003DE04C
		public void PlayConflictAnim(bool isWin)
		{
			string animName = isWin ? "win_chess" : "lose_chess";
			this.skeletonGraphic.AnimationState.SetAnimation(0, animName, false);
		}

		// Token: 0x06008575 RID: 34165 RVA: 0x003DFE80 File Offset: 0x003DE080
		public bool HasUnrevealedObject()
		{
			bool flag = !this.IsRevealed;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (int strategyId in this.Pawn.Strategies)
				{
					ActivatedStrategy strategy;
					bool valid = this.Model.DebateGame.ActivatedStrategies.TryGetValue(strategyId, out strategy) && strategy != null;
					bool flag2 = valid;
					if (flag2)
					{
						bool isRevealed = strategy.IsRevealed;
						if (isRevealed)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06008576 RID: 34166 RVA: 0x003DFF04 File Offset: 0x003DE104
		public void SetPowerRevealed()
		{
			this.Pawn.IsRevealed = true;
		}

		// Token: 0x06008577 RID: 34167 RVA: 0x003DFF14 File Offset: 0x003DE114
		public void SetStrategyRevealed()
		{
			for (int i = 0; i < this.strategyLayout.transform.childCount; i++)
			{
				DebateUnitStrategy strategyRefers = this.strategyLayout.GetChild(i).GetComponent<DebateUnitStrategy>();
				bool flag = !strategyRefers.gameObject.activeSelf;
				if (!flag)
				{
					bool needPlayAnim = strategyRefers.SetStrategyRevealed();
					bool flag2 = needPlayAnim;
					if (flag2)
					{
						this._needPlayRevealSound = true;
					}
				}
			}
		}

		// Token: 0x06008578 RID: 34168 RVA: 0x003DFF80 File Offset: 0x003DE180
		public void RefreshPower(int bases = -1, bool isCreate = false)
		{
			this.selfRevealParticle.Stop();
			this.selfRevealParticle.gameObject.SetActive(false);
			this.enemyRevealParticle.Stop();
			this.enemyRevealParticle.gameObject.SetActive(false);
			int power = Mathf.Max(0, (bases >= 0) ? bases : this.Model.DebateGame.GetPawnBases(this.Pawn.Id, -1, false, true));
			bool isOwnedByTaiwu = this.Pawn.IsOwnedByTaiwu;
			if (isOwnedByTaiwu)
			{
				bool flag = !this.Pawn.IsRevealed;
				if (flag)
				{
					this.npcRevealed.SetActive(false);
				}
				else
				{
					this.npcRevealed.SetActive(true);
					bool flag2 = !this._isNpcRevealed;
					if (flag2)
					{
						this._isNpcRevealed = true;
						this.selfRevealParticle.gameObject.SetActive(true);
						this.selfRevealParticle.Play();
						this._needPlayRevealSound = true;
					}
				}
			}
			else
			{
				this.npcRevealed.SetActive(false);
			}
			bool isRevealed = this.Model.ShowHiddenInfo || this.Pawn.IsRevealed || this.Pawn.IsOwnedByTaiwu;
			bool flag3 = !this.Pawn.IsOwnedByTaiwu && !isRevealed;
			if (flag3)
			{
				this.selfUnrevealed.SetActive(true);
				this.textPower.gameObject.SetActive(false);
				this.Power = power;
			}
			else
			{
				bool needPlayRevealedAnim = this.Pawn.IsRevealed && !this.IsRevealed;
				this.IsRevealed = isRevealed;
				bool flag4 = needPlayRevealedAnim;
				if (flag4)
				{
					this._needPlayRevealSound = true;
					this.npcRevealAnim.AnimationState.SetAnimation(0, "turn_2", false);
					this.enemyRevealParticle.gameObject.SetActive(true);
					this.enemyRevealParticle.Play();
					DOVirtual.DelayedCall(0.1f, delegate
					{
						this.selfUnrevealed.SetActive(false);
					}, false).SetTarget(base.gameObject);
					DOVirtual.DelayedCall(0.22f, delegate
					{
						this.textPower.gameObject.SetActive(true);
					}, false).SetTarget(base.gameObject);
					DOVirtual.DelayedCall(2f, delegate
					{
						this.enemyRevealParticle.gameObject.SetActive(false);
					}, false).SetTarget(base.gameObject);
				}
				else
				{
					this.selfUnrevealed.SetActive(false);
					this.textPower.gameObject.SetActive(true);
				}
				bool flag5 = power != this.Power && !isCreate && !needPlayRevealedAnim;
				if (flag5)
				{
					this.PlayPowerChangeAnim(power > this.Power);
					this.Power = power;
				}
				else
				{
					this.Power = power;
					this.RefreshPowerNumber(this.Power);
				}
			}
		}

		// Token: 0x06008579 RID: 34169 RVA: 0x003E023C File Offset: 0x003DE43C
		public void PlayRevealSound()
		{
			bool flag = !this._needPlayRevealSound;
			if (!flag)
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundReveal, false, true);
				this._needPlayRevealSound = false;
			}
		}

		// Token: 0x0600857A RID: 34170 RVA: 0x003E0272 File Offset: 0x003DE472
		private void RefreshPowerNumber(int power)
		{
			this.textPower.text = CommonUtils.GetDisplayStringForNum(power, 10000);
		}

		// Token: 0x0600857B RID: 34171 RVA: 0x003E028C File Offset: 0x003DE48C
		public void ShowStrategyTargetMark(bool show, Action<bool> onClick)
		{
			this.ShowStrategyTargetMark(show, onClick, 0);
		}

		// Token: 0x0600857C RID: 34172 RVA: 0x003E0298 File Offset: 0x003DE498
		public void ShowStrategyTargetMark(bool show, Action<bool> onClick, int selectedSlotCount)
		{
			if (show)
			{
				this.RefreshStrategy(selectedSlotCount, false, true);
			}
			this.button.interactable = show;
			this.strategyTargetMark.SetActive(show);
			this._onStrategySelectTarget = onClick;
			bool flag = !show;
			if (flag)
			{
				this.SetData(this.Pawn, false, true);
			}
		}

		// Token: 0x0600857D RID: 34173 RVA: 0x003E02EF File Offset: 0x003DE4EF
		public void SetSelected(bool isSelected)
		{
			this.selectedMark.SetActive(isSelected);
		}

		// Token: 0x0600857E RID: 34174 RVA: 0x003E0300 File Offset: 0x003DE500
		private void RefreshDebugInfo()
		{
			this.id.text = string.Format("id={0}", this.Pawn.Id);
			this.id.gameObject.SetActive(this.Model.ShowHiddenInfo);
		}

		// Token: 0x0600857F RID: 34175 RVA: 0x003E0350 File Offset: 0x003DE550
		private void RefreshTip()
		{
			this.tip.Type = TipType.LifeSkillCombatUnit;
			TooltipInvoker tooltipInvoker = this.tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.tip.RuntimeParam.SetObject("Pawn", this.Pawn);
		}

		// Token: 0x06008580 RID: 34176 RVA: 0x003E03A7 File Offset: 0x003DE5A7
		public void PlayProtectEffect()
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundProtectUnit, false, true);
			this.effectPlayer.PlayEffectAt(this.RectTrans, "LifeSkillCombatUnitProtectEffect", 2f, false);
		}

		// Token: 0x06008581 RID: 34177 RVA: 0x003E03D9 File Offset: 0x003DE5D9
		public Transform GetTransform()
		{
			return this.RectTrans;
		}

		// Token: 0x06008582 RID: 34178 RVA: 0x003E03E4 File Offset: 0x003DE5E4
		public static string GetSpriteName(bool isTaiwu, sbyte lifeSkillType)
		{
			return isTaiwu ? string.Format("lifeskillcombat_chesspieces_{0}", lifeSkillType) : string.Format("lifeskillcombat_redchesspieces_{0}", lifeSkillType);
		}

		// Token: 0x06008583 RID: 34179 RVA: 0x003E041C File Offset: 0x003DE61C
		private void PlayPowerChangeAnim(bool isAdd)
		{
			string soundName = isAdd ? ViewDebate.SoundUnitPowerUp : ViewDebate.SoundUnitPowerDown;
			AudioManager.Instance.PlaySound(soundName, false, true);
			string animName = isAdd ? "LifeSkillCombatUnitPowerUpEffect" : "LifeSkillCombatUnitPowerDownEffect";
			this.effectPlayer.PlayEffectAt(this.textPower.transform, animName, 1f, false);
			this.textPower.DOKill(true);
			DOVirtual.DelayedCall(0.2f, delegate
			{
				this.RefreshPowerNumber(this.Power);
			}, false).SetTarget(this.textPower);
		}

		// Token: 0x0400664B RID: 26187
		[Header("棋子形象与交互")]
		[SerializeField]
		private SkeletonGraphic skeletonGraphic;

		// Token: 0x0400664C RID: 26188
		[SerializeField]
		private CButton button;

		// Token: 0x0400664D RID: 26189
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x0400664E RID: 26190
		[SerializeField]
		private TextMeshProUGUI id;

		// Token: 0x0400664F RID: 26191
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04006650 RID: 26192
		[SerializeField]
		private GameObject hover;

		// Token: 0x04006651 RID: 26193
		[SerializeField]
		private GameObject selectedMark;

		// Token: 0x04006652 RID: 26194
		[SerializeField]
		private GameObject strategyTargetMark;

		// Token: 0x04006653 RID: 26195
		[Header("论据")]
		[SerializeField]
		private TextMeshProUGUI textPower;

		// Token: 0x04006654 RID: 26196
		[SerializeField]
		private GameObject selfUnrevealed;

		// Token: 0x04006655 RID: 26197
		[SerializeField]
		private GameObject npcRevealed;

		// Token: 0x04006656 RID: 26198
		[SerializeField]
		private UIParticle selfRevealParticle;

		// Token: 0x04006657 RID: 26199
		[SerializeField]
		private UIParticle enemyRevealParticle;

		// Token: 0x04006658 RID: 26200
		[SerializeField]
		private SkeletonGraphic npcRevealAnim;

		// Token: 0x04006659 RID: 26201
		[Header("其他")]
		[SerializeField]
		private RectTransform strategyLayout;

		// Token: 0x0400665A RID: 26202
		[SerializeField]
		private RectTransform conflictEffectRoot;

		// Token: 0x0400665B RID: 26203
		[SerializeField]
		private RectTransform conflictDrawEffectRoot;

		// Token: 0x0400665C RID: 26204
		[SerializeField]
		private RectTransform destroyEffectRoot;

		// Token: 0x0400665D RID: 26205
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x04006660 RID: 26208
		private bool _isNpcRevealed;

		// Token: 0x04006661 RID: 26209
		private bool _needPlayRevealSound;

		// Token: 0x04006663 RID: 26211
		public const float ConflictAnimDuration = 1f;

		// Token: 0x04006664 RID: 26212
		public const float RevealAnimDuration = 0.5f;

		// Token: 0x04006665 RID: 26213
		public const float DeleteByStrategyAnimDuration = 0.5f;

		// Token: 0x04006666 RID: 26214
		public const float CreateAnimDuration = 0.2f;

		// Token: 0x04006667 RID: 26215
		public const float CreateFailedAnimDuration = 1f;

		// Token: 0x04006668 RID: 26216
		public const float UnrevealedMarkDisappear = 0.1f;

		// Token: 0x04006669 RID: 26217
		public const float NumberLayoutAppear = 0.22f;

		// Token: 0x0400666A RID: 26218
		public const float EnemyNumberEffectDuration = 2f;

		// Token: 0x0400666B RID: 26219
		private Action<bool> _onStrategySelectTarget;

		// Token: 0x0400666C RID: 26220
		public const string ProtectEffectName = "LifeSkillCombatUnitProtectEffect";
	}
}
