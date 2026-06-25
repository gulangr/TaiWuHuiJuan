using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ECC RID: 3788
	public class MerchantDropdownCell : MonoBehaviour, ICellContent<MerchantDropdownCellData>, ICellContent
	{
		// Token: 0x0600AF03 RID: 44803 RVA: 0x004FBEE4 File Offset: 0x004FA0E4
		private void Awake()
		{
			this._dropDownList.Clear();
			this._dropDownList.Add(LocalStringManager.Get(LanguageKey.LK_VillagerRole_Merchant_Random));
			foreach (MerchantTypeItem item in ((IEnumerable<MerchantTypeItem>)MerchantType.Instance))
			{
				bool flag = item.TemplateId >= 7;
				if (!flag)
				{
					this._dropDownList.Add(item.Name);
				}
			}
			this.dropdown.onValueChanged.RemoveAllListeners();
			this.dropdown.ClearOptions();
			this.dropdown.AddOptions(this._dropDownList);
		}

		// Token: 0x0600AF04 RID: 44804 RVA: 0x004FBFA4 File Offset: 0x004FA1A4
		public void SetData(MerchantDropdownCellData data)
		{
			TaiwuDomainMethod.AsyncCall.GetMerchantType(null, data.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				sbyte value = 0;
				Serializer.Deserialize(dataPool, offset, ref value);
				this.dropdown.value = (int)((value == 7) ? 0 : (value + 1));
				this.dropdown.onValueChanged.RemoveAllListeners();
				this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownChanged));
			});
			this._data = data;
		}

		// Token: 0x0600AF05 RID: 44805 RVA: 0x004FBFC7 File Offset: 0x004FA1C7
		private void OnDropdownChanged(int index)
		{
			Action<int, int> dropdownSetAction = this._data.DropdownSetAction;
			if (dropdownSetAction != null)
			{
				dropdownSetAction(index, this._data.CharacterId);
			}
		}

		// Token: 0x0400878A RID: 34698
		public CDropdown dropdown;

		// Token: 0x0400878B RID: 34699
		private MerchantDropdownCellData _data;

		// Token: 0x0400878C RID: 34700
		private readonly List<string> _dropDownList = new List<string>();
	}
}
