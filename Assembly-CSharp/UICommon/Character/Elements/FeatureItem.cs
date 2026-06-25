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
	// Token: 0x020005F4 RID: 1524
	public class FeatureItem
	{
		// Token: 0x060047D4 RID: 18388 RVA: 0x0021A98C File Offset: 0x00218B8C
		public FeatureItem(Refers refers, short templateId, int characterId = -1)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("refers can not be null to create FeatureItem robot!");
			}
			this._characterId = characterId;
			this._config = CharacterFeature.Instance[templateId];
			bool flag2 = this._config == null;
			if (flag2)
			{
				throw new Exception(string.Format("failed to get CharacterFeature Config Data for id:{0}!", templateId));
			}
			this.InitUIRefers(refers);
			this.RefreshView();
		}

		// Token: 0x060047D5 RID: 18389 RVA: 0x0021AA0C File Offset: 0x00218C0C
		public FeatureItem(Refers refers)
		{
			this.InitUIRefers(refers);
			this.ShowEmpty();
		}

		// Token: 0x060047D6 RID: 18390 RVA: 0x0021AA30 File Offset: 0x00218C30
		private void InitUIRefers(Refers refers)
		{
			this._nameLabel = refers.CGet<TextMeshProUGUI>("FeatureName");
			this._levelImage = refers.CGetList<CImage>("Level");
			bool flag = refers.Names.Contains("MouseTip");
			if (flag)
			{
				this._mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
			}
			this._cornerImage = refers.CGet<CImage>("FeatureCorner");
			this._normalTextList = refers.CGetList<GameObject>("NormalText");
			this._normalFeatureArea = refers.CGet<GameObject>("FeatureTypeNormal");
			this._specialFeatureArea = refers.CGet<GameObject>("FeatureTypeSpecial");
			this._specialText = refers.CGet<TextMeshProUGUI>("SpetialTypeText");
			this._temporaryFeatureArea = refers.CGet<GameObject>("FeatureTypeTemporary");
			this._temporaryFeatureLeftTime = refers.CGet<TextMeshProUGUI>("FeatureLeftTimeText");
			this._deleteButton = refers.CGet<CButtonObsolete>("DeleteButton");
			this._deleteButton.gameObject.SetActive(false);
			bool flag2 = refers.Names.Contains("MainContent");
			if (flag2)
			{
				this._emptyBack = refers.CGet<GameObject>("EmptyBack");
				this._mainContent = refers.CGet<GameObject>("MainContent");
			}
		}

		// Token: 0x060047D7 RID: 18391 RVA: 0x0021AB58 File Offset: 0x00218D58
		public void Refresh(short templateId, int characterId = -1)
		{
			this._config = CharacterFeature.Instance[templateId];
			bool flag = this._config == null;
			if (flag)
			{
				throw new Exception(string.Format("failed to get CharacterFeature Config Data for id:{0}!", templateId));
			}
			this._characterId = characterId;
			this.RefreshView();
		}

		// Token: 0x060047D8 RID: 18392 RVA: 0x0021ABA8 File Offset: 0x00218DA8
		private void RefreshView()
		{
			this.ShowMainContent();
			bool flag = GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress() && (this._config.TemplateId == 210 || this._config.TemplateId == 211);
			if (flag)
			{
				this._nameLabel.text = this._config.SmallVillageName;
			}
			else
			{
				this._nameLabel.text = this._config.Name;
			}
			this.RefreshCorner();
			this.RefreshLevelImages();
			this.RefreshMouseTip();
			this.RefreshWithFeatureType();
		}

		// Token: 0x060047D9 RID: 18393 RVA: 0x0021AC44 File Offset: 0x00218E44
		public void RefreshDeleteButton(bool isTaiwu)
		{
			bool canDelete = this._config.CanDeleteManually && isTaiwu;
			this._deleteButton.gameObject.SetActive(canDelete);
			bool flag = canDelete;
			if (flag)
			{
				this._deleteButton.ClearAndAddListener(new Action(this.OnClickDeleteButton));
			}
		}

		// Token: 0x060047DA RID: 18394 RVA: 0x0021AC94 File Offset: 0x00218E94
		public void ShowEmpty()
		{
			bool flag = this._mainContent != null;
			if (flag)
			{
				this._mainContent.SetActive(false);
				this._emptyBack.SetActive(true);
			}
		}

		// Token: 0x060047DB RID: 18395 RVA: 0x0021ACD0 File Offset: 0x00218ED0
		public void ShowMainContent()
		{
			bool flag = this._mainContent != null;
			if (flag)
			{
				this._mainContent.SetActive(true);
				this._emptyBack.SetActive(false);
			}
		}

		// Token: 0x060047DC RID: 18396 RVA: 0x0021AD0C File Offset: 0x00218F0C
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

		// Token: 0x060047DD RID: 18397 RVA: 0x0021ADBA File Offset: 0x00218FBA
		private void DeleteFeature()
		{
			TaiwuDomainMethod.Call.DeleteTaiwuFeature(this._config.TemplateId);
		}

		// Token: 0x060047DE RID: 18398 RVA: 0x0021ADCE File Offset: 0x00218FCE
		private void RefreshCorner()
		{
			this._cornerImage.SetSprite(this.GetCorner(), false, null);
		}

		// Token: 0x060047DF RID: 18399 RVA: 0x0021ADE8 File Offset: 0x00218FE8
		private string GetCorner()
		{
			bool flag = this._config.Type == ECharacterFeatureType.Bad || this._config.Type == ECharacterFeatureType.Good;
			string result;
			if (flag)
			{
				result = FeatureItem.NormalCornerConfig[this.GetNormalFeatureIndex()][(int)(Math.Abs(this._config.Level) - 1)];
			}
			else
			{
				bool flag2 = this._config.Type == ECharacterFeatureType.Special;
				if (flag2)
				{
					result = FeatureItem.SpecialCornerConfig[this.GetSpecialFeatureIndex()];
				}
				else
				{
					bool flag3 = this._config.Type == ECharacterFeatureType.Temporary;
					if (!flag3)
					{
						throw new Exception("Unknown feature type!");
					}
					result = "sp_01_gn_featurestextbase_05";
				}
			}
			return result;
		}

		// Token: 0x060047E0 RID: 18400 RVA: 0x0021AE88 File Offset: 0x00219088
		private void RefreshWithFeatureType()
		{
			bool flag = this._config.Type == ECharacterFeatureType.Bad || this._config.Type == ECharacterFeatureType.Good;
			if (flag)
			{
				this._normalFeatureArea.SetActive(true);
				this._specialFeatureArea.SetActive(false);
				this._temporaryFeatureArea.SetActive(false);
				this.RefreshNormalTextColor();
			}
			else
			{
				bool flag2 = this._config.Type == ECharacterFeatureType.Special;
				if (flag2)
				{
					this._normalFeatureArea.SetActive(false);
					this._specialFeatureArea.SetActive(true);
					this._temporaryFeatureArea.SetActive(false);
					this.RefreshSpecialText();
				}
				else
				{
					bool flag3 = this._config.Type == ECharacterFeatureType.Temporary;
					if (flag3)
					{
						this._normalFeatureArea.SetActive(false);
						this._specialFeatureArea.SetActive(false);
						this._temporaryFeatureArea.SetActive(true);
						this.RefreshTemporaryFeatureExpireDate();
					}
				}
			}
		}

		// Token: 0x060047E1 RID: 18401 RVA: 0x0021AF74 File Offset: 0x00219174
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
			this._specialText.text = LocalStringManager.Get(LanguageKey.LK_FeatureItem_Special).SetColor(color);
		}

		// Token: 0x060047E2 RID: 18402 RVA: 0x0021AFF4 File Offset: 0x002191F4
		private void RefreshNormalTextColor()
		{
			int targetIndex = this.GetNormalFeatureIndex();
			for (int i = 0; i < this._normalTextList.Count; i++)
			{
				this._normalTextList[i].SetActive(i == targetIndex);
			}
		}

		// Token: 0x060047E3 RID: 18403 RVA: 0x0021B03C File Offset: 0x0021923C
		private void RefreshMouseTip()
		{
			bool flag = this._mouseTip != null;
			if (flag)
			{
				TooltipInvoker mouseTip = this._mouseTip;
				if (mouseTip.RuntimeParam == null)
				{
					mouseTip.RuntimeParam = new ArgumentBox();
				}
				this._mouseTip.RuntimeParam.Set("FeatureId", this._config.TemplateId);
				this._mouseTip.RuntimeParam.Set("CharacterId", this._characterId);
			}
		}

		// Token: 0x060047E4 RID: 18404 RVA: 0x0021B0B8 File Offset: 0x002192B8
		private void RefreshLevelImages()
		{
			int indexMedal = 0;
			for (int i = 0; i < 3; i++)
			{
				FeatureMedals medals = this._config.FeatureMedals[i];
				for (int j = 0; j < medals.Values.Count; j++)
				{
					CImage medalImage = this._levelImage[indexMedal++];
					bool flag = null == medalImage;
					if (flag)
					{
						break;
					}
					medalImage.gameObject.SetActive(true);
					medalImage.SetSprite(FeatureItem.FeatureIconConfig[(int)medals.Values[j]][i], false, null);
				}
			}
			while (indexMedal < 3)
			{
				this._levelImage[indexMedal].gameObject.SetActive(false);
				indexMedal++;
			}
		}

		// Token: 0x060047E5 RID: 18405 RVA: 0x0021B188 File Offset: 0x00219388
		private void RefreshTemporaryFeatureExpireDate()
		{
			bool flag = this._config.Type != ECharacterFeatureType.Temporary;
			if (!flag)
			{
				bool flag2 = this._characterId < 0;
				if (flag2)
				{
					this._temporaryFeatureLeftTime.text = "-";
				}
				else
				{
					CharacterDomainMethod.AsyncCall.GetCharacterTemporaryFeaturesExpireDate(null, new IntPair(this._characterId, (int)this._config.TemplateId), delegate(int offset, RawDataPool dataPool)
					{
						int expireDate = -1;
						Serializer.Deserialize(dataPool, offset, ref expireDate);
						bool flag3 = expireDate > -1;
						if (flag3)
						{
							this._temporaryFeatureLeftTime.text = (expireDate - SingletonObject.getInstance<BasicGameData>().CurrDate).ToString();
						}
						else
						{
							this._temporaryFeatureLeftTime.text = "-";
						}
					});
				}
			}
		}

		// Token: 0x060047E6 RID: 18406 RVA: 0x0021B1F8 File Offset: 0x002193F8
		private int GetNormalFeatureIndex()
		{
			bool flag = this._config.Type == ECharacterFeatureType.Bad;
			int result;
			if (flag)
			{
				result = 2;
			}
			else
			{
				bool flag2 = this._config.Type == ECharacterFeatureType.Good;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060047E7 RID: 18407 RVA: 0x0021B238 File Offset: 0x00219438
		private int GetSpecialFeatureIndex()
		{
			bool flag = this._config.InfectedType == ECharacterFeatureInfectedType.LegendaryBook;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				bool flag2 = this._config.InfectedType == ECharacterFeatureInfectedType.PartlyInfected;
				if (flag2)
				{
					result = 2;
				}
				else
				{
					bool flag3 = this._config.InfectedType == ECharacterFeatureInfectedType.CompletelyInfected;
					if (flag3)
					{
						result = 2;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0400319B RID: 12699
		private static readonly string[][] FeatureIconConfig = new string[][]
		{
			new string[]
			{
				"sp_icon_renwutexing_10",
				"sp_icon_renwutexing_9",
				"sp_icon_renwutexing_11"
			},
			new string[]
			{
				"sp_icon_renwutexing_4",
				"sp_icon_renwutexing_3",
				"sp_icon_renwutexing_5"
			},
			new string[]
			{
				"sp_icon_renwutexing_1",
				"sp_icon_renwutexing_0",
				"sp_icon_renwutexing_2"
			},
			new string[]
			{
				"sp_icon_renwutexing_7",
				"sp_icon_renwutexing_6",
				"sp_icon_renwutexing_8"
			}
		};

		// Token: 0x0400319C RID: 12700
		private const string TemporaryFeatureBack = "sp_01_gn_featurestextbase_05";

		// Token: 0x0400319D RID: 12701
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

		// Token: 0x0400319E RID: 12702
		private static readonly string[] SpecialCornerConfig = new string[]
		{
			"sp_01_gn_featurestextbase_04_01",
			"sp_01_gn_featurestextbase_04_02",
			"sp_01_gn_featurestextbase_04_03"
		};

		// Token: 0x0400319F RID: 12703
		private CharacterFeatureItem _config;

		// Token: 0x040031A0 RID: 12704
		private int _characterId;

		// Token: 0x040031A1 RID: 12705
		private TextMeshProUGUI _nameLabel;

		// Token: 0x040031A2 RID: 12706
		private List<CImage> _levelImage;

		// Token: 0x040031A3 RID: 12707
		private CImage _cornerImage;

		// Token: 0x040031A4 RID: 12708
		private TooltipInvoker _mouseTip;

		// Token: 0x040031A5 RID: 12709
		private List<GameObject> _normalTextList;

		// Token: 0x040031A6 RID: 12710
		private GameObject _normalFeatureArea;

		// Token: 0x040031A7 RID: 12711
		private GameObject _specialFeatureArea;

		// Token: 0x040031A8 RID: 12712
		private TextMeshProUGUI _specialText;

		// Token: 0x040031A9 RID: 12713
		private GameObject _temporaryFeatureArea;

		// Token: 0x040031AA RID: 12714
		private TextMeshProUGUI _temporaryFeatureLeftTime;

		// Token: 0x040031AB RID: 12715
		private CButtonObsolete _deleteButton;

		// Token: 0x040031AC RID: 12716
		private GameObject _emptyBack;

		// Token: 0x040031AD RID: 12717
		private GameObject _mainContent;

		// Token: 0x040031AE RID: 12718
		private readonly DialogCmd _deleteDialogCmd = new DialogCmd();
	}
}
