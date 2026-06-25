using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.MouseTips.CombatBanned;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200084A RID: 2122
	public class MouseTipCombatRawCreate : MouseTipBase
	{
		// Token: 0x17000C64 RID: 3172
		// (get) Token: 0x06006709 RID: 26377 RVA: 0x002F0209 File Offset: 0x002EE409
		protected override bool CanStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600670A RID: 26378 RVA: 0x002F020C File Offset: 0x002EE40C
		protected override void Init(ArgumentBox argsBox)
		{
			this._itemDisplayDataList.Clear();
			argsBox.Get<RawCreateCollection>("rawCreateCollection", out this._rawCreateCollection);
			this.NeedRefresh = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x0600670B RID: 26379 RVA: 0x002F0268 File Offset: 0x002EE468
		public override void Refresh()
		{
			List<ItemKey> itemKeys = this._rawCreateCollection.Effects.Keys.ToList<ItemKey>();
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this, itemKeys, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._itemDisplayDataList);
				this.ProcessDisplayData();
			});
		}

		// Token: 0x0600670C RID: 26380 RVA: 0x002F02A0 File Offset: 0x002EE4A0
		public override void Refresh(ArgumentBox argBox)
		{
			RawCreateCollection newCollection;
			bool flag = argBox != null && argBox.Get<RawCreateCollection>("rawCreateCollection", out newCollection) && newCollection != null;
			if (flag)
			{
				this._rawCreateCollection = newCollection;
			}
			this.Refresh();
		}

		// Token: 0x0600670D RID: 26381 RVA: 0x002F02D9 File Offset: 0x002EE4D9
		private void OnListenerIdReady()
		{
			this.Refresh();
		}

		// Token: 0x0600670E RID: 26382 RVA: 0x002F02E4 File Offset: 0x002EE4E4
		private void ProcessDisplayData()
		{
			for (int i = 0; i < this._itemDisplayDataList.Count; i++)
			{
				bool flag = i >= this.holder.childCount;
				if (flag)
				{
					Object.Instantiate<GameObject>(this.template, this.holder);
				}
				this.FillData(i);
			}
			for (int j = this._itemDisplayDataList.Count; j < this.holder.childCount; j++)
			{
				this.holder.GetChild(j).gameObject.SetActive(false);
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x002F0388 File Offset: 0x002EE588
		private void FillData(int index)
		{
			Transform obj = this.holder.GetChild(index);
			ItemDisplayData itemDisplayData = this._itemDisplayDataList[index];
			ItemKey itemKey = itemDisplayData.Key;
			CombatBannedItem item = obj.GetChild(0).GetComponent<CombatBannedItem>();
			sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
			string curr = (itemDisplayData.Durability > itemDisplayData.MaxDurability / 2) ? itemDisplayData.Durability.ToString().SetColor("brightblue") : itemDisplayData.Durability.ToString().SetColor("brightred");
			int specialEffectTemplateId = this.GetSpecialEffectTemplateId(itemKey);
			item.itemIcon.SetSprite(ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId), false, null);
			item.nameLabel.text = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId).SetColor(Colors.Instance.GradeColors[(int)grade]);
			item.timeLabel.text = curr + "/" + itemDisplayData.MaxDurability.ToString();
			item.bannedStopLabel.text = "";
			bool flag = specialEffectTemplateId >= 0;
			if (flag)
			{
				SpecialEffectItem specialEffectItem = SpecialEffect.Instance[specialEffectTemplateId];
				EquipmentEffectItem rawCreateEffectItem = EquipmentEffect.Instance[specialEffectItem.RawCreateEffect];
				item.bannedStopLabel.text = rawCreateEffectItem.Desc;
			}
			obj.gameObject.SetActive(true);
		}

		// Token: 0x06006710 RID: 26384 RVA: 0x002F04F8 File Offset: 0x002EE6F8
		private int GetSpecialEffectTemplateId(ItemKey itemKey)
		{
			foreach (KeyValuePair<ItemKey, int> pair in this._rawCreateCollection.Effects)
			{
				bool flag = pair.Key.Equals(itemKey);
				if (flag)
				{
					return pair.Value;
				}
			}
			return -1;
		}

		// Token: 0x040048AA RID: 18602
		[SerializeField]
		private Transform holder;

		// Token: 0x040048AB RID: 18603
		[SerializeField]
		private GameObject template;

		// Token: 0x040048AC RID: 18604
		private RawCreateCollection _rawCreateCollection;

		// Token: 0x040048AD RID: 18605
		private List<ItemDisplayData> _itemDisplayDataList = new List<ItemDisplayData>();
	}
}
