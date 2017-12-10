using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack
{
    public class FullEnumIter : IBackpackAlgorithm
    {
        private class BinMask
        {
            public List<bool> Mask { get; private set; }

            public BinMask(int maxValue)
            {
                Mask = new List<bool>();
                
                for (int i = 0; i < maxValue; i++)
                {
                    Mask.Add(false);
                }
            }

            public static BinMask operator ++(BinMask binMask)
            {
                int i = -1;
                do
                {
                    i++;
                    binMask.Mask[i] = !binMask.Mask[i];
                }
                while (!binMask.Mask[i] && binMask.Mask.Count > i + 1);

                return binMask;
            }
        }

        private class BackpackInfo
        {
            public double SummCost { get; set; }
            public double SummMass { get; set; }
            public List<bool> Items { get; set; }
        }

        public List<int> Run(Backpack backpack, ref double sumCost)
        {
            sumCost = 0;
            BackpackInfo result = new BackpackInfo()
            {
                Items = new List<bool>(),
                SummCost = 0,
                SummMass = 0
            };

            long maxIter = (long)Math.Pow(2, backpack.Items.Count);
            BinMask binMask = new BinMask(backpack.Items.Count);
            for (long i = 0; i < maxIter; i++)
            {
                BackpackInfo curBackpack = new BackpackInfo()
                {
                    Items = new List<bool>(),
                    SummCost = 0,
                    SummMass = 0
                };

                for (int j = 0; j < binMask.Mask.Count; j++)
                {
                    curBackpack.Items.Add(binMask.Mask[j]);
                    if (binMask.Mask[j])
                    {
                        curBackpack.SummCost += backpack.Items[j].Cost;
                        curBackpack.SummMass += backpack.Items[j].Mass;
                    }
                }
                binMask++;

                if (curBackpack.SummMass <= backpack.MaxMass && curBackpack.SummCost > result.SummCost)
                {
                    result = curBackpack;
                }
            }

            List<int> resultNums = new List<int>();
            for (int i = 0; i < result.Items.Count; i++)
            {
                if (result.Items[i])
                {
                    sumCost += backpack.Items[i].Cost;
                    resultNums.Add(backpack.Items[i].Num);
                }
            }

            return resultNums;
        }
    }
}