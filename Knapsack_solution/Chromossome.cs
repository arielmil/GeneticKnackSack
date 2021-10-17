using System;

namespace Knapsack_solution {

    public class Chromossome {

        private int chromossomeSize;
        public bool[] encodedChromossome { get; private set; }
        
        private Random Randomizer;

        private Backpack bp;
        
        public Chromossome(Random Randomizer, Backpack bp, int chromossomeSize) {
            this.chromossomeSize = chromossomeSize;
            this.Randomizer = Randomizer;
            
            encodedChromossome = new bool[chromossomeSize];
            encodeChromossome(bp);
        }
        
        public Chromossome(Random Randomizer, int [] intToBoolArray) {
            int j = 0;
            chromossomeSize = intToBoolArray.Length;
            encodedChromossome = new bool[chromossomeSize];
            
            foreach (int i in intToBoolArray) {
                if (i == 0) {
                    encodedChromossome[j] = false;
                }
                else {
                    encodedChromossome[j] = true;
                }

                j++;
            }
            
            this.Randomizer = Randomizer;
        }

        public Chromossome(Random Randomizer, bool[] encodedChromossome) {
            this.encodedChromossome = encodedChromossome;
            this.Randomizer = Randomizer;
            
            chromossomeSize = encodedChromossome.Length;
        }

        private void encodeChromossome(Backpack bp) {
            int i = 0;
            Item[] backPackItems = bp.Items;

            foreach (Item item in backPackItems) {
                encodedChromossome[i] = bp.IsItemInBackpack(item);
                i++;
            }
        }

        public static Backpack decodeChromossome(Chromossome c) {
            int i;
            string name;

            Backpack bp = new Backpack(c.chromossomeSize);

            for (i = 0; i < c.chromossomeSize; i++) {

                if (c.encodedChromossome[i]) {
                    name = Backpack.getNameInPosition(i, c.chromossomeSize);
                    bp.pack(name);
                }
            }
            
            return bp;
        }
        
        public void mutate(float mutationProbability) {
            float drawnFloat = (float)Randomizer.NextDouble();
            int drawnInt = Randomizer.Next(0, chromossomeSize);

            if (mutationProbability >= drawnFloat) {
                changeGene(drawnInt);
            }
        }
        
        private void changeGene(int geneNumber) {
            if (geneNumber >= 0 && geneNumber <= chromossomeSize) {
                encodedChromossome[geneNumber] = !encodedChromossome[geneNumber];
            }

            else {
                throw (new Exception("Error: Invalid chromossome number !"));
            }
        }

    }
    
}