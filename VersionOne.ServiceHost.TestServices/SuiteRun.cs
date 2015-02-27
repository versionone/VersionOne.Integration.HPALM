using System;

namespace VersionOne.ServiceHost.TestServices {
    public class SuiteRun {
        public readonly string Description;
        public readonly string Name;
        public readonly DateTime Stamp;
        public readonly string SuiteRef;

        public SuiteRun(string name, string description, DateTime stamp, string suiteref) {
            Name = name;
            Description = description;
            Stamp = stamp;
            SuiteRef = suiteref;
        }

        public int Passed { get; set; }

        public int Failed { get; set; }

        public int NotRun { get; set; }

        public double Elapsed { get; set; }

        public override string ToString() {
            return string.Format("{0} - {1} - {2} - {3} - ({4},{5},{6}) {7} ms", Name, Description, Stamp, SuiteRef, Passed, Failed, NotRun, Elapsed);
        }
    }
}