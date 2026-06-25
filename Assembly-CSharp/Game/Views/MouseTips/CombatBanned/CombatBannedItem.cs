using System;
using Config;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.CombatBanned
{
	// Token: 0x020008B8 RID: 2232
	public class CombatBannedItem : MonoBehaviour
	{
		// Token: 0x06006A91 RID: 27281 RVA: 0x00313158 File Offset: 0x00311358
		public void Set(short id)
		{
			CombatSkillItem config = CombatSkill.Instance[id];
			this.itemIcon.gameObject.SetActive(false);
			this.skillFrame.gameObject.SetActive(true);
			this.skillFrame.SetSprite(CombatBannedItem.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
			this.skillIcon.SetSprite(config.Icon, false, null);
			this.nameLabel.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
			this.bannedStop.SetActive(false);
			this.bannedStopLabel.text = LanguageKey.LK_BannedRemoved.Tr();
		}

		// Token: 0x06006A92 RID: 27282 RVA: 0x00313224 File Offset: 0x00311424
		public void Set(ItemKey key)
		{
			WeaponItem config = Weapon.Instance[key.TemplateId];
			this.skillFrame.gameObject.SetActive(false);
			this.itemIcon.gameObject.SetActive(true);
			this.itemIcon.SetSprite(config.Icon, false, null);
			this.nameLabel.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
			this.bannedStop.SetActive(false);
			this.bannedStopLabel.text = LanguageKey.LK_WeaponBannedRemoved.Tr();
		}

		// Token: 0x06006A93 RID: 27283 RVA: 0x003132CA File Offset: 0x003114CA
		public void SetBannedStop(bool val)
		{
			this.bannedStop.SetActive(val);
		}

		// Token: 0x06006A94 RID: 27284 RVA: 0x003132DC File Offset: 0x003114DC
		public void SetTime(int left, int total)
		{
			bool flag = left < 0;
			if (flag)
			{
				this.timeLabel.text = LanguageKey.LK_Infinity.Tr();
				this.fill.fillAmount = 1f;
			}
			else
			{
				bool flag2 = left > 0;
				if (flag2)
				{
					this.timeLabel.text = string.Format("{0:F1}s", (float)left / 60f);
					this.fill.fillAmount = (float)left / (float)total;
				}
				else
				{
					this.timeLabel.text = "0s";
					this.fill.fillAmount = 0f;
				}
			}
		}

		// Token: 0x04004CF5 RID: 19701
		public CImage skillFrame;

		// Token: 0x04004CF6 RID: 19702
		public CImage skillIcon;

		// Token: 0x04004CF7 RID: 19703
		public CImage itemIcon;

		// Token: 0x04004CF8 RID: 19704
		public TextMeshProUGUI nameLabel;

		// Token: 0x04004CF9 RID: 19705
		public TextMeshProUGUI timeLabel;

		// Token: 0x04004CFA RID: 19706
		public CImage fill;

		// Token: 0x04004CFB RID: 19707
		public GameObject bannedStop;

		// Token: 0x04004CFC RID: 19708
		public TextMeshProUGUI bannedStopLabel;

		// Token: 0x04004CFD RID: 19709
		public CanvasGroup bannedStopCanvasGroup;

		// Token: 0x04004CFE RID: 19710
		public static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};
	}
}
