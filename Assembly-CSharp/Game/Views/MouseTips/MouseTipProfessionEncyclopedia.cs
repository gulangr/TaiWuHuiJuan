using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x02000873 RID: 2163
	public class MouseTipProfessionEncyclopedia : MouseTipBase
	{
		// Token: 0x17000C7B RID: 3195
		// (get) Token: 0x0600682E RID: 26670 RVA: 0x002F9FBF File Offset: 0x002F81BF
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600682F RID: 26671 RVA: 0x002F9FC2 File Offset: 0x002F81C2
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x06006830 RID: 26672 RVA: 0x002F9FCD File Offset: 0x002F81CD
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get("ProfessionId", out this._professionId);
		}

		// Token: 0x06006831 RID: 26673 RVA: 0x002F9FE2 File Offset: 0x002F81E2
		public override void Refresh(ArgumentBox argsBox)
		{
			this.ReadArgs(argsBox);
			this.InitRefers();
			this.RefreshInner();
			this.OnGetProfessionTipDisplayData();
			this.RefreshAttainmentItems();
		}

		// Token: 0x06006832 RID: 26674 RVA: 0x002FA00C File Offset: 0x002F820C
		private void RefreshInner()
		{
			int id;
			bool flag = int.TryParse(this._professionId, out id);
			if (flag)
			{
				this._config = Profession.Instance[id];
			}
			else
			{
				this._config = Profession.Instance[this._professionId];
			}
			this.titleLabel.text = this._config.Name;
			this.descLabel.text = this._config.Desc;
			this.RefreshSeniorityGains();
		}

		// Token: 0x06006833 RID: 26675 RVA: 0x002FA089 File Offset: 0x002F8289
		private void OnGetProfessionTipDisplayData()
		{
			this.RefreshClothingArea();
		}

		// Token: 0x06006834 RID: 26676 RVA: 0x002FA094 File Offset: 0x002F8294
		private void RefreshAttainmentItems()
		{
			string sepChar = LocalStringManager.Get(LanguageKey.LK_Separator);
			List<sbyte> types = this._config.BonusLifeSkills;
			bool isCombatSkill = false;
			bool flag = types.Count == 0;
			if (flag)
			{
				types = this._config.BonusCombatSkills;
				isCombatSkill = true;
			}
			int rowAmount = types.Count / 5 + 1;
			this.attainmentArea.gameObject.SetActive(types.Count > 0);
			CommonUtils.PrepareEnoughChildren(this.attainmentLayoutHolder, this.attainmentLayoutLineTemplate.gameObject, rowAmount, null);
			for (int i = 0; i < types.Count; i++)
			{
				int groupIndex = i / 5;
				Transform layout = this.attainmentLayoutHolder.GetChild(groupIndex);
				bool flag2 = i % 5 == 0;
				if (flag2)
				{
					int itemCountInThisLine = Math.Clamp(types.Count - groupIndex * 5, 0, 5);
					CommonUtils.PrepareEnoughChildren(layout, this.attainmentTemplate.gameObject, itemCountInThisLine, null);
				}
				Transform item = layout.GetChild(i % 5);
				Refers refers = item.GetComponent<Refers>();
				CImage icon = refers.CGet<CImage>("Icon");
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				sbyte type = types[i];
				icon.SetSprite((isCombatSkill ? "ui9_back_combatskill_small_1_" : "ui9_back_lifeskill_small_1_") + type.ToString(), false, null);
				string skillName = isCombatSkill ? CombatSkillType.Instance[type].Name : LifeSkillType.Instance[type].Name;
				label.text = skillName + ((i == types.Count - 1) ? string.Empty : sepChar);
				item.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
			}
		}

		// Token: 0x06006835 RID: 26677 RVA: 0x002FA258 File Offset: 0x002F8458
		private void RefreshClothingArea()
		{
			short clothing = this._config.BonusClothing;
			ClothingItem clothingConfig = Clothing.Instance[clothing];
			string clothName = clothingConfig.Name.SetGradeColor((int)clothingConfig.Grade);
			this.clothBonusLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionTip_ClothBonus, clothName).ColorReplace();
		}

		// Token: 0x06006836 RID: 26678 RVA: 0x002FA2AC File Offset: 0x002F84AC
		private void RefreshSeniorityGains()
		{
			CommonUtils.PrepareEnoughChildren(this.seniorityGainTips.transform, this.seniorityGainTemplate.gameObject, this._config.SeniorityGainTips.Length, null);
			int i = 0;
			while (i < this._config.SeniorityGainTips.Length)
			{
				uint? dlcId = (i < this._config.SeniorityGainTipsDlcId.Length) ? new uint?(this._config.SeniorityGainTipsDlcId[i]) : null;
				if (dlcId == null)
				{
					goto IL_A7;
				}
				uint? num = dlcId;
				uint num2 = 0U;
				if (num.GetValueOrDefault() == num2 & num != null)
				{
					goto IL_A7;
				}
				bool flag = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(dlcId.Value);
				IL_A8:
				bool isDlcInstalled = flag;
				Transform tip = this.seniorityGainTips.GetChild(i);
				bool flag2 = !isDlcInstalled;
				if (flag2)
				{
					tip.gameObject.SetActive(false);
				}
				else
				{
					Refers refers = tip.GetComponent<Refers>();
					TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
					label.text = this._config.SeniorityGainTips[i].ColorReplace();
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						float labelHeight = label.GetComponent<RectTransform>().rect.height;
						LayoutElement layoutElement = refers.GetComponent<LayoutElement>();
						layoutElement.preferredHeight = labelHeight;
					});
				}
				i++;
				continue;
				IL_A7:
				flag = true;
				goto IL_A8;
			}
		}

		// Token: 0x06006837 RID: 26679 RVA: 0x002FA404 File Offset: 0x002F8604
		private static string FormatPercent(int percent)
		{
			string color = (percent > 100) ? "brightblue" : ((percent < 100) ? "brightred" : "pinkyellow");
			return string.Format("{0}%", percent).SetColor(color);
		}

		// Token: 0x06006838 RID: 26680 RVA: 0x002FA44A File Offset: 0x002F864A
		private void InitRefers()
		{
		}

		// Token: 0x040049BC RID: 18876
		[SerializeField]
		private GameObject clothingArea;

		// Token: 0x040049BD RID: 18877
		[SerializeField]
		private RectTransform seniorityGainTips;

		// Token: 0x040049BE RID: 18878
		[SerializeField]
		private Refers attainmentTemplate;

		// Token: 0x040049BF RID: 18879
		[SerializeField]
		private Refers seniorityGainTemplate;

		// Token: 0x040049C0 RID: 18880
		[SerializeField]
		private TextMeshProUGUI clothBonusLabel;

		// Token: 0x040049C1 RID: 18881
		[SerializeField]
		private TextMeshProUGUI descLabel;

		// Token: 0x040049C2 RID: 18882
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040049C3 RID: 18883
		[SerializeField]
		private RectTransform attainmentLayoutHolder;

		// Token: 0x040049C4 RID: 18884
		[SerializeField]
		private RectTransform attainmentLayoutLineTemplate;

		// Token: 0x040049C5 RID: 18885
		[SerializeField]
		private RectTransform attainmentArea;

		// Token: 0x040049C6 RID: 18886
		private string _professionId;

		// Token: 0x040049C7 RID: 18887
		private ProfessionItem _config;
	}
}
