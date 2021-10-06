using System;

namespace Knapsack_solution
{
    class Program
    {
        static void Main(string[] args) {
            GeneticSolver GS = new GeneticSolver();
            Backpack solution = GS.Solve();
        }
    }
}
