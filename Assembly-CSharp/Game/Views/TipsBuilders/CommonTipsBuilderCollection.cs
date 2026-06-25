using System;
using System.Collections.Generic;
using Config;
using FrameWork;

namespace Game.Views.TipsBuilders
{
	// Token: 0x02000751 RID: 1873
	public static class CommonTipsBuilderCollection
	{
		// Token: 0x06005AD9 RID: 23257 RVA: 0x002A2638 File Offset: 0x002A0838
		public static Dictionary<int, CommonTipsBuilderInfo> CreateBuilderCollection()
		{
			return new Dictionary<int, CommonTipsBuilderInfo>
			{
				{
					28,
					new CommonTipsBuilderInfo(28, new CommonTipsBuilder_Feature())
				},
				{
					38,
					new CommonTipsBuilderInfo(38, new CommonTipsBuilder_SecretInformation())
				},
				{
					41,
					new CommonTipsBuilderInfo(41, new Func<CommonTipItem, ArgumentBox, ArgumentBox>(CommonTipsBuildFuncs.BuildCommonTipArg_Flaw))
				},
				{
					50,
					new CommonTipsBuilderInfo(50, new CommonTipsBuilder_LegendaryBookBonus())
				}
			};
		}

		// Token: 0x06005ADA RID: 23258 RVA: 0x002A26AC File Offset: 0x002A08AC
		public static Dictionary<TipType, int> CreateReplaceTipTypeToTemplateIdMap()
		{
			return new Dictionary<TipType, int>
			{
				{
					TipType.Feature,
					28
				},
				{
					TipType.SecretInformation,
					38
				},
				{
					TipType.LegendaryBookBonus,
					50
				},
				{
					TipType.Flaw,
					41
				}
			};
		}
	}
}
