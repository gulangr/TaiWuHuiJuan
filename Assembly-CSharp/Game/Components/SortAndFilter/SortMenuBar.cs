using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CD2 RID: 3282
	public class SortMenuBar : MonoBehaviour
	{
		// Token: 0x1700114F RID: 4431
		// (get) Token: 0x0600A5DF RID: 42463 RVA: 0x004D426C File Offset: 0x004D246C
		private bool IsButtonInteractable
		{
			get
			{
				return this.button.interactable;
			}
		}

		// Token: 0x17001150 RID: 4432
		// (get) Token: 0x0600A5E0 RID: 42464 RVA: 0x004D4279 File Offset: 0x004D2479
		public bool IsEsc
		{
			get
			{
				return this._isAsc;
			}
		}

		// Token: 0x0600A5E1 RID: 42465 RVA: 0x004D4284 File Offset: 0x004D2484
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerTriggerEnter));
			this.pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerTriggerExit));
			this.btnSortOrder.onValueChanged.AddListener(new UnityAction<bool>(this.OnSortOrderChange));
		}

		// Token: 0x0600A5E2 RID: 42466 RVA: 0x004D42EC File Offset: 0x004D24EC
		private void OnDestroy()
		{
			this.pointerTrigger.EnterEvent.RemoveListener(new UnityAction(this.OnPointerTriggerEnter));
			this.pointerTrigger.ExitEvent.RemoveListener(new UnityAction(this.OnPointerTriggerExit));
			this.btnSortOrder.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnSortOrderChange));
		}

		// Token: 0x0600A5E3 RID: 42467 RVA: 0x004D4351 File Offset: 0x004D2551
		private void OnSortOrderChange(bool isAsc)
		{
			Action<bool> onSortOrderChanged = this._onSortOrderChanged;
			if (onSortOrderChanged != null)
			{
				onSortOrderChanged(isAsc);
			}
			this.SetSortAscVisual(isAsc);
		}

		// Token: 0x0600A5E4 RID: 42468 RVA: 0x004D436F File Offset: 0x004D256F
		private void SetSortAscVisual(bool isAsc)
		{
			this._isAsc = isAsc;
			this.imgSortOrderDesc.SetActive(!isAsc);
			this.imgHoverSortOrder.SetSprite(isAsc ? "ui9_btn_sort_1_1" : "ui9_btn_sort_0_1", false, null);
		}

		// Token: 0x0600A5E5 RID: 42469 RVA: 0x004D43A6 File Offset: 0x004D25A6
		public void SetOnChangeSortOrder(Action<bool> onChangeSortOrder)
		{
			this._onSortOrderChanged = onChangeSortOrder;
		}

		// Token: 0x0600A5E6 RID: 42470 RVA: 0x004D43B0 File Offset: 0x004D25B0
		public void SetInteractable(bool interactable)
		{
			this.button.interactable = interactable;
		}

		// Token: 0x0600A5E7 RID: 42471 RVA: 0x004D43C0 File Offset: 0x004D25C0
		public void SetSelected(bool selected)
		{
			bool flag = this._isSelected == selected;
			if (!flag)
			{
				this._isSelected = selected;
				this.bg.sprite = (selected ? this.selectedSprite : this.normalSprite);
			}
		}

		// Token: 0x0600A5E8 RID: 42472 RVA: 0x004D4401 File Offset: 0x004D2601
		public void UpdateLabel(string text)
		{
			this.label.text = text;
		}

		// Token: 0x0600A5E9 RID: 42473 RVA: 0x004D4414 File Offset: 0x004D2614
		public void UpdateSortDirectionIcon(bool isActive, bool isDsc)
		{
			this.btnSortOrder.gameObject.SetActive(isActive);
			bool flag = !isActive;
			if (!flag)
			{
				this.SetSortAscVisual(!isDsc);
			}
		}

		// Token: 0x0600A5EA RID: 42474 RVA: 0x004D4449 File Offset: 0x004D2649
		public void AddEnterEvent(UnityAction action)
		{
			this.pointerTrigger.EnterEvent.AddListener(action);
		}

		// Token: 0x0600A5EB RID: 42475 RVA: 0x004D445E File Offset: 0x004D265E
		public void AddExitEvent(UnityAction action)
		{
			this.pointerTrigger.ExitEvent.AddListener(action);
		}

		// Token: 0x0600A5EC RID: 42476 RVA: 0x004D4473 File Offset: 0x004D2673
		public void RemoveEnterEvent(UnityAction action)
		{
			this.pointerTrigger.EnterEvent.RemoveListener(action);
		}

		// Token: 0x0600A5ED RID: 42477 RVA: 0x004D4488 File Offset: 0x004D2688
		public void RemoveExitEvent(UnityAction action)
		{
			this.pointerTrigger.ExitEvent.RemoveListener(action);
		}

		// Token: 0x0600A5EE RID: 42478 RVA: 0x004D449D File Offset: 0x004D269D
		private void OnPointerTriggerEnter()
		{
		}

		// Token: 0x0600A5EF RID: 42479 RVA: 0x004D44A0 File Offset: 0x004D26A0
		private void OnPointerTriggerExit()
		{
		}

		// Token: 0x040082ED RID: 33517
		[SerializeField]
		private CButton button;

		// Token: 0x040082EE RID: 33518
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x040082EF RID: 33519
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040082F0 RID: 33520
		[SerializeField]
		private CImage bg;

		// Token: 0x040082F1 RID: 33521
		[SerializeField]
		private Sprite normalSprite;

		// Token: 0x040082F2 RID: 33522
		[SerializeField]
		private Sprite selectedSprite;

		// Token: 0x040082F3 RID: 33523
		[Header("排序按钮")]
		[SerializeField]
		private CToggle btnSortOrder;

		// Token: 0x040082F4 RID: 33524
		[SerializeField]
		private GameObject imgSortOrderDesc;

		// Token: 0x040082F5 RID: 33525
		[SerializeField]
		private CImage imgHoverSortOrder;

		// Token: 0x040082F6 RID: 33526
		private Action<bool> _onSortOrderChanged;

		// Token: 0x040082F7 RID: 33527
		private bool _isSelected;

		// Token: 0x040082F8 RID: 33528
		private bool _isAsc;
	}
}
