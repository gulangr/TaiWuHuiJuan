using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Global.Inscription;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x0200080B RID: 2059
	public class NewGameSubPageName : NewGameSubPage
	{
		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x06006505 RID: 25861 RVA: 0x002E3415 File Offset: 0x002E1615
		private sbyte Gender
		{
			get
			{
				return this.parent.Gender;
			}
		}

		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x06006506 RID: 25862 RVA: 0x002E3422 File Offset: 0x002E1622
		private sbyte OrgId
		{
			get
			{
				return this.parent.OrganizationTemplateId;
			}
		}

		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x06006507 RID: 25863 RVA: 0x002E342F File Offset: 0x002E162F
		public sbyte BirthMonth
		{
			get
			{
				return (sbyte)this.birthMonthToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x06006508 RID: 25864 RVA: 0x002E343D File Offset: 0x002E163D
		public sbyte NeiliType
		{
			get
			{
				return Month.Instance[this.BirthMonth].FiveElementsType;
			}
		}

		// Token: 0x06006509 RID: 25865 RVA: 0x002E3454 File Offset: 0x002E1654
		private string GetBehaviorImage(int index, short value)
		{
			int lightType = 3;
			bool flag = value == this._behaviorMorality[index];
			if (flag)
			{
				lightType = 0;
			}
			else
			{
				bool flag2 = index - 1 >= 0 && value >= this._behaviorEdge[index - 1][0] && value <= this._behaviorEdge[index - 1][1];
				if (flag2)
				{
					lightType = 1;
				}
				else
				{
					bool flag3 = index < this._behaviorEdge.Length && value >= this._behaviorEdge[index][0] && value <= this._behaviorEdge[index][1];
					if (flag3)
					{
						lightType = 2;
					}
				}
			}
			return string.Format("ui9_tex_new_game_behavior_{0}_{1}", index, lightType);
		}

		// Token: 0x0600650A RID: 25866 RVA: 0x002E34F6 File Offset: 0x002E16F6
		protected override void Awake()
		{
			this.InitNameArea();
			this.InitBirthArea();
			this.InitBehaviorArea();
			base.Awake();
		}

		// Token: 0x0600650B RID: 25867 RVA: 0x002E3515 File Offset: 0x002E1715
		private void OnEnable()
		{
			this.UpdateMonth();
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x0600650C RID: 25868 RVA: 0x002E3537 File Offset: 0x002E1737
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x0600650D RID: 25869 RVA: 0x002E3552 File Offset: 0x002E1752
		private void InitNameArea()
		{
			this.nameCharacter.Init(new Action<string, string>(this.OnRefreshNameCharacter), new Action(this.OnRandomNameClick));
		}

		// Token: 0x0600650E RID: 25870 RVA: 0x002E357C File Offset: 0x002E177C
		public override void Init()
		{
			string familyNameText = base.CreationInfoMap.GetValueOrDefault("Surname");
			string givenNameText = base.CreationInfoMap.GetValueOrDefault("GivenName");
			this.nameCharacter.Refresh(familyNameText, givenNameText);
			this.OnRefreshNameCharacter(familyNameText, givenNameText);
			string birthMonthString;
			sbyte birthMonth;
			bool flag = !base.CreationInfoMap.TryGetValue("BirthMonth", out birthMonthString) || !sbyte.TryParse(birthMonthString, out birthMonth);
			if (flag)
			{
				birthMonth = 0;
			}
			this.birthMonthToggleGroup.Set((int)birthMonth, false);
			string goodnessValueString;
			int value;
			bool flag2 = !base.CreationInfoMap.TryGetValue("Morality", out goodnessValueString) || !int.TryParse(goodnessValueString, out value);
			if (flag2)
			{
				value = 0;
			}
			this.UpdateBehavior((short)value);
		}

		// Token: 0x0600650F RID: 25871 RVA: 0x002E3634 File Offset: 0x002E1834
		private void InitBirthArea()
		{
			this.birthMonthToggleGroup.Init(-1);
			this.birthMonthToggleGroup.OnActiveIndexChange += this.OnBirthMonthChange;
			for (int i = 0; i < 12; i++)
			{
				CToggleNameAndLineHelper monthToggle = this.birthMonthToggleGroup.Get(i).GetComponent<CToggleNameAndLineHelper>();
				TooltipInvoker tips = monthToggle.Tips;
				monthToggle.SetName(Month.Instance[i].Name);
				TooltipInvoker tooltipInvoker = tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tips.RuntimeParam.Set("BirthMonth", i);
			}
			GridLayoutGroup gridLayoutGroup = this.birthMonthToggleGroup.GetComponent<GridLayoutGroup>();
			gridLayoutGroup.cellSize = gridLayoutGroup.cellSize.SetX((float)((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? 168 : 225));
		}

		// Token: 0x06006510 RID: 25872 RVA: 0x002E370C File Offset: 0x002E190C
		private void InitBehaviorArea()
		{
			for (int i = 0; i < this.behaviorButtons.childCount; i++)
			{
				int index = i;
				this.behaviorButtons.GetChild(index).GetComponent<CButton>().ClearAndAddListener(delegate
				{
					this.OnMoralityChange(this._behaviorMorality[index]);
				});
			}
			this.behaviorSlider.onValueChanged.AddListener(delegate(float behaviorValue)
			{
				this.OnMoralityChange((short)behaviorValue);
			});
		}

		// Token: 0x06006511 RID: 25873 RVA: 0x002E3790 File Offset: 0x002E1990
		private void OnRefreshNameCharacter(string familyNameStr, string givenNameStr)
		{
			this.toggleHelper.taiwuName.text = ((this.nameCharacter.FamilyName.IsNullOrEmpty() || this.nameCharacter.GivenName.IsNullOrEmpty()) ? LanguageKey.LK_NewGame_NoName.Tr().SetColor("brightred") : this.nameCharacter.FixedName);
			this._disableEnterGameReason = ((string.IsNullOrEmpty(familyNameStr) || string.IsNullOrEmpty(givenNameStr)) ? LanguageKey.UI_NewGame_CreateTip_Name.Tr() : ((NameCenter.HasInvalidCharForName(familyNameStr) || NameCenter.HasInvalidCharForName(givenNameStr)) ? LanguageKey.UI_NewGame_CreateTip_InvalidName.Tr() : ""));
			base.RefreshDisableEnterGameReason();
		}

		// Token: 0x06006512 RID: 25874 RVA: 0x002E383E File Offset: 0x002E1A3E
		private void OnRandomNameClick()
		{
			CharacterDomainMethod.Call.GenerateRandomHanName(this.parent.Element.GameDataListenerId, -1, -1, this.Gender);
		}

		// Token: 0x06006513 RID: 25875 RVA: 0x002E3860 File Offset: 0x002E1A60
		internal void OnGenerateRandomHanNameReturn(int offset, RawDataPool dataPool)
		{
			FullName fullName = default(FullName);
			Serializer.Deserialize(dataPool, offset, ref fullName);
			ValueTuple<string, string> name = fullName.GetName(this.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
			string familyNameText = name.Item1;
			string givenNameText = name.Item2;
			this.nameCharacter.Refresh(familyNameText, givenNameText);
		}

		// Token: 0x06006514 RID: 25876 RVA: 0x002E38B2 File Offset: 0x002E1AB2
		private void OnBirthMonthChange(int _, int __)
		{
			this.UpdateMonth();
		}

		// Token: 0x06006515 RID: 25877 RVA: 0x002E38BC File Offset: 0x002E1ABC
		private void OnMoralityChange(short value)
		{
			this.UpdateBehavior(value);
		}

		// Token: 0x06006516 RID: 25878 RVA: 0x002E38C7 File Offset: 0x002E1AC7
		private void OnTopUiChanged(ArgumentBox box)
		{
			this.behaviorSlider.interactable = UIManager.Instance.IsFocusElement(UIElement.NewGame);
		}

		// Token: 0x06006517 RID: 25879 RVA: 0x002E38E8 File Offset: 0x002E1AE8
		private void UpdateMonth()
		{
			int month = this.birthMonthToggleGroup.GetActiveIndex();
			int featureId = 184 + month;
			this.birthMonthDesc.text = CharacterFeature.Instance[featureId].Desc;
			this.birthSeason.SetSprite(string.Format("ui9_back_season_{0}", NewGameSubPageName._monthToSeason[month]), false, null);
			int with = 974;
			bool flag = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			if (flag)
			{
				with = 750;
			}
			Vector2 sizeDelta = this.birthMonthDesc.GetComponent<RectTransform>().sizeDelta;
			this.birthMonthDesc.GetComponent<RectTransform>().sizeDelta = new Vector2((float)with, sizeDelta.y);
		}

		// Token: 0x06006518 RID: 25880 RVA: 0x002E3998 File Offset: 0x002E1B98
		private void UpdateBehavior(short value)
		{
			sbyte type = GameData.Domains.Character.BehaviorType.GetBehaviorType(value);
			for (int i = 0; i < 5; i++)
			{
				CRawImage image = this.behaviorButtons.GetChild(i).GetComponent<CRawImage>();
				string textureName = this.GetBehaviorImage(i, value);
				image.SetTexture(textureName);
			}
			this.behaviorSlider.SetValueWithoutNotify((float)value);
			this.behaviorDesc.text = LocalStringManager.Get(string.Format("LK_Goodness_Desc_Color_{0}", type)).ColorReplace();
			string behaviorIconPath = string.Format("ui9_icon_new_game_circleback_behavior_{0}_{1}", type, 0);
			this.toggleHelper.behaviorIcon.SetSprite(behaviorIconPath, false, null);
			this.toggleHelper.behaviorName.text = LocalStringManager.Get(string.Format("LK_Goodness_{0}", type)).ColorReplace();
			this.behaviorDescIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_new_game_behavior_", type), false, null);
			this.behaviorDescIcon.gameObject.SetActive(true);
		}

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x06006519 RID: 25881 RVA: 0x002E3AA5 File Offset: 0x002E1CA5
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x0600651A RID: 25882 RVA: 0x002E3AA8 File Offset: 0x002E1CA8
		public override string DisableEnterGameReason
		{
			get
			{
				return this._disableEnterGameReason;
			}
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x0600651B RID: 25883 RVA: 0x002E3AB0 File Offset: 0x002E1CB0
		// (set) Token: 0x0600651C RID: 25884 RVA: 0x002E3AB3 File Offset: 0x002E1CB3
		public override bool StartGameChecked
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x0600651D RID: 25885 RVA: 0x002E3AB8 File Offset: 0x002E1CB8
		public void SetInscriptionCharacter(InscribedCharacter data)
		{
			this.nameCharacter.SetInteractable(data == null);
			bool flag = data == null;
			if (!flag)
			{
				this.nameCharacter.Refresh(data.Surname, data.GivenName);
				this.birthMonthToggleGroup.Set((int)data.BirthMonth, false);
				this.UpdateBehavior(data.Morality);
				this.OnRefreshNameCharacter(data.Surname, data.GivenName);
			}
		}

		// Token: 0x0600651E RID: 25886 RVA: 0x002E3B2C File Offset: 0x002E1D2C
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			bool useInscription = this.parent.UseInscription;
			if (!useInscription)
			{
				Dictionary<string, string> creationInfoMap = base.CreationInfoMap;
				protagonistCreationInfo.Surname = this.nameCharacter.FamilyName;
				creationInfoMap["Surname"] = protagonistCreationInfo.Surname;
				protagonistCreationInfo.GivenName = this.nameCharacter.GivenName;
				creationInfoMap["GivenName"] = protagonistCreationInfo.GivenName;
				protagonistCreationInfo.Morality = (short)this.behaviorSlider.value;
				creationInfoMap["Morality"] = protagonistCreationInfo.Morality.ToString();
				protagonistCreationInfo.BirthMonth = (sbyte)this.birthMonthToggleGroup.GetActiveIndex();
				base.CreationInfoMap["BirthMonth"] = this.BirthMonth.ToString();
			}
		}

		// Token: 0x04004651 RID: 18001
		[SerializeField]
		private NewGameSubPageNameToggleHelper toggleHelper;

		// Token: 0x04004652 RID: 18002
		[SerializeField]
		private NameCharacter nameCharacter;

		// Token: 0x04004653 RID: 18003
		[SerializeField]
		private CToggleGroup birthMonthToggleGroup;

		// Token: 0x04004654 RID: 18004
		[SerializeField]
		private TextMeshProUGUI birthMonthDesc;

		// Token: 0x04004655 RID: 18005
		[SerializeField]
		private CImage birthSeason;

		// Token: 0x04004656 RID: 18006
		[SerializeField]
		private Transform behaviorButtons;

		// Token: 0x04004657 RID: 18007
		[SerializeField]
		private CSlider behaviorSlider;

		// Token: 0x04004658 RID: 18008
		[SerializeField]
		private TextMeshProUGUI behaviorDesc;

		// Token: 0x04004659 RID: 18009
		[SerializeField]
		private CImage behaviorDescIcon;

		// Token: 0x0400465A RID: 18010
		private static Dictionary<int, int> _monthToSeason = new Dictionary<int, int>
		{
			{
				0,
				1
			},
			{
				1,
				1
			},
			{
				2,
				1
			},
			{
				3,
				2
			},
			{
				4,
				2
			},
			{
				5,
				2
			},
			{
				6,
				3
			},
			{
				7,
				3
			},
			{
				8,
				3
			},
			{
				9,
				0
			},
			{
				10,
				0
			},
			{
				11,
				0
			}
		};

		// Token: 0x0400465B RID: 18011
		private readonly short[] _behaviorMorality = new short[]
		{
			500,
			250,
			0,
			-250,
			-500
		};

		// Token: 0x0400465C RID: 18012
		private readonly short[][] _behaviorEdge = new short[][]
		{
			new short[]
			{
				251,
				499
			},
			new short[]
			{
				1,
				249
			},
			new short[]
			{
				-249,
				-1
			},
			new short[]
			{
				-499,
				-251
			}
		};

		// Token: 0x0400465D RID: 18013
		private string _disableEnterGameReason = "";
	}
}
