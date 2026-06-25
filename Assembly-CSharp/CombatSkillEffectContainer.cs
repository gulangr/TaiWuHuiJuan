using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Combat;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000267 RID: 615
public class CombatSkillEffectContainer : MonoBehaviour
{
	// Token: 0x060028D0 RID: 10448 RVA: 0x0012E0B0 File Offset: 0x0012C2B0
	public void Set(IReadOnlyList<CombatSkillEffectData> data)
	{
		bool flag = data == null || data.Count <= 0;
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			bool anyVisibleData = false;
			foreach (CombatSkillEffectContainer.EffectLayoutData effect in this.effects)
			{
				bool effectDataExist = data.Exist(new Predicate<CombatSkillEffectData>(effect.IsMatch));
				bool flag2 = effectDataExist != effect.layout.activeSelf;
				if (flag2)
				{
					effect.layout.SetActive(effectDataExist);
				}
				bool flag3 = !effectDataExist;
				if (!flag3)
				{
					anyVisibleData = true;
					CombatSkillEffectData effectData = data.First(new Func<CombatSkillEffectData, bool>(effect.IsMatch));
					effect.value.text = LocalStringManager.GetFormat(effect.languageKey, effectData.Value).ColorReplace();
				}
			}
			base.gameObject.SetActive(anyVisibleData);
		}
	}

	// Token: 0x04001DC0 RID: 7616
	[SerializeField]
	private List<CombatSkillEffectContainer.EffectLayoutData> effects;

	// Token: 0x020015EC RID: 5612
	[Serializable]
	public class EffectLayoutData
	{
		// Token: 0x0600D064 RID: 53348 RVA: 0x005A651C File Offset: 0x005A471C
		public bool IsMatch(CombatSkillEffectData data)
		{
			return this.type == data.Type;
		}

		// Token: 0x0400A66D RID: 42605
		public ECombatSkillEffectType type;

		// Token: 0x0400A66E RID: 42606
		public GameObject layout;

		// Token: 0x0400A66F RID: 42607
		public TextMeshProUGUI value;

		// Token: 0x0400A670 RID: 42608
		public string languageKey;
	}
}
