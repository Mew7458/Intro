using UnityEngine;

namespace Intro.UnityPrototype.Units
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float moveSpeed = 8f;

        private UnitModel model;
        private System.Func<Vector2Int, Vector3> gridToWorld;

        public UnitModel Model => model;

        public void Bind(UnitModel boundModel, System.Func<Vector2Int, Vector3> gridToWorldFn)
        {
            model = boundModel;
            gridToWorld = gridToWorldFn;
            transform.position = gridToWorld(model.GridPosition);
            RefreshAliveState();
        }

        private void Update()
        {
            if (model == null || gridToWorld == null) return;

            var target = gridToWorld(model.GridPosition);
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
            RefreshAliveState();
        }

        private void RefreshAliveState()
        {
            if (visualRoot == null) return;
            visualRoot.gameObject.SetActive(model != null && model.IsAlive);
        }
    }
}
