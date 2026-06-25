using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class MouseTipDestiny : MouseTipBase
{
	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x060029DE RID: 10718 RVA: 0x0013D715 File Offset: 0x0013B915
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x0013D718 File Offset: 0x0013B918
	protected override void Init(ArgumentBox argsBox)
	{
		sbyte destinyType;
		argsBox.Get("DestinyType", out destinyType);
		DestinyTypeItem destinyTypeItem = DestinyType.Instance[destinyType];
		base.CGet<TextMeshProUGUI>("Title").text = destinyTypeItem.Name;
		base.CGet<TextMeshProUGUI>("Desc").text = destinyTypeItem.Desc;
		Refers motherSettlement = base.CGet<Refers>("MotherSettlement");
		OrganizationItem organizationItem = Organization.Instance[destinyTypeItem.SectList.First<sbyte>()];
		sbyte goodness = organizationItem.Goodness;
		if (!true)
		{
		}
		LanguageKey languageKey;
		if (goodness != -1)
		{
			if (goodness != 1)
			{
				languageKey = LanguageKey.LK_Organization_ExtraDescNormal;
			}
			else
			{
				languageKey = LanguageKey.LK_Organization_ExtraDescGood;
			}
		}
		else
		{
			languageKey = LanguageKey.LK_Organization_ExtraDescEvil;
		}
		if (!true)
		{
		}
		LanguageKey goodnessKey = languageKey;
		string goodnessStr = LocalStringManager.Get(goodnessKey);
		motherSettlement.CGet<TextMeshProUGUI>("Text").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Destiny_MotherSettlement, goodnessStr).ColorReplace();
		RectTransform layout = motherSettlement.CGet<RectTransform>("Layout");
		GameObject prefab = layout.GetChild(0).gameObject;
		for (int i = 0; i < destinyTypeItem.SectList.Count; i++)
		{
			GameObject obj = (layout.childCount > i) ? layout.GetChild(i).gameObject : Object.Instantiate<GameObject>(prefab, layout);
			obj.SetActive(true);
			TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
			OrganizationItem orgConfig = Organization.Instance[destinyTypeItem.SectList[i]];
			text.text = "-" + orgConfig.Name;
		}
		for (int j = destinyTypeItem.SectList.Count; j < layout.childCount; j++)
		{
			layout.GetChild(j).gameObject.SetActive(false);
		}
		Refers motherOrgGrade = base.CGet<Refers>("MotherOrgGrade");
		motherOrgGrade.CGet<TextMeshProUGUI>("Text").text = LocalStringManager.Get(LanguageKey.LK_MouseTip_Destiny_MotherOrgGrade);
		RectTransform layout2 = motherOrgGrade.CGet<RectTransform>("Layout");
		GameObject prefab2 = layout2.GetChild(0).gameObject;
		for (int k = 0; k < destinyTypeItem.OrganizationGradeRange.Length; k++)
		{
			GameObject obj2 = (layout2.childCount > k) ? layout2.GetChild(k).gameObject : Object.Instantiate<GameObject>(prefab2, layout2);
			obj2.SetActive(true);
			TextMeshProUGUI text2 = obj2.GetComponent<TextMeshProUGUI>();
			sbyte grade = destinyTypeItem.OrganizationGradeRange[k];
			string gradeName = CommonUtils.GetOrgGradeName((int)grade);
			text2.text = "-" + gradeName;
		}
		for (int l = destinyTypeItem.OrganizationGradeRange.Length; l < layout2.childCount; l++)
		{
			layout2.GetChild(l).gameObject.SetActive(false);
		}
		Refers behavior = base.CGet<Refers>("Behavior");
		behavior.CGet<TextMeshProUGUI>("Text").text = LocalStringManager.Get(LanguageKey.LK_MouseTip_Destiny_BehaviorType);
		List<sbyte> behaviorList = new List<sbyte>();
		sbyte m = 0;
		while ((int)m < GameData.Domains.Character.BehaviorType.Ranges.Length)
		{
			ValueTuple<short, short> range = GameData.Domains.Character.BehaviorType.Ranges[(int)m];
			short curMin = destinyTypeItem.MoralityRange[0];
			short curMax = destinyTypeItem.MoralityRange[1];
			bool flag = curMin <= range.Item1 && curMax >= range.Item2;
			if (flag)
			{
				behaviorList.Add(m);
			}
			m += 1;
		}
		RectTransform layout3 = behavior.CGet<RectTransform>("Layout");
		GameObject prefab3 = layout3.GetChild(0).gameObject;
		for (int n = 0; n < behaviorList.Count; n++)
		{
			GameObject obj3 = (layout3.childCount > n) ? layout3.GetChild(n).gameObject : Object.Instantiate<GameObject>(prefab3, layout3);
			obj3.SetActive(true);
			TextMeshProUGUI text3 = obj3.GetComponent<TextMeshProUGUI>();
			sbyte behaviorType = behaviorList[n];
			BehaviorTypeItem behaviorTypeItem = Config.BehaviorType.Instance[(short)behaviorType];
			text3.text = "-<SpName=" + behaviorTypeItem.Icon + ">" + CommonUtils.GetBehaviorString(behaviorType);
			text3.GetComponent<TMPTextSpriteHelper>().Parse();
		}
		for (int i2 = behaviorList.Count; i2 < layout3.childCount; i2++)
		{
			layout3.GetChild(i2).gameObject.SetActive(false);
		}
		Refers feature = base.CGet<Refers>("Feature");
		feature.CGet<TextMeshProUGUI>("Text").text = LocalStringManager.Get(LanguageKey.LK_MouseTip_Destiny_CharacterFeature);
		CharacterFeatureItem characterFeatureItem = CharacterFeature.Instance[destinyTypeItem.Feature];
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		sb.Append("-");
		for (int i3 = 0; i3 < 3; i3++)
		{
			FeatureMedals medals = characterFeatureItem.FeatureMedals[i3];
			for (int j2 = 0; j2 < medals.Values.Count; j2++)
			{
				sbyte value = medals.Values.First<sbyte>();
				string spName = MouseTipDestiny.FeatureIconConfig[(int)value][i3];
				sb.Append("<SpName=" + spName + ">");
			}
		}
		sb.Append(characterFeatureItem.Name.SetColor("pinkyellow"));
		TextMeshProUGUI featureName = feature.CGet<TextMeshProUGUI>("FeatureName");
		featureName.text = sb.ToString();
		featureName.GetComponent<TMPTextSpriteHelper>().Parse();
		EasyPool.Free<StringBuilder>(sb);
	}

	// Token: 0x04001E6A RID: 7786
	private static readonly string[][] FeatureIconConfig = new string[][]
	{
		new string[]
		{
			"mousetip_characteristic_10",
			"mousetip_characteristic_9",
			"mousetip_characteristic_11"
		},
		new string[]
		{
			"mousetip_characteristic_4",
			"mousetip_characteristic_3",
			"mousetip_characteristic_5"
		},
		new string[]
		{
			"mousetip_characteristic_1",
			"mousetip_characteristic_0",
			"mousetip_characteristic_2"
		},
		new string[]
		{
			"mousetip_characteristic_7",
			"mousetip_characteristic_6",
			"mousetip_characteristic_8"
		}
	};
}
