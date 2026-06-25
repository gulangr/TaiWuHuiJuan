using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game.Components.Item;
using Game.Views.Combat.Migrate;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B09 RID: 2825
	public class CombatDrops : MonoBehaviour
	{
		// Token: 0x06008AE9 RID: 35561 RVA: 0x0040471C File Offset: 0x0040291C
		public void SetPoolItem()
		{
			PoolManager.SetSrcObject("ui_CombatDrop_ResourcePrefabKey", this.resourceHolderGo);
			PoolManager.SetSrcObject("ui_CombatDrop_ItemPrefabKey", this.itemHolderGo);
		}

		// Token: 0x06008AEA RID: 35562 RVA: 0x00404741 File Offset: 0x00402941
		public void ClearPoolItem()
		{
			PoolManager.RemoveData("ui_CombatDrop_ResourcePrefabKey");
			PoolManager.RemoveData("ui_CombatDrop_ItemPrefabKey");
		}

		// Token: 0x06008AEB RID: 35563 RVA: 0x0040475A File Offset: 0x0040295A
		public void PlayDropAniCoroutine(CombatResultDisplayData displayData, CombatInfoChar enemyInfoChar, SkeletonAnimation skeletonAnimation)
		{
			base.StartCoroutine(this.PlayDropAni(displayData, enemyInfoChar, skeletonAnimation));
		}

		// Token: 0x06008AEC RID: 35564 RVA: 0x0040476D File Offset: 0x0040296D
		public IEnumerator PlayDropAni(CombatResultDisplayData displayData, CombatInfoChar enemyInfoChar, SkeletonAnimation skeletonAnimation)
		{
			int resourceTotalCount = 0;
			sbyte b;
			for (sbyte resourceType = 0; resourceType < 8; resourceType = b + 1)
			{
				resourceTotalCount += displayData.Resource.Get((int)resourceType);
				b = resourceType;
			}
			bool flag = resourceTotalCount > 0 || displayData.ItemList.Count > 0;
			if (flag)
			{
				AudioManager.Instance.PlaySound("combatDrops_falling", false, false);
				bool flag2 = displayData.ItemList.Count > 0;
				if (flag2)
				{
					Vector2 force = (displayData.CombatStatus == 5) ? this._forceFlee : this._forceItem;
					int num;
					for (int i = 0; i < displayData.ItemList.Count; i = num + 1)
					{
						yield return this._waitForSeconds;
						force += ((displayData.CombatStatus == 5) ? this._forceFlee : this._forceItemAdd);
						RectTransform itemHolder = PoolManager.GetObject<RectTransform>("ui_CombatDrop_ItemPrefabKey");
						CombatDropsItemHolder itemHolderRefers = itemHolder.GetComponent<CombatDropsItemHolder>();
						ItemBack item = itemHolderRefers.itemView;
						ItemDisplayData itemDisplayData = displayData.ItemList[i];
						item.Set(itemDisplayData, false);
						CombatDropsTrailingHolder trailingHolder = itemHolderRefers.trailingHolder;
						List<ParticleSystem> particles = trailingHolder.gradeList;
						sbyte grade = ItemTemplateHelper.GetGrade(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
						bool flag3 = grade < 2;
						if (flag3)
						{
							grade = 2;
						}
						for (int j = 0; j < particles.Count; j = num + 1)
						{
							bool flag4 = particles[j].gameObject.name.Contains(grade.ToString());
							if (flag4)
							{
								particles[j].gameObject.SetActive(true);
								particles[j].Play();
							}
							num = j;
						}
						itemHolder.GetComponent<CombatDropsAudio>().SetAudioName((Random.Range(0, 2) == 0) ? "combatDrops_resource_goods_01" : "combatDrops_resource_goods_02");
						this.DoDropAni(itemHolder, "ui_CombatDrop_ItemPrefabKey", enemyInfoChar, skeletonAnimation, force);
						itemHolder = null;
						itemHolderRefers = null;
						item = null;
						itemDisplayData = null;
						trailingHolder = null;
						particles = null;
						num = i;
					}
					force = default(Vector2);
				}
				for (sbyte resourceType2 = 0; resourceType2 < 8; resourceType2 = b + 1)
				{
					int resourceCount = displayData.Resource.Get((int)resourceType2);
					bool flag5 = resourceCount == 0;
					if (!flag5)
					{
						string iconName = CombatDrops.GetIconName(resourceCount, resourceType2, false);
						yield return this._waitForSeconds;
						this.DoDropResourceAni(enemyInfoChar, "ui_CombatDrop_ResourcePrefabKey", skeletonAnimation, iconName, 6, displayData);
						iconName = null;
					}
					b = resourceType2;
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x06008AED RID: 35565 RVA: 0x00404794 File Offset: 0x00402994
		public static string GetIconName(int count, sbyte resourceType, bool isItem = false)
		{
			int level = CombatDrops.GetCountLevel(count, resourceType);
			string[] nameArray = isItem ? CombatDrops.ResourceItemIconNameArray : CombatDrops.ResourceIconNameArray;
			if (isItem)
			{
				level = Mathf.Max(1, level);
			}
			return nameArray[(int)resourceType] + level.ToString();
		}

		// Token: 0x06008AEE RID: 35566 RVA: 0x004047E0 File Offset: 0x004029E0
		public static string GetExpIconName(int count, bool isItem = false)
		{
			int level = CombatDrops.GetCountLevel(count, 0);
			if (isItem)
			{
				level = Mathf.Max(1, level);
			}
			string prefix = isItem ? "icon_resource_item_exp_" : "icon_resource_exp_";
			return prefix + level.ToString();
		}

		// Token: 0x06008AEF RID: 35567 RVA: 0x00404824 File Offset: 0x00402A24
		private static int GetCountLevel(int count, sbyte resourceType)
		{
			int level = 0;
			bool flag = count == 0;
			int result;
			if (flag)
			{
				result = level;
			}
			else
			{
				level = 1;
				for (int i = 0; i < GlobalConfig.Instance.CombatResourceDropParam.Length; i++)
				{
					int range = GlobalConfig.Instance.CombatResourceDropParam[i];
					range = range * (int)WorldResource.Instance[12].InfluenceFactors[(int)SingletonObject.getInstance<BasicGameData>().WorldResourceAmountType] / 100;
					bool flag2 = resourceType == 7;
					if (flag2)
					{
						range /= 10;
					}
					bool flag3 = resourceType < 6;
					if (flag3)
					{
						range /= 5;
					}
					bool flag4 = count < range;
					if (flag4)
					{
						return level;
					}
					level++;
				}
				result = level;
			}
			return result;
		}

		// Token: 0x06008AF0 RID: 35568 RVA: 0x004048D4 File Offset: 0x00402AD4
		private void DoDropResourceAni(CombatInfoChar enemyInfoChar, string prefabKey, SkeletonAnimation skeletonAnimation, string iconName, sbyte resourceType, CombatResultDisplayData displayData)
		{
			RectTransform resourceHolder = PoolManager.GetObject<RectTransform>(prefabKey);
			CombatDropsResourceHolder resourceRefers = resourceHolder.GetComponent<CombatDropsResourceHolder>();
			CombatDropsTrailingHolder trailingHolder = resourceRefers.trailingHolder;
			ParticleSystem particles = trailingHolder.gradeList[0];
			bool flag = !particles.gameObject.activeSelf;
			if (flag)
			{
				particles.gameObject.SetActive(true);
			}
			particles.Play();
			CImage image = resourceRefers.resourceIcon;
			image.SetSprite(iconName, false, null);
			image.SetNativeSize();
			BoxCollider2D boxCollider2D = resourceHolder.GetComponent<BoxCollider2D>();
			boxCollider2D.size = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
			string audioName = string.Empty;
			bool flag2 = resourceType == 6;
			if (flag2)
			{
				audioName = "combatDrops_resource_money";
			}
			bool flag3 = resourceType == 7;
			if (flag3)
			{
				audioName = "combatDrops_resource_authority";
			}
			boxCollider2D.GetComponent<CombatDropsAudio>().SetAudioName(audioName);
			this.DoDropAni(resourceHolder, prefabKey, enemyInfoChar, skeletonAnimation, (displayData.CombatStatus == 5) ? this._forceFlee : this._forceResource);
		}

		// Token: 0x06008AF1 RID: 35569 RVA: 0x004049E4 File Offset: 0x00402BE4
		private void DoDropAni(RectTransform tipsTransform, string prefabKey, CombatInfoChar enemyInfoChar, SkeletonAnimation skeletonAnimation, Vector2 force)
		{
			tipsTransform.gameObject.SetActive(true);
			tipsTransform.SetParent(this.holder, false);
			tipsTransform.position = skeletonAnimation.transform.TransformPoint(Vector3.zero.SetY(150f));
			tipsTransform.GetComponent<Rigidbody2D>().AddForce(force);
			CanvasGroup canvasGroup = tipsTransform.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1f;
			canvasGroup.DOFade(0.3f, 1.5f).SetDelay(3f).OnComplete(delegate
			{
				PoolManager.Destroy(prefabKey, tipsTransform.gameObject);
			});
		}

		// Token: 0x04006A89 RID: 27273
		[SerializeField]
		private GameObject resourceHolderGo;

		// Token: 0x04006A8A RID: 27274
		[SerializeField]
		private RectTransform holder;

		// Token: 0x04006A8B RID: 27275
		[SerializeField]
		private GameObject itemHolderGo;

		// Token: 0x04006A8C RID: 27276
		private const string ResourcePrefabKey = "ui_CombatDrop_ResourcePrefabKey";

		// Token: 0x04006A8D RID: 27277
		private const string ItemPrefabKey = "ui_CombatDrop_ItemPrefabKey";

		// Token: 0x04006A8E RID: 27278
		private static readonly string[] ResourceIconNameArray = new string[]
		{
			"icon_resource_food_",
			"icon_resource_wood_",
			"icon_resource_ston_",
			"icon_resource_jade_",
			"icon_resource_silk_",
			"icon_resource_herbal_",
			"icon_resource_money_",
			"icon_resource_authority_"
		};

		// Token: 0x04006A8F RID: 27279
		private static readonly string[] ResourceItemIconNameArray = new string[]
		{
			"icon_resource_item_food_",
			"icon_resource_item_wood_",
			"icon_resource_item_ston_",
			"icon_resource_item_jade_",
			"icon_resource_item_silk_",
			"icon_resource_item_herbal_",
			"icon_resource_item_money_",
			"icon_resource_item_authority_"
		};

		// Token: 0x04006A90 RID: 27280
		private const string ExpIconNamePrefix = "icon_resource_exp_";

		// Token: 0x04006A91 RID: 27281
		private const string ExpItemIconNamePrefix = "icon_resource_item_exp_";

		// Token: 0x04006A92 RID: 27282
		private const int ResourceRate = 5;

		// Token: 0x04006A93 RID: 27283
		private const int AuthorityRate = 10;

		// Token: 0x04006A94 RID: 27284
		private const float ItemFadeTime = 3f;

		// Token: 0x04006A95 RID: 27285
		private const float ForceResourceY = 150f;

		// Token: 0x04006A96 RID: 27286
		private const float ForceResourceX = 50f;

		// Token: 0x04006A97 RID: 27287
		private const float ForceItemY = 200f;

		// Token: 0x04006A98 RID: 27288
		private const float ForceItemX = 80f;

		// Token: 0x04006A99 RID: 27289
		private const float PositionYOffset = 150f;

		// Token: 0x04006A9A RID: 27290
		private readonly Vector2 _forceResource = new Vector2(-50f, 150f);

		// Token: 0x04006A9B RID: 27291
		private readonly Vector2 _forceItem = new Vector2(80f, 200f);

		// Token: 0x04006A9C RID: 27292
		private readonly Vector2 _forceItemAdd = new Vector2(10f, 5f);

		// Token: 0x04006A9D RID: 27293
		private readonly Vector2 _forceFlee = Vector2.zero;

		// Token: 0x04006A9E RID: 27294
		private readonly WaitForSeconds _waitForSeconds = new WaitForSeconds(0.05f);

		// Token: 0x04006A9F RID: 27295
		private const string DropsAudioNameStartDrop = "combatDrops_falling";

		// Token: 0x04006AA0 RID: 27296
		private const string DropsAudioNameMoney = "combatDrops_resource_money";

		// Token: 0x04006AA1 RID: 27297
		private const string DropsAudioNameAuthority = "combatDrops_resource_authority";

		// Token: 0x04006AA2 RID: 27298
		private const string DropsAudioNameItem1 = "combatDrops_resource_goods_01";

		// Token: 0x04006AA3 RID: 27299
		private const string DropsAudioNameItem2 = "combatDrops_resource_goods_02";
	}
}
