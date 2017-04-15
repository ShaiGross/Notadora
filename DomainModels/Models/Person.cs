using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    public enum PersonFormality
    {
        Formal,
        Informal
    }

    public enum PersonPlurality
    {
        Single,
        Plural
    }

    public enum PersonGender
    {
        None,
        Masculine,
        Feminine,
    }

    public enum PersonOrder
    {
        None,
        First,
        Second,
        Third
    }

    [Table(Name = "Persons")]
    public class Person
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(CanBeNull = false)]
        public string SpanishExpression;        

        [Column(CanBeNull = false)]
        public string Description;

        [Column(DbType = "INT")]
        public PersonPlurality Plurality;

        [Column(DbType = "INT")]
        public PersonFormality Formality;

        [Column(DbType = "INT")]
        public PersonGender Gender;

        [Column(DbType = "INT")]
        public PersonOrder Order;

        public int GetIndex()
        {
            if (Order == PersonOrder.None)
                return 0;

            var pluralityValue = (Plurality == PersonPlurality.Single) ? 0 : 3;
            var orderValue = (Order == PersonOrder.First) ? 0 : (Order == PersonOrder.Second) ? 1 : 2;

            return pluralityValue + orderValue;
        }
    }
}
