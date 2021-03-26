using System.Runtime.CompilerServices;
using Fixie;

namespace SetupReference.IntegrationTests.Fixie
{
    public class DatabasePerClassConvention : Discovery, Execution
    {
        public DatabasePerClassConvention()
        {
            // All public methods are used by default so exclude the following by name
            Methods.Where(method => method.Name != nameof(ClassSetUp)
                                    && method.Name != nameof(ResetCheckpoint)
                                    && method.Name != nameof(SetUp)
                                    && method.Name != nameof(TearDown)
                                    && method.Name != nameof(ClassTearDown));
        }

        // This defines how each test class will be processed
        public void Execute(TestClass testClass)
        {
            var globalInstance = testClass.Construct();

            // Allow class to do 'once per class' set up e.g. intialise the database
            ClassSetUp(globalInstance);

            testClass.RunCases(@case =>
            {
                var instance = testClass.Construct();

                // Trigger a database reset before each method
                ResetCheckpoint(instance);

                // Perform test case/method level setup
                SetUp(instance);

                @case.Execute(instance);

                // Perform test case/method level tear down
                TearDown(instance);
            });

            // Allow class to do 'once per class' tear down e.g. destroy the database
            ClassTearDown(globalInstance);
        }

        // Use the current method name to look for same method in test class
        private static void ClassSetUp(object instance) => ExecuteLifecycleMethod(instance, GetCaller());

        private static void ResetCheckpoint(object instance) => ExecuteLifecycleMethod(instance, GetCaller());

        private static void ClassTearDown(object instance) => ExecuteLifecycleMethod(instance, GetCaller());

        private static void SetUp(object instance) => ExecuteLifecycleMethod(instance, GetCaller());

        private static void TearDown(object instance) => ExecuteLifecycleMethod(instance, GetCaller());

        private static void ExecuteLifecycleMethod(object instance, string methodName) => instance.GetType().GetMethod(methodName)?.Execute(instance);

        // https://stackoverflow.com/questions/38098736/using-nameof-to-get-name-of-current-method
        private static string GetCaller([CallerMemberName] string caller = null) => caller;
    }
}
