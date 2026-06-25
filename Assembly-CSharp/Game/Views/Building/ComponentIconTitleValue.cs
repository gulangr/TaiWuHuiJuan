using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE4 RID: 3044
	public class ComponentIconTitleValue : MonoBehaviour
	{
		// Token: 0x06009A75 RID: 39541 RVA: 0x00485561 File Offset: 0x00483761
		public void Set(string sprite, string title, string value)
		{
			this.imgIcon.SetSprite(sprite, false, null);
			this.txtTitle.text = title;
			this.txtValue.text = value;
		}

		// Token: 0x06009A76 RID: 39542 RVA: 0x0048558D File Offset: 0x0048378D
		public void SetValue(string value)
		{
			this.txtValue.text = value;
		}

		// Token: 0x06009A77 RID: 39543 RVA: 0x0048559D File Offset: 0x0048379D
		public void SetValue(string title, string value)
		{
			this.txtTitle.text = title;
			this.txtValue.text = value;
		}

		// Token: 0x04007757 RID: 30551
		[SerializeField]
		private CImage imgIcon;

		// Token: 0x04007758 RID: 30552
		[SerializeField]
		private TextMeshProUGUI txtTitle;

		// Token: 0x04007759 RID: 30553
		[SerializeField]
		private TextMeshProUGUI txtValue;
	}
}
