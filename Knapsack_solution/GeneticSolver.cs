using System;
using System.Collections.Generic;

namespace Knapsack_solution {
    public class GeneticSolver {
        private List<Chromossome> Population = new List<Chromossome>();
        private Chromossome Best;

        private List <BreedingChance> PopulationBreedingChances = new List<BreedingChance>();
        private Random Randomizer = new Random();
        
        private int currentPopulationSize = 0;
        private int maxAllowedGeneration;

        private float bestScore = 0.0f;

        public GeneticSolver(int maxPopulationSize = 1000, int maxAllowedGeneration = 100) {
            this.maxAllowedGeneration = maxAllowedGeneration;
        }
        
        public Backpack Solve() {
            int i = 0;
            
            generateFirstGeneration();
            
            while (i < maxAllowedGeneration) {
                if (i % 2 == 0) {
                    generateFirstGeneration();
                }
                generateNextGeneration();
                i = i + 2;
            }
            
            Console.WriteLine($"Best solution fitness is: {bestScore}");
            return Chromossome.decodeChromossome(Best);
        }
        
        void generateFirstGeneration() {
            int i;
            for (i = 0; i < maxAllowedGeneration; i++) {
                createNewBeing();
            }
        }
        
        //Roulette like method
        void generateNextGeneration(int maxPopulationSize = 998) {
            int i;
            
            Console.WriteLine($"Best score so far: {bestScore}");
            
            float totalFitness = 0.0f;
            float beingFitness;
            float drawnFloat;

            float bestGenerationScore = 0.0f;
            
            foreach (Chromossome c in Population) {
                beingFitness = fit(c);
                totalFitness = totalFitness + beingFitness;

                if (beingFitness > bestScore) {
                    bestScore = beingFitness;
                    Best = c;
                }

                if (beingFitness > bestGenerationScore) {
                    bestGenerationScore = beingFitness;
                }
                
                PopulationBreedingChances.Add(new BreedingChance(c, beingFitness));
            }
            
            Console.WriteLine($"Best Generation's score: {bestGenerationScore}");

            BreedingChance bc;
            for (i = 0; i < PopulationBreedingChances.Count; i++) {
                bc = PopulationBreedingChances[i];
                
                bc.breedingChance = (bc.fitnessScore / totalFitness);
                drawnFloat = (float) Randomizer.NextDouble();
                
                if (bc.breedingChance >= drawnFloat) {
                    if (currentPopulationSize >= maxPopulationSize) {
                        killLowestsFitnessScoresBeings(500);
                    }
                    breed(Best, bc.chromossome, 10, 0.02f);
                }
                
            }
        }
        
        private static float fit(Chromossome c) {
            Backpack bp = Chromossome.decodeChromossome(c);
            if (isValidSolution(c)) {
                return bp.currentValue;
            }
            
            //Console.WriteLine($"Invalid Solution !");
            return 0.0f;
        }
        
        private static bool isValidSolution(Chromossome c) {
            Backpack bp = Chromossome.decodeChromossome(c);
            return !(bp.currentWeight > bp.capacity);
        }
        
        private void createNewBeing(float probability = 0.5f) {
            int i;
            
            float drawnFloat;
            
            bool[] encodedChromossome = new bool[37];
            
            Chromossome c;
            
            for (i = 0; i < 37; i++) {
                drawnFloat = (float)Randomizer.NextDouble();
                
                if (probability >= drawnFloat) {
                    encodedChromossome[i] = true;
                }
                else {
                    encodedChromossome[i] = false;
                }
                
            }

            c = new Chromossome(encodedChromossome);
            addNewChromossomeToPopulation(c);
        }
        
        private void replaceBeing(Chromossome c1, Chromossome c2) {
            Population.Remove(c1);
            addNewChromossomeToPopulation(c2);
        }
        
        private void mutate(Chromossome c, float probability) {
            float drawnFloat = (float)Randomizer.NextDouble();
            int drawnInt = Randomizer.Next(0, 37);
            
            if (probability >= drawnFloat) {
               c.changeGene(drawnInt);
            }
        }

        private void breed(Chromossome father, Chromossome mother, int slicingIndex = 10, float probability = 0.2f) {
            int i;
            
            Chromossome son1;
            Chromossome son2;

            bool[] encodedFatherChromossome;
            bool[] encodedMotherChromossome;

            bool[] encodedSon1Chromossome = new bool[37];
            bool[] encodedSon2Chromossome = new bool[37];
            
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

            son1 = new Chromossome(encodedSon1Chromossome);
            son2 = new Chromossome(encodedSon2Chromossome);
            
            mutate(son1, probability);
            mutate(son2, probability);
            
            addNewChromossomeToPopulation(son1);
            addNewChromossomeToPopulation(son2);
        }

        void addNewChromossomeToPopulation(Chromossome c) {
            Population.Add(c);
            currentPopulationSize = Population.Count;
        }

        private void kill(Chromossome c) {
            Population.Remove(c);
            currentPopulationSize = Population.Count;
        }
        
        private void killLowestsFitnessScoresBeings(int range) {
            int i = 0;
            
            PopulationBreedingChances.Sort(BreedingChance.OrderByFitnessScore);
            Chromossome c;

            foreach (BreedingChance bc in PopulationBreedingChances) {
                if (i == range) {
                    break;
                }
                
                c = bc.chromossome;
                kill(c);
                
                i++;
            }
        }
        
        private struct BreedingChance {
            public Chromossome chromossome;
            public float fitnessScore;
            public float breedingChance;

            public BreedingChance(Chromossome chromossome, float fitnessScore) {
                this.chromossome = chromossome;
                this.fitnessScore = fitnessScore;
                breedingChance = 0.0f;
            }

            public static int OrderByFitnessScore(BreedingChance bc1, BreedingChance bc2) {
                if (bc1.breedingChance > bc2.breedingChance) {
                    return 1;
                }
                
                else if (bc1.breedingChance < bc2.breedingChance) {
                    return -1;
                }
                return 0;
            }
        }
        
    }
}