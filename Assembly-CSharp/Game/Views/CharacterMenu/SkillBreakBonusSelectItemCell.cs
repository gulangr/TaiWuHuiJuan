using System;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B84 RID: 2948
	public class SkillBreakBonusSelectItemCell : MonoBehaviour, ICellContent<ItemDisplayData>, ICellContent
	{
		// Token: 0x0600919E RID: 37278 RVA: 0x0043D903 File Offset: 0x0043BB03
		public void SetData(ItemDisplayData itemData)
		{
			this.itemBack.Set(itemData, false);
			this.nameLabel.text = ItemTemplateHelper.GetName(itemData.RealKey.ItemType, itemData.RealKey.TemplateId);
		}

		// Token: 0x04007027 RID: 28711
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04007028 RID: 28712
		[SerializeField]
		private ItemBack itemBack;
	}
}
