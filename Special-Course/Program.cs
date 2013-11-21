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
        class Element
        {
            int DimensionID;            //id измерения
            public List<int> values;    //значения ключей

            public Element()
            {
                values = new List<int>();
            }
            public int DimID
            {
                get { return DimensionID; }
                set { DimensionID = value; }
            }
        };

        class FixedElement
        { 
            int DimensionID;                //id измерения
            int FixedID;                    //значение зафиксированного первичного ключа
            
            public FixedElement() { }
            public int DimID
            {
                get { return DimensionID; }
                set { DimensionID = value; }
            }
            public int FixID
            {
                get { return FixedID; }
                set { FixedID = value; }
            }
        };

        public static void Selection(Dimensions dimension, SQLiteConnection connection, XmlWriter writer)
        {
            Console.WriteLine("---------------\nSelect fixed ID in dimension {0}:", dimension.UserName);
            string command = "SELECT * FROM ";
            command += dimension.TableName;      //запрос для получения из БД всех полей измерения dimension

            SQLiteCommand comm = new SQLiteCommand(command, connection);
            SQLiteDataReader reader = comm.ExecuteReader();
            foreach (DbDataRecord record in reader)
                Console.WriteLine(record[0] + " - " + record[1] + "   " + record[2]);

            Console.WriteLine("------------------");
            string result = Console.ReadLine(); //вводим значения ID для тех полей, которые хотим зафиксировать
            writer.WriteString(result);         //записываем эти значения в XML
        }

        public static void GenerateXML(List <Dimensions> allDimensions, SQLiteConnection connection)
        {
            string nameXML = "D:\\UNIVER\\3 курс\\Спецкурс\\";
            Console.Write("Print the name of XML Document: ");
            nameXML += Console.ReadLine();
            nameXML += ".xml";

            List<Dimensions> copyList = new List<Dimensions>(allDimensions); //список измерений
            int dimensionColumn = 0, dimensionRow = 0, dimensionFixed = 0;

            using (XmlWriter writer = XmlWriter.Create(nameXML))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Report");

                int key = 0;
                do
                {
                    //выбираем измерение по столбцам
                    Console.Write("\nSelect dimension on column\n 1 - {0}\n 2 - {1}\n 3 - {2}\nInput key: ", copyList[0].UserName,
                        copyList[1].UserName, copyList[2].UserName);
                    key = Convert.ToInt32(Console.ReadLine());
                    if (key != 1 & key != 2 & key != 3)
                    {
                        Console.WriteLine("Wrong key!");
                        key = 0;
                    }

                    else
                    {
                        dimensionColumn = copyList[key - 1].ID;
                        copyList.RemoveAt(key - 1);
                    }
                } while (key == 0);

                do
                {
                    //выбираем измерение по строкам
                    Console.Write("\nSelect dimension on row\n 1 - {0}\n 2 - {1}\nInput key: ", copyList[0].UserName,
                        copyList[1].UserName);
                    key = Convert.ToInt32(Console.ReadLine());
                    if (key != 1 & key != 2)
                    {
                        Console.WriteLine("Wrong key!");
                        key = 0;
                    }

                    else
                    {
                        dimensionRow = copyList[key - 1].ID;
                        copyList.RemoveAt(key - 1);
                    }
                } while (key == 0);

                dimensionFixed = copyList[0].ID; //фиксированное измерение
                Console.WriteLine("\nFixed dimension is {0}", allDimensions[dimensionFixed-1].UserName);

                writer.WriteElementString("DimensionByColumn", allDimensions[dimensionColumn-1].ID.ToString());
                writer.WriteElementString("DimensionByRow", allDimensions[dimensionRow-1].ID.ToString());
                writer.WriteStartElement("Fixed");
                writer.WriteElementString("Dimension", allDimensions[dimensionFixed-1].ID.ToString());

                writer.WriteStartElement("ID");
                Selection(allDimensions[dimensionFixed-1], connection, writer);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Selection");

                writer.WriteStartElement("Column");
                writer.WriteStartElement("ID");
                Selection(allDimensions[dimensionColumn-1], connection, writer);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Row");
                writer.WriteStartElement("ID");
                Selection(allDimensions[dimensionRow-1], connection, writer);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndElement();

            }
        }

        public static void ReadFacts(Facts facts)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load("D:\\UNIVER\\3 курс\\Спецкурс\\Facts.xml");
            XmlNode report = xml.SelectSingleNode("Report_Facts");
            XmlNode datatable = report.SelectSingleNode("Datatable");
            facts.Name = datatable.SelectSingleNode("name").InnerText;
            facts.RealName = datatable.SelectSingleNode("real_name").InnerText;
            facts.Measure = Convert.ToInt32(datatable.SelectSingleNode("measure").InnerText);
            XmlNode fields = datatable.SelectSingleNode("Fields");

            foreach (XmlNode field in fields)
            {
                Field concrete_field = new Field();
                concrete_field.ID = Convert.ToInt32(field.SelectSingleNode("ID").InnerText);
                concrete_field.Name = field.SelectSingleNode("name").InnerText;
                concrete_field.RealName = field.SelectSingleNode("real_name").InnerText;
                String reference = field.SelectSingleNode("dimension").InnerText;
                if (reference.Equals(""))
                    concrete_field.Reference = 0;
                else
                    concrete_field.Reference = Convert.ToInt32(reference);
                
                facts.AddField(concrete_field);
            }
        }
        public static void ReadDimensions(List<Dimensions> dimensions)
        {
            XmlDocument dim = new XmlDocument();
            dim.Load("D:\\UNIVER\\3 курс\\Спецкурс\\Dimensions.xml");
            XmlNode node = dim.SelectSingleNode("Report_Dimensions");
            XmlNode all_dimensions = node.SelectSingleNode("Dimensions");

            foreach (XmlNode concrete_dimension in all_dimensions)
            {
                Dimensions dimension = new Dimensions();

                XmlNode dimInfo = concrete_dimension.SelectSingleNode("DimensionInfo");
                int id = Convert.ToInt32(dimInfo.SelectSingleNode("ID").InnerText);
                dimension.ID = id;
                String name = dimInfo.SelectSingleNode("name").InnerText;
                dimension.UserName = name;

                XmlNode attributes = concrete_dimension.SelectSingleNode("Attributes");
                foreach (XmlNode concrete_attribute in attributes)
                {
                    int attrID = Convert.ToInt32(concrete_attribute.SelectSingleNode("ID").InnerText);
                    String attrName = concrete_attribute.SelectSingleNode("name").InnerText;
                    String attrRealName = concrete_attribute.SelectSingleNode("real_name").InnerText;
                    Attribute attribute = new Attribute(attrID, attrName, attrRealName);
                    dimension.AddAttribute(attribute);
                }
                XmlNode tableInfo = concrete_dimension.SelectSingleNode("TableInfo");
                dimension.TableName = tableInfo.SelectSingleNode("name").InnerText;
                dimension.PK = tableInfo.SelectSingleNode("PrimaryKey").InnerText;
                dimensions.Add(dimension);
            }
        }

        static void Main(string[] args)
        {
            //создание экземпляров классов Element и FixedElement 
            FixedElement fixedElement = new FixedElement();
            Element dimensionColumn = new Element();
            Element dimensionRow = new Element();

            //создание списка измерений, а также экземпляра класса таблицы фактов
            List<Dimensions> allDimensions = new List<Dimensions>();
            Facts facts = new Facts();

            //установка соединения с БД
            const string databaseName = @"D:\Warehouse_apple.db";
            SQLiteConnection connection =
                new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            connection.Open();

            //заполнение полей экземляра таблицы фактов и списка измерений
            ReadFacts(facts);
            ReadDimensions(allDimensions);

            XmlDocument doc = new XmlDocument();
            int key;
            do
            {
                Console.Write(" 1 - Create XML\n 2 - Open report\n 0 - Exit\nSelect action: ");
                key = Convert.ToInt32(Console.ReadLine());

                switch (key)
                {
                    case 1:
                        GenerateXML(allDimensions, connection);
                        break;
                    case 2:
                        Console.Write("Input the name of report: ");
                        doc.Load("D:\\UNIVER\\3 курс\\Спецкурс\\"+Console.ReadLine());
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Wrong key!");
                        break;
                }
            } while (key == 1);    
            
            XmlNode report = doc.SelectSingleNode("Report");

            foreach (XmlNode node in report)
            {
                switch (node.Name)
                {
                    case "DimensionByColumn":
                        dimensionColumn.DimID = Convert.ToInt32(node.InnerText);
                        break;
                    case "DimensionByRow":
                        dimensionRow.DimID = Convert.ToInt32(node.InnerText);
                        break;
                    case "Fixed":
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.Name == "Dimension")
                            {
                                fixedElement.DimID = Convert.ToInt32(child.InnerText);
                            }
                            else
                            {
                                fixedElement.FixID = Convert.ToInt32(child.InnerText);
                                break;
                            }
                        }
                        break;
                    
                    case "Selection":
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            string text = child.FirstChild.InnerText;
                            string[] values = text.Split(' ');

                            if (child.Name == "Column")
                            {
                              //  dimensionColumn.key_name = child.FirstChild.Name;
                                for (int i = 0; i < values.Length; i++)
                                    dimensionColumn.values.Add(Convert.ToInt32(values[i]));
                            }

                            if (child.Name == "Row")
                            {
                             //   dimensionRow.key_name = child.FirstChild.Name;
                                for (int i = 0; i < values.Length; i++)
                                    dimensionRow.values.Add(Convert.ToInt32(values[i]));
                            }
                        }
                        break;
                }
            }

           //формируем запрос для получения названий строк
            string command_Row = "SELECT * FROM " + allDimensions[dimensionRow.DimID-1].TableName + " WHERE " + 
                allDimensions[dimensionRow.DimID-1].PK + " = " + dimensionRow.values[0].ToString();
            for (int i = 1; i < dimensionRow.values.Count; i++)
            {
                command_Row += " OR ID = ";
                command_Row += dimensionRow.values[i].ToString();
            }
            SQLiteCommand comm_Row = new SQLiteCommand(command_Row, connection);
            SQLiteDataReader reader_Row = comm_Row.ExecuteReader();

            //формируем запрос для получения названий столбцов
            string command_Column = "SELECT * FROM " + allDimensions[dimensionColumn.DimID-1].TableName + " WHERE  " + 
              allDimensions[dimensionColumn.DimID-1].PK + " = " + dimensionColumn.values[0].ToString();
            for (int i = 1; i < dimensionColumn.values.Count; i++)
            {
                command_Column += " OR ID = ";
                command_Column += dimensionColumn.values[i].ToString();
            }
            SQLiteCommand comm_Column = new SQLiteCommand(command_Column, connection);
            SQLiteDataReader reader_Column = comm_Column.ExecuteReader();

            //запрос для получения данных о фиксированном измерении
            string command = "SELECT * FROM " + allDimensions[fixedElement.DimID-1].TableName + " WHERE " + 
                allDimensions[fixedElement.DimID-1].PK + " = " + fixedElement.FixID.ToString();
            SQLiteCommand comm = new SQLiteCommand(command, connection);
            SQLiteDataReader reader = comm.ExecuteReader();
            List <Attribute> AttributesFixed = new List<Attribute>();
            AttributesFixed = allDimensions[fixedElement.DimID - 1].AttributesList;
            Console.WriteLine("------------------\nReport:");
            
            foreach (DbDataRecord record in reader)
                for (int num = 1; num < record.FieldCount; num++)
                {
                    Console.WriteLine("{0}: {1}", AttributesFixed[num].Name, record[num]);
                }

            Console.WriteLine();
            Console.WriteLine("В таблице выводится {0}\n", facts.GetField(facts.Measure - 1).Name);
            // Console.Write("{0}\\{1}", dimensionRow.table_name, dimensionColumn.table_name);
            // Console.Write("\t   ");
            foreach (DbDataRecord record_Column in reader_Column)
            {
                Console.Write("\t      {0} {1}", record_Column[1], record_Column[2]);
            }

            Console.WriteLine();
            int order = 0;
            Dimensions dimFixed = allDimensions[fixedElement.DimID-1];
            Dimensions dimColumn = allDimensions[dimensionColumn.DimID-1];
            Dimensions dimRow = allDimensions[dimensionRow.DimID-1];

            foreach (DbDataRecord record_Row in reader_Row)
            {
                string selection = "SELECT " + facts.GetField(facts.Measure-1).RealName +" FROM " + facts.RealName + " WHERE " + 
                    facts.GetField(dimFixed.ID-1).RealName + " = " + fixedElement.FixID.ToString() +
                    " AND " + facts.GetField(dimRow.ID-1).RealName + " = " + dimensionRow.values[order] + " AND ";
                Console.Write("{0} {1}|", record_Row[1], record_Row[2]);
                
                for (int l = 0; l < dimensionColumn.values.Count; l++)
                {
                    string finalSelection = selection + facts.GetField(dimColumn.ID-1).RealName + " = " + dimensionColumn.values[l].ToString();
                    SQLiteCommand result = new SQLiteCommand(finalSelection, connection);
                    SQLiteDataReader prices = result.ExecuteReader();

                    Console.Write("\t ");
                    foreach (DbDataRecord price in prices)
                        Console.Write("{0}", price[0]);
                    Console.Write("\t\t|");

                }
                order++;
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n--------------------------------");
            connection.Close();
        }
    }
}
