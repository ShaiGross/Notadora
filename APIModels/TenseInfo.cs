using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace APIModels
{
    [DataContract]
    public class TenseInfo
    {
        #region Data Members

        [DataMember]
        private int id;

        [DataMember]
        private string name;

        [DataMember]
        private string description;

        [DataMember]
        private int rugularConjugationRuleId;

        [DataMember]
        private List<int> personsIds;

        [DataMember]
        private List<int> irregularConjugationRulesIds;

        #endregion

        #region Ctor

        public TenseInfo(int id,
                         string name,
                         string desc,
                         int regularConjRuleId,
                         List<int> irregularConjRulesIds,
                         List<int> personsIds)
        {
            this.id = id;
            this.name = name;
            this.description = desc;
            this.rugularConjugationRuleId = regularConjRuleId;
            this.irregularConjugationRulesIds = irregularConjRulesIds;
            this.personsIds = personsIds;            
        }

        #endregion
    }
}
