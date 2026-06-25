using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200009E RID: 158
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPTextSpriteHelper : MonoBehaviour
{
	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600056F RID: 1391 RVA: 0x00024890 File Offset: 0x00022A90
	// (set) Token: 0x06000570 RID: 1392 RVA: 0x00024898 File Offset: 0x00022A98
	public string SrcText { get; private set; }

	// Token: 0x06000571 RID: 1393 RVA: 0x000248A1 File Offset: 0x00022AA1
	private void Awake()
	{
		TMPTextSpriteHelper.InitNeedAddBaseSpriteNameList();
		this._textComponent = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x000248B8 File Offset: 0x00022AB8
	private void OnEnable()
	{
		bool waitOnEnableHandle = this._waitOnEnableHandle;
		if (waitOnEnableHandle)
		{
			this._waitOnEnableHandle = false;
			this._delayHandleSpriteCoroutine = SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.HandleSpriteReplace());
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x000248F0 File Offset: 0x00022AF0
	private void OnDisable()
	{
		bool flag = !this.autoClear;
		if (!flag)
		{
			this.ClearAllImages();
		}
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x00024914 File Offset: 0x00022B14
	private void OnDestroy()
	{
		bool flag = null != TMPTextSpriteHelper._yieldHelper && this._delayHandleSpriteCoroutine != null;
		if (flag)
		{
			TMPTextSpriteHelper._yieldHelper.StopCoroutine(this._delayHandleSpriteCoroutine);
			this._delayHandleSpriteCoroutine = null;
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0002495C File Offset: 0x00022B5C
	private static void InitNeedAddBaseSpriteNameList()
	{
		bool flag = TMPTextSpriteHelper._needAddBaseSpriteNameMap != null;
		if (!flag)
		{
			TMPTextSpriteHelper._needAddBaseSpriteNameMap = new Dictionary<string, string>();
			LifeSkillType.Instance.Iterate(delegate(LifeSkillTypeItem e)
			{
				TMPTextSpriteHelper._needAddBaseSpriteNameMap.Add(e.DisplayIcon, "charactermenu3_26_icon_fang");
				return true;
			});
			CombatSkillType.Instance.Iterate(delegate(CombatSkillTypeItem e)
			{
				TMPTextSpriteHelper._needAddBaseSpriteNameMap.Add(e.DisplayIcon, "charactermenu3_26_icon_fang");
				return true;
			});
			WorldCreation.Instance.Iterate(delegate(WorldCreationItem e)
			{
				string[] icons = e.Icons;
				bool flag2 = icons == null || icons.Length <= 0;
				bool result;
				if (flag2)
				{
					result = true;
				}
				else
				{
					foreach (string iconName in e.Icons)
					{
						bool flag3 = string.IsNullOrEmpty(iconName) || TMPTextSpriteHelper._needAddBaseSpriteNameMap.ContainsKey(iconName);
						if (!flag3)
						{
							TMPTextSpriteHelper._needAddBaseSpriteNameMap.Add(iconName, "sp_01_renwudi_3");
						}
					}
					result = true;
				}
				return result;
			});
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x00024A04 File Offset: 0x00022C04
	private string AddSpacesToLinks(string originalText)
	{
		return TMPTextSpriteHelper.linkRegex.Replace(originalText, (Match m) => string.Concat(new string[]
		{
			"<link=\"",
			m.Groups[1].Value,
			"\"> ",
			m.Groups[2].Value,
			" </link>"
		}));
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00024A44 File Offset: 0x00022C44
	private void UpdateLinkText(TextMeshProUGUI textComponent)
	{
		bool flag = !UIManager.Instance.IsFocusElement(UIElement.CharacterMenuLifeRecords);
		if (!flag)
		{
			string newTextWithSpaces = this.AddSpacesToLinks(textComponent.text);
			textComponent.text = newTextWithSpaces;
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00024A80 File Offset: 0x00022C80
	private void StartHandleTextSprite_Internal()
	{
		bool flag = null == this._textComponent;
		if (!flag)
		{
			bool flag2 = string.IsNullOrEmpty(this._textComponent.text);
			if (flag2)
			{
				this.ClearAllImages();
				Action onParseComplete = this.OnParseComplete;
				if (onParseComplete != null)
				{
					onParseComplete();
				}
			}
			else
			{
				bool flag3 = this._spriteDataList == null;
				if (flag3)
				{
					this._spriteDataList = new List<ValueTuple<string, string>>();
				}
				this._spriteDataList.Clear();
				this.UpdateLinkText(this._textComponent);
				List<char> baseCharList = EasyPool.Get<List<char>>();
				List<char> spriteNameCharList = EasyPool.Get<List<char>>();
				for (int i = 0; i < this.SrcText.Length; i++)
				{
					char iterateChar = this.SrcText[i];
					bool flag4 = !this.IsSpriteIndex(i);
					if (flag4)
					{
						baseCharList.Add(iterateChar);
					}
					else
					{
						spriteNameCharList.Clear();
						i += "<SpName=".Length;
						iterateChar = this.SrcText[i];
						for (;;)
						{
							bool flag5 = iterateChar == '>' || i >= this.SrcText.Length;
							if (flag5)
							{
								break;
							}
							spriteNameCharList.Add(iterateChar);
							iterateChar = this.SrcText[++i];
						}
						string spriteName = new string(spriteNameCharList.ToArray());
						spriteName = spriteName.Trim();
						bool flag6 = !string.IsNullOrEmpty(spriteName);
						if (flag6)
						{
							string spriteIndexHexString = "000";
							string startStr = (this.SpriteSizeFitType == TMPTextSpriteHelper.SizeFitType.Static) ? string.Format("<size={0}>", (this.StaticSpriteSize.x / 3f + this.SpriteSpacing) / 0.53f) : string.Empty;
							string endStr = (this.SpriteSizeFitType == TMPTextSpriteHelper.SizeFitType.Static) ? "</size>" : string.Empty;
							string replaceString = string.Format("<alpha=#00>{0}{1}{2}<alpha=#FF>", startStr, spriteIndexHexString, endStr);
							this._spriteDataList.Add(new ValueTuple<string, string>(spriteIndexHexString, spriteName));
							baseCharList.AddRange(replaceString.ToCharArray());
						}
					}
				}
				bool flag7 = this._spriteDataList.Count > 0;
				if (flag7)
				{
					this._textComponent.SetText(baseCharList.ToArray());
					bool flag8 = !base.gameObject.activeInHierarchy;
					if (flag8)
					{
						this._waitOnEnableHandle = true;
					}
					else
					{
						this._delayHandleSpriteCoroutine = SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.HandleSpriteReplace());
					}
				}
				else
				{
					this.ClearAllImages();
					Action onParseComplete2 = this.OnParseComplete;
					if (onParseComplete2 != null)
					{
						onParseComplete2();
					}
				}
			}
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x00024D10 File Offset: 0x00022F10
	private bool IsSpriteIndex(int index)
	{
		string text = this.SrcText;
		for (int i = 0; i < "<SpName=".Length; i++)
		{
			bool flag = text[index + i] != "<SpName="[i];
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00024D68 File Offset: 0x00022F68
	private IEnumerator HandleSpriteReplace()
	{
		this._imageCache.Clear();
		foreach (CImage image in base.transform.GetComponentsInTopChildren(true))
		{
			bool flag = null == image;
			if (!flag)
			{
				bool flag2 = !this._imageCache.ContainsKey(image.name);
				if (flag2)
				{
					this._imageCache.Add(image.name, new List<CImage>
					{
						image
					});
				}
				bool flag3 = !this._imageCache[image.name].Contains(image);
				if (flag3)
				{
					this._imageCache[image.name].Add(image);
				}
				image = null;
			}
		}
		CImage[] array = null;
		yield return null;
		yield return null;
		yield return new WaitUntil(() => null != this._textComponent && this._textComponent.textInfo != null);
		bool flag4 = !this._spriteDataList.CheckIndex(0) || this._textComponent.textInfo.characterInfo.Length == 0;
		if (flag4)
		{
			this.ClearAllImages();
			Action onParseComplete = this.OnParseComplete;
			if (onParseComplete != null)
			{
				onParseComplete();
			}
			this._delayHandleSpriteCoroutine = null;
			yield break;
		}
		int spriteIndex = 0;
		ValueTuple<string, string> valueTuple = this._spriteDataList[spriteIndex];
		string hexString = valueTuple.Item1;
		string spriteName = valueTuple.Item2;
		int i = 0;
		int max = this._textComponent.textInfo.characterInfo.Length;
		while (i < max)
		{
			bool flag5 = this._textComponent.textInfo.characterInfo[i].color.a == 0;
			int num;
			if (flag5)
			{
				bool matchFlag = true;
				for (int j = 0; j < hexString.Length; j = num)
				{
					bool flag6 = this._textComponent.textInfo.characterInfo[i + j].color.a > 0;
					if (flag6)
					{
						matchFlag = false;
						break;
					}
					bool flag7 = this._textComponent.textInfo.characterInfo[i + j].character != hexString[j];
					if (flag7)
					{
						matchFlag = false;
						break;
					}
					num = j + 1;
				}
				bool flag8 = matchFlag;
				if (flag8)
				{
					TMP_CharacterInfo charInfoA = this._textComponent.textInfo.characterInfo[i];
					int nextCharIndex = Mathf.Min(max - 1, i + hexString.Length - 1);
					TMP_CharacterInfo charInfoB = this._textComponent.textInfo.characterInfo[nextCharIndex];
					List<CImage> imageList;
					bool flag9 = this._imageCache.TryGetValue(spriteName, out imageList);
					CImage image2;
					GameObject obj;
					if (flag9)
					{
						image2 = imageList[0];
						obj = image2.gameObject;
						imageList.RemoveAt(0);
						bool flag10 = imageList.Count <= 0;
						if (flag10)
						{
							this._imageCache.Remove(spriteName);
						}
					}
					else
					{
						obj = new GameObject(spriteName, new Type[]
						{
							typeof(RectTransform),
							typeof(LayoutElement),
							typeof(CImage)
						});
						image2 = obj.GetComponent<CImage>();
						image2.rectTransform.SetParent(base.transform, false);
					}
					Vector2 size = Vector2.zero;
					bool flag11 = charInfoA.lineNumber == charInfoB.lineNumber;
					Vector3 localPos;
					if (flag11)
					{
						size.x = charInfoB.topRight.x - charInfoA.bottomLeft.x;
						size.y = charInfoB.topRight.y - charInfoA.bottomLeft.y;
						localPos = 0.5f * (charInfoA.bottomLeft + charInfoB.topRight);
					}
					else
					{
						size.x = Mathf.Max(charInfoB.topRight.x - charInfoB.bottomLeft.x, this._textComponent.fontSize);
						size.y = Mathf.Max(charInfoB.topRight.y - charInfoB.bottomLeft.y, this._textComponent.fontSize);
						localPos = 0.5f * (charInfoB.bottomLeft + charInfoB.topRight);
					}
					bool flag12 = this.SpriteSizeFitType == TMPTextSpriteHelper.SizeFitType.Static;
					if (flag12)
					{
						size = this.StaticSpriteSize;
					}
					obj.GetComponent<LayoutElement>().ignoreLayout = true;
					image2.rectTransform.SetSize(size);
					image2.preserveAspect = true;
					bool flag13 = this.CustomSetSpriteHandler == null || !this.CustomSetSpriteHandler(image2, spriteName);
					if (flag13)
					{
						bool flag14 = !this.HandleContentSprite(image2, spriteName);
						if (flag14)
						{
							image2.SetSpriteOnly(spriteName, false, null);
							foreach (object obj2 in image2.rectTransform)
							{
								Transform child = (Transform)obj2;
								Object.Destroy(child.gameObject);
								child = null;
							}
							IEnumerator enumerator = null;
						}
					}
					image2.rectTransform.localPosition = localPos + this.Offset;
					bool flag15 = this.SpriteSizeFitType == TMPTextSpriteHelper.SizeFitType.Native;
					if (flag15)
					{
						image2.SetNativeSize();
					}
					obj.SetActive(true);
					i += hexString.Length - 1;
					bool flag16 = i >= max;
					if (flag16)
					{
						break;
					}
					num = spriteIndex;
					spriteIndex = num + 1;
					bool flag17 = !this._spriteDataList.CheckIndex(spriteIndex);
					if (flag17)
					{
						break;
					}
					valueTuple = this._spriteDataList[spriteIndex];
					hexString = valueTuple.Item1;
					spriteName = valueTuple.Item2;
					charInfoA = default(TMP_CharacterInfo);
					charInfoB = default(TMP_CharacterInfo);
					obj = null;
					image2 = null;
					imageList = null;
					size = default(Vector2);
					localPos = default(Vector3);
				}
			}
			num = i + 1;
			i = num;
		}
		bool flag18 = this._imageCache.Count > 0;
		if (flag18)
		{
			foreach (KeyValuePair<string, List<CImage>> pair in this._imageCache)
			{
				foreach (CImage image3 in pair.Value)
				{
					bool flag19 = image3 != null;
					if (flag19)
					{
						image3.gameObject.SetActive(false);
					}
					image3 = null;
				}
				List<CImage>.Enumerator enumerator3 = default(List<CImage>.Enumerator);
				pair = default(KeyValuePair<string, List<CImage>>);
			}
			Dictionary<string, List<CImage>>.Enumerator enumerator2 = default(Dictionary<string, List<CImage>>.Enumerator);
		}
		this._delayHandleSpriteCoroutine = null;
		Action onParseComplete2 = this.OnParseComplete;
		if (onParseComplete2 != null)
		{
			onParseComplete2();
		}
		yield break;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00024D78 File Offset: 0x00022F78
	private void ClearAllImages()
	{
		foreach (object obj in base.transform)
		{
			Transform child = (Transform)obj;
			CImage image = child.GetComponent<CImage>();
			bool flag = null != image;
			if (flag)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x00024DF4 File Offset: 0x00022FF4
	public void Parse()
	{
		bool flag = null == this._textComponent;
		if (flag)
		{
			this.Awake();
		}
		bool flag2 = null == TMPTextSpriteHelper._yieldHelper;
		if (flag2)
		{
			TMPTextSpriteHelper._yieldHelper = SingletonObject.getInstance<YieldHelper>();
		}
		bool flag3 = null != TMPTextSpriteHelper._yieldHelper && this._delayHandleSpriteCoroutine != null;
		if (flag3)
		{
			TMPTextSpriteHelper._yieldHelper.StopCoroutine(this._delayHandleSpriteCoroutine);
		}
		this.SrcText = this._textComponent.text;
		this.StartHandleTextSprite_Internal();
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00024E7C File Offset: 0x0002307C
	private bool HandleContentSprite(CImage image, string spriteName)
	{
		bool flag = !this.showBaseSprite;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			string backSpriteName;
			bool flag2 = !TMPTextSpriteHelper._needAddBaseSpriteNameMap.TryGetValue(spriteName, out backSpriteName);
			if (flag2)
			{
				result = false;
			}
			else
			{
				image.SetSpriteOnly(backSpriteName, false, null);
				CImage iconImage = null;
				Transform iconChild = image.transform.Find("icon");
				bool flag3 = null != iconChild;
				if (flag3)
				{
					iconImage = iconChild.GetComponent<CImage>();
				}
				bool flag4 = null == iconImage;
				if (flag4)
				{
					iconImage = Object.Instantiate<CImage>(image, image.rectTransform, false);
					iconImage.name = "icon";
				}
				iconImage.rectTransform.localPosition = Vector3.zero;
				float ratio = image.rectTransform.rect.width / image.sprite.rect.width;
				iconImage.SetSpriteOnly(spriteName, false, delegate
				{
					Vector2 size = iconImage.sprite.rect.size * ratio;
					size.x = Mathf.Min(size.x, image.rectTransform.rect.width);
					size.y = Mathf.Min(size.y, image.rectTransform.rect.height);
					iconImage.rectTransform.SetSize(size);
				});
				iconImage.gameObject.SetActive(true);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00024FD4 File Offset: 0x000231D4
	public static string GetStringWithTextSpriteTag(string spriteName)
	{
		return "<SpName=" + spriteName + ">";
	}

	// Token: 0x0400046C RID: 1132
	private const string SpriteStartSign = "<SpName=";

	// Token: 0x0400046D RID: 1133
	private const string ReplaceBaseString = "<alpha=#00>{0}{1}{2}<alpha=#FF>";

	// Token: 0x0400046E RID: 1134
	private const string SizeTagStartString = "<size={0}>";

	// Token: 0x0400046F RID: 1135
	private const string SizeTagEndString = "</size>";

	// Token: 0x04000470 RID: 1136
	private static YieldHelper _yieldHelper;

	// Token: 0x04000471 RID: 1137
	private static readonly Regex linkRegex = new Regex("<link=\"([^\"]+)\">([^<]+)</link>", RegexOptions.Compiled);

	// Token: 0x04000472 RID: 1138
	private TextMeshProUGUI _textComponent;

	// Token: 0x04000473 RID: 1139
	private List<ValueTuple<string, string>> _spriteDataList;

	// Token: 0x04000474 RID: 1140
	private Dictionary<string, List<CImage>> _imageCache = new Dictionary<string, List<CImage>>();

	// Token: 0x04000475 RID: 1141
	private Coroutine _delayHandleSpriteCoroutine;

	// Token: 0x04000476 RID: 1142
	[NonSerialized]
	public Action OnParseComplete;

	// Token: 0x04000477 RID: 1143
	[SerializeField]
	internal bool autoClear = true;

	// Token: 0x04000478 RID: 1144
	[SerializeField]
	public bool showBaseSprite = true;

	// Token: 0x04000479 RID: 1145
	public TMPTextSpriteHelper.SizeFitType SpriteSizeFitType = TMPTextSpriteHelper.SizeFitType.Static;

	// Token: 0x0400047A RID: 1146
	[Tooltip("仅Static适配模式生效")]
	public Vector2 StaticSpriteSize = Vector2.one * 30f;

	// Token: 0x0400047B RID: 1147
	public Vector2 Offset = Vector2.zero;

	// Token: 0x0400047C RID: 1148
	[Tooltip("图片间距，仅Static适配模式生效，单位是像素")]
	[Range(0f, 100f)]
	public float SpriteSpacing;

	// Token: 0x0400047E RID: 1150
	public Func<CImage, string, bool> CustomSetSpriteHandler;

	// Token: 0x0400047F RID: 1151
	private static Dictionary<string, string> _needAddBaseSpriteNameMap;

	// Token: 0x04000480 RID: 1152
	private bool _waitOnEnableHandle;

	// Token: 0x02001109 RID: 4361
	public enum SizeFitType
	{
		// Token: 0x04009553 RID: 38227
		Native,
		// Token: 0x04009554 RID: 38228
		Auto,
		// Token: 0x04009555 RID: 38229
		Static
	}
}
