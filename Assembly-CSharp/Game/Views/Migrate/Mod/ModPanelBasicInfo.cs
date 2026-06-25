using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x0200091E RID: 2334
	public class ModPanelBasicInfo : MonoBehaviour
	{
		// Token: 0x040050E1 RID: 20705
		public TextMeshProUGUI nameValue;

		// Token: 0x040050E2 RID: 20706
		public TextMeshProUGUI authorValue;

		// Token: 0x040050E3 RID: 20707
		public TextMeshProUGUI versionValue;

		// Token: 0x040050E4 RID: 20708
		public RectTransform sourceValue;

		// Token: 0x040050E5 RID: 20709
		public TextMeshProUGUI fileIdValue;

		// Token: 0x040050E6 RID: 20710
		public TextMeshProUGUI fileSizeValue;

		// Token: 0x040050E7 RID: 20711
		public TextMeshProUGUI createDateValue;

		// Token: 0x040050E8 RID: 20712
		public TextMeshProUGUI updateDataValue;

		// Token: 0x040050E9 RID: 20713
		public RectTransform tagLayout;

		// Token: 0x040050EA RID: 20714
		public TextMeshProUGUI countFavorite;

		// Token: 0x040050EB RID: 20715
		public TextMeshProUGUI countSubscribe;

		// Token: 0x040050EC RID: 20716
		public TextMeshProUGUI countComment;

		// Token: 0x040050ED RID: 20717
		public RectTransform starLayout;

		// Token: 0x040050EE RID: 20718
		public ModImageInfo modImageInfo;
	}
}
