using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Config;
using EventEditor;
using EventEditor.EventScript;
using GameData.Domains.TaiwuEvent;
using UnityEngine;

namespace EventScript
{
	// Token: 0x020006DA RID: 1754
	public class EventScriptCompilerException : Exception
	{
		// Token: 0x06005376 RID: 21366 RVA: 0x0026B94C File Offset: 0x00269B4C
		private EventScriptCompilerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06005377 RID: 21367 RVA: 0x0026B958 File Offset: 0x00269B58
		public static EventScriptCompilerException CreateAtArg(short predefinedLogId, EventArgumentItem argCfg, EventArgumentEditorData argData, int argIndex, Exception innerException = null)
		{
			EventScriptCompilerException eventScriptCompilerException = innerException as EventScriptCompilerException;
			bool flag = eventScriptCompilerException == null;
			if (flag)
			{
				if (!true)
				{
				}
				string text;
				if (predefinedLogId != 26)
				{
					if (predefinedLogId != 28)
					{
						text = PredefinedLog.Instance[predefinedLogId].Info;
					}
					else
					{
						text = string.Format(PredefinedLog.Instance[predefinedLogId].Info, argCfg.CustomEnumText.IndexOf(argData.Value));
					}
				}
				else
				{
					text = string.Format(PredefinedLog.Instance[predefinedLogId].Info, argCfg.ConfigTable);
				}
				if (!true)
				{
				}
				string msg = text;
				eventScriptCompilerException = new EventScriptCompilerException(msg, innerException);
			}
			eventScriptCompilerException._argIndex = new int?(argIndex);
			eventScriptCompilerException._argTypeName = argCfg.Name;
			eventScriptCompilerException._argValue = argData.Value;
			eventScriptCompilerException._argIsExpression = argData.IsExpression;
			eventScriptCompilerException._argValueString = argData.ToString();
			return eventScriptCompilerException;
		}

		// Token: 0x06005378 RID: 21368 RVA: 0x0026BA44 File Offset: 0x00269C44
		public static EventScriptCompilerException CreateAtRow(int row, EventInstructionEditorData editorData, Exception innerException)
		{
			EventScriptCompilerException eventScriptCompilerException = innerException as EventScriptCompilerException;
			bool flag = eventScriptCompilerException == null;
			if (flag)
			{
				eventScriptCompilerException = new EventScriptCompilerException(innerException.Message, innerException);
			}
			eventScriptCompilerException._rowId = new int?(row);
			eventScriptCompilerException._funcName = editorData.FunctionName;
			return eventScriptCompilerException;
		}

		// Token: 0x06005379 RID: 21369 RVA: 0x0026BA90 File Offset: 0x00269C90
		public static EventScriptCompilerException CreateAtScript(EventScriptId scriptId, string filePath, Exception innerException)
		{
			EventScriptCompilerException eventScriptCompilerException = innerException as EventScriptCompilerException;
			bool flag = eventScriptCompilerException != null;
			EventScriptCompilerException result;
			if (flag)
			{
				eventScriptCompilerException._scriptId = new EventScriptId?(scriptId);
				eventScriptCompilerException._filePath = filePath;
				result = eventScriptCompilerException;
			}
			else
			{
				result = new EventScriptCompilerException(innerException.Message, innerException)
				{
					_scriptId = new EventScriptId?(scriptId),
					_filePath = filePath
				};
			}
			return result;
		}

		// Token: 0x0600537A RID: 21370 RVA: 0x0026BAE8 File Offset: 0x00269CE8
		public override string ToString()
		{
			bool flag = this._toString != null;
			string toString;
			if (flag)
			{
				toString = this._toString;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.Message);
				bool flag2 = this._argIndex != null;
				if (flag2)
				{
					stringBuilder.AppendFormat("\tat arg {0} {1}: {2}\n", this._argIndex.ToString(), this._argTypeName, this._argValueString);
				}
				bool flag3 = this._rowId != null;
				if (flag3)
				{
					stringBuilder.AppendFormat("\tat row {0}: {1}\n", this._rowId.ToString(), this._funcName);
				}
				bool flag4 = this._scriptId != null;
				if (flag4)
				{
					stringBuilder.AppendFormat("\tat script {0}\n", this._scriptId.ToString());
				}
				bool flag5 = !string.IsNullOrEmpty(this._filePath);
				if (flag5)
				{
					stringBuilder.AppendFormat("\tat file {0} \n", this._filePath);
				}
				bool flag6 = base.InnerException != null;
				if (flag6)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(base.InnerException.StackTrace);
				}
				this._toString = stringBuilder.ToString();
				toString = this._toString;
			}
			return toString;
		}

		// Token: 0x0600537B RID: 21371 RVA: 0x0026BC24 File Offset: 0x00269E24
		public string GetArg()
		{
			bool flag = this._argIndex == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = string.Concat(new string[]
				{
					"参数名".SetColor(Color.green),
					"：",
					this._argTypeName,
					"\n",
					"参数值".SetColor(Color.green),
					"：",
					this._argValueString
				});
			}
			return result;
		}

		// Token: 0x0600537C RID: 21372 RVA: 0x0026BCAC File Offset: 0x00269EAC
		public bool HasArgInfo()
		{
			return this._argIndex != null;
		}

		// Token: 0x0600537D RID: 21373 RVA: 0x0026BCCC File Offset: 0x00269ECC
		public string GetArgTypeName()
		{
			return this._argTypeName;
		}

		// Token: 0x0600537E RID: 21374 RVA: 0x0026BCE4 File Offset: 0x00269EE4
		public string GetArgTypeValue()
		{
			return this._argValue;
		}

		// Token: 0x0600537F RID: 21375 RVA: 0x0026BCFC File Offset: 0x00269EFC
		public bool IsArgExpression()
		{
			return this._argIsExpression;
		}

		// Token: 0x06005380 RID: 21376 RVA: 0x0026BD14 File Offset: 0x00269F14
		public int GetRowId()
		{
			bool flag = this._rowId != null;
			int result;
			if (flag)
			{
				result = this._rowId.Value;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x06005381 RID: 21377 RVA: 0x0026BD48 File Offset: 0x00269F48
		public EventScriptId? GetScriptId()
		{
			return this._scriptId;
		}

		// Token: 0x06005382 RID: 21378 RVA: 0x0026BD60 File Offset: 0x00269F60
		public string GetLocation()
		{
			bool flag = this._scriptId == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				EventEditorModel eventModel = SingletonObject.getInstance<EventEditorModel>();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Format("{0}：{1}", "脚本类型".SetColor(Color.yellow), (this._scriptId != null) ? new sbyte?(this._scriptId.GetValueOrDefault().Type) : null));
				sbyte? b = (this._scriptId != null) ? new sbyte?(this._scriptId.GetValueOrDefault().Type) : null;
				int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				int num2 = 0;
				bool flag2 = num.GetValueOrDefault() == num2 & num != null;
				if (flag2)
				{
					result = stringBuilder.ToString();
				}
				else
				{
					Debug.Assert(this._scriptId != null);
					EventScriptRef eventScriptRef = this._scriptId.Value.EventScriptRef;
					string guid = eventScriptRef.Guid.ToString();
					EventEditorData targetEvent = eventModel.GetEvent(guid);
					bool hasSubGuid = eventScriptRef.HasSubGuid;
					if (hasSubGuid)
					{
						string subGuid = eventScriptRef.SubGuid.ToString();
						Dictionary<int, EventEditorData.Option> optionTable = targetEvent.Options;
						int order = -1;
						foreach (int key in optionTable.Keys)
						{
							EventEditorData.Option option = optionTable[key];
							string optionGuid = option.Guid;
							bool flag3 = optionGuid == subGuid;
							if (flag3)
							{
								order = key;
								break;
							}
						}
						stringBuilder.AppendLine(string.Format("{0}: {1} {2}", "选项".SetColor(Color.yellow), order, subGuid.SetColor(Color.cyan)));
					}
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"事件".SetColor(Color.yellow),
						":",
						targetEvent.EventName,
						" ",
						guid.SetColor(Color.cyan)
					}));
					EventGroupData eventGroup = eventModel.GetGroupDataByEventGuid(guid);
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"事件组".SetColor(Color.yellow),
						":",
						eventGroup.Name,
						" ",
						eventGroup.Key.SetColor(Color.cyan)
					}));
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		// Token: 0x04003876 RID: 14454
		private int? _argIndex;

		// Token: 0x04003877 RID: 14455
		private string _argTypeName;

		// Token: 0x04003878 RID: 14456
		private string _argValueString;

		// Token: 0x04003879 RID: 14457
		private string _argValue;

		// Token: 0x0400387A RID: 14458
		private bool _argIsExpression;

		// Token: 0x0400387B RID: 14459
		private int? _rowId;

		// Token: 0x0400387C RID: 14460
		private string _funcName;

		// Token: 0x0400387D RID: 14461
		private EventScriptId? _scriptId;

		// Token: 0x0400387E RID: 14462
		private string _filePath;

		// Token: 0x0400387F RID: 14463
		private string _toString;
	}
}
