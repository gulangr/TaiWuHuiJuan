using System;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB3 RID: 2739
	public class CricketCombatResultRewardItem : MonoBehaviour
	{
		// Token: 0x06008668 RID: 34408 RVA: 0x003E8DD0 File Offset: 0x003E6FD0
		public void Set(CricketCombatRewardData data)
		{
			bool isCharacter = data.Kind == CricketCombatRewardKind.Character;
			this.itemModeRoot.SetActive(!isCharacter);
			this.characterModeRoot.SetActive(isCharacter);
			this.nameText.gameObject.SetActive(isCharacter);
			bool flag = isCharacter;
			if (flag)
			{
				this.nameText.text = data.DisplayName;
			}
			bool flag2 = isCharacter;
			if (flag2)
			{
				this.SetCharacterMode(data);
			}
			else
			{
				this.SetItemMode(data);
			}
		}

		// Token: 0x06008669 RID: 34409 RVA: 0x003E8E48 File Offset: 0x003E7048
		private void SetItemMode(CricketCombatRewardData data)
		{
			ItemKey key = data.Content.Key;
			bool isExp = key.ItemType == 12 && key.TemplateId == 8;
			bool isResource = ItemTemplateHelper.IsMiscResource(data.Content.Key.ItemType, data.Content.Key.TemplateId);
			this.itemRowItemMain.SetData(data.Content);
			this.itemCardItem.Set(this.itemRowItemMain, !isExp && !isResource);
		}

		// Token: 0x0600866A RID: 34410 RVA: 0x003E8EC9 File Offset: 0x003E70C9
		private void SetCharacterMode(CricketCombatRewardData data)
		{
			this.avatar.Refresh(data.CharacterDisplayData, true);
		}

		// Token: 0x0400673E RID: 26430
		[SerializeField]
		private GameObject itemModeRoot;

		// Token: 0x0400673F RID: 26431
		[SerializeField]
		private CardItem itemCardItem;

		// Token: 0x04006740 RID: 26432
		[SerializeField]
		private RowItemMain itemRowItemMain;

		// Token: 0x04006741 RID: 26433
		[SerializeField]
		private GameObject characterModeRoot;

		// Token: 0x04006742 RID: 26434
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006743 RID: 26435
		[SerializeField]
		private TextMeshProUGUI nameText;
	}
}
