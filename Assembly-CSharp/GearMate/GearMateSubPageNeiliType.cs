using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using UnityEngine;
using UnityEngine.UI;

namespace GearMate
{
	// Token: 0x0200061D RID: 1565
	public class GearMateSubPageNeiliType : GearMateSubPageBase
	{
		// Token: 0x060049D6 RID: 18902 RVA: 0x002273D4 File Offset: 0x002255D4
		protected override void InitInternal()
		{
			base.InitInternal();
			bool shouldInit = this._shouldInit;
			if (shouldInit)
			{
				this.InitButtons();
				this._mixParticle = base.CGet<UIParticle>("MixParticle");
				this._shouldInit = false;
			}
		}

		// Token: 0x060049D7 RID: 18903 RVA: 0x00227414 File Offset: 0x00225614
		public override void OnGearMateDataChanged()
		{
			base.OnGearMateDataChanged();
			this.RefreshElementsHighlight(base.Parent.GearMate.NeiliType);
		}

		// Token: 0x060049D8 RID: 18904 RVA: 0x00227438 File Offset: 0x00225638
		private void OnEnable()
		{
			bool flag = this._neiliType != -1;
			if (flag)
			{
				this.RefreshElementsHighlight(this._neiliType);
			}
		}

		// Token: 0x060049D9 RID: 18905 RVA: 0x00227464 File Offset: 0x00225664
		private void InitButtons()
		{
			for (int i = 0; i < 5; i++)
			{
				Refers element = this.GetRefersByNeiliType(i);
				for (int j = 0; j < 7; j++)
				{
					GameObject image = element.CGet<GameObject>(string.Format("Highlight{0}", j));
					Transform raycast = image.transform.parent.Find("Raycast");
					raycast.SetAsLastSibling();
					CButtonObsolete button = raycast.gameObject.GetComponent<CButtonObsolete>();
					GameObject buttonHover = Object.Instantiate<GameObject>(base.CGet<GameObject>("Hover"), button.transform, false);
					buttonHover.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					buttonHover.name = "Hover";
					image.SetActive(false);
					Object @object = button;
					string format = "Button{0}_{1}";
					object arg = j;
					string name = image.GetComponent<Image>().sprite.name;
					@object.name = string.Format(format, arg, name[name.Length - 1]);
					CImage component = buttonHover.GetComponent<CImage>();
					string format2 = "charactermenu3_22_thefiveelementshaver_{0}";
					string name2 = image.GetComponent<Image>().sprite.name;
					component.SetSprite(string.Format(format2, name2[name2.Length - 1]), true, null);
					this.InitButton(button, (j == 0) ? i : (6 + i * 6 + j - 1));
				}
			}
			List<CButtonObsolete> pureButtonList = base.CGetList<CButtonObsolete>("PureButton_");
			for (int t = 0; t <= 5; t++)
			{
				this.InitButton(pureButtonList[t], t);
			}
		}

		// Token: 0x060049DA RID: 18906 RVA: 0x00227600 File Offset: 0x00225800
		private void InitButton(CButtonObsolete button, int neiliType)
		{
			PointerTrigger trigger = button.gameObject.GetComponent<PointerTrigger>();
			GameObject hover = button.transform.Find("Hover").gameObject;
			TooltipInvoker tips = button.gameObject.GetComponent<TooltipInvoker>();
			tips.Type = TipType.FiveElements;
			button.ClearAndAddListener(delegate
			{
				this.OnClickNeiliTypeButton(neiliType);
			});
			trigger.EnterEvent.AddListener(delegate()
			{
				this.OnPointerEnter(neiliType, hover);
			});
			trigger.ExitEvent.AddListener(delegate()
			{
				this.OnPointerExit(hover);
			});
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox().Set("neiliType", neiliType);
			}
		}

		// Token: 0x060049DB RID: 18907 RVA: 0x002276CC File Offset: 0x002258CC
		private void OnClickNeiliTypeButton(int neiliType)
		{
			ExtraDomainMethod.Call.UpgradeGearMate(base.Parent.GearMateId, 12, ItemKey.Invalid, neiliType);
			bool flag = neiliType != this._neiliType;
			if (flag)
			{
				this.RefreshElementsHighlight(neiliType);
			}
		}

		// Token: 0x060049DC RID: 18908 RVA: 0x0022770C File Offset: 0x0022590C
		private void OnPointerEnter(int neiliType, GameObject obj)
		{
			bool flag = neiliType != this._neiliType;
			if (flag)
			{
				obj.SetActive(true);
			}
		}

		// Token: 0x060049DD RID: 18909 RVA: 0x00227732 File Offset: 0x00225932
		private void OnPointerExit(GameObject obj)
		{
			obj.SetActive(false);
		}

		// Token: 0x060049DE RID: 18910 RVA: 0x00227740 File Offset: 0x00225940
		private Refers GetRefersByNeiliType(int neiliType)
		{
			Refers result;
			switch (neiliType)
			{
			case 0:
				result = base.CGet<Refers>("JinGang");
				break;
			case 1:
				result = base.CGet<Refers>("ZiXia");
				break;
			case 2:
				result = base.CGet<Refers>("XuanYin");
				break;
			case 3:
				result = base.CGet<Refers>("ChunYang");
				break;
			case 4:
				result = base.CGet<Refers>("GuiYuan");
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x060049DF RID: 18911 RVA: 0x002277BC File Offset: 0x002259BC
		private void RefreshElementsHighlight(int neiliType)
		{
			bool flag = this._neiliType >= 0 && this._neiliType != 5;
			if (flag)
			{
				int fiveElement = CommonUtils.GetFiveElementByNeiliType(this._neiliType);
				Refers element = this.GetRefersByNeiliType(fiveElement);
				int index = this.GetIndexByNeiliType(this._neiliType);
				element.CGet<GameObject>(string.Format("Highlight{0}", index)).SetActive(false);
				element.CGet<ParticleSystem>(string.Format("Particle{0}", index)).gameObject.SetActive(false);
			}
			this._neiliType = neiliType;
			bool flag2 = neiliType == 5;
			if (flag2)
			{
				this._mixParticle.gameObject.SetActive(true);
				this._mixParticle.Play();
			}
			else
			{
				this._mixParticle.gameObject.SetActive(false);
			}
			bool flag3 = this._neiliType >= 0 && this._neiliType != 5;
			if (flag3)
			{
				int fiveElement2 = CommonUtils.GetFiveElementByNeiliType(this._neiliType);
				Refers element2 = this.GetRefersByNeiliType(fiveElement2);
				int index2 = this.GetIndexByNeiliType(this._neiliType);
				element2.CGet<GameObject>(string.Format("Highlight{0}", index2)).SetActive(true);
				element2.CGet<ParticleSystem>(string.Format("Particle{0}", index2)).gameObject.SetActive(true);
				element2.CGet<ParticleSystem>(string.Format("Particle{0}", index2)).Play();
			}
		}

		// Token: 0x060049E0 RID: 18912 RVA: 0x00227938 File Offset: 0x00225B38
		private int GetIndexByNeiliType(int neiliType)
		{
			bool flag = neiliType > 5;
			int result;
			if (flag)
			{
				result = neiliType % 6 + 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x04003327 RID: 13095
		private const int HighlightNum = 7;

		// Token: 0x04003328 RID: 13096
		private int _neiliType = -1;

		// Token: 0x04003329 RID: 13097
		private bool _shouldInit = true;

		// Token: 0x0400332A RID: 13098
		private UIParticle _mixParticle;
	}
}
