# EasyDependencyInjection
A quick and easy to use wrapper for DI

# How To Get Started!

  - Reference the DLL
  - Create the interface and concrete class you want to inject (ITestService, TestService)
  - Add this attribute to your concrete 
  - [BindTo(Scope.Singleton, typeof(ITestService))]
  - To Inject add this to the place you want to do so
  - private ITestService _testService = InjectionContainer.Resolve<ITestService>();
  - ANNNNNNDDDDDD your done!

