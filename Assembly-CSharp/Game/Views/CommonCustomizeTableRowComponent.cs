using System;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006E2 RID: 1762
	public class CommonCustomizeTableRowComponent : Refers
	{
		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x060053B3 RID: 21427 RVA: 0x0026CE47 File Offset: 0x0026B047
		public int ChildCount
		{
			get
			{
				return this.content.childCount;
			}
		}

		// Token: 0x060053B4 RID: 21428 RVA: 0x0026CE54 File Offset: 0x0026B054
		public Refers GetChildRefers(int index)
		{
			bool flag = index >= this.content.childCount;
			Refers result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.content.GetChild(index).GetComponent<Refers>();
			}
			return result;
		}

		// Token: 0x060053B5 RID: 21429 RVA: 0x0026CE90 File Offset: 0x0026B090
		public void InitClickEvent(bool isOn, int charId, Action<int> onClicked)
		{
			this._onClicked = onClicked;
		}

		// Token: 0x060053B6 RID: 21430 RVA: 0x0026CE9A File Offset: 0x0026B09A
		public void Refresh(bool isOn)
		{
		}

		// Token: 0x060053B7 RID: 21431 RVA: 0x0026CE9D File Offset: 0x0026B09D
		public void OnRowClicked(bool _)
		{
			Action<int> onClicked = this._onClicked;
			if (onClicked != null)
			{
				onClicked(this.DataId);
			}
		}

		// Token: 0x04003898 RID: 14488
		public RectTransform content;

		// Token: 0x04003899 RID: 14489
		public int DataId;

		// Token: 0x0400389A RID: 14490
		private Action<int> _onClicked;
	}
}
