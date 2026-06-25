using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F76 RID: 3958
	public class AvatarAdjustItemCloth : AvatarAdjustItemBase
	{
		// Token: 0x0600B597 RID: 46487 RVA: 0x0052BA30 File Offset: 0x00529C30
		protected override void Start()
		{
			this.OnlyCreateRes = true;
			this.UpdateAssetCore();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.OnValueChanged = (Action<int>)Delegate.Combine(idSwitch.OnValueChanged, new Action<int>(delegate(int delta)
			{
				this.OnQuickAdjustTriggered(0);
			}));
			bool flag = !this.LockColorChoose;
			if (flag)
			{
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").GetComponent<CToggleGroupObsolete>().OnActiveToggleChange = delegate(CToggleObsolete n, CToggleObsolete o)
				{
					this.OnQuickAdjustTriggered(0);
				};
			}
			bool flag2 = null != this.Controller;
			if (flag2)
			{
				this.OnQuickAdjustTriggered(0);
			}
			base.Close(false);
		}

		// Token: 0x0600B598 RID: 46488 RVA: 0x0052BAD4 File Offset: 0x00529CD4
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").gameObject.SetActive(!this.LockColorChoose);
			bool flag = this.Refers.Names.Contains("ColorDisable");
			if (flag)
			{
				this.Refers.CGet<GameObject>("ColorDisable").gameObject.SetActive(this.LockColorChoose);
			}
			bool flag2 = !this.LockColorChoose;
			if (flag2)
			{
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			}
		}

		// Token: 0x0600B599 RID: 46489 RVA: 0x0052BB74 File Offset: 0x00529D74
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			this._allBodyRes = new List<BodyRes>();
			bool flag = !this.LockColorChoose;
			if (flag)
			{
				base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.ClothColors);
			}
		}

		// Token: 0x0600B59A RID: 46490 RVA: 0x0052BBCC File Offset: 0x00529DCC
		private void ArgsUpdateCallback()
		{
			foreach (Avatar avatar in this.Controller.AvatarList)
			{
				bool flag = this.Controller.GetAge() < 16;
				if (flag)
				{
					avatar.Refresh();
				}
				else
				{
					avatar.UpdateCloth();
					avatar.UpdateFrontHair();
					avatar.UpdateBackHair();
				}
			}
			this.OnQuickAdjustTriggered(0);
			this.RefreshClothInfo();
		}

		// Token: 0x0600B59B RID: 46491 RVA: 0x0052BC64 File Offset: 0x00529E64
		private void RefreshClothInfo()
		{
			bool flag = this.Refers.Names.Contains("ClothView");
			if (flag)
			{
				Refers clothViewRefers = this.Refers.CGet<Refers>("ClothView");
				bool flag2 = !clothViewRefers || !clothViewRefers.gameObject.activeSelf;
				if (!flag2)
				{
					short clothId = ItemTemplateHelper.GetClothingTemplateIdByDisplayId((byte)base.Data.ClothDisplayId);
					clothViewRefers.CGet<CImage>("ClothIcon").SetSprite(ItemTemplateHelper.GetIcon(3, clothId), false, null);
					clothViewRefers.CGet<CImage>("ClothBg").color = Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(3, clothId)];
					clothViewRefers.CGet<TextMeshProUGUI>("ClothName").SetText(ItemTemplateHelper.GetName(3, clothId), true);
					TooltipInvoker tipDisPlayer = clothViewRefers.CGet<TooltipInvoker>("MouseTip");
					ItemDisplayData displayData = new ItemDisplayData();
					displayData.Key = new ItemKey(3, 0, clothId, 0);
					tipDisPlayer.Type = TooltipManager.ItemTypeToTipType[displayData.Key.ItemType];
					tipDisPlayer.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("ItemData", displayData);
				}
			}
		}

		// Token: 0x0600B59C RID: 46492 RVA: 0x0052BD8C File Offset: 0x00529F8C
		private void UpdateAssetCore()
		{
			this._allBodyRes.Clear();
			bool flag = null == this.Controller;
			if (!flag)
			{
				List<BodyRes> bodyResList = this.Controller.AvatarGroup.BodyRes;
				bool flag2 = this.Controller.GetAge() < 16;
				BodyRes bodyRes;
				if (flag2)
				{
					byte childAvatarId = SingletonObject.getInstance<AvatarManager>().GetChildAvatarIdByAvatarId(base.Data.AvatarId);
					bodyResList = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)childAvatarId).BodyRes;
					this._allBodyRes.AddRange(bodyResList);
					bodyRes = this._allBodyRes.Find((BodyRes e) => (int)e.Id == (int)this.Data.ClothDisplayId % bodyResList.Count + 1);
				}
				else
				{
					bool flag3 = this.CustomAssetsFilterHandler != null;
					if (flag3)
					{
						this._allBodyRes = this.CustomAssetsFilterHandler(bodyResList);
					}
					else
					{
						this._allBodyRes.AddRange(bodyResList.FindAll((BodyRes e) => e.Cloth.Config.ElementId != 0 && (!this.OnlyCreateRes || e.Cloth.Config.CanCreate)));
					}
					bodyRes = this._allBodyRes.Find((BodyRes e) => e.Id == this.Data.ClothDisplayId);
					bool flag4 = bodyRes == null && this._allBodyRes.CheckIndex((int)base.Data.ClothDisplayId);
					if (flag4)
					{
						bodyRes = this._allBodyRes[(int)base.Data.ClothDisplayId];
					}
				}
				int selectIndex = (bodyRes != null) ? (this._allBodyRes.IndexOf(bodyRes) + 1) : 0;
				selectIndex = Mathf.Max(selectIndex, 0);
				this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._allBodyRes.Count, 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				this.RefreshClothInfo();
			}
		}

		// Token: 0x0600B59D RID: 46493 RVA: 0x0052BF54 File Offset: 0x0052A154
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._allBodyRes.CheckIndex(newIndex);
			if (flag)
			{
				short newId = this._allBodyRes[newIndex].Id;
				bool flag2 = newId != base.Data.ClothDisplayId;
				if (flag2)
				{
					base.Data.ClothDisplayId = newId;
					Action onArgsUpdate = this.OnArgsUpdate;
					if (onArgsUpdate != null)
					{
						onArgsUpdate();
					}
				}
			}
		}

		// Token: 0x0600B59E RID: 46494 RVA: 0x0052BFC0 File Offset: 0x0052A1C0
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.ClothColors.Count; i++)
			{
				bool flag = AvatarAdjustController.ClothColors[i].Item1 == base.Data.ColorClothId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B59F RID: 46495 RVA: 0x0052C013 File Offset: 0x0052A213
		public override void SetColorIndex(int index)
		{
			base.Data.ColorClothId = AvatarAdjustController.ClothColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5A0 RID: 46496 RVA: 0x0052C044 File Offset: 0x0052A244
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._allBodyRes.FindIndex((BodyRes p) => p.Id == base.Data.ClothDisplayId) + 1);
			bool flag = delta < 0;
			if (flag)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").BtnPrevId.onClick.Invoke();
			}
			else
			{
				bool flag2 = delta > 0;
				if (flag2)
				{
					this.Refers.CGet<IdSwitch>("IDSwitch").BtnNextId.onClick.Invoke();
				}
			}
			bool flag3 = this.Refers.Names.Contains("ColorPrefab");
			if (flag3)
			{
				base.OnColorPrefabRender(currIndex, this.Refers.CGet<Refers>("ColorPrefab"));
			}
			bool flag4 = this.Refers.Names.Contains("SimpleInfo");
			if (flag4)
			{
				this.Refers.CGet<TextMeshProUGUI>("SimpleInfo").SetText(this.Refers.CGet<IdSwitch>("IDSwitch").IdValue.text, true);
			}
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x04008D51 RID: 36177
		public bool LockColorChoose;

		// Token: 0x04008D52 RID: 36178
		public bool OnlyCreateRes;

		// Token: 0x04008D53 RID: 36179
		private List<BodyRes> _allBodyRes = new List<BodyRes>();

		// Token: 0x04008D54 RID: 36180
		public Func<List<BodyRes>, List<BodyRes>> CustomAssetsFilterHandler;
	}
}
