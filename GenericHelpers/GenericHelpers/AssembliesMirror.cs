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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace GenericHelpers
{


    public static class AssembliesMirror
    {

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        private enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 1,
            MOVEFILE_COPY_ALLOWED = 2,
            MOVEFILE_DELAY_UNTIL_REBOOT = 4,
            MOVEFILE_WRITE_THROUGH = 8
        }
        // ReSharper restore UnusedMember.Local
        // ReSharper restore InconsistentNaming

        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "MoveFileEx")]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        public static string MainDllPath { get; private set; }

        public static ReadOnlyCollection<string> AllDlls { get; private set; }

        public static void Initialize(string binPath, params string[] otherPaths)
        {
            var dlls = new List<string>();

            AllDlls = new ReadOnlyCollection<string>(new List<string>());
            var reversed = otherPaths.Reverse();
            MainDllPath = binPath;
            if (string.IsNullOrWhiteSpace(MainDllPath))
            {
                var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tmpPath);
                MainDllPath = tmpPath;
                MoveFileEx(tmpPath, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
            }
            else
            {
                Directory.CreateDirectory(MainDllPath);
            }
            foreach (var dir in reversed)
            {
                var addedDlls = LoadDlls(dir);
                dlls.AddRange(addedDlls);
            }
            AllDlls = new ReadOnlyCollection<string>(dlls);
        }

        private static List<string> LoadDlls(string dir)
        {
            var dlls = new List<string>();
            foreach (var dllFile in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var fileName = Path.GetFileName(dllFile);
                if (fileName == null) continue;
                var resultFile = Path.Combine(MainDllPath, fileName);
                if (File.Exists(resultFile)) continue;
                File.Copy(dllFile, resultFile);
                MoveFileEx(resultFile, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
                dlls.Add(resultFile);
            }
            dlls.Reverse();
            return dlls;
        }

        public static void InitializeAppDomain(AppDomain appDomain)
        {
            appDomain.SetupInformation.PrivateBinPath = MainDllPath;
        }

    }
}
