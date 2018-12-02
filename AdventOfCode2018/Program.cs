﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2018
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var problems = new IProblem[] { new Day1.Problem(), new Day2.Problem() };

            foreach (var problem in problems)
            {
                var day = problem.GetType().Namespace.Split(".", StringSplitOptions.RemoveEmptyEntries).Last();

                (var part1Time, var part1Result) = RunPart(File.ReadAllLines($"{day}\\input.txt"), problem.Part1);
                (var part2Time, var part2Result) = RunPart(File.ReadAllLines($"{day}\\input.txt"), problem.Part2);

                Console.WriteLine($"{day}.1 - {part1Result} in {part1Time / 1000.0, 3} sec");
                Console.WriteLine($"{day}.2 - {part2Result} in {part1Time / 1000.0, 3} sec");
            }
        }

        private static (long timeInMsec, string result) RunPart(string[] input, Func<string[], string> part)
        {
            var partWatch = Stopwatch.StartNew();
            var partResult = part(input);
            var partTime = partWatch.ElapsedMilliseconds;
            partWatch.Stop();

            return (partTime, partResult);
        }
    }
}
