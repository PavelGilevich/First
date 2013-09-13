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
                Console.WriteLine("Select\n 1 - Grow Apples\n 0 - Exit");
                key = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

           
                switch (key) 
                {               
                    case 0:                   
                        break;              
                   
                    case 1:                       
                        int apples;                       
                        Random rand = new Random();
                        apples = rand.Next(100);

                        tree.Grow(apples);
                        Console.WriteLine("{0} apples were grown", apples);

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
        public Tree()
        {
            number = 0;
        }

        public int GetNumber()
        {
            return number;
        }

        public void Grow(int NumberOfGrowingApples)
        {
            number += NumberOfGrowingApples;
        }
    }

    class Apple
    {
        int NumberOfBones;
    }

}
