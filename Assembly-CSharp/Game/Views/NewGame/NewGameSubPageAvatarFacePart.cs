using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x020007F2 RID: 2034
	public class NewGameSubPageAvatarFacePart : MonoBehaviour
	{
		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x0600634C RID: 25420 RVA: 0x002D7834 File Offset: 0x002D5A34
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x0600634D RID: 25421 RVA: 0x002D783C File Offset: 0x002D5A3C
		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
		}

		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x0600634E RID: 25422 RVA: 0x002D7844 File Offset: 0x002D5A44
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x0600634F RID: 25423 RVA: 0x002D784C File Offset: 0x002D5A4C
		public bool NeedsAvatarRefresh
		{
			get
			{
				return this._needsAvatarRefresh;
			}
		}

		// Token: 0x06006350 RID: 25424 RVA: 0x002D7854 File Offset: 0x002D5A54
		public void Refresh(int index, AvatarData avatarData, short age, FacePartType partType, FacePartAvatarPreviewSetting setting, bool isSelected, bool isEmpty = false, bool refreshAvatarImmediately = true)
		{
			this._index = index;
			this.indexLabel.text = (this._index + 1).ToString();
			this._isEmpty = isEmpty;
			bool flag = this.contentArea != null;
			if (flag)
			{
				this.contentArea.SetActive(!isEmpty);
			}
			this.RefreshToggleSprites(isEmpty);
			bool flag2 = !isEmpty && this.avatarPreview != null && avatarData != null;
			if (flag2)
			{
				if (refreshAvatarImmediately)
				{
					this.RefreshAvatarPreview(avatarData, age, partType, setting);
					this._needsAvatarRefresh = false;
					this._pendingAvatarData = null;
				}
				else
				{
					this._pendingAvatarData = avatarData;
					this._pendingAge = age;
					this._pendingPartType = partType;
					this._pendingSetting = setting;
					this._needsAvatarRefresh = true;
				}
			}
			else
			{
				this._needsAvatarRefresh = false;
				this._pendingAvatarData = null;
			}
			bool flag3 = this.toggle != null;
			if (flag3)
			{
				this.toggle.SetIsOnWithoutNotify(isSelected);
			}
		}

		// Token: 0x06006351 RID: 25425 RVA: 0x002D7958 File Offset: 0x002D5B58
		public void RefreshAvatarIfNeeded()
		{
			bool flag = !this._needsAvatarRefresh || this.avatarPreview == null || this._pendingAvatarData == null;
			if (!flag)
			{
				this.RefreshAvatarPreview(this._pendingAvatarData, this._pendingAge, this._pendingPartType, this._pendingSetting);
				this._needsAvatarRefresh = false;
				this._pendingAvatarData = null;
			}
		}

		// Token: 0x06006352 RID: 25426 RVA: 0x002D79BC File Offset: 0x002D5BBC
		private void RefreshToggleSprites(bool isEmpty)
		{
			bool flag = this.toggle == null;
			if (!flag)
			{
				Image backgroundImage = this.toggle.targetGraphic as Image;
				Image checkmarkImage = this.toggle.graphic as Image;
				if (isEmpty)
				{
					bool flag2 = backgroundImage != null && this.emptyBackground != null;
					if (flag2)
					{
						backgroundImage.sprite = this.emptyBackground;
					}
					bool flag3 = checkmarkImage != null && this.emptyCheckmark != null;
					if (flag3)
					{
						checkmarkImage.sprite = this.emptyCheckmark;
					}
				}
				else
				{
					bool flag4 = backgroundImage != null && this.normalBackground != null;
					if (flag4)
					{
						backgroundImage.sprite = this.normalBackground;
					}
					bool flag5 = checkmarkImage != null && this.normalCheckmark != null;
					if (flag5)
					{
						checkmarkImage.sprite = this.normalCheckmark;
					}
				}
			}
		}

		// Token: 0x06006353 RID: 25427 RVA: 0x002D7AB8 File Offset: 0x002D5CB8
		private void RefreshAvatarPreview(AvatarData avatarData, short age, FacePartType partType, FacePartAvatarPreviewSetting setting)
		{
			sbyte bodyType = avatarData.GetBodyType();
			Vector2 offset = (setting != null) ? setting.GetOffset(bodyType) : Vector2.zero;
			float scale = (setting != null) ? setting.GetScale(bodyType) : 1f;
			bool flag = this.avatarPivotParent != null;
			if (flag)
			{
				this.avatarPivotParent.anchoredPosition = offset;
			}
			Game.Components.Avatar.Avatar avatar = this.avatarPreview;
			if (!true)
			{
			}
			EFacePartVisibility facePartVisibility;
			switch (partType)
			{
			case FacePartType.Eyebrow:
				facePartVisibility = EFacePartVisibility.Eyebrow;
				break;
			case FacePartType.Eyes:
				facePartVisibility = EFacePartVisibility.Eyes;
				break;
			case FacePartType.Nose:
				facePartVisibility = EFacePartVisibility.Nose;
				break;
			case FacePartType.Mouth:
				facePartVisibility = EFacePartVisibility.Mouth;
				break;
			default:
				facePartVisibility = EFacePartVisibility.All;
				break;
			}
			if (!true)
			{
			}
			avatar.FacePartVisibility = facePartVisibility;
			this.avatarPreview.transform.localScale = Vector3.one * scale;
			this.avatarPreview.Refresh(avatarData, age);
			this.RefreshDefaultFacePartImages(avatarData, partType);
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x002D7B94 File Offset: 0x002D5D94
		private void RefreshDefaultFacePartImages(AvatarData avatarData, FacePartType currentPartType)
		{
			bool flag = this.defaultFacePartImage == null;
			if (!flag)
			{
				this.LoadDefaultFacePartImage(this.defaultFacePartImage, avatarData, currentPartType);
				this.defaultFacePartImage.gameObject.SetActive(true);
			}
		}

		// Token: 0x06006355 RID: 25429 RVA: 0x002D7BD8 File Offset: 0x002D5DD8
		private void LoadDefaultFacePartImage(CImage image, AvatarData avatarData, FacePartType currentPartType)
		{
			sbyte gender = avatarData.Gender;
			sbyte bodyType = avatarData.GetBodyType();
			if (!true)
			{
			}
			int num;
			switch (currentPartType)
			{
			case FacePartType.Eyebrow:
				num = 0;
				break;
			case FacePartType.Eyes:
				num = 1;
				break;
			case FacePartType.Nose:
				num = 2;
				break;
			case FacePartType.Mouth:
				num = 3;
				break;
			default:
				num = 0;
				break;
			}
			if (!true)
			{
			}
			int partState = num;
			string imageName = string.Format("{0}_{1}_{2}_{3}", new object[]
			{
				"ui9_back_create_avatar_default_part",
				gender,
				bodyType,
				partState
			});
			image.SetSprite(imageName, true, null);
		}

		// Token: 0x04004532 RID: 17714
		[SerializeField]
		private GameObject contentArea;

		// Token: 0x04004533 RID: 17715
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04004534 RID: 17716
		[SerializeField]
		private TextMeshProUGUI indexLabel;

		// Token: 0x04004535 RID: 17717
		[Header("Avatar预览")]
		[SerializeField]
		private RectTransform avatarPivotParent;

		// Token: 0x04004536 RID: 17718
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarPreview;

		// Token: 0x04004537 RID: 17719
		[Header("默认脸部组件图片（根据性别体型和当前部件切换）")]
		[SerializeField]
		private CImage defaultFacePartImage;

		// Token: 0x04004538 RID: 17720
		[Header("Toggle 状态 Sprites")]
		[SerializeField]
		private Sprite normalBackground;

		// Token: 0x04004539 RID: 17721
		[SerializeField]
		private Sprite normalCheckmark;

		// Token: 0x0400453A RID: 17722
		[SerializeField]
		private Sprite emptyBackground;

		// Token: 0x0400453B RID: 17723
		[SerializeField]
		private Sprite emptyCheckmark;

		// Token: 0x0400453C RID: 17724
		private int _index;

		// Token: 0x0400453D RID: 17725
		private bool _isEmpty;

		// Token: 0x0400453E RID: 17726
		private bool _needsAvatarRefresh;

		// Token: 0x0400453F RID: 17727
		private AvatarData _pendingAvatarData;

		// Token: 0x04004540 RID: 17728
		private short _pendingAge;

		// Token: 0x04004541 RID: 17729
		private FacePartType _pendingPartType;

		// Token: 0x04004542 RID: 17730
		private FacePartAvatarPreviewSetting _pendingSetting;
	}
}
