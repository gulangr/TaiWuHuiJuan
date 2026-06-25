using System;
using System.Collections.Generic;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F2 RID: 1522
	public class CharacterFeatureView : Refers
	{
		// Token: 0x060047BE RID: 18366 RVA: 0x0021A078 File Offset: 0x00218278
		public void Set(CharacterFeatureItem config, int characterId = -1, bool isTaiwu = false)
		{
			this._config = config;
			this._characterId = characterId;
			this.ShowMainContent();
			bool flag = GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress() && (this._config.TemplateId == 210 || this._config.TemplateId == 211);
			if (flag)
			{
				this.nameLabel.text = this._config.SmallVillageName;
			}
			else
			{
				this.nameLabel.text = this._config.Name;
			}
			this.RefreshCorner();
			this.RefreshLevelImages();
			this.RefreshMouseTip();
			this.RefreshWithFeatureType();
			this.RefreshDeleteButton(isTaiwu);
		}

		// Token: 0x060047BF RID: 18367 RVA: 0x0021A128 File Offset: 0x00218328
		public void ShowEmpty()
		{
			bool flag = !this.mainContent;
			if (!flag)
			{
				this.mainContent.SetActive(false);
				this.emptyBack.SetActive(true);
			}
		}

		// Token: 0x060047C0 RID: 18368 RVA: 0x0021A164 File Offset: 0x00218364
		public void ShowMainContent()
		{
			bool flag = !this.mainContent;
			if (!flag)
			{
				this.mainContent.SetActive(true);
				this.emptyBack.SetActive(false);
			}
		}

		// Token: 0x060047C1 RID: 18369 RVA: 0x0021A1A0 File Offset: 0x002183A0
		private void RefreshLevelImages()
		{
			int indexMedal = 0;
			for (int i = 0; i < 3; i++)
			{
				FeatureMedals medals = this._config.FeatureMedals[i];
				foreach (sbyte medalType in medals.Values)
				{
					CImage medalImage = this.levelImages[indexMedal++];
					bool flag = !medalImage;
					if (flag)
					{
						break;
					}
					medalImage.gameObject.SetActive(true);
					medalImage.SetSprite(CharacterFeatureView.FeatureIconConfig[(int)medalType][i], false, null);
				}
			}
			while (indexMedal < 3)
			{
				this.levelImages[indexMedal].gameObject.SetActive(false);
				indexMedal++;
			}
		}

		// Token: 0x060047C2 RID: 18370 RVA: 0x0021A28C File Offset: 0x0021848C
		private void RefreshCorner()
		{
			this.cornerImage.SetSprite(this.GetCorner(), false, null);
		}

		// Token: 0x060047C3 RID: 18371 RVA: 0x0021A2A4 File Offset: 0x002184A4
		private string GetCorner()
		{
			string result;
			switch (this._config.Type)
			{
			case ECharacterFeatureType.Special:
				result = CharacterFeatureView.SpecialCornerConfig[this.GetSpecialFeatureIndex()];
				break;
			case ECharacterFeatureType.Good:
			case ECharacterFeatureType.Bad:
				result = CharacterFeatureView.NormalCornerConfig[this.GetNormalFeatureIndex()][(int)(Math.Abs(this._config.Level) - 1)];
				break;
			case ECharacterFeatureType.Temporary:
				result = "sp_01_gn_featurestextbase_05";
				break;
			default:
				throw new Exception("Unknown feature type!");
			}
			return result;
		}

		// Token: 0x060047C4 RID: 18372 RVA: 0x0021A320 File Offset: 0x00218520
		private void RefreshWithFeatureType()
		{
			switch (this._config.Type)
			{
			case ECharacterFeatureType.Special:
				this.normalFeatureArea.SetActive(false);
				this.specialFeatureArea.SetActive(true);
				this.temporaryFeatureArea.SetActive(false);
				this.RefreshSpecialText();
				break;
			case ECharacterFeatureType.Good:
			case ECharacterFeatureType.Bad:
				this.normalFeatureArea.SetActive(true);
				this.specialFeatureArea.SetActive(false);
				this.temporaryFeatureArea.SetActive(false);
				this.RefreshNormalTextColor();
				break;
			case ECharacterFeatureType.Temporary:
				this.normalFeatureArea.SetActive(false);
				this.specialFeatureArea.SetActive(false);
				this.temporaryFeatureArea.SetActive(true);
				this.RefreshTemporaryFeatureExpireDate();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060047C5 RID: 18373 RVA: 0x0021A3F0 File Offset: 0x002185F0
		private void RefreshTemporaryFeatureExpireDate()
		{
			bool flag = this._config.Type != ECharacterFeatureType.Temporary;
			if (!flag)
			{
				bool flag2 = !this.temporaryFeatureLeftTime;
				if (!flag2)
				{
					bool flag3 = this._characterId < 0;
					if (flag3)
					{
						this.temporaryFeatureLeftTime.text = this._config.Duration.ToString();
					}
					else
					{
						CharacterDomainMethod.AsyncCall.GetCharacterTemporaryFeaturesExpireDate(null, new IntPair(this._characterId, (int)this._config.TemplateId), delegate(int offset, RawDataPool dataPool)
						{
							int expireDate = -1;
							Serializer.Deserialize(dataPool, offset, ref expireDate);
							this.temporaryFeatureLeftTime.text = ((expireDate > -1) ? (expireDate - SingletonObject.getInstance<BasicGameData>().CurrDate).ToString() : this._config.Duration.ToString());
						});
					}
				}
			}
		}

		// Token: 0x060047C6 RID: 18374 RVA: 0x0021A480 File Offset: 0x00218680
		private void RefreshSpecialText()
		{
			int specialFeatureIndex = this.GetSpecialFeatureIndex();
			if (!true)
			{
			}
			Color color2;
			if (specialFeatureIndex != 1)
			{
				if (specialFeatureIndex != 2)
				{
					color2 = Colors.Instance["white"];
				}
				else
				{
					color2 = Colors.Instance.GradeColors[5];
				}
			}
			else
			{
				color2 = Colors.Instance.GradeColors[6];
			}
			if (!true)
			{
			}
			Color color = color2;
			this.specialText.text = LocalStringManager.Get(LanguageKey.LK_FeatureItem_Special).SetColor(color);
		}

		// Token: 0x060047C7 RID: 18375 RVA: 0x0021A500 File Offset: 0x00218700
		private void RefreshNormalTextColor()
		{
			int targetIndex = this.GetNormalFeatureIndex();
			for (int i = 0; i < this.normalTextList.Count; i++)
			{
				this.normalTextList[i].SetActive(i == targetIndex);
			}
		}

		// Token: 0x060047C8 RID: 18376 RVA: 0x0021A548 File Offset: 0x00218748
		private int GetNormalFeatureIndex()
		{
			ECharacterFeatureType type = this._config.Type;
			if (!true)
			{
			}
			int result;
			if (type != ECharacterFeatureType.Good)
			{
				if (type != ECharacterFeatureType.Bad)
				{
					result = 0;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = 1;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060047C9 RID: 18377 RVA: 0x0021A588 File Offset: 0x00218788
		private int GetSpecialFeatureIndex()
		{
			ECharacterFeatureInfectedType infectedType = this._config.InfectedType;
			if (!true)
			{
			}
			int result;
			if (infectedType - ECharacterFeatureInfectedType.PartlyInfected > 1)
			{
				if (infectedType != ECharacterFeatureInfectedType.LegendaryBook)
				{
					result = 0;
				}
				else
				{
					result = 1;
				}
			}
			else
			{
				result = 2;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060047CA RID: 18378 RVA: 0x0021A5CC File Offset: 0x002187CC
		private void RefreshMouseTip()
		{
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.RuntimeParam.Set("FeatureId", this._config.TemplateId);
		}

		// Token: 0x060047CB RID: 18379 RVA: 0x0021A618 File Offset: 0x00218818
		public void RefreshDeleteButton(bool isTaiwu)
		{
			bool canDelete = this._config.CanDeleteManually && isTaiwu;
			this.deleteButton.gameObject.SetActive(canDelete);
			bool flag = canDelete;
			if (flag)
			{
				this.deleteButton.ClearAndAddListener(new Action(this.OnClickDeleteButton));
			}
		}

		// Token: 0x060047CC RID: 18380 RVA: 0x0021A668 File Offset: 0x00218868
		private void OnClickDeleteButton()
		{
			this._deleteDialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_ManuallyDeleteFeature_Dialog_Title);
			this._deleteDialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_ManuallyDeleteFeature_Dialog_Content).ColorReplace();
			this._deleteDialogCmd.Yes = new Action(this.DeleteFeature);
			this._deleteDialogCmd.No = delegate()
			{
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._deleteDialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060047CD RID: 18381 RVA: 0x0021A716 File Offset: 0x00218916
		private void DeleteFeature()
		{
			TaiwuDomainMethod.Call.DeleteTaiwuFeature(this._config.TemplateId);
		}

		// Token: 0x04003184 RID: 12676
		[SerializeField]
		private List<CImage> levelImages;

		// Token: 0x04003185 RID: 12677
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04003186 RID: 12678
		[SerializeField]
		private GameObject temporaryFeatureArea;

		// Token: 0x04003187 RID: 12679
		[SerializeField]
		private TextMeshProUGUI temporaryFeatureLeftTime;

		// Token: 0x04003188 RID: 12680
		[SerializeField]
		private CButtonObsolete deleteButton;

		// Token: 0x04003189 RID: 12681
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x0400318A RID: 12682
		[SerializeField]
		private CImage cornerImage;

		// Token: 0x0400318B RID: 12683
		[SerializeField]
		private List<GameObject> normalTextList;

		// Token: 0x0400318C RID: 12684
		[SerializeField]
		private GameObject normalFeatureArea;

		// Token: 0x0400318D RID: 12685
		[SerializeField]
		private GameObject specialFeatureArea;

		// Token: 0x0400318E RID: 12686
		[SerializeField]
		private TextMeshProUGUI specialText;

		// Token: 0x0400318F RID: 12687
		[SerializeField]
		private GameObject emptyBack;

		// Token: 0x04003190 RID: 12688
		[SerializeField]
		private GameObject mainContent;

		// Token: 0x04003191 RID: 12689
		private CharacterFeatureItem _config;

		// Token: 0x04003192 RID: 12690
		private int _characterId;

		// Token: 0x04003193 RID: 12691
		private static readonly string[][] FeatureIconConfig = new string[][]
		{
			new string[]
			{
				"ui_sp_icon_characteristic_10",
				"ui_sp_icon_characteristic_9",
				"ui_sp_icon_characteristic_11"
			},
			new string[]
			{
				"ui_sp_icon_characteristic_4",
				"ui_sp_icon_characteristic_3",
				"ui_sp_icon_characteristic_5"
			},
			new string[]
			{
				"ui_sp_icon_characteristic_1",
				"ui_sp_icon_characteristic_0",
				"ui_sp_icon_characteristic_2"
			},
			new string[]
			{
				"ui_sp_icon_characteristic_7",
				"ui_sp_icon_characteristic_6",
				"ui_sp_icon_characteristic_8"
			}
		};

		// Token: 0x04003194 RID: 12692
		private const string TemporaryFeatureBack = "sp_01_gn_featurestextbase_05";

		// Token: 0x04003195 RID: 12693
		private static readonly string[][] NormalCornerConfig = new string[][]
		{
			new string[]
			{
				"sp_01_gn_featurestextbase_01_01",
				"sp_01_gn_featurestextbase_01_02",
				"sp_01_gn_featurestextbase_01_03"
			},
			new string[]
			{
				"sp_01_gn_featurestextbase_03_01",
				"sp_01_gn_featurestextbase_03_02",
				"sp_01_gn_featurestextbase_03_03"
			},
			new string[]
			{
				"sp_01_gn_featurestextbase_02_01",
				"sp_01_gn_featurestextbase_02_02",
				"sp_01_gn_featurestextbase_02_03"
			}
		};

		// Token: 0x04003196 RID: 12694
		private static readonly string[] SpecialCornerConfig = new string[]
		{
			"sp_01_gn_featurestextbase_04_01",
			"sp_01_gn_featurestextbase_04_02",
			"sp_01_gn_featurestextbase_04_03"
		};

		// Token: 0x04003197 RID: 12695
		private readonly DialogCmd _deleteDialogCmd = new DialogCmd();
	}
}
