using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200036C RID: 876
public class UI_AudioSetting : UIBase
{
	// Token: 0x060032DC RID: 13020 RVA: 0x00190F36 File Offset: 0x0018F136
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitAudioSettings();
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x00190F40 File Offset: 0x0018F140
	private void OnEnable()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.SystemOption);
		if (flag)
		{
			AudioManager.Instance.DisableMusicVolumeRate();
			AudioManager.Instance.SetMusicVolumeWithFade(0.33f, 0f);
		}
	}

	// Token: 0x060032DE RID: 13022 RVA: 0x00190F84 File Offset: 0x0018F184
	private void OnDisable()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.SystemOption) && !this._hasChanged;
		if (flag)
		{
			AudioManager.Instance.EnableMusicVolumeRate(0.2f);
			AudioManager.Instance.SetMusicVolumeWithFade(1f, 0f);
		}
		this._hasChanged = false;
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x00190FE1 File Offset: 0x0018F1E1
	public override void QuickHide()
	{
		base.QuickHide();
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
	}

	// Token: 0x060032E0 RID: 13024 RVA: 0x00191000 File Offset: 0x0018F200
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x060032E1 RID: 13025 RVA: 0x0019102C File Offset: 0x0018F22C
	public void InitAudioSettings()
	{
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		CToggleObsolete bgmTog = base.CGet<CToggleObsolete>("MusicToggle");
		TextMeshProUGUI bgmVolume = base.CGet<TextMeshProUGUI>("MusicValue");
		CSliderLegacy bgmVolumeSlider = base.CGet<CSliderLegacy>("MusicValueSlider");
		CToggleObsolete seTog = base.CGet<CToggleObsolete>("SoundEffectToggle");
		TextMeshProUGUI seVolume = base.CGet<TextMeshProUGUI>("SoundEffectValue");
		CSliderLegacy seVolumeSlider = base.CGet<CSliderLegacy>("SoundEffectValueSlider");
		bgmTog.onValueChanged.RemoveAllListeners();
		bgmVolumeSlider.onValueChanged.RemoveAllListeners();
		bgmTog.isOn = settingData.BgmOn;
		bgmVolume.text = (settingData.BgmOn ? settingData.BgmVolume.ToString() : "0");
		bgmVolumeSlider.value = (float)(settingData.BgmVolume / 10);
		bgmTog.onValueChanged.AddListener(delegate(bool isOn)
		{
			bgmVolumeSlider.interactable = isOn;
			settingData.BgmOn = isOn;
			bgmVolume.text = (isOn ? settingData.BgmVolume.ToString() : "0");
			this.CGet<GameObject>("MusicDisabledHandle").SetActive(!isOn);
			this._hasChanged = true;
		});
		base.CGet<GameObject>("MusicDisabledHandle").SetActive(!bgmTog.isOn);
		bgmVolumeSlider.interactable = settingData.BgmOn;
		bgmVolumeSlider.onValueChanged.AddListener(delegate(float value)
		{
			sbyte volume = (sbyte)(value * 10f);
			bgmVolume.text = volume.ToString();
			settingData.BgmVolume = volume;
			this._hasChanged = true;
		});
		seTog.onValueChanged.RemoveAllListeners();
		seVolumeSlider.onValueChanged.RemoveAllListeners();
		seTog.isOn = settingData.SeOn;
		seVolume.text = (settingData.SeOn ? settingData.SeVolume.ToString() : "0");
		seVolumeSlider.value = (float)(settingData.SeVolume / 10);
		seTog.onValueChanged.AddListener(delegate(bool isOn)
		{
			seVolumeSlider.interactable = isOn;
			bool flag = this.CloseSeOnAlways();
			if (flag)
			{
				settingData.SeOn = false;
			}
			else
			{
				settingData.SeOn = isOn;
			}
			seVolume.text = (isOn ? settingData.SeVolume.ToString() : "0");
			this.CGet<GameObject>("SoundDisabledHandle").SetActive(!isOn);
			this._hasChanged = true;
		});
		base.CGet<GameObject>("SoundDisabledHandle").SetActive(!seTog.isOn);
		seVolumeSlider.interactable = settingData.SeOn;
		seVolumeSlider.onValueChanged.AddListener(delegate(float value)
		{
			sbyte volume = (sbyte)(value * 10f);
			seVolume.text = volume.ToString();
			settingData.SeVolume = volume;
			AudioManager.Instance.PlaySound("ui_default_increase", false, false);
			this._hasChanged = true;
		});
		CToggleGroupObsolete muteIfNotFocus = base.CGet<CToggleGroupObsolete>("MuteIfNotFocus");
		muteIfNotFocus.InitPreOnToggle(-1);
		muteIfNotFocus.Set(settingData.MuteIfNotFocus ? 0 : 1, true, false);
		muteIfNotFocus.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnMuteIfNotFocusToggleChange);
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x001912B8 File Offset: 0x0018F4B8
	private void OnMuteIfNotFocusToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog == null;
		if (!flag)
		{
			SingletonObject.getInstance<GlobalSettings>().MuteIfNotFocus = (newTog.Key == 0);
		}
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x001912E8 File Offset: 0x0018F4E8
	private bool CloseSeOnAlways()
	{
		bool closeSeOnAlways = false;
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		if (flag)
		{
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			closeSeOnAlways = worldMapModel.CrossArchiveLockMoveTime;
		}
		return closeSeOnAlways;
	}

	// Token: 0x0400252E RID: 9518
	private bool _hasChanged;
}
