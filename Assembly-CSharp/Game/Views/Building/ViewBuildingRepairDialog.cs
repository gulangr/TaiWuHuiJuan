using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE2 RID: 3042
	public class ViewBuildingRepairDialog : UIBase
	{
		// Token: 0x06009A03 RID: 39427 RVA: 0x0048098C File Offset: 0x0047EB8C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<BuildingBlockKey>("blockKey", out this._blockKey);
			argsBox.Get<Action>("onConfirm", out this._onConfirm);
			argsBox.Get<BuildingBlockData>("blockData", out this._blockData);
			string buildingName;
			argsBox.Get("buildingName", out buildingName);
			int ownMoney;
			argsBox.Get("ownMoney", out ownMoney);
			int costMoney;
			argsBox.Get("costMoney", out costMoney);
			BuildingBlockItem buildingConfig = BuildingBlock.Instance[this._blockData.TemplateId];
			int percent = (int)((buildingConfig.MaxDurability - this._blockData.Durability) * 100 / buildingConfig.MaxDurability);
			this.imageDamage.fillAmount = (float)percent / 100f;
			string color = (this._blockData.Durability < buildingConfig.MaxDurability) ? "brightred" : "pinkyellow";
			this.textDamage.text = string.Format("{0}%", percent).SetColor(color);
			this.textTitle.text = LanguageKey.LK_Building_QuickAction_Repair_Title.Tr();
			this.textConfirm.text = LanguageKey.LK_Building_QuickAction_Repair_Confirm.TrFormat(buildingName);
			ResourceTypeItem resourceConfig = ResourceType.Instance[6];
			string costStr = CommonUtils.GetDisplayStringForNum(ownMoney, 100000).SetColor("brightblue") + "/" + CommonUtils.GetDisplayStringForNum(costMoney, 100000);
			this.textCost.text = LanguageKey.LK_Building_QuickAction_Repair_Content.TrFormat(resourceConfig.Icon, costStr);
			this.textCostSpriteHelper.Parse();
		}

		// Token: 0x06009A04 RID: 39428 RVA: 0x00480B19 File Offset: 0x0047ED19
		private void Awake()
		{
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.buttonCancel.ClearAndAddListener(new Action(this.OnClickCancel));
		}

		// Token: 0x06009A05 RID: 39429 RVA: 0x00480B4C File Offset: 0x0047ED4C
		private void OnClickCancel()
		{
			this.QuickHide();
		}

		// Token: 0x06009A06 RID: 39430 RVA: 0x00480B58 File Offset: 0x0047ED58
		private void OnClickConfirm()
		{
			BuildingDomainMethod.AsyncCall.Repair(null, this._blockKey, delegate(int offset, RawDataPool pool)
			{
				ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
				Serializer.Deserialize(pool, offset, ref retValue);
				GEvent.OnEvent(UiEvents.RepairBuilding, EasyPool.Get<ArgumentBox>().Set("BuildingBlockIndex", retValue.Item2.BlockIndex));
			});
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
			this.QuickHide();
		}

		// Token: 0x040076BA RID: 30394
		[SerializeField]
		private CImage imageDamage;

		// Token: 0x040076BB RID: 30395
		[SerializeField]
		private TextMeshProUGUI textDamage;

		// Token: 0x040076BC RID: 30396
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040076BD RID: 30397
		[SerializeField]
		private TextMeshProUGUI textConfirm;

		// Token: 0x040076BE RID: 30398
		[SerializeField]
		private TextMeshProUGUI textCost;

		// Token: 0x040076BF RID: 30399
		[SerializeField]
		private TMPTextSpriteHelper textCostSpriteHelper;

		// Token: 0x040076C0 RID: 30400
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x040076C1 RID: 30401
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x040076C2 RID: 30402
		private BuildingBlockKey _blockKey;

		// Token: 0x040076C3 RID: 30403
		private BuildingBlockData _blockData;

		// Token: 0x040076C4 RID: 30404
		private Action _onConfirm;
	}
}
