using System.Collections.Generic;
using System.Linq;

namespace SAMI.Error
{
    internal class ErrorManager : IErrorManager
    {
        private static IErrorManager _instance;
        public static IErrorManager GetInstance()
        {
            if(_instance == null)
            {
                _instance = new ErrorManager();
            }
            return _instance;
        }

        private Queue<SAMIUserException> _newErrors;

        public bool AnyErrorsAvailable
        {
            get
            {
                lock(_newErrors)
                {
                    return _newErrors.Any();
                }
            }
        }

        public ErrorManager()
        {
            _newErrors = new Queue<SAMIUserException>();
        }

        public SAMIUserException GetNextError()
        {
            lock (_newErrors)
            {
                if (_newErrors.Any())
                {
                    return _newErrors.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        public void AddError(SAMIUserException exp)
        {
            lock(_newErrors)
            {
                _newErrors.Enqueue(exp);
            }
        }
    }
}
