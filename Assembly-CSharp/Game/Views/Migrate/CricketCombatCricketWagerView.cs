using System;
using Config;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x020008EA RID: 2282
	public class CricketCombatCricketWagerView : MonoBehaviour
	{
		// Token: 0x06006D08 RID: 27912 RVA: 0x00324258 File Offset: 0x00322458
		public void SetData(Wager wager)
		{
			bool flag = wager.Type == 1;
			if (flag)
			{
				ItemDomainMethod.AsyncCall.GetItemDisplayData(null, wager.ItemKey, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					ItemDisplayData itemDisplayData = null;
					Serializer.Deserialize(dataPool, offset, ref itemDisplayData);
					this.SetData(wager, itemDisplayData);
				});
			}
			else
			{
				this.SetData(wager, null);
			}
		}

		// Token: 0x06006D09 RID: 27913 RVA: 0x003242C8 File Offset: 0x003224C8
		public void SetData(Wager wager, ItemDisplayData itemDisplayData)
		{
			this.goWagerResource.SetActive(false);
			this.goCardItemName.SetActive(false);
			this.goWagerChar.SetActive(false);
			this.goWagerExp.SetActive(false);
			this.cardItem.gameObject.SetActive(false);
			switch (wager.Type)
			{
			case -1:
				this.descriptionText.text = LocalStringManager.Get("LK_CricketWager_Invalid");
				this.descriptionIcon.gameObject.SetActive(false);
				break;
			case 0:
			{
				ResourceTypeItem config = ResourceType.Instance[wager.WagerResourceType];
				this.descriptionText.text = config.Name + ": " + CommonUtils.GetDisplayStringForNum(wager.Count, 100000);
				this.descriptionIcon.SetSprite(config.Icon, false, null);
				this.descriptionIcon.gameObject.SetActive(true);
				CommonUtils.SetResourceImage(wager.WagerResourceType, wager.Count, this.resourceIcon, 0.6f);
				this.goWagerResource.SetActive(true);
				break;
			}
			case 1:
			{
				this.descriptionText.text = CommonUtils.GetDisplayStringForNum(wager.CalcWagerValue((int)itemDisplayData.Value, 0, 0, 0, -1, 0));
				this.descriptionIcon.SetSprite(ResourceType.Instance[6].Icon, false, null);
				this.descriptionIcon.gameObject.SetActive(true);
				RowItemMain rowItemMain = this.cardItem.GetComponent<RowItemMain>();
				rowItemMain.SetData(itemDisplayData);
				this.cardItem.Set(rowItemMain, true);
				this.cardItem.gameObject.SetActive(true);
				break;
			}
			case 2:
			{
				bool isTeammate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(wager.CharId);
				this.descriptionIcon.gameObject.SetActive(false);
				this._avatarHandler = new CharacterAvatar(this.characterAvatar, false)
				{
					CharacterId = wager.CharId
				};
				this._nameHandler = new CharacterName(this.descriptionText, null, null)
				{
					CharacterId = wager.CharId
				};
				this.goTeammate.SetActive(isTeammate);
				this.goPrisoner.SetActive(!isTeammate);
				this.goWagerChar.SetActive(true);
				break;
			}
			case 3:
				this.descriptionText.text = CommonUtils.GetDisplayStringForNum(wager.Count, 100000);
				this.descriptionIcon.SetSprite("sp_icon_lilian", false, null);
				this.descriptionIcon.gameObject.SetActive(true);
				this.goWagerExp.SetActive(true);
				break;
			}
		}

		// Token: 0x04004F41 RID: 20289
		public TextMeshProUGUI descriptionText;

		// Token: 0x04004F42 RID: 20290
		public CImage descriptionIcon;

		// Token: 0x04004F43 RID: 20291
		public CRawImage resourceIcon;

		// Token: 0x04004F44 RID: 20292
		public Game.Components.Avatar.Avatar characterAvatar;

		// Token: 0x04004F45 RID: 20293
		public CardItem cardItem;

		// Token: 0x04004F46 RID: 20294
		[SerializeField]
		private GameObject goWagerResource;

		// Token: 0x04004F47 RID: 20295
		[SerializeField]
		private GameObject goWagerChar;

		// Token: 0x04004F48 RID: 20296
		[SerializeField]
		private GameObject goTeammate;

		// Token: 0x04004F49 RID: 20297
		[SerializeField]
		private GameObject goPrisoner;

		// Token: 0x04004F4A RID: 20298
		[SerializeField]
		private GameObject goWagerExp;

		// Token: 0x04004F4B RID: 20299
		[SerializeField]
		private GameObject goCardItemName;

		// Token: 0x04004F4C RID: 20300
		private CharacterAvatar _avatarHandler;

		// Token: 0x04004F4D RID: 20301
		private CharacterName _nameHandler;
	}
}
