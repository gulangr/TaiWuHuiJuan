using System;
using System.Collections.Generic;
using System.Text;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F19 RID: 3865
	public class CharacterCircle : MonoBehaviour
	{
		// Token: 0x0600B210 RID: 45584 RVA: 0x00511378 File Offset: 0x0050F578
		public void Set(CharacterDisplayData displayData, bool isTaiwu = false, bool isHideTitleWhenNone = false)
		{
			bool flag = displayData == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				bool flag2 = this.avatar != null;
				if (flag2)
				{
					this.avatar.Refresh(displayData, true);
				}
				bool flag3 = this.characterNameAndTitle != null;
				if (flag3)
				{
					this.characterNameAndTitle.Set(displayData, isTaiwu, isHideTitleWhenNone);
				}
				this.SetBirthAndAge(displayData);
				this.SetAgeTip(displayData);
				this.SetHealth(displayData.Health, displayData.LeftMaxHealth);
				this.SetInnateFiveElementsTypeTip(displayData);
			}
		}

		// Token: 0x0600B211 RID: 45585 RVA: 0x00511404 File Offset: 0x0050F604
		public void SetEmpty()
		{
			bool flag = this.avatar != null;
			if (flag)
			{
				this.avatar.ResetToBlank(false);
			}
			bool flag2 = this.ageTip != null;
			if (flag2)
			{
				this.ageTip.enabled = false;
			}
		}

		// Token: 0x0600B212 RID: 45586 RVA: 0x00511450 File Offset: 0x0050F650
		public void PlayAnim()
		{
			this.avatarGroup.alpha = 0f;
			this.textGroup.alpha = 0f;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				this.avatarGroup.DOFade(1f, 0.2f);
			});
			sequence.AppendInterval(0.16f);
			sequence.AppendCallback(delegate
			{
				this.textGroup.DOFade(1f, 0.66f);
			});
		}

		// Token: 0x0600B213 RID: 45587 RVA: 0x005114B8 File Offset: 0x0050F6B8
		private void SetBirthAndAge(CharacterDisplayData displayData)
		{
			bool flag = this.birth != null;
			if (flag)
			{
				sbyte birthMonth = SingletonObject.getInstance<TimeManager>().GetMonthInYear(displayData.BirthDate);
				bool flag2 = birthMonth < 0;
				if (flag2)
				{
					birthMonth += 12;
				}
				MonthItem monthConfig = Month.Instance[birthMonth];
				this.birth.text = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Tips, monthConfig.Name);
			}
			bool flag3 = this.age != null;
			if (flag3)
			{
				bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(displayData.CreatingType);
				bool shouldHideAge = Character.Instance[displayData.TemplateId].HideAge && isNonEvolutionaryType;
				this.age.text = (shouldHideAge ? "-" : LocalStringManager.GetFormat(LanguageKey.LK_Age_EnglishWrap, displayData.PhysiologicalAge));
			}
		}

		// Token: 0x0600B214 RID: 45588 RVA: 0x00511588 File Offset: 0x0050F788
		private void SetAgeTip(CharacterDisplayData displayData)
		{
			bool flag = this.ageTip == null;
			if (!flag)
			{
				bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(displayData.CreatingType);
				CharacterItem charConfig = Character.Instance[displayData.TemplateId];
				bool flag2 = isNonEvolutionaryType || (charConfig != null && charConfig.HideAge);
				if (flag2)
				{
					this.ageTip.enabled = false;
				}
				else
				{
					this.ageTip.enabled = true;
					this.ageTip.Type = TipType.GeneralLines;
					int lineCount = 0;
					TooltipInvoker tooltipInvoker = this.ageTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.ageTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Char_Age));
					this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
					{
						LocalStringManager.Get(LanguageKey.LK_Age_Tip_Desc)
					}, null));
					this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
					{
						Type = 4,
						PreferredHeight = 20f
					});
					StringBuilder ageInfo = new StringBuilder();
					ageInfo.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Age_DisplayAge_FormatBase, displayData.PhysiologicalAge).SetColor("pinkyellow"));
					AgeAffector ageAffector = (AgeAffector)displayData.AgeAffector;
					bool flag3 = (ageAffector & AgeAffector.TaoismPassive) > AgeAffector.None;
					if (flag3)
					{
						ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Profession_TaoismPassive.Tr());
					}
					else
					{
						bool flag4 = (ageAffector & AgeAffector.TaoismActive) > AgeAffector.None;
						if (flag4)
						{
							ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Profession_TaoismActive.Tr());
						}
					}
					bool flag5 = (ageAffector & AgeAffector.MaleKeepYoung) > AgeAffector.None;
					if (flag5)
					{
						ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Gongfa_Xisui.Tr());
					}
					else
					{
						bool flag6 = (ageAffector & AgeAffector.FemaleKeepYoung) > AgeAffector.None;
						if (flag6)
						{
							ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Gongfa_Taiyin.Tr());
						}
					}
					ageInfo.Remove(ageInfo.Length - 1, 1);
					this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
					{
						"mousetip_age_0",
						ageInfo.ToString()
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
					short displayActualAge = displayData.ActualAge;
					bool flag7 = displayData.FeatureIds.Contains(879);
					if (flag7)
					{
						displayActualAge = displayData.ActualAge + 16;
					}
					this.ageTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
					{
						"mousetip_age_1",
						LocalStringManager.GetFormat(LanguageKey.LK_Age_ActualAge_FormatBase, displayActualAge).SetColor("pinkyellow")
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
					this.ageTip.RuntimeParam.Set("EncyclopediaLink", 79);
				}
			}
		}

		// Token: 0x0600B215 RID: 45589 RVA: 0x00511B4C File Offset: 0x0050FD4C
		private void SetHealth(short health, short leftMaxHealth)
		{
			ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(health, leftMaxHealth, -1);
			EHealthType healthType = (EHealthType)info.Item3;
			this.UpdateHealthState(healthType);
		}

		// Token: 0x0600B216 RID: 45590 RVA: 0x00511B74 File Offset: 0x0050FD74
		private void SetInnateFiveElementsTypeTip(CharacterDisplayData displayData)
		{
			bool flag = this.innateFiveElementsTypeTip == null;
			if (!flag)
			{
				sbyte birthMonth = SingletonObject.getInstance<TimeManager>().GetMonthInYear(displayData.BirthDate);
				this.innateFiveElementsTypeTip.Type = TipType.InnateFiveElements;
				TooltipInvoker tooltipInvoker = this.innateFiveElementsTypeTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.innateFiveElementsTypeTip.RuntimeParam.Set("BirthMonth", (int)birthMonth);
			}
		}

		// Token: 0x0600B217 RID: 45591 RVA: 0x00511BE8 File Offset: 0x0050FDE8
		private void UpdateHealthState(EHealthType healthType)
		{
			bool flag = this.healthStateImage == null;
			if (!flag)
			{
				if (healthType != EHealthType.Dying)
				{
					if (healthType - EHealthType.Sick > 2)
					{
						this.healthStateImage.texture = this.healthWorseSprite;
					}
					else
					{
						this.healthStateImage.texture = this.healthBetterSprite;
					}
				}
				else
				{
					this.healthStateImage.texture = this.healthDyingSprite;
				}
			}
		}

		// Token: 0x04008A13 RID: 35347
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04008A14 RID: 35348
		[SerializeField]
		private NameAndTitle characterNameAndTitle;

		// Token: 0x04008A15 RID: 35349
		[SerializeField]
		private TextMeshProUGUI birth;

		// Token: 0x04008A16 RID: 35350
		[SerializeField]
		private TextMeshProUGUI age;

		// Token: 0x04008A17 RID: 35351
		[SerializeField]
		private TooltipInvoker innateFiveElementsTypeTip;

		// Token: 0x04008A18 RID: 35352
		[SerializeField]
		private TooltipInvoker ageTip;

		// Token: 0x04008A19 RID: 35353
		[SerializeField]
		private CRawImage healthStateImage;

		// Token: 0x04008A1A RID: 35354
		[SerializeField]
		private Texture healthBetterSprite;

		// Token: 0x04008A1B RID: 35355
		[SerializeField]
		private Texture healthWorseSprite;

		// Token: 0x04008A1C RID: 35356
		[SerializeField]
		private Texture healthDyingSprite;

		// Token: 0x04008A1D RID: 35357
		[SerializeField]
		private CanvasGroup avatarGroup;

		// Token: 0x04008A1E RID: 35358
		[SerializeField]
		private CanvasGroup textGroup;

		// Token: 0x04008A1F RID: 35359
		private const float AvatarDuration = 0.2f;

		// Token: 0x04008A20 RID: 35360
		private const float TextStart = 0.16f;

		// Token: 0x04008A21 RID: 35361
		private const float TextDuration = 0.66f;
	}
}
