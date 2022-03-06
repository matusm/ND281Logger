using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using At.Matus.StatisticPod;
using Bev.Instruments.ND281;

namespace ND281Logger
{
    class Program
    {
        private static StreamWriter csvFile;

        private static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;

            Options options = new Options();
            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
                Console.WriteLine("*** ParseArgumentsStrict returned false");

            StatisticPod stp = new StatisticPod("length reading");
            ND281 device = new ND281(options.Port);

            Console.WriteLine($"Application:  {appName} {appVersion}");
            Console.WriteLine($"StartTimeUTC: {DateTime.UtcNow:dd-MM-yyyy HH:mm}");
            Console.WriteLine($"InstrumentID: {device.InstrumentID}");
            Console.WriteLine($"Samples (n):  {options.MaximumSamples}");
            OpenNewCsvFile(); // this prompts a message
            Console.WriteLine();

            int measurementIndex = 0;
            bool shallLoop = true;
            while (shallLoop)
            {
                Console.WriteLine("press any key to start a measurement, 's' to start a new file, 'q' to quit");
                ConsoleKeyInfo cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.Q:
                        shallLoop = false;
                        break;
                    case ConsoleKey.S:
                        OpenNewCsvFile();
                        measurementIndex = 0; // new numbering for new file (or better not?)
                        break;
                    default:
                        int iterationIndex = 0;
                        measurementIndex++;
                        Console.WriteLine();
                        Console.WriteLine($"Measurement #{measurementIndex}");
                        stp.Restart();
                        while (iterationIndex < options.MaximumSamples)
                        {
                            iterationIndex++;
                            double value = device.GetValue();
                            Console.WriteLine($"***debug*** [{device.LastResponse}]"); // TODO remove when working
                            stp.Update(value);
                            Console.WriteLine($"{iterationIndex,4}:  {value:F5} mm");
                        }
                        Console.WriteLine($"Result: {stp.AverageValue:F6} mm ± {stp.StandardDeviation * 1e3:F3} µm");
                        Console.WriteLine();
                        csvFile.WriteLine($"{measurementIndex},{stp.AverageValue:F6},{stp.StandardDeviation * 1e3:F3},{stp.Range * 1e3:F3}");
                        csvFile.Flush();
                        break;
                }
            }

            csvFile.Close();
            Console.WriteLine("bye.");

            /***************************************************/
            string GenerateCsvFileName() => $"{options.FilePrefix}{DateTime.UtcNow:yyyyMMddHHmm}.csv";
            /***************************************************/
            string OpenNewCsvFile()
            {
                if (csvFile != null)
                    csvFile.Close();
                string fileName = GenerateCsvFileName();
                csvFile = new StreamWriter(fileName, false);
                csvFile.WriteLine("index,length/mm,standard deviation/µm,range/µm");
                csvFile.Flush();
                Console.WriteLine($"Result file:  {fileName}");
                return fileName;
            }
            /***************************************************/
        }
    }
}
