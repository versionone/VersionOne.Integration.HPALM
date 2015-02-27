using System;

namespace VersionOne.ServiceHost.TestServices {
    public class TestRun {
        public enum TestRunState {
            NotRun,
            Passed,
            Failed
        }

        public readonly DateTime Stamp;
        public readonly string TestRef;

        public TestRunState State { get; set; }

        public double Elapsed { get; set; }

        public TestRun(DateTime stamp, string testref) {
            Stamp = stamp;
            TestRef = testref;
        }

        public override string ToString() {
            return string.Format("{0} - {1} - {2} - {3} ms", Stamp, TestRef, State.ToString(), Elapsed);
        }
    }
}