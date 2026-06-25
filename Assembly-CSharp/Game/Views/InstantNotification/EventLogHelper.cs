using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Information;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Information;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.TaiwuEvent.EventLog;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.InstantNotification
{
	// Token: 0x02000A16 RID: 2582
	public class EventLogHelper : Refers
	{
		// Token: 0x17000DBB RID: 3515
		// (get) Token: 0x06007E5E RID: 32350 RVA: 0x003AAEE0 File Offset: 0x003A90E0
		public IList<EventLogResultData> ResultList
		{
			get
			{
				IList<EventLogResultData> result;
				if (!this._isShowAll)
				{
					IList<EventLogResultData> list = this._filteredResultList;
					result = list;
				}
				else
				{
					EventLogData eventLogData = this._eventLogData;
					IList<EventLogResultData> list = (eventLogData != null) ? eventLogData.ResultList : null;
					result = (list ?? Array.Empty<EventLogResultData>());
				}
				return result;
			}
		}

		// Token: 0x06007E5F RID: 32351 RVA: 0x003AAF1C File Offset: 0x003A911C
		public void Init(IAsyncMethodRequestHandler parent, EventLogData data)
		{
			this._parent = parent;
			this._isDisplaying = true;
			this._eventLogData = data;
			EventLogResultData[] array;
			if (data == null)
			{
				array = null;
			}
			else
			{
				array = data.ResultList.Where(delegate(EventLogResultData resultData)
				{
					sbyte type = resultData.Type;
					return type == 0 || type == 1 || type == 2 || type == 30;
				}).ToArray<EventLogResultData>();
			}
			this._filteredResultList = (array ?? Array.Empty<EventLogResultData>());
			this.MarkInitContent();
		}

		// Token: 0x06007E60 RID: 32352 RVA: 0x003AAF90 File Offset: 0x003A9190
		private void MarkInitContent()
		{
			bool awakeCalled = this._awakeCalled;
			if (awakeCalled)
			{
				this.InitContent();
			}
			else
			{
				this._hasData = true;
			}
		}

		// Token: 0x06007E61 RID: 32353 RVA: 0x003AAFB8 File Offset: 0x003A91B8
		private void Awake()
		{
			this._nameColor = Colors.Instance["pinkyellow"].ColorToHexString("#");
			this._itemColor = Colors.Instance["grey"].ColorToHexString("#");
			this._positiveColor = Colors.Instance["brightblue"].ColorToHexString("#");
			this._negativeColor = Colors.Instance["brightred"].ColorToHexString("#");
			this.switchButton.Init(-1);
			this.switchButton.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this.OnSwitch(newTog);
			};
			this._arrowPool = new PoolItem("UI_EventLog_ArrowPrefab", this.arrow);
			this._singleValuePool = new PoolItem("UI_EventLog_SingleValuePrefab", this.singleValueComponent);
			this._statusPool = new PoolItem("UI_EventLog_StatusPrefab", this.statusComponent);
			this._dialogPool = new PoolItem("UI_EventLog_DialogPrefab", this.dialogComponent);
			this._dialog2Pool = new PoolItem("UI_EventLog_Dialog2Prefab", this.dialogComponent2);
			this._dialog3Pool = new PoolItem("UI_EventLog_Dialog3Prefab", this.dialogComponent3);
			this._dialog4Pool = new PoolItem("UI_EventLog_Dialog4Prefab", this.dialogComponent4);
			this._combatResultPool = new PoolItem("UI_EventLog_CombatResultPrefab", this.combatResultComponent);
			this._itemPool = new PoolItem("UI_EventLog_ItemPrefab", this.itemComponent);
			this._resourcePool = new PoolItem("UI_EventLog_ResourcePrefab", this.resourceComponent);
			this._teammatePool = new PoolItem("UI_EventLog_TeammatePrefab", this.teammateComponent);
			this._combatSkillPool = new PoolItem("UI_EventLog_CombatSkillPrefab", this.combatSkillComponent);
			this._lifeSkillPool = new PoolItem("UI_EventLog_LifeSkillPrefab", this.lifeSkillComponent);
			this._relationPool = new PoolItem("UI_EventLog_RelationPrefab", this.relationComponent);
			this._featurePool = new PoolItem("UI_EventLog_FeaturePrefab", this.featureComponent);
			this._professionPool = new PoolItem("UI_EventLog_ProfessionPrefab", this.professionComponent);
			this._normalInformationPool = new PoolItem("UI_EventLog_NormalInformationPrefab", this.informationComponent);
			this._secretInformationPool = new PoolItem("UI_EventLog_SecretInformationPrefab", this.secretInformationComponent);
			this._endingPool = new PoolItem("UI_EventLog_EndingPrefab", this.endingComponent);
			int statusHeight = (int)this.statusComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int combatResultHeight = (int)this.combatResultComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int itemHeight = (int)this.itemComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int resourceHeight = (int)this.resourceComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int singleValueHeight = (int)this.singleValueComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int teammateHeight = (int)this.teammateComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int combatSkillHeight = (int)this.combatSkillComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int lifeSkillHeight = (int)this.lifeSkillComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int relationHeight = (int)this.relationComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int featureHeight = (int)this.featureComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int professionHeight = (int)this.professionComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int informationHeight = (int)this.informationComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int secretInformationHeight = (int)this.secretInformationComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			int endingHeight = (int)this.endingComponent.GetComponent<RectTransform>().sizeDelta.y + 15;
			this._constantElementHeights.Add(3, statusHeight);
			this._constantElementHeights.Add(4, statusHeight);
			this._constantElementHeights.Add(5, statusHeight);
			this._constantElementHeights.Add(6, statusHeight);
			this._constantElementHeights.Add(7, statusHeight);
			this._constantElementHeights.Add(8, combatResultHeight);
			this._constantElementHeights.Add(9, combatResultHeight);
			this._constantElementHeights.Add(10, combatResultHeight);
			this._constantElementHeights.Add(11, itemHeight);
			this._constantElementHeights.Add(12, resourceHeight);
			this._constantElementHeights.Add(27, resourceHeight);
			this._constantElementHeights.Add(13, singleValueHeight);
			this._constantElementHeights.Add(14, teammateHeight);
			this._constantElementHeights.Add(15, statusHeight);
			this._constantElementHeights.Add(16, singleValueHeight);
			this._constantElementHeights.Add(17, singleValueHeight);
			this._constantElementHeights.Add(18, singleValueHeight);
			this._constantElementHeights.Add(19, singleValueHeight);
			this._constantElementHeights.Add(20, singleValueHeight);
			this._constantElementHeights.Add(21, combatSkillHeight);
			this._constantElementHeights.Add(22, lifeSkillHeight);
			this._constantElementHeights.Add(23, singleValueHeight);
			this._constantElementHeights.Add(24, relationHeight);
			this._constantElementHeights.Add(25, featureHeight);
			this._constantElementHeights.Add(26, professionHeight);
			this._constantElementHeights.Add(28, informationHeight);
			this._constantElementHeights.Add(29, secretInformationHeight);
			this._constantElementHeights.Add(30, endingHeight);
			this.loopScrollView.InitLoop(this.lineContainer, 0, new Action<Transform, int>(this.RenderLineContainer), new Action<Transform>(this.ReturnLineContainer));
			this._awakeCalled = true;
			bool hasData = this._hasData;
			if (hasData)
			{
				this.InitContent();
				this._hasData = false;
			}
		}

		// Token: 0x06007E62 RID: 32354 RVA: 0x003AB57B File Offset: 0x003A977B
		private void OnEnable()
		{
			this.switchButton.SetWithoutNotify(this._isShowAll ? 1 : 0);
		}

		// Token: 0x06007E63 RID: 32355 RVA: 0x003AB596 File Offset: 0x003A9796
		private void OnDisable()
		{
			this._isDisplaying = false;
		}

		// Token: 0x06007E64 RID: 32356 RVA: 0x003AB5A0 File Offset: 0x003A97A0
		public void ClearData()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
			{
				this._eventLogData = null;
			});
		}

		// Token: 0x06007E65 RID: 32357 RVA: 0x003AB5BC File Offset: 0x003A97BC
		private void OnDestroy()
		{
			PoolItem arrowPool = this._arrowPool;
			if (arrowPool != null)
			{
				arrowPool.Destroy();
			}
			this._arrowPool = null;
			PoolItem singleValuePool = this._singleValuePool;
			if (singleValuePool != null)
			{
				singleValuePool.Destroy();
			}
			this._singleValuePool = null;
			PoolItem statusPool = this._statusPool;
			if (statusPool != null)
			{
				statusPool.Destroy();
			}
			this._statusPool = null;
			PoolItem dialogPool = this._dialogPool;
			if (dialogPool != null)
			{
				dialogPool.Destroy();
			}
			this._dialogPool = null;
			PoolItem dialog2Pool = this._dialog2Pool;
			if (dialog2Pool != null)
			{
				dialog2Pool.Destroy();
			}
			this._dialog2Pool = null;
			PoolItem dialog3Pool = this._dialog3Pool;
			if (dialog3Pool != null)
			{
				dialog3Pool.Destroy();
			}
			this._dialog3Pool = null;
			PoolItem dialog4Pool = this._dialog4Pool;
			if (dialog4Pool != null)
			{
				dialog4Pool.Destroy();
			}
			this._dialog4Pool = null;
			PoolItem combatResultPool = this._combatResultPool;
			if (combatResultPool != null)
			{
				combatResultPool.Destroy();
			}
			this._combatResultPool = null;
			PoolItem itemPool = this._itemPool;
			if (itemPool != null)
			{
				itemPool.Destroy();
			}
			this._itemPool = null;
			PoolItem resourcePool = this._resourcePool;
			if (resourcePool != null)
			{
				resourcePool.Destroy();
			}
			this._resourcePool = null;
			PoolItem teammatePool = this._teammatePool;
			if (teammatePool != null)
			{
				teammatePool.Destroy();
			}
			this._teammatePool = null;
			PoolItem combatSkillPool = this._combatSkillPool;
			if (combatSkillPool != null)
			{
				combatSkillPool.Destroy();
			}
			this._combatSkillPool = null;
			PoolItem lifeSkillPool = this._lifeSkillPool;
			if (lifeSkillPool != null)
			{
				lifeSkillPool.Destroy();
			}
			this._lifeSkillPool = null;
			PoolItem relationPool = this._relationPool;
			if (relationPool != null)
			{
				relationPool.Destroy();
			}
			this._relationPool = null;
			PoolItem featurePool = this._featurePool;
			if (featurePool != null)
			{
				featurePool.Destroy();
			}
			this._featurePool = null;
			PoolItem professionPool = this._professionPool;
			if (professionPool != null)
			{
				professionPool.Destroy();
			}
			this._professionPool = null;
			PoolItem normalInformationPool = this._normalInformationPool;
			if (normalInformationPool != null)
			{
				normalInformationPool.Destroy();
			}
			this._normalInformationPool = null;
			PoolItem secretInformationPool = this._secretInformationPool;
			if (secretInformationPool != null)
			{
				secretInformationPool.Destroy();
			}
			this._secretInformationPool = null;
			PoolItem endingPool = this._endingPool;
			if (endingPool != null)
			{
				endingPool.Destroy();
			}
			this._endingPool = null;
		}

		// Token: 0x06007E66 RID: 32358 RVA: 0x003AB7A8 File Offset: 0x003A99A8
		private void OnSwitch(int index)
		{
			this._isShowAll = (index == 1);
			bool isDisplaying = this._isDisplaying;
			if (isDisplaying)
			{
				this.RefreshContent();
			}
			else
			{
				this.InitContent();
			}
		}

		// Token: 0x06007E67 RID: 32359 RVA: 0x003AB7DC File Offset: 0x003A99DC
		private void ReturnLineContainer(Transform trans)
		{
			for (int i = 0; i < trans.childCount; i++)
			{
				GameObject component = trans.GetChild(i).gameObject;
				Refers refers = component.GetComponent<Refers>();
				bool flag = refers == null;
				if (flag)
				{
					this._arrowPool.DestroyObject(component);
				}
				else
				{
					switch (refers.UserInt)
					{
					case 0:
					case 1:
					{
						float userFloat = refers.UserFloat;
						float num = userFloat;
						if (num <= 2f)
						{
							if (num != 1f)
							{
								if (num == 2f)
								{
									this._dialog2Pool.DestroyObject(component);
								}
							}
							else
							{
								this._dialogPool.DestroyObject(component);
							}
						}
						else if (num != 3f)
						{
							if (num == 4f)
							{
								this._dialog4Pool.DestroyObject(component);
							}
						}
						else
						{
							this._dialog3Pool.DestroyObject(component);
						}
						break;
					}
					case 2:
						this._dialog2Pool.DestroyObject(component);
						break;
					case 3:
						this._statusPool.DestroyObject(component);
						break;
					case 4:
						this._statusPool.DestroyObject(component);
						break;
					case 5:
						this._statusPool.DestroyObject(component);
						break;
					case 6:
						this._statusPool.DestroyObject(component);
						break;
					case 7:
						this._statusPool.DestroyObject(component);
						break;
					case 8:
					case 9:
					case 10:
						this._combatResultPool.DestroyObject(component);
						break;
					case 11:
						this._itemPool.DestroyObject(component);
						break;
					case 12:
					case 27:
						this._resourcePool.DestroyObject(component);
						break;
					case 13:
						this._singleValuePool.DestroyObject(component);
						break;
					case 14:
						this._teammatePool.DestroyObject(component);
						break;
					case 15:
						this._statusPool.DestroyObject(component);
						break;
					case 16:
						this._singleValuePool.DestroyObject(component);
						break;
					case 17:
					case 18:
						this._singleValuePool.DestroyObject(component);
						break;
					case 19:
						this._singleValuePool.DestroyObject(component);
						break;
					case 20:
						this._singleValuePool.DestroyObject(component);
						break;
					case 21:
						this._combatSkillPool.DestroyObject(component);
						break;
					case 22:
						this._lifeSkillPool.DestroyObject(component);
						break;
					case 23:
						this._singleValuePool.DestroyObject(component);
						break;
					case 24:
						this._relationPool.DestroyObject(component);
						break;
					case 25:
						this._featurePool.DestroyObject(component);
						break;
					case 26:
						this._professionPool.DestroyObject(component);
						break;
					case 28:
						this._normalInformationPool.DestroyObject(component);
						break;
					case 29:
						this._secretInformationPool.DestroyObject(component);
						break;
					case 30:
						this._endingPool.DestroyObject(component);
						break;
					}
				}
			}
		}

		// Token: 0x06007E68 RID: 32360 RVA: 0x003ABAF4 File Offset: 0x003A9CF4
		private void RenderLineContainer(Transform tf, int index)
		{
			IList<EventLogResultData> lst = this.ResultList;
			bool flag = lst.CheckIndex(index);
			if (flag)
			{
				this.RenderLineContainer(tf, lst[index]);
			}
		}

		// Token: 0x06007E69 RID: 32361 RVA: 0x003ABB24 File Offset: 0x003A9D24
		private float RenderLineContainer(Transform tf, EventLogResultData result)
		{
			this.ReturnLineContainer(tf);
			GameObject obj = null;
			switch (result.Type)
			{
			case 0:
				obj = this.UpdateDialogDisplay(result, false);
				break;
			case 1:
				obj = this.UpdateDialogDisplay(result, true);
				break;
			case 2:
				obj = this._dialog2Pool.GetObject();
				this.UpdateResponseDisplay(obj, result);
				break;
			case 3:
				obj = this.UpdateHappinessDisplay(result);
				break;
			case 4:
				obj = this.UpdateFameDisplay(result);
				break;
			case 5:
				obj = this.UpdateFavorabilityToTaiwuDisplay(result);
				break;
			case 6:
				obj = this.UpdateInfectionDisplay(result);
				break;
			case 7:
				obj = this.UpdateInfectionStatusDisplay(result);
				break;
			case 8:
			case 9:
			case 10:
				obj = this._combatResultPool.GetObject();
				this.UpdateCombatResultDisplay(obj, result);
				break;
			case 11:
				obj = this._itemPool.GetObject();
				this.UpdateItemDisplay(obj, result);
				break;
			case 12:
				obj = this._resourcePool.GetObject();
				this.UpdateResourceDisplay(obj, result);
				break;
			case 13:
				obj = this.UpdateSpiritualDebtDisplay(result);
				break;
			case 14:
				obj = this._teammatePool.GetObject();
				this.UpdateTeammateDisplay(obj, result);
				break;
			case 15:
				obj = this.UpdateHealthDisplay(result);
				break;
			case 16:
				obj = this.UpdateMainAttributeDisplay(result);
				break;
			case 17:
			case 18:
				obj = this.UpdateInjuryDisplay(result);
				break;
			case 19:
				obj = this.UpdatePoisonDisplay(result);
				break;
			case 20:
				obj = this.UpdateDisorderOfQiDisplay(result);
				break;
			case 21:
				obj = this._combatSkillPool.GetObject();
				this.UpdateCombatSkillDisplay(obj, result);
				break;
			case 22:
				obj = this._lifeSkillPool.GetObject();
				this.UpdateLifeSkillDisplay(obj, result);
				break;
			case 23:
				obj = this.UpdateApprovedTaiwuDisplay(result);
				break;
			case 24:
				obj = this._relationPool.GetObject();
				this.UpdateRelationDisplay(obj, result);
				break;
			case 25:
				obj = this._featurePool.GetObject();
				this.UpdateFeatureDisplay(obj, result);
				break;
			case 26:
				obj = this._professionPool.GetObject();
				this.UpdateProfessionDisplay(obj, result);
				break;
			case 27:
				obj = this._resourcePool.GetObject();
				this.UpdateExpDisplay(obj, result);
				break;
			case 28:
				obj = this._normalInformationPool.GetObject();
				this.UpdateNormalInfoDisplay(obj, result);
				break;
			case 29:
				obj = this._secretInformationPool.GetObject();
				this.UpdateSecretInfoDisplay(obj, result);
				break;
			case 30:
				obj = this._endingPool.GetObject();
				break;
			}
			bool flag = obj == null;
			float result2;
			if (flag)
			{
				result2 = 0f;
			}
			else
			{
				Refers refers = obj.GetComponent<Refers>();
				refers.UserInt = (int)result.Type;
				bool flag2 = !string.IsNullOrEmpty(result.Text);
				if (flag2)
				{
					TextMeshProUGUI text = refers.CGet<TextMeshProUGUI>("MainContent");
					text.text = result.Text.ColorReplace();
				}
				int val;
				float height = this._constantElementHeights.TryGetValue(result.Type, out val) ? ((float)val) : this.CalculateTextHeights(result, obj.GetComponent<Refers>());
				tf.GetComponent<RectTransform>().sizeDelta = new Vector2(1360f, height);
				LayoutElement le = obj.GetComponent<LayoutElement>();
				bool flag3 = le != null;
				if (flag3)
				{
					le.preferredHeight = height;
				}
				LayoutElement le2 = tf.GetComponent<LayoutElement>();
				bool flag4 = le2 != null;
				if (flag4)
				{
					le2.preferredHeight = height;
				}
				obj.transform.SetParent(tf, false);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.SetAsFirstSibling();
				this.AddArrow(tf.gameObject, result);
				result2 = height;
			}
			return result2;
		}

		// Token: 0x06007E6A RID: 32362 RVA: 0x003ABEF4 File Offset: 0x003AA0F4
		private int GetDialogType(EventLogResultData result)
		{
			int id = result.ValueList[1];
			int id2 = result.ValueList[2];
			bool isLeftCharacterActor = result.LeftActorData != null;
			bool isRightCharacterActor = result.RightActorData != null;
			bool flag = !isLeftCharacterActor && !isRightCharacterActor && id < 0 && id2 < 0;
			int result2;
			if (flag)
			{
				result2 = 4;
			}
			else
			{
				bool flag2 = !isLeftCharacterActor && id < 0;
				if (flag2)
				{
					result2 = 3;
				}
				else
				{
					bool flag3 = !isRightCharacterActor && id2 < 0;
					if (flag3)
					{
						result2 = 2;
					}
					else
					{
						result2 = 1;
					}
				}
			}
			return result2;
		}

		// Token: 0x06007E6B RID: 32363 RVA: 0x003ABF80 File Offset: 0x003AA180
		private void InitContent()
		{
			this.loopScrollView.totalCount = this.ResultList.Count;
			this.loopScrollView.RefillCellsFromEnd(0, false);
			this.loopScrollView.RefillCellsAtCurrentPosition();
			this.loopScrollView.RefillCellsFromEnd(0, false);
			bool flag = this.noContent;
			if (flag)
			{
				this.noContent.SetActive(this.ResultList.Count == 0);
			}
		}

		// Token: 0x06007E6C RID: 32364 RVA: 0x003ABFF8 File Offset: 0x003AA1F8
		private void RefreshContent()
		{
			this.loopScrollView.totalCount = this.ResultList.Count;
			this.loopScrollView.RefillCellsFromEnd(0, false);
			bool flag = this.noContent;
			if (flag)
			{
				this.noContent.SetActive(this.ResultList.Count == 0);
			}
		}

		// Token: 0x06007E6D RID: 32365 RVA: 0x003AC058 File Offset: 0x003AA258
		private void RefreshCharacterDisplayData(EventLogResultData result, string charName, int charId, GameObject root, string nameComponent, string thumbnailComponent)
		{
			bool flag = charId == -1;
			if (flag)
			{
				bool flag2 = !string.IsNullOrEmpty(nameComponent);
				if (flag2)
				{
					root.GetComponent<Refers>().CGet<TMP_Text>(nameComponent).text = "";
				}
				bool flag3 = !string.IsNullOrEmpty(thumbnailComponent);
				if (flag3)
				{
					root.GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>(thumbnailComponent).gameObject.SetActive(false);
				}
			}
			else
			{
				Dictionary<int, NameStringAndAvatar> charDict = result.CharDict;
				NameStringAndAvatar displayData;
				bool flag4 = charDict != null && charDict.TryGetValue(charId, out displayData);
				if (flag4)
				{
					bool flag5 = string.IsNullOrEmpty(charName);
					if (flag5)
					{
						charName = displayData.Name;
					}
					bool flag6 = !string.IsNullOrEmpty(nameComponent);
					if (flag6)
					{
						root.GetComponent<Refers>().CGet<TMP_Text>(nameComponent).text = charName;
					}
					bool flag7 = !string.IsNullOrEmpty(thumbnailComponent);
					if (flag7)
					{
						EventModel eventModel = SingletonObject.getInstance<EventModel>();
						short taiwuClothingDisplayId;
						bool flag8 = eventModel.TryGetTaiwuClothingDisplayId(charId, out taiwuClothingDisplayId);
						if (flag8)
						{
							displayData.Avatar.ClothingDisplayId = taiwuClothingDisplayId;
						}
						Game.Components.Avatar.Avatar avatar = root.GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>(thumbnailComponent);
						avatar.Refresh(displayData.Avatar, displayData.CharTemplateId);
						avatar.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06007E6E RID: 32366 RVA: 0x003AC188 File Offset: 0x003AA388
		private void RefreshMerchantDisplayData(short templateId, GameObject root, string nameComponent, string thumbnailComponent)
		{
			MerchantTypeItem merchantTypeConfig = MerchantType.Instance[Merchant.Instance[(int)templateId].MerchantType];
			string merchantName = merchantTypeConfig.Name;
			bool flag = !string.IsNullOrEmpty(nameComponent);
			if (flag)
			{
				root.GetComponent<Refers>().CGet<TMP_Text>(nameComponent).text = merchantName;
			}
			bool flag2 = !string.IsNullOrEmpty(thumbnailComponent);
			if (flag2)
			{
				Game.Components.Avatar.Avatar avatar = root.GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>(thumbnailComponent);
				ResLoader.LoadModOrGameResource<Texture2D>(this._npcAvatarTexturePath + "/" + merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(avatar.Refresh), null);
				avatar.gameObject.SetActive(true);
			}
		}

		// Token: 0x06007E6F RID: 32367 RVA: 0x003AC230 File Offset: 0x003AA430
		private void RefreshActorDisplayData(string charName, EventActorData data, GameObject root, string nameComponent, string thumbnailComponent)
		{
			bool flag = !string.IsNullOrEmpty(nameComponent);
			if (flag)
			{
				root.GetComponent<Refers>().CGet<TMP_Text>(nameComponent).text = charName;
			}
			bool flag2 = !string.IsNullOrEmpty(thumbnailComponent);
			if (flag2)
			{
				string texture = EventActors.Instance[data.TemplateId].Texture;
				Game.Components.Avatar.Avatar avatar = root.GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>(thumbnailComponent);
				bool flag3 = !string.IsNullOrEmpty(texture);
				if (flag3)
				{
					ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/NpcFace/SmallFace/", texture), new Action<Texture2D>(avatar.Refresh), null, false);
				}
				else
				{
					data.AvatarData.ClothDisplayId = data.ClothDisplayId;
					avatar.Refresh(data.AvatarData, (short)data.Age);
				}
				avatar.gameObject.SetActive(true);
			}
		}

		// Token: 0x06007E70 RID: 32368 RVA: 0x003AC2FC File Offset: 0x003AA4FC
		private void RefreshStatusDisplayData(Refers refers, string charName, string propertyName, bool isPositive, bool isImageInMid, string image = null, string specialSuffix = null, bool colorReversed = false)
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			if (isImageInMid)
			{
				this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(LanguageKey.LK_EventLog_Possessive));
				refers.CGet<TextMeshProUGUI>("Content1").text = builder.ToString().ColorReplace();
				builder.Clear();
				this.BuildString(builder, this._nameColor, propertyName);
			}
			else
			{
				refers.CGet<TextMeshProUGUI>("Content1").text = "";
				this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(LanguageKey.LK_EventLog_Possessive), propertyName);
			}
			this.BuildString(builder, (colorReversed ? (!isPositive) : isPositive) ? this._positiveColor : this._negativeColor, specialSuffix ?? LocalStringManager.Get(isPositive ? LanguageKey.LK_EventLog_Result_Up : LanguageKey.LK_EventLog_Result_Down));
			refers.CGet<TextMeshProUGUI>("Content2").text = builder.ToString().ColorReplace();
			bool flag = string.IsNullOrEmpty(image);
			if (flag)
			{
				refers.CGet<GameObject>("IconContainer").SetActive(false);
			}
			else
			{
				CImage icon = refers.CGet<CImage>("Icon");
				GameObject iconContainer = refers.CGet<GameObject>("IconContainer");
				icon.SetSprite(image, true, null);
				icon.SetNativeSize();
				iconContainer.GetComponent<RectTransform>().sizeDelta = icon.GetComponent<RectTransform>().sizeDelta;
				iconContainer.SetActive(true);
			}
		}

		// Token: 0x06007E71 RID: 32369 RVA: 0x003AC46C File Offset: 0x003AA66C
		private void RefreshSingleValueDisplayData(Refers refers, string charName, string originalValue, string newValue, sbyte resultType, string image, int? subType = null)
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			builder.Append(charName);
			builder.Append(":");
			bool flag = string.IsNullOrEmpty(image);
			if (flag)
			{
				refers.CGet<GameObject>("IconContainer").SetActive(false);
			}
			else
			{
				CImage icon = refers.CGet<CImage>("Icon");
				GameObject iconContainer = refers.CGet<GameObject>("IconContainer");
				icon.SetSprite(image, true, null);
				icon.SetNativeSize();
				iconContainer.GetComponent<RectTransform>().sizeDelta = icon.GetComponent<RectTransform>().sizeDelta;
				iconContainer.SetActive(true);
			}
			refers.CGet<TextMeshProUGUI>("CharacterName").text = builder.ToString();
			refers.CGet<TextMeshProUGUI>("PropertyName").text = this.GetTypeString(resultType, subType).ColorReplace();
			refers.CGet<TextMeshProUGUI>("OriginalValue").text = originalValue;
			refers.CGet<TextMeshProUGUI>("NewValue").text = newValue;
		}

		// Token: 0x06007E72 RID: 32370 RVA: 0x003AC563 File Offset: 0x003AA763
		private void RefreshSelectableCharacterDisplayData(AvatarNormalWithName actor, int charId)
		{
			actor.Set(this._parent, charId);
		}

		// Token: 0x06007E73 RID: 32371 RVA: 0x003AC574 File Offset: 0x003AA774
		private string GetTypeString(sbyte resultType, int? subtype = null)
		{
			if (!true)
			{
			}
			string result;
			switch (resultType)
			{
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness);
				goto IL_24A;
			case 4:
				result = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Fame);
				goto IL_24A;
			case 5:
				result = LocalStringManager.Get(LanguageKey.LK_EventLog_Favorability);
				goto IL_24A;
			case 6:
				result = LocalStringManager.Get(LanguageKey.LK_EventLog_Infection);
				goto IL_24A;
			case 13:
				result = LocalStringManager.Get(LanguageKey.LK_Area_Debt_Tip_Title);
				goto IL_24A;
			case 15:
				result = LocalStringManager.Get(LanguageKey.LK_EventLog_Health);
				goto IL_24A;
			case 16:
			{
				if (!true)
				{
				}
				string text;
				if (subtype != null)
				{
					switch (subtype.GetValueOrDefault())
					{
					case 0:
						text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Strength);
						goto IL_161;
					case 1:
						text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Dexterity);
						goto IL_161;
					case 2:
						text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Concentration);
						goto IL_161;
					case 3:
						text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Vitality);
						goto IL_161;
					case 4:
						text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Energy);
						goto IL_161;
					case 5:
						text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Intelligence);
						goto IL_161;
					}
				}
				text = "";
				IL_161:
				if (!true)
				{
				}
				result = text;
				goto IL_24A;
			}
			case 17:
				result = this.GetInjuryString(true, subtype);
				goto IL_24A;
			case 18:
				result = this.GetInjuryString(false, subtype);
				goto IL_24A;
			case 19:
			{
				if (!true)
				{
				}
				string text;
				if (subtype != null)
				{
					switch (subtype.GetValueOrDefault())
					{
					case 0:
						text = LocalStringManager.Get(LanguageKey.LK_Poison_Name_0);
						goto IL_213;
					case 1:
						text = LocalStringManager.Get(LanguageKey.LK_Poison_Name_1);
						goto IL_213;
					case 2:
						text = LocalStringManager.Get(LanguageKey.LK_Poison_Name_2);
						goto IL_213;
					case 3:
						text = LocalStringManager.Get(LanguageKey.LK_Poison_Name_3);
						goto IL_213;
					case 4:
						text = LocalStringManager.Get(LanguageKey.LK_Poison_Name_4);
						goto IL_213;
					case 5:
						text = LocalStringManager.Get(LanguageKey.LK_Poison_Name_5);
						goto IL_213;
					}
				}
				text = "";
				IL_213:
				if (!true)
				{
				}
				result = text;
				goto IL_24A;
			}
			case 20:
				result = LocalStringManager.Get(LanguageKey.LK_Qi_Disorder);
				goto IL_24A;
			case 23:
				result = LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_OrganizationApprove);
				goto IL_24A;
			case 27:
				result = LocalStringManager.Get(LanguageKey.LK_Exp);
				goto IL_24A;
			}
			result = "";
			IL_24A:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E74 RID: 32372 RVA: 0x003AC7D8 File Offset: 0x003AA9D8
		private string GetInjuryString(bool isInnerInjury, int? bodyPartType)
		{
			if (!true)
			{
			}
			string text;
			if (bodyPartType != null)
			{
				switch (bodyPartType.GetValueOrDefault())
				{
				case 0:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_Chest);
					goto IL_A2;
				case 1:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_Belly);
					goto IL_A2;
				case 2:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_Head);
					goto IL_A2;
				case 3:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_LeftHand);
					goto IL_A2;
				case 4:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_RightHand);
					goto IL_A2;
				case 5:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_LeftFoot);
					goto IL_A2;
				case 6:
					text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_RightFoot);
					goto IL_A2;
				}
			}
			text = string.Empty;
			IL_A2:
			if (!true)
			{
			}
			string bodyPartName = text;
			string injuryLabel = LocalStringManager.Get(LanguageKey.LK_Injury);
			string injuryColor = Colors.Instance[isInnerInjury ? "innerinjury" : "outterinjury"].ColorToHexString("#");
			LanguageKey id = LanguageKey.LK_EventLog_BodyPartInjury;
			string str = bodyPartName;
			text = injuryColor;
			object arg = str.SetColor(text.Substring(1, text.Length - 1));
			string str2 = injuryLabel;
			text = injuryColor;
			return LocalStringManager.GetFormat(id, arg, str2.SetColor(text.Substring(1, text.Length - 1))).ColorReplace();
		}

		// Token: 0x06007E75 RID: 32373 RVA: 0x003AC8FF File Offset: 0x003AAAFF
		private string GetNameString(EventLogResultData result, int id)
		{
			Dictionary<int, NameStringAndAvatar> charDict = result.CharDict;
			return ((charDict != null) ? charDict.GetValueOrDefault(id).Name : null) ?? "";
		}

		// Token: 0x06007E76 RID: 32374 RVA: 0x003AC922 File Offset: 0x003AAB22
		private void BuildString(StringBuilder builder, string color, string text)
		{
			builder.Append("<color=");
			builder.Append(color);
			builder.Append(">");
			builder.Append(text);
			builder.Append("</color>");
		}

		// Token: 0x06007E77 RID: 32375 RVA: 0x003AC959 File Offset: 0x003AAB59
		private void BuildString(StringBuilder builder, string color, string text1, string text2)
		{
			builder.Append("<color=");
			builder.Append(color);
			builder.Append(">");
			builder.Append(text1);
			builder.Append(text2);
			builder.Append("</color>");
		}

		// Token: 0x06007E78 RID: 32376 RVA: 0x003AC99C File Offset: 0x003AAB9C
		private void BuildString(StringBuilder builder, string color, string text1, string text2, string text3)
		{
			builder.Append("<color=");
			builder.Append(color);
			builder.Append(">");
			builder.Append(text1);
			builder.Append(text2);
			builder.Append(text3);
			builder.Append("</color>");
		}

		// Token: 0x06007E79 RID: 32377 RVA: 0x003AC9F0 File Offset: 0x003AABF0
		private void BuildString(StringBuilder builder, string color, string text1, string text2, string text3, string text4)
		{
			builder.Append("<color=");
			builder.Append(color);
			builder.Append(">");
			builder.Append(text1);
			builder.Append(text2);
			builder.Append(text3);
			builder.Append(text4);
			builder.Append("</color>");
		}

		// Token: 0x06007E7A RID: 32378 RVA: 0x003ACA50 File Offset: 0x003AAC50
		private void AddArrow(GameObject renderedLineContainer, EventLogResultData result)
		{
		}

		// Token: 0x06007E7B RID: 32379 RVA: 0x003ACA60 File Offset: 0x003AAC60
		private void CreateArrow(GameObject renderedLineContainer, bool isLeft)
		{
			GameObject obj = this._arrowPool.GetObject();
			obj.transform.SetParent(renderedLineContainer.transform, false);
			if (isLeft)
			{
				obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 180f, 0f);
				obj.GetComponent<RectTransform>().localPosition = new Vector3((float)(-(float)this.ArrowOffset), 0f, 0f);
			}
			else
			{
				obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
				obj.GetComponent<RectTransform>().localPosition = new Vector3((float)this.ArrowOffset, 0f, 0f);
			}
		}

		// Token: 0x06007E7C RID: 32380 RVA: 0x003ACB20 File Offset: 0x003AAD20
		private GameObject UpdateDialogDisplay(EventLogResultData result, bool isMerchant)
		{
			int id = result.ValueList[1];
			int id2 = result.ValueList[2];
			bool isLeftCharacterActor = result.LeftActorData != null;
			bool isRightCharacterActor = result.RightActorData != null;
			int dialogType = this.GetDialogType(result);
			GameObject obj;
			switch (dialogType)
			{
			case 2:
			{
				obj = this._dialog2Pool.GetObject();
				bool flag = isLeftCharacterActor;
				if (flag)
				{
					this.RefreshActorDisplayData(result.LeftName, result.LeftActorData, obj, "LeftName", "LeftActor");
				}
				else
				{
					this.RefreshCharacterDisplayData(result, result.LeftName, id, obj, "LeftName", "LeftActor");
				}
				obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").SetActive(false);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("MainContent").alignment = TextAlignmentOptions.Center;
				break;
			}
			case 3:
				obj = this._dialog3Pool.GetObject();
				if (isMerchant)
				{
					this.RefreshMerchantDisplayData(result.RightActorData.TemplateId, obj, "RightName", "RightActor");
				}
				else
				{
					bool flag2 = isRightCharacterActor;
					if (flag2)
					{
						this.RefreshActorDisplayData(result.RightName, result.RightActorData, obj, "RightName", "RightActor");
					}
					else
					{
						this.RefreshCharacterDisplayData(result, result.RightName, id2, obj, "RightName", "RightActor");
					}
				}
				break;
			case 4:
				obj = this._dialog4Pool.GetObject();
				break;
			default:
			{
				obj = this._dialogPool.GetObject();
				if (isMerchant)
				{
					this.RefreshMerchantDisplayData(result.RightActorData.TemplateId, obj, "RightName", "RightActor");
				}
				else
				{
					bool flag3 = isRightCharacterActor;
					if (flag3)
					{
						this.RefreshActorDisplayData(result.RightName, result.RightActorData, obj, "RightName", "RightActor");
					}
					else
					{
						this.RefreshCharacterDisplayData(result, result.RightName, id2, obj, "RightName", "RightActor");
					}
				}
				bool flag4 = isLeftCharacterActor;
				if (flag4)
				{
					this.RefreshActorDisplayData(result.LeftName, result.LeftActorData, obj, "LeftName", "LeftActor");
				}
				else
				{
					this.RefreshCharacterDisplayData(result, result.LeftName, id, obj, "LeftName", "LeftActor");
				}
				break;
			}
			}
			obj.GetComponent<Refers>().UserFloat = (float)dialogType;
			return obj;
		}

		// Token: 0x06007E7D RID: 32381 RVA: 0x003ACD68 File Offset: 0x003AAF68
		private void UpdateResponseDisplay(GameObject obj, EventLogResultData result)
		{
			this.RefreshCharacterDisplayData(result, result.LeftName, result.ValueList[1], obj, "LeftName", "LeftActor");
			int behaviorType = result.ValueList[2] - 1;
			bool flag = behaviorType == -1;
			if (flag)
			{
				obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").SetActive(false);
			}
			else
			{
				Refers refers = obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").GetComponent<Refers>();
				refers.CGet<CImage>("Icon").SetSprite(Config.BehaviorType.Instance.GetItem((short)behaviorType).Icon, false, null);
				refers.CGet<CImage>("Icon").rectTransform.sizeDelta = this.imageSizeDelta;
				refers.CGet<TextMeshProUGUI>("InfoValue").text = CommonUtils.GetBehaviorString((sbyte)behaviorType);
				obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").SetActive(true);
			}
		}

		// Token: 0x06007E7E RID: 32382 RVA: 0x003ACE58 File Offset: 0x003AB058
		private GameObject UpdateHappinessDisplay(EventLogResultData result)
		{
			GameObject obj = this._statusPool.GetObject();
			this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, true, string.Format("ui9_icon_mood_big_{0}", HappinessType.GetHappinessType((sbyte)Math.Clamp(result.ValueList[3], -119, 119))), null, false);
			return obj;
		}

		// Token: 0x06007E7F RID: 32383 RVA: 0x003ACEF8 File Offset: 0x003AB0F8
		private GameObject UpdateFameDisplay(EventLogResultData result)
		{
			GameObject obj = this._statusPool.GetObject();
			this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, true, "taiwuevent_01_history_icon_9", null, false);
			return obj;
		}

		// Token: 0x06007E80 RID: 32384 RVA: 0x003ACF70 File Offset: 0x003AB170
		private void UpdateNormalInfoDisplay(GameObject obj, EventLogResultData result)
		{
			NormalInformation info = new NormalInformation((short)result.ValueList[2], (sbyte)result.ValueList[3]);
			InformationItem config = Information.Instance[info.TemplateId];
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			Refers refers = obj.GetComponent<Refers>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, this.GetNameString(result, result.ValueList[1]), LocalStringManager.Get(LanguageKey.LK_GetItem_Information), ": ");
			this.BuildString(builder, this._itemColor, InformationInfo.Instance[config.InfoIds[(int)info.Level]].Name);
			refers.CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			refers.CGet<InformationCardItem>("InformationTemplate").Set(new InformationSortAndFilterData
			{
				Level = info.Level,
				TemplateId = info.TemplateId,
				UsedCount = 0,
				UsedCountMax = (sbyte)GlobalConfig.Instance.NormalInformationDefaultCostableMaxUseCount
			}, true);
		}

		// Token: 0x06007E81 RID: 32385 RVA: 0x003AD088 File Offset: 0x003AB288
		private void UpdateSecretInfoDisplay(GameObject obj, EventLogResultData result)
		{
			int metaDataId = result.ValueList[result.ValueList.Count - 1];
			string secretInfoName = "";
			bool flag = this._eventLogData != null;
			if (flag)
			{
				IEnumerable<SecretInformationDisplayData> secretInformationList = this._eventLogData.SecretInformationList;
				Func<SecretInformationDisplayData, bool> <>9__0;
				Func<SecretInformationDisplayData, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((SecretInformationDisplayData secretInfo) => (int)secretInfo.SecretInformationId == metaDataId));
				}
				using (IEnumerator<SecretInformationDisplayData> enumerator = secretInformationList.Where(predicate).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						SecretInformationDisplayData secretInfo2 = enumerator.Current;
						secretInfoName = SecretInformation.Instance[secretInfo2.SecretInformationTemplateId].Name;
						SecretInformationDisplayPackage package = new SecretInformationDisplayPackage();
						package.SecretInformationDisplayDataList.Add(secretInfo2);
						for (int i = 2; i < result.ValueList.Count - 1; i++)
						{
							foreach (CharacterDisplayData displayData in this._eventLogData.CharacterList)
							{
								bool flag2 = displayData.CharacterId == result.ValueList[i];
								if (flag2)
								{
									package.CharacterData[result.ValueList[i]] = displayData;
									break;
								}
							}
						}
						InformationUtils.RefreshSecretInformationView(obj.GetComponent<Refers>().CGet<Refers>("SecretInformationTemplate"), secretInfo2, package);
					}
				}
			}
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, this.GetNameString(result, result.ValueList[1]), LocalStringManager.Get(LanguageKey.LK_GetItem_SelectInformation), ": ");
			this.BuildString(builder, this._itemColor, secretInfoName);
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
		}

		// Token: 0x06007E82 RID: 32386 RVA: 0x003AD2BC File Offset: 0x003AB4BC
		private GameObject UpdateInfectionDisplay(EventLogResultData result)
		{
			GameObject obj = this._statusPool.GetObject();
			this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, false, "ui9_icon_winning_status_0", null, true);
			return obj;
		}

		// Token: 0x06007E83 RID: 32387 RVA: 0x003AD334 File Offset: 0x003AB534
		private GameObject UpdateInfectionStatusDisplay(EventLogResultData result)
		{
			GameObject obj = this._statusPool.GetObject();
			int status = result.ValueList[3];
			Refers component = obj.GetComponent<Refers>();
			string nameString = this.GetNameString(result, result.ValueList[1]);
			bool isPositive = status == 209;
			if (!true)
			{
			}
			string specialSuffix;
			switch (status)
			{
			case 209:
				specialSuffix = LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Cured);
				break;
			case 210:
				specialSuffix = LocalStringManager.Get(LanguageKey.LK_EventLog_Result_PartiallyInfected);
				break;
			case 211:
				specialSuffix = LocalStringManager.Get(LanguageKey.LK_EventLog_Result_CompletelyInfected);
				break;
			default:
				specialSuffix = "";
				break;
			}
			if (!true)
			{
			}
			this.RefreshStatusDisplayData(component, nameString, "", isPositive, false, "ui9_icon_winning_status_0", specialSuffix, false);
			return obj;
		}

		// Token: 0x06007E84 RID: 32388 RVA: 0x003AD3F8 File Offset: 0x003AB5F8
		private void UpdateCombatResultDisplay(GameObject obj, EventLogResultData result)
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			StringBuilder builder2 = builder;
			string negativeColor = this._negativeColor;
			sbyte type = result.Type;
			if (!true)
			{
			}
			LanguageKey id;
			switch (type)
			{
			case 8:
				id = LanguageKey.LK_EventLog_StartCombat;
				break;
			case 9:
				id = LanguageKey.LK_EventLog_StartLifeCombat;
				break;
			case 10:
				id = LanguageKey.LK_EventLog_StartCricketCombat;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			this.BuildString(builder2, negativeColor, LocalStringManager.Get(id));
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			obj.GetComponent<Refers>().CGet<CImage>("Result").SetSprite((result.ValueList[1] == 1) ? "ui9_icon_winning_status_0" : "ui9_icon_winning_status_1", true, null);
		}

		// Token: 0x06007E85 RID: 32389 RVA: 0x003AD4C4 File Offset: 0x003AB6C4
		private void UpdateItemDisplay(GameObject obj, EventLogResultData result)
		{
			EventLogData eventLogData = this._eventLogData;
			ItemDisplayData data = (eventLogData != null) ? eventLogData.ItemList.FirstOrDefault((ItemDisplayData displayData) => displayData.Key.Id == result.ValueList[2]) : null;
			bool flag = data == null;
			if (!flag)
			{
				string itemName = ItemTemplateHelper.GetName(data.Key.ItemType, data.Key.TemplateId);
				sbyte itemGrade = ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId);
				string charName = this.GetNameString(result, result.ValueList[1]);
				Refers refers = obj.GetComponent<Refers>();
				StringBuilder builder = EasyPool.Get<StringBuilder>();
				builder.Clear();
				this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(result.IsLosing ? LanguageKey.LK_EventLog_Result_Lose : LanguageKey.LK_EventLog_Result_Gain), ": ");
				this.BuildString(builder, Colors.Instance.GradeColors[(int)itemGrade].ColorToHexString("#"), itemName);
				refers.CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
				ItemBack itemView = refers.CGet<ItemBack>("ItemBack");
				data.Amount = result.ValueList[3];
				itemView.Set(data, false);
				bool flag2 = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
				if (flag2)
				{
					RowItemLine.SetResourceTip(data, refers.CGet<TooltipInvoker>("Tips"), SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName, true, true);
				}
				else
				{
					RowItemLine.SetMouseTipDisplayer(true, data, refers.CGet<TooltipInvoker>("Tips"));
				}
			}
		}

		// Token: 0x06007E86 RID: 32390 RVA: 0x003AD67C File Offset: 0x003AB87C
		private void UpdateResourceDisplay(GameObject obj, EventLogResultData result)
		{
			string charName = this.GetNameString(result, result.ValueList[1]);
			int resourceOrig = result.ValueList[2];
			int resourceNew = result.ValueList[3];
			int resourceType = result.ValueList[4];
			bool isAdd = resourceNew - resourceOrig > 0;
			ResourceTypeItem config = Config.ResourceType.Instance[resourceType];
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(isAdd ? LanguageKey.LK_EventLog_Result_Gain : LanguageKey.LK_EventLog_Result_Lose), ": ");
			this.BuildString(builder, this._itemColor, config.Name);
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			Refers view = obj.GetComponent<Refers>().CGet<Refers>("CommonParameterHorizontal");
			view.CGet<TextMeshProUGUI>("ResourceOldTitle").text = config.Name;
			view.CGet<TextMeshProUGUI>("ResourceNewTitle").text = config.Name;
			view.CGet<CImage>("ResourceOldIcon").SetSprite(config.Icon, true, null);
			view.CGet<CImage>("ResourceNewIcon").SetSprite(config.Icon, true, null);
			view.CGet<TextMeshProUGUI>("ResourceOldValue").text = resourceOrig.ToString();
			view.CGet<TextMeshProUGUI>("ResourceNewValue").text = resourceNew.ToString();
		}

		// Token: 0x06007E87 RID: 32391 RVA: 0x003AD7F8 File Offset: 0x003AB9F8
		private GameObject UpdateSpiritualDebtDisplay(EventLogResultData result)
		{
			GameObject obj = this._singleValuePool.GetObject();
			string areaName = SingletonObject.getInstance<WorldMapModel>().Areas[result.ValueList[4]].GetConfig().Name;
			this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), areaName, result.ValueList[2].ToString(), result.ValueList[3].ToString(), result.Type, "sp_icon_enyi", null);
			return obj;
		}

		// Token: 0x06007E88 RID: 32392 RVA: 0x003AD888 File Offset: 0x003ABA88
		private void UpdateTeammateDisplay(GameObject obj, EventLogResultData result)
		{
			string charName = this.GetNameString(result, result.ValueList[1]);
			string objectName = this.GetNameString(result, result.ValueList[2]);
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(result.IsLosing ? LanguageKey.LK_EventLog_Result_LoseTeammate : LanguageKey.LK_EventLog_Result_GainTeammate), ": ");
			this.BuildString(builder, this._itemColor, objectName);
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			this.RefreshSelectableCharacterDisplayData(obj.GetComponent<Refers>().CGet<AvatarNormalWithName>("Actor"), result.ValueList[2]);
		}

		// Token: 0x06007E89 RID: 32393 RVA: 0x003AD94C File Offset: 0x003ABB4C
		private void UpdateExpDisplay(GameObject obj, EventLogResultData result)
		{
			string charName = this.GetNameString(result, result.ValueList[1]);
			int resourceOrig = result.ValueList[2];
			int resourceNew = result.ValueList[3];
			bool isAdd = resourceNew - resourceOrig > 0;
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(isAdd ? LanguageKey.LK_EventLog_Result_Gain : LanguageKey.LK_EventLog_Result_Lose), ": ");
			this.BuildString(builder, this._itemColor, this.GetTypeString(result.Type, null));
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			Refers view = obj.GetComponent<Refers>().CGet<Refers>("CommonParameterHorizontal");
			view.CGet<TextMeshProUGUI>("ResourceOldTitle").text = this.GetTypeString(result.Type, null);
			view.CGet<TextMeshProUGUI>("ResourceNewTitle").text = this.GetTypeString(result.Type, null);
			view.CGet<CImage>("ResourceOldIcon").SetSprite("sp_icon_lilianyuan", true, null);
			view.CGet<CImage>("ResourceNewIcon").SetSprite("sp_icon_lilianyuan", true, null);
			view.CGet<TextMeshProUGUI>("ResourceOldValue").text = resourceOrig.ToString();
			view.CGet<TextMeshProUGUI>("ResourceNewValue").text = resourceNew.ToString();
		}

		// Token: 0x06007E8A RID: 32394 RVA: 0x003ADAD4 File Offset: 0x003ABCD4
		private GameObject UpdateHealthDisplay(EventLogResultData result)
		{
			GameObject obj = this._statusPool.GetObject();
			this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, false, null, null, false);
			return obj;
		}

		// Token: 0x06007E8B RID: 32395 RVA: 0x003ADB48 File Offset: 0x003ABD48
		private GameObject UpdateMainAttributeDisplay(EventLogResultData result)
		{
			GameObject obj = this._singleValuePool.GetObject();
			Refers component = obj.GetComponent<Refers>();
			string nameString = this.GetNameString(result, result.ValueList[1]);
			string originalValue = result.ValueList[2].ToString();
			string newValue = (result.ValueList[2] + result.ValueList[3]).ToString();
			sbyte type = result.Type;
			int num = result.ValueList[4];
			if (!true)
			{
			}
			string image;
			switch (num)
			{
			case 0:
				image = "sp_icon_attribute_0";
				break;
			case 1:
				image = "sp_icon_attribute_1";
				break;
			case 2:
				image = "sp_icon_attribute_2";
				break;
			case 3:
				image = "sp_icon_attribute_3";
				break;
			case 4:
				image = "sp_icon_attribute_4";
				break;
			case 5:
				image = "sp_icon_attribute_5";
				break;
			default:
				image = "";
				break;
			}
			if (!true)
			{
			}
			this.RefreshSingleValueDisplayData(component, nameString, originalValue, newValue, type, image, new int?(result.ValueList[4]));
			return obj;
		}

		// Token: 0x06007E8C RID: 32396 RVA: 0x003ADC60 File Offset: 0x003ABE60
		private GameObject UpdateInjuryDisplay(EventLogResultData result)
		{
			GameObject obj = this._singleValuePool.GetObject();
			this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), result.ValueList[2].ToString(), (result.ValueList[2] + result.ValueList[3]).ToString(), result.Type, (result.Type == 17) ? "sp_icon_neiwaishang_1" : "sp_icon_neiwaishang_0", new int?(result.ValueList[4]));
			return obj;
		}

		// Token: 0x06007E8D RID: 32397 RVA: 0x003ADD04 File Offset: 0x003ABF04
		private GameObject UpdatePoisonDisplay(EventLogResultData result)
		{
			GameObject obj = this._singleValuePool.GetObject();
			this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), result.ValueList[2].ToString(), (result.ValueList[2] + result.ValueList[3]).ToString(), result.Type, "sp_icon_neiwaishang_2", new int?(result.ValueList[4]));
			return obj;
		}

		// Token: 0x06007E8E RID: 32398 RVA: 0x003ADD94 File Offset: 0x003ABF94
		private GameObject UpdateDisorderOfQiDisplay(EventLogResultData result)
		{
			GameObject obj = this._singleValuePool.GetObject();
			this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), (result.ValueList[2] / 10).ToString(), (result.ValueList[3] / 10).ToString(), result.Type, "sp_disorderofqi_0", null);
			return obj;
		}

		// Token: 0x06007E8F RID: 32399 RVA: 0x003ADE18 File Offset: 0x003AC018
		private void UpdateCombatSkillDisplay(GameObject obj, EventLogResultData result)
		{
			int templateId = result.ValueList[2];
			EventLogData eventLogData = this._eventLogData;
			CombatSkillDisplayData skillData = (eventLogData != null) ? eventLogData.CombatSkillList.Find((CombatSkillDisplayData d) => (int)d.TemplateId == templateId) : null;
			bool flag = skillData == null;
			if (!flag)
			{
				string charName = this.GetNameString(result, result.ValueList[1]);
				CombatSkillItem config = CombatSkill.Instance[templateId];
				StringBuilder builder = EasyPool.Get<StringBuilder>();
				Refers refers = obj.GetComponent<Refers>();
				builder.Clear();
				this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(LanguageKey.LK_GetItem_LearnCombatSkill), ": ");
				this.BuildString(builder, Colors.Instance.GradeColors[(int)config.Grade].ColorToHexString("#"), config.Name);
				refers.CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
				refers.CGet<CharacterMenuCombatSkillItem>("CommonCombatSkill").Set(skillData);
			}
		}

		// Token: 0x06007E90 RID: 32400 RVA: 0x003ADF2C File Offset: 0x003AC12C
		private void UpdateLifeSkillDisplay(GameObject obj, EventLogResultData result)
		{
			string charName = this.GetNameString(result, result.ValueList[1]);
			Config.LifeSkillItem configData = LifeSkill.Instance[result.ValueList[2]];
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			Refers lifeSkillView = obj.GetComponent<Refers>().CGet<Refers>("LifeSkillView");
			Refers refers = obj.GetComponent<Refers>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(LanguageKey.LK_GetItem_LearnLifeSkill), ": ");
			refers.CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			builder.Clear();
			this.BuildString(builder, Colors.Instance.GradeColors[(int)configData.Grade].ColorToHexString("#"), configData.Name);
			refers.CGet<TextMeshProUGUI>("Subtitle").text = builder.ToString().ColorReplace();
			refers.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
			refers.CGet<TextMeshProUGUI>("Grade").text = ItemView.GetGradeText(configData.Grade);
			lifeSkillView.CGet<TextMeshProUGUI>("Name").text = configData.Name.SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
			lifeSkillView.CGet<CImage>("GradeBack").gameObject.SetActive(true);
			lifeSkillView.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
			lifeSkillView.CGet<TextMeshProUGUI>("Grade").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
			lifeSkillView.CGet<CImage>("SkillType0").SetSprite(string.Format("sp_building_jiyituan_{0}", configData.Type), false, null);
			lifeSkillView.CGet<CImage>("SkillType1").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", configData.Type), false, null);
			lifeSkillView.CGet<TextMeshProUGUI>("QualificationType").text = Config.LifeSkillType.Instance[configData.Type].Name;
			lifeSkillView.CGet<TextMeshProUGUI>("QualificationAdd1").text = "";
			lifeSkillView.CGet<TextMeshProUGUI>("QualificationAdd2").text = "";
		}

		// Token: 0x06007E91 RID: 32401 RVA: 0x003AE188 File Offset: 0x003AC388
		private GameObject UpdateApprovedTaiwuDisplay(EventLogResultData result)
		{
			GameObject obj = this._singleValuePool.GetObject();
			this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), Config.Organization.Instance[result.ValueList[4]].Name, result.ValueList[2].ToString(), result.ValueList[3].ToString(), result.Type, "sp_icon_zhichi", null);
			return obj;
		}

		// Token: 0x06007E92 RID: 32402 RVA: 0x003AE20C File Offset: 0x003AC40C
		private void UpdateRelationDisplay(GameObject obj, EventLogResultData result)
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			this.BuildString(builder, this._nameColor, LocalStringManager.Get(result.IsLosing ? LanguageKey.LK_EventLog_Result_LoseRelation : LanguageKey.LK_EventLog_Result_GainRelation), ": ");
			this.BuildString(builder, this._itemColor, LocalStringManager.Get(this._relationTypeSortedDisplayKeyList[result.ValueList[3]]));
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			switch (result.ValueList[4])
			{
			case 0:
				obj.GetComponent<Refers>().CGet<CImage>("Result").SetSprite(result.IsLosing ? "ui9_icon_winning_status_1" : "ui9_icon_winning_status_0", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("Result").gameObject.SetActive(true);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").gameObject.SetActive(false);
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").gameObject.SetActive(false);
				break;
			case 1:
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").SetSprite("ui9_icon_arrow_ir", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").SetSprite("ui9_icon_arrow_nl", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("Result").gameObject.SetActive(false);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").gameObject.SetActive(true);
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").gameObject.SetActive(true);
				break;
			case 2:
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").SetSprite("ui9_icon_arrow_nr", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").SetSprite("ui9_icon_arrow_dl", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("Result").gameObject.SetActive(false);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").gameObject.SetActive(true);
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").gameObject.SetActive(true);
				break;
			default:
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").SetSprite("ui9_icon_arrow_ir", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").SetSprite("ui9_icon_arrow_dl", true, null);
				obj.GetComponent<Refers>().CGet<CImage>("Result").gameObject.SetActive(false);
				obj.GetComponent<Refers>().CGet<CImage>("LeftArrow").gameObject.SetActive(true);
				obj.GetComponent<Refers>().CGet<CImage>("RightArrow").gameObject.SetActive(true);
				break;
			}
			this.RefreshSelectableCharacterDisplayData(obj.GetComponent<Refers>().CGet<AvatarNormalWithName>("LeftActor"), result.ValueList[1]);
			this.RefreshSelectableCharacterDisplayData(obj.GetComponent<Refers>().CGet<AvatarNormalWithName>("RightActor"), result.ValueList[2]);
		}

		// Token: 0x06007E93 RID: 32403 RVA: 0x003AE548 File Offset: 0x003AC748
		private void UpdateFeatureDisplay(GameObject obj, EventLogResultData result)
		{
			CharacterFeatureItem config = CharacterFeature.Instance[result.ValueList[2]];
			string objectName = config.Name;
			int charId = result.ValueList[1];
			LanguageKey id = result.IsLosing ? LanguageKey.LK_EventLog_LoseFeature : LanguageKey.LK_EventLog_GainFeature;
			string nameString = this.GetNameString(result, charId);
			string text = this._nameColor;
			object arg = nameString.SetColor(text.Substring(1, text.Length - 1));
			object arg2 = LocalStringManager.Get(LanguageKey.LK_Feature);
			string str = objectName;
			text = this._itemColor;
			string title = LocalStringManager.GetFormat(id, arg, arg2, str.SetColor(text.Substring(1, text.Length - 1)));
			obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = title.ColorReplace();
			Feature characterFeatureView = obj.GetComponent<Refers>().CGet<Feature>("CharacterFeatureView");
			characterFeatureView.Set(config.TemplateId, charId, charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, -1);
		}

		// Token: 0x06007E94 RID: 32404 RVA: 0x003AE638 File Offset: 0x003AC838
		private void UpdateProfessionDisplay(GameObject obj, EventLogResultData result)
		{
			ProfessionItem config = Profession.Instance[result.ValueList[4]];
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			Refers refers = obj.GetComponent<Refers>();
			int value = result.ValueList[3] - result.ValueList[2];
			builder.Clear();
			this.BuildString(builder, this._nameColor, this.GetNameString(result, result.ValueList[1]), LocalStringManager.Get((value > 0) ? LanguageKey.LK_EventLog_Result_Gain : LanguageKey.LK_EventLog_Result_Lose), LocalStringManager.Get(LanguageKey.LK_EventLog_ProfessionSeniority), ": ");
			this.BuildString(builder, this._itemColor, config.Name);
			bool flag = value > 0;
			if (flag)
			{
				this.BuildString(builder, this._positiveColor, "+", value.ToString());
			}
			else
			{
				this.BuildString(builder, this._negativeColor, "-", (-value).ToString());
			}
			refers.CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
			refers.CGet<TextMeshProUGUI>("ProfessionName").text = config.Name;
			ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/Profession", config.Texture), delegate(Texture2D tex)
			{
				obj.GetComponent<Refers>().CGet<CRawImage>("Thumbnail").texture = tex;
			}, null, false);
		}

		// Token: 0x06007E95 RID: 32405 RVA: 0x003AE798 File Offset: 0x003AC998
		private GameObject UpdateFavorabilityToTaiwuDisplay(EventLogResultData result)
		{
			GameObject obj = this._statusPool.GetObject();
			this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[2] > 0, true, "ui_mousetip_favor_big_0", null, false);
			return obj;
		}

		// Token: 0x06007E96 RID: 32406 RVA: 0x003AE804 File Offset: 0x003ACA04
		private float CalculateTextHeights(EventLogResultData result, Refers refers)
		{
			TextMeshProUGUI text = refers.CGet<TextMeshProUGUI>("MainContent");
			text.text = result.Text.ColorReplace();
			UIRectSizeController controller = text.GetComponent<UIRectSizeController>();
			bool flag = controller != null;
			float result2;
			if (flag)
			{
				result2 = Math.Max(text.GetPreferredValues().y, text.GetComponent<LayoutElement>().minHeight) + 15f + controller.ControlList[0].SizeOffset.y;
			}
			else
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(refers.RectTransform);
				HorizontalLayoutGroup layout = refers.GetComponent<HorizontalLayoutGroup>();
				refers.RectTransform.sizeDelta = refers.RectTransform.sizeDelta.SetY(layout.preferredHeight);
				float ret = layout.preferredHeight + 15f;
				LayoutElement element = refers.GetComponent<LayoutElement>();
				bool flag2 = element != null;
				if (flag2)
				{
					ret = Math.Max(ret, element.minHeight);
				}
				result2 = ret;
			}
			return result2;
		}

		// Token: 0x06007E97 RID: 32407 RVA: 0x003AE8E7 File Offset: 0x003ACAE7
		private void SetLastIndex()
		{
			this.loopScrollView.RefillCellsFromEnd(0, false);
		}

		// Token: 0x0400606D RID: 24685
		private IAsyncMethodRequestHandler _parent;

		// Token: 0x0400606E RID: 24686
		private const int ContainerWidth = 1360;

		// Token: 0x0400606F RID: 24687
		private const int ContainerHeightIncrement = 15;

		// Token: 0x04006070 RID: 24688
		[SerializeField]
		private int ArrowOffset = 700;

		// Token: 0x04006071 RID: 24689
		private const string NpcAvatarTexturePath = "RemakeResources/Textures/NpcFace/SmallFace/";

		// Token: 0x04006072 RID: 24690
		private readonly string _npcAvatarTexturePath = "NpcFace/SmallFace";

		// Token: 0x04006073 RID: 24691
		private const string IconWin = "ui9_icon_winning_status_0";

		// Token: 0x04006074 RID: 24692
		private const string IconLose = "ui9_icon_winning_status_1";

		// Token: 0x04006075 RID: 24693
		private EventLogResultData[] _filteredResultList;

		// Token: 0x04006076 RID: 24694
		private readonly List<LanguageKey> _relationTypeSortedDisplayKeyList = new List<LanguageKey>
		{
			LanguageKey.EventEditor_Error_DuplicateGroupKey,
			LanguageKey.LK_RelationShip_Parent,
			LanguageKey.LK_RelationShip_Child,
			LanguageKey.LK_RelationShip_Bro,
			LanguageKey.LK_RelationShip_Parent,
			LanguageKey.LK_RelationShip_Child,
			LanguageKey.LK_RelationShip_Bro,
			LanguageKey.LK_RelationShip_Parent,
			LanguageKey.LK_RelationShip_Child,
			LanguageKey.LK_RelationShip_Bro,
			LanguageKey.LK_RelationShip_SwornBro,
			LanguageKey.LK_RelationShip_Wife,
			LanguageKey.LK_RelationShip_Mentor,
			LanguageKey.LK_RelationShip_Mentor,
			LanguageKey.LK_RelationShip_Friend,
			LanguageKey.LK_RelationShip_Adored,
			LanguageKey.LK_RelationShip_Enemy
		};

		// Token: 0x04006077 RID: 24695
		private readonly Dictionary<sbyte, int> _constantElementHeights = new Dictionary<sbyte, int>();

		// Token: 0x04006078 RID: 24696
		private string _nameColor;

		// Token: 0x04006079 RID: 24697
		private string _itemColor;

		// Token: 0x0400607A RID: 24698
		private string _positiveColor;

		// Token: 0x0400607B RID: 24699
		private string _negativeColor;

		// Token: 0x0400607C RID: 24700
		private bool _isDisplaying;

		// Token: 0x0400607D RID: 24701
		private bool _isShowAll = true;

		// Token: 0x0400607E RID: 24702
		private EventLogData _eventLogData;

		// Token: 0x0400607F RID: 24703
		private PoolItem _arrowPool;

		// Token: 0x04006080 RID: 24704
		private PoolItem _singleValuePool;

		// Token: 0x04006081 RID: 24705
		private PoolItem _statusPool;

		// Token: 0x04006082 RID: 24706
		private PoolItem _dialogPool;

		// Token: 0x04006083 RID: 24707
		private PoolItem _dialog2Pool;

		// Token: 0x04006084 RID: 24708
		private PoolItem _dialog3Pool;

		// Token: 0x04006085 RID: 24709
		private PoolItem _dialog4Pool;

		// Token: 0x04006086 RID: 24710
		private PoolItem _combatResultPool;

		// Token: 0x04006087 RID: 24711
		private PoolItem _itemPool;

		// Token: 0x04006088 RID: 24712
		private PoolItem _resourcePool;

		// Token: 0x04006089 RID: 24713
		private PoolItem _teammatePool;

		// Token: 0x0400608A RID: 24714
		private PoolItem _combatSkillPool;

		// Token: 0x0400608B RID: 24715
		private PoolItem _lifeSkillPool;

		// Token: 0x0400608C RID: 24716
		private PoolItem _relationPool;

		// Token: 0x0400608D RID: 24717
		private PoolItem _featurePool;

		// Token: 0x0400608E RID: 24718
		private PoolItem _professionPool;

		// Token: 0x0400608F RID: 24719
		private PoolItem _normalInformationPool;

		// Token: 0x04006090 RID: 24720
		private PoolItem _secretInformationPool;

		// Token: 0x04006091 RID: 24721
		private PoolItem _endingPool;

		// Token: 0x04006092 RID: 24722
		private bool _awakeCalled;

		// Token: 0x04006093 RID: 24723
		private bool _hasData;

		// Token: 0x04006094 RID: 24724
		[SerializeField]
		private GameObject lineContainer;

		// Token: 0x04006095 RID: 24725
		[SerializeField]
		private GameObject arrow;

		// Token: 0x04006096 RID: 24726
		[SerializeField]
		private GameObject singleValueComponent;

		// Token: 0x04006097 RID: 24727
		[SerializeField]
		private GameObject statusComponent;

		// Token: 0x04006098 RID: 24728
		[SerializeField]
		private GameObject dialogComponent;

		// Token: 0x04006099 RID: 24729
		[SerializeField]
		private GameObject dialogComponent2;

		// Token: 0x0400609A RID: 24730
		[SerializeField]
		private GameObject dialogComponent3;

		// Token: 0x0400609B RID: 24731
		[SerializeField]
		private GameObject dialogComponent4;

		// Token: 0x0400609C RID: 24732
		[SerializeField]
		private GameObject combatResultComponent;

		// Token: 0x0400609D RID: 24733
		[SerializeField]
		private GameObject itemComponent;

		// Token: 0x0400609E RID: 24734
		[SerializeField]
		private GameObject resourceComponent;

		// Token: 0x0400609F RID: 24735
		[SerializeField]
		private GameObject teammateComponent;

		// Token: 0x040060A0 RID: 24736
		[SerializeField]
		private GameObject combatSkillComponent;

		// Token: 0x040060A1 RID: 24737
		[SerializeField]
		private GameObject lifeSkillComponent;

		// Token: 0x040060A2 RID: 24738
		[SerializeField]
		private GameObject relationComponent;

		// Token: 0x040060A3 RID: 24739
		[SerializeField]
		private GameObject featureComponent;

		// Token: 0x040060A4 RID: 24740
		[SerializeField]
		private GameObject professionComponent;

		// Token: 0x040060A5 RID: 24741
		[SerializeField]
		private GameObject informationComponent;

		// Token: 0x040060A6 RID: 24742
		[SerializeField]
		private GameObject secretInformationComponent;

		// Token: 0x040060A7 RID: 24743
		[SerializeField]
		private GameObject endingComponent;

		// Token: 0x040060A8 RID: 24744
		[SerializeField]
		private LoopVerticalScrollRect loopScrollView;

		// Token: 0x040060A9 RID: 24745
		[SerializeField]
		private CToggleGroup switchButton;

		// Token: 0x040060AA RID: 24746
		[SerializeField]
		private GameObject noContent;

		// Token: 0x040060AB RID: 24747
		[SerializeField]
		private Vector2 imageSizeDelta = new Vector2(30f, 30f);
	}
}
