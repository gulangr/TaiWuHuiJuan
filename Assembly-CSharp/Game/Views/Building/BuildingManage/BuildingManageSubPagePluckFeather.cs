using System;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Building.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BFD RID: 3069
	public class BuildingManageSubPagePluckFeather : BuildingManageSubPage
	{
		// Token: 0x06009C00 RID: 39936 RVA: 0x004918C4 File Offset: 0x0048FAC4
		private void Awake()
		{
			this.confirmBtn.ClearAndAddListener(delegate
			{
				BuildingDomainMethod.Call.TriggerCultivateFeatherEvent();
				this.ParentView.RequestData();
			});
			for (int i = 0; i < this.featherIconTips.Length; i++)
			{
				this.featherIconTips[i].PresetParam[0] = Config.Material.Instance[343 + i].Desc;
			}
		}

		// Token: 0x06009C01 RID: 39937 RVA: 0x00491928 File Offset: 0x0048FB28
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x06009C02 RID: 39938 RVA: 0x00491943 File Offset: 0x0048FB43
		private void OnTopUiChanged(ArgumentBox argBox)
		{
			this.ParentView.RequestData();
		}

		// Token: 0x06009C03 RID: 39939 RVA: 0x00491952 File Offset: 0x0048FB52
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.OnEvent(UiEvents.OnUpdateQuickBtnState, null);
		}

		// Token: 0x06009C04 RID: 39940 RVA: 0x0049197E File Offset: 0x0048FB7E
		public override void RequestData()
		{
			BuildingDomainMethod.AsyncCall.GetChickenPluckFeatherDisplayData(this.ParentView, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._data);
			});
			base.RequestData();
		}

		// Token: 0x06009C05 RID: 39941 RVA: 0x004919A0 File Offset: 0x0048FBA0
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this.totalProgress.text = string.Format("{0}%", this._data.FeatherValue);
			this.previewProgress.text = string.Format("(+{0}%/<SpName={1}>)", this._data.MonthlyIncrease, "ui9_icon_month");
			this.previewProgress.transform.GetComponent<TMPTextSpriteHelper>().Parse();
			int featherValuePerFeather = GlobalConfig.Instance.FeatherValuePerFeather;
			float firstProgress = Math.Min((float)this._data.FeatherValue / (float)featherValuePerFeather, 1f);
			float secondProgress = Math.Min((float)(this._data.FeatherValue + this._data.MonthlyIncrease) / (float)featherValuePerFeather, 1f);
			RectTransform firstRect = this.firstProgressBar.GetComponent<RectTransform>();
			RectTransform secondRect = this.secondProgressBar.GetComponent<RectTransform>();
			firstRect.anchoredPosition = new Vector2(-1f * (1f - firstProgress) * firstRect.rect.width, 0f);
			secondRect.anchoredPosition = new Vector2(-1f * (1f - secondProgress) * secondRect.rect.width, 0f);
			this.currentActionPoint.text = string.Format("{0}/{1}", this._data.RemainingDays.ToString().SetColor("brightblue"), GlobalConfig.Instance.CultivateEnergyCost);
			this.currentChickenCount.text = LanguageKey.LK_ChickenCanPluckFeatherCount.TrFormat(string.Format("{0}/{1}", this._data.CanPluckCount.ToString().SetColor("lightblue"), this._data.ChickenCount));
			this.SetConfirmBtnEnable();
		}

		// Token: 0x06009C06 RID: 39942 RVA: 0x00491B70 File Offset: 0x0048FD70
		private void SetConfirmBtnEnable()
		{
			bool resourceEngough = true;
			BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
			for (sbyte i = 0; i <= 6; i += 1)
			{
				int count = buildingModel.GetResourceCount(i);
				bool flag = count < GlobalConfig.Instance.CultivateCost;
				if (flag)
				{
					resourceEngough = false;
					break;
				}
			}
			int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			this.confirmBtn.interactable = (resourceEngough && this._data.RemainingDays >= GlobalConfig.Instance.CultivateEnergyCost);
			TooltipInvoker tip = this.confirmBtn.GetComponent<TooltipInvoker>();
			tip.enabled = !this.confirmBtn.interactable;
			bool flag2 = !this.confirmBtn.interactable;
			if (flag2)
			{
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				sb.Clear();
				bool flag3 = !resourceEngough;
				if (flag3)
				{
					sb.AppendLine(LocalStringManager.Get("LK_Building_PluckFeather_Resource_NotEnough").SetColor("brightred"));
				}
				bool flag4 = this._data.RemainingDays < GlobalConfig.Instance.CultivateEnergyCost;
				if (flag4)
				{
					sb.AppendLine(LocalStringManager.Get("LK_Building_PluckFeather_Time_NotEnough").SetColor("brightred"));
				}
				tip.PresetParam[0] = sb.ToString().ColorReplace();
				EasyPool.Free<StringBuilder>(sb);
			}
		}

		// Token: 0x040078D7 RID: 30935
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x040078D8 RID: 30936
		[SerializeField]
		private TextMeshProUGUI totalProgress;

		// Token: 0x040078D9 RID: 30937
		[SerializeField]
		private TextMeshProUGUI previewProgress;

		// Token: 0x040078DA RID: 30938
		[Header("当前值")]
		[SerializeField]
		private CImage firstProgressBar;

		// Token: 0x040078DB RID: 30939
		[Header("当前值+下月增加值")]
		[SerializeField]
		private CImage secondProgressBar;

		// Token: 0x040078DC RID: 30940
		[SerializeField]
		private TextMeshProUGUI currentChickenCount;

		// Token: 0x040078DD RID: 30941
		[SerializeField]
		private TextMeshProUGUI currentActionPoint;

		// Token: 0x040078DE RID: 30942
		[SerializeField]
		private TooltipInvoker confirmTips;

		// Token: 0x040078DF RID: 30943
		[SerializeField]
		private TooltipInvoker[] featherIconTips;

		// Token: 0x040078E0 RID: 30944
		private ChickenPluckFeatherDisplayData _data = new ChickenPluckFeatherDisplayData();
	}
}
