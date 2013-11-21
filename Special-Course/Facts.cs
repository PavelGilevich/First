using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLite
{
    class Field : Attribute
    {
        int ReferenceToDimension;          //связь с измерением (внешний ключ)
        public Field() { }
        public Field(int id, String name, String realName)
        {
            this.attributeID = id;
            this.name = name;
            this.realName = realName;
        }
        public int Reference
        {
            get { return ReferenceToDimension; }
            set { ReferenceToDimension = value; }
        }
    }
    class Facts
    {
        String name;            //имя таблицы для пользователя
        String realName;        //имя таблицы в БД
        int measure;            //id поля, в котором находится мера
        List<Field> fields;     //список полей в таблице

        public Facts()
        {
            fields = new List<Field>();
        }
        public Facts(String name, String realName, int measure)
        {
            this.name = name;
            this.realName = realName;
            this.measure = measure;
            fields = new List<Field>();
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public String RealName
        {
            get { return realName; }
            set { realName = value; }
        }
        public int Measure
        {
            get { return measure; }
            set { measure = value; }
        }
        public List<Field> FieldsList
        {
            get { return fields; }
        }
        public void AddField(Field field)
        {
            fields.Add(field);
        }
        public Field GetField(int id)
        {
            return fields[id];
        }
    }
}
