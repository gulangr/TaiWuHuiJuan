using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Views.Building.BuildingManage;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x0200094D RID: 2381
	public class MakeBuildingItem : MonoBehaviour
	{
		// Token: 0x06007085 RID: 28805 RVA: 0x00341BC0 File Offset: 0x0033FDC0
		public void Init(int index)
		{
			short buildingId = MakeBuildingItem.BuildingIdList[index];
			BuildingBlockItem config = BuildingBlock.Instance[buildingId];
			ViewBuildingManage.SetBuildingIcon(this.imageIcon, config, false, null);
			this.textName.SetText(config.Name, true);
		}

		// Token: 0x0400536F RID: 21359
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04005370 RID: 21360
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005371 RID: 21361
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005372 RID: 21362
		public static readonly List<short> BuildingIdList = new List<short>
		{
			203,
			149,
			159,
			179,
			139,
			169,
			129
		};
	}
}
