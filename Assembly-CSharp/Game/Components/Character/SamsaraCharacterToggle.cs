using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F41 RID: 3905
	[RequireComponent(typeof(CToggle))]
	public class SamsaraCharacterToggle : MonoBehaviour
	{
		// Token: 0x0600B35C RID: 45916 RVA: 0x0051A4B7 File Offset: 0x005186B7
		public void SetEmpty()
		{
			this.avatar.ResetToBlank(false);
			this.characterName.text = string.Empty;
		}

		// Token: 0x0600B35D RID: 45917 RVA: 0x0051A4D8 File Offset: 0x005186D8
		public void Set(CharacterDisplayData data)
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
		}

		// Token: 0x0600B35E RID: 45918 RVA: 0x0051A500 File Offset: 0x00518700
		public void Set(DeadCharacter data, ref NameRelatedData nameRelatedData)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetBasic(data, ref nameRelatedData);
			}
		}

		// Token: 0x0600B35F RID: 45919 RVA: 0x0051A528 File Offset: 0x00518728
		public void Set(short templateId)
		{
			bool flag = templateId < 0;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetBasic(templateId);
			}
		}

		// Token: 0x0600B360 RID: 45920 RVA: 0x0051A550 File Offset: 0x00518750
		public void SetAvatarAnchoredPosition(Vector3 pos)
		{
			RectTransform rectTrans = this.avatar.transform as RectTransform;
			rectTrans.anchoredPosition = pos;
		}

		// Token: 0x0600B361 RID: 45921 RVA: 0x0051A57C File Offset: 0x0051877C
		public void SetInvisible()
		{
			AtlasInfo.Instance.GetSpriteWithoutPackerName("ui9_back_unknown_avatar", delegate(Sprite sprite)
			{
				this.avatar.Refresh(sprite);
			});
			this.characterName.text = LanguageKey.LK_UnknownCharName.Tr();
		}

		// Token: 0x0600B362 RID: 45922 RVA: 0x0051A5B1 File Offset: 0x005187B1
		public void SetIndex(int index)
		{
			this.textPos.SetSprite(CommonUtils.GetPreexistenceNumberText((sbyte)index), false, null);
		}

		// Token: 0x0600B363 RID: 45923 RVA: 0x0051A5CC File Offset: 0x005187CC
		private void SetBasic(CharacterDisplayData data)
		{
			this.avatar.Refresh(data, true);
			bool isTaiwu = data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(data, isTaiwu);
		}

		// Token: 0x0600B364 RID: 45924 RVA: 0x0051A60E File Offset: 0x0051880E
		private void SetBasic(DeadCharacter deadCharacter, ref NameRelatedData nameRelatedData)
		{
			this.avatar.Refresh(deadCharacter.GenerateAvatarRelatedData(), deadCharacter.TemplateId);
			this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, false, true);
		}

		// Token: 0x0600B365 RID: 45925 RVA: 0x0051A640 File Offset: 0x00518840
		private void SetBasic(short templateId)
		{
			CharacterItem config = Character.Instance.GetItem(templateId);
			this.avatar.Refresh(new AvatarRelatedData(), templateId);
			this.characterName.text = config.Surname + config.GivenName;
		}

		// Token: 0x04008B4C RID: 35660
		[SerializeField]
		protected CToggle toggle;

		// Token: 0x04008B4D RID: 35661
		[SerializeField]
		protected Game.Components.Avatar.Avatar avatar;

		// Token: 0x04008B4E RID: 35662
		[SerializeField]
		protected TextMeshProUGUI characterName;

		// Token: 0x04008B4F RID: 35663
		[SerializeField]
		protected CImage textPos;
	}
}
