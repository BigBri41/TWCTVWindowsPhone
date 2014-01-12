namespace Caliburn.Micro
{
    using System;

    public interface IResult
    {
        event EventHandler<ResultCompletionEventArgs> Completed;

        void Execute(ActionExecutionContext context);
    }
}

