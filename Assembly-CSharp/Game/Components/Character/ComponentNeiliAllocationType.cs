using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F1F RID: 3871
	public class ComponentNeiliAllocationType : MonoBehaviour
	{
		// Token: 0x0600B255 RID: 45653 RVA: 0x00512A70 File Offset: 0x00510C70
		public void Init(byte type)
		{
			string i = LocalStringManager.Get("LK_Neili_Allocation_Type_" + type.ToString());
			this._color = NeiliAllocationTypes.GetColorByType(type);
			this.typeName.text = i.SetColor(this._color);
			base.GetComponent<TooltipInvoker>().PresetParam = new string[]
			{
				i,
				LocalStringManager.Get("LK_NeiliDesc_" + type.ToString())
			};
		}

		// Token: 0x0600B256 RID: 45654 RVA: 0x00512AE8 File Offset: 0x00510CE8
		public void Set(int value, int extraValue, bool isAdd)
		{
			bool flag = value < 0;
			if (flag)
			{
				value = 0;
			}
			string sign = isAdd ? "+" : "/";
			if (!true)
			{
			}
			string text;
			if (extraValue <= 0)
			{
				if (extraValue >= 0)
				{
					text = "";
				}
				else
				{
					text = extraValue.ToString();
				}
			}
			else
			{
				text = sign + extraValue.ToString();
			}
			if (!true)
			{
			}
			string extraText = text;
			this.imageNumber.Set((uint)value, 2, 3, this._color);
			this.extraNumber.text = extraText.SetColor(this._color);
		}

		// Token: 0x04008A56 RID: 35414
		public ImageDigits imageNumber;

		// Token: 0x04008A57 RID: 35415
		public TextMeshProUGUI extraNumber;

		// Token: 0x04008A58 RID: 35416
		public CButton btnAdd;

		// Token: 0x04008A59 RID: 35417
		public CButton btnMinus;

		// Token: 0x04008A5A RID: 35418
		public TextMeshProUGUI typeName;

		// Token: 0x04008A5B RID: 35419
		private const int DisplayMinCount = 2;

		// Token: 0x04008A5C RID: 35420
		private const int DisplayMaxCount = 3;

		// Token: 0x04008A5D RID: 35421
		private string _color;
	}
}
