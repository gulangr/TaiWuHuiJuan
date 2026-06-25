using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB1 RID: 3761
	public class BookIconAndNameCell : MonoBehaviour, ICellContent<ITradeableContent>, ICellContent
	{
		// Token: 0x0600AEC8 RID: 44744 RVA: 0x004FA308 File Offset: 0x004F8508
		public void SetData(ITradeableContent data)
		{
			bool finished = true;
			bool flag = data.BookPageProgress == null;
			if (flag)
			{
				finished = false;
			}
			else
			{
				foreach (sbyte progress in data.BookPageProgress)
				{
					bool flag2 = progress != 100;
					if (flag2)
					{
						finished = false;
						break;
					}
				}
			}
			string txt = data.GetName(false).SetColor("brightyellow");
			bool flag3 = finished;
			if (flag3)
			{
				this.label.text = "<SpName=ui9_icon_finish_reading> " + txt;
				this.label.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			else
			{
				this.label.text = txt;
				this.label.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			string sprite = ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId);
			this.icon.SetSprite(sprite, false, null);
			sbyte grade = ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId);
			this.gradeIcon.SetSprite("ui9_icon_item_grade_" + grade.ToString(), false, null);
		}

		// Token: 0x04008721 RID: 34593
		[SerializeField]
		private CImage icon;

		// Token: 0x04008722 RID: 34594
		[SerializeField]
		private CImage gradeIcon;

		// Token: 0x04008723 RID: 34595
		[SerializeField]
		private TextMeshProUGUI label;
	}
}
