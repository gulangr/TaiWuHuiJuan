using System;
using System.Linq;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.Encyclopedia;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000850 RID: 2128
	public class MouseTipDebateAudience : MouseTipBase
	{
		// Token: 0x06006760 RID: 26464 RVA: 0x002F2130 File Offset: 0x002F0330
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			argsBox.Get<CharacterDisplayData>("CharData", out this._charData);
			bool flag = this._charData == null;
			if (flag)
			{
				int charId;
				bool flag2 = !argsBox.Get("CharId", out charId);
				if (!flag2)
				{
					this.NeedWaitData = true;
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool pool)
					{
						Serializer.Deserialize(pool, offset, ref this._charData);
						this.RefreshData();
						this.Element.ShowAfterRefresh();
					});
				}
			}
			else
			{
				this.NeedWaitData = false;
				this.RefreshData();
			}
		}

		// Token: 0x06006761 RID: 26465 RVA: 0x002F21B0 File Offset: 0x002F03B0
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Audience);
			}
		}

		// Token: 0x06006762 RID: 26466 RVA: 0x002F21E4 File Offset: 0x002F03E4
		private void RefreshData()
		{
			string charName = NameCenter.GetMonasticTitleOrDisplayName(this._charData, false);
			this.titleLabel.text = charName;
			this.nameLabel.text = charName;
			this.avatar.Refresh(this._charData, true);
			TooltipItemProperty gender = this.properties.GetChild(0).GetComponent<TooltipItemProperty>();
			string genderIcon = string.Format("{0}{1}", "ui9_icon_gender_small_", Gender.Flip(this._charData.Gender));
			string genderStr = CommonUtils.GetGenderString(CommonUtils.GetDisplayGender(this._charData.Gender, this._charData.TemplateId));
			gender.Set(genderIcon, LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), genderStr, true);
			TooltipItemProperty charm = this.properties.GetChild(1).GetComponent<TooltipItemProperty>();
			CharacterItem charConfig = Character.Instance.GetItem(this._charData.TemplateId);
			AvatarRelatedData avatarData = this._charData.AvatarRelatedData;
			bool? flag;
			if (avatarData == null)
			{
				flag = null;
			}
			else
			{
				AvatarData avatarData2 = avatarData.AvatarData;
				flag = ((avatarData2 != null) ? new bool?(avatarData2.FaceVisible) : null);
			}
			bool faceVisible = flag ?? true;
			short clothDisplayId = (avatarData != null) ? avatarData.ClothingDisplayId : 0;
			bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
			string charmIcon = CommonUtils.GetCharmLevelSmallIcon(this._charData.Charm, this._charData.ActualAge, this._charData.AvatarRelatedData.ClothingDisplayId, this._charData.AvatarRelatedData.AvatarData.FaceVisible, isFixedPresetType);
			string charmStr = CommonUtils.GetCharmLevelText(this._charData.Charm, this._charData.Gender, this._charData.PhysiologicalAge, clothDisplayId, isFixedPresetType, faceVisible);
			charm.Set(charmIcon, LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), charmStr, true);
			TooltipItemProperty happiness = this.properties.GetChild(2).GetComponent<TooltipItemProperty>();
			sbyte happinessType = HappinessType.GetHappinessType(this._charData.Happiness);
			string happinessIcon = CommonUtils.GetHappinessIconName(happinessType);
			string happinessStr = CommonUtils.GetHappinessString(happinessType);
			happiness.Set(happinessIcon, LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), happinessStr, true);
			TooltipItemProperty behavior = this.properties.GetChild(3).GetComponent<TooltipItemProperty>();
			string behaviorIcon = CommonUtils.GetBehaviorTypeIcon(this._charData.BehaviorType);
			string behaviorStr = CommonUtils.GetBehaviorString(this._charData.BehaviorType);
			behavior.Set(behaviorIcon, LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), behaviorStr, true);
			TooltipItemProperty favor = this.properties.GetChild(4).GetComponent<TooltipItemProperty>();
			string favorIcon = CommonUtils.GetFavorabilityIconName(this._charData.FavorabilityToTaiwu, true);
			string favorStr = CommonUtils.GetFavorString(this._charData.FavorabilityToTaiwu);
			favor.Set(favorIcon, LanguageKey.LK_Favorability.Tr(), favorStr, true);
			bool isEnemy = SingletonObject.getInstance<LifeSkillCombatModel>().EnemyAudienceList.Contains(this._charData);
			CharacterDisplayData taiwuCharData = SingletonObject.getInstance<LifeSkillCombatModel>().TaiwuCharData;
			CharacterDisplayData enemyCharData = SingletonObject.getInstance<LifeSkillCombatModel>().EnemyCharData;
			string targetName = isEnemy ? NameCenter.GetMonasticTitleOrDisplayName(enemyCharData, false) : NameCenter.GetMonasticTitleOrDisplayName(taiwuCharData, true);
			DebateNodeEffectItem effectConfig = DebateNodeEffect.Instance.First((DebateNodeEffectItem e) => e.BehaviorType == this._charData.BehaviorType);
			this.nodeEffectTitle.text = "<SpName=" + behaviorIcon + "> " + effectConfig.Name.SetColor(Colors.Instance.BehaviorTypeColors[(int)effectConfig.BehaviorType]);
			this.nodeEffectTitle.GetComponent<TMPTextSpriteHelper>().Parse();
			this.nodeEffectDesc1.text = effectConfig.Desc.ColorReplace();
			this.nodeEffectDesc2.text = LocalStringManager.GetFormat(string.Format("LK_LifeskillCombat_Block_Tip_TaiwuDesc_{0}", this._charData.BehaviorType), targetName).ColorReplace();
			DebateCommentItem positiveConfig = DebateComment.Instance.First((DebateCommentItem e) => e.BehaviorType == this._charData.BehaviorType && e.IsPositive);
			this.positiveEffectTitle.text = "<SpName=" + behaviorIcon + "> " + positiveConfig.Name.SetColor(Colors.Instance.BehaviorTypeColors[(int)positiveConfig.BehaviorType]);
			this.positiveEffectTitle.GetComponent<TMPTextSpriteHelper>().Parse();
			this.positiveEffectDesc1.text = positiveConfig.Desc.ColorReplace();
			this.positiveEffectDesc2.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeskillCombat_Audience_Tip_CommentPositiveContent, targetName).ColorReplace();
			DebateCommentItem negativeConfig = DebateComment.Instance.First((DebateCommentItem e) => e.BehaviorType == this._charData.BehaviorType && !e.IsPositive);
			this.negativeEffectTitle.text = "<SpName=" + behaviorIcon + "> " + negativeConfig.Name.SetColor(Colors.Instance.BehaviorTypeColors[(int)negativeConfig.BehaviorType]);
			this.negativeEffectTitle.GetComponent<TMPTextSpriteHelper>().Parse();
			this.negativeEffectDesc1.text = negativeConfig.Desc.ColorReplace();
			this.negativeEffectDesc2.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeskillCombat_Audience_Tip_CommentNegativeContent, targetName).ColorReplace();
		}

		// Token: 0x040048F9 RID: 18681
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040048FA RID: 18682
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040048FB RID: 18683
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040048FC RID: 18684
		[SerializeField]
		private Transform properties;

		// Token: 0x040048FD RID: 18685
		[SerializeField]
		private TextMeshProUGUI nodeEffectTitle;

		// Token: 0x040048FE RID: 18686
		[SerializeField]
		private TextMeshProUGUI nodeEffectDesc1;

		// Token: 0x040048FF RID: 18687
		[SerializeField]
		private TextMeshProUGUI nodeEffectDesc2;

		// Token: 0x04004900 RID: 18688
		[SerializeField]
		private TextMeshProUGUI positiveEffectTitle;

		// Token: 0x04004901 RID: 18689
		[SerializeField]
		private TextMeshProUGUI positiveEffectDesc1;

		// Token: 0x04004902 RID: 18690
		[SerializeField]
		private TextMeshProUGUI positiveEffectDesc2;

		// Token: 0x04004903 RID: 18691
		[SerializeField]
		private TextMeshProUGUI negativeEffectTitle;

		// Token: 0x04004904 RID: 18692
		[SerializeField]
		private TextMeshProUGUI negativeEffectDesc1;

		// Token: 0x04004905 RID: 18693
		[SerializeField]
		private TextMeshProUGUI negativeEffectDesc2;

		// Token: 0x04004906 RID: 18694
		private CharacterDisplayData _charData;
	}
}
