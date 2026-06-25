using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Domains.Combat;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;

// Token: 0x02000281 RID: 641
public class MouseTipCombatChangeTrick : MouseTipBase
{
	// Token: 0x06002951 RID: 10577 RVA: 0x00134C7A File Offset: 0x00132E7A
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x00134CB7 File Offset: 0x00132EB7
	private void OnListenerIdReady()
	{
		CombatDomainMethod.Call.GetChangeTrickDisplayData(this.Element.GameDataListenerId);
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x00134CCC File Offset: 0x00132ECC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.DomainId == 8 && notification.MethodId == 66;
			if (flag)
			{
				ChangeTrickDisplayData displayData = default(ChangeTrickDisplayData);
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayData);
				this.HandlerMethodGetChangeTrickDisplayData(displayData);
			}
		}
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x00134D60 File Offset: 0x00132F60
	private void HandlerMethodGetChangeTrickDisplayData(ChangeTrickDisplayData displayData)
	{
		base.CGet<TextMeshProUGUI>("CostCount").text = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Change_Trick_Cost_Count, displayData.CostCount, displayData.CanChangeTrick ? "brightblue" : "brightred").ColorReplace();
		base.CGet<TextMeshProUGUI>("AddHitRate").text = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Change_Trick_Add_Hit_Rate, displayData.AddHitRate).ColorReplace();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x00134DE4 File Offset: 0x00132FE4
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.PinpointStrikes);
		}
	}
}
