using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Labs;
using System.Collections.Generic;

[Command(Name = "lab-runner", Description = "A utility for running different lab programs.")]
[Subcommand(typeof(VersionCommand), typeof(RunLabCommand), typeof(SetPathCommand))]
class LabRunnerApp
{
    static int Main(string[] args)
    {
        while (true)
        {
            try
            {
                args = new[] { "run", "lab1", "-I", @"D:\fit\cross-platform\cross-platform-shpytchuk\lab-1\INPUT.txt", 
                    "-o", @"D:\fit\cross-platform\cross-platform-shpytchuk\lab-1\OUTPUT.txt" };
                return CommandLineApplication.Execute<LabRunnerApp>(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Use --help");
                return 1;
            }
        }

    }
}

[Command("version", Description = "Displays author and version")]
public class VersionCommand
{
    private void OnExecute()
    {
        Console.WriteLine("Author: Ivan Shpytchuk");
        Console.WriteLine("Version: 1.0");
    }
}

[Command("run", Description = "Run the labs")]
public class RunLabCommand
{
    [Argument(0, Description = "Name of the lab - lab1, lab2, lab3)")]
    public string Lab { get; set; }

    [Option("-I|--input <INPUT>", Description = "Path to input file")]
    public string Input { get; set; }

    [Option("-o|--output <OUTPUT>", Description = "Path to output file")]
    public string Output { get; set; }

    private static readonly Dictionary<string, Action<string, string>> LabRunners
        = new Dictionary<string, Action<string, string>>
        {
            { "lab1", FirstLab.Run },
            { "lab2", SecondLab.Run },
            { "lab3", ThirdLab.Run }
        };

    private void OnExecute()
    {
        if (LabRunners.ContainsKey(Lab))
        {
            LabRunners[Lab](Input, Output);
            Console.WriteLine($"Running {Lab} with input={Input} and output={Output}");
        }
        else
        {
            Console.Error.WriteLine($"Error: Unknown lab {Lab}.");
        }
    }
}

[Command("set-path", Description = "Sets the path to the folder with input and output files.")]
public class SetPathCommand
{
    [Option("-p|--path <PATH>", CommandOptionType.SingleValue, Description = "Path to the folder with input and output files.")]
    public string Path { get; set; }

    private void OnExecute()
    {
        try
        {
            File.WriteAllText("config.txt", Path);
            Console.WriteLine($"Successfully set LAB_PATH to {Path}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error setting LAB_PATH: {ex.Message}");
        }
    }
}
