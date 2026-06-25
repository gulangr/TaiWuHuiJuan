using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompDevLib.Interpreter;
using Config;
using Config.Common;
using DG.Tweening;
using EventEditor.EventScript;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.EventEditor.UIScripts.Migrate;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EventEditor
{
	// Token: 0x02000648 RID: 1608
	public class EventEditorScript : EventEditorSubPageBase
	{
		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x06004C59 RID: 19545 RVA: 0x002403CA File Offset: 0x0023E5CA
		public int SelectedCount
		{
			get
			{
				return this._selectedInstructions.Count;
			}
		}

		// Token: 0x06004C5A RID: 19546 RVA: 0x002403D7 File Offset: 0x0023E5D7
		public static void Init(EventEditorScript instance)
		{
			EventEditorScript.Instance = instance;
			EventEditorScript.Instance.InternalInit();
			EventEditorScript.Instance.Hide();
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x002403F8 File Offset: 0x0023E5F8
		protected override void InternalInit()
		{
			EventEditorScriptConsole.Init(this.scriptConsole);
			this.showLogTrigger.ClearAndAddListener(new Action(this.OnShowLogClick));
			this.OperateStack = new OperateStack(32);
			if (this._instructions == null)
			{
				this._instructions = new List<EventEditorInstruction>();
			}
			this._functions = new Dictionary<int, GameObject>();
			this._interpreter = new Interpreter<BasicContext>(true);
			this.functionToggleGroup.Init(-1);
			this.functionToggleGroup.OnActiveIndexChange += delegate(int _, int _)
			{
				this.UpdateShownFunctions();
			};
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnConfirm));
			this.buttonCancel.ClearAndAddListener(new Action(this.OnCancel));
			this.redoBtn.ClearAndAddListener(new Action(this.Redo));
			this.undoBtn.ClearAndAddListener(new Action(this.Undo));
			this.selectAllBtn.ClearAndAddListener(new Action(this.SelectAll));
			this.copyBtn.ClearAndAddListener(new Action(this.CopySelectedInstructions));
			this.pasteBtn.ClearAndAddListener(new Action(this.PasteCopiedInstructionsAtEnd));
			this.monitorToggle.onValueChanged.RemoveAllListeners();
			this.monitorToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetMonitored));
			this._model = SingletonObject.getInstance<EventEditorModel>();
			this._listenerId = GameDataBridge.RegisterListener(delegate(List<NotificationWrapper> notificationWrappers)
			{
				foreach (NotificationWrapper wrapper in notificationWrappers)
				{
					Notification notification = wrapper.Notification;
					bool flag = notification.Type == 1 && notification.DomainId == 12 && notification.MethodId == 61;
					if (flag)
					{
						List<int> result = new List<int>();
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
						this._implementedFunctions = new HashSet<int>(result);
						this.InitializeFunctions();
						GameDataBridge.UnregisterListener(this._listenerId);
						this._listenerId = -1;
						break;
					}
				}
			});
			TaiwuEventDomainMethod.Call.GetImplementedFunctionIds(this._listenerId);
			this.showAllToggle.onValueChanged.RemoveAllListeners();
			this.showAllToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetShowAllFunctions));
			this.functionSearch.onValueChanged.RemoveAllListeners();
			this.functionSearch.onValueChanged.AddListener(new UnityAction<string>(this.SearchFunctions));
		}

		// Token: 0x06004C5C RID: 19548 RVA: 0x002405E8 File Offset: 0x0023E7E8
		private void OnShowLogClick()
		{
			bool isShowing = EventEditorScriptConsole.Instance.IsShowing;
			if (isShowing)
			{
				EventEditorScriptConsole.Instance.Hide();
			}
			else
			{
				EventEditorScriptConsole.Instance.Show();
			}
		}

		// Token: 0x06004C5D RID: 19549 RVA: 0x0024061C File Offset: 0x0023E81C
		public void GotoLine(int index)
		{
			bool flag = !this._instructions.CheckIndex(index);
			if (!flag)
			{
				EventEditorInstruction instruction = this._instructions[index];
				this.instructionScroll.ScrollTo(instruction.transform as RectTransform, 0.3f);
				this.SelectSingleInstruction(index);
			}
		}

		// Token: 0x06004C5E RID: 19550 RVA: 0x00240670 File Offset: 0x0023E870
		public void Show(EventScriptEditorData script, Action<EventScriptEditorData> onConfirm, sbyte scriptType, string eventGuid = "", string optionGuid = "")
		{
			this._model = SingletonObject.getInstance<EventEditorModel>();
			this._isConditionList = (scriptType == 2 || scriptType == 4 || scriptType == 6 || scriptType == 5);
			bool flag = scriptType == 1 || scriptType == 2 || scriptType == 0;
			if (flag)
			{
				this._scriptId = new EventScriptId(scriptType, new EventScriptRef(eventGuid, null));
			}
			else
			{
				bool flag2 = scriptType == 4 || scriptType == 5 || scriptType == 3;
				if (flag2)
				{
					this._scriptId = new EventScriptId(scriptType, new EventScriptRef(eventGuid, optionGuid));
				}
				else
				{
					this._scriptId = EventScriptId.Invalid;
				}
			}
			List<CToggle> toggleList = this.functionToggleGroup.GetAll();
			for (int i = 0; i < toggleList.Count; i++)
			{
				int key = i - 1;
				toggleList[i].gameObject.SetActive(key == 3 || !this._isConditionList);
			}
			this.functionToggleGroup.SetToFirstInteractable(false);
			this._isDirty = false;
			this._onConfirm = onConfirm;
			this._scriptEditorData = (script ?? new EventScriptEditorData());
			this.UpdateScriptLabel(scriptType, eventGuid, optionGuid);
			this.ClearInstructions();
			foreach (EventInstructionEditorData instruction in this._scriptEditorData.Instructions)
			{
				EventEditorInstruction inst = this.LoadInstruction(instruction);
				this._instructions.Add(inst);
			}
			this.OperateStack.Clear();
			this._selectedInstructions.Clear();
			base.gameObject.SetActive(true);
			this.buttonConfirm.interactable = false;
			this.IsGroupSelect = false;
			if (this._currScriptIdentifiers == null)
			{
				this._currScriptIdentifiers = new Dictionary<int, HashSet<string>>();
			}
			this._currScriptIdentifiers.Clear();
			this.UpdateScriptIdentifiers();
			this.UpdateShownFunctions(this.showAllToggle.isOn, this.functionSearch.text);
			this.UpdateLineIndexes();
			this.CheckInstructionsLogic();
			this.UpdateMenuButtons();
		}

		// Token: 0x06004C5F RID: 19551 RVA: 0x00240880 File Offset: 0x0023EA80
		private void UpdateScriptLabel(sbyte scriptType, string eventGuid, string optionGuid)
		{
			bool flag = this._scriptEditorData == null || scriptType < 0;
			if (flag)
			{
				this.scriptLabel.text = "";
			}
			else
			{
				this.StringBuilder.Clear();
				this.StringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_ScriptType) + ": ");
				switch (scriptType)
				{
				case 0:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_GlobalScript));
					break;
				case 1:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_EventEnterScript));
					break;
				case 2:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_EventConditionList));
					break;
				case 3:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_OptionScript));
					break;
				case 4:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_OptionAvailableConditionList));
					break;
				case 5:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_OptionVisibleConditionList));
					break;
				case 6:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_AdventureRemakeTriggerCondition));
					break;
				case 7:
					this.StringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ScriptType_AdventureRemakeAdvanceMonth));
					break;
				}
				bool flag2 = scriptType == 0;
				if (flag2)
				{
					this.StringBuilder.Append("Guid: ");
					this.StringBuilder.AppendLine(eventGuid);
				}
				else
				{
					this.StringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_EventGuid) + ": ");
					this.StringBuilder.AppendLine(eventGuid);
					bool flag3 = scriptType == 3 || scriptType == 4 || scriptType == 5;
					if (flag3)
					{
						this.StringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_OptionGuid) + ": ");
						this.StringBuilder.AppendLine(optionGuid);
					}
				}
				this.scriptLabel.text = this.StringBuilder.ToString();
			}
		}

		// Token: 0x06004C60 RID: 19552 RVA: 0x00240A8D File Offset: 0x0023EC8D
		public override void Show()
		{
			this.Show(null, null, -1, "", "");
		}

		// Token: 0x06004C61 RID: 19553 RVA: 0x00240AA4 File Offset: 0x0023ECA4
		public override void Hide()
		{
			this._eventNameToGuid = null;
			this._eventGuidToName = null;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004C62 RID: 19554 RVA: 0x00240AC4 File Offset: 0x0023ECC4
		private void InitializeFunctions()
		{
			for (int i = this.functionScroll.Content.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(this.functionScroll.Content.GetChild(i).gameObject);
			}
			this._functions.Clear();
			using (IEnumerator<EventFunctionItem> enumerator = ((IEnumerable<EventFunctionItem>)EventFunction.Instance).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EventFunctionItem functionCfg = enumerator.Current;
					bool flag = !functionCfg.CanCreateManually;
					if (!flag)
					{
						GameObject functionObj = Object.Instantiate<GameObject>(this.functionTemplate, this.functionScroll.Content);
						EventEditorScriptFunctionTemplate functionRefers = functionObj.GetComponent<EventEditorScriptFunctionTemplate>();
						functionRefers.UserInt = functionCfg.TemplateId;
						functionRefers.funcName.SetText(functionCfg.Name, true);
						CButton button = functionRefers.button;
						button.interactable = this._implementedFunctions.Contains(functionCfg.TemplateId);
						button.ClearAndAddListener(delegate
						{
							this.CreateInstruction(functionCfg.TemplateId);
						});
						button.targetGraphic.color = this.GetFunctionColor(functionCfg);
						TooltipInvoker mouseTipDisplayer = functionRefers.GetComponent<TooltipInvoker>();
						TooltipInvoker tooltipInvoker = mouseTipDisplayer;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
						}
						mouseTipDisplayer.RuntimeParam.Set("CanStick", false);
						bool interactable = button.interactable;
						if (interactable)
						{
							mouseTipDisplayer.RuntimeParam.Set("arg0", functionCfg.Desc);
						}
						else
						{
							mouseTipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Combat_TeammateCommandBanReason_Internal));
						}
						this._functions.Add(functionCfg.TemplateId, functionObj);
					}
				}
			}
		}

		// Token: 0x06004C63 RID: 19555 RVA: 0x00240CD4 File Offset: 0x0023EED4
		private void SearchFunctions(string searchText)
		{
			bool showAll = this.showAllToggle.isOn;
			this.UpdateShownFunctions(showAll, searchText);
		}

		// Token: 0x06004C64 RID: 19556 RVA: 0x00240CF8 File Offset: 0x0023EEF8
		private void SetShowAllFunctions(bool showAll)
		{
			string searchText = this.functionSearch.text;
			this.UpdateShownFunctions(showAll, searchText);
		}

		// Token: 0x06004C65 RID: 19557 RVA: 0x00240D1C File Offset: 0x0023EF1C
		private void UpdateShownFunctions()
		{
			bool showAll = this.showAllToggle.isOn;
			string searchText = this.functionSearch.text;
			this.UpdateShownFunctions(showAll, searchText);
		}

		// Token: 0x06004C66 RID: 19558 RVA: 0x00240D4C File Offset: 0x0023EF4C
		private void UpdateShownFunctions(bool showAll, string searchText)
		{
			EEventFunctionType currType = (EEventFunctionType)(this._isConditionList ? 3 : (this.functionToggleGroup.GetActiveIndex() - 1));
			for (int i = 0; i < EventFunction.Instance.Count; i++)
			{
				GameObject functionObj;
				bool flag = !this._functions.TryGetValue(i, out functionObj);
				if (!flag)
				{
					EventFunctionItem functionCfg = EventFunction.Instance[i];
					bool matchType = functionCfg.Type == currType || currType < EEventFunctionType.Basic;
					bool flag2 = functionCfg.AllowedInCondition && !this._isConditionList;
					if (flag2)
					{
						matchType = false;
					}
					bool matchSearchResult = string.IsNullOrEmpty(searchText) || functionCfg.Name.Contains(searchText);
					bool matchImplemented = showAll || this._implementedFunctions.Contains(i);
					functionObj.SetActive(matchSearchResult && matchImplemented && matchType);
				}
			}
		}

		// Token: 0x06004C67 RID: 19559 RVA: 0x00240E28 File Offset: 0x0023F028
		public Color GetFunctionColor(EventFunctionItem functionCfg)
		{
			EEventFunctionType type = functionCfg.Type;
			if (!true)
			{
			}
			Color result;
			switch (type)
			{
			case EEventFunctionType.Basic:
				result = Color.green;
				break;
			case EEventFunctionType.UI:
				result = Color.cyan;
				break;
			case EEventFunctionType.Behavior:
				result = Color.white;
				break;
			case EEventFunctionType.Condition:
				result = Color.yellow;
				break;
			default:
				result = Color.gray;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06004C68 RID: 19560 RVA: 0x00240E8C File Offset: 0x0023F08C
		private void ClearInstructions()
		{
			foreach (EventEditorInstruction inst in this._instructions)
			{
				Object.Destroy(inst.gameObject);
			}
			this._instructions.Clear();
		}

		// Token: 0x06004C69 RID: 19561 RVA: 0x00240EF8 File Offset: 0x0023F0F8
		public void UpdateScriptIdentifiers()
		{
			foreach (HashSet<string> idSet in this._currScriptIdentifiers.Values)
			{
				idSet.Clear();
			}
			foreach (EventInstructionEditorData inst in this._scriptEditorData.Instructions)
			{
				EventFunctionItem funcCfg = inst.FunctionConfig;
				bool flag = funcCfg.ReturnValue < 0;
				if (!flag)
				{
					bool flag2 = string.IsNullOrEmpty(inst.AssignToVar);
					if (!flag2)
					{
						HashSet<string> idSet2;
						bool flag3 = !this._currScriptIdentifiers.TryGetValue(funcCfg.ReturnValue, out idSet2);
						if (flag3)
						{
							idSet2 = new HashSet<string>();
							this._currScriptIdentifiers.Add(funcCfg.ReturnValue, idSet2);
						}
						idSet2.Add(inst.AssignToVar);
					}
				}
			}
		}

		// Token: 0x06004C6A RID: 19562 RVA: 0x00241014 File Offset: 0x0023F214
		public void InitializeDisabledInstructions(Predicate<int> condition)
		{
			int i = 0;
			int count = EventEditorScript.Instance._instructions.Count;
			while (i < count)
			{
				EventEditorInstruction inst = EventEditorScript.Instance._instructions[i];
				bool flag = condition(i);
				if (!flag)
				{
					inst.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
					this.DisabledInstructionIndices.Add(i);
				}
				i++;
			}
		}

		// Token: 0x06004C6B RID: 19563 RVA: 0x00241080 File Offset: 0x0023F280
		public void ResetDisabledInstructions()
		{
			int i = 0;
			int count = EventEditorScript.Instance._instructions.Count;
			while (i < count)
			{
				EventEditorInstruction inst = EventEditorScript.Instance._instructions[i];
				inst.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
				i++;
			}
			this.DisabledInstructionIndices.Clear();
		}

		// Token: 0x06004C6C RID: 19564 RVA: 0x002410DC File Offset: 0x0023F2DC
		private bool CheckLoopLogic(List<int> loopIds, int ifLayer, int breakWithIfId, ref int returnId)
		{
			this.SetInstructionDisable(loopIds[0], false);
			this.SetInstructionDisable(loopIds[loopIds.Count - 1], false);
			bool flag = loopIds.Count <= 2;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool loopTerminationFound = false;
				int terminatedByBreakId = int.MaxValue;
				for (int i = 1; i < loopIds.Count - 1; i++)
				{
					EventEditorInstruction inst = EventEditorScript.Instance._instructions[loopIds[i]];
					int templateId = inst.Data.FunctionConfig.TemplateId;
					bool flag2 = loopIds[i] != breakWithIfId;
					if (flag2)
					{
						this.SetInstructionDisable(loopIds[i], false);
					}
					bool flag3 = loopTerminationFound;
					if (flag3)
					{
						bool flag4 = terminatedByBreakId != breakWithIfId;
						if (flag4)
						{
							this.SetInstructionDisable(loopIds[i], true);
							this.DisabledInstructionIndices.Add(loopIds[i]);
						}
					}
					else
					{
						bool flag5 = templateId == 4 || templateId == 9 || templateId == 13;
						if (flag5)
						{
							loopTerminationFound = true;
							bool flag6 = templateId == 9 && ifLayer <= 0;
							if (flag6)
							{
								returnId = Mathf.Min(returnId, loopIds[i]);
							}
							else
							{
								bool flag7 = templateId == 4;
								if (flag7)
								{
									terminatedByBreakId = loopIds[i];
								}
							}
						}
					}
				}
				result = loopTerminationFound;
			}
			return result;
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x00241248 File Offset: 0x0023F448
		private bool CheckIfLogic(List<int> ifIds, ref int breakWithIfId, int loopLayer)
		{
			this.SetInstructionDisable(ifIds[0], false);
			this.SetInstructionDisable(ifIds[ifIds.Count - 1], false);
			bool flag = ifIds.Count <= 2;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool returnFound = false;
				bool correct = true;
				for (int i = 1; i < ifIds.Count - 1; i++)
				{
					EventEditorInstruction inst = EventEditorScript.Instance._instructions[ifIds[i]];
					int templateId = inst.Data.FunctionConfig.TemplateId;
					bool flag2 = returnFound;
					if (flag2)
					{
						this.SetInstructionDisable(ifIds[i], true);
						this.DisabledInstructionIndices.Add(ifIds[i]);
					}
					else
					{
						this.SetInstructionDisable(ifIds[i], false);
						bool flag3 = templateId == 4;
						if (flag3)
						{
							bool flag4 = loopLayer > 0;
							if (flag4)
							{
								returnFound = true;
							}
							else
							{
								correct = false;
							}
							breakWithIfId = ifIds[i];
						}
						else
						{
							bool flag5 = templateId == 9 || templateId == 13;
							if (flag5)
							{
								returnFound = true;
							}
						}
					}
				}
				result = correct;
			}
			return result;
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x00241370 File Offset: 0x0023F570
		private void UpdateMenuButtons()
		{
			this.redoBtn.interactable = this.OperateStack.CanRedo;
			this.undoBtn.interactable = this.OperateStack.CanUndo;
			this.selectAllBtn.interactable = (this._instructions.Count > 0);
			this.copyBtn.interactable = (this._selectedInstructions.Count > 0);
			this.pasteBtn.interactable = (this._copiedInstructions.Count > 0 && this.CanPasteCopiedInstructionsAt(this._instructions.Count));
			this.monitorToggle.SetIsOnWithoutNotify(this._scriptId.IsValid() && this._model.ScriptRuntimeSettings.MonitoredScripts.Contains(this._scriptId));
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x00241448 File Offset: 0x0023F648
		private void CheckInstructionsLogic()
		{
			EventEditorScript.<>c__DisplayClass66_0 CS$<>8__locals1 = new EventEditorScript.<>c__DisplayClass66_0();
			bool returnFound = false;
			bool correct = true;
			this.DisabledInstructionIndices.Clear();
			Stack<int> scopeStack = EasyPool.Get<Stack<int>>();
			CS$<>8__locals1.scopeIndexStack = EasyPool.Get<Stack<int>>();
			Stack<int> scopeIndentStack = EasyPool.Get<Stack<int>>();
			int loopLayer = 0;
			int ifLayer = 0;
			int returnId = int.MaxValue;
			CS$<>8__locals1.breakWithIfId = -1;
			scopeStack.Clear();
			CS$<>8__locals1.scopeIndexStack.Clear();
			scopeIndentStack.Clear();
			List<int> idList = EasyPool.Get<List<int>>();
			idList.Clear();
			int i;
			Func<int, bool> <>9__3;
			Predicate<int> <>9__4;
			int i2;
			for (i = 0; i < EventEditorScript.Instance._instructions.Count; i = i2 + 1)
			{
				EventEditorInstruction inst = EventEditorScript.Instance._instructions[i];
				int templateId = inst.Data.FunctionConfig.TemplateId;
				idList.Add(i);
				bool indentNext = inst.Data.FunctionConfig.IndentNext;
				if (indentNext)
				{
					scopeStack.Push(templateId);
					CS$<>8__locals1.scopeIndexStack.Push(i);
					scopeIndentStack.Push(inst.Data.Indent);
					bool flag = templateId == 3;
					if (flag)
					{
						loopLayer++;
					}
					else
					{
						bool flag2 = templateId == 0;
						if (flag2)
						{
							ifLayer++;
						}
					}
				}
				else
				{
					bool flag3 = templateId == 5;
					if (flag3)
					{
						bool flag4 = scopeStack.Count == 0;
						if (flag4)
						{
							this.ShowError("非法的End", true);
							correct = false;
						}
						else
						{
							bool flag5 = scopeStack.Peek() == 3;
							if (flag5)
							{
								bool loopCorrect = this.CheckLoopLogic((from k in idList
								where k >= CS$<>8__locals1.scopeIndexStack.Peek() && k <= i
								select k).ToList<int>(), ifLayer, CS$<>8__locals1.breakWithIfId, ref returnId);
								idList.RemoveAll((int k) => k >= CS$<>8__locals1.scopeIndexStack.Peek() && k <= i);
								CS$<>8__locals1.scopeIndexStack.Pop();
								scopeStack.Pop();
								scopeIndentStack.Pop();
								loopLayer--;
								bool flag6 = !loopCorrect;
								if (flag6)
								{
									this.ShowError("循环错误！", true);
									correct = false;
								}
							}
							else
							{
								bool flag7 = scopeStack.Peek() == 0 || scopeStack.Peek() == 1 || scopeStack.Peek() == 2;
								if (flag7)
								{
									while (scopeStack.Count != 0 && (scopeStack.Peek() == 0 || scopeStack.Peek() == 1 || scopeStack.Peek() == 2) && scopeIndentStack.Peek() == inst.Data.Indent)
									{
										CS$<>8__locals1.breakWithIfId = -1;
										IEnumerable<int> source = idList;
										Func<int, bool> predicate;
										if ((predicate = <>9__3) == null)
										{
											predicate = (<>9__3 = ((int k) => k >= CS$<>8__locals1.scopeIndexStack.Peek() && k <= i));
										}
										bool ifCorrect = this.CheckIfLogic(source.Where(predicate).ToList<int>(), ref CS$<>8__locals1.breakWithIfId, loopLayer);
										List<int> list = idList;
										Predicate<int> match;
										if ((match = <>9__4) == null)
										{
											match = (<>9__4 = ((int k) => k >= CS$<>8__locals1.scopeIndexStack.Peek() && k < i && k != CS$<>8__locals1.breakWithIfId));
										}
										list.RemoveAll(match);
										CS$<>8__locals1.scopeIndexStack.Pop();
										scopeStack.Pop();
										scopeIndentStack.Pop();
										bool flag8 = !ifCorrect && loopLayer <= 0;
										if (flag8)
										{
											this.ShowError("非法的Break", true);
											correct = false;
										}
									}
									this.SetInstructionDisable(i, false);
									idList.RemoveAll((int k) => k == i);
									ifLayer--;
								}
								else
								{
									int start = CS$<>8__locals1.scopeIndexStack.Peek();
									for (int t = start; t <= i; t++)
									{
										this.SetInstructionDisable(t, false);
									}
									idList.RemoveAll((int k) => k >= start && k <= i);
									CS$<>8__locals1.scopeIndexStack.Pop();
									scopeStack.Pop();
									scopeIndentStack.Pop();
								}
							}
						}
					}
					else
					{
						bool flag9 = (templateId == 9 || templateId == 13) && inst.Data.Indent == 0;
						if (flag9)
						{
							returnId = Mathf.Min(returnId, i);
						}
					}
				}
				i2 = i;
			}
			bool flag10 = returnId != int.MaxValue;
			if (flag10)
			{
				this.SetInstructionDisable(returnId, false);
				for (int l = returnId + 1; l < EventEditorScript.Instance._instructions.Count; l++)
				{
					this.SetInstructionDisable(l, true);
				}
			}
			else
			{
				for (int j = 0; j < idList.Count; j++)
				{
					EventEditorInstruction inst2 = EventEditorScript.Instance._instructions[idList[j]];
					int templateId2 = inst2.Data.FunctionConfig.TemplateId;
					bool flag11 = templateId2 == 4;
					if (flag11)
					{
						this.ShowError("非法的Break", true);
						correct = false;
					}
					bool flag12 = returnFound;
					if (flag12)
					{
						this.SetInstructionDisable(idList[j], true);
						this.DisabledInstructionIndices.Add(idList[j]);
					}
					else
					{
						this.SetInstructionDisable(idList[j], false);
						bool flag13 = inst2.Data.FunctionConfig.TemplateId == 9 || inst2.Data.FunctionConfig.TemplateId == 13;
						if (flag13)
						{
							returnFound = true;
						}
					}
				}
			}
			bool flag14 = correct;
			if (flag14)
			{
				this.ClearErrorLog();
			}
			EasyPool.Free<Stack<int>>(scopeStack);
			EasyPool.Free<Stack<int>>(CS$<>8__locals1.scopeIndexStack);
			EasyPool.Free<Stack<int>>(scopeIndentStack);
			EasyPool.Free<List<int>>(idList);
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x00241A50 File Offset: 0x0023FC50
		private void SetInstructionDisable(int idx, bool disabled)
		{
			bool flag = idx < 0 || idx >= EventEditorScript.Instance._instructions.Count;
			if (flag)
			{
				throw new IndexOutOfRangeException();
			}
			EventEditorInstruction inst = EventEditorScript.Instance._instructions[idx];
			inst.GetComponent<DisableStyleRoot>().SetStyleEffect(disabled, false);
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x00241AA4 File Offset: 0x0023FCA4
		private void OfflineAddInstruction(int index, EventEditorInstruction instruction)
		{
			EventInstructionEditorData data = instruction.Data;
			bool flag = index == this._scriptEditorData.Instructions.Count;
			if (flag)
			{
				this._scriptEditorData.Instructions.Add(data);
				this._instructions.Add(instruction);
				instruction.transform.SetSiblingIndex(index);
			}
			else
			{
				this._scriptEditorData.Instructions.Insert(index, data);
				this._instructions.Insert(index, instruction);
				instruction.transform.SetSiblingIndex(index);
			}
			EventInstructionEditorData prevInst = (index > 0) ? this._scriptEditorData.Instructions[index - 1] : null;
			data.AdjustIndention(prevInst);
			instruction.SetIndent(data.Indent);
		}

		// Token: 0x06004C72 RID: 19570 RVA: 0x00241B60 File Offset: 0x0023FD60
		private void OfflineRemoveInstruction(int index)
		{
			EventEditorInstruction instruction = this._instructions[index];
			this._instructions.RemoveAt(index);
			this._scriptEditorData.Instructions.RemoveAt(index);
			Object.Destroy(instruction.gameObject);
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x00241BA8 File Offset: 0x0023FDA8
		private void GetInstructionGroup(int srcIndex, List<int> result)
		{
			EventEditorInstruction instruction = this._instructions[srcIndex];
			EventInstructionEditorData instData = instruction.Data;
			EventFunctionItem funcConfig = instData.FunctionConfig;
			bool indentNext = funcConfig.IndentNext;
			if (indentNext)
			{
				result.Add(srcIndex);
				for (int i = srcIndex + 1; i < this._instructions.Count; i++)
				{
					EventEditorInstruction currInst = this._instructions[i];
					EventInstructionEditorData currInstData = currInst.Data;
					EventFunctionItem currInstFuncConfig = currInstData.FunctionConfig;
					bool flag = currInstData.Indent > instData.Indent;
					if (flag)
					{
						result.Add(i);
					}
					else
					{
						bool flag2 = currInstData.Indent == instData.Indent;
						if (flag2)
						{
							List<int> requiredPreviousCommands = currInstFuncConfig.RequiredPreviousCommands;
							bool flag3 = requiredPreviousCommands != null && requiredPreviousCommands.Count > 0 && currInstFuncConfig.RequiredPreviousCommands.Contains(instData.FuncId);
							if (!flag3)
							{
								break;
							}
							requiredPreviousCommands = funcConfig.RequiredPreviousCommands;
							bool flag4 = requiredPreviousCommands != null && requiredPreviousCommands.Count > 0;
							if (flag4)
							{
								IEnumerable<int> union = currInstFuncConfig.RequiredPreviousCommands.Union(funcConfig.RequiredPreviousCommands);
								bool flag5 = union.SequenceEqual(currInstFuncConfig.RequiredPreviousCommands);
								if (flag5)
								{
									break;
								}
							}
							result.Add(i);
							bool flag6 = !currInstFuncConfig.IndentNext;
							if (flag6)
							{
								break;
							}
						}
						else
						{
							bool flag7 = funcConfig.FollowUp >= 0;
							if (flag7)
							{
								throw new Exception("Current scope is not exited properly");
							}
							break;
						}
					}
				}
			}
			else
			{
				List<int> requiredPreviousCommands = funcConfig.RequiredPreviousCommands;
				bool flag8 = requiredPreviousCommands != null && requiredPreviousCommands.Count > 0;
				if (flag8)
				{
					int rootIndex = this.GetRootInstructionIndex(srcIndex);
					bool flag9 = rootIndex < 0;
					if (flag9)
					{
						throw new Exception("Unable to find instruction dependency in the same scope.");
					}
					this.GetInstructionGroup(rootIndex, result);
				}
				else
				{
					result.Add(srcIndex);
				}
			}
		}

		// Token: 0x06004C74 RID: 19572 RVA: 0x00241D90 File Offset: 0x0023FF90
		private int GetRootInstructionIndex(int srcIndex)
		{
			EventEditorInstruction instruction = this._instructions[srcIndex];
			EventInstructionEditorData instData = instruction.Data;
			EventFunctionItem funcConfig = instData.FunctionConfig;
			int i = srcIndex - 1;
			while (i >= 0)
			{
				EventEditorInstruction currInst = this._instructions[i];
				EventInstructionEditorData currInstData = currInst.Data;
				EventFunctionItem currInstFuncConfig = currInstData.FunctionConfig;
				bool flag = currInstData.Indent == instData.Indent;
				if (flag)
				{
					bool flag2 = funcConfig.RequiredPreviousCommands.Contains(currInstData.FuncId);
					if (flag2)
					{
						return i;
					}
					List<int> requiredPreviousCommands = currInstFuncConfig.RequiredPreviousCommands;
					bool flag3 = requiredPreviousCommands != null && requiredPreviousCommands.Count > 0;
					if (!flag3)
					{
						break;
					}
				}
				else
				{
					bool flag4 = currInstData.Indent > instData.Indent;
					if (!flag4)
					{
						break;
					}
				}
				i--;
				continue;
				break;
			}
			return -1;
		}

		// Token: 0x06004C75 RID: 19573 RVA: 0x00241E70 File Offset: 0x00240070
		public bool IsInstructionSelected(int index)
		{
			return this._selectedInstructions.Contains(index);
		}

		// Token: 0x06004C76 RID: 19574 RVA: 0x00241E90 File Offset: 0x00240090
		public EventEditorInstruction GetInstruction(int index)
		{
			return this._instructions[index];
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x00241EB0 File Offset: 0x002400B0
		public void DeselectAllInstructions()
		{
			bool flag = this._selectedInstructions.Count == 0;
			if (!flag)
			{
				this.UpdateInstructionSelectionViews(this._selectedInstructions, false);
				this._selectedInstructions.Clear();
			}
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x00241EEC File Offset: 0x002400EC
		public void SelectInstruction(EventEditorInstruction instruction)
		{
			int index = this._instructions.IndexOf(instruction);
			bool flag = (CommonCommandKit.Shift.Check(UIElement.EventEditor, true, false, false, true, false) || CommonCommandKit.Shift.Check(UIElement.AdventureEditorRemake, true, false, false, true, false)) && this._selectedInstructions.Count > 0;
			if (flag)
			{
				int minIndex = Math.Min(index, this._selectedInstructions[0]);
				int val = index;
				List<int> selectedInstructions = this._selectedInstructions;
				int maxIndex = Math.Max(val, selectedInstructions[selectedInstructions.Count - 1]);
				this.SelectInstructionsInRange(minIndex, maxIndex);
			}
			else
			{
				this.SelectInstructionGroup(index);
			}
		}

		// Token: 0x06004C79 RID: 19577 RVA: 0x00241F8A File Offset: 0x0024018A
		private void SelectSingleInstruction(int index)
		{
			this.UpdateInstructionSelectionViews(this._selectedInstructions, false);
			this._selectedInstructions.Clear();
			this._selectedInstructions.Add(index);
			this.UpdateInstructionSelectionViews(this._selectedInstructions, true);
			this.IsGroupSelect = false;
		}

		// Token: 0x06004C7A RID: 19578 RVA: 0x00241FCC File Offset: 0x002401CC
		private void SelectInstructionsInRange(int fromIndex, int toIndex)
		{
			int fromIndent = this._instructions[fromIndex].Data.Indent;
			int toIndent = this._instructions[toIndex].Data.Indent;
			bool flag = fromIndent > toIndent;
			if (!flag)
			{
				for (int i = fromIndex + 1; i < toIndex; i++)
				{
					int currIndent = this._instructions[i].Data.Indent;
					bool flag2 = currIndent < fromIndent || currIndent < toIndent;
					if (flag2)
					{
						return;
					}
				}
				this.UpdateInstructionSelectionViews(this._selectedInstructions, false);
				this._selectedInstructions.Clear();
				for (int j = fromIndex; j <= toIndex; j++)
				{
					this._selectingInstructions.Clear();
					this.GetInstructionGroup(j, this._selectingInstructions);
					foreach (int index in this._selectingInstructions)
					{
						bool flag3 = !this._selectedInstructions.Contains(index);
						if (flag3)
						{
							this._selectedInstructions.Add(index);
						}
					}
				}
				this._selectedInstructions.Sort();
				this.UpdateInstructionSelectionViews(this._selectedInstructions, true);
				this.IsGroupSelect = true;
			}
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x00242138 File Offset: 0x00240338
		private void SelectInstructionGroup(int index)
		{
			this._selectingInstructions.Clear();
			bool flag = index < 0;
			if (!flag)
			{
				this.UpdateInstructionSelectionViews(this._selectedInstructions, false);
				bool flag2 = this._selectedInstructions.Contains(index);
				if (flag2)
				{
					bool flag3 = this._selectedInstructions.Count > 1;
					if (flag3)
					{
						this._selectedInstructions.Clear();
						this._selectedInstructions.Add(index);
						this.UpdateInstructionSelectionViews(this._selectedInstructions, true);
					}
					else
					{
						this._selectedInstructions.Clear();
					}
					this.IsGroupSelect = false;
				}
				else
				{
					this.GetInstructionGroup(index, this._selectingInstructions);
					this.IsGroupSelect = true;
					List<int> selectingInstructions = this._selectingInstructions;
					List<int> selectedInstructions = this._selectedInstructions;
					this._selectedInstructions = selectingInstructions;
					this._selectingInstructions = selectedInstructions;
					this._selectingInstructions.Clear();
					this.UpdateInstructionSelectionViews(this._selectedInstructions, true);
				}
			}
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x00242220 File Offset: 0x00240420
		private void UpdateInstructionSelectionViews(List<int> list, bool selected)
		{
			foreach (int selectedIndex in list)
			{
				EventEditorInstruction selectedInst = this._instructions[selectedIndex];
				selectedInst.Selected.SetActive(selected);
			}
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x00242288 File Offset: 0x00240488
		private void SetMonitored(bool monitored)
		{
			bool flag = !this._scriptId.IsValid();
			if (!flag)
			{
				EventScriptRuntimeSettings settings = this._model.ScriptRuntimeSettings;
				if (monitored)
				{
					settings.MonitoredScripts.Add(this._scriptId);
				}
				else
				{
					settings.MonitoredScripts.Remove(this._scriptId);
				}
				this._model.ScriptRuntimeSettings = settings;
			}
		}

		// Token: 0x06004C7E RID: 19582 RVA: 0x002422EE File Offset: 0x002404EE
		private void SelectAll()
		{
			this.SelectInstructionsInRange(0, this._instructions.Count - 1);
			this.UpdateMenuButtons();
		}

		// Token: 0x06004C7F RID: 19583 RVA: 0x0024230D File Offset: 0x0024050D
		public void PasteCopiedInstructionsAtEnd()
		{
			this.PasteCopiedInstructions(this._instructions.Count);
		}

		// Token: 0x06004C80 RID: 19584 RVA: 0x00242324 File Offset: 0x00240524
		public void CopySelectedInstructions()
		{
			this._copiedInstructions.Clear();
			foreach (int index in this._selectedInstructions)
			{
				this._copiedInstructions.Add(new EventInstructionEditorData(this._instructions[index].Data));
			}
			this.UpdateMenuButtons();
		}

		// Token: 0x06004C81 RID: 19585 RVA: 0x002423A8 File Offset: 0x002405A8
		public void CutSelectedInstructions()
		{
			this.CopySelectedInstructions();
			this.DeleteSelectedInstructions();
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x002423BC File Offset: 0x002405BC
		public bool CanPasteCopiedInstructionsAt(int index)
		{
			bool flag = this._copiedInstructions.Count == 0;
			return !flag && this.CanPlaceAtIndex(this._copiedInstructions[0].FunctionConfig, index);
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x002423FC File Offset: 0x002405FC
		public void PasteCopiedInstructions(int index)
		{
			bool flag = !this.CanPasteCopiedInstructionsAt(index);
			if (!flag)
			{
				this.DeselectAllInstructions();
				List<EventInstructionEditorData> copiedInstructions = new List<EventInstructionEditorData>(this._copiedInstructions);
				OperateCommand cmd = new OperateCommand("PasteCopiedInstructions")
				{
					Do = delegate()
					{
						for (int i = 0; i < copiedInstructions.Count; i++)
						{
							EventInstructionEditorData instData = new EventInstructionEditorData(copiedInstructions[i]);
							EventEditorInstruction instView = this.LoadInstruction(instData);
							this.OfflineAddInstruction(index + i, instView);
						}
					},
					Undo = delegate()
					{
						for (int i = copiedInstructions.Count - 1; i >= 0; i--)
						{
							this.OfflineRemoveInstruction(index + i);
						}
					}
				};
				this.ExecuteOperateCommand(cmd);
			}
		}

		// Token: 0x06004C84 RID: 19588 RVA: 0x00242484 File Offset: 0x00240684
		public void DeleteSelectedInstructions()
		{
			List<ValueTuple<int, EventInstructionEditorData>> removedList = new List<ValueTuple<int, EventInstructionEditorData>>();
			foreach (int index in this._selectedInstructions)
			{
				removedList.Add(new ValueTuple<int, EventInstructionEditorData>(index, this._instructions[index].Data));
			}
			this._selectedInstructions.Clear();
			this.IsGroupSelect = false;
			OperateCommand cmd = new OperateCommand("DeleteSelectedInstructions")
			{
				Do = delegate()
				{
					for (int i = removedList.Count - 1; i >= 0; i--)
					{
						this.OfflineRemoveInstruction(removedList[i].Item1);
					}
				},
				Undo = delegate()
				{
					for (int i = 0; i < removedList.Count; i++)
					{
						ValueTuple<int, EventInstructionEditorData> item = removedList[i];
						EventEditorInstruction inst = this.LoadInstruction(item.Item2);
						this.OfflineAddInstruction(item.Item1, inst);
					}
				}
			};
			this.ExecuteOperateCommand(cmd);
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x00242558 File Offset: 0x00240758
		public void OnConfirm()
		{
			this._isDirty = false;
			this.buttonConfirm.interactable = false;
			Action<EventScriptEditorData> onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm(this._scriptEditorData);
			}
		}

		// Token: 0x06004C86 RID: 19590 RVA: 0x00242588 File Offset: 0x00240788
		public void OnCancel()
		{
			bool isDirty = this._isDirty;
			if (isDirty)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.UI_InstructionEditor_Confirm_Edit),
					Content = LocalStringManager.Get(LanguageKey.UI_InstructionEditor_Confirm_Edit_Desc),
					Type = 3,
					Yes = new Action(this.OnConfirm),
					No = new Action(this.Hide),
					GroupYesText = LocalStringManager.Get(LanguageKey.LK_Yes),
					GroupNoText = LocalStringManager.Get(LanguageKey.LK_No)
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.Hide();
			}
		}

		// Token: 0x06004C87 RID: 19591 RVA: 0x0024264C File Offset: 0x0024084C
		public EventEditorInstruction LoadInstruction(EventInstructionEditorData instData)
		{
			GameObject instObj = Object.Instantiate<GameObject>(this.instructionTemplate, this.instructionScroll.Content);
			EventEditorInstruction inst = instObj.GetComponent<EventEditorInstruction>();
			inst.Load(instData, this._isConditionList);
			return inst;
		}

		// Token: 0x06004C88 RID: 19592 RVA: 0x0024268C File Offset: 0x0024088C
		public void CreateInstruction(int funcId)
		{
			this.DeselectAllInstructions();
			bool flag = this._operatingInstGroup != null;
			if (!flag)
			{
				EventFunctionItem funcCfg = EventFunction.Instance[funcId];
				this._operatingInstGroup = new EventEditorScript.OperatingInstructionGroup(funcId);
				this.InitializeDisabledInstructions((int i) => this.CanPlaceAtIndex(funcCfg, i + 1));
				List<int> requiredPreviousCommands = funcCfg.RequiredPreviousCommands;
				bool flag2 = requiredPreviousCommands != null && requiredPreviousCommands.Count > 0;
				if (flag2)
				{
				}
			}
		}

		// Token: 0x06004C89 RID: 19593 RVA: 0x00242710 File Offset: 0x00240910
		public void CancelCreateInstruction()
		{
			bool flag = this._operatingInstGroup == null;
			if (!flag)
			{
				Object.Destroy(this._operatingInstGroup.DraggingObj.gameObject);
				foreach (EventEditorInstruction inst in this._operatingInstGroup.Instructions)
				{
					Object.Destroy(inst.gameObject);
				}
				this._operatingInstGroup = null;
				this.UpdateLineIndexes();
				this.CheckInstructionsLogic();
			}
		}

		// Token: 0x06004C8A RID: 19594 RVA: 0x00242784 File Offset: 0x00240984
		public void PlaceInstruction()
		{
			bool flag = this._operatingInstGroup == null;
			if (!flag)
			{
				EventEditorScript.OperatingInstructionGroup newInst = this._operatingInstGroup;
				int targetIndex = newInst.Instructions[0].transform.GetSiblingIndex();
				bool flag2 = !this.CanPlaceAtIndex(newInst.Instructions[0].Data.FunctionConfig, targetIndex);
				if (!flag2)
				{
					foreach (EventEditorInstruction inst in newInst.Instructions)
					{
						inst.GetComponent<CanvasGroup>().alpha = 1f;
					}
					Object.Destroy(this._operatingInstGroup.DraggingObj.gameObject);
					this._operatingInstGroup = null;
					OperateCommand operateCmd = new OperateCommand("PlaceInstruction")
					{
						Do = delegate()
						{
							bool isDisposed = newInst.IsDisposed;
							if (isDisposed)
							{
								for (int j = 0; j < newInst.InstructionDatas.Length; j++)
								{
									EventEditorInstruction instruction = this.LoadInstruction(newInst.InstructionDatas[j]);
									this.OfflineAddInstruction(targetIndex + j, instruction);
								}
							}
							else
							{
								for (int k = 0; k < newInst.Instructions.Length; k++)
								{
									this.OfflineAddInstruction(targetIndex + k, newInst.Instructions[k]);
								}
							}
							LayoutRebuilder.ForceRebuildLayoutImmediate(this.instructionScroll.Content);
						},
						Undo = delegate()
						{
							for (int j = newInst.Instructions.Length - 1; j >= 0; j--)
							{
								this.OfflineRemoveInstruction(targetIndex + j);
							}
							newInst.IsDisposed = true;
							LayoutRebuilder.ForceRebuildLayoutImmediate(this.instructionScroll.Content);
						}
					};
					this.ExecuteOperateCommand(operateCmd);
				}
			}
		}

		// Token: 0x06004C8B RID: 19595 RVA: 0x00242898 File Offset: 0x00240A98
		public int FindInstructionIndex(EventEditorInstruction instruction)
		{
			return this._instructions.IndexOf(instruction);
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x002428B8 File Offset: 0x00240AB8
		public int FindInstructionIndex(Vector3 pos)
		{
			int count = this._scriptEditorData.Instructions.Count;
			for (int i = 0; i < count; i++)
			{
				Transform child = this.instructionScroll.Content.GetChild(i);
				bool flag = child.position.y < pos.y;
				if (flag)
				{
					return i;
				}
			}
			return count;
		}

		// Token: 0x06004C8D RID: 19597 RVA: 0x00242920 File Offset: 0x00240B20
		public bool CanPlaceAtIndex(EventFunctionItem functionCfg, int index)
		{
			List<int> requiredPreviousCommands = functionCfg.RequiredPreviousCommands;
			bool flag = requiredPreviousCommands == null || requiredPreviousCommands.Count <= 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = index > 0;
				if (flag2)
				{
					EventInstructionEditorData prevInst = this._scriptEditorData.Instructions[index - 1];
					bool flag3 = functionCfg.RequiredPreviousCommands.Contains(prevInst.FuncId);
					if (flag3)
					{
						return true;
					}
				}
				bool flag4 = index < this._scriptEditorData.Instructions.Count - 1;
				if (flag4)
				{
					EventInstructionEditorData nextInst = this._scriptEditorData.Instructions[index];
					EventFunctionItem nextInstFuncConfig = nextInst.FunctionConfig;
					requiredPreviousCommands = nextInstFuncConfig.RequiredPreviousCommands;
					bool flag5 = requiredPreviousCommands == null || requiredPreviousCommands.Count <= 0;
					if (flag5)
					{
						return false;
					}
					bool flag6 = !nextInstFuncConfig.RequiredPreviousCommands.Contains(functionCfg.TemplateId);
					if (flag6)
					{
						return false;
					}
					for (int i = index - 1; i >= 0; i--)
					{
						EventInstructionEditorData prevInst2 = this._scriptEditorData.Instructions[i];
						bool flag7 = prevInst2.Indent > nextInst.Indent;
						if (!flag7)
						{
							bool flag8 = prevInst2.Indent < nextInst.Indent;
							return !flag8 && functionCfg.RequiredPreviousCommands.Contains(prevInst2.FuncId);
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06004C8E RID: 19598 RVA: 0x00242A90 File Offset: 0x00240C90
		private void UpdateDraggingInstructionPosition()
		{
			Vector3 pos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
			this._operatingInstGroup.DraggingObj.transform.position = new Vector3(pos.x, pos.y);
			this._operatingInstGroup.UpdateIndex();
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x00242AE8 File Offset: 0x00240CE8
		public void Update()
		{
			bool flag = this._operatingInstGroup != null;
			if (flag)
			{
				this.UpdateDraggingInstructionPosition();
				bool mouseButtonDown = Input.GetMouseButtonDown(0);
				if (mouseButtonDown)
				{
					this.PlaceInstruction();
				}
				else
				{
					bool mouseButtonDown2 = Input.GetMouseButtonDown(1);
					if (mouseButtonDown2)
					{
						this.CancelCreateInstruction();
					}
				}
			}
			bool flag2 = EventEditorCommandKit.UndoCommand.Check(UIElement.EventEditor, false, false, false, true, false);
			if (flag2)
			{
				this.Undo();
			}
			bool flag3 = EventEditorCommandKit.RedoCommand.Check(UIElement.EventEditor, false, false, false, true, false);
			if (flag3)
			{
				this.Redo();
			}
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x00242B70 File Offset: 0x00240D70
		public void ExecuteOperateCommand(OperateCommand cmd)
		{
			bool flag = cmd != null;
			if (flag)
			{
				this.OperateStack.Execute(cmd, true);
				this.SetDirty();
			}
		}

		// Token: 0x06004C91 RID: 19601 RVA: 0x00242B9D File Offset: 0x00240D9D
		public void Undo()
		{
			this.DeselectAllInstructions();
			this.OperateStack.Undo();
			this.SetDirty();
		}

		// Token: 0x06004C92 RID: 19602 RVA: 0x00242BBA File Offset: 0x00240DBA
		public void Redo()
		{
			this.DeselectAllInstructions();
			this.OperateStack.Redo();
			this.SetDirty();
		}

		// Token: 0x06004C93 RID: 19603 RVA: 0x00242BD7 File Offset: 0x00240DD7
		public void SetDirty()
		{
			this._isDirty = true;
			this.buttonConfirm.interactable = true;
			this.UpdateLineIndexes();
			this.CheckInstructionsLogic();
			this.UpdateMenuButtons();
		}

		// Token: 0x06004C94 RID: 19604 RVA: 0x00242C04 File Offset: 0x00240E04
		private void UpdateLineIndexes()
		{
			for (int index = 0; index < this._instructions.Count; index++)
			{
				EventEditorInstruction inst = this._instructions[index];
				inst.UpdateIndex(index);
			}
		}

		// Token: 0x06004C95 RID: 19605 RVA: 0x00242C44 File Offset: 0x00240E44
		public bool IsExpressionValid(EventArgumentItem argConfig, string expression)
		{
			try
			{
				this._interpreter.BuildExpression(expression);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06004C96 RID: 19606 RVA: 0x00242C80 File Offset: 0x00240E80
		public string ConvertValueToExpression(EventArgumentItem argConfig, string value)
		{
			bool flag = value.IsNullOrEmpty();
			string result;
			if (flag)
			{
				result = value;
			}
			else
			{
				int valueTemplateId;
				bool flag2;
				if (EventValue.Instance.RefNameMap.TryGetValue(value, out valueTemplateId))
				{
					EventValueItem eventValueItem = EventValue.Instance[valueTemplateId];
					int? num = (eventValueItem != null) ? new int?(eventValueItem.EventArgument) : null;
					int num2 = argConfig.TemplateId;
					flag2 = (num.GetValueOrDefault() == num2 & num != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					EventValueItem valueCfg = EventValue.Instance[valueTemplateId];
					switch (valueCfg.Type)
					{
					case EEventValueType.Global:
						return valueCfg.Alias;
					case EEventValueType.Event:
						return valueCfg.ArgBoxKey.IsNullOrEmpty() ? valueCfg.Alias : valueCfg.ArgBoxKey;
					case EEventValueType.Constant:
					{
						int num2 = valueCfg.EventArgument;
						return (num2 == 5 || num2 == 4) ? EventEditorScript.WrapText(valueCfg.ConstValue) : valueCfg.ConstValue;
					}
					}
				}
				bool flag4 = argConfig.TemplateId == 91;
				if (flag4)
				{
					result = EventEditorScript.WrapText(value);
				}
				else
				{
					bool flag5 = argConfig.TemplateId == 5;
					if (flag5)
					{
						this.InitEventNameCache();
						result = (this._eventNameToGuid.GetValueOrDefault(value, value) ?? value);
					}
					else
					{
						bool flag6 = argConfig.TemplateId == 22;
						if (flag6)
						{
							result = this._model.GetItemType(value).ToString();
						}
						else
						{
							bool flag7 = argConfig.TemplateId == 8;
							if (flag7)
							{
								result = string.Empty;
							}
							else
							{
								bool flag8 = argConfig.TemplateId == 78;
								if (flag8)
								{
									result = value;
								}
								else
								{
									bool flag9 = argConfig.TemplateId == 33;
									if (flag9)
									{
										result = this._model.GetWorldFunctionType(value).ToString();
									}
									else
									{
										bool flag10 = !string.IsNullOrEmpty(argConfig.ConfigTable);
										if (flag10)
										{
											IConfigData configTable = ConfigCollection.NameMap[argConfig.ConfigTable];
											int templateId;
											bool flag11 = !configTable.RefNameMap.TryGetValue(value, out templateId);
											if (flag11)
											{
												result = string.Empty;
											}
											else
											{
												IEventArgumentCollectionFormatter formatter = configTable as IEventArgumentCollectionFormatter;
												bool flag12 = formatter != null;
												if (flag12)
												{
													result = EventEditorScript.WrapText(formatter.ToArgString(templateId));
												}
												else
												{
													result = templateId.ToString();
												}
											}
										}
										else
										{
											string[] customEnumText = argConfig.CustomEnumText;
											bool flag13 = customEnumText != null && customEnumText.Length > 0;
											if (flag13)
											{
												int index = argConfig.CustomEnumText.IndexOf(value);
												int[] customEnumValues = argConfig.CustomEnumValues;
												bool flag14 = customEnumValues != null && customEnumValues.Length > 0;
												if (flag14)
												{
													bool flag15 = !argConfig.CustomEnumValues.CheckIndex(index);
													if (flag15)
													{
														result = string.Empty;
													}
													else
													{
														result = argConfig.CustomEnumValues[index].ToString();
													}
												}
												else
												{
													result = index.ToString();
												}
											}
											else
											{
												result = value;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004C97 RID: 19607 RVA: 0x00242F74 File Offset: 0x00241174
		public string ConvertValueFromExpression(EventArgumentItem argConfig, string expression)
		{
			bool flag = string.IsNullOrEmpty(expression);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				foreach (KeyValuePair<string, int> keyValuePair in EventValue.Instance.RefNameMap)
				{
					string text;
					int eventArgument;
					keyValuePair.Deconstruct(out text, out eventArgument);
					string refName = text;
					int valueTemplateId = eventArgument;
					bool flag2 = valueTemplateId < 0;
					if (!flag2)
					{
						EventValueItem valueCfg = EventValue.Instance[valueTemplateId];
						bool flag3 = valueCfg.EventArgument != argConfig.TemplateId;
						if (!flag3)
						{
							bool flag4 = valueCfg.Type == EEventValueType.Global && valueCfg.Alias == expression;
							if (flag4)
							{
								return refName;
							}
							bool flag5 = (valueCfg.Type == EEventValueType.Event && valueCfg.ArgBoxKey == expression) || valueCfg.Alias == expression;
							if (flag5)
							{
								return refName;
							}
							bool flag6 = valueCfg.Type == EEventValueType.Constant;
							if (flag6)
							{
								eventArgument = valueCfg.EventArgument;
								bool flag7 = eventArgument == 5 || eventArgument == 4;
								if (flag7)
								{
									bool flag8 = EventEditorScript.WrapText(valueCfg.ConstValue) == expression;
									if (flag8)
									{
										return refName;
									}
								}
								else
								{
									bool flag9 = valueCfg.ConstValue == expression;
									if (flag9)
									{
										return refName;
									}
								}
							}
						}
					}
				}
				bool flag10 = argConfig.TemplateId == 91;
				if (flag10)
				{
					string unwrapped = EventEditorScript.UnwrapText(expression);
					LanguageKey languageKey;
					bool flag11 = Enum.TryParse<LanguageKey>(unwrapped, out languageKey);
					if (flag11)
					{
						result = unwrapped;
					}
					else
					{
						bool flag12 = Enum.TryParse<LanguageKey>(expression, out languageKey);
						if (flag12)
						{
							result = expression;
						}
						else
						{
							result = string.Empty;
						}
					}
				}
				else
				{
					bool flag13 = argConfig.TemplateId == 5;
					if (flag13)
					{
						this.InitEventNameCache();
						result = this._eventGuidToName.GetValueOrDefault(expression, expression);
					}
					else
					{
						bool flag14 = argConfig.TemplateId == 89;
						if (flag14)
						{
							SectMainStoryEventArgKeyItem keyCfg2 = SectMainStoryEventArgKey.Instance.FirstOrDefault((SectMainStoryEventArgKeyItem keyCfg) => keyCfg.ArgBoxKey == EventEditorScript.UnwrapText(expression));
							result = ((keyCfg2 != null) ? SectMainStoryEventArgKey.Instance.GetRefName(keyCfg2.TemplateId) : expression);
						}
						else
						{
							bool flag15 = argConfig.TemplateId == 8;
							if (flag15)
							{
								result = string.Empty;
							}
							else
							{
								bool flag16 = argConfig.TemplateId == 78;
								if (flag16)
								{
									result = expression;
								}
								else
								{
									int id;
									bool flag17 = int.TryParse(expression, out id);
									if (flag17)
									{
										bool flag18 = !string.IsNullOrEmpty(argConfig.ConfigTable);
										if (flag18)
										{
											return this._model.GetConfigRefKey(ConfigCollection.NameMap[argConfig.ConfigTable], id);
										}
										bool flag19 = argConfig.TemplateId == 22;
										if (flag19)
										{
											return this._model.GetItemTypeName((sbyte)id);
										}
										bool flag20 = argConfig.TemplateId == 33;
										if (flag20)
										{
											return this._model.GetWorldFunctionTypeName((byte)id);
										}
										string[] customEnumText = argConfig.CustomEnumText;
										bool flag21 = customEnumText != null && customEnumText.Length > 0;
										if (flag21)
										{
											int index = id;
											int[] customEnumValues = argConfig.CustomEnumValues;
											bool flag22 = customEnumValues != null && customEnumValues.Length > 0;
											if (flag22)
											{
												index = argConfig.CustomEnumValues.IndexOf(id);
											}
											return argConfig.CustomEnumText[index];
										}
									}
									result = expression;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004C98 RID: 19608 RVA: 0x00243344 File Offset: 0x00241544
		public static string WrapText(string text)
		{
			return (text == null) ? "\"\"" : ("\"" + text + "\"");
		}

		// Token: 0x06004C99 RID: 19609 RVA: 0x00243370 File Offset: 0x00241570
		public static string UnwrapText(string text)
		{
			bool flag = text[0] == '"' && text[text.Length - 1] == '"';
			string result;
			if (flag)
			{
				result = text.Substring(1, text.Length - 2);
			}
			else
			{
				result = text;
			}
			return result;
		}

		// Token: 0x06004C9A RID: 19610 RVA: 0x002433BC File Offset: 0x002415BC
		private List<string> InitConfigRefNameCache(IConfigData configData)
		{
			List<string> refNameList;
			bool flag = !this._configRefNameListCache.TryGetValue(configData, out refNameList);
			if (flag)
			{
				IReadOnlyDictionary<string, int> refNameMap = configData.RefNameMap;
				refNameList = new List<string>();
				refNameList.AddRange(refNameMap.Keys);
				refNameList.Sort((string a, string b) => refNameMap[a].CompareTo(refNameMap[b]));
				this._configRefNameListCache.Add(configData, refNameList);
			}
			return refNameList;
		}

		// Token: 0x06004C9B RID: 19611 RVA: 0x00243434 File Offset: 0x00241634
		public void ShowConfigInputHint(string currVal, TMP_InputField inputField, IConfigData configData, int argTemplateId)
		{
			EventEditorScript.<>c__DisplayClass111_0 CS$<>8__locals1 = new EventEditorScript.<>c__DisplayClass111_0();
			CS$<>8__locals1.configData = configData;
			List<string> refNameList = this.InitConfigRefNameCache(CS$<>8__locals1.configData);
			if (argTemplateId <= 74)
			{
				if (argTemplateId == 45)
				{
					refNameList = refNameList.FindAll(delegate(string refName)
					{
						CharacterItem characterItem = Character.Instance[refName];
						byte? b = (characterItem != null) ? new byte?(characterItem.CreatingType) : null;
						if (b != null)
						{
							byte valueOrDefault = b.GetValueOrDefault();
							if (valueOrDefault - 2 <= 1)
							{
								return true;
							}
						}
						return false;
					});
					goto IL_1BB;
				}
				if (argTemplateId == 46)
				{
					refNameList = refNameList.FindAll(delegate(string refName)
					{
						CharacterItem characterItem = Character.Instance[refName];
						return ((characterItem != null) ? new byte?(characterItem.CreatingType) : null) == 0;
					});
					goto IL_1BB;
				}
				if (argTemplateId == 74)
				{
					refNameList = refNameList.FindAll(delegate(string refName)
					{
						CharacterItem characterItem = Character.Instance[refName];
						return ((characterItem != null) ? new byte?(characterItem.CreatingType) : null) == 2;
					});
					goto IL_1BB;
				}
			}
			else
			{
				if (argTemplateId == 80)
				{
					refNameList = refNameList.FindAll(delegate(string refName)
					{
						InstantNotificationItem instantNotificationItem = InstantNotification.Instance[refName];
						return ((instantNotificationItem != null) ? new bool?(instantNotificationItem.AllowByEventFunction) : null) ?? false;
					});
					goto IL_1BB;
				}
				if (argTemplateId == 93)
				{
					refNameList = refNameList.FindAll(delegate(string refName)
					{
						MonthlyEventItem monthlyEventItem = MonthlyEvent.Instance[refName];
						return ((monthlyEventItem != null) ? new bool?(monthlyEventItem.AllowByEventFunction) : null) ?? false;
					});
					goto IL_1BB;
				}
				if (argTemplateId == 94)
				{
					refNameList = refNameList.FindAll(delegate(string refName)
					{
						MonthlyNotificationItem monthlyNotificationItem = MonthlyNotification.Instance[refName];
						return ((monthlyNotificationItem != null) ? new bool?(monthlyNotificationItem.AllowByEventFunction) : null) ?? false;
					});
					goto IL_1BB;
				}
			}
			EventArgumentItem argCfg = EventArgument.Instance[argTemplateId];
			bool flag = argCfg.EnumRange.First < argCfg.EnumRange.Second;
			if (flag)
			{
				refNameList = refNameList.FindAll(delegate(string refName)
				{
					int templateId = CS$<>8__locals1.configData.RefNameMap[refName];
					return templateId >= argCfg.EnumRange.First && templateId <= argCfg.EnumRange.Second;
				});
			}
			IL_1BB:
			UI_SearchResultShow.ShowInputHint(currVal, inputField, refNameList, null);
		}

		// Token: 0x06004C9C RID: 19612 RVA: 0x00243608 File Offset: 0x00241808
		private void InitEventNameCache()
		{
			bool flag = this._eventNameToGuid != null;
			if (!flag)
			{
				this._eventNameToGuid = new Dictionary<string, string>();
				this._eventGuidToName = new Dictionary<string, string>();
				EventGroupData editingEventGroup = EventGroupTreeView.Instance.EditingEventGroup;
				foreach (KeyValuePair<string, int> keyValuePair in EventValue.Instance.RefNameMap)
				{
					string text;
					int num;
					keyValuePair.Deconstruct(out text, out num);
					string refName = text;
					int valueTemplateId = num;
					bool flag2 = valueTemplateId < 0;
					if (!flag2)
					{
						EventValueItem valueCfg = EventValue.Instance[valueTemplateId];
						bool flag3 = valueCfg.Type != EEventValueType.Constant || valueCfg.EventArgument != 5;
						if (!flag3)
						{
							string guid = EventEditorScript.WrapText(valueCfg.ConstValue);
							this._eventGuidToName[guid] = refName;
							this._eventNameToGuid[refName] = guid;
						}
					}
				}
				bool flag4 = editingEventGroup == null;
				if (!flag4)
				{
					List<ValueTuple<string, string, string>> allEvents = editingEventGroup.GetDisplayList(false);
					foreach (ValueTuple<string, string, string> item in allEvents)
					{
						string guid2 = EventEditorScript.WrapText(item.Item1);
						string eventName = item.Item2;
						this._eventGuidToName[guid2] = eventName;
						this._eventNameToGuid[eventName] = guid2;
					}
				}
			}
		}

		// Token: 0x06004C9D RID: 19613 RVA: 0x00243798 File Offset: 0x00241998
		public void ShowEventInputHint(string currVal, TMP_InputField inputField)
		{
			this.InitEventNameCache();
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._eventNameToGuid.Keys, null);
		}

		// Token: 0x06004C9E RID: 19614 RVA: 0x002437B6 File Offset: 0x002419B6
		public void ShowAdventureElementInputHint(string currVal, TMP_InputField inputField)
		{
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._model.GetAdventureElementTypeNames(), null);
		}

		// Token: 0x06004C9F RID: 19615 RVA: 0x002437CD File Offset: 0x002419CD
		public void ShowMajorEventIdInputHint(string currVal, TMP_InputField inputField)
		{
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._model.GetMajorEventIdNames(), null);
		}

		// Token: 0x06004CA0 RID: 19616 RVA: 0x002437E4 File Offset: 0x002419E4
		public void ShowAdventureRemakeTemplateInputHint(string currVal, TMP_InputField inputField)
		{
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._model.GetAdventureRemakeTemplateNames(), null);
		}

		// Token: 0x06004CA1 RID: 19617 RVA: 0x002437FB File Offset: 0x002419FB
		public void ShowLanguageKeyInputHint(string currVal, TMP_InputField inputField)
		{
			if (this._languageKeyNames == null)
			{
				this._languageKeyNames = typeof(LanguageKey).GetEnumNames();
			}
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._languageKeyNames, null);
		}

		// Token: 0x06004CA2 RID: 19618 RVA: 0x0024382A File Offset: 0x00241A2A
		public void ShowItemTypeInputHint(string currVal, TMP_InputField inputField)
		{
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._model.GetItemTypeNames(), null);
		}

		// Token: 0x06004CA3 RID: 19619 RVA: 0x00243841 File Offset: 0x00241A41
		public void ShowWorldFunctionTypeInputHint(string currVal, TMP_InputField inputField)
		{
			UI_SearchResultShow.ShowInputHint(currVal, inputField, this._model.GetWorldFunctionTypeNames(), null);
		}

		// Token: 0x06004CA4 RID: 19620 RVA: 0x00243858 File Offset: 0x00241A58
		public void ShowVariableNameInputHint(string currVal, TMP_InputField inputField, int argTemplateId)
		{
			IReadOnlyDictionary<string, int> refNameMap = EventValue.Instance.RefNameMap;
			List<string> optionStrings = EasyPool.Get<List<string>>();
			optionStrings.Clear();
			Dictionary<string, string> optionDescs = EasyPool.Get<Dictionary<string, string>>();
			optionDescs.Clear();
			foreach (KeyValuePair<string, int> keyValuePair in refNameMap)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string refName = text;
				int templateId = num;
				bool flag = templateId < 0;
				if (!flag)
				{
					EventValueItem eventValueCfg = EventValue.Instance[templateId];
					bool flag2 = eventValueCfg.EventArgument != argTemplateId;
					if (!flag2)
					{
						bool flag3 = eventValueCfg.Type > EEventValueType.Global;
						if (flag3)
						{
							optionDescs.Add(refName, this.GetEventValueDesc(eventValueCfg));
							optionStrings.Add(refName);
						}
						else
						{
							bool flag4 = eventValueCfg.Type != EEventValueType.Event;
							if (flag4)
							{
								optionDescs.Add(refName, this.GetEventValueDesc(eventValueCfg));
								optionStrings.Add(refName);
							}
						}
					}
				}
			}
			HashSet<string> idSet;
			bool flag5 = this._currScriptIdentifiers.TryGetValue(argTemplateId, out idSet);
			if (flag5)
			{
				optionStrings.AddRange(idSet);
			}
			bool flag6 = this._currScriptIdentifiers.TryGetValue(0, out idSet);
			if (flag6)
			{
				optionStrings.AddRange(idSet);
			}
			bool flag7 = optionStrings.Count > 0;
			if (flag7)
			{
				UI_SearchResultShow.ShowInputHint(currVal, inputField, optionStrings, optionDescs);
			}
			EasyPool.Free<List<string>>(optionStrings);
		}

		// Token: 0x06004CA5 RID: 19621 RVA: 0x002439CC File Offset: 0x00241BCC
		public void ShowCurrScriptVariableNameInputHint(string currVal, TMP_InputField inputField, int argTemplateId)
		{
			List<string> optionStrings = EasyPool.Get<List<string>>();
			optionStrings.Clear();
			HashSet<string> idSet;
			bool flag = this._currScriptIdentifiers.TryGetValue(argTemplateId, out idSet);
			if (flag)
			{
				optionStrings.AddRange(idSet);
			}
			bool flag2 = argTemplateId != 0 && this._currScriptIdentifiers.TryGetValue(0, out idSet);
			if (flag2)
			{
				optionStrings.AddRange(idSet);
			}
			bool flag3 = optionStrings.Count > 0;
			if (flag3)
			{
				UI_SearchResultShow.ShowInputHint(currVal, inputField, optionStrings, null);
			}
			EasyPool.Free<List<string>>(optionStrings);
		}

		// Token: 0x06004CA6 RID: 19622 RVA: 0x00243A44 File Offset: 0x00241C44
		public List<string> GetCurrScriptAllowableVariableName(int argTemplateId)
		{
			List<string> variableNames = new List<string>();
			HashSet<string> idSet;
			bool flag = this._currScriptIdentifiers.TryGetValue(argTemplateId, out idSet);
			if (flag)
			{
				variableNames.AddRange(idSet);
			}
			bool flag2 = this._currScriptIdentifiers.TryGetValue(0, out idSet);
			if (flag2)
			{
				variableNames.AddRange(idSet);
			}
			return variableNames;
		}

		// Token: 0x06004CA7 RID: 19623 RVA: 0x00243A98 File Offset: 0x00241C98
		private string GetEventValueDesc(EventValueItem eventValueCfg)
		{
			string text = eventValueCfg.Desc;
			bool flag = !string.IsNullOrEmpty(eventValueCfg.ArgBoxKey);
			if (flag)
			{
				bool flag2 = !string.IsNullOrEmpty(text);
				if (flag2)
				{
					text += "\n";
				}
				text += eventValueCfg.ArgBoxKey;
			}
			bool flag3 = !string.IsNullOrEmpty(eventValueCfg.Alias);
			if (flag3)
			{
				bool flag4 = !string.IsNullOrEmpty(text);
				if (flag4)
				{
					text += "\n";
				}
				text += eventValueCfg.ArgBoxKey;
			}
			return text;
		}

		// Token: 0x06004CA8 RID: 19624 RVA: 0x00243B2C File Offset: 0x00241D2C
		public void ShowError(string error, bool animate)
		{
			bool flag = string.IsNullOrEmpty(error);
			if (!flag)
			{
				this.logTrigger.TextObject.DOKill(false);
				this.logTrigger.TextObject.color = Color.red;
				this.logTrigger.StartTypeWriter(error);
				bool flag2 = !animate;
				if (flag2)
				{
					this.logTrigger.StopTypeWriter();
				}
				EventEditorScriptConsole.Instance.AddNote(error);
			}
		}

		// Token: 0x06004CA9 RID: 19625 RVA: 0x00243B9B File Offset: 0x00241D9B
		public void ClearErrorLog()
		{
			this.logTrigger.StopTypeWriter();
			this.logTrigger.TextObject.DOKill(false);
			this.logTrigger.TextObject.text = "";
		}

		// Token: 0x040034F9 RID: 13561
		[SerializeField]
		private GameObject instructionTemplate;

		// Token: 0x040034FA RID: 13562
		[SerializeField]
		private CScrollRect instructionScroll;

		// Token: 0x040034FB RID: 13563
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x040034FC RID: 13564
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x040034FD RID: 13565
		[SerializeField]
		private CScrollRect functionScroll;

		// Token: 0x040034FE RID: 13566
		[SerializeField]
		private GameObject functionTemplate;

		// Token: 0x040034FF RID: 13567
		[SerializeField]
		private CButton undoBtn;

		// Token: 0x04003500 RID: 13568
		[SerializeField]
		private CButton redoBtn;

		// Token: 0x04003501 RID: 13569
		[SerializeField]
		private TMP_InputField functionSearch;

		// Token: 0x04003502 RID: 13570
		[SerializeField]
		private CToggle showAllToggle;

		// Token: 0x04003503 RID: 13571
		[SerializeField]
		private CToggleGroup functionToggleGroup;

		// Token: 0x04003504 RID: 13572
		[SerializeField]
		private UITypeWriterEffect logTrigger;

		// Token: 0x04003505 RID: 13573
		[SerializeField]
		private EventEditorScriptConsole scriptConsole;

		// Token: 0x04003506 RID: 13574
		[SerializeField]
		private CButton showLogTrigger;

		// Token: 0x04003507 RID: 13575
		[SerializeField]
		private TextMeshProUGUI scriptLabel;

		// Token: 0x04003508 RID: 13576
		[SerializeField]
		private CButton selectAllBtn;

		// Token: 0x04003509 RID: 13577
		[SerializeField]
		private CButton copyBtn;

		// Token: 0x0400350A RID: 13578
		[SerializeField]
		private CButton pasteBtn;

		// Token: 0x0400350B RID: 13579
		[SerializeField]
		private CToggle monitorToggle;

		// Token: 0x0400350C RID: 13580
		public static EventEditorScript Instance;

		// Token: 0x0400350D RID: 13581
		private EventScriptEditorData _scriptEditorData;

		// Token: 0x0400350E RID: 13582
		private bool _isConditionList;

		// Token: 0x0400350F RID: 13583
		private bool _isDirty;

		// Token: 0x04003510 RID: 13584
		private EventScriptId _scriptId = EventScriptId.Invalid;

		// Token: 0x04003511 RID: 13585
		private List<EventEditorInstruction> _instructions;

		// Token: 0x04003512 RID: 13586
		private Dictionary<int, GameObject> _functions;

		// Token: 0x04003513 RID: 13587
		public readonly HashSet<int> DisabledInstructionIndices = new HashSet<int>();

		// Token: 0x04003514 RID: 13588
		private List<int> _selectedInstructions = new List<int>();

		// Token: 0x04003515 RID: 13589
		private List<int> _selectingInstructions = new List<int>();

		// Token: 0x04003516 RID: 13590
		private List<EventInstructionEditorData> _copiedInstructions = new List<EventInstructionEditorData>();

		// Token: 0x04003517 RID: 13591
		private Dictionary<string, string> _eventNameToGuid;

		// Token: 0x04003518 RID: 13592
		private Dictionary<string, string> _eventGuidToName;

		// Token: 0x04003519 RID: 13593
		private string[] _languageKeyNames;

		// Token: 0x0400351A RID: 13594
		private HashSet<int> _implementedFunctions;

		// Token: 0x0400351B RID: 13595
		private int _listenerId;

		// Token: 0x0400351C RID: 13596
		private Interpreter<BasicContext> _interpreter;

		// Token: 0x0400351D RID: 13597
		private Action<EventScriptEditorData> _onConfirm;

		// Token: 0x0400351E RID: 13598
		public bool IsGroupSelect;

		// Token: 0x0400351F RID: 13599
		private Dictionary<int, HashSet<string>> _currScriptIdentifiers;

		// Token: 0x04003520 RID: 13600
		private EventEditorModel _model;

		// Token: 0x04003521 RID: 13601
		public readonly StringBuilder StringBuilder = new StringBuilder();

		// Token: 0x04003522 RID: 13602
		private EventEditorScript.OperatingInstructionGroup _operatingInstGroup;

		// Token: 0x04003523 RID: 13603
		private readonly Dictionary<IConfigData, List<string>> _configRefNameListCache = new Dictionary<IConfigData, List<string>>();

		// Token: 0x02001A70 RID: 6768
		private class OperatingInstructionGroup
		{
			// Token: 0x0600DE2E RID: 56878 RVA: 0x005D4E38 File Offset: 0x005D3038
			public OperatingInstructionGroup(int funcId)
			{
				EventFunctionItem funcCfg = EventFunction.Instance[funcId];
				this.Instructions = new EventEditorInstruction[(funcCfg.FollowUp >= 0) ? 2 : 1];
				this.InstructionDatas = new EventInstructionEditorData[this.Instructions.Length];
				EventInstructionEditorData data = new EventInstructionEditorData(funcId);
				this.Instructions[0] = EventEditorScript.Instance.LoadInstruction(data);
				this.Instructions[0].GetComponent<CanvasGroup>().alpha = 0.4f;
				this.InstructionDatas[0] = data;
				bool flag = funcCfg.FollowUp >= 0;
				if (flag)
				{
					EventInstructionEditorData endInstData = new EventInstructionEditorData(funcCfg.FollowUp);
					this.Instructions[1] = EventEditorScript.Instance.LoadInstruction(endInstData);
					this.Instructions[1].GetComponent<CanvasGroup>().alpha = 0.4f;
					this.InstructionDatas[1] = endInstData;
				}
				this.DraggingObj = Object.Instantiate<GameObject>(EventEditorScript.Instance.functionTemplate, EventEditorScript.Instance.transform).GetComponent<EventEditorScriptFunctionTemplate>();
				this.DraggingObj.funcName.SetText(funcCfg.Name, true);
				this.DraggingObj.GetComponent<CButton>().interactable = false;
			}

			// Token: 0x0600DE2F RID: 56879 RVA: 0x005D4F60 File Offset: 0x005D3160
			public void UpdateIndex()
			{
				int index = EventEditorScript.Instance.FindInstructionIndex(this.DraggingObj.transform.position);
				EventEditorInstruction mainInstView = this.Instructions[0];
				int currIndex = mainInstView.transform.GetSiblingIndex();
				bool flag = index == currIndex;
				if (!flag)
				{
					EventEditorInstruction instView = this.Instructions[0];
					EventInstructionEditorData prevInst = (index > 0) ? EventEditorScript.Instance._scriptEditorData.Instructions[index - 1] : null;
					instView.Data.AdjustIndention(prevInst);
					instView.SetIndent(instView.Data.Indent);
					instView.transform.SetSiblingIndex(index);
					for (int i = 1; i < this.Instructions.Length; i++)
					{
						instView = this.Instructions[i];
						instView.Data.AdjustIndention(this.Instructions[i - 1].Data);
						instView.SetIndent(instView.Data.Indent);
						instView.transform.SetSiblingIndex(index + i);
					}
				}
			}

			// Token: 0x0400B629 RID: 46633
			public readonly EventEditorScriptFunctionTemplate DraggingObj;

			// Token: 0x0400B62A RID: 46634
			public readonly EventEditorInstruction[] Instructions;

			// Token: 0x0400B62B RID: 46635
			public readonly EventInstructionEditorData[] InstructionDatas;

			// Token: 0x0400B62C RID: 46636
			public bool IsDisposed;
		}
	}
}
