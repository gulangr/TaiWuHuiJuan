using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using FrameWork;
using GameData.Domains.Character.Display;

// Token: 0x0200013C RID: 316
public class NameCenter
{
	// Token: 0x060010BB RID: 4283 RVA: 0x00063E0C File Offset: 0x0006200C
	public static string GetDisplayName(ref NameRelatedData data, bool isTaiwu)
	{
		ValueTuple<string, string> name = data.GetDisplayName(isTaiwu);
		return NameCenter.FormatName(name.Item1, name.Item2);
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x00063E38 File Offset: 0x00062038
	public static string GetRealName(ref NameRelatedData data)
	{
		ValueTuple<string, string> name = data.GetRealName();
		return NameCenter.FormatName(name.Item1, name.Item2);
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x00063E64 File Offset: 0x00062064
	public static bool SearchTextInDisplayName(ref NameRelatedData data, string text, bool isTaiwu)
	{
		ValueTuple<string, string> name = data.GetDisplayName(isTaiwu);
		bool flag = name.Item2 != null && name.Item2.Contains(text);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = name.Item1 != null && name.Item1.Contains(text);
			result = (flag2 || NameCenter.FormatName(name.Item1, name.Item2).Contains(text));
		}
		return result;
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x00063ED4 File Offset: 0x000620D4
	public static string GetMonasticTitle(ref NameRelatedData data, bool isTaiwu)
	{
		return data.GetMonasticTitle(isTaiwu);
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00063EF0 File Offset: 0x000620F0
	public static string GetNameByDisplayData(CharacterDisplayData displayData, bool isTaiwu, bool getRealName)
	{
		NameRelatedData data = NameCenter.GetNameRelatedData(displayData);
		return getRealName ? NameCenter.GetRealName(ref data) : NameCenter.GetDisplayName(ref data, isTaiwu);
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x00063F20 File Offset: 0x00062120
	public static string GetMonasticTitleOrDisplayName(ref NameRelatedData data, bool isTaiwu, bool ignoreNickName = false)
	{
		ValueTuple<string, string> name = data.GetMonasticTitleOrDisplayNameDetailed(isTaiwu, ignoreNickName);
		return NameCenter.FormatName(name.Item1, name.Item2);
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x00063F4C File Offset: 0x0006214C
	public static string GetMonasticTitleOrDisplayName(CharacterDisplayData displayData, bool isTaiwu)
	{
		NameRelatedData data = NameCenter.GetNameRelatedData(displayData);
		return NameCenter.GetMonasticTitleOrDisplayName(ref data, isTaiwu, false);
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x00063F70 File Offset: 0x00062170
	public static string GetMonasticTitleOrDisplayName(GraveDisplayData displayData, bool isTaiwu)
	{
		return NameCenter.GetMonasticTitleOrDisplayName(ref displayData.NameData, isTaiwu, false);
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00063F90 File Offset: 0x00062190
	public static string GetNameSequenceStringByNameRelatedDataList(List<NameRelatedData> nameList, bool includeTaiwu = false)
	{
		StringBuilder teammateNameBuilder = EasyPool.Get<StringBuilder>();
		teammateNameBuilder.Clear();
		for (int i = 0; i < nameList.Count; i++)
		{
			bool flag = i > 0;
			if (flag)
			{
				teammateNameBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
			}
			NameRelatedData nameRelatedData = nameList[i];
			teammateNameBuilder.Append(NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, includeTaiwu && i == 0, false));
		}
		string teammateNames = teammateNameBuilder.ToString();
		EasyPool.Free<StringBuilder>(teammateNameBuilder);
		return teammateNames;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00064018 File Offset: 0x00062218
	public static NameRelatedData GetNameRelatedData(CharacterDisplayData displayData)
	{
		NameRelatedData data;
		data.CharTemplateId = displayData.TemplateId;
		data.Gender = displayData.Gender;
		data.MonkType = displayData.MonkType;
		data.FullName = displayData.FullName;
		data.OrgTemplateId = displayData.OrgInfo.OrgTemplateId;
		data.OrgGrade = displayData.OrgInfo.Grade;
		data.MonasticTitle = displayData.MonasticTitle;
		data.CustomDisplayNameId = displayData.CustomDisplayNameId;
		data.NickNameId = displayData.NickNameId;
		data.ExtraNameTextTemplateId = displayData.ExtraNameTextTemplateId;
		return data;
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x000640B8 File Offset: 0x000622B8
	[Obsolete("Use GetMonasticTitleOrDisplayName instead.")]
	public static string GetCharMonasticTitleOrNameByDisplayData(CharacterDisplayData displayData, bool isTaiwu, bool getRealName = false)
	{
		NameRelatedData data;
		data.CharTemplateId = displayData.TemplateId;
		data.Gender = displayData.Gender;
		data.MonkType = displayData.MonkType;
		data.FullName = displayData.FullName;
		data.OrgTemplateId = displayData.OrgInfo.OrgTemplateId;
		data.OrgGrade = displayData.OrgInfo.Grade;
		data.MonasticTitle = displayData.MonasticTitle;
		data.CustomDisplayNameId = displayData.CustomDisplayNameId;
		data.NickNameId = displayData.NickNameId;
		data.ExtraNameTextTemplateId = displayData.ExtraNameTextTemplateId;
		ValueTuple<string, string> monasticTitleOrName = NameCenter.GetMonasticTitleOrName(ref data, isTaiwu, getRealName);
		string surname = monasticTitleOrName.Item1;
		string givenName = monasticTitleOrName.Item2;
		return NameCenter.FormatName(surname, givenName);
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00064174 File Offset: 0x00062374
	[Obsolete("Use GetMonasticTitleOrDisplayName instead.")]
	public static string GetCharMonasticTitleAndNameByDisplayData(CharacterDisplayData displayData, bool isTaiwu, bool getRealName)
	{
		NameRelatedData data;
		data.CharTemplateId = displayData.TemplateId;
		data.Gender = displayData.Gender;
		data.MonkType = displayData.MonkType;
		data.FullName = displayData.FullName;
		data.OrgTemplateId = displayData.OrgInfo.OrgTemplateId;
		data.OrgGrade = displayData.OrgInfo.Grade;
		data.MonasticTitle = displayData.MonasticTitle;
		data.CustomDisplayNameId = displayData.CustomDisplayNameId;
		data.NickNameId = displayData.NickNameId;
		data.ExtraNameTextTemplateId = displayData.ExtraNameTextTemplateId;
		return NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref data, isTaiwu, getRealName);
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x0006421C File Offset: 0x0006241C
	[Obsolete("Use GetMonasticTitleOrDisplayName instead.")]
	public static string GetCharMonasticTitleAndNameByNameRelatedData(ref NameRelatedData data, bool isTaiwu, bool getRealName)
	{
		ValueTuple<string, string> charName = NameCenter.GetMonasticTitleOrName(ref data, isTaiwu, getRealName);
		return NameCenter.FormatName(charName.Item1, charName.Item2);
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x00064248 File Offset: 0x00062448
	[Obsolete("Use GetMonasticTitleOrDisplayName instead.")]
	public static string GetCharMonasticTitleOrNameByGraveData(GraveDisplayData displayData, bool isTaiwu, bool getRealName = false)
	{
		ValueTuple<string, string> monasticTitleOrName = NameCenter.GetMonasticTitleOrName(ref displayData.NameData, isTaiwu, getRealName);
		string surname = monasticTitleOrName.Item1;
		string givenName = monasticTitleOrName.Item2;
		return NameCenter.FormatName(surname, givenName);
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x0006427C File Offset: 0x0006247C
	[Obsolete]
	[return: TupleElementNames(new string[]
	{
		"surname",
		"givenName"
	})]
	public static ValueTuple<string, string> GetMonasticTitleOrName(ref NameRelatedData data, bool isTaiwu, bool getRealName = false)
	{
		ValueTuple<string, string> result;
		if (getRealName)
		{
			result = data.GetRealName();
		}
		else
		{
			result = data.GetMonasticTitleOrDisplayName(isTaiwu);
		}
		return result;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x000642A4 File Offset: 0x000624A4
	[Obsolete("Use GetDisplayName or GetRealName instead.")]
	[return: TupleElementNames(new string[]
	{
		"surname",
		"givenName"
	})]
	public static ValueTuple<string, string> GetName(ref NameRelatedData data, bool isTaiwu, bool getRealName = false)
	{
		bool flag = data.CharTemplateId < 0;
		ValueTuple<string, string> result;
		if (flag)
		{
			result = new ValueTuple<string, string>(null, LocalStringManager.Get(LanguageKey.LK_UnknownCharName));
		}
		else if (getRealName)
		{
			result = data.GetRealName();
		}
		else
		{
			result = data.GetDisplayName(isTaiwu);
		}
		return result;
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x000642EC File Offset: 0x000624EC
	public static bool HasInvalidCharForName(string nameString)
	{
		bool flag = LocalStringManager.CurLanguageKey == "CN";
		if (flag)
		{
			bool flag2 = NameCenter.JapaneseCharRegex.IsMatch(nameString);
			if (flag2)
			{
				return true;
			}
			bool flag3 = NameCenter.EnglishCharRegex.IsMatch(nameString);
			if (flag3)
			{
				return true;
			}
		}
		return NameCenter.InvalidCharRegex.IsMatch(nameString);
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x00064344 File Offset: 0x00062544
	public static string FormatName(string surname, string givenName)
	{
		return LocalStringManager.FormatFullName(surname, givenName);
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x00064350 File Offset: 0x00062550
	public static int GetMaxSurnameLength()
	{
		LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
		bool flag = curLanguageType == LocalStringManager.LanguageType.CN || curLanguageType == LocalStringManager.LanguageType.CNH;
		int result;
		if (flag)
		{
			result = (int)GlobalConfig.Instance.NameLengthConfig_CN[0];
		}
		else
		{
			result = (int)GlobalConfig.Instance.NameLengthConfig_EN[0];
		}
		return result;
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x00064398 File Offset: 0x00062598
	public static int GetMaxNameLength()
	{
		LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
		bool flag = curLanguageType == LocalStringManager.LanguageType.CN || curLanguageType == LocalStringManager.LanguageType.CNH;
		int result;
		if (flag)
		{
			result = (int)GlobalConfig.Instance.NameLengthConfig_CN[1];
		}
		else
		{
			result = (int)GlobalConfig.Instance.NameLengthConfig_EN[1];
		}
		return result;
	}

	// Token: 0x04000EE9 RID: 3817
	private static readonly Regex InvalidCharRegex = new Regex("[\\u0022\\s~!@#$%^&\\*\\(\\)_\\+\\-=\\[\\]\\{\\}\\\\\\|;:',.<>\\/\\?·`！￥…（）—、【】：；‘’“”《》，。？]");

	// Token: 0x04000EEA RID: 3818
	private static readonly Regex EnglishCharRegex = new Regex("[A-Z|a-z|0-9]");

	// Token: 0x04000EEB RID: 3819
	private static readonly Regex JapaneseCharRegex = new Regex("[ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ゛゜ゝゞァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾ]");
}
