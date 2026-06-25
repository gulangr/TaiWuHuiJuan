using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED5 RID: 3797
	public class ReadingBookIconAndNameCell : MonoBehaviour, ICellContent<ReadingBookIconAndNameCellData>, ICellContent
	{
		// Token: 0x0600AF20 RID: 44832 RVA: 0x004FC9E8 File Offset: 0x004FABE8
		public void SetData(ReadingBookIconAndNameCellData data)
		{
			ITradeableContent itemData = data.ItemData;
			bool flag = itemData == null;
			if (flag)
			{
				this.label.text = string.Empty;
				this.completeMark.SetActive(false);
			}
			else
			{
				string nameText = itemData.GetName(false).SetColor("brightyellow");
				this.label.text = nameText;
				TMPTextSpriteHelper spriteHelper;
				bool flag2 = this.label.TryGetComponent<TMPTextSpriteHelper>(out spriteHelper);
				if (flag2)
				{
					spriteHelper.Parse();
				}
				string sprite = ItemTemplateHelper.GetIcon(itemData.Key.ItemType, itemData.Key.TemplateId);
				this.icon.SetSprite(sprite, false, null);
				sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
				this.gradeIcon.SetSprite("ui9_icon_item_grade_" + grade.ToString(), false, null);
				this.completeMark.SetActive(data.IsFinished);
			}
		}

		// Token: 0x040087A0 RID: 34720
		[SerializeField]
		private CImage icon;

		// Token: 0x040087A1 RID: 34721
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x040087A2 RID: 34722
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040087A3 RID: 34723
		[SerializeField]
		private GameObject completeMark;
	}
}
