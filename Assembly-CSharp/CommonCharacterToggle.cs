using System;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x02000323 RID: 803
public class CommonCharacterToggle : CToggleObsolete
{
	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x06002EEC RID: 12012 RVA: 0x00171578 File Offset: 0x0016F778
	// (set) Token: 0x06002EED RID: 12013 RVA: 0x00171580 File Offset: 0x0016F780
	public int CharacterId
	{
		get
		{
			return this._characterId;
		}
		set
		{
			this._characterId = value;
			this.Refresh();
		}
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x00171591 File Offset: 0x0016F791
	private new void OnDisable()
	{
		this.CharacterId = -1;
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x0017159C File Offset: 0x0016F79C
	public void Refresh()
	{
		bool flag = this._characterId < 0;
		if (flag)
		{
			this.NameLabel.text = string.Empty;
			this.Avatar.ResetToBlank(false);
		}
		else
		{
			this.RefreshByGetCharacterDisplayData();
		}
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x001715E2 File Offset: 0x0016F7E2
	public void RefreshByGetCharacterDisplayData()
	{
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.CharacterId, delegate(int offset, RawDataPool pool)
		{
			CharacterDisplayData characterDisplayData = null;
			Serializer.Deserialize(pool, offset, ref characterDisplayData);
			this.RefreshByCharacterDisplayData(characterDisplayData);
		});
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x001715FE File Offset: 0x0016F7FE
	public void RefreshByCharacterDisplayData(CharacterDisplayData characterDisplayData)
	{
		this.Avatar.Refresh(characterDisplayData, this.IsShowGrave);
		this.NameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, this._characterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
	}

	// Token: 0x0400220C RID: 8716
	public Avatar Avatar;

	// Token: 0x0400220D RID: 8717
	public TextMeshProUGUI NameLabel;

	// Token: 0x0400220E RID: 8718
	public bool IsShowGrave = true;

	// Token: 0x0400220F RID: 8719
	private int _characterId;
}
