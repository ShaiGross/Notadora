using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL.Models
{
    [Table(Name = "Tenses")]
    public class TensePerson : NotaDbObject<TensePerson>
    {
        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public int TenseId { get; set; }

        [Column(CanBeNull = false)]
        public int PersonId { get; set; }

        #endregion
        
        #region NotaDbObject Implementation

        public bool DbCompare(TensePerson other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
