using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LARP.Science.Database;

namespace LARP.Science.Operation
{
    public abstract class OperationInfo
    {
        protected readonly Character Patient;

        public abstract void PerformOperation();
    }
}
