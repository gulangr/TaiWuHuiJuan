using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008AE RID: 2222
	public class TooltipItemOtherArea : MonoBehaviour
	{
		// Token: 0x06006A66 RID: 27238 RVA: 0x00311901 File Offset: 0x0030FB01
		public void Refresh(List<ItemFunction> disableFunctionList)
		{
			this.RefreshDisableFunction(disableFunctionList);
			base.gameObject.SetActive(disableFunctionList != null && disableFunctionList.Count > 0);
		}

		// Token: 0x06006A67 RID: 27239 RVA: 0x00311928 File Offset: 0x0030FB28
		private void RefreshDisableFunction(List<ItemFunction> disableFunctionList)
		{
			bool showDisableFunc = disableFunctionList.Count > 0;
			this.propertyDisableFunction.gameObject.SetActive(showDisableFunc);
			bool flag = showDisableFunc;
			if (flag)
			{
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				sb.Clear();
				for (int i = 0; i < disableFunctionList.Count; i++)
				{
					ItemFunction function = disableFunctionList[i];
					if (!true)
					{
					}
					string text2;
					switch (function)
					{
					case ItemFunction.Repairable:
						text2 = LocalStringManager.Get(LanguageKey.LK_Repair_Item).SetColor("repair");
						break;
					case ItemFunction.Transferable:
						text2 = LocalStringManager.Get(LanguageKey.LK_Transfer_Item).SetColor("gift");
						break;
					case ItemFunction.Poisonable:
						text2 = LocalStringManager.Get(LanguageKey.LK_Poison_Item).SetColor("darkpurple");
						break;
					case ItemFunction.Refinable:
						text2 = LocalStringManager.Get(LanguageKey.LK_Strengthen_Item).SetColor("lightblue");
						break;
					case ItemFunction.Disassemble:
						text2 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item).SetColor("pinkyellow");
						break;
					case ItemFunction.CanChangeTrick:
						text2 = LocalStringManager.Get(LanguageKey.LK_WeaponChangeTrickPercent).SetColor("brightred");
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!true)
					{
					}
					string functionKey = text2;
					sb.Append(functionKey);
					bool flag2 = i < disableFunctionList.Count - 1;
					if (flag2)
					{
						sb.Append(LocalStringManager.Get(LanguageKey.LK_ItemTips_DisableFunctionSeparator));
					}
				}
				string text = sb.ToString();
				this.propertyDisableFunction.SetValue(text);
				sb.Clear();
				EasyPool.Free<StringBuilder>(sb);
			}
		}

		// Token: 0x04004CD5 RID: 19669
		[SerializeField]
		private TooltipItemProperty propertyDisableFunction;
	}
}
