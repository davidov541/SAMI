namespace SAMI.Error
{
    internal interface IErrorManager
    {
        bool AnyErrorsAvailable
        {
            get;
        }

        SAMIUserException GetNextError();

        void AddError(SAMIUserException exp);
    }
}
