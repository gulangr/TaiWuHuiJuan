using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000846 RID: 2118
	public class MainPanel : MonoBehaviour
	{
		// Token: 0x17000C60 RID: 3168
		// (get) Token: 0x060066F0 RID: 26352 RVA: 0x002EF4DD File Offset: 0x002ED6DD
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x060066F1 RID: 26353 RVA: 0x002EF4EC File Offset: 0x002ED6EC
		public void Set(CharacterDisplayDataForMapBlock data)
		{
			bool isTaiwu = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == data.CharacterId;
			this.SetAvatar(data, isTaiwu);
			this.SetInjury(data);
			this.SetRelation(data);
			this.SetProperty(data, isTaiwu);
		}

		// Token: 0x060066F2 RID: 26354 RVA: 0x002EF52F File Offset: 0x002ED72F
		private void OnLanguageChange(ArgumentBox _)
		{
			this.OnInjuryLanguageChange();
			this.OnRelationLanguageChange();
		}

		// Token: 0x060066F3 RID: 26355 RVA: 0x002EF540 File Offset: 0x002ED740
		private void SetAvatar(CharacterDisplayDataForMapBlock data, bool isTaiwu)
		{
			data.AvatarRelatedData.AvatarData.DarkAshStyle = CommonUtils.GetDarkAshStyle(data.CharacterId, data.DarkAshProtector);
			bool flag = data.TemplateId >= 0;
			if (flag)
			{
				this.avatar.Refresh(data.AvatarRelatedData, data.TemplateId);
			}
			else
			{
				this.avatar.Refresh(data.AvatarRelatedData);
			}
			this.title.text = (this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameRelatedData, isTaiwu, false));
			this.consummateLevelIcon.SetSprite(CommonUtils.GetConsummateLevelShowDataLegacy(data.ConsummateLevel).Item1, false, null);
			this.consummateLevelText.text = data.ConsummateLevel.ToString();
			this.neiliIcon.SetSprite(CommonUtils.GetNeiliTypeSpriteName(data.NeiliProportionOfFiveElements.GetNeiliType(data.BirthMonth)), false, null);
		}

		// Token: 0x060066F4 RID: 26356 RVA: 0x002EF62C File Offset: 0x002ED82C
		private void OnInjuryLanguageChange()
		{
			LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
			if (!true)
			{
			}
			float num;
			if (curLanguageType != LocalStringManager.LanguageType.CN)
			{
				num = this.enWidth;
			}
			else
			{
				num = this.cnWidth;
			}
			if (!true)
			{
			}
			float width = num;
			this.outerTrans.sizeDelta = this.outerTrans.sizeDelta.SetX(width);
			this.innerTrans.sizeDelta = this.innerTrans.sizeDelta.SetX(width);
			this.poisonTrans.sizeDelta = this.poisonTrans.sizeDelta.SetX(width);
			this.qiTrans.sizeDelta = this.qiTrans.sizeDelta.SetX(width);
			this.healthTrans.sizeDelta = this.healthTrans.sizeDelta.SetX(width);
		}

		// Token: 0x060066F5 RID: 26357 RVA: 0x002EF6F4 File Offset: 0x002ED8F4
		private void SetInjury(CharacterDisplayDataForMapBlock data)
		{
			this.OnInjuryLanguageChange();
			ValueTuple<sbyte, sbyte> bothSum = data.Injuries.GetBothSum();
			sbyte outer = bothSum.Item1;
			sbyte inner = bothSum.Item2;
			CharacterItem characterItem = (data.TemplateId == -1) ? null : Character.Instance[data.TemplateId];
			this.outerValue.text = ((characterItem != null && characterItem.OuterInjuryImmunity) ? LocalStringManager.Get(LanguageKey.LK_PoisonImmune) : outer.ToString());
			this.innerValue.text = ((characterItem != null && characterItem.InnerInjuryImmunity) ? LocalStringManager.Get(LanguageKey.LK_PoisonImmune) : inner.ToString());
			PoisonsAndLevels poisonsAndLevels = data.Poisons.GetPoisonsAndLevels();
			bool flag;
			if (characterItem == null)
			{
				flag = false;
			}
			else
			{
				flag = !characterItem.PoisonImmunities.Exist((bool p) => !p);
			}
			bool allPoisonImmune = flag;
			this.poisonValue.text = (allPoisonImmune ? LocalStringManager.Get(LanguageKey.LK_PoisonImmune) : poisonsAndLevels.GetTotalLevel().ToString().SetColor("poisoned"));
			this.qiState.Set(data.DisorderOfQi);
			this.healthState.Set(data.Health, data.LeftMaxHealth, -1);
		}

		// Token: 0x17000C61 RID: 3169
		// (get) Token: 0x060066F6 RID: 26358 RVA: 0x002EF834 File Offset: 0x002EDA34
		private TMP_Text[] RelationValues
		{
			get
			{
				LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
				if (!true)
				{
				}
				TMP_Text[] result;
				if (curLanguageType != LocalStringManager.LanguageType.CN)
				{
					result = this.relationValuesLong;
				}
				else
				{
					result = this.relationValuesShort;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000C62 RID: 3170
		// (get) Token: 0x060066F7 RID: 26359 RVA: 0x002EF868 File Offset: 0x002EDA68
		private RectTransform[] Relations
		{
			get
			{
				LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
				if (!true)
				{
				}
				RectTransform[] result;
				if (curLanguageType != LocalStringManager.LanguageType.CN)
				{
					result = this.relationsLong;
				}
				else
				{
					result = this.relationsShort;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x060066F8 RID: 26360 RVA: 0x002EF89C File Offset: 0x002EDA9C
		private void OnRelationLanguageChange()
		{
			foreach (RectTransform item in this.relationsShort.Concat(this.relationsLong))
			{
				item.gameObject.SetActive(false);
			}
		}

		// Token: 0x060066F9 RID: 26361 RVA: 0x002EF900 File Offset: 0x002EDB00
		private void SetRelation(CharacterDisplayDataForMapBlock data)
		{
			this.OnRelationLanguageChange();
			List<short> relationshipToTaiwuList2 = data.RelationshipToTaiwuList;
			List<short> list;
			if (relationshipToTaiwuList2 == null)
			{
				list = null;
			}
			else
			{
				list = (from id in relationshipToTaiwuList2
				where RelationDisplayType.Instance[id].TipToTaiwuDisplayOrder > 0
				orderby RelationDisplayType.Instance[id].TipToTaiwuDisplayOrder
				select id).ToList<short>();
			}
			List<short> relationshipToTaiwuList = list;
			int count = (relationshipToTaiwuList != null) ? relationshipToTaiwuList.Count : 0;
			for (int i = 0; i < this.Relations.Length; i++)
			{
				RectTransform child = this.Relations[i];
				bool flag = i < count;
				if (!flag)
				{
					break;
				}
				short id2 = relationshipToTaiwuList[i];
				RelationDisplayTypeItem relationDisplayTypeItem = RelationDisplayType.Instance[id2];
				child.gameObject.SetActive(true);
				this.RelationValues[i].text = relationDisplayTypeItem.Name;
			}
			this.noRelation.SetActive(count == 0);
		}

		// Token: 0x060066FA RID: 26362 RVA: 0x002EF9FC File Offset: 0x002EDBFC
		private void SetProperty(CharacterDisplayDataForMapBlock data, bool isTaiwu)
		{
			this.age.Set(LanguageKey.LK_Char_Age.Tr(), LanguageKey.LK_Age.TrFormat(data.Age), null);
			Game.Components.Character.Gender gender = this.gender;
			sbyte b = data.Gender;
			if (!true)
			{
			}
			CommonUtils.EDisplayGender displayGender;
			if (b != 0)
			{
				if (b != 1)
				{
					displayGender = CommonUtils.EDisplayGender.Hidden;
				}
				else
				{
					displayGender = CommonUtils.EDisplayGender.Male;
				}
			}
			else
			{
				displayGender = CommonUtils.EDisplayGender.Female;
			}
			if (!true)
			{
			}
			gender.Set(displayGender);
			Charm charm = this.charm;
			short num = data.Charm;
			sbyte b2 = data.Gender;
			short num2 = data.Age;
			short clothingDisplayId = data.AvatarRelatedData.ClothingDisplayId;
			bool isFixedPresetType = CreatingType.IsFixedPresetType(data.CreatingType);
			AvatarData avatarData = data.AvatarRelatedData.AvatarData;
			charm.Set(num, b2, num2, clothingDisplayId, isFixedPresetType, avatarData == null || avatarData.FaceVisible);
			this.happiness.SetType(data.HappinessType);
			this.behaviorType.Set(data.BehaviorType);
			string text;
			this.organization.Set(data.OrganizationInfo, CommonUtils.TryGetCharacterSpecialGradeName((int)data.TemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.OrganizationInfo), null);
			this.identity.Set(data.OrganizationInfo, (int)data.TemplateId, data.Gender, data.Age, data.IsReclusiveChar, null, false);
			this.fame.Set(data.FameType, isTaiwu);
			this.favorability.Set(data.FavorabilityToTaiwu, isTaiwu, true);
			this.alertness.Set(data.Alertness, false);
			this.influence.Set(LanguageKey.LK_Settlement_Influence.Tr(), data.InfluencePower.ToString(), null);
			this.samsara.Set(LanguageKey.LK_Samsara.Tr(), data.PreexistenceCharCount.ToString(), null);
			CurrentProfession currentProfession = this.currentProfession;
			if (currentProfession != null)
			{
				currentProfession.Set(data.CurrentProfession, data.CreatingType == 1);
			}
			List<sbyte> lst = data.LegendaryBookTypeList;
			bool flag = lst != null && lst.Count > 0;
			if (flag)
			{
				this.legendaryBookHolder.gameObject.SetActive(true);
				this.legendaryBookText.text = ((lst != null && lst.Count > 1) ? LanguageKey.LK_MouseTip_LegendaryBook_MapBlockTip_Many.TrFormat(lst.Count) : LanguageKey.LK_MouseTip_LegendaryBook_MapBlockTip_One.TrFormat(Misc.Instance[CombatSkillType.Instance[lst[0]].LegendaryBookTemplateId].Name)).ColorReplace();
			}
			else
			{
				this.legendaryBookHolder.gameObject.SetActive(false);
			}
		}

		// Token: 0x04004877 RID: 18551
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004878 RID: 18552
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004879 RID: 18553
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x0400487A RID: 18554
		[SerializeField]
		private TMP_Text consummateLevelText;

		// Token: 0x0400487B RID: 18555
		[SerializeField]
		private CImage neiliIcon;

		// Token: 0x0400487C RID: 18556
		[SerializeField]
		private CImage consummateLevelIcon;

		// Token: 0x0400487D RID: 18557
		[SerializeField]
		private TMP_Text outerValue;

		// Token: 0x0400487E RID: 18558
		[SerializeField]
		private TMP_Text innerValue;

		// Token: 0x0400487F RID: 18559
		[SerializeField]
		private TMP_Text poisonValue;

		// Token: 0x04004880 RID: 18560
		[SerializeField]
		private RectTransform outerTrans;

		// Token: 0x04004881 RID: 18561
		[SerializeField]
		private RectTransform innerTrans;

		// Token: 0x04004882 RID: 18562
		[SerializeField]
		private RectTransform poisonTrans;

		// Token: 0x04004883 RID: 18563
		[SerializeField]
		private RectTransform qiTrans;

		// Token: 0x04004884 RID: 18564
		[SerializeField]
		private RectTransform healthTrans;

		// Token: 0x04004885 RID: 18565
		[SerializeField]
		private QiState qiState;

		// Token: 0x04004886 RID: 18566
		[SerializeField]
		private Health healthState;

		// Token: 0x04004887 RID: 18567
		[SerializeField]
		private float cnWidth = 118f;

		// Token: 0x04004888 RID: 18568
		[SerializeField]
		private float enWidth = 200f;

		// Token: 0x04004889 RID: 18569
		[SerializeField]
		private TMP_Text[] relationValuesShort;

		// Token: 0x0400488A RID: 18570
		[SerializeField]
		private TMP_Text[] relationValuesLong;

		// Token: 0x0400488B RID: 18571
		[SerializeField]
		private RectTransform[] relationsShort;

		// Token: 0x0400488C RID: 18572
		[SerializeField]
		private RectTransform[] relationsLong;

		// Token: 0x0400488D RID: 18573
		[SerializeField]
		private GameObject noRelation;

		// Token: 0x0400488E RID: 18574
		[SerializeField]
		private PropertyItem age;

		// Token: 0x0400488F RID: 18575
		[SerializeField]
		private Game.Components.Character.Gender gender;

		// Token: 0x04004890 RID: 18576
		[SerializeField]
		private Charm charm;

		// Token: 0x04004891 RID: 18577
		[SerializeField]
		private Happiness happiness;

		// Token: 0x04004892 RID: 18578
		[SerializeField]
		private Game.Components.Character.BehaviorType behaviorType;

		// Token: 0x04004893 RID: 18579
		[SerializeField]
		private Game.Components.Character.Organization organization;

		// Token: 0x04004894 RID: 18580
		[SerializeField]
		private Identity identity;

		// Token: 0x04004895 RID: 18581
		[SerializeField]
		private Fame fame;

		// Token: 0x04004896 RID: 18582
		[SerializeField]
		private Favorability favorability;

		// Token: 0x04004897 RID: 18583
		[SerializeField]
		private Alertness alertness;

		// Token: 0x04004898 RID: 18584
		[SerializeField]
		private PropertyItem influence;

		// Token: 0x04004899 RID: 18585
		[SerializeField]
		private PropertyItem samsara;

		// Token: 0x0400489A RID: 18586
		[SerializeField]
		private RectTransform legendaryBookHolder;

		// Token: 0x0400489B RID: 18587
		[SerializeField]
		private TMP_Text legendaryBookText;

		// Token: 0x0400489C RID: 18588
		[SerializeField]
		private CurrentProfession currentProfession;
	}
}
