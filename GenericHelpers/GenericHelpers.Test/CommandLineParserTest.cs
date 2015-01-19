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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericHelpers.Test
{
	[TestClass]
	public class ReflectionUtilsTest
	{
		[TestMethod]
		public void ReflectionUtils_MustBeUsable()
		{
		}
	}
	[TestClass]
	public class CommandLineParserTest
	{
		[TestMethod]
		public void ItShouldBePossibleToCreateACommandLneParser()
		{
			ShowHelpCalled = 0;
			const string helpString = "help";
			var args = new[] { "-test", "-parameter", "parameterValue" };
			var commandLineParser = new CommandLineParser(args, helpString);
			var resultValue = commandLineParser["test"];
			Assert.IsTrue(commandLineParser.IsSet("test"));
			Assert.IsTrue(resultValue.Length == 0);
			resultValue = commandLineParser["notPresent"];
			Assert.IsNull(resultValue);
			Assert.IsFalse(commandLineParser.IsSet("notPresent"));
			resultValue = commandLineParser["parameter"];
			Assert.IsTrue(commandLineParser.IsSet("parameter"));
			Assert.AreEqual("parameterValue", resultValue);

			commandLineParser["parameter"] = "newValue";
			Assert.IsTrue(commandLineParser.IsSet("parameter"));
			resultValue = commandLineParser["parameter"];
			Assert.AreEqual("newValue", resultValue);
		}

		[TestMethod]
		public void ItShouldBePossibleToCreateACommandLneParserAndShowHelp()
		{
			ShowHelpCalled = 0;
			const string helpString = "help";
			var args = new[] { "-test", "-help" };
			var commandLineParser = new CommandLineParser(args, helpString, () => { ShowHelpCalled++; });
			Assert.AreEqual(helpString, commandLineParser.Help);
			Assert.IsTrue(commandLineParser.IsSet("help"));
			Assert.AreEqual(1, ShowHelpCalled);
		}

		[TestMethod]
		public void ItShouldBePossibleToCheckForValuesPresence()
		{
			ShowHelpCalled = 0;
			const string helpString = "help";
			var args = new[] { "-test", "-gino", "-pino", "pinoValue" };
			var commandLineParser = new CommandLineParser(args, helpString, () => { ShowHelpCalled++; });

			Assert.IsTrue(commandLineParser.Has(new[] { "test", "gino" }));
			Assert.IsFalse(commandLineParser.Has(new[] { "test", "fake" }));
			Assert.IsFalse(commandLineParser.Has(new[] { "fluke", "fake" }));

			Assert.IsTrue(commandLineParser.HasAllOrNone(new[] { "test", "gino" }));
			Assert.IsFalse(commandLineParser.HasAllOrNone(new[] { "test", "fake" }));
			Assert.IsTrue(commandLineParser.HasAllOrNone(new[] { "fluke", "fake" }));

			Assert.IsFalse(commandLineParser.HasOneAndOnlyOne(new[] { "test", "gino" }));
			Assert.IsTrue(commandLineParser.HasOneAndOnlyOne(new[] { "test", "fake" }));
			Assert.IsFalse(commandLineParser.HasOneAndOnlyOne(new[] { "fluke", "fake" }));

			Assert.AreEqual(0, ShowHelpCalled);
		}


		[TestMethod]
		public void ItShouldBePossibleToGetEnvironmentVariables()
		{
			ShowHelpCalled = 0;
			var args = new[] { "-test" };
			var commandLineParser = new CommandLineParser(args, "help");
			var temp = CommandLineParser.GetEnv("TEMP");
			commandLineParser["TEMP"] = temp;
			Assert.IsTrue(Directory.Exists(temp));
			var os = CommandLineParser.GetEnv("OS");
			commandLineParser["os"] = os;

			var notExistingVariable = CommandLineParser.GetEnv("thisDoesNotExists" + Guid.NewGuid());
			Assert.IsNull(notExistingVariable);

			Assert.IsTrue(commandLineParser.IsSet("os"));
			Assert.IsTrue(commandLineParser.IsSet("oS"));
			Assert.AreEqual(commandLineParser["os"], commandLineParser["oS"]);

			Assert.IsTrue(commandLineParser.IsSet("Temp"));
			Assert.IsTrue(commandLineParser.IsSet("temP"));
			Assert.AreEqual(commandLineParser["TEMP"], commandLineParser["temp"]);

			Assert.IsFalse(string.IsNullOrWhiteSpace(os));
		}

		public int ShowHelpCalled = 0;


		[Ignore]
		[TestMethod]
		public void ShouldBeAbleToHandleMultipleItems()
		{

		}
		[Ignore]
		[TestMethod]
		public void ShouldBeAbleToSetGetEnvironmentVariables()
		{

		}
	}
}
