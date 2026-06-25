using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x020007FC RID: 2044
	public class NewGameSubPageAvatarStyleItem : MonoBehaviour
	{
		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x060063CC RID: 25548 RVA: 0x002DBF23 File Offset: 0x002DA123
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x060063CD RID: 25549 RVA: 0x002DBF2B File Offset: 0x002DA12B
		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
		}

		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x060063CE RID: 25550 RVA: 0x002DBF33 File Offset: 0x002DA133
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x060063CF RID: 25551 RVA: 0x002DBF3B File Offset: 0x002DA13B
		public bool NeedsAvatarRefresh
		{
			get
			{
				return this._needsAvatarRefresh;
			}
		}

		// Token: 0x060063D0 RID: 25552 RVA: 0x002DBF44 File Offset: 0x002DA144
		public void Refresh(int index, AvatarData previewData, short displayAge, bool isSelected, bool isEmpty = false, bool refreshAvatarImmediately = true)
		{
			this._index = index;
			this._isEmpty = isEmpty;
			this.indexLabel.text = this._index.ToString();
			bool flag = this.contentArea != null;
			if (flag)
			{
				this.contentArea.SetActive(!isEmpty);
			}
			bool flag2 = this.toggle != null;
			if (flag2)
			{
				Image backgroundImage = this.toggle.targetGraphic as Image;
				Image checkmarkImage = this.toggle.graphic as Image;
				if (isEmpty)
				{
					bool flag3 = backgroundImage != null && this.emptyBackground != null;
					if (flag3)
					{
						backgroundImage.sprite = this.emptyBackground;
					}
					bool flag4 = checkmarkImage != null && this.emptyCheckmark != null;
					if (flag4)
					{
						checkmarkImage.sprite = this.emptyCheckmark;
					}
				}
				else
				{
					bool flag5 = backgroundImage != null && this.normalBackground != null;
					if (flag5)
					{
						backgroundImage.sprite = this.normalBackground;
					}
					bool flag6 = checkmarkImage != null && this.normalCheckmark != null;
					if (flag6)
					{
						checkmarkImage.sprite = this.normalCheckmark;
					}
				}
				this.toggle.spriteState = new SpriteState
				{
					highlightedSprite = (isEmpty ? this.emptyHoverBackground : this.normalHoverBackground),
					pressedSprite = (isEmpty ? this.emptyCheckmark : this.normalCheckmark),
					selectedSprite = (isEmpty ? this.emptyCheckmark : this.normalCheckmark)
				};
			}
			bool flag7 = !isEmpty && this.avatar != null && previewData != null;
			if (flag7)
			{
				if (refreshAvatarImmediately)
				{
					this.avatar.Refresh(previewData, displayAge);
					this._needsAvatarRefresh = false;
					this._pendingPreviewData = null;
				}
				else
				{
					this._pendingPreviewData = previewData;
					this._pendingDisplayAge = displayAge;
					this._needsAvatarRefresh = true;
				}
			}
			else
			{
				this._needsAvatarRefresh = false;
				this._pendingPreviewData = null;
			}
			bool flag8 = this.toggle != null;
			if (flag8)
			{
				this.toggle.SetIsOnWithoutNotify(isSelected);
			}
		}

		// Token: 0x060063D1 RID: 25553 RVA: 0x002DC184 File Offset: 0x002DA384
		public void RefreshAvatarIfNeeded()
		{
			bool flag = !this._needsAvatarRefresh || this._isEmpty || this.avatar == null || this._pendingPreviewData == null;
			if (!flag)
			{
				this.avatar.Refresh(this._pendingPreviewData, this._pendingDisplayAge);
				this._needsAvatarRefresh = false;
				this._pendingPreviewData = null;
			}
		}

		// Token: 0x040045A0 RID: 17824
		[SerializeField]
		private GameObject contentArea;

		// Token: 0x040045A1 RID: 17825
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040045A2 RID: 17826
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040045A3 RID: 17827
		[SerializeField]
		private TextMeshProUGUI indexLabel;

		// Token: 0x040045A4 RID: 17828
		[Header("Toggle 状态 Sprites")]
		[SerializeField]
		private Sprite normalBackground;

		// Token: 0x040045A5 RID: 17829
		[SerializeField]
		private Sprite normalHoverBackground;

		// Token: 0x040045A6 RID: 17830
		[SerializeField]
		private Sprite normalCheckmark;

		// Token: 0x040045A7 RID: 17831
		[SerializeField]
		private Sprite emptyBackground;

		// Token: 0x040045A8 RID: 17832
		[SerializeField]
		private Sprite emptyHoverBackground;

		// Token: 0x040045A9 RID: 17833
		[SerializeField]
		private Sprite emptyCheckmark;

		// Token: 0x040045AA RID: 17834
		private int _index;

		// Token: 0x040045AB RID: 17835
		private bool _isEmpty;

		// Token: 0x040045AC RID: 17836
		private bool _needsAvatarRefresh;

		// Token: 0x040045AD RID: 17837
		private AvatarData _pendingPreviewData;

		// Token: 0x040045AE RID: 17838
		private short _pendingDisplayAge;
	}
}
