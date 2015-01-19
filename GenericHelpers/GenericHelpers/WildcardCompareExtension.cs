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

namespace GenericHelpers
{
	public static class WildcardCompareExtension
	{
		/// <summary>
		/// Thanks to:
		/// http://stackoverflow.com/questions/2433998/system-stringcomparer-that-supports-wildcard
		/// </summary>
		public static bool WildcardCompare(this string toCheck, string mask, StringComparison stringComparison = StringComparison.Ordinal)
		{
			int i = 0, k = 0;

			while (k != toCheck.Length)
			{
				if (i > mask.Length - 1)
					return false;

				switch (mask[i])
				{
					case '*':
						{
							if ((i + 1) == mask.Length)
							{
								return true;
							}

							while (k != toCheck.Length)
							{
								if (string.Compare(toCheck.Substring(k + 1), mask.Substring(i + 1), stringComparison) == 0)
								{
									return true;
								}
								k += 1;
							}
						}
						return false;
					case '?':
						break;
					default:
						if (string.Compare(toCheck.Substring(k, 1), mask.Substring(i, 1), stringComparison) != 0)
						{
							return false;
						}
						break;
				}

				i += 1;
				k += 1;
			}

			if (k == toCheck.Length)
			{
				if (i == mask.Length || mask[i] == ';' || mask[i] == '*')
					return true;
			}

			return false;
		}
	}
}
