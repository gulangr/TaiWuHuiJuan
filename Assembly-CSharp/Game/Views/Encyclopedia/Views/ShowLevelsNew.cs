using System;
using System.Diagnostics.CodeAnalysis;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Elements;
using Game.Views.Encyclopedia.Save;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A72 RID: 2674
	public class ShowLevelsNew : MonoBehaviour
	{
		// Token: 0x060083AA RID: 33706 RVA: 0x003D4770 File Offset: 0x003D2970
		public void SetInit()
		{
			bool changing = this._changing;
			if (!changing)
			{
				bool flag = Save.SaveData == null;
				if (flag)
				{
					Save.LoadInfo();
				}
				bool flag2 = Save.SaveData == null;
				if (!flag2)
				{
					Save.SaveData.Inited = true;
					Save.SaveInfo();
				}
			}
		}

		// Token: 0x060083AB RID: 33707 RVA: 0x003D47BB File Offset: 0x003D29BB
		public void OnEnable()
		{
			this.Init(null);
		}

		// Token: 0x060083AC RID: 33708 RVA: 0x003D47C8 File Offset: 0x003D29C8
		public void Init([MaybeNull] NodeData nodeData)
		{
			bool flag = nodeData != null;
			if (flag)
			{
				this._nodeData = nodeData;
			}
			NodeData nodeData2 = this._nodeData;
			EEncyclopediaContentLevel showLevel2 = Save.GetShowLevel((nodeData2 != null && nodeData2.Id != -1) ? nodeData : null, true);
			NodeData nodeData3 = this._nodeData;
			EEncyclopediaContentLevel showLevel = showLevel2 | ((nodeData3 != null) ? nodeData3.RecursiveTempShowLevel : EEncyclopediaContentLevel.None);
			nodeData2 = this._nodeData;
			EEncyclopediaContentLevel selfLevel = (nodeData2 != null && nodeData2.Id != -1) ? this._nodeData.Level : EEncyclopediaContentLevel.LowMidHigh;
			bool flag2 = this.isLevelFour;
			if (flag2)
			{
				this.RefreshLevelFour();
			}
			else
			{
				this.lowTog.interactable = true;
				this.midTog.interactable = ((selfLevel & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None);
				this.highTog.interactable = ((selfLevel & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None);
			}
			this._changing = true;
			bool flag3 = this.isLevelFour;
			if (flag3)
			{
				this.RefreshLevelFour();
			}
			else
			{
				this.RefreshToggleStates(showLevel);
			}
			this._changing = false;
		}

		// Token: 0x060083AD RID: 33709 RVA: 0x003D48AC File Offset: 0x003D2AAC
		public void Change(bool isRecursive, EEncyclopediaContentLevel showLevel)
		{
			bool changing = this._changing;
			if (!changing)
			{
				NodeData nodeData = this._nodeData;
				bool flag = nodeData != null && nodeData.Id != -1;
				if (flag)
				{
					this._nodeData.TempShowLevel = EEncyclopediaContentLevel.None;
				}
				showLevel &= ~EEncyclopediaContentLevel.Inherit;
				nodeData = this._nodeData;
				bool flag2 = nodeData != null && nodeData.Id != -1;
				if (flag2)
				{
					bool flag3 = this.isLevelFour;
					if (flag3)
					{
						this._nodeData.TempShowLevel = showLevel;
					}
					else
					{
						Save.SetShowLevel(this._nodeData, showLevel);
					}
				}
				else
				{
					Save.SetGlobalShowLevel(showLevel);
				}
				int selectedId = -1;
				PageDetailElement pageDetailElement = this.parent;
				bool flag4 = pageDetailElement != null && pageDetailElement.gameObject.activeInHierarchy;
				if (flag4)
				{
					selectedId = this.parent.PushSelectedId();
					bool flag5 = !this.isLevelFour;
					if (flag5)
					{
						bool currStatus = this.parent.resetCurrentNodeDataShowLevel;
						this.parent.resetCurrentNodeDataShowLevel = true;
						this.parent.Init(this.parent.NodeData, null);
						this.parent.resetCurrentNodeDataShowLevel = currStatus;
					}
					else
					{
						this.parent.Init(this.parent.NodeData, null);
					}
					PageDetailElement pageDetailElement2 = this.parent;
					BasicInfoView instance = BasicInfoView.Instance;
					pageDetailElement2.RefreshSearchResultHighlight((instance != null) ? instance.Searcher : null, false);
				}
				BasicInfoView instance2 = BasicInfoView.Instance;
				if (instance2 != null)
				{
					instance2.RefreshPageDetail();
				}
				this._changing = true;
				bool flag6 = this.isLevelFour;
				if (flag6)
				{
					this.RefreshLevelFour();
				}
				else
				{
					this.RefreshToggleStates(showLevel);
				}
				this._changing = false;
				bool flag7 = !isRecursive;
				if (flag7)
				{
					BasicInfoView instance3 = BasicInfoView.Instance;
					if (instance3 != null)
					{
						instance3.RefreshLevelTwoTitle(false);
					}
				}
				bool flag8 = selectedId != -1;
				if (flag8)
				{
					this.parent.PopSelectedId(selectedId);
				}
			}
		}

		// Token: 0x060083AE RID: 33710 RVA: 0x003D4A70 File Offset: 0x003D2C70
		private void RefreshToggleStates(EEncyclopediaContentLevel showLevel)
		{
			int levelInt = (int)(showLevel & EEncyclopediaContentLevel.LowMidHigh);
			this.highTog.isOn = (this.highTog.interactable && levelInt > 3);
			this.midTog.isOn = (!this.highTog.isOn && this.midTog.interactable && levelInt > 1);
			this.lowTog.isOn = (!this.highTog.isOn && !this.midTog.isOn);
		}

		// Token: 0x060083AF RID: 33711 RVA: 0x003D4AF9 File Offset: 0x003D2CF9
		public void ChangeLow(bool current)
		{
			this.ChangeImpl(false, EEncyclopediaContentLevel.Low);
		}

		// Token: 0x060083B0 RID: 33712 RVA: 0x003D4B04 File Offset: 0x003D2D04
		public void ChangeMid(bool current)
		{
			this.ChangeImpl(false, EEncyclopediaContentLevel.LowMid);
		}

		// Token: 0x060083B1 RID: 33713 RVA: 0x003D4B0F File Offset: 0x003D2D0F
		public void ChangeHigh(bool current)
		{
			this.ChangeImpl(false, EEncyclopediaContentLevel.LowMidHigh);
		}

		// Token: 0x060083B2 RID: 33714 RVA: 0x003D4B1C File Offset: 0x003D2D1C
		public void ChangeImpl(bool isOn, EEncyclopediaContentLevel level)
		{
			bool changing = this._changing;
			if (!changing)
			{
				this.Change(isOn, level);
			}
		}

		// Token: 0x060083B3 RID: 33715 RVA: 0x003D4B40 File Offset: 0x003D2D40
		public void ResetState(bool _)
		{
			this.Change(true, EEncyclopediaContentLevel.Inherit);
			this._changing = true;
			EEncyclopediaContentLevel showLevel = Save.GetShowLevel(this._nodeData, true);
			this.RefreshToggleStates(showLevel);
			this._changing = false;
		}

		// Token: 0x060083B4 RID: 33716 RVA: 0x003D4B7C File Offset: 0x003D2D7C
		public static void ResetAll()
		{
			byte status = Save.SaveData.GlobalLabelStatus;
			Save.SaveData.LabelStatus.Clear();
			Save.SaveData.GlobalLabelStatus = status;
		}

		// Token: 0x060083B5 RID: 33717 RVA: 0x003D4BB4 File Offset: 0x003D2DB4
		public void RefreshLevelFour()
		{
			this.lowTog.SetIsOnWithoutNotify(false);
			this.midTog.SetIsOnWithoutNotify(false);
			this.highTog.SetIsOnWithoutNotify(false);
			this.lowTog.gameObject.SetActive(false);
			this.midTog.gameObject.SetActive(this._nodeData != null && ((this._nodeData.RecursiveTempShowLevel | Save.GetShowLevel(this._nodeData, true)) & EEncyclopediaContentLevel.Mid) == EEncyclopediaContentLevel.None && (this._nodeData.ConfigItem.Level & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None);
			this.highTog.gameObject.SetActive(this._nodeData != null && ((this._nodeData.RecursiveTempShowLevel | Save.GetShowLevel(this._nodeData, true)) & EEncyclopediaContentLevel.High) == EEncyclopediaContentLevel.None && (this._nodeData.ConfigItem.Level & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None);
		}

		// Token: 0x040064D7 RID: 25815
		[SerializeField]
		private bool isLevelFour;

		// Token: 0x040064D8 RID: 25816
		[SerializeField]
		private CToggle lowTog;

		// Token: 0x040064D9 RID: 25817
		[SerializeField]
		private CToggle midTog;

		// Token: 0x040064DA RID: 25818
		[SerializeField]
		private CToggle highTog;

		// Token: 0x040064DB RID: 25819
		[SerializeField]
		[CanBeNull]
		private PageDetailElement parent;

		// Token: 0x040064DC RID: 25820
		private bool _changing;

		// Token: 0x040064DD RID: 25821
		[CanBeNull]
		private NodeData _nodeData;
	}
}
