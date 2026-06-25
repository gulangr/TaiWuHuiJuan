using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B74 RID: 2932
	public class EquipCombatSkillGroupedSkillScroll : MonoBehaviour
	{
		// Token: 0x060090E7 RID: 37095 RVA: 0x00438E61 File Offset: 0x00437061
		private void Awake()
		{
			this.TryInitScrollView();
		}

		// Token: 0x060090E8 RID: 37096 RVA: 0x00438E6B File Offset: 0x0043706B
		private void OnEnable()
		{
			this.TryInitScrollView();
			this.TryRefreshScrollView();
		}

		// Token: 0x060090E9 RID: 37097 RVA: 0x00438E7C File Offset: 0x0043707C
		private void OnDisable()
		{
			bool flag = this._pendingRefreshCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._pendingRefreshCoroutine);
				this._pendingRefreshCoroutine = null;
			}
		}

		// Token: 0x060090EA RID: 37098 RVA: 0x00438EB0 File Offset: 0x004370B0
		private void OnDestroy()
		{
			bool flag = this.scrollView != null;
			if (flag)
			{
				this.scrollView.StopLoop();
			}
		}

		// Token: 0x060090EB RID: 37099 RVA: 0x00438EDC File Offset: 0x004370DC
		public void Set(ViewCharacterMenuEquipCombatSkill parentView, List<CombatSkillDisplayDataCharacterMenuListItem> equippedDataList, List<CombatSkillDisplayDataCharacterMenuListItem> unEquippedDataList, bool reset)
		{
			this._parentView = parentView;
			this._equippedDataList = (equippedDataList ?? new List<CombatSkillDisplayDataCharacterMenuListItem>());
			this._unEquippedDataList = (unEquippedDataList ?? new List<CombatSkillDisplayDataCharacterMenuListItem>());
			bool flag = this.scrollView == null;
			if (!flag)
			{
				this.BuildFlatList();
				this.QueueRefreshScrollView(reset || !this._hasInitLoop);
			}
		}

		// Token: 0x060090EC RID: 37100 RVA: 0x00438F44 File Offset: 0x00437144
		private void TryInitScrollView()
		{
			bool flag = this.scrollView == null;
			if (!flag)
			{
				bool flag2 = this.itemTemplate == null;
				if (!flag2)
				{
					bool activeSelf = this.itemTemplate.gameObject.activeSelf;
					if (activeSelf)
					{
						this.itemTemplate.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060090ED RID: 37101 RVA: 0x00438F9C File Offset: 0x0043719C
		private void BuildFlatList()
		{
			this._flatSkillList.Clear();
			this._flatIsEquipped.Clear();
			bool flag = this._equippedDataList != null;
			if (flag)
			{
				for (int i = 0; i < this._equippedDataList.Count; i++)
				{
					this._flatSkillList.Add(this._equippedDataList[i]);
					this._flatIsEquipped.Add(true);
				}
			}
			bool flag2 = this._unEquippedDataList != null;
			if (flag2)
			{
				for (int j = 0; j < this._unEquippedDataList.Count; j++)
				{
					this._flatSkillList.Add(this._unEquippedDataList[j]);
					this._flatIsEquipped.Add(false);
				}
			}
		}

		// Token: 0x060090EE RID: 37102 RVA: 0x00439069 File Offset: 0x00437269
		private void QueueRefreshScrollView(bool reset)
		{
			this._hasPendingRefresh = true;
			this._pendingReset = (this._pendingReset || reset);
			this.TryRefreshScrollView();
		}

		// Token: 0x060090EF RID: 37103 RVA: 0x00439088 File Offset: 0x00437288
		private void TryRefreshScrollView()
		{
			bool flag = !this._hasPendingRefresh;
			if (!flag)
			{
				bool flag2 = this.scrollView == null;
				if (!flag2)
				{
					bool flag3 = this.itemTemplate == null;
					if (!flag3)
					{
						bool flag4 = !base.isActiveAndEnabled || !base.gameObject.activeInHierarchy || !this.scrollView.isActiveAndEnabled;
						if (!flag4)
						{
							bool flag5 = EquipCombatSkillGroupedSkillScroll.IsCanvasRebuilding();
							if (flag5)
							{
								bool flag6 = this._pendingRefreshCoroutine == null;
								if (flag6)
								{
									this._pendingRefreshCoroutine = base.StartCoroutine(this.DelayedRefreshScrollView());
								}
							}
							else
							{
								bool reset = this._pendingReset;
								this._hasPendingRefresh = false;
								this._pendingReset = false;
								int count = this._flatSkillList.Count;
								bool flag7 = !this._hasInitLoop;
								if (flag7)
								{
									this.scrollView.InitLoop(this.itemTemplate.gameObject, count, new Action<Transform, int>(this.OnRenderItem), null);
									this._hasInitLoop = true;
								}
								else
								{
									this.scrollView.totalCount = count;
									bool flag8 = reset;
									if (flag8)
									{
										this.scrollView.RefillCells(0, false);
									}
									else
									{
										this.scrollView.RefillCellsAtCurrentPosition();
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060090F0 RID: 37104 RVA: 0x004391C8 File Offset: 0x004373C8
		private static bool IsCanvasRebuilding()
		{
			return CanvasUpdateRegistry.IsRebuildingLayout() || CanvasUpdateRegistry.IsRebuildingGraphics();
		}

		// Token: 0x060090F1 RID: 37105 RVA: 0x004391E9 File Offset: 0x004373E9
		private IEnumerator DelayedRefreshScrollView()
		{
			yield return null;
			while (EquipCombatSkillGroupedSkillScroll.IsCanvasRebuilding())
			{
				yield return null;
			}
			this._pendingRefreshCoroutine = null;
			this.TryRefreshScrollView();
			yield break;
		}

		// Token: 0x060090F2 RID: 37106 RVA: 0x004391F8 File Offset: 0x004373F8
		private void OnRenderItem(Transform obj, int index)
		{
			bool flag = index < 0 || index >= this._flatSkillList.Count;
			if (!flag)
			{
				CombatSkillDisplayDataCharacterMenuListItem data = this._flatSkillList[index];
				bool isEquipped = this._flatIsEquipped[index];
				EquipCombatSkillItem equipItem = obj.GetComponent<EquipCombatSkillItem>() ?? obj.GetComponentInChildren<EquipCombatSkillItem>(true);
				CharacterMenuCombatSkillItem skillItem = (equipItem != null && equipItem.combatSkillItem != null) ? equipItem.combatSkillItem : (obj.GetComponent<CharacterMenuCombatSkillItem>() ?? obj.GetComponentInChildren<CharacterMenuCombatSkillItem>(true));
				bool flag2 = skillItem == null;
				if (!flag2)
				{
					skillItem.SetByCharacterMenuList(data);
					skillItem.SetEquipCombatSkillRunningMarkVisible(isEquipped);
					bool flag3 = equipItem != null && this._parentView != null;
					if (flag3)
					{
						bool flag4 = !this._parentView.IsInFavoriteMode;
						if (flag4)
						{
							equipItem.Set(this._parentView, index, data, isEquipped ? EEquipCombatSkillItemType.ScrollEquipped : EEquipCombatSkillItemType.ScrollUnEquipped, true);
						}
						else
						{
							equipItem.Set(this._parentView, index, data, EEquipCombatSkillItemType.FavoriteMode, true);
						}
					}
				}
			}
		}

		// Token: 0x04006FA2 RID: 28578
		[SerializeField]
		private LoopVerticalScrollRect scrollView;

		// Token: 0x04006FA3 RID: 28579
		[SerializeField]
		private CharacterMenuCombatSkillItem itemTemplate;

		// Token: 0x04006FA4 RID: 28580
		private ViewCharacterMenuEquipCombatSkill _parentView;

		// Token: 0x04006FA5 RID: 28581
		private List<CombatSkillDisplayDataCharacterMenuListItem> _equippedDataList;

		// Token: 0x04006FA6 RID: 28582
		private List<CombatSkillDisplayDataCharacterMenuListItem> _unEquippedDataList;

		// Token: 0x04006FA7 RID: 28583
		private readonly List<CombatSkillDisplayDataCharacterMenuListItem> _flatSkillList = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x04006FA8 RID: 28584
		private readonly List<bool> _flatIsEquipped = new List<bool>();

		// Token: 0x04006FA9 RID: 28585
		private bool _hasInitLoop;

		// Token: 0x04006FAA RID: 28586
		private bool _hasPendingRefresh;

		// Token: 0x04006FAB RID: 28587
		private bool _pendingReset;

		// Token: 0x04006FAC RID: 28588
		private Coroutine _pendingRefreshCoroutine;
	}
}
