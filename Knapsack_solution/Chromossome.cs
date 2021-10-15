using System;

namespace Knapsack_solution {

    public class Chromossome {

        private int chromossomeSize;
        public bool[] encodedChromossome { get; private set; }
        
        public Chromossome(Backpack bp, int chromossomeSize) {
            this.chromossomeSize = chromossomeSize;
            encodedChromossome = new bool[chromossomeSize];
            encodeChromossome(bp);
        }
        
        public Chromossome(int [] intToBoolArray) {
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
        }

        public Chromossome(bool[] encodedChromossome) {
            chromossomeSize = encodedChromossome.Length;
            this.encodedChromossome = encodedChromossome;
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

            Backpack bp = new Backpack();

            for (i = 0; i < c.chromossomeSize; i++) {

                if (c.encodedChromossome[i]) {
                    name = Backpack.getNameInPosition(i);
                    bp.pack(name);
                }
            }
            
            return bp;
        }

        public void changeGene(int geneNumber) {
            if (geneNumber >= 0 && geneNumber <= chromossomeSize) {
                encodedChromossome[geneNumber] = !encodedChromossome[geneNumber];
            }

            else {
                throw (new Exception("Error: Invalid chromossome number !"));
            }
        }

    }
    
}