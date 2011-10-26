using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Layouts;
using NLog;

namespace BouilholLib.Helper
{
    public class ConfigLogger
    {
        public const string Separator = "--------------------------------------------------------------------------------";

        public ConfigLogger(string logPath, string filenameInfo, string filenameError)
        {
            Init(logPath, filenameInfo, filenameError);
        }

        public void Init(string logPath, string filenameInfo, string filenameError)
        {
            NLog.LogManager.Configuration = BuildConfiguration(logPath, filenameInfo, filenameError);
        }

        private LoggingConfiguration BuildConfiguration(string logPath, string filenameInfo, string filenameError)
        {
            LoggingConfiguration config = new LoggingConfiguration();

            Target infoFileTarget = BuildInfoFileTarget(logPath, filenameInfo);
            config.AddTarget("infoFileTarget", infoFileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, infoFileTarget));

            Target instantFileTarget = BuildInstantFileTarget(logPath, filenameError);
            config.AddTarget("instantFileTarget", instantFileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, instantFileTarget));

            return config;
        }

        private Target BuildInfoFileTarget(string logPath, string filename)
        {
            FileTarget target = new FileTarget();

            target.FileName = string.Concat(logPath, filename);
            target.AutoFlush = false;
            target.Layout = BuildLayout();
            target.ArchiveFileName = string.Concat(logPath, @"archives\log.{#####}.", filename);
            target.ArchiveEvery = FileArchivePeriod.Month;
            target.ArchiveNumbering = ArchiveNumberingMode.Sequence;

            BufferingTargetWrapper wrapper = new BufferingTargetWrapper(target);

            return wrapper;
        }

        private Target BuildInstantFileTarget(string logPath, string filename)
        {
            FileTarget fileTarget = new FileTarget();

            fileTarget.FileName = string.Concat(logPath, filename);
            fileTarget.AutoFlush = true;
            fileTarget.Layout = BuildLayout();

            return fileTarget;
        }

        private LayoutWithHeaderAndFooter BuildLayout()
        {
            LayoutWithHeaderAndFooter layout = new LayoutWithHeaderAndFooter();

            layout.Header = string.Concat(Separator, "${newline}${date:format=dddd dd MMMM yyyy}${newline}${date:format=T}${newline}Login: ${windows-identity}${newline}Machine: ${machinename}${newline}Version: ${gdc:item=version}${newline}", Separator);
            layout.Layout = "${message}";
            layout.Footer = string.Concat(Separator, "${newline}${date:format=T} - ${qpc}s${newline}", Separator);

            return layout;
        }
    }
}
