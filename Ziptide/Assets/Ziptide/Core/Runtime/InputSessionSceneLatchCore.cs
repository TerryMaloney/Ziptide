namespace Ziptide.Core
{
    /// <summary>
    /// Pure scene-generation latch for input-session consolidation. Unity PlayMode can preserve static
    /// event subscriptions between tests while re-running AfterSceneLoad bootstraps; both callbacks may
    /// observe the same scene activation. This latch makes that activation exactly-once without using
    /// frame timing or scene names.
    /// </summary>
    public struct InputSessionSceneLatchCore
    {
        private bool _hasScene;
        private int _sceneHandle;

        public bool HasScene => _hasScene;
        public int SceneHandle => _sceneHandle;

        public bool TryEnter(int sceneHandle)
        {
            if (_hasScene && _sceneHandle == sceneHandle) return false;
            _hasScene = true;
            _sceneHandle = sceneHandle;
            return true;
        }

        public void Reset()
        {
            _hasScene = false;
            _sceneHandle = 0;
        }
    }
}
