using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using GameData.Domains.Character.Creation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x0200081A RID: 2074
	public class ViewChallenge : UIBase
	{
		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x060065AF RID: 26031 RVA: 0x002E70E4 File Offset: 0x002E52E4
		private bool IsOpenChallenge
		{
			get
			{
				return this.switchToggle.isOn;
			}
		}

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x060065B0 RID: 26032 RVA: 0x002E70F1 File Offset: 0x002E52F1
		private int CurTotalPoint
		{
			get
			{
				return this._enabledChallengeSet.Sum((int c) => ChallengeMode.Instance[c].Point);
			}
		}

		// Token: 0x060065B1 RID: 26033 RVA: 0x002E7120 File Offset: 0x002E5320
		public override void OnInit(ArgumentBox argsBox)
		{
			this.textOptionalTitle.text = LanguageKey.LK_NewGame_Challenge_Optional.TrFormat(3);
			this.warningTips.SetActive(!this.switchToggle.isOn);
			string customTips = this.switchToggle.isOn ? LanguageKey.LK_NewGame_Challenge_Open.Tr() : LanguageKey.LK_NewGame_Challenge_NotOpen.Tr();
			this.customTipsText.SetText(customTips.SetColor(this.switchToggle.isOn ? "pinkyellow" : "lowwarning").ColorReplace(), true);
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x002E71BC File Offset: 0x002E53BC
		private void OnEnable()
		{
			List<ChallengeModeItem> essentialConfigList = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Required
			select c).ToList<ChallengeModeItem>();
			for (int i = 0; i < essentialConfigList.Count; i++)
			{
				ChallengeModeItem config = essentialConfigList[i];
				this.essentialItemList[i].Init(config.TemplateId, false, this.IsItemEnable(config.TemplateId), new Action<NewGameSubPageChallengeItem, bool>(this.OnChallengeItemChange));
			}
			List<ChallengeModeItem> optionalConfigList = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Optional
			select c).ToList<ChallengeModeItem>();
			for (int j = 0; j < optionalConfigList.Count; j++)
			{
				ChallengeModeItem config2 = optionalConfigList[j];
				this.optionalItemList[j].Init(config2.TemplateId, false, this.IsItemEnable(config2.TemplateId), new Action<NewGameSubPageChallengeItem, bool>(this.OnChallengeItemChange));
			}
			List<ChallengeModeItem> bonusConfigList = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Bonus
			select c).ToList<ChallengeModeItem>();
			for (int k = 0; k < bonusConfigList.Count; k++)
			{
				ChallengeModeItem config3 = bonusConfigList[k];
				this.bonusItemList[k].Init(config3.TemplateId, false, this.IsItemEnable(config3.TemplateId), new Action<NewGameSubPageChallengeItem, bool>(this.OnChallengeItemChange));
				bool flag = config3.TemplateId == this._featureChallengeItemTemplateId;
				if (flag)
				{
					this._featureItem = this.bonusItemList[k];
					this.ShowFeature(this.IsItemEnable(config3.TemplateId) && this._reincarnationBonusFeatureId >= 0);
				}
			}
			this.RefreshCountAndPoint();
			this.RefreshChallengeItemInteractable();
			this.RefreshButtonConfirm();
			this.RefreshRootOff();
			this.RefreshRootEffectOn();
		}

		// Token: 0x060065B3 RID: 26035 RVA: 0x002E73DC File Offset: 0x002E55DC
		private void Awake()
		{
			this.switchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SwitchToggleOnValueChanged));
			this.buttonClose.ClearAndAddListener(new Action(this.QuickHide));
			this.buttonChangeFeature.ClearAndAddListener(new Action(this.OpenSelectFeature));
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnClickButtonConfirm));
		}

		// Token: 0x060065B4 RID: 26036 RVA: 0x002E7450 File Offset: 0x002E5650
		private bool IsItemEnable(int templateId)
		{
			return this._enabledChallengeSet.Contains(templateId);
		}

		// Token: 0x060065B5 RID: 26037 RVA: 0x002E7460 File Offset: 0x002E5660
		private void SwitchToggleOnValueChanged(bool isOn)
		{
			if (isOn)
			{
				foreach (NewGameSubPageChallengeItem item in this.essentialItemList)
				{
					this._enabledChallengeSet.Add(item.Config.TemplateId);
				}
				foreach (NewGameSubPageChallengeItem item2 in this.optionalItemList)
				{
					bool isOn2 = item2.SwitchToggle.isOn;
					if (isOn2)
					{
						this._enabledChallengeSet.Add(item2.Config.TemplateId);
					}
				}
			}
			this.warningTips.SetActive(!isOn);
			string customTips = isOn ? LanguageKey.LK_NewGame_Challenge_Open.Tr() : LanguageKey.LK_NewGame_Challenge_NotOpen.Tr();
			this.customTipsText.SetText(customTips.SetColor(isOn ? "pinkyellow" : "lowwarning").ColorReplace(), true);
			this.RefreshCountAndPoint();
			this.RefreshChallengeItemInteractable();
			this.RefreshButtonConfirm();
			this.RefreshRootOff();
			this.RefreshRootEffectOn();
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x002E75B0 File Offset: 0x002E57B0
		private void RefreshRootOff()
		{
			this.rootOff.SetActive(!this.switchToggle.isOn);
			this.toggleEffectOn.SetActive(!this.switchToggle.isOn);
		}

		// Token: 0x060065B7 RID: 26039 RVA: 0x002E75E7 File Offset: 0x002E57E7
		private void RefreshRootEffectOn()
		{
			this.rootEffectOn.SetActive(this.switchToggle.isOn);
		}

		// Token: 0x060065B8 RID: 26040 RVA: 0x002E7600 File Offset: 0x002E5800
		private void OnChallengeItemChange(NewGameSubPageChallengeItem item, bool isOn)
		{
			int templateId = item.Config.TemplateId;
			if (isOn)
			{
				this._enabledChallengeSet.Add(templateId);
			}
			else
			{
				this._enabledChallengeSet.Remove(templateId);
			}
			this.RefreshCountAndPoint();
			this.RefreshChallengeItemInteractable();
			this.RefreshButtonConfirm();
			bool flag = templateId == this._featureChallengeItemTemplateId;
			if (flag)
			{
				if (isOn)
				{
					this.OpenSelectFeature();
				}
				else
				{
					this.ShowFeature(false);
				}
			}
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x002E767C File Offset: 0x002E587C
		private void RefreshChallengeItemInteractable()
		{
			foreach (NewGameSubPageChallengeItem item in this.optionalItemList)
			{
				item.SwitchToggle.interactable = this.IsOpenChallenge;
			}
			int totalPoint = this.CurTotalPoint;
			foreach (NewGameSubPageChallengeItem item2 in this.bonusItemList)
			{
				bool isSelected = this._enabledChallengeSet.Contains(item2.Config.TemplateId);
				bool allowSelect = !isSelected && totalPoint + item2.Config.Point >= 0;
				bool interactable = isSelected || allowSelect;
				item2.SwitchToggle.interactable = (this.IsOpenChallenge && interactable);
			}
		}

		// Token: 0x060065BA RID: 26042 RVA: 0x002E7778 File Offset: 0x002E5978
		private void RefreshCountAndPoint()
		{
			this.textCount.text = LanguageKey.LK_NewGame_Challenge_Count.TrFormat(this._enabledChallengeSet.Count);
			string curTotalPointStr = this.CurTotalPoint.ToString().SetColor("specialyellow");
			string maxTotalPointStr = (from c in ChallengeMode.Instance
			where c.Point > 0
			select c).Sum((ChallengeModeItem c) => c.Point).ToString().SetColor("brightyellow");
			this.textTotalProtagonistPoint.text = (curTotalPointStr.SetColor("brightyellow") + "/" + maxTotalPointStr).ColorReplace();
			int essentialPoint = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Required
			select c).Sum((ChallengeModeItem c) => c.Point);
			this.textEssentialProtagonistPoint.text = essentialPoint.ToString().SetColor("specialyellow");
			string curOptionalPointStr = (from e in this._enabledChallengeSet
			select ChallengeMode.Instance[e] into c
			where c.Type == EChallengeModeType.Optional
			select c).Sum((ChallengeModeItem c) => c.Point).ToString().SetColor("specialyellow");
			string maxOptionalPointStr = (from c in ChallengeMode.Instance
			where c.Type == EChallengeModeType.Optional
			select c).Sum((ChallengeModeItem c) => c.Point).ToString().SetColor("brightyellow");
			this.textOptionalProtagonistPoint.text = curOptionalPointStr + "/" + maxOptionalPointStr;
			this.SetInfernoBg(this.GetEnabledOptionalCount());
		}

		// Token: 0x060065BB RID: 26043 RVA: 0x002E79C8 File Offset: 0x002E5BC8
		private int GetEnabledOptionalCount()
		{
			bool flag = !this.IsOpenChallenge;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = this._enabledChallengeSet.Count((int id) => ChallengeMode.Instance[id].Type == EChallengeModeType.Optional);
			}
			return result;
		}

		// Token: 0x060065BC RID: 26044 RVA: 0x002E7A18 File Offset: 0x002E5C18
		private void RefreshButtonConfirm()
		{
			int optionalCount = this._enabledChallengeSet.Count((int c) => ChallengeMode.Instance[c].Type == EChallengeModeType.Optional);
			bool isOptionalCountMeet = optionalCount >= 3;
			bool isPointMeet = this.CurTotalPoint >= 0;
			this.buttonConfirm.interactable = (!this.IsOpenChallenge || (isOptionalCountMeet && isPointMeet));
			this.tipButtonConfirm.enabled = !this.buttonConfirm.interactable;
			bool enabled = this.tipButtonConfirm.enabled;
			if (enabled)
			{
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				bool flag = !isOptionalCountMeet;
				if (flag)
				{
					stringBuilder.AppendLine(LanguageKey.LK_NewGame_Challenge_SelectNotEnough.TrFormat(3).SetColor("brightred"));
				}
				bool flag2 = !isPointMeet;
				if (flag2)
				{
					stringBuilder.AppendLine(LanguageKey.LK_NewGame_Challenge_PointTip.Tr().SetColor("brightred"));
				}
				this.tipButtonConfirm.PresetParam = new string[]
				{
					stringBuilder.ToString()
				};
			}
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x002E7B20 File Offset: 0x002E5D20
		private void OnClickButtonConfirm()
		{
			ViewNewGame.ChallengeModeIds.Clear();
			bool isOpenChallenge = this.IsOpenChallenge;
			if (isOpenChallenge)
			{
				ViewNewGame.ChallengeModeIds.AddRange(this._enabledChallengeSet);
			}
			ViewNewGame.ChallengeModeInfo = new ChallengeModeInfo
			{
				ReincarnationBonusFeatureId = this._reincarnationBonusFeatureId
			};
			this.QuickHide();
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x002E7B74 File Offset: 0x002E5D74
		private void ShowFeature(bool isShow)
		{
			this.rootFeature.SetActive(isShow);
			this.challengeOff.SetActive(!isShow);
			bool flag = !isShow;
			if (!flag)
			{
				CharacterFeatureItem config = CharacterFeature.Instance[this._reincarnationBonusFeatureId];
				this.textFeatureName.text = config.Name;
				int indexMedal = 0;
				for (int i = 0; i < 3; i++)
				{
					FeatureMedals medals = config.FeatureMedals[i];
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

		// Token: 0x060065BF RID: 26047 RVA: 0x002E7CD4 File Offset: 0x002E5ED4
		private void OpenSelectFeature()
		{
			ArgumentBox argsBox = new ArgumentBox();
			argsBox.SetObject("FeatureIds", ViewChallenge.ReincarnationBonusFeatureIdList);
			argsBox.SetObject("CurrentSelected", this._reincarnationBonusFeatureId);
			argsBox.SetObject("OnConfirm", new Action<short>(this.OnSelectFeature));
			argsBox.SetObject("OnCancel", new Action(this.OnCancelFeature));
			UIElement.SelectFeatureStartGame.SetOnInitArgs(argsBox);
			UIManager.Instance.MaskUI(UIElement.SelectFeatureStartGame);
		}

		// Token: 0x060065C0 RID: 26048 RVA: 0x002E7D5C File Offset: 0x002E5F5C
		private void OnSelectFeature(short result)
		{
			this._reincarnationBonusFeatureId = result;
			bool isSelected = this._reincarnationBonusFeatureId >= 0;
			this.ShowFeature(isSelected);
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x002E7D88 File Offset: 0x002E5F88
		private void OnCancelFeature()
		{
			bool isSelected = this._reincarnationBonusFeatureId >= 0;
			this.ShowFeature(isSelected);
			bool flag = !isSelected;
			if (flag)
			{
				this._enabledChallengeSet.Remove(this._featureItem.Config.TemplateId);
				this._featureItem.SetIsOnWithoutNotify(false);
			}
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x002E7DE0 File Offset: 0x002E5FE0
		private void SetInfernoBg(int optionalEnabledCount)
		{
			bool flag = this.imagenInfernoImg == null;
			if (!flag)
			{
				int index = ViewChallenge.GetInfernoBgSpriteIndex(optionalEnabledCount);
				string path = string.Format("{0}{1}", this.resPath, index);
				ResLoader.Load<Sprite>(path, delegate(Sprite sprite)
				{
					this.imagenInfernoImg.sprite = sprite;
				}, null, false);
			}
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x002E7E34 File Offset: 0x002E6034
		public static int GetInfernoBgSpriteIndex(int optionalEnabledCount)
		{
			bool flag = optionalEnabledCount <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				if (!true)
				{
				}
				int num;
				switch (optionalEnabledCount)
				{
				case 1:
					num = 1;
					break;
				case 2:
					num = 2;
					break;
				case 3:
					num = 3;
					break;
				case 4:
				case 5:
					num = 4;
					break;
				case 6:
				case 7:
					num = 5;
					break;
				case 8:
				case 9:
					num = 6;
					break;
				case 10:
				case 11:
					num = 7;
					break;
				default:
					num = 8;
					break;
				}
				if (!true)
				{
				}
				result = num;
			}
			return result;
		}

		// Token: 0x040046E8 RID: 18152
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x040046E9 RID: 18153
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x040046EA RID: 18154
		[SerializeField]
		private TooltipInvoker tipButtonConfirm;

		// Token: 0x040046EB RID: 18155
		[SerializeField]
		private CToggle switchToggle;

		// Token: 0x040046EC RID: 18156
		[SerializeField]
		private GameObject rootOff;

		// Token: 0x040046ED RID: 18157
		[SerializeField]
		private GameObject rootEffectOn;

		// Token: 0x040046EE RID: 18158
		[SerializeField]
		private GameObject toggleEffectOn;

		// Token: 0x040046EF RID: 18159
		[SerializeField]
		private TextMeshProUGUI textCount;

		// Token: 0x040046F0 RID: 18160
		[SerializeField]
		private TextMeshProUGUI textTotalProtagonistPoint;

		// Token: 0x040046F1 RID: 18161
		[SerializeField]
		private TextMeshProUGUI textEssentialProtagonistPoint;

		// Token: 0x040046F2 RID: 18162
		[SerializeField]
		private TextMeshProUGUI textOptionalProtagonistPoint;

		// Token: 0x040046F3 RID: 18163
		[SerializeField]
		private TextMeshProUGUI textOptionalTitle;

		// Token: 0x040046F4 RID: 18164
		[SerializeField]
		private List<NewGameSubPageChallengeItem> essentialItemList;

		// Token: 0x040046F5 RID: 18165
		[SerializeField]
		private List<NewGameSubPageChallengeItem> optionalItemList;

		// Token: 0x040046F6 RID: 18166
		[SerializeField]
		private List<NewGameSubPageChallengeItem> bonusItemList;

		// Token: 0x040046F7 RID: 18167
		[SerializeField]
		private GameObject warningTips;

		// Token: 0x040046F8 RID: 18168
		[SerializeField]
		private TextMeshProUGUI customTipsText;

		// Token: 0x040046F9 RID: 18169
		[Header("大道轮回")]
		[SerializeField]
		private CButton buttonChangeFeature;

		// Token: 0x040046FA RID: 18170
		[SerializeField]
		private GameObject rootFeature;

		// Token: 0x040046FB RID: 18171
		[SerializeField]
		private GameObject challengeOff;

		// Token: 0x040046FC RID: 18172
		[SerializeField]
		private CImage[] levelImages;

		// Token: 0x040046FD RID: 18173
		[SerializeField]
		private TextMeshProUGUI textFeatureName;

		// Token: 0x040046FE RID: 18174
		[SerializeField]
		private CImage imagenInfernoImg;

		// Token: 0x040046FF RID: 18175
		private short _reincarnationBonusFeatureId = -1;

		// Token: 0x04004700 RID: 18176
		private static readonly List<short> ReincarnationBonusFeatureIdList = new List<short>
		{
			232,
			233,
			234,
			235,
			236,
			237,
			238,
			239,
			240,
			241
		};

		// Token: 0x04004701 RID: 18177
		private NewGameSubPageChallengeItem _featureItem;

		// Token: 0x04004702 RID: 18178
		private readonly int _featureChallengeItemTemplateId = 21;

		// Token: 0x04004703 RID: 18179
		private readonly HashSet<int> _enabledChallengeSet = new HashSet<int>();

		// Token: 0x04004704 RID: 18180
		private const int MinEnabledOptionalChallengeItemCount = 3;

		// Token: 0x04004705 RID: 18181
		private string resPath = "RemakeResources/Textures/RemakeTextures/WordDetail/ui9_tex_world_detail_Inferno_";
	}
}
