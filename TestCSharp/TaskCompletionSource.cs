using System.Threading.Tasks;

class TCS
{
    void M()
    {
        TaskCompletionSource<int> tcs = null;

        tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously.ToString());
        tcs = new TaskCompletionSource<int>((int)TaskCreationOptions.RunContinuationsAsynchronously);
        var validEnum = TaskCreationOptions.RunContinuationsAsynchronously;
        tcs = new TaskCompletionSource<int>(validEnum);

        tcs = new TaskCompletionSource<int>(TaskContinuationOptions.RunContinuationsAsynchronously); // Invalid
        tcs = new TaskCompletionSource<int>(TaskContinuationOptions.RunContinuationsAsynchronously.ToString());
        tcs = new TaskCompletionSource<int>((int)TaskCreationOptions.RunContinuationsAsynchronously);
        var invalidEnum = TaskContinuationOptions.RunContinuationsAsynchronously;
        tcs = new TaskCompletionSource<int>(invalidEnum); // Invalid
    }
}