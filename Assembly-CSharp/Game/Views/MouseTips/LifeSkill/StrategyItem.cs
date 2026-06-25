using System;
using System.Collections.Generic;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.LifeSkill
{
	// Token: 0x02000893 RID: 2195
	public class StrategyItem : MonoBehaviour
	{
		// Token: 0x0600693A RID: 26938 RVA: 0x003056A4 File Offset: 0x003038A4
		public void Set(sbyte lifeSkillType, int level, bool unlocked)
		{
			foreach (DebateStrategyItem config in ((IEnumerable<DebateStrategyItem>)DebateStrategy.Instance))
			{
				bool flag = config.LifeSkillType != lifeSkillType || (int)config.Level != level;
				if (!flag)
				{
					this.icon.SetSprite(string.Format("{0}{1}", "ui9_icon_craftsmanship_small_0_", lifeSkillType), false, null);
					this.nameLabel.text = config.Name;
					this.descLabel.SetText(config.Desc.ColorReplace(), true);
					this.disableStyleRoot.SetStyleEffect(!unlocked, false);
					break;
				}
			}
		}

		// Token: 0x04004B6A RID: 19306
		[SerializeField]
		private DisableStyleRoot disableStyleRoot;

		// Token: 0x04004B6B RID: 19307
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B6C RID: 19308
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04004B6D RID: 19309
		[SerializeField]
		private TextMeshProUGUI descLabel;
	}
}
