using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F8 RID: 1528
	public class SelectableCharacter : MonoBehaviour
	{
		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06004827 RID: 18471 RVA: 0x0021DCBF File Offset: 0x0021BEBF
		// (set) Token: 0x06004828 RID: 18472 RVA: 0x0021DCC8 File Offset: 0x0021BEC8
		public int CharacterId
		{
			get
			{
				return this._characterId;
			}
			set
			{
				bool flag = !this._initFlag;
				if (flag)
				{
					this.InitHandlers();
				}
				bool flag2 = this._characterId == value;
				if (!flag2)
				{
					this._characterId = value;
					this._avatarHandler.CharacterId = this._characterId;
					this._nameHandler.CharacterId = this._characterId;
				}
			}
		}

		// Token: 0x06004829 RID: 18473 RVA: 0x0021DD24 File Offset: 0x0021BF24
		private void Awake()
		{
			this.toggle.onValueChanged.RemoveAllListeners();
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x0600482A RID: 18474 RVA: 0x0021DD55 File Offset: 0x0021BF55
		private void OnValueChanged(bool val)
		{
			this.selected.SetActive(val);
		}

		// Token: 0x0600482B RID: 18475 RVA: 0x0021DD68 File Offset: 0x0021BF68
		private void InitHandlers()
		{
			this.avatar.ResetToBlank(false);
			this._avatarHandler = new CharacterAvatar(this.avatar, true);
			this._avatarHandler.CanShowGrave = false;
			this._nameHandler = new CharacterName(this.characterName, null, null);
			this._initFlag = true;
		}

		// Token: 0x0600482C RID: 18476 RVA: 0x0021DDBC File Offset: 0x0021BFBC
		public void SetIsDeadCharacter(bool isDead)
		{
			bool flag = !this._initFlag;
			if (flag)
			{
				this.InitHandlers();
			}
			this._avatarHandler.SetIsDead(isDead);
			this._nameHandler.SetIsDead(isDead);
		}

		// Token: 0x0600482D RID: 18477 RVA: 0x0021DDF8 File Offset: 0x0021BFF8
		public void SetData(CharacterDisplayData displayData)
		{
			bool flag = displayData == null;
			if (flag)
			{
				this.avatar.ResetToBlank(false);
				this.characterName.text = LanguageKey.LK_None.Tr();
			}
			else
			{
				string charName = NameCenter.GetMonasticTitleOrDisplayName(displayData, displayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				this.avatar.Refresh(displayData, true);
				this.characterName.text = charName;
			}
			this._setByData = true;
		}

		// Token: 0x0600482E RID: 18478 RVA: 0x0021DE74 File Offset: 0x0021C074
		public void SetData(CharacterDisplayDataForTooltip displayData)
		{
			bool flag = displayData == null;
			if (flag)
			{
				this.avatar.ResetToBlank(false);
				this.characterName.text = LanguageKey.LK_None.Tr();
			}
			else
			{
				bool isTaiwu = displayData.Id == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				NameRelatedData nameRelatedData = displayData.GetNameRelatedData();
				string charName = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, isTaiwu, false);
				this.avatar.Refresh(displayData.AvatarRelatedData);
				this.characterName.text = charName;
			}
			this._setByData = true;
		}

		// Token: 0x0600482F RID: 18479 RVA: 0x0021DF00 File Offset: 0x0021C100
		public void OnCharacterItemHoverIn()
		{
			bool flag = !this._initFlag && !this._setByData;
			if (flag)
			{
				this.InitHandlers();
			}
			bool flag2 = this.toggle && this.toggle.interactable;
			if (flag2)
			{
				this.hoverLight.SetActive(true);
			}
		}

		// Token: 0x06004830 RID: 18480 RVA: 0x0021DF5B File Offset: 0x0021C15B
		public void OnCharacterItemHoverOut()
		{
			this.hoverLight.SetActive(false);
		}

		// Token: 0x06004831 RID: 18481 RVA: 0x0021DF6B File Offset: 0x0021C16B
		public void ResetToEmpty()
		{
			this.CharacterId = -1;
			this._setByData = false;
		}

		// Token: 0x040031D3 RID: 12755
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x040031D4 RID: 12756
		public CToggle toggle;

		// Token: 0x040031D5 RID: 12757
		public TextMeshProUGUI characterName;

		// Token: 0x040031D6 RID: 12758
		public GameObject selected;

		// Token: 0x040031D7 RID: 12759
		public GameObject hoverLight;

		// Token: 0x040031D8 RID: 12760
		public RectTransform softMask;

		// Token: 0x040031D9 RID: 12761
		public CImage none;

		// Token: 0x040031DA RID: 12762
		public GameObject prevSelected;

		// Token: 0x040031DB RID: 12763
		private int _characterId;

		// Token: 0x040031DC RID: 12764
		private bool _setByData = false;

		// Token: 0x040031DD RID: 12765
		private bool _initFlag;

		// Token: 0x040031DE RID: 12766
		private CharacterAvatar _avatarHandler;

		// Token: 0x040031DF RID: 12767
		private CharacterName _nameHandler;
	}
}
