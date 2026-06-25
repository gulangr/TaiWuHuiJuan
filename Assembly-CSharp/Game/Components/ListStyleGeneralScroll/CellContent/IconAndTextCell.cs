using System;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EBB RID: 3771
	public class IconAndTextCell : MonoBehaviour, ICellContent<IconAndTextCellData>, ICellContent
	{
		// Token: 0x0600AEE7 RID: 44775 RVA: 0x004FACFC File Offset: 0x004F8EFC
		public void SetData(IconAndTextCellData data)
		{
			bool flag = this.icon != null;
			if (flag)
			{
				bool emptyName = string.IsNullOrEmpty(data.IconName);
				bool showIconFlag = data.ShowIcon && (!data.HideIconIfEmpty || !emptyName);
				this.icon.gameObject.SetActive(showIconFlag);
				bool flag2 = this.iconElement != null;
				if (flag2)
				{
					this.iconElement.enabled = !data.UseNativeSize;
				}
				bool flag3 = showIconFlag;
				if (flag3)
				{
					this.icon.SetSprite(data.IconName, data.UseNativeSize, delegate
					{
						Sprite temp = this.icon.sprite;
						bool flag6 = this.icon.sprite == null && data.HideIconIfEmpty;
						if (flag6)
						{
							this.icon.gameObject.SetActive(false);
						}
					});
				}
			}
			bool flag4 = this.label != null;
			if (flag4)
			{
				this.label.text = (data.Text ?? string.Empty);
				TMPTextEnLayoutHelper.ApplyIconAndTextListLabel(this.label);
				this.label.ForceMeshUpdate(false, false);
			}
			bool flag5 = this.icon != null && this.label != null;
			if (flag5)
			{
				bool labelInFront = data.LabelInFront;
				if (labelInFront)
				{
					this.icon.transform.SetAsLastSibling();
				}
				else
				{
					this.label.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x04008747 RID: 34631
		[SerializeField]
		private CImage icon;

		// Token: 0x04008748 RID: 34632
		[SerializeField]
		private LayoutElement iconElement;

		// Token: 0x04008749 RID: 34633
		[SerializeField]
		private TextMeshProUGUI label;
	}
}
