using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Building
{
	// Token: 0x0200065B RID: 1627
	public class BuildingExpandTaiwuVillage : MonoBehaviour, IBuildingExpandTaiwuVillageSteleHandler, IBuildingExpandTaiwuVillageSteleProvider
	{
		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06004D7F RID: 19839 RVA: 0x002491B4 File Offset: 0x002473B4
		private BuildingBlockItem ConfigData
		{
			get
			{
				return BuildingBlock.Instance[44];
			}
		}

		// Token: 0x06004D80 RID: 19840 RVA: 0x002491C4 File Offset: 0x002473C4
		private void Awake()
		{
			this.animationController.Bind(this);
			this._steles.Clear();
			BuildingExpandTaiwuVillageStele[] steles = base.GetComponentsInChildren<BuildingExpandTaiwuVillageStele>();
			foreach (BuildingExpandTaiwuVillageStele stele in steles)
			{
				stele.Bind(this);
				this._steles[stele.orgTemplateId] = stele;
			}
		}

		// Token: 0x06004D81 RID: 19841 RVA: 0x00249224 File Offset: 0x00247424
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		}

		// Token: 0x06004D82 RID: 19842 RVA: 0x00249240 File Offset: 0x00247440
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.OnBuildingBlockDataChange));
		}

		// Token: 0x06004D83 RID: 19843 RVA: 0x0024925C File Offset: 0x0024745C
		public void Refresh(UI_BuildingManage parent, BuildingBlockKey key, BuildingBlockData blockData)
		{
			this._parent = parent;
			this._key = key;
			this._buildingModel = SingletonObject.getInstance<BuildingModel>();
			this.RequestGetIsJiaoPoolOpen();
			this.ResetStatus();
		}

		// Token: 0x06004D84 RID: 19844 RVA: 0x00249288 File Offset: 0x00247488
		public void OnConfirm()
		{
			bool flag = this._selectedOrgTemplateId < 0;
			if (!flag)
			{
				this.EnableClickMask();
				Sequence sequence = this.animationController.PlayUnlockEffect(this._buildingBlockData, this._selectedOrgTemplateId);
				sequence.AppendCallback(new TweenCallback(this.RequestUnlockBuildingLevelSlot));
				sequence.PlayForward();
			}
		}

		// Token: 0x06004D85 RID: 19845 RVA: 0x002492DE File Offset: 0x002474DE
		public void OnCancel()
		{
			this.ResetStatus();
		}

		// Token: 0x06004D86 RID: 19846 RVA: 0x002492E8 File Offset: 0x002474E8
		private void OnBuildingBlockDataChange(ArgumentBox argumentBox)
		{
			this.ResetStatus();
		}

		// Token: 0x06004D87 RID: 19847 RVA: 0x002492F4 File Offset: 0x002474F4
		private void ResetStatus()
		{
			this._selectedOrgTemplateId = -1;
			this._buildingBlockData = this._buildingModel.GetTaiwuBuildingData(this._key);
			this.upgrade.gameObject.SetActive(false);
			this.upgrade.Set(this._buildingBlockData, this.ConfigData);
			this.RefreshSteles();
		}

		// Token: 0x06004D88 RID: 19848 RVA: 0x00249351 File Offset: 0x00247551
		private void RequestGetIsJiaoPoolOpen()
		{
			this.upgrade.Set(false);
			ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this._parent, new AsyncMethodCallbackDelegate(this.HandlerGetIsJiaoPoolOpen));
		}

		// Token: 0x06004D89 RID: 19849 RVA: 0x0024937C File Offset: 0x0024757C
		private void HandlerGetIsJiaoPoolOpen(int offset, RawDataPool pool)
		{
			bool jiaoPoolIsOpen = false;
			Serializer.Deserialize(pool, offset, ref jiaoPoolIsOpen);
			this.upgrade.Set(jiaoPoolIsOpen);
		}

		// Token: 0x06004D8A RID: 19850 RVA: 0x002493A4 File Offset: 0x002475A4
		private void RequestUnlockBuildingLevelSlot()
		{
			int i = (int)(this._selectedOrgTemplateId - 1);
			BuildingDomainMethod.AsyncCall.UnlockBuildingLevelSlot(this._parent, this._key, i, new AsyncMethodCallbackDelegate(this.HandlerUnlockBuildingLevelSlot));
		}

		// Token: 0x06004D8B RID: 19851 RVA: 0x002493DA File Offset: 0x002475DA
		private void HandlerUnlockBuildingLevelSlot(int offset, RawDataPool pool)
		{
			this.DisableClickMask();
		}

		// Token: 0x06004D8C RID: 19852 RVA: 0x002493E4 File Offset: 0x002475E4
		private void EnableClickMask()
		{
			this.clickMask.SetActive(true);
			UIManager.Instance.SetEscHandler(new Action(this.InfinityEscHandler));
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x0024940B File Offset: 0x0024760B
		private void DisableClickMask()
		{
			this.clickMask.SetActive(false);
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x00249427 File Offset: 0x00247627
		private void InfinityEscHandler()
		{
			UIManager.Instance.SetEscHandler(new Action(this.InfinityEscHandler));
		}

		// Token: 0x06004D8F RID: 19855 RVA: 0x00249444 File Offset: 0x00247644
		private void RefreshSteles()
		{
			BuildingBlockItem blockConfig = this.ConfigData;
			for (int i = 0; i < (int)blockConfig.MaxLevel; i++)
			{
				bool unlocked = this._buildingBlockData.SlotIsUnlocked(i);
				sbyte orgTemplateId = (sbyte)(i + 1);
				BuildingExpandTaiwuVillageStele stele;
				bool flag = this._steles.TryGetValue(orgTemplateId, out stele);
				if (flag)
				{
					stele.Set(unlocked, this._selectedOrgTemplateId);
				}
			}
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x002494A8 File Offset: 0x002476A8
		private void ShowLockedDialog()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 2,
				Title = LocalStringManager.Get(LanguageKey.LK_Building_ExpandTaiwuVillage_Locked_Dialog_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Building_ExpandTaiwuVillage_Locked_Dialog_Desc)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x00249510 File Offset: 0x00247710
		void IBuildingExpandTaiwuVillageSteleHandler.Handle(sbyte orgTemplateId)
		{
			bool flag = !SingletonObject.getInstance<TaskModel>().IsTaskFinished(31);
			if (flag)
			{
				this.ShowLockedDialog();
			}
			else
			{
				this._selectedOrgTemplateId = orgTemplateId;
				this.upgrade.gameObject.SetActive(this._selectedOrgTemplateId >= 0);
				bool flag2 = this._selectedOrgTemplateId < 0;
				if (!flag2)
				{
					this.upgrade.Set(this._selectedOrgTemplateId);
					this.RefreshSteles();
				}
			}
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x00249588 File Offset: 0x00247788
		void IBuildingExpandTaiwuVillageSteleHandler.Cancel()
		{
			this.ResetStatus();
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x00249594 File Offset: 0x00247794
		BuildingExpandTaiwuVillageStele IBuildingExpandTaiwuVillageSteleProvider.GetStele(sbyte orgTemplateId)
		{
			return this._steles[orgTemplateId];
		}

		// Token: 0x040035BF RID: 13759
		public const int CoreOrganizationCount = 5;

		// Token: 0x040035C0 RID: 13760
		[SerializeField]
		private BuildingExpandTaiwuVillageUpgrade upgrade;

		// Token: 0x040035C1 RID: 13761
		[SerializeField]
		private BuildingExpandTaiwuVillageAnimationController animationController;

		// Token: 0x040035C2 RID: 13762
		[SerializeField]
		private GameObject clickMask;

		// Token: 0x040035C3 RID: 13763
		private UI_BuildingManage _parent;

		// Token: 0x040035C4 RID: 13764
		private BuildingBlockKey _key;

		// Token: 0x040035C5 RID: 13765
		private BuildingModel _buildingModel;

		// Token: 0x040035C6 RID: 13766
		private BuildingBlockData _buildingBlockData;

		// Token: 0x040035C7 RID: 13767
		private sbyte _selectedOrgTemplateId = -1;

		// Token: 0x040035C8 RID: 13768
		private readonly Dictionary<sbyte, BuildingExpandTaiwuVillageStele> _steles = new Dictionary<sbyte, BuildingExpandTaiwuVillageStele>();
	}
}
