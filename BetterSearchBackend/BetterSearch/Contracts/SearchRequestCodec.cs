using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace BetterSearch.Contracts
{
	// Token: 0x02000004 RID: 4
	[NullableContext(1)]
	[Nullable(0)]
	public static class SearchRequestCodec
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static string Encode(SearchScope scope, short areaId, short blockId, string keyword)
		{
			string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyword ?? string.Empty));
			string[] array = new string[8];
			array[0] = "BetterSearchScope|";
			int num = 1;
			int num2 = (int)scope;
			array[num] = num2.ToString();
			array[2] = "|";
			array[3] = areaId.ToString();
			array[4] = "|";
			array[5] = blockId.ToString();
			array[6] = "|";
			array[7] = text;
			return string.Concat(array);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020C8 File Offset: 0x000002C8
		public static bool TryDecode(string value, out SearchRequest request)
		{
			request = default(SearchRequest);
			if (string.IsNullOrEmpty(value) || !value.StartsWith("BetterSearchScope|", StringComparison.Ordinal))
			{
				return false;
			}
			string[] array = value.Substring("BetterSearchScope|".Length).Split(new char[]
			{
				'|'
			}, 4);
			if (array.Length != 4)
			{
				return false;
			}
			int num;
			short areaId;
			short blockId;
			if (!int.TryParse(array[0], out num) || !short.TryParse(array[1], out areaId) || !short.TryParse(array[2], out blockId) || !Enum.IsDefined(typeof(SearchScope), num))
			{
				return false;
			}
			bool result;
			try
			{
				request.Scope = (SearchScope)num;
				request.AreaId = areaId;
				request.BlockId = blockId;
				request.Keyword = Encoding.UTF8.GetString(Convert.FromBase64String(array[3]));
				result = true;
			}
			catch
			{
				request = default(SearchRequest);
				result = false;
			}
			return result;
		}

		// Token: 0x0400000A RID: 10
		public const ushort ScopedCharacterSearchMethodId = 65001;

		// Token: 0x0400000B RID: 11
		public const ushort ScopedCharacterSearchIdsMethodId = 65002;

		// Token: 0x0400000C RID: 12
		private const string Prefix = "BetterSearchScope|";
	}
}
