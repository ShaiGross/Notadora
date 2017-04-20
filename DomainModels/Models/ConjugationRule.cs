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
        OffsetDepndent,
        NewStemDependent,
        NewInfDependent,
        SpecialConjugation
    }

    [Table(Name = "ConjugationRules")]
    public class ConjugationRule : NotaDbObject<ConjugationRule>, IComparable<ConjugationRule>
    {
        #region Properties

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; }

        [Column(CanBeNull = false)]
        public int TenseId { get; set; }

        [Column(DbType = "INT")]
        public ConjugationRuleType Type { get; set; }

        [Column(DbType = "BIT", CanBeNull = false)]
        public bool IsRegular { get; set; }

        [Column(CanBeNull = false)]
        public int PersonCount { get; set; }

        internal List<Person> Persons { get; set; }

        #endregion

        #region NotaDbObject Implementation

        public bool DbCompare(ConjugationRule other)
        {
            throw new NotImplementedException();
        }

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
