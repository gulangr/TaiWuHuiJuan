using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x02000910 RID: 2320
	public class ModCurModPanel : MonoBehaviour
	{
		// Token: 0x04005099 RID: 20633
		public InfinityScroll curModScroll;

		// Token: 0x0400509A RID: 20634
		public ModPanelBasicInfo basicInfo;

		// Token: 0x0400509B RID: 20635
		public TextMeshProUGUI description;

		// Token: 0x0400509C RID: 20636
		public RectTransform settings;

		// Token: 0x0400509D RID: 20637
		public CButton enableBtn;

		// Token: 0x0400509E RID: 20638
		public CButton disableBtn;

		// Token: 0x0400509F RID: 20639
		public CButton applyBtn;

		// Token: 0x040050A0 RID: 20640
		public CButton closeBtn;

		// Token: 0x040050A1 RID: 20641
		public ModIdSwitch curModPageSwitch;

		// Token: 0x040050A2 RID: 20642
		public TMP_InputField curModSearchInputField;

		// Token: 0x040050A3 RID: 20643
		public CButton downloadingBtn;

		// Token: 0x040050A4 RID: 20644
		public CButton removeRemoteBtn;

		// Token: 0x040050A5 RID: 20645
		public CButton removeLocalBtn;

		// Token: 0x040050A6 RID: 20646
		public CButton buttonOpenExplorer;

		// Token: 0x040050A7 RID: 20647
		public ModEnableDependenceDialog enableDependenceDialog;

		// Token: 0x040050A8 RID: 20648
		public GameObject versionWarningMark;
	}
}
