using System;
using System.Diagnostics.CodeAnalysis;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Elements;
using Game.Views.Encyclopedia.Save;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A70 RID: 2672
	public class ShowInLevelFour : MonoBehaviour
	{
		// Token: 0x17000E70 RID: 3696
		// (get) Token: 0x06008393 RID: 33683 RVA: 0x003D3C7C File Offset: 0x003D1E7C
		private CScrollRect Parent
		{
			get
			{
				return this.parentContainer.scrollContainer;
			}
		}

		// Token: 0x06008394 RID: 33684 RVA: 0x003D3C89 File Offset: 0x003D1E89
		public void OnEnable()
		{
			this.Init(null);
		}

		// Token: 0x06008395 RID: 33685 RVA: 0x003D3C94 File Offset: 0x003D1E94
		private void Awake()
		{
			this.midTog.onValueChanged.RemoveAllListeners();
			this.midTog.onValueChanged.AddListener(new UnityAction<bool>(this.ChangeMid));
			this.highTog.onValueChanged.RemoveAllListeners();
			this.highTog.onValueChanged.AddListener(new UnityAction<bool>(this.ChangeHigh));
			this.midLayout.SetActive(true);
			this.highLayout.SetActive(true);
		}

		// Token: 0x06008396 RID: 33686 RVA: 0x003D3D18 File Offset: 0x003D1F18
		public void Init([MaybeNull] NodeData nodeData)
		{
			bool flag = nodeData != null;
			if (flag)
			{
				this.nodeData = nodeData;
			}
			this._changing = true;
			NodeData nodeData2 = this.nodeData;
			this.RefreshLevelFour((nodeData2 != null) ? nodeData2.RecursiveTempShowLevel : Save.GetGlobalShowLevel());
			this._changing = false;
		}

		// Token: 0x06008397 RID: 33687 RVA: 0x003D3D60 File Offset: 0x003D1F60
		public void Change(bool isRecursive, EEncyclopediaContentLevel showLevel)
		{
			bool changing = this._changing;
			if (!changing)
			{
				NodeData nodeData = this.nodeData;
				bool flag = nodeData != null && nodeData.Id != -1;
				if (flag)
				{
					this.nodeData.TempShowLevel = EEncyclopediaContentLevel.None;
				}
				showLevel &= ~EEncyclopediaContentLevel.Inherit;
				nodeData = this.nodeData;
				bool flag2 = nodeData != null && nodeData.Id != -1;
				if (flag2)
				{
					Save.SetShowLevel(this.nodeData, showLevel);
				}
				this._changing = true;
				this.RefreshLevelFour(showLevel);
				this._changing = false;
			}
		}

		// Token: 0x06008398 RID: 33688 RVA: 0x003D3DE9 File Offset: 0x003D1FE9
		private void ChangeMid(bool current)
		{
			this.ChangeImpl(current, this.midTog, EEncyclopediaContentLevel.LowMid);
		}

		// Token: 0x06008399 RID: 33689 RVA: 0x003D3DFB File Offset: 0x003D1FFB
		private void ChangeHigh(bool current)
		{
			this.ChangeImpl(current, this.highTog, EEncyclopediaContentLevel.LowMidHigh);
		}

		// Token: 0x0600839A RID: 33690 RVA: 0x003D3E10 File Offset: 0x003D2010
		private void ChangeImpl(bool current, CToggle toggle, EEncyclopediaContentLevel level)
		{
			bool changing = this._changing;
			if (!changing)
			{
				RectTransform rectTrans = toggle.transform as RectTransform;
				Vector2 curLoc = rectTrans.anchoredPosition;
				this.Change(true, current ? level : ((EEncyclopediaContentLevel)((byte)level >> 1)));
				this.Parent.ScrollTo(this.Parent.Content.anchoredPosition + curLoc - rectTrans.anchoredPosition, 0f);
			}
		}

		// Token: 0x0600839B RID: 33691 RVA: 0x003D3E84 File Offset: 0x003D2084
		private void RefreshLevelFour(EEncyclopediaContentLevel level)
		{
			NodeData nodeData = this.nodeData;
			EEncyclopediaContentLevel selfLevel = (nodeData != null) ? nodeData.ConfigItem.Level : EEncyclopediaContentLevel.Low;
			EEncyclopediaContentLevel globalLevel = Save.GetGlobalShowLevel();
			bool flag = (globalLevel & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None;
			if (flag)
			{
				this.midTog.isOn = true;
				this.midTog.gameObject.SetActive(false);
			}
			else
			{
				this.midTog.gameObject.SetActive((selfLevel & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None);
				this.midTog.isOn = ((level & EEncyclopediaContentLevel.Mid) > EEncyclopediaContentLevel.None);
				TextMeshProUGUI label = this.midTog.GetComponentInChildren<TextMeshProUGUI>();
				label.text = (this.midTog.isOn ? LanguageKey.LK_Encyclopedia_LevelButton_Mid4_Hide : LanguageKey.LK_Encyclopedia_LevelButton_Mid4).Tr();
				this.midImage.rectTransform.localScale = new Vector3(1f, (float)(this.midTog.isOn ? -1 : 1), 1f);
			}
			this.midLayout.SetActive(this.midTog.isOn);
			bool flag2 = (globalLevel & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None;
			if (flag2)
			{
				this.highTog.isOn = true;
				this.highTog.gameObject.SetActive(false);
			}
			else
			{
				this.highTog.gameObject.SetActive((selfLevel & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None);
				this.highTog.isOn = (((globalLevel | level) & EEncyclopediaContentLevel.High) > EEncyclopediaContentLevel.None);
				TextMeshProUGUI label2 = this.highTog.GetComponentInChildren<TextMeshProUGUI>();
				label2.text = (this.highTog.isOn ? LanguageKey.LK_Encyclopedia_LevelButton_High4_Hide : LanguageKey.LK_Encyclopedia_LevelButton_High4).Tr();
				this.highImage.rectTransform.localScale = new Vector3(1f, (float)(this.highTog.isOn ? -1 : 1), 1f);
			}
			this.highLayout.SetActive(this.highTog.isOn);
		}

		// Token: 0x040064BF RID: 25791
		[SerializeField]
		private CToggle midTog;

		// Token: 0x040064C0 RID: 25792
		[SerializeField]
		private CToggle highTog;

		// Token: 0x040064C1 RID: 25793
		[SerializeField]
		private GameObject midLayout;

		// Token: 0x040064C2 RID: 25794
		[SerializeField]
		private GameObject highLayout;

		// Token: 0x040064C3 RID: 25795
		[SerializeField]
		private CImage midImage;

		// Token: 0x040064C4 RID: 25796
		[SerializeField]
		private CImage highImage;

		// Token: 0x040064C5 RID: 25797
		[SerializeField]
		private LeveledContainer parentContainer;

		// Token: 0x040064C6 RID: 25798
		[SerializeField]
		[ReadOnly]
		private NodeData nodeData;

		// Token: 0x040064C7 RID: 25799
		private bool _changing;
	}
}
