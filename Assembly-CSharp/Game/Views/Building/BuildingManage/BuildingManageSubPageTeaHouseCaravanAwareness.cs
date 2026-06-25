using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C03 RID: 3075
	public class BuildingManageSubPageTeaHouseCaravanAwareness : BuildingManageSubPage
	{
		// Token: 0x06009C68 RID: 40040 RVA: 0x004940E8 File Offset: 0x004922E8
		private void Awake()
		{
			for (int i = 0; i < this.routRoot.childCount; i++)
			{
				Transform child = this.routRoot.GetChild(i);
				CImage image = child.GetComponent<CImage>();
				this._routeImages.Add(image);
			}
			for (int j = 0; j < this.levelItemRoot.childCount; j++)
			{
				this._levelItems.Add(this.levelItemRoot.GetChild(j).GetComponent<TeaHouseCaravanAwarenessLevelItems>());
			}
			this._maxLevel = this._levelItems.Count;
		}

		// Token: 0x06009C69 RID: 40041 RVA: 0x00494182 File Offset: 0x00492382
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this.RefreshInner();
		}

		// Token: 0x06009C6A RID: 40042 RVA: 0x00494194 File Offset: 0x00492394
		private void RefreshInner()
		{
			sbyte level = this.DisplayData.BlockData.CalcUnlockedLevelCount();
			for (int i = 0; i < this._maxLevel; i++)
			{
				this._levelItems[i].RefreshLevelItem(i + 1, (int)level);
				bool flag = i < this._maxLevel - 1;
				if (flag)
				{
					this.RefreshRouteItem(this._routeImages[i], i + 2, (int)level);
				}
			}
			this.CentralizeCurrentLevelItem((int)level);
		}

		// Token: 0x06009C6B RID: 40043 RVA: 0x00494210 File Offset: 0x00492410
		private void RefreshRouteItem(CImage route, int myEndLevel, int level)
		{
			int x = (level >= myEndLevel) ? 1 : 0;
			int y = myEndLevel - 2;
			route.SetSprite(string.Format("ui_buildingpopup_teahorse_route_{0}_{1}", x, y), false, null);
		}

		// Token: 0x06009C6C RID: 40044 RVA: 0x0049424C File Offset: 0x0049244C
		private void CentralizeCurrentLevelItem(int level)
		{
			bool flag = level < 1 || level > this._levelItems.Count;
			if (!flag)
			{
				RectTransform item = this._levelItems[level - 1].transform as RectTransform;
				Vector3 itemWorldPos = item.TransformPoint(item.rect.center);
				Vector3 mapLocalPos = this.map.InverseTransformPoint(itemWorldPos);
				Vector3 maskWorldCenter = this.mask.TransformPoint(this.mask.rect.center);
				Vector3 maskLocalCenter = this.map.InverseTransformPoint(maskWorldCenter);
				Vector3 offset = maskLocalCenter - mapLocalPos;
				this.map.anchoredPosition += new Vector2(offset.x, offset.y);
				this.map.GetComponent<UIRectDragMove>().SetDirty();
			}
		}

		// Token: 0x0400791A RID: 31002
		private readonly List<CImage> _routeImages = new List<CImage>();

		// Token: 0x0400791B RID: 31003
		private readonly List<TeaHouseCaravanAwarenessLevelItems> _levelItems = new List<TeaHouseCaravanAwarenessLevelItems>();

		// Token: 0x0400791C RID: 31004
		private int _maxLevel;

		// Token: 0x0400791D RID: 31005
		[SerializeField]
		private RectTransform routRoot;

		// Token: 0x0400791E RID: 31006
		[SerializeField]
		private RectTransform levelItemRoot;

		// Token: 0x0400791F RID: 31007
		[SerializeField]
		private RectTransform mask;

		// Token: 0x04007920 RID: 31008
		[SerializeField]
		private RectTransform map;
	}
}
