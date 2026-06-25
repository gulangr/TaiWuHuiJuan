using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Tutorial;
using Game.Views.Migrate.Mod;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.Tutorial
{
	// Token: 0x02000748 RID: 1864
	public class ViewGuidingChapter : UIBase
	{
		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x06005A49 RID: 23113 RVA: 0x0029E524 File Offset: 0x0029C724
		private BasicGameData BasicGameData
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>();
			}
		}

		// Token: 0x06005A4A RID: 23114 RVA: 0x0029E52C File Offset: 0x0029C72C
		public override void OnInit(ArgumentBox argsBox)
		{
			bool hasValue = false;
			bool flag = argsBox != null;
			if (flag)
			{
				hasValue = argsBox.Get("TemplateId", out this._curTemplateId);
			}
			this._triggeredGuidingChapterDictionary = this.BasicGameData.GetTriggeredGuidingChapterDictionary();
			this.totalTemplateIdList = this._triggeredGuidingChapterDictionary.Keys.ToList<short>();
			this.CalcClassTemplateIdDict();
			bool flag2 = !hasValue;
			if (flag2)
			{
				this._curClassTemplateId = this.classTemplateIdDict.Keys.First<short>();
				this._curTemplateId = this.classTemplateIdDict[this._curClassTemplateId].FirstOrDefault<short>();
			}
			else
			{
				this._curClassTemplateId = GuidingChapter.Instance[this._curTemplateId].Class;
			}
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x06005A4B RID: 23115 RVA: 0x0029E608 File Offset: 0x0029C808
		private void OnListenerIdReady()
		{
			this.Element.ShowAfterRefresh();
			this.unlockItemScroll.SetDataCount(this.GetClassTemplateIdList(this._curClassTemplateId).Count);
			this.ShowChapter(this._curTemplateId);
			this.InitClassToggleGroup();
			this.classToggleGroup.SetWithoutNotify(this.GetClassTemplateIdIndex(this._curClassTemplateId));
			this.RefreshClassSwitchButton();
			this.RefreshClearAllNewButton();
		}

		// Token: 0x06005A4C RID: 23116 RVA: 0x0029E67C File Offset: 0x0029C87C
		private void RefreshClearAllNewButton()
		{
			this.buttonClearAllNew.interactable = false;
			List<short> list = this.GetClassTemplateIdList(this._curClassTemplateId);
			foreach (short tempId in list)
			{
				sbyte state = this.BasicGameData.GetTriggeredGuidingChapterState(tempId);
				bool flag = state != 0;
				if (!flag)
				{
					this.buttonClearAllNew.interactable = true;
					break;
				}
			}
			TooltipInvoker tips = this.buttonClearAllNew.GetComponent<TooltipInvoker>();
			tips.enabled = !this.buttonClearAllNew.interactable;
			TextMeshProUGUI txt = this.buttonClearAllNew.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			txt.color = (this.buttonClearAllNew.interactable ? new Color(0.81f, 0.79f, 0.76f) : new Color(0.42f, 0.42f, 0.42f));
		}

		// Token: 0x06005A4D RID: 23117 RVA: 0x0029E784 File Offset: 0x0029C984
		private void Awake()
		{
			this.unlockItemScroll.OnItemRender += this.OnItemRender;
			this.classToggleGroup.Init(0);
			this.classToggleGroup.OnActiveIndexChange += this.OnClassToggleChange;
			this.pageSwitch.OnValueChanged = new Action<int>(this.OnPageValueChanged);
			this.encyclopediaBtn.onClick.ResetListener(delegate()
			{
				Config.GuidingChapterItem config = GuidingChapter.Instance[this._curTemplateId];
				UIElement.Encyclopedia.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("key", config.Encyclopedia));
				UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
			});
			this.prevClass.onClick.ResetListener(delegate()
			{
				this.RefreshClassSwitchButton();
				int index = this.GetClassTemplateIdIndex(this._curClassTemplateId);
				bool flag = index > 0;
				if (flag)
				{
					this.classToggleGroup.Set(index - 1, true);
				}
			});
			this.nextClass.onClick.ResetListener(delegate()
			{
				this.RefreshClassSwitchButton();
				int index = this.GetClassTemplateIdIndex(this._curClassTemplateId);
				bool flag = index < this.classToggleGroup.Count() - 1;
				if (flag)
				{
					this.classToggleGroup.Set(index + 1, true);
				}
			});
			this.buttonClearAllNew.ClearAndAddListener(delegate
			{
				foreach (KeyValuePair<short, List<short>> info in this.classTemplateIdDict)
				{
					foreach (short tempId in info.Value)
					{
						sbyte state = this.BasicGameData.GetTriggeredGuidingChapterState(tempId);
						bool flag = state != 0;
						if (!flag)
						{
							WorldDomainMethod.Call.TriggeredGuidingChapter(tempId, EGuidingChapterState.AlreadyRead);
							for (int i = 0; i < this.scrollRect.Content.childCount; i++)
							{
								Transform child = this.scrollRect.Content.GetChild(i);
								Game.Components.Tutorial.GuidingChapterItem chapterItem = child.GetComponent<Game.Components.Tutorial.GuidingChapterItem>();
								bool flag2 = chapterItem == null;
								if (!flag2)
								{
									bool flag3 = tempId != chapterItem.TemplateId;
									if (!flag3)
									{
										chapterItem.SetStateIcon(-1);
									}
								}
							}
						}
					}
				}
				TooltipInvoker tips = this.buttonClearAllNew.GetComponent<TooltipInvoker>();
				tips.enabled = true;
				this.buttonClearAllNew.interactable = false;
				TextMeshProUGUI txt = this.buttonClearAllNew.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
				txt.color = new Color(0.42f, 0.42f, 0.42f);
			});
		}

		// Token: 0x06005A4E RID: 23118 RVA: 0x0029E855 File Offset: 0x0029CA55
		private void OnDestroy()
		{
			this.unlockItemScroll.OnItemRender -= this.OnItemRender;
			this.classToggleGroup.OnActiveIndexChange -= this.OnClassToggleChange;
		}

		// Token: 0x06005A4F RID: 23119 RVA: 0x0029E888 File Offset: 0x0029CA88
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCancel" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005A50 RID: 23120 RVA: 0x0029E8B8 File Offset: 0x0029CAB8
		private void OnPageValueChanged(int index)
		{
			Config.GuidingChapterItem config = GuidingChapter.Instance[this._curTemplateId];
			this.SetPartInfo(config, --index);
		}

		// Token: 0x06005A51 RID: 23121 RVA: 0x0029E8E8 File Offset: 0x0029CAE8
		private void ShowChapter(short templateId)
		{
			Config.GuidingChapterItem config = GuidingChapter.Instance[templateId];
			this.pageSwitch.Init(1, (int)config.PartCount, 1);
			this.SetPartInfo(config, 0);
			WorldDomainMethod.Call.TriggeredGuidingChapter(templateId, EGuidingChapterState.AlreadyRead);
		}

		// Token: 0x06005A52 RID: 23122 RVA: 0x0029E928 File Offset: 0x0029CB28
		private void SetPartInfo(Config.GuidingChapterItem config, int index)
		{
			this.partDesc.SetText(config.PartDesc[index], true);
			this.partTitle.SetText(config.PartTitle[index], true);
			string[] imageArray = config.PartImage.Split(',', StringSplitOptions.None);
			CommonUtils.SetRawImage(this.partImage, this.texturePath + imageArray[index], false);
		}

		// Token: 0x06005A53 RID: 23123 RVA: 0x0029E98C File Offset: 0x0029CB8C
		private void OnClassToggleChange(int newIndex, int oldIndex)
		{
			this._curClassTemplateId = this.classTemplateIdList[newIndex];
			List<short> templateIdList = this.GetClassTemplateIdList(this._curClassTemplateId);
			this._curTemplateId = templateIdList[0];
			this.ShowChapter(this._curTemplateId);
			this.unlockItemScroll.SetDataCount(this.GetClassTemplateIdList(this._curClassTemplateId).Count);
			this.unlockItemScroll.Scroll.ScrollBar.value = 0f;
			this.RefreshClassSwitchButton();
			this.RefreshClearAllNewButton();
		}

		// Token: 0x06005A54 RID: 23124 RVA: 0x0029EA1C File Offset: 0x0029CC1C
		private void RefreshClassSwitchButton()
		{
			int index = this.GetClassTemplateIdIndex(this._curClassTemplateId);
			this.prevClass.interactable = (index > 0);
			this.nextClass.interactable = (index < this.classToggleGroup.Count() - 1);
		}

		// Token: 0x06005A55 RID: 23125 RVA: 0x0029EA63 File Offset: 0x0029CC63
		private void InitClassToggleGroup()
		{
			this.classToggleGroup.Clear();
			this.classToggleHolder.Rebuild<CToggle>(this.classTemplateIdList.Count, delegate(CToggle toggle, int index)
			{
				TextMeshProUGUI title = toggle.GetComponentInChildren<TextMeshProUGUI>();
				short classTemplateId = this.classTemplateIdList[index];
				GuidingChapterClassItem config = GuidingChapterClass.Instance[classTemplateId];
				title.SetText(config.Name, true);
				bool flag = this.classToggleGroup.CanAddToggle(toggle);
				if (flag)
				{
					this.classToggleGroup.Add(toggle);
				}
				title.text.SetColor("lightyellow").ColorReplace();
			});
		}

		// Token: 0x06005A56 RID: 23126 RVA: 0x0029EA98 File Offset: 0x0029CC98
		private void OnItemRender(int index, GameObject go)
		{
			Game.Components.Tutorial.GuidingChapterItem chapterItem = go.GetComponent<Game.Components.Tutorial.GuidingChapterItem>();
			List<short> list = this.GetClassTemplateIdList(this._curClassTemplateId);
			short templateId = list[index];
			Config.GuidingChapterItem config = GuidingChapter.Instance[templateId];
			chapterItem.chapterName.text = config.Name;
			chapterItem.SetTemplateId(templateId);
			sbyte state = this.BasicGameData.GetTriggeredGuidingChapterState(templateId);
			bool flag = state == 0 && templateId != this._curTemplateId;
			if (flag)
			{
				chapterItem.SetStateIcon(state);
			}
			else
			{
				chapterItem.SetStateIcon(-1);
			}
			chapterItem.chapterBtn.onClick.ResetListener(delegate()
			{
				this.HideAllChapterItemSelected();
				chapterItem.selected.gameObject.SetActive(true);
				this.ShowChapter(templateId);
				this._curTemplateId = templateId;
				chapterItem.SetStateIcon(-1);
			});
			chapterItem.selected.gameObject.SetActive(templateId == this._curTemplateId);
		}

		// Token: 0x06005A57 RID: 23127 RVA: 0x0029EBA8 File Offset: 0x0029CDA8
		private void HideAllChapterItemSelected()
		{
			for (int i = 0; i < this.scrollRect.Content.childCount; i++)
			{
				Transform child = this.scrollRect.Content.GetChild(i);
				Game.Components.Tutorial.GuidingChapterItem chapterItem = child.GetComponent<Game.Components.Tutorial.GuidingChapterItem>();
				bool flag = chapterItem != null;
				if (flag)
				{
					chapterItem.selected.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06005A58 RID: 23128 RVA: 0x0029EC10 File Offset: 0x0029CE10
		private void CalcClassTemplateIdDict()
		{
			foreach (short templateId in this.totalTemplateIdList)
			{
				Config.GuidingChapterItem config = GuidingChapter.Instance[templateId];
				bool flag = !this.classTemplateIdDict.ContainsKey(config.Class);
				if (flag)
				{
					this.classTemplateIdDict.Add(config.Class, new List<short>
					{
						templateId
					});
				}
				else
				{
					bool flag2 = !this.classTemplateIdDict[config.Class].Contains(templateId);
					if (flag2)
					{
						this.classTemplateIdDict[config.Class].Add(templateId);
					}
				}
			}
			this.classTemplateIdList.Clear();
			this.classTemplateIdList = this.classTemplateIdDict.Keys.ToList<short>();
		}

		// Token: 0x06005A59 RID: 23129 RVA: 0x0029ED0C File Offset: 0x0029CF0C
		private List<short> GetClassTemplateIdList(short classTemplateId)
		{
			return this.classTemplateIdDict[classTemplateId];
		}

		// Token: 0x06005A5A RID: 23130 RVA: 0x0029ED2C File Offset: 0x0029CF2C
		private int GetClassTemplateIdIndex(short classTemplateId)
		{
			return this.classTemplateIdList.IndexOf(classTemplateId);
		}

		// Token: 0x04003E34 RID: 15924
		[SerializeField]
		private CRawImage partImage;

		// Token: 0x04003E35 RID: 15925
		[SerializeField]
		private TextMeshProUGUI partDesc;

		// Token: 0x04003E36 RID: 15926
		[SerializeField]
		private TextMeshProUGUI partTitle;

		// Token: 0x04003E37 RID: 15927
		[SerializeField]
		private CToggleGroup classToggleGroup;

		// Token: 0x04003E38 RID: 15928
		[SerializeField]
		private TemplatedContainerAssemblyNew classToggleHolder;

		// Token: 0x04003E39 RID: 15929
		[SerializeField]
		private CButton encyclopediaBtn;

		// Token: 0x04003E3A RID: 15930
		[SerializeField]
		private CButton prevClass;

		// Token: 0x04003E3B RID: 15931
		[SerializeField]
		private CButton nextClass;

		// Token: 0x04003E3C RID: 15932
		[SerializeField]
		private InfinityScroll unlockItemScroll;

		// Token: 0x04003E3D RID: 15933
		[SerializeField]
		private ModIdSwitch pageSwitch;

		// Token: 0x04003E3E RID: 15934
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x04003E3F RID: 15935
		[SerializeField]
		private CButton buttonClearAllNew;

		// Token: 0x04003E40 RID: 15936
		private short _curTemplateId;

		// Token: 0x04003E41 RID: 15937
		private short _curClassTemplateId;

		// Token: 0x04003E42 RID: 15938
		private readonly Dictionary<short, List<short>> classTemplateIdDict = new Dictionary<short, List<short>>();

		// Token: 0x04003E43 RID: 15939
		private List<short> classTemplateIdList = new List<short>();

		// Token: 0x04003E44 RID: 15940
		private List<short> totalTemplateIdList = new List<short>();

		// Token: 0x04003E45 RID: 15941
		private Dictionary<short, sbyte> _triggeredGuidingChapterDictionary = new Dictionary<short, sbyte>();

		// Token: 0x04003E46 RID: 15942
		private readonly string texturePath = "RemakeResources/Textures/GuidingChapter/";
	}
}
