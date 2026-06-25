using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Switch;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.NewGame
{
	// Token: 0x020007E7 RID: 2023
	public class NewGameFeatureItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x06006295 RID: 25237 RVA: 0x002D209D File Offset: 0x002D029D
		public ProtagonistFeatureItem Data
		{
			get
			{
				return this._featureData;
			}
		}

		// Token: 0x06006296 RID: 25238 RVA: 0x002D20A8 File Offset: 0x002D02A8
		public void Init(ProtagonistFeatureItem data, bool isSelected, bool canSelect, int[] currentSpentPoints, int maxPoints, Action<NewGameFeatureItem> onMainClick, Action<ProtagonistFeatureItem> onSelectCustomItemClick, bool customItemsSelected = false)
		{
			this._featureData = data;
			this._onMainClick = onMainClick;
			this._onSelectCustomItemClick = onSelectCustomItemClick;
			this._customItemsSelected = customItemsSelected;
			bool needPlayAppearEffect = this._canSelect != canSelect && canSelect;
			bool needPlayDisappearEffect = this._canSelect != canSelect && !canSelect;
			this._isSelected = isSelected;
			this._canSelect = canSelect;
			this._currentSpentPoints = currentSpentPoints;
			this._maxPoints = maxPoints;
			this._isHovering = false;
			this.nameText.text = data.Name;
			bool flag = this.costText != null;
			if (flag)
			{
				this.costText.text = string.Format("-{0}", data.Cost);
			}
			this.UpdateCostIcon();
			this.UpdateDisabledStyle();
			this.UpdateMouseTip();
			this.mainBtn.interactable = (canSelect || isSelected);
			this.mainBtn.ClearAndAddListener(delegate
			{
				Action<NewGameFeatureItem> onMainClick2 = this._onMainClick;
				if (onMainClick2 != null)
				{
					onMainClick2(this);
				}
			});
			this.SetupSelectCustomItemToggle(isSelected, onSelectCustomItemClick);
			bool flag2 = this.mouseTipDisplayer != null;
			if (flag2)
			{
				this.mouseTipDisplayer.Type = TipType.Simple;
				this.mouseTipDisplayer.IsLanguageKey = false;
				this.mouseTipDisplayer.enabled = true;
			}
			bool flag3 = needPlayAppearEffect;
			if (flag3)
			{
				this.PlayAppearEffect();
			}
			else
			{
				bool flag4 = needPlayDisappearEffect;
				if (flag4)
				{
					this.PlayDisappearEffect();
				}
			}
		}

		// Token: 0x06006297 RID: 25239 RVA: 0x002D21FC File Offset: 0x002D03FC
		private void SetupSelectCustomItemToggle(bool isSelected, Action<ProtagonistFeatureItem> onSelectCustomItemClick)
		{
			bool flag = this.selectCustomItemToggle == null;
			if (!flag)
			{
				bool show = isSelected && onSelectCustomItemClick != null;
				this.selectCustomItemToggle.gameObject.SetActive(show);
				bool flag2 = !show;
				if (!flag2)
				{
					this.ApplyCustomItemToggleVisual(this._customItemsSelected);
					this.selectCustomItemToggle.onValueChanged.RemoveAllListeners();
					this.selectCustomItemToggle.onValueChanged.AddListener(delegate(bool _)
					{
						this.ApplyCustomItemToggleVisual(this._customItemsSelected);
						Action<ProtagonistFeatureItem> onSelectCustomItemClick2 = onSelectCustomItemClick;
						if (onSelectCustomItemClick2 != null)
						{
							onSelectCustomItemClick2(this._featureData);
						}
					});
				}
			}
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x002D2297 File Offset: 0x002D0497
		private void ApplyCustomItemToggleVisual(bool isOn)
		{
			this.selectCustomItemToggle.SetIsOnWithoutNotify(isOn);
			this.selectCustomItemToggle.OnClick(isOn);
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x002D22B4 File Offset: 0x002D04B4
		public void PlayAppearEffect()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				this.effectPlayer.PlayEffectAt(this.effectPlayer.transform, NewGameFeatureItem.AppearEffectName, NewGameFeatureItem.EffectDuration, false);
			}
		}

		// Token: 0x0600629A RID: 25242 RVA: 0x002D22F8 File Offset: 0x002D04F8
		public void PlayDisappearEffect()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				this.effectPlayer.PlayEffectAt(this.effectPlayer.transform, NewGameFeatureItem.DisappearEffectName, NewGameFeatureItem.EffectDuration, false);
			}
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x002D233C File Offset: 0x002D053C
		private void UpdateCostIcon()
		{
			bool flag = this.costIcon == null || this._featureData == null;
			if (!flag)
			{
				int cost = (int)this._featureData.Cost;
				bool flag2 = cost <= 0;
				if (flag2)
				{
					this.costIcon.gameObject.SetActive(false);
				}
				else
				{
					this.costIcon.gameObject.SetActive(true);
					int colorIndex = (int)this._featureData.Type;
					int clampedCost = Mathf.Clamp(cost, 1, 5);
					string spriteName = string.Format("{0}{1}_{2}_{3}", new object[]
					{
						"ui9_icon_feature_cost_",
						colorIndex,
						clampedCost,
						0
					});
					this.costIcon.SetSprite(spriteName, false, null);
				}
			}
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x002D2408 File Offset: 0x002D0608
		private void UpdateDisabledStyle()
		{
			bool isDisabled = !this._canSelect && !this._isSelected;
			bool flag = this.disableStyle != null;
			if (flag)
			{
				this.disableStyle.RestoreAllToWhite();
				bool flag2 = isDisabled;
				if (flag2)
				{
					this.disableStyle.MultiplyColor(NewGameFeatureItem.DisabledMultiply);
				}
			}
			bool flag3 = this.lockIcon != null;
			if (flag3)
			{
				this.lockIcon.SetActive(isDisabled);
			}
			bool flag4 = this.nameText != null && this._featureData != null;
			if (flag4)
			{
				int typeIndex = Mathf.Clamp((int)this._featureData.Type, 0, NewGameFeatureItem.TypeNameColors.Length - 1);
				this.nameText.color = (isDisabled ? NewGameFeatureItem.DisabledTextColor : NewGameFeatureItem.TypeNameColors[typeIndex]);
			}
			bool flag5 = this.costText != null;
			if (flag5)
			{
				this.costText.color = (isDisabled ? NewGameFeatureItem.DisabledTextColor : NewGameFeatureItem.CostTextEnabledColor);
			}
			base.GetComponent<CanvasGroup>().alpha = (isDisabled ? 0.3f : 1f);
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x002D252C File Offset: 0x002D072C
		private void UpdateMouseTip()
		{
			bool flag = this.mouseTipDisplayer == null || this._featureData == null || this._currentSpentPoints == null;
			if (!flag)
			{
				sbyte typeIndex = this._featureData.Type;
				int selectedTypePoint = this._currentSpentPoints[(int)typeIndex];
				int leftPoint = this._maxPoints - this._currentSpentPoints[3];
				bool hasEnoughPoint = leftPoint >= (int)this._featureData.Cost || this._isSelected;
				bool hasEnoughPrereq = (int)this._featureData.PrerequisiteCost <= selectedTypePoint || this._isSelected;
				ValueTuple<string, string> mouseTipContent = NewGameFeatureItem.GetMouseTipContent(this._featureData, selectedTypePoint, hasEnoughPoint, hasEnoughPrereq);
				string title = mouseTipContent.Item1;
				string desc = mouseTipContent.Item2;
				this.mouseTipDisplayer.PresetParam = new string[]
				{
					title,
					desc
				};
			}
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x002D25F8 File Offset: 0x002D07F8
		public static ValueTuple<string, string> GetMouseTipContent(ProtagonistFeatureItem featureData, int selectedTypePoint = -1, bool hasEnoughPoint = true, bool hasEnoughPrereq = true)
		{
			sbyte typeIndex = featureData.Type;
			Color[] gradeColors = Colors.Instance.GradeColors;
			if (!true)
			{
			}
			int num;
			if (typeIndex != 0)
			{
				if (typeIndex != 1)
				{
					num = 5;
				}
				else
				{
					num = 6;
				}
			}
			else
			{
				num = 3;
			}
			if (!true)
			{
			}
			string pointColorString = gradeColors[num].ColorToHexString("#");
			string abilityTypeString = LocalStringManager.Get(NewGameFeatureItem.TypeLangKeys[(int)typeIndex]);
			string conditionColorString = hasEnoughPrereq ? pointColorString : Color.red.ColorToHexString("#");
			string pointEnoughColorString = hasEnoughPoint ? Colors.Instance.GradeColors[1].ColorToHexString("#") : Color.red.ColorToHexString("#");
			string content = LocalStringManager.GetFormat(LanguageKey.UI_NewGame_Tip_PointInfo, new object[]
			{
				pointEnoughColorString,
				featureData.Cost,
				pointColorString,
				abilityTypeString,
				conditionColorString,
				hasEnoughPrereq ? ((int)featureData.PrerequisiteCost) : selectedTypePoint,
				featureData.PrerequisiteCost
			});
			string nl = "\n" + LanguageKey.LK_Dot_Content.Tr();
			string effectDesc = (LanguageKey.LK_Lingxing_Sprite.Tr() + LanguageKey.UI_CharacterMenu_FeatureEffect_Extra.Tr()).SetColor("pinkyellow") + nl + featureData.EffectDesc.Replace("\n\n", nl);
			string content2 = string.Concat(new string[]
			{
				content.TrimEnd(),
				"\n\n",
				featureData.Desc,
				"\n\n",
				effectDesc
			});
			return new ValueTuple<string, string>(featureData.Name, content2);
		}

		// Token: 0x0600629F RID: 25247 RVA: 0x002D2794 File Offset: 0x002D0994
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool flag = !this.mainBtn.interactable;
			if (!flag)
			{
				this._isHovering = true;
				this.UpdateCostIcon();
				this.UpdateMouseTip();
			}
		}

		// Token: 0x060062A0 RID: 25248 RVA: 0x002D27CB File Offset: 0x002D09CB
		public void OnPointerExit(PointerEventData eventData)
		{
			this._isHovering = false;
			this.UpdateCostIcon();
		}

		// Token: 0x0400449E RID: 17566
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x0400449F RID: 17567
		[SerializeField]
		private TextMeshProUGUI costText;

		// Token: 0x040044A0 RID: 17568
		[SerializeField]
		private CImage costIcon;

		// Token: 0x040044A1 RID: 17569
		[SerializeField]
		private CButton mainBtn;

		// Token: 0x040044A2 RID: 17570
		[Tooltip("若打开界面选中物品并确认，则显示isOn状态，若未选择则显示isOn为false的状态")]
		[SerializeField]
		private SwitchToggleSmall selectCustomItemToggle;

		// Token: 0x040044A3 RID: 17571
		[SerializeField]
		private ColorMultiplyStyleRoot disableStyle;

		// Token: 0x040044A4 RID: 17572
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x040044A5 RID: 17573
		[SerializeField]
		private GameObject lockIcon;

		// Token: 0x040044A6 RID: 17574
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x040044A7 RID: 17575
		private static readonly string AppearEffectName = "eff_newgame_tezhichu_ui_001";

		// Token: 0x040044A8 RID: 17576
		private static readonly string DisappearEffectName = "eff_newgame_tezhxiao_ui_001";

		// Token: 0x040044A9 RID: 17577
		public static readonly float EffectDuration = 0.2f;

		// Token: 0x040044AA RID: 17578
		private static readonly Color[] TypeNameColors = new Color[]
		{
			new Color(0.365f, 0.573f, 0.765f),
			new Color(0.745f, 0.541f, 0.184f),
			new Color(0.533f, 0.325f, 0.565f)
		};

		// Token: 0x040044AB RID: 17579
		private static readonly string[] TypeLangKeys = new string[]
		{
			"UI_NewGame_Speciality_Experience",
			"UI_NewGame_Speciality_Wealth",
			"UI_NewGame_Speciality_Artistry"
		};

		// Token: 0x040044AC RID: 17580
		private static readonly Color DisabledTextColor = new Color(0.463f, 0.459f, 0.463f);

		// Token: 0x040044AD RID: 17581
		private static readonly Color CostTextEnabledColor = new Color(0.812f, 0.792f, 0.761f);

		// Token: 0x040044AE RID: 17582
		private static readonly Vector4 DisabledMultiply = new Vector4(0.5f, 0.5f, 0.5f, 1f);

		// Token: 0x040044AF RID: 17583
		private ProtagonistFeatureItem _featureData;

		// Token: 0x040044B0 RID: 17584
		private Action<NewGameFeatureItem> _onMainClick;

		// Token: 0x040044B1 RID: 17585
		private Action<ProtagonistFeatureItem> _onSelectCustomItemClick;

		// Token: 0x040044B2 RID: 17586
		private bool _isSelected;

		// Token: 0x040044B3 RID: 17587
		private bool _isHovering;

		// Token: 0x040044B4 RID: 17588
		private bool _canSelect;

		// Token: 0x040044B5 RID: 17589
		private bool _customItemsSelected;

		// Token: 0x040044B6 RID: 17590
		private int[] _currentSpentPoints;

		// Token: 0x040044B7 RID: 17591
		private int _maxPoints;
	}
}
