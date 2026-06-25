using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.MouseTips;
using GameData.Domains.SpecialEffect;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B1B RID: 2843
	public class CombatSpecialEffectCostNeiliAllocation : MonoBehaviour
	{
		// Token: 0x06008B88 RID: 35720 RVA: 0x0040755D File Offset: 0x0040575D
		private void Awake()
		{
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.TryRequestCost));
		}

		// Token: 0x06008B89 RID: 35721 RVA: 0x00407578 File Offset: 0x00405778
		private static string ParseTypeIcon(CastBoostEffectDisplayData data)
		{
			ECastBoostType ecastBoostType = data.Type;
			if (!true)
			{
			}
			string result;
			switch (ecastBoostType)
			{
			case ECastBoostType.CostNeiliAllocation:
				result = string.Format("sp_23_logo_{0}", (int)(data.NeiliAllocationType + 2));
				break;
			case ECastBoostType.CostWugKing:
				result = "sp_icon_wugking_0";
				break;
			case ECastBoostType.CostClearDefend:
				result = "ui9_icon_cost_neiliallocation_clear_defend_0";
				break;
			default:
				throw new ArgumentOutOfRangeException(string.Format("{0}", data.Type));
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008B8A RID: 35722 RVA: 0x004075F8 File Offset: 0x004057F8
		private static string ParseHoverIcon(CastBoostEffectDisplayData data)
		{
			ECastBoostType ecastBoostType = data.Type;
			if (!true)
			{
			}
			string result;
			switch (ecastBoostType)
			{
			case ECastBoostType.CostNeiliAllocation:
				result = "ui9_btn_distance_martial_art_1";
				break;
			case ECastBoostType.CostWugKing:
				result = "ui9_btn_distance_martial_art_1";
				break;
			case ECastBoostType.CostClearDefend:
				result = "ui9_icon_cost_neiliallocation_clear_defend_1";
				break;
			default:
				throw new ArgumentOutOfRangeException(string.Format("{0}", data.Type));
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008B8B RID: 35723 RVA: 0x00407664 File Offset: 0x00405864
		private static string ParseCostText(CastBoostEffectDisplayData data)
		{
			ECastBoostType ecastBoostType = data.Type;
			if (!true)
			{
			}
			string result;
			switch (ecastBoostType)
			{
			case ECastBoostType.CostNeiliAllocation:
				result = data.NeiliAllocationValue.ToString();
				break;
			case ECastBoostType.CostWugKing:
				result = string.Empty;
				break;
			case ECastBoostType.CostClearDefend:
				result = "+" + data.AddQiDisorder.ToString();
				break;
			default:
				throw new ArgumentOutOfRangeException(string.Format("{0}", data.Type));
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008B8C RID: 35724 RVA: 0x004076F0 File Offset: 0x004058F0
		private static Color ParseCostColor(CastBoostEffectDisplayData data)
		{
			ECastBoostType ecastBoostType = data.Type;
			if (!true)
			{
			}
			Color result;
			if (ecastBoostType != ECastBoostType.CostNeiliAllocation)
			{
				if (ecastBoostType - ECastBoostType.CostWugKing > 1)
				{
					throw new ArgumentOutOfRangeException("Type");
				}
				result = Color.white;
			}
			else
			{
				Colors instance = Colors.Instance;
				byte neiliAllocationType = data.NeiliAllocationType;
				if (!true)
				{
				}
				string colorName;
				switch (neiliAllocationType)
				{
				case 0:
					colorName = "attack";
					break;
				case 1:
					colorName = "agile";
					break;
				case 2:
					colorName = "defense";
					break;
				case 3:
					colorName = "assist";
					break;
				default:
					throw new ArgumentOutOfRangeException("NeiliAllocationType");
				}
				if (!true)
				{
				}
				result = instance[colorName];
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008B8D RID: 35725 RVA: 0x0040779C File Offset: 0x0040599C
		public void Refresh(CastBoostEffectDisplayData effectData, int current, bool extraCheck, CombatSpecialEffectCostNeiliAllocation.RequestCostNeiliEffectDelegate requestDelegate)
		{
			this._effectData = effectData;
			this._requestDelegate = requestDelegate;
			this.type.SetSprite(CombatSpecialEffectCostNeiliAllocation.ParseTypeIcon(effectData), false, null);
			this.hoverImage.SetSprite(CombatSpecialEffectCostNeiliAllocation.ParseHoverIcon(effectData), false, null);
			this.cost.text = CombatSpecialEffectCostNeiliAllocation.ParseCostText(effectData);
			this.cost.color = CombatSpecialEffectCostNeiliAllocation.ParseCostColor(effectData);
			TooltipInvoker tip = base.GetComponent<TooltipInvoker>();
			tip.Type = TipType.CommonTip;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Clear();
			tip.enabled = true;
			CommonTipBaseRuntime runtime = CombatSpecialEffectCostNeiliAllocation.BuildBoost(effectData, current, extraCheck);
			bool flag = runtime != null;
			if (flag)
			{
				tip.RuntimeParam.SetObject("Runtime", runtime);
			}
			else
			{
				AdaptableLog.Warning(string.Format("Failed to build cast boost tooltip at {0}", effectData.Type), true);
			}
			this.CreateEffect();
		}

		// Token: 0x06008B8E RID: 35726 RVA: 0x00407890 File Offset: 0x00405A90
		private static CommonTipBaseRuntime BuildBoost(CastBoostEffectDisplayData data, int current, bool extraCheck)
		{
			ECastBoostType ecastBoostType = data.Type;
			if (!true)
			{
			}
			CommonTipBaseRuntime result;
			switch (ecastBoostType)
			{
			case ECastBoostType.CostNeiliAllocation:
				result = CommonTip.DefValue.CostNeiliAllocation.BuildSimple().Set("SpecialEffect.Desc", CommonUtils.GetSpecialEffectDesc(data.EffectDescription)).Set("Current", current.ToString().SetColor((current < -data.NeiliAllocationValue) ? "brightred" : "brightblue")).Set("Cost", (-data.NeiliAllocationValue).ToString()).Set("Name", LocalStringManager.Get(string.Format("LK_Neili_Allocation_Type_{0}", data.NeiliAllocationType))).Set("Icon", string.Format("mousetip_zhenqi_{0}", data.NeiliAllocationType));
				break;
			case ECastBoostType.CostWugKing:
				result = CommonTip.DefValue.CostWugKing.BuildSimple().Set("Medicine.Name", Medicine.Instance[data.WugMedicineTemplateId].Name).Set("Count", data.WugKingCount.ToString());
				break;
			case ECastBoostType.CostClearDefend:
				result = CommonTip.DefValue.CostClearDefend.BuildSimple().Set("Current", current.ToString().SetColor((current < -data.NeiliAllocationValue) ? "brightred" : "brightblue")).Set("Cost", data.AddQiDisorder.ToString()).SetParagraphVisible("NoDefend", !extraCheck);
				break;
			default:
				result = null;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008B8F RID: 35727 RVA: 0x00407A24 File Offset: 0x00405C24
		public void ChangeInteractable(bool interactable)
		{
			base.GetComponent<CButton>().interactable = interactable;
			base.GetComponent<DisableStyleRoot>().SetStyleEffect(!interactable, false);
			this.type.SetSprite(interactable ? CombatSpecialEffectCostNeiliAllocation.ParseTypeIcon(this._effectData) : CombatSpecialEffectCostNeiliAllocation.ParseDisabledTypeIcon(this._effectData), false, null);
		}

		// Token: 0x06008B90 RID: 35728 RVA: 0x00407A7C File Offset: 0x00405C7C
		private static string ParseDisabledTypeIcon(CastBoostEffectDisplayData data)
		{
			ECastBoostType ecastBoostType = data.Type;
			if (!true)
			{
			}
			string result;
			switch (ecastBoostType)
			{
			case ECastBoostType.CostNeiliAllocation:
				result = string.Format("sp_23_logo_{0}", (int)(data.NeiliAllocationType + 2));
				break;
			case ECastBoostType.CostWugKing:
				result = "sp_icon_wugking_0";
				break;
			case ECastBoostType.CostClearDefend:
				result = "ui9_icon_cost_neiliallocation_clear_defend_3";
				break;
			default:
				throw new ArgumentOutOfRangeException(string.Format("{0}", data.Type));
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008B91 RID: 35729 RVA: 0x00407AFA File Offset: 0x00405CFA
		public void TryRequestCost()
		{
			CombatSpecialEffectCostNeiliAllocation.RequestCostNeiliEffectDelegate requestDelegate = this._requestDelegate;
			if (requestDelegate != null)
			{
				requestDelegate(this._effectData);
			}
		}

		// Token: 0x06008B92 RID: 35730 RVA: 0x00407B18 File Offset: 0x00405D18
		private void ClearEffect()
		{
			bool flag = null != this._aroundEffectInstance;
			if (flag)
			{
				Object.Destroy(this._aroundEffectInstance);
			}
		}

		// Token: 0x06008B93 RID: 35731 RVA: 0x00407B44 File Offset: 0x00405D44
		public void CreateEffect()
		{
			this.ClearEffect();
			ECastBoostType ecastBoostType = this._effectData.Type;
			if (!true)
			{
			}
			int num;
			switch (ecastBoostType)
			{
			case ECastBoostType.CostNeiliAllocation:
				num = (int)this._effectData.NeiliAllocationType;
				break;
			case ECastBoostType.CostWugKing:
				num = 4;
				break;
			case ECastBoostType.CostClearDefend:
				num = 5;
				break;
			default:
				throw new ArgumentOutOfRangeException(string.Format("{0}", this._effectData.Type));
			}
			if (!true)
			{
			}
			int index = num;
			this._aroundEffectInstance = Object.Instantiate<GameObject>(this.aroundEffects[index], base.transform);
			this._aroundEffectInstance.SetActive(true);
		}

		// Token: 0x04006AE2 RID: 27362
		[SerializeField]
		private CImage type;

		// Token: 0x04006AE3 RID: 27363
		[SerializeField]
		private CImage hoverImage;

		// Token: 0x04006AE4 RID: 27364
		[SerializeField]
		private TextMeshProUGUI cost;

		// Token: 0x04006AE5 RID: 27365
		public GameObject[] aroundEffects;

		// Token: 0x04006AE6 RID: 27366
		private CastBoostEffectDisplayData _effectData;

		// Token: 0x04006AE7 RID: 27367
		private CombatSpecialEffectCostNeiliAllocation.RequestCostNeiliEffectDelegate _requestDelegate;

		// Token: 0x04006AE8 RID: 27368
		private GameObject _aroundEffectInstance;

		// Token: 0x020020D8 RID: 8408
		// (Invoke) Token: 0x0600F86F RID: 63599
		public delegate void RequestCostNeiliEffectDelegate(CastBoostEffectDisplayData effectData);
	}
}
