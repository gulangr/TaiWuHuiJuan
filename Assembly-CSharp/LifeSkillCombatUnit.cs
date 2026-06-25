using System;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Taiwu.Debate;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000249 RID: 585
public class LifeSkillCombatUnit : ILifeSkillCombatSelectable
{
	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x0600263C RID: 9788 RVA: 0x00118B64 File Offset: 0x00116D64
	public Vector2Int Position
	{
		get
		{
			return new Vector2Int(this.Pawn.Coordinate.First, this.Pawn.Coordinate.Second);
		}
	}

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x0600263D RID: 9789 RVA: 0x00118B8B File Offset: 0x00116D8B
	public RectTransform RectTrans
	{
		get
		{
			return this._refers.GetComponent<RectTransform>();
		}
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x0600263E RID: 9790 RVA: 0x00118B98 File Offset: 0x00116D98
	private TooltipInvoker Tip
	{
		get
		{
			return this._refers.CGet<TooltipInvoker>("Tip");
		}
	}

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x0600263F RID: 9791 RVA: 0x00118BAA File Offset: 0x00116DAA
	private SkeletonGraphic SkeletonGraphic
	{
		get
		{
			return this._refers.CGet<SkeletonGraphic>("SkeletonGraphic");
		}
	}

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x06002640 RID: 9792 RVA: 0x00118BBC File Offset: 0x00116DBC
	private TextMeshProUGUI Id
	{
		get
		{
			return this._refers.CGet<TextMeshProUGUI>("Id");
		}
	}

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x06002641 RID: 9793 RVA: 0x00118BCE File Offset: 0x00116DCE
	private CButtonObsolete Button
	{
		get
		{
			return this._refers.CGet<CButtonObsolete>("Button");
		}
	}

	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x06002642 RID: 9794 RVA: 0x00118BE0 File Offset: 0x00116DE0
	private HorizontalLayoutGroup PowerNumberLayout
	{
		get
		{
			return this._refers.CGet<HorizontalLayoutGroup>("PowerNumberLayout");
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x06002643 RID: 9795 RVA: 0x00118BF2 File Offset: 0x00116DF2
	private GameObject UnrevealedPowerMark
	{
		get
		{
			return this._refers.CGet<GameObject>("UnrevealedPowerMark");
		}
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x06002644 RID: 9796 RVA: 0x00118C04 File Offset: 0x00116E04
	private GameObject StrategyTargetMark
	{
		get
		{
			return this._refers.CGet<GameObject>("StrategyTargetMark");
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06002645 RID: 9797 RVA: 0x00118C16 File Offset: 0x00116E16
	private GameObject SelectedMark
	{
		get
		{
			return this._refers.CGet<GameObject>("SelectedMark");
		}
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x06002646 RID: 9798 RVA: 0x00118C28 File Offset: 0x00116E28
	private RectTransform StrategyLayout
	{
		get
		{
			return this._refers.CGet<RectTransform>("StrategyLayout");
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x06002647 RID: 9799 RVA: 0x00118C3A File Offset: 0x00116E3A
	public RectTransform ConflictEffectRoot
	{
		get
		{
			return this._refers.CGet<RectTransform>("ConflictEffectRoot");
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06002648 RID: 9800 RVA: 0x00118C4C File Offset: 0x00116E4C
	public RectTransform ConflictDrawEffectRoot
	{
		get
		{
			return this._refers.CGet<RectTransform>("ConflictDrawEffectRoot");
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x06002649 RID: 9801 RVA: 0x00118C5E File Offset: 0x00116E5E
	public RectTransform DestroyEffectRoot
	{
		get
		{
			return this._refers.CGet<RectTransform>("DestroyEffectRoot");
		}
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x0600264A RID: 9802 RVA: 0x00118C70 File Offset: 0x00116E70
	public EffectPlayer EffectPlayer
	{
		get
		{
			return this._refers.CGet<EffectPlayer>("EffectPlayer");
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x0600264B RID: 9803 RVA: 0x00118C82 File Offset: 0x00116E82
	private PointerTrigger PointerTrigger
	{
		get
		{
			return this._refers.CGet<PointerTrigger>("PointerTrigger");
		}
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x0600264C RID: 9804 RVA: 0x00118C94 File Offset: 0x00116E94
	private GameObject Hover
	{
		get
		{
			return this._refers.CGet<GameObject>("Hover");
		}
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x0600264D RID: 9805 RVA: 0x00118CA6 File Offset: 0x00116EA6
	private GameObject NpcUnrevealed
	{
		get
		{
			return this._refers.CGet<GameObject>("NpcUnrevealed");
		}
	}

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x0600264E RID: 9806 RVA: 0x00118CB8 File Offset: 0x00116EB8
	private ParticleSystem SelfRevealParticle
	{
		get
		{
			return this._refers.CGet<ParticleSystem>("SelfPowerRevealParticle");
		}
	}

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x0600264F RID: 9807 RVA: 0x00118CCA File Offset: 0x00116ECA
	private ParticleSystem EnemyRevealParticle
	{
		get
		{
			return this._refers.CGet<ParticleSystem>("EnemyPowerRevealParticle");
		}
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06002650 RID: 9808 RVA: 0x00118CDC File Offset: 0x00116EDC
	private SkeletonGraphic NpcRevealAnim
	{
		get
		{
			return this._refers.CGet<SkeletonGraphic>("NpcRevealAnim");
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x06002651 RID: 9809 RVA: 0x00118CEE File Offset: 0x00116EEE
	// (set) Token: 0x06002652 RID: 9810 RVA: 0x00118CF6 File Offset: 0x00116EF6
	public Pawn Pawn { get; private set; }

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x06002653 RID: 9811 RVA: 0x00118CFF File Offset: 0x00116EFF
	// (set) Token: 0x06002654 RID: 9812 RVA: 0x00118D07 File Offset: 0x00116F07
	public bool IsRevealed { get; private set; }

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002655 RID: 9813 RVA: 0x00118D10 File Offset: 0x00116F10
	// (set) Token: 0x06002656 RID: 9814 RVA: 0x00118D18 File Offset: 0x00116F18
	public int Power { get; private set; }

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002657 RID: 9815 RVA: 0x00118D21 File Offset: 0x00116F21
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x00118D28 File Offset: 0x00116F28
	public LifeSkillCombatUnit(Refers refers, Pawn pawn, int bases)
	{
		this._refers = refers;
		this.SetData(pawn, true, true);
		this.RefreshPower(bases, true);
		this.PointerTrigger.EnterEvent.RemoveAllListeners();
		this.PointerTrigger.EnterEvent.AddListener(delegate()
		{
			this.Hover.SetActive(true);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Unit", this).Set("IsEnter", true);
			GEvent.OnEvent(UiEvents.CombatLifeSkillHoverUnit, args);
		});
		this.PointerTrigger.ExitEvent.RemoveAllListeners();
		this.PointerTrigger.ExitEvent.AddListener(delegate()
		{
			this.Hover.SetActive(false);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Unit", this).Set("IsEnter", false);
			GEvent.OnEvent(UiEvents.CombatLifeSkillHoverUnit, args);
		});
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x00118DD4 File Offset: 0x00116FD4
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
		this.Button.interactable = false;
		this.Button.ClearAndAddListener(new Action(this.OnClick));
		this.StrategyTargetMark.SetActive(false);
		this.SetSelected(false);
		string skinName = this.Pawn.IsOwnedByTaiwu ? string.Format("lifeskillcombat_chesspieces_{0}", this.Model.LifeSkillType) : string.Format("lifeskillcombat_redchesspieces_{0}", this.Model.LifeSkillType);
		Skin skin = this.SkeletonGraphic.Skeleton.Data.FindSkin(skinName);
		this.SkeletonGraphic.Skeleton.SetSkin(skin);
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x00118ED8 File Offset: 0x001170D8
	private void OnClick()
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
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Unit", this);
			GEvent.OnEvent(UiEvents.CombatLifeSkillClickUnit, args);
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x0600265B RID: 9819 RVA: 0x00118F50 File Offset: 0x00117150
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

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x0600265C RID: 9820 RVA: 0x00118FB7 File Offset: 0x001171B7
	public int MaxStrategyCount
	{
		get
		{
			return DebateConstants.PawnStrategyLimit;
		}
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x00118FC0 File Offset: 0x001171C0
	public void PlayRemoveStrategyAnim(int index, bool isCastedByTaiwu)
	{
		bool flag = this.StrategyLayout == null;
		if (!flag)
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundRemoveUnitStrategy, false, true);
			Refers strategyRefers = this.StrategyLayout.GetChild(index).GetComponent<Refers>();
			strategyRefers.CGet<CImage>("Image").SetAlpha(0f);
			strategyRefers.CGet<GameObject>("Unrevealed").SetActive(false);
			string effectKey = isCastedByTaiwu ? "SelfRemovedEffect" : "EnemyRemovedEffect";
			UIParticle removedEffect = strategyRefers.CGet<UIParticle>(effectKey);
			removedEffect.gameObject.SetActive(true);
			removedEffect.Play();
		}
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x00119058 File Offset: 0x00117258
	public void PlayAddStrategyAnim(int index, bool revealed, bool isCastedByTaiwu)
	{
		bool flag = this.StrategyLayout == null;
		if (!flag)
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundAddUnitStrategy, false, true);
			Refers strategyRefers = this.StrategyLayout.GetChild(index).GetComponent<Refers>();
			strategyRefers.gameObject.SetActive(true);
			CImage image = strategyRefers.CGet<CImage>("Image");
			string spName = this.GetStrategyImage(false, false, isCastedByTaiwu);
			image.SetSprite(spName, false, null);
			strategyRefers.CGet<GameObject>("Unrevealed").SetActive(!revealed);
			UIParticle addedEffect = strategyRefers.CGet<UIParticle>(isCastedByTaiwu ? "SelfAddedEffect" : "EnemyAddedEffect");
			addedEffect.gameObject.SetActive(true);
			addedEffect.Play();
			DOVirtual.DelayedCall(0.2f, delegate
			{
				image.SetAlpha(1f);
			}, false);
		}
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x00119134 File Offset: 0x00117334
	public void RefreshStrategy(int selectedSlotCount = 0, bool isCreate = false, bool isPreview = false)
	{
		bool flag = this.StrategyLayout == null;
		if (!flag)
		{
			bool hasStrategy = this.CurStrategyCount > 0 || selectedSlotCount > 0;
			bool flag2 = !hasStrategy;
			if (flag2)
			{
				for (int i = 0; i < this._strategyRevealStateArray.Length; i++)
				{
					this._strategyRevealStateArray[i] = false;
					this._strategyNpcRevealStateArray[i] = false;
				}
			}
			for (int j = 0; j < this.StrategyLayout.transform.childCount; j++)
			{
				Refers strategyRefers = this.StrategyLayout.GetChild(j).GetComponent<Refers>();
				int id = this.Pawn.Strategies[j];
				ActivatedStrategy strategy;
				bool valid = this.Model.DebateGame.ActivatedStrategies.TryGetValue(id, out strategy) && strategy != null && !isCreate;
				bool isSlotSelected = !valid && this.StrategyLayout.transform.childCount - j <= selectedSlotCount && selectedSlotCount > 0;
				bool isSlotPreviewed = !valid && !isSlotSelected && isPreview;
				bool show = valid || isSlotSelected || isSlotPreviewed;
				strategyRefers.CGet<UIParticle>("SelfAddedEffect").gameObject.SetActive(false);
				strategyRefers.CGet<UIParticle>("EnemyAddedEffect").gameObject.SetActive(false);
				strategyRefers.CGet<UIParticle>("SelfRemovedEffect").gameObject.SetActive(false);
				strategyRefers.CGet<UIParticle>("EnemyRemovedEffect").gameObject.SetActive(false);
				strategyRefers.gameObject.SetActive(show);
				bool flag3 = !show;
				if (flag3)
				{
					this._strategyRevealStateArray[j] = false;
				}
				else
				{
					bool isCastedByTaiwu = strategy != null && strategy.IsCastedByTaiwu;
					strategyRefers.CGet<CImage>("Image").SetAlpha(1f);
					GameObject unrevealed = strategyRefers.CGet<GameObject>("Unrevealed");
					TooltipInvoker tip = strategyRefers.CGet<TooltipInvoker>("Tip");
					CImage image = strategyRefers.CGet<CImage>("Image");
					string spName = this.GetStrategyImage(isSlotPreviewed, isSlotSelected, isCastedByTaiwu);
					image.SetSprite(spName, false, null);
					bool flag4 = isSlotSelected || isSlotPreviewed;
					if (flag4)
					{
						unrevealed.SetActive(false);
						tip.enabled = false;
					}
					else
					{
						bool needPlayAnim = !this._strategyRevealStateArray[j] && strategy.IsRevealed;
						bool flag5 = needPlayAnim;
						if (flag5)
						{
							this._needPlayRevealSound = true;
							this.EffectPlayer.PlayEffectAt(strategyRefers.transform, "LifeSkillCombatUnitRevealEffect", 0.5f, false);
						}
						bool isRevealed = strategy.IsRevealed || strategy.IsCastedByTaiwu || this.Model.ShowHiddenInfo;
						this._strategyRevealStateArray[j] = isRevealed;
						unrevealed.SetActive(!isRevealed);
						string[] presetParam = tip.PresetParam;
						bool flag6 = presetParam == null || presetParam.Length != 2;
						if (flag6)
						{
							tip.PresetParam = new string[2];
						}
						DebateStrategyItem config = strategy.GetConfig();
						tip.PresetParam[0] = config.Name;
						tip.PresetParam[1] = config.PawnEffectDesc;
						tip.enabled = isRevealed;
					}
					GameObject npcUnrevealed = strategyRefers.CGet<GameObject>("NpcUnrevealed");
					ParticleSystem npcRevealParticle = strategyRefers.CGet<ParticleSystem>("NpcRevealParticle");
					npcRevealParticle.Stop();
					bool flag7 = isCastedByTaiwu;
					if (flag7)
					{
						bool flag8 = !strategy.IsRevealed;
						if (flag8)
						{
							npcUnrevealed.SetActive(true);
						}
						else
						{
							npcUnrevealed.SetActive(false);
							bool flag9 = !this._strategyNpcRevealStateArray[j];
							if (flag9)
							{
								this._strategyNpcRevealStateArray[j] = true;
								npcRevealParticle.Play();
								this._needPlayRevealSound = true;
							}
						}
					}
					else
					{
						npcUnrevealed.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x001194E0 File Offset: 0x001176E0
	private string GetStrategyImage(bool isPreviewed, bool isSlotSelected, bool isCastedByTaiwu)
	{
		return isSlotSelected ? "lifeskillcombat_numberbase_4" : (isPreviewed ? "lifeskillcombat_numberbase_5" : (isCastedByTaiwu ? "lifeskillcombat_numberbase_0" : "lifeskillcombat_numberbase_1"));
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x00119515 File Offset: 0x00117715
	private void RefreshGrade()
	{
		this.PlayIdleAnim();
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x00119520 File Offset: 0x00117720
	public void PlayIdleAnim()
	{
		string animName = "idle_chess";
		this.SkeletonGraphic.AnimationState.SetAnimation(0, animName, true);
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x00119548 File Offset: 0x00117748
	public void PlayConflictAnim(bool isWin)
	{
		string animName = isWin ? "win_chess" : "lose_chess";
		this.SkeletonGraphic.AnimationState.SetAnimation(0, animName, false);
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x0011957C File Offset: 0x0011777C
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

	// Token: 0x06002665 RID: 9829 RVA: 0x00119600 File Offset: 0x00117800
	public void SetPowerRevealed()
	{
		this.Pawn.IsRevealed = true;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x00119610 File Offset: 0x00117810
	public void SetStrategyRevealed()
	{
		for (int i = 0; i < this.StrategyLayout.transform.childCount; i++)
		{
			Refers strategyRefers = this.StrategyLayout.GetChild(i).GetComponent<Refers>();
			bool flag = !strategyRefers.gameObject.activeSelf;
			if (!flag)
			{
				strategyRefers.CGet<CImage>("Image").SetAlpha(1f);
				GameObject unrevealed = strategyRefers.CGet<GameObject>("Unrevealed");
				bool needPlayAnim = !this._strategyRevealStateArray[i];
				bool flag2 = needPlayAnim;
				if (flag2)
				{
					this._needPlayRevealSound = true;
					this.EffectPlayer.PlayEffectAt(strategyRefers.transform, "LifeSkillCombatUnitRevealEffect", 0.5f, false);
				}
				this._strategyRevealStateArray[i] = true;
				unrevealed.SetActive(false);
			}
		}
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x001196DC File Offset: 0x001178DC
	public void RefreshPower(int bases = -1, bool isCreate = false)
	{
		int power = Mathf.Max(0, (bases >= 0) ? bases : this.Model.DebateGame.GetPawnBases(this.Pawn.Id, -1, false, true));
		this.SelfRevealParticle.Stop();
		bool isOwnedByTaiwu = this.Pawn.IsOwnedByTaiwu;
		if (isOwnedByTaiwu)
		{
			bool flag = !this.Pawn.IsRevealed;
			if (flag)
			{
				this.NpcUnrevealed.SetActive(true);
			}
			else
			{
				this.NpcUnrevealed.SetActive(false);
				bool flag2 = !this._isNpcRevealed;
				if (flag2)
				{
					this._isNpcRevealed = true;
					this.SelfRevealParticle.Play();
					this._needPlayRevealSound = true;
				}
			}
		}
		else
		{
			this.NpcUnrevealed.SetActive(false);
		}
		bool isRevealed = this.Model.ShowHiddenInfo || this.Pawn.IsRevealed || this.Pawn.IsOwnedByTaiwu;
		bool flag3 = !this.Pawn.IsOwnedByTaiwu && !isRevealed;
		if (flag3)
		{
			this.UnrevealedPowerMark.SetActive(true);
			this.PowerNumberLayout.gameObject.SetActive(false);
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
				this.NpcRevealAnim.AnimationState.SetAnimation(0, "turn_2", false);
				this.EnemyRevealParticle.transform.parent.gameObject.SetActive(true);
				this.EnemyRevealParticle.Play();
				DOVirtual.DelayedCall(0.1f, delegate
				{
					this.UnrevealedPowerMark.SetActive(false);
				}, false).SetTarget(this._refers);
				DOVirtual.DelayedCall(0.22f, delegate
				{
					this.PowerNumberLayout.gameObject.SetActive(true);
				}, false).SetTarget(this._refers);
				DOVirtual.DelayedCall(2f, delegate
				{
					this.EnemyRevealParticle.transform.parent.gameObject.SetActive(false);
				}, false).SetTarget(this._refers);
			}
			else
			{
				this.UnrevealedPowerMark.SetActive(false);
				this.PowerNumberLayout.gameObject.SetActive(true);
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

	// Token: 0x06002668 RID: 9832 RVA: 0x00119960 File Offset: 0x00117B60
	public void PlayRevealSound()
	{
		bool flag = !this._needPlayRevealSound;
		if (!flag)
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundReveal, false, true);
			this._needPlayRevealSound = false;
		}
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x00119998 File Offset: 0x00117B98
	private void RefreshPowerNumber(int power)
	{
		int[] costNumbers = new int[4];
		int showingCount = 0;
		bool showWan = power > 10000;
		int numberWan = power / 10000;
		bool flag = showWan;
		if (flag)
		{
			showingCount++;
			power = numberWan;
		}
		this.PowerNumberLayout.transform.GetChild(this.PowerNumberLayout.transform.childCount - 1).gameObject.SetActive(showWan);
		string numberStr = power.ToString();
		int offset = costNumbers.Length - numberStr.Length;
		for (int i = 0; i < costNumbers.Length; i++)
		{
			bool flag2 = i < offset;
			if (flag2)
			{
				costNumbers[i] = 0;
			}
			else
			{
				int startIndex = i - offset;
				string str = numberStr.Substring(startIndex, 1);
				costNumbers[i] = int.Parse(str);
			}
		}
		int startShowIndex = -1;
		int gradeNumberIndex = showWan ? 1 : ((power > 0) ? 2 : 3);
		for (int j = 0; j < costNumbers.Length; j++)
		{
			int number = costNumbers[j];
			CImage image = this.PowerNumberLayout.transform.GetChild(j).GetComponent<CImage>();
			bool flag3 = number > 0 && startShowIndex < 0;
			if (flag3)
			{
				startShowIndex = j;
			}
			bool show = (startShowIndex >= 0 && j >= startShowIndex) || (power == 0 && j == costNumbers.Length - 1);
			image.gameObject.SetActive(show);
			bool flag4 = show;
			if (flag4)
			{
				image.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", gradeNumberIndex, number), false, null);
				showingCount++;
			}
		}
		if (!true)
		{
		}
		float num;
		switch (showingCount)
		{
		case 1:
			num = 1f;
			break;
		case 2:
			num = 0.8f;
			break;
		case 3:
			num = 0.6f;
			break;
		case 4:
			num = 0.45f;
			break;
		default:
			num = 0.4f;
			break;
		}
		if (!true)
		{
		}
		float scaleValue = num;
		this.PowerNumberLayout.transform.localScale = Vector3.one * scaleValue;
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x00119B97 File Offset: 0x00117D97
	public void ShowStrategyTargetMark(bool show, Action<bool> onClick)
	{
		this.ShowStrategyTargetMark(show, onClick, 0);
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x00119BA4 File Offset: 0x00117DA4
	public void ShowStrategyTargetMark(bool show, Action<bool> onClick, int selectedSlotCount)
	{
		if (show)
		{
			this.RefreshStrategy(selectedSlotCount, false, true);
		}
		this.Button.interactable = show;
		this.StrategyTargetMark.SetActive(show);
		this._onStrategySelectTarget = onClick;
		bool flag = !show;
		if (flag)
		{
			this.SetData(this.Pawn, false, true);
		}
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x00119BFB File Offset: 0x00117DFB
	public void SetSelected(bool isSelected)
	{
		this.SelectedMark.SetActive(isSelected);
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x00119C0C File Offset: 0x00117E0C
	private void RefreshDebugInfo()
	{
		this.Id.text = string.Format("id={0}", this.Pawn.Id);
		this.Id.gameObject.SetActive(this.Model.ShowHiddenInfo);
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x00119C5C File Offset: 0x00117E5C
	private void RefreshTip()
	{
		this.Tip.Type = TipType.LifeSkillCombatUnit;
		TooltipInvoker tip = this.Tip;
		if (tip.RuntimeParam == null)
		{
			tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		this.Tip.RuntimeParam.SetObject("Pawn", this.Pawn);
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x00119CB3 File Offset: 0x00117EB3
	public void PlayProtectEffect()
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundProtectUnit, false, true);
		this.EffectPlayer.PlayEffectAt(this.RectTrans, "LifeSkillCombatUnitProtectEffect", 2f, false);
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x00119CE5 File Offset: 0x00117EE5
	public Transform GetTransform()
	{
		return this.RectTrans;
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x00119CF0 File Offset: 0x00117EF0
	public static string GetSpriteName(bool isTaiwu, sbyte lifeSkillType)
	{
		return isTaiwu ? string.Format("lifeskillcombat_chesspieces_{0}", lifeSkillType) : string.Format("lifeskillcombat_redchesspieces_{0}", lifeSkillType);
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x00119D28 File Offset: 0x00117F28
	private void PlayPowerChangeAnim(bool isAdd)
	{
		string soundName = isAdd ? UI_LifeSkillCombat2.SoundUnitPowerUp : UI_LifeSkillCombat2.SoundUnitPowerDown;
		AudioManager.Instance.PlaySound(soundName, false, true);
		string animName = isAdd ? "LifeSkillCombatUnitPowerUpEffect" : "LifeSkillCombatUnitPowerDownEffect";
		this.EffectPlayer.PlayEffectAt(this.PowerNumberLayout.transform, animName, 1f, false);
		this.PowerNumberLayout.DOKill(true);
		DOVirtual.DelayedCall(0.2f, delegate
		{
			this.RefreshPowerNumber(this.Power);
		}, false).SetTarget(this.PowerNumberLayout);
	}

	// Token: 0x04001C44 RID: 7236
	private Refers _refers;

	// Token: 0x04001C47 RID: 7239
	private bool _isNpcRevealed;

	// Token: 0x04001C48 RID: 7240
	private bool _needPlayRevealSound;

	// Token: 0x04001C4A RID: 7242
	public const float ConflictAnimDuration = 1f;

	// Token: 0x04001C4B RID: 7243
	public const float RevealAnimDuration = 0.5f;

	// Token: 0x04001C4C RID: 7244
	public const float DeleteByStrategyAnimDuration = 0.5f;

	// Token: 0x04001C4D RID: 7245
	public const float CreateAnimDuration = 0.2f;

	// Token: 0x04001C4E RID: 7246
	public const float CreateFailedAnimDuration = 1f;

	// Token: 0x04001C4F RID: 7247
	public const float UnrevealedMarkDisappear = 0.1f;

	// Token: 0x04001C50 RID: 7248
	public const float NumberLayoutAppear = 0.22f;

	// Token: 0x04001C51 RID: 7249
	public const float EnemyNumberEffectDuration = 2f;

	// Token: 0x04001C52 RID: 7250
	private Action<bool> _onStrategySelectTarget;

	// Token: 0x04001C53 RID: 7251
	private readonly bool[] _strategyRevealStateArray = new bool[DebateConstants.PawnStrategyLimit];

	// Token: 0x04001C54 RID: 7252
	private readonly bool[] _strategyNpcRevealStateArray = new bool[DebateConstants.PawnStrategyLimit];

	// Token: 0x04001C55 RID: 7253
	public const string RevealEffectName = "LifeSkillCombatUnitRevealEffect";

	// Token: 0x04001C56 RID: 7254
	public const string ProtectEffectName = "LifeSkillCombatUnitProtectEffect";
}
