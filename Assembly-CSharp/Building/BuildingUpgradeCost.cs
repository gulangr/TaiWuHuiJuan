using System;
using TMPro;
using UnityEngine;

namespace Building
{
	// Token: 0x02000662 RID: 1634
	public class BuildingUpgradeCost : MonoBehaviour
	{
		// Token: 0x06004DB4 RID: 19892 RVA: 0x00249F9C File Offset: 0x0024819C
		public void Set(int current, int cost)
		{
			string currentStr = CommonUtils.GetDisplayStringForNum(current, 100000);
			currentStr = currentStr.SetColor((current < cost) ? "brightred" : "brightblue");
			this.costText.text = currentStr + "/" + cost.ToString();
		}

		// Token: 0x040035E6 RID: 13798
		[SerializeField]
		private TextMeshProUGUI costText;
	}
}
