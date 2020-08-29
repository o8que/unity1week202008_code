using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public sealed class TaskMove : Action
{
    public SharedVector3 input;
    private float timer;

    public override void OnStart() {
        float horizontal = GetInput(Random.value);
        float vertical = GetInput(Random.value);
        input.Value = new Vector3(horizontal, vertical);
        timer = 0f;
    }

    private float GetInput(float rand) {
        if (rand < 0.4f) {
            return -1f;
        } else if (rand < 0.8f) {
            return 1f;
        } else {
            return 0f;
        }
    }

    public override TaskStatus OnUpdate() {
        timer += Time.deltaTime;
        if (timer > 0.5f) {
            input.Value = Vector3.zero;
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
