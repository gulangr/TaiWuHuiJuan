using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x0200063C RID: 1596
	public class EventEditorAddEventGroup : EventEditorSubPageBase
	{
		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x06004B64 RID: 19300 RVA: 0x0023715E File Offset: 0x0023535E
		// (set) Token: 0x06004B65 RID: 19301 RVA: 0x00237165 File Offset: 0x00235365
		public static EventEditorAddEventGroup Instance { get; private set; }

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x06004B66 RID: 19302 RVA: 0x0023716D File Offset: 0x0023536D
		private EventEditorModel Model
		{
			get
			{
				return SingletonObject.getInstance<EventEditorModel>();
			}
		}

		// Token: 0x06004B67 RID: 19303 RVA: 0x00237174 File Offset: 0x00235374
		public static void Init(EventEditorAddEventGroup instance)
		{
			EventEditorAddEventGroup.Instance = instance;
			EventEditorAddEventGroup.Instance.InternalInit();
			EventEditorAddEventGroup.Instance.Hide();
		}

		// Token: 0x06004B68 RID: 19304 RVA: 0x00237194 File Offset: 0x00235394
		protected override void InternalInit()
		{
			this._keyInputField = this.keyInputField;
			this._keyInputField.onEndEdit.RemoveAllListeners();
			this._keyInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnKeyInputEndEdit));
			this._keyInputField.SetTextWithoutNotify(string.Empty);
			this._nameInputField = this.nameInputField;
			this._nameInputField.onEndEdit.RemoveAllListeners();
			this._nameInputField.onEndEdit.AddListener(new UnityAction<string>(this.OnNameInputEndEdit));
			this._nameInputField.SetTextWithoutNotify(string.Empty);
			this._notifyLabel = this.textMeshNotice;
			this._notifyLabel.text = string.Empty;
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirmAddEventGroup));
			this.btnCancel.ClearAndAddListener(new Action(this.OnCancelAddEventGroup));
		}

		// Token: 0x06004B69 RID: 19305 RVA: 0x00237285 File Offset: 0x00235485
		public override void Show()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x00237295 File Offset: 0x00235495
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x002372A8 File Offset: 0x002354A8
		private void OnKeyInputEndEdit(string groupKey)
		{
			this._notifyLabel.text = string.Empty;
			bool flag = !string.IsNullOrEmpty(groupKey);
			if (flag)
			{
				bool flag2 = !this.Model.IsValidEventGroupKey(groupKey);
				if (flag2)
				{
					this._notifyLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidKey_Used, groupKey);
				}
				else
				{
					bool flag3 = this.Model.IsGroupKeyExist(groupKey);
					if (flag3)
					{
						this._notifyLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidKey_Exist, groupKey);
					}
					else
					{
						bool flag4 = this._keyRegex.IsMatch(groupKey);
						if (flag4)
						{
							this._notifyLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidKey_InputFailed, groupKey);
						}
					}
				}
			}
		}

		// Token: 0x06004B6C RID: 19308 RVA: 0x00237354 File Offset: 0x00235554
		private void OnNameInputEndEdit(string groupName)
		{
			this._notifyLabel.text = string.Empty;
			bool flag = !string.IsNullOrEmpty(groupName);
			if (flag)
			{
				bool flag2 = !this.Model.IsValidEventGroupName(groupName);
				if (flag2)
				{
					this._notifyLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidName_Used, groupName);
				}
				else
				{
					bool flag3 = this.Model.IsGroupNameExist(groupName);
					if (flag3)
					{
						this._notifyLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidName_Exist, groupName);
					}
					else
					{
						bool flag4 = EventEditorAddEventGroup.InvalidCharRegex.IsMatch(groupName);
						if (flag4)
						{
							this._notifyLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_InvalidName_InputFailed, groupName);
						}
					}
				}
			}
		}

		// Token: 0x06004B6D RID: 19309 RVA: 0x00237400 File Offset: 0x00235600
		private void CreateEventGroupDirectoryAndHide()
		{
			string groupKey = this._keyInputField.text;
			string groupName = this._nameInputField.text;
			string eventCorePath = ModManager.GetModEventSaveCore();
			string groupDirectory = Path.Combine(eventCorePath, groupName);
			bool flag = Directory.Exists(groupDirectory);
			if (flag)
			{
				Directory.Delete(groupDirectory);
			}
			Directory.CreateDirectory(groupDirectory);
			string indexFilePath = Path.Combine(groupDirectory, "index.lua");
			StringBuilder indexContentBuilder = new StringBuilder();
			indexContentBuilder.AppendLine("return {");
			indexContentBuilder.AppendLine("\tKey = \"" + groupKey + "\",");
			indexContentBuilder.AppendLine("\tName = \"" + groupName + "\",");
			indexContentBuilder.AppendLine(GroupTableKeys.AllEventContent + " = {},");
			indexContentBuilder.AppendLine("}");
			File.WriteAllText(indexFilePath, indexContentBuilder.ToString(), new UTF8Encoding(false));
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			model.OnModelDataReady = delegate()
			{
				EventGroupData data = model.GetGroupData(groupKey);
				EventGroupTreeView.Instance.RefreshToEventGroup(data);
				EventEditorEventList.Instance.Show();
			};
			model.LoadEventCore();
			this.Hide();
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x00237520 File Offset: 0x00235720
		private void OnConfirmAddEventGroup()
		{
			string groupKey = this._keyInputField.text;
			this.OnKeyInputEndEdit(groupKey);
			bool flag = string.IsNullOrEmpty(this._notifyLabel.text) && string.IsNullOrEmpty(groupKey);
			if (flag)
			{
				this._notifyLabel.text = LocalStringManager.Get(LanguageKey.LK_EventEditor_AddEventGroup_InvalidKey_CannotEmpty);
			}
			bool flag2 = !string.IsNullOrEmpty(this._notifyLabel.text);
			if (!flag2)
			{
				string groupName = this._nameInputField.text;
				this.OnNameInputEndEdit(groupName);
				bool flag3 = string.IsNullOrEmpty(this._notifyLabel.text) && string.IsNullOrEmpty(groupName);
				if (flag3)
				{
					this._notifyLabel.text = LocalStringManager.Get(LanguageKey.LK_EventEditor_AddEventGroup_InvalidName_CannotEmpty);
				}
				bool flag4 = !string.IsNullOrEmpty(this._notifyLabel.text);
				if (!flag4)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_GameName),
						Content = LocalStringManager.GetFormat(LanguageKey.LK_EventEditor_AddEventGroup_ConfirmNote, groupKey, groupName),
						Yes = new Action(this.CreateEventGroupDirectoryAndHide)
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x00237660 File Offset: 0x00235860
		private void OnCancelAddEventGroup()
		{
			bool flag = string.IsNullOrEmpty(this._nameInputField.text) && string.IsNullOrEmpty(this._keyInputField.text);
			if (flag)
			{
				this.Hide();
			}
			else
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_GameName),
					Content = LocalStringManager.Get(LanguageKey.LK_EventEditor_AddEventGroup_GiveUpNote),
					Yes = new Action(this.Hide)
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x04003461 RID: 13409
		private TMP_InputField _keyInputField;

		// Token: 0x04003462 RID: 13410
		private TMP_InputField _nameInputField;

		// Token: 0x04003463 RID: 13411
		private TextMeshProUGUI _notifyLabel;

		// Token: 0x04003464 RID: 13412
		private Regex _keyRegex = new Regex("([^A-Z|a-z|0-9|_|])+");

		// Token: 0x04003465 RID: 13413
		private static readonly Regex InvalidCharRegex = new Regex("[\\u0022\\s~!@#$%^&\\*\\(\\)_\\+\\=\\[\\]\\{\\}\\\\\\|;:',.<>\\/\\?·`！￥…（）—、【】：；‘’“”《》，。？]");

		// Token: 0x04003466 RID: 13414
		[SerializeField]
		private TMP_InputField keyInputField;

		// Token: 0x04003467 RID: 13415
		[SerializeField]
		private TMP_InputField nameInputField;

		// Token: 0x04003468 RID: 13416
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04003469 RID: 13417
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x0400346A RID: 13418
		[SerializeField]
		private TextMeshProUGUI textMeshNotice;
	}
}
