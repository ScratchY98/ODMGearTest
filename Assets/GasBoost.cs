using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GasBoost : MonoBehaviour
{
    [SerializeField] private float boostStreight;
    [SerializeField] private ODM ODMa;
    [SerializeField] private ODM ODMb;
    [SerializeField] private KeyCode gasBoost;
    [SerializeField] private bool canBoost;
    [SerializeField] private float delay;
    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(gasBoost) && canBoost)
            DoGasBoost();
    }

    private void DoGasBoost()
    {
        canBoost = false;
        if (!ODMa.joint && !ODMb.joint)
            return;

        bool isA = ODMa.joint && !ODMb.joint;
        bool isB = !ODMa.joint && ODMb.joint;
        bool isAll = ODMa.joint && ODMb.joint;
        Vector3 point = Vector3.zero;

        if (isA)
            point = ODMa.ODMGearPoint.position;
        else if (isB)
            point = ODMb.ODMGearPoint.position;
        else if (isAll)
            point = Vector3.Lerp(ODMa.ODMGearPoint.position, ODMb.ODMGearPoint.position, 0.5f);

        Vector3 direction = (point - transform.position).normalized;
        playerRb.AddForce(direction * boostStreight, ForceMode.Impulse);

        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(delay);
        canBoost = true;
    }
}
