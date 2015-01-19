// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using System.Collections.Generic;

namespace GenericHelpers.Series
{
	public class MovingAvgBuilder : ISeriesBuilder
	{
		private ISeriesBuilder _seriesToCalculateOn;
		private List<Single> _newValues;
		private int _steps;

		public bool IsContinuous { get { return true; } }

		public MovingAvgBuilder(ISeriesBuilder seriesToCalculateOn, int steps)
		{
			if (!seriesToCalculateOn.IsContinuous) throw new InvalidOperationException("MovingAvg can be calculate only on continuos series!");
			_seriesToCalculateOn = seriesToCalculateOn;
			_steps = steps;
			_newValues = new List<float>();


		}

		public void ElaborateTimeSeries(List<TemporalValue> temporalValues)
		{
			var startPoint = -1;
			for (int i = 0; i < temporalValues.Count; i++)
			{
				if (temporalValues[i].IsSet)
				{
					startPoint = i;
					break;
				}
			}
			if (startPoint == -1) throw new InvalidOperationException("No data set!");
			_seriesToCalculateOn.ElaborateTimeSeries(temporalValues);
			for (int i = startPoint; i < temporalValues.Count; i++)
			{
				if (i < (startPoint + _steps))
				{
					_newValues.Add(0);
				}
				else
				{
					_newValues.Add(CalculatePrevAvg(i, temporalValues));
				}
			}
			for (int i = 0; i < temporalValues.Count; i++)
			{
				if (i < (startPoint + _steps))
				{
					temporalValues[i].Value = 0;
					temporalValues[i].IsSet = false;
				}
				else
				{
					temporalValues[i].Value = _newValues[i];
				}
			}
		}

		private float CalculatePrevAvg(int upTo, List<TemporalValue> temporalValues)
		{
			float result = 0.0f;
			for (int i = upTo - _steps; i < upTo; i++)
			{
				result += temporalValues[i].Value;
			}
			if (result == 0) return 0;
			return result / _steps;
		}
	}

}
