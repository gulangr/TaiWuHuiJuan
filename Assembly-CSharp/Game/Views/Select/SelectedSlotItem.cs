using System;
using Game.Components.ListStyleGeneralScroll;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x0200079F RID: 1951
	public class SelectedSlotItem : MonoBehaviour
	{
		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x06005E55 RID: 24149 RVA: 0x002B539C File Offset: 0x002B359C
		public GameObject EmptySlot
		{
			get
			{
				return this.emptySlot;
			}
		}

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06005E56 RID: 24150 RVA: 0x002B53A4 File Offset: 0x002B35A4
		public RowItem CurrentRowItem
		{
			get
			{
				return this.currentRowItem;
			}
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x002B53AC File Offset: 0x002B35AC
		public void RestoreRowItemReference()
		{
			bool flag = this.currentRowItem != null && this.currentRowItem.transform != null && this.currentRowItem.transform.parent == base.transform;
			if (!flag)
			{
				this.currentRowItem = null;
				for (int i = 0; i < base.transform.childCount; i++)
				{
					Transform child = base.transform.GetChild(i);
					RowItem rowItem = child.GetComponent<RowItem>();
					bool flag2 = rowItem != null;
					if (flag2)
					{
						this.currentRowItem = rowItem;
						break;
					}
				}
			}
		}

		// Token: 0x06005E58 RID: 24152 RVA: 0x002B5450 File Offset: 0x002B3650
		public void PrepareStructure(RowItem template)
		{
			bool flag = template == null;
			if (!flag)
			{
				bool flag2 = this.currentRowItem != null;
				if (flag2)
				{
					Object.DestroyImmediate(this.currentRowItem.gameObject);
					this.currentRowItem = null;
				}
				this.currentRowItem = Object.Instantiate<RowItem>(template, base.transform);
				bool shouldShowRowItem = this.emptySlot == null || !this.emptySlot.activeSelf;
				this.currentRowItem.gameObject.SetActive(shouldShowRowItem);
				this.RestoreRowItemReference();
			}
		}

		// Token: 0x06005E59 RID: 24153 RVA: 0x002B54E0 File Offset: 0x002B36E0
		public void SetSlotState(bool isEmpty)
		{
			bool flag = this.emptySlot != null;
			if (flag)
			{
				this.emptySlot.SetActive(isEmpty);
			}
			this.RestoreRowItemReference();
			bool flag2 = this.currentRowItem != null;
			if (flag2)
			{
				this.currentRowItem.gameObject.SetActive(!isEmpty);
			}
		}

		// Token: 0x04004143 RID: 16707
		[SerializeField]
		private GameObject emptySlot;

		// Token: 0x04004144 RID: 16708
		[SerializeField]
		private RowItem currentRowItem;
	}
}
