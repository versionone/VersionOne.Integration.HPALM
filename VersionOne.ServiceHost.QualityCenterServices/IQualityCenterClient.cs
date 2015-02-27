using System;
using System.Collections;

namespace VersionOne.ServiceHost.QualityCenterServices {
    public interface IQualityCenterClient : IDisposable {
        /// <summary>
        /// Get information about the Quality Center project where this object is connected
        /// </summary>
        QCProject ProjectInfo { get; }

        /// <summary>
        /// Create a Quality Center Defect
        /// </summary>
        string CreateQCTest(string title, string description, string externalId);

        /// <summary>
        /// Get the latest test runs
        /// </summary>
        IList GetLatestTestRuns(DateTime lastCheck);

        /// <summary>
        /// Get the latest Defects
        /// </summary>
        IList GetLatestDefects(DateTime lastCheck);

        /// <summary>
        /// Get the fully qualified quality center name based on an identifier
        /// </summary>
        string GetFullyQualifiedQCId(string localId);

        /// <summary>
		/// Update Quality Center Defect with comments and a link
		/// </summary>
        void OnDefectCreated(string id, ICollection comments, string link);

        /// <summary>
		/// Update Status of Quality Center Defect
		/// </summary>
        bool OnDefectStateChange(string id, ICollection comments);
    }
}