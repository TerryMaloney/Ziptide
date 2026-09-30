using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Gameplay
{
    public partial class JobDirector
    {
        private readonly Dictionary<RepairableMachine, System.Action<RepairStage>> _missionMachineHandlers =
            new Dictionary<RepairableMachine, System.Action<RepairStage>>();
        private MissionCheckpointSession _mission;
        private SaveSystem _missionSaveOwner;
        private bool _missionConfigured;
        private bool _missionSavePending;
        private bool _missionCommitPending;
        private float _nextMissionRetry;
        public bool UsesMissionCheckpoints => _missionConfigured;

        private void InitializeMissionCheckpoint()
        {
            if (worldPack.jobs == null) return;
            foreach (var job in worldPack.jobs)
                if (job != null && job.checkpointRevision > 0) _missionConfigured = true;
            if (!_missionConfigured) return;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (!MissionCheckpointSession.TryOpen(profile, gameObject.scene.name, worldPack, out _mission, out string error))
            {
                Debug.LogWarning("ZIPTIDE: MISSION_RESTORE_BLOCKED world=" + gameObject.scene.name + " reason=" + error);
                return;
            }
            _runtime = _mission.Runtime;
            _missionSaveOwner = SaveSystem.Instance;
            _missionSaveOwner.BeforeSave += PrepareMissionForSave;
            Debug.Log("ZIPTIDE: MISSION_RESTORED world=" + gameObject.scene.name + " step=" + _runtime.CurrentStepIndex + " complete=" + _runtime.IsComplete);
            PersistMissionCheckpoint();
        }

        private bool MissionProfileIsCurrent()
        {
            if (_mission == null) return false;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (_mission.MatchesProfile(profile)) return true;
            // New Game/Load may replace the profile before the old scene unloads. Never write the
            // departing scene's counters into that replacement profile or fall back to legacy payout.
            _mission = null; _missionCommitPending = _missionSavePending = false;
            Debug.LogWarning("ZIPTIDE: MISSION_PROFILE_REPLACED world=" + gameObject.scene.name);
            return false;
        }

        public bool TryCollectPlacement(string placementId)
        {
            if (!MissionProfileIsCurrent() || !_mission.Collect(placementId)) return false;
            PersistMissionCheckpoint();
            return true;
        }

        private void BindMissionMachine(RepairableMachine machine)
        {
            if (!_missionConfigured || _mission == null || machine == null || _missionMachineHandlers.ContainsKey(machine)) return;
            if (!machine.RestoreStage((RepairStage)_mission.RepairStageFor(machine.MachineId)))
                Debug.LogWarning("ZIPTIDE: MISSION_MACHINE_RESTORE_FAIL id=" + machine.MachineId);
            System.Action<RepairStage> handler = stage =>
            {
                if (!MissionProfileIsCurrent()) return;
                _mission.SetRepairStage(machine.MachineId, (int)stage);
                PersistMissionCheckpoint();
            };
            _missionMachineHandlers.Add(machine, handler);
            machine.StageChanged += handler;
        }

        // Queue after each interaction. LateUpdate runs after the repair/pickup event listeners,
        // so their tutorial/clarity flags are included in the same disk snapshot.
        private void PersistMissionCheckpoint()
        {
            if (_missionConfigured && MissionProfileIsCurrent()) _missionCommitPending = true;
        }

        private void PrepareMissionForSave()
        {
            if (!_missionConfigured || !MissionProfileIsCurrent()) return;
            if (!_mission.TryCommit(out bool changed, out string error))
                throw new System.InvalidOperationException("MISSION_COMMIT_FAIL " + error);
            _missionCommitPending = false;
            _missionSavePending |= changed;
        }

        private void LateUpdate()
        {
            if ((!_missionSavePending && !_missionCommitPending) || Time.unscaledTime < _nextMissionRetry || !MissionProfileIsCurrent()) return;
            try
            {
                PrepareMissionForSave();
                if (!_missionSavePending) return;
                if (_missionSaveOwner.TrySave())
                {
                    _missionSavePending = false;
                    Debug.Log("ZIPTIDE: MISSION_CHECKPOINT_SAVED world=" + gameObject.scene.name + " complete=" + _runtime.IsComplete);
                }
            }
            catch (System.Exception error)
            {
                _missionCommitPending = true;
                Debug.LogWarning("ZIPTIDE: MISSION_COMMIT_FAIL reason=" + error.Message);
            }
            _nextMissionRetry = Time.unscaledTime + 5f;
        }

        private void UnbindMissionCheckpoint()
        {
            if (_missionSaveOwner != null) _missionSaveOwner.BeforeSave -= PrepareMissionForSave;
            foreach (var binding in _missionMachineHandlers)
                if (binding.Key != null) binding.Key.StageChanged -= binding.Value;
            _missionMachineHandlers.Clear();
        }
    }
}
