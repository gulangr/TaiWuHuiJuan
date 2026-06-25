using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA0 RID: 3232
	public class FilterSummaryItem : MonoBehaviour
	{
		// Token: 0x0600A477 RID: 42103 RVA: 0x004CC83D File Offset: 0x004CAA3D
		public void Setup(int lineId, int menuId, string text, Action<int, int> onDelete)
		{
			this._lineId = lineId;
			this._menuId = menuId;
			this._onDelete = onDelete;
			this.labelText.SetText(text, true);
			this.deleteButton.ClearAndAddListener(new Action(this.OnDeleteClicked));
		}

		// Token: 0x0600A478 RID: 42104 RVA: 0x004CC87C File Offset: 0x004CAA7C
		private void OnDeleteClicked()
		{
			Action<int, int> onDelete = this._onDelete;
			if (onDelete != null)
			{
				onDelete(this._lineId, this._menuId);
			}
		}

		// Token: 0x04008233 RID: 33331
		[SerializeField]
		private TextMeshProUGUI labelText;

		// Token: 0x04008234 RID: 33332
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x04008235 RID: 33333
		private int _lineId;

		// Token: 0x04008236 RID: 33334
		private int _menuId;

		// Token: 0x04008237 RID: 33335
		private Action<int, int> _onDelete;
	}
}
