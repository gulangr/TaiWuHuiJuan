using System;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F24 RID: 3876
	public class Feature : MonoBehaviour
	{
		// Token: 0x0600B268 RID: 45672 RVA: 0x0051317C File Offset: 0x0051137C
		public void Set(short featureId, int characterId = -1, bool isTaiwu = false, int leftTime = -1)
		{
			this._config = CharacterFeature.Instance[featureId];
			this._characterId = characterId;
			this._isTaiwu = isTaiwu;
			this._leftTime = leftTime;
			bool isInGame = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			bool flag = isInGame && GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress() && (this._config.TemplateId == 210 || this._config.TemplateId == 211);
			if (flag)
			{
				this.nameLabel.text = this._config.SmallVillageName;
			}
			else
			{
				this.nameLabel.text = this._config.Name;
			}
			base.gameObject.SetActive(true);
			this.RefreshCorner();
			this.RefreshLevelImages();
			this.RefreshMouseTip();
			this.RefreshWithFeatureType();
			this.RefreshDeleteButton();
		}

		// Token: 0x0600B269 RID: 45673 RVA: 0x0051325B File Offset: 0x0051145B
		public void SetTipEnabled(bool enabled)
		{
			this.mouseTip.enabled = enabled;
		}

		// Token: 0x0600B26A RID: 45674 RVA: 0x0051326C File Offset: 0x0051146C
		public void SetTipTemplateDataOnly(bool templateDataOnly)
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("TemplateDataOnly", templateDataOnly);
				if (templateDataOnly)
				{
					this.mouseTip.RuntimeParam.Set("CharacterId", -1);
				}
			}
		}

		// Token: 0x0600B26B RID: 45675 RVA: 0x005132DC File Offset: 0x005114DC
		public static string GetMedalImageName(sbyte medalType, int index)
		{
			return "ui9_icon_strategy_big_" + Feature.FeatureIconConfig[(int)medalType][index];
		}

		// Token: 0x0600B26C RID: 45676 RVA: 0x005132F4 File Offset: 0x005114F4
		private void RefreshLevelImages()
		{
			bool flag = this.levelImages == null || this._config == null;
			if (!flag)
			{
				int indexMedal = 0;
				for (int i = 0; i < 3; i++)
				{
					FeatureMedals medals = this._config.FeatureMedals[i];
					foreach (sbyte medalType in medals.Values)
					{
						bool flag2 = indexMedal >= this.levelImages.Length;
						if (flag2)
						{
							break;
						}
						CImage medalImage = this.levelImages[indexMedal++];
						bool flag3 = medalImage == null;
						if (flag3)
						{
							break;
						}
						medalImage.gameObject.SetActive(true);
						string imageName = Feature.GetMedalImageName(medalType, i);
						medalImage.SetSprite(imageName, false, null);
					}
				}
				while (indexMedal < this.levelImages.Length)
				{
					bool flag4 = this.levelImages[indexMedal] != null;
					if (flag4)
					{
						this.levelImages[indexMedal].gameObject.SetActive(false);
					}
					indexMedal++;
				}
			}
		}

		// Token: 0x0600B26D RID: 45677 RVA: 0x00513428 File Offset: 0x00511628
		private void RefreshCorner()
		{
			bool flag = this.cornerImage == null || this._config == null;
			if (!flag)
			{
				string cornerSpriteName = this.GetCorner();
				this.cornerImage.SetSprite("ui9_icon_feature_" + cornerSpriteName, false, null);
			}
		}

		// Token: 0x0600B26E RID: 45678 RVA: 0x00513478 File Offset: 0x00511678
		private string GetCorner()
		{
			bool flag = this._config == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				switch (this._config.Type)
				{
				case ECharacterFeatureType.Special:
					result = "4_1";
					break;
				case ECharacterFeatureType.Good:
				case ECharacterFeatureType.Bad:
					result = Feature.NormalCornerConfig[this.GetNormalFeatureIndex()][(int)(Math.Abs(this._config.Level) - 1)];
					break;
				case ECharacterFeatureType.Temporary:
					result = "3_1";
					break;
				default:
					result = "";
					break;
				}
			}
			return result;
		}

		// Token: 0x0600B26F RID: 45679 RVA: 0x005134FC File Offset: 0x005116FC
		private void RefreshWithFeatureType()
		{
			bool flag = this._config == null;
			if (!flag)
			{
				bool isTemp = this._config.Type == ECharacterFeatureType.Temporary;
				this.temporaryFeatureArea.SetActive(isTemp);
				bool flag2 = isTemp;
				if (flag2)
				{
					this.RefreshTemporaryFeatureExpireDate();
				}
			}
		}

		// Token: 0x0600B270 RID: 45680 RVA: 0x00513544 File Offset: 0x00511744
		private void RefreshTemporaryFeatureExpireDate()
		{
			CharacterFeatureItem config = this._config;
			bool flag = config == null || config.Type != ECharacterFeatureType.Temporary || this.temporaryFeatureLeftTime == null;
			if (!flag)
			{
				bool flag2 = this._config.TemplateId == 216 && this._leftTime == 0;
				if (flag2)
				{
					this.temporaryFeatureLeftTime.text = "-";
				}
				else
				{
					int displayTime = (this._leftTime >= 0) ? this._leftTime : ((int)this._config.Duration);
					this.temporaryFeatureLeftTime.text = displayTime.ToString();
				}
			}
		}

		// Token: 0x0600B271 RID: 45681 RVA: 0x005135E8 File Offset: 0x005117E8
		private int GetNormalFeatureIndex()
		{
			bool flag = this._config == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				ECharacterFeatureType type = this._config.Type;
				if (!true)
				{
				}
				int num;
				if (type != ECharacterFeatureType.Good)
				{
					if (type != ECharacterFeatureType.Bad)
					{
						num = 0;
					}
					else
					{
						num = 2;
					}
				}
				else
				{
					num = 1;
				}
				if (!true)
				{
				}
				result = num;
			}
			return result;
		}

		// Token: 0x0600B272 RID: 45682 RVA: 0x00513638 File Offset: 0x00511838
		private int GetSpecialFeatureIndex()
		{
			bool flag = this._config == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				ECharacterFeatureInfectedType infectedType = this._config.InfectedType;
				if (!true)
				{
				}
				int num;
				if (infectedType - ECharacterFeatureInfectedType.PartlyInfected > 1)
				{
					if (infectedType != ECharacterFeatureInfectedType.LegendaryBook)
					{
						num = 0;
					}
					else
					{
						num = 1;
					}
				}
				else
				{
					num = 2;
				}
				if (!true)
				{
				}
				result = num;
			}
			return result;
		}

		// Token: 0x0600B273 RID: 45683 RVA: 0x0051368C File Offset: 0x0051188C
		private void RefreshMouseTip()
		{
			bool flag = this.mouseTip == null || this._config == null;
			if (!flag)
			{
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("FeatureId", this._config.TemplateId);
				this.mouseTip.RuntimeParam.Set("CharacterId", this._characterId);
			}
		}

		// Token: 0x0600B274 RID: 45684 RVA: 0x00513714 File Offset: 0x00511914
		private void RefreshDeleteButton()
		{
			bool flag = this.deleteButton == null || this._config == null;
			if (!flag)
			{
				bool canDelete = this._config.CanDeleteManually && this._isTaiwu;
				this.deleteButton.gameObject.SetActive(canDelete);
				bool flag2 = canDelete;
				if (flag2)
				{
					this.deleteButton.ClearAndAddListener(new Action(this.OnClickDeleteButton));
				}
			}
		}

		// Token: 0x0600B275 RID: 45685 RVA: 0x0051378C File Offset: 0x0051198C
		private void OnClickDeleteButton()
		{
			bool flag = this._config == null;
			if (!flag)
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
		}

		// Token: 0x0600B276 RID: 45686 RVA: 0x0051384C File Offset: 0x00511A4C
		private void DeleteFeature()
		{
			TaiwuDomainMethod.Call.DeleteTaiwuFeature(this._config.TemplateId);
			GEvent.OnEvent(UiEvents.OnTaiwuFeatureDeleted, null);
		}

		// Token: 0x04008A68 RID: 35432
		[SerializeField]
		private CImage[] levelImages;

		// Token: 0x04008A69 RID: 35433
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04008A6A RID: 35434
		[SerializeField]
		private GameObject temporaryFeatureArea;

		// Token: 0x04008A6B RID: 35435
		[SerializeField]
		private TextMeshProUGUI temporaryFeatureLeftTime;

		// Token: 0x04008A6C RID: 35436
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x04008A6D RID: 35437
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008A6E RID: 35438
		[SerializeField]
		private CImage cornerImage;

		// Token: 0x04008A6F RID: 35439
		private CharacterFeatureItem _config;

		// Token: 0x04008A70 RID: 35440
		private int _characterId;

		// Token: 0x04008A71 RID: 35441
		private bool _isTaiwu;

		// Token: 0x04008A72 RID: 35442
		private int _leftTime = -1;

		// Token: 0x04008A73 RID: 35443
		private readonly DialogCmd _deleteDialogCmd = new DialogCmd();

		// Token: 0x04008A74 RID: 35444
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

		// Token: 0x04008A75 RID: 35445
		private const string TemporaryCorner = "3_1";

		// Token: 0x04008A76 RID: 35446
		private const string SpecialCorner = "4_1";

		// Token: 0x04008A77 RID: 35447
		private static readonly string[][] NormalCornerConfig = new string[][]
		{
			new string[]
			{
				"2_1",
				"2_2",
				"2_3"
			},
			new string[]
			{
				"0_1",
				"0_2",
				"0_3"
			},
			new string[]
			{
				"1_1",
				"1_2",
				"1_3"
			}
		};
	}
}
