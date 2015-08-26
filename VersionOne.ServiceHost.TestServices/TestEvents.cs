namespace VersionOne.ServiceHost.TestServices {
    /**
	 * Event used to signal a V1 test ready to move to the partner system
	 */
    public class V1TestReady {
        public readonly string Description;
        public readonly string DisplayId;
        public readonly string Oid;
        public readonly string Project;
        public readonly string Title;

        public V1TestReady(string oid, string displayId, string title, string description, string project) {
            Oid = oid;
            DisplayId = displayId;
            Title = title;
            Description = description;
            Project = project;
        }
    }

    /**
	 * Event used to indicate that the partner system has accepted the V1 test
	 */
    public class PartnerTestEvent {
        public readonly string Oid;

        private bool successful = true;

        public PartnerTestEvent(V1TestReady v1Test) {
            Oid = v1Test.Oid;
        }

        public string Reference { get; set; }

        public bool Successful {
            get { return successful; }
            set { successful = value; }
        }
    }
}