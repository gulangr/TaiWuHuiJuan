using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE9 RID: 3817
	public class TagCell : MonoBehaviour, ICellContent<List<string>>, ICellContent
	{
		// Token: 0x0600AF49 RID: 44873 RVA: 0x004FDAD8 File Offset: 0x004FBCD8
		public void SetData(List<string> data)
		{
			Rect rect = this.content.GetComponent<RectTransform>().rect;
			this._pageItemCount = (int)(rect.height / this.content.cellSize.y) * (int)(rect.width / this.content.cellSize.x);
			this._pageCount = (int)Math.Ceiling((double)((float)data.Count / (float)this._pageItemCount));
			this._currPage = 0;
			this._data = data;
			this.btnPrev.ClearAndAddListener(new Action(this.OnClickPrev));
			this.btnNext.ClearAndAddListener(new Action(this.OnClickNext));
			this.btnPrev.GetComponent<HoverHelper>().Init();
			this.btnNext.GetComponent<HoverHelper>().Init();
			this.Refresh();
		}

		// Token: 0x0600AF4A RID: 44874 RVA: 0x004FDBB4 File Offset: 0x004FBDB4
		private void Refresh()
		{
			int start = this._pageItemCount * this._currPage;
			for (int i = 0; i < this._pageItemCount; i++)
			{
				int index = start + i;
				Transform obj = this.content.transform.GetChild(i);
				bool flag = index < this._data.Count;
				if (flag)
				{
					obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = this._data[index];
					obj.gameObject.SetActive(true);
				}
				else
				{
					obj.gameObject.SetActive(false);
				}
			}
			this.btnPrev.gameObject.SetActive(this._currPage > 0 && this._pageCount > 1);
			this.btnNext.gameObject.SetActive(this._currPage < this._pageCount - 1);
		}

		// Token: 0x0600AF4B RID: 44875 RVA: 0x004FDCA0 File Offset: 0x004FBEA0
		private void OnClickPrev()
		{
			bool flag = this._currPage <= 0;
			if (!flag)
			{
				this._currPage--;
				this.Refresh();
			}
		}

		// Token: 0x0600AF4C RID: 44876 RVA: 0x004FDCD8 File Offset: 0x004FBED8
		private void OnClickNext()
		{
			bool flag = this._currPage >= this._pageCount - 1;
			if (!flag)
			{
				this._currPage++;
				this.Refresh();
			}
		}

		// Token: 0x040087DE RID: 34782
		[SerializeField]
		private GridLayoutGroup content;

		// Token: 0x040087DF RID: 34783
		[SerializeField]
		private CButton btnPrev;

		// Token: 0x040087E0 RID: 34784
		[SerializeField]
		private CButton btnNext;

		// Token: 0x040087E1 RID: 34785
		private List<string> _data;

		// Token: 0x040087E2 RID: 34786
		private int _pageItemCount;

		// Token: 0x040087E3 RID: 34787
		private int _pageCount;

		// Token: 0x040087E4 RID: 34788
		private int _currPage = 0;
	}
}
