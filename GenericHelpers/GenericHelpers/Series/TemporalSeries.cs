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

	public class TemporalSeries
	{
		private TimeSpan _span;
		private readonly List<TemporalValue> _temporalValues;
		private DateTime _from;
		private DateTime _to;
		private ISeriesBuilder _seriesBuilder;

		public List<TemporalValue> Values
		{
			get
			{
				return _temporalValues;
			}
		}

		public TemporalSeries(TimeSpan span, DateTime from, DateTime to, ISeriesBuilder seriesBuilder = null)
		{
			_span = span;
			_from = from;
			_to = to;
			_seriesBuilder = seriesBuilder;
			_temporalValues = new List<TemporalValue>();
			SetupTimeSeries();
		}

		private void SetupTimeSeries()
		{
			var start = _from;
			while (start < _to)
			{
				_temporalValues.Add(new TemporalValue { Current = start });
				start += _span;
			}
		}

		public void AddValues<T>(IEnumerable<T> values, Func<T, int> retrieveValue, Func<T, DateTime> retrieveDate)
		{
			foreach (var item in values)
			{
				AddValue(retrieveDate(item), retrieveValue(item));
			}
		}

		public void AddValue(DateTime sampleTimestamp, int sampleValue)
		{
			if (sampleTimestamp < _from || sampleTimestamp > _to) return;
			var relativePosition = sampleTimestamp - _from;
			var step = (int)Math.Floor((double)(relativePosition.TotalMilliseconds / _span.TotalMilliseconds));
			if (step > _temporalValues.Count) return;
			var current = _temporalValues[step];
			current.Value += sampleValue;
			current.IsSet = true;
		}

		public void DoFinalize()
		{
			if (_seriesBuilder != null)
			{
				_seriesBuilder.ElaborateTimeSeries(_temporalValues);
			}
		}
	}
}
