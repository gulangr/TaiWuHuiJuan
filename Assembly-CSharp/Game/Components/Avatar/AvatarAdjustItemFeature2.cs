using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F7A RID: 3962
	public class AvatarAdjustItemFeature2 : AvatarAdjustItemBase
	{
		// Token: 0x0600B5E5 RID: 46565 RVA: 0x0052DB18 File Offset: 0x0052BD18
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

		// Token: 0x0600B5E6 RID: 46566 RVA: 0x0052DBB0 File Offset: 0x0052BDB0
		protected override void Awake()
		{
			base.Awake();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.OnToggleValueChanged = new Action<sbyte>(this.OnToggleValueChanged);
		}

		// Token: 0x0600B5E7 RID: 46567 RVA: 0x0052DBE8 File Offset: 0x0052BDE8
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			this.UpdateColorScroll();
		}

		// Token: 0x0600B5E8 RID: 46568 RVA: 0x0052DC17 File Offset: 0x0052BE17
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.FeatureColors);
		}

		// Token: 0x0600B5E9 RID: 46569 RVA: 0x0052DC4C File Offset: 0x0052BE4C
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

		// Token: 0x0600B5EA RID: 46570 RVA: 0x0052DCD0 File Offset: 0x0052BED0
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._featureList = AvatarGroup.GetFeatureResExcludeDelete(this.Controller.AvatarGroup.Feature2Res);
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._featureList = this.CustomAssetsFilterHandler(this._featureList);
				}
				AvatarAsset featureAsset = this._featureList.Find((AvatarAsset e) => e.Id == base.Data.Feature2Id);
				int selectIndex = (featureAsset == null) ? 1 : (this._featureList.IndexOf(featureAsset) + 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._featureList.Count, 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
			}
		}

		// Token: 0x0600B5EB RID: 46571 RVA: 0x0052DD98 File Offset: 0x0052BF98
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._featureList.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.Feature2Id = this._featureList[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
				this.UpdateColorScroll();
			}
		}

		// Token: 0x0600B5EC RID: 46572 RVA: 0x0052DDF2 File Offset: 0x0052BFF2
		public override void SetColorIndex(int index)
		{
			base.Data.ColorFeature2Id = AvatarAdjustController.FeatureColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5ED RID: 46573 RVA: 0x0052DE24 File Offset: 0x0052C024
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.FeatureColors.Count; i++)
			{
				bool flag = AvatarAdjustController.FeatureColors[i].Item1 == base.Data.ColorFeature2Id;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B5EE RID: 46574 RVA: 0x0052DE78 File Offset: 0x0052C078
		private void UpdateColorScroll()
		{
			Refers color = this.Refers.CGet<Refers>("ColorPrefab");
			AvatarAsset featureAsset = this.Controller.AvatarGroup.Get(EAvatarElementsType.Feature2, new short[]
			{
				base.Data.Feature2Id
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

		// Token: 0x0600B5EF RID: 46575 RVA: 0x0052DF7C File Offset: 0x0052C17C
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.SetValueAndRefresh(this._featureList.FindIndex((AvatarAsset p) => p.Id == base.Data.Feature2Id) + 1);
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
			AvatarAsset avatarAssert = this._featureList.Find((AvatarAsset e) => e.Config.ElementId == AvatarGroup.GetUsefulFeatureId(base.Data.Feature2Id));
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

		// Token: 0x0600B5F0 RID: 46576 RVA: 0x0052E0FC File Offset: 0x0052C2FC
		public void OnToggleValueChanged(sbyte featureMirrorType)
		{
			foreach (Avatar avatar in this.Controller.AvatarList)
			{
				avatar.UpdateFeatureMirrorType(EAvatarElementsType.Feature2, featureMirrorType);
			}
			base.Data.Feature2MirrorType = featureMirrorType;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5F1 RID: 46577 RVA: 0x0052E17C File Offset: 0x0052C37C
		public void SetFeatureToggle(sbyte mirrorType)
		{
			bool flag = mirrorType == -1;
			if (flag)
			{
				AvatarManager manager = SingletonObject.getInstance<AvatarManager>();
				AvatarAsset feature2Asset = manager.GetAsset((int)base.Data.AvatarId, EAvatarElementsType.Feature2, new short[]
				{
					base.Data.Feature2Id
				});
				bool flag2 = feature2Asset != null;
				if (flag2)
				{
					mirrorType = (sbyte)feature2Asset.Config.DefaultMirrorType;
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

		// Token: 0x04008D5B RID: 36187
		private List<AvatarAsset> _featureList;

		// Token: 0x04008D5C RID: 36188
		public Func<List<AvatarAsset>, List<AvatarAsset>> CustomAssetsFilterHandler;
	}
}
