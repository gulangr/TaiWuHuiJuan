using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x0200095F RID: 2399
	public class ResourceCostItem : MonoBehaviour
	{
		// Token: 0x0600728F RID: 29327 RVA: 0x00353690 File Offset: 0x00351890
		public void Set(sbyte resourceType, int requiredCount, int ownedCount = 0)
		{
			this.resourceIcon.SetSprite("ui9_icon_resource_big_" + resourceType.ToString(), false, null);
			this.resourceName.text = ResourceType.Instance[resourceType].Name;
			string color = (ownedCount >= requiredCount) ? "brightblue" : "brightred";
			string ownedText = ownedCount.ToString().SetColor(color);
			this.resourceCount.text = string.Format("{0}/{1}", ownedText, requiredCount);
		}

		// Token: 0x04005500 RID: 21760
		[SerializeField]
		private CImage resourceIcon;

		// Token: 0x04005501 RID: 21761
		[SerializeField]
		private TextMeshProUGUI resourceName;

		// Token: 0x04005502 RID: 21762
		[SerializeField]
		private TextMeshProUGUI resourceCount;
	}
}
