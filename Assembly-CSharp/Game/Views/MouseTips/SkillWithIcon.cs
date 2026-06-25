using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000848 RID: 2120
	public class SkillWithIcon : MonoBehaviour
	{
		// Token: 0x06006700 RID: 26368 RVA: 0x002EFEAC File Offset: 0x002EE0AC
		public void Set(short skill)
		{
			bool flag = skill < 0;
			if (flag)
			{
				this.text.text = LanguageKey.LK_None.Tr().SetGradeColor(0);
				this.text.margin = this.marginEmpty;
				this.icon.gameObject.SetActive(false);
			}
			else
			{
				this.icon.gameObject.SetActive(true);
				this.text.margin = this.marginWithIcon;
				CombatSkillItem cfg = CombatSkill.Instance[skill];
				this.icon.SetSprite(string.Format("ui9_icon_attainments_small_0_{0}", cfg.Type), false, null);
				this.text.text = cfg.Name.SetGradeColor((int)cfg.Grade);
			}
		}

		// Token: 0x040048A2 RID: 18594
		[SerializeField]
		private CImage icon;

		// Token: 0x040048A3 RID: 18595
		[SerializeField]
		private TMP_Text text;

		// Token: 0x040048A4 RID: 18596
		[SerializeField]
		private Vector4 marginWithIcon = new Vector4(41f, 0f, 0f, 0f);

		// Token: 0x040048A5 RID: 18597
		[SerializeField]
		private Vector4 marginEmpty = new Vector4(13f, 0f, 0f, 0f);
	}
}
