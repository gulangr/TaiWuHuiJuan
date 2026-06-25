using System;
using UnityEngine;

namespace Game.Views.Building.BuildingAreaQuickActionMenu
{
	// Token: 0x02000C20 RID: 3104
	[Serializable]
	public class BuildingSizeConfig
	{
		// Token: 0x04007A02 RID: 31234
		public float baseRadius;

		// Token: 0x04007A03 RID: 31235
		[Tooltip("左组中心角度（12点钟方向为0度）")]
		public float leftGroupCenterAngle;

		// Token: 0x04007A04 RID: 31236
		[Tooltip("上组中心角度（12点钟方向为0度）")]
		public float topGroupCenterAngle;

		// Token: 0x04007A05 RID: 31237
		[Tooltip("右组中心角度（12点钟方向为0度）")]
		public float rightGroupCenterAngle;

		// Token: 0x04007A06 RID: 31238
		[Tooltip("右组中心角度（12点钟方向为0度）")]
		public float bottomGroupCenterAngle;

		// Token: 0x04007A07 RID: 31239
		[Tooltip("左组按钮间距角度（度）")]
		public float leftGroupSpacingAngle;

		// Token: 0x04007A08 RID: 31240
		[Tooltip("上组按钮间距角度（度）")]
		public float topGroupSpacingAngle;

		// Token: 0x04007A09 RID: 31241
		[Tooltip("右组按钮间距角度（度）")]
		public float rightGroupSpacingAngle;

		// Token: 0x04007A0A RID: 31242
		[Tooltip("右组按钮间距角度（度）")]
		public float bottomGroupSpacingAngle;

		// Token: 0x04007A0B RID: 31243
		[Tooltip("背景对象")]
		public CImage Background;
	}
}
