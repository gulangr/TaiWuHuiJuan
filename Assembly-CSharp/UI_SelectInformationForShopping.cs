using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Information;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public class UI_SelectInformationForShopping : UI_SelectInformation
{
	// Token: 0x06003809 RID: 14345 RVA: 0x001C32B8 File Offset: 0x001C14B8
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = !this._awakeDone;
		if (flag)
		{
			this.Awake();
		}
		string charName;
		bool flag2 = argsBox.Get("characterName", out charName);
		if (flag2)
		{
			base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_SecretInformationForCharacter, charName);
		}
		SecretInformationDisplayPackage package;
		bool flag3 = argsBox.Get<SecretInformationDisplayPackage>("secretInformation", out package);
		if (flag3)
		{
			bool flag4 = null != this.InformationSortAndFilter.SecretInformationTimeFilter;
			if (flag4)
			{
				this.InformationSortAndFilter.SecretInformationTimeFilter.gameObject.SetActive(false);
				this.InformationSortAndFilter.SecretInformationTimeFilter.InformationFilter = null;
			}
			this.InformationSortAndFilter.SecretInformationTimeFilter = null;
			this.InformationSortAndFilter.SetIsShowSecret(true);
			bool flag5 = null != this.InformationSortAndFilter.InformationFilter;
			if (flag5)
			{
				this._informationFilter = this.InformationSortAndFilter.InformationFilter;
			}
			this._informationFilter.SetTogglesVisibleState(new bool[]
			{
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				true
			});
			this._informationFilter.SetCount(this._informationFilter.ToggleBroadcastSecret, package.SecretInformationDisplayDataList.Count);
			this.InformationSortAndFilter.InformationFilter = null;
			this.InformationSortAndFilter.SetSrcSecretInformationPackage(package);
		}
		this.PopupWindow.OnCancelClick = new Action(this.QuickHide);
		base.CGet<CButtonObsolete>("Close").ClearAndAddListener(new Action(this.QuickHide));
		this._shopRoot = base.CGet<Refers>("ShopRoot");
		this._shopItemTemplate = this._shopRoot.CGet<Refers>("Template");
		this._shopItemTemplate.gameObject.SetActive(false);
		argsBox.Get("characterId", out this._shopCharId);
		this._shopRepoList.Clear();
		foreach (SecretInformationDisplayData key in this._shopRepoInstanceDict.Keys.ToArray<SecretInformationDisplayData>())
		{
			Object.Destroy(this._shopRepoInstanceDict[key].gameObject);
		}
		this._shopRepoInstanceDict.Clear();
		base.CGet<CButtonObsolete>("Confirm").gameObject.SetActive(false);
		base.CGet<CButtonObsolete>("Cancel").gameObject.SetActive(false);
		this.Element.OnListenerIdReady = new Action(this.RefreshPanels);
	}

	// Token: 0x0600380A RID: 14346 RVA: 0x001C3517 File Offset: 0x001C1717
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)SingletonObject.getInstance<BasicGameData>().TaiwuCharId, new uint[]
		{
			34U
		}));
	}

	// Token: 0x0600380B RID: 14347 RVA: 0x001C3544 File Offset: 0x001C1744
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
				bool flag = uid.DomainId == 4;
				if (flag)
				{
					bool flag2 = uid.SubId1 == 34U;
					if (flag2)
					{
						ResourceInts data = default(ResourceInts);
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref data);
						this._selfMoney = data.Get(6);
						this.RefreshPanels();
					}
				}
			}
		}
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x001C3610 File Offset: 0x001C1810
	public override void QuickHide()
	{
		TaiwuEventDomainMethod.Call.SetSecretInformationSelectResult("", -1);
		TaiwuEventDomainMethod.Call.TriggerListener("ShopActionComplete", true);
		GEvent.OnEvent(UiEvents.EventWindowAppearResult, EasyPool.Get<ArgumentBox>());
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x0600380D RID: 14349 RVA: 0x001C366B File Offset: 0x001C186B
	private void OnSecretInformationFilterDataChange()
	{
		this.ContentViewSecretInformation.SetDataCount(this.InformationSortAndFilter.GetShowingSecretCount());
	}

	// Token: 0x0600380E RID: 14350 RVA: 0x001C3688 File Offset: 0x001C1888
	private void OnSecretInformationItemRender(int index, Refers refers)
	{
		SecretInformationDisplayData secretInformation = this.InformationSortAndFilter.GetSecretInformationAtIndex(index);
		InformationUtils.RefreshSecretInformationView(refers, secretInformation, this.InformationSortAndFilter.GetSecretInformationDisplayPackage());
		CToggleObsolete toggle = refers.GetComponentInChildren<CToggleObsolete>(true);
		toggle.GetComponent<PointerTrigger>().enabled = true;
		toggle.enabled = true;
		toggle.isOn = false;
		toggle.onValueChanged.RemoveAllListeners();
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			bool flag = !isOn;
			if (!flag)
			{
				List<SecretInformationDisplayData> list = this.InformationSortAndFilter.GetSecretInformationDisplayPackage().SecretInformationDisplayDataList;
				list.Remove(secretInformation);
				this._shopRepoList.Add(secretInformation);
				this.RefreshPanels();
			}
		});
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x001C3718 File Offset: 0x001C1918
	private void RefreshPanels()
	{
		SecretInformationDisplayPackage package = this.InformationSortAndFilter.GetSecretInformationDisplayPackage();
		foreach (SecretInformationDisplayData key in this._shopRepoInstanceDict.Keys.ToArray<SecretInformationDisplayData>())
		{
			bool flag = this._shopRepoList.Contains(key);
			if (!flag)
			{
				Object.Destroy(this._shopRepoInstanceDict[key].gameObject);
				this._shopRepoInstanceDict.Remove(key);
			}
		}
		using (List<SecretInformationDisplayData>.Enumerator enumerator = this._shopRepoList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SecretInformationDisplayData data = enumerator.Current;
				bool flag2 = this._shopRepoInstanceDict.ContainsKey(data);
				if (!flag2)
				{
					Refers instance = Object.Instantiate<Refers>(this._shopItemTemplate, this._shopItemTemplate.transform.parent);
					this._shopRepoInstanceDict.Add(data, instance);
					instance.CGet<TextMeshProUGUI>("Name").text = SecretInformation.Instance[data.SecretInformationTemplateId].Name;
					instance.CGet<TextMeshProUGUI>("Value").text = data.ShopValue.ToString();
					instance.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
					{
						List<SecretInformationDisplayData> list = package.SecretInformationDisplayDataList;
						list.Add(data);
						this._shopRepoList.Remove(data);
						this.RefreshPanels();
					});
					instance.RectTransform.localScale = Vector3.one;
					instance.gameObject.SetActive(true);
				}
			}
		}
		Refers panel = base.CGet<Refers>("MoneyDisplay");
		TextMeshProUGUI labelValue = panel.CGet<TextMeshProUGUI>("Value");
		TextMeshProUGUI labelSign = panel.CGet<TextMeshProUGUI>("Sign");
		TextMeshProUGUI labelDelta = panel.CGet<TextMeshProUGUI>("Delta");
		int delta = -this._shopRepoList.Sum((SecretInformationDisplayData data) => data.ShopValue);
		labelSign.gameObject.SetActive(delta != 0);
		labelDelta.gameObject.SetActive(delta != 0);
		labelValue.text = this._selfMoney.ToString();
		labelSign.text = ((delta < 0) ? "-" : "+");
		labelDelta.text = Mathf.Abs(delta).ToString();
		UI_RecruitPeopleOverview.RefitContentSize(panel.RectTransform, labelValue.rectTransform);
		UI_RecruitPeopleOverview.RefitContentSize(panel.RectTransform, labelDelta.rectTransform);
		CButtonObsolete buttonSettle = base.CGet<CButtonObsolete>("ButtonConfirm");
		CButtonObsolete buttonReset = base.CGet<CButtonObsolete>("Reset");
		CButtonObsolete cbuttonObsolete = buttonSettle;
		bool interactable;
		if (this._shopRepoList.Count != 0)
		{
			interactable = (this._shopRepoList.Sum((SecretInformationDisplayData data) => data.ShopValue) <= this._selfMoney);
		}
		else
		{
			interactable = false;
		}
		cbuttonObsolete.interactable = interactable;
		buttonSettle.gameObject.GetOrAddComponent<DisableStyleRoot>().SetStyleEffect(!buttonSettle.interactable, false);
		buttonSettle.ClearAndAddListener(delegate
		{
			List<SecretInformationDisplayData> current = package.SecretInformationDisplayDataList;
			List<IntPair> data = new List<IntPair>();
			foreach (SecretInformationDisplayData item in this._shopRepoList)
			{
				data.Add(new IntPair((int)item.SecretInformationId, item.ShopValue));
				current.Remove(item);
			}
			this._shopRepoList.Clear();
			this.RefreshPanels();
			InformationDomainMethod.Call.SettleSecretInformationShopTrade(data, this._shopCharId);
		});
		buttonReset.interactable = (this._shopRepoList.Count != 0);
		buttonReset.ClearAndAddListener(delegate
		{
			List<SecretInformationDisplayData> current = package.SecretInformationDisplayDataList;
			current.AddRange(this._shopRepoList);
			this._shopRepoList.Clear();
			this.RefreshPanels();
		});
		this.InformationSortAndFilter.UpdateShowingSecretInformation();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x001C3AB8 File Offset: 0x001C1CB8
	private void Awake()
	{
		this._awakeDone = true;
		this.InformationSortAndFilter.RegisterSecretInformationHandler(new Action(this.OnSecretInformationFilterDataChange));
		this.ContentViewSecretInformation.OnItemRender = new Action<int, Refers>(this.OnSecretInformationItemRender);
	}

	// Token: 0x0400288E RID: 10382
	private bool _awakeDone = false;

	// Token: 0x0400288F RID: 10383
	private Refers _shopRoot;

	// Token: 0x04002890 RID: 10384
	private Refers _shopItemTemplate;

	// Token: 0x04002891 RID: 10385
	private int _selfMoney;

	// Token: 0x04002892 RID: 10386
	private int _shopCharId;

	// Token: 0x04002893 RID: 10387
	private readonly List<SecretInformationDisplayData> _shopRepoList = new List<SecretInformationDisplayData>();

	// Token: 0x04002894 RID: 10388
	private readonly Dictionary<SecretInformationDisplayData, Refers> _shopRepoInstanceDict = new Dictionary<SecretInformationDisplayData, Refers>();

	// Token: 0x04002895 RID: 10389
	private InformationFilter _informationFilter;
}
