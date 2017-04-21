using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    #region Enums

    public enum Times
    {
        None = -1,
        Past,
        Present,
        Conditional,
        Future
    }
    
    #endregion

    [Table(Name = "Tenses")]
    public class Tense : NotaDbObject<Tense>, IComparable<Tense>
    {
        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; }

        public Times TimeReference { get; set; }

        [Column(CanBeNull = false)]
        public int RugularConjugationRuleId { get; set; }

        [Column(CanBeNull = true)]
        public int PersonsCount { get; set; }

        public List<ConjugationRule> IrregularConjugationRules { get; set; }

        public ConjugationRule RegularConjugationRule { get; set; }

        #endregion       

        #region NotaDbObject Implementation

        public bool DbCompare(Tense other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Icomparable Implementation

        public int CompareTo(Tense other)
        {
            return Math.Sign((int)other.TimeReference - (int)this.TimeReference);
        }

        #endregion
    }
}
