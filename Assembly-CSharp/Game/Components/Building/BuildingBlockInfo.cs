using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Building
{
	// Token: 0x02000F63 RID: 3939
	public class BuildingBlockInfo : MonoBehaviour
	{
		// Token: 0x04008C52 RID: 35922
		[Header("收获")]
		[SerializeField]
		public CButton getItemBtn;

		// Token: 0x04008C53 RID: 35923
		[SerializeField]
		public CImage getItemIcon;

		// Token: 0x04008C54 RID: 35924
		[SerializeField]
		public TextMeshProUGUI getItemCount;

		// Token: 0x04008C55 RID: 35925
		[Header("经营进度和人数")]
		[SerializeField]
		public TooltipInvoker shopCountMouseTip;

		// Token: 0x04008C56 RID: 35926
		[SerializeField]
		public TextMeshProUGUI shopCountValue;

		// Token: 0x04008C57 RID: 35927
		[SerializeField]
		public CImage shopProgressCircle;

		// Token: 0x04008C58 RID: 35928
		[SerializeField]
		public TooltipInvoker shopProgressMouseTip;

		// Token: 0x04008C59 RID: 35929
		[Header("损坏")]
		[SerializeField]
		public TextMeshProUGUI damageText;

		// Token: 0x04008C5A RID: 35930
		[SerializeField]
		public TooltipInvoker damageTip;

		// Token: 0x04008C5B RID: 35931
		[Header("轮回台")]
		[SerializeField]
		public TextMeshProUGUI samsaraText;

		// Token: 0x04008C5C RID: 35932
		[SerializeField]
		public TooltipInvoker samsaraMouseTip;

		// Token: 0x04008C5D RID: 35933
		[Header("居所")]
		[SerializeField]
		public TextMeshProUGUI residentsText;

		// Token: 0x04008C5E RID: 35934
		[SerializeField]
		public TooltipInvoker residentsMouseTip;

		// Token: 0x04008C5F RID: 35935
		[Header("茶马帮")]
		[SerializeField]
		public TextMeshProUGUI teaHorseCaravanText;

		// Token: 0x04008C60 RID: 35936
		[SerializeField]
		public TooltipInvoker teaHorseCaravanTip;

		// Token: 0x04008C61 RID: 35937
		[Header("新建、移除操作")]
		[SerializeField]
		public CImage buildingOperateIcon;

		// Token: 0x04008C62 RID: 35938
		[SerializeField]
		public TextMeshProUGUI buildingOperateContentText;

		// Token: 0x04008C63 RID: 35939
		[SerializeField]
		public GameObject multiplyRemoveSelected;

		// Token: 0x04008C64 RID: 35940
		[SerializeField]
		public CButton stopOperateButton;

		// Token: 0x04008C65 RID: 35941
		[Header("不能移动")]
		[SerializeField]
		public TooltipInvoker canNotMoveTip;

		// Token: 0x04008C66 RID: 35942
		[SerializeField]
		public TextMeshProUGUI canNotMoveContent;

		// Token: 0x04008C67 RID: 35943
		[Header("等级和名字")]
		[SerializeField]
		public TooltipInvoker levelMouseTip;

		// Token: 0x04008C68 RID: 35944
		[SerializeField]
		public TextMeshProUGUI levelText;

		// Token: 0x04008C69 RID: 35945
		[SerializeField]
		public TextMeshProUGUI buildingName;

		// Token: 0x04008C6A RID: 35946
		[SerializeField]
		public CImage nameBack;

		// Token: 0x04008C6B RID: 35947
		[Header("库房资源")]
		[SerializeField]
		public CImage resourceStatus;

		// Token: 0x04008C6C RID: 35948
		[SerializeField]
		public TextMeshProUGUI resourceStatusText;

		// Token: 0x04008C6D RID: 35949
		[SerializeField]
		public TooltipInvoker resourceStatusTip;

		// Token: 0x04008C6E RID: 35950
		[Header("异常")]
		[SerializeField]
		public TooltipInvoker exceptionTip;

		// Token: 0x04008C6F RID: 35951
		[SerializeField]
		public TextMeshProUGUI exceptionText;

		// Token: 0x04008C70 RID: 35952
		[Header("心材冷却")]
		[SerializeField]
		public TextMeshProUGUI cooldownText;

		// Token: 0x04008C71 RID: 35953
		[SerializeField]
		public TooltipInvoker cooldownTip;
	}
}
