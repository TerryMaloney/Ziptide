using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Step: repair a count of machines (hands-on RepairableMachine stages: panel → part → power).
    /// </summary>
    public class RepairMachineCountStepDefinition : JobStepDefinition
    {
        [Tooltip("Machine id that must be repaired (blank = any machine counts).")]
        public string machineId = "";

        [Tooltip("Number of machines required to complete the step.")]
        public int count = 1;
    }
}
