using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Views.Building.BuildingAreaQuickActionMenu
{
	// Token: 0x02000C26 RID: 3110
	public class LeftLayoutManager
	{
		// Token: 0x06009DC9 RID: 40393 RVA: 0x0049DFE9 File Offset: 0x0049C1E9
		public LeftLayoutManager(ViewBuildingQuickActionMenu menu)
		{
			this._menu = menu;
		}

		// Token: 0x06009DCA RID: 40394 RVA: 0x0049DFFA File Offset: 0x0049C1FA
		public void RequestLayoutUpdate()
		{
			this.UpdateLayout();
		}

		// Token: 0x06009DCB RID: 40395 RVA: 0x0049E004 File Offset: 0x0049C204
		public void UpdateLayout()
		{
			List<LeftButtonProcessor> visibleButtons = (from b in this._menu.LeftButtons
			where b.IsVisible()
			select b).ToList<LeftButtonProcessor>();
			bool flag = visibleButtons.Count == 0;
			if (!flag)
			{
				BuildingSizeConfig config = this._menu.GetCurrentConfig();
				float centerAngle = config.leftGroupCenterAngle;
				float spacingAngle = config.leftGroupSpacingAngle;
				List<float> angles = BuildingActionUtils.CalculateGroupAngles(centerAngle, visibleButtons.Count, spacingAngle);
				for (int i = 0; i < visibleButtons.Count; i++)
				{
					GameObject buttonObj = visibleButtons[i].ButtonObject;
					bool flag2 = buttonObj != null;
					if (flag2)
					{
						RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
						rectTransform.anchoredPosition = BuildingActionUtils.PolarToCartesian(angles[i], this._menu.GetBaseRadius());
					}
				}
			}
		}

		// Token: 0x04007A46 RID: 31302
		private readonly ViewBuildingQuickActionMenu _menu;
	}
}
