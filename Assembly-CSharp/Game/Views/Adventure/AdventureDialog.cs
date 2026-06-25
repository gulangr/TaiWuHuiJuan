using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameData.Adventure;
using TMPro;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C62 RID: 3170
	public class AdventureDialog : MonoBehaviour
	{
		// Token: 0x0600A185 RID: 41349 RVA: 0x004B7914 File Offset: 0x004B5B14
		public void RefreshDialog(AdventureTextData adventureTextData, AdventureElementData elementData)
		{
			this.canvasGroup.DOKill(true);
			this.canvasGroup.alpha = 1f;
			this._fading = false;
			this.content.SetText(adventureTextData.Text, true);
			bool flag = elementData != null;
			if (flag)
			{
				TextMeshProUGUI textMeshProUGUI = this.elementName;
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI.SetText(elementData.Name, true);
				}
				AdventureDialog.SetElementAvatarIcon(elementData.Icon, this.elementIcon);
			}
			else
			{
				WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
				string taiwuName = SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName;
				TextMeshProUGUI textMeshProUGUI2 = this.elementName;
				if (textMeshProUGUI2 != null)
				{
					textMeshProUGUI2.SetText(taiwuName, true);
				}
				CImage cimage = this.elementIcon;
				if (cimage != null)
				{
					cimage.SetSprite(ViewAdventureRemake.GetElementUIIconName("adventure_element_character_protagonist_" + worldMapModel.TaiwuGender.ToString()), false, null);
				}
			}
			this._showTime = 0f;
			base.gameObject.SetActive(true);
		}

		// Token: 0x0600A186 RID: 41350 RVA: 0x004B7A00 File Offset: 0x004B5C00
		public static void SetElementAvatarIcon(string iconName, CImage icon)
		{
			string avatarIconName = ViewAdventureRemake.GetElementAvatarIconName(iconName);
			AtlasInfo.Instance.GetSprite(avatarIconName, delegate(Sprite sprite)
			{
				bool flag = sprite == null;
				if (flag)
				{
					icon.SetSprite(ViewAdventureRemake.GetElementUIIconName(iconName), false, null);
					icon.GetComponent<RectTransform>().localPosition = AdventureDialog.SmallIconOffset;
				}
				else
				{
					icon.sprite = sprite;
					icon.gameObject.SetActive(true);
					icon.GetComponent<RectTransform>().localPosition = AdventureDialog.AvatarIconOffset;
				}
			});
		}

		// Token: 0x0600A187 RID: 41351 RVA: 0x004B7A48 File Offset: 0x004B5C48
		public bool UpdateShowTime()
		{
			bool flag = !base.gameObject.activeSelf;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool fading = this._fading;
				if (fading)
				{
					result = false;
				}
				else
				{
					bool flag2 = this.StartFade();
					if (flag2)
					{
						this._fading = true;
						this.canvasGroup.DOFade(0f, GlobalConfig.Instance.AdventureDialogFadeTime).OnComplete(delegate
						{
							this._fading = false;
							base.gameObject.SetActive(false);
						});
						this._showTime = 0f;
						result = true;
					}
					else
					{
						this._showTime += Time.deltaTime;
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A188 RID: 41352 RVA: 0x004B7ADF File Offset: 0x004B5CDF
		public void KillTween()
		{
			this.canvasGroup.DOKill(true);
		}

		// Token: 0x0600A189 RID: 41353 RVA: 0x004B7AEF File Offset: 0x004B5CEF
		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600A18A RID: 41354 RVA: 0x004B7B00 File Offset: 0x004B5D00
		private bool StartFade()
		{
			return this._showTime >= GlobalConfig.Instance.AdventureDialogContinuousTime;
		}

		// Token: 0x04007D49 RID: 32073
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04007D4A RID: 32074
		[SerializeField]
		private TextMeshProUGUI content;

		// Token: 0x04007D4B RID: 32075
		[SerializeField]
		private CImage elementIcon;

		// Token: 0x04007D4C RID: 32076
		[SerializeField]
		private RectTransform elementIconRect;

		// Token: 0x04007D4D RID: 32077
		[SerializeField]
		private TextMeshProUGUI elementName;

		// Token: 0x04007D4E RID: 32078
		private float _showTime;

		// Token: 0x04007D4F RID: 32079
		private bool _fading;

		// Token: 0x04007D50 RID: 32080
		private static readonly Vector3 AvatarIconOffset = new Vector3(0f, -8f, 0f);

		// Token: 0x04007D51 RID: 32081
		private static readonly Vector3 SmallIconOffset = new Vector3(0f, -26f, 0f);
	}
}
