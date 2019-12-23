using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LARP.Minesweeper;

namespace LARP.Science.Operation
{
    public partial class HealingDetails : OperationInfo
    {
        private int[] operationSetup;
        private int operationSuccess;

        protected override void OnStart()
        {

        }

        protected async override Task<bool> Process()
        {
            MinesweeperWindow sweeper = new MinesweeperWindow(operationSetup);
            sweeper.ShowDialog();
            operationSuccess = await sweeper.Execute();
            if (operationSuccess >= 2200) return false; else return true;
        }

        protected override void OnFail()
        {
            Patient.Alive = false;
        }

        protected override void OnSuccess()
        {
            Patient.Stat.operationsSurvived++;
            Database.Controller.SaveCharacters();
        }

        protected override void OnFinished()
        {
            
        }

        private static List<int> ParametrizedDifficultyCalculation(Random randomizer, int minMines, int minAttempts, int maxAttempts, int minIncreasage, int maxIncreasage)
        {
            List<int> result = new List<int>();
            int amount = randomizer.Next(minAttempts, maxAttempts);
            int previous = minMines;
            for (int i = 0; i < amount; i++)
                result.Add(previous = randomizer.Next(previous, previous + randomizer.Next(minIncreasage, maxIncreasage)));
            return result;
        }

        private void CalculateDifficulty(List<DamageType> damageDealt)
        {
            List<int> result = new List<int>();
            Random randomizer = new Random();
            foreach (DamageType damage in damageDealt)
            {
                List<int> subResult = new List<int>();
                switch (damage)
                {
                    case DamageType.Blade:
                    case DamageType.Blunt:
                        {
                            subResult = ParametrizedDifficultyCalculation(randomizer, 5, 1, 2, 2, 5);
                            break;
                        }
                    case DamageType.Electro:
                    case DamageType.Blaster:
                        {
                            subResult = ParametrizedDifficultyCalculation(randomizer, 7, 1, 3, 3, 6);
                            break;
                        }
                    case DamageType.Saber:
                        {
                            subResult = ParametrizedDifficultyCalculation(randomizer, 9, 2, 3, 4, 8);
                            break;
                        }
                    case DamageType.Poison:
                        {
                            subResult = ParametrizedDifficultyCalculation(randomizer, 10, 3, 4, 2, 3);
                            break;
                        }
                    case DamageType.BodyHole:
                        {
                            subResult = ParametrizedDifficultyCalculation(randomizer, 10, 4, 5, 2, 4);
                            break;
                        }
                }
                result.AddRange(subResult);
            }
            operationSetup = result.ToArray();
        }

        public HealingDetails(List<DamageType> damageDealt)
        {
            CalculateDifficulty(damageDealt);
        }
    }
}