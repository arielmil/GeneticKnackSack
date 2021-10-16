using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Knapsack_solution {
    public class GeneticSolver {
        private List<Chromossome> Population = new List<Chromossome>();
        private Chromossome Best;

        private List <BreedingChance> PopulationBreedingChances = new List<BreedingChance>();
        private Random Randomizer;
        private int currentPopulationSize = 0;
        private int maxAllowedGeneration;

        private float bestScore = 0.0f;
        private float mutationProbability = 0.05f;

        public delegate float FIT(Chromossome c);
        private FIT Fit; 
        
        public delegate bool ISVALIDSOLUTION(Chromossome c);
        private ISVALIDSOLUTION ISValidSolution;
        
        public delegate Chromossome[] BREED(Chromossome father, Chromossome mother, float mutationProbability = 0.2f);
        private BREED Breed;
        
        public delegate Chromossome CREATENEWBEING();
        private CREATENEWBEING CreateNewBeing;
        
        private List<float> statesFitnessScores { get; set; } = new List<float>();

        private List<float> bestsHistory = new List<float>();

        // ReSharper disable once UnusedParameter.Local
        public GeneticSolver(int maxPopulationSize = 1000, int maxAllowedGeneration = 100, float mutationProbability = 0.05f) {
            this.maxAllowedGeneration = maxAllowedGeneration;
        }

        // ReSharper disable once UnusedParameter.Local
        public GeneticSolver(FIT fit,  ISVALIDSOLUTION isValidSolution, BREED breed, CREATENEWBEING createnewbeing, Random Randomizer, int maxPopulationSize = 1000, int maxAllowedGeneration = 100, float mutationProbability = 0.05f) {
            Fit = fit;
            ISValidSolution = isValidSolution;
            Breed = breed;
            CreateNewBeing = createnewbeing;
            
            this.maxAllowedGeneration = maxAllowedGeneration;
            this.Randomizer = Randomizer;
        }
        
        public Chromossome Solve(int genRangeForElitismDetector, float mutationGrowthRate = 1.0f, bool mutationRateStatic = true) {
            int i = 0;
            generateFirstGeneration();

            if (mutationRateStatic) {
                while (i < maxAllowedGeneration) {
                    if (i >= genRangeForElitismDetector * 2) {
                        if (i >= maxAllowedGeneration - 15) {
                            if (elitismDetector(genRangeForElitismDetector)) {
                                killHighestFitnessScoresBeings(10);
                                mutationProbability = (float) (Randomizer.NextDouble()  * (0.0 - 0.5) + 0.5);
                            }
                        }
                    }
                
                    if (i % 2 == 0) {
                        generateFirstGeneration();
                    }
                    generateNextGeneration();
                    i = i + 2;
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
            return Best;
        }

        private void createNewBeing() {
            addNewChromossomeToPopulation(CreateNewBeing());
        }

        private void breed(Chromossome father, Chromossome mother) {
            Chromossome[] array = Breed(father, mother, mutationProbability);

            Chromossome son1 = array[0];
            Chromossome son2 = array[1];
            
            addNewChromossomeToPopulation(son1);
            addNewChromossomeToPopulation(son2);
        }
        
        // ReSharper disable once UnusedMember.Local
        private bool isValidSolution(Chromossome c) {
            return ISValidSolution(c);
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
                totalFitness += beingFitness;

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
                    
                    //mutate(localBest);
                    breed(localBest, bc.chromossome);
                }
                
            }
        }
        
        //Detects if elitism is occurring by checking for unchanged fitness value on genRange last gens
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private bool elitismDetector(int genRange = 10, float lowerBound = 0.1f, float upperBound = 0.1f) {
            int i;
            
            float totalFitnessScore = 0.0f;
            float mean = 0.0f;
            float[] possibleElitists = new float[genRange];

            for (i = 0; i < genRange; i++) {
                possibleElitists[i] = bestsHistory[bestsHistory.Count - (genRange - i)];
            }

            for (i = 0; i < genRange; i++) {
                totalFitnessScore += possibleElitists[i];
            }

            mean = totalFitnessScore / genRange;

            return (mean >= bestScore - lowerBound && mean <= bestScore + upperBound);
        }

        private float fit(Chromossome c) {
            float fitness = Fit(c);
            statesFitnessScores.Add(fitness);
            return fitness;
        }
        
        // ReSharper disable once UnusedMember.Local
        private void replaceBeing(Chromossome c1, Chromossome c2) {
            Population.Remove(c1);
            addNewChromossomeToPopulation(c2);
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
            
            foreach (BreedingChance bc in PopulationBreedingChances) {
                if (i == range) {
                    break;
                }
                
                kill(bc.chromossome);
                
                i++;
            }
        }
        
        private void killHighestFitnessScoresBeings(int range) {
            int i = PopulationBreedingChances.Count;
            range = i - range;
            
            PopulationBreedingChances.Sort(BreedingChance.OrderByFitnessScore);
            
            foreach (BreedingChance bc in PopulationBreedingChances) {
                if (i == range) {
                    break;
                }
                
                kill(bc.chromossome);
                
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