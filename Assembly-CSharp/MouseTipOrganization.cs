using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character.Ai;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class MouseTipOrganization : MouseTipBase
{
	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x06002AF3 RID: 10995 RVA: 0x0014B285 File Offset: 0x00149485
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x0014B288 File Offset: 0x00149488
	protected override void Init(ArgumentBox argsBox)
	{
		RectTransform villagerNeedLayout = base.CGet<RectTransform>("VillagerNeedLayout");
		CharacterDisplayData characterDisplayData;
		argsBox.Get<CharacterDisplayData>("CharacterDisplayData", out characterDisplayData);
		bool isTaiWu = characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		bool isVillager = characterDisplayData.OrgInfo.OrgTemplateId == 16 && !isTaiWu;
		villagerNeedLayout.gameObject.SetActive(isVillager);
		this.NeedWaitData = true;
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization);
		TextMeshProUGUI textPreferSect = base.CGet<TextMeshProUGUI>("PreferSectText");
		TextMeshProUGUI endLineDescText = base.CGet<TextMeshProUGUI>("EndLineDesc");
		Refers sectScore = base.CGet<Refers>("SectScore");
		TextMeshProUGUI sectScoreTitleText = sectScore.CGet<TextMeshProUGUI>("TitleText");
		TextMeshProUGUI sectScoreDescText = sectScore.CGet<TextMeshProUGUI>("DescText");
		TextMeshProUGUI sectScoreDescTextB = sectScore.CGet<TextMeshProUGUI>("DescTextB");
		TextMeshProUGUI sectScoreDescTextC = sectScore.CGet<TextMeshProUGUI>("DescTextC");
		sectScoreDescTextB.gameObject.SetActive(isVillager);
		sectScoreDescTextC.gameObject.SetActive(isVillager);
		base.CGet<GameObject>("PreferSect").SetActive(!isTaiWu);
		bool flag = !isTaiWu;
		if (flag)
		{
			OrganizationItem orgConfig = Organization.Instance.GetItem(characterDisplayData.IdealSect);
			bool flag2 = orgConfig == null || !orgConfig.IsSect;
			if (flag2)
			{
				textPreferSect.text = LocalStringManager.Get(LanguageKey.LK_NoneJustNow).SetColor("pinkyellow");
			}
			else
			{
				textPreferSect.text = string.Format("<SpName=mousetip_menpai_{0}>", orgConfig.TemplateId);
				TextMeshProUGUI textMeshProUGUI = textPreferSect;
				TextMeshProUGUI textMeshProUGUI2 = textMeshProUGUI;
				string text = textMeshProUGUI.text;
				string name = orgConfig.Name;
				sbyte goodness = orgConfig.Goodness;
				if (!true)
				{
				}
				string color;
				if (goodness != -1)
				{
					if (goodness != 1)
					{
						color = "5987f3";
					}
					else
					{
						color = "e9c13a";
					}
				}
				else
				{
					color = "b2373b";
				}
				if (!true)
				{
				}
				textMeshProUGUI2.text = text + name.SetColor(color);
				textPreferSect.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
			}
		}
		bool flag3 = isVillager;
		if (flag3)
		{
			endLineDescText.text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization_Tip_Villager_2);
			endLineDescText.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
			sectScoreTitleText.text = LocalStringManager.Get(LanguageKey.LK_Villager) + LocalStringManager.Get(LanguageKey.LK_WorldMap_Settlement);
			sectScoreDescText.text = string.Empty;
			sectScoreDescTextB.text = string.Empty;
			sectScoreDescTextC.text = string.Empty;
			TaiwuDomainMethod.AsyncCall.GetVillagerClassesDict(this, delegate(int offset, RawDataPool pool)
			{
				this._gradeDict.Clear();
				DictIntSbyteWrapper wrapper = new DictIntSbyteWrapper();
				Serializer.Deserialize(pool, offset, ref wrapper);
				bool flag5 = ((wrapper != null) ? wrapper.Value : null) != null;
				if (flag5)
				{
					this._gradeDict.AddRangeOverride(wrapper.Value);
				}
				sbyte grade;
				bool flag6 = !this._gradeDict.TryGetValue(characterDisplayData.CharacterId, out grade);
				if (flag6)
				{
					grade = 0;
				}
				CImage gradeImage = sectScore.CGet<CImage>("GradeBack");
				TextMeshProUGUI gradeShort = sectScore.CGet<TextMeshProUGUI>("Grade");
				TextMeshProUGUI gradeText = sectScore.CGet<TextMeshProUGUI>("GradeText");
				gradeImage.SetSprite(ItemView.GetGradeIcon(grade), false, null);
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
				{
					gradeImage.gameObject.SetActive(true);
				});
				gradeShort.text = ItemView.GetGradeText(grade);
				gradeText.text = CommonUtils.GetShortGradeText((int)grade, true);
				TextMeshProUGUI sectScoreDescText2 = sectScoreDescText;
				sectScoreDescText2.text += LocalStringManager.GetFormat(LanguageKey.LK_Main_SummaryInfo_Organization_TipContent_Villager_2, characterDisplayData.InfluencePower.ToString().SetColor("pinkyellow"));
				TextMeshProUGUI sectScoreDescTextB = sectScoreDescTextB;
				sectScoreDescTextB.text += LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization_TipContent_Villager_3);
				TextMeshProUGUI sectScoreDescTextC = sectScoreDescTextC;
				sectScoreDescTextC.text += LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization_TipContent_Villager_4);
				sectScoreDescText.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
				sectScoreDescTextB.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
				sectScoreDescTextC.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
			});
			TaiwuDomainMethod.AsyncCall.GetVillagerTreasuryNeed(this, characterDisplayData.CharacterId, delegate(int offset, RawDataPool pool)
			{
				VillagerTreasuryNeed villagerTreasuryNeed = new VillagerTreasuryNeed();
				Serializer.Deserialize(pool, offset, ref villagerTreasuryNeed);
				List<GameData.Domains.Character.Ai.PersonalNeed> list = (villagerTreasuryNeed != null) ? villagerTreasuryNeed.PersonalNeeds : null;
				bool showNeed = list != null && list.Count > 0;
				villagerNeedLayout.gameObject.SetActive(showNeed);
				bool flag5 = showNeed;
				if (flag5)
				{
					GameObject template = villagerNeedLayout.GetChild(0).gameObject;
					for (int i = 0; i < villagerTreasuryNeed.PersonalNeeds.Count; i++)
					{
						GameObject go = (i < villagerNeedLayout.childCount) ? villagerNeedLayout.GetChild(i).gameObject : Object.Instantiate<GameObject>(template, villagerNeedLayout);
						go.SetActive(true);
						Refers refers = go.GetComponent<Refers>();
						GameData.Domains.Character.Ai.PersonalNeed personalNeed = villagerTreasuryNeed.PersonalNeeds[i];
						ItemDisplayData itemData = new ItemDisplayData(personalNeed.ItemType, personalNeed.ItemTemplateId);
						ItemView itemView = refers.CGet<ItemView>("ItemView");
						itemView.EnbaleTip = false;
						itemView.SetPointTriggerEnabled(false);
						itemView.SetData(itemData, false, -1, false, true, null, true, true);
						sbyte grade = ItemTemplateHelper.GetGrade(personalNeed.ItemType, personalNeed.ItemTemplateId);
						refers.CGet<TextMeshProUGUI>("Name").text = ItemTemplateHelper.GetName(personalNeed.ItemType, personalNeed.ItemTemplateId).SetGradeColor((int)grade);
						TextMeshProUGUI timeText = refers.CGet<TextMeshProUGUI>("Time");
						timeText.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_Tip_Time, personalNeed.RemainingMonths.ToString().SetColor("pinkyellow"));
						timeText.GetComponent<TMPTextSpriteHelper>().Parse();
					}
					for (int j = villagerTreasuryNeed.PersonalNeeds.Count; j < villagerNeedLayout.childCount; j++)
					{
						villagerNeedLayout.GetChild(j).gameObject.SetActive(false);
					}
				}
				else
				{
					TextMeshProUGUI sectScoreDescTextC = sectScoreDescTextC;
					sectScoreDescTextC.text += LocalStringManager.Get(LanguageKey.LK_NoneJustNow).SetColor("pinkyellow");
					sectScoreDescTextC.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
				}
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, new Action(this.Element.ShowAfterRefresh));
			});
		}
		else
		{
			endLineDescText.text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization_TipContent_NotVillager);
			endLineDescText.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
			sectScoreTitleText.text = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization_Category_1);
			sectScoreDescText.text = LocalStringManager.GetFormat(LanguageKey.LK_Treasury_Contribution, characterDisplayData.Contribution.ToString().SetColor("pinkyellow")) + "\n";
			TextMeshProUGUI sectScoreDescText3 = sectScoreDescText;
			sectScoreDescText3.text += LocalStringManager.GetFormat(LanguageKey.LK_Treasury_InfluencePower, characterDisplayData.InfluencePower.ToString().SetColor("pinkyellow"));
			sectScoreDescText.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, new Action(this.Element.ShowAfterRefresh));
		}
		YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
		bool flag4 = this._layoutCoroutine != null;
		if (flag4)
		{
			yieldHelper.StopCoroutine(this._layoutCoroutine);
		}
		yieldHelper.StartCoroutine(this._layoutCoroutine = base.RectTransform.LayoutRebuildRoutine());
	}

	// Token: 0x04001F07 RID: 7943
	private Dictionary<int, sbyte> _gradeDict = new Dictionary<int, sbyte>();

	// Token: 0x04001F08 RID: 7944
	private IEnumerator _layoutCoroutine;
}
