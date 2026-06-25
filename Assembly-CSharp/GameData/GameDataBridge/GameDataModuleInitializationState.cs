using System;

namespace GameData.GameDataBridge
{
	// Token: 0x02000FB5 RID: 4021
	public static class GameDataModuleInitializationState
	{
		// Token: 0x0600B911 RID: 47377 RVA: 0x00545F84 File Offset: 0x00544184
		public static bool CheckTransition(sbyte prevState, sbyte nextState)
		{
			bool flag = nextState == 1;
			bool result;
			if (flag)
			{
				result = (prevState == 0 || prevState == 3);
			}
			else
			{
				result = (nextState == prevState + 1);
			}
			return result;
		}

		// Token: 0x04008F6C RID: 36716
		public const sbyte Uninitialized = 0;

		// Token: 0x04008F6D RID: 36717
		public const sbyte AllowInitialization = 1;

		// Token: 0x04008F6E RID: 36718
		public const sbyte Initializing = 2;

		// Token: 0x04008F6F RID: 36719
		public const sbyte Initialized = 3;
	}
}
