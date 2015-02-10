﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using SolutionCop.Core;

namespace SolutionCop.CommandLine
{
    internal static class Program
    {
        private static readonly RulesDirectoryCatalog RulesDirectoryCatalog = new RulesDirectoryCatalog();
        private static readonly ConfigurationFileParser ConfigurationFileParser = new ConfigurationFileParser();

        private static void Main(string[] args)
        {
            var commandLineParameters = new CommandLineParameters();
            if (Parser.Default.ParseArguments(args, commandLineParameters))
            {
                var pathToConfigFile = commandLineParameters.PathToConfigFile;

                var errors = new List<string>();
                var validationResults = new List<ValidationResult>();
                var solutionInfo = SolutionParser.LoadFromFile(commandLineParameters.PathToSolution);

                if (!solutionInfo.IsParsed)
                {
                    Console.Out.WriteLine("FATAL: Cannot parse solution file.");
                    Environment.Exit(-1);
                }

                var rules = RulesDirectoryCatalog.LoadRules();
                var ruleConfigsMap = ConfigurationFileParser.Parse(commandLineParameters.PathToSolution, ref pathToConfigFile, rules, errors);

                Console.Out.WriteLine("INFO: Starting analysis...");
                foreach (var rule in rules)
                {
                    Console.Out.WriteLine("INFO: Analyzing projects using rule {0}", rule.Id);
                    var validationResult = rule.ValidateAllProjects(ruleConfigsMap[rule.Id], solutionInfo.ProjectFilePaths.ToArray());
                    errors.AddRange(validationResult.Errors);
                    validationResults.Add(validationResult);
                }
                Console.Out.WriteLine("INFO: Analysis finished!");

                if (errors.Any())
                {
                    Console.Out.WriteLine("ERROR: ***** Full list of errors: *****");
                    errors.ForEach(x => Console.Out.WriteLine("ERROR: {0}", x));
                    if (commandLineParameters.BuildServerType == BuildServer.TeamCity)
                    {
                        // adding empty line for a better formatting in TC output
                        var extendedErrorsInfo = Enumerable.Repeat("", 1)
                            .Concat(errors.Select((x, idx) => string.Format("ERROR ({0}/{1}): {2}", idx + 1, errors.Count, x)))
                            .Concat(Enumerable.Repeat("", 1))
                            .Concat(validationResults.Select(x => string.Format("INFO: Rule {0} is {1}", x.RuleId, x.IsEnabled ? "enabled" : "disabled")));
                        Console.Out.WriteLine("##teamcity[testFailed name='SolutionCop' message='FAILED - {0}' details='{1}']", EscapeForTeamCity(Path.GetFileName(pathToConfigFile)), string.Join("|r|n", extendedErrorsInfo.Select(EscapeForTeamCity)));
                        Console.Out.WriteLine("##teamcity[buildStatus text='FAILED - {0}']", EscapeForTeamCity(Path.GetFileName(pathToConfigFile)));
                    }
                }
                else
                {
                    Console.Out.WriteLine("INFO: No errors found!");
                    if (commandLineParameters.BuildServerType == BuildServer.TeamCity)
                    {
                        // TODO
                        Console.Out.WriteLine("##teamcity[buildStatus status='SUCCESS' text='PASSED - {0}']", EscapeForTeamCity(Path.GetFileName(pathToConfigFile)));
                    }
                }
                Environment.Exit(errors.Any() ? -1 : 0);
            }
            else
            {
                Environment.Exit(-1);
            }
        }

        private static string EscapeForTeamCity(string originalString)
        {
            return originalString.Replace("|", "||").Replace("'", "|'").Replace("\r", "|r").Replace("\n", "|n").Replace("]", "|]");
        }
    }
}