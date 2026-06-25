using System;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000878 RID: 2168
	public class MouseTipTrickType : MouseTipBase
	{
		// Token: 0x06006858 RID: 26712 RVA: 0x002FB53C File Offset: 0x002F973C
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.CombatMoves);
			}
		}

		// Token: 0x06006859 RID: 26713 RVA: 0x002FB570 File Offset: 0x002F9770
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			sbyte trickId;
			argsBox.Get("TrickType", out trickId);
			bool isAvoidTrick;
			argsBox.Get("IsAvoidTrick", out isAvoidTrick);
			TrickTypeItem trickConfig = Config.TrickType.Instance[trickId];
			this.title.text = trickConfig.Name;
			MouseTipTrickType.AvoidTrickTipHitOddsInfo hitOddsInfo;
			this.UpdateText(trickConfig, isAvoidTrick && argsBox.Get<MouseTipTrickType.AvoidTrickTipHitOddsInfo>("AvoidTrickTipHitOddsInfo", out hitOddsInfo), hitOddsInfo);
		}

		// Token: 0x0600685A RID: 26714 RVA: 0x002FB5E0 File Offset: 0x002F97E0
		private void UpdateText(TrickTypeItem trickConfig, bool hasHitOddsInfo, MouseTipTrickType.AvoidTrickTipHitOddsInfo hitOddsInfo)
		{
			this.hitMethod.text = ((!MouseTipTrickType.SpName.CheckIndex((int)trickConfig.AvoidType) || trickConfig.TemplateId == 21) ? LanguageKey.LK_Mousetip_TrickType_HitProperty.TrFormat(this.unknownSp, LanguageKey.LK_Mousetip_TrickType_HitProperty_Special.Tr()) : LanguageKey.LK_Mousetip_TrickType_HitProperty.TrFormat(MouseTipTrickType.SpName[(int)trickConfig.AvoidType].Item1, MouseTipTrickType.SpName[(int)trickConfig.AvoidType].Item2.Tr()));
			this.parseHelper.Parse();
			if (hasHitOddsInfo)
			{
				int hitValue = hitOddsInfo.SelfHit[(int)trickConfig.AvoidType];
				int avoidValue = hitOddsInfo.EnemyAvoid[(int)trickConfig.AvoidType];
				int odds = CFormula.FormulaCalcHitOdds(hitValue, avoidValue);
				this.hitProbability.text = LanguageKey.LK_Mousetip_TrickType_HitBase.TrFormat(odds);
				this.hitProbability.gameObject.SetActive(true);
			}
			else
			{
				this.hitProbability.gameObject.SetActive(false);
			}
			int sum = Math.Max(1, trickConfig.InjuryPartAtkRateDistribution.Sum((sbyte x) => (int)x));
			sbyte max = trickConfig.InjuryPartAtkRateDistribution.Max((sbyte x) => x);
			for (int i = 0; i < trickConfig.InjuryPartAtkRateDistribution.Length; i++)
			{
				sbyte val = trickConfig.InjuryPartAtkRateDistribution[i];
				this.hitDetails[i].Set((int)val, sum, i, val == max);
			}
		}

		// Token: 0x04004A04 RID: 18948
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004A05 RID: 18949
		[SerializeField]
		private TMP_Text hitMethod;

		// Token: 0x04004A06 RID: 18950
		[SerializeField]
		private TMP_Text hitProbability;

		// Token: 0x04004A07 RID: 18951
		[SerializeField]
		private TMP_Text hitDetail;

		// Token: 0x04004A08 RID: 18952
		[SerializeField]
		private TrickItem[] hitDetails;

		// Token: 0x04004A09 RID: 18953
		[SerializeField]
		private TMPTextSpriteHelper parseHelper;

		// Token: 0x04004A0A RID: 18954
		[SerializeField]
		private string unknownSp = "ui9_icon_tips_unknow_hit_type";

		// Token: 0x04004A0B RID: 18955
		private static readonly ValueTuple<string, LanguageKey>[] SpName = new ValueTuple<string, LanguageKey>[]
		{
			new ValueTuple<string, LanguageKey>(TipsRefiningEffect.RefiningIconName[0][0], LanguageKey.LK_HitType_0),
			new ValueTuple<string, LanguageKey>(TipsRefiningEffect.RefiningIconName[0][1], LanguageKey.LK_HitType_1),
			new ValueTuple<string, LanguageKey>(TipsRefiningEffect.RefiningIconName[0][2], LanguageKey.LK_HitType_2),
			new ValueTuple<string, LanguageKey>(TipsRefiningEffect.RefiningIconName[0][3], LanguageKey.LK_HitType_3)
		};

		// Token: 0x02001D90 RID: 7568
		public struct AvoidTrickTipHitOddsInfo
		{
			// Token: 0x0400C68E RID: 50830
			public HitOrAvoidInts SelfHit;

			// Token: 0x0400C68F RID: 50831
			public HitOrAvoidInts EnemyAvoid;
		}
	}
}
