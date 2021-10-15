using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Knapsack_solution
{
    class Program
    {
        static void Main(string[] args) {
            GeneticSolver GS = new GeneticSolver(37, maxAllowedGeneration:110);
            Backpack solution = GS.Solve(5);
            
            plotInPython(GS.getStatesFitnessInArrayForm());
            solution.openBackpack();
            
            //testSomethingQuick();
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
            string[] items = Backpack.exportItemsInStringArray(37);
            
            string scriptPath = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "Items.csv"));
            File.WriteAllLines(scriptPath, items);
        }
        private static void testSomethingQuick() {
            Chromossome c = new Chromossome(new int[] {0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0});
            (Chromossome.decodeChromossome(c, 37)).openBackpack();
        }
        
    }
    
    
}
