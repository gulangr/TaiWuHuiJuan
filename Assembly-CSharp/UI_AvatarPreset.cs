using System;
using System.Collections.Generic;
using System.IO;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200036D RID: 877
public class UI_AvatarPreset : UIBase
{
	// Token: 0x060032E5 RID: 13029 RVA: 0x00191326 File Offset: 0x0018F526
	private void OnEnable()
	{
	}

	// Token: 0x060032E6 RID: 13030 RVA: 0x00191329 File Offset: 0x0018F529
	private void OnDisable()
	{
	}

	// Token: 0x060032E7 RID: 13031 RVA: 0x0019132C File Offset: 0x0018F52C
	public override void OnReset()
	{
		LanguageKey langId = (this._gender == 1) ? LanguageKey.UI_NewGame_FemaleLike : LanguageKey.UI_NewGame_MaleLike;
		base.CGet<TextMeshProUGUI>("ConverseGenderOffLabel").text = LocalStringManager.Get(langId);
		base.CGet<TextMeshProUGUI>("ConverseGenderOnLabel").text = LocalStringManager.Get(langId);
	}

	// Token: 0x060032E8 RID: 13032 RVA: 0x00191380 File Offset: 0x0018F580
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = this._age == 0;
		if (flag)
		{
			this._age = 16;
		}
		CSliderLegacy ageSlider = base.CGet<CSliderLegacy>("AgeSlider");
		ageSlider.minValue = 16f;
		ageSlider.maxValue = (float)GlobalConfig.Instance.MaxAgeOfCreatingChar;
		ageSlider.value = (float)this._age;
		base.CGet<TextMeshProUGUI>("AgeText").text = LocalStringManager.GetFormat(LanguageKey.LK_Age, this._age);
		ageSlider.onValueChanged.RemoveAllListeners();
		ageSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnAgeSliderValueChange));
		CToggleGroupObsolete genderToggleGroup = base.CGet<CToggleGroupObsolete>("CommonInfos");
		genderToggleGroup.InitPreOnToggle(-1);
		this._gender = ((genderToggleGroup.GetActive().Key == 1) ? 1 : 0);
		genderToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnGenderToggleChange);
		base.CGet<CToggleObsolete>("ConverseGender").onValueChanged.RemoveAllListeners();
		base.CGet<CToggleObsolete>("ConverseGender").onValueChanged.AddListener(new UnityAction<bool>(this.OnGenderConverseToggleValueChanged));
		Game.Components.Avatar.Avatar[] updateAvatars = new Game.Components.Avatar.Avatar[]
		{
			base.CGet<Game.Components.Avatar.Avatar>("Avatar"),
			base.CGet<Game.Components.Avatar.Avatar>("Avatar_SmallSize"),
			base.CGet<Game.Components.Avatar.Avatar>("Avatar_NormalSize")
		};
		base.CGet<AvatarAdjustController>("AdjustRoot").Init(ref this._avatarData, this._gender, (short)this._age, updateAvatars, true, false);
	}

	// Token: 0x060032E9 RID: 13033 RVA: 0x001914F4 File Offset: 0x0018F6F4
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "BtnLoad" == btnName;
		if (flag)
		{
			UI_FileExplorer.Command command = new UI_FileExplorer.Command();
			command.OnExploreComplete = new Func<string, bool>(this.LoadAvatar);
			command.Title = LocalStringManager.Get(LanguageKey.UI_AvatarPreset_SelectAvatarPreset);
			command.InitialPath = ModManager.GetPublishAvatarPreset();
			command.FileFilter = "*.twa";
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.SetObject("Cmd", command);
			UIElement.FileExplorer.SetOnInitArgs(box);
			UIManager.Instance.ShowUI(UIElement.FileExplorer, true);
		}
		else
		{
			bool flag2 = "BtnSave" == btnName;
			if (flag2)
			{
				UI_FileExplorer.Command command2 = new UI_FileExplorer.Command();
				command2.OnExploreComplete = new Func<string, bool>(this.SaveAvatar);
				command2.Title = LocalStringManager.Get(LanguageKey.UI_AvatarPreset_Save);
				command2.InitialPath = ModManager.GetPublishAvatarPreset();
				command2.IsSaveAction = true;
				command2.FileFilter = "*.twa";
				ArgumentBox box2 = EasyPool.Get<ArgumentBox>();
				box2.SetObject("Cmd", command2);
				UIElement.FileExplorer.SetOnInitArgs(box2);
				UIManager.Instance.ShowUI(UIElement.FileExplorer, true);
			}
			else
			{
				bool flag3 = "RandomAvatar" == btnName;
				if (flag3)
				{
					base.CGet<AvatarAdjustController>("AdjustRoot").RandomAvatar();
				}
				else
				{
					bool flag4 = "BtnClose" == btnName;
					if (flag4)
					{
						this.QuickHide();
					}
				}
			}
		}
	}

	// Token: 0x060032EA RID: 13034 RVA: 0x00191664 File Offset: 0x0018F864
	public override void QuickHide()
	{
		this._avatarData = null;
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x060032EB RID: 13035 RVA: 0x00191680 File Offset: 0x0018F880
	private void OnAgeSliderValueChange(float newAge)
	{
		this._age = (byte)newAge;
		base.CGet<TextMeshProUGUI>("AgeText").text = LocalStringManager.GetFormat(LanguageKey.LK_Age, this._age);
		base.CGet<AvatarAdjustController>("AdjustRoot").SetAge((short)this._age);
	}

	// Token: 0x060032EC RID: 13036 RVA: 0x001916D4 File Offset: 0x0018F8D4
	private void OnGenderToggleChange(CToggleObsolete newToggle, CToggleObsolete preToggle)
	{
		this._gender = ((newToggle.Key == 1) ? 1 : 0);
		AvatarAdjustController controller = base.CGet<AvatarAdjustController>("AdjustRoot");
		controller.SetGender(this._gender);
		LanguageKey langId = (this._gender == 1) ? LanguageKey.UI_NewGame_FemaleLike : LanguageKey.UI_NewGame_MaleLike;
		base.CGet<TextMeshProUGUI>("ConverseGenderOffLabel").text = LocalStringManager.Get(langId);
		base.CGet<TextMeshProUGUI>("ConverseGenderOnLabel").text = LocalStringManager.Get(langId);
	}

	// Token: 0x060032ED RID: 13037 RVA: 0x00191754 File Offset: 0x0018F954
	private void OnGenderConverseToggleValueChanged(bool isOn)
	{
		AvatarAdjustController controller = base.CGet<AvatarAdjustController>("AdjustRoot");
		controller.SetTransGender(isOn);
	}

	// Token: 0x060032EE RID: 13038 RVA: 0x00191778 File Offset: 0x0018F978
	private bool LoadAvatar(string path)
	{
		Dictionary<string, string> dataMap = new Dictionary<string, string>();
		AvatarData avatarData = AvatarDataSaveLoadHelper.Load(path, ref dataMap);
		base.CGet<AvatarAdjustController>("AdjustRoot").SetAvatarData(avatarData);
		string str;
		sbyte transGenderFlag;
		bool flag = dataMap.TryGetValue("TransGender", out str) && sbyte.TryParse(str, out transGenderFlag);
		if (flag)
		{
			base.CGet<CToggleObsolete>("ConverseGender").isOn = (transGenderFlag == 1);
		}
		string strGender;
		sbyte gender;
		bool flag2 = dataMap.TryGetValue("Gender", out strGender) && sbyte.TryParse(strGender, out gender);
		if (flag2)
		{
			int onKey = (gender == 1) ? 1 : 2;
			base.CGet<CToggleGroupObsolete>("CommonInfos").Set(onKey, true, false);
		}
		string strAge;
		sbyte age;
		bool flag3 = dataMap.TryGetValue("DisplayAge", out strAge) && sbyte.TryParse(strAge, out age);
		if (flag3)
		{
			base.CGet<CSliderLegacy>("AgeSlider").value = (float)age;
		}
		return true;
	}

	// Token: 0x060032EF RID: 13039 RVA: 0x00191860 File Offset: 0x0018FA60
	private bool SaveAvatar(string savePath)
	{
		UI_AvatarPreset.<>c__DisplayClass13_0 CS$<>8__locals1 = new UI_AvatarPreset.<>c__DisplayClass13_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.savePath = savePath;
		bool flag = string.IsNullOrEmpty(CS$<>8__locals1.savePath);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = Path.GetFileNameWithoutExtension(CS$<>8__locals1.savePath).HasInvalidCharForFileName();
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = !CS$<>8__locals1.savePath.EndsWith(".twa");
				if (flag3)
				{
					CS$<>8__locals1.savePath += ".twa";
				}
				bool flag4 = File.Exists(CS$<>8__locals1.savePath);
				if (flag4)
				{
					DialogCmd cmd = new DialogCmd();
					cmd.Type = 1;
					cmd.Title = LocalStringManager.Get(LanguageKey.LK_GameName);
					cmd.Content = LocalStringManager.GetFormat(LanguageKey.UI_AvatarPreset_PresetNameExist, CS$<>8__locals1.savePath);
					cmd.Yes = delegate()
					{
						base.<SaveAvatar>g__DoSave|0();
						UIManager.Instance.HideUI(UIElement.FileExplorer);
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
					result = false;
				}
				else
				{
					CS$<>8__locals1.<SaveAvatar>g__DoSave|0();
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x0400252F RID: 9519
	private AvatarData _avatarData;

	// Token: 0x04002530 RID: 9520
	private sbyte _gender = 1;

	// Token: 0x04002531 RID: 9521
	private byte _age;
}
