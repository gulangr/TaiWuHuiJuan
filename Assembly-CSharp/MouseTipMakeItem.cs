using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003FF RID: 1023
public class MouseTipMakeItem : MouseTipBase
{
	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x06003D28 RID: 15656 RVA: 0x001EC713 File Offset: 0x001EA913
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003D29 RID: 15657 RVA: 0x001EC718 File Offset: 0x001EA918
	protected override void Init(ArgumentBox argsBox)
	{
		string content;
		argsBox.Get("Title", out content);
		this.textTips.text = content;
		float argWidth;
		Vector2 size = this.textTips.rectTransform.sizeDelta.SetX(Mathf.Min(argsBox.Get("Width", out argWidth) ? argWidth : this.textTips.preferredWidth, this.MaxWidth));
		this.textTips.rectTransform.SetSize(size);
		LayoutRebuilder.MarkLayoutForRebuild(this.textTips.rectTransform);
		MakeResult makeResult;
		argsBox.Get<MakeResult>("MakeResult", out makeResult);
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < 3; i++)
		{
			bool flag = !makeResult.MakeResultItemArray.CheckIndex(i) || !makeResult.MakeResultItemArray[i].IsInit;
			if (flag)
			{
				break;
			}
			StringBuilder stringBuilder = sb;
			if (!true)
			{
			}
			string value;
			if (i > 1)
			{
				if (i != 2)
				{
					if (!true)
					{
					}
					<PrivateImplementationDetails>.ThrowSwitchExpressionException(i);
				}
				else
				{
					BuildingBlockData buildingBlockData;
					value = LanguageKey.LK_Make_Review_Building.TrFormat(makeResult.MakeResultItemArray[i].LifeSkillRequiredAttainment.ToString().SetColor(makeResult.MakeResultItemArray[i].LifeSkillIsMeet ? "brightblue" : "brightred"), BuildingBlock.Instance[makeResult.UpgradeBuildingNameTemplate].Name.SetColor(SingletonObject.getInstance<BuildingModel>().GetBuilding(makeResult.UpgradeBuildingNameTemplate, out buildingBlockData) ? "brightblue" : "brightred"));
				}
			}
			else
			{
				value = LanguageKey.LK_Make_Review_Attainment.TrFormat(makeResult.MakeResultItemArray[i].LifeSkillRequiredAttainment.ToString().SetColor(makeResult.MakeResultItemArray[i].LifeSkillIsMeet ? "brightblue" : "brightred"));
			}
			if (!true)
			{
			}
			stringBuilder.AppendLine(value);
			List<short> subTypeIdList = makeResult.MakeResultItemArray[i].SubTypeIdList;
			IEnumerable<ValueTuple<short, short>> enumerable;
			if (subTypeIdList == null || subTypeIdList.Count <= 0)
			{
				enumerable = Enumerable.Empty<ValueTuple<short, short>>().Append(new ValueTuple<short, short>(makeResult.MakeResultItemArray[i].SubTypeId, makeResult.MakeResultItemArray[i].TemplateId));
			}
			else
			{
				IEnumerable<ValueTuple<short, short>> enumerable2 = makeResult.MakeResultItemArray[i].SubTypeIdList.Zip(makeResult.MakeResultItemArray[i].TemplateIdList, (short x, short y) => new ValueTuple<short, short>(x, y));
				enumerable = enumerable2;
			}
			foreach (ValueTuple<short, short> valueTuple in enumerable)
			{
				short subType = valueTuple.Item1;
				short templateId = valueTuple.Item2;
				MakeItemSubTypeItem result = MakeItemSubType.Instance[subType];
				sb.AppendLine((string.IsNullOrWhiteSpace(result.Name) ? LanguageKey.LK_Make_Review_EmptyText : LanguageKey.LK_Make_Review_Text).TrFormat(new object[]
				{
					result.Name,
					ItemTemplateHelper.GetGrade(result.Result.ItemType, templateId),
					CommonUtils.GetPreGradeText(ItemTemplateHelper.GetGrade(result.Result.ItemType, templateId)),
					ItemTemplateHelper.GetName(result.Result.ItemType, templateId)
				}));
			}
			sb.AppendLine();
		}
		this.textContent.text = sb.ToString().ColorReplace();
	}

	// Token: 0x04002BF3 RID: 11251
	[SerializeField]
	private TMP_Text textTips;

	// Token: 0x04002BF4 RID: 11252
	[SerializeField]
	private TMP_Text textContent;

	// Token: 0x04002BF5 RID: 11253
	public float MaxWidth;

	// Token: 0x04002BF6 RID: 11254
	private readonly List<Refers> _stageList = new List<Refers>();

	// Token: 0x04002BF7 RID: 11255
	private readonly Dictionary<Refers, List<Refers>> _stageToItemDict = new Dictionary<Refers, List<Refers>>();
}
