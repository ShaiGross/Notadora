using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "VerbsConjugationRules")]
    public class VerbsConjugationRule_TEMP : NotaDbObject<VerbsConjugationRule_TEMP>
    {
        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public int VerbId { get; set; }

        [Column(CanBeNull = false)]
        public int ConjugationRuleId { get; set; }

        public string ConjugationData { get; set; }

        #endregion

        #region NotaDbObject Implementation

        public bool DbCompare(VerbsConjugationRule_TEMP other)
        {
            return (VerbId == other.VerbId && ConjugationRuleId == other.ConjugationRuleId);
        }

        #endregion
    }
}
