using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Config;
using EventEditor.EventScript;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent.Enum;
using GameData.Domains.TaiwuEvent.EventOption;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x02000640 RID: 1600
	public class EventEditorEventDetail : EventEditorSubPageBase
	{
		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x06004B89 RID: 19337 RVA: 0x0023833F File Offset: 0x0023653F
		public EventEditorData CurEvent
		{
			get
			{
				return this._curEvent;
			}
		}

		// Token: 0x06004B8A RID: 19338 RVA: 0x00238347 File Offset: 0x00236547
		public static void Init(EventEditorEventDetail instance)
		{
			EventEditorEventDetail.Instance = instance;
			EventEditorEventDetail.Instance.InternalInit();
		}

		// Token: 0x06004B8B RID: 19339 RVA: 0x0023835C File Offset: 0x0023655C
		protected override void InternalInit()
		{
			this.OperateStack = new OperateStack(64);
			this.btnEditScript.ClearAndAddListener(new Action(this.OnEditScript));
			this.btnEditCondition.ClearAndAddListener(new Action(this.OnEditCondition));
			this.btnEditCode.ClearAndAddListener(new Action(this.OnEditCode));
			this.btnEditBoolState.ClearAndAddListener(new Action(this.OnEditBoolState));
			this.btnCopyGuid.ClearAndAddListener(new Action(this.OnCopyGuid));
			this.btnCopyEvent.ClearAndAddListener(new Action(this.OnCopyEvent));
			this.btnPasteEvent.ClearAndAddListener(new Action(this.OnPasteEvent));
			this.btnDeleteEvent.ClearAndAddListener(new Action(this.OnDeleteEvent));
			this.btnSaveEvent.ClearAndAddListener(new Action(this.OnSaveEvent));
			this.btnEditLanguage.ClearAndAddListener(new Action(this.OnEditEventLanguage));
			this.btnSelectAudio.ClearAndAddListener(new Action(this.OnSelectAudio));
			this.btnPlayAudio.ClearAndAddListener(new Action(this.OnPlayAudio));
			this.btnPasteEvent.interactable = false;
			this.InitComponents();
			EventEditorEventPreview.Instance.Refresh(null);
			EventEditorTagReplace.Init(this.panelTagReplace);
			this.btnTagReplace.ClearAndAddListener(new Action(EventEditorTagReplace.Instance.SetShow));
		}

		// Token: 0x06004B8C RID: 19340 RVA: 0x002384DF File Offset: 0x002366DF
		public override void Show()
		{
			GEvent.Add(ModEditorEvents.EventDeleted, new GEvent.Callback(this.OnDeleteEventFromOutSide));
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x002384FA File Offset: 0x002366FA
		public override void Hide()
		{
			GEvent.Remove(ModEditorEvents.EventDeleted, new GEvent.Callback(this.OnDeleteEventFromOutSide));
		}

		// Token: 0x06004B8E RID: 19342 RVA: 0x00238518 File Offset: 0x00236718
		public void EditEvent(EventEditorData eventData)
		{
			EventEditorEventDetail.<>c__DisplayClass46_0 CS$<>8__locals1 = new EventEditorEventDetail.<>c__DisplayClass46_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.eventData = eventData;
			bool flag = CS$<>8__locals1.eventData == null;
			if (flag)
			{
				this._curEvent = null;
				this.InitComponents();
				EventEditorEventPreview.Instance.Refresh(null);
			}
			else
			{
				bool flag2 = this._curEvent != null;
				if (flag2)
				{
					string curEventGuid = this._curEvent.EventGuid;
					bool flag3 = CS$<>8__locals1.eventData.EventGuid == curEventGuid;
					if (flag3)
					{
						this.Refresh();
						return;
					}
					bool dirty = this._curEvent.Dirty;
					if (dirty)
					{
						string eventIdentify = this._curEvent.EventName;
						bool flag4 = string.IsNullOrEmpty(eventIdentify);
						if (flag4)
						{
							eventIdentify = this._curEvent.EventGuid;
						}
						DialogCmd cmd = new DialogCmd
						{
							Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
							Content = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_EventChanged, eventIdentify).ColorReplace(),
							Type = 3,
							Yes = delegate()
							{
								CS$<>8__locals1.<>4__this.OnSaveEvent();
								base.<EditEvent>g__DoEdit|0();
							},
							No = new Action(CS$<>8__locals1.<EditEvent>g__DoEdit|0),
							GroupYesText = LocalStringManager.Get(LanguageKey.UI_EventEditor_Dialog_Save),
							GroupNoText = LocalStringManager.Get(LanguageKey.UI_EventEditor_Dialog_DoNotSave)
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
						return;
					}
				}
				CS$<>8__locals1.<EditEvent>g__DoEdit|0();
			}
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x00238694 File Offset: 0x00236894
		public void InitComponents()
		{
			bool conchShipEditor = UI_EventEditor.IsDev;
			bool flag = this.eventGroupInput == null;
			if (!flag)
			{
				this.eventGroupInput.onValueChanged.RemoveAllListeners();
				this.eventGroupInput.text = LocalStringManager.Get(LanguageKey.LK_Common_All);
				this.InitEventGroupInput(this.eventGroupInput);
				this.eventTypeDropDown.ClearOptions();
				List<string> eventTypeList = new List<string>();
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_1));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_2));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_3));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_4));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_5));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_6));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_7));
				eventTypeList.Add(LocalStringManager.Get(LanguageKey.UI_EventEditor_EventType_8));
				this.eventTypeDropDown.AddOptions(eventTypeList);
				this.eventTypeDropDown.onValueChanged.RemoveAllListeners();
				this.eventTypeDropDown.onValueChanged.AddListener(new UnityAction<int>(this.OnEventTypeDropdownValueChange));
				this.eventTypeDropDown.interactable = conchShipEditor;
				bool flag2 = !conchShipEditor;
				if (flag2)
				{
					this.eventTypeDropDown.value = 6;
					this.eventTypeDropDown.RefreshShownValue();
				}
				this._triggerTypeList = new List<string>();
				IReadOnlyDictionary<string, int> refMap = EventTriggerType.Instance.RefNameMap;
				this._triggerTypeList.AddRange(refMap.Keys);
				this._triggerTypeList.Sort((string a, string b) => refMap[a].CompareTo(refMap[b]));
				this._triggerTypeList.RemoveAll((string triggerType) => !EventTriggerType.Instance[triggerType].AllowExternal);
				this.eventTriggerInputField.onSelect.RemoveAllListeners();
				this.eventTriggerInputField.onSelect.AddListener(new UnityAction<string>(this.OnEventTriggerBeginEdit));
				this.eventTriggerInputField.onValueChanged.RemoveAllListeners();
				this.eventTriggerInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnEventTriggerChanged));
				this.eventTriggerInputField.onEndEdit.RemoveAllListeners();
				this.eventTriggerInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEventTriggerEndEdit));
				CToggle forceSingleToggle = this.toggleForceSingle;
				forceSingleToggle.isOn = false;
				forceSingleToggle.onValueChanged.RemoveAllListeners();
				forceSingleToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnForceSingleToggleValueChange));
				CToggle maskControlToggle = this.toggleControlMask;
				maskControlToggle.isOn = false;
				maskControlToggle.onValueChanged.RemoveAllListeners();
				maskControlToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnControlMaskToggleChange));
				this.goLineMaskAction.SetActive(false);
				this.goLineMaskTween.SetActive(false);
				CDropdown maskControlTypeDropdown = this.maskControlTypeDropDown;
				maskControlTypeDropdown.ClearOptions();
				List<string> maskControlTypeStrings = new List<string>
				{
					LocalStringManager.Get(LanguageKey.UI_EventEditor_MaskControlType_1),
					LocalStringManager.Get(LanguageKey.UI_EventEditor_MaskControlType_2),
					LocalStringManager.Get(LanguageKey.UI_EventEditor_MaskControlType_3),
					LocalStringManager.Get(LanguageKey.UI_EventEditor_MaskControlType_4),
					LocalStringManager.Get(LanguageKey.UI_EventEditor_MaskControlType_5),
					LocalStringManager.Get(LanguageKey.UI_EventEditor_MaskControlType_6)
				};
				maskControlTypeDropdown.AddOptions(maskControlTypeStrings);
				maskControlTypeDropdown.onValueChanged.RemoveAllListeners();
				maskControlTypeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnControlMaskTypeDropDownChange));
				this.txtMeshGuidValue.text = string.Empty;
				this.eventNameInputField.text = string.Empty;
				this.eventOrderInputField.text = string.Empty;
				this.decideRoleKeyInputField.text = string.Empty;
				this.targetRoleKeyInputField.text = string.Empty;
				this.maskTweenTimeInputField.text = string.Empty;
				this.eventTextureNameInputField.text = string.Empty;
				this.maskTweenTimeInputField.onEndEdit.RemoveAllListeners();
				this.eventNameInputField.onEndEdit.RemoveAllListeners();
				this.eventOrderInputField.onEndEdit.RemoveAllListeners();
				this.decideRoleKeyInputField.onEndEdit.RemoveAllListeners();
				this.targetRoleKeyInputField.onEndEdit.RemoveAllListeners();
				this.eventTextureNameInputField.onEndEdit.RemoveAllListeners();
				this.maskTweenTimeInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnMaskTweenTimeInputFieldEndEdit));
				this.eventNameInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEventNameInputEndEdit));
				this.eventOrderInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEventSortingOrderInputEndEdit));
				this.decideRoleKeyInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnDecideRoleInputEndEdit));
				this.targetRoleKeyInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnTargetRoleInputEndEdit));
				this.eventTextureNameInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEventTextureNameInputEndEdit));
				this.lineAudio.gameObject.SetActive(conchShipEditor);
				bool flag3 = conchShipEditor;
				if (flag3)
				{
					this.txtMeshAudioFilePath.text = string.Empty;
					this.btnSelectAudio.gameObject.SetActive(false);
					this.btnPlayAudio.gameObject.SetActive(false);
					this.toggleHasAudio.isOn = false;
					this.toggleHasAudio.onValueChanged.RemoveAllListeners();
					this.toggleHasAudio.onValueChanged.AddListener(new UnityAction<bool>(this.OnPlayAudioToggleValueChange));
				}
				this.goLineLanguage.SetActive(false);
				this.goLineBtnEditLanguage.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x00238C68 File Offset: 0x00236E68
		public void ExecuteOperateCommand(OperateCommand cmd)
		{
			bool flag = cmd != null;
			if (flag)
			{
				OperateStack operateStack = this.OperateStack;
				if (operateStack != null)
				{
					operateStack.Execute(cmd, true);
				}
			}
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x00238C92 File Offset: 0x00236E92
		public void Undo()
		{
			OperateStack operateStack = this.OperateStack;
			if (operateStack != null)
			{
				operateStack.Undo();
			}
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x00238CA7 File Offset: 0x00236EA7
		public void Redo()
		{
			this.OperateStack.Redo();
		}

		// Token: 0x06004B93 RID: 19347 RVA: 0x00238CB8 File Offset: 0x00236EB8
		public bool IsCurrentEditingEvent(string eventGuid)
		{
			return this._curEvent != null && eventGuid == this._curEvent.EventGuid;
		}

		// Token: 0x06004B94 RID: 19348 RVA: 0x00238CE8 File Offset: 0x00236EE8
		private void Refresh()
		{
			this._ignoreDirty = true;
			EventEditorData copiedEvent = EventEditorClipBoard.CopiedEvent;
			bool flag = copiedEvent != null && this._curEvent.EventGuid != copiedEvent.EventGuid;
			if (flag)
			{
				this.btnPasteEvent.interactable = true;
			}
			else
			{
				this.btnPasteEvent.interactable = false;
			}
			this.txtMeshGuidValue.text = this._curEvent.EventGuid;
			Dictionary<string, string> eventGroupDic = SingletonObject.getInstance<EventEditorModel>().EventGroupInfoDic;
			string eventGroup = this._curEvent.EventGroup;
			bool flag2 = string.IsNullOrEmpty(eventGroup);
			if (flag2)
			{
				eventGroup = "None";
			}
			string key;
			bool flag3 = eventGroupDic.TryGetValue(eventGroup, out key);
			if (flag3)
			{
				this.eventGroupInput.SetTextWithoutNotify(key);
			}
			this.toggleForceSingle.isOn = this._curEvent.ForceSingle;
			string typeString = this._curEvent.EventType;
			bool flag4 = !string.IsNullOrEmpty(typeString);
			if (flag4)
			{
				EEventType type;
				bool flag5 = Enum.TryParse<EEventType>(typeString, out type);
				if (flag5)
				{
					this.eventTypeDropDown.value = (int)type;
				}
			}
			else
			{
				this.eventTypeDropDown.value = 0;
			}
			string triggerCode = this._curEvent.TriggerType;
			bool flag6 = !string.IsNullOrEmpty(triggerCode);
			if (flag6)
			{
				EventTriggerTypeItem eventTriggerType = EventTriggerType.Instance.GetByKeyCode(triggerCode);
				string refName = EventTriggerType.Instance.GetRefName((eventTriggerType != null) ? eventTriggerType.TemplateId : -1);
				this.eventTriggerInputField.SetTextWithoutNotify(refName);
			}
			else
			{
				this.eventTriggerInputField.SetTextWithoutNotify(string.Empty);
			}
			this.eventNameInputField.text = this._curEvent.EventName;
			this.eventOrderInputField.text = this._curEvent.EventOrder.ToString();
			this.decideRoleKeyInputField.text = this._curEvent.DecideRole;
			this.targetRoleKeyInputField.text = this._curEvent.TargetRole;
			this.eventTextureNameInputField.text = this._curEvent.EventTexture;
			bool controlMask = this._curEvent.ControlMask;
			this.toggleControlMask.isOn = controlMask;
			this.goLineMaskAction.SetActive(controlMask);
			this.goLineMaskTween.SetActive(controlMask);
			bool flag7 = controlMask;
			if (flag7)
			{
				byte maskCode = this._curEvent.ControlMaskCode;
				this.maskControlTypeDropDown.value = (int)maskCode;
				this.maskTweenTimeInputField.text = this._curEvent.MaskTweenTime.ToString(CultureInfo.InvariantCulture);
			}
			string audioName = this._curEvent.AudioName;
			this.toggleHasAudio.isOn = !string.IsNullOrEmpty(audioName);
			this.txtMeshAudioFilePath.text = audioName;
			this.btnSelectAudio.gameObject.SetActive(!string.IsNullOrEmpty(audioName));
			this.btnPlayAudio.gameObject.SetActive(this.CanPlayAudio());
			EventEditorEventPreview.Instance.Refresh(this._curEvent);
			this._ignoreDirty = false;
		}

		// Token: 0x06004B95 RID: 19349 RVA: 0x00238FD8 File Offset: 0x002371D8
		private void SetDirty()
		{
			bool flag = !this._ignoreDirty && this._curEvent != null;
			if (flag)
			{
				Debug.Log("SetDirty Called");
				this._curEvent.Dirty = true;
				this._curEvent.TmEdit = Save.GetTimeStamp();
			}
		}

		// Token: 0x06004B96 RID: 19350 RVA: 0x00239028 File Offset: 0x00237228
		private void GenerateEventScript()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				SingletonObject.getInstance<EventEditorModel>().CreateEventCodeFile(this._curEvent);
			}
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x00239058 File Offset: 0x00237258
		private void OnEditScript()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string eventGuidString = this._curEvent.EventGuid;
				EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
				string eventDir = model.EnsureEventScriptPath(this._curEvent);
				bool flag2 = !Directory.Exists(eventDir);
				if (!flag2)
				{
					string scriptUrl = Path.Combine(eventDir, eventGuidString + ".tws");
					EventScriptEditorData scriptData = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
					EventEditorScript.Instance.Show(scriptData, delegate(EventScriptEditorData data)
					{
						model.EnsureEventScriptPath(this._curEvent);
						model.SaveEventScript(eventGuidString, scriptUrl, data);
					}, 1, eventGuidString, "");
				}
			}
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x0023912C File Offset: 0x0023732C
		private void OnEditBoolState()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string eventGuidString = this._curEvent.EventGuid;
				EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
				string eventDir = model.EnsureEventScriptPath(this._curEvent);
				bool flag2 = !Directory.Exists(eventDir);
				if (!flag2)
				{
					string scriptUrl = Path.Combine(eventDir, eventGuidString + "_boolState.twe");
					Dictionary<short, EventBoolStateInfo> eventBoolState = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<Dictionary<short, EventBoolStateInfo>>(File.ReadAllText(scriptUrl)) : null;
					EventBoolStateEditor.Instance.Show(eventBoolState, delegate(Dictionary<short, EventBoolStateInfo> data)
					{
						model.EnsureEventScriptPath(this._curEvent);
						bool flag3 = data.Count == 0;
						if (flag3)
						{
							File.Delete(scriptUrl);
						}
						else
						{
							File.WriteAllText(scriptUrl, JsonConvert.SerializeObject(data, Formatting.Indented));
						}
					});
				}
			}
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x002391E8 File Offset: 0x002373E8
		private void OnEditCondition()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string eventGuidString = this._curEvent.EventGuid;
				EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
				string eventDir = model.EnsureEventScriptPath(this._curEvent);
				bool flag2 = !Directory.Exists(eventDir);
				if (!flag2)
				{
					string scriptUrl = Path.Combine(eventDir, eventGuidString + "_condition.tws");
					EventScriptEditorData scriptData = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
					EventEditorScript.Instance.Show(scriptData, delegate(EventScriptEditorData data)
					{
						model.EnsureEventScriptPath(this._curEvent);
						model.SaveEventScript(eventGuidString, scriptUrl, data);
					}, 2, eventGuidString, "");
				}
			}
		}

		// Token: 0x06004B9A RID: 19354 RVA: 0x002392BC File Offset: 0x002374BC
		private void OnEditCode()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string eventGuidString = this._curEvent.EventGuid;
				string eventGroup = this._curEvent.EventGroup;
				bool flag2 = string.IsNullOrEmpty(eventGroup);
				if (flag2)
				{
					TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_SaveEventNoEventGroup), true);
				}
				else
				{
					string eventDir = Save.GetEventSaveDir(this._curEvent);
					bool flag3 = !Directory.Exists(eventDir);
					if (flag3)
					{
						this.OnSaveEvent();
					}
					bool flag4 = !Directory.Exists(eventDir);
					if (!flag4)
					{
						string scriptUrl = Path.Combine(eventDir, eventGuidString + ".cs");
						bool flag5 = !File.Exists(scriptUrl);
						if (flag5)
						{
							this.GenerateEventScript();
						}
						UI_EventEditor.OpenCSharpCodeFile(scriptUrl);
					}
				}
			}
		}

		// Token: 0x06004B9B RID: 19355 RVA: 0x00239380 File Offset: 0x00237580
		private void OnEditEventLanguage()
		{
			bool flag = this._curEvent == null;
			if (flag)
			{
			}
		}

		// Token: 0x06004B9C RID: 19356 RVA: 0x002393A0 File Offset: 0x002375A0
		public void OnSaveEvent()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventGroupData groupData = SingletonObject.getInstance<EventEditorModel>().GetGroupData(this._curEvent.EventGroup);
				bool flag2 = groupData == null;
				if (flag2)
				{
					this._curEvent = null;
					this.InitComponents();
				}
				else
				{
					string eventName = this._curEvent.EventName;
					bool flag3 = string.IsNullOrEmpty(eventName);
					if (flag3)
					{
						TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_SaveEventNoName).ColorReplace(), true);
					}
					else
					{
						bool flag4 = string.IsNullOrEmpty(this._curEvent.EventGroup);
						if (flag4)
						{
							TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_SaveEventNoEventGroup), true);
						}
						else
						{
							EventEditorData srcData = SingletonObject.getInstance<EventEditorModel>().GetEvent(this._curEvent.EventGuid);
							bool flag5 = srcData == null;
							if (flag5)
							{
								srcData = this._curEvent.Duplicate();
								SingletonObject.getInstance<EventEditorModel>().AddEvent(srcData);
								EventEditorEventList.Instance.Select(this._curEvent.EventGuid);
							}
							else
							{
								this._curEvent.Refill(srcData);
							}
							SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
							{
								string saveInfo = SingletonObject.getInstance<EventEditorModel>().SaveEvent(srcData);
								bool flag6 = string.IsNullOrEmpty(saveInfo);
								if (flag6)
								{
									string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_SaveEventOK, eventName);
									EventEditorNotes.Instance.AddNote(info);
									EventEditorEventList.Instance.Show();
								}
								else
								{
									EventEditorNotes.Instance.AddNote(saveInfo);
								}
								EventEditorEventList.Instance.Select(this._curEvent.EventGuid);
							});
							this._curEvent.Dirty = false;
						}
					}
				}
			}
		}

		// Token: 0x06004B9D RID: 19357 RVA: 0x00239514 File Offset: 0x00237714
		private void OnCopyEvent()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string eventName = this._curEvent.EventName;
				bool flag2 = string.IsNullOrEmpty(eventName);
				if (flag2)
				{
					TaskControlPanel.Instance.ShowTips("Error:Empty eventName,Copy failed!", true);
				}
				else
				{
					EventEditorClipBoard.CopiedEvent = this._curEvent;
					this.btnPasteEvent.interactable = false;
					string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_CopyEvent, eventName);
					EventEditorNotes.Instance.AddNote(info);
				}
			}
		}

		// Token: 0x06004B9E RID: 19358 RVA: 0x0023958C File Offset: 0x0023778C
		private void OnPasteEvent()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData srcData = EventEditorClipBoard.CopiedEvent;
				bool flag2 = srcData.EventGuid == this._curEvent.EventGuid;
				if (!flag2)
				{
					EventEditorData preData = this._curEvent.Duplicate();
					OperateCommand cmd = new OperateCommand("PasteEvent")
					{
						Do = delegate()
						{
							EventEditorClipBoard.PasteEvent(this._curEvent);
							this.Refresh();
						},
						Undo = delegate()
						{
							EventEditorData preCopiedData = srcData;
							EventEditorClipBoard.CopiedEvent = preData;
							EventEditorClipBoard.PasteEvent(this._curEvent);
							EventEditorClipBoard.CopiedEvent = preCopiedData;
							this.Refresh();
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
					string eventName = this._curEvent.EventName;
					bool flag3 = !string.IsNullOrEmpty(eventName);
					if (flag3)
					{
						string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_PasteEvent, eventName);
						EventEditorNotes.Instance.AddNote(info);
					}
					else
					{
						TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_PasteEventOK), true);
					}
				}
			}
		}

		// Token: 0x06004B9F RID: 19359 RVA: 0x00239690 File Offset: 0x00237890
		private void OnDeleteEventFromOutSide(ArgumentBox box)
		{
			string guid;
			bool flag = box.Get("Guid", out guid) && this._curEvent != null;
			if (flag)
			{
				bool flag2 = this._curEvent.EventGuid == guid;
				if (flag2)
				{
					this._curEvent = null;
					this.InitComponents();
				}
			}
		}

		// Token: 0x06004BA0 RID: 19360 RVA: 0x002396E4 File Offset: 0x002378E4
		private void OnDeleteEvent()
		{
			EventEditorEventDetail.<>c__DisplayClass64_0 CS$<>8__locals1 = new EventEditorEventDetail.<>c__DisplayClass64_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._curEvent == null;
			if (!flag)
			{
				CS$<>8__locals1.eventName = this._curEvent.EventName;
				bool flag2 = !string.IsNullOrEmpty(CS$<>8__locals1.eventName);
				if (flag2)
				{
					string confirmInfo = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_EventDeleteConfirm, CS$<>8__locals1.eventName);
					DialogCmd cmd = new DialogCmd
					{
						Type = 1,
						Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
						Content = confirmInfo,
						Yes = new Action(CS$<>8__locals1.<OnDeleteEvent>g__DoDelete|0)
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					CS$<>8__locals1.<OnDeleteEvent>g__DoDelete|0();
				}
			}
		}

		// Token: 0x06004BA1 RID: 19361 RVA: 0x002397B4 File Offset: 0x002379B4
		private void OnCopyGuid()
		{
			bool flag = this._curEvent != null;
			if (flag)
			{
				GUIUtility.systemCopyBuffer = "\"" + this._curEvent.EventGuid + "\"";
				EventEditorNotes.Instance.AddNote(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_CopyGuidOK));
			}
		}

		// Token: 0x06004BA2 RID: 19362 RVA: 0x00239808 File Offset: 0x00237A08
		private void OnForceSingleToggleValueChange(bool isForceSingle)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				bool preForceSingle = this._curEvent.ForceSingle;
				bool flag2 = preForceSingle == isForceSingle;
				if (!flag2)
				{
					CToggle forceSingleToggle = this.toggleForceSingle;
					OperateCommand cmd = new OperateCommand("SetForceSingle")
					{
						Do = delegate()
						{
							this._curEvent.ForceSingle = isForceSingle;
							forceSingleToggle.isOn = isForceSingle;
						},
						Undo = delegate()
						{
							this._curEvent.ForceSingle = preForceSingle;
							forceSingleToggle.isOn = preForceSingle;
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BA3 RID: 19363 RVA: 0x002398B0 File Offset: 0x00237AB0
		private void InitEventGroupInput(TMP_InputField groupInput)
		{
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			List<string> searchGroupList = new List<string>();
			UI_SearchResultShow.OnSelectEvent <>9__5;
			groupInput.onValueChanged.AddListener(delegate(string str)
			{
				bool flag = this._curEvent == null;
				if (!flag)
				{
					searchGroupList.Clear();
					foreach (KeyValuePair<string, string> pair in model.EventGroupInfoDic)
					{
						bool flag2 = pair.Value.Contains(str);
						if (flag2)
						{
							searchGroupList.Add(pair.Value);
						}
					}
					bool flag3 = searchGroupList.Count <= 0;
					if (flag3)
					{
						bool flag4 = UIManager.Instance.IsElementActive(UIElement.SearchResultShow);
						if (flag4)
						{
							UIManager.Instance.HideUI(UIElement.SearchResultShow);
						}
					}
					else
					{
						List<string> showDataList = new List<string>();
						showDataList.AddRange(searchGroupList);
						bool flag5 = UIManager.Instance.IsElementActive(UIElement.SearchResultShow);
						if (flag5)
						{
							UI_SearchResultShow ui_SearchResultShow = UIElement.SearchResultShow.UiBase as UI_SearchResultShow;
							if (ui_SearchResultShow != null)
							{
								ui_SearchResultShow.UpdateData(showDataList, null);
							}
						}
						else
						{
							ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
							argBox.SetObject("Data", showDataList);
							ArgumentBox argumentBox = argBox;
							string key = "OnSelect";
							UI_SearchResultShow.OnSelectEvent arg;
							if ((arg = <>9__5) == null)
							{
								arg = (<>9__5 = delegate(int dataIndex, string _)
								{
									bool flag6 = model.EventGroupInfoDic.ContainsValue(searchGroupList[dataIndex]);
									if (flag6)
									{
										TMP_InputField.SubmitEvent onEndEdit = groupInput.onEndEdit;
										if (onEndEdit != null)
										{
											onEndEdit.Invoke(searchGroupList[dataIndex]);
										}
									}
									else
									{
										TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_SelectOrInputValidEventGroup), true);
									}
								});
							}
							argumentBox.SetObject(key, arg);
							argBox.SetObject("Trans", groupInput.transform.parent.GetComponent<RectTransform>());
							argBox.Set("AutoSize", true);
							UIElement.SearchResultShow.SetOnInitArgs(argBox);
							UIManager.Instance.ShowUI(UIElement.SearchResultShow, true);
						}
					}
				}
			});
			groupInput.onEndEdit.AddListener(delegate(string str)
			{
				bool flag = this._curEvent == null;
				if (!flag)
				{
					bool flag2 = model.EventGroupInfoDic.ContainsValue(str);
					if (flag2)
					{
						foreach (KeyValuePair<string, string> pair in model.EventGroupInfoDic)
						{
							bool flag3 = pair.Value == str;
							if (flag3)
							{
								base.<InitEventGroupInput>g__SetEventGroup|0(pair.Key);
								groupInput.SetTextWithoutNotify(str);
								break;
							}
						}
					}
					else
					{
						bool flag4 = string.IsNullOrEmpty(str);
						if (flag4)
						{
							string curGroupKey = this._curEvent.EventGroup;
							bool flag5 = model.EventGroupInfoDic.ContainsKey(curGroupKey);
							if (flag5)
							{
								groupInput.SetTextWithoutNotify(model.EventGroupInfoDic[curGroupKey]);
							}
						}
					}
				}
			});
		}

		// Token: 0x06004BA4 RID: 19364 RVA: 0x00239924 File Offset: 0x00237B24
		private void OnEventTypeDropdownValueChange(int newValue)
		{
			EventEditorEventDetail.<>c__DisplayClass68_0 CS$<>8__locals1 = new EventEditorEventDetail.<>c__DisplayClass68_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.newValue = newValue;
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorEventDetail.<>c__DisplayClass68_0 CS$<>8__locals2 = CS$<>8__locals1;
				EEventType newValue2 = (EEventType)CS$<>8__locals1.newValue;
				CS$<>8__locals2.newEventType = newValue2.ToString();
				CS$<>8__locals1.preEventType = this._curEvent.EventType;
				bool flag2 = !string.IsNullOrEmpty(CS$<>8__locals1.preEventType) && CS$<>8__locals1.preEventType == CS$<>8__locals1.newEventType;
				if (!flag2)
				{
					CS$<>8__locals1.preValue = 0;
					bool flag3 = !string.IsNullOrEmpty(CS$<>8__locals1.preEventType);
					if (flag3)
					{
						EEventType type;
						bool flag4 = Enum.TryParse<EEventType>(CS$<>8__locals1.preEventType, out type);
						if (flag4)
						{
							CS$<>8__locals1.preValue = (int)type;
						}
					}
					OperateCommand cmd = new OperateCommand("ChangeEventType")
					{
						Do = delegate()
						{
							CS$<>8__locals1.<>4__this._curEvent.EventType = CS$<>8__locals1.newEventType;
							bool flag5 = CS$<>8__locals1.<>4__this.eventTypeDropDown.value != CS$<>8__locals1.newValue;
							if (flag5)
							{
								CS$<>8__locals1.<>4__this.eventTypeDropDown.value = CS$<>8__locals1.newValue;
							}
						},
						Undo = delegate()
						{
							CS$<>8__locals1.<>4__this._curEvent.EventType = CS$<>8__locals1.preEventType;
							bool flag5 = CS$<>8__locals1.<>4__this.eventTypeDropDown.value != CS$<>8__locals1.preValue;
							if (flag5)
							{
								CS$<>8__locals1.<>4__this.eventTypeDropDown.value = CS$<>8__locals1.preValue;
							}
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BA5 RID: 19365 RVA: 0x00239A28 File Offset: 0x00237C28
		private void OnEventTriggerBeginEdit(string val)
		{
			UI_SearchResultShow.ShowInputHint(string.Empty, this.eventTriggerInputField, this._triggerTypeList, null);
		}

		// Token: 0x06004BA6 RID: 19366 RVA: 0x00239A43 File Offset: 0x00237C43
		private void OnEventTriggerChanged(string val)
		{
			UI_SearchResultShow.ShowInputHint(val, this.eventTriggerInputField, this._triggerTypeList, null);
		}

		// Token: 0x06004BA7 RID: 19367 RVA: 0x00239A5C File Offset: 0x00237C5C
		private void OnEventTriggerEndEdit(string val)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string prevCode = string.Empty;
				bool flag2 = !string.IsNullOrEmpty(this._curEvent.TriggerType);
				if (flag2)
				{
					prevCode = this._curEvent.TriggerType;
				}
				EventTriggerTypeItem prevEventTrigger = EventTriggerType.Instance.GetByKeyCode(prevCode);
				string prevRefName = EventTriggerType.Instance.GetRefName((prevEventTrigger != null) ? prevEventTrigger.TemplateId : -1);
				bool hasPrev = prevEventTrigger != null;
				int templateId;
				bool flag3 = !EventTriggerType.Instance.RefNameMap.TryGetValue(val, out templateId);
				if (flag3)
				{
					this.eventTriggerInputField.SetTextWithoutNotify(prevRefName);
				}
				else
				{
					EventTriggerTypeItem newEventTrigger = EventTriggerType.Instance[val];
					string keyCode = ((newEventTrigger != null) ? newEventTrigger.KeyCode : null) ?? "None";
					bool flag4 = prevCode == keyCode;
					if (!flag4)
					{
						bool flag5 = newEventTrigger == null && val != "None";
						if (flag5)
						{
							this.eventTriggerInputField.SetTextWithoutNotify(prevRefName);
						}
						else
						{
							OperateCommand cmd = new OperateCommand("ChangeEventTriggerType")
							{
								Do = delegate()
								{
									this._curEvent.TriggerType = keyCode;
									this.eventTriggerInputField.SetTextWithoutNotify(val);
								},
								Undo = delegate()
								{
									bool flag6 = !hasPrev;
									if (!flag6)
									{
										this._curEvent.TriggerType = prevCode;
										this.eventTriggerInputField.SetTextWithoutNotify(prevRefName);
									}
								}
							};
							this.OperateStack.Execute(cmd, true);
							this.SetDirty();
						}
					}
				}
			}
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x00239BF4 File Offset: 0x00237DF4
		private void OnEventNameInputEndEdit(string newName)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string preName = this._curEvent.EventName;
				bool flag2 = preName == newName;
				if (!flag2)
				{
					TMP_InputField eventNameInput = this.eventNameInputField;
					OperateCommand cmd = new OperateCommand("ChangeEventName")
					{
						Do = delegate()
						{
							this._curEvent.EventName = newName;
							eventNameInput.text = newName;
						},
						Undo = delegate()
						{
							this._curEvent.EventName = preName;
							eventNameInput.text = preName;
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BA9 RID: 19369 RVA: 0x00239CA0 File Offset: 0x00237EA0
		private void OnEventSortingOrderInputEndEdit(string orderString)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				short newOrder;
				bool flag2 = !short.TryParse(orderString, out newOrder);
				if (!flag2)
				{
					newOrder = Math.Clamp(newOrder, 0, short.MaxValue);
					short preOrder = short.MinValue;
					bool flag3 = this._curEvent.EventOrder >= 0;
					if (flag3)
					{
						preOrder = this._curEvent.EventOrder;
					}
					bool flag4 = preOrder == newOrder;
					if (!flag4)
					{
						TMP_InputField orderInput = this.eventOrderInputField;
						OperateCommand cmd = new OperateCommand("ChangeEventOrder")
						{
							Do = delegate()
							{
								this._curEvent.EventOrder = newOrder;
								orderInput.text = newOrder.ToString();
							},
							Undo = delegate()
							{
								bool flag5 = preOrder == short.MinValue;
								if (!flag5)
								{
									this._curEvent.EventOrder = preOrder;
									orderInput.text = preOrder.ToString();
								}
							}
						};
						this.OperateStack.Execute(cmd, true);
						this.SetDirty();
					}
				}
			}
		}

		// Token: 0x06004BAA RID: 19370 RVA: 0x00239D98 File Offset: 0x00237F98
		private void OnDecideRoleInputEndEdit(string newRoleKey)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string preRoleKey = this._curEvent.DecideRole;
				bool flag2 = preRoleKey == newRoleKey;
				if (!flag2)
				{
					TMP_InputField decideRoleInput = this.decideRoleKeyInputField;
					OperateCommand cmd = new OperateCommand("ChangeDecideRoleKey")
					{
						Do = delegate()
						{
							this._curEvent.DecideRole = newRoleKey;
							decideRoleInput.text = newRoleKey;
							EventEditorEventPreview.Instance.UpdateOptions();
						},
						Undo = delegate()
						{
							this._curEvent.DecideRole = preRoleKey;
							decideRoleInput.text = preRoleKey;
							EventEditorEventPreview.Instance.UpdateOptions();
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BAB RID: 19371 RVA: 0x00239E44 File Offset: 0x00238044
		private void OnTargetRoleInputEndEdit(string newRoleKey)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string preRoleKey = this._curEvent.TargetRole;
				bool flag2 = preRoleKey == newRoleKey;
				if (!flag2)
				{
					TMP_InputField targetRoleInput = this.targetRoleKeyInputField;
					OperateCommand cmd = new OperateCommand("ChangeTargetRoleKey")
					{
						Do = delegate()
						{
							this._curEvent.TargetRole = newRoleKey;
							targetRoleInput.text = newRoleKey;
							EventEditorEventPreview.Instance.RefreshTargetRole();
						},
						Undo = delegate()
						{
							this._curEvent.TargetRole = preRoleKey;
							targetRoleInput.text = preRoleKey;
							EventEditorEventPreview.Instance.RefreshTargetRole();
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BAC RID: 19372 RVA: 0x00239EF0 File Offset: 0x002380F0
		private void OnEventTextureNameInputEndEdit(string newTextureName)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				string preTextureName = string.Empty;
				bool flag2 = !string.IsNullOrEmpty(this._curEvent.EventTexture);
				if (flag2)
				{
					preTextureName = this._curEvent.EventTexture;
				}
				bool flag3 = preTextureName == newTextureName;
				if (!flag3)
				{
					TMP_InputField texNameInput = this.eventTextureNameInputField;
					OperateCommand cmd = new OperateCommand("ChangeEventTexture")
					{
						Do = delegate()
						{
							this._curEvent.EventTexture = newTextureName;
							texNameInput.SetTextWithoutNotify(newTextureName);
							EventEditorEventPreview.Instance.UpdateTexture();
						},
						Undo = delegate()
						{
							this._curEvent.EventTexture = preTextureName;
							texNameInput.SetTextWithoutNotify(preTextureName);
							EventEditorEventPreview.Instance.UpdateTexture();
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BAD RID: 19373 RVA: 0x00239FC0 File Offset: 0x002381C0
		private void OnControlMaskToggleChange(bool isControlMask)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				bool preControlState = this._curEvent.ControlMask;
				bool flag2 = preControlState == isControlMask;
				if (!flag2)
				{
					this.goLineMaskAction.SetActive(isControlMask);
					this.goLineMaskTween.SetActive(isControlMask);
					CToggle toggle = this.toggleControlMask;
					OperateCommand cmd = new OperateCommand("ChangeMaskControlState")
					{
						Do = delegate()
						{
							this._curEvent.ControlMask = isControlMask;
							toggle.isOn = isControlMask;
							bool isControlMask2 = isControlMask;
							if (isControlMask2)
							{
								this.maskControlTypeDropDown.value = (int)this._curEvent.ControlMaskCode;
								this.maskTweenTimeInputField.text = this._curEvent.MaskTweenTime.ToString(CultureInfo.InvariantCulture);
							}
							this.goLineMaskAction.SetActive(isControlMask);
							this.goLineMaskTween.SetActive(isControlMask);
						},
						Undo = delegate()
						{
							bool preControlState;
							this._curEvent.ControlMask = preControlState;
							toggle.isOn = preControlState;
							this.goLineMaskAction.SetActive(preControlState);
							this.goLineMaskTween.SetActive(preControlState);
							preControlState = preControlState;
							if (preControlState)
							{
								this.maskControlTypeDropDown.value = (int)this._curEvent.ControlMaskCode;
								this.maskTweenTimeInputField.text = this._curEvent.MaskTweenTime.ToString(CultureInfo.InvariantCulture);
							}
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x0023A090 File Offset: 0x00238290
		private void OnControlMaskTypeDropDownChange(int newCode)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				int preCode = -1;
				bool flag2 = this._curEvent.ControlMaskCode > 0;
				if (flag2)
				{
					preCode = (int)this._curEvent.ControlMaskCode;
				}
				bool flag3 = preCode == newCode;
				if (!flag3)
				{
					CDropdown dropdown = this.maskControlTypeDropDown;
					OperateCommand cmd = new OperateCommand("ChangeMaskControlType")
					{
						Do = delegate()
						{
							this._curEvent.ControlMaskCode = (byte)newCode;
							dropdown.value = newCode;
							dropdown.RefreshShownValue();
						},
						Undo = delegate()
						{
							this._curEvent.ControlMaskCode = (byte)preCode;
							dropdown.value = preCode;
							dropdown.RefreshShownValue();
						}
					};
					this.OperateStack.Execute(cmd, true);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BAF RID: 19375 RVA: 0x0023A154 File Offset: 0x00238354
		private void OnMaskTweenTimeInputFieldEndEdit(string newTimeString)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				float newTime;
				bool flag2 = !float.TryParse(newTimeString, out newTime);
				if (!flag2)
				{
					string preTimeString = this._curEvent.MaskTweenTime.ToString(CultureInfo.InvariantCulture);
					float preTime = 0f;
					bool flag3 = !string.IsNullOrEmpty(preTimeString);
					if (flag3)
					{
						float.TryParse(preTimeString, out preTime);
					}
					bool flag4 = Math.Abs(preTime - newTime) < 0.01f;
					if (!flag4)
					{
						TMP_InputField input = this.maskTweenTimeInputField;
						OperateCommand cmd = new OperateCommand("ChangeMaskTweenTime")
						{
							Do = delegate()
							{
								this._curEvent.MaskTweenTime = newTime;
								input.text = newTime.ToString("f2");
							},
							Undo = delegate()
							{
								bool flag5 = Math.Abs(preTime - float.MinValue) < 0.01f;
								if (!flag5)
								{
									this._curEvent.MaskTweenTime = preTime;
									input.text = preTime.ToString("f2");
								}
							}
						};
						this.OperateStack.Execute(cmd, true);
						this.SetDirty();
					}
				}
			}
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x0023A24C File Offset: 0x0023844C
		private bool CanPlayAudio()
		{
			bool flag = this._curEvent == null || string.IsNullOrEmpty(this._curEvent.AudioName);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string audioName = this._curEvent.AudioName;
				bool flag2 = string.IsNullOrEmpty(audioName);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = File.Exists(audioName);
					if (flag3)
					{
						result = true;
					}
					else
					{
						audioName = Path.Combine(Save.GetEventSaveDir(this._curEvent), audioName);
						result = File.Exists(audioName);
					}
				}
			}
			return result;
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x0023A2C4 File Offset: 0x002384C4
		private void OnPlayAudioToggleValueChange(bool isPlay)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				this.btnSelectAudio.gameObject.SetActive(isPlay);
				string audioName = string.Empty;
				bool flag2 = !string.IsNullOrEmpty(this._curEvent.AudioName);
				if (flag2)
				{
					audioName = this._curEvent.AudioName;
				}
				this.btnPlayAudio.gameObject.SetActive(this.CanPlayAudio());
				this.txtMeshAudioFilePath.text = audioName;
			}
		}

		// Token: 0x06004BB2 RID: 19378 RVA: 0x0023A340 File Offset: 0x00238540
		private void OnSelectAudio()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				OpenFileName openFileName = new OpenFileName();
				openFileName.structSize = Marshal.SizeOf<OpenFileName>(openFileName);
				openFileName.filter = "(*.wav;*.ogg)\0*.wav;*.ogg";
				openFileName.file = new string(new char[256]);
				openFileName.maxFile = openFileName.file.Length;
				openFileName.fileTitle = new string(new char[64]);
				openFileName.maxFileTitle = openFileName.fileTitle.Length;
				openFileName.initialDir = PlayerPrefs.GetString("LastSelectAudioKey", Application.dataPath.Replace('/', '\\'));
				openFileName.flags = 530440;
				bool unityOpenFileName = LocalDialog.GetUnityOpenFileName(openFileName);
				if (unityOpenFileName)
				{
					string audioName = openFileName.file;
					bool flag2 = !string.IsNullOrEmpty(audioName);
					if (flag2)
					{
						FileInfo fileInfo = new FileInfo(audioName);
						PlayerPrefs.SetString("LastSelectAudioKey", fileInfo.DirectoryName);
					}
					TextMeshProUGUI audioPathText = this.txtMeshAudioFilePath;
					CButton playButton = this.btnPlayAudio;
					string prevAudioName = string.Empty;
					bool flag3 = !string.IsNullOrEmpty(this._curEvent.AudioName);
					if (flag3)
					{
						prevAudioName = this._curEvent.AudioName;
					}
					bool flag4 = prevAudioName == audioName;
					if (!flag4)
					{
						OperateCommand cmd = new OperateCommand("SetAudioName")
						{
							Do = delegate()
							{
								this._curEvent.AudioName = audioName;
								audioPathText.text = audioName;
								playButton.gameObject.SetActive(this.CanPlayAudio());
							},
							Undo = delegate()
							{
								this._curEvent.AudioName = prevAudioName;
								audioPathText.text = prevAudioName;
								playButton.gameObject.SetActive(this.CanPlayAudio());
							}
						};
						this.OperateStack.Execute(cmd, true);
						this.SetDirty();
					}
				}
			}
		}

		// Token: 0x06004BB3 RID: 19379 RVA: 0x0023A4F8 File Offset: 0x002386F8
		private void DoPlayAudio()
		{
			this.lineAudio.clip = this._cachedAudioClip;
			this.lineAudio.Play();
		}

		// Token: 0x06004BB4 RID: 19380 RVA: 0x0023A519 File Offset: 0x00238719
		private IEnumerator LoadExternalAudio()
		{
			string audioFilePath = this._curEvent.AudioName;
			bool flag = !File.Exists(audioFilePath);
			if (flag)
			{
				audioFilePath = Path.Combine(Save.GetEventSaveDir(this._curEvent), audioFilePath);
				bool flag2 = !File.Exists(audioFilePath);
				if (flag2)
				{
					yield break;
				}
			}
			WWW www = new WWW(audioFilePath);
			yield return www;
			AudioType audioType = AudioType.OGGVORBIS;
			bool flag3 = audioFilePath.EndsWith(".wav");
			if (flag3)
			{
				audioType = AudioType.WAV;
			}
			AudioClip clip = www.GetAudioClip(false, false, audioType);
			bool flag4 = null != clip;
			if (flag4)
			{
				clip.name = Path.GetFileNameWithoutExtension(audioFilePath);
				this._cachedAudioClip = clip;
				this.DoPlayAudio();
			}
			yield break;
		}

		// Token: 0x06004BB5 RID: 19381 RVA: 0x0023A528 File Offset: 0x00238728
		private void OnPlayAudio()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				bool flag2 = string.IsNullOrEmpty(this._curEvent.AudioName);
				if (!flag2)
				{
					string audioFileName = this._curEvent.AudioName;
					string audioName = Path.GetFileNameWithoutExtension(audioFileName);
					this.lineAudio.Stop();
					bool flag3 = null != this._cachedAudioClip && this._cachedAudioClip.name == audioName;
					if (flag3)
					{
						this.DoPlayAudio();
					}
					else
					{
						base.StopAllCoroutines();
						base.StartCoroutine(this.LoadExternalAudio());
					}
				}
			}
		}

		// Token: 0x0400347C RID: 13436
		public static EventEditorEventDetail Instance;

		// Token: 0x0400347D RID: 13437
		[SerializeField]
		private TextMeshProUGUI txtMeshGuidValue;

		// Token: 0x0400347E RID: 13438
		[SerializeField]
		private CButton btnCopyGuid;

		// Token: 0x0400347F RID: 13439
		[SerializeField]
		private TMP_InputField eventGroupInput;

		// Token: 0x04003480 RID: 13440
		[SerializeField]
		private CDropdown eventTypeDropDown;

		// Token: 0x04003481 RID: 13441
		[SerializeField]
		private TMP_InputField eventTriggerInputField;

		// Token: 0x04003482 RID: 13442
		[SerializeField]
		private TMP_InputField eventNameInputField;

		// Token: 0x04003483 RID: 13443
		[SerializeField]
		private TMP_InputField eventOrderInputField;

		// Token: 0x04003484 RID: 13444
		[SerializeField]
		private TMP_InputField decideRoleKeyInputField;

		// Token: 0x04003485 RID: 13445
		[SerializeField]
		private TMP_InputField targetRoleKeyInputField;

		// Token: 0x04003486 RID: 13446
		[SerializeField]
		private TMP_InputField eventTextureNameInputField;

		// Token: 0x04003487 RID: 13447
		[SerializeField]
		private CButton btnEditCode;

		// Token: 0x04003488 RID: 13448
		[SerializeField]
		private CButton btnDeleteEvent;

		// Token: 0x04003489 RID: 13449
		[SerializeField]
		private CToggle toggleForceSingle;

		// Token: 0x0400348A RID: 13450
		[SerializeField]
		private CButton btnSaveEvent;

		// Token: 0x0400348B RID: 13451
		[SerializeField]
		private CButton btnEditLanguage;

		// Token: 0x0400348C RID: 13452
		[SerializeField]
		private CDropdown languageDropDown;

		// Token: 0x0400348D RID: 13453
		[SerializeField]
		private CToggle toggleControlMask;

		// Token: 0x0400348E RID: 13454
		[SerializeField]
		private CDropdown maskControlTypeDropDown;

		// Token: 0x0400348F RID: 13455
		[SerializeField]
		private TMP_InputField maskTweenTimeInputField;

		// Token: 0x04003490 RID: 13456
		[SerializeField]
		private CButton btnCopyEvent;

		// Token: 0x04003491 RID: 13457
		[SerializeField]
		private CButton btnPasteEvent;

		// Token: 0x04003492 RID: 13458
		[SerializeField]
		private GameObject goLineBtnEditLanguage;

		// Token: 0x04003493 RID: 13459
		[SerializeField]
		private GameObject goLineLanguage;

		// Token: 0x04003494 RID: 13460
		[SerializeField]
		private GameObject goLineMaskAction;

		// Token: 0x04003495 RID: 13461
		[SerializeField]
		private GameObject goLineMaskTween;

		// Token: 0x04003496 RID: 13462
		[SerializeField]
		private CToggle toggleHasAudio;

		// Token: 0x04003497 RID: 13463
		[SerializeField]
		private TextMeshProUGUI txtMeshAudioFilePath;

		// Token: 0x04003498 RID: 13464
		[SerializeField]
		private CButton btnSelectAudio;

		// Token: 0x04003499 RID: 13465
		[SerializeField]
		private CButton btnPlayAudio;

		// Token: 0x0400349A RID: 13466
		[SerializeField]
		private AudioSource lineAudio;

		// Token: 0x0400349B RID: 13467
		[SerializeField]
		private EventEditorTagReplace panelTagReplace;

		// Token: 0x0400349C RID: 13468
		[SerializeField]
		private CButton btnTagReplace;

		// Token: 0x0400349D RID: 13469
		[SerializeField]
		private CButton btnEditScript;

		// Token: 0x0400349E RID: 13470
		[SerializeField]
		private CButton btnEditCondition;

		// Token: 0x0400349F RID: 13471
		[SerializeField]
		private CButton btnEditBoolState;

		// Token: 0x040034A0 RID: 13472
		private EventEditorData _curEvent;

		// Token: 0x040034A1 RID: 13473
		private AudioClip _cachedAudioClip;

		// Token: 0x040034A2 RID: 13474
		private bool _ignoreDirty;

		// Token: 0x040034A3 RID: 13475
		private List<string> _triggerTypeList;
	}
}
