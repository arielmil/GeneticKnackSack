using System;

namespace Knapsack_solution {

    public class Chromossome {
        public bool[] encodedChromossome { get; private set; }
        private int genesQnt;
        public Chromossome(Backpack bp) {
            genesQnt = bp.itemsQnt;
            encodedChromossome = new bool[genesQnt];
            encodeChromossome(bp);
        }
        
        public Chromossome(int [] intToBoolArray) {
            genesQnt = intToBoolArray.Length;
            
            int j = 0;
            encodedChromossome = new bool[genesQnt];
            
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
            this.encodedChromossome = encodedChromossome;
            genesQnt = encodedChromossome.Length;
        }

        private void encodeChromossome(Backpack bp) {
            int i = 0;
            Item[] backPackItems = bp.Items;

            foreach (Item item in backPackItems) {
                encodedChromossome[i] = bp.IsItemInBackpack(item);
                i++;
            }
        }

        public static Backpack decodeChromossome(Chromossome c, int genesQnt) {
            int i;
            string name;

            Backpack bp = new Backpack(genesQnt);

            for (i = 0; i < genesQnt; i++) {

                if (c.encodedChromossome[i]) {
                    name = Backpack.getNameInPosition(i, genesQnt);
                    bp.pack(name);
                }
            }
            
            return bp;
        }

        public void changeGene(int geneNumber) {
            if (geneNumber >= 0 && geneNumber <= genesQnt) {
                encodedChromossome[geneNumber] = !encodedChromossome[geneNumber];
            }

            else {
                throw (new Exception("Error: Invalid chromossome number !"));
            }
        }

    }
    
}