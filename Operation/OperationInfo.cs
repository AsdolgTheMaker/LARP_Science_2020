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

        public void Execute()
        {
            OnStart();
            bool isSuccessful = Process().Result;
            if (isSuccessful) OnSuccess(); else OnFail();
            OnFinished();
        }

        protected abstract Task<bool> Process();
        protected abstract void OnStart();
        protected abstract void OnFail();
        protected abstract void OnSuccess();
        protected abstract void OnFinished();

        public OperationInfo()
            => Patient = Controller.SelectedCharacter;
    }
}