using System;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA2 RID: 3746
	public interface ICellContent<in TData> : ICellContent
	{
		// Token: 0x0600AD85 RID: 44421
		void SetData(TData data);

		// Token: 0x0600AD86 RID: 44422 RVA: 0x004F24CC File Offset: 0x004F06CC
		void SetSelected(bool selected)
		{
		}

		// Token: 0x0600AD87 RID: 44423 RVA: 0x004F24CF File Offset: 0x004F06CF
		bool SetDisabled(bool disabled)
		{
			return false;
		}
	}
}
