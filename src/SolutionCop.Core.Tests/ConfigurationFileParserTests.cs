﻿namespace SolutionCop.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Linq;
    using ApprovalTests;
    using ApprovalTests.Namers;
    using ApprovalTests.Reporters;
    using Shouldly;
    using Xunit;
    using Xunit.Extensions;

    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("ApprovedResults")]
    public class ConfigurationFileParserTests
    {
        [Theory]
        [InlineData("<Rules></Rules>")]
        [InlineData("<Rules xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"SolutionCop.xsd\"></Rules>")]
        public void Should_pass_for_empty_config(string xmlRules)
        {
            var newConfigFileContent = string.Empty;
            Action<string, byte[]> saveConfigFileAction = (s, bytes) =>
            {
                newConfigFileContent = Encoding.UTF8.GetString(bytes);
            };
            var instance = CreateInstance(saveConfigFileAction);
            var errors = new List<string>();
            instance.ParseConfigString("SolutionCop.xml", xmlRules, new IProjectRule[] { new DummyRule() }, errors, false);
            errors.ShouldBeEmpty();
            newConfigFileContent.ShouldNotBeEmpty();
            Approvals.Verify(newConfigFileContent);
        }

        [Fact]
        public void Should_pass_for_config_with_valid_section()
        {
            const string xmlRules = @"
<Rules xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:noNamespaceSchemaLocation='SolutionCop.xsd'>
  <Dummy />
</Rules>";
            var newConfigFileContent = string.Empty;
            Action<string, byte[]> saveConfigFileAction = (s, bytes) =>
            {
                newConfigFileContent = Encoding.UTF8.GetString(bytes);
            };
            var instance = CreateInstance(saveConfigFileAction);
            var errors = new List<string>();
            instance.ParseConfigString("SolutionCop.xml", xmlRules, new IProjectRule[] { new DummyRule() }, errors, false);
            errors.ShouldBeEmpty();
            newConfigFileContent.ShouldBeEmpty();
        }

        [Fact]
        public void Should_pass_for_config_with_unknown_section()
        {
            const string xmlRules = @"
<Rules xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:noNamespaceSchemaLocation='SolutionCop.xsd'>
  <NonExistingId />
  <Dummy />
</Rules>
";
            var newConfigFileContent = string.Empty;
            Action<string, byte[]> saveConfigFileAction = (s, bytes) =>
            {
                newConfigFileContent = Encoding.UTF8.GetString(bytes);
            };
            var instance = CreateInstance(saveConfigFileAction);
            var errors = new List<string>();
            instance.ParseConfigString("SolutionCop.xml", xmlRules, new IProjectRule[] { new DummyRule() }, errors, false);
            errors.ShouldBeEmpty();
            newConfigFileContent.ShouldBeEmpty();
        }

        [Fact]
        public void Should_fail_for_config_with_wrong_root_element()
        {
            const string xmlRules = @"
<SomeRoot>
  <Dummy />
</SomeRoot>
";
            var newConfigFileContent = string.Empty;
            Action<string, byte[]> saveConfigFileAction = (s, bytes) =>
            {
                newConfigFileContent = Encoding.UTF8.GetString(bytes);
            };
            var instance = CreateInstance(saveConfigFileAction);
            var errors = new List<string>();
            instance.ParseConfigString("SolutionCop.xml", xmlRules, new IProjectRule[] { new DummyRule() }, errors, false);
            errors.ShouldNotBeEmpty();
            newConfigFileContent.ShouldBeEmpty();
        }

        [Fact]
        public void Should_fail_for_malformed_xml()
        {
            const string xmlRules = @"
<Rules>
  <BadElement>
</Rules>
";
            var newConfigFileContent = string.Empty;
            Action<string, byte[]> saveConfigFileAction = (s, bytes) =>
            {
                newConfigFileContent = Encoding.UTF8.GetString(bytes);
            };
            var instance = CreateInstance(saveConfigFileAction);
            var errors = new List<string>();
            instance.ParseConfigString("SolutionCop.xml", xmlRules, new IProjectRule[] { new DummyRule() }, errors, false);
            errors.ShouldNotBeEmpty();
            newConfigFileContent.ShouldBeEmpty();
        }

        private ConfigurationFileParser CreateInstance(Action<string, byte[]> saveConfigFileAction)
        {
            return new ConfigurationFileParser(saveConfigFileAction, new DefaultSolutionCopConsole());
        }

        private class DummyRule : IProjectRule
        {
            public string Id
            {
                get { return "Dummy"; }
            }

            public XElement DefaultConfig
            {
                get { return new XElement(Id); }
            }

            public ValidationResult ValidateAllProjects(XElement xmlRuleConfigs, params string[] projectFilePaths)
            {
                throw new NotImplementedException();
            }
        }
    }
}