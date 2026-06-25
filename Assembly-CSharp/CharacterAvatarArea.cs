using System;
using System.Collections.Generic;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200031A RID: 794
public class CharacterAvatarArea : UIBehaviour
{
	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x06002E89 RID: 11913 RVA: 0x0016F447 File Offset: 0x0016D647
	// (set) Token: 0x06002E8A RID: 11914 RVA: 0x0016F44F File Offset: 0x0016D64F
	public bool Installed { get; private set; }

	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x06002E8B RID: 11915 RVA: 0x0016F458 File Offset: 0x0016D658
	public bool HasAgeHealthMonitor
	{
		get
		{
			return this._ageHealthMonitor != null;
		}
	}

	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x06002E8C RID: 11916 RVA: 0x0016F463 File Offset: 0x0016D663
	public bool HasAvatarInfoMonitor
	{
		get
		{
			return this._avatarInfoMonitor != null;
		}
	}

	// Token: 0x17000519 RID: 1305
	// (get) Token: 0x06002E8D RID: 11917 RVA: 0x0016F46E File Offset: 0x0016D66E
	public short ActualAge
	{
		get
		{
			return this._ageHealthMonitor.ActualAge;
		}
	}

	// Token: 0x1700051A RID: 1306
	// (get) Token: 0x06002E8E RID: 11918 RVA: 0x0016F47B File Offset: 0x0016D67B
	public byte CreatingType
	{
		get
		{
			return this._avatarInfoMonitor.CreatingType;
		}
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x0016F488 File Offset: 0x0016D688
	public void Install(IList<CharacterUIElement> elements)
	{
		bool installed = this.Installed;
		if (!installed)
		{
			bool flag = this.characterHealthInfo;
			if (flag)
			{
				this._elementHealth = new CharacterHealth(this.characterHealthInfo);
				this._elementHealth.SetGetHealthStringFunc(new Func<short[], int, string>(this.GetHealthString));
			}
			bool flag2 = this.labelName;
			if (flag2)
			{
				this._elementName = new CharacterName(this.labelName, null, null);
			}
			bool flag3 = this.avatar;
			if (flag3)
			{
				this._elementAvatar = new CharacterAvatar(this.avatar, true);
			}
			bool flag4 = elements != null;
			if (flag4)
			{
				bool flag5 = this._elementHealth != null;
				if (flag5)
				{
					elements.Add(this._elementHealth);
				}
				bool flag6 = this._elementName != null;
				if (flag6)
				{
					elements.Add(this._elementName);
				}
				bool flag7 = this._elementAvatar != null;
				if (flag7)
				{
					elements.Add(this._elementAvatar);
				}
			}
			this.Installed = true;
		}
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x0016F590 File Offset: 0x0016D790
	public void Uninstall(Action onHealthChange = null, Action onAvatarDataChange = null)
	{
		bool flag = !this.Installed;
		if (!flag)
		{
			bool flag2 = this._elementHealth != null;
			if (flag2)
			{
				this._elementHealth.CharacterId = -1;
			}
			bool flag3 = this._elementName != null;
			if (flag3)
			{
				this._elementName.CharacterId = -1;
			}
			bool flag4 = this._elementAvatar != null;
			if (flag4)
			{
				this._elementAvatar.CharacterId = -1;
			}
			bool flag5 = this._ageHealthMonitor != null;
			if (flag5)
			{
				bool flag6 = onHealthChange != null;
				if (flag6)
				{
					this._ageHealthMonitor.RemoveOnHealthChangeEventListener(onHealthChange);
				}
				this._ageHealthMonitor.RemoveOnAgeChangeEventListener(new Action(this.UpdateAgeInfo));
				this._ageHealthMonitor.RemoveOnPhysiologicalAgeChangeEventListener(new Action(this.UpdateAge));
				this._ageHealthMonitor.RemoveTemplateIdListener(new Action(this.UpdateAge));
			}
			bool flag7 = onAvatarDataChange != null;
			if (flag7)
			{
				AvatarInfoMonitor avatarInfoMonitor = this._avatarInfoMonitor;
				if (avatarInfoMonitor != null)
				{
					avatarInfoMonitor.RemoveOnAvatarDataChangeEventListener(onAvatarDataChange);
				}
			}
			this._ageHealthMonitor = null;
			this._avatarInfoMonitor = null;
			this.Installed = false;
		}
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x0016F6A4 File Offset: 0x0016D8A4
	public void Setup(int characterId, Action onHealthChange = null, Action onAvatarDataChange = null)
	{
		CharacterMonitorModel monitorModel = SingletonObject.getInstance<CharacterMonitorModel>();
		bool flag = this._ageHealthMonitor != null;
		if (flag)
		{
			bool flag2 = onHealthChange != null;
			if (flag2)
			{
				this._ageHealthMonitor.RemoveOnHealthChangeEventListener(onHealthChange);
			}
			this._ageHealthMonitor.RemoveOnAgeChangeEventListener(new Action(this.UpdateAgeInfo));
			this._ageHealthMonitor.RemoveOnPhysiologicalAgeChangeEventListener(new Action(this.UpdateAge));
			this._ageHealthMonitor.RemoveTemplateIdListener(new Action(this.UpdateAge));
		}
		bool flag3 = this._elementHealth != null;
		if (flag3)
		{
			this._elementHealth.CharacterId = characterId;
			this._elementHealth.GearMateMode = monitorModel.IsTaiwuGearMate(characterId);
			this._ageHealthMonitor = this._elementHealth.GetMonitor<AgeHealthMonitor>();
			bool flag4 = onHealthChange != null;
			if (flag4)
			{
				this._ageHealthMonitor.AddOnHealthChangeEventListener(onHealthChange);
			}
			this._ageHealthMonitor.AddOnAgeChangeEventListener(new Action(this.UpdateAgeInfo));
			this._ageHealthMonitor.AddOnPhysiologicalAgeChangeEventListener(new Action(this.UpdateAge));
			this._ageHealthMonitor.AddTemplateIdListener(new Action(this.UpdateAge));
			this._ageHealthMonitor.Refresh();
		}
		bool flag5 = this._elementName != null;
		if (flag5)
		{
			this._elementName.CharacterId = characterId;
		}
		bool flag6 = this._elementAvatar != null;
		if (flag6)
		{
			this._elementAvatar.CharacterId = characterId;
		}
		this._avatarInfoMonitor = monitorModel.GetMonitorItem<AvatarInfoMonitor>(characterId, false);
		bool init = this._avatarInfoMonitor.Init;
		if (init)
		{
			if (onAvatarDataChange != null)
			{
				onAvatarDataChange();
			}
		}
		else
		{
			bool flag7 = onAvatarDataChange != null;
			if (flag7)
			{
				this._avatarInfoMonitor.AddOnAvatarDataChangeEventListener(onAvatarDataChange);
			}
		}
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x0016F858 File Offset: 0x0016DA58
	private string GetHealthString(short[] paramsHealth, int characterId)
	{
		this._avatarInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(characterId, false);
		bool flag = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(characterId);
		if (flag)
		{
			paramsHealth[0] = paramsHealth[1];
		}
		EHealthType type = CommonUtils.GetHealthType(paramsHealth[0], paramsHealth[1], characterId);
		string text = CommonUtils.GetHealthString(type);
		bool flag2 = type == EHealthType.Healthy || type == EHealthType.Sick || type == EHealthType.Unknown;
		if (flag2)
		{
			this.healthBetter.SetActive(true);
			this.healthWorse.SetActive(false);
			this.healthDying.SetActive(false);
		}
		else
		{
			bool flag3 = type == EHealthType.Dying;
			if (flag3)
			{
				this.healthBetter.SetActive(false);
				this.healthWorse.SetActive(false);
				this.healthDying.SetActive(true);
			}
			else
			{
				this.healthBetter.SetActive(false);
				this.healthWorse.SetActive(true);
				this.healthDying.SetActive(false);
			}
		}
		return text;
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x0016F948 File Offset: 0x0016DB48
	private void UpdateAgeInfo()
	{
		int birthMonth = (int)(this._ageHealthMonitor.BirthMonth % 12);
		bool flag = birthMonth < 0;
		if (flag)
		{
			birthMonth += 12;
		}
		MonthItem monthConfig = Month.Instance[birthMonth];
		CImage fiveElementsIcon = this.innateFiveElementsType;
		TooltipInvoker fiveElementsTips = fiveElementsIcon.GetComponent<TooltipInvoker>();
		this.UpdateAge();
		this.birth.text = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Tips, monthConfig.Name);
		fiveElementsIcon.SetSprite(string.Format("ui_sp_icon_fiveelements_{0}", monthConfig.FiveElementsType), false, null);
		TooltipInvoker tooltipInvoker = fiveElementsTips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		fiveElementsTips.RuntimeParam.Set("BirthMonth", birthMonth);
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x0016FA00 File Offset: 0x0016DC00
	private void UpdateAge()
	{
		bool flag = this._ageHealthMonitor.TemplateId < 0;
		if (!flag)
		{
			byte creatingType = this._avatarInfoMonitor.CreatingType;
			bool isNonEvolutionaryType = GameData.Domains.Character.Creation.CreatingType.IsNonEvolutionaryType(creatingType);
			this.age.text = ((Character.Instance[this._ageHealthMonitor.TemplateId].HideAge && isNonEvolutionaryType) ? "-" : LocalStringManager.GetFormat(LanguageKey.LK_Age, this._ageHealthMonitor.PhysiologicalAge));
			TooltipInvoker ageMouseTip = this.ageTips;
			CharacterItem charItem = Character.Instance.GetItem(this._avatarInfoMonitor.TemplateId);
			bool flag2 = isNonEvolutionaryType || (charItem != null && charItem.HideAge);
			if (flag2)
			{
				ageMouseTip.enabled = false;
			}
			else
			{
				ageMouseTip.enabled = true;
				int lineCount = 0;
				TooltipInvoker tooltipInvoker = ageMouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				ageMouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Char_Age));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Age_Tip_Desc)
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 20f
				});
				StringBuilder ageInfo = new StringBuilder();
				ageInfo.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Age_DisplayAge_FormatBase, this._ageHealthMonitor.PhysiologicalAge).SetColor("pinkyellow"));
				bool flag3 = (this._ageHealthMonitor.AgeAffector & AgeAffector.TaoismPassive) > AgeAffector.None;
				if (flag3)
				{
					ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Profession_TaoismPassive.Tr());
				}
				else
				{
					bool flag4 = (this._ageHealthMonitor.AgeAffector & AgeAffector.TaoismActive) > AgeAffector.None;
					if (flag4)
					{
						ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Profession_TaoismActive.Tr());
					}
				}
				bool flag5 = (this._ageHealthMonitor.AgeAffector & AgeAffector.MaleKeepYoung) > AgeAffector.None;
				if (flag5)
				{
					ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Gongfa_Xisui.Tr());
				}
				else
				{
					bool flag6 = (this._ageHealthMonitor.AgeAffector & AgeAffector.FemaleKeepYoung) > AgeAffector.None;
					if (flag6)
					{
						ageInfo.AppendLine(LanguageKey.LK_Age_DisplayAge_Gongfa_Taiyin.Tr());
					}
				}
				ageInfo.Remove(ageInfo.Length - 1, 1);
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					"mousetip_age_0",
					ageInfo.ToString()
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				});
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_1).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_2).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_3).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_DisplayAge_TipContent_4).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(4, null, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					"mousetip_age_1",
					LocalStringManager.GetFormat(LanguageKey.LK_Age_ActualAge_FormatBase, this._ageHealthMonitor.ActualAge).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				});
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_ActualAge_TipContent_1).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_Age_ActualAge_TipContent_2).SetColor("pinkyellow")
				}, null));
				ageMouseTip.RuntimeParam.Set("LineCount", lineCount);
			}
		}
	}

	// Token: 0x040021C2 RID: 8642
	[SerializeField]
	private CharacterHealthBar characterHealthInfo;

	// Token: 0x040021C3 RID: 8643
	[SerializeField]
	private GameObject healthBetter;

	// Token: 0x040021C4 RID: 8644
	[SerializeField]
	private GameObject healthWorse;

	// Token: 0x040021C5 RID: 8645
	[SerializeField]
	private GameObject healthDying;

	// Token: 0x040021C6 RID: 8646
	[SerializeField]
	private CImage innateFiveElementsType;

	// Token: 0x040021C7 RID: 8647
	[SerializeField]
	private TextMeshProUGUI birth;

	// Token: 0x040021C8 RID: 8648
	[SerializeField]
	private TextMeshProUGUI age;

	// Token: 0x040021C9 RID: 8649
	[SerializeField]
	private TooltipInvoker ageTips;

	// Token: 0x040021CA RID: 8650
	[SerializeField]
	private TextMeshProUGUI labelName;

	// Token: 0x040021CB RID: 8651
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x040021CC RID: 8652
	private CharacterHealth _elementHealth;

	// Token: 0x040021CD RID: 8653
	private CharacterName _elementName;

	// Token: 0x040021CE RID: 8654
	private CharacterAvatar _elementAvatar;

	// Token: 0x040021CF RID: 8655
	private AgeHealthMonitor _ageHealthMonitor;

	// Token: 0x040021D0 RID: 8656
	private AvatarInfoMonitor _avatarInfoMonitor;
}
