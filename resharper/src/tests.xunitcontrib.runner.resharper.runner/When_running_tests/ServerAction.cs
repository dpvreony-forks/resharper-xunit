namespace XunitContrib.Runner.ReSharper.RemoteRunner.Tests.When_running_tests
{
    public enum ServerAction
    {
        TaskStarting,
        TaskProgress,
        TaskError,
        TaskOutput,
        TaskFinished,
        TaskDuration,
        TaskExplain,
        TaskException,
        CreateDynamicElement
    }
}