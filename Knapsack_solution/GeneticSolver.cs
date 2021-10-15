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
        private int currentGeneration = 0;
        private int chromossomeGenesQnt;

        private float bestScore = 0.0f;
        public float mutationProbability = 0.05f;
        public List<float> statesFitnessScores { get; set; } = new List<float>();

        private List<float> bestsHistory = new List<float>();

        public GeneticSolver(int chromossomeGenesQnt, int maxPopulationSize = 1000, int maxAllowedGeneration = 100, float mutationProbability = 0.05f) {
            this.maxAllowedGeneration = maxAllowedGeneration;
            this.chromossomeGenesQnt = chromossomeGenesQnt;
        }
        
        public Backpack Solve(int genRangeForElitismDetector, float mutationGrowthRate = 0.5f, bool mutationRateStatic = true) {
            int i = 0;
            generateFirstGeneration();

            if (mutationRateStatic) {
                while (i < maxAllowedGeneration) {
                    if (i >= genRangeForElitismDetector * 2) {
                        if (elitismDetector(genRangeForElitismDetector)) {
                            killHighestFitnessScoresBeings(10);
                            if (mutationProbability <= 0.3) {
                                mutationProbability = (float) (Randomizer.NextDouble()  * (0.0 - 0.5) + 0.5) + (mutationProbability * mutationGrowthRate);
                            }
                            
                            Console.WriteLine($"Mutation probability: {mutationProbability}, generation: {currentGeneration}");
                        }
                    }
                
                    if (i % 2 == 0) {
                        generateFirstGeneration();
                    }
                    generateNextGeneration();
                    i = i + 2;
                    currentGeneration = i;
                }
            }

            else {
                while (true) {
                    if (i >= genRangeForElitismDetector * 2) {
                        if (elitismDetector(genRangeForElitismDetector)) {
                            mutationProbability = mutationProbability * mutationGrowthRate;
                            mutationGrowthRate = mutationGrowthRate * 1.5f;
                        }
                    }
                
                    if (i % 2 == 0) {
                        generateFirstGeneration();
                    }
                    generateNextGeneration();
                    i = i + 2;
                    
                    
                }
            }
            
            Console.WriteLine($"\nBest solution fitness is: {bestScore}");
            return Chromossome.decodeChromossome(Best, chromossomeGenesQnt);
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
            
            Chromossome localBest = null;
            
            foreach (Chromossome c in Population) {
                beingFitness = fit(c);
                totalFitness = totalFitness + beingFitness;

                if (beingFitness > bestScore) {
                    bestScore = beingFitness;
                    Best = c;
                }

                if (beingFitness > bestGenerationScore) {
                    bestGenerationScore = beingFitness;
                    localBest = c;
                }
                
                PopulationBreedingChances.Add(new BreedingChance(c, beingFitness));
            }
            
            bestsHistory.Add(bestGenerationScore);
            Console.WriteLine($"Best Generation's score: {bestGenerationScore}");

            BreedingChance bc;
            for (i = 0; i < PopulationBreedingChances.Count; i++) {
                bc = PopulationBreedingChances[i];
                
                bc.breedingChance = (bc.fitnessScore / totalFitness);
                drawnFloat = (float) Randomizer.NextDouble();
                
                if (bc.breedingChance >= drawnFloat) {
                    if (currentPopulationSize >= maxPopulationSize) {
                        killLowestsFitnessScoresBeings(100);
                    }

                    if (currentGeneration >= maxAllowedGeneration - 30) {
                        mutate(localBest);
                    }
                    breed(localBest, bc.chromossome, mutationProbability);
                }
                
            }
        }
        
        //Detects if elitism is occurring by checking for unchanged fitness value on genRange last gens
        private bool elitismDetector(int genRange = 10, float lowerBound = 0.1f, float upperBound = 0.1f) {
            int i;
            
            float totalFitnessScore = 0.0f;
            float mean = 0.0f;
            float[] possibleElitists = new float[genRange];

            for (i = 0; i < genRange; i++) {
                possibleElitists[i] = bestsHistory[bestsHistory.Count - (genRange - i)];
            }

            for (i = 0; i < genRange; i++) {
                totalFitnessScore = totalFitnessScore + possibleElitists[i];
            }

            mean = totalFitnessScore / genRange;

            return (mean >= bestScore - lowerBound && mean <= bestScore + upperBound);
        }
        
        private float fit(Chromossome c) {
            float bpCurrentValue;
            Backpack bp = Chromossome.decodeChromossome(c, chromossomeGenesQnt);
            
            if (isValidSolution(c)) {
                bpCurrentValue = bp.currentValue;
                
                statesFitnessScores.Add(bpCurrentValue);
                return bpCurrentValue;
            }
            
            //Console.WriteLine($"Invalid Solution !");
            return 0.0f;
        }
        
        private bool isValidSolution(Chromossome c) {
            Backpack bp = Chromossome.decodeChromossome(c, chromossomeGenesQnt);
            return !(bp.currentWeight > bp.capacity);
        }
        
        private void createNewBeing(float itemAcceptanceProbability = 0.5f) {
            int i;
            
            float drawnFloat;
            
            bool[] encodedChromossome = new bool[chromossomeGenesQnt];
            
            Chromossome c;
            
            for (i = 0; i < chromossomeGenesQnt; i++) {
                drawnFloat = (float)Randomizer.NextDouble();
                
                if (itemAcceptanceProbability >= drawnFloat) {
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
        
        private void mutate(Chromossome c) {
            float drawnFloat = (float)Randomizer.NextDouble();
            int drawnInt = Randomizer.Next(0, chromossomeGenesQnt);

            if (mutationProbability >= drawnFloat && (currentGeneration == maxAllowedGeneration - 1)) {
                c.changeGene(drawnInt);
            }
            
        }

        private void breed(Chromossome father, Chromossome mother, int slicingIndex = 10, float mutationProbability = 0.2f) {
            int i;
            
            Chromossome son1;
            Chromossome son2;

            bool[] encodedFatherChromossome;
            bool[] encodedMotherChromossome;

            bool[] encodedSon1Chromossome = new bool[chromossomeGenesQnt];
            bool[] encodedSon2Chromossome = new bool[chromossomeGenesQnt];
            
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
            
            mutate(son1);
            mutate(son2);
            
            addNewChromossomeToPopulation(son1);
            addNewChromossomeToPopulation(son2);
        }

        private void breed(Chromossome father, Chromossome mother, float mutationProbability = 0.2f) {
            int i;
            
            Chromossome son1;
            Chromossome son2;

            bool[] mask;
            bool[] encodedFatherChromossome;
            bool[] encodedMotherChromossome;

            bool[] encodedSon1Chromossome = new bool[chromossomeGenesQnt];
            bool[] encodedSon2Chromossome = new bool[chromossomeGenesQnt];

            mask = generateMask(chromossomeGenesQnt, 0.5f);
            
            encodedFatherChromossome = father.encodedChromossome;
            encodedMotherChromossome = mother.encodedChromossome;
            
            
            for (i = 0; i < chromossomeGenesQnt; i++) {
                if (mask[i]) {
                    encodedSon1Chromossome[i] = encodedFatherChromossome[i];
                    encodedSon2Chromossome[i] = encodedMotherChromossome[i];
                }

                else {
                    encodedSon1Chromossome[i] = encodedMotherChromossome[i];
                    encodedSon2Chromossome[i] = encodedFatherChromossome[i];
                }
            }
            
            son1 = new Chromossome(encodedSon1Chromossome);
            son2 = new Chromossome(encodedSon2Chromossome);
            
            mutate(son1);
            mutate(son2);
            
            addNewChromossomeToPopulation(son1);
            addNewChromossomeToPopulation(son2);
        }

        private bool[] generateMask(int maskSize, float trueProbability) {
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
        
        private void killHighestFitnessScoresBeings(int range) {
            int i = PopulationBreedingChances.Count;
            range = i - range;
            
            PopulationBreedingChances.Sort(BreedingChance.OrderByFitnessScore);
            Chromossome c;

            foreach (BreedingChance bc in PopulationBreedingChances) {
                if (i == range) {
                    break;
                }
                
                c = bc.chromossome;
                kill(c);
                
                i--;
            }
        }

        public float[] getStatesFitnessInArrayForm() {
            return statesFitnessScores.ToArray();
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