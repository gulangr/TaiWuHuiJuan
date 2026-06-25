using System;
using System.Collections.Generic;
using System.Text;

namespace FrameWork
{
	// Token: 0x02000FE7 RID: 4071
	public static class Utils_Sorting
	{
		// Token: 0x0600B9E2 RID: 47586 RVA: 0x0054ADA8 File Offset: 0x00548FA8
		public static int CompareByCurrentLangEncoding(string strA, string strB)
		{
			return Utils_Sorting.CompareByEncoding(strA, strB, Utils_Sorting.LanguageEncodingDict[LocalStringManager.CurLanguageKey]);
		}

		// Token: 0x0600B9E3 RID: 47587 RVA: 0x0054ADD0 File Offset: 0x00548FD0
		public static int CompareByEncoding(string strA, string strB, string encodingName)
		{
			Encoding encoding = Encoding.GetEncoding(encodingName);
			return Utils_Sorting.CompareByEncoding(strA, strB, encoding);
		}

		// Token: 0x0600B9E4 RID: 47588 RVA: 0x0054ADF4 File Offset: 0x00548FF4
		public static int CompareByEncoding(string strA, string strB, Encoding encoding)
		{
			byte[] strABytes = encoding.GetBytes(strA);
			byte[] strBBytes = encoding.GetBytes(strB);
			int i = 0;
			while (i < strBBytes.Length)
			{
				bool flag = strABytes.Length <= i;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					int comparison = strABytes[i].CompareTo(strBBytes[i]);
					bool flag2 = comparison == 0;
					if (flag2)
					{
						i++;
						continue;
					}
					result = comparison;
				}
				return result;
			}
			return (strABytes.Length > strBBytes.Length) ? 1 : 0;
		}

		// Token: 0x04008FBF RID: 36799
		private static readonly Dictionary<string, Encoding> LanguageEncodingDict = new Dictionary<string, Encoding>
		{
			{
				"CN",
				Encoding.GetEncoding("GBK")
			},
			{
				"CNH",
				Encoding.GetEncoding("GBK")
			},
			{
				"EN",
				Encoding.ASCII
			},
			{
				"JP",
				Encoding.Unicode
			},
			{
				"KO",
				Encoding.Unicode
			}
		};
	}
}
