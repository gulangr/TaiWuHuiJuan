using System;

// Token: 0x0200013D RID: 317
public static class NumberFormatUtils
{
	// Token: 0x060010D1 RID: 4305 RVA: 0x00064417 File Offset: 0x00062617
	public static string FormatItemWeight(int weight)
	{
		return (weight % 10 == 0) ? string.Format("{0:f1}", (float)weight / 100f) : string.Format("{0:f2}", (float)weight / 100f);
	}
}
