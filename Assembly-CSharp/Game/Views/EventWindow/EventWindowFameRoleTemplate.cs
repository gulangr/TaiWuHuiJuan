using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A3C RID: 2620
	public class EventWindowFameRoleTemplate : MonoBehaviour
	{
		// Token: 0x06008168 RID: 33128 RVA: 0x003C382E File Offset: 0x003C1A2E
		private void Awake()
		{
			this.mainBtn.ClearAndAddListener(new Action(this.OnClick));
		}

		// Token: 0x06008169 RID: 33129 RVA: 0x003C3849 File Offset: 0x003C1A49
		private void OnClick()
		{
			Action actionOnClick = this.ActionOnClick;
			if (actionOnClick != null)
			{
				actionOnClick();
			}
		}

		// Token: 0x0600816A RID: 33130 RVA: 0x003C385E File Offset: 0x003C1A5E
		public void SetSelected(bool isSelected)
		{
			this.selectedCover.SetActive(isSelected);
		}

		// Token: 0x040062D1 RID: 25297
		public CImage icon;

		// Token: 0x040062D2 RID: 25298
		public TextMeshProUGUI txtName;

		// Token: 0x040062D3 RID: 25299
		public TextMeshProUGUI txtFameInfluence;

		// Token: 0x040062D4 RID: 25300
		public TextMeshProUGUI txtDurationFrom;

		// Token: 0x040062D5 RID: 25301
		public TextMeshProUGUI txtDurationTom;

		// Token: 0x040062D6 RID: 25302
		public GameObject selectedCover;

		// Token: 0x040062D7 RID: 25303
		public CButton mainBtn;

		// Token: 0x040062D8 RID: 25304
		public Action ActionOnClick;
	}
}
