using System;
using Config;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Building
{
	// Token: 0x02000F62 RID: 3938
	public class BuildingAreaResourceChange : MonoBehaviour
	{
		// Token: 0x0600B44C RID: 46156 RVA: 0x005205AC File Offset: 0x0051E7AC
		public bool RefreshResourceChangeOnPlan(int[] costArray)
		{
			this.idleVillagerCount.gameObject.SetActive(false);
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			bool isAllMeet = true;
			int index = 0;
			for (sbyte i = 0; i < 8; i += 1)
			{
				int costResource = (costArray != null) ? costArray.GetOrDefault((int)i) : 0;
				bool show = costResource > 0;
				RectTransform item = this.resourceHolder.GetChild((int)i).GetComponent<RectTransform>();
				item.gameObject.SetActive(show);
				bool flag = !show;
				if (!flag)
				{
					ResourceTypeItem resourceConfig = ResourceType.Instance[i];
					item.GetChild(0).GetComponent<CImage>().SetSprite(resourceConfig.Icon, false, null);
					item.GetChild(1).GetComponent<TextMeshProUGUI>().text = resourceConfig.Name;
					int ownResource = buildingModel.GetResourceCount(i);
					bool isMeet = ownResource >= costResource;
					bool flag2 = !isMeet;
					if (flag2)
					{
						isAllMeet = false;
					}
					string color = isMeet ? "brightblue" : "brightred";
					string ownStr = CommonUtils.GetDisplayStringForNum(ownResource, 100000).SetColor(color);
					string costStr = CommonUtils.GetDisplayStringForNum(costResource, 100000);
					item.GetChild(2).GetComponent<TextMeshProUGUI>().text = ownStr + "/" + costStr;
					Transform child = item.GetChild(3);
					if (child != null)
					{
						child.gameObject.SetActive(index < 4);
					}
					index++;
				}
			}
			return isAllMeet;
		}

		// Token: 0x0600B44D RID: 46157 RVA: 0x0052071C File Offset: 0x0051E91C
		public void RefreshResourceChangeOnRemove(int count, int[] gainArray)
		{
			this.idleVillagerCount.gameObject.SetActive(true);
			this.idleVillagerCountTitle.text = LanguageKey.LK_Building_IdleVillagerCount.Tr();
			this.idleVillagerCountValue.text = count.ToString();
			this.imageTips.SetActive(count == 0);
			int index = 0;
			for (int i = 0; i < 8; i++)
			{
				int gainResource = gainArray.GetOrDefault(i);
				bool show = gainResource > 0;
				RectTransform item = this.resourceHolder.GetChild(i).GetComponent<RectTransform>();
				item.gameObject.SetActive(show);
				bool flag = !show;
				if (!flag)
				{
					ResourceTypeItem resourceConfig = ResourceType.Instance[i];
					item.GetChild(0).GetComponent<CImage>().SetSprite(resourceConfig.Icon, false, null);
					item.GetChild(1).GetComponent<TextMeshProUGUI>().text = resourceConfig.Name;
					item.GetChild(2).GetComponent<TextMeshProUGUI>().text = "+" + CommonUtils.GetDisplayStringForNum(gainResource, 100000);
					Transform child = item.GetChild(3);
					if (child != null)
					{
						child.gameObject.SetActive(index < 4);
					}
					index++;
				}
			}
		}

		// Token: 0x04008C4D RID: 35917
		[SerializeField]
		private GameObject idleVillagerCount;

		// Token: 0x04008C4E RID: 35918
		[SerializeField]
		private TextMeshProUGUI idleVillagerCountTitle;

		// Token: 0x04008C4F RID: 35919
		[SerializeField]
		private TextMeshProUGUI idleVillagerCountValue;

		// Token: 0x04008C50 RID: 35920
		[SerializeField]
		private RectTransform resourceHolder;

		// Token: 0x04008C51 RID: 35921
		[SerializeField]
		private GameObject imageTips;
	}
}
