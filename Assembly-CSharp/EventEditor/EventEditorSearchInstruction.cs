using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using EventEditor.EventScript;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200064A RID: 1610
	public class EventEditorSearchInstruction : EventEditorSubPageBase
	{
		// Token: 0x06004CB9 RID: 19641 RVA: 0x00243F9F File Offset: 0x0024219F
		public static void Init(EventEditorSearchInstruction instance)
		{
			EventEditorSearchInstruction.Instance = instance;
			EventEditorSearchInstruction.Instance.InternalInit();
			EventEditorSearchInstruction.Instance.Hide();
		}

		// Token: 0x06004CBA RID: 19642 RVA: 0x00243FC0 File Offset: 0x002421C0
		public void Show(string group)
		{
			bool flag = this._scriptData != null || this._group != group;
			if (flag)
			{
				this._searchResults.Clear();
				this._scriptData = null;
				this._group = group;
				this._searchResultScroll.UpdateData(0);
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004CBB RID: 19643 RVA: 0x00244020 File Offset: 0x00242220
		public void Show(EventScriptEditorData scriptData)
		{
			bool flag = this._scriptData != scriptData || this._group != null;
			if (flag)
			{
				this._searchResults.Clear();
				this._scriptData = scriptData;
				this._group = null;
				this._searchResultScroll.UpdateData(0);
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004CBC RID: 19644 RVA: 0x00244080 File Offset: 0x00242280
		private void RenderSearchResult(int index, GameObject goSearchResTemp)
		{
			EventEditorSearchInstructionSearchResultTempInfo searchResTempInfo = goSearchResTemp.GetComponent<EventEditorSearchInstructionSearchResultTempInfo>();
			TextMeshProUGUI displayText = searchResTempInfo.txtMeshDisplayText;
			displayText.text = this._searchResults[index].Item1;
			CButton btn = searchResTempInfo.btnGoto;
			btn.ClearAndAddListener(delegate
			{
				ValueTuple<string, EventScriptId, int> searchResult = this._searchResults[index];
				bool flag = this._scriptData != null;
				if (flag)
				{
					this.ShowScript(index);
				}
				else
				{
					this.ShowScript(searchResult.Item2, searchResult.Item3);
				}
			});
		}

		// Token: 0x06004CBD RID: 19645 RVA: 0x002440E8 File Offset: 0x002422E8
		protected override void InternalInit()
		{
			this._searchResults = new List<ValueTuple<string, EventScriptId, int>>();
			this._inputText = this.inputTxt;
			this._searchResultScroll = this.searchResultScroll;
			CButton searchByFuncNameBtn = this.btnSearchFuncName;
			CButton searchByArgBtn = this.btnSearchParam;
			searchByFuncNameBtn.ClearAndAddListener(new Action(this.SearchByFuncName));
			searchByArgBtn.ClearAndAddListener(new Action(this.SearchByArg));
			this._searchResultScroll.OnItemRender += this.RenderSearchResult;
		}

		// Token: 0x06004CBE RID: 19646 RVA: 0x00244165 File Offset: 0x00242365
		public override void Show()
		{
		}

		// Token: 0x06004CBF RID: 19647 RVA: 0x00244168 File Offset: 0x00242368
		public override void Hide()
		{
			this._group = null;
			this._scriptData = null;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004CC0 RID: 19648 RVA: 0x00244188 File Offset: 0x00242388
		private void SearchByFuncName()
		{
			this._searchResults.Clear();
			bool flag = this._scriptData != null;
			if (flag)
			{
				List<ValueTuple<int, string>> list = new List<ValueTuple<int, string>>();
				this._scriptData.FindInstructionsByFuncName(this._inputText.text, list);
				foreach (ValueTuple<int, string> valueTuple in list)
				{
					int line = valueTuple.Item1;
					string value = valueTuple.Item2;
					this._searchResults.Add(new ValueTuple<string, EventScriptId, int>(string.Format("{0} at line {1}", value.SetColor(Color.yellow), line), EventScriptId.Invalid, line));
				}
			}
			else
			{
				bool flag2 = !this._group.IsNullOrEmpty();
				if (flag2)
				{
					Dictionary<EventScriptId, List<ValueTuple<int, string>>> dictionary = new Dictionary<EventScriptId, List<ValueTuple<int, string>>>();
					SingletonObject.getInstance<EventEditorModel>().SearchInstructionInGroup(this._group, this._inputText.text, true, dictionary);
					foreach (KeyValuePair<EventScriptId, List<ValueTuple<int, string>>> keyValuePair in dictionary)
					{
						EventScriptId eventScriptId;
						List<ValueTuple<int, string>> list3;
						keyValuePair.Deconstruct(out eventScriptId, out list3);
						EventScriptId scriptId = eventScriptId;
						List<ValueTuple<int, string>> list2 = list3;
						foreach (ValueTuple<int, string> valueTuple2 in list2)
						{
							int line2 = valueTuple2.Item1;
							string value2 = valueTuple2.Item2;
							this._searchResults.Add(new ValueTuple<string, EventScriptId, int>(string.Format("{0} at {1} line {2}", value2.SetColor(Color.yellow), scriptId, line2), scriptId, line2));
						}
					}
				}
			}
			this._searchResultScroll.UpdateData(this._searchResults.Count);
		}

		// Token: 0x06004CC1 RID: 19649 RVA: 0x00244380 File Offset: 0x00242580
		private void SearchByArg()
		{
			this._searchResults.Clear();
			bool flag = this._scriptData != null;
			if (flag)
			{
				List<ValueTuple<int, string>> list = new List<ValueTuple<int, string>>();
				this._scriptData.FindInstructionsByParameter(this._inputText.text, list);
				foreach (ValueTuple<int, string> valueTuple in list)
				{
					int line = valueTuple.Item1;
					string value = valueTuple.Item2;
					this._searchResults.Add(new ValueTuple<string, EventScriptId, int>(string.Format("{0} at line {1}", value.SetColor(Color.yellow), line), EventScriptId.Invalid, line));
				}
			}
			else
			{
				bool flag2 = !this._group.IsNullOrEmpty();
				if (flag2)
				{
					Dictionary<EventScriptId, List<ValueTuple<int, string>>> dictionary = new Dictionary<EventScriptId, List<ValueTuple<int, string>>>();
					SingletonObject.getInstance<EventEditorModel>().SearchInstructionInGroup(this._group, this._inputText.text, false, dictionary);
					foreach (KeyValuePair<EventScriptId, List<ValueTuple<int, string>>> keyValuePair in dictionary)
					{
						EventScriptId eventScriptId;
						List<ValueTuple<int, string>> list3;
						keyValuePair.Deconstruct(out eventScriptId, out list3);
						EventScriptId scriptId = eventScriptId;
						List<ValueTuple<int, string>> list2 = list3;
						foreach (ValueTuple<int, string> valueTuple2 in list2)
						{
							int line2 = valueTuple2.Item1;
							string value2 = valueTuple2.Item2;
							this._searchResults.Add(new ValueTuple<string, EventScriptId, int>(string.Format("{0} at {1} line {2}", value2.SetColor(Color.yellow), scriptId, line2), scriptId, line2));
						}
					}
				}
			}
			this._searchResultScroll.UpdateData(this._searchResults.Count);
		}

		// Token: 0x06004CC2 RID: 19650 RVA: 0x00244578 File Offset: 0x00242778
		public void ShowScript(EventScriptId id, int line)
		{
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			EventScriptEditorData script;
			bool flag = !model.TryGetEventScript(id, out script);
			if (!flag)
			{
				EventScriptRef scriptRef = id.EventScriptRef;
				string eventGuid = scriptRef.Guid.ToString();
				string optionGuid = scriptRef.HasSubGuid ? scriptRef.SubGuid.ToString() : string.Empty;
				EventEditorEventList.Instance.Select(eventGuid);
				EventEditorScript.Instance.Hide();
				EventEditorScript.Instance.Show(script, delegate(EventScriptEditorData editingScript)
				{
					EventEditorData eventTable = model.GetEvent(eventGuid);
					string saveDir = model.EnsureEventScriptPath(eventTable);
					bool flag2 = !Directory.Exists(saveDir);
					if (!flag2)
					{
						string scriptUrl = Path.Combine(saveDir, id.GetFileName() + ".tws");
						model.SaveEventScript(eventGuid, scriptUrl, editingScript);
					}
				}, id.Type, eventGuid, optionGuid);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
				{
					EventEditorScript.Instance.GotoLine(line);
				});
			}
		}

		// Token: 0x06004CC3 RID: 19651 RVA: 0x00244671 File Offset: 0x00242871
		private void ShowScript(int line)
		{
			EventEditorScript.Instance.GotoLine(line);
		}

		// Token: 0x0400352B RID: 13611
		public static EventEditorSearchInstruction Instance;

		// Token: 0x0400352C RID: 13612
		[SerializeField]
		private CButton btnSearchFuncName;

		// Token: 0x0400352D RID: 13613
		[SerializeField]
		private CButton btnSearchParam;

		// Token: 0x0400352E RID: 13614
		[SerializeField]
		private TMP_InputField inputTxt;

		// Token: 0x0400352F RID: 13615
		[SerializeField]
		private InfinityScroll searchResultScroll;

		// Token: 0x04003530 RID: 13616
		private TMP_InputField _inputText;

		// Token: 0x04003531 RID: 13617
		private string _group;

		// Token: 0x04003532 RID: 13618
		private EventScriptEditorData _scriptData;

		// Token: 0x04003533 RID: 13619
		private InfinityScroll _searchResultScroll;

		// Token: 0x04003534 RID: 13620
		[TupleElementNames(new string[]
		{
			"displayText",
			"scriptId",
			"line"
		})]
		private List<ValueTuple<string, EventScriptId, int>> _searchResults;
	}
}
