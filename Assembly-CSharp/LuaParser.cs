using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Tree;
using MoonSharp.Interpreter.Tree.Expressions;
using MoonSharp.Interpreter.Tree.Statements;

// Token: 0x0200002C RID: 44
public static class LuaParser
{
	// Token: 0x0600019B RID: 411 RVA: 0x0000A870 File Offset: 0x00008A70
	public static DynValue Parse(string luaSourceString, Script script = null)
	{
		bool flag = string.IsNullOrEmpty(luaSourceString);
		DynValue result;
		if (flag)
		{
			result = DynValue.Nil;
		}
		else
		{
			SourceCode source = new SourceCode("luaSourceString", luaSourceString, 0, script);
			ScriptLoadingContext ctx = new ScriptLoadingContext(script)
			{
				Scope = new BuildTimeScope(),
				Source = source,
				Lexer = new Lexer(source.SourceID, source.Code, true)
			};
			result = global::LuaParser.MakeStatementValue(script, new ChunkStatement(ctx));
		}
		return result;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000A8E4 File Offset: 0x00008AE4
	private static DynValue MakeStatementValue(Script script, Statement statement)
	{
		CompositeStatement compositeStatement = statement as CompositeStatement;
		DynValue result;
		if (compositeStatement == null)
		{
			ReturnStatement returnStatement = statement as ReturnStatement;
			if (returnStatement == null)
			{
				ChunkStatement chunkStatement = statement as ChunkStatement;
				if (chunkStatement == null)
				{
					result = DynValue.Nil;
				}
				else
				{
					result = global::LuaParser.MakeStatementValue(script, chunkStatement.InnerStatement);
				}
			}
			else
			{
				result = global::LuaParser.MakeExpressionValue(script, returnStatement.ReturnValueExpression);
			}
		}
		else
		{
			result = global::LuaParser.MakeStatementValue(script, compositeStatement.Statements[0]);
		}
		return result;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000A960 File Offset: 0x00008B60
	private static DynValue MakeExpressionValue(Script script, Expression expression)
	{
		ExprListExpression exprListExpression = expression as ExprListExpression;
		DynValue result;
		if (exprListExpression == null)
		{
			TableConstructor tableConstructor = expression as TableConstructor;
			if (tableConstructor == null)
			{
				UnaryOperatorExpression unaryOperatorExpression = expression as UnaryOperatorExpression;
				if (unaryOperatorExpression == null)
				{
					LiteralExpression literalExpression = expression as LiteralExpression;
					if (literalExpression == null)
					{
						result = DynValue.Nil;
					}
					else
					{
						result = literalExpression.Value;
					}
				}
				else
				{
					DynValue value = global::LuaParser.MakeExpressionValue(script, unaryOperatorExpression.m_Exp);
					bool flag = value.Type == DataType.Number;
					if (flag)
					{
						bool flag2 = unaryOperatorExpression.m_OpText.Equals("-");
						if (flag2)
						{
							value = DynValue.NewNumber(value.Number * -1.0);
						}
					}
					result = value;
				}
			}
			else
			{
				Table table = new Table(script);
				foreach (KeyValuePair<Expression, Expression> pair in tableConstructor.Arguments)
				{
					table.Set(global::LuaParser.MakeExpressionValue(script, pair.Key), global::LuaParser.MakeExpressionValue(script, pair.Value));
				}
				foreach (Expression positional in tableConstructor.PositionalValues)
				{
					table.Set(table.Length + 1, global::LuaParser.MakeExpressionValue(script, positional));
				}
				result = DynValue.NewTable(table);
			}
		}
		else
		{
			result = global::LuaParser.MakeExpressionValue(script, exprListExpression.GetExpressions()[0]);
		}
		return result;
	}
}
