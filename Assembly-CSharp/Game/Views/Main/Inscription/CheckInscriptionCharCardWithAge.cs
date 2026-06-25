using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Main.Inscription
{
	// Token: 0x02000976 RID: 2422
	public class CheckInscriptionCharCardWithAge : MonoBehaviour
	{
		// Token: 0x17000D27 RID: 3367
		// (get) Token: 0x0600743C RID: 29756 RVA: 0x00361EA8 File Offset: 0x003600A8
		public GameObject AgeObj
		{
			get
			{
				return this.ageObj;
			}
		}

		// Token: 0x0600743D RID: 29757 RVA: 0x00361EB0 File Offset: 0x003600B0
		private void Awake()
		{
			this._rectTransform = base.GetComponent<RectTransform>();
			this.cardButton.onClick.AddListener(new UnityAction(this.OnCardClicked));
			this.pinButton.onClick.AddListener(new UnityAction(this.OnPinClicked));
			this.deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteClicked));
			bool flag = this.ageSlider != null;
			if (flag)
			{
				this.ageSlider.minValue = 1f;
				this.ageSlider.maxValue = 100f;
				this.ageSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnAgeChanged));
			}
			bool flag2 = this.selectionToggle != null;
			if (flag2)
			{
				this.selectionToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
		}

		// Token: 0x0600743E RID: 29758 RVA: 0x00361FA4 File Offset: 0x003601A4
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

		// Token: 0x0600743F RID: 29759 RVA: 0x00361FF7 File Offset: 0x003601F7
		private void UpdateHoverButtonsVisibility()
		{
			this.pinButton.gameObject.SetActive(this._isHovering);
			this.deleteButton.gameObject.SetActive(this._isHovering);
		}

		// Token: 0x06007440 RID: 29760 RVA: 0x00362028 File Offset: 0x00360228
		public void SetData(CheckInscriptionCharData data, ViewSelectInscriptionForEvolution parentView, bool isViewing, bool isSelected, short selectedAge)
		{
			this._data = data;
			this._parentView = parentView;
			this._selectedAge = selectedAge;
			this.nameLabel.text = data.Character.Surname + data.Character.GivenName;
			AvatarRelatedData displayData = data.Character.GenerateAvatarRelatedData();
			this.avatar.Refresh(displayData);
			GameObject gameObject = this.viewingFrame;
			if (gameObject != null)
			{
				gameObject.SetActive(isViewing);
			}
			bool flag = this.selectionToggle != null;
			if (flag)
			{
				this._isUpdatingToggle = true;
				this.selectionToggle.isOn = isSelected;
				this._isUpdatingToggle = false;
			}
			bool flag2 = this.toggleHoverImage != null;
			if (flag2)
			{
				this.toggleHoverImage.SetSprite(isSelected ? "ui9_btn_inscription_select_1_selected" : "ui9_btn_inscription_select_1", false, null);
			}
			this.pinnedNode.SetActive(data.PinOrder >= 0);
			bool flag3 = this.ageSlider != null;
			if (flag3)
			{
				this.ageSlider.value = (float)selectedAge;
			}
			bool flag4 = this.ageLabel != null;
			if (flag4)
			{
				this.ageLabel.text = LanguageKey.LK_Inscription_Age.TrFormat(selectedAge);
			}
			this.SetCurrentAgeMarker();
			this._isHovering = false;
			this.pinButton.gameObject.SetActive(false);
			this.deleteButton.gameObject.SetActive(false);
		}

		// Token: 0x06007441 RID: 29761 RVA: 0x0036219C File Offset: 0x0036039C
		public void SetToggleActive(bool active)
		{
			bool flag = this.toggleObj != null;
			if (flag)
			{
				this.toggleObj.SetActive(active);
			}
		}

		// Token: 0x06007442 RID: 29762 RVA: 0x003621C9 File Offset: 0x003603C9
		private void OnCardClicked()
		{
			ViewSelectInscriptionForEvolution parentView = this._parentView;
			if (parentView != null)
			{
				parentView.ViewCharInfo(this._data.Key);
			}
		}

		// Token: 0x06007443 RID: 29763 RVA: 0x003621EC File Offset: 0x003603EC
		private void OnToggleValueChanged(bool value)
		{
			bool isUpdatingToggle = this._isUpdatingToggle;
			if (!isUpdatingToggle)
			{
				ViewSelectInscriptionForEvolution parentView = this._parentView;
				if (parentView != null)
				{
					parentView.SetToggleSelection(this._data.Key, value);
				}
				bool flag = this.toggleHoverImage != null;
				if (flag)
				{
					this.toggleHoverImage.SetSprite(value ? "ui9_btn_inscription_select_1_selected" : "ui9_btn_inscription_select_1", false, null);
				}
			}
		}

		// Token: 0x06007444 RID: 29764 RVA: 0x00362253 File Offset: 0x00360453
		private void OnPinClicked()
		{
			ViewSelectInscriptionForEvolution parentView = this._parentView;
			if (parentView != null)
			{
				parentView.TogglePin(this._data.Key);
			}
		}

		// Token: 0x06007445 RID: 29765 RVA: 0x00362273 File Offset: 0x00360473
		private void OnDeleteClicked()
		{
			ViewSelectInscriptionForEvolution parentView = this._parentView;
			if (parentView != null)
			{
				parentView.DeleteInscribedCharacter(this._data.Key);
			}
		}

		// Token: 0x06007446 RID: 29766 RVA: 0x00362294 File Offset: 0x00360494
		private void OnAgeChanged(float value)
		{
			short newAge = (short)value;
			this._selectedAge = newAge;
			bool flag = this.ageLabel != null;
			if (flag)
			{
				this.ageLabel.text = LanguageKey.LK_Inscription_Age.TrFormat(newAge);
			}
			ViewSelectInscriptionForEvolution parentView = this._parentView;
			if (parentView != null)
			{
				parentView.SetAge(this._data.Key, newAge);
			}
			this.SetCurrentAgeMarker();
		}

		// Token: 0x06007447 RID: 29767 RVA: 0x00362300 File Offset: 0x00360500
		private void SetCurrentAgeMarker()
		{
			bool flag = this.currentAgeMarker == null || this.ageSlider == null || this._data == null;
			if (!flag)
			{
				short currentAge = this._data.Character.CurrAge;
				bool showMarker = this._selectedAge != currentAge;
				this.currentAgeMarker.gameObject.SetActive(showMarker);
				bool flag2 = showMarker;
				if (flag2)
				{
					float normalizedPos = Mathf.InverseLerp(this.ageSlider.minValue, this.ageSlider.maxValue, (float)currentAge);
					Vector2 anchors = this.currentAgeMarker.anchorMin;
					anchors.x = normalizedPos;
					this.currentAgeMarker.anchorMin = anchors;
					this.currentAgeMarker.anchorMax = anchors;
				}
			}
		}

		// Token: 0x040056B0 RID: 22192
		[SerializeField]
		private CButton cardButton;

		// Token: 0x040056B1 RID: 22193
		[SerializeField]
		private CButton pinButton;

		// Token: 0x040056B2 RID: 22194
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x040056B3 RID: 22195
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040056B4 RID: 22196
		[SerializeField]
		private GameObject viewingFrame;

		// Token: 0x040056B5 RID: 22197
		[SerializeField]
		private CToggle selectionToggle;

		// Token: 0x040056B6 RID: 22198
		[SerializeField]
		private GameObject toggleObj;

		// Token: 0x040056B7 RID: 22199
		[SerializeField]
		private CImage toggleHoverImage;

		// Token: 0x040056B8 RID: 22200
		[SerializeField]
		private GameObject pinnedNode;

		// Token: 0x040056B9 RID: 22201
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040056BA RID: 22202
		[SerializeField]
		private CSlider ageSlider;

		// Token: 0x040056BB RID: 22203
		[SerializeField]
		private TextMeshProUGUI ageLabel;

		// Token: 0x040056BC RID: 22204
		[SerializeField]
		private RectTransform currentAgeMarker;

		// Token: 0x040056BD RID: 22205
		[SerializeField]
		private GameObject ageObj;

		// Token: 0x040056BE RID: 22206
		private RectTransform _rectTransform;

		// Token: 0x040056BF RID: 22207
		private bool _isHovering;

		// Token: 0x040056C0 RID: 22208
		private CheckInscriptionCharData _data;

		// Token: 0x040056C1 RID: 22209
		private ViewSelectInscriptionForEvolution _parentView;

		// Token: 0x040056C2 RID: 22210
		private short _selectedAge;

		// Token: 0x040056C3 RID: 22211
		private bool _isUpdatingToggle;
	}
}
