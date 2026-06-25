using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C9C RID: 3228
	public class FilterMenuOption : MonoBehaviour
	{
		// Token: 0x0600A43B RID: 42043 RVA: 0x004CB58A File Offset: 0x004C978A
		public void BindButton(Action onClick)
		{
			this.button.ClearAndAddListener(onClick);
		}

		// Token: 0x0600A43C RID: 42044 RVA: 0x004CB59A File Offset: 0x004C979A
		public void SetInteractable(bool interactable)
		{
			this.button.interactable = interactable;
			this.checkBoxButton.interactable = interactable;
		}

		// Token: 0x0600A43D RID: 42045 RVA: 0x004CB5B7 File Offset: 0x004C97B7
		public void SetLabel(string str)
		{
			this.label.text = str;
		}

		// Token: 0x0600A43E RID: 42046 RVA: 0x004CB5C8 File Offset: 0x004C97C8
		public void SetSelected(bool selected)
		{
			bool flag = this.selectMark != null;
			if (flag)
			{
				this.selectMark.SetActive(selected);
			}
		}

		// Token: 0x0600A43F RID: 42047 RVA: 0x004CB5F3 File Offset: 0x004C97F3
		public void SetCheckBoxActive(bool active)
		{
			this.checkBoxArea.SetActive(active);
		}

		// Token: 0x0600A440 RID: 42048 RVA: 0x004CB603 File Offset: 0x004C9803
		public void BindCheckBoxButton(Action onclickCheckBox)
		{
			this.checkBoxButton.ClearAndAddListener(onclickCheckBox);
		}

		// Token: 0x04008208 RID: 33288
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008209 RID: 33289
		[SerializeField]
		private GameObject selectMark;

		// Token: 0x0400820A RID: 33290
		[SerializeField]
		private CButton button;

		// Token: 0x0400820B RID: 33291
		[SerializeField]
		private CButton checkBoxButton;

		// Token: 0x0400820C RID: 33292
		[SerializeField]
		private GameObject checkBoxArea;
	}
}
