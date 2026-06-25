using System;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB6 RID: 3766
	public class BookPageItem : MonoBehaviour
	{
		// Token: 0x0600AED7 RID: 44759 RVA: 0x004FA9B4 File Offset: 0x004F8BB4
		public void SetData(sbyte state, sbyte progress, bool disabled)
		{
			this._currentState = state;
			this._currentProgress = progress;
			this._pageType = -1;
			this._isCombatBook = false;
			this._isDisabled = disabled;
			this.UpdateDisplay();
		}

		// Token: 0x0600AED8 RID: 44760 RVA: 0x004FA9E1 File Offset: 0x004F8BE1
		public void SetData(sbyte state, sbyte progress, sbyte pageType, bool isCombatBook, int pageIndex, bool disabled)
		{
			this._currentState = state;
			this._currentProgress = progress;
			this._pageType = pageType;
			this._isCombatBook = isCombatBook;
			this._isDisabled = disabled;
			this.UpdateDisplay(pageIndex);
		}

		// Token: 0x0600AED9 RID: 44761 RVA: 0x004FAA14 File Offset: 0x004F8C14
		public void SetDisabled(bool disabled)
		{
			bool flag = this._isDisabled == disabled;
			if (!flag)
			{
				this._isDisabled = disabled;
				this.UpdateDisplay();
			}
		}

		// Token: 0x0600AEDA RID: 44762 RVA: 0x004FAA3F File Offset: 0x004F8C3F
		private void UpdateDisplay()
		{
			this.UpdateDisplay(0);
		}

		// Token: 0x0600AEDB RID: 44763 RVA: 0x004FAA4C File Offset: 0x004F8C4C
		private void UpdateDisplay(int pageIndex)
		{
			string iconPattern = this._isDisabled ? "ui9_icon_item_book_reading_status_{0}_disabled" : "ui9_icon_item_book_reading_status_{0}";
			string iconName = string.Format(iconPattern, this._currentState);
			this.pageIcon.SetSprite(iconName, false, null);
			bool isGeneral = (this._isCombatBook && pageIndex == 0) || !this._isCombatBook;
			bool directRead = this._pageType == -1 || this._pageType == 0;
			bool flag = isGeneral;
			if (flag)
			{
				this.pageProgress.sprite = this.spriteProgressGeneral;
			}
			else
			{
				bool flag2 = directRead;
				if (flag2)
				{
					this.pageProgress.sprite = this.spriteProgressDirect;
				}
				else
				{
					this.pageProgress.sprite = this.spriteProgressReverse;
				}
			}
			this.pageProgress.fillAmount = (float)this._currentProgress / 100f;
		}

		// Token: 0x0400872F RID: 34607
		[SerializeField]
		private CImage pageIcon;

		// Token: 0x04008730 RID: 34608
		[SerializeField]
		private CImage pageProgress;

		// Token: 0x04008731 RID: 34609
		[SerializeField]
		private Sprite spriteProgressGeneral;

		// Token: 0x04008732 RID: 34610
		[SerializeField]
		private Sprite spriteProgressDirect;

		// Token: 0x04008733 RID: 34611
		[SerializeField]
		private Sprite spriteProgressReverse;

		// Token: 0x04008734 RID: 34612
		private sbyte _currentState;

		// Token: 0x04008735 RID: 34613
		private sbyte _currentProgress;

		// Token: 0x04008736 RID: 34614
		private sbyte _pageType;

		// Token: 0x04008737 RID: 34615
		private bool _isCombatBook;

		// Token: 0x04008738 RID: 34616
		private bool _isDisabled;
	}
}
