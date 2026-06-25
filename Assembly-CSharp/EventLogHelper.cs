using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.TaiwuEvent.EventLog;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000348 RID: 840
public class EventLogHelper : Refers
{
	// Token: 0x0600310E RID: 12558 RVA: 0x00180866 File Offset: 0x0017EA66
	public void Init(EventLogData data)
	{
		this._isDisplaying = true;
		this._eventLogData = data;
		this._lastIndex = -1;
		this.InitContent();
		this.RefreshContent();
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x0018088C File Offset: 0x0017EA8C
	private void Awake()
	{
		this._nameColor = Colors.Instance["pinkyellow"].ColorToHexString("#");
		this._itemColor = Colors.Instance["grey"].ColorToHexString("#");
		this._positiveColor = Colors.Instance["brightblue"].ColorToHexString("#");
		this._negativeColor = Colors.Instance["brightred"].ColorToHexString("#");
		this._switchButton = base.CGet<SwitchButton>("SwitchButton");
		this._switchButton.InitSwitchButton(LanguageKey.LK_EventLog_ShowDialogOnly.Tr(), LanguageKey.LK_EventLog_ShowAll.Tr(), new UnityAction<bool>(this.OnSwitch));
		this._containerPool = new PoolItem("UI_EventLog_LineContainer", base.CGet<GameObject>("LineContainer"));
		this._arrowPool = new PoolItem("UI_EventLog_ArrowPrefab", base.CGet<GameObject>("Arrow"));
		this._singleValuePool = new PoolItem("UI_EventLog_SingleValuePrefab", base.CGet<GameObject>("SingleValueComponent"));
		this._statusPool = new PoolItem("UI_EventLog_StatusPrefab", base.CGet<GameObject>("StatusComponent"));
		this._dialogPool = new PoolItem("UI_EventLog_DialogPrefab", base.CGet<GameObject>("DialogComponent"));
		this._dialog2Pool = new PoolItem("UI_EventLog_Dialog2Prefab", base.CGet<GameObject>("DialogComponent2"));
		this._dialog3Pool = new PoolItem("UI_EventLog_Dialog3Prefab", base.CGet<GameObject>("DialogComponent3"));
		this._dialog4Pool = new PoolItem("UI_EventLog_Dialog4Prefab", base.CGet<GameObject>("DialogComponent4"));
		this._combatResultPool = new PoolItem("UI_EventLog_CombatResultPrefab", base.CGet<GameObject>("CombatResultComponent"));
		this._itemPool = new PoolItem("UI_EventLog_ItemPrefab", base.CGet<GameObject>("ItemComponent"));
		this._resourcePool = new PoolItem("UI_EventLog_ResourcePrefab", base.CGet<GameObject>("ResourceComponent"));
		this._teammatePool = new PoolItem("UI_EventLog_TeammatePrefab", base.CGet<GameObject>("TeammateComponent"));
		this._combatSkillPool = new PoolItem("UI_EventLog_CombatSkillPrefab", base.CGet<GameObject>("CombatSkillComponent"));
		this._lifeSkillPool = new PoolItem("UI_EventLog_LifeSkillPrefab", base.CGet<GameObject>("LifeSkillComponent"));
		this._relationPool = new PoolItem("UI_EventLog_RelationPrefab", base.CGet<GameObject>("RelationComponent"));
		this._featurePool = new PoolItem("UI_EventLog_FeaturePrefab", base.CGet<GameObject>("FeatureComponent"));
		this._professionPool = new PoolItem("UI_EventLog_ProfessionPrefab", base.CGet<GameObject>("ProfessionComponent"));
		this._normalInformationPool = new PoolItem("UI_EventLog_NormalInformationPrefab", base.CGet<GameObject>("InformationComponent"));
		this._secretInformationPool = new PoolItem("UI_EventLog_SecretInformationPrefab", base.CGet<GameObject>("SecretInformationComponent"));
		this._endingPool = new PoolItem("UI_EventLog_EndingPrefab", base.CGet<GameObject>("EndingComponent"));
		this._scrollView = base.CGet<CScrollRectLegacy>("VerticalScrollView");
		this._content = base.CGet<GameObject>("Content").transform;
		this._contentRect = this._content.GetComponent<RectTransform>();
		int statusHeight = (int)base.CGet<GameObject>("StatusComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int combatResultHeight = (int)base.CGet<GameObject>("CombatResultComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int itemHeight = (int)base.CGet<GameObject>("ItemComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int resourceHeight = (int)base.CGet<GameObject>("ResourceComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int singleValueHeight = (int)base.CGet<GameObject>("SingleValueComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int teammateHeight = (int)base.CGet<GameObject>("TeammateComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int combatSkillHeight = (int)base.CGet<GameObject>("CombatSkillComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int lifeSkillHeight = (int)base.CGet<GameObject>("LifeSkillComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int relationHeight = (int)base.CGet<GameObject>("RelationComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int featureHeight = (int)base.CGet<GameObject>("FeatureComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int professionHeight = (int)base.CGet<GameObject>("ProfessionComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int informationHeight = (int)base.CGet<GameObject>("InformationComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int secretInformationHeight = (int)base.CGet<GameObject>("SecretInformationComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
		int endingHeight = (int)base.CGet<GameObject>("EndingComponent").GetComponent<RectTransform>().sizeDelta.y + 15;
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
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x00180F11 File Offset: 0x0017F111
	private void OnEnable()
	{
		this._switchButton.SetStatus(this._isShowAll);
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x00180F28 File Offset: 0x0017F128
	private void OnDisable()
	{
		for (int i = 0; i < this._content.childCount; i++)
		{
			this.ReturnLineContainer(this._content.GetChild(i));
		}
		this._isDisplaying = false;
		this._eventLogData = null;
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x00180F74 File Offset: 0x0017F174
	private void OnDestroy()
	{
		PoolItem containerPool = this._containerPool;
		if (containerPool != null)
		{
			containerPool.Destroy();
		}
		this._containerPool = null;
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

	// Token: 0x06003113 RID: 12563 RVA: 0x00181178 File Offset: 0x0017F378
	private void OnSwitch(bool isOn)
	{
		this._isShowAll = isOn;
		this.SetLastIndex();
		bool isDisplaying = this._isDisplaying;
		if (isDisplaying)
		{
			this.RefreshContent();
		}
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x001811A8 File Offset: 0x0017F3A8
	private void ReturnLineContainer(Transform lineContainer)
	{
		for (int i = 0; i < lineContainer.childCount; i++)
		{
			GameObject component = lineContainer.GetChild(i).gameObject;
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
		this._containerPool.DestroyObject(lineContainer.gameObject);
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x001814D4 File Offset: 0x0017F6D4
	private float RenderLineContainer(EventLogResultData result)
	{
		GameObject lineContainer = this._containerPool.GetObject();
		lineContainer.transform.SetParent(this._content, false);
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
			lineContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(1360f, height);
			lineContainer.GetComponent<LayoutElement>().preferredHeight = height;
			obj.transform.SetParent(lineContainer.transform, false);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.SetAsFirstSibling();
			this.AddArrow(lineContainer, result);
			result2 = height;
		}
		return result2;
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x00181890 File Offset: 0x0017FA90
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

	// Token: 0x06003117 RID: 12567 RVA: 0x0018191C File Offset: 0x0017FB1C
	private void InitContent()
	{
		this._eventLogHeight.Clear();
		foreach (EventLogResultData result in this._eventLogData.ResultList)
		{
			this._eventLogHeight.Add(this.RenderLineContainer(result));
		}
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x00181990 File Offset: 0x0017FB90
	private void RefreshContent()
	{
		RectTransform verticalScrollViewRectTrans = this._scrollView.GetComponent<RectTransform>();
		float totalHeight = 0f;
		float distanceToBottom = 0f;
		verticalScrollViewRectTrans.anchoredPosition = verticalScrollViewRectTrans.anchoredPosition.SetX(3000f);
		for (int i = 0; i < this._eventLogData.ResultList.Count; i++)
		{
			bool flag;
			if (!this._isShowAll)
			{
				sbyte type = this._eventLogData.ResultList[i].Type;
				flag = (type == 0 || type == 1 || type == 2 || type == 30);
			}
			else
			{
				flag = true;
			}
			bool show = flag;
			this._content.GetChild(i).gameObject.SetActive(show);
			bool flag2 = !show;
			if (!flag2)
			{
				float height = this._eventLogHeight[i];
				totalHeight += height;
				bool flag3 = i < this._lastIndex;
				if (flag3)
				{
					totalHeight += height;
				}
				else
				{
					distanceToBottom += height;
				}
			}
		}
		this._scrollView.ScrollBar.gameObject.SetActive(totalHeight > verticalScrollViewRectTrans.rect.height);
		this._scrollView.ScrollBar.size = 1f / Math.Min(10f, (float)this._eventLogData.ResultList.Count);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			bool flag4 = this._lastIndex >= 0 && distanceToBottom > verticalScrollViewRectTrans.rect.height;
			if (flag4)
			{
				this._contentRect.anchoredPosition = this._contentRect.anchoredPosition.SetY(-(this._content.GetChild(this._lastIndex).localPosition.y + this._eventLogHeight[this._lastIndex] / 2f));
			}
			else
			{
				this._scrollView.ScrollBar.value = 1f;
			}
			this._scrollView.UpdateScrollBarValue();
			verticalScrollViewRectTrans.anchoredPosition = verticalScrollViewRectTrans.anchoredPosition.SetX(0f);
		});
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x00181B2C File Offset: 0x0017FD2C
	private void RefreshCharacterDisplayData(EventLogResultData result, string charName, int charId, GameObject root, string nameComponent, string thumbnailComponent)
	{
		bool flag = charId == -1;
		if (flag)
		{
			bool flag2 = !string.IsNullOrEmpty(nameComponent);
			if (flag2)
			{
				root.GetComponent<Refers>().CGet<CommonCharacterNameFrame>(nameComponent).SetName("");
			}
			bool flag3 = !string.IsNullOrEmpty(thumbnailComponent);
			if (flag3)
			{
				root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).SetActive(false);
			}
		}
		else
		{
			NameStringAndAvatar displayData;
			bool flag4 = result.CharDict.TryGetValue(charId, out displayData);
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
					root.GetComponent<Refers>().CGet<CommonCharacterNameFrame>(nameComponent).SetName(charName);
				}
				bool flag7 = !string.IsNullOrEmpty(thumbnailComponent);
				if (flag7)
				{
					root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(displayData.Avatar, displayData.CharTemplateId);
					root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).SetActive(true);
				}
			}
		}
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x00181C40 File Offset: 0x0017FE40
	private void RefreshMerchantDisplayData(short templateId, GameObject root, string nameComponent, string thumbnailComponent)
	{
		MerchantTypeItem merchantTypeConfig = MerchantType.Instance[Merchant.Instance[(int)templateId].MerchantType];
		string merchantName = merchantTypeConfig.Name;
		bool flag = !string.IsNullOrEmpty(nameComponent);
		if (flag)
		{
			root.GetComponent<Refers>().CGet<CommonCharacterNameFrame>(nameComponent).SetName(merchantName);
		}
		bool flag2 = !string.IsNullOrEmpty(thumbnailComponent);
		if (flag2)
		{
			ResLoader.LoadModOrGameResource<Texture2D>(this._npcAvatarTexturePath + "/" + merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh), null);
			root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).SetActive(true);
		}
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x00181CF8 File Offset: 0x0017FEF8
	private void RefreshActorDisplayData(string charName, EventActorData data, GameObject root, string nameComponent, string thumbnailComponent)
	{
		bool flag = !string.IsNullOrEmpty(nameComponent);
		if (flag)
		{
			root.GetComponent<Refers>().CGet<CommonCharacterNameFrame>(nameComponent).SetName(charName);
		}
		bool flag2 = !string.IsNullOrEmpty(thumbnailComponent);
		if (flag2)
		{
			string texture = EventActors.Instance[data.TemplateId].Texture;
			bool flag3 = !string.IsNullOrEmpty(texture);
			if (flag3)
			{
				ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/NpcFace/SmallFace/", texture), new Action<Texture2D>(root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh), null, false);
			}
			else
			{
				data.AvatarData.ClothDisplayId = data.ClothDisplayId;
				root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).GetComponent<Refers>().CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(data.AvatarData, (short)data.Age);
			}
			root.GetComponent<Refers>().CGet<GameObject>(thumbnailComponent).SetActive(true);
		}
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x00181DF4 File Offset: 0x0017FFF4
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

	// Token: 0x0600311D RID: 12573 RVA: 0x00181F64 File Offset: 0x00180164
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

	// Token: 0x0600311E RID: 12574 RVA: 0x0018205B File Offset: 0x0018025B
	private void RefreshSelectableCharacterDisplayData(CharacterToggleBright actor, int charId)
	{
		actor.CharacterId = charId;
		actor.Unknown = false;
		actor.Disable = false;
		actor.Interactable = false;
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x00182080 File Offset: 0x00180280
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

	// Token: 0x06003120 RID: 12576 RVA: 0x001822E4 File Offset: 0x001804E4
	private string GetInjuryString(bool isInnerInjury, int? bodyPartType)
	{
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		builder.Clear();
		StringBuilder builder2 = builder;
		string color = Colors.Instance[isInnerInjury ? "innerinjury" : "outterinjury"].ColorToHexString("#");
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
				goto IL_D5;
			case 1:
				text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_Belly);
				goto IL_D5;
			case 2:
				text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_Head);
				goto IL_D5;
			case 3:
				text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_LeftHand);
				goto IL_D5;
			case 4:
				text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_RightHand);
				goto IL_D5;
			case 5:
				text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_LeftFoot);
				goto IL_D5;
			case 6:
				text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_HitParts_RightFoot);
				goto IL_D5;
			}
		}
		text = "";
		IL_D5:
		if (!true)
		{
		}
		this.BuildString(builder2, color, text, LocalStringManager.Get(LanguageKey.LK_Injury));
		return builder.ToString();
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x001823EA File Offset: 0x001805EA
	private string GetNameString(EventLogResultData result, int id)
	{
		return result.CharDict.GetValueOrDefault(id).Name ?? "";
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x00182406 File Offset: 0x00180606
	private void BuildString(StringBuilder builder, string color, string text)
	{
		builder.Append("<color=");
		builder.Append(color);
		builder.Append(">");
		builder.Append(text);
		builder.Append("</color>");
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x0018243D File Offset: 0x0018063D
	private void BuildString(StringBuilder builder, string color, string text1, string text2)
	{
		builder.Append("<color=");
		builder.Append(color);
		builder.Append(">");
		builder.Append(text1);
		builder.Append(text2);
		builder.Append("</color>");
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x00182480 File Offset: 0x00180680
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

	// Token: 0x06003125 RID: 12581 RVA: 0x001824D4 File Offset: 0x001806D4
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

	// Token: 0x06003126 RID: 12582 RVA: 0x00182531 File Offset: 0x00180731
	private void SetItemSelectedCount(ItemView itemView, int count)
	{
		itemView.CGet<TextMeshProUGUI>("Count").text = string.Format("{0}/{1}", count, itemView.Data.Amount.ToString().SetColor("grey"));
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x00182570 File Offset: 0x00180770
	private void AddArrow(GameObject lineContainer, EventLogResultData result)
	{
		switch (result.Type)
		{
		case 3:
		case 4:
		case 6:
		case 7:
		case 11:
		case 12:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 25:
		case 27:
			this.CreateArrow(lineContainer, result.ValueList[1] == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			break;
		case 5:
			this.CreateArrow(lineContainer, false);
			break;
		case 8:
		case 9:
		case 10:
			this.CreateArrow(lineContainer, true);
			this.CreateArrow(lineContainer, false);
			break;
		case 13:
		case 14:
		case 23:
		case 26:
			this.CreateArrow(lineContainer, true);
			break;
		case 24:
		{
			int mcId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = mcId == result.ValueList[1] || mcId == result.ValueList[2];
			if (flag)
			{
				this.CreateArrow(lineContainer, true);
				this.CreateArrow(lineContainer, false);
			}
			break;
		}
		}
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x00182690 File Offset: 0x00180890
	private void CreateArrow(GameObject lineContainer, bool isLeft)
	{
		GameObject obj = this._arrowPool.GetObject();
		obj.transform.SetParent(lineContainer.transform, false);
		if (isLeft)
		{
			obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 180f, 0f);
			obj.GetComponent<RectTransform>().localPosition = new Vector3(-700f, 0f, 0f);
		}
		else
		{
			obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
			obj.GetComponent<RectTransform>().localPosition = new Vector3(700f, 0f, 0f);
		}
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x00182748 File Offset: 0x00180948
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

	// Token: 0x0600312A RID: 12586 RVA: 0x00182990 File Offset: 0x00180B90
	private void UpdateResponseDisplay(GameObject obj, EventLogResultData result)
	{
		this.RefreshCharacterDisplayData(result, result.LeftName, result.ValueList[1], obj, "LeftName", "LeftActor");
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("MainContent").alignment = TextAlignmentOptions.Left;
		int behaviorType = result.ValueList[2] - 1;
		bool flag = behaviorType == -1;
		if (flag)
		{
			obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").SetActive(false);
		}
		else
		{
			Refers refers = obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").GetComponent<Refers>();
			refers.CGet<CImage>("Icon").SetSprite(BehaviorType.Instance.GetItem((short)behaviorType).Icon, false, null);
			refers.CGet<TextMeshProUGUI>("InfoName").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior);
			refers.CGet<TextMeshProUGUI>("InfoValue").text = CommonUtils.GetBehaviorString((sbyte)behaviorType);
			obj.GetComponent<Refers>().CGet<GameObject>("CharacterBehavior").SetActive(true);
		}
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x00182A9C File Offset: 0x00180C9C
	private GameObject UpdateHappinessDisplay(EventLogResultData result)
	{
		GameObject obj = this._statusPool.GetObject();
		this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, true, "ui_mousetip_mood_big_0", null, false);
		return obj;
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x00182B14 File Offset: 0x00180D14
	private GameObject UpdateFameDisplay(EventLogResultData result)
	{
		GameObject obj = this._statusPool.GetObject();
		this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, true, "taiwuevent_01_history_icon_9", null, false);
		return obj;
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x00182B8C File Offset: 0x00180D8C
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
		InformationUtils.RefreshNormalInformationView(refers.CGet<Refers>("InformationTemplate"), info);
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x00182C6C File Offset: 0x00180E6C
	private void UpdateSecretInfoDisplay(GameObject obj, EventLogResultData result)
	{
		int metaDataId = result.ValueList[result.ValueList.Count - 1];
		string secretInfoName = "";
		foreach (SecretInformationDisplayData secretInfo in this._eventLogData.SecretInformationList)
		{
			bool flag = (int)secretInfo.SecretInformationId == metaDataId;
			if (flag)
			{
				secretInfoName = SecretInformation.Instance[secretInfo.SecretInformationTemplateId].Name;
				SecretInformationDisplayPackage package = new SecretInformationDisplayPackage();
				package.SecretInformationDisplayDataList.Add(secretInfo);
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
				InformationUtils.RefreshSecretInformationView(obj.GetComponent<Refers>().CGet<Refers>("SecretInformationTemplate"), secretInfo, package);
				break;
			}
		}
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		builder.Clear();
		this.BuildString(builder, this._nameColor, this.GetNameString(result, result.ValueList[1]), LocalStringManager.Get(LanguageKey.LK_GetItem_SelectInformation), ": ");
		this.BuildString(builder, this._itemColor, secretInfoName);
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x00182E78 File Offset: 0x00181078
	private GameObject UpdateInfectionDisplay(EventLogResultData result)
	{
		GameObject obj = this._statusPool.GetObject();
		this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, false, "ui_taiwuevent_recall_icon_0", null, true);
		return obj;
	}

	// Token: 0x06003130 RID: 12592 RVA: 0x00182EF0 File Offset: 0x001810F0
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
		this.RefreshStatusDisplayData(component, nameString, "", isPositive, false, "ui_taiwuevent_recall_icon_0", specialSuffix, false);
		return obj;
	}

	// Token: 0x06003131 RID: 12593 RVA: 0x00182FB4 File Offset: 0x001811B4
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
		obj.GetComponent<Refers>().CGet<CImage>("Result").SetSprite((result.ValueList[1] == 1) ? "ui_taiwuevent_recall_icon_word_0" : "ui_taiwuevent_recall_icon_word_1", true, null);
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x00183080 File Offset: 0x00181280
	private void UpdateItemDisplay(GameObject obj, EventLogResultData result)
	{
		ItemDisplayData data = null;
		foreach (ItemDisplayData displayData in this._eventLogData.ItemList)
		{
			bool flag = displayData.Key.Id == result.ValueList[2];
			if (flag)
			{
				data = displayData;
				break;
			}
		}
		bool flag2 = data == null;
		if (!flag2)
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
			CommonItemBack itemView = refers.CGet<CommonItemBack>("CommonItemBack");
			itemView.SetData(data, -1);
			itemView.SetInteractable(false);
		}
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x00183204 File Offset: 0x00181404
	private void UpdateResourceDisplay(GameObject obj, EventLogResultData result)
	{
		string charName = this.GetNameString(result, result.ValueList[1]);
		int resourceOrig = result.ValueList[2];
		int resourceNew = result.ValueList[3];
		int resourceType = result.ValueList[4];
		bool isAdd = resourceNew - resourceOrig > 0;
		ResourceTypeItem config = ResourceType.Instance[resourceType];
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		builder.Clear();
		this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(isAdd ? LanguageKey.LK_EventLog_Result_Gain : LanguageKey.LK_EventLog_Result_Lose), ": ");
		this.BuildString(builder, this._itemColor, config.Name);
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
		builder.Clear();
		builder.Append("  ");
		builder.Append(resourceOrig);
		bool flag = isAdd;
		if (flag)
		{
			this.BuildString(builder, this._positiveColor, "  +", Math.Abs(resourceNew - resourceOrig).ToString());
		}
		else
		{
			this.BuildString(builder, this._negativeColor, "  -", Math.Abs(resourceNew - resourceOrig).ToString());
		}
		Refers view = obj.GetComponent<Refers>().CGet<Refers>("CommonParameterHorizontal");
		view.CGet<TextMeshProUGUI>("Title").text = config.Name;
		view.CGet<TextMeshProUGUI>("Value").text = builder.ToString().ColorReplace();
		view.CGet<CImage>("Icon").SetSprite(config.Icon, true, null);
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x001833AC File Offset: 0x001815AC
	private GameObject UpdateSpiritualDebtDisplay(EventLogResultData result)
	{
		GameObject obj = this._singleValuePool.GetObject();
		string areaName = SingletonObject.getInstance<WorldMapModel>().Areas[result.ValueList[4]].GetConfig().Name;
		this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), areaName, result.ValueList[2].ToString(), result.ValueList[3].ToString(), result.Type, "sp_icon_enyi", null);
		return obj;
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x0018343C File Offset: 0x0018163C
	private void UpdateTeammateDisplay(GameObject obj, EventLogResultData result)
	{
		string charName = this.GetNameString(result, result.ValueList[1]);
		string objectName = this.GetNameString(result, result.ValueList[2]);
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		builder.Clear();
		this.BuildString(builder, this._nameColor, charName, LocalStringManager.Get(result.IsLosing ? LanguageKey.LK_EventLog_Result_LoseTeammate : LanguageKey.LK_EventLog_Result_GainTeammate), ": ");
		this.BuildString(builder, this._itemColor, objectName);
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
		this.RefreshSelectableCharacterDisplayData(obj.GetComponent<Refers>().CGet<CharacterToggleBright>("Actor"), result.ValueList[2]);
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x00183500 File Offset: 0x00181700
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
		builder.Clear();
		builder.Append("  ");
		builder.Append(resourceOrig);
		bool flag = isAdd;
		if (flag)
		{
			this.BuildString(builder, this._positiveColor, "  +", Math.Abs(resourceNew - resourceOrig).ToString());
		}
		else
		{
			this.BuildString(builder, this._negativeColor, "  -", Math.Abs(resourceNew - resourceOrig).ToString());
		}
		Refers view = obj.GetComponent<Refers>().CGet<Refers>("CommonParameterHorizontal");
		view.CGet<TextMeshProUGUI>("Title").text = this.GetTypeString(result.Type, null);
		view.CGet<TextMeshProUGUI>("Value").text = builder.ToString().ColorReplace();
		view.CGet<CImage>("Icon").SetSprite("sp_icon_lilianyuan", false, null);
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x001836A4 File Offset: 0x001818A4
	private GameObject UpdateHealthDisplay(EventLogResultData result)
	{
		GameObject obj = this._statusPool.GetObject();
		this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[3] - result.ValueList[2] > 0, false, null, null, false);
		return obj;
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x00183718 File Offset: 0x00181918
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

	// Token: 0x06003139 RID: 12601 RVA: 0x00183830 File Offset: 0x00181A30
	private GameObject UpdateInjuryDisplay(EventLogResultData result)
	{
		GameObject obj = this._singleValuePool.GetObject();
		this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), result.ValueList[2].ToString(), (result.ValueList[2] + result.ValueList[3]).ToString(), result.Type, (result.Type == 17) ? "sp_icon_neiwaishang_1" : "sp_icon_neiwaishang_0", new int?(result.ValueList[4]));
		return obj;
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x001838D4 File Offset: 0x00181AD4
	private GameObject UpdatePoisonDisplay(EventLogResultData result)
	{
		GameObject obj = this._singleValuePool.GetObject();
		this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), result.ValueList[2].ToString(), (result.ValueList[2] + result.ValueList[3]).ToString(), result.Type, "sp_icon_neiwaishang_2", new int?(result.ValueList[4]));
		return obj;
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x00183964 File Offset: 0x00181B64
	private GameObject UpdateDisorderOfQiDisplay(EventLogResultData result)
	{
		GameObject obj = this._singleValuePool.GetObject();
		this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), (result.ValueList[2] / 10).ToString(), (result.ValueList[3] / 10).ToString(), result.Type, "sp_disorderofqi_0", null);
		return obj;
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x001839E8 File Offset: 0x00181BE8
	private void UpdateCombatSkillDisplay(GameObject obj, EventLogResultData result)
	{
		int templateId = result.ValueList[2];
		CombatSkillDisplayData skillData = this._eventLogData.CombatSkillList.Find((CombatSkillDisplayData d) => (int)d.TemplateId == templateId);
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
			CommonCombatSkill skillView = refers.CGet<CommonCombatSkill>("CommonCombatSkill");
			skillView.Refresh(skillData);
			skillView.toggle.interactable = false;
		}
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x00183B08 File Offset: 0x00181D08
	private void UpdateLifeSkillDisplay(GameObject obj, EventLogResultData result)
	{
		string charName = this.GetNameString(result, result.ValueList[1]);
		LifeSkillItem configData = LifeSkill.Instance[result.ValueList[2]];
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
		lifeSkillView.CGet<TextMeshProUGUI>("QualificationType").text = LifeSkillType.Instance[configData.Type].Name;
		lifeSkillView.CGet<TextMeshProUGUI>("QualificationAdd1").text = "";
		lifeSkillView.CGet<TextMeshProUGUI>("QualificationAdd2").text = "";
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x00183D64 File Offset: 0x00181F64
	private GameObject UpdateApprovedTaiwuDisplay(EventLogResultData result)
	{
		GameObject obj = this._singleValuePool.GetObject();
		this.RefreshSingleValueDisplayData(obj.GetComponent<Refers>(), Organization.Instance[result.ValueList[4]].Name, result.ValueList[2].ToString(), result.ValueList[3].ToString(), result.Type, "sp_icon_zhichi", null);
		return obj;
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x00183DE8 File Offset: 0x00181FE8
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
			obj.GetComponent<Refers>().CGet<CImage>("Result").SetSprite(result.IsLosing ? "ui_taiwuevent_recall_icon_word_1" : "ui_taiwuevent_recall_icon_word_0", true, null);
			break;
		case 1:
			obj.GetComponent<Refers>().CGet<CImage>("Result").SetSprite("ui_taiwuevent_recall_icon_mark_0", true, null);
			break;
		case 2:
			obj.GetComponent<Refers>().CGet<CImage>("Result").SetSprite("ui_taiwuevent_recall_icon_mark_1", true, null);
			break;
		}
		this.RefreshSelectableCharacterDisplayData(obj.GetComponent<Refers>().CGet<CharacterToggleBright>("LeftActor"), result.ValueList[1]);
		this.RefreshSelectableCharacterDisplayData(obj.GetComponent<Refers>().CGet<CharacterToggleBright>("RightActor"), result.ValueList[2]);
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x00183F50 File Offset: 0x00182150
	private void UpdateFeatureDisplay(GameObject obj, EventLogResultData result)
	{
		CharacterFeatureItem config = CharacterFeature.Instance[result.ValueList[2]];
		string objectName = config.Name;
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		int charId = result.ValueList[1];
		builder.Clear();
		this.BuildString(builder, this._nameColor, this.GetNameString(result, charId), LocalStringManager.Get(result.IsLosing ? LanguageKey.LK_EventLog_Result_Lose : LanguageKey.LK_EventLog_Result_Gain), LocalStringManager.Get(LanguageKey.LK_Feature), ": ");
		this.BuildString(builder, this._itemColor, objectName);
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Title").text = builder.ToString().ColorReplace();
		CharacterFeatureView characterFeatureView = obj.GetComponent<Refers>().CGet<CharacterFeatureView>("CharacterFeatureView");
		characterFeatureView.Set(config, charId, false);
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x00184024 File Offset: 0x00182224
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

	// Token: 0x06003142 RID: 12610 RVA: 0x00184184 File Offset: 0x00182384
	private GameObject UpdateFavorabilityToTaiwuDisplay(EventLogResultData result)
	{
		GameObject obj = this._statusPool.GetObject();
		this.RefreshStatusDisplayData(obj.GetComponent<Refers>(), this.GetNameString(result, result.ValueList[1]), this.GetTypeString(result.Type, null), result.ValueList[2] > 0, true, "ui_mousetip_favor_big_0", null, false);
		return obj;
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x001841F0 File Offset: 0x001823F0
	private float CalculateTextHeights(EventLogResultData result, Refers refers)
	{
		TextMeshProUGUI text = refers.CGet<TextMeshProUGUI>("MainContent");
		text.text = result.Text.ColorReplace();
		return Math.Max(text.GetPreferredValues().y, text.GetComponent<LayoutElement>().minHeight) + text.GetComponent<UIRectSizeController>().ControlList[0].SizeOffset.y + 15f;
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x00184260 File Offset: 0x00182460
	private void SetLastIndex()
	{
		float minDiff = this._contentRect.rect.height;
		float contentPosition = this._contentRect.anchoredPosition.y;
		this._lastIndex = -1;
		for (int i = 0; i < this._eventLogData.ResultList.Count; i++)
		{
			sbyte type = this._eventLogData.ResultList[i].Type;
			bool flag = type == 0 || type == 1 || type == 2;
			if (flag)
			{
				float diff = Math.Abs(this._content.GetChild(i).localPosition.y + this._eventLogHeight[i] / 2f + contentPosition);
				bool flag2 = diff < minDiff;
				if (flag2)
				{
					this._lastIndex = i;
					minDiff = diff;
				}
			}
		}
	}

	// Token: 0x040023EC RID: 9196
	private const int ContainerWidth = 1360;

	// Token: 0x040023ED RID: 9197
	private const int ContainerHeightIncrement = 15;

	// Token: 0x040023EE RID: 9198
	private const int ArrowOffset = 700;

	// Token: 0x040023EF RID: 9199
	private const string NpcAvatarTexturePath = "RemakeResources/Textures/NpcFace/SmallFace/";

	// Token: 0x040023F0 RID: 9200
	private readonly string _npcAvatarTexturePath = "NpcFace/SmallFace";

	// Token: 0x040023F1 RID: 9201
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

	// Token: 0x040023F2 RID: 9202
	private readonly Dictionary<sbyte, int> _constantElementHeights = new Dictionary<sbyte, int>();

	// Token: 0x040023F3 RID: 9203
	private string _nameColor;

	// Token: 0x040023F4 RID: 9204
	private string _itemColor;

	// Token: 0x040023F5 RID: 9205
	private string _positiveColor;

	// Token: 0x040023F6 RID: 9206
	private string _negativeColor;

	// Token: 0x040023F7 RID: 9207
	private int _lastIndex;

	// Token: 0x040023F8 RID: 9208
	private bool _isDisplaying;

	// Token: 0x040023F9 RID: 9209
	private bool _isShowAll = true;

	// Token: 0x040023FA RID: 9210
	private readonly List<float> _eventLogHeight = new List<float>();

	// Token: 0x040023FB RID: 9211
	private SwitchButton _switchButton;

	// Token: 0x040023FC RID: 9212
	private CScrollRectLegacy _scrollView;

	// Token: 0x040023FD RID: 9213
	private Transform _content;

	// Token: 0x040023FE RID: 9214
	private RectTransform _contentRect;

	// Token: 0x040023FF RID: 9215
	private EventLogData _eventLogData;

	// Token: 0x04002400 RID: 9216
	private PoolItem _containerPool;

	// Token: 0x04002401 RID: 9217
	private PoolItem _arrowPool;

	// Token: 0x04002402 RID: 9218
	private PoolItem _singleValuePool;

	// Token: 0x04002403 RID: 9219
	private PoolItem _statusPool;

	// Token: 0x04002404 RID: 9220
	private PoolItem _dialogPool;

	// Token: 0x04002405 RID: 9221
	private PoolItem _dialog2Pool;

	// Token: 0x04002406 RID: 9222
	private PoolItem _dialog3Pool;

	// Token: 0x04002407 RID: 9223
	private PoolItem _dialog4Pool;

	// Token: 0x04002408 RID: 9224
	private PoolItem _combatResultPool;

	// Token: 0x04002409 RID: 9225
	private PoolItem _itemPool;

	// Token: 0x0400240A RID: 9226
	private PoolItem _resourcePool;

	// Token: 0x0400240B RID: 9227
	private PoolItem _teammatePool;

	// Token: 0x0400240C RID: 9228
	private PoolItem _combatSkillPool;

	// Token: 0x0400240D RID: 9229
	private PoolItem _lifeSkillPool;

	// Token: 0x0400240E RID: 9230
	private PoolItem _relationPool;

	// Token: 0x0400240F RID: 9231
	private PoolItem _featurePool;

	// Token: 0x04002410 RID: 9232
	private PoolItem _professionPool;

	// Token: 0x04002411 RID: 9233
	private PoolItem _normalInformationPool;

	// Token: 0x04002412 RID: 9234
	private PoolItem _secretInformationPool;

	// Token: 0x04002413 RID: 9235
	private PoolItem _endingPool;
}
