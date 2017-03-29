using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
    public bool isStatic = false;
    public bool ground = false;
    public float explosionTime;
    public GameObject mainController;
    public Object explosionSphere;

    // Use this for initialization
    void Start () {
        mainController = GameObject.Find("SplitScreenGameManager");
    }

    // Update is called once per frame
    void Update() {
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
            explosionTime = mainController.GetComponent<GameController>().currentTime() + SplitScreenModel.bombExplosionFuse;
        }
        if (mainController.GetComponent<GameController>().currentTime() >= explosionTime && ground && !mainController.GetComponent<GameController>().isCentralCannonUp) { 
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, SplitScreenModel.bombExplosionRadius);
            foreach (Collider hit in colliders)
                {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                rb.AddExplosionForce(SplitScreenModel.bombStartPower * SplitScreenModel.getMultiplier(mainController.GetComponent<GameController>().currentTime()) , explosionPos, SplitScreenModel.bombExplosionRadius, 1F);
                }
            mainController.GetComponent<GameController>().inGameObjects.Remove(gameObject);
            mainController.GetComponent<GameController>().inGameObjects.Add((GameObject)Instantiate(explosionSphere, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity));
            Destroy(gameObject);
        }
    }
}
