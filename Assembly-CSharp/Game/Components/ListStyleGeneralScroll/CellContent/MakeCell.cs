using System;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ECA RID: 3786
	public class MakeCell : MonoBehaviour, ICellContent<MakeCellData>, ICellContent
	{
		// Token: 0x0600AF00 RID: 44800 RVA: 0x004FBC34 File Offset: 0x004F9E34
		public void SetData(MakeCellData data)
		{
			base.gameObject.SetActive(data.IsShow);
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			this.buttonAttainment.gameObject.SetActive(data.Type == MakeCellType.Attainment);
			this.buttonAttainment.interactable = data.IsMeetAttainment;
			bool activeSelf = this.buttonAttainment.gameObject.activeSelf;
			if (activeSelf)
			{
				TooltipInvoker tip = this.buttonAttainment.GetComponent<TooltipInvoker>();
				tip.Type = TipType.Simple;
				string skillName = LifeSkillType.Instance[data.AttainmentTipData.LifeSkillType].Name;
				string title = LanguageKey.LK_Make_Target_Attainment_Tip_Title.Tr();
				stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Attainment_Tip_Desc.Tr());
				stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Attainment_Tip_Range.TrFormat(skillName, data.AttainmentTipData.MinNeedAttainment, data.AttainmentTipData.MaxNeedAttainment));
				stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Attainment_Tip_Current.TrFormat(skillName, data.AttainmentTipData.CharAttainment));
				stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Attainment_Tip_Tool.TrFormat(data.AttainmentTipData.MaxToolAttainment));
				string content = stringBuilder.ToString();
				tip.PresetParam = new string[]
				{
					title,
					content
				};
			}
			this.buttonTool.gameObject.SetActive(data.Type == MakeCellType.Tool);
			this.buttonTool.interactable = data.IsMeetTool;
			bool activeSelf2 = this.buttonTool.gameObject.activeSelf;
			if (activeSelf2)
			{
				TooltipInvoker tip2 = this.buttonTool.GetComponent<TooltipInvoker>();
				tip2.Type = TipType.Simple;
				string title2 = LanguageKey.LK_Make_Target_Tool_Tip_Title.Tr();
				stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Tool_Tip_Desc.Tr());
				stringBuilder.AppendLine(LanguageKey.LK_Make_Target_Tool_Tip_Count.TrFormat(data.ToolTipData.AvailableToolCount));
				string content2 = stringBuilder.ToString();
				tip2.PresetParam = new string[]
				{
					title2,
					content2
				};
			}
			this.buttonMaterial.gameObject.SetActive(data.Type == MakeCellType.Material);
			this.buttonMaterial.interactable = data.IsMeetMaterial;
			bool activeSelf3 = this.buttonMaterial.gameObject.activeSelf;
			if (activeSelf3)
			{
				TooltipInvoker tip3 = this.buttonMaterial.GetComponent<TooltipInvoker>();
				tip3.Type = TipType.MakeTargetMaterial;
				tip3.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("MaterialTipData", data.MaterialTipData);
			}
			stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x04008785 RID: 34693
		[SerializeField]
		private CButton buttonAttainment;

		// Token: 0x04008786 RID: 34694
		[SerializeField]
		private CButton buttonTool;

		// Token: 0x04008787 RID: 34695
		[SerializeField]
		private CButton buttonMaterial;
	}
}
