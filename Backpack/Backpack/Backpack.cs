using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backpack
{
    public class Item
    {
        public double Cost { get; }
        public double Mass { get; }
        public double AveCost { get; }
        public int Num { get; }

        public Item(double cost, double mass, int num)
        {
            Cost = cost;
            Mass = mass;
            AveCost = cost / mass;
            Num = num;
        }
    }

    public class Backpack
    {
        public double MaxMass { get; }
        public List<Item> Items { get; set; }

        public Backpack(double mass)
        {
            MaxMass = mass;
            Items = new List<Item>();
        }

        public void AddItem(double cost, double mass)
        {
            Items.Add(new Item(cost, mass, Items.Count + 1));
        }
    }

    public static class Test
    {
        private static Random rand = new Random((int)DateTime.Now.Ticks);

        public static void GenTest(int countItems, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                int sizeBackpack = rand.Next(5000, 10000);


                List<int> listOfMass = new List<int>();                
                for (int i = 0; i < countItems; i++) // Веса предметов
                {
                    listOfMass.Add(rand.Next(sizeBackpack / countItems - 100, sizeBackpack / countItems + 1000));
                }
                
                List<int> listOfCost = new List<int>();                
                for (int i = 0; i < countItems; i++) // Стоимости предметов
                {
                    listOfCost.Add(rand.Next(7000, 8000));
                }
                
                writer.WriteLine($"{countItems} {sizeBackpack}");
                writer.WriteLine(string.Join(" ", listOfMass));
                writer.WriteLine(string.Join(" ", listOfCost));
            }
        }

        public static void GenTest(int countItems, ref Backpack backpack)
        {
            backpack = new Backpack(rand.Next(5000, 10000));
            
            for (int i = 0; i < countItems; i++) // Веса предметов
            {
                backpack.Items.Add(new Item(
                        mass: rand.Next(7000, 8000), 
                        cost: rand.Next((int)backpack.MaxMass / countItems - 100, (int)backpack.MaxMass / countItems + 1000),  
                        num: i + 1));
            }
        }

        public static void GenBadTestForGreedy(int countItems, ref Backpack backpack)
        {
            GenTest(countItems, ref backpack);
            backpack.Items = backpack.Items.OrderByDescending(x => x.AveCost).ToList();
        }
    }
}