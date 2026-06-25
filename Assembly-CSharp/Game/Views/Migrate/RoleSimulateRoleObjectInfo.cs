using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x02000909 RID: 2313
	public class RoleSimulateRoleObjectInfo : MonoBehaviour
	{
		// Token: 0x04004FBE RID: 20414
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004FBF RID: 20415
		public TMP_InputField roleKeyInput;

		// Token: 0x04004FC0 RID: 20416
		public TMP_InputField roleNameInput;

		// Token: 0x04004FC1 RID: 20417
		public CButton btnRandomName;

		// Token: 0x04004FC2 RID: 20418
		public TMP_InputField roleAgeInput;

		// Token: 0x04004FC3 RID: 20419
		public CDropdown identityDropDown;

		// Token: 0x04004FC4 RID: 20420
		public CButton btnRandom;

		// Token: 0x04004FC5 RID: 20421
		public CButton btnPreset;

		// Token: 0x04004FC6 RID: 20422
		public CButton btnDeleteRole;

		// Token: 0x04004FC7 RID: 20423
		public CDropdown behaviorDropDown;
	}
}
