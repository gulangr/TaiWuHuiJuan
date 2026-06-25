using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200039A RID: 922
public class UI_ResourceListCostConfirm : UIBase
{
	// Token: 0x0600378E RID: 14222 RVA: 0x001BF710 File Offset: 0x001BD910
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<List<ResourceInfo>>("ResourcesCost", out this._resourcesCost);
		argsBox.Get("ActionName", out this._actionName);
		argsBox.Get("ConfirmText", out this._confirmText);
		argsBox.Get("Content", out this._content);
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
		this._canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		this._canvasGroup.alpha = 0.01f;
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x001BF79A File Offset: 0x001BD99A
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			34U,
			66U
		}));
	}

	// Token: 0x06003790 RID: 14224 RVA: 0x001BF7CC File Offset: 0x001BD9CC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				bool flag = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0;
				if (flag)
				{
					uint subId = notification.Uid.SubId1;
					uint num = subId;
					if (num != 34U)
					{
						if (num == 66U)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
							this.Refresh();
						}
					}
					else
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
						this.Refresh();
					}
				}
			}
		}
	}

	// Token: 0x06003791 RID: 14225 RVA: 0x001BF8C8 File Offset: 0x001BDAC8
	private void Refresh()
	{
		this.Element.ShowAfterRefresh();
		bool canExecuteSkill = true;
		base.CGet<GameObject>("Time").SetActive(false);
		bool showResource = false;
		GameObject resourceHolder = base.CGet<GameObject>("ResourceHolder");
		int i;
		int j;
		for (i = 0; i < 8; i = j + 1)
		{
			Transform child = resourceHolder.transform.GetChild(i);
			bool isDisplay = this._resourcesCost.Any((ResourceInfo r) => (int)r.ResourceType == i);
			ResourceInfo resourceInfo = this._resourcesCost.Find((ResourceInfo r) => (int)r.ResourceType == i);
			bool flag = (int)resourceInfo.ResourceType == i && isDisplay;
			if (flag)
			{
				showResource = true;
				child.gameObject.SetActive(true);
				child.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(this._resources.Get(i), 100000).SetColor((this._resources.Get(i) < resourceInfo.ResourceCount) ? "brightred" : "brightblue"), resourceInfo.ResourceCount.ToString());
				bool flag2 = this._resources.Get(i) < resourceInfo.ResourceCount;
				if (flag2)
				{
					canExecuteSkill = false;
				}
			}
			else
			{
				child.gameObject.SetActive(false);
			}
			j = i;
		}
		bool noCost = !showResource;
		resourceHolder.SetActive(!noCost);
		bool activeSelf = resourceHolder.activeSelf;
		if (activeSelf)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(resourceHolder.GetComponent<RectTransform>());
		}
		base.CGet<TextMeshProUGUI>("Content").text = this._content;
		base.CGet<TextMeshProUGUI>("ConfirmText").text = this._confirmText;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		});
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
		{
			this._canvasGroup.alpha = 1f;
		});
		base.CGet<CButtonObsolete>("Confirm").interactable = canExecuteSkill;
	}

	// Token: 0x06003792 RID: 14226 RVA: 0x001BFAF1 File Offset: 0x001BDCF1
	private void OnCancelClick()
	{
		UIManager.Instance.HideUI(UIElement.ResourceListCostConfirm);
	}

	// Token: 0x06003793 RID: 14227 RVA: 0x001BFB04 File Offset: 0x001BDD04
	private void OnConfirmClick()
	{
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(UIElement.ResourceListCostConfirm);
	}

	// Token: 0x06003794 RID: 14228 RVA: 0x001BFB29 File Offset: 0x001BDD29
	public override void QuickHide()
	{
		base.QuickHide();
		this.OnCancelClick();
	}

	// Token: 0x06003795 RID: 14229 RVA: 0x001BFB3C File Offset: 0x001BDD3C
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "Confirm"))
		{
			if (a == "Cancel")
			{
				this.OnCancelClick();
			}
		}
		else
		{
			this.OnConfirmClick();
		}
	}

	// Token: 0x06003796 RID: 14230 RVA: 0x001BFB84 File Offset: 0x001BDD84
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && base.CGet<CButtonObsolete>("Confirm").interactable;
		if (flag)
		{
			this.OnConfirmClick();
		}
		else
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x04002823 RID: 10275
	private Action _onConfirm;

	// Token: 0x04002824 RID: 10276
	private ResourceInts _resources;

	// Token: 0x04002825 RID: 10277
	private int _exp;

	// Token: 0x04002826 RID: 10278
	private CanvasGroup _canvasGroup;

	// Token: 0x04002827 RID: 10279
	private List<ResourceInfo> _resourcesCost;

	// Token: 0x04002828 RID: 10280
	private string _actionName;

	// Token: 0x04002829 RID: 10281
	private string _confirmText;

	// Token: 0x0400282A RID: 10282
	private string _content;
}
