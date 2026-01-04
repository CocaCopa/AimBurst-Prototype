using System;
using System.Threading.Tasks;

public static class TaskExtensions {
    public static async void Forget(this Task task) {
        try {
            await task;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
}
