using System;
using Config;

namespace Game.Views.MouseTips
{
	// Token: 0x0200082F RID: 2095
	public static class CommonTipExtensions
	{
		// Token: 0x06006678 RID: 26232 RVA: 0x002EBACC File Offset: 0x002E9CCC
		public static CommonTipSimpleRuntime BuildSimple(this CommonTipItem item)
		{
			return new CommonTipSimpleRuntime(item);
		}
	}
}
