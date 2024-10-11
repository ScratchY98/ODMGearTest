using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GasBoost : MonoBehaviour
{
    [SerializeField] private float boostStrength;
    [SerializeField] private ODM ODMleft;
    [SerializeField] private ODM ODMright;
    [SerializeField] private KeyCode gasBoost;
    [SerializeField] private bool canBoost;
    [SerializeField] private float delay;
    [SerializeField] private ParticleSystem boostParticle;
    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(gasBoost))// && canBoost)
            DoGasBoost();
    }

    private void DoGasBoost()
    {
        canBoost = false;

        // Variables pour stocker les positions
        Vector3 pointA = ODMleft.joint ? ODMleft.ODMGearPoint.position : Vector3.zero;
        Vector3 pointB = ODMright.joint ? ODMright.ODMGearPoint.position : Vector3.zero;

        // Calcul du nombre d'attaches actives
        int activeCount = (ODMleft.joint ? 1 : 0) + (ODMright.joint ? 1 : 0);

        Vector3 boostPoint = Vector3.zero;

        // Utilisation du switch pour déterminer le point de boost
        switch (activeCount)
        {
            case 1:
                boostPoint = ODMleft.joint ? pointA : pointB;
                break;
            case 2:
                boostPoint = Vector3.Lerp(pointA, pointB, 0.5f);
                break;
            default:
                // Si aucun ODM n'est actif, on annule le boost
                return;
        }

        if (boostPoint != Vector3.zero)
            boostParticle.Play();

        // Appliquer la force de boost en direction du point calculé
        Vector3 direction = (boostPoint - transform.position).normalized;
        playerRb.AddForce(direction * boostStrength, ForceMode.Impulse);

        // Lancer le timer pour gérer le délai avant le prochain boost
        Invoke("Timer", delay);
    }

    private void Timer()
    {
        canBoost = true;
    }
}
