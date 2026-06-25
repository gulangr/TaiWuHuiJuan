using System;
using System.Collections.Generic;
using Config;
using GameData.ActionPlanning.MonthlyAI;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200081F RID: 2079
	public class AiActionAreaNormal : MonoBehaviour
	{
		// Token: 0x06006636 RID: 26166 RVA: 0x002EADD8 File Offset: 0x002E8FD8
		public void SetData(CharacterMissionDisplayData mission, sbyte behaviorType)
		{
			base.gameObject.SetActive(true);
			this.txtTitle.text = CharacterMission.Instance[mission.MissionTemplateId].Name;
			List<CharacterGoalDisplayData> goals = mission.Goals;
			int goalCount = (goals != null) ? goals.Count : 0;
			this.txtMonth.color = ((mission.EndDate > 0) ? AiActionAreaNormal.color_TextFinish : AiActionAreaNormal.color_TextNotFinish);
			this.txtMonthTitle.color = ((mission.EndDate > 0) ? AiActionAreaNormal.color_TextFinish : AiActionAreaNormal.color_TextNotFinish);
			AiActionAreaNormal.EMissionState missionState = AiActionAreaNormal.GetBubbleState(mission);
			CommonUtils.PrepareEnoughChildren(this.itemLayout, this.missionItemTemplate.gameObject, goalCount, null);
			int finishedCount = 0;
			for (int i = 0; i < goalCount; i++)
			{
				CharacterGoalDisplayData goal = goals[i];
				Refers refers = this.itemLayout.GetChild(i).GetComponent<Refers>();
				TextMeshProUGUI title = refers.CGet<TextMeshProUGUI>("Title");
				title.text = PlanningGoal.Instance[goal.GoalTemplateId].Name;
				CImage icon = refers.CGet<CImage>("Icon");
				bool finished = goal.Finished;
				if (finished)
				{
					finishedCount++;
					icon.SetSprite("ui9_back_moustip_smalltick_1", false, null);
				}
				else
				{
					icon.SetSprite("ui9_back_moustip_smalltick_0", false, null);
				}
				AiActionAreaNormal.ApplyGoalParameterContent(refers, goal);
			}
			int displayRemainMonth = AiActionAreaNormal.GetDisplayRemainMonth(mission);
			bool showRemainMonth = displayRemainMonth != int.MinValue;
			this.txtMonth.gameObject.SetActive(showRemainMonth);
			bool flag = showRemainMonth;
			if (flag)
			{
				this.txtMonth.text = displayRemainMonth.ToString();
			}
			this.ApplyBubbleState(missionState, behaviorType, CharacterMission.Instance[mission.MissionTemplateId]);
		}

		// Token: 0x06006637 RID: 26167 RVA: 0x002EAFA7 File Offset: 0x002E91A7
		private static int GetDisplayRemainMonth(CharacterMissionDisplayData mission)
		{
			return mission.RemainMonth;
		}

		// Token: 0x06006638 RID: 26168 RVA: 0x002EAFB0 File Offset: 0x002E91B0
		private static AiActionAreaNormal.EMissionState GetBubbleState(CharacterMissionDisplayData mission)
		{
			bool isSuccess = mission.IsComplete;
			bool isFail = !isSuccess && mission.IsTimeout;
			bool isInProgress = !isSuccess && !isFail;
			bool flag = isSuccess;
			AiActionAreaNormal.EMissionState result;
			if (flag)
			{
				result = AiActionAreaNormal.EMissionState.Finish;
			}
			else
			{
				bool flag2 = isInProgress;
				if (flag2)
				{
					result = AiActionAreaNormal.EMissionState.InProcess;
				}
				else
				{
					result = AiActionAreaNormal.EMissionState.NotFinish;
				}
			}
			return result;
		}

		// Token: 0x06006639 RID: 26169 RVA: 0x002EAFFC File Offset: 0x002E91FC
		private void ApplyBubbleState(AiActionAreaNormal.EMissionState state, sbyte behaviorType, CharacterMissionItem characterMissionItem)
		{
			this.bubbleObj.SetActive(false);
			bool flag = behaviorType < 0 || characterMissionItem == null;
			if (!flag)
			{
				string[] targetArr;
				if (state != AiActionAreaNormal.EMissionState.InProcess)
				{
					if (state != AiActionAreaNormal.EMissionState.Finish)
					{
						this.imgBubble.SetSprite("ui9_back_mousetip_base_npcthink_2", false, null);
						this.txtBubble.color = AiActionAreaNormal.color_BubbleTextNotFinish;
						targetArr = characterMissionItem.BubbleContentFail;
					}
					else
					{
						this.imgBubble.SetSprite("ui9_back_mousetip_base_npcthink_0", false, null);
						this.txtBubble.color = AiActionAreaNormal.color_BubbleTextFinish;
						targetArr = characterMissionItem.BubbleContentSuccess;
					}
				}
				else
				{
					this.imgBubble.SetSprite("ui9_back_mousetip_base_npcthink_1", false, null);
					this.txtBubble.color = AiActionAreaNormal.color_BubbleTextInProcess;
					targetArr = characterMissionItem.BubbleContentInProgress;
				}
				bool flag2 = targetArr == null || targetArr.Length <= (int)behaviorType;
				if (!flag2)
				{
					string content = targetArr[(int)behaviorType];
					content = content.Replace("{", string.Empty).Replace("}", string.Empty);
					bool flag3 = string.IsNullOrWhiteSpace(content);
					if (!flag3)
					{
						this.bubbleObj.SetActive(true);
						this.txtBubble.text = content;
					}
				}
			}
		}

		// Token: 0x0600663A RID: 26170 RVA: 0x002EB12C File Offset: 0x002E932C
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

		// Token: 0x0400476F RID: 18287
		[SerializeField]
		private TextMeshProUGUI txtTitle;

		// Token: 0x04004770 RID: 18288
		[SerializeField]
		private RectTransform itemLayout;

		// Token: 0x04004771 RID: 18289
		[SerializeField]
		private Refers missionItemTemplate;

		// Token: 0x04004772 RID: 18290
		[Header("时间")]
		[SerializeField]
		private TextMeshProUGUI txtMonth;

		// Token: 0x04004773 RID: 18291
		[SerializeField]
		private TextMeshProUGUI txtMonthTitle;

		// Token: 0x04004774 RID: 18292
		[Header("气泡")]
		[SerializeField]
		private GameObject bubbleObj;

		// Token: 0x04004775 RID: 18293
		[SerializeField]
		private CImage imgBubble;

		// Token: 0x04004776 RID: 18294
		[SerializeField]
		private TextMeshProUGUI txtBubble;

		// Token: 0x04004777 RID: 18295
		private static readonly Color color_BubbleTextNotFinish = new Color(0.9254902f, 0.372549f, 0.4078431f);

		// Token: 0x04004778 RID: 18296
		private static readonly Color color_BubbleTextInProcess = new Color(0.5529412f, 0.7647059f, 0.7647059f);

		// Token: 0x04004779 RID: 18297
		private static readonly Color color_BubbleTextFinish = new Color(0.8039216f, 0.6941177f, 0.2862745f);

		// Token: 0x0400477A RID: 18298
		private static readonly Color color_TextNotFinish = new Color(0.9686275f, 0.9686275f, 0.9686275f);

		// Token: 0x0400477B RID: 18299
		private static readonly Color color_TextFinish = new Color(0.8039216f, 0.6941177f, 0.2862745f);

		// Token: 0x0400477C RID: 18300
		private const string bubbleBgNotFinish = "ui9_back_mousetip_base_npcthink_2";

		// Token: 0x0400477D RID: 18301
		private const string bubbleBgInProcess = "ui9_back_mousetip_base_npcthink_1";

		// Token: 0x0400477E RID: 18302
		private const string bubbleBgFinish = "ui9_back_mousetip_base_npcthink_0";

		// Token: 0x0400477F RID: 18303
		private const string iconChecked = "ui9_back_moustip_smalltick_1";

		// Token: 0x04004780 RID: 18304
		private const string iconNotChecked = "ui9_back_moustip_smalltick_0";

		// Token: 0x02001D56 RID: 7510
		private enum EMissionState
		{
			// Token: 0x0400C5D9 RID: 50649
			NotFinish,
			// Token: 0x0400C5DA RID: 50650
			InProcess,
			// Token: 0x0400C5DB RID: 50651
			Finish
		}
	}
}
