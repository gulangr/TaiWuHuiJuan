using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu.EquipCombatSkill
{
	// Token: 0x02000BBB RID: 3003
	public class EquipCombatSkillLine : MonoBehaviour
	{
		// Token: 0x17001033 RID: 4147
		// (get) Token: 0x06009742 RID: 38722 RVA: 0x00467FA1 File Offset: 0x004661A1
		public sbyte CombatSkillEquipType
		{
			get
			{
				return this.combatSkillEquipType;
			}
		}

		// Token: 0x17001034 RID: 4148
		// (get) Token: 0x06009743 RID: 38723 RVA: 0x00467FA9 File Offset: 0x004661A9
		private PoolItem SkillItemPool
		{
			get
			{
				return this._parentView.SkillItemPool;
			}
		}

		// Token: 0x17001035 RID: 4149
		// (get) Token: 0x06009744 RID: 38724 RVA: 0x00467FB6 File Offset: 0x004661B6
		private PoolItem EmptyGridPool
		{
			get
			{
				return this._parentView.EmptyGridPool;
			}
		}

		// Token: 0x17001036 RID: 4150
		// (get) Token: 0x06009745 RID: 38725 RVA: 0x00467FC3 File Offset: 0x004661C3
		private List<short> SkillIds
		{
			get
			{
				return this._parentView.GetEquippedSkills(this.combatSkillEquipType);
			}
		}

		// Token: 0x17001037 RID: 4151
		// (get) Token: 0x06009746 RID: 38726 RVA: 0x00467FD6 File Offset: 0x004661D6
		private sbyte CombatSkillSlotCount
		{
			get
			{
				return this._parentView.CombatSkillSlotCounts[(int)this.combatSkillEquipType];
			}
		}

		// Token: 0x17001038 RID: 4152
		// (get) Token: 0x06009747 RID: 38727 RVA: 0x00467FEA File Offset: 0x004661EA
		private byte GenericGridAllocatedCount
		{
			get
			{
				return (this.combatSkillEquipType != 0) ? this._parentView.GenericGridAllocation[(int)(this.combatSkillEquipType - 1)] : 0;
			}
		}

		// Token: 0x17001039 RID: 4153
		// (get) Token: 0x06009748 RID: 38728 RVA: 0x0046800C File Offset: 0x0046620C
		private sbyte CombatSkillSlotAllocatedCount
		{
			get
			{
				return this._parentView.CombatSkillSlotAllocation[(int)this.combatSkillEquipType];
			}
		}

		// Token: 0x1700103A RID: 4154
		// (get) Token: 0x06009749 RID: 38729 RVA: 0x00468020 File Offset: 0x00466220
		public sbyte TotalSlotCount
		{
			get
			{
				return this.CombatSkillSlotCount + (sbyte)this.GenericGridAllocatedCount;
			}
		}

		// Token: 0x1700103B RID: 4155
		// (get) Token: 0x0600974A RID: 38730 RVA: 0x00468030 File Offset: 0x00466230
		public sbyte EmptyGridCount
		{
			get
			{
				return this.TotalSlotCount - this.CombatSkillSlotAllocatedCount;
			}
		}

		// Token: 0x1700103C RID: 4156
		// (get) Token: 0x0600974B RID: 38731 RVA: 0x00468040 File Offset: 0x00466240
		public sbyte ExtraSpecificGridCount
		{
			get
			{
				return this._parentView.ExtraSpecificGridCount[(int)this.combatSkillEquipType];
			}
		}

		// Token: 0x0600974C RID: 38732 RVA: 0x00468054 File Offset: 0x00466254
		protected virtual void Awake()
		{
			this.addButton.ClearAndAddListener(delegate
			{
				this._parentView.AllocateGenericGrid(this.combatSkillEquipType);
				this.addButton.enabled = false;
				this.reduceButton.enabled = false;
			});
			this.reduceButton.ClearAndAddListener(delegate
			{
				this._parentView.DeallocateGenericGrid(this.combatSkillEquipType);
				this.addButton.enabled = false;
				this.reduceButton.enabled = false;
			});
		}

		// Token: 0x0600974D RID: 38733 RVA: 0x00468088 File Offset: 0x00466288
		private static void EnsurePoolItemUiDepthZero(Transform tr)
		{
			RectTransform rt = tr as RectTransform;
			bool flag = rt == null;
			if (!flag)
			{
				Vector3 lp = rt.localPosition;
				bool flag2 = Mathf.Abs(lp.z) < 1E-05f;
				if (!flag2)
				{
					rt.localPosition = new Vector3(lp.x, lp.y, 0f);
				}
			}
		}

		// Token: 0x0600974E RID: 38734 RVA: 0x004680E6 File Offset: 0x004662E6
		private static void ResetEmptyGridVisualForEquipLine(Transform emptyGridTransform)
		{
			emptyGridTransform.GetChild(1).gameObject.SetActive(true);
			emptyGridTransform.GetChild(2).gameObject.SetActive(false);
		}

		// Token: 0x0600974F RID: 38735 RVA: 0x00468110 File Offset: 0x00466310
		public virtual void Set(ViewCharacterMenuEquipCombatSkill parentView)
		{
			this.addButton.enabled = true;
			this.reduceButton.enabled = true;
			this._parentView = parentView;
			this.addButton.interactable = (this._parentView.availableGenericGridCount > 0 && this.TotalSlotCount - this.ExtraSpecificGridCount < GameData.Domains.Character.CombatSkillHelper.MaxSlotCounts[(int)this.combatSkillEquipType] && this._parentView.CanOperate);
			this.reduceButton.interactable = (this.GenericGridAllocatedCount > 0 && this._parentView.CanOperate);
			this.UpdateTypeGrid();
			this.UpdateSkillItems();
			this.UpdateEmptyGrids();
			this.UpdateBtnsMousetip();
			this.UpdateEquipTypeMousetip();
		}

		// Token: 0x06009750 RID: 38736 RVA: 0x004681C8 File Offset: 0x004663C8
		private void UpdateSkillItems()
		{
			int skillCount = 0;
			foreach (short skillId in this.SkillIds)
			{
				bool flag = skillId >= 0;
				if (flag)
				{
					skillCount++;
				}
			}
			this.skillRoot.gameObject.SetActive(skillCount > 0);
			while (this.skillRoot.childCount < skillCount)
			{
				GameObject obj = this.SkillItemPool.GetObject();
				obj.transform.SetParent(this.skillRoot, false);
				EquipCombatSkillLine.EnsurePoolItemUiDepthZero(obj.transform);
			}
			int slotCount = 0;
			for (int i = 0; i < this.skillRoot.childCount; i++)
			{
				GameObject obj2 = this.skillRoot.GetChild(i).gameObject;
				bool flag2 = i >= skillCount;
				if (flag2)
				{
					this.SkillItemPool.DestroyObject(obj2);
				}
				else
				{
					bool flag3 = !obj2.activeSelf;
					if (flag3)
					{
						int instanceId = obj2.GetInstanceID();
						PoolItemTime poolItem;
						bool flag4 = this.SkillItemPool.objectList.TryGetValue(instanceId, out poolItem);
						if (flag4)
						{
							poolItem.Active();
						}
						else
						{
							Debug.LogWarning("出现不属于对象池的Obj");
						}
					}
					EquipCombatSkillLine.EnsurePoolItemUiDepthZero(obj2.transform);
					EquipCombatSkillItem equipCombatSkillItem = obj2.GetComponent<EquipCombatSkillItem>();
					CombatSkillDisplayDataCharacterMenuListItem skillData = this._parentView.EquippedSkillDataDict[this.SkillIds[i]];
					slotCount += (int)skillData.GridCount;
					equipCombatSkillItem.combatSkillItem.SetByCharacterMenuList(skillData);
					equipCombatSkillItem.Set(this._parentView, i, skillData, EEquipCombatSkillItemType.EquippedLine, slotCount <= (int)this.TotalSlotCount);
					this.BindSkillItemToLine(equipCombatSkillItem);
				}
			}
		}

		// Token: 0x06009751 RID: 38737 RVA: 0x004683B0 File Offset: 0x004665B0
		protected virtual void BindSkillItemToLine(EquipCombatSkillItem equipCombatSkillItem)
		{
		}

		// Token: 0x06009752 RID: 38738 RVA: 0x004683B4 File Offset: 0x004665B4
		protected virtual void OnLayoutSlotClicked()
		{
			bool flag = this._parentView == null;
			if (!flag)
			{
				this._parentView.IsInEditingMode = true;
				this._parentView.SetCombatSkillFilterType(this.combatSkillEquipType);
			}
		}

		// Token: 0x06009753 RID: 38739 RVA: 0x004683F4 File Offset: 0x004665F4
		private void UpdateEmptyGrids()
		{
			sbyte emptyGridCount = this.EmptyGridCount;
			while (this.emptyGridRoot.childCount < (int)emptyGridCount)
			{
				GameObject obj = this.EmptyGridPool.GetObject();
				obj.transform.SetParent(this.emptyGridRoot, false);
				EquipCombatSkillLine.EnsurePoolItemUiDepthZero(obj.transform);
			}
			for (int i = 0; i < this.emptyGridRoot.childCount; i++)
			{
				GameObject obj2 = this.emptyGridRoot.GetChild(i).gameObject;
				bool flag = i >= (int)emptyGridCount;
				if (flag)
				{
					this.EmptyGridPool.DestroyObject(obj2);
				}
				else
				{
					bool flag2 = !obj2.activeSelf;
					if (flag2)
					{
						int instanceId = obj2.GetInstanceID();
						PoolItemTime poolItem;
						bool flag3 = this.EmptyGridPool.objectList.TryGetValue(instanceId, out poolItem);
						if (flag3)
						{
							poolItem.Active();
						}
						else
						{
							Debug.LogWarning("出现不属于对象池的Obj");
						}
					}
					EquipCombatSkillLine.EnsurePoolItemUiDepthZero(obj2.transform);
					EquipCombatSkillLine.ResetEmptyGridVisualForEquipLine(obj2.transform);
					CButton btn = obj2.GetComponent<CButton>();
					btn.enabled = this._parentView.CanOperate;
					btn.ClearAndAddListener(new Action(this.OnLayoutSlotClicked));
					PointerTrigger pointerTrigger = obj2.GetComponent<PointerTrigger>();
					pointerTrigger.enabled = this._parentView.CanOperate;
					GameObject hover = obj2.transform.GetChild(2).gameObject;
					GameObject skillLock = obj2.transform.GetChild(3).gameObject;
					skillLock.SetActive(!this._parentView.CanOperate);
					pointerTrigger.EnterEvent.RemoveAllListeners();
					pointerTrigger.ExitEvent.RemoveAllListeners();
					this.BindEmptyGridPointerHover(pointerTrigger, hover);
					this.OnEmptyGridBound(obj2, pointerTrigger);
				}
			}
		}

		// Token: 0x06009754 RID: 38740 RVA: 0x004685C8 File Offset: 0x004667C8
		protected virtual void BindEmptyGridPointerHover(PointerTrigger pointerTrigger, GameObject hover)
		{
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				hover.SetActive(true);
			});
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				hover.SetActive(false);
			});
		}

		// Token: 0x06009755 RID: 38741 RVA: 0x00468613 File Offset: 0x00466813
		protected virtual void OnEmptyGridBound(GameObject obj, PointerTrigger pointerTrigger)
		{
		}

		// Token: 0x06009756 RID: 38742 RVA: 0x00468618 File Offset: 0x00466818
		private void UpdateTypeGrid()
		{
			this.currentValue.text = this.CombatSkillSlotAllocatedCount.ToString();
			this.limit.text = string.Format("/{0}", Math.Max((int)(this.CombatSkillSlotCount + (sbyte)this.GenericGridAllocatedCount), 0));
		}

		// Token: 0x06009757 RID: 38743 RVA: 0x00468670 File Offset: 0x00466870
		private void UpdateBtnsMousetip()
		{
			TooltipInvoker addButtonTip = this.addButton.GetComponent<TooltipInvoker>();
			addButtonTip.PresetParam[0] = "<SpName=ui9_icon_attainments_universal>-1";
			TooltipInvoker reduceButtonTip = this.reduceButton.GetComponent<TooltipInvoker>();
			reduceButtonTip.PresetParam[0] = string.Format("<SpName={0}>+{1}", "ui9_icon_attainments_universal", (this.GenericGridAllocatedCount == 0) ? 0 : 1);
		}

		// Token: 0x06009758 RID: 38744 RVA: 0x004686CC File Offset: 0x004668CC
		private void UpdateEquipTypeMousetip()
		{
			string typeName = LocalStringManager.Get("LK_CombatSkill_EquipType_" + this.combatSkillEquipType.ToString());
			this.equipTypeMouseTip.PresetParam[0] = typeName;
			this.equipTypeMouseTip.PresetParam[1] = LanguageKey.LK_EquipCombatSkill_CombatSkill_SkillLine.TrFormat(this.CombatSkillSlotAllocatedCount, this.TotalSlotCount, typeName);
		}

		// Token: 0x04007408 RID: 29704
		[SerializeField]
		[Range(0f, 4f)]
		[Header("功法装备类型")]
		private sbyte combatSkillEquipType;

		// Token: 0x04007409 RID: 29705
		[SerializeField]
		private TextMeshProUGUI currentValue;

		// Token: 0x0400740A RID: 29706
		[SerializeField]
		private TextMeshProUGUI limit;

		// Token: 0x0400740B RID: 29707
		[SerializeField]
		private CButton addButton;

		// Token: 0x0400740C RID: 29708
		[SerializeField]
		private CButton reduceButton;

		// Token: 0x0400740D RID: 29709
		[SerializeField]
		protected RectTransform skillRoot;

		// Token: 0x0400740E RID: 29710
		[SerializeField]
		protected RectTransform emptyGridRoot;

		// Token: 0x0400740F RID: 29711
		[SerializeField]
		private RectTransform preview;

		// Token: 0x04007410 RID: 29712
		[SerializeField]
		private CScrollRect skillScroll;

		// Token: 0x04007411 RID: 29713
		[SerializeField]
		private TooltipInvoker equipTypeMouseTip;

		// Token: 0x04007412 RID: 29714
		protected ViewCharacterMenuEquipCombatSkill _parentView;
	}
}
