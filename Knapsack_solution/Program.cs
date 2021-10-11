﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Knapsack_solution
{
    class Program
    {
        GeneticSolver GS = new GeneticSolver();
        static void Main(string[] args) {
            GeneticSolver GS = new GeneticSolver();
            Backpack solution = GS.Solve(20);
            
            plotInPython(GS.getStatesFitnessInArrayForm());
            solution.openBackpack();
        }

        static void plotInPython(float []AllStatesFitnessScore) {
            string jsonString = JsonSerializer.Serialize(AllStatesFitnessScore);
            
            ProcessStartInfo psi = new ProcessStartInfo();
            
            psi.FileName = @"/usr/bin/python3";
            string scriptPath = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "graphicalPlotter.py"));

            psi.Arguments = $"\"{scriptPath}\" \"{jsonString}\"";
            
            psi.CreateNoWindow = false;
            psi.UseShellExecute = false;
            
            using (Process process = Process.Start(psi)) {
                process.WaitForExit();
            }
            
        }

        static void exportItems() {
            string[] items = Backpack.exportItemsInStringArray(22);
            
            string scriptPath = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "Items.csv"));
            File.WriteAllLines(scriptPath, items);
        }
        
    }
    
}
