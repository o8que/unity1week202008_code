using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public sealed class TaskWait : Action
{
    public SharedVector3 input;
    private float timer;

    public override void OnStart() {
        input.Value = Vector3.zero;
        timer = 0f;
    }

    public override TaskStatus OnUpdate() {
        timer += Time.deltaTime;
        if (timer > 1f) {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
