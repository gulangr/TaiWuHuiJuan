using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CD1 RID: 3281
	public class SortDropdownItem : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
	{
		// Token: 0x1700114C RID: 4428
		// (get) Token: 0x0600A5D1 RID: 42449 RVA: 0x004D4184 File Offset: 0x004D2384
		// (set) Token: 0x0600A5D2 RID: 42450 RVA: 0x004D418C File Offset: 0x004D238C
		public SortDropdownItem.ESortState CurrentSortState { get; private set; }

		// Token: 0x1700114D RID: 4429
		// (get) Token: 0x0600A5D3 RID: 42451 RVA: 0x004D4195 File Offset: 0x004D2395
		// (set) Token: 0x0600A5D4 RID: 42452 RVA: 0x004D419D File Offset: 0x004D239D
		public SortDropdownItem.EDirectionIconState CurrentDirectionState { get; private set; } = SortDropdownItem.EDirectionIconState.Hidden;

		// Token: 0x1700114E RID: 4430
		// (get) Token: 0x0600A5D5 RID: 42453 RVA: 0x004D41A6 File Offset: 0x004D23A6
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x0600A5D6 RID: 42454 RVA: 0x004D41AE File Offset: 0x004D23AE
		public void SetCheckBoxMouseExitEvent(Action onExit)
		{
			this._onMouseExitCheckBox = onExit;
		}

		// Token: 0x0600A5D7 RID: 42455 RVA: 0x004D41B8 File Offset: 0x004D23B8
		public void OnMouseExitCheckBox()
		{
			Action onMouseExitCheckBox = this._onMouseExitCheckBox;
			if (onMouseExitCheckBox != null)
			{
				onMouseExitCheckBox();
			}
		}

		// Token: 0x0600A5D8 RID: 42456 RVA: 0x004D41CD File Offset: 0x004D23CD
		public void OnPointerExit(PointerEventData eventData)
		{
			Action onMouseExitCheckBox = this._onMouseExitCheckBox;
			if (onMouseExitCheckBox != null)
			{
				onMouseExitCheckBox();
			}
		}

		// Token: 0x0600A5D9 RID: 42457 RVA: 0x004D41E2 File Offset: 0x004D23E2
		public void UpdateLabels(string labelText)
		{
			this.label.text = labelText;
		}

		// Token: 0x0600A5DA RID: 42458 RVA: 0x004D41F2 File Offset: 0x004D23F2
		public void UpdateNumber(bool isNumberActive, int number)
		{
			this.numberLabel.gameObject.SetActive(isNumberActive);
			this.numberLabel.text = number.ToString();
		}

		// Token: 0x0600A5DB RID: 42459 RVA: 0x004D421A File Offset: 0x004D241A
		public void SetPendingRemoval()
		{
			this.CurrentSortState = SortDropdownItem.ESortState.PendingRemoval;
		}

		// Token: 0x0600A5DC RID: 42460 RVA: 0x004D4225 File Offset: 0x004D2425
		public void SetNormal()
		{
			this.CurrentSortState = SortDropdownItem.ESortState.Normal;
		}

		// Token: 0x0600A5DD RID: 42461 RVA: 0x004D4230 File Offset: 0x004D2430
		public float GetButtonWidth()
		{
			RectTransform buttonRectTransform = this.button.GetComponent<RectTransform>();
			return buttonRectTransform.rect.width;
		}

		// Token: 0x040082E9 RID: 33513
		private Action _onMouseExitCheckBox;

		// Token: 0x040082EA RID: 33514
		[SerializeField]
		private CButton button;

		// Token: 0x040082EB RID: 33515
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040082EC RID: 33516
		[SerializeField]
		private TextMeshProUGUI numberLabel;

		// Token: 0x0200241A RID: 9242
		public enum ESortState
		{
			// Token: 0x0400E161 RID: 57697
			Normal,
			// Token: 0x0400E162 RID: 57698
			PendingRemoval
		}

		// Token: 0x0200241B RID: 9243
		public enum EDirectionIconState
		{
			// Token: 0x0400E164 RID: 57700
			Hidden,
			// Token: 0x0400E165 RID: 57701
			Up,
			// Token: 0x0400E166 RID: 57702
			Down
		}
	}
}
