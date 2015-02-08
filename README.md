# SolutionCop

Tool for static analysis of Visual Studio solutions and projects. 

Usage: SolutionCop.exe -s *path-to-vs-solution* [-c *path-to-solutioncop-config*]

If <path-to-solutioncop-config> is not provided then tool looks for SolutionCop.xml file next to target *.sln. If config file cannot be found then default one is created - you can use it for reference.

Any rule can be disabled by setting *enabled* attribute to *false*. Note that tool adds default entries for rules if they are missing.

## Supported rules

* Basic
  * TargetFrameworkVersion
  * FilesIncludedIntoProject
  * TreatWarningsAsErrors
  * SuppressWarnings
  * [WarningLevel](https://github.com/Litee/SolutionCop/wiki/WarningLevel)
* NuGet
  * [NuGetAutomaticPackagesRestore](https://github.com/Litee/SolutionCop/wiki/NuGetAutomaticPackagesRestore)
  * [NuGetPackagesUsage](https://github.com/Litee/SolutionCop/wiki/NuGetPackagesUsage)
  * ReferenceNuGetPackagesOnly
  * [NuGetPackageVersions](https://github.com/Litee/SolutionCop/wiki/NuGetPackageVersions)
* StyleCop
  * StyleCopEnabled
  * TreatStyleCopWarningsAsErrors

TODO - add rule details to wiki

### TODO rules:
* Copy Local
* Binary within NuGet package is referenced directly without proper reference in packages.config
* Same package versions are used across projects (support exceptions)
* VS solution version
* Assembly and root namespace should have same name
* Unapproved build configurations
* No duplicate NuGet packages (looks like low priority - haven't seen this problem in practice for a long time)
* Classify project by type (e.g. production, testing) and disallow references between some groups
* Proper owner in AssemblyInfo
* Proper copyright date in AssemblyInfo
* Forbid pre-release versions of packages (should be added to NuGetPackageVersions rule)

### Other Todos:
* Group errors by project or by rule
* NuGet commands in log for fixing versions
* More flexible exceptions
* Option to fail on missing sections
* Links to broken rule details in wiki
* Treat rules as separate tests for TeamCity (not sure whether it will work better than plain list)
* Custom folders for searching rules
* Configuration can be an exception
* Error messages -> resources