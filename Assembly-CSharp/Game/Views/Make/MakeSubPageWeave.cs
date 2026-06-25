using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Building;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000959 RID: 2393
	public class MakeSubPageWeave : MakeSubPage
	{
		// Token: 0x17000CFD RID: 3325
		// (get) Token: 0x06007238 RID: 29240 RVA: 0x00351725 File Offset: 0x0034F925
		private ViewMake.ItemSourceTogKey CurTargetSourceTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.targetSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CFE RID: 3326
		// (get) Token: 0x06007239 RID: 29241 RVA: 0x00351732 File Offset: 0x0034F932
		private ItemSourceType CurTargetSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurTargetSourceTogKey);
			}
		}

		// Token: 0x0600723A RID: 29242 RVA: 0x0035173F File Offset: 0x0034F93F
		private List<ItemDisplayData> GetItemList(ItemSourceType source)
		{
			return this.ParentView.GetItemList(source);
		}

		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x0600723B RID: 29243 RVA: 0x0035174D File Offset: 0x0034F94D
		private ItemDisplayData CurrentTool
		{
			get
			{
				return this.ParentView.SelectedTool;
			}
		}

		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x0600723C RID: 29244 RVA: 0x0035175A File Offset: 0x0034F95A
		private AvatarData OriginAvatarData
		{
			get
			{
				return this.DisplayData.CharacterDisplayData.AvatarRelatedData.AvatarData;
			}
		}

		// Token: 0x17000D01 RID: 3329
		// (get) Token: 0x0600723D RID: 29245 RVA: 0x00351771 File Offset: 0x0034F971
		private sbyte CurLifeSkillType
		{
			get
			{
				return this.ParentView.CurLifeSkillType;
			}
		}

		// Token: 0x0600723E RID: 29246 RVA: 0x00351780 File Offset: 0x0034F980
		public override void Init(ViewMake parentView)
		{
			base.Init(parentView);
			this._inited = false;
			this._selectedTarget = null;
			this._selectedMaterial = -1;
			this._targetAvatarData = null;
			this._materialAvatarData = null;
			this.targetSlot.Set(null, 0, LanguageKey.LK_Making_Weave_Target.Tr(), false, null, 0, 0, null, null, -1);
			this.materialSlot.Set(null, 0, LanguageKey.LK_Making_Weave_Select_Material.Tr(), true, null, 0, 0, null, null, -1);
			this.toolSlot.Cancel();
			this.ShowToolPanel(false);
			this.materialListPanel.gameObject.SetActive(false);
			this.targetListPanel.gameObject.SetActive(false);
		}

		// Token: 0x0600723F RID: 29247 RVA: 0x00351830 File Offset: 0x0034FA30
		private void Awake()
		{
			this.targetListScroll.Init("ViewMakeWeaveTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnTargetItemRender), new Action<ITradeableContent, RowItemLine>(this.OnTargetItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.WeaveTemplate, null, null, null);
			this.targetListScroll.SetCustomBuildGroup(new Action(this.TargetBuildGroup), true);
			this.targetListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.targetListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.targetSourceToggleGroup.Init(-1);
			this.targetSourceToggleGroup.OnActiveIndexChange += this.OnTargetSourceToggleChange;
			this.materialListScroll.Init("ViewMakeWeaveTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnMaterialItemRender), new Action<ITradeableContent, RowItemLine>(this.OnMaterialItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.WeaveCount, null, null, null);
			this.materialListScroll.SetCustomBuildGroup(new Action(this.MaterialBuildGroup), true);
			this.materialListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.materialListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.materialSourceToggleGroup.Init(-1);
			this.materialSourceToggleGroup.OnActiveIndexChange += this.OnMaterialSourceToggleChange;
			this.toolSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.Tool, new Action<int, ItemDisplayData>(this.OnCancelTool), new Action(this.OnClickToolSlot), new Func<bool>(this.GetToolInteractable), new Action<bool>(this.OnAutoSelectToolToggleChanged), -1, null, false, null);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.confirmBtn.ClearAndAddListener(delegate
			{
				BuildingDomainMethod.AsyncCall.WeaveClothingItem(this.ParentView, this.CurrentTool, this._selectedTarget, this._selectedMaterial, delegate(int offset, RawDataPool dataPool)
				{
					ItemDisplayData resultItemDisplayData = new ItemDisplayData();
					Serializer.Deserialize(dataPool, offset, ref resultItemDisplayData);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("ItemList", new List<ItemDisplayData>
					{
						resultItemDisplayData
					});
					UIElement.GetItem.SetOnInitArgs(argBox);
					UIManager.Instance.MaskUI(UIElement.GetItem);
					this._selectedTarget.WeavedClothingTemplateId = resultItemDisplayData.WeavedClothingTemplateId;
					this.ParentView.RequestData();
				});
			});
			this._selectedTarget = null;
			this._selectedMaterial = -1;
			this.buttonCancelSelectMaterial.ClearAndAddListener(delegate
			{
				this.materialListPanel.gameObject.SetActive(false);
				this.ParentView.ShowToolPanel(false);
				this.ParentView.ExitFocusMode();
			});
			this.buttonCancelSelectTarget.ClearAndAddListener(delegate
			{
				this.targetListPanel.gameObject.SetActive(false);
				this.ParentView.ShowToolPanel(false);
				this.ParentView.ExitFocusMode();
			});
		}

		// Token: 0x06007240 RID: 29248 RVA: 0x00351A6C File Offset: 0x0034FC6C
		public void OnAutoSelectToolToggleChanged(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake = isOn;
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
			bool flag = this._selectedTarget != null && isOn;
			if (flag)
			{
				this.ParentView.AutoSelectTool();
			}
		}

		// Token: 0x06007241 RID: 29249 RVA: 0x00351AAC File Offset: 0x0034FCAC
		private void OnMaterialSourceToggleChange(int arg1, int arg2)
		{
		}

		// Token: 0x06007242 RID: 29250 RVA: 0x00351AAF File Offset: 0x0034FCAF
		private void OnTargetSourceToggleChange(int arg1, int arg2)
		{
			this.RefreshTargetList();
			this.targetListScroll.SetItemList(this._targetList);
		}

		// Token: 0x06007243 RID: 29251 RVA: 0x00351ACC File Offset: 0x0034FCCC
		private void OnEnable()
		{
			this._selectedTarget = null;
			this._selectedMaterial = -1;
			this.ParentView.ShowToolPanel(false);
			this.materialListPanel.gameObject.SetActive(false);
			this.targetListPanel.gameObject.SetActive(false);
		}

		// Token: 0x06007244 RID: 29252 RVA: 0x00351B1C File Offset: 0x0034FD1C
		private void ShowToolPanel(bool isShowTool = true)
		{
			bool flag = !this.IsTargetValid();
			if (flag)
			{
				this.ParentView.ClearToolList("");
			}
			else
			{
				sbyte lifeSkillType = this.CurLifeSkillType;
				this._toolDurabilityCost = this.CalculateToolDurabilityCost();
				this.ParentView.RefreshToolList(0, new List<sbyte>
				{
					lifeSkillType
				}, new List<List<sbyte>>
				{
					new List<sbyte>((int)this._selectedTarget.Grade)
				}, isShowTool ? this.CurrentTool : null, new Action<ItemDisplayData>(this.OnToolSelected), this.toolSlot.IsToggleOn, 1);
			}
		}

		// Token: 0x06007245 RID: 29253 RVA: 0x00351BB8 File Offset: 0x0034FDB8
		private void OnToolSelected(ItemDisplayData tool)
		{
			this.toolSlot.Select(this.CurrentTool, false);
			this.CheckCondition();
		}

		// Token: 0x06007246 RID: 29254 RVA: 0x00351BD8 File Offset: 0x0034FDD8
		private bool GetToolInteractable()
		{
			return this.IsTargetValid();
		}

		// Token: 0x06007247 RID: 29255 RVA: 0x00351BF0 File Offset: 0x0034FDF0
		private bool IsTargetValid()
		{
			return this._selectedTarget != null && this._selectedTarget.Key != ItemKey.Invalid;
		}

		// Token: 0x06007248 RID: 29256 RVA: 0x00351C12 File Offset: 0x0034FE12
		private bool IsMaterialValid()
		{
			return this._selectedMaterial != -1;
		}

		// Token: 0x06007249 RID: 29257 RVA: 0x00351C20 File Offset: 0x0034FE20
		private void OnClickToolSlot()
		{
		}

		// Token: 0x0600724A RID: 29258 RVA: 0x00351C23 File Offset: 0x0034FE23
		private void OnCancelTool(int index, ItemDisplayData itemDisplayData)
		{
		}

		// Token: 0x0600724B RID: 29259 RVA: 0x00351C28 File Offset: 0x0034FE28
		private void TargetBuildGroup()
		{
			string title = LanguageKey.LK_Making_Weave_Target_Available.Tr();
			this._availableTargetList.Clear();
			this._availableTargetList.AddRange(from d in this.targetListScroll.FilteredData
			where d.Interactable
			select d);
			this.targetListScroll.AddGroup(0, title, this._availableTargetList, null, true);
			string title2 = LanguageKey.LK_Making_Weave_Target_UnAvailable.Tr();
			this._notAvailableTargetList.Clear();
			this._notAvailableTargetList.AddRange(from d in this.targetListScroll.FilteredData
			where !d.Interactable
			select d);
			this.targetListScroll.AddGroup(1, title2, this._notAvailableTargetList, null, true);
		}

		// Token: 0x0600724C RID: 29260 RVA: 0x00351D0C File Offset: 0x0034FF0C
		private void MaterialBuildGroup()
		{
			string title = LanguageKey.LK_Making_Weave_Material_Available.Tr();
			this._availableMaterialList.Clear();
			this._availableMaterialList.AddRange(from d in this.materialListScroll.FilteredData
			where d.Interactable
			select d);
			this.materialListScroll.AddGroup(0, title, this._availableMaterialList, null, true);
			string title2 = LanguageKey.LK_Making_Weave_Material_UnAvailable.Tr();
			this._notAvailableMaterialList.Clear();
			this._notAvailableMaterialList.AddRange(from d in this.materialListScroll.FilteredData
			where !d.Interactable
			select d);
			this.materialListScroll.AddGroup(1, title2, this._notAvailableMaterialList, null, true);
		}

		// Token: 0x0600724D RID: 29261 RVA: 0x00351DF0 File Offset: 0x0034FFF0
		private void OnTargetItemRender(ITradeableContent content, RowItemLine line)
		{
			RowItemMain rowItemMain = line.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			line.Set(rowItemMain, true);
			bool isSelected = this._selectedTarget == content;
			line.SetSelected(isSelected);
			line.SetInteractable(true, true);
			rowItemMain.HideInteractionState();
		}

		// Token: 0x0600724E RID: 29262 RVA: 0x00351E38 File Offset: 0x00350038
		private void OnTargetItemClick(ITradeableContent content, RowItemLine line)
		{
			bool flag = this._selectedTarget == content;
			if (flag)
			{
				this._selectedTarget = null;
			}
			else
			{
				this._selectedTarget = (content as ItemDisplayData);
			}
			this.ParentView.ExitFocusMode();
			bool flag2 = this.IsTargetValid();
			if (flag2)
			{
				this.ParentView.ShowToolPanel(true);
			}
			else
			{
				this.ParentView.ShowToolPanel(false);
			}
			this.UpdateData();
		}

		// Token: 0x0600724F RID: 29263 RVA: 0x00351EA8 File Offset: 0x003500A8
		private unsafe void OnMaterialItemRender(ITradeableContent content, RowItemLine line)
		{
			bool flag = !this.materialListScroll.IsCardMode;
			if (flag)
			{
				RowItemMain rowItemMain = line.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(content);
				line.Set(rowItemMain, true);
				ItemDisplayData materialData = content as ItemDisplayData;
				bool isSelected = materialData != null && this._selectedMaterial == materialData.WeavedClothingTemplateId;
				line.SetSelected(isSelected);
				line.SetInteractable(true, true);
				rowItemMain.HideInteractionState();
			}
			else
			{
				WeaveClothItem weaveItem = line.GetComponent<WeaveClothItem>();
				line.Set(null, false);
				ItemDisplayData data = content as ItemDisplayData;
				AvatarData avatarData = new AvatarData(this.OriginAvatarData)
				{
					ClothDisplayId = Clothing.Instance[data.WeavedClothingTemplateId].DisplayId
				};
				ClothingItem clothingConfig = Clothing.Instance[data.WeavedClothingTemplateId];
				short requiredAttainment = this.ParentView.GetAttainmentByBuildingEffect(10, clothingConfig.WeaveNeedAttainment);
				short currentAttainment = *this.DisplayData.LifeSkillAttainments[10];
				weaveItem.Set(avatarData, data, data.SpecialArg, clothingConfig.Name, this.DisplayData.CharacterDisplayData.PhysiologicalAge, requiredAttainment, currentAttainment);
			}
		}

		// Token: 0x06007250 RID: 29264 RVA: 0x00351FCC File Offset: 0x003501CC
		private void OnMaterialItemClick(ITradeableContent content, RowItemLine line)
		{
			ItemDisplayData data = content as ItemDisplayData;
			bool flag = this._selectedMaterial == data.WeavedClothingTemplateId;
			if (flag)
			{
				this._selectedMaterial = -1;
				this._selectedTarget = null;
				this.ShowToolPanel(false);
				this.ParentView.ShowToolPanel(false);
				this.targetListPanel.gameObject.SetActive(false);
			}
			else
			{
				this._selectedMaterial = data.WeavedClothingTemplateId;
			}
			this.ParentView.ExitFocusMode();
			this.materialListPanel.gameObject.SetActive(false);
			bool flag2 = this.IsMaterialValid();
			if (flag2)
			{
				this.targetListPanel.gameObject.SetActive(true);
			}
			this.UpdateData();
		}

		// Token: 0x06007251 RID: 29265 RVA: 0x0035207C File Offset: 0x0035027C
		public override void Refresh(BuildingMakeDisplayData displayData)
		{
			base.Refresh(displayData);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.ParentView.RefreshSourceToggleInteractable(this.targetSourceToggleGroup);
			this.ParentView.RefreshSourceToggleInteractable(this.materialSourceToggleGroup);
			this.UpdateData();
		}

		// Token: 0x06007252 RID: 29266 RVA: 0x003520D4 File Offset: 0x003502D4
		private void UpdateData()
		{
			bool flag = !this._inited;
			if (flag)
			{
				this._inited = true;
				this._targetAvatarSetting = new ValueTuple<sbyte, sbyte>(this.OriginAvatarData.GetGender(), this.OriginAvatarData.GetBodyType());
				this._materialAvatarSetting = new ValueTuple<sbyte, sbyte>(this.OriginAvatarData.GetGender(), this.OriginAvatarData.GetBodyType());
			}
			bool flag2 = this.IsTargetValid();
			if (flag2)
			{
				this._targetAvatarData = new AvatarData(this.OriginAvatarData)
				{
					ClothDisplayId = Clothing.Instance[this._selectedTarget.WeavedClothingTemplateId].DisplayId
				};
				this._targetAvatarData.ChangeGender(this._targetAvatarSetting.Item1);
				this._targetAvatarData.ChangeBodyType(this._targetAvatarSetting.Item2);
			}
			else
			{
				this._targetAvatarData = null;
			}
			bool flag3 = this._selectedMaterial != -1;
			if (flag3)
			{
				this._materialAvatarData = new AvatarData(this.OriginAvatarData)
				{
					ClothDisplayId = Clothing.Instance[this._selectedMaterial].DisplayId
				};
				this._materialAvatarData.ChangeGender(this._materialAvatarSetting.Item1);
				this._materialAvatarData.ChangeBodyType(this._materialAvatarSetting.Item2);
			}
			else
			{
				this._materialAvatarData = null;
			}
			this.RefreshTargetList();
			this.RefreshMaterialList();
			this.UpdateViews();
		}

		// Token: 0x06007253 RID: 29267 RVA: 0x0035223C File Offset: 0x0035043C
		private void UpdateViews()
		{
			ItemDisplayData selectedTarget = this._selectedTarget;
			short targetTemplateId = (selectedTarget != null) ? selectedTarget.Key.TemplateId : -1;
			string targetName = (targetTemplateId != -1) ? Clothing.Instance[targetTemplateId].Name : LanguageKey.LK_Making_Weave_Target.Tr();
			this.targetSlot.Set(this._targetAvatarData, this.DisplayData.CharacterDisplayData.PhysiologicalAge, targetName, this.IsMaterialValid(), new Action(this.OnTargetSlotClicked), (int)(1 - this._targetAvatarSetting.Item1), (int)(2 - this._targetAvatarSetting.Item2), delegate(int newIndex, int oldIndex)
			{
				this._targetAvatarSetting.Item1 = (sbyte)(1 - newIndex);
				this.UpdateData();
			}, delegate(int newIndex, int oldIndex)
			{
				this._targetAvatarSetting.Item2 = (sbyte)(2 - newIndex);
				this.UpdateData();
			}, targetTemplateId);
			this.targetSlot.SetToFaceless();
			string materialName = (this._selectedMaterial != -1) ? Clothing.Instance[this._selectedMaterial].Name : LanguageKey.LK_Making_Weave_Select_Material.Tr();
			this.materialSlot.Set(this._materialAvatarData, this.DisplayData.CharacterDisplayData.PhysiologicalAge, materialName, true, new Action(this.OnMaterialSlotClicked), (int)(1 - this._materialAvatarSetting.Item1), (int)(2 - this._materialAvatarSetting.Item2), delegate(int newIndex, int oldIndex)
			{
				this._materialAvatarSetting.Item1 = (sbyte)(1 - newIndex);
				this.UpdateData();
			}, delegate(int newIndex, int oldIndex)
			{
				this._materialAvatarSetting.Item2 = (sbyte)(2 - newIndex);
				this.UpdateData();
			}, this._selectedMaterial);
			this.materialSlot.SetToFaceless();
			this.materialListScroll.SetItemList(this._materialList);
			bool flag = this.IsMaterialValid();
			if (flag)
			{
				this.targetListScroll.SetItemList(this._targetList);
			}
			else
			{
				this.targetListScroll.SetEmptyContent(string.Empty);
			}
			this.toolSlot.Select(this.CurrentTool, false);
			this.ShowToolPanel(true);
			this.requirementLabel.gameObject.SetActive(this._selectedMaterial != -1);
			this.CheckCondition();
		}

		// Token: 0x06007254 RID: 29268 RVA: 0x00352418 File Offset: 0x00350618
		private void OnMaterialSlotClicked()
		{
			bool flag = this.IsMaterialValid();
			if (flag)
			{
				this._selectedMaterial = -1;
				this._selectedTarget = null;
				this.ShowToolPanel(false);
				this.ParentView.ShowToolPanel(false);
				this.targetListPanel.gameObject.SetActive(false);
				this.UpdateData();
			}
			else
			{
				this.ShowMaterialList();
			}
		}

		// Token: 0x06007255 RID: 29269 RVA: 0x0035247C File Offset: 0x0035067C
		private void OnTargetSlotClicked()
		{
			bool flag = this._selectedTarget != null;
			if (flag)
			{
				this._selectedTarget = null;
				this.ShowToolPanel(false);
				this.ParentView.ShowToolPanel(false);
				this.UpdateData();
			}
			else
			{
				this.ShowTargetList();
			}
		}

		// Token: 0x06007256 RID: 29270 RVA: 0x003524C7 File Offset: 0x003506C7
		private void ShowMaterialList()
		{
			this.materialListPanel.gameObject.SetActive(true);
			this.ParentView.EnterFocusMode(this.materialListPanel, null);
		}

		// Token: 0x06007257 RID: 29271 RVA: 0x003524EF File Offset: 0x003506EF
		private void ShowTargetList()
		{
			this.targetListPanel.gameObject.SetActive(true);
		}

		// Token: 0x06007258 RID: 29272 RVA: 0x00352504 File Offset: 0x00350704
		private unsafe int CalculateMinToolAttainmentBonus()
		{
			bool flag = this._selectedTarget == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(this._selectedTarget.Key.ItemType, this._selectedTarget.Key.TemplateId);
				sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(this._selectedTarget.Key.ItemType, this._selectedTarget.Key.TemplateId);
				float factor = (this._selectedTarget.Durability == 0) ? 1f : 0.5f;
				short requiredAttainment = (short)((float)GlobalConfig.Instance.RepairAttainments[(int)grade] * factor);
				short currentAttainment = *this.DisplayData.LifeSkillAttainments[(int)lifeSkillType];
				int minToolAttainment = (int)(requiredAttainment - currentAttainment);
				result = Math.Max(0, minToolAttainment);
			}
			return result;
		}

		// Token: 0x06007259 RID: 29273 RVA: 0x003525C8 File Offset: 0x003507C8
		public void CheckCondition()
		{
			bool res = true;
			TooltipInvoker tip = this.confirmBtn.GetComponent<TooltipInvoker>();
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			this.confirmBtn.interactable = false;
			bool flag = this._selectedMaterial == -1;
			if (flag)
			{
				res = false;
				sb.AppendLine(LanguageKey.LK_Making_Weave_Material_UnSelected.Tr().SetColor("brightred"));
			}
			bool flag2 = this._selectedTarget == null;
			if (flag2)
			{
				res = false;
				sb.AppendLine(LanguageKey.LK_Making_Weave_Target_UnSelected.Tr().SetColor("brightred"));
			}
			bool flag3 = this.CurrentTool == null || this.IsEmptyTool(this.CurrentTool);
			if (flag3)
			{
				sb.AppendLine(LanguageKey.LK_Making_Weave_Tool_UnSelected.Tr().SetColor("brightred"));
			}
			bool flag4 = !this.CheckToolSufficient();
			if (flag4)
			{
				res = false;
			}
			tip.PresetParam[0] = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			tip.enabled = !res;
			this.confirmBtn.interactable = res;
		}

		// Token: 0x0600725A RID: 29274 RVA: 0x003526CC File Offset: 0x003508CC
		private bool CheckToolSufficient()
		{
			bool flag = this.CurrentTool == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.IsEmptyTool(this.CurrentTool);
				result = (flag2 || this.CurrentTool.Durability >= this._toolDurabilityCost);
			}
			return result;
		}

		// Token: 0x0600725B RID: 29275 RVA: 0x00352719 File Offset: 0x00350919
		private bool IsEmptyTool(ItemDisplayData data)
		{
			return data != null && this.IsEmptyTool(data.Key);
		}

		// Token: 0x0600725C RID: 29276 RVA: 0x0035272D File Offset: 0x0035092D
		private bool IsEmptyTool(ItemKey itemKey)
		{
			return ItemTemplateHelper.IsEmptyTool(itemKey.ItemType, itemKey.TemplateId);
		}

		// Token: 0x0600725D RID: 29277 RVA: 0x00352740 File Offset: 0x00350940
		private short CalculateToolDurabilityCost()
		{
			bool flag = this._selectedTarget == null || this.CurrentTool == null || this.IsEmptyTool(this.CurrentTool);
			short result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(this._selectedTarget.Key.ItemType, this._selectedTarget.Key.TemplateId);
				result = CraftTool.Instance[this.CurrentTool.Key.TemplateId].DurabilityCost[(int)grade];
			}
			return result;
		}

		// Token: 0x0600725E RID: 29278 RVA: 0x003527C0 File Offset: 0x003509C0
		private void RefreshTargetList()
		{
			List<ItemDisplayData> itemList = this.GetItemList(this.CurTargetSource) ?? new List<ItemDisplayData>();
			bool flag = this.CurTargetSource == ItemSourceType.Inventory;
			if (flag)
			{
				itemList = itemList.Concat(this.GetItemList(ItemSourceType.Equipment) ?? new List<ItemDisplayData>()).ToList<ItemDisplayData>();
			}
			this._targetList.Clear();
			this._targetList.AddRange(itemList.Where(new Func<ItemDisplayData, bool>(this.FilterWeaveItem)).ToList<ItemDisplayData>());
		}

		// Token: 0x0600725F RID: 29279 RVA: 0x0035283C File Offset: 0x00350A3C
		private bool FilterWeaveItem(ItemDisplayData d)
		{
			bool flag = !d.Key.IsValid();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = d.Key.ItemType != 3;
				result = !flag2;
			}
			return result;
		}

		// Token: 0x06007260 RID: 29280 RVA: 0x00352884 File Offset: 0x00350A84
		private void RefreshMaterialList()
		{
			List<short> clothingList = this.DisplayData.OwnedClothingList;
			this._materialList.Clear();
			using (List<short>.Enumerator enumerator = clothingList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					short itemTemplateId = enumerator.Current;
					ItemKey itemKey = new ItemKey(3, 0, itemTemplateId, -1);
					ItemDisplayData itemDisplayData = new ItemDisplayData();
					itemDisplayData.WeavedClothingTemplateId = itemTemplateId;
					itemDisplayData.SpecialArg = this._materialCountDict.GetValueOrDefault(itemTemplateId, 0);
					Dictionary<int, short> clothingDisplayModifications = this.DisplayData.ClothingDisplayModifications;
					itemDisplayData.Amount = ((clothingDisplayModifications != null) ? clothingDisplayModifications.Count((KeyValuePair<int, short> x) => x.Value == itemTemplateId) : 0);
					itemDisplayData.Key = itemKey;
					ItemDisplayData data = itemDisplayData;
					this._materialList.Add(data);
				}
			}
		}

		// Token: 0x040054A1 RID: 21665
		[SerializeField]
		private MakeTargetSlot toolSlot;

		// Token: 0x040054A2 RID: 21666
		[SerializeField]
		private WeaveTargetSlot targetSlot;

		// Token: 0x040054A3 RID: 21667
		[SerializeField]
		private WeaveTargetSlot materialSlot;

		// Token: 0x040054A4 RID: 21668
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x040054A5 RID: 21669
		[SerializeField]
		private RectTransform warning;

		// Token: 0x040054A6 RID: 21670
		[SerializeField]
		private TextMeshProUGUI requirementLabel;

		// Token: 0x040054A7 RID: 21671
		[SerializeField]
		private CToggleGroup targetSourceToggleGroup;

		// Token: 0x040054A8 RID: 21672
		[SerializeField]
		private ItemListScroll targetListScroll;

		// Token: 0x040054A9 RID: 21673
		[SerializeField]
		private CToggleGroup materialSourceToggleGroup;

		// Token: 0x040054AA RID: 21674
		[SerializeField]
		private ItemListScroll materialListScroll;

		// Token: 0x040054AB RID: 21675
		[SerializeField]
		private RectTransform targetListPanel;

		// Token: 0x040054AC RID: 21676
		[SerializeField]
		private RectTransform materialListPanel;

		// Token: 0x040054AD RID: 21677
		[SerializeField]
		private GameObject goLineToolToMaterialGrey;

		// Token: 0x040054AE RID: 21678
		[SerializeField]
		private GameObject goLineToolToMaterialGold;

		// Token: 0x040054AF RID: 21679
		[SerializeField]
		private GameObject goLineTargetToMaterialGrey;

		// Token: 0x040054B0 RID: 21680
		[SerializeField]
		private GameObject goLineTargetToMaterialGold;

		// Token: 0x040054B1 RID: 21681
		[SerializeField]
		private CButton buttonCancelSelectMaterial;

		// Token: 0x040054B2 RID: 21682
		[SerializeField]
		private CButton buttonCancelSelectTarget;

		// Token: 0x040054B3 RID: 21683
		private readonly List<ITradeableContent> _targetList = new List<ITradeableContent>();

		// Token: 0x040054B4 RID: 21684
		private readonly List<ITradeableContent> _availableTargetList = new List<ITradeableContent>();

		// Token: 0x040054B5 RID: 21685
		private readonly List<ITradeableContent> _notAvailableTargetList = new List<ITradeableContent>();

		// Token: 0x040054B6 RID: 21686
		private ItemDisplayData _selectedTarget;

		// Token: 0x040054B7 RID: 21687
		private AvatarData _targetAvatarData;

		// Token: 0x040054B8 RID: 21688
		[TupleElementNames(new string[]
		{
			"gender",
			"bodyType"
		})]
		private ValueTuple<sbyte, sbyte> _targetAvatarSetting;

		// Token: 0x040054B9 RID: 21689
		private readonly Dictionary<ItemKey, short> _targetModificationsDict = new Dictionary<ItemKey, short>();

		// Token: 0x040054BA RID: 21690
		private readonly List<ITradeableContent> _materialList = new List<ITradeableContent>();

		// Token: 0x040054BB RID: 21691
		private readonly List<ITradeableContent> _availableMaterialList = new List<ITradeableContent>();

		// Token: 0x040054BC RID: 21692
		private readonly List<ITradeableContent> _notAvailableMaterialList = new List<ITradeableContent>();

		// Token: 0x040054BD RID: 21693
		private short _selectedMaterial;

		// Token: 0x040054BE RID: 21694
		private AvatarData _materialAvatarData;

		// Token: 0x040054BF RID: 21695
		[TupleElementNames(new string[]
		{
			"gender",
			"bodyType"
		})]
		private ValueTuple<sbyte, sbyte> _materialAvatarSetting;

		// Token: 0x040054C0 RID: 21696
		private readonly Dictionary<short, int> _materialCountDict = new Dictionary<short, int>();

		// Token: 0x040054C1 RID: 21697
		private short _toolDurabilityCost;

		// Token: 0x040054C2 RID: 21698
		private bool _inited = false;
	}
}
