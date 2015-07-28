using System.Xml.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using VersionOne.ServiceHost.ConfigurationTool.Attributes;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    /// <summary>
    /// Core TestWriterService configuration entity.
    /// </summary>
    [DependsOnVersionOne]
    [XmlRoot("TestWriterService")]
    public class TestWriterEntity : BaseServiceEntity, IVersionOneSettingsConsumer {
        public const string PassedOidProperty = "PassedOid";
        public const string FailedOidProperty = "FailedOid";
        public const string ReferenceAttributeProperty = "ReferenceAttribute";
        public const string ChangeCommentProperty = "ChangeComment";
        public const string DescriptionSuffixProperty = "DescriptionSuffix";
        public const string CreateDefectProperty = "CreateDefect";

        private string _passedOid;
        private string _failedOid;
        private string _referenceAttribute;
        private string _changeComment;
        private string _descriptionSuffix;
        private string _createDefect;

        [NonEmptyStringValidator]
        [HelpString(HelpResourceKey = "TestsPassedStatus")]
        public string PassedOid {
            get
            {
                return _passedOid;
            }
            set
            {
                if (!value.Equals(_passedOid))
                {
                    _passedOid = value;
                    NotifyPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        [HelpString(HelpResourceKey = "TestsFailedStatus")]
        public string FailedOid {
            get { return _failedOid; }
            set
            {
                if (!value.Equals(_failedOid))
                {
                    _failedOid = value;
                    NotifyPropertyChanged();
                }
            } }

        [XmlElement("TestReferenceAttribute")]
        [NonEmptyStringValidator]
        [HelpString(HelpResourceKey = "TestsReference")]
        public string ReferenceAttribute {
            get
            {
                return _referenceAttribute;
            }
            set
            {
                if (!value.Equals(_referenceAttribute))
                {
                    _referenceAttribute = value;
                    NotifyPropertyChanged();
                }
            } }

        [StringLengthValidator(3, 255, MessageTemplate = "Comment length should be 3 to 255 chars")]
        [HelpString(HelpResourceKey = "TestsComment")]
        public string ChangeComment {
            get
            {
                return _changeComment;
            }
            set
            {
                if (!value.Equals(_changeComment))
                {
                    _changeComment = value;
                    NotifyPropertyChanged();
                }
            } }

        [HelpString(HelpResourceKey = "TestsDescription")]
        public string DescriptionSuffix {
            get
            {
                return _descriptionSuffix;
            }
            set
            {
                if (!value.Equals(_descriptionSuffix))
                {
                    _descriptionSuffix = value;
                    NotifyPropertyChanged();
                }
            } }

        [NonEmptyStringValidator]
        [HelpString(HelpResourceKey = "TestsCreateDefect")]
        public string CreateDefect {
            get
            {
                return _createDefect;
            }
            set
            {
                if (!value.Equals(_createDefect))
                {
                    _createDefect = value;
                    NotifyPropertyChanged();
                }
            } }

        public VersionOneSettings Settings { get; set; }

        public TestWriterEntity() {
            Settings = new VersionOneSettings();
        }

        public override bool Equals(object obj) {
            if(obj == null || obj.GetType() != GetType()) {
                return false;
            }

            var other = (TestWriterEntity)obj;

            return string.Equals(PassedOid, other.PassedOid) && string.Equals(FailedOid, other.FailedOid) &&
                string.Equals(ReferenceAttribute, other.ReferenceAttribute) &&
                    string.Equals(ChangeComment, other.ChangeComment) &&
                        string.Equals(DescriptionSuffix, other.DescriptionSuffix) &&
                            string.Equals(CreateDefect, other.CreateDefect) &&
                                Disabled == other.Disabled;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}