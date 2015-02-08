﻿using System.IO;
using System.Xml.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Shouldly;
using Xunit;

namespace SolutionCop.DefaultRules.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("ApprovedResults")]
    public class ReferenceNuGetPackagesOnlyRuleTests : ProjectRuleTest
    {
        public ReferenceNuGetPackagesOnlyRuleTests() : base(new ReferenceNuGetPackagesOnlyRule())
        {
        }

        [Fact]
        public void Should_accept_project_references_to_packages_only()
        {
            var xmlConfig = XElement.Parse("<ReferenceNuGetPackagesOnly/>");
            ShouldPassNormally(new FileInfo(@"..\..\Data\ReferenceNuGetPackagesOnlyRule\ReferencesPackagesFolderOnly.csproj").FullName, xmlConfig);
        }

        [Fact]
        public void Should_fail_for_project_with_direct_references_to_binaries()
        {
            var xmlConfig = XElement.Parse("<ReferenceNuGetPackagesOnly/>");
            ShouldFailNormally(new FileInfo(@"..\..\Data\ReferenceNuGetPackagesOnlyRule\HasReferencesToLocalBinaries.csproj").FullName, xmlConfig);
        }

        [Fact]
        public void Should_pass_for_project_with_direct_references_to_binaries_if_project_is_an_exception()
        {
            var xmlConfig = XElement.Parse(@"
<ReferenceNuGetPackagesOnly>
  <Exception>
    <Project>HasReferencesToLocalBinaries.csproj</Project>
  </Exception>
</ReferenceNuGetPackagesOnly>");
            ShouldPassNormally(new FileInfo(@"..\..\Data\ReferenceNuGetPackagesOnlyRule\HasReferencesToLocalBinaries.csproj").FullName, xmlConfig);
        }

        [Fact]
        public void Should_fail_if_exception_misses_project()
        {
            var xmlConfig = XElement.Parse(@"
<ReferenceNuGetPackagesOnly>
  <Exception>Some text</Exception>
</ReferenceNuGetPackagesOnly>");
            ShouldFailOnConfiguration(new FileInfo(@"..\..\Data\ReferenceNuGetPackagesOnlyRule\HasReferencesToLocalBinaries.csproj").FullName, xmlConfig);
        }

        [Fact]
        public void Should_pass_if_rule_is_disabled()
        {
            var xmlConfig = XElement.Parse("<ReferenceNuGetPackagesOnly enabled=\"false\"/>");
            ShouldPassAsDisabled(new FileInfo(@"..\..\Data\ReferenceNuGetPackagesOnlyRule\HasReferencesToLocalBinaries.csproj").FullName, xmlConfig);
        }
    }
}