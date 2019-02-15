using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Base.Common;


[XmlType(TypeName = "SystemSetting")]
public class SystemSetting
{
    [XmlElement]
    public bool IsDebug;
    [XmlElement]
    public CinemachineSetting cinemachineSetting;
    [XmlElement]
    public CommunicationSetting communicationSetting;
    [XmlElement]
    public VersionSetting versionSetting;

    public SystemSetting()
    {
        cinemachineSetting = new CinemachineSetting();
        communicationSetting = new CommunicationSetting();
        versionSetting = new VersionSetting();
    }
}

