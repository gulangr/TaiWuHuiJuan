using System;
using FrameWork;

namespace Game.Views.MouseTips
{
	// Token: 0x02000830 RID: 2096
	public static class CommonTipArgumentBoxExtensions
	{
		// Token: 0x06006679 RID: 26233 RVA: 0x002EBAE4 File Offset: 0x002E9CE4
		public static ArgumentBox Set(this ArgumentBox argumentBox, string key, CommonTipBaseRuntime runtime)
		{
			return argumentBox.SetObject(key, runtime);
		}
	}
}
