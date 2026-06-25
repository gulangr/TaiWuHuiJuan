using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Config;
using EventEditor.EventScript;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Views.Migrate;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x02000641 RID: 1601
	public class EventEditorEventList : EventEditorSubPageBase
	{
		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06004BB7 RID: 19383 RVA: 0x0023A5CA File Offset: 0x002387CA
		// (set) Token: 0x06004BB8 RID: 19384 RVA: 0x0023A5D8 File Offset: 0x002387D8
		public EventGroupData ShowingEventGroup
		{
			get
			{
				return EventGroupTreeView.Instance.EditingEventGroup;
			}
			set
			{
				EventGroupTreeView.Instance.EditingEventGroup = value;
				SingletonObject.getInstance<EventEditorModel>().SearchEventGroup(string.Empty, (EventGroupData groupData) => groupData != null && value != null);
				bool isEventEditorShow = TaskControlPanel.Instance.isEventEditorShow;
				if (isEventEditorShow)
				{
					this.Show();
				}
			}
		}

		// Token: 0x06004BB9 RID: 19385 RVA: 0x0023A635 File Offset: 0x00238835
		public static void Init(EventEditorEventList instance)
		{
			EventEditorEventList.Instance = instance;
			EventEditorEventList.Instance.InternalInit();
		}

		// Token: 0x06004BBA RID: 19386 RVA: 0x0023A649 File Offset: 0x00238849
		protected override void InternalInit()
		{
			this._showingEvents = new List<ValueTuple<string, string, string>>();
			this.InitComponents();
		}

		// Token: 0x06004BBB RID: 19387 RVA: 0x0023A660 File Offset: 0x00238860
		public override void Show()
		{
			this.InitAllEvents();
			this.RefreshList();
			string groupName = (this.ShowingEventGroup == null) ? string.Empty : this.ShowingEventGroup.Name;
			this.eventGroupInput.SetTextWithoutNotify(groupName);
		}

		// Token: 0x06004BBC RID: 19388 RVA: 0x0023A6A4 File Offset: 0x002388A4
		public override void Hide()
		{
		}

		// Token: 0x06004BBD RID: 19389 RVA: 0x0023A6A7 File Offset: 0x002388A7
		private void OnEnable()
		{
			GEvent.Add(ModEditorEvents.EditingEventChange, new GEvent.Callback(this.OnEditingEventChange));
			GEvent.Add(ModEditorEvents.EventDeleted, new GEvent.Callback(this.OnDeleteEvent));
		}

		// Token: 0x06004BBE RID: 19390 RVA: 0x0023A6DA File Offset: 0x002388DA
		private void OnDisable()
		{
			GEvent.Remove(ModEditorEvents.EventDeleted, new GEvent.Callback(this.OnDeleteEvent));
			GEvent.Remove(ModEditorEvents.EditingEventChange, new GEvent.Callback(this.OnEditingEventChange));
		}

		// Token: 0x06004BBF RID: 19391 RVA: 0x0023A710 File Offset: 0x00238910
		public void InitComponents()
		{
			this.listScroll.OnItemRender += this.OnEventItemRender;
			this.searchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchInputValueChange));
			this.eventGroupInput.interactable = false;
		}

		// Token: 0x06004BC0 RID: 19392 RVA: 0x0023A760 File Offset: 0x00238960
		public void Select(string eventGuid)
		{
			bool flag = !this.ShowingEventGroup.HasEvent(eventGuid);
			if (flag)
			{
				EventGroupData groupData = SingletonObject.getInstance<EventEditorModel>().GetGroupDataByEventGuid(eventGuid);
				bool flag2 = groupData != null;
				if (flag2)
				{
					this.ShowingEventGroup = groupData;
				}
			}
			this.InitAllEvents();
			this.RefreshList();
			int index = -1;
			for (int i = 0; i < this._showingEvents.Count; i++)
			{
				bool flag3 = this._showingEvents[i].Item1 == eventGuid;
				if (flag3)
				{
					index = i;
					break;
				}
			}
			bool flag4 = this.ShowingEventGroup.HasEvent(eventGuid);
			if (flag4)
			{
				bool flag5 = index >= 0;
				if (flag5)
				{
					this.OnEditEvent(eventGuid);
					this.listScroll.ScrollTo(index, 0.3f);
				}
			}
		}

		// Token: 0x06004BC1 RID: 19393 RVA: 0x0023A834 File Offset: 0x00238A34
		private void OnDeleteEvent(ArgumentBox box)
		{
			string guid;
			bool flag = box.Get("Guid", out guid);
			if (flag)
			{
				for (int i = 0; i < this._allEvents.Count; i++)
				{
					bool flag2 = this._allEvents[i].Item1 == guid;
					if (flag2)
					{
						this._allEvents.RemoveAt(i);
						this.RefreshList();
						break;
					}
				}
			}
		}

		// Token: 0x06004BC2 RID: 19394 RVA: 0x0023A8A8 File Offset: 0x00238AA8
		private void InitAllEvents()
		{
			bool flag = this._allEvents == null;
			if (flag)
			{
				this._allEvents = new List<ValueTuple<string, string, string>>();
			}
			else
			{
				this._allEvents.Clear();
			}
			bool flag2 = this.ShowingEventGroup != null;
			if (flag2)
			{
				this._allEvents = this.ShowingEventGroup.GetDisplayList(true);
				SingletonObject.getInstance<EventEditorModel>().SearchEventGroup(string.Empty, null);
			}
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x0023A910 File Offset: 0x00238B10
		private void RefreshList()
		{
			string searchKey = this.searchInputField.text;
			this._showingEvents.Clear();
			Regex guidRegex = new Regex("[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}");
			foreach (ValueTuple<string, string, string> tuple in this._allEvents)
			{
				bool flag = string.IsNullOrEmpty(searchKey);
				if (flag)
				{
					this._showingEvents.Add(tuple);
				}
				else
				{
					bool flag2 = tuple.Item1.Contains(searchKey);
					if (flag2)
					{
						bool flag3 = guidRegex.IsMatch(tuple.Item1) && tuple.Item1 == searchKey;
						if (flag3)
						{
							this._showingEvents.Clear();
							this._showingEvents.Add(tuple);
							break;
						}
						this._showingEvents.Add(tuple);
					}
					else
					{
						bool flag4 = tuple.Item2.Contains(searchKey);
						if (flag4)
						{
							this._showingEvents.Add(tuple);
						}
						else
						{
							bool flag5 = !string.IsNullOrEmpty(tuple.Item3) && tuple.Item3.Contains(searchKey);
							if (flag5)
							{
								this._showingEvents.Add(tuple);
							}
						}
					}
				}
			}
			this.listScroll.UpdateData(this._showingEvents.Count);
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x0023AA74 File Offset: 0x00238C74
		private void OnEditingEventChange(ArgumentBox box)
		{
			int index = -1;
			string targetEventGuid = EventEditorEventDetail.Instance.CurEvent.EventGuid;
			for (int i = 0; i < this._showingEvents.Count; i++)
			{
				bool flag = this._showingEvents[i].Item1 == targetEventGuid;
				if (flag)
				{
					index = i;
					break;
				}
			}
			InfinityScroll scroll = this.listScroll;
			bool flag2 = null == scroll.GetActiveCell(index);
			if (flag2)
			{
				scroll.ScrollTo(index - 6, 0.3f);
			}
			else
			{
				scroll.ReRender();
			}
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x0023AB06 File Offset: 0x00238D06
		private void OnSearchInputValueChange(string searchKey)
		{
			this.RefreshList();
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x0023AB10 File Offset: 0x00238D10
		private void OnEventItemRender(int dataIndex, GameObject goItem)
		{
			ValueTuple<string, string, string> valueTuple = this._showingEvents[dataIndex];
			string guid = valueTuple.Item1;
			string eventName = valueTuple.Item2;
			string eventContent = valueTuple.Item3;
			EventEditorWindowCellPrefabInfo cellInfo = goItem.GetComponent<EventEditorWindowCellPrefabInfo>();
			PointClickBridge bridge = cellInfo.clickBridge;
			bridge.OnLeftClick = delegate()
			{
				this.OnEventLeftClick(dataIndex, guid);
			};
			bridge.OnRightClick = delegate()
			{
				this.OnEventRightClick(guid);
			};
			cellInfo.goEditingEvent.SetActive(EventEditorEventDetail.Instance.IsCurrentEditingEvent(guid));
			cellInfo.txtMeshEventName.text = eventName;
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			EventEditorData eventData = SingletonObject.getInstance<EventEditorModel>().GetEvent(guid);
			bool flag = eventData != null;
			if (flag)
			{
				string triggerCode = eventData.TriggerType;
				cellInfo.goHasTrigger.SetActive(triggerCode != "None");
				EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
				string eventDir = model.EnsureEventScriptPath(eventData);
				string scriptUrl = Path.Combine(eventDir, guid + ".tws");
				bool hasInstruction = File.Exists(scriptUrl);
				cellInfo.goHasInstruction.SetActive(hasInstruction);
				bool hasCode = SingletonObject.getInstance<EventEditorModel>().IsEventCodeValidity(eventData);
				cellInfo.goHasCode.SetActive(hasCode);
				EventTriggerTypeItem triggerTypeItem = EventTriggerType.Instance.GetByKeyCode(triggerCode);
				string refName = EventTriggerType.Instance.GetRefName((triggerTypeItem != null) ? triggerTypeItem.TemplateId : -1);
				stringBuilder.AppendLine("触发类型: " + refName);
				int scriptRowCount = hasInstruction ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)).Instructions.Count : 0;
				stringBuilder.AppendLine(string.Format("指令行数: {0}", scriptRowCount));
				string conditionScriptUrl = Path.Combine(eventDir, guid + "_condition.tws");
				int conditionScriptRowCount = File.Exists(conditionScriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(conditionScriptUrl)).Instructions.Count : 0;
				stringBuilder.AppendLine(string.Format("条件指令行数: {0}", conditionScriptRowCount));
				stringBuilder.AppendLine(hasCode ? ("是否有C#代码: " + LocalStringManager.Get(LanguageKey.LK_Yes)) : ("是否有C#代码: " + LocalStringManager.Get(LanguageKey.LK_No)));
				int optionCount = eventData.Options.Count;
				stringBuilder.AppendLine(string.Format("选项数量: {0}", optionCount));
			}
			stringBuilder.Append(eventContent.IsNullOrEmpty() ? eventContent : eventContent.Replace("<NL>", "\n"));
			cellInfo.mouseTip.PresetParam = new string[]
			{
				stringBuilder.ToString()
			};
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x0023ADD8 File Offset: 0x00238FD8
		private void OnEventLeftClick(int dataIndex, string eventGuid)
		{
			string prevGuid = string.Empty;
			bool flag = EventEditorEventDetail.Instance.CurEvent != null;
			if (flag)
			{
				prevGuid = EventEditorEventDetail.Instance.CurEvent.EventGuid;
			}
			int changedIndex = -1;
			for (int i = 0; i < this._allEvents.Count; i++)
			{
				bool flag2 = this._allEvents[i].Item1 == prevGuid;
				if (flag2)
				{
					changedIndex = i;
					break;
				}
			}
			this.OnEditEvent(eventGuid);
			EventEditorEventPreview.Instance.ClearStack();
			InfinityScroll scroll = this.listScroll;
			bool flag3 = -1 != changedIndex;
			if (flag3)
			{
				scroll.RefreshCell(changedIndex);
			}
			scroll.RefreshCell(dataIndex);
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x0023AE8C File Offset: 0x0023908C
		private void OnEventRightClick(string eventGuid)
		{
			bool flag = !TaskControlPanel.Instance.eventEditorWindowReady;
			if (!flag)
			{
				List<SheetButtonInfo> sheetInfos = new List<SheetButtonInfo>();
				EventEditorData eventData = SingletonObject.getInstance<EventEditorModel>().GetEvent(eventGuid);
				sheetInfos.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_CopyGuid), delegate()
				{
					GUIUtility.systemCopyBuffer = "\"" + eventGuid + "\"";
				}, true, ""));
				bool isCopiedEvent = EventEditorClipBoard.CopiedEvent != null && EventEditorClipBoard.CopiedEvent.EventGuid == eventGuid;
				sheetInfos.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_CopyEvent), delegate()
				{
					EventEditorClipBoard.CopiedEvent = eventData;
				}, !isCopiedEvent, ""));
				bool canPasteEvent = EventEditorClipBoard.CopiedEvent != null && EventEditorClipBoard.CopiedEvent.EventGuid != eventGuid;
				sheetInfos.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_PasteEvent), delegate()
				{
					EventEditorClipBoard.PasteEvent(eventData);
				}, canPasteEvent, ""));
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", sheetInfos);
				argBox.SetObject("ButtonTextHandler", UI_ButtonSheet.HandleButtonLabelContent);
				argBox.SetObject("ButtonSize", new Vector2(200f, 45f));
				UIElement.ButtonSheet.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
			}
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x0023AFFC File Offset: 0x002391FC
		private void OnEditEvent(string eventGuid)
		{
			bool flag = EventEditorEventDetail.Instance.IsCurrentEditingEvent(eventGuid);
			if (!flag)
			{
				EventEditorData eventData = SingletonObject.getInstance<EventEditorModel>().GetEvent(eventGuid);
				bool flag2 = eventData != null;
				if (flag2)
				{
					EventEditorEventDetail.Instance.EditEvent(eventData);
				}
			}
		}

		// Token: 0x040034A4 RID: 13476
		private List<ValueTuple<string, string, string>> _allEvents;

		// Token: 0x040034A5 RID: 13477
		private List<ValueTuple<string, string, string>> _showingEvents;

		// Token: 0x040034A6 RID: 13478
		public static EventEditorEventList Instance;

		// Token: 0x040034A7 RID: 13479
		[SerializeField]
		private InfinityScroll listScroll;

		// Token: 0x040034A8 RID: 13480
		[SerializeField]
		private TMP_InputField searchInputField;

		// Token: 0x040034A9 RID: 13481
		[SerializeField]
		private TMP_InputField eventGroupInput;

		// Token: 0x040034AA RID: 13482
		[SerializeField]
		private TextMeshProUGUI txtMeshAuthor;
	}
}
