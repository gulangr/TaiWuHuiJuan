using System;
using GameData.Domains.Mod;
using MoonSharp.Interpreter;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x0200104C RID: 4172
	public class ToggleSetting : SettingEntry, ISettingValueWrapper<bool>
	{
		// Token: 0x17001572 RID: 5490
		// (get) Token: 0x0600BE6A RID: 48746 RVA: 0x005656EC File Offset: 0x005638EC
		// (set) Token: 0x0600BE6B RID: 48747 RVA: 0x005656F4 File Offset: 0x005638F4
		public bool Value { get; set; }

		// Token: 0x17001573 RID: 5491
		// (get) Token: 0x0600BE6C RID: 48748 RVA: 0x005656FD File Offset: 0x005638FD
		public override SettingEntry.SettingType Type
		{
			get
			{
				return SettingEntry.SettingType.Toggle;
			}
		}

		// Token: 0x0600BE6D RID: 48749 RVA: 0x00565700 File Offset: 0x00563900
		public ToggleSetting()
		{
		}

		// Token: 0x0600BE6E RID: 48750 RVA: 0x0056570A File Offset: 0x0056390A
		public ToggleSetting(string group, string key, string displayName, string description, bool initValue) : base(group, key, displayName, description)
		{
			this.Value = initValue;
		}

		// Token: 0x0600BE6F RID: 48751 RVA: 0x00565724 File Offset: 0x00563924
		public override SettingEntry Clone()
		{
			return new ToggleSetting(this.GroupName, this.Key, this.DisplayName, this.Description, this.Value);
		}

		// Token: 0x0600BE70 RID: 48752 RVA: 0x00565759 File Offset: 0x00563959
		public override void SaveToSerializableModData(SerializableModData settings)
		{
			settings.Set(this.Key, this.Value);
		}

		// Token: 0x0600BE71 RID: 48753 RVA: 0x00565770 File Offset: 0x00563970
		public override void RestoreFromSerializableModData(SerializableModData settings)
		{
			bool val;
			bool flag = settings.Get(this.Key, out val);
			if (flag)
			{
				this.Value = val;
			}
		}

		// Token: 0x0600BE72 RID: 48754 RVA: 0x00565798 File Offset: 0x00563998
		public override void SaveToLuaTable(Table luaTable)
		{
			luaTable.Save(this.Key, this.Value);
		}

		// Token: 0x0600BE73 RID: 48755 RVA: 0x005657B0 File Offset: 0x005639B0
		public override void LoadFromLuaTable(Table luaTable)
		{
			bool flag = !luaTable.ContainsKey(this.Key);
			if (!flag)
			{
				bool value = false;
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

		// Token: 0x0600BE74 RID: 48756 RVA: 0x00565834 File Offset: 0x00563A34
		public override void LoadDefaultSetting(Table settingEntryTable)
		{
			base.LoadDefaultSetting(settingEntryTable);
			bool defaultValue;
			settingEntryTable.Load("DefaultValue", out defaultValue);
			this.Value = defaultValue;
		}

		// Token: 0x0600BE75 RID: 48757 RVA: 0x00565860 File Offset: 0x00563A60
		public override void SaveDefaultSetting(Table settingEntryTable)
		{
			base.SaveDefaultSetting(settingEntryTable);
			settingEntryTable.Save("DefaultValue", this.Value);
		}
	}
}
