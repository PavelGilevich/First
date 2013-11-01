using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Xml;
using System.Collections;

namespace SQLite
{
    class Program
    {
        struct FixedElement
        {
            public String full_name;
            public String column_name;
            public String value;
        };

        struct Element
        {
            public String full_name;
            public String table;
            public String column_name;
        };

        public static void GenerateXML(SQLiteConnection connection)
        {
            string nameXML = "D:\\UNIVER\\3 курс\\Спецкурс\\";
            Console.Write("Print the name of XML Document: ");
            nameXML += Console.ReadLine();
            nameXML += ".xml";
            List<string> Dimensions = new List<string>();
            Dimensions.Add("apple"); Dimensions.Add("Shops"); Dimensions.Add("Date");

            string dimensionColumn = "", dimensionRow = "", dimensionFixed = "";

            using (XmlWriter writer = XmlWriter.Create(nameXML))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Report");

                int key = 0;
                do
                {
                    Console.Write("Select dimension on column\n 1 - {0}\n 2 - {1}\n 3 - {2}\nInput key: ", Dimensions[0],
                        Dimensions[1], Dimensions[2]);
                    key = Convert.ToInt32(Console.ReadLine());
                    switch (key)
                    {
                        case 1:
                            dimensionColumn = Dimensions[0];
                            Dimensions.RemoveAt(0);
                            break;
                        case 2:
                            dimensionColumn = Dimensions[1];
                            Dimensions.RemoveAt(1);
                            break;
                        case 3:
                            dimensionColumn = Dimensions[2];
                            Dimensions.RemoveAt(2);
                            break;
                        default:
                            Console.WriteLine("Wrong key!");
                            key = 0;
                            break;
                    }
                } while (key == 0);

                do
                {
                    Console.Write("Select dimension on row\n 1 - {0}\n 2 - {1}\nInput key: ", Dimensions[0],
                        Dimensions[1]);
                    key = Convert.ToInt32(Console.ReadLine());
                    switch (key)
                    {
                        case 1:
                            dimensionRow = Dimensions[0];
                            Dimensions.RemoveAt(0);
                            break;
                        case 2:
                            dimensionRow = Dimensions[1];
                            Dimensions.RemoveAt(1);
                            break;
                        default:
                            Console.WriteLine("Wrong key!");
                            key = 0;
                            break;
                    }
                } while (key == 0);

                dimensionFixed = Dimensions[0];
                Console.WriteLine("\nFixed dimension is {0}", dimensionFixed);

                writer.WriteElementString("DimensionByColumn", dimensionColumn);
                writer.WriteElementString("DimensionByRow", dimensionRow);
                writer.WriteElementString("Fixed", dimensionFixed);
                writer.WriteStartElement("Selection");

                Console.Write("----------------\nSelect ID-s from dimension {0} to show in report (separate by <Space> button):\n--------\n", 
                    dimensionColumn);
                string command = "SELECT * FROM ";
                command += dimensionColumn;

                SQLiteCommand comm = new SQLiteCommand(command, connection);
                SQLiteDataReader reader = comm.ExecuteReader();

                foreach (DbDataRecord record in reader)
                    Console.WriteLine(record[0] + "   " + record[1] + "   " + record[2]);

                Console.WriteLine("----------------");
                string result = Console.ReadLine();

                writer.WriteElementString(dimensionColumn, result);

                command = "SELECT * FROM ";
                command += dimensionRow;
                Console.Write("----------------\nSelect ID-s from dimension {0} to show in report (separate by <Space> button):\n--------\n", 
                    dimensionRow);
                comm = new SQLiteCommand(command, connection);
                reader = comm.ExecuteReader();

                foreach (DbDataRecord record in reader)
                    Console.WriteLine(record[0] + "   " + record[1] + "   " + record[2]);

                Console.WriteLine("--------------------");
                result = Console.ReadLine();

                writer.WriteElementString(dimensionRow, result);
                writer.WriteEndElement();
                writer.WriteEndElement();

            }
        }
                 

        static void Main(string[] args)
        {
            FixedElement fixedElement;
            fixedElement.full_name = ""; fixedElement.column_name = ""; fixedElement.value = "";

            List <Element> elements = new List<Element>();

            const string databaseName = @"D:\Warehouse_apple.db";
            SQLiteConnection connection =
                new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            connection.Open();

            XmlDocument doc = new XmlDocument();
            int key;
            do
            {
                Console.Write(" 1 - Fixed Apple\n 2 - Fixed Shop\n 3 - Fixed Date\n 4 - Create XML\n 0 - Exit\nSelect action: ");
                key = Convert.ToInt32(Console.ReadLine());

                switch (key)
                {
                    case 1:
                        doc.Load("D:\\UNIVER\\3 курс\\Спецкурс\\report1.xml");
                        break;
                    case 2:
                        doc.Load("D:\\UNIVER\\3 курс\\Спецкурс\\report2.xml");
                        break;
                    case 3:
                        doc.Load("D:\\UNIVER\\3 курс\\Спецкурс\\report3.xml");
                        break;
                    case 4:
                        GenerateXML(connection);
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Wrong key!");
                        break;
                }
            } while (key == 4);    
            
            XmlNode report = doc.SelectSingleNode("Report");

            foreach (XmlNode node in report)
            {
                if (node.Name == "fixed")
                {
                    XmlNode element = node.SelectSingleNode("element");
                    foreach (XmlNode child in element.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case "full_name":
                                fixedElement.full_name = child.InnerText;
                                break;
                            case "column_name":
                                fixedElement.column_name = child.InnerText;
                                break;
                            case "value":
                                fixedElement.value = child.InnerText;
                                break;
                        }
                    }
                }
                if (node.Name == "output")
                {
                    foreach (XmlNode element in node.ChildNodes)
                    {
                        Element out_element;
                        out_element.table = "";
                        out_element.column_name = "";
                        out_element.full_name = "";
                        foreach (XmlNode child in element.ChildNodes)
                        {
                            switch (child.Name)
                            {
                                case "full_name":
                                    out_element.full_name = child.InnerText;
                                    break;
                                case "column_name":
                                    out_element.column_name = child.InnerText;
                                    break;
                                case "table":
                                    out_element.table = child.InnerText;
                                    break;

                            }
                        }
                        elements.Add(out_element);
                    }
                }
            }



            string command = "SELECT";
            int i = 1;
            foreach (Element el in elements)
            {
                command += " ";
                command += el.table;
                command += ".";
                command += el.column_name;
                if (i < elements.Count)
                    command += ",";
                i++;
            }
            command += " FROM apple JOIN Sales ON (apple.apple_ID = Sales.apple_id) JOIN Shops ON (Shops.shop_ID = Sales.shop_id) JOIN Date ON (Date.date_ID = Sales.date_id) WHERE Sales.";
            command += fixedElement.column_name;
            command += "=";
            command += fixedElement.value;

            Console.WriteLine("\nFixed dimension: {0} = {1}\n", fixedElement.full_name, fixedElement.value);
            
            SQLiteCommand comm = new SQLiteCommand(command, connection);
            SQLiteDataReader reader = comm.ExecuteReader();
            i = 1;

            foreach (DbDataRecord record in reader)
            {
                Console.WriteLine("Row {0}:", i);
                int k = 0;
                while (k < elements.Count)
                {
                    Console.WriteLine(elements[k].full_name + ": " + record[k]);
                    k++;
                }
                Console.WriteLine("------------------------");
                i++;
            }
            
            connection.Close();
        }
    }
}
