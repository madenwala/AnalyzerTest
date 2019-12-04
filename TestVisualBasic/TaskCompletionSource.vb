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
        tcs = New TaskCompletionSource(Of Integer)(NewProperty) ' Invalid

    End Sub
    Private myPropery As TaskContinuationOptions
    Public Property NewProperty() As TaskContinuationOptions
        Get
            Return myPropery
        End Get
        Set(ByVal value As TaskContinuationOptions)
            myPropery = value
        End Set
    End Property
End Class