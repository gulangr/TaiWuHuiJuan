using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.MouseTips;
using GameData.Domains.Global;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementPrison
{
	// Token: 0x02000789 RID: 1929
	public class ViewSectLaw : UIBase
	{
		// Token: 0x06005D55 RID: 23893 RVA: 0x002AEFC0 File Offset: 0x002AD1C0
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("IsSect", out this._isSect);
			this._stateId = -1;
			sbyte id;
			bool flag = argsBox.Get("StateTemplateId", out id);
			if (flag)
			{
				this._stateId = id;
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			this.lawTemplate.gameObject.SetActive(false);
			argsBox.Get("IsCustomizeSectLaw", out this._isCustomizeSectLaw);
			argsBox.Get("AllowRange", out this._allowRange);
			argsBox.Get("AllowOverdriveCount", out this._allowOverdriveCount);
			argsBox.Get("HasVillagerHead", out this._hasVillagerHead);
			this.buttons.Get(0).gameObject.SetActive(!this._isCustomizeSectLaw || this._isSect);
			this.buttons.Get(0).enabled = (!this._isCustomizeSectLaw || !this._isSect);
			this.buttons.Get(1).gameObject.SetActive(!this._isCustomizeSectLaw || !this._isSect);
			this.buttons.Get(1).enabled = (!this._isCustomizeSectLaw || this._isSect);
			this.customizeButtons.SetActive(this._isCustomizeSectLaw);
			this.tipCustom.gameObject.SetActive(this._isCustomizeSectLaw);
			this._isModify = false;
			UIManager.Instance.UnMaskComponent(this.dropdownUIMask.GetComponent<RectTransform>());
			this.confirm.interactable = false;
			GlobalDomainMethod.Call.InvokeGuidingTrigger(124);
		}

		// Token: 0x06005D56 RID: 23894 RVA: 0x002AF17E File Offset: 0x002AD37E
		private void OnListenerIdReady()
		{
			OrganizationDomainMethod.AsyncCall.GetCustomizePunishmentSeverityCost(this, this._stateId, true, delegate(int offset, RawDataPool pool)
			{
				int cost = 0;
				Serializer.Deserialize(pool, offset, ref cost);
				this._authrityCostCache[true] = cost;
				bool flag = this.buttons.GetActiveIndex() == 0;
				if (flag)
				{
					this.RefreshAuthorityCost(true);
				}
				this.Element.ShowAfterRefresh();
			});
			OrganizationDomainMethod.AsyncCall.GetCustomizePunishmentSeverityCost(this, this._stateId, false, delegate(int offset, RawDataPool pool)
			{
				int cost = 0;
				Serializer.Deserialize(pool, offset, ref cost);
				this._authrityCostCache[false] = cost;
				bool flag = this.buttons.GetActiveIndex() != 0;
				if (flag)
				{
					this.RefreshAuthorityCost(false);
				}
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06005D57 RID: 23895 RVA: 0x002AF1B8 File Offset: 0x002AD3B8
		private void RefreshAuthorityCost(bool isSect)
		{
			int cost = this._authrityCostCache.GetOrDefault(isSect);
			bool flag = cost == 0;
			if (flag)
			{
				this.authorityCostArea.SetActive(false);
			}
			else
			{
				this.authorityCostArea.SetActive(true);
				this.authorityCost.text = string.Format("{0}/{1}", cost, LocalStringManager.Get(LanguageKey.LK_Month));
			}
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x002AF220 File Offset: 0x002AD420
		public void Awake()
		{
			this.buttons.OnActiveIndexChange += this.OnToggleGroupChanged;
			this.buttons.Init(this._isSect ? 0 : 1);
			CommonTipSimpleRuntime runtime = CommonTip.DefValue.CustomSectLaw.BuildSimple();
			TooltipInvoker tooltipInvoker = this.tipCustom;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.tipCustom.RuntimeParam.SetObject("Runtime", runtime);
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x002AF2A0 File Offset: 0x002AD4A0
		private void OnEnable()
		{
			this.buttons.Set(this._isSect ? 0 : 1, true);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				this.verticalScrollbar.value = 0f;
			});
			bool flag = this._focusedDropdown != null;
			if (flag)
			{
				this._focusedDropdown.transform.SetParent(this._focusedDropdownOriginParent);
				this._focusedDropdown = null;
			}
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x002AF30F File Offset: 0x002AD50F
		private void OnDisable()
		{
			this._modifyMap.Clear();
			this.Clear();
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x002AF328 File Offset: 0x002AD528
		public override void QuickHide()
		{
			bool activeSelf = this.dropdownUIMask.gameObject.activeSelf;
			if (activeSelf)
			{
				this._focusedDropdown.OnCancel(null);
			}
			else
			{
				bool interactable = this.confirm.interactable;
				if (interactable)
				{
					string title = LanguageKey.LK_Law_Cancel_Dialog_Title.Tr();
					string content = LanguageKey.LK_Law_Cancel_Dialog_Content.Tr();
					CommonUtils.ShowConfirmDialog(title, content, new Action(this.Hide), null, EDialogType.None);
				}
				else
				{
					this.Hide();
				}
			}
		}

		// Token: 0x06005D5C RID: 23900 RVA: 0x002AF3A4 File Offset: 0x002AD5A4
		private void Hide()
		{
			base.QuickHide();
			bool isCustomizeSectLaw = this._isCustomizeSectLaw;
			if (isCustomizeSectLaw)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("FinishCityPunishmentSeverityCustomizeUI", this._isModify);
			}
		}

		// Token: 0x06005D5D RID: 23901 RVA: 0x002AF3D4 File Offset: 0x002AD5D4
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "Cancel";
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				bool flag2 = btn.name == "Confirm";
				if (flag2)
				{
					OrganizationDomainMethod.Call.UpdateCityPunishmentSeverityCustomizeData(this._stateId, this._isSect, this._targetPunishmentTypeTemplateId, (sbyte)this._targetPunishmentSeverityTemplateId);
					this._isModify = true;
					this.Hide();
				}
				else
				{
					bool flag3 = btn.name == "Close";
					if (flag3)
					{
						this.QuickHide();
					}
				}
			}
		}

		// Token: 0x06005D5E RID: 23902 RVA: 0x002AF464 File Offset: 0x002AD664
		private void OnToggleGroupChanged(int togNew, int togOld)
		{
			bool flag = togNew < 0;
			if (!flag)
			{
				sbyte stateId = this._stateId;
				bool flag2 = stateId < 0;
				if (flag2)
				{
					WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
					stateId = mapModel.Areas[(int)mapModel.CurrentAreaId].GetConfig().StateID;
				}
				bool isSect = togNew == 0;
				List<ShortPair> punishments;
				bool flag3 = SingletonObject.getInstance<PunishmentModel>().GetPunishmentMap(isSect).TryGetValue((short)stateId, out punishments);
				if (flag3)
				{
					this.RefreshLawList(punishments, isSect);
				}
				this.RefreshAuthorityCost(isSect);
			}
		}

		// Token: 0x06005D5F RID: 23903 RVA: 0x002AF4DF File Offset: 0x002AD6DF
		public void OnDetailBtnClicked()
		{
			UIManager.Instance.ShowUI(UIElement.PunishmentSeverity, true);
		}

		// Token: 0x06005D60 RID: 23904 RVA: 0x002AF4F4 File Offset: 0x002AD6F4
		private void RefreshLawList(List<ShortPair> punishments, bool isSect)
		{
			this.Clear();
			this._modifyMap.Clear();
			PunishmentModel punishmentModel = SingletonObject.getInstance<PunishmentModel>();
			List<PunishmentSeverityCustomizeData> customizeDataList = punishmentModel.GetCustomizeDataList(this._stateId, isSect);
			bool flag = customizeDataList != null;
			if (flag)
			{
				foreach (PunishmentSeverityCustomizeData customizeData in customizeDataList)
				{
					this._modifyMap.Add(customizeData.PunishmentTypeTemplateId, customizeData);
				}
			}
			for (int i = 0; i < punishments.Count; i++)
			{
				ShortPair punishment = punishments[i];
				GameObject obj = Object.Instantiate<GameObject>(this.lawTemplate, this.lawContent.transform);
				SectLawCrimeTemplate refers = obj.GetComponent<SectLawCrimeTemplate>();
				short type = punishment.First;
				short severity = punishment.Second;
				sbyte originalSeverity = punishmentModel.GetOriginalPunishmentSeverity(this._stateId, punishment.First, isSect);
				PunishmentSeverityCustomizeData customizeData2 = this._modifyMap.GetOrDefault(type);
				refers.Init(type, severity, this._isCustomizeSectLaw, customizeData2, (short)originalSeverity);
				bool isCustomizeSectLaw = this._isCustomizeSectLaw;
				if (isCustomizeSectLaw)
				{
					this.SetDropDown(refers, type, severity, (short)originalSeverity);
				}
				obj.SetActive(true);
				this._objList.Add(obj);
			}
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x002AF650 File Offset: 0x002AD850
		private void SetDropDown(SectLawCrimeTemplate refers, short punishmentTypeTemplateId, short severityTemplateId, short severityTemplateIdForGenerate)
		{
			CDropdown dropdown = refers.Dropdown;
			this._dropdownList.Add(dropdown);
			List<short> severityTemplateIdList = this.GenerateSeverityIds(severityTemplateId);
			dropdown.options.Clear();
			foreach (PunishmentSeverityItem config in from optionId in severityTemplateIdList
			select PunishmentSeverity.Instance[(int)optionId])
			{
				dropdown.options.Add(new CDropdown.OptionData
				{
					text = config.Name.SetColor(config.NameColor)
				});
			}
			int severityTemplateIdIndex = severityTemplateIdList.IndexOf(severityTemplateIdForGenerate);
			this._dropdownDefaultValueList.Add(severityTemplateIdIndex);
			dropdown.value = severityTemplateIdIndex;
			dropdown.RefreshShownValue();
			dropdown.onValueChanged.RemoveAllListeners();
			dropdown.onValueChanged.AddListener(delegate(int index)
			{
				short selectedId = severityTemplateIdList[index];
				refers.SetChangeDuration(selectedId);
				this.SetTargetDropdown(dropdown, severityTemplateIdIndex, punishmentTypeTemplateId, selectedId);
				this.UpdateConfirmButton();
			});
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x002AF7BC File Offset: 0x002AD9BC
		private List<short> GenerateSeverityIds(short baseId)
		{
			return this.GenerateSeverityIds(baseId, this._allowOverdriveCount);
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x002AF7CC File Offset: 0x002AD9CC
		private List<short> GenerateSeverityIds(short baseId, int allowOverdriveCount)
		{
			List<short> ids = new List<short>();
			for (int offset = -this._allowRange; offset <= this._allowRange; offset++)
			{
				short currentId = (short)((int)baseId - offset);
				int overdrive = Math.Abs((int)(currentId - baseId)) - 1;
				bool flag = overdrive > allowOverdriveCount;
				if (!flag)
				{
					bool flag2 = currentId >= 0 && currentId <= 4;
					if (flag2)
					{
						ids.Add(currentId);
					}
				}
			}
			return ids;
		}

		// Token: 0x06005D64 RID: 23908 RVA: 0x002AF844 File Offset: 0x002ADA44
		private void SetTargetDropdown(CDropdown dropdown, int severityTemplateIdIndex, short punishmentTypeTemplateId, short punishmentSeverityTemplateId)
		{
			bool flag = this._targetDropdown == dropdown;
			if (!flag)
			{
				bool flag2 = this._targetDropdown != null;
				if (flag2)
				{
					this._targetDropdown.value = this._targetSeverityTemplateIdIndex;
				}
				this._targetDropdown = dropdown;
				this._targetSeverityTemplateIdIndex = severityTemplateIdIndex;
				this._targetPunishmentTypeTemplateId = punishmentTypeTemplateId;
				this._targetPunishmentSeverityTemplateId = punishmentSeverityTemplateId;
			}
		}

		// Token: 0x06005D65 RID: 23909 RVA: 0x002AF8A4 File Offset: 0x002ADAA4
		private void UpdateConfirmButton()
		{
			for (int i = 0; i < this._dropdownDefaultValueList.Count; i++)
			{
				bool flag = this._dropdownDefaultValueList[i] != this._dropdownList[i].value;
				if (flag)
				{
					this.confirm.interactable = true;
					return;
				}
			}
			this.confirm.interactable = false;
		}

		// Token: 0x06005D66 RID: 23910 RVA: 0x002AF914 File Offset: 0x002ADB14
		private void LateUpdate()
		{
			foreach (CDropdown dropdown in this._dropdownList)
			{
				this.UpdateDropdownVisual(dropdown);
				this.HandleDropdownFocus(dropdown);
			}
		}

		// Token: 0x06005D67 RID: 23911 RVA: 0x002AF978 File Offset: 0x002ADB78
		private void UpdateDropdownVisual(CDropdown dropdown)
		{
			bool flag = !dropdown.IsExpanded;
			if (flag)
			{
				dropdown.image.sprite = this.dropdownNormalSprite;
			}
			else
			{
				dropdown.image.sprite = this.dropdownExpandSprite;
				Transform dropdownList = dropdown.transform.Find("Dropdown List");
				bool flag2 = !dropdownList;
				if (!flag2)
				{
					RectTransform content = dropdownList.GetChild(0).GetComponent<CScrollRect>().Content;
					for (int i = 1; i < content.childCount; i++)
					{
						Transform item = content.GetChild(i);
						bool flag3 = !item.gameObject.activeSelf;
						if (!flag3)
						{
							bool isSelectedItem = dropdown.value == i - 1;
							item.GetComponent<CToggle>().interactable = !isSelectedItem;
						}
					}
				}
			}
		}

		// Token: 0x06005D68 RID: 23912 RVA: 0x002AFA50 File Offset: 0x002ADC50
		private void HandleDropdownFocus(CDropdown dropdown)
		{
			bool flag = dropdown.IsExpanded && this._focusedDropdown == null;
			if (flag)
			{
				this._focusedDropdownOriginParent = dropdown.transform.parent;
				dropdown.transform.SetParent(this.dropdownSlot);
				UIManager.Instance.MaskComponent(this.dropdownUIMask.GetComponent<RectTransform>());
				this._focusedDropdown = dropdown;
			}
			else
			{
				bool flag2 = this._focusedDropdown == dropdown && !dropdown.IsExpanded;
				if (flag2)
				{
					dropdown.transform.SetParent(this._focusedDropdownOriginParent);
					UIManager.Instance.UnMaskComponent(this.dropdownUIMask.GetComponent<RectTransform>());
					this._focusedDropdown = null;
				}
			}
		}

		// Token: 0x06005D69 RID: 23913 RVA: 0x002AFB0C File Offset: 0x002ADD0C
		private void Clear()
		{
			foreach (GameObject obj in this._objList)
			{
				Object.Destroy(obj);
			}
			this._objList.Clear();
			this._dropdownDefaultValueList.Clear();
			this._dropdownList.Clear();
		}

		// Token: 0x04004036 RID: 16438
		[SerializeField]
		private CToggleGroup buttons;

		// Token: 0x04004037 RID: 16439
		[SerializeField]
		private CScrollbar verticalScrollbar;

		// Token: 0x04004038 RID: 16440
		[SerializeField]
		private GameObject lawTemplate;

		// Token: 0x04004039 RID: 16441
		[SerializeField]
		private GameObject lawContent;

		// Token: 0x0400403A RID: 16442
		[SerializeField]
		private GameObject authorityCostArea;

		// Token: 0x0400403B RID: 16443
		[SerializeField]
		private TextMeshProUGUI authorityCost;

		// Token: 0x0400403C RID: 16444
		[SerializeField]
		private Sprite dropdownNormalSprite;

		// Token: 0x0400403D RID: 16445
		[SerializeField]
		private Sprite dropdownExpandSprite;

		// Token: 0x0400403E RID: 16446
		[SerializeField]
		private CButton dropdownUIMask;

		// Token: 0x0400403F RID: 16447
		[SerializeField]
		private RectTransform dropdownSlot;

		// Token: 0x04004040 RID: 16448
		[SerializeField]
		private GameObject customizeButtons;

		// Token: 0x04004041 RID: 16449
		[SerializeField]
		private CButton confirm;

		// Token: 0x04004042 RID: 16450
		[SerializeField]
		private CButton cancel;

		// Token: 0x04004043 RID: 16451
		[SerializeField]
		private TooltipInvoker tipCustom;

		// Token: 0x04004044 RID: 16452
		private readonly List<GameObject> _objList = new List<GameObject>();

		// Token: 0x04004045 RID: 16453
		private bool _isCustomizeSectLaw;

		// Token: 0x04004046 RID: 16454
		private bool _isSect;

		// Token: 0x04004047 RID: 16455
		private sbyte _stateId;

		// Token: 0x04004048 RID: 16456
		private readonly Dictionary<short, PunishmentSeverityCustomizeData> _modifyMap = new Dictionary<short, PunishmentSeverityCustomizeData>();

		// Token: 0x04004049 RID: 16457
		private readonly Dictionary<bool, int> _authrityCostCache = new Dictionary<bool, int>();

		// Token: 0x0400404A RID: 16458
		private bool _isModify;

		// Token: 0x0400404B RID: 16459
		private readonly List<CDropdown> _dropdownList = new List<CDropdown>();

		// Token: 0x0400404C RID: 16460
		private readonly List<int> _dropdownDefaultValueList = new List<int>();

		// Token: 0x0400404D RID: 16461
		private CDropdown _focusedDropdown;

		// Token: 0x0400404E RID: 16462
		private Transform _focusedDropdownOriginParent;

		// Token: 0x0400404F RID: 16463
		private const short MIN_SEVERITY_ID = 0;

		// Token: 0x04004050 RID: 16464
		private const short MAX_SEVERITY_ID = 4;

		// Token: 0x04004051 RID: 16465
		private CDropdown _targetDropdown;

		// Token: 0x04004052 RID: 16466
		private int _targetSeverityTemplateIdIndex;

		// Token: 0x04004053 RID: 16467
		private short _targetPunishmentSeverityTemplateId = -1;

		// Token: 0x04004054 RID: 16468
		private short _targetPunishmentTypeTemplateId = -1;

		// Token: 0x04004055 RID: 16469
		private const int SectKey = 0;

		// Token: 0x04004056 RID: 16470
		private const int StateKey = 1;

		// Token: 0x04004057 RID: 16471
		private int _allowRange;

		// Token: 0x04004058 RID: 16472
		private int _allowOverdriveCount;

		// Token: 0x04004059 RID: 16473
		private bool _hasVillagerHead;
	}
}
