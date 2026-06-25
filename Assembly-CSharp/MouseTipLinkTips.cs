using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020002B5 RID: 693
[RequireComponent(typeof(TextMeshProUGUI))]
public class MouseTipLinkTips : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	// Token: 0x06002AA7 RID: 10919 RVA: 0x001469F0 File Offset: 0x00144BF0
	private void Awake()
	{
		bool flag = null == this._tmpText;
		if (flag)
		{
			this._tmpText = base.GetComponent<TextMeshProUGUI>();
		}
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x00146A1A File Offset: 0x00144C1A
	private void OnEnable()
	{
		this._currentTipType = TipType.Count;
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x00146A28 File Offset: 0x00144C28
	private void OnDisable()
	{
		this.HideTip();
		this.UnRegisterListener();
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x00146A3C File Offset: 0x00144C3C
	private void LateUpdate()
	{
		bool flag = this._needCheckId && this._tmpText;
		if (flag)
		{
			int linkIndex = TMP_TextUtilities.FindIntersectingLink(this._tmpText, Input.mousePosition, UIManager.Instance.UiCamera);
			bool flag2 = linkIndex == -1;
			if (flag2)
			{
				this.HideTip();
			}
			else
			{
				TMP_LinkInfo linkInfo = this._tmpText.textInfo.linkInfo[linkIndex];
				string linkId = linkInfo.GetLinkID();
				bool flag3 = !string.IsNullOrEmpty(linkId) && linkId != this._currentLinkId;
				if (flag3)
				{
					ArgumentBox box = EasyPool.Get<ArgumentBox>();
					bool flag4 = this.OnLinkTipActivate == null;
					if (flag4)
					{
						this._currentTipType = this.OnMouseOverLinkDefault(linkId, box);
					}
					else
					{
						this._currentTipType = this.OnLinkTipActivate(linkId, box);
					}
					bool flag5 = this._currentTipType == TipType.Count;
					if (!flag5)
					{
						SingletonObject.getInstance<TooltipManager>().ShowTips(this._currentTipType, box, false, false, false, null);
						this._currentLinkId = linkId;
					}
				}
			}
		}
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x00146B4C File Offset: 0x00144D4C
	private void HideTip()
	{
		bool flag = this._currentTipType != TipType.Count;
		if (flag)
		{
			SingletonObject.getInstance<TooltipManager>().HideTips(this._currentTipType, true);
		}
		this._currentTipType = TipType.Count;
		this._currentLinkId = string.Empty;
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x00146B96 File Offset: 0x00144D96
	public void OnPointerEnter(PointerEventData eventData)
	{
		this._needCheckId = true;
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x00146BA0 File Offset: 0x00144DA0
	public void OnPointerExit(PointerEventData eventData)
	{
		this._needCheckId = false;
		this.HideTip();
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x00146BB4 File Offset: 0x00144DB4
	private void RegisterListener()
	{
		bool flag = this._listenerId > 0;
		if (!flag)
		{
			this._itemDisplayDataCacheMap = new Dictionary<string, ItemDisplayData>();
			this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		}
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x00146BF4 File Offset: 0x00144DF4
	private void UnRegisterListener()
	{
		bool flag = this._listenerId > 0;
		if (flag)
		{
			GameDataBridge.UnregisterListener(this._listenerId);
			this._itemDisplayDataCacheMap.Clear();
			this._itemDisplayDataCacheMap = null;
			this._listenerId = -1;
		}
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x00146C38 File Offset: 0x00144E38
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 6;
				if (flag)
				{
					bool flag2 = notification.MethodId == 7;
					if (flag2)
					{
						ItemDisplayData data = new ItemDisplayData();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref data);
						string key = string.Format("item_{0}_{1}_{2}_{3}", new object[]
						{
							data.Key.Id,
							data.Key.ItemType,
							data.Key.TemplateId,
							data.Key.ModificationState
						});
						bool flag3 = this._itemDisplayDataCacheMap.ContainsKey(key);
						if (flag3)
						{
							this._itemDisplayDataCacheMap[key] = data;
						}
					}
				}
			}
		}
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x00146D7C File Offset: 0x00144F7C
	private TipType OnMouseOverLinkDefault(string id, ArgumentBox argBox)
	{
		bool flag = id.StartsWith("character_");
		TipType result;
		if (flag)
		{
			int charId = int.Parse(id.Substring(10));
			argBox.Set("charId", charId);
			result = TipType.Character;
		}
		else
		{
			bool flag2 = id.StartsWith("ForgottenCharacter");
			if (flag2)
			{
				argBox.Set("arg0", "???");
				argBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_Forgotten_Character));
				result = TipType.Simple;
			}
			else
			{
				bool flag3 = id.StartsWith("resource_");
				if (flag3)
				{
					string[] splitArray = id.Split('_', StringSplitOptions.None);
					sbyte resourceType;
					int count;
					int countMax;
					bool flag4 = sbyte.TryParse(splitArray[2], out resourceType) && int.TryParse(splitArray[3], out count) && int.TryParse(splitArray[4], out countMax);
					if (flag4)
					{
						argBox.Set("CharName", splitArray[1]);
						argBox.Set("ResourceType", resourceType);
						argBox.Set("ResourceCount", count);
						argBox.Set("ResourceCountMax", countMax);
						return TipType.Resource;
					}
				}
				bool flag5 = id.StartsWith("item_");
				if (flag5)
				{
					string[] splitArray2 = id.Split('_', StringSplitOptions.None);
					int itemId;
					sbyte itemType;
					short templateId;
					byte modificationState;
					bool flag6 = int.TryParse(splitArray2[1], out itemId) && sbyte.TryParse(splitArray2[2], out itemType) && short.TryParse(splitArray2[3], out templateId) && byte.TryParse(splitArray2[4], out modificationState);
					if (flag6)
					{
						ItemKey itemKey = new ItemKey(itemType, modificationState, templateId, itemId);
						this.RegisterListener();
						ItemDisplayData itemData;
						bool flag7 = !this._itemDisplayDataCacheMap.TryGetValue(id, out itemData);
						if (flag7)
						{
							ItemDomainMethod.Call.GetItemDisplayData(this._listenerId, itemKey);
							this._itemDisplayDataCacheMap.Add(id, null);
							return TipType.Count;
						}
						bool flag8 = itemData == null;
						if (flag8)
						{
							return TipType.Count;
						}
						argBox.SetObject("ItemData", itemData);
						return TooltipManager.ItemTypeToTipType[itemType];
					}
				}
				argBox.Set("arg0", "Invalid LinkId");
				argBox.Set("arg1", id);
				result = TipType.Simple;
			}
		}
		return result;
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x00146F90 File Offset: 0x00145190
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = this._needCheckId && this._tmpText;
		if (flag)
		{
			int linkIndex = TMP_TextUtilities.FindIntersectingLink(this._tmpText, Input.mousePosition, UIManager.Instance.UiCamera);
			bool flag2 = linkIndex != -1;
			if (flag2)
			{
				TMP_LinkInfo linkInfo = this._tmpText.textInfo.linkInfo[linkIndex];
				string linkId = linkInfo.GetLinkID();
				bool flag3 = !string.IsNullOrEmpty(linkId);
				if (flag3)
				{
					this.HideTip();
					Action<string> onLinkTipClick = this.OnLinkTipClick;
					if (onLinkTipClick != null)
					{
						onLinkTipClick(linkId);
					}
				}
			}
		}
	}

	// Token: 0x04001EDE RID: 7902
	private TipType _currentTipType;

	// Token: 0x04001EDF RID: 7903
	private string _currentLinkId;

	// Token: 0x04001EE0 RID: 7904
	private bool _needCheckId;

	// Token: 0x04001EE1 RID: 7905
	private TextMeshProUGUI _tmpText;

	// Token: 0x04001EE2 RID: 7906
	public Func<string, ArgumentBox, TipType> OnLinkTipActivate;

	// Token: 0x04001EE3 RID: 7907
	public Action<string> OnLinkTipClick;

	// Token: 0x04001EE4 RID: 7908
	private int _listenerId;

	// Token: 0x04001EE5 RID: 7909
	private Dictionary<string, ItemDisplayData> _itemDisplayDataCacheMap;
}
