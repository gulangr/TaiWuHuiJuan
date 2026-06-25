using System;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200088A RID: 2186
	public class TooltipCombatSkillHitPartGroup : MonoBehaviour
	{
		// Token: 0x060068F5 RID: 26869 RVA: 0x0030360F File Offset: 0x0030180F
		public void SetTitle(string title)
		{
			this.titleItem.SetTitle(title);
		}

		// Token: 0x060068F6 RID: 26870 RVA: 0x00303620 File Offset: 0x00301820
		public void SetItems(int[] hitPartWeights, string[,] hitPartNames, string iconPrefix, string activeColorName, string inactiveColorName)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				int configIndex = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)i);
				bool isActive = configIndex >= 0 && configIndex < hitPartWeights.Length && hitPartWeights[configIndex] > 0;
				string content = LocalStringManager.Get(hitPartNames[i, 1]).SetColor(isActive ? activeColorName : inactiveColorName).ColorReplace();
				this.items[i].Set(string.Format("{0}{1}", iconPrefix, i), content, Colors.Instance[isActive ? activeColorName : inactiveColorName], true);
			}
		}

		// Token: 0x04004B17 RID: 19223
		[SerializeField]
		private TooltipCombatSkillTextItem titleItem;

		// Token: 0x04004B18 RID: 19224
		[SerializeField]
		private TooltipCombatSkillHitPartItem[] items;
	}
}
