using System;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA7 RID: 3751
	public class TableHeadCell : MonoBehaviour
	{
		// Token: 0x0600ADCC RID: 44492 RVA: 0x004F3798 File Offset: 0x004F1998
		public void Init(ColumnDefinition columnDefinition, bool showRightLine, int columnIndex, Action<int, short> onSortClicked)
		{
			this._labelGetter = columnDefinition.TableHeadLabel;
			TMP_Text tmp_Text = this.label;
			Func<string> labelGetter = this._labelGetter;
			tmp_Text.text = (((labelGetter != null) ? labelGetter() : null) ?? string.Empty);
			this.RefreshLabelRule();
			columnDefinition.LayoutOption.ApplyTo(this.layoutElement);
			this.rightLine.SetActive(showRightLine);
			this._sortId = columnDefinition.SortId;
			this._columnIndex = columnIndex;
			this._onSortClicked = onSortClicked;
			this.button.interactable = false;
			this.numberObject.SetActive(false);
			this.arrowImage.gameObject.SetActive(false);
			bool flag = this.tips && columnDefinition.RefreshTips != null;
			if (flag)
			{
				columnDefinition.RefreshTips(this.tips);
			}
		}

		// Token: 0x0600ADCD RID: 44493 RVA: 0x004F3875 File Offset: 0x004F1A75
		private void OnDisable()
		{
			this._languageType = new LocalStringManager.LanguageType?(LocalStringManager.CurLanguageType);
		}

		// Token: 0x0600ADCE RID: 44494 RVA: 0x004F3888 File Offset: 0x004F1A88
		private void OnEnable()
		{
			bool flag = this._languageType != null && this._languageType.Value != LocalStringManager.CurLanguageType;
			if (flag)
			{
				this.RefreshLabel();
				this._languageType = null;
			}
		}

		// Token: 0x0600ADCF RID: 44495 RVA: 0x004F38D4 File Offset: 0x004F1AD4
		public void Init(ColumnDefinition columnDefinition, bool showRightLine)
		{
			this.Init(columnDefinition, showRightLine, -1, null);
		}

		// Token: 0x0600ADD0 RID: 44496 RVA: 0x004F38E4 File Offset: 0x004F1AE4
		private void OnButtonClicked()
		{
			bool flag = this._sortId >= 0;
			if (flag)
			{
				Action<int, short> onSortClicked = this._onSortClicked;
				if (onSortClicked != null)
				{
					onSortClicked(this._columnIndex, this._sortId);
				}
			}
		}

		// Token: 0x0600ADD1 RID: 44497 RVA: 0x004F3922 File Offset: 0x004F1B22
		public void SetSortState(int sortOrder, ESortDirection direction)
		{
		}

		// Token: 0x0600ADD2 RID: 44498 RVA: 0x004F3928 File Offset: 0x004F1B28
		private void RefreshLabelRule()
		{
			LanguageRuleTips rule;
			bool flag = this.label.TryGetComponent<LanguageRuleTips>(out rule);
			if (flag)
			{
				rule.Refresh();
			}
		}

		// Token: 0x0600ADD3 RID: 44499 RVA: 0x004F394E File Offset: 0x004F1B4E
		public void ClearSortState()
		{
		}

		// Token: 0x0600ADD4 RID: 44500 RVA: 0x004F3951 File Offset: 0x004F1B51
		public void SetSortState(ESortDirection direction)
		{
		}

		// Token: 0x0600ADD5 RID: 44501 RVA: 0x004F3954 File Offset: 0x004F1B54
		private void UpdateArrowDisplay(ESortDirection direction)
		{
			switch (direction)
			{
			case ESortDirection.None:
				this.numberObject.SetActive(false);
				this.arrowImage.gameObject.SetActive(false);
				break;
			case ESortDirection.Ascending:
				this.numberObject.SetActive(true);
				this.arrowImage.gameObject.SetActive(true);
				this.arrowImage.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
				break;
			case ESortDirection.Descending:
				this.numberObject.SetActive(true);
				this.arrowImage.gameObject.SetActive(true);
				this.arrowImage.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				break;
			}
		}

		// Token: 0x0600ADD6 RID: 44502 RVA: 0x004F3A2D File Offset: 0x004F1C2D
		public void RefreshLabel()
		{
			TMP_Text tmp_Text = this.label;
			Func<string> labelGetter = this._labelGetter;
			tmp_Text.text = (((labelGetter != null) ? labelGetter() : null) ?? string.Empty);
			this.RefreshLabelRule();
		}

		// Token: 0x170013AA RID: 5034
		// (get) Token: 0x0600ADD7 RID: 44503 RVA: 0x004F3A5E File Offset: 0x004F1C5E
		public short SortId
		{
			get
			{
				return this._sortId;
			}
		}

		// Token: 0x0600ADD8 RID: 44504 RVA: 0x004F3A68 File Offset: 0x004F1C68
		private bool GetDefaultInteractable()
		{
			return this._sortId >= 0;
		}

		// Token: 0x0600ADD9 RID: 44505 RVA: 0x004F3A86 File Offset: 0x004F1C86
		public void SetInteractable(bool interactable)
		{
			this.button.interactable = false;
		}

		// Token: 0x0400863D RID: 34365
		[SerializeField]
		private GameObject numberObject;

		// Token: 0x0400863E RID: 34366
		[SerializeField]
		private TextMeshProUGUI numberLabel;

		// Token: 0x0400863F RID: 34367
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008640 RID: 34368
		[SerializeField]
		private CImage arrowImage;

		// Token: 0x04008641 RID: 34369
		[SerializeField]
		private CButton button;

		// Token: 0x04008642 RID: 34370
		[SerializeField]
		private GameObject rightLine;

		// Token: 0x04008643 RID: 34371
		[SerializeField]
		private LayoutElement layoutElement;

		// Token: 0x04008644 RID: 34372
		[SerializeField]
		private TooltipInvoker tips;

		// Token: 0x04008645 RID: 34373
		[Header("排序箭头图标")]
		[SerializeField]
		private Sprite arrowSprite;

		// Token: 0x04008646 RID: 34374
		private short _sortId = -1;

		// Token: 0x04008647 RID: 34375
		private int _columnIndex;

		// Token: 0x04008648 RID: 34376
		private Action<int, short> _onSortClicked;

		// Token: 0x04008649 RID: 34377
		private ESortDirection _sortDirection = ESortDirection.None;

		// Token: 0x0400864A RID: 34378
		private Func<string> _labelGetter;

		// Token: 0x0400864B RID: 34379
		private LocalStringManager.LanguageType? _languageType = null;
	}
}
