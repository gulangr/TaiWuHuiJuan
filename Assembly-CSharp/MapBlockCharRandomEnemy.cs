using System;
using Config;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003D2 RID: 978
public class MapBlockCharRandomEnemy : MapBlockCharAlive
{
	// Token: 0x06003AF6 RID: 15094 RVA: 0x001DE3B0 File Offset: 0x001DC5B0
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterItem charConfig, int animalId, sbyte enemyDuration)
	{
		base.Init(canInteract, mapBlock, null);
		this._charConfig = charConfig;
		this._animalId = animalId;
		bool flag = enemyDuration > 0;
		if (flag)
		{
			this.duration.text = enemyDuration.ToString();
		}
		this.durationBase.SetActive(enemyDuration > 0);
		this.Refresh();
	}

	// Token: 0x06003AF7 RID: 15095 RVA: 0x001DE40C File Offset: 0x001DC60C
	protected override void RefreshName()
	{
		bool flag = GameData.Domains.World.SharedMethods.SmallVillageXiangshu((short)this._charConfig.OrganizationInfo.OrgTemplateId, false);
		if (flag)
		{
			this.nameText.text = CommonUtils.GetXiangshuMinion0AnonymousTitle();
		}
		else
		{
			string nameContent = this.nameText.text = this._charConfig.Surname + this._charConfig.GivenName;
			this.nameText.text = nameContent;
		}
	}

	// Token: 0x06003AF8 RID: 15096 RVA: 0x001DE484 File Offset: 0x001DC684
	protected override void RefreshOrganization()
	{
		this.organizationText.text = CommonUtils.GetOrganizationGradeString(this._charConfig.OrganizationInfo, -1, -1, -1);
		this.organizationIcon.gameObject.SetActive(true);
		this.organizationIcon.SetSprite(CommonUtils.GetIdentityIcon(this._charConfig.OrganizationInfo.Grade), false, null);
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x001DE4E6 File Offset: 0x001DC6E6
	protected override void RefreshAvatar()
	{
		base.RefreshAvatar();
		ResLoader.LoadModOrGameResource<Texture2D>(MapBlockCharBase.NpcAvatarTexturePath + "/" + this._charConfig.FixedAvatarName, new Action<Texture2D>(this.avatar.Refresh), null);
	}

	// Token: 0x06003AFA RID: 15098 RVA: 0x001DE522 File Offset: 0x001DC722
	protected override void RefreshIcon()
	{
		this.iconImage.SetSprite("blockchar_icon_diren", false, null);
	}

	// Token: 0x06003AFB RID: 15099 RVA: 0x001DE538 File Offset: 0x001DC738
	protected override void OnClickButton()
	{
		MapBlockData mapBlock = this.MapBlock;
		short? num = (mapBlock != null) ? new short?(mapBlock.BlockId) : null;
		int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
		int currentBlockId = base.CurrentBlockId;
		bool flag = !(num2.GetValueOrDefault() == currentBlockId & num2 != null);
		if (!flag)
		{
			bool flag2 = this._animalId >= 0;
			if (flag2)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 25, this._animalId);
			}
			else
			{
				bool flag3 = this._charConfig.TemplateId >= 296 && this._charConfig.TemplateId <= 383;
				if (flag3)
				{
					GameDataBridge.AddMethodCall<short>(-1, 12, 35, this._charConfig.TemplateId);
				}
			}
			base.OnClickButton();
		}
	}

	// Token: 0x04002A72 RID: 10866
	private CharacterItem _charConfig;

	// Token: 0x04002A73 RID: 10867
	private int _animalId;

	// Token: 0x04002A74 RID: 10868
	[SerializeField]
	private TMP_Text duration;

	// Token: 0x04002A75 RID: 10869
	[SerializeField]
	private GameObject durationBase;
}
