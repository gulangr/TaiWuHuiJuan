using System;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using TMPro;
using UILogic;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007C9 RID: 1993
	public class ViewProfessionPreview : UIBase
	{
		// Token: 0x06006170 RID: 24944 RVA: 0x002CADB7 File Offset: 0x002C8FB7
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06006171 RID: 24945 RVA: 0x002CADBC File Offset: 0x002C8FBC
		public void Set(int professionId)
		{
			bool flag = professionId < 0;
			if (flag)
			{
				this.line.gameObject.SetActive(false);
			}
			else
			{
				this.line.gameObject.SetActive(true);
				ProfessionItem professionConfig = Profession.Instance[professionId];
				CImage professionIcon = this.line.professionIcon;
				RectTransform skillLayout = this.line.skillLayout;
				TextMeshProUGUI professionNameLabel = this.line.professionNameLabel;
				int skillCount = (professionConfig.ExtraProfessionSkill != -1) ? 4 : 3;
				professionIcon.SetSprite(string.Format("Profession_2_{0}", professionId), false, null);
				professionNameLabel.text = professionConfig.Name;
				LanguageRuleTips ruleTip;
				bool flag2 = professionNameLabel.TryGetComponent<LanguageRuleTips>(out ruleTip);
				if (flag2)
				{
					ruleTip.Refresh();
				}
				for (int i = 0; i < skillLayout.childCount; i++)
				{
					Transform skillItemTrans = skillLayout.GetChild(i);
					bool flag3 = i < skillCount;
					if (flag3)
					{
						skillItemTrans.GetComponent<UILogic.ProfessionSkillItem>().SetTemplate(professionId, i);
						skillItemTrans.gameObject.SetActive(true);
					}
					else
					{
						skillItemTrans.gameObject.SetActive(false);
					}
				}
				TooltipInvoker tips = professionIcon.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tips.RuntimeParam.Set("ProfessionId", professionId);
			}
		}

		// Token: 0x04004394 RID: 17300
		[SerializeField]
		private BottomProfessionScrollLine line;
	}
}
