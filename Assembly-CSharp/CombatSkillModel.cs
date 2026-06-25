using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.CombatSkill;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class CombatSkillModel : ISingletonInit, IDisposable
{
	// Token: 0x060009EB RID: 2539 RVA: 0x000414D0 File Offset: 0x0003F6D0
	public void Init()
	{
		short templateId = 0;
		while ((int)templateId < CombatSkill.Instance.Count)
		{
			CombatSkillItem configData = CombatSkill.Instance[templateId];
			bool flag = !this.SectCombatSkillDict.ContainsKey(configData.SectId);
			if (flag)
			{
				this.SectCombatSkillDict.Add(configData.SectId, new SortedDictionary<sbyte, List<short>>());
			}
			SortedDictionary<sbyte, List<short>> skillTypeDict = this.SectCombatSkillDict[configData.SectId];
			bool flag2 = !skillTypeDict.ContainsKey(configData.Type);
			if (flag2)
			{
				skillTypeDict.Add(configData.Type, new List<short>());
			}
			skillTypeDict[configData.Type].Add(templateId);
			templateId += 1;
		}
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x000415B9 File Offset: 0x0003F7B9
	public void Dispose()
	{
		this.SectCombatSkillDict = null;
		GameDataBridge.UnregisterListener(this._listenerId);
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x000415E8 File Offset: 0x0003F7E8
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.Type == 1 && notification.DomainId == 7 && (notification.MethodId == 0 || notification.MethodId == 2);
			if (flag)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._responseDisplayData);
				bool flag2 = this._responseDisplayData.Count > 0;
				if (flag2)
				{
					int charId = this._responseDisplayData[0].CharId;
					Dictionary<short, CombatSkillDisplayData> displayDataDict = this.CharacterCombatSkillDisplayDataDict.GetOrNew(charId);
					foreach (CombatSkillDisplayData displayData in this._responseDisplayData)
					{
						displayDataDict.GetOrNew(displayData.TemplateId).Assign(displayData);
					}
					short splitSkillId;
					bool flag3 = this._splitRequestCharacterSkillTemplateIds.TryGetValue(charId, out splitSkillId) && !this._responseDisplayData.Any((CombatSkillDisplayData x) => x.TemplateId == splitSkillId);
					if (!flag3)
					{
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("charId", charId).SetObject("displayDataList", this._responseDisplayData);
						object extraTag;
						bool flag4 = this._requestDisplayDataExtraTagDict.TryGetValue(charId, out extraTag);
						if (flag4)
						{
							argBox.SetObject("extraTag", extraTag);
						}
						GEvent.OnEvent(UiEvents.ReceivedCombatSkillDisplayData, argBox);
						this._splitRequestCharacterSkillTemplateIds.Remove(charId);
						this._requestDisplayDataExtraTagDict.Remove(charId);
					}
				}
			}
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x000417EC File Offset: 0x0003F9EC
	private void OnTaiwuCharIdChange(ArgumentBox argbox)
	{
		int oldTaiwuCharId;
		argbox.Get("OldTaiwuCharId", out oldTaiwuCharId);
		int newTaiwuCharId;
		argbox.Get("NewTaiwuCharId", out newTaiwuCharId);
		this.Clear(oldTaiwuCharId);
		this.Clear(newTaiwuCharId);
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00041828 File Offset: 0x0003FA28
	public bool Contains(int charId, short skillId)
	{
		Dictionary<short, CombatSkillDisplayData> skillDisplayData;
		return this.CharacterCombatSkillDisplayDataDict.TryGetValue(charId, out skillDisplayData) && skillDisplayData.ContainsKey(skillId);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00041854 File Offset: 0x0003FA54
	public bool AllReceived(int charId, IEnumerable<short> skillIdList)
	{
		Dictionary<short, CombatSkillDisplayData> skillDisplayData;
		return this.CharacterCombatSkillDisplayDataDict.TryGetValue(charId, out skillDisplayData) && skillIdList.All((short x) => skillDisplayData.ContainsKey(x));
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00041898 File Offset: 0x0003FA98
	public bool TryGet(int charId, short skillId, out CombatSkillDisplayData displayData)
	{
		displayData = null;
		Dictionary<short, CombatSkillDisplayData> skillDisplayData;
		return this.CharacterCombatSkillDisplayDataDict.TryGetValue(charId, out skillDisplayData) && skillDisplayData.TryGetValue(skillId, out displayData);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x000418C8 File Offset: 0x0003FAC8
	public CombatSkillDisplayData Get(int charId, short skillId)
	{
		return this.CharacterCombatSkillDisplayDataDict[charId][skillId];
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000418EC File Offset: 0x0003FAEC
	public List<CombatSkillDisplayData> GetCache(int charId)
	{
		List<CombatSkillDisplayData> cache = this._characterCombatSkillDisplayDataCache.GetOrNew(charId);
		cache.Clear();
		Dictionary<short, CombatSkillDisplayData> displayDataDict;
		bool flag = this.CharacterCombatSkillDisplayDataDict.TryGetValue(charId, out displayDataDict);
		if (flag)
		{
			cache.AddRange(displayDataDict.Values);
		}
		return cache;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00041932 File Offset: 0x0003FB32
	public void Clear(int charId)
	{
		this.CharacterCombatSkillDisplayDataDict.Remove(charId);
		this._characterCombatSkillDisplayDataCache.Remove(charId);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00041950 File Offset: 0x0003FB50
	public void ClearNotInListSkills(int charId, IList<short> skillList)
	{
		Dictionary<short, CombatSkillDisplayData> dict;
		bool flag = this.CharacterCombatSkillDisplayDataDict.TryGetValue(charId, out dict);
		if (flag)
		{
			List<short> toRemoveKeys = new List<short>();
			foreach (short key in dict.Keys)
			{
				bool flag2 = !skillList.Contains(key);
				if (flag2)
				{
					toRemoveKeys.Add(key);
				}
			}
			foreach (short key2 in toRemoveKeys)
			{
				dict.Remove(key2);
			}
		}
		List<CombatSkillDisplayData> cache;
		bool flag3 = this._characterCombatSkillDisplayDataCache.TryGetValue(charId, out cache);
		if (flag3)
		{
			cache.RemoveAll((CombatSkillDisplayData x) => !skillList.Contains(x.TemplateId));
		}
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00041A5C File Offset: 0x0003FC5C
	public void UpdateAs(CombatSkillDisplayData displayData)
	{
		this.CharacterCombatSkillDisplayDataDict.GetOrNew(displayData.CharId).GetOrNew(displayData.TemplateId).Assign(displayData);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00041A84 File Offset: 0x0003FC84
	public void RequestCombatSkillDisplayData(int charId, IEnumerable<short> skillIdList, object extraTag = null)
	{
		this._requestSkillIds.Clear();
		this._requestSkillIds.AddRange(from x in skillIdList
		where x >= 0
		select x);
		for (int i = 0; i < this._requestSkillIds.Count; i += 100)
		{
			CombatSkillDomainMethod.Call.GetCombatSkillDisplayData(this._listenerId, charId, this._requestSkillIds.GetRange(i, Mathf.Min(100, this._requestSkillIds.Count - i)));
		}
		bool flag = this._requestSkillIds.Count >= 100;
		if (flag)
		{
			Dictionary<int, short> splitRequestCharacterSkillTemplateIds = this._splitRequestCharacterSkillTemplateIds;
			List<short> requestSkillIds = this._requestSkillIds;
			splitRequestCharacterSkillTemplateIds[charId] = requestSkillIds[requestSkillIds.Count - 1];
		}
		bool flag2 = extraTag != null;
		if (flag2)
		{
			this._requestDisplayDataExtraTagDict[charId] = extraTag;
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00041B63 File Offset: 0x0003FD63
	public void RequestCharacterEquipCombatSkillDisplayData(int charId)
	{
		CombatSkillDomainMethod.Call.GetCharacterEquipCombatSkillDisplayData(this._listenerId, charId);
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00041B74 File Offset: 0x0003FD74
	public static void GetCombatSkillDisplayData(int listenerId, int charId, List<short> skillIdList)
	{
		for (int i = 0; i < skillIdList.Count; i += 100)
		{
			CombatSkillDomainMethod.Call.GetCombatSkillDisplayData(listenerId, charId, skillIdList.GetRange(i, Mathf.Min(100, skillIdList.Count - i)));
		}
	}

	// Token: 0x04000CEF RID: 3311
	private int _listenerId;

	// Token: 0x04000CF0 RID: 3312
	public Dictionary<sbyte, SortedDictionary<sbyte, List<short>>> SectCombatSkillDict = new Dictionary<sbyte, SortedDictionary<sbyte, List<short>>>();

	// Token: 0x04000CF1 RID: 3313
	public Dictionary<int, Dictionary<short, CombatSkillDisplayData>> CharacterCombatSkillDisplayDataDict = new Dictionary<int, Dictionary<short, CombatSkillDisplayData>>();

	// Token: 0x04000CF2 RID: 3314
	private readonly Dictionary<int, List<CombatSkillDisplayData>> _characterCombatSkillDisplayDataCache = new Dictionary<int, List<CombatSkillDisplayData>>();

	// Token: 0x04000CF3 RID: 3315
	private readonly Dictionary<int, short> _splitRequestCharacterSkillTemplateIds = new Dictionary<int, short>();

	// Token: 0x04000CF4 RID: 3316
	private readonly Dictionary<int, object> _requestDisplayDataExtraTagDict = new Dictionary<int, object>();

	// Token: 0x04000CF5 RID: 3317
	private readonly List<short> _requestSkillIds = new List<short>();

	// Token: 0x04000CF6 RID: 3318
	private List<CombatSkillDisplayData> _responseDisplayData = new List<CombatSkillDisplayData>();
}
