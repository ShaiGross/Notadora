using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    public enum ConjugationRuleType
    {
        Independent,
        NewStemDependent,
        NewInfDependent,
        SpecialConjugation
    }

    [Table(Name = "ConjugationRules")]
    public class ConjugationRule : IComparable<ConjugationRule>
    {
        #region Props

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id;

        [Column(CanBeNull = false)]
        public string Name;

        [Column(CanBeNull = false)]
        public string Description;

        [Column(CanBeNull = false)]
        public int TenseId;

        [Column(DbType = "INT")]
        public ConjugationRuleType Type;

        [Column(DbType = "INT", CanBeNull = false)]
        public bool IsRegular;

        [Column(CanBeNull = false)]
        public int PersonCount;

        internal List<Person> Persons;

        #endregion

        #region IComparable Implementation

        public int CompareTo(ConjugationRule other)
        {
            var personsDiff = this.PersonCount - other.PersonCount;

            if (personsDiff == 0)
            {
                if (this.Type == ConjugationRuleType.SpecialConjugation && other.Type != ConjugationRuleType.SpecialConjugation)
                    return 1;
                else if (this.Type != ConjugationRuleType.SpecialConjugation && other.Type == ConjugationRuleType.SpecialConjugation)
                    return -1;
                else return 0;          
            }

            return personsDiff / Math.Abs(personsDiff);
        }

        #endregion    
    }

}
