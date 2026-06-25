using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F25 RID: 3877
	public class FeatureScroll : MonoBehaviour
	{
		// Token: 0x0600B279 RID: 45689 RVA: 0x00513996 File Offset: 0x00511B96
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600B27A RID: 45690 RVA: 0x005139A0 File Offset: 0x00511BA0
		private void OnDestroy()
		{
			bool flag = this.infinityScroll != null;
			if (flag)
			{
				this.infinityScroll.OnItemRender -= this.OnFeatureItemRender;
			}
		}

		// Token: 0x0600B27B RID: 45691 RVA: 0x005139D8 File Offset: 0x00511BD8
		public void Set(CharacterDisplayData displayData, bool useLargeItem = false, bool isTaiwu = false, Dictionary<short, int> temporaryFeatureLeftTimes = null)
		{
			this.Init();
			this._currentData = displayData;
			this._isTaiwu = isTaiwu;
			this._temporaryFeatureLeftTimes = temporaryFeatureLeftTimes;
			bool flag = this.infinityScroll == null;
			if (flag)
			{
				Debug.LogError("InfinityScroll component is not assigned!");
			}
			else
			{
				bool flag2 = displayData == null || displayData.FeatureIds == null;
				if (flag2)
				{
					this.SetEmpty();
				}
				else
				{
					this.Set(displayData.FeatureIds, useLargeItem, isTaiwu, temporaryFeatureLeftTimes, -1);
				}
			}
		}

		// Token: 0x0600B27C RID: 45692 RVA: 0x00513A50 File Offset: 0x00511C50
		public void Set(List<short> featureIds, bool useLargeItem = false, bool isTaiwu = false, Dictionary<short, int> temporaryFeatureLeftTimes = null, int characterId = -1)
		{
			this._showFeatureList = new List<short>();
			this._currentCharacterId = characterId;
			this._temporaryFeatureLeftTimes = temporaryFeatureLeftTimes;
			bool flag = this.infinityScroll != null;
			if (flag)
			{
				this.infinityScroll.OnItemRender -= this.OnFeatureItemRender;
				this.infinityScroll.OnItemRender += this.OnFeatureItemRender;
			}
			bool flag2 = featureIds == null || featureIds.Count <= 0;
			if (flag2)
			{
				this.SetEmpty();
			}
			else
			{
				this._showFeatureList.Clear();
				for (int i = 0; i < featureIds.Count; i++)
				{
					short featureId = featureIds[i];
					CharacterFeatureItem config = CharacterFeature.Instance[featureId];
					bool flag3 = config != null && !config.Hidden;
					if (flag3)
					{
						this._showFeatureList.Add(featureId);
					}
				}
				this.infinityScroll.SetDataCount(this._showFeatureList.Count);
				bool flag4 = (this.infinityScroll.srcPrefab == this.smallFeatureItemTemplate && useLargeItem) || (this.infinityScroll.srcPrefab == this.largeFeatureItemTemplate && !useLargeItem);
				if (flag4)
				{
					GameObject newTemplate = useLargeItem ? this.largeFeatureItemTemplate : this.smallFeatureItemTemplate;
					int newLineCount = useLargeItem ? this.largeColumnCount : this.smallColumnCount;
					this.infinityScroll.UpdateStyle(this.infinityScroll.Direction, newLineCount, this.infinityScroll.gap, this.infinityScroll.padding, newTemplate);
				}
			}
		}

		// Token: 0x0600B27D RID: 45693 RVA: 0x00513BF0 File Offset: 0x00511DF0
		public void SetEmpty()
		{
			List<short> showFeatureList = this._showFeatureList;
			if (showFeatureList != null)
			{
				showFeatureList.Clear();
			}
			bool flag = this.infinityScroll != null;
			if (flag)
			{
				this.infinityScroll.SetDataCount(0);
			}
		}

		// Token: 0x0600B27E RID: 45694 RVA: 0x00513C30 File Offset: 0x00511E30
		public void Refresh()
		{
			bool flag = this._currentData != null;
			if (flag)
			{
				this.Set(this._currentData, this._isTaiwu, false, null);
			}
		}

		// Token: 0x0600B27F RID: 45695 RVA: 0x00513C64 File Offset: 0x00511E64
		private void Init()
		{
			bool initialized = this._initialized;
			if (!initialized)
			{
				this._showFeatureList = new List<short>();
				bool flag = this.infinityScroll != null;
				if (flag)
				{
					this.infinityScroll.OnItemRender += this.OnFeatureItemRender;
				}
				this._initialized = true;
			}
		}

		// Token: 0x0600B280 RID: 45696 RVA: 0x00513CBC File Offset: 0x00511EBC
		private void OnFeatureItemRender(int index, GameObject itemGo)
		{
			bool flag = index < 0 || index >= this._showFeatureList.Count || itemGo == null;
			if (!flag)
			{
				short featureId = this._showFeatureList[index];
				Feature featureItem = itemGo.GetComponent<Feature>();
				bool flag2 = featureItem != null;
				if (flag2)
				{
					CharacterDisplayData currentData = this._currentData;
					int characterId = (currentData != null) ? currentData.CharacterId : this._currentCharacterId;
					int leftTime = -1;
					Dictionary<short, int> temporaryFeatureLeftTimes = this._temporaryFeatureLeftTimes;
					if (temporaryFeatureLeftTimes != null)
					{
						temporaryFeatureLeftTimes.TryGetValue(featureId, out leftTime);
					}
					featureItem.Set(featureId, characterId, this._isTaiwu, leftTime);
				}
			}
		}

		// Token: 0x04008A78 RID: 35448
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x04008A79 RID: 35449
		[SerializeField]
		private GameObject smallFeatureItemTemplate;

		// Token: 0x04008A7A RID: 35450
		[SerializeField]
		private GameObject largeFeatureItemTemplate;

		// Token: 0x04008A7B RID: 35451
		[SerializeField]
		private int smallColumnCount = 6;

		// Token: 0x04008A7C RID: 35452
		[SerializeField]
		private int largeColumnCount = 3;

		// Token: 0x04008A7D RID: 35453
		private List<short> _showFeatureList;

		// Token: 0x04008A7E RID: 35454
		private CharacterDisplayData _currentData;

		// Token: 0x04008A7F RID: 35455
		private int _currentCharacterId = -1;

		// Token: 0x04008A80 RID: 35456
		private bool _isTaiwu;

		// Token: 0x04008A81 RID: 35457
		private Dictionary<short, int> _temporaryFeatureLeftTimes;

		// Token: 0x04008A82 RID: 35458
		private bool _initialized;
	}
}
