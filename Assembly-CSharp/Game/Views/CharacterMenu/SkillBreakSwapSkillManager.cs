using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B96 RID: 2966
	public class SkillBreakSwapSkillManager : MonoBehaviour
	{
		// Token: 0x06009253 RID: 37459 RVA: 0x00442F2B File Offset: 0x0044112B
		public void ReturnCellsBack()
		{
			this._movedCells.Clear();
			this._originalSiblingIndex.Clear();
		}

		// Token: 0x06009254 RID: 37460 RVA: 0x00442F48 File Offset: 0x00441148
		private void Update()
		{
			bool flag = !base.gameObject.activeSelf;
			if (flag)
			{
			}
		}

		// Token: 0x06009255 RID: 37461 RVA: 0x00442F6A File Offset: 0x0044116A
		private void OnEnable()
		{
			CharacterDomainMethod.AsyncCall.GetFixedCharacterName(null, 597, delegate(int offset, RawDataPool dataPool)
			{
				NameRelatedData nameRelatedData = new NameRelatedData();
				Serializer.Deserialize(dataPool, offset, ref nameRelatedData);
				this.txtSnakeName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, false, false);
			});
		}

		// Token: 0x040070D7 RID: 28887
		[SerializeField]
		private RectTransform scaleAnimationRootMirror;

		// Token: 0x040070D8 RID: 28888
		[SerializeField]
		private RectTransform gridDragAndScaleMirror;

		// Token: 0x040070D9 RID: 28889
		[SerializeField]
		private RectTransform gridAreaMirror;

		// Token: 0x040070DA RID: 28890
		[SerializeField]
		private RectTransform cellRootMirror;

		// Token: 0x040070DB RID: 28891
		[SerializeField]
		private TextMeshProUGUI txtSnakeName;

		// Token: 0x040070DC RID: 28892
		[SerializeField]
		private RectTransform scaleAnimationRoot;

		// Token: 0x040070DD RID: 28893
		[SerializeField]
		private RectTransform gridDragAndScale;

		// Token: 0x040070DE RID: 28894
		[SerializeField]
		private RectTransform gridArea;

		// Token: 0x040070DF RID: 28895
		[SerializeField]
		private RectTransform cellRoot;

		// Token: 0x040070E0 RID: 28896
		private readonly List<RectTransform> _movedCells = new List<RectTransform>();

		// Token: 0x040070E1 RID: 28897
		private readonly Dictionary<RectTransform, int> _originalSiblingIndex = new Dictionary<RectTransform, int>();
	}
}
