using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Information;
using GameData.GameDataBridge;
using GameData.Serializer;

// Token: 0x02000105 RID: 261
public class CharacterMonitorModel : ISingletonInit, IDisposable
{
	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000911 RID: 2321 RVA: 0x0003E86B File Offset: 0x0003CA6B
	// (set) Token: 0x06000912 RID: 2322 RVA: 0x0003E873 File Offset: 0x0003CA73
	public int ListenerId { get; private set; } = -1;

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000913 RID: 2323 RVA: 0x0003E87C File Offset: 0x0003CA7C
	// (set) Token: 0x06000914 RID: 2324 RVA: 0x0003E884 File Offset: 0x0003CA84
	public List<NormalInformation> TaiwuReceivedNormalInformationInMonth { get; private set; }

	// Token: 0x06000915 RID: 2325 RVA: 0x0003E890 File Offset: 0x0003CA90
	public void Dispose()
	{
		this.ClearDeadCharacterCache();
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuChange));
		GameDataBridge.AddDataUnMonitor(this.ListenerId, 5, 31, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this.ListenerId, 5, 33, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this.ListenerId, 18, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this.ListenerId, 5, 66, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this.ListenerId, 5, 67, ulong.MaxValue, uint.MaxValue);
		for (int index = 0; index < 3; index++)
		{
			GameDataBridge.AddDataUnMonitor(this.ListenerId, 5, 32, (ulong)((long)index), uint.MaxValue);
		}
		foreach (KeyValuePair<int, CharacterMonitor> pair in this._characterMap)
		{
			pair.Value.ClearMonitor();
		}
		this._characterMap.Clear();
		this.TaiwuReceivedNormalInformationInMonth = null;
		bool flag = this.ListenerId >= 0;
		if (flag)
		{
			GLog.TagWarn(base.GetType().Name, "called UnregisterListener", Array.Empty<object>());
			GameDataBridge.UnregisterListener(this.ListenerId);
		}
		this.ListenerId = -1;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x0003E9E4 File Offset: 0x0003CBE4
	public void Init()
	{
		this._characterMap = new Dictionary<int, CharacterMonitor>();
		this._characterMethodCallQueue = new Queue<ValueTuple<ushort, int>>();
		this._deadCharacterCacheMap = new Dictionary<int, DeadCharacter>();
		this.ListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyCharacterData));
		GameDataBridge.AddDataMonitor(this.ListenerId, 5, 31, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this.ListenerId, 5, 33, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this.ListenerId, 18, 2, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this.ListenerId, 5, 66, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this.ListenerId, 5, 67, ulong.MaxValue, uint.MaxValue);
		for (int index = 0; index < 3; index++)
		{
			GameDataBridge.AddDataMonitor(this.ListenerId, 5, 32, (ulong)((long)index), uint.MaxValue);
		}
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuChange));
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0003EAC4 File Offset: 0x0003CCC4
	public void RemoveUnusedCharacter()
	{
		CharacterSet removeCharacterSet = default(CharacterSet);
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		foreach (KeyValuePair<int, CharacterMonitor> pair in this._characterMap)
		{
			bool flag = taiwuCharId != pair.Key;
			if (flag)
			{
				pair.Value.ClearMonitor();
				removeCharacterSet.Add(pair.Key);
			}
		}
		foreach (int charId in removeCharacterSet.GetCollection())
		{
			this._characterMap.Remove(charId);
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0003EBAC File Offset: 0x0003CDAC
	public void RemoveCharacterMonitor(int charId)
	{
		bool flag = charId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		if (flag)
		{
			this._characterMap.Remove(charId);
		}
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x0003EBDC File Offset: 0x0003CDDC
	public T GetMonitorItem<T>(int charId, bool isDead = false) where T : MonitorDataItemBase, new()
	{
		CharacterMonitor character;
		bool flag = !this._characterMap.TryGetValue(charId, out character);
		if (flag)
		{
			character = new CharacterMonitor(isDead)
			{
				CharacterId = charId,
				Manager = this,
				ListenerId = this.ListenerId
			};
			this._characterMap.Add(charId, character);
		}
		return character.GetMonitorDataItem<T>();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0003EC3C File Offset: 0x0003CE3C
	public void AddDeadCharacterCache(int charId, DeadCharacter deadCharacter)
	{
		bool flag = !this._deadCharacterCacheMap.ContainsKey(charId);
		if (flag)
		{
			this._deadCharacterCacheMap.Add(charId, deadCharacter);
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0003EC6C File Offset: 0x0003CE6C
	public DeadCharacter GetDeadCharacterData(int charId)
	{
		DeadCharacter character;
		this._deadCharacterCacheMap.TryGetValue(charId, out character);
		return character;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0003EC8E File Offset: 0x0003CE8E
	public void ClearDeadCharacterCache()
	{
		this._deadCharacterCacheMap.Clear();
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0003ECA0 File Offset: 0x0003CEA0
	public void RefreshAllMonitorCharacterAliveState()
	{
		foreach (KeyValuePair<int, CharacterMonitor> pair in this._characterMap)
		{
			pair.Value.RefreshAliveState();
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0003ED00 File Offset: 0x0003CF00
	public void RefreshTargetMonitorAliveState(int charId, bool isDead)
	{
		CharacterMonitor characterMonitor;
		bool flag = this._characterMap.TryGetValue(charId, out characterMonitor);
		if (flag)
		{
			characterMonitor.SetDeadState(isDead);
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0003ED2C File Offset: 0x0003CF2C
	public bool HasMonitorCharacter(int charId)
	{
		return this._characterMap.ContainsKey(charId);
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0003ED4A File Offset: 0x0003CF4A
	private void OnTaiwuChange(ArgumentBox box)
	{
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0003ED50 File Offset: 0x0003CF50
	public bool IsTaiwuTeamCharacter(int charId)
	{
		return this._taiwuTeam.Contains(charId);
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0003ED70 File Offset: 0x0003CF70
	public bool IsTaiwuTeamWithSpecialMember(int charId)
	{
		return this.IsTaiwuTeamCharacter(charId) || this.IsTaiwuSpecialTeammate(charId);
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0003ED98 File Offset: 0x0003CF98
	public List<int> GetTaiwuTeamCharIds()
	{
		List<int> charIdList = new List<int>();
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		charIdList.Add(taiwuId);
		foreach (int id in this._taiwuTeam.GetCollection())
		{
			bool flag = id != taiwuId;
			if (flag)
			{
				charIdList.Add(id);
			}
		}
		return charIdList;
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x0003EE24 File Offset: 0x0003D024
	public int GetTaiwuGroupMaxCount()
	{
		return this._taiwuGroupMaxCount;
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0003EE3C File Offset: 0x0003D03C
	public List<int> GetTaiwuCombatTeamCharIds()
	{
		List<int> charIdList = new List<int>();
		charIdList.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		charIdList.AddRange(this._taiwuCombatTeam);
		return charIdList;
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0003EE73 File Offset: 0x0003D073
	public void DoMethodCall(ushort methodId, int charId)
	{
		this._characterMethodCallQueue.Enqueue(new ValueTuple<ushort, int>(methodId, charId));
		GameDataBridge.AddMethodCall<int>(this.ListenerId, 4, methodId, charId);
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0003EE98 File Offset: 0x0003D098
	public void DoMethodCall<T>(ushort methodId, int charId, T arg1)
	{
		this._characterMethodCallQueue.Enqueue(new ValueTuple<ushort, int>(methodId, charId));
		GameDataBridge.AddMethodCall<int, T>(this.ListenerId, 4, methodId, charId, arg1);
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0003EEC0 File Offset: 0x0003D0C0
	private void OnNotifyCharacterData(List<NotificationWrapper> notificationList)
	{
		foreach (NotificationWrapper wrapper in notificationList)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.Type == 0 && notification.Uid.DomainId == 5;
			if (flag)
			{
				bool flag2 = notification.Uid.DataId == 31;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuTeam);
					GEvent.OnEvent(EEvents.TaiwuGroupChange, EasyPool.Get<ArgumentBox>().Set<CharacterSet>("groupCharIds", this._taiwuTeam));
				}
				else
				{
					bool flag3 = notification.Uid.DataId == 33;
					if (flag3)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuGroupMaxCount);
					}
					else
					{
						bool flag4 = notification.Uid.DataId == 32;
						if (flag4)
						{
							int charId = -1;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref charId);
							int index = (int)notification.Uid.SubId0;
							this._taiwuCombatTeam[index] = charId;
							GEvent.OnEvent(UiEvents.CombatTeammateChange, new ArgumentBox().Set("index", index).Set("characterId", charId));
						}
						else
						{
							bool flag5 = notification.Uid.DataId == 66;
							if (flag5)
							{
								List<int> specialGroup = EasyPool.Get<List<int>>();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref specialGroup);
								this._taiwuSpecialGroup.Clear();
								this._taiwuSpecialGroup.UnionWith(specialGroup);
								EasyPool.Free<List<int>>(specialGroup);
							}
							else
							{
								bool flag6 = notification.Uid.DataId == 67;
								if (flag6)
								{
									List<int> gearMateGroup = EasyPool.Get<List<int>>();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref gearMateGroup);
									this._taiwuGearMateGroup.Clear();
									this._taiwuGearMateGroup.UnionWith(gearMateGroup);
									EasyPool.Free<List<int>>(gearMateGroup);
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag7 = notification.Type == 0 && notification.Uid.DomainId == 18;
				if (flag7)
				{
					bool flag8 = notification.Uid.DataId == 2;
					if (flag8)
					{
						List<NormalInformation> list = null;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list);
						this.TaiwuReceivedNormalInformationInMonth = list;
					}
				}
				else
				{
					bool flag9 = (notification.Type == 1 && notification.DomainId == 4) || (notification.Type == 0 && notification.Uid.DomainId == 4);
					if (flag9)
					{
						int charId2 = -1;
						bool flag10 = notification.Type == 0;
						if (flag10)
						{
							charId2 = (int)notification.Uid.SubId0;
						}
						else
						{
							bool flag11 = notification.Type == 1;
							if (flag11)
							{
								bool flag12 = this._characterMethodCallQueue.Count <= 0;
								if (flag12)
								{
									continue;
								}
								ValueTuple<ushort, int> tuple = this._characterMethodCallQueue.Peek();
								bool flag13 = tuple.Item1 != notification.MethodId;
								if (flag13)
								{
									continue;
								}
								charId2 = tuple.Item2;
								this._characterMethodCallQueue.Dequeue();
							}
						}
						CharacterMonitor characterMonitor;
						bool flag14 = this._characterMap.TryGetValue(charId2, out characterMonitor);
						if (flag14)
						{
							characterMonitor.OnNotifyData(wrapper);
						}
					}
				}
			}
		}
		List<int> keys = EasyPool.Get<List<int>>();
		keys.AddRange(this._characterMap.Keys);
		foreach (int charId3 in keys)
		{
			CharacterMonitor characterMonitor2;
			bool flag15 = this._characterMap.TryGetValue(charId3, out characterMonitor2);
			if (flag15)
			{
				characterMonitor2.NotifyDataInit();
			}
		}
		EasyPool.Free<List<int>>(keys);
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0003F2BC File Offset: 0x0003D4BC
	public bool IsTaiwuSpecialTeammate(int charId)
	{
		return this._taiwuSpecialGroup.Contains(charId);
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x0003F2DC File Offset: 0x0003D4DC
	public bool IsTaiwuGearMate(int charId)
	{
		return this._taiwuGearMateGroup.Contains(charId);
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0003F2FC File Offset: 0x0003D4FC
	public bool IsTaiwuBeastTeammate(int charId)
	{
		return this.IsTaiwuSpecialTeammate(charId) && !this.IsTaiwuGearMate(charId);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0003F324 File Offset: 0x0003D524
	public IReadOnlyCollection<int> GetTaiwuSpecialGroup()
	{
		return this._taiwuSpecialGroup;
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0003F33C File Offset: 0x0003D53C
	public IReadOnlyCollection<int> GetTaiwuGearMateGroup()
	{
		return this._taiwuGearMateGroup;
	}

	// Token: 0x04000C0C RID: 3084
	private Dictionary<int, CharacterMonitor> _characterMap;

	// Token: 0x04000C0D RID: 3085
	private Queue<ValueTuple<ushort, int>> _characterMethodCallQueue;

	// Token: 0x04000C0E RID: 3086
	private CharacterSet _taiwuTeam;

	// Token: 0x04000C0F RID: 3087
	private readonly HashSet<int> _taiwuSpecialGroup = new HashSet<int>();

	// Token: 0x04000C10 RID: 3088
	private readonly HashSet<int> _taiwuGearMateGroup = new HashSet<int>();

	// Token: 0x04000C11 RID: 3089
	private int _taiwuGroupMaxCount;

	// Token: 0x04000C12 RID: 3090
	private readonly int[] _taiwuCombatTeam = new int[3];

	// Token: 0x04000C14 RID: 3092
	private Dictionary<int, DeadCharacter> _deadCharacterCacheMap;
}
