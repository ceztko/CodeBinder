#!/usr/bin/env pwsh

# Builds, and optionally publishes, the Java redistributable components:
#
#   org.codebinder:codebinder-redist          multi-release jar for JDK projects
#   org.codebinder:codebinder-android-redist  plain jar for Android projects
#
# Both carry the same "CodeBinder" package and must never end up on the same
# classpath. The version is declared once, as the "revision" property of the
# root pom: bump it there.

[CmdletBinding(DefaultParameterSetName = 'None')]
param (
    [string]$action
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'stop'

# Splits "https://<repositoryId>@host/path" into the repository id, which selects
# the <server> of settings.xml holding the credentials, and the url to publish to.
# An empty spec means maven central, which nobody here can write to: an Upload
# that forgot to set the variable fails on authentication instead of publishing
# somewhere unintended.
function resolveRepository([string]$spec, [string]$variableName)
{
    if (!$spec) {
        return @{ Id = "central"; Url = "https://repo.maven.apache.org/maven2" }
    }

    if ($spec -notmatch '^(?<scheme>[a-zA-Z][a-zA-Z0-9+.-]*://)(?<id>[^/@]+)@(?<location>.+)$') {
        throw "${variableName} must have the form <scheme>://<repositoryId>@<host>/<path>, got '${spec}'"
    }

    return @{ Id = $Matches.id; Url = $Matches.scheme + $Matches.location }
}

function doWork()
{
    if (!$action) {
        $action = "Build"
    }

    $version = (mvn --batch-mode --quiet help:evaluate "-Dexpression=project.version" -DforceStdout).Trim()

    # The two artifacts go to two different repositories, for example
    #
    #   $env:CODEBINDER_JDK_MAVEN_REPO = "https://<repositoryId>@<host>/<path>"
    #   $env:CODEBINDER_ANDROID_MAVEN_REPO = "https://<repositoryId>@<host>/<path>"
    #
    # where the part before the "@" is the <server> id of settings.xml that
    # supplies the credentials for that host.
    $jdkRepository = resolveRepository $env:CODEBINDER_JDK_MAVEN_REPO "CODEBINDER_JDK_MAVEN_REPO"
    $androidRepository = resolveRepository $env:CODEBINDER_ANDROID_MAVEN_REPO "CODEBINDER_ANDROID_MAVEN_REPO"

    Write-Output "Version: ${version}"
    Write-Output "Action: ${action}"
    Write-Output "JDK repository: $($jdkRepository.Url) ($($jdkRepository.Id))"
    Write-Output "Android repository: $($androidRepository.Url) ($($androidRepository.Id))"

    switch ($action)
    {
        "Upload"
        {
            # The poms handed over are the flattened ones: parent free, with the
            # version resolved, so consumers never need anything else
            mvn --batch-mode --no-transfer-progress deploy:deploy-file `
                "-DpomFile=jdk/target/pom-publish.xml" `
                "-Dfile=jdk/target/codebinder-redist-${version}.jar" `
                "-Dsources=jdk/target/codebinder-redist-${version}-sources.jar" `
                "-Durl=$($jdkRepository.Url)" "-DrepositoryId=$($jdkRepository.Id)"

            mvn --batch-mode --no-transfer-progress deploy:deploy-file `
                "-DpomFile=android/target/pom-publish.xml" `
                "-Dfile=android/target/codebinder-android-redist-${version}.jar" `
                "-Dsources=android/target/codebinder-android-redist-${version}-sources.jar" `
                "-Durl=$($androidRepository.Url)" "-DrepositoryId=$($androidRepository.Id)"
            break
        }
        # "Build"
        default
        {
            mvn --batch-mode --no-transfer-progress clean package
            break
        }
    }
}

Push-Location $PSScriptRoot
try
{
    doWork
}
finally
{
    Pop-Location
}
