using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Global.Inscription;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007E1 RID: 2017
	public class InscriptionPresetItem : MonoBehaviour
	{
		// Token: 0x06006260 RID: 25184 RVA: 0x002D14C2 File Offset: 0x002CF6C2
		private void Awake()
		{
			this._rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x06006261 RID: 25185 RVA: 0x002D14D4 File Offset: 0x002CF6D4
		private void Update()
		{
			bool wasHovering = this._isHovering;
			this._isHovering = RectTransformUtility.RectangleContainsScreenPoint(this._rectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
			bool flag = this._isHovering != wasHovering;
			if (flag)
			{
				this.deleteBtnRoot.SetActive(this._isHovering);
			}
		}

		// Token: 0x06006262 RID: 25186 RVA: 0x002D1534 File Offset: 0x002CF734
		public void Init(NewGameSubPageAvatarPresetPage page)
		{
			this._page = page;
			this.deleteButton.onClick.RemoveAllListeners();
			this.deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteClick));
			this.toggle.onValueChanged.RemoveAllListeners();
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x06006263 RID: 25187 RVA: 0x002D15A8 File Offset: 0x002CF7A8
		public void Refresh(InscribedCharacterKey key, InscribedCharacter character, bool isSelected)
		{
			this._key = key;
			this._character = character;
			this._isHovering = false;
			this.toggle.onValueChanged.RemoveAllListeners();
			this.toggle.SetIsOnWithoutNotify(isSelected);
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			AvatarRelatedData displayData = character.GenerateAvatarRelatedData();
			this.avatar.Refresh(displayData);
			this.nameLabel.text = character.Surname + character.GivenName;
			this.deleteBtnRoot.SetActive(false);
		}

		// Token: 0x06006264 RID: 25188 RVA: 0x002D1644 File Offset: 0x002CF844
		private void OnDeleteClick()
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character.Tr(),
				Content = LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character_Confirm.Tr(),
				Yes = delegate()
				{
					NewGameSubPageAvatarPresetPage page = this._page;
					if (page != null)
					{
						page.OnDeleteInscribedCharacter(this._key);
					}
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x002D16B4 File Offset: 0x002CF8B4
		private void OnToggleChanged(bool isOn)
		{
			if (isOn)
			{
				NewGameSubPageAvatarPresetPage page = this._page;
				if (page != null)
				{
					page.OnSelectInscribedCharacter(this._key, this._character);
				}
			}
			else
			{
				NewGameSubPageAvatarPresetPage page2 = this._page;
				if (page2 != null)
				{
					page2.OnDeselectInscribedCharacter(this._key);
				}
			}
		}

		// Token: 0x0400446E RID: 17518
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400446F RID: 17519
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04004470 RID: 17520
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x04004471 RID: 17521
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04004472 RID: 17522
		[SerializeField]
		private GameObject deleteBtnRoot;

		// Token: 0x04004473 RID: 17523
		private InscribedCharacterKey _key;

		// Token: 0x04004474 RID: 17524
		private InscribedCharacter _character;

		// Token: 0x04004475 RID: 17525
		private NewGameSubPageAvatarPresetPage _page;

		// Token: 0x04004476 RID: 17526
		private RectTransform _rectTransform;

		// Token: 0x04004477 RID: 17527
		private bool _isHovering;
	}
}
