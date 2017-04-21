using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "ConjugationRulesInstructions")]
    public class ConjugationRulesInstruction : NotaDbObject<ConjugationRulesInstruction>
    {
        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public int ConjugationRuleId { get; set; }

        [Column(CanBeNull = false)]
        public VerbType VerbType { get; set; }

        [Column(CanBeNull = false)]
        public int PersonId { get; set; }

        [Column(CanBeNull = true)]
        public string Suffix { get; set; }

        #endregion

        #region NotaDbObject Implementation

        public bool DbCompare(ConjugationRulesInstruction other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
