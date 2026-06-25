using System;

namespace Game.Components.Item
{
	// Token: 0x02000EF0 RID: 3824
	public static class GradeBackVisual
	{
		// Token: 0x0600AF6B RID: 44907 RVA: 0x004FF2FC File Offset: 0x004FD4FC
		public static void ApplyTint(CImage gradeBack, sbyte grade)
		{
			bool flag = gradeBack == null;
			if (!flag)
			{
				bool flag2 = grade < 0;
				if (flag2)
				{
					gradeBack.enabled = false;
				}
				else
				{
					gradeBack.color = Colors.Instance.GradeColors[(int)grade];
					gradeBack.enabled = true;
				}
			}
		}
	}
}
