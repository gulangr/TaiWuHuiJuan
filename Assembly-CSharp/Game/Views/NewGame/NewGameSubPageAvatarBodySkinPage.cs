using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x020007EE RID: 2030
	public class NewGameSubPageAvatarBodySkinPage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x0600631C RID: 25372 RVA: 0x002D6030 File Offset: 0x002D4230
		private void Awake()
		{
			bool flag = this.bodyTypeToggleGroup != null;
			if (flag)
			{
				this.bodyTypeToggleGroup.Init(-1);
				this.bodyTypeToggleGroup.OnActiveIndexChange += this.OnBodyTypeChanged;
			}
			bool flag2 = this.skinColorTemplate != null;
			if (flag2)
			{
				this.skinColorTemplate.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x002D6097 File Offset: 0x002D4297
		protected override void OnEnable()
		{
			this.LoadSkinColors();
			this.CreateSkinColorToggles();
			this.LoadHeadOptions();
			this.CreateHeadToggles();
			base.OnEnable();
		}

		// Token: 0x0600631E RID: 25374 RVA: 0x002D60BD File Offset: 0x002D42BD
		public override void UpdateUI()
		{
			this.RefreshFromAvatarData();
		}

		// Token: 0x0600631F RID: 25375 RVA: 0x002D60C8 File Offset: 0x002D42C8
		public void RefreshSwitchSex()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this.RefreshBodyUI();
				this.LoadHeadOptions();
				this.CreateHeadToggles();
				bool flag2 = this._headOptions != null && this._headOptions.Count > 0 && this.GetHeadIndex(avatarData.HeadId) < 0;
				if (flag2)
				{
					avatarData.HeadId = this._headOptions[0].HeadId;
				}
				base.RefreshAvatarAndMarkDirty();
			}
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x002D6148 File Offset: 0x002D4348
		private void LoadSkinColors()
		{
			IAvatarSubPageParent avatarPage = this.AvatarPage;
			Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>> filter = (avatarPage != null) ? avatarPage.GetSkinColorFilter() : null;
			bool flag = this._skinColors != null && filter == null;
			if (!flag)
			{
				List<ValueTuple<byte, Color>> allColors = NewGameSubPageAvatarColorHelper.SkinColors;
				this._skinColors = ((filter != null) ? filter(allColors) : allColors);
			}
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x002D6198 File Offset: 0x002D4398
		private void CreateSkinColorToggles()
		{
			bool flag = this._skinColors == null;
			if (!flag)
			{
				Transform container = this.skinColorToggleGroup.transform;
				CommonUtils.PrepareEnoughChildren(container, this.skinColorTemplate.gameObject, this._skinColors.Count, null);
				this.skinColorToggleGroup.Clear();
				for (int i = 0; i < this._skinColors.Count; i++)
				{
					Color color = this._skinColors[i].Item2;
					Transform toggleTrans = container.GetChild(i);
					CImage colorImage = toggleTrans.GetChild(0).GetChild(0).GetComponent<CImage>();
					colorImage.color = color;
					this.skinColorToggleGroup.Add(toggleTrans.GetComponent<CToggle>());
				}
				bool flag2 = !this._isInitialized;
				if (flag2)
				{
					this.skinColorToggleGroup.Init(-1);
					this.skinColorToggleGroup.OnActiveIndexChange += this.OnSkinColorChanged;
					this._isInitialized = true;
				}
			}
		}

		// Token: 0x06006322 RID: 25378 RVA: 0x002D62A0 File Offset: 0x002D44A0
		private void LoadHeadOptions()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				byte avatarId = avatarData.AvatarId;
				this._headOptions = new List<AvatarHeadItem>();
				foreach (byte key in AvatarHead.Instance.GetAllKeys())
				{
					AvatarHeadItem item = AvatarHead.Instance[key];
					bool flag2 = item.AvatarId == avatarId && item.CanRandom;
					if (flag2)
					{
						this._headOptions.Add(item);
					}
				}
				this._headOptions.Sort((AvatarHeadItem a, AvatarHeadItem b) => a.HeadId.CompareTo(b.HeadId));
			}
		}

		// Token: 0x06006323 RID: 25379 RVA: 0x002D6380 File Offset: 0x002D4580
		private void CreateHeadToggles()
		{
			bool flag = this._headOptions == null || this._headOptions.Count == 0;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					byte avatarId = avatarData.AvatarId;
					this.headToggleGroup.Clear();
					int count = Mathf.Min(this._headOptions.Count, this.headPreviewImages.Length);
					for (int i = 0; i < count; i++)
					{
						AvatarHeadItem item = this._headOptions[i];
						Sprite sprite = AvatarAtlasAssets.Instance.GetSprite(avatarId, item.NameOrPath, 1);
						bool flag3 = sprite != null;
						if (flag3)
						{
							this.headPreviewImages[i].sprite = sprite;
							this.headPreviewImages[i].SetNativeSize();
						}
						this.headToggleGroup.Add(this.headToggleGroup.transform.GetChild(i).GetComponent<CToggle>());
					}
					bool flag4 = !this._headInitialized;
					if (flag4)
					{
						this.headToggleGroup.Init(-1);
						this.headToggleGroup.OnActiveIndexChange += this.OnHeadChanged;
						this._headInitialized = true;
					}
				}
			}
		}

		// Token: 0x06006324 RID: 25380 RVA: 0x002D64C4 File Offset: 0x002D46C4
		private void RefreshFromAvatarData()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				bool flag2 = this.bodyTypeToggleGroup != null;
				if (flag2)
				{
					int bodyType = (int)avatarData.GetBodyType();
					this.bodyTypeToggleGroup.Set(bodyType, false);
				}
				bool flag3 = this.skinColorToggleGroup != null && this._skinColors != null;
				if (flag3)
				{
					int index = this.GetSkinColorIndex(avatarData.ColorSkinId);
					bool flag4 = index >= 0;
					if (flag4)
					{
						this.skinColorToggleGroup.Set(index, false);
					}
				}
				bool flag5 = this.headToggleGroup != null && this._headOptions != null;
				if (flag5)
				{
					int index2 = this.GetHeadIndex(avatarData.HeadId);
					bool flag6 = index2 >= 0;
					if (flag6)
					{
						this.headToggleGroup.Set(index2, false);
					}
				}
				sbyte i = 0;
				while ((int)i < this.bodyTypeToggleGroup.GetAll().Count)
				{
					Selectable selectable = this.bodyTypeToggleGroup.Get((int)i);
					IAvatarSubPageParent avatarPage = this.AvatarPage;
					bool interactable;
					if (avatarPage == null)
					{
						interactable = true;
					}
					else
					{
						Func<sbyte, bool> bodyTypeFilter = avatarPage.GetBodyTypeFilter();
						bool? flag7 = (bodyTypeFilter != null) ? new bool?(bodyTypeFilter(i)) : null;
						bool flag8 = false;
						interactable = !(flag7.GetValueOrDefault() == flag8 & flag7 != null);
					}
					selectable.interactable = interactable;
					bool interactable2 = this.bodyTypeToggleGroup.Get((int)i).interactable;
					if (interactable2)
					{
						HSVStyleRoot component = this.bodyTypeToggleGroup.Get((int)i).GetComponent<HSVStyleRoot>();
						if (component != null)
						{
							component.SetDefault();
						}
					}
					else
					{
						HSVStyleRoot component2 = this.bodyTypeToggleGroup.Get((int)i).GetComponent<HSVStyleRoot>();
						if (component2 != null)
						{
							component2.SetDefaultBlack();
						}
					}
					i += 1;
				}
			}
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x002D668C File Offset: 0x002D488C
		private int GetSkinColorIndex(byte skinId)
		{
			bool flag = this._skinColors == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._skinColors.Count; i++)
				{
					bool flag2 = this._skinColors[i].Item1 == skinId;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x002D66EC File Offset: 0x002D48EC
		private int GetHeadIndex(byte headId)
		{
			bool flag = this._headOptions == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._headOptions.Count; i++)
				{
					bool flag2 = this._headOptions[i].HeadId == headId;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06006327 RID: 25383 RVA: 0x002D674C File Offset: 0x002D494C
		private void OnBodyTypeChanged(int newIndex, int oldIndex)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				avatarData.ChangeBodyType((sbyte)newIndex);
				this.LoadHeadOptions();
				this.CreateHeadToggles();
				bool flag2 = this._headOptions != null && this._headOptions.Count > 0 && this.GetHeadIndex(avatarData.HeadId) < 0;
				if (flag2)
				{
					avatarData.HeadId = this._headOptions[0].HeadId;
				}
				base.RefreshAvatarAndMarkDirty();
			}
		}

		// Token: 0x06006328 RID: 25384 RVA: 0x002D67D0 File Offset: 0x002D49D0
		private void OnSkinColorChanged(int newIndex, int oldIndex)
		{
			bool flag = this._skinColors == null || newIndex < 0 || newIndex >= this._skinColors.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					avatarData.ColorSkinId = this._skinColors[newIndex].Item1;
					base.RefreshAvatarAndMarkDirty();
				}
			}
		}

		// Token: 0x06006329 RID: 25385 RVA: 0x002D6834 File Offset: 0x002D4A34
		private void OnHeadChanged(int newIndex, int oldIndex)
		{
			bool flag = this._headOptions == null || newIndex < 0 || newIndex >= this._headOptions.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					avatarData.HeadId = this._headOptions[newIndex].HeadId;
					base.RefreshAvatarAndMarkDirty();
				}
			}
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x002D6898 File Offset: 0x002D4A98
		private void RefreshBodyUI()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				bool flag2 = this.bodyTypeImages != null;
				if (flag2)
				{
					sbyte gender = avatarData.GetGender();
					sbyte index = gender;
					for (int i = 0; i < this.bodyTypeImages.Length; i++)
					{
						this.bodyTypeImages[i].sprite = this.bodyTypeSprites[(int)index];
						index += 2;
					}
				}
			}
		}

		// Token: 0x0400450C RID: 17676
		[Header("体型")]
		[SerializeField]
		private CToggleGroup bodyTypeToggleGroup;

		// Token: 0x0400450D RID: 17677
		[SerializeField]
		private CImage[] bodyTypeImages;

		// Token: 0x0400450E RID: 17678
		[SerializeField]
		private Sprite[] bodyTypeSprites;

		// Token: 0x0400450F RID: 17679
		[Header("肤色")]
		[SerializeField]
		private CToggleGroup skinColorToggleGroup;

		// Token: 0x04004510 RID: 17680
		[SerializeField]
		private CToggle skinColorTemplate;

		// Token: 0x04004511 RID: 17681
		[Header("头型")]
		[SerializeField]
		private CToggleGroup headToggleGroup;

		// Token: 0x04004512 RID: 17682
		[SerializeField]
		private CImage[] headPreviewImages;

		// Token: 0x04004513 RID: 17683
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private List<ValueTuple<byte, Color>> _skinColors;

		// Token: 0x04004514 RID: 17684
		private List<AvatarHeadItem> _headOptions;

		// Token: 0x04004515 RID: 17685
		private bool _isInitialized = false;

		// Token: 0x04004516 RID: 17686
		private bool _headInitialized;
	}
}
