using System;
using Config;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B66 RID: 2918
	public class CharacterMenuLifeSkillItem : MonoBehaviour
	{
		// Token: 0x0600907E RID: 36990 RVA: 0x00435B68 File Offset: 0x00433D68
		public void Set(CharacterMenuLifeSkillItemData skillData)
		{
			bool flag = skillData == null || skillData.LifeSkillItem.SkillTemplateId < 0;
			if (!flag)
			{
				LifeSkillItem config = LifeSkill.Instance[skillData.LifeSkillItem.SkillTemplateId];
				Color gradeColor = CommonUtils.GetCharacterSkillColorByValue((short)config.Grade);
				this.skillName.text = config.Name.SetColor(gradeColor);
				this.gradeLine.color = gradeColor;
				SkillBookItem skillBook = SkillBook.Instance[config.SkillBookId];
				this.icon.SetSprite(skillBook.Icon, false, null);
				this.activationStateList.Refresh(skillData.LifeSkillItem, skillData.ReadingProgress);
				this.mouseTip.Type = TipType.SkillBook;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.mouseTip.RuntimeParam.Set<ItemDisplayData>("ItemData", new ItemDisplayData(10, skillData.LifeSkillItem.SkillTemplateId));
				this.mouseTip.RuntimeParam.Set("TemplateDataOnly", true);
				this.mouseTip.RuntimeParam.Set("IsInCompareUI", false);
				this.mouseTip.RuntimeParam.Set("DisableCompare", true);
			}
		}

		// Token: 0x04006F30 RID: 28464
		[SerializeField]
		private CImage back;

		// Token: 0x04006F31 RID: 28465
		[SerializeField]
		private CImage icon;

		// Token: 0x04006F32 RID: 28466
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x04006F33 RID: 28467
		[SerializeField]
		private CommonPageReadStates activationStateList;

		// Token: 0x04006F34 RID: 28468
		[SerializeField]
		private CImage gradeLine;

		// Token: 0x04006F35 RID: 28469
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04006F36 RID: 28470
		public const int StandardWidth = 184;

		// Token: 0x04006F37 RID: 28471
		public const int StandardHeight = 196;

		// Token: 0x04006F38 RID: 28472
		private static readonly string[] ActivationStateColors = new string[]
		{
			"#3f3f3f",
			"#4f769e",
			"#bd484d"
		};

		// Token: 0x04006F39 RID: 28473
		private const string NoBonusColor = "#3a3a3a";
	}
}
