using System;

namespace Knapsack_solution {
    public class Utils {
        private static int chromossomeSize;
        
        private static float mutationProbability;
        
        private static Random Randomizer = new Random();

        public Utils(int chromossomeSize, float mutationProbability, Random Randomizer) {
            Utils.chromossomeSize = chromossomeSize;
            Utils.mutationProbability = mutationProbability;
            Utils.Randomizer = Randomizer;
        }
        
        
        public static bool isValidSolution(Chromossome c) {
            Backpack bp = Chromossome.decodeChromossome(c);
            return !(bp.currentWeight > bp.capacity);
        }
        
        public static Chromossome createNewBeing() {
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

            c = new Chromossome(Randomizer, encodedChromossome);
            return c;
        }
        
        public static float fit(Chromossome c) {
            float bpCurrentValue;
            Backpack bp = Chromossome.decodeChromossome(c);
            
            if (isValidSolution(c)) {
                bpCurrentValue = bp.currentValue;
                return bpCurrentValue;
            }
            
            //Console.WriteLine($"Invalid Solution !");
            return 0.01f;
        }
        
        
        public Chromossome[] breed(Chromossome father, Chromossome mother, int slicingIndex = 10, float mutationProbability = 0.2f) {
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

            son1 = new Chromossome(Randomizer, encodedSon1Chromossome);
            son2 = new Chromossome(Randomizer, encodedSon2Chromossome);
            
            son1.mutate(mutationProbability);
            son2.mutate(mutationProbability);

            array[0] = son1;
            array[1] = son2;
            
            return array;
        }
        
        public static Chromossome[] breed(Chromossome father, Chromossome mother, float mutationProbability = 0.2f) {
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
            
            son1 = new Chromossome(Randomizer, encodedSon1Chromossome);
            son2 = new Chromossome(Randomizer, encodedSon2Chromossome);
            
            son1.mutate(mutationProbability);
            son2.mutate(mutationProbability);
            
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