using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EBC RID: 3772
	public class IconAndTextListCell : MonoBehaviour, ICellContent<List<IconAndTextCellData>>, ICellContent
	{
		// Token: 0x0600AEE9 RID: 44777 RVA: 0x004FAE98 File Offset: 0x004F9098
		public void SetData(List<IconAndTextCellData> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				Transform line = this.content.GetChild(i);
				CImage icon = line.GetChild(0).GetComponent<CImage>();
				TextMeshProUGUI label = line.GetChild(1).GetComponent<TextMeshProUGUI>();
				bool showIcon = data[i].ShowIcon;
				if (showIcon)
				{
					icon.SetSprite(data[i].IconName, false, null);
					icon.gameObject.SetActive(true);
				}
				else
				{
					icon.gameObject.SetActive(false);
				}
				label.text = data[i].Text;
				line.gameObject.SetActive(true);
			}
			for (int j = data.Count; j < this.content.childCount; j++)
			{
				this.content.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x0400874A RID: 34634
		[SerializeField]
		private Transform content;
	}
}
