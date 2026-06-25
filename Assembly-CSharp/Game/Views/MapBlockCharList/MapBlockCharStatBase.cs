using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Config.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x0200093C RID: 2364
	public class MapBlockCharStatBase : MonoBehaviour
	{
		// Token: 0x06006E29 RID: 28201 RVA: 0x0032E528 File Offset: 0x0032C728
		private void SetStatus(List<sbyte> isLegendaryBook, int merchantId, int bountyLevel, int guardLevel, bool isSpecialNpc, bool isEvilEnemy, bool isRighteous, bool isAnimal, bool isInfected)
		{
			this.legendaryBook.SetActive(isLegendaryBook != null);
			bool flag = isLegendaryBook != null;
			if (flag)
			{
				this.legendaryBookDisplayer.PresetParam[0] = LanguageKey.LK_LegendaryBook.Tr();
				this.legendaryBookDisplayer.PresetParam[1] = LanguageKey.LK_MouseTip_LegendaryBook_Desc.TrFormat(string.Join(" ", from x in isLegendaryBook
				select LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", x))), string.Join("", from x in isLegendaryBook
				select string.Format("<SpName={0}{1}>", "ui9_back_attainments_combat_3_", x)), string.Join(" ", from x in isLegendaryBook
				select CombatSkillType.Instance[x].Name));
			}
			bool flag2 = this.merchantSprites.CheckIndex(merchantId);
			if (flag2)
			{
				this.merchant.sprite = this.merchantSprites[merchantId];
				this.merchant.gameObject.SetActive(true);
				MerchantItem cfg = Merchant.Instance[merchantId];
				this.merchantDisplayer.Type = TipType.Simple;
				this.merchantDisplayer.PresetParam[0] = LanguageKey.LK_MouseTip_Merchant_Title.Tr();
				this.merchantDisplayer.PresetParam[1] = LanguageKey.LK_MouseTip_Merchant_Desc.TrFormat(Config.MerchantType.Instance[cfg.MerchantType].Name, CommonUtils.GetMerchantLevel(cfg.Level + 1));
			}
			else
			{
				this.merchant.gameObject.SetActive(false);
			}
			bool flag3 = this.bounties.CheckIndex(bountyLevel);
			if (flag3)
			{
				this.bounty.sprite = this.bounties[bountyLevel];
				this.bounty.gameObject.SetActive(true);
			}
			else
			{
				this.bounty.gameObject.SetActive(false);
			}
			bool flag4 = this.guards.CheckIndex(guardLevel);
			if (flag4)
			{
				this.guard.sprite = this.guards[guardLevel];
				this.guard.gameObject.SetActive(true);
			}
			else
			{
				this.guard.gameObject.SetActive(false);
			}
			this.specialNpc.SetActive(isSpecialNpc);
			this.evilEnemy.SetActive(isEvilEnemy);
			this.righteous.SetActive(isRighteous);
			this.animal.SetActive(isAnimal);
			this.infected.SetActive(isInfected);
		}

		// Token: 0x06006E2A RID: 28202 RVA: 0x0032E7A8 File Offset: 0x0032C9A8
		public void SetEmpty()
		{
			this.SetNormal();
			this.SetStatus(null, -1, -1, -1, false, false, false, false, false);
		}

		// Token: 0x06006E2B RID: 28203 RVA: 0x0032E7D0 File Offset: 0x0032C9D0
		public void Set(CharacterDisplayData data, bool isSpecialNpc = false)
		{
			this.baseDisplayer.enabled = false;
			bool isApproveTaiwu = data.IsApproveTaiwu;
			if (isApproveTaiwu)
			{
				this.SetApproveTaiwu((int)data.OrgInfo.OrgTemplateId, (int)data.ApproveTaiwu);
			}
			else
			{
				this.SetNormal();
			}
			ConfigData<OrganizationMemberItem, short> instance = OrganizationMember.Instance;
			OrganizationItem organizationItem = Organization.Instance[data.OrgInfo.OrgTemplateId];
			OrganizationMemberItem memberConfig = instance[(organizationItem != null) ? organizationItem.Members[(int)data.OrgInfo.Grade] : -1];
			List<sbyte> legendaryBooks = data.LegendaryBooks;
			int merchantId = (int)((data.PhysiologicalAge >= 0 && (int)data.PhysiologicalAge < (((int)((memberConfig != null) ? new short?(memberConfig.IdentityActiveAge) : null)) ?? int.MaxValue)) ? -1 : data.MerchantTemplateId);
			int bountyPunishmentSeverity = (int)data.BountyPunishmentSeverity;
			int guardLevel = (int)(data.SettlementTreasuryGuardLevel - 1);
			bool isEvilEnemy = false;
			bool isRighteous = false;
			bool isAnimal = false;
			sbyte orgTemplateId = data.OrgInfo.OrgTemplateId;
			this.SetStatus(legendaryBooks, merchantId, bountyPunishmentSeverity, guardLevel, isSpecialNpc, isEvilEnemy, isRighteous, isAnimal, orgTemplateId == 20 || orgTemplateId == 19);
			bool flag = data.SettlementTreasuryGuardLevel > 0;
			if (flag)
			{
				string orgName = Organization.Instance[data.OrgInfo.OrgTemplateId].Name;
				this.guardDisplayer.Type = TipType.Simple;
				this.guardDisplayer.PresetParam[0] = MapBlockCharStatBase.GuardTitles.GetClampedIndexValue((int)(data.SettlementTreasuryGuardLevel - 1)).Tr();
				StringBuilder builder = new StringBuilder();
				builder.AppendLine(MapBlockCharStatBase.GuardContents.GetClampedIndexValue((int)(data.SettlementTreasuryGuardLevel - 1)).TrFormat(orgName));
				bool flag2 = !data.SettlementTreasuryGuardWorking;
				if (flag2)
				{
					builder.AppendLine(LanguageKey.LK_MapBlockCharList_SettlementTreasury_Guard_Not_Valid.Tr());
				}
				this.guardDisplayer.PresetParam[1] = builder.ToString();
			}
			bool flag3 = data.BountyPunishmentSeverity >= 0;
			if (flag3)
			{
				OrganizationItem orgConfig = Organization.Instance[data.BountyOrgTemplate];
				this.bountyDisplayer.PresetParam[0] = LanguageKey.LK_Mousetipcharacter_Bounty.Tr();
				this.bountyDisplayer.PresetParam[1] = LanguageKey.LK_Mousetipcharacter_Bounty_Tip.TrFormat(orgConfig.Name);
			}
		}

		// Token: 0x06006E2C RID: 28204 RVA: 0x0032E9F4 File Offset: 0x0032CBF4
		public void Set(GraveDisplayData data = null)
		{
			int duration = (int)((data != null) ? data.Durability : 0);
			int level = (int)((data != null) ? data.Level : -1);
			bool isInfected;
			if (data != null)
			{
				short orgSettlementId = data.OrgSettlementId;
				if (orgSettlementId - 19 <= 1)
				{
					isInfected = true;
					goto IL_33;
				}
			}
			isInfected = false;
			IL_33:
			this.SetDead(duration, level, isInfected);
		}

		// Token: 0x06006E2D RID: 28205 RVA: 0x0032EA40 File Offset: 0x0032CC40
		public void SetDead(int duration, int level = -1, bool isInfected = false)
		{
			bool flag = level < 0;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetGrave(duration, GlobalConfig.Instance.GraveDurabilities.CheckIndex(level) ? ((int)GlobalConfig.Instance.GraveDurabilities[level]) : duration);
				this.SetStatus(null, -1, -1, -1, false, false, false, false, isInfected);
			}
		}

		// Token: 0x06006E2E RID: 28206 RVA: 0x0032EA9C File Offset: 0x0032CC9C
		public void Set(MapTemplateEnemyInfo data)
		{
			this.SetEnemy((int)data.Duration);
			sbyte templateId = Character.Instance[data.TemplateId].OrganizationInfo.OrgTemplateId;
			this.SetStatus(null, -1, -1, -1, false, templateId == 17, templateId == 18, false, templateId == 19);
		}

		// Token: 0x06006E2F RID: 28207 RVA: 0x0032EAF0 File Offset: 0x0032CCF0
		public void Set(CaravanDisplayData data)
		{
			this.SetMerchant(data);
			this.SetStatus(null, -1, -1, -1, false, false, false, false, false);
		}

		// Token: 0x06006E30 RID: 28208 RVA: 0x0032EB18 File Offset: 0x0032CD18
		public void Set(GameData.Domains.Character.Animal data)
		{
			this.SetNormal();
			this.SetStatus(null, -1, -1, -1, false, false, false, true, false);
		}

		// Token: 0x06006E31 RID: 28209 RVA: 0x0032EB3D File Offset: 0x0032CD3D
		private void SetNormal()
		{
			this.baseValueTransform.gameObject.SetActive(false);
			this.gridLayoutGroup.constraintCount = this.maxConstrainCount;
			this.baseDisplayer.enabled = false;
		}

		// Token: 0x06006E32 RID: 28210 RVA: 0x0032EB74 File Offset: 0x0032CD74
		private void SetApproveTaiwu(int orgTemplateId, int value)
		{
			this.baseImage.sprite = (this.sects.CheckIndex(orgTemplateId) ? this.sects[orgTemplateId] : this.sects[0]);
			this.baseValue.text = string.Format("{0:f1}%", (double)value / 10.0);
			this.gridLayoutGroup.constraintCount = this.maxConstrainCount - 1;
			this.baseValueTransform.gameObject.SetActive(true);
			this.baseDisplayer.enabled = false;
		}

		// Token: 0x06006E33 RID: 28211 RVA: 0x0032EC08 File Offset: 0x0032CE08
		private void SetMerchant(CaravanDisplayData displayData)
		{
			MerchantItem cfg = Merchant.Instance[(int)displayData.MerchantTemplateId];
			bool flag = cfg == null;
			if (flag)
			{
				Debug.LogWarning(string.Format("invalid merchant type: {0}", displayData.MerchantTemplateId));
				this.SetNormal();
			}
			else
			{
				int level = GameData.Domains.Merchant.SharedMethods.GetFavorLevel(displayData.Favorability);
				int num;
				int limitedFavor;
				CommonUtils.IsMerchantFavorabilityReachProgressLimit(displayData.Favorability, out num, out limitedFavor);
				int curFavor = Mathf.Min(displayData.Favorability, limitedFavor);
				string favorStr = string.Format("{0}/{1}", curFavor, limitedFavor);
				this.baseImage.sprite = this.merchantSprites[(int)displayData.MerchantTemplateId];
				this.baseValue.text = favorStr;
				this.gridLayoutGroup.constraintCount = this.maxConstrainCount - 1;
				this.baseValueTransform.gameObject.SetActive(true);
				this.baseDisplayer.enabled = true;
				this.baseDisplayer.Type = TipType.Simple;
				this.baseDisplayer.PresetParam[0] = LanguageKey.LK_MouseTip_Merchant_Title.Tr();
				this.baseDisplayer.PresetParam[1] = LanguageKey.LK_MouseTip_Merchant_Desc.TrFormat(Config.MerchantType.Instance[cfg.MerchantType].Name, CommonUtils.GetMerchantLevel(cfg.Level + 1));
			}
		}

		// Token: 0x06006E34 RID: 28212 RVA: 0x0032ED54 File Offset: 0x0032CF54
		private void SetGrave(int currDuration, int maxDuration)
		{
			this.baseImage.sprite = this.graveDurationSprite;
			this.baseValue.text = string.Format("{0}/{1}", currDuration, maxDuration);
			this.gridLayoutGroup.constraintCount = this.maxConstrainCount - 1;
			this.baseValueTransform.gameObject.SetActive(true);
			this.baseDisplayer.enabled = false;
		}

		// Token: 0x06006E35 RID: 28213 RVA: 0x0032EDCC File Offset: 0x0032CFCC
		private void SetEnemy(int duration)
		{
			bool flag = duration > 0;
			if (flag)
			{
				this.baseImage.sprite = this.enemyDurationSprite;
				this.baseValue.text = string.Format("{0}", duration);
				this.gridLayoutGroup.constraintCount = this.maxConstrainCount - 1;
				this.baseValueTransform.gameObject.SetActive(true);
				this.baseDisplayer.enabled = false;
			}
			else
			{
				this.SetNormal();
			}
		}

		// Token: 0x06006E37 RID: 28215 RVA: 0x0032EE5E File Offset: 0x0032D05E
		// Note: this type is marked as 'beforefieldinit'.
		static MapBlockCharStatBase()
		{
			LanguageKey[] array = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.033A8DFB0659136B8C54E0185707A5CE9804E22B859C837D834E8314C5CDAE97).FieldHandle);
			MapBlockCharStatBase.GuardTitles = array;
			LanguageKey[] array2 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.C1195564870137F520DDC9C9CAAAA35F652E888063551A4533FCE3EC5831E986).FieldHandle);
			MapBlockCharStatBase.GuardContents = array2;
		}

		// Token: 0x040051D5 RID: 20949
		[SerializeField]
		private int maxConstrainCount = 3;

		// Token: 0x040051D6 RID: 20950
		[SerializeField]
		private GridLayoutGroup gridLayoutGroup;

		// Token: 0x040051D7 RID: 20951
		[SerializeField]
		private RectTransform baseValueTransform;

		// Token: 0x040051D8 RID: 20952
		[SerializeField]
		private CImage baseImage;

		// Token: 0x040051D9 RID: 20953
		[SerializeField]
		private TMP_Text baseValue;

		// Token: 0x040051DA RID: 20954
		[SerializeField]
		private Sprite graveDurationSprite;

		// Token: 0x040051DB RID: 20955
		[SerializeField]
		private Sprite enemyDurationSprite;

		// Token: 0x040051DC RID: 20956
		[SerializeField]
		private GameObject legendaryBook;

		// Token: 0x040051DD RID: 20957
		[SerializeField]
		private CImage merchant;

		// Token: 0x040051DE RID: 20958
		[SerializeField]
		private CImage bounty;

		// Token: 0x040051DF RID: 20959
		[SerializeField]
		private CImage guard;

		// Token: 0x040051E0 RID: 20960
		[SerializeField]
		private GameObject specialNpc;

		// Token: 0x040051E1 RID: 20961
		[SerializeField]
		private GameObject evilEnemy;

		// Token: 0x040051E2 RID: 20962
		[SerializeField]
		private GameObject righteous;

		// Token: 0x040051E3 RID: 20963
		[SerializeField]
		private GameObject animal;

		// Token: 0x040051E4 RID: 20964
		[SerializeField]
		private GameObject infected;

		// Token: 0x040051E5 RID: 20965
		[SerializeField]
		private Sprite[] bounties;

		// Token: 0x040051E6 RID: 20966
		[SerializeField]
		private Sprite[] guards;

		// Token: 0x040051E7 RID: 20967
		[SerializeField]
		private Sprite[] merchantSprites;

		// Token: 0x040051E8 RID: 20968
		[SerializeField]
		private Sprite[] sects;

		// Token: 0x040051E9 RID: 20969
		[SerializeField]
		private TooltipInvoker baseDisplayer;

		// Token: 0x040051EA RID: 20970
		[SerializeField]
		private TooltipInvoker merchantDisplayer;

		// Token: 0x040051EB RID: 20971
		[SerializeField]
		private TooltipInvoker legendaryBookDisplayer;

		// Token: 0x040051EC RID: 20972
		[SerializeField]
		private TooltipInvoker guardDisplayer;

		// Token: 0x040051ED RID: 20973
		[SerializeField]
		private TooltipInvoker bountyDisplayer;

		// Token: 0x040051EE RID: 20974
		private static readonly LanguageKey[] GuardTitles;

		// Token: 0x040051EF RID: 20975
		private static readonly LanguageKey[] GuardContents;
	}
}
