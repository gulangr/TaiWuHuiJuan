using System;
using Config;

namespace Game.Views.MouseTips
{
	// Token: 0x0200085A RID: 2138
	public class MouseTipFuyuBodyDamage : MouseTipFuyuPropertyGrid
	{
		// Token: 0x060067A8 RID: 26536 RVA: 0x002F5D18 File Offset: 0x002F3F18
		public void Set(int addValue, int addPercent)
		{
			bool hasBonus = addValue != 0 || addPercent != 0;
			base.gameObject.SetActive(hasBonus);
			bool flag = !hasBonus;
			if (!flag)
			{
				BodyPart bodyParts = BodyPart.Instance;
				int partCount = bodyParts.Count;
				base.PrepareItems(partCount);
				for (int i = 0; i < partCount; i++)
				{
					BodyPartItem partConfig = bodyParts[i];
					MouseTipFuyuPropertyItem item = base.GetItem(i);
					item.Set(partConfig.MouseTipIcon, partConfig.Name, addValue, addPercent, false);
				}
			}
		}
	}
}
