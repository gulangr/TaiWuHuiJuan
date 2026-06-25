using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x0200092D RID: 2349
	public class ModWorkshopPanel : MonoBehaviour
	{
		// Token: 0x0400513B RID: 20795
		public TMP_InputField workshopSearchInputField;

		// Token: 0x0400513C RID: 20796
		public ModIdSwitch workshopPageSwitch;

		// Token: 0x0400513D RID: 20797
		public CToggleGroup workshopSortToggleGroup;

		// Token: 0x0400513E RID: 20798
		public CToggleGroup workshopTimeToggleGroup;

		// Token: 0x0400513F RID: 20799
		public InfinityScroll workshopScroll;

		// Token: 0x04005140 RID: 20800
		public GameObject list;

		// Token: 0x04005141 RID: 20801
		public GameObject detail;

		// Token: 0x04005142 RID: 20802
		public TextMeshProUGUI label;

		// Token: 0x04005143 RID: 20803
		public TextMeshProUGUI content;

		// Token: 0x04005144 RID: 20804
		public ModPanelBasicInfo basicInfo;

		// Token: 0x04005145 RID: 20805
		public CToggleGroup modDetailToggleGroup;

		// Token: 0x04005146 RID: 20806
		public CButton buttonClose;

		// Token: 0x04005147 RID: 20807
		public CButton buttonVoteUp;

		// Token: 0x04005148 RID: 20808
		public CButton buttonVoteDown;

		// Token: 0x04005149 RID: 20809
		public CButton buttonShare;

		// Token: 0x0400514A RID: 20810
		public CButton buttonCloseShare;

		// Token: 0x0400514B RID: 20811
		public GameObject share;

		// Token: 0x0400514C RID: 20812
		public GameObject shareTip;

		// Token: 0x0400514D RID: 20813
		public TMP_InputField shareInputField;

		// Token: 0x0400514E RID: 20814
		public CDropdown tagDropdown;

		// Token: 0x0400514F RID: 20815
		public CButton buttonManage;

		// Token: 0x04005150 RID: 20816
		public CButton buttonSubscribe;

		// Token: 0x04005151 RID: 20817
		public GameObject checkMark;

		// Token: 0x04005152 RID: 20818
		public CButton buttonOpenWeb;

		// Token: 0x04005153 RID: 20819
		public ModSubscribeDependenceDialog subscribeDependenceDialog;
	}
}
