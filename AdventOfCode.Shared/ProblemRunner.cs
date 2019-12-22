﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode.Shared
{
    public class ProblemRunner
    {
        private readonly IEnumerable<IProblem> problems;

        public ProblemRunner(IEnumerable<IProblem> problems)
        {
            this.problems = problems;
        }

        public void Run()
        {
            var totalTime = new Stopwatch();

            foreach (var problem in this.problems)
            {
                var day = problem.GetType().Namespace.Split(".", StringSplitOptions.RemoveEmptyEntries).Last();
                var input = File.ReadAllLines($"{day}\\input.txt");

                totalTime.Start();
                (var part1Time, var part1Result) = RunPart(input, problem.Part1);
                totalTime.Stop();
                Console.WriteLine($"{day}.1 - {part1Result} in {part1Time / 1000.0,3} sec | Total time: {totalTime.ElapsedMilliseconds / 1000.0,3} sec");

                totalTime.Start();
                (var part2Time, var part2Result) = RunPart(input, problem.Part2);
                totalTime.Stop();
                Console.WriteLine($"{day}.2 - {part2Result} in {part2Time / 1000.0,3} sec | Total time: {totalTime.ElapsedMilliseconds / 1000.0,3} sec");
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
