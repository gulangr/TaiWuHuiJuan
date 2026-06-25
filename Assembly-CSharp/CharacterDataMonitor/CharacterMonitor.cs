using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006B9 RID: 1721
	public class CharacterMonitor
	{
		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06005106 RID: 20742 RVA: 0x0025C254 File Offset: 0x0025A454
		public ulong CharId
		{
			get
			{
				return (ulong)((long)this.CharacterId);
			}
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06005107 RID: 20743 RVA: 0x0025C25D File Offset: 0x0025A45D
		// (set) Token: 0x06005108 RID: 20744 RVA: 0x0025C265 File Offset: 0x0025A465
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			private set
			{
				this._isDead = value;
			}
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x0025C26F File Offset: 0x0025A46F
		public CharacterMonitor(bool isDead)
		{
			this.IsDead = isDead;
			this._monitorMap = new Dictionary<Type, MonitorDataItemBase>();
		}

		// Token: 0x0600510A RID: 20746 RVA: 0x0025C294 File Offset: 0x0025A494
		public T GetMonitorDataItem<T>() where T : MonitorDataItemBase, new()
		{
			Type t = typeof(T);
			MonitorDataItemBase itemBase;
			bool flag = !this._monitorMap.TryGetValue(t, out itemBase);
			if (flag)
			{
				itemBase = Activator.CreateInstance<T>();
				this._monitorMap.Add(t, itemBase);
				itemBase.Character = this;
			}
			else
			{
				bool flag2 = itemBase is BasicInfoMonitor;
				if (flag2)
				{
					itemBase.Refresh();
				}
			}
			bool isDead = this.IsDead;
			if (isDead)
			{
				bool flag3 = this._deadCharacter == null;
				if (flag3)
				{
					this._deadCharacter = SingletonObject.getInstance<CharacterMonitorModel>().GetDeadCharacterData(this.CharacterId);
				}
				bool flag4 = this._deadCharacter == null;
				if (flag4)
				{
					this.CallMethod(93);
				}
				else
				{
					itemBase.InitFromDeadCharacter(this._deadCharacter);
				}
			}
			return itemBase as T;
		}

		// Token: 0x0600510B RID: 20747 RVA: 0x0025C368 File Offset: 0x0025A568
		public void RemoveMonitorDataItem(Type t)
		{
			MonitorDataItemBase item;
			bool flag = this._monitorMap != null && this._monitorMap.TryGetValue(t, out item) && !item.IsTaiwu;
			if (flag)
			{
				item.UnMonitorData();
				Dictionary<Type, MonitorDataItemBase> monitorMap = this._monitorMap;
				if (monitorMap != null)
				{
					monitorMap.Remove(t);
				}
				bool flag2 = this._monitorMap.Count <= 0;
				if (flag2)
				{
					this.Manager.RemoveCharacterMonitor(this.CharacterId);
				}
			}
		}

		// Token: 0x0600510C RID: 20748 RVA: 0x0025C3E4 File Offset: 0x0025A5E4
		public void ClearMonitor()
		{
			bool flag = this._monitorMap == null;
			if (flag)
			{
				this.CharacterId = -1;
			}
			else
			{
				List<MonitorDataItemBase> list = new List<MonitorDataItemBase>(this._monitorMap.Values);
				foreach (MonitorDataItemBase monitor in list)
				{
					monitor.UnMonitorData();
				}
				this._monitorMap.Clear();
				this.CharacterId = -1;
			}
		}

		// Token: 0x0600510D RID: 20749 RVA: 0x0025C474 File Offset: 0x0025A674
		public void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 1 && wrapper.Notification.MethodId == 93;
			if (flag)
			{
				bool prevDeadState = this.IsDead;
				Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this._deadCharacter);
				this.IsDead = (this._deadCharacter != null);
				bool flag2 = !this.IsDead;
				if (!flag2)
				{
					SingletonObject.getInstance<CharacterMonitorModel>().AddDeadCharacterCache(this.CharacterId, this._deadCharacter);
					List<Type> keysList = EasyPool.Get<List<Type>>();
					keysList.Clear();
					keysList.AddRange(this._monitorMap.Keys);
					int i = 0;
					int max = keysList.Count;
					while (i < max)
					{
						Type key = keysList[i];
						MonitorDataItemBase monitorDataItemBase;
						bool flag3 = !this._monitorMap.TryGetValue(key, out monitorDataItemBase);
						if (!flag3)
						{
							monitorDataItemBase.UnMonitorData();
							bool flag4 = this._monitorMap[key].DataFlag != 2;
							if (flag4)
							{
								this._monitorMap[key].InitFromDeadCharacter(this._deadCharacter);
							}
							this._monitorMap[key].OnDataInit();
						}
						i++;
					}
					EasyPool.Free<List<Type>>(keysList);
				}
			}
			else
			{
				List<Type> keysList = EasyPool.Get<List<Type>>();
				keysList.Clear();
				keysList.AddRange(this._monitorMap.Keys);
				int j = 0;
				int max2 = keysList.Count;
				while (j < max2)
				{
					Type key2 = keysList[j];
					MonitorDataItemBase monitorDataItemBase2;
					bool flag5 = !this._monitorMap.TryGetValue(key2, out monitorDataItemBase2);
					if (!flag5)
					{
						bool flag6;
						if (wrapper.Notification.Type != 1)
						{
							if (wrapper.Notification.Type == 0)
							{
								List<uint> relativeFieldIds = monitorDataItemBase2.RelativeFieldIds;
								flag6 = (relativeFieldIds != null && relativeFieldIds.Contains(wrapper.Notification.Uid.SubId1));
							}
							else
							{
								flag6 = false;
							}
						}
						else
						{
							flag6 = true;
						}
						bool flag7 = flag6;
						if (flag7)
						{
							monitorDataItemBase2.OnNotifyData(wrapper);
						}
					}
					j++;
				}
				EasyPool.Free<List<Type>>(keysList);
			}
		}

		// Token: 0x0600510E RID: 20750 RVA: 0x0025C690 File Offset: 0x0025A890
		public void NotifyDataInit()
		{
			List<MonitorDataItemBase> list = new List<MonitorDataItemBase>(this._monitorMap.Values);
			int i = 0;
			int max = list.Count;
			while (i < max)
			{
				MonitorDataItemBase monitor = list[i];
				bool flag = monitor.DataFlag == 1;
				if (flag)
				{
					monitor.DataFlag = 2;
					monitor.OnDataInit();
				}
				i++;
			}
		}

		// Token: 0x0600510F RID: 20751 RVA: 0x0025C6F4 File Offset: 0x0025A8F4
		public void RefreshAliveState()
		{
			bool isDead = this.IsDead;
			if (!isDead)
			{
				this.CallMethod(93);
			}
		}

		// Token: 0x06005110 RID: 20752 RVA: 0x0025C718 File Offset: 0x0025A918
		public void SetDeadState(bool isDead)
		{
			bool flag = !this.IsDead && isDead;
			if (flag)
			{
				this.RefreshAliveState();
			}
			bool deadToAlive = this.IsDead && !isDead;
			this.IsDead = isDead;
			bool flag2;
			if (deadToAlive)
			{
				Dictionary<Type, MonitorDataItemBase> monitorMap = this._monitorMap;
				flag2 = (monitorMap != null && monitorMap.Count > 0);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				foreach (MonitorDataItemBase monitor in this._monitorMap.Values.ToList<MonitorDataItemBase>())
				{
					if (monitor != null)
					{
						monitor.OnAlive();
					}
				}
			}
		}

		// Token: 0x06005111 RID: 20753 RVA: 0x0025C7D4 File Offset: 0x0025A9D4
		public void CallMethod(ushort methodId)
		{
			this.Manager.DoMethodCall(methodId, this.CharacterId);
		}

		// Token: 0x06005112 RID: 20754 RVA: 0x0025C7EA File Offset: 0x0025A9EA
		public void CallMethod<T>(ushort methodId, T arg1)
		{
			this.Manager.DoMethodCall<T>(methodId, this.CharacterId, arg1);
		}

		// Token: 0x04003794 RID: 14228
		public CharacterMonitorModel Manager;

		// Token: 0x04003795 RID: 14229
		public int ListenerId;

		// Token: 0x04003796 RID: 14230
		public int CharacterId = -1;

		// Token: 0x04003797 RID: 14231
		private DeadCharacter _deadCharacter;

		// Token: 0x04003798 RID: 14232
		public CombatCharacterDisplayData CombatCharacter;

		// Token: 0x04003799 RID: 14233
		private readonly Dictionary<Type, MonitorDataItemBase> _monitorMap;

		// Token: 0x0400379A RID: 14234
		private bool _isDead;
	}
}
