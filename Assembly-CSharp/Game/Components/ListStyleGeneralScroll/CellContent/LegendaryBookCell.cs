using System;
using Config;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EC1 RID: 3777
	public class LegendaryBookCell : MonoBehaviour, ICellContent<LegendaryBookCellData>, ICellContent
	{
		// Token: 0x0600AEF6 RID: 44790 RVA: 0x004FB70C File Offset: 0x004F990C
		private void Awake()
		{
			bool flag = this.skillTypeName != null;
			if (flag)
			{
				this._skillTypeBaseFontSize = this.skillTypeName.fontSize;
			}
			bool flag2 = this.bookName != null;
			if (flag2)
			{
				this._bookNameBaseFontSize = this.bookName.fontSize;
			}
		}

		// Token: 0x0600AEF7 RID: 44791 RVA: 0x004FB75C File Offset: 0x004F995C
		public void SetData(LegendaryBookCellData data)
		{
			MiscItem miscConfig = Misc.Instance[240 + (int)data.BookType];
			this.skillIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_attainments_small_0_", data.BookType), false, null);
			this.skillTypeName.text = CombatSkillType.Instance[data.BookType].Name;
			TMPTextEnLayoutHelper.ApplyLegendaryBookSkillTypeName(this.skillTypeName, this._skillTypeBaseFontSize);
			this.skillTypeName.ForceMeshUpdate(false, false);
			this.bookIcon.SetSprite(miscConfig.Icon, false, null);
			this.bookName.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", data.BookType));
			TMPTextEnLayoutHelper.ApplyLegendaryBookBookName(this.bookName, this._bookNameBaseFontSize);
			this.bookName.ForceMeshUpdate(false, false);
		}

		// Token: 0x0400875B RID: 34651
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x0400875C RID: 34652
		[SerializeField]
		private TextMeshProUGUI skillTypeName;

		// Token: 0x0400875D RID: 34653
		[SerializeField]
		private CImage bookIcon;

		// Token: 0x0400875E RID: 34654
		[SerializeField]
		private TextMeshProUGUI bookName;

		// Token: 0x0400875F RID: 34655
		private float _skillTypeBaseFontSize;

		// Token: 0x04008760 RID: 34656
		private float _bookNameBaseFontSize;
	}
}
