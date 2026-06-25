using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EventEditor.EventScript;
using EventEditor.GlobalScript;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000643 RID: 1603
	public class EventEditorGlobalScriptBrowser : EventEditorSubPageBase
	{
		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06004C01 RID: 19457 RVA: 0x0023D9AC File Offset: 0x0023BBAC
		public Dictionary<string, GlobalScriptEditorData> ScriptNameDic
		{
			get
			{
				bool flag = this._scriptNameDic.Count == 0;
				if (flag)
				{
					this.LoadAllGlobalScripts(false);
				}
				return this._scriptNameDic;
			}
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06004C02 RID: 19458 RVA: 0x0023D9E0 File Offset: 0x0023BBE0
		public Dictionary<string, GlobalScriptEditorData> ScriptGuidDic
		{
			get
			{
				bool flag = this._scriptGuidDic.Count == 0;
				if (flag)
				{
					this.LoadAllGlobalScripts(false);
				}
				return this._scriptGuidDic;
			}
		}

		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06004C03 RID: 19459 RVA: 0x0023DA14 File Offset: 0x0023BC14
		public List<string> ScriptNames
		{
			get
			{
				bool flag = this._scriptNameDic.Count == 0;
				if (flag)
				{
					this.LoadAllGlobalScripts(false);
				}
				return this._scriptNameDic.Keys.ToList<string>();
			}
		}

		// Token: 0x06004C04 RID: 19460 RVA: 0x0023DA52 File Offset: 0x0023BC52
		public static void Init(EventEditorGlobalScriptBrowser instance)
		{
			EventEditorGlobalScriptBrowser.Instance = instance;
			EventEditorGlobalScriptBrowser.Instance.InternalInit();
			EventEditorGlobalScriptBrowser.Instance.Hide();
		}

		// Token: 0x06004C05 RID: 19461 RVA: 0x0023DA74 File Offset: 0x0023BC74
		private void ClearScripts()
		{
			this._scriptNameDic.Clear();
			for (int i = 0; i < this._scriptsScroll.Content.childCount; i++)
			{
				Object.Destroy(this._scriptsScroll.Content.GetChild(i).gameObject);
			}
		}

		// Token: 0x06004C06 RID: 19462 RVA: 0x0023DACB File Offset: 0x0023BCCB
		private void OnCancel()
		{
			this.Hide();
		}

		// Token: 0x06004C07 RID: 19463 RVA: 0x0023DAD5 File Offset: 0x0023BCD5
		private void OnConfirm()
		{
			Debug.Log("Confirm");
		}

		// Token: 0x06004C08 RID: 19464 RVA: 0x0023DAE4 File Offset: 0x0023BCE4
		private void LoadAllGlobalScripts(bool creatObj = true)
		{
			this._scriptNameDic.Clear();
			this._scriptObjDic.Clear();
			this._scriptGuidDic.Clear();
			string path = ModManager.GetModEventGlobalScriptsFolder();
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				bool flag = fileInfo.Extension == ".tws";
				if (flag)
				{
					GlobalScriptEditorData data = JsonConvert.DeserializeObject<GlobalScriptEditorData>(File.ReadAllText(fileInfo.FullName));
					bool flag2 = string.IsNullOrEmpty(data.FileName);
					if (flag2)
					{
						string scriptName = Path.GetFileNameWithoutExtension(fileInfo.Name);
						data.FileName = scriptName;
						File.WriteAllText(fileInfo.FullName, JsonConvert.SerializeObject(data, Formatting.Indented));
					}
					bool flag3 = string.IsNullOrEmpty(data.Guid);
					if (flag3)
					{
						data.Guid = Guid.NewGuid().ToString();
						File.WriteAllText(fileInfo.FullName, JsonConvert.SerializeObject(data, Formatting.Indented));
					}
					this._scriptNameDic.Add(data.FileName, data);
					this._scriptGuidDic.Add(data.Guid, data);
					if (creatObj)
					{
						this.CreateObject(data.FileName);
					}
				}
			}
		}

		// Token: 0x06004C09 RID: 19465 RVA: 0x0023DC38 File Offset: 0x0023BE38
		private void CreateObject(string scriptName)
		{
			GameObject obj = Object.Instantiate<GameObject>(this._scriptTemplate, this._scriptsScroll.Content);
			EventEditorGlobalScriptBrowserScriptTemplateInfo scriptTempInfo = obj.GetComponent<EventEditorGlobalScriptBrowserScriptTemplateInfo>();
			scriptTempInfo.txtMeshScriptName.SetText(scriptName, true);
			CButton button = scriptTempInfo.btn;
			button.ClearAndAddListener(delegate
			{
				this.OnEditScript(scriptName);
			});
			this._scriptObjDic.Add(scriptName, obj);
			PointClickBridge pointClickBridge = obj.GetComponent<PointClickBridge>();
			pointClickBridge.OnRightClick = delegate()
			{
				this.ActiveScriptButtonSheet(scriptName);
			};
		}

		// Token: 0x06004C0A RID: 19466 RVA: 0x0023DCD4 File Offset: 0x0023BED4
		private void ActiveScriptButtonSheet(string scriptName)
		{
			List<SheetButtonInfo> sheetInfos = new List<SheetButtonInfo>
			{
				new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_Delete), delegate()
				{
					this.DeleteScript(scriptName);
				}, true, ""),
				new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_RenameScript), delegate()
				{
					this.OnChangeGlobalScriptName(scriptName);
				}, true, ""),
				new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_CopyGuid), delegate()
				{
					GUIUtility.systemCopyBuffer = "\"" + this._scriptNameDic[scriptName].Guid + "\"";
				}, true, "")
			};
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", sheetInfos);
			UIElement.ButtonSheet.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
		}

		// Token: 0x06004C0B RID: 19467 RVA: 0x0023DDA4 File Offset: 0x0023BFA4
		private void DeleteScript(string scriptName)
		{
			string path = this.GetGlobalScriptFullPath(scriptName);
			GlobalScriptEditorData script;
			bool flag = this._scriptNameDic.TryGetValue(scriptName, out script);
			if (flag)
			{
				this._scriptGuidDic.Remove(script.Guid);
				this._scriptNameDic.Remove(scriptName);
				Object.Destroy(this._scriptObjDic[scriptName]);
				this._scriptObjDic.Remove(scriptName);
			}
			File.Delete(path);
		}

		// Token: 0x06004C0C RID: 19468 RVA: 0x0023DE14 File Offset: 0x0023C014
		private void CreateScript()
		{
			string scriptName = this._scriptNameInputField.text;
			bool flag = string.IsNullOrEmpty(scriptName);
			if (!flag)
			{
				while (this._scriptNameDic.ContainsKey(scriptName))
				{
					scriptName += "Copy";
				}
				GlobalScriptEditorData data = new GlobalScriptEditorData();
				data.FileName = scriptName;
				this._scriptNameDic.Add(scriptName, data);
				this._scriptGuidDic.Add(data.Guid, data);
				this.CreateObject(scriptName);
				this.SaveGlobalScript(scriptName);
				this._scriptNameEditObj.SetActive(false);
			}
		}

		// Token: 0x06004C0D RID: 19469 RVA: 0x0023DEA8 File Offset: 0x0023C0A8
		private void ChangeGlobalScriptName(GlobalScriptEditorData data)
		{
			string newName = this._scriptNameInputField.text;
			bool flag = newName == data.FileName;
			if (!flag)
			{
				bool flag2 = string.IsNullOrEmpty(newName);
				if (!flag2)
				{
					while (this._scriptNameDic.ContainsKey(newName))
					{
						newName += "Copy";
					}
					this.DeleteScript(data.FileName);
					data.FileName = newName;
					this._scriptNameDic.Add(data.FileName, data);
					this._scriptGuidDic.Add(data.Guid, data);
					this.CreateObject(data.FileName);
					this.SaveGlobalScript(data.FileName);
					this._scriptNameEditObj.SetActive(false);
				}
			}
		}

		// Token: 0x06004C0E RID: 19470 RVA: 0x0023DF68 File Offset: 0x0023C168
		private void OnChangeGlobalScriptName(string scriptName)
		{
			GlobalScriptEditorData data;
			bool flag = this._scriptNameDic.TryGetValue(scriptName, out data);
			if (flag)
			{
				this._scriptNameInputField.SetTextWithoutNotify(data.FileName);
				this._scriptNameEditObj.SetActive(true);
				this._scriptNameEditConfirmBtn.ClearAndAddListener(delegate
				{
					this.ChangeGlobalScriptName(data);
				});
			}
		}

		// Token: 0x06004C0F RID: 19471 RVA: 0x0023DFD7 File Offset: 0x0023C1D7
		private void OnCreateNewScript()
		{
			this._scriptNameInputField.SetTextWithoutNotify("New Script");
			this._scriptNameEditObj.SetActive(true);
			this._scriptNameEditConfirmBtn.ClearAndAddListener(new Action(this.CreateScript));
		}

		// Token: 0x06004C10 RID: 19472 RVA: 0x0023E010 File Offset: 0x0023C210
		private void OnEditScript(string scriptName)
		{
			GlobalScriptEditorData data;
			bool flag = this._scriptNameDic.TryGetValue(scriptName, out data);
			if (flag)
			{
				EventEditorScript.Instance.Show(data, delegate(EventScriptEditorData editorData)
				{
					this.SaveGlobalScript(scriptName);
				}, 0, data.Guid, "");
			}
		}

		// Token: 0x06004C11 RID: 19473 RVA: 0x0023E070 File Offset: 0x0023C270
		private void SaveGlobalScript(string scriptName)
		{
			string path = this.GetGlobalScriptFullPath(scriptName);
			GlobalScriptEditorData data;
			bool flag = this._scriptNameDic.TryGetValue(scriptName, out data);
			if (flag)
			{
				File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
			}
		}

		// Token: 0x06004C12 RID: 19474 RVA: 0x0023E0A8 File Offset: 0x0023C2A8
		private string GetGlobalScriptFullPath(string scriptName)
		{
			return Path.Combine(ModManager.GetModEventGlobalScriptsFolder(), scriptName + ".tws");
		}

		// Token: 0x06004C13 RID: 19475 RVA: 0x0023E0D0 File Offset: 0x0023C2D0
		protected override void InternalInit()
		{
			this._cancelBtn = this.btnCancel;
			this._newScriptBtn = this.btnNewScript;
			this._scriptsScroll = this.scriptScroll;
			this._scriptTemplate = this.goScriptTemplate;
			this._scriptNameEditObj = this.goScriptNameEdit;
			this._scriptNameEditObj.gameObject.SetActive(false);
			this._scriptNameInputField = this._scriptNameEditObj.GetComponentInChildren<TMP_InputField>();
			this._scriptNameEditConfirmBtn = this.btnNameEdiConfirm;
			this._scriptNameEditCancelBtn = this.btnNameEdiCancel;
			this._cancelBtn.ClearAndAddListener(new Action(this.OnCancel));
			this._newScriptBtn.ClearAndAddListener(new Action(this.OnCreateNewScript));
			this._scriptNameEditCancelBtn.ClearAndAddListener(delegate
			{
				this._scriptNameEditObj.SetActive(false);
			});
		}

		// Token: 0x06004C14 RID: 19476 RVA: 0x0023E19D File Offset: 0x0023C39D
		public override void Show()
		{
			base.gameObject.SetActive(true);
			this.ClearScripts();
			this.LoadAllGlobalScripts(true);
		}

		// Token: 0x06004C15 RID: 19477 RVA: 0x0023E1BC File Offset: 0x0023C3BC
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x040034C8 RID: 13512
		public static EventEditorGlobalScriptBrowser Instance;

		// Token: 0x040034C9 RID: 13513
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x040034CA RID: 13514
		[SerializeField]
		private CScrollRect scriptScroll;

		// Token: 0x040034CB RID: 13515
		[SerializeField]
		private GameObject goScriptTemplate;

		// Token: 0x040034CC RID: 13516
		[SerializeField]
		private CButton btnNewScript;

		// Token: 0x040034CD RID: 13517
		[SerializeField]
		private GameObject goScriptNameEdit;

		// Token: 0x040034CE RID: 13518
		[SerializeField]
		private CButton btnNameEdiCancel;

		// Token: 0x040034CF RID: 13519
		[SerializeField]
		private CButton btnNameEdiConfirm;

		// Token: 0x040034D0 RID: 13520
		private CButton _confirmBtn;

		// Token: 0x040034D1 RID: 13521
		private CButton _cancelBtn;

		// Token: 0x040034D2 RID: 13522
		private CButton _newScriptBtn;

		// Token: 0x040034D3 RID: 13523
		private CScrollRect _scriptsScroll;

		// Token: 0x040034D4 RID: 13524
		private GameObject _scriptTemplate;

		// Token: 0x040034D5 RID: 13525
		private GameObject _scriptNameEditObj;

		// Token: 0x040034D6 RID: 13526
		private TMP_InputField _scriptNameInputField;

		// Token: 0x040034D7 RID: 13527
		private CButton _scriptNameEditConfirmBtn;

		// Token: 0x040034D8 RID: 13528
		private CButton _scriptNameEditCancelBtn;

		// Token: 0x040034D9 RID: 13529
		private readonly Dictionary<string, GlobalScriptEditorData> _scriptNameDic = new Dictionary<string, GlobalScriptEditorData>();

		// Token: 0x040034DA RID: 13530
		private readonly Dictionary<string, GameObject> _scriptObjDic = new Dictionary<string, GameObject>();

		// Token: 0x040034DB RID: 13531
		private readonly Dictionary<string, GlobalScriptEditorData> _scriptGuidDic = new Dictionary<string, GlobalScriptEditorData>();
	}
}
