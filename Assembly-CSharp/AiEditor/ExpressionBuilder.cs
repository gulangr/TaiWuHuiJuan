using System;
using System.Collections.Generic;
using Config;
using GameData.Combat.Math;
using GameData.Utilities;

namespace AiEditor
{
	// Token: 0x02000686 RID: 1670
	public class ExpressionBuilder
	{
		// Token: 0x06004EBA RID: 20154 RVA: 0x0024F1C1 File Offset: 0x0024D3C1
		public void Clear()
		{
			this._parts.Clear();
		}

		// Token: 0x06004EBB RID: 20155 RVA: 0x0024F1D0 File Offset: 0x0024D3D0
		public void BuildOperator(EExpressionOperatorType opType)
		{
			this._parts.Add(new CExpressionPart
			{
				Type = EExpressionPartType.Operator,
				Value = (int)opType
			});
		}

		// Token: 0x06004EBC RID: 20156 RVA: 0x0024F204 File Offset: 0x0024D404
		public void BuildNumber(int number)
		{
			this._parts.Add(new CExpressionPart
			{
				Type = EExpressionPartType.Number,
				Value = number
			});
		}

		// Token: 0x06004EBD RID: 20157 RVA: 0x0024F238 File Offset: 0x0024D438
		public void BuildPersonality(sbyte personalityType)
		{
			this._parts.Add(new CExpressionPart
			{
				Type = EExpressionPartType.Personality,
				Value = (int)personalityType
			});
		}

		// Token: 0x06004EBE RID: 20158 RVA: 0x0024F26C File Offset: 0x0024D46C
		public void BuildConsummateLevel()
		{
			this._parts.Add(new CExpressionPart
			{
				Type = EExpressionPartType.ConsummateLevel,
				Value = 0
			});
		}

		// Token: 0x06004EBF RID: 20159 RVA: 0x0024F2A0 File Offset: 0x0024D4A0
		public void BuildBehaviorType()
		{
			this._parts.Add(new CExpressionPart
			{
				Type = EExpressionPartType.BehaviorType,
				Value = 0
			});
		}

		// Token: 0x06004EC0 RID: 20160 RVA: 0x0024F2D4 File Offset: 0x0024D4D4
		private static bool TrySampleZero(CExpression expression, out int sampleZero, out Exception exception)
		{
			bool result;
			try
			{
				sampleZero = expression.SampleZero();
				exception = null;
				result = true;
			}
			catch (Exception e)
			{
				sampleZero = 0;
				exception = e;
				result = false;
			}
			return result;
		}

		// Token: 0x06004EC1 RID: 20161 RVA: 0x0024F310 File Offset: 0x0024D510
		public CExpression ToExpressionNoWarnings()
		{
			CExpression result = new CExpression(this._parts);
			int num;
			Exception ex;
			return ExpressionBuilder.TrySampleZero(result, out num, out ex) ? result : null;
		}

		// Token: 0x06004EC2 RID: 20162 RVA: 0x0024F340 File Offset: 0x0024D540
		public CExpression ToExpression()
		{
			CExpression result = new CExpression(this._parts);
			int sampleZero;
			Exception exception;
			bool flag = ExpressionBuilder.TrySampleZero(result, out sampleZero, out exception);
			CExpression result2;
			if (flag)
			{
				AdaptableLog.Info(string.Format("Build success. SampleZero is {0}", sampleZero));
				result2 = result;
			}
			else
			{
				PredefinedLog.Show(8, "Build failed with exception " + exception.Message + "\nstacktrace:\n" + exception.StackTrace);
				result2 = null;
			}
			return result2;
		}

		// Token: 0x0400364D RID: 13901
		private readonly List<CExpressionPart> _parts = new List<CExpressionPart>();
	}
}
