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
        private string description;

        [DataMember]
        private int rugularConjugationRuleId;

        [DataMember]
        private List<PeronInfo> persons;

        [DataMember]
        private List<int> irregularConjugationRulesIds;

        #endregion

        #region Ctor

        public TenseInfo(int id,
                         string desc,
                         int regularConjRuleId,
                         List<int> irregularConjRulesIds,
                         List<PeronInfo> persons)
        {
            this.id = id;
            this.description = desc;
            this.rugularConjugationRuleId = regularConjRuleId;
            this.irregularConjugationRulesIds = irregularConjRulesIds;
            this.persons = persons;            
        }

        #endregion
    }
}
