using System;
using GameData.Domains.Item;
using GameData.GameDataBridge;

namespace GameData.Domains.LegendaryBook
{
	// Token: 0x02000FC6 RID: 4038
	public static class LegendaryBookDomainMethod
	{
		// Token: 0x02002607 RID: 9735
		public static class Call
		{
			// Token: 0x06011276 RID: 70262 RVA: 0x00679E78 File Offset: 0x00678078
			public static void GmCmd_GetAllLegendaryBookStates(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 11, 0);
			}

			// Token: 0x06011277 RID: 70263 RVA: 0x00679E85 File Offset: 0x00678085
			public static void GmCmd_GiveAllTaiwuLegendaryBookToRandomNpc()
			{
				GameDataBridge.AddMethodCall(-1, 11, 1);
			}

			// Token: 0x06011278 RID: 70264 RVA: 0x00679E92 File Offset: 0x00678092
			public static void GetLegendaryBookIncrementData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 11, 2);
			}

			// Token: 0x06011279 RID: 70265 RVA: 0x00679E9F File Offset: 0x0067809F
			public static void GetAllLegendaryBooksOwningState(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 11, 3);
			}

			// Token: 0x0601127A RID: 70266 RVA: 0x00679EAC File Offset: 0x006780AC
			public static void GetLegendaryBookPresetDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 11, 4);
			}

			// Token: 0x0601127B RID: 70267 RVA: 0x00679EB9 File Offset: 0x006780B9
			public static void AddLegendaryBookSkillEmptyPreset()
			{
				GameDataBridge.AddMethodCall(-1, 11, 5);
			}

			// Token: 0x0601127C RID: 70268 RVA: 0x00679EC6 File Offset: 0x006780C6
			public static void DuplicateLegendaryBookSkillPreset(sbyte presetIndex)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 11, 6, presetIndex);
			}

			// Token: 0x0601127D RID: 70269 RVA: 0x00679ED4 File Offset: 0x006780D4
			public static void RemoveLegendaryBookSkillPreset(sbyte presetIndex)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 11, 7, presetIndex);
			}

			// Token: 0x0601127E RID: 70270 RVA: 0x00679EE2 File Offset: 0x006780E2
			public static void ResetLegendaryBookSkillPreset(int presetIndex)
			{
				GameDataBridge.AddMethodCall<int>(-1, 11, 8, presetIndex);
			}

			// Token: 0x0601127F RID: 70271 RVA: 0x00679EF0 File Offset: 0x006780F0
			public static void SetLegendaryBookSkillPreset(sbyte presetIndex)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 11, 9, presetIndex);
			}

			// Token: 0x06011280 RID: 70272 RVA: 0x00679EFF File Offset: 0x006780FF
			public static void SetLegendaryBookSkillPreset(sbyte presetIndex, bool forceSet)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 11, 9, presetIndex, forceSet);
			}

			// Token: 0x06011281 RID: 70273 RVA: 0x00679F0F File Offset: 0x0067810F
			public static void SaveLegendaryBookSkillPresetSlotCurrent(sbyte skillType, int index, short skillTemplateId)
			{
				GameDataBridge.AddMethodCall<sbyte, int, short>(-1, 11, 10, skillType, index, skillTemplateId);
			}

			// Token: 0x06011282 RID: 70274 RVA: 0x00679F20 File Offset: 0x00678120
			public static void ResetLegendaryBookBonus(sbyte skillType, bool isYin)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 11, 11, skillType, isYin);
			}

			// Token: 0x06011283 RID: 70275 RVA: 0x00679F30 File Offset: 0x00678130
			public static void SaveLegendaryBookWeaponPresetSlotCurrent(sbyte skillType, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<sbyte, ItemKey>(-1, 11, 12, skillType, weaponKey);
			}

			// Token: 0x06011284 RID: 70276 RVA: 0x00679F40 File Offset: 0x00678140
			public static void GmCmd_AddRandomLegendaryBookContestChar()
			{
				GameDataBridge.AddMethodCall(-1, 11, 13);
			}
		}

		// Token: 0x02002608 RID: 9736
		public static class AsyncCall
		{
			// Token: 0x06011285 RID: 70277 RVA: 0x00679F50 File Offset: 0x00678150
			public static void GmCmd_GetAllLegendaryBookStates(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(11, 0, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011286 RID: 70278 RVA: 0x00679F7A File Offset: 0x0067817A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.GmCmd_GiveAllTaiwuLegendaryBookToRandomNpc instead.", true)]
			public static void GmCmd_GiveAllTaiwuLegendaryBookToRandomNpc(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011287 RID: 70279 RVA: 0x00679F84 File Offset: 0x00678184
			public static void GetLegendaryBookIncrementData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(11, 2, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011288 RID: 70280 RVA: 0x00679FB0 File Offset: 0x006781B0
			public static void GetAllLegendaryBooksOwningState(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(11, 3, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011289 RID: 70281 RVA: 0x00679FDC File Offset: 0x006781DC
			public static void GetLegendaryBookPresetDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(11, 4, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601128A RID: 70282 RVA: 0x0067A006 File Offset: 0x00678206
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.AddLegendaryBookSkillEmptyPreset instead.", true)]
			public static void AddLegendaryBookSkillEmptyPreset(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601128B RID: 70283 RVA: 0x0067A00E File Offset: 0x0067820E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.DuplicateLegendaryBookSkillPreset instead.", true)]
			public static void DuplicateLegendaryBookSkillPreset(IAsyncMethodRequestHandler requestHandler, sbyte presetIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601128C RID: 70284 RVA: 0x0067A016 File Offset: 0x00678216
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.RemoveLegendaryBookSkillPreset instead.", true)]
			public static void RemoveLegendaryBookSkillPreset(IAsyncMethodRequestHandler requestHandler, sbyte presetIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601128D RID: 70285 RVA: 0x0067A01E File Offset: 0x0067821E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.ResetLegendaryBookSkillPreset instead.", true)]
			public static void ResetLegendaryBookSkillPreset(IAsyncMethodRequestHandler requestHandler, int presetIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601128E RID: 70286 RVA: 0x0067A026 File Offset: 0x00678226
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.SetLegendaryBookSkillPreset instead.", true)]
			public static void SetLegendaryBookSkillPreset(IAsyncMethodRequestHandler requestHandler, sbyte presetIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601128F RID: 70287 RVA: 0x0067A02E File Offset: 0x0067822E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.SetLegendaryBookSkillPreset instead.", true)]
			public static void SetLegendaryBookSkillPreset(IAsyncMethodRequestHandler requestHandler, sbyte presetIndex, bool forceSet, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011290 RID: 70288 RVA: 0x0067A036 File Offset: 0x00678236
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.SaveLegendaryBookSkillPresetSlotCurrent instead.", true)]
			public static void SaveLegendaryBookSkillPresetSlotCurrent(IAsyncMethodRequestHandler requestHandler, sbyte skillType, int index, short skillTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011291 RID: 70289 RVA: 0x0067A03E File Offset: 0x0067823E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.ResetLegendaryBookBonus instead.", true)]
			public static void ResetLegendaryBookBonus(IAsyncMethodRequestHandler requestHandler, sbyte skillType, bool isYin, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011292 RID: 70290 RVA: 0x0067A046 File Offset: 0x00678246
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.SaveLegendaryBookWeaponPresetSlotCurrent instead.", true)]
			public static void SaveLegendaryBookWeaponPresetSlotCurrent(IAsyncMethodRequestHandler requestHandler, sbyte skillType, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011293 RID: 70291 RVA: 0x0067A04E File Offset: 0x0067824E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use LegendaryBookDomainMethod.Call.GmCmd_AddRandomLegendaryBookContestChar instead.", true)]
			public static void GmCmd_AddRandomLegendaryBookContestChar(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
