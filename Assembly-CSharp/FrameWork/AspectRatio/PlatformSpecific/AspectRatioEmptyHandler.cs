using System;

namespace FrameWork.AspectRatio.PlatformSpecific
{
	// Token: 0x0200107A RID: 4218
	public class AspectRatioEmptyHandler : AspectRatioHandlerBase
	{
		// Token: 0x0600BF69 RID: 49001 RVA: 0x0056874D File Offset: 0x0056694D
		public AspectRatioEmptyHandler(AspectRatioDefinition definition) : base(definition)
		{
		}

		// Token: 0x0600BF6A RID: 49002 RVA: 0x00568758 File Offset: 0x00566958
		public override void OnResolutionChanged(int width, int height, bool fullScreen)
		{
		}

		// Token: 0x0600BF6B RID: 49003 RVA: 0x0056875B File Offset: 0x0056695B
		public override void OnFullScreenChanged(int width, int height, bool fullScreen)
		{
		}

		// Token: 0x0600BF6C RID: 49004 RVA: 0x0056875E File Offset: 0x0056695E
		public override void OnApplicationQuit()
		{
		}
	}
}
