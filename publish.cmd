@ECHO OFF

SET config=Release
SET proj=src\Cascade.DirectoryAgent\Cascade.DirectoryAgent.csproj
SET dest=publish

dotnet publish %proj% -c %config% -o %dest%