using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.SpecialEffect;
using GameData.Domains.Story;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class CombatModel : ISingletonInit, IDisposable
{
	// Token: 0x06000931 RID: 2353 RVA: 0x0003F480 File Offset: 0x0003D680
	public void Init()
	{
		this._handlerData = new List<CombatNotifyHandler>(CombatNotifyHelper.ParseHandlerData<CombatModel>(this));
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyData));
		this._dispatcher = DispatcherUtils.RegisterDispatcher();
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x0003F4B6 File Offset: 0x0003D6B6
	public void Dispose()
	{
		this.UnMonitorAllData();
		GameDataBridge.UnregisterListener(this._listenerId);
		this._listenerId = -1;
		DispatcherUtils.UnregisterDispatcher(this._dispatcher);
		this._dispatcher = null;
		this._handlerData = null;
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000933 RID: 2355 RVA: 0x0003F4ED File Offset: 0x0003D6ED
	public CombatConfigItem Config
	{
		get
		{
			return CombatConfig.Instance[this._configId];
		}
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000934 RID: 2356 RVA: 0x0003F4FF File Offset: 0x0003D6FF
	public CombatSceneItem Scene
	{
		get
		{
			return global::Config.CombatScene.Instance[this._sceneId];
		}
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000935 RID: 2357 RVA: 0x0003F511 File Offset: 0x0003D711
	public IReadOnlyList<int> SelfTeam
	{
		get
		{
			return this._selfTeam;
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000936 RID: 2358 RVA: 0x0003F519 File Offset: 0x0003D719
	public IReadOnlyList<int> EnemyTeam
	{
		get
		{
			return this._enemyTeam;
		}
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000937 RID: 2359 RVA: 0x0003F521 File Offset: 0x0003D721
	public bool IsAllyMainChar
	{
		get
		{
			return this._selfTeam.Count > 0 && this.SelfCharId == this._selfTeam[0];
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000938 RID: 2360 RVA: 0x0003F548 File Offset: 0x0003D748
	public bool IsEnemyMainChar
	{
		get
		{
			return this._enemyTeam.Count > 0 && this.EnemyCharId == this._enemyTeam[0];
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000939 RID: 2361 RVA: 0x0003F56F File Offset: 0x0003D76F
	public CombatSubProcessorCharacter SelfCharacter
	{
		get
		{
			return this.ProcessorCharacters.GetValueOrDefault(this.SelfCharId);
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x0600093A RID: 2362 RVA: 0x0003F582 File Offset: 0x0003D782
	public CombatSubProcessorCharacter EnemyCharacter
	{
		get
		{
			return this.ProcessorCharacters.GetValueOrDefault(this.EnemyCharId);
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x0600093B RID: 2363 RVA: 0x0003F595 File Offset: 0x0003D795
	public IReadOnlyDictionary<int, CharacterDisplayData> DisplayDataCache
	{
		get
		{
			return this._displayDataCache;
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x0600093C RID: 2364 RVA: 0x0003F59D File Offset: 0x0003D79D
	public IReadOnlyDictionary<int, List<ItemDisplayData>> EquipmentDataCache
	{
		get
		{
			return this._equipmentDataCache;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x0600093D RID: 2365 RVA: 0x0003F5A5 File Offset: 0x0003D7A5
	public IReadOnlyDictionary<int, IReadOnlyList<CombatSkillDisplayData>> ProactiveSkillData
	{
		get
		{
			return this._proactiveSkillData;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x0600093E RID: 2366 RVA: 0x0003F5AD File Offset: 0x0003D7AD
	public IReadOnlyDictionary<int, List<short>> OrderedProactiveSkillList
	{
		get
		{
			return this._orderedProactiveSkillList;
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x0600093F RID: 2367 RVA: 0x0003F5B5 File Offset: 0x0003D7B5
	public CombatSkillDisplayData PreviewCostSkillData
	{
		get
		{
			return this._previewCostSkillData;
		}
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000940 RID: 2368 RVA: 0x0003F5BD File Offset: 0x0003D7BD
	public IReadOnlyList<CastBoostEffectDisplayData> CostNeiliEffects
	{
		get
		{
			return this._costNeiliEffects;
		}
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000941 RID: 2369 RVA: 0x0003F5C5 File Offset: 0x0003D7C5
	public bool ShowSkillCostPreview
	{
		get
		{
			return this.PreviewCostSkill >= 0 && this.PreviewCostSkillCanUse && this.PreviewCostSkillData != null && this.PreviewCostSkillData.TemplateId == this.PreviewCostSkill;
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000942 RID: 2370 RVA: 0x0003F5F6 File Offset: 0x0003D7F6
	public IReadOnlyDictionary<CombatSkillKey, CombatSubProcessorSkill> ProcessorSkills
	{
		get
		{
			return this._subProcessorSkills;
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000943 RID: 2371 RVA: 0x0003F5FE File Offset: 0x0003D7FE
	public IReadOnlyDictionary<ItemKey, CombatSubProcessorWeapon> ProcessorWeapons
	{
		get
		{
			return this._subProcessorWeapons;
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000944 RID: 2372 RVA: 0x0003F606 File Offset: 0x0003D806
	public IReadOnlyDictionary<int, CombatSubProcessorCharacter> ProcessorCharacters
	{
		get
		{
			return this._subProcessorCharacters;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000945 RID: 2373 RVA: 0x0003F60E File Offset: 0x0003D80E
	// (set) Token: 0x06000946 RID: 2374 RVA: 0x0003F616 File Offset: 0x0003D816
	public CombatSubProcessorTaiwu ProcessorTaiwu { get; private set; }

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000947 RID: 2375 RVA: 0x0003F61F File Offset: 0x0003D81F
	// (set) Token: 0x06000948 RID: 2376 RVA: 0x0003F627 File Offset: 0x0003D827
	public CombatSubProcessorCharacterDisplay ProcessorAnimalChar { get; private set; }

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000949 RID: 2377 RVA: 0x0003F630 File Offset: 0x0003D830
	// (set) Token: 0x0600094A RID: 2378 RVA: 0x0003F638 File Offset: 0x0003D838
	public CombatSubProcessorCharacterDisplay ProcessorSpecialChar { get; private set; }

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x0600094B RID: 2379 RVA: 0x0003F641 File Offset: 0x0003D841
	public bool IsCombatOver
	{
		get
		{
			return CombatStatusType.IsCombatOver(this.CombatStatus);
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x0600094C RID: 2380 RVA: 0x0003F64E File Offset: 0x0003D84E
	public EShowMercyOption ShowingMercyOption
	{
		get
		{
			return (EShowMercyOption)this._showMercyOption;
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x0600094D RID: 2381 RVA: 0x0003F656 File Offset: 0x0003D856
	public bool ShowingMercy
	{
		get
		{
			return this.ShowingMercyOption > EShowMercyOption.Invalid;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x0600094E RID: 2382 RVA: 0x0003F664 File Offset: 0x0003D864
	public bool ShowingMercyIsAlly
	{
		get
		{
			EShowMercyOption showingMercyOption = this.ShowingMercyOption;
			return showingMercyOption == EShowMercyOption.PlayerShowMercy || showingMercyOption == EShowMercyOption.FuyuSword;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x0600094F RID: 2383 RVA: 0x0003F689 File Offset: 0x0003D889
	public bool NotExecuted
	{
		get
		{
			return this._selectedMercyOption <= 0;
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000950 RID: 2384 RVA: 0x0003F698 File Offset: 0x0003D898
	public bool IsTutorialCombat
	{
		get
		{
			CombatSubProcessorCharacter processor;
			return this._subProcessorCharacters.TryGetValue(this.SelfCharId, out processor) && processor.TemplateId == 908;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000951 RID: 2385 RVA: 0x0003F6CA File Offset: 0x0003D8CA
	public bool SelfChangingTrick
	{
		get
		{
			CombatSubProcessorCharacter selfCharacter = this.SelfCharacter;
			return selfCharacter != null && selfCharacter.ChangingTrick;
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000952 RID: 2386 RVA: 0x0003F6DE File Offset: 0x0003D8DE
	// (set) Token: 0x06000953 RID: 2387 RVA: 0x0003F6E6 File Offset: 0x0003D8E6
	public bool IsPausing { get; private set; }

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000954 RID: 2388 RVA: 0x0003F6EF File Offset: 0x0003D8EF
	public bool TaiwuInCombat
	{
		get
		{
			return this.TaiwuCharId == this.MainCharId(true);
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000955 RID: 2389 RVA: 0x0003F700 File Offset: 0x0003D900
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000956 RID: 2390 RVA: 0x0003F70C File Offset: 0x0003D90C
	public bool CanCostTrickDuringPreparingSkill
	{
		get
		{
			return this._canCostTrickDuringPreparingSkill;
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000957 RID: 2391 RVA: 0x0003F714 File Offset: 0x0003D914
	public bool CanOperateSelfCharacter
	{
		get
		{
			return this.CanOperateCharacter(this.SelfCharId);
		}
	}

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000958 RID: 2392 RVA: 0x0003F724 File Offset: 0x0003D924
	// (remove) Token: 0x06000959 RID: 2393 RVA: 0x0003F75C File Offset: 0x0003D95C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event OnSpecialMiscEvent OnShowUseGoldenWireChanged;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x0600095A RID: 2394 RVA: 0x0003F794 File Offset: 0x0003D994
	// (remove) Token: 0x0600095B RID: 2395 RVA: 0x0003F7CC File Offset: 0x0003D9CC
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event OnGetProactiveSkillListEvent OnGetProactiveSkillList;

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x0600095C RID: 2396 RVA: 0x0003F801 File Offset: 0x0003DA01
	// (set) Token: 0x0600095D RID: 2397 RVA: 0x0003F809 File Offset: 0x0003DA09
	public int ChangingFromCharId { get; private set; } = -1;

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x0600095E RID: 2398 RVA: 0x0003F812 File Offset: 0x0003DA12
	// (set) Token: 0x0600095F RID: 2399 RVA: 0x0003F81A File Offset: 0x0003DA1A
	public int ChangingToCharId { get; private set; } = -1;

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000960 RID: 2400 RVA: 0x0003F823 File Offset: 0x0003DA23
	// (set) Token: 0x06000961 RID: 2401 RVA: 0x0003F82B File Offset: 0x0003DA2B
	public short PreviewCostSkill { get; private set; }

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000962 RID: 2402 RVA: 0x0003F834 File Offset: 0x0003DA34
	// (set) Token: 0x06000963 RID: 2403 RVA: 0x0003F83C File Offset: 0x0003DA3C
	public bool PreviewCostSkillCanUse { get; private set; }

	// Token: 0x06000964 RID: 2404 RVA: 0x0003F848 File Offset: 0x0003DA48
	public void RaiseEvent(ECombatEvents eventType)
	{
		this._raisingEvent = true;
		List<OnCombatEvent> handlers;
		bool flag = this._combatEvents.TryGetValue(eventType, out handlers);
		if (flag)
		{
			foreach (OnCombatEvent handler in handlers)
			{
				handler();
			}
		}
		this._raisingEvent = false;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0003F8BC File Offset: 0x0003DABC
	public void AddEvent(ECombatEvents eventType, OnCombatEvent handler)
	{
		bool raisingEvent = this._raisingEvent;
		if (raisingEvent)
		{
			Debug.LogError(string.Format("AddEvent is not allowed in RaiseEvent, {0}", eventType));
		}
		else
		{
			this._combatEvents.GetOrNew(eventType).Add(handler);
		}
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0003F900 File Offset: 0x0003DB00
	public void RemoveEvent(ECombatEvents eventType, OnCombatEvent handler)
	{
		bool raisingEvent = this._raisingEvent;
		if (raisingEvent)
		{
			Debug.LogError(string.Format("AddEvent is not allowed in RaiseEvent, {0}", eventType));
		}
		else
		{
			List<OnCombatEvent> handlers;
			bool flag = this._combatEvents.TryGetValue(eventType, out handlers);
			if (flag)
			{
				handlers.Remove(handler);
			}
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0003F94C File Offset: 0x0003DB4C
	public void AddSubProcessor(CombatSubProcessor subProcessor)
	{
		bool flag = this._subProcessors.Contains(subProcessor);
		if (!flag)
		{
			foreach (DataUid uid in subProcessor.GetProcessorDataUids())
			{
				this.MonitorData(uid);
			}
			this._subProcessors.Add(subProcessor);
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0003F9BC File Offset: 0x0003DBBC
	public void RemoveSubProcessor(CombatSubProcessor subProcessor)
	{
		bool flag = !this._subProcessors.Contains(subProcessor);
		if (!flag)
		{
			foreach (DataUid uid in subProcessor.GetProcessorDataUids())
			{
				this.UnMonitorData(uid);
			}
			this._subProcessors.Remove(subProcessor);
		}
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0003FA30 File Offset: 0x0003DC30
	public void UpdateSubProcessors()
	{
		this.UpdateSubProcessorSkills();
		this.UpdateSubProcessorWeapons();
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0003FA44 File Offset: 0x0003DC44
	private void UpdateSubProcessorSkills()
	{
		foreach (CombatSubProcessorCharacter processor in this.ProcessorCharacters.Values)
		{
			foreach (short id in processor.IterCombatSkillIds())
			{
				this.UpdateSubProcessorSkill(processor.CharacterId, id);
			}
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0003FAD8 File Offset: 0x0003DCD8
	private void UpdateSubProcessorSkill(int charId, short skillTemplateId)
	{
		CombatSkillKey combatSkillKey = new CombatSkillKey(charId, skillTemplateId);
		bool flag = this._subProcessorSkills.ContainsKey(combatSkillKey);
		if (!flag)
		{
			this._subProcessorSkills[combatSkillKey] = new CombatSubProcessorSkill(combatSkillKey);
		}
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0003FB14 File Offset: 0x0003DD14
	private void UpdateSubProcessorWeapons()
	{
		foreach (CombatSubProcessorCharacter processor in this.ProcessorCharacters.Values)
		{
			ItemKey[] weapons = processor.Weapons;
			bool flag = weapons == null || weapons.Length <= 0;
			if (!flag)
			{
				foreach (ItemKey key in processor.Weapons)
				{
					bool flag2 = !key.IsValid() || this._subProcessorWeapons.ContainsKey(key);
					if (!flag2)
					{
						this._subProcessorWeapons[key] = new CombatSubProcessorWeapon(key);
					}
				}
			}
		}
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0003FBE8 File Offset: 0x0003DDE8
	public void RequestSimulatePrepareCombat(AsyncMethodCallbackDelegate callback)
	{
		CombatDomainMethod.AsyncCall.PrepareSimulate(this._dispatcher, this._selfTeam, this._enemyTeam, callback);
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x0003FC04 File Offset: 0x0003DE04
	public void RequestGetCharacterWisdomList(bool isAlly, AsyncMethodCallbackDelegate callback)
	{
		List<int> request = isAlly ? this._selfTeam : this._enemyTeam;
		CharacterDomainMethod.AsyncCall.GetCharacterWisdomList(this._dispatcher, request, callback);
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0003FC34 File Offset: 0x0003DE34
	public void RequestApplyVitalOnTeammate(StoryTeammateType type, int index, Action<bool> callback)
	{
		bool flag = type < StoryTeammateType.IronPlate && !this.Config.AllowVitalDemon;
		if (flag)
		{
			Action<bool> callback2 = callback;
			if (callback2 != null)
			{
				callback2(false);
			}
		}
		else
		{
			StoryDomainMethod.Call.ThreeVitalsReplaceTeammateRecordSet((int)type, index);
			CombatDomainMethod.AsyncCall.ApplyVitalOnTeammate(this._dispatcher, (int)type, index, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				bool flag2 = displayData == null;
				if (flag2)
				{
					Action<bool> callback3 = callback;
					if (callback3 != null)
					{
						callback3(false);
					}
				}
				else
				{
					this._displayDataCache[displayData.CharacterId] = displayData;
					bool flag3 = index == this._selfTeam.Count;
					if (flag3)
					{
						this._selfTeam.Add(displayData.CharacterId);
					}
					else
					{
						this._selfTeam[index] = displayData.CharacterId;
					}
					Action<bool> callback4 = callback;
					if (callback4 != null)
					{
						callback4(true);
					}
				}
			});
		}
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0003FCB8 File Offset: 0x0003DEB8
	public void RequestRevertVitalOnTeammate(StoryTeammateType type, Action<bool> callback)
	{
		StoryDomainMethod.Call.ThreeVitalsReplaceTeammateRecordRemove((int)type);
		CombatDomainMethod.AsyncCall.RevertVitalOnTeammate(this._dispatcher, (int)type, delegate(int offset, RawDataPool pool)
		{
			int replacedIndex = -1;
			Serializer.Deserialize(pool, offset, ref replacedIndex);
			bool flag = replacedIndex < 0;
			if (flag)
			{
				Action<bool> callback2 = callback;
				if (callback2 != null)
				{
					callback2(false);
				}
			}
			else
			{
				int originCharId = this.CommandChangeData.LeftTeam.TeammateCharIds.GetOrDefault(replacedIndex - 1, -1);
				bool flag2 = originCharId < 0;
				if (flag2)
				{
					this._selfTeam.RemoveAt(replacedIndex);
				}
				else
				{
					this._selfTeam[replacedIndex] = originCharId;
				}
				Action<bool> callback3 = callback;
				if (callback3 != null)
				{
					callback3(true);
				}
			}
		});
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0003FCFC File Offset: 0x0003DEFC
	public void RequestGetAllEquipmentItems(int charId, OnCombatEvent callback = null)
	{
		CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this._dispatcher, charId, delegate(int offset, RawDataPool pool)
		{
			List<ItemDisplayData> equipments = new List<ItemDisplayData>();
			Serializer.Deserialize(pool, offset, ref equipments);
			this._equipmentDataCache[charId] = equipments;
			OnCombatEvent callback2 = callback;
			if (callback2 != null)
			{
				callback2();
			}
		});
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0003FD44 File Offset: 0x0003DF44
	public void RequestProactiveSkillList(int charId)
	{
		CombatDomainMethod.AsyncCall.GetProactiveSkillList(this._dispatcher, charId, delegate(int offset, RawDataPool pool)
		{
			List<CombatSkillDisplayData> deserializeDisplayDataList = null;
			Serializer.Deserialize(pool, offset, ref deserializeDisplayDataList);
			IReadOnlyList<CombatSkillDisplayData> displayDataList = deserializeDisplayDataList;
			if (displayDataList == null)
			{
				displayDataList = Array.Empty<CombatSkillDisplayData>();
			}
			this._proactiveSkillData[charId] = displayDataList;
			bool flag = charId == this.SelfCharId;
			if (flag)
			{
				foreach (CombatSkillDisplayData data in displayDataList)
				{
					this.UpdateSubProcessorSkill(charId, data.TemplateId);
				}
			}
			base.<RequestProactiveSkillList>g__UpdateOrderedProactiveSkillData|1(displayDataList);
			OnGetProactiveSkillListEvent onGetProactiveSkillList = this.OnGetProactiveSkillList;
			if (onGetProactiveSkillList != null)
			{
				onGetProactiveSkillList(charId);
			}
		});
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0003FD85 File Offset: 0x0003DF85
	public void RequestSetPuppetDisableAi(bool disableAi)
	{
		CombatDomainMethod.Call.SetPuppetDisableAi(this._listenerId, disableAi);
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0003FD94 File Offset: 0x0003DF94
	public void RequestSetPuppetUnyieldingFallen(bool unyieldingFallen)
	{
		CombatDomainMethod.Call.SetPuppetUnyieldingFallen(this._listenerId, unyieldingFallen);
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0003FDA3 File Offset: 0x0003DFA3
	public void DoRequestUseTeammateCommand(int index, int teammateId)
	{
		CombatDomainMethod.Call.ExecuteTeammateCommand(this._listenerId, true, index, teammateId);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0003FDB4 File Offset: 0x0003DFB4
	public void RequestGetPreviewCostSkillDisplayData(short skillTemplateId, bool canUse, Action callback)
	{
		IReadOnlyList<CombatSkillDisplayData> skillDataList;
		bool flag = !this._proactiveSkillData.TryGetValue(this.SelfCharId, out skillDataList);
		if (!flag)
		{
			bool flag2 = skillDataList.All((CombatSkillDisplayData data) => data.TemplateId != skillTemplateId);
			if (!flag2)
			{
				List<short> skillList = EasyPool.Get<List<short>>();
				skillList.Clear();
				skillList.Add(skillTemplateId);
				this.PreviewCostSkill = skillTemplateId;
				this.PreviewCostSkillCanUse = canUse;
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayData(this._dispatcher, this.SelfCharId, skillList, delegate(int offset, RawDataPool rawDataPool)
				{
					List<CombatSkillDisplayData> dataList = null;
					Serializer.Deserialize(rawDataPool, offset, ref dataList);
					this._previewCostSkillData = dataList[0];
					Action callback2 = callback;
					if (callback2 != null)
					{
						callback2();
					}
					EasyPool.Free<List<short>>(skillList);
				});
			}
		}
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0003FE73 File Offset: 0x0003E073
	public void DoRequestGetAllCostNeiliEffectData()
	{
		SpecialEffectDomainMethod.Call.GetAllCostNeiliEffectData(this._listenerId, this.SelfCharId, this.SelfCharacter.PreparingSkillId);
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0003FE93 File Offset: 0x0003E093
	public void DoRequestCostNeiliEffect(short effectId)
	{
		SpecialEffectDomainMethod.Call.CostNeiliEffect(this.SelfCharId, this.SelfCharacter.PreparingSkillId, effectId);
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0003FEAE File Offset: 0x0003E0AE
	public void DoRequestCanCostTrickDuringPreparingSkill()
	{
		SpecialEffectDomainMethod.Call.CanCostTrickDuringPreparingSkill(this._listenerId, this.SelfCharId, this.SelfCharacter.PreparingSkillId);
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0003FECE File Offset: 0x0003E0CE
	public void DoRequestCostTrickDuringPreparingSkill(int trickIndex)
	{
		SpecialEffectDomainMethod.Call.CostTrickDuringPreparingSkill(this._listenerId, this.SelfCharId, trickIndex);
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0003FEE4 File Offset: 0x0003E0E4
	public void SetEntryData(short configId, IReadOnlyList<int> leftTeam, IReadOnlyList<int> rightTeam)
	{
		this._configId = configId;
		this._sceneId = this.Config.Scene;
		bool flag = this._sceneId < 0;
		if (flag)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			short blockTemplateId = mapModel.GetBlockData(mapModel.CurrentBlockId).TemplateId;
			this._sceneId = ((blockTemplateId == 12 && Utils_Random.RandomCheck(1, 100)) ? 42 : MapBlock.Instance[blockTemplateId].CombatScene);
		}
		this._selfTeam.ClearAndAddRange(from x in leftTeam
		where x >= 0
		select x);
		this._enemyTeam.ClearAndAddRange(from x in rightTeam
		where x >= 0
		select x);
		this._displayDataCache.Clear();
		CombatDomainMethod.Call.PreparePreRandomTeammateCommands(this._listenerId, this._configId, this._selfTeam, this._enemyTeam);
		this.ResetMonitorValues();
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0003FFED File Offset: 0x0003E1ED
	public void SetChangeData(int charId, IEnumerable<sbyte> cmdTypes)
	{
		this.CommandChangeData.SetCharTeammateCommands(charId, cmdTypes);
		this.RaiseEvent(ECombatEvents.BeginTeammateCommandChanged);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00040006 File Offset: 0x0003E206
	public void ClearPreviewCostSkillData()
	{
		this._previewCostSkillData = null;
		this.PreviewCostSkill = -1;
		this.PreviewCostSkillCanUse = false;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00040020 File Offset: 0x0003E220
	public void ModifyPreviewCostSkillCanUse(bool canUse)
	{
		this.PreviewCostSkillCanUse = canUse;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x0004002B File Offset: 0x0003E22B
	public void SetPausing(bool isPausing)
	{
		this.IsPausing = isPausing;
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00040036 File Offset: 0x0003E236
	public bool CharIsAlly(int charId)
	{
		return this._selfTeam.Contains(charId) || this.CarrierAnimalCombatCharId == charId || this.SpecialShowCombatCharId == charId;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0004005B File Offset: 0x0003E25B
	public int MainCharId(bool isAlly)
	{
		return isAlly ? this._selfTeam[0] : this._enemyTeam[0];
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x0004007C File Offset: 0x0003E27C
	public bool TryGetCharacterDisplayProcessor(int charId, out CombatSubProcessorCharacterDisplay processorDisplay)
	{
		CombatSubProcessorCharacter processor;
		bool flag = this._subProcessorCharacters.TryGetValue(charId, out processor);
		if (flag)
		{
			processorDisplay = processor.Display;
		}
		else
		{
			bool flag2 = charId == this.CarrierAnimalCombatCharId;
			if (flag2)
			{
				processorDisplay = this.ProcessorAnimalChar;
			}
			else
			{
				bool flag3 = charId == this.SpecialShowCombatCharId;
				if (flag3)
				{
					processorDisplay = this.ProcessorSpecialChar;
				}
				else
				{
					processorDisplay = null;
				}
			}
		}
		return processorDisplay != null;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x000400E4 File Offset: 0x0003E2E4
	public bool TryGetWeaponKeyById(int itemId, out ItemKey weaponKey)
	{
		foreach (CombatSubProcessorCharacter processor in this._subProcessorCharacters.Values)
		{
			ItemKey[] weapons = processor.Weapons;
			bool flag = weapons == null || weapons.Length <= 0;
			if (!flag)
			{
				foreach (ItemKey key in processor.Weapons)
				{
					bool flag2 = key.Id != itemId;
					if (!flag2)
					{
						weaponKey = key;
						return true;
					}
				}
			}
		}
		weaponKey = ItemKey.Invalid;
		return false;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x000401B4 File Offset: 0x0003E3B4
	public sbyte GetWeaponExpectRatio(int charId, int weaponIndex)
	{
		bool flag = charId == this.TaiwuCharId;
		sbyte result;
		if (flag)
		{
			bool flag2 = weaponIndex < 3;
			if (flag2)
			{
				List<ItemDisplayData> equipment;
				bool flag3 = this._equipmentDataCache.TryGetValue(charId, out equipment);
				if (flag3)
				{
					ItemKey item = equipment[weaponIndex].RealKey;
					bool flag4 = item.TemplateId >= 0;
					if (flag4)
					{
						return this._weaponInnerRatiosById.GetValueOrDefault(item.Id, Weapon.Instance[item.TemplateId].DefaultInnerRatio);
					}
				}
				result = -1;
			}
			else
			{
				if (!true)
				{
				}
				short num;
				switch (weaponIndex)
				{
				case 3:
					num = 0;
					break;
				case 4:
					num = 1;
					break;
				case 5:
					num = 2;
					break;
				default:
					num = 884;
					break;
				}
				if (!true)
				{
				}
				short templateId = num;
				result = this._weaponInnerRatiosByTemplateId.GetValueOrDefault(templateId, Weapon.Instance[templateId].DefaultInnerRatio);
			}
		}
		else
		{
			CombatSubProcessorCharacter processor;
			bool flag5 = !this._subProcessorCharacters.TryGetValue(charId, out processor);
			if (flag5)
			{
				result = -1;
			}
			else
			{
				ItemKey weapon = processor.Weapons.GetOrDefault(weaponIndex, ItemKey.Invalid);
				result = (weapon.IsValid() ? Weapon.Instance[weapon.TemplateId].DefaultInnerRatio : -1);
			}
		}
		return result;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00040300 File Offset: 0x0003E500
	public bool CanOperateCharacter(int charId)
	{
		bool flag = !this.CharIsAlly(charId);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !this.TaiwuInCombat;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = charId == this.MainCharId(true);
				if (flag3)
				{
					result = true;
				}
				else
				{
					CombatSubProcessorCharacter processor;
					bool flag4 = !this.ProcessorCharacters.TryGetValue(charId, out processor);
					if (flag4)
					{
						result = false;
					}
					else
					{
						bool flag5 = processor.ExecutingTeammateCommand < 0;
						result = (!flag5 && TeammateCommand.Instance[processor.ExecutingTeammateCommand].Implement.IsFight());
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00040394 File Offset: 0x0003E594
	public void RequestPrepareCombat()
	{
		CombatDomainMethod.Call.PrepareCombat(this._listenerId, this._configId, this._selfTeam, this._enemyTeam);
		foreach (DataUid uid in CombatModel.AutoMonitorDataUids)
		{
			this.MonitorData(uid);
		}
		this.ProcessorTaiwu = new CombatSubProcessorTaiwu();
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00040410 File Offset: 0x0003E610
	private void MonitorData(DataUid uid)
	{
		this._monitorUids.Add(uid);
		GameDataBridge.AddDataMonitor(this._listenerId, uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1);
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00040444 File Offset: 0x0003E644
	private void MonitorData(ushort dataId)
	{
		this.MonitorData(new DataUid(8, dataId, ulong.MaxValue, uint.MaxValue));
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00040457 File Offset: 0x0003E657
	private void MonitorData(ushort domainId, ushort dataId)
	{
		this.MonitorData(new DataUid(domainId, dataId, ulong.MaxValue, uint.MaxValue));
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0004046A File Offset: 0x0003E66A
	private void MonitorData(ushort domainId, ushort subId0, ushort subId1)
	{
		this.MonitorData(new DataUid(domainId, subId0, (ulong)subId1, uint.MaxValue));
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0004047D File Offset: 0x0003E67D
	private void MonitorData(ushort domainId, ushort subId0, ushort subId1, ushort subId2)
	{
		this.MonitorData(new DataUid(domainId, subId0, (ulong)subId1, (uint)subId2));
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00040494 File Offset: 0x0003E694
	private void UnMonitorData(DataUid uid)
	{
		bool flag = this._monitorUids.Remove(uid);
		if (flag)
		{
			GameDataBridge.AddDataUnMonitor(this._listenerId, uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1);
		}
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x000404D8 File Offset: 0x0003E6D8
	private void UnMonitorByTemporaryList()
	{
		this._monitorUids.RemoveAll(new Predicate<DataUid>(this._unMonitorTemporaryUids.Contains));
		foreach (DataUid uid in this._unMonitorTemporaryUids)
		{
			GameDataBridge.AddDataUnMonitor(this._listenerId, uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1);
		}
		this._unMonitorTemporaryUids.Clear();
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x00040578 File Offset: 0x0003E778
	private void UnMonitorData(ushort dataId)
	{
		foreach (DataUid uid in this._monitorUids)
		{
			bool flag = uid.DomainId == 8 && uid.DataId == dataId;
			if (flag)
			{
				this._unMonitorTemporaryUids.Add(uid);
			}
		}
		this.UnMonitorByTemporaryList();
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x000405F4 File Offset: 0x0003E7F4
	private void UnMonitorData(ushort domainId, ushort dataId)
	{
		foreach (DataUid uid in this._monitorUids)
		{
			bool flag = uid.DomainId == domainId && uid.DataId == dataId;
			if (flag)
			{
				this._unMonitorTemporaryUids.Add(uid);
			}
		}
		this.UnMonitorByTemporaryList();
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00040670 File Offset: 0x0003E870
	private void UnMonitorData(ushort domainId, ushort dataId, ulong subId0)
	{
		foreach (DataUid uid in this._monitorUids)
		{
			bool flag = uid.DomainId == domainId && uid.DataId == dataId && uid.SubId0 == subId0;
			if (flag)
			{
				this._unMonitorTemporaryUids.Add(uid);
			}
		}
		this.UnMonitorByTemporaryList();
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x000406F8 File Offset: 0x0003E8F8
	private void UnMonitorData(ushort domainId, ushort dataId, ulong subId0, uint subId1)
	{
		foreach (DataUid uid in this._monitorUids)
		{
			bool flag = uid.DomainId == domainId && uid.DataId == dataId && uid.SubId0 == subId0 && uid.SubId1 == subId1;
			if (flag)
			{
				this._unMonitorTemporaryUids.Add(uid);
			}
		}
		this.UnMonitorByTemporaryList();
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00040788 File Offset: 0x0003E988
	private void UnMonitorAllData()
	{
		foreach (DataUid uid in this._monitorUids)
		{
			GameDataBridge.AddDataUnMonitor(this._listenerId, uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1);
		}
		foreach (CombatSubProcessor subProcessor in this._subProcessors)
		{
			subProcessor.AntiReady();
		}
		this._monitorUids.Clear();
		this._subProcessors.Clear();
		this._subProcessorSkills.Clear();
		this._subProcessorWeapons.Clear();
		this._subProcessorCharacters.Clear();
		this.ProcessorTaiwu = null;
		this.ProcessorAnimalChar = null;
		this.ProcessorSpecialChar = null;
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x00040894 File Offset: 0x0003EA94
	private void OnNotifyData(List<NotificationWrapper> notifications)
	{
		bool flag = this._handlerData == null;
		if (!flag)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				foreach (CombatNotifyHandler handler in this._handlerData)
				{
					bool flag2 = handler.IsMatch(wrapper.Notification);
					if (flag2)
					{
						handler.Handle(wrapper);
					}
				}
				foreach (CombatSubProcessor subProcessor in this._subProcessors)
				{
					subProcessor.Process(wrapper);
				}
			}
			this.UpdateSubProcessors();
		}
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0004099C File Offset: 0x0003EB9C
	[CombatNotifyMethod(8, 104)]
	private void HandlerPreparePreRandomTeammateCommands(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CommandChangeData);
		int enemyMainCharId = this._enemyTeam[0];
		List<int> enemyTeam = this.CommandChangeData.RightTeam.TeammateCharIds;
		this._enemyTeam.Clear();
		this._enemyTeam.Add(enemyMainCharId);
		for (int i = 0; i < 3; i++)
		{
			bool flag = i < enemyTeam.Count && enemyTeam[i] >= 0;
			if (flag)
			{
				this._enemyTeam.Add(enemyTeam[i]);
			}
		}
		CombatDomainMethod.Call.PrepareEnemyEquipments(this._configId, this._enemyTeam);
		List<int> request = EasyPool.Get<List<int>>();
		request.Clear();
		foreach (int charId in this._selfTeam)
		{
			bool flag2 = charId >= 0;
			if (flag2)
			{
				request.Add(charId);
			}
		}
		foreach (int charId2 in this._enemyTeam)
		{
			bool flag3 = charId2 >= 0;
			if (flag3)
			{
				request.Add(charId2);
			}
		}
		Dictionary<int, int> betrayedCharIds = this.CommandChangeData.LeftTeam.BetrayedCharIds;
		bool flag4 = betrayedCharIds != null && betrayedCharIds.Count > 0;
		if (flag4)
		{
			request.AddRange(this.CommandChangeData.RightTeam.BetrayedCharIds.Values);
		}
		CharacterDomainMethod.Call.GetCharacterDisplayDataList(this._listenerId, request);
		EasyPool.Free<List<int>>(request);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00040B5C File Offset: 0x0003ED5C
	[CombatNotifyMethod(4, 48)]
	private void HandlerGetCharacterDisplayDataList(RawDataPool pool, int offset)
	{
		List<CharacterDisplayData> displayDataList = null;
		Serializer.Deserialize(pool, offset, ref displayDataList);
		foreach (CharacterDisplayData data in displayDataList)
		{
			this._displayDataCache[data.CharacterId] = data;
		}
		this.RaiseEvent(ECombatEvents.BeginDataReady);
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x00040BD0 File Offset: 0x0003EDD0
	[CombatNotifyMethod(8, 36)]
	private void HandlerPrepareCombat(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.FirstMoveType);
		foreach (int charId in this.SelfTeam)
		{
			this._subProcessorCharacters[charId] = new CombatSubProcessorCharacter(charId);
		}
		foreach (int charId2 in this.EnemyTeam)
		{
			this._subProcessorCharacters[charId2] = new CombatSubProcessorCharacter(charId2);
		}
		this.RaiseEvent(ECombatEvents.CombatPrepared);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00040C8C File Offset: 0x0003EE8C
	[CombatNotifyMethod(17, 0)]
	private void HandleGetAllCostNeiliEffectData(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this._costNeiliEffects);
		this.RaiseEvent(ECombatEvents.OnGetAllCostNeiliEffectData);
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x00040CA6 File Offset: 0x0003EEA6
	[CombatNotifyMethod(17, 2)]
	private void HandleCanCostTrickDuringPreparingSkill(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this._canCostTrickDuringPreparingSkill);
		this.RaiseEvent(ECombatEvents.OnGetCanCostTrickDuringPreparingSkill);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x00040CC0 File Offset: 0x0003EEC0
	[CombatNotifyData(8, 9)]
	[CombatAutoMonitor]
	private void HandlerDataBgmIndex(RawDataPool pool, int offset)
	{
		sbyte bgmIndex = 0;
		Serializer.Deserialize(pool, offset, ref bgmIndex);
		bool flag = bgmIndex > 0;
		if (flag)
		{
			AudioManager.Instance.PlayMusic(this.Config.Bgm[(int)bgmIndex], 1f, 100, null);
		}
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00040D04 File Offset: 0x0003EF04
	[CombatNotifyData(8, 19)]
	[CombatAutoMonitor]
	private void HandlerDataCombatStatus(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CombatStatus);
		bool isCombatOver = this.IsCombatOver;
		if (isCombatOver)
		{
			this.OnCombatEnd();
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00040D31 File Offset: 0x0003EF31
	[CombatNotifyData(8, 2)]
	[CombatAutoMonitor]
	private void HandlerDataCombatFrame(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CombatFrame);
		this.RaiseEvent(ECombatEvents.OnCombatFrameChanged);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00040D4A File Offset: 0x0003EF4A
	[CombatNotifyData(8, 0)]
	[CombatAutoMonitor]
	private void HandlerDataTimeScale(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TimeScale);
		this.RaiseEvent(ECombatEvents.OnTimeScaleChanged);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x00040D63 File Offset: 0x0003EF63
	[CombatNotifyData(8, 4)]
	[CombatAutoMonitor]
	private void HandlerDataCurrentDistance(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrentDistance);
		this.RaiseEvent(ECombatEvents.OnCurrentDistanceChanged);
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x00040D7D File Offset: 0x0003EF7D
	[CombatNotifyData(8, 27)]
	[CombatAutoMonitor]
	private void HandlerDataIsPuppetCombat(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.IsPuppetCombat);
		this.RaiseEvent(ECombatEvents.OnIsPuppetCombatChanged);
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00040D97 File Offset: 0x0003EF97
	[CombatNotifyData(8, 28)]
	[CombatAutoMonitor]
	private void HandlerDataIsPlaygroundCombat(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.IsPlaygroundCombat);
		this.RaiseEvent(ECombatEvents.OnIsPlaygroundCombatChanged);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00040DB1 File Offset: 0x0003EFB1
	[CombatNotifyData(8, 5)]
	[CombatAutoMonitor]
	private void HandlerDataDamageCompareData(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.DamageCompareData);
		this.RaiseEvent(ECombatEvents.OnDamageCompareDataChanged);
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00040DCB File Offset: 0x0003EFCB
	[CombatNotifyData(8, 41)]
	[CombatAutoMonitor]
	private void HandlerDataSkillDamageData(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SkillDamageData);
		this.RaiseEvent(ECombatEvents.OnSkillDamageDataChanged);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00040DE5 File Offset: 0x0003EFE5
	[CombatNotifyData(8, 20)]
	[CombatAutoMonitor]
	private void HandlerDataShowMercyOption(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this._showMercyOption);
		this.RaiseEvent(ECombatEvents.OnShowMercyOptionChanged);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00040DFF File Offset: 0x0003EFFF
	[CombatNotifyData(8, 21)]
	[CombatAutoMonitor]
	private void HandlerDataSelectedMercyOption(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this._selectedMercyOption);
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00040E10 File Offset: 0x0003F010
	[CombatNotifyData(8, 22)]
	[CombatAutoMonitor]
	private void HandlerDataCarrierAnimalCombatCharId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CarrierAnimalCombatCharId);
		bool flag = this.CarrierAnimalCombatCharId < 0;
		if (!flag)
		{
			CombatSubProcessorCharacterDisplay processorAnimalChar = this.ProcessorAnimalChar;
			if (processorAnimalChar != null)
			{
				processorAnimalChar.Close();
			}
			this.ProcessorAnimalChar = new CombatSubProcessorCharacterDisplay(this.CarrierAnimalCombatCharId);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this._dispatcher, this.CarrierAnimalCombatCharId, new AsyncMethodCallbackDelegate(this.HandlerDataCarrierAnimalCombatCharIdSubRequest));
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00040E80 File Offset: 0x0003F080
	private void HandlerDataCarrierAnimalCombatCharIdSubRequest(int offset, RawDataPool pool)
	{
		CharacterDisplayData data = null;
		Serializer.Deserialize(pool, offset, ref data);
		this._displayDataCache[data.CharacterId] = data;
		this.RaiseEvent(ECombatEvents.CarrierAnimalCombatCharIdReady);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00040EB8 File Offset: 0x0003F0B8
	[CombatNotifyData(8, 23)]
	[CombatAutoMonitor]
	private void HandlerDataSpecialShowCombatCharId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SpecialShowCombatCharId);
		bool flag = this.SpecialShowCombatCharId < 0;
		if (!flag)
		{
			CombatSubProcessorCharacterDisplay processorSpecialChar = this.ProcessorSpecialChar;
			if (processorSpecialChar != null)
			{
				processorSpecialChar.Close();
			}
			this.ProcessorSpecialChar = new CombatSubProcessorCharacterDisplay(this.SpecialShowCombatCharId);
			this.RequestGetAllEquipmentItems(this.SpecialShowCombatCharId, null);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this._dispatcher, this.SpecialShowCombatCharId, new AsyncMethodCallbackDelegate(this.HandlerDataSpecialShowCombatCharIdSubRequest));
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00040F34 File Offset: 0x0003F134
	private void HandlerDataSpecialShowCombatCharIdSubRequest(int offset, RawDataPool pool)
	{
		CharacterDisplayData data = null;
		Serializer.Deserialize(pool, offset, ref data);
		this._displayDataCache[data.CharacterId] = data;
		this.RaiseEvent(ECombatEvents.SpecialShowCombatCharIdReady);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00040F6A File Offset: 0x0003F16A
	[CombatNotifyData(5, 30)]
	[CombatAutoMonitor]
	private void HandlerDataCurrEquipmentPlanId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrEquipmentPlanId);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00040F7A File Offset: 0x0003F17A
	[CombatNotifyData(5, 108)]
	[CombatAutoMonitor]
	private void HandlerDataHideSkeletonEquipSlots(RawDataPool pool, int offset)
	{
		Serializer.DeserializeModifications<int>(pool, offset, this.HideSkeletonEquipSlots);
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00040F8A File Offset: 0x0003F18A
	[CombatNotifyData(5, 16)]
	[CombatAutoMonitor]
	private void HandlerDataCurrCombatSkillPlanId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrCombatSkillPlanId);
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00040F9C File Offset: 0x0003F19C
	[CombatNotifyData(19, 5)]
	[CombatAutoMonitor]
	private void HandlerDataCombatSkillOrderPlans(RawDataPool pool, int offset)
	{
		Serializer.DeserializeModifications<int>(pool, offset, this.CombatSkillOrderPlans);
		bool flag = !this.IsTutorialCombat;
		if (flag)
		{
			this.RequestProactiveSkillList(this.SelfCharId);
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00040FD2 File Offset: 0x0003F1D2
	[CombatNotifyData(5, 110)]
	[CombatAutoMonitor]
	private void HandlerDataWeaponInnerRatios(RawDataPool pool, int offset)
	{
		Serializer.DeserializeModifications<int>(pool, offset, this._weaponInnerRatiosById);
		this.RaiseEvent(ECombatEvents.OnWeaponInnerRatioChanged);
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00040FEC File Offset: 0x0003F1EC
	[CombatNotifyData(5, 111)]
	[CombatAutoMonitor]
	private void HandlerDataVoiceWeaponInnerRatio(RawDataPool pool, int offset)
	{
		Serializer.DeserializeModifications<short>(pool, offset, this._weaponInnerRatiosByTemplateId);
		this.RaiseEvent(ECombatEvents.OnWeaponInnerRatioChanged);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00041008 File Offset: 0x0003F208
	[CombatNotifyData(8, 12)]
	[CombatAutoMonitor]
	private void HandlerDataSelfCharId(RawDataPool pool, int offset)
	{
		int newCharId = -1;
		Serializer.Deserialize(pool, offset, ref newCharId);
		this.HandlerDataCommonCharId(ref this.SelfCharId, newCharId);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x00041030 File Offset: 0x0003F230
	[CombatNotifyData(8, 16)]
	[CombatAutoMonitor]
	private void HandlerDataEnemyCharId(RawDataPool pool, int offset)
	{
		int newCharId = -1;
		Serializer.Deserialize(pool, offset, ref newCharId);
		this.HandlerDataCommonCharId(ref this.EnemyCharId, newCharId);
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00041058 File Offset: 0x0003F258
	private void HandlerDataCommonCharId(ref int srcCharId, int dstCharId)
	{
		bool flag = srcCharId == dstCharId;
		if (!flag)
		{
			bool flag2 = srcCharId >= 0 && dstCharId >= 0;
			if (flag2)
			{
				this.ChangingFromCharId = srcCharId;
				this.ChangingToCharId = dstCharId;
				this.RaiseEvent(ECombatEvents.ChangeChar);
				this.ChangingFromCharId = (this.ChangingToCharId = -1);
			}
			srcCharId = dstCharId;
		}
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x000410B4 File Offset: 0x0003F2B4
	[CombatNotifyData(8, 13)]
	[CombatAutoMonitor]
	private void HandlerDataSelfTeamWisdomType(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SelfTeamWisdomType);
		this.RaiseEvent(ECombatEvents.OnTeamWisdomTypeChanged);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x000410CE File Offset: 0x0003F2CE
	[CombatNotifyData(8, 17)]
	[CombatAutoMonitor]
	private void HandlerDataEnemyTeamWisdomType(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.EnemyTeamWisdomType);
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x000410DF File Offset: 0x0003F2DF
	[CombatNotifyData(8, 14)]
	[CombatAutoMonitor]
	private void HandlerDataSelfTeamWisdomCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SelfTeamWisdomCount);
		this.RaiseEvent(ECombatEvents.OnTeamWisdomCountChanged);
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x000410F9 File Offset: 0x0003F2F9
	[CombatNotifyData(8, 18)]
	[CombatAutoMonitor]
	private void HandlerDataEnemyTeamWisdomCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.EnemyTeamWisdomCount);
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x0004110C File Offset: 0x0003F30C
	[CombatNotifyData(8, 26)]
	[CombatAutoMonitor]
	private void HandlerDataShowUseGoldenWire(RawDataPool pool, int offset)
	{
		bool flag = this.OnShowUseGoldenWireChanged == null;
		if (!flag)
		{
			SpecialMiscData data = default(SpecialMiscData);
			Serializer.Deserialize(pool, offset, ref data);
			this.OnShowUseGoldenWireChanged(data);
		}
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00041148 File Offset: 0x0003F348
	[CombatNotifyData(8, 25)]
	[CombatAutoMonitor]
	private void HandlerDataWaitingDelaySettlement(RawDataPool pool, int offset)
	{
		bool waitingDelaySettlement = false;
		Serializer.Deserialize(pool, offset, ref waitingDelaySettlement);
		bool flag = waitingDelaySettlement;
		if (flag)
		{
			this.RaiseEvent(ECombatEvents.WaitingDelaySettlement);
		}
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00041170 File Offset: 0x0003F370
	private void ResetMonitorValues()
	{
		this.CarrierAnimalCombatCharId = -1;
		this.SpecialShowCombatCharId = -1;
		this.SelfCharId = (this.EnemyCharId = -1);
		this._showMercyOption = (this._selectedMercyOption = -1);
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x000411AC File Offset: 0x0003F3AC
	private int[] ParseRequest(IReadOnlyList<int> request)
	{
		for (int i = 0; i < 4; i++)
		{
			this._requestTemporaryArray[i] = ((i < request.Count) ? request[i] : -1);
		}
		return this._requestTemporaryArray;
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000411EF File Offset: 0x0003F3EF
	public void OnCombatEnd()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			this.UnMonitorAllData();
			this.RaiseEvent(ECombatEvents.CombatEnd);
		});
	}

	// Token: 0x04000C15 RID: 3093
	private const int TeamCharCount = 4;

	// Token: 0x04000C16 RID: 3094
	private const short EasterEggBlockId = 12;

	// Token: 0x04000C17 RID: 3095
	private const sbyte EasterEggOdds = 1;

	// Token: 0x04000C18 RID: 3096
	public static readonly bool CanOperateTeammate = true;

	// Token: 0x04000C19 RID: 3097
	public const byte NormalWeaponCount = 3;

	// Token: 0x04000C1A RID: 3098
	public const byte MaxWeaponCount = 7;

	// Token: 0x04000C1B RID: 3099
	public const int WeaponIndexEmptyHand = 3;

	// Token: 0x04000C1C RID: 3100
	public const int WeaponIndexBranch = 4;

	// Token: 0x04000C1D RID: 3101
	public const int WeaponIndexStone = 5;

	// Token: 0x04000C1E RID: 3102
	public const int WeaponIndexVoice = 6;

	// Token: 0x04000C1F RID: 3103
	private static readonly IReadOnlyList<DataUid> AutoMonitorDataUids = new List<DataUid>(CombatNotifyHelper.ParseAutoMonitors<CombatModel>());

	// Token: 0x04000C24 RID: 3108
	public sbyte CombatStatus;

	// Token: 0x04000C25 RID: 3109
	public ulong CombatFrame;

	// Token: 0x04000C26 RID: 3110
	public float TimeScale;

	// Token: 0x04000C27 RID: 3111
	public short CurrentDistance;

	// Token: 0x04000C28 RID: 3112
	public bool IsPuppetCombat;

	// Token: 0x04000C29 RID: 3113
	public bool IsPlaygroundCombat;

	// Token: 0x04000C2A RID: 3114
	public DamageCompareData DamageCompareData = new DamageCompareData();

	// Token: 0x04000C2B RID: 3115
	public SkillDamageData SkillDamageData;

	// Token: 0x04000C2C RID: 3116
	private sbyte _showMercyOption;

	// Token: 0x04000C2D RID: 3117
	private sbyte _selectedMercyOption;

	// Token: 0x04000C2E RID: 3118
	public int CarrierAnimalCombatCharId;

	// Token: 0x04000C2F RID: 3119
	public int SpecialShowCombatCharId;

	// Token: 0x04000C30 RID: 3120
	public int CurrEquipmentPlanId;

	// Token: 0x04000C31 RID: 3121
	public readonly Dictionary<int, SByteList> HideSkeletonEquipSlots = new Dictionary<int, SByteList>();

	// Token: 0x04000C32 RID: 3122
	public int CurrCombatSkillPlanId;

	// Token: 0x04000C33 RID: 3123
	public readonly Dictionary<int, GameData.Utilities.ShortList> CombatSkillOrderPlans = new Dictionary<int, GameData.Utilities.ShortList>();

	// Token: 0x04000C34 RID: 3124
	private Dictionary<int, sbyte> _weaponInnerRatiosById = new Dictionary<int, sbyte>();

	// Token: 0x04000C35 RID: 3125
	private Dictionary<short, sbyte> _weaponInnerRatiosByTemplateId = new Dictionary<short, sbyte>();

	// Token: 0x04000C36 RID: 3126
	public int SelfCharId;

	// Token: 0x04000C37 RID: 3127
	public int EnemyCharId;

	// Token: 0x04000C38 RID: 3128
	public sbyte SelfTeamWisdomType;

	// Token: 0x04000C39 RID: 3129
	public sbyte EnemyTeamWisdomType;

	// Token: 0x04000C3A RID: 3130
	public short SelfTeamWisdomCount;

	// Token: 0x04000C3B RID: 3131
	public short EnemyTeamWisdomCount;

	// Token: 0x04000C3C RID: 3132
	private readonly Dictionary<CombatSkillKey, CombatSubProcessorSkill> _subProcessorSkills = new Dictionary<CombatSkillKey, CombatSubProcessorSkill>();

	// Token: 0x04000C3D RID: 3133
	private readonly Dictionary<ItemKey, CombatSubProcessorWeapon> _subProcessorWeapons = new Dictionary<ItemKey, CombatSubProcessorWeapon>();

	// Token: 0x04000C3E RID: 3134
	private readonly Dictionary<int, CombatSubProcessorCharacter> _subProcessorCharacters = new Dictionary<int, CombatSubProcessorCharacter>();

	// Token: 0x04000C3F RID: 3135
	public OnDataChangedEvent OnNeiliTypeChanged;

	// Token: 0x04000C40 RID: 3136
	public OnDataChangedEvent OnCurrNeiliChanged;

	// Token: 0x04000C41 RID: 3137
	public OnDataChangedEvent OnMaxNeiliChanged;

	// Token: 0x04000C42 RID: 3138
	public OnDataChangedEvent OnHealthChanged;

	// Token: 0x04000C43 RID: 3139
	public OnDataChangedEvent OnConsummateLevelChanged;

	// Token: 0x04000C44 RID: 3140
	public OnDataChangedEvent OnMoveSpeedChanged;

	// Token: 0x04000C45 RID: 3141
	public OnDataChangedEvent OnInnerRatioChanged;

	// Token: 0x04000C46 RID: 3142
	public OnDataChangedEvent OnWeaponsChanged;

	// Token: 0x04000C47 RID: 3143
	public OnDataChangedEvent OnUsingWeaponIndexChanged;

	// Token: 0x04000C48 RID: 3144
	public OnDataChangedEvent OnNeigongListChanged;

	// Token: 0x04000C49 RID: 3145
	public OnDataChangedEvent OnAttackSkillListChanged;

	// Token: 0x04000C4A RID: 3146
	public OnDataChangedEvent OnAssistSkillListChanged;

	// Token: 0x04000C4B RID: 3147
	public OnWeaponDataChangedEvent OnWeaponDurabilityChanged;

	// Token: 0x04000C4C RID: 3148
	public OnWeaponDataChangedEvent OnWeaponCanChangeToChanged;

	// Token: 0x04000C4D RID: 3149
	public OnWeaponDataChangedEvent OnWeaponCdFrameChanged;

	// Token: 0x04000C4E RID: 3150
	public OnWeaponDataChangedEvent OnWeaponFixedCdLeftFrameChanged;

	// Token: 0x04000C4F RID: 3151
	public OnWeaponDataChangedEvent OnWeaponFixedCdTotalFrameChanged;

	// Token: 0x04000C50 RID: 3152
	public OnWeaponDataChangedEvent OnWeaponInnerRatioChanged;

	// Token: 0x04000C51 RID: 3153
	public OnCombatSkillDataChangedEvent OnCombatSkillDirectionChanged;

	// Token: 0x04000C52 RID: 3154
	public OnCombatSkillDataChangedEvent OnCombatSkillCanUseChanged;

	// Token: 0x04000C53 RID: 3155
	public OnCombatSkillDataChangedEvent OnCombatSkillLeftCdFrameChanged;

	// Token: 0x04000C54 RID: 3156
	public OnCombatSkillDataChangedEvent OnCombatSkillTotalCdFrameChanged;

	// Token: 0x04000C55 RID: 3157
	public OnCombatSkillDataChangedEvent OnCombatSkillBanReasonChanged;

	// Token: 0x04000C56 RID: 3158
	public OnCombatSkillDataChangedEvent OnCombatSkillEffectDataChanged;

	// Token: 0x04000C57 RID: 3159
	public OnCharacterDataChangedEvent OnCharacterTemplateIdChanged;

	// Token: 0x04000C58 RID: 3160
	public OnCharacterDataChangedEvent OnCharacterVisibleChanged;

	// Token: 0x04000C59 RID: 3161
	public OnDataChangedEvent OnAllocatedNeiliEffectsChanged;

	// Token: 0x04000C5A RID: 3162
	public OnDataChangedEvent OnDisorderOfQiChanged;

	// Token: 0x04000C5B RID: 3163
	public OnDataChangedEvent OnNeiliAllocationChanged;

	// Token: 0x04000C5C RID: 3164
	public OnDataChangedEvent OnOriginNeiliAllocationChanged;

	// Token: 0x04000C5D RID: 3165
	public OnDataChangedEvent OnDataNeiliAllocationCdChanged;

	// Token: 0x04000C5E RID: 3166
	public OnDataChangedEvent OnBreathValueChanged;

	// Token: 0x04000C5F RID: 3167
	public OnDataChangedEvent OnStanceValueChanged;

	// Token: 0x04000C60 RID: 3168
	public OnDataChangedEvent OnNormalAttackRecoveryChanged;

	// Token: 0x04000C61 RID: 3169
	public OnDataChangedEvent OnAttackRangeChanged;

	// Token: 0x04000C62 RID: 3170
	public OnDataChangedEvent OnWeaponTricksChanged;

	// Token: 0x04000C63 RID: 3171
	public OnDataChangedEvent OnWeaponTrickIndexChanged;

	// Token: 0x04000C64 RID: 3172
	public OnDataChangedEvent OnTricksChanged;

	// Token: 0x04000C65 RID: 3173
	public OnDataChangedEvent OnMaxTrickCountChanged;

	// Token: 0x04000C66 RID: 3174
	public OnDataChangedEvent OnChangeTrickCountChanged;

	// Token: 0x04000C67 RID: 3175
	public OnDataChangedEvent OnChangeTrickProgressChanged;

	// Token: 0x04000C68 RID: 3176
	public OnDataChangedEvent OnCanChangeTrickChanged;

	// Token: 0x04000C69 RID: 3177
	public OnDataChangedEvent OnChangingTrickChanged;

	// Token: 0x04000C6A RID: 3178
	public OnDataChangedEvent OnChangeTrickAttackChanged;

	// Token: 0x04000C6B RID: 3179
	public OnDataChangedEvent OnSkillEffectCollectionChanged;

	// Token: 0x04000C6C RID: 3180
	public OnDataChangedEvent OnPreparingSkillIdChanged;

	// Token: 0x04000C6D RID: 3181
	public OnDataChangedEvent OnSkillPreparePercentChanged;

	// Token: 0x04000C6E RID: 3182
	public OnDataChangedEvent OnPerformingSkillIdChanged;

	// Token: 0x04000C6F RID: 3183
	public OnDataChangedEvent OnAutoCastingSkillChanged;

	// Token: 0x04000C70 RID: 3184
	public OnDataChangedEvent OnPreparingOtherActionChanged;

	// Token: 0x04000C71 RID: 3185
	public OnDataChangedEvent OnPreparingOtherActionInterruptTypeChanged;

	// Token: 0x04000C72 RID: 3186
	public OnDataChangedEvent OnOtherActionPreparePercentChanged;

	// Token: 0x04000C73 RID: 3187
	public OnDataChangedEvent OnOtherActionCanUseChanged;

	// Token: 0x04000C74 RID: 3188
	public OnDataChangedEvent OnHealInjuryCountChanged;

	// Token: 0x04000C75 RID: 3189
	public OnDataChangedEvent OnHealPoisonCountChanged;

	// Token: 0x04000C76 RID: 3190
	public OnDataChangedEvent OnCanUseItemChanged;

	// Token: 0x04000C77 RID: 3191
	public OnDataChangedEvent OnPreparingItemChanged;

	// Token: 0x04000C78 RID: 3192
	public OnDataChangedEvent OnUseItemPreparePercentChanged;

	// Token: 0x04000C79 RID: 3193
	public OnCharacterDataChangedEvent<DefeatMarkCollection> OnDefeatMarkCollectionChanged;

	// Token: 0x04000C7A RID: 3194
	public OnCharacterDataChangedEvent OnCurrTeammateCommandsChanged;

	// Token: 0x04000C7B RID: 3195
	public OnCharacterDataChangedEvent OnTeammateCommandCanUseChanged;

	// Token: 0x04000C7C RID: 3196
	public OnCharacterDataChangedEvent OnShowTransferInjuryCommandChanged;

	// Token: 0x04000C7D RID: 3197
	public OnCharacterDataChangedEvent OnTeammateCommandBanReasonsChanged;

	// Token: 0x04000C7E RID: 3198
	public OnCharacterDataChangedEvent<sbyte> OnExecutingTeammateCommandChanged;

	// Token: 0x04000C7F RID: 3199
	public OnCharacterDataChangedEvent OnTeammateCommandCdChanged;

	// Token: 0x04000C80 RID: 3200
	public OnCharacterDataChangedEvent OnTeammateCommandPreparePercentChanged;

	// Token: 0x04000C81 RID: 3201
	public OnCharacterDataChangedEvent OnTeammateCommandTimePercentChanged;

	// Token: 0x04000C82 RID: 3202
	public OnCharacterDataChangedEvent OnAttackCommandWeaponKeyChanged;

	// Token: 0x04000C83 RID: 3203
	public OnCharacterDataChangedEvent OnAttackCommandTrickTypeChanged;

	// Token: 0x04000C84 RID: 3204
	public OnCharacterDataChangedEvent OnAttackCommandSkillIdChanged;

	// Token: 0x04000C85 RID: 3205
	public OnCharacterDataChangedEvent OnDefendCommandSkillIdChanged;

	// Token: 0x04000C86 RID: 3206
	public OnDataChangedEvent OnHaveLeftArmChanged;

	// Token: 0x04000C87 RID: 3207
	public OnDataChangedEvent OnHaveRightArmChanged;

	// Token: 0x04000C88 RID: 3208
	public OnDataChangedEvent OnHaveLeftLegChanged;

	// Token: 0x04000C89 RID: 3209
	public OnDataChangedEvent OnHaveRightLegChanged;

	// Token: 0x04000C8A RID: 3210
	public OnDataChangedEvent OnHitValuesChanged;

	// Token: 0x04000C8B RID: 3211
	public OnDataChangedEvent OnAvoidValuesChanged;

	// Token: 0x04000C8C RID: 3212
	public OnDataChangedEvent OnOuterDamageValueChanged;

	// Token: 0x04000C8D RID: 3213
	public OnDataChangedEvent OnInnerDamageValueChanged;

	// Token: 0x04000C8E RID: 3214
	public OnDataChangedEvent OnOldInjuriesChanged;

	// Token: 0x04000C8F RID: 3215
	public OnDataChangedEvent OnOldPoisonChanged;

	// Token: 0x04000C90 RID: 3216
	public OnDataChangedEvent OnOldDisorderOfQiChanged;

	// Token: 0x04000C91 RID: 3217
	public OnCharacterDataChangedEvent<CombatReserveData> OnCombatReserveDataChanged;

	// Token: 0x04000C92 RID: 3218
	public OnDataChangedEvent OnReserveNormalAttack;

	// Token: 0x04000C93 RID: 3219
	public OnDataChangedEvent OnGangqiChanged;

	// Token: 0x04000C94 RID: 3220
	public OnDataChangedEvent OnGangqiMaxChanged;

	// Token: 0x04000C95 RID: 3221
	public OnCharacterDataChangedEvent OnParticleToPlayChanged;

	// Token: 0x04000C96 RID: 3222
	public OnCharacterDataChangedEvent OnParticleToLoopChanged;

	// Token: 0x04000C97 RID: 3223
	public OnCharacterDataChangedEvent OnParticleToLoopByCombatSkillChanged;

	// Token: 0x04000C98 RID: 3224
	public OnCharacterDataChangedEvent OnAnimationToPlayOnceChanged;

	// Token: 0x04000C99 RID: 3225
	public OnCharacterDataChangedEvent OnSoundToLoopChanged;

	// Token: 0x04000C9A RID: 3226
	public OnDataChangedEvent OnMindRhythmChanged;

	// Token: 0x04000C9B RID: 3227
	public OnDataChangedEvent OnMindUpheavalTimeChanged;

	// Token: 0x04000C9C RID: 3228
	public OnDataChangedEvent OnMindUpheavalChanged;

	// Token: 0x04000C9D RID: 3229
	public OnDataChangedEvent OnBossPhaseChanged;

	// Token: 0x04000CA0 RID: 3232
	public TeammateCommandChangeData CommandChangeData;

	// Token: 0x04000CA1 RID: 3233
	public sbyte FirstMoveType;

	// Token: 0x04000CA4 RID: 3236
	private short _configId;

	// Token: 0x04000CA5 RID: 3237
	private short _sceneId;

	// Token: 0x04000CA6 RID: 3238
	private readonly List<int> _selfTeam = new List<int>();

	// Token: 0x04000CA7 RID: 3239
	private readonly List<int> _enemyTeam = new List<int>();

	// Token: 0x04000CA8 RID: 3240
	private readonly Dictionary<int, CharacterDisplayData> _displayDataCache = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x04000CA9 RID: 3241
	private readonly Dictionary<int, List<ItemDisplayData>> _equipmentDataCache = new Dictionary<int, List<ItemDisplayData>>();

	// Token: 0x04000CAA RID: 3242
	private readonly Dictionary<int, IReadOnlyList<CombatSkillDisplayData>> _proactiveSkillData = new Dictionary<int, IReadOnlyList<CombatSkillDisplayData>>();

	// Token: 0x04000CAB RID: 3243
	private readonly Dictionary<int, List<short>> _orderedProactiveSkillList = new Dictionary<int, List<short>>();

	// Token: 0x04000CAE RID: 3246
	private CombatSkillDisplayData _previewCostSkillData;

	// Token: 0x04000CAF RID: 3247
	private List<CastBoostEffectDisplayData> _costNeiliEffects = new List<CastBoostEffectDisplayData>();

	// Token: 0x04000CB0 RID: 3248
	private bool _canCostTrickDuringPreparingSkill;

	// Token: 0x04000CB1 RID: 3249
	private IReadOnlyList<CombatNotifyHandler> _handlerData;

	// Token: 0x04000CB2 RID: 3250
	private int _listenerId = -1;

	// Token: 0x04000CB3 RID: 3251
	private DispatcherInstance _dispatcher;

	// Token: 0x04000CB4 RID: 3252
	private readonly List<DataUid> _monitorUids = new List<DataUid>();

	// Token: 0x04000CB5 RID: 3253
	private readonly List<CombatSubProcessor> _subProcessors = new List<CombatSubProcessor>();

	// Token: 0x04000CB6 RID: 3254
	private readonly List<DataUid> _unMonitorTemporaryUids = new List<DataUid>();

	// Token: 0x04000CB7 RID: 3255
	private readonly int[] _requestTemporaryArray = new int[4];

	// Token: 0x04000CB8 RID: 3256
	private readonly Dictionary<ECombatEvents, List<OnCombatEvent>> _combatEvents = new Dictionary<ECombatEvents, List<OnCombatEvent>>();

	// Token: 0x04000CB9 RID: 3257
	private bool _raisingEvent;
}
