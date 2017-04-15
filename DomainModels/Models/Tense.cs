using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    public enum Times
    {
        None = -1,
        Past,
        Present,
        Conditional,
        Future
    }

    [Table(Name = "Tenses")]
    public class Tense : IComparable<Tense>
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(CanBeNull = false)]
        public string Name;

        [Column(CanBeNull = false)]
        public string Description;

        public Times TimeReference;

        [Column(CanBeNull = false)]
        public int RugularConjugationRuleId;

        [Column(CanBeNull = true)]
        public int PersonsCount;

        public List<ConjugationRule> IrregularConjugationRules { get; set; }

        public ConjugationRule RegularConjugationRule { get; set; }

        #region Icomparable Implementation

        public int CompareTo(Tense other)
        {            
            return Math.Sign((int)other.TimeReference - (int)this.TimeReference);
        }

        #endregion
    }
}
