using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F70 RID: 3952
	public class AvatarAdjustItemBackHair : AvatarAdjustItemBase
	{
		// Token: 0x0600B53A RID: 46394 RVA: 0x00529C14 File Offset: 0x00527E14
		protected override void Start()
		{
			this.OnlyCreateRes = true;
			this.UpdateAssetCore();
			bool flag = this._frontHairAsset == null || this._frontHairAsset.Config.DisableRelativeType;
			if (!flag)
			{
				IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
				idSwitch.OnValueChanged = (Action<int>)Delegate.Combine(idSwitch.OnValueChanged, new Action<int>(delegate(int delta)
				{
					this.OnQuickAdjustTriggered(0);
				}));
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").GetComponent<CToggleGroupObsolete>().OnActiveToggleChange = delegate(CToggleObsolete n, CToggleObsolete o)
				{
					this.OnQuickAdjustTriggered(0);
				};
				bool flag2 = null != this.Controller;
				if (flag2)
				{
					this.OnQuickAdjustTriggered(0);
				}
				bool flag3 = null != this.ShaveBaldToggle;
				if (flag3)
				{
					this.SetDisableStyle(this.ShaveBaldToggle.interactable);
					this.ShaveBaldToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShaveBaldValueChanged));
				}
			}
		}

		// Token: 0x0600B53B RID: 46395 RVA: 0x00529D08 File Offset: 0x00527F08
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			bool flag = this._frontHairAsset == null || this._frontHairAsset.Config.DisableRelativeType;
			if (!flag)
			{
				base.OnOpen(anim);
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			}
		}

		// Token: 0x0600B53C RID: 46396 RVA: 0x00529D60 File Offset: 0x00527F60
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			this.Refers.CGet<CToggleObsolete>("LockSwitch").onValueChanged.RemoveAllListeners();
			this.Refers.CGet<CToggleObsolete>("LockSwitch").isOn = true;
			this.Refers.CGet<CToggleObsolete>("LockSwitch").onValueChanged.AddListener(delegate(bool val)
			{
				this.Controller.UpdateColorLockGroup(this);
			});
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.HairColors);
		}

		// Token: 0x0600B53D RID: 46397 RVA: 0x00529DF8 File Offset: 0x00527FF8
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
					avatar.UpdateBackHair();
					avatar.UpdateFrontHair();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B53E RID: 46398 RVA: 0x00529E84 File Offset: 0x00528084
		private void UpdateAssetCore()
		{
			bool flag = null != this.Controller;
			if (flag)
			{
				this._frontHairAsset = this.Controller.AvatarGroup.Get(EAvatarElementsType.Hair1, new short[]
				{
					base.Data.FrontHairId
				});
			}
			bool flag2 = this._frontHairAsset == null || this._frontHairAsset.Config.DisableRelativeType;
			if (flag2)
			{
				base.Close(true);
			}
			else
			{
				this._backHairResList = this.Controller.AvatarGroup.Hair2Res.FindAll((HairRes e) => !this.OnlyCreateRes || e.Hair.Config.CanCreate);
				List<uint> excludeByFrontHairIdList = new List<uint>();
				bool flag3 = this._frontHairAsset.Config.BanElements != null;
				if (flag3)
				{
					excludeByFrontHairIdList.AddRange(this._frontHairAsset.Config.BanElements);
				}
				this._backHairResList.RemoveAll((HairRes e) => excludeByFrontHairIdList.Contains(e.Hair.Config.TemplateId));
				bool flag4 = this.CustomAssetsFilterHandler != null;
				if (flag4)
				{
					this._backHairResList = this.CustomAssetsFilterHandler(this._backHairResList);
				}
				HairRes hairRes = this._backHairResList.Find((HairRes e) => e.Id == this.Data.BackHairId);
				int selectIndex = 1;
				bool flag5 = hairRes != null;
				if (flag5)
				{
					selectIndex = this._backHairResList.IndexOf(hairRes) + 1;
				}
				this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._backHairResList.Count, 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
			}
		}

		// Token: 0x0600B53F RID: 46399 RVA: 0x0052A020 File Offset: 0x00528220
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._backHairResList.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.BackHairId = this._backHairResList[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B540 RID: 46400 RVA: 0x0052A074 File Offset: 0x00528274
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.HairColors.Count; i++)
			{
				bool flag = AvatarAdjustController.HairColors[i].Item1 == base.Data.ColorBackHairId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B541 RID: 46401 RVA: 0x0052A0C7 File Offset: 0x005282C7
		public override void SetColorIndex(int index)
		{
			base.Data.ColorBackHairId = AvatarAdjustController.HairColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B542 RID: 46402 RVA: 0x0052A0F8 File Offset: 0x005282F8
		public override void OnQuickAdjustTriggered(int delta)
		{
			bool flag = this._backHairResList == null;
			if (!flag)
			{
				int currIndex = this.GetColorIndex();
				bool flag2 = this._backHairResList != null;
				if (flag2)
				{
					this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._backHairResList.FindIndex((HairRes p) => p.Id == base.Data.BackHairId) + 1);
				}
				bool flag3 = delta < 0;
				if (flag3)
				{
					this.Refers.CGet<IdSwitch>("IDSwitch").BtnPrevId.onClick.Invoke();
				}
				else
				{
					bool flag4 = delta > 0;
					if (flag4)
					{
						this.Refers.CGet<IdSwitch>("IDSwitch").BtnNextId.onClick.Invoke();
					}
				}
				Refers color = this.Refers.CGet<Refers>("ColorPrefab");
				bool flag5 = color != null;
				if (flag5)
				{
					base.OnColorPrefabRender(currIndex, color);
				}
				bool flag6 = this.Refers.Names.Contains("SimpleInfo");
				if (flag6)
				{
					this.Refers.CGet<TextMeshProUGUI>("SimpleInfo").SetText(this.Refers.CGet<IdSwitch>("IDSwitch").IdValue.text, true);
				}
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			}
		}

		// Token: 0x0600B543 RID: 46403 RVA: 0x0052A23C File Offset: 0x0052843C
		public void SetDisableStyle(bool isEnable)
		{
			this._shaveBaldDisableStyleRoot = this.ShaveBaldToggle.GetComponent<DisableStyleRoot>();
			bool flag = null != this._shaveBaldDisableStyleRoot;
			if (flag)
			{
				this._shaveBaldDisableStyleRoot.SetStyleEffect(!isEnable, false);
			}
			TooltipInvoker mouseTip = this.ShaveBaldToggle.GetComponent<TooltipInvoker>();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.PresetParam[1] = (isEnable ? "UI_LK_ShaveBald_Enable" : "UI_LK_ShaveBald_Disable");
			}
		}

		// Token: 0x0600B544 RID: 46404 RVA: 0x0052A2AC File Offset: 0x005284AC
		private void OnShaveBaldValueChanged(bool isOn)
		{
			base.Data.SetGrowableElementShowingState(0, !isOn);
			bool flag = null != this.Controller.AvatarAdjustItemFrontHair && null != this.Controller.AvatarAdjustItemFrontHair.ShaveBaldToggle;
			if (flag)
			{
				this.Controller.AvatarAdjustItemFrontHair.ShaveBaldToggle.SetIsOnWithoutNotify(isOn);
			}
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x04008D33 RID: 36147
		public bool OnlyCreateRes;

		// Token: 0x04008D34 RID: 36148
		private List<HairRes> _backHairResList;

		// Token: 0x04008D35 RID: 36149
		private AvatarAsset _frontHairAsset;

		// Token: 0x04008D36 RID: 36150
		public Func<List<HairRes>, List<HairRes>> CustomAssetsFilterHandler;
	}
}
