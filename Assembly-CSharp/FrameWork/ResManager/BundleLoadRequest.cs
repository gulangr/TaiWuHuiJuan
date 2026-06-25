using System;
using FrameWork.AssetBundlePackage;
using UnityEngine;

namespace FrameWork.ResManager
{
	// Token: 0x02001040 RID: 4160
	internal class BundleLoadRequest : LoadRequest
	{
		// Token: 0x0600BDD0 RID: 48592 RVA: 0x00562EDC File Offset: 0x005610DC
		public void OnDependenceLoaded(Object assetBundleObj)
		{
			this.LoadedDependenceCount += 1;
			bool flag = this.LoadedDependenceCount >= this.DependenceCount;
			if (flag)
			{
				Action onAllDependenceLoaded = this.OnAllDependenceLoaded;
				if (onAllDependenceLoaded != null)
				{
					onAllDependenceLoaded();
				}
				this.OnAllDependenceLoaded = null;
			}
		}

		// Token: 0x04009212 RID: 37394
		public ResourcePackage Package;

		// Token: 0x04009213 RID: 37395
		public string BundleKey;

		// Token: 0x04009214 RID: 37396
		public string BundleName;

		// Token: 0x04009215 RID: 37397
		public byte DependenceCount;

		// Token: 0x04009216 RID: 37398
		public byte LoadedDependenceCount;

		// Token: 0x04009217 RID: 37399
		public Action OnAllDependenceLoaded;
	}
}
