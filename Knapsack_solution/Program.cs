using System;
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
            
            DelegateToPython(GS.getStatesFitnessInArrayForm());
            solution.openBackpack();
        }

        static void DelegateToPython(float []AllStatesFitnessScore) {
            string jsonString = JsonSerializer.Serialize(AllStatesFitnessScore);
            
            ProcessStartInfo psi = new ProcessStartInfo();
            
            psi.FileName = @"/usr/bin/python3";
            string script = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "graphicalPlotter.py"));

            psi.Arguments = $"\"{script}\" \"{jsonString}\"";
            
            psi.CreateNoWindow = false;
            psi.UseShellExecute = true;
            using (Process process = Process.Start(psi)) {
                process.WaitForExit();
            }
            
        }
        
    }
}
