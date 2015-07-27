using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using VersionOne.ServiceHost.ConfigurationTool.Attributes;

namespace VersionOne.ServiceHost.ConfigurationTool.Entities {
    public abstract class BaseEntity {
        public const string DisabledProperty = "Disabled";
        protected bool disabled;

        [XmlIgnore]
        public virtual string TagName { get; set; }

        [XmlAttribute("class")]
        public string ClassName {
            get { return ServicesMap.GetByEntityType(GetType()).FullTypeNameAndAssembly; }
            set { }
        }

        [XmlIgnore]
        [HelpString(HelpResourceKey = "CommonDisabled")]
        public bool Disabled
        {
            get { return disabled; }
            set
            {
                if (disabled != value)
                {
                    disabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //[XmlAttribute("disabled")]
        //public int DisabledNumeric {
        //    get { return Convert.ToInt32(Disabled); }
        //    set { Disabled = Convert.ToBoolean(value); }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}