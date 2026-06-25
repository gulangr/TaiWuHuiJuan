using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;

namespace GameData.Domains.Item
{
	// Token: 0x02000FC7 RID: 4039
	public static class ItemDomainMethod
	{
		// Token: 0x02002609 RID: 9737
		public static class Call
		{
			// Token: 0x06011294 RID: 70292 RVA: 0x0067A056 File Offset: 0x00678256
			public static void IdentifyPoisons(int listenerId, int charId, ItemDisplayData itemDisplayData)
			{
				GameDataBridge.AddMethodCall<int, ItemDisplayData>(listenerId, 6, 0, charId, itemDisplayData);
			}

			// Token: 0x06011295 RID: 70293 RVA: 0x0067A064 File Offset: 0x00678264
			public static void CatchCricket(int listenerId, short colorId, short partId, short singLevel, sbyte cricketPlaceId)
			{
				GameDataBridge.AddMethodCall<short, short, short, sbyte>(listenerId, 6, 1, colorId, partId, singLevel, cricketPlaceId);
			}

			// Token: 0x06011296 RID: 70294 RVA: 0x0067A075 File Offset: 0x00678275
			public static void GetCricketData(int listenerId, int itemId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 6, 2, itemId);
			}

			// Token: 0x06011297 RID: 70295 RVA: 0x0067A082 File Offset: 0x00678282
			public static void SetCricketRecord(int itemId, bool win, int enemyItemId)
			{
				GameDataBridge.AddMethodCall<int, bool, int>(-1, 6, 3, itemId, win, enemyItemId);
			}

			// Token: 0x06011298 RID: 70296 RVA: 0x0067A091 File Offset: 0x00678291
			public static void AddCricketInjury(int itemId, int index, short value)
			{
				GameDataBridge.AddMethodCall<int, int, short>(-1, 6, 4, itemId, index, value);
			}

			// Token: 0x06011299 RID: 70297 RVA: 0x0067A0A0 File Offset: 0x006782A0
			public static void GetWeaponTricks(int listenerId, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 5, weaponKey);
			}

			// Token: 0x0601129A RID: 70298 RVA: 0x0067A0AD File Offset: 0x006782AD
			public static void GetCricketCombatRecords(int listenerId, ItemKey cricketKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 6, cricketKey);
			}

			// Token: 0x0601129B RID: 70299 RVA: 0x0067A0BA File Offset: 0x006782BA
			public static void GetItemDisplayData(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 7, itemKey);
			}

			// Token: 0x0601129C RID: 70300 RVA: 0x0067A0C7 File Offset: 0x006782C7
			public static void GetItemDisplayData(int listenerId, ItemKey itemKey, int charId)
			{
				GameDataBridge.AddMethodCall<ItemKey, int>(listenerId, 6, 7, itemKey, charId);
			}

			// Token: 0x0601129D RID: 70301 RVA: 0x0067A0D5 File Offset: 0x006782D5
			public static void GetItemDisplayDataList(int listenerId, List<ItemKey> itemKeyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 6, 8, itemKeyList);
			}

			// Token: 0x0601129E RID: 70302 RVA: 0x0067A0E2 File Offset: 0x006782E2
			public static void GetItemDisplayDataList(int listenerId, List<ItemKey> itemKeyList, int charId)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, int>(listenerId, 6, 8, itemKeyList, charId);
			}

			// Token: 0x0601129F RID: 70303 RVA: 0x0067A0F0 File Offset: 0x006782F0
			public static void GetSkillBookPagesInfo(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 9, itemKey);
			}

			// Token: 0x060112A0 RID: 70304 RVA: 0x0067A0FE File Offset: 0x006782FE
			public static void GetValue(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 10, itemKey);
			}

			// Token: 0x060112A1 RID: 70305 RVA: 0x0067A10C File Offset: 0x0067830C
			public static void GetPrice(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 11, itemKey);
			}

			// Token: 0x060112A2 RID: 70306 RVA: 0x0067A11A File Offset: 0x0067831A
			public static void DisassembleItem(int listenerId, int charId, ItemKey itemKey, ItemKey toolKey, sbyte itemSourceType, sbyte toolSourceType)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, ItemKey, sbyte, sbyte>(listenerId, 6, 12, charId, itemKey, toolKey, itemSourceType, toolSourceType);
			}

			// Token: 0x060112A3 RID: 70307 RVA: 0x0067A12E File Offset: 0x0067832E
			public static void DiscardItem(int charId, ItemKey itemKey, sbyte itemSourceType)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, sbyte>(-1, 6, 13, charId, itemKey, itemSourceType);
			}

			// Token: 0x060112A4 RID: 70308 RVA: 0x0067A13E File Offset: 0x0067833E
			public static void DiscardItem(int charId, ItemKey itemKey, sbyte itemSourceType, int count)
			{
				GameDataBridge.AddMethodCall<int, ItemKey, sbyte, int>(-1, 6, 13, charId, itemKey, itemSourceType, count);
			}

			// Token: 0x060112A5 RID: 70309 RVA: 0x0067A14F File Offset: 0x0067834F
			public static void GetRepairableItems(int listenerId, int charId, ItemKey toolKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 6, 14, charId, toolKey);
			}

			// Token: 0x060112A6 RID: 70310 RVA: 0x0067A15E File Offset: 0x0067835E
			public static void GetDisassemblableItems(int listenerId, int charId, ItemKey toolKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 6, 15, charId, toolKey);
			}

			// Token: 0x060112A7 RID: 70311 RVA: 0x0067A16D File Offset: 0x0067836D
			public static void ChangeDurability(int charId, short changeValue, sbyte itemType, short startId, short endId)
			{
				GameDataBridge.AddMethodCall<int, short, sbyte, short, short>(-1, 6, 16, charId, changeValue, itemType, startId, endId);
			}

			// Token: 0x060112A8 RID: 70312 RVA: 0x0067A180 File Offset: 0x00678380
			public static void ChangePoisonIdentified(int charId, bool isIdentified)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 6, 17, charId, isIdentified);
			}

			// Token: 0x060112A9 RID: 70313 RVA: 0x0067A18F File Offset: 0x0067838F
			public static void DiscardItemList(int charId, List<ItemKey> keyList, sbyte itemSourceType)
			{
				GameDataBridge.AddMethodCall<int, List<ItemKey>, sbyte>(-1, 6, 18, charId, keyList, itemSourceType);
			}

			// Token: 0x060112AA RID: 70314 RVA: 0x0067A19F File Offset: 0x0067839F
			public static void DisassembleItemList(int listenerId, int charId, List<MultiplyOperation> operationList)
			{
				GameDataBridge.AddMethodCall<int, List<MultiplyOperation>>(listenerId, 6, 19, charId, operationList);
			}

			// Token: 0x060112AB RID: 70315 RVA: 0x0067A1AE File Offset: 0x006783AE
			public static void SetCricketBattleConfig(sbyte minGrade, sbyte maxGrade, bool onlyNoInjuryCricket)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte, bool>(-1, 6, 20, minGrade, maxGrade, onlyNoInjuryCricket);
			}

			// Token: 0x060112AC RID: 70316 RVA: 0x0067A1BE File Offset: 0x006783BE
			public static void GetCricketDataList(int listenerId, List<ItemKey> itemList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 6, 21, itemList);
			}

			// Token: 0x060112AD RID: 70317 RVA: 0x0067A1CC File Offset: 0x006783CC
			public static void GetWeaponAttackRange(int listenerId, int charId, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 6, 22, charId, weaponKey);
			}

			// Token: 0x060112AE RID: 70318 RVA: 0x0067A1DB File Offset: 0x006783DB
			public static void GetCricketsAliveState(int listenerId, List<ItemKey> keyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 6, 23, keyList);
			}

			// Token: 0x060112AF RID: 70319 RVA: 0x0067A1E9 File Offset: 0x006783E9
			public static void ModifyCombatSkillBookPageNormal(int listenerId, ItemKey itemKey, List<byte> pageIds, List<sbyte> directions)
			{
				GameDataBridge.AddMethodCall<ItemKey, List<byte>, List<sbyte>>(listenerId, 6, 24, itemKey, pageIds, directions);
			}

			// Token: 0x060112B0 RID: 70320 RVA: 0x0067A1F9 File Offset: 0x006783F9
			public static void ModifyCombatSkillBookPageOutline(int listenerId, ItemKey itemKey, sbyte behaviorType)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte>(listenerId, 6, 25, itemKey, behaviorType);
			}

			// Token: 0x060112B1 RID: 70321 RVA: 0x0067A208 File Offset: 0x00678408
			public static void GetTaiwuInventoryCombatSkillBooks(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 6, 26);
			}

			// Token: 0x060112B2 RID: 70322 RVA: 0x0067A215 File Offset: 0x00678415
			public static void GetItemDisplayDataListOptional(int listenerId, List<ItemKey> itemKeyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 6, 27, itemKeyList);
			}

			// Token: 0x060112B3 RID: 70323 RVA: 0x0067A223 File Offset: 0x00678423
			public static void GetItemDisplayDataListOptional(int listenerId, List<ItemKey> itemKeyList, int charId)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, int>(listenerId, 6, 27, itemKeyList, charId);
			}

			// Token: 0x060112B4 RID: 70324 RVA: 0x0067A232 File Offset: 0x00678432
			public static void GetItemDisplayDataListOptional(int listenerId, List<ItemKey> itemKeyList, int charId, sbyte itemSourceType)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, int, sbyte>(listenerId, 6, 27, itemKeyList, charId, itemSourceType);
			}

			// Token: 0x060112B5 RID: 70325 RVA: 0x0067A242 File Offset: 0x00678442
			public static void GetItemDisplayDataListOptional(int listenerId, List<ItemKey> itemKeyList, int charId, sbyte itemSourceType, bool merge)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, int, sbyte, bool>(listenerId, 6, 27, itemKeyList, charId, itemSourceType, merge);
			}

			// Token: 0x060112B6 RID: 70326 RVA: 0x0067A254 File Offset: 0x00678454
			public static void GetSkillBookPageDisplayDataList(int listenerId, List<ItemKey> itemKeyList)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>>(listenerId, 6, 28, itemKeyList);
			}

			// Token: 0x060112B7 RID: 70327 RVA: 0x0067A262 File Offset: 0x00678462
			public static void GetEmptyToolKey(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 6, 29);
			}

			// Token: 0x060112B8 RID: 70328 RVA: 0x0067A26F File Offset: 0x0067846F
			public static void GetItemDisplayDataListOptionalFromInventory(int listenerId, Inventory inventory)
			{
				GameDataBridge.AddMethodCall<Inventory>(listenerId, 6, 30, inventory);
			}

			// Token: 0x060112B9 RID: 70329 RVA: 0x0067A27D File Offset: 0x0067847D
			public static void GetItemDisplayDataListOptionalFromInventory(int listenerId, Inventory inventory, int charId)
			{
				GameDataBridge.AddMethodCall<Inventory, int>(listenerId, 6, 30, inventory, charId);
			}

			// Token: 0x060112BA RID: 70330 RVA: 0x0067A28C File Offset: 0x0067848C
			public static void GetItemDisplayDataListOptionalFromInventory(int listenerId, Inventory inventory, int charId, sbyte itemSourceType)
			{
				GameDataBridge.AddMethodCall<Inventory, int, sbyte>(listenerId, 6, 30, inventory, charId, itemSourceType);
			}

			// Token: 0x060112BB RID: 70331 RVA: 0x0067A29C File Offset: 0x0067849C
			public static void GetItemDisplayDataListOptionalFromInventory(int listenerId, Inventory inventory, int charId, sbyte itemSourceType, bool merge)
			{
				GameDataBridge.AddMethodCall<Inventory, int, sbyte, bool>(listenerId, 6, 30, inventory, charId, itemSourceType, merge);
			}

			// Token: 0x060112BC RID: 70332 RVA: 0x0067A2AE File Offset: 0x006784AE
			public static void SettlementCricketWager(int listenerId, bool win, ItemKey[] taiwuCricketKeys, short[] durabilityList)
			{
				GameDataBridge.AddMethodCall<bool, ItemKey[], short[]>(listenerId, 6, 31, win, taiwuCricketKeys, durabilityList);
			}

			// Token: 0x060112BD RID: 70333 RVA: 0x0067A2BE File Offset: 0x006784BE
			public static void SettlementCricketWager(int listenerId, bool win, ItemKey[] taiwuCricketKeys, short[] durabilityList, bool invokeExtraWager)
			{
				GameDataBridge.AddMethodCall<bool, ItemKey[], short[], bool>(listenerId, 6, 31, win, taiwuCricketKeys, durabilityList, invokeExtraWager);
			}

			// Token: 0x060112BE RID: 70334 RVA: 0x0067A2D0 File Offset: 0x006784D0
			public static void GmCmd_StartCricketCombat(int enemyId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 6, 32, enemyId);
			}

			// Token: 0x060112BF RID: 70335 RVA: 0x0067A2DE File Offset: 0x006784DE
			public static void SettlementCricketWagerByGiveUp(int listenerId, bool win)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 6, 33, win);
			}

			// Token: 0x060112C0 RID: 70336 RVA: 0x0067A2EC File Offset: 0x006784EC
			public static void SettlementCricketWagerByGiveUp(int listenerId, bool win, bool invokeExtraWager)
			{
				GameDataBridge.AddMethodCall<bool, bool>(listenerId, 6, 33, win, invokeExtraWager);
			}

			// Token: 0x060112C1 RID: 70337 RVA: 0x0067A2FB File Offset: 0x006784FB
			public static void MakeCricketRebirth(ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(-1, 6, 34, itemKey);
			}

			// Token: 0x060112C2 RID: 70338 RVA: 0x0067A309 File Offset: 0x00678509
			public static void GetRepairItemNeedResourceCount(int listenerId, ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 6, 35, itemKey);
			}

			// Token: 0x060112C3 RID: 70339 RVA: 0x0067A317 File Offset: 0x00678517
			public static void GetRepairItemNeedResourceCount(int listenerId, ItemKey itemKey, short targetDurability)
			{
				GameDataBridge.AddMethodCall<ItemKey, short>(listenerId, 6, 35, itemKey, targetDurability);
			}

			// Token: 0x060112C4 RID: 70340 RVA: 0x0067A326 File Offset: 0x00678526
			public static void SetCombatSkillBookPage(int listenerId, ItemKey itemKey, sbyte behaviorType, List<sbyte> directions)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte, List<sbyte>>(listenerId, 6, 36, itemKey, behaviorType, directions);
			}

			// Token: 0x060112C5 RID: 70341 RVA: 0x0067A336 File Offset: 0x00678536
			public static void GetWeaponPrepareFrame(int listenerId, int charId, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<int, ItemKey>(listenerId, 6, 37, charId, weaponKey);
			}

			// Token: 0x060112C6 RID: 70342 RVA: 0x0067A345 File Offset: 0x00678545
			public static void GmCmd_ChangeAllMysteryCompatibility(int deltaValue)
			{
				GameDataBridge.AddMethodCall<int>(-1, 6, 38, deltaValue);
			}

			// Token: 0x060112C7 RID: 70343 RVA: 0x0067A353 File Offset: 0x00678553
			public static void GmCmd_ChangeAllCricketSpirit(int addValue)
			{
				GameDataBridge.AddMethodCall<int>(-1, 6, 39, addValue);
			}

			// Token: 0x060112C8 RID: 70344 RVA: 0x0067A361 File Offset: 0x00678561
			public static void SetCricketName(ItemKey itemKey, string name)
			{
				GameDataBridge.AddMethodCall<ItemKey, string>(-1, 6, 40, itemKey, name);
			}

			// Token: 0x060112C9 RID: 70345 RVA: 0x0067A370 File Offset: 0x00678570
			public static void DiscardItemInventory(int charId, Inventory inventory, sbyte itemSourceType)
			{
				GameDataBridge.AddMethodCall<int, Inventory, sbyte>(-1, 6, 41, charId, inventory, itemSourceType);
			}
		}

		// Token: 0x0200260A RID: 9738
		public static class AsyncCall
		{
			// Token: 0x060112CA RID: 70346 RVA: 0x0067A380 File Offset: 0x00678580
			public static void IdentifyPoisons(IAsyncMethodRequestHandler requestHandler, int charId, ItemDisplayData itemDisplayData, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemDisplayData>(6, 0, charId, itemDisplayData, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112CB RID: 70347 RVA: 0x0067A3AC File Offset: 0x006785AC
			public static void CatchCricket(IAsyncMethodRequestHandler requestHandler, short colorId, short partId, short singLevel, sbyte cricketPlaceId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short, short, sbyte>(6, 1, colorId, partId, singLevel, cricketPlaceId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112CC RID: 70348 RVA: 0x0067A3DC File Offset: 0x006785DC
			public static void GetCricketData(IAsyncMethodRequestHandler requestHandler, int itemId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(6, 2, itemId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112CD RID: 70349 RVA: 0x0067A406 File Offset: 0x00678606
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.SetCricketRecord instead.", true)]
			public static void SetCricketRecord(IAsyncMethodRequestHandler requestHandler, int itemId, bool win, int enemyItemId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112CE RID: 70350 RVA: 0x0067A40E File Offset: 0x0067860E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.AddCricketInjury instead.", true)]
			public static void AddCricketInjury(IAsyncMethodRequestHandler requestHandler, int itemId, int index, short value, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112CF RID: 70351 RVA: 0x0067A418 File Offset: 0x00678618
			public static void GetWeaponTricks(IAsyncMethodRequestHandler requestHandler, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 5, weaponKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D0 RID: 70352 RVA: 0x0067A444 File Offset: 0x00678644
			public static void GetCricketCombatRecords(IAsyncMethodRequestHandler requestHandler, ItemKey cricketKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 6, cricketKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D1 RID: 70353 RVA: 0x0067A470 File Offset: 0x00678670
			public static void GetItemDisplayData(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 7, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D2 RID: 70354 RVA: 0x0067A49C File Offset: 0x0067869C
			public static void GetItemDisplayData(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, int>(6, 7, itemKey, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D3 RID: 70355 RVA: 0x0067A4C8 File Offset: 0x006786C8
			public static void GetItemDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(6, 8, itemKeyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D4 RID: 70356 RVA: 0x0067A4F4 File Offset: 0x006786F4
			public static void GetItemDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>, int>(6, 8, itemKeyList, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D5 RID: 70357 RVA: 0x0067A520 File Offset: 0x00678720
			public static void GetSkillBookPagesInfo(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 9, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D6 RID: 70358 RVA: 0x0067A54C File Offset: 0x0067874C
			public static void GetValue(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 10, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D7 RID: 70359 RVA: 0x0067A578 File Offset: 0x00678778
			public static void GetPrice(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 11, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D8 RID: 70360 RVA: 0x0067A5A4 File Offset: 0x006787A4
			public static void DisassembleItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, ItemKey toolKey, sbyte itemSourceType, sbyte toolSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey, ItemKey, sbyte, sbyte>(6, 12, charId, itemKey, toolKey, itemSourceType, toolSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112D9 RID: 70361 RVA: 0x0067A5D6 File Offset: 0x006787D6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.DiscardItem instead.", true)]
			public static void DiscardItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, sbyte itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112DA RID: 70362 RVA: 0x0067A5DE File Offset: 0x006787DE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.DiscardItem instead.", true)]
			public static void DiscardItem(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey itemKey, sbyte itemSourceType, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112DB RID: 70363 RVA: 0x0067A5E8 File Offset: 0x006787E8
			public static void GetRepairableItems(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey toolKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(6, 14, charId, toolKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112DC RID: 70364 RVA: 0x0067A614 File Offset: 0x00678814
			public static void GetDisassemblableItems(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey toolKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(6, 15, charId, toolKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112DD RID: 70365 RVA: 0x0067A640 File Offset: 0x00678840
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.ChangeDurability instead.", true)]
			public static void ChangeDurability(IAsyncMethodRequestHandler requestHandler, int charId, short changeValue, sbyte itemType, short startId, short endId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112DE RID: 70366 RVA: 0x0067A648 File Offset: 0x00678848
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.ChangePoisonIdentified instead.", true)]
			public static void ChangePoisonIdentified(IAsyncMethodRequestHandler requestHandler, int charId, bool isIdentified, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112DF RID: 70367 RVA: 0x0067A650 File Offset: 0x00678850
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.DiscardItemList instead.", true)]
			public static void DiscardItemList(IAsyncMethodRequestHandler requestHandler, int charId, List<ItemKey> keyList, sbyte itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112E0 RID: 70368 RVA: 0x0067A658 File Offset: 0x00678858
			public static void DisassembleItemList(IAsyncMethodRequestHandler requestHandler, int charId, List<MultiplyOperation> operationList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, List<MultiplyOperation>>(6, 19, charId, operationList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E1 RID: 70369 RVA: 0x0067A684 File Offset: 0x00678884
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.SetCricketBattleConfig instead.", true)]
			public static void SetCricketBattleConfig(IAsyncMethodRequestHandler requestHandler, sbyte minGrade, sbyte maxGrade, bool onlyNoInjuryCricket, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112E2 RID: 70370 RVA: 0x0067A68C File Offset: 0x0067888C
			public static void GetCricketDataList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(6, 21, itemList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E3 RID: 70371 RVA: 0x0067A6B8 File Offset: 0x006788B8
			public static void GetWeaponAttackRange(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(6, 22, charId, weaponKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E4 RID: 70372 RVA: 0x0067A6E4 File Offset: 0x006788E4
			public static void GetCricketsAliveState(IAsyncMethodRequestHandler requestHandler, List<ItemKey> keyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(6, 23, keyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E5 RID: 70373 RVA: 0x0067A710 File Offset: 0x00678910
			public static void ModifyCombatSkillBookPageNormal(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, List<byte> pageIds, List<sbyte> directions, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, List<byte>, List<sbyte>>(6, 24, itemKey, pageIds, directions, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E6 RID: 70374 RVA: 0x0067A740 File Offset: 0x00678940
			public static void ModifyCombatSkillBookPageOutline(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte behaviorType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, sbyte>(6, 25, itemKey, behaviorType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E7 RID: 70375 RVA: 0x0067A76C File Offset: 0x0067896C
			public static void GetTaiwuInventoryCombatSkillBooks(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(6, 26, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E8 RID: 70376 RVA: 0x0067A798 File Offset: 0x00678998
			public static void GetItemDisplayDataListOptional(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(6, 27, itemKeyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112E9 RID: 70377 RVA: 0x0067A7C4 File Offset: 0x006789C4
			public static void GetItemDisplayDataListOptional(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>, int>(6, 27, itemKeyList, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112EA RID: 70378 RVA: 0x0067A7F0 File Offset: 0x006789F0
			public static void GetItemDisplayDataListOptional(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, int charId, sbyte itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>, int, sbyte>(6, 27, itemKeyList, charId, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112EB RID: 70379 RVA: 0x0067A820 File Offset: 0x00678A20
			public static void GetItemDisplayDataListOptional(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, int charId, sbyte itemSourceType, bool merge, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>, int, sbyte, bool>(6, 27, itemKeyList, charId, itemSourceType, merge, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112EC RID: 70380 RVA: 0x0067A850 File Offset: 0x00678A50
			public static void GetSkillBookPageDisplayDataList(IAsyncMethodRequestHandler requestHandler, List<ItemKey> itemKeyList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<ItemKey>>(6, 28, itemKeyList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112ED RID: 70381 RVA: 0x0067A87C File Offset: 0x00678A7C
			public static void GetEmptyToolKey(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(6, 29, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112EE RID: 70382 RVA: 0x0067A8A8 File Offset: 0x00678AA8
			public static void GetItemDisplayDataListOptionalFromInventory(IAsyncMethodRequestHandler requestHandler, Inventory inventory, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Inventory>(6, 30, inventory, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112EF RID: 70383 RVA: 0x0067A8D4 File Offset: 0x00678AD4
			public static void GetItemDisplayDataListOptionalFromInventory(IAsyncMethodRequestHandler requestHandler, Inventory inventory, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Inventory, int>(6, 30, inventory, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F0 RID: 70384 RVA: 0x0067A900 File Offset: 0x00678B00
			public static void GetItemDisplayDataListOptionalFromInventory(IAsyncMethodRequestHandler requestHandler, Inventory inventory, int charId, sbyte itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Inventory, int, sbyte>(6, 30, inventory, charId, itemSourceType, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F1 RID: 70385 RVA: 0x0067A930 File Offset: 0x00678B30
			public static void GetItemDisplayDataListOptionalFromInventory(IAsyncMethodRequestHandler requestHandler, Inventory inventory, int charId, sbyte itemSourceType, bool merge, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<Inventory, int, sbyte, bool>(6, 30, inventory, charId, itemSourceType, merge, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F2 RID: 70386 RVA: 0x0067A960 File Offset: 0x00678B60
			public static void SettlementCricketWager(IAsyncMethodRequestHandler requestHandler, bool win, ItemKey[] taiwuCricketKeys, short[] durabilityList, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, ItemKey[], short[]>(6, 31, win, taiwuCricketKeys, durabilityList, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F3 RID: 70387 RVA: 0x0067A990 File Offset: 0x00678B90
			public static void SettlementCricketWager(IAsyncMethodRequestHandler requestHandler, bool win, ItemKey[] taiwuCricketKeys, short[] durabilityList, bool invokeExtraWager, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, ItemKey[], short[], bool>(6, 31, win, taiwuCricketKeys, durabilityList, invokeExtraWager, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F4 RID: 70388 RVA: 0x0067A9C0 File Offset: 0x00678BC0
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.GmCmd_StartCricketCombat instead.", true)]
			public static void GmCmd_StartCricketCombat(IAsyncMethodRequestHandler requestHandler, int enemyId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112F5 RID: 70389 RVA: 0x0067A9C8 File Offset: 0x00678BC8
			public static void SettlementCricketWagerByGiveUp(IAsyncMethodRequestHandler requestHandler, bool win, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(6, 33, win, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F6 RID: 70390 RVA: 0x0067A9F4 File Offset: 0x00678BF4
			public static void SettlementCricketWagerByGiveUp(IAsyncMethodRequestHandler requestHandler, bool win, bool invokeExtraWager, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, bool>(6, 33, win, invokeExtraWager, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F7 RID: 70391 RVA: 0x0067AA20 File Offset: 0x00678C20
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.MakeCricketRebirth instead.", true)]
			public static void MakeCricketRebirth(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112F8 RID: 70392 RVA: 0x0067AA28 File Offset: 0x00678C28
			public static void GetRepairItemNeedResourceCount(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(6, 35, itemKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112F9 RID: 70393 RVA: 0x0067AA54 File Offset: 0x00678C54
			public static void GetRepairItemNeedResourceCount(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, short targetDurability, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, short>(6, 35, itemKey, targetDurability, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112FA RID: 70394 RVA: 0x0067AA80 File Offset: 0x00678C80
			public static void SetCombatSkillBookPage(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte behaviorType, List<sbyte> directions, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey, sbyte, List<sbyte>>(6, 36, itemKey, behaviorType, directions, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112FB RID: 70395 RVA: 0x0067AAB0 File Offset: 0x00678CB0
			public static void GetWeaponPrepareFrame(IAsyncMethodRequestHandler requestHandler, int charId, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, ItemKey>(6, 37, charId, weaponKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060112FC RID: 70396 RVA: 0x0067AADC File Offset: 0x00678CDC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.GmCmd_ChangeAllMysteryCompatibility instead.", true)]
			public static void GmCmd_ChangeAllMysteryCompatibility(IAsyncMethodRequestHandler requestHandler, int deltaValue, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112FD RID: 70397 RVA: 0x0067AAE4 File Offset: 0x00678CE4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.GmCmd_ChangeAllCricketSpirit instead.", true)]
			public static void GmCmd_ChangeAllCricketSpirit(IAsyncMethodRequestHandler requestHandler, int addValue, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112FE RID: 70398 RVA: 0x0067AAEC File Offset: 0x00678CEC
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.SetCricketName instead.", true)]
			public static void SetCricketName(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, string name, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060112FF RID: 70399 RVA: 0x0067AAF4 File Offset: 0x00678CF4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use ItemDomainMethod.Call.DiscardItemInventory instead.", true)]
			public static void DiscardItemInventory(IAsyncMethodRequestHandler requestHandler, int charId, Inventory inventory, sbyte itemSourceType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
