using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities.Extensions;

namespace Utilities.Helpers
{
    [Export]
    public class CommandLineParser
    {
        private static readonly Lazy<CommandLineParser> InstanceCache =
            new Lazy<CommandLineParser>(() => new CommandLineParser());

        [ImportingConstructor]
        public CommandLineParser() : this(Environment.CommandLine)
        {
        }

        public CommandLineParser(string commandLine)
        {
            CommandLine = commandLine;
            Parse(commandLine);
        }

        public string ApplicationName { get; private set; }

        public string CommandLine { get; set; }

        public IList<string> Parameters { get; private set; }

        public IDictionary<string, string> Switches { get; private set; }

        public string this[string name]
        {
            get { return Switches.TryGetValue(name); }
        }

        public static CommandLineParser Create()
        {
            return InstanceCache.Value;
        }

        protected void Parse(string commandLine)
        {
            ApplicationName = ParseApplicationName(commandLine);
            Switches = ParseSwitches(commandLine);
            Parameters = ParseParameters(commandLine).ToList();
        }

        protected virtual string ParseApplicationName(string commandLine)
        {
            var regex = new Regex(Constants.ApplicationNamePatternConstant, RegexOptions.IgnorePatternWhitespace);
            Match match = regex.Match(CommandLine);

            return match.Success ? match.Groups["value"].Value : null;
        }

        protected virtual IEnumerable<string> ParseParameters(string commandLine)
        {
            var regex = new Regex(Constants.ParameterPatternConstant, RegexOptions.IgnorePatternWhitespace);
            MatchCollection matchCollection = regex.Matches(commandLine);

            return matchCollection.Cast<Match>().Select(match => match.Groups["param"].Value);
        }

        protected virtual IDictionary<string, string> ParseSwitches(string commandLine)
        {
            var regex = new Regex(Constants.NonNamedSwitchPatternConstant, RegexOptions.IgnorePatternWhitespace);
            MatchCollection matchCollection = regex.Matches(commandLine);

            return matchCollection.Cast<Match>()
                .ToDictionary(match => match.Groups["key"].Value, match => match.Groups["value"].Value);
        }

        public static class Constants
        {
            public const string ApplicationNamePatternConstant = StringArgumentPatternConstant;
            public const string BooleanArgumentPatternConstant = @"(?<value>(\+|-){0,1})";
            public const string IntegerArgumentPatternConstant = @"(?<value>(-/\+)?[0-9]+)";

            public const string NonNamedSwitchPatternConstant =
                SwitchKeyValuePrefixPatternConstant + "(?<switch>" + SwitchKeyPrefixCharacterPatternPrefix +
                "(?<key>\\w+)" + SwitchKeyValueDividerPatternConstant + StringArgumentPatternConstant + ")" +
                SwitchKeyValueSuffixPatternConstant;

            public const string NumberArgumentPatternConstant = @"(?<value>(-/\+)?([0-9]+)(\.[0-9]+))?";
            public const string ParameterPatternConstant = @"((\s*(""(?<param>.+?)""|(?<param>\S+))))";

            public const string StringArgumentPatternConstant =
                "(((?<quote>[\"'])(?<value>(?:\\\\\\k<quote>|.)*?)\\k<quote>)  |  ((?<value>\\S+)))";

            public const string SwitchKeyPrefixCharacterPatternPrefix = "(?<switchPrefix>(?:/|-{1,2}))";
            public const string SwitchKeyValueDividerPatternConstant = @"(?:[:=]|\s+)";
            public const string SwitchKeyValuePrefixPatternConstant = @"(\s+|^)";
            public const string SwitchKeyValueSuffixPatternConstant = @"(?=(\s|$))";
        }
    }
}