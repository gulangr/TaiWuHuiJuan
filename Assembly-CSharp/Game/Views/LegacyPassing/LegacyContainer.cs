using System;
using System.Collections.Generic;
using Config;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000990 RID: 2448
	public class LegacyContainer : MonoBehaviour
	{
		// Token: 0x06007606 RID: 30214 RVA: 0x00370960 File Offset: 0x0036EB60
		public LegacyItem Instantiate()
		{
			LegacyItem item = Object.Instantiate<LegacyItem>(this.itemPrefab, this.itemContainer, false);
			item.parent = this;
			return item;
		}

		// Token: 0x06007607 RID: 30215 RVA: 0x00370990 File Offset: 0x0036EB90
		public void Destroy(LegacyItem item)
		{
			bool flag = item;
			if (flag)
			{
				Object.Destroy(item.gameObject);
			}
		}

		// Token: 0x17000D47 RID: 3399
		// (get) Token: 0x06007608 RID: 30216 RVA: 0x003709B4 File Offset: 0x0036EBB4
		public int FreeCount
		{
			get
			{
				return this._freeLegacies.Count;
			}
		}

		// Token: 0x06007609 RID: 30217 RVA: 0x003709C1 File Offset: 0x0036EBC1
		public void AddExtraLegacy(short templateId)
		{
			this._freeLegacies.Add(templateId);
			this.Instantiate().Set(templateId, true, true);
		}

		// Token: 0x0600760A RID: 30218 RVA: 0x003709E0 File Offset: 0x0036EBE0
		public void OnSubmit()
		{
			this._freeLegacies.Clear();
		}

		// Token: 0x0600760B RID: 30219 RVA: 0x003709F0 File Offset: 0x0036EBF0
		public void RefreshItems(IEnumerable<short> itemTemplateIds, bool canSelect)
		{
			this._canSelect = canSelect;
			int idx = this.itemContainer.childCount;
			while (idx-- > 0)
			{
				Object.Destroy(this.itemContainer.GetChild(idx).gameObject);
			}
			bool flag = itemTemplateIds == null;
			if (!flag)
			{
				foreach (short item in itemTemplateIds)
				{
					this.Instantiate().Set(item, false, canSelect);
				}
				foreach (short item2 in this._freeLegacies)
				{
					this.Instantiate().Set(item2, true, canSelect);
				}
				GEvent.OnEvent(UiEvents.RequestLegacyItemRefresh, null);
			}
		}

		// Token: 0x0600760C RID: 30220 RVA: 0x00370AEC File Offset: 0x0036ECEC
		public void RefreshItemsForFree(IEnumerable<short> itemTemplateIds, bool canSelect)
		{
			this._canSelect = canSelect;
			int idx = this.itemContainer.childCount;
			while (idx-- > 0)
			{
				Object.Destroy(this.itemContainer.GetChild(idx).gameObject);
			}
			bool flag = itemTemplateIds == null;
			if (!flag)
			{
				foreach (short item in itemTemplateIds)
				{
					this.Instantiate().Set(item, true, canSelect);
				}
				GEvent.OnEvent(UiEvents.RequestLegacyItemRefresh, null);
			}
		}

		// Token: 0x0600760D RID: 30221 RVA: 0x00370B94 File Offset: 0x0036ED94
		public void RefreshInteractable(int now)
		{
			bool flag = !this._canSelect;
			if (!flag)
			{
				int idx = this.itemContainer.childCount;
				while (idx-- > 0)
				{
					LegacyItem item = this.itemContainer.GetChild(idx).GetComponent<LegacyItem>();
					bool flag2 = item != null;
					if (flag2)
					{
						LegacyItem legacyItem = item;
						ConflictType conflictType = item.ConflictType;
						legacyItem.Interactable = (((conflictType == ConflictType.None || conflictType == ConflictType.HigherLevel) && now >= (item.IsFree ? 0 : item.Cost)) || item.SelectedIsOn);
					}
				}
			}
		}

		// Token: 0x0600760E RID: 30222 RVA: 0x00370C1C File Offset: 0x0036EE1C
		public void DeselectAllConflictLegacies(short templateId)
		{
			int idx = this.itemContainer.childCount;
			while (idx-- > 0)
			{
				LegacyItem item = this.itemContainer.GetChild(idx).GetComponent<LegacyItem>();
				bool flag = item != null && item.SelectedIsOn;
				if (flag)
				{
					this.DeselectConflictLegacies(item, templateId);
				}
			}
		}

		// Token: 0x0600760F RID: 30223 RVA: 0x00370C70 File Offset: 0x0036EE70
		public ValueTuple<List<short>, List<short>> GetSelected()
		{
			List<short> normalFeature = new List<short>();
			List<short> freeFeature = new List<short>();
			int idx = this.itemContainer.childCount;
			while (idx-- > 0)
			{
				LegacyItem item = this.itemContainer.GetChild(idx).GetComponent<LegacyItem>();
				bool flag = item != null && item.SelectedIsOn;
				if (flag)
				{
					(item.IsFree ? freeFeature : normalFeature).Add(item.TemplateId);
				}
			}
			return new ValueTuple<List<short>, List<short>>(normalFeature, freeFeature);
		}

		// Token: 0x06007610 RID: 30224 RVA: 0x00370CF0 File Offset: 0x0036EEF0
		private void DeselectConflictLegacies(LegacyItem item, short legacyTemplateId)
		{
			bool flag = item.TemplateId == legacyTemplateId;
			if (!flag)
			{
				short currTemplateId = item.TemplateId;
				bool flag2 = LegacyContainer.CheckLegacyConflict(legacyTemplateId, currTemplateId);
				if (flag2)
				{
					item.SelectedIsOn = false;
				}
			}
		}

		// Token: 0x06007611 RID: 30225 RVA: 0x00370D28 File Offset: 0x0036EF28
		public static bool CheckLegacyConflict(short templateIdA, short templateIdB)
		{
			LegacyItem configA = Legacy.Instance[templateIdA];
			LegacyItem configB = Legacy.Instance[templateIdB];
			bool flag = configA.GroupId >= 0 && configA.GroupId == configB.GroupId;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = configA.AddFeature >= 0 && configB.AddFeature >= 0;
				if (flag2)
				{
					bool flag3 = configA.AddFeature == configB.AddFeature;
					if (flag3)
					{
						result = true;
					}
					else
					{
						short mutexGroupA = CharacterFeature.Instance[configA.AddFeature].MutexGroupId;
						short mutexGroupB = CharacterFeature.Instance[configB.AddFeature].MutexGroupId;
						result = (mutexGroupA == mutexGroupB && mutexGroupA >= 0);
					}
				}
				else
				{
					bool flag4 = configA.TargetBehaviorType >= 0 && configB.TargetBehaviorType >= 0;
					result = flag4;
				}
			}
			return result;
		}

		// Token: 0x040058DB RID: 22747
		[SerializeField]
		private LegacyItem itemPrefab;

		// Token: 0x040058DC RID: 22748
		[SerializeField]
		private RectTransform itemContainer;

		// Token: 0x040058DD RID: 22749
		private List<short> _freeLegacies = new List<short>();

		// Token: 0x040058DE RID: 22750
		private bool _canSelect;

		// Token: 0x040058DF RID: 22751
		public Action<LegacyItem, bool> Selected = delegate(LegacyItem _, bool __)
		{
		};

		// Token: 0x040058E0 RID: 22752
		public Action<LegacyItem> Set = delegate(LegacyItem item)
		{
			item.tipDisplayer.PresetParam[0] = Legacy.Instance[item.id].Desc;
		};
	}
}
