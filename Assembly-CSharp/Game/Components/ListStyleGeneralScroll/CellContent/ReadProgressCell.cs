using System;
using Config;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EDB RID: 3803
	public class ReadProgressCell : MonoBehaviour, ICellContent<ReadProgressData>, ICellContent
	{
		// Token: 0x0600AF2D RID: 44845 RVA: 0x004FCFF8 File Offset: 0x004FB1F8
		public void SetData(ReadProgressData data)
		{
			SkillBookItem skillBookConfig = SkillBook.Instance[data.TemplateId];
			this.contentRoot.gameObject.SetActive(skillBookConfig != null);
			this.empty.gameObject.SetActive(skillBookConfig == null);
			bool flag = skillBookConfig == null;
			if (!flag)
			{
				this.skillNameText.text = skillBookConfig.Name.SetColor(Colors.Instance.GradeColors[(int)skillBookConfig.Grade]);
				bool isAllRead = ReadProgressCell.IsAllRead(data.ReadingProgress);
				this.bonusIcon.gameObject.SetActive(isAllRead);
				this.bonusText.gameObject.SetActive(isAllRead);
				this.commonPageReadStates.Refresh(data.ReadingProgress, data.PageType, SkillBook.Instance[data.TemplateId].CombatSkillType != -1, data.IsBrokenOut);
			}
		}

		// Token: 0x0600AF2E RID: 44846 RVA: 0x004FD0E4 File Offset: 0x004FB2E4
		private static bool IsAllRead(sbyte[] readingProgress)
		{
			bool flag = readingProgress == null || readingProgress.Length == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (sbyte progress in readingProgress)
				{
					bool flag2 = progress < 100;
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x040087B3 RID: 34739
		[SerializeField]
		private TextMeshProUGUI skillNameText;

		// Token: 0x040087B4 RID: 34740
		[SerializeField]
		private CommonPageReadStates commonPageReadStates;

		// Token: 0x040087B5 RID: 34741
		[SerializeField]
		private CImage bonusIcon;

		// Token: 0x040087B6 RID: 34742
		[SerializeField]
		private TextMeshProUGUI bonusText;

		// Token: 0x040087B7 RID: 34743
		[SerializeField]
		private RectTransform contentRoot;

		// Token: 0x040087B8 RID: 34744
		[SerializeField]
		private TextMeshProUGUI empty;
	}
}
