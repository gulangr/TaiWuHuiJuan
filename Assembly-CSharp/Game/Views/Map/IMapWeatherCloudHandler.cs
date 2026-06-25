using System;
using UnityEngine;

namespace Game.Views.Map
{
	// Token: 0x02000944 RID: 2372
	public interface IMapWeatherCloudHandler
	{
		// Token: 0x17000CC5 RID: 3269
		// (get) Token: 0x06006EAC RID: 28332
		Vector2 SamplePos { get; }

		// Token: 0x17000CC6 RID: 3270
		// (get) Token: 0x06006EAD RID: 28333
		float SampleFactor { get; }

		// Token: 0x17000CC7 RID: 3271
		// (get) Token: 0x06006EAE RID: 28334
		string SampleImage { get; }

		// Token: 0x17000CC8 RID: 3272
		// (get) Token: 0x06006EAF RID: 28335
		float BaseScale { get; }

		// Token: 0x17000CC9 RID: 3273
		// (get) Token: 0x06006EB0 RID: 28336
		Vector2 BaseOffset { get; }

		// Token: 0x06006EB1 RID: 28337
		bool ShouldReset(Vector2 pos);
	}
}
