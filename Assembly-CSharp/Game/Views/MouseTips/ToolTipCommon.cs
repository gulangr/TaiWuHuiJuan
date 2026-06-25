using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000831 RID: 2097
	public class ToolTipCommon : MouseTipBase
	{
		// Token: 0x17000C58 RID: 3160
		// (get) Token: 0x0600667A RID: 26234 RVA: 0x002EBAFE File Offset: 0x002E9CFE
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600667B RID: 26235 RVA: 0x002EBB04 File Offset: 0x002E9D04
		public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
		{
			CommonTipBaseRuntime runtime;
			return argumentBox.Get<CommonTipBaseRuntime>("Runtime", out runtime) && runtime != null;
		}

		// Token: 0x0600667C RID: 26236 RVA: 0x002EBB2C File Offset: 0x002E9D2C
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			this.Refresh(argsBox);
		}

		// Token: 0x0600667D RID: 26237 RVA: 0x002EBB43 File Offset: 0x002E9D43
		public override void Refresh(ArgumentBox argumentBox)
		{
			this.SyncRuntime(argumentBox);
			base.Refresh();
			this.RefreshCommonTip();
		}

		// Token: 0x0600667E RID: 26238 RVA: 0x002EBB5C File Offset: 0x002E9D5C
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshCommonTip();
		}

		// Token: 0x0600667F RID: 26239 RVA: 0x002EBB70 File Offset: 0x002E9D70
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
			bool flag = this._runtime != null && base.gameObject.activeInHierarchy;
			if (flag)
			{
				this.RefreshCommonTip();
			}
		}

		// Token: 0x06006680 RID: 26240 RVA: 0x002EBBA8 File Offset: 0x002E9DA8
		private void RefreshCommonTip()
		{
			bool flag = this._runtime == null;
			if (!flag)
			{
				this._config = ToolTipCommon.LoadConfig(this._runtime.ConfigLine);
				this._activeParagraphCount = 0;
				this._activeAtomCounts.Clear();
				TextMeshProUGUI label = this.titleLabel;
				CommonTipConfig config = this._config;
				ToolTipCommon.SetLabelText(label, this.ResolveText((config != null) ? config.Title : null));
				CommonTipConfig config2 = this._config;
				bool flag2 = ((config2 != null) ? config2.Paragraphs : null) != null;
				if (flag2)
				{
					foreach (CommonTipParagraphConfig paragraph in this._config.Paragraphs)
					{
						bool flag3 = paragraph == null;
						if (!flag3)
						{
							bool flag4 = !string.IsNullOrEmpty(paragraph.Name) && !this._runtime.ShouldShowParagraph(paragraph.Name);
							if (!flag4)
							{
								bool flag5 = paragraph.Atoms == null;
								if (!flag5)
								{
									CommonParagraph paragraphView = null;
									foreach (CommonTipAtomConfig atom in paragraph.Atoms)
									{
										bool flag6 = atom == null;
										if (!flag6)
										{
											CommonTipAtomType atomType = ToolTipCommon.ParseAtomType(atom.Type);
											bool flag7 = atomType == CommonTipAtomType.Invalid;
											if (flag7)
											{
												Debug.LogWarning("Unsupported CommonTip atom type: " + atom.Type);
											}
											else
											{
												bool flag8 = !string.IsNullOrEmpty(atom.Name) && !this._runtime.ShouldShowAtom(paragraph.Name, atom.Name);
												if (!flag8)
												{
													if (paragraphView == null)
													{
														paragraphView = this.AcquireParagraph();
													}
													paragraphView.SetBackground(paragraph.Background);
													this.CreateAtom(atom, atomType, paragraphView);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				CommonTipConfig config3 = this._config;
				bool flag9 = !string.IsNullOrEmpty((config3 != null) ? config3.EncyclopediaTipLink : null);
				if (flag9)
				{
					this._encyclopediaLinkKey = this._config.EncyclopediaTipLink;
					this.encyclopediaLinkGo.SetActive(true);
				}
				else
				{
					this._encyclopediaLinkKey = null;
					this.encyclopediaLinkGo.SetActive(false);
				}
				this.ReleaseUnusedParagraphs();
				this.ReleaseUnusedAtoms();
			}
		}

		// Token: 0x06006681 RID: 26241 RVA: 0x002EBE3C File Offset: 0x002EA03C
		private CommonParagraph AcquireParagraph()
		{
			bool flag = this._activeParagraphCount >= this._paragraphPool.Count;
			if (flag)
			{
				CommonParagraph paragraphView = Object.Instantiate<CommonParagraph>(this.paragraphTemplate, this.paragraphContainer);
				paragraphView.gameObject.SetActive(false);
				this._paragraphPool.Add(paragraphView);
			}
			List<CommonParagraph> paragraphPool = this._paragraphPool;
			int activeParagraphCount = this._activeParagraphCount;
			this._activeParagraphCount = activeParagraphCount + 1;
			CommonParagraph activeParagraph = paragraphPool[activeParagraphCount];
			activeParagraph.gameObject.SetActive(true);
			activeParagraph.transform.SetAsLastSibling();
			return activeParagraph;
		}

		// Token: 0x06006682 RID: 26242 RVA: 0x002EBED0 File Offset: 0x002EA0D0
		private void ReleaseUnusedParagraphs()
		{
			for (int i = this._activeParagraphCount; i < this._paragraphPool.Count; i++)
			{
				this._paragraphPool[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x06006683 RID: 26243 RVA: 0x002EBF18 File Offset: 0x002EA118
		private T AcquireAtom<T>(CommonTipAtomType atomType, T template, Transform atomContainer) where T : CommonAtomBase
		{
			List<CommonAtomBase> atomPool;
			bool flag = !this._atomPools.TryGetValue(atomType, out atomPool);
			if (flag)
			{
				atomPool = new List<CommonAtomBase>();
				this._atomPools.Add(atomType, atomPool);
			}
			int activeCount;
			this._activeAtomCounts.TryGetValue(atomType, out activeCount);
			bool flag2 = activeCount < atomPool.Count;
			T atomView;
			if (flag2)
			{
				atomView = (atomPool[activeCount] as T);
			}
			else
			{
				atomView = Object.Instantiate<T>(template, atomContainer);
				atomView.gameObject.SetActive(false);
				atomPool.Add(atomView);
			}
			this._activeAtomCounts[atomType] = activeCount + 1;
			atomView.transform.SetParent(atomContainer, false);
			atomView.gameObject.SetActive(true);
			atomView.transform.SetAsLastSibling();
			return atomView;
		}

		// Token: 0x06006684 RID: 26244 RVA: 0x002EBFFC File Offset: 0x002EA1FC
		private void ReleaseUnusedAtoms()
		{
			foreach (KeyValuePair<CommonTipAtomType, List<CommonAtomBase>> pair in this._atomPools)
			{
				int activeCount;
				this._activeAtomCounts.TryGetValue(pair.Key, out activeCount);
				List<CommonAtomBase> atomPool = pair.Value;
				for (int i = activeCount; i < atomPool.Count; i++)
				{
					atomPool[i].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006685 RID: 26245 RVA: 0x002EC09C File Offset: 0x002EA29C
		private void CreateAtom(CommonTipAtomConfig atom, CommonTipAtomType atomType, CommonParagraph paragraphView)
		{
			bool flag = atom == null;
			if (!flag)
			{
				switch (atomType)
				{
				case CommonTipAtomType.SubTitle:
					this.CreateSubTitle(atom, paragraphView);
					break;
				case CommonTipAtomType.SimpleContent:
					this.CreateSimpleContent(atom, paragraphView);
					break;
				case CommonTipAtomType.KeyValueItem:
					this.CreateKeyValueItem(atom, paragraphView);
					break;
				case CommonTipAtomType.Space:
					this.CreateSpace(atom, paragraphView);
					break;
				case CommonTipAtomType.SubTitle2:
					this.CreateSubTitle2(atom, paragraphView);
					break;
				}
			}
		}

		// Token: 0x06006686 RID: 26246 RVA: 0x002EC110 File Offset: 0x002EA310
		private void CreateSubTitle(CommonTipAtomConfig atom, CommonParagraph paragraphView)
		{
			CommonAtomSubTitle line = this.AcquireAtom<CommonAtomSubTitle>(CommonTipAtomType.SubTitle, this.subTitleTemplate, paragraphView.AtomContainer);
			line.SetMarginLeft(atom.MarginLeft);
			line.SetText(this.ResolveText(atom.Content));
		}

		// Token: 0x06006687 RID: 26247 RVA: 0x002EC154 File Offset: 0x002EA354
		private void CreateSubTitle2(CommonTipAtomConfig atom, CommonParagraph paragraphView)
		{
			CommonAtomSubTitle line = this.AcquireAtom<CommonAtomSubTitle>(CommonTipAtomType.SubTitle2, this.subTitle2Template, paragraphView.AtomContainer);
			line.SetMarginLeft(atom.MarginLeft);
			line.SetText(this.ResolveText(atom.Content));
		}

		// Token: 0x06006688 RID: 26248 RVA: 0x002EC198 File Offset: 0x002EA398
		private void CreateSimpleContent(CommonTipAtomConfig atom, CommonParagraph paragraphView)
		{
			CommonTipSimpleContentLineView line = this.AcquireAtom<CommonTipSimpleContentLineView>(CommonTipAtomType.SimpleContent, this.simpleContentTemplate, paragraphView.AtomContainer);
			line.SetMarginLeft(atom.MarginLeft);
			line.SetText(this.ResolveText(atom.Content));
		}

		// Token: 0x06006689 RID: 26249 RVA: 0x002EC1DC File Offset: 0x002EA3DC
		private void CreateKeyValueItem(CommonTipAtomConfig atom, CommonParagraph paragraphView)
		{
			CommonAtomTitleAndValue line = this.AcquireAtom<CommonAtomTitleAndValue>(CommonTipAtomType.KeyValueItem, this.itemTemplate, paragraphView.AtomContainer);
			line.SetMarginLeft(atom.MarginLeft);
			line.SetTexts(this.ResolveText(atom.Title), this.ResolveText(atom.Value));
		}

		// Token: 0x0600668A RID: 26250 RVA: 0x002EC22C File Offset: 0x002EA42C
		private void CreateSpace(CommonTipAtomConfig atom, CommonParagraph paragraphView)
		{
			CommonAtomSpace space = this.AcquireAtom<CommonAtomSpace>(CommonTipAtomType.Space, this.spaceTemplate, paragraphView.AtomContainer);
			space.SetLevel(atom.Level);
		}

		// Token: 0x0600668B RID: 26251 RVA: 0x002EC25C File Offset: 0x002EA45C
		private static CommonTipConfig LoadConfig(CommonTipItem configLine)
		{
			bool flag = configLine == null || string.IsNullOrEmpty(configLine.Path);
			CommonTipConfig result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string cacheKey = string.Format("{0}:{1}", LocalStringManager.CurLanguageType, configLine.Path);
				CommonTipConfig cachedConfig;
				bool flag2 = ToolTipCommon.ConfigCache.TryGetValue(cacheKey, out cachedConfig);
				if (flag2)
				{
					result = cachedConfig;
				}
				else
				{
					string path = Path.Combine(Application.streamingAssetsPath, string.Format("Language_{0}", LocalStringManager.CurLanguageType), "CommonTip", configLine.Path + ".json");
					bool flag3 = !File.Exists(path);
					if (flag3)
					{
						Debug.LogWarning("CommonTip config not found: " + path);
						result = null;
					}
					else
					{
						try
						{
							string json = File.ReadAllText(path);
							json = Regex.Replace(json, "^\\s*//.*$", "", RegexOptions.Multiline);
							CommonTipConfig config = JsonConvert.DeserializeObject<CommonTipConfig>(json) ?? new CommonTipConfig();
							ToolTipCommon.ConfigCache[cacheKey] = config;
							result = config;
						}
						catch (Exception e)
						{
							Debug.LogError(string.Format("Failed to load CommonTip config: {0}\n{1}", path, e));
							result = null;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600668C RID: 26252 RVA: 0x002EC388 File Offset: 0x002EA588
		private string ResolveText(string content)
		{
			bool flag = string.IsNullOrEmpty(content);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = ToolTipCommon.ArgumentPattern.Replace(content, delegate(Match match)
				{
					string key = match.Groups["key"].Value;
					CommonTipBaseRuntime runtime = this._runtime;
					return ((runtime != null) ? runtime.GetArgument(key) : null) ?? match.Value;
				});
			}
			return result;
		}

		// Token: 0x0600668D RID: 26253 RVA: 0x002EC3C4 File Offset: 0x002EA5C4
		private static CommonTipAtomType ParseAtomType(string type)
		{
			CommonTipAtomType atomType;
			return Enum.TryParse<CommonTipAtomType>(type, true, out atomType) ? atomType : CommonTipAtomType.Invalid;
		}

		// Token: 0x0600668E RID: 26254 RVA: 0x002EC3E8 File Offset: 0x002EA5E8
		private static void SetLabelText(TextMeshProUGUI label, string content)
		{
			label.text = (((content != null) ? content.ColorReplace() : null) ?? string.Empty);
			TMPTextSpriteHelper spriteHelper = label.GetComponent<TMPTextSpriteHelper>();
			if (spriteHelper != null)
			{
				spriteHelper.Parse();
			}
		}

		// Token: 0x0600668F RID: 26255 RVA: 0x002EC425 File Offset: 0x002EA625
		private void DeactivateParagraphs()
		{
			this._activeParagraphCount = 0;
			this._activeAtomCounts.Clear();
			this.ReleaseUnusedParagraphs();
			this.ReleaseUnusedAtoms();
		}

		// Token: 0x06006690 RID: 26256 RVA: 0x002EC449 File Offset: 0x002EA649
		protected override void OnDisable()
		{
			CommonTipBaseRuntime runtime = this._runtime;
			if (runtime != null)
			{
				runtime.Detach();
			}
			this._runtime = null;
			base.OnDisable();
			this.DeactivateParagraphs();
		}

		// Token: 0x06006691 RID: 26257 RVA: 0x002EC474 File Offset: 0x002EA674
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false) && !string.IsNullOrEmpty(this._encyclopediaLinkKey);
			if (flag)
			{
				PropertyInfo prop = typeof(EncyclopediaTipLink.DefValue).GetProperty(this._encyclopediaLinkKey, BindingFlags.Static | BindingFlags.Public);
				EncyclopediaTipLinkItem linkItem;
				bool flag2;
				if (prop != null)
				{
					linkItem = (prop.GetValue(null) as EncyclopediaTipLinkItem);
					flag2 = (linkItem != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					ViewEncyclopediaPanel.OpenLink(linkItem);
				}
			}
		}

		// Token: 0x06006692 RID: 26258 RVA: 0x002EC4F0 File Offset: 0x002EA6F0
		private void SyncRuntime(ArgumentBox argumentBox)
		{
			CommonTipBaseRuntime runtime;
			bool hasRuntime = argumentBox.Get<CommonTipBaseRuntime>("Runtime", out runtime) && runtime != null;
			bool flag = !hasRuntime;
			if (flag)
			{
				CommonTipBaseRuntime runtime2 = this._runtime;
				if (runtime2 != null)
				{
					runtime2.Detach();
				}
				this._runtime = null;
			}
			else
			{
				bool flag2 = this._runtime != runtime;
				if (flag2)
				{
					CommonTipBaseRuntime runtime3 = this._runtime;
					if (runtime3 != null)
					{
						runtime3.Detach();
					}
					this._runtime = runtime;
				}
				this._runtime.Attach(this);
			}
		}

		// Token: 0x040047B9 RID: 18361
		private const string CommonTipFolderName = "CommonTip";

		// Token: 0x040047BA RID: 18362
		private static readonly Regex ArgumentPattern = new Regex("\\{(?<key>[A-Za-z0-9_.]+)\\}", RegexOptions.Compiled);

		// Token: 0x040047BB RID: 18363
		private static readonly Dictionary<string, CommonTipConfig> ConfigCache = new Dictionary<string, CommonTipConfig>();

		// Token: 0x040047BC RID: 18364
		private readonly List<CommonParagraph> _paragraphPool = new List<CommonParagraph>();

		// Token: 0x040047BD RID: 18365
		private readonly Dictionary<CommonTipAtomType, List<CommonAtomBase>> _atomPools = new Dictionary<CommonTipAtomType, List<CommonAtomBase>>();

		// Token: 0x040047BE RID: 18366
		private readonly Dictionary<CommonTipAtomType, int> _activeAtomCounts = new Dictionary<CommonTipAtomType, int>();

		// Token: 0x040047BF RID: 18367
		private CommonTipBaseRuntime _runtime;

		// Token: 0x040047C0 RID: 18368
		private CommonTipConfig _config;

		// Token: 0x040047C1 RID: 18369
		private int _activeParagraphCount;

		// Token: 0x040047C2 RID: 18370
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x040047C3 RID: 18371
		[SerializeField]
		private Transform paragraphContainer;

		// Token: 0x040047C4 RID: 18372
		[SerializeField]
		private CommonParagraph paragraphTemplate;

		// Token: 0x040047C5 RID: 18373
		[SerializeField]
		private CommonAtomSubTitle subTitleTemplate;

		// Token: 0x040047C6 RID: 18374
		[SerializeField]
		private CommonAtomSubTitle subTitle2Template;

		// Token: 0x040047C7 RID: 18375
		[SerializeField]
		private CommonTipSimpleContentLineView simpleContentTemplate;

		// Token: 0x040047C8 RID: 18376
		[SerializeField]
		private CommonAtomTitleAndValue itemTemplate;

		// Token: 0x040047C9 RID: 18377
		[SerializeField]
		private CommonAtomSpace spaceTemplate;

		// Token: 0x040047CA RID: 18378
		[SerializeField]
		private GameObject encyclopediaLinkGo;

		// Token: 0x040047CB RID: 18379
		private string _encyclopediaLinkKey;
	}
}
