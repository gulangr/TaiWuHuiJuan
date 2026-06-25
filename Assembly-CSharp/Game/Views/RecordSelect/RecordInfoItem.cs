using System;
using System.IO;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.RecordSelect
{
	// Token: 0x020007B9 RID: 1977
	public class RecordInfoItem : MonoBehaviour
	{
		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06006065 RID: 24677 RVA: 0x002C3634 File Offset: 0x002C1834
		public Renamer Renamer
		{
			get
			{
				return this._renamer;
			}
		}

		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06006066 RID: 24678 RVA: 0x002C363C File Offset: 0x002C183C
		public CButton BtnStart
		{
			get
			{
				return this._btnContinue;
			}
		}

		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x06006067 RID: 24679 RVA: 0x002C3644 File Offset: 0x002C1844
		public RectTransform BtnLayout
		{
			get
			{
				return this._btnLayout;
			}
		}

		// Token: 0x17000BC4 RID: 3012
		// (get) Token: 0x06006068 RID: 24680 RVA: 0x002C364C File Offset: 0x002C184C
		public CButton BtnContinue
		{
			get
			{
				return this._btnContinue;
			}
		}

		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06006069 RID: 24681 RVA: 0x002C3654 File Offset: 0x002C1854
		public CButton BtnDelete
		{
			get
			{
				return this._btnDelete;
			}
		}

		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x0600606A RID: 24682 RVA: 0x002C365C File Offset: 0x002C185C
		public CButton BtnRevert
		{
			get
			{
				return this._btnRevert;
			}
		}

		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x0600606B RID: 24683 RVA: 0x002C3664 File Offset: 0x002C1864
		public CButton BtnWarning
		{
			get
			{
				return this._btnWarning;
			}
		}

		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x0600606C RID: 24684 RVA: 0x002C366C File Offset: 0x002C186C
		public TooltipInvoker MouseTip
		{
			get
			{
				return this._mouseTip;
			}
		}

		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x0600606D RID: 24685 RVA: 0x002C3674 File Offset: 0x002C1874
		public PointClickBridge PointClickBridge
		{
			get
			{
				return this.pointClickBridge;
			}
		}

		// Token: 0x0600606E RID: 24686 RVA: 0x002C367C File Offset: 0x002C187C
		private void Awake()
		{
			this._btnRename.ClearAndAddListener(delegate
			{
				new RenameCfg
				{
					Title = LanguageKey.LK_Record_Title.Tr(),
					Description = LanguageKey.LK_Record_Desc.TrFormat(this.GetRecordName()),
					CharCount = 6,
					EmptyDesc = LanguageKey.LK_Record_Empty.Tr(),
					Default = this.GetRecordName(),
					Submit = delegate(string str)
					{
						this.EditRecordName(this._renamer, str);
					}
				}.Show();
			});
		}

		// Token: 0x0600606F RID: 24687 RVA: 0x002C3698 File Offset: 0x002C1898
		public string GetRecordFullName(string saveName = null)
		{
			string recordName = saveName ?? this.GetRecordName();
			return (recordName != null && recordName.Length > 0) ? string.Concat(new string[]
			{
				"<color=#F8E6C1>",
				recordName,
				"</color><color=#BDBDBD>(",
				this.CharacterName,
				")</color>"
			}) : ("<color=#F8E6C1>" + this.CharacterName + "</color>");
		}

		// Token: 0x06006070 RID: 24688 RVA: 0x002C3704 File Offset: 0x002C1904
		public string GetRecordName()
		{
			string file = Path.Combine(GameApp.GetArchiveDirPath(), string.Format("world_{0}", this.Index), "name");
			if (File.Exists(file))
			{
				string recordName = File.ReadAllText(file).Replace("\r", "").Replace("\n", "");
				if (recordName != null)
				{
					int length = recordName.Length;
					if (length > 0 && length <= 6)
					{
						return recordName;
					}
				}
			}
			return "";
		}

		// Token: 0x06006071 RID: 24689 RVA: 0x002C3784 File Offset: 0x002C1984
		public void EditRecordName(Renamer renamer, string saveName)
		{
			File.WriteAllText(Path.Combine(GameApp.GetArchiveDirPath(), string.Format("world_{0}", this.Index), "name"), saveName);
			renamer.Name.text = this.GetRecordFullName(saveName);
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x002C37D0 File Offset: 0x002C19D0
		public void OnRenameClicked(Renamer renamer, string nonsense = "")
		{
			renamer.Input.text = this.GetRecordName();
		}

		// Token: 0x06006073 RID: 24691 RVA: 0x002C37E8 File Offset: 0x002C19E8
		public void SetAsEmptyArchive()
		{
			this._renamer.Refresh(LocalStringManager.Get(LanguageKey.UI_RecordSelect_EmptyScroll), 6, false, null);
			this._recordBroken.SetActive(false);
			this._locationBar.SetActive(false);
			this._gameTime.text = string.Empty;
			this._ageSamsara.text = string.Empty;
			this._newRecordIcon.gameObject.SetActive(true);
			this._btnRename.gameObject.SetActive(false);
			this._btnLayout.gameObject.SetActive(false);
			this._avatartFrame.gameObject.SetActive(false);
			this._btnWarning.gameObject.SetActive(false);
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x002C38A8 File Offset: 0x002C1AA8
		public void SetAsBrokenArchive()
		{
			this._recordBroken.SetActive(true);
			this._renamer.Refresh("", 6, false, null);
			this._locationBar.SetActive(false);
			this._gameTime.text = string.Empty;
			this._ageSamsara.text = string.Empty;
			this._newRecordIcon.gameObject.SetActive(false);
			this._btnRename.gameObject.SetActive(false);
			this._btnLayout.gameObject.SetActive(false);
			this._avatartFrame.gameObject.SetActive(false);
			this._btnWarning.gameObject.SetActive(false);
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x002C3960 File Offset: 0x002C1B60
		public void SetArchiveInfo(int index, WorldInfo worldInfo, DateTime utcSave, int year, string characterName)
		{
			this._recordBroken.SetActive(false);
			this._newRecordIcon.gameObject.SetActive(false);
			this.Index = index;
			this.CharacterName = characterName;
			this._renamer.Refresh(this.GetRecordFullName(null), 6, true, this.GetRecordFullName(""));
			this._locationBar.SetActive(true);
			this._location.text = worldInfo.MapStateName;
			this._location.text = MapState.Instance[worldInfo.MapStateTemplateId].Name;
			this._location2.text = MapArea.Instance[worldInfo.MapAreaTemplateId].Name;
			this._locationIcon.SetSprite("ui9_icon_main_arrive_icon_0_" + ((int)(worldInfo.MapStateTemplateId - 1)).ToString(), false, null);
			this._gameTime.text = utcSave.ToLocalTime().ToString("yyyy-MM-dd [HH:mm]");
			this._ageSamsara.text = LocalStringManager.GetFormat(LanguageKey.UI_RecordSelect_Year, year, worldInfo.TaiwuGenerationsCount);
			this._btnRename.gameObject.SetActive(true);
			this._btnLayout.gameObject.SetActive(base.GetComponent<CToggle>().isOn);
			this._avatartFrame.gameObject.SetActive(true);
			this.pointClickBridge.OnDoubleClick = delegate()
			{
				bool interactable = this._btnContinue.interactable;
				if (interactable)
				{
					AudioManager.Instance.PlaySound("Continue_click", false, false);
					this._btnContinue.onClick.Invoke();
				}
			};
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x002C3AE4 File Offset: 0x002C1CE4
		public void SetAvatar(bool active)
		{
			this._avatar.gameObject.SetActive(active);
			this.brokenImage.gameObject.SetActive(!active);
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x002C3B0E File Offset: 0x002C1D0E
		public void RefreshAvatar(AvatarRelatedData avatarRelatedData)
		{
			this._avatar.Refresh(avatarRelatedData);
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x002C3B1E File Offset: 0x002C1D1E
		public void RefreshAvatar(Sprite sprite)
		{
			this._avatar.Refresh(sprite);
		}

		// Token: 0x06006079 RID: 24697 RVA: 0x002C3B2E File Offset: 0x002C1D2E
		public void SetBtnLayoutActive(bool active)
		{
			this._btnLayout.gameObject.SetActive(active);
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x002C3B44 File Offset: 0x002C1D44
		public void SetButtonsState(bool hasBackup, Action onRevert, Action onDelete, Action onContinue)
		{
			this._btnRevert.gameObject.SetActive(true);
			if (hasBackup)
			{
				this._btnRevert.interactable = true;
				this._btnRevert.ClearAndAddListener(onRevert);
			}
			else
			{
				this._btnRevert.interactable = false;
			}
			this._btnDelete.gameObject.SetActive(true);
			this._btnDelete.interactable = true;
			this._btnDelete.ClearAndAddListener(onDelete);
			this._btnContinue.gameObject.SetActive(true);
			this._btnContinue.interactable = true;
			this._btnContinue.ClearAndAddListener(onContinue);
		}

		// Token: 0x0600607B RID: 24699 RVA: 0x002C3BF0 File Offset: 0x002C1DF0
		public void SetBrokenArchiveButtons(bool hasBackup, Action onRevert, Action onDelete)
		{
			this._btnContinue.gameObject.SetActive(true);
			this._btnContinue.interactable = false;
			if (hasBackup)
			{
				this._btnRevert.gameObject.SetActive(true);
				this._btnRevert.interactable = true;
				this._btnRevert.ClearAndAddListener(onRevert);
			}
			else
			{
				this._btnRevert.interactable = false;
			}
			this._btnDelete.gameObject.SetActive(true);
			this._btnDelete.interactable = true;
			this._btnDelete.ClearAndAddListener(onDelete);
		}

		// Token: 0x0600607C RID: 24700 RVA: 0x002C3C8D File Offset: 0x002C1E8D
		public void SetWarningButton(bool active, bool interactable, Action onClick)
		{
			this._btnWarning.gameObject.SetActive(active);
			this._btnWarning.interactable = interactable;
			this._btnWarning.ClearAndAddListener(onClick);
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x002C3CBC File Offset: 0x002C1EBC
		public void SetMouseTip(bool enabled, string[] presetParam = null)
		{
			this._mouseTip.enabled = enabled;
			bool flag = presetParam != null;
			if (flag)
			{
				this._mouseTip.PresetParam = presetParam;
			}
		}

		// Token: 0x0600607E RID: 24702 RVA: 0x002C3CED File Offset: 0x002C1EED
		public void SetBtnStartInteractable(bool interactable)
		{
			this._btnContinue.interactable = interactable;
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x002C3D00 File Offset: 0x002C1F00
		private short GetStateId(string stateName)
		{
			short stateId = 0;
			MapState.Instance.Iterate(delegate(MapStateItem e)
			{
				bool flag = e.Name == stateName;
				bool result;
				if (flag)
				{
					stateId = (short)e.TemplateId;
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			});
			return stateId;
		}

		// Token: 0x040042D3 RID: 17107
		public int Index;

		// Token: 0x040042D4 RID: 17108
		public string CharacterName;

		// Token: 0x040042D5 RID: 17109
		[SerializeField]
		private Renamer _renamer;

		// Token: 0x040042D6 RID: 17110
		[SerializeField]
		private GameObject _recordBroken;

		// Token: 0x040042D7 RID: 17111
		[SerializeField]
		private Game.Components.Avatar.Avatar _avatar;

		// Token: 0x040042D8 RID: 17112
		[SerializeField]
		private GameObject _locationBar;

		// Token: 0x040042D9 RID: 17113
		[SerializeField]
		private TextMeshProUGUI _gameTime;

		// Token: 0x040042DA RID: 17114
		[SerializeField]
		private TextMeshProUGUI _ageSamsara;

		// Token: 0x040042DB RID: 17115
		[SerializeField]
		private TooltipInvoker _mouseTip;

		// Token: 0x040042DC RID: 17116
		[SerializeField]
		private CButton _btnDelete;

		// Token: 0x040042DD RID: 17117
		[SerializeField]
		private CButton _btnRevert;

		// Token: 0x040042DE RID: 17118
		[SerializeField]
		private CButton _btnContinue;

		// Token: 0x040042DF RID: 17119
		[SerializeField]
		private CButton _btnWarning;

		// Token: 0x040042E0 RID: 17120
		[SerializeField]
		private RectTransform _btnLayout;

		// Token: 0x040042E1 RID: 17121
		[SerializeField]
		private TextMeshProUGUI _location;

		// Token: 0x040042E2 RID: 17122
		[SerializeField]
		private TextMeshProUGUI _location2;

		// Token: 0x040042E3 RID: 17123
		[SerializeField]
		private CImage _locationIcon;

		// Token: 0x040042E4 RID: 17124
		[SerializeField]
		private CImage _newRecordIcon;

		// Token: 0x040042E5 RID: 17125
		[SerializeField]
		private CButton _btnRename;

		// Token: 0x040042E6 RID: 17126
		[SerializeField]
		private RectTransform _avatartFrame;

		// Token: 0x040042E7 RID: 17127
		[SerializeField]
		private PointClickBridge pointClickBridge;

		// Token: 0x040042E8 RID: 17128
		[SerializeField]
		private CImage brokenImage;
	}
}
