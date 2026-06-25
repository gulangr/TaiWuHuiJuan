using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Building.BuildingAreaQuickActionMenu;
using Game.Views.Building.BuildingManage;
using GameData.Domains.Building;
using GameData.Domains.Global;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Buildings
{
	// Token: 0x02000BC4 RID: 3012
	public class ViewSamsaraPlatform : UIBase
	{
		// Token: 0x060097C2 RID: 38850 RVA: 0x0046B353 File Offset: 0x00469553
		private void Awake()
		{
			this.recordBtn.gameObject.SetActive(false);
			this.recordBtn.onClick.ResetListener(delegate()
			{
				BuildingActionUtils.OpenBuildingManage(this.BuildingKey, Game.Views.Building.BuildingManage.BuildingManageTogKey.SamsaraPlatformRecord);
			});
		}

		// Token: 0x060097C3 RID: 38851 RVA: 0x0046B388 File Offset: 0x00469588
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<BuildingBlockKey>("BuildingKey", out this.BuildingKey);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
			{
				AudioManager.Instance.StopSound("ui_industry_reincarnation");
				AudioManager.Instance.PlaySound("ui_industry_reincarnation", true, false);
			}));
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(114);
			}
		}

		// Token: 0x060097C4 RID: 38852 RVA: 0x0046B428 File Offset: 0x00469628
		public override void QuickHide()
		{
			AudioManager.Instance.StopSound("ui_industry_reincarnation");
			bool dataChanged = this.DataChanged;
			if (dataChanged)
			{
				GEvent.OnEvent(UiEvents.SamsaraPlatformRecordDataChange, null);
				GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
				this.DataChanged = false;
			}
			base.QuickHide();
		}

		// Token: 0x060097C5 RID: 38853 RVA: 0x0046B47C File Offset: 0x0046967C
		public void RequestData()
		{
			this.BuildingBlockDataChange(null);
			BuildingDomainMethod.AsyncCall.GetSamsaraPlatformBonusAttributes(this, -1, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._baseAttributes);
				this.samsaraPlatformProperty.Set(this._baseAttributes);
			});
			BuildingDomainMethod.AsyncCall.GetSamsaraPlatformCharList(this, delegate(int offset, RawDataPool pool)
			{
				List<SamsaraPlatformCharDisplayData> samsaraCharList = new List<SamsaraPlatformCharDisplayData>();
				Serializer.Deserialize(pool, offset, ref samsaraCharList);
				this.CharDataDict.Clear();
				foreach (SamsaraPlatformCharDisplayData data in samsaraCharList)
				{
					this.CharDataDict[data.Id] = data;
				}
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x060097C6 RID: 38854 RVA: 0x0046B4B0 File Offset: 0x004696B0
		public void SetAttributeDelta(int charId = -1)
		{
			ViewSamsaraPlatform.<>c__DisplayClass14_0 CS$<>8__locals1 = new ViewSamsaraPlatform.<>c__DisplayClass14_0();
			CS$<>8__locals1.<>4__this = this;
			ViewSamsaraPlatform.<>c__DisplayClass14_0 CS$<>8__locals2 = CS$<>8__locals1;
			int version = this._version + 1;
			this._version = version;
			CS$<>8__locals2.version = version;
			BuildingDomainMethod.AsyncCall.GetSamsaraPlatformBonusAttributes(this, charId, delegate(int offset, RawDataPool pool)
			{
				bool flag = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
				if (!flag)
				{
					Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.<>4__this._fullAttributes);
					CS$<>8__locals1.<>4__this.samsaraPlatformProperty.SetDelta(CS$<>8__locals1.<>4__this._fullAttributes);
				}
			});
		}

		// Token: 0x060097C7 RID: 38855 RVA: 0x0046B4F6 File Offset: 0x004696F6
		public void ResetAttribute()
		{
			this._version++;
			this.samsaraPlatformProperty.Reset();
		}

		// Token: 0x060097C8 RID: 38856 RVA: 0x0046B513 File Offset: 0x00469713
		private void BuildingBlockDataChange(ArgumentBox argbox)
		{
			this.BuildingData = SingletonObject.getInstance<BuildingModel>().GetTaiwuBuildingData(this.BuildingKey);
			this.UpdateBuildingLevel();
		}

		// Token: 0x060097C9 RID: 38857 RVA: 0x0046B533 File Offset: 0x00469733
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.BuildingBlockDataChange));
		}

		// Token: 0x060097CA RID: 38858 RVA: 0x0046B54F File Offset: 0x0046974F
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.BuildingBlockDataChange));
		}

		// Token: 0x060097CB RID: 38859 RVA: 0x0046B56C File Offset: 0x0046976C
		private void UpdateBuildingLevel()
		{
			sbyte type = 0;
			while ((int)type < DestinyType.Instance.Count)
			{
				this.samsaraPlatformButtons[(int)type].Init(this);
				type += 1;
			}
		}

		// Token: 0x04007478 RID: 29816
		[SerializeField]
		private SamsaraPlatformButton[] samsaraPlatformButtons;

		// Token: 0x04007479 RID: 29817
		[SerializeField]
		private SamsaraPlatformProperty samsaraPlatformProperty;

		// Token: 0x0400747A RID: 29818
		[SerializeField]
		private CButton recordBtn;

		// Token: 0x0400747B RID: 29819
		private SamsaraPlatformBonusAttributes _baseAttributes;

		// Token: 0x0400747C RID: 29820
		private SamsaraPlatformBonusAttributes _fullAttributes;

		// Token: 0x0400747D RID: 29821
		[NonSerialized]
		public BuildingBlockData BuildingData;

		// Token: 0x0400747E RID: 29822
		[NonSerialized]
		public BuildingBlockKey BuildingKey;

		// Token: 0x0400747F RID: 29823
		private bool _firstEnter;

		// Token: 0x04007480 RID: 29824
		public bool DataChanged;

		// Token: 0x04007481 RID: 29825
		private int _version;

		// Token: 0x04007482 RID: 29826
		private sbyte _buildingLevel;

		// Token: 0x04007483 RID: 29827
		public readonly Dictionary<int, SamsaraPlatformCharDisplayData> CharDataDict = new Dictionary<int, SamsaraPlatformCharDisplayData>();

		// Token: 0x04007484 RID: 29828
		private sbyte _selectingCharDestiny;
	}
}
