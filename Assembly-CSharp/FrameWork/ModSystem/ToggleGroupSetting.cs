using System;
using GameData.Domains.Mod;
using GameData.Utilities;
using MoonSharp.Interpreter;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x0200104B RID: 4171
	public class ToggleGroupSetting : SettingEntry, ISettingValueWrapper<int>
	{
		// Token: 0x17001570 RID: 5488
		// (get) Token: 0x0600BE5E RID: 48734 RVA: 0x0056551E File Offset: 0x0056371E
		// (set) Token: 0x0600BE5F RID: 48735 RVA: 0x00565526 File Offset: 0x00563726
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = MathUtils.Clamp(value, 0, this.Toggles.Length - 1);
			}
		}

		// Token: 0x17001571 RID: 5489
		// (get) Token: 0x0600BE60 RID: 48736 RVA: 0x0056553F File Offset: 0x0056373F
		public override SettingEntry.SettingType Type
		{
			get
			{
				return SettingEntry.SettingType.ToggleGroup;
			}
		}

		// Token: 0x0600BE61 RID: 48737 RVA: 0x00565542 File Offset: 0x00563742
		public ToggleGroupSetting()
		{
		}

		// Token: 0x0600BE62 RID: 48738 RVA: 0x0056554C File Offset: 0x0056374C
		public ToggleGroupSetting(string group, string key, string displayName, string description, int initValue, params string[] toggles) : base(group, key, displayName, description)
		{
			this.Toggles = toggles;
		}

		// Token: 0x0600BE63 RID: 48739 RVA: 0x00565564 File Offset: 0x00563764
		public override SettingEntry Clone()
		{
			return new ToggleGroupSetting(this.GroupName, this.Key, this.DisplayName, this.Description, this.Value, this.Toggles);
		}

		// Token: 0x0600BE64 RID: 48740 RVA: 0x0056559F File Offset: 0x0056379F
		public override void SaveToSerializableModData(SerializableModData settings)
		{
			settings.Set(this.Key, this.Value);
		}

		// Token: 0x0600BE65 RID: 48741 RVA: 0x005655B8 File Offset: 0x005637B8
		public override void RestoreFromSerializableModData(SerializableModData settings)
		{
			int val;
			bool flag = settings.Get(this.Key, out val);
			if (flag)
			{
				this.Value = val;
			}
		}

		// Token: 0x0600BE66 RID: 48742 RVA: 0x005655E0 File Offset: 0x005637E0
		public override void SaveToLuaTable(Table luaTable)
		{
			luaTable.Save(this.Key, this.Value);
		}

		// Token: 0x0600BE67 RID: 48743 RVA: 0x005655F8 File Offset: 0x005637F8
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

		// Token: 0x0600BE68 RID: 48744 RVA: 0x0056567C File Offset: 0x0056387C
		public override void LoadDefaultSetting(Table settingEntryTable)
		{
			base.LoadDefaultSetting(settingEntryTable);
			int defaultValue;
			settingEntryTable.Load("DefaultValue", out defaultValue);
			string[] toggles;
			settingEntryTable.Load("Toggles", out toggles);
			this.Value = defaultValue;
			this.Toggles = toggles;
		}

		// Token: 0x0600BE69 RID: 48745 RVA: 0x005656BD File Offset: 0x005638BD
		public override void SaveDefaultSetting(Table settingEntryTable)
		{
			base.SaveDefaultSetting(settingEntryTable);
			settingEntryTable.Save("DefaultValue", this.Value);
			settingEntryTable.Save("Toggles", this.Toggles);
		}

		// Token: 0x0400924A RID: 37450
		public string[] Toggles;

		// Token: 0x0400924B RID: 37451
		private int _value;
	}
}
