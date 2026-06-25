using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.EventWindow
{
	// Token: 0x02000EFF RID: 3839
	public class EventWindowDirectSelectCharacterListItem : MonoBehaviour
	{
		// Token: 0x0600B113 RID: 45331 RVA: 0x0050BF1F File Offset: 0x0050A11F
		private void Awake()
		{
			this.EnsureAvatarContentRootCached();
			this.btnMain.ClearAndAddListener(delegate
			{
				Action actionOnClick = this._actionOnClick;
				if (actionOnClick != null)
				{
					actionOnClick();
				}
			});
		}

		// Token: 0x0600B114 RID: 45332 RVA: 0x0050BF44 File Offset: 0x0050A144
		private void EnsureAvatarContentRootCached()
		{
			bool flag = this.avatarContentRoot != null || this.avatar == null;
			if (!flag)
			{
				this._cachedAvatarContentRoot = ((this.avatar.transform as RectTransform) ?? (this.avatar.transform.parent as RectTransform));
			}
		}

		// Token: 0x1700140F RID: 5135
		// (get) Token: 0x0600B115 RID: 45333 RVA: 0x0050BFA4 File Offset: 0x0050A1A4
		private RectTransform ResolvedAvatarContentRoot
		{
			get
			{
				return (this.avatarContentRoot != null) ? this.avatarContentRoot : this._cachedAvatarContentRoot;
			}
		}

		// Token: 0x0600B116 RID: 45334 RVA: 0x0050BFC4 File Offset: 0x0050A1C4
		private void SetAvatarDisplayLoading(bool loading)
		{
			if (loading)
			{
				bool isAvatarDisplayLoading = this._isAvatarDisplayLoading;
				if (!isAvatarDisplayLoading)
				{
					this.SetAvatarDisplayLoadingImmediate(true);
				}
			}
			else
			{
				bool flag = !this._isAvatarDisplayLoading;
				if (!flag)
				{
					this.SetAvatarDisplayLoadingImmediate(false);
				}
			}
		}

		// Token: 0x0600B117 RID: 45335 RVA: 0x0050C008 File Offset: 0x0050A208
		private void SetAvatarDisplayLoadingImmediate(bool loading)
		{
			RectTransform root = this.ResolvedAvatarContentRoot;
			if (loading)
			{
				this._isAvatarDisplayLoading = true;
				bool flag = root != null;
				if (flag)
				{
					this._avatarContentRootOriginalPos = root.anchoredPosition;
					root.anchoredPosition = new Vector2(10000f, 10000f);
					this._avatarContentMovedOffscreen = true;
				}
				else
				{
					this._avatarContentMovedOffscreen = false;
				}
				bool flag2 = this.avatarAreaLoading != null;
				if (flag2)
				{
					this.avatarAreaLoading.gameObject.SetActive(true);
				}
			}
			else
			{
				this._isAvatarDisplayLoading = false;
				bool flag3 = this._avatarContentMovedOffscreen && root != null;
				if (flag3)
				{
					root.anchoredPosition = this._avatarContentRootOriginalPos;
					this._avatarContentMovedOffscreen = false;
				}
				bool flag4 = this.avatarAreaLoading != null;
				if (flag4)
				{
					this.avatarAreaLoading.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600B118 RID: 45336 RVA: 0x0050C0E8 File Offset: 0x0050A2E8
		public uint BeginAvatarLoading()
		{
			this._avatarLoadingVersion += 1U;
			this.SetAvatarDisplayLoading(true);
			return this._avatarLoadingVersion;
		}

		// Token: 0x0600B119 RID: 45337 RVA: 0x0050C116 File Offset: 0x0050A316
		public bool IsAvatarLoadingVersionValid(uint version)
		{
			return version == this._avatarLoadingVersion;
		}

		// Token: 0x0600B11A RID: 45338 RVA: 0x0050C124 File Offset: 0x0050A324
		public void EndAvatarLoading(uint version)
		{
			bool flag = !this.IsAvatarLoadingVersionValid(version);
			if (!flag)
			{
				this.SetAvatarDisplayLoading(false);
			}
		}

		// Token: 0x0600B11B RID: 45339 RVA: 0x0050C14A File Offset: 0x0050A34A
		public void SetEmpty()
		{
			this.SetAvatarDisplayLoadingImmediate(false);
			this.avatar.ResetToBlank(false);
			this.characterName.text = string.Empty;
		}

		// Token: 0x0600B11C RID: 45340 RVA: 0x0050C174 File Offset: 0x0050A374
		public void SetApprove(CharacterDisplayData data, short approve, bool selected, Action actionOnClick)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetBasic(data);
			}
			this.approvementArea.gameObject.SetActive(true);
			this._actionOnClick = actionOnClick;
			this.selectedBg.gameObject.SetActive(selected);
			this.approvementText.text = LocalStringManager.GetFormat(LanguageKey.Lk_EventWindow_CharacterApprovement, ((float)approve / 10f).ToString());
		}

		// Token: 0x0600B11D RID: 45341 RVA: 0x0050C1ED File Offset: 0x0050A3ED
		public void SetBasic(AvatarRelatedData avatarData, string nameData)
		{
			this.avatar.Refresh(avatarData);
			this.characterName.text = nameData;
		}

		// Token: 0x0600B11E RID: 45342 RVA: 0x0050C20A File Offset: 0x0050A40A
		public void SetAvatar(AvatarRelatedData avatarData, Action actionOnClick)
		{
			this._actionOnClick = actionOnClick;
			this.avatar.Refresh(avatarData);
			this.characterName.text = LanguageKey.Lk_EventWindow_SelectAvatar_EmptyName.Tr();
			this.approvementArea.gameObject.SetActive(false);
		}

		// Token: 0x0600B11F RID: 45343 RVA: 0x0050C249 File Offset: 0x0050A449
		public void SetName(string charName)
		{
			this.characterName.text = charName;
		}

		// Token: 0x0600B120 RID: 45344 RVA: 0x0050C258 File Offset: 0x0050A458
		private void SetBasic(CharacterDisplayData data)
		{
			this.avatar.Refresh(data, true);
			this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(data, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		}

		// Token: 0x0600B121 RID: 45345 RVA: 0x0050C28D File Offset: 0x0050A48D
		public void SetInteractable(bool interactable)
		{
			this.btnMain.interactable = interactable;
			this.disableStyleRoot.SetStyleEffect(!interactable, false);
		}

		// Token: 0x0600B122 RID: 45346 RVA: 0x0050C2AE File Offset: 0x0050A4AE
		private void OnDisable()
		{
			this.SetAvatarDisplayLoadingImmediate(false);
		}

		// Token: 0x0400891A RID: 35098
		[SerializeField]
		protected GameObject approvementArea;

		// Token: 0x0400891B RID: 35099
		[SerializeField]
		protected GameObject selectedBg;

		// Token: 0x0400891C RID: 35100
		[SerializeField]
		protected CButton btnMain;

		// Token: 0x0400891D RID: 35101
		[SerializeField]
		protected TextMeshProUGUI approvementText;

		// Token: 0x0400891E RID: 35102
		[SerializeField]
		protected Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400891F RID: 35103
		[Header("头像加载显示（参考 ViewCharacterMenuEquip / DriveWugKingPanel）")]
		[Tooltip("可选；加载中显示。为空时仅位移隐藏头像内容。")]
		[SerializeField]
		private LoadingAnimation avatarAreaLoading;

		// Token: 0x04008920 RID: 35104
		[Tooltip("加载期间移出屏幕的内容根；为空时尝试使用 Avatar 或其父节点。")]
		[SerializeField]
		private RectTransform avatarContentRoot;

		// Token: 0x04008921 RID: 35105
		[SerializeField]
		protected TextMeshProUGUI characterName;

		// Token: 0x04008922 RID: 35106
		[SerializeField]
		public TooltipInvoker mouseTip;

		// Token: 0x04008923 RID: 35107
		[SerializeField]
		protected DisableStyleRoot disableStyleRoot;

		// Token: 0x04008924 RID: 35108
		private Action _actionOnClick;

		// Token: 0x04008925 RID: 35109
		private RectTransform _cachedAvatarContentRoot;

		// Token: 0x04008926 RID: 35110
		private Vector2 _avatarContentRootOriginalPos;

		// Token: 0x04008927 RID: 35111
		private bool _avatarContentMovedOffscreen;

		// Token: 0x04008928 RID: 35112
		private bool _isAvatarDisplayLoading;

		// Token: 0x04008929 RID: 35113
		private uint _avatarLoadingVersion;
	}
}
