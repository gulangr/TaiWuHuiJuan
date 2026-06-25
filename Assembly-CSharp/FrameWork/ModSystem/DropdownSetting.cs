using System;
using System.Collections.Generic;
using GameData.Domains.Mod;
using GameData.Utilities;
using MoonSharp.Interpreter;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x02001044 RID: 4164
	public class DropdownSetting : SettingEntry, ISettingValueWrapper<int>
	{
		// Token: 0x17001564 RID: 5476
		// (get) Token: 0x0600BDFE RID: 48638 RVA: 0x00563CA0 File Offset: 0x00561EA0
		// (set) Token: 0x0600BDFF RID: 48639 RVA: 0x00563CA8 File Offset: 0x00561EA8
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = MathUtils.Clamp(value, 0, this.Options.Count - 1);
			}
		}

		// Token: 0x17001565 RID: 5477
		// (get) Token: 0x0600BE00 RID: 48640 RVA: 0x00563CC4 File Offset: 0x00561EC4
		public override SettingEntry.SettingType Type
		{
			get
			{
				return SettingEntry.SettingType.Dropdown;
			}
		}

		// Token: 0x0600BE01 RID: 48641 RVA: 0x00563CC7 File Offset: 0x00561EC7
		public override void SaveToSerializableModData(SerializableModData settings)
		{
			settings.Set(this.Key, this.Value);
		}

		// Token: 0x0600BE02 RID: 48642 RVA: 0x00563CE0 File Offset: 0x00561EE0
		public override void RestoreFromSerializableModData(SerializableModData settings)
		{
			int val;
			bool flag = settings.Get(this.Key, out val);
			if (flag)
			{
				this.Value = val;
			}
		}

		// Token: 0x0600BE03 RID: 48643 RVA: 0x00563D08 File Offset: 0x00561F08
		public override void SaveToLuaTable(Table luaTable)
		{
			luaTable.Save(this.Key, this.Value);
		}

		// Token: 0x0600BE04 RID: 48644 RVA: 0x00563D20 File Offset: 0x00561F20
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

		// Token: 0x0600BE05 RID: 48645 RVA: 0x00563DA4 File Offset: 0x00561FA4
		public DropdownSetting()
		{
		}

		// Token: 0x0600BE06 RID: 48646 RVA: 0x00563DAE File Offset: 0x00561FAE
		public DropdownSetting(string group, string key, string displayName, string description, int initialValue, List<string> options) : base(group, key, displayName, description)
		{
			this.Options = new List<string>(options);
			this.Value = initialValue;
		}

		// Token: 0x0600BE07 RID: 48647 RVA: 0x00563DD4 File Offset: 0x00561FD4
		public override SettingEntry Clone()
		{
			return new DropdownSetting(this.GroupName, this.Key, this.DisplayName, this.Description, this.Value, this.Options);
		}

		// Token: 0x0600BE08 RID: 48648 RVA: 0x00563E10 File Offset: 0x00562010
		public override void LoadDefaultSetting(Table settingEntryTable)
		{
			base.LoadDefaultSetting(settingEntryTable);
			List<string> options;
			settingEntryTable.Load("Options", out options);
			int defaultValue;
			settingEntryTable.Load("DefaultValue", out defaultValue);
			this.Options = options;
			this.Value = defaultValue;
		}

		// Token: 0x0600BE09 RID: 48649 RVA: 0x00563E51 File Offset: 0x00562051
		public override void SaveDefaultSetting(Table settingEntryTable)
		{
			base.SaveDefaultSetting(settingEntryTable);
			settingEntryTable.Save("Options", this.Options);
			settingEntryTable.Save("DefaultValue", this.Value);
		}

		// Token: 0x04009220 RID: 37408
		public List<string> Options;

		// Token: 0x04009221 RID: 37409
		private int _value;
	}
}
