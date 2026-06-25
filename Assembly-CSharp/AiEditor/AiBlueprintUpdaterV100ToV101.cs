using System;
using System.Collections.Generic;
using System.Linq;

namespace AiEditor
{
	// Token: 0x02000667 RID: 1639
	public class AiBlueprintUpdaterV100ToV101 : IAiBlueprintUpdater
	{
		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x06004DC9 RID: 19913 RVA: 0x0024AE70 File Offset: 0x00249070
		public string FromVersion
		{
			get
			{
				return "1.0.0";
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06004DCA RID: 19914 RVA: 0x0024AE77 File Offset: 0x00249077
		public string ToVersion
		{
			get
			{
				return "1.0.1";
			}
		}

		// Token: 0x06004DCB RID: 19915 RVA: 0x0024AE80 File Offset: 0x00249080
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
					bool flag2 = this._fillParams.Contains(subType);
					if (flag2)
					{
						this.FillParam(snapshot, i);
					}
				}
			}
		}

		// Token: 0x06004DCC RID: 19916 RVA: 0x0024AF50 File Offset: 0x00249150
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

		// Token: 0x06004DCD RID: 19917 RVA: 0x0024AFA4 File Offset: 0x002491A4
		private void FillParam(AiNodeDataSnapshot snapshot, int index)
		{
			List<string> param = snapshot.Params[index];
			int count = param.Count;
			bool flag = count == 1 || count == 2;
			if (flag)
			{
				param.Insert(1, "0");
			}
		}

		// Token: 0x040035F1 RID: 13809
		private readonly List<int> _reverseParams = new List<int>
		{
			3,
			4,
			5
		};

		// Token: 0x040035F2 RID: 13810
		private readonly List<int> _fillParams = new List<int>
		{
			6,
			7
		};
	}
}
