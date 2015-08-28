$path = Resolve-Path ".\VersionOne.ServiceHost\App.config"

function Clean-ConfigFile {
    $xml = New-Object System.Xml.XmlDocument
    $xml.PreserveWhitespace = $true
    $xml.Load($path)

    # LogService settings
    $xml.configuration.Services.LogService.Console.enabled = "1"
    $xml.configuration.Services.LogService.Console.LogLevel = "Debug"
    $xml.configuration.Services.LogService.File.enabled = "1"
    $xml.configuration.Services.LogService.File.LogLevel = "Info"

    # ProfileFlushTimer settings    
    $xml.configuration.Services.ProfileFlushTimer.Interval = "10000"

    # QualityCenterService settings
    $xml.configuration.Services.QualityCenterService.Connection.ApplicationUrl = "http://hostname:port/qcbin"
    $xml.configuration.Services.QualityCenterService.Connection.Username = "username"
    $xml.configuration.Services.QualityCenterService.Connection.Password = "password"
    $scrubProject = "true"    
    $xml.configuration.Services.QualityCenterService.QCProjects.Project | % {
        if ($scrubProject -eq "true") {
            $_.id = ""
            $_.Project = ""
            $_.VersionOneProject = ""
            $scrubProject = "false"
        } else {
            $xml.configuration.Services.QualityCenterService.QCProjects.RemoveChild($_)
        }
    }

    # V1TestService settings
    $xml.configuration.Services.V1TestService.Settings.AuthenticationType = "0"
    $xml.configuration.Services.V1TestService.Settings.ApplicationUrl = "http://server/instance"
    $xml.configuration.Services.V1TestService.Settings.AccessToken = ""
    $xml.configuration.Services.V1TestService.Settings.Username = ""
    $xml.configuration.Services.V1TestService.Settings.Password = ""
    $scrubMapping = "true"    
    $xml.configuration.Services.V1TestService.TestPublishProjectMap.V1Project | % {
        if ($scrubMapping -eq "true") {
            $_.Name = ""
            $_.IncludeChildren = "Y"
            $_.InnerText = ""
            $scrubMapping = "false"
        } else {
            $xml.configuration.Services.V1TestService.TestPublishProjectMap.RemoveChild($_)
        }
    }

    # WorkitemWriterService settings
    $xml.configuration.Services.WorkitemWriterService.Settings.AuthenticationType = "0"
    $xml.configuration.Services.WorkitemWriterService.Settings.ApplicationUrl = "http://server/instance"
    $xml.configuration.Services.WorkitemWriterService.Settings.AccessToken = ""
    $xml.configuration.Services.WorkitemWriterService.Settings.Username = ""
    $xml.configuration.Services.WorkitemWriterService.Settings.Password = ""

    # V1TestReadTimer settings
    $xml.configuration.Services.V1TestReadTimer.Interval = "300000"

    # QCTestReadTimer settings    
    $xml.configuration.Services.QCTestReadTimer.Interval = "300000"

	$xml.Save($path);
}

Clean-ConfigFile
