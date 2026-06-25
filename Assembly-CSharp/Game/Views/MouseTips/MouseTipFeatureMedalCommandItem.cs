using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000855 RID: 2133
	public class MouseTipFeatureMedalCommandItem : MonoBehaviour
	{
		// Token: 0x06006791 RID: 26513 RVA: 0x002F51B4 File Offset: 0x002F33B4
		public void Set(sbyte medalType, TeammateCommandItem command)
		{
			this.nameLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_Command, command.Name);
			this.icon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)medalType, 0), false, null);
			this.medalCountLabel.text = string.Format("x{0}", command.MedalCount);
		}

		// Token: 0x0400492A RID: 18730
		[SerializeField]
		public TextMeshProUGUI nameLabel;

		// Token: 0x0400492B RID: 18731
		[SerializeField]
		public CImage icon;

		// Token: 0x0400492C RID: 18732
		[SerializeField]
		public TextMeshProUGUI medalCountLabel;
	}
}
