using System;
using System.Linq;
using Config;

namespace Game.Views.World
{
	// Token: 0x02000728 RID: 1832
	public class GlobalSettingsChecker
	{
		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x0600579A RID: 22426 RVA: 0x0028B286 File Offset: 0x00289486
		// (set) Token: 0x0600579B RID: 22427 RVA: 0x0028B28E File Offset: 0x0028948E
		public bool HideAll
		{
			get
			{
				return this.ForceHide;
			}
			set
			{
				this.ForceHide = false;
				Array.Fill<bool>(this.Enabled, !value);
			}
		}

		// Token: 0x0600579C RID: 22428 RVA: 0x0028B2A8 File Offset: 0x002894A8
		public void Set(GlobalSettings global)
		{
			this._global = global;
			Array.Fill<bool>(this.Enabled, true);
			foreach (string data in global.AreaStateDisabledData.Split(',', StringSplitOptions.RemoveEmptyEntries))
			{
				int index;
				bool flag = int.TryParse(data, out index);
				if (flag)
				{
					this.Enabled[index] = false;
				}
			}
		}

		// Token: 0x17000A8E RID: 2702
		public bool this[int index]
		{
			get
			{
				return this.Enabled[index];
			}
			set
			{
				bool flag = this.Enabled[index] == value;
				if (!flag)
				{
					this.Enabled[index] = value;
					this._global.AreaStateDisabledData = string.Join<string>(',', from x in this.Enabled.Select((bool x, int i) => x ? string.Empty : i.ToString())
					where !string.IsNullOrEmpty(x)
					select x);
				}
			}
		}

		// Token: 0x04003C1A RID: 15386
		public bool ForceHide;

		// Token: 0x04003C1B RID: 15387
		public bool[] Enabled = new bool[MapLegend.Instance.Count];

		// Token: 0x04003C1C RID: 15388
		private GlobalSettings _global;
	}
}
