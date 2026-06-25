using System;
using FrameWork.UISystem.UIElements;
using Game.Views.Following;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007C4 RID: 1988
	public class TravelerStation : MonoBehaviour
	{
		// Token: 0x06006101 RID: 24833 RVA: 0x002C7931 File Offset: 0x002C5B31
		public void Init()
		{
			this.canvasGroup.alpha = 0f;
			this.AreaId = -1;
		}

		// Token: 0x04004341 RID: 17217
		[SerializeField]
		internal CanvasGroup canvasGroup;

		// Token: 0x04004342 RID: 17218
		[SerializeField]
		internal CButton focusBtn;

		// Token: 0x04004343 RID: 17219
		[SerializeField]
		internal CButton moveBtn;

		// Token: 0x04004344 RID: 17220
		[SerializeField]
		internal CButton removeBtn;

		// Token: 0x04004345 RID: 17221
		[SerializeField]
		internal CButton editNameButton;

		// Token: 0x04004346 RID: 17222
		[SerializeField]
		internal CButton buildBtn;

		// Token: 0x04004347 RID: 17223
		[SerializeField]
		internal TooltipInvoker moveBtnMouseTipDisplayer;

		// Token: 0x04004348 RID: 17224
		[SerializeField]
		internal TooltipInvoker buildBtnMouseTipDisplayer;

		// Token: 0x04004349 RID: 17225
		[SerializeField]
		internal DisableStyleRoot moveBtnDisableStyleRoot;

		// Token: 0x0400434A RID: 17226
		[SerializeField]
		internal TMP_Text nameTitle;

		// Token: 0x0400434B RID: 17227
		[SerializeField]
		internal Game.Views.Following.CharacterLocationItem blockHolder;

		// Token: 0x0400434C RID: 17228
		[SerializeField]
		internal PositionFollower mapEffect;

		// Token: 0x0400434D RID: 17229
		[SerializeField]
		internal DisableStyleRoot buildBtnDisableStyleRootRoot;

		// Token: 0x0400434E RID: 17230
		[NonSerialized]
		public short AreaId = -1;
	}
}
