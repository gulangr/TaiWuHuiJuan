using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using UnityEngine;

namespace Game.Views.Achievement
{
	// Token: 0x02000C80 RID: 3200
	public class AchievementTypeToggleGroup : CToggleGroup
	{
		// Token: 0x0600A36C RID: 41836 RVA: 0x004C7CAC File Offset: 0x004C5EAC
		public void Init(Action onToggleChange)
		{
			this._onToggleChange = onToggleChange;
			base.Init(-1);
			base.OnActiveIndexChange += this.OnToggleChange;
			int count = 10;
			for (int i = 0; i < count; i++)
			{
				CToggleNameAndLineHelper obj = Object.Instantiate<CToggleNameAndLineHelper>(this.toggleTemplate, this.toggleParent);
				bool flag = i == count - 1;
				if (flag)
				{
					obj.SetLine(false);
				}
				base.Add(obj.toggle);
			}
			this.RefreshName();
		}

		// Token: 0x0600A36D RID: 41837 RVA: 0x004C7D2C File Offset: 0x004C5F2C
		public void RefreshName()
		{
			int count = 10;
			for (int i = 0; i < count; i++)
			{
				base.Get(i).GetComponent<CToggleNameAndLineHelper>().SetName(AchievementTypeToggleGroup._titles[(EAchievementInfoType)i].Tr());
			}
		}

		// Token: 0x0600A36E RID: 41838 RVA: 0x004C7D70 File Offset: 0x004C5F70
		public void SetIsNew(EAchievementInfoType type, bool value)
		{
			Transform toggle = base.Get((int)type).transform;
			toggle.GetChild(toggle.childCount - 1).gameObject.SetActive(value);
		}

		// Token: 0x0600A36F RID: 41839 RVA: 0x004C7DA8 File Offset: 0x004C5FA8
		private void OnToggleChange(int cur, int old)
		{
			bool flag = old >= 0 && old != 9;
			if (flag)
			{
				base.Get(old).GetComponent<CToggleNameAndLineHelper>().SetLine(true);
			}
			bool flag2 = cur >= 0;
			if (flag2)
			{
				base.Get(cur).GetComponent<CToggleNameAndLineHelper>().SetLine(false);
			}
			this._onToggleChange();
		}

		// Token: 0x04007F29 RID: 32553
		[SerializeField]
		private CToggleNameAndLineHelper toggleTemplate;

		// Token: 0x04007F2A RID: 32554
		[SerializeField]
		private Transform toggleParent;

		// Token: 0x04007F2B RID: 32555
		private static readonly Dictionary<EAchievementInfoType, LanguageKey> _titles = new Dictionary<EAchievementInfoType, LanguageKey>
		{
			{
				EAchievementInfoType.Taiwu,
				LanguageKey.LK_Achievement_Type_Taiwu
			},
			{
				EAchievementInfoType.Area,
				LanguageKey.LK_Achievement_Type_Area
			},
			{
				EAchievementInfoType.Character,
				LanguageKey.LK_Achievement_Type_Character
			},
			{
				EAchievementInfoType.Combat,
				LanguageKey.LK_Achievement_Type_Combat
			},
			{
				EAchievementInfoType.Profession,
				LanguageKey.LK_Achievement_Type_Profession
			},
			{
				EAchievementInfoType.LifeSkill,
				LanguageKey.LK_Achievement_Type_LifeSkill
			},
			{
				EAchievementInfoType.Building,
				LanguageKey.LK_Achievement_Type_Building
			},
			{
				EAchievementInfoType.CombatSkill,
				LanguageKey.LK_Achievement_Type_CombatSkill
			},
			{
				EAchievementInfoType.LegendBook,
				LanguageKey.LK_Achievement_Type_LegendBook
			},
			{
				EAchievementInfoType.Travel,
				LanguageKey.LK_Achievement_Type_Travel
			}
		};

		// Token: 0x04007F2C RID: 32556
		private Action _onToggleChange;
	}
}
