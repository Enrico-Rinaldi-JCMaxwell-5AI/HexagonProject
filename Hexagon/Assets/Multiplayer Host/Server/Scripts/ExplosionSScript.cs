using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSScript : MonoBehaviour {

    public bool isStatic = false;
    public bool ground = false;
    public float explosionTime;
    public GameObject mainController;
    public Object explosionSphere;

    // Use this for initialization
    void Start()
    {
        mainController = GameObject.Find("HostGameManager(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.45f && !ground)
        {
            if (isStatic)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            }
            else
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
            GetComponent<Transform>().position = new Vector3(transform.position.x, 0.4f, transform.position.z);
            ground = true;
            explosionTime = mainController.GetComponent<ServerGameController>().model.gameTime + SplitScreenModel.bombExplosionFuse;
        }
        if (mainController.GetComponent<ServerGameController>().model.gameTime >= explosionTime && ground && !mainController.GetComponent<ServerGameController>().isCentralCannonUp)
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, SplitScreenModel.bombExplosionRadius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(SplitScreenModel.bombStartPower * SplitScreenModel.getMultiplier(mainController.GetComponent<ServerGameController>().model.gameTime), explosionPos, SplitScreenModel.bombExplosionRadius, 1F);
            }
            mainController.GetComponent<ServerGameController>().allInGameObjects.Remove(gameObject);
            mainController.GetComponent<ServerGameController>().nmanager.sendDestroyObject(System.Int32.Parse(gameObject.name));
            //mainController.GetComponent<ServerGameController>().inGameObjects.Add((GameObject)Instantiate(explosionSphere, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity));
            Destroy(gameObject);
        }
    }
}
