using System;
using System.Collections.Generic;
using Config;
using Game.Components.Item;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Utilities.Information;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EFC RID: 3836
	public class SecretInformationTargetContent : MonoBehaviour
	{
		// Token: 0x0600B0B5 RID: 45237 RVA: 0x00508E20 File Offset: 0x00507020
		public void Set(SecretInformationDisplayData data, IReadOnlyDictionary<int, CharacterDisplayData> characterDisplayData)
		{
			SecretInformationItem config = SecretInformation.Instance[data.SecretInformationTemplateId];
			bool flag = data.ParametersPack == null;
			if (flag)
			{
				base.transform.GetChild(0).gameObject.SetActive(false);
			}
			else
			{
				CharacterDisplayData mainData = null;
				CharacterDisplayData secondData1 = null;
				CharacterDisplayData secondData2 = null;
				Location location = Location.Invalid;
				List<ItemDisplayData> itemDataList = null;
				sbyte resourceType = -1;
				data.ParametersPack.ExtractSecretParameters(config, delegate(int index, int charId)
				{
					switch (index)
					{
					case 0:
						mainData = characterDisplayData[charId];
						break;
					case 1:
						secondData1 = characterDisplayData[charId];
						break;
					case 2:
						secondData2 = characterDisplayData[charId];
						break;
					}
				}, delegate(int _, Location theLocation)
				{
					location = theLocation;
				}, delegate(int _, sbyte r)
				{
					resourceType = r;
				}, delegate(int _, ItemKey itemKey)
				{
					bool flag2 = itemKey.IsValid();
					if (flag2)
					{
						if (itemDataList == null)
						{
							itemDataList = new List<ItemDisplayData>();
						}
						itemDataList.Add(new ItemDisplayData(itemKey, 1));
					}
				}, null, null, delegate(int _, int value)
				{
					bool flag2 = resourceType != -1;
					if (flag2)
					{
						ItemDisplayData displayData = new ItemDisplayData(12, (short)resourceType)
						{
							Amount = value
						};
						itemDataList.Add(displayData);
					}
				});
				this.Set(mainData, secondData1, secondData2, itemDataList, location);
				base.transform.GetChild(0).gameObject.SetActive(true);
			}
		}

		// Token: 0x0600B0B6 RID: 45238 RVA: 0x00508F30 File Offset: 0x00507130
		public void Set(CharacterDisplayData mainData, CharacterDisplayData secondData1, CharacterDisplayData secondData2, List<ItemDisplayData> itemDataList, Location location)
		{
			this.mainCharacter.Set(mainData, 0);
			bool flag = secondData1 != null;
			if (flag)
			{
				this.character1.Set(secondData1, 0);
				this.character1.gameObject.SetActive(true);
			}
			else
			{
				this.character1.gameObject.SetActive(false);
			}
			bool flag2 = secondData2 != null;
			if (flag2)
			{
				this.character2.Set(secondData2, 0);
				this.character2.gameObject.SetActive(true);
			}
			else
			{
				this.character2.gameObject.SetActive(false);
			}
			bool flag3 = itemDataList == null;
			if (flag3)
			{
				this.items.gameObject.SetActive(false);
			}
			else
			{
				for (int i = 0; i < itemDataList.Count; i++)
				{
					Transform item = this.items.GetChild(i);
					ItemDisplayData displayData = itemDataList[i];
					ItemKey itemKey = displayData.RealKey;
					ItemBack itemBack = item.GetChild(0).GetComponent<ItemBack>();
					itemBack.Set(displayData, false);
					bool flag4 = displayData.IsResource && displayData.Amount > 0;
					string nameText;
					if (flag4)
					{
						itemBack.SetCount(0, true);
						ResourceTypeItem resourceTypeConfig = ResourceType.Instance[displayData.ResourceType];
						nameText = LocalStringManager.GetFormat(LanguageKey.LK_SecretInfo_ResourceAmountName, displayData.Amount, resourceTypeConfig.Name);
					}
					else
					{
						nameText = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId).SetGradeColor((int)ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId));
					}
					item.GetChild(1).GetComponent<TextMeshProUGUI>().text = nameText;
					item.gameObject.SetActive(true);
				}
				for (int j = itemDataList.Count; j < this.items.childCount; j++)
				{
					this.items.GetChild(j).gameObject.SetActive(false);
				}
			}
			this.locationLine.SetActive(itemDataList != null && location.IsValid());
			bool flag5 = location.IsValid();
			if (flag5)
			{
				MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
				bool flag6 = areas.CheckIndex((int)location.AreaId);
				if (flag6)
				{
					short areaTemplateId = areas[(int)location.AreaId].GetTemplateId();
					MapAreaItem areaConfig = MapArea.Instance.GetItem(areaTemplateId);
					bool flag7 = areaConfig != null;
					if (flag7)
					{
						this.locationIcon.SetSprite(areaConfig.BigMapIcon, false, null);
						this.locationName.text = areaConfig.Name;
						this.locationObj.SetActive(true);
					}
					else
					{
						this.locationObj.SetActive(false);
					}
				}
				else
				{
					this.locationObj.SetActive(false);
				}
			}
			else
			{
				this.locationObj.SetActive(false);
			}
		}

		// Token: 0x040088C4 RID: 35012
		[SerializeField]
		protected SecretInformationSourceItem mainCharacter;

		// Token: 0x040088C5 RID: 35013
		[SerializeField]
		protected SecretInformationSourceItem character1;

		// Token: 0x040088C6 RID: 35014
		[SerializeField]
		protected SecretInformationSourceItem character2;

		// Token: 0x040088C7 RID: 35015
		[SerializeField]
		protected Transform items;

		// Token: 0x040088C8 RID: 35016
		[SerializeField]
		protected GameObject locationObj;

		// Token: 0x040088C9 RID: 35017
		[SerializeField]
		protected GameObject locationLine;

		// Token: 0x040088CA RID: 35018
		[SerializeField]
		protected TextMeshProUGUI locationName;

		// Token: 0x040088CB RID: 35019
		[SerializeField]
		protected CImage locationIcon;
	}
}
