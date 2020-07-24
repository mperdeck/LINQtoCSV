using GlobalRoam.Build.Nuke;
using Nuke.Common.Execution;
using System;
using System.Linq;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NugetPackageBuild
{
	public static int Main() => Execute<Build>(x => x.Test);
}
