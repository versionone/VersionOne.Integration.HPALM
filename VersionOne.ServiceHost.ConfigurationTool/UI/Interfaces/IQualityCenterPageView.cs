using System;

using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using System.Collections.Generic;

namespace VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces {
    public interface IQualityCenterPageView : IPageView<QCServiceEntity> {
        event EventHandler ValidationRequested;
        void SetValidationResult(bool validationSuccessful);

        IList<string> SourceList { get; set; }
        IList<string> ProjectList { get; set; }
        IList<ListValue> VersionOnePriorities { get; set; }

        void SetGeneralTabValid(bool isValid);
        void SetMappingTabValid(bool isValid);
    }
}