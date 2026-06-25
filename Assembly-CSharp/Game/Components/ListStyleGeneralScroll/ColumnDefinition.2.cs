using System;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA0 RID: 3744
	public class ColumnDefinition<TRow, TCellData> : ColumnDefinition
	{
		// Token: 0x0600AD80 RID: 44416 RVA: 0x004F238C File Offset: 0x004F058C
		public override object CreateCellData(object rowData)
		{
			return this.CellDataGenerator((TRow)((object)rowData));
		}

		// Token: 0x0600AD81 RID: 44417 RVA: 0x004F23B4 File Offset: 0x004F05B4
		public override void SetCell(ICellContent cell, object cellData)
		{
			ICellContent<TCellData> typedCell = cell as ICellContent<TCellData>;
			bool flag = typedCell != null;
			if (flag)
			{
				typedCell.SetData((TCellData)((object)cellData));
			}
			else
			{
				Debug.LogError("格子类型不匹配: 期望 ICellContent<" + typeof(TCellData).Name + ">, 实际 " + cell.GetType().Name);
			}
		}

		// Token: 0x0600AD82 RID: 44418 RVA: 0x004F2410 File Offset: 0x004F0610
		public override void SetSelected(ICellContent cell, bool isSelected)
		{
			ICellContent<TCellData> typedCell = cell as ICellContent<TCellData>;
			bool flag = typedCell != null;
			if (flag)
			{
				typedCell.SetSelected(isSelected);
			}
			else
			{
				Debug.LogError("格子类型不匹配: 期望 ICellContent<" + typeof(TCellData).Name + ">, 实际 " + cell.GetType().Name);
			}
		}

		// Token: 0x0600AD83 RID: 44419 RVA: 0x004F2468 File Offset: 0x004F0668
		public override bool SetDisabled(ICellContent cell, bool disabled)
		{
			ICellContent<TCellData> typedCell = cell as ICellContent<TCellData>;
			bool flag = typedCell != null;
			bool result;
			if (flag)
			{
				result = typedCell.SetDisabled(disabled);
			}
			else
			{
				Debug.LogError("格子类型不匹配: 期望 ICellContent<" + typeof(TCellData).Name + ">, 实际 " + cell.GetType().Name);
				result = false;
			}
			return result;
		}

		// Token: 0x04008609 RID: 34313
		public Func<TRow, TCellData> CellDataGenerator;
	}
}
