using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE8 RID: 3816
	public class SwitchCell : MonoBehaviour, ICellContent<SwitchCellData>, ICellContent
	{
		// Token: 0x0600AF47 RID: 44871 RVA: 0x004FDA5C File Offset: 0x004FBC5C
		public void SetData(SwitchCellData data)
		{
			this.toggle.onValueChanged.RemoveAllListeners();
			this.toggle.isOn = data.GetAction(data.Id);
			this.toggle.onValueChanged.AddListener(delegate(bool value)
			{
				data.SetAction(data.Id, value);
			});
		}

		// Token: 0x040087DD RID: 34781
		[SerializeField]
		private CToggle toggle;
	}
}
