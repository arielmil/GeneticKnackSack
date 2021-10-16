using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Knapsack_solution
{
    class Program {
        static private float mutationProbability;
        static private int chromossomeSize = 37;
        
        static private Random Randomizer = new Random();
        static void Main(string[] args) {
            GeneticSolver GS = new GeneticSolver(fit, isValidSolution, breed, createNewBeing, Randomizer, maxAllowedGeneration:110);
            Backpack solution = Chromossome.decodeChromossome(GS.Solve(20));
            
            solution.openBackpack();
            plotInPython(GS.getStatesFitnessInArrayForm());
            
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
            string[] items = Backpack.exportItemsInStringArray(chromossomeSize);
            
            string scriptPath = Path.GetFullPath(Path.Combine(".", "..", "..", "..", "..", "Items.csv"));
            File.WriteAllLines(scriptPath, items);
        }
        static private void testSomethingQuick() {
            Chromossome c = new Chromossome(Randomizer, new int[] {0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0}, mutationProbability);
            (Chromossome.decodeChromossome(c)).openBackpack();
        }
        
        private static bool isValidSolution(Chromossome c) {
            Backpack bp = Chromossome.decodeChromossome(c);
            return !(bp.currentWeight > bp.capacity);
        }
        
        private static Chromossome createNewBeing() {
            float itemAcceptanceProbability = 0.5f;
            int i;
            
            float drawnFloat;
            
            bool[] encodedChromossome = new bool[chromossomeSize];
            
            Chromossome c;
            
            for (i = 0; i < chromossomeSize; i++) {
                drawnFloat = (float)Randomizer.NextDouble();
                
                if (itemAcceptanceProbability >= drawnFloat) {
                    encodedChromossome[i] = true;
                }
                else {
                    encodedChromossome[i] = false;
                }
                
            }

            c = new Chromossome(Randomizer, encodedChromossome, mutationProbability);
            return c;
        }
        
        private static float fit(Chromossome c) {
            float bpCurrentValue;
            Backpack bp = Chromossome.decodeChromossome(c);
            
            if (isValidSolution(c)) {
                bpCurrentValue = bp.currentValue;
                return bpCurrentValue;
            }
            
            //Console.WriteLine($"Invalid Solution !");
            return 0.01f;
        }
        
        
        private Chromossome[] breed(Chromossome father, Chromossome mother, int slicingIndex = 10, float mutationProbability = 0.2f) {
            int i;

            Chromossome[] array = new Chromossome[2];
            Chromossome son1;
            Chromossome son2;

            bool[] encodedFatherChromossome;
            bool[] encodedMotherChromossome;

            bool[] encodedSon1Chromossome = new bool[chromossomeSize];
            bool[] encodedSon2Chromossome = new bool[chromossomeSize];
            
            encodedFatherChromossome = father.encodedChromossome;
            encodedMotherChromossome = mother.encodedChromossome;

            for (i = 0; i < slicingIndex / 2; i++) {
                encodedSon1Chromossome[i] = encodedFatherChromossome[i];
                encodedSon2Chromossome[i] = encodedMotherChromossome[i];
            }

            for (; i < slicingIndex; i++) {
                encodedSon1Chromossome[i] = encodedMotherChromossome[i];
                encodedSon2Chromossome[i] = encodedFatherChromossome[i];
            }

            son1 = new Chromossome(Randomizer, encodedSon1Chromossome, mutationProbability);
            son2 = new Chromossome(Randomizer, encodedSon2Chromossome, mutationProbability);
            
            son1.mutate();
            son2.mutate();

            array[0] = son1;
            array[1] = son2;
            
            return array;
        }
        
        private static Chromossome[] breed(Chromossome father, Chromossome mother, float mutationProbability = 0.2f) {
            int i;
            
            Chromossome[] array = new Chromossome[2];
            Chromossome son1;
            Chromossome son2;

            bool[] mask;
            bool[] encodedFatherChromossome;
            bool[] encodedMotherChromossome;

            bool[] encodedSon1Chromossome = new bool[chromossomeSize];
            bool[] encodedSon2Chromossome = new bool[chromossomeSize];

            mask = generateMask(chromossomeSize, 0.5f);
            
            encodedFatherChromossome = father.encodedChromossome;
            encodedMotherChromossome = mother.encodedChromossome;
            
            
            for (i = 0; i < chromossomeSize; i++) {
                if (mask[i]) {
                    encodedSon1Chromossome[i] = encodedFatherChromossome[i];
                    encodedSon2Chromossome[i] = encodedMotherChromossome[i];
                }

                else {
                    encodedSon1Chromossome[i] = encodedMotherChromossome[i];
                    encodedSon2Chromossome[i] = encodedFatherChromossome[i];
                }
            }
            
            son1 = new Chromossome(Randomizer, encodedSon1Chromossome, mutationProbability);
            son2 = new Chromossome(Randomizer, encodedSon2Chromossome, mutationProbability);
            
            son1.mutate();
            son2.mutate();
            
            array[0] = son1;
            array[1] = son2;

            return array;
        }

        private static bool[] generateMask(int maskSize, float trueProbability) {
            int i;
            
            float randomValue;
            
            bool[] mask = new bool[maskSize];
            
            
            for (i = 0; i < maskSize; i++) {
                randomValue = (float)Randomizer.NextDouble();
                
                if (randomValue <= trueProbability) {
                    mask[i] = true;
                }
                else {
                    mask[i] = false;
                }
                
            }

            return mask;
        }
        
    }
    
    
}
