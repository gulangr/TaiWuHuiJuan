using System;
using TMPro;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000782 RID: 1922
	public class SubCategoryItem : MonoBehaviour
	{
		// Token: 0x06005C64 RID: 23652 RVA: 0x002ABA90 File Offset: 0x002A9C90
		public void Set(bool isShow, string groupTitle)
		{
			TextMeshProUGUI textMeshProUGUI = this.groupTitleLabel;
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.transform.parent.gameObject.SetActive(isShow);
			}
			bool flag = isShow && this.groupTitleLabel;
			if (flag)
			{
				this.groupTitleLabel.text = groupTitle;
			}
		}

		// Token: 0x04003FDD RID: 16349
		[SerializeField]
		private TextMeshProUGUI groupTitleLabel;
	}
}
