using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Display.VillagerRoleArrangement;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Villager
{
	// Token: 0x02000745 RID: 1861
	public class VillagerWorkDetail : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06005A30 RID: 23088 RVA: 0x0029D95D File Offset: 0x0029BB5D
		private void Awake()
		{
			this.OnPointerExit(null);
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x0029D967 File Offset: 0x0029BB67
		private void OnDisable()
		{
			this.OnPointerExit(null);
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x0029D974 File Offset: 0x0029BB74
		public void Set(ViewVillagerWork parentObject, int index, bool chickenUnlocked, bool setAsSelected)
		{
			this.parent = parentObject;
			this.isSelected = setAsSelected;
			bool flag = index < 0 || index >= this.parent.SortedCharacterIdList.Count;
			if (!flag)
			{
				int charId = this.parent.SortedCharacterIdList[index];
				VillagerRoleCharacterDisplayData displayData = this.parent.TaiwuVillagerRoleDisplayData.Villagers[charId];
				this.selfButton.onClick.ResetListener(delegate()
				{
					bool flag3 = this.parent.SelectedCharacterId != charId;
					if (flag3)
					{
						this.parent.SelectedCharacterId = charId;
						this.parent.detailScroll.ReRender();
					}
					this.parent.assignPanel.Character = new ValueTuple<int, int>(charId, (int)displayData.RoleTemplateId);
					bool flag4 = displayData.ArrangementDisplayData.AreaId != -1;
					if (flag4)
					{
						bool flag5 = this.parent.SelectedRoleTemplateId != 5;
						if (flag5)
						{
							this.parent.OnClickArea(displayData.ArrangementDisplayData.AreaId);
						}
						else
						{
							ViewVillagerWork viewVillagerWork = this.parent;
							GuardingSwordTombDisplayData guard = displayData.ArrangementDisplayData.ArrangementData as GuardingSwordTombDisplayData;
							viewVillagerWork.SelectSwordTombByTombId((sbyte)((guard != null) ? ((int)guard.SwordTombId) : this.parent.assignPanel.areaId));
							this.parent.RefreshSwordTombs(true);
						}
					}
					else
					{
						this.parent.areaMap.SelectedAreaTemplateId = -1;
						this.parent.assignPanel.gameObject.SetActive(false);
						bool flag6 = this.parent.SelectedRoleTemplateId == 5;
						if (flag6)
						{
							this.parent.SelectSwordTombByTombId(-1);
						}
					}
				});
				this.avatar.Refresh(displayData.Avatar, displayData.Name.CharTemplateId);
				this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(ref displayData.Name, false, false);
				this.selected.gameObject.SetActive(this.isSelected);
				this.selected.sprite = (this.isSelected ? this.selectedSp : this.hoverSp);
				this.recallButton.gameObject.SetActive(displayData.ArrangementDisplayData.ArrangementTemplateId != -1);
				this.recallButton.ClearAndAddListener(delegate
				{
					this.parent.OnClickRecallButton(charId);
				});
				IVillagerRoleArrangementDisplayData arrangement = displayData.ArrangementDisplayData.ArrangementData;
				IntPair[] entity2;
				if (arrangement != null)
				{
					if (!true)
					{
					}
					HealingDisplayData healing = arrangement as HealingDisplayData;
					IntPair[] array;
					if (healing == null)
					{
						PeddlingDisplayData peddling = arrangement as PeddlingDisplayData;
						if (peddling == null)
						{
							EntertainingDisplayData entertaining = arrangement as EntertainingDisplayData;
							if (entertaining == null)
							{
								GuardingSwordTombDisplayData guardingSwordTomb = arrangement as GuardingSwordTombDisplayData;
								if (guardingSwordTomb == null)
								{
									TaiwuEnvoyDisplayData taiwuEnvoy = arrangement as TaiwuEnvoyDisplayData;
									if (taiwuEnvoy == null)
									{
										FarmerDisplayData farmerDisplayData = arrangement as FarmerDisplayData;
										if (farmerDisplayData == null)
										{
											array = Array.Empty<IntPair>();
										}
										else
										{
											array = new IntPair[]
											{
												new IntPair(0, farmerDisplayData.CollectResourceActionCount),
												new IntPair(2, farmerDisplayData.MigrateResourceBaseSuccessRate * farmerDisplayData.MigrateResourceSuccessRateBuildingBonus),
												new IntPair(3, farmerDisplayData.MigrateResourceSuccessRateBonus),
												new IntPair(4, farmerDisplayData.UpgradeBuildingCoreRate)
											};
										}
									}
									else
									{
										array = new IntPair[]
										{
											new IntPair(16, taiwuEnvoy.SpecialRuleCount),
											new IntPair(17, taiwuEnvoy.MonthlyAuthorityCost)
										};
									}
								}
								else
								{
									array = new IntPair[]
									{
										new IntPair(36, guardingSwordTomb.InformationGatheringSuccessRate),
										new IntPair(37, guardingSwordTomb.InjuryProbability),
										new IntPair(39, guardingSwordTomb.FeatureGainRateA),
										new IntPair(40, guardingSwordTomb.FeatureGainRateB),
										new IntPair(41, guardingSwordTomb.InfectionDecreaseRate)
									};
								}
							}
							else
							{
								array = new IntPair[]
								{
									new IntPair(21, entertaining.ActionEffectCount),
									new IntPair(22, entertaining.ActionEffectValue),
									new IntPair(23, entertaining.ExtraPeopleCount),
									new IntPair(24, entertaining.RelationChange)
								};
							}
						}
						else
						{
							array = new IntPair[]
							{
								new IntPair(25, (int)peddling.InteractTargetGrade),
								new IntPair(30, peddling.BuyPriceRate),
								new IntPair(31, peddling.SellPriceRate),
								new IntPair(32, peddling.AddFavorA),
								new IntPair(33, peddling.AddFavorB)
							};
						}
					}
					else
					{
						array = new IntPair[]
						{
							new IntPair(5, healing.InteractTargetGrade),
							new IntPair(10, healing.HealXiangshuInfectionAmount)
						};
					}
					if (!true)
					{
					}
					entity2 = array;
				}
				else
				{
					entity2 = Array.Empty<IntPair>();
				}
				IntPair[] entity = entity2;
				int lastItemIndex = entity.Length;
				bool flag2;
				do
				{
					int lastItemIndex2 = lastItemIndex;
					lastItemIndex = lastItemIndex2 - 1;
					if (lastItemIndex2 <= 0)
					{
						break;
					}
					flag2 = (chickenUnlocked || !VillagerWorkDetail.<Set>g__IsChicken|13_3(entity[lastItemIndex].First));
				}
				while (!flag2);
				this.items.Rebuild<VillagerWorkProperty>(entity.Length, delegate(VillagerWorkProperty item, int i)
				{
					IntPair unit = entity[i];
					VillagerRoleFormulaItem cfg = VillagerRoleFormula.Instance.GetItem(unit.First);
					int value = unit.Second;
					bool isChicken = VillagerWorkDetail.<Set>g__IsChicken|13_3(cfg.TemplateId);
					bool active = !isChicken | chickenUnlocked;
					item.gameObject.SetActive(active);
					bool flag3 = active;
					if (flag3)
					{
						string displayName = cfg.DisplayName;
						int templateId = cfg.TemplateId;
						if (!true)
						{
						}
						string valueText;
						if (templateId != 5)
						{
							if (templateId != 25)
							{
								valueText = string.Format(cfg.DisplayFormat, value);
							}
							else
							{
								valueText = LocalStringManager.Get(string.Format("LK_OrgGrade_Short_{0}", value)).SetGradeColor(value);
							}
						}
						else
						{
							valueText = LocalStringManager.Get(string.Format("LK_OrgGrade_Short_{0}", value)).SetGradeColor(value);
						}
						if (!true)
						{
						}
						item.Set(displayName, valueText, i < lastItemIndex);
					}
				});
				VillagerRoleUtils.RefreshWorkingIcon(this.workingIcon, charId, (short)displayData.ArrangementDisplayData.ArrangementTemplateId, displayData);
			}
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x0029DE48 File Offset: 0x0029C048
		public void OnPointerEnter(PointerEventData eventData)
		{
			bool flag = this.isSelected;
			if (!flag)
			{
				this.selected.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005A34 RID: 23092 RVA: 0x0029DE74 File Offset: 0x0029C074
		public void OnPointerExit(PointerEventData eventData)
		{
			bool flag = this.isSelected;
			if (!flag)
			{
				this.selected.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005A36 RID: 23094 RVA: 0x0029DEA9 File Offset: 0x0029C0A9
		[CompilerGenerated]
		internal static bool <Set>g__IsChicken|13_3(int id)
		{
			return id == 10 || id == 32 || id == 33 || id == 23 || id == 24 || id == 41 || id == 4;
		}

		// Token: 0x04003E18 RID: 15896
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003E19 RID: 15897
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x04003E1A RID: 15898
		[SerializeField]
		private ViewVillagerWork parent;

		// Token: 0x04003E1B RID: 15899
		[SerializeField]
		private CButton recallButton;

		// Token: 0x04003E1C RID: 15900
		[SerializeField]
		private CButton selfButton;

		// Token: 0x04003E1D RID: 15901
		[SerializeField]
		private CImage workingIcon;

		// Token: 0x04003E1E RID: 15902
		[SerializeField]
		private CImage selected;

		// Token: 0x04003E1F RID: 15903
		[SerializeField]
		private Sprite selectedSp;

		// Token: 0x04003E20 RID: 15904
		[SerializeField]
		private Sprite hoverSp;

		// Token: 0x04003E21 RID: 15905
		[SerializeField]
		private bool isSelected;

		// Token: 0x04003E22 RID: 15906
		[SerializeField]
		private TemplatedContainerAssemblyNew items;
	}
}
