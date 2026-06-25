using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu.Relationship
{
	// Token: 0x02000BB3 RID: 2995
	public class RelationshipGroup : MonoBehaviour
	{
		// Token: 0x060096D3 RID: 38611 RVA: 0x004656E8 File Offset: 0x004638E8
		public void SetIconAndName(ViewCharacterMenuRelationship.GraphViewGroupConfiguration configuration)
		{
			this.icon.sprite = configuration.icon;
			this.icon.SetNativeSize();
			this.backImg.texture = configuration.back;
			this.backImg.SetNativeSize();
			this.nameLabel.text = ((configuration.group == ViewCharacterMenuRelationship.EGroup.Faction) ? LocalStringManager.Get(LanguageKey.LK_Faction) : LocalStringManager.Get(string.Format("LK_RelationShip_{0}", configuration.group)));
		}

		// Token: 0x060096D4 RID: 38612 RVA: 0x00465770 File Offset: 0x00463970
		[return: TupleElementNames(new string[]
		{
			"position",
			"size"
		})]
		public ValueTuple<Vector3, Vector2> SetBack(float scale, float angleDelta, int index)
		{
			Vector2 textureSize = new Vector2((float)this.backImg.texture.width, (float)this.backImg.texture.height);
			RectTransform rectTransform = base.GetComponent<RectTransform>();
			Vector2 size = textureSize * scale;
			float angle = (90f - (float)index * angleDelta) * 0.017453292f;
			Vector3 localPosition = new Vector3(Mathf.Cos(angle) * size.x, Mathf.Sin(angle) * size.y) * 2f;
			this.backImg.rectTransform.localScale = scale * Vector3.one;
			rectTransform.anchoredPosition3D = localPosition;
			rectTransform.sizeDelta = size;
			return new ValueTuple<Vector3, Vector2>(localPosition, size);
		}

		// Token: 0x060096D5 RID: 38613 RVA: 0x0046582D File Offset: 0x00463A2D
		public void SetLink(Vector3 pos)
		{
			this.link.Vertices = new Vector2[]
			{
				Vector2.zero,
				pos
			};
		}

		// Token: 0x060096D6 RID: 38614 RVA: 0x0046585C File Offset: 0x00463A5C
		public void PlayAnim()
		{
			this.linkGroup.alpha = 0f;
			this.backGroup.alpha = 0f;
			this.detailGroup.alpha = 0f;
			this.rootGroup.alpha = 0f;
			Transform backTransform = this.backGroup.transform;
			Vector3 backGroupPos = backTransform.position;
			float backGroupScale = backTransform.localScale.x;
			float startScale = backGroupScale * 0.5f;
			backTransform.position = new Vector3(0f, 0f, 240f);
			backTransform.localScale = new Vector3(startScale, startScale, startScale);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				backTransform.DOMove(backGroupPos, 0.16f, false);
			});
			sequence.AppendCallback(delegate
			{
				backTransform.DOScale(backGroupScale, 0.16f);
			});
			sequence.AppendCallback(delegate
			{
				this.backGroup.DOFade(1f, 0.16f);
			});
			sequence.AppendInterval(0.16f);
			sequence.AppendCallback(delegate
			{
				this.linkGroup.DOFade(1f, 0.5f);
			});
			sequence.AppendInterval(0.040000007f);
			sequence.AppendCallback(delegate
			{
				this.detailGroup.DOFade(1f, 0.666f);
			});
			sequence.AppendCallback(delegate
			{
				this.rootGroup.DOFade(1f, 0.666f);
			});
		}

		// Token: 0x040073A2 RID: 29602
		[SerializeField]
		private Line2DGenerator link;

		// Token: 0x040073A3 RID: 29603
		[SerializeField]
		private CRawImage backImg;

		// Token: 0x040073A4 RID: 29604
		[SerializeField]
		private CImage icon;

		// Token: 0x040073A5 RID: 29605
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040073A6 RID: 29606
		[SerializeField]
		private CanvasGroup linkGroup;

		// Token: 0x040073A7 RID: 29607
		[SerializeField]
		private CanvasGroup backGroup;

		// Token: 0x040073A8 RID: 29608
		[SerializeField]
		private CanvasGroup detailGroup;

		// Token: 0x040073A9 RID: 29609
		[SerializeField]
		private CanvasGroup rootGroup;

		// Token: 0x040073AA RID: 29610
		private const float LinkStart = 0.16f;

		// Token: 0x040073AB RID: 29611
		private const float LinkDuration = 0.5f;

		// Token: 0x040073AC RID: 29612
		private const float BackDuration = 0.16f;

		// Token: 0x040073AD RID: 29613
		private const float BackScaleStart = 0.5f;

		// Token: 0x040073AE RID: 29614
		private const float ContentStart = 0.2f;

		// Token: 0x040073AF RID: 29615
		private const float ContentDuration = 0.666f;

		// Token: 0x040073B0 RID: 29616
		private const int PositionZ = 240;
	}
}
