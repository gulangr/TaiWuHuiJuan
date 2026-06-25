using System;
using FrameWork;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementTreasury
{
	// Token: 0x02000786 RID: 1926
	public class ViewSettlementTreasuryReplenish : UIBase
	{
		// Token: 0x06005D49 RID: 23881 RVA: 0x002AE710 File Offset: 0x002AC910
		public override void OnInit(ArgumentBox argsBox)
		{
			int influenceRefreshTime;
			argsBox.Get("InfluenceRefreshTime", out influenceRefreshTime);
			Inventory inventory;
			argsBox.Get<Inventory>("SupplyItems", out inventory);
			sbyte[] supplyCounts;
			argsBox.Get<sbyte[]>("SupplyCounts", out supplyCounts);
			this.textTip.text = LanguageKey.LK_Building_Treasury_Replenish_Tip.TrFormat(influenceRefreshTime);
			for (int index = 0; index < this.replenishLayerArray.Length; index++)
			{
				ReplenishLayer replenishLayer = this.replenishLayerArray[index];
				replenishLayer.Set(index, inventory, supplyCounts);
			}
		}

		// Token: 0x06005D4A RID: 23882 RVA: 0x002AE794 File Offset: 0x002AC994
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "ButtonCloseView")
			{
				this.QuickHide();
			}
		}

		// Token: 0x0400400D RID: 16397
		[SerializeField]
		private TextMeshProUGUI textTip;

		// Token: 0x0400400E RID: 16398
		[SerializeField]
		private ReplenishLayer[] replenishLayerArray = new ReplenishLayer[3];
	}
}
