using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.World;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000363 RID: 867
public class WorldDetailSettingGroup : MonoBehaviour
{
	// Token: 0x17000574 RID: 1396
	// (get) Token: 0x06003245 RID: 12869 RVA: 0x0018CBA2 File Offset: 0x0018ADA2
	// (set) Token: 0x06003246 RID: 12870 RVA: 0x0018CBAA File Offset: 0x0018ADAA
	public int SettingCount { get; private set; }

	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x06003247 RID: 12871 RVA: 0x0018CBB3 File Offset: 0x0018ADB3
	public WorldCreationGroupItem ConfigItem
	{
		get
		{
			return WorldCreationGroup.Instance[this._groupId];
		}
	}

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x06003248 RID: 12872 RVA: 0x0018CBC5 File Offset: 0x0018ADC5
	public sbyte GroupId
	{
		get
		{
			return this._groupId;
		}
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x0018CBD0 File Offset: 0x0018ADD0
	public static Color GetLevelColor(int level)
	{
		if (!true)
		{
		}
		Color result;
		switch (level)
		{
		case 0:
			result = Colors.Instance["pinkyellow"];
			break;
		case 1:
			result = Colors.Instance["brightblue"];
			break;
		case 2:
			result = Colors.Instance["yellow"];
			break;
		case 3:
			result = Colors.Instance.GradeColors[8];
			break;
		default:
			throw new ArgumentOutOfRangeException("level", level, null);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x0018CC5C File Offset: 0x0018AE5C
	public void Init(sbyte groupId, Action<byte, byte> onSettingChangedHandler, bool isLegacy)
	{
		this._groupId = groupId;
		this._onSettingChanged = onSettingChangedHandler;
		this._level = 0;
		this.SettingCount = 0;
		foreach (WorldDetailSettingItem setting in this._settings)
		{
			setting.gameObject.SetActive(false);
		}
		Action<byte, byte> settingChangedDelegate = new Action<byte, byte>(this.OnValueChanged);
		WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[groupId];
		for (int i = 0; i < groupCfg.WorldCreations.Length; i++)
		{
			byte templateId = groupCfg.WorldCreations[i];
			WorldDetailSettingItem settingItem = this.GetOrCreateNextSettingItem();
			settingItem.Init(templateId, settingChangedDelegate);
			settingItem.gameObject.SetActive(true);
		}
		bool isRegular = groupId == 3;
		this._titleBack.gameObject.SetActive(!isRegular);
		this._titleNoProgressBack.SetActive(isRegular);
		this.UpdateProgress();
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x0018CD6C File Offset: 0x0018AF6C
	public void LoadByDifficultyPreset(sbyte difficulty)
	{
		this._creationInfo = WorldCreationInfo.CreateByDifficultyPreset(difficulty);
		this.LoadFromWorldCreationInfo(this._creationInfo);
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x0018CD88 File Offset: 0x0018AF88
	public void LoadFromWorldCreationInfo(WorldCreationInfo creationInfo)
	{
		for (int i = 0; i < this.SettingCount; i++)
		{
			WorldDetailSettingItem setting = this._settings[i];
			int value = creationInfo.Get(setting.Config.TemplateId);
			setting.SetWithoutNotify(value);
		}
		this._creationInfo = creationInfo;
		this._level = this._creationInfo.GetGroupLevel(this._groupId);
		this.UpdateProgress();
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x0018CDFC File Offset: 0x0018AFFC
	public void SaveToWorldCreationInfo(ref WorldCreationInfo creationInfo)
	{
		for (int i = 0; i < this.SettingCount; i++)
		{
			WorldDetailSettingItem setting = this._settings[i];
			int value = setting.GetSettingValue();
			creationInfo.Set(setting.Config.TemplateId, (byte)value);
		}
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x0018CE4C File Offset: 0x0018B04C
	public void RefreshInteractable(WorldDetailSettingGroup.EInteractType type)
	{
		this.RefreshProgressBarStatus(type);
		bool flag = type == WorldDetailSettingGroup.EInteractType.NewGamePreset;
		if (flag)
		{
			foreach (WorldDetailSettingItem item in this._settings)
			{
				item.ToggleGroup.SetInteractable(true, null);
			}
		}
		else
		{
			bool flag2 = type == WorldDetailSettingGroup.EInteractType.NewGameCustom;
			if (flag2)
			{
				foreach (WorldDetailSettingItem item2 in this._settings)
				{
					item2.ToggleGroup.SetInteractable(true, null);
				}
			}
			else
			{
				for (int i = 0; i < this.SettingCount; i++)
				{
					WorldDetailSettingItem item3 = this._settings[i];
					int key = item3.GetSettingValue();
					for (int togIndex = 0; togIndex < item3.Toggles.Length; togIndex++)
					{
						CToggleObsolete toggle = item3.Toggles[togIndex];
						bool flag3 = type == WorldDetailSettingGroup.EInteractType.Inherit;
						if (flag3)
						{
							toggle.interactable = (Math.Abs(key - togIndex) <= 1 && item3.Config.ShowInLegacy);
						}
						else
						{
							bool canResetWorldSettings = GlobalOperations.CanResetWorldSettings;
							if (canResetWorldSettings)
							{
								toggle.interactable = item3.Config.ShowInLegacy;
							}
							else
							{
								toggle.interactable = false;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x0018CFE0 File Offset: 0x0018B1E0
	private void RefreshProgressBarStatus(WorldDetailSettingGroup.EInteractType type)
	{
		bool isRegular = this._groupId == 3;
		bool hasBar = type != WorldDetailSettingGroup.EInteractType.Legacy || GlobalOperations.CanResetWorldSettings;
		this._progressBar.SetActive(hasBar && !isRegular);
		this._emptyBar.SetActive(!hasBar && !isRegular);
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x0018D034 File Offset: 0x0018B234
	private WorldDetailSettingItem GetOrCreateNextSettingItem()
	{
		int settingCount = this.SettingCount;
		this.SettingCount = settingCount + 1;
		bool flag = this._settings.Count > settingCount;
		WorldDetailSettingItem result;
		if (flag)
		{
			result = this._settings[settingCount];
		}
		else
		{
			GameObject newObj = Object.Instantiate<GameObject>(this._settings[0].gameObject, this._contentRoot);
			WorldDetailSettingItem item = newObj.GetComponent<WorldDetailSettingItem>();
			this._settings.Add(item);
			result = item;
		}
		return result;
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x0018D0B0 File Offset: 0x0018B2B0
	private void OnValueChanged(byte templateId, byte value)
	{
		this._creationInfo.Set(templateId, value);
		Action<byte, byte> onSettingChanged = this._onSettingChanged;
		if (onSettingChanged != null)
		{
			onSettingChanged(templateId, value);
		}
		this._level = this._creationInfo.GetGroupLevel(this._groupId);
		this.UpdateProgress();
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x0018D100 File Offset: 0x0018B300
	private void UpdateProgress()
	{
		WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[this._groupId];
		string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
		string levelText = LocalStringManager.Get(string.Format("LK_WorldCreation_GroupLevel_{0}", this._level));
		Color color = WorldDetailSettingGroup.GetLevelColor(this._level);
		this._title.text = (groupCfg.Name + dot + levelText).SetColor(color);
		this._titleNoProgress.SetText(groupCfg.Name.SetColor(color), true);
		this._titleBack.sprite = this._titleBackSprites[this._level];
		int sum = this._creationInfo.GetGroupLegacyBonusSum(this._groupId);
		TMP_Text progressText = this._progressText;
		string format = "{0}/{1}";
		object arg = sum;
		sbyte[] legacyGroupLevelThresholds = GlobalConfig.Instance.LegacyGroupLevelThresholds;
		progressText.text = string.Format(format, arg, legacyGroupLevelThresholds[legacyGroupLevelThresholds.Length - 1]);
		this._progressFill.sprite = this._progressBarFillSprites[this._level];
		Image progressFill = this._progressFill;
		float num = (float)sum;
		sbyte[] legacyGroupLevelThresholds2 = GlobalConfig.Instance.LegacyGroupLevelThresholds;
		progressFill.fillAmount = num / (float)legacyGroupLevelThresholds2[legacyGroupLevelThresholds2.Length - 1];
	}

	// Token: 0x040024CE RID: 9422
	[SerializeField]
	private Sprite[] _titleBackSprites;

	// Token: 0x040024CF RID: 9423
	[SerializeField]
	private Sprite[] _progressBarFillSprites;

	// Token: 0x040024D0 RID: 9424
	[SerializeField]
	private TextMeshProUGUI _title;

	// Token: 0x040024D1 RID: 9425
	[SerializeField]
	private CImage _titleBack;

	// Token: 0x040024D2 RID: 9426
	[SerializeField]
	private TextMeshProUGUI _titleNoProgress;

	// Token: 0x040024D3 RID: 9427
	[SerializeField]
	private GameObject _titleNoProgressBack;

	// Token: 0x040024D4 RID: 9428
	[SerializeField]
	private GameObject _progressBar;

	// Token: 0x040024D5 RID: 9429
	[SerializeField]
	private CImage _progressFill;

	// Token: 0x040024D6 RID: 9430
	[SerializeField]
	private TextMeshProUGUI _progressText;

	// Token: 0x040024D7 RID: 9431
	[SerializeField]
	private List<WorldDetailSettingItem> _settings;

	// Token: 0x040024D8 RID: 9432
	[SerializeField]
	private RectTransform _contentRoot;

	// Token: 0x040024D9 RID: 9433
	[SerializeField]
	private GameObject _emptyBar;

	// Token: 0x040024DB RID: 9435
	private sbyte _groupId;

	// Token: 0x040024DC RID: 9436
	private int _level;

	// Token: 0x040024DD RID: 9437
	private Action<byte, byte> _onSettingChanged;

	// Token: 0x040024DE RID: 9438
	private WorldCreationInfo _creationInfo;

	// Token: 0x02001726 RID: 5926
	public enum EInteractType
	{
		// Token: 0x0400AA90 RID: 43664
		NewGamePreset,
		// Token: 0x0400AA91 RID: 43665
		NewGameCustom,
		// Token: 0x0400AA92 RID: 43666
		Legacy,
		// Token: 0x0400AA93 RID: 43667
		Inherit
	}
}
