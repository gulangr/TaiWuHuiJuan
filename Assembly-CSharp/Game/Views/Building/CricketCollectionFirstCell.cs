using System;
using Config;
using Game.Components.ListStyleGeneralScroll;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BDC RID: 3036
	public class CricketCollectionFirstCell : MonoBehaviour, ICellContent<CricketCollectionFirstCellData>, ICellContent
	{
		// Token: 0x060098CC RID: 39116 RVA: 0x004723E8 File Offset: 0x004705E8
		public void SetData(CricketCollectionFirstCellData data)
		{
			ITradeableContent item = data.ItemData;
			this.label.text = item.GetName(false);
			bool flag = item.Key.ItemType == 11;
			if (flag)
			{
				CricketPartsItem colorConfig = CricketParts.Instance[item.CricketColorId];
				this.icon.SetSprite(colorConfig.Icon, false, null);
				sbyte grade = new ValueTuple<short, short>(item.CricketColorId, item.CricketPartId).CalcCricketGrade();
				this.gradeIcon.SetSprite("ui9_icon_item_grade_" + grade.ToString(), false, null);
			}
			else
			{
				this.icon.SetSprite(ItemTemplateHelper.GetIcon(item.Key.ItemType, item.Key.TemplateId), false, null);
				sbyte grade2 = ItemTemplateHelper.GetGrade(item.Key.ItemType, item.Key.TemplateId);
				this.gradeIcon.SetSprite("ui9_icon_item_grade_" + grade2.ToString(), false, null);
			}
			bool isInCollection = data.IsInCollection;
			if (isInCollection)
			{
				this.itemStateIcon.SetSprite("ui9_icon_item_state_equip_1", false, null);
				this.itemStateIcon.gameObject.SetActive(true);
				TooltipInvoker tips = this.itemStateIcon.GetComponent<TooltipInvoker>();
				tips.PresetParam = new string[]
				{
					LocalStringManager.Get("LK_CricketCollection_InCollection")
				};
				tips.enabled = true;
			}
			else
			{
				this.itemStateIcon.gameObject.SetActive(false);
			}
		}

		// Token: 0x04007599 RID: 30105
		[SerializeField]
		private CImage icon;

		// Token: 0x0400759A RID: 30106
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x0400759B RID: 30107
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x0400759C RID: 30108
		[SerializeField]
		private CImage itemStateIcon;
	}
}
