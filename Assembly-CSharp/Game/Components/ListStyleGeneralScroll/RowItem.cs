using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA6 RID: 3750
	public class RowItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x170013A3 RID: 5027
		// (get) Token: 0x0600ADB3 RID: 44467 RVA: 0x004F31CC File Offset: 0x004F13CC
		// (set) Token: 0x0600ADB4 RID: 44468 RVA: 0x004F31D4 File Offset: 0x004F13D4
		public bool IsHovering { get; private set; }

		// Token: 0x170013A4 RID: 5028
		// (get) Token: 0x0600ADB5 RID: 44469 RVA: 0x004F31DD File Offset: 0x004F13DD
		// (set) Token: 0x0600ADB6 RID: 44470 RVA: 0x004F31E5 File Offset: 0x004F13E5
		public bool HoverEnabled { get; set; } = true;

		// Token: 0x170013A5 RID: 5029
		// (get) Token: 0x0600ADB7 RID: 44471 RVA: 0x004F31EE File Offset: 0x004F13EE
		public TooltipInvoker TipDisplayer
		{
			get
			{
				return this.tipDisplayer;
			}
		}

		// Token: 0x170013A6 RID: 5030
		// (get) Token: 0x0600ADB8 RID: 44472 RVA: 0x004F31F6 File Offset: 0x004F13F6
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x170013A7 RID: 5031
		// (get) Token: 0x0600ADB9 RID: 44473 RVA: 0x004F3203 File Offset: 0x004F1403
		public Action<int, RowItem> OnButtonClicked
		{
			get
			{
				return this._onButtonClicked;
			}
		}

		// Token: 0x170013A8 RID: 5032
		// (get) Token: 0x0600ADBA RID: 44474 RVA: 0x004F320B File Offset: 0x004F140B
		// (set) Token: 0x0600ADBB RID: 44475 RVA: 0x004F3213 File Offset: 0x004F1413
		public bool Interactable { get; private set; }

		// Token: 0x170013A9 RID: 5033
		// (get) Token: 0x0600ADBC RID: 44476 RVA: 0x004F321C File Offset: 0x004F141C
		public Transform ContainerRoot
		{
			get
			{
				return this.horizontalLayoutGroup.transform;
			}
		}

		// Token: 0x0600ADBD RID: 44477 RVA: 0x004F322C File Offset: 0x004F142C
		public void Init(IReadOnlyList<ColumnDefinition> columnDefinitions, bool createColumn = true)
		{
			this._cellContainers.Clear();
			this._columnDefinitions = columnDefinitions;
			bool flag = !createColumn;
			if (!flag)
			{
				int cellIndex = 0;
				int nodeIndex = 0;
				while (cellIndex < columnDefinitions.Count && nodeIndex < this.horizontalLayoutGroup.transform.childCount)
				{
					RowCellContainer container = this.horizontalLayoutGroup.transform.GetChild(nodeIndex).GetComponent<RowCellContainer>();
					nodeIndex++;
					bool flag2 = container == null;
					if (!flag2)
					{
						container.Init(columnDefinitions[cellIndex], cellIndex < columnDefinitions.Count - 1);
						this._cellContainers.Add(container);
						cellIndex++;
					}
				}
				Tester.Assert(this._columnDefinitions.Count == this._cellContainers.Count, string.Format("{0} vs {1}", this._columnDefinitions.Count, this._cellContainers.Count));
			}
		}

		// Token: 0x0600ADBE RID: 44478 RVA: 0x004F3324 File Offset: 0x004F1524
		public void SetData(object rowData, bool showBottomLine, Func<int, ColumnDefinition, object, ListStyleGeneralScroll.CellStyle> cellStyleProvider = null)
		{
			for (int i = 0; i < this._columnDefinitions.Count; i++)
			{
				ColumnDefinition columnDefinition = this._columnDefinitions[i];
				object cellData = columnDefinition.CreateCellData(rowData);
				this._cellContainers[i].SetData(cellData);
				ListStyleGeneralScroll.CellStyle style = (cellStyleProvider != null) ? cellStyleProvider(i, columnDefinition, cellData) : ListStyleGeneralScroll.CellStyle.Default;
				this._cellContainers[i].SetSpecialBg(style.ShowSpecialBg);
			}
			bool flag = this.bottomLine != null;
			if (flag)
			{
				this.bottomLine.SetActive(true);
			}
		}

		// Token: 0x0600ADBF RID: 44479 RVA: 0x004F33C3 File Offset: 0x004F15C3
		public void SetDataForContainerDirect(int index, object rowData)
		{
			RowCellContainer orDefault = this._cellContainers.GetOrDefault(index);
			if (orDefault != null)
			{
				orDefault.SetData(rowData);
			}
		}

		// Token: 0x0600ADC0 RID: 44480 RVA: 0x004F33E0 File Offset: 0x004F15E0
		public void SetRowInteraction(bool enableRowInteraction, int index, Action<int, RowItem> onButtonClicked)
		{
			this._interactionEnabled = enableRowInteraction;
			this._currentIndex = index;
			this._onButtonClicked = onButtonClicked;
			this.button.interactable = enableRowInteraction;
			if (enableRowInteraction)
			{
				this.button.ClearAndAddListener(new Action(this.HandleButtonClick));
			}
		}

		// Token: 0x0600ADC1 RID: 44481 RVA: 0x004F3430 File Offset: 0x004F1630
		public void SetSelected(bool selected)
		{
			this._selected = selected;
			bool flag = this.selectedObject != null;
			if (flag)
			{
				this.selectedObject.SetActive(selected);
			}
			bool flag2 = this._columnDefinitions != null;
			if (flag2)
			{
				for (int i = 0; i < this._columnDefinitions.Count; i++)
				{
					RowCellContainer orDefault = this._cellContainers.GetOrDefault(i);
					if (orDefault != null)
					{
						orDefault.SetSelected(selected);
					}
				}
			}
		}

		// Token: 0x0600ADC2 RID: 44482 RVA: 0x004F34A8 File Offset: 0x004F16A8
		public virtual void SetDisabled(bool disabled)
		{
			for (int i = 0; i < this._cellContainers.Count; i++)
			{
				this._cellContainers[i].SetDisabled(disabled);
			}
		}

		// Token: 0x0600ADC3 RID: 44483 RVA: 0x004F34E8 File Offset: 0x004F16E8
		private void HandleButtonClick()
		{
			Action<int, RowItem> onButtonClicked = this._onButtonClicked;
			if (onButtonClicked != null)
			{
				onButtonClicked(this._currentIndex, this);
			}
			bool flag = !this._selected;
			if (flag)
			{
				bool flag2 = !string.IsNullOrWhiteSpace(this.soundEffectSelect);
				if (flag2)
				{
					AudioManager.Instance.PlaySound(this.soundEffectSelect, false, false);
				}
			}
			else
			{
				bool flag3 = !string.IsNullOrWhiteSpace(this.soundEffectDeselect);
				if (flag3)
				{
					AudioManager.Instance.PlaySound(this.soundEffectDeselect, false, false);
				}
			}
		}

		// Token: 0x0600ADC4 RID: 44484 RVA: 0x004F356C File Offset: 0x004F176C
		public void SetClickEvent(Action onClick)
		{
			this._onButtonClicked = delegate(int i, RowItem item)
			{
				Action onClick2 = onClick;
				if (onClick2 != null)
				{
					onClick2();
				}
			};
			this.button.ClearAndAddListener(new Action(this.HandleButtonClick));
		}

		// Token: 0x0600ADC5 RID: 44485 RVA: 0x004F35B4 File Offset: 0x004F17B4
		public void SetInteractable(bool interactable, bool enableStyle = true)
		{
			this.Interactable = interactable;
			this.button.interactable = interactable;
			this.HoverEnabled = interactable;
			bool flag = !interactable && this.IsHovering;
			if (flag)
			{
				this.IsHovering = false;
				bool flag2 = this.hover != null;
				if (flag2)
				{
					this.hover.SetActive(false);
				}
				Action onPointerExitEvent = this.OnPointerExitEvent;
				if (onPointerExitEvent != null)
				{
					onPointerExitEvent();
				}
			}
		}

		// Token: 0x0600ADC6 RID: 44486 RVA: 0x004F3628 File Offset: 0x004F1828
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool flag = !this.HoverEnabled;
			if (!flag)
			{
				this.IsHovering = true;
				bool flag2 = this.hover != null;
				if (flag2)
				{
					this.hover.SetActive(true);
				}
				Action onPointerEnterEvent = this.OnPointerEnterEvent;
				if (onPointerEnterEvent != null)
				{
					onPointerEnterEvent();
				}
			}
		}

		// Token: 0x0600ADC7 RID: 44487 RVA: 0x004F367C File Offset: 0x004F187C
		public void OnPointerExit(PointerEventData eventData)
		{
			bool flag = !this.HoverEnabled;
			if (!flag)
			{
				this.IsHovering = false;
				bool flag2 = this.hover != null;
				if (flag2)
				{
					this.hover.SetActive(false);
				}
				Action onPointerExitEvent = this.OnPointerExitEvent;
				if (onPointerExitEvent != null)
				{
					onPointerExitEvent();
				}
			}
		}

		// Token: 0x0600ADC8 RID: 44488 RVA: 0x004F36D0 File Offset: 0x004F18D0
		private void OnDisable()
		{
			bool isHovering = this.IsHovering;
			if (isHovering)
			{
				this.IsHovering = false;
				bool flag = this.hover != null;
				if (flag)
				{
					this.hover.SetActive(false);
				}
				Action onPointerExitEvent = this.OnPointerExitEvent;
				if (onPointerExitEvent != null)
				{
					onPointerExitEvent();
				}
			}
		}

		// Token: 0x0600ADC9 RID: 44489 RVA: 0x004F3721 File Offset: 0x004F1921
		public virtual void OnItemHide()
		{
		}

		// Token: 0x0600ADCA RID: 44490 RVA: 0x004F3724 File Offset: 0x004F1924
		public void ResetSibling()
		{
			this.bottomLine.transform.SetAsLastSibling();
			this.selectedObject.transform.SetAsLastSibling();
			bool flag = this.hover;
			if (flag)
			{
				this.hover.transform.SetAsLastSibling();
			}
		}

		// Token: 0x04008629 RID: 34345
		[SerializeField]
		private HorizontalLayoutGroup horizontalLayoutGroup;

		// Token: 0x0400862A RID: 34346
		[SerializeField]
		private CButton button;

		// Token: 0x0400862B RID: 34347
		[SerializeField]
		private GameObject selectedObject;

		// Token: 0x0400862C RID: 34348
		[SerializeField]
		private GameObject bottomLine;

		// Token: 0x0400862D RID: 34349
		[SerializeField]
		private GameObject hover;

		// Token: 0x0400862E RID: 34350
		[SerializeField]
		protected TooltipInvoker tipDisplayer;

		// Token: 0x0400862F RID: 34351
		[SerializeField]
		private UIInteractionBehaviour interactionBehaviour;

		// Token: 0x04008630 RID: 34352
		[SerializeField]
		private string soundEffectSelect;

		// Token: 0x04008631 RID: 34353
		[SerializeField]
		private string soundEffectDeselect;

		// Token: 0x04008632 RID: 34354
		public Action OnPointerEnterEvent;

		// Token: 0x04008633 RID: 34355
		public Action OnPointerExitEvent;

		// Token: 0x04008636 RID: 34358
		private readonly List<RowCellContainer> _cellContainers = new List<RowCellContainer>();

		// Token: 0x04008637 RID: 34359
		private IReadOnlyList<ColumnDefinition> _columnDefinitions;

		// Token: 0x04008638 RID: 34360
		private int _currentIndex = -1;

		// Token: 0x04008639 RID: 34361
		private bool _interactionEnabled;

		// Token: 0x0400863A RID: 34362
		private Action<int, RowItem> _onButtonClicked;

		// Token: 0x0400863C RID: 34364
		private bool _selected;
	}
}
