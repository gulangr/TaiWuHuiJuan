using System;
using System.Collections.Generic;
using System.Linq;

namespace AiEditor
{
	// Token: 0x0200066C RID: 1644
	public class AiBlueprintUpdaterV105ToV106 : IAiBlueprintUpdater
	{
		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x06004DE2 RID: 19938 RVA: 0x0024B580 File Offset: 0x00249780
		public string FromVersion
		{
			get
			{
				return "1.0.5";
			}
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x06004DE3 RID: 19939 RVA: 0x0024B587 File Offset: 0x00249787
		public string ToVersion
		{
			get
			{
				return "1.0.6";
			}
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x0024B590 File Offset: 0x00249790
		public void Update(AiBlueprintSnapshot blueprint)
		{
			foreach (AiNodeDataSnapshot snapshot in from x in blueprint.Data
			where x.Type == 1
			select x)
			{
				for (int i = 0; i < snapshot.SubTypes.Count; i++)
				{
					int subType = snapshot.SubTypes[i];
					int newSubType;
					bool flag = AiBlueprintUpdaterV105ToV106.ErrorTemplateIdMapping.TryGetValue(subType, out newSubType);
					if (flag)
					{
						snapshot.SubTypes[i] = newSubType;
					}
				}
			}
		}

		// Token: 0x040035F9 RID: 13817
		private static readonly Dictionary<int, int> ErrorTemplateIdMapping = new Dictionary<int, int>
		{
			{
				108,
				110
			},
			{
				109,
				108
			},
			{
				110,
				109
			}
		};
	}
}
