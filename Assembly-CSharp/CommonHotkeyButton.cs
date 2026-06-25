using System;
using System.Text;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200032B RID: 811
[RequireComponent(typeof(LanguageRuleActiveOne), typeof(PointerTrigger), typeof(HSVStyleRoot))]
public class CommonHotkeyButton : CButtonObsolete, ILanguage
{
	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x06002F1D RID: 12061 RVA: 0x0017244C File Offset: 0x0017064C
	private LanguageRuleActiveOne LanguageRule4
	{
		get
		{
			return base.GetComponent<LanguageRuleActiveOne>();
		}
	}

	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x06002F1E RID: 12062 RVA: 0x00172454 File Offset: 0x00170654
	private PointerTrigger PointerTrigger
	{
		get
		{
			return base.GetComponent<PointerTrigger>();
		}
	}

	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x06002F1F RID: 12063 RVA: 0x0017245C File Offset: 0x0017065C
	private HSVStyleRoot HSVStyleRoot
	{
		get
		{
			return base.GetComponent<HSVStyleRoot>();
		}
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x00172464 File Offset: 0x00170664
	protected override void Awake()
	{
		base.Awake();
		this.PointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerTriggerEnter));
		this.PointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerTriggerExit));
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x001724B3 File Offset: 0x001706B3
	protected override void Start()
	{
		base.Start();
		this.RefreshStyle();
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x001724C4 File Offset: 0x001706C4
	protected override void OnInteractableChangeInternal(bool value)
	{
		base.OnInteractableChangeInternal(value);
		this.RefreshStyle();
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x001724D6 File Offset: 0x001706D6
	private void OnPointerTriggerEnter()
	{
		this._isHovered = true;
		this.RefreshStyle();
		UnityEvent enterEvent = this.EnterEvent;
		if (enterEvent != null)
		{
			enterEvent.Invoke();
		}
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x001724F9 File Offset: 0x001706F9
	private void OnPointerTriggerExit()
	{
		this._isHovered = false;
		this.RefreshStyle();
		UnityEvent exitEvent = this.ExitEvent;
		if (exitEvent != null)
		{
			exitEvent.Invoke();
		}
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x0017251C File Offset: 0x0017071C
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		this.LanguageRule4.OnLanguageChange(languageType);
		bool flag = this._mainLabelKey != null;
		if (flag)
		{
			this.UpdateMainLabelByLanguageKey(this._mainLabelKey.Value);
		}
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x0017255C File Offset: 0x0017075C
	private void RefreshStyle()
	{
		bool flag = !base.interactable;
		Sprite targetSprite;
		TextStyleData targetStyle;
		if (flag)
		{
			targetSprite = this.disabledNameBg;
			targetStyle = this.labelDisabledStyle;
		}
		else
		{
			bool isHovered = this._isHovered;
			if (isHovered)
			{
				targetSprite = this.hoverNameBg;
				targetStyle = this.labelHoverStyle;
			}
			else
			{
				targetSprite = this.normalNameBg;
				targetStyle = this.labelNormalStyle;
			}
		}
		bool flag2 = targetSprite != null;
		if (flag2)
		{
			this.bg.sprite = targetSprite;
		}
		bool flag3 = targetStyle != null;
		if (flag3)
		{
			targetStyle.ApplyTo(this.cnMainLabel);
			targetStyle.ApplyTo(this.enMainLabel);
		}
		this.HSVStyleRoot.ClearColorBackups();
		bool flag4 = !base.interactable;
		if (flag4)
		{
			this.HSVStyleRoot.enabled = true;
			this.HSVStyleRoot.SetDefaultGrayAndBlack();
		}
		else
		{
			this.HSVStyleRoot.SetDefault();
			this.HSVStyleRoot.enabled = false;
			bool flag5 = this._hotkeyTextCache != null;
			if (flag5)
			{
				this.UpdateHotKeyLabelRaw(this._hotkeyTextCache);
			}
		}
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x00172668 File Offset: 0x00170868
	public void UpdateHotKeyLabelRaw(string hotkeyString)
	{
		string colorReplaced = hotkeyString.ColorReplace();
		this.cnHotKeyLabel.text = colorReplaced;
		this.enHotKeyLabel.text = colorReplaced;
		this._hotkeyTextCache = hotkeyString;
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x001726A0 File Offset: 0x001708A0
	public void UpdateHotkeyWithDefaultFormat(string hotkeyString)
	{
		string colorReplaced = LocalStringManager.GetFormat(LanguageKey.LK_ShortCuts_Hotkey_CommonPattern, hotkeyString).ColorReplace();
		this.cnHotKeyLabel.text = colorReplaced;
		this.enHotKeyLabel.text = colorReplaced;
		this._hotkeyTextCache = colorReplaced;
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x001726E0 File Offset: 0x001708E0
	public void UpdateMainLabelRaw(string labelString)
	{
		this.cnMainLabel.text = labelString.ColorReplace();
		this.enMainLabel.text = labelString.ColorReplace();
		this._mainLabelKey = null;
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x00172714 File Offset: 0x00170914
	public void UpdateHotKeyByCommand(HotKeyCommand command)
	{
		CommonHotkeyButton._sb.Clear();
		KeyCode[] keyCodes = command.GetKeyCode(false);
		for (int i = 0; i < keyCodes.Length; i++)
		{
			KeyCode keyCode = keyCodes[i];
			bool flag = i > 0;
			if (flag)
			{
				CommonHotkeyButton._sb.Append("+");
			}
			CommonHotkeyButton._sb.Append(command.GetKeyCodeString(keyCode));
		}
		this.UpdateHotkeyWithDefaultFormat(CommonHotkeyButton._sb.ToString());
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x0017278A File Offset: 0x0017098A
	public void UpdateMainLabelByLanguageKey(LanguageKey key)
	{
		this.cnMainLabel.text = LocalStringManager.Get(key).ColorReplace();
		this.enMainLabel.text = LocalStringManager.Get(key).ColorReplace();
		this._mainLabelKey = new LanguageKey?(key);
	}

	// Token: 0x0400223C RID: 8764
	[SerializeField]
	private CImage bg;

	// Token: 0x0400223D RID: 8765
	[SerializeField]
	private TextMeshProUGUI cnMainLabel;

	// Token: 0x0400223E RID: 8766
	[SerializeField]
	private TextMeshProUGUI enMainLabel;

	// Token: 0x0400223F RID: 8767
	[SerializeField]
	private TextMeshProUGUI cnHotKeyLabel;

	// Token: 0x04002240 RID: 8768
	[SerializeField]
	private TextMeshProUGUI enHotKeyLabel;

	// Token: 0x04002241 RID: 8769
	[SerializeField]
	private Sprite normalNameBg;

	// Token: 0x04002242 RID: 8770
	[SerializeField]
	private Sprite hoverNameBg;

	// Token: 0x04002243 RID: 8771
	[SerializeField]
	private Sprite disabledNameBg;

	// Token: 0x04002244 RID: 8772
	[SerializeField]
	private TextStyleData labelNormalStyle;

	// Token: 0x04002245 RID: 8773
	[SerializeField]
	private TextStyleData labelHoverStyle;

	// Token: 0x04002246 RID: 8774
	[SerializeField]
	private TextStyleData labelDisabledStyle;

	// Token: 0x04002247 RID: 8775
	private LanguageKey? _mainLabelKey;

	// Token: 0x04002248 RID: 8776
	private string _hotkeyTextCache;

	// Token: 0x04002249 RID: 8777
	[NonSerialized]
	public UnityEvent EnterEvent;

	// Token: 0x0400224A RID: 8778
	[NonSerialized]
	public UnityEvent ExitEvent;

	// Token: 0x0400224B RID: 8779
	private bool _isHovered;

	// Token: 0x0400224C RID: 8780
	private static readonly string[] TextStyleControlKeys = new string[]
	{
		"m_fontAsset",
		"m_fontStyle",
		"m_sharedMaterial",
		"m_fontWeight",
		"m_fontColor",
		"m_enableVertexGradient",
		"m_fontColorGradient",
		"m_colorMode",
		"m_fontColorGradientPreset",
		"m_isRichText"
	};

	// Token: 0x0400224D RID: 8781
	private static readonly StringBuilder _sb = new StringBuilder(32);
}
