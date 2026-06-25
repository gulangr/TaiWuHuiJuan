using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B90 RID: 2960
	public class SkillBreakPlateBonusLayout : MonoBehaviour
	{
		// Token: 0x060091F6 RID: 37366 RVA: 0x0043F584 File Offset: 0x0043D784
		public void Refresh(IEnumerable<SkillBreakPlateBonus> bonusList, short skillId, LifeSkillShorts lifeSkillAttainments, IAsyncMethodRequestHandler requestHandler)
		{
			SkillBreakPlateBonusLayout.<>c__DisplayClass4_0 CS$<>8__locals1;
			CS$<>8__locals1.requestHandler = requestHandler;
			CS$<>8__locals1.<>4__this = this;
			Queue<GameObject> namePool = EasyPool.Get<Queue<GameObject>>();
			CS$<>8__locals1.itemPool = EasyPool.Get<Queue<GameObject>>();
			namePool.Clear();
			CS$<>8__locals1.itemPool.Clear();
			this.RecycleAll(namePool, CS$<>8__locals1.itemPool);
			List<SkillBreakBonusEffectDisplay> effectList = EasyPool.Get<List<SkillBreakBonusEffectDisplay>>();
			bool flag = bonusList != null;
			if (flag)
			{
				foreach (SkillBreakPlateBonus bonus in bonusList)
				{
					bool flag2 = bonus.Type == ESkillBreakPlateBonusType.None;
					if (!flag2)
					{
						GameObject title = this.<Refresh>g__GetOne|4_2(namePool, this.bonusNameTemplate, "Title", ref CS$<>8__locals1);
						this.<Refresh>g__RefreshTitle|4_0(title, bonus, ref CS$<>8__locals1);
						SkillBreakBonusEffectHelper.GenerateBonusEffectDisplays(skillId, bonus, lifeSkillAttainments, effectList);
						this.<Refresh>g__RefreshEffectList|4_1(effectList, ref CS$<>8__locals1);
					}
				}
			}
			EasyPool.Free<Queue<GameObject>>(CS$<>8__locals1.itemPool);
			EasyPool.Free<Queue<GameObject>>(namePool);
			EasyPool.Free<List<SkillBreakBonusEffectDisplay>>(effectList);
		}

		// Token: 0x060091F7 RID: 37367 RVA: 0x0043F68C File Offset: 0x0043D88C
		private void RecycleAll(Queue<GameObject> namePool, Queue<GameObject> itemPool)
		{
			for (int i = 0; i < this.recycleBin.childCount; i++)
			{
				Transform child = this.recycleBin.GetChild(i);
				child.gameObject.SetActive(false);
				bool flag = child.name == "Title";
				if (flag)
				{
					namePool.Enqueue(child.gameObject);
				}
				else
				{
					itemPool.Enqueue(child.gameObject);
				}
			}
			for (int j = this.layout.childCount - 1; j >= 0; j--)
			{
				Transform child2 = this.layout.GetChild(j);
				child2.gameObject.SetActive(false);
				child2.SetParent(this.recycleBin);
				bool flag2 = child2.name == "Title";
				if (flag2)
				{
					namePool.Enqueue(child2.gameObject);
				}
				else
				{
					itemPool.Enqueue(child2.gameObject);
				}
			}
		}

		// Token: 0x060091F8 RID: 37368 RVA: 0x0043F788 File Offset: 0x0043D988
		public void Clean()
		{
			for (int i = this.layout.childCount - 1; i >= 0; i--)
			{
				Transform child = this.layout.GetChild(i);
				child.gameObject.SetActive(false);
				child.SetParent(this.recycleBin);
			}
		}

		// Token: 0x060091FA RID: 37370 RVA: 0x0043F7E8 File Offset: 0x0043D9E8
		[CompilerGenerated]
		private void <Refresh>g__RefreshTitle|4_0(GameObject titleObj, SkillBreakPlateBonus bonus, ref SkillBreakPlateBonusLayout.<>c__DisplayClass4_0 A_3)
		{
			TextMeshProUGUI label = titleObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			SkillBreakPlateUtils.AsyncGetBonusName(A_3.requestHandler, bonus, delegate(string bonusName)
			{
				label.text = bonusName.SetGradeColor((int)bonus.Grade);
			});
		}

		// Token: 0x060091FB RID: 37371 RVA: 0x0043F838 File Offset: 0x0043DA38
		[CompilerGenerated]
		private void <Refresh>g__RefreshEffectList|4_1(List<SkillBreakBonusEffectDisplay> effects, ref SkillBreakPlateBonusLayout.<>c__DisplayClass4_0 A_2)
		{
			foreach (SkillBreakBonusEffectDisplay effect in effects)
			{
				GameObject bonusItem = this.<Refresh>g__GetOne|4_2(A_2.itemPool, this.bonusEffectTemplate.gameObject, "Item", ref A_2);
				bonusItem.GetComponent<SkillBreakBonusEffect>().Refresh(effect, SkillBreakBonusEffect.EBonusIconSize.Big);
			}
		}

		// Token: 0x060091FC RID: 37372 RVA: 0x0043F8B0 File Offset: 0x0043DAB0
		[CompilerGenerated]
		private GameObject <Refresh>g__GetOne|4_2(Queue<GameObject> pool, GameObject template, string objName, ref SkillBreakPlateBonusLayout.<>c__DisplayClass4_0 A_4)
		{
			bool flag = pool.Count > 0;
			GameObject result;
			if (flag)
			{
				GameObject obj = pool.Dequeue();
				obj.SetActive(true);
				obj.transform.SetParent(this.layout);
				obj.name = objName;
				result = obj;
			}
			else
			{
				GameObject obj2 = Object.Instantiate<GameObject>(template, this.layout, false);
				obj2.name = objName;
				obj2.SetActive(true);
				result = obj2;
			}
			return result;
		}

		// Token: 0x0400707A RID: 28794
		[SerializeField]
		private Transform layout;

		// Token: 0x0400707B RID: 28795
		[SerializeField]
		private SkillBreakBonusEffect bonusEffectTemplate;

		// Token: 0x0400707C RID: 28796
		[SerializeField]
		private GameObject bonusNameTemplate;

		// Token: 0x0400707D RID: 28797
		[SerializeField]
		private Transform recycleBin;
	}
}
