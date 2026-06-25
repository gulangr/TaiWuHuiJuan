using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CE3 RID: 3299
	public static class SecretFilterOptionHelper
	{
		// Token: 0x0600A656 RID: 42582 RVA: 0x004D6B5C File Offset: 0x004D4D5C
		public static List<short> GetGeneralFilterTemplateIds()
		{
			List<short> result = new List<short>();
			foreach (SecretInformationGeneralFilterItem configItem in ((IEnumerable<SecretInformationGeneralFilterItem>)SecretInformationGeneralFilter.Instance))
			{
				bool flag = configItem == null;
				if (!flag)
				{
					result.Add(configItem.TemplateId);
				}
			}
			return result;
		}
	}
}
