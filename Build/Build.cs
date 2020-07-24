using GlobalRoam.Build.Nuke;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using System;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NugetPackageBuild
{
	protected override bool IncludeSourceInPack => false;
	protected override bool IncludeSymbolsInPack => false;

	public static int Main() => Execute<Build>(x => x.Test);

}
