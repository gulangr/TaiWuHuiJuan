using System;
using Config;

namespace Game.Views.MouseTips
{
	// Token: 0x0200082C RID: 2092
	public abstract class CommonTipBaseRuntime
	{
		// Token: 0x06006657 RID: 26199 RVA: 0x002EB6AC File Offset: 0x002E98AC
		protected CommonTipBaseRuntime(CommonTipItem configLine)
		{
			this.ConfigLine = configLine;
		}

		// Token: 0x17000C55 RID: 3157
		// (get) Token: 0x06006658 RID: 26200 RVA: 0x002EB6BD File Offset: 0x002E98BD
		public CommonTipItem ConfigLine { get; }

		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x06006659 RID: 26201 RVA: 0x002EB6C5 File Offset: 0x002E98C5
		// (set) Token: 0x0600665A RID: 26202 RVA: 0x002EB6CD File Offset: 0x002E98CD
		private protected ToolTipCommon Owner { protected get; private set; }

		// Token: 0x0600665B RID: 26203 RVA: 0x002EB6D8 File Offset: 0x002E98D8
		internal void Attach(ToolTipCommon owner)
		{
			bool flag = this.Owner == owner;
			if (!flag)
			{
				this.Detach();
				this.Owner = owner;
				this.OnAttached();
			}
		}

		// Token: 0x0600665C RID: 26204 RVA: 0x002EB70C File Offset: 0x002E990C
		internal void Detach()
		{
			bool flag = this.Owner == null;
			if (!flag)
			{
				this.OnDetaching();
				this.Owner = null;
			}
		}

		// Token: 0x0600665D RID: 26205
		public abstract string GetArgument(string key);

		// Token: 0x0600665E RID: 26206 RVA: 0x002EB73C File Offset: 0x002E993C
		public virtual bool ShouldShowParagraph(string name)
		{
			return true;
		}

		// Token: 0x0600665F RID: 26207 RVA: 0x002EB750 File Offset: 0x002E9950
		public virtual bool ShouldShowAtom(string paragraphName, string name)
		{
			return true;
		}

		// Token: 0x06006660 RID: 26208 RVA: 0x002EB763 File Offset: 0x002E9963
		protected void RefreshOwner()
		{
			ToolTipCommon owner = this.Owner;
			if (owner != null)
			{
				owner.Refresh();
			}
		}

		// Token: 0x06006661 RID: 26209 RVA: 0x002EB778 File Offset: 0x002E9978
		protected virtual void OnAttached()
		{
		}

		// Token: 0x06006662 RID: 26210 RVA: 0x002EB77B File Offset: 0x002E997B
		protected virtual void OnDetaching()
		{
		}
	}
}
