using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.Buildings.Migrate;
using GameData.Domains.Building;
using GameData.Domains.Building.Display;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Buildings
{
	// Token: 0x02000BC3 RID: 3011
	public class ViewPracticeRoomPuppet : UIBase
	{
		// Token: 0x17001040 RID: 4160
		// (get) Token: 0x060097A5 RID: 38821 RVA: 0x0046A575 File Offset: 0x00468775
		private int CurrDate
		{
			get
			{
				return SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			}
		}

		// Token: 0x17001041 RID: 4161
		// (get) Token: 0x060097A6 RID: 38822 RVA: 0x0046A581 File Offset: 0x00468781
		private sbyte Difficulty
		{
			get
			{
				return Puppet.Instance[this._selectedPuppet].Difficulties[(int)this.slider.value];
			}
		}

		// Token: 0x17001042 RID: 4162
		// (get) Token: 0x060097A7 RID: 38823 RVA: 0x0046A5A9 File Offset: 0x004687A9
		private bool UseLargeFeatureItem
		{
			get
			{
				return LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN;
			}
		}

		// Token: 0x060097A8 RID: 38824 RVA: 0x0046A5B3 File Offset: 0x004687B3
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x060097A9 RID: 38825 RVA: 0x0046A5B6 File Offset: 0x004687B6
		private void Awake()
		{
			this.InitPuppetTypeToggleGroup();
			this.InitPuppetScroll();
			this.InitConsummate();
			this.InitFeatureScroll();
			this.InitButtons();
		}

		// Token: 0x060097AA RID: 38826 RVA: 0x0046A5DC File Offset: 0x004687DC
		private void OnEnable()
		{
			this.RequestData();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(115);
		}

		// Token: 0x060097AB RID: 38827 RVA: 0x0046A5EE File Offset: 0x004687EE
		private void OnDisable()
		{
			this.puppetScroll.SetDataCount(0);
		}

		// Token: 0x060097AC RID: 38828 RVA: 0x0046A5FE File Offset: 0x004687FE
		private void RequestData()
		{
			BuildingDomainMethod.AsyncCall.GetPuppetPageDisplayData(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._data);
				this.UpdateAll();
			});
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				string consummateName = CommonUtils.GetConsummateLevelShowData(displayData.ConsummateLevel).Item2;
				this.consummateLevelTaiwu.text = string.Format("{0} {1} {2}", LanguageKey.LK_Consummate_Title.Tr(), displayData.ConsummateLevel, consummateName);
			});
		}

		// Token: 0x060097AD RID: 38829 RVA: 0x0046A634 File Offset: 0x00468834
		private void InitPuppetTypeToggleGroup()
		{
			this.puppetTypeToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.puppetTypeToggleGroup, 0, null);
			this.puppetTypeToggleGroup.OnActiveIndexChange += this.OnPuppetTypeToggleGroupChange;
			for (int i = 0; i < this.puppetTypeToggleGroup.Count(); i++)
			{
				this._puppets[i] = new List<short>();
			}
		}

		// Token: 0x060097AE RID: 38830 RVA: 0x0046A6A7 File Offset: 0x004688A7
		private void InitPuppetScroll()
		{
			this.puppetScroll.OnItemRender += this.OnRenderPuppet;
		}

		// Token: 0x060097AF RID: 38831 RVA: 0x0046A6C2 File Offset: 0x004688C2
		private void InitConsummate()
		{
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.UpdateConsummate));
		}

		// Token: 0x060097B0 RID: 38832 RVA: 0x0046A6E2 File Offset: 0x004688E2
		private void InitFeatureScroll()
		{
			this.featureScroll.OnItemRender += this.OnRenderFeature;
		}

		// Token: 0x060097B1 RID: 38833 RVA: 0x0046A700 File Offset: 0x00468900
		private void InitButtons()
		{
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.btnSelectAll.ClearAndAddListener(new Action(this.OnClickSelectAll));
			this.btnDeselectAll.ClearAndAddListener(new Action(this.OnClickDeselectAll));
			for (int i = 0; i < this.modeBtnList.Count; i++)
			{
				int mode = i;
				CButton btn = this.modeBtnList[i];
				btn.ClearAndAddListener(delegate
				{
					bool flag = this._selectedPuppet < 0;
					if (!flag)
					{
						bool flag2 = !this.CanClick();
						if (!flag2)
						{
							CombatDomainMethod.Call.EnterBossPuppetCombat(this._selectedPuppet, this.Difficulty, mode == 1);
							WorldDomainMethod.Call.AdvanceDaysInMonth(this._modeCostMap[mode]);
							UIManager.Instance.HideUI(UIElement.BuildingQuickActionMenu);
						}
					}
				});
			}
		}

		// Token: 0x060097B2 RID: 38834 RVA: 0x0046A7A8 File Offset: 0x004689A8
		private void UpdateAll()
		{
			foreach (List<short> list in this._puppets.Values)
			{
				list.Clear();
			}
			PuppetPageDisplayData data = this._data;
			if (data.Puppets == null)
			{
				data.Puppets = new List<short>();
			}
			data = this._data;
			if (data.Features == null)
			{
				data.Features = new List<short>();
			}
			foreach (short id in this._data.Puppets)
			{
				this._puppets[(int)Puppet.Instance[id].Type].Add(id);
			}
			for (int i = 0; i < this.puppetTypeToggleGroup.Count(); i++)
			{
				this.puppetTypeToggleGroup.Get(i).interactable = (this._puppets[i].Count != 0);
			}
			int index = this.puppetTypeToggleGroup.GetActiveIndex();
			bool flag = index < 0 || this._puppets[index].Count < 0;
			if (flag)
			{
				for (int j = 0; j < this.puppetTypeToggleGroup.Count(); j++)
				{
					bool flag2 = this._puppets[j].Count > 0;
					if (flag2)
					{
						this.puppetTypeToggleGroup.SetWithoutNotify(j);
						break;
					}
				}
			}
			this.featureScroll.UpdateStyle(this.UseLargeFeatureItem ? this.featureTemplateLarge : this.featureTemplate, this.UseLargeFeatureItem ? 1 : 2);
			this.featureScroll.SetDataCount(14);
			this.featureScroll.ScrollTo(0, 0.3f);
			this.UpdatePuppets();
			this.UpdateFeatures();
			this.UpdateButtons();
		}

		// Token: 0x060097B3 RID: 38835 RVA: 0x0046A9C8 File Offset: 0x00468BC8
		private void UpdatePuppets()
		{
			int index = this.puppetTypeToggleGroup.GetActiveIndex();
			int count = this._puppets[index].Count;
			this.puppetScroll.SetDataCount(Mathf.Max(count, this.puppetScroll.PageCount));
			bool flag = this._puppets[index].Count >= 0;
			if (flag)
			{
				this.OnClickPuppet(this._puppets[index][0]);
			}
			this.puppetScroll.ScrollTo(0, 0.3f);
		}

		// Token: 0x060097B4 RID: 38836 RVA: 0x0046AA58 File Offset: 0x00468C58
		private void UpdateConsummate(float value = 0f)
		{
			bool flag = this._selectedPuppet < 0;
			if (!flag)
			{
				string difficulty = this.Difficulty.ToString();
				this.consummateLevelText.text = difficulty;
				this.consummateLevelText2.text = difficulty;
				for (int i = 0; i < this.names.childCount; i++)
				{
					PracticeRoomPuppetLevelName levelName = this.names.GetChild(i).GetComponent<PracticeRoomPuppetLevelName>();
					bool flag2 = !levelName.gameObject.activeSelf;
					if (!flag2)
					{
						levelName.normal.SetActive((float)i > value);
						levelName.selected.SetActive((float)i <= value);
					}
				}
			}
		}

		// Token: 0x060097B5 RID: 38837 RVA: 0x0046AB10 File Offset: 0x00468D10
		private void UpdateFeatures()
		{
			bool flag = this.puppetTypeToggleGroup.GetActiveIndex() != 1;
			if (flag)
			{
				this.btnSelectAll.interactable = false;
				this.btnDeselectAll.interactable = false;
				this.featureScroll.gameObject.SetActive(false);
				this.featureScroll.emptyObject.SetActive(true);
			}
			else
			{
				this.featureCount.text = this._data.Features.Count.ToString();
				this.btnSelectAll.interactable = true;
				this.btnDeselectAll.interactable = true;
				this.featureScroll.gameObject.SetActive(true);
				this.featureScroll.emptyObject.SetActive(this.featureScroll.CurrentDataCount <= 0);
				for (int i = 0; i < 14; i++)
				{
					GameObject obj = this.featureScroll.GetActiveCell(i);
					bool flag2 = obj != null;
					if (flag2)
					{
						obj.GetComponent<PracticeRoomPuppetFeature>().SetSelected(this._data.Features.Contains(CombatSkillType.Instance[i].LegendaryBookConsumedFeature));
					}
					short templateId = CombatSkillType.Instance[i].LegendaryBookConsumedFeature;
					bool value = this._data.Features.Contains(templateId);
					ExtraDomainMethod.Call.UpdateWoodenXiangshuAvatarSelectedFeatures(templateId, value);
				}
			}
		}

		// Token: 0x060097B6 RID: 38838 RVA: 0x0046AC78 File Offset: 0x00468E78
		private void UpdateButtons()
		{
			for (int i = 0; i < this.modeBtnList.Count; i++)
			{
				this.modeBtnList[i].GetComponent<PracticeRoomPuppetMode>().Set(this._modeCostMap[i], this.CurrDate, this._data.IsAtSettlement);
				this.modeBtnList[i].GetComponent<PracticeRoomPuppetMode>().SetTips(i == 0, this.Difficulty);
			}
		}

		// Token: 0x060097B7 RID: 38839 RVA: 0x0046ACF7 File Offset: 0x00468EF7
		private void OnPuppetTypeToggleGroupChange(int _, int __)
		{
			this.UpdatePuppets();
			this.UpdateFeatures();
		}

		// Token: 0x060097B8 RID: 38840 RVA: 0x0046AD08 File Offset: 0x00468F08
		private void OnRenderPuppet(int index, GameObject obj)
		{
			PracticeRoomPuppetTemplate item = obj.GetComponent<PracticeRoomPuppetTemplate>();
			short templateId = -1;
			bool flag = this._puppets[this.puppetTypeToggleGroup.GetActiveIndex()].CheckIndex(index);
			if (flag)
			{
				templateId = this._puppets[this.puppetTypeToggleGroup.GetActiveIndex()][index];
			}
			item.Set(templateId);
			item.SetSelected(templateId == this._selectedPuppet);
			item.button.ClearAndAddListener(delegate
			{
				this.OnClickPuppet(templateId);
			});
		}

		// Token: 0x060097B9 RID: 38841 RVA: 0x0046ADB0 File Offset: 0x00468FB0
		private void OnClickPuppet(short templateId)
		{
			List<short> puppets = this._puppets[this.puppetTypeToggleGroup.GetActiveIndex()];
			for (int i = 0; i < puppets.Count; i++)
			{
				GameObject cell = this.puppetScroll.GetActiveCell(i);
				bool flag = cell == null;
				if (!flag)
				{
					bool flag2 = puppets[i] == this._selectedPuppet;
					if (flag2)
					{
						cell.GetComponent<PracticeRoomPuppetTemplate>().SetSelected(false);
						break;
					}
				}
			}
			bool flag3 = templateId < 0;
			if (flag3)
			{
				this.consummateLevelPanel.SetActive(false);
			}
			else
			{
				PuppetItem config = Puppet.Instance[templateId];
				for (int j = 0; j < puppets.Count; j++)
				{
					GameObject cell2 = this.puppetScroll.GetActiveCell(j);
					bool flag4 = cell2 == null;
					if (!flag4)
					{
						bool flag5 = puppets[j] == templateId;
						if (flag5)
						{
							cell2.GetComponent<PracticeRoomPuppetTemplate>().SetSelected(true);
							break;
						}
					}
				}
				this.slider.maxValue = (float)(config.Difficulties.Count - 1);
				this.slider.SetValueWithoutNotify((float)((this._selectedPuppet < 0) ? 0 : Math.Max(0, config.Difficulties.IndexOf(this.Difficulty))));
				for (int k = 0; k < config.Difficulties.Count; k++)
				{
					Transform obj = this.names.GetChild(k);
					TooltipInvoker mouseTip = obj.GetComponent<TooltipInvoker>();
					sbyte consummateLevel = config.Difficulties[k];
					ConsummateLevelItem consummateConfig = ConsummateLevel.Instance[consummateLevel];
					PracticeRoomPuppetLevelName levelName = obj.GetComponent<PracticeRoomPuppetLevelName>();
					levelName.normalName.text = consummateConfig.Name;
					levelName.selectedName.text = consummateConfig.Name;
					mouseTip.PresetParam[0] = config.Name.ColorReplace();
					mouseTip.PresetParam[1] = CommonUtils.GetConsummateLevelTips((int)consummateLevel) + "\n\n" + consummateConfig.Desc.ColorReplace();
					obj.gameObject.SetActive(true);
				}
				for (int l = config.Difficulties.Count; l < this.names.childCount; l++)
				{
					this.names.GetChild(l).gameObject.SetActive(false);
				}
				this.SetConsummateLabelPosition(templateId);
				this.consummateLevelPanel.SetActive(true);
			}
			this._selectedPuppet = templateId;
			this.UpdateConsummate(this.slider.value);
		}

		// Token: 0x060097BA RID: 38842 RVA: 0x0046B050 File Offset: 0x00469250
		private void OnRenderFeature(int index, GameObject obj)
		{
			PracticeRoomPuppetFeature item = obj.GetComponent<PracticeRoomPuppetFeature>();
			short templateId = ((this._data.LegendaryBookOwningState & 1 << index) != 0) ? CombatSkillType.Instance[index].LegendaryBookConsumedFeature : -1;
			item.Set(templateId);
			item.SetSelected(templateId >= 0 && this._data.Features.Contains(templateId));
			item.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.OnClickFeature(templateId);
			});
		}

		// Token: 0x060097BB RID: 38843 RVA: 0x0046B0F0 File Offset: 0x004692F0
		private void OnClickFeature(short templateId)
		{
			bool flag = templateId < 0;
			if (!flag)
			{
				bool flag2 = this._data.Features.Contains(templateId);
				if (flag2)
				{
					this._data.Features.Remove(templateId);
				}
				else
				{
					this._data.Features.Add(templateId);
				}
				this.UpdateFeatures();
			}
		}

		// Token: 0x060097BC RID: 38844 RVA: 0x0046B14C File Offset: 0x0046934C
		private void OnClickSelectAll()
		{
			this._data.Features.Clear();
			for (int i = 0; i < 14; i++)
			{
				bool flag = (this._data.LegendaryBookOwningState & 1 << i) != 0;
				if (flag)
				{
					this._data.Features.Add(CombatSkillType.Instance[i].LegendaryBookConsumedFeature);
				}
			}
			this.UpdateFeatures();
		}

		// Token: 0x060097BD RID: 38845 RVA: 0x0046B1BD File Offset: 0x004693BD
		private void OnClickDeselectAll()
		{
			this._data.Features.Clear();
			this.UpdateFeatures();
		}

		// Token: 0x060097BE RID: 38846 RVA: 0x0046B1D8 File Offset: 0x004693D8
		private void SetConsummateLabelPosition(short templateId)
		{
			int count = Puppet.Instance[templateId].Difficulties.Count;
			float width = this.names.GetComponent<RectTransform>().rect.width;
			float interval = width / (float)(count - 1);
			for (int i = 0; i < count; i++)
			{
				Transform obj = this.names.GetChild(i);
				bool flag = i == 0;
				if (flag)
				{
					obj.localPosition = obj.localPosition.SetX(0f);
				}
				else
				{
					bool flag2 = i == count - 1;
					if (flag2)
					{
						obj.localPosition = obj.localPosition.SetX(width);
					}
					else
					{
						obj.localPosition = obj.localPosition.SetX(interval * (float)i);
					}
				}
			}
		}

		// Token: 0x04007461 RID: 29793
		[SerializeField]
		private CToggleGroup puppetTypeToggleGroup;

		// Token: 0x04007462 RID: 29794
		[SerializeField]
		private InfinityScroll puppetScroll;

		// Token: 0x04007463 RID: 29795
		[SerializeField]
		private GameObject consummateLevelPanel;

		// Token: 0x04007464 RID: 29796
		[SerializeField]
		private TextMeshProUGUI consummateLevelText;

		// Token: 0x04007465 RID: 29797
		[SerializeField]
		private TextMeshProUGUI consummateLevelText2;

		// Token: 0x04007466 RID: 29798
		[SerializeField]
		private TextMeshProUGUI consummateLevelTaiwu;

		// Token: 0x04007467 RID: 29799
		[SerializeField]
		private Transform names;

		// Token: 0x04007468 RID: 29800
		[SerializeField]
		private CSlider slider;

		// Token: 0x04007469 RID: 29801
		[SerializeField]
		private InfinityScroll featureScroll;

		// Token: 0x0400746A RID: 29802
		[SerializeField]
		private GameObject featureTemplate;

		// Token: 0x0400746B RID: 29803
		[SerializeField]
		private GameObject featureTemplateLarge;

		// Token: 0x0400746C RID: 29804
		[SerializeField]
		private GameObject featurePanel;

		// Token: 0x0400746D RID: 29805
		[SerializeField]
		private TextMeshProUGUI featureCount;

		// Token: 0x0400746E RID: 29806
		[SerializeField]
		private CButton btnSelectAll;

		// Token: 0x0400746F RID: 29807
		[SerializeField]
		private CButton btnDeselectAll;

		// Token: 0x04007470 RID: 29808
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04007471 RID: 29809
		[SerializeField]
		private List<CButton> modeBtnList;

		// Token: 0x04007472 RID: 29810
		private const int SmallFeatureItemLineCount = 2;

		// Token: 0x04007473 RID: 29811
		private const int BigFeatureItemLineCount = 1;

		// Token: 0x04007474 RID: 29812
		private readonly Dictionary<int, int> _modeCostMap = new Dictionary<int, int>
		{
			{
				0,
				10
			},
			{
				1,
				0
			}
		};

		// Token: 0x04007475 RID: 29813
		private PuppetPageDisplayData _data;

		// Token: 0x04007476 RID: 29814
		private Dictionary<int, List<short>> _puppets = new Dictionary<int, List<short>>();

		// Token: 0x04007477 RID: 29815
		private short _selectedPuppet = -1;
	}
}
