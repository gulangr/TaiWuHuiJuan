using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008B1 RID: 2225
	public class TooltipItemProperty : MonoBehaviour
	{
		// Token: 0x17000C97 RID: 3223
		// (get) Token: 0x06006A70 RID: 27248 RVA: 0x00311E5E File Offset: 0x0031005E
		public DisableStyleRoot StyleRoot
		{
			get
			{
				return this.styleRoot;
			}
		}

		// Token: 0x06006A71 RID: 27249 RVA: 0x00311E68 File Offset: 0x00310068
		public void Set(string iconSpriteName, string title, string value, bool canHideIcon = true)
		{
			bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(iconSpriteName);
			bool flag = !isNullOrWhiteSpace;
			if (flag)
			{
				CImage cimage = this.icon;
				if (cimage != null)
				{
					cimage.SetSprite(iconSpriteName, false, null);
				}
			}
			if (canHideIcon)
			{
				CImage cimage2 = this.icon;
				if (cimage2 != null)
				{
					cimage2.gameObject.SetActive(!isNullOrWhiteSpace);
				}
			}
			TextMeshProUGUI textMeshProUGUI = this.titleText;
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText(title, true);
			}
			this.SetValue(value);
		}

		// Token: 0x06006A72 RID: 27250 RVA: 0x00311ED7 File Offset: 0x003100D7
		public void SetValue(string value)
		{
			TextMeshProUGUI textMeshProUGUI = this.valueText;
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText((value != null) ? value.ColorReplace() : null, true);
			}
			TMPTextSpriteHelper tmptextSpriteHelper = this.spriteHelper;
			if (tmptextSpriteHelper != null)
			{
				tmptextSpriteHelper.Parse();
			}
		}

		// Token: 0x04004CE3 RID: 19683
		[SerializeField]
		protected CImage icon;

		// Token: 0x04004CE4 RID: 19684
		[SerializeField]
		protected TextMeshProUGUI titleText;

		// Token: 0x04004CE5 RID: 19685
		[SerializeField]
		protected TextMeshProUGUI valueText;

		// Token: 0x04004CE6 RID: 19686
		[SerializeField]
		protected DisableStyleRoot styleRoot;

		// Token: 0x04004CE7 RID: 19687
		[SerializeField]
		protected TMPTextSpriteHelper spriteHelper;
	}
}
