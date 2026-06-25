using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.World.Notification;
using GameData.Utilities;
using UnityEngine;

namespace UILogic.DisplayDataStructure
{
	// Token: 0x020006B3 RID: 1715
	public class NotificationItem
	{
		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x0600501E RID: 20510 RVA: 0x002574B0 File Offset: 0x002556B0
		private bool IsSimpleOnly
		{
			get
			{
				return this._displayDesc.IsNullOrEmpty();
			}
		}

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x0600501F RID: 20511 RVA: 0x002574BD File Offset: 0x002556BD
		public bool HasDetail
		{
			get
			{
				return this._isMerged || !this.IsSimpleOnly;
			}
		}

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x06005020 RID: 20512 RVA: 0x002574D3 File Offset: 0x002556D3
		public short RecordType
		{
			get
			{
				RenderInfo renderInfo = this.RenderInfoList.FirstOrDefault<RenderInfo>();
				return (renderInfo != null) ? renderInfo.RecordType : -1;
			}
		}

		// Token: 0x06005021 RID: 20513 RVA: 0x002574EC File Offset: 0x002556EC
		public NotificationItem(int date, RenderInfo info, Func<short, List<sbyte>> funcGetMergeableParameters)
		{
			this.Date = date;
			this.RenderInfoList = new List<RenderInfo>
			{
				info
			};
			this.GetMergeableParameters = funcGetMergeableParameters;
			this.MergeType = 3;
			this._dirty = true;
		}

		// Token: 0x06005022 RID: 20514 RVA: 0x00257540 File Offset: 0x00255740
		public override string ToString()
		{
			string result;
			try
			{
				result = this.GetDesc();
			}
			catch (Exception)
			{
				Debug.LogWarning(this.RenderInfoList[0].RecordType.ToString() + ": " + this.RenderInfoList[0].Text);
				result = this.RenderInfoList[0].Text;
			}
			return result;
		}

		// Token: 0x06005023 RID: 20515 RVA: 0x002575B8 File Offset: 0x002557B8
		private object[] GetFillParams(RenderInfo info)
		{
			return info.ParseArguments(this.RenderedArgumentCollection);
		}

		// Token: 0x06005024 RID: 20516 RVA: 0x002575C8 File Offset: 0x002557C8
		public string GetDesc()
		{
			bool flag = this.RenderedArgumentCollection == null || this.RenderInfoList == null || this.RenderInfoList.Count <= 0;
			string result3;
			if (flag)
			{
				result3 = string.Empty;
			}
			else
			{
				bool flag2 = !this._dirty;
				if (flag2)
				{
					result3 = this._displayDesc;
				}
				else
				{
					RenderInfo info = this.RenderInfoList[0];
					bool flag3 = info.Text.IsNullOrEmpty();
					if (flag3)
					{
						result3 = this.GetSimpleDesc();
					}
					else
					{
						object[] fillParams = this.GetFillParams(info);
						List<sbyte> mergeableParameters = this.GetMergeableParameters(info.RecordType);
						bool flag4 = mergeableParameters == null || mergeableParameters.Count <= 0;
						if (flag4)
						{
							this._displayDesc = info.Text.GetFormat(fillParams).ColorReplace();
						}
						else
						{
							bool flag5 = this.RenderInfoList.Count == 1;
							if (flag5)
							{
								this._displayDesc = info.Text.GetFormat(fillParams).ColorReplace();
							}
							else
							{
								switch (this.MergeType)
								{
								case 0:
								{
									List<string> mergedResultList = new List<string>();
									bool flag6 = mergeableParameters.Count == 1;
									if (flag6)
									{
										sbyte pos = mergeableParameters.Single<sbyte>();
										foreach (RenderInfo t in this.RenderInfoList)
										{
											ValueTuple<sbyte, int> args = t.Arguments[(int)pos];
											string result = this.RenderedArgumentCollection.Get(args.Item1, args.Item2);
											mergedResultList.Add(result);
										}
										fillParams[(int)pos] = string.Join("、", mergedResultList);
										this._displayDesc = info.Text.GetFormat(fillParams).ColorReplace();
									}
									else
									{
										bool flag7 = string.IsNullOrEmpty(this._mergeStringBase);
										if (flag7)
										{
											this.SetMergeStringBase(mergeableParameters);
										}
										foreach (RenderInfo tempInfo in this.RenderInfoList)
										{
											for (int i = 0; i < fillParams.Length; i++)
											{
												ValueTuple<sbyte, int> args2 = tempInfo.Arguments[i];
												fillParams[i] = this.RenderedArgumentCollection.Get(args2.Item1, args2.Item2);
											}
											string element = this._mergeStringBase.GetFormat(fillParams);
											bool flag8 = !mergedResultList.Contains(element);
											if (flag8)
											{
												mergedResultList.Add(element);
											}
										}
										this._displayDesc = this._formatStringBase.Replace("<MergedStringTag>", string.Join("、", mergedResultList));
										this._displayDesc = this._displayDesc.GetFormat(fillParams).ColorReplace();
									}
									break;
								}
								case 1:
								{
									object[] newFillParams = new object[fillParams.Length + 1];
									fillParams.CopyTo(newFillParams, 0);
									object[] array = newFillParams;
									array[array.Length - 1] = this.RenderInfoList.Count;
									this._displayDesc = this.MergeDesc.GetFormat(newFillParams).ColorReplace();
									break;
								}
								case 2:
								{
									object[] newFillParams = new object[fillParams.Length - mergeableParameters.Count];
									int indexNew = 0;
									sbyte indexOld = 0;
									while ((int)indexOld < fillParams.Length)
									{
										bool flag9 = !mergeableParameters.Contains(indexOld);
										if (flag9)
										{
											newFillParams[indexNew++] = fillParams[(int)indexOld];
										}
										indexOld += 1;
									}
									this._displayDesc = this.MergeDesc.GetFormat(newFillParams).ColorReplace();
									break;
								}
								case 3:
								{
									List<string> mergedResultList2 = new List<string>();
									sbyte pos2 = mergeableParameters.Single<sbyte>();
									foreach (RenderInfo t2 in this.RenderInfoList)
									{
										ValueTuple<sbyte, int> args3 = t2.Arguments[(int)pos2];
										string result2 = this.RenderedArgumentCollection.Get(args3.Item1, args3.Item2);
										bool flag10 = !mergedResultList2.Contains(result2);
										if (flag10)
										{
											mergedResultList2.Add(result2);
										}
									}
									fillParams[(int)pos2] = string.Join("、", mergedResultList2);
									this._displayDesc = info.Text.GetFormat(fillParams).ColorReplace();
									break;
								}
								}
							}
						}
						this._dirty = false;
						result3 = this._displayDesc;
					}
				}
			}
			return result3;
		}

		// Token: 0x06005025 RID: 20517 RVA: 0x00257A60 File Offset: 0x00255C60
		public string GetSimpleDesc()
		{
			bool flag = !this._displaySimpleDesc.IsNullOrEmpty();
			string result;
			if (flag)
			{
				result = this._displaySimpleDesc;
			}
			else
			{
				bool flag2 = this.RenderedArgumentCollection == null || this.RenderInfoList == null || this.RenderInfoList.Count <= 0;
				if (flag2)
				{
					result = string.Empty;
				}
				else
				{
					InstantNotificationRenderInfo info = this.RenderInfoList[0] as InstantNotificationRenderInfo;
					bool flag3 = info == null;
					if (flag3)
					{
						result = string.Empty;
					}
					else
					{
						bool flag4 = info.SimpleText.IsNullOrEmpty();
						if (flag4)
						{
							this._displaySimpleDesc = string.Empty;
						}
						else
						{
							object[] fillParams = this.GetFillParams(info);
							bool flag5 = this.RenderInfoList.Count > 1;
							if (flag5)
							{
								List<sbyte> mergeableParameters = this.GetMergeableParameters(info.RecordType);
								bool flag6 = mergeableParameters != null && mergeableParameters.Count > 0;
								if (flag6)
								{
									int index = this.RenderInfoList[0].Arguments.FindIndex(([TupleElementNames(new string[]
									{
										"paramType",
										"index"
									})] ValueTuple<sbyte, int> tuple) => tuple.Item1 == 0);
									bool flag7 = index > -1 && mergeableParameters.Contains((sbyte)index);
									if (flag7)
									{
										string name = fillParams[index] as string;
										bool isDiff = this.RenderInfoList.Exists(delegate(RenderInfo r)
										{
											InstantNotificationRenderInfo info2 = r as InstantNotificationRenderInfo;
											object[] fillParams2 = this.GetFillParams(info2);
											string str = fillParams2[index] as string;
											return name != str;
										});
										bool flag8 = isDiff;
										if (flag8)
										{
											name += LocalStringManager.Get(LanguageKey.UI_MouseTip_SecretInforBroadcast_AndMoreCharacters);
										}
										fillParams[index] = name;
									}
								}
							}
							this._displaySimpleDesc = info.SimpleText.GetFormat(fillParams).ColorReplace();
						}
						result = this._displaySimpleDesc;
					}
				}
			}
			return result;
		}

		// Token: 0x06005026 RID: 20518 RVA: 0x00257C74 File Offset: 0x00255E74
		public bool TryMerge(int date, RenderInfo info)
		{
			RenderInfo srcRenderInfo = this.RenderInfoList[0];
			bool flag = date != this.Date || srcRenderInfo.RecordType != info.RecordType;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<sbyte> mergeableParameters = this.GetMergeableParameters(info.RecordType);
				bool flag2 = mergeableParameters == null || mergeableParameters.Count <= 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					sbyte i = 0;
					while ((int)i < srcRenderInfo.Arguments.Count)
					{
						bool flag3 = !mergeableParameters.Contains(i);
						if (flag3)
						{
							ValueTuple<sbyte, int> srcArgumentInfo = srcRenderInfo.Arguments[(int)i];
							ValueTuple<sbyte, int> curArgumentInfo = info.Arguments[(int)i];
							bool flag4 = srcArgumentInfo.Item1 != curArgumentInfo.Item1;
							if (flag4)
							{
								return false;
							}
							bool flag5 = this.RenderedArgumentCollection.Get(srcArgumentInfo.Item1, srcArgumentInfo.Item2) != this.RenderedArgumentCollection.Get(curArgumentInfo.Item1, curArgumentInfo.Item2);
							if (flag5)
							{
								return false;
							}
						}
						i += 1;
					}
					bool flag6 = this.MergeableParameterValues != null;
					if (flag6)
					{
						foreach (KeyValuePair<int, string> keyValuePair in this.MergeableParameterValues)
						{
							int num;
							string text;
							keyValuePair.Deconstruct(out num, out text);
							int index = num;
							string value = text;
							bool flag7 = this.RenderedArgumentCollection.Get(info.Arguments[index].Item1, info.Arguments[index].Item2) != value;
							if (flag7)
							{
								return false;
							}
						}
					}
					this.RenderInfoList.Add(info);
					this._dirty = true;
					this._isMerged = true;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06005027 RID: 20519 RVA: 0x00257E74 File Offset: 0x00256074
		public void SetDirty()
		{
			this._dirty = true;
		}

		// Token: 0x06005028 RID: 20520 RVA: 0x00257E80 File Offset: 0x00256080
		private void SetMergeStringBase(List<sbyte> mergeableParameters)
		{
			sbyte minIndex = sbyte.MaxValue;
			sbyte maxIndex = sbyte.MinValue;
			foreach (sbyte index in mergeableParameters)
			{
				minIndex = (sbyte)Mathf.Min((int)minIndex, (int)index);
				maxIndex = (sbyte)Mathf.Max((int)minIndex, (int)index);
			}
			string matchMin = string.Format("{{{0}}}", minIndex);
			string matchMax = string.Format("{{{0}}}", maxIndex);
			string srcText = this.RenderInfoList[0].Text;
			int startIndex = srcText.IndexOf(matchMin, StringComparison.Ordinal);
			int endIndex = srcText.IndexOf(matchMax, StringComparison.Ordinal);
			bool flag = startIndex > endIndex;
			if (flag)
			{
				int num = endIndex;
				int num2 = startIndex;
				startIndex = num;
				endIndex = num2;
			}
			this._mergeStringBase = srcText.Substring(startIndex, endIndex + 3 - startIndex);
			Regex fillIndexRegex = new Regex("{(?<indexString>[\\d]+)}");
			MatchCollection matchCollection = fillIndexRegex.Matches(this._mergeStringBase);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				Match match = matchCollection[i];
				sbyte index2;
				bool flag2 = sbyte.TryParse(match.Groups["indexString"].Value, out index2) && !mergeableParameters.Contains(index2);
				if (flag2)
				{
					throw new Exception(this._mergeStringBase + " contains invalid fill argument index which is not included in MergeableParameters");
				}
			}
			sbyte pairCode = NotificationItem.<SetMergeStringBase>g__CheckTagInPair|31_0(this._mergeStringBase);
			bool flag3 = pairCode != 0;
			if (flag3)
			{
				sbyte needTagCount = (sbyte)Mathf.Abs((int)pairCode);
				bool flag4 = pairCode < 0;
				if (flag4)
				{
					while (startIndex >= 0)
					{
						bool flag5 = srcText[startIndex] == '<';
						if (flag5)
						{
							bool flag6 = srcText[startIndex + 1] == '/';
							if (flag6)
							{
								needTagCount += 1;
							}
							else
							{
								needTagCount -= 1;
							}
						}
						bool flag7 = needTagCount == 0;
						if (flag7)
						{
							break;
						}
						startIndex--;
					}
				}
				else
				{
					bool flag8 = pairCode > 0;
					if (flag8)
					{
						while (endIndex < srcText.Length)
						{
							bool flag9 = srcText[endIndex] == '<';
							if (flag9)
							{
								bool flag10 = endIndex + 1 >= srcText.Length;
								if (flag10)
								{
									throw new Exception("invalid tag in " + srcText);
								}
								bool flag11 = srcText[endIndex + 1] == '/';
								if (flag11)
								{
									needTagCount -= 1;
								}
								else
								{
									needTagCount += 1;
								}
							}
							bool flag12 = srcText[endIndex] == '>' && needTagCount == 0;
							if (flag12)
							{
								break;
							}
							endIndex++;
						}
					}
				}
				this._mergeStringBase = srcText.Substring(startIndex, endIndex + 1 - startIndex);
			}
			pairCode = NotificationItem.<SetMergeStringBase>g__CheckTagInPair|31_0(this._mergeStringBase);
			bool flag13 = pairCode != 0;
			if (flag13)
			{
				throw new Exception(srcText + " get replace tag pair failed!");
			}
			this._formatStringBase = srcText.Replace(this._mergeStringBase, "<MergedStringTag>");
		}

		// Token: 0x06005029 RID: 20521 RVA: 0x00258194 File Offset: 0x00256394
		[CompilerGenerated]
		internal static sbyte <SetMergeStringBase>g__CheckTagInPair|31_0(string srcString)
		{
			sbyte code = 0;
			for (int i = 0; i < srcString.Length; i++)
			{
				bool flag = srcString[i] == '<';
				if (flag)
				{
					bool flag2 = i + 1 < srcString.Length;
					if (flag2)
					{
						bool flag3 = srcString[i + 1] == '/';
						if (flag3)
						{
							code -= 1;
						}
						else
						{
							code += 1;
						}
					}
				}
			}
			return code;
		}

		// Token: 0x0400373F RID: 14143
		public int Date;

		// Token: 0x04003740 RID: 14144
		public RenderedArgumentCollection RenderedArgumentCollection;

		// Token: 0x04003741 RID: 14145
		public List<RenderInfo> RenderInfoList;

		// Token: 0x04003742 RID: 14146
		public List<int> CharIds = null;

		// Token: 0x04003743 RID: 14147
		[TupleElementNames(new string[]
		{
			"itemType",
			"itemTemplateId"
		})]
		public List<ValueTuple<sbyte, short>> Items = null;

		// Token: 0x04003744 RID: 14148
		public Func<short, List<sbyte>> GetMergeableParameters;

		// Token: 0x04003745 RID: 14149
		public bool ReadState;

		// Token: 0x04003746 RID: 14150
		public bool EnterClick;

		// Token: 0x04003747 RID: 14151
		public Dictionary<int, string> MergeableParameterValues;

		// Token: 0x04003748 RID: 14152
		public string MergeDesc;

		// Token: 0x04003749 RID: 14153
		public sbyte MergeType;

		// Token: 0x0400374A RID: 14154
		private string _mergeStringBase;

		// Token: 0x0400374B RID: 14155
		private string _formatStringBase;

		// Token: 0x0400374C RID: 14156
		private const string MergedStringTag = "<MergedStringTag>";

		// Token: 0x0400374D RID: 14157
		private bool _dirty;

		// Token: 0x0400374E RID: 14158
		private string _displayDesc;

		// Token: 0x0400374F RID: 14159
		private string _displaySimpleDesc;

		// Token: 0x04003750 RID: 14160
		private bool _isMerged;
	}
}
