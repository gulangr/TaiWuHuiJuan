using System;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009CB RID: 2507
	public class ZhujianGearMateSkillInfoItem : MonoBehaviour
	{
		// Token: 0x060079AE RID: 31150 RVA: 0x00388A98 File Offset: 0x00386C98
		public void Set(IAsyncMethodRequestHandler handler, int charId, short templateId, LifeSkillShorts lifeSkillAttainments, ushort activationState)
		{
			this.bonusEffect.RefreshBonusGroupedLayout(handler, charId, templateId, lifeSkillAttainments);
			this.SetPages(activationState);
			Array.ForEach<CImage>(this.bgs, delegate(CImage e)
			{
				e.gameObject.SetActive(true);
			});
		}

		// Token: 0x060079AF RID: 31151 RVA: 0x00388AEB File Offset: 0x00386CEB
		public void Set()
		{
			this.bonusEffect.gameObject.SetActive(false);
			Array.ForEach<CImage>(this.bgs, delegate(CImage e)
			{
				e.gameObject.SetActive(false);
			});
		}

		// Token: 0x060079B0 RID: 31152 RVA: 0x00388B2C File Offset: 0x00386D2C
		private void SetPages(ushort activationState)
		{
			for (byte i = 1; i <= 5; i += 1)
			{
				sbyte pageActiveDirection = CombatSkillStateHelper.GetPageActiveDirection(activationState, i);
				bool flag = pageActiveDirection == 0;
				if (flag)
				{
					this.bgs[(int)i].sprite = this.normalBgs[0];
					string key = LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", (int)(i - 1)));
					this.labels[(int)i].text = key.SetColor("brightblue");
				}
				else
				{
					bool flag2 = pageActiveDirection == 1;
					if (flag2)
					{
						this.bgs[(int)i].sprite = this.normalBgs[1];
						string key2 = LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", (int)(i - 1)));
						this.labels[(int)i].text = key2.SetColor("brightred");
					}
					else
					{
						this.bgs[(int)i].sprite = this.normalBgs[2];
						string key3 = LocalStringManager.Get("LK_None");
						this.labels[(int)i].text = key3.SetColor("grey");
					}
				}
			}
			sbyte outlineType = CombatSkillStateHelper.GetActiveOutlinePageType(activationState);
			this.bgs[0].sprite = this.outlineTypeBgs[(outlineType != -1) ? 0 : 1];
			string outlineKey = (outlineType != -1) ? LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", outlineType)) : LocalStringManager.Get("LK_None");
			this.labels[0].text = outlineKey.SetColor((outlineType != -1) ? "brightyellow" : "grey");
		}

		// Token: 0x04005C3B RID: 23611
		[SerializeField]
		private CImage[] bgs;

		// Token: 0x04005C3C RID: 23612
		[SerializeField]
		private TextMeshProUGUI[] labels;

		// Token: 0x04005C3D RID: 23613
		[SerializeField]
		private CombatSkillGroupedBonusEffect bonusEffect;

		// Token: 0x04005C3E RID: 23614
		[SerializeField]
		private Sprite[] normalBgs;

		// Token: 0x04005C3F RID: 23615
		[SerializeField]
		private Sprite[] outlineTypeBgs;

		// Token: 0x02001F1C RID: 7964
		private enum ChapterType
		{
			// Token: 0x0400CC5B RID: 52315
			Outline,
			// Token: 0x0400CC5C RID: 52316
			Direct,
			// Token: 0x0400CC5D RID: 52317
			Reverse
		}
	}
}
