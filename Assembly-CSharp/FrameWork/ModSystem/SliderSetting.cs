using System;
using GameData.Domains.Mod;
using GameData.Utilities;
using MoonSharp.Interpreter;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x0200104A RID: 4170
	public class SliderSetting : SettingEntry, ISettingValueWrapper<int>
	{
		// Token: 0x1700156B RID: 5483
		// (get) Token: 0x0600BE4C RID: 48716 RVA: 0x00565282 File Offset: 0x00563482
		// (set) Token: 0x0600BE4D RID: 48717 RVA: 0x0056528A File Offset: 0x0056348A
		public int MinValue { get; private set; }

		// Token: 0x1700156C RID: 5484
		// (get) Token: 0x0600BE4E RID: 48718 RVA: 0x00565293 File Offset: 0x00563493
		// (set) Token: 0x0600BE4F RID: 48719 RVA: 0x0056529B File Offset: 0x0056349B
		public int MaxValue { get; private set; }

		// Token: 0x1700156D RID: 5485
		// (get) Token: 0x0600BE50 RID: 48720 RVA: 0x005652A4 File Offset: 0x005634A4
		// (set) Token: 0x0600BE51 RID: 48721 RVA: 0x005652AC File Offset: 0x005634AC
		public int StepSize { get; private set; }

		// Token: 0x1700156E RID: 5486
		// (get) Token: 0x0600BE52 RID: 48722 RVA: 0x005652B5 File Offset: 0x005634B5
		// (set) Token: 0x0600BE53 RID: 48723 RVA: 0x005652BD File Offset: 0x005634BD
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = MathUtils.Clamp(value, this.MinValue, this.MaxValue);
			}
		}

		// Token: 0x1700156F RID: 5487
		// (get) Token: 0x0600BE54 RID: 48724 RVA: 0x005652D7 File Offset: 0x005634D7
		public override SettingEntry.SettingType Type
		{
			get
			{
				return SettingEntry.SettingType.Slider;
			}
		}

		// Token: 0x0600BE55 RID: 48725 RVA: 0x005652DA File Offset: 0x005634DA
		public SliderSetting()
		{
		}

		// Token: 0x0600BE56 RID: 48726 RVA: 0x005652E4 File Offset: 0x005634E4
		public SliderSetting(string group, string key, string displayName, string description, int initValue, int minValue, int maxValue, int stepSize) : base(group, key, displayName, description)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.StepSize = stepSize;
			this.Value = initValue;
		}

		// Token: 0x0600BE57 RID: 48727 RVA: 0x00565318 File Offset: 0x00563518
		public override SettingEntry Clone()
		{
			return new SliderSetting(this.GroupName, this.Key, this.DisplayName, this.Description, this.Value, this.MinValue, this.MaxValue, this.StepSize);
		}

		// Token: 0x0600BE58 RID: 48728 RVA: 0x0056535F File Offset: 0x0056355F
		public override void SaveToSerializableModData(SerializableModData settings)
		{
			settings.Set(this.Key, this.Value);
		}

		// Token: 0x0600BE59 RID: 48729 RVA: 0x00565378 File Offset: 0x00563578
		public override void RestoreFromSerializableModData(SerializableModData settings)
		{
			int val;
			bool flag = settings.Get(this.Key, out val);
			if (flag)
			{
				this.Value = val;
			}
		}

		// Token: 0x0600BE5A RID: 48730 RVA: 0x005653A0 File Offset: 0x005635A0
		public override void LoadFromLuaTable(Table luaTable)
		{
			bool flag = !luaTable.ContainsKey(this.Key);
			if (!flag)
			{
				int value = 0;
				try
				{
					luaTable.Load(this.Key, out value);
				}
				catch (Exception e)
				{
					Debug.LogWarning(string.Format("Failed to load setting value on key:{0}, and use default value:{1}", this.Key, value));
				}
				finally
				{
					this.Value = value;
				}
			}
		}

		// Token: 0x0600BE5B RID: 48731 RVA: 0x00565424 File Offset: 0x00563624
		public override void SaveToLuaTable(Table luaTable)
		{
			luaTable.Save(this.Key, this.Value);
		}

		// Token: 0x0600BE5C RID: 48732 RVA: 0x0056543C File Offset: 0x0056363C
		public override void LoadDefaultSetting(Table settingEntryTable)
		{
			base.LoadDefaultSetting(settingEntryTable);
			int minValue;
			settingEntryTable.Load("MinValue", out minValue);
			int maxValue;
			settingEntryTable.Load("MaxValue", out maxValue);
			int stepSize = 1;
			bool flag = settingEntryTable.ContainsKey("StepSize");
			if (flag)
			{
				settingEntryTable.Load("StepSize", out stepSize);
			}
			int defaultValue;
			settingEntryTable.Load("DefaultValue", out defaultValue);
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.StepSize = stepSize;
			this.Value = defaultValue;
		}

		// Token: 0x0600BE5D RID: 48733 RVA: 0x005654C0 File Offset: 0x005636C0
		public override void SaveDefaultSetting(Table settingEntryTable)
		{
			base.SaveDefaultSetting(settingEntryTable);
			settingEntryTable.Save("MinValue", this.MinValue);
			settingEntryTable.Save("MaxValue", this.MaxValue);
			settingEntryTable.Save("StepSize", this.StepSize);
			settingEntryTable.Save("DefaultValue", this.Value);
		}

		// Token: 0x04009249 RID: 37449
		private int _value;
	}
}
