using System;
using System.Collections.Generic;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Information;
using TMPro;
using UnityEngine;

// Token: 0x020003A3 RID: 931
public class UI_SelectInformation : UIBase
{
	// Token: 0x060037F6 RID: 14326 RVA: 0x001C2714 File Offset: 0x001C0914
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("showOnly", out this._isShowOnly);
		argsBox.Get("SelectInformationType", out this._selectInformationType);
		bool flag = !argsBox.Get("banBroadCast", out this._banBroadCast);
		if (flag)
		{
			this._banBroadCast = false;
		}
		string title;
		bool flag2 = !argsBox.Get("title", out title);
		if (flag2)
		{
			title = string.Empty;
		}
		TextMeshProUGUI text = base.CGet<TextMeshProUGUI>("Title");
		text.text = title;
		List<NormalInformation> srcNormalInformationList;
		bool flag3 = argsBox.Get<List<NormalInformation>>("normalInformation", out srcNormalInformationList);
		if (flag3)
		{
			this._isShowSecret = false;
			this.InformationSortAndFilter.SetIsShowSecret(this._isShowSecret);
			this.InformationSortAndFilter.SetSrcNormalInformationList(srcNormalInformationList);
		}
		SecretInformationDisplayPackage package;
		bool flag4 = argsBox.Get<SecretInformationDisplayPackage>("secretInformation", out package);
		if (flag4)
		{
			this._isShowSecret = true;
			this.InformationSortAndFilter.SetIsShowSecret(this._isShowSecret);
			this.InformationSortAndFilter.SetSrcSecretInformationPackage(package);
		}
		object callback;
		bool flag5 = argsBox.Get<object>("callback", out callback);
		if (flag5)
		{
			this._onNormalInformationConfirm = (callback as Action<NormalInformation>);
			this._onSecretInformationConfirm = (callback as Action<SecretInformationDisplayData>);
		}
		else
		{
			this._onNormalInformationConfirm = null;
			this._onSecretInformationConfirm = null;
		}
		Enum normalInformationFilterMode;
		bool flag6 = argsBox.Get("PresetFilterMode", out normalInformationFilterMode);
		if (flag6)
		{
			this.InformationSortAndFilter.NormalInformationFilter.PresetFilter = (NormalInformationFilter.PresetFilterMode)normalInformationFilterMode;
		}
		else
		{
			this.InformationSortAndFilter.NormalInformationFilter.PresetFilter = NormalInformationFilter.PresetFilterMode.None;
		}
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		ResourceMonitor monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(taiwuCharId, false);
		this._authority = monitor.Resources[7];
		this.Setup();
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x001C28BC File Offset: 0x001C0ABC
	public override void QuickHide()
	{
		Action<NormalInformation> onNormalInformationConfirm = this._onNormalInformationConfirm;
		if (onNormalInformationConfirm != null)
		{
			onNormalInformationConfirm(new NormalInformation(-1, -1));
		}
		Action<SecretInformationDisplayData> onSecretInformationConfirm = this._onSecretInformationConfirm;
		if (onSecretInformationConfirm != null)
		{
			onSecretInformationConfirm(new SecretInformationDisplayData
			{
				SecretInformationTemplateId = -1,
				SecretInformationId = SecretInformationId.Invalid
			});
		}
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x001C2930 File Offset: 0x001C0B30
	private void Setup()
	{
		Array.ForEach<GameObject>(this.ActiveObjsWhenNormalInformation, delegate(GameObject e)
		{
			e.SetActive(!this._isShowSecret);
		});
		Array.ForEach<GameObject>(this.ActiveObjsWhenSecretInformation, delegate(GameObject e)
		{
			e.SetActive(this._isShowSecret);
		});
		base.CGet<CButtonObsolete>("Close").gameObject.SetActive(this._isShowOnly);
		base.CGet<CButtonObsolete>("Confirm").gameObject.SetActive(!this._isShowOnly);
		base.CGet<CButtonObsolete>("Cancel").gameObject.SetActive(!this._isShowOnly);
		this.PopupWindow.ConfirmButton.interactable = false;
		this.PopupWindow.ConfirmBtnTips.enabled = false;
		bool isShowSecret = this._isShowSecret;
		if (isShowSecret)
		{
			bool shouldBroadcast = !this._banBroadCast;
			this.InformationSortAndFilter.SetInformationFilterStyle(new bool[]
			{
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				default(bool),
				!this._isShowOnly,
				shouldBroadcast
			});
		}
		else
		{
			bool[] flags = new bool[]
			{
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false
			};
			bool flag = this._selectInformationType >= 0;
			if (flag)
			{
				for (int i = 0; i < flags.Length; i++)
				{
					flags[i] = false;
				}
				bool flag2 = this._selectInformationType == 0;
				if (flag2)
				{
					flags[0] = true;
				}
				bool flag3 = this._selectInformationType == 1;
				if (flag3)
				{
					flags[1] = true;
				}
				bool flag4 = this._selectInformationType == 2;
				if (flag4)
				{
					flags[2] = true;
				}
				bool flag5 = this._selectInformationType == 3;
				if (flag5)
				{
					flags[3] = true;
				}
				bool flag6 = this._selectInformationType == 5;
				if (flag6)
				{
					flags[4] = true;
				}
				bool flag7 = this._selectInformationType == 6;
				if (flag7)
				{
					flags[5] = true;
				}
			}
			this.InformationSortAndFilter.SetInformationFilterStyle(flags);
			this.ResetConfirmBtnTipsForSecretInformation();
			this.PopupWindow.ConfirmBtnTips.enabled = true;
		}
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x001C2B0C File Offset: 0x001C0D0C
	private void ResetConfirmBtnTipsForSecretInformation()
	{
		this.PopupWindow.ConfirmBtnTips.Type = TipType.SingleDesc;
		this.PopupWindow.ConfirmBtnTips.RuntimeParam = null;
		this.PopupWindow.ConfirmBtnTips.PresetParam = new string[]
		{
			LocalStringManager.Get(LanguageKey.UI_SelectInformation_NotSelected)
		};
	}

	// Token: 0x060037FA RID: 14330 RVA: 0x001C2B60 File Offset: 0x001C0D60
	private void SwitchSelectedIndex(int index)
	{
		int prevIndex = this._curSelectIndex;
		this._curSelectIndex = index;
		bool emptySelection = index < 0;
		bool flag = !emptySelection && this._isShowSecret && this.InformationSortAndFilter.GetSecretInformationAtIndex(index).AuthorityCostWhenDisseminatingForBroadcast > 0;
		if (flag)
		{
			int cost = this.InformationSortAndFilter.GetSecretInformationAtIndex(index).AuthorityCostWhenDisseminatingForBroadcast;
			int owned = SingletonObject.getInstance<BuildingModel>().GetResourceCount(7);
			bool usable = owned >= cost;
			StringBuilder text = new StringBuilder();
			text.AppendLine(LocalStringManager.Get(LanguageKey.LK_Broadcast_Action_Tip).SetColor("grey"));
			text.AppendLine("<SpName=mousetip_lingxing>" + LocalStringManager.Get(LanguageKey.LK_Item_Repair_Tip_Resource_Title).SetColor("pinkyellow"));
			text.Append("    ·<SpName=mousetip_ziyuan_7>");
			text.Append((ResourceType.Instance[7].Name + ": ").SetColor("grey"));
			text.AppendLine(cost.ToString().SetColor(usable ? "lightblue" : "brightred") + "/" + owned.ToString().SetColor("pinkyellow"));
			bool flag2 = !usable;
			if (flag2)
			{
				text.AppendLine(LocalStringManager.Get(LanguageKey.LK_MerchantInfo_Protect_Tip_ResourceNotMeet).SetColor("brightred"));
			}
			this.PopupWindow.ConfirmButton.interactable = usable;
			this.PopupWindow.ConfirmBtnTips.Type = TipType.Simple;
			this.PopupWindow.ConfirmBtnTips.RuntimeParam = null;
			this.PopupWindow.ConfirmBtnTips.PresetParam = new string[]
			{
				LocalStringManager.Get(LanguageKey.LK_Broadcast),
				text.ToString()
			};
			this.PopupWindow.ConfirmBtnTips.enabled = true;
		}
		else
		{
			this.ResetConfirmBtnTipsForSecretInformation();
			this.PopupWindow.ConfirmButton.interactable = !emptySelection;
			this.PopupWindow.ConfirmBtnTips.enabled = emptySelection;
		}
		bool flag3 = !this._isShowSecret;
		if (flag3)
		{
			bool flag4 = prevIndex >= 0 && prevIndex < this.InformationSortAndFilter.GetShowingNormalCount();
			if (flag4)
			{
				this.ContentViewNormalInformation.RefreshCell(prevIndex);
			}
		}
		else
		{
			bool flag5 = prevIndex >= 0 && prevIndex < this.InformationSortAndFilter.GetShowingSecretCount();
			if (flag5)
			{
				this.ContentViewSecretInformation.RefreshCell(prevIndex);
			}
		}
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x001C2DCC File Offset: 0x001C0FCC
	public void ChooseInformationOrSecretInformation(sbyte type)
	{
		if (type != 0)
		{
			if (type == 1)
			{
				bool flag = this.InformationSortAndFilter.GetShowingSecretCount() > 0;
				if (flag)
				{
					int index = Random.Range(0, this.InformationSortAndFilter.GetShowingSecretCount() - 1);
					this.SwitchSelectedIndex(index);
					this.PopupWindow.OnConfirmClick();
				}
				else
				{
					this.QuickHide();
				}
			}
		}
		else
		{
			bool flag2 = this.InformationSortAndFilter.GetShowingNormalCount() > 0;
			if (flag2)
			{
				int index2 = Random.Range(0, this.InformationSortAndFilter.GetShowingNormalCount() - 1);
				this.SwitchSelectedIndex(index2);
				this.PopupWindow.OnConfirmClick();
			}
			else
			{
				this.PopupWindow.OnCancelClick();
			}
		}
	}

	// Token: 0x060037FC RID: 14332 RVA: 0x001C2E98 File Offset: 0x001C1098
	private void OnNormalInformationItemRender(int index, Refers refers)
	{
		NormalInformation normalInformationData = this.InformationSortAndFilter.GetNormalInformationAtIndex(index);
		InformationUtils.RefreshNormalInformationView(refers, normalInformationData);
		CToggleObsolete toggle = refers.GetComponentInChildren<CToggleObsolete>(true);
		toggle.GetComponent<PointerTrigger>().enabled = true;
		toggle.enabled = !this._isShowOnly;
		toggle.onValueChanged.RemoveAllListeners();
		toggle.isOn = (this._curSelectIndex == index);
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.SwitchSelectedIndex(index);
			}
			else
			{
				this.SwitchSelectedIndex(-1);
			}
		});
	}

	// Token: 0x060037FD RID: 14333 RVA: 0x001C2F34 File Offset: 0x001C1134
	private void OnSecretInformationItemRender(int index, Refers refers)
	{
		SecretInformationDisplayData secretInformation = this.InformationSortAndFilter.GetSecretInformationAtIndex(index);
		InformationUtils.RefreshSecretInformationView(refers, secretInformation, this.InformationSortAndFilter.GetSecretInformationDisplayPackage());
		CToggleObsolete toggle = refers.GetComponentInChildren<CToggleObsolete>(true);
		toggle.GetComponent<PointerTrigger>().enabled = !this._isShowOnly;
		toggle.enabled = !this._isShowOnly;
		toggle.onValueChanged.RemoveAllListeners();
		bool flag = !this._isShowOnly;
		if (flag)
		{
			toggle.isOn = (this._curSelectIndex == index);
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.SwitchSelectedIndex(index);
				}
				else
				{
					this.SwitchSelectedIndex(-1);
				}
			});
			bool flag2 = this._authority < secretInformation.AuthorityCostWhenDisseminating;
			if (flag2)
			{
				toggle.interactable = false;
			}
		}
		else
		{
			toggle.isOn = false;
		}
	}

	// Token: 0x060037FE RID: 14334 RVA: 0x001C3018 File Offset: 0x001C1218
	private void OnNormalInformationFilterDataChange()
	{
		this.SwitchSelectedIndex(-1);
		this.ContentViewNormalInformation.SetDataCount(this.InformationSortAndFilter.GetShowingNormalCount());
	}

	// Token: 0x060037FF RID: 14335 RVA: 0x001C303A File Offset: 0x001C123A
	private void OnSecretInformationFilterDataChange()
	{
		this.SwitchSelectedIndex(-1);
		this.ContentViewSecretInformation.SetDataCount(this.InformationSortAndFilter.GetShowingSecretCount());
	}

	// Token: 0x06003800 RID: 14336 RVA: 0x001C305C File Offset: 0x001C125C
	private void OnConfirm()
	{
		bool flag = !this._isShowSecret && this._curSelectIndex >= 0;
		if (flag)
		{
			Action<NormalInformation> onNormalInformationConfirm = this._onNormalInformationConfirm;
			if (onNormalInformationConfirm != null)
			{
				onNormalInformationConfirm(this.InformationSortAndFilter.GetNormalInformationAtIndex(this._curSelectIndex));
			}
			UIManager.Instance.HideUI(this.Element);
		}
		else
		{
			bool flag2 = this._isShowSecret && this._curSelectIndex >= 0;
			if (flag2)
			{
				Action<SecretInformationDisplayData> onSecretInformationConfirm = this._onSecretInformationConfirm;
				if (onSecretInformationConfirm != null)
				{
					onSecretInformationConfirm(this.InformationSortAndFilter.GetSecretInformationAtIndex(this._curSelectIndex));
				}
				UIManager.Instance.HideUI(this.Element);
			}
			else
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x06003801 RID: 14337 RVA: 0x001C3118 File Offset: 0x001C1318
	private void ClearView()
	{
		bool flag = this.ContentViewNormalInformation.OnItemRender != null;
		if (flag)
		{
			this.ContentViewNormalInformation.UpdateData(0);
		}
		bool flag2 = this.ContentViewSecretInformation.OnItemRender != null;
		if (flag2)
		{
			this.ContentViewSecretInformation.UpdateData(0);
		}
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x001C3164 File Offset: 0x001C1364
	private void Awake()
	{
		this.PopupWindow.OnConfirmClick = new Action(this.OnConfirm);
		this.PopupWindow.OnCancelClick = new Action(this.QuickHide);
		this.InformationSortAndFilter.RegisterNormalInformationHandler(new Action(this.OnNormalInformationFilterDataChange));
		this.InformationSortAndFilter.RegisterSecretInformationHandler(new Action(this.OnSecretInformationFilterDataChange));
		this.ContentViewNormalInformation.OnItemRender = new Action<int, Refers>(this.OnNormalInformationItemRender);
		this.ContentViewSecretInformation.OnItemRender = new Action<int, Refers>(this.OnSecretInformationItemRender);
		base.CGet<CButtonObsolete>("Close").ClearAndAddListener(new Action(this.QuickHide));
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x001C321D File Offset: 0x001C141D
	private void OnEnable()
	{
		this.InformationSortAndFilter.SetDirty();
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x001C322C File Offset: 0x001C142C
	private void OnDisable()
	{
		this.ClearView();
	}

	// Token: 0x06003805 RID: 14341 RVA: 0x001C3238 File Offset: 0x001C1438
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.PopupWindow.ConfirmButton.interactable;
		if (flag)
		{
			Action onConfirmClick = this.PopupWindow.OnConfirmClick;
			if (onConfirmClick != null)
			{
				onConfirmClick();
			}
		}
	}

	// Token: 0x04002880 RID: 10368
	public PopupWindow PopupWindow;

	// Token: 0x04002881 RID: 10369
	public GameObject[] ActiveObjsWhenNormalInformation;

	// Token: 0x04002882 RID: 10370
	public GameObject[] ActiveObjsWhenSecretInformation;

	// Token: 0x04002883 RID: 10371
	public InfinityScrollLegacy ContentViewNormalInformation;

	// Token: 0x04002884 RID: 10372
	public InfinityScrollLegacy ContentViewSecretInformation;

	// Token: 0x04002885 RID: 10373
	public InformationSortAndFilter InformationSortAndFilter;

	// Token: 0x04002886 RID: 10374
	private bool _isShowOnly;

	// Token: 0x04002887 RID: 10375
	private bool _banBroadCast;

	// Token: 0x04002888 RID: 10376
	private bool _isShowSecret;

	// Token: 0x04002889 RID: 10377
	private int _curSelectIndex;

	// Token: 0x0400288A RID: 10378
	private int _authority;

	// Token: 0x0400288B RID: 10379
	private sbyte _selectInformationType;

	// Token: 0x0400288C RID: 10380
	private Action<NormalInformation> _onNormalInformationConfirm;

	// Token: 0x0400288D RID: 10381
	private Action<SecretInformationDisplayData> _onSecretInformationConfirm;
}
