using System;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class UI_MapInfoOption : UIBase
{
	// Token: 0x0600277B RID: 10107 RVA: 0x00123340 File Offset: 0x00121540
	public override void OnInit(ArgumentBox argsBox)
	{
		this.Show();
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x0012334C File Offset: 0x0012154C
	private void Show()
	{
		RectTransform optionLayout = base.CGet<RectTransform>("Options");
		int professionId = SingletonObject.getInstance<ProfessionModel>().TaiwuCurrProfessionId;
		for (int i = 0; i < optionLayout.childCount; i++)
		{
			GameObject child = optionLayout.GetChild(i).gameObject;
			this.SetActive(child, false);
		}
		if (!true)
		{
		}
		string text;
		if (professionId <= 1)
		{
			if (professionId == 0)
			{
				text = "Savage";
				goto IL_D5;
			}
			if (professionId == 1)
			{
				text = "Hunter";
				goto IL_D5;
			}
		}
		else
		{
			if (professionId == 5)
			{
				text = "TaoistMonk";
				goto IL_D5;
			}
			switch (professionId)
			{
			case 10:
				text = "Civilian";
				goto IL_D5;
			case 11:
			case 12:
				break;
			case 13:
				text = "Doctor";
				goto IL_D5;
			case 14:
				text = "TravelingTaoistMonk";
				goto IL_D5;
			default:
				if (professionId == 17)
				{
					text = "Duke";
					goto IL_D5;
				}
				break;
			}
		}
		text = string.Empty;
		IL_D5:
		if (!true)
		{
		}
		string professionName = text;
		bool flag = !professionName.IsNullOrEmpty();
		if (flag)
		{
			this.SetActive(base.CGet<GameObject>(professionName), true);
		}
		bool showTreasure = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
		this.SetActive(base.CGet<GameObject>("Treasure"), showTreasure);
		this.Init();
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x0012347C File Offset: 0x0012167C
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Mask";
		if (flag)
		{
			this.QuickHide();
		}
		GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
		string name = btn.name;
		string text = name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1284028883U)
		{
			if (num <= 526977061U)
			{
				if (num != 521774151U)
				{
					if (num == 526977061U)
					{
						if (text == "DestroyedBlock")
						{
							this.ClickButton("DestroyedBlock", ref globalSettings.ShowSavageSkillDestroyedBlock);
						}
					}
				}
				else if (text == "Behavior")
				{
					this.ClickButton("Behavior", ref globalSettings.ShowTravelingBuddhistMonkSkillRebelAndEgoisticPeople);
				}
			}
			else if (num != 1065164893U)
			{
				if (num != 1201140803U)
				{
					if (num == 1284028883U)
					{
						if (text == "PersonalEnemy")
						{
							this.ClickButton("PersonalEnemy", ref globalSettings.ShowCivilianSkillPersonalEnemy);
						}
					}
				}
				else if (text == "PartlyInfected")
				{
					this.ClickButton("PartlyInfected", ref globalSettings.ShowTaoistMonkSkillPartlyInfected);
				}
			}
			else if (text == "CompletelyInfected")
			{
				this.ClickButton("CompletelyInfected", ref globalSettings.ShowTaoistMonkSkillCompletelyInfected);
			}
		}
		else if (num <= 3134516746U)
		{
			if (num != 3002756941U)
			{
				if (num == 3134516746U)
				{
					if (text == "DisorderOfQi")
					{
						this.ClickButton("DisorderOfQi", ref globalSettings.ShowDoctorSkillDisorderPeople);
					}
				}
			}
			else if (text == "AnimalInfo")
			{
				this.ClickButton("AnimalInfo", ref globalSettings.ShowHunterSkillAnimal);
			}
		}
		else if (num != 3158204625U)
		{
			if (num != 3269364858U)
			{
				if (num == 3807005144U)
				{
					if (text == "Hurt")
					{
						this.ClickButton("Hurt", ref globalSettings.ShowDoctorSkillHurtPeople);
					}
				}
			}
			else if (text == "GiveTitle")
			{
				this.ClickButton("GiveTitle", ref globalSettings.ShowDukeSkillGiveTitle);
			}
		}
		else if (text == "Poison")
		{
			this.ClickButton("Poison", ref globalSettings.ShowDoctorSkillPoisonedPeople);
		}
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x00123714 File Offset: 0x00121914
	private void SetActive(GameObject obj, bool active)
	{
		DisableStyleRoot[] disableStyleRoots = obj.GetComponentsInChildren<DisableStyleRoot>();
		bool flag = disableStyleRoots != null;
		if (flag)
		{
			foreach (DisableStyleRoot disableStyleRoot in disableStyleRoots)
			{
				disableStyleRoot.SetStyleEffect(!active, false);
			}
		}
		CButtonObsolete[] buttons = obj.GetComponentsInChildren<CButtonObsolete>();
		bool flag2 = buttons != null;
		if (flag2)
		{
			foreach (CButtonObsolete button in buttons)
			{
				button.interactable = active;
			}
		}
		CToggleObsolete[] toggles = obj.GetComponentsInChildren<CToggleObsolete>();
		bool flag3 = toggles != null;
		if (flag3)
		{
			foreach (CToggleObsolete toggle in toggles)
			{
				toggle.interactable = active;
			}
		}
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x001237CC File Offset: 0x001219CC
	private void Init()
	{
		GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
		GameObject treasure = base.CGet<GameObject>("Treasure");
		CToggleGroupObsolete toggleGroup = treasure.GetComponent<CToggleGroupObsolete>();
		int activeKey = globalSettings.ShowTreasure ? 0 : 1;
		toggleGroup.InitPreOnToggle(-1);
		toggleGroup.Set(activeKey, true, false);
		toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnTreasureActiveToggleChange);
		this.SetCheckMark("DestroyedBlock", globalSettings.ShowSavageSkillDestroyedBlock);
		this.SetCheckMark("AnimalInfo", globalSettings.ShowHunterSkillAnimal);
		this.SetCheckMark("PartlyInfected", globalSettings.ShowTaoistMonkSkillPartlyInfected);
		this.SetCheckMark("CompletelyInfected", globalSettings.ShowTaoistMonkSkillCompletelyInfected);
		this.SetCheckMark("PersonalEnemy", globalSettings.ShowCivilianSkillPersonalEnemy);
		this.SetCheckMark("Behavior", globalSettings.ShowTravelingBuddhistMonkSkillRebelAndEgoisticPeople);
		this.SetCheckMark("Hurt", globalSettings.ShowDoctorSkillHurtPeople);
		this.SetCheckMark("Poison", globalSettings.ShowDoctorSkillPoisonedPeople);
		this.SetCheckMark("DisorderOfQi", globalSettings.ShowDoctorSkillDisorderPeople);
		this.SetCheckMark("GiveTitle", globalSettings.ShowDukeSkillGiveTitle);
		this.InitTitle("DestroyedBlock", 0);
		this.InitTitle("AnimalInfo", 1);
		this.InitTitle("PartlyInfected", 5);
		this.InitTitle("CompletelyInfected", 5);
		this.InitTitle("PersonalEnemy", 10);
		this.InitTitle("Behavior", 12);
		this.InitTitle("Hurt", 13);
		this.InitTitle("Poison", 13);
		this.InitTitle("DisorderOfQi", 13);
		this.InitTitle("GiveTitle", 17);
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x00123960 File Offset: 0x00121B60
	private void ClickButton(string name, ref bool check)
	{
		check = !check;
		this.SetCheckMark(name, check);
		SingletonObject.getInstance<GlobalSettings>().SaveSettings();
		SingletonObject.getInstance<WorldMapModel>().UpdateViewModeData();
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x0012398C File Offset: 0x00121B8C
	private void SetCheckMark(string name, bool check)
	{
		GameObject root = base.CGet<GameObject>(name);
		root.GetComponent<Refers>().CGet<GameObject>("CheckMark").SetActive(check);
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x001239BC File Offset: 0x00121BBC
	private void InitTitle(string name, ushort configKey)
	{
		GameObject root = base.CGet<GameObject>(name);
		TextMeshProUGUI title = root.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
		title.text = Profession.Instance[(int)configKey].Name;
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x001239FC File Offset: 0x00121BFC
	private void OnTreasureActiveToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
		globalSettings.ShowTreasure = (newTog.Key == 0);
		globalSettings.SaveSettings();
		SingletonObject.getInstance<WorldMapModel>().UpdateViewModeData();
	}

	// Token: 0x04001CBD RID: 7357
	private const string DestroyedBlock = "DestroyedBlock";

	// Token: 0x04001CBE RID: 7358
	private const string AnimalInfo = "AnimalInfo";

	// Token: 0x04001CBF RID: 7359
	private const string PartlyInfected = "PartlyInfected";

	// Token: 0x04001CC0 RID: 7360
	private const string CompletelyInfected = "CompletelyInfected";

	// Token: 0x04001CC1 RID: 7361
	private const string PersonalEnemy = "PersonalEnemy";

	// Token: 0x04001CC2 RID: 7362
	private const string Behavior = "Behavior";

	// Token: 0x04001CC3 RID: 7363
	private const string Hurt = "Hurt";

	// Token: 0x04001CC4 RID: 7364
	private const string Poison = "Poison";

	// Token: 0x04001CC5 RID: 7365
	private const string DisorderOfQi = "DisorderOfQi";

	// Token: 0x04001CC6 RID: 7366
	private const string GiveTitle = "GiveTitle";
}
