using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DisplayConfig;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000805 RID: 2053
	public class NewGameSubPageCustomPresetNeili : NewGameSubPageCustomPresetItemBase
	{
		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x06006487 RID: 25735 RVA: 0x002E014C File Offset: 0x002DE34C
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000C18 RID: 3096
		// (get) Token: 0x06006488 RID: 25736 RVA: 0x002E014F File Offset: 0x002DE34F
		// (set) Token: 0x06006489 RID: 25737 RVA: 0x002E0157 File Offset: 0x002DE357
		public override bool StartGameChecked { get; set; }

		// Token: 0x17000C19 RID: 3097
		// (get) Token: 0x0600648A RID: 25738 RVA: 0x002E0160 File Offset: 0x002DE360
		public override int SpentPoints
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000C1A RID: 3098
		// (get) Token: 0x0600648B RID: 25739 RVA: 0x002E0163 File Offset: 0x002DE363
		public override int RemainingPoints
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x002E0168 File Offset: 0x002DE368
		private void Awake()
		{
			foreach (NewGameCustomPresetNeiliButton binding in this.fiveElementButtons)
			{
				binding.Initialize(new Action<sbyte>(this.SetNeiliType));
			}
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x002E01A3 File Offset: 0x002DE3A3
		private void OnEnable()
		{
			this.RefreshVisuals(false);
		}

		// Token: 0x0600648E RID: 25742 RVA: 0x002E01AE File Offset: 0x002DE3AE
		private void OnDestroy()
		{
			this.gradientSourceCanvasGroup.DOKill(false);
		}

		// Token: 0x0600648F RID: 25743 RVA: 0x002E01BE File Offset: 0x002DE3BE
		public override void RefreshUI()
		{
			this.RefreshVisuals(false);
		}

		// Token: 0x06006490 RID: 25744 RVA: 0x002E01C9 File Offset: 0x002DE3C9
		public override void ApplyToPreset(CustomProtagonistPresetItem presetItem)
		{
			presetItem.NeiliProportion = this._neiliProportion;
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x002E01D8 File Offset: 0x002DE3D8
		public override void ApplyFromPreset(CustomProtagonistPresetItem presetItem)
		{
			this._neiliProportion = presetItem.NeiliProportion;
			this._selectedNeiliType = CustomProtagonistPresetItem.GetNeiliTypeByProportion(this._neiliProportion);
		}

		// Token: 0x06006492 RID: 25746 RVA: 0x002E01F8 File Offset: 0x002DE3F8
		public void ApplyRecommendedNeiliType()
		{
			sbyte recommendedType;
			bool flag = !this.TryGetCurrentSectRecommendedType(out recommendedType);
			if (!flag)
			{
				this._selectedNeiliType = recommendedType;
				this._neiliProportion = CustomProtagonistPresetItem.GenerateNeiliProportionByNeiliType(recommendedType);
			}
		}

		// Token: 0x06006493 RID: 25747 RVA: 0x002E022C File Offset: 0x002DE42C
		public void SetNeiliType(sbyte type)
		{
			bool flag = this._selectedNeiliType == type;
			if (!flag)
			{
				this._selectedNeiliType = type;
				this._neiliProportion = CustomProtagonistPresetItem.GenerateNeiliProportionByNeiliType(type);
				base.NotifyDataModified();
				this.RefreshVisuals(true);
			}
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x002E026C File Offset: 0x002DE46C
		private void RefreshVisuals(bool animateGradient)
		{
			sbyte recommendedType;
			this.TryGetCurrentSectRecommendedType(out recommendedType);
			this.RefreshButtonStates(recommendedType);
			this.RefreshOrganizationBackground();
			this.RefreshGradientBackground(animateGradient);
			this.RefreshTips(recommendedType);
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x002E02A4 File Offset: 0x002DE4A4
		private void RefreshButtonStates(sbyte recommendedType)
		{
			for (int i = 0; i < this.fiveElementButtons.Length; i++)
			{
				NewGameCustomPresetNeiliButton binding = this.fiveElementButtons[i];
				bool isSelected = binding.FiveElementType == this._selectedNeiliType;
				binding.RefreshState(isSelected, NewGameSubPageCustomPresetNeili.IsConflictingFiveElement(binding.FiveElementType, recommendedType));
			}
		}

		// Token: 0x06006496 RID: 25750 RVA: 0x002E02F8 File Offset: 0x002DE4F8
		private void RefreshOrganizationBackground()
		{
			OrganizationItem sectConfig;
			bool flag = !this.TryGetCurrentSectConfig(out sectConfig);
			if (flag)
			{
				this.organizationBackground.SetTexture(null);
			}
			else
			{
				this.organizationBackground.SetTexture(string.Format("ui9_tex_newgame_custom_preset_organization_{0}", (int)(sectConfig.TemplateId - 1)));
			}
		}

		// Token: 0x06006497 RID: 25751 RVA: 0x002E0348 File Offset: 0x002DE548
		private void RefreshGradientBackground(bool animateGradient)
		{
			string targetTextureName = NewGameSubPageCustomPresetNeili.GetGradientTextureName(this._selectedNeiliType);
			this.gradientSourceCanvasGroup.DOKill(false);
			bool flag = string.IsNullOrEmpty(this._currentGradientTextureName);
			if (flag)
			{
				this.SetTextureWithSize(this.gradientSourceBackground, targetTextureName);
				this.SetTextureWithSize(this.gradientTargetBackground, targetTextureName);
				this.gradientSourceCanvasGroup.alpha = 0f;
				this._currentGradientTextureName = targetTextureName;
			}
			else
			{
				bool flag2 = !animateGradient || this._currentGradientTextureName == targetTextureName;
				if (flag2)
				{
					this.SetTextureWithSize(this.gradientSourceBackground, targetTextureName);
					this.SetTextureWithSize(this.gradientTargetBackground, targetTextureName);
					this.gradientSourceCanvasGroup.alpha = 0f;
					this._currentGradientTextureName = targetTextureName;
				}
				else
				{
					this.gradientSourceBackground.SetTexture(this._currentGradientTextureName);
					this.SetTextureWithSize(this.gradientTargetBackground, targetTextureName);
					this.gradientSourceCanvasGroup.alpha = 1f;
					this.gradientSourceCanvasGroup.DOFade(0f, this.gradientFadeDuration).SetEase(Ease.Linear);
					this._currentGradientTextureName = targetTextureName;
				}
			}
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x002E045A File Offset: 0x002DE65A
		private void SetTextureWithSize(CRawImage rawImage, string textureName)
		{
			rawImage.SetTexture(textureName);
			this.SetImageSize(rawImage, textureName);
		}

		// Token: 0x06006499 RID: 25753 RVA: 0x002E0470 File Offset: 0x002DE670
		private void SetImageSize(CRawImage rawImage, string textureName)
		{
			Vector2 size = (textureName == "ui9_tex_newgame_custom_preset_five_element_bg") ? this.fiveElementBgTextureSize : this.fiveElementTextureSize;
			rawImage.rectTransform.sizeDelta = size;
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x002E04A8 File Offset: 0x002DE6A8
		private void RefreshTips(sbyte recommendedType)
		{
			OrganizationItem sectConfig;
			string sectName = this.TryGetCurrentSectConfig(out sectConfig) ? sectConfig.Name : LanguageKey.LK_Common_None.Tr();
			string recommendedTypeName = NewGameSubPageCustomPresetNeili.IsValidFiveElementType(recommendedType) ? FiveElement.Instance[(int)recommendedType].Name : LanguageKey.LK_Common_None.Tr();
			for (int i = 0; i < this.fiveElementButtons.Length; i++)
			{
				this.RefreshSingleTip(this.fiveElementButtons[i], sectName, recommendedType, recommendedTypeName);
			}
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x002E0528 File Offset: 0x002DE728
		private void RefreshSingleTip(NewGameCustomPresetNeiliButton binding, string sectName, sbyte recommendedType, string recommendedTypeName)
		{
			binding.RefreshTips(delegate(ArgumentBox tipParam)
			{
				NeiliTypeItem neiliConfig = NeiliType.Instance[binding.FiveElementType];
				int lineCount = 0;
				tipParam.Set("Title", neiliConfig.Name);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(7, new List<string>
				{
					neiliConfig.SimpleDesc
				}, null), ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddSpacer(16f, ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(1, new List<string>
				{
					LanguageKey.LK_FIveElements_NeiliEffect.Tr()
				}, null), ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(7, new List<string>
				{
					neiliConfig.EffectDesc
				}, null), ref lineCount, tipParam);
				bool flag = NewGameSubPageCustomPresetNeili.IsConflictingFiveElement(binding.FiveElementType, recommendedType);
				if (flag)
				{
					NewGameSubPageCustomPresetNeili.AddSpacer(16f, ref lineCount, tipParam);
					NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(1, new List<string>
					{
						LanguageKey.LK_NewGame_CustomPreset_NeiliTips_Conflict.Tr()
					}, null), ref lineCount, tipParam);
					string relationText = NewGameSubPageCustomPresetNeili.GetConflictRelationText(binding.FiveElementType, recommendedType);
					bool flag2 = !string.IsNullOrEmpty(relationText);
					if (flag2)
					{
						NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(5, new List<string>
						{
							relationText
						}, null), ref lineCount, tipParam);
					}
					NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(3, new List<string>
					{
						LanguageKey.UI_NewGame_NeiliType_Tips1.Tr()
					}, null), ref lineCount, tipParam);
				}
				NewGameSubPageCustomPresetNeili.AddSpacer(16f, ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(1, new List<string>
				{
					LanguageKey.LK_NewGame_CustomPreset_NeiliTips_SectInfo.Tr()
				}, null), ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(3, new List<string>
				{
					LanguageKey.LK_NewGame_CustomPreset_NeiliTips_CurrentSect.TrFormat(sectName)
				}, null), ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(3, new List<string>
				{
					LanguageKey.LK_NewGame_CustomPreset_NeiliTips_RecommendNeili.TrFormat(recommendedTypeName)
				}, null), ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(3, new List<string>
				{
					LanguageKey.LK_NewGame_CustomPreset_NeiliTips_CurrentNeili.TrFormat(FiveElement.Instance[(int)binding.FiveElementType].Name)
				}, null), ref lineCount, tipParam);
				NewGameSubPageCustomPresetNeili.AddLine(new GeneralLineData(3, new List<string>
				{
					LanguageKey.LK_NewGame_CustomPreset_NeiliTips_RecommendSect.TrFormat(this.GetRecommendedSectNames(binding.FiveElementType))
				}, null), ref lineCount, tipParam);
				tipParam.Set("LineCount", lineCount);
			});
		}

		// Token: 0x0600649C RID: 25756 RVA: 0x002E0578 File Offset: 0x002DE778
		private static void AddLine(GeneralLineData lineData, ref int lineCount, ArgumentBox tipParam)
		{
			string format = "LineData{0}";
			int num = lineCount + 1;
			lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x0600649D RID: 25757 RVA: 0x002E05A8 File Offset: 0x002DE7A8
		private static void AddSpacer(float height, ref int lineCount, ArgumentBox tipParam)
		{
			string format = "LineData{0}";
			int num = lineCount + 1;
			lineCount = num;
			tipParam.SetObject(string.Format(format, num), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = height
			});
		}

		// Token: 0x0600649E RID: 25758 RVA: 0x002E05E8 File Offset: 0x002DE7E8
		private string GetRecommendedSectNames(sbyte fiveElementType)
		{
			bool flag = fiveElementType == 5;
			string result;
			if (flag)
			{
				result = LanguageKey.LK_NewGame_CustomPreset_NeiliTips_RecommendSectAny.Tr();
			}
			else
			{
				List<string> names = new List<string>();
				foreach (OrganizationItem orgConfig in ((IEnumerable<OrganizationItem>)Organization.Instance))
				{
					bool flag2 = !orgConfig.IsSect || orgConfig.FiveElementsType != fiveElementType;
					if (!flag2)
					{
						names.Add(orgConfig.Name);
					}
				}
				bool flag3 = names.Count == 0;
				if (flag3)
				{
					result = LanguageKey.LK_Common_None.Tr();
				}
				else
				{
					result = string.Join(LanguageKey.LK_Split_Symbol.Tr(), names);
				}
			}
			return result;
		}

		// Token: 0x0600649F RID: 25759 RVA: 0x002E06B0 File Offset: 0x002DE8B0
		private static string GetGradientTextureName(sbyte fiveElementType)
		{
			return NewGameSubPageCustomPresetNeili.IsValidSelectableFiveElementType(fiveElementType) ? string.Format("ui9_tex_newgame_custom_preset_five_element_{0}", fiveElementType) : "ui9_tex_newgame_custom_preset_five_element_bg";
		}

		// Token: 0x060064A0 RID: 25760 RVA: 0x002E06E4 File Offset: 0x002DE8E4
		private static bool IsConflictingFiveElement(sbyte currentType, sbyte recommendedType)
		{
			return NewGameSubPageCustomPresetNeili.IsValidFiveElementType(currentType) && NewGameSubPageCustomPresetNeili.IsValidFiveElementType(recommendedType) && (currentType == FiveElementsType.Countering[(int)recommendedType] || currentType == FiveElementsType.Countered[(int)recommendedType]);
		}

		// Token: 0x060064A1 RID: 25761 RVA: 0x002E0720 File Offset: 0x002DE920
		private static string GetConflictRelationText(sbyte currentType, sbyte recommendedType)
		{
			bool flag = !NewGameSubPageCustomPresetNeili.IsConflictingFiveElement(currentType, recommendedType);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string relationKey = (currentType == FiveElementsType.Countering[(int)recommendedType]) ? string.Format("LK_NewGame_CustomPreset_FiveElement_Relation_1_{0}", recommendedType) : string.Format("LK_NewGame_CustomPreset_FiveElement_Relation_1_{0}", currentType);
				result = LocalStringManager.Get(relationKey);
			}
			return result;
		}

		// Token: 0x060064A2 RID: 25762 RVA: 0x002E077C File Offset: 0x002DE97C
		private bool TryGetCurrentSectRecommendedType(out sbyte fiveElementType)
		{
			OrganizationItem sectConfig;
			bool flag = this.TryGetCurrentSectConfig(out sectConfig) && NewGameSubPageCustomPresetNeili.IsValidSelectableFiveElementType(sectConfig.FiveElementsType);
			bool result;
			if (flag)
			{
				fiveElementType = sectConfig.FiveElementsType;
				result = true;
			}
			else
			{
				fiveElementType = -1;
				result = false;
			}
			return result;
		}

		// Token: 0x060064A3 RID: 25763 RVA: 0x002E07BC File Offset: 0x002DE9BC
		private bool TryGetCurrentSectConfig(out OrganizationItem sectConfig)
		{
			sectConfig = null;
			string stateTemplateIdText;
			sbyte stateTemplateId;
			bool flag = this.ParentCustomPresetPage == null || !this.ParentCustomPresetPage.CreationInfoMap.TryGetValue("TaiwuVillageStateTemplateId", out stateTemplateIdText) || !sbyte.TryParse(stateTemplateIdText, out stateTemplateId);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MapStateItem stateConfig = MapState.Instance[stateTemplateId];
				sectConfig = Organization.Instance[stateConfig.SectID];
				result = (sectConfig != null);
			}
			return result;
		}

		// Token: 0x060064A4 RID: 25764 RVA: 0x002E0838 File Offset: 0x002DEA38
		private static bool IsValidFiveElementType(sbyte fiveElementType)
		{
			return fiveElementType >= 0 && fiveElementType < 5;
		}

		// Token: 0x060064A5 RID: 25765 RVA: 0x002E0858 File Offset: 0x002DEA58
		private static bool IsValidSelectableFiveElementType(sbyte fiveElementType)
		{
			return fiveElementType >= 0 && fiveElementType <= 5;
		}

		// Token: 0x04004614 RID: 17940
		[Header("内力五行")]
		[SerializeField]
		private NewGameCustomPresetNeiliButton[] fiveElementButtons;

		// Token: 0x04004615 RID: 17941
		[SerializeField]
		private CRawImage organizationBackground;

		// Token: 0x04004616 RID: 17942
		[SerializeField]
		private CRawImage gradientSourceBackground;

		// Token: 0x04004617 RID: 17943
		[SerializeField]
		private CanvasGroup gradientSourceCanvasGroup;

		// Token: 0x04004618 RID: 17944
		[SerializeField]
		private CRawImage gradientTargetBackground;

		// Token: 0x04004619 RID: 17945
		[SerializeField]
		private float gradientFadeDuration = 0.25f;

		// Token: 0x0400461A RID: 17946
		[SerializeField]
		private Vector2 fiveElementTextureSize;

		// Token: 0x0400461B RID: 17947
		[SerializeField]
		private Vector2 fiveElementBgTextureSize;

		// Token: 0x0400461C RID: 17948
		private NeiliProportionOfFiveElements _neiliProportion = CustomProtagonistPresetItem.GenerateNeiliProportionByNeiliType(5);

		// Token: 0x0400461D RID: 17949
		private sbyte _selectedNeiliType = 5;

		// Token: 0x0400461E RID: 17950
		private string _currentGradientTextureName;
	}
}
