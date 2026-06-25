using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Legacy.WorldMap
{
	// Token: 0x020009FA RID: 2554
	public class MapElementMerchantTypeItem : MonoBehaviour
	{
		// Token: 0x17000DB6 RID: 3510
		// (get) Token: 0x06007DBB RID: 32187 RVA: 0x003A655B File Offset: 0x003A475B
		public CanvasGroup CanvasGroup
		{
			get
			{
				return this.canvasGroup;
			}
		}

		// Token: 0x06007DBC RID: 32188 RVA: 0x003A6564 File Offset: 0x003A4764
		public void Init(int id)
		{
			MerchantTypeItem config = MerchantType.Instance[id];
			this.imageIcon.SetSprite(config.Icon, false, null);
			this.textName.SetText(config.Name.SetColor("caravan"), true);
			this.canvasGroup.alpha = 0f;
		}

		// Token: 0x04005FBB RID: 24507
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005FBC RID: 24508
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005FBD RID: 24509
		[SerializeField]
		private CanvasGroup canvasGroup;
	}
}
