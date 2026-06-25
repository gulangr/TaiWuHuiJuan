using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Global.Inscription;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Main.Inscription
{
	// Token: 0x02000975 RID: 2421
	public class CheckInscriptionCharCard : MonoBehaviour
	{
		// Token: 0x06007434 RID: 29748 RVA: 0x00361BB4 File Offset: 0x0035FDB4
		private void Awake()
		{
			this._rectTransform = base.GetComponent<RectTransform>();
			this.cardButton.onClick.AddListener(new UnityAction(this.OnCardClicked));
			this.pinButton.onClick.AddListener(new UnityAction(this.OnPinClicked));
			this.deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteClicked));
		}

		// Token: 0x06007435 RID: 29749 RVA: 0x00361C28 File Offset: 0x0035FE28
		private void Update()
		{
			bool wasHovering = this._isHovering;
			this._isHovering = RectTransformUtility.RectangleContainsScreenPoint(this._rectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
			bool flag = this._isHovering != wasHovering;
			if (flag)
			{
				this.UpdateHoverButtonsVisibility();
			}
		}

		// Token: 0x06007436 RID: 29750 RVA: 0x00361C7B File Offset: 0x0035FE7B
		private void UpdateHoverButtonsVisibility()
		{
			this.pinButton.gameObject.SetActive(this._showPin && this._isHovering);
			this.deleteButton.gameObject.SetActive(this._isHovering);
		}

		// Token: 0x06007437 RID: 29751 RVA: 0x00361CB8 File Offset: 0x0035FEB8
		public void SetData(CheckInscriptionCharData data, InscribedCharacterKey selectedKey, object parentView, bool showPin = true)
		{
			this._data = data;
			this._parentView = parentView;
			this._showPin = showPin;
			this.nameLabel.text = data.Character.Surname + data.Character.GivenName;
			AvatarRelatedData displayData = data.Character.GenerateAvatarRelatedData();
			this.avatar.Refresh(displayData);
			this.selectedFrame.SetActive(selectedKey.Equals(data.Key));
			this.pinnedNode.SetActive(this._showPin && data.PinOrder >= 0);
			this._isHovering = false;
			this.pinButton.gameObject.SetActive(false);
			this.deleteButton.gameObject.SetActive(false);
		}

		// Token: 0x06007438 RID: 29752 RVA: 0x00361D84 File Offset: 0x0035FF84
		private void OnCardClicked()
		{
			ViewCheckInscription view = this._parentView as ViewCheckInscription;
			bool flag = view != null;
			if (flag)
			{
				view.SelectCharByKey(this._data.Key);
			}
			else
			{
				ViewSelectInscriptionForPlay view2 = this._parentView as ViewSelectInscriptionForPlay;
				bool flag2 = view2 != null;
				if (flag2)
				{
					view2.SelectCharByKey(this._data.Key);
				}
			}
		}

		// Token: 0x06007439 RID: 29753 RVA: 0x00361DE0 File Offset: 0x0035FFE0
		private void OnPinClicked()
		{
			ViewCheckInscription view = this._parentView as ViewCheckInscription;
			bool flag = view != null;
			if (flag)
			{
				view.TogglePin(this._data.Key);
			}
			else
			{
				ViewSelectInscriptionForPlay view2 = this._parentView as ViewSelectInscriptionForPlay;
				bool flag2 = view2 != null;
				if (flag2)
				{
					view2.TogglePin(this._data.Key);
				}
			}
		}

		// Token: 0x0600743A RID: 29754 RVA: 0x00361E3C File Offset: 0x0036003C
		private void OnDeleteClicked()
		{
			ViewCheckInscription view = this._parentView as ViewCheckInscription;
			bool flag = view != null;
			if (flag)
			{
				view.DeleteInscribedCharacter(this._data.Key);
			}
			else
			{
				ViewSelectInscriptionForPlay view2 = this._parentView as ViewSelectInscriptionForPlay;
				bool flag2 = view2 != null;
				if (flag2)
				{
					view2.DeleteInscribedCharacter(this._data.Key);
				}
			}
		}

		// Token: 0x040056A4 RID: 22180
		[SerializeField]
		private CButton cardButton;

		// Token: 0x040056A5 RID: 22181
		[SerializeField]
		private CButton pinButton;

		// Token: 0x040056A6 RID: 22182
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x040056A7 RID: 22183
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040056A8 RID: 22184
		[SerializeField]
		private GameObject selectedFrame;

		// Token: 0x040056A9 RID: 22185
		[SerializeField]
		private GameObject pinnedNode;

		// Token: 0x040056AA RID: 22186
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040056AB RID: 22187
		private RectTransform _rectTransform;

		// Token: 0x040056AC RID: 22188
		private bool _isHovering;

		// Token: 0x040056AD RID: 22189
		private CheckInscriptionCharData _data;

		// Token: 0x040056AE RID: 22190
		private object _parentView;

		// Token: 0x040056AF RID: 22191
		private bool _showPin = true;
	}
}
