using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Combat.Cricket;
using GameData.Combat.Cricket.SkillsImplement;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC4 RID: 2756
	public class CricketCombatBlackBoard
	{
		// Token: 0x17000EE7 RID: 3815
		// (get) Token: 0x060087C7 RID: 34759 RVA: 0x003F1DBE File Offset: 0x003EFFBE
		private IAsyncMethodRequestHandler Async
		{
			get
			{
				ICricketCombatHandler handler = this._handler;
				return (handler != null) ? handler.Async : null;
			}
		}

		// Token: 0x17000EE8 RID: 3816
		// (get) Token: 0x060087C8 RID: 34760 RVA: 0x003F1DD2 File Offset: 0x003EFFD2
		// (set) Token: 0x060087C9 RID: 34761 RVA: 0x003F1DDA File Offset: 0x003EFFDA
		public CricketCombatConfig Requires { get; private set; }

		// Token: 0x17000EE9 RID: 3817
		// (get) Token: 0x060087CA RID: 34762 RVA: 0x003F1DE3 File Offset: 0x003EFFE3
		// (set) Token: 0x060087CB RID: 34763 RVA: 0x003F1DEB File Offset: 0x003EFFEB
		public ECricketCombatStatus Status { get; private set; }

		// Token: 0x17000EEA RID: 3818
		// (get) Token: 0x060087CC RID: 34764 RVA: 0x003F1DF4 File Offset: 0x003EFFF4
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000EEB RID: 3819
		// (get) Token: 0x060087CD RID: 34765 RVA: 0x003F1E00 File Offset: 0x003F0000
		public CharacterDisplayData SelfChar
		{
			get
			{
				return this._characterData[this.TaiwuCharId];
			}
		}

		// Token: 0x17000EEC RID: 3820
		// (get) Token: 0x060087CE RID: 34766 RVA: 0x003F1E13 File Offset: 0x003F0013
		public CharacterDisplayData EnemyChar
		{
			get
			{
				return this._characterData[this._enemyCharId];
			}
		}

		// Token: 0x17000EED RID: 3821
		// (get) Token: 0x060087CF RID: 34767 RVA: 0x003F1E26 File Offset: 0x003F0026
		// (set) Token: 0x060087D0 RID: 34768 RVA: 0x003F1E2E File Offset: 0x003F002E
		public Wager SelfWager { get; private set; }

		// Token: 0x17000EEE RID: 3822
		// (get) Token: 0x060087D1 RID: 34769 RVA: 0x003F1E37 File Offset: 0x003F0037
		public Wager EnemyWager
		{
			get
			{
				return this._enemyWagerData.Wager;
			}
		}

		// Token: 0x17000EEF RID: 3823
		// (get) Token: 0x060087D2 RID: 34770 RVA: 0x003F1E44 File Offset: 0x003F0044
		public IReadOnlyList<ItemDisplayData> EnemyCrickets
		{
			get
			{
				return this._enemyWagerData.Crickets;
			}
		}

		// Token: 0x17000EF0 RID: 3824
		// (get) Token: 0x060087D3 RID: 34771 RVA: 0x003F1E51 File Offset: 0x003F0051
		// (set) Token: 0x060087D4 RID: 34772 RVA: 0x003F1E59 File Offset: 0x003F0059
		public bool AllowSelectCricket { get; private set; }

		// Token: 0x17000EF1 RID: 3825
		// (get) Token: 0x060087D5 RID: 34773 RVA: 0x003F1E62 File Offset: 0x003F0062
		public IReadOnlyDictionary<int, ItemDisplayData> TaiwuAllowCrickets
		{
			get
			{
				return this._taiwuDisplayData.TaiwuAllowCrickets;
			}
		}

		// Token: 0x17000EF2 RID: 3826
		// (get) Token: 0x060087D6 RID: 34774 RVA: 0x003F1E6F File Offset: 0x003F006F
		public IReadOnlyDictionary<int, CricketData> AllCricketData
		{
			get
			{
				return this._taiwuDisplayData.AllCricketData;
			}
		}

		// Token: 0x17000EF3 RID: 3827
		// (get) Token: 0x060087D7 RID: 34775 RVA: 0x003F1E7C File Offset: 0x003F007C
		// (set) Token: 0x060087D8 RID: 34776 RVA: 0x003F1E84 File Offset: 0x003F0084
		public int CurrentCricketPlanIndex { get; private set; } = -1;

		// Token: 0x17000EF4 RID: 3828
		// (get) Token: 0x060087D9 RID: 34777 RVA: 0x003F1E8D File Offset: 0x003F008D
		// (set) Token: 0x060087DA RID: 34778 RVA: 0x003F1E95 File Offset: 0x003F0095
		public int CurrentMatch { get; private set; }

		// Token: 0x17000EF5 RID: 3829
		// (get) Token: 0x060087DB RID: 34779 RVA: 0x003F1E9E File Offset: 0x003F009E
		// (set) Token: 0x060087DC RID: 34780 RVA: 0x003F1EA6 File Offset: 0x003F00A6
		public int MatchWinCount { get; private set; }

		// Token: 0x17000EF6 RID: 3830
		// (get) Token: 0x060087DD RID: 34781 RVA: 0x003F1EAF File Offset: 0x003F00AF
		// (set) Token: 0x060087DE RID: 34782 RVA: 0x003F1EB7 File Offset: 0x003F00B7
		public bool InvokeExtraWager { get; private set; }

		// Token: 0x17000EF7 RID: 3831
		// (get) Token: 0x060087DF RID: 34783 RVA: 0x003F1EC0 File Offset: 0x003F00C0
		public int MatchLoseCount
		{
			get
			{
				return this.CurrentMatch - this.MatchWinCount;
			}
		}

		// Token: 0x17000EF8 RID: 3832
		// (get) Token: 0x060087E0 RID: 34784 RVA: 0x003F1ECF File Offset: 0x003F00CF
		// (set) Token: 0x060087E1 RID: 34785 RVA: 0x003F1ED7 File Offset: 0x003F00D7
		public CricketCombatDisplayData SelfCricket { get; private set; }

		// Token: 0x17000EF9 RID: 3833
		// (get) Token: 0x060087E2 RID: 34786 RVA: 0x003F1EE0 File Offset: 0x003F00E0
		// (set) Token: 0x060087E3 RID: 34787 RVA: 0x003F1EE8 File Offset: 0x003F00E8
		public CricketCombatDisplayData EnemyCricket { get; private set; }

		// Token: 0x17000EFA RID: 3834
		// (get) Token: 0x060087E4 RID: 34788 RVA: 0x003F1EF1 File Offset: 0x003F00F1
		public CricketViewNew SelfCricketView
		{
			get
			{
				return this.SelfCricketJar.Cricket;
			}
		}

		// Token: 0x17000EFB RID: 3835
		// (get) Token: 0x060087E5 RID: 34789 RVA: 0x003F1EFE File Offset: 0x003F00FE
		public CricketViewNew EnemyCricketView
		{
			get
			{
				return this.EnemyCricketJar.Cricket;
			}
		}

		// Token: 0x060087E6 RID: 34790 RVA: 0x003F1F0C File Offset: 0x003F010C
		public void Initialize(CricketCombatConfig requires, int enemyId, bool doubleDamage, Wager selfWager, CricketWagerData enemyWager)
		{
			this.CurrentMatch = (this.MatchWinCount = 0);
			this.Requires = requires;
			this._enemyCharId = enemyId;
			this._doubleDamage = doubleDamage;
			this.SelfWager = selfWager;
			this._enemyWagerData = enemyWager;
			this.SelfCrickets.Clear();
			this.Status = ECricketCombatStatus.Preparing;
			this.AllowSelectCricket = false;
			this.SelfCricketJar = (this.EnemyCricketJar = null);
			this._matchContext = null;
			this.InvokeExtraWager = false;
			this._selectedCricketPolymorphCharacters.Clear();
		}

		// Token: 0x060087E7 RID: 34791 RVA: 0x003F1F9C File Offset: 0x003F019C
		public void RequestData(ICricketCombatHandler handler)
		{
			this._handler = handler;
			this._handler.OnEvent(ECricketCombatGlobalEventType.RequestData, null);
			List<int> charList = new List<int>
			{
				this.TaiwuCharId,
				this._enemyCharId
			};
			bool flag = this.SelfWager.Type == 2;
			if (flag)
			{
				charList.Add(this.SelfWager.CharId);
			}
			bool flag2 = this.EnemyWager.Type == 2;
			if (flag2)
			{
				charList.Add(this.EnemyWager.CharId);
			}
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this.Async, charList, new AsyncMethodCallbackDelegate(this.HandlerGetCharacterData));
			List<ItemKey> enemyCricketKeys = (from x in this.EnemyCrickets
			select x.Key).ToPoolList<ItemKey>();
			TaiwuDomainMethod.AsyncCall.GetCricketCombatTaiwuDisplayData(this.Async, this.Requires, enemyCricketKeys, new AsyncMethodCallbackDelegate(this.HandlerGetCricketData));
			EasyPool.Free<List<ItemKey>>(enemyCricketKeys);
			this.RequestTeamCricketPolymorphCharacters();
		}

		// Token: 0x060087E8 RID: 34792 RVA: 0x003F20A0 File Offset: 0x003F02A0
		private void RequestTeamCricketPolymorphCharacters()
		{
			bool flag = !CricketPolymorphHelper.IsCricketPolymorphEnabled;
			if (!flag)
			{
				List<int> teamCharIds = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds();
				int taiwuCharId = this.TaiwuCharId;
				teamCharIds.RemoveAll((int id) => id == taiwuCharId || id == this._enemyCharId);
				bool flag2 = teamCharIds.Count == 0;
				if (!flag2)
				{
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this.Async, teamCharIds, new AsyncMethodCallbackDelegate(this.HandlerGetTeamCricketPolymorphCharacters));
				}
			}
		}

		// Token: 0x060087E9 RID: 34793 RVA: 0x003F211C File Offset: 0x003F031C
		private void HandlerGetTeamCricketPolymorphCharacters(int offset, RawDataPool pool)
		{
			List<CharacterDisplayDataForGeneralScrollList> displayDataList = null;
			Serializer.Deserialize(pool, offset, ref displayDataList);
			bool flag = displayDataList == null;
			if (!flag)
			{
				this._teamCricketPolymorphCharacters.Clear();
				foreach (CharacterDisplayDataForGeneralScrollList data in displayDataList)
				{
					bool flag2 = CricketPolymorphHelper.IsCricketPolymorphCharacter(data);
					if (flag2)
					{
						this._teamCricketPolymorphCharacters.Add(data);
					}
				}
			}
		}

		// Token: 0x060087EA RID: 34794 RVA: 0x003F21A4 File Offset: 0x003F03A4
		private void HandlerGetCharacterData(int offset, RawDataPool pool)
		{
			List<CharacterDisplayData> displayDataList = null;
			Serializer.Deserialize(pool, offset, ref displayDataList);
			this._characterData.Clear();
			foreach (CharacterDisplayData data in displayDataList)
			{
				this._characterData.Add(data.CharacterId, data);
			}
			this._handler.OnEvent(ECricketCombatGlobalEventType.CharacterDataReady, null);
		}

		// Token: 0x060087EB RID: 34795 RVA: 0x003F2228 File Offset: 0x003F0428
		private void HandlerGetCricketData(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._taiwuDisplayData);
			this.RefreshTaiwuAllowCricketPresetState();
			this._handler.OnEvent(ECricketCombatGlobalEventType.CricketDataReady, null);
		}

		// Token: 0x060087EC RID: 34796 RVA: 0x003F2250 File Offset: 0x003F0450
		public void UpdateCurrentCricketPlan(int planIndex, List<ItemKey> cricketPlanData)
		{
			this.CurrentCricketPlanIndex = planIndex;
			this._cricketPlanData.Clear();
			bool flag = cricketPlanData != null;
			if (flag)
			{
				this._cricketPlanData.AddRange(cricketPlanData);
			}
			this.RefreshTaiwuAllowCricketPresetState();
		}

		// Token: 0x060087ED RID: 34797 RVA: 0x003F2290 File Offset: 0x003F0490
		public void UpdateCurrentCricketPlanCricket(int cricketIndex, ItemKey cricket)
		{
			bool flag = this.CurrentCricketPlanIndex < 0;
			if (!flag)
			{
				this._cricketPlanData.SetOrAdd(cricketIndex, cricket, ItemKey.Invalid);
				this.RefreshTaiwuAllowCricketPresetState();
			}
		}

		// Token: 0x060087EE RID: 34798 RVA: 0x003F22C8 File Offset: 0x003F04C8
		public CharacterDisplayData GetWagerChar(Wager wager)
		{
			return (wager.Type != 2) ? null : this._characterData.GetOrDefault(wager.CharId);
		}

		// Token: 0x060087EF RID: 34799 RVA: 0x003F22F7 File Offset: 0x003F04F7
		public bool IsShowCricket(int enemyCricketIndex)
		{
			return this._enemyWagerData.IsShowCricket(enemyCricketIndex);
		}

		// Token: 0x060087F0 RID: 34800 RVA: 0x003F2308 File Offset: 0x003F0508
		public void ChangeState(ECricketCombatStatus status)
		{
			bool flag = status == this.Status;
			if (!flag)
			{
				this.Status = status;
				this._handler.OnEvent(ECricketCombatGlobalEventType.CombatStatusChanged, null);
			}
		}

		// Token: 0x060087F1 RID: 34801 RVA: 0x003F233C File Offset: 0x003F053C
		public void ChangeAllowSelectCricket(bool allowSelectCricket)
		{
			bool flag = allowSelectCricket == this.AllowSelectCricket;
			if (!flag)
			{
				this.AllowSelectCricket = allowSelectCricket;
				this._handler.OnEvent(ECricketCombatGlobalEventType.AllowSelectCricketChanged, null);
			}
		}

		// Token: 0x060087F2 RID: 34802 RVA: 0x003F2370 File Offset: 0x003F0570
		public void StartCombat()
		{
			this._handler.OnEvent(ECricketCombatGlobalEventType.MatchPrepare, null);
			CharacterDisplayDataForGeneralScrollList selectedPolymorphCharacter = this.GetSelectedCricketPolymorphCharacter(this.CurrentMatch);
			this.SelfCricket = new CricketCombatDisplayData(this.SelfCrickets[this.CurrentMatch], selectedPolymorphCharacter);
			this.EnemyCricket = new CricketCombatDisplayData(this.EnemyCrickets[this.CurrentMatch]);
			this._handler.OnEvent(ECricketCombatGlobalEventType.SkillResolved, null);
			this._matchContext = new CricketCombatContext(this.SelfCricket.Data, this.EnemyCricket.Data, true, null);
			this._matchContext.DamageMultiplier = (this._doubleDamage ? 2 : 1);
			this._matchWinnerId = (this._matchContext.Simulate() ? this.SelfCricket.Data.RuntimeId : this.EnemyCricket.Data.RuntimeId);
		}

		// Token: 0x060087F3 RID: 34803 RVA: 0x003F2454 File Offset: 0x003F0654
		public CricketCombatLog NextLog()
		{
			CricketCombatContext matchContext = this._matchContext;
			Queue<CricketCombatLog> queue = (matchContext != null) ? matchContext.Logs : null;
			bool flag = queue == null || queue.Count <= 0;
			CricketCombatLog result;
			if (flag)
			{
				result = null;
			}
			else
			{
				CricketCombatLog log = this._matchContext.Logs.Dequeue();
				CricketCombatLogSkillPropertyModify propertyModify = log as CricketCombatLogSkillPropertyModify;
				bool flag2 = propertyModify != null;
				if (flag2)
				{
					this.GetCricket(propertyModify.Target.RuntimeId).Data.AddSkillPropertyModify(propertyModify.Modify);
				}
				bool flag3 = log.Type == ECricketCombatLogEventType.RoundEnd;
				if (flag3)
				{
					this.SelfCricket.Data.ClearRoundPropertyModify();
					this.EnemyCricket.Data.ClearRoundPropertyModify();
				}
				bool flag4 = log.Type == ECricketCombatLogEventType.CombatEnd;
				if (flag4)
				{
					bool invokeExtraWager;
					if (!this.InvokeExtraWager)
					{
						ExtraWager extraWager = this._matchContext.CricketSkillL as ExtraWager;
						invokeExtraWager = (extraWager != null && extraWager.Invoked);
					}
					else
					{
						invokeExtraWager = true;
					}
					this.InvokeExtraWager = invokeExtraWager;
				}
				result = log;
			}
			return result;
		}

		// Token: 0x060087F4 RID: 34804 RVA: 0x003F2554 File Offset: 0x003F0754
		public void EndCombat()
		{
			bool win = this.IsWinner(this.SelfCricket.Data.RuntimeId);
			int selfCricketId = this.SelfCrickets[this.CurrentMatch].Key.Id;
			int enemyCricketId = this.EnemyCrickets[this.CurrentMatch].Key.Id;
			ItemDomainMethod.Call.SetCricketRecord(selfCricketId, win, enemyCricketId);
			ItemDomainMethod.Call.SetCricketRecord(enemyCricketId, !win, selfCricketId);
			int num = this.CurrentMatch;
			this.CurrentMatch = num + 1;
			bool flag = win;
			if (flag)
			{
				num = this.MatchWinCount;
				this.MatchWinCount = num + 1;
			}
			bool flag2 = this.MatchWinCount >= 2 || this.MatchLoseCount >= 2;
			if (flag2)
			{
				this._handler.DoSettlement(this.MatchWinCount >= 2, false);
			}
			else
			{
				this.StartCombat();
			}
		}

		// Token: 0x060087F5 RID: 34805 RVA: 0x003F262F File Offset: 0x003F082F
		public bool IsAlly(long runtimeId)
		{
			return runtimeId == this.SelfCricket.Data.RuntimeId;
		}

		// Token: 0x060087F6 RID: 34806 RVA: 0x003F2644 File Offset: 0x003F0844
		public bool IsWinner(long runtimeId)
		{
			return runtimeId == this._matchWinnerId;
		}

		// Token: 0x060087F7 RID: 34807 RVA: 0x003F264F File Offset: 0x003F084F
		public CricketCombatDisplayData GetOther(CricketCombatDisplayData data)
		{
			return (data == this.SelfCricket) ? this.EnemyCricket : this.SelfCricket;
		}

		// Token: 0x060087F8 RID: 34808 RVA: 0x003F2668 File Offset: 0x003F0868
		public CricketCombatDisplayData GetCricket(long runtimeId)
		{
			return (runtimeId == this.SelfCricket.Data.RuntimeId) ? this.SelfCricket : ((runtimeId == this.EnemyCricket.Data.RuntimeId) ? this.EnemyCricket : null);
		}

		// Token: 0x060087F9 RID: 34809 RVA: 0x003F26A1 File Offset: 0x003F08A1
		public CricketCombatDisplayData GetCricket(bool ally)
		{
			return ally ? this.SelfCricket : this.EnemyCricket;
		}

		// Token: 0x060087FA RID: 34810 RVA: 0x003F26B4 File Offset: 0x003F08B4
		public CricketViewNew GetCricketView(long runtimeId)
		{
			return (runtimeId == this.SelfCricket.Data.RuntimeId) ? this.SelfCricketView : ((runtimeId == this.EnemyCricket.Data.RuntimeId) ? this.EnemyCricketView : null);
		}

		// Token: 0x060087FB RID: 34811 RVA: 0x003F26ED File Offset: 0x003F08ED
		public CricketViewNew GetCricketView(bool ally)
		{
			return ally ? this.SelfCricketView : this.EnemyCricketView;
		}

		// Token: 0x060087FC RID: 34812 RVA: 0x003F2700 File Offset: 0x003F0900
		public ItemDisplayData GetCricketItem(bool ally)
		{
			IReadOnlyList<ItemDisplayData> readOnlyList;
			if (!ally)
			{
				readOnlyList = this.EnemyCrickets;
			}
			else
			{
				IReadOnlyList<ItemDisplayData> selfCrickets = this.SelfCrickets;
				readOnlyList = selfCrickets;
			}
			return readOnlyList[this.CurrentMatch];
		}

		// Token: 0x060087FD RID: 34813 RVA: 0x003F272C File Offset: 0x003F092C
		public int GetSelfCricketOrder(ItemKey itemKey)
		{
			return this.SelfCrickets.FindIndex((ItemDisplayData x) => x != null && x.RealKey.Equals(itemKey));
		}

		// Token: 0x060087FE RID: 34814 RVA: 0x003F2760 File Offset: 0x003F0960
		private void RefreshTaiwuAllowCricketPresetState()
		{
			bool flag;
			if (this.CurrentCricketPlanIndex >= 0)
			{
				CricketCombatTaiwuDisplayData taiwuDisplayData = this._taiwuDisplayData;
				flag = (((taiwuDisplayData != null) ? taiwuDisplayData.TaiwuAllowCrickets : null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				using (Dictionary<int, ItemDisplayData>.ValueCollection.Enumerator enumerator = this._taiwuDisplayData.TaiwuAllowCrickets.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ItemDisplayData cricket = enumerator.Current;
						cricket.IsInCurrentCricketPreset = this._cricketPlanData.Exists((ItemKey key) => key.IsValid() && key.Equals(cricket.RealKey));
					}
				}
			}
		}

		// Token: 0x060087FF RID: 34815 RVA: 0x003F2810 File Offset: 0x003F0A10
		public List<CharacterDisplayDataForGeneralScrollList> GetAvailableCricketPolymorphCharacters(int jarIndex)
		{
			List<CharacterDisplayDataForGeneralScrollList> result = new List<CharacterDisplayDataForGeneralScrollList>();
			foreach (CharacterDisplayDataForGeneralScrollList ch in this._teamCricketPolymorphCharacters)
			{
				int usedByJar = this.GetCricketPolymorphCharacterJarIndex(ch.CharacterId);
				bool flag = usedByJar < 0 || usedByJar == jarIndex;
				if (flag)
				{
					result.Add(ch);
				}
			}
			return result;
		}

		// Token: 0x06008800 RID: 34816 RVA: 0x003F2894 File Offset: 0x003F0A94
		public IReadOnlyList<CharacterDisplayDataForGeneralScrollList> GetAllCricketPolymorphCharacters()
		{
			return this._teamCricketPolymorphCharacters;
		}

		// Token: 0x06008801 RID: 34817 RVA: 0x003F289C File Offset: 0x003F0A9C
		public void SelectCricketPolymorphCharacter(int jarIndex, int characterId)
		{
			CharacterDisplayDataForGeneralScrollList ch = this._teamCricketPolymorphCharacters.Find((CharacterDisplayDataForGeneralScrollList x) => x.CharacterId == characterId);
			bool flag = ch == null;
			if (!flag)
			{
				this._selectedCricketPolymorphCharacters[jarIndex] = ch;
			}
		}

		// Token: 0x06008802 RID: 34818 RVA: 0x003F28E7 File Offset: 0x003F0AE7
		public void DeselectCricketPolymorphCharacter(int jarIndex)
		{
			this._selectedCricketPolymorphCharacters.Remove(jarIndex);
		}

		// Token: 0x06008803 RID: 34819 RVA: 0x003F28F8 File Offset: 0x003F0AF8
		public CharacterDisplayDataForGeneralScrollList GetSelectedCricketPolymorphCharacter(int jarIndex)
		{
			return this._selectedCricketPolymorphCharacters.GetOrDefault(jarIndex);
		}

		// Token: 0x06008804 RID: 34820 RVA: 0x003F2918 File Offset: 0x003F0B18
		public int GetCricketPolymorphCharacterJarIndex(int characterId)
		{
			foreach (KeyValuePair<int, CharacterDisplayDataForGeneralScrollList> kv in this._selectedCricketPolymorphCharacters)
			{
				bool flag = kv.Value.CharacterId == characterId;
				if (flag)
				{
					return kv.Key;
				}
			}
			return -1;
		}

		// Token: 0x04006831 RID: 26673
		private ICricketCombatHandler _handler;

		// Token: 0x04006834 RID: 26676
		private int _enemyCharId;

		// Token: 0x04006835 RID: 26677
		private readonly Dictionary<int, CharacterDisplayData> _characterData = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x04006836 RID: 26678
		private CricketWagerData _enemyWagerData;

		// Token: 0x04006837 RID: 26679
		private bool _doubleDamage;

		// Token: 0x04006839 RID: 26681
		public readonly List<ItemDisplayData> SelfCrickets = new List<ItemDisplayData>();

		// Token: 0x0400683B RID: 26683
		private CricketCombatTaiwuDisplayData _taiwuDisplayData;

		// Token: 0x0400683C RID: 26684
		private List<CharacterDisplayDataForGeneralScrollList> _teamCricketPolymorphCharacters = new List<CharacterDisplayDataForGeneralScrollList>();

		// Token: 0x0400683D RID: 26685
		private readonly Dictionary<int, CharacterDisplayDataForGeneralScrollList> _selectedCricketPolymorphCharacters = new Dictionary<int, CharacterDisplayDataForGeneralScrollList>();

		// Token: 0x0400683F RID: 26687
		private readonly List<ItemKey> _cricketPlanData = new List<ItemKey>();

		// Token: 0x04006843 RID: 26691
		private CricketCombatContext _matchContext;

		// Token: 0x04006844 RID: 26692
		private long _matchWinnerId;

		// Token: 0x04006847 RID: 26695
		public CricketJar SelfCricketJar;

		// Token: 0x04006848 RID: 26696
		public CricketJar EnemyCricketJar;
	}
}
