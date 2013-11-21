using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SQLite
{
    class Dimensions
    {
        int dimensionID;               //id измерения
        String userName;               //имя для пользователя
        String tableName;              //имя таблицы
        String PrimaryKey;             //имя поля-первичного ключа
        List<Attribute> Attributes;    //список атрибутов

        public Dimensions()
        {
            Attributes = new List<Attribute>();
        }
        public Dimensions(int id, String username, String tablename, String PK)
        {
            this.dimensionID = id;
            this.userName = username;
            this.tableName = tablename;
            this.PrimaryKey = PK;
            Attributes = new List<Attribute>();
        }
        public int ID
        {
            get { return dimensionID; }
            set { dimensionID = value; }
        }
        public String UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public String TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public String PK
        {
            get { return PrimaryKey; }
            set { PrimaryKey = value; }
        }
        public void AddAttribute(Attribute attribute)
        {
            Attributes.Add(attribute);
        }
        public List<Attribute> AttributesList
        {
            get { return Attributes; }
        }
    }
}
