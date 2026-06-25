using System;
using Game.Components.Avatar;
using Spine.Unity;
using UnityEngine;

// Token: 0x020003C5 RID: 965
public abstract class MapBlockCharAlive2 : MapBlockCharBase2
{
	// Token: 0x06003A48 RID: 14920 RVA: 0x001DA6DC File Offset: 0x001D88DC
	protected virtual void RefreshAvatar()
	{
		bool flag = this.DisplayData == null;
		if (flag)
		{
			this.avatar.ResetToBlank(false);
		}
		else
		{
			this.avatar.Refresh(this.DisplayData, true);
		}
		this.avatar.gameObject.SetActive(true);
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x001DA72B File Offset: 0x001D892B
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshAvatar();
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x001DA73C File Offset: 0x001D893C
	protected string GetMerchantLevelImage(sbyte level)
	{
		return string.Format("blockchar_shoplevel_{0}", level);
	}

	// Token: 0x04002A07 RID: 10759
	[SerializeField]
	protected Game.Components.Avatar.Avatar avatar;

	// Token: 0x04002A08 RID: 10760
	[SerializeField]
	protected TooltipInvoker extraMouseTipDisplay;

	// Token: 0x04002A09 RID: 10761
	[SerializeField]
	protected CircleGenerator circleGenerator;

	// Token: 0x04002A0A RID: 10762
	[SerializeField]
	protected SkeletonGraphic skeletonGraphic;
}
