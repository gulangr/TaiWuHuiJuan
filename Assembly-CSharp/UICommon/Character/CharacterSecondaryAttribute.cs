using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005E8 RID: 1512
	public class CharacterSecondaryAttribute : CharacterUIElement
	{
		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x0600475C RID: 18268 RVA: 0x00216A9B File Offset: 0x00214C9B
		private SecondaryAttributeMonitor Item
		{
			get
			{
				return this.MonitorDataItem as SecondaryAttributeMonitor;
			}
		}

		// Token: 0x0600475D RID: 18269 RVA: 0x00216AA8 File Offset: 0x00214CA8
		public CharacterSecondaryAttribute(Dictionary<sbyte, Refers> attributeRefersMap)
		{
			bool flag = attributeRefersMap == null || attributeRefersMap.Count <= 0;
			if (flag)
			{
				throw new Exception("CharacterSecondaryAttribute must handle as least one attribute refers");
			}
			this._attributeSliders = new AttributeSlider[10];
			for (sbyte i = 0; i < 10; i += 1)
			{
				Refers refers;
				bool flag2 = attributeRefersMap.TryGetValue(i, out refers) && null != refers;
				if (flag2)
				{
					this._attributeSliders[(int)i] = new AttributeSlider(refers, CharacterSecondaryAttribute.SecondaryAttributeTemplateIdArray[(int)i], 0f);
					this._attributeSliders[(int)i].MaxValue = 1000f;
				}
			}
		}

		// Token: 0x0600475E RID: 18270 RVA: 0x00216B48 File Offset: 0x00214D48
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<SecondaryAttributeMonitor>(charId, this.IsDead);
		}

		// Token: 0x0600475F RID: 18271 RVA: 0x00216B6B File Offset: 0x00214D6B
		internal override void BindEvent()
		{
			this.Item.AddAttributeListener(new Action<sbyte>(this.FillValue));
		}

		// Token: 0x06004760 RID: 18272 RVA: 0x00216B86 File Offset: 0x00214D86
		public override void UnbindEvent()
		{
			this.Item.RemoveAttributeListener(new Action<sbyte>(this.FillValue));
		}

		// Token: 0x06004761 RID: 18273 RVA: 0x00216BA1 File Offset: 0x00214DA1
		public override void FillElement()
		{
			this.FillValue(10);
		}

		// Token: 0x06004762 RID: 18274 RVA: 0x00216BB0 File Offset: 0x00214DB0
		public override void ResetToEmpty()
		{
			foreach (AttributeSlider slider in this._attributeSliders)
			{
				bool flag = slider == null;
				if (!flag)
				{
					slider.Value = 0f;
				}
			}
		}

		// Token: 0x06004763 RID: 18275 RVA: 0x00216BF0 File Offset: 0x00214DF0
		private void FillValue(sbyte type)
		{
			bool flag = this._attributeSliders == null;
			if (!flag)
			{
				bool flag2 = type == 10;
				if (flag2)
				{
					for (sbyte i = 0; i < 10; i += 1)
					{
						this.FillValue(i);
					}
				}
				else
				{
					bool flag3 = this._attributeSliders.CheckIndex((int)type) && this._attributeSliders[(int)type] != null;
					if (flag3)
					{
						this._attributeSliders[(int)type].Value = (float)this.Item.Attributes[(int)type];
					}
				}
			}
		}

		// Token: 0x06004764 RID: 18276 RVA: 0x00216C78 File Offset: 0x00214E78
		public void SetCompareValue(sbyte type, float compareValue, float value = -1f)
		{
			bool flag = this._attributeSliders == null;
			if (!flag)
			{
				bool flag2 = this._attributeSliders.CheckIndex((int)type) && this._attributeSliders[(int)type] != null;
				if (flag2)
				{
					bool flag3 = value < 0f;
					if (flag3)
					{
						this._attributeSliders[(int)type].SetCompareValue((float)this.Item.Attributes[(int)type] + compareValue);
					}
					else
					{
						this._attributeSliders[(int)type].UpdateValueImmediately(value, compareValue);
					}
				}
			}
		}

		// Token: 0x06004765 RID: 18277 RVA: 0x00216CF4 File Offset: 0x00214EF4
		public void SetValueToStringFunc(Func<float, string> func)
		{
			foreach (AttributeSlider slider in this._attributeSliders)
			{
				slider.GetShowValueString = func;
			}
		}

		// Token: 0x06004766 RID: 18278 RVA: 0x00216D28 File Offset: 0x00214F28
		public void SetSliderMaxValue(sbyte type, float maxValue)
		{
			bool flag = this._attributeSliders == null;
			if (!flag)
			{
				bool flag2 = type == 10;
				if (flag2)
				{
					for (sbyte i = 0; i < 10; i += 1)
					{
						this.SetSliderMaxValue(i, maxValue);
					}
				}
				else
				{
					bool flag3 = this._attributeSliders.CheckIndex((int)type) && this._attributeSliders[(int)type] != null;
					if (flag3)
					{
						this._attributeSliders[(int)type].MaxValue = maxValue;
					}
				}
			}
		}

		// Token: 0x04003144 RID: 12612
		private static readonly short[] SecondaryAttributeTemplateIdArray = new short[]
		{
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27
		};

		// Token: 0x04003145 RID: 12613
		private readonly AttributeSlider[] _attributeSliders;
	}
}
