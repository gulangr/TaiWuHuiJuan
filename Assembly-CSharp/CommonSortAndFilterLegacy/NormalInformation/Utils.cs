using System;
using Config;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000489 RID: 1161
	public static class Utils
	{
		// Token: 0x0600412A RID: 16682 RVA: 0x00200F3C File Offset: 0x001FF13C
		public static InformationInfoItem GetInformationInfo(NormalInformationDisplayData displayData)
		{
			InformationItem config = Information.Instance.GetItem(displayData.NormalInformation.TemplateId);
			return InformationInfo.Instance.GetItem(config.InfoIds[(int)displayData.NormalInformation.Level]);
		}
	}
}
