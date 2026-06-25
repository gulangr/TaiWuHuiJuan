using System;
using System.Linq;

namespace AiEditor
{
	// Token: 0x02000669 RID: 1641
	public class AiBlueprintUpdaterV102ToV103 : IAiBlueprintUpdater
	{
		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x06004DD4 RID: 19924 RVA: 0x0024B18C File Offset: 0x0024938C
		public string FromVersion
		{
			get
			{
				return "1.0.2";
			}
		}

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x06004DD5 RID: 19925 RVA: 0x0024B193 File Offset: 0x00249393
		public string ToVersion
		{
			get
			{
				return "1.0.3";
			}
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x0024B19C File Offset: 0x0024939C
		public void Update(AiBlueprintSnapshot blueprint)
		{
			foreach (AiNodeDataSnapshot snapshot in from x in blueprint.Data
			where x.Type == 1
			select x)
			{
				for (int i = 0; i < snapshot.SubTypes.Count; i++)
				{
					bool flag = snapshot.SubTypes[i] == this._fromConditionId;
					if (flag)
					{
						snapshot.SubTypes[i] = this._toConditionId;
					}
				}
			}
		}

		// Token: 0x040035F4 RID: 13812
		private readonly int _fromConditionId = 52;

		// Token: 0x040035F5 RID: 13813
		private readonly int _toConditionId = 1;
	}
}
