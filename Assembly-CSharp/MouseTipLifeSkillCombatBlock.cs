using System;
using Config;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class MouseTipLifeSkillCombatBlock : MouseTipBase
{
	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06002A79 RID: 10873 RVA: 0x001455F2 File Offset: 0x001437F2
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06002A7A RID: 10874 RVA: 0x001455F5 File Offset: 0x001437F5
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x001455FC File Offset: 0x001437FC
	protected override void Init(ArgumentBox argsBox)
	{
		DebateNodeEffectState effectState;
		argsBox.Get<DebateNodeEffectState>("EffectState", out effectState);
		DebateNodeEffectItem effectConfig = DebateNodeEffect.Instance[effectState.TemplateId];
		base.CGet<TextMeshProUGUI>("Title").text = effectConfig.Name;
		CharacterDisplayData charData = this.Model.GetAudienceData(effectState.CasterId);
		string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, false);
		base.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.GetFormat(LanguageKey.LK_LifeskillCombat_Block_Tip_Creator_Name, charName);
		string behaviorTypeIconStr = string.Format("<SpName=mousetip_lichang_{0}> ", charData.BehaviorType);
		BehaviorTypeItem behaviorConfig = BehaviorType.Instance[(short)charData.BehaviorType];
		TextMeshProUGUI behavior = base.CGet<TextMeshProUGUI>("Behavior");
		behavior.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeskillCombat_Block_Tip_Creator_Behavior, behaviorTypeIconStr + behaviorConfig.Name);
		behavior.GetComponent<TMPTextSpriteHelper>().Parse();
		string favorName = "<SpName=" + CommonUtils.GetFavorIconLegacy(charData.FavorabilityToTaiwu) + "> " + CommonUtils.GetFavorString(charData.FavorabilityToTaiwu);
		TextMeshProUGUI favor = base.CGet<TextMeshProUGUI>("Favor");
		favor.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeskillCombat_Block_Tip_Creator_Favor, favorName);
		favor.GetComponent<TMPTextSpriteHelper>().Parse();
		TextMeshProUGUI effect = base.CGet<TextMeshProUGUI>("Effect");
		effect.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeskillCombat_Block_Tip_Effect_Content, behaviorTypeIconStr + effectConfig.Name, effectConfig.Desc).ColorReplace();
		effect.GetComponent<TMPTextSpriteHelper>().Parse();
		bool isTaiwu = false;
		foreach (CharacterDisplayData data in this.Model.SelfAudienceList)
		{
			bool flag = data != null && data.CharacterId == effectState.CasterId;
			if (flag)
			{
				isTaiwu = true;
			}
		}
		TextMeshProUGUI desc = base.CGet<TextMeshProUGUI>("TaiwuDesc");
		string playerName = isTaiwu ? NameCenter.GetMonasticTitleOrDisplayName(this.Model.TaiwuCharData, true) : NameCenter.GetMonasticTitleOrDisplayName(this.Model.EnemyCharData, false);
		desc.text = LocalStringManager.GetFormat(string.Format("LK_LifeskillCombat_Block_Tip_TaiwuDesc_{0}", charData.BehaviorType), playerName).ColorReplace();
		desc.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x0014584C File Offset: 0x00143A4C
	public override void UpdateOffsetPos()
	{
		float offsetY = (this.Model.FocusingCardItem == null) ? 0f : (-ConchShipCursor.Instance.GetLifeSkillCombatUseStrategyTipHeight());
		this.RightDownOffsetPos = new Vector2(0f, offsetY);
	}
}
