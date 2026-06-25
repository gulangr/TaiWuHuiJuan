using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Game.Views.Encyclopedia.Utilities;
using Spine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000026 RID: 38
public static class Extentions
{
	// Token: 0x0600011B RID: 283 RVA: 0x000084E0 File Offset: 0x000066E0
	public static bool IsNullOrEmpty(this string str)
	{
		return string.IsNullOrEmpty(str);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x000084F8 File Offset: 0x000066F8
	public static void ForEach<T>(this T[] array, Func<int, T, bool> _action)
	{
		int i = 0;
		int max = array.Length;
		while (i < max)
		{
			bool flag = _action(i, array[i]);
			if (flag)
			{
				break;
			}
			i++;
		}
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00008534 File Offset: 0x00006734
	public static string FixPath(this string path)
	{
		return path.Replace('/', '\\');
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00008550 File Offset: 0x00006750
	public static string PathFix(this string path)
	{
		return path.Replace('\\', '/');
	}

	// Token: 0x0600011F RID: 287 RVA: 0x0000856C File Offset: 0x0000676C
	public static bool HasInvalidCharForFileName(this string fileName)
	{
		return Extentions.InvalidFileNameCharRegex.IsMatch(fileName);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0000858C File Offset: 0x0000678C
	public static T[] GetComponentsInTopChildren<T>(this Transform transform, bool includeInactive = false) where T : Object
	{
		List<T> list = new List<T>();
		bool flag = null == transform;
		T[] result;
		if (flag)
		{
			result = list.ToArray();
		}
		else
		{
			int i = 0;
			int max = transform.childCount;
			while (i < max)
			{
				Transform child = transform.GetChild(i);
				bool flag2 = !child.gameObject.activeSelf && !includeInactive;
				if (!flag2)
				{
					T comp = child.GetComponent<T>();
					bool flag3 = null != comp;
					if (flag3)
					{
						list.Add(comp);
					}
				}
				i++;
			}
			result = list.ToArray();
		}
		return result;
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00008628 File Offset: 0x00006828
	public static void GetComponentsInTopChildren<T>(this Transform transform, IList<T> list, bool includeInactive = false)
	{
		list.Clear();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			bool flag = !child.gameObject.activeSelf && !includeInactive;
			if (!flag)
			{
				T component = child.GetComponent<T>();
				bool flag2 = component != null;
				if (flag2)
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00008698 File Offset: 0x00006898
	public static T[] ChangeArrType<TA, T>(this TA[] arrSrc, Func<TA, T> changeFunc = null)
	{
		T[] tarArr = new T[arrSrc.Length];
		int i = 0;
		int max = arrSrc.Length;
		while (i < max)
		{
			bool flag = changeFunc != null;
			if (flag)
			{
				tarArr[i] = changeFunc(arrSrc[i]);
			}
			else
			{
				tarArr[i] = (T)((object)Convert.ChangeType(arrSrc[i], typeof(T)));
			}
			i++;
		}
		return tarArr;
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00008714 File Offset: 0x00006914
	public static Color HexStringToColor(this string colorStr)
	{
		bool flag = colorStr.IsNullOrEmpty();
		Color result;
		if (flag)
		{
			result = Color.white;
		}
		else
		{
			bool flag2 = colorStr.StartsWith("#");
			if (flag2)
			{
				colorStr = colorStr.Substring(1);
			}
			bool flag3 = colorStr.Length != 6 && colorStr.Length != 8;
			if (flag3)
			{
				result = Color.white;
			}
			else
			{
				Color color;
				bool flag4 = ColorUtility.TryParseHtmlString(colorStr, out color);
				if (flag4)
				{
					result = color;
				}
				else
				{
					char[] stringCharArray = colorStr.ToCharArray();
					int r = Convert.ToInt32(string.Format("0x{0}{1}", stringCharArray[0], stringCharArray[1]), 16);
					int g = Convert.ToInt32(string.Format("0x{0}{1}", stringCharArray[2], stringCharArray[3]), 16);
					int b = Convert.ToInt32(string.Format("0x{0}{1}", stringCharArray[4], stringCharArray[5]), 16);
					int a = 255;
					bool flag5 = colorStr.Length == 8;
					if (flag5)
					{
						a = Convert.ToInt32(string.Format("0x{0}{1}", stringCharArray[6], stringCharArray[7]), 16);
					}
					result = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
				}
			}
		}
		return result;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x0000886C File Offset: 0x00006A6C
	public static string ColorToHexString(this Color color, string starts = "#")
	{
		return starts + ColorUtility.ToHtmlStringRGBA(color);
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000888C File Offset: 0x00006A8C
	public static string ColorReplace(this string srcString)
	{
		bool flag = srcString.IsNullOrEmpty();
		string result;
		if (flag)
		{
			result = srcString;
		}
		else
		{
			srcString = Extentions._colorReg.Replace(srcString, delegate(Match match)
			{
				string key = match.Groups["key"].Value.ToLower();
				string colorString = Colors.Instance[key].ColorToHexString("#");
				return "<color=" + colorString + ">";
			});
			result = srcString;
		}
		return result;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x000088DC File Offset: 0x00006ADC
	public static string RemoveColor(this string srcString)
	{
		bool flag = srcString.IsNullOrEmpty();
		string result;
		if (flag)
		{
			result = srcString;
		}
		else
		{
			Match match = Extentions._colorReg2.Match(srcString);
			result = (match.Success ? match.Groups[1].Value : srcString);
		}
		return result;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00008924 File Offset: 0x00006B24
	public static string RemoveColorTags(this string srcString)
	{
		bool flag = string.IsNullOrEmpty(srcString);
		string result;
		if (flag)
		{
			result = srcString;
		}
		else
		{
			result = Extentions._colorReg3.Replace(srcString, "");
		}
		return result;
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00008954 File Offset: 0x00006B54
	public static string RemoveAllRichTags(this string text)
	{
		bool flag = string.IsNullOrEmpty(text);
		string result;
		if (flag)
		{
			result = text;
		}
		else
		{
			result = Extentions._richTextRegex.Replace(text, "");
		}
		return result;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00008984 File Offset: 0x00006B84
	public static string ApplyHighlightColor(this string srcString, OptimizedHtmlPatternMatcher matcher, bool selecting, int matchingIndex)
	{
		matcher.Sb.Clear();
		int processing = 0;
		int num;
		foreach (ValueTuple<int, int, int> valueTuple in matcher.FindMatches(srcString))
		{
			int index = valueTuple.Item1;
			int start = valueTuple.Item2;
			int end = valueTuple.Item3;
			StringBuilder sb = matcher.Sb;
			num = processing;
			StringBuilder stringBuilder = sb.Append(srcString.Substring(num, start - num)).Append((index == matchingIndex && selecting) ? "<mark=#ce5d205a>" : "<mark=#9973315a>");
			num = start;
			stringBuilder.Append(srcString.Substring(num, (processing = end) - num)).Append("</mark>");
		}
		StringBuilder sb2 = matcher.Sb;
		num = processing;
		return sb2.Append(srcString.Substring(num, srcString.Length - num)).Replace("{{{{", "<").Replace("}}}}", ">").Replace("{{}}", "\\").ToString();
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00008AA4 File Offset: 0x00006CA4
	public static string ParseText(this string srcString)
	{
		return srcString.Replace("\\u003c", "<").Replace("\\u003e", ">").Replace("\\u005c", "\\");
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00008AD4 File Offset: 0x00006CD4
	public static string ParseHighlightText(this string srcString)
	{
		return srcString.Replace("\\u003c", "{{{{").Replace("\\u003e", "}}}}").Replace("\\u005c", "{{}}");
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00008B04 File Offset: 0x00006D04
	public static string SetColor(this string str, string color)
	{
		bool flag = color.IsNullOrEmpty();
		string result;
		if (flag)
		{
			result = str;
		}
		else
		{
			result = string.Concat(new string[]
			{
				"<color=#",
				color,
				">",
				str,
				"</color>"
			}).ColorReplace();
		}
		return result;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00008B54 File Offset: 0x00006D54
	public static string SetColor(this string str, Color color)
	{
		return string.Concat(new string[]
		{
			"<color=",
			color.ColorToHexString("#"),
			">",
			str,
			"</color>"
		});
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00008B9B File Offset: 0x00006D9B
	public static string SetValueColor(this short value)
	{
		return ((int)value).SetValueColor();
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00008BA4 File Offset: 0x00006DA4
	public static string SetValueColor(this int value)
	{
		bool flag = value <= 29;
		string color;
		if (flag)
		{
			color = "8E8E8E";
		}
		else
		{
			bool flag2 = value <= 39;
			if (flag2)
			{
				color = "FBFBFB";
			}
			else
			{
				bool flag3 = value <= 49;
				if (flag3)
				{
					color = "6DB75F";
				}
				else
				{
					bool flag4 = value <= 59;
					if (flag4)
					{
						color = "8FBAE7";
					}
					else
					{
						bool flag5 = value <= 69;
						if (flag5)
						{
							color = "63CED0";
						}
						else
						{
							bool flag6 = value <= 79;
							if (flag6)
							{
								color = "AE5AC8";
							}
							else
							{
								bool flag7 = value <= 89;
								if (flag7)
								{
									color = "E3C66D";
								}
								else
								{
									bool flag8 = value <= 99;
									if (flag8)
									{
										color = "F26A34";
									}
									else
									{
										color = "E4504D";
									}
								}
							}
						}
					}
				}
			}
		}
		return string.Concat(new string[]
		{
			"<color=#",
			color,
			">",
			value.ToString(),
			"</color>"
		}).ColorReplace();
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00008CC0 File Offset: 0x00006EC0
	public static string SetColorByValue(this int value)
	{
		bool flag = value <= 29;
		string color;
		if (flag)
		{
			color = "8E8E8E";
		}
		else
		{
			bool flag2 = value <= 39;
			if (flag2)
			{
				color = "FBFBFB";
			}
			else
			{
				bool flag3 = value <= 49;
				if (flag3)
				{
					color = "6DB75F";
				}
				else
				{
					bool flag4 = value <= 59;
					if (flag4)
					{
						color = "8FBAE7";
					}
					else
					{
						bool flag5 = value <= 69;
						if (flag5)
						{
							color = "63CED0";
						}
						else
						{
							bool flag6 = value <= 79;
							if (flag6)
							{
								color = "AE5AC8";
							}
							else
							{
								bool flag7 = value <= 89;
								if (flag7)
								{
									color = "E3C66D";
								}
								else
								{
									bool flag8 = value <= 99;
									if (flag8)
									{
										color = "F26A34";
									}
									else
									{
										color = "E4504D";
									}
								}
							}
						}
					}
				}
			}
		}
		return string.Concat(new string[]
		{
			"<color=#",
			color,
			">",
			value.ToString(),
			"</color>"
		}).ColorReplace();
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00008DDC File Offset: 0x00006FDC
	public static string SetGradeColor(this string str, int gradeLevel)
	{
		string color = "E4504D";
		bool flag = Colors.Instance.GradeColors.CheckIndex(gradeLevel);
		if (flag)
		{
			color = Colors.Instance.GradeColors[gradeLevel].ColorToHexString(string.Empty);
		}
		return string.Concat(new string[]
		{
			"<color=#",
			color,
			">",
			str,
			"</color>"
		}).ColorReplace();
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00008E54 File Offset: 0x00007054
	public static string SetConsummateColor(this string str, int consummateLevel)
	{
		bool flag = consummateLevel <= 3;
		string color;
		if (flag)
		{
			color = "8E8E8E";
		}
		else
		{
			bool flag2 = consummateLevel <= 5;
			if (flag2)
			{
				color = "FBFBFB";
			}
			else
			{
				bool flag3 = consummateLevel <= 7;
				if (flag3)
				{
					color = "6DB75F";
				}
				else
				{
					bool flag4 = consummateLevel <= 9;
					if (flag4)
					{
						color = "8FBAE7";
					}
					else
					{
						bool flag5 = consummateLevel <= 11;
						if (flag5)
						{
							color = "63CED0";
						}
						else
						{
							bool flag6 = consummateLevel <= 13;
							if (flag6)
							{
								color = "AE5AC8";
							}
							else
							{
								bool flag7 = consummateLevel <= 15;
								if (flag7)
								{
									color = "E3C66D";
								}
								else
								{
									bool flag8 = consummateLevel <= 17;
									if (flag8)
									{
										color = "F26A34";
									}
									else
									{
										color = "E4504D";
									}
								}
							}
						}
					}
				}
			}
		}
		return string.Concat(new string[]
		{
			"<color=#",
			color,
			">",
			str,
			"</color>"
		}).ColorReplace();
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00008F68 File Offset: 0x00007168
	public static int IndexOf<T>(this IReadOnlyList<T> list, T target)
	{
		int i = 0;
		foreach (T element in list)
		{
			bool flag = target.Equals(element);
			if (flag)
			{
				return i;
			}
			i++;
		}
		return -1;
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00008FD8 File Offset: 0x000071D8
	public static void Shuffle<T>(this IList<T> list, int count = 1)
	{
		bool flag = list == null;
		if (!flag)
		{
			for (int i = 0; i < count; i++)
			{
				for (int j = list.Count - 1; j > 0; j--)
				{
					int rand = Random.Range(0, j + 1);
					int index = j;
					int index2 = rand;
					T value = list[rand];
					T value2 = list[j];
					list[index] = value;
					list[index2] = value2;
				}
			}
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00009064 File Offset: 0x00007264
	public static T GetRandom<T>(this IList<T> list)
	{
		bool flag = list == null || list.Count <= 0;
		T result;
		if (flag)
		{
			result = default(T);
		}
		else
		{
			result = list[GameApp.RandomRange(0, list.Count)];
		}
		return result;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x000090AC File Offset: 0x000072AC
	public static int GetRandomIndex(this IReadOnlyList<int> weights)
	{
		int totalWeight = weights.Sum();
		int randomValue = GameApp.RandomRange(0, totalWeight);
		for (int i = 0; i < weights.Count; i++)
		{
			randomValue -= weights[i];
			bool flag = randomValue < 0;
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00009100 File Offset: 0x00007300
	public static bool CheckIndex(this ICollection collection, int index)
	{
		return collection != null && collection.Count.CheckIndex(index);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00009124 File Offset: 0x00007324
	public static bool CheckIndexReadOnly<T>(this IReadOnlyCollection<T> collection, int index)
	{
		return collection != null && collection.Count.CheckIndex(index);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00009148 File Offset: 0x00007348
	public static bool CheckIndex(this int count, int index)
	{
		return index >= 0 && index < count;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00009158 File Offset: 0x00007358
	public static int GetManhattanDistance(this Vector2Int vecSelf, Vector2Int vecTarget)
	{
		return Mathf.Abs(vecTarget.x - vecSelf.x) + Mathf.Abs(vecTarget.y - vecSelf.y);
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00009193 File Offset: 0x00007393
	public static void SetSize(this RectTransform rectTrans, Vector2 size)
	{
		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		rectTrans.sizeDelta = size;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x000091BA File Offset: 0x000073BA
	public static void SetWidth(this RectTransform rectTrans, float width)
	{
		rectTrans.sizeDelta = new Vector2(width, rectTrans.sizeDelta.y);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x000091D5 File Offset: 0x000073D5
	public static void SetHeight(this RectTransform rectTrans, float height)
	{
		rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000091F0 File Offset: 0x000073F0
	public static void ChangeLocalPositionY(this RectTransform rectTrans, float delta)
	{
		rectTrans.localPosition = new Vector3(rectTrans.localPosition.x, rectTrans.localPosition.y + delta, rectTrans.localPosition.z);
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00009222 File Offset: 0x00007422
	public static void ChangeAnchoredPositionY(this RectTransform rectTrans, float delta)
	{
		rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y + delta);
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000924C File Offset: 0x0000744C
	public static void SetPivot(this RectTransform rectTrans, Vector2 pivot)
	{
		Vector3 deltaPosition = rectTrans.pivot - pivot;
		deltaPosition.Scale(rectTrans.rect.size);
		deltaPosition.Scale(rectTrans.localScale);
		deltaPosition = rectTrans.rotation * deltaPosition;
		bool flag = float.IsNaN(deltaPosition.x) || float.IsNaN(deltaPosition.y) || float.IsNaN(deltaPosition.z) || float.IsInfinity(deltaPosition.x) || float.IsInfinity(deltaPosition.y) || float.IsInfinity(deltaPosition.z);
		if (flag)
		{
			rectTrans.pivot = pivot;
		}
		else
		{
			rectTrans.pivot = pivot;
			rectTrans.localPosition -= deltaPosition;
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000931C File Offset: 0x0000751C
	public static void SetAnchor(this RectTransform rectTrans, Vector2 min, Vector2 max)
	{
		Vector3 pos = rectTrans.localPosition;
		rectTrans.anchorMin = min;
		rectTrans.anchorMax = max;
		rectTrans.localPosition = pos;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000934C File Offset: 0x0000754C
	public static ValueTuple<Vector2, Vector2> TransformRectInfo(this RectTransform rectTrans, RectTransform targetTrans)
	{
		Vector2 rectMinWorldPos = rectTrans.TransformPoint(rectTrans.rect.min);
		Vector2 rectMaxWorldPos = rectTrans.TransformPoint(rectTrans.rect.max);
		Vector2 minPos = targetTrans.InverseTransformPoint(rectMinWorldPos);
		Vector2 maxPos = targetTrans.InverseTransformPoint(rectMaxWorldPos);
		return new ValueTuple<Vector2, Vector2>(minPos, maxPos);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x000093CC File Offset: 0x000075CC
	public static Rect RectTransToScreenPos(this RectTransform rt, Camera camera)
	{
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		Vector2 v0 = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
		Vector2 v = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
		Rect rect = new Rect(v0, v - v0);
		return rect;
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000941C File Offset: 0x0000761C
	public static int ParseIntWithDefaultValue(this string value, int returnValueWhenFailed)
	{
		int result;
		bool flag = int.TryParse(value, out result);
		int result2;
		if (flag)
		{
			result2 = result;
		}
		else
		{
			result2 = returnValueWhenFailed;
		}
		return result2;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00009440 File Offset: 0x00007640
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Behaviour
	{
		T component = gameObject.GetComponent<T>();
		bool flag = component != null;
		T result;
		if (flag)
		{
			result = component;
		}
		else
		{
			result = gameObject.AddComponent<T>();
		}
		return result;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00009474 File Offset: 0x00007674
	public static void ResetListener(this UnityEvent evt, Action action)
	{
		bool flag = action == null;
		if (flag)
		{
			throw new ArgumentNullException("action");
		}
		evt.RemoveAllListeners();
		evt.AddListener(delegate()
		{
			action();
		});
	}

	// Token: 0x06000147 RID: 327 RVA: 0x000094C4 File Offset: 0x000076C4
	public static void ResetListener<T>(this UnityEvent<T> evt, Action<T> action)
	{
		bool flag = action == null;
		if (flag)
		{
			throw new ArgumentNullException("action");
		}
		evt.RemoveAllListeners();
		evt.AddListener(delegate(T arg0)
		{
			action(arg0);
		});
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00009514 File Offset: 0x00007714
	public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
	{
		transform.localScale = Vector3.one;
		Vector3 lossyScale = transform.lossyScale;
		Vector3 targetScale = new Vector3(globalScale.x / lossyScale.x, globalScale.y / lossyScale.y, globalScale.z / lossyScale.z);
		bool flag = float.IsInfinity(targetScale.x) || float.IsNaN(targetScale.x) || float.IsInfinity(targetScale.y) || float.IsNaN(targetScale.y) || float.IsInfinity(targetScale.z) || float.IsNaN(targetScale.z);
		if (!flag)
		{
			transform.localScale = targetScale;
		}
	}

	// Token: 0x06000149 RID: 329 RVA: 0x000095C0 File Offset: 0x000077C0
	public static Vector3 SetX(this Vector3 v3, float x)
	{
		v3.x = x;
		return v3;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x000095DC File Offset: 0x000077DC
	public static Vector3 SetY(this Vector3 v3, float y)
	{
		v3.y = y;
		return v3;
	}

	// Token: 0x0600014B RID: 331 RVA: 0x000095F8 File Offset: 0x000077F8
	public static Vector3 SetZ(this Vector3 v3, float z)
	{
		v3.z = z;
		return v3;
	}

	// Token: 0x0600014C RID: 332 RVA: 0x00009614 File Offset: 0x00007814
	public static Vector2 SetX(this Vector2 v2, float x)
	{
		v2.x = x;
		return v2;
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00009630 File Offset: 0x00007830
	public static Vector2 SetY(this Vector2 v2, float y)
	{
		v2.y = y;
		return v2;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000964C File Offset: 0x0000784C
	public static Vector2 SetXY(this Vector2 v2, float x, float y)
	{
		v2.x = x;
		v2.y = y;
		return v2;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00009670 File Offset: 0x00007870
	public static Color SetAlpha(this Color color, float alpha)
	{
		color.a = alpha;
		return color;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000968C File Offset: 0x0000788C
	public static float GetEvtTime(this Spine.Animation anim, string evtName)
	{
		IEnumerable<EventTimeline> evts = (from a in anim.Timelines
		where a is EventTimeline
		select a).Cast<EventTimeline>();
		Func<Spine.Event, bool> <>9__1;
		foreach (EventTimeline evt in evts)
		{
			IEnumerable<Spine.Event> events = evt.Events;
			Func<Spine.Event, bool> predicate;
			if ((predicate = <>9__1) == null)
			{
				predicate = (<>9__1 = ((Spine.Event a) => a.Data.Name == evtName));
			}
			Spine.Event tar = events.FirstOrDefault(predicate);
			bool flag = tar != null;
			if (flag)
			{
				return tar.Time;
			}
		}
		return -1f;
	}

	// Token: 0x06000151 RID: 337 RVA: 0x00009760 File Offset: 0x00007960
	public static int ToInt(this Enum e)
	{
		return Convert.ToInt32(e);
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00009768 File Offset: 0x00007968
	public static sbyte ToSbyte(this Enum e)
	{
		return Convert.ToSByte(e);
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00009770 File Offset: 0x00007970
	public static string Repeat(this char ch, int times)
	{
		string result = string.Empty;
		for (int i = 0; i < times; i++)
		{
			result += ch.ToString();
		}
		return result;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x000097A8 File Offset: 0x000079A8
	public static string Repeat(this string str, int times)
	{
		string result = string.Empty;
		for (int i = 0; i < times; i++)
		{
			result += str;
		}
		return result;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x000097DC File Offset: 0x000079DC
	public static Color32 BrightnessDelta(this Color32 color, int delta)
	{
		color.r = (byte)Mathf.Clamp((int)color.r + delta, 0, 255);
		color.g = (byte)Mathf.Clamp((int)color.g + delta, 0, 255);
		color.b = (byte)Mathf.Clamp((int)color.b + delta, 0, 255);
		return color;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00009840 File Offset: 0x00007A40
	public static Color BrightnessDelta(this Color color, float delta)
	{
		color.r = Mathf.Clamp01(color.r + delta);
		color.g = Mathf.Clamp01(color.g + delta);
		color.b = Mathf.Clamp01(color.b + delta);
		return color;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00009890 File Offset: 0x00007A90
	public static bool ContentIsSame<T>(this IList<T> a, List<T> b)
	{
		int aCount = (a != null) ? a.Count : 0;
		int bCount = (b != null) ? b.Count : 0;
		bool flag = aCount == 0 && bCount == 0;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = aCount != bCount;
			if (flag2)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < a.Count; i++)
				{
					T t = a[i];
					bool flag3 = !t.Equals(b[i]);
					if (flag3)
					{
						return false;
					}
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000992C File Offset: 0x00007B2C
	public static bool ContentIsDifferent<T>(this IList<T> a, List<T> b)
	{
		return !a.ContentIsSame(b);
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00009938 File Offset: 0x00007B38
	public static bool ContainsWithBorder(this Rect rect, Vector2 point)
	{
		return (int)point.x >= (int)rect.xMin && (int)point.x <= (int)rect.xMax && (int)point.y >= (int)rect.yMin && (int)point.y <= (int)rect.yMax;
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00009994 File Offset: 0x00007B94
	internal static IEnumerator LayoutRebuildRoutine(this RectTransform self)
	{
		foreach (RectTransform rect in self.GetComponentsInChildren<RectTransform>().Reverse<RectTransform>())
		{
			bool flag = null == rect;
			if (flag)
			{
				yield break;
			}
			ContentSizeFitter fitter = rect.GetComponent<ContentSizeFitter>();
			bool flag2 = null != fitter;
			if (flag2)
			{
				fitter.SetLayoutHorizontal();
				fitter.SetLayoutVertical();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
			LayoutRebuilder.MarkLayoutForRebuild(rect);
			yield return new WaitForEndOfFrame();
			fitter = null;
			rect = null;
		}
		IEnumerator<RectTransform> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x0600015B RID: 347 RVA: 0x000099A3 File Offset: 0x00007BA3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string Tr(this LanguageKey languageKey)
	{
		return LocalStringManager.Get(languageKey);
	}

	// Token: 0x0600015C RID: 348 RVA: 0x000099AB File Offset: 0x00007BAB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string TrFormat(this LanguageKey languageKey, object arg0)
	{
		return LocalStringManager.GetFormat(languageKey, arg0);
	}

	// Token: 0x0600015D RID: 349 RVA: 0x000099B4 File Offset: 0x00007BB4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string TrFormat(this LanguageKey languageKey, object arg0, object arg1)
	{
		return LocalStringManager.GetFormat(languageKey, arg0, arg1);
	}

	// Token: 0x0600015E RID: 350 RVA: 0x000099BE File Offset: 0x00007BBE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string TrFormat(this LanguageKey languageKey, object arg0, object arg1, object arg2)
	{
		return LocalStringManager.GetFormat(languageKey, arg0, arg1, arg2);
	}

	// Token: 0x0600015F RID: 351 RVA: 0x000099C9 File Offset: 0x00007BC9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string TrFormat(this LanguageKey languageKey, params object[] args)
	{
		return LocalStringManager.GetFormat(languageKey, args);
	}

	// Token: 0x040000AF RID: 175
	public static readonly Regex InvalidFileNameCharRegex = new Regex("[><?？*|\\\"~\\\\!$%^&+{};:',.·`！￥…、【】：；‘’“”《》，。=]");

	// Token: 0x040000B0 RID: 176
	private static readonly Regex _colorReg = new Regex("<color=#(?<key>([A-Z|a-z][A-Z|a-z|0-9]+_[A-Z|a-z|0-9]+)|([a-z]+))>");

	// Token: 0x040000B1 RID: 177
	private static readonly Regex _colorReg2 = new Regex("<color=[^>]+>(.*?)<\\/color>");

	// Token: 0x040000B2 RID: 178
	private static readonly Regex _colorReg3 = new Regex("<color=[^>]+>|<\\/color>");

	// Token: 0x040000B3 RID: 179
	private static readonly Regex _richTextRegex = new Regex("<[^>]*>", RegexOptions.Compiled);

	// Token: 0x040000B4 RID: 180
	public const string HighlightColorStart = "<mark=#9973315a>";

	// Token: 0x040000B5 RID: 181
	public const string HighlightColorEnd = "</mark>";

	// Token: 0x040000B6 RID: 182
	public const string SelectedHighlightColorStart = "<mark=#ce5d205a>";
}
