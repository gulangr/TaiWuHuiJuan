using System;
using Config;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EDC RID: 3804
	public class ReferenceCombatSkillQiArtStrategyCell : MonoBehaviour, ICellContent<CombatSkillItem>, ICellContent
	{
		// Token: 0x0600AF30 RID: 44848 RVA: 0x004FD13C File Offset: 0x004FB33C
		public void SetData(CombatSkillItem data)
		{
			for (int i = 0; i < this.strategyIcons.Length; i++)
			{
				bool flag = data.PossibleQiArtStrategyList.CheckIndex(i);
				if (flag)
				{
					sbyte templateId = data.PossibleQiArtStrategyList[i];
					this.strategyIcons[i].enabled = true;
					this.strategyIcons[i].SetSprite(QiArtStrategy.Instance[templateId].Icon, false, null);
					TooltipInvoker strategyTip = this.strategyIcons[i].GetComponent<TooltipInvoker>();
					strategyTip.Type = TipType.Simple;
					strategyTip.PresetParam[0] = QiArtStrategy.Instance[templateId].Name;
					strategyTip.PresetParam[1] = QiArtStrategy.Instance[templateId].Desc;
				}
				else
				{
					this.strategyIcons[i].enabled = false;
				}
			}
		}

		// Token: 0x040087B9 RID: 34745
		[SerializeField]
		private CImage[] strategyIcons;
	}
}
