using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x02000919 RID: 2329
	public class ModEntryTemplate : MonoBehaviour
	{
		// Token: 0x040050BB RID: 20667
		public TextMeshProUGUI title;

		// Token: 0x040050BC RID: 20668
		public CToggle isModEnabledToggle;

		// Token: 0x040050BD RID: 20669
		public GameObject steam;

		// Token: 0x040050BE RID: 20670
		public GameObject external;

		// Token: 0x040050BF RID: 20671
		public TextMeshProUGUI ratingAmountLabel;

		// Token: 0x040050C0 RID: 20672
		public List<CImage> starList;

		// Token: 0x040050C1 RID: 20673
		public GameObject rating;

		// Token: 0x040050C2 RID: 20674
		public GameObject outdatedWarning;

		// Token: 0x040050C3 RID: 20675
		public GameObject downloading;

		// Token: 0x040050C4 RID: 20676
		public CImage back;

		// Token: 0x040050C5 RID: 20677
		public GameObject dependencyWarning;
	}
}
