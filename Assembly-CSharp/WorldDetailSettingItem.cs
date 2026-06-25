using System;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x02000364 RID: 868
public class WorldDetailSettingItem : MonoBehaviour
{
	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x06003254 RID: 12884 RVA: 0x0018D22A File Offset: 0x0018B42A
	// (set) Token: 0x06003255 RID: 12885 RVA: 0x0018D232 File Offset: 0x0018B432
	public WorldCreationItem Config { get; private set; }

	// Token: 0x06003256 RID: 12886 RVA: 0x0018D23B File Offset: 0x0018B43B
	private void Awake()
	{
		this.NameAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
		this.ToggleGroup.InitPreOnToggle(-1);
		this.ToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleGroupValueChanged);
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x0018D274 File Offset: 0x0018B474
	public void Init(byte templateId, Action<byte, byte> onSettingChangedHandler)
	{
		this._onSettingChangedHandler = onSettingChangedHandler;
		this.Config = WorldCreation.Instance.GetItem(templateId);
		this.NameLabel.text = this.Config.Name;
		this.MouseTip.PresetParam = new string[]
		{
			this.Config.Name,
			this.Config.Desc
		};
		for (int i = 0; i < 4; i++)
		{
			bool hasOption = i < this.Config.Options.Length;
			Transform transform = this.Toggles[i].transform.Find("PointLabel");
			TextMeshProUGUI label = (transform != null) ? transform.GetComponent<TextMeshProUGUI>() : null;
			bool flag = label;
			if (flag)
			{
				label.text = (this.Config.LegacyPointBonus.CheckIndex(i) ? ("+" + this.Config.LegacyPointBonus[i].ToString()) : string.Empty);
			}
			this.Toggles[i].gameObject.SetActive(hasOption);
			bool flag2 = hasOption;
			if (flag2)
			{
				this.Toggles[i].GetComponentInChildren<TextMeshProUGUI>().text = this.Config.Options[i].ColorReplace();
			}
		}
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x0018D3BD File Offset: 0x0018B5BD
	public void SetWithoutNotify(int onKey)
	{
		this.LevelIcon.SetSprite(this.Config.Icons[onKey], false, null);
		this.ToggleGroup.SetWithoutNotify(onKey, true);
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x0018D3EC File Offset: 0x0018B5EC
	public int GetSettingValue()
	{
		bool flag = null == this.ToggleGroup.GetActive();
		if (flag)
		{
			this.ToggleGroup.InitPreOnToggle(-1);
		}
		return this.ToggleGroup.GetActive().Key;
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x0018D430 File Offset: 0x0018B630
	private void OnToggleGroupValueChanged(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		Action<byte, byte> onSettingChangedHandler = this._onSettingChangedHandler;
		if (onSettingChangedHandler != null)
		{
			onSettingChangedHandler(this.Config.TemplateId, (byte)newTog.Key);
		}
		this.LevelIcon.SetSprite(this.Config.Icons[newTog.Key], false, null);
	}

	// Token: 0x040024DF RID: 9439
	public const float NameAreaWidth = 150f;

	// Token: 0x040024E0 RID: 9440
	public TextMeshProUGUI NameLabel;

	// Token: 0x040024E1 RID: 9441
	public RectTransform NameAreaRectTransform;

	// Token: 0x040024E2 RID: 9442
	public CImage LevelIcon;

	// Token: 0x040024E3 RID: 9443
	public CToggleGroupObsolete ToggleGroup;

	// Token: 0x040024E4 RID: 9444
	public CToggleObsolete[] Toggles;

	// Token: 0x040024E5 RID: 9445
	public TooltipInvoker MouseTip;

	// Token: 0x040024E6 RID: 9446
	private Action<byte, byte> _onSettingChangedHandler;
}
