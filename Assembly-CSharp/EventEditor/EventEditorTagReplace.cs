using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000650 RID: 1616
	public class EventEditorTagReplace : EventEditorSubPageBase
	{
		// Token: 0x06004CEA RID: 19690 RVA: 0x00244DD4 File Offset: 0x00242FD4
		public static void Init(EventEditorTagReplace instance)
		{
			EventEditorTagReplace.Instance = instance;
			EventEditorTagReplace.Instance.InternalInit();
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x00244DE8 File Offset: 0x00242FE8
		protected override void InternalInit()
		{
			this.toggleGroup.Init(-1);
			this.SetHide();
			this._replaceRulesList = new List<ValueTuple<string, string>>();
			GEvent.Add(ModEditorEvents.EditingEventChange, new GEvent.Callback(this.OnEditingEventChanged));
			this.replaceRulesScroll.OnItemRender += this.OnRuleItemRender;
			this.btnHidePanelTagReplace.ClearAndAddListener(new Action(this.SetHide));
			this.btnCopyRoleTag.ClearAndAddListener(new Action(this.OnCopyRoleTag));
			this.btnAddNewReplace.ClearAndAddListener(new Action(this.OnAddNewRule));
			this.btnStartReplace.ClearAndAddListener(new Action(this.OnExecuteReplace));
		}

		// Token: 0x06004CEC RID: 19692 RVA: 0x00244EA5 File Offset: 0x002430A5
		public override void Show()
		{
		}

		// Token: 0x06004CED RID: 19693 RVA: 0x00244EA8 File Offset: 0x002430A8
		public override void Hide()
		{
		}

		// Token: 0x06004CEE RID: 19694 RVA: 0x00244EAB File Offset: 0x002430AB
		public void SetShow()
		{
			this._isShowing = true;
			this.Refresh();
			base.gameObject.SetActive(true);
			base.transform.DOLocalMoveX(0f, 0.3f, false).SetEase(Ease.OutFlash);
		}

		// Token: 0x06004CEF RID: 19695 RVA: 0x00244EE7 File Offset: 0x002430E7
		public void SetHide()
		{
			this._isShowing = false;
			base.transform.DOLocalMoveX(1000f, 0.3f, false).SetEase(Ease.InFlash).OnComplete(delegate
			{
				base.gameObject.SetActive(false);
			});
		}

		// Token: 0x06004CF0 RID: 19696 RVA: 0x00244F20 File Offset: 0x00243120
		private void OnEditingEventChanged(ArgumentBox box)
		{
			bool flag = !this._isShowing;
			if (!flag)
			{
				this.Refresh();
			}
		}

		// Token: 0x06004CF1 RID: 19697 RVA: 0x00244F44 File Offset: 0x00243144
		private void Refresh()
		{
			EventEditorData eventData = EventEditorEventDetail.Instance.CurEvent;
			bool flag = eventData == null;
			if (flag)
			{
				this.SetAsDefault();
			}
			else
			{
				string eventGroup = eventData.EventGroup;
				string displayGroup;
				bool flag2 = !string.IsNullOrEmpty(eventGroup) && SingletonObject.getInstance<EventEditorModel>().EventGroupInfoDic.TryGetValue(eventGroup, out displayGroup);
				if (flag2)
				{
					this.txtMeshEventGroup.text = displayGroup;
				}
				else
				{
					this.txtMeshEventGroup.text = string.Empty;
				}
				this.txtMeshEventName.text = eventData.EventName;
				this.replaceRulesScroll.SetDataCount(this._replaceRulesList.Count);
			}
		}

		// Token: 0x06004CF2 RID: 19698 RVA: 0x00244FE2 File Offset: 0x002431E2
		private void SetAsDefault()
		{
			this.txtMeshEventGroup.text = string.Empty;
			this.txtMeshEventName.text = string.Empty;
			this.replaceRulesScroll.SetDataCount(0);
		}

		// Token: 0x06004CF3 RID: 19699 RVA: 0x00245014 File Offset: 0x00243214
		private void OnRuleItemRender(int index, GameObject goItem)
		{
			ValueTuple<string, string> valueTuple = this._replaceRulesList[index];
			string srcString = valueTuple.Item1;
			string targetString = valueTuple.Item2;
			EventEditorTagReplaceReplaceRulesPrefabInfo replaceRulesItemInfo = goItem.GetComponent<EventEditorTagReplaceReplaceRulesPrefabInfo>();
			replaceRulesItemInfo.srcTagInputField.onEndEdit.RemoveAllListeners();
			replaceRulesItemInfo.srcTagInputField.text = srcString;
			replaceRulesItemInfo.srcTagInputField.onEndEdit.AddListener(delegate(string str)
			{
				srcString = str;
				this._replaceRulesList[index] = new ValueTuple<string, string>(srcString, targetString);
			});
			replaceRulesItemInfo.targetTagInputField.onEndEdit.RemoveAllListeners();
			replaceRulesItemInfo.targetTagInputField.text = targetString;
			replaceRulesItemInfo.targetTagInputField.onEndEdit.AddListener(delegate(string str)
			{
				targetString = str;
				this._replaceRulesList[index] = new ValueTuple<string, string>(srcString, targetString);
			});
			replaceRulesItemInfo.btnDelete.ClearAndAddListener(delegate
			{
				this._replaceRulesList.RemoveAt(index);
				this.replaceRulesScroll.SetDataCount(this._replaceRulesList.Count);
			});
		}

		// Token: 0x06004CF4 RID: 19700 RVA: 0x002450FF File Offset: 0x002432FF
		private void OnCopyRoleTag()
		{
			GUIUtility.systemCopyBuffer = this.txtMeshRoleTag.text;
		}

		// Token: 0x06004CF5 RID: 19701 RVA: 0x00245113 File Offset: 0x00243313
		private void OnAddNewRule()
		{
			this._replaceRulesList.Add(new ValueTuple<string, string>(string.Empty, string.Empty));
			this.replaceRulesScroll.SetDataCount(this._replaceRulesList.Count);
		}

		// Token: 0x06004CF6 RID: 19702 RVA: 0x00245148 File Offset: 0x00243348
		private void OnExecuteReplace()
		{
			EventEditorData eventData = EventEditorEventDetail.Instance.CurEvent;
			bool flag = eventData == null;
			if (!flag)
			{
				string curEventGuid = eventData.EventGuid;
				bool groupReplace = this.toggleGroup.GetActiveIndex() == 1;
				bool flag2 = groupReplace;
				if (flag2)
				{
					string eventGroup = eventData.EventGroup;
					EventGroupData groupData = SingletonObject.getInstance<EventEditorModel>().GetGroupData(eventGroup);
					bool flag3 = groupData != null;
					if (flag3)
					{
						foreach (KeyValuePair<string, EventEditorBaseData> keyValuePair in groupData.AllEventContent)
						{
							string text;
							EventEditorBaseData eventEditorBaseData;
							keyValuePair.Deconstruct(out text, out eventEditorBaseData);
							string key = text;
							EventEditorBaseData tab = eventEditorBaseData;
							string content = tab.EventContent;
							bool flag4 = !string.IsNullOrEmpty(content);
							if (flag4)
							{
								foreach (ValueTuple<string, string> tuple in this._replaceRulesList)
								{
									content = content.Replace(tuple.Item1, tuple.Item2);
									tab.EventContent = content;
								}
							}
							foreach (int i in tab.Options.Keys.ToArray<int>())
							{
								string optionContent = tab.Options[i];
								bool flag5 = !string.IsNullOrEmpty(optionContent);
								if (flag5)
								{
									foreach (ValueTuple<string, string> tuple2 in this._replaceRulesList)
									{
										optionContent = optionContent.Replace(tuple2.Item1, tuple2.Item2);
										tab.Options[i] = optionContent;
									}
								}
							}
							bool flag6 = key == curEventGuid;
							if (flag6)
							{
								eventData = groupData.GetEvent(curEventGuid);
								eventData.Dirty = true;
							}
						}
						groupData.SaveGroup();
						groupData.UpdateContent();
						bool isEventEditorShow = TaskControlPanel.Instance.isEventEditorShow;
						if (isEventEditorShow)
						{
							EventEditorEventList.Instance.Show();
						}
					}
				}
				else
				{
					string content2 = eventData.EventContent;
					bool flag7 = !string.IsNullOrEmpty(content2);
					if (flag7)
					{
						foreach (ValueTuple<string, string> tuple3 in this._replaceRulesList)
						{
							content2 = content2.Replace(tuple3.Item1, tuple3.Item2);
							eventData.EventContent = content2;
						}
					}
					Dictionary<int, EventEditorData.Option> options = eventData.Options;
					foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair2 in options)
					{
						int num;
						EventEditorData.Option option2;
						keyValuePair2.Deconstruct(out num, out option2);
						EventEditorData.Option option = option2;
						string optionContent2 = option.Content;
						bool flag8 = !string.IsNullOrEmpty(optionContent2);
						if (flag8)
						{
							foreach (ValueTuple<string, string> tuple4 in this._replaceRulesList)
							{
								optionContent2 = optionContent2.Replace(tuple4.Item1, tuple4.Item2);
								option.InternalContent = optionContent2;
							}
						}
					}
					eventData.Dirty = true;
				}
				EventEditorEventPreview.Instance.Refresh(eventData);
			}
		}

		// Token: 0x0400354C RID: 13644
		public static EventEditorTagReplace Instance;

		// Token: 0x0400354D RID: 13645
		[SerializeField]
		private CButton btnHidePanelTagReplace;

		// Token: 0x0400354E RID: 13646
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x0400354F RID: 13647
		[SerializeField]
		private TextMeshProUGUI txtMeshEventGroup;

		// Token: 0x04003550 RID: 13648
		[SerializeField]
		private TextMeshProUGUI txtMeshEventName;

		// Token: 0x04003551 RID: 13649
		[SerializeField]
		private TextMeshProUGUI txtMeshRoleTag;

		// Token: 0x04003552 RID: 13650
		[SerializeField]
		private CButton btnCopyRoleTag;

		// Token: 0x04003553 RID: 13651
		[SerializeField]
		private InfinityScroll replaceRulesScroll;

		// Token: 0x04003554 RID: 13652
		[SerializeField]
		private CButton btnAddNewReplace;

		// Token: 0x04003555 RID: 13653
		[SerializeField]
		private CButton btnStartReplace;

		// Token: 0x04003556 RID: 13654
		private bool _isShowing;

		// Token: 0x04003557 RID: 13655
		private List<ValueTuple<string, string>> _replaceRulesList;
	}
}
