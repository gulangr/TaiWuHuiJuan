using System;
using Game.Components.Item;
using TMPro;

// Token: 0x02000231 RID: 561
public class ItemViewWithColor : ItemView
{
	// Token: 0x060023E8 RID: 9192 RVA: 0x00107A70 File Offset: 0x00105C70
	protected override void ApplyGradeBack(CImage gradeBack, bool showGrade, sbyte grade)
	{
		bool flag = gradeBack == null;
		if (!flag)
		{
			gradeBack.gameObject.SetActive(showGrade);
			bool flag2 = !showGrade;
			if (!flag2)
			{
				GradeBackVisual.ApplyTint(gradeBack, grade);
			}
		}
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x00107AAC File Offset: 0x00105CAC
	protected override void ApplyGradeLabel(TextMeshProUGUI gradeLabel, bool showGrade, sbyte grade)
	{
		bool flag = gradeLabel != null;
		if (flag)
		{
			gradeLabel.gameObject.SetActive(false);
		}
	}
}
