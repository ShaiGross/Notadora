using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    #region Enums

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

    #endregion

    [Table(Name = "Persons")]
    public class Person : NotaDbObject<Person>
    {
        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string SpanishExpression { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; }

        [Column(DbType = "INT")]
        public PersonPlurality Plurality { get; set; }

        [Column(DbType = "INT")]
        public PersonFormality Formality { get; set; }

        [Column(DbType = "INT")]
        public PersonGender Gender { get; set; }

        [Column(DbType = "INT")]
        public PersonOrder Order { get; set; }

        #endregion

        #region Expresion-Bodied Members

        public int Index => GetIndex();

        #endregion

        #region Methods

        private int GetIndex()
        {
            if (Order == PersonOrder.None)
                return 0;

            var pluralityValue = (Plurality == PersonPlurality.Single) ? 0 : 3;
            var orderValue = (Order == PersonOrder.First) ? 0 : (Order == PersonOrder.Second) ? 1 : 2;

            return pluralityValue + orderValue;
        }

        #endregion

        #region NotaDbObject Implementation

        public bool DbCompare(Person other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
