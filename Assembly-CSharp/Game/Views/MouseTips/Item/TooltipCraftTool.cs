using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Make;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x0200089F RID: 2207
	public class TooltipCraftTool : TooltipItemBase
	{
		// Token: 0x17000C8B RID: 3211
		// (get) Token: 0x0600699F RID: 27039 RVA: 0x00309397 File Offset: 0x00307597
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069A0 RID: 27040 RVA: 0x0030939C File Offset: 0x0030759C
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemKey = this._itemData.RealKey;
			this.configData = CraftTool.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			this.DisableDetail = true;
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x060069A1 RID: 27041 RVA: 0x00309417 File Offset: 0x00307617
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x060069A2 RID: 27042 RVA: 0x00309429 File Offset: 0x00307629
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshAddProperty();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x060069A3 RID: 27043 RVA: 0x0030944C File Offset: 0x0030764C
		private void RefreshAddProperty()
		{
			List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
			int index = 0;
			short attainment = ViewMake.GetToolAttainment(this._itemData.Key.TemplateId, -1);
			foreach (sbyte lifeSkillType in this.configData.RequiredLifeSkillTypes)
			{
				if (!true)
				{
				}
				ECharacterPropertyReferencedType echaracterPropertyReferencedType;
				switch (lifeSkillType)
				{
				case 6:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentForging;
					break;
				case 7:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentWoodworking;
					break;
				case 8:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentMedicine;
					break;
				case 9:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentToxicology;
					break;
				case 10:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentWeaving;
					break;
				case 11:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentJade;
					break;
				case 12:
				case 13:
					goto IL_AA;
				case 14:
					echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentCooking;
					break;
				default:
					goto IL_AA;
				}
				if (!true)
				{
				}
				ECharacterPropertyReferencedType referencedType = echaracterPropertyReferencedType;
				TooltipUtil.AppendAddProperty(ref index, list, (short)referencedType, (int)attainment, (int)attainment, this.IsDetail, false, false, false, true, true, false, false);
				continue;
				IL_AA:
				throw new ArgumentOutOfRangeException();
			}
			for (int i = index; i < list.Count; i++)
			{
				list[i].gameObject.SetActive(false);
			}
			this.layoutAddProperty.gameObject.SetActive(index > 0);
		}

		// Token: 0x060069A4 RID: 27044 RVA: 0x003095A0 File Offset: 0x003077A0
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool flag = !this.configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Repairable);
			}
			bool flag2 = !this.configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(ItemFunction.Transferable);
			}
			bool flag3 = !this.configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(ItemFunction.Poisonable);
			}
			bool flag4 = !this.configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(ItemFunction.Refinable);
			}
		}

		// Token: 0x04004BDC RID: 19420
		[Header("加成属性")]
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004BDD RID: 19421
		private CraftToolItem configData;
	}
}
