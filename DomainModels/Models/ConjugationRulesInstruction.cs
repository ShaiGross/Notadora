using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "ConjugationRulesInstructions")]
    public class ConjugationRulesInstruction
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(CanBeNull = false)]
        public int ConjugationRuleId;

        [Column(CanBeNull = false)]
        public VerbType VerbType;

        [Column(CanBeNull = false)]
        public int PersonId;

        [Column(CanBeNull = true)]
        public string Suffix;
    }
}
