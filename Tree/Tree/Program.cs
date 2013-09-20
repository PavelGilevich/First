using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tree
{
    class Program
    {
        static void Main(string[] args)
        {
            int key;
            Tree tree = new Tree();

            do
            {
                Console.WriteLine("Number of apples now: {0}", tree.GetNumber());
                Console.WriteLine("Number of seeds now: {0}", tree.GetSeeds());
                Console.WriteLine("Select\n 1 - Grow Apples\n 2 - Shake the tree\n 0 - Exit");
                key = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

           
                switch (key) 
                {               
                    case 0:                   
                        break;              
                   
                    case 1:                                             
                        tree.Grow();                    
                        break;   
                    
                    case 2:
                        tree.Shake();
                        break;

                    default:
                        Console.WriteLine("You entered wrong key!");
                        break;
                }            
            } while (key != 0);
        }
    }

    class Tree
    {
        int number;
        List<Apple> apples_list;

        public Tree()
        {
            number = 0;
            apples_list = new List<Apple>();
        }

        public int GetNumber()
        {
            return number;
        }

        public void Grow()
        {
            int num_apples;
            Random rand = new Random();
            num_apples = rand.Next(100);

            for (int i = 1; i <= num_apples; i++)
            {
                Apple apple = new Apple(rand.Next(10));
                apples_list.Add(apple);
            }

            this.number += num_apples;
            Console.WriteLine("{0} apples were grown", num_apples);
        }

        public int GetSeeds()
        {
            int num = 0;
            foreach (Apple app in apples_list)
                num += app.GetNumberOfSeeds();
            return num;
        }
        public void Shake()
        {
            int apples;
            Random rand = new Random();
            apples = rand.Next(GetNumber() + 1);

            for (int i = 1; i <= apples; i++)
                apples_list.RemoveAt(0);

            this.number -= apples;
            Console.WriteLine("{0} apples were shaked", apples);
        }
    }

    class Apple
    {
        int NumberOfSeeds;

        public int GetNumberOfSeeds()
        {
            return this.NumberOfSeeds;
        }

        public Apple(int seeds)
        {
            this.NumberOfSeeds = seeds;
        }
    }

}
