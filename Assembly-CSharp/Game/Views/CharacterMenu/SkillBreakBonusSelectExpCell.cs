using System;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B83 RID: 2947
	public class SkillBreakBonusSelectExpCell : MonoBehaviour, ICellContent<int>, ICellContent
	{
		// Token: 0x0600919C RID: 37276 RVA: 0x0043D8AC File Offset: 0x0043BAAC
		public void SetData(int expLevel)
		{
			this.itemBack.SetIcon(CommonUtils.GetResOrExpIcon(-1, true));
			this.itemBack.SetBack((sbyte)expLevel);
			int levelExpValue = SkillBreakPlateConstants.ExpLevelValues[expLevel];
			this.numberLabel.text = levelExpValue.ToString();
		}

		// Token: 0x04007025 RID: 28709
		[SerializeField]
		private TextMeshProUGUI numberLabel;

		// Token: 0x04007026 RID: 28710
		[SerializeField]
		private ItemBack itemBack;
	}
}
