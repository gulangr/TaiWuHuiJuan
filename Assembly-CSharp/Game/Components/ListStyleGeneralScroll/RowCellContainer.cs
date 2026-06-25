using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA5 RID: 3749
	public class RowCellContainer : MonoBehaviour
	{
		// Token: 0x170013A2 RID: 5026
		// (get) Token: 0x0600ADAC RID: 44460 RVA: 0x004F30C0 File Offset: 0x004F12C0
		public Transform ContentRoot
		{
			get
			{
				return this.contentRoot;
			}
		}

		// Token: 0x0600ADAD RID: 44461 RVA: 0x004F30C8 File Offset: 0x004F12C8
		public void Init(ColumnDefinition columnDefinition, bool showRightLine)
		{
			this._columnDefinition = columnDefinition;
			columnDefinition.LayoutOption.ApplyTo(this.layoutElement);
			Transform cellGo = this.contentRoot.GetChild(0);
			this._cellContent = cellGo.GetComponent<ICellContent>();
			this.rightLine.SetActive(showRightLine);
		}

		// Token: 0x0600ADAE RID: 44462 RVA: 0x004F3115 File Offset: 0x004F1315
		public void SetData(object cellData)
		{
			this._columnDefinition.SetCell(this._cellContent, cellData);
		}

		// Token: 0x0600ADAF RID: 44463 RVA: 0x004F312B File Offset: 0x004F132B
		public void SetSelected(bool selected)
		{
			this._columnDefinition.SetSelected(this._cellContent, selected);
		}

		// Token: 0x0600ADB0 RID: 44464 RVA: 0x004F3144 File Offset: 0x004F1344
		public void SetSpecialBg(bool show)
		{
			bool flag = this.specialBg == null;
			if (!flag)
			{
				this.specialBg.enabled = show;
			}
		}

		// Token: 0x0600ADB1 RID: 44465 RVA: 0x004F3174 File Offset: 0x004F1374
		public void SetDisabled(bool disabled)
		{
			bool handled = this._columnDefinition.SetDisabled(this._cellContent, disabled);
			bool flag = this.canvasGroup != null;
			if (flag)
			{
				this.canvasGroup.alpha = ((handled || !disabled) ? 1f : 0.3f);
			}
		}

		// Token: 0x04008622 RID: 34338
		[SerializeField]
		private Transform contentRoot;

		// Token: 0x04008623 RID: 34339
		[SerializeField]
		private GameObject rightLine;

		// Token: 0x04008624 RID: 34340
		[SerializeField]
		private CImage specialBg;

		// Token: 0x04008625 RID: 34341
		[SerializeField]
		private LayoutElement layoutElement;

		// Token: 0x04008626 RID: 34342
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04008627 RID: 34343
		private ICellContent _cellContent;

		// Token: 0x04008628 RID: 34344
		private ColumnDefinition _columnDefinition;
	}
}
