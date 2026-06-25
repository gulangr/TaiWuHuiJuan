using System;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class TimeManager
{
	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600117C RID: 4476 RVA: 0x0006A109 File Offset: 0x00068309
	private static BasicGameData BasicGameData
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>();
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x0600117D RID: 4477 RVA: 0x0006A110 File Offset: 0x00068310
	private int CurDate
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().CurrDate;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x0600117E RID: 4478 RVA: 0x0006A11C File Offset: 0x0006831C
	public static int ActionPointMax
	{
		get
		{
			return TimeManager.BasicGameData.ChallengeModeData.IsEnabled(EChallengeModeImplement.MoreActionPoint) ? GlobalConfig.Instance.MoreActionPointLimitPerMonth : GlobalConfig.Instance.ActionPointLimitPerMonth;
		}
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x0600117F RID: 4479 RVA: 0x0006A147 File Offset: 0x00068347
	public static int ActionPointRecovery
	{
		get
		{
			return TimeManager.BasicGameData.ChallengeModeData.IsEnabled(EChallengeModeImplement.MoreActionPoint) ? GlobalConfig.Instance.MoreActionPointRecoveryPerMonth : GlobalConfig.Instance.ActionPointRecoveryPerMonth;
		}
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0006A174 File Offset: 0x00068374
	public int GetYear()
	{
		return Mathf.CeilToInt(((float)this.CurDate + 0.5f) / 12f);
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0006A1A0 File Offset: 0x000683A0
	public sbyte GetMonthInCurrYear()
	{
		return (sbyte)(this.CurDate % 12);
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0006A1BC File Offset: 0x000683BC
	public sbyte GetMonthInYear(int date)
	{
		int birthYear = (date >= 0) ? (date / 12) : ((date - 11) / 12);
		int birthMonth = date - birthYear * 12;
		return (sbyte)birthMonth;
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0006A1EC File Offset: 0x000683EC
	public int GetYearByDate(int date)
	{
		return Mathf.CeilToInt(((float)date + 0.5f) / 12f);
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0006A214 File Offset: 0x00068414
	public int GetRemainingActionPointConvertToDays()
	{
		return SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / 10;
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0006A234 File Offset: 0x00068434
	public float GetRemainingFloatActionPointConvertToDays()
	{
		return (float)SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / 10f;
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0006A258 File Offset: 0x00068458
	public float ActionPointConvertToDays(int actionPoint)
	{
		return (float)actionPoint / 10f;
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x0006A274 File Offset: 0x00068474
	public bool IsActionPointRunningOut()
	{
		return this.GetRemainingActionPointConvertToDays() <= 0;
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x0006A294 File Offset: 0x00068494
	public bool IsActionDayEnough(int day = 0)
	{
		return this.GetRemainingActionPointConvertToDays() >= day;
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x0006A2B4 File Offset: 0x000684B4
	public bool IsActionPointEnough(int point)
	{
		return SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth >= point;
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x0006A2D8 File Offset: 0x000684D8
	public int GetYearBetweenDate(int dateFrom, int dateTo)
	{
		int date = dateTo - dateFrom;
		bool flag = date < 0;
		if (flag)
		{
			throw new Exception("dateTo must larger than dateFrom!");
		}
		return date / 12;
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0006A308 File Offset: 0x00068508
	public string GetDateDisplayContent(int date)
	{
		int year = SingletonObject.getInstance<TimeManager>().GetYearByDate(date);
		int month = (int)SingletonObject.getInstance<TimeManager>().GetMonthInYear(date);
		return LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year, year) + " " + LocalStringManager.GetFormat(LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Month, month + 1) + " ";
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0006A363 File Offset: 0x00068563
	public static string GetYearDisplayString(int date)
	{
		return (date >= 0) ? LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year.TrFormat(date / 12 + 1) : LanguageKey.UI_AdvanceMonth_TimeChangeInfo_Year_Before.TrFormat(1 - (date + 1) / 12);
	}
}
