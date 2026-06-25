using System;
using System.Collections.Generic;
using System.Linq;

namespace AiEditor
{
	// Token: 0x02000668 RID: 1640
	public class AiBlueprintUpdaterV101ToV102 : IAiBlueprintUpdater
	{
		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06004DCF RID: 19919 RVA: 0x0024B03A File Offset: 0x0024923A
		public string FromVersion
		{
			get
			{
				return "1.0.1";
			}
		}

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x06004DD0 RID: 19920 RVA: 0x0024B041 File Offset: 0x00249241
		public string ToVersion
		{
			get
			{
				return "1.0.2";
			}
		}

		// Token: 0x06004DD1 RID: 19921 RVA: 0x0024B048 File Offset: 0x00249248
		public void Update(AiBlueprintSnapshot blueprint)
		{
			foreach (AiNodeDataSnapshot snapshot in from x in blueprint.Data
			where x.Type == 1
			select x)
			{
				for (int i = 0; i < snapshot.SubTypes.Count; i++)
				{
					int subType = snapshot.SubTypes[i];
					bool flag = this._reverseParams.Contains(subType);
					if (flag)
					{
						this.ReverseParam(snapshot, i);
					}
				}
			}
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x0024B100 File Offset: 0x00249300
		private void ReverseParam(AiNodeDataSnapshot snapshot, int index)
		{
			List<string> param = snapshot.Params[index];
			bool flag = param.Count == 2;
			if (flag)
			{
				List<string> list = param;
				List<string> list2 = param;
				string value = param[1];
				string value2 = param[0];
				list[0] = value;
				list2[1] = value2;
			}
		}

		// Token: 0x040035F3 RID: 13811
		private readonly List<int> _reverseParams = new List<int>
		{
			23,
			24,
			25,
			73
		};
	}
}
