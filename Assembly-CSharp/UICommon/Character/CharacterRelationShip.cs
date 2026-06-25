using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005E6 RID: 1510
	public class CharacterRelationShip : CharacterUIElement
	{
		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x0600474B RID: 18251 RVA: 0x00216518 File Offset: 0x00214718
		private CharacterRelationMonitor Item
		{
			get
			{
				return this.MonitorDataItem as CharacterRelationMonitor;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x0600474C RID: 18252 RVA: 0x00216525 File Offset: 0x00214725
		// (set) Token: 0x0600474D RID: 18253 RVA: 0x0021652D File Offset: 0x0021472D
		public int RelationShowedCount { get; private set; }

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x0600474E RID: 18254 RVA: 0x00216536 File Offset: 0x00214736
		public bool HasRelationShowed
		{
			get
			{
				return this.RelationShowedCount > 0;
			}
		}

		// Token: 0x0600474F RID: 18255 RVA: 0x00216544 File Offset: 0x00214744
		public CharacterRelationShip(Refers refers)
		{
			this._relationItemPool = new PoolItem(string.Empty, refers.CGet<GameObject>("RelationItem"));
			this.LayoutRoot = refers.CGet<RectTransform>("LayoutBack");
			this.RelationShowedCount = 0;
		}

		// Token: 0x06004750 RID: 18256 RVA: 0x0021670C File Offset: 0x0021490C
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<CharacterRelationMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004751 RID: 18257 RVA: 0x0021672F File Offset: 0x0021492F
		internal override void BindEvent()
		{
			this.Item.AddRelationShipListener(new Action(this.FillElement));
		}

		// Token: 0x06004752 RID: 18258 RVA: 0x0021674B File Offset: 0x0021494B
		public override void UnbindEvent()
		{
			this.Item.RemoveRelationShipListener(new Action(this.FillElement));
		}

		// Token: 0x06004753 RID: 18259 RVA: 0x00216768 File Offset: 0x00214968
		public override void FillElement()
		{
			this.ResetToEmpty();
			this.RelationShowedCount = 0;
			bool flag = this.Item.RelationBitFlag != ushort.MaxValue;
			if (flag)
			{
				int i = 0;
				int max = this._relationTypeSortedTypeList.Count;
				while (i < max)
				{
					bool flag2 = (this._relationTypeSortedTypeList[i] & this.Item.RelationBitFlag) == 0;
					if (!flag2)
					{
						int relationShowedCount = this.RelationShowedCount;
						this.RelationShowedCount = relationShowedCount + 1;
						bool flag3 = !this._relationTypeSortedDisplayKeyList.CheckIndex(i);
						if (flag3)
						{
							throw new Exception("_relationTypeSortedTypeList.Count != _relationTypeSortedDisplayKeyList.Count!");
						}
						string displayContent = LocalStringManager.Get(this._relationTypeSortedDisplayKeyList[i]);
						GameObject item = this._relationItemPool.GetObject();
						item.transform.SetParent(this.LayoutRoot, false);
						item.GetComponentInChildren<TextMeshProUGUI>().text = displayContent;
					}
					i++;
				}
			}
		}

		// Token: 0x06004754 RID: 18260 RVA: 0x00216860 File Offset: 0x00214A60
		public override void ResetToEmpty()
		{
			this.RelationShowedCount = 0;
			bool flag = null != this.LayoutRoot;
			if (flag)
			{
				foreach (object obj in this.LayoutRoot)
				{
					Transform child = (Transform)obj;
					bool activeSelf = child.gameObject.activeSelf;
					if (activeSelf)
					{
						this._relationItemPool.DestroyObject(child.gameObject);
					}
				}
			}
		}

		// Token: 0x0400313E RID: 12606
		public RectTransform LayoutRoot;

		// Token: 0x04003140 RID: 12608
		private PoolItem _relationItemPool;

		// Token: 0x04003141 RID: 12609
		private readonly List<ushort> _relationTypeSortedTypeList = new List<ushort>
		{
			1,
			8,
			64,
			2,
			16,
			128,
			4,
			32,
			256,
			512,
			1024,
			2048,
			4096,
			8192,
			16384,
			32768
		};

		// Token: 0x04003142 RID: 12610
		private List<LanguageKey> _relationTypeSortedDisplayKeyList = new List<LanguageKey>
		{
			LanguageKey.LK_RelationShip_Parent,
			LanguageKey.LK_RelationShip_Parent,
			LanguageKey.LK_RelationShip_Parent,
			LanguageKey.LK_RelationShip_Child,
			LanguageKey.LK_RelationShip_Child,
			LanguageKey.LK_RelationShip_Child,
			LanguageKey.LK_RelationShipGeneration_Siblings,
			LanguageKey.LK_RelationShipGeneration_Siblings,
			LanguageKey.LK_RelationShipGeneration_Siblings,
			LanguageKey.LK_RelationShip_SwornBro,
			LanguageKey.LK_RelationShip_Wife,
			LanguageKey.LK_RelationShip_Mentor,
			LanguageKey.LK_RelationShip_Mentor,
			LanguageKey.LK_RelationShip_Friend,
			LanguageKey.LK_RelationShip_Adored,
			LanguageKey.LK_RelationShip_Enemy
		};
	}
}
