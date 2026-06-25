using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000653 RID: 1619
	public class EventGroupTreeView : EventEditorSubPageBase
	{
		// Token: 0x06004D0B RID: 19723 RVA: 0x00245AA4 File Offset: 0x00243CA4
		public static void Init(EventGroupTreeView instance)
		{
			EventGroupTreeView.Instance = instance;
			EventGroupTreeView.Instance.InternalInit();
		}

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x06004D0C RID: 19724 RVA: 0x00245AB8 File Offset: 0x00243CB8
		// (set) Token: 0x06004D0D RID: 19725 RVA: 0x00245AC0 File Offset: 0x00243CC0
		public EventGroupData EditingEventGroup { get; set; }

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x06004D0E RID: 19726 RVA: 0x00245AC9 File Offset: 0x00243CC9
		private EventEditorModel Model
		{
			get
			{
				return SingletonObject.getInstance<EventEditorModel>();
			}
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x00245AD0 File Offset: 0x00243CD0
		protected override void InternalInit()
		{
			PoolManager.SetSrcObject("EventGroupTreeView_ColumnPrefab", this.goEventColumnPrefab);
			PoolManager.SetSrcObject("EventGroupTreeView_TitlePrefab", this.titlePrefab);
			PoolManager.SetSrcObject("EventGroupTreeView_EventPrefab", this.eventPrefab);
			EventGroupTreeView._groupTitleNameBackColor = "5D7482".HexStringToColor();
			EventGroupTreeView._toEventTitleNameBackColor = "C2CB65".HexStringToColor();
			EventGroupTreeView._eventOptionNextBackColor = "7E504D".HexStringToColor();
			this.InitComponents();
			this.InitAllEventGroupData();
			this._treeColumnDataList = new List<TreeColumnData>();
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x00245B57 File Offset: 0x00243D57
		public override void Show()
		{
			this.groupList.localScale = Vector3.one;
			this.InitAllEventGroupData();
			this.RefreshGroupList();
			this.OnSwitchEventGroup();
		}

		// Token: 0x06004D11 RID: 19729 RVA: 0x00245B80 File Offset: 0x00243D80
		public override void Hide()
		{
		}

		// Token: 0x06004D12 RID: 19730 RVA: 0x00245B83 File Offset: 0x00243D83
		private void OnDestroy()
		{
			PoolManager.RemoveData("EventGroupTreeView_ColumnPrefab");
			PoolManager.RemoveData("EventGroupTreeView_TitlePrefab");
			PoolManager.RemoveData("EventGroupTreeView_EventPrefab");
		}

		// Token: 0x06004D13 RID: 19731 RVA: 0x00245BA7 File Offset: 0x00243DA7
		public IEnumerable<string> GetShowingEventGroups()
		{
			List<EventGroupData> showingGroupDataList = this._showingGroupDataList;
			bool flag = showingGroupDataList == null || showingGroupDataList.Count <= 0;
			if (flag)
			{
				yield break;
			}
			foreach (EventGroupData groupData in this._showingGroupDataList)
			{
				bool exportFlag = groupData.ExportFlag;
				if (exportFlag)
				{
					yield return groupData.Key;
				}
				groupData = null;
			}
			List<EventGroupData>.Enumerator enumerator = default(List<EventGroupData>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x00245BB8 File Offset: 0x00243DB8
		public void RefreshToEventGroup(EventGroupData groupData)
		{
			bool flag = this.EditingEventGroup == groupData;
			if (!flag)
			{
				this.EditingEventGroup = groupData;
				this.InitAllEventGroupData();
				this.RefreshGroupList();
			}
		}

		// Token: 0x06004D15 RID: 19733 RVA: 0x00245BEC File Offset: 0x00243DEC
		private void InitAllEventGroupData()
		{
			if (this._allGroupDataList == null)
			{
				this._allGroupDataList = new List<EventGroupData>();
			}
			List<string> groupKeyList = this.Model.GetAllGroupDataKeyList();
			bool flag = groupKeyList == null;
			if (!flag)
			{
				groupKeyList.Sort();
				bool flag2 = this.EditingEventGroup == null;
				if (flag2)
				{
					string lastEditEventGroup = PlayerPrefs.GetString("LastEditingEventGroupKey", string.Empty);
					bool flag3 = string.IsNullOrEmpty(lastEditEventGroup) && groupKeyList.Count > 0;
					if (flag3)
					{
						lastEditEventGroup = groupKeyList[0];
					}
					bool flag4 = !string.IsNullOrEmpty(lastEditEventGroup) && groupKeyList.Contains(lastEditEventGroup);
					if (flag4)
					{
						this.EditingEventGroup = this.Model.GetGroupData(lastEditEventGroup);
					}
				}
				bool flag5 = groupKeyList.Count == this._allGroupDataList.Count;
				if (!flag5)
				{
					this._allGroupDataList.Clear();
					foreach (string t in groupKeyList)
					{
						this._allGroupDataList.Add(this.Model.GetGroupData(t));
					}
				}
			}
		}

		// Token: 0x06004D16 RID: 19734 RVA: 0x00245D1C File Offset: 0x00243F1C
		private void InitComponents()
		{
			this._currentClickGroupItemIndex = -1;
			this.eventGroupScrollView.OnItemRender += this.OnGroupItemRender;
			this.inputField.text = PlayerPrefs.GetString("LastEventGroupSearchKey", string.Empty);
			this.inputField.onValueChanged.AddListener(delegate(string _)
			{
				this.RefreshGroupList();
			});
		}

		// Token: 0x06004D17 RID: 19735 RVA: 0x00245D84 File Offset: 0x00243F84
		private void RefreshGroupList()
		{
			bool flag = this._showingGroupDataList == null;
			if (flag)
			{
				this._showingGroupDataList = new List<EventGroupData>();
			}
			else
			{
				this._showingGroupDataList.Clear();
			}
			string searchKey = this.inputField.text;
			this._showingGroupDataList = SingletonObject.getInstance<EventEditorModel>().SearchEventGroup(searchKey, null);
			PlayerPrefs.SetString("LastEventGroupSearchKey", searchKey);
			InfinityScroll scroll = this.eventGroupScrollView;
			scroll.UpdateData(this._showingGroupDataList.Count);
			bool flag2 = this.EditingEventGroup != null;
			if (flag2)
			{
				this.EditingEventGroup = SingletonObject.getInstance<EventEditorModel>().GetGroupData(this.EditingEventGroup.Key);
			}
			this._currentClickGroupItemIndex = this._showingGroupDataList.IndexOf(this.EditingEventGroup);
			scroll.Refresh(this._currentClickGroupItemIndex);
		}

		// Token: 0x06004D18 RID: 19736 RVA: 0x00245E48 File Offset: 0x00244048
		private void OnGroupItemRender(int index, GameObject go)
		{
			EventGroupData data = this._showingGroupDataList[index];
			EventGroupTreeViewGroupPrefabInfo itemInfo = go.GetComponent<EventGroupTreeViewGroupPrefabInfo>();
			itemInfo.txtMeshGroupInfo.text = data.Name;
			itemInfo.goExportFlag.SetActive(data.ExportFlag);
			itemInfo.btnExportSwitch.ClearAndAddListener(delegate
			{
				data.ExportFlag = !data.ExportFlag;
				data.SaveGroup();
				this.eventGroupScrollView.RefreshCell(index);
			});
			TooltipInvoker displayer = itemInfo.exportMouseTip;
			TooltipInvoker tooltipInvoker = displayer;
			string[] presetParam;
			if (!data.ExportFlag)
			{
				(presetParam = new string[1])[0] = LocalStringManager.Get(LanguageKey.UI_EventEditor_EventGroupExportNo);
			}
			else
			{
				(presetParam = new string[1])[0] = LocalStringManager.Get(LanguageKey.UI_EventEditor_EventGroupExportYes);
			}
			tooltipInvoker.PresetParam = presetParam;
			itemInfo.goSelected.SetActive(index == this._currentClickGroupItemIndex);
			PointClickBridge bridge = itemInfo.clickBridge;
			bridge.OnLeftClick = delegate()
			{
				this.OnGroupItemLeftClick(index);
			};
			bridge.OnRightClick = delegate()
			{
				this.OnGroupItemRightClick(index);
			};
			bridge.OnDoubleClick = delegate()
			{
				this.OnGroupItemDoubleClick(data);
			};
		}

		// Token: 0x06004D19 RID: 19737 RVA: 0x00245F68 File Offset: 0x00244168
		private void OnGroupItemLeftClick(int index)
		{
			InfinityScroll scroll = this.eventGroupScrollView;
			int prevIndex = this._currentClickGroupItemIndex;
			this._currentClickGroupItemIndex = index;
			bool flag = prevIndex >= 0;
			if (flag)
			{
				scroll.RefreshCell(prevIndex);
			}
			scroll.RefreshCell(index);
			this.EditingEventGroup = this._showingGroupDataList[index];
			PlayerPrefs.SetString("LastEditingEventGroupKey", this.EditingEventGroup.Key);
			this.OnSwitchEventGroup();
			this.RefreshTree(-1);
		}

		// Token: 0x06004D1A RID: 19738 RVA: 0x00245FE0 File Offset: 0x002441E0
		private void OnGroupItemRightClick(int index)
		{
			this.OnGroupItemLeftClick(index);
			List<SheetButtonInfo> buttons = new List<SheetButtonInfo>();
			string btnText = LocalStringManager.Get(LanguageKey.LK_EventEditor_TestTargetEventGroup);
			buttons.Add(new SheetButtonInfo(btnText, new Action(this.OnQuickTestEventGroup), true, ""));
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", buttons);
			argBox.SetObject("ButtonTextHandler", UI_ButtonSheet.HandleButtonLabelContent);
			argBox.SetObject("ButtonSize", new Vector2(200f, 45f));
			UIElement.ButtonSheet.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x00246085 File Offset: 0x00244285
		private void OnGroupItemDoubleClick(EventGroupData groupData)
		{
			this.EditingEventGroup = groupData;
			PlayerPrefs.SetString("LastSelectAudioKey", groupData.Key);
			TaskControlPanel.Instance.OnEventEditorClick();
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x002460AC File Offset: 0x002442AC
		private void OnQuickTestEventGroup()
		{
			SingletonObject.getInstance<EventEditorModel>().QuickTestEventGroup(this.EditingEventGroup);
			GEvent.AddOneShot(ModEditorEvents.EventCompileComplete, new GEvent.Callback(this.OnCompileComplete));
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x002460D8 File Offset: 0x002442D8
		private void OnCompileComplete(ArgumentBox argBox)
		{
			EventEditorNotes.Instance.AddNote(LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_TestTargetEventGroup_Ready, this.EditingEventGroup.Name));
		}

		// Token: 0x06004D1E RID: 19742 RVA: 0x002460FC File Offset: 0x002442FC
		private void ReleaseColumnItem(EventGroupTreeViewEventColumnPrefabInfo columnInfo)
		{
			EventGroupTreeView.<>c__DisplayClass41_0 CS$<>8__locals1;
			CS$<>8__locals1.columnInfo = columnInfo;
			EventGroupTreeView.<ReleaseColumnItem>g__DestroyTarget|41_0<EventGroupTreeViewTitlePrefabInfo>("EventGroupTreeView_TitlePrefab", "Title_", ref CS$<>8__locals1);
			EventGroupTreeView.<ReleaseColumnItem>g__DestroyTarget|41_0<EventGroupTreeViewEventPrefabInfo>("EventGroupTreeView_EventPrefab", "Event_", ref CS$<>8__locals1);
			PoolManager.Destroy("EventGroupTreeView_ColumnPrefab", CS$<>8__locals1.columnInfo.gameObject);
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x00246150 File Offset: 0x00244350
		private void OnSwitchEventGroup()
		{
			foreach (TreeColumnData element in this._treeColumnDataList)
			{
				element.EventGroupData = null;
				element.SelectingEvent = string.Empty;
				List<ValueTuple<string, List<ValueTuple<string, string, string>>>> optionOnSelectToEventList = element.OptionOnSelectToEventList;
				if (optionOnSelectToEventList != null)
				{
					optionOnSelectToEventList.Clear();
				}
				List<ValueTuple<string, string, string>> jumpToEventList = element.JumpToEventList;
				if (jumpToEventList != null)
				{
					jumpToEventList.Clear();
				}
				EasyPool.Free<TreeColumnData>(element);
			}
			this._treeColumnDataList.Clear();
			bool flag = this.EditingEventGroup != null;
			if (flag)
			{
				TreeColumnData data = EasyPool.Get<TreeColumnData>();
				data.Depth = 0;
				data.EventGroupData = this.EditingEventGroup;
				data.SelectingEvent = string.Empty;
				List<ValueTuple<string, List<ValueTuple<string, string, string>>>> optionOnSelectToEventList2 = data.OptionOnSelectToEventList;
				if (optionOnSelectToEventList2 != null)
				{
					optionOnSelectToEventList2.Clear();
				}
				TreeColumnData treeColumnData = data;
				if (treeColumnData.JumpToEventList == null)
				{
					treeColumnData.JumpToEventList = new List<ValueTuple<string, string, string>>();
				}
				List<ValueTuple<string, string, string>> eventInfoList = this.EditingEventGroup.GetDisplayList(false);
				foreach (ValueTuple<string, string, string> tuple in eventInfoList)
				{
					EventEditorData eventData = this.EditingEventGroup.GetEvent(tuple.Item1);
					bool flag2 = eventData != null && eventData.TriggerType != "None";
					if (flag2)
					{
						data.JumpToEventList.Add(tuple);
					}
				}
				bool flag3 = data.JumpToEventList.Count <= 0 && eventInfoList.Count > 0;
				if (flag3)
				{
					bool flag4 = eventInfoList.Count == 1;
					if (flag4)
					{
						data.JumpToEventList.AddRange(eventInfoList);
					}
					else
					{
						List<string> possibleHeadEventList = eventInfoList.ConvertAll<string>((ValueTuple<string, string, string> e) => e.Item1);
						List<ValueTuple<string, string, string>> toEventList = new List<ValueTuple<string, string, string>>();
						List<ValueTuple<string, List<ValueTuple<string, string, string>>>> onSelectToEventList = new List<ValueTuple<string, List<ValueTuple<string, string, string>>>>();
						foreach (ValueTuple<string, string, string> valueTuple in eventInfoList)
						{
							string guid = valueTuple.Item1;
							toEventList.Clear();
							onSelectToEventList.Clear();
							SingletonObject.getInstance<EventEditorModel>().GetEventToEventList(guid, toEventList, onSelectToEventList);
							onSelectToEventList.Add(new ValueTuple<string, List<ValueTuple<string, string, string>>>("ToEvent", toEventList));
							for (int i = 0; i < onSelectToEventList.Count; i++)
							{
								List<ValueTuple<string, string, string>> list = onSelectToEventList[i].Item2;
								for (int j = 0; j < list.Count; j++)
								{
									possibleHeadEventList.Remove(list[j].Item1);
								}
							}
						}
						bool flag5 = possibleHeadEventList.Count > 0;
						if (flag5)
						{
							data.JumpToEventList.AddRange(eventInfoList.FindAll((ValueTuple<string, string, string> e) => possibleHeadEventList.Contains(e.Item1)));
						}
						else
						{
							data.JumpToEventList = eventInfoList;
						}
					}
				}
				bool flag6 = data.JumpToEventList.Count > 0;
				if (flag6)
				{
					this._treeColumnDataList.Add(data);
				}
			}
			this.RefreshTree(-1);
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x002464B0 File Offset: 0x002446B0
		private void RefreshTree(int fromIndex = -1)
		{
			RectTransform columnRoot = this.content;
			List<EventGroupTreeViewEventColumnPrefabInfo> columnInfoList = new List<EventGroupTreeViewEventColumnPrefabInfo>(columnRoot.GetComponentsInTopChildren(false));
			int i;
			int j;
			for (i = 0; i < this._treeColumnDataList.Count; i = j)
			{
				EventGroupTreeViewEventColumnPrefabInfo columnInfo = columnInfoList.Find((EventGroupTreeViewEventColumnPrefabInfo e) => e.gameObject.name == string.Format("Column_{0}", this._treeColumnDataList[i].Depth));
				bool flag = columnInfo != null;
				if (flag)
				{
					columnInfoList.Remove(columnInfo);
				}
				else
				{
					columnInfo = PoolManager.GetObject<EventGroupTreeViewEventColumnPrefabInfo>("EventGroupTreeView_ColumnPrefab");
					columnInfo.transform.SetParent(columnRoot, false);
				}
				bool flag2 = fromIndex >= 0 && i < fromIndex;
				if (!flag2)
				{
					columnInfo.transform.SetAsLastSibling();
					this.RefreshColumnItem(columnInfo, this._treeColumnDataList[i]);
				}
				j = i + 1;
			}
			foreach (EventGroupTreeViewEventColumnPrefabInfo columnInfo2 in columnInfoList)
			{
				this.ReleaseColumnItem(columnInfo2);
			}
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x002465E8 File Offset: 0x002447E8
		private void RefreshColumnItem(EventGroupTreeViewEventColumnPrefabInfo columnInfo, TreeColumnData data)
		{
			columnInfo.gameObject.name = string.Format("Column_{0}", data.Depth);
			EventGroupTreeView.<>c__DisplayClass44_0 CS$<>8__locals1;
			CS$<>8__locals1.rootTrans = columnInfo.content;
			List<EventGroupTreeViewTitlePrefabInfo> titlePrefabInfos = EventGroupTreeView.<RefreshColumnItem>g__GetPrefabInfos|44_0<EventGroupTreeViewTitlePrefabInfo>("Title_", ref CS$<>8__locals1);
			List<EventGroupTreeViewEventPrefabInfo> eventPrefabInfos = EventGroupTreeView.<RefreshColumnItem>g__GetPrefabInfos|44_0<EventGroupTreeViewEventPrefabInfo>("Event_", ref CS$<>8__locals1);
			int titleIndex = 0;
			int eventIndex = 0;
			bool flag = data.Depth == 0;
			if (flag)
			{
				bool flag2 = data.JumpToEventList == null || data.JumpToEventList.Count <= 0;
				if (flag2)
				{
					return;
				}
				EventGroupTreeViewTitlePrefabInfo titleInfo = EventGroupTreeView.<RefreshColumnItem>g__GetTargetInfo|44_1<EventGroupTreeViewTitlePrefabInfo>(titlePrefabInfos, "EventGroupTreeView_TitlePrefab", ref titleIndex, ref CS$<>8__locals1);
				titleInfo.gameObject.name = "Title_" + this.EditingEventGroup.Key;
				titleInfo.img.color = EventGroupTreeView._groupTitleNameBackColor;
				titleInfo.txtMeshContent.text = this.EditingEventGroup.Name;
				foreach (ValueTuple<string, string, string> evt in data.JumpToEventList)
				{
					EventGroupTreeViewEventPrefabInfo eventInfo = EventGroupTreeView.<RefreshColumnItem>g__GetTargetInfo|44_1<EventGroupTreeViewEventPrefabInfo>(eventPrefabInfos, "EventGroupTreeView_EventPrefab", ref eventIndex, ref CS$<>8__locals1);
					this.RefreshEventItem(eventInfo, evt, (int)data.Depth);
				}
			}
			else
			{
				TreeColumnData parentData = this._treeColumnDataList[(int)(data.Depth - 1)];
				EventEditorData parentEventData = SingletonObject.getInstance<EventEditorModel>().GetEvent(parentData.SelectingEvent);
				string parentEventName = parentEventData.EventName;
				List<ValueTuple<string, string, string>> jumpToEventList = data.JumpToEventList;
				bool flag3 = jumpToEventList != null && jumpToEventList.Count > 0;
				if (flag3)
				{
					EventGroupTreeViewTitlePrefabInfo toEventTitleInfo = EventGroupTreeView.<RefreshColumnItem>g__GetTargetInfo|44_1<EventGroupTreeViewTitlePrefabInfo>(titlePrefabInfos, "EventGroupTreeView_TitlePrefab", ref titleIndex, ref CS$<>8__locals1);
					toEventTitleInfo.gameObject.name = "Title_ToEvent";
					toEventTitleInfo.img.color = EventGroupTreeView._toEventTitleNameBackColor;
					toEventTitleInfo.txtMeshContent.text = parentEventName + " ToEventList:";
					foreach (ValueTuple<string, string, string> evt2 in data.JumpToEventList)
					{
						EventGroupTreeViewEventPrefabInfo eventInfo2 = EventGroupTreeView.<RefreshColumnItem>g__GetTargetInfo|44_1<EventGroupTreeViewEventPrefabInfo>(eventPrefabInfos, "EventGroupTreeView_EventPrefab", ref eventIndex, ref CS$<>8__locals1);
						this.RefreshEventItem(eventInfo2, evt2, (int)data.Depth);
					}
				}
				List<ValueTuple<string, List<ValueTuple<string, string, string>>>> optionOnSelectToEventList = data.OptionOnSelectToEventList;
				bool flag4 = optionOnSelectToEventList != null && optionOnSelectToEventList.Count > 0;
				if (flag4)
				{
					for (int i = 0; i < data.OptionOnSelectToEventList.Count; i++)
					{
						List<ValueTuple<string, string, string>> list = data.OptionOnSelectToEventList[i].Item2;
						bool flag5 = list != null && list.Count > 0;
						if (flag5)
						{
							EventGroupTreeViewTitlePrefabInfo optionTitleInfo = EventGroupTreeView.<RefreshColumnItem>g__GetTargetInfo|44_1<EventGroupTreeViewTitlePrefabInfo>(titlePrefabInfos, "EventGroupTreeView_TitlePrefab", ref titleIndex, ref CS$<>8__locals1);
							optionTitleInfo.gameObject.name = string.Format("Title_Option_{0}", i + 1);
							optionTitleInfo.img.color = EventGroupTreeView._eventOptionNextBackColor;
							optionTitleInfo.txtMeshContent.text = string.Format("{0} Option {1} OnSelect:", parentEventName, i + 1);
							foreach (ValueTuple<string, string, string> tuple in list)
							{
								EventGroupTreeViewEventPrefabInfo eventInfo3 = EventGroupTreeView.<RefreshColumnItem>g__GetTargetInfo|44_1<EventGroupTreeViewEventPrefabInfo>(eventPrefabInfos, "EventGroupTreeView_EventPrefab", ref eventIndex, ref CS$<>8__locals1);
								this.RefreshEventItem(eventInfo3, tuple, (int)data.Depth);
							}
						}
					}
				}
			}
			while (titleIndex < titlePrefabInfos.Count)
			{
				PoolManager.Destroy("EventGroupTreeView_TitlePrefab", titlePrefabInfos[titleIndex].gameObject);
				titleIndex++;
			}
			while (eventIndex < eventPrefabInfos.Count)
			{
				PoolManager.Destroy("EventGroupTreeView_EventPrefab", eventPrefabInfos[eventIndex].gameObject);
				eventIndex++;
			}
			EasyPool.Free<List<EventGroupTreeViewTitlePrefabInfo>>(titlePrefabInfos);
			EasyPool.Free<List<EventGroupTreeViewEventPrefabInfo>>(eventPrefabInfos);
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x00246A00 File Offset: 0x00244C00
		private void RefreshEventItem(EventGroupTreeViewEventPrefabInfo eventInfo, [TupleElementNames(new string[]
		{
			"guid",
			"eventName",
			"eventContent"
		})] ValueTuple<string, string, string> tuple, int depth)
		{
			TreeColumnData data = this._treeColumnDataList[depth];
			eventInfo.gameObject.name = "Event_" + tuple.Item1;
			eventInfo.goSelect.SetActive(tuple.Item1 == data.SelectingEvent);
			eventInfo.txtMeshEventName.text = tuple.Item2;
			eventInfo.content.enabled = !string.IsNullOrEmpty(tuple.Item3);
			eventInfo.content.PresetParam = new string[]
			{
				tuple.Item3
			};
			eventInfo.error.gameObject.SetActive(false);
			PointClickBridge clickBridge = eventInfo.clickBridge;
			clickBridge.OnLeftClick = delegate()
			{
				this.OnEventLeftClick(data, tuple.Item1);
			};
			clickBridge.OnDoubleClick = delegate()
			{
				EventGroupTreeView.OnEventDoubleClick(data, tuple.Item1);
			};
		}

		// Token: 0x06004D23 RID: 19747 RVA: 0x00246B10 File Offset: 0x00244D10
		private void OnEventLeftClick(TreeColumnData data, string eventGuid)
		{
			bool flag;
			if (this._treeColumnDataList.Count > 1)
			{
				List<TreeColumnData> treeColumnDataList = this._treeColumnDataList;
				flag = (data != treeColumnDataList[treeColumnDataList.Count - 1]);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._treeColumnDataList.RemoveRange((int)(data.Depth + 1), this._treeColumnDataList.Count - (int)data.Depth - 1);
			}
			CScrollbar bar = this.horizontalScrollBar;
			data.SelectingEvent = eventGuid;
			int prevIndex = -1;
			for (int i = 0; i < (int)data.Depth; i++)
			{
				TreeColumnData prevData = this._treeColumnDataList[i];
				for (int j = 0; j < prevData.JumpToEventList.Count; j++)
				{
					bool flag3 = prevData.JumpToEventList[j].Item1 == eventGuid;
					if (flag3)
					{
						prevIndex = i;
						prevData.SelectingEvent = eventGuid;
						break;
					}
				}
				bool flag4 = -1 == prevIndex && prevData.OptionOnSelectToEventList != null;
				if (flag4)
				{
					for (int k = 0; k < prevData.OptionOnSelectToEventList.Count; k++)
					{
						foreach (ValueTuple<string, string, string> valueTuple in prevData.OptionOnSelectToEventList[k].Item2)
						{
							string guid = valueTuple.Item1;
							bool flag5 = guid == eventGuid;
							if (flag5)
							{
								prevIndex = i;
								prevData.SelectingEvent = eventGuid;
								break;
							}
						}
						bool flag6 = prevIndex > -1;
						if (flag6)
						{
							break;
						}
					}
				}
				bool flag7 = prevIndex > -1;
				if (flag7)
				{
					break;
				}
			}
			bool flag8 = prevIndex > -1;
			if (flag8)
			{
				this.RefreshTree((int)data.Depth);
				float fromValue = bar.value;
				float toValue = (float)prevIndex / (float)this._treeColumnDataList.Count;
				DOVirtual.Float(fromValue, toValue, 0.3f, delegate(float stepValue)
				{
					bar.value = stepValue;
				}).SetAutoKill(true);
			}
			else
			{
				TreeColumnData nextData = EasyPool.Get<TreeColumnData>();
				nextData.Depth = data.Depth + 1;
				nextData.EventGroupData = data.EventGroupData;
				nextData.SelectingEvent = string.Empty;
				nextData.JumpToEventList = new List<ValueTuple<string, string, string>>();
				nextData.OptionOnSelectToEventList = new List<ValueTuple<string, List<ValueTuple<string, string, string>>>>();
				SingletonObject.getInstance<EventEditorModel>().GetEventToEventList(eventGuid, nextData.JumpToEventList, nextData.OptionOnSelectToEventList);
				bool flag9 = nextData.JumpToEventList.Count > 0;
				if (flag9)
				{
					this._treeColumnDataList.Add(nextData);
				}
				else
				{
					foreach (ValueTuple<string, List<ValueTuple<string, string, string>>> tuple in nextData.OptionOnSelectToEventList)
					{
						bool flag10 = tuple.Item2.Count > 0;
						if (flag10)
						{
							this._treeColumnDataList.Add(nextData);
							break;
						}
					}
				}
				this.RefreshTree((int)data.Depth);
				float startValue = bar.value;
				DOVirtual.Float(startValue, 1f, 0.3f, delegate(float stepValue)
				{
					bar.value = stepValue;
				}).SetAutoKill(true);
			}
		}

		// Token: 0x06004D24 RID: 19748 RVA: 0x00246E60 File Offset: 0x00245060
		private static void OnEventDoubleClick(TreeColumnData data, string eventGuid)
		{
			data.SelectingEvent = eventGuid;
			TaskControlPanel.Instance.OnEventEditorClick();
			EventEditorEventList.Instance.Select(eventGuid);
		}

		// Token: 0x06004D27 RID: 19751 RVA: 0x00246E94 File Offset: 0x00245094
		[CompilerGenerated]
		internal static void <ReleaseColumnItem>g__DestroyTarget|41_0<T>(string prefabKey, string nameTag, ref EventGroupTreeView.<>c__DisplayClass41_0 A_2) where T : MonoBehaviour
		{
			foreach (T target in A_2.columnInfo.content.GetComponentsInTopChildren(false))
			{
				bool flag = target.gameObject.name.StartsWith(nameTag);
				if (flag)
				{
					PoolManager.Destroy(prefabKey, target.gameObject);
				}
			}
		}

		// Token: 0x06004D28 RID: 19752 RVA: 0x00246F00 File Offset: 0x00245100
		[CompilerGenerated]
		internal static List<T> <RefreshColumnItem>g__GetPrefabInfos|44_0<T>(string nameTag, ref EventGroupTreeView.<>c__DisplayClass44_0 A_1) where T : MonoBehaviour
		{
			List<T> infos = EasyPool.Get<List<T>>();
			T[] targetInfos = A_1.rootTrans.GetComponentsInTopChildren(false);
			for (int i = 0; i < targetInfos.Length; i++)
			{
				bool flag = targetInfos[i].name.StartsWith(nameTag);
				if (flag)
				{
					infos.Add(targetInfos[i]);
				}
			}
			return infos;
		}

		// Token: 0x06004D29 RID: 19753 RVA: 0x00246F68 File Offset: 0x00245168
		[CompilerGenerated]
		internal static T <RefreshColumnItem>g__GetTargetInfo|44_1<T>(List<T> list, string prefabKey, ref int index, ref EventGroupTreeView.<>c__DisplayClass44_0 A_3) where T : MonoBehaviour
		{
			bool flag = list.CheckIndex(index);
			T res;
			if (flag)
			{
				res = list[index];
				index++;
			}
			else
			{
				res = PoolManager.GetObject<T>(prefabKey);
				res.transform.SetParent(A_3.rootTrans, false);
			}
			res.transform.SetAsLastSibling();
			return res;
		}

		// Token: 0x04003568 RID: 13672
		public static EventGroupTreeView Instance;

		// Token: 0x04003569 RID: 13673
		[SerializeField]
		private RectTransform content;

		// Token: 0x0400356A RID: 13674
		[SerializeField]
		private InfinityScroll eventGroupScrollView;

		// Token: 0x0400356B RID: 13675
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x0400356C RID: 13676
		[SerializeField]
		private GameObject goEventColumnPrefab;

		// Token: 0x0400356D RID: 13677
		[SerializeField]
		private GameObject titlePrefab;

		// Token: 0x0400356E RID: 13678
		[SerializeField]
		private GameObject eventPrefab;

		// Token: 0x0400356F RID: 13679
		[SerializeField]
		private CScrollbar horizontalScrollBar;

		// Token: 0x04003570 RID: 13680
		[SerializeField]
		private RectTransform groupList;

		// Token: 0x04003572 RID: 13682
		private const string ColumnPrefabKey = "EventGroupTreeView_ColumnPrefab";

		// Token: 0x04003573 RID: 13683
		private const string TitlePrefabKey = "EventGroupTreeView_TitlePrefab";

		// Token: 0x04003574 RID: 13684
		private const string EventPrefabKey = "EventGroupTreeView_EventPrefab";

		// Token: 0x04003575 RID: 13685
		private static Color _groupTitleNameBackColor;

		// Token: 0x04003576 RID: 13686
		private static Color _toEventTitleNameBackColor;

		// Token: 0x04003577 RID: 13687
		private static Color _eventOptionNextBackColor;

		// Token: 0x04003578 RID: 13688
		private List<EventGroupData> _allGroupDataList;

		// Token: 0x04003579 RID: 13689
		private List<EventGroupData> _showingGroupDataList;

		// Token: 0x0400357A RID: 13690
		private List<TreeColumnData> _treeColumnDataList;

		// Token: 0x0400357B RID: 13691
		private int _currentClickGroupItemIndex;
	}
}
