using System;
using CharacterDataMonitor;
using Game.Components.Avatar;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;

// Token: 0x0200031D RID: 797
public class CharacterCard_Middle : Refers
{
	// Token: 0x1700051D RID: 1309
	// (get) Token: 0x06002EB1 RID: 11953 RVA: 0x00170839 File Offset: 0x0016EA39
	// (set) Token: 0x06002EB2 RID: 11954 RVA: 0x00170844 File Offset: 0x0016EA44
	public int CharacterId
	{
		get
		{
			return this._characterId;
		}
		set
		{
			bool flag = this._avatar == null;
			if (flag)
			{
				Refers ageRefers = base.CGet<Refers>("CharacterAgeInfo");
				this._avatar = new CharacterAvatar(base.CGet<Avatar>("Avatar"), true);
				this._age = new CharacterAge(ageRefers.CGet<TextMeshProUGUI>("BirthInfo"), ageRefers.CGet<CImage>("Icon"), null, ageRefers.CGet<TooltipInvoker>("MouseTip"), false, false, null, null);
				this._health = new CharacterHealth(base.CGet<CharacterHealthBar>("CharacterHealthInfo"));
			}
			this._characterId = value;
			this._avatar.CharacterId = this._characterId;
			this._age.CharacterId = this._characterId;
			this._health.CharacterId = this._characterId;
		}
	}

	// Token: 0x1700051E RID: 1310
	// (get) Token: 0x06002EB3 RID: 11955 RVA: 0x00170908 File Offset: 0x0016EB08
	public short DisplayAge
	{
		get
		{
			return this._age.GetMonitor<AgeHealthMonitor>().PhysiologicalAge;
		}
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x0017091C File Offset: 0x0016EB1C
	private void OnDisable()
	{
		bool flag = this._avatar != null;
		if (flag)
		{
			this._avatar.CharacterId = -1;
		}
		bool flag2 = this._age != null;
		if (flag2)
		{
			this._age.CharacterId = -1;
		}
		bool flag3 = this._health != null;
		if (flag3)
		{
			this._health.CharacterId = -1;
		}
	}

	// Token: 0x040021E2 RID: 8674
	private CharacterAvatar _avatar;

	// Token: 0x040021E3 RID: 8675
	private CharacterAge _age;

	// Token: 0x040021E4 RID: 8676
	private CharacterHealth _health;

	// Token: 0x040021E5 RID: 8677
	private int _characterId;
}
