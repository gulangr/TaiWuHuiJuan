using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Views.CombatSkillTree;
using GameData.Domains.CombatSkill;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006F9 RID: 1785
	public class ViewCombatSkillTree : UIBase
	{
		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x060054B5 RID: 21685 RVA: 0x0027408F File Offset: 0x0027228F
		private bool IsInGame
		{
			get
			{
				return GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			}
		}

		// Token: 0x060054B6 RID: 21686 RVA: 0x0027409E File Offset: 0x0027229E
		private void Awake()
		{
			this.skillLineInfinityScroll.OnItemRender += this.OnSkillLineRender;
		}

		// Token: 0x060054B7 RID: 21687 RVA: 0x002740BC File Offset: 0x002722BC
		private void OnSkillLineRender(int index, GameObject skillLineObj)
		{
			CombatSkillTreeSkillLine skillLine = skillLineObj.GetComponent<CombatSkillTreeSkillLine>();
			List<CombatSkillItem> combatSkillItemList = this._sectSkillList[index];
			CombatSkillTypeItem config = Config.CombatSkillType.Instance[combatSkillItemList[0].Type];
			CombatSkillTreeSkillLine combatSkillTreeSkillLine = skillLine;
			sbyte visibleLevel = this._visibleLevel;
			sbyte type = combatSkillItemList[0].Type;
			CombatSkillTypeItem config2 = config;
			List<CombatSkillItem> combatSkillItemList2 = combatSkillItemList;
			OrganizationCombatSkillsDisplayData data = this._data;
			combatSkillTreeSkillLine.Set(visibleLevel, type, config2, combatSkillItemList2, (data != null) ? data.LearnedSkills : null);
		}

		// Token: 0x060054B8 RID: 21688 RVA: 0x00274122 File Offset: 0x00272322
		private void OnDisable()
		{
			Action callBack = this._callBack;
			if (callBack != null)
			{
				callBack();
			}
		}

		// Token: 0x060054B9 RID: 21689 RVA: 0x00274138 File Offset: 0x00272338
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<Action>("CallBack", out this._callBack);
			this._sectTemplateId = -1;
			bool flag = !argsBox.Get("SectTemplateId", out this._sectTemplateId);
			if (!flag)
			{
				this._taiwuFiveElementsType = -1;
				argsBox.Get("TaiwuFiveElementsType", out this._taiwuFiveElementsType);
				this.taiwuFiveElements.transform.parent.gameObject.SetActive(!this.IsInGame);
				this._visibleLevel = 0;
				this.UpdateSectInfo();
				this.NeedDataListenerId = (GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame);
				UIElement element = this.Element;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
				{
					bool needDataListenerId = this.NeedDataListenerId;
					if (needDataListenerId)
					{
						OrganizationDomainMethod.Call.GetOrganizationCombatSkillsDisplayData(this.Element.GameDataListenerId, this._sectTemplateId);
					}
					else
					{
						this.UpdateApprovingRateProgress();
						this.InitSkillData();
						this.skillLineInfinityScroll.SetDataCount(this._sectSkillList.Count);
						this.Element.ShowAfterRefresh();
					}
				}));
			}
		}

		// Token: 0x060054BA RID: 21690 RVA: 0x00274204 File Offset: 0x00272404
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				base.QuickHide();
			}
		}

		// Token: 0x060054BB RID: 21691 RVA: 0x00274234 File Offset: 0x00272434
		private void UpdateSectInfo()
		{
			OrganizationItem orgCfg = Organization.Instance[this._sectTemplateId];
			this.sectName.text = orgCfg.Name;
			this.sectDesc.text = orgCfg.Desc.ColorReplace();
			this.sectBehaviour.text = CommonUtils.GetOrganizationExtraDescString(orgCfg.TemplateId).ColorReplace();
			this.sectWarning.gameObject.SetActive(orgCfg.Members != null && orgCfg.Members.Length != 0 && OrganizationMember.Instance[orgCfg.Members[0]].ChildGrade < 0);
			this.sectIcon.SetSprite("ui9_icon_sect_icon_" + orgCfg.TemplateId.ToString(), false, null);
			this.sectRecommendFiveElements.SetSprite("ui9_icon_elements_big_" + orgCfg.FiveElementsType.ToString(), false, null);
			this.taiwuFiveElements.SetSprite("ui9_icon_elements_big_" + this._taiwuFiveElementsType.ToString(), false, null);
			this.sectRestrainFiveElements[0].SetSprite("ui9_icon_elements_big_" + FiveElementsType.Countered[(int)orgCfg.FiveElementsType].ToString(), false, null);
			this.sectRestrainFiveElements[1].SetSprite("ui9_icon_elements_big_" + FiveElementsType.Countering[(int)orgCfg.FiveElementsType].ToString(), false, null);
			this.sectBlock.texture = this.sectTexs[(int)(this._sectTemplateId - 1)];
		}

		// Token: 0x060054BC RID: 21692 RVA: 0x002743BC File Offset: 0x002725BC
		private void UpdateApprovingRateProgress()
		{
			OrganizationCombatSkillsDisplayData data = this._data;
			short? num = (data != null) ? new short?(data.ApprovingRate) : null;
			float approvePercent = ((num != null) ? new float?((float)num.GetValueOrDefault() / 1000f) : null).GetValueOrDefault();
			OrganizationCombatSkillsDisplayData data2 = this._data;
			num = ((data2 != null) ? new short?(data2.ApprovingRateTotal) : null);
			float approvePercentTotal = ((num != null) ? new float?((float)num.GetValueOrDefault() / 1000f) : null).GetValueOrDefault();
			OrganizationCombatSkillsDisplayData data3 = this._data;
			num = ((data3 != null) ? new short?(data3.ApprovingRateUpperLimit) : null);
			float? num2 = (num != null) ? new float?((float)num.GetValueOrDefault() / 1000f) : null;
			double num3 = (num2 != null) ? ((double)num2.GetValueOrDefault()) : 0.3;
			OrganizationCombatSkillsDisplayData data4 = this._data;
			int num4 = (int)((data4 != null) ? data4.ApprovingRateUpperLimit : 300);
			OrganizationCombatSkillsDisplayData data5 = this._data;
			int limit = num4 + (int)((data5 != null) ? data5.ApprovingRateUpperLimitBonus : 0);
			float approveLimitPercent = (float)limit / 1000f;
			float percent = (approvePercentTotal >= 0.9f) ? 1f : ((approvePercentTotal <= 0.2f) ? (approvePercentTotal * 0.625f) : (1.25f * approvePercentTotal - 0.125f));
			this.approvingRateProgress.fillAmount = percent;
			this.currentApprovingRate.text = LanguageKey.LK_CombatSkillTree_CurrentApprove.TrFormat(approvePercentTotal * 100f);
			this.approvingRateLimit.text = LanguageKey.LK_CombatSkillTree_CurrentApproveLimit.TrFormat(approveLimitPercent * 100f);
			int limitPointCount = this.CalculatePointCount((double)(approveLimitPercent * 100f));
			int approvePointCount = this.CalculatePointCount((double)(approvePercentTotal * 100f));
			for (int i = 0; i < this.approvingRatePoints.Length; i++)
			{
				this.approvingRatePoints[i].gameObject.SetActive(i <= Math.Min(limitPointCount, approvePointCount));
			}
			this.limitLine.gameObject.SetActive(this.IsInGame && (double)approveLimitPercent <= 0.9);
			this.limitArea.gameObject.SetActive(this.IsInGame && (double)approveLimitPercent <= 0.9);
			this.limitLine.position = new Vector3(this.approvingRatePoints[limitPointCount].rectTransform.position.x, this.approvingRatePoints[limitPointCount].rectTransform.position.y, this.approvingRatePoints[limitPointCount].rectTransform.position.z);
			this.limitArea.offsetMin = new Vector2(this.limitLine.anchoredPosition.x, this.limitArea.offsetMin.y);
			TooltipInvoker mouseTip = this.approvingRateTip;
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			mouseTip.Type = TipType.Simple;
			mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_OrganizationApprove));
			string approveTipsContent = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillTree_OrganizationApprove_TipContent, approveLimitPercent);
			OrganizationCombatSkillsDisplayData data6 = this._data;
			num = ((data6 != null) ? new short?(data6.ApprovingRateUpperLimitBonus) : null);
			int? num5 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			int num6 = 0;
			bool flag = num5.GetValueOrDefault() > num6 & num5 != null;
			if (flag)
			{
				approveTipsContent += LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_OrganizationApprove_TipContent_Extra);
			}
			mouseTip.RuntimeParam.Set("arg1", approveTipsContent);
			this.UpdateApproveEffectTips((int)Math.Round((double)((this._data != null) ? (approvePercent * 100f) : -1f)));
		}

		// Token: 0x060054BD RID: 21693 RVA: 0x002747E4 File Offset: 0x002729E4
		private int CalculatePointCount(double percent)
		{
			int p = Mathf.CeilToInt((float)percent);
			return Math.Min((p <= 20) ? (p / 20) : (p / 10 - 1), this.approvingRatePoints.Length - 1);
		}

		// Token: 0x060054BE RID: 21694 RVA: 0x00274820 File Offset: 0x00272A20
		private float CalculateProgressPosition(float value)
		{
			bool flag = value <= 0.2f;
			float result;
			if (flag)
			{
				result = value * 0.625f;
			}
			else
			{
				result = (value - 1f) * 0.125f;
			}
			return result;
		}

		// Token: 0x060054BF RID: 21695 RVA: 0x0027485C File Offset: 0x00272A5C
		private void UpdateApproveEffectTips(int currApprove = -1)
		{
			int xiangshuMaxGrade = (int)((currApprove >= 0) ? (SharedMethods.GetMaxGradeOfXiangshuInfection(SingletonObject.getInstance<BasicGameData>().XiangshuProgress) - 2) : -1);
			for (int i = 0; i < 10; i++)
			{
				bool flag = i == 1;
				if (!flag)
				{
					string tipsContent = (i == 0) ? SectApprovingEffect.Instance[(int)(this._sectTemplateId - 1)].Desc : this.ApproveEffectDesc[i - 2];
					bool flag2 = currApprove >= 0;
					if (flag2)
					{
						int requireApprove = i * 10;
						bool flag3 = currApprove < requireApprove;
						if (flag3)
						{
							tipsContent = tipsContent + "\n\n" + LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillTree_Require_Approve_Tips, requireApprove);
							tipsContent = tipsContent + "\n\n" + LocalStringManager.GetFormat(LanguageKey.LK_CombatSkillTree_Curr_Approve_Tips, (xiangshuMaxGrade + 4) * 10);
						}
					}
					bool flag4 = i == 0;
					if (flag4)
					{
						this.approvingRateEffectTips[i].GetComponent<CImage>().SetSprite(string.Format("{0}{1}", "ui9_icon_combat_skill_tree_approving_icon_0_0_", (int)(this._sectTemplateId - 1)), false, null);
						this.approvingRateEffectTips[i].PresetParam[0] = SectApprovingEffect.Instance[(int)(this._sectTemplateId - 1)].Name;
					}
					this.approvingRateEffectTips[(i == 0) ? i : (i - 1)].PresetParam[1] = tipsContent;
				}
			}
		}

		// Token: 0x060054C0 RID: 21696 RVA: 0x002749AD File Offset: 0x00272BAD
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
		}

		// Token: 0x060054C1 RID: 21697 RVA: 0x002749CC File Offset: 0x00272BCC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 3;
					if (flag)
					{
						bool flag2 = notification.MethodId == 3;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._data);
							this.UpdateApprovingRateProgress();
							this.InitSkillData();
							this.skillLineInfinityScroll.SetDataCount(this._sectSkillList.Count);
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x060054C2 RID: 21698 RVA: 0x00274AA4 File Offset: 0x00272CA4
		private void InitSkillData()
		{
			Dictionary<sbyte, List<CombatSkillItem>> skillTypeMap = new Dictionary<sbyte, List<CombatSkillItem>>();
			CombatSkill.Instance.Iterate(delegate(CombatSkillItem item)
			{
				bool flag = item.SectId == this._sectTemplateId;
				if (flag)
				{
					List<CombatSkillItem> list;
					bool flag2 = !skillTypeMap.TryGetValue(item.Type, out list);
					if (flag2)
					{
						list = new List<CombatSkillItem>();
						skillTypeMap.Add(item.Type, list);
					}
					bool flag3 = !list.Contains(item);
					if (flag3)
					{
						list.Add(item);
					}
				}
				return true;
			});
			this._sectSkillList = new List<List<CombatSkillItem>>(skillTypeMap.Values);
			this._sectSkillList.ForEach(delegate(List<CombatSkillItem> e)
			{
				e.Sort((CombatSkillItem left, CombatSkillItem right) => (int)(left.Grade - right.Grade));
			});
			this._sectSkillList.Sort((List<CombatSkillItem> left, List<CombatSkillItem> right) => (int)(left[0].Type - right[0].Type));
		}

		// Token: 0x0400395C RID: 14684
		[SerializeField]
		private CImage approvingRateProgress;

		// Token: 0x0400395D RID: 14685
		[SerializeField]
		private TextMeshProUGUI currentApprovingRate;

		// Token: 0x0400395E RID: 14686
		[SerializeField]
		private TextMeshProUGUI approvingRateLimit;

		// Token: 0x0400395F RID: 14687
		[SerializeField]
		private CImage[] approvingRatePoints;

		// Token: 0x04003960 RID: 14688
		[SerializeField]
		private RectTransform limitLine;

		// Token: 0x04003961 RID: 14689
		[SerializeField]
		private RectTransform limitArea;

		// Token: 0x04003962 RID: 14690
		[SerializeField]
		private TooltipInvoker[] approvingRateEffectTips;

		// Token: 0x04003963 RID: 14691
		[SerializeField]
		private TooltipInvoker approvingRateTip;

		// Token: 0x04003964 RID: 14692
		[SerializeField]
		private InfinityScroll skillLineInfinityScroll;

		// Token: 0x04003965 RID: 14693
		[SerializeField]
		private CImage sectIcon;

		// Token: 0x04003966 RID: 14694
		[SerializeField]
		private TextMeshProUGUI sectName;

		// Token: 0x04003967 RID: 14695
		[SerializeField]
		private TextMeshProUGUI sectDesc;

		// Token: 0x04003968 RID: 14696
		[SerializeField]
		private CRawImage sectBlock;

		// Token: 0x04003969 RID: 14697
		[SerializeField]
		private Texture2D[] sectTexs;

		// Token: 0x0400396A RID: 14698
		[SerializeField]
		private RectTransform sectWarning;

		// Token: 0x0400396B RID: 14699
		[SerializeField]
		private TextMeshProUGUI sectBehaviour;

		// Token: 0x0400396C RID: 14700
		[SerializeField]
		private CImage sectRecommendFiveElements;

		// Token: 0x0400396D RID: 14701
		[SerializeField]
		private CImage[] sectRestrainFiveElements;

		// Token: 0x0400396E RID: 14702
		[SerializeField]
		private CImage taiwuFiveElements;

		// Token: 0x0400396F RID: 14703
		private readonly string[] ApproveEffectDesc = new string[]
		{
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_FameChange_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_GetAuthority_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_ReadBookEfficientBonus_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_SkillBreakSuccessBonus_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_FavorChange_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_SkillBreakStepBonus_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_SkillBreakCostDebtBonus_TipContent),
			LocalStringManager.Get(LanguageKey.LK_CombatSkillTree_RemakePledge_TipContent)
		};

		// Token: 0x04003970 RID: 14704
		private sbyte _sectTemplateId;

		// Token: 0x04003971 RID: 14705
		private OrganizationCombatSkillsDisplayData _data;

		// Token: 0x04003972 RID: 14706
		private List<List<CombatSkillItem>> _sectSkillList;

		// Token: 0x04003973 RID: 14707
		private sbyte _visibleLevel;

		// Token: 0x04003974 RID: 14708
		private Action _callBack;

		// Token: 0x04003975 RID: 14709
		private sbyte _taiwuFiveElementsType;
	}
}
