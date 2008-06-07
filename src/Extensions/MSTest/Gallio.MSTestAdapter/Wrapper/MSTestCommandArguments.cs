﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal class MSTestCommandArguments
    {
        public bool NoLogo { get; set; }
        public bool NoIsolation { get; set; }
        public string ResultsFile { get; set; }
        public string TestList { get; set; }
        public string TestMetadata { get; set; }

        public MSTestCommandArguments Copy()
        {
            MSTestCommandArguments copy = new MSTestCommandArguments();
            copy.NoLogo = NoLogo;
            copy.NoIsolation = NoIsolation;
            copy.ResultsFile = ResultsFile;
            copy.TestList = TestList;
            copy.TestMetadata = TestMetadata;
            return copy;
        }

        public string[] ToStringArray()
        {
            List<string> args = new List<string>();

            if (NoLogo)
                AddArgument(args, "/nologo", null);
            if (NoIsolation)
                AddArgument(args, "/noisolation", null);
            if (ResultsFile != null)
                AddArgument(args, "/resultsfile", ResultsFile);
            if (TestList != null)
                AddArgument(args, "/testlist", TestList);
            if (TestMetadata != null)
                AddArgument(args, "/testmetadata", TestMetadata);

            return args.ToArray();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (string arg in ToStringArray())
            {
                if (result.Length != 0)
                    result.Append(' ');
                result.Append('"').Append(arg).Append('"');
            }

            return result.ToString();
        }

        private static void AddArgument(ICollection<string> args, string argName, string argValue)
        {
            if (argValue != null)
                args.Add(argName + ":" + argValue);
            else
                args.Add(argName);
        }
    }
}