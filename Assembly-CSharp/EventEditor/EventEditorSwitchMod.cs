using System;
using System.Collections.Generic;
using System.IO;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200064E RID: 1614
	public class EventEditorSwitchMod : EventEditorSubPageBase
	{
		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x06004CDA RID: 19674 RVA: 0x00244B2E File Offset: 0x00242D2E
		// (set) Token: 0x06004CDB RID: 19675 RVA: 0x00244B35 File Offset: 0x00242D35
		public static EventEditorSwitchMod Instance { get; private set; }

		// Token: 0x06004CDC RID: 19676 RVA: 0x00244B3D File Offset: 0x00242D3D
		public static void Init(EventEditorSwitchMod instance)
		{
			EventEditorSwitchMod.Instance = instance;
			EventEditorSwitchMod.Instance.InternalInit();
			EventEditorSwitchMod.Instance.Hide();
		}

		// Token: 0x06004CDD RID: 19677 RVA: 0x00244B5D File Offset: 0x00242D5D
		public override void Show()
		{
			this.InitScrollData();
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004CDE RID: 19678 RVA: 0x00244B74 File Offset: 0x00242D74
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004CDF RID: 19679 RVA: 0x00244B84 File Offset: 0x00242D84
		protected override void InternalInit()
		{
			this._scroll = this.modList;
			this._scroll.OnItemRender += this.OnItemRender;
			this._toggleGroup = this._scroll.GetComponent<CScrollRect>().Content.GetComponent<CToggleGroup>();
			this.btnCancel.ClearAndAddListener(new Action(this.Hide));
			this.btnConfirm.ClearAndAddListener(new Action(this.SwitchAndHide));
		}

		// Token: 0x06004CE0 RID: 19680 RVA: 0x00244C04 File Offset: 0x00242E04
		private void SwitchAndHide()
		{
			List<CToggle> allToggles = this._toggleGroup.GetAll();
			int toggleIndex = this._toggleGroup.GetActiveIndex();
			CToggle tog = allToggles[toggleIndex];
			int index = this._scroll.GetCellIndex(tog.gameObject);
			string modName = this._modList[index];
			bool flag = modName != ModManager.GetWorkingModName();
			if (flag)
			{
				ModManager.SetWorkingModName(modName);
				SingletonObject.getInstance<EventEditorModel>().OnModelDataReady = delegate()
				{
					EventGroupData groupData = EventGroupTreeView.Instance.EditingEventGroup;
					bool flag2 = groupData != null;
					if (flag2)
					{
						groupData = SingletonObject.getInstance<EventEditorModel>().GetGroupData(groupData.Key);
					}
					bool isEventEditorShow = TaskControlPanel.Instance.isEventEditorShow;
					if (isEventEditorShow)
					{
						EventEditorEventList.Instance.ShowingEventGroup = groupData;
						EventEditorData eventData = EventEditorEventDetail.Instance.CurEvent;
						bool flag3 = eventData != null;
						if (flag3)
						{
							eventData = SingletonObject.getInstance<EventEditorModel>().GetEvent(eventData.EventGuid);
						}
						EventEditorEventDetail.Instance.EditEvent(eventData);
					}
					else
					{
						EventGroupTreeView.Instance.Show();
					}
				};
				SingletonObject.getInstance<EventEditorModel>().LoadEventCore();
			}
			this.Hide();
		}

		// Token: 0x06004CE1 RID: 19681 RVA: 0x00244CAC File Offset: 0x00242EAC
		private void InitScrollData()
		{
			this._prevMod = ModManager.GetWorkingModName();
			if (this._modList == null)
			{
				this._modList = new List<string>();
			}
			this._modList.Clear();
			string modRooDir = ModManager.GetModFactoryRootFolder().PathFix() + "/";
			string[] modDirectories = Directory.GetDirectories(modRooDir, "*", SearchOption.TopDirectoryOnly);
			foreach (string t in modDirectories)
			{
				this._modList.Add(t.PathFix().Replace(modRooDir, string.Empty));
			}
			this._scroll.SetDataCount(this._modList.Count);
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x00244D54 File Offset: 0x00242F54
		private void OnItemRender(int index, GameObject go)
		{
			string modName = this._modList[index];
			EventEditorSwitchModCellInfo cellInfo = go.GetComponent<EventEditorSwitchModCellInfo>();
			cellInfo.txtMeshModName.text = modName;
		}

		// Token: 0x04003544 RID: 13636
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04003545 RID: 13637
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x04003546 RID: 13638
		[SerializeField]
		private InfinityScroll modList;

		// Token: 0x04003547 RID: 13639
		private List<string> _modList;

		// Token: 0x04003548 RID: 13640
		private InfinityScroll _scroll;

		// Token: 0x04003549 RID: 13641
		private string _prevMod;

		// Token: 0x0400354A RID: 13642
		private CToggleGroup _toggleGroup;
	}
}
