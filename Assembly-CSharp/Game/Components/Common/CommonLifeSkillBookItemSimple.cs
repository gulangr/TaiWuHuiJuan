using System;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F8B RID: 3979
	public class CommonLifeSkillBookItemSimple : MonoBehaviour
	{
		// Token: 0x0600B718 RID: 46872 RVA: 0x005371F4 File Offset: 0x005353F4
		public void Refresh(GameData.Domains.Character.LifeSkillItem data, sbyte[] readingProgress)
		{
			Config.LifeSkillItem config = LifeSkill.Instance[data.SkillTemplateId];
			SkillBookItem skillBookConfig = SkillBook.Instance[config.SkillBookId];
			string grade = config.Grade.ToString();
			this.normalGradeIcon.SetSprite("ui9_btn_lifeskill_0_{0}".GetFormat(grade), false, null);
			this.icon.SetSprite(skillBookConfig.Icon, false, null);
			this.bookName.text = config.Name.SetGradeColor((int)config.Grade);
			this.pageStates.Refresh(data, readingProgress);
			bool flag = this.mouseTip.enabled && readingProgress != null;
			if (flag)
			{
				this.mouseTip.Type = TipType.LifeSkillDetailReadProgress;
				this.mouseTip.RuntimeParam = new ArgumentBox().SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyBookConfig, skillBookConfig).SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyReadProgresses, readingProgress);
				bool showing = this.mouseTip.Showing;
				if (showing)
				{
					this.mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x04008E30 RID: 36400
		public TooltipInvoker mouseTip;

		// Token: 0x04008E31 RID: 36401
		public CImage normalGradeIcon;

		// Token: 0x04008E32 RID: 36402
		public CImage icon;

		// Token: 0x04008E33 RID: 36403
		public TextMeshProUGUI bookName;

		// Token: 0x04008E34 RID: 36404
		public CommonPageReadStates pageStates;
	}
}
