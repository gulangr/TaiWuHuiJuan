using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Game.Components.Building
{
	// Token: 0x02000F60 RID: 3936
	public class BuildingAreaBlock : MonoBehaviour
	{
		// Token: 0x17001473 RID: 5235
		// (get) Token: 0x0600B412 RID: 46098 RVA: 0x0051E907 File Offset: 0x0051CB07
		private string CreateBuildingEffectKey
		{
			get
			{
				return this.isLarge ? "CreateBuildingEffectLargeKey" : "CreateBuildingEffectSmallKey";
			}
		}

		// Token: 0x17001474 RID: 5236
		// (get) Token: 0x0600B413 RID: 46099 RVA: 0x0051E91D File Offset: 0x0051CB1D
		private string CreateBuildingDoneEffectKey
		{
			get
			{
				return this.isLarge ? "CreateBuildingDoneEffectLargeKey" : "CreateBuildingDoneEffectSmallKey";
			}
		}

		// Token: 0x0600B414 RID: 46100 RVA: 0x0051E933 File Offset: 0x0051CB33
		private void Awake()
		{
			this.multiplyRemoveSelected.gameObject.SetActive(false);
			this._buildingIconOrgPos = this.buildingIcon.GetComponent<RectTransform>().anchoredPosition;
		}

		// Token: 0x0600B415 RID: 46101 RVA: 0x0051E95E File Offset: 0x0051CB5E
		private void OnDisable()
		{
			Sequence buildOperationDoneSequence = this._buildOperationDoneSequence;
			if (buildOperationDoneSequence != null)
			{
				buildOperationDoneSequence.Kill(false);
			}
			this._buildOperationDoneSequence = null;
			this.ClearEffect();
		}

		// Token: 0x0600B416 RID: 46102 RVA: 0x0051E984 File Offset: 0x0051CB84
		private void ClearEffect()
		{
			bool flag = this._createBuildingEffect != null;
			if (flag)
			{
				PoolManager.Destroy(this.CreateBuildingEffectKey, this._createBuildingEffect);
			}
			bool flag2 = this._createBuildingDoneEffect != null;
			if (flag2)
			{
				PoolManager.Destroy(this.CreateBuildingDoneEffectKey, this._createBuildingDoneEffect);
			}
			this._createBuildingEffect = null;
			this._createBuildingDoneEffect = null;
		}

		// Token: 0x0600B417 RID: 46103 RVA: 0x0051E9E4 File Offset: 0x0051CBE4
		private void GetCreateBuildingEffect(string animName)
		{
			bool flag = this._createBuildingEffect == null;
			if (flag)
			{
				this._createBuildingEffect = PoolManager.GetObject(this.CreateBuildingEffectKey);
			}
			this._createBuildingEffect.transform.SetParent(this.effectHolder);
			this._createBuildingEffect.transform.localScale = Vector3.one;
			this._createBuildingEffect.transform.localPosition = Vector3.zero;
			this._createBuildingEffect.GetComponent<CanvasGroup>().alpha = 1f;
			SkeletonGraphic spine = this._createBuildingEffect.transform.GetChild(0).GetComponent<SkeletonGraphic>();
			spine.AnimationState.SetAnimation(0, animName, true);
			this._createBuildingEffect.SetActive(true);
		}

		// Token: 0x0600B418 RID: 46104 RVA: 0x0051EAA0 File Offset: 0x0051CCA0
		public void SetBuildingOperateState(sbyte operationType, bool isStopping)
		{
			Sequence buildOperationDoneSequence = this._buildOperationDoneSequence;
			if (buildOperationDoneSequence != null)
			{
				buildOperationDoneSequence.Kill(true);
			}
			this._buildOperationDoneSequence = null;
			bool flag = operationType == -1;
			if (flag)
			{
				this.buildingIcon.GetComponent<CanvasRenderer>().SetAlpha(1f);
				this.ClearEffect();
			}
			else
			{
				bool flag2 = operationType == 0;
				if (flag2)
				{
					this.buildingIcon.GetComponent<CanvasRenderer>().SetAlpha(0f);
					this.GetCreateBuildingEffect(isStopping ? "building_1" : "building_2");
				}
			}
		}

		// Token: 0x0600B419 RID: 46105 RVA: 0x0051EB28 File Offset: 0x0051CD28
		public void CreateBuildingDone()
		{
			Sequence buildOperationDoneSequence = this._buildOperationDoneSequence;
			if (buildOperationDoneSequence != null)
			{
				buildOperationDoneSequence.Kill(true);
			}
			this._buildOperationDoneSequence = null;
			bool flag = this._createBuildingDoneEffect == null;
			if (flag)
			{
				this._createBuildingDoneEffect = PoolManager.GetObject(this.CreateBuildingDoneEffectKey);
			}
			this._createBuildingDoneEffect.transform.SetParent(this.effectHolder);
			this._createBuildingDoneEffect.transform.localScale = Vector3.one;
			this._createBuildingDoneEffect.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this._createBuildingDoneEffect.SetActive(false);
			this.buildingIcon.GetComponent<CanvasRenderer>().SetAlpha(1f);
			this.buildingIcon.color = Color.clear;
			RectTransform iconRect = this.buildingIcon.GetComponent<RectTransform>();
			iconRect.anchoredPosition = new Vector2(this._buildingIconOrgPos.x, this._buildingIconOrgPos.y - 20f);
			this.buildingIcon.gameObject.SetActive(true);
			this.GetCreateBuildingEffect("building_1");
			RectTransform buildEffRect = this._createBuildingEffect.GetComponent<RectTransform>();
			CanvasGroup buildEffCg = this._createBuildingEffect.GetComponent<CanvasGroup>();
			this._buildOperationDoneSequence = DOTween.Sequence();
			this._buildOperationDoneSequence.InsertCallback(0.1f, delegate
			{
				this._createBuildingDoneEffect.SetActive(true);
			});
			this._buildOperationDoneSequence.Insert(0.1f, buildEffRect.DOAnchorPosY(-20f, 0.2f, false));
			this._buildOperationDoneSequence.Insert(0.3f, buildEffCg.DOFade(0f, 0.6f));
			this._buildOperationDoneSequence.Insert(0.45f, this.buildingIcon.DOColor(Color.white, 0.25f));
			this._buildOperationDoneSequence.Insert(0.65f, iconRect.DOAnchorPos(this._buildingIconOrgPos, 0.15f, false));
			this._buildOperationDoneSequence.InsertCallback(1f, delegate
			{
				bool flag2 = this._createBuildingEffect != null;
				if (flag2)
				{
					PoolManager.Destroy(this.CreateBuildingEffectKey, this._createBuildingEffect);
				}
				this._createBuildingEffect = null;
			});
			this._buildOperationDoneSequence.InsertCallback(1.5f, new TweenCallback(this.ClearEffect));
			this._buildOperationDoneSequence.Play<Sequence>();
		}

		// Token: 0x04008C0D RID: 35853
		[SerializeField]
		public CImage canBuild;

		// Token: 0x04008C0E RID: 35854
		[SerializeField]
		public CImage buildingIcon;

		// Token: 0x04008C0F RID: 35855
		[SerializeField]
		public CImage selectTip;

		// Token: 0x04008C10 RID: 35856
		[SerializeField]
		public CRawImage back;

		// Token: 0x04008C11 RID: 35857
		[SerializeField]
		public CRawImage operateImg;

		// Token: 0x04008C12 RID: 35858
		[SerializeField]
		public GameObject holder;

		// Token: 0x04008C13 RID: 35859
		[SerializeField]
		public RectTransform levelAndNameHolder;

		// Token: 0x04008C14 RID: 35860
		[SerializeField]
		public RectTransform getEarnHolder;

		// Token: 0x04008C15 RID: 35861
		[SerializeField]
		public RectTransform leftTopHolder;

		// Token: 0x04008C16 RID: 35862
		[SerializeField]
		public PointerTrigger pointerTrigger;

		// Token: 0x04008C17 RID: 35863
		[SerializeField]
		public TooltipInvoker buildingDescMouseTip;

		// Token: 0x04008C18 RID: 35864
		[SerializeField]
		public DisableStyleRoot styleRoot;

		// Token: 0x04008C19 RID: 35865
		[SerializeField]
		public GameObject multiplyRemoveSelected;

		// Token: 0x04008C1A RID: 35866
		[SerializeField]
		public Transform effectHolder;

		// Token: 0x04008C1B RID: 35867
		[SerializeField]
		public bool isLarge;

		// Token: 0x04008C1C RID: 35868
		public short buildingBlockIndex;

		// Token: 0x04008C1D RID: 35869
		public const string CreateBuildingEffectSmallKey = "CreateBuildingEffectSmallKey";

		// Token: 0x04008C1E RID: 35870
		public const string CreateBuildingDoneEffectSmallKey = "CreateBuildingDoneEffectSmallKey";

		// Token: 0x04008C1F RID: 35871
		public const string CreateBuildingEffectLargeKey = "CreateBuildingEffectLargeKey";

		// Token: 0x04008C20 RID: 35872
		public const string CreateBuildingDoneEffectLargeKey = "CreateBuildingDoneEffectLargeKey";

		// Token: 0x04008C21 RID: 35873
		private const string BuildOperationSpineAnimEmpty = "building_1";

		// Token: 0x04008C22 RID: 35874
		private const string BuildOperationSpineAnimWorking = "building_2";

		// Token: 0x04008C23 RID: 35875
		private const float BuildOperationDoneAnchoredOffset = 20f;

		// Token: 0x04008C24 RID: 35876
		private GameObject _createBuildingEffect;

		// Token: 0x04008C25 RID: 35877
		private GameObject _createBuildingDoneEffect;

		// Token: 0x04008C26 RID: 35878
		private Sequence _buildOperationDoneSequence;

		// Token: 0x04008C27 RID: 35879
		private Vector2 _buildingIconOrgPos;
	}
}
