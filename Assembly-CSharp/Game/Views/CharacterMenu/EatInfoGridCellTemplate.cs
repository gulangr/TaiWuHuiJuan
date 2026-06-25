using System;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B6E RID: 2926
	public class EatInfoGridCellTemplate : MonoBehaviour
	{
		// Token: 0x060090C1 RID: 37057 RVA: 0x00437854 File Offset: 0x00435A54
		public void Setup(string title, string iconName, int value, bool showMonthIcon)
		{
			this.perMonth.SetActive(showMonthIcon);
			this.titleTxt.text = title;
			this.icon.SetSprite(iconName, false, null);
			this.valueTxt.text = ((value > 0) ? value.ToString() : value.ToString().SetColor(Color.red));
		}

		// Token: 0x04006F73 RID: 28531
		[SerializeField]
		private CImage icon;

		// Token: 0x04006F74 RID: 28532
		[SerializeField]
		private TextMeshProUGUI titleTxt;

		// Token: 0x04006F75 RID: 28533
		[SerializeField]
		private TextMeshProUGUI valueTxt;

		// Token: 0x04006F76 RID: 28534
		[SerializeField]
		private GameObject perMonth;
	}
}
