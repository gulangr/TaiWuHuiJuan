using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB3 RID: 3763
	public class BookPageAndStateCell : MonoBehaviour, ICellContent<BookPageInfoAndStateData>, ICellContent
	{
		// Token: 0x0600AECB RID: 44747 RVA: 0x004FA44C File Offset: 0x004F864C
		public void SetData(BookPageInfoAndStateData data)
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
					pageItem.transform.GetChild(0).gameObject.SetActive(data.States[i]);
				}
				for (int j = pageCount; j < this._pageItems.Count; j++)
				{
					this._pageItems[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600AECC RID: 44748 RVA: 0x004FA578 File Offset: 0x004F8778
		private void SetEmpty()
		{
			foreach (BookPageItem item in this._pageItems)
			{
				item.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600AECD RID: 44749 RVA: 0x004FA5D4 File Offset: 0x004F87D4
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

		// Token: 0x0600AECE RID: 44750 RVA: 0x004FA640 File Offset: 0x004F8840
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

		// Token: 0x04008726 RID: 34598
		[SerializeField]
		private Transform pageContainer;

		// Token: 0x04008727 RID: 34599
		[SerializeField]
		private BookPageItem pageItemTemplate;

		// Token: 0x04008728 RID: 34600
		private readonly List<BookPageItem> _pageItems = new List<BookPageItem>();

		// Token: 0x04008729 RID: 34601
		private bool _isDisabled;
	}
}
