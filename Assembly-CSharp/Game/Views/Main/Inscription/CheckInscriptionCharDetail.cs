using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Global.Inscription;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Main.Inscription
{
	// Token: 0x02000978 RID: 2424
	public class CheckInscriptionCharDetail : MonoBehaviour
	{
		// Token: 0x0600744A RID: 29770 RVA: 0x003623D9 File Offset: 0x003605D9
		private void OnEnable()
		{
			this.UpdateConfig();
		}

		// Token: 0x0600744B RID: 29771 RVA: 0x003623E4 File Offset: 0x003605E4
		private void UpdateConfig()
		{
			bool isCn = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			this._currentFeatureTemplate = (isCn ? this.featureTemplateCn : this.featureTemplateEn);
			GridLayoutGroup group;
			bool flag = this.featureRoot.TryGetComponent<GridLayoutGroup>(out group);
			if (flag)
			{
				group.constraintCount = (isCn ? this.featureGroupCountCn : this.featureGroupCountEn);
				group.cellSize = new Vector2(isCn ? this.featureGroupCellSizeXCn : this.featureGroupCellSizeXEn, this.featureGroupCellSizeY);
			}
			this.featureTemplateCn.gameObject.SetActive(false);
			this.featureTemplateEn.gameObject.SetActive(false);
			this.skillTemplate.gameObject.SetActive(false);
		}

		// Token: 0x0600744C RID: 29772 RVA: 0x00362498 File Offset: 0x00360698
		public unsafe void SetData(CheckInscriptionCharData data)
		{
			bool flag = this._currentFeatureTemplate == null;
			if (flag)
			{
				this.UpdateConfig();
			}
			this._data = data;
			this.root.SetActive(true);
			InscribedCharacter c = data.Character;
			this.nameLabel.text = c.Surname + c.GivenName;
			MonthItem monthConfig = Month.Instance[c.BirthMonth];
			string monthName = ((monthConfig != null) ? monthConfig.Name : null) ?? string.Empty;
			this.ageLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Inscription_Age_With_Month, c.CurrAge, monthName);
			this.SetAgeTip(c);
			AvatarData avatar = c.Avatar;
			bool faceVisible = avatar == null || avatar.FaceVisible;
			this.charm.Set(data.Charm, c.Gender, c.ActualAge, c.ClothingDisplayId, false, faceVisible);
			CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(c.Gender, -1);
			this.gender.Set(displayGender);
			MainAttributes maxAttrs = c.CalcMaxMainAttributes(c.ActualAge);
			int[] displayOrder = new int[]
			{
				0,
				3,
				1,
				4,
				2,
				5
			};
			int i = 0;
			while (i < this.mainAttributes.Length && i < displayOrder.Length)
			{
				int attrIndex = displayOrder[i];
				string attrName = Attribute.GetMainAttributeName((sbyte)attrIndex);
				string iconName = "ui9_icon_attribute_major_big_" + attrIndex.ToString();
				this.mainAttributes[i].Set(iconName, attrName, maxAttrs.Get((sbyte)attrIndex).ToString(), null, false);
				int templateId = attrIndex;
				CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[templateId];
				bool flag2 = config != null;
				if (flag2)
				{
					this.mainAttributes[i].SetTip(config.Name, config.Desc);
				}
				i++;
			}
			LifeSkillShorts lifeSkillValues = c.CalcLifeSkillQualifications(c.ActualAge);
			int lifeCount = 16;
			CommonUtils.PrepareEnoughChildren(this.lifeSkillRoot, this.skillTemplate.gameObject, lifeCount, null);
			for (int j = 0; j < lifeCount; j++)
			{
				CheckInscriptionSkillItem item = this.lifeSkillRoot.GetChild(j).GetComponent<CheckInscriptionSkillItem>();
				item.SetLifeSkill(j, (int)(*lifeSkillValues[j]));
				item.gameObject.SetActive(true);
			}
			CombatSkillShorts combatSkillValues = c.CalcCombatSkillQualifications(c.ActualAge);
			int combatCount = 14;
			CommonUtils.PrepareEnoughChildren(this.combatSkillRoot, this.skillTemplate.gameObject, combatCount, null);
			for (int k = 0; k < combatCount; k++)
			{
				CheckInscriptionSkillItem item2 = this.combatSkillRoot.GetChild(k).GetComponent<CheckInscriptionSkillItem>();
				item2.SetCombatSkill(k, (int)(*combatSkillValues[k]));
				item2.gameObject.SetActive(true);
			}
			bool flag3 = c.FeatureIds != null;
			if (flag3)
			{
				for (int l = 0; l < this.featureRoot.childCount; l++)
				{
					Transform child = this.featureRoot.GetChild(l);
					bool flag4 = child.name != LocalStringManager.CurLanguageType.ToString();
					if (flag4)
					{
						Object.Destroy(child.gameObject);
					}
				}
				CommonUtils.PrepareEnoughChildren(this.featureRoot, this._currentFeatureTemplate.gameObject, c.FeatureIds.Count, null);
				for (int m = 0; m < c.FeatureIds.Count; m++)
				{
					Feature view = this.featureRoot.GetChild(m).GetComponent<Feature>();
					short fid = c.FeatureIds[m];
					view.gameObject.name = LocalStringManager.CurLanguageType.ToString();
					view.Set(fid, data.Key.CharId, false, -1);
					view.SetTipTemplateDataOnly(true);
					view.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600744D RID: 29773 RVA: 0x003628C4 File Offset: 0x00360AC4
		private void SetAgeTip(InscribedCharacter character)
		{
			bool flag = this.ageTip == null;
			if (!flag)
			{
				this.ageTip.enabled = true;
				this.ageTip.Type = TipType.GeneralLines;
				TooltipInvoker tooltipInvoker = this.ageTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				int lineCount = 0;
				this.ageTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Char_Age));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Age_Tip_Desc)
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				});
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					"mousetip_age_0",
					LocalStringManager.GetFormat(LanguageKey.LK_Age_DisplayAge_FormatBase, character.CurrAge).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				});
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_1).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_2).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_3).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_4).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(4, null, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					"mousetip_age_1",
					LocalStringManager.GetFormat(LanguageKey.LK_Age_ActualAge_FormatBase, character.ActualAge).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				});
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_ActualAge_TipContent_1).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_ActualAge_TipContent_2).SetColor("pinkyellow")
				}, null));
				this.ageTip.RuntimeParam.Set("LineCount", lineCount);
			}
		}

		// Token: 0x0600744E RID: 29774 RVA: 0x00362D50 File Offset: 0x00360F50
		public void Clear()
		{
			this.root.SetActive(false);
		}

		// Token: 0x040056CB RID: 22219
		[SerializeField]
		private GameObject root;

		// Token: 0x040056CC RID: 22220
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040056CD RID: 22221
		[SerializeField]
		private TextMeshProUGUI ageLabel;

		// Token: 0x040056CE RID: 22222
		[SerializeField]
		private TooltipInvoker ageTip;

		// Token: 0x040056CF RID: 22223
		[SerializeField]
		private Charm charm;

		// Token: 0x040056D0 RID: 22224
		[SerializeField]
		private Game.Components.Character.Gender gender;

		// Token: 0x040056D1 RID: 22225
		[SerializeField]
		private Transform featureRoot;

		// Token: 0x040056D2 RID: 22226
		[SerializeField]
		private Feature featureTemplateCn;

		// Token: 0x040056D3 RID: 22227
		[SerializeField]
		private Feature featureTemplateEn;

		// Token: 0x040056D4 RID: 22228
		[SerializeField]
		private PropertyItem[] mainAttributes;

		// Token: 0x040056D5 RID: 22229
		[SerializeField]
		private Transform lifeSkillRoot;

		// Token: 0x040056D6 RID: 22230
		[SerializeField]
		private Transform combatSkillRoot;

		// Token: 0x040056D7 RID: 22231
		[SerializeField]
		private CheckInscriptionSkillItem skillTemplate;

		// Token: 0x040056D8 RID: 22232
		[SerializeField]
		private int featureGroupCountCn = 5;

		// Token: 0x040056D9 RID: 22233
		[SerializeField]
		private int featureGroupCountEn = 3;

		// Token: 0x040056DA RID: 22234
		[SerializeField]
		private float featureGroupCellSizeXCn = 234f;

		// Token: 0x040056DB RID: 22235
		[SerializeField]
		private float featureGroupCellSizeXEn = 480f;

		// Token: 0x040056DC RID: 22236
		[SerializeField]
		private float featureGroupCellSizeY = 108f;

		// Token: 0x040056DD RID: 22237
		private Feature _currentFeatureTemplate;

		// Token: 0x040056DE RID: 22238
		private CheckInscriptionCharData _data;
	}
}
