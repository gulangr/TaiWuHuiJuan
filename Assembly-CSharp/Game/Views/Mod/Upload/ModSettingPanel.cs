using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008D7 RID: 2263
	public class ModSettingPanel : MonoBehaviour
	{
		// Token: 0x06006C55 RID: 27733 RVA: 0x0031F218 File Offset: 0x0031D418
		public void Init(Action<List<string>, List<SettingEntry>> onConfirm)
		{
			this._onConfirm = onConfirm;
			this.groupScroll.OnItemRender -= this.GroupScrollOnOnItemRender;
			this.groupScroll.OnItemRender += this.GroupScrollOnOnItemRender;
			this.buttonAddGroup.ClearAndAddListener(new Action(this.AddGroup));
			this.entryScroll.OnItemRender -= this.EntryScrollOnOnItemRender;
			this.entryScroll.OnItemRender += this.EntryScrollOnOnItemRender;
			this.buttonAddEntry.ClearAndAddListener(new Action(this.AddEntry));
			this.buttonClose.ClearAndAddListener(delegate
			{
				this.Hide(false, null);
			});
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
		}

		// Token: 0x06006C56 RID: 27734 RVA: 0x0031F2ED File Offset: 0x0031D4ED
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ModEditSettings, new GEvent.Callback(this.OnModEditSettings));
		}

		// Token: 0x06006C57 RID: 27735 RVA: 0x0031F30C File Offset: 0x0031D50C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ModEditSettings, new GEvent.Callback(this.OnModEditSettings));
		}

		// Token: 0x06006C58 RID: 27736 RVA: 0x0031F32C File Offset: 0x0031D52C
		public void Show(List<string> settingGroupList, List<SettingEntry> settingEntryList)
		{
			this._originSettingGroupList = settingGroupList;
			this._originGroupSettingEntryList = settingEntryList;
			this._settingGroupList.Clear();
			this._settingGroupList.AddRange(settingGroupList);
			this._totalGroupSettingEntryList.Clear();
			this._totalGroupSettingEntryList.AddRange(settingEntryList);
			this.BuildGroup();
			this.SelectGroup((this._settingGroupList.Count > 0) ? 0 : -1);
			base.gameObject.SetActive(true);
		}

		// Token: 0x06006C59 RID: 27737 RVA: 0x0031F3A8 File Offset: 0x0031D5A8
		private void BuildGroup()
		{
			this._settingDict.Clear();
			foreach (SettingEntry settingEntry in this._totalGroupSettingEntryList)
			{
				string group = settingEntry.GroupName.IsNullOrEmpty() ? "Default" : settingEntry.GroupName;
				List<SettingEntry> list;
				bool flag = !this._settingDict.TryGetValue(group, out list);
				if (flag)
				{
					list = (this._settingDict[group] = new List<SettingEntry>());
					bool flag2 = !this._settingGroupList.Contains(group);
					if (flag2)
					{
						this._settingGroupList.Add(group);
					}
				}
				list.Add(settingEntry);
			}
			foreach (string group2 in this._settingGroupList)
			{
				bool flag3 = !this._settingDict.ContainsKey(group2);
				if (flag3)
				{
					this._settingDict[group2] = new List<SettingEntry>();
				}
			}
		}

		// Token: 0x06006C5A RID: 27738 RVA: 0x0031F4E4 File Offset: 0x0031D6E4
		private void GroupScrollOnOnItemRender(int index, GameObject obj)
		{
			string groupTitle = this._settingGroupList[index];
			ModSettingGroup group = obj.GetComponent<ModSettingGroup>();
			bool isSelected = this._selectedGroupIndex == index;
			group.Refresh(index, this._settingGroupList.Count, groupTitle, isSelected, new Action<int>(this.OnClickGroup), new Action<int, int>(this.OnMoveGroup), new Action<int, string>(this.OnRenameGroup), new Action<int>(this.OnDeleteGroup));
		}

		// Token: 0x06006C5B RID: 27739 RVA: 0x0031F558 File Offset: 0x0031D758
		private void OnDeleteGroup(int index)
		{
			ModSettingPanel.<>c__DisplayClass22_0 CS$<>8__locals1 = new ModSettingPanel.<>c__DisplayClass22_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			string title = LanguageKey.LK_Mod_EditSetting_RemoveGroup.Tr();
			string content = LanguageKey.LK_Mod_EditSetting_RemoveGroup_Confirm.Tr();
			CommonUtils.ShowConfirmDialog(title, content, new Action(CS$<>8__locals1.<OnDeleteGroup>g__Delete|0), null, EDialogType.None);
		}

		// Token: 0x06006C5C RID: 27740 RVA: 0x0031F5A8 File Offset: 0x0031D7A8
		private void OnRenameGroup(int index, string groupTitle)
		{
			bool flag = groupTitle.IsNullOrEmpty();
			if (!flag)
			{
				bool flag2 = this._settingGroupList.Any((string g) => g == groupTitle);
				if (!flag2)
				{
					string oldTitle = this._settingGroupList[index];
					List<SettingEntry> list;
					bool flag3 = this._settingDict.Remove(oldTitle, out list);
					if (flag3)
					{
						this._settingDict[groupTitle] = list;
						foreach (SettingEntry entry in list)
						{
							bool flag4 = entry.GroupName == oldTitle;
							if (flag4)
							{
								entry.GroupName = groupTitle;
							}
						}
					}
					this._settingGroupList[index] = groupTitle;
					this.groupScroll.ReRender();
				}
			}
		}

		// Token: 0x06006C5D RID: 27741 RVA: 0x0031F6B0 File Offset: 0x0031D8B0
		private void OnMoveGroup(int oldIndex, int newIndex)
		{
			string oldGroup = this._settingGroupList[oldIndex];
			string newGroup = this._settingGroupList[newIndex];
			this._settingGroupList[oldIndex] = newGroup;
			this._settingGroupList[newIndex] = oldGroup;
			this.SelectGroup(newIndex);
			this.groupScroll.ReRender();
		}

		// Token: 0x06006C5E RID: 27742 RVA: 0x0031F708 File Offset: 0x0031D908
		private void OnClickGroup(int index)
		{
			this.SelectGroup(index);
			this.groupScroll.ReRender();
		}

		// Token: 0x06006C5F RID: 27743 RVA: 0x0031F720 File Offset: 0x0031D920
		private void AddGroup()
		{
			string newTitle = LanguageKey.LK_Mod_EditSetting_NewGroup.Tr();
			int newCount = this._settingGroupList.Count((string g) => g.Contains(newTitle)) + 1;
			string finalTitle = newTitle + newCount.ToString();
			this._settingGroupList.Add(finalTitle);
			this._settingDict[finalTitle] = new List<SettingEntry>();
			this.SelectGroup(this._settingGroupList.Count - 1);
		}

		// Token: 0x06006C60 RID: 27744 RVA: 0x0031F7A4 File Offset: 0x0031D9A4
		private void SelectGroup(int index)
		{
			this._selectedGroupIndex = index;
			this.RefreshEntryList();
			this.groupScroll.SetDataCount(this._settingGroupList.Count);
		}

		// Token: 0x06006C61 RID: 27745 RVA: 0x0031F7CC File Offset: 0x0031D9CC
		private void RefreshEntryList()
		{
			this._curGroupSettingEntryList.Clear();
			string group = this._settingGroupList.GetOrDefault(this._selectedGroupIndex, string.Empty);
			List<SettingEntry> list;
			bool flag = this._settingDict.TryGetValue(group, out list);
			if (flag)
			{
				this._curGroupSettingEntryList.AddRange(list);
			}
			this.entryScroll.SetDataCount(this._curGroupSettingEntryList.Count);
			this.buttonAddEntry.gameObject.SetActive(this._selectedGroupIndex >= 0);
		}

		// Token: 0x06006C62 RID: 27746 RVA: 0x0031F850 File Offset: 0x0031DA50
		private void EntryScrollOnOnItemRender(int index, GameObject obj)
		{
			SettingEntry settingEntry = this._curGroupSettingEntryList[index];
			ModSettingEntry entry = obj.GetComponent<ModSettingEntry>();
			entry.Refresh(settingEntry, new Action<SettingEntry>(this.OnClickEntry), new Action<SettingEntry>(this.OnDeleteEntry));
		}

		// Token: 0x06006C63 RID: 27747 RVA: 0x0031F894 File Offset: 0x0031DA94
		private void OnClickEntry(SettingEntry entry)
		{
			string group = this._settingGroupList[this._selectedGroupIndex];
			this.editSettingPanel.Show(this._totalGroupSettingEntryList, entry, group, this._settingGroupList);
		}

		// Token: 0x06006C64 RID: 27748 RVA: 0x0031F8CE File Offset: 0x0031DACE
		private void OnDeleteEntry(SettingEntry entry)
		{
			this._curGroupSettingEntryList.Remove(entry);
			this._totalGroupSettingEntryList.Remove(entry);
			this.entryScroll.SetDataCount(this._curGroupSettingEntryList.Count);
		}

		// Token: 0x06006C65 RID: 27749 RVA: 0x0031F904 File Offset: 0x0031DB04
		private void AddEntry()
		{
			string group = this._settingGroupList[this._selectedGroupIndex];
			this.editSettingPanel.Show(this._totalGroupSettingEntryList, null, group, this._settingGroupList);
		}

		// Token: 0x06006C66 RID: 27750 RVA: 0x0031F93E File Offset: 0x0031DB3E
		private void OnModEditSettings(ArgumentBox argumentBox)
		{
			this.BuildGroup();
			this.SelectGroup(this._selectedGroupIndex);
		}

		// Token: 0x06006C67 RID: 27751 RVA: 0x0031F958 File Offset: 0x0031DB58
		public void Hide(bool check = false, Action onConfirm = null)
		{
			ModSettingPanel.<>c__DisplayClass34_0 CS$<>8__locals1 = new ModSettingPanel.<>c__DisplayClass34_0();
			CS$<>8__locals1.onConfirm = onConfirm;
			CS$<>8__locals1.<>4__this = this;
			bool needConfirm = this._originSettingGroupList.ContentIsDifferent(this._settingGroupList) || this._originGroupSettingEntryList.ContentIsDifferent(this._totalGroupSettingEntryList);
			bool flag = needConfirm;
			if (flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Content);
				CommonUtils.ShowConfirmDialog(title, content, new Action(CS$<>8__locals1.<Hide>g__Action|0), null, EDialogType.None);
			}
			else
			{
				CS$<>8__locals1.<Hide>g__Action|0();
			}
		}

		// Token: 0x06006C68 RID: 27752 RVA: 0x0031F9E4 File Offset: 0x0031DBE4
		private void OnClickConfirm()
		{
			this._onConfirm(this._settingGroupList, this._totalGroupSettingEntryList);
			this.Hide(false, null);
		}

		// Token: 0x04004E9A RID: 20122
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04004E9B RID: 20123
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04004E9C RID: 20124
		[SerializeField]
		private ModEditSettingPanel editSettingPanel;

		// Token: 0x04004E9D RID: 20125
		[Header("设置分组")]
		[SerializeField]
		private InfinityScroll groupScroll;

		// Token: 0x04004E9E RID: 20126
		[SerializeField]
		private CButton buttonAddGroup;

		// Token: 0x04004E9F RID: 20127
		[Header("设置列表")]
		[SerializeField]
		private InfinityScroll entryScroll;

		// Token: 0x04004EA0 RID: 20128
		[SerializeField]
		private CButton buttonAddEntry;

		// Token: 0x04004EA1 RID: 20129
		private readonly Dictionary<string, List<SettingEntry>> _settingDict = new Dictionary<string, List<SettingEntry>>();

		// Token: 0x04004EA2 RID: 20130
		private readonly List<string> _settingGroupList = new List<string>();

		// Token: 0x04004EA3 RID: 20131
		private readonly List<SettingEntry> _curGroupSettingEntryList = new List<SettingEntry>();

		// Token: 0x04004EA4 RID: 20132
		private readonly List<SettingEntry> _totalGroupSettingEntryList = new List<SettingEntry>();

		// Token: 0x04004EA5 RID: 20133
		private List<string> _originSettingGroupList;

		// Token: 0x04004EA6 RID: 20134
		private List<SettingEntry> _originGroupSettingEntryList;

		// Token: 0x04004EA7 RID: 20135
		private int _selectedGroupIndex = -1;

		// Token: 0x04004EA8 RID: 20136
		private const string DefaultGroup = "Default";

		// Token: 0x04004EA9 RID: 20137
		private Action<List<string>, List<SettingEntry>> _onConfirm;
	}
}
