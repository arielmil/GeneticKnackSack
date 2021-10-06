using System;

namespace Knapsack_solution {

    public class Chromossome {
        public bool[] encodedChromossome { get; private set; }
        
        public Chromossome(Backpack bp) {
            encodedChromossome = new bool[22];
            encodeChromossome(bp);
        }

        public Chromossome(bool[] encodedChromossome) {
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

            for (i = 0; i < 22; i++) {

                if (c.encodedChromossome[i]) {
                    name = Backpack.getNameInPosition(i);
                    bp.pack(name);
                }
            }

            return bp;
        }

        public void changeGene(int geneNumber) {
            if (geneNumber >= 0 && geneNumber <= 22) {
                encodedChromossome[geneNumber] = !encodedChromossome[geneNumber];
            }

            else {
                throw (new Exception("Error: Invalid chromossome number !"));
            }
        }

    }
    
}