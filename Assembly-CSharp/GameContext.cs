using System;
using System.Collections.Generic;
using GameData;
using GameData.Adventure;
using GameData.Combat.Math;
using GameData.Domains.Adventure;
using GameData.Domains.Map;
using GameData.Domains.Story.MainStory;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.World;
using GameData.Utilities;
using Redzen.Random;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class GameContext : IGameContext
{
	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000819 RID: 2073 RVA: 0x00037C3A File Offset: 0x00035E3A
	public uint WorldId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().WorldId;
		}
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00037C46 File Offset: 0x00035E46
	public bool IsProfessionalSkillUnlockedAndEquipped(int professionSkillTemplateId)
	{
		return SingletonObject.getInstance<ProfessionModel>().IsProfessionalSkillUnlockedAndEquipped(professionSkillTemplateId);
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00037C53 File Offset: 0x00035E53
	public ProfessionData GetProfessionData(int professionSkillTemplateId)
	{
		return SingletonObject.getInstance<ProfessionModel>().GetProfessionData(professionSkillTemplateId);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00037C60 File Offset: 0x00035E60
	public bool GetWorldFunctionsStatus(byte worldFunctionType)
	{
		return SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(worldFunctionType);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00037C6D File Offset: 0x00035E6D
	public byte GetAreaSize(short areaId)
	{
		return SingletonObject.getInstance<WorldMapModel>().GetAreaSize(areaId);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00037C7A File Offset: 0x00035E7A
	public MapBlockData GetBlockData(Location location)
	{
		return SingletonObject.getInstance<WorldMapModel>().GetBlockData(location);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00037C88 File Offset: 0x00035E88
	public IEnumerable<short> GetGroupBlockIds(Location rootLocation, MapBlockData rootBlock)
	{
		HashSet<short> groupBlockIds;
		IEnumerable<short> result;
		if (!SingletonObject.getInstance<WorldMapModel>().BlockGroupDict.TryGetValue(rootLocation, out groupBlockIds))
		{
			IEnumerable<short> enumerable = Array.Empty<short>();
			result = enumerable;
		}
		else
		{
			IEnumerable<short> enumerable = groupBlockIds;
			result = enumerable;
		}
		return result;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x00037CB5 File Offset: 0x00035EB5
	public bool IsTaskFinished(int taskInfoId)
	{
		return SingletonObject.getInstance<TaskModel>().IsTaskFinished(taskInfoId);
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x00037CC2 File Offset: 0x00035EC2
	public bool IsTaskInProgress(int taskInfoId)
	{
		return SingletonObject.getInstance<TaskModel>().IsTaskInProgress(taskInfoId);
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06000822 RID: 2082 RVA: 0x00037CCF File Offset: 0x00035ECF
	public IRandomSource Random
	{
		get
		{
			return GameApp.Random;
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06000823 RID: 2083 RVA: 0x00037CD6 File Offset: 0x00035ED6
	public string Language
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>().Language;
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06000824 RID: 2084 RVA: 0x00037CE2 File Offset: 0x00035EE2
	public string DataPath
	{
		get
		{
			return Application.dataPath;
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000825 RID: 2085 RVA: 0x00037CE9 File Offset: 0x00035EE9
	public bool DevOnlyPredefinedLog
	{
		get
		{
			return GameApp.Instance.IsTestBranch;
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x06000826 RID: 2086 RVA: 0x00037CF5 File Offset: 0x00035EF5
	public bool HideTaiwuOriginalSurname
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>().HideTaiwuOriginalSurname;
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x06000827 RID: 2087 RVA: 0x00037D01 File Offset: 0x00035F01
	public int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000828 RID: 2088 RVA: 0x00037D0D File Offset: 0x00035F0D
	public Location TaiwuLocation
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000829 RID: 2089 RVA: 0x00037D19 File Offset: 0x00035F19
	public sbyte TaiwuGender
	{
		get
		{
			return SingletonObject.getInstance<TaiwuCharacterModel>().Gender;
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x0600082A RID: 2090 RVA: 0x00037D25 File Offset: 0x00035F25
	public sbyte TaiwuDisplayingGender
	{
		get
		{
			return SingletonObject.getInstance<TaiwuCharacterModel>().DisplayingGender;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x0600082B RID: 2091 RVA: 0x00037D31 File Offset: 0x00035F31
	public int CurrDate
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().CurrDate;
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x0600082C RID: 2092 RVA: 0x00037D3D File Offset: 0x00035F3D
	[Obsolete]
	public short MainStoryLineProgress
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x0600082D RID: 2093 RVA: 0x00037D40 File Offset: 0x00035F40
	public byte WorldResourceAmountType
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().WorldResourceAmountType;
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x0600082E RID: 2094 RVA: 0x00037D4C File Offset: 0x00035F4C
	public sbyte XiangshuProgress
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x0600082F RID: 2095 RVA: 0x00037D58 File Offset: 0x00035F58
	public bool NoProfessionSkillCooldown
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000830 RID: 2096 RVA: 0x00037D5B File Offset: 0x00035F5B
	public CValuePercent MoveTimeCostPercent
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>().MoveTimeCostPercent;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000831 RID: 2097 RVA: 0x00037D6C File Offset: 0x00035F6C
	public TwelveImmortalsCacheData TwelveImmortalsCache
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TwelveImmortalsCache;
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000832 RID: 2098 RVA: 0x00037D78 File Offset: 0x00035F78
	public ChallengeModeData ChallengeModeData
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().ChallengeModeData;
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000833 RID: 2099 RVA: 0x00037D84 File Offset: 0x00035F84
	public IReadOnlyDictionary<int, string> CustomTexts
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().CustomTexts;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000834 RID: 2100 RVA: 0x00037D90 File Offset: 0x00035F90
	public AdventureCore AdventureCore
	{
		get
		{
			return AdventureRemakeModel.Core;
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00037D98 File Offset: 0x00035F98
	public AdventureRuntime GetAdventure(int adventureId)
	{
		return SingletonObject.getInstance<AdventureRemakeModel>().AdventureRemakeDict.GetOrDefault(adventureId);
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x00037DBC File Offset: 0x00035FBC
	public AdventureMajorEvent GetMajorEvent(int majorEventId)
	{
		return SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventDict.GetOrDefault(majorEventId);
	}
}
