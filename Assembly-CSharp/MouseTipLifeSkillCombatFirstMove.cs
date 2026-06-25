using System;
using FrameWork;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class MouseTipLifeSkillCombatFirstMove : MouseTipBase
{
	// Token: 0x06002A7E RID: 10878 RVA: 0x00145894 File Offset: 0x00143A94
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
