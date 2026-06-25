using System;
using System.Collections.Generic;
using Game.Views.Main.Reading;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED7 RID: 3799
	public class ReadingBookPageInfoCell : MonoBehaviour, ICellContent<ReadingBookPageInfoCellData>, ICellContent
	{
		// Token: 0x0600AF23 RID: 44835 RVA: 0x004FCB10 File Offset: 0x004FAD10
		public void SetData(ReadingBookPageInfoCellData data)
		{
			bool flag = data.PageStates == null || data.PageProgress == null;
			if (flag)
			{
				this.HideAllPages();
			}
			else
			{
				int pageCount = data.PageStates.Length;
				this.EnsurePageCount(pageCount);
				for (int i = 0; i < pageCount; i++)
				{
					ReadingBookPages page = this._pageItems[i];
					page.gameObject.SetActive(true);
					ReadingDisplayHelper.SetPageCompleteState(data.PageStates[i], page.pageIcon);
					bool isGeneral = (data.IsCombatBook && i == 0) || !data.IsCombatBook;
					bool directRead = data.PageTypes == null || !data.PageTypes.CheckIndex(i) || data.PageTypes[i] == 0;
					bool flag2 = isGeneral;
					if (flag2)
					{
						page.pageProgress.sprite = page.spriteProgressGeneral;
					}
					else
					{
						bool flag3 = directRead;
						if (flag3)
						{
							page.pageProgress.sprite = page.spriteProgressDirect;
						}
						else
						{
							page.pageProgress.sprite = page.spriteProgressReverse;
						}
					}
					page.pageProgress.fillAmount = (float)data.PageProgress[i] / 100f;
					bool flag4 = page.supply != null;
					if (flag4)
					{
						page.supply.SetActive(data.SupplyStates != null && data.SupplyStates.CheckIndex(i) && data.SupplyStates[i]);
					}
				}
				for (int j = pageCount; j < this._pageItems.Count; j++)
				{
					this._pageItems[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600AF24 RID: 44836 RVA: 0x004FCCB8 File Offset: 0x004FAEB8
		private void EnsurePageCount(int count)
		{
			CommonUtils.PrepareEnoughChildren(this.pageRoot, this.pageTemplate.gameObject, count, null);
			this._pageItems.Clear();
			for (int i = 0; i < count; i++)
			{
				ReadingBookPages pageItem = this.pageRoot.GetChild(i).GetComponent<ReadingBookPages>();
				this._pageItems.Add(pageItem);
			}
		}

		// Token: 0x0600AF25 RID: 44837 RVA: 0x004FCD24 File Offset: 0x004FAF24
		private void HideAllPages()
		{
			for (int i = 0; i < this._pageItems.Count; i++)
			{
				this._pageItems[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x040087A9 RID: 34729
		[SerializeField]
		private Transform pageRoot;

		// Token: 0x040087AA RID: 34730
		[SerializeField]
		private ReadingBookPages pageTemplate;

		// Token: 0x040087AB RID: 34731
		private readonly List<ReadingBookPages> _pageItems = new List<ReadingBookPages>();
	}
}
