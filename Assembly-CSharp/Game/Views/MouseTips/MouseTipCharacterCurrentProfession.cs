using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Taiwu.Profession;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200083C RID: 2108
	public class MouseTipCharacterCurrentProfession : MouseTipBase
	{
		// Token: 0x060066C9 RID: 26313 RVA: 0x002EDFBD File Offset: 0x002EC1BD
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ProfessionAllDisplayData>("ProfessionData", out this._cachedData);
			this.RefreshData();
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x002EDFDC File Offset: 0x002EC1DC
		private void RefreshData()
		{
			ProfessionData currentProfession = this.GetCurrentProfession();
			bool hasCurrentProfession = currentProfession != null;
			ProfessionAllDisplayData cachedData = this._cachedData;
			List<ProfessionData> list = (cachedData != null) ? cachedData.ProfessionDataList : null;
			bool hasExpList = list != null && list.Count > 0;
			this.professionArea.SetActive(hasCurrentProfession);
			this.professionEmptyArea.SetActive(!hasCurrentProfession);
			this.expRect.parent.gameObject.SetActive(hasExpList);
			bool flag = !hasCurrentProfession;
			if (!flag)
			{
				ProfessionItem professionConfig = currentProfession.GetConfig();
				this.txtProfessionName.text = professionConfig.Name;
				this.txtExp.text = MouseTipCharacterCurrentProfession.FormatSeniorityPercent(currentProfession);
				this.RefreshRequireAttainment(currentProfession, professionConfig);
				bool flag2 = hasExpList;
				if (flag2)
				{
					this.RefreshProfessionExpList();
				}
			}
		}

		// Token: 0x060066CB RID: 26315 RVA: 0x002EE0A0 File Offset: 0x002EC2A0
		private ProfessionData GetCurrentProfession()
		{
			ProfessionAllDisplayData cachedData = this._cachedData;
			bool flag = ((cachedData != null) ? cachedData.ProfessionDataList : null) == null;
			ProfessionData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				foreach (ProfessionData data in this._cachedData.ProfessionDataList)
				{
					bool flag2 = data != null && data.TemplateId == this._cachedData.CurrProfessionId;
					if (flag2)
					{
						return data;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060066CC RID: 26316 RVA: 0x002EE13C File Offset: 0x002EC33C
		private void RefreshRequireAttainment(ProfessionData currentProfession, ProfessionItem professionConfig)
		{
			int nextSkillIndex = currentProfession.GetUnlockedSkillCount();
			int[] thresholds = ProfessionRelatedConstants.MaxSeniorityAttainmentThresholds;
			int thresholdIndex = Math.Min(nextSkillIndex, thresholds.Length - 1);
			this.txtRequireAttainment.text = thresholds[thresholdIndex].ToString();
			this.RefreshAttainmentIcon(professionConfig);
		}

		// Token: 0x060066CD RID: 26317 RVA: 0x002EE184 File Offset: 0x002EC384
		private void RefreshAttainmentIcon(ProfessionItem professionConfig)
		{
			List<sbyte> list = professionConfig.BonusLifeSkills;
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				this.imgAttainment.SetSprite("mousetip_jiyi_" + professionConfig.BonusLifeSkills[0].ToString(), false, null);
			}
			else
			{
				list = professionConfig.BonusCombatSkills;
				bool flag2 = list != null && list.Count > 0;
				if (flag2)
				{
					this.imgAttainment.SetSprite("mousetip_gongfa_" + professionConfig.BonusCombatSkills[0].ToString(), false, null);
				}
			}
		}

		// Token: 0x060066CE RID: 26318 RVA: 0x002EE224 File Offset: 0x002EC424
		private void RefreshProfessionExpList()
		{
			Dictionary<int, ProfessionData> seniorityMap = new Dictionary<int, ProfessionData>(this._cachedData.ProfessionDataList.Count);
			foreach (ProfessionData data in this._cachedData.ProfessionDataList)
			{
				bool flag = data != null;
				if (flag)
				{
					seniorityMap[data.TemplateId] = data;
				}
			}
			int professionCount = Profession.Instance.Count;
			CommonUtils.PrepareEnoughChildren(this.expRect, this.professionTemplate.gameObject, professionCount, null);
			for (int i = 0; i < professionCount; i++)
			{
				ProfessionItem professionConfig = Profession.Instance[i];
				Refers refers = this.expRect.GetChild(i).GetComponent<Refers>();
				refers.CGet<TextMeshProUGUI>("Title").text = professionConfig.Name;
				ProfessionData professionData;
				refers.CGet<TextMeshProUGUI>("Value").text = (seniorityMap.TryGetValue(professionConfig.TemplateId, out professionData) ? MouseTipCharacterCurrentProfession.FormatSeniorityPercent(professionData) : "0%");
			}
		}

		// Token: 0x060066CF RID: 26319 RVA: 0x002EE35C File Offset: 0x002EC55C
		private static string FormatSeniorityPercent(ProfessionData professionData)
		{
			bool isMax = professionData.Seniority == 3000000;
			bool isExtra = isMax && professionData.ExtraSeniority > 0;
			bool flag = isExtra;
			string result;
			if (flag)
			{
				int current = professionData.ExtraSeniority;
				int max = 1500000;
				result = string.Format("{0}%", current * 100 / max);
			}
			else
			{
				result = string.Format("{0}%", professionData.GetSeniorityPercent());
			}
			return result;
		}

		// Token: 0x04004830 RID: 18480
		[SerializeField]
		private GameObject professionArea;

		// Token: 0x04004831 RID: 18481
		[SerializeField]
		private GameObject professionEmptyArea;

		// Token: 0x04004832 RID: 18482
		[Header("当前志向")]
		[SerializeField]
		private TextMeshProUGUI txtProfessionName;

		// Token: 0x04004833 RID: 18483
		[SerializeField]
		private TextMeshProUGUI txtExp;

		// Token: 0x04004834 RID: 18484
		[SerializeField]
		private TextMeshProUGUI txtRequireAttainment;

		// Token: 0x04004835 RID: 18485
		[SerializeField]
		private CImage imgAttainment;

		// Token: 0x04004836 RID: 18486
		[Header("志向经验")]
		[SerializeField]
		private RectTransform expRect;

		// Token: 0x04004837 RID: 18487
		[SerializeField]
		private Refers professionTemplate;

		// Token: 0x04004838 RID: 18488
		private ProfessionAllDisplayData _cachedData;
	}
}
