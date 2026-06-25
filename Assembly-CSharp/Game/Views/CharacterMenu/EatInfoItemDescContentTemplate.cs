using System;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B6F RID: 2927
	public class EatInfoItemDescContentTemplate : MonoBehaviour
	{
		// Token: 0x060090C3 RID: 37059 RVA: 0x004378BF File Offset: 0x00435ABF
		public void Setup(string title, string content)
		{
			this.titleTxt.text = title;
			this.contentTxt.text = content;
			TMPTextSpriteHelper tmptextSpriteHelper = this.spriteParser;
			if (tmptextSpriteHelper != null)
			{
				tmptextSpriteHelper.Parse();
			}
		}

		// Token: 0x04006F77 RID: 28535
		[SerializeField]
		private TextMeshProUGUI titleTxt;

		// Token: 0x04006F78 RID: 28536
		[SerializeField]
		private TextMeshProUGUI contentTxt;

		// Token: 0x04006F79 RID: 28537
		[SerializeField]
		private TMPTextSpriteHelper spriteParser;
	}
}
