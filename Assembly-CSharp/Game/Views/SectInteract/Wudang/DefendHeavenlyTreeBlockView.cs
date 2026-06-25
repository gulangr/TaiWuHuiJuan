using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009D8 RID: 2520
	public class DefendHeavenlyTreeBlockView : MonoBehaviour
	{
		// Token: 0x17000D9B RID: 3483
		// (get) Token: 0x06007B53 RID: 31571 RVA: 0x0039477F File Offset: 0x0039297F
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x06007B54 RID: 31572 RVA: 0x00394788 File Offset: 0x00392988
		public void SetData(MapBlockData blockData, bool isTree, bool isCreate, int visibleIndex, Vector2 pos, int enemyCount, int villagerCount, RectTransform mapLayer, RectTransform pathLayer, RectTransform infoLayer, RectTransform characterLayer)
		{
			this._blockData = blockData;
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			base.name = blockData.GetLocation().BlockId.ToString();
			bool visible = visibleIndex >= 0;
			base.gameObject.SetActive(visible);
			bool flag = visible;
			if (flag)
			{
				this.toggle.isOn = false;
			}
			MapBlockData rootBlockData = (blockData.RootBlockId > -1) ? worldMapModel.GetBlockData(new Location(blockData.AreaId, blockData.RootBlockId)) : null;
			this.mapBlockView.Refresh(blockData, rootBlockData);
			base.transform.localPosition = pos;
			if (isCreate)
			{
				this.elementInfo.transform.SetParent(infoLayer);
			}
			bool hasEnemy = enemyCount > 0;
			bool hasVillager = villagerCount > 0;
			this.textEnemyCount.text = enemyCount.ToString();
			this.textVillagerCount.text = villagerCount.ToString();
			this.rootEnemyCount.SetActive(hasEnemy);
			this.rootVillagerCount.SetActive(hasVillager);
			this.elementInfo.gameObject.SetActive(hasEnemy || hasVillager);
			if (isCreate)
			{
				this.elementCharacter.transform.SetParent(characterLayer);
			}
			this.elementCharacter.gameObject.SetActive(isTree);
			this.pathLineRenderer.gameObject.SetActive(false);
			this.pathLineRenderer.Vertices = null;
		}

		// Token: 0x06007B55 RID: 31573 RVA: 0x003948F8 File Offset: 0x00392AF8
		public void RefreshLine(bool showLine, Location treeLocation, List<MapBlockData> blockList, RectTransform pathLayer, RectTransform mapLayer)
		{
			LineRenderer2D line = this.pathLineRenderer;
			line.gameObject.transform.SetParent(pathLayer);
			line.Vertices = null;
			line.GetComponent<CImage>().SetVerticesDirty();
			line.gameObject.SetActive(showLine);
			if (showLine)
			{
				Location curLocation = this._blockData.GetLocation();
				MapDomainMethod.AsyncCall.GetPathInAreaWithoutCost(null, curLocation, treeLocation, delegate(int offset, RawDataPool pool)
				{
					List<Location> locationList = new List<Location>();
					Serializer.Deserialize(pool, offset, ref locationList);
					Vector2[] pointList = new Vector2[locationList.Count];
					Vector3 curPos = this.transform.localPosition;
					for (int index = 0; index < locationList.Count; index++)
					{
						Location location = locationList[index];
						int blockIndex = blockList.FindIndex((MapBlockData b) => b.GetLocation() == location);
						Transform blockView = (blockIndex < 0) ? null : mapLayer.GetChild(blockIndex);
						Vector3 localPos = (blockView != null) ? blockView.transform.localPosition : default(Vector3);
						Vector3 pointPos = localPos - curPos;
						pointList[index] = pointPos;
					}
					line.Vertices = pointList;
					line.GetComponent<CImage>().SetVerticesDirty();
				});
			}
		}

		// Token: 0x06007B56 RID: 31574 RVA: 0x003949A0 File Offset: 0x00392BA0
		public void Clear()
		{
			base.gameObject.SetActive(false);
			this.elementCharacter.gameObject.SetActive(false);
			this.elementInfo.gameObject.SetActive(false);
			this.pathLineRenderer.gameObject.SetActive(false);
			this.pathLineRenderer.Vertices = null;
		}

		// Token: 0x04005D99 RID: 23961
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04005D9A RID: 23962
		[SerializeField]
		private MapBlockView mapBlockView;

		// Token: 0x04005D9B RID: 23963
		[SerializeField]
		private GameObject elementInfo;

		// Token: 0x04005D9C RID: 23964
		[SerializeField]
		private GameObject elementCharacter;

		// Token: 0x04005D9D RID: 23965
		[SerializeField]
		private LineRenderer2D pathLineRenderer;

		// Token: 0x04005D9E RID: 23966
		[SerializeField]
		private TextMeshProUGUI textEnemyCount;

		// Token: 0x04005D9F RID: 23967
		[SerializeField]
		private GameObject rootEnemyCount;

		// Token: 0x04005DA0 RID: 23968
		[SerializeField]
		private TextMeshProUGUI textVillagerCount;

		// Token: 0x04005DA1 RID: 23969
		[SerializeField]
		private GameObject rootVillagerCount;

		// Token: 0x04005DA2 RID: 23970
		private MapBlockData _blockData;
	}
}
