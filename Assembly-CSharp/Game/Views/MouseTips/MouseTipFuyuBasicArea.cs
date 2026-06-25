using System;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000858 RID: 2136
	public class MouseTipFuyuBasicArea : MonoBehaviour
	{
		// Token: 0x060067A4 RID: 26532 RVA: 0x002F5C3C File Offset: 0x002F3E3C
		public void Set(ItemDisplayData itemData, bool templateDataOnly)
		{
			if (templateDataOnly)
			{
				this.commonArea.Refresh(itemData.RealKey, true);
			}
			else
			{
				this.commonArea.Refresh(itemData, false);
			}
		}

		// Token: 0x04004942 RID: 18754
		[SerializeField]
		private TooltipItemCommonArea commonArea;
	}
}
