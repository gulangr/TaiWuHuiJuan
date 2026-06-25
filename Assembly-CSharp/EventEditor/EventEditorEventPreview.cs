using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.Tweening;
using EventEditor.EventScript;
using FrameWork;
using FrameWork.ExternalTexture;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Migrate;
using GameData.Domains.TaiwuEvent.EventOption;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventEditor
{
	// Token: 0x02000642 RID: 1602
	public class EventEditorEventPreview : EventEditorSubPageBase
	{
		// Token: 0x06004BCB RID: 19403 RVA: 0x0023B046 File Offset: 0x00239246
		public static void Init(EventEditorEventPreview instance)
		{
			EventEditorEventPreview.Instance = instance;
			EventEditorEventPreview.Instance.InternalInit();
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x0023B05C File Offset: 0x0023925C
		protected override void InternalInit()
		{
			this._optionBehaviorDropdownOptions = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Goodness_None),
				LocalStringManager.Get(LanguageKey.LK_Goodness_0).ColorReplace(),
				LocalStringManager.Get(LanguageKey.LK_Goodness_1).ColorReplace(),
				LocalStringManager.Get(LanguageKey.LK_Goodness_2).ColorReplace(),
				LocalStringManager.Get(LanguageKey.LK_Goodness_3).ColorReplace(),
				LocalStringManager.Get(LanguageKey.LK_Goodness_4).ColorReplace()
			};
			PoolManager.SetSrcObject("EventEditorOptionItemStyle_1", this.goOptionItemStyle1);
			EventEditorSelectJumpEvent.Instance.OnSelected = new Action<string>(this.OnInternalJumpToEvent);
			this.btnAddOption.ClearAndAddListener(new Action(this.OnAddOption));
			EventEditorEventPreviewOptionItemStyle1Info optionItemInfo = this.goOptionItemStyle1.GetComponent<EventEditorEventPreviewOptionItemStyle1Info>();
			optionItemInfo.behaviorDropdown.ClearOptions();
			optionItemInfo.behaviorDropdown.AddOptions(this._optionBehaviorDropdownOptions);
			this._jumpStack = new List<string>();
			this.btnBack.ClearAndAddListener(new Action(this.OnStackBackEvent));
			this.btnForward.ClearAndAddListener(new Action(this.OnStackForwardEvent));
			this._adaptableTextureNames = new List<string>();
			this._nameKeyTextureGroup = (NameKeyTextureGroup)SingletonObject.getInstance<TextureCenter>().GetTextureGroup(ModManager.GetWorkingModName());
			this._adaptableTextureNames.AddRange(this._nameKeyTextureGroup.GetAllTextureNames());
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x0023B1D4 File Offset: 0x002393D4
		public override void Show()
		{
			this.mainWindow.anchoredPosition = Vector2.zero;
		}

		// Token: 0x06004BCE RID: 19406 RVA: 0x0023B1E8 File Offset: 0x002393E8
		public override void Hide()
		{
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x0023B1EB File Offset: 0x002393EB
		private void OnEnable()
		{
			GEvent.Add(ModEditorEvents.EventDeleted, new GEvent.Callback(this.OnDeleteEvent));
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x0023B206 File Offset: 0x00239406
		private void OnDisable()
		{
			GEvent.Remove(ModEditorEvents.EventDeleted, new GEvent.Callback(this.OnDeleteEvent));
		}

		// Token: 0x06004BD1 RID: 19409 RVA: 0x0023B221 File Offset: 0x00239421
		private void OnDestroy()
		{
			PoolManager.RemoveData("EventEditorOptionItemStyle_1");
		}

		// Token: 0x06004BD2 RID: 19410 RVA: 0x0023B230 File Offset: 0x00239430
		private void Update()
		{
			bool flag = EventEditorCommandKit.AddOption.Check(UIElement.EventEditor, false, false, false, true, false);
			if (flag)
			{
				bool isEventEditorShow = TaskControlPanel.Instance.isEventEditorShow;
				if (isEventEditorShow)
				{
					this.OnAddOption();
					return;
				}
			}
			bool keyDown = Input.GetKeyDown(KeyCode.LeftAlt);
			if (keyDown)
			{
				this._optionBehaviorShowState = true;
				this.UpdateOptionBehaviorShowState();
			}
			bool keyUp = Input.GetKeyUp(KeyCode.LeftAlt);
			if (keyUp)
			{
				this._optionBehaviorShowState = false;
				this.UpdateOptionBehaviorShowState();
			}
			bool keyUp2 = Input.GetKeyUp(KeyCode.Mouse3);
			if (keyUp2)
			{
				this.OnStackBackEvent();
			}
			bool keyUp3 = Input.GetKeyUp(KeyCode.Mouse4);
			if (keyUp3)
			{
				this.OnStackForwardEvent();
			}
			bool optionBehaviorShowState = this._optionBehaviorShowState;
			if (optionBehaviorShowState)
			{
				bool keyUp4 = Input.GetKeyUp(KeyCode.LeftArrow);
				if (keyUp4)
				{
					this.OnStackBackEvent();
				}
				bool keyUp5 = Input.GetKeyUp(KeyCode.RightArrow);
				if (keyUp5)
				{
					this.OnStackForwardEvent();
				}
			}
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x0023B319 File Offset: 0x00239519
		public void ClearStack()
		{
			this._stackIndex = 0;
			List<string> jumpStack = this._jumpStack;
			if (jumpStack != null)
			{
				jumpStack.Clear();
			}
		}

		// Token: 0x06004BD4 RID: 19412 RVA: 0x0023B338 File Offset: 0x00239538
		public void Refresh(EventEditorData eventData = null)
		{
			this._curEvent = eventData;
			bool flag = eventData == null;
			if (flag)
			{
				this.Reset();
			}
			else
			{
				this.UpdateTexture();
				string eventContent = this._curEvent.EventContent;
				this.txtMeshEventContent.text = (eventContent.IsNullOrEmpty() ? eventContent : EventEditorStringCenter.DecodeTag(eventContent.Replace("<NL>", "\n").ColorReplace()));
				this.RefreshTargetRole();
				PointClickBridge clickBridge = this.eventContentContainer;
				clickBridge.OnDoubleClick = new Action(this.OnEditContent);
				clickBridge.OnRightClick = new Action(this.OnContentRightClick);
				this._optionBehaviorShowState = false;
				this.UpdateOptions();
			}
		}

		// Token: 0x06004BD5 RID: 19413 RVA: 0x0023B3E8 File Offset: 0x002395E8
		public void RefreshTargetRole()
		{
			string targetRoleKey = this._curEvent.TargetRole;
			bool hasTargetRole = !string.IsNullOrEmpty(targetRoleKey);
			this.goEventActor.SetActive(hasTargetRole);
			bool flag = hasTargetRole;
			if (flag)
			{
				EventEditorRole role = EventEditorSimulateEnvironment.Instance.GetRole(targetRoleKey);
				bool flag2 = role != null;
				if (flag2)
				{
					role.OnDataChange = new Action(this.RefreshTargetRole);
					this.avatar.Refresh(role.AvatarData, (short)role.Age);
					this.txtMeshActorName.text = role.GetName();
				}
				else
				{
					this.goEventActor.SetActive(false);
				}
			}
		}

		// Token: 0x06004BD6 RID: 19414 RVA: 0x0023B484 File Offset: 0x00239684
		public void UpdateTexture()
		{
			string textureName = this._curEvent.EventTexture;
			this.rawImgEventTexture.texture = this._nameKeyTextureGroup.GetTexture(string.IsNullOrEmpty(textureName) ? this._adaptableTextureNames.GetRandom<string>() : textureName);
			this.rawImgEventTexture.DOFade(1f, 0.3f);
		}

		// Token: 0x06004BD7 RID: 19415 RVA: 0x0023B4E4 File Offset: 0x002396E4
		public void UpdateOptions()
		{
			CToggleGroup toggleGroup = this.optionToggleGroup;
			toggleGroup.Clear();
			toggleGroup.OnActiveIndexChange -= this.OnToggleGroupIndexChange;
			this.RestoreAllOptionCells();
			string decideRoleKey = this._curEvent.DecideRole;
			bool hasDecideRole = !string.IsNullOrEmpty(decideRoleKey);
			sbyte onKey = -1;
			string escOptionKey = string.Empty;
			bool flag = !string.IsNullOrEmpty(this._curEvent.EscOption);
			if (flag)
			{
				escOptionKey = this._curEvent.EscOption;
			}
			Dictionary<int, EventEditorData.Option> options = this._curEvent.Options;
			bool flag2 = options != null;
			if (flag2)
			{
				foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in options)
				{
					int num;
					EventEditorData.Option option2;
					keyValuePair.Deconstruct(out num, out option2);
					int i = num;
					EventEditorData.Option option = option2;
					EventEditorEventPreviewOptionItemStyle1Info optionItemInfo = PoolManager.GetObject<EventEditorEventPreviewOptionItemStyle1Info>("EventEditorOptionItemStyle_1");
					optionItemInfo.gameObject.name = option.OptionKey;
					int index = i;
					optionItemInfo.btnEditScript.ClearAndAddListener(delegate
					{
						this.OnEditOptionScript(index);
					});
					optionItemInfo.btnEditNextEvent.ClearAndAddListener(delegate
					{
						this.OnOptionNextEvent(index);
					});
					optionItemInfo.txtMeshOptionIndex.text = string.Format("({0}).", index);
					optionItemInfo.txtMeshOptionDesc.text = option.Content.ColorReplace();
					EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
					string optionGuid = option.Guid;
					string saveDir = model.EnsureEventScriptPath(this._curEvent);
					string scriptUrl = Path.Combine(saveDir, optionGuid + ".tws");
					bool hasInstruction = File.Exists(scriptUrl);
					optionItemInfo.goHasInstruction.SetActive(hasInstruction);
					bool hasCode = SingletonObject.getInstance<EventEditorModel>().IsOptionCodeValidity(this._curEvent, optionGuid);
					optionItemInfo.goHasCode.SetActive(hasCode);
					StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
					int scriptRowCount = hasInstruction ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)).Instructions.Count : 0;
					stringBuilder.AppendLine(string.Format("指令行数: {0}", scriptRowCount));
					string visibleConditionUrl = Path.Combine(saveDir, optionGuid + "_visible.tws");
					int visibleConditionInstructionCount = File.Exists(visibleConditionUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(visibleConditionUrl)).Instructions.Count : 0;
					stringBuilder.AppendLine(string.Format("可见条件指令行数: {0}", visibleConditionInstructionCount));
					string availableConditionUrl = Path.Combine(saveDir, optionGuid + "_available.tws");
					int availableConditionInstructionCount = File.Exists(availableConditionUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(availableConditionUrl)).Instructions.Count : 0;
					stringBuilder.AppendLine(string.Format("可用条件指令行数: {0}", availableConditionInstructionCount));
					string state = OptionDefaultStateKeys.Normal;
					bool flag3 = !string.IsNullOrEmpty(option.DefaultState);
					if (flag3)
					{
						state = option.DefaultState;
						stringBuilder.AppendLine("默认状态: " + state);
					}
					TextMeshProUGUI contentLabel = optionItemInfo.txtMeshOptionDesc;
					CImage image = optionItemInfo.imgBack;
					Color color = image.color;
					string optionDesc = option.Content ?? string.Empty;
					bool flag4 = state.Equals(OptionDefaultStateKeys.Disable);
					if (flag4)
					{
						string info = "<color=#grey>" + optionDesc + "</color>";
						contentLabel.text = info.ColorReplace();
						color.a = 0.3f;
					}
					else
					{
						contentLabel.text = optionDesc.ColorReplace();
						color.a = 1f;
					}
					image.color = color;
					optionItemInfo.goReadStateContainer.SetActive(state == OptionDefaultStateKeys.UnRead || state == OptionDefaultStateKeys.Read);
					optionItemInfo.goRead.SetActive(state == OptionDefaultStateKeys.Read);
					optionItemInfo.goUnRead.SetActive(state == OptionDefaultStateKeys.UnRead);
					bool oneTimeOnly = option.OneTimeOnly;
					stringBuilder.AppendLine(string.Format("只可点击一次: {0}", oneTimeOnly));
					optionItemInfo.goOneTimeOnly.SetActive(oneTimeOnly);
					bool important = option.Important;
					stringBuilder.AppendLine(string.Format("重要选项: {0}", important));
					optionItemInfo.goImportant.SetActive(important);
					bool flag5 = !string.IsNullOrEmpty(option.RedirectTargetOption.Item1) && !string.IsNullOrEmpty(option.RedirectTargetOption.Item2);
					if (flag5)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionRedirect));
						stringBuilder.AppendLine(option.RedirectTargetOption.Item1);
						stringBuilder.Append(option.RedirectTargetOption.Item2);
					}
					optionItemInfo.mouseTip.PresetParam = new string[]
					{
						stringBuilder.ToString()
					};
					EasyPool.Free<StringBuilder>(stringBuilder);
					CDropdown behaviorDropdown = optionItemInfo.behaviorDropdown;
					behaviorDropdown.onValueChanged.RemoveAllListeners();
					int behaviorIndex = option.Behavior;
					behaviorDropdown.value = behaviorIndex;
					behaviorDropdown.onValueChanged.AddListener(delegate(int newValue)
					{
						this.OnOptionBehaviorDropdownValueChanged(index, newValue, behaviorDropdown);
					});
					optionItemInfo.txtMeshName.text = (hasDecideRole ? decideRoleKey : string.Empty);
					PointClickBridge pointClickBridge = optionItemInfo.clickBridge;
					pointClickBridge.OnLeftClick = delegate()
					{
						this.OnOptionClick(index);
					};
					pointClickBridge.OnDoubleClick = delegate()
					{
						this.OnOptionDoubleClick(index, optionItemInfo);
					};
					pointClickBridge.OnRightClick = delegate()
					{
						this.OnOptionRightClick(index);
					};
					optionItemInfo.transform.SetParent(this.optionRoot, false);
					optionItemInfo.transform.SetAsLastSibling();
					sbyte toggleKey = (sbyte)(i - 1);
					CToggle toggle = optionItemInfo.bindToEsc;
					toggle.isOn = false;
					toggleGroup.Add(toggle);
					string optionKey = option.OptionKey;
					bool flag6 = optionKey == escOptionKey;
					if (flag6)
					{
						onKey = toggleKey;
					}
					PointerTrigger trigger = optionItemInfo.trigger;
					trigger.EnterEvent.RemoveAllListeners();
					trigger.EnterEvent.AddListener(delegate()
					{
						optionItemInfo.goControlLayer.SetActive(true);
						optionItemInfo.goMarkLayer.SetActive(false);
					});
					trigger.ExitEvent.RemoveAllListeners();
					trigger.ExitEvent.AddListener(delegate()
					{
						bool flag8 = !this._optionBehaviorShowState && !behaviorDropdown.transform.Find("Dropdown List");
						if (flag8)
						{
							optionItemInfo.goControlLayer.SetActive(false);
							optionItemInfo.goMarkLayer.SetActive(true);
						}
					});
				}
				bool flag7 = onKey != -1;
				if (flag7)
				{
					toggleGroup.Set((int)onKey, false);
				}
				toggleGroup.OnActiveIndexChange += this.OnToggleGroupIndexChange;
			}
			this.UpdateOptionBehaviorShowState();
			this.btnAddOption.transform.SetAsLastSibling();
			base.StopAllCoroutines();
			base.StartCoroutine(this.AdjustOptionSize());
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x0023BC4C File Offset: 0x00239E4C
		private void OnToggleGroupIndexChange(int newIndex, int oldIndex)
		{
			EventEditorData.Option optionNew = null;
			bool flag = newIndex >= 0;
			if (flag)
			{
				optionNew = this._curEvent.Options[newIndex + 1];
			}
			string newEscOptionKey = (optionNew != null) ? optionNew.OptionKey : string.Empty;
			string preEscOptionKey = string.Empty;
			bool flag2 = !string.IsNullOrEmpty(this._curEvent.EscOption);
			if (flag2)
			{
				preEscOptionKey = this._curEvent.EscOption;
			}
			OperateCommand cmd = new OperateCommand("ChangeDecideRoleKey")
			{
				Do = delegate()
				{
					this._curEvent.EscOption = newEscOptionKey;
				},
				Undo = delegate()
				{
					this._curEvent.EscOption = preEscOptionKey;
				}
			};
			EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
			this.SetDirty();
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x0023BD15 File Offset: 0x00239F15
		private IEnumerator AdjustOptionSize()
		{
			WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
			yield return waitFrame;
			yield return waitFrame;
			LayoutElement layoutElement = this.optionContainer;
			VerticalLayoutGroup layout = this.optionRoot.GetComponent<VerticalLayoutGroup>();
			float startValue = layoutElement.preferredHeight;
			DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
			{
				layoutElement.preferredHeight = Mathf.Lerp(startValue, Mathf.Min(layout.preferredHeight, 650f), stepValue);
			}).SetAutoKill(true);
			yield break;
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x0023BD24 File Offset: 0x00239F24
		private void RestoreAllOptionCells()
		{
			EventEditorEventPreviewOptionItemStyle1Info[] childRefers = this.optionRoot.GetComponentsInTopChildren(true);
			childRefers.ForEach(delegate(int _, EventEditorEventPreviewOptionItemStyle1Info info)
			{
				PoolManager.Destroy("EventEditorOptionItemStyle_1", info.gameObject);
				return false;
			});
		}

		// Token: 0x06004BDB RID: 19419 RVA: 0x0023BD68 File Offset: 0x00239F68
		private void Reset()
		{
			this.rawImgEventTexture.DOFade(0f, 0.3f);
			this.rawImgEventTexture.texture = null;
			this.txtMeshEventContent.text = string.Empty;
			this.goEventActor.SetActive(false);
			this.RestoreAllOptionCells();
			this.mainWindow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 613f);
			base.StopAllCoroutines();
			base.StartCoroutine(this.AdjustOptionSize());
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x0023BDE4 File Offset: 0x00239FE4
		private void SetDirty()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				this._curEvent.Dirty = true;
				this._curEvent.TmEdit = Save.GetTimeStamp();
			}
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x0023BE20 File Offset: 0x0023A020
		private void OnEditContent()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("TextComponent", this.txtMeshEventContent);
				argBox.Set("MultiLine", true);
				argBox.SetObject("OnEditComplete", new Action<string>(this.OnContentEditComplete));
				string prevContent = this._curEvent.EventContent;
				bool flag2 = !string.IsNullOrEmpty(prevContent);
				if (flag2)
				{
					prevContent = prevContent.Replace("<NL>", "\n");
				}
				argBox.Set("PreContent", prevContent);
				EventEditorInput.Instance.Show(argBox);
			}
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x0023BEC0 File Offset: 0x0023A0C0
		private void OnContentEditComplete(string newContent)
		{
			string preContent = this._curEvent.EventContent;
			bool flag = preContent == newContent;
			if (!flag)
			{
				OperateCommand cmd = new OperateCommand("ChangeEventContent")
				{
					Do = delegate()
					{
						this._curEvent.EventContent = newContent;
						this.txtMeshEventContent.text = EventEditorStringCenter.DecodeTag(newContent);
					},
					Undo = delegate()
					{
						this._curEvent.EventContent = preContent;
						this.txtMeshEventContent.text = EventEditorStringCenter.DecodeTag(preContent);
					}
				};
				EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
				this.SetDirty();
			}
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x0023BF4C File Offset: 0x0023A14C
		private void OnContentRightClick()
		{
			List<SheetButtonInfo> list = new List<SheetButtonInfo>
			{
				new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_ContentCopy), delegate()
				{
					GUIUtility.systemCopyBuffer = this._curEvent.EventContent;
				}, true, "")
			};
			bool flag = !string.IsNullOrEmpty(GUIUtility.systemCopyBuffer);
			if (flag)
			{
				list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_ContentPaste), delegate()
				{
					this.OnContentEditComplete(GUIUtility.systemCopyBuffer);
				}, true, ""));
			}
			list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_ContentEdit), new Action(this.OnEditContent), true, ""));
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", list);
			UIElement.ButtonSheet.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
		}

		// Token: 0x06004BE0 RID: 19424 RVA: 0x0023C01C File Offset: 0x0023A21C
		private void OnAddOption()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				SingletonObject.getInstance<EventEditorModel>().EventAddNewOption(this._curEvent);
				this.UpdateOptions();
				this.SetDirty();
			}
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x0023C058 File Offset: 0x0023A258
		private void OnOptionClick(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				this.OnOptionSelect(optionIndex);
			}
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x0023C080 File Offset: 0x0023A280
		private void OnOptionNextEvent(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				IReadOnlyList<string> toEventList = model.GetOptionToEventList(this._curEvent, option);
				bool flag2 = toEventList == null || toEventList.Count <= 0;
				if (flag2)
				{
					EventEditorData newEvent = model.CreateNewEvent();
					bool flag3 = EventGroupTreeView.Instance.EditingEventGroup != null;
					if (flag3)
					{
						newEvent.EventGroup = EventGroupTreeView.Instance.EditingEventGroup.Key;
					}
					string eventGuid = newEvent.EventGuid;
					EventEditorData.Option eventOption = this._curEvent.Options[optionIndex];
					bool flag4 = eventOption == null;
					if (!flag4)
					{
						string optionGuid = eventOption.Guid;
						string saveDir = model.EnsureEventScriptPath(this._curEvent);
						string scriptUrl = Path.Combine(saveDir, optionGuid + ".tws");
						EventScriptEditorData script = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
						EventInstructionEditorData inst = new EventInstructionEditorData(13, new string[]
						{
							"\"" + eventGuid + "\""
						});
						inst.Args[0].IsExpression = true;
						script.Instructions.Add(inst);
						model.SaveEventScript(optionGuid, scriptUrl, script);
						EventEditorEventDetail.Instance.OnSaveEvent();
						EventEditorEventDetail.Instance.EditEvent(newEvent);
						EventEditorEventDetail.Instance.OnSaveEvent();
					}
				}
				else
				{
					this.OnOptionSelect(optionIndex);
				}
			}
		}

		// Token: 0x06004BE3 RID: 19427 RVA: 0x0023C208 File Offset: 0x0023A408
		private void OnOptionRightClick(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				List<SheetButtonInfo> list = new List<SheetButtonInfo>
				{
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Edit_Consumption), delegate()
					{
						this.OnEditOptionConsumption(optionIndex);
					}, true, ""),
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Edit_Available_Condition), delegate()
					{
						this.OnEditOptionAvailableConditions(optionIndex);
					}, true, ""),
					new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Edit_Visible_Condition), delegate()
					{
						this.OnEditOptionVisibleConditions(optionIndex);
					}, true, "")
				};
				bool flag2 = optionIndex > 1;
				if (flag2)
				{
					list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionUp), delegate()
					{
						this.OnOptionUp(optionIndex);
					}, true, ""));
				}
				Dictionary<int, EventEditorData.Option> options = this._curEvent.Options;
				bool flag3 = options.ContainsKey(optionIndex);
				if (flag3)
				{
					list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionDown), delegate()
					{
						this.OnOptionDown(optionIndex);
					}, true, ""));
					bool flag4 = !string.IsNullOrEmpty(options[optionIndex].RedirectTargetOption.Item1) && !string.IsNullOrEmpty(options[optionIndex].RedirectTargetOption.Item2);
					if (flag4)
					{
						list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionDeleteRedirect), delegate()
						{
							this.OnOptionRemoveRedirect(optionIndex);
						}, true, ""));
					}
				}
				list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionCopy), delegate()
				{
					this.OnCopyOption(optionIndex);
				}, true, ""));
				bool flag5 = EventEditorClipBoard.CopiedOptionData != null;
				if (flag5)
				{
					list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionPaste), delegate()
					{
						this.OnPasteOption(optionIndex);
					}, true, ""));
				}
				list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionDelete), delegate()
				{
					this.OnDeleteOption(optionIndex);
				}, true, ""));
				list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionAllDelete), new Action(this.OnDeleteAllOption), true, ""));
				bool flag6 = EventEditorClipBoard.CopiedOptionData != null;
				if (flag6)
				{
					list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionRedirect), delegate()
					{
						this.OnRedirectToCopiedOption(optionIndex);
					}, true, ""));
				}
				bool flag7 = EventEditorClipBoard.CopiedEvent != null;
				if (flag7)
				{
					list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_Button_OptionPasteAllAsRedirect), new Action(this.OnPasteAllEventOptionsAsRedirect), true, ""));
				}
				list.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_EditEventCode), delegate()
				{
					this.OnEditOptionCode(optionIndex);
				}, true, ""));
				List<SheetButtonInfo> clickOnceList = new List<SheetButtonInfo>
				{
					new SheetButtonInfo("是", delegate()
					{
						this.OnEditOptionOneTimeOnly(optionIndex, true);
					}, true, ""),
					new SheetButtonInfo("否", delegate()
					{
						this.OnEditOptionOneTimeOnly(optionIndex, false);
					}, true, "")
				};
				list.Add(new SheetButtonInfo("设置只可点击一次", delegate()
				{
					ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", clickOnceList);
					argBox2.SetObject("ButtonSize", new Vector2(300f, 44f));
					UIElement.ButtonSheet.SetOnInitArgs(argBox2);
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
					});
				}, true, ""));
				List<SheetButtonInfo> importantList = new List<SheetButtonInfo>
				{
					new SheetButtonInfo("是", delegate()
					{
						this.OnEditOptionImportant(optionIndex, true);
					}, true, ""),
					new SheetButtonInfo("否", delegate()
					{
						this.OnEditOptionImportant(optionIndex, false);
					}, true, "")
				};
				list.Add(new SheetButtonInfo("重要选项", delegate()
				{
					ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", importantList);
					argBox2.SetObject("ButtonSize", new Vector2(300f, 44f));
					UIElement.ButtonSheet.SetOnInitArgs(argBox2);
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
					});
				}, true, ""));
				List<SheetButtonInfo> defaultStateList = new List<SheetButtonInfo>
				{
					new SheetButtonInfo("Normal", delegate()
					{
						this.OnEditOptionDefaultState(optionIndex, OptionDefaultStateKeys.Normal);
					}, true, ""),
					new SheetButtonInfo("UnRead", delegate()
					{
						this.OnEditOptionDefaultState(optionIndex, OptionDefaultStateKeys.UnRead);
					}, true, ""),
					new SheetButtonInfo("Read", delegate()
					{
						this.OnEditOptionDefaultState(optionIndex, OptionDefaultStateKeys.Read);
					}, true, ""),
					new SheetButtonInfo("Disable", delegate()
					{
						this.OnEditOptionDefaultState(optionIndex, OptionDefaultStateKeys.Disable);
					}, true, "")
				};
				list.Add(new SheetButtonInfo("设置默认状态", delegate()
				{
					ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", defaultStateList);
					argBox2.SetObject("ButtonSize", new Vector2(300f, 44f));
					UIElement.ButtonSheet.SetOnInitArgs(argBox2);
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
					});
				}, true, ""));
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", list);
				argBox.SetObject("ButtonSize", new Vector2(300f, 44f));
				UIElement.ButtonSheet.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
			}
		}

		// Token: 0x06004BE4 RID: 19428 RVA: 0x0023C6E4 File Offset: 0x0023A8E4
		private void OnOptionDoubleClick(int optionIndex, EventEditorEventPreviewOptionItemStyle1Info infos)
		{
			EventEditorEventPreview.<>c__DisplayClass54_0 CS$<>8__locals1 = new EventEditorEventPreview.<>c__DisplayClass54_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._curEvent == null;
			if (!flag)
			{
				CS$<>8__locals1.option = this._curEvent.Options[optionIndex];
				CS$<>8__locals1.preContent = CS$<>8__locals1.option.Content;
				TextMeshProUGUI optionDesc = infos.txtMeshOptionDesc;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("TextComponent", optionDesc);
				argBox.SetObject("OnEditComplete", new Action<string>(CS$<>8__locals1.<OnOptionDoubleClick>g__OnOptionEditComplete|0));
				argBox.Set("PreContent", CS$<>8__locals1.preContent);
				EventEditorInput.Instance.Show(argBox);
			}
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x0023C788 File Offset: 0x0023A988
		private void OnOptionSelect(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				IReadOnlyList<string> toEventList = SingletonObject.getInstance<EventEditorModel>().GetOptionToEventList(this._curEvent, option);
				bool flag2 = toEventList == null;
				if (flag2)
				{
					TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_OptionHasNoLinkEvent), true);
				}
				else
				{
					bool flag3 = toEventList.Count == 1;
					if (flag3)
					{
						this.OnInternalJumpToEvent(toEventList[0]);
					}
					else
					{
						bool flag4 = toEventList.Count > 1;
						if (flag4)
						{
							EventEditorSelectJumpEvent.Instance.Show(toEventList);
						}
						else
						{
							TaskControlPanel.Instance.ShowTips(LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_OptionHasNoLinkEvent), true);
						}
					}
				}
			}
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x0023C844 File Offset: 0x0023AA44
		private void OnInternalJumpToEvent(string eventGuid)
		{
			bool flag = this._curEvent != null;
			if (flag)
			{
				bool flag2 = this._stackIndex < this._jumpStack.Count;
				if (flag2)
				{
					this._jumpStack.RemoveRange(this._stackIndex, this._jumpStack.Count - this._stackIndex);
				}
				this._jumpStack.Add(this._curEvent.EventGuid);
				this._stackIndex = this._jumpStack.Count;
			}
			EventEditorData eventData = SingletonObject.getInstance<EventEditorModel>().GetEvent(eventGuid);
			bool flag3 = eventData != null;
			if (flag3)
			{
				EventEditorEventList.Instance.Select(eventGuid);
				EventEditorEventDetail.Instance.EditEvent(eventData);
			}
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x0023C8F4 File Offset: 0x0023AAF4
		private void OnCopyOption(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorClipBoard.GetOptionCopyData(this._curEvent, optionIndex, true);
				string eventName = this._curEvent.EventName;
				bool flag2 = !string.IsNullOrEmpty(eventName);
				if (flag2)
				{
					EventEditorData.Option option = this._curEvent.Options[optionIndex];
					GUIUtility.systemCopyBuffer = "\"" + option.Guid + "\"";
					string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_CopyOption, eventName, option.Content);
					EventEditorNotes.Instance.AddNote(info);
				}
			}
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x0023C988 File Offset: 0x0023AB88
		private void OnPasteOption(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorClipBoard.EventOptionCopyData preCopiedOptionData = EventEditorClipBoard.GetOptionCopyData(this._curEvent, optionIndex, false);
				EventEditorClipBoard.EventOptionCopyData newCopiedOptionData = EventEditorClipBoard.CopiedOptionData;
				OperateCommand cmd = new OperateCommand("PasteOption")
				{
					Do = delegate()
					{
						EventEditorClipBoard.CopiedOptionData = newCopiedOptionData;
						EventEditorClipBoard.PasteOption(this._curEvent, optionIndex);
						this.UpdateOptions();
					},
					Undo = delegate()
					{
						EventEditorClipBoard.CopiedOptionData = preCopiedOptionData;
						EventEditorClipBoard.PasteOption(this._curEvent, optionIndex);
						this.UpdateOptions();
					}
				};
				EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
				this.SetDirty();
				string eventName = this._curEvent.EventName;
				bool flag2 = !string.IsNullOrEmpty(eventName);
				if (flag2)
				{
					string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_PasteOption, eventName, optionIndex);
					EventEditorNotes.Instance.AddNote(info);
				}
			}
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x0023CA64 File Offset: 0x0023AC64
		private void OnRedirectToCopiedOption(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorClipBoard.EventOptionCopyData newCopiedOptionData = EventEditorClipBoard.CopiedOptionData;
				EventEditorClipBoard.EventOptionCopyData preCopiedOptionData = EventEditorClipBoard.GetOptionCopyData(this._curEvent, optionIndex, false);
				OperateCommand cmd = new OperateCommand("RedirectToCopiedOption")
				{
					Do = delegate()
					{
						EventEditorClipBoard.CopiedOptionData = newCopiedOptionData;
						EventEditorClipBoard.RedirectToCopiedOption(this._curEvent, optionIndex);
						this.UpdateOptions();
					},
					Undo = delegate()
					{
						EventEditorClipBoard.CopiedOptionData = preCopiedOptionData;
						EventEditorClipBoard.PasteOption(this._curEvent, optionIndex);
						this.UpdateOptions();
					}
				};
				EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
				this.SetDirty();
				string eventName = this._curEvent.EventName;
				bool flag2 = !string.IsNullOrEmpty(eventName);
				if (flag2)
				{
					string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_RedirectToCopiedOption, eventName, optionIndex);
					EventEditorNotes.Instance.AddNote(info);
				}
			}
		}

		// Token: 0x06004BEA RID: 19434 RVA: 0x0023CB40 File Offset: 0x0023AD40
		private void OnOptionRemoveRedirect(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				ValueTuple<string, string> redirect = this._curEvent.Options[optionIndex].RedirectTargetOption;
				OperateCommand cmd = new OperateCommand("OptionRemoveRedirect")
				{
					Do = delegate()
					{
						this._curEvent.Options[optionIndex].RedirectTargetOption = new ValueTuple<string, string>(string.Empty, string.Empty);
						this.UpdateOptions();
					},
					Undo = delegate()
					{
						this._curEvent.Options[optionIndex].RedirectTargetOption = redirect;
						this.UpdateOptions();
					}
				};
				EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
				this.SetDirty();
				string eventName = this._curEvent.EventName;
				bool flag2 = !string.IsNullOrEmpty(eventName);
				if (flag2)
				{
					string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Button_OptionDeleteRedirect, eventName, optionIndex);
					EventEditorNotes.Instance.AddNote(info);
				}
			}
		}

		// Token: 0x06004BEB RID: 19435 RVA: 0x0023CC1C File Offset: 0x0023AE1C
		private void OnPasteAllEventOptionsAsRedirect()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData srcData = EventEditorClipBoard.CopiedEvent;
				bool flag2 = srcData.EventGuid == this._curEvent.EventGuid;
				if (!flag2)
				{
					EventEditorData newCopiedEventData = EventEditorClipBoard.CopiedEvent;
					EventEditorData preCopiedEventData = this._curEvent.Duplicate();
					OperateCommand cmd = new OperateCommand("PasteAllEventOptionsAsRedirect")
					{
						Do = delegate()
						{
							EventEditorClipBoard.CopiedEvent = newCopiedEventData;
							EventEditorClipBoard.PasteAllEventOptionsAsRedirect(this._curEvent);
							this.Refresh(this._curEvent);
						},
						Undo = delegate()
						{
							EventEditorClipBoard.CopiedEvent = preCopiedEventData;
							EventEditorClipBoard.PasteEvent(this._curEvent);
							EventEditorClipBoard.CopiedEvent = preCopiedEventData;
							this.Refresh(this._curEvent);
						}
					};
					EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
					this.SetDirty();
					string eventName = preCopiedEventData.EventName;
					bool flag3 = !string.IsNullOrEmpty(eventName);
					if (flag3)
					{
						string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_PasteAllEventOptionsAsRedirect, eventName);
						EventEditorNotes.Instance.AddNote(info);
					}
				}
			}
		}

		// Token: 0x06004BEC RID: 19436 RVA: 0x0023CD08 File Offset: 0x0023AF08
		private void OnOptionOrderChange(int fromIndex, int toIndex)
		{
			Dictionary<int, EventEditorData.Option> options = this._curEvent.Options;
			bool flag = fromIndex < 1 || fromIndex > options.Count;
			if (!flag)
			{
				bool flag2 = toIndex < 1 || toIndex > options.Count;
				if (!flag2)
				{
					EventEditorData.Option srcOption = options[fromIndex];
					EventEditorData.Option targetOption = options[toIndex];
					OperateCommand cmd = new OperateCommand("ChangeOptionOrder")
					{
						Do = delegate()
						{
							options[fromIndex] = targetOption;
							options[toIndex] = srcOption;
							this.UpdateOptions();
						},
						Undo = delegate()
						{
							options[fromIndex] = srcOption;
							options[toIndex] = targetOption;
							this.UpdateOptions();
						}
					};
					EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BED RID: 19437 RVA: 0x0023CE00 File Offset: 0x0023B000
		private void OnOptionUp(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				this.OnOptionOrderChange(optionIndex, optionIndex - 1);
			}
		}

		// Token: 0x06004BEE RID: 19438 RVA: 0x0023CE28 File Offset: 0x0023B028
		private void OnOptionDown(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				this.OnOptionOrderChange(optionIndex, optionIndex + 1);
			}
		}

		// Token: 0x06004BEF RID: 19439 RVA: 0x0023CE50 File Offset: 0x0023B050
		private void OnDeleteOption(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				Dictionary<int, EventEditorData.Option> options = this._curEvent.Options;
				EventEditorData.Option optionTable = options[optionIndex];
				DialogCmd dialogCmd = new DialogCmd
				{
					Type = 1,
					Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
					Content = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_OptionDeleteConfirm, optionIndex).ColorReplace(),
					Yes = delegate()
					{
						for (int i = optionIndex; i < options.Count; i++)
						{
							options[i] = options[i + 1];
						}
						options.Remove(options.Count);
						string optionGuid = optionTable.Guid;
						string saveDir = Save.GetEventSaveDir(this._curEvent);
						string csCodeUrl = Path.Combine(saveDir, optionGuid + ".cs");
						bool flag2 = File.Exists(csCodeUrl);
						if (flag2)
						{
							File.Delete(csCodeUrl);
						}
						string optionScriptUrl = Path.Combine(saveDir, optionGuid + ".tws");
						bool flag3 = File.Exists(optionScriptUrl);
						if (flag3)
						{
							File.Delete(optionScriptUrl);
						}
						string visibleConditionUrl = Path.Combine(saveDir, optionGuid + "_visible.tws");
						bool flag4 = File.Exists(visibleConditionUrl);
						if (flag4)
						{
							File.Delete(visibleConditionUrl);
						}
						string availableConditionUrl = Path.Combine(saveDir, optionGuid + "_available.tws");
						bool flag5 = File.Exists(availableConditionUrl);
						if (flag5)
						{
							File.Delete(availableConditionUrl);
						}
						this.UpdateOptions();
						EventEditorEventDetail.Instance.OnSaveEvent();
						this.SetDirty();
						string eventName = this._curEvent.EventName;
						bool flag6 = !string.IsNullOrEmpty(eventName);
						if (flag6)
						{
							string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_DeleteOption, eventName, optionIndex);
							EventEditorNotes.Instance.AddNote(info);
						}
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x0023CF28 File Offset: 0x0023B128
		private void OnDeleteAllOption()
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				Dictionary<int, EventEditorData.Option> options = this._curEvent.Options;
				DialogCmd dialogCmd = new DialogCmd
				{
					Type = 1,
					Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
					Content = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_OptionDeleteConfirm, " All").ColorReplace(),
					Yes = delegate()
					{
						for (int i = options.Count; i >= 1; i--)
						{
							EventEditorData.Option optionTable = options[i];
							string optionGuid = optionTable.Guid;
							string scriptUrl = Path.Combine(Save.GetEventSaveDir(this._curEvent), optionGuid + ".cs");
							bool flag2 = !string.IsNullOrEmpty(scriptUrl) && File.Exists(scriptUrl);
							if (flag2)
							{
								File.Delete(scriptUrl);
							}
							options.Remove(i);
						}
						this.UpdateOptions();
						EventEditorEventDetail.Instance.OnSaveEvent();
						this.SetDirty();
						string eventName = this._curEvent.EventName;
						bool flag3 = !string.IsNullOrEmpty(eventName);
						if (flag3)
						{
							string info = LocalStringManager.GetFormat(LanguageKey.UI_EventEditor_Tip_DeleteOption, eventName, " All");
							EventEditorNotes.Instance.AddNote(info);
						}
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06004BF1 RID: 19441 RVA: 0x0023CFDC File Offset: 0x0023B1DC
		private void OnDeleteEvent(ArgumentBox box)
		{
			string guid;
			bool flag = this._curEvent != null && box.Get("Guid", out guid);
			if (flag)
			{
				string curEventGuid = this._curEvent.EventGuid;
				bool flag2 = curEventGuid == guid;
				if (flag2)
				{
					this.Refresh(null);
				}
			}
		}

		// Token: 0x06004BF2 RID: 19442 RVA: 0x0023D02C File Offset: 0x0023B22C
		private void OnEditOptionScript(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
					string optionGuid = option.Guid;
					string saveDir = model.EnsureEventScriptPath(this._curEvent);
					bool flag3 = !Directory.Exists(saveDir);
					if (!flag3)
					{
						string scriptUrl = Path.Combine(saveDir, optionGuid + ".tws");
						EventScriptEditorData scriptData = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
						EventEditorScript.Instance.Show(scriptData, delegate(EventScriptEditorData data)
						{
							model.EnsureEventScriptPath(this._curEvent);
							model.SaveEventScript(optionGuid, scriptUrl, data);
							this.UpdateOptions();
						}, 3, this._curEvent.EventGuid, optionGuid);
					}
				}
			}
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x0023D124 File Offset: 0x0023B324
		private void OnEditOptionAvailableConditions(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
					string optionGuid = option.Guid;
					string saveDir = model.EnsureEventScriptPath(this._curEvent);
					bool flag3 = !Directory.Exists(saveDir);
					if (!flag3)
					{
						string scriptUrl = Path.Combine(saveDir, optionGuid + "_available.tws");
						EventScriptEditorData scriptData = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
						EventEditorScript.Instance.Show(scriptData, delegate(EventScriptEditorData data)
						{
							model.EnsureEventScriptPath(this._curEvent);
							model.SaveEventScript(optionGuid, scriptUrl, data);
							this.UpdateOptions();
						}, 4, this._curEvent.EventGuid, optionGuid);
					}
				}
			}
		}

		// Token: 0x06004BF4 RID: 19444 RVA: 0x0023D21C File Offset: 0x0023B41C
		private void OnEditOptionVisibleConditions(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
					string optionGuid = option.Guid;
					string saveDir = model.EnsureEventScriptPath(this._curEvent);
					bool flag3 = !Directory.Exists(saveDir);
					if (!flag3)
					{
						string scriptUrl = Path.Combine(saveDir, optionGuid + "_visible.tws");
						EventScriptEditorData scriptData = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<EventScriptEditorData>(File.ReadAllText(scriptUrl)) : new EventScriptEditorData();
						EventEditorScript.Instance.Show(scriptData, delegate(EventScriptEditorData data)
						{
							model.EnsureEventScriptPath(this._curEvent);
							model.SaveEventScript(optionGuid, scriptUrl, data);
							this.UpdateOptions();
						}, 5, this._curEvent.EventGuid, optionGuid);
					}
				}
			}
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x0023D314 File Offset: 0x0023B514
		private void OnEditOptionConsumption(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
					string optionGuid = option.Guid;
					string saveDir = model.EnsureEventScriptPath(this._curEvent);
					bool flag3 = !Directory.Exists(saveDir);
					if (!flag3)
					{
						string scriptUrl = Path.Combine(saveDir, optionGuid + "_cost.twe");
						List<EventOptionCost> optionCosts = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<List<EventOptionCost>>(File.ReadAllText(scriptUrl)) : null;
						OptionConsumeEditor.Instance.Show(optionCosts, delegate(List<EventOptionCost> data)
						{
							model.EnsureEventScriptPath(this._curEvent);
							bool flag4 = data.Count == 0;
							if (flag4)
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
		}

		// Token: 0x06004BF6 RID: 19446 RVA: 0x0023D3EC File Offset: 0x0023B5EC
		private void OnEditOptionCode(int optionIndex)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
					string optionGuid = option.Guid;
					string saveDir = model.EnsureEventScriptPath(this._curEvent);
					bool flag3 = !Directory.Exists(saveDir);
					if (!flag3)
					{
						string scriptUrl = Path.Combine(saveDir, optionGuid + ".cs");
						bool flag4 = !File.Exists(scriptUrl);
						if (flag4)
						{
							model.GenerateOptionCodeFile(this._curEvent, optionGuid);
						}
						UI_EventEditor.OpenCSharpCodeFile(scriptUrl);
						this.UpdateOptions();
					}
				}
			}
		}

		// Token: 0x06004BF7 RID: 19447 RVA: 0x0023D498 File Offset: 0x0023B698
		private void OnEditOptionOneTimeOnly(int optionIndex, bool oneTimeOnly)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					bool preVale = option.OneTimeOnly;
					bool flag3 = preVale == oneTimeOnly;
					if (flag3)
					{
						this.UpdateOptions();
					}
					else
					{
						OperateCommand cmd = new OperateCommand("ChangeOptionOneTimeOnly")
						{
							Do = delegate()
							{
								option.OneTimeOnly = oneTimeOnly;
								this.UpdateOptions();
							},
							Undo = delegate()
							{
								option.OneTimeOnly = preVale;
								this.UpdateOptions();
							}
						};
						EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
						this.SetDirty();
					}
				}
			}
		}

		// Token: 0x06004BF8 RID: 19448 RVA: 0x0023D568 File Offset: 0x0023B768
		private void OnEditOptionImportant(int optionIndex, bool important)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					bool preVale = option.Important;
					bool flag3 = preVale == important;
					if (flag3)
					{
						this.UpdateOptions();
					}
					else
					{
						OperateCommand cmd = new OperateCommand("ChangeOptionImportant")
						{
							Do = delegate()
							{
								option.Important = important;
								this.UpdateOptions();
							},
							Undo = delegate()
							{
								option.Important = preVale;
								this.UpdateOptions();
							}
						};
						EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
						this.SetDirty();
					}
				}
			}
		}

		// Token: 0x06004BF9 RID: 19449 RVA: 0x0023D638 File Offset: 0x0023B838
		private void OnEditOptionDefaultState(int optionIndex, string state)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				EventEditorData.Option option = this._curEvent.Options[optionIndex];
				bool flag2 = option == null;
				if (!flag2)
				{
					string preValue = OptionDefaultStateKeys.Normal;
					bool flag3 = !string.IsNullOrEmpty(option.DefaultState);
					if (flag3)
					{
						preValue = option.DefaultState;
						bool flag4 = preValue.Equals(state);
						if (flag4)
						{
							this.UpdateOptions();
							return;
						}
					}
					OperateCommand cmd = new OperateCommand("ChangeOptionDefaultState")
					{
						Do = delegate()
						{
							option.DefaultState = state;
							this.UpdateOptions();
						},
						Undo = delegate()
						{
							option.DefaultState = preValue;
							this.UpdateOptions();
						}
					};
					EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
					this.SetDirty();
				}
			}
		}

		// Token: 0x06004BFA RID: 19450 RVA: 0x0023D730 File Offset: 0x0023B930
		private void OnOptionBehaviorDropdownValueChanged(int optionIndex, int newValue, CDropdown dropdown)
		{
			bool flag = this._curEvent == null;
			if (!flag)
			{
				Dictionary<int, EventEditorData.Option> options = this._curEvent.Options;
				EventEditorData.Option targetOption = options[optionIndex];
				bool flag2 = targetOption == null;
				if (!flag2)
				{
					int preValue = targetOption.Behavior;
					bool flag3 = preValue == newValue;
					if (!flag3)
					{
						string optionKey = targetOption.OptionKey;
						OperateCommand cmd = new OperateCommand("ChangeOptionBehavior_" + optionKey)
						{
							Do = delegate()
							{
								targetOption.Behavior = newValue;
								dropdown.value = newValue;
							},
							Undo = delegate()
							{
								bool flag4 = -1 == preValue;
								if (!flag4)
								{
									targetOption.Behavior = preValue;
									dropdown.value = preValue;
								}
							}
						};
						EventEditorEventDetail.Instance.ExecuteOperateCommand(cmd);
						this.SetDirty();
					}
				}
			}
		}

		// Token: 0x06004BFB RID: 19451 RVA: 0x0023D80C File Offset: 0x0023BA0C
		private void UpdateOptionBehaviorShowState()
		{
			EventEditorEventPreviewOptionItemStyle1Info[] optionRefers = this.optionRoot.GetComponentsInTopChildren(false);
			foreach (EventEditorEventPreviewOptionItemStyle1Info optionRefer in optionRefers)
			{
				bool showState = this._optionBehaviorShowState;
				CDropdown dropdown = optionRefer.behaviorDropdown;
				bool flag = dropdown.transform.Find("Dropdown List");
				if (flag)
				{
					showState = false;
				}
				optionRefer.goControlLayer.SetActive(showState);
			}
		}

		// Token: 0x06004BFC RID: 19452 RVA: 0x0023D87C File Offset: 0x0023BA7C
		private void OnStackBackEvent()
		{
			bool flag = this._curEvent != null && this._stackIndex == this._jumpStack.Count && this._jumpStack.Count > 0;
			if (flag)
			{
				string curEventGuid = this._curEvent.EventGuid;
				bool flag2 = this._jumpStack[this._stackIndex - 1] != curEventGuid;
				if (flag2)
				{
					this._jumpStack.Add(curEventGuid);
				}
			}
			bool flag3 = this._stackIndex > 0;
			if (flag3)
			{
				List<string> jumpStack = this._jumpStack;
				int num = this._stackIndex - 1;
				this._stackIndex = num;
				string eventGuid = jumpStack[num];
				EventEditorEventList.Instance.Select(eventGuid);
			}
		}

		// Token: 0x06004BFD RID: 19453 RVA: 0x0023D930 File Offset: 0x0023BB30
		private void OnStackForwardEvent()
		{
			bool flag = this._stackIndex < this._jumpStack.Count - 1;
			if (flag)
			{
				List<string> jumpStack = this._jumpStack;
				int num = this._stackIndex + 1;
				this._stackIndex = num;
				string eventGuid = jumpStack[num];
				EventEditorEventList.Instance.Select(eventGuid);
			}
		}

		// Token: 0x040034AB RID: 13483
		public static EventEditorEventPreview Instance;

		// Token: 0x040034AC RID: 13484
		[SerializeField]
		private RectTransform mainWindow;

		// Token: 0x040034AD RID: 13485
		[SerializeField]
		private CRawImage rawImgEventTexture;

		// Token: 0x040034AE RID: 13486
		[SerializeField]
		private RectTransform optionRoot;

		// Token: 0x040034AF RID: 13487
		[SerializeField]
		private GameObject goOptionItemStyle1;

		// Token: 0x040034B0 RID: 13488
		[SerializeField]
		private LayoutElement optionContainer;

		// Token: 0x040034B1 RID: 13489
		[SerializeField]
		private TextMeshProUGUI txtMeshEventContent;

		// Token: 0x040034B2 RID: 13490
		[SerializeField]
		private GameObject goEventActor;

		// Token: 0x040034B3 RID: 13491
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040034B4 RID: 13492
		[SerializeField]
		private TextMeshProUGUI txtMeshPartGoodValue;

		// Token: 0x040034B5 RID: 13493
		[SerializeField]
		private CImage imgPartBar;

		// Token: 0x040034B6 RID: 13494
		[SerializeField]
		private CImage imgActorFavorBar1;

		// Token: 0x040034B7 RID: 13495
		[SerializeField]
		private CImage imgActorFavorBar2;

		// Token: 0x040034B8 RID: 13496
		[SerializeField]
		private CImage imgWarinessBar;

		// Token: 0x040034B9 RID: 13497
		[SerializeField]
		private TextMeshProUGUI txtMeshActorGeneration;

		// Token: 0x040034BA RID: 13498
		[SerializeField]
		private TextMeshProUGUI txtMeshActorName;

		// Token: 0x040034BB RID: 13499
		[SerializeField]
		private PointClickBridge eventContentContainer;

		// Token: 0x040034BC RID: 13500
		[SerializeField]
		private CButton btnAddOption;

		// Token: 0x040034BD RID: 13501
		[SerializeField]
		private CButton btnBack;

		// Token: 0x040034BE RID: 13502
		[SerializeField]
		private CButton btnForward;

		// Token: 0x040034BF RID: 13503
		[SerializeField]
		private CToggleGroup optionToggleGroup;

		// Token: 0x040034C0 RID: 13504
		private EventEditorData _curEvent;

		// Token: 0x040034C1 RID: 13505
		private bool _optionBehaviorShowState;

		// Token: 0x040034C2 RID: 13506
		private const string OptionItemStyle1 = "EventEditorOptionItemStyle_1";

		// Token: 0x040034C3 RID: 13507
		private List<string> _optionBehaviorDropdownOptions;

		// Token: 0x040034C4 RID: 13508
		private int _stackIndex;

		// Token: 0x040034C5 RID: 13509
		private List<string> _jumpStack;

		// Token: 0x040034C6 RID: 13510
		private List<string> _adaptableTextureNames;

		// Token: 0x040034C7 RID: 13511
		private NameKeyTextureGroup _nameKeyTextureGroup;
	}
}
