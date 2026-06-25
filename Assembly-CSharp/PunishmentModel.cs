using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Organization.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x02000140 RID: 320
public class PunishmentModel : ISingletonInit, IDisposable
{
	// Token: 0x170001DD RID: 477
	// (get) Token: 0x0600110D RID: 4365 RVA: 0x00066B03 File Offset: 0x00064D03
	// (set) Token: 0x0600110E RID: 4366 RVA: 0x00066B0B File Offset: 0x00064D0B
	public int ListenerId { get; private set; } = -1;

	// Token: 0x0600110F RID: 4367 RVA: 0x00066B14 File Offset: 0x00064D14
	public void Init()
	{
		this.ListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyCharacterData));
		GameDataBridge.AddDataMonitor(this.ListenerId, 3, 15, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x00066B44 File Offset: 0x00064D44
	private void ResetConfigData()
	{
		this._sectPunishmentMap.Clear();
		this._statePunishmentMap.Clear();
		foreach (PunishmentTypeItem config in ((IEnumerable<PunishmentTypeItem>)PunishmentType.Instance))
		{
			foreach (ShortPair pair in config.SectPunishmentSeverities)
			{
				bool flag = !this._sectPunishmentMap.ContainsKey(pair.First);
				if (flag)
				{
					this._sectPunishmentMap.Add(pair.First, new List<ShortPair>());
				}
				this._sectPunishmentMap[pair.First].Add(new ShortPair(config.TemplateId, pair.Second));
			}
			foreach (ShortPair pair2 in config.CivilianPunishmentSeverities)
			{
				bool flag2 = !this._statePunishmentMap.ContainsKey(pair2.First);
				if (flag2)
				{
					this._statePunishmentMap.Add(pair2.First, new List<ShortPair>());
				}
				this._statePunishmentMap[pair2.First].Add(new ShortPair(config.TemplateId, pair2.Second));
			}
		}
		foreach (List<ShortPair> list in this._sectPunishmentMap.Values)
		{
			list.Sort(new Comparison<ShortPair>(this.ComparePunishment));
		}
		foreach (List<ShortPair> list2 in this._statePunishmentMap.Values)
		{
			list2.Sort(new Comparison<ShortPair>(this.ComparePunishment));
		}
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x00066DD0 File Offset: 0x00064FD0
	public void Dispose()
	{
		this._sectPunishmentMap.Clear();
		this._statePunishmentMap.Clear();
		GameDataBridge.AddDataUnMonitor(this.ListenerId, 3, 15, ulong.MaxValue, uint.MaxValue);
		bool flag = this.ListenerId >= 0;
		if (flag)
		{
			GLog.TagWarn(base.GetType().Name, "called UnregisterListener", Array.Empty<object>());
			GameDataBridge.UnregisterListener(this.ListenerId);
		}
		this.ListenerId = -1;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x00066E4C File Offset: 0x0006504C
	public sbyte GetOriginalPunishmentSeverity(sbyte stateId, short punishmentTypeTemplateId, bool isSect)
	{
		PunishmentTypeItem typeConfig = PunishmentType.Instance[punishmentTypeTemplateId];
		return typeConfig.GetSeverity(stateId, isSect, true);
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x00066E74 File Offset: 0x00065074
	private void OnNotifyCharacterData(List<NotificationWrapper> notificationList)
	{
		foreach (NotificationWrapper wrapper in notificationList)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.Type == 0 && notification.Uid.DomainId == 3;
			if (flag)
			{
				bool flag2 = notification.Uid.DataId == 15;
				if (flag2)
				{
					Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._cityPunishmentSeverityCustomizeDict);
					this.ResetConfigData();
					this.UpdateStatePunishmentMap();
				}
			}
		}
		foreach (List<ShortPair> list in this._statePunishmentMap.Values)
		{
			list.Sort(new Comparison<ShortPair>(this.ComparePunishment));
		}
		foreach (List<ShortPair> list2 in this._sectPunishmentMap.Values)
		{
			list2.Sort(new Comparison<ShortPair>(this.ComparePunishment));
		}
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x00066FD4 File Offset: 0x000651D4
	private void UpdateStatePunishmentMap()
	{
		foreach (KeyValuePair<short, SerializableList<PunishmentSeverityCustomizeData>> keyValuePair in this._cityPunishmentSeverityCustomizeDict)
		{
			short num;
			SerializableList<PunishmentSeverityCustomizeData> serializableList;
			keyValuePair.Deconstruct(out num, out serializableList);
			short key = num;
			SerializableList<PunishmentSeverityCustomizeData> customizeDataList = serializableList;
			ValueTuple<sbyte, bool> keyData = PunishmentSeverityCustomizeData.DecodePunishmentSeverityCustomizeKey(key);
			Dictionary<short, List<ShortPair>> map = this.GetPunishmentMap(keyData.Item2);
			List<ShortPair> punishments;
			bool flag = map.TryGetValue((short)keyData.Item1, out punishments);
			if (flag)
			{
				PunishmentModel.<UpdateStatePunishmentMap>g__SyncLawModifications|12_0(punishments, customizeDataList);
			}
		}
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00067070 File Offset: 0x00065270
	private int ComparePunishment(ShortPair a, ShortPair b)
	{
		return b.Second.CompareTo(a.Second);
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00067094 File Offset: 0x00065294
	public Dictionary<short, List<ShortPair>> GetPunishmentMap(bool isSect)
	{
		return isSect ? this._sectPunishmentMap : this._statePunishmentMap;
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x000670A8 File Offset: 0x000652A8
	public List<PunishmentSeverityCustomizeData> GetCustomizeDataList(sbyte stateTemplateId, bool isSect)
	{
		short key = PunishmentSeverityCustomizeData.GetPunishmentSeverityCustomizeKey(stateTemplateId, isSect);
		SerializableList<PunishmentSeverityCustomizeData> customizeDataList;
		bool flag = this._cityPunishmentSeverityCustomizeDict.TryGetValue(key, out customizeDataList);
		List<PunishmentSeverityCustomizeData> result;
		if (flag)
		{
			result = customizeDataList.Items;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x00067110 File Offset: 0x00065310
	[CompilerGenerated]
	internal static void <UpdateStatePunishmentMap>g__SyncLawModifications|12_0(List<ShortPair> shortPairList, SerializableList<PunishmentSeverityCustomizeData> customizeDataList)
	{
		bool flag = customizeDataList.Items == null;
		if (!flag)
		{
			for (int i = 0; i < shortPairList.Count; i++)
			{
				ShortPair data = shortPairList[i];
				for (int j = 0; j < customizeDataList.Items.Count; j++)
				{
					PunishmentSeverityCustomizeData customizeData = customizeDataList.Items[j];
					bool flag2 = customizeData.PunishmentTypeTemplateId != data.First;
					if (!flag2)
					{
						shortPairList[i] = new ShortPair(data.First, (short)customizeData.CustomizedPunishmentSeverityTemplateId);
						break;
					}
				}
			}
		}
	}

	// Token: 0x04000F02 RID: 3842
	private readonly Dictionary<short, List<ShortPair>> _sectPunishmentMap = new Dictionary<short, List<ShortPair>>();

	// Token: 0x04000F03 RID: 3843
	private readonly Dictionary<short, List<ShortPair>> _statePunishmentMap = new Dictionary<short, List<ShortPair>>();

	// Token: 0x04000F04 RID: 3844
	private readonly Dictionary<short, SerializableList<PunishmentSeverityCustomizeData>> _cityPunishmentSeverityCustomizeDict = new Dictionary<short, SerializableList<PunishmentSeverityCustomizeData>>();
}
