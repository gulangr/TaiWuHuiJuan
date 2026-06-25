using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007EF RID: 2031
	public class NewGameSubPageAvatarClothItem : MonoBehaviour
	{
		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x0600632C RID: 25388 RVA: 0x002D691E File Offset: 0x002D4B1E
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x0600632D RID: 25389 RVA: 0x002D6926 File Offset: 0x002D4B26
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x0600632E RID: 25390 RVA: 0x002D692E File Offset: 0x002D4B2E
		public bool NeedsAvatarRefresh
		{
			get
			{
				return this._needsAvatarRefresh;
			}
		}

		// Token: 0x0600632F RID: 25391 RVA: 0x002D6938 File Offset: 0x002D4B38
		public void Refresh(int index, AvatarData previewData, short displayAge, bool isSelected = false, bool isEmpty = false, bool refreshAvatarImmediately = true)
		{
			this._index = index;
			this.indexLabel.text = (this._index + 1).ToString();
			bool flag = this.avatar != null && previewData != null;
			if (flag)
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
			bool flag2 = this.toggle != null;
			if (flag2)
			{
				this.toggle.SetIsOnWithoutNotify(isSelected);
			}
		}

		// Token: 0x06006330 RID: 25392 RVA: 0x002D69F4 File Offset: 0x002D4BF4
		public void RefreshAvatarIfNeeded()
		{
			bool flag = !this._needsAvatarRefresh || this.avatar == null || this._pendingPreviewData == null;
			if (!flag)
			{
				this.avatar.Refresh(this._pendingPreviewData, this._pendingDisplayAge);
				this._needsAvatarRefresh = false;
				this._pendingPreviewData = null;
			}
		}

		// Token: 0x04004517 RID: 17687
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004518 RID: 17688
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04004519 RID: 17689
		[SerializeField]
		private TextMeshProUGUI indexLabel;

		// Token: 0x0400451A RID: 17690
		private int _index;

		// Token: 0x0400451B RID: 17691
		private bool _needsAvatarRefresh;

		// Token: 0x0400451C RID: 17692
		private AvatarData _pendingPreviewData;

		// Token: 0x0400451D RID: 17693
		private short _pendingDisplayAge;
	}
}
