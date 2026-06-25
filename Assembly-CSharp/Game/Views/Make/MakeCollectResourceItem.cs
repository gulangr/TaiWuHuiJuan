using System;
using System.Collections;
using Config;
using GameData.Domains.Map;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x0200094E RID: 2382
	public class MakeCollectResourceItem : MonoBehaviour
	{
		// Token: 0x17000CDC RID: 3292
		// (get) Token: 0x06007088 RID: 28808 RVA: 0x00341C7F File Offset: 0x0033FE7F
		private int CountFactorType
		{
			get
			{
				return (int)((this._resourceType >= 0) ? ResourceType.Instance[this._resourceType].CollectMultiplier : 1);
			}
		}

		// Token: 0x17000CDD RID: 3293
		// (get) Token: 0x06007089 RID: 28809 RVA: 0x00341CA2 File Offset: 0x0033FEA2
		private int CountFactor0
		{
			get
			{
				return (SingletonObject.getInstance<BasicGameData>().WorldResourceAmountType == 0) ? 2 : 1;
			}
		}

		// Token: 0x0600708A RID: 28810 RVA: 0x00341CB4 File Offset: 0x0033FEB4
		private void LateUpdate()
		{
			bool flag = this._resourceType < 0;
			if (!flag)
			{
				bool flag2 = this._parent.ShowingResIconCacheList[(int)this._resourceType].Count > 81;
				if (flag2)
				{
					int count = this._parent.ShowingResIconCacheList[(int)this._resourceType].Count / 2;
					for (int i = 0; i < count; i++)
					{
						Rigidbody2D b = this._parent.ShowingResIconCacheList[(int)this._resourceType][i].GetComponent<Rigidbody2D>();
						b.constraints = ((b.attachedColliderCount > 0) ? RigidbodyConstraints2D.FreezePosition : RigidbodyConstraints2D.None);
					}
				}
			}
		}

		// Token: 0x0600708B RID: 28811 RVA: 0x00341D58 File Offset: 0x0033FF58
		public void Init(MakeCollectResourcePanel parent)
		{
			this._parent = parent;
		}

		// Token: 0x0600708C RID: 28812 RVA: 0x00341D64 File Offset: 0x0033FF64
		public void Set(CollectResourceResult result)
		{
			this._resourceType = result.ResourceType;
			this._hasItem = (result.ItemDisplayData != null);
			this._maxCount = result.ResourceCount / this.CountFactorType / this.CountFactor0;
			bool flag = this._maxCount > 10;
			if (flag)
			{
				this._maxCount = 10;
			}
			bool flag2 = this._maxCount == 0;
			if (flag2)
			{
				this._maxCount = 1;
			}
		}

		// Token: 0x0600708D RID: 28813 RVA: 0x00341DD1 File Offset: 0x0033FFD1
		public IEnumerator DropItem()
		{
			int itemIndex = this._hasItem ? Math.Max(0, this._maxCount - 4) : -1;
			float resCollectTime = this._hasItem ? 2f : 4f;
			float generateGap = Mathf.Min(resCollectTime / (float)Math.Max(this._maxCount - 1, 1), 0.3f);
			WaitForSeconds wait = new WaitForSeconds(generateGap);
			yield return wait;
			int num;
			for (int i = 0; i < this._maxCount; i = num)
			{
				GameObject dropIcon = this._parent.GetDropIcon(i == itemIndex, this._resourceType);
				dropIcon.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				dropIcon.transform.SetParent(this.iconHolder, false);
				dropIcon.transform.localPosition = new Vector3(Random.Range(this._localPosition.x + -150f, this._localPosition.x + 50f), this._localPosition.y + (float)i, 0f);
				Rigidbody2D rigidBody = dropIcon.GetComponent<Rigidbody2D>();
				rigidBody.constraints = RigidbodyConstraints2D.None;
				rigidBody.AddForce((i == itemIndex) ? new Vector2(Random.Range(-50f, 50f), 0f) : new Vector2(Random.Range(-10f, 10f), 0f));
				dropIcon.gameObject.SetActive(true);
				yield return wait;
				dropIcon = null;
				rigidBody = null;
				num = i + 1;
			}
			yield return wait;
			yield return new WaitForSeconds(generateGap + 0.3f);
			this._parent.SetIsCollecting(false);
			yield break;
		}

		// Token: 0x0600708E RID: 28814 RVA: 0x00341DE0 File Offset: 0x0033FFE0
		public void SetIconHolderPosition(Vector2 vector2)
		{
			this._localPosition = vector2;
		}

		// Token: 0x04005373 RID: 21363
		public Transform iconHolder;

		// Token: 0x04005374 RID: 21364
		private MakeCollectResourcePanel _parent;

		// Token: 0x04005375 RID: 21365
		private sbyte _resourceType;

		// Token: 0x04005376 RID: 21366
		private int _maxCount = 50;

		// Token: 0x04005377 RID: 21367
		private bool _hasItem;

		// Token: 0x04005378 RID: 21368
		private Vector2 _localPosition;

		// Token: 0x04005379 RID: 21369
		private const float CollectAniTimeBase = 2f;

		// Token: 0x0400537A RID: 21370
		private const float GapTime = 0.3f;

		// Token: 0x0400537B RID: 21371
		private const float FadeTime = 0.3f;

		// Token: 0x0400537C RID: 21372
		private const int MaxDropResourceIconCount = 210;

		// Token: 0x0400537D RID: 21373
		private const int MaxDropResourceIconCountSkill = 40;

		// Token: 0x0400537E RID: 21374
		private const int FreezeCount = 81;

		// Token: 0x0400537F RID: 21375
		private const float SavageSkillSize = 0.5f;

		// Token: 0x04005380 RID: 21376
		private const float NormalSize = 0.8f;

		// Token: 0x04005381 RID: 21377
		private const int CountFactor1 = 10;

		// Token: 0x04005382 RID: 21378
		private const int CountFactor2 = 4;

		// Token: 0x04005383 RID: 21379
		private const int PositionMin = -150;

		// Token: 0x04005384 RID: 21380
		private const int PositionMax = 50;

		// Token: 0x04005385 RID: 21381
		private const float ItemForce = 50f;

		// Token: 0x04005386 RID: 21382
		private const float ResourceForce = 10f;
	}
}
