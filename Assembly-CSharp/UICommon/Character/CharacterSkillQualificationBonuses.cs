using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005E9 RID: 1513
	public class CharacterSkillQualificationBonuses : CharacterUIElement
	{
		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06004768 RID: 18280 RVA: 0x00216DB9 File Offset: 0x00214FB9
		protected SkillQualificationMonitor Item
		{
			get
			{
				return this.MonitorDataItem as SkillQualificationMonitor;
			}
		}

		// Token: 0x06004769 RID: 18281 RVA: 0x00216DC8 File Offset: 0x00214FC8
		public CharacterSkillQualificationBonuses(Refers[] array, sbyte ageLevel)
		{
			bool flag = array == null || array.Length == 0;
			if (flag)
			{
				throw new Exception("can not handle CharacterSkillQualificationBonuses for null elements");
			}
			this._refersArray = array;
			this._ageLevel = ageLevel;
		}

		// Token: 0x0600476A RID: 18282 RVA: 0x00216E08 File Offset: 0x00215008
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<SkillQualificationMonitor>(charId, this.IsDead);
		}

		// Token: 0x0600476B RID: 18283 RVA: 0x00216E2B File Offset: 0x0021502B
		internal override void BindEvent()
		{
			this.Item.AddQualificationBonusListener(new Action(this.FillElement));
		}

		// Token: 0x0600476C RID: 18284 RVA: 0x00216E47 File Offset: 0x00215047
		public override void UnbindEvent()
		{
			this.Item.RemoveQualificationBonusListener(new Action(this.FillElement));
		}

		// Token: 0x0600476D RID: 18285 RVA: 0x00216E64 File Offset: 0x00215064
		public override void FillElement()
		{
			for (int i = 0; i < this._refersArray.Length; i++)
			{
				this.FillCell(this._refersArray[i], i, false);
			}
			Action onFillComplete = this.OnFillComplete;
			if (onFillComplete != null)
			{
				onFillComplete();
			}
		}

		// Token: 0x0600476E RID: 18286 RVA: 0x00216EB0 File Offset: 0x002150B0
		public override void ResetToEmpty()
		{
			for (int i = 0; i < this._refersArray.Length; i++)
			{
				this.FillCell(this._refersArray[i], i, true);
			}
		}

		// Token: 0x0600476F RID: 18287 RVA: 0x00216EE8 File Offset: 0x002150E8
		private void FillCell(Refers refers, int index, bool isReset = false)
		{
			TooltipInvoker mouseTipDisPlayer = refers.CGet<TooltipInvoker>("MouseTips");
			if (isReset)
			{
				mouseTipDisPlayer.enabled = false;
				bool flag = index >= 2;
				if (flag)
				{
					refers.CGet<CImage>("SkillIcon").SetSprite("", false, null);
					refers.CGet<TextMeshProUGUI>("SkillEffect").text = string.Empty;
				}
			}
			else
			{
				mouseTipDisPlayer.enabled = true;
				bool flag2 = this.Item.BonusesList.CheckIndex(index);
				if (flag2)
				{
					SkillQualificationBonus bonus = this.Item.BonusesList[index];
					string skillIcon = string.Empty;
					string skillTypeName = string.Empty;
					string skillTypeDesc = string.Empty;
					string mouseTipsTitle2 = string.Empty;
					string mouseTipsContent2 = string.Empty;
					ValueTuple<sbyte, sbyte> skillGroupAndType = bonus.GetSkillGroupAndType();
					sbyte skillGroup = skillGroupAndType.Item1;
					sbyte skillType = skillGroupAndType.Item2;
					bool flag3 = skillGroup == 0;
					if (flag3)
					{
						LifeSkillTypeItem config = Config.LifeSkillType.Instance[skillType];
						skillTypeName = config.Name;
						skillTypeDesc = config.Desc;
						skillIcon = config.DisplayIcon;
					}
					else
					{
						bool flag4 = skillGroup == 1;
						if (flag4)
						{
							CombatSkillTypeItem config2 = CombatSkillType.Instance[skillType];
							skillTypeName = config2.Name;
							skillTypeDesc = config2.Desc;
							skillIcon = config2.DisplayIcon;
						}
					}
					bool flag5 = index < 2;
					if (flag5)
					{
						mouseTipsTitle2 = LocalStringManager.Get(LanguageKey.LK_Talent);
						mouseTipsContent2 = LocalStringManager.GetFormat(LanguageKey.LK_QulificationBonus, skillTypeName, bonus.Bonus) + "\n\n" + skillTypeDesc;
					}
					else
					{
						bool flag6 = skillGroup == 0;
						if (flag6)
						{
							Config.LifeSkillItem item = LifeSkill.Instance[bonus.SkillId];
							mouseTipsTitle2 = item.Name;
							mouseTipsContent2 = LocalStringManager.GetFormat(LanguageKey.LK_QulificationBonus, skillTypeName, bonus.Bonus) + "\n\n" + item.Desc;
						}
						else
						{
							bool flag7 = skillGroup == 1;
							if (flag7)
							{
								CombatSkillItem item2 = CombatSkill.Instance[bonus.SkillId];
								mouseTipsTitle2 = item2.Name;
								mouseTipsContent2 = LocalStringManager.GetFormat(LanguageKey.LK_QulificationBonus, skillTypeName, bonus.Bonus) + "\n\n" + item2.Desc;
								skillIcon = item2.Icon;
							}
						}
						refers.CGet<PointerTrigger>("PointerTrigger").EnterEvent.RemoveAllListeners();
					}
					refers.CGet<CImage>("SkillIcon").SetSprite(skillIcon, false, null);
					refers.CGet<TextMeshProUGUI>("SkillEffect").text = string.Format("{0} + {1}", skillTypeName, bonus.Bonus);
					Color color = Colors.Instance["white"];
					bool flag8 = bonus.Bonus >= 9;
					if (flag8)
					{
						color = Colors.Instance["lightblue"];
					}
					bool flag9 = bonus.Bonus >= 12;
					if (flag9)
					{
						color = Colors.Instance["orange"];
					}
					refers.CGet<TextMeshProUGUI>("SkillEffect").color = color;
					mouseTipDisPlayer.PresetParam = new string[]
					{
						mouseTipsTitle2,
						mouseTipsContent2
					};
				}
				else
				{
					string skillIcon2 = string.Empty;
					string mouseTipsTitle = string.Empty;
					string mouseTipsContent = string.Empty;
					PointerTrigger trigger = refers.CGet<PointerTrigger>("PointerTrigger");
					bool flag10 = index <= (int)this._ageLevel;
					if (flag10)
					{
						skillIcon2 = "LifeSkillIcon_16";
						mouseTipsTitle = LocalStringManager.Get(LanguageKey.LK_TaiwuShrine_NotToughtTips_Title);
						trigger.EnterEvent.RemoveAllListeners();
						trigger.EnterEvent.AddListener(delegate()
						{
							Func<string> getMouseTipContent = this.GetMouseTipContent;
							mouseTipsContent = ((getMouseTipContent != null) ? getMouseTipContent() : null);
							mouseTipDisPlayer.PresetParam = new string[]
							{
								mouseTipsTitle,
								mouseTipsContent
							};
						});
					}
					else
					{
						mouseTipsTitle = LocalStringManager.Get(LanguageKey.LK_TaiwuShrine_CanNotTeachTips_Title);
						mouseTipsContent = LocalStringManager.GetFormat(LanguageKey.LK_TaiwuShrine_CanNotTeachTips_Content, index * 10);
						mouseTipDisPlayer.PresetParam = new string[]
						{
							mouseTipsTitle,
							mouseTipsContent
						};
					}
					refers.CGet<CImage>("SkillIcon").SetSprite(skillIcon2, false, null);
					refers.CGet<TextMeshProUGUI>("SkillEffect").text = string.Empty;
				}
			}
		}

		// Token: 0x06004770 RID: 18288 RVA: 0x00217338 File Offset: 0x00215538
		public List<SkillQualificationBonus> GetSkillQualificationBonus()
		{
			return this.Item.BonusesList;
		}

		// Token: 0x04003146 RID: 12614
		public Action OnFillComplete;

		// Token: 0x04003147 RID: 12615
		public Func<string> GetMouseTipContent;

		// Token: 0x04003148 RID: 12616
		private readonly Refers[] _refersArray;

		// Token: 0x04003149 RID: 12617
		private readonly sbyte _ageLevel;
	}
}
