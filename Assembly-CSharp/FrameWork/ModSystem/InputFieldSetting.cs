using System;
using GameData.Domains.Mod;
using MoonSharp.Interpreter;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x02001045 RID: 4165
	public class InputFieldSetting : SettingEntry, ISettingValueWrapper<string>
	{
		// Token: 0x17001566 RID: 5478
		// (get) Token: 0x0600BE0A RID: 48650 RVA: 0x00563E80 File Offset: 0x00562080
		// (set) Token: 0x0600BE0B RID: 48651 RVA: 0x00563E88 File Offset: 0x00562088
		public string Value { get; set; }

		// Token: 0x17001567 RID: 5479
		// (get) Token: 0x0600BE0C RID: 48652 RVA: 0x00563E91 File Offset: 0x00562091
		public override SettingEntry.SettingType Type
		{
			get
			{
				return SettingEntry.SettingType.InputField;
			}
		}

		// Token: 0x0600BE0D RID: 48653 RVA: 0x00563E94 File Offset: 0x00562094
		public InputFieldSetting()
		{
		}

		// Token: 0x0600BE0E RID: 48654 RVA: 0x00563E9E File Offset: 0x0056209E
		public InputFieldSetting(string group, string key, string displayName, string description, string initialValue) : base(group, key, displayName, description)
		{
			this.Value = initialValue;
		}

		// Token: 0x0600BE0F RID: 48655 RVA: 0x00563EB8 File Offset: 0x005620B8
		public override SettingEntry Clone()
		{
			return new InputFieldSetting(this.GroupName, this.Key, this.DisplayName, this.Description, this.Value);
		}

		// Token: 0x0600BE10 RID: 48656 RVA: 0x00563EED File Offset: 0x005620ED
		public override void SaveToSerializableModData(SerializableModData settings)
		{
			settings.Set(this.Key, this.Value);
		}

		// Token: 0x0600BE11 RID: 48657 RVA: 0x00563F04 File Offset: 0x00562104
		public override void RestoreFromSerializableModData(SerializableModData settings)
		{
			string val;
			bool flag = settings.Get(this.Key, out val);
			if (flag)
			{
				this.Value = val;
			}
		}

		// Token: 0x0600BE12 RID: 48658 RVA: 0x00563F2C File Offset: 0x0056212C
		public override void SaveToLuaTable(Table luaTable)
		{
			luaTable.Save(this.Key, this.Value);
		}

		// Token: 0x0600BE13 RID: 48659 RVA: 0x00563F44 File Offset: 0x00562144
		public override void LoadFromLuaTable(Table luaTable)
		{
			bool flag = !luaTable.ContainsKey(this.Key);
			if (!flag)
			{
				string value = string.Empty;
				try
				{
					luaTable.Load(this.Key, out value);
				}
				catch (Exception e)
				{
					Debug.LogWarning("Failed to load setting value on key:" + this.Key + ", and use default value:" + value);
				}
				finally
				{
					this.Value = value;
				}
			}
		}

		// Token: 0x0600BE14 RID: 48660 RVA: 0x00563FCC File Offset: 0x005621CC
		public override void LoadDefaultSetting(Table settingEntryTable)
		{
			base.LoadDefaultSetting(settingEntryTable);
			string defaultValue;
			settingEntryTable.Load("DefaultValue", out defaultValue);
			this.Value = defaultValue;
		}

		// Token: 0x0600BE15 RID: 48661 RVA: 0x00563FF8 File Offset: 0x005621F8
		public override void SaveDefaultSetting(Table settingEntryTable)
		{
			base.SaveDefaultSetting(settingEntryTable);
			settingEntryTable.Save("DefaultValue", this.Value);
		}
	}
}
