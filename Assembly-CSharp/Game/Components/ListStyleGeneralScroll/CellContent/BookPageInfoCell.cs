using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB5 RID: 3765
	public class BookPageInfoCell : MonoBehaviour, ICellContent<BookPageInfoData>, ICellContent
	{
		// Token: 0x0600AED1 RID: 44753 RVA: 0x004FA6D0 File Offset: 0x004F88D0
		public void SetData(BookPageInfoData data)
		{
			ITradeableContent itemData = data.ItemData;
			bool flag = itemData == null || itemData.BookPageStates == null || itemData.BookPageProgress == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				int pageCount = itemData.BookPageStates.Length;
				this.EnsurePageItemCount(pageCount);
				for (int i = 0; i < pageCount; i++)
				{
					BookPageItem pageItem = this._pageItems[i];
					pageItem.gameObject.SetActive(true);
					sbyte pageType = (itemData.BookPageTypes != null && itemData.BookPageTypes.CheckIndex(i)) ? itemData.BookPageTypes[i] : -1;
					pageItem.SetData(itemData.BookPageStates[i], itemData.BookPageProgress[i], pageType, itemData.IsCombatBook, i, this._isDisabled);
				}
				for (int j = pageCount; j < this._pageItems.Count; j++)
				{
					this._pageItems[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600AED2 RID: 44754 RVA: 0x004FA7D4 File Offset: 0x004F89D4
		public void SetData(sbyte[] bookPageStates)
		{
			int pageCount = bookPageStates.Length;
			this.EnsurePageItemCount(pageCount);
			for (int i = 0; i < pageCount; i++)
			{
				BookPageItem pageItem = this._pageItems[i];
				pageItem.gameObject.SetActive(true);
				pageItem.SetData(bookPageStates[i], 0, this._isDisabled);
			}
			for (int j = pageCount; j < this._pageItems.Count; j++)
			{
				this._pageItems[j].gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AED3 RID: 44755 RVA: 0x004FA868 File Offset: 0x004F8A68
		private void SetEmpty()
		{
			foreach (BookPageItem item in this._pageItems)
			{
				item.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AED4 RID: 44756 RVA: 0x004FA8C4 File Offset: 0x004F8AC4
		private void EnsurePageItemCount(int count)
		{
			CommonUtils.PrepareEnoughChildren(this.pageContainer, this.pageItemTemplate.gameObject, count, null);
			this._pageItems.Clear();
			for (int i = 0; i < count; i++)
			{
				BookPageItem child = this.pageContainer.GetChild(i).GetComponent<BookPageItem>();
				this._pageItems.Add(child);
			}
		}

		// Token: 0x0600AED5 RID: 44757 RVA: 0x004FA930 File Offset: 0x004F8B30
		public void SetDisabled(bool disabled)
		{
			this._isDisabled = disabled;
			foreach (BookPageItem item in this._pageItems)
			{
				bool activeSelf = item.gameObject.activeSelf;
				if (activeSelf)
				{
					item.SetDisabled(disabled);
				}
			}
		}

		// Token: 0x0400872B RID: 34603
		[SerializeField]
		private Transform pageContainer;

		// Token: 0x0400872C RID: 34604
		[SerializeField]
		private BookPageItem pageItemTemplate;

		// Token: 0x0400872D RID: 34605
		private readonly List<BookPageItem> _pageItems = new List<BookPageItem>();

		// Token: 0x0400872E RID: 34606
		private bool _isDisabled;
	}
}
