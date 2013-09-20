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
            int key = 1;
            Tree tree = new Tree();
            

            do
            {
                if (key == 1)
                {
                    Console.WriteLine("Press\n 1 - to Blossom\n 0 - to Exit");
                    key = Convert.ToInt32(Console.ReadLine());

                    switch (key)
                    {
                        case 1:
                            int flowers = tree.Blossom();
                            tree.Grow(flowers);
                            break;

                        case 0:
                            break;

                        default:
                            Console.WriteLine("You entered the wrong key!");
                            break;
                    }
                }
                if (key == 0)
                    break;

                Console.Clear();
                Console.WriteLine("Number of apples now: {0}", tree.GetNumber());
                Console.WriteLine("Number of seeds now: {0}", tree.GetSeeds());
                Console.WriteLine("Select\n 1 - Reset\n 2 - Shake the tree\n 0 - Exit");
                key = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

           
                switch (key) 
                {               
                    case 0:                   
                        break;              
                   
                    case 1:
                        tree.Reset();                  
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

        public void Grow(int num_apples)
        {
            Random rand = new Random();
            for (int i = 1; i <= num_apples; i++)
            {
                Apple apple = new Apple(rand.Next(10));
                apples_list.Add(apple);
            }

            this.number += num_apples;
            Console.WriteLine("{0} apples were grown", num_apples);
        }

        public int Blossom()
        {
            Random rand = new Random();
            int num_flowers = rand.Next(300);

            return num_flowers;
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

        public void Reset()
        {
            this.number = 0;
            apples_list.Clear();
            Console.WriteLine("The progress was reseted");
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
