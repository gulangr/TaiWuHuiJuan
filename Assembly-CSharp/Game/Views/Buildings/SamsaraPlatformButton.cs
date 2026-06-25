using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.ListStyleGeneralScroll;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings
{
	// Token: 0x02000BC1 RID: 3009
	public class SamsaraPlatformButton : MonoBehaviour
	{
		// Token: 0x0600978D RID: 38797 RVA: 0x004696FC File Offset: 0x004678FC
		private void Awake()
		{
			this.samsaraDisplayer.Type = TipType.Destiny;
			this.samsaraDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("DestinyType", this.templateId).Set("ScreenWidth", 1860f);
			this.charDisplayer.Type = TipType.DeadCharacterComplete;
			this.rebornBtn.onClick.ResetListener(new Action(this.Reborn));
			this.unlockBtn.onClick.ResetListener(new Action(this.UnlockSlot));
			this.charButtonBtn.onClick.ResetListener(new Action(this.ShowSelectCharWindow));
			this.charEmptyButtonBtn.onClick.ResetListener(new Action(this.ShowSelectCharWindow));
			this.removeBtn.onClick.ResetListener(delegate()
			{
				this.OnSelectCharId(-1);
			});
		}

		// Token: 0x0600978E RID: 38798 RVA: 0x004697E8 File Offset: 0x004679E8
		private void OnEnable()
		{
			this.OnHoverCharEnd();
		}

		// Token: 0x0600978F RID: 38799 RVA: 0x004697F4 File Offset: 0x004679F4
		public void Init(ViewSamsaraPlatform platform)
		{
			this.parent = platform;
			this._unlocked = platform.BuildingData.SlotIsUnlocked((int)this.templateId);
			this.bg.sprite = (this._unlocked ? this.enableBg : this.disableBg);
			this.buildingNameBase.sprite = (this._unlocked ? this.enableName : this.disableName);
			this.buildingName.text = DestinyType.Instance[this.templateId].Name;
			bool unlocked = this._unlocked;
			if (unlocked)
			{
				this.parent = platform;
				BuildingDomainMethod.AsyncCall.GetSamsaraPlatformCharDisplayData(platform, this.templateId, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._charDisplayData);
					this.Refresh();
				});
			}
			else
			{
				this._charDisplayData.Data = null;
				this.Refresh();
			}
		}

		// Token: 0x06009790 RID: 38800 RVA: 0x004698CC File Offset: 0x00467ACC
		public void Refresh()
		{
			SamsaraPlatformCharDisplayData charDisplayData = this._charDisplayData;
			bool flag = charDisplayData != null && charDisplayData.Id != -1;
			if (flag)
			{
				this.processing.gameObject.SetActive(true);
				this.locked.gameObject.SetActive(false);
				this.empty.gameObject.SetActive(false);
				this.rebornBtn.gameObject.SetActive(true);
				this.emptyHover.gameObject.SetActive(true);
				this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(ref this._charDisplayData.Data.NameData, false, false);
				this.avatar.Refresh(this._charDisplayData.AvatarRelatedData, this._charDisplayData.TemplateId);
				this.RefreshSamsaraProgress();
				TooltipInvoker tooltipInvoker = this.charDisplayer;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("CharId", this._charDisplayData.Id);
			}
			else
			{
				this.processing.gameObject.SetActive(false);
				this.emptyHover.gameObject.SetActive(this._unlocked);
				this.locked.gameObject.SetActive(!this._unlocked);
				this.empty.gameObject.SetActive(this._unlocked);
				bool unlocked = this._unlocked;
				if (!unlocked)
				{
					DestinyTypeItem destinyTypeItem = DestinyType.Instance[this.templateId];
					this.costResource.SetSprite(destinyTypeItem.UnlockResourceTypeIcon, false, null);
					sbyte costType = (sbyte)destinyTypeItem.UnlockCost.Length;
					bool flag2;
					do
					{
						sbyte b = costType;
						costType = b - 1;
						if (b <= 0)
						{
							break;
						}
						flag2 = (destinyTypeItem.UnlockCost[(int)costType] > 0);
					}
					while (!flag2);
					int holdCount = SingletonObject.getInstance<BuildingModel>().GetResourceCount(costType);
					bool enough = holdCount >= (int)destinyTypeItem.UnlockCost[(int)costType];
					this.costText.text = CommonUtils.GetDisplayStringForNum(holdCount, 100000).SetColor(enough ? "brightblue" : "brightred") + "/" + CommonUtils.GetDisplayStringForNum((int)destinyTypeItem.UnlockCost[(int)costType], 10000);
					this.unlockBtn.interactable = enough;
				}
			}
		}

		// Token: 0x06009791 RID: 38801 RVA: 0x00469B20 File Offset: 0x00467D20
		private void RefreshSamsaraProgress()
		{
			bool flag = this._charDisplayData.NameRelatedData.CharTemplateId == 779;
			if (flag)
			{
				this.progress.text = string.Format("{0}/1", Math.Clamp(this._charDisplayData.Progress, 0, 1));
				this.rebornBtn.gameObject.SetActive(this._charDisplayData.Progress > 0);
				this.progressFill.fillAmount = (float)((this._charDisplayData.Progress == 0) ? 0 : 1);
			}
			else
			{
				this.progress.text = string.Format("{0}/{1}", Math.Clamp(this._charDisplayData.Progress, 0, (int)GlobalConfig.Instance.SamsaraPlatformMaxProgress), GlobalConfig.Instance.SamsaraPlatformMaxProgress);
				this.rebornBtn.gameObject.SetActive(this._charDisplayData.Progress >= (int)GlobalConfig.Instance.SamsaraPlatformMaxProgress);
				this.progressFill.fillAmount = (float)this._charDisplayData.Progress / (float)GlobalConfig.Instance.SamsaraPlatformMaxProgress;
			}
			this.rebornBtn.interactable = true;
		}

		// Token: 0x06009792 RID: 38802 RVA: 0x00469C60 File Offset: 0x00467E60
		public void UnlockSlot()
		{
			WorldDomainMethod.Call.RequestSetStat(55, (int)(this.parent.BuildingData.CalcUnlockedLevelCount() + 1));
			BuildingDomainMethod.Call.UnlockBuildingLevelSlot(this.parent.Element.GameDataListenerId, this.parent.BuildingKey, (int)this.templateId);
		}

		// Token: 0x06009793 RID: 38803 RVA: 0x00469CAF File Offset: 0x00467EAF
		private void Reborn()
		{
			this.rebornBtn.interactable = false;
			this.parent.DataChanged = true;
			BuildingDomainMethod.AsyncCall.SamsaraPlatformReborn(this.parent, this.templateId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData motherData = new CharacterDisplayData();
				Serializer.Deserialize(pool, offset, ref motherData);
				bool flag = this._charDisplayData.NameRelatedData.CharTemplateId == 779;
				if (flag)
				{
					this.parent.QuickHide();
				}
				else
				{
					string bornCharName = NameCenter.GetMonasticTitleOrDisplayName(ref this._charDisplayData.Data.NameData, false, false).SetColor("darkbrown");
					bool flag2 = motherData != null;
					if (flag2)
					{
						MapAreaItem areaConfig = SingletonObject.getInstance<WorldMapModel>().Areas[(int)motherData.Location.AreaId].GetConfig();
						string areaName = areaConfig.Name.SetColor("pinkyellow");
						string stateName = MapState.Instance[areaConfig.StateID].Name.SetColor("pinkyellow");
						string orgInfo = CommonUtils.GetOrganizationGradeString(motherData.OrgInfo, motherData.Gender, motherData.PhysiologicalAge, -1);
						string motherName = NameCenter.GetMonasticTitleOrDisplayName(motherData, false).SetColor("darkbrown");
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
						{
							Title = LanguageKey.LK_Samsara_Platform_Success_Title.Tr(),
							Content = LanguageKey.LK_Samsara_Platform_Success_Tips.TrFormat(new object[]
							{
								bornCharName,
								DestinyType.Instance[this.templateId].Name,
								stateName,
								areaName,
								orgInfo,
								motherName
							}),
							Type = 2
						}));
					}
					else
					{
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
						{
							Title = LanguageKey.LK_Samsara_Platform_Fail_Title.Tr(),
							Content = LanguageKey.LK_Samsara_Platform_Fail_Tips.TrFormat(bornCharName),
							Type = 2
						}));
					}
					UIManager.Instance.MaskUI(UIElement.Dialog);
					this.Init(this.parent);
					this.parent.RequestData();
				}
			});
		}

		// Token: 0x06009794 RID: 38804 RVA: 0x00469CEC File Offset: 0x00467EEC
		public void OnHoverChar()
		{
			SamsaraPlatformCharDisplayData charDisplayData = this._charDisplayData;
			bool flag = charDisplayData == null || charDisplayData.Id == -1;
			if (flag)
			{
				this.emptyHoverHighlight.gameObject.SetActive(true);
			}
			else
			{
				this.parent.SetAttributeDelta(this._charDisplayData.Id);
			}
		}

		// Token: 0x06009795 RID: 38805 RVA: 0x00469D48 File Offset: 0x00467F48
		public void OnHoverCharEnd()
		{
			SamsaraPlatformCharDisplayData charDisplayData = this._charDisplayData;
			bool flag = charDisplayData == null || charDisplayData.Id == -1;
			if (flag)
			{
				this.emptyHoverHighlight.gameObject.SetActive(false);
			}
			else
			{
				this.parent.ResetAttribute();
			}
		}

		// Token: 0x06009796 RID: 38806 RVA: 0x00469D98 File Offset: 0x00467F98
		public void SetCharId(SamsaraPlatformCharDisplayData charDisplayData = null)
		{
			this._charDisplayData = charDisplayData;
			bool charIsValid = charDisplayData != null && charDisplayData.Id != -1;
			this.rebornBtn.gameObject.SetActive(false);
			this.processing.gameObject.SetActive(charIsValid);
			this.empty.gameObject.SetActive(!charIsValid);
			this.OnHoverCharEnd();
		}

		// Token: 0x06009797 RID: 38807 RVA: 0x00469E04 File Offset: 0x00468004
		private void ShowSelectCharWindow()
		{
			IEnumerable<SamsaraPlatformCharDisplayData> selector = this.parent.CharDataDict.Values.Where(delegate(SamsaraPlatformCharDisplayData x)
			{
				bool result;
				if (x.Progress == -1)
				{
					int id = x.Id;
					SamsaraPlatformCharDisplayData charDisplayData2 = this._charDisplayData;
					int? num = (charDisplayData2 != null) ? new int?(charDisplayData2.Id) : null;
					result = !(id == num.GetValueOrDefault() & num != null);
				}
				else
				{
					result = false;
				}
				return result;
			});
			SamsaraPlatformCharDisplayData charDisplayData = this._charDisplayData;
			bool flag = charDisplayData != null && charDisplayData.Id != -1;
			if (flag)
			{
				selector = selector.Prepend(this._charDisplayData);
			}
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.SamsaraPlatform);
			config.InteractionMode = ESelectCharacterInteractionMode.Instant;
			config.SelectionMode = ESelectCharacterSelectionMode.Single;
			CommonSelectCharacterConfig commonSelectCharacterConfig = config;
			charDisplayData = this._charDisplayData;
			object initialSelectedCharacterIds;
			if (charDisplayData == null || charDisplayData.Id == -1)
			{
				initialSelectedCharacterIds = null;
			}
			else
			{
				(initialSelectedCharacterIds = new List<int>()).Add(this._charDisplayData.Id);
			}
			commonSelectCharacterConfig.InitialSelectedCharacterIds = initialSelectedCharacterIds;
			config.RefreshDeadAsAlive = true;
			config.CustomColumnGenerator = new Dictionary<ESelectCharacterSubPage, Func<IEnumerable<ColumnDefinition>>>();
			config.CustomColumnGenerator[ESelectCharacterSubPage.SamsaraPlatform] = new Func<IEnumerable<ColumnDefinition>>(this.CustomColumn);
			config.CustomAvatar = ViewSelectCharacter.CreateAvatarWithNameColumn(true, delegate(TooltipInvoker displayer, int i)
			{
				displayer.Type = TipType.CharacterComplete;
				TooltipInvoker displayer2 = displayer;
				ArgumentBox argumentBox;
				if ((argumentBox = displayer2.RuntimeParam) == null)
				{
					argumentBox = (displayer2.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("CharId", i);
				CharacterDomainMethod.AsyncCall.GetDeadCharacterDisplayDataForTooltip(null, i, delegate(int offset, RawDataPool pool)
				{
					CharacterDisplayDataForTooltip characterDisplayDataForTooltip = new CharacterDisplayDataForTooltip();
					Serializer.Deserialize(pool, offset, ref characterDisplayDataForTooltip);
					displayer.RuntimeParam.SetObject("Data", characterDisplayDataForTooltip);
					displayer.enabled = true;
				});
			}, null);
			UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selector.ToList<SamsaraPlatformCharDisplayData>()).SetObject("SelectCharacterCallback", new SelectCharacterCallback(this.OnSelectChar)));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		}

		// Token: 0x06009798 RID: 38808 RVA: 0x00469F56 File Offset: 0x00468156
		private IEnumerable<ColumnDefinition> CustomColumn()
		{
			yield return ViewSelectCharacter.Relationship;
			yield return ViewSelectCharacter.Favor;
			yield return ViewSelectCharacter.BirthDate;
			yield return ViewSelectCharacter.CreateGenericTextColumn<SamsaraPlatformCharDisplayData>(() => LanguageKey.LK_Died_Time.Tr(), (SamsaraPlatformCharDisplayData data) => TimeManager.GetYearDisplayString(data.DeadAt), -1, 30f, 90f);
			yield return ViewSelectCharacter.CharacterIdentity;
			yield return ViewSelectCharacter.Fame;
			yield return ViewSelectCharacter.PreexistenceCharCount;
			yield break;
		}

		// Token: 0x06009799 RID: 38809 RVA: 0x00469F66 File Offset: 0x00468166
		private void OnSelectChar(List<int> charIds)
		{
			this.OnSelectCharId((charIds != null && charIds.Count > 0) ? charIds[0] : -1);
		}

		// Token: 0x0600979A RID: 38810 RVA: 0x00469F88 File Offset: 0x00468188
		private void OnSelectCharId(int charId)
		{
			bool flag = charId == this._charDisplayData.Id;
			if (!flag)
			{
				BuildingDomainMethod.Call.SetSamsaraPlatformChar(this.templateId, charId);
				this.parent.RequestData();
			}
		}

		// Token: 0x04007436 RID: 29750
		private const float MouseTipScreenWidth = 1860f;

		// Token: 0x04007437 RID: 29751
		private SamsaraPlatformCharDisplayData _charDisplayData = new SamsaraPlatformCharDisplayData
		{
			Data = null
		};

		// Token: 0x04007438 RID: 29752
		private bool _unlocked;

		// Token: 0x04007439 RID: 29753
		[SerializeField]
		private Sprite enableBg;

		// Token: 0x0400743A RID: 29754
		[SerializeField]
		private Sprite disableBg;

		// Token: 0x0400743B RID: 29755
		[SerializeField]
		private Sprite enableName;

		// Token: 0x0400743C RID: 29756
		[SerializeField]
		private Sprite disableName;

		// Token: 0x0400743D RID: 29757
		[SerializeField]
		private sbyte templateId;

		// Token: 0x0400743E RID: 29758
		[SerializeField]
		private ViewSamsaraPlatform parent;

		// Token: 0x0400743F RID: 29759
		[SerializeField]
		private CButton unlockBtn;

		// Token: 0x04007440 RID: 29760
		[SerializeField]
		private CButton charButtonBtn;

		// Token: 0x04007441 RID: 29761
		[SerializeField]
		private CButton charEmptyButtonBtn;

		// Token: 0x04007442 RID: 29762
		[SerializeField]
		private CButton rebornBtn;

		// Token: 0x04007443 RID: 29763
		[SerializeField]
		private CButton removeBtn;

		// Token: 0x04007444 RID: 29764
		[SerializeField]
		private RectTransform locked;

		// Token: 0x04007445 RID: 29765
		[SerializeField]
		private RectTransform empty;

		// Token: 0x04007446 RID: 29766
		[SerializeField]
		private RectTransform emptyHover;

		// Token: 0x04007447 RID: 29767
		[SerializeField]
		private RectTransform emptyHoverHighlight;

		// Token: 0x04007448 RID: 29768
		[SerializeField]
		private RectTransform processing;

		// Token: 0x04007449 RID: 29769
		[SerializeField]
		private CImage bg;

		// Token: 0x0400744A RID: 29770
		[SerializeField]
		private CImage progressFill;

		// Token: 0x0400744B RID: 29771
		[SerializeField]
		private CImage buildingNameBase;

		// Token: 0x0400744C RID: 29772
		[SerializeField]
		private CImage costResource;

		// Token: 0x0400744D RID: 29773
		[SerializeField]
		private TooltipInvoker charDisplayer;

		// Token: 0x0400744E RID: 29774
		[SerializeField]
		private TooltipInvoker samsaraDisplayer;

		// Token: 0x0400744F RID: 29775
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007450 RID: 29776
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x04007451 RID: 29777
		[SerializeField]
		private TMP_Text progress;

		// Token: 0x04007452 RID: 29778
		[SerializeField]
		private TMP_Text buildingName;

		// Token: 0x04007453 RID: 29779
		[SerializeField]
		private TMP_Text costText;
	}
}
