using System;
using GameData.GameDataBridge;

namespace GameData.Domains.TutorialChapter
{
	// Token: 0x02000FBC RID: 4028
	public static class TutorialChapterDomainMethod
	{
		// Token: 0x020025F3 RID: 9715
		public static class Call
		{
			// Token: 0x06010D58 RID: 68952 RVA: 0x006729CA File Offset: 0x00670BCA
			public static void StartChapter(int chapter)
			{
				GameDataBridge.AddMethodCall<int>(-1, 15, 0, chapter);
			}

			// Token: 0x06010D59 RID: 68953 RVA: 0x006729D8 File Offset: 0x00670BD8
			public static void GetNextForceMoveToLocation(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 15, 1);
			}
		}

		// Token: 0x020025F4 RID: 9716
		public static class AsyncCall
		{
			// Token: 0x06010D5A RID: 68954 RVA: 0x006729E5 File Offset: 0x00670BE5
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use TutorialChapterDomainMethod.Call.StartChapter instead.", true)]
			public static void StartChapter(IAsyncMethodRequestHandler requestHandler, int chapter, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06010D5B RID: 68955 RVA: 0x006729F0 File Offset: 0x00670BF0
			public static void GetNextForceMoveToLocation(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(15, 1, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}
		}
	}
}
