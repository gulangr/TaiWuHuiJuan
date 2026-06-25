using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD1 RID: 3025
	public class BuildingUpgradeCostComponent : MonoBehaviour
	{
		// Token: 0x0600984B RID: 38987 RVA: 0x0046F6FC File Offset: 0x0046D8FC
		public void Set(int current, int cost)
		{
			string currentStr = CommonUtils.GetDisplayStringForNum(current, 100000);
			currentStr = currentStr.SetColor((current < cost) ? "brightred" : "brightblue");
			this.costText.text = currentStr + "/" + cost.ToString();
		}

		// Token: 0x0600984C RID: 38988 RVA: 0x0046F74B File Offset: 0x0046D94B
		public void SetEmpty()
		{
			this.costText.text = "/";
		}

		// Token: 0x04007523 RID: 29987
		[SerializeField]
		private TextMeshProUGUI costText;
	}
}
