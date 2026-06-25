using System;

namespace FrameWork.AspectRatio
{
	// Token: 0x02001079 RID: 4217
	public abstract class AspectRatioHandlerBase
	{
		// Token: 0x0600BF64 RID: 48996 RVA: 0x00568721 File Offset: 0x00566921
		protected AspectRatioHandlerBase(AspectRatioDefinition definition)
		{
			this.AspectRatioDefinition = definition;
		}

		// Token: 0x0600BF65 RID: 48997 RVA: 0x00568732 File Offset: 0x00566932
		public void SetAspectRatio(int width, int height)
		{
			this.AspectRatioDefinition.Width = width;
			this.AspectRatioDefinition.Height = height;
		}

		// Token: 0x0600BF66 RID: 48998
		public abstract void OnResolutionChanged(int width, int height, bool fullScreen);

		// Token: 0x0600BF67 RID: 48999
		public abstract void OnFullScreenChanged(int width, int height, bool fullScreen);

		// Token: 0x0600BF68 RID: 49000
		public abstract void OnApplicationQuit();

		// Token: 0x040092A3 RID: 37539
		protected AspectRatioDefinition AspectRatioDefinition;
	}
}
