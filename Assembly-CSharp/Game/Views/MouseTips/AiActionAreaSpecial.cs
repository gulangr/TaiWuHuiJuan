using System;
using System.Collections.Generic;
using Config;
using GameData.ActionPlanning.MonthlyAI;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000820 RID: 2080
	public class AiActionAreaSpecial : MonoBehaviour
	{
		// Token: 0x0600663D RID: 26173 RVA: 0x002EB20C File Offset: 0x002E940C
		public void SetData(IReadOnlyList<CharacterGoalDisplayData> goals)
		{
			bool hasGoals = goals != null && goals.Count > 0;
			base.gameObject.SetActive(hasGoals);
			bool flag = !hasGoals;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.itemLayout, this.missionItemTemplate.gameObject, goals.Count, null);
				for (int i = 0; i < goals.Count; i++)
				{
					CharacterGoalDisplayData goal = goals[i];
					Refers refers = this.itemLayout.GetChild(i).GetComponent<Refers>();
					PlanningGoalItem goalTemplate = PlanningGoal.Instance[goal.GoalTemplateId];
					refers.CGet<TextMeshProUGUI>("Title").text = goalTemplate.Name;
					refers.CGet<TextMeshProUGUI>("Value").text = AiActionAreaSpecial.FormatGoalValue(goal);
					AiActionAreaSpecial.ApplyGoalParameterContent(refers, goal);
				}
			}
		}

		// Token: 0x0600663E RID: 26174 RVA: 0x002EB2F0 File Offset: 0x002E94F0
		private static void ApplyGoalParameterContent(Refers refers, CharacterGoalDisplayData goal)
		{
			TextMeshProUGUI parameterContent = refers.CGet<TextMeshProUGUI>("ParameterContent");
			bool hasParameterContent = !string.IsNullOrEmpty(goal.ParameterContent);
			parameterContent.gameObject.SetActive(hasParameterContent);
			bool flag = hasParameterContent;
			if (flag)
			{
				parameterContent.text = goal.ParameterContent;
			}
		}

		// Token: 0x0600663F RID: 26175 RVA: 0x002EB338 File Offset: 0x002E9538
		private static string FormatGoalValue(CharacterGoalDisplayData goal)
		{
			return (goal.RemainMonth >= 0) ? goal.RemainMonth.ToString() : "-";
		}

		// Token: 0x04004781 RID: 18305
		[SerializeField]
		private TextMeshProUGUI txtTitle;

		// Token: 0x04004782 RID: 18306
		[SerializeField]
		private RectTransform itemLayout;

		// Token: 0x04004783 RID: 18307
		[SerializeField]
		private Refers missionItemTemplate;
	}
}
