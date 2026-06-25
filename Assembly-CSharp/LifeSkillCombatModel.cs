using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Components.Combat;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu.Debate;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class LifeSkillCombatModel : ISingletonInit, IDisposable
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0005F1B6 File Offset: 0x0005D3B6
	// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x0005F1BE File Offset: 0x0005D3BE
	public CharacterDisplayData TaiwuCharData { get; private set; }

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0005F1C7 File Offset: 0x0005D3C7
	// (set) Token: 0x06000FDB RID: 4059 RVA: 0x0005F1CF File Offset: 0x0005D3CF
	public CharacterDisplayData EnemyCharData { get; private set; }

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0005F1D8 File Offset: 0x0005D3D8
	public int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06000FDD RID: 4061 RVA: 0x0005F1E4 File Offset: 0x0005D3E4
	// (set) Token: 0x06000FDE RID: 4062 RVA: 0x0005F1EC File Offset: 0x0005D3EC
	public bool HideTaiwuAudience { get; private set; }

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06000FDF RID: 4063 RVA: 0x0005F1F5 File Offset: 0x0005D3F5
	public bool IsTaiwuRound
	{
		get
		{
			return this.DebateGame.GetIsTaiwuTurn();
		}
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x0005F202 File Offset: 0x0005D402
	// (set) Token: 0x06000FE1 RID: 4065 RVA: 0x0005F20A File Offset: 0x0005D40A
	public bool DataReady { get; private set; }

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x0005F213 File Offset: 0x0005D413
	// (set) Token: 0x06000FE3 RID: 4067 RVA: 0x0005F21B File Offset: 0x0005D41B
	public DebateGame DebateGame { get; private set; } = new DebateGame();

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0005F224 File Offset: 0x0005D424
	// (set) Token: 0x06000FE5 RID: 4069 RVA: 0x0005F22C File Offset: 0x0005D42C
	public bool IsAuto { get; set; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x0005F235 File Offset: 0x0005D435
	// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x0005F23D File Offset: 0x0005D43D
	public LifeSkillCombatCardItem FocusingCardItem { get; set; }

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x0005F246 File Offset: 0x0005D446
	public bool IsHalfRound
	{
		get
		{
			return this.DebateGame.Round > DebateConstants.MaxRound / 2;
		}
	}

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x0005F25C File Offset: 0x0005D45C
	// (set) Token: 0x06000FEA RID: 4074 RVA: 0x0005F264 File Offset: 0x0005D464
	public bool Pause { get; private set; }

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06000FEB RID: 4075 RVA: 0x0005F26D File Offset: 0x0005D46D
	// (set) Token: 0x06000FEC RID: 4076 RVA: 0x0005F275 File Offset: 0x0005D475
	public float Speed { get; private set; }

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x06000FED RID: 4077 RVA: 0x0005F27E File Offset: 0x0005D47E
	// (set) Token: 0x06000FEE RID: 4078 RVA: 0x0005F286 File Offset: 0x0005D486
	public int SpeedIndex { get; private set; }

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000FEF RID: 4079 RVA: 0x0005F290 File Offset: 0x0005D490
	// (remove) Token: 0x06000FF0 RID: 4080 RVA: 0x0005F2C8 File Offset: 0x0005D4C8
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event LifeSkillCombatModel.OnEnd EndEvent;

	// Token: 0x06000FF1 RID: 4081 RVA: 0x0005F2FD File Offset: 0x0005D4FD
	public void Dispose()
	{
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x0005F300 File Offset: 0x0005D500
	public void Init()
	{
		for (int i = 0; i < 3; i++)
		{
			this.SelfAudienceList.Add(null);
			this.EnemyAudienceList.Add(null);
		}
		this.ResetStrategyPresetTipContent = new List<string[]>
		{
			new string[]
			{
				LanguageKey.LK_LifeskillCombat_ResetStrategy.Tr(),
				LanguageKey.LK_LifeskillCombat_ResetStrategy_Tip.Tr()
			},
			new string[]
			{
				LanguageKey.LK_LifeskillCombat_ResetStrategy.Tr(),
				LanguageKey.LK_LifeskillCombat_ResetStrategy_Tip.Tr() + "\n" + LanguageKey.LK_LifeskillCombat_ResetStrategy_Tip2.Tr()
			}
		};
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x0005F3AC File Offset: 0x0005D5AC
	public void StartLifeSkillCombat(int enemyCharId, sbyte lifeSkillType = -1, bool hideTaiwuAudience = false)
	{
		this.DataReady = false;
		this.EnemyCharId = enemyCharId;
		this.LifeSkillType = lifeSkillType;
		this.HideTaiwuAudience = hideTaiwuAudience;
		SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(this.TaiwuCharId, false);
		bool flag = this.LifeSkillType == -1 || this.LifeSkillType == 16;
		if (flag)
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("Self", this.TaiwuCharId);
			box.Set("EnemyId", this.EnemyCharId);
			box.SetObject("CallBack", new Action<sbyte, sbyte>(this.StartMatch));
			UIElement.LifeSkillCombatBegin.SetOnInitArgs(box);
			UIManager.Instance.ShowUI(UIElement.LifeSkillCombatBegin, true);
		}
		else
		{
			this.StartMatch(this.LifeSkillType, -1);
		}
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0005F478 File Offset: 0x0005D678
	public void StartMatch(sbyte lifeSkillType, sbyte firstTurn)
	{
		this.LifeSkillType = lifeSkillType;
		this.DebateGame.IsTaiwuFirst = (firstTurn == 1);
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(null, new List<int>
		{
			this.TaiwuCharId,
			this.EnemyCharId
		}, delegate(int offset, RawDataPool pool)
		{
			List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
			Serializer.Deserialize(pool, offset, ref charDataList);
			this.TaiwuCharData = ((charDataList != null && charDataList.CheckIndex(0)) ? charDataList[0] : null);
			this.EnemyCharData = ((charDataList != null && charDataList.CheckIndex(1)) ? charDataList[1] : null);
			ExtraDomainMethod.Call.InitAiLifeSkillCombatUsedCard(-1, this.LifeSkillType, this.EnemyCharId);
			this.DataReady = true;
		});
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("Self", this.TaiwuCharId);
		box.Set("EnemyId", this.EnemyCharId);
		box.Set("LifeSkillType", this.LifeSkillType);
		UIElement.LifeSkillCombatBegin.SetOnInitArgs(box);
		UIManager.Instance.ShowUI(UIElement.LifeSkillCombatBegin, true);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0005F528 File Offset: 0x0005D728
	public void SetForceSilentResult(sbyte type, bool result, sbyte behaviorType)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.Set("Type", type);
		args.Set("Result", result);
		args.Set("Behavior", behaviorType);
		GEvent.OnEvent(UiEvents.OnLifeSkillCombatForceSilentResult, args);
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0005F574 File Offset: 0x0005D774
	public List<CharacterDisplayData> GetAudienceList(bool isSelf)
	{
		return isSelf ? this.SelfAudienceList : this.EnemyAudienceList;
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0005F588 File Offset: 0x0005D788
	public CharacterDisplayData GetAudienceData(int charId)
	{
		return this.SelfAudienceList.Union(this.EnemyAudienceList).FirstOrDefault((CharacterDisplayData p) => p != null && p.CharacterId == charId);
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x0005F5CC File Offset: 0x0005D7CC
	public void ClearAudience()
	{
		for (int i = 0; i < 3; i++)
		{
			this.SelfAudienceList[i] = null;
			this.EnemyAudienceList[i] = null;
		}
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x0005F608 File Offset: 0x0005D808
	public void SetDebateGame(DebateGame debateGame)
	{
		this.DebateGame = debateGame;
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0005F614 File Offset: 0x0005D814
	public bool NeedSelectTarget(DebateStrategyItem debateStrategyItem)
	{
		List<short[]> targetList = debateStrategyItem.TargetList;
		return targetList != null && targetList.Count > 0;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x0005F637 File Offset: 0x0005D837
	public bool CheckCost(DebateStrategyItem debateStrategyItem)
	{
		return this.DebateGame.PlayerLeft.StrategyPoint >= (int)debateStrategyItem.UsedCost;
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0005F654 File Offset: 0x0005D854
	public void SetSpeed(float speed)
	{
		this.Speed = speed;
		this.SpeedIndex = CombatTimeScaleToggle.AvailableTimeScales.IndexOf(speed);
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x0005F671 File Offset: 0x0005D871
	public void RefreshTimeScale()
	{
		Time.timeScale = (this.Pause ? 0f : this.Speed);
		AudioManager.Instance.SetSoundTimeScale(Time.timeScale);
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x0005F69F File Offset: 0x0005D89F
	public void ResetPauseAndTimeScale()
	{
		this.Pause = false;
		Time.timeScale = 1f;
		AudioManager.Instance.SetSoundTimeScale(Time.timeScale);
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x0005F6C5 File Offset: 0x0005D8C5
	public void SetPause(bool pause)
	{
		this.Pause = pause;
		this.RefreshTimeScale();
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x0005F6D7 File Offset: 0x0005D8D7
	public void End(bool isTaiwuWin)
	{
		LifeSkillCombatModel.OnEnd endEvent = this.EndEvent;
		if (endEvent != null)
		{
			endEvent(isTaiwuWin);
		}
	}

	// Token: 0x04000E84 RID: 3716
	public int EnemyCharId;

	// Token: 0x04000E85 RID: 3717
	public sbyte LifeSkillType;

	// Token: 0x04000E88 RID: 3720
	public bool IsInCombat;

	// Token: 0x04000E89 RID: 3721
	public bool IsGameOver;

	// Token: 0x04000E8A RID: 3722
	public bool ShowHiddenInfo = false;

	// Token: 0x04000E8B RID: 3723
	public readonly List<CharacterDisplayData> SelfAudienceList = new List<CharacterDisplayData>();

	// Token: 0x04000E8C RID: 3724
	public readonly List<CharacterDisplayData> EnemyAudienceList = new List<CharacterDisplayData>();

	// Token: 0x04000E8D RID: 3725
	public const int AudienceCount = 3;

	// Token: 0x04000E8F RID: 3727
	public List<int> RemovingCards = new List<int>();

	// Token: 0x04000E90 RID: 3728
	public bool IsRemovingCards = false;

	// Token: 0x04000E91 RID: 3729
	public List<string[]> ResetStrategyPresetTipContent;

	// Token: 0x04000E93 RID: 3731
	public bool IsPlayingOperation;

	// Token: 0x020011E2 RID: 4578
	// (Invoke) Token: 0x0600C415 RID: 50197
	public delegate void OnEnd(bool isTaiwuWin);
}
