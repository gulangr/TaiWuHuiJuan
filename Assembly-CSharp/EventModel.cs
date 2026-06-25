using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Views.EventWindow;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.TaiwuEvent.EventOption;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class EventModel : ISingletonInit, IDisposable
{
	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000C75 RID: 3189 RVA: 0x00051906 File Offset: 0x0004FB06
	public int ListenerId
	{
		get
		{
			return this._listenerId;
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000C76 RID: 3190 RVA: 0x0005190E File Offset: 0x0004FB0E
	// (set) Token: 0x06000C77 RID: 3191 RVA: 0x00051916 File Offset: 0x0004FB16
	public bool HasListeningEvent { get; private set; }

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000C78 RID: 3192 RVA: 0x0005191F File Offset: 0x0004FB1F
	// (set) Token: 0x06000C79 RID: 3193 RVA: 0x00051927 File Offset: 0x0004FB27
	public bool LockInputByEvent { get; private set; }

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000C7A RID: 3194 RVA: 0x00051930 File Offset: 0x0004FB30
	// (set) Token: 0x06000C7B RID: 3195 RVA: 0x00051938 File Offset: 0x0004FB38
	public string LeftRoleNameKey { get; private set; }

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000C7C RID: 3196 RVA: 0x00051941 File Offset: 0x0004FB41
	// (set) Token: 0x06000C7D RID: 3197 RVA: 0x00051949 File Offset: 0x0004FB49
	public string RightRoleNameKey { get; private set; }

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000C7E RID: 3198 RVA: 0x00051952 File Offset: 0x0004FB52
	// (set) Token: 0x06000C7F RID: 3199 RVA: 0x0005195A File Offset: 0x0004FB5A
	public sbyte RightRoleXiangShuAvatarId { get; private set; } = 9;

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000C80 RID: 3200 RVA: 0x00051963 File Offset: 0x0004FB63
	// (set) Token: 0x06000C81 RID: 3201 RVA: 0x0005196B File Offset: 0x0004FB6B
	public sbyte RightRoleXiangShuDisplayStatus { get; private set; }

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000C82 RID: 3202 RVA: 0x00051974 File Offset: 0x0004FB74
	// (set) Token: 0x06000C83 RID: 3203 RVA: 0x0005197C File Offset: 0x0004FB7C
	public bool HideAllMapBlockCharacters { get; private set; }

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000C84 RID: 3204 RVA: 0x00051985 File Offset: 0x0004FB85
	// (set) Token: 0x06000C85 RID: 3205 RVA: 0x0005198D File Offset: 0x0004FB8D
	public bool NeedToNotifyNewMonth { get; private set; }

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00051996 File Offset: 0x0004FB96
	public bool IsOnNormalInteractEvent
	{
		get
		{
			IEnumerable<string> canOperateEventArray = EventModel.CanOperateEventArray;
			TaiwuEventDisplayData displayingEventData = this.DisplayingEventData;
			return canOperateEventArray.Contains((displayingEventData != null) ? displayingEventData.EventGuid : null);
		}
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x000519B4 File Offset: 0x0004FBB4
	public void Select(string optionKey, Action importantOptionCancel = null)
	{
		EventModel.<>c__DisplayClass62_0 CS$<>8__locals1 = new EventModel.<>c__DisplayClass62_0();
		CS$<>8__locals1.optionKey = optionKey;
		CS$<>8__locals1.<>4__this = this;
		bool flag = this.DisplayingEventData != null;
		if (flag)
		{
			int optionIndex = this.DisplayingEventData.EventOptionInfos.FindIndex((EventOptionInfo e) => e.OptionKey == CS$<>8__locals1.optionKey);
			bool flag2 = optionIndex >= 0;
			if (flag2)
			{
				EventModel.<>c__DisplayClass62_1 CS$<>8__locals2 = new EventModel.<>c__DisplayClass62_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.eventOptionInfo = this.DisplayingEventData.EventOptionInfos[optionIndex];
				bool important = CS$<>8__locals2.eventOptionInfo.Important;
				if (important)
				{
					string title = LocalStringManager.Get(LanguageKey.UI_EventWindow_Important_Option_Title);
					string content = LocalStringManager.Get(LanguageKey.UI_EventWindow_Important_Option_Content);
					CommonUtils.ShowConfirmDialog(title, content, new Action(CS$<>8__locals2.<Select>g__ConfirmAction|1), importantOptionCancel, EDialogType.None);
				}
				else
				{
					CS$<>8__locals2.<Select>g__ConfirmAction|1();
				}
			}
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00051A8C File Offset: 0x0004FC8C
	private void TryRecordInteractionOptionCount(EventOptionInfo eventOptionInfo)
	{
		string optionGuid = eventOptionInfo.OptionGuid;
		foreach (InteractionEventOptionItem config in ((IEnumerable<InteractionEventOptionItem>)InteractionEventOption.Instance))
		{
			bool flag = config.OptionGuid == optionGuid;
			if (flag)
			{
				GameLifetimeDataManager.RecordInteractionEventOptionSelected(config.TemplateId);
			}
		}
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00051AFC File Offset: 0x0004FCFC
	public void SetSelectItemResult(List<ItemKey> selectList, Dictionary<ItemKey, int> selectDict)
	{
		List<ItemKey> itemKeyList = new List<ItemKey>();
		bool flag = this.DisplayingEventData.ExtraData.SelectItemData != null;
		if (flag)
		{
			EventModel.<>c__DisplayClass64_0 CS$<>8__locals1;
			CS$<>8__locals1.filterList = new List<SelectItemFilter>(this.DisplayingEventData.ExtraData.SelectItemData.FilterList);
			Dictionary<ItemKey, ITradeableContent> canSelectItemMap = new Dictionary<ItemKey, ITradeableContent>();
			int i = 0;
			int max = this.DisplayingEventData.ExtraData.SelectItemData.CanSelectItemList.Count;
			while (i < max)
			{
				ITradeableContent data = this.DisplayingEventData.ExtraData.SelectItemData.CanSelectItemList[i];
				Inventory inventory = data.GetAllInventoryFromPool();
				foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
				{
					ItemKey itemKey4;
					int num;
					keyValuePair.Deconstruct(out itemKey4, out num);
					ItemKey key = itemKey4;
					canSelectItemMap[key] = data;
				}
				ItemDisplayData.ReturnInventoryToPool(inventory);
				i++;
			}
			List<ItemKey> selectListCollect = new List<ItemKey>();
			foreach (KeyValuePair<ItemKey, int> item in selectDict)
			{
				bool flag2 = ItemTemplateHelper.IsMiscResource(item.Key.ItemType, item.Key.TemplateId);
				if (flag2)
				{
					selectListCollect.Add(item.Key);
				}
				else
				{
					for (int j = 0; j < item.Value; j++)
					{
						selectListCollect.Add(item.Key);
					}
				}
			}
			for (int k = CS$<>8__locals1.filterList.Count - 1; k >= 0; k--)
			{
				SelectItemFilter filter = CS$<>8__locals1.filterList[k];
				bool flag3 = filter.DisplayDataFilterId > 0;
				if (flag3)
				{
					Predicate<ITradeableContent> matcher = ItemDisplayDataFilters.GetFilter(filter.DisplayDataFilterId);
					bool flag4 = matcher != null;
					if (flag4)
					{
						int l = 0;
						int jMax = selectListCollect.Count;
						while (l < jMax)
						{
							ItemKey itemKey = selectListCollect[l];
							ITradeableContent itemDisplayData;
							bool flag5 = canSelectItemMap.TryGetValue(itemKey, out itemDisplayData) && itemDisplayData != null && matcher(itemDisplayData);
							if (flag5)
							{
								itemKeyList.Add(selectListCollect[l]);
								CS$<>8__locals1.filterList.RemoveAt(k);
								selectListCollect.RemoveAt(l);
								bool flag6 = !itemDisplayData.HasAnyPoison;
								if (flag6)
								{
									bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemKey.ItemType, itemKey.TemplateId);
									bool flag7 = isMiscResource;
									if (flag7)
									{
										TaiwuEventDomainMethod.Call.SetItemSelectCount(filter.Key, selectDict[itemKey]);
									}
									TaiwuEventDomainMethod.Call.SetItemSelectResult(filter.Key, itemKey, CS$<>8__locals1.filterList.Count == 0);
								}
								else
								{
									EventModel.<SetSelectItemResult>g__SendPoisonItemToBackend|64_0(itemDisplayData, filter.Key, ref CS$<>8__locals1);
								}
								break;
							}
							l++;
						}
					}
				}
				else
				{
					ItemFilterRulesItem filterConfig = ItemFilterRules.Instance.GetItem(filter.FilterTemplateId);
					bool flag8 = filterConfig != null;
					if (flag8)
					{
						int m = 0;
						int jMax2 = selectListCollect.Count;
						while (m < jMax2)
						{
							ItemKey itemKey2 = selectListCollect[m];
							ITradeableContent itemDisplayData2;
							bool flag9 = ItemUtils.MatchItemFilterRule(itemKey2, filterConfig) && canSelectItemMap.TryGetValue(itemKey2, out itemDisplayData2) && itemDisplayData2 != null;
							if (flag9)
							{
								itemKeyList.Add(selectListCollect[m]);
								CS$<>8__locals1.filterList.RemoveAt(k);
								selectListCollect.RemoveAt(m);
								bool flag10 = !itemDisplayData2.HasAnyPoison;
								if (flag10)
								{
									bool isMiscResource2 = ItemTemplateHelper.IsMiscResource(itemKey2.ItemType, itemKey2.TemplateId);
									bool flag11 = isMiscResource2;
									if (flag11)
									{
										TaiwuEventDomainMethod.Call.SetItemSelectCount(filter.Key, selectDict[itemKey2]);
									}
									TaiwuEventDomainMethod.Call.SetItemSelectResult(filter.Key, itemKey2, CS$<>8__locals1.filterList.Count == 0);
								}
								else
								{
									EventModel.<SetSelectItemResult>g__SendPoisonItemToBackend|64_0(itemDisplayData2, filter.Key, ref CS$<>8__locals1);
								}
								break;
							}
							m++;
						}
					}
				}
			}
			int minSelectAmount = this.DisplayingEventData.ExtraData.SelectItemData.MinSelectAmount;
			bool flag12 = CS$<>8__locals1.filterList.Count > 0 && (selectListCollect.Count == CS$<>8__locals1.filterList.Count || selectListCollect.Count >= minSelectAmount);
			if (flag12)
			{
				int iterationStart = (selectListCollect.Count == CS$<>8__locals1.filterList.Count) ? (CS$<>8__locals1.filterList.Count - 1) : (selectListCollect.Count - 1);
				for (int n = iterationStart; n >= 0; n--)
				{
					SelectItemFilter filter2 = CS$<>8__locals1.filterList[n];
					ItemKey itemKey3 = selectListCollect[n];
					itemKeyList.Add(selectListCollect[n]);
					CS$<>8__locals1.filterList.RemoveAt(n);
					selectListCollect.RemoveAt(n);
					ITradeableContent itemDisplayData3;
					bool flag13 = canSelectItemMap.TryGetValue(itemKey3, out itemDisplayData3) && itemDisplayData3 != null;
					if (flag13)
					{
						bool flag14 = !itemDisplayData3.HasAnyPoison;
						if (flag14)
						{
							bool isMiscResource3 = ItemTemplateHelper.IsMiscResource(itemKey3.ItemType, itemKey3.TemplateId);
							bool flag15 = isMiscResource3;
							if (flag15)
							{
								TaiwuEventDomainMethod.Call.SetItemSelectCount(filter2.Key, selectDict[itemKey3]);
							}
							TaiwuEventDomainMethod.Call.SetItemSelectResult(filter2.Key, itemKey3, CS$<>8__locals1.filterList.Count == 0);
						}
						else
						{
							EventModel.<SetSelectItemResult>g__SendPoisonItemToBackend|64_0(itemDisplayData3, filter2.Key, ref CS$<>8__locals1);
						}
					}
				}
			}
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.SetObject("ItemResult", itemKeyList);
			GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
		}
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x000520EC File Offset: 0x000502EC
	public void SetSelectCharacterResult(List<int> charIdList)
	{
		bool flag = this.DisplayingEventData.ExtraData.SelectCharacterData != null;
		if (flag)
		{
			List<CharacterSelectFilter> filterList = this.DisplayingEventData.ExtraData.SelectCharacterData.FilterList;
			bool flag2 = filterList != null;
			if (flag2)
			{
				for (int i = 0; i < filterList.Count; i++)
				{
					CharacterSelectFilter filter = filterList[i];
					bool flag3 = filter.AvailableCharactersDisplayDataList == null;
					if (!flag3)
					{
						List<int> availableCharacters = (from element in filter.AvailableCharactersDisplayDataList
						select element.CharacterId).ToList<int>();
						for (int a = 0; a < charIdList.Count; a++)
						{
							bool flag4 = availableCharacters.Contains(charIdList[a]);
							if (flag4)
							{
								TaiwuEventDomainMethod.Call.SetCharacterSelectResult(filter.SelectKey, charIdList[a], i == filterList.Count - 1);
								ArgumentBox box = EasyPool.Get<ArgumentBox>();
								box.SetObject("CharacterResult", charIdList);
								GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
								charIdList.RemoveAt(a);
								break;
							}
						}
					}
				}
			}
			else
			{
				bool flag5 = this.DisplayingEventData.ExtraData.SelectCharacterData.SelectApprovedTaiwu != null;
				if (flag5)
				{
					TaiwuEventDomainMethod.Call.SetCharacterMultSelectResult("SelectApprovedTaiwu", charIdList, true);
					ArgumentBox box2 = EasyPool.Get<ArgumentBox>();
					box2.SetObject("CharacterResult", charIdList);
					GEvent.OnEvent(UiEvents.EventWindowAppearResult, box2);
				}
			}
		}
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00052280 File Offset: 0x00050480
	public void SetInputResult(string inputResult)
	{
		bool flag = this.DisplayingEventData.ExtraData.InputRequestData != null;
		if (flag)
		{
			bool flag2 = this.DisplayingEventData.ExtraData.InputRequestData.InputDataType == 1;
			if (flag2)
			{
				int intValue;
				bool flag3 = int.TryParse(inputResult, out intValue);
				if (flag3)
				{
					int[] range = this.DisplayingEventData.ExtraData.InputRequestData.NumberRange;
					bool flag4 = range == null || (intValue >= range[0] && intValue <= range[1]);
					if (flag4)
					{
						TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("InputActionComplete", this.DisplayingEventData.ExtraData.InputRequestData.DataKey, intValue);
					}
				}
			}
			else
			{
				bool flag5 = this.DisplayingEventData.ExtraData.InputRequestData.InputDataType == 3;
				if (flag5)
				{
					SingletonObject.getInstance<ProfessionModel>().HandleSetGiveNameResult(inputResult, this.DisplayingEventData.ExtraData.InputRequestData.FullName);
				}
				TaiwuEventDomainMethod.Call.SetListenerEventActionStringArg("InputActionComplete", this.DisplayingEventData.ExtraData.InputRequestData.DataKey, inputResult);
			}
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("InputValue", inputResult);
			GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
		}
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x000523B8 File Offset: 0x000505B8
	public void SetInputNameResult(string inputResultSurName, string inputResultGivenName)
	{
		bool flag = this.DisplayingEventData.ExtraData.InputRequestData != null;
		if (flag)
		{
			TaiwuEventDomainMethod.Call.SetListenerEventActionStringArg("InputActionComplete", this.DisplayingEventData.ExtraData.InputRequestData.DataKey + EventInputRequestData.ExtraSurNameKey, inputResultSurName);
			TaiwuEventDomainMethod.Call.SetListenerEventActionStringArg("InputActionComplete", this.DisplayingEventData.ExtraData.InputRequestData.DataKey, inputResultGivenName);
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("InputNameSur", inputResultSurName);
			box.Set("InputNameGiven", inputResultGivenName);
			GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
		}
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0005245C File Offset: 0x0005065C
	private void SetSecretInformationSelectResult(SecretInformationDisplayData displayData)
	{
		bool flag = displayData != null;
		if (flag)
		{
			TaiwuEventDomainMethod.Call.SetSecretInformationSelectResult(this.SelectInformationData.SaveKey, (int)displayData.SecretInformationId);
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("SecretInformationResult", (int)displayData.SecretInformationId);
			GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x000524BC File Offset: 0x000506BC
	public void SetNormalInformationSelectResult(NormalInformation normalInformation)
	{
		TaiwuEventDomainMethod.Call.SetNormalInformationSelectResult(this.SelectInformationData.SaveKey, normalInformation);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("NormalInformationTemplateId", normalInformation.TemplateId).Set("NormalInformationLevel", normalInformation.Level);
		GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00052514 File Offset: 0x00050714
	public void SetSelectCountResult(int count)
	{
		TaiwuEventDomainMethod.Call.SetSelectCount(count);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.SetObject("SelectCount", count);
		GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00052550 File Offset: 0x00050750
	public void SetSelectBookCountResult(int count, ItemKey targetBook)
	{
		TaiwuEventDomainMethod.Call.SetSelectCount(count);
		TaiwuEventDomainMethod.Call.SetShowingEventItemKeyArg("SelectItemKey", targetBook);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.SetObject("SelectCount", count);
		GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00052598 File Offset: 0x00050798
	public void SetSelectCombatSkillCountResult(int count, short combstSkillId)
	{
		TaiwuEventDomainMethod.Call.SetSelectCount(count);
		TaiwuEventDomainMethod.Call.SetShowingEventShortArg("SelectCombatSkillTemplateId", combstSkillId);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.SetObject("SelectCount", count);
		GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x000525E0 File Offset: 0x000507E0
	public void SetSelectFameActionResult(List<short> fameActionIdList)
	{
		GameData.Utilities.ShortList shortList = GameData.Utilities.ShortList.Create();
		shortList.Items = fameActionIdList;
		TaiwuEventDomainMethod.Call.SetShowingEventShortListArg("SelectedFameActions", shortList);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set<GameData.Utilities.ShortList>("SelectedFameActions", shortList);
		GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x00052629 File Offset: 0x00050829
	public void SetLockInputState(bool state)
	{
		this.LockInputByEvent = state;
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00052634 File Offset: 0x00050834
	public bool CanCommunicateWithMateByEventModel()
	{
		bool hasListeningEvent = this.HasListeningEvent;
		bool result;
		if (hasListeningEvent)
		{
			result = false;
		}
		else
		{
			bool flag = this.DisplayingEventData != null;
			result = !flag;
		}
		return result;
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x00052668 File Offset: 0x00050868
	public string GetOptionConditionContent(short index)
	{
		bool flag = EventModel._customOptionAvailableHandler != null;
		if (flag)
		{
			string result = EventModel._customOptionAvailableHandler(index);
			bool flag2 = !string.IsNullOrEmpty(result);
			if (flag2)
			{
				return result;
			}
		}
		bool flag3 = this._optionAvailableContents.CheckIndex((int)index);
		string result2;
		if (flag3)
		{
			result2 = this._optionAvailableContents[(int)index];
		}
		else
		{
			result2 = string.Format("{0} id out of OptionAvailableContents range!", index);
		}
		return result2;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000526D4 File Offset: 0x000508D4
	public void SetCgDataHandled()
	{
		ValueTuple<bool, string, float> tuple = this.EventCgTextureData;
		tuple.Item1 = true;
		this.EventCgTextureData = tuple;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x000526F8 File Offset: 0x000508F8
	public void SetEventCgData(string texturePath, float tweenTime)
	{
		this.EventCgTextureData = new ValueTuple<bool, string, float>(false, texturePath, tweenTime);
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0005270C File Offset: 0x0005090C
	public string GetOptionConsumeInfoTextMeshProSpriteString(OptionConsumeInfo consumeInfo)
	{
		string result = consumeInfo.HoldCount.ToString().SetColor(consumeInfo.HasEnough ? "brightblue" : "brightred") + "/" + consumeInfo.ConsumeCount.ToString();
		bool flag = consumeInfo.ConsumeType < 8;
		if (flag)
		{
			result = "<SpName=ui9_icon_resource_bar_" + consumeInfo.ConsumeType.ToString() + "> " + result;
		}
		else
		{
			sbyte consumeType = consumeInfo.ConsumeType;
			bool flag2 = consumeType >= 12 && consumeType <= 17;
			if (flag2)
			{
				result = "<SpName=ui9_icon_attribute_major_big_" + ((int)(consumeInfo.ConsumeType - 12)).ToString() + "> " + result;
			}
			else
			{
				switch (consumeInfo.ConsumeType)
				{
				case 8:
					result = "<SpName=ui9_icon_event_action_point_4> " + result;
					break;
				case 9:
				case 10:
					result = LocalStringManager.GetFormat(LanguageKey.LK_EventOptionConsumeFormat, CommonUtils.GetDisplayStringForNum(consumeInfo.ConsumeCount, 100000) ?? "", CommonUtils.GetDisplayStringForNum(consumeInfo.HoldCount, 100000) ?? "");
					result = "<SpName=" + string.Format("ui9_btn_resource_bar_{0}_0", 10) + "> " + result;
					break;
				case 11:
					result = "<SpName=ui9_icon_resource_big_8> " + result;
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0005287C File Offset: 0x00050A7C
	public string GetOptionConsumeInfoDisplayName(sbyte consumeType)
	{
		bool flag = consumeType < 8;
		string result;
		if (flag)
		{
			bool flag2 = consumeType < 0 || (int)consumeType >= Config.ResourceType.Instance.Count;
			if (flag2)
			{
				result = string.Empty;
			}
			else
			{
				result = Config.ResourceType.Instance[consumeType].Name;
			}
		}
		else
		{
			bool flag3 = consumeType >= 12 && consumeType <= 17;
			if (flag3)
			{
				result = EventModel.GetOptionConsumeMainAttributeName(consumeType - 12);
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (consumeType)
				{
				case 8:
					text = LocalStringManager.Get(LanguageKey.LK_ActionPoint);
					break;
				case 9:
				case 10:
					text = LocalStringManager.Get(LanguageKey.LK_Area_Debt_Tip_Title);
					break;
				case 11:
					text = LocalStringManager.Get(LanguageKey.LK_Exp);
					break;
				default:
					text = string.Empty;
					break;
				}
				if (!true)
				{
				}
				result = text;
			}
		}
		return result;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x00052948 File Offset: 0x00050B48
	private static string GetOptionConsumeMainAttributeName(sbyte type)
	{
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case 0:
			result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Strength);
			break;
		case 1:
			result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Dexterity);
			break;
		case 2:
			result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Concentration);
			break;
		case 3:
			result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Vitality);
			break;
		case 4:
			result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Energy);
			break;
		case 5:
			result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Intelligence);
			break;
		default:
			result = string.Empty;
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x000529DC File Offset: 0x00050BDC
	public bool NeedShowAsMarriageLook1(int charId)
	{
		return this._marriageLook1CharIdList != null && this._marriageLook1CharIdList.Contains(charId);
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x00052A08 File Offset: 0x00050C08
	public bool NeedShowAsMarriageLook2(int charId)
	{
		return this._marriageLook2CharIdList != null && this._marriageLook2CharIdList.Contains(charId);
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x00052A34 File Offset: 0x00050C34
	public bool NeedShowJieqingMask(int charId)
	{
		return this._jieqingMaskCharIdList != null && this._jieqingMaskCharIdList.Contains(charId);
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x00052A60 File Offset: 0x00050C60
	public bool NeedShowShixiangBarbarianMasterCloth(int charId)
	{
		return this._shixiangBarbarianMasterIdList != null && this._shixiangBarbarianMasterIdList.Contains(charId);
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x00052A8C File Offset: 0x00050C8C
	public bool NeedShowBlush(int charId)
	{
		return this._showBlushCharIdList != null && this._showBlushCharIdList.Contains(charId);
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x00052AB8 File Offset: 0x00050CB8
	public bool TryGetTaiwuClothingDisplayId(int charId, out short displayId)
	{
		displayId = 0;
		bool flag = this._taiwuClothingTemplateId < 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag2 = charId != taiwuCharId;
			if (flag2)
			{
				result = false;
			}
			else
			{
				ClothingItem config = Clothing.Instance[this._taiwuClothingTemplateId];
				displayId = config.DisplayId;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x00052B14 File Offset: 0x00050D14
	public void CheckAvatarClothDisplayIdForEvent(int charId, AvatarData avatarData, AvatarRelatedData avatarRelatedData = null)
	{
		bool flag = this.DisplayingEventData == null || this.DisplayingEventData.ExtraData == null;
		if (!flag)
		{
			CharacterDisplayData mainCharacter = this.DisplayingEventData.MainCharacter;
			int? num = (mainCharacter != null) ? new int?(mainCharacter.CharacterId) : null;
			bool flag2 = (charId == num.GetValueOrDefault() & num != null) && this.DisplayingEventData.ExtraData.MainRoleAdjustClothDisplayId >= 0;
			if (flag2)
			{
				avatarData.ClothDisplayId = this.DisplayingEventData.ExtraData.MainRoleAdjustClothDisplayId;
				bool flag3 = avatarRelatedData != null;
				if (flag3)
				{
					avatarRelatedData.ClothingDisplayId = this.DisplayingEventData.ExtraData.MainRoleAdjustClothDisplayId;
				}
			}
			CharacterDisplayData targetCharacter = this.DisplayingEventData.TargetCharacter;
			num = ((targetCharacter != null) ? new int?(targetCharacter.CharacterId) : null);
			bool flag4 = (charId == num.GetValueOrDefault() & num != null) && this.DisplayingEventData.ExtraData.TargetRoleAdjustClothDisplayId >= 0;
			if (flag4)
			{
				avatarData.ClothDisplayId = this.DisplayingEventData.ExtraData.TargetRoleAdjustClothDisplayId;
				bool flag5 = avatarRelatedData != null;
				if (flag5)
				{
					avatarRelatedData.ClothingDisplayId = this.DisplayingEventData.ExtraData.TargetRoleAdjustClothDisplayId;
				}
			}
		}
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x00052C64 File Offset: 0x00050E64
	public void Init()
	{
		this.LoadEventOptionTipsLanguageFile();
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 24, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 10, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 11, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 12, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 13, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 14, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 15, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 16, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 20, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 17, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 18, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 29, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 21, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 22, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 64, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 5, 117, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 25, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 27, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 12, 28, ulong.MaxValue, uint.MaxValue);
		SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false);
		GEvent.Add(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIElementHide));
		foreach (EventOptionTipsInfoItem config in ((IEnumerable<EventOptionTipsInfoItem>)EventOptionTipsInfo.Instance))
		{
			bool flag = config.Guid != null;
			if (flag)
			{
				foreach (string guid in config.Guid)
				{
					EventModel.EventOptionTipInfo[guid] = (short)config.TemplateId;
				}
			}
		}
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x00052EF4 File Offset: 0x000510F4
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 3, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 4, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 5, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 24, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 10, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 11, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 12, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 13, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 14, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 15, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 16, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 20, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 17, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 18, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 29, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 21, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 22, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 64, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 5, 117, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 25, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 27, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 12, 28, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._listenerId);
		this._listenerId = -1;
		GEvent.Remove(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIElementHide));
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x000530CC File Offset: 0x000512CC
	private void OnUIElementHide(ArgumentBox box)
	{
		UIElement element;
		bool flag = box.Get<UIElement>("Element", out element);
		if (flag)
		{
			bool flag2 = element == UIElement.Dialog;
			if (flag2)
			{
				this.NextNotify();
			}
		}
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00053104 File Offset: 0x00051304
	private void AddNotify(EventNotifyData notifyData)
	{
		bool flag = !string.IsNullOrEmpty(notifyData.TitleKey) && !string.IsNullOrEmpty(notifyData.ContentKey);
		if (flag)
		{
			this._notifyInfoList.Add(notifyData);
		}
		bool flag2 = !UIElement.Dialog.Exist;
		if (flag2)
		{
			this.NextNotify();
		}
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0005315C File Offset: 0x0005135C
	private void NextNotify()
	{
		bool flag = this._notifyInfoList == null || this._notifyInfoList.Count <= 0;
		if (!flag)
		{
			EventNotifyData data = this._notifyInfoList[0];
			DialogCmd dialogCmd = new DialogCmd();
			string title;
			if (data.TitleFormatArgs != null)
			{
				string titleKey = data.TitleKey;
				string[] titleFormatArgs = data.TitleFormatArgs;
				title = LocalStringManager.GetFormat(titleKey, (titleFormatArgs != null) ? titleFormatArgs.ChangeArrType(null) : null).ColorReplace();
			}
			else
			{
				title = LocalStringManager.Get(data.TitleKey);
			}
			dialogCmd.Title = title;
			string content;
			if (data.ContentFormatArgs != null)
			{
				string contentKey = data.ContentKey;
				string[] contentFormatArgs = data.ContentFormatArgs;
				content = LocalStringManager.GetFormat(contentKey, (contentFormatArgs != null) ? contentFormatArgs.ChangeArrType(null) : null).ColorReplace();
			}
			else
			{
				content = LocalStringManager.Get(data.ContentKey);
			}
			dialogCmd.Content = content;
			dialogCmd.Type = 2;
			DialogCmd cmd = dialogCmd;
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
			this._notifyInfoList.RemoveAt(0);
		}
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00053260 File Offset: 0x00051460
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 12;
					if (flag)
					{
						this.GetMethodReturnValue(notification.MethodId, notification.ValueOffset, wrapper.DataPool);
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag2 = uid.DomainId == 12;
				if (flag2)
				{
					this.UpdateGameData(uid, notification.ValueOffset, wrapper.DataPool);
				}
				bool flag3 = uid.DomainId == 19;
				if (flag3)
				{
					ushort dataId = uid.DataId;
					ushort num = dataId;
					if (num == 64)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._shixiangBarbarianMasterIdList);
					}
				}
				bool flag4 = uid.DomainId == 5;
				if (flag4)
				{
					ushort dataId2 = uid.DataId;
					ushort num2 = dataId2;
					if (num2 == 117)
					{
						short value = 0;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref value);
						this._taiwuClothingTemplateId = value;
						bool exist = UIElement.EventWindow.Exist;
						if (exist)
						{
							GEvent.OnEvent(UiEvents.OnEventWindowDisplayDataChanged, null);
						}
					}
				}
			}
		}
		bool displayDataDirtyFlag = this._displayDataDirtyFlag;
		if (displayDataDirtyFlag)
		{
			this.RefreshShowingEvent();
		}
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x000533FC File Offset: 0x000515FC
	private void UpdateGameData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		switch (uid.DataId)
		{
		case 3:
		{
			bool flag2 = this._notifyInfoList == null;
			if (flag2)
			{
				this._notifyInfoList = new List<EventNotifyData>();
			}
			EventNotifyData data = new EventNotifyData();
			Serializer.Deserialize(dataPool, valueOffset, ref data);
			this.AddNotify(data);
			break;
		}
		case 4:
		{
			bool flag = false;
			Serializer.Deserialize(dataPool, valueOffset, ref flag);
			this.HasListeningEvent = flag;
			break;
		}
		case 5:
			Serializer.Deserialize(dataPool, valueOffset, ref this.SelectInformationData);
			this.HandleSelectInformationDataChange();
			break;
		case 10:
		{
			string nameKey = string.Empty;
			Serializer.Deserialize(dataPool, valueOffset, ref nameKey);
			this.LeftRoleNameKey = nameKey;
			break;
		}
		case 11:
		{
			string nameKey2 = string.Empty;
			Serializer.Deserialize(dataPool, valueOffset, ref nameKey2);
			this.RightRoleNameKey = nameKey2;
			break;
		}
		case 12:
		{
			sbyte[] dataArray = new sbyte[2];
			Serializer.Deserialize(dataPool, valueOffset, ref dataArray);
			this.RightRoleXiangShuAvatarId = dataArray[0];
			this.RightRoleXiangShuDisplayStatus = dataArray[1];
			break;
		}
		case 13:
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this.SelectCombatSkillData);
			bool flag3 = this.SelectCombatSkillData != null;
			if (flag3)
			{
				this.SelectCombatSkillData.SelectResultIndex = -1;
			}
			break;
		}
		case 14:
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this.SelectLifeSkillData);
			bool flag4 = this.SelectLifeSkillData != null;
			if (flag4)
			{
				this.SelectLifeSkillData.SelectResultIndex = -1;
			}
			break;
		}
		case 15:
			Serializer.Deserialize(dataPool, valueOffset, ref this.LeftItemList);
			break;
		case 16:
			Serializer.Deserialize(dataPool, valueOffset, ref this.RightItemList);
			break;
		case 17:
			Serializer.Deserialize(dataPool, valueOffset, ref this.ShowItemWithCricketBattleGuess);
			break;
		case 18:
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this.DisplayingEventData);
			bool flag5 = this.DisplayingEventData != null && string.IsNullOrEmpty(this.DisplayingEventData.EventGuid);
			if (flag5)
			{
				this.DisplayingEventData = null;
			}
			this.CommonOptionPreviewEventOptionInfos = null;
			this.AdjustDataForDisplay();
			this._displayDataDirtyFlag = true;
			break;
		}
		case 20:
			Serializer.Deserialize(dataPool, valueOffset, ref this.CoverCricketJarGradeList);
			break;
		case 21:
			Serializer.Deserialize(dataPool, valueOffset, ref this._marriageLook1CharIdList);
			GEvent.OnEvent(UiEvents.OnMarriageCharacterListChanged, null);
			break;
		case 22:
			Serializer.Deserialize(dataPool, valueOffset, ref this._marriageLook2CharIdList);
			GEvent.OnEvent(UiEvents.OnMarriageCharacterListChanged, null);
			break;
		case 24:
			Serializer.Deserialize(dataPool, valueOffset, ref this.CricketBettingData);
			this.HandleCricketBettingDataChange();
			break;
		case 25:
		{
			List<int> charIdList = new List<int>();
			Serializer.Deserialize(dataPool, valueOffset, ref charIdList);
			SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.JieqingMaskCharacterListChanged(charIdList));
			break;
		}
		case 27:
		{
			bool hideAllMapBlockCharacters = false;
			Serializer.Deserialize(dataPool, valueOffset, ref hideAllMapBlockCharacters);
			this.HideAllMapBlockCharacters = hideAllMapBlockCharacters;
			GEvent.OnEvent(UiEvents.WorldMapShowInfoStatusChange, null);
			break;
		}
		case 28:
		{
			bool needToNotifyNewMonth = false;
			Serializer.Deserialize(dataPool, valueOffset, ref needToNotifyNewMonth);
			this.NeedToNotifyNewMonth = needToNotifyNewMonth;
			break;
		}
		case 29:
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this.CommonOptionPreviewEventOptionInfos);
			bool exist = UIElement.EventWindow.Exist;
			if (exist)
			{
				GEvent.OnEvent(UiEvents.OnEventWindowDisplayDataChanged, null);
			}
			break;
		}
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0005375B File Offset: 0x0005195B
	private IEnumerator JieqingMaskCharacterListChanged(List<int> charIdlList)
	{
		ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
		argsBox.SetObject("JieqingMaskCharIdList", charIdlList);
		GEvent.OnEvent(UiEvents.OnJieqingMaskCharacterListChanged, argsBox);
		yield return new WaitForSeconds(0.3f);
		this._jieqingMaskCharIdList = charIdlList;
		GEvent.OnEvent(UiEvents.OnJieqingMaskCharacterListChanged, null);
		yield break;
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x00053774 File Offset: 0x00051974
	private void RefreshShowingEvent()
	{
		bool flag = this.DisplayingEventData == null;
		if (flag)
		{
			bool exist = UIElement.EventWindow.Exist;
			if (exist)
			{
				UIElement.EventWindow.UiBaseAs<ViewEventWindow>().HideSelf();
			}
			bool flag2 = this._showBlushCharIdList != null;
			if (flag2)
			{
				this._showBlushCharIdList.Clear();
			}
			GEvent.OnEvent(UiEvents.OnChangeTopUiInAdventure, null);
		}
		else
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				bool exist2 = UIElement.EventWindow.Exist;
				if (exist2)
				{
					GEvent.OnEvent(UiEvents.OnEventWindowDisplayDataChanged, null);
				}
				else
				{
					UIManager.Instance.MaskUI(UIElement.EventWindow);
				}
			});
		}
		this._displayDataDirtyFlag = false;
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x00053810 File Offset: 0x00051A10
	private void GetMethodReturnValue(ushort methodId, int valueOffset, RawDataPool dataPool)
	{
		bool flag = methodId == 11;
		if (flag)
		{
			List<TaiwuEventDisplayData> list = null;
			Serializer.Deserialize(dataPool, valueOffset, ref list);
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00053838 File Offset: 0x00051A38
	private string GetCharacterName(int charId, bool nickName = false)
	{
		try
		{
			bool flag = this.DisplayingEventData.NameDecodeDataList != null;
			if (flag)
			{
				int i = 0;
				int max = this.DisplayingEventData.NameDecodeDataList.Count;
				while (i < max)
				{
					bool flag2 = this.DisplayingEventData.NameDecodeDataList[i].CharacterId == charId;
					if (flag2)
					{
						bool isTaiwu = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == charId;
						NameRelatedData data = this.DisplayingEventData.NameDecodeDataList[i].NameRelatedData;
						ValueTuple<string, string> monasticTitleOrDisplayName = data.GetMonasticTitleOrDisplayName(isTaiwu);
						string surName = monasticTitleOrDisplayName.Item1;
						string givenName = monasticTitleOrDisplayName.Item2;
						if (!nickName)
						{
							return NameCenter.FormatName(surName, givenName);
						}
						bool flag3 = givenName.Length < 2;
						if (!flag3)
						{
							return givenName;
						}
						bool flag4 = data.Gender == 1;
						if (flag4)
						{
							return LocalStringManager.GetFormat(LanguageKey.LK_CharacterDefaultNickName_Man, givenName);
						}
						return LocalStringManager.GetFormat(LanguageKey.LK_CharacterDefaultNickName_Woman, givenName);
					}
					else
					{
						i++;
					}
				}
			}
		}
		catch (Exception e)
		{
			GLog.TagError("EventModel", string.Format("{0}:Decode {1} Name failed!\n{2}", this.DisplayingEventData.EventGuid, charId, e), Array.Empty<object>());
			throw;
		}
		return string.Format("<color=#red><Character Id = {0}></color>", charId);
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x000539AC File Offset: 0x00051BAC
	private void AdjustDataForDisplay()
	{
		TaiwuEventDisplayData displayingEventData = this.DisplayingEventData;
		bool flag = ((displayingEventData != null) ? displayingEventData.ExtraData : null) == null;
		if (!flag)
		{
			bool flag2 = this.DisplayingEventData.ExtraData.SelectOneAvatarRelatedDataList != null && this.DisplayingEventData.ExtraData.SelectOneAvatarRelatedDataList.Count <= 0;
			if (flag2)
			{
				this.DisplayingEventData.ExtraData.SelectOneAvatarRelatedDataList = null;
			}
			bool flag3 = this._showBlushCharIdList == null;
			if (flag3)
			{
				this._showBlushCharIdList = new List<int>();
			}
			this._showBlushCharIdList.Clear();
			bool flag4 = this.DisplayingEventData.ExtraData.MainRoleShyFlag && this.DisplayingEventData.MainCharacter != null;
			if (flag4)
			{
				this._showBlushCharIdList.Add(this.DisplayingEventData.MainCharacter.CharacterId);
			}
			bool flag5 = this.DisplayingEventData.ExtraData.TargetRoleShyFlag && this.DisplayingEventData.TargetCharacter != null;
			if (flag5)
			{
				this._showBlushCharIdList.Add(this.DisplayingEventData.TargetCharacter.CharacterId);
			}
			this.DisplayingEventData.EventContent = this.<AdjustDataForDisplay>g__HandleContentTag|100_0(this.DisplayingEventData.EventContent);
			int i = 0;
			for (;;)
			{
				int num = i;
				List<EventOptionInfo> eventOptionInfos = this.DisplayingEventData.EventOptionInfos;
				int? num2 = (eventOptionInfos != null) ? new int?(eventOptionInfos.Count) : null;
				if (!(num < num2.GetValueOrDefault() & num2 != null))
				{
					break;
				}
				EventOptionInfo optionInfo = this.DisplayingEventData.EventOptionInfos[i];
				optionInfo.OptionContent = this.<AdjustDataForDisplay>g__HandleContentTag|100_0(optionInfo.OptionContent);
				this.DisplayingEventData.EventOptionInfos[i] = optionInfo;
				bool flag6 = optionInfo.OptionAvailableConditions != null;
				if (flag6)
				{
					for (int j = 0; j < optionInfo.OptionAvailableConditions.Count; j++)
					{
						foreach (OptionAvailableInfoMinimumElement minimumElement in optionInfo.OptionAvailableConditions[j].Data)
						{
							int s = 0;
							for (;;)
							{
								int num3 = s;
								string[] formatArgs = minimumElement.FormatArgs;
								num2 = ((formatArgs != null) ? new int?(formatArgs.Length) : null);
								if (!(num3 < num2.GetValueOrDefault() & num2 != null))
								{
									break;
								}
								minimumElement.FormatArgs[s] = this.<AdjustDataForDisplay>g__HandleContentTag|100_0(minimumElement.FormatArgs[s]);
								s++;
							}
						}
					}
				}
				bool flag7 = optionInfo.OptionAvailableConditionInfos != null;
				if (flag7)
				{
					foreach (OptionAvailableConditionInfo condition in optionInfo.OptionAvailableConditionInfos)
					{
						bool flag8 = condition.Args == null;
						if (!flag8)
						{
							for (int argIndex = 0; argIndex < condition.Args.Length; argIndex++)
							{
								condition.Args[argIndex] = this.<AdjustDataForDisplay>g__HandleContentTag|100_0(condition.Args[argIndex]);
							}
						}
					}
				}
				i++;
			}
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			int k = 0;
			for (;;)
			{
				int num4 = k;
				List<EventOptionInfo> eventOptionInfos2 = this.DisplayingEventData.EventOptionInfos;
				int? num2 = (eventOptionInfos2 != null) ? new int?(eventOptionInfos2.Count) : null;
				if (!(num4 < num2.GetValueOrDefault() & num2 != null))
				{
					break;
				}
				builder.Append("<$new response dialog>");
				builder.Append(this.DisplayingEventData.EventOptionInfos[k].OptionKey);
				builder.Append("<$optionKey>");
				builder.Append(this.DisplayingEventData.EventOptionInfos[k].OptionContent);
				builder.Append("<$optionKey>");
				builder.Append(this.DisplayingEventData.EventOptionInfos[k].Behavior);
				k++;
			}
			CharacterDisplayData mainCharacter = this.DisplayingEventData.MainCharacter;
			int charId = (mainCharacter != null) ? mainCharacter.CharacterId : -1;
			CharacterDisplayData targetCharacter = this.DisplayingEventData.TargetCharacter;
			int charId2 = (targetCharacter != null) ? targetCharacter.CharacterId : -1;
			IntPair charIds = new IntPair(charId, charId2);
			string leftName = (this.DisplayingEventData.MainCharacter == null) ? this.GetCharacterNameTextFromExtraData(true) : this.GetCharacterNameText(true);
			string rightName = (this.DisplayingEventData.TargetCharacter == null) ? this.GetCharacterNameTextFromExtraData(false) : this.GetCharacterNameText(false);
			short merchantTemplateId = (this.DisplayingEventData.ExtraData.CaravanData == null) ? -1 : this.DisplayingEventData.ExtraData.CaravanData.MerchantTemplateId;
			TaiwuEventDomainMethod.Call.StartNewDialog(charIds, this.DisplayingEventData.EventContent ?? "", builder.ToString(), this.DisplayingEventData.ExtraData.LeftActorData, this.DisplayingEventData.ExtraData.ActorData, leftName, rightName, merchantTemplateId);
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00053EC0 File Offset: 0x000520C0
	private string GetCharacterNameTextFromExtraData(bool isLeftCharacter)
	{
		string result;
		if (!isLeftCharacter)
		{
			TaiwuEventDisplayData displayingEventData = this.DisplayingEventData;
			if (displayingEventData == null)
			{
				result = null;
			}
			else
			{
				TaiwuEventDisplayExtraData extraData = displayingEventData.ExtraData;
				if (extraData == null)
				{
					result = null;
				}
				else
				{
					EventActorData actorData = extraData.ActorData;
					result = ((actorData != null) ? actorData.DisplayName : null);
				}
			}
		}
		else
		{
			TaiwuEventDisplayData displayingEventData2 = this.DisplayingEventData;
			if (displayingEventData2 == null)
			{
				result = null;
			}
			else
			{
				TaiwuEventDisplayExtraData extraData2 = displayingEventData2.ExtraData;
				if (extraData2 == null)
				{
					result = null;
				}
				else
				{
					EventActorData leftActorData = extraData2.LeftActorData;
					result = ((leftActorData != null) ? leftActorData.DisplayName : null);
				}
			}
		}
		return result;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00053F2C File Offset: 0x0005212C
	public string GetCharacterNameText(bool isLeftCharacter)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		CharacterDisplayData character = isLeftCharacter ? this.DisplayingEventData.MainCharacter : this.DisplayingEventData.TargetCharacter;
		EventActorData actor = isLeftCharacter ? this.DisplayingEventData.ExtraData.LeftActorData : this.DisplayingEventData.ExtraData.ActorData;
		string roleNameKey = isLeftCharacter ? this.LeftRoleNameKey : this.RightRoleNameKey;
		bool isAlternativeName = isLeftCharacter ? this.DisplayingEventData.ExtraData.MainRoleUseAlternativeName : this.DisplayingEventData.ExtraData.TargetRoleUseAlternativeName;
		bool flag = isAlternativeName;
		string result;
		if (flag)
		{
			CharacterItem config = Character.Instance.GetItem(character.TemplateId);
			bool flag2 = config != null && !string.IsNullOrEmpty(config.AnonymousTitle);
			if (flag2)
			{
				result = config.AnonymousTitle;
			}
			else
			{
				result = string.Empty;
			}
		}
		else
		{
			bool flag3 = !string.IsNullOrEmpty(roleNameKey);
			if (flag3)
			{
				result = LocalStringManager.Get(roleNameKey);
			}
			else
			{
				bool flag4 = actor != null;
				if (flag4)
				{
					result = actor.DisplayName;
				}
				else
				{
					result = NameCenter.GetCharMonasticTitleOrNameByDisplayData(character, character.CharacterId == taiwuCharId, false);
				}
			}
		}
		return result;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x00054054 File Offset: 0x00052254
	private void HandleSelectInformationDataChange()
	{
		EventModel.<>c__DisplayClass103_0 CS$<>8__locals1 = new EventModel.<>c__DisplayClass103_0();
		CS$<>8__locals1.<>4__this = this;
		bool flag = this.SelectInformationData == null || !this.SelectInformationData.AvailableData;
		if (!flag)
		{
			CS$<>8__locals1.argBox = EasyPool.Get<ArgumentBox>();
			bool isForShopping = this.SelectInformationData.IsForShopping;
			if (isForShopping)
			{
				bool isTaiwu = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this.SelectInformationData.RelatedCharacterId;
				InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackage(null, this.SelectInformationData.ToSelectSecretInformationDataIdList, delegate(int offset, RawDataPool dataPool)
				{
					SecretInformationDisplayPackage package = null;
					Serializer.Deserialize(dataPool, offset, ref package);
					CS$<>8__locals1.argBox.SetObject("secretInformation", package);
					CS$<>8__locals1.argBox.SetObject("secretInformationIdList", CS$<>8__locals1.<>4__this.SelectInformationData.ToSelectSecretInformationDataIdList);
					CS$<>8__locals1.argBox.Set("characterId", CS$<>8__locals1.<>4__this.SelectInformationData.RelatedCharacterId);
					CS$<>8__locals1.argBox.Set("characterName", NameCenter.GetDisplayName(ref CS$<>8__locals1.<>4__this.SelectInformationData.CharacterNameRelatedData, isTaiwu));
					GEvent.OnEvent(UiEvents.EventWindowSelectSecretInformation, null);
					UIElement.SelectInformationForShopping.SetOnInitArgs(CS$<>8__locals1.argBox);
					UIManager.Instance.MaskUI(UIElement.SelectInformationForShopping);
				});
			}
			else
			{
				bool flag2 = this.SelectInformationData.ToSelectNormalInformation != null;
				if (!flag2)
				{
					bool flag3 = this.SelectInformationData.ToSelectSecretInformationDataIdList != null;
					if (!flag3)
					{
						throw new Exception("no information data set to SelectInformationData,select failed!");
					}
				}
				bool flag4 = this.SelectInformationData.ToSelectSecretInformationDataIdList != null && this.SelectInformationData.ToSelectSecretInformationDataIdList.Count > 0;
				if (flag4)
				{
					InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackage(null, this.SelectInformationData.ToSelectSecretInformationDataIdList, delegate(int offset, RawDataPool dataPool)
					{
						SecretInformationDisplayPackage package = null;
						Serializer.Deserialize(dataPool, offset, ref package);
						CS$<>8__locals1.argBox.SetObject("secretInformation", package);
						base.<HandleSelectInformationDataChange>g__OpenSelectInformationPage|1();
					});
				}
				else
				{
					CS$<>8__locals1.<HandleSelectInformationDataChange>g__OpenSelectInformationPage|1();
				}
			}
		}
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00054188 File Offset: 0x00052388
	private void HandleCricketBettingDataChange()
	{
		EventCricketBettingData cricketBettingData = this.CricketBettingData;
		bool flag = cricketBettingData == null || !cricketBettingData.IsValid;
		if (!flag)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CricketBettingData", this.CricketBettingData);
			UIElement.CricketBetting.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CricketBetting, true);
		}
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x000541E8 File Offset: 0x000523E8
	public bool IsEventCommonOptionUnlocked(EventCommonOptionItem config)
	{
		bool flag = config.RequiredTask >= 0 && !SingletonObject.getInstance<TaskModel>().IsTaskFinished(config.RequiredTask);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = config.TemplateId == 5;
			if (flag2)
			{
				TaiwuEventDisplayData displayingEventData = this.DisplayingEventData;
				bool flag3 = ((displayingEventData != null) ? displayingEventData.ExtraData : null) == null || !this.DisplayingEventData.ExtraData.ShowInteractOption;
				if (flag3)
				{
					return false;
				}
			}
			short areaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
			short templateId = config.TemplateId;
			if (!true)
			{
			}
			bool flag4;
			if (templateId != 0)
			{
				if (templateId != 1)
				{
					flag4 = (areaId != 135 && areaId != 137);
				}
				else
				{
					flag4 = (areaId != 135);
				}
			}
			else
			{
				flag4 = true;
			}
			if (!true)
			{
			}
			result = flag4;
		}
		return result;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x000542C0 File Offset: 0x000524C0
	private void LoadEventOptionTipsLanguageFile()
	{
		string optionAvailableLanguageFileName = "EventOptionTips_" + LocalStringManager.CurLanguageKey;
		string filePath = Path.Combine("RemakeResources/Data/Language_EventOptionTips", optionAvailableLanguageFileName);
		ResLoader.Load<TextAsset>(filePath, delegate(TextAsset asset)
		{
			this._optionAvailableContents = asset.text.Split('\n', StringSplitOptions.None);
		}, delegate(string error)
		{
			string defaultFileName = "EventOptionTips_CN";
			string defaultFilePath = Path.Combine("RemakeResources/Data/Language_EventOptionTips", defaultFileName);
			ResLoader.Load<TextAsset>(defaultFilePath, delegate(TextAsset asset)
			{
				this._optionAvailableContents = asset.text.Split('\n', StringSplitOptions.None);
			}, null, false);
		}, false);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0005430C File Offset: 0x0005250C
	public static void RegisterCustomEventOptionAvailableContentHandler(Func<short, string> handler)
	{
		bool flag = handler == null;
		if (!flag)
		{
			EventModel._customOptionAvailableHandler = (Func<short, string>)Delegate.Remove(EventModel._customOptionAvailableHandler, handler);
			EventModel._customOptionAvailableHandler = (Func<short, string>)Delegate.Combine(EventModel._customOptionAvailableHandler, handler);
		}
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0005459C File Offset: 0x0005279C
	[CompilerGenerated]
	internal static void <SetSelectItemResult>g__SendPoisonItemToBackend|64_0(ITradeableContent itemDisplayData, string key, ref EventModel.<>c__DisplayClass64_0 A_2)
	{
		Inventory poisonItems = itemDisplayData.GetOperationInventoryFromPool(1, false);
		TaiwuEventDomainMethod.Call.SetItemSelectResult(key, poisonItems.Items.Keys.First<ItemKey>(), A_2.filterList.Count == 0);
		itemDisplayData.ChangeAmount(poisonItems, false);
		ItemDisplayData.ReturnInventoryToPool(poisonItems);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x000545E8 File Offset: 0x000527E8
	[CompilerGenerated]
	private string <AdjustDataForDisplay>g__HandleContentTag|100_0(string src)
	{
		Regex langRegex = new Regex("<Language Key( +)?=( +)?(?<langKey>[A-Z|a-z|0-9|_]+)( +)?/>");
		bool flag = !string.IsNullOrEmpty(src) && langRegex.IsMatch(src);
		if (flag)
		{
			src = langRegex.Replace(src, (Match match) => LocalStringManager.Get(match.Groups["langKey"].Value));
		}
		Regex langNumRegex = new Regex("<Language Num( +)?=( +)?(?<langNum>-?[0-9]+)( +)?/>");
		bool flag2 = !string.IsNullOrEmpty(src) && langNumRegex.IsMatch(src);
		if (flag2)
		{
			src = langNumRegex.Replace(src, (Match match) => LocalStringManager.GetLanguageNumber(long.Parse(match.Groups["langNum"].Value)));
		}
		Regex charNameRegex = new Regex("<CharName Id=(?<IdStr>[0-9]+) />");
		bool flag3 = !string.IsNullOrEmpty(src) && charNameRegex.IsMatch(src);
		if (flag3)
		{
			src = charNameRegex.Replace(src, delegate(Match match)
			{
				string idString = match.Groups["IdStr"].Value;
				int charId;
				int.TryParse(idString, out charId);
				return this.GetCharacterName(charId, false);
			});
		}
		Regex charNickNameRegex = new Regex("<CharNickname Id=(?<IdStr>[0-9]+) />");
		bool flag4 = !string.IsNullOrEmpty(src) && charNickNameRegex.IsMatch(src);
		if (flag4)
		{
			src = charNickNameRegex.Replace(src, delegate(Match match)
			{
				string idString = match.Groups["IdStr"].Value;
				int charId;
				int.TryParse(idString, out charId);
				return this.GetCharacterName(charId, true);
			});
		}
		Regex cricketRegex = new Regex("<Cricket (part=(?<partId>[0-9|_]+))? (color=(?<colorId>[0-9|_]+))?/>");
		bool flag5 = !string.IsNullOrEmpty(src) && cricketRegex.IsMatch(src);
		if (flag5)
		{
			src = cricketRegex.Replace(src, delegate(Match match)
			{
				short partId;
				short colorId;
				bool flag6 = short.TryParse(match.Groups["partId"].Value, out partId) && short.TryParse(match.Groups["colorId"].Value, out colorId);
				string result;
				if (flag6)
				{
					result = new ValueTuple<short, short>(colorId, partId).CalcCricketName();
				}
				else
				{
					result = "{cricket decode error:frontend}";
				}
				return result;
			});
		}
		return src;
	}

	// Token: 0x04000DB6 RID: 3510
	public static bool IgnoreEventBehavior;

	// Token: 0x04000DB7 RID: 3511
	private string[] _optionAvailableContents;

	// Token: 0x04000DB8 RID: 3512
	private static Func<short, string> _customOptionAvailableHandler;

	// Token: 0x04000DB9 RID: 3513
	private List<EventNotifyData> _notifyInfoList;

	// Token: 0x04000DBA RID: 3514
	public TaiwuEventDisplayData DisplayingEventData;

	// Token: 0x04000DBB RID: 3515
	public List<EventOptionInfo> CommonOptionPreviewEventOptionInfos;

	// Token: 0x04000DBC RID: 3516
	public ValueTuple<bool, string, float> EventCgTextureData = new ValueTuple<bool, string, float>(true, string.Empty, 0f);

	// Token: 0x04000DBD RID: 3517
	public EventSelectInformationData SelectInformationData;

	// Token: 0x04000DBE RID: 3518
	public EventCricketBettingData CricketBettingData;

	// Token: 0x04000DBF RID: 3519
	public EventSelectCombatSkillData SelectCombatSkillData;

	// Token: 0x04000DC0 RID: 3520
	public EventSelectLifeSkillData SelectLifeSkillData;

	// Token: 0x04000DC1 RID: 3521
	public ItemDisplayData[] LeftItemList;

	// Token: 0x04000DC2 RID: 3522
	public ItemDisplayData[] RightItemList;

	// Token: 0x04000DC3 RID: 3523
	public List<sbyte> CoverCricketJarGradeList;

	// Token: 0x04000DC4 RID: 3524
	public bool ShowItemWithCricketBattleGuess;

	// Token: 0x04000DC5 RID: 3525
	private int _listenerId = -1;

	// Token: 0x04000DCE RID: 3534
	private List<int> _marriageLook1CharIdList;

	// Token: 0x04000DCF RID: 3535
	private List<int> _marriageLook2CharIdList;

	// Token: 0x04000DD0 RID: 3536
	private List<int> _jieqingMaskCharIdList;

	// Token: 0x04000DD1 RID: 3537
	private List<int> _showBlushCharIdList;

	// Token: 0x04000DD2 RID: 3538
	private List<int> _shixiangBarbarianMasterIdList;

	// Token: 0x04000DD3 RID: 3539
	private short _taiwuClothingTemplateId = -1;

	// Token: 0x04000DD4 RID: 3540
	private bool _displayDataDirtyFlag;

	// Token: 0x04000DD5 RID: 3541
	public static Dictionary<string, short> EventOptionTipInfo = new Dictionary<string, short>();

	// Token: 0x04000DD6 RID: 3542
	public static readonly string[] CanOperateEventArray = new string[]
	{
		"05e87c45-f14e-49ef-8769-cbaced4753ae",
		"9dce4f27-347c-4588-9be4-08c1c7f1f4a3",
		"a9d0bcd8-e378-4ee9-96a6-1e5b9db17371",
		"bad63f08-115a-45aa-970c-fa203dd85e2b",
		"7c70ce0c-577a-4049-bcad-e593c63d62d4",
		"fb38f657-6ed0-41e4-a0c2-c82afb49762f"
	};

	// Token: 0x04000DD7 RID: 3543
	public static readonly string[][] XiangShuAvatarDisplayTextures = new string[][]
	{
		new string[]
		{
			"NpcFace_monv",
			"NpcFace_xiaomonv_happy",
			"NpcFace_xiaomonv_sad",
			"NpcFace_monv_happy",
			"NpcFace_monv_sad"
		},
		new string[]
		{
			"NpcFace_dayueyaochang",
			"NpcFace_xiaodayueyaochang_happy",
			"NpcFace_xiaodayueyaochang_sad",
			"NpcFace_dayueyaochang_happy",
			"NpcFace_dayueyaochang_sad"
		},
		new string[]
		{
			"NpcFace_jiuhan",
			"NpcFace_xiaojiuhan_happy",
			"NpcFace_xiaojiuhan_sad",
			"NpcFace_jiuhan_happy",
			"NpcFace_jiuhan_sad"
		},
		new string[]
		{
			"NpcFace_jinhuanger",
			"NpcFace_xiaojinhuanger_happy",
			"NpcFace_xiaojinhuanger_sad",
			"NpcFace_jinhuanger_happy",
			"NpcFace_jinhuanger_sad"
		},
		new string[]
		{
			"NpcFace_yiyihou",
			"NpcFace_xiaoyiyihou_happy",
			"NpcFace_xiaoyiyihou_sad",
			"NpcFace_yiyihou_happy",
			"NpcFace_yiyihou_sad"
		},
		new string[]
		{
			"NpcFace_weiqi",
			"NpcFace_xiaoweiqi_happy",
			"NpcFace_xiaoweiqi_sad",
			"NpcFace_weiqi_happy",
			"NpcFace_weiqi_sad"
		},
		new string[]
		{
			"NpcFace_yixiang",
			"NpcFace_xiaoyixiang_happy",
			"NpcFace_xiaoyixiang_sad",
			"NpcFace_yixiang_happy",
			"NpcFace_yixiang_sad"
		},
		new string[]
		{
			"NpcFace_xuefeng",
			"NpcFace_xiaoxuefeng_happy",
			"NpcFace_xiaoxuefeng_sad",
			"NpcFace_xuefeng_happy",
			"NpcFace_xuefeng_sad"
		},
		new string[]
		{
			"NpcFace_shufang",
			"NpcFace_xiaoshufang_happy",
			"NpcFace_xiaoshufang_sad",
			"NpcFace_shufang_happy",
			"NpcFace_shufang_sad"
		}
	};
}
