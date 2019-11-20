Imports System.Threading.Tasks

Class T
    Sub M()
        Dim tcs As TaskCompletionSource(Of Integer)

        tcs = New TaskCompletionSource(Of Integer)(TaskCreationOptions.RunContinuationsAsynchronously)
        tcs = New TaskCompletionSource(Of Integer)(TaskCreationOptions.RunContinuationsAsynchronously.ToString())
        tcs = New TaskCompletionSource(Of Integer)(CInt(TaskCreationOptions.RunContinuationsAsynchronously))
        Dim validEnum As TaskCreationOptions = TaskCreationOptions.RunContinuationsAsynchronously
        tcs = New TaskCompletionSource(Of Integer)(validEnum)

        tcs = New TaskCompletionSource(Of Integer)(TaskContinuationOptions.RunContinuationsAsynchronously) ' Invalid
        tcs = New TaskCompletionSource(Of Integer)(TaskContinuationOptions.RunContinuationsAsynchronously.ToString())
        tcs = New TaskCompletionSource(Of Integer)(CInt(TaskContinuationOptions.RunContinuationsAsynchronously))
        Dim invalidEnum As TaskContinuationOptions = TaskContinuationOptions.RunContinuationsAsynchronously
        tcs = New TaskCompletionSource(Of Integer)(invalidEnum) ' Invalid

    End Sub
End Class