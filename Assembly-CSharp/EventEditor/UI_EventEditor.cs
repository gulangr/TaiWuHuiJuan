using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FrameWork;
using FrameWork.ExternalTexture;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000656 RID: 1622
	public class UI_EventEditor : UIBase
	{
		// Token: 0x06004D45 RID: 19781 RVA: 0x002478CC File Offset: 0x00245ACC
		public override void OnInit(ArgumentBox argsBox)
		{
			this.simulateEnv.SetActive(UI_EventEditor.IsDev);
			PoolManager.CleanPool();
			EventEditorNotify.Init(this.eventEditorNotify);
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			model.OnModelDataReady = new Action(this.InitSubPages);
			model.OnCompileError = new Action<Exception>(this.OnCompileError);
			bool needRefresh = !model.DataReady;
			this.fromAdventureRemake = false;
			bool flag = argsBox != null;
			if (flag)
			{
				argsBox.Get("FromAdventureRemake", out this.fromAdventureRemake);
			}
			bool flag2 = needRefresh;
			if (flag2)
			{
				model.LoadEventCore();
			}
			else
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, new Action(this.InitSubPages));
			}
			string eventId;
			bool flag3 = argsBox != null && argsBox.Get("EventId", out eventId);
			if (flag3)
			{
				UIElement element = this.Element;
				element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
				{
					this.StartCoroutine(this.InitSelectEvent(eventId));
				}));
			}
		}

		// Token: 0x06004D46 RID: 19782 RVA: 0x002479CD File Offset: 0x00245BCD
		private IEnumerator InitSelectEvent(string eventId)
		{
			yield return new WaitUntil(() => SingletonObject.getInstance<EventEditorModel>().DataReady);
			EventEditorData eventTable = SingletonObject.getInstance<EventEditorModel>().GetEvent(eventId);
			bool flag = eventTable != null;
			if (flag)
			{
				EventEditorEventDetail.Instance.EditEvent(eventTable);
				EventEditorEventList.Instance.Select(eventId);
				bool flag2 = !TaskControlPanel.Instance.isEventEditorShow;
				if (flag2)
				{
					TaskControlPanel.Instance.OnEventEditorClick();
				}
			}
			yield break;
		}

		// Token: 0x06004D47 RID: 19783 RVA: 0x002479E4 File Offset: 0x00245BE4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "BtnMainMenu"))
			{
				if (!(a == "HideTextureSelect"))
				{
					if (a == "HideInput")
					{
						EventEditorInput.Instance.Hide();
					}
				}
				else
				{
					EventEditorTextureSelect.Instance.Hide();
				}
			}
			else
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ButtonInfos", this.GetMenuSheet());
				argBox.SetObject("TargetRect", btn.transform as RectTransform);
				argBox.SetObject("ButtonSize", new Vector2(300f, 44f));
				argBox.SetObject("ButtonTextHandler", UI_ButtonSheet.HandleButtonLabelContent);
				UIElement.ButtonSheet.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.ButtonSheet, true);
			}
		}

		// Token: 0x06004D48 RID: 19784 RVA: 0x00247AC8 File Offset: 0x00245CC8
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				string modEventTexturePath = ModManager.GetModEventTexturesFolder();
				SingletonObject.getInstance<TextureCenter>().LoadTextureGroupFromPath<NameKeyTextureGroup>(ModManager.GetWorkingModName(), modEventTexturePath);
			}
		}

		// Token: 0x06004D49 RID: 19785 RVA: 0x00247AF4 File Offset: 0x00245CF4
		private void Update()
		{
			this.CheckAutoSave();
		}

		// Token: 0x06004D4A RID: 19786 RVA: 0x00247B00 File Offset: 0x00245D00
		private void InitSubPages()
		{
			EventGroupTreeView.Init(this.eventGroupTreeView);
			EventEditorInput.Init(this.eventInput);
			EventEditorNotes.Init(this.notePanel);
			EventEditorSelectJumpEvent.Init(this.multiJumpEventSelector);
			EventEditorEventPreview.Init(this.eventPreview);
			EventEditorEventList.Init(this.eventList);
			TaskControlPanel.Init(this.taskControlPanel);
			TaskControlPanel.Instance.OnEventEditorClick();
			EventEditorAddEventGroup.Init(this.eventEditorAddEventGroup);
			EventEditorSwitchMod.Init(this.eventEditorSwitchMod);
			EventEditorScript.Init(this.eventEditorScript);
			OptionConsumeEditor.Init(this.optionConsumeEditor);
			EventBoolStateEditor.Init(this.eventBoolStateEditor);
			EventEditorGlobalScriptBrowser.Init(this.globalScriptBrowser);
			EventEditorSearchInstruction.Init(this.eventEditorSearchInstruction);
			EventEditorSettingsSubPage.Init(this.settings);
			EventEditorCompileErrorTip.Init(this.eventEditorCompileErrorTip);
			this._autoSave = (PlayerPrefs.GetInt("EventEditorAutoSave", 1) == 1);
		}

		// Token: 0x06004D4B RID: 19787 RVA: 0x00247BF0 File Offset: 0x00245DF0
		private void CheckAutoSave()
		{
			bool autoSave = this._autoSave;
			if (autoSave)
			{
				bool flag = Time.realtimeSinceStartup - this._tmLastAutoSave > this.AutoSaveDelay;
				if (flag)
				{
					this._tmLastAutoSave = Time.realtimeSinceStartup;
					EventEditorEventDetail.Instance.OnSaveEvent();
				}
			}
		}

		// Token: 0x06004D4C RID: 19788 RVA: 0x00247C3C File Offset: 0x00245E3C
		private void SwitchAutoSave()
		{
			this._autoSave = !this._autoSave;
			int saveCode = this._autoSave ? 1 : 0;
			PlayerPrefs.SetInt("EventEditorAutoSave", saveCode);
		}

		// Token: 0x06004D4D RID: 19789 RVA: 0x00247C74 File Offset: 0x00245E74
		private List<SheetButtonInfo> GetMenuSheet()
		{
			List<SheetButtonInfo> buttons = new List<SheetButtonInfo>();
			bool flag = SingletonObject.getInstance<EventEditorModel>().EventGroupInfoDic.Count > 0 && EventGroupTreeView.Instance.EditingEventGroup != null;
			if (flag)
			{
				buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_CreateEvent), new Action(this.OnCreateNewEvent), true, ""));
				buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_SearchInstruction), new Action(this.OnSearchScriptInGroup), true, ""));
			}
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_AddEventGroup), new Action(UI_EventEditor.OnOpenAddEventGroup), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_UpdateEventGroup), new Action(this.OnUpdateEventGroupForMod), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_SwitchWorkSpace), new Action(UI_EventEditor.OnSwitchWorkSpaceForMod), true, ""));
			buttons.Add(this._autoSave ? new SheetButtonInfo("<color=green>√</color>  " + LocalStringManager.Get(LanguageKey.UI_EventEditor_AutoSave), new Action(this.SwitchAutoSave), true, "") : new SheetButtonInfo("  " + LocalStringManager.Get(LanguageKey.UI_EventEditor_AutoSave), new Action(this.SwitchAutoSave), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_WebAPI_Document), new Action(this.OpenOnlineApiDocument), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_Download_Example), new Action(UI_EventEditor.OnDownLoadExample), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_Publish), new Action(this.OnPublish), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_EditConfigTemplate), new Action(UI_EventEditor.OnEditCustomConfig), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_Open_CustomConfig_Directory), new Action(this.OnOpenCustomConfigDirectory), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_Open_EventTexturesDirectory), new Action(this.OnOpenEventTextures), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_OpenGlobalScriptsList), new Action(UI_EventEditor.OnOpenGlobalScriptBrowser), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.UI_EventEditor_CompileGlobalScripts), new Action(this.OnCompileGlobalScripts), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_EventEditor_Open_EventDirectory), new Action(UI_EventEditor.OnOpenModFolder), true, ""));
			buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_Settings), new Action(UI_EventEditor.OnOpenSettingsPage), true, ""));
			bool flag2 = this.fromAdventureRemake;
			if (flag2)
			{
				buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_BackToAdventureRemakeEditor), new Action(this.BackToAdventureRemakeEditor), true, ""));
			}
			else
			{
				buttons.Add(new SheetButtonInfo(LocalStringManager.Get(LanguageKey.LK_BackToMainMenu), new Action(this.BackToMainMenu), true, ""));
			}
			return buttons;
		}

		// Token: 0x06004D4E RID: 19790 RVA: 0x00247FD0 File Offset: 0x002461D0
		private static void OnOpenGlobalScriptBrowser()
		{
			Debug.Log("Open EventEditorScriptBrowser");
			EventEditorGlobalScriptBrowser.Instance.Show();
		}

		// Token: 0x06004D4F RID: 19791 RVA: 0x00247FE9 File Offset: 0x002461E9
		private static void OnOpenSettingsPage()
		{
			EventEditorSettingsSubPage.Instance.Show();
		}

		// Token: 0x06004D50 RID: 19792 RVA: 0x00247FF7 File Offset: 0x002461F7
		private void BackToMainMenu()
		{
			UIManager.Instance.HideUI(UIElement.ButtonSheet);
			UIManager.Instance.ChangeToUI(UIElement.MainMenu);
			this.Element.Destroy();
		}

		// Token: 0x06004D51 RID: 19793 RVA: 0x00248026 File Offset: 0x00246226
		private void BackToAdventureRemakeEditor()
		{
			UIManager.Instance.HideUI(UIElement.EventEditor);
		}

		// Token: 0x06004D52 RID: 19794 RVA: 0x00248039 File Offset: 0x00246239
		private void OpenOnlineApiDocument()
		{
			Application.OpenURL("https://mod-doc.conchship.com.cn/html/177bf7bc-5b37-c84b-d993-475bee320729.htm");
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x00248048 File Offset: 0x00246248
		private void OnSearchScriptInGroup()
		{
			EventGroupData group = EventGroupTreeView.Instance.EditingEventGroup;
			bool flag = group != null;
			if (flag)
			{
				EventEditorSearchInstruction.Instance.Show(group.Key);
			}
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x0024807C File Offset: 0x0024627C
		private void OnCompileError(Exception e)
		{
			bool flag = e != null;
			if (flag)
			{
				EventEditorCompileErrorTip.Instance.Show(e);
			}
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x002480A0 File Offset: 0x002462A0
		private void OnCreateNewEvent()
		{
			EventEditorData newEvent = SingletonObject.getInstance<EventEditorModel>().CreateNewEvent();
			bool flag = EventGroupTreeView.Instance.EditingEventGroup != null;
			if (flag)
			{
				newEvent.EventGroup = EventGroupTreeView.Instance.EditingEventGroup.Key;
				EventGroupTreeView.Instance.EditingEventGroup.AddEvent(newEvent, true);
			}
			EventEditorEventDetail.Instance.EditEvent(newEvent);
			EventEditorEventDetail.Instance.OnSaveEvent();
			bool flag2 = !TaskControlPanel.Instance.isEventEditorShow;
			if (flag2)
			{
				TaskControlPanel.Instance.OnEventEditorClick();
			}
			else
			{
				EventEditorEventList.Instance.Show();
			}
		}

		// Token: 0x06004D56 RID: 19798 RVA: 0x00248133 File Offset: 0x00246333
		private void OnCompileGlobalScripts()
		{
			SingletonObject.getInstance<EventEditorModel>().CompileGlobalScripts();
		}

		// Token: 0x06004D57 RID: 19799 RVA: 0x00248141 File Offset: 0x00246341
		public void OnPublish()
		{
			GEvent.AddOneShot(ModEditorEvents.EventCompileComplete, new GEvent.Callback(this.OnCompileComplete));
			SingletonObject.getInstance<EventEditorModel>().ExportAllEventGroup(true, false);
		}

		// Token: 0x06004D58 RID: 19800 RVA: 0x0024816C File Offset: 0x0024636C
		private void OnCompileComplete(ArgumentBox argBox)
		{
			string configFilePath = SingletonObject.getInstance<EventEditorModel>().PublishCurrentModEventCore();
			SingletonObject.getInstance<EventEditorModel>().CreateLocalTestMod(ModManager.GetPublishRootPath());
			EventEditorNotify.Instance.SetNotifyAndShow(LocalStringManager.Get(LanguageKey.LK_EventEditor_Publish_Steps));
			UI_EventEditor.OpenCSharpCodeFile(configFilePath);
		}

		// Token: 0x06004D59 RID: 19801 RVA: 0x002481B4 File Offset: 0x002463B4
		public static void OpenCSharpCodeFile(string filePath)
		{
			bool flag = !File.Exists(filePath);
			if (!flag)
			{
				string editorPath = PlayerPrefs.GetString("EventEditorVsCodeExePath", string.Empty);
				bool flag2 = !string.IsNullOrEmpty(editorPath) && File.Exists(editorPath);
				if (flag2)
				{
					try
					{
						ProcessStartInfo processInfo = new ProcessStartInfo(editorPath, "\"" + filePath + "\"");
						Process.Start(processInfo);
					}
					catch (Exception e)
					{
						Debug.LogError(string.Format("Unable to open file at {0} with editor at {1}.\n{2}", filePath, editorPath, e));
						Process.Start(filePath);
					}
				}
				else
				{
					Process.Start(filePath);
				}
			}
		}

		// Token: 0x06004D5A RID: 19802 RVA: 0x00248258 File Offset: 0x00246458
		private void OnExportCSFiles()
		{
			Export.ExportEventCsFiles();
		}

		// Token: 0x06004D5B RID: 19803 RVA: 0x00248264 File Offset: 0x00246464
		private void OnUpdateEventGroupForMod()
		{
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			model.OnModelDataReady = delegate()
			{
				EventGroupTreeView.Instance.Show();
				EventEditorData curEvent = EventEditorEventDetail.Instance.CurEvent;
				bool flag = curEvent != null;
				if (flag)
				{
					EventGroupData groupData = model.GetGroupData(curEvent.EventGroup);
					bool flag2 = groupData == null;
					if (flag2)
					{
						string guid = curEvent.EventGuid;
						model.DeleteEvent(guid);
					}
				}
			};
			model.LoadEventCore();
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x002482A6 File Offset: 0x002464A6
		private static void OnOpenAddEventGroup()
		{
			EventEditorAddEventGroup.Instance.Show();
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x002482B4 File Offset: 0x002464B4
		private static void ExportInternalEventConfigLuaFile()
		{
			EnumDataUpdater.DoExportToLuaWithEventGroup();
		}

		// Token: 0x06004D5E RID: 19806 RVA: 0x002482BD File Offset: 0x002464BD
		private static void OnOpenModFolder()
		{
			Process.Start("explorer.exe", "\"" + ModManager.GetModEventSaveCore().FixPath() + "\"");
		}

		// Token: 0x06004D5F RID: 19807 RVA: 0x002482E4 File Offset: 0x002464E4
		private void OnEnable()
		{
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			model.ExportEventGroupHashset.Clear();
		}

		// Token: 0x06004D60 RID: 19808 RVA: 0x00248304 File Offset: 0x00246504
		private static void OnSwitchWorkSpaceForMod()
		{
			EventEditorSwitchMod.Instance.Show();
		}

		// Token: 0x06004D61 RID: 19809 RVA: 0x00248312 File Offset: 0x00246512
		private static void OnDownLoadExample()
		{
			Application.OpenURL("https://file.conchship.com.cn/event/ConchShipEventEditorBootCamp.zip");
		}

		// Token: 0x06004D62 RID: 19810 RVA: 0x00248320 File Offset: 0x00246520
		private static void OnEditCustomConfig()
		{
			string templateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/CustomConfigTemplate.template");
			string content = File.ReadAllText(templateFilePath);
			string editConfigFolder = ModManager.GetModEditingConfigFolder();
			int counter = 0;
			string autoCreateFilePath;
			bool flag2;
			do
			{
				autoCreateFilePath = Path.Combine(editConfigFolder, string.Format("custom_config_{0}.lua", ++counter));
				bool flag = !File.Exists(autoCreateFilePath);
				if (flag)
				{
					break;
				}
				flag2 = (counter >= 65535);
			}
			while (!flag2);
			File.WriteAllText(autoCreateFilePath, content);
			UI_EventEditor.OpenCSharpCodeFile(autoCreateFilePath);
			EventEditorNotify.Instance.SetNotifyAndShow(LocalStringManager.Get(LanguageKey.LK_EventEditor_EditConfigTemplate_Guide));
		}

		// Token: 0x06004D63 RID: 19811 RVA: 0x002483BD File Offset: 0x002465BD
		private void OnOpenCustomConfigDirectory()
		{
			Process.Start("explorer.exe", "\"" + ModManager.GetModEditingConfigFolder().FixPath() + "\"");
		}

		// Token: 0x06004D64 RID: 19812 RVA: 0x002483E4 File Offset: 0x002465E4
		private void OnOpenEventTextures()
		{
			Process.Start("explorer.exe", "\"" + ModManager.GetModEventTexturesFolder().FixPath() + "\"");
		}

		// Token: 0x0400359C RID: 13724
		[SerializeField]
		private GameObject simulateEnv;

		// Token: 0x0400359D RID: 13725
		[SerializeField]
		private EventGroupTreeView eventGroupTreeView;

		// Token: 0x0400359E RID: 13726
		[SerializeField]
		private TaskControlPanel taskControlPanel;

		// Token: 0x0400359F RID: 13727
		[SerializeField]
		private EventEditorEventPreview eventPreview;

		// Token: 0x040035A0 RID: 13728
		[SerializeField]
		private EventEditorInput eventInput;

		// Token: 0x040035A1 RID: 13729
		[SerializeField]
		private EventEditorEventList eventList;

		// Token: 0x040035A2 RID: 13730
		[SerializeField]
		private EventEditorTextureSelect eventTextureSelect;

		// Token: 0x040035A3 RID: 13731
		[SerializeField]
		private EventEditorNotes notePanel;

		// Token: 0x040035A4 RID: 13732
		[SerializeField]
		private EventEditorSelectJumpEvent multiJumpEventSelector;

		// Token: 0x040035A5 RID: 13733
		[SerializeField]
		private EventEditorAddEventGroup eventEditorAddEventGroup;

		// Token: 0x040035A6 RID: 13734
		[SerializeField]
		private EventEditorSwitchMod eventEditorSwitchMod;

		// Token: 0x040035A7 RID: 13735
		[SerializeField]
		private EventEditorNotify eventEditorNotify;

		// Token: 0x040035A8 RID: 13736
		[SerializeField]
		private EventEditorScript eventEditorScript;

		// Token: 0x040035A9 RID: 13737
		[SerializeField]
		private OptionConsumeEditor optionConsumeEditor;

		// Token: 0x040035AA RID: 13738
		[SerializeField]
		private EventEditorGlobalScriptBrowser globalScriptBrowser;

		// Token: 0x040035AB RID: 13739
		[SerializeField]
		private EventEditorSearchInstruction eventEditorSearchInstruction;

		// Token: 0x040035AC RID: 13740
		[SerializeField]
		private EventEditorSettingsSubPage settings;

		// Token: 0x040035AD RID: 13741
		[SerializeField]
		private EventEditorCompileErrorTip eventEditorCompileErrorTip;

		// Token: 0x040035AE RID: 13742
		[SerializeField]
		private EventBoolStateEditor eventBoolStateEditor;

		// Token: 0x040035AF RID: 13743
		private static UI_EventEditor.EExportActionType _prevExportActionType;

		// Token: 0x040035B0 RID: 13744
		private bool _autoSave;

		// Token: 0x040035B1 RID: 13745
		private float _tmLastAutoSave;

		// Token: 0x040035B2 RID: 13746
		public float AutoSaveDelay = 60f;

		// Token: 0x040035B3 RID: 13747
		public bool fromAdventureRemake;

		// Token: 0x040035B4 RID: 13748
		public static readonly bool IsDev;

		// Token: 0x02001A92 RID: 6802
		private enum EExportActionType
		{
			// Token: 0x0400B681 RID: 46721
			None,
			// Token: 0x0400B682 RID: 46722
			ExportCompileAll,
			// Token: 0x0400B683 RID: 46723
			ExportAll,
			// Token: 0x0400B684 RID: 46724
			ExportCompileFiltered
		}
	}
}
