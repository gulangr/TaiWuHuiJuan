using System;
using System.Collections.Generic;
using System.Linq;

namespace AiEditor
{
	// Token: 0x0200066B RID: 1643
	public class AiBlueprintUpdaterV104ToV105 : IAiBlueprintUpdater
	{
		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06004DDD RID: 19933 RVA: 0x0024B3AB File Offset: 0x002495AB
		public string FromVersion
		{
			get
			{
				return "1.0.4";
			}
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06004DDE RID: 19934 RVA: 0x0024B3B2 File Offset: 0x002495B2
		public string ToVersion
		{
			get
			{
				return "1.0.5";
			}
		}

		// Token: 0x06004DDF RID: 19935 RVA: 0x0024B3BC File Offset: 0x002495BC
		public void Update(AiBlueprintSnapshot blueprint)
		{
			foreach (AiNodeDataSnapshot snapshot in from x in blueprint.Data
			where x.Type == 2
			select x)
			{
				for (int i = 0; i < snapshot.SubTypes.Count; i++)
				{
					int subType = snapshot.SubTypes[i];
					bool flag = subType == this._fillIndexAction;
					if (flag)
					{
						this.FillParam(snapshot, i);
					}
				}
			}
			foreach (AiNodeDataSnapshot snapshot2 in from x in blueprint.Data
			where x.Type == 1
			select x)
			{
				for (int j = 0; j < snapshot2.SubTypes.Count; j++)
				{
					int subType2 = snapshot2.SubTypes[j];
					bool flag2 = subType2 == this._fillIndexCondition;
					if (flag2)
					{
						this.FillParam(snapshot2, j);
					}
				}
			}
		}

		// Token: 0x06004DE0 RID: 19936 RVA: 0x0024B520 File Offset: 0x00249720
		private void FillParam(AiNodeDataSnapshot snapshot, int index)
		{
			List<string> param = snapshot.Params[index];
			bool flag = param.Count != 0;
			if (!flag)
			{
				param.Insert(0, "0");
				param.Insert(1, "6");
			}
		}

		// Token: 0x040035F7 RID: 13815
		private readonly int _fillIndexAction = 20;

		// Token: 0x040035F8 RID: 13816
		private readonly int _fillIndexCondition = 35;
	}
}
