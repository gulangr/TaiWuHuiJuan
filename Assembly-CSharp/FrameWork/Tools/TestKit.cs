using System;
using GameData;

namespace FrameWork.Tools
{
	// Token: 0x02001033 RID: 4147
	public static class TestKit
	{
		// Token: 0x0600BD80 RID: 48512 RVA: 0x005613A1 File Offset: 0x0055F5A1
		public static void InitLocalStringManagerManual()
		{
			ExternalDataBridge.Initialize(new GameContext());
			LocalStringManager.Init(LocalStringManager.LanguageType.CN);
		}
	}
}
