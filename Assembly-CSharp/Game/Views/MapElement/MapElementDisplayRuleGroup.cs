using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.MapElement
{
	// Token: 0x02000930 RID: 2352
	public class MapElementDisplayRuleGroup : MonoBehaviour
	{
		// Token: 0x17000CB3 RID: 3251
		// (get) Token: 0x06006DC6 RID: 28102 RVA: 0x0032BB07 File Offset: 0x00329D07
		private GlobalSettings GlobalSettings
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x06006DC7 RID: 28103 RVA: 0x0032BB10 File Offset: 0x00329D10
		public void Refresh(short groupId)
		{
			this._groupId = groupId;
			bool state = this.GlobalSettings.GetMapElementDisplayRuleGroupState(groupId);
			MapElementDisplayRuleGroupItem config = MapElementDisplayRuleGroup.Instance[groupId];
			this.imageIcon.SetSprite(config.Icon, false, null);
			this.textName.SetText(config.Name, true);
			this.itemScroll.OnItemRender -= this.OnItemRender;
			this.itemScroll.OnItemRender += this.OnItemRender;
			this.buttonEnableAll.ClearAndAddListener(new Action(this.OnClickButtonEnableAll));
			this.buttonDisableAll.ClearAndAddListener(new Action(this.OnClickButtonDisableAll));
			this.switchToggle.onValueChanged.RemoveAllListeners();
			this.switchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSwitchToggleValueChanged));
			this.switchToggle.SetIsOnWithoutNotify(!state);
			this._itemIdList.Clear();
			foreach (MapElementDisplayRuleItemItem itemConfig in ((IEnumerable<MapElementDisplayRuleItemItem>)MapElementDisplayRuleItem.Instance))
			{
				bool flag = itemConfig.Group != groupId;
				if (!flag)
				{
					short templateId = itemConfig.TemplateId;
					short num = templateId;
					if (num - 24 > 1)
					{
						if (num - 37 <= 1)
						{
							continue;
						}
						if (num == 42)
						{
							if (!SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled())
							{
								continue;
							}
						}
					}
					else if (!SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong))
					{
						continue;
					}
					this._itemIdList.Add(itemConfig.TemplateId);
				}
			}
			this.itemScroll.SetDataCount(this._itemIdList.Count);
		}

		// Token: 0x06006DC8 RID: 28104 RVA: 0x0032BCE4 File Offset: 0x00329EE4
		private void OnItemRender(int index, GameObject obj)
		{
			short id = this._itemIdList[index];
			MapElementDisplayRuleItem item = obj.GetComponent<MapElementDisplayRuleItem>();
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			bool pickupInteractable = settings.GetMapElementDisplayRuleItemState(40, true);
			bool interactable = id != 41 || pickupInteractable;
			item.Refresh(id, interactable);
		}

		// Token: 0x06006DC9 RID: 28105 RVA: 0x0032BD2C File Offset: 0x00329F2C
		private void OnClickButtonEnableAll()
		{
			foreach (short id in this._itemIdList)
			{
				this.GlobalSettings.SetMapElementDisplayRuleItemState(id, true);
			}
			this.itemScroll.ReRender();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06006DCA RID: 28106 RVA: 0x0032BDA4 File Offset: 0x00329FA4
		private void OnClickButtonDisableAll()
		{
			foreach (short id in this._itemIdList)
			{
				this.GlobalSettings.SetMapElementDisplayRuleItemState(id, false);
			}
			this.itemScroll.ReRender();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06006DCB RID: 28107 RVA: 0x0032BE1C File Offset: 0x0032A01C
		private void OnSwitchToggleValueChanged(bool isOn)
		{
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(this._groupId, !isOn);
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06006DCC RID: 28108 RVA: 0x0032BE44 File Offset: 0x0032A044
		private void Update()
		{
			short hoverItemId = -1;
			for (int i = 0; i < this.itemScroll.Scroll.Content.childCount; i++)
			{
				RectTransform itemRectTrans = this.itemScroll.Scroll.Content.GetChild(i) as RectTransform;
				Vector2 localPos;
				bool flag = RectTransformUtility.ScreenPointToLocalPointInRectangle(itemRectTrans, Input.mousePosition, UIManager.Instance.UiCamera, out localPos) && itemRectTrans.rect.Contains(localPos);
				if (flag)
				{
					MapElementDisplayRuleItem item = itemRectTrans.GetComponent<MapElementDisplayRuleItem>();
					hoverItemId = item.ItemId;
				}
			}
			bool flag2 = this._hoverItemId == hoverItemId;
			if (!flag2)
			{
				this._hoverItemId = hoverItemId;
				bool showMapElementMerchant = false;
				bool itemIsValid = this._hoverItemId >= 0;
				MapElementDisplayRuleItemItem itemConfig = MapElementDisplayRuleItem.Instance[this._hoverItemId];
				switch (this._groupId)
				{
				case 0:
				{
					MapBlockCharacterCountData mapBlockCharacterCountData;
					if (this._hoverItemId != -1)
					{
						(mapBlockCharacterCountData = new MapBlockCharacterCountData()).CharacterCountDict = new Dictionary<short, int>
						{
							{
								this._hoverItemId,
								999
							}
						};
					}
					else
					{
						mapBlockCharacterCountData = null;
					}
					MapBlockCharacterCountData data = mapBlockCharacterCountData;
					this.mapElementInfo.RefreshActorCount(data);
					break;
				}
				case 1:
				{
					bool flag3 = itemIsValid;
					if (flag3)
					{
						this.mapElementCharacter.Preview(this._hoverItemId);
					}
					break;
				}
				case 2:
				{
					showMapElementMerchant = (itemIsValid && itemConfig.MerchantType >= 0);
					this.mapElementInfo.RefreshPlaceMarkForDisplayRuleGroup(false);
					this.mapElementInfo.RefreshActorCount(null);
					bool flag4 = showMapElementMerchant;
					if (flag4)
					{
						this.mapElementMerchant.Init();
						this.mapElementMerchant.Refresh(new List<sbyte>
						{
							itemConfig.MerchantType
						}, true);
					}
					else
					{
						bool flag5 = this._hoverItemId == 28;
						if (flag5)
						{
							this.mapElementInfo.RefreshActorCount(null);
							this.mapElementInfo.RefreshPlaceMarkForDisplayRuleGroup(true);
						}
						else
						{
							bool flag6 = this._hoverItemId == 27;
							if (flag6)
							{
								MapBlockCharacterCountData mapBlockCharacterCountData2;
								if (this._hoverItemId != -1)
								{
									(mapBlockCharacterCountData2 = new MapBlockCharacterCountData()).CharacterCountDict = new Dictionary<short, int>
									{
										{
											this._hoverItemId,
											999
										}
									};
								}
								else
								{
									mapBlockCharacterCountData2 = null;
								}
								MapBlockCharacterCountData data = mapBlockCharacterCountData2;
								this.mapElementInfo.RefreshActorCount(data);
							}
						}
					}
					break;
				}
				case 3:
				{
					this.temporaryMarkPrefab.SetActive(this._hoverItemId == 39);
					short hoverItemId2 = this._hoverItemId;
					short num = hoverItemId2;
					if (num - 40 <= 1)
					{
						bool isInsight = this._hoverItemId == 40;
						MapElementPickupDisplayData mapElementPickupDisplayData = new MapElementPickupDisplayData
						{
							Pickup = MapPickup.CreateResource(Location.Invalid, 0, 1, 0, false)
						};
						this.mapElementPickup.Refresh(mapElementPickupDisplayData, isInsight, 2);
					}
					break;
				}
				}
				GameObject gameObject = this.mapElementInfo.gameObject;
				short num2 = this._groupId;
				gameObject.SetActive((num2 == 0 || num2 == 2) && itemIsValid);
				this.mapElementCharacter.gameObject.SetActive(this._groupId == 1 && itemIsValid);
				this.mapElementMerchant.gameObject.SetActive(this._groupId == 2 && showMapElementMerchant);
				this.mapElementCricket.gameObject.SetActive(this._groupId == 3 && this._hoverItemId == 36);
				this.mapElementAdventureMajorEvent.gameObject.SetActive(this._groupId == 3 && this._hoverItemId == 38);
				this.mapElementAdventureRemake.gameObject.SetActive(this._groupId == 3 && this._hoverItemId == 37);
				GameObject gameObject2 = this.mapElementPickup.gameObject;
				bool active;
				if (this._groupId == 3)
				{
					num2 = this._hoverItemId;
					active = (num2 == 40 || num2 == 41);
				}
				else
				{
					active = false;
				}
				gameObject2.SetActive(active);
			}
		}

		// Token: 0x04005166 RID: 20838
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04005167 RID: 20839
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04005168 RID: 20840
		[SerializeField]
		private CToggle switchToggle;

		// Token: 0x04005169 RID: 20841
		[SerializeField]
		private CButton buttonEnableAll;

		// Token: 0x0400516A RID: 20842
		[SerializeField]
		private CButton buttonDisableAll;

		// Token: 0x0400516B RID: 20843
		[SerializeField]
		private InfinityScroll itemScroll;

		// Token: 0x0400516C RID: 20844
		[Header("人物数量")]
		[SerializeField]
		private MapElementInfo mapElementInfo;

		// Token: 0x0400516D RID: 20845
		[Header("人物头像")]
		[SerializeField]
		private MapElementCharacter mapElementCharacter;

		// Token: 0x0400516E RID: 20846
		[Header("地图元素")]
		[SerializeField]
		private MapElementMerchant mapElementMerchant;

		// Token: 0x0400516F RID: 20847
		[Header("地图互动")]
		[SerializeField]
		private MapElementCricket mapElementCricket;

		// Token: 0x04005170 RID: 20848
		[SerializeField]
		private MapElementAdventureRemake mapElementAdventureRemake;

		// Token: 0x04005171 RID: 20849
		[SerializeField]
		private MapElementAdventureMajorEvent mapElementAdventureMajorEvent;

		// Token: 0x04005172 RID: 20850
		[SerializeField]
		private MapElementPickup mapElementPickup;

		// Token: 0x04005173 RID: 20851
		[SerializeField]
		private GameObject temporaryMarkPrefab;

		// Token: 0x04005174 RID: 20852
		private short _groupId;

		// Token: 0x04005175 RID: 20853
		private readonly List<short> _itemIdList = new List<short>();

		// Token: 0x04005176 RID: 20854
		private short _hoverItemId;
	}
}
