using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x0200092A RID: 2346
	public class ModUploadDescriptionArea : MonoBehaviour
	{
		// Token: 0x04005118 RID: 20760
		public CToggleGroup modDescriptionToggleGroup;

		// Token: 0x04005119 RID: 20761
		public GameObject descriptionRoot;

		// Token: 0x0400511A RID: 20762
		public GameObject updateLogRoot;

		// Token: 0x0400511B RID: 20763
		public TMP_InputField descriptionInputField;

		// Token: 0x0400511C RID: 20764
		public TextMeshProUGUI updateLogContent;

		// Token: 0x0400511D RID: 20765
		public GameObject warningMark;
	}
}
