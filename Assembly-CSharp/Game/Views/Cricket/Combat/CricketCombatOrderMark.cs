using System;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ACC RID: 2764
	public class CricketCombatOrderMark : MonoBehaviour
	{
		// Token: 0x06008833 RID: 34867 RVA: 0x003F35C5 File Offset: 0x003F17C5
		private void Awake()
		{
			this.SetOrder(-1);
		}

		// Token: 0x06008834 RID: 34868 RVA: 0x003F35D0 File Offset: 0x003F17D0
		public void SetOrder(int order)
		{
			bool flag = order < 0;
			if (flag)
			{
				this.tip.enabled = false;
				this.icon.gameObject.SetActive(false);
			}
			else
			{
				order = Mathf.Clamp(order, 0, 2);
				this.icon.gameObject.SetActive(true);
				this.icon.SetSprite(string.Format("{0}{1}", "ui9_icon_cricketcombat_1_", order), false, null);
				this.tip.Type = TipType.SingleDesc;
				this.tip.PresetParam = new string[]
				{
					LocalStringManager.Get(string.Format("LK_CricketBattle_Round{0}", order))
				};
				this.tip.enabled = true;
			}
		}

		// Token: 0x04006861 RID: 26721
		[SerializeField]
		private CImage icon;

		// Token: 0x04006862 RID: 26722
		[SerializeField]
		private TooltipInvoker tip;
	}
}
