using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005DF RID: 1503
	public class CharacterMajorAttribute : CharacterUIElement
	{
		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x060046F8 RID: 18168 RVA: 0x002139F7 File Offset: 0x00211BF7
		private AttributeMonitor Item
		{
			get
			{
				return this.MonitorDataItem as AttributeMonitor;
			}
		}

		// Token: 0x060046F9 RID: 18169 RVA: 0x00213A04 File Offset: 0x00211C04
		public CharacterMajorAttribute(Dictionary<sbyte, Refers> mainAttrRefersMap = null, Dictionary<sbyte, Refers> recoveryRefersMap = null, Refers[] atkHitRefers = null, Refers[] defHitRefers = null, Refers[] atkPenetrabilityRefers = null, Refers[] defPenetrabilityRefers = null)
		{
			bool hasMainAttrRefers = mainAttrRefersMap != null && mainAttrRefersMap.Count > 0;
			bool hasRecoveryRefers = recoveryRefersMap != null && recoveryRefersMap.Count > 0;
			bool hasAtkHitRefers = atkHitRefers != null && atkHitRefers.Length != 0;
			bool hasDefHitRefers = defHitRefers != null && defHitRefers.Length != 0;
			bool hasAtkPenetrabilityRefers = atkPenetrabilityRefers != null && atkPenetrabilityRefers.Length != 0;
			bool hasDefPenetrabilityRefers = defPenetrabilityRefers != null && defPenetrabilityRefers.Length != 0;
			bool flag = !hasMainAttrRefers && !hasAtkHitRefers && !hasDefHitRefers && !hasAtkPenetrabilityRefers && !hasDefPenetrabilityRefers;
			if (flag)
			{
				throw new Exception("CharacterMainAttribute must handle as least one attribute refers");
			}
			bool flag2 = mainAttrRefersMap != null && hasMainAttrRefers;
			if (flag2)
			{
				this._mainAttributeItemsWithRecovery = new AttributeItemWithRecovery[6];
				for (sbyte i = 0; i < 6; i += 1)
				{
					Refers refers;
					bool flag3 = mainAttrRefersMap.TryGetValue(i, out refers);
					if (flag3)
					{
						Refers r;
						Refers recoveryRefers = (hasRecoveryRefers && recoveryRefersMap.TryGetValue(i, out r)) ? r : null;
						this._mainAttributeItemsWithRecovery[(int)i] = new AttributeItemWithRecovery(refers, recoveryRefers, CharacterMajorAttribute.MainAttributeTemplateIdArray[(int)i], i / 2 % 2 == 0);
					}
					this._mainAttributeItemsWithRecovery[(int)i].GetShowValueString = new Func<short, int, string>(this.GetMainAttributeShowString);
				}
			}
			bool flag4 = hasAtkHitRefers && atkHitRefers != null;
			if (flag4)
			{
				this._atkHitAttributeItems = new AttributeItem[4];
				for (sbyte j = 0; j < 4; j += 1)
				{
					Refers refers2 = atkHitRefers[(int)j];
					bool flag5 = null == refers2;
					if (!flag5)
					{
						this._atkHitAttributeItems[(int)j] = new AttributeItem(refers2, CharacterMajorAttribute.AtkHitAttributeTemplateIdArray[(int)j], j / 2 % 2 == 0);
					}
				}
			}
			bool flag6 = hasDefHitRefers && defHitRefers != null;
			if (flag6)
			{
				this._defHitAttributeItems = new AttributeItem[4];
				for (sbyte k = 0; k < 4; k += 1)
				{
					Refers refers3 = defHitRefers[(int)k];
					bool flag7 = null == refers3;
					if (!flag7)
					{
						this._defHitAttributeItems[(int)k] = new AttributeItem(refers3, CharacterMajorAttribute.DefHitAttributeTemplateIdArray[(int)k], k / 2 % 2 == 0);
					}
				}
			}
			bool flag8 = hasAtkPenetrabilityRefers && atkPenetrabilityRefers != null;
			if (flag8)
			{
				this._atkPenetrabilityItems = new AttributeItem[2];
				bool flag9 = null != atkPenetrabilityRefers[0];
				if (flag9)
				{
					this._atkPenetrabilityItems[0] = new AttributeItem(atkPenetrabilityRefers[0], CharacterMajorAttribute.AtkPenetrabilityOuter, true);
				}
				bool flag10 = null != atkPenetrabilityRefers[1];
				if (flag10)
				{
					this._atkPenetrabilityItems[1] = new AttributeItem(atkPenetrabilityRefers[1], CharacterMajorAttribute.AtkPenetrabilityInner, false);
				}
			}
			bool flag11 = hasDefPenetrabilityRefers && defPenetrabilityRefers != null;
			if (flag11)
			{
				this._defPenetrabilityItems = new AttributeItem[2];
				bool flag12 = null != defPenetrabilityRefers[0];
				if (flag12)
				{
					this._defPenetrabilityItems[0] = new AttributeItem(defPenetrabilityRefers[0], CharacterMajorAttribute.DefPenetrabilityOuter, true);
				}
				bool flag13 = null != defPenetrabilityRefers[1];
				if (flag13)
				{
					this._defPenetrabilityItems[1] = new AttributeItem(defPenetrabilityRefers[1], CharacterMajorAttribute.DefPenetrabilityInner, false);
				}
			}
		}

		// Token: 0x060046FA RID: 18170 RVA: 0x00213CEC File Offset: 0x00211EEC
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			this._recoveryMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(charId, this.IsDead);
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AttributeMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046FB RID: 18171 RVA: 0x00213D28 File Offset: 0x00211F28
		internal override void BindEvent()
		{
			this.Item.AddMainAttributeListener(new Action<sbyte>(this.FillMainAttributeValue));
			this.Item.AddAtkHitValuesListener(new Action<sbyte>(this.FillAtkHitAttributeValue));
			this.Item.AddDefHitValuesListener(new Action<sbyte>(this.FillDefHitAttributeValue));
			this.Item.AddAtkPenetrabilityListener(new Action(this.FillAtkPenetrabilityValue));
			this.Item.AddDefPenetrabilityListener(new Action(this.FillDefPenetrabilityValue));
			bool flag = this._recoveryMonitor != null;
			if (flag)
			{
				this._recoveryMonitor.AddChangeOfMainAttributeListener(new Action(this.FillRecoveryValues));
			}
		}

		// Token: 0x060046FC RID: 18172 RVA: 0x00213DD4 File Offset: 0x00211FD4
		public override void UnbindEvent()
		{
			this.Item.RemoveMainAttributeListener(new Action<sbyte>(this.FillMainAttributeValue));
			this.Item.RemoveAtkHitValuesListener(new Action<sbyte>(this.FillAtkHitAttributeValue));
			this.Item.RemoveDefHitValuesListener(new Action<sbyte>(this.FillDefHitAttributeValue));
			this.Item.RemoveAtkPenetrabilityListener(new Action(this.FillAtkPenetrabilityValue));
			this.Item.RemoveDefPenetrabilityListener(new Action(this.FillDefPenetrabilityValue));
			bool flag = this._recoveryMonitor != null;
			if (flag)
			{
				this._recoveryMonitor.RemoveChangeOfMainAttributeListener(new Action(this.FillRecoveryValues));
			}
		}

		// Token: 0x060046FD RID: 18173 RVA: 0x00213E7F File Offset: 0x0021207F
		public override void FillElement()
		{
			this.FillMainAttributeValue(6);
			this.FillAtkHitAttributeValue(4);
			this.FillDefHitAttributeValue(4);
			this.FillAtkPenetrabilityValue();
			this.FillDefPenetrabilityValue();
			this.FillRecoveryValues();
		}

		// Token: 0x060046FE RID: 18174 RVA: 0x00213EB0 File Offset: 0x002120B0
		public override void ResetToEmpty()
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				bool flag = this._mainAttributeItemsWithRecovery[(int)i] == null;
				if (!flag)
				{
					this._mainAttributeItemsWithRecovery[(int)i].GetShowValueString = null;
					this._mainAttributeItemsWithRecovery[(int)i].UpdateValue(0, 0);
					this._mainAttributeItemsWithRecovery[(int)i].GetShowValueString = new Func<short, int, string>(this.GetMainAttributeShowString);
					this._mainAttributeItemsWithRecovery[(int)i].UpdateRecoveryValue(0);
				}
			}
			for (sbyte j = 0; j < 4; j += 1)
			{
				AttributeItem attributeItem = this._atkHitAttributeItems[(int)j];
				if (attributeItem != null)
				{
					attributeItem.UpdateValue(0, 0);
				}
			}
			for (sbyte k = 0; k < 4; k += 1)
			{
				AttributeItem attributeItem2 = this._defHitAttributeItems[(int)k];
				if (attributeItem2 != null)
				{
					attributeItem2.UpdateValue(0, 0);
				}
			}
			AttributeItem attributeItem3 = this._atkPenetrabilityItems[0];
			if (attributeItem3 != null)
			{
				attributeItem3.UpdateValue(0, 0);
			}
			AttributeItem attributeItem4 = this._atkPenetrabilityItems[1];
			if (attributeItem4 != null)
			{
				attributeItem4.UpdateValue(0, 0);
			}
			AttributeItem attributeItem5 = this._defPenetrabilityItems[0];
			if (attributeItem5 != null)
			{
				attributeItem5.UpdateValue(0, 0);
			}
			AttributeItem attributeItem6 = this._defPenetrabilityItems[1];
			if (attributeItem6 != null)
			{
				attributeItem6.UpdateValue(0, 0);
			}
		}

		// Token: 0x060046FF RID: 18175 RVA: 0x00213FDC File Offset: 0x002121DC
		private string GetMainAttributeShowString(short templateId, int value)
		{
			bool flag = this.Item == null;
			string result;
			if (flag)
			{
				result = value.ToString();
			}
			else
			{
				int index = Array.IndexOf<short>(CharacterMajorAttribute.MainAttributeTemplateIdArray, templateId);
				bool flag2 = !CharacterMajorAttribute.MainAttributeTemplateIdArray.CheckIndex(index);
				if (flag2)
				{
					result = value.ToString();
				}
				else
				{
					result = string.Format("{0}/{1}", value, this.Item.MaxMainAttribute[index]);
				}
			}
			return result;
		}

		// Token: 0x06004700 RID: 18176 RVA: 0x00214050 File Offset: 0x00212250
		private void FillMainAttributeValue(sbyte type)
		{
			bool flag = this._mainAttributeItemsWithRecovery == null;
			if (!flag)
			{
				bool flag2 = type == 6;
				if (flag2)
				{
					for (sbyte i = 0; i < 6; i += 1)
					{
						this.FillMainAttributeValue(i);
					}
				}
				else
				{
					bool flag3 = this._mainAttributeItemsWithRecovery.CheckIndex((int)type);
					if (flag3)
					{
						this._mainAttributeItemsWithRecovery[(int)type].UpdateValue((int)this.Item.CurMainAttribute[(int)type], 0);
					}
				}
			}
		}

		// Token: 0x06004701 RID: 18177 RVA: 0x002140C4 File Offset: 0x002122C4
		private void FillRecoveryValues()
		{
			bool flag = this._mainAttributeItemsWithRecovery == null || this._recoveryMonitor == null;
			if (!flag)
			{
				for (sbyte i = 0; i < 6; i += 1)
				{
					bool flag2 = this._mainAttributeItemsWithRecovery.CheckIndex((int)i);
					if (flag2)
					{
						this._mainAttributeItemsWithRecovery[(int)i].UpdateRecoveryValue(this._recoveryMonitor.MainAttribute[(int)i]);
					}
				}
			}
		}

		// Token: 0x06004702 RID: 18178 RVA: 0x0021412C File Offset: 0x0021232C
		private void FillAtkHitAttributeValue(sbyte type)
		{
			bool flag = this._atkHitAttributeItems == null;
			if (!flag)
			{
				bool flag2 = type == 4;
				if (flag2)
				{
					for (sbyte i = 0; i < 4; i += 1)
					{
						this.FillAtkHitAttributeValue(i);
					}
				}
				else
				{
					bool flag3 = this._atkHitAttributeItems.CheckIndex((int)type) && this._atkHitAttributeItems[(int)type] != null;
					if (flag3)
					{
						this._atkHitAttributeItems[(int)type].UpdateValue(this.Item.AtkHitValues[(int)type], 0);
					}
				}
			}
		}

		// Token: 0x06004703 RID: 18179 RVA: 0x002141AC File Offset: 0x002123AC
		private void FillDefHitAttributeValue(sbyte type)
		{
			bool flag = this._defHitAttributeItems == null;
			if (!flag)
			{
				bool flag2 = type == 4;
				if (flag2)
				{
					for (sbyte i = 0; i < 4; i += 1)
					{
						this.FillDefHitAttributeValue(i);
					}
				}
				else
				{
					bool flag3 = this._defHitAttributeItems.CheckIndex((int)type) && this._defHitAttributeItems[(int)type] != null;
					if (flag3)
					{
						this._defHitAttributeItems[(int)type].UpdateValue(this.Item.DefHitValues[(int)type], 0);
					}
				}
			}
		}

		// Token: 0x06004704 RID: 18180 RVA: 0x0021422C File Offset: 0x0021242C
		private void FillAtkPenetrabilityValue()
		{
			AttributeItem[] atkPenetrabilityItems = this._atkPenetrabilityItems;
			if (atkPenetrabilityItems != null)
			{
				AttributeItem attributeItem = atkPenetrabilityItems[0];
				if (attributeItem != null)
				{
					attributeItem.UpdateValue(this.Item.AtkPenetrability.Outer, 0);
				}
			}
			AttributeItem[] atkPenetrabilityItems2 = this._atkPenetrabilityItems;
			if (atkPenetrabilityItems2 != null)
			{
				AttributeItem attributeItem2 = atkPenetrabilityItems2[1];
				if (attributeItem2 != null)
				{
					attributeItem2.UpdateValue(this.Item.AtkPenetrability.Inner, 0);
				}
			}
		}

		// Token: 0x06004705 RID: 18181 RVA: 0x00214290 File Offset: 0x00212490
		private void FillDefPenetrabilityValue()
		{
			AttributeItem[] defPenetrabilityItems = this._defPenetrabilityItems;
			if (defPenetrabilityItems != null)
			{
				AttributeItem attributeItem = defPenetrabilityItems[0];
				if (attributeItem != null)
				{
					attributeItem.UpdateValue(this.Item.DefPenetrability.Outer, 0);
				}
			}
			AttributeItem[] defPenetrabilityItems2 = this._defPenetrabilityItems;
			if (defPenetrabilityItems2 != null)
			{
				AttributeItem attributeItem2 = defPenetrabilityItems2[1];
				if (attributeItem2 != null)
				{
					attributeItem2.UpdateValue(this.Item.DefPenetrability.Inner, 0);
				}
			}
		}

		// Token: 0x06004706 RID: 18182 RVA: 0x002142F4 File Offset: 0x002124F4
		public void SetMainAttributeBonus(sbyte type, int bonus, int value = -1)
		{
			bool flag = this._mainAttributeItemsWithRecovery == null || !this._mainAttributeItemsWithRecovery.CheckIndex((int)type);
			if (!flag)
			{
				bool flag2 = value < 0;
				if (flag2)
				{
					this._mainAttributeItemsWithRecovery[(int)type].UpdateValue((int)this.Item.CurMainAttribute[(int)type], bonus);
				}
				else
				{
					this._mainAttributeItemsWithRecovery[(int)type].UpdateValueImmediately(value, bonus);
				}
			}
		}

		// Token: 0x06004707 RID: 18183 RVA: 0x00214358 File Offset: 0x00212558
		public void SetAtkHitAttributeBonus(sbyte type, int bonus, int value = -1)
		{
			bool flag = this._atkHitAttributeItems == null || !this._atkHitAttributeItems.CheckIndex((int)type);
			if (!flag)
			{
				bool flag2 = value < 0;
				if (flag2)
				{
					this._atkHitAttributeItems[(int)type].UpdateValue(this.Item.AtkHitValues[(int)type], bonus);
				}
				else
				{
					this._atkHitAttributeItems[(int)type].UpdateValueImmediately(value, bonus);
				}
			}
		}

		// Token: 0x06004708 RID: 18184 RVA: 0x002143BC File Offset: 0x002125BC
		public void SetDefHitAttributeBonus(sbyte type, int bonus, int value = -1)
		{
			bool flag = this._defHitAttributeItems == null || !this._defHitAttributeItems.CheckIndex((int)type);
			if (!flag)
			{
				bool flag2 = value < 0;
				if (flag2)
				{
					this._defHitAttributeItems[(int)type].UpdateValue(this.Item.DefHitValues[(int)type], bonus);
				}
				else
				{
					this._defHitAttributeItems[(int)type].UpdateValueImmediately(value, bonus);
				}
			}
		}

		// Token: 0x06004709 RID: 18185 RVA: 0x00214420 File Offset: 0x00212620
		public void SetAtkPenetrabilityBonus(bool isOuter, int bonus, int value = -1)
		{
			bool flag = this._atkPenetrabilityItems == null;
			if (!flag)
			{
				if (isOuter)
				{
					bool flag2 = value < 0;
					if (flag2)
					{
						this._atkPenetrabilityItems[0].UpdateValue(this.Item.AtkPenetrability.Outer, bonus);
					}
					else
					{
						this._atkPenetrabilityItems[0].UpdateValueImmediately(value, bonus);
					}
				}
				else
				{
					bool flag3 = value < 0;
					if (flag3)
					{
						this._atkPenetrabilityItems[1].UpdateValue(this.Item.AtkPenetrability.Inner, bonus);
					}
					else
					{
						this._atkPenetrabilityItems[1].UpdateValueImmediately(value, bonus);
					}
				}
			}
		}

		// Token: 0x0600470A RID: 18186 RVA: 0x002144BC File Offset: 0x002126BC
		public void SetDefPenetrabilityBonus(bool isOuter, int bonus, int value = -1)
		{
			bool flag = this._defPenetrabilityItems == null;
			if (!flag)
			{
				if (isOuter)
				{
					bool flag2 = value < 0;
					if (flag2)
					{
						this._defPenetrabilityItems[0].UpdateValue(this.Item.DefPenetrability.Outer, bonus);
					}
					else
					{
						this._defPenetrabilityItems[0].UpdateValueImmediately(value, bonus);
					}
				}
				else
				{
					bool flag3 = value < 0;
					if (flag3)
					{
						this._defPenetrabilityItems[1].UpdateValue(this.Item.DefPenetrability.Inner, bonus);
					}
					else
					{
						this._defPenetrabilityItems[1].UpdateValueImmediately(value, bonus);
					}
				}
			}
		}

		// Token: 0x04003111 RID: 12561
		public static readonly short[] MainAttributeTemplateIdArray = new short[]
		{
			0,
			1,
			2,
			3,
			4,
			5
		};

		// Token: 0x04003112 RID: 12562
		private static readonly short[] AtkHitAttributeTemplateIdArray = new short[]
		{
			6,
			7,
			8,
			9
		};

		// Token: 0x04003113 RID: 12563
		private static readonly short[] DefHitAttributeTemplateIdArray = new short[]
		{
			12,
			13,
			14,
			15
		};

		// Token: 0x04003114 RID: 12564
		private static readonly short AtkPenetrabilityOuter = 10;

		// Token: 0x04003115 RID: 12565
		private static readonly short AtkPenetrabilityInner = 11;

		// Token: 0x04003116 RID: 12566
		private static readonly short DefPenetrabilityOuter = 16;

		// Token: 0x04003117 RID: 12567
		private static readonly short DefPenetrabilityInner = 17;

		// Token: 0x04003118 RID: 12568
		private DisorderOfQiMonitor _recoveryMonitor;

		// Token: 0x04003119 RID: 12569
		private readonly AttributeItemWithRecovery[] _mainAttributeItemsWithRecovery;

		// Token: 0x0400311A RID: 12570
		private readonly AttributeItem[] _atkHitAttributeItems;

		// Token: 0x0400311B RID: 12571
		private readonly AttributeItem[] _defHitAttributeItems;

		// Token: 0x0400311C RID: 12572
		private readonly AttributeItem[] _atkPenetrabilityItems;

		// Token: 0x0400311D RID: 12573
		private readonly AttributeItem[] _defPenetrabilityItems;
	}
}
