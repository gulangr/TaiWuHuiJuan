using System;
using TMPro;
using UICommon;
using UnityEngine;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000430 RID: 1072
	public class CommonSortDropdownItem : MonoBehaviour
	{
		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06003F9F RID: 16287 RVA: 0x001FACDB File Offset: 0x001F8EDB
		// (set) Token: 0x06003FA0 RID: 16288 RVA: 0x001FACE3 File Offset: 0x001F8EE3
		public CommonSortDropdownItem.ESortState CurrentSortState { get; private set; }

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06003FA1 RID: 16289 RVA: 0x001FACEC File Offset: 0x001F8EEC
		public CommonDirectionButton DirectionButton
		{
			get
			{
				return this.directionButton;
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06003FA2 RID: 16290 RVA: 0x001FACF4 File Offset: 0x001F8EF4
		public CButtonObsolete Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x06003FA3 RID: 16291 RVA: 0x001FACFC File Offset: 0x001F8EFC
		public void SetCheckBoxMouseExitEvent(Action onExit)
		{
			this._onMouseExitCheckBox = onExit;
		}

		// Token: 0x06003FA4 RID: 16292 RVA: 0x001FAD06 File Offset: 0x001F8F06
		public void OnMouseExitCheckBox()
		{
			Action onMouseExitCheckBox = this._onMouseExitCheckBox;
			if (onMouseExitCheckBox != null)
			{
				onMouseExitCheckBox();
			}
		}

		// Token: 0x06003FA5 RID: 16293 RVA: 0x001FAD1C File Offset: 0x001F8F1C
		public void UpdateLabels(string labelText)
		{
			foreach (TextMeshProUGUI label in this.labels)
			{
				label.text = labelText;
			}
		}

		// Token: 0x06003FA6 RID: 16294 RVA: 0x001FAD4E File Offset: 0x001F8F4E
		public void UpdateNumber(bool isNumberActive, int number)
		{
			this.numberObject.SetActive(isNumberActive);
			this.numberLabel.text = number.ToString();
		}

		// Token: 0x06003FA7 RID: 16295 RVA: 0x001FAD71 File Offset: 0x001F8F71
		public void SetPendingRemoval()
		{
			this.CurrentSortState = CommonSortDropdownItem.ESortState.PendingRemoval;
		}

		// Token: 0x06003FA8 RID: 16296 RVA: 0x001FAD7C File Offset: 0x001F8F7C
		public void SetNormal()
		{
			this.CurrentSortState = CommonSortDropdownItem.ESortState.Normal;
		}

		// Token: 0x06003FA9 RID: 16297 RVA: 0x001FAD88 File Offset: 0x001F8F88
		public float GetButtonWidth()
		{
			RectTransform buttonRectTransform = this.button.GetComponent<RectTransform>();
			return buttonRectTransform.rect.width;
		}

		// Token: 0x04002D7A RID: 11642
		private Action _onMouseExitCheckBox;

		// Token: 0x04002D7B RID: 11643
		[SerializeField]
		private CommonDirectionButton directionButton;

		// Token: 0x04002D7C RID: 11644
		[SerializeField]
		private CButtonObsolete button;

		// Token: 0x04002D7D RID: 11645
		[SerializeField]
		private TextMeshProUGUI[] labels;

		// Token: 0x04002D7E RID: 11646
		[SerializeField]
		private GameObject numberObject;

		// Token: 0x04002D7F RID: 11647
		[SerializeField]
		private TextMeshProUGUI numberLabel;

		// Token: 0x020018D7 RID: 6359
		public enum ESortState
		{
			// Token: 0x0400B027 RID: 45095
			Normal,
			// Token: 0x0400B028 RID: 45096
			PendingRemoval
		}
	}
}
