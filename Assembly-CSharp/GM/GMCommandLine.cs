using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GM
{
	// Token: 0x02000605 RID: 1541
	public class GMCommandLine : Refers
	{
		// Token: 0x06004893 RID: 18579 RVA: 0x0021F53C File Offset: 0x0021D73C
		public void OnGmWindowAwake(UI_GMWindow gmWindow)
		{
			this._gmWindow = gmWindow;
			this._inputField = base.CGet<TMP_InputField>("InputField");
			this._gmCommandCompletionItem = base.CGet<GameObject>("GmCommandCompletionItem");
			this._possibleCommands = base.CGet<RectTransform>("PossibleCommands");
			this._inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
			this._inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnValueChanged));
		}

		// Token: 0x06004894 RID: 18580 RVA: 0x0021F5C0 File Offset: 0x0021D7C0
		public void OnGmWindowUpdate()
		{
			bool isFocused = this._inputField.isFocused;
			if (isFocused)
			{
				this.HandleInput();
			}
		}

		// Token: 0x06004895 RID: 18581 RVA: 0x0021F5E8 File Offset: 0x0021D7E8
		public void Prepare(List<MemberInfo>[] pages)
		{
			this.SetInputFieldText("");
			foreach (List<MemberInfo> page in pages)
			{
				foreach (MemberInfo member in page)
				{
					string consoleName = GMCommandLine.GenerateGmCommandName(member);
					bool flag = consoleName == null;
					if (!flag)
					{
						GMFuncAttribute info = member.GetCustomAttribute<GMFuncAttribute>();
						GMCommandLine.CommandLineFunc func = new GMCommandLine.CommandLineFunc
						{
							Method = (member as MethodInfo),
							RunMode = info.RunMode
						};
						this._funcs.Add(consoleName, func);
					}
				}
			}
		}

		// Token: 0x06004896 RID: 18582 RVA: 0x0021F6B4 File Offset: 0x0021D8B4
		private static string GenerateGmCommandName(MemberInfo member)
		{
			GMFuncAttribute gmAttribute = member.GetCustomAttribute<GMFuncAttribute>();
			bool flag = gmAttribute == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string consoleName = gmAttribute.ConsoleName;
				bool flag2 = consoleName == null;
				if (flag2)
				{
					consoleName = GMCommandLine.GenerateDefaultConsoleName(member.Name);
				}
				result = consoleName;
			}
			return result;
		}

		// Token: 0x06004897 RID: 18583 RVA: 0x0021F6FC File Offset: 0x0021D8FC
		private static string GenerateDefaultConsoleName(string memberName)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in memberName)
			{
				bool flag = char.IsUpper(c);
				if (flag)
				{
					bool flag2 = sb.Length > 0;
					if (flag2)
					{
						sb.Append('_');
					}
					sb.Append(char.ToLower(c));
				}
				else
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		// Token: 0x06004898 RID: 18584 RVA: 0x0021F77C File Offset: 0x0021D97C
		public static string GenerateGmHelpString(MethodInfo methodInfo)
		{
			string commandName = GMCommandLine.GenerateGmCommandName(methodInfo);
			return GMCommandLine.GenerateConsoleGmHelp(commandName, methodInfo);
		}

		// Token: 0x06004899 RID: 18585 RVA: 0x0021F7A0 File Offset: 0x0021D9A0
		private void SendNewConsoleGm()
		{
			string commandStr = this._inputField.text.Trim();
			bool flag = commandStr.IsNullOrEmpty();
			if (!flag)
			{
				this.SetInputFieldText(commandStr);
				string[] command = commandStr.Split(' ', StringSplitOptions.None);
				bool flag2 = command.Length == 0;
				if (!flag2)
				{
					string commandName = command[0];
					string[] commandParams = command.Skip(1).ToArray<string>();
					GMCommandLine.CommandLineFunc func;
					bool flag3 = !this._funcs.TryGetValue(commandName, out func);
					if (flag3)
					{
						this._gmWindow.Log(("Command " + commandName + " not found.").SetColor(Color.red));
					}
					else
					{
						List<object> realParams = new List<object>();
						ParameterInfo[] paramInfo = func.Method.GetParameters();
						bool flag4 = !GMCommandLine.CheckParamNumber(commandParams, paramInfo);
						if (flag4)
						{
							string helpString = GMCommandLine.GenerateConsoleGmHelp(commandName, func.Method);
							this._gmWindow.Log("Usage: \n" + helpString);
						}
						else
						{
							int i = 0;
							while (i < paramInfo.Length)
							{
								bool flag5 = i < commandParams.Length;
								if (flag5)
								{
									try
									{
										object specialValue;
										bool flag6 = this.TryParseConsoleSpecialVariables(commandParams[i], out specialValue);
										if (flag6)
										{
											bool flag7 = paramInfo[i].ParameterType != specialValue.GetType();
											if (flag7)
											{
												throw new Exception(string.Format("Special value type not match: {0} != {1}", paramInfo[i].ParameterType, specialValue.GetType()));
											}
											realParams.Add(specialValue);
										}
										else
										{
											object arg = this._gmWindow.ParseArgValue(paramInfo[i].ParameterType, commandParams[i], true);
											realParams.Add(arg);
										}
									}
									catch (Exception e)
									{
										this._gmWindow.Log(("Error: " + e.Message).SetColor(Color.red));
										string helpString2 = GMCommandLine.GenerateConsoleGmHelp(commandName, func.Method);
										this._gmWindow.Log("Usage: \n" + helpString2);
										return;
									}
								}
								else
								{
									bool hasDefaultValue = paramInfo[i].HasDefaultValue;
									if (hasDefaultValue)
									{
										realParams.Add(paramInfo[i].DefaultValue);
									}
									else
									{
										realParams.Add(null);
									}
								}
								IL_21B:
								i++;
								continue;
								goto IL_21B;
							}
							this._gmWindow.Log(("$ " + commandStr).SetColor(Color.yellow));
							GmRunMode runMode = func.RunMode;
							GmRunMode gmRunMode = runMode;
							if (gmRunMode != GmRunMode.Default)
							{
								if (gmRunMode == GmRunMode.NoTry)
								{
									func.Method.Invoke(null, realParams.ToArray());
								}
							}
							else
							{
								try
								{
									func.Method.Invoke(null, realParams.ToArray());
								}
								catch (Exception e2)
								{
									this._gmWindow.Log(("Error: " + e2.Message).SetColor(Color.red));
								}
							}
							this._commandHistory.Add(commandStr);
							bool flag8 = this._commandHistory.Count > 32;
							if (flag8)
							{
								this._commandHistory.RemoveAt(0);
							}
							this._commandHistoryIndex = this._commandHistory.Count;
							this.SetInputFieldText("");
						}
					}
				}
			}
		}

		// Token: 0x0600489A RID: 18586 RVA: 0x0021FAE8 File Offset: 0x0021DCE8
		private static bool CheckParamNumber(string[] commandParams, ParameterInfo[] paramInfo)
		{
			int necessaryParamCount = paramInfo.Count(delegate(ParameterInfo x)
			{
				bool isOptional = x.IsOptional;
				bool result;
				if (isOptional)
				{
					result = false;
				}
				else
				{
					bool isNullable = x.ParameterType.IsGenericType && x.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);
					result = !isNullable;
				}
				return result;
			});
			return commandParams.Length >= necessaryParamCount;
		}

		// Token: 0x0600489B RID: 18587 RVA: 0x0021FB2C File Offset: 0x0021DD2C
		private void OnEndEdit(string text)
		{
			bool keyDown = Input.GetKeyDown(KeyCode.Return);
			if (keyDown)
			{
				this.SendNewConsoleGm();
			}
		}

		// Token: 0x0600489C RID: 18588 RVA: 0x0021FB50 File Offset: 0x0021DD50
		private void OnValueChanged(string text)
		{
			bool flag = text.Trim().IsNullOrEmpty();
			if (flag)
			{
				this.HideAllCompletions();
			}
			else
			{
				List<string> completion = this.QueryCommandCompletion(text.Trim());
				this.RefershCompletions(completion);
			}
		}

		// Token: 0x0600489D RID: 18589 RVA: 0x0021FB8C File Offset: 0x0021DD8C
		private void PrepareCompletionItem(int count)
		{
			int currentCount = this._possibleCommands.childCount;
			bool flag = currentCount < count;
			if (flag)
			{
				for (int i = currentCount; i < count; i++)
				{
					GameObject item = Object.Instantiate<GameObject>(this._gmCommandCompletionItem, this._possibleCommands);
					item.SetActive(true);
				}
			}
			else
			{
				for (int j = count; j < currentCount; j++)
				{
					this._possibleCommands.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600489E RID: 18590 RVA: 0x0021FC14 File Offset: 0x0021DE14
		private void RefershCompletions(List<string> completion)
		{
			this.PrepareCompletionItem(completion.Count);
			for (int i = 0; i < completion.Count; i++)
			{
				Transform child = this._possibleCommands.GetChild(i);
				child.gameObject.SetActive(true);
				Refers refers = child.GetComponent<Refers>();
				TextMeshProUGUI commandContent = refers.CGet<TextMeshProUGUI>("CommandContent");
				commandContent.text = completion[i];
				int ii = i;
				refers.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
				{
					this.SetInputFieldText(completion[ii]);
				});
			}
		}

		// Token: 0x0600489F RID: 18591 RVA: 0x0021FCE0 File Offset: 0x0021DEE0
		private void SetInputFieldText(string s)
		{
			s = s.Replace("`", "");
			this._inputField.text = s;
			this._inputField.caretPosition = this._inputField.text.Length;
			this.FocusInputField();
		}

		// Token: 0x060048A0 RID: 18592 RVA: 0x0021FD30 File Offset: 0x0021DF30
		private void HideAllCompletions()
		{
			for (int i = 0; i < this._possibleCommands.childCount; i++)
			{
				this._possibleCommands.GetChild(i).gameObject.SetActive(false);
			}
		}

		// Token: 0x060048A1 RID: 18593 RVA: 0x0021FD72 File Offset: 0x0021DF72
		private void ResetCommandHistoryIndex()
		{
			this._commandHistoryIndex = this._commandHistory.Count;
		}

		// Token: 0x060048A2 RID: 18594 RVA: 0x0021FD88 File Offset: 0x0021DF88
		private static string GenerateConsoleGmHelp(string commandName, MethodInfo methodInfo)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.Append("$ ");
			sb.Append(commandName);
			foreach (ParameterInfo param in methodInfo.GetParameters())
			{
				sb.Append(" ");
				sb.Append(param.Name);
				sb.Append("(");
				sb.Append(param.ParameterType.Name);
				sb.Append(")");
			}
			string result = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return result;
		}

		// Token: 0x060048A3 RID: 18595 RVA: 0x0021FE30 File Offset: 0x0021E030
		private bool TryParseConsoleSpecialVariables(string paramName, out object output)
		{
			bool flag = paramName == "$taiwu_id";
			bool result;
			if (flag)
			{
				output = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				result = true;
			}
			else
			{
				output = null;
				result = false;
			}
			return result;
		}

		// Token: 0x060048A4 RID: 18596 RVA: 0x0021FE6B File Offset: 0x0021E06B
		public void OnGmWindowOpen()
		{
			this._inputField.text = this._inputField.text.Replace("`", "");
		}

		// Token: 0x060048A5 RID: 18597 RVA: 0x0021FE94 File Offset: 0x0021E094
		public void FocusInputField()
		{
			EventSystem.current.SetSelectedGameObject(this._inputField.gameObject, null);
			this._inputField.OnPointerClick(new PointerEventData(EventSystem.current)
			{
				button = PointerEventData.InputButton.Left
			});
		}

		// Token: 0x060048A6 RID: 18598 RVA: 0x0021FECC File Offset: 0x0021E0CC
		private List<string> QueryCommandCompletion(string commandStr)
		{
			return (from x in this._funcs.Keys
			where x.Contains(commandStr)
			select x).ToList<string>();
		}

		// Token: 0x060048A7 RID: 18599 RVA: 0x0021FF10 File Offset: 0x0021E110
		private void HandleInput()
		{
			bool keyDown = Input.GetKeyDown(KeyCode.UpArrow);
			if (keyDown)
			{
				this.MoveCommandHistoryIndex(-1);
			}
			else
			{
				bool keyDown2 = Input.GetKeyDown(KeyCode.DownArrow);
				if (keyDown2)
				{
					this.MoveCommandHistoryIndex(1);
				}
			}
		}

		// Token: 0x060048A8 RID: 18600 RVA: 0x0021FF50 File Offset: 0x0021E150
		private void MoveCommandHistoryIndex(int delta)
		{
			bool flag = this._commandHistory.Count == 0;
			if (!flag)
			{
				this._commandHistoryIndex += delta;
				bool flag2 = this._commandHistoryIndex > this._commandHistory.Count - 1;
				if (flag2)
				{
					this._commandHistoryIndex = this._commandHistory.Count;
					this.SetInputFieldText("");
				}
				else
				{
					bool flag3 = this._commandHistoryIndex < 0;
					if (flag3)
					{
						this._commandHistoryIndex = 0;
					}
					this.SetInputFieldText(this._commandHistory[this._commandHistoryIndex]);
				}
			}
		}

		// Token: 0x060048A9 RID: 18601 RVA: 0x0021FFE8 File Offset: 0x0021E1E8
		public void GenerateConsoleHistoryFromUi(MethodInfo methodInfo, object[] args)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			string commandName = GMCommandLine.GenerateGmCommandName(methodInfo);
			bool flag = commandName == null;
			if (flag)
			{
				Debug.LogError("GenerateGmCommandName failed.");
			}
			else
			{
				sb.Append(commandName);
				foreach (object arg in args)
				{
					sb.Append(" ");
					bool flag2 = arg is string;
					if (flag2)
					{
						sb.Append("\"");
						sb.Append(arg);
						sb.Append("\"");
					}
					else
					{
						sb.Append(arg);
					}
				}
				string commandStr = sb.ToString();
				EasyPool.Free<StringBuilder>(sb);
				this._commandHistory.Add(commandStr);
				bool flag3 = this._commandHistory.Count > 32;
				if (flag3)
				{
					this._commandHistory.RemoveAt(0);
				}
				this._commandHistoryIndex = this._commandHistory.Count;
				this.ResetCommandHistoryIndex();
			}
		}

		// Token: 0x04003219 RID: 12825
		private UI_GMWindow _gmWindow;

		// Token: 0x0400321A RID: 12826
		private TMP_InputField _inputField;

		// Token: 0x0400321B RID: 12827
		private GameObject _gmCommandCompletionItem;

		// Token: 0x0400321C RID: 12828
		private RectTransform _possibleCommands;

		// Token: 0x0400321D RID: 12829
		private Dictionary<string, GMCommandLine.CommandLineFunc> _funcs = new Dictionary<string, GMCommandLine.CommandLineFunc>();

		// Token: 0x0400321E RID: 12830
		private const int MaxCommandHistory = 32;

		// Token: 0x0400321F RID: 12831
		private List<string> _commandHistory = new List<string>();

		// Token: 0x04003220 RID: 12832
		private int _commandHistoryIndex;

		// Token: 0x020019CB RID: 6603
		private class CommandLineFunc
		{
			// Token: 0x0400B3C3 RID: 46019
			public MethodInfo Method;

			// Token: 0x0400B3C4 RID: 46020
			public GmRunMode RunMode;
		}
	}
}
