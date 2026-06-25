using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using EasyButtons;
using FrameWork.Tools;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class ProfessionAvatarController : MonoBehaviour
{
	// Token: 0x06002C1B RID: 11291 RVA: 0x0015918C File Offset: 0x0015738C
	[Button("模拟组件位置")]
	public void ExplainSubElementsDevOnly(string professionIdsStr)
	{
		TestKit.InitLocalStringManagerManual();
		Profession.Instance.Init();
		bool flag = professionIdsStr.IsNullOrEmpty();
		if (flag)
		{
			Debug.LogError("Cannot invoke without any text.");
		}
		else
		{
			bool flag2 = professionIdsStr.Split(',', StringSplitOptions.None).Any(delegate(string x)
			{
				int num;
				return !int.TryParse(x, out num);
			});
			if (flag2)
			{
				Debug.LogError("Cannot parse to profession id list, check input " + professionIdsStr + ".");
			}
			else
			{
				bool flag3 = professionIdsStr.Split(',', StringSplitOptions.None).Select(new Func<string, int>(int.Parse)).Any((int x) => x < 0 || x >= Profession.Instance.Count);
				if (flag3)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Some profession id out of range, make sure all id is >= 0 and < ",
						Profession.Instance.Count.ToString(),
						", check input ",
						professionIdsStr,
						"."
					}));
				}
				else
				{
					this.ExplainSubElements(professionIdsStr.Split(',', StringSplitOptions.None).Select(new Func<string, int>(int.Parse)).ToList<int>());
				}
			}
		}
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x001592C0 File Offset: 0x001574C0
	public void ExplainSubElements(IReadOnlyList<int> professionIds)
	{
		foreach (ProfessionAvatarElement element in base.GetComponentsInChildren<ProfessionAvatarElement>(true))
		{
			element.gameObject.SetActive(professionIds.Contains(element.professionId));
		}
	}
}
