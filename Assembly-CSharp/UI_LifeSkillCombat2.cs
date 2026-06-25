using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.Tools;
using FrameWork.UISystem.Components;
using Game.Components.Combat;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Debate;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x0200024C RID: 588
public class UI_LifeSkillCombat2 : UIBase
{
	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x06002692 RID: 9874 RVA: 0x0011AE34 File Offset: 0x00119034
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x06002693 RID: 9875 RVA: 0x0011AE3B File Offset: 0x0011903B
	private GlobalSettings SettingData
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>();
		}
	}

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x06002694 RID: 9876 RVA: 0x0011AE42 File Offset: 0x00119042
	private global::DebateRecord SelfRecord
	{
		get
		{
			return base.CGet<global::DebateRecord>("SelfRecord");
		}
	}

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x06002695 RID: 9877 RVA: 0x0011AE4F File Offset: 0x0011904F
	private global::DebateRecord EnemyRecord
	{
		get
		{
			return base.CGet<global::DebateRecord>("EnemyRecord");
		}
	}

	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06002696 RID: 9878 RVA: 0x0011AE5C File Offset: 0x0011905C
	private RectTransform FlagsRoot
	{
		get
		{
			return base.CGet<RectTransform>("Flags");
		}
	}

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06002697 RID: 9879 RVA: 0x0011AE69 File Offset: 0x00119069
	private LifeSkillCombatCardArea2 CardArea
	{
		get
		{
			return base.CGet<LifeSkillCombatCardArea2>("CardArea");
		}
	}

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x06002698 RID: 9880 RVA: 0x0011AE76 File Offset: 0x00119076
	private LifeSkillCombatCardView EnemyUsingCardView
	{
		get
		{
			return base.CGet<LifeSkillCombatCardView>("EnemyUsingCardView");
		}
	}

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x06002699 RID: 9881 RVA: 0x0011AE83 File Offset: 0x00119083
	private EffectPlayer EffectPlayer
	{
		get
		{
			return base.CGet<EffectPlayer>("EffectPlayer");
		}
	}

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x0600269A RID: 9882 RVA: 0x0011AE90 File Offset: 0x00119090
	private PositionFollower _noTargetNotify
	{
		get
		{
			return base.CGet<PositionFollower>("NoTargetNotify");
		}
	}

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x0600269B RID: 9883 RVA: 0x0011AE9D File Offset: 0x0011909D
	private CButtonObsolete BtnForceGiveUp
	{
		get
		{
			return base.CGet<CButtonObsolete>("BtnForceGiveUp");
		}
	}

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x0600269C RID: 9884 RVA: 0x0011AEAA File Offset: 0x001190AA
	private CButtonObsolete BtnGiveUp
	{
		get
		{
			return base.CGet<CButtonObsolete>("BtnGiveUp");
		}
	}

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x0600269D RID: 9885 RVA: 0x0011AEB7 File Offset: 0x001190B7
	private CButtonObsolete BtnTurnEnd
	{
		get
		{
			return base.CGet<CButtonObsolete>("BtnTurnEnd");
		}
	}

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x0600269E RID: 9886 RVA: 0x0011AEC4 File Offset: 0x001190C4
	private CButtonObsolete UnitGradeAreaMask
	{
		get
		{
			return base.CGet<CButtonObsolete>("UnitGradeAreaMask");
		}
	}

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x0600269F RID: 9887 RVA: 0x0011AED1 File Offset: 0x001190D1
	private CButtonObsolete ButtonConfirm
	{
		get
		{
			return base.CGet<CButtonObsolete>("ButtonConfirm");
		}
	}

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x060026A0 RID: 9888 RVA: 0x0011AEDE File Offset: 0x001190DE
	private GameObject SelectCardArea
	{
		get
		{
			return base.CGet<GameObject>("SelectCardArea");
		}
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x060026A1 RID: 9889 RVA: 0x0011AEEB File Offset: 0x001190EB
	private GameObject OperationMask
	{
		get
		{
			return base.CGet<GameObject>("OperationMask");
		}
	}

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x060026A2 RID: 9890 RVA: 0x0011AEF8 File Offset: 0x001190F8
	private RectTransform HighlightRoot
	{
		get
		{
			return base.CGet<RectTransform>("HighlightRoot");
		}
	}

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x060026A3 RID: 9891 RVA: 0x0011AF05 File Offset: 0x00119105
	private GameObject ControlPanel
	{
		get
		{
			return base.CGet<GameObject>("ControlPanel");
		}
	}

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x060026A4 RID: 9892 RVA: 0x0011AF12 File Offset: 0x00119112
	private SkeletonGraphic BtnTurnEndSkeletonGraphic
	{
		get
		{
			return base.CGet<SkeletonGraphic>("BtnTurnEndSkeletonGraphic");
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x060026A5 RID: 9893 RVA: 0x0011AF1F File Offset: 0x0011911F
	private UIParticle BtnTurnEndEffect
	{
		get
		{
			return base.CGet<UIParticle>("BtnTurnEndEffect");
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x060026A6 RID: 9894 RVA: 0x0011AF2C File Offset: 0x0011912C
	private GameObject StateBack
	{
		get
		{
			return base.CGet<GameObject>("StateBack");
		}
	}

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x060026A7 RID: 9895 RVA: 0x0011AF39 File Offset: 0x00119139
	private TextMeshProUGUI StateText
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("StateText");
		}
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x060026A8 RID: 9896 RVA: 0x0011AF46 File Offset: 0x00119146
	private UISwitcher StartCombat
	{
		get
		{
			return base.CGet<UISwitcher>("StartCombat");
		}
	}

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x060026A9 RID: 9897 RVA: 0x0011AF53 File Offset: 0x00119153
	private CButtonObsolete SpeedDown
	{
		get
		{
			return base.CGet<CButtonObsolete>("SpeedDown");
		}
	}

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x060026AA RID: 9898 RVA: 0x0011AF60 File Offset: 0x00119160
	private CButtonObsolete SpeedUp
	{
		get
		{
			return base.CGet<CButtonObsolete>("SpeedUp");
		}
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x060026AB RID: 9899 RVA: 0x0011AF6D File Offset: 0x0011916D
	private CanvasGroup HalfRoundEffect
	{
		get
		{
			return base.CGet<CanvasGroup>("HalfRoundEffect");
		}
	}

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x060026AC RID: 9900 RVA: 0x0011AF7A File Offset: 0x0011917A
	private TooltipInvoker StageTip
	{
		get
		{
			return base.CGet<TooltipInvoker>("StageTip");
		}
	}

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x060026AD RID: 9901 RVA: 0x0011AF87 File Offset: 0x00119187
	private GameObject RemoveWarning
	{
		get
		{
			return base.CGet<GameObject>("RemoveWarning");
		}
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x060026AE RID: 9902 RVA: 0x0011AF94 File Offset: 0x00119194
	private TextMeshProUGUI RemoveWarningText
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("RemoveWarningText");
		}
	}

	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x060026AF RID: 9903 RVA: 0x0011AFA1 File Offset: 0x001191A1
	private CButtonObsolete ButtonResetStrategy
	{
		get
		{
			return base.CGet<CButtonObsolete>("ButtonResetStrategy");
		}
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x0011AFB0 File Offset: 0x001191B0
	public override void OnInit(ArgumentBox argsBox)
	{
		this.StopAutoFight();
		this.Model.RemovingCards.Clear();
		this.Model.IsGameOver = false;
		this.Model.IsPlayingOperation = false;
		this.SelfRecord.Clear();
		this.EnemyRecord.Clear();
		this.SelfRecord.SetIsTaiwu(true);
		this.EnemyRecord.SetIsTaiwu(false);
		this.HideCombatStateInfo();
		this.StartCombat.Switch(true);
		this.SetTimeScale(this.SettingData.DebateSpeed);
		this.ShowOperationMask(true);
		LifeSkillTypeItem config = LifeSkillType.Instance.GetItem(this.Model.LifeSkillType);
		Assert.IsNotNull<LifeSkillTypeItem>(config, string.Format("combat skill type invalid:{0}", this.Model.LifeSkillType));
		base.CGet<TextMeshProUGUI>("LifeSkillTypeName").text = config.Name;
		this._lifeSkillCombatPlayerTaiwu.Init(base.CGet<Refers>("TaiwuCharacter"), true, this.Model.TaiwuCharData);
		this._lifeSkillCombatPlayerEnemy.Init(base.CGet<Refers>("EnemyCharacter"), false, this.Model.EnemyCharData);
		this._lifeSkillCombatPlayerTaiwu.ScoreChanged = (this._lifeSkillCombatPlayerEnemy.ScoreChanged = delegate()
		{
			this._flagsNeedUpdate = true;
		});
		this._lifeSkillCombatGrid.Init(base.CGet<Refers>("Grid"), new Action<Vector2Int, sbyte>(this.CreateUnit));
		this._lifeSkillCombatPlayerTaiwu.Speak(0, 4f);
		this._lifeSkillCombatPlayerEnemy.Speak(0, 4f);
		base.CGet<LifeSkillCombatCardView>("SelfResetCardView").transform.Rotate(new Vector3(0f, 0f, 100f));
		base.CGet<LifeSkillCombatCardView>("EnemyResetCardView").transform.Rotate(new Vector3(0f, 0f, 100f));
		this.InitRoundStage();
		this.UnitGradeAreaMask.gameObject.SetActive(false);
		this.UnitGradeAreaMask.ClearAndAddListener(delegate
		{
			this.HideCreateUnitArea(true);
		});
		this.SelectCardArea.gameObject.SetActive(false);
		this.BtnTurnEndEffect.Stop();
		this.BtnTurnEndEffect.gameObject.SetActive(true);
		this.RemoveWarning.SetActive(false);
		this.HideCreateUnitArea(true);
		LifeSkillCombatCardCameraManager.Instance.Hide();
		LifeSkillCombatUnitCameraManager.Instance.Hide();
		this.UpdateAutoFightMark(false);
		this.ResetFlags();
		this._flagsNeedUpdate = true;
		this.NeedWaitData = true;
		List<int> selfAudienceList = (from d in this.Model.GetAudienceList(true)
		where d != null
		select d.CharacterId).ToList<int>();
		List<int> enemyAudienceList = (from d in this.Model.GetAudienceList(false)
		where d != null
		select d.CharacterId).ToList<int>();
		List<int> allAudienceList = selfAudienceList.Union(enemyAudienceList).ToList<int>();
		TaiwuDomainMethod.AsyncCall.DebateGameInitialize(this, this.Model.LifeSkillType, this.Model.DebateGame.IsTaiwuFirst, this.Model.EnemyCharId, allAudienceList, delegate(int offset, RawDataPool dataPool)
		{
			DebateGame debateGame = null;
			Serializer.Deserialize(dataPool, offset, ref debateGame);
			this.Model.SetDebateGame(debateGame);
			this.RefreshRoundAndStage();
			this.RefreshData(false);
			this.Element.ShowAfterRefresh();
			this.FinishStage();
			this._flagsNeedUpdate = true;
		});
		Refers firstMove = base.CGet<Refers>("FirstMove");
		EPrepareCombatResult result = this.Model.DebateGame.IsTaiwuFirst ? EPrepareCombatResult.SelfFirst : EPrepareCombatResult.EnemyFirst;
		UI_LifeSkillCombatBegin.RefreshFirstMove(firstMove, result, true);
		CombatBeginFirstMove allyFirstMove = firstMove.CGet<CombatBeginFirstMove>("Ally");
		CombatBeginFirstMove enemyFirstMove = firstMove.CGet<CombatBeginFirstMove>("Enemy");
		TooltipInvoker allyTip = allyFirstMove.GetComponent<TooltipInvoker>();
		allyTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		allyTip.Type = (this.Model.DebateGame.IsTaiwuFirst ? TipType.LifeSkillCombatFirstMove : TipType.LifeSkillCombatLastMove);
		TooltipInvoker enemyTip = enemyFirstMove.GetComponent<TooltipInvoker>();
		enemyTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		enemyTip.Type = ((!this.Model.DebateGame.IsTaiwuFirst) ? TipType.LifeSkillCombatFirstMove : TipType.LifeSkillCombatLastMove);
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x0011B40C File Offset: 0x0011960C
	private void Update()
	{
		bool flag = this._flagsNeedUpdate && this.FlagsCanUpdate();
		if (flag)
		{
			this.UpdateFlags();
			this._flagsNeedUpdate = false;
		}
		bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
		if (flag2)
		{
			bool flag3 = this.CardArea.FocusingCardItem != null;
			if (flag3)
			{
				this.CardArea.UnselectCard(true);
				this.RefreshCards();
			}
		}
		else
		{
			bool flag4 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag4)
			{
				bool flag5 = this.Model.IsAuto || this.Model.IsGameOver || this.Model.IsPlayingOperation;
				if (!flag5)
				{
					bool flag6 = !this.BtnTurnEnd.gameObject.activeSelf || !this.BtnTurnEnd.interactable;
					if (!flag6)
					{
						bool flag7 = this.CardArea.FocusingCardItem == null;
						if (flag7)
						{
							this.OnClickBtnTurnEnd();
						}
						else
						{
							this.CardArea.UnselectCard(true);
							this.RefreshCards();
						}
					}
				}
			}
			else
			{
				bool flag8 = CombatCommandKit.SpeedUp.Check(this.Element, false, false, false, false, false) && this.SpeedUp.gameObject.activeSelf && this.SpeedUp.interactable;
				if (flag8)
				{
					this.OnClickSpeedUp();
				}
				else
				{
					bool flag9 = CombatCommandKit.SpeedDown.Check(this.Element, false, false, false, false, false) && this.SpeedDown.gameObject.activeSelf && this.SpeedDown.interactable;
					if (flag9)
					{
						this.OnClickSpeedDown();
					}
				}
			}
		}
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x0011B5CC File Offset: 0x001197CC
	public override void QuickHide()
	{
		bool activeSelf = this.OperationMask.gameObject.activeSelf;
		if (!activeSelf)
		{
			bool activeSelf2 = this.UnitGradeAreaMask.gameObject.activeSelf;
			if (activeSelf2)
			{
				this.HideCreateUnitArea(true);
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
			else
			{
				bool isRemovingCards = this.Model.IsRemovingCards;
				if (isRemovingCards)
				{
					this.ExitSelectRemovingCardMode();
				}
				else
				{
					bool flag = this.CardArea.FocusingCardItem != null;
					if (flag)
					{
						AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
						bool flag2 = this._unitSelectionStack.Count > 0;
						if (flag2)
						{
							this.RemoveRepeatUnitSelection();
							bool flag3 = this.Model.NeedSelectTarget(this.CardArea.FocusingCardItem.CardView.CardConfig);
							if (flag3)
							{
								this.CheckTarget();
							}
						}
						else
						{
							this.CardArea.UnselectCard(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x0011B6C8 File Offset: 0x001198C8
	private void OnEnable()
	{
		AudioManager.Instance.PlayMusic(UI_LifeSkillCombat2.BGM, 1f, 100, null);
		AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		GEvent.Add(UiEvents.OnLifeSkillCombatEnterCardFocusMode, new GEvent.Callback(this.EnterCardFocusMode));
		GEvent.Add(UiEvents.OnLifeSkillCombatExitCardFocusMode, new GEvent.Callback(this.ExitCardFocusMode));
		GEvent.Add(UiEvents.ShowCombatLifeSkillHiddenInfo, new GEvent.Callback(this.ShowCombatLifeSkillHiddenInfo));
		GEvent.Add(UiEvents.ShowCombatLifeSkillTalk, new GEvent.Callback(this.ShowCombatLifeSkillTalk));
		GEvent.Add(UiEvents.CombatLifeSkillClickUnit, new GEvent.Callback(this.CombatLifeSkillClickUnit));
		GEvent.Add(UiEvents.CombatLifeSkillHoverUnit, new GEvent.Callback(this.CombatLifeSkillHoverUnit));
		GEvent.Add(UiEvents.CombatLifeSkillClickBlock, new GEvent.Callback(this.CombatLifeSkillClickBlock));
		GEvent.Add(UiEvents.ChangeLifeSkillCombatData, new GEvent.Callback(this.ChangeLifeSkillCombatData));
		GEvent.Add(UiEvents.CombatLifeSkillHoverStrategy, new GEvent.Callback(this.CombatLifeSkillHoverStrategy));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x0011B81C File Offset: 0x00119A1C
	private void OnDisable()
	{
		this.Model.IsGameOver = true;
		base.transform.DOKill(false);
		this._lifeSkillCombatGrid.Clear();
		this.Model.ResetPauseAndTimeScale();
		this.RefreshHalfRoundEffect(false);
		GEvent.Remove(UiEvents.OnLifeSkillCombatEnterCardFocusMode, new GEvent.Callback(this.EnterCardFocusMode));
		GEvent.Remove(UiEvents.OnLifeSkillCombatExitCardFocusMode, new GEvent.Callback(this.ExitCardFocusMode));
		GEvent.Remove(UiEvents.ShowCombatLifeSkillHiddenInfo, new GEvent.Callback(this.ShowCombatLifeSkillHiddenInfo));
		GEvent.Remove(UiEvents.ShowCombatLifeSkillTalk, new GEvent.Callback(this.ShowCombatLifeSkillTalk));
		GEvent.Remove(UiEvents.CombatLifeSkillClickUnit, new GEvent.Callback(this.CombatLifeSkillClickUnit));
		GEvent.Remove(UiEvents.CombatLifeSkillHoverUnit, new GEvent.Callback(this.CombatLifeSkillHoverUnit));
		GEvent.Remove(UiEvents.CombatLifeSkillClickBlock, new GEvent.Callback(this.CombatLifeSkillClickBlock));
		GEvent.Remove(UiEvents.ChangeLifeSkillCombatData, new GEvent.Callback(this.ChangeLifeSkillCombatData));
		GEvent.Remove(UiEvents.CombatLifeSkillHoverStrategy, new GEvent.Callback(this.CombatLifeSkillHoverStrategy));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		SingletonObject.getInstance<GameSort>().SetCharacterSortConfig("UI_SelectChar", new CharacterSortFilterSetting());
		AudioManager.Instance.StopSound(UI_LifeSkillCombat2.SoundHalfRound);
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x0011B99C File Offset: 0x00119B9C
	private void TopUiChanged(ArgumentBox argumentBox)
	{
		bool needPause = UIElement.SystemOption.IsShowing || UIElement.SystemSetting.IsShowing || GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
		this.Model.SetPause(needPause);
		this.CardArea.OnTopUiChange(UIManager.Instance.IsFocusElement(this.Element));
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x0011B9FC File Offset: 0x00119BFC
	private void InitRoundStage()
	{
		List<GameObject> goList = base.CGetList<GameObject>("RoundStage");
		goList.ForEach(delegate(GameObject go)
		{
			go.gameObject.SetActive(true);
		});
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x0011BA3C File Offset: 0x00119C3C
	private void RefreshRoundAndStage()
	{
		sbyte roundStage = this.Model.DebateGame.State;
		sbyte b = roundStage;
		sbyte b2 = b;
		string content;
		if (b2 - -1 > 2)
		{
			if (b2 != 2)
			{
				throw new ArgumentOutOfRangeException("roundStage", roundStage, null);
			}
			content = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Result);
		}
		else
		{
			bool isTaiwuRound = this.Model.IsTaiwuRound;
			if (isTaiwuRound)
			{
				bool playerCanMakeMove = this.Model.DebateGame.GetPlayerCanMakeMove(true);
				if (playerCanMakeMove)
				{
					content = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_SelfStart);
				}
				else
				{
					content = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_SelfEnd);
				}
			}
			else
			{
				content = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Enemy);
			}
		}
		base.CGet<TextMeshProUGUI>("RoundStageText").text = content;
		List<GameObject> goList = base.CGetList<GameObject>("RoundStage");
		for (int i = 0; i < goList.Count; i++)
		{
			goList[i].gameObject.SetActive((int)roundStage <= i);
		}
		this.RefreshRoundNumber();
		this.RefreshMaxRoundNumber();
		this.RefreshHalfRoundEffect(true);
		this.RefreshStageTip();
	}

	// Token: 0x060026B8 RID: 9912 RVA: 0x0011BB54 File Offset: 0x00119D54
	private void RefreshStageTip()
	{
		string[] presetParam = this.StageTip.PresetParam;
		bool flag = presetParam == null || presetParam.Length != 2;
		if (flag)
		{
			this.StageTip.PresetParam = new string[2];
		}
		this.StageTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage);
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Tip));
		bool isHalfRound = this.Model.IsHalfRound;
		if (isHalfRound)
		{
			int remainRound = DebateConstants.MaxRound - this.Model.DebateGame.Round;
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Stage_GameOver_Tip, remainRound).ColorReplace());
		}
		this.StageTip.PresetParam[1] = stringBuilder.ToString();
		stringBuilder.Clear();
		EasyPool.Free<StringBuilder>(stringBuilder);
	}

	// Token: 0x060026B9 RID: 9913 RVA: 0x0011BC38 File Offset: 0x00119E38
	private void RefreshHalfRoundEffect(bool show)
	{
		bool flag = !show;
		if (flag)
		{
			this.HalfRoundEffect.alpha = 0f;
			this.HalfRoundEffect.gameObject.SetActive(false);
		}
		else
		{
			bool flag2 = !this.HalfRoundEffect.gameObject.activeSelf && this.Model.IsHalfRound;
			if (flag2)
			{
				this.HalfRoundEffect.alpha = 0f;
				this.HalfRoundEffect.gameObject.SetActive(true);
				this.HalfRoundEffect.DOKill(false);
				this.HalfRoundEffect.DOFade(1f, 2f);
				AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundHalfRound, true, true);
			}
		}
	}

	// Token: 0x060026BA RID: 9914 RVA: 0x0011BCF4 File Offset: 0x00119EF4
	private void FinishStage()
	{
		this.BtnTurnEnd.interactable = false;
		bool isGameOver = this.Model.IsGameOver;
		if (!isGameOver)
		{
			this.ShowOperationMask(true);
			TaiwuDomainMethod.AsyncCall.DebateGameNextState(this, delegate(int offset, RawDataPool dataPool)
			{
				DebateGame debateGame = null;
				Serializer.Deserialize(dataPool, offset, ref debateGame);
				this.Model.SetDebateGame(debateGame);
				this.Model.RemovingCards.Clear();
				this._lifeSkillCombatGrid.RefreshAllBlock();
				bool flag = !this.Model.IsTaiwuRound || this.Model.IsAuto;
				if (flag)
				{
					this.RefreshRoundAndStage();
					this.SetButtonInteractable(this.BtnForceGiveUp, this.Model.IsTaiwuRound);
					this.SetButtonInteractable(this.BtnGiveUp, this.Model.IsTaiwuRound);
					this.SetButtonInteractable(this.BtnTurnEnd, this.Model.IsTaiwuRound);
				}
				float duration = (this.Model.IsTaiwuRound && !this.Model.IsAuto) ? 0f : this.PlayStageSwitchAnim();
				this.PlayOperation(duration, delegate
				{
					this.RefreshData(true);
					bool flag2 = !this.Model.IsTaiwuRound;
					if (flag2)
					{
						this.FinishStage();
					}
					else
					{
						bool isAuto = this.Model.IsAuto;
						if (isAuto)
						{
							this.AutoAction();
						}
						else
						{
							float duration2 = this.PlayStageSwitchAnim();
							DOVirtual.DelayedCall(duration2, delegate
							{
								this.RefreshRoundAndStage();
								this.SetButtonInteractable(this.BtnForceGiveUp, this.Model.IsTaiwuRound);
								this.SetButtonInteractable(this.BtnGiveUp, this.Model.IsTaiwuRound);
								this.SetButtonInteractable(this.BtnTurnEnd, this.Model.IsTaiwuRound);
								this.ShowOperationMask(false);
								bool isAuto2 = this.Model.IsAuto;
								if (isAuto2)
								{
									this.AutoAction();
								}
							}, false);
						}
					}
				});
			});
		}
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x0011BD3B File Offset: 0x00119F3B
	private void AutoAction()
	{
		this.BtnTurnEnd.interactable = false;
		this.ShowOperationMask(true);
		TaiwuDomainMethod.AsyncCall.DebateGameSetTaiwuAi(null, true, delegate(int offset, RawDataPool pool)
		{
			DebateGame debateGame = null;
			Serializer.Deserialize(pool, offset, ref debateGame);
			bool flag = !this.Model.DebateGame.IsTaiwuAiProcessedInRound && debateGame.IsTaiwuAiProcessedInRound;
			if (flag)
			{
				this.Model.SetDebateGame(debateGame);
				this.PlayOperation(0f, delegate
				{
					this.BtnTurnEnd.interactable = true;
					this.RefreshData(true);
					bool isAuto2 = this.Model.IsAuto;
					if (isAuto2)
					{
						this.FinishStage();
					}
					else
					{
						this.ShowOperationMask(false);
					}
				});
			}
			else
			{
				this.BtnTurnEnd.interactable = true;
				bool flag2 = !this.Model.IsPlayingOperation;
				if (flag2)
				{
					this.ShowOperationMask(this.Model.IsAuto);
					bool isAuto = this.Model.IsAuto;
					if (isAuto)
					{
						this.FinishStage();
					}
				}
			}
		});
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x0011BD68 File Offset: 0x00119F68
	private void GameOver(bool isTaiwuWin, bool isSurrender = false)
	{
		bool isGameOver = this.Model.IsGameOver;
		if (!isGameOver)
		{
			this.Model.IsGameOver = true;
			this._lifeSkillCombatPlayerTaiwu.Speak(this.GetPlaySpeakConfigOnGameOver(isTaiwuWin), 4f);
			this._lifeSkillCombatPlayerEnemy.Speak(this.GetPlaySpeakConfigOnGameOver(!isTaiwuWin), 4f);
			DOVirtual.DelayedCall(1f, delegate
			{
				TaiwuDomainMethod.AsyncCall.DebateGameOver(this, isTaiwuWin, isSurrender, delegate(int offset, RawDataPool dataPool)
				{
					DebateResult debateResult = null;
					Serializer.Deserialize(dataPool, offset, ref debateResult);
					ArgumentBox box = EasyPool.Get<ArgumentBox>();
					box.SetObject("DebateResult", debateResult);
					UIElement.DebateResult.SetOnInitArgs(box);
					UIManager.Instance.MaskUI(UIElement.DebateResult);
					Time.timeScale = 1f;
				});
			}, false);
		}
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x0011BE04 File Offset: 0x0011A004
	private float PlayStageSwitchAnim()
	{
		sbyte state = this.Model.DebateGame.State;
		bool flag = state == 0 || state == 1;
		float result;
		if (flag)
		{
			bool isTaiwuRound = this.Model.IsTaiwuRound;
			if (isTaiwuRound)
			{
				result = this.ShowTaiwuRound();
			}
			else
			{
				result = this.ShowEnemyRound();
			}
		}
		else
		{
			result = this.ShowResultRound();
		}
		return result;
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x0011BE68 File Offset: 0x0011A068
	private float ShowResultRound()
	{
		this.LogStepInfo("entering result round ...", true);
		base.CGet<GameObject>("TaiwuRound").SetActive(false);
		base.CGet<GameObject>("EnemyRound").SetActive(false);
		base.CGet<TextMeshProUGUI>("RoundLabel").text = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Result).SetColor("enemyround");
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("Round");
		canvasGroup.alpha = 0f;
		canvasGroup.gameObject.SetActive(true);
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.Append(canvasGroup.DOFade(1f, 0.3f));
		sequence.AppendInterval(0.5f);
		sequence.Append(canvasGroup.DOFade(0f, 0.2f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.gameObject.SetActive(false);
		});
		sequence.SetAutoKill(true);
		sequence.Play<DG.Tweening.Sequence>();
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStage3, false, true);
		return sequence.Duration(true);
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x0011BF90 File Offset: 0x0011A190
	private float ShowTaiwuRound()
	{
		this.LogStepInfo("entering taiwu round ...", true);
		base.CGet<GameObject>("TaiwuRound").SetActive(true);
		base.CGet<GameObject>("EnemyRound").SetActive(false);
		base.CGet<TextMeshProUGUI>("RoundLabel").text = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_RoundSelf).SetColor("enemyround");
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("Round");
		canvasGroup.alpha = 0f;
		canvasGroup.gameObject.SetActive(true);
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.Append(canvasGroup.DOFade(1f, 0.3f));
		sequence.AppendInterval(0.5f);
		sequence.Append(canvasGroup.DOFade(0f, 0.2f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.gameObject.SetActive(false);
		});
		sequence.SetAutoKill(true);
		sequence.Play<DG.Tweening.Sequence>();
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStage1, false, true);
		return sequence.Duration(true);
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x0011C0B8 File Offset: 0x0011A2B8
	private float ShowEnemyRound()
	{
		this.LogStepInfo("entering enemy round ...", true);
		base.CGet<GameObject>("TaiwuRound").SetActive(false);
		base.CGet<GameObject>("EnemyRound").SetActive(true);
		base.CGet<TextMeshProUGUI>("RoundLabel").text = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_RoundEnemy).SetColor("taiwuround");
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("Round");
		canvasGroup.alpha = 0f;
		canvasGroup.gameObject.SetActive(true);
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.Append(canvasGroup.DOFade(1f, 0.3f));
		sequence.AppendInterval(0.5f);
		sequence.Append(canvasGroup.DOFade(0f, 0.2f));
		sequence.AppendCallback(delegate
		{
			canvasGroup.gameObject.SetActive(false);
		});
		sequence.SetAutoKill(true);
		sequence.Play<DG.Tweening.Sequence>();
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStage2, false, true);
		return sequence.Duration(true);
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x0011C1E0 File Offset: 0x0011A3E0
	private void RefreshRoundNumber()
	{
		int numberIndex = this.Model.IsHalfRound ? 7 : 5;
		RectTransform layout = base.CGet<RectTransform>("RoundNumberLayout");
		int firstNumber = this.Model.DebateGame.Round / 10;
		int secondNumber = this.Model.DebateGame.Round % 10;
		CImage firstImage = layout.GetChild(0).GetComponent<CImage>();
		firstImage.gameObject.SetActive(firstNumber > 0);
		firstImage.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", numberIndex, firstNumber), false, null);
		CImage secondImage = layout.GetChild(1).GetComponent<CImage>();
		secondImage.gameObject.SetActive(true);
		secondImage.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", numberIndex, secondNumber), false, null);
		base.CGet<CImage>("RoundNumberLine").SetSprite(this.Model.IsHalfRound ? "lifeskillcombat_horizontal" : "lifeskillcombat_horizontal_0", false, null);
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x0011C2DC File Offset: 0x0011A4DC
	private void RefreshMaxRoundNumber()
	{
		int numberIndex = this.Model.IsHalfRound ? 8 : 9;
		int maxRound = DebateConstants.MaxRound;
		RectTransform layout = base.CGet<RectTransform>("MaxRoundNumberLayout");
		int firstNumber = maxRound / 10;
		int secondNumber = maxRound % 10;
		CImage firstImage = layout.GetChild(0).GetComponent<CImage>();
		firstImage.gameObject.SetActive(firstNumber > 0);
		firstImage.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", numberIndex, firstNumber), false, null);
		CImage secondImage = layout.GetChild(1).GetComponent<CImage>();
		secondImage.gameObject.SetActive(true);
		secondImage.SetSprite(string.Format("lifeskillcombat_number_{0}_{1}", numberIndex, secondNumber), false, null);
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x0011C398 File Offset: 0x0011A598
	private void CreateUnit(Vector2Int position, sbyte grade)
	{
		this.HideCreateUnitArea(true);
		this.ShowOperationMask(true);
		IntPair coordinate = new IntPair(position.x, position.y);
		TaiwuDomainMethod.AsyncCall.DebateGameMakeMove(this, coordinate, true, grade, true, delegate(int offset, RawDataPool dataPool)
		{
			DebateGame debateGame = null;
			Serializer.Deserialize(dataPool, offset, ref debateGame);
			this.Model.SetDebateGame(debateGame);
			this._lifeSkillCombatGrid.RefreshAllBlock();
			this.PlayOperation(0f, delegate
			{
				this.RefreshData(true);
				this.RefreshRoundAndStage();
				this.ShowOperationMask(false);
			});
		});
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x0011C3E2 File Offset: 0x0011A5E2
	private void ClickUnit(LifeSkillCombatUnit unit)
	{
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x0011C3E8 File Offset: 0x0011A5E8
	private void CombatLifeSkillClickUnit(ArgumentBox argumentBox)
	{
		LifeSkillCombatUnit unit;
		argumentBox.Get<LifeSkillCombatUnit>("Unit", out unit);
		this.ClickUnit(unit);
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x0011C40C File Offset: 0x0011A60C
	private void CombatLifeSkillHoverUnit(ArgumentBox argumentBox)
	{
		bool flag = !this.Model.IsTaiwuRound || this.Model.IsAuto || this.Model.IsGameOver;
		if (!flag)
		{
			bool flag2 = this.CardArea.FocusingCardItem == null;
			if (!flag2)
			{
				LifeSkillCombatUnit unit;
				argumentBox.Get<LifeSkillCombatUnit>("Unit", out unit);
				bool isEnter;
				argumentBox.Get("IsEnter", out isEnter);
				bool flag3 = !unit.Pawn.IsOwnedByTaiwu;
				if (!flag3)
				{
					bool isRecycleBases = this.CardArea.FocusingCardItem.CardConfig.EffectList.Exists((IntPair e) => e.First == 27);
					bool flag4 = isRecycleBases;
					if (flag4)
					{
						int curEnergy = isEnter ? (this._lifeSkillCombatPlayerTaiwu.Energy + this.Model.DebateGame.GetPawnBases(unit.Pawn.Id, -1, true, true)) : this._lifeSkillCombatPlayerTaiwu.Energy;
						this._lifeSkillCombatPlayerTaiwu.PreviewEnergy(curEnergy, false);
					}
				}
			}
		}
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x0011C528 File Offset: 0x0011A728
	private void CombatLifeSkillHoverStrategy(ArgumentBox argumentBox)
	{
		bool flag = !this.Model.IsTaiwuRound || this.Model.IsAuto || this.Model.IsGameOver;
		if (!flag)
		{
			LifeSkillCombatCardView card = null;
			bool isEnter = true;
			if (argumentBox != null)
			{
				argumentBox.Get<LifeSkillCombatCardView>("Card", out card);
			}
			if (argumentBox != null)
			{
				argumentBox.Get("IsEnter", out isEnter);
			}
			LifeSkillCombatCardItem focusingCardItem = this.CardArea.FocusingCardItem;
			LifeSkillCombatCardView curCard = ((focusingCardItem != null) ? focusingCardItem.CardView : null) ?? card;
			int previewStrategyCount = isEnter ? (this._lifeSkillCombatPlayerTaiwu.StrategyCount - (int)curCard.CardConfig.UsedCost) : this._lifeSkillCombatPlayerTaiwu.StrategyCount;
			LifeSkillCombatCardItem focusingCardItem2 = this.CardArea.FocusingCardItem;
			bool flag2;
			if (focusingCardItem2 == null)
			{
				flag2 = false;
			}
			else
			{
				flag2 = focusingCardItem2.CardConfig.EffectList.Exists((IntPair e) => e.First == 30);
			}
			bool isRecycleStrategy = flag2;
			bool flag3 = isRecycleStrategy;
			if (flag3)
			{
				foreach (StrategyTarget strategyTarget in this._selectedStrategyTargetList)
				{
					foreach (ulong num in strategyTarget.List)
					{
						int index = (int)num;
						LifeSkillCombatCardItem cardItem = this.CardArea.CardList[index];
						previewStrategyCount += (int)cardItem.CardConfig.UsedCost;
					}
				}
			}
			this._lifeSkillCombatPlayerTaiwu.PreviewStrategyCount(previewStrategyCount);
		}
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x0011C6E0 File Offset: 0x0011A8E0
	private void ClickBlock(LifeSkillCombatBlock block)
	{
		this.ShowCreateUnitArea(block, null);
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x0011C6EC File Offset: 0x0011A8EC
	private void CombatLifeSkillClickBlock(ArgumentBox argumentBox)
	{
		LifeSkillCombatBlock block;
		argumentBox.Get<LifeSkillCombatBlock>("Block", out block);
		this.ClickBlock(block);
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x0011C710 File Offset: 0x0011A910
	private void ChangeLifeSkillCombatData(ArgumentBox argumentBox)
	{
		this.ShowOperationMask(true);
		DebateGame debateGame;
		argumentBox.Get<DebateGame>("DebateGame", out debateGame);
		this.Model.SetDebateGame(debateGame);
		this.PlayOperation(0f, delegate
		{
			this.RefreshData(true);
			this.ShowOperationMask(false);
		});
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x0011C75C File Offset: 0x0011A95C
	private void PlayOperation(float delay = 0f, Action onPlayEnd = null)
	{
		UI_LifeSkillCombat2.<>c__DisplayClass135_0 CS$<>8__locals1 = new UI_LifeSkillCombat2.<>c__DisplayClass135_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.onPlayEnd = onPlayEnd;
		bool flag = this.Model.DebateGame.IsGameOver && this.Model.DebateGame.Round >= DebateConstants.MaxRound;
		if (flag)
		{
			this.GameOver(this.Model.DebateGame.IsTaiwuWin, false);
		}
		else
		{
			bool flag2 = this.Model.DebateGame.DebateOperations == null;
			if (flag2)
			{
				this.Model.IsPlayingOperation = false;
				Action onPlayEnd2 = CS$<>8__locals1.onPlayEnd;
				if (onPlayEnd2 != null)
				{
					onPlayEnd2();
				}
			}
			else
			{
				this.Model.IsPlayingOperation = true;
				CS$<>8__locals1.sequence = DOTween.Sequence().SetTarget(base.transform);
				CS$<>8__locals1.sequence.AppendInterval(delay);
				for (int index = 0; index < this.Model.DebateGame.DebateOperations.Count; index++)
				{
					UI_LifeSkillCombat2.<>c__DisplayClass135_1 CS$<>8__locals2 = new UI_LifeSkillCombat2.<>c__DisplayClass135_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.operation = this.Model.DebateGame.DebateOperations[index];
					float animTime = 0.2f;
					UI_LifeSkillCombat2.<>c__DisplayClass135_2 CS$<>8__locals3 = new UI_LifeSkillCombat2.<>c__DisplayClass135_2();
					CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
					switch (CS$<>8__locals3.CS$<>8__locals2.operation.OperationType)
					{
					case 0:
					{
						animTime = 1f;
						CS$<>8__locals3.selfPawn = this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
						CS$<>8__locals3.enemyPawn = this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.NpcPawnId];
						CS$<>8__locals3.selfUnit = this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.selfPawn.Id);
						CS$<>8__locals3.enemyUnit = this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.enemyPawn.Id);
						bool hasUnrevealedObject = CS$<>8__locals3.enemyUnit.HasUnrevealedObject();
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.ConflictUnit(CS$<>8__locals3.selfPawn, CS$<>8__locals3.enemyPawn, CS$<>8__locals3.CS$<>8__locals2.operation.Result, CS$<>8__locals3.CS$<>8__locals2.operation.Target);
						});
						float conflictEffectDelay = 0.25f;
						bool flag3 = hasUnrevealedObject;
						if (flag3)
						{
							conflictEffectDelay += 0.5f;
						}
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(conflictEffectDelay);
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							switch (CS$<>8__locals3.CS$<>8__locals2.operation.Result)
							{
							case 0:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.EffectPlayer.PlayEffectAt(CS$<>8__locals3.enemyUnit.ConflictEffectRoot, "eff_lifeskillcombat_ui_pen_lanqiang", 1f, false);
								break;
							case 1:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.EffectPlayer.PlayEffectAt(CS$<>8__locals3.selfUnit.ConflictEffectRoot, "eff_lifeskillcombat_ui_pen_hongqiang", 1f, true);
								break;
							case 2:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.EffectPlayer.PlayEffectAt(CS$<>8__locals3.selfUnit.ConflictDrawEffectRoot, "eff_lifeskillcombat_ui_pen_yiyangqiang", 1f, false);
								break;
							}
						});
						bool flag4 = CS$<>8__locals3.CS$<>8__locals2.operation.Result != 2;
						if (flag4)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.5f);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								bool isTaiwuWin = CS$<>8__locals3.CS$<>8__locals2.operation.Result == 0;
								LifeSkillCombatUnit loseUnit = isTaiwuWin ? CS$<>8__locals3.enemyUnit : CS$<>8__locals3.selfUnit;
								string destroyEffectName = isTaiwuWin ? "eff_lifeskillcombat_ui_pen_posui2" : "eff_lifeskillcombat_ui_pen_posui1";
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.EffectPlayer.PlayEffectAt(loseUnit.DestroyEffectRoot, destroyEffectName, 1f, !isTaiwuWin);
								DOVirtual.DelayedCall(0.2f, delegate
								{
									loseUnit.RectTrans.localScale = Vector3.zero;
								}, false);
								bool flag23 = isTaiwuWin;
								if (flag23)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.Speak(13, 4f);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.Speak(14, 4f);
								}
								else
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.Speak(14, 4f);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.Speak(13, 4f);
								}
							});
						}
						break;
					}
					case 1:
					{
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.MoveUnit(pawn, new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second));
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
						bool flag5 = CS$<>8__locals3.CS$<>8__locals2.operation.Result > 0;
						if (flag5)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								LifeSkillCombatUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.PlayCreateUnitAnim(true, unit.RectTrans.position, unit.Pawn.IsOwnedByTaiwu);
							});
						}
						break;
					}
					case 2:
					{
						bool isTaiwu = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu;
						if (isTaiwu)
						{
							this.SetButtonInteractable(this.BtnForceGiveUp, false);
						}
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Vector2Int position = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
							bool isFailed2 = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed;
							if (isFailed2)
							{
								LifeSkillCombatBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindBlock(position);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.PlayCreateUnitAnim(true, block.RectTrans.position, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
								DOVirtual.DelayedCall(0.2f, delegate
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayMakeMoveFailedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu, block.RectTrans.position);
								}, false);
							}
							else
							{
								Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.CreateUnit(position, pawn, CS$<>8__locals3.CS$<>8__locals2.operation.Result);
								LifeSkillCombatPlayer player = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu : CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy;
								player.Speak(12, 4f);
							}
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
						bool isFailed = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed;
						if (isFailed)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1f);
						}
						break;
					}
					case 3:
					{
						int result = CS$<>8__locals3.CS$<>8__locals2.operation.Result;
						bool flag6 = result == 2 || result == 3;
						if (flag6)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStrategyDeleteUnit, false, true);
								string effectName = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? "EffectStrategyDeleteTaiwuPawn" : "EffectStrategyDeleteEnemyPawn";
								LifeSkillCombatUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.EffectPlayer.PlayEffectAt(unit.RectTrans, effectName, 0.5f, false);
							});
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.1f);
						}
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.DeleteUnit(pawn);
						});
						break;
					}
					case 4:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayReducePointAnimOnArrive(pawn, CS$<>8__locals3.CS$<>8__locals2.operation);
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1.5f);
						break;
					case 5:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							string targetCharName = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.PlayerName : CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.PlayerName;
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.AddComment(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu, targetCharName);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.AddComment(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu, targetCharName);
						});
						break;
					case 6:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundAddUnitStrategy, false, true);
							Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
							LifeSkillCombatUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
							int strategyId = pawn.Strategies[CS$<>8__locals3.CS$<>8__locals2.operation.Result];
							ActivatedStrategy strategy = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.ActivatedStrategies[strategyId];
							unit.PlayAddStrategyAnim(CS$<>8__locals3.CS$<>8__locals2.operation.Result, strategy.IsCastedByTaiwu, strategy.IsCastedByTaiwu);
							bool isRevealed = strategy.IsRevealed;
							if (isRevealed)
							{
								unit.SetStrategyRevealed();
								unit.PlayRevealSound();
							}
						});
						break;
					case 8:
					{
						int num = index;
						IEnumerable<DebateOperation> source = from o in this.Model.DebateGame.DebateOperations
						where o.OperationType == 8
						select o;
						Func<DebateOperation, int> selector;
						if ((selector = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__14) == null)
						{
							selector = (CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__14 = ((DebateOperation o) => CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.DebateOperations.IndexOf(o)));
						}
						bool isLastCard = num == source.Max(selector);
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayCardChange(CS$<>8__locals3.CS$<>8__locals2.operation);
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(isLastCard ? (0.5f + animTime) : 0.25f);
						break;
					}
					case 9:
					{
						bool needPlayTaiwuStressReduceScoreAnim = this._lifeSkillCombatPlayerTaiwu.Stress < CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuPressure && this._lifeSkillCombatPlayerTaiwu.Score > CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint;
						bool flag7 = needPlayTaiwuStressReduceScoreAnim;
						if (flag7)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.SetStress(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuPressure, true);
							});
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.2f);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.PlayStressReduceScoreAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuPressure);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.SetScore(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint);
							});
						}
						bool needPlayEnemyStressReduceScoreAnim = this._lifeSkillCombatPlayerEnemy.Stress < CS$<>8__locals3.CS$<>8__locals2.operation.NpcPressure && this._lifeSkillCombatPlayerEnemy.Score > CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint;
						bool flag8 = needPlayEnemyStressReduceScoreAnim;
						if (flag8)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.SetStress(CS$<>8__locals3.CS$<>8__locals2.operation.NpcPressure, true);
							});
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.2f);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.PlayStressReduceScoreAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.NpcPressure);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.SetScore(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint);
							});
						}
						break;
					}
					case 10:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
							LifeSkillCombatUnit findUnit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit(pawn.Id);
							if (findUnit != null)
							{
								findUnit.RefreshPower(CS$<>8__locals3.CS$<>8__locals2.operation.Result, false);
							}
						});
						break;
					case 11:
					{
						bool hasExtraTriggeredAnim = LifeSkillCombatBlock.HasExtraTriggeredAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId);
						bool flag9 = hasExtraTriggeredAnim;
						if (flag9)
						{
							DebateNodeEffectItem effectConfig = DebateNodeEffect.Instance[CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId];
							int reduceScoreIndex = effectConfig.TriggerEffectList.FindIndex((IntPair e) => e.First == 43);
							bool needReduceScore = reduceScoreIndex >= 0;
							bool flag10 = needReduceScore;
							if (flag10)
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									LifeSkillCombatPlayer player = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu : CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy;
									int removeScore = effectConfig.TriggerEffectList[reduceScoreIndex].Second;
									for (int i = 0; i < removeScore; i++)
									{
										int oldScore = player.Score - i;
										RectTransform oldScoreTrans = player.GetScoreRectTrans(oldScore);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayBlockEffectTriggeredExtraAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, oldScoreTrans);
									}
								});
							}
						}
						else
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								bool flag23 = CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint > CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.Score;
								if (flag23)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.PlayScoreAddedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
								}
								else
								{
									bool flag24 = CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint > CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.Score;
									if (flag24)
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.PlayScoreAddedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
									}
									else
									{
										bool flag25 = CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint < CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.Score;
										if (flag25)
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.PlayScoreReducedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
										}
										else
										{
											bool flag26 = CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint < CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.Score;
											if (flag26)
											{
												CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.PlayScoreReducedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
											}
										}
									}
								}
							});
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
						}
						break;
					}
					case 12:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							LifeSkillCombatUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
							unit.PlayProtectEffect();
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1f);
						break;
					case 13:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							LifeSkillCombatUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
							if (unit != null)
							{
								unit.PlayRemoveStrategyAnim(CS$<>8__locals3.CS$<>8__locals2.operation.Result, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
							}
						});
						break;
					case 14:
					{
						UI_LifeSkillCombat2.<>c__DisplayClass135_2 CS$<>8__locals5 = CS$<>8__locals3;
						List<StrategyTarget> strategyTargets = CS$<>8__locals3.CS$<>8__locals2.operation.StrategyTargets;
						List<LifeSkillCombatUnit> targetUnitList;
						if (strategyTargets == null)
						{
							targetUnitList = null;
						}
						else
						{
							IEnumerable<ulong> source2 = (from s in strategyTargets
							where s.Type == EDebateStrategyTargetObjectType.Pawn
							select s).SelectMany((StrategyTarget s) => s.List);
							Func<ulong, LifeSkillCombatUnit> selector2;
							if ((selector2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__26) == null)
							{
								selector2 = (CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__26 = ((ulong id) => CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit((int)id)));
							}
							targetUnitList = source2.Select(selector2).ToList<LifeSkillCombatUnit>();
						}
						CS$<>8__locals5.targetUnitList = targetUnitList;
						List<LifeSkillCombatUnit> targetUnitList2 = CS$<>8__locals3.targetUnitList;
						if (targetUnitList2 != null)
						{
							targetUnitList2.RemoveAll((LifeSkillCombatUnit u) => u == null);
						}
						float useCardTargetPawnAnimDuration = 0f;
						bool needPlayUseCardTargetPawnAnim = !CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed && this.NeedPlayUseCardTargetPawnAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, CS$<>8__locals3.targetUnitList, out useCardTargetPawnAnimDuration);
						bool flag11 = !CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu;
						if (flag11)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayEnemyUseCardAnim(CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed, CS$<>8__locals3.CS$<>8__locals2.operation, CS$<>8__locals3.targetUnitList);
							});
							float interval = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed ? 1f : 0.7f;
							bool flag12 = needPlayUseCardTargetPawnAnim;
							if (flag12)
							{
								interval += useCardTargetPawnAnimDuration;
							}
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(interval);
						}
						else
						{
							this.SetButtonInteractable(this.BtnForceGiveUp, false);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
							{
								List<LifeSkillCombatCardItem> cardList = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.CardArea.CardList;
								Predicate<LifeSkillCombatCardItem> match;
								if ((match = CS$<>8__locals3.CS$<>8__locals2.<>9__38) == null)
								{
									match = (CS$<>8__locals3.CS$<>8__locals2.<>9__38 = ((LifeSkillCombatCardItem c) => c.Visible && c.CardView.CardConfig.TemplateId == CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId));
								}
								LifeSkillCombatCardItem cardItem = cardList.Find(match);
								bool flag23 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.CardArea.FocusingCardItem == null;
								if (flag23)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.CardArea.SelectCard(cardItem);
								}
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayTaiwuUseCardAnim(CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed, CS$<>8__locals3.CS$<>8__locals2.operation, CS$<>8__locals3.targetUnitList);
							});
							float interval2 = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed ? 1f : 0.7f;
							bool flag13 = needPlayUseCardTargetPawnAnim;
							if (flag13)
							{
								interval2 += useCardTargetPawnAnimDuration;
							}
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(interval2);
						}
						break;
					}
					case 15:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Vector2Int pos = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
							LifeSkillCombatBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindBlock(pos);
							block.PlayEffectAddedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState);
							CharacterDisplayData charData = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.GetAudienceData(CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.CasterId);
							DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.TemplateId];
							bool useTaiwuColor = (CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.TemplateId == 3) ? block.IsSelf : CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.IsHelpTaiwu;
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.AudienceSpeak(charData.CharacterId, nodeEffectConfig.BubbleContent, useTaiwuColor);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.AudienceSpeak(charData.CharacterId, nodeEffectConfig.BubbleContent, useTaiwuColor);
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
						break;
					case 16:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Vector2Int pos = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
							LifeSkillCombatBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindBlock(pos);
							bool isLast2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.HasOnlyOneBlockEffect((short)CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.TemplateId);
							block.PlayEffectRemovedAnim(isLast2);
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
						break;
					case 19:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							Vector2Int pos = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
							LifeSkillCombatBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindBlock(pos);
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayBlockEffectTriggeredAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState, block.RectTrans, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
						});
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
						break;
					case 20:
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddRecord(CS$<>8__locals3.CS$<>8__locals2.operation);
						});
						animTime = 0f;
						break;
					case 21:
					{
						bool hasExtraTriggeredAnim2 = LifeSkillCombatBlock.HasExtraTriggeredAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId);
						bool flag14 = hasExtraTriggeredAnim2;
						if (flag14)
						{
							DebateNodeEffectItem effectConfig2 = DebateNodeEffect.Instance[CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId];
							bool needAddEnergy = effectConfig2.TriggerEffectList.Exists((IntPair e) => e.First == 9);
							bool flag15 = needAddEnergy;
							if (flag15)
							{
								LifeSkillCombatPlayer player = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? this._lifeSkillCombatPlayerTaiwu : this._lifeSkillCombatPlayerEnemy;
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayBlockEffectTriggeredExtraAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, player.EnergySliderTrans);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.SetEnergy(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuBases, true);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.SetEnergy(CS$<>8__locals3.CS$<>8__locals2.operation.NpcBases, true);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
							}
						}
						break;
					}
					case 22:
					{
						List<short> cardIdList = CS$<>8__locals3.CS$<>8__locals2.operation.CardIdList;
						bool flag16 = cardIdList != null && cardIdList.Count > 0;
						if (flag16)
						{
							LifeSkillCombatCardView cardView = base.CGet<LifeSkillCombatCardView>(CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? "SelfResetCardView" : "EnemyResetCardView");
							float duration = 0.1f;
							using (List<short>.Enumerator enumerator = CS$<>8__locals3.CS$<>8__locals2.operation.CardIdList.GetEnumerator())
							{
								TweenCallback <>9__43;
								while (enumerator.MoveNext())
								{
									short cardId = enumerator.Current;
									DG.Tweening.Sequence sequence = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence;
									TweenerCore<Quaternion, Vector3, QuaternionOptions> t = cardView.transform.DOLocalRotate(new Vector3(0f, 0f, 12f), duration, DG.Tweening.RotateMode.Fast).From(new Vector3(0f, 0f, -12f), true, false).OnStart(delegate
									{
										cardView.SetData(cardId, 0);
										cardView.SetEnabled(true, false);
										cardView.SetInteractable(false);
										cardView.ShowCover(!CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
										AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundCardFly, false, false);
									});
									TweenCallback action;
									if ((action = <>9__43) == null)
									{
										action = (<>9__43 = delegate()
										{
											cardView.transform.Rotate(new Vector3(0f, 0f, 100f));
										});
									}
									sequence.Append(t.OnComplete(action));
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(duration);
								}
							}
						}
						break;
					}
					}
					bool flag17 = CS$<>8__locals2.operation.OperationType == 20;
					if (!flag17)
					{
						bool flag18 = CS$<>8__locals2.operation.OperationType == 8;
						if (!flag18)
						{
							sbyte operationType = CS$<>8__locals2.operation.OperationType;
							bool flag19 = operationType == 15 || operationType == 16 || operationType == 19 || operationType == 21;
							if (!flag19)
							{
								operationType = CS$<>8__locals2.operation.OperationType;
								bool flag20 = operationType == 6 || operationType == 13;
								if (flag20)
								{
									int lastIndex = index - 1;
									DebateOperation lastOperation = this.Model.DebateGame.DebateOperations.CheckIndex(lastIndex) ? this.Model.DebateGame.DebateOperations[lastIndex] : null;
									if (lastOperation == null)
									{
										goto IL_10FB;
									}
									operationType = lastOperation.OperationType;
									if (operationType != 6 && operationType != 13)
									{
										goto IL_10FB;
									}
									bool flag21 = lastOperation.OperationType != CS$<>8__locals2.operation.OperationType;
									IL_10FC:
									bool changeType = flag21;
									int num2 = index;
									IEnumerable<DebateOperation> source3 = this.Model.DebateGame.DebateOperations.Where(delegate(DebateOperation o)
									{
										sbyte operationType2 = o.OperationType;
										return operationType2 == 6 || operationType2 == 13;
									});
									Func<DebateOperation, int> selector3;
									if ((selector3 = CS$<>8__locals2.CS$<>8__locals1.<>9__45) == null)
									{
										selector3 = (CS$<>8__locals2.CS$<>8__locals1.<>9__45 = ((DebateOperation o) => CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.DebateOperations.IndexOf(o)));
									}
									bool isLast = num2 == source3.Max(selector3);
									bool flag22 = changeType || isLast;
									if (flag22)
									{
										CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.3f);
									}
									goto IL_11C9;
									IL_10FB:
									flag21 = false;
									goto IL_10FC;
								}
								CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.SetEnergy(CS$<>8__locals2.operation.TaiwuBases, true);
									CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.SetStress(CS$<>8__locals2.operation.TaiwuPressure, true);
									CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerTaiwu.SetScore(CS$<>8__locals2.operation.TaiwuGamePoint);
									CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.SetEnergy(CS$<>8__locals2.operation.NpcBases, true);
									CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.SetStress(CS$<>8__locals2.operation.NpcPressure, true);
									CS$<>8__locals2.CS$<>8__locals1.<>4__this._lifeSkillCombatPlayerEnemy.SetScore(CS$<>8__locals2.operation.NpcGamePoint);
									bool flag23 = CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.IsGameOver && (CS$<>8__locals2.operation.TaiwuGamePoint <= 0 || CS$<>8__locals2.operation.NpcGamePoint <= 0);
									if (flag23)
									{
										CS$<>8__locals2.CS$<>8__locals1.<>4__this.GameOver(CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.IsTaiwuWin, false);
										CS$<>8__locals2.CS$<>8__locals1.sequence.Pause<DG.Tweening.Sequence>();
									}
								});
							}
						}
					}
					IL_11C9:;
				}
				CS$<>8__locals1.sequence.AppendCallback(delegate
				{
					CS$<>8__locals1.<>4__this.Model.IsPlayingOperation = false;
					Action onPlayEnd3 = CS$<>8__locals1.onPlayEnd;
					if (onPlayEnd3 != null)
					{
						onPlayEnd3();
					}
				});
				CS$<>8__locals1.sequence.Play<DG.Tweening.Sequence>();
			}
		}
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x0011D998 File Offset: 0x0011BB98
	public void PlayBlockEffectTriggeredAnim(DebateNodeEffectState nodeEffectState, Transform target, bool isTaiwu)
	{
		DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[nodeEffectState.TemplateId];
		AudioManager.Instance.PlaySound(nodeEffectConfig.TriggerSound, false, true);
		int index = nodeEffectState.TemplateId + 1;
		string triggeredAnimName = LifeSkillCombatBlock.HasExtraTriggeredAnim((short)nodeEffectState.TemplateId) ? string.Format("eff_lifeskillcombat_5dalei_{0}xs1", index) : string.Format("eff_lifeskillcombat_5dalei_{0}xs", index);
		bool isAddEnergy = nodeEffectState.TemplateId == 1;
		bool flag = isAddEnergy;
		if (flag)
		{
			GameObject effectObj = this.EffectPlayer.PlayEffectAt(target, triggeredAnimName, 1f, false);
			effectObj.transform.DOKill(false);
			LifeSkillCombatPlayer player = isTaiwu ? this._lifeSkillCombatPlayerTaiwu : this._lifeSkillCombatPlayerEnemy;
			effectObj.transform.DOMove(player.EnergySliderTrans.position, 0.5f, false);
		}
		else
		{
			this.EffectPlayer.PlayEffectAt(target, triggeredAnimName, 1f, false);
		}
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x0011DA88 File Offset: 0x0011BC88
	public void PlayBlockEffectTriggeredExtraAnim(short nodeEffectTemplateId, Transform target)
	{
		DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[nodeEffectTemplateId];
		AudioManager.Instance.PlaySound(nodeEffectConfig.ExtraTriggerSound, false, true);
		int index = (int)(nodeEffectTemplateId + 1);
		string triggeredExtraAnimName = string.Format("eff_lifeskillcombat_5dalei_{0}xs2", index);
		this.EffectPlayer.PlayEffectAt(target, triggeredExtraAnimName, 1f, false);
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x0011DAE0 File Offset: 0x0011BCE0
	private void AddRecord(DebateOperation operation)
	{
		bool isTaiwu = operation.IsTaiwu;
		if (isTaiwu)
		{
			this.SelfRecord.Add(operation);
		}
		else
		{
			this.EnemyRecord.Add(operation);
		}
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x0011DB14 File Offset: 0x0011BD14
	private float PlayBlockEffectAnim()
	{
		return 0f;
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x0011DB2C File Offset: 0x0011BD2C
	private void PlayCardChange(DebateOperation operation)
	{
		float duration = 0.5f;
		float minScale = 0.2f;
		Vector3 rotation = Vector3.zero.SetZ(90f);
		bool flag = operation.Source == 2;
		if (flag)
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundMoveCard, false, true);
			int num = operation.Destination;
			GameObject controlPoint = base.CGet<GameObject>((num == 0 || num == 1) ? "LeftCardAnimControlPoint" : "RightCardAnimControlPoint");
			Vector3 controlPos = controlPoint.transform.position;
			LifeSkillCombatCardItem cardItem = this.CardArea.CardList[operation.Index];
			RectTransform rectTrans = cardItem.CardView.RectTransform;
			rectTrans.DOKill(true);
			Transform originParent = rectTrans.parent;
			rectTrans.SetParent(this.CardArea.transform);
			Transform destination = this.GetCardGroupTransform(operation.Destination);
			Vector3 sourcePos = rectTrans.position;
			DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
			{
				rectTrans.position = BezierMath.Bezier2(sourcePos, controlPos, destination.position, stepValue);
			}).SetTarget(rectTrans).SetEase(Ease.OutQuart);
			rectTrans.DOScale(minScale, duration);
			rectTrans.DORotate(rotation, duration, DG.Tweening.RotateMode.Fast).OnComplete(delegate
			{
				cardItem.SetVisible(false, true);
				rectTrans.SetParent(originParent);
				rectTrans.localPosition = Vector3.zero;
				rectTrans.localScale = Vector3.one;
				rectTrans.localRotation = Quaternion.identity;
				this.CardArea.LayoutSelfCards();
			});
		}
		else
		{
			bool flag2 = operation.Destination == 2;
			if (flag2)
			{
				AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundMoveCard, false, true);
				int num = operation.Source;
				GameObject controlPoint2 = base.CGet<GameObject>((num == 0 || num == 1) ? "LeftCardAnimControlPoint" : "RightCardAnimControlPoint");
				Vector3 controlPos = controlPoint2.transform.position;
				LifeSkillCombatCardItem cardItem = this.CardArea.CardList[operation.Index];
				RectTransform rectTrans = cardItem.CardView.RectTransform;
				rectTrans.DOKill(true);
				cardItem.SetVisible(true, true);
				cardItem.CardView.SetData(operation.TemplateId, operation.Index);
				cardItem.CardView.SetEnabled(true, false);
				cardItem.CardView.SetInteractable(false);
				cardItem.CardView.SetPointerTrigger(false);
				this.CardArea.LayoutSelfCards();
				Transform originParent = rectTrans.parent;
				Transform source = this.GetCardGroupTransform(operation.Source);
				rectTrans.localScale = Vector3.zero;
				TweenCallback <>9__4;
				DOVirtual.DelayedCall(0.1f, delegate
				{
					Vector3 sourcePos = source.transform.position;
					Vector3 destinationPos = cardItem.RectTransform.position;
					rectTrans.SetParent(this.CardArea.transform);
					rectTrans.position = sourcePos;
					DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
					{
						rectTrans.position = BezierMath.Bezier2(sourcePos, controlPos, destinationPos, stepValue);
					}).SetTarget(rectTrans).SetEase(Ease.OutQuart);
					rectTrans.DOScale(0.8f, duration).From(minScale, true, false);
					TweenerCore<Quaternion, Vector3, QuaternionOptions> t = rectTrans.DORotate(Vector3.zero, duration, DG.Tweening.RotateMode.Fast).From(rotation, true, false);
					TweenCallback action;
					if ((action = <>9__4) == null)
					{
						action = (<>9__4 = delegate()
						{
							rectTrans.SetParent(originParent);
							rectTrans.localPosition = Vector3.zero;
							rectTrans.localScale = Vector3.one;
							rectTrans.localRotation = Quaternion.identity;
						});
					}
					t.OnComplete(action);
				}, false);
			}
		}
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x0011DE88 File Offset: 0x0011C088
	private void PlayTaiwuUseCardAnim(bool usingFailed, DebateOperation operation, List<LifeSkillCombatUnit> targetUnitList)
	{
		if (usingFailed)
		{
			this.CardArea.FocusingCardItem.SetVisible(false, true);
			this.PlayUseCardFailedAnim(true, operation.TemplateId, this.CardArea.FocusingCardItem.CardView.transform.position);
			this.CardArea.UnselectCard(true);
		}
		else
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundUseCard, false, true);
			this.CardArea.FocusingCardItem.CardView.PlayUseEffect();
			this.CardArea.UnselectCard(false);
			this.PlayUseCardTargetPawnAnim(true, operation.TemplateId, targetUnitList);
		}
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x0011DF2C File Offset: 0x0011C12C
	private void PlayEnemyUseCardAnim(bool usingFailed, DebateOperation operation, List<LifeSkillCombatUnit> targetUnitList)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundMoveCard, false, true);
		this.EnemyUsingCardView.SetData(operation.TemplateId, 0);
		this.EnemyUsingCardView.ShowCover(true);
		this.EnemyUsingCardView.SetEnabled(true, false);
		this.EnemyUsingCardView.SetInteractable(false);
		this.EnemyUsingCardView.SetPointerTrigger(false);
		this.EnemyUsingCardView.DOKill(false);
		this.EnemyUsingCardView.gameObject.SetActive(true);
		GameObject enemyUsingCardPoint = base.CGet<GameObject>("EnemyUsingCardPoint");
		this.EnemyUsingCardView.RectTransform.DOMove(enemyUsingCardPoint.transform.position, 0.4f, false).From(this._lifeSkillCombatPlayerEnemy.RectTrans.position, true, false).SetEase(Ease.OutQuart);
		this.EnemyUsingCardView.RectTransform.DOScale(Vector3.one.SetX(-1f), 0.4f).From(Vector3.zero, true, false).SetEase(Ease.OutQuart);
		this.EnemyUsingCardView.RectTransform.DORotate(new Vector3(0f, 0f, 360f), 0.4f, DG.Tweening.RotateMode.FastBeyond360).From(Vector3.zero, true, false);
		TweenCallback <>9__1;
		DOVirtual.DelayedCall(0.4f, delegate
		{
			bool usingFailed2 = usingFailed;
			if (usingFailed2)
			{
				this.EnemyUsingCardView.gameObject.SetActive(false);
				this.PlayUseCardFailedAnim(false, operation.TemplateId, this.EnemyUsingCardView.transform.position);
			}
			else
			{
				this.EnemyUsingCardView.PlayUseEffect();
				AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundUseCard, false, true);
				this.PlayUseCardTargetPawnAnim(false, operation.TemplateId, targetUnitList);
				float delay = 0.7f;
				TweenCallback callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundMoveCard, false, true);
						Transform destination = this.GetCardGroupTransform(4);
						this.EnemyUsingCardView.RectTransform.DOMove(destination.position, 0.4f, false).From(enemyUsingCardPoint.transform.position, true, false).SetEase(Ease.OutQuart);
						this.EnemyUsingCardView.RectTransform.DOScale(Vector3.zero, 0.4f).From(Vector3.one.SetX(-1f), true, false).SetEase(Ease.OutQuart);
						this.EnemyUsingCardView.RectTransform.DORotate(new Vector3(0f, 0f, 720f), 0.4f, DG.Tweening.RotateMode.FastBeyond360).From(new Vector3(0f, 0f, 360f), true, false);
						bool flag = targetUnitList != null;
						if (flag)
						{
							foreach (LifeSkillCombatUnit unit in targetUnitList)
							{
								unit.RefreshStrategy(0, false, false);
							}
						}
					});
				}
				DOVirtual.DelayedCall(delay, callback, false).SetTarget(this.EnemyUsingCardView);
			}
		}, false).SetTarget(this.EnemyUsingCardView);
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x0011E0C4 File Offset: 0x0011C2C4
	private bool NeedPlayUseCardTargetPawnAnim(short strategyTemplateId, List<LifeSkillCombatUnit> targetUnitList, out float duration)
	{
		DebateStrategyItem config = DebateStrategy.Instance[strategyTemplateId];
		bool hasTargetEffect = targetUnitList != null && targetUnitList.Count > 0 && config.MarkType == EDebateStrategyMarkType.Affect;
		duration = (hasTargetEffect ? 0.5f : 0f);
		return hasTargetEffect;
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x0011E110 File Offset: 0x0011C310
	private void PlayUseCardTargetPawnAnim(bool isTaiwuCast, short strategyTemplateId, List<LifeSkillCombatUnit> targetUnitList)
	{
		string effectName = isTaiwuCast ? "eff_lifeskillcombat_ui_dxkp_shan_lan" : "eff_lifeskillcombat_ui_dxkp_shan_hong";
		float duration;
		bool hasTargetEffect = this.NeedPlayUseCardTargetPawnAnim(strategyTemplateId, targetUnitList, out duration);
		bool flag = hasTargetEffect;
		if (flag)
		{
			foreach (LifeSkillCombatUnit unit in targetUnitList)
			{
				this.EffectPlayer.PlayEffectAt(unit.GetTransform(), effectName, duration, false);
			}
		}
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x0011E194 File Offset: 0x0011C394
	private void PlayUseCardFailedAnim(bool isTaiwu, short strategyTemplateId, Vector3 pos)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundUseCardFailed, false, true);
		LifeSkillCombatCardCameraManager.Instance.Show(isTaiwu, strategyTemplateId);
		Refers refers = base.CGet<Refers>("CastStrategyFailedEffect");
		refers.transform.position = pos;
		refers.gameObject.SetActive(true);
		RawImage image = refers.CGet<RawImage>("Image");
		image.texture = LifeSkillCombatCardCameraManager.Instance.Camera.targetTexture;
		DOVirtual.Float(1f, 0.5f, 1f, delegate(float value)
		{
			image.material.SetFloat("_rongjie", value);
		}).OnComplete(delegate
		{
			LifeSkillCombatCardCameraManager.Instance.Hide();
			refers.gameObject.SetActive(false);
		});
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x0011E260 File Offset: 0x0011C460
	private void PlayMakeMoveFailedAnim(bool isTaiwu, Vector3 pos)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundCreateUnitFailed, false, true);
		LifeSkillCombatUnitCameraManager.Instance.Show(isTaiwu, this.Model.LifeSkillType);
		Refers refers = base.CGet<Refers>("MakeMoveFailedEffect");
		refers.transform.position = pos;
		refers.gameObject.SetActive(true);
		RawImage image = refers.CGet<RawImage>("Image");
		image.texture = LifeSkillCombatUnitCameraManager.Instance.Camera.targetTexture;
		DOVirtual.Float(1f, 0.5f, 1f, delegate(float value)
		{
			image.material.SetFloat("_rongjie", value);
		}).OnComplete(delegate
		{
			LifeSkillCombatUnitCameraManager.Instance.Hide();
			refers.gameObject.SetActive(false);
		});
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x0011E338 File Offset: 0x0011C538
	private void PlayReducePointAnimOnArrive(Pawn pawn, DebateOperation operation)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundUnitArrive, false, true);
		LifeSkillCombatUnit findUnit = this._lifeSkillCombatGrid.FindUnit(pawn.Id);
		findUnit.RectTrans.localScale = Vector3.zero;
		bool isReduceTaiwu = !pawn.IsOwnedByTaiwu;
		string arriveEffectName = isReduceTaiwu ? "eff_lifeskillcombat_ui_dx_hualizi2" : "eff_lifeskillcombat_ui_dx_hualizi";
		float arriveEffectDuration = 0.5f;
		float flyDuration = 0.25f;
		this.EffectPlayer.PlayEffectAt(findUnit.RectTrans, arriveEffectName, arriveEffectDuration, false);
		DOVirtual.DelayedCall(arriveEffectDuration, delegate
		{
			LifeSkillCombatPlayer player = isReduceTaiwu ? this._lifeSkillCombatPlayerTaiwu : this._lifeSkillCombatPlayerEnemy;
			int curScore = isReduceTaiwu ? operation.TaiwuGamePoint : operation.NpcGamePoint;
			curScore = Mathf.Max(0, curScore);
			int lastScore = player.Score;
			TweenCallback <>9__1;
			for (int i = lastScore; i > curScore; i--)
			{
				string flyEffectName = isReduceTaiwu ? string.Format("eff_lifeskillcombat_ui_defentuowei_hong{0}_{1}", pawn.Coordinate.Second + 1, 7 - i) : string.Format("eff_lifeskillcombat_ui_defentuowei_lan{0}_{1}", pawn.Coordinate.Second + 1, 7 - i);
				this.EffectPlayer.PlayEffectAt(findUnit.RectTrans, flyEffectName, flyDuration, isReduceTaiwu);
				float delay = flyDuration * 0.6f;
				TweenCallback callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						player.PlayScoreReducedAnim(curScore, pawn.IsOwnedByTaiwu);
					});
				}
				DOVirtual.DelayedCall(delay, callback, false);
			}
		}, false);
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x0011E410 File Offset: 0x0011C610
	private bool FlagsCanUpdate()
	{
		foreach (Spine.AnimationState flagState in from ske in this.FlagsRoot.GetComponentsInChildren<SkeletonGraphic>()
		select ske.AnimationState)
		{
			TrackEntry currentTrack = flagState.GetCurrent(0);
			bool flag = currentTrack != null && !currentTrack.IsComplete && !currentTrack.Loop;
			if (flag)
			{
				return false;
			}
		}
		return this.Model.DebateGame.State != 2 && !this.Model.IsPlayingOperation;
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x0011E4D8 File Offset: 0x0011C6D8
	private void ResetFlags()
	{
		SkeletonGraphic[] skeletonGraphics = this.FlagsRoot.GetComponentsInChildren<SkeletonGraphic>();
		foreach (SkeletonGraphic skeletonGraphic in skeletonGraphics)
		{
			Spine.AnimationState flagState = skeletonGraphic.AnimationState;
			flagState.SetAnimation(0, "grey", true);
		}
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x0011E520 File Offset: 0x0011C720
	private void UpdateFlags()
	{
		UI_LifeSkillCombat2.<>c__DisplayClass150_0 CS$<>8__locals1 = new UI_LifeSkillCombat2.<>c__DisplayClass150_0();
		int diff = this._lifeSkillCombatPlayerTaiwu.Score - this._lifeSkillCombatPlayerEnemy.Score;
		CS$<>8__locals1.target = ((diff == 0) ? "grey" : ((diff > 0) ? "blue" : "red"));
		CS$<>8__locals1.audioManager = AudioManager.Instance;
		SkeletonGraphic[] skeletonGraphics = this.FlagsRoot.GetComponentsInChildren<SkeletonGraphic>();
		SkeletonGraphic[] array = skeletonGraphics;
		int i = 0;
		while (i < array.Length)
		{
			SkeletonGraphic skeletonGraphic = array[i];
			UI_LifeSkillCombat2.<>c__DisplayClass150_1 CS$<>8__locals2 = new UI_LifeSkillCombat2.<>c__DisplayClass150_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals2.flagState = skeletonGraphic.AnimationState;
			TrackEntry currentTrack = CS$<>8__locals2.flagState.GetCurrent(0);
			string current = (currentTrack != null) ? currentTrack.Animation.Name : null;
			bool flag = !string.IsNullOrEmpty(current);
			if (flag)
			{
				bool flag2 = current == CS$<>8__locals2.CS$<>8__locals1.target;
				if (!flag2)
				{
					int plastic = current.IndexOf('_');
					bool flag3 = plastic < 0;
					if (flag3)
					{
						TrackEntry track = CS$<>8__locals2.flagState.SetAnimation(0, current + "_" + CS$<>8__locals2.CS$<>8__locals1.target, false);
						track.MixDuration = 0f;
						track.Complete += delegate(TrackEntry _)
						{
							CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals2.flagState);
						};
						CS$<>8__locals2.CS$<>8__locals1.audioManager.PlaySound(UI_LifeSkillCombat2.SoundChangeFlag, false, true);
					}
					else
					{
						UI_LifeSkillCombat2.<>c__DisplayClass150_2 CS$<>8__locals3 = new UI_LifeSkillCombat2.<>c__DisplayClass150_2();
						CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
						UI_LifeSkillCombat2.<>c__DisplayClass150_2 CS$<>8__locals4 = CS$<>8__locals3;
						string text = current;
						int num = plastic + 1;
						CS$<>8__locals4.ending = text.Substring(num, text.Length - num);
						currentTrack.MixDuration = 0f;
						currentTrack.Complete += ((CS$<>8__locals3.ending == CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.target) ? delegate(TrackEntry _)
						{
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals3.CS$<>8__locals2.flagState);
						} : delegate(TrackEntry _)
						{
							TrackEntry track2 = CS$<>8__locals3.CS$<>8__locals2.flagState.SetAnimation(0, CS$<>8__locals3.ending + "_" + CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.target, false);
							track2.MixDuration = 0f;
							TrackEntry trackEntry = track2;
							Spine.AnimationState.TrackEntryDelegate value;
							if ((value = CS$<>8__locals3.CS$<>8__locals2.<>9__4) == null)
							{
								value = (CS$<>8__locals3.CS$<>8__locals2.<>9__4 = delegate(TrackEntry _)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals3.CS$<>8__locals2.flagState);
								});
							}
							trackEntry.Complete += value;
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.audioManager.PlaySound(UI_LifeSkillCombat2.SoundChangeFlag, false, true);
						});
					}
				}
			}
			else
			{
				CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals2.flagState);
			}
			IL_200:
			i++;
			continue;
			goto IL_200;
		}
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x0011E740 File Offset: 0x0011C940
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		string name = btn.name;
		string text = name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1009673262U)
		{
			if (num <= 27060459U)
			{
				if (num != 14325120U)
				{
					if (num == 27060459U)
					{
						if (text == "BtnGiveUp")
						{
							this.GiveUp();
						}
					}
				}
				else if (text == "AutoFight")
				{
					this.OnClickAutoFight();
				}
			}
			else if (num != 272219533U)
			{
				if (num == 1009673262U)
				{
					if (text == "BtnWatch")
					{
						this.WatchEnemyStrategy();
					}
				}
			}
			else if (text == "SpeedUp")
			{
				this.OnClickSpeedUp();
			}
		}
		else if (num <= 2726324140U)
		{
			if (num != 1535019745U)
			{
				if (num == 2726324140U)
				{
					if (text == "SpeedDown")
					{
						this.OnClickSpeedDown();
					}
				}
			}
			else if (text == "ButtonResetStrategy")
			{
				this.OnClickButtonResetStrategy();
			}
		}
		else if (num != 3338201710U)
		{
			if (num == 3566856089U)
			{
				if (text == "BtnTurnEnd")
				{
					this.OnClickBtnTurnEnd();
				}
			}
		}
		else if (text == "BtnForceGiveUp")
		{
			this.ForceGiveUp();
		}
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x0011E8B0 File Offset: 0x0011CAB0
	private void OnClickBtnTurnEnd()
	{
		int count;
		bool flag = this.Model.DebateGame.TryGetPlayerCardRemovingCount(true, out count) && this.Model.RemovingCards.Count != count;
		if (flag)
		{
			this.EnterSelectRemovingCardMode(count);
		}
		else
		{
			Spine.Animation clickAnim = this.BtnTurnEndSkeletonGraphic.SkeletonData.FindAnimation("click");
			this.BtnTurnEndSkeletonGraphic.AnimationState.SetAnimation(0, clickAnim.Name, false);
			this.BtnTurnEndEffect.Play();
			DOVirtual.DelayedCall(clickAnim.Duration, delegate
			{
				this.BtnTurnEndSkeletonGraphic.AnimationState.SetAnimation(0, "idle", false);
				this.BtnTurnEndEffect.Stop();
			}, false);
			this.FinishStage();
		}
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x0011E958 File Offset: 0x0011CB58
	private void GiveUp()
	{
		bool flag = !this.Model.IsTaiwuRound || this.Model.IsGameOver;
		if (!flag)
		{
			string title = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_Tips_SelfGiveUpTitle);
			string content = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_Tips_SelfGiveUpContent);
			CommonUtils.ShowConfirmDialog(title, content, delegate
			{
				this.LogStepInfo("send:RequestCurrentPlayerCommitOperation: self give up", true);
				this.GameOver(false, true);
				this._lifeSkillCombatPlayerTaiwu.Speak(15, 4f);
			}, null, EDialogType.None);
		}
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x0011E9B4 File Offset: 0x0011CBB4
	private void ForceGiveUp()
	{
		bool flag = !this.Model.IsTaiwuRound || this.Model.IsGameOver;
		if (!flag)
		{
			this.SetButtonInteractable(this.BtnForceGiveUp, false);
			this.LogStepInfo("send:RequestCurrentPlayerCommitOperation: force adversary give up", true);
			TaiwuDomainMethod.AsyncCall.DebateGameTryForceWin(this, delegate(int offset, RawDataPool pool)
			{
				bool canForceWin = false;
				Serializer.Deserialize(pool, offset, ref canForceWin);
				bool flag2 = canForceWin;
				if (flag2)
				{
					this.GameOver(true, false);
				}
				else
				{
					this._lifeSkillCombatPlayerEnemy.Speak(17, 4f);
				}
				this._lifeSkillCombatPlayerTaiwu.Speak(16, 4f);
				bool flag3 = !canForceWin;
				if (flag3)
				{
					this.FinishStage();
				}
			});
		}
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x0011EA14 File Offset: 0x0011CC14
	private void WatchEnemyStrategy()
	{
		bool isShowingEnemyCard = this.CardArea.IsShowingEnemyCard();
		this.CardArea.ShowEnemyCard(!isShowingEnemyCard);
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x0011EA40 File Offset: 0x0011CC40
	private void EnterCardFocusMode(ArgumentBox argumentBox)
	{
		bool isAuto = this.Model.IsAuto;
		if (!isAuto)
		{
			this.ButtonConfirm.transform.position = this.ButtonConfirm.transform.position.SetX(this.CardArea.FocusingCardItem.CardView.transform.position.x);
			this.ButtonConfirm.ClearAndAddListener(new Action(this.TaiwuUseCard));
			this.ButtonConfirm.gameObject.SetActive(true);
			bool flag = this.PrepareUseCard();
			if (!flag)
			{
				this.ControlPanel.transform.SetParent(this.CardArea.transform);
				this.CardArea.FocusingCardItem.CardView.transform.SetParent(this.HighlightRoot);
				this.SelectCardArea.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x0011EB2C File Offset: 0x0011CD2C
	private void ExitCardFocusMode(ArgumentBox argumentBox)
	{
		this.ControlPanel.transform.SetParent(base.transform);
		ConchShipCursor.Instance.HideLifeSkillCombatUseStrategyTip();
		bool refreshStrategy;
		bool flag = argumentBox == null || !argumentBox.Get("refreshStrategy", out refreshStrategy);
		if (flag)
		{
			refreshStrategy = true;
		}
		this.HideAllStrategyTarget(refreshStrategy);
		this.HideCreateUnitArea(false);
		bool activeSelf = this.SelectCardArea.activeSelf;
		if (activeSelf)
		{
			this.CardArea.FocusingCardItem.CardView.RectTransform.SetParent(this.CardArea.FocusingCardItem.transform);
			this.CardArea.FocusingCardItem.CardView.RectTransform.anchoredPosition = Vector3.zero;
			this.SelectCardArea.gameObject.SetActive(false);
		}
		this._lifeSkillCombatPlayerTaiwu.PreviewEnergy(this._lifeSkillCombatPlayerTaiwu.Energy, false);
		this._lifeSkillCombatPlayerTaiwu.PreviewStrategyCount(this._lifeSkillCombatPlayerTaiwu.StrategyCount);
		this._highlightCardTargetStack.Clear();
		this._unitSelectionStack.Clear();
		this._selectedStrategyTargetList.Clear();
		this._selectedSelectableTargetList.Clear();
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x0011EC5C File Offset: 0x0011CE5C
	private void HideAllStrategyTarget(bool refreshStrategy = true)
	{
		while (this._highlightCardTargetStack.Count > 0)
		{
			ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action> info = this._highlightCardTargetStack.Pop();
			info.Item1.GetTransform().SetParent(info.Item2);
			info.Item1.GetTransform().SetSiblingIndex(info.Item3);
			info.Item4();
		}
		bool flag = this.CardArea.IsShowingEnemyCard();
		if (flag)
		{
			this.CardArea.ShowEnemyCard(false);
		}
		this.CardArea.SelectedCardHasScale = true;
		this._lifeSkillCombatGrid.RefreshAllUnit(false, refreshStrategy);
		this._lifeSkillCombatGrid.RefreshAllBlock();
		this._lifeSkillCombatGrid.OnCancelHighlightBlock();
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x0011ED14 File Offset: 0x0011CF14
	private bool PrepareUseCard()
	{
		bool needSelectTarget = this.Model.NeedSelectTarget(this.CardArea.FocusingCardItem.CardView.CardConfig);
		bool flag = !needSelectTarget;
		bool result;
		if (flag)
		{
			short strategyTemplateId = this.CardArea.FocusingCardItem.CardView.CardConfig.TemplateId;
			List<StrategyTarget> list;
			bool targetIsMeet = this.Model.DebateGame.TryGetStrategyTarget(strategyTemplateId, true, out list);
			bool costIsMeet = this.Model.CheckCost(this.CardArea.FocusingCardItem.CardView.CardConfig);
			this.ButtonConfirm.interactable = (targetIsMeet && costIsMeet);
			int previewStrategyCount = this._lifeSkillCombatPlayerTaiwu.StrategyCount - (int)this.CardArea.FocusingCardItem.CardView.CardConfig.UsedCost;
			this._lifeSkillCombatPlayerTaiwu.PreviewStrategyCount(previewStrategyCount);
			this.ButtonConfirm.gameObject.SetActive(!this.ButtonConfirm.interactable);
			bool interactable = this.ButtonConfirm.interactable;
			if (interactable)
			{
				this.TaiwuUseCard();
				result = true;
			}
			else
			{
				ConchShipCursor.Instance.HideLifeSkillCombatUseStrategyTip();
				result = false;
			}
		}
		else
		{
			foreach (short[] targetInfo in this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList)
			{
				DebateStrategyTargetItem targetConfig = DebateStrategyTarget.Instance[targetInfo[0]];
				this._selectedStrategyTargetList.Add(new StrategyTarget(targetConfig.ObjectType, new List<ulong>()));
				this._selectedSelectableTargetList.Add(new List<ILifeSkillCombatSelectable>());
			}
			result = this.CheckTarget();
		}
		return result;
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x0011EEE0 File Offset: 0x0011D0E0
	private bool CheckTarget()
	{
		this.CombatLifeSkillHoverStrategy(null);
		short[] lastShowingTargetInfo;
		int curIndex;
		bool isMax;
		bool targetIsMeet = this.CheckTargetIsAllMeet(out lastShowingTargetInfo, out curIndex, out isMax);
		bool costIsMeet = this.Model.CheckCost(this.CardArea.FocusingCardItem.CardView.CardConfig);
		this.ButtonConfirm.interactable = (targetIsMeet && costIsMeet);
		short[] targetInfo = lastShowingTargetInfo;
		short targetTemplateId = targetInfo[0];
		DebateStrategyTargetItem targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
		this.RefreshTargetCount(curIndex);
		bool flag = this.CheckTargetIsAllFixedCount();
		if (flag)
		{
			this.ButtonConfirm.gameObject.SetActive(false);
			bool interactable = this.ButtonConfirm.interactable;
			if (interactable)
			{
				this.TaiwuUseCard();
				return true;
			}
		}
		short strategyTemplateId = this.CardArea.FocusingCardItem.CardView.CardConfig.TemplateId;
		List<StrategyTarget> strategyTargetList;
		this.Model.DebateGame.TryGetStrategyTarget(strategyTemplateId, true, out strategyTargetList);
		StrategyTarget strategyTarget = strategyTargetList.CheckIndex(curIndex) ? strategyTargetList[curIndex] : null;
		List<ulong> list = (strategyTarget != null) ? strategyTarget.List : null;
		bool flag2 = list == null || list.Count <= 0;
		bool result;
		if (flag2)
		{
			this.HideAllStrategyTarget(true);
			this.CardArea.FocusingCardItem.CardView.SetSelected(true);
			result = false;
		}
		else
		{
			bool flag3 = !this.GetNeedMergeTarget() && targetConfig.ObjectType != EDebateStrategyTargetObjectType.PawnGrade;
			if (flag3)
			{
				for (int i = 0; i < this._selectedSelectableTargetList.Count; i++)
				{
					bool flag4 = i < curIndex;
					if (flag4)
					{
						foreach (ILifeSkillCombatSelectable lifeSkillCombatSelectable in this._selectedSelectableTargetList[i])
						{
							lifeSkillCombatSelectable.ShowStrategyTargetMark(false, null);
							lifeSkillCombatSelectable.SetSelected(true);
						}
					}
				}
				using (Stack<ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>>.Enumerator enumerator2 = this._highlightCardTargetStack.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action> valueTuple = enumerator2.Current;
						Predicate<ILifeSkillCombatSelectable> <>9__1;
						bool flag5 = this._selectedSelectableTargetList.Exists(delegate(List<ILifeSkillCombatSelectable> l)
						{
							Predicate<ILifeSkillCombatSelectable> match;
							if ((match = <>9__1) == null)
							{
								match = (<>9__1 = ((ILifeSkillCombatSelectable s) => s == valueTuple.Item1));
							}
							return l.Exists(match);
						});
						if (!flag5)
						{
							valueTuple.Item1.ShowStrategyTargetMark(false, null);
						}
					}
				}
			}
			switch (targetConfig.ObjectType)
			{
			case EDebateStrategyTargetObjectType.Pawn:
				this.ShowStrategyTargetPawn(targetInfo, strategyTarget, isMax);
				break;
			case EDebateStrategyTargetObjectType.Node:
				this.ShowStrategyTargetNode(targetInfo, strategyTarget, isMax);
				break;
			case EDebateStrategyTargetObjectType.PawnGrade:
				this.ShowStrategyTargetPawnGrade(targetInfo, strategyTarget);
				break;
			case EDebateStrategyTargetObjectType.StrategyCard:
				this.ShowStrategyTargetStrategyCard(targetInfo, strategyTarget, isMax);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x0011F1CC File Offset: 0x0011D3CC
	private StrategyTarget GetSelectedStrategyTarget(int index)
	{
		return this._selectedStrategyTargetList.CheckIndex(index) ? this._selectedStrategyTargetList[index] : null;
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x0011F1EB File Offset: 0x0011D3EB
	private List<ILifeSkillCombatSelectable> GetSelectedSelectableTarget(int index)
	{
		return this._selectedSelectableTargetList.CheckIndex(index) ? this._selectedSelectableTargetList[index] : null;
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x0011F20C File Offset: 0x0011D40C
	private bool CheckTargetIsAllMeet(out short[] lastShowingTargetInfo, out int curIndex, out bool isMax)
	{
		bool allIsMeet = true;
		lastShowingTargetInfo = null;
		curIndex = 0;
		isMax = false;
		bool targetTypeIsOr = false;
		bool targetTypeIsAnd = true;
		for (int index = 0; index < this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Count; index++)
		{
			short[] targetInfo = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList[index];
			StrategyTarget selectedStrategyTarget = this.GetSelectedStrategyTarget(index);
			int curCount = (selectedStrategyTarget != null) ? selectedStrategyTarget.List.Count : 0;
			short minCount = targetInfo[1];
			short maxCount = targetInfo[2];
			bool isMeet = curCount >= (int)minCount && curCount <= (int)maxCount;
			isMax = (curCount == (int)maxCount);
			bool flag = isMeet;
			if (flag)
			{
				bool flag2 = targetTypeIsOr;
				if (flag2)
				{
					break;
				}
			}
			else
			{
				bool flag3 = lastShowingTargetInfo == null;
				if (flag3)
				{
					curIndex = index;
					lastShowingTargetInfo = targetInfo;
				}
				allIsMeet = false;
				bool flag4 = targetTypeIsAnd;
				if (flag4)
				{
					break;
				}
			}
		}
		bool flag5 = allIsMeet;
		if (flag5)
		{
			curIndex = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Count - 1;
			lastShowingTargetInfo = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Last<short[]>();
		}
		return allIsMeet;
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x0011F354 File Offset: 0x0011D554
	private bool CheckTargetIsAllFixedCount()
	{
		foreach (short[] targetInfo in this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList)
		{
			short minCount = targetInfo[1];
			short maxCount = targetInfo[2];
			bool flag = minCount != maxCount;
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060026E9 RID: 9961 RVA: 0x0011F3DC File Offset: 0x0011D5DC
	private void RefreshTargetCount(int curIndex)
	{
		string content = this.GetTargetCountText(curIndex);
		ConchShipCursor.Instance.ShowLifeSkillCombatUseStrategyTip(content);
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x0011F400 File Offset: 0x0011D600
	private bool GetNeedMergeTarget()
	{
		bool needMerge = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Count > 0;
		bool flag = needMerge;
		if (flag)
		{
			short curTargetTemplateId = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList[0][0];
			foreach (short[] target in this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList)
			{
				bool flag2 = curTargetTemplateId != target[0];
				if (flag2)
				{
					needMerge = false;
					break;
				}
			}
		}
		return needMerge;
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x0011F4D4 File Offset: 0x0011D6D4
	private string GetTargetCountText(int index)
	{
		bool needMerge = this.GetNeedMergeTarget();
		short[] targetInfo = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList[index];
		StrategyTarget selectedStrategyTarget = this.GetSelectedStrategyTarget(index);
		bool flag = needMerge;
		int curCount;
		int minCount;
		int maxCount;
		if (flag)
		{
			curCount = this._selectedStrategyTargetList.Sum((StrategyTarget t) => t.List.Count);
			minCount = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Sum((short[] t) => (int)t[1]);
			maxCount = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Sum((short[] t) => (int)t[2]);
		}
		else
		{
			curCount = ((selectedStrategyTarget != null) ? selectedStrategyTarget.List.Count : 0);
			minCount = (int)targetInfo[1];
			maxCount = (int)targetInfo[2];
		}
		bool isMeet = curCount >= minCount && curCount <= maxCount;
		short targetTemplateId = targetInfo[0];
		DebateStrategyTargetItem targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
		string color = isMeet ? "brightblue" : "brightred";
		return LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_TargetState, targetConfig.Name, curCount.ToString().SetColor(color), maxCount.ToString());
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x0011F654 File Offset: 0x0011D854
	private void ShowStrategyTargetPawn(short[] targetInfo, StrategyTarget strategyTarget, bool isMax)
	{
		UI_LifeSkillCombat2.<>c__DisplayClass172_0 CS$<>8__locals1 = new UI_LifeSkillCombat2.<>c__DisplayClass172_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.targetInfo = targetInfo;
		CS$<>8__locals1.needMerge = this.GetNeedMergeTarget();
		short targetTemplateId = CS$<>8__locals1.targetInfo[0];
		CS$<>8__locals1.targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
		UI_LifeSkillCombat2.<>c__DisplayClass172_0 CS$<>8__locals2 = CS$<>8__locals1;
		int maxCount;
		if (!CS$<>8__locals1.needMerge)
		{
			maxCount = (int)CS$<>8__locals1.targetInfo[2];
		}
		else
		{
			maxCount = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.Sum((short[] t) => (int)t[2]);
		}
		CS$<>8__locals2.maxCount = maxCount;
		CS$<>8__locals1.isAttach = (this.CardArea.FocusingCardItem.CardConfig.MarkType == EDebateStrategyMarkType.Attach);
		CS$<>8__locals1.isRepeatable = (targetTemplateId == 0 || targetTemplateId == 2 || targetTemplateId == 4);
		strategyTarget.List.RemoveAll((ulong id) => CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit((int)id) == null);
		strategyTarget.List.Sort(delegate(ulong a, ulong b)
		{
			LifeSkillCombatUnit unitA = CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit((int)a);
			LifeSkillCombatUnit unitB = CS$<>8__locals1.<>4__this._lifeSkillCombatGrid.FindUnit((int)b);
			return unitA.Position.y.CompareTo(unitB.Position.y);
		});
		using (List<ulong>.Enumerator enumerator = strategyTarget.List.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				UI_LifeSkillCombat2.<>c__DisplayClass172_1 CS$<>8__locals3 = new UI_LifeSkillCombat2.<>c__DisplayClass172_1();
				CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals3.id = enumerator.Current;
				LifeSkillCombatUnit unit = this._lifeSkillCombatGrid.FindUnit((int)CS$<>8__locals3.id);
				this._highlightCardTargetStack.Push(new ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>(unit, unit.RectTrans.parent, unit.RectTrans.GetSiblingIndex(), delegate()
				{
					unit.ShowStrategyTargetMark(false, null);
				}));
				unit.RectTrans.SetParent(this.HighlightRoot);
				bool isSelected2 = this.IsSelected(CS$<>8__locals3.id, CS$<>8__locals3.CS$<>8__locals1.targetConfig.ObjectType);
				bool canSelect = !isMax || isSelected2;
				int curSelectedCount = this._unitSelectionStack.Count((LifeSkillCombatUnit u) => u == unit);
				Func<LifeSkillCombatUnit, bool> <>9__6;
				unit.ShowStrategyTargetMark(canSelect, delegate(bool isSelected)
				{
					bool isAttach = CS$<>8__locals3.CS$<>8__locals1.isAttach;
					if (isAttach)
					{
						IEnumerable<LifeSkillCombatUnit> unitSelectionStack = CS$<>8__locals3.CS$<>8__locals1.<>4__this._unitSelectionStack;
						Func<LifeSkillCombatUnit, bool> predicate;
						if ((predicate = <>9__6) == null)
						{
							predicate = (<>9__6 = ((LifeSkillCombatUnit u) => u == unit));
						}
						int curSelectedCount2 = unitSelectionStack.Count(predicate);
						bool canAdd = unit.CurStrategyCount + curSelectedCount2 < unit.MaxStrategyCount && CS$<>8__locals3.CS$<>8__locals1.<>4__this._unitSelectionStack.Count < CS$<>8__locals3.CS$<>8__locals1.maxCount;
						bool flag = canAdd && ((!CS$<>8__locals3.CS$<>8__locals1.needMerge | CS$<>8__locals3.CS$<>8__locals1.isRepeatable) || curSelectedCount2 < 1);
						if (flag)
						{
							CS$<>8__locals3.CS$<>8__locals1.<>4__this.AddRepeatUnitSelection(CS$<>8__locals3.CS$<>8__locals1.targetInfo, unit);
						}
						unit.SetSelected(true);
					}
					else
					{
						bool flag2 = isSelected && !CS$<>8__locals3.CS$<>8__locals1.<>4__this.IsSelected(CS$<>8__locals3.id, CS$<>8__locals3.CS$<>8__locals1.targetConfig.ObjectType);
						if (flag2)
						{
							CS$<>8__locals3.CS$<>8__locals1.<>4__this.AddSelection(CS$<>8__locals3.CS$<>8__locals1.targetInfo, CS$<>8__locals3.id, unit);
						}
						else
						{
							CS$<>8__locals3.CS$<>8__locals1.<>4__this.RemoveSelection(CS$<>8__locals3.id, unit);
						}
					}
					CS$<>8__locals3.CS$<>8__locals1.<>4__this.CheckTarget();
				}, curSelectedCount);
			}
		}
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x0011F8B4 File Offset: 0x0011DAB4
	private void AddRepeatUnitSelection(short[] targetInfo, LifeSkillCombatUnit unit)
	{
		this._unitSelectionStack.Push(unit);
		this.AddSelection(targetInfo, (ulong)((long)unit.Pawn.Id), unit);
		int curSelectedCount = this._unitSelectionStack.Count((LifeSkillCombatUnit u) => u == unit);
		unit.RefreshStrategy(curSelectedCount, false, true);
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x0011F928 File Offset: 0x0011DB28
	private void RemoveRepeatUnitSelection()
	{
		LifeSkillCombatUnit unit = this._unitSelectionStack.Pop();
		this.RemoveSelection((ulong)((long)unit.Pawn.Id), unit);
		int curSelectedCount = this._unitSelectionStack.Count((LifeSkillCombatUnit u) => u == unit);
		unit.RefreshStrategy(curSelectedCount, false, true);
		unit.SetSelected(curSelectedCount > 0);
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x0011F9A4 File Offset: 0x0011DBA4
	private void ShowStrategyTargetNode(short[] targetInfo, StrategyTarget strategyTarget, bool isMax)
	{
		UI_LifeSkillCombat2.<>c__DisplayClass175_0 CS$<>8__locals1 = new UI_LifeSkillCombat2.<>c__DisplayClass175_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.targetInfo = targetInfo;
		short targetTemplateId = CS$<>8__locals1.targetInfo[0];
		CS$<>8__locals1.targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
		strategyTarget.List.Sort(delegate(ulong id1, ulong id2)
		{
			LifeSkillCombatBlock block = CS$<>8__locals1.<>4__this.GetBlockByPos((IntPair)id1);
			LifeSkillCombatBlock block2 = CS$<>8__locals1.<>4__this.GetBlockByPos((IntPair)id2);
			return block.Position.y.CompareTo(block2.Position.y);
		});
		using (List<ulong>.Enumerator enumerator = strategyTarget.List.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				UI_LifeSkillCombat2.<>c__DisplayClass175_1 CS$<>8__locals2 = new UI_LifeSkillCombat2.<>c__DisplayClass175_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.id = enumerator.Current;
				LifeSkillCombatBlock block = this.GetBlockByPos((IntPair)CS$<>8__locals2.id);
				this._highlightCardTargetStack.Push(new ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>(block, block.RectTrans.parent, block.RectTrans.GetSiblingIndex(), delegate()
				{
					block.ShowStrategyTargetMark(false, null);
				}));
				block.RectTrans.SetParent(this.HighlightRoot);
				bool isSelected2 = this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
				bool canSelect = !isMax || isSelected2;
				block.ShowStrategyTargetMark(canSelect, delegate(bool isSelected)
				{
					bool flag = isSelected && !CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
					if (flag)
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSelection(CS$<>8__locals2.CS$<>8__locals1.targetInfo, CS$<>8__locals2.id, block);
					}
					else
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.RemoveSelection(CS$<>8__locals2.id, block);
					}
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.CheckTarget();
				});
			}
		}
		this._lifeSkillCombatGrid.OnHighlightBlock();
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x0011FB48 File Offset: 0x0011DD48
	private void ShowStrategyTargetPawnGrade(short[] targetInfo, StrategyTarget strategyTarget)
	{
		int index = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.IndexOf(targetInfo);
		StrategyTarget nodeTarget = this._selectedStrategyTargetList.Find(delegate(StrategyTarget t)
		{
			bool result;
			if (t.Type == EDebateStrategyTargetObjectType.Node)
			{
				List<ulong> list = t.List;
				result = (list != null && list.Count > 0);
			}
			else
			{
				result = false;
			}
			return result;
		});
		bool flag = nodeTarget == null;
		if (flag)
		{
			throw new Exception("not found block to select unit grade");
		}
		LifeSkillCombatBlock block = this.GetBlockByPos((IntPair)nodeTarget.List.First<ulong>());
		this.ShowCreateUnitArea(block, delegate(int grade)
		{
			this.HideCreateUnitArea(false);
			StrategyTarget target = this.GetSelectedStrategyTarget(index);
			target.List.Add((ulong)((long)grade));
			this.TaiwuUseCard();
		});
		ConchShipCursor.Instance.SetSelectCountCur(1, "brightblue");
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x0011FC04 File Offset: 0x0011DE04
	private void ShowStrategyTargetStrategyCard(short[] targetInfo, StrategyTarget strategyTarget, bool isMax)
	{
		UI_LifeSkillCombat2.<>c__DisplayClass177_0 CS$<>8__locals1 = new UI_LifeSkillCombat2.<>c__DisplayClass177_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.targetInfo = targetInfo;
		short targetTemplateId = CS$<>8__locals1.targetInfo[0];
		CS$<>8__locals1.targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
		this.CardArea.SelectedCardHasScale = false;
		using (List<ulong>.Enumerator enumerator = strategyTarget.List.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				UI_LifeSkillCombat2.<>c__DisplayClass177_1 CS$<>8__locals2 = new UI_LifeSkillCombat2.<>c__DisplayClass177_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.id = enumerator.Current;
				int index = (int)CS$<>8__locals2.id;
				bool flag = index == this.CardArea.FocusingCardItem.CardView.Index;
				if (!flag)
				{
					LifeSkillCombatCardView cardView = this.CardArea.CardList[index].CardView;
					this._highlightCardTargetStack.Push(new ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>(cardView, cardView.transform.parent, cardView.transform.GetSiblingIndex(), delegate()
					{
						cardView.ShowStrategyTargetMark(false, null);
						cardView.SetEnabled(true, false);
					}));
					cardView.transform.SetParent(this.HighlightRoot);
					bool isSelected2 = this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
					bool canSelect = !isMax || isSelected2;
					cardView.ShowStrategyTargetMark(canSelect, delegate(bool isSelected)
					{
						bool flag2 = isSelected && !CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
						if (flag2)
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSelection(CS$<>8__locals2.CS$<>8__locals1.targetInfo, CS$<>8__locals2.id, cardView);
						}
						else
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.RemoveSelection(CS$<>8__locals2.id, cardView);
						}
						cardView.SetSelected(isSelected);
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.CheckTarget();
					});
				}
			}
		}
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x0011FDC4 File Offset: 0x0011DFC4
	private LifeSkillCombatBlock GetBlockByPos(IntPair nodePos)
	{
		Vector2Int pos = new Vector2Int(nodePos.First, nodePos.Second);
		return this._lifeSkillCombatGrid.FindBlock(pos);
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x0011FDF8 File Offset: 0x0011DFF8
	private bool IsSelected(ulong id, EDebateStrategyTargetObjectType type)
	{
		return this._selectedStrategyTargetList.Exists((StrategyTarget t) => t.Type == type && t.List.Contains(id));
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x0011FE38 File Offset: 0x0011E038
	private void AddSelection(short[] targetInfo, ulong id, ILifeSkillCombatSelectable selectable)
	{
		int index = this.CardArea.FocusingCardItem.CardView.CardConfig.TargetList.IndexOf(targetInfo);
		List<ulong> selectedList = this.GetSelectedStrategyTarget(index).List;
		selectedList.Add(id);
		List<ILifeSkillCombatSelectable> selectedSelectableList = this.GetSelectedSelectableTarget(index);
		selectedSelectableList.Add(selectable);
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x0011FE8C File Offset: 0x0011E08C
	private void RemoveSelection(ulong id, ILifeSkillCombatSelectable selectable)
	{
		foreach (StrategyTarget target in this._selectedStrategyTargetList)
		{
			target.List.Remove(id);
		}
		foreach (List<ILifeSkillCombatSelectable> target2 in this._selectedSelectableTargetList)
		{
			target2.Remove(selectable);
		}
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x0011FF30 File Offset: 0x0011E130
	private void UnselectAllSelectableTarget()
	{
		bool flag = this.CardArea.FocusingCardItem != null;
		if (flag)
		{
			for (int i = 0; i < this._selectedSelectableTargetList.Count; i++)
			{
				List<ILifeSkillCombatSelectable> list = this._selectedSelectableTargetList[i];
				for (int j = 0; j < list.Count; j++)
				{
					ILifeSkillCombatSelectable selectable = list[j];
					LifeSkillCombatBlock block = selectable as LifeSkillCombatBlock;
					bool flag2 = block != null;
					if (flag2)
					{
						block.OnClick();
					}
				}
			}
		}
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x0011FFC0 File Offset: 0x0011E1C0
	private void TaiwuUseCard()
	{
		this.HideAllStrategyTarget(true);
		this.ShowOperationMask(true);
		TaiwuDomainMethod.AsyncCall.DebateGameCastStrategy(this, this.CardArea.FocusingCardItem.CardView.Index, true, this._selectedStrategyTargetList, delegate(int offset, RawDataPool pool)
		{
			DebateGame debateGame = null;
			Serializer.Deserialize(pool, offset, ref debateGame);
			this.Model.SetDebateGame(debateGame);
			this.PlayOperation(0f, delegate
			{
				this.RefreshData(true);
				this.ShowOperationMask(false);
			});
		});
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x00120010 File Offset: 0x0011E210
	private void RefreshCards()
	{
		this.CardArea.RefreshSelfCards(this.Model.DebateGame.PlayerLeft.CanUseCards, this.Model.IsTaiwuRound);
		this.CardArea.RefreshEnemyCards(this.Model.DebateGame.PlayerRight.CanUseCards);
		this.CardArea.RefreshCardGroup(this.Model.DebateGame.PlayerLeft, true);
		this.CardArea.RefreshCardGroup(this.Model.DebateGame.PlayerRight, false);
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x001200A5 File Offset: 0x0011E2A5
	private void HideNoTargetNotify()
	{
		this._noTargetNotify.gameObject.SetActive(false);
		this._noTargetNotify.Target = null;
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x001200C8 File Offset: 0x0011E2C8
	private void SetTimeScale(float speed)
	{
		this.Model.SetSpeed(speed);
		this.SpeedUp.interactable = (this.Model.SpeedIndex < CombatTimeScaleToggle.AvailableTimeScales.Length - 1);
		this.SpeedUp.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.SpeedUp.interactable, false);
		this.SpeedDown.interactable = (this.Model.SpeedIndex > 0);
		this.SpeedDown.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.SpeedDown.interactable, false);
		base.CGet<TextMeshProUGUI>("Speed").text = string.Format("X {0}", this.Model.Speed);
		this.Model.RefreshTimeScale();
		this.SetButtonInteractable(this.BtnForceGiveUp, this.Model.IsTaiwuRound && !this.Model.Pause);
		this.SetButtonInteractable(this.BtnGiveUp, this.Model.IsTaiwuRound && !this.Model.Pause);
		this.SetButtonInteractable(this.BtnTurnEnd, this.Model.IsTaiwuRound && !this.Model.Pause);
		bool autoSaveDebateSpeed = this.SettingData.AutoSaveDebateSpeed;
		if (autoSaveDebateSpeed)
		{
			this.SettingData.DebateSpeed = speed;
		}
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x00120234 File Offset: 0x0011E434
	private void OnClickSpeedUp()
	{
		int index = this.Model.SpeedIndex;
		index++;
		bool flag = index >= CombatTimeScaleToggle.AvailableTimeScales.Length;
		if (flag)
		{
			index = 0;
		}
		this.SetTimeScale(CombatTimeScaleToggle.AvailableTimeScales[index]);
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x00120274 File Offset: 0x0011E474
	private void OnClickSpeedDown()
	{
		int index = this.Model.SpeedIndex;
		index--;
		bool flag = index < 0;
		if (flag)
		{
			index = CombatTimeScaleToggle.AvailableTimeScales.Length - 1;
		}
		this.SetTimeScale(CombatTimeScaleToggle.AvailableTimeScales[index]);
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x001202B2 File Offset: 0x0011E4B2
	private void ShowCombatStateInfo()
	{
		this.StateBack.SetActive(true);
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x001202C2 File Offset: 0x0011E4C2
	private void HideCombatStateInfo()
	{
		this.StateBack.SetActive(false);
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x001202D4 File Offset: 0x0011E4D4
	private void OnClickAutoFight()
	{
		bool isGameOver = this.Model.IsGameOver;
		if (!isGameOver)
		{
			this.Model.IsAuto = !this.Model.IsAuto;
			this.UpdateAutoFightMark(this.Model.IsAuto);
			bool isAuto = this.Model.IsAuto;
			if (isAuto)
			{
				bool flag = this.BtnTurnEnd.interactable && this.Model.IsTaiwuRound && !this.Model.IsPlayingOperation;
				if (flag)
				{
					this.AutoAction();
				}
			}
			else
			{
				TaiwuDomainMethod.Call.DebateGameSetTaiwuAi(this.Element.GameDataListenerId, false);
			}
		}
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x0012037F File Offset: 0x0011E57F
	private void StopAutoFight()
	{
		this.Model.IsAuto = false;
		this.UpdateAutoFightMark(this.Model.IsAuto);
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x001203A4 File Offset: 0x0011E5A4
	private void UpdateAutoFightMark(bool autoCombat)
	{
		CImage autoFightIcon = base.CGet<CImage>("AutoFightIcon");
		autoFightIcon.SetSprite(autoCombat ? "combat_top_icon_0" : "combat_top_icon_1", false, null);
		autoFightIcon.rectTransform.DOKill(false);
		if (autoCombat)
		{
			autoFightIcon.rectTransform.DOLocalRotate(Vector3.zero.SetZ(-360f), 3f, DG.Tweening.RotateMode.Fast).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1).SetUpdate(true);
		}
		else
		{
			autoFightIcon.rectTransform.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x00120434 File Offset: 0x0011E634
	private void EnterSelectRemovingCardMode(int count)
	{
		List<short> cards = this.Model.DebateGame.GetPlayerByPlayerIsTaiwu(true).CanUseCards;
		this.Model.IsRemovingCards = true;
		this.Model.RemovingCards.Clear();
		this.RemoveWarningText.text = LanguageKey.LK_LifeskillCombat_RemoveWarning.TrFormat(GlobalConfig.Instance.DebateMaxCanUseCards, count);
		this.RefreshSelectRemovingCardMode(count);
		this.CardArea.SelectedCardHasScale = false;
		int i = 0;
		while (i < cards.Count && i < this.CardArea.CardList.Count)
		{
			int index = i;
			LifeSkillCombatCardView cardView = this.CardArea.CardList[index].CardView;
			this._highlightCardTargetStack.Push(new ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>(cardView, cardView.transform.parent, cardView.transform.GetSiblingIndex(), delegate()
			{
				cardView.ShowStrategyTargetMark(false, null);
				cardView.SetEnabled(true, false);
			}));
			cardView.transform.SetParent(this.HighlightRoot);
			cardView.ShowStrategyTargetMark(true, delegate(bool isSelected)
			{
				bool flag = this.Model.RemovingCards.Contains(index);
				if (flag)
				{
					this.Model.RemovingCards.Remove(index);
				}
				else
				{
					this.Model.RemovingCards.Add(index);
				}
				cardView.SetSelected(isSelected);
				this.RefreshSelectRemovingCardMode(count);
			});
			i++;
		}
		this.RemoveWarning.SetActive(true);
		this.ButtonConfirm.gameObject.SetActive(false);
		this.ControlPanel.transform.SetParent(this.CardArea.transform);
		this.SelectCardArea.gameObject.SetActive(true);
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x00120600 File Offset: 0x0011E800
	private void RefreshSelectRemovingCardMode(int count)
	{
		bool flag = this.Model.RemovingCards.Count == count;
		if (flag)
		{
			this.ShowOperationMask(true);
			this.ExitSelectRemovingCardMode();
			TaiwuDomainMethod.AsyncCall.DebateGameRemoveCards(this, true, this.Model.RemovingCards, delegate(int offset, RawDataPool pool)
			{
				DebateGame debateGame = null;
				Serializer.Deserialize(pool, offset, ref debateGame);
				this.Model.SetDebateGame(debateGame);
				this.PlayOperation(0f, delegate
				{
					this.RefreshData(true);
					this.ShowOperationMask(false);
					this.OnClickBtnTurnEnd();
				});
			});
		}
		else
		{
			ConchShipCursor.Instance.ShowLifeSkillCombatUseStrategyTip(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_TargetState, LanguageKey.LK_LifeSkillCombat_CardGroup_Used.Tr(), this.Model.RemovingCards.Count.ToString().SetColor("brightred"), count.ToString()));
		}
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x001206A0 File Offset: 0x0011E8A0
	private void ExitSelectRemovingCardMode()
	{
		this.Model.IsRemovingCards = false;
		this.HideAllStrategyTarget(true);
		this.ControlPanel.transform.SetParent(base.transform);
		ConchShipCursor.Instance.HideLifeSkillCombatUseStrategyTip();
		this.SelectCardArea.gameObject.SetActive(false);
		this.RemoveWarning.SetActive(false);
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x00120703 File Offset: 0x0011E903
	private void OnClickButtonResetStrategy()
	{
		this.OperationMask.SetActive(true);
		TaiwuDomainMethod.AsyncCall.DebateGameResetCards(this, true, true, delegate(int offset, RawDataPool pool)
		{
			DebateGame debateGame = null;
			Serializer.Deserialize(pool, offset, ref debateGame);
			this.Model.SetDebateGame(debateGame);
			this.PlayOperation(0f, delegate
			{
				this.RefreshData(true);
				this.OperationMask.SetActive(false);
			});
		});
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x00120728 File Offset: 0x0011E928
	private void RefreshButtonResetStrategy()
	{
		DebatePlayer player = this.Model.DebateGame.GetPlayerByPlayerIsTaiwu(true);
		bool playerCanUseResetStrategy = this.Model.DebateGame.GetPlayerCanUseResetStrategy(true);
		if (playerCanUseResetStrategy)
		{
			this.ButtonResetStrategy.interactable = true;
			this.ButtonResetStrategy.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
			this.ButtonResetStrategy.gameObject.SetActive(true);
		}
		else
		{
			bool flag = player.OwnedCards.Count == 0 && player.UsedCards.Count != 0;
			if (flag)
			{
				this.ButtonResetStrategy.interactable = false;
				this.ButtonResetStrategy.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
				this.ButtonResetStrategy.gameObject.SetActive(true);
			}
			else
			{
				this.ButtonResetStrategy.gameObject.SetActive(false);
			}
		}
		this.ButtonResetStrategy.GetComponent<TooltipInvoker>().PresetParam = ((player.Pressure == player.MaxPressure) ? this.Model.ResetStrategyPresetTipContent[1] : this.Model.ResetStrategyPresetTipContent[0]);
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x00120844 File Offset: 0x0011EA44
	public void OnEnterButtonResetStrategy()
	{
		DebatePlayer player = this.Model.DebateGame.GetPlayerByPlayerIsTaiwu(true);
		this._lifeSkillCombatPlayerTaiwu.ShowPreviewPressure(player.Pressure, player.MaxPressure, GlobalConfig.Instance.DebateResetCardsPressureDelta);
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x00120886 File Offset: 0x0011EA86
	public void OnExitButtonResetStrategy()
	{
		this._lifeSkillCombatPlayerTaiwu.HidePreviewPressure();
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x00120898 File Offset: 0x0011EA98
	private void RefreshData(bool hasAnim = true)
	{
		this._lifeSkillCombatGrid.RefreshAllUnit(true, true);
		this._lifeSkillCombatGrid.RefreshAllBlock();
		this._lifeSkillCombatPlayerTaiwu.Refresh(this.Model.DebateGame.PlayerLeft, hasAnim);
		this._lifeSkillCombatPlayerEnemy.Refresh(this.Model.DebateGame.PlayerRight, hasAnim);
		this.RefreshCards();
		this.RefreshButtonResetStrategy();
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x00120908 File Offset: 0x0011EB08
	private void LogStepInfo(string message, bool appendAdversaryInfo = true)
	{
		string adversaryInfo = string.Empty;
		if (appendAdversaryInfo)
		{
			adversaryInfo = string.Format("[enemy character {0}] | ", this.Model.EnemyCharId);
		}
		GLog.TagLog("LifeSkillCombat", string.Format("{0}{1:MM/dd-HH:mm:ss} | {2}", adversaryInfo, DateTime.Now, message), Array.Empty<object>());
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x00120962 File Offset: 0x0011EB62
	private void SetButtonInteractable(CButtonObsolete button, bool interactable)
	{
		button.interactable = interactable;
		button.GetComponent<DisableStyleRoot>().SetStyleEffect(!interactable, false);
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x0012097E File Offset: 0x0011EB7E
	private void ShowCombatLifeSkillHiddenInfo(ArgumentBox argumentBox)
	{
		this._lifeSkillCombatGrid.RefreshAllUnit(true, true);
		this._lifeSkillCombatPlayerEnemy.Refresh(this.Model.DebateGame.PlayerRight, false);
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x001209AC File Offset: 0x0011EBAC
	private void ShowCombatLifeSkillTalk(ArgumentBox argumentBox)
	{
		short id;
		argumentBox.Get("Id", out id);
		bool isTaiwu;
		bool flag = !argumentBox.Get("IsTaiwu", out isTaiwu);
		if (flag)
		{
			isTaiwu = true;
		}
		LifeSkillCombatPlayer player = isTaiwu ? this._lifeSkillCombatPlayerTaiwu : this._lifeSkillCombatPlayerEnemy;
		player.Speak(id, 4f);
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x001209FD File Offset: 0x0011EBFD
	private void ShowCreateUnitArea(LifeSkillCombatBlock block, Action<int> onSelectGradeForStrategy = null)
	{
		this.UnitGradeAreaMask.gameObject.SetActive(true);
		this._lifeSkillCombatGrid.RefreshUnitGradeArea(block, onSelectGradeForStrategy, delegate(int cost)
		{
			this._lifeSkillCombatPlayerTaiwu.PreviewEnergy(this._lifeSkillCombatPlayerTaiwu.Energy - cost, false);
		});
		this._lifeSkillCombatGrid.ShowUnitGradeArea();
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x00120A38 File Offset: 0x0011EC38
	private void HideCreateUnitArea(bool unselect = true)
	{
		this.UnitGradeAreaMask.gameObject.SetActive(false);
		this._lifeSkillCombatGrid.HideUnitGradeArea();
		if (unselect)
		{
			this.UnselectAllSelectableTarget();
		}
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x00120A70 File Offset: 0x0011EC70
	private void ShowOperationMask(bool show)
	{
		this.OperationMask.gameObject.SetActive(show);
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x00120A84 File Offset: 0x0011EC84
	private short GetPlaySpeakConfigOnGameOver(bool isWin)
	{
		return isWin ? 19 : 18;
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x00120A90 File Offset: 0x0011EC90
	private Transform GetCardGroupTransform(int location)
	{
		return (location == 5) ? this._lifeSkillCombatPlayerEnemy.RectTrans : this.CardArea.GetCardGroupTransform(location);
	}

	// Token: 0x04001C61 RID: 7265
	private readonly LifeSkillCombatPlayer _lifeSkillCombatPlayerTaiwu = new LifeSkillCombatPlayer();

	// Token: 0x04001C62 RID: 7266
	private readonly LifeSkillCombatPlayer _lifeSkillCombatPlayerEnemy = new LifeSkillCombatPlayer();

	// Token: 0x04001C63 RID: 7267
	private bool _flagsNeedUpdate;

	// Token: 0x04001C64 RID: 7268
	private readonly LifeSkillCombatGrid _lifeSkillCombatGrid = new LifeSkillCombatGrid();

	// Token: 0x04001C65 RID: 7269
	public const string EffectConflictTaiwuWin = "eff_lifeskillcombat_ui_pen_lanqiang";

	// Token: 0x04001C66 RID: 7270
	public const string EffectConflictEnemyWin = "eff_lifeskillcombat_ui_pen_hongqiang";

	// Token: 0x04001C67 RID: 7271
	public const string EffectConflictDraw = "eff_lifeskillcombat_ui_pen_yiyangqiang";

	// Token: 0x04001C68 RID: 7272
	public const string EffectConflictDestroyTaiwu = "eff_lifeskillcombat_ui_pen_posui1";

	// Token: 0x04001C69 RID: 7273
	public const string EffectConflictDestroyEnemy = "eff_lifeskillcombat_ui_pen_posui2";

	// Token: 0x04001C6A RID: 7274
	public const string EffectArriveEndEnemy = "eff_lifeskillcombat_ui_dx_hualizi2";

	// Token: 0x04001C6B RID: 7275
	public const string EffectArriveEndTaiwu = "eff_lifeskillcombat_ui_dx_hualizi";

	// Token: 0x04001C6C RID: 7276
	public const string EffectStrategyDeleteTaiwuPawn = "EffectStrategyDeleteTaiwuPawn";

	// Token: 0x04001C6D RID: 7277
	public const string EffectStrategyDeleteEnemyPawn = "EffectStrategyDeleteEnemyPawn";

	// Token: 0x04001C6E RID: 7278
	public const string EffectSelfStrategyTargetPawn = "eff_lifeskillcombat_ui_dxkp_shan_lan";

	// Token: 0x04001C6F RID: 7279
	public const string EffectEnemyStrategyTargetPawn = "eff_lifeskillcombat_ui_dxkp_shan_hong";

	// Token: 0x04001C70 RID: 7280
	public static readonly string BGM = "Mu_art_jiaoyi_3";

	// Token: 0x04001C71 RID: 7281
	public static readonly string SoundConflict = "art_battle";

	// Token: 0x04001C72 RID: 7282
	public static readonly string SoundShowCreateUnitPanel = "art_level";

	// Token: 0x04001C73 RID: 7283
	public static readonly string SoundHoverCreateUnitPanel = "art_ClickHover";

	// Token: 0x04001C74 RID: 7284
	public static readonly string SoundCreateUnit = "art_generate";

	// Token: 0x04001C75 RID: 7285
	public static readonly string SoundMoveUnit = "art_ChessMove";

	// Token: 0x04001C76 RID: 7286
	public static readonly string SoundProtectUnit = "art_protect";

	// Token: 0x04001C77 RID: 7287
	public static readonly string SoundStrategyDeleteUnit = "art_prohibit";

	// Token: 0x04001C78 RID: 7288
	public static readonly string SoundReveal = "art_generate";

	// Token: 0x04001C79 RID: 7289
	public static readonly string SoundMoveCard = "art_CardMove";

	// Token: 0x04001C7A RID: 7290
	public static readonly string SoundUseCard = "art_use";

	// Token: 0x04001C7B RID: 7291
	public static readonly string SoundAddUnitStrategy = "ui_art_card";

	// Token: 0x04001C7C RID: 7292
	public static readonly string SoundRemoveUnitStrategy = "art_remove";

	// Token: 0x04001C7D RID: 7293
	public static readonly string SoundUnitArrive = "art_ChessHurt";

	// Token: 0x04001C7E RID: 7294
	public static readonly string SoundAddScore = "art_add";

	// Token: 0x04001C7F RID: 7295
	public static readonly string SoundRemoveScore = "art_hurt";

	// Token: 0x04001C80 RID: 7296
	public static readonly string SoundAddStress = "art_stressed";

	// Token: 0x04001C81 RID: 7297
	public static readonly string SoundStressReduceScore = "art_burn";

	// Token: 0x04001C82 RID: 7298
	public static readonly string SoundOpenCardGroup = "art_OpenCard";

	// Token: 0x04001C83 RID: 7299
	public static readonly string SoundStage1 = "ui_art_round_1";

	// Token: 0x04001C84 RID: 7300
	public static readonly string SoundStage2 = "ui_art_round_2";

	// Token: 0x04001C85 RID: 7301
	public static readonly string SoundStage3 = "art_EndDrum";

	// Token: 0x04001C86 RID: 7302
	public static readonly string SoundStressEffect = "art_debuff";

	// Token: 0x04001C87 RID: 7303
	public static readonly string SoundChangeFlag = "art_FlagChange";

	// Token: 0x04001C88 RID: 7304
	public static readonly string SoundAddNodeEffect = "art_grid";

	// Token: 0x04001C89 RID: 7305
	public static readonly string SoundUseCardFailed = "art_burn";

	// Token: 0x04001C8A RID: 7306
	public static readonly string SoundCreateUnitFailed = "art_burn";

	// Token: 0x04001C8B RID: 7307
	public static readonly string SoundStressUp = "art_PressureUp";

	// Token: 0x04001C8C RID: 7308
	public static readonly string SoundStressDown = "art_PressureDown";

	// Token: 0x04001C8D RID: 7309
	public static readonly string SoundHalfRound = "art_BackgroundFire";

	// Token: 0x04001C8E RID: 7310
	public static readonly string SoundUnitPowerUp = "art_PointUp";

	// Token: 0x04001C8F RID: 7311
	public static readonly string SoundUnitPowerDown = "art_PointDown";

	// Token: 0x04001C90 RID: 7312
	public static readonly string SoundCardFly = "art_CardFly";

	// Token: 0x04001C91 RID: 7313
	[TupleElementNames(new string[]
	{
		"self",
		"parent",
		"childIndex",
		"onCancel"
	})]
	private readonly Stack<ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>> _highlightCardTargetStack = new Stack<ValueTuple<ILifeSkillCombatSelectable, Transform, int, Action>>();

	// Token: 0x04001C92 RID: 7314
	private readonly List<StrategyTarget> _selectedStrategyTargetList = new List<StrategyTarget>();

	// Token: 0x04001C93 RID: 7315
	private readonly Stack<LifeSkillCombatUnit> _unitSelectionStack = new Stack<LifeSkillCombatUnit>();

	// Token: 0x04001C94 RID: 7316
	private readonly List<List<ILifeSkillCombatSelectable>> _selectedSelectableTargetList = new List<List<ILifeSkillCombatSelectable>>();
}
