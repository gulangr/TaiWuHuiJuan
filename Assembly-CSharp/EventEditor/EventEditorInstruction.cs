using System;
using System.Collections.Generic;
using System.Text;
using AdventureEditor.Beta;
using Config;
using Config.Common;
using EventEditor.EventScript;
using EventEditor.GlobalScript;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.EventEditor.UIScripts.Migrate;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EventEditor
{
	// Token: 0x02000645 RID: 1605
	public class EventEditorInstruction : MonoBehaviour
	{
		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x06004C1F RID: 19487 RVA: 0x0023E4B2 File Offset: 0x0023C6B2
		public EventInstructionEditorData Data
		{
			get
			{
				return this._data;
			}
		}

		// Token: 0x06004C20 RID: 19488 RVA: 0x0023E4BC File Offset: 0x0023C6BC
		private void Start()
		{
			PointClickBridge pointClickBridge = this._backgroundImg.GetComponent<PointClickBridge>();
			pointClickBridge.OnRightClick = new Action(this.ActivateButtonSheet);
			pointClickBridge.OnLeftClick = new Action(this.SelectInstruction);
			this.Selected.SetActive(false);
		}

		// Token: 0x06004C21 RID: 19489 RVA: 0x0023E507 File Offset: 0x0023C707
		public void SelectInstruction()
		{
			EventEditorScript.Instance.SelectInstruction(this);
		}

		// Token: 0x06004C22 RID: 19490 RVA: 0x0023E518 File Offset: 0x0023C718
		public void ActivateButtonSheet()
		{
			EventEditorScript scriptEditor = EventEditorScript.Instance;
			int index = base.transform.GetSiblingIndex();
			bool flag = !scriptEditor.IsInstructionSelected(index);
			if (flag)
			{
				scriptEditor.DeselectAllInstructions();
				scriptEditor.SelectInstruction(this);
			}
			bool flag2 = scriptEditor.SelectedCount == 0;
			if (!flag2)
			{
				List<SheetButtonInfo> sheetInfos = new List<SheetButtonInfo>
				{
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_Delete), new Action(this.ActionDelete), this.ConditionCanOperate(), ""),
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_Copy), new Action(this.ActionCopy), this.ConditionCanOperate(), ""),
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_Cut), new Action(this.ActionCut), this.ConditionCanOperate(), ""),
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_PasteAbove), new Action(this.ActionPasteAbove), this.ConditionCanPasteAbove(), ""),
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_PasteBelow), new Action(this.ActionPasteBelow), this.ConditionCanPasteBelow(), "")
				};
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", sheetInfos);
				UIElement.ButtonSheet.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
			}
		}

		// Token: 0x06004C23 RID: 19491 RVA: 0x0023E682 File Offset: 0x0023C882
		private void ActionDelete()
		{
			EventEditorScript.Instance.DeleteSelectedInstructions();
		}

		// Token: 0x06004C24 RID: 19492 RVA: 0x0023E690 File Offset: 0x0023C890
		private void ActionCopy()
		{
			EventEditorScript.Instance.CopySelectedInstructions();
		}

		// Token: 0x06004C25 RID: 19493 RVA: 0x0023E69E File Offset: 0x0023C89E
		private void ActionCut()
		{
			EventEditorScript.Instance.CutSelectedInstructions();
		}

		// Token: 0x06004C26 RID: 19494 RVA: 0x0023E6AC File Offset: 0x0023C8AC
		private void ActionPasteAbove()
		{
			int index = base.transform.GetSiblingIndex();
			EventEditorScript.Instance.PasteCopiedInstructions(index);
		}

		// Token: 0x06004C27 RID: 19495 RVA: 0x0023E6D4 File Offset: 0x0023C8D4
		private void ActionPasteBelow()
		{
			int index = base.transform.GetSiblingIndex();
			EventEditorScript.Instance.PasteCopiedInstructions(index + 1);
		}

		// Token: 0x06004C28 RID: 19496 RVA: 0x0023E6FC File Offset: 0x0023C8FC
		private bool ConditionCanOperate()
		{
			return EventEditorScript.Instance.SelectedCount > 1 || (EventEditorScript.Instance.SelectedCount == 1 && EventEditorScript.Instance.IsGroupSelect) || this.Data.CanOperateAlone;
		}

		// Token: 0x06004C29 RID: 19497 RVA: 0x0023E744 File Offset: 0x0023C944
		private bool ConditionCanPasteAbove()
		{
			int index = base.transform.GetSiblingIndex();
			return EventEditorScript.Instance.CanPasteCopiedInstructionsAt(index);
		}

		// Token: 0x06004C2A RID: 19498 RVA: 0x0023E770 File Offset: 0x0023C970
		private bool ConditionCanPasteBelow()
		{
			int index = base.transform.GetSiblingIndex();
			return EventEditorScript.Instance.CanPasteCopiedInstructionsAt(index + 1);
		}

		// Token: 0x06004C2B RID: 19499 RVA: 0x0023E79B File Offset: 0x0023C99B
		public void UpdateIndex(int index)
		{
			this._lineIndexLabel.text = index.ToString();
		}

		// Token: 0x06004C2C RID: 19500 RVA: 0x0023E7B4 File Offset: 0x0023C9B4
		public void Load(EventInstructionEditorData data, bool loadAsCondition)
		{
			this._data = data;
			if (this._arguments == null)
			{
				this._arguments = new List<EventEditorScriptInputArgTemplate>();
			}
			this._arguments.Clear();
			EventFunctionItem funcCfg = EventFunction.Instance[data.FuncId];
			this._funcNameLabel.text = funcCfg.Name;
			TooltipInvoker mouseTipDisplayer = this._funcNameLabel.GetComponent<TooltipInvoker>();
			bool flag = !mouseTipDisplayer.PresetParam.CheckIndex(0);
			if (flag)
			{
				mouseTipDisplayer.PresetParam = new string[1];
			}
			mouseTipDisplayer.PresetParam[0] = funcCfg.Desc;
			CImage functionBack = this._funcNameLabel.transform.parent.GetComponent<CImage>();
			functionBack.color = EventEditorScript.Instance.GetFunctionColor(funcCfg);
			this.SetIndent(data.Indent);
			bool flag2 = funcCfg.ReturnValue >= 0 && !loadAsCondition;
			if (flag2)
			{
				this._retValInputField.gameObject.SetActive(true);
				this._retValInputField.SetTextWithoutNotify(data.AssignToVar);
				this._retValInputField.onEndEdit.RemoveAllListeners();
				this._retValInputField.onEndEdit.AddListener(new UnityAction<string>(this.EditRetVariable));
				UnityAction<string> showHint = delegate(string input)
				{
					EventEditorScript.Instance.ShowCurrScriptVariableNameInputHint(input, this._retValInputField, funcCfg.ReturnValue);
				};
				this._retValInputField.onSelect.RemoveAllListeners();
				this._retValInputField.onSelect.AddListener(showHint);
				this._retValInputField.onValueChanged.RemoveAllListeners();
				this._retValInputField.onValueChanged.AddListener(showHint);
				TextMeshProUGUI placeHolder = (TextMeshProUGUI)this._retValInputField.placeholder;
				placeHolder.text = "变量名";
				this._assignLabel.SetActive(true);
			}
			else
			{
				this._retValInputField.gameObject.SetActive(false);
				this._assignLabel.SetActive(false);
			}
			bool flag3 = funcCfg.ParameterTypes != null;
			if (flag3)
			{
				bool flag4 = data.Args.Length != funcCfg.ParameterTypes.Length;
				if (flag4)
				{
					Debug.LogError(string.Format("{0} argument count mismatch: {1} defined, {2} given.", data.FunctionName, funcCfg.ParameterTypes.Length, data.Args.Length));
					EventArgumentEditorData[] newArgs = new EventArgumentEditorData[funcCfg.ParameterTypes.Length];
					int length = Math.Min(data.Args.Length, funcCfg.ParameterTypes.Length);
					Array.Copy(data.Args, newArgs, length);
					for (int i = data.Args.Length; i < newArgs.Length; i++)
					{
						newArgs[i] = new EventArgumentEditorData(EventArgument.Instance[funcCfg.ParameterTypes[i]]);
					}
					data.Args = newArgs;
				}
				for (int j = 0; j < funcCfg.ParameterTypes.Length; j++)
				{
					int argTemplateId = funcCfg.ParameterTypes[j];
					EventArgumentItem argCfg = EventArgument.Instance[argTemplateId];
					EventArgumentEditorData argData = data.Args[j];
					string argName = funcCfg.ParameterNames.CheckIndex(j) ? funcCfg.ParameterNames[j] : string.Empty;
					this.InitArgument(j, argName, argCfg, argData);
				}
			}
			if (loadAsCondition)
			{
				this._isReverseToggle.gameObject.SetActive(true);
				this._isReverseToggle.onValueChanged.RemoveAllListeners();
				this._isReverseToggle.isOn = data.Reverse;
				this._isReverseToggle.onValueChanged.AddListener(new UnityAction<bool>(this.EditReverse));
			}
			else
			{
				this._isReverseToggle.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004C2D RID: 19501 RVA: 0x0023EBA8 File Offset: 0x0023CDA8
		private void InitArgument(int index, string argName, EventArgumentItem argCfg, EventArgumentEditorData argData)
		{
			EventEditorInstruction.<>c__DisplayClass27_0 CS$<>8__locals1 = new EventEditorInstruction.<>c__DisplayClass27_0();
			CS$<>8__locals1.index = index;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.argData = argData;
			CS$<>8__locals1.argCfg = argCfg;
			GameObject argObj = Object.Instantiate<GameObject>(this._textInputArgTemplate.gameObject, this._elementRoot.transform);
			EventEditorScriptInputArgTemplate argRefers = argObj.GetComponent<EventEditorScriptInputArgTemplate>();
			CToggle argToggle = argRefers.boolInput;
			TMP_InputField argInputField = argRefers.textInput;
			CButton argButton = argRefers.buttonInput;
			CToggle isExpressionToggle = argRefers.isExpression;
			TooltipInvoker mouseTipDisplayer = argObj.GetComponent<TooltipInvoker>();
			string hintText = (!string.IsNullOrEmpty(argName)) ? (argName + "(" + CS$<>8__locals1.argCfg.Name + ")") : CS$<>8__locals1.argCfg.Name;
			bool flag = !mouseTipDisplayer.PresetParam.CheckIndex(0);
			if (flag)
			{
				mouseTipDisplayer.PresetParam = new string[1];
			}
			mouseTipDisplayer.PresetParam[0] = hintText;
			this._arguments.Add(argRefers);
			bool flag2 = CS$<>8__locals1.argCfg.TemplateId == 48 && !CS$<>8__locals1.argData.IsExpression && !string.IsNullOrEmpty(CS$<>8__locals1.argData.Value);
			if (flag2)
			{
				string scriptGuid = CS$<>8__locals1.argData.Value.Replace("\\", string.Empty).Replace("\"", string.Empty);
				GlobalScriptEditorData scriptData;
				bool flag3 = EventEditorGlobalScriptBrowser.Instance.ScriptGuidDic.TryGetValue(scriptGuid, out scriptData);
				if (flag3)
				{
					argInputField.SetTextWithoutNotify(scriptData.FileName);
				}
				else
				{
					Debug.Log("Failed to find script with guid :" + scriptGuid);
				}
			}
			else
			{
				argInputField.SetTextWithoutNotify(this.UnwrapArgumentValue(CS$<>8__locals1.argCfg, CS$<>8__locals1.argData.Value));
			}
			UnityAction<string> showHint = new UnityAction<string>(CS$<>8__locals1.<InitArgument>g__ShowHint|1);
			argInputField.onSelect.RemoveAllListeners();
			argInputField.onSelect.AddListener(showHint);
			argInputField.onValueChanged.RemoveAllListeners();
			argInputField.onValueChanged.AddListener(showHint);
			argInputField.onEndEdit.RemoveAllListeners();
			argInputField.onEndEdit.AddListener(new UnityAction<string>(CS$<>8__locals1.<InitArgument>g__OnTextInputEndEdit|2));
			argInputField.onEndEdit.AddListener(delegate(string val)
			{
				base.<InitArgument>g__OnCheckArgumentType|8(val, CS$<>8__locals1.index);
			});
			this.SetArgumentContentType(argInputField, CS$<>8__locals1.argCfg, CS$<>8__locals1.argData.IsExpression);
			TextMeshProUGUI placeHolder = (TextMeshProUGUI)argInputField.placeholder;
			placeHolder.text = hintText;
			bool flag4 = CS$<>8__locals1.argCfg.TemplateId == 3;
			if (flag4)
			{
				TextMeshProUGUI label = argToggle.GetComponentInChildren<TextMeshProUGUI>(true);
				label.text = ((!string.IsNullOrEmpty(argName)) ? argName : CS$<>8__locals1.argCfg.Name);
				argToggle.onValueChanged.RemoveAllListeners();
				bool val2;
				argToggle.isOn = (bool.TryParse(CS$<>8__locals1.argData.Value, out val2) && val2);
				argToggle.onValueChanged.AddListener(new UnityAction<bool>(CS$<>8__locals1.<InitArgument>g__OnBoolValueChanged|4));
			}
			else
			{
				bool flag5 = CS$<>8__locals1.argCfg.TemplateId == 23;
				if (flag5)
				{
					short value;
					bool flag6 = !short.TryParse(CS$<>8__locals1.argData.Value, out value);
					if (flag6)
					{
						value = -1;
					}
					TextMeshProUGUI label2 = argButton.GetComponentInChildren<TextMeshProUGUI>(true);
					label2.text = this.GetArgumentDisplayName(CS$<>8__locals1.index);
					argButton.ClearAndAddListener(delegate
					{
						CS$<>8__locals1.<>4__this.OpenSelectItemSubType(value, new Action<short>(CS$<>8__locals1.<InitArgument>g__OnItemSubTypeChanged|6));
					});
				}
				else
				{
					bool flag7 = CS$<>8__locals1.argCfg.TemplateId == 8;
					if (flag7)
					{
						uint uintVal;
						TemplateKey value = uint.TryParse(CS$<>8__locals1.argData.Value, out uintVal) ? ((TemplateKey)uintVal) : new TemplateKey(-1, -1);
						TextMeshProUGUI label3 = argButton.GetComponentInChildren<TextMeshProUGUI>(true);
						label3.text = this.GetArgumentDisplayName(CS$<>8__locals1.index);
						argButton.ClearAndAddListener(delegate
						{
							EventEditorInstruction <>4__this = CS$<>8__locals1.<>4__this;
							sbyte itemType = value.ItemType;
							short templateId2 = value.TemplateId;
							Action<ValueTuple<sbyte, short>> onSelect;
							if ((onSelect = CS$<>8__locals1.<>9__11) == null)
							{
								onSelect = (CS$<>8__locals1.<>9__11 = delegate(ValueTuple<sbyte, short> tuple)
								{
									TemplateKey selectedValue = new TemplateKey(tuple.Item1, tuple.Item2);
									base.<InitArgument>g__OnItemTemplateValueChanged|5(selectedValue);
								});
							}
							<>4__this.OpenSelectItemTemplate(itemType, templateId2, onSelect);
						});
					}
					else
					{
						bool flag8 = CS$<>8__locals1.argCfg.TemplateId == 78;
						if (flag8)
						{
							TextMeshProUGUI label4 = argButton.GetComponentInChildren<TextMeshProUGUI>(true);
							label4.text = this.GetArgumentDisplayName(CS$<>8__locals1.index);
							argButton.ClearAndAddListener(delegate
							{
								CS$<>8__locals1.<>4__this.OpenMultiSelectItemTemplate(CS$<>8__locals1.argData.Value, new Action<List<EditingAdventureData.ItemCostItem>>(base.<InitArgument>g__OnItemTemplateListChanged|7));
							});
						}
					}
				}
			}
			isExpressionToggle.onValueChanged.RemoveAllListeners();
			isExpressionToggle.isOn = CS$<>8__locals1.argData.IsExpression;
			isExpressionToggle.onValueChanged.AddListener(new UnityAction<bool>(CS$<>8__locals1.<InitArgument>g__OnIsExpressionChanged|3));
			isExpressionToggle.interactable = CS$<>8__locals1.argCfg.AllowSwitchingExpression;
			bool showToggleInput = CS$<>8__locals1.argCfg.TemplateId == 3 && !CS$<>8__locals1.argData.IsExpression;
			int templateId = CS$<>8__locals1.argCfg.TemplateId;
			bool showButtonInput = (templateId == 8 || templateId == 78 || templateId == 23) && !CS$<>8__locals1.argData.IsExpression;
			argInputField.gameObject.SetActive(!showToggleInput && !showButtonInput);
			argToggle.gameObject.SetActive(showToggleInput);
			argButton.gameObject.SetActive(showButtonInput);
			argObj.SetActive(true);
		}

		// Token: 0x06004C2E RID: 19502 RVA: 0x0023F0DC File Offset: 0x0023D2DC
		private void OpenSelectItemTemplate(sbyte itemType, short templateId, Action<ValueTuple<sbyte, short>> onSelect)
		{
			EventEditorInstruction._itemTemplateArr[0] = new ValueTuple<sbyte, short>(itemType, templateId);
			EditingAdventureData.ItemCostItem[] initialSelection = (itemType >= 0) ? EventEditorInstruction._itemTemplateArr : null;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MultipleChoice", false).Set("SelectType", UI_ItemTemplateSelector.ESelectType.ItemTemplate).SetObject("InitialSelection", initialSelection).SetObject("OnConfirm", new UI_ItemTemplateSelector.OnConfirmHandler(delegate(List<EditingAdventureData.ItemCostItem> list)
			{
				EditingAdventureData.ItemCostItem result = (list.Count == 1) ? list[0] : new ValueTuple<sbyte, short>(-1, -1);
				onSelect(result);
			}));
			UIElement.ItemTemplateSelector.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ItemTemplateSelector, true);
		}

		// Token: 0x06004C2F RID: 19503 RVA: 0x0023F17C File Offset: 0x0023D37C
		private void OpenMultiSelectItemTemplate(string data, Action<List<EditingAdventureData.ItemCostItem>> onSelect)
		{
			List<TemplateKey> keyList = EventEditorInstruction.GetItemTemplateListByString(data);
			EditingAdventureData.ItemCostItem[] initialSelection = null;
			bool flag = keyList.Count > 0;
			if (flag)
			{
				initialSelection = new EditingAdventureData.ItemCostItem[keyList.Count];
				for (int index = 0; index < keyList.Count; index++)
				{
					TemplateKey key = keyList[index];
					initialSelection[index] = new ValueTuple<sbyte, short>(key.ItemType, key.TemplateId);
				}
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MultipleChoice", true).Set("SelectType", UI_ItemTemplateSelector.ESelectType.ItemTemplate).SetObject("InitialSelection", initialSelection).SetObject("OnConfirm", new UI_ItemTemplateSelector.OnConfirmHandler(onSelect.Invoke));
			UIElement.ItemTemplateSelector.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ItemTemplateSelector, true);
		}

		// Token: 0x06004C30 RID: 19504 RVA: 0x0023F254 File Offset: 0x0023D454
		private void OpenSelectItemSubType(short itemSubType, Action<short> onSelect)
		{
			sbyte itemType = ItemSubType.GetType(itemSubType);
			EventEditorInstruction._itemTemplateArr[0] = new ValueTuple<sbyte, short>(itemType, UI_ItemTemplateSelector.WrapOrUnwrapItemSubType(itemSubType));
			EditingAdventureData.ItemCostItem[] initialSelection = (itemSubType >= 0) ? EventEditorInstruction._itemTemplateArr : null;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("MultipleChoice", false).Set("SelectType", UI_ItemTemplateSelector.ESelectType.ItemSubType).SetObject("InitialSelection", initialSelection).SetObject("OnConfirm", new UI_ItemTemplateSelector.OnConfirmHandler(delegate(List<EditingAdventureData.ItemCostItem> list)
			{
				short result = (list.Count == 1) ? UI_ItemTemplateSelector.WrapOrUnwrapItemSubType(list[0].Item2) : -1;
				onSelect(result);
			}));
			UIElement.ItemTemplateSelector.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ItemTemplateSelector, true);
		}

		// Token: 0x06004C31 RID: 19505 RVA: 0x0023F300 File Offset: 0x0023D500
		private void SetArgumentContentType(TMP_InputField inputField, EventArgumentItem argConfig, bool isExpression)
		{
			if (isExpression)
			{
				inputField.contentType = TMP_InputField.ContentType.Standard;
			}
			else
			{
				int templateId = argConfig.TemplateId;
				int num = templateId;
				if (num != 1)
				{
					if (num != 2)
					{
						inputField.contentType = TMP_InputField.ContentType.Standard;
					}
					else
					{
						inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
					}
				}
				else
				{
					inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
				}
			}
		}

		// Token: 0x06004C32 RID: 19506 RVA: 0x0023F354 File Offset: 0x0023D554
		private string WrapArgumentValue(EventArgumentItem argConfig, string val)
		{
			return val;
		}

		// Token: 0x06004C33 RID: 19507 RVA: 0x0023F368 File Offset: 0x0023D568
		private string UnwrapArgumentValue(EventArgumentItem argConfig, string val)
		{
			return val;
		}

		// Token: 0x06004C34 RID: 19508 RVA: 0x0023F37B File Offset: 0x0023D57B
		public void SetIndent(int indent)
		{
			this._data.Indent = indent;
			this._backgroundImg.offsetMin = new Vector2((float)(indent * 80), this._backgroundImg.offsetMin.y);
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x0023F3B0 File Offset: 0x0023D5B0
		private void UpdateArgumentHint(int index, string val)
		{
			EventEditorScript.Instance.DeselectAllInstructions();
			bool isExpression = this._data.Args[index].IsExpression;
			if (!isExpression)
			{
				int argTemplateId = this._data.FunctionConfig.ParameterTypes[index];
				EventArgumentItem argCfg = EventArgument.Instance[argTemplateId];
				TMP_InputField textInput = this._arguments[index].textInput;
				bool flag = argTemplateId == 5;
				if (flag)
				{
					EventEditorScript.Instance.ShowEventInputHint(val, textInput);
				}
				else
				{
					bool flag2 = argTemplateId == 48;
					if (flag2)
					{
						UI_SearchResultShow.ShowInputHint(val, textInput, EventEditorGlobalScriptBrowser.Instance.ScriptNames, null);
					}
					else
					{
						bool flag3 = argTemplateId == 89;
						if (flag3)
						{
							EventEditorScript.Instance.ShowConfigInputHint(val, textInput, SectMainStoryEventArgKey.Instance, argCfg.TemplateId);
						}
						else
						{
							bool flag4 = argTemplateId == 22;
							if (flag4)
							{
								EventEditorScript.Instance.ShowItemTypeInputHint(val, textInput);
							}
							else
							{
								bool flag5 = argTemplateId == 33;
								if (flag5)
								{
									EventEditorScript.Instance.ShowWorldFunctionTypeInputHint(val, textInput);
								}
								else
								{
									bool flag6 = argTemplateId == 61;
									if (flag6)
									{
										EventEditorScript.Instance.ShowAdventureElementInputHint(val, textInput);
									}
									else
									{
										bool flag7 = argTemplateId == 71;
										if (flag7)
										{
											EventEditorScript.Instance.ShowMajorEventIdInputHint(val, textInput);
										}
										else
										{
											bool flag8 = argTemplateId == 81;
											if (flag8)
											{
												EventEditorScript.Instance.ShowAdventureRemakeTemplateInputHint(val, textInput);
											}
											else
											{
												bool flag9 = argTemplateId == 91;
												if (flag9)
												{
													EventEditorScript.Instance.ShowLanguageKeyInputHint(val, textInput);
												}
												else
												{
													IConfigData configData;
													bool flag10 = !string.IsNullOrEmpty(argCfg.ConfigTable) && ConfigCollection.NameMap.TryGetValue(argCfg.ConfigTable, out configData);
													if (flag10)
													{
														EventEditorScript.Instance.ShowConfigInputHint(val, textInput, configData, argCfg.TemplateId);
													}
													else
													{
														string[] customEnumText = argCfg.CustomEnumText;
														bool flag11 = customEnumText != null && customEnumText.Length > 0;
														if (flag11)
														{
															UI_SearchResultShow.ShowInputHint(val, textInput, argCfg.CustomEnumText, null);
														}
														else
														{
															EventEditorScript.Instance.ShowVariableNameInputHint(val, textInput, argCfg.TemplateId);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06004C36 RID: 19510 RVA: 0x0023F5B0 File Offset: 0x0023D7B0
		private void EditArgumentIsExpression(int index, bool isExpression)
		{
			EventEditorScript.Instance.DeselectAllInstructions();
			bool prevIsExpression = this._data.Args[index].IsExpression;
			bool flag = prevIsExpression == isExpression;
			if (!flag)
			{
				OperateCommand command = new OperateCommand("EditArgumentIsExpression")
				{
					Do = delegate()
					{
						this.OfflineSetIsExpression(index, isExpression);
					},
					Undo = delegate()
					{
						this.OfflineSetIsExpression(index, prevIsExpression);
					}
				};
				EventEditorScript.Instance.ExecuteOperateCommand(command);
			}
		}

		// Token: 0x06004C37 RID: 19511 RVA: 0x0023F650 File Offset: 0x0023D850
		private void OfflineSetIsExpression(int index, bool isExpression)
		{
			EventArgumentItem argConfig = EventArgument.Instance[this._data.FunctionConfig.ParameterTypes[index]];
			this._data.Args[index].IsExpression = isExpression;
			this._data.Args[index].Value = (isExpression ? EventEditorScript.Instance.ConvertValueToExpression(argConfig, this._data.Args[index].Value) : EventEditorScript.Instance.ConvertValueFromExpression(argConfig, this._data.Args[index].Value));
			this._arguments[index].isExpression.SetIsOnWithoutNotify(isExpression);
			TMP_InputField inputField = this._arguments[index].textInput;
			this.SetArgumentContentType(inputField, argConfig, isExpression);
			inputField.SetTextWithoutNotify(this._data.Args[index].Value);
			bool flag = argConfig.TemplateId == 3;
			if (flag)
			{
				CToggle boolInput = this._arguments[index].boolInput;
				boolInput.gameObject.SetActive(!isExpression);
				inputField.gameObject.SetActive(isExpression);
				bool flag2 = !isExpression;
				if (flag2)
				{
					bool boolVal;
					boolInput.SetIsOnWithoutNotify(bool.TryParse(this._data.Args[index].Value, out boolVal) && boolVal);
				}
			}
			else
			{
				int templateId = argConfig.TemplateId;
				bool flag3 = templateId == 8 || templateId == 78 || templateId == 23;
				if (flag3)
				{
					CButton buttonInput = this._arguments[index].buttonInput;
					buttonInput.gameObject.SetActive(!isExpression);
					inputField.gameObject.SetActive(isExpression);
					bool flag4 = !isExpression;
					if (flag4)
					{
						buttonInput.GetComponentInChildren<TextMeshProUGUI>(true).text = this.GetArgumentDisplayName(index);
					}
				}
			}
		}

		// Token: 0x06004C38 RID: 19512 RVA: 0x0023F814 File Offset: 0x0023DA14
		private void EditArgument(int index, string val)
		{
			EventEditorScript.Instance.DeselectAllInstructions();
			string prevVal = this._data.Args[index].Value;
			bool flag = prevVal == val;
			if (!flag)
			{
				EventEditorScript scriptEditor = EventEditorScript.Instance;
				int instIndex = scriptEditor.FindInstructionIndex(this);
				OperateCommand editCommand = new OperateCommand("EditArgument")
				{
					Do = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(instIndex);
						EventEditorScriptInputArgTemplate argument = instruction._arguments[index];
						TMP_InputField textInput = argument.textInput;
						CToggle boolInput = argument.boolInput;
						textInput.SetTextWithoutNotify(val);
						bool activeSelf = boolInput.gameObject.activeSelf;
						if (activeSelf)
						{
							bool boolVal;
							boolInput.SetIsOnWithoutNotify(bool.TryParse(val, out boolVal) && boolVal);
						}
						instruction._data.Args[index].Value = val;
					},
					Undo = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(instIndex);
						EventEditorScriptInputArgTemplate argument = instruction._arguments[index];
						TMP_InputField textInput = argument.textInput;
						CToggle boolInput = argument.boolInput;
						textInput.SetTextWithoutNotify(prevVal);
						bool activeSelf = boolInput.gameObject.activeSelf;
						if (activeSelf)
						{
							bool boolVal;
							boolInput.SetIsOnWithoutNotify(bool.TryParse(prevVal, out boolVal) && boolVal);
						}
						instruction._data.Args[index].Value = prevVal;
					}
				};
				EventEditorScript.Instance.ExecuteOperateCommand(editCommand);
			}
		}

		// Token: 0x06004C39 RID: 19513 RVA: 0x0023F8D0 File Offset: 0x0023DAD0
		private void EditButtonArgument(int index, List<EditingAdventureData.ItemCostItem> templateKeys)
		{
			EventEditorScript.Instance.DeselectAllInstructions();
			string prevVal = this._data.Args[index].Value;
			List<TemplateKey> keys = new List<TemplateKey>();
			foreach (EditingAdventureData.ItemCostItem key in templateKeys)
			{
				keys.Add(new TemplateKey(key.Item1, key.Item2));
			}
			string val = EventEditorInstruction.GetStringByItemTemplateList(keys);
			bool flag = prevVal == val;
			if (!flag)
			{
				EventEditorScript scriptEditor = EventEditorScript.Instance;
				int instIndex = scriptEditor.FindInstructionIndex(this);
				OperateCommand editCommand = new OperateCommand("EditArgument")
				{
					Do = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(instIndex);
						EventEditorScriptInputArgTemplate argument = instruction._arguments[index];
						TMP_InputField textInput = argument.textInput;
						CToggle boolInput = argument.boolInput;
						TextMeshProUGUI buttonText = argument.buttonInput.GetComponentInChildren<TextMeshProUGUI>(true);
						textInput.SetTextWithoutNotify(val);
						bool activeSelf = boolInput.gameObject.activeSelf;
						if (activeSelf)
						{
							bool boolVal;
							boolInput.SetIsOnWithoutNotify(bool.TryParse(val, out boolVal) && boolVal);
						}
						instruction._data.Args[index].Value = val;
						buttonText.text = this.GetArgumentDisplayName(index);
					},
					Undo = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(instIndex);
						EventEditorScriptInputArgTemplate argument = instruction._arguments[index];
						TMP_InputField textInput = argument.textInput;
						CToggle boolInput = argument.boolInput;
						TextMeshProUGUI buttonText = argument.buttonInput.GetComponentInChildren<TextMeshProUGUI>(true);
						textInput.SetTextWithoutNotify(prevVal);
						bool activeSelf = boolInput.gameObject.activeSelf;
						if (activeSelf)
						{
							bool boolVal;
							boolInput.SetIsOnWithoutNotify(bool.TryParse(prevVal, out boolVal) && boolVal);
						}
						instruction._data.Args[index].Value = prevVal;
						buttonText.text = this.GetArgumentDisplayName(index);
					}
				};
				EventEditorScript.Instance.ExecuteOperateCommand(editCommand);
			}
		}

		// Token: 0x06004C3A RID: 19514 RVA: 0x0023F9F4 File Offset: 0x0023DBF4
		private void EditButtonArgument(int index, string val)
		{
			EventEditorScript.Instance.DeselectAllInstructions();
			string prevVal = this._data.Args[index].Value;
			bool flag = prevVal == val;
			if (!flag)
			{
				EventEditorScript scriptEditor = EventEditorScript.Instance;
				int instIndex = scriptEditor.FindInstructionIndex(this);
				OperateCommand editCommand = new OperateCommand("EditArgument")
				{
					Do = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(instIndex);
						instruction._data.Args[index].Value = val;
						EventEditorScriptInputArgTemplate argument = instruction._arguments[index];
						TMP_InputField textInput = argument.textInput;
						CButton buttonInput = argument.buttonInput;
						textInput.SetTextWithoutNotify(val);
						bool activeSelf = buttonInput.gameObject.activeSelf;
						if (activeSelf)
						{
							TextMeshProUGUI label = buttonInput.GetComponentInChildren<TextMeshProUGUI>(true);
							label.text = instruction.GetArgumentDisplayName(index);
						}
					},
					Undo = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(instIndex);
						instruction._data.Args[index].Value = prevVal;
						EventEditorScriptInputArgTemplate argument = instruction._arguments[index];
						TMP_InputField textInput = argument.textInput;
						CButton buttonInput = argument.buttonInput;
						textInput.SetTextWithoutNotify(prevVal);
						bool activeSelf = buttonInput.gameObject.activeSelf;
						if (activeSelf)
						{
							TextMeshProUGUI label = buttonInput.GetComponentInChildren<TextMeshProUGUI>(true);
							label.text = instruction.GetArgumentDisplayName(index);
						}
					}
				};
				EventEditorScript.Instance.ExecuteOperateCommand(editCommand);
			}
		}

		// Token: 0x06004C3B RID: 19515 RVA: 0x0023FAB0 File Offset: 0x0023DCB0
		private bool CheckArgument(EventArgumentItem argCfg, EventArgumentEditorData argData, int index, out string log)
		{
			log = "";
			try
			{
				string value = this.Data.GetArgString(index);
				bool flag = !EventEditorScript.Instance.IsExpressionValid(argCfg, value);
				if (flag)
				{
					return false;
				}
				Debug.Log("动态类型参数类型检查成功，编译成功！");
			}
			catch (Exception e)
			{
				log = e.Message + " at: " + e.StackTrace;
				return false;
			}
			return true;
		}

		// Token: 0x06004C3C RID: 19516 RVA: 0x0023FB30 File Offset: 0x0023DD30
		private void OnArgumentTypeWrong(string addLog, EventArgumentItem argCfg = null, EventArgumentEditorData argDate = null)
		{
			bool flag = !string.IsNullOrEmpty(addLog);
			if (flag)
			{
				EventEditorScript.Instance.ShowError(addLog, true);
			}
			else
			{
				string log = argDate.Value + " 不符合参数类型： " + argCfg.Name;
				EventEditorScript.Instance.ShowError(log, true);
			}
		}

		// Token: 0x06004C3D RID: 19517 RVA: 0x0023FB82 File Offset: 0x0023DD82
		private void OnArgumentTypeRight()
		{
			EventEditorScript.Instance.ClearErrorLog();
		}

		// Token: 0x06004C3E RID: 19518 RVA: 0x0023FB90 File Offset: 0x0023DD90
		private void EditReverse(bool isOn)
		{
			bool prevVal = this._data.Reverse;
			bool flag = prevVal == isOn;
			if (!flag)
			{
				EventEditorScript scriptEditor = EventEditorScript.Instance;
				int index = scriptEditor.FindInstructionIndex(this);
				OperateCommand editCommand = new OperateCommand("EditReverse")
				{
					Do = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(index);
						instruction._isReverseToggle.SetIsOnWithoutNotify(isOn);
						instruction._data.Reverse = isOn;
					},
					Undo = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(index);
						instruction._isReverseToggle.SetIsOnWithoutNotify(prevVal);
						instruction._data.Reverse = prevVal;
					}
				};
				EventEditorScript.Instance.ExecuteOperateCommand(editCommand);
			}
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x0023FC28 File Offset: 0x0023DE28
		private void EditRetVariable(string variableName)
		{
			string prevVariableName = this._data.AssignToVar;
			bool flag = prevVariableName == variableName;
			if (!flag)
			{
				EventEditorScript scriptEditor = EventEditorScript.Instance;
				int index = scriptEditor.FindInstructionIndex(this);
				OperateCommand editCommand = new OperateCommand("EditRetVariable")
				{
					Do = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(index);
						instruction._retValInputField.SetTextWithoutNotify(variableName);
						instruction._data.AssignToVar = variableName;
						scriptEditor.UpdateScriptIdentifiers();
					},
					Undo = delegate()
					{
						EventEditorInstruction instruction = scriptEditor.GetInstruction(index);
						instruction._retValInputField.SetTextWithoutNotify(prevVariableName);
						instruction._data.AssignToVar = prevVariableName;
						scriptEditor.UpdateScriptIdentifiers();
					}
				};
				EventEditorScript.Instance.ExecuteOperateCommand(editCommand);
			}
		}

		// Token: 0x06004C40 RID: 19520 RVA: 0x0023FCC4 File Offset: 0x0023DEC4
		private string GetArgumentDisplayName(int index)
		{
			EventFunctionItem funcCfg = this._data.FunctionConfig;
			string argName = funcCfg.ParameterNames.CheckIndex(index) ? funcCfg.ParameterNames[index] : string.Empty;
			EventArgumentItem argCfg = EventArgument.Instance[funcCfg.ParameterTypes[index]];
			EventArgumentEditorData argData = this._data.Args[index];
			bool flag = argCfg.TemplateId == 8;
			string result;
			if (flag)
			{
				uint uintVal;
				TemplateKey templateKey = uint.TryParse(argData.Value, out uintVal) ? ((TemplateKey)uintVal) : new TemplateKey(-1, -1);
				result = ((templateKey.ItemType >= 0) ? ItemTemplateHelper.GetName(templateKey.ItemType, templateKey.TemplateId) : (string.IsNullOrEmpty(argName) ? argCfg.Name : argName));
			}
			else
			{
				bool flag2 = argCfg.TemplateId == 78;
				if (flag2)
				{
					result = string.Format("{0}({1})", argCfg.Name, EventEditorInstruction.GetItemTemplateListByString(argData.Value).Count);
				}
				else
				{
					bool flag3 = argCfg.TemplateId == 23;
					if (flag3)
					{
						short itemSubType;
						bool flag4 = !short.TryParse(argData.Value, out itemSubType);
						if (flag4)
						{
							itemSubType = -1;
						}
						result = ((itemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", itemSubType)) : (string.IsNullOrEmpty(argName) ? argCfg.Name : argName));
					}
					else
					{
						result = argData.Value;
					}
				}
			}
			return result;
		}

		// Token: 0x06004C41 RID: 19521 RVA: 0x0023FE28 File Offset: 0x0023E028
		public static List<TemplateKey> GetItemTemplateListByString(string str)
		{
			List<TemplateKey> res = new List<TemplateKey>();
			bool flag = str.IsNullOrEmpty();
			List<TemplateKey> result;
			if (flag)
			{
				result = res;
			}
			else
			{
				string[] keyList = str.Replace("ItemTemplate", "").Replace("[", "").Replace("]", "").Split(", ", StringSplitOptions.None);
				foreach (string key in keyList)
				{
					string[] itemTemplateId = key.Replace("{", "").Replace("}", "").Replace("ItemType=", "").Replace("TemplateId=", "").Split(',', StringSplitOptions.None);
					sbyte type;
					short templateId;
					bool flag2 = sbyte.TryParse(itemTemplateId[0], out type) && short.TryParse(itemTemplateId[1], out templateId);
					if (flag2)
					{
						res.Add(new TemplateKey(type, templateId));
					}
				}
				result = res;
			}
			return result;
		}

		// Token: 0x06004C42 RID: 19522 RVA: 0x0023FF2C File Offset: 0x0023E12C
		public static string GetStringByItemTemplateList(List<TemplateKey> list)
		{
			bool flag = list == null || list.Count == 0;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("ItemTemplate[");
				for (int index = 0; index < list.Count; index++)
				{
					TemplateKey key = list[index];
					sb.Append("ItemTemplate{");
					sb.Append(string.Format("ItemType={0},", key.ItemType));
					sb.Append(string.Format("TemplateId={0}", key.TemplateId));
					sb.Append("}");
					bool flag2 = index != list.Count - 1;
					if (flag2)
					{
						sb.Append(", ");
					}
				}
				sb.Append("]");
				result = sb.ToString();
			}
			return result;
		}

		// Token: 0x040034E2 RID: 13538
		[SerializeField]
		private HorizontalLayoutGroup _elementRoot;

		// Token: 0x040034E3 RID: 13539
		[SerializeField]
		private TMP_InputField _retValInputField;

		// Token: 0x040034E4 RID: 13540
		[SerializeField]
		private GameObject _assignLabel;

		// Token: 0x040034E5 RID: 13541
		[SerializeField]
		private TextMeshProUGUI _funcNameLabel;

		// Token: 0x040034E6 RID: 13542
		[SerializeField]
		private EventEditorScriptInputArgTemplate _textInputArgTemplate;

		// Token: 0x040034E7 RID: 13543
		[SerializeField]
		private RectTransform _backgroundImg;

		// Token: 0x040034E8 RID: 13544
		[SerializeField]
		private CToggle _isReverseToggle;

		// Token: 0x040034E9 RID: 13545
		[SerializeField]
		private TextMeshProUGUI _lineIndexLabel;

		// Token: 0x040034EA RID: 13546
		public GameObject Selected;

		// Token: 0x040034EB RID: 13547
		private List<EventEditorScriptInputArgTemplate> _arguments;

		// Token: 0x040034EC RID: 13548
		private EventInstructionEditorData _data;

		// Token: 0x040034ED RID: 13549
		private static StringBuilder _stringBuilder = new StringBuilder();

		// Token: 0x040034EE RID: 13550
		private static EditingAdventureData.ItemCostItem[] _itemTemplateArr = new EditingAdventureData.ItemCostItem[1];
	}
}
