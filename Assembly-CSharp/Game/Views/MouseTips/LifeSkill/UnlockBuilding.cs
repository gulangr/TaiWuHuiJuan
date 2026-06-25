using System;
using System.Linq;
using Config;
using Game.Views.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.LifeSkill
{
	// Token: 0x02000894 RID: 2196
	public class UnlockBuilding : MonoBehaviour
	{
		// Token: 0x0600693C RID: 26940 RVA: 0x00305780 File Offset: 0x00303980
		public void Set(BuildingBlockItem config, short lifeSkillType, int page, bool unlocked)
		{
			string color = unlocked ? "brightblue" : "brightred";
			SkillBookItem bookConfig = SkillBook.Instance.First((SkillBookItem book) => book.LifeSkillTemplateId == lifeSkillType);
			ViewBuildingArea.SetBuildingIcon(this.icon, config, false, null);
			this.nameLabel.text = config.Name;
			this.disableStyleRoot.SetStyleEffect(!unlocked, false);
			this.lockObj.SetActive(!unlocked);
			this.bookNameLabel.text = bookConfig.Name.SetColor(color);
			this.bookPageLabel.text = LocalStringManager.Get("LK_Book_Page_Index_" + page.ToString()).SetColor(color);
		}

		// Token: 0x04004B6E RID: 19310
		[SerializeField]
		private DisableStyleRoot disableStyleRoot;

		// Token: 0x04004B6F RID: 19311
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B70 RID: 19312
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04004B71 RID: 19313
		[SerializeField]
		private TextMeshProUGUI bookNameLabel;

		// Token: 0x04004B72 RID: 19314
		[SerializeField]
		private TextMeshProUGUI bookPageLabel;

		// Token: 0x04004B73 RID: 19315
		[SerializeField]
		private GameObject lockObj;
	}
}
