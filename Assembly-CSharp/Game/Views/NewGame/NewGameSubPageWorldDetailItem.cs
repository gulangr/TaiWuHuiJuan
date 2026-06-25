using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Switch;
using GameData.Domains.World;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x02000813 RID: 2067
	public class NewGameSubPageWorldDetailItem : MonoBehaviour
	{
		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x0600657B RID: 25979 RVA: 0x002E6239 File Offset: 0x002E4439
		public CToggle SwitchToggle
		{
			get
			{
				return this.switchToggle;
			}
		}

		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x0600657C RID: 25980 RVA: 0x002E6241 File Offset: 0x002E4441
		// (set) Token: 0x0600657D RID: 25981 RVA: 0x002E6249 File Offset: 0x002E4449
		public WorldCreationItem ConfigItem { get; private set; }

		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x0600657E RID: 25982 RVA: 0x002E6252 File Offset: 0x002E4452
		public int SettingValue
		{
			get
			{
				return this._settingValue;
			}
		}

		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x0600657F RID: 25983 RVA: 0x002E625A File Offset: 0x002E445A
		private bool IsSwitch
		{
			get
			{
				return this.ConfigItem.Options.Length == 2;
			}
		}

		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x06006580 RID: 25984 RVA: 0x002E626C File Offset: 0x002E446C
		private bool IsRegular
		{
			get
			{
				return this._groupId == 3;
			}
		}

		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x06006581 RID: 25985 RVA: 0x002E6277 File Offset: 0x002E4477
		// (set) Token: 0x06006582 RID: 25986 RVA: 0x002E627F File Offset: 0x002E447F
		public bool IsNoConfig { get; private set; }

		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x06006583 RID: 25987 RVA: 0x002E6288 File Offset: 0x002E4488
		// (set) Token: 0x06006584 RID: 25988 RVA: 0x002E6290 File Offset: 0x002E4490
		public string Desc { get; private set; }

		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x06006585 RID: 25989 RVA: 0x002E6299 File Offset: 0x002E4499
		// (set) Token: 0x06006586 RID: 25990 RVA: 0x002E62A1 File Offset: 0x002E44A1
		public string Title { get; private set; }

		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x06006587 RID: 25991 RVA: 0x002E62AA File Offset: 0x002E44AA
		// (set) Token: 0x06006588 RID: 25992 RVA: 0x002E62B2 File Offset: 0x002E44B2
		public string TexturePrefix { get; private set; }

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x06006589 RID: 25993 RVA: 0x002E62BB File Offset: 0x002E44BB
		public string Texture
		{
			get
			{
				return this.IsNoConfig ? "ui9_tex_newgame_6_img_{0}_0".GetFormat(this.TexturePrefix) : "ui9_tex_newgame_6_img_difficulty_{0}_{1}".GetFormat(this.TexturePrefix, this.SettingValue);
			}
		}

		// Token: 0x0600658A RID: 25994 RVA: 0x002E62F4 File Offset: 0x002E44F4
		private void Awake()
		{
			this.buttonLast.ClearAndAddListener(new Action(this.OnClickButtonLast));
			this.buttonNext.ClearAndAddListener(new Action(this.OnClickButtonNext));
			this.switchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSwitchToggleValueChange));
		}

		// Token: 0x0600658B RID: 25995 RVA: 0x002E6350 File Offset: 0x002E4550
		public void Init(byte templateId, sbyte groupId, Action<byte, byte> onSettingChangedHandler, Action<NewGameSubPageWorldDetailItem> onEnter = null, Action<NewGameSubPageWorldDetailItem> onExit = null)
		{
			this._onSettingChangedHandler = onSettingChangedHandler;
			this.ConfigItem = WorldCreation.Instance.GetItem(templateId);
			this.TexturePrefix = templateId.ToString();
			this.Title = this.ConfigItem.Name;
			this.Desc = this.ConfigItem.Desc.ColorReplace();
			this._groupId = groupId;
			this.textTitle.text = this.Title;
			bool flag = this.tip;
			if (flag)
			{
				this.tip.Type = TipType.Simple;
				this.tip.PresetParam = new string[]
				{
					this.Title,
					this.Desc
				};
			}
			this.SetValueRangeDefault();
			this.switchToggle.gameObject.SetActive(this.IsSwitch);
			this.toggleRoot.SetActive(!this.IsSwitch);
			this.InitPointerTrigger(onEnter, onExit);
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x002E6448 File Offset: 0x002E4648
		public void Init(string texturePrefix, string title, string desc, int value, bool isSwitch, string[] options, Action<byte, byte> onSettingChangedHandler, Action<NewGameSubPageWorldDetailItem> onEnter, Action<NewGameSubPageWorldDetailItem> onExit)
		{
			this.IsNoConfig = true;
			this.TexturePrefix = texturePrefix;
			this.Title = title;
			this.Desc = desc;
			this._noConfigOptions = options;
			this._onSettingChangedHandler = onSettingChangedHandler;
			this.textTitle.text = title;
			this.InitPointerTrigger(onEnter, onExit);
			this.switchToggle.gameObject.SetActive(isSwitch);
			this.toggleRoot.SetActive(!isSwitch);
			this.SetValueRange(0, options.Length - 1);
			this.SetWithoutNotify(value);
			this.SetInteractable(true);
		}

		// Token: 0x0600658D RID: 25997 RVA: 0x002E64E4 File Offset: 0x002E46E4
		private void InitPointerTrigger(Action<NewGameSubPageWorldDetailItem> onEnter, Action<NewGameSubPageWorldDetailItem> onExit)
		{
			bool flag = this.pointerTrigger;
			if (flag)
			{
				PointerTrigger pointerTrigger = this.pointerTrigger;
				if (pointerTrigger.EnterEvent == null)
				{
					pointerTrigger.EnterEvent = new UnityEvent();
				}
				this.pointerTrigger.EnterEvent.RemoveAllListeners();
				bool flag2 = onEnter != null;
				if (flag2)
				{
					this.pointerTrigger.EnterEvent.AddListener(delegate()
					{
						onEnter(this);
					});
				}
				pointerTrigger = this.pointerTrigger;
				if (pointerTrigger.ExitEvent == null)
				{
					pointerTrigger.ExitEvent = new UnityEvent();
				}
				this.pointerTrigger.ExitEvent.RemoveAllListeners();
				bool flag3 = onExit != null;
				if (flag3)
				{
					this.pointerTrigger.ExitEvent.AddListener(delegate()
					{
						onExit(this);
					});
				}
			}
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x002E65D0 File Offset: 0x002E47D0
		public void SetWithoutNotify(int index)
		{
			string[] options = this.IsNoConfig ? this._noConfigOptions : this.ConfigItem.Options;
			this._settingValue = index;
			bool flag = !this.IsNoConfig;
			if (flag)
			{
				bool flag2 = this.imageIcon;
				if (flag2)
				{
					string[] icons = this.ConfigItem.Icons;
					string icon = (icons != null) ? icons.GetOrDefault(index) : null;
					this.imageIcon.SetSprite(icon, false, null);
				}
				bool flag3 = this.textPoint;
				if (flag3)
				{
					this.textPoint.text = (this.ConfigItem.LegacyPointBonus.CheckIndex(index) ? ("+" + this.ConfigItem.LegacyPointBonus[index].ToString()) : string.Empty);
				}
			}
			bool isRegular = this.IsRegular;
			if (isRegular)
			{
				this.textLevelName.text = options[index];
			}
			else
			{
				Color color = NewGameSubPageWorldDetailGroup.GetLevelColor(index);
				this.textLevelName.text = options[index].SetColor(color);
			}
			bool activeSelf = this.switchToggle.gameObject.activeSelf;
			if (activeSelf)
			{
				this.switchToggle.SetWithoutNotify(this._settingValue == 0);
			}
			this.RefreshButton();
		}

		// Token: 0x0600658F RID: 25999 RVA: 0x002E6714 File Offset: 0x002E4914
		public int GetSettingValue()
		{
			return this._settingValue;
		}

		// Token: 0x06006590 RID: 26000 RVA: 0x002E671C File Offset: 0x002E491C
		private void OnSwitchToggleValueChange(bool isOn)
		{
			this.SetWithoutNotify(isOn ? 0 : 1);
			this.OnSettingChanged();
		}

		// Token: 0x06006591 RID: 26001 RVA: 0x002E6734 File Offset: 0x002E4934
		private void OnClickButtonLast()
		{
			this.SetWithoutNotify(this._settingValue - 1);
			this.OnSettingChanged();
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x002E674D File Offset: 0x002E494D
		private void OnClickButtonNext()
		{
			this.SetWithoutNotify(this._settingValue + 1);
			this.OnSettingChanged();
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x002E6766 File Offset: 0x002E4966
		private void OnSettingChanged()
		{
			Action<byte, byte> onSettingChangedHandler = this._onSettingChangedHandler;
			if (onSettingChangedHandler != null)
			{
				WorldCreationItem configItem = this.ConfigItem;
				onSettingChangedHandler((configItem != null) ? configItem.TemplateId : 0, (byte)this._settingValue);
			}
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x002E6794 File Offset: 0x002E4994
		private void RefreshButton()
		{
			bool isLocked = !this.IsRegular && NewGameSubPageWorldDetail.IsDifficultyLocked(WorldCreationInfo.EDifficultyLevel.Level4.ToInt());
			int end = isLocked ? Mathf.Min(2, this._settingValueRange.end) : this._settingValueRange.end;
			this.buttonLast.interactable = (this._interactable && this._settingValue > this._settingValueRange.start);
			this.buttonNext.interactable = (this._interactable && this._settingValue < end);
			this.tipButtonNext.enabled = (!this.buttonNext.interactable && isLocked && !this.IsNoConfig && this._settingValue == 2);
			this.switchToggle.interactable = this._interactable;
		}

		// Token: 0x06006595 RID: 26005 RVA: 0x002E686E File Offset: 0x002E4A6E
		public void SetValueRange(int min, int max)
		{
			min = Mathf.Max(min, 0);
			max = Mathf.Min(max, 3);
			this._settingValueRange = new RangeInt(min, max - min);
			this.RefreshButton();
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x002E6899 File Offset: 0x002E4A99
		public void SetValueRangeDefault()
		{
			this.SetValueRange(0, 3);
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x002E68A4 File Offset: 0x002E4AA4
		public void SetInteractable(bool interactable)
		{
			this._interactable = interactable;
			this.RefreshButton();
		}

		// Token: 0x040046B8 RID: 18104
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x040046B9 RID: 18105
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040046BA RID: 18106
		[SerializeField]
		private TextMeshProUGUI textPoint;

		// Token: 0x040046BB RID: 18107
		[SerializeField]
		private TextMeshProUGUI textLevelName;

		// Token: 0x040046BC RID: 18108
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x040046BD RID: 18109
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x040046BE RID: 18110
		[Header("页签")]
		[SerializeField]
		private GameObject toggleRoot;

		// Token: 0x040046BF RID: 18111
		[SerializeField]
		private CButton buttonLast;

		// Token: 0x040046C0 RID: 18112
		[SerializeField]
		private CButton buttonNext;

		// Token: 0x040046C1 RID: 18113
		[SerializeField]
		private TooltipInvoker tipButtonNext;

		// Token: 0x040046C2 RID: 18114
		[Header("开关")]
		[SerializeField]
		private SwitchToggleSmall switchToggle;

		// Token: 0x040046C3 RID: 18115
		private Action<byte, byte> _onSettingChangedHandler;

		// Token: 0x040046C5 RID: 18117
		private int _settingValue;

		// Token: 0x040046C6 RID: 18118
		private sbyte _groupId;

		// Token: 0x040046C7 RID: 18119
		public const int SettingValueMin = 0;

		// Token: 0x040046C8 RID: 18120
		public const int SettingValueMax = 3;

		// Token: 0x040046C9 RID: 18121
		private RangeInt _settingValueRange;

		// Token: 0x040046CA RID: 18122
		private bool _interactable;

		// Token: 0x040046CB RID: 18123
		public const byte SwitchOnIndex = 0;

		// Token: 0x040046CC RID: 18124
		public const byte SwitchOffIndex = 1;

		// Token: 0x040046CE RID: 18126
		private string[] _noConfigOptions;
	}
}
