using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class MapPickupAutoTriggerSettingPanel : MonoBehaviour
{
	// Token: 0x06002DBB RID: 11707 RVA: 0x0016A38C File Offset: 0x0016858C
	private void Awake()
	{
		this.TryInit();
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x0016A398 File Offset: 0x00168598
	private void TryInit()
	{
		bool initialized = this._initialized;
		if (!initialized)
		{
			this.xiangshuMinionToggleGroup.InitPreOnToggle(-1);
			CToggleGroupObsolete ctoggleGroupObsolete = this.xiangshuMinionToggleGroup;
			ctoggleGroupObsolete.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(ctoggleGroupObsolete.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(delegate(CToggleObsolete _, CToggleObsolete _)
			{
				bool isAutoSetting = this._isAutoSetting;
				if (!isAutoSetting)
				{
					Action onSettingChanged = this.OnSettingChanged;
					if (onSettingChanged != null)
					{
						onSettingChanged();
					}
				}
			}));
			this.gradeDropdown.Setup(this._selectedGrade, delegate(sbyte grade)
			{
				bool isAutoSetting = this._isAutoSetting;
				if (!isAutoSetting)
				{
					this._selectedGrade = grade;
					Action onSettingChanged = this.OnSettingChanged;
					if (onSettingChanged != null)
					{
						onSettingChanged();
					}
				}
			});
			int typeCount = 14;
			CommonUtils.PrepareEnoughChildren(this.typeToggleButtonRoot, this.typeToggleButtonTemplate.gameObject, typeCount, null);
			for (int i = 0; i < typeCount; i++)
			{
				Transform child = this.typeToggleButtonRoot.GetChild(i);
				Refers refers = child.GetComponent<Refers>();
				GameObject checkMark = refers.CGet<GameObject>("CheckMark");
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				label.text = LocalStringManager.Get(string.Format("LK_MapPickupType2_{0}", i));
				this._typeSelectedDict[i] = false;
				CButtonObsolete button = child.GetComponent<CButtonObsolete>();
				int ii = i;
				button.ClearAndAddListener(delegate
				{
					this._typeSelectedDict[ii] = !this._typeSelectedDict[ii];
					checkMark.SetActive(this._typeSelectedDict[ii]);
					Action onSettingChanged = this.OnSettingChanged;
					if (onSettingChanged != null)
					{
						onSettingChanged();
					}
				});
				MapPickupAutoTriggerSettingPanel.InitItemTypeTips(child, i);
			}
			this._initialized = true;
		}
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x0016A4E8 File Offset: 0x001686E8
	private static void InitItemTypeTips(Transform child, int i)
	{
		TooltipInvoker tip = child.GetComponent<TooltipInvoker>();
		bool tipEnabled = i == 0 || i == 1 || i == 2;
		tip.enabled = tipEnabled;
		bool flag = !tipEnabled;
		if (!flag)
		{
			tip.Type = TipType.SimpleList;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			if (!true)
			{
			}
			MouseTipSimpleList.Config? config2;
			switch (i)
			{
			case 0:
				config2 = new MouseTipSimpleList.Config?(new MouseTipSimpleList.Config
				{
					TitleKey = LanguageKey.LK_TipResourceTypes_Title,
					Lines = new List<MouseTipSimpleList.LineConfig>
					{
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_0", LanguageKey.LK_Resource_Name_Food),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_1", LanguageKey.LK_Resource_Name_Wood),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_2", LanguageKey.LK_Resource_Name_Metal),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_3", LanguageKey.LK_Resource_Name_Jade),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_4", LanguageKey.LK_Resource_Name_Fabric),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_5", LanguageKey.LK_Resource_Name_Herb),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_6", LanguageKey.LK_Resource_Name_Money),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_7", LanguageKey.LK_Resource_Name_Authority)
					}
				});
				break;
			case 1:
				config2 = new MouseTipSimpleList.Config?(new MouseTipSimpleList.Config
				{
					TitleKey = LanguageKey.LK_TipResourceTypes_Title,
					Lines = new List<MouseTipSimpleList.LineConfig>
					{
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_0", LanguageKey.LK_MapPickup_Material_Name_Food),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_1", LanguageKey.LK_MapPickup_Material_Name_Wood),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_2", LanguageKey.LK_MapPickup_Material_Name_Metal),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_3", LanguageKey.LK_MapPickup_Material_Name_Jade),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_4", LanguageKey.LK_MapPickup_Material_Name_Fabric),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_5", LanguageKey.LK_MapPickup_Material_Name_Herb),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_8", LanguageKey.LK_MapPickup_Material_Name_Poison)
					}
				});
				break;
			case 2:
				config2 = new MouseTipSimpleList.Config?(new MouseTipSimpleList.Config
				{
					TitleKey = LanguageKey.LK_TipResourceTypes_Title,
					Lines = new List<MouseTipSimpleList.LineConfig>
					{
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_8", LanguageKey.LK_MapPickup_Medicine_Name_Medicine),
						new MouseTipSimpleList.LineConfig("mousetip_mapeventicon_8", LanguageKey.LK_MapPickup_Medicine_Name_Poison)
					}
				});
				break;
			default:
				config2 = null;
				break;
			}
			if (!true)
			{
			}
			MouseTipSimpleList.Config? config = config2;
			tip.RuntimeParam.SetObject("Config", config.Value);
		}
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x0016A7F8 File Offset: 0x001689F8
	public void RefreshBySetting(MapPickupAutoTriggerSetting setting)
	{
		this.TryInit();
		this._isAutoSetting = true;
		this.gradeDropdown.SetByGrade(setting.MinGrade);
		this.xiangshuMinionToggleGroup.Set(setting.IncludeXiangshuMinion ? 0 : 1, true, false);
		int typeCount = 14;
		for (int i = 0; i < typeCount; i++)
		{
			bool included = setting.PickupTypes[i];
			this._typeSelectedDict[i] = included;
			Transform child = this.typeToggleButtonRoot.GetChild(i);
			Refers refers = child.GetComponent<Refers>();
			GameObject checkMark = refers.CGet<GameObject>("CheckMark");
			checkMark.SetActive(included);
		}
		this._isAutoSetting = false;
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x0016A89C File Offset: 0x00168A9C
	public MapPickupAutoTriggerSetting GetSetting()
	{
		return new MapPickupAutoTriggerSetting
		{
			IncludeXiangshuMinion = (this.xiangshuMinionToggleGroup.GetActive().Key == 0),
			MinGrade = this.gradeDropdown.GetSelectedGrade(),
			PickupTypes = this._typeSelectedDict
		};
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x0016A8EB File Offset: 0x00168AEB
	public void SetInteractable(bool interactable)
	{
		this.clickBlocker.SetActive(!interactable);
		this.disableRoot.SetStyleEffect(!interactable, false);
	}

	// Token: 0x0400210F RID: 8463
	private sbyte _selectedGrade;

	// Token: 0x04002110 RID: 8464
	private readonly bool[] _typeSelectedDict = new bool[14];

	// Token: 0x04002111 RID: 8465
	public Action OnSettingChanged;

	// Token: 0x04002112 RID: 8466
	private bool _initialized;

	// Token: 0x04002113 RID: 8467
	private bool _isAutoSetting;

	// Token: 0x04002114 RID: 8468
	public RectTransform panel;

	// Token: 0x04002115 RID: 8469
	public CToggleGroupObsolete xiangshuMinionToggleGroup;

	// Token: 0x04002116 RID: 8470
	public GradeDropdown gradeDropdown;

	// Token: 0x04002117 RID: 8471
	public RectTransform typeToggleButtonRoot;

	// Token: 0x04002118 RID: 8472
	public Refers typeToggleButtonTemplate;

	// Token: 0x04002119 RID: 8473
	public DisableStyleRoot disableRoot;

	// Token: 0x0400211A RID: 8474
	public GameObject clickBlocker;
}
