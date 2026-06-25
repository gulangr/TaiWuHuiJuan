using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Legacy.WorldMap
{
	// Token: 0x020009F9 RID: 2553
	public class MapElementInfoCountItem : MonoBehaviour
	{
		// Token: 0x17000DB5 RID: 3509
		// (get) Token: 0x06007DB8 RID: 32184 RVA: 0x003A64DC File Offset: 0x003A46DC
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x06007DB9 RID: 32185 RVA: 0x003A64EC File Offset: 0x003A46EC
		public void Refresh(short id, int count, bool showCount)
		{
			MapElementDisplayRuleItemItem config = MapElementDisplayRuleItem.Instance[id];
			this.imageIcon.SetSprite(config.BlockInfoIcon, false, null);
			this.imageBack.enabled = showCount;
			string content = showCount ? count.ToString().SetColor(config.Color) : string.Empty;
			this.textCount.SetText(content, true);
		}

		// Token: 0x04005FB8 RID: 24504
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005FB9 RID: 24505
		[SerializeField]
		private TextMeshProUGUI textCount;

		// Token: 0x04005FBA RID: 24506
		[SerializeField]
		private CImage imageBack;
	}
}
