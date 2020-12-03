mergeInto(LibraryManager.library, {

  _OnGameStarted: function () {
    OnGameStarted();
  },
  
 _OnGameStopped: function () {
    OnGameStopped();
  },
  
 _ExitFullScreen: function () {
    ExitFullScreen();
  },
  
});