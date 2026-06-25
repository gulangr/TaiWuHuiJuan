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
	// Token: 0x0200093B RID: 2363
	public class MapBlockCharStat : MonoBehaviour
	{
		// Token: 0x06006E1A RID: 28186 RVA: 0x0032DA20 File Offset: 0x0032BC20
		private void SetStatus(List<sbyte> isLegendaryBook, int merchantId, int bountyLevel, int guardLevel, bool isSpecialNpc, bool isEvilEnemy, bool isRighteous, bool isAnimal, bool isInfected, bool isInfectedMinion, bool isInfectedDemon)
		{
			int simpleCounter = 2;
			bool flag = this.isSimple;
			if (flag)
			{
				bool basicEnabled = this.baseValueTransform.gameObject.activeSelf;
				this.isSimple = false;
				this.SetEmpty();
				this.isSimple = true;
				this.baseValueTransform.gameObject.SetActive(basicEnabled);
				bool flag2 = basicEnabled;
				if (flag2)
				{
					simpleCounter--;
				}
				this.gridLayoutGroup.constraintCount = simpleCounter;
			}
			this.legendaryBook.SetActive(isLegendaryBook != null);
			bool flag3 = isLegendaryBook != null;
			if (flag3)
			{
				this.legendaryBookDisplayer.PresetParam[0] = LanguageKey.LK_LegendaryBook.Tr();
				this.legendaryBookDisplayer.PresetParam[1] = string.Join<string>('\n', from x in isLegendaryBook
				select LanguageKey.LK_MouseTip_LegendaryBook_Desc.TrFormat(LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", x)), string.Format("<SpName={0}{1}>", "ui9_back_attainments_combat_3_", x), CombatSkillType.Instance[x].Name));
				bool flag4 = this.isSimple && --simpleCounter <= 0;
				if (flag4)
				{
					return;
				}
			}
			bool flag5 = this.guards.CheckIndex(guardLevel);
			if (flag5)
			{
				this.guard.sprite = this.guards[guardLevel];
				this.guard.gameObject.SetActive(true);
				bool flag6 = this.isSimple && --simpleCounter <= 0;
				if (flag6)
				{
					return;
				}
			}
			else
			{
				this.guard.gameObject.SetActive(false);
			}
			this.xiangshuInfectedDemon.SetActive(isInfectedDemon);
			bool flag7 = isInfectedDemon && this.isSimple && --simpleCounter <= 0;
			if (!flag7)
			{
				this.infectedMinion.SetActive(isInfectedMinion);
				bool flag8 = isInfectedMinion && this.isSimple && --simpleCounter <= 0;
				if (!flag8)
				{
					this.infected.SetActive(isInfected);
					bool flag9 = isInfected && this.isSimple && --simpleCounter <= 0;
					if (!flag9)
					{
						bool flag10 = this.bounties.CheckIndex(bountyLevel);
						if (flag10)
						{
							this.bounty.sprite = this.bounties[bountyLevel];
							this.bounty.gameObject.SetActive(true);
							bool flag11 = this.isSimple && --simpleCounter <= 0;
							if (flag11)
							{
								return;
							}
						}
						else
						{
							this.bounty.gameObject.SetActive(false);
						}
						bool flag12 = this.merchantSprites.CheckIndex(merchantId);
						if (flag12)
						{
							this.merchant.sprite = this.merchantSprites[merchantId];
							this.merchant.gameObject.SetActive(true);
							MerchantItem cfg = Merchant.Instance[merchantId];
							this.merchantDisplayer.Type = TipType.Simple;
							this.merchantDisplayer.PresetParam[0] = LanguageKey.LK_MouseTip_Merchant_Title.Tr();
							this.merchantDisplayer.PresetParam[1] = LanguageKey.LK_MouseTip_Merchant_Desc.TrFormat(Config.MerchantType.Instance[cfg.MerchantType].Name, CommonUtils.GetMerchantLevel(cfg.Level + 1));
							bool flag13 = this.isSimple && --simpleCounter <= 0;
							if (flag13)
							{
								return;
							}
						}
						else
						{
							this.merchant.gameObject.SetActive(false);
						}
						this.specialNpc.SetActive(isSpecialNpc);
						bool flag14 = isSpecialNpc && this.isSimple && --simpleCounter <= 0;
						if (!flag14)
						{
							this.evilEnemy.SetActive(isEvilEnemy);
							bool flag15 = isEvilEnemy && this.isSimple && --simpleCounter <= 0;
							if (!flag15)
							{
								this.righteous.SetActive(isRighteous);
								bool flag16 = isRighteous && this.isSimple && --simpleCounter <= 0;
								if (!flag16)
								{
									this.animal.SetActive(isAnimal);
									bool flag17 = isAnimal && this.isSimple && simpleCounter - 1 <= 0;
									if (flag17)
									{
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006E1B RID: 28187 RVA: 0x0032DE34 File Offset: 0x0032C034
		public void SetEmpty()
		{
			this.SetNormal();
			this.SetStatus(null, -1, -1, -1, false, false, false, false, false, false, false);
		}

		// Token: 0x06006E1C RID: 28188 RVA: 0x0032DE5C File Offset: 0x0032C05C
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
			bool isInfected = data.OrgInfo.OrgTemplateId == 20;
			bool isInfectedMinion = data.OrgInfo.OrgTemplateId == 19;
			List<short> featureIds = data.FeatureIds;
			this.SetStatus(legendaryBooks, merchantId, bountyPunishmentSeverity, guardLevel, isSpecialNpc, isEvilEnemy, isRighteous, isAnimal, isInfected, isInfectedMinion, featureIds != null && featureIds.Contains(814));
			bool flag = data.SettlementTreasuryGuardLevel > 0;
			if (flag)
			{
				string orgName = Organization.Instance[data.OrgInfo.OrgTemplateId].Name;
				this.guardDisplayer.Type = TipType.Simple;
				this.guardDisplayer.PresetParam[0] = MapBlockCharStat.GuardTitles.GetClampedIndexValue((int)(data.SettlementTreasuryGuardLevel - 1)).Tr();
				StringBuilder builder = new StringBuilder();
				builder.AppendLine(MapBlockCharStat.GuardContents.GetClampedIndexValue((int)(data.SettlementTreasuryGuardLevel - 1)).TrFormat(orgName));
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

		// Token: 0x06006E1D RID: 28189 RVA: 0x0032E094 File Offset: 0x0032C294
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

		// Token: 0x06006E1E RID: 28190 RVA: 0x0032E0E0 File Offset: 0x0032C2E0
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
				this.SetStatus(null, -1, -1, -1, false, false, false, false, isInfected, false, false);
			}
		}

		// Token: 0x06006E1F RID: 28191 RVA: 0x0032E13C File Offset: 0x0032C33C
		public void Set(MapTemplateEnemyInfo data)
		{
			this.SetEnemy((int)data.Duration);
			sbyte templateId = Character.Instance[data.TemplateId].OrganizationInfo.OrgTemplateId;
			this.SetStatus(null, -1, -1, -1, false, templateId == 17, templateId == 18, false, false, templateId == 19, false);
		}

		// Token: 0x06006E20 RID: 28192 RVA: 0x0032E190 File Offset: 0x0032C390
		public void Set(CaravanDisplayData data)
		{
			this.SetMerchant(data);
			this.SetStatus(null, -1, -1, -1, false, false, false, false, false, false, false);
		}

		// Token: 0x06006E21 RID: 28193 RVA: 0x0032E1B8 File Offset: 0x0032C3B8
		public void Set(GameData.Domains.Character.Animal data)
		{
			this.SetNormal();
			this.SetStatus(null, -1, -1, -1, false, false, false, true, false, false, false);
		}

		// Token: 0x06006E22 RID: 28194 RVA: 0x0032E1DF File Offset: 0x0032C3DF
		private void SetNormal()
		{
			this.baseValueTransform.gameObject.SetActive(false);
			this.gridLayoutGroup.constraintCount = this.maxConstrainCount;
		}

		// Token: 0x06006E23 RID: 28195 RVA: 0x0032E208 File Offset: 0x0032C408
		private void SetApproveTaiwu(int orgTemplateId, int value)
		{
			this.baseImage.sprite = (this.sects.CheckIndex(orgTemplateId) ? this.sects[orgTemplateId] : this.sects[0]);
			this.baseValue.text = string.Format("{0:f1}%", (double)value / 10.0);
			this.gridLayoutGroup.constraintCount = this.maxConstrainCount - 1;
			this.baseValueTransform.gameObject.SetActive(true);
			this.baseDisplayer.enabled = false;
		}

		// Token: 0x06006E24 RID: 28196 RVA: 0x0032E29C File Offset: 0x0032C49C
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

		// Token: 0x06006E25 RID: 28197 RVA: 0x0032E3E8 File Offset: 0x0032C5E8
		private void SetGrave(int currDuration, int maxDuration)
		{
			this.baseImage.sprite = this.graveDurationSprite;
			this.baseValue.text = string.Format("{0}/{1}", currDuration, maxDuration);
			this.gridLayoutGroup.constraintCount = this.maxConstrainCount - 1;
			this.baseValueTransform.gameObject.SetActive(true);
			this.baseDisplayer.enabled = false;
		}

		// Token: 0x06006E26 RID: 28198 RVA: 0x0032E460 File Offset: 0x0032C660
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

		// Token: 0x06006E28 RID: 28200 RVA: 0x0032E4F9 File Offset: 0x0032C6F9
		// Note: this type is marked as 'beforefieldinit'.
		static MapBlockCharStat()
		{
			LanguageKey[] array = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.033A8DFB0659136B8C54E0185707A5CE9804E22B859C837D834E8314C5CDAE97).FieldHandle);
			MapBlockCharStat.GuardTitles = array;
			LanguageKey[] array2 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.C1195564870137F520DDC9C9CAAAA35F652E888063551A4533FCE3EC5831E986).FieldHandle);
			MapBlockCharStat.GuardContents = array2;
		}

		// Token: 0x040051B7 RID: 20919
		[SerializeField]
		private bool isSimple = true;

		// Token: 0x040051B8 RID: 20920
		[SerializeField]
		private int maxConstrainCount = 3;

		// Token: 0x040051B9 RID: 20921
		[SerializeField]
		private GridLayoutGroup gridLayoutGroup;

		// Token: 0x040051BA RID: 20922
		[SerializeField]
		private RectTransform baseValueTransform;

		// Token: 0x040051BB RID: 20923
		[SerializeField]
		private CImage baseImage;

		// Token: 0x040051BC RID: 20924
		[SerializeField]
		private TMP_Text baseValue;

		// Token: 0x040051BD RID: 20925
		[SerializeField]
		private Sprite graveDurationSprite;

		// Token: 0x040051BE RID: 20926
		[SerializeField]
		private Sprite enemyDurationSprite;

		// Token: 0x040051BF RID: 20927
		[SerializeField]
		private GameObject legendaryBook;

		// Token: 0x040051C0 RID: 20928
		[SerializeField]
		private CImage merchant;

		// Token: 0x040051C1 RID: 20929
		[SerializeField]
		private CImage bounty;

		// Token: 0x040051C2 RID: 20930
		[SerializeField]
		private CImage guard;

		// Token: 0x040051C3 RID: 20931
		[SerializeField]
		private GameObject specialNpc;

		// Token: 0x040051C4 RID: 20932
		[SerializeField]
		private GameObject evilEnemy;

		// Token: 0x040051C5 RID: 20933
		[SerializeField]
		private GameObject righteous;

		// Token: 0x040051C6 RID: 20934
		[SerializeField]
		private GameObject animal;

		// Token: 0x040051C7 RID: 20935
		[SerializeField]
		private GameObject infected;

		// Token: 0x040051C8 RID: 20936
		[SerializeField]
		private GameObject infectedMinion;

		// Token: 0x040051C9 RID: 20937
		[SerializeField]
		private GameObject xiangshuInfectedDemon;

		// Token: 0x040051CA RID: 20938
		[SerializeField]
		private Sprite[] bounties;

		// Token: 0x040051CB RID: 20939
		[SerializeField]
		private Sprite[] guards;

		// Token: 0x040051CC RID: 20940
		[SerializeField]
		private Sprite[] merchantSprites;

		// Token: 0x040051CD RID: 20941
		[SerializeField]
		private Sprite[] sects;

		// Token: 0x040051CE RID: 20942
		[SerializeField]
		private TooltipInvoker baseDisplayer;

		// Token: 0x040051CF RID: 20943
		[SerializeField]
		private TooltipInvoker merchantDisplayer;

		// Token: 0x040051D0 RID: 20944
		[SerializeField]
		private TooltipInvoker legendaryBookDisplayer;

		// Token: 0x040051D1 RID: 20945
		[SerializeField]
		private TooltipInvoker guardDisplayer;

		// Token: 0x040051D2 RID: 20946
		[SerializeField]
		private TooltipInvoker bountyDisplayer;

		// Token: 0x040051D3 RID: 20947
		private static readonly LanguageKey[] GuardTitles;

		// Token: 0x040051D4 RID: 20948
		private static readonly LanguageKey[] GuardContents;
	}
}
