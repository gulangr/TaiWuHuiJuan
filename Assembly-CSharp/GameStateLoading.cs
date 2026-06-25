using System;
using FrameWork;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class GameStateLoading : GameStateBase
{
	// Token: 0x06000841 RID: 2113 RVA: 0x0003801F File Offset: 0x0003621F
	public GameStateLoading(Enum stateName) : base(stateName)
	{
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0003802C File Offset: 0x0003622C
	public override void OnEnter(ArgumentBox argsBox)
	{
		base.OnEnter(argsBox);
		GEvent.OnEvent(UiEvents.UnloadPackers, null);
		UIElement loading = UIElement.Loading;
		loading.OnHide = (Action)Delegate.Combine(loading.OnHide, new Action(delegate()
		{
			bool exist = UIElement.PermanentTips.Exist;
			if (exist)
			{
				UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().ClearAllPermanentTips();
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}));
		UIElement.Loading.SetOnInitArgs(argsBox);
		UIElement.Loading.Show();
	}
}
