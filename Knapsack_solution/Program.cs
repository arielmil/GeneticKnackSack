using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Knapsack_solution {
    class Program {
        static private float mutationProbability = 0.02f;
        static private int chromossomeSize = 37;

        static private Random Randomizer = new Random();

        static void Main(string[] args) {
            Utils utils = new Utils(chromossomeSize, mutationProbability, Randomizer);
            GeneticSolver GS = new GeneticSolver(Utils.fit, Utils.isValidSolution, Utils.breed, Utils.createNewBeing, mutationProbability, Randomizer, maxAllowedGeneration: 110);
            Backpack solution = Chromossome.decodeChromossome(GS.Solve(20));

            solution.openBackpack();
            plotInPython(GS.getStatesFitnessInArrayForm());

            //testSomethingQuick();
        }

        static void plotInPython(float[] AllStatesFitnessScore) {
            string jsonString = JsonSerializer.Serialize(AllStatesFitnessScore);
            
            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = @"/usr/bin/python3";
            string scriptPath = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "graphicalPlotter.py"));

            psi.Arguments = $"\"{scriptPath}\" \"{jsonString}\"";

            psi.CreateNoWindow = false;
            psi.UseShellExecute = false;

            using (Process process = Process.Start(psi)) {
                if (process != null) {
                    process.WaitForExit();
                }

                else {
                    throw (new Exception("Process is null !"));
                }
            }

        }

        static void exportItems() {
            string[] items = Backpack.exportItemsInStringArray(chromossomeSize);

            string scriptPath = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "Items.csv"));
            File.WriteAllLines(scriptPath, items);
        }

        static private void testSomethingQuick() {
            Chromossome c = new Chromossome(Randomizer, new int[] {0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0});
            Chromossome.decodeChromossome(c).openBackpack();
        }
    }
}
