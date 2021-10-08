using System;
using System.Diagnostics;
using System.Text.Json;

namespace Knapsack_solution
{
    class Program
    {
        GeneticSolver GS = new GeneticSolver();
        static void Main(string[] args) {
            GeneticSolver GS = new GeneticSolver();
            Backpack solution = GS.Solve(20);
            
            DelegateToPython(GS.getStatesFitnessInArrayForm());
            solution.openBackpack();
        }

        static void DelegateToPython(float []AllStatesFitnessScore) {
            string jsonString = JsonSerializer.Serialize(AllStatesFitnessScore);
            //Console.WriteLine(jsonString);
            ProcessStartInfo psi = new ProcessStartInfo();
            
            psi.FileName = @"/usr/bin/python3";
            string script =
                @"/home/ariel/Desktop/8o_periodo/AI/Genetic_programming/Genetic_algorithm/Knapsack_problem/Knapsack_solution/graphicalPlotter.py";

            psi.Arguments = $"\"{script}\" \"{jsonString}\"";
            
            psi.CreateNoWindow = false;
            psi.UseShellExecute = true;
            using (Process process = Process.Start(psi)) {
                //process.WaitForExit();
            }
            
        }
        
    }
}
