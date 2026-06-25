using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Building.BuildingManage;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007DC RID: 2012
	public class AvatarPresetItem : MonoBehaviour
	{
		// Token: 0x06006227 RID: 25127 RVA: 0x002D0BBB File Offset: 0x002CEDBB
		private void Awake()
		{
			this._rectTransform = base.GetComponent<RectTransform>();
			TMP_Text textComponent = this.renameInput.textComponent;
			this._renameInputFontAsset = (((textComponent != null) ? textComponent.font : null) ?? this.renameInput.fontAsset);
		}

		// Token: 0x06006228 RID: 25128 RVA: 0x002D0BF8 File Offset: 0x002CEDF8
		private void Update()
		{
			bool flag = !this._isCustomPreset || !this.contentNode.activeSelf;
			if (!flag)
			{
				bool wasHovering = this._isHovering;
				this._isHovering = RectTransformUtility.RectangleContainsScreenPoint(this._rectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag2 = this._isHovering != wasHovering;
				if (flag2)
				{
					this.UpdateHoverButtonsVisibility();
				}
			}
		}

		// Token: 0x06006229 RID: 25129 RVA: 0x002D0C6C File Offset: 0x002CEE6C
		private void UpdateHoverButtonsVisibility()
		{
			this.deleteBtnRoot.SetActive(this._isHovering);
			this.renameButton.gameObject.SetActive(this._isHovering);
			this.indexLabelNode.SetActive(!this._isHovering);
		}

		// Token: 0x0600622A RID: 25130 RVA: 0x002D0CB8 File Offset: 0x002CEEB8
		public void Init(NewGameSubPageAvatarPresetPage page)
		{
			this._page = page;
			this.deleteButton.onClick.RemoveAllListeners();
			this.deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteClick));
			this.renameButton.onClick.RemoveAllListeners();
			this.renameButton.onClick.AddListener(new UnityAction(this.OnRenameClick));
			this.renameInput.onEndEdit.RemoveAllListeners();
			this.renameInput.onEndEdit.AddListener(new UnityAction<string>(this.OnRenameSubmit));
			this.renameInput.onValueChanged.RemoveListener(new UnityAction<string>(this.OnRenameInputChanged));
			this.renameInput.onValueChanged.AddListener(new UnityAction<string>(this.OnRenameInputChanged));
			this.toggle.onValueChanged.RemoveAllListeners();
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			this.addButton.onClick.RemoveAllListeners();
			this.addButton.onClick.AddListener(new UnityAction(this.OnAddClick));
		}

		// Token: 0x0600622B RID: 25131 RVA: 0x002D0DF0 File Offset: 0x002CEFF0
		public void Refresh(int index, AvatarPreset preset, bool isCustomPreset, bool isSelected)
		{
			this._index = index;
			this.indexLabel.text = (this._index + 1).ToString();
			this._isCustomPreset = isCustomPreset;
			this._isHovering = false;
			this.contentNode.SetActive(true);
			this.emptyNode.SetActive(false);
			this.toggle.onValueChanged.RemoveAllListeners();
			this.toggle.SetIsOnWithoutNotify(isSelected);
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			this.avatar.Refresh(preset.Data, 16);
			this.nameLabel.text = (this._isCustomPreset ? (preset.Name ?? LanguageKey.LK_NewGame_Avatar_Default_Preset_Name.TrFormat(index + 1)) : LanguageKey.LK_NewGame_Avatar_SubPage_Preset_DefaultName.TrFormat(index + 1));
			this.nameLabel.gameObject.SetActive(true);
			this.deleteBtnRoot.SetActive(false);
			this.renameButton.gameObject.SetActive(false);
			this.indexLabelNode.SetActive(true);
			this.renameInput.gameObject.SetActive(false);
		}

		// Token: 0x0600622C RID: 25132 RVA: 0x002D0F2D File Offset: 0x002CF12D
		public void RefreshEmpty(int index)
		{
			this._index = index;
			this._isCustomPreset = false;
			this._isHovering = false;
			this.contentNode.SetActive(false);
			this.emptyNode.SetActive(true);
		}

		// Token: 0x0600622D RID: 25133 RVA: 0x002D0F60 File Offset: 0x002CF160
		private void OnDeleteClick()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.LK_NewGame_Avatar_Delete_Preset_Title.Tr(),
				Content = LanguageKey.LK_NewGame_Avatar_Delete_Preset_Desc.Tr(),
				Type = 1,
				Yes = delegate()
				{
					NewGameSubPageAvatarPresetPage page = this._page;
					if (page != null)
					{
						page.OnDeletePreset(this._index);
					}
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x0600622E RID: 25134 RVA: 0x002D0FD8 File Offset: 0x002CF1D8
		private void OnRenameClick()
		{
			new RenameCfg
			{
				Title = LanguageKey.LK_Building_QuickAction_Rename_Title.Tr(),
				EmptyDesc = LanguageKey.Lk_NewGameSubPageAvatar_ChangeName_Tips.Tr(),
				Submit = new Action<string>(this.OnRenameSubmit),
				IsHideDescription = true,
				CharCount = ViewBuildingManage.GetBuildingNameCharCount()
			}.Show();
		}

		// Token: 0x0600622F RID: 25135 RVA: 0x002D1038 File Offset: 0x002CF238
		private void OnRenameSubmit(string newName)
		{
			this.renameInput.gameObject.SetActive(false);
			this.nameLabel.gameObject.SetActive(true);
			bool flag = !string.IsNullOrEmpty(newName);
			if (flag)
			{
				this.renameInput.FixAndSetInputFieldText(ref newName, this._renameInputFontAsset);
				bool flag2 = string.IsNullOrEmpty(newName);
				if (!flag2)
				{
					bool flag3 = this.renameInput.SensitiveWordHandle(ref newName);
					if (!flag3)
					{
						NewGameSubPageAvatarPresetPage page = this._page;
						if (page != null)
						{
							page.OnRenamePreset(this._index, newName);
						}
					}
				}
			}
		}

		// Token: 0x06006230 RID: 25136 RVA: 0x002D10C5 File Offset: 0x002CF2C5
		private void OnRenameInputChanged(string value)
		{
			this.renameInput.FixAndSetInputFieldText(ref value, this._renameInputFontAsset);
		}

		// Token: 0x06006231 RID: 25137 RVA: 0x002D10DC File Offset: 0x002CF2DC
		private void OnToggleChanged(bool isOn)
		{
			if (isOn)
			{
				NewGameSubPageAvatarPresetPage page = this._page;
				if (page != null)
				{
					page.OnSelectPreset(this._index);
				}
			}
			else
			{
				NewGameSubPageAvatarPresetPage page2 = this._page;
				if (page2 != null)
				{
					page2.OnDeselectPreset(this._index);
				}
			}
		}

		// Token: 0x06006232 RID: 25138 RVA: 0x002D1125 File Offset: 0x002CF325
		private void OnAddClick()
		{
			NewGameSubPageAvatarPresetPage page = this._page;
			if (page != null)
			{
				page.OnAddPreset();
			}
		}

		// Token: 0x04004453 RID: 17491
		[Header("内容节点")]
		[SerializeField]
		private GameObject contentNode;

		// Token: 0x04004454 RID: 17492
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004455 RID: 17493
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04004456 RID: 17494
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x04004457 RID: 17495
		[SerializeField]
		private CButton renameButton;

		// Token: 0x04004458 RID: 17496
		[SerializeField]
		private TMP_InputField renameInput;

		// Token: 0x04004459 RID: 17497
		[SerializeField]
		private CToggle toggle;

		// Token: 0x0400445A RID: 17498
		[SerializeField]
		private GameObject deleteBtnRoot;

		// Token: 0x0400445B RID: 17499
		[SerializeField]
		private TextMeshProUGUI indexLabel;

		// Token: 0x0400445C RID: 17500
		[SerializeField]
		private GameObject indexLabelNode;

		// Token: 0x0400445D RID: 17501
		[Header("空状态节点")]
		[SerializeField]
		private GameObject emptyNode;

		// Token: 0x0400445E RID: 17502
		[SerializeField]
		private CButton addButton;

		// Token: 0x0400445F RID: 17503
		private int _index;

		// Token: 0x04004460 RID: 17504
		private NewGameSubPageAvatarPresetPage _page;

		// Token: 0x04004461 RID: 17505
		private bool _isCustomPreset;

		// Token: 0x04004462 RID: 17506
		private RectTransform _rectTransform;

		// Token: 0x04004463 RID: 17507
		private bool _isHovering;

		// Token: 0x04004464 RID: 17508
		private TMP_FontAsset _renameInputFontAsset;
	}
}
