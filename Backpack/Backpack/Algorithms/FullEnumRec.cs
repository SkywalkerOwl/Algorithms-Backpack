using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backpack
{
    class FullEnumRec : IBackpackAlgorithm
    {
        private class BackpackInfo
        {
            public double SummCost { get; set; }
            public double SummMass { get; set; }
            public List<bool> SelectedItems { get; set; }

            public BackpackInfo Copy()
            {
                return new BackpackInfo()
                {
                    SummCost = this.SummCost,
                    SummMass = this.SummMass,
                    SelectedItems = new List<bool>(this.SelectedItems)
                };
            }
        }

        private void Rec(Backpack backpack, BackpackInfo curBackpack, ref BackpackInfo bestBackpack)
        {
            if (curBackpack.SelectedItems.Count == backpack.Items.Count)
            {
                if (curBackpack.SummCost > bestBackpack.SummCost && curBackpack.SummMass <= backpack.MaxMass)
                {
                    bestBackpack = curBackpack.Copy();
                }
            }
            else
            {
                int indexCurItem = curBackpack.SelectedItems.Count;

                BackpackInfo unselectItem = curBackpack.Copy();
                unselectItem.SelectedItems.Add(false);
                Rec(backpack, unselectItem, ref bestBackpack);
                
                BackpackInfo selectItem = curBackpack.Copy();
                selectItem.SelectedItems.Add(true);
                selectItem.SummCost += backpack.Items[indexCurItem].Cost;
                selectItem.SummMass += backpack.Items[indexCurItem].Mass;
                Rec(backpack, selectItem, ref bestBackpack);
            }
        }

        public List<int> Run(Backpack backpack, ref double sumCost)
        {
            sumCost = 0;
            BackpackInfo result = new BackpackInfo()
            {
                SelectedItems = new List<bool>(),
                SummCost = 0,
                SummMass = 0
            };

            BackpackInfo startBackpackInfo = new BackpackInfo()
            {
                SelectedItems = new List<bool>(),
                SummCost = 0,
                SummMass = 0
            };

            Rec(backpack, startBackpackInfo, ref result);

            List<int> resultNums = new List<int>();
            for (int i = 0; i < result.SelectedItems.Count; i++)
            {
                if (result.SelectedItems[i])
                {
                    sumCost += backpack.Items[i].Cost;
                    resultNums.Add(backpack.Items[i].Num);
                }
            }

            return resultNums;
        }
    }
}
