using CommandLine;
using CommandLine.Text;

namespace ND281Logger
{
    class Options
    {
        [Option('p', "port", DefaultValue = "COM1", HelpText = "Serial port name.")]
        public string Port { get; set; }

        [Option('n', "number", DefaultValue = 10, HelpText = "Number of samples.")]
        public int MaximumSamples { get; set; }

        [Option("prefix", DefaultValue = "", HelpText = "Prefix for csv files.")]
        public string FilePrefix { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string AppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            HelpText help = new HelpText
            {
                Heading = new HeadingInfo($"{AppName}, version {AppVer}"),
                Copyright = new CopyrightInfo("Michael Matus", 2022),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            string preamble = "Program to operate a incremental probe by Heidenhain. " +
                "Measurement results are logged in a csv file.";
            help.AddPreOptionsLine(preamble);
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine($"Usage: {AppName} [options]");
            help.AddPostOptionsLine("");
            help.AddOptions(this);

            return help;
        }

    }
}
