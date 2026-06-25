using System;
using Config;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003D3 RID: 979
public class MapBlockCharRandomEnemy2 : MapBlockCharAlive2
{
	// Token: 0x06003AFD RID: 15101 RVA: 0x001DE62C File Offset: 0x001DC82C
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

	// Token: 0x06003AFE RID: 15102 RVA: 0x001DE688 File Offset: 0x001DC888
	protected override void RefreshName()
	{
		bool flag = GameData.Domains.World.SharedMethods.SmallVillageXiangshu((short)this._charConfig.OrganizationInfo.OrgTemplateId, false);
		if (flag)
		{
			this.nameLabel.text = CommonUtils.GetXiangshuMinion0AnonymousTitle();
		}
		else
		{
			string nameContent = this.nameLabel.text = this._charConfig.Surname + this._charConfig.GivenName;
			this.nameLabel.text = nameContent;
		}
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x001DE6FF File Offset: 0x001DC8FF
	protected override void RefreshOrganization()
	{
		this.organizationLabel.text = CommonUtils.GetOrganizationGradeString(this._charConfig.OrganizationInfo, -1, -1, -1);
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x001DE721 File Offset: 0x001DC921
	protected override void RefreshAvatar()
	{
		base.RefreshAvatar();
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x001DE72C File Offset: 0x001DC92C
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

	// Token: 0x04002A76 RID: 10870
	private CharacterItem _charConfig;

	// Token: 0x04002A77 RID: 10871
	private int _animalId;

	// Token: 0x04002A78 RID: 10872
	[SerializeField]
	private TMP_Text duration;

	// Token: 0x04002A79 RID: 10873
	[SerializeField]
	private GameObject durationBase;
}
