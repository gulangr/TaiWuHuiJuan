using System;
using GameData.GameDataBridge;

namespace GameData.Domains.SpecialEffect
{
	// Token: 0x02000FC0 RID: 4032
	public static class SpecialEffectDomainMethod
	{
		// Token: 0x020025FB RID: 9723
		public static class Call
		{
			// Token: 0x060110F8 RID: 69880 RVA: 0x00677A97 File Offset: 0x00675C97
			public static void GetAllCostNeiliEffectData(int listenerId, int charId, short skillId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 17, 0, charId, skillId);
			}

			// Token: 0x060110F9 RID: 69881 RVA: 0x00677AA6 File Offset: 0x00675CA6
			public static void CostNeiliEffect(int charId, short skillId, short effectId)
			{
				GameDataBridge.AddMethodCall<int, short, short>(-1, 17, 1, charId, skillId, effectId);
			}

			// Token: 0x060110FA RID: 69882 RVA: 0x00677AB6 File Offset: 0x00675CB6
			public static void CanCostTrickDuringPreparingSkill(int listenerId, int charId, short skillId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 17, 2, charId, skillId);
			}

			// Token: 0x060110FB RID: 69883 RVA: 0x00677AC5 File Offset: 0x00675CC5
			public static void CostTrickDuringPreparingSkill(int listenerId, int charId, int trickIndex)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 17, 3, charId, trickIndex);
			}
		}

		// Token: 0x020025FC RID: 9724
		public static class AsyncCall
		{
			// Token: 0x060110FC RID: 69884 RVA: 0x00677AD4 File Offset: 0x00675CD4
			public static void GetAllCostNeiliEffectData(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(17, 0, charId, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110FD RID: 69885 RVA: 0x00677B00 File Offset: 0x00675D00
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use SpecialEffectDomainMethod.Call.CostNeiliEffect instead.", true)]
			public static void CostNeiliEffect(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, short effectId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060110FE RID: 69886 RVA: 0x00677B08 File Offset: 0x00675D08
			public static void CanCostTrickDuringPreparingSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(17, 2, charId, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060110FF RID: 69887 RVA: 0x00677B34 File Offset: 0x00675D34
			public static void CostTrickDuringPreparingSkill(IAsyncMethodRequestHandler requestHandler, int charId, int trickIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(17, 3, charId, trickIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
