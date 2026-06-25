using System;
using System.Collections.Generic;
using System.Linq;

namespace AiEditor
{
	// Token: 0x0200066A RID: 1642
	public class AiBlueprintUpdaterV103ToV104 : IAiBlueprintUpdater
	{
		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06004DD8 RID: 19928 RVA: 0x0024B26C File Offset: 0x0024946C
		public string FromVersion
		{
			get
			{
				return "1.0.3";
			}
		}

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x06004DD9 RID: 19929 RVA: 0x0024B273 File Offset: 0x00249473
		public string ToVersion
		{
			get
			{
				return "1.0.4";
			}
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x0024B27C File Offset: 0x0024947C
		public void Update(AiBlueprintSnapshot blueprint)
		{
			foreach (AiNodeDataSnapshot snapshot in from x in blueprint.Data
			where x.Type == 2
			select x)
			{
				for (int i = 0; i < snapshot.SubTypes.Count; i++)
				{
					int subType = snapshot.SubTypes[i];
					bool flag = this._fillParams.Contains(subType);
					if (flag)
					{
						this.FillParam(snapshot, i);
					}
				}
			}
		}

		// Token: 0x06004DDB RID: 19931 RVA: 0x0024B334 File Offset: 0x00249534
		private void FillParam(AiNodeDataSnapshot snapshot, int index)
		{
			List<string> param = snapshot.Params[index];
			int count = param.Count;
			bool flag = count == 0 || count == 1;
			if (flag)
			{
				param.Insert(0, "undefined");
			}
		}

		// Token: 0x040035F6 RID: 13814
		private readonly List<int> _fillParams = new List<int>
		{
			1,
			2,
			3,
			4
		};
	}
}
