using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Components.Character
{
	// Token: 0x02000F13 RID: 3859
	public class AvatarWithName : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600B1D6 RID: 45526 RVA: 0x005102E8 File Offset: 0x0050E4E8
		private void RestoreColorState()
		{
			ColorMultiplyStyleRoot colorMultiplyStyleRoot = this.colorMultiplyStyleRoot;
			if (colorMultiplyStyleRoot != null)
			{
				colorMultiplyStyleRoot.RestoreAllToWhite();
			}
			this._isDisabled = false;
		}

		// Token: 0x0600B1D7 RID: 45527 RVA: 0x00510304 File Offset: 0x0050E504
		private void Awake()
		{
			bool flag = this.nameLabel != null;
			if (flag)
			{
				this._nameLabelOriginalColor = this.nameLabel.color;
			}
		}

		// Token: 0x0600B1D8 RID: 45528 RVA: 0x00510334 File Offset: 0x0050E534
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool interactable = this.button.interactable;
			if (interactable)
			{
				this.frame.sprite = this.frameHoverSprite;
			}
		}

		// Token: 0x0600B1D9 RID: 45529 RVA: 0x00510365 File Offset: 0x0050E565
		public void OnPointerExit(PointerEventData eventData)
		{
			this.frame.sprite = (this.button.interactable ? this.frameNormalSprite : this.frameDisabledSprite);
		}

		// Token: 0x0600B1DA RID: 45530 RVA: 0x00510390 File Offset: 0x0050E590
		public void Set(short merchantTemplateId)
		{
			this.RestoreColorState();
			MerchantItem config = Merchant.Instance[(int)merchantTemplateId];
			this.nameLabel.text = config.UiName;
			this.nameLabel.color = this._nameLabelOriginalColor;
			MerchantTypeItem merchantTypeConfig = MerchantType.Instance[config.MerchantType];
			ResLoader.LoadModOrGameResource<Texture2D>("NpcFace/SmallFace/" + merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(this.avatar.Refresh), null);
		}

		// Token: 0x0600B1DB RID: 45531 RVA: 0x00510410 File Offset: 0x0050E610
		public void Set(string displayName, int characterId, Action<int> onClickCallback = null, bool isCompanion = false, bool isFollowed = false)
		{
			this.RestoreColorState();
			this._characterId = characterId;
			this._onClickCallback = onClickCallback;
			this.SetIsCompanion(isCompanion);
			bool flag = this.avatar != null;
			if (flag)
			{
				this.avatar.RefreshAsGrave();
			}
			bool flag2 = this.nameLabel != null;
			if (flag2)
			{
				this.nameLabel.text = displayName;
				this.nameLabel.color = this._nameLabelOriginalColor;
			}
			bool flag3 = this.button != null;
			if (flag3)
			{
				this.button.ClearAndAddListener(new Action(this.OnButtonClicked));
			}
			bool flag4 = this.followed != null;
			if (flag4)
			{
				this.followed.gameObject.SetActive(isFollowed);
			}
			this.RefreshTips();
		}

		// Token: 0x0600B1DC RID: 45532 RVA: 0x005104E4 File Offset: 0x0050E6E4
		public void Set(AvatarRelatedData avatarRelatedData, short templateId, string displayName, int characterId, Action<int> onClickCallback = null, bool isCompanion = false, bool isFollowed = false)
		{
			this.RestoreColorState();
			this._characterId = characterId;
			this._onClickCallback = onClickCallback;
			this.SetIsCompanion(isCompanion);
			bool flag = this.avatar != null;
			if (flag)
			{
				bool flag2 = avatarRelatedData == null;
				if (flag2)
				{
					this.avatar.ResetToBlank(false);
				}
				else
				{
					this.avatar.Refresh(avatarRelatedData, templateId);
				}
			}
			bool flag3 = this.nameLabel != null;
			if (flag3)
			{
				this.nameLabel.text = displayName;
				this.nameLabel.color = this._nameLabelOriginalColor;
			}
			bool flag4 = this.button != null;
			if (flag4)
			{
				this.button.ClearAndAddListener(new Action(this.OnButtonClicked));
			}
			bool flag5 = this.followed != null;
			if (flag5)
			{
				this.followed.gameObject.SetActive(isFollowed);
			}
			this.RefreshTips();
		}

		// Token: 0x0600B1DD RID: 45533 RVA: 0x005105D8 File Offset: 0x0050E7D8
		public void Set(GroupCharDisplayData data, bool isTaiwu, Action<int> onClickCallback = null, bool isCompanion = false)
		{
			string displayName = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, isTaiwu, false);
			this.Set(data.AvatarRelatedData, data.CharacterTemplateId, displayName, data.CharacterId, onClickCallback, false, false);
			this.SetIsCompanion(isCompanion);
		}

		// Token: 0x0600B1DE RID: 45534 RVA: 0x0051061A File Offset: 0x0050E81A
		private void SetIsCompanion(bool isCompanion)
		{
			CImage cimage = this.companionSign;
			if (cimage != null)
			{
				cimage.gameObject.SetActive(isCompanion);
			}
		}

		// Token: 0x0600B1DF RID: 45535 RVA: 0x00510638 File Offset: 0x0050E838
		public void SetEmpty()
		{
			this.RestoreColorState();
			this._characterId = -1;
			this._onClickCallback = null;
			bool flag = this.avatar != null;
			if (flag)
			{
				this.avatar.ResetToBlank(false);
			}
			bool flag2 = this.nameLabel != null;
			if (flag2)
			{
				this.nameLabel.text = string.Empty;
			}
			bool flag3 = this.mouseTip != null;
			if (flag3)
			{
				this.mouseTip.enabled = false;
			}
		}

		// Token: 0x0600B1E0 RID: 45536 RVA: 0x005106BC File Offset: 0x0050E8BC
		public void SetSelected(bool selected)
		{
			this.frame.sprite = (selected ? this.frameSelectdSprite : (this.button.interactable ? this.frameNormalSprite : this.frameDisabledSprite));
			this.selectedBg.gameObject.SetActive(selected);
		}

		// Token: 0x0600B1E1 RID: 45537 RVA: 0x00510710 File Offset: 0x0050E910
		public void SetDisabled(bool disabled)
		{
			bool flag = this.colorMultiplyStyleRoot == null;
			if (!flag)
			{
				bool flag2 = this._isDisabled == disabled;
				if (!flag2)
				{
					this._isDisabled = disabled;
					if (disabled)
					{
						this.colorMultiplyStyleRoot.MultiplyColor(new Vector4(0.5f, 0.5f, 0.5f, 1f));
					}
					else
					{
						this.colorMultiplyStyleRoot.RestoreAllToWhite();
					}
				}
			}
		}

		// Token: 0x0600B1E2 RID: 45538 RVA: 0x00510782 File Offset: 0x0050E982
		private void OnButtonClicked()
		{
			Action<int> onClickCallback = this._onClickCallback;
			if (onClickCallback != null)
			{
				onClickCallback(this._characterId);
			}
		}

		// Token: 0x0600B1E3 RID: 45539 RVA: 0x005107A0 File Offset: 0x0050E9A0
		private void RefreshTips()
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.enabled = true;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				bool flag2 = this._mouseTipModifier != null;
				if (flag2)
				{
					this._mouseTipModifier(this.mouseTip, this._characterId);
				}
				else
				{
					switch (this.tipType)
					{
					case EAvatarWithNameTipType.Simple:
						this.mouseTip.Type = TipType.Character;
						this.mouseTip.RuntimeParam.Set("charId", this._characterId);
						break;
					case EAvatarWithNameTipType.Detail:
						this.mouseTip.Type = TipType.CharacterOnMapBlock;
						this.mouseTip.RuntimeParam.Set("CharId", this._characterId);
						break;
					case EAvatarWithNameTipType.LifeCombatSkillValue:
						this.mouseTip.Type = TipType.LifeCombatSkillValue;
						this.mouseTip.RuntimeParam.Set("charId", this._characterId);
						break;
					}
				}
			}
		}

		// Token: 0x0600B1E4 RID: 45540 RVA: 0x005108C0 File Offset: 0x0050EAC0
		public void SetMouseTipModifier(Action<TooltipInvoker, int> modifier)
		{
			this._mouseTipModifier = modifier;
			bool flag = this.mouseTip != null && this.mouseTip.enabled;
			if (flag)
			{
				this.RefreshTips();
			}
		}

		// Token: 0x040089D3 RID: 35283
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040089D4 RID: 35284
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040089D5 RID: 35285
		[SerializeField]
		private CButton button;

		// Token: 0x040089D6 RID: 35286
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x040089D7 RID: 35287
		[SerializeField]
		private CImage selectedBg;

		// Token: 0x040089D8 RID: 35288
		[SerializeField]
		private CImage followed;

		// Token: 0x040089D9 RID: 35289
		[SerializeField]
		private CImage frame;

		// Token: 0x040089DA RID: 35290
		[SerializeField]
		private Sprite frameNormalSprite;

		// Token: 0x040089DB RID: 35291
		[SerializeField]
		private Sprite frameHoverSprite;

		// Token: 0x040089DC RID: 35292
		[SerializeField]
		private Sprite frameSelectdSprite;

		// Token: 0x040089DD RID: 35293
		[SerializeField]
		private Sprite frameDisabledSprite;

		// Token: 0x040089DE RID: 35294
		[SerializeField]
		private ColorMultiplyStyleRoot colorMultiplyStyleRoot;

		// Token: 0x040089DF RID: 35295
		[SerializeField]
		private CImage companionSign;

		// Token: 0x040089E0 RID: 35296
		[Header("配置类")]
		[SerializeField]
		private EAvatarWithNameTipType tipType = EAvatarWithNameTipType.Detail;

		// Token: 0x040089E1 RID: 35297
		private int _characterId;

		// Token: 0x040089E2 RID: 35298
		private Action<int> _onClickCallback;

		// Token: 0x040089E3 RID: 35299
		private Color _nameLabelOriginalColor;

		// Token: 0x040089E4 RID: 35300
		private bool _isDisabled;

		// Token: 0x040089E5 RID: 35301
		private Action<TooltipInvoker, int> _mouseTipModifier;
	}
}
