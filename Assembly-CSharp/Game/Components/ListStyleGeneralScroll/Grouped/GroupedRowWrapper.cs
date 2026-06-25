using System;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.Grouped
{
	// Token: 0x02000EAD RID: 3757
	public class GroupedRowWrapper : MonoBehaviour
	{
		// Token: 0x170013C8 RID: 5064
		// (get) Token: 0x0600AE94 RID: 44692 RVA: 0x004F8FCB File Offset: 0x004F71CB
		public RowItem ContentRowItem
		{
			get
			{
				return this.contentRowItem;
			}
		}

		// Token: 0x170013C9 RID: 5065
		// (get) Token: 0x0600AE95 RID: 44693 RVA: 0x004F8FD3 File Offset: 0x004F71D3
		public bool HasBottomLine
		{
			get
			{
				return this.bottomLine != null;
			}
		}

		// Token: 0x0600AE96 RID: 44694 RVA: 0x004F8FE4 File Offset: 0x004F71E4
		public void ShowTitle(string title)
		{
			bool flag = this.titleLabel != null;
			if (flag)
			{
				this.titleLabel.text = (title ?? string.Empty);
			}
			bool flag2 = this.titleRoot != null;
			if (flag2)
			{
				this.titleRoot.SetActive(true);
			}
			bool flag3 = this.contentRoot != null;
			if (flag3)
			{
				this.contentRoot.SetActive(false);
			}
		}

		// Token: 0x0600AE97 RID: 44695 RVA: 0x004F9054 File Offset: 0x004F7254
		public void ShowContent()
		{
			bool flag = this.titleRoot != null;
			if (flag)
			{
				this.titleRoot.SetActive(false);
			}
			bool flag2 = this.contentRoot != null;
			if (flag2)
			{
				this.contentRoot.SetActive(true);
			}
		}

		// Token: 0x0600AE98 RID: 44696 RVA: 0x004F909C File Offset: 0x004F729C
		public void SetBottomLine(bool show)
		{
			bool flag = this.bottomLine != null;
			if (flag)
			{
				this.bottomLine.SetActive(show);
			}
		}

		// Token: 0x0600AE99 RID: 44697 RVA: 0x004F90C8 File Offset: 0x004F72C8
		public void SetTitleBack(Sprite sprite)
		{
			bool flag = this.titleImage;
			if (flag)
			{
				this.titleImage.sprite = sprite;
				this.titleImage.enabled = (sprite != null);
			}
		}

		// Token: 0x040086FB RID: 34555
		[SerializeField]
		private GameObject titleRoot;

		// Token: 0x040086FC RID: 34556
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040086FD RID: 34557
		[SerializeField]
		private CImage titleImage;

		// Token: 0x040086FE RID: 34558
		[SerializeField]
		private GameObject contentRoot;

		// Token: 0x040086FF RID: 34559
		[SerializeField]
		private RowItem contentRowItem;

		// Token: 0x04008700 RID: 34560
		[SerializeField]
		private GameObject bottomLine;
	}
}
