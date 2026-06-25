using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD7 RID: 2775
	public class CricketDamage : MonoBehaviour
	{
		// Token: 0x17000F13 RID: 3859
		// (get) Token: 0x060088A3 RID: 34979 RVA: 0x003F4FC2 File Offset: 0x003F31C2
		private GameObject DamageTemplate
		{
			get
			{
				return this.rectTsDamage.gameObject;
			}
		}

		// Token: 0x17000F14 RID: 3860
		// (get) Token: 0x060088A4 RID: 34980 RVA: 0x003F4FCF File Offset: 0x003F31CF
		private int DamageIndex
		{
			get
			{
				return this.rectTsDamage.GetSiblingIndex();
			}
		}

		// Token: 0x17000F15 RID: 3861
		// (get) Token: 0x060088A5 RID: 34981 RVA: 0x003F4FDC File Offset: 0x003F31DC
		private int DamageCount
		{
			get
			{
				return this.rectTsDamageParent.childCount;
			}
		}

		// Token: 0x060088A6 RID: 34982 RVA: 0x003F4FEC File Offset: 0x003F31EC
		public void Set(ECricketDamageShowType type, int damage, bool isCritical, bool ally)
		{
			this.imgCritical.gameObject.SetActive(type == ECricketDamageShowType.Hp && isCritical);
			this.imgHurtSideFlagA.SetSprite(ally ? "ui9_icon_cricketcombat_4_1" : "ui9_icon_cricketcombat_4_0", true, null);
			int damageIndex = this.DamageIndex;
			int digit = 0;
			foreach (string spriteName in this.ParseSpriteNames(damage, type, ally))
			{
				int childIndex = damageIndex + digit;
				bool flag = childIndex == this.DamageCount;
				if (flag)
				{
					Object.Instantiate<GameObject>(this.DamageTemplate, this.rectTsDamageParent);
				}
				this.rectTsDamageParent.GetChild(childIndex).GetComponent<CImage>().SetSprite(spriteName, true, null);
				digit++;
			}
			for (int i = damageIndex; i < this.DamageCount; i++)
			{
				this.rectTsDamageParent.GetChild(i).gameObject.SetActive(i - damageIndex < digit);
			}
		}

		// Token: 0x060088A7 RID: 34983 RVA: 0x003F50FC File Offset: 0x003F32FC
		public void DoAnimation(bool needDelay, bool isCritical)
		{
			base.GetComponent<CanvasGroup>().alpha = 1f;
			float delay = needDelay ? Random.Range(0.25f, 0.45f) : 0f;
			base.GetComponent<CanvasGroup>().DOFade(0f, 0.4f).SetDelay(0.4f + delay).SetEase(Ease.InSine);
			RectTransform rectTs = base.GetComponent<RectTransform>();
			TweenCallback <>9__2;
			rectTs.DOLocalMoveY(base.transform.localPosition.y + 80f, 0.8f, false).SetDelay(delay).OnComplete(delegate
			{
				PoolManager.Destroy("UI_CricketCombat_DamagePrefabKey", this.gameObject);
			}).OnStart(delegate
			{
				this.gameObject.SetActive(true);
				bool isCritical2 = isCritical;
				if (isCritical2)
				{
					TweenerCore<Vector3, Vector3, VectorOptions> t = rectTs.DOScale(Vector3.one * 1.4f, 0.15f);
					TweenCallback action;
					if ((action = <>9__2) == null)
					{
						action = (<>9__2 = delegate()
						{
							rectTs.DOScale(Vector3.one, 0.15f);
						});
					}
					t.OnComplete(action);
				}
			});
		}

		// Token: 0x060088A8 RID: 34984 RVA: 0x003F51D0 File Offset: 0x003F33D0
		private string GetSpritePrefix(ECricketDamageShowType type, int damage)
		{
			bool flag = damage < 0;
			string result;
			if (flag)
			{
				result = "ui9_number_cricketcombat_4_";
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (type)
				{
				case ECricketDamageShowType.Hp:
					text = "ui9_number_cricketcombat_0_";
					break;
				case ECricketDamageShowType.Sp:
					text = "ui9_number_cricketcombat_2_";
					break;
				case ECricketDamageShowType.Durability:
					text = "ui9_number_cricketcombat_1_";
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x060088A9 RID: 34985 RVA: 0x003F5239 File Offset: 0x003F3439
		private IEnumerable<string> ParseSpriteNames(int damage, ECricketDamageShowType type, bool ally)
		{
			string prefix = this.GetSpritePrefix(type, damage);
			yield return prefix + "10";
			string text = Mathf.Abs(damage).ToString();
			for (int i = 0; i < text.Length; i++)
			{
				yield return prefix + text[i].ToString();
			}
			text = null;
			yield return ally ? "ui9_icon_cricketcombat_3_1" : "ui9_icon_cricketcombat_3_0";
			yield break;
		}

		// Token: 0x04006899 RID: 26777
		[SerializeField]
		private RectTransform rectTsDamageParent;

		// Token: 0x0400689A RID: 26778
		[SerializeField]
		private RectTransform rectTsDamage;

		// Token: 0x0400689B RID: 26779
		[SerializeField]
		private CImage imgCritical;

		// Token: 0x0400689C RID: 26780
		[SerializeField]
		private CImage imgHurtSideFlagA;
	}
}
