using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008DA RID: 2266
	public class ModUploadListItem : MonoBehaviour
	{
		// Token: 0x06006CB3 RID: 27827 RVA: 0x0032263D File Offset: 0x0032083D
		public void Refresh(string title, bool isLocal)
		{
			this.textTitle.SetText(title, true);
			this.imageState.sprite = (isLocal ? this.spriteStateLocal : this.spriteStateUploaded);
		}

		// Token: 0x04004EE4 RID: 20196
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04004EE5 RID: 20197
		[SerializeField]
		private CImage imageState;

		// Token: 0x04004EE6 RID: 20198
		[SerializeField]
		private Sprite spriteStateLocal;

		// Token: 0x04004EE7 RID: 20199
		[SerializeField]
		private Sprite spriteStateUploaded;
	}
}
