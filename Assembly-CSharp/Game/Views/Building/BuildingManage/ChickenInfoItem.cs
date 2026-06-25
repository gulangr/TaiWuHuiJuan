using System;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C09 RID: 3081
	public class ChickenInfoItem : MonoBehaviour
	{
		// Token: 0x1700108D RID: 4237
		// (get) Token: 0x06009C7E RID: 40062 RVA: 0x004948D6 File Offset: 0x00492AD6
		private ViewBuildingManage Parent
		{
			get
			{
				return UIElement.BuildingManage.UiBaseAs<ViewBuildingManage>();
			}
		}

		// Token: 0x06009C7F RID: 40063 RVA: 0x004948E2 File Offset: 0x00492AE2
		private void Awake()
		{
			this.btnRename.ClearAndAddListener(delegate
			{
				new RenameCfg
				{
					Title = LanguageKey.LK_Building_Chicken_Rename_Title.Tr(),
					Description = LanguageKey.LK_Building_Chicken_Rename_Desc.TrFormat(this._chickenData.NickName),
					EmptyDesc = LanguageKey.LK_Building_Chicken_Rename_Empty.Tr(),
					Default = this._chickenData.NickName,
					CharCount = 6,
					Submit = delegate(string x)
					{
						this.RenameConfirm(x);
						this.Parent.RequestData();
					}
				}.Show();
			});
		}

		// Token: 0x06009C80 RID: 40064 RVA: 0x00494900 File Offset: 0x00492B00
		public void Set(ChickenData data)
		{
			this._chickenData = data;
			ChickenItem chickenItem = Config.Chicken.Instance.GetItem(data.TemplateId);
			this.happiness.text = data.Happiness.ToString();
			TextMeshProUGUI textMeshProUGUI = this.personalityTitle;
			sbyte personalityType = chickenItem.PersonalityType;
			if (!true)
			{
			}
			string text;
			switch (personalityType)
			{
			case 0:
				text = LanguageKey.LK_Personality_Calm_Name.Tr();
				break;
			case 1:
				text = LanguageKey.LK_Personality_Clever_Name.Tr();
				break;
			case 2:
				text = LanguageKey.LK_Personality_Enthusiastic_Name.Tr();
				break;
			case 3:
				text = LanguageKey.LK_Personality_Brave_Name.Tr();
				break;
			case 4:
				text = LanguageKey.LK_Personality_Firm_Name.Tr();
				break;
			case 5:
				text = LanguageKey.LK_Personality_Lucky_Name.Tr();
				break;
			case 6:
				text = LanguageKey.LK_Personality_Perceptive_Name.Tr();
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(personalityType);
				break;
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
			this.personalityValue.text = chickenItem.PersonalityValue.ToString();
			this.desc.text = chickenItem.Desc;
			CharacterFeatureItem featureConfig = CharacterFeature.Instance[chickenItem.FeatureId];
			this.featureName.text = featureConfig.Name;
			this.featureDesc.text = featureConfig.Desc;
			TooltipInvoker featureTip = this.featureName.transform.parent.GetComponent<TooltipInvoker>();
			featureTip.Type = TipType.Feature;
			TooltipInvoker tooltipInvoker = featureTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			featureTip.RuntimeParam.Set("FeatureId", featureConfig.TemplateId);
			int indexMedal = 0;
			for (int i = 0; i < 3; i++)
			{
				FeatureMedals medals = featureConfig.FeatureMedals[i];
				foreach (sbyte medalType in medals.Values)
				{
					bool flag = indexMedal >= 3;
					if (flag)
					{
						break;
					}
					this.featureIcons[indexMedal++].SetSprite("ui9_icon_strategy_big_" + ChickenInfoItem.FeatureIconConfig[(int)medalType][i], false, null);
				}
			}
			Action<ItemKey> <>9__1;
			this.feedBtn.ClearAndAddListener(delegate
			{
				int gameDataListenerId = this.Parent.Element.GameDataListenerId;
				IAsyncMethodRequestHandler parent = this.Parent;
				int templateId = (int)data.TemplateId;
				Action<ItemKey> customCallback;
				if ((customCallback = <>9__1) == null)
				{
					customCallback = (<>9__1 = delegate(ItemKey v)
					{
						this.Parent.RequestData();
					});
				}
				ChickenCommonHelper.SelectItemAndFeedChicken(gameDataListenerId, parent, templateId, customCallback);
			});
			this.renamer.Refresh(data.Name, 6, true, data.Name);
		}

		// Token: 0x06009C81 RID: 40065 RVA: 0x00494BB4 File Offset: 0x00492DB4
		public void OnRenameClicked(Renamer renamer, string nonsense = "")
		{
			renamer.Input.text = this._chickenData.Name;
		}

		// Token: 0x06009C82 RID: 40066 RVA: 0x00494BCE File Offset: 0x00492DCE
		public void EditRecordName(Renamer renamer, string saveName)
		{
			this.RenameConfirm(saveName);
			this.Parent.RequestData();
		}

		// Token: 0x06009C83 RID: 40067 RVA: 0x00494BE8 File Offset: 0x00492DE8
		private void RenameConfirm(string nickname)
		{
			bool flag = nickname.IsNullOrEmpty();
			if (flag)
			{
				nickname = "";
			}
			BuildingDomainMethod.Call.SetNickNameByChickenId(this._chickenData.Id, nickname);
		}

		// Token: 0x04007945 RID: 31045
		[SerializeField]
		private Renamer renamer;

		// Token: 0x04007946 RID: 31046
		[SerializeField]
		private TextMeshProUGUI happiness;

		// Token: 0x04007947 RID: 31047
		[SerializeField]
		private CButton feedBtn;

		// Token: 0x04007948 RID: 31048
		[SerializeField]
		private TextMeshProUGUI personalityTitle;

		// Token: 0x04007949 RID: 31049
		[SerializeField]
		private TextMeshProUGUI personalityValue;

		// Token: 0x0400794A RID: 31050
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x0400794B RID: 31051
		[SerializeField]
		private CImage[] featureIcons;

		// Token: 0x0400794C RID: 31052
		[SerializeField]
		private TextMeshProUGUI featureName;

		// Token: 0x0400794D RID: 31053
		[SerializeField]
		private TextMeshProUGUI featureDesc;

		// Token: 0x0400794E RID: 31054
		[SerializeField]
		private CButton btnRename;

		// Token: 0x0400794F RID: 31055
		private static readonly string[][] FeatureIconConfig = new string[][]
		{
			new string[]
			{
				"0_2",
				"1_2",
				"3_2"
			},
			new string[]
			{
				"0_1",
				"1_1",
				"3_1"
			},
			new string[]
			{
				"0_0",
				"1_0",
				"3_0"
			},
			new string[]
			{
				"0_3",
				"1_3",
				"3_3"
			}
		};

		// Token: 0x04007950 RID: 31056
		private ChickenData _chickenData;
	}
}
