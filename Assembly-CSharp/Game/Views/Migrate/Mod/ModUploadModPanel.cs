using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x0200092B RID: 2347
	public class ModUploadModPanel : MonoBehaviour
	{
		// Token: 0x0400511E RID: 20766
		public InfinityScroll uploadModScroll;

		// Token: 0x0400511F RID: 20767
		public ModUploadBasicInfo basicInfo;

		// Token: 0x04005120 RID: 20768
		public RectTransform settings;

		// Token: 0x04005121 RID: 20769
		public ModIdSwitch uploadModPageSwitch;

		// Token: 0x04005122 RID: 20770
		public TMP_InputField uploadModSearchInputField;

		// Token: 0x04005123 RID: 20771
		public CButton buttonAdd;

		// Token: 0x04005124 RID: 20772
		public ModUploadDescriptionArea descriptionArea;

		// Token: 0x04005125 RID: 20773
		public GameObject modInfo;

		// Token: 0x04005126 RID: 20774
		public CButton buttonCreate;

		// Token: 0x04005127 RID: 20775
		public CButton buttonAddSetting;

		// Token: 0x04005128 RID: 20776
		public ModSetProgramPanel setProgramPanel;

		// Token: 0x04005129 RID: 20777
		public ModEditSettingPanel editSettingPanel;

		// Token: 0x0400512A RID: 20778
		public GameObject deleteLocalBtn;

		// Token: 0x0400512B RID: 20779
		public ModEditUpdateLogPanel editUpdateLogPanel;

		// Token: 0x0400512C RID: 20780
		public GameObject deleteRemoteBtn;

		// Token: 0x0400512D RID: 20781
		public GameObject uploadBtn;

		// Token: 0x0400512E RID: 20782
		public GameObject updateBtn;

		// Token: 0x0400512F RID: 20783
		public CButton buttonOpenExplorer;

		// Token: 0x04005130 RID: 20784
		public ModUploadConfirmDialog uploadConfirmDialog;

		// Token: 0x04005131 RID: 20785
		public ModSetDependencePanel setDependencePanel;

		// Token: 0x04005132 RID: 20786
		public CButton buttonSync;

		// Token: 0x04005133 RID: 20787
		public CButton buttonOpenUpload;

		// Token: 0x04005134 RID: 20788
		public ModDirectlyUploadPanel directlyUploadPanel;

		// Token: 0x04005135 RID: 20789
		public GameObject versionInputFieldLayout;

		// Token: 0x04005136 RID: 20790
		public CButton buttonCreateFormDirectory;
	}
}
