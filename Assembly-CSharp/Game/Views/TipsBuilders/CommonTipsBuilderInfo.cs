using System;
using Config;
using FrameWork;

namespace Game.Views.TipsBuilders
{
	// Token: 0x02000752 RID: 1874
	public struct CommonTipsBuilderInfo
	{
		// Token: 0x06005ADB RID: 23259 RVA: 0x002A26EF File Offset: 0x002A08EF
		public CommonTipsBuilderInfo(int tipItemKey, ICommonTipBuilder builder)
		{
			this.TipItemKey = tipItemKey;
			this.Builder = builder;
			this.BuildFunc = new Func<CommonTipItem, ArgumentBox, ArgumentBox>(builder.BuildTip);
		}

		// Token: 0x06005ADC RID: 23260 RVA: 0x002A2713 File Offset: 0x002A0913
		public CommonTipsBuilderInfo(int tipItemKey, Func<CommonTipItem, ArgumentBox, ArgumentBox> buildFunc)
		{
			this.TipItemKey = tipItemKey;
			this.Builder = null;
			this.BuildFunc = buildFunc;
		}

		// Token: 0x04003E96 RID: 16022
		public readonly int TipItemKey;

		// Token: 0x04003E97 RID: 16023
		public readonly ICommonTipBuilder Builder;

		// Token: 0x04003E98 RID: 16024
		public readonly Func<CommonTipItem, ArgumentBox, ArgumentBox> BuildFunc;
	}
}
