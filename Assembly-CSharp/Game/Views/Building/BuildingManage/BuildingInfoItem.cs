using System;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BEF RID: 3055
	public class BuildingInfoItem : MonoBehaviour
	{
		// Token: 0x06009B3C RID: 39740 RVA: 0x0048AB20 File Offset: 0x00488D20
		public void Refresh(short buildingTemplateId, EBuildingNotAvailableType notAvailableType, string notAvailableTip, Action<short> onClick)
		{
			BuildingBlockItem buildingConfig = BuildingBlock.Instance[buildingTemplateId];
			ViewBuildingManage.SetBuildingIcon(this.imageBuildingIcon, buildingConfig, false, null);
			this.textBuildingName.text = buildingConfig.Name;
			CImage cimage = this.imageBuildingWidth;
			if (cimage != null)
			{
				cimage.SetSprite(CommonUtils.GetBuildingWidthIcon(buildingConfig.Width), false, null);
			}
			this.tip.enabled = true;
			this.tip.Type = TipType.Simple;
			TooltipInvoker tooltipInvoker = this.tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.tip.RuntimeParam.Set("arg0", buildingConfig.Name);
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			stringBuilder.Append(buildingConfig.Desc);
			bool flag = !string.IsNullOrEmpty(buildingConfig.FuncDesc);
			if (flag)
			{
				stringBuilder.Append("\n \n").Append(buildingConfig.FuncDesc.SetColor("pinkyellow"));
			}
			this.tip.RuntimeParam.Set("arg1", stringBuilder.ToString());
			EasyPool.Free<StringBuilder>(stringBuilder);
			this.button.interactable = true;
			this.button.ClearAndAddListener(delegate
			{
				onClick(buildingTemplateId);
			});
			this.textInvalid.text = notAvailableTip;
			this.HandleDisableStyle(notAvailableType);
		}

		// Token: 0x06009B3D RID: 39741 RVA: 0x0048AC93 File Offset: 0x00488E93
		protected virtual void HandleDisableStyle(EBuildingNotAvailableType notAvailableType)
		{
			this.objLock.SetActive(notAvailableType == EBuildingNotAvailableType.Locked);
			this.rootInvalid.SetActive(notAvailableType == EBuildingNotAvailableType.BuildConditionNotMet);
		}

		// Token: 0x04007809 RID: 30729
		[SerializeField]
		protected CButton button;

		// Token: 0x0400780A RID: 30730
		[SerializeField]
		protected CImage imageBuildingIcon;

		// Token: 0x0400780B RID: 30731
		[SerializeField]
		protected CImage imageBuildingWidth;

		// Token: 0x0400780C RID: 30732
		[SerializeField]
		protected TextMeshProUGUI textBuildingName;

		// Token: 0x0400780D RID: 30733
		[SerializeField]
		protected TooltipInvoker tip;

		// Token: 0x0400780E RID: 30734
		[SerializeField]
		protected GameObject rootInvalid;

		// Token: 0x0400780F RID: 30735
		[SerializeField]
		protected TextMeshProUGUI textInvalid;

		// Token: 0x04007810 RID: 30736
		[SerializeField]
		protected GameObject objLock;
	}
}
