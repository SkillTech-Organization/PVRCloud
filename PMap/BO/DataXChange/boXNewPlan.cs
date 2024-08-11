using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PMapCore.BO.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO.DataXChange
{
    public class boXNewPlan : boXBase
    {
        public enum EStatus
        {
            [Description("OK")]
            OK,
            [Description("VALIDATIONERROR")]
            VALIDATIONERROR,
            [Description("ERROR")]
            ERROR,
            [Description("EXCEPTION")]
            EXCEPTION,
            [Description("WARNING")]
            WARNING
        };

        [DisplayNameAttributeX(Name = "Új terv ID", Order = 1)]
        public int PLN_ID { get; set; }

        [DisplayNameAttributeX(Name = "Geokódolás miatt kihagyott lerakók", Order = 2)]
        public List<boDepot> lstDepWithoutGeoCoding { get; set; }



        [DisplayNameAttributeX(Name = "Státusz", Order = 100000)]
        public EStatus Status { get; set; }

        [DisplayNameAttributeX(Name = "Üzenet", Order = 100001)]
        public string ErrMessage { get; set; }

        public boXNewPlan()
        {
            Status = EStatus.OK;
            PLN_ID = -1;
        }
    }
}
