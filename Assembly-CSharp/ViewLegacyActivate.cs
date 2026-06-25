using System;
using FrameWork;
using GameData.Domains.Global;

// Token: 0x020003DE RID: 990
public class ViewLegacyActivate : UIBase
{
	// Token: 0x06003B9A RID: 15258 RVA: 0x001E30F8 File Offset: 0x001E12F8
	public override void OnInit(ArgumentBox argsBox)
	{
	}

	// Token: 0x06003B9B RID: 15259 RVA: 0x001E30FC File Offset: 0x001E12FC
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, true, false, true, false) || CommonCommandKit.LeftMouse.Check(this.Element, false, true, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, true, false, true, false);
		if (flag)
		{
			this.QuickHide();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(319);
		}
	}
}
