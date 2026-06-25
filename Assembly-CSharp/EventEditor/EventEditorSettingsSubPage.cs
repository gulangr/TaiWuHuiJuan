using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x0200064C RID: 1612
	public class EventEditorSettingsSubPage : EventEditorSubPageBase
	{
		// Token: 0x06004CCE RID: 19662 RVA: 0x00244859 File Offset: 0x00242A59
		protected override void InternalInit()
		{
			this.btnCancel.ClearAndAddListener(new Action(this.OnCancel));
			this.InitPathSetting(this.goPathSettingTemp, LocalStringManager.Get(LanguageKey.UI_EventEditor_SetCodeEditorPath), "EventEditorVsCodeExePath");
		}

		// Token: 0x06004CCF RID: 19663 RVA: 0x00244890 File Offset: 0x00242A90
		private void InitScriptDebugSettings(EventScriptRuntimeSettings settings)
		{
			int count = EventScriptType.Instance.Count;
			for (int i = 0; i < this.scriptTypes.Count; i++)
			{
				GameObject scriptTypeItemGo = this.scriptTypes[i];
				bool flag = i >= count;
				if (flag)
				{
					scriptTypeItemGo.gameObject.SetActive(false);
				}
				else
				{
					EventScriptTypeItem config = EventScriptType.Instance[i];
					EventEditorSettingScriptTypesItemInfo info = scriptTypeItemGo.GetComponent<EventEditorSettingScriptTypesItemInfo>();
					info.txtMeshLabelOn.text = config.Name;
					info.txtMeshLabelOff.text = config.Name;
					CToggle toggle = info.toggle;
					toggle.isOn = settings.LogScriptTypes[i];
					scriptTypeItemGo.gameObject.SetActive(true);
				}
			}
			this.logMonitoredScriptsOnly.isOn = settings.LogMonitoredScriptsOnly;
		}

		// Token: 0x06004CD0 RID: 19664 RVA: 0x0024496C File Offset: 0x00242B6C
		private void InitPathSetting(GameObject goTemp, string labelString, string keyString)
		{
			EventEditorSettingsSubPage.<>c__DisplayClass8_0 CS$<>8__locals1 = new EventEditorSettingsSubPage.<>c__DisplayClass8_0();
			CS$<>8__locals1.keyString = keyString;
			EventEditorSettingPathSettingTempInfo tempInfo = goTemp.GetComponent<EventEditorSettingPathSettingTempInfo>();
			TextMeshProUGUI label = tempInfo.txtMeshLabel;
			CS$<>8__locals1.path = tempInfo.path;
			CButton browse = tempInfo.btnBrowse;
			label.text = labelString;
			string content = PlayerPrefs.GetString(CS$<>8__locals1.keyString, string.Empty);
			CS$<>8__locals1.path.onValueChanged.RemoveAllListeners();
			CS$<>8__locals1.path.text = content;
			CS$<>8__locals1.path.onValueChanged.AddListener(new UnityAction<string>(CS$<>8__locals1.<InitPathSetting>g__SetPath|1));
			browse.ClearAndAddListener(delegate
			{
				OpenFileName openFileName = new OpenFileName();
				openFileName.structSize = Marshal.SizeOf<OpenFileName>(openFileName);
				openFileName.file = new string(new char[256]);
				openFileName.maxFile = openFileName.file.Length;
				openFileName.fileTitle = new string(new char[64]);
				openFileName.maxFileTitle = openFileName.fileTitle.Length;
				openFileName.initialDir = Application.dataPath;
				openFileName.title = "Set " + CS$<>8__locals1.keyString + " Path";
				openFileName.flags = 530440;
				bool unityOpenFileName = LocalDialog.GetUnityOpenFileName(openFileName);
				if (unityOpenFileName)
				{
					bool flag = File.Exists(openFileName.file);
					if (flag)
					{
						FileInfo fileInfo = new FileInfo(openFileName.file);
						string targetFolderDirectory = fileInfo.FullName;
						base.<InitPathSetting>g__SetPath|1(targetFolderDirectory);
						CS$<>8__locals1.path.SetTextWithoutNotify(targetFolderDirectory);
					}
				}
			});
		}

		// Token: 0x06004CD1 RID: 19665 RVA: 0x00244A14 File Offset: 0x00242C14
		private void OnCancel()
		{
			this.Hide();
			foreach (EventScriptTypeItem typeCfg in ((IEnumerable<EventScriptTypeItem>)EventScriptType.Instance))
			{
				CToggle toggle = this.scriptTypes[(int)typeCfg.TemplateId].GetComponent<EventEditorSettingScriptTypesItemInfo>().toggle;
				this._runtimeSettings.LogScriptTypes[(int)typeCfg.TemplateId] = toggle.isOn;
			}
			this._runtimeSettings.LogMonitoredScriptsOnly = this.logMonitoredScriptsOnly.isOn;
			SingletonObject.getInstance<EventEditorModel>().ScriptRuntimeSettings = this._runtimeSettings;
		}

		// Token: 0x06004CD2 RID: 19666 RVA: 0x00244AC0 File Offset: 0x00242CC0
		public override void Show()
		{
			this._runtimeSettings = SingletonObject.getInstance<EventEditorModel>().ScriptRuntimeSettings;
			this.InitScriptDebugSettings(this._runtimeSettings);
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004CD3 RID: 19667 RVA: 0x00244AED File Offset: 0x00242CED
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004CD4 RID: 19668 RVA: 0x00244AFD File Offset: 0x00242CFD
		public static void Init(EventEditorSettingsSubPage instance)
		{
			EventEditorSettingsSubPage.Instance = instance;
			EventEditorSettingsSubPage.Instance.InternalInit();
			EventEditorSettingsSubPage.Instance.Hide();
		}

		// Token: 0x0400353C RID: 13628
		public static EventEditorSettingsSubPage Instance;

		// Token: 0x0400353D RID: 13629
		[SerializeField]
		private List<GameObject> scriptTypes;

		// Token: 0x0400353E RID: 13630
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x0400353F RID: 13631
		[SerializeField]
		private GameObject goPathSettingTemp;

		// Token: 0x04003540 RID: 13632
		[SerializeField]
		private CToggle logMonitoredScriptsOnly;

		// Token: 0x04003541 RID: 13633
		private EventScriptRuntimeSettings _runtimeSettings;
	}
}
