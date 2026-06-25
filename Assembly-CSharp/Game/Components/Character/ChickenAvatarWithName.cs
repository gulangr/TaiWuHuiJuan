using System;
using System.IO;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.CellContent;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Components.Character
{
	// Token: 0x02000F1E RID: 3870
	public class ChickenAvatarWithName : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600B248 RID: 45640 RVA: 0x005127C4 File Offset: 0x005109C4
		private void RestoreColorState()
		{
			ColorMultiplyStyleRoot colorMultiplyStyleRoot = this.colorMultiplyStyleRoot;
			if (colorMultiplyStyleRoot != null)
			{
				colorMultiplyStyleRoot.RestoreAllToWhite();
			}
			this._isDisabled = false;
		}

		// Token: 0x0600B249 RID: 45641 RVA: 0x005127E0 File Offset: 0x005109E0
		private void Awake()
		{
		}

		// Token: 0x0600B24A RID: 45642 RVA: 0x005127E4 File Offset: 0x005109E4
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool interactable = this.button.interactable;
			if (interactable)
			{
				this.frame.sprite = this.frameHoverSprite;
			}
		}

		// Token: 0x0600B24B RID: 45643 RVA: 0x00512815 File Offset: 0x00510A15
		public void OnPointerExit(PointerEventData eventData)
		{
			this.frame.sprite = (this.button.interactable ? this.frameNormalSprite : this.frameDisabledSprite);
		}

		// Token: 0x0600B24C RID: 45644 RVA: 0x00512840 File Offset: 0x00510A40
		public void Set(ChickenAvatarWithNameCellData avatarRelatedData)
		{
			this.RestoreColorState();
			ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/Chicken", avatarRelatedData.iconName), delegate(Sprite sprite)
			{
				this.avatar.sprite = sprite;
				this.avatar.enabled = true;
			}, null, false);
			this.nameLabel.text = avatarRelatedData.DisplayName;
			bool flag = this.button != null;
			if (flag)
			{
				this.button.ClearAndAddListener(new Action(this.OnButtonClicked));
			}
			this.RefreshTips();
		}

		// Token: 0x0600B24D RID: 45645 RVA: 0x005128BC File Offset: 0x00510ABC
		public void SetEmpty()
		{
			this.RestoreColorState();
			bool flag = this.avatar != null;
			if (flag)
			{
				this.avatar.SetSprite("", false, null);
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

		// Token: 0x0600B24E RID: 45646 RVA: 0x00512938 File Offset: 0x00510B38
		public void SetSelected(bool selected)
		{
			this.frame.sprite = (selected ? this.frameSelectdSprite : (this.button.interactable ? this.frameNormalSprite : this.frameDisabledSprite));
			this.selectedBg.gameObject.SetActive(selected);
		}

		// Token: 0x0600B24F RID: 45647 RVA: 0x0051298C File Offset: 0x00510B8C
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

		// Token: 0x0600B250 RID: 45648 RVA: 0x005129FE File Offset: 0x00510BFE
		private void OnButtonClicked()
		{
		}

		// Token: 0x0600B251 RID: 45649 RVA: 0x00512A01 File Offset: 0x00510C01
		private void RefreshTips()
		{
		}

		// Token: 0x0600B252 RID: 45650 RVA: 0x00512A04 File Offset: 0x00510C04
		public void SetMouseTipModifier(Action<TooltipInvoker, int> modifier)
		{
			this._mouseTipModifier = modifier;
			bool flag = this.mouseTip != null && this.mouseTip.enabled;
			if (flag)
			{
				this.RefreshTips();
			}
		}

		// Token: 0x04008A48 RID: 35400
		[SerializeField]
		private CImage avatar;

		// Token: 0x04008A49 RID: 35401
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04008A4A RID: 35402
		[SerializeField]
		private CButton button;

		// Token: 0x04008A4B RID: 35403
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008A4C RID: 35404
		[SerializeField]
		private CImage selectedBg;

		// Token: 0x04008A4D RID: 35405
		[SerializeField]
		private CImage frame;

		// Token: 0x04008A4E RID: 35406
		[SerializeField]
		private Sprite frameNormalSprite;

		// Token: 0x04008A4F RID: 35407
		[SerializeField]
		private Sprite frameHoverSprite;

		// Token: 0x04008A50 RID: 35408
		[SerializeField]
		private Sprite frameSelectdSprite;

		// Token: 0x04008A51 RID: 35409
		[SerializeField]
		private Sprite frameDisabledSprite;

		// Token: 0x04008A52 RID: 35410
		[SerializeField]
		private ColorMultiplyStyleRoot colorMultiplyStyleRoot;

		// Token: 0x04008A53 RID: 35411
		[Header("配置类")]
		[SerializeField]
		private EAvatarWithNameTipType tipType = EAvatarWithNameTipType.Detail;

		// Token: 0x04008A54 RID: 35412
		private bool _isDisabled;

		// Token: 0x04008A55 RID: 35413
		private Action<TooltipInvoker, int> _mouseTipModifier;
	}
}
