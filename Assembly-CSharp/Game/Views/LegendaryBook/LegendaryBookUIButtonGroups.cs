using System;
using UnityEngine;

namespace Game.Views.LegendaryBook
{
	// Token: 0x0200098C RID: 2444
	public class LegendaryBookUIButtonGroups : MonoBehaviour
	{
		// Token: 0x0600759E RID: 30110 RVA: 0x0036C9B7 File Offset: 0x0036ABB7
		public LegendaryBookUIItem GetByIndex(int index)
		{
			return this.items[index];
		}

		// Token: 0x17000D3E RID: 3390
		// (get) Token: 0x0600759F RID: 30111 RVA: 0x0036C9C1 File Offset: 0x0036ABC1
		public int CurrentSelected
		{
			get
			{
				return this.currentSelected;
			}
		}

		// Token: 0x17000D3F RID: 3391
		// (get) Token: 0x060075A0 RID: 30112 RVA: 0x0036C9C9 File Offset: 0x0036ABC9
		public int Count
		{
			get
			{
				return this.items.Length;
			}
		}

		// Token: 0x060075A1 RID: 30113 RVA: 0x0036C9D4 File Offset: 0x0036ABD4
		private void Awake()
		{
			bool flag = this.items == null || this.items.Length == 0;
			if (flag)
			{
				this.items = base.GetComponentsInChildren<LegendaryBookUIItem>();
			}
			for (int i = 0; i < this.items.Length; i++)
			{
				int tempId = i;
				this.items[i].OnClick = delegate()
				{
					this.ClickChild(tempId);
				};
			}
		}

		// Token: 0x060075A2 RID: 30114 RVA: 0x0036CA50 File Offset: 0x0036AC50
		private void ClickChild(int index)
		{
			bool flag = this.CheckCanSelect != null && !this.CheckCanSelect(index);
			if (!flag)
			{
				int previousIndex = this.currentSelected;
				bool sameIndex = index == this.currentSelected;
				bool flag2 = sameIndex;
				if (!flag2)
				{
					this.currentSelected = index;
					Action<int, int> action = this.onActiveIndexChange;
					if (action != null)
					{
						action(this.currentSelected, previousIndex);
					}
					this.RefreshDisplay(true);
				}
			}
		}

		// Token: 0x060075A3 RID: 30115 RVA: 0x0036CAC0 File Offset: 0x0036ACC0
		private void RefreshDisplay(bool refreshRelative = true)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				this.items[i].selectedObs.SetActive(this.currentSelected == i);
			}
			if (refreshRelative)
			{
				this.relatedGroup.SetByRelative(this.currentSelected);
			}
		}

		// Token: 0x060075A4 RID: 30116 RVA: 0x0036CB1C File Offset: 0x0036AD1C
		private void SetByRelative(int index)
		{
			int previousIndex = this.currentSelected;
			this.currentSelected = index;
			this.RefreshDisplay(false);
		}

		// Token: 0x060075A5 RID: 30117 RVA: 0x0036CB40 File Offset: 0x0036AD40
		public void Set(int index)
		{
			this.ClickChild(index);
		}

		// Token: 0x060075A6 RID: 30118 RVA: 0x0036CB4C File Offset: 0x0036AD4C
		public void SetWithoutNotify(int index)
		{
			int previousIndex = this.currentSelected;
			this.currentSelected = index;
			this.RefreshDisplay(true);
		}

		// Token: 0x060075A7 RID: 30119 RVA: 0x0036CB70 File Offset: 0x0036AD70
		public void ResetItemsHint()
		{
			foreach (LegendaryBookUIItem item in this.items)
			{
				item.transform.parent = base.transform;
				item.HideHint();
			}
		}

		// Token: 0x060075A8 RID: 30120 RVA: 0x0036CBB4 File Offset: 0x0036ADB4
		public void ResetItemParent(int index)
		{
			bool flag = index < 0 || index >= this.items.Length;
			if (!flag)
			{
				this.items[index].transform.parent = base.transform;
			}
		}

		// Token: 0x060075A9 RID: 30121 RVA: 0x0036CBF6 File Offset: 0x0036ADF6
		public LegendaryBookUIItem GetItem(int targetIndex)
		{
			return this.items[targetIndex];
		}

		// Token: 0x04005858 RID: 22616
		[SerializeField]
		private LegendaryBookUIItem[] items;

		// Token: 0x04005859 RID: 22617
		[SerializeField]
		private LegendaryBookUIButtonGroups relatedGroup;

		// Token: 0x0400585A RID: 22618
		public Action<int, int> onActiveIndexChange;

		// Token: 0x0400585B RID: 22619
		private int currentSelected = -1;

		// Token: 0x0400585C RID: 22620
		public Func<int, bool> CheckCanSelect;
	}
}
