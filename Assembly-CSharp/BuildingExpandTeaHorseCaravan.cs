using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class BuildingExpandTeaHorseCaravan : MonoBehaviour
{
	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001816 RID: 6166 RVA: 0x00093D3F File Offset: 0x00091F3F
	private BuildingModel BuildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x00093D48 File Offset: 0x00091F48
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
			Transform child2 = this.levelItemRoot.GetChild(j);
			Refers refers = child2.GetComponent<Refers>();
			this._levelItems.Add(refers);
		}
		this._maxLevel = this._levelItems.Count;
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x00093DEC File Offset: 0x00091FEC
	public void Refresh(BuildingBlockKey blockKey)
	{
		this._key = blockKey;
		this.RefreshInner();
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x00093E00 File Offset: 0x00092000
	private void RefreshInner()
	{
		sbyte level = this.BuildingModel.GetTaiwuBuildingData(this._key).CalcUnlockedLevelCount();
		for (int i = 0; i < this._maxLevel; i++)
		{
			this.RefreshLevelItem(this._levelItems[i], i + 1, (int)level);
			bool flag = i < this._maxLevel - 1;
			if (flag)
			{
				this.RefreshRouteItem(this._routeImages[i], i + 2, (int)level);
			}
		}
		this.CentralizeCurrentLevelItem((int)level);
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x00093E84 File Offset: 0x00092084
	private void RefreshRouteItem(CImage route, int myEndLevel, int level)
	{
		int x = (level >= myEndLevel) ? 1 : 0;
		int y = myEndLevel - 2;
		route.SetSprite(string.Format("ui_buildingpopup_teahorse_route_{0}_{1}", x, y), false, null);
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x00093EC0 File Offset: 0x000920C0
	private void RefreshLevelItem(Refers refers, int myLevel, int level)
	{
		CImage point = refers.CGet<CImage>("Point");
		CImage bg = refers.CGet<CImage>("Bg");
		TextMeshProUGUI awarenessLabel = refers.CGet<TextMeshProUGUI>("AwarenessLabel");
		TextMeshProUGUI awarenessTitleDisable = refers.CGet<TextMeshProUGUI>("AwarenessTitle_Disable");
		TextMeshProUGUI awarenessTitle = refers.CGet<TextMeshProUGUI>("AwarenessTitle");
		GameObject currentMark = refers.CGet<GameObject>("CurrentMark");
		CImage icon = refers.CGet<CImage>("Icon");
		bool unlocked = level >= myLevel;
		string pathId = unlocked ? "1" : "0";
		bg.SetSprite("ui_buildingpopup_teahorse_place_" + pathId, false, null);
		point.SetSprite("ui_buildingpopup_teahorse_point_" + pathId, false, null);
		string negativePathId = unlocked ? "0" : "1";
		icon.SetSprite(string.Format("ui_buildingpopup_teahorse_placeicon_{0}_{1}", BuildingExpandTeaHorseCaravan.ItemIcons[myLevel - 1], negativePathId), false, null);
		currentMark.SetActive(level == myLevel);
		refers.CGet<GameObject>("Effect").SetActive(level == myLevel);
		point.gameObject.SetActive(level != myLevel);
		awarenessLabel.text = GlobalConfig.Instance.TeaHorseCaravanLevelToAwareness[myLevel - 1].ToString();
		awarenessTitle.gameObject.SetActive(unlocked);
		awarenessTitleDisable.gameObject.SetActive(!unlocked);
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x00094014 File Offset: 0x00092214
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

	// Token: 0x0400135C RID: 4956
	private readonly List<CImage> _routeImages = new List<CImage>();

	// Token: 0x0400135D RID: 4957
	private readonly List<Refers> _levelItems = new List<Refers>();

	// Token: 0x0400135E RID: 4958
	private int _maxLevel;

	// Token: 0x0400135F RID: 4959
	private BuildingBlockKey _key;

	// Token: 0x04001360 RID: 4960
	private static readonly sbyte[] ItemIcons = new sbyte[]
	{
		1,
		0,
		0,
		1,
		2,
		0,
		0,
		0,
		1,
		2,
		2,
		2,
		1,
		0,
		1,
		0,
		1,
		2,
		0,
		1
	};

	// Token: 0x04001361 RID: 4961
	[SerializeField]
	private RectTransform routRoot;

	// Token: 0x04001362 RID: 4962
	[SerializeField]
	private RectTransform levelItemRoot;

	// Token: 0x04001363 RID: 4963
	[SerializeField]
	private RectTransform mask;

	// Token: 0x04001364 RID: 4964
	[SerializeField]
	private RectTransform map;
}
