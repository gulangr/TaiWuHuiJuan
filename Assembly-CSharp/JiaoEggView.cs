using System;
using Config;
using GameData.DLC.FiveLoong;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class JiaoEggView : Refers
{
	// Token: 0x060023EB RID: 9195 RVA: 0x00107ADC File Offset: 0x00105CDC
	public void Refresh(JiaoLoongDisplayData data, bool hideGeneration = false)
	{
		bool flag = data == null;
		if (!flag)
		{
			JiaoItem jiaoConfig = Config.Jiao.Instance[data.TemplateId];
			MaterialItem material = Config.Material.Instance[jiaoConfig.EggMaterial];
			string baseImage = string.Format("icon_Material_JiaoEgg_Base_{0}", jiaoConfig.ColorList.Count - 1);
			base.CGet<CImage>("Base").SetSprite(baseImage, false, null);
			base.CGet<CImage>("Color").SetSprite(material.Icon, false, null);
			int genderIndex = data.Jiao.Gender ? 0 : 1;
			string genderImage = string.Format("icon_Material_JiaoEgg_Gender_{0}", genderIndex);
			base.CGet<CImage>("Gender").SetSprite(genderImage, false, null);
			CImage generationBg = base.CGet<CImage>("GenerationBg");
			generationBg.gameObject.SetActive(!hideGeneration);
			if (!hideGeneration)
			{
				int generation = Mathf.Clamp(data.Jiao.Generation + 1, 1, 999);
				int hundred = generation / 100;
				int hundredsRemainder = generation % 100;
				int ten = hundredsRemainder / 10;
				int single = hundredsRemainder % 10;
				bool flag2 = hundred > 0;
				int digit;
				if (flag2)
				{
					digit = 3;
				}
				else
				{
					bool flag3 = ten > 0;
					if (flag3)
					{
						digit = 2;
					}
					else
					{
						digit = 1;
					}
				}
				for (int i = 0; i < generationBg.transform.childCount; i++)
				{
					CImage numberImage = generationBg.transform.GetChild(i).GetComponent<CImage>();
					numberImage.gameObject.SetActive(i < digit);
					bool activeSelf = numberImage.gameObject.activeSelf;
					if (activeSelf)
					{
						if (!true)
						{
						}
						int num;
						switch (i)
						{
						case 0:
							num = single;
							break;
						case 1:
							num = ten;
							break;
						case 2:
							num = hundred;
							break;
						default:
							if (!true)
							{
							}
							<PrivateImplementationDetails>.ThrowSwitchExpressionException(i);
							break;
						}
						if (!true)
						{
						}
						int number = num;
						numberImage.SetSprite(string.Format("sp_number_0_{0}", number), false, null);
					}
				}
				generationBg.SetSprite(string.Format("icon_Material_JiaoEgg_Number_{0}", digit - 1), false, null);
			}
		}
	}
}
