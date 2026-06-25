using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200087C RID: 2172
	public class QualificationItem : MonoBehaviour
	{
		// Token: 0x06006865 RID: 26725 RVA: 0x002FBCD8 File Offset: 0x002F9ED8
		public void Set(Sprite sprite, string content)
		{
			this.icon.sprite = sprite;
			this.text.text = content;
		}

		// Token: 0x06006866 RID: 26726 RVA: 0x002FBCF5 File Offset: 0x002F9EF5
		public void Set(string spriteName, string content)
		{
			this.icon.SetSprite(spriteName, false, null);
			this.text.text = content;
		}

		// Token: 0x04004A1A RID: 18970
		[SerializeField]
		private CImage icon;

		// Token: 0x04004A1B RID: 18971
		[SerializeField]
		private TMP_Text text;
	}
}
