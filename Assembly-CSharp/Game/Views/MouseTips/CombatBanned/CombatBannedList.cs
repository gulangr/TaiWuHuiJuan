using System;
using System.Collections.Generic;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips.CombatBanned
{
	// Token: 0x020008B9 RID: 2233
	public class CombatBannedList : MonoBehaviour
	{
		// Token: 0x06006A97 RID: 27287 RVA: 0x003133BF File Offset: 0x003115BF
		public void Clear()
		{
			this._skillObjects.Clear();
			this._itemObjects.Clear();
		}

		// Token: 0x06006A98 RID: 27288 RVA: 0x003133DA File Offset: 0x003115DA
		public void SetConstraint(int val)
		{
			this.itemHolder.GetComponent<GridLayoutGroup>().constraintCount = val;
		}

		// Token: 0x06006A99 RID: 27289 RVA: 0x003133F0 File Offset: 0x003115F0
		public void Set(List<short> ids)
		{
			for (int i = 0; i < ids.Count; i++)
			{
				bool flag = i >= this.itemHolder.childCount;
				if (flag)
				{
					Object.Instantiate<Transform>(this.itemHolder.GetChild(0), this.itemHolder);
				}
				short id = ids[i];
				CombatBannedItem obj = this.itemHolder.GetChild(i).GetComponent<CombatBannedItem>();
				obj.Set(id);
				obj.gameObject.SetActive(true);
				this._skillObjects[id] = obj;
			}
			for (int j = ids.Count; j < this.itemHolder.childCount; j++)
			{
				this.itemHolder.GetChild(j).gameObject.SetActive(false);
			}
			int lineCount = Math.Min(this.lines.childCount, ids.Count);
			for (int k = 0; k < lineCount; k++)
			{
				this.lines.GetChild(k).gameObject.SetActive(true);
			}
			for (int l = lineCount; l < this.lines.childCount; l++)
			{
				this.lines.GetChild(l).gameObject.SetActive(false);
			}
		}

		// Token: 0x06006A9A RID: 27290 RVA: 0x00313544 File Offset: 0x00311744
		public void Set(List<ItemKey> keys)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				bool flag = i >= this.itemHolder.childCount;
				if (flag)
				{
					Object.Instantiate<Transform>(this.itemHolder.GetChild(0), this.itemHolder);
				}
				ItemKey key = keys[i];
				CombatBannedItem obj = this.itemHolder.GetChild(i).GetComponent<CombatBannedItem>();
				obj.Set(key);
				obj.gameObject.SetActive(true);
				this._itemObjects.Add(obj);
			}
			for (int j = keys.Count; j < this.itemHolder.childCount; j++)
			{
				this.itemHolder.GetChild(j).gameObject.SetActive(false);
			}
			int lineCount = Math.Min(this.lines.childCount, keys.Count);
			for (int k = 0; k < lineCount; k++)
			{
				this.lines.GetChild(k).gameObject.SetActive(true);
			}
			for (int l = lineCount; l < this.lines.childCount; l++)
			{
				this.lines.GetChild(l).gameObject.SetActive(false);
			}
		}

		// Token: 0x06006A9B RID: 27291 RVA: 0x00313694 File Offset: 0x00311894
		public void HandleData(Dictionary<short, CountdownData> countdownData)
		{
			foreach (KeyValuePair<short, CombatBannedItem> keyValuePair in this._skillObjects)
			{
				short num;
				CombatBannedItem combatBannedItem;
				keyValuePair.Deconstruct(out num, out combatBannedItem);
				short id = num;
				CombatBannedItem obj = combatBannedItem;
				CountdownData data;
				bool flag = !countdownData.TryGetValue(id, out data);
				if (flag)
				{
					obj.SetTime(0, 0);
					obj.SetBannedStop(true);
					this._lifted.Add(obj.bannedStopCanvasGroup);
				}
				else
				{
					obj.SetTime(data.Left, data.Total);
					bool flag2 = data.Left > 0;
					if (flag2)
					{
						obj.SetBannedStop(false);
						this._lifted.Remove(obj.bannedStopCanvasGroup);
					}
					else
					{
						bool flag3 = data.Left < 0;
						if (flag3)
						{
							obj.SetBannedStop(false);
							this._lifted.Add(obj.bannedStopCanvasGroup);
						}
						else
						{
							obj.SetBannedStop(true);
							this._lifted.Add(obj.bannedStopCanvasGroup);
						}
					}
				}
			}
		}

		// Token: 0x06006A9C RID: 27292 RVA: 0x003137C8 File Offset: 0x003119C8
		public void HandleData(List<CountdownData> countdownData)
		{
			for (int index = 0; index < this._itemObjects.Count; index++)
			{
				CombatBannedItem obj = this._itemObjects[index];
				bool flag = !countdownData.CheckIndex(index);
				if (flag)
				{
					obj.SetTime(0, 0);
					obj.SetBannedStop(true);
					this._lifted.Add(obj.bannedStopCanvasGroup);
				}
				else
				{
					CountdownData data = countdownData[index];
					obj.SetTime(data.Left, data.Total);
					bool flag2 = data.Left > 0;
					if (flag2)
					{
						obj.SetBannedStop(false);
						this._lifted.Remove(obj.bannedStopCanvasGroup);
					}
					else
					{
						bool flag3 = data.Left < 0;
						if (flag3)
						{
							obj.SetBannedStop(false);
							this._lifted.Add(obj.bannedStopCanvasGroup);
						}
						else
						{
							obj.SetBannedStop(true);
							this._lifted.Add(obj.bannedStopCanvasGroup);
						}
					}
				}
			}
		}

		// Token: 0x06006A9D RID: 27293 RVA: 0x003138D4 File Offset: 0x00311AD4
		private void Update()
		{
			this._alpha += 0.01f * (float)this._multiplier;
			bool flag = this._alpha >= 1f;
			if (flag)
			{
				this._alpha = 1f;
				this._multiplier = -this._multiplier;
			}
			else
			{
				bool flag2 = this._alpha <= 0.05f;
				if (flag2)
				{
					this._alpha = 0.05f;
					this._multiplier = -this._multiplier;
				}
			}
			foreach (CanvasGroup obj in this._lifted)
			{
				obj.alpha = this._alpha;
			}
		}

		// Token: 0x04004CFF RID: 19711
		[SerializeField]
		private Transform lines;

		// Token: 0x04004D00 RID: 19712
		[SerializeField]
		private Transform itemHolder;

		// Token: 0x04004D01 RID: 19713
		private const float MaxAlpha = 1f;

		// Token: 0x04004D02 RID: 19714
		private const float MinAlpha = 0.05f;

		// Token: 0x04004D03 RID: 19715
		private const float AlphaSpeed = 0.01f;

		// Token: 0x04004D04 RID: 19716
		private float _alpha = 1f;

		// Token: 0x04004D05 RID: 19717
		private int _multiplier = -1;

		// Token: 0x04004D06 RID: 19718
		private Dictionary<short, CombatBannedItem> _skillObjects = new Dictionary<short, CombatBannedItem>();

		// Token: 0x04004D07 RID: 19719
		private List<CombatBannedItem> _itemObjects = new List<CombatBannedItem>();

		// Token: 0x04004D08 RID: 19720
		private readonly HashSet<CanvasGroup> _lifted = new HashSet<CanvasGroup>();
	}
}
