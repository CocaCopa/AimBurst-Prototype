namespace CocaCopa.SaveSystem.Unity {
    internal enum SaveSystemInitPhase {
        AfterAssembliesLoaded = 0,
        BeforeSplashScreen = 1,
        SubsystemRegistration = 2,
        BeforeSceneLoad = 3,
        AfterSceneLoad = 4,
    }
}