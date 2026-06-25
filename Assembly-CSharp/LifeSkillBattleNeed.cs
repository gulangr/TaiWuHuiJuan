using System;
using System.Text;
using Config;
using FrameWork;
using TMPro;

// Token: 0x02000242 RID: 578
public class LifeSkillBattleNeed : Refers
{
	// Token: 0x060025AB RID: 9643 RVA: 0x00115221 File Offset: 0x00113421
	public void SetInfo(sbyte lifeSkillType)
	{
		this.SetResourceTitle(lifeSkillType);
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x0011522C File Offset: 0x0011342C
	private void SetResourceTitle(sbyte lifeSkillType)
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		LifeSkillTypeItem config = LifeSkillType.Instance[lifeSkillType];
		stringBuilder.Clear();
		stringBuilder.Append(config.Name);
		stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Craftsman_LifeSkillBattle));
		base.CGet<TextMeshProUGUI>("content").text = stringBuilder.ToString();
		base.CGet<CImage>("icon").SetSprite(config.DisplayIcon, false, null);
		EasyPool.Free<StringBuilder>(stringBuilder);
	}
}
