using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200084F RID: 2127
	public class MouseTipDeadCharacterComplete : MouseTipBase
	{
		// Token: 0x06006752 RID: 26450 RVA: 0x002F1CC4 File Offset: 0x002EFEC4
		protected override void Init(ArgumentBox argsBox)
		{
			base.RectTransform.localScale = Vector3.zero;
			CharacterDisplayDataForTooltip data;
			bool flag = argsBox.Get<CharacterDisplayDataForTooltip>("Data", out data);
			if (flag)
			{
				this.OnDataReady(data);
			}
			else
			{
				int charId;
				bool flag2 = argsBox.Get("CharId", out charId);
				if (!flag2)
				{
					throw new ArgumentException("No character given.");
				}
				CharacterDomainMethod.AsyncCall.GetDeadCharacterDisplayDataForTooltip(this, charId, new AsyncMethodCallbackDelegate(this.<Init>g__CallBack|16_0));
			}
		}

		// Token: 0x06006753 RID: 26451 RVA: 0x002F1D34 File Offset: 0x002EFF34
		private void OnDataReady(CharacterDisplayDataForTooltip data)
		{
			bool flag = data == null;
			if (!flag)
			{
				this.SetMainFrame(data);
				this.lifeSkill.gameObject.SetActive(true);
				this.combatSkill.gameObject.SetActive(true);
				this.SetSkillFrame(data);
				base.RectTransform.localScale = Vector3.one;
			}
		}

		// Token: 0x06006754 RID: 26452 RVA: 0x002F1D91 File Offset: 0x002EFF91
		private void SetMainFrame(CharacterDisplayDataForTooltip data)
		{
			this.SetAvatar(data);
			this.SetGender(data);
			this.SetCharm(data);
			this.SetBehavior(data);
			this.SetAge(data);
			this.SetMainAttribute(data);
		}

		// Token: 0x06006755 RID: 26453 RVA: 0x002F1DC4 File Offset: 0x002EFFC4
		private void SetAvatar(CharacterDisplayDataForTooltip data)
		{
			bool flag = data.TemplateId >= 0;
			if (flag)
			{
				this.avatar.Refresh(data.AvatarRelatedData, data.TemplateId);
			}
			else
			{
				this.avatar.Refresh(data.AvatarRelatedData);
			}
			NameRelatedData nameRelatedData = data.GetNameRelatedData();
			bool isTaiwu = data.Id == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			string charNameStr = NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref nameRelatedData, isTaiwu, false);
			this.charName.text = charNameStr;
			this.title.text = charNameStr;
		}

		// Token: 0x06006756 RID: 26454 RVA: 0x002F1E4C File Offset: 0x002F004C
		private void SetGender(CharacterDisplayDataForTooltip data)
		{
			this.gender.Set(CommonUtils.GetDisplayGender(data.Gender, data.TemplateId));
		}

		// Token: 0x06006757 RID: 26455 RVA: 0x002F1E6C File Offset: 0x002F006C
		private void SetCharm(CharacterDisplayDataForTooltip data)
		{
			Charm charm = this.charm;
			short num = data.AvatarRelatedData.AvatarData.GetCharm(data.Age, data.AvatarRelatedData.ClothingDisplayId);
			sbyte b = data.Gender;
			short num2 = data.Age;
			short clothingDisplayId = data.AvatarRelatedData.ClothingDisplayId;
			bool isFixedPresetType = CreatingType.IsFixedPresetType(data.CreatingType);
			AvatarData avatarData = data.AvatarRelatedData.AvatarData;
			charm.Set(num, b, num2, clothingDisplayId, isFixedPresetType, avatarData == null || avatarData.FaceVisible);
		}

		// Token: 0x06006758 RID: 26456 RVA: 0x002F1EDF File Offset: 0x002F00DF
		private void SetBehavior(CharacterDisplayDataForTooltip data)
		{
			this.behaviourType.Set(data.BehaviorType);
		}

		// Token: 0x06006759 RID: 26457 RVA: 0x002F1EF3 File Offset: 0x002F00F3
		private void SetAge(CharacterDisplayDataForTooltip data)
		{
			this.age.gameObject.SetActive(true);
			this.age.SetValue(LocalStringManager.GetFormat(LanguageKey.LK_Age, data.Age));
		}

		// Token: 0x0600675A RID: 26458 RVA: 0x002F1F2C File Offset: 0x002F012C
		private void SetMainAttribute(CharacterDisplayDataForTooltip data)
		{
			for (int i = 0; i < 6; i++)
			{
				this.attributes[i].SetValue(data.MainAttributes[i].ToString());
			}
		}

		// Token: 0x0600675B RID: 26459 RVA: 0x002F1F69 File Offset: 0x002F0169
		private void SetSkillFrame(CharacterDisplayDataForTooltip data)
		{
			this.SetAnySkillFrame(data, true);
			this.SetAnySkillFrame(data, false);
		}

		// Token: 0x0600675C RID: 26460 RVA: 0x002F1F80 File Offset: 0x002F0180
		private void SetAnySkillFrame(CharacterDisplayDataForTooltip data, bool isLifeSkill)
		{
			ValueTuple<int, int, sbyte>[] render = this.IterQualifications(data, isLifeSkill).ToArray<ValueTuple<int, int, sbyte>>();
			(isLifeSkill ? this.lifeTitleGrowth : this.combatTitleGrowth).text = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + " " + CommonUtils.GetAgeGrowthString((int)(isLifeSkill ? data.LifeSkillQualificationGrowthType : data.CombatSkillQualificationGrowthType), (int)data.Age);
			bool flag = render.Length == 0;
			if (flag)
			{
				GameObject stupidGo = isLifeSkill ? this.lifeStupid : this.combatStupid;
				stupidGo.SetActive(true);
				TMP_Text stupidLabel = stupidGo.GetComponentInChildren<TMP_Text>(true);
				bool flag2 = stupidLabel != null;
				if (flag2)
				{
					stupidLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_RecruitPeopleData_Tips_Text_2, LocalStringManager.Get(isLifeSkill ? LanguageKey.LK_LifeSkill : LanguageKey.LK_Team_Tog_CombatSkill)).ColorReplace();
				}
			}
			else
			{
				(isLifeSkill ? this.lifeStupid : this.combatStupid).SetActive(false);
				(isLifeSkill ? this.lifeSkill : this.combatSkill).Rebuild<QualificationItem>(render.Length, delegate(QualificationItem item, int i)
				{
					ValueTuple<int, int, sbyte> valueTuple = render[i];
					int type = valueTuple.Item1;
					int value = valueTuple.Item2;
					sbyte grade = valueTuple.Item3;
					item.Set(string.Format("{0}{1}", isLifeSkill ? this.lifeIconNamePrefix : this.combatIconNamePrefix, type), LanguageKey.LK_Building_RejectRecruitPeople_Tips_Detail_Content.TrFormat(isLifeSkill ? Config.LifeSkillType.Instance[type].Name : CombatSkillType.Instance[type].Name, Colors.Instance.GradeColors[(int)grade].ColorToHexString(""), value));
				});
			}
		}

		// Token: 0x0600675D RID: 26461 RVA: 0x002F20CC File Offset: 0x002F02CC
		[return: TupleElementNames(new string[]
		{
			"type",
			"value",
			"grade"
		})]
		private unsafe IEnumerable<ValueTuple<int, int, sbyte>> IterQualifications(CharacterDisplayDataForTooltip data, bool isLifeSkill)
		{
			int typeCount = isLifeSkill ? 16 : 14;
			int num;
			for (int i = 0; i < typeCount; i = num + 1)
			{
				short value = isLifeSkill ? (*data.LifeSkillQualifications[i]) : (*data.CombatSkillQualifications[i]);
				sbyte grade = Grade.GetQualificationGrade((int)value);
				bool flag = grade < 3;
				if (!flag)
				{
					yield return new ValueTuple<int, int, sbyte>(i, (int)value, grade);
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x0600675F RID: 26463 RVA: 0x002F210C File Offset: 0x002F030C
		[CompilerGenerated]
		private void <Init>g__CallBack|16_0(int offset, RawDataPool pool)
		{
			CharacterDisplayDataForTooltip displayData = null;
			Serializer.Deserialize(pool, offset, ref displayData);
			this.OnDataReady(displayData);
		}

		// Token: 0x040048E9 RID: 18665
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040048EA RID: 18666
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x040048EB RID: 18667
		[SerializeField]
		private TMP_Text title;

		// Token: 0x040048EC RID: 18668
		[SerializeField]
		private Game.Components.Character.Gender gender;

		// Token: 0x040048ED RID: 18669
		[SerializeField]
		private Charm charm;

		// Token: 0x040048EE RID: 18670
		[SerializeField]
		private Game.Components.Character.BehaviorType behaviourType;

		// Token: 0x040048EF RID: 18671
		[SerializeField]
		private PropertyItem age;

		// Token: 0x040048F0 RID: 18672
		[SerializeField]
		private PropertyItem[] attributes;

		// Token: 0x040048F1 RID: 18673
		[SerializeField]
		private TemplatedContainerAssemblyNew lifeSkill;

		// Token: 0x040048F2 RID: 18674
		[SerializeField]
		private TemplatedContainerAssemblyNew combatSkill;

		// Token: 0x040048F3 RID: 18675
		[SerializeField]
		private TMP_Text lifeTitleGrowth;

		// Token: 0x040048F4 RID: 18676
		[SerializeField]
		private TMP_Text combatTitleGrowth;

		// Token: 0x040048F5 RID: 18677
		[SerializeField]
		private GameObject lifeStupid;

		// Token: 0x040048F6 RID: 18678
		[SerializeField]
		private GameObject combatStupid;

		// Token: 0x040048F7 RID: 18679
		[SerializeField]
		private string lifeIconNamePrefix = "ui9_icon_craftsmanship_small_1_";

		// Token: 0x040048F8 RID: 18680
		[SerializeField]
		private string combatIconNamePrefix = "ui9_icon_attainments_small_1_";
	}
}
