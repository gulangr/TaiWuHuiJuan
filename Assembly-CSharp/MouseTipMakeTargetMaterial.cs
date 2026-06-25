using System;
using System.Text;
using Config;
using FrameWork;
using Game.Components.ListStyleGeneralScroll.CellContent;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class MouseTipMakeTargetMaterial : MouseTipBase
{
	// Token: 0x06002AC1 RID: 10945 RVA: 0x001476F0 File Offset: 0x001458F0
	protected override void Init(ArgumentBox argsBox)
	{
		MakeMaterialTipData materialTipData;
		argsBox.Get<MakeMaterialTipData>("MaterialTipData", out materialTipData);
		this.textTitle.text = LanguageKey.LK_Make_Target_Material_Tip_Title.Tr();
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		string resourceName = ResourceType.Instance[materialTipData.ResourceType].Name;
		LanguageKey hardnessKey = (materialTipData.Hardness >= 0) ? (LanguageKey.LK_CommonSortAndFilter_Material_FilterHardness_0 + materialTipData.Hardness) : LanguageKey.Invalid;
		string hardnessName = (hardnessKey == LanguageKey.Invalid) ? string.Empty : hardnessKey.Tr();
		string typeStr = hardnessName.IsNullOrEmpty() ? resourceName : (resourceName + "-" + hardnessName);
		stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Material_Tip_Type.Tr() + typeStr);
		string minGradeStr = CommonUtils.GetFullGradeText(materialTipData.MinGrade, true);
		string maxGradeStr = CommonUtils.GetFullGradeText(materialTipData.MaxGrade, true);
		string gradeStr = (materialTipData.MinGrade == materialTipData.MaxGrade) ? minGradeStr : (minGradeStr + "-" + maxGradeStr);
		stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Material_Tip_Grade.Tr() + gradeStr);
		this.textContent.text = stringBuilder.ToString();
		stringBuilder.Clear();
		EasyPool.Free<StringBuilder>(stringBuilder);
		Transform template = this.layoutItem.GetChild(0);
		for (int i = 0; i < materialTipData.ItemDataList.Count; i++)
		{
			Transform child = (i < this.layoutItem.childCount) ? this.layoutItem.GetChild(i) : Object.Instantiate<Transform>(template, this.layoutItem);
			ItemDisplayData data = materialTipData.ItemDataList[i];
			MouseTipMakeTargetMaterialItem item = child.GetComponent<MouseTipMakeTargetMaterialItem>();
			item.Init(data);
			item.gameObject.SetActive(true);
		}
		for (int j = materialTipData.ItemDataList.Count; j < this.layoutItem.childCount; j++)
		{
			this.layoutItem.GetChild(j).gameObject.SetActive(false);
		}
	}

	// Token: 0x04001EF2 RID: 7922
	[SerializeField]
	private TextMeshProUGUI textTitle;

	// Token: 0x04001EF3 RID: 7923
	[SerializeField]
	private TextMeshProUGUI textContent;

	// Token: 0x04001EF4 RID: 7924
	[SerializeField]
	private Transform layoutItem;
}
