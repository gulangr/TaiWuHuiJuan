using System;
using GameData.Domains.Mod;
using MoonSharp.Interpreter;

namespace FrameWork.ModSystem
{
	// Token: 0x02001049 RID: 4169
	public abstract class SettingEntry
	{
		// Token: 0x1700156A RID: 5482
		// (get) Token: 0x0600BE42 RID: 48706 RVA: 0x0056517F File Offset: 0x0056337F
		public virtual SettingEntry.SettingType Type
		{
			get
			{
				return SettingEntry.SettingType.Toggle;
			}
		}

		// Token: 0x0600BE43 RID: 48707 RVA: 0x00565182 File Offset: 0x00563382
		public SettingEntry()
		{
		}

		// Token: 0x0600BE44 RID: 48708 RVA: 0x0056518C File Offset: 0x0056338C
		public SettingEntry(string groupName, string key, string displayName, string description)
		{
			this.GroupName = groupName;
			this.Key = key;
			this.DisplayName = displayName;
			this.Description = description;
		}

		// Token: 0x0600BE45 RID: 48709
		public abstract void SaveToSerializableModData(SerializableModData settings);

		// Token: 0x0600BE46 RID: 48710
		public abstract void RestoreFromSerializableModData(SerializableModData settings);

		// Token: 0x0600BE47 RID: 48711
		public abstract void SaveToLuaTable(Table luaTable);

		// Token: 0x0600BE48 RID: 48712
		public abstract void LoadFromLuaTable(Table luaTable);

		// Token: 0x0600BE49 RID: 48713 RVA: 0x005651B4 File Offset: 0x005633B4
		public virtual void SaveDefaultSetting(Table settingEntryTable)
		{
			settingEntryTable.Save("SettingType", this.Type.ToString());
			settingEntryTable.Save("Key", this.Key);
			settingEntryTable.Save("DisplayName", this.DisplayName);
			settingEntryTable.Save("Description", this.Description);
			settingEntryTable.Save("GroupName", this.GroupName);
		}

		// Token: 0x0600BE4A RID: 48714 RVA: 0x0056522C File Offset: 0x0056342C
		public virtual void LoadDefaultSetting(Table settingEntryTable)
		{
			settingEntryTable.Load("Key", out this.Key);
			settingEntryTable.Load("DisplayName", out this.DisplayName);
			settingEntryTable.Load("Description", out this.Description);
			settingEntryTable.Load("GroupName", out this.GroupName);
		}

		// Token: 0x0600BE4B RID: 48715
		public abstract SettingEntry Clone();

		// Token: 0x04009242 RID: 37442
		public string Key;

		// Token: 0x04009243 RID: 37443
		public string DisplayName;

		// Token: 0x04009244 RID: 37444
		public string Description;

		// Token: 0x04009245 RID: 37445
		public string GroupName;

		// Token: 0x02002683 RID: 9859
		public enum SettingType
		{
			// Token: 0x0400EAF6 RID: 60150
			Toggle,
			// Token: 0x0400EAF7 RID: 60151
			InputField,
			// Token: 0x0400EAF8 RID: 60152
			Slider,
			// Token: 0x0400EAF9 RID: 60153
			Dropdown,
			// Token: 0x0400EAFA RID: 60154
			ToggleGroup
		}
	}
}
