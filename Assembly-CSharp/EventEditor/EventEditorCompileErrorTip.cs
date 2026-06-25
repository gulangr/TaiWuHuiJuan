using System;
using System.Collections.Generic;
using EventScript;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventEditor
{
	// Token: 0x0200063E RID: 1598
	public class EventEditorCompileErrorTip : EventEditorSubPageBase
	{
		// Token: 0x06004B77 RID: 19319 RVA: 0x00237865 File Offset: 0x00235A65
		public static void Init(EventEditorCompileErrorTip instance)
		{
			EventEditorCompileErrorTip.Instance = instance;
			EventEditorCompileErrorTip.Instance.InternalInit();
			EventEditorCompileErrorTip.Instance.Hide();
		}

		// Token: 0x06004B78 RID: 19320 RVA: 0x00237884 File Offset: 0x00235A84
		public void Show(Exception e)
		{
			base.gameObject.SetActive(true);
			Transform parent = this.goLineTemp.transform.parent;
			this.goLineTemp.gameObject.SetActive(true);
			for (int i = 1; i < parent.childCount; i++)
			{
				Object.Destroy(parent.GetChild(i).gameObject);
			}
			EventEditorCompileErrorTipLineTempInfo typeErrorInfo = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
			this.GetLabel(typeErrorInfo).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_ErrorType);
			this.GetContent(typeErrorInfo).text = e.Message;
			EventScriptCompilerException compilerException = e as EventScriptCompilerException;
			bool flag = compilerException != null;
			if (flag)
			{
				bool flag2 = compilerException.HasArgInfo();
				if (flag2)
				{
					EventEditorCompileErrorTipLineTempInfo argErrorInfo = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					this.GetLabel(argErrorInfo).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_ErrorArg);
					this.GetContent(argErrorInfo).gameObject.SetActive(false);
					EventEditorCompileErrorTipLineTempInfo argName = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					argName.gameObject.SetActive(true);
					this.GetLabel(argName).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_ArgName);
					this.GetContent(argName).text = compilerException.GetArgTypeName();
					this.GetIndent(argName).SetActive(true);
					EventEditorCompileErrorTipLineTempInfo argValue = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					this.GetLabel(argValue).text = LocalStringManager.Get(LanguageKey.UI_EventEidtor_ArgValue);
					this.GetContent(argValue).text = compilerException.GetArgTypeValue();
					this.GetIndent(argValue).SetActive(true);
					EventEditorCompileErrorTipLineTempInfo argIsExpression = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					this.GetLabel(argIsExpression).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_IsArgExpression);
					this.GetContent(argIsExpression).text = (compilerException.IsArgExpression() ? LocalStringManager.Get(LanguageKey.LK_Yes) : LocalStringManager.Get(LanguageKey.LK_No));
					this.GetIndent(argIsExpression).SetActive(true);
				}
				bool flag3 = compilerException.GetRowId() != -1;
				if (flag3)
				{
					EventEditorCompileErrorTipLineTempInfo errorLine = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					this.GetLabel(errorLine).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_ErrorLine);
					TMP_Text content = this.GetContent(errorLine);
					int rowId = compilerException.GetRowId();
					content.text = rowId.ToString();
				}
				EventScriptId? scriptId = compilerException.GetScriptId();
				bool flag4 = scriptId != null;
				if (flag4)
				{
					EventEditorCompileErrorTipLineTempInfo location = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					this.GetLabel(location).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_ScriptLocation);
					this.GetContent(location).gameObject.SetActive(false);
					EventScriptId script = scriptId.Value;
					EventEditorModel eventModel = SingletonObject.getInstance<EventEditorModel>();
					EventEditorCompileErrorTipLineTempInfo scriptType = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
					this.GetLabel(scriptType).text = LocalStringManager.Get(LanguageKey.LK_ScriptType);
					this.GetContent(scriptType).text = script.Type.ToString();
					this.GetIndent(scriptType).SetActive(true);
					bool flag5 = script.Type != 0;
					if (flag5)
					{
						string guid = script.EventScriptRef.Guid.ToString();
						EventEditorData targetEvent = eventModel.GetEvent(guid);
						bool hasSubGuid = script.EventScriptRef.HasSubGuid;
						if (hasSubGuid)
						{
							string subGuid = script.EventScriptRef.SubGuid.ToString();
							Dictionary<int, EventEditorData.Option> optionTable = targetEvent.Options;
							int order = -1;
							foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in optionTable)
							{
								int rowId;
								EventEditorData.Option option3;
								keyValuePair.Deconstruct(out rowId, out option3);
								int key = rowId;
								EventEditorData.Option option = option3;
								string optionGuid = option.Guid;
								bool flag6 = optionGuid == subGuid;
								if (flag6)
								{
									order = key;
									break;
								}
							}
							bool flag7 = order != -1;
							if (flag7)
							{
								EventEditorCompileErrorTipLineTempInfo option2 = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
								this.GetLabel(option2).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_Option);
								this.GetContent(option2).text = string.Format("{0} {1}", order, subGuid.SetColor(Color.cyan));
								this.GetIndent(option2).SetActive(true);
								this.GetBtn(option2).gameObject.SetActive(true);
								this.GetBtn(option2).ClearAndAddListener(delegate
								{
									GUIUtility.systemCopyBuffer = subGuid;
								});
							}
						}
						EventEditorCompileErrorTipLineTempInfo eventRefer = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
						this.GetLabel(eventRefer).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_Event);
						this.GetContent(eventRefer).text = targetEvent.EventName + " " + guid.SetColor(Color.cyan);
						this.GetBtn(eventRefer).gameObject.SetActive(true);
						this.GetBtn(eventRefer).ClearAndAddListener(delegate
						{
							GUIUtility.systemCopyBuffer = guid;
						});
						this.GetIndent(eventRefer).SetActive(true);
						EventEditorCompileErrorTipLineTempInfo eventGroupRefer = Object.Instantiate<GameObject>(this.goLineTemp, parent).GetComponent<EventEditorCompileErrorTipLineTempInfo>();
						this.GetLabel(eventGroupRefer).text = LocalStringManager.Get(LanguageKey.UI_EventEditor_EventGroup);
						EventGroupData eventGroup = eventModel.GetGroupDataByEventGuid(guid);
						this.GetContent(eventGroupRefer).text = eventGroup.Name + " " + eventGroup.Key.SetColor(Color.cyan);
						this.GetIndent(eventGroupRefer).SetActive(true);
					}
				}
				string exceptionType = (compilerException.InnerException != null) ? compilerException.InnerException.GetType().ToString() : compilerException.GetType().ToString();
				string stackTrace = (compilerException.InnerException != null) ? compilerException.InnerException.StackTrace : compilerException.StackTrace;
				this.txtMeshStackTrack.text = exceptionType + "\n" + stackTrace;
				this.btnGoTo.interactable = (scriptId != null);
				this.btnGoTo.ClearAndAddListener(delegate
				{
					bool flag8 = scriptId == null;
					if (!flag8)
					{
						int line = compilerException.GetRowId();
						bool flag9 = line == -1;
						if (flag9)
						{
							line = 0;
						}
						this.Hide();
						EventEditorSearchInstruction.Instance.ShowScript(scriptId.Value, line);
					}
				});
			}
			else
			{
				this.txtMeshStackTrack.text = string.Format("{0}\n{1}", e.GetType(), e.StackTrace);
				this.GetContent(typeErrorInfo).enableWordWrapping = true;
				this.GetContent(typeErrorInfo).transform.GetComponent<LayoutElement>().preferredWidth = 1300f;
				this.btnGoTo.interactable = false;
			}
			this.goLineTemp.SetActive(false);
		}

		// Token: 0x06004B79 RID: 19321 RVA: 0x00238048 File Offset: 0x00236248
		private TextMeshProUGUI GetLabel(EventEditorCompileErrorTipLineTempInfo tempInfo)
		{
			return tempInfo.txtMeshLabel;
		}

		// Token: 0x06004B7A RID: 19322 RVA: 0x00238060 File Offset: 0x00236260
		private TextMeshProUGUI GetContent(EventEditorCompileErrorTipLineTempInfo tempInfo)
		{
			return tempInfo.txtMeshContent;
		}

		// Token: 0x06004B7B RID: 19323 RVA: 0x00238078 File Offset: 0x00236278
		private GameObject GetIndent(EventEditorCompileErrorTipLineTempInfo tempInfo)
		{
			return tempInfo.goIndent;
		}

		// Token: 0x06004B7C RID: 19324 RVA: 0x00238090 File Offset: 0x00236290
		private CButton GetBtn(EventEditorCompileErrorTipLineTempInfo tempInfo)
		{
			return tempInfo.btn;
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x002380A8 File Offset: 0x002362A8
		protected override void InternalInit()
		{
		}

		// Token: 0x06004B7E RID: 19326 RVA: 0x002380AB File Offset: 0x002362AB
		public override void Show()
		{
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x002380AE File Offset: 0x002362AE
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x04003471 RID: 13425
		public static EventEditorCompileErrorTip Instance;

		// Token: 0x04003472 RID: 13426
		[SerializeField]
		private CButton btnGoTo;

		// Token: 0x04003473 RID: 13427
		[SerializeField]
		private TextMeshProUGUI txtMeshStackTrack;

		// Token: 0x04003474 RID: 13428
		[SerializeField]
		private GameObject goLineTemp;
	}
}
