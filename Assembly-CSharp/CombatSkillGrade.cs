using System;
using System.Runtime.CompilerServices;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class CombatSkillGrade : MonoBehaviour
{
	// Token: 0x06001FA0 RID: 8096 RVA: 0x000E667C File Offset: 0x000E487C
	public void Set(sbyte grade)
	{
		bool flag = grade < 0 || grade >= 9;
		if (flag)
		{
			PredefinedLog.Show(13, string.Format("CombatSkillGrade.Set {0}", grade));
		}
		else
		{
			this.icon.SetSprite(CombatSkillGrade.GradeIconNames[(int)grade], false, null);
			this.name.text = LocalStringManager.Get(CombatSkillGrade.GradeNameLanguageKey[(int)grade]);
		}
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000E66F0 File Offset: 0x000E48F0
	// Note: this type is marked as 'beforefieldinit'.
	static CombatSkillGrade()
	{
		LanguageKey[] array = new LanguageKey[9];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.DE0C2B0D46932EB1666686AC3BAB0DD1482E06C5A49DC78F6DF59AD319B077F2).FieldHandle);
		CombatSkillGrade.GradeNameLanguageKey = array;
	}

	// Token: 0x040017C4 RID: 6084
	[SerializeField]
	private CImage icon;

	// Token: 0x040017C5 RID: 6085
	[SerializeField]
	private new TextMeshProUGUI name;

	// Token: 0x040017C6 RID: 6086
	private static readonly string[] GradeIconNames = new string[]
	{
		"sp_icon_pinji_0",
		"sp_icon_pinji_1",
		"sp_icon_pinji_2",
		"sp_icon_pinji_3",
		"sp_icon_pinji_4",
		"sp_icon_pinji_5",
		"sp_icon_pinji_6",
		"sp_icon_pinji_7",
		"sp_icon_pinji_8"
	};

	// Token: 0x040017C7 RID: 6087
	private static readonly LanguageKey[] GradeNameLanguageKey;
}
