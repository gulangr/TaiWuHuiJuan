using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using Game.Views.MouseTips.LifeSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000851 RID: 2129
	public class MouseTipDebateCardType : MouseTipBase
	{
		// Token: 0x06006768 RID: 26472 RVA: 0x002F2760 File Offset: 0x002F0960
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			sbyte lifeSkillType;
			argsBox.Get("LifeSkillType", out lifeSkillType);
			int bit;
			argsBox.Get("UnlockedState", out bit);
			this.nameLabel.text = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_SelectCardType_Tip_Title);
			this.descLabel.text = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_SelectCardType_Tip_Content).ColorReplace();
			this.descLabel.GetComponent<TMPTextSpriteHelper>().Parse();
			for (int i = 0; i < this.strategies.childCount; i++)
			{
				this.strategies.GetChild(i).GetComponent<StrategyItem>().Set(lifeSkillType, i + 1, this.IsUnlocked(bit, i));
			}
		}

		// Token: 0x06006769 RID: 26473 RVA: 0x002F2818 File Offset: 0x002F0A18
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.DebateStrategy);
			}
		}

		// Token: 0x0600676A RID: 26474 RVA: 0x002F284A File Offset: 0x002F0A4A
		private bool IsUnlocked(int bit, int index)
		{
			return (bit & 1 << index) != 0;
		}

		// Token: 0x04004907 RID: 18695
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04004908 RID: 18696
		[SerializeField]
		private TextMeshProUGUI descLabel;

		// Token: 0x04004909 RID: 18697
		[SerializeField]
		private Transform strategies;
	}
}
