using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLite
{
    class Attribute
    {
        protected int attributeID;     //id атрибута
        protected String name;         //имя для пользователя
        protected String realName;     //имя в таблицу
        
        public Attribute() { }
        public Attribute(int id, String name, String realName)
        {
            this.attributeID = id;
            this.name = name;
            this.realName = realName;
        }

        public int ID
        {
            get { return attributeID; }
            set { attributeID = value; }
        }

        public String Name
        {
            get {return name;}
            set { name = value; }
        }

        public String RealName
        {
            get { return realName; }
            set { realName = value; }
        }
    }
}
