using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200085C RID: 2140
	public class MouseTipFuyuCombatUse : MonoBehaviour
	{
		// Token: 0x060067AC RID: 26540 RVA: 0x002F5E48 File Offset: 0x002F4048
		public void Set(int consumedFeatureMedals, bool isFuyuSword)
		{
			bool showCombatUse = consumedFeatureMedals > 0 || isFuyuSword;
			base.gameObject.SetActive(showCombatUse);
			bool flag = !showCombatUse;
			if (!flag)
			{
				bool flag2 = this.costWisdomLayout != null;
				if (flag2)
				{
					this.costWisdomLayout.SetActive(isFuyuSword);
					this.costWisdomText.SetText("0", true);
				}
			}
		}

		// Token: 0x04004943 RID: 18755
		[SerializeField]
		private GameObject costWisdomLayout;

		// Token: 0x04004944 RID: 18756
		[SerializeField]
		private TextMeshProUGUI costWisdomText;
	}
}
