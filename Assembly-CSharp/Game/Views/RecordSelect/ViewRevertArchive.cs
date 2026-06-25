using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.RecordSelect
{
	// Token: 0x020007BD RID: 1981
	public class ViewRevertArchive : UIBase
	{
		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x060060CB RID: 24779 RVA: 0x002C628B File Offset: 0x002C448B
		// (set) Token: 0x060060CC RID: 24780 RVA: 0x002C6293 File Offset: 0x002C4493
		public int SelectedIndex
		{
			get
			{
				return this._selectIndex;
			}
			set
			{
				this._selectIndex = value;
				this.confirmBtn.interactable = (value >= 0);
			}
		}

		// Token: 0x060060CD RID: 24781 RVA: 0x002C62B0 File Offset: 0x002C44B0
		private void Awake()
		{
			this.scroll.OnItemRender += this.OnArchiveItemRender;
		}

		// Token: 0x060060CE RID: 24782 RVA: 0x002C62CC File Offset: 0x002C44CC
		public override void OnInit(ArgumentBox argsBox)
		{
			bool arg1State = argsBox.Get("ArchiveIndex", out this._archiveIndex);
			bool arg2State = argsBox.Get<ArchiveInfo>("ArchiveData", out this._archiveInfo);
			bool arg3State = argsBox.Get<Action<long>>("OnConfirmEnter", out this._onConfirmEnter);
			bool flag = !arg1State || !arg2State || !arg3State;
			if (flag)
			{
				UIManager.Instance.HideUI(this.Element);
			}
			else
			{
				WorldInfo worldInfo = this._archiveInfo.WorldInfo;
				string taiwuName = UI_RecordSelect.GetCharacterName(worldInfo);
				this._archiveInfo.BackupWorldsInfo.Sort(([TupleElementNames(new string[]
				{
					"timestamp",
					"worldInfo"
				})] ValueTuple<long, WorldInfo> a, [TupleElementNames(new string[]
				{
					"timestamp",
					"worldInfo"
				})] ValueTuple<long, WorldInfo> b) => b.Item1.CompareTo(a.Item1));
				this.revertTitle.text = LocalStringManager.Get(LanguageKey.LK_Record_RevertArchive_Title);
				this.scroll.SetDataCount(this._archiveInfo.BackupWorldsInfo.Count);
				this.scroll.ScrollTo(0, 0.3f);
			}
		}

		// Token: 0x060060CF RID: 24783 RVA: 0x002C63C4 File Offset: 0x002C45C4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "BtnClose" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
			bool flag2 = btnName == "EnterGame";
			if (flag2)
			{
				this.EnterGame();
			}
		}

		// Token: 0x060060D0 RID: 24784 RVA: 0x002C6409 File Offset: 0x002C4609
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x002C6430 File Offset: 0x002C4630
		private void OnArchiveItemRender(int index, GameObject refers)
		{
			RevertArchiveItem item = refers.GetComponent<RevertArchiveItem>();
			ValueTuple<long, WorldInfo> data = this._archiveInfo.BackupWorldsInfo[index];
			Game.Components.Avatar.Avatar avatar = item.Avatar;
			WorldInfo worldInfo = data.Item2;
			bool flag = this._archiveInfo.WorldInfo != null;
			string taiwuName;
			string mapStateName;
			string mapAreaName;
			string savingTimeText;
			string ageSamsaraText;
			if (flag)
			{
				int year = data.Item2.CurrDate / 12 + 1;
				taiwuName = worldInfo.TaiwuSurname + worldInfo.TaiwuGivenName;
				bool hideTaiwuOriginalSurname = SingletonObject.getInstance<GlobalSettings>().HideTaiwuOriginalSurname;
				if (hideTaiwuOriginalSurname)
				{
					taiwuName = LocalStringManager.Get(LanguageKey.LK_Taiwu) + worldInfo.TaiwuGivenName;
				}
				mapStateName = MapState.Instance[worldInfo.MapStateTemplateId].Name;
				mapAreaName = MapArea.Instance[worldInfo.MapAreaTemplateId].Name;
				savingTimeText = DateTime.MinValue.AddTicks(worldInfo.SavingTimestamp).ToLocalTime().ToString("yyyy-MM-dd 【HH:mm:ss】");
				ageSamsaraText = LocalStringManager.GetFormat(LanguageKey.UI_RecordSelect_Year, year, worldInfo.TaiwuGenerationsCount);
				worldInfo.AvatarRelatedData.AvatarData.ClothDisplayId = worldInfo.AvatarRelatedData.ClothingDisplayId;
				avatar.Refresh(worldInfo.AvatarRelatedData.AvatarData, worldInfo.AvatarRelatedData.DisplayAge);
			}
			else
			{
				taiwuName = LocalStringManager.Get(LanguageKey.LK_UnknownCharName);
				mapStateName = LocalStringManager.Get(LanguageKey.LK_Unknown_Area_Name);
				mapAreaName = LocalStringManager.Get(LanguageKey.LK_Unknown);
				savingTimeText = string.Format("{0} {1}", LocalStringManager.Get(LanguageKey.LK_Save_Backup), index + 1);
				ageSamsaraText = LocalStringManager.Get(LanguageKey.LK_Unknown);
				avatar.ResetToBlank(false);
			}
			item.SetText(taiwuName, ageSamsaraText, savingTimeText, mapStateName, mapAreaName);
			item.InitToggleListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.SelectedIndex = index;
					this.scroll.ReRender();
				}
				else
				{
					bool flag2 = index == this.SelectedIndex;
					if (flag2)
					{
						this.EnterGame();
					}
					else
					{
						this.SelectedIndex = -1;
					}
				}
			});
			item.SetDoubleClickCallback(new Action(this.EnterGame));
			item.Toggle.SetIsOnWithoutNotify(index == this.SelectedIndex);
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x002C664C File Offset: 0x002C484C
		private void EnterGame()
		{
			bool flag = this.SelectedIndex < 0;
			if (flag)
			{
				Debug.LogWarning("未选择存档");
			}
			else
			{
				base.QuickHide();
				ValueTuple<long, WorldInfo> data = this._archiveInfo.BackupWorldsInfo[this.SelectedIndex];
				Action<long> onConfirmEnter = this._onConfirmEnter;
				if (onConfirmEnter != null)
				{
					onConfirmEnter(data.Item1);
				}
			}
		}

		// Token: 0x04004316 RID: 17174
		private ArchiveInfo _archiveInfo;

		// Token: 0x04004317 RID: 17175
		private sbyte _archiveIndex;

		// Token: 0x04004318 RID: 17176
		private Action<long> _onConfirmEnter;

		// Token: 0x04004319 RID: 17177
		private int _selectIndex;

		// Token: 0x0400431A RID: 17178
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x0400431B RID: 17179
		[SerializeField]
		private TextMeshProUGUI revertTitle;

		// Token: 0x0400431C RID: 17180
		[SerializeField]
		private CButton confirmBtn;
	}
}
