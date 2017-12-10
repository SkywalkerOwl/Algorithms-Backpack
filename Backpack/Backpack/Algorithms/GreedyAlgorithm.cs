using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack
{
    class GreedyAlgorithm : IBackpackAlgorithm
    {
        public List<int> Run(Backpack backpack, ref double sumCost)
        {
            sumCost = 0;
            var items = backpack.Items.OrderByDescending(x => x.AveCost).ToList();

            double curMass = 0;
            List<int> nums = new List<int>();
            int index = 0;

            while (index < backpack.Items.Count)
            {
                if (curMass + items[index].Mass <= backpack.MaxMass)
                {
                    nums.Add(items[index].Num);
                    sumCost += items[index].Cost;
                    curMass += items[index].Mass;
                }
                index++;
            }

            return nums;
        }
    }
}
