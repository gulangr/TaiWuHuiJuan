using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000365 RID: 869
public class UICostResourceConfirm : UIBase
{
	// Token: 0x0600325C RID: 12892 RVA: 0x0018D48B File Offset: 0x0018B68B
	public override void OnInit(ArgumentBox argsBox)
	{
		this.ReadArgs(argsBox);
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x0018D496 File Offset: 0x0018B696
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			34U
		}));
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x0018D4C4 File Offset: 0x0018B6C4
	private void Refresh()
	{
		this.titleLabel.text = this._title.GetString();
		this.desc1Label.text = this._desc1.GetString();
		this.desc2Label.text = this._desc2.GetString();
		this.RefreshResources();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x0018D52C File Offset: 0x0018B72C
	private unsafe void RefreshResources()
	{
		CommonUtils.PrepareEnoughChildren(this.resourceRoot, this.resourceTemplate.gameObject, this._needResources.Count, null);
		for (int i = 0; i < this._needResources.Count; i++)
		{
			ResourceInfo resource = this._needResources[i];
			Refers refers = this.resourceRoot.GetChild(i).GetComponent<Refers>();
			TextMeshProUGUI resourceLabel = refers.CGet<TextMeshProUGUI>("ResourceLabel");
			CImage resourceIcon = refers.CGet<CImage>("ResourceIcon");
			TextMeshProUGUI resourceTypeLabel = refers.CGet<TextMeshProUGUI>("ResourceTypeLabel");
			ResourceTypeItem config = Config.ResourceType.Instance[resource.ResourceType];
			resourceIcon.SetSprite(config.Icon, false, null);
			resourceTypeLabel.text = config.Name;
			int needCount = resource.ResourceCount;
			int haveCount = (this._haveResources != null) ? this._haveResources[i] : (*this._taiwuResources[(int)resource.ResourceType]);
			resourceLabel.text = CommonUtils.GetColoredStringByCompare(haveCount, needCount, needCount.CompareTo(haveCount), true);
		}
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x0018D650 File Offset: 0x0018B850
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 4 && uid.DataId == 0;
				if (flag)
				{
					uint subId = notification.Uid.SubId1;
					uint num = subId;
					if (num == 34U)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuResources);
						this.Refresh();
					}
				}
			}
		}
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x0018D720 File Offset: 0x0018B920
	private void ReadArgs(ArgumentBox argsBox)
	{
		bool flag = !argsBox.Get<StringKey>("Title", out this._title);
		if (flag)
		{
			this._title = LanguageKey.LK_ResourceConfirm_Title;
		}
		argsBox.Get<StringKey>("Desc1", out this._desc1);
		argsBox.Get<StringKey>("Desc2", out this._desc2);
		argsBox.Get<List<ResourceInfo>>("NeedResources", out this._needResources);
		argsBox.Get<List<int>>("HaveResources", out this._haveResources);
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x0018D7B4 File Offset: 0x0018B9B4
	protected override void OnClick(Transform btn)
	{
		UIManager.Instance.HideUI(this.Element);
		bool flag = btn.name == "Confirm";
		if (flag)
		{
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
		}
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x0018D7FC File Offset: 0x0018B9FC
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.confirmButton.interactable;
		if (flag)
		{
			Button.ButtonClickedEvent onClick = this.confirmButton.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
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

	// Token: 0x040024E8 RID: 9448
	private StringKey _title;

	// Token: 0x040024E9 RID: 9449
	private StringKey _desc1;

	// Token: 0x040024EA RID: 9450
	private StringKey _desc2;

	// Token: 0x040024EB RID: 9451
	private List<ResourceInfo> _needResources;

	// Token: 0x040024EC RID: 9452
	private List<int> _haveResources;

	// Token: 0x040024ED RID: 9453
	private Action _onConfirm;

	// Token: 0x040024EE RID: 9454
	private ResourceInts _taiwuResources;

	// Token: 0x040024EF RID: 9455
	[SerializeField]
	private TextMeshProUGUI titleLabel;

	// Token: 0x040024F0 RID: 9456
	[SerializeField]
	private RectTransform resourceRoot;

	// Token: 0x040024F1 RID: 9457
	[SerializeField]
	private Refers resourceTemplate;

	// Token: 0x040024F2 RID: 9458
	[SerializeField]
	private TextMeshProUGUI desc1Label;

	// Token: 0x040024F3 RID: 9459
	[SerializeField]
	private TextMeshProUGUI desc2Label;

	// Token: 0x040024F4 RID: 9460
	[SerializeField]
	private CButtonObsolete confirmButton;
}
