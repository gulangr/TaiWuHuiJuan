using System;
using System.Linq;
using Config;
using EasyButtons;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class ProfessionAvatarElement : MonoBehaviour
{
	// Token: 0x06002C1E RID: 11294 RVA: 0x0015930C File Offset: 0x0015750C
	[Button("设置为指定志向")]
	public void SetAs(string professionName)
	{
		bool flag = Profession.Instance.Any((ProfessionItem x) => x.Name.Contains(professionName));
		if (flag)
		{
			this.professionId = Profession.Instance.First((ProfessionItem x) => x.Name.Contains(professionName)).TemplateId;
		}
		else
		{
			Debug.LogError("Cannot analysis profession name by " + professionName);
		}
	}

	// Token: 0x04001FF5 RID: 8181
	public int professionId;
}
