using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x02000928 RID: 2344
	public class ModUploadBasicInfo : MonoBehaviour
	{
		// Token: 0x04005103 RID: 20739
		public TextMeshProUGUI authorValue;

		// Token: 0x04005104 RID: 20740
		public TextMeshProUGUI fileSizeValue;

		// Token: 0x04005105 RID: 20741
		public TextMeshProUGUI createDateValue;

		// Token: 0x04005106 RID: 20742
		public TextMeshProUGUI updateDataValue;

		// Token: 0x04005107 RID: 20743
		public RectTransform tagLayout;

		// Token: 0x04005108 RID: 20744
		public TMP_InputField nameInputField;

		// Token: 0x04005109 RID: 20745
		public TMP_InputField versionInputField;

		// Token: 0x0400510A RID: 20746
		public CDropdown visibilityDropdown;

		// Token: 0x0400510B RID: 20747
		public CButton buttonProgram;

		// Token: 0x0400510C RID: 20748
		public CButton buttonMore;

		// Token: 0x0400510D RID: 20749
		public GameObject tagWarningMark;

		// Token: 0x0400510E RID: 20750
		public GameObject nameWarningMark;

		// Token: 0x0400510F RID: 20751
		public GameObject versionWarningMark;

		// Token: 0x04005110 RID: 20752
		public ModImageInfo modImageInfo;

		// Token: 0x04005111 RID: 20753
		public GameObject buttonMoreLayout;

		// Token: 0x04005112 RID: 20754
		public CToggle changeConfig;

		// Token: 0x04005113 RID: 20755
		public CToggle hasArchive;

		// Token: 0x04005114 RID: 20756
		public CToggle needRestart;
	}
}
