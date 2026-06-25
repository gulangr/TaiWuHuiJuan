using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000832 RID: 2098
	public class ImagesAndTexts : MonoBehaviour
	{
		// Token: 0x06006696 RID: 26262 RVA: 0x002EC600 File Offset: 0x002EA800
		public void SetText([CanBeNull] params string[] data)
		{
			foreach (string _ in this.texts.Zip(data ?? Enumerable.Empty<string>(), (TMP_Text text, string s) => text.text = s.ColorReplace()))
			{
			}
		}

		// Token: 0x06006697 RID: 26263 RVA: 0x002EC67C File Offset: 0x002EA87C
		public void SetTextRaw([CanBeNull] params string[] data)
		{
			foreach (string _ in this.texts.Zip(data ?? Enumerable.Empty<string>(), delegate(TMP_Text text, string s)
			{
				text.text = s;
				return s;
			}))
			{
			}
		}

		// Token: 0x06006698 RID: 26264 RVA: 0x002EC6F8 File Offset: 0x002EA8F8
		public void SetImageSprites([CanBeNull] params string[] spriteNames)
		{
			foreach (ValueTuple<CImage, string> valueTuple in this.images.Zip(spriteNames ?? Enumerable.Empty<string>(), (CImage img, string s) => new ValueTuple<CImage, string>(img, s)))
			{
				CImage img2 = valueTuple.Item1;
				string s2 = valueTuple.Item2;
				img2.SetSprite(s2, false, null);
			}
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x002EC788 File Offset: 0x002EA988
		public void SetImageSprites([CanBeNull] params Sprite[] sprites)
		{
			foreach (Sprite _ in this.images.Zip(sprites ?? Enumerable.Empty<Sprite>(), delegate(CImage img, Sprite s)
			{
				img.sprite = s;
				return s;
			}))
			{
			}
		}

		// Token: 0x040047CC RID: 18380
		[SerializeField]
		public TMP_Text[] texts;

		// Token: 0x040047CD RID: 18381
		[SerializeField]
		public CImage[] images;
	}
}
