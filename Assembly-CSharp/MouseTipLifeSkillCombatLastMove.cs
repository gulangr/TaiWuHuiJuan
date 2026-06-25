using System;
using FrameWork;
using UnityEngine;

// Token: 0x020002AD RID: 685
public class MouseTipLifeSkillCombatLastMove : MouseTipBase
{
	// Token: 0x06002A80 RID: 10880 RVA: 0x001458EC File Offset: 0x00143AEC
	protected override void Init(ArgumentBox argsBox)
	{
		GameObject effectLayout = base.CGet<GameObject>("EffectLayout");
		TMPTextSpriteHelper[] helpers = effectLayout.GetComponentsInChildren<TMPTextSpriteHelper>();
		bool flag = helpers != null;
		if (flag)
		{
			foreach (TMPTextSpriteHelper helper in helpers)
			{
				helper.Parse();
			}
		}
	}
}
