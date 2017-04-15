using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "VerbsConjugationRules")]
    public class VerbsConjugationRule
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(CanBeNull = false)]
        public int VerbId;

        [Column(CanBeNull = false)]
        public int ConjugationRuleId;

        public string ConjugationData;

    }
}
