using System;
using UnityEngine;

namespace FrameWork.ResManager
{
	// Token: 0x0200103F RID: 4159
	internal class LoadRequest
	{
		// Token: 0x0400920B RID: 37387
		public string Path;

		// Token: 0x0400920C RID: 37388
		public Action<string> OnLoadError;

		// Token: 0x0400920D RID: 37389
		public bool BRealLoad = false;

		// Token: 0x0400920E RID: 37390
		public bool IsBundle;

		// Token: 0x0400920F RID: 37391
		public Type ResType;

		// Token: 0x04009210 RID: 37392
		public bool IsAsyncLoad = false;

		// Token: 0x04009211 RID: 37393
		public Action<Object> OnLoadFinish;
	}
}
