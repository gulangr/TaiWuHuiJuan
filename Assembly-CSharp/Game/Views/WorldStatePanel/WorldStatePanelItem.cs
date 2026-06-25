using System;
using FrameWork.UISystem.UIElements;
using Game.Views.MouseTips;
using TMPro;
using UnityEngine;

namespace Game.Views.WorldStatePanel
{
	// Token: 0x02000722 RID: 1826
	public class WorldStatePanelItem : MonoBehaviour
	{
		// Token: 0x04003BAC RID: 15276
		public CImage icon;

		// Token: 0x04003BAD RID: 15277
		public TextMeshProUGUI desc;

		// Token: 0x04003BAE RID: 15278
		public CButton jumpButton;

		// Token: 0x04003BAF RID: 15279
		public RectTransform contentHolder;

		// Token: 0x04003BB0 RID: 15280
		public TextMeshProUGUI subTitle;

		// Token: 0x04003BB1 RID: 15281
		public GameObject conditionPrefab;

		// Token: 0x04003BB2 RID: 15282
		public TextMeshProUGUI extraDescLabel;

		// Token: 0x04003BB3 RID: 15283
		public RectTransform conditionRoot;

		// Token: 0x04003BB4 RID: 15284
		public TextMeshProUGUI specialText;

		// Token: 0x04003BB5 RID: 15285
		public Refers taiwuWantedTemplate;

		// Token: 0x04003BB6 RID: 15286
		public PracticeNoticeItem practiceNoticeItem;

		// Token: 0x04003BB7 RID: 15287
		public CImage selected;
	}
}
