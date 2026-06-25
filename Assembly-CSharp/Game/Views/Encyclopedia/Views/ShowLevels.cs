using System;
using System.Diagnostics.CodeAnalysis;
using Game.Views.Encyclopedia.Elements;
using Game.Views.Encyclopedia.Save;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A71 RID: 2673
	public class ShowLevels : MonoBehaviour
	{
		// Token: 0x0600839D RID: 33693 RVA: 0x003D406C File Offset: 0x003D226C
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

		// Token: 0x0600839E RID: 33694 RVA: 0x003D40B7 File Offset: 0x003D22B7
		public void OnEnable()
		{
			this.Init(null);
		}

		// Token: 0x0600839F RID: 33695 RVA: 0x003D40C4 File Offset: 0x003D22C4
		public void Init([MaybeNull] NodeData nodeData)
		{
			bool flag = nodeData != null;
			if (flag)
			{
				this.NodeData = nodeData;
			}
			bool flag2 = this.child != null && this.child.gameObject.activeInHierarchy;
			if (flag2)
			{
				this.child.Init(null);
			}
			NodeData nodeData2 = this.NodeData;
			EEncyclopediaContentLevel showLevel2 = Save.GetShowLevel((nodeData2 != null && nodeData2.Id != -1) ? nodeData : null, true);
			NodeData nodeData3 = this.NodeData;
			EEncyclopediaContentLevel showLevel = showLevel2 | ((nodeData3 != null) ? nodeData3.RecursiveTempShowLevel : EEncyclopediaContentLevel.None);
			nodeData2 = this.NodeData;
			EEncyclopediaContentLevel selfLevel = (nodeData2 != null && nodeData2.Id != -1) ? this.NodeData.Level : EEncyclopediaContentLevel.LowMidHigh;
			bool flag3 = this.isLevelFour;
			if (flag3)
			{
				this.RefreshLevelFour();
			}
			else
			{
				this.Low.interactable = true;
				this.Mid.interactable = ((selfLevel & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None);
				this.High.interactable = ((selfLevel & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None);
				this.lowStyle.SetStyleEffect(!this.Low.interactable, false);
				this.midStyle.SetStyleEffect(!this.Mid.interactable, false);
				this.highStyle.SetStyleEffect(!this.High.interactable, false);
				this.LowBg.gameObject.SetActive(this.Low.isOn);
				this.MidBg.gameObject.SetActive(this.Mid.isOn);
				this.HighBg.gameObject.SetActive(this.High.isOn);
			}
			this._changing = true;
			bool flag4 = this.isLevelFour;
			if (flag4)
			{
				this.RefreshLevelFour();
			}
			else
			{
				this.RefreshToggleStates(showLevel);
			}
			this._changing = false;
		}

		// Token: 0x060083A0 RID: 33696 RVA: 0x003D4288 File Offset: 0x003D2488
		public void Change(bool isRecursive, EEncyclopediaContentLevel showLevel)
		{
			bool changing = this._changing;
			if (!changing)
			{
				NodeData nodeData = this.NodeData;
				bool flag = nodeData != null && nodeData.Id != -1;
				if (flag)
				{
					this.NodeData.TempShowLevel = EEncyclopediaContentLevel.None;
				}
				showLevel &= ~EEncyclopediaContentLevel.Inherit;
				nodeData = this.NodeData;
				bool flag2 = nodeData != null && nodeData.Id != -1;
				if (flag2)
				{
					bool flag3 = this.isLevelFour;
					if (flag3)
					{
						this.NodeData.TempShowLevel = showLevel;
					}
					else
					{
						Save.SetShowLevel(this.NodeData, showLevel);
					}
				}
				else
				{
					Save.SetGlobalShowLevel(showLevel);
				}
				int selectedId = -1;
				PageDetailElement parent = this.Parent;
				bool flag4 = parent != null && parent.gameObject.activeInHierarchy;
				if (flag4)
				{
					selectedId = this.Parent.PushSelectedId();
					bool flag5 = !this.isLevelFour;
					if (flag5)
					{
						bool currStatus = this.Parent.resetCurrentNodeDataShowLevel;
						this.Parent.resetCurrentNodeDataShowLevel = true;
						this.Parent.Init(this.Parent.NodeData, null);
						this.Parent.resetCurrentNodeDataShowLevel = currStatus;
					}
					else
					{
						this.Parent.Init(this.Parent.NodeData, null);
					}
					PageDetailElement parent2 = this.Parent;
					BasicInfoView instance = BasicInfoView.Instance;
					parent2.RefreshSearchResultHighlight((instance != null) ? instance.Searcher : null, false);
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
				bool flag7 = this.child != null && this.child.gameObject.activeInHierarchy;
				if (flag7)
				{
					this.child.Change(true, showLevel);
					Save.SetShowLevel(this.child.NodeData, EEncyclopediaContentLevel.Inherit);
				}
				bool flag8 = !isRecursive;
				if (flag8)
				{
					BasicInfoView instance3 = BasicInfoView.Instance;
					if (instance3 != null)
					{
						instance3.RefreshLevelTwoTitle(false);
					}
				}
				bool flag9 = selectedId != -1;
				if (flag9)
				{
					this.Parent.PopSelectedId(selectedId);
				}
			}
		}

		// Token: 0x060083A1 RID: 33697 RVA: 0x003D4498 File Offset: 0x003D2698
		private void RefreshToggleStates(EEncyclopediaContentLevel showLevel)
		{
			int levelInt = (int)(showLevel & EEncyclopediaContentLevel.LowMidHigh);
			this.High.isOn = (this.High.interactable && levelInt > 3);
			this.Mid.isOn = (!this.High.isOn && this.Mid.interactable && levelInt > 1);
			this.Low.isOn = (!this.High.isOn && !this.Mid.isOn);
			this.Reset.isOn = ((showLevel & EEncyclopediaContentLevel.Inherit) > EEncyclopediaContentLevel.None);
			this.LowBg.gameObject.SetActive(this.Low.isOn);
			this.MidBg.gameObject.SetActive(this.Mid.isOn);
			this.HighBg.gameObject.SetActive(this.High.isOn);
		}

		// Token: 0x060083A2 RID: 33698 RVA: 0x003D4587 File Offset: 0x003D2787
		public void ChangeLow(bool current)
		{
			this.ChangeImpl(false, EEncyclopediaContentLevel.Low);
		}

		// Token: 0x060083A3 RID: 33699 RVA: 0x003D4592 File Offset: 0x003D2792
		public void ChangeMid(bool current)
		{
			this.ChangeImpl(false, EEncyclopediaContentLevel.LowMid);
		}

		// Token: 0x060083A4 RID: 33700 RVA: 0x003D459D File Offset: 0x003D279D
		public void ChangeHigh(bool current)
		{
			this.ChangeImpl(false, EEncyclopediaContentLevel.LowMidHigh);
		}

		// Token: 0x060083A5 RID: 33701 RVA: 0x003D45A8 File Offset: 0x003D27A8
		public void ChangeImpl(bool isOn, EEncyclopediaContentLevel level)
		{
			bool changing = this._changing;
			if (!changing)
			{
				this.Change(isOn, level);
			}
		}

		// Token: 0x060083A6 RID: 33702 RVA: 0x003D45CC File Offset: 0x003D27CC
		public void ResetState(bool _)
		{
			this.Change(true, EEncyclopediaContentLevel.Inherit);
			this._changing = true;
			EEncyclopediaContentLevel showLevel = Save.GetShowLevel(this.NodeData, true);
			this.RefreshToggleStates(showLevel);
			this.Reset.isOn = ((showLevel & EEncyclopediaContentLevel.Inherit) > EEncyclopediaContentLevel.None);
			this._changing = false;
		}

		// Token: 0x060083A7 RID: 33703 RVA: 0x003D4618 File Offset: 0x003D2818
		public static void ResetAll()
		{
			byte status = Save.SaveData.GlobalLabelStatus;
			Save.SaveData.LabelStatus.Clear();
			Save.SaveData.GlobalLabelStatus = status;
		}

		// Token: 0x060083A8 RID: 33704 RVA: 0x003D4650 File Offset: 0x003D2850
		public void RefreshLevelFour()
		{
			this.Low.SetIsOnWithoutNotify(false);
			this.Mid.SetIsOnWithoutNotify(false);
			this.High.SetIsOnWithoutNotify(false);
			this.Low.gameObject.SetActive(false);
			this.Mid.gameObject.SetActive(this.NodeData != null && ((this.NodeData.RecursiveTempShowLevel | Save.GetShowLevel(this.NodeData, true)) & EEncyclopediaContentLevel.Mid) == EEncyclopediaContentLevel.None && (this.NodeData.ConfigItem.Level & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None);
			this.High.gameObject.SetActive(this.NodeData != null && ((this.NodeData.RecursiveTempShowLevel | Save.GetShowLevel(this.NodeData, true)) & EEncyclopediaContentLevel.High) == EEncyclopediaContentLevel.None && (this.NodeData.ConfigItem.Level & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None);
			this.LowBg.gameObject.SetActive(false);
			this.MidBg.gameObject.SetActive(false);
			this.HighBg.gameObject.SetActive(false);
		}

		// Token: 0x040064C8 RID: 25800
		[SerializeField]
		private bool isLevelFour;

		// Token: 0x040064C9 RID: 25801
		public CToggleObsolete Low;

		// Token: 0x040064CA RID: 25802
		public CToggleObsolete Mid;

		// Token: 0x040064CB RID: 25803
		public CToggleObsolete High;

		// Token: 0x040064CC RID: 25804
		public CToggleObsolete Reset;

		// Token: 0x040064CD RID: 25805
		public CImage LowBg;

		// Token: 0x040064CE RID: 25806
		public CImage MidBg;

		// Token: 0x040064CF RID: 25807
		public CImage HighBg;

		// Token: 0x040064D0 RID: 25808
		[SerializeField]
		private DisableStyleRoot lowStyle;

		// Token: 0x040064D1 RID: 25809
		[SerializeField]
		private DisableStyleRoot midStyle;

		// Token: 0x040064D2 RID: 25810
		[SerializeField]
		private DisableStyleRoot highStyle;

		// Token: 0x040064D3 RID: 25811
		[SerializeField]
		private ShowLevels child;

		// Token: 0x040064D4 RID: 25812
		private bool _changing;

		// Token: 0x040064D5 RID: 25813
		[CanBeNull]
		public NodeData NodeData;

		// Token: 0x040064D6 RID: 25814
		[CanBeNull]
		public PageDetailElement Parent;
	}
}
