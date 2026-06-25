using System;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.World;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F1C RID: 3868
	public class CharacterNormal : MonoBehaviour
	{
		// Token: 0x0600B237 RID: 45623 RVA: 0x00512154 File Offset: 0x00510354
		public void SetEmpty()
		{
			this.love.SetActive(false);
			this.hate.SetActive(false);
			this.avatar.ResetToBlank(false);
			this.characterName.text = string.Empty;
			this.guard.gameObject.SetActive(false);
			this.bounty.gameObject.SetActive(false);
			this.fortune.gameObject.SetActive(false);
		}

		// Token: 0x0600B238 RID: 45624 RVA: 0x005121D0 File Offset: 0x005103D0
		public void Set(CharacterDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetBasic(data);
				this.SetStatus(data);
			}
		}

		// Token: 0x0600B239 RID: 45625 RVA: 0x00512201 File Offset: 0x00510401
		public void SetBasic(AvatarRelatedData avatarData, string nameData)
		{
			this.avatar.Refresh(avatarData);
			this.characterName.text = nameData;
		}

		// Token: 0x0600B23A RID: 45626 RVA: 0x0051221E File Offset: 0x0051041E
		public void SetBasic(AvatarRelatedData avatarData, string nameData, short characterTemplateId)
		{
			this.avatar.Refresh(avatarData, characterTemplateId);
			this.characterName.text = nameData;
		}

		// Token: 0x0600B23B RID: 45627 RVA: 0x0051223C File Offset: 0x0051043C
		public void SetName(string charName)
		{
			this.characterName.text = charName;
		}

		// Token: 0x0600B23C RID: 45628 RVA: 0x0051224C File Offset: 0x0051044C
		private void SetBasic(CharacterDisplayData data)
		{
			ushort relation = data.RelationFromTaiwu;
			bool flag = relation == ushort.MaxValue;
			if (flag)
			{
				this.love.SetActive(false);
				this.hate.SetActive(false);
			}
			else
			{
				this.love.SetActive(RelationType.HasRelation(relation, 16384));
				this.hate.SetActive(RelationType.HasRelation(relation, 32768));
			}
			this.avatar.Refresh(data, true);
			bool flag2 = data.FullName.Type == 0 && SharedMethods.SmallVillageXiangshu((short)data.OrgInfo.OrgTemplateId, false);
			if (flag2)
			{
				this.characterName.text = CommonUtils.GetXiangshuMinion0AnonymousTitle();
			}
			else
			{
				this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(data, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			}
		}

		// Token: 0x0600B23D RID: 45629 RVA: 0x00512325 File Offset: 0x00510525
		private void SetStatus(CharacterDisplayData data)
		{
			this.SetStatusGuard(data);
			this.SetStatusBounty(data);
			this.SetStatusFortune(data);
		}

		// Token: 0x0600B23E RID: 45630 RVA: 0x00512340 File Offset: 0x00510540
		private void SetStatusGuard(CharacterDisplayData data)
		{
			bool show = data.IsSettlementTreasuryGuard && data.AliveState == 0;
			this.guard.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this.guard.SetSprite("ui9_icon_building_guard_" + data.SettlementTreasuryGuardLevel.ToString(), false, null);
				TooltipInvoker tip = this.guard.GetComponent<TooltipInvoker>();
				bool flag2 = tip == null || tip.PresetParam == null;
				if (flag2)
				{
					string str = "[41876] ";
					TooltipInvoker tooltipInvoker = tip;
					Debug.LogError(str + ((tooltipInvoker != null) ? tooltipInvoker.ToString() : null) + " is not valid in CharacterNormal");
				}
				else
				{
					tip.PresetParam[0] = CharacterNormal.GuardTitles.GetClampedIndexValue((int)data.SettlementTreasuryGuardLevel).Tr();
					string orgName = Organization.Instance[data.OrgInfo.OrgTemplateId].Name;
					StringBuilder builder = new StringBuilder();
					builder.AppendLine(CharacterNormal.GuardContents.GetClampedIndexValue((int)data.SettlementTreasuryGuardLevel).TrFormat(orgName));
					bool flag3 = !data.SettlementTreasuryGuardWorking;
					if (flag3)
					{
						builder.AppendLine(LanguageKey.LK_MapBlockCharList_SettlementTreasury_Guard_Not_Valid.Tr());
					}
					tip.PresetParam[1] = builder.ToString();
				}
			}
		}

		// Token: 0x0600B23F RID: 45631 RVA: 0x00512484 File Offset: 0x00510684
		private void SetStatusBounty(CharacterDisplayData data)
		{
			bool show = data.BountyPunishmentSeverity >= 0 && data.BountyOrgTemplate >= 0 && data.AliveState == 0;
			this.bounty.gameObject.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this.bounty.SetSprite("ui9_icon_bounty_" + data.BountyPunishmentSeverity.ToString(), false, null);
				TooltipInvoker tip = this.bounty.GetComponent<TooltipInvoker>();
				tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Mousetipcharacter_Bounty);
				OrganizationItem config = Organization.Instance[data.BountyOrgTemplate];
				tip.PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_Mousetipcharacter_Bounty_Tip, config.Name);
			}
		}

		// Token: 0x0600B240 RID: 45632 RVA: 0x00512538 File Offset: 0x00510738
		private void SetStatusFortune(CharacterDisplayData data)
		{
			this.fortune.gameObject.SetActive(false);
			CommonUtils.QueryJieqingSpecialInteractionUnlocked(delegate(bool unlocked)
			{
				bool flag = !unlocked || !this.isActiveAndEnabled;
				if (!flag)
				{
					int setting = SingletonObject.getInstance<GlobalSettings>().JieQingMurderSignDisplay;
					bool flag2 = !this.IgnoreFortuneDisplaySettings && !CommonUtils.CheckSectFlag(setting, (int)data.OrgInfo.OrgTemplateId);
					if (!flag2)
					{
						int levelIndex = CommonUtils.GetJieqingSignLevelIndex(data.FortuneExtraLegacyPointWorth);
						bool show = levelIndex >= 0;
						this.fortune.gameObject.SetActive(show);
						bool flag3 = !show;
						if (!flag3)
						{
							this.fortune.SetSprite("ui9_icon_sect_story_fortune_" + levelIndex.ToString(), false, null);
						}
					}
				}
			}, null);
		}

		// Token: 0x0600B242 RID: 45634 RVA: 0x00512588 File Offset: 0x00510788
		// Note: this type is marked as 'beforefieldinit'.
		static CharacterNormal()
		{
			LanguageKey[] array = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.033A8DFB0659136B8C54E0185707A5CE9804E22B859C837D834E8314C5CDAE97).FieldHandle);
			CharacterNormal.GuardTitles = array;
			LanguageKey[] array2 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.C1195564870137F520DDC9C9CAAAA35F652E888063551A4533FCE3EC5831E986).FieldHandle);
			CharacterNormal.GuardContents = array2;
		}

		// Token: 0x04008A3B RID: 35387
		private static readonly LanguageKey[] GuardTitles;

		// Token: 0x04008A3C RID: 35388
		private static readonly LanguageKey[] GuardContents;

		// Token: 0x04008A3D RID: 35389
		[SerializeField]
		protected Game.Components.Avatar.Avatar avatar;

		// Token: 0x04008A3E RID: 35390
		[SerializeField]
		protected TextMeshProUGUI characterName;

		// Token: 0x04008A3F RID: 35391
		[SerializeField]
		protected GameObject love;

		// Token: 0x04008A40 RID: 35392
		[SerializeField]
		protected GameObject hate;

		// Token: 0x04008A41 RID: 35393
		[SerializeField]
		protected CImage guard;

		// Token: 0x04008A42 RID: 35394
		[SerializeField]
		protected CImage bounty;

		// Token: 0x04008A43 RID: 35395
		[SerializeField]
		protected CImage fortune;

		// Token: 0x04008A44 RID: 35396
		[NonSerialized]
		public bool IgnoreFortuneDisplaySettings;
	}
}
