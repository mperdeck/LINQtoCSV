rem Builds the project and creates NuGet package. 

nuget pack ..\..\LINQtoCSV\LINQtoCSV.csproj -Prop Configuration=Release -Build -OutputDirectory ..\GeneratedPackages
