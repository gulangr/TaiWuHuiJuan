using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.ActionPlanning.MonthlyAI;
using GameData.Domains.Character;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000837 RID: 2103
	public class MouseTipAiAction : MouseTipBase
	{
		// Token: 0x060066A3 RID: 26275 RVA: 0x002ECA44 File Offset: 0x002EAC44
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("charId", out this._charId);
			CharacterDomainMethod.AsyncCall.GetActionPlanningDsiplayData(this, this._charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._cachedData);
				this.RefreshData();
			});
		}

		// Token: 0x060066A4 RID: 26276 RVA: 0x002ECA74 File Offset: 0x002EAC74
		private void RefreshData()
		{
			bool flag = this._cachedData == null;
			if (!flag)
			{
				List<CharacterGoalDisplayData> emergencyGoals = new List<CharacterGoalDisplayData>();
				List<CharacterGoalDisplayData> usualGoals = new List<CharacterGoalDisplayData>();
				List<CharacterGoalDisplayData> goals = this._cachedData.Goals;
				bool flag2 = goals != null && goals.Count > 0;
				if (flag2)
				{
					foreach (CharacterGoalDisplayData goal in this._cachedData.Goals)
					{
						PlanningGoalItem goalConfig = PlanningGoal.Instance[goal.GoalTemplateId];
						bool flag3 = goalConfig == null;
						if (!flag3)
						{
							bool isPrioritizedGoal = PlanningGoal.Instance[goal.GoalTemplateId].IsPrioritizedGoal;
							if (isPrioritizedGoal)
							{
								emergencyGoals.Add(goal);
							}
							else
							{
								usualGoals.Add(goal);
							}
						}
					}
				}
				this.emergencyArea.SetData(emergencyGoals);
				this.usualArea.SetData(usualGoals);
				List<CharacterMissionDisplayData> missions = MouseTipAiAction.CollectVisibleMissions(this._cachedData.Missions);
				this.normalLayout.gameObject.SetActive(missions.Count > 0);
				bool flag4 = missions.Count == 0;
				if (!flag4)
				{
					CommonUtils.PrepareEnoughChildren(this.normalLayout, this.normalMissionTemplate.gameObject, missions.Count, null);
					for (int i = 0; i < missions.Count; i++)
					{
						this.normalLayout.GetChild(i).GetComponent<AiActionAreaNormal>().SetData(missions[i], this._cachedData.BehaviorType);
					}
				}
			}
		}

		// Token: 0x060066A5 RID: 26277 RVA: 0x002ECC28 File Offset: 0x002EAE28
		private static List<CharacterMissionDisplayData> CollectVisibleMissions(CharacterMissionDisplayData[] missions)
		{
			List<CharacterMissionDisplayData> result = new List<CharacterMissionDisplayData>();
			bool flag = missions == null;
			List<CharacterMissionDisplayData> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				foreach (CharacterMissionDisplayData mission in missions)
				{
					bool flag2 = mission != null && mission.MissionTemplateId >= 0;
					if (flag2)
					{
						result.Add(mission);
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x040047E2 RID: 18402
		[SerializeField]
		private AiActionAreaSpecial emergencyArea;

		// Token: 0x040047E3 RID: 18403
		[SerializeField]
		private AiActionAreaSpecial usualArea;

		// Token: 0x040047E4 RID: 18404
		[SerializeField]
		private RectTransform normalLayout;

		// Token: 0x040047E5 RID: 18405
		[SerializeField]
		private AiActionAreaNormal normalMissionTemplate;

		// Token: 0x040047E6 RID: 18406
		private int _charId;

		// Token: 0x040047E7 RID: 18407
		private ActionPlanningDisplayData _cachedData;
	}
}
