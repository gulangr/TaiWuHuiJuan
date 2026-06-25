using System;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class CricketWagerView : Refers
{
	// Token: 0x060020A8 RID: 8360 RVA: 0x000EDC64 File Offset: 0x000EBE64
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

	// Token: 0x060020A9 RID: 8361 RVA: 0x000EDCD4 File Offset: 0x000EBED4
	public void SetData(Wager wager, ItemDisplayData itemDisplayData)
	{
		base.CGet<GameObject>("WagerResource").SetActive(false);
		base.CGet<GameObject>("WagerItem").SetActive(false);
		base.CGet<GameObject>("WagerChar").SetActive(false);
		base.CGet<GameObject>("WagerExp").SetActive(false);
		switch (wager.Type)
		{
		case -1:
			this.DescriptionText.text = LocalStringManager.Get("LK_CricketWager_Invalid");
			this.DescriptionIcon.gameObject.SetActive(false);
			break;
		case 0:
		{
			ResourceTypeItem config = ResourceType.Instance[wager.WagerResourceType];
			this.DescriptionText.text = config.Name + ": " + CommonUtils.GetDisplayStringForNum(wager.Count, 100000);
			this.DescriptionIcon.SetSprite(config.Icon, false, null);
			this.DescriptionIcon.gameObject.SetActive(true);
			CommonUtils.SetResourceImage(wager.WagerResourceType, wager.Count, this.ResourceIcon, 0.6f);
			base.CGet<GameObject>("WagerResource").SetActive(true);
			break;
		}
		case 1:
			this.ItemView.SetData(itemDisplayData, false, -1, false, true, null, false, true);
			this.DescriptionText.text = CommonUtils.GetDisplayStringForNum(wager.CalcWagerValue((int)itemDisplayData.Value, 0, 0, 0, -1, 0));
			this.DescriptionIcon.SetSprite(ResourceType.Instance[6].Icon, false, null);
			this.DescriptionIcon.gameObject.SetActive(true);
			base.CGet<GameObject>("WagerItem").SetActive(true);
			break;
		case 2:
		{
			bool isTeammate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(wager.CharId);
			this.DescriptionIcon.gameObject.SetActive(false);
			this._avatarHandler = new CharacterAvatar(this.CharacterAvatar, false)
			{
				CharacterId = wager.CharId
			};
			this._nameHandler = new CharacterName(this.DescriptionText, null, null)
			{
				CharacterId = wager.CharId
			};
			base.CGet<GameObject>("Teammate").SetActive(isTeammate);
			base.CGet<GameObject>("Prisoner").SetActive(!isTeammate);
			base.CGet<GameObject>("WagerChar").SetActive(true);
			break;
		}
		case 3:
			this.DescriptionText.text = CommonUtils.GetDisplayStringForNum(wager.Count, 100000);
			this.DescriptionIcon.SetSprite("sp_icon_lilian", false, null);
			this.DescriptionIcon.gameObject.SetActive(true);
			base.CGet<GameObject>("WagerExp").SetActive(true);
			break;
		}
	}

	// Token: 0x040018F9 RID: 6393
	public TextMeshProUGUI DescriptionText;

	// Token: 0x040018FA RID: 6394
	public CImage DescriptionIcon;

	// Token: 0x040018FB RID: 6395
	public CRawImage ResourceIcon;

	// Token: 0x040018FC RID: 6396
	public Game.Components.Avatar.Avatar CharacterAvatar;

	// Token: 0x040018FD RID: 6397
	public ItemView ItemView;

	// Token: 0x040018FE RID: 6398
	private CharacterAvatar _avatarHandler;

	// Token: 0x040018FF RID: 6399
	private CharacterName _nameHandler;
}
