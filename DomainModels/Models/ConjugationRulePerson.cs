using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "ConjugationRulePersons")]
    public class ConjugationRulePerson : NotaDbObject<ConjugationRulePerson>
    {
        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public int ConjugationRuleId { get; set; }

        [Column(CanBeNull = false)]
        public int PersonId { get; set; }

        #endregion

        public bool DbCompare(ConjugationRulePerson other)
        {
            throw new NotImplementedException();
        }
    }
}
