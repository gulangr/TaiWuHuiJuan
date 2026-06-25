using System;
using Game.Components.Item;

// Token: 0x02000229 RID: 553
public class ConfigItemViewWithColor : ConfigItemView
{
	// Token: 0x0600232F RID: 9007 RVA: 0x0010347D File Offset: 0x0010167D
	protected override void ApplyGradeBack(sbyte grade)
	{
		GradeBackVisual.ApplyTint(this._gradeBack, grade);
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x00103490 File Offset: 0x00101690
	protected override void ApplyGradeLabel(sbyte grade)
	{
		bool flag = this._grade != null;
		if (flag)
		{
			this._grade.gameObject.SetActive(false);
		}
	}
}
