using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using EventEditor;
using EventEditor.EventScript;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Utilities;
using Newtonsoft.Json;
using Property;
using SubEditor;
using SubEditor.AdventureCommonRefersListEditor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class AdventureEditorElementInfo : MonoBehaviour
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x060014E8 RID: 5352 RVA: 0x0008171D File Offset: 0x0007F91D
	internal FileInfo EditingFile
	{
		get
		{
			return this._editingFile;
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x00081728 File Offset: 0x0007F928
	private void OnEnable()
	{
		this.Refresh();
		bool flag = this.topLayerAnchor;
		if (flag)
		{
			AdventureEditorKit.FixLayerSortingOrder(base.gameObject, this.topLayerAnchor);
		}
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x00081760 File Offset: 0x0007F960
	private void OnDisable()
	{
		bool flag = this.topLayerAnchor;
		if (flag)
		{
			AdventureEditorKit.RestoreLayerSortingOrder(base.gameObject);
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0008178B File Offset: 0x0007F98B
	public void Set(FileInfo file)
	{
		this._editingFile = file;
		this._editingDirectory = null;
		this.Refresh();
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x000817A3 File Offset: 0x0007F9A3
	public void Set(DirectoryInfo directory)
	{
		this._editingFile = null;
		this._editingDirectory = directory;
		this.Refresh();
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000817BB File Offset: 0x0007F9BB
	internal bool HasEditingTarget()
	{
		return this._editingFile != null || this._editingDirectory != null;
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000817D4 File Offset: 0x0007F9D4
	internal bool IsEditingTarget(FileInfo file, DirectoryInfo directory)
	{
		bool flag;
		if (file != null && file.Exists)
		{
			FileInfo editingFile = this._editingFile;
			flag = (editingFile != null && editingFile.Exists);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		bool result;
		if (flag2)
		{
			result = (this._editingFile.FullName == file.FullName);
		}
		else
		{
			bool flag3;
			if (directory != null && directory.Exists)
			{
				DirectoryInfo editingDirectory = this._editingDirectory;
				flag3 = (editingDirectory != null && editingDirectory.Exists);
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			result = (flag4 && this._editingDirectory.FullName == directory.FullName);
		}
		return result;
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x00081868 File Offset: 0x0007FA68
	public void Clear()
	{
		bool flag = this._editingFile == null && this._editingDirectory == null;
		if (!flag)
		{
			this._editingFile = null;
			this._editingDirectory = null;
			this.Refresh();
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000818A8 File Offset: 0x0007FAA8
	public void Delete()
	{
		FileInfo editingFile = this._editingFile;
		bool flag = editingFile != null && editingFile.Exists;
		if (flag)
		{
			File.Delete(this._editingFile.FullName);
		}
		DirectoryInfo editingDirectory = this._editingDirectory;
		bool flag2 = editingDirectory != null && editingDirectory.Exists;
		if (flag2)
		{
			Directory.Delete(this._editingDirectory.FullName, true);
		}
		UnityEvent<int> unityEvent = this.onElementContainerRefresh;
		if (unityEvent != null)
		{
			unityEvent.Invoke(-1);
		}
		this.Clear();
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x00081924 File Offset: 0x0007FB24
	public bool CheckModified()
	{
		FileInfo editingFile = this._editingFile;
		AdventureElementSnapshot data;
		bool flag = editingFile != null && editingFile.Exists && AdventureElementSnapshot.TryLoadFromFile(this._editingFile.FullName, out data);
		bool result;
		if (flag)
		{
			string fileName;
			AdventureElementSnapshot newData = this.CreateEditingElement(out fileName);
			result = (fileName != this._editingFile.Name || !newData.SameOf(data));
		}
		else
		{
			DirectoryInfo editingDirectory = this._editingDirectory;
			bool flag2 = editingDirectory != null && editingDirectory.Exists;
			result = (!flag2 || this._editingDirectory.Name != this.pathInput.text);
		}
		return result;
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x000819D0 File Offset: 0x0007FBD0
	public void Save()
	{
		bool flag = !this.CheckModified();
		if (!flag)
		{
			FileInfo editingFile = this._editingFile;
			AdventureElementSnapshot data;
			bool flag2 = editingFile != null && editingFile.Exists && AdventureElementSnapshot.TryLoadFromFile(this._editingFile.FullName, out data);
			if (flag2)
			{
				File.Delete(this._editingFile.FullName);
				string fileName;
				AdventureElementSnapshot newData = this.CreateEditingElement(out fileName);
				newData.Id = data.Id;
				string newPath = this._editingFile.FullName.ReplaceLast(this._editingFile.Name, fileName);
				AdventureElementSnapshot.SaveToFile(newPath, newData);
				this._editingFile = new FileInfo(newPath);
				GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusSaved, new ArgumentBox().Set("Path", newPath));
			}
			else
			{
				DirectoryInfo editingDirectory = this._editingDirectory;
				bool flag3 = editingDirectory != null && editingDirectory.Exists;
				if (flag3)
				{
					string newPath2 = this.pathInput.text;
					foreach (char invalidChar in Path.GetInvalidPathChars())
					{
						newPath2 = newPath2.Replace(invalidChar.ToString(), "");
					}
					newPath2 = this._editingDirectory.FullName.ReplaceLast(this._editingDirectory.Name, newPath2);
					bool flag4 = !Directory.Exists(newPath2);
					if (flag4)
					{
						this._editingDirectory.MoveTo(newPath2);
					}
					this.pathInput.text = this._editingDirectory.Name;
				}
				else
				{
					this.Clear();
				}
			}
			UnityEvent<int> unityEvent = this.onElementContainerRefresh;
			if (unityEvent != null)
			{
				unityEvent.Invoke(-1);
			}
		}
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x00081B80 File Offset: 0x0007FD80
	public void Duplicate()
	{
		FileInfo editingFile = this._editingFile;
		AdventureElementSnapshot data;
		bool flag = editingFile != null && editingFile.Exists && AdventureElementSnapshot.TryLoadFromFile(this._editingFile.FullName, out data);
		if (flag)
		{
			string f = AdventureEditorKit.GetNewElementFilePath(this._editingFile.DirectoryName);
			int id = AdventureEditorKit.GetNewElementId();
			data.Id = id;
			AdventureElementSnapshot.SaveToFile(f, data);
			UnityEvent<int> unityEvent = this.onElementContainerRefresh;
			if (unityEvent != null)
			{
				unityEvent.Invoke(id);
			}
			GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusSaved, new ArgumentBox().Set("Path", f));
			DialogCmd dialog = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Copy),
				Content = f,
				Type = 2
			};
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog);
			UIElement.Dialog.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x00081C70 File Offset: 0x0007FE70
	private void RefreshEditData(AdventureElementSnapshot data)
	{
		AdventureEditorElementInfo.<>c__DisplayClass53_0 CS$<>8__locals1 = new AdventureEditorElementInfo.<>c__DisplayClass53_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.data = data;
		this._elementFieldEditInfo.Clear();
		CS$<>8__locals1.<RefreshEditData>g__ValueString|26(EAdventureElementOverrideType.Name, this.nameInput, delegate(AdventureElementSnapshot d)
		{
			CS$<>8__locals1.<>4__this.warning.gameObject.SetActive(d == null);
			bool flag = d == null;
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Unknow);
			}
			else
			{
				result = d.Name;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, string v)
		{
			d.Name = v;
			UnityEvent<int> unityEvent = CS$<>8__locals1.<>4__this.onElementContainerRefresh;
			if (unityEvent != null)
			{
				unityEvent.Invoke(-1);
			}
		});
		this.definitionInput.onEndEdit.ResetListener(delegate(string d)
		{
			CS$<>8__locals1.data.Definition = d;
			CS$<>8__locals1.<>4__this.definitionInput.SetTextWithoutNotify(d);
		});
		this.definitionInput.SetTextWithoutNotify(CS$<>8__locals1.data.Definition);
		CS$<>8__locals1.<RefreshEditData>g__ValueString|26(EAdventureElementOverrideType.Icon, this.iconInput, delegate(AdventureElementSnapshot d)
		{
			bool flag = d == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = d.Icon;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, string v)
		{
			d.Icon = v;
		});
		CS$<>8__locals1.<RefreshEditData>g__ValueString|26(EAdventureElementOverrideType.Tags, this.tagInput, delegate(AdventureElementSnapshot d)
		{
			bool flag = d == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string tags = "";
				for (int i = 0; i < d.Tags.Count; i++)
				{
					string tagName = d.Tags[i];
					tags += tagName;
					bool flag2 = i != d.Tags.Count - 1;
					if (flag2)
					{
						tags += ",";
					}
				}
				result = tags;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, string v)
		{
			d.Tags.Clear();
			bool flag = v.IsNullOrEmpty();
			if (!flag)
			{
				List<string> tags = v.Split(",", StringSplitOptions.None).ToList<string>();
				foreach (string tagName in tags)
				{
					string tagValid = tagName.Trim();
					bool flag2 = !d.Tags.Contains(tagValid) && !tagValid.IsNullOrEmpty();
					if (flag2)
					{
						d.Tags.Add(tagValid);
					}
				}
			}
		});
		CS$<>8__locals1.<RefreshEditData>g__ValueString|26(EAdventureElementOverrideType.Desc, this.inputFieldDesc, delegate(AdventureElementSnapshot d)
		{
			bool flag = d == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = d.Desc;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, string v)
		{
			d.Desc = v;
		});
		CS$<>8__locals1.<RefreshEditData>g__ValueEnum|23<EAdventureElementCreatingType>(EAdventureElementOverrideType.CreatingType, this.dropdownCreateType, delegate(AdventureElementSnapshot d)
		{
			bool flag = d == null;
			EAdventureElementCreatingType result;
			if (flag)
			{
				result = EAdventureElementCreatingType.RandomInBlock;
			}
			else
			{
				result = d.CreatingType;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, EAdventureElementCreatingType v)
		{
			d.CreatingType = v;
		}, "LK_AdventureEditor_AdventureElementCreatingType_{0}");
		CS$<>8__locals1.<RefreshEditData>g__ValueInt|24(EAdventureElementOverrideType.TimeCost, this.inputFieldCost, delegate(AdventureElementSnapshot d)
		{
			bool flag = d == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = d.TimeCost;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, int v)
		{
			d.TimeCost = v;
		});
		CS$<>8__locals1.<RefreshEditData>g__ValueInt|24(EAdventureElementOverrideType.VisiblePriority, this.inputFieldVisiblePriority, delegate(AdventureElementSnapshot d)
		{
			bool flag = d == null;
			int result;
			if (flag)
			{
				result = int.MaxValue;
			}
			else
			{
				result = d.VisiblePriority;
			}
			return result;
		}, delegate(AdventureElementSnapshot d, int v)
		{
			d.VisiblePriority = v;
		});
		this.visibleIgnoreSortingToggle.SetIsOnWithoutNotify(CS$<>8__locals1.data.VisibleIgnoreSorting);
		this.visibleIgnoreSortingToggle.onValueChanged.ResetListener(delegate(bool ison)
		{
			CS$<>8__locals1.data.VisibleIgnoreSorting = ison;
		});
		this.inputFieldComment.onValueChanged.ResetListener(delegate(string v)
		{
			CS$<>8__locals1.data.Comment = v;
		});
		this.inputFieldComment.text = CS$<>8__locals1.data.Comment;
		this.buttonCharacterData.onClick.ResetListener(delegate()
		{
			CS$<>8__locals1.<>4__this.subEditorCharacterData.OriginalEditingOwner = CS$<>8__locals1.data;
			CS$<>8__locals1.<>4__this.subEditorCharacterData.OverviewBlocks = null;
			AdventureCharacterDataEditor adventureCharacterDataEditor = CS$<>8__locals1.<>4__this.subEditorCharacterData;
			Func<short> editCharacterIdGetter;
			if ((editCharacterIdGetter = CS$<>8__locals1.<>9__31) == null)
			{
				editCharacterIdGetter = (CS$<>8__locals1.<>9__31 = (() => CS$<>8__locals1.data.CharacterId));
			}
			adventureCharacterDataEditor.EditCharacterIdGetter = editCharacterIdGetter;
			AdventureCharacterDataEditor adventureCharacterDataEditor2 = CS$<>8__locals1.<>4__this.subEditorCharacterData;
			Action<short> editCharacterIdSetter;
			if ((editCharacterIdSetter = CS$<>8__locals1.<>9__32) == null)
			{
				editCharacterIdSetter = (CS$<>8__locals1.<>9__32 = delegate(short v)
				{
					CS$<>8__locals1.data.CharacterId = v;
				});
			}
			adventureCharacterDataEditor2.EditCharacterIdSetter = editCharacterIdSetter;
			CS$<>8__locals1.<>4__this.subEditorCharacterData.SetEditableList(new List<AdventureCharacterData>
			{
				CS$<>8__locals1.data.CharacterData ?? AdventureCharacterDataEditor.CreateEmptyCharacterData()
			});
			base.<RefreshEditData>g__SetupSubEditorOpen|30(CS$<>8__locals1.<>4__this.subEditorCharacterData.gameObject);
		});
		this.buttonMoveRule.onClick.ResetListener(delegate()
		{
			CS$<>8__locals1.<>4__this.subEditorMoveData.Creator = ((IList<AdventureElementMoveSnapshot> _) => new AdventureElementMoveSnapshot());
			AdventureCommonRefersListEditor<AdventureElementMoveSnapshot> adventureCommonRefersListEditor = CS$<>8__locals1.<>4__this.subEditorMoveData;
			Action<IList<AdventureElementMoveSnapshot>, MonoBehaviour, int, Action> refreshAction;
			if ((refreshAction = CS$<>8__locals1.<>9__34) == null)
			{
				refreshAction = (CS$<>8__locals1.<>9__34 = delegate(IList<AdventureElementMoveSnapshot> subList, MonoBehaviour subUnit, int subIndex, Action subOnUpdate)
				{
					AdventureElementMoveSnapshot sub = subList[subIndex];
					AdventureElementMoveRuleEditorTemplate template = subUnit.GetComponent<AdventureElementMoveRuleEditorTemplate>();
					AdventureAbstractListEditor<AdventureElementMoveSnapshot, MonoBehaviour>.ItemAddRemove(subList, template, subIndex, subOnUpdate, null);
					AdventureAbstractListEditor<AdventureElementMoveSnapshot, MonoBehaviour>.ItemAddMoveSwap(subList, template, subIndex, subOnUpdate, null);
					AdventureElementMoveSnapshot sub2 = sub;
					if (sub2.MoveCondition == null)
					{
						sub2.MoveCondition = new InstructionAdaptor();
					}
					TMP_InputField inputFieldMoveSpeed = template.moveSpeed;
					AdventureEditorElementPicker pickerTargetElementId = template.targetElementId;
					CDropdown dropDownElementMoveType = template.elementMoveType;
					TMP_InputField dropDownTargetElementTags = template.targetElementTags;
					TMP_InputField dropDownGroupIds = template.groupIds;
					inputFieldMoveSpeed.characterValidation = TMP_InputField.CharacterValidation.Integer;
					inputFieldMoveSpeed.onValueChanged.ResetListener(delegate(string v)
					{
						sub.MoveSpeed = int.Parse(v);
					});
					inputFieldMoveSpeed.text = sub.MoveSpeed.ToString();
					UnityAction<int> <>9__39;
					Action<string> <>9__40;
					Action<string> <>9__41;
					dropDownElementMoveType.SetupEditor("LK_AdventureEditor_AdventureElementMoveType_{0}", delegate(EAdventureElementMoveType v)
					{
						sub.MoveType = v;
						pickerTargetElementId.gameObject.gameObject.SetActive(false);
						TMP_InputField dropDownTargetElementTags;
						dropDownTargetElementTags.gameObject.gameObject.SetActive(false);
						TMP_InputField dropDownGroupIds;
						dropDownGroupIds.gameObject.gameObject.SetActive(false);
						switch (sub.MoveType)
						{
						case EAdventureElementMoveType.CloserToElement:
						case EAdventureElementMoveType.AwayFromElement:
						{
							pickerTargetElementId.gameObject.gameObject.SetActive(true);
							pickerTargetElementId.onElementIdSelected = new UnityEvent<int>();
							UnityEvent<int> onElementIdSelected = pickerTargetElementId.onElementIdSelected;
							UnityAction<int> call;
							if ((call = <>9__39) == null)
							{
								call = (<>9__39 = delegate(int targetId)
								{
									sub.TargetElementId = targetId;
									pickerTargetElementId.Setup(sub.TargetElementId);
								});
							}
							onElementIdSelected.AddListener(call);
							pickerTargetElementId.Setup(sub.TargetElementId);
							break;
						}
						case EAdventureElementMoveType.CloserToTag:
						case EAdventureElementMoveType.AwayFromTag:
						{
							dropDownTargetElementTags.gameObject.gameObject.SetActive(true);
							UnityEvent<string> onValueChanged = dropDownTargetElementTags.onValueChanged;
							Action<string> action;
							if ((action = <>9__40) == null)
							{
								action = (<>9__40 = delegate(string str)
								{
									string[] tags = str.Split(",", StringSplitOptions.None);
									sub.TargetTags.ClearAndAddRange(from t in tags
									select t.Trim());
								});
							}
							onValueChanged.ResetListener(action);
							dropDownTargetElementTags = dropDownTargetElementTags;
							string separator = ", ";
							IEnumerable<string> targetTags = sub.TargetTags;
							dropDownTargetElementTags.text = string.Join(separator, targetTags ?? Array.Empty<string>());
							break;
						}
						case EAdventureElementMoveType.PatrolInSpecifyGroup:
						{
							dropDownGroupIds.gameObject.gameObject.SetActive(true);
							UnityEvent<string> onValueChanged2 = dropDownGroupIds.onValueChanged;
							Action<string> action2;
							if ((action2 = <>9__41) == null)
							{
								action2 = (<>9__41 = delegate(string str)
								{
									string[] groupIds = str.Split(",", StringSplitOptions.None);
									sub.TargetGroupIds.Clear();
									foreach (string groupId in groupIds)
									{
										int value;
										bool flag = int.TryParse(groupId.Trim(), out value);
										if (flag)
										{
											sub.TargetGroupIds.Add(value);
										}
									}
								});
							}
							onValueChanged2.ResetListener(action2);
							dropDownGroupIds = dropDownGroupIds;
							string separator2 = ", ";
							List<int> targetGroupIds = sub.TargetGroupIds;
							IEnumerable<string> enumerable;
							if (targetGroupIds == null)
							{
								enumerable = null;
							}
							else
							{
								enumerable = from x in targetGroupIds
								select x.ToString();
							}
							dropDownGroupIds.text = string.Join(separator2, enumerable ?? Array.Empty<string>());
							break;
						}
						}
					}, (EAdventureElementMoveType v) => sub.MoveType == v, true);
					Action<EventScriptEditorData> <>9__44;
					template.condition.onClick.ResetListener(delegate()
					{
						string json = sub.MoveCondition.EventScriptJson;
						CS$<>8__locals1.<>4__this.editorScript.GetComponent<RectTransform>().position = Vector3.zero;
						EventEditorScript.Init(CS$<>8__locals1.<>4__this.editorScript);
						EventEditorScript eventEditorScript = CS$<>8__locals1.<>4__this.editorScript;
						EventScriptEditorData script = string.IsNullOrEmpty(json) ? new EventScriptEditorData() : JsonConvert.DeserializeObject<EventScriptEditorData>(json);
						Action<EventScriptEditorData> onConfirm;
						if ((onConfirm = <>9__44) == null)
						{
							onConfirm = (<>9__44 = delegate(EventScriptEditorData jD)
							{
								sub.MoveCondition.EventScriptJson = JsonConvert.SerializeObject(jD);
								sub.MoveCondition.EventScriptType = 6;
							});
						}
						eventEditorScript.Show(script, onConfirm, 6, string.Empty, string.Empty);
					});
				});
			}
			adventureCommonRefersListEditor.RefreshAction = refreshAction;
			CS$<>8__locals1.<>4__this.subEditorMoveData.SetEditableList(CS$<>8__locals1.data.MoveData);
			CS$<>8__locals1.<>4__this.subEditorMoveData.gameObject.SetActive(true);
			base.<RefreshEditData>g__SetupSubEditorOpen|30(CS$<>8__locals1.<>4__this.subEditorMoveData.gameObject);
		});
		this.buttonVisibleCondition.onClick.ResetListener(delegate()
		{
			CS$<>8__locals1.<>4__this.subEditorVisibleCondition.Creator = ((IList<AdventureElementVisibleSnapshot> _) => new AdventureElementVisibleSnapshot());
			AdventureCommonRefersListEditor<AdventureElementVisibleSnapshot> adventureCommonRefersListEditor = CS$<>8__locals1.<>4__this.subEditorVisibleCondition;
			Action<IList<AdventureElementVisibleSnapshot>, MonoBehaviour, int, Action> refreshAction;
			if ((refreshAction = CS$<>8__locals1.<>9__46) == null)
			{
				refreshAction = (CS$<>8__locals1.<>9__46 = delegate(IList<AdventureElementVisibleSnapshot> subList, MonoBehaviour subUnit, int subIndex, Action subOnUpdate)
				{
					AdventureElementVisibleSnapshot sub = subList[subIndex];
					AdventureElementConditionEditorTemplate template = subUnit.GetComponent<AdventureElementConditionEditorTemplate>();
					AdventureAbstractListEditor<AdventureElementVisibleSnapshot, MonoBehaviour>.ItemAddRemove(subList, template, subIndex, subOnUpdate, null);
					AdventureAbstractListEditor<AdventureElementVisibleSnapshot, MonoBehaviour>.ItemAddMoveSwap(subList, template, subIndex, subOnUpdate, null);
					TMP_InputField inputFieldIcon = template.icon;
					inputFieldIcon.onValueChanged.ResetListener(delegate(string v)
					{
						sub.VisibleIcon = v;
					});
					inputFieldIcon.text = sub.VisibleIcon;
					Action<EventScriptEditorData> <>9__49;
					template.condition.onClick.ResetListener(delegate()
					{
						AdventureElementVisibleSnapshot sub = sub;
						if (sub.VisibleCondition == null)
						{
							sub.VisibleCondition = new InstructionAdaptor();
						}
						sub.VisibleCondition.EventScriptType = 6;
						string json = sub.VisibleCondition.EventScriptJson;
						CS$<>8__locals1.<>4__this.editorScript.GetComponent<RectTransform>().position = Vector3.zero;
						EventEditorScript.Init(CS$<>8__locals1.<>4__this.editorScript);
						EventEditorScript eventEditorScript = CS$<>8__locals1.<>4__this.editorScript;
						EventScriptEditorData script = string.IsNullOrEmpty(json) ? new EventScriptEditorData() : JsonConvert.DeserializeObject<EventScriptEditorData>(json);
						Action<EventScriptEditorData> onConfirm;
						if ((onConfirm = <>9__49) == null)
						{
							onConfirm = (<>9__49 = delegate(EventScriptEditorData jD)
							{
								sub.VisibleCondition.EventScriptJson = JsonConvert.SerializeObject(jD);
								sub.VisibleCondition.EventScriptType = 6;
							});
						}
						eventEditorScript.Show(script, onConfirm, 6, string.Empty, string.Empty);
					});
				});
			}
			adventureCommonRefersListEditor.RefreshAction = refreshAction;
			CS$<>8__locals1.<>4__this.subEditorVisibleCondition.SetEditableList(CS$<>8__locals1.data.VisibleCondition);
			CS$<>8__locals1.<>4__this.subEditorVisibleCondition.gameObject.SetActive(true);
			base.<RefreshEditData>g__SetupSubEditorOpen|30(CS$<>8__locals1.<>4__this.subEditorVisibleCondition.gameObject);
		});
		this.buttonTriggerEvent.onClick.ResetListener(delegate()
		{
			CS$<>8__locals1.<>4__this.subEditorTriggerEvent.Creator = ((IList<AdventureElementEventSnapshot> _) => new AdventureElementEventSnapshot());
			CS$<>8__locals1.<>4__this.subEditorTriggerEvent.RefreshAction = delegate(IList<AdventureElementEventSnapshot> subList, MonoBehaviour subUnit, int subIndex, Action subOnUpdate)
			{
				AdventureElementEventSnapshot sub = subList[subIndex];
				AdventureElementTriggerEventEditorTemplate template = subUnit.GetComponent<AdventureElementTriggerEventEditorTemplate>();
				AdventureAbstractListEditor<AdventureElementEventSnapshot, MonoBehaviour>.ItemAddRemove(subList, template, subIndex, subOnUpdate, null);
				AdventureAbstractListEditor<AdventureElementEventSnapshot, MonoBehaviour>.ItemAddMoveSwap(subList, template, subIndex, subOnUpdate, null);
				AdventureAutoEventEditorItem.EventEditorRefresh<AdventureElementEventSnapshot>(subList, subIndex, subOnUpdate, sub, template.onlyOnce, template.moveUp, template.moveDown, template.guid, template.comment, template.triggerType);
			};
			CS$<>8__locals1.<>4__this.subEditorTriggerEvent.SetEditableList(CS$<>8__locals1.data.Events);
			CS$<>8__locals1.<>4__this.subEditorTriggerEvent.gameObject.SetActive(true);
			base.<RefreshEditData>g__SetupSubEditorOpen|30(CS$<>8__locals1.<>4__this.subEditorTriggerEvent.gameObject);
		});
		this.buttonVariable.onClick.ResetListener(delegate()
		{
			AdventureCommonRefersListEditor<AdventureParameterSnapshot> adventureCommonRefersListEditor = CS$<>8__locals1.<>4__this.subEditorVariable;
			Func<IList<AdventureParameterSnapshot>, AdventureParameterSnapshot> creator;
			if ((creator = CS$<>8__locals1.<>9__52) == null)
			{
				creator = (CS$<>8__locals1.<>9__52 = ((IList<AdventureParameterSnapshot> _) => AdventureGlobalVariableEditor.GenerateNewItem(CS$<>8__locals1.data.Parameters)));
			}
			adventureCommonRefersListEditor.Creator = creator;
			AdventureCommonRefersListEditor<AdventureParameterSnapshot> adventureCommonRefersListEditor2 = CS$<>8__locals1.<>4__this.subEditorVariable;
			Action<IList<AdventureParameterSnapshot>, MonoBehaviour, int, Action> refreshAction;
			if ((refreshAction = CS$<>8__locals1.<>9__53) == null)
			{
				refreshAction = (CS$<>8__locals1.<>9__53 = delegate(IList<AdventureParameterSnapshot> subList, MonoBehaviour subUnit, int subIndex, Action _)
				{
					AdventureGlobalVariableEditorItem item = subUnit.GetComponent<AdventureGlobalVariableEditorItem>();
					item.Refresh(subIndex, subList, delegate(Action<IList<AdventureParameterSnapshot>> proc)
					{
						proc(subList);
					}, new Action<IList<AdventureParameterSnapshot>>(CS$<>8__locals1.<>4__this.subEditorVariable.ForceRefresh));
				});
			}
			adventureCommonRefersListEditor2.RefreshAction = refreshAction;
			CS$<>8__locals1.<>4__this.subEditorVariable.SetEditableList(CS$<>8__locals1.data.Parameters);
			CS$<>8__locals1.<>4__this.subEditorVariable.gameObject.SetActive(true);
			base.<RefreshEditData>g__SetupSubEditorOpen|30(CS$<>8__locals1.<>4__this.subEditorVariable.gameObject);
		});
		this.buttonPointLight.ClearAndAddListener(delegate
		{
			CS$<>8__locals1.<>4__this.subEditorPointLight.Setup(CS$<>8__locals1.data);
			CS$<>8__locals1.<>4__this.subEditorPointLight.gameObject.SetActive(true);
			base.<RefreshEditData>g__SetupSubEditorOpen|30(CS$<>8__locals1.<>4__this.subEditorPointLight.gameObject);
		});
		List<ValueTuple<int, string>> inheritOptions = AdventureEditorKit.GetElementIds().ToList<ValueTuple<int, string>>();
		inheritOptions.RemoveAll(([TupleElementNames(new string[]
		{
			"Id",
			"path"
		})] ValueTuple<int, string> o) => o.Item1 == CS$<>8__locals1.data.Id);
		inheritOptions.Insert(0, new ValueTuple<int, string>(0, string.Empty));
		this.pickerInherit.onElementIdSelected = new UnityEvent<int>();
		this.pickerInherit.onElementIdSelected.AddListener(delegate(int targetId)
		{
			AdventureEditorElementInfo.<>c__DisplayClass53_4 CS$<>8__locals2 = new AdventureEditorElementInfo.<>c__DisplayClass53_4();
			CS$<>8__locals2.CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals2.targetId = targetId;
			bool flag = CS$<>8__locals1.data.InheritId != CS$<>8__locals2.targetId;
			if (flag)
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_AdventureEditor_Element_Inherit);
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_AdventureEditor_Element_Change_Inherit);
				dialogCmd.Yes = new Action(CS$<>8__locals2.<RefreshEditData>g__Confirm|57);
				Action no;
				if ((no = CS$<>8__locals1.<>9__58) == null)
				{
					no = (CS$<>8__locals1.<>9__58 = delegate()
					{
						CS$<>8__locals1.<>4__this.pickerInherit.Setup(CS$<>8__locals1.data.InheritId);
					});
				}
				dialogCmd.No = no;
				DialogCmd cmd = dialogCmd;
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.SetObject("Cmd", cmd);
				UIElement.Dialog.SetOnInitArgs(box);
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		});
		this.pickerInherit.Setup(CS$<>8__locals1.data.InheritId);
		CS$<>8__locals1.<RefreshEditData>g__UpdateAllFieldEditorSign|28();
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x00082058 File Offset: 0x00080258
	private void Refresh()
	{
		this.signFieldInherit.gameObject.SetActive(false);
		FileInfo editingFile = this._editingFile;
		AdventureElementSnapshot data;
		bool flag = editingFile != null && editingFile.Exists && AdventureElementSnapshot.TryLoadFromFile(this._editingFile.FullName, out data);
		if (flag)
		{
			this.layoutCommon.gameObject.SetActive(true);
			this.layoutElement.gameObject.SetActive(true);
			this.pathInput.text = AdventureEditorElementInfo.GetShortFileName(this._editingFile.Name);
			this.RefreshEditData(data);
			this._editingData = data;
			GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusLoaded, new ArgumentBox().Set("Path", this._editingFile.FullName));
		}
		else
		{
			DirectoryInfo editingDirectory = this._editingDirectory;
			bool flag2 = editingDirectory != null && editingDirectory.Exists;
			if (flag2)
			{
				this.layoutCommon.gameObject.SetActive(true);
				this.layoutElement.gameObject.SetActive(false);
				this.pathInput.text = this._editingDirectory.Name;
			}
			else
			{
				this.layoutCommon.gameObject.SetActive(false);
				this.layoutElement.gameObject.SetActive(false);
				this.Clear();
			}
		}
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x000821AC File Offset: 0x000803AC
	internal static string GetShortFileName(string original)
	{
		return original.ReplaceLast(".adve", "");
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x000821C0 File Offset: 0x000803C0
	private AdventureElementSnapshot CreateEditingElement(out string fileName)
	{
		fileName = this.pathInput.text + ".adve";
		return this._editingData;
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x0008221C File Offset: 0x0008041C
	[CompilerGenerated]
	internal static AdventureElementSnapshot <RefreshEditData>g__GetElementAt|53_29(int index)
	{
		AdventureElementSnapshot element;
		return AdventureEditorKit.TryGetElementInAnywhere(index, out element) ? element : null;
	}

	// Token: 0x04001161 RID: 4449
	[SerializeField]
	private TMP_InputField pathInput;

	// Token: 0x04001162 RID: 4450
	[SerializeField]
	private AdventureEditorElementPicker pickerInherit;

	// Token: 0x04001163 RID: 4451
	[SerializeField]
	private CDropdown dropdownCreateType;

	// Token: 0x04001164 RID: 4452
	[SerializeField]
	private GameObject signFieldInherit;

	// Token: 0x04001165 RID: 4453
	[SerializeField]
	private CButton buttonCharacterData;

	// Token: 0x04001166 RID: 4454
	[SerializeField]
	private TMP_InputField nameInput;

	// Token: 0x04001167 RID: 4455
	[SerializeField]
	private TMP_InputField definitionInput;

	// Token: 0x04001168 RID: 4456
	[SerializeField]
	private TMP_InputField iconInput;

	// Token: 0x04001169 RID: 4457
	[SerializeField]
	private TMP_InputField tagInput;

	// Token: 0x0400116A RID: 4458
	[SerializeField]
	private GameObject layoutCommon;

	// Token: 0x0400116B RID: 4459
	[SerializeField]
	private GameObject layoutElement;

	// Token: 0x0400116C RID: 4460
	[SerializeField]
	private TMP_InputField inputFieldDesc;

	// Token: 0x0400116D RID: 4461
	[SerializeField]
	private TMP_InputField inputFieldCost;

	// Token: 0x0400116E RID: 4462
	[SerializeField]
	private TMP_InputField inputFieldVisiblePriority;

	// Token: 0x0400116F RID: 4463
	[SerializeField]
	private TMP_InputField inputFieldComment;

	// Token: 0x04001170 RID: 4464
	[SerializeField]
	private CButton buttonMoveRule;

	// Token: 0x04001171 RID: 4465
	[SerializeField]
	private CButton buttonVisibleCondition;

	// Token: 0x04001172 RID: 4466
	[SerializeField]
	private CButton buttonTriggerEvent;

	// Token: 0x04001173 RID: 4467
	[SerializeField]
	private CButton buttonVariable;

	// Token: 0x04001174 RID: 4468
	[SerializeField]
	private CButton buttonPointLight;

	// Token: 0x04001175 RID: 4469
	[SerializeField]
	private AdventureCharacterDataEditor subEditorCharacterData;

	// Token: 0x04001176 RID: 4470
	[SerializeField]
	private AdventureCommonRefersListEditor<AdventureElementMoveSnapshot> subEditorMoveData;

	// Token: 0x04001177 RID: 4471
	[SerializeField]
	private AdventureCommonRefersListEditor<AdventureElementVisibleSnapshot> subEditorVisibleCondition;

	// Token: 0x04001178 RID: 4472
	[SerializeField]
	private AdventureCommonRefersListEditor<AdventureElementEventSnapshot> subEditorTriggerEvent;

	// Token: 0x04001179 RID: 4473
	[SerializeField]
	private AdventureElementVariableEditor subEditorVariable;

	// Token: 0x0400117A RID: 4474
	[SerializeField]
	private AdventureElementLightSubEditor subEditorPointLight;

	// Token: 0x0400117B RID: 4475
	[SerializeField]
	private EventEditorScript editorScript;

	// Token: 0x0400117C RID: 4476
	[SerializeField]
	private Canvas topLayerAnchor;

	// Token: 0x0400117D RID: 4477
	[SerializeField]
	internal UnityEvent<int> onElementContainerRefresh = new UnityEvent<int>();

	// Token: 0x0400117E RID: 4478
	[SerializeField]
	private RectTransform placeForSubEditor;

	// Token: 0x0400117F RID: 4479
	[SerializeField]
	private GameObject warning;

	// Token: 0x04001180 RID: 4480
	[SerializeField]
	private CToggle visibleIgnoreSortingToggle;

	// Token: 0x04001181 RID: 4481
	public const int ElementInvalidId = 0;

	// Token: 0x04001182 RID: 4482
	private readonly Dictionary<EAdventureElementOverrideType, AdventureEditorElementInfo.ElementFieldEditInfo> _elementFieldEditInfo = new Dictionary<EAdventureElementOverrideType, AdventureEditorElementInfo.ElementFieldEditInfo>();

	// Token: 0x04001183 RID: 4483
	private readonly Dictionary<Selectable, Toggle> _fieldInheritSigns = new Dictionary<Selectable, Toggle>();

	// Token: 0x04001184 RID: 4484
	private FileInfo _editingFile;

	// Token: 0x04001185 RID: 4485
	private AdventureElementSnapshot _editingData;

	// Token: 0x04001186 RID: 4486
	private DirectoryInfo _editingDirectory;

	// Token: 0x04001187 RID: 4487
	private int _topLayerSort;

	// Token: 0x02001283 RID: 4739
	private struct ElementFieldEditInfo
	{
		// Token: 0x04009AB9 RID: 39609
		public Func<AdventureElementSnapshot, object> ValueGetter;

		// Token: 0x04009ABA RID: 39610
		public Action<AdventureElementSnapshot, object> ValueSetter;

		// Token: 0x04009ABB RID: 39611
		public Action<object> ValueInspector;

		// Token: 0x04009ABC RID: 39612
		public Selectable Control;
	}
}
