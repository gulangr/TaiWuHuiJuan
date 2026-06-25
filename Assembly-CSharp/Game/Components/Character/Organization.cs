using System;
using Config;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F3D RID: 3901
	public class Organization : MonoBehaviour
	{
		// Token: 0x0600B34A RID: 45898 RVA: 0x0051A0B0 File Offset: 0x005182B0
		public void Set(CharacterDisplayData data, bool isShowBack = true)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				string text;
				string orgName = CommonUtils.TryGetCharacterSpecialGradeName((int)data.TemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.OrgInfo);
				this.Set(data.OrgInfo, orgName, data);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B34B RID: 45899 RVA: 0x0051A114 File Offset: 0x00518314
		public void Set(OrganizationInfo orgInfo, string orgName, CharacterDisplayData data = null)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				Sprite orgIconSprite = this.GetOrganizationIconSprite((int)orgInfo.OrgTemplateId);
				this.propertyItem.Set(orgIconSprite, LanguageKey.LK_Main_SummaryInfo_Organization.Tr(), orgName, null, false);
			}
			bool flag2 = this.organizationTip != null && data != null;
			if (flag2)
			{
				this.organizationTip.Type = TipType.Organization;
				this.organizationTip.enabled = true;
				ArgumentBox args = new ArgumentBox().Set<CharacterDisplayData>("CharacterDisplayData", data);
				this.organizationTip.RuntimeParam = args;
			}
		}

		// Token: 0x0600B34C RID: 45900 RVA: 0x0051A1BC File Offset: 0x005183BC
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(null, string.Empty, string.Empty, null, false);
			}
			bool flag2 = this.organizationTip != null;
			if (flag2)
			{
				this.organizationTip.enabled = false;
			}
		}

		// Token: 0x0600B34D RID: 45901 RVA: 0x0051A218 File Offset: 0x00518418
		private Sprite GetOrganizationIconSprite(int orgTemplateId)
		{
			OrganizationItem orgCfg = Organization.Instance[orgTemplateId];
			bool flag = orgCfg == null;
			Sprite result;
			if (flag)
			{
				result = this.otherOrganizationSprite;
			}
			else
			{
				bool flag2 = orgCfg.TemplateId == 16;
				if (flag2)
				{
					result = this.taiwuOrganizationSprite;
				}
				else
				{
					bool flag3 = orgCfg.SettlementType == EOrganizationSettlementType.Village;
					if (flag3)
					{
						switch (orgCfg.Goodness)
						{
						case -1:
							return this.evilSectSprite;
						case 0:
							return this.neutralSectSprite;
						case 1:
							return this.goodSectSprite;
						}
					}
					result = this.otherOrganizationSprite;
				}
			}
			return result;
		}

		// Token: 0x04008B3F RID: 35647
		[Header("从属组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008B40 RID: 35648
		[SerializeField]
		private Sprite taiwuOrganizationSprite;

		// Token: 0x04008B41 RID: 35649
		[SerializeField]
		private Sprite goodSectSprite;

		// Token: 0x04008B42 RID: 35650
		[SerializeField]
		private Sprite neutralSectSprite;

		// Token: 0x04008B43 RID: 35651
		[SerializeField]
		private Sprite evilSectSprite;

		// Token: 0x04008B44 RID: 35652
		[SerializeField]
		private Sprite otherOrganizationSprite;

		// Token: 0x04008B45 RID: 35653
		[SerializeField]
		private TooltipInvoker organizationTip;
	}
}
