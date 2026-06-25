using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F79 RID: 3961
	public class AvatarAdjustItemFeature1 : AvatarAdjustItemBase
	{
		// Token: 0x0600B5D2 RID: 46546 RVA: 0x0052D35C File Offset: 0x0052B55C
		protected override void Start()
		{
			this.UpdateAssetCore();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.OnValueChanged = (Action<int>)Delegate.Combine(idSwitch.OnValueChanged, new Action<int>(delegate(int delta)
			{
				this.OnQuickAdjustTriggered(0);
			}));
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").GetComponent<CToggleGroupObsolete>().OnActiveToggleChange = delegate(CToggleObsolete n, CToggleObsolete o)
			{
				this.OnQuickAdjustTriggered(0);
			};
			bool flag = null != this.Controller;
			if (flag)
			{
				this.UpdateColorScroll();
				this.OnQuickAdjustTriggered(0);
			}
			base.Close(false);
		}

		// Token: 0x0600B5D3 RID: 46547 RVA: 0x0052D3F4 File Offset: 0x0052B5F4
		protected override void Awake()
		{
			base.Awake();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.OnToggleValueChanged = new Action<sbyte>(this.OnToggleValueChanged);
		}

		// Token: 0x0600B5D4 RID: 46548 RVA: 0x0052D42C File Offset: 0x0052B62C
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			this.UpdateColorScroll();
		}

		// Token: 0x0600B5D5 RID: 46549 RVA: 0x0052D45B File Offset: 0x0052B65B
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.FeatureColors);
		}

		// Token: 0x0600B5D6 RID: 46550 RVA: 0x0052D490 File Offset: 0x0052B690
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
					avatar.UpdateFeature();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B5D7 RID: 46551 RVA: 0x0052D514 File Offset: 0x0052B714
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._featureList = AvatarGroup.GetFeatureResExcludeDelete(this.Controller.AvatarGroup.Feature1Res);
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._featureList = this.CustomAssetsFilterHandler(this._featureList);
				}
				AvatarAsset featureAsset = this._featureList.Find((AvatarAsset e) => e.Id == base.Data.Feature1Id);
				int selectIndex = (featureAsset == null) ? 1 : (this._featureList.IndexOf(featureAsset) + 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._featureList.Count, 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
			}
		}

		// Token: 0x0600B5D8 RID: 46552 RVA: 0x0052D5DC File Offset: 0x0052B7DC
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._featureList.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.Feature1Id = this._featureList[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
				this.UpdateColorScroll();
			}
		}

		// Token: 0x0600B5D9 RID: 46553 RVA: 0x0052D636 File Offset: 0x0052B836
		public override void SetColorIndex(int index)
		{
			base.Data.ColorFeature1Id = AvatarAdjustController.FeatureColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5DA RID: 46554 RVA: 0x0052D668 File Offset: 0x0052B868
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.FeatureColors.Count; i++)
			{
				bool flag = AvatarAdjustController.FeatureColors[i].Item1 == base.Data.ColorFeature1Id;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B5DB RID: 46555 RVA: 0x0052D6BC File Offset: 0x0052B8BC
		private void UpdateColorScroll()
		{
			Refers color = this.Refers.CGet<Refers>("ColorPrefab");
			AvatarAsset featureAsset = this.Controller.AvatarGroup.Get(EAvatarElementsType.Feature1, new short[]
			{
				base.Data.Feature1Id
			});
			bool flag = featureAsset != null;
			if (flag)
			{
				bool staticColor = featureAsset.Config.ColorGroup == 0;
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").gameObject.SetActive(!staticColor);
				this.Refers.CGet<GameObject>("ColorDisable").SetActive(staticColor);
				bool flag2 = color != null;
				if (flag2)
				{
					color.gameObject.SetActive(!staticColor);
				}
			}
			else
			{
				this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").gameObject.SetActive(false);
				this.Refers.CGet<GameObject>("ColorDisable").SetActive(true);
				bool flag3 = color != null;
				if (flag3)
				{
					color.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600B5DC RID: 46556 RVA: 0x0052D7C0 File Offset: 0x0052B9C0
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.SetValueAndRefresh(this._featureList.FindIndex((AvatarAsset p) => p.Id == base.Data.Feature1Id) + 1);
			bool flag = delta < 0;
			if (flag)
			{
				idSwitch.BtnPrevId.onClick.Invoke();
			}
			else
			{
				bool flag2 = delta > 0;
				if (flag2)
				{
					idSwitch.BtnNextId.onClick.Invoke();
				}
			}
			Refers color = this.Refers.CGet<Refers>("ColorPrefab");
			bool flag3 = color != null;
			if (flag3)
			{
				base.OnColorPrefabRender(currIndex, color);
			}
			bool flag4 = this.Refers.Names.Contains("SimpleInfo");
			if (flag4)
			{
				this.Refers.CGet<TextMeshProUGUI>("SimpleInfo").SetText(idSwitch.IdValue.text, true);
			}
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			AvatarAsset avatarAssert = this._featureList.Find((AvatarAsset e) => e.Config.ElementId == AvatarGroup.GetUsefulFeatureId(base.Data.Feature1Id));
			bool flag5 = idSwitch.FeatureLeftToggle != null && avatarAssert != null;
			if (flag5)
			{
				idSwitch.FeatureLeftToggle.gameObject.SetActive(avatarAssert.Config.CanMirror);
			}
			bool flag6 = idSwitch.FeatureRightToggle != null && avatarAssert != null;
			if (flag6)
			{
				idSwitch.FeatureRightToggle.gameObject.SetActive(avatarAssert.Config.CanMirror);
			}
		}

		// Token: 0x0600B5DD RID: 46557 RVA: 0x0052D940 File Offset: 0x0052BB40
		public void OnToggleValueChanged(sbyte featureMirrorType)
		{
			foreach (Avatar avatar in this.Controller.AvatarList)
			{
				avatar.UpdateFeatureMirrorType(EAvatarElementsType.Feature1, featureMirrorType);
			}
			base.Data.Feature1MirrorType = featureMirrorType;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5DE RID: 46558 RVA: 0x0052D9C0 File Offset: 0x0052BBC0
		public void SetFeatureToggle(sbyte mirrorType)
		{
			bool flag = mirrorType == -1;
			if (flag)
			{
				AvatarManager manager = SingletonObject.getInstance<AvatarManager>();
				AvatarAsset feature1Asset = manager.GetAsset((int)base.Data.AvatarId, EAvatarElementsType.Feature1, new short[]
				{
					base.Data.Feature1Id
				});
				bool flag2 = feature1Asset != null;
				if (flag2)
				{
					mirrorType = (sbyte)feature1Asset.Config.DefaultMirrorType;
				}
			}
			bool leftIsOn = mirrorType == 0 || mirrorType == 2;
			bool rightIsOn = mirrorType == 1 || mirrorType == 2;
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			bool flag3 = idSwitch.FeatureLeftToggle != null;
			if (flag3)
			{
				idSwitch.FeatureLeftToggle.isOn = leftIsOn;
			}
			bool flag4 = idSwitch.FeatureRightToggle != null;
			if (flag4)
			{
				idSwitch.FeatureRightToggle.isOn = rightIsOn;
			}
			bool flag5 = mirrorType == -1;
			if (flag5)
			{
				idSwitch.FeatureLeftToggle.isOn = true;
			}
			this.UpdateAssetCore();
			this.OnToggleValueChanged(mirrorType);
		}

		// Token: 0x04008D59 RID: 36185
		private List<AvatarAsset> _featureList;

		// Token: 0x04008D5A RID: 36186
		public Func<List<AvatarAsset>, List<AvatarAsset>> CustomAssetsFilterHandler;
	}
}
