using System;

namespace Game.Views.Exchange
{
	// Token: 0x02000A2D RID: 2605
	public interface IShopRefresh
	{
		// Token: 0x17000DC7 RID: 3527
		// (get) Token: 0x06007F87 RID: 32647 RVA: 0x003B621F File Offset: 0x003B441F
		bool CanShow
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000DC8 RID: 3528
		// (get) Token: 0x06007F88 RID: 32648 RVA: 0x003B6222 File Offset: 0x003B4422
		bool Protected
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000DC9 RID: 3529
		// (get) Token: 0x06007F89 RID: 32649 RVA: 0x003B6225 File Offset: 0x003B4425
		bool CanRefreshCurrentGoods
		{
			get
			{
				return SingletonObject.getInstance<TimeManager>().IsActionPointEnough(GlobalConfig.Instance.RefreshItemApCost);
			}
		}

		// Token: 0x17000DCA RID: 3530
		// (get) Token: 0x06007F8A RID: 32650 RVA: 0x003B623B File Offset: 0x003B443B
		int RefreshCount
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / Math.Max(GlobalConfig.Instance.RefreshItemApCost, 1);
			}
		}

		// Token: 0x17000DCB RID: 3531
		// (get) Token: 0x06007F8B RID: 32651 RVA: 0x003B6258 File Offset: 0x003B4458
		string RefreshTipTitle
		{
			get
			{
				return this.NameId.Tr();
			}
		}

		// Token: 0x17000DCC RID: 3532
		// (get) Token: 0x06007F8C RID: 32652
		LanguageKey NameId { get; }

		// Token: 0x17000DCD RID: 3533
		// (get) Token: 0x06007F8D RID: 32653
		LanguageKey DescId { get; }

		// Token: 0x17000DCE RID: 3534
		// (get) Token: 0x06007F8E RID: 32654
		LanguageKey BreakId { get; }

		// Token: 0x17000DCF RID: 3535
		// (get) Token: 0x06007F8F RID: 32655
		LanguageKey NoTimeId { get; }

		// Token: 0x17000DD0 RID: 3536
		// (get) Token: 0x06007F90 RID: 32656
		LanguageKey NeedClearId { get; }

		// Token: 0x06007F91 RID: 32657 RVA: 0x003B6268 File Offset: 0x003B4468
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

		// Token: 0x06007F92 RID: 32658
		void InitShopRefresh(Action refreshCount, Action<bool> refreshActiveStates, Action<bool> refreshTips);

		// Token: 0x06007F93 RID: 32659
		void RefreshCurrentGoods();

		// Token: 0x06007F94 RID: 32660 RVA: 0x003B6348 File Offset: 0x003B4548
		void NoticeCannotRefreshCurrentGoods()
		{
		}
	}
}
