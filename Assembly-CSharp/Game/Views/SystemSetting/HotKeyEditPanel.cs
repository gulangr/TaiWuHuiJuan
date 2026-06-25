using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000769 RID: 1897
	public class HotKeyEditPanel : MonoBehaviour
	{
		// Token: 0x06005BC8 RID: 23496 RVA: 0x002A9854 File Offset: 0x002A7A54
		private void Awake()
		{
			this._viewSystemSetting = base.GetComponentInParent<ViewSystemSetting>();
			this.confirmBtn.ClearAndAddListener(new Action(this.OnConfirmClick));
			this.cancelBtn.ClearAndAddListener(new Action(this.OnCancelClick));
			this.deleteBtn.ClearAndAddListener(new Action(this.OnDeleteClick));
			this.resetBtn.ClearAndAddListener(new Action(this.OnResetClick));
		}

		// Token: 0x06005BC9 RID: 23497 RVA: 0x002A98CE File Offset: 0x002A7ACE
		private void OnEnable()
		{
			this._isEditing = true;
			this.confirmBtn.interactable = true;
			CommandKitBase.SetDisable(true);
			CommandKitBase.SetDisable(true);
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x002A98F3 File Offset: 0x002A7AF3
		private void OnDisable()
		{
			this._isEditing = false;
			CommandKitBase.SetDisable(false);
			CommandKitBase.SetDisable(false);
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x002A990C File Offset: 0x002A7B0C
		private void Update()
		{
			bool flag = !this._isEditing || this._command == null;
			if (!flag)
			{
				this.ListenKeyInput();
				this._guiDetectedKey = KeyCode.None;
				this._hasGuiKeyDetected = false;
			}
		}

		// Token: 0x06005BCC RID: 23500 RVA: 0x002A994C File Offset: 0x002A7B4C
		private void OnGUI()
		{
			bool flag = !this._isEditing || this._command == null;
			if (!flag)
			{
				bool flag2 = Event.current.type == EventType.KeyDown && Event.current.keyCode > KeyCode.None;
				if (flag2)
				{
					KeyCode keyCode = Event.current.keyCode;
					this._guiDetectedKey = keyCode;
					this._hasGuiKeyDetected = true;
					Event.current.Use();
				}
			}
		}

		// Token: 0x06005BCD RID: 23501 RVA: 0x002A99BC File Offset: 0x002A7BBC
		public void Set(HotKeyCommand command, byte kitId, ESettingSubCategory subCategory, bool isMouseKey, Action onEditComplete = null)
		{
			this._command = command;
			this._kitId = kitId;
			this._subCategory = subCategory;
			this._isMouseKey = isMouseKey;
			this._onEditComplete = onEditComplete;
			this.SaveOriginalKey();
			this._newKey = KeyCode.None;
			this._newFnKey = KeyCode.None;
			this._hasNewInput = false;
			this.UpdateDisplay(this._command);
			this.SetTipText(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_0.Tr());
		}

		// Token: 0x06005BCE RID: 23502 RVA: 0x002A9A2C File Offset: 0x002A7C2C
		private void ListenKeyInput()
		{
			KeyCode keyCode = KeyCode.None;
			KeyCode fnKey = KeyCode.None;
			bool flag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			if (flag)
			{
				fnKey = KeyCode.LeftControl;
			}
			else
			{
				bool flag2 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				if (flag2)
				{
					fnKey = KeyCode.LeftAlt;
				}
				else
				{
					bool flag3 = Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta);
					if (flag3)
					{
						fnKey = KeyCode.LeftMeta;
					}
					else
					{
						bool flag4 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
						if (flag4)
						{
							fnKey = KeyCode.LeftShift;
						}
					}
				}
			}
			bool flag5 = Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt) || Input.GetKeyUp(KeyCode.LeftMeta) || Input.GetKeyUp(KeyCode.RightMeta) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
			if (flag5)
			{
				this._hasSet = false;
			}
			this.ShowFnKeyPlus(fnKey);
			bool flag6 = this._hasGuiKeyDetected && this._guiDetectedKey > KeyCode.None;
			if (flag6)
			{
				keyCode = this._guiDetectedKey;
			}
			bool flag7 = keyCode == KeyCode.None;
			if (flag7)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(0);
				if (mouseButtonDown)
				{
					keyCode = KeyCode.Mouse0;
				}
				else
				{
					bool mouseButtonDown2 = Input.GetMouseButtonDown(1);
					if (mouseButtonDown2)
					{
						keyCode = KeyCode.Mouse1;
					}
					else
					{
						bool mouseButtonDown3 = Input.GetMouseButtonDown(2);
						if (mouseButtonDown3)
						{
							keyCode = KeyCode.Mouse2;
						}
						else
						{
							bool mouseButtonDown4 = Input.GetMouseButtonDown(3);
							if (mouseButtonDown4)
							{
								keyCode = KeyCode.Mouse3;
							}
							else
							{
								bool mouseButtonDown5 = Input.GetMouseButtonDown(4);
								if (mouseButtonDown5)
								{
									keyCode = KeyCode.Mouse4;
								}
								else
								{
									bool mouseButtonDown6 = Input.GetMouseButtonDown(5);
									if (mouseButtonDown6)
									{
										keyCode = KeyCode.Mouse5;
									}
									else
									{
										bool mouseButtonDown7 = Input.GetMouseButtonDown(6);
										if (mouseButtonDown7)
										{
											keyCode = KeyCode.Mouse6;
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag8 = keyCode == KeyCode.None;
			if (flag8)
			{
				Vector2 scrollDelta = Input.mouseScrollDelta;
				bool flag9 = scrollDelta.y > 0.5f;
				if (flag9)
				{
					keyCode = (KeyCode)1000;
				}
				else
				{
					bool flag10 = scrollDelta.y < -0.5f;
					if (flag10)
					{
						keyCode = (KeyCode)1001;
					}
				}
			}
			bool flag11 = keyCode == KeyCode.None || keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1;
			if (!flag11)
			{
				bool flag12 = HotKeyEditPanel.IsFnKey(keyCode);
				if (flag12)
				{
					this.ShowTemporaryTip(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_1.Tr());
				}
				else
				{
					bool flag13 = !HotKeyEditPanel.IsCanSetKey(keyCode) || (fnKey == KeyCode.LeftShift && HotKeyEditPanel.IsKeypadKey(keyCode));
					if (flag13)
					{
						this.ShowTemporaryTip(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_2.Tr());
					}
					else
					{
						ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand> conflictCheckRes = HotKeyService.CheckConflict(this._subCategory, this._command, keyCode, fnKey);
						bool flag14 = !conflictCheckRes.Item2;
						if (flag14)
						{
							this.ShowTemporaryTip(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_3.Tr());
						}
						else
						{
							this._newKey = keyCode;
							this._newFnKey = fnKey;
							this._hasNewInput = true;
							this._hasSet = true;
							bool item = conflictCheckRes.Item1;
							if (item)
							{
								this.SetTipText(LanguageKey.LK_HotKeyGroup_HotKey_Conflict_Notice.TrFormat(conflictCheckRes.Item4.Tr(), conflictCheckRes.Item5.DescLanguageId.Tr()));
							}
							else
							{
								this.SetTipText(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_0.Tr());
							}
							this.UpdateDisplay(this._command);
						}
					}
				}
			}
		}

		// Token: 0x06005BCF RID: 23503 RVA: 0x002A9D80 File Offset: 0x002A7F80
		private void OnConfirmClick()
		{
			bool flag = this._command == null;
			if (!flag)
			{
				bool hasNewInput = this._hasNewInput;
				if (hasNewInput)
				{
					List<HotKeyConflictInfo> conflicts = HotKeyService.FindConflicts(this._subCategory, this._command, this._newKey, this._newFnKey);
					bool flag2 = conflicts.Count > 0;
					if (flag2)
					{
						this.ShowConflictDialog(conflicts);
					}
					else
					{
						this.ApplyHotKeySetting(conflicts);
					}
				}
				else
				{
					this.ClosePanel();
				}
			}
		}

		// Token: 0x06005BD0 RID: 23504 RVA: 0x002A9DF8 File Offset: 0x002A7FF8
		public void OnCancelClick()
		{
			bool flag = this._command != null;
			if (flag)
			{
				this._command.SetCustomKey(this._originalKey, this._originalFnKey, this._isMouseKey);
			}
			this.ClosePanel();
		}

		// Token: 0x06005BD1 RID: 23505 RVA: 0x002A9E3C File Offset: 0x002A803C
		private void OnDeleteClick()
		{
			bool flag = this._command == null;
			if (!flag)
			{
				this._newKey = KeyCode.None;
				this._newFnKey = KeyCode.None;
				this._hasNewInput = true;
				this.UpdateDisplay(this._command);
			}
		}

		// Token: 0x06005BD2 RID: 23506 RVA: 0x002A9E7C File Offset: 0x002A807C
		private void OnResetClick()
		{
			bool flag = this._command == null;
			if (!flag)
			{
				this.SaveOriginalKey();
				this._command.Reset();
				HotKeyGroup keyGroup = this._command.KeyGroup;
				bool flag2 = !this._isMouseKey;
				if (flag2)
				{
					this._newKey = keyGroup.Key;
					this._newFnKey = keyGroup.FunctionKey;
				}
				else
				{
					this._newKey = keyGroup.MouseKey;
					this._newFnKey = keyGroup.FunctionMouseKey;
				}
				this._hasNewInput = true;
				bool flag3 = this._newKey > KeyCode.None;
				if (flag3)
				{
					ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand> conflictCheckRes = HotKeyService.CheckConflict(this._subCategory, this._command, this._newKey, this._newFnKey);
					bool flag4 = !conflictCheckRes.Item2;
					if (flag4)
					{
						this.SetTipText(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_3.Tr());
					}
					else
					{
						bool item = conflictCheckRes.Item1;
						if (item)
						{
							this.SetTipText(LanguageKey.LK_HotKeyGroup_HotKey_Conflict_Notice.TrFormat(conflictCheckRes.Item4.Tr(), conflictCheckRes.Item5.DescLanguageId.Tr()));
						}
						else
						{
							this.SetTipText(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_0.Tr());
						}
					}
				}
				else
				{
					this.SetTipText(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_0.Tr());
				}
				this.UpdateDisplay(this._command);
			}
		}

		// Token: 0x06005BD3 RID: 23507 RVA: 0x002A9FD0 File Offset: 0x002A81D0
		private void ShowFnKeyPlus(KeyCode fnKey)
		{
			bool hasSet = this._hasSet;
			if (hasSet)
			{
				this.tempDisplayItem.SetActive(false);
				this.hotKeyDisplayItem.gameObject.SetActive(true);
			}
			else if (fnKey != KeyCode.None)
			{
				switch (fnKey)
				{
				case KeyCode.LeftShift:
					this.tempDisplayItem.SetActive(true);
					this.hotKeyDisplayItem.gameObject.SetActive(false);
					this.tempFnText.text = "Shift";
					break;
				case KeyCode.LeftControl:
					this.tempDisplayItem.SetActive(true);
					this.hotKeyDisplayItem.gameObject.SetActive(false);
					this.tempFnText.text = "Ctrl";
					break;
				case KeyCode.LeftAlt:
					this.tempDisplayItem.SetActive(true);
					this.hotKeyDisplayItem.gameObject.SetActive(false);
					this.tempFnText.text = "Alt";
					break;
				case KeyCode.LeftMeta:
					this.tempDisplayItem.SetActive(true);
					this.hotKeyDisplayItem.gameObject.SetActive(false);
					this.tempFnText.text = "Command";
					break;
				}
			}
			else
			{
				this.tempDisplayItem.SetActive(false);
				this.hotKeyDisplayItem.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005BD4 RID: 23508 RVA: 0x002AA138 File Offset: 0x002A8338
		private void SaveOriginalKey()
		{
			bool flag = this._command == null;
			if (!flag)
			{
				HotKeyGroup keyGroup = this._command.KeyGroup;
				bool flag2 = !this._isMouseKey;
				if (flag2)
				{
					this._originalKey = keyGroup.Key;
					this._originalFnKey = keyGroup.FunctionKey;
				}
				else
				{
					this._originalKey = keyGroup.MouseKey;
					this._originalFnKey = keyGroup.FunctionMouseKey;
				}
			}
		}

		// Token: 0x06005BD5 RID: 23509 RVA: 0x002AA1A4 File Offset: 0x002A83A4
		private void UpdateDisplay(HotKeyCommand curCommand)
		{
			bool flag = this.hotKeyDisplayItem == null;
			if (!flag)
			{
				bool hasNewInput = this._hasNewInput;
				if (hasNewInput)
				{
					this.hotKeyDisplayItem.SetWithTransition(curCommand, this._newKey, this._newFnKey, this._isMouseKey);
				}
				else
				{
					this.hotKeyDisplayItem.Set(curCommand, this._isMouseKey, true);
				}
			}
		}

		// Token: 0x06005BD6 RID: 23510 RVA: 0x002AA208 File Offset: 0x002A8408
		private void SetTipText(string text)
		{
			bool flag = this.tipText != null;
			if (flag)
			{
				this.tipText.text = text.ColorReplace();
			}
			TextMeshProUGUI textMeshProUGUI = this.tipText;
			TMPTextSpriteHelper helper;
			bool flag2 = textMeshProUGUI != null && textMeshProUGUI.TryGetComponent<TMPTextSpriteHelper>(out helper);
			if (flag2)
			{
				helper.Parse();
			}
		}

		// Token: 0x06005BD7 RID: 23511 RVA: 0x002AA25C File Offset: 0x002A845C
		private void ShowTemporaryTip(string text)
		{
			this.SetTipText(text);
			bool flag = this._tipCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._tipCoroutine);
			}
			this._tipCoroutine = base.StartCoroutine(this.DelayResetTip(3f));
		}

		// Token: 0x06005BD8 RID: 23512 RVA: 0x002AA2A3 File Offset: 0x002A84A3
		private IEnumerator DelayResetTip(float delay)
		{
			yield return new WaitForSeconds(delay);
			this.SetTipText(LanguageKey.LK_SystemSetting_HotKey_Edit_Notice_0.Tr());
			this._tipCoroutine = null;
			yield break;
		}

		// Token: 0x06005BD9 RID: 23513 RVA: 0x002AA2BC File Offset: 0x002A84BC
		private void ClosePanel()
		{
			this._isEditing = false;
			this._command = null;
			bool flag = this._tipCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._tipCoroutine);
				this._tipCoroutine = null;
			}
			Action onEditComplete = this._onEditComplete;
			if (onEditComplete != null)
			{
				onEditComplete();
			}
			this._onEditComplete = null;
			this._viewSystemSetting.EndHotKeyEdit();
		}

		// Token: 0x06005BDA RID: 23514 RVA: 0x002AA320 File Offset: 0x002A8520
		private void ShowConflictDialog(List<HotKeyConflictInfo> conflicts)
		{
			ValueTuple<string, string> valueTuple = this.BuildConflictInfoText(conflicts);
			string groupName = valueTuple.Item1;
			string commandName = valueTuple.Item2;
			UIElement dialog = UIElement.Dialog;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			string key = "Cmd";
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Title = LanguageKey.LK_HotKeyGroup_HotKey_Conflict_Title.Tr();
			dialogCmd.Content = LanguageKey.LK_HotKeyGroup_HotKey_Conflict_Content.TrFormat(groupName, commandName);
			dialogCmd.Yes = delegate()
			{
				this.ApplyHotKeySetting(conflicts);
			};
			dialogCmd.No = delegate()
			{
			};
			dialogCmd.Type = 1;
			dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06005BDB RID: 23515 RVA: 0x002AA3EC File Offset: 0x002A85EC
		[return: TupleElementNames(new string[]
		{
			"groupName",
			"commandName"
		})]
		private ValueTuple<string, string> BuildConflictInfoText(List<HotKeyConflictInfo> conflicts)
		{
			bool flag = conflicts.Count == 0;
			ValueTuple<string, string> result;
			if (flag)
			{
				result = new ValueTuple<string, string>(string.Empty, string.Empty);
			}
			else
			{
				HotKeyConflictInfo firstConflict = conflicts[0];
				string groupName = firstConflict.GroupLangId.Tr();
				string commandName = firstConflict.Command.DescLanguageId.Tr();
				result = new ValueTuple<string, string>(groupName, commandName);
			}
			return result;
		}

		// Token: 0x06005BDC RID: 23516 RVA: 0x002AA44C File Offset: 0x002A864C
		private void ApplyHotKeySetting(List<HotKeyConflictInfo> conflicts)
		{
			this._command.SetCustomKey(this._newKey, this._newFnKey, this._isMouseKey);
			for (int i = 0; i < conflicts.Count; i++)
			{
				HotKeyConflictInfo conflict = conflicts[i];
				bool keyConflict = conflict.KeyConflict;
				if (keyConflict)
				{
					conflict.Command.ClearKeyBinding();
				}
				bool mouseKeyConflict = conflict.MouseKeyConflict;
				if (mouseKeyConflict)
				{
					conflict.Command.ClearMouseKeyBinding();
				}
			}
			HotKeyService.SetCommandConflictState(this._command, false, false);
			for (int j = 0; j < conflicts.Count; j++)
			{
				HotKeyConflictInfo conflict2 = conflicts[j];
				HotKeyService.SetCommandConflictState(conflict2.Command, conflict2.KeyConflict, conflict2.MouseKeyConflict);
			}
			this._viewSystemSetting.RefreshAllHotKeySettingItems();
			this.ClosePanel();
		}

		// Token: 0x06005BDD RID: 23517 RVA: 0x002AA528 File Offset: 0x002A8728
		private static bool IsFnKey(KeyCode keyCode)
		{
			for (int i = 0; i < HotKeyEditPanel.FnKeyList.Length; i++)
			{
				bool flag = HotKeyEditPanel.FnKeyList[i] == keyCode;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005BDE RID: 23518 RVA: 0x002AA568 File Offset: 0x002A8768
		private static bool IsCanSetKey(KeyCode keyCode)
		{
			for (int i = 0; i < HotKeyEditPanel.CanSetKeyList.Length; i++)
			{
				bool flag = HotKeyEditPanel.CanSetKeyList[i] == keyCode;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005BDF RID: 23519 RVA: 0x002AA5A8 File Offset: 0x002A87A8
		private static bool IsKeypadKey(KeyCode keyCode)
		{
			return keyCode >= KeyCode.Keypad0 && keyCode <= KeyCode.Keypad9;
		}

		// Token: 0x06005BE1 RID: 23521 RVA: 0x002AA5F0 File Offset: 0x002A87F0
		// Note: this type is marked as 'beforefieldinit'.
		static HotKeyEditPanel()
		{
			KeyCode[] array = new KeyCode[88];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.6E78EB411E2F6DB517393C8C393DFEE4C1157DE65C4D75B2FDE63868F547C3B8).FieldHandle);
			HotKeyEditPanel.CanSetKeyList = array;
			KeyCode[] array2 = new KeyCode[8];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.1E0A815E16C86B1AA2717EF42844B5B78203BED0A5FC135D0607A5FF923C9726).FieldHandle);
			HotKeyEditPanel.FnKeyList = array2;
			HotKeyEditPanel.ShiftKeypadRemap = new Dictionary<KeyCode, KeyCode>
			{
				{
					KeyCode.Home,
					KeyCode.Keypad7
				},
				{
					KeyCode.UpArrow,
					KeyCode.Keypad8
				},
				{
					KeyCode.PageUp,
					KeyCode.Keypad9
				},
				{
					KeyCode.LeftArrow,
					KeyCode.Keypad4
				},
				{
					KeyCode.RightArrow,
					KeyCode.Keypad6
				},
				{
					KeyCode.End,
					KeyCode.Keypad1
				},
				{
					KeyCode.DownArrow,
					KeyCode.Keypad2
				},
				{
					KeyCode.PageDown,
					KeyCode.Keypad3
				},
				{
					KeyCode.Insert,
					KeyCode.Keypad0
				},
				{
					KeyCode.Delete,
					KeyCode.KeypadPeriod
				}
			};
		}

		// Token: 0x04003F4F RID: 16207
		[Header("按钮")]
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04003F50 RID: 16208
		[SerializeField]
		private CButton cancelBtn;

		// Token: 0x04003F51 RID: 16209
		[SerializeField]
		private CButton deleteBtn;

		// Token: 0x04003F52 RID: 16210
		[SerializeField]
		private CButton resetBtn;

		// Token: 0x04003F53 RID: 16211
		[Header("显示")]
		[SerializeField]
		private HotKeyDisplayItem hotKeyDisplayItem;

		// Token: 0x04003F54 RID: 16212
		[SerializeField]
		private TextMeshProUGUI tipText;

		// Token: 0x04003F55 RID: 16213
		[SerializeField]
		private GameObject tempDisplayItem;

		// Token: 0x04003F56 RID: 16214
		[SerializeField]
		private TextMeshProUGUI tempFnText;

		// Token: 0x04003F57 RID: 16215
		private ViewSystemSetting _viewSystemSetting;

		// Token: 0x04003F58 RID: 16216
		private HotKeyCommand _command;

		// Token: 0x04003F59 RID: 16217
		private byte _kitId;

		// Token: 0x04003F5A RID: 16218
		private ESettingSubCategory _subCategory;

		// Token: 0x04003F5B RID: 16219
		private bool _isMouseKey;

		// Token: 0x04003F5C RID: 16220
		private KeyCode _originalKey;

		// Token: 0x04003F5D RID: 16221
		private KeyCode _originalFnKey;

		// Token: 0x04003F5E RID: 16222
		private KeyCode _newKey = KeyCode.None;

		// Token: 0x04003F5F RID: 16223
		private KeyCode _newFnKey = KeyCode.None;

		// Token: 0x04003F60 RID: 16224
		private bool _hasNewInput;

		// Token: 0x04003F61 RID: 16225
		private bool _isEditing;

		// Token: 0x04003F62 RID: 16226
		private Coroutine _tipCoroutine;

		// Token: 0x04003F63 RID: 16227
		private Action _onEditComplete;

		// Token: 0x04003F64 RID: 16228
		private KeyCode _guiDetectedKey = KeyCode.None;

		// Token: 0x04003F65 RID: 16229
		private bool _hasGuiKeyDetected;

		// Token: 0x04003F66 RID: 16230
		private bool _hasSet;

		// Token: 0x04003F67 RID: 16231
		private static readonly KeyCode[] CanSetKeyList;

		// Token: 0x04003F68 RID: 16232
		private static readonly KeyCode[] FnKeyList;

		// Token: 0x04003F69 RID: 16233
		private static readonly Dictionary<KeyCode, KeyCode> ShiftKeypadRemap;
	}
}
