using System;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000808 RID: 2056
	public class NewGameSubPageCustomPresetToggleHelper : MonoBehaviour
	{
		// Token: 0x060064DF RID: 25823 RVA: 0x002E1D68 File Offset: 0x002DFF68
		public void RefreshPoints(int remainingPoints, int totalPoints)
		{
			bool flag = this.pointsText != null;
			if (flag)
			{
				this.pointsText.text = string.Format("{0}", remainingPoints);
			}
		}

		// Token: 0x04004640 RID: 17984
		[SerializeField]
		private TextMeshProUGUI pointsText;
	}
}
