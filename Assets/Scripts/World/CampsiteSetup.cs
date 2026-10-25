using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Constructs the campsite scene matching the reference image layout:
    /// Orange tent, campfire fire pit, gear tripod, backpack, snowy pine trees, and player character.
    /// </summary>
    public class CampsiteSetup : MonoBehaviour
    {
        [Header("Campsite Objects")]
        [SerializeField] private GameObject tentPrefab;
        [SerializeField] private GameObject campfirePrefab;
        [SerializeField] private GameObject tripodPrefab;
        [SerializeField] private GameObject backpackPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] treePrefabs;

        [Header("Campsite Placement Positions")]
        [SerializeField] private Vector3 tentOffset = new Vector3(2.0f, 0.8f, 0f);
        [SerializeField] private Vector3 campfireOffset = new Vector3(-1.2f, -0.4f, 0f);
        [SerializeField] private Vector3 tripodOffset = new Vector3(3.4f, 1.2f, 0f);
        [SerializeField] private Vector3 backpackOffset = new Vector3(2.5f, -0.4f, 0f);
        [SerializeField] private Vector3 playerStartOffset = new Vector3(-0.3f, -1.2f, 0f);

        private void Start()
        {
            BuildCampsiteScene();
        }

        [ContextMenu("Build Campsite Scene")]
        public void BuildCampsiteScene()
        {
            if (tentPrefab != null)
                Instantiate(tentPrefab, transform.position + tentOffset, Quaternion.identity, transform);

            if (campfirePrefab != null)
                Instantiate(campfirePrefab, transform.position + campfireOffset, Quaternion.identity, transform);

            if (tripodPrefab != null)
                Instantiate(tripodPrefab, transform.position + tripodOffset, Quaternion.identity, transform);

            if (backpackPrefab != null)
                Instantiate(backpackPrefab, transform.position + backpackOffset, Quaternion.identity, transform);

            if (playerPrefab != null)
                Instantiate(playerPrefab, transform.position + playerStartOffset, Quaternion.identity);
        }
    }
}
