using System;

// Token: 0x02000301 RID: 769
public interface IShopRefresh
{
	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x06002D02 RID: 11522 RVA: 0x00161CDC File Offset: 0x0015FEDC
	bool CanShow
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x06002D03 RID: 11523 RVA: 0x00161CDF File Offset: 0x0015FEDF
	bool Protected
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x06002D04 RID: 11524 RVA: 0x00161CE2 File Offset: 0x0015FEE2
	bool CanRefreshCurrentGoods
	{
		get
		{
			return SingletonObject.getInstance<TimeManager>().IsActionPointEnough(GlobalConfig.Instance.RefreshItemApCost);
		}
	}

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x06002D05 RID: 11525 RVA: 0x00161CF8 File Offset: 0x0015FEF8
	int RefreshCount
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / Math.Max(GlobalConfig.Instance.RefreshItemApCost, 1);
		}
	}

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x06002D06 RID: 11526 RVA: 0x00161D15 File Offset: 0x0015FF15
	string RefreshTipTitle
	{
		get
		{
			return LocalStringManager.Get(this.NameId);
		}
	}

	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x06002D07 RID: 11527
	LanguageKey NameId { get; }

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x06002D08 RID: 11528
	LanguageKey DescId { get; }

	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x06002D09 RID: 11529
	LanguageKey BreakId { get; }

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x06002D0A RID: 11530
	LanguageKey NoTimeId { get; }

	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x06002D0B RID: 11531
	LanguageKey NeedClearId { get; }

	// Token: 0x06002D0C RID: 11532 RVA: 0x00161D24 File Offset: 0x0015FF24
	string RefreshTips(bool needClear)
	{
		bool noTime = !SingletonObject.getInstance<TimeManager>().IsActionPointEnough(GlobalConfig.Instance.RefreshItemApCost);
		return LocalStringManager.GetFormat(this.DescId, new object[]
		{
			SingletonObject.getInstance<TimeManager>().IsActionPointEnough(GlobalConfig.Instance.RefreshItemApCost) ? "brightblue" : "brightred",
			SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / 10,
			GlobalConfig.Instance.RefreshItemApCost / 10,
			(needClear || noTime) ? LocalStringManager.Get(this.BreakId) : "",
			noTime ? LocalStringManager.Get(this.NoTimeId) : "",
			needClear ? LocalStringManager.Get(this.NeedClearId) : ""
		}).Trim('\n').ColorReplace();
	}

	// Token: 0x06002D0D RID: 11533
	void InitShopRefresh(Action refreshCount, Action<bool> refreshActiveStates, Action<bool> refreshTips);

	// Token: 0x06002D0E RID: 11534
	void RefreshCurrentGoods();

	// Token: 0x06002D0F RID: 11535 RVA: 0x00161E04 File Offset: 0x00160004
	void NoticeCannotRefreshCurrentGoods()
	{
	}
}
