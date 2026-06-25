using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F0D RID: 3853
	public class Attribute : MonoBehaviour
	{
		// Token: 0x17001417 RID: 5143
		// (get) Token: 0x0600B16F RID: 45423 RVA: 0x0050D4B2 File Offset: 0x0050B6B2
		public PropertyItem[] PoisonResistItem
		{
			get
			{
				return this.poisonResistItem;
			}
		}

		// Token: 0x17001418 RID: 5144
		// (get) Token: 0x0600B170 RID: 45424 RVA: 0x0050D4BA File Offset: 0x0050B6BA
		public PropertyItem PenetrationOuter
		{
			get
			{
				return this.penetrationOuter;
			}
		}

		// Token: 0x17001419 RID: 5145
		// (get) Token: 0x0600B171 RID: 45425 RVA: 0x0050D4C2 File Offset: 0x0050B6C2
		public PropertyItem PenetrationInner
		{
			get
			{
				return this.penetrationInner;
			}
		}

		// Token: 0x1700141A RID: 5146
		// (get) Token: 0x0600B172 RID: 45426 RVA: 0x0050D4CA File Offset: 0x0050B6CA
		public PropertyItem PenetrationResistOuter
		{
			get
			{
				return this.penetrationResistOuter;
			}
		}

		// Token: 0x1700141B RID: 5147
		// (get) Token: 0x0600B173 RID: 45427 RVA: 0x0050D4D2 File Offset: 0x0050B6D2
		public PropertyItem PenetrationResistInner
		{
			get
			{
				return this.penetrationResistInner;
			}
		}

		// Token: 0x1700141C RID: 5148
		// (get) Token: 0x0600B174 RID: 45428 RVA: 0x0050D4DA File Offset: 0x0050B6DA
		public PropertyItem[] HitValueItems
		{
			get
			{
				return this.hitValueItems;
			}
		}

		// Token: 0x1700141D RID: 5149
		// (get) Token: 0x0600B175 RID: 45429 RVA: 0x0050D4E2 File Offset: 0x0050B6E2
		public PropertyItem[] AvoidValueItems
		{
			get
			{
				return this.avoidValueItems;
			}
		}

		// Token: 0x1700141E RID: 5150
		// (get) Token: 0x0600B176 RID: 45430 RVA: 0x0050D4EA File Offset: 0x0050B6EA
		public PropertyItem RecoveryOfStance
		{
			get
			{
				return this.recoveryOfStance;
			}
		}

		// Token: 0x1700141F RID: 5151
		// (get) Token: 0x0600B177 RID: 45431 RVA: 0x0050D4F2 File Offset: 0x0050B6F2
		public PropertyItem RecoveryOfBreath
		{
			get
			{
				return this.recoveryOfBreath;
			}
		}

		// Token: 0x17001420 RID: 5152
		// (get) Token: 0x0600B178 RID: 45432 RVA: 0x0050D4FA File Offset: 0x0050B6FA
		public PropertyItem AttackSpeed
		{
			get
			{
				return this.attackSpeed;
			}
		}

		// Token: 0x17001421 RID: 5153
		// (get) Token: 0x0600B179 RID: 45433 RVA: 0x0050D502 File Offset: 0x0050B702
		public PropertyItem MoveSpeed
		{
			get
			{
				return this.moveSpeed;
			}
		}

		// Token: 0x17001422 RID: 5154
		// (get) Token: 0x0600B17A RID: 45434 RVA: 0x0050D50A File Offset: 0x0050B70A
		public PropertyItem WeaponSwitchSpeed
		{
			get
			{
				return this.weaponSwitchSpeed;
			}
		}

		// Token: 0x17001423 RID: 5155
		// (get) Token: 0x0600B17B RID: 45435 RVA: 0x0050D512 File Offset: 0x0050B712
		public PropertyItem CastSpeed
		{
			get
			{
				return this.castSpeed;
			}
		}

		// Token: 0x17001424 RID: 5156
		// (get) Token: 0x0600B17C RID: 45436 RVA: 0x0050D51A File Offset: 0x0050B71A
		public PropertyItem RecoveryOfFlaw
		{
			get
			{
				return this.recoveryOfFlaw;
			}
		}

		// Token: 0x17001425 RID: 5157
		// (get) Token: 0x0600B17D RID: 45437 RVA: 0x0050D522 File Offset: 0x0050B722
		public PropertyItem RecoveryOfBlockedAcupoint
		{
			get
			{
				return this.recoveryOfBlockedAcupoint;
			}
		}

		// Token: 0x17001426 RID: 5158
		// (get) Token: 0x0600B17E RID: 45438 RVA: 0x0050D52A File Offset: 0x0050B72A
		public PropertyItem RecoveryOfQiDisorder
		{
			get
			{
				return this.recoveryOfQiDisorder;
			}
		}

		// Token: 0x17001427 RID: 5159
		// (get) Token: 0x0600B17F RID: 45439 RVA: 0x0050D532 File Offset: 0x0050B732
		public PropertyItem InnerRatio
		{
			get
			{
				return this.innerRatio;
			}
		}

		// Token: 0x17001428 RID: 5160
		// (get) Token: 0x0600B180 RID: 45440 RVA: 0x0050D53A File Offset: 0x0050B73A
		public CharacterAttributeDisplayData CurrentData
		{
			get
			{
				return this._currentData;
			}
		}

		// Token: 0x0600B181 RID: 45441 RVA: 0x0050D542 File Offset: 0x0050B742
		private void Awake()
		{
			this.InitTips();
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		}

		// Token: 0x0600B182 RID: 45442 RVA: 0x0050D568 File Offset: 0x0050B768
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		}

		// Token: 0x0600B183 RID: 45443 RVA: 0x0050D588 File Offset: 0x0050B788
		private void OnLanguageChange(ArgumentBox _)
		{
			this.InitTips();
			bool flag = this._currentData != null;
			if (flag)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600B184 RID: 45444 RVA: 0x0050D5B4 File Offset: 0x0050B7B4
		private void InitTips()
		{
			PropertyItem propertyItem = this.penetrationOuter;
			if (propertyItem != null)
			{
				propertyItem.SetTip(10);
			}
			PropertyItem propertyItem2 = this.penetrationInner;
			if (propertyItem2 != null)
			{
				propertyItem2.SetTip(11);
			}
			PropertyItem propertyItem3 = this.penetrationResistOuter;
			if (propertyItem3 != null)
			{
				propertyItem3.SetTip(16);
			}
			PropertyItem propertyItem4 = this.penetrationResistInner;
			if (propertyItem4 != null)
			{
				propertyItem4.SetTip(17);
			}
			bool flag = this.hitValueItems != null;
			if (flag)
			{
				int i = 0;
				while (i < this.hitValueItems.Length && i < 4)
				{
					PropertyItem propertyItem5 = this.hitValueItems[i];
					if (propertyItem5 != null)
					{
						propertyItem5.SetTip(6 + i);
					}
					i++;
				}
			}
			bool flag2 = this.avoidValueItems != null;
			if (flag2)
			{
				int j = 0;
				while (j < this.avoidValueItems.Length && j < 4)
				{
					PropertyItem propertyItem6 = this.avoidValueItems[j];
					if (propertyItem6 != null)
					{
						propertyItem6.SetTip(12 + j);
					}
					j++;
				}
			}
			PropertyItem propertyItem7 = this.recoveryOfStance;
			if (propertyItem7 != null)
			{
				propertyItem7.SetTip(18);
			}
			PropertyItem propertyItem8 = this.recoveryOfBreath;
			if (propertyItem8 != null)
			{
				propertyItem8.SetTip(19);
			}
			PropertyItem propertyItem9 = this.attackSpeed;
			if (propertyItem9 != null)
			{
				propertyItem9.SetTip(25);
			}
			PropertyItem propertyItem10 = this.moveSpeed;
			if (propertyItem10 != null)
			{
				propertyItem10.SetTip(20);
			}
			PropertyItem propertyItem11 = this.weaponSwitchSpeed;
			if (propertyItem11 != null)
			{
				propertyItem11.SetTip(24);
			}
			PropertyItem propertyItem12 = this.castSpeed;
			if (propertyItem12 != null)
			{
				propertyItem12.SetTip(22);
			}
			PropertyItem propertyItem13 = this.recoveryOfFlaw;
			if (propertyItem13 != null)
			{
				propertyItem13.SetTip(21);
			}
			PropertyItem propertyItem14 = this.recoveryOfBlockedAcupoint;
			if (propertyItem14 != null)
			{
				propertyItem14.SetTip(23);
			}
			PropertyItem propertyItem15 = this.recoveryOfQiDisorder;
			if (propertyItem15 != null)
			{
				propertyItem15.SetTip(27);
			}
			PropertyItem propertyItem16 = this.innerRatio;
			if (propertyItem16 != null)
			{
				propertyItem16.SetTip(26);
			}
			bool flag3 = this.poisonResistItem != null;
			if (flag3)
			{
				int k = 0;
				while (k < this.poisonResistItem.Length && k < 6)
				{
					PropertyItem propertyItem17 = this.poisonResistItem[k];
					if (propertyItem17 != null)
					{
						propertyItem17.SetTip(28 + k);
					}
					k++;
				}
			}
		}

		// Token: 0x0600B185 RID: 45445 RVA: 0x0050D7C4 File Offset: 0x0050B9C4
		public void Set(CharacterAttributeDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this._currentData = data;
				bool flag2 = !this.IsDisplayingAllocationBonus;
				if (flag2)
				{
					this.Refresh();
				}
				else
				{
					this.RefreshAllocationBonus();
				}
			}
		}

		// Token: 0x0600B186 RID: 45446 RVA: 0x0050D810 File Offset: 0x0050BA10
		public void Refresh()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(96);
			this.RefreshMainAttributes();
			this.RefreshPoisonResists();
			this.RefreshMainAttributeRecoveries();
			this.RefreshPenetrations();
			this.RefreshPenetrationResists();
			this.RefreshHitValues();
			this.RefreshAvoidValues();
			this.RefreshSecondaryAttributes();
		}

		// Token: 0x0600B187 RID: 45447 RVA: 0x0050D860 File Offset: 0x0050BA60
		public void SetEmpty()
		{
			this._currentData = null;
			bool flag = this.mainAttributeItems != null;
			if (flag)
			{
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
				while (i < this.mainAttributeItems.Length && i < displayOrder.Length)
				{
					bool flag2 = this.mainAttributeItems[i] == null;
					if (!flag2)
					{
						int attrIndex = displayOrder[i];
						string attrName = Attribute.GetMainAttributeName((sbyte)attrIndex);
						string iconName = Attribute.GetMainAttributeIcon((sbyte)attrIndex);
						this.mainAttributeItems[i].Set(iconName, attrName, "-", null, false);
					}
					i++;
				}
			}
			bool flag3 = this.mainAttributeRecoveries != null;
			if (flag3)
			{
				foreach (ValuePerMonth recovery in this.mainAttributeRecoveries)
				{
					bool flag4 = recovery == null;
					if (!flag4)
					{
						recovery.gameObject.SetActive(false);
					}
				}
			}
			PropertyItem propertyItem = this.penetrationOuter;
			if (propertyItem != null)
			{
				propertyItem.Set(Attribute.GetPenetrationIcon(true), Attribute.GetPenetrationName(true), "-", null, false);
			}
			PropertyItem propertyItem2 = this.penetrationInner;
			if (propertyItem2 != null)
			{
				propertyItem2.Set(Attribute.GetPenetrationIcon(false), Attribute.GetPenetrationName(false), "-", null, false);
			}
			PropertyItem propertyItem3 = this.penetrationResistOuter;
			if (propertyItem3 != null)
			{
				propertyItem3.Set(Attribute.GetPenetrationResistIcon(true), Attribute.GetPenetrationResistName(true), "-", null, false);
			}
			PropertyItem propertyItem4 = this.penetrationResistInner;
			if (propertyItem4 != null)
			{
				propertyItem4.Set(Attribute.GetPenetrationResistIcon(false), Attribute.GetPenetrationResistName(false), "-", null, false);
			}
			bool flag5 = this.hitValueItems != null;
			if (flag5)
			{
				sbyte j = 0;
				while ((int)j < this.hitValueItems.Length && j < 4)
				{
					bool flag6 = this.hitValueItems[(int)j] == null;
					if (!flag6)
					{
						this.hitValueItems[(int)j].Set(Attribute.GetHitValueIcon(j), Attribute.GetHitValueName(j), "-", null, false);
					}
					j += 1;
				}
			}
			bool flag7 = this.avoidValueItems != null;
			if (flag7)
			{
				sbyte k = 0;
				while ((int)k < this.avoidValueItems.Length && k < 4)
				{
					bool flag8 = this.avoidValueItems[(int)k] == null;
					if (!flag8)
					{
						this.avoidValueItems[(int)k].Set(Attribute.GetAvoidValueIcon(k), Attribute.GetAvoidValueName(k), "-", null, false);
					}
					k += 1;
				}
			}
			PropertyItem propertyItem5 = this.recoveryOfStance;
			if (propertyItem5 != null)
			{
				propertyItem5.Set(Attribute.GetMinorAttributeIcon("0"), LocalStringManager.Get(LanguageKey.LK_SpeedOf_Stance), "-", null, false);
			}
			PropertyItem propertyItem6 = this.recoveryOfBreath;
			if (propertyItem6 != null)
			{
				propertyItem6.Set(Attribute.GetMinorAttributeIcon("1"), LocalStringManager.Get(LanguageKey.LK_SpeedOf_Breath), "-", null, false);
			}
			PropertyItem propertyItem7 = this.attackSpeed;
			if (propertyItem7 != null)
			{
				propertyItem7.Set(Attribute.GetMinorAttributeIcon("6"), LocalStringManager.Get(LanguageKey.LK_AttackSpeed), "-", null, false);
			}
			PropertyItem propertyItem8 = this.moveSpeed;
			if (propertyItem8 != null)
			{
				propertyItem8.Set(Attribute.GetMinorAttributeIcon("2"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfMobility), "-", null, false);
			}
			PropertyItem propertyItem9 = this.weaponSwitchSpeed;
			if (propertyItem9 != null)
			{
				propertyItem9.Set(Attribute.GetMinorAttributeIcon("7"), LocalStringManager.Get(LanguageKey.LK_WeaponSwitchSpeed), "-", null, false);
			}
			PropertyItem propertyItem10 = this.castSpeed;
			if (propertyItem10 != null)
			{
				propertyItem10.Set(Attribute.GetMinorAttributeIcon("4"), LocalStringManager.Get(LanguageKey.LK_CastSpeed), "-", null, false);
			}
			PropertyItem propertyItem11 = this.recoveryOfFlaw;
			if (propertyItem11 != null)
			{
				propertyItem11.Set(Attribute.GetMinorAttributeIcon("3"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfFlaw), "-", null, false);
			}
			PropertyItem propertyItem12 = this.recoveryOfBlockedAcupoint;
			if (propertyItem12 != null)
			{
				propertyItem12.Set(Attribute.GetMinorAttributeIcon("5"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfBlockedAcupoint), "-", null, false);
			}
			PropertyItem propertyItem13 = this.recoveryOfQiDisorder;
			if (propertyItem13 != null)
			{
				propertyItem13.Set(Attribute.GetMinorAttributeIcon("9"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfQiDisorder), "-", null, false);
			}
			PropertyItem propertyItem14 = this.innerRatio;
			if (propertyItem14 != null)
			{
				propertyItem14.Set(Attribute.GetMinorAttributeIcon("8"), LocalStringManager.Get(LanguageKey.LK_InnerRatio), "-", null, false);
			}
		}

		// Token: 0x0600B188 RID: 45448 RVA: 0x0050DD2D File Offset: 0x0050BF2D
		public void SetDisplayPoisonResist(bool value)
		{
			this.mainAttributeObj.SetActive(!value);
			this.poisonResistObj.SetActive(value);
		}

		// Token: 0x0600B189 RID: 45449 RVA: 0x0050DD4D File Offset: 0x0050BF4D
		private void RefreshMainAttributes()
		{
			this.RefreshMainAttributes(this._currentData.CurMainAttributes, this._currentData.MaxMainAttributes);
			this.RefreshMainAttributeTips();
		}

		// Token: 0x0600B18A RID: 45450 RVA: 0x0050DD74 File Offset: 0x0050BF74
		public unsafe void RefreshMainAttributes(MainAttributes curMainAttributes, MainAttributes maxMainAttributes)
		{
			bool flag = this.mainAttributeItems == null;
			if (!flag)
			{
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
				while (i < this.mainAttributeItems.Length && i < displayOrder.Length)
				{
					bool flag2 = this.mainAttributeItems[i] == null;
					if (!flag2)
					{
						int attrIndex = displayOrder[i];
						short curValue = *curMainAttributes[attrIndex];
						short maxValue = *maxMainAttributes[attrIndex];
						string valueStr = string.Format("{0}/{1}", curValue, maxValue);
						string attrName = Attribute.GetMainAttributeName((sbyte)attrIndex);
						string iconName = Attribute.GetMainAttributeIcon((sbyte)attrIndex);
						this.mainAttributeItems[i].Set(iconName, attrName, valueStr, null, false);
					}
					i++;
				}
			}
		}

		// Token: 0x0600B18B RID: 45451 RVA: 0x0050DE4C File Offset: 0x0050C04C
		private void RefreshMainAttributeTips()
		{
			bool flag = this._currentData == null;
			if (!flag)
			{
				this.RefreshMainAttributeTips(this._currentData.MainAttributeRecoveries);
			}
		}

		// Token: 0x0600B18C RID: 45452 RVA: 0x0050DE7C File Offset: 0x0050C07C
		public unsafe void RefreshMainAttributeTips(MainAttributes mainAttributeRecoveries)
		{
			bool flag = this.mainAttributeItems == null;
			if (!flag)
			{
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
				while (i < this.mainAttributeItems.Length && i < displayOrder.Length)
				{
					bool flag2 = this.mainAttributeItems[i] == null;
					if (!flag2)
					{
						int attrIndex = displayOrder[i];
						int templateId = attrIndex;
						CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[templateId];
						bool flag3 = config == null;
						if (!flag3)
						{
							short recovery = *mainAttributeRecoveries[attrIndex];
							string recoveryLine = LocalStringManager.GetFormat(LanguageKey.LK_CharacterMenu_MajorAttribute_Recovery_Tip_1, recovery);
							string recoveryLine2 = LocalStringManager.Get(LanguageKey.LK_CharacterMenu_MajorAttribute_Recovery_Tip_2);
							string tipDesc = string.Concat(new string[]
							{
								config.Desc,
								"\n\n",
								recoveryLine,
								"\n",
								recoveryLine2
							});
							this.mainAttributeItems[i].SetTip(config.Name, tipDesc);
						}
					}
					i++;
				}
			}
		}

		// Token: 0x0600B18D RID: 45453 RVA: 0x0050DF85 File Offset: 0x0050C185
		private void RefreshPoisonResists()
		{
			this.RefreshPoisonResists(this._currentData.PoisonResists);
		}

		// Token: 0x0600B18E RID: 45454 RVA: 0x0050DF9C File Offset: 0x0050C19C
		public void RefreshPoisonResists(PoisonInts poison)
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				string iconName = Attribute.GetPoisonResistIcon(i);
				string attrName = Attribute.GetPoisonResistName(i);
				this.poisonResistItem[(int)i].Set(iconName, attrName, poison[(int)i].ToString(), null, false);
			}
		}

		// Token: 0x0600B18F RID: 45455 RVA: 0x0050DFF4 File Offset: 0x0050C1F4
		private void RefreshMainAttributeRecoveries()
		{
			this.RefreshMainAttributeRecoveries(this._currentData.MainAttributeRecoveries);
		}

		// Token: 0x0600B190 RID: 45456 RVA: 0x0050E008 File Offset: 0x0050C208
		public unsafe void RefreshMainAttributeRecoveries(MainAttributes mainAttributeRecoveries)
		{
			bool flag = this.mainAttributeRecoveries == null;
			if (!flag)
			{
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
				while (i < this.mainAttributeRecoveries.Length && i < displayOrder.Length)
				{
					bool flag2 = this.mainAttributeRecoveries[i] == null;
					if (!flag2)
					{
						this.mainAttributeRecoveries[i].gameObject.SetActive(true);
						int attrIndex = displayOrder[i];
						short recovery = *mainAttributeRecoveries[attrIndex];
						this.mainAttributeRecoveries[i].Set((int)recovery);
					}
					i++;
				}
			}
		}

		// Token: 0x0600B191 RID: 45457 RVA: 0x0050E0A4 File Offset: 0x0050C2A4
		private void RefreshPenetrations()
		{
			bool flag = this._currentData == null;
			if (!flag)
			{
				PropertyItem propertyItem = this.penetrationOuter;
				if (propertyItem != null)
				{
					propertyItem.Set(Attribute.GetPenetrationIcon(true), Attribute.GetPenetrationName(true), this._currentData.AtkPenetrability.Outer.ToString(), null, false);
				}
				PropertyItem propertyItem2 = this.penetrationInner;
				if (propertyItem2 != null)
				{
					propertyItem2.Set(Attribute.GetPenetrationIcon(false), Attribute.GetPenetrationName(false), this._currentData.AtkPenetrability.Inner.ToString(), null, false);
				}
				int delta = (int)(this._currentData.CanAffectedByCombatDifficulty ? (CombatDifficulty.Instance.GetItem(SingletonObject.getInstance<BasicGameData>().CombatDifficulty).Penetrations - 100) : 0);
				PropertyItem propertyItem3 = this.penetrationTotal;
				if (propertyItem3 != null)
				{
					propertyItem3.SetDelta(delta, LanguageKey.LK_Combat_Attack_Value, true);
				}
				PropertyItem propertyItem4 = this.penetrationOuter;
				if (propertyItem4 != null)
				{
					propertyItem4.SetDelta(delta, LanguageKey.LK_Combat_Attack_Value, false);
				}
				PropertyItem propertyItem5 = this.penetrationInner;
				if (propertyItem5 != null)
				{
					propertyItem5.SetDelta(delta, LanguageKey.LK_Combat_Attack_Value, false);
				}
			}
		}

		// Token: 0x0600B192 RID: 45458 RVA: 0x0050E1B8 File Offset: 0x0050C3B8
		private void RefreshPenetrationResists()
		{
			bool flag = this._currentData == null;
			if (!flag)
			{
				PropertyItem propertyItem = this.penetrationResistOuter;
				if (propertyItem != null)
				{
					propertyItem.Set(Attribute.GetPenetrationResistIcon(true), Attribute.GetPenetrationResistName(true), this._currentData.DefPenetrability.Outer.ToString(), null, false);
				}
				PropertyItem propertyItem2 = this.penetrationResistInner;
				if (propertyItem2 != null)
				{
					propertyItem2.Set(Attribute.GetPenetrationResistIcon(false), Attribute.GetPenetrationResistName(false), this._currentData.DefPenetrability.Inner.ToString(), null, false);
				}
				int delta = (int)(this._currentData.CanAffectedByCombatDifficulty ? (CombatDifficulty.Instance.GetItem(SingletonObject.getInstance<BasicGameData>().CombatDifficulty).PenetrationResists - 100) : 0);
				PropertyItem propertyItem3 = this.penetrationResistTotal;
				if (propertyItem3 != null)
				{
					propertyItem3.SetDelta(delta, LanguageKey.LK_Combat_Defend_Value, true);
				}
				PropertyItem propertyItem4 = this.penetrationResistOuter;
				if (propertyItem4 != null)
				{
					propertyItem4.SetDelta(delta, LanguageKey.LK_Combat_Defend_Value, false);
				}
				PropertyItem propertyItem5 = this.penetrationResistInner;
				if (propertyItem5 != null)
				{
					propertyItem5.SetDelta(delta, LanguageKey.LK_Combat_Defend_Value, false);
				}
			}
		}

		// Token: 0x0600B193 RID: 45459 RVA: 0x0050E2CC File Offset: 0x0050C4CC
		private void RefreshHitValues()
		{
			bool flag = this.hitValueItems == null || this._currentData == null;
			if (!flag)
			{
				int delta = (int)(this._currentData.CanAffectedByCombatDifficulty ? (CombatDifficulty.Instance.GetItem(SingletonObject.getInstance<BasicGameData>().CombatDifficulty).HitValues - 100) : 0);
				PropertyItem propertyItem = this.hitTotal;
				if (propertyItem != null)
				{
					propertyItem.SetDelta(delta, LanguageKey.LK_WeaponHitRate, true);
				}
				sbyte i = 0;
				while ((int)i < this.hitValueItems.Length && i < 4)
				{
					bool flag2 = this.hitValueItems[(int)i] == null;
					if (!flag2)
					{
						this.hitValueItems[(int)i].Set(Attribute.GetHitValueIcon(i), Attribute.GetHitValueName(i), this._currentData.AtkHitAttribute[(int)i].ToString(), null, false);
						PropertyItem propertyItem2 = this.hitValueItems[(int)i];
						if (propertyItem2 != null)
						{
							propertyItem2.SetDelta(delta, LanguageKey.LK_WeaponHitRate, false);
						}
					}
					i += 1;
				}
			}
		}

		// Token: 0x0600B194 RID: 45460 RVA: 0x0050E3D4 File Offset: 0x0050C5D4
		private void RefreshAvoidValues()
		{
			bool flag = this.avoidValueItems == null || this._currentData == null;
			if (!flag)
			{
				int delta = (int)(this._currentData.CanAffectedByCombatDifficulty ? (CombatDifficulty.Instance.GetItem(SingletonObject.getInstance<BasicGameData>().CombatDifficulty).AvoidValues - 100) : 0);
				PropertyItem propertyItem = this.avoidTotal;
				if (propertyItem != null)
				{
					propertyItem.SetDelta(delta, LanguageKey.LK_CombatAttribute_Avoid, true);
				}
				sbyte i = 0;
				while ((int)i < this.avoidValueItems.Length && i < 4)
				{
					bool flag2 = this.avoidValueItems[(int)i] == null;
					if (!flag2)
					{
						this.avoidValueItems[(int)i].Set(Attribute.GetAvoidValueIcon(i), Attribute.GetAvoidValueName(i), this._currentData.DefHitAttribute[(int)i].ToString(), null, false);
						PropertyItem propertyItem2 = this.avoidValueItems[(int)i];
						if (propertyItem2 != null)
						{
							propertyItem2.SetDelta(delta, LanguageKey.LK_CombatAttribute_Avoid, false);
						}
					}
					i += 1;
				}
			}
		}

		// Token: 0x0600B195 RID: 45461 RVA: 0x0050E4DC File Offset: 0x0050C6DC
		private void RefreshSecondaryAttributes()
		{
			bool flag = this._currentData == null;
			if (!flag)
			{
				CombatDifficultyItem config = this._currentData.CanAffectedByCombatDifficulty ? CombatDifficulty.Instance.GetItem(SingletonObject.getInstance<BasicGameData>().CombatDifficulty) : null;
				int valueOfRecoveryOfStance = (int)(((config != null) ? config.RecoveryOfStanceAndBreath.Outer : 100) - 100);
				int valueOfRecoveryOfBreath = (int)(((config != null) ? config.RecoveryOfStanceAndBreath.Inner : 100) - 100);
				int valueOfMoveSpeed = (int)(((config != null) ? config.MoveSpeed : 100) - 100);
				int valueOfRecoveryOfFlaw = (int)(((config != null) ? config.RecoveryOfFlaw : 100) - 100);
				int valueOfCastSpeed = (int)(((config != null) ? config.CastSpeed : 100) - 100);
				int valueOfRecoveryOfBlockedAcupoint = (int)(((config != null) ? config.RecoveryOfBlockedAcupoint : 100) - 100);
				int valueOfWeaponSwitchSpeed = (int)(((config != null) ? config.WeaponSwitchSpeed : 100) - 100);
				int valueOfAttackSpeed = (int)(((config != null) ? config.AttackSpeed : 100) - 100);
				int valueOfInnerRatio = (int)(((config != null) ? config.InnerRatio : 100) - 100);
				int valueOfRecoveryOfQiDisorder = (int)(((config != null) ? config.RecoveryOfQiDisorder : 100) - 100);
				PropertyItem propertyItem = this.recoveryTotal;
				if (propertyItem != null)
				{
					ECharacterPropertyDisplayType[] array = new ECharacterPropertyDisplayType[10];
					RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.9130ECB13B342EE97778A9F1342D6562CF9F25D930398F59EE32732715FA1DFB).FieldHandle);
					propertyItem.SetDelta(array, new int[]
					{
						valueOfRecoveryOfStance,
						valueOfRecoveryOfBreath,
						valueOfAttackSpeed,
						valueOfMoveSpeed,
						valueOfWeaponSwitchSpeed,
						valueOfCastSpeed,
						valueOfRecoveryOfFlaw,
						valueOfRecoveryOfBlockedAcupoint,
						valueOfRecoveryOfQiDisorder,
						valueOfInnerRatio
					}, true);
				}
				PropertyItem propertyItem2 = this.recoveryOfStance;
				if (propertyItem2 != null)
				{
					propertyItem2.Set(Attribute.GetMinorAttributeIcon("0"), LocalStringManager.Get(LanguageKey.LK_SpeedOf_Stance), string.Format("{0}%", this._currentData.RecoveryOfStanceAndBreath.Outer), null, false);
				}
				PropertyItem propertyItem3 = this.recoveryOfStance;
				if (propertyItem3 != null)
				{
					propertyItem3.SetDelta(ECharacterPropertyDisplayType.RecoveryOfStance, valueOfRecoveryOfStance, false);
				}
				PropertyItem propertyItem4 = this.recoveryOfBreath;
				if (propertyItem4 != null)
				{
					propertyItem4.Set(Attribute.GetMinorAttributeIcon("1"), LocalStringManager.Get(LanguageKey.LK_SpeedOf_Breath), string.Format("{0}%", this._currentData.RecoveryOfStanceAndBreath.Inner), null, false);
				}
				PropertyItem propertyItem5 = this.recoveryOfBreath;
				if (propertyItem5 != null)
				{
					propertyItem5.SetDelta(ECharacterPropertyDisplayType.RecoveryOfBreath, valueOfRecoveryOfBreath, false);
				}
				PropertyItem propertyItem6 = this.attackSpeed;
				if (propertyItem6 != null)
				{
					propertyItem6.Set(Attribute.GetMinorAttributeIcon("6"), LocalStringManager.Get(LanguageKey.LK_AttackSpeed), string.Format("{0}%", this._currentData.AttackSpeed), null, false);
				}
				PropertyItem propertyItem7 = this.attackSpeed;
				if (propertyItem7 != null)
				{
					propertyItem7.SetDelta(ECharacterPropertyDisplayType.AttackSpeed, valueOfAttackSpeed, false);
				}
				PropertyItem propertyItem8 = this.moveSpeed;
				if (propertyItem8 != null)
				{
					propertyItem8.Set(Attribute.GetMinorAttributeIcon("2"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfMobility), string.Format("{0}%", this._currentData.MoveSpeed), null, false);
				}
				PropertyItem propertyItem9 = this.moveSpeed;
				if (propertyItem9 != null)
				{
					propertyItem9.SetDelta(ECharacterPropertyDisplayType.MoveSpeed, valueOfMoveSpeed, false);
				}
				PropertyItem propertyItem10 = this.weaponSwitchSpeed;
				if (propertyItem10 != null)
				{
					propertyItem10.Set(Attribute.GetMinorAttributeIcon("7"), LocalStringManager.Get(LanguageKey.LK_WeaponSwitchSpeed), string.Format("{0}%", this._currentData.WeaponSwitchSpeed), null, false);
				}
				PropertyItem propertyItem11 = this.weaponSwitchSpeed;
				if (propertyItem11 != null)
				{
					propertyItem11.SetDelta(ECharacterPropertyDisplayType.WeaponSwitchSpeed, valueOfWeaponSwitchSpeed, false);
				}
				PropertyItem propertyItem12 = this.castSpeed;
				if (propertyItem12 != null)
				{
					propertyItem12.Set(Attribute.GetMinorAttributeIcon("4"), LocalStringManager.Get(LanguageKey.LK_CastSpeed), string.Format("{0}%", this._currentData.CastSpeed), null, false);
				}
				PropertyItem propertyItem13 = this.castSpeed;
				if (propertyItem13 != null)
				{
					propertyItem13.SetDelta(ECharacterPropertyDisplayType.CastSpeed, valueOfCastSpeed, false);
				}
				PropertyItem propertyItem14 = this.recoveryOfFlaw;
				if (propertyItem14 != null)
				{
					propertyItem14.Set(Attribute.GetMinorAttributeIcon("3"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfFlaw), string.Format("{0}%", this._currentData.RecoveryOfFlaw), null, false);
				}
				PropertyItem propertyItem15 = this.recoveryOfFlaw;
				if (propertyItem15 != null)
				{
					propertyItem15.SetDelta(ECharacterPropertyDisplayType.RecoveryOfFlaw, valueOfRecoveryOfFlaw, false);
				}
				PropertyItem propertyItem16 = this.recoveryOfBlockedAcupoint;
				if (propertyItem16 != null)
				{
					propertyItem16.Set(Attribute.GetMinorAttributeIcon("5"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfBlockedAcupoint), string.Format("{0}%", this._currentData.RecoveryOfBlockedAcupoint), null, false);
				}
				PropertyItem propertyItem17 = this.recoveryOfBlockedAcupoint;
				if (propertyItem17 != null)
				{
					propertyItem17.SetDelta(ECharacterPropertyDisplayType.RecoveryOfBlockedAcupoint, valueOfRecoveryOfBlockedAcupoint, false);
				}
				PropertyItem propertyItem18 = this.recoveryOfQiDisorder;
				if (propertyItem18 != null)
				{
					propertyItem18.Set(Attribute.GetMinorAttributeIcon("9"), LocalStringManager.Get(LanguageKey.LK_RecoveryOfQiDisorder), string.Format("{0}%", this._currentData.RecoveryOfQiDisorder), null, false);
				}
				PropertyItem propertyItem19 = this.recoveryOfQiDisorder;
				if (propertyItem19 != null)
				{
					propertyItem19.SetDelta(ECharacterPropertyDisplayType.RecoveryOfQiDisorder, valueOfRecoveryOfQiDisorder, false);
				}
				PropertyItem propertyItem20 = this.innerRatio;
				if (propertyItem20 != null)
				{
					propertyItem20.Set(Attribute.GetMinorAttributeIcon("8"), LocalStringManager.Get(LanguageKey.LK_InnerRatio), string.Format("{0}%", this._currentData.InnerRatio), null, false);
				}
				PropertyItem propertyItem21 = this.innerRatio;
				if (propertyItem21 != null)
				{
					propertyItem21.SetDelta(ECharacterPropertyDisplayType.InnerRatio, valueOfInnerRatio, false);
				}
			}
		}

		// Token: 0x0600B196 RID: 45462 RVA: 0x0050EA18 File Offset: 0x0050CC18
		private static string GetMinorAttributeIcon(string iconSuffix)
		{
			return "ui9_icon_attribute_minor_big_" + iconSuffix;
		}

		// Token: 0x0600B197 RID: 45463 RVA: 0x0050EA38 File Offset: 0x0050CC38
		public static string GetMainAttributeName(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Strength);
				break;
			case 1:
				result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Dexterity);
				break;
			case 2:
				result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Concentration);
				break;
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Vitality);
				break;
			case 4:
				result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Energy);
				break;
			case 5:
				result = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Intelligence);
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B198 RID: 45464 RVA: 0x0050EACC File Offset: 0x0050CCCC
		public static string GetMainAttributeIcon(sbyte type)
		{
			return "ui9_icon_attribute_major_big_" + type.ToString();
		}

		// Token: 0x0600B199 RID: 45465 RVA: 0x0050EAF0 File Offset: 0x0050CCF0
		public static string GetPoisonResistName(sbyte type)
		{
			return LocalStringManager.Get(string.Format("LK_Poison_Name_{0}", type));
		}

		// Token: 0x0600B19A RID: 45466 RVA: 0x0050EB18 File Offset: 0x0050CD18
		private static string GetPoisonResistIcon(sbyte type)
		{
			return "ui9_icon_attribute_poison_big_" + type.ToString();
		}

		// Token: 0x0600B19B RID: 45467 RVA: 0x0050EB3C File Offset: 0x0050CD3C
		private static string GetPenetrationName(bool isOuter)
		{
			return isOuter ? LocalStringManager.Get(LanguageKey.LK_Penetrate_Outer) : LocalStringManager.Get(LanguageKey.LK_Penetrate_Inner);
		}

		// Token: 0x0600B19C RID: 45468 RVA: 0x0050EB68 File Offset: 0x0050CD68
		private static string GetPenetrationIcon(bool isOuter)
		{
			return "ui9_icon_attribute_attack_big_" + (isOuter ? "0" : "1");
		}

		// Token: 0x0600B19D RID: 45469 RVA: 0x0050EB94 File Offset: 0x0050CD94
		private static string GetPenetrationResistName(bool isOuter)
		{
			return isOuter ? LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Outer) : LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Inner);
		}

		// Token: 0x0600B19E RID: 45470 RVA: 0x0050EBC0 File Offset: 0x0050CDC0
		private static string GetPenetrationResistIcon(bool isOuter)
		{
			return "ui9_icon_attribute_defence_big_" + (isOuter ? "0" : "1");
		}

		// Token: 0x0600B19F RID: 45471 RVA: 0x0050EBEC File Offset: 0x0050CDEC
		private static string GetHitValueName(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_0);
				break;
			case 1:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_1);
				break;
			case 2:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_2);
				break;
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_HitType_3);
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B1A0 RID: 45472 RVA: 0x0050EC5C File Offset: 0x0050CE5C
		private static string GetHitValueIcon(sbyte type)
		{
			return "ui9_icon_attribute_hit_big_" + type.ToString();
		}

		// Token: 0x0600B1A1 RID: 45473 RVA: 0x0050EC80 File Offset: 0x0050CE80
		private static string GetAvoidValueName(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_0);
				break;
			case 1:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_1);
				break;
			case 2:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_2);
				break;
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_AvoidType_3);
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B1A2 RID: 45474 RVA: 0x0050ECF0 File Offset: 0x0050CEF0
		private static string GetAvoidValueIcon(sbyte type)
		{
			return "ui9_icon_attribute_avoid_big_" + type.ToString();
		}

		// Token: 0x04008980 RID: 35200
		[SerializeField]
		private GameObject mainAttributeObj;

		// Token: 0x04008981 RID: 35201
		[Header("主属性")]
		[SerializeField]
		private PropertyItem[] mainAttributeItems;

		// Token: 0x04008982 RID: 35202
		[Header("主属性恢复")]
		[SerializeField]
		private ValuePerMonth[] mainAttributeRecoveries;

		// Token: 0x04008983 RID: 35203
		[SerializeField]
		private GameObject poisonResistObj;

		// Token: 0x04008984 RID: 35204
		[Header("毒素抵抗")]
		[SerializeField]
		private PropertyItem[] poisonResistItem;

		// Token: 0x04008985 RID: 35205
		[Header("攻击属性")]
		[SerializeField]
		private PropertyItem penetrationTotal;

		// Token: 0x04008986 RID: 35206
		[SerializeField]
		private PropertyItem penetrationOuter;

		// Token: 0x04008987 RID: 35207
		[SerializeField]
		private PropertyItem penetrationInner;

		// Token: 0x04008988 RID: 35208
		[Header("防御属性")]
		[SerializeField]
		private PropertyItem penetrationResistTotal;

		// Token: 0x04008989 RID: 35209
		[SerializeField]
		private PropertyItem penetrationResistOuter;

		// Token: 0x0400898A RID: 35210
		[SerializeField]
		private PropertyItem penetrationResistInner;

		// Token: 0x0400898B RID: 35211
		[Header("命中属性")]
		[SerializeField]
		private PropertyItem hitTotal;

		// Token: 0x0400898C RID: 35212
		[SerializeField]
		private PropertyItem[] hitValueItems;

		// Token: 0x0400898D RID: 35213
		[Header("化解属性")]
		[SerializeField]
		private PropertyItem avoidTotal;

		// Token: 0x0400898E RID: 35214
		[SerializeField]
		private PropertyItem[] avoidValueItems;

		// Token: 0x0400898F RID: 35215
		[Header("次要属性")]
		[SerializeField]
		private PropertyItem recoveryTotal;

		// Token: 0x04008990 RID: 35216
		[SerializeField]
		private PropertyItem recoveryOfStance;

		// Token: 0x04008991 RID: 35217
		[SerializeField]
		private PropertyItem recoveryOfBreath;

		// Token: 0x04008992 RID: 35218
		[SerializeField]
		private PropertyItem attackSpeed;

		// Token: 0x04008993 RID: 35219
		[SerializeField]
		private PropertyItem moveSpeed;

		// Token: 0x04008994 RID: 35220
		[SerializeField]
		private PropertyItem weaponSwitchSpeed;

		// Token: 0x04008995 RID: 35221
		[SerializeField]
		private PropertyItem castSpeed;

		// Token: 0x04008996 RID: 35222
		[SerializeField]
		private PropertyItem recoveryOfFlaw;

		// Token: 0x04008997 RID: 35223
		[SerializeField]
		private PropertyItem recoveryOfBlockedAcupoint;

		// Token: 0x04008998 RID: 35224
		[SerializeField]
		private PropertyItem recoveryOfQiDisorder;

		// Token: 0x04008999 RID: 35225
		[SerializeField]
		private PropertyItem innerRatio;

		// Token: 0x0400899A RID: 35226
		private CharacterAttributeDisplayData _currentData;

		// Token: 0x0400899B RID: 35227
		public bool IsDisplayingAllocationBonus = false;

		// Token: 0x0400899C RID: 35228
		public Action RefreshAllocationBonus;

		// Token: 0x0400899D RID: 35229
		private const string MinorAttributeIconStance = "0";

		// Token: 0x0400899E RID: 35230
		private const string MinorAttributeIconBreath = "1";

		// Token: 0x0400899F RID: 35231
		private const string MinorAttributeIconMoveSpeed = "2";

		// Token: 0x040089A0 RID: 35232
		private const string MinorAttributeIconRecoveryFlaw = "3";

		// Token: 0x040089A1 RID: 35233
		private const string MinorAttributeIconCastSpeed = "4";

		// Token: 0x040089A2 RID: 35234
		private const string MinorAttributeIconBlockedAcupoint = "5";

		// Token: 0x040089A3 RID: 35235
		private const string MinorAttributeIconAttackSpeed = "6";

		// Token: 0x040089A4 RID: 35236
		private const string MinorAttributeIconInnerRatio = "8";

		// Token: 0x040089A5 RID: 35237
		private const string MinorAttributeIconQiDisorder = "9";

		// Token: 0x040089A6 RID: 35238
		private const string MinorAttributeIconWeaponSwitch = "7";
	}
}
