using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common
{

    public class NoMappedTableNameException : Exception
    {
        public NoMappedTableNameException()
        {
        }
    }
    
    
    public class DuplicatedDEP_CODEException : Exception
    {
        public DuplicatedDEP_CODEException()
        {
        }

        public DuplicatedDEP_CODEException(string message)
            : base(message)
        {
        }

        public DuplicatedDEP_CODEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedWHS_CODEException : Exception
    {
        public DuplicatedWHS_CODEException()
        {
        }

        public DuplicatedWHS_CODEException(string message)
            : base(message)
        {
        }

        public DuplicatedWHS_CODEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedZIP_NUMException : Exception
    {
        public DuplicatedZIP_NUMException()
        {
        }

        public DuplicatedZIP_NUMException(string message)
            : base(message)
        {
        }

        public DuplicatedZIP_NUMException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedTRK_CODEException : Exception
    {
        public DuplicatedTRK_CODEException()
        {
        }

        public DuplicatedTRK_CODEException(string message)
            : base(message)
        {
        }

        public DuplicatedTRK_CODEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedPLN_NAMEException : Exception
    {
        public DuplicatedPLN_NAMEException()
        {
        }

        public DuplicatedPLN_NAMEException(string message)
            : base(message)
        {
        }

        public DuplicatedPLN_NAMEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedCTP_CODEException : Exception
    {
        public DuplicatedCTP_CODEException()
        {
        }

        public DuplicatedCTP_CODEException(string message)
            : base(message)
        {
        }

        public DuplicatedCTP_CODEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedCRR_CODEException : Exception
    {
        public DuplicatedCRR_CODEException()
        {
        }

        public DuplicatedCRR_CODEException(string message)
            : base(message)
        {
        }

        public DuplicatedCRR_CODEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedOTP_CODEException : Exception
    {
        public DuplicatedOTP_CODEException()
        {
        }

        public DuplicatedOTP_CODEException(string message)
            : base(message)
        {
        }

        public DuplicatedOTP_CODEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicatedMPO_MPORDERException : Exception
    {
        public DuplicatedMPO_MPORDERException()
        {
        }

        public DuplicatedMPO_MPORDERException(string message)
            : base(message)
        {
        }

        public DuplicatedMPO_MPORDERException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
