using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F98 RID: 3992
	public class SpriteLabel : MonoBehaviour
	{
		// Token: 0x0600B78E RID: 46990 RVA: 0x0053A24C File Offset: 0x0053844C
		public void Refresh(string str)
		{
			CommonUtils.PrepareEnoughChildren(base.transform, this.template.gameObject, str.Length, null);
			for (int i = 0; i < str.Length; i++)
			{
				char inputChar = str[i];
				int index = this.chars.IndexOf(inputChar);
				Transform child = base.transform.GetChild(i);
				bool flag = index < 0;
				if (flag)
				{
					child.gameObject.SetActive(false);
				}
				else
				{
					child.gameObject.SetActive(true);
					child.gameObject.name = inputChar.ToString();
					CImage image = child.GetComponent<CImage>();
					image.sprite = this.sprites[index];
					image.SetNativeSize();
				}
			}
		}

		// Token: 0x04008E87 RID: 36487
		[SerializeField]
		private List<char> chars;

		// Token: 0x04008E88 RID: 36488
		[SerializeField]
		private List<Sprite> sprites;

		// Token: 0x04008E89 RID: 36489
		[SerializeField]
		private CImage template;
	}
}
